// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks of shared memory (JSD-0041 and JSD-0042, phase F6): who may block, what bounds a wait, when an
/// asynchronous waiter settles, which compositions build the surface at all, and a block two agents
/// share.
/// </summary>
/// <remarks>
/// Each check compiles one or two scripts and invokes a sequence of entry points against one instance;
/// a composition's host surface answers <c>[[CanBlock]]</c> through <see cref="IJsHostAgentPolicy"/>
/// or does not answer.
/// </remarks>
internal static class SharedChecks
{
    private const string Caller = "js-slice-compiler://shared";

    private const string Prelude = "var i32 = new Int32Array(new SharedArrayBuffer(16));\n";

    /// <summary>Every shared-memory check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        AnAgentThatMayNotBlockRefusesWait(),
        AnAgentThatMayBlockWaitsAndTimesOut(),
        AWaitWithNoTimeoutEndsWithItsAllowance(),
        AnAsynchronousWaiterSettlesOnlyAtAHostDrain(),
        ANotifyWakesAnAsynchronousWaiterAtTheNextDrain(),
        TheSharedSurfaceAloneIsRefused(),
        ADeclinedSharedSurfaceBuildsNeitherGlobal(),
        TwoInstancesShareOneBlock(),
        OnlyAFixedLengthSharedBufferIsHandedOut(),
        ARealmThatDeclinedTheSharedSurfaceAdoptsNoBlock(),
    ];

    /// <summary>A host that does not say its agent may block is an event loop: <c>wait</c> refuses.</summary>
    private static (string, bool, string) AnAgentThatMayNotBlockRefusesWait() =>
        Check(
            "shared/s2/an-agent-that-may-not-block-refuses-wait",
            Prelude + "var r; try { r = Atomics.wait(i32, 0, 0, 0); } catch (e) { r = e.name; } r;",
            null,
            ["main"],
            "TypeError",
            "a realm whose host surface did not answer [[CanBlock]] refused Atomics.wait with a TypeError",
            canBlock: null);

    /// <summary>A host that says its agent may block gets a wait that times out.</summary>
    private static (string, bool, string) AnAgentThatMayBlockWaitsAndTimesOut() =>
        Check(
            "shared/s2/an-agent-that-may-block-waits-and-times-out",
            Prelude + "var t = Date.now(); var r = Atomics.wait(i32, 0, 0, 30); r + ':' + (Date.now() - t >= 25) + ':' + Atomics.wait(i32, 0, 1, 1000);",
            null,
            ["main"],
            "timed-out:true:not-equal",
            "the wait slept about its 30 ms and timed out, and a wait on a value the element did not hold answered not-equal at once",
            canBlock: true);

    /// <summary>A wait with no timeout is bounded by the operation's wall clock, never by the program.</summary>
    private static (string, bool, string) AWaitWithNoTimeoutEndsWithItsAllowance() =>
        Check(
            "shared/s2/a-wait-with-no-timeout-ends-with-its-allowance",
            Prelude + "Atomics.wait(i32, 0, 0);",
            null,
            ["main"],
            "ResourceExhaustion/AllowanceExhausted",
            "a wait no agent could notify ended when the operation's 300 ms wall clock was spent",
            canBlock: true,
            wallClock: 300);

    /// <summary>
    /// A <c>waitAsync</c> promise is not settled by the script that made it, and is settled by the host
    /// drain that follows, which waits for its deadline.
    /// </summary>
    private static (string, bool, string) AnAsynchronousWaiterSettlesOnlyAtAHostDrain() =>
        Check(
            "shared/s2/an-asynchronous-waiter-settles-only-at-a-host-drain",
            Prelude + "var seen = 'pending'; var r = Atomics.waitAsync(i32, 0, 0, 20); r.value.then(function (v) { seen = v; }); r.async + ':' + seen;",
            "seen",
            ["main", "read", JavaScriptProfile.DrainEntryPoint, "read"],
            "true:pending|pending|undefined|timed-out",
            "the script and a later one saw the promise pending; the host drain waited for its 20 ms deadline and settled it timed-out",
            canBlock: null);

    /// <summary>A notification marks an asynchronous waiter, and the next host drain settles it <c>"ok"</c>.</summary>
    private static (string, bool, string) ANotifyWakesAnAsynchronousWaiterAtTheNextDrain() =>
        Check(
            "shared/s2/a-notify-wakes-an-asynchronous-waiter-at-the-next-drain",
            Prelude + "var seen = 'pending'; Atomics.waitAsync(i32, 1, 0).value.then(function (v) { seen = v; }); Atomics.notify(i32, 1) + ':' + Atomics.notify(i32, 1) + ':' + seen;",
            "seen",
            ["main", JavaScriptProfile.DrainEntryPoint, "read"],
            "1:0:pending|undefined|ok",
            "the first notify woke the one waiter and the second none; the drain settled the promise ok",
            canBlock: null);

    /// <summary>A descriptor naming the shared surface without the binary one is refused.</summary>
    private static (string, bool, string) TheSharedSurfaceAloneIsRefused()
    {
        const string Name = "shared/s1/the-shared-surface-alone-is-refused";

        try
        {
            _ = JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.SharedManifest);
            return (Name, false, "a descriptor admitting broiler.javascript.shared alone was built");
        }
        catch (System.ArgumentException refused)
        {
            var named = refused.Message.Contains(
                Broiler.VM.Profile.JavaScript.Format.JsSurfaces.Binary, System.StringComparison.Ordinal);

            var both = JavaScriptProfile.DescriptorAdmitting(
                JavaScriptProfile.SharedManifest, JavaScriptProfile.BinaryManifest) is not null;

            return (
                Name,
                named && both,
                "the descriptor naming the shared surface alone was refused, naming the binary surface it needs, and one naming both was built");
        }
    }

    /// <summary>A composition that declined the shared surface builds neither global.</summary>
    private static (string, bool, string) ADeclinedSharedSurfaceBuildsNeitherGlobal() =>
        Check(
            "shared/s1/a-declined-shared-surface-builds-neither-global",
            "typeof SharedArrayBuffer + '/' + typeof Atomics + '/' + typeof ArrayBuffer;",
            null,
            ["main"],
            "undefined/undefined/function",
            "a realm admitting the binary surface and not the shared one built ArrayBuffer and neither SharedArrayBuffer nor Atomics",
            canBlock: null,
            surfaces: [JavaScriptProfile.BinaryManifest]);

    /// <summary>The script both agents of the agent checks run, one entry point per step.</summary>
    private static readonly JsScriptUnit[] AgentUnits =
    [
        new JsScriptUnit("make", "var sab = new SharedArrayBuffer(8); var a = new Int32Array(sab); Atomics.store(a, 0, 41); 'made';", SliceParseOptions.Script, false, Caller),
        new JsScriptUnit("add", "var b = new Int32Array(shared); Atomics.add(b, 0, 1) + ':' + (shared instanceof SharedArrayBuffer) + ':' + shared.byteLength;", SliceParseOptions.Script, false, Caller),
        new JsScriptUnit("read", "Atomics.load(a, 0);", SliceParseOptions.Script, false, Caller),
        new JsScriptUnit("wait", "Atomics.wait(a, 1, 0, 3000);", SliceParseOptions.Script, false, Caller),
        new JsScriptUnit("notify", "Atomics.notify(b, 1);", SliceParseOptions.Script, false, Caller),
        new JsScriptUnit("growable", "var g = new SharedArrayBuffer(4, { maxByteLength: 8 }); var plain = new ArrayBuffer(4); 'made';", SliceParseOptions.Script, false, Caller),
    ];

    /// <summary>
    /// Two instances on two runtimes - two agents, as JSD-0042 builds them - share one block: an
    /// <c>Atomics.add</c> through the second is seen by the first, and a notification from the second
    /// wakes the first, blocked on a thread of its own.
    /// </summary>
    private static (string, bool, string) TwoInstancesShareOneBlock()
    {
        const string Name = "shared/agents/two-instances-share-one-block";

        using var first = Agent.TryCreate(AgentUnits, []);
        using var second = Agent.TryCreate(AgentUnits, []);

        if (first.Failure is not null || second.Failure is not null)
        {
            return (Name, false, first.Failure ?? second.Failure!);
        }

        var answers = new System.Collections.Generic.List<string> { first.Invoke("make") };
        JsHostSharedBlock? block = null;

        first.Surface.Turn = realm => block = realm.ShareBlock(realm.GetProperty(realm.Global, "sab"));
        answers.Add(first.Invoke(JavaScriptProfile.TurnEntryPoint));

        second.Surface.Turn = realm => realm.SetProperty(realm.Global, "shared", realm.AdoptBlock(block!));
        answers.Add(second.Invoke(JavaScriptProfile.TurnEntryPoint));
        answers.Add(second.Invoke("add"));
        answers.Add(first.Invoke("read"));

        // THE FIRST AGENT BLOCKS ON A THREAD OF ITS OWN, and the second notifies until a waiter is
        // there to wake: a notification sent before the wait began wakes nothing and answers 0.
        var waiting = System.Threading.Tasks.Task.Run(() => first.Invoke("wait"));
        var woken = "0";

        for (var attempt = 0; attempt < 200 && woken == "0"; attempt++)
        {
            System.Threading.Thread.Sleep(10);
            woken = second.Invoke("notify");
        }

        answers.Add(woken);
        answers.Add(waiting.Wait(System.TimeSpan.FromSeconds(10)) ? waiting.Result : "still waiting");

        var joined = string.Join('|', answers);
        const string Expected = "made|undefined|undefined|41:true:8|42|1|ok";

        return (
            Name,
            string.Equals(joined, Expected, System.StringComparison.Ordinal),
            $"answered `{joined}` (`{Expected}` expected): the second agent's add was the first's, and its notify woke the first agent's blocking wait");
    }

    /// <summary>
    /// <see cref="JsHostRealm.ShareBlock"/> refuses an <c>ArrayBuffer</c> and a growable
    /// <c>SharedArrayBuffer</c>, each with a guest <c>TypeError</c>.
    /// </summary>
    private static (string, bool, string) OnlyAFixedLengthSharedBufferIsHandedOut()
    {
        const string Name = "shared/agents/only-a-fixed-length-shared-buffer-is-handed-out";

        using var agent = Agent.TryCreate(AgentUnits, []);

        if (agent.Failure is { } why)
        {
            return (Name, false, why);
        }

        var refusals = new System.Collections.Generic.List<string>();

        agent.Surface.Turn = realm =>
        {
            foreach (var name in new[] { "plain", "g" })
            {
                try
                {
                    _ = realm.ShareBlock(realm.GetProperty(realm.Global, name));
                    refusals.Add(name + " shared");
                }
                catch (JsHostThrowException)
                {
                    refusals.Add(name + " refused");
                }
            }
        };

        _ = agent.Invoke("growable");
        _ = agent.Invoke(JavaScriptProfile.TurnEntryPoint);

        var joined = string.Join(',', refusals);

        return (
            Name,
            string.Equals(joined, "plain refused,g refused", System.StringComparison.Ordinal),
            $"answered `{joined}`: an ArrayBuffer has no block to share, and a growable block replaces its storage when it grows");
    }

    /// <summary>A realm whose composition declined <c>broiler.javascript.shared</c> adopts no block.</summary>
    private static (string, bool, string) ARealmThatDeclinedTheSharedSurfaceAdoptsNoBlock()
    {
        const string Name = "shared/agents/a-realm-that-declined-the-shared-surface-adopts-no-block";

        // THE SECOND AGENT'S PROGRAM NAMES NO SHARED GLOBAL, or its own verification would refuse it
        // before the crossing could.
        using var first = Agent.TryCreate(AgentUnits, []);
        using var second = Agent.TryCreate(
            [new JsScriptUnit("idle", "'idle';", SliceParseOptions.Script, false, Caller)],
            [JavaScriptProfile.BinaryManifest]);

        if (first.Failure is not null || second.Failure is not null)
        {
            return (Name, false, first.Failure ?? second.Failure!);
        }

        JsHostSharedBlock? block = null;
        var answer = "adopted";

        _ = first.Invoke("make");
        first.Surface.Turn = realm => block = realm.ShareBlock(realm.GetProperty(realm.Global, "sab"));
        _ = first.Invoke(JavaScriptProfile.TurnEntryPoint);

        second.Surface.Turn = realm =>
        {
            try
            {
                _ = realm.AdoptBlock(block!);
            }
            catch (JsHostThrowException)
            {
                answer = "refused";
            }
        };

        _ = second.Invoke(JavaScriptProfile.TurnEntryPoint);

        return (
            Name,
            block is not null && answer == "refused",
            $"the block was {(block is null ? "not handed out" : "handed out")} and {answer} by a realm admitting the binary surface alone");
    }

    /// <summary>One agent of the agent checks: a runtime of its own, one instance, and the surface that runs its turns.</summary>
    private sealed class Agent : System.IDisposable
    {
        private VmRuntime? runtime;

        private VmVerifiedArtifact? artifact;

        private VmInstance? instance;

        private Agent()
        {
        }

        internal TurnSurface Surface { get; } = new();

        internal string? Failure { get; private set; }

        internal static Agent TryCreate(JsScriptUnit[] units, VmFeatureManifestId[] surfaces)
        {
            var agent = new Agent();
            var compiled = JsCompiler.Compile(units, [], new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                agent.Failure = "the source was refused";
                return agent;
            }

            var descriptor = JavaScriptProfile.DescriptorHostingRealms(agent.Surface, surfaces);
            var created = VmRuntime.Create(VmCatalog.CreateBuilder().Add(descriptor).Build(), Options(null));

            if (!created.TryGetRuntime(out agent.runtime))
            {
                agent.Failure = $"the runtime refused creation: {created.Outcome}/{created.Reason}";
                return agent;
            }

            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = agent.runtime.Verify(in artifactDescriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out agent.artifact))
            {
                agent.Failure = $"verification refused: {verified.Outcome}/{verified.Reason}";
                return agent;
            }

            var instantiated = agent.runtime.Instantiate(agent.artifact, System.Threading.CancellationToken.None);

            if (!instantiated.TryGetInstance(out agent.instance))
            {
                agent.Failure = $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
            }

            return agent;
        }

        internal string Invoke(string entry)
        {
            var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));
            var result = instance!.Invoke(in request, System.Threading.CancellationToken.None);

            return JavaScriptProfile.TryGetWideCompletion(in result, out var completion) ? completion.Value
                : JavaScriptProfile.TryGetUncaught(in result, out var uncaught) ? "uncaught " + uncaught.Message
                : $"{result.Outcome}/{result.Reason}";
        }

        public void Dispose()
        {
            instance?.Dispose();
            artifact?.Dispose();
            runtime?.Dispose();
        }
    }

    /// <summary>An embedder whose turn runs whatever the check hands it, and whose agent may block.</summary>
    private sealed class TurnSurface : IJsHostSurface, IJsHostAgentPolicy
    {
        internal System.Action<JsHostRealm>? Turn { get; set; }

        public bool CanBlock => true;

        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm) => Turn?.Invoke(realm);
    }

    /// <summary>
    /// Compiles <paramref name="main"/> and <paramref name="read"/>, invokes <paramref name="sequence"/>
    /// against one instance, and compares the answers joined with <c>|</c>.
    /// </summary>
    private static (string, bool, string) Check(
        string name,
        string main,
        string? read,
        string[] sequence,
        string expected,
        string meaning,
        bool? canBlock,
        ulong? wallClock = null,
        VmFeatureManifestId[]? surfaces = null)
    {
        JsScriptUnit[] units = read is null
            ? [new JsScriptUnit("main", main, SliceParseOptions.Script, false, Caller)]
            :
            [
                new JsScriptUnit("main", main, SliceParseOptions.Script, false, Caller),
                new JsScriptUnit("read", read, SliceParseOptions.Script, false, Caller),
            ];

        var compiled = JsCompiler.Compile(units, [], new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (name, false, "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        IJsHostSurface surface = canBlock is { } answer ? new BlockingSurface(answer) : new QuietSurface();
        var descriptor = JavaScriptProfile.DescriptorHostingRealms(surface, surfaces ?? []);
        var created = VmRuntime.Create(VmCatalog.CreateBuilder().Add(descriptor).Build(), Options(wallClock));

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
                        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));
                        var result = instance.Invoke(in request, System.Threading.CancellationToken.None);

                        answers.Add(
                            JavaScriptProfile.TryGetWideCompletion(in result, out var completion) ? completion.Value
                            : JavaScriptProfile.TryGetUncaught(in result, out var uncaught) ? "uncaught " + uncaught.Message
                            : $"{result.Outcome}/{result.Reason}");
                    }

                    var joined = string.Join('|', answers);

                    return (
                        name,
                        string.Equals(joined, expected, System.StringComparison.Ordinal),
                        $"answered `{joined}` (`{expected}` expected): {meaning}");
                }
            }
        }
    }

    /// <summary>The runtime options: the host-surface permission, default ceilings, and a wall clock when one is asked for.</summary>
    private static VmRuntimeCreationOptions Options(ulong? wallClock)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : dimension == VmBudgetDimension.WallClock && wallClock is { } milliseconds
                    ? VmCeilingSpec.Value(dimension, milliseconds)
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

    /// <summary>An embedder that installs nothing and says nothing about blocking.</summary>
    private sealed class QuietSurface : IJsHostSurface
    {
        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm)
        {
        }
    }

    /// <summary>An embedder that answers <c>[[CanBlock]]</c>.</summary>
    private sealed class BlockingSurface(bool canBlock) : IJsHostSurface, IJsHostAgentPolicy
    {
        public bool CanBlock => canBlock;

        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm)
        {
        }
    }
}
