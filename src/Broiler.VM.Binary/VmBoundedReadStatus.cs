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
/// <para>
/// The mapping the core and every profile verifier apply follows ADR 0005's malformed-versus-bounded
/// rule - a value that contradicts the format is invalid-artifact, a value that is well formed but
/// exceeds a configured bound is resource-exhaustion, and where both apply at one point
/// invalid-artifact wins - and roadmap section 7, which rules the whole set of statuses in one
/// place. <see cref="Truncated"/> and <see cref="MalformedEncoding"/> are the invalid-artifact
/// conditions. <see cref="ArtifactBytesExceeded"/>, <see cref="SectionCountExceeded"/>,
/// <see cref="DeclaredCountExceeded"/>, <see cref="StructuralDepthExceeded"/>,
/// <see cref="AllocationRefused"/> and <see cref="WorkBudgetExhausted"/> are resource-exhaustion
/// conditions naming a dimension and a scope. Encoding that mapping here would put contract
/// vocabulary into a mechanism assembly, which ADR 0001 forbids.
/// </para>
/// <para>
/// The four ceiling statuses are together on the resource side because the reader puts them there.
/// Each is a comparison against a field of the caller-supplied <see cref="VmReadBounds"/> - the four
/// artifact-shaped ceilings projected out of the host, profile and artifact intersection - and none
/// examines the bytes. The ordering inside <c>TryEnterSection</c> is the proof: the intrinsic test,
/// whether a declared section fits in what remains, runs first and reports <see cref="Truncated"/>,
/// so a section-count or structural-depth breach is only ever reached on bytes whose framing has
/// already been shown to be sound. The same bytes pass or fail on the value of a host number, which
/// is what resource-exhaustion means and what invalid-artifact must not be made to mean.
/// </para>
/// <para>
/// This paragraph previously said the mapping was "fixed by ADR 0006 and ADR 0007" and placed
/// <see cref="SectionCountExceeded"/> and <see cref="StructuralDepthExceeded"/> on the
/// invalid-artifact side. Neither record mentions either status; the ruling is ADR 0005's. The
/// misattribution had consequences, which is why it is noted rather than quietly corrected: three
/// verifiers in this repository implemented three different mappings from it, and a corpus entry
/// pinned one of them as expected behaviour.
/// </para>
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
