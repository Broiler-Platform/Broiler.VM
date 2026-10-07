// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks of the host-drained finalization sweep (JSD-0029 D03-a, phase F4), through the scripted
/// eligibility seam the record's section 6 asks for.
/// </summary>
/// <remarks>
/// <para>
/// <b>No check here waits for the collector.</b> A target is marked collected through the internal
/// seam, by identity, while the program still holds it; what the checks hold the model to is where
/// the mark is read and what happens after, which no collection can change. The one check on the
/// production path asks the opposite question - a target that stays reachable is never reported,
/// however many collections run - which a collection cannot make fail either.
/// </para>
/// <para>
/// Each check compiles a program of two scripts, <c>main</c> and <c>read</c>, and invokes a sequence
/// of entry points against one instance; <c>#step-jobs</c> is stepped to the end by resuming its
/// parks. A realm gets <c>host.mark(object)</c>, the seam, and <c>host.drain()</c>, the drain an
/// embedder delegates to a guest, which must not sweep.
/// </para>
/// </remarks>
internal static class FinalizationChecks
{
    private const string Caller = "js-slice-compiler://finalization";

    /// <summary>The prelude every program opens with: a log and a registry that writes to it.</summary>
    private const string Prelude =
        "var log = []; var registry = new FinalizationRegistry(function (held) { log.push(held); });\n";

