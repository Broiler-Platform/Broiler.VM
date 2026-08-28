// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           32
// Human-reviewed:   0/2
// IP risk:          None
// Security risk:    Medium
// Resource impact:  0/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The single frozen entry contract: everything the core needs to know about one VM profile.
/// </summary>
/// <remarks>
/// <para>
/// A sealed immutable class with a full-arity constructor, deliberately <strong>not</strong> a
/// struct. <c>default(T)</c> over a struct would present an empty identity and a zero contract
/// version as though they had been declared, whereas a null reference is rejected loudly. Catalog
/// construction happens once per process, so the allocation a struct would save is irrelevant
/// beside that hole.
/// </para>
/// <para>
/// Every field is required and none is defaulted, with two exceptions:
/// <see cref="BuiltAgainstCoreContractVersion"/>, which defaults to the core constant because it is
/// a machine-derived fact about the compilation rather than an author's claim, and
/// <see cref="ArtifactSharing"/>, which defaults to the restrictive value. A fluent per-field
/// builder was rejected because it turns "forgot a field" from a compile error into a run-time
/// failure and multiplies the construction paths every identity and drift check must then cover.
/// </para>
/// <para>
/// <strong>Excluded by construction at core contract version 1</strong>: priority, precedence,
/// ordering hint, enabled flag, alias set, deprecation marker, feature content, localized text,
/// file path, assembly name, type name, and any string intended to be resolved into a type. The
/// last is the seed of reflection-based composition and is forbidden whether or not the core
/// resolves it today. Priority and enabled flags are forbidden because a composition root is an
/// explicit package and never a run-time option that removes an already rooted profile.
/// </para>
/// <para>
/// <see cref="AuthoredCoreContractVersion"/> is a required parameter and must never be populated
/// from <see cref="VmCoreContract.Version"/>. There is deliberately no overload that could: the
/// whole point of having two integers is that one is what the author wrote and the other is what
/// the compiler saw.
/// </para>
/// </remarks>
public sealed class VmProfileDescriptor
{
    /// <summary>The most feature manifests one descriptor may accept.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s7; IP=None; Security=Medium; Resources=0; Fingerprint=EDA3BD
    // Broiler-Human: PENDING
    public const int MaximumAcceptedFeatureManifests = 64;

    /// <summary>The most characters a display name may have.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s7; IP=None; Security=Medium; Resources=0; Fingerprint=948C28
    // Broiler-Human: PENDING
    public const int MaximumDisplayNameLength = 64;

    /// <summary>Creates a descriptor. Every row of the frozen table is supplied.</summary>
    public VmProfileDescriptor(
        VmProfileId profileId,
        string displayName,
        int descriptorRevision,
        VmFormatVersionRange supportedFormatVersions,
        System.Collections.Immutable.ImmutableArray<VmFeatureManifestId> acceptedFeatureManifests,
        IVmProfileVerifier verifier,
        VmExecutorFactory executorFactory,
        VmArtifactRepresentationKind artifactRepresentationKind,
        VmArtifactLifetimeKind artifactLifetimeKind,
        bool supportsConcurrentVerification,
        VmThreadAffinity threadAffinity,
        ulong cancellationPollBound,
        ulong abandonBudget,
        VmLimitVector limitDefaults,
        VmLimitVector profileHardMaxima,
        VmBudgetDeclarationMatrix budgetDeclarationMatrix,
        System.Collections.Immutable.ImmutableArray<VmCapabilityImport> hostCapabilityDescriptors,
        VmGuestLoadDeclaration guestInitiatedLoads,
        VmDeclaration asynchronousInstantiation,
        VmDeclaration externalSuspension,
        VmPayloadKindIdRange payloadKindIdRange,
        int authoredCoreContractVersion,
        VmConformanceManifestId conformanceManifestId,
        int conformanceManifestVersion,
        VmDiagnosticsIdentity diagnosticsIdentity,
        VmPackageIdentity packageIdentity,
        VmFaultRecovery faultRecovery,
        uint maxUnchargedWork,
        uint chargingGranularity,
        VmArtifactSharing artifactSharing = VmArtifactSharing.RuntimeScoped,
        int builtAgainstCoreContractVersion = VmCoreContract.Version)
    {
        ProfileId = profileId;
        DisplayName = displayName;
        DescriptorRevision = descriptorRevision;
        SupportedFormatVersions = supportedFormatVersions;
        AcceptedFeatureManifests = acceptedFeatureManifests;
        Verifier = verifier;
        ExecutorFactory = executorFactory;
        ArtifactRepresentationKind = artifactRepresentationKind;
        ArtifactLifetimeKind = artifactLifetimeKind;
        SupportsConcurrentVerification = supportsConcurrentVerification;
        ThreadAffinity = threadAffinity;
        CancellationPollBound = cancellationPollBound;
        AbandonBudget = abandonBudget;
        LimitDefaults = limitDefaults;
        ProfileHardMaxima = profileHardMaxima;
        BudgetDeclarationMatrix = budgetDeclarationMatrix;
        HostCapabilityDescriptors = hostCapabilityDescriptors;
        GuestInitiatedLoads = guestInitiatedLoads;
        AsynchronousInstantiation = asynchronousInstantiation;
        ExternalSuspension = externalSuspension;
        PayloadKindIdRange = payloadKindIdRange;
        AuthoredCoreContractVersion = authoredCoreContractVersion;
        ConformanceManifestId = conformanceManifestId;
        ConformanceManifestVersion = conformanceManifestVersion;
        DiagnosticsIdentity = diagnosticsIdentity;
        PackageIdentity = packageIdentity;
        FaultRecovery = faultRecovery;
        MaxUnchargedWork = maxUnchargedWork;
        ChargingGranularity = chargingGranularity;
        ArtifactSharing = artifactSharing;
        BuiltAgainstCoreContractVersion = builtAgainstCoreContractVersion;
    }

