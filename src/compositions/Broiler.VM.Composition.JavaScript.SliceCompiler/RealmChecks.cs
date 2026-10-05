// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks of an engine that holds several realms (JSD-0030 SR-1, SR-2 and SR-7, phase F5): the
/// agent's Symbols shared, a function running in its own realm, the embedder told of a realm a guest
/// created, and the creation charged to the allowance.
/// </summary>
/// <remarks>
/// Each check compiles one script and invokes it against one instance of a runtime whose composition
/// installed a host surface; the surface gives every realm it is told of a <c>host</c> object whose
/// <c>told()</c> answers how many realms the surface has been told of so far.
/// </remarks>
internal static class RealmChecks
{
    private const string Caller = "js-slice-compiler://realms";

    /// <summary>Every realm check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        TwoRealmsShareTheAgentSymbols(),
        ABuiltInRunsInItsOwnRealm(),
        AFunctionCalledFromAnotherRealmRunsInItsOwn(),
        TheHostIsToldOfACreatedRealm(),
        ACreatedRealmIsCharged(),
        ADeclinedDynamicSurfaceHasNoShadowRealm(),
        TheShadowRealmSurfaceAloneIsRefused(),
    ];

    /// <summary>
    /// JSD-0030 section 6, case 5: a composition that declined the dynamic surface has no
    /// <c>ShadowRealm</c>, because there is no route by which one could evaluate anything.
    /// </summary>
    private static (string, bool, string) ADeclinedDynamicSurfaceHasNoShadowRealm() =>
        Check(
            "realms/sr3/a-declined-dynamic-surface-has-no-shadow-realm",
            "typeof ShadowRealm + '/' + ('ShadowRealm' in globalThis);",
            "undefined/false",
            "a realm whose composition admitted the binary and BigInt surfaces and not the dynamic one built no ShadowRealm",
            surfaces: [JavaScriptProfile.BinaryManifest, JavaScriptProfile.BigIntManifest]);

    /// <summary>JSD-0040: a descriptor naming the ShadowRealm surface without the dynamic one is refused.</summary>
    private static (string, bool, string) TheShadowRealmSurfaceAloneIsRefused()
    {
        const string Name = "realms/sr3/the-shadow-realm-surface-alone-is-refused";

        try
        {
            _ = JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.ShadowRealmManifest);
            return (Name, false, "a descriptor admitting broiler.javascript.shadowrealm alone was built");
        }
        catch (System.ArgumentException refused)
        {
            var named = refused.Message.Contains(Broiler.VM.Profile.JavaScript.Format.JsSurfaces.Dynamic, System.StringComparison.Ordinal);
            var both = JavaScriptProfile.DescriptorAdmitting(
                JavaScriptProfile.ShadowRealmManifest, JavaScriptProfile.DynamicManifest) is not null;

            return (
                Name,
                named && both,
                "the descriptor naming the ShadowRealm surface alone was refused, naming the dynamic surface it needs, and one naming both was built");
        }
    }

    /// <summary>SR-1: the well-known Symbols and the <c>Symbol.for</c> registry are the engine's.</summary>
    private static (string, bool, string) TwoRealmsShareTheAgentSymbols() =>
        Check(
            "realms/sr1/two-realms-share-the-agent-symbols",
            "var g = $262.createRealm().global;\n" +
            "[g.Symbol.iterator === Symbol.iterator, g.Symbol.asyncIterator === Symbol.asyncIterator,\n" +
            " g.Symbol.for('a') === Symbol.for('a'), Symbol.keyFor(g.Symbol.for('b')), g.Symbol === Symbol].join();",
            "true,true,true,b,false",
            "the well-known Symbols and the registry are one in both realms, and the Symbol constructor is each realm's own");

    /// <summary>SR-2: another realm's built-in runs in that realm, whoever calls it.</summary>
    /// <remarks>
    /// Built-ins rather than script functions, because the runtime these checks build registers no
    /// compiler for <c>eval</c>; the CLI fixture <c>runs/a-created-realm-is-a-realm-of-its-own.js</c>
    /// holds the same for script functions.
    /// </remarks>
    private static (string, bool, string) ABuiltInRunsInItsOwnRealm() =>
        Check(
            "realms/sr2/a-built-in-runs-in-its-own-realm",
            "var g = $262.createRealm().global; var thrown;\n" +
            "try { g.Array.prototype.map.call([], null); } catch (e) { thrown = e; }\n" +
            "[thrown instanceof g.TypeError, thrown instanceof TypeError,\n" +
            " Object.getPrototypeOf(g.Array.prototype.concat.call([])) === g.Array.prototype,\n" +
            " Object.getPrototypeOf(g.Object(1)) === g.Number.prototype].join();",
            "true,false,true,true",
            "the other realm's map threw its own realm's TypeError, its concat built in its own realm because the first realm's Array is no species there, and its Object wrapped a number with its own Number.prototype");

    /// <summary>SR-2: a function called from another realm's built-in runs in its own realm.</summary>
    private static (string, bool, string) AFunctionCalledFromAnotherRealmRunsInItsOwn() =>
        Check(
            "realms/sr2/a-function-called-from-another-realm-runs-in-its-own",
            "var g = $262.createRealm().global;\n" +
            "function here() { return [Object.getPrototypeOf([]) === Array.prototype, this === globalThis].join(); }\n" +
            "g.Reflect.apply(here, undefined, []);",
            "true,true",
            "called through the other realm's Reflect.apply, the function built from its own Array.prototype and bound its own global as a sloppy this");

    /// <summary>SR-7: the embedder's surface is told of a realm a guest created, inside the guest's step.</summary>
    private static (string, bool, string) TheHostIsToldOfACreatedRealm() =>
        Check(
            "realms/sr7/the-host-is-told-of-a-created-realm",
            "var before = host.told(); var r = $262.createRealm();\n" +
            "[before, host.told(), typeof r.global.host, r.global.host === host, r.global.host.told(), r.global.host.foreign].join();",
            "1,2,object,false,2,ForeignRealm",
            "the surface was told of the first realm at instantiation and of the created one when the guest created it, gave the created one a host object of its own, and had the first realm's global refused by the created realm's view by name");

    /// <summary>SR-7 under D6: a guest creating realms without bound meets the allowance.</summary>
    private static (string, bool, string) ACreatedRealmIsCharged() =>
        Check(
            "realms/sr7/a-created-realm-is-charged",
            "var held = []; for (;;) { held.push($262.createRealm()); }",
            "ResourceExhaustion/CeilingReached",
            "each realm was charged its retained bytes before it was built, so the loop ended at the live-bytes ceiling rather than in the process's memory",
            liveBytes: 8_388_608);

    /// <summary>Compiles <paramref name="source"/>, invokes it once, and compares the answer.</summary>
    private static (string, bool, string) Check(
        string name,
        string source,
        string expected,
        string meaning,
        ulong? liveBytes = null,
        VmFeatureManifestId[]? surfaces = null)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (name, false, "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        var descriptor = JavaScriptProfile.DescriptorHostingRealms(new CountingSurface(), surfaces ?? []);
        var created = VmRuntime.Create(VmCatalog.CreateBuilder().Add(descriptor).Build(), Options(liveBytes));

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
                    var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));
                    var result = instance.Invoke(in request, System.Threading.CancellationToken.None);

                    var answer = JavaScriptProfile.TryGetWideCompletion(in result, out var completion)
                        ? completion.Value
                        : JavaScriptProfile.TryGetUncaught(in result, out var uncaught)
                            ? "uncaught " + uncaught.Message
                            : $"{result.Outcome}/{result.Reason}";

                    return (
                        name,
                        string.Equals(answer, expected, System.StringComparison.Ordinal),
                        $"answered `{answer}` (`{expected}` expected): {meaning}");
                }
            }
        }
    }

    /// <summary>The runtime options: the host-surface permission and default ceilings, or a live-bytes one.</summary>
    private static VmRuntimeCreationOptions Options(ulong? liveBytes)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : dimension == VmBudgetDimension.LiveBytes && liveBytes is { } bytes
                    ? VmCeilingSpec.Value(dimension, bytes)
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

    /// <summary>
    /// An embedder that counts the realms it is told of, gives each a <c>host.told()</c>, and offers
    /// every later realm's view the first realm's global, recording how the view answered in
    /// <c>host.foreign</c>.
    /// </summary>
    private sealed class CountingSurface : IJsHostSurface
    {
        private int told;

        private JsHostValue first;

        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            told++;

            var host = realm.NewObject();
            realm.DefineValue(host, "told", realm.NewMethod("told", (_, _, _) => JsHostValue.Number(told)));

            if (told == 1)
            {
                first = realm.Global;
            }
            else
            {
                // A REF ONE VIEW MINTED IS REFUSED BY ANOTHER, BY NAME, on one engine as across two
                // (roadmap.hosting.md JSH-7): the refusal written before a second realm existed.
                string answer;

                try
                {
                    realm.DefineValue(host, "first", first);
                    answer = "accepted";
                }
                catch (JsHostSurfaceException refusal)
                {
                    answer = refusal.Refusal.ToString();
                }

                realm.DefineValue(host, "foreign", JsHostValue.String(answer));
            }

            realm.DefineValue(realm.Global, "host", host);
        }
    }
}
