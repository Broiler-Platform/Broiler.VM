// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks that a <c>WeakMap</c> is weak in its keys and costs the collector one ordinary walk
/// (JSC-260).
/// </summary>
/// <remarks>
/// <para>
/// <b>The collection is forced from inside the run</b>, by a host method <c>collect()</c> that makes
/// a blocking, compacting collection of every generation and answers how long it took, so a check can
/// build a graph, collect while the graph is still reachable, and read the graph again.
/// </para>
/// <para>
/// <b>Weakness is observed through <c>WeakRef</c>s that are not read before the collection</b>: a
/// <c>WeakRef</c> is weak until its first <c>deref</c>, so counting the ones that answer
/// <c>undefined</c> afterwards counts what the collector freed. The counts are bounds, not exact
/// figures, because an operand-stack slot may still hold the last value a loop wrote.
/// </para>
/// </remarks>
internal static class WeakMapChecks
{
    private const string Caller = "js-slice-compiler://weak-map";

    /// <summary>Every weak-map check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        ADeepChainIsCollectedInOneWalk(),
        AValueDiesWithItsKey(),
        AValueOfACollectedMapIsDroppedFromItsKey(),
    ];

    /// <summary>
    /// The chain test262's <c>regress-1507322-deep-weakmap</c> builds: 99,999 keys, each the value
    /// stored under the one before.
    /// </summary>
    /// <remarks>
    /// Over the runtime's dependent-handle table one collection of this chain ran for minutes. The
    /// bound below is far above what one ordinary walk takes and far below what that took.
    /// </remarks>
    private static (string, bool, string) ADeepChainIsCollectedInOneWalk()
    {
        const string Name = "weak-map/jsc-260/a-deep-chain-is-collected-in-one-walk";

        const string Source =
            """
            var m = new WeakMap(), head = {};
            for (var key = head, i = 0; i < 99999; i++, key = m.get(key)) { m.set(key, {}); }
            var took = host.collect();
            var links = 0;
            for (key = head; key !== undefined; key = m.get(key)) { links++; }
            [links, took < 10000].join(',');
            """;

        if (!TryRun(Source, out var answer, out var why))
        {
            return (Name, false, why);
        }

        return (
            Name,
            answer == "100000,true",
            $"answered {answer} (100000,true expected): every link survives a full collection, " +
            "which finished within ten seconds");
    }

    /// <summary>A value that refers to its own key is collected with the key.</summary>
    private static (string, bool, string) AValueDiesWithItsKey()
    {
        const string Name = "weak-map/jsc-260/a-value-dies-with-its-key";

        const string Source =
            """
            var m = new WeakMap(), refs = [], kept = [];
            (function () {
                for (var i = 0; i < 1000; i++) {
                    var key = {}, value = { owner: key };
                    m.set(key, value);
                    refs.push(new WeakRef(value));
                    if (i % 10 === 0) kept.push(key);
                }
            })();
            host.collect();
            var freed = 0;
            for (var i = 0; i < refs.length; i++) { if (refs[i].deref() === undefined) freed++; }
            var held = 0;
            for (var j = 0; j < kept.length; j++) { if (m.get(kept[j]).owner === kept[j]) held++; }
            [freed >= 800, held].join(',');
            """;

        if (!TryRun(Source, out var answer, out var why))
        {
            return (Name, false, why);
        }

        return (
            Name,
            answer == "true,100",
            $"answered {answer} (true,100 expected): at least 800 of the 900 values whose keys " +
            "were dropped were collected although each refers to its key, and the 100 kept keys " +
            "still answer their values");
    }

    /// <summary>
    /// A live key drops the value of a map that has been collected, the next time it gains an entry.
    /// </summary>
    private static (string, bool, string) AValueOfACollectedMapIsDroppedFromItsKey()
    {
        const string Name = "weak-map/jsc-260/a-value-of-a-collected-map-is-dropped-from-its-key";

        const string Source =
            """
            var key = {}, ref;
            (function () {
                var gone = new WeakMap(), value = {};
                gone.set(key, value);
                ref = new WeakRef(value);
            })();
            host.collect();
            var a = new WeakMap(), b = new WeakMap();
            a.set(key, 1);
            b.set(key, 2);
            host.collect();
            [ref.deref() === undefined, a.get(key), b.get(key)].join(',');
            """;

        if (!TryRun(Source, out var answer, out var why))
        {
            return (Name, false, why);
        }

        return (
            Name,
            answer == "true,1,2",
            $"answered {answer} (true,1,2 expected): the collected map's value left the live key " +
            "when the key gained entries, and the live maps' entries are intact");
    }

    /// <summary>Compiles and runs <paramref name="source"/> with <c>host.collect</c> installed.</summary>
    private static bool TryRun(string source, out string answer, out string why)
    {
        answer = string.Empty;

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            why = "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());

            return false;
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(new CollectSurface()))
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            why = $"the runtime refused creation: {created.Outcome}/{created.Reason}";
            return false;
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                why = $"verification refused: {verified.Outcome}/{verified.Reason}";
                return false;
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, System.Threading.CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    why = $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                    return false;
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(
                        new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                    var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

                    if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                    {
                        why = $"the run answered {invoked.Outcome}/{invoked.Reason}";
                        return false;
                    }

                    answer = completion.Value;
                    why = string.Empty;
                    return true;
                }
            }
        }
    }

    /// <summary>The runtime options: the host-surface permission and every default ceiling.</summary>
    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        // THE PERMISSION, AND IT CARRIES NO TRAFFIC, as in CloneChecks.
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
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities);
    }

    /// <summary>An embedder that gives a realm <c>host.collect</c> and nothing else.</summary>
    private sealed class CollectSurface : IJsHostSurface
    {
        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "collect",
                realm.NewMethod(
                    "collect",
                    (_, _, _) =>
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        System.GC.Collect(
                            System.GC.MaxGeneration, System.GCCollectionMode.Forced, blocking: true, compacting: true);
                        System.GC.WaitForPendingFinalizers();
                        System.GC.Collect(
                            System.GC.MaxGeneration, System.GCCollectionMode.Forced, blocking: true, compacting: true);
                        return JsHostValue.Number(watch.Elapsed.TotalMilliseconds);
                    }));

            realm.DefineValue(realm.Global, "host", host);
        }
    }
}
