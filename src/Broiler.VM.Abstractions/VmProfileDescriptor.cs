// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           32
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Abstractions;

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
/// <remarks>Creates a descriptor. Every row of the frozen table is supplied.</remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6BF23B
// Broiler-Human:        PENDING
public sealed class VmProfileDescriptor(
    VmProfileId profileId,
    string displayName,
    int descriptorRevision,
    VmFormatVersionRange supportedFormatVersions,
    ImmutableArray<VmFeatureManifestId> acceptedFeatureManifests,
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
    ImmutableArray<VmCapabilityImport> hostCapabilityDescriptors,
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
    /// <summary>The most feature manifests one descriptor may accept.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0002 s7; IP=None; Security=Medium; Resources=0; Fingerprint=EDA3BD
    // Broiler-Human:        PENDING
    public const int MaximumAcceptedFeatureManifests = 64;

    /// <summary>The most characters a display name may have.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0002 s7; IP=None; Security=Medium; Resources=0; Fingerprint=948C28
    // Broiler-Human:        PENDING
    public const int MaximumDisplayNameLength = 64;

    /// <summary>Row 1. The profile's stable identity.</summary>
    public VmProfileId ProfileId { get; } = profileId;

    /// <summary>
    /// Row 2. A required human-readable label of 1 to 64 UTF-16 units, non-localized and
    /// mechanically inert: never compared, folded, sorted, or used for lookup, uniqueness,
    /// ordering, cache keys, handle identity or envelope dispatch. Two entries may share one.
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>Row 3. Incremented whenever anything that can affect verification changes.</summary>
    public int DescriptorRevision { get; } = descriptorRevision;

    /// <summary>Row 4. The inclusive profile-format version range.</summary>
    public VmFormatVersionRange SupportedFormatVersions { get; } = supportedFormatVersions;

    /// <summary>Row 5. One to sixty-four manifests, normalized to ascending ordinal order.</summary>
    public ImmutableArray<VmFeatureManifestId> AcceptedFeatureManifests { get; } = acceptedFeatureManifests;

    /// <summary>Row 6. The verifier, referenced directly rather than named.</summary>
    public IVmProfileVerifier Verifier { get; } = verifier;

    /// <summary>Row 7. The per-runtime executor factory, trim-rooted by direct reference.</summary>
    public VmExecutorFactory ExecutorFactory { get; } = executorFactory;

    /// <summary>Row 8. Snapshot or decoded.</summary>
    public VmArtifactRepresentationKind ArtifactRepresentationKind { get; } = artifactRepresentationKind;

    /// <summary>Row 9. Whether a verified artifact owns disposable resources.</summary>
    public VmArtifactLifetimeKind ArtifactLifetimeKind { get; } = artifactLifetimeKind;

    /// <summary>Row 10. Whether two verifications may run concurrently in one runtime.</summary>
    public bool SupportsConcurrentVerification { get; } = supportsConcurrentVerification;

    /// <summary>Row 11. The profile's declared thread affinity.</summary>
    public VmThreadAffinity ThreadAffinity { get; } = threadAffinity;

    /// <summary>
    /// Row 12. The bound, in the profile's own work units, on work between two polls. It is what
    /// makes cancellation latency bounded in declared work units rather than in wall-clock time the
    /// core cannot promise.
    /// </summary>
    public ulong CancellationPollBound { get; } = cancellationPollBound;

    /// <summary>Row 13. The bounded allowance the profile gets for its terminal unwind.</summary>
    public ulong AbandonBudget { get; } = abandonBudget;

    /// <summary>
    /// Row 14. Per-dimension bounded defaults a host may explicitly adopt. No member may encode
    /// "unbounded" or "unset": a default that meant either would make omission mean unbounded,
    /// which invariant 9 forbids outright.
    /// </summary>
    public VmLimitVector LimitDefaults { get; } = limitDefaults;

    /// <summary>Row 15. Per-dimension hard maxima the profile imposes on itself.</summary>
    public VmLimitVector ProfileHardMaxima { get; } = profileHardMaxima;

    /// <summary>Row 16. Which dimensions the profile charges.</summary>
    public VmBudgetDeclarationMatrix BudgetDeclarationMatrix { get; } = budgetDeclarationMatrix;

    /// <summary>
    /// Row 17. The capability imports, possibly empty. Empty means the profile imports nothing,
    /// which is a legal and expected state rather than an omission.
    /// </summary>
    public ImmutableArray<VmCapabilityImport> HostCapabilityDescriptors { get; } = hostCapabilityDescriptors;

    /// <summary>Row 18. Whether the profile may request code while executing, and under what bounds.</summary>
    public VmGuestLoadDeclaration GuestInitiatedLoads { get; } = guestInitiatedLoads;

    /// <summary>Row 19. Whether instantiation may suspend.</summary>
    public VmDeclaration AsynchronousInstantiation { get; } = asynchronousInstantiation;

    /// <summary>Row 20. Whether the host may suspend an operation from outside.</summary>
    public VmDeclaration ExternalSuspension { get; } = externalSuspension;

    /// <summary>Row 21. The closed range of payload kind IDs the profile may stamp.</summary>
    public VmPayloadKindIdRange PayloadKindIdRange { get; } = payloadKindIdRange;

    /// <summary>Row 22. The contract version the profile was compiled against; machine-derived.</summary>
    public int BuiltAgainstCoreContractVersion { get; } = builtAgainstCoreContractVersion;

    /// <summary>Row 23. The contract version the author wrote for; never derived from a constant.</summary>
    public int AuthoredCoreContractVersion { get; } = authoredCoreContractVersion;

    /// <summary>Row 24. The conformance corpus identity, for support tables and evidence only.</summary>
    public VmConformanceManifestId ConformanceManifestId { get; } = conformanceManifestId;

    /// <summary>Row 24. Its version.</summary>
    public int ConformanceManifestVersion { get; } = conformanceManifestVersion;

    /// <summary>Row 25. The profile's diagnostics token, under its own ID namespace.</summary>
    public VmDiagnosticsIdentity DiagnosticsIdentity { get; } = diagnosticsIdentity;

    /// <summary>Row 26. Package, version and owner tag, used by architecture and release checks.</summary>
    public VmPackageIdentity PackageIdentity { get; } = packageIdentity;

    /// <summary>Row 27. Whether one verified artifact may serve more than one runtime.</summary>
    public VmArtifactSharing ArtifactSharing { get; } = artifactSharing;

    /// <summary>Row 28. What a profile fault does to the instance that produced it.</summary>
    public VmFaultRecovery FaultRecovery { get; } = faultRecovery;

    /// <summary>Row 29. The bound on work performed between two polls, in the profile's own units.</summary>
    public uint MaxUnchargedWork { get; } = maxUnchargedWork;

    /// <summary>Row 30. The granularity at which the profile charges, in the same units.</summary>
    public uint ChargingGranularity { get; } = chargingGranularity;
}
