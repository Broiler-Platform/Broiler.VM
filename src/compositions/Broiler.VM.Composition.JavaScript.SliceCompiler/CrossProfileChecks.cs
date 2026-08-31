using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The claims that need a second profile in the catalog, plus the descriptor refusals.
/// </summary>
/// <remarks>
/// These are JS-0's carried gate clause, discharged at JS-1 where a descriptor exists to compose.
/// They live in this root rather than the execution-only one because they need a neighbour, and a
/// neighbour in that closure would contradict the single-profile claim that closure makes.
/// </remarks>
internal static class CrossProfileChecks
{
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        ANeighboursMaximumDoesNotReach(),
        ANeighboursAdoptedDefaultDoes(),
        AForeignPayloadIsNotProjected(),
        RegistrationOrderDoesNotChangeTheCatalogIdentity(),
        TheDescriptorIsAdmittedAndItsRefusalsAreNamed(),
    ];

    /// <summary>
    /// A neighbour's hard maximum does not reach this profile's artifacts.
    /// </summary>
    /// <remarks>
    /// The neighbour declares a section-count maximum of one, which every artifact of this format
    /// exceeds. A runtime holding both profiles still verifies this profile's artifact, because a
    /// maximum is applied at verification against the profile the artifact NAMES. That was not
    /// always true - the core clamped a runtime ceiling to the tightest maximum in the catalog
    /// until the defect was removed - so this check is what stops the correction rotting.
    /// </remarks>
    private static (string, bool, string) ANeighboursMaximumDoesNotReach()
    {
        var created = VmRuntime.Create(Catalog(hostile: true), Options(statedSectionCount: 64));

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("a-neighbours-maximum-does-not-reach-this-profile", false, $"{created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor();
            var verified = runtime.Verify(in descriptor, SliceLowering.Addition(), CancellationToken.None);

            return verified.TryGetArtifact(out _)
                ? ("a-neighbours-maximum-does-not-reach-this-profile", true,
                    "the neighbour caps section count at 1 and this profile's artifact still verified")
                : ("a-neighbours-maximum-does-not-reach-this-profile", false,
                    $"verification {verified.Outcome}/{verified.Reason}/{verified.Diagnostics.ExhaustedDimension}");
        }
    }

    /// <summary>
    /// A neighbour's adopted default does reach this profile, and that is the exposure that
    /// survives.
    /// </summary>
    /// <remarks>
    /// The same neighbour declares a section-count DEFAULT of one. A host that adopts profile
    /// defaults rather than stating numbers gets the tightest in the catalog, so this profile's
    /// artifact is refused for a dimension it did not breach, in a verifier that did nothing
    /// wrong. Proving it is the point: it is the hazard the roadmap says a composing component has
    /// to reconcile, and a check that could not show it would leave the claim untested.
    /// </remarks>
    private static (string, bool, string) ANeighboursAdoptedDefaultDoes()
    {
        var created = VmRuntime.Create(Catalog(hostile: true), Options(statedSectionCount: null));

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("a-neighbours-adopted-default-does-reach-this-profile", false, $"{created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor();
            var verified = runtime.Verify(in descriptor, SliceLowering.Addition(), CancellationToken.None);

            return !verified.TryGetArtifact(out _) &&
                verified.Outcome == VmOutcome.ResourceExhaustion &&
                verified.Diagnostics.ExhaustedDimension == VmBudgetDimension.SectionCount
                ? ("a-neighbours-adopted-default-does-reach-this-profile", true,
                    "adopting defaults beside a neighbour declaring 1 refused this artifact, naming SectionCount")
                : ("a-neighbours-adopted-default-does-reach-this-profile", false,
                    $"expected a SectionCount exhaustion, observed {verified.Outcome}/{verified.Reason}/" +
                    $"{verified.Diagnostics.ExhaustedDimension}");
        }
    }

    /// <summary>A payload minted by another profile is not projected by this profile's accessor.</summary>
    private static (string, bool, string) AForeignPayloadIsNotProjected()
    {
        var created = VmRuntime.Create(Catalog(hostile: false), Options(statedSectionCount: 64));

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("a-foreign-payload-is-not-projected", false, $"{created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Neighbour.ArtifactDescriptor();
            var verified = runtime.Verify(in descriptor, Neighbour.Artifact(), CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return ("a-foreign-payload-is-not-projected", false, $"verification {verified.Outcome}/{verified.Reason}");
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return ("a-foreign-payload-is-not-projected", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var request = new VmInvocationRequest(new VmUtf8Text("run"u8));
            var result = instance.Invoke(in request, CancellationToken.None);

            if (result.Outcome != VmOutcome.Normal)
            {
                return ("a-foreign-payload-is-not-projected", false, $"invocation {result.Outcome}/{result.Reason}");
            }

            return !JavaScriptProfile.TryGetCompletion(in result, out _) &&
                !JavaScriptProfile.TryGetFault(in result, out _)
                ? ("a-foreign-payload-is-not-projected", true,
                    "the neighbour's payload completed and projected as neither of this profile's")
                : ("a-foreign-payload-is-not-projected", false, "a foreign payload was projected as this profile's");
        }
    }

    /// <summary>
    /// A permutation of registration orders over the same descriptor set produces a byte-identical
    /// catalog identity.
    /// </summary>
    private static (string, bool, string) RegistrationOrderDoesNotChangeTheCatalogIdentity()
    {
        var neighbour = Neighbour.Descriptor(hostile: false);

        var forward = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.Descriptor)
            .Add(neighbour)
            .Build();

        var reverse = VmCatalog.CreateBuilder()
            .Add(neighbour)
            .Add(JavaScriptProfile.Descriptor)
            .Build();

        return forward.Identity.Equals(reverse.Identity)
            ? ("registration-order-does-not-change-the-catalog-identity", true,
                $"both encodings are {forward.Identity.EncodedLength} bytes and equal")
            : ("registration-order-does-not-change-the-catalog-identity", false,
                "the two registration orders encoded differently");
    }

    /// <summary>
    /// The JavaScript descriptor is admitted, and four named negative cases are each refused.
    /// </summary>
    /// <remarks>
    /// Every negative case is one descriptor with exactly one row changed, so what it proves is
    /// that the row matters rather than that some malformed descriptor is refused. The reason is
    /// not compared: the catalog's reason vocabulary is the core's, this composition holds no copy
    /// of it, and the core's own suite is what binds a refusal to its reason.
    /// </remarks>
    private static (string, bool, string) TheDescriptorIsAdmittedAndItsRefusalsAreNamed()
    {
        try
        {
            _ = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        }
        catch (Exception failure)
        {
            return ("descriptor-admitted-and-its-refusals-named", false, $"the catalog refused it: {failure.Message}");
        }

        var admitted = new List<string>();

        Provoke(admitted, "a manifest outside its profile's namespace", Neighbour.ManifestOutOfNamespace);
        Provoke(admitted, "a limit default above the profile maximum", Neighbour.DefaultAboveMaximum);
        Provoke(admitted, "an unconstrained limit default", Neighbour.UnconstrainedDefault);
        Provoke(admitted, "the reserved namespace without a Broiler package", Neighbour.ReservedNamespaceMismatch);

        return admitted.Count == 0
            ? ("descriptor-admitted-and-its-refusals-named", true,
                "admitted, and four named negative cases were each refused")
            : ("descriptor-admitted-and-its-refusals-named", false, string.Join("; ", admitted));
    }

    private static void Provoke(List<string> admitted, string what, Func<VmProfileDescriptor> build)
    {
        try
        {
            _ = VmCatalog.CreateBuilder().Add(build()).Build();
            admitted.Add($"{what} was admitted");
        }
        catch (Exception)
        {
            // Refused, which is the answer this case exists to produce.
        }
    }

    private static VmCatalog Catalog(bool hostile) => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Add(Neighbour.Descriptor(hostile))
        .Build();

    private static VmArtifactDescriptor Descriptor() =>
        new(
            JavaScriptProfile.Id,
            1,
            JavaScriptProfile.SliceManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("js-slice-compiler://cross-profile"));

    /// <summary>
    /// Runtime options that either state a section-count ceiling or adopt the catalog's tightest
    /// default for it.
    /// </summary>
    /// <remarks>
    /// The two cases differ in exactly one ceiling, which is what makes the pair of checks above a
    /// comparison rather than two unrelated runs.
    /// </remarks>
    private static VmRuntimeCreationOptions Options(uint? statedSectionCount)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (dimension is VmBudgetDimension.LiveRuntimes)
            {
                ceilings.Add(VmCeilingSpec.AdoptParentRemaining(dimension));
                continue;
            }

            if (dimension is VmBudgetDimension.SectionCount && statedSectionCount is { } stated)
            {
                ceilings.Add(VmCeilingSpec.Value(dimension, stated));
                continue;
            }

            ceilings.Add(VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: ImmutableArray<VmCapabilityRegistration>.Empty);
    }
}
