using Broiler.VM;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// A second profile, composed beside the JavaScript one so that the cross-profile claims have a
/// neighbour to be about.
/// </summary>
/// <remarks>
/// <para>
/// It is defined inside this composition root rather than in an assembly of its own, and it lives
/// in THIS root rather than in the execution-only one, because putting a second profile in that
/// closure would contradict the single-profile claim that closure exists to make. What this root
/// claims is that it carries a lowering; a neighbour costs it nothing it was not already
/// admitting.
/// </para>
/// <para>
/// Its format is four magic bytes and one count, which is as much as a neighbour needs to be: the
/// point of it is its DECLARATIONS, not its language. The hostile variant declares a section-count
/// maximum and default of one, which is the tightest thing a profile can honestly say about a
/// dimension it uses once - and which is exactly the shape that reaches across a catalog through
/// one of those two vectors and not the other.
/// </para>
/// </remarks>
internal static class Neighbour
{
    internal static VmProfileId Id { get; } = VmProfileId.Parse("com.example.neighbour");

    internal static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("com.example.neighbour.base");

    /// <summary>A well-formed artifact of the neighbour's own format.</summary>
    internal static byte[] Artifact() => [(byte)'N', (byte)'B', (byte)'R', (byte)'1', 7];

    /// <summary>The artifact descriptor that names one.</summary>
    internal static VmArtifactDescriptor ArtifactDescriptor() =>
        new(Id, 1, Manifest, default, VmCallerIdentity.FromCanonicalIdentity("js-slice-compiler://neighbour"));

    /// <summary>The neighbour's descriptor, hostile or not.</summary>
    internal static VmProfileDescriptor Descriptor(bool hostile) =>
        Build(Id, Manifest, "Com.Example.Neighbour", hostile ? 1u : 64u, hostile ? 1u : 64u, unconstrainedDefault: false);

    /// <summary>A descriptor whose manifest is outside its own profile's namespace.</summary>
    internal static VmProfileDescriptor ManifestOutOfNamespace() =>
        Build(Id, VmFeatureManifestId.Parse("other.thing.base"), "Com.Example.Neighbour", 64, 64, false);

    /// <summary>A descriptor whose limit default is above its own hard maximum.</summary>
    internal static VmProfileDescriptor DefaultAboveMaximum() =>
        Build(Id, Manifest, "Com.Example.Neighbour", defaultSections: 64, maximumSections: 1, unconstrainedDefault: false);

    /// <summary>A descriptor with an unconstrained limit default.</summary>
    internal static VmProfileDescriptor UnconstrainedDefault() =>
        Build(Id, Manifest, "Com.Example.Neighbour", 64, 64, unconstrainedDefault: true);

    /// <summary>A descriptor claiming the reserved first label without a Broiler package identity.</summary>
    internal static VmProfileDescriptor ReservedNamespaceMismatch() =>
        Build(
            VmProfileId.Parse("broiler.neighbour"),
            VmFeatureManifestId.Parse("broiler.neighbour.base"),
            "Com.Example.Neighbour",
            64,
            64,
            false);

    private static VmProfileDescriptor Build(
        VmProfileId id,
        VmFeatureManifestId manifest,
        string packageId,
        uint defaultSections,
        uint maximumSections,
        bool unconstrainedDefault)
    {
        VmDiagnosticsIdentity.TryCreate(id, id + ".diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: id,
            displayName: "Example Neighbour",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: ImmutableArray.Create(manifest),
            verifier: new NeighbourVerifier(id),
            executorFactory: environment => new NeighbourExecutor(id),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Vector(defaultSections, unconstrainedDefault),
            profileHardMaxima: Vector(maximumSections, false),
            budgetDeclarationMatrix: Matrix(),
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(2000, 2099),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("com.example.neighbour.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(packageId, "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    private static VmLimitVector Vector(uint sections, bool unconstrained)
    {
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = 1_000_000;
        }

        values[(int)VmBudgetDimension.SectionCount] = sections;
        values[(int)VmBudgetDimension.LiveRuntimes] = 16;

        if (unconstrained)
        {
            values[(int)VmBudgetDimension.Fuel] = ulong.MaxValue;
        }

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        rows[(int)VmBudgetDimension.HostCalls] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}

/// <summary>The neighbour's decoded artifact: one number.</summary>
internal sealed class NeighbourProgram : IVmVerifiedState
{
    internal NeighbourProgram(byte value) => Value = value;

    internal byte Value { get; }
}

/// <summary>The neighbour's payload, whose kind IDs lie in its own declared range.</summary>
internal sealed class NeighbourAnswer : IVmProfilePayload
{
    internal NeighbourAnswer(VmProfileId profileId, byte value)
    {
        Identity = new VmPayloadIdentity(profileId, 2001, 1);
        Value = value;
    }

    public VmPayloadIdentity Identity { get; }

    internal byte Value { get; }
}

/// <summary>The neighbour's instance state.</summary>
internal sealed class NeighbourInstance : IVmInstanceState
{
    internal NeighbourInstance(NeighbourProgram program) => Program = program;

    internal NeighbourProgram Program { get; }
}

/// <summary>The neighbour's verifier: four magic bytes and a count.</summary>
internal sealed class NeighbourVerifier : IVmProfileVerifier
{
    internal NeighbourVerifier(VmProfileId profileId) => ProfileId = profileId;

    public VmProfileId ProfileId { get; }

    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    public int AuthoredCoreContractVersion => 1;

    public int VerifierSemanticVersion => 1;

    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        CancellationToken cancellationToken)
    {
        if (descriptor.ProfileId != ProfileId)
        {
            return VmVerifierOutcome.UnsupportedProfile();
        }

        if (payload.Length != 5 || !payload[..4].SequenceEqual("NBR1"u8))
        {
            return VmVerifierOutcome.InvalidArtifact(VmReason.MalformedEncoding, 9001, new VmSourcePosition(-1, 0, 0, 0));
        }

        return VmVerifierOutcome.Verified(new NeighbourProgram(payload[4]), VmArtifactSharing.Shareable);
    }
}

/// <summary>The neighbour's executor.</summary>
internal sealed class NeighbourExecutor : IVmProfileExecutor
{
    internal NeighbourExecutor(VmProfileId profileId) => ProfileId = profileId;

    public VmProfileId ProfileId { get; }

    public VmExecutionStep Instantiate(VmVerifiedArtifact artifact, CancellationToken cancellationToken) =>
        artifact.TryGetState(out var state) && state is NeighbourProgram program
            ? VmExecutionStep.Instantiated(new NeighbourInstance(program), null)
            : VmExecutionStep.ContractViolation(VmReason.ForeignHandle);

    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        CancellationToken cancellationToken) =>
        state is NeighbourInstance instance
            ? VmExecutionStep.Completed(new NeighbourAnswer(ProfileId, instance.Program.Value))
            : VmExecutionStep.ContractViolation(VmReason.ForeignPayload);

    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);

    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }
}
