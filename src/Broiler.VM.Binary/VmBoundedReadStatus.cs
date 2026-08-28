// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           9
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// Why a bounded read stopped. This is mechanism vocabulary and deliberately not a contract
/// reason: the caller that owns the contract maps it onto an outcome and a reason.
/// </summary>
/// <remarks>
/// The mapping the core and every profile verifier apply is fixed by ADR 0006 and ADR 0007:
/// <see cref="Truncated"/>, <see cref="MalformedEncoding"/>, <see cref="SectionCountExceeded"/> and
/// <see cref="StructuralDepthExceeded"/> are invalid-artifact conditions, while
/// <see cref="DeclaredCountExceeded"/>, <see cref="ArtifactBytesExceeded"/>,
/// <see cref="AllocationRefused"/> and <see cref="WorkBudgetExhausted"/> are resource-exhaustion
/// conditions naming a dimension and a scope. Encoding that mapping here would put contract
/// vocabulary into a mechanism assembly, which ADR 0001 forbids.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F5396D
// Broiler-Human:        PENDING
public enum VmBoundedReadStatus
{
    /// <summary>No bound has been reached; the reader is usable.</summary>
    Ok = 0,

    /// <summary>The read ran past the end of the supplied bytes.</summary>
    Truncated = 1,

    /// <summary>A variable-length encoding was not well formed.</summary>
    MalformedEncoding = 2,

    /// <summary>An untrusted declared count exceeded its configured bound.</summary>
    DeclaredCountExceeded = 3,

    /// <summary>More sections were entered than the configured bound permits.</summary>
    SectionCountExceeded = 4,

    /// <summary>Section nesting went deeper than the configured bound permits.</summary>
    StructuralDepthExceeded = 5,

    /// <summary>The supplied byte range is larger than the configured artifact bound.</summary>
    ArtifactBytesExceeded = 6,

    /// <summary>The meter refused an allocation reservation.</summary>
    AllocationRefused = 7,

    /// <summary>The meter refused a work charge, or a poll said stop.</summary>
    WorkBudgetExhausted = 8,
}
