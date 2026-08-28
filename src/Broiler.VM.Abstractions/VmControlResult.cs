// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           12
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>The four kinds a control operation can answer with.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=970DEC
// Broiler-Human:        PENDING
public enum VmControlOutcome
{
    /// <summary>The request was accepted and the state changed.</summary>
    Accepted = 0,

    /// <summary>The request was legal and there was nothing to do.</summary>
    NoOp = 1,

    /// <summary>The request is not legal against the target in its current state.</summary>
    InvalidState = 2,

    /// <summary>
    /// The capability the request needs was never declared or never enabled. Invariant 8 makes this
    /// distinct from an illegal transition: a missing capability is a truthful absence, not a state
    /// error.
    /// </summary>
    Unsupported = 3,
}

/// <summary>
/// What every control operation returns: disposal, cancellation and suspension requests, deadline
/// polling, and lease acquire and release.
/// </summary>
/// <remarks>
/// <para>
/// A control operation is not a stage. It appears in no row of the stage matrix, carries no
/// <see cref="VmOutcome"/> and no profile payload, charges nothing, and is never refused for
/// exhaustion. Disposal that cannot fail for lack of budget is the reason: a runtime that could not
/// afford to be disposed would be unbounded in exactly the way the budget contract exists to
/// prevent.
/// </para>
/// <para>
/// <strong>A note on shape.</strong> ADR 0003's frozen public-name table records this as an
/// <c>enum {Accepted, NoOp, InvalidState, Unsupported}</c>, while ADR 0004 requires it to carry
/// exactly one reason code - a lifecycle reason when the kind is <see cref="VmControlOutcome.InvalidState"/>,
/// and the missing declaration when it is <see cref="VmControlOutcome.Unsupported"/> - and ADR 0009
/// requires it to distinguish an undeclared external suspension from an unenabled one. A bare enum
/// cannot carry a reason and cannot make that distinction, so the type is a readonly struct over
/// the frozen four-member kind. The kind enum keeps the frozen name and members as
/// <see cref="VmControlOutcome"/>; the discrepancy is recorded as an erratum against the name
/// table rather than resolved by dropping a requirement.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=ACBD34
// Broiler-Human:        PENDING
public readonly struct VmControlResult : System.IEquatable<VmControlResult>
{
    private VmControlResult(VmControlOutcome kind, VmReason reason)
    {
        Kind = kind;
        Reason = reason;
    }

    /// <summary>The request was accepted.</summary>
    public static VmControlResult Accepted { get; } = new(VmControlOutcome.Accepted, VmReason.None);

    /// <summary>The request was legal and there was nothing to do. Idempotent disposal answers this.</summary>
    public static VmControlResult NoOp { get; } = new(VmControlOutcome.NoOp, VmReason.None);

    /// <summary>The request is not legal from the current state, for the given lifecycle reason.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=738825
    // Broiler-Human:        PENDING
    public static VmControlResult InvalidState(VmReason reason) =>
        new(VmControlOutcome.InvalidState, reason);

    /// <summary>
    /// The capability was not declared or not enabled. The reason names which of the two, so a
    /// host can tell "this profile cannot" from "this composition did not".
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DF3206
    // Broiler-Human:        PENDING
    public static VmControlResult Unsupported(VmReason missingDeclaration) =>
        new(VmControlOutcome.Unsupported, missingDeclaration);

    /// <summary>Which of the four kinds this is.</summary>
    public VmControlOutcome Kind { get; }

    /// <summary>The single reason code, or <see cref="VmReason.None"/> for the two success kinds.</summary>
    public VmReason Reason { get; }

    /// <summary>True for <see cref="VmControlOutcome.Accepted"/> and <see cref="VmControlOutcome.NoOp"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F848E2
    // Broiler-Human:        PENDING
    public bool IsSuccess => Kind is VmControlOutcome.Accepted or VmControlOutcome.NoOp;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EBA962
    // Broiler-Human:        PENDING
    public bool Equals(VmControlResult other) => Kind == other.Kind && Reason == other.Reason;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmControlResult other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9DE490
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine((int)Kind, (int)Reason);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8B436E
    // Broiler-Human:        PENDING
    public override string ToString() =>
        Reason is VmReason.None ? Kind.ToString() : Kind + "/" + Reason;

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmControlResult left, VmControlResult right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FC4ADD
    // Broiler-Human:        PENDING
    public static bool operator !=(VmControlResult left, VmControlResult right) => !left.Equals(right);
}

/// <summary>
/// Thrown when the core detects a violation of one of its own invariants.
/// </summary>
/// <remarks>
/// <para>
/// This is not an error channel. Every condition a caller or a profile can cause is a returned
/// result; this exception means the core itself is in a state it does not know how to describe, and
/// the affected runtime transitions to its terminal poisoned state so that every later call on it
/// returns an invalid-state result rather than continuing on a broken invariant.
/// </para>
/// <para>
/// It is one of exactly three exception types that may escape a public member. The other two are
/// <see cref="System.ArgumentException"/> and its derivatives, from pure guard clauses only, and
/// <c>VmCatalogValidationException</c> from composition, which lives beside the catalog in
/// <c>Broiler.VM.Runtime</c> because nothing a profile package names mentions it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=98445A
// Broiler-Human:        PENDING
public sealed class VmCoreDefectException : System.Exception
{
    /// <summary>Creates a defect report naming the runtime that was poisoned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6AF01B
    // Broiler-Human:        PENDING
    public VmCoreDefectException(string message, VmObjectId runtimeId)
        : base(message) => RuntimeId = runtimeId;

    /// <summary>Creates a defect report with no runtime attributed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AB41C2
    // Broiler-Human:        PENDING
    public VmCoreDefectException(string message)
        : base(message)
    {
    }

    /// <summary>The runtime that was poisoned, or the empty identity where none was.</summary>
    public VmObjectId RuntimeId { get; }
}
