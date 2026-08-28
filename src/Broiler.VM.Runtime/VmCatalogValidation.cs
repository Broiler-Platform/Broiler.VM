// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           5
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Resource impact:  1/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// Why a descriptor was rejected at catalog construction.
/// </summary>
/// <remarks>
/// <para>
/// This is a deliberately separate vocabulary from <see cref="VmReason"/>. One condition, one
/// surface: no test may reach a catalog reason from a load-path API or a load-path reason from a
/// catalog API, and routing any of these conditions into an operation-result category would be an
/// amendment, and a breaking one.
/// </para>
/// <para>
/// The set is explicitly open. Adding a member is not an amendment, because a composition-time
/// rejection is observed by the composition root that caused it and by nothing else - there is no
/// persisted artifact, no support table and no evidence bundle that records these numerically.
/// </para>
/// </remarks>
public enum VmCatalogValidationReason
{
    /// <summary>The profile ID does not satisfy the frozen grammar.</summary>
    ProfileIdMalformed = 0,

    /// <summary>
    /// The ID claims the reserved namespace and the entry's package ID does not. This catches
    /// accident, not forgery - exactly as a checksum detects corruption but does not authenticate.
    /// </summary>
    ProfileIdReservedNamespace = 1,

    /// <summary>Two entries declare the same ID.</summary>
    DuplicateProfileId = 2,

    /// <summary>Two entries declare IDs that differ only by ASCII case.</summary>
    ProfileIdAliasCollision = 3,

    /// <summary>The display name is absent, too long, or carries a forbidden character.</summary>
    DisplayNameMalformed = 4,

    /// <summary>The format version range is not well formed.</summary>
    FormatVersionRangeInvalid = 5,

    /// <summary>The accepted feature-manifest set is empty.</summary>
    FeatureManifestSetEmpty = 6,

    /// <summary>The accepted feature-manifest set is larger than sixty-four.</summary>
    FeatureManifestSetTooLarge = 7,

    /// <summary>A declared feature-manifest ID does not satisfy the grammar.</summary>
    FeatureManifestIdMalformed = 8,

    /// <summary>A declared feature-manifest ID does not lie under the profile's own namespace.</summary>
    FeatureManifestIdOutOfNamespace = 9,

    /// <summary>The same feature manifest is declared twice.</summary>
    DuplicateFeatureManifestId = 10,

    /// <summary>No verifier was supplied.</summary>
    MissingVerifier = 11,

    /// <summary>No executor factory was supplied.</summary>
    MissingExecutorFactory = 12,

    /// <summary>The verifier declares an identity or contract version the descriptor does not.</summary>
    VerifierIdentityMismatch = 13,

    /// <summary>A limit default is unset, unbounded, or looser than the profile's own maximum.</summary>
    LimitDefaultsInvalid = 14,

    /// <summary>A declared host-capability descriptor is not internally consistent.</summary>
    HostCapabilityDescriptorInvalid = 15,

    /// <summary>Two capability imports name the same capability ID.</summary>
    DuplicateHostCapabilityId = 16,

    /// <summary>The conformance manifest identity is absent.</summary>
    ConformanceIdentityMissing = 17,

    /// <summary>The diagnostics identity is absent or not under the profile's namespace.</summary>
    DiagnosticsIdentityMalformed = 18,

    /// <summary>The package identity is incomplete.</summary>
    PackageIdentityMissing = 19,

    /// <summary>The catalog would exceed its entry ceiling.</summary>
    CatalogTooLarge = 20,

    /// <summary>The builder has already been built. It is single-use.</summary>
    BuilderConsumed = 21,

    /// <summary>The descriptor was built against a core contract version this build does not yet support.</summary>
    CoreContractVersionNotYetSupported = 22,

    /// <summary>The descriptor was built against a core contract version this build has retired.</summary>
    CoreContractVersionRetired = 23,

    /// <summary>The verifier and the descriptor disagree on the built-against contract version.</summary>
    CoreContractBuiltAgainstMismatch = 24,

    /// <summary>The authored contract version is higher than the built-against one.</summary>
    CoreContractAuthoredExceedsBuiltAgainst = 25,

    /// <summary>The budget declaration matrix does not carry all fifteen rows.</summary>
    BudgetDeclarationMatrixIncomplete = 26,