    /// <summary>Every finalization check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        ADrainDeliversOnceAndAScriptNever(),
        AStepDeliversInItsTurn(),
        TheDelegatedDrainNeverSweeps(),
        AnUnregisterBeforeTheJobPreventsTheCall(),
        AThrowingCallbackFaultsAndTheRestArriveLater(),
        AnInertCompositionNeverDelivers(),
        AReachableTargetIsNeverReported(),
    ];

    /// <summary>
    /// A marked registration's callback runs once, with its held value, in a <c>#drain-jobs</c> -
    /// not in the script that marked it, and not again in a second drain.
    /// </summary>
    private static (string, bool, string) ADrainDeliversOnceAndAScriptNever()
    {
        const string Name = "finalization/f4/a-drain-delivers-once-and-a-script-never";
        const string Main = Prelude + "var target = {}; registry.register(target, 'a'); host.mark(target); 'marked';";

        return Check(
            Name,
            Main,
            "log.join(',')",
            ["main", "read", JavaScriptProfile.DrainEntryPoint, "read", JavaScriptProfile.DrainEntryPoint, "read"],
            "marked||undefined|a|undefined|a",
            "the callback ran once, at the first drain, with its held value - not in the script that marked the target and not at the second drain");
    }

    /// <summary>A <c>#step-jobs</c> turn sweeps before its job, so a cleanup it queues is that turn's job.</summary>
    private static (string, bool, string) AStepDeliversInItsTurn()
    {
        const string Name = "finalization/f4/a-step-delivers-in-its-turn";
        const string Main = Prelude + "var target = {}; registry.register(target, 'a'); host.mark(target); 'marked';";

        return Check(
            Name,
            Main,
            "log.join(',')",
            ["main", JavaScriptProfile.StepEntryPoint, "read"],
            "marked||a",
            "a step over an empty queue swept, queued the cleanup and ran it in that turn");
    }

    /// <summary>
    /// The drain an embedder delegates to a guest sweeps nothing, whether a script calls it or a job
    /// a host drain is running does; the marks it was handed are read at the next host drain.
    /// </summary>
    private static (string, bool, string) TheDelegatedDrainNeverSweeps()
    {
        const string Name = "finalization/f4/the-delegated-drain-never-sweeps";
        const string Main = Prelude +
            "var first = {}, second = {}, inner = -1; registry.register(first, 'a'); registry.register(second, 'b');\n" +
            "host.mark(first); var ran = host.drain();\n" +
            "Promise.resolve().then(function () { host.mark(second); inner = host.drain(); });\n" +
            "'ran=' + ran + ' log=' + log.join(',');";

        return Check(
            Name,
            Main,
            "'inner=' + inner + ' log=' + log.join(',')",
            ["main", JavaScriptProfile.DrainEntryPoint, "read", JavaScriptProfile.DrainEntryPoint, "read"],
            "ran=0 log=|undefined|inner=1 log=a|undefined|inner=1 log=a,b",
            "a script's delegated drain ran nothing and swept nothing; inside the first host drain a job's delegated drain ran the one cleanup job that drain's sweep had already queued, delivering only what was marked before the sweep, and swept nothing itself; the second host drain delivered the rest");
    }

    /// <summary><c>unregister</c> between the sweep and the cleanup job prevents the call and answers true.</summary>
    private static (string, bool, string) AnUnregisterBeforeTheJobPreventsTheCall()
    {
        const string Name = "finalization/f4/an-unregister-before-the-job-prevents-the-call";
        const string Main = Prelude +
            "var target = {}, token = {}, removed; registry.register(target, 'a', token); host.mark(target);\n" +
            "Promise.resolve().then(function () { removed = registry.unregister(token); }); 'queued';";

        return Check(
            Name,
            Main,
            "'removed=' + removed + ' log=' + log.join(',')",
            ["main", JavaScriptProfile.StepEntryPoint, "read"],
            "queued||removed=true log=",
            "the step's sweep queued the cleanup behind the job already due; that job unregistered the marked registration, answering true, and the cleanup that ran next called nothing");
    }

    /// <summary>
    /// A callback that throws faults the drain as a throwing job does, and the registrations it had
    /// not reached arrive at the next drain.
    /// </summary>
    private static (string, bool, string) AThrowingCallbackFaultsAndTheRestArriveLater()
    {
        const string Name = "finalization/f4/a-throwing-callback-faults-and-the-rest-arrive-later";
        const string Main =
            "var log = []; var registry = new FinalizationRegistry(function (held) { log.push(held); if (held === 'x') throw new Error('boom'); });\n" +
            "var first = {}, second = {}; registry.register(first, 'x'); registry.register(second, 'y');\n" +
            "host.mark(first); host.mark(second); 'marked';";

        return Check(
            Name,
            Main,
            "log.join(',')",
            ["main", JavaScriptProfile.DrainEntryPoint, "read", JavaScriptProfile.DrainEntryPoint, "read"],
            "marked|uncaught Error: boom|x|undefined|x,y",
            "the throw faulted the first drain after its held value was delivered, and the second drain's sweep queued a fresh job for the registration left marked");
    }

    /// <summary>A composition that did not turn the sweep on delivers nothing, marks or not.</summary>
    private static (string, bool, string) AnInertCompositionNeverDelivers()
    {
        const string Name = "finalization/f4/an-inert-composition-never-delivers";
        const string Main = Prelude + "var target = {}; registry.register(target, 'a'); host.mark(target); 'marked';";

        return Check(
            Name,
            Main,
            "log.join(',')",
            ["main", JavaScriptProfile.DrainEntryPoint, "read"],
            "marked|undefined|",
            "a realm built by the hosting door, which does not sweep, ran no callback",
            sweeping: false);
    }

    /// <summary>
    /// On the production path, a target the program keeps reachable is never reported, however many
    /// collections run before the drains (JSD-0029 section 6's safety test).
    /// </summary>
    private static (string, bool, string) AReachableTargetIsNeverReported()
    {
        const string Name = "finalization/f4/a-reachable-target-is-never-reported";
        const string Main = Prelude + "var target = {}; registry.register(target, 'kept'); 'registered';";

        return Check(
            Name,
            Main,
            "log.join(',') + ':' + typeof target",
            ["main", "collect", JavaScriptProfile.DrainEntryPoint, "collect", JavaScriptProfile.DrainEntryPoint, "read"],
            "registered||undefined||undefined|:object",
            "two forced collections and two host drains reported nothing for a target the program still held");
    }

    /// <summary>
    /// Compiles <paramref name="main"/> and <paramref name="read"/>, invokes <paramref name="sequence"/>
    /// against one instance, and compares the answers joined with <c>|</c>.
    /// </summary>
    private static (string, bool, string) Check(
        string name, string main, string read, string[] sequence, string expected, string meaning, bool sweeping = true)
    {
        var compiled = JsCompiler.Compile(
            [
                new JsScriptUnit("main", main, SliceParseOptions.Script, false, Caller),
                new JsScriptUnit("read", read, SliceParseOptions.Script, false, Caller),
            ],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (name, false, "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        var surface = new FinalizationSurface();
        var descriptor = sweeping
            ? JavaScriptProfile.DescriptorSweepingFinalization(surface)
            : JavaScriptProfile.DescriptorHostingRealms(surface);

        var created = VmRuntime.Create(VmCatalog.CreateBuilder().Add(descriptor).Build(), Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return (name, false, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in artifactDescriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return (name, false, $"verification refused: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, System.Threading.CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return (name, false, $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}");
                }

                using (instance)
                {
                    var answers = new System.Collections.Generic.List<string>();

                    foreach (var entry in sequence)
                    {
                        if (entry == "collect")
                        {
                            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced, blocking: true);
                            System.GC.WaitForPendingFinalizers();
                            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced, blocking: true);
                            answers.Add(string.Empty);
                            continue;
                        }

                        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));
                        var result = instance.Invoke(in request, System.Threading.CancellationToken.None);

                        if (result.Outcome is not VmOutcome.Suspension || !result.TryGetSuspension(out var suspension))
                        {
                            answers.Add(Describe(in result));
                            continue;
                        }

                        // A STEP THAT PARKS IS RESUMED TO ITS END, each resumed turn sweeping before its job.
                        var resumed = runtime.Resume(suspension);

                        while (resumed.Outcome is VmOutcome.Suspension && resumed.TryGetSuspension(out var next))
                        {
                            resumed = runtime.Resume(next);
                        }

                        answers.Add(
                            resumed.TryGetPayload(out JsCompletion completion) ? completion.Value
                            : resumed.TryGetPayload(out JsUncaught uncaught) ? "uncaught " + uncaught.Message
                            : $"{resumed.Outcome}/{resumed.Reason}");
                    }

                    var answer = string.Join('|', answers);

                    return (
                        name,
                        string.Equals(answer, expected, System.StringComparison.Ordinal),
                        $"answered `{answer}` (`{expected}` expected): {meaning}");
                }
            }
        }
    }

    /// <summary>One invocation's answer: its completion value, or its uncaught error, or its outcome.</summary>
    private static string Describe(in VmInvocationResult result)
    {
        if (JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
        {
            return completion.Value;
        }

        if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
        {
            return "uncaught " + uncaught.Message;
        }

        return $"{result.Outcome}/{result.Reason}";
    }

    /// <summary>The runtime options: the host-surface permission, external suspension for stepping, and default ceilings.</summary>
    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var capabilities = ImmutableArray.Create(
            VmCapabilityRegistration.Value(
                JavaScriptProfile.HostSurfaceCapability,
                (VmBytes argument, out VmOpaqueRef answer) =>
                {
                    answer = default;
                    return VmHostCallOutcome.Completed;
                }));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Enabled,
            capabilities: capabilities);
    }

    /// <summary><c>JsHostRealm.MarkFinalizationTargetCollected</c>, which is internal to the profile.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MarkFinalizationTargetCollected")]
    private static extern void Mark(JsHostRealm realm, JsHostValue target);

    /// <summary>An embedder that gives a realm <c>host.mark</c> and <c>host.drain</c>.</summary>
    private sealed class FinalizationSurface : IJsHostSurface
    {
        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "mark",
                realm.NewMethod(
                    "mark",
                    (r, _, arguments) =>
                    {
                        Mark(r, arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);
                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "drain",
                realm.NewMethod("drain", (r, _, _) => JsHostValue.Number(r.DrainJobs())));

            realm.DefineValue(realm.Global, "host", host);
        }
    }
}
