// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.WebAssembly;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>
/// The composition itself: one catalog, two product profiles, one set of ceilings.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the file the whole root exists for.</b> Two descriptors are added to one builder and
/// the runtime is created once; nothing looks a profile up by name, scans a directory or loads an
/// assembly, and neither profile knows the other is there. What a caller hands this host decides
/// which of the two verifies the bytes, and the core decides nothing differently because there are
/// two of them.
/// </para>
/// <para>
/// <b>THE CEILINGS ARE STATED AND NOT ADOPTED, AND THAT IS FORCED BY THE CATALOG HAVING TWO
/// ROWS.</b> A runtime that adopts the profile default for a dimension resolves to the TIGHTEST
/// default in the catalog - a catalog-wide fold, recorded in <c>docs/compositions.md</c> section 3
/// as the reason the two-profile fixture root states three ceilings itself. So in a two-profile
/// image the JavaScript profile's declared defaults would silently decide what a WebAssembly
/// module may do, and the WebAssembly profile's would silently decide what a script may do,
/// whichever is smaller per dimension - and a program refused for want of allowance would be
/// refused because of a profile it never touched. This host therefore states the dimensions where
/// the fold bites, and a caller's <c>--fuel</c>, <c>--wall</c>, <c>--call-depth</c> and
/// <c>--live-bytes</c> override what it states.
/// </para>
/// <para>
/// <b>What stating a ceiling does NOT do is widen anything.</b> The effective ceiling for an
/// operation is the intersection of what the host states with the profile the artifact names, so
/// a stated value larger than a profile's own hard maximum buys that profile nothing. The reason
/// to state one is to stop the OTHER profile's default from deciding, which is a different act
/// from raising a limit.
/// </para>
/// </remarks>
internal static class Composition
{
    /// <summary>
    /// The catalog: two profiles, each arriving through its own assembly's static accessor.
    /// </summary>
    /// <remarks>
    /// <b>There is no aggregate profile listing anywhere and there must not be.</b>
    /// <c>docs/compositions.md</c> section 2 forbids an <c>AllProfiles</c> or <c>KnownProfiles</c>
    /// type outright: one would reference every profile assembly in the repository and defeat the
    /// exact closure the register exists to describe. Two <c>Add</c> calls naming two descriptors
    /// is what composing two profiles looks like, and a third profile would be a third call in a
    /// root that referenced it.
    /// </remarks>
    /// <param name="emitter">
    /// The backend whose bytes the JavaScript verifier may RE-EMIT and compare, or null for the
    /// ordinary descriptor. See the remark below: this is a different door and not an option.
    /// </param>
    /// <remarks>
    /// <b>A NATIVE PAYLOAD IS ADMITTED THROUGH A DESCRIPTOR THAT CARRIES THE BACKEND, AND THAT IS
    /// WHAT MAKES ITS BYTES VERIFIABLE RATHER THAN MERELY WELL-FRAMED.</b> An image holding the
    /// emitter can recompile the bytecode the artifact also carries and require the emitted bytes
    /// to match; an image without one checks the framing and then trusts provenance. This root
    /// carries the lowering, so when a caller asks for a native form it takes the stronger door and
    /// registers the descriptor built around that exact backend. The emitter is fixed when the
    /// descriptor is registered, for the reason the profile gives: a verifier that could be asked
    /// twice could be given two answers.
    /// </remarks>
    internal static VmCatalog Catalog(Broiler.VM.Profile.JavaScript.Format.IJsNativeEmitter? emitter = null) =>
        VmCatalog.CreateBuilder()
            .Add(emitter is null
                ? JavaScriptProfile.Descriptor
                : JavaScriptProfile.DescriptorReEmittingWith(emitter, JavaScriptProfile.NativeManifest))
            .Add(WebAssemblyProfile.Descriptor)
            .Build();

    /// <summary>The ceilings a caller may move, with null meaning "this host does not state it".</summary>
    /// <param name="Fuel">The instruction allowance per run.</param>
    /// <param name="WallClock">The wall-clock allowance per run, in milliseconds.</param>
    /// <param name="CallDepth">The call-depth allowance per run, in frames.</param>
    /// <param name="LiveBytes">The live-memory allowance per run, in bytes.</param>
    internal sealed record Allowances(
        ulong? Fuel, ulong? WallClock, ulong? CallDepth, ulong? LiveBytes);

