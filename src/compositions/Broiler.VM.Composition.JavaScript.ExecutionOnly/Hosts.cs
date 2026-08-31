using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The runtimes and artifact descriptors this composition builds, one per replay mode.
/// </summary>
/// <remarks>
/// The whole of the composition is here and it is written down rather than discovered: the profile
/// arrives through a static accessor on its own type, no name is looked up, no directory is
/// scanned, no assembly is loaded, and nothing here or in the profile calls into reflection. That
/// is what makes a closure report over a publish of this project mean anything.
/// </remarks>
internal static class Hosts
{
    /// <summary>The profile identity a foreign-descriptor replay names, which no catalog holds.</summary>
    internal static VmProfileId AbsentProfile { get; } = VmProfileId.Parse("com.example.absent");

    /// <summary>Builds the catalog for a mode. One profile, named by its own static accessor.</summary>
    /// <remarks>
    /// There is no aggregate profile type to name instead, by design: one would reference every
    /// profile assembly and this closure would stop being a single-profile closure.
    /// </remarks>
    internal static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>Creates the runtime a replay mode calls for.</summary>
    internal static VmRuntime? Runtime(string mode, out string failure)
    {
        var created = VmRuntime.Create(Catalog(), Options(mode));

        if (created.TryGetRuntime(out var runtime))
        {
            failure = string.Empty;
            return runtime;
        }

        failure = $"{created.Outcome}/{created.Reason}";
        return null;
    }

    /// <summary>
    /// The artifact descriptor a replay mode presents with the bytes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The foreign mode names a profile the catalog does not hold, which is the input that makes
    /// the core answer <c>UnsupportedProfile</c> before this profile's verifier is reached at all.
    /// </para>
    /// <para>
    /// <b>It also names a manifest in that absent profile's own namespace, and the first attempt
    /// did not.</b> A feature-manifest identity must begin with its profile's ID, so a descriptor
    /// naming <c>com.example.absent</c> beside <c>broiler.javascript.slice</c> is MALFORMED rather
    /// than merely unknown - and the core answered <c>InvalidArtifact</c> /
    /// <c>MalformedArtifactDescriptor</c>, which is correct and is a different claim from the one
    /// this row exists to make. Two ways of being wrong about a descriptor, and the corpus pins
    /// the one it names.
    /// </para>
    /// </remarks>
    internal static VmArtifactDescriptor Descriptor(string mode) =>
        string.Equals(mode, "foreign-profile", StringComparison.Ordinal)
            ? new VmArtifactDescriptor(
                AbsentProfile,
                1,
                VmFeatureManifestId.Parse("com.example.absent.base"),
                default,
                VmCallerIdentity.FromCanonicalIdentity("js-execution-only://corpus"))
            : new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                1,
                JavaScriptProfile.SliceManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("js-execution-only://corpus"));

    /// <summary>
    /// The runtime options for a mode.
    /// </summary>
    /// <remarks>
    /// <b>The default mode adopts profile defaults on fourteen dimensions</b> rather than stating
    /// numbers, which is what a host does when it has no opinion - and which is why this profile's
    /// declared defaults are the vector that has to be chosen with a neighbour in mind. The tight
    /// mode states one ceiling explicitly and leaves the rest adopted, so what it proves is about
    /// that dimension and nothing else.
    /// </remarks>
    internal static VmRuntimeCreationOptions Options(string mode)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (dimension is VmBudgetDimension.LiveRuntimes)
            {
                ceilings.Add(VmCeilingSpec.AdoptParentRemaining(dimension));
                continue;
            }

            if (dimension is VmBudgetDimension.SectionCount &&
                string.Equals(mode, "tight-sections", StringComparison.Ordinal))
            {
                // One section, which every artifact of this format exceeds: the four required
                // sections mean the second one is already past the ceiling. The artifact is well
                // formed and this host declined to admit it, which is a resource answer.
                ceilings.Add(VmCeilingSpec.Value(dimension, 1));
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
