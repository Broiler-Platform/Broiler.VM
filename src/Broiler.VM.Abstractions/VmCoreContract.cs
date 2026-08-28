// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The numbered revision of the profile-neutral Broiler.VM contract: the lifecycle states and
/// legal transitions, the operation-result categories, resource authority, verified-artifact
/// ownership, guest-initiated loads, external control, and the host-capability shape.
/// </summary>
/// <remarks>
/// <para>
/// This is versioned independently of any profile format, feature manifest, or package version.
/// ADR 0003 (0003-core-contract-v1-and-amendments.md) assigns version 1, enumerates exactly what
/// the version covers, and publishes the amendment procedure that changes it. A test binds these
/// constants to that record, so the number cannot drift between documentation and code.
/// </para>
/// <para>
/// These are the only two values Broiler.VM declares at milestone VM-0. Every other type in the
/// component is a project shell without behaviour; nothing here implies that a runtime, catalog,
/// verifier, or budget exists.
/// </para>
/// </remarks>
// Broiler-AI:    Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E91C39
// Broiler-Human: PENDING
public static class VmCoreContract
{
    /// <summary>The core contract version this build implements.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0003 s1; IP=None; Security=Low; Resources=0; Fingerprint=A5EC68
    // Broiler-Human: PENDING
    public const int Version = 1;

    /// <summary>
    /// The oldest core contract version this build admits from a profile descriptor or a
    /// persisted envelope header.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0003 s5; IP=None; Security=Medium; Resources=0; Fingerprint=4EDE3F
    // Broiler-Human: PENDING
    public const int MinimumSupportedVersion = 1;
}
