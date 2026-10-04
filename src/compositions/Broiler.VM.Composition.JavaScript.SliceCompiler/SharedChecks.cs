// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks of shared memory in one agent (JSD-0041, phase F6's first slice): who may block, what bounds
/// a wait, when an asynchronous waiter settles, and which compositions build the surface at all.
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