    /// <summary>Verifier work is declared not applicable, which is illegal: verification always works.</summary>
    VerifierWorkNotApplicable = 27,

    /// <summary>A limit default is looser than the profile's own hard maximum for that dimension.</summary>
    ProfileDefaultExceedsProfileMaximum = 28,

    /// <summary>A declared guest-load declaration is missing a mandatory part.</summary>
    GuestLoadDeclarationIncomplete = 29,

    /// <summary>A declared guest-load maximum is not finite.</summary>
    GuestLoadMaximumUnbounded = 30,

    /// <summary>The verifier-work-to-fuel rate is not positive and finite.</summary>
    VerifierWorkToFuelRateInvalid = 31,

    /// <summary>The payload kind ID range is not well formed.</summary>
    PayloadKindIdRangeInvalid = 32,

    /// <summary>The charging granularity is below one.</summary>
    ChargingGranularityInvalid = 33,

    /// <summary>The maximum uncharged work is below one.</summary>
    MaxUnchargedWorkInvalid = 34,

    /// <summary>The descriptor revision is below one.</summary>
    DescriptorRevisionInvalid = 35,

    /// <summary>A null descriptor was offered.</summary>
    DescriptorMissing = 36,
}

/// <summary>
/// The composition-time failure. Catalog construction throws rather than returning a result.
/// </summary>
/// <remarks>
/// <para>
/// A catalog is authored by a composition root from trusted compile-time data, so a defect there is
/// a wiring bug: it must be loud and unrecoverable, and it must name the registration call that
/// caused it. That is why single-descriptor rules run eagerly at <c>Add</c> - so the exception's
/// stack points at the offending line - and why there is no <c>VmCatalogResult</c> type.
/// </para>
/// <para>
/// <c>Build</c> never collects every error and throws once; the core never logs a warning and skips
/// an entry; and no option, flag or environment variable tolerates, downgrades or suppresses an
/// admission failure.
/// </para>
/// </remarks>
public sealed class VmCatalogValidationException : System.Exception
{
    /// <summary>Creates a failure naming the offending profile ID.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s8; IP=Low; Security=Low; Resources=1; Fingerprint=F47996
    // Broiler-Human: PENDING
    public VmCatalogValidationException(VmCatalogValidationReason reason, VmProfileId offendingProfileId, string offendingField)
        : base(Describe(reason, offendingProfileId.ToString(), offendingField))
    {
        Reason = reason;
        OffendingProfileId = offendingProfileId;
        HasOffendingProfileId = true;
        OffendingOrdinalPosition = -1;
        OffendingField = offendingField;
    }

    /// <summary>
    /// Creates a failure naming the offending registration by position, for the case where the ID
    /// itself is what is malformed and so cannot identify anything.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s8; IP=Low; Security=Low; Resources=1; Fingerprint=64A960
    // Broiler-Human: PENDING
    public VmCatalogValidationException(VmCatalogValidationReason reason, int offendingOrdinalPosition, string offendingField)
        : base(Describe(reason, "entry #" + offendingOrdinalPosition, offendingField))
    {
        Reason = reason;
        HasOffendingProfileId = false;
        OffendingOrdinalPosition = offendingOrdinalPosition;
        OffendingField = offendingField;
    }

    /// <summary>Why the descriptor was rejected.</summary>
    public VmCatalogValidationReason Reason { get; }

    /// <summary>
    /// Whether the offending entry is identified by ID. A discriminated carrier rather than a
    /// nullable ID beside a nullable index, so a consumer cannot read both or neither.
    /// </summary>
    public bool HasOffendingProfileId { get; }

    /// <summary>The offending profile ID, meaningful only when <see cref="HasOffendingProfileId"/>.</summary>
    public VmProfileId OffendingProfileId { get; }

    /// <summary>The offending registration's position, meaningful only when the ID is malformed.</summary>
    public int OffendingOrdinalPosition { get; }

    /// <summary>The descriptor field at fault.</summary>
    public string OffendingField { get; }

    // Broiler-AI:    Origin=AI; Spec=ADR-0002 s8; IP=Low; Security=Low; Resources=1; Fingerprint=9BED92
    // Broiler-Human: PENDING
    private static string Describe(VmCatalogValidationReason reason, string subject, string field) =>
        "Broiler.VM catalog validation failed for " + subject + ": " + reason + " (field " + field + ").";
}
