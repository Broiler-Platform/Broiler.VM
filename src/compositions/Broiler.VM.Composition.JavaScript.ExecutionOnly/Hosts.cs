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
    /// The tight modes: one per budget dimension a verification of this profile can answer a
    /// resource exhaustion on, each naming the one ceiling that mode states explicitly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Seven, because the verifier answers seven.</b> Four are the bounded reader's ceilings -
    /// section count, declared counts, structural depth and artifact bytes - and three are the
    /// meter's allowances: the bounded allocator's bytes, the link stage's work charge, and the
    /// wall clock a poll stops for. An exhaustion answer carries no diagnostic code, so the
    /// registry's both-directions binding reaches none of them and a corpus entry per dimension is
    /// the only thing that does.
    /// </para>
    /// <para>
    /// <b>Every value here is a host declining, not an artifact being large.</b> The bytes each
    /// mode is presented with are a well-formed program that the default mode verifies and runs, so
    /// what a row proves is a property of the host. Two of the seven are zero on purpose: a
    /// structural depth of zero refuses the first frame the format opens, and a wall clock of zero
    /// is already spent at the first poll, which is what makes the answer a fact about the ceiling
    /// rather than about how fast the machine ran.
    /// </para>
    /// <para>
    /// <b>The artifact-bytes row is answered by the core and not by this profile</b>, and it is the
    /// one of the seven that is. The core compares the payload length against the same effective
    /// ceiling the reader would be handed, one call before the verifier is entered, so no host
    /// ceiling can reach the reader's own artifact-bytes arm - that arm is defensive and the
    /// ordering assertions reach it by calling the verifier directly. The row still records the
    /// dimension and the scope the answer named, which is what the gate asks of it.
    /// </para>
    /// </remarks>
    internal static readonly (string Mode, VmBudgetDimension Dimension, ulong Value)[] TightModes =
    [
        // One section, which every artifact of this format exceeds: the four required sections
        // mean the second one is already past the ceiling.
        ("tight-sections", VmBudgetDimension.SectionCount, 1UL),

        // Eight bytes, which no artifact of this format fits in: the magic alone is four.
        ("tight-artifact-bytes", VmBudgetDimension.ArtifactBytes, 8UL),

        // One, which the first count the artifact declares - how many sections it carries -
        // already exceeds.
        ("tight-declared-count", VmBudgetDimension.DeclaredCount, 1UL),

        // Zero frames of nesting, which refuses the first section rather than a deep one.
        ("tight-structural-depth", VmBudgetDimension.StructuralDepth, 0UL),

        // Eight bytes, which the first array the verifier sizes from the artifact exceeds.
        ("tight-allocated-bytes", VmBudgetDimension.AllocatedBytes, 8UL),

        // One work unit, which the read stage spends before it has finished the header.
        ("tight-verifier-work", VmBudgetDimension.VerifierWork, 1UL),

        // No milliseconds at all, so the first poll finds the allowance spent whatever the clock
        // says. A one-millisecond allowance would pass or fail by how busy the machine was.
        ("tight-wall-clock", VmBudgetDimension.WallClock, 0UL),
    ];

    /// <summary>
    /// The runtime options for a mode.
    /// </summary>
    /// <remarks>
    /// <b>The default mode adopts profile defaults on fourteen dimensions</b> rather than stating
    /// numbers, which is what a host does when it has no opinion - and which is why this profile's
    /// declared defaults are the vector that has to be chosen with a neighbour in mind. A tight
    /// mode states one ceiling explicitly and leaves the rest adopted, so what it proves is about
    /// that dimension and nothing else.
    /// </remarks>
    internal static VmRuntimeCreationOptions Options(string mode)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();
        var tight = System.Array.Find(TightModes, candidate =>
            string.Equals(candidate.Mode, mode, StringComparison.Ordinal));

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (dimension is VmBudgetDimension.LiveRuntimes)
            {
                ceilings.Add(VmCeilingSpec.AdoptParentRemaining(dimension));
                continue;
            }

            if (tight.Mode is not null && dimension == tight.Dimension)
            {
                ceilings.Add(VmCeilingSpec.Value(dimension, tight.Value));
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
