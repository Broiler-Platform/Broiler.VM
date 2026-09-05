using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Collections.Immutable;

using Broiler.VM.Profile.JavaScript.Format;

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
    internal static VmCatalog Catalog() => Catalog("default");

    /// <summary>The prefix every module replay mode shares.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal const string ModuleMode = "modules";

    /// <summary>The module mode that admits the surface and registers a resolver.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal const string ModuleAdmittedMode = "modules";

    /// <summary>
    /// The module mode that admits the surface and registers no resolver.
    /// </summary>
    /// <remarks>
    /// <b>It is a THIRD mode and not the declining one, because the two refuse for different
    /// reasons.</b> A composition that does not admit the surface is told
    /// <c>SurfaceOutsideComposition</c>, which the wide-declining mode already reaches; one that
    /// admits it and registers nothing is told <c>ModuleResolverAbsent</c>. Folding them into one
    /// mode would have left one of the two codes with no retained entry.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal const string ModuleUnresolvedMode = "modules-no-resolver";

    /// <summary>
    /// The catalog a replay mode registers, which is where a composition declines a surface.
    /// </summary>
    /// <remarks>
    /// <b>Declining is a registration and not a flag.</b> The declining mode registers a descriptor
    /// built to admit no optional surface at all, which is exactly what a composition wanting an
    /// execution image with no shared mutable memory would write. Nothing else about the mode
    /// differs: the same bytes, the same descriptor presented with them, the same ceilings.
    /// </remarks>
    internal static VmCatalog Catalog(string mode) => VmCatalog.CreateBuilder()
        .Add(string.Equals(mode, "wide-declining", StringComparison.Ordinal)
            ? JavaScriptProfile.DescriptorAdmitting()
            : JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>Creates the runtime a replay mode calls for.</summary>
    internal static VmRuntime? Runtime(string mode, out string failure)
    {
        var created = VmRuntime.Create(Catalog(mode), Options(mode));

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
    internal static VmArtifactDescriptor Descriptor(string mode)
    {
        if (string.Equals(mode, "foreign-profile", StringComparison.Ordinal))
        {
            return new VmArtifactDescriptor(
                AbsentProfile,
                1,
                VmFeatureManifestId.Parse("com.example.absent.base"),
                default,
                VmCallerIdentity.FromCanonicalIdentity("js-execution-only://corpus"));
        }

        // THE WIDE MODE IS A DESCRIPTOR AND NOTHING ELSE. The bytes of a version-2 entry are
        // version-2 bytes; what this mode changes is which format version and manifest the caller
        // says they are, because a descriptor that said version 1 would be refused for the
        // mismatch before the version-2 pass ever read a section.
        if (string.Equals(mode, "wide", StringComparison.Ordinal) ||
            string.Equals(mode, "wide-declining", StringComparison.Ordinal) ||
            mode.StartsWith(ModuleMode, StringComparison.Ordinal))
        {
            return new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("js-execution-only://corpus"));
        }

        return new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            1,
            JavaScriptProfile.SliceManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("js-execution-only://corpus"));
    }

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

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        // REGISTERING THE RESOLVER IS THE ONE THING THAT SEPARATES TWO OF THE MODULE MODES. A
        // resolution rule of "the key the request already carries" is what this replay's producer
        // used to bundle its graphs, so this host confirms every request; the mode that registers
        // nothing is the one whose artifact is refused for want of a resolver.
        if (string.Equals(mode, ModuleAdmittedMode, StringComparison.Ordinal))
        {
            capabilities.Add(VmCapabilityRegistration.Value(
                JavaScriptProfile.ResolveCapability,
                Confirm));
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
    /// This replay's module resolution: the key the request already carries.
    /// </summary>
    /// <remarks>
    /// The retained module entries are bundled by a producer whose keys are the names it gave each
    /// module, so this host's rule is the same one - and a request carrying no key at all is
    /// refused, which is what makes the confirmation a check rather than a formality.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static VmHostCallOutcome Confirm(VmBytes argument, out VmOpaqueRef result)
    {
        result = default;
        var parts = JsFormat.DecodeText(argument.Span).Split('\0');

        return parts.Length == 3 && parts[2].Length != 0
            ? VmHostCallOutcome.Completed
            : VmHostCallOutcome.Refused;
    }
}