    /// <summary>
    /// The runtime options this host creates a runtime with.
    /// </summary>
    /// <param name="allowances">What the caller asked for, dimension by dimension.</param>
    /// <param name="javascript">
    /// Whether the JavaScript profile's host capabilities are registered for this runtime.
    /// </param>
    /// <param name="provider">
    /// The artifact provider this composition answers guest-initiated loads with, or null to
    /// register none - which refuses every such load deterministically.
    /// </param>
    internal static VmRuntimeCreationOptions Options(
        Allowances allowances, bool javascript, SourceProvider? provider)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel when allowances.Fuel is { } fuel =>
                    VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.WallClock when allowances.WallClock is { } wall =>
                    VmCeilingSpec.Value(dimension, wall),
                VmBudgetDimension.CallDepth when allowances.CallDepth is { } frames =>
                    VmCeilingSpec.Value(dimension, frames),
                VmBudgetDimension.LiveBytes when allowances.LiveBytes is { } bytes =>
                    VmCeilingSpec.Value(dimension, bytes),
                _ when Stated.TryGetValue(dimension, out var stated) =>
                    VmCeilingSpec.Value(dimension, stated),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        if (javascript)
        {
            // THE THREE REGISTRATIONS ARE THE JAVASCRIPT LANE'S CONTENT POLICY AND NOBODY ELSE'S.
            // `print` reaching standard output, a specifier resolving the way this host resolves
            // one, and source becoming an artifact are each a decision a sibling root may take
            // differently while composing the same profile. They are registered only for a
            // JavaScript run: registering a capability no composed profile imports is legal and
            // the fixture roots demonstrate it, but a WebAssembly module has no use for any of the
            // three, and a runtime built for one should not carry the other's answers.
            capabilities.Add(VmCapabilityRegistration.Value(
                JavaScriptProfile.WriteCapability, Write));

            capabilities.Add(VmCapabilityRegistration.Value(
                JavaScriptProfile.ResolveCapability, Resolve));

            if (provider is not null)
            {
                capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
                    JavaScriptProfile.SourceProviderCapability, provider));
            }
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>
    /// The dimensions this host states itself, because the catalog-wide fold would otherwise
    /// decide them from whichever profile declared less.
    /// </summary>
    /// <remarks>
    /// <b>Each value is the larger of the two profiles' declared defaults for that dimension,
    /// computed rather than typed.</b> A literal here would be a number nobody could check and
    /// would go stale the day either profile revised a default; reading both descriptors and
    /// taking the maximum states, in one expression, exactly what this host means - neither
    /// profile's default may decide the other's run. It widens nothing: the effective ceiling is
    /// still intersected with the profile the artifact names.
    /// </remarks>
    private static readonly IReadOnlyDictionary<VmBudgetDimension, ulong> Stated = StateTheFold();

    private static IReadOnlyDictionary<VmBudgetDimension, ulong> StateTheFold()
    {
        var stated = new Dictionary<VmBudgetDimension, ulong>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (dimension is VmBudgetDimension.LiveRuntimes)
            {
                continue;
            }

            if (!TryDefault(JavaScriptProfile.Descriptor, dimension, out var script) ||
                !TryDefault(WebAssemblyProfile.Descriptor, dimension, out var module))
            {
                continue;
            }

            if (script != module)
            {
                stated[dimension] = System.Math.Max(script, module);
            }
        }

        return stated;
    }

    /// <summary>The finite default one profile declares for one dimension, where it declares one.</summary>
    /// <remarks>
    /// <b>An UNCONSTRAINED default is not a number and is answered as an absence.</b> A dimension
    /// one profile leaves unconstrained and the other bounds has no maximum this host could state
    /// - the only honest statement would be "unconstrained", which is not a value a ceiling
    /// specification carries - so the fold is left to decide it, which resolves to the bound the
    /// bounding profile declared. That is the tighter of the two and it is the only one either
    /// profile ever wrote down.
    /// </remarks>
    private static bool TryDefault(
        VmProfileDescriptor descriptor, VmBudgetDimension dimension, out ulong value)
    {
        value = 0;

        if (descriptor.LimitDefaults.IsEmpty || descriptor.LimitDefaults.IsUnconstrained(dimension))
        {
            return false;
        }

        value = descriptor.LimitDefaults[dimension];
        return true;
    }

    /// <summary>Rules on whether a module request resolves the way this composition resolves it.</summary>
    /// <remarks>
    /// <c>Refused</c> is a policy answer and not a failure of the call, which is exactly what it
    /// means here: the artifact was resolved by rules that are not this host's, and this host
    /// declines to evaluate it.
    /// </remarks>
    private static VmHostCallOutcome Resolve(VmBytes argument, out VmOpaqueRef result)
    {
        result = default;

        return ModuleGraph.Confirms(argument.Span)
            ? VmHostCallOutcome.Completed
            : VmHostCallOutcome.Refused;
    }

    /// <summary>Write a line to standard output, which is how a program's <c>print</c> arrives.</summary>
    private static VmHostCallOutcome Write(VmBytes argument, out VmOpaqueRef result)
    {
        result = default;
        Console.Out.WriteLine(System.Text.Encoding.UTF8.GetString(argument.Span));
        return VmHostCallOutcome.Completed;
    }
}