    /// <summary>Row 1. The profile's stable identity.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>
    /// Row 2. A required human-readable label of 1 to 64 UTF-16 units, non-localized and
    /// mechanically inert: never compared, folded, sorted, or used for lookup, uniqueness,
    /// ordering, cache keys, handle identity or envelope dispatch. Two entries may share one.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>Row 3. Incremented whenever anything that can affect verification changes.</summary>
    public int DescriptorRevision { get; }

    /// <summary>Row 4. The inclusive profile-format version range.</summary>
    public VmFormatVersionRange SupportedFormatVersions { get; }

    /// <summary>Row 5. One to sixty-four manifests, normalized to ascending ordinal order.</summary>
    public System.Collections.Immutable.ImmutableArray<VmFeatureManifestId> AcceptedFeatureManifests { get; }

    /// <summary>Row 6. The verifier, referenced directly rather than named.</summary>
    public IVmProfileVerifier Verifier { get; }

    /// <summary>Row 7. The per-runtime executor factory, trim-rooted by direct reference.</summary>
    public VmExecutorFactory ExecutorFactory { get; }

    /// <summary>Row 8. Snapshot or decoded.</summary>
    public VmArtifactRepresentationKind ArtifactRepresentationKind { get; }

    /// <summary>Row 9. Whether a verified artifact owns disposable resources.</summary>
    public VmArtifactLifetimeKind ArtifactLifetimeKind { get; }

    /// <summary>Row 10. Whether two verifications may run concurrently in one runtime.</summary>
    public bool SupportsConcurrentVerification { get; }

    /// <summary>Row 11. The profile's declared thread affinity.</summary>
    public VmThreadAffinity ThreadAffinity { get; }

    /// <summary>
    /// Row 12. The bound, in the profile's own work units, on work between two polls. It is what
    /// makes cancellation latency bounded in declared work units rather than in wall-clock time the
    /// core cannot promise.
    /// </summary>
    public ulong CancellationPollBound { get; }

    /// <summary>Row 13. The bounded allowance the profile gets for its terminal unwind.</summary>
    public ulong AbandonBudget { get; }

    /// <summary>
    /// Row 14. Per-dimension bounded defaults a host may explicitly adopt. No member may encode
    /// "unbounded" or "unset": a default that meant either would make omission mean unbounded,
    /// which invariant 9 forbids outright.
    /// </summary>
    public VmLimitVector LimitDefaults { get; }

    /// <summary>Row 15. Per-dimension hard maxima the profile imposes on itself.</summary>
    public VmLimitVector ProfileHardMaxima { get; }

    /// <summary>Row 16. Which dimensions the profile charges.</summary>
    public VmBudgetDeclarationMatrix BudgetDeclarationMatrix { get; }

    /// <summary>
    /// Row 17. The capability imports, possibly empty. Empty means the profile imports nothing,
    /// which is a legal and expected state rather than an omission.
    /// </summary>
    public System.Collections.Immutable.ImmutableArray<VmCapabilityImport> HostCapabilityDescriptors { get; }

    /// <summary>Row 18. Whether the profile may request code while executing, and under what bounds.</summary>
    public VmGuestLoadDeclaration GuestInitiatedLoads { get; }

    /// <summary>Row 19. Whether instantiation may suspend.</summary>
    public VmDeclaration AsynchronousInstantiation { get; }

    /// <summary>Row 20. Whether the host may suspend an operation from outside.</summary>
    public VmDeclaration ExternalSuspension { get; }

    /// <summary>Row 21. The closed range of payload kind IDs the profile may stamp.</summary>
    public VmPayloadKindIdRange PayloadKindIdRange { get; }

    /// <summary>Row 22. The contract version the profile was compiled against; machine-derived.</summary>
    public int BuiltAgainstCoreContractVersion { get; }

    /// <summary>Row 23. The contract version the author wrote for; never derived from a constant.</summary>
    public int AuthoredCoreContractVersion { get; }

    /// <summary>Row 24. The conformance corpus identity, for support tables and evidence only.</summary>
    public VmConformanceManifestId ConformanceManifestId { get; }

    /// <summary>Row 24. Its version.</summary>
    public int ConformanceManifestVersion { get; }

    /// <summary>Row 25. The profile's diagnostics token, under its own ID namespace.</summary>
    public VmDiagnosticsIdentity DiagnosticsIdentity { get; }

    /// <summary>Row 26. Package, version and owner tag, used by architecture and release checks.</summary>
    public VmPackageIdentity PackageIdentity { get; }

    /// <summary>Row 27. Whether one verified artifact may serve more than one runtime.</summary>
    public VmArtifactSharing ArtifactSharing { get; }

    /// <summary>Row 28. What a profile fault does to the instance that produced it.</summary>
    public VmFaultRecovery FaultRecovery { get; }

    /// <summary>Row 29. The bound on work performed between two polls, in the profile's own units.</summary>
    public uint MaxUnchargedWork { get; }

    /// <summary>Row 30. The granularity at which the profile charges, in the same units.</summary>
    public uint ChargingGranularity { get; }
}
