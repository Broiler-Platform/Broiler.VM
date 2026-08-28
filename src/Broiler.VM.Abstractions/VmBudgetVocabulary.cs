// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           24
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The closed set of fifteen budgeted dimensions the core meters.
/// </summary>
/// <remarks>
/// <para>
/// Declaration order is normative, not cosmetic: when several dimensions are exhausted at one
/// observation point, the tie is broken by outermost scope first and then by the first dimension
/// in this order. A reordering would change which dimension a host sees named in a
/// resource-exhaustion result, so it is a breaking amendment rather than a refactor.
/// </para>
/// <para>
/// No dimension names a language concept. <c>DeclaredCount</c> is deliberately not "constants" or
/// "strings" or "opcodes": a core dimension defined in language structure would put that structure
/// into a semantics-neutral contract, against invariant 4. The profile owns which of its counts
/// pass through the guard; the core owns the guard.
/// </para>
/// <para>
/// <c>Allocation</c>, <c>HostCallCount</c> and <c>LiveRuntimeCount</c> are struck names from an
/// earlier eleven-member spelling of this set and appear nowhere.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1FCDD8
// Broiler-Human:        PENDING
public enum VmBudgetDimension
{
    /// <summary>Profile-charged abstract execution work units. Allowance.</summary>
    Fuel = 0,

    /// <summary>Attributed execution time. Allowance.</summary>
    WallClock = 1,

    /// <summary>Cumulative attributed bytes allocated. Allowance.</summary>
    AllocatedBytes = 2,

    /// <summary>Host-capability invocations. Allowance.</summary>
    HostCalls = 3,

    /// <summary>Artifact-provider requests admitted for one operation. Allowance.</summary>
    NestedLoadFanOut = 4,

    /// <summary>Provider-returned bytes for one operation. Allowance.</summary>
    NestedLoadBytes = 5,

    /// <summary>
    /// Verifier work units for one verification, including every nested verification charged to
    /// the same operation. Allowance.
    /// </summary>
    VerifierWork = 6,

    /// <summary>Attributed live (retained) bytes. Ceiling.</summary>
    LiveBytes = 7,

    /// <summary>Call and frame depth. Ceiling.</summary>
    CallDepth = 8,

    /// <summary>Provider-mediated nesting depth. Ceiling.</summary>
    NestedLoadDepth = 9,

    /// <summary>Bytes of one artifact presented to one verification. Ceiling.</summary>
    ArtifactBytes = 10,

    /// <summary>Top-level framed units one artifact may declare. Ceiling.</summary>
    SectionCount = 11,

    /// <summary>
    /// The greatest value any single untrusted declared count, length, index or offset may hold
    /// before it may size an allocation. Ceiling.
    /// </summary>
    DeclaredCount = 12,

    /// <summary>Framing nesting depth inside one artifact. Ceiling.</summary>
    StructuralDepth = 13,

    /// <summary>Concurrently live runtimes under one aggregate budget. Ceiling.</summary>
    LiveRuntimes = 14,
}

/// <summary>
/// The scope a budget is declared and enforced at. Closed at five.
/// </summary>
/// <remarks>
/// Every resource-exhaustion result names exactly one dimension and exactly one scope. The
/// tie-break is outermost scope first - <see cref="Aggregate"/>, <see cref="Runtime"/>,
/// <see cref="Artifact"/>, <see cref="Instance"/>, <see cref="Invocation"/> - so a host reading a
/// result learns which ceiling actually stopped it rather than the innermost one that happened to
/// notice.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=945A7B
// Broiler-Human:        PENDING
public enum VmBudgetScope
{
    /// <summary>One invoke or resume of one operation.</summary>
    Invocation = 0,

    /// <summary>One instantiated instance and every operation on it.</summary>
    Instance = 1,

    /// <summary>The verified handle's effective verification and instantiation ceilings.</summary>
    Artifact = 2,

    /// <summary>One runtime.</summary>
    Runtime = 3,

    /// <summary>The shared parent budget.</summary>
    Aggregate = 4,
}

/// <summary>
/// Which arithmetic a dimension obeys.
/// </summary>
/// <remarks>
/// An allowance is consumed and never refunded; a ceiling is a bound on a live measure that may go
/// down as well as up. Exposing the distinction lets a drift test assert the seven allowances and
/// eight ceilings without a hard-coded table living in test code, where it could drift from the
/// contract it is supposed to check.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3FDFEC
// Broiler-Human:        PENDING
public enum VmBudgetClass
{
    /// <summary>Consumed monotonically and never refunded.</summary>
    Allowance = 0,

    /// <summary>A bound on a live measure.</summary>
    Ceiling = 1,
}

/// <summary>
/// Whether a VM profile charges a dimension at all.
/// </summary>
/// <remarks>
/// There is no third value and no default. A descriptor that omits a row is rejected at catalog
/// construction, because an omitted row is a claim nobody made.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C7D069
// Broiler-Human:        PENDING
public enum VmBudgetApplicability
{
    /// <summary>The profile charges this dimension.</summary>
    Charged = 0,

    /// <summary>The dimension does not apply to this profile.</summary>
    NotApplicable = 1,
}

/// <summary>
/// Static facts about the fifteen dimensions: their class, and which scopes each may be declared
/// at.
/// </summary>
/// <remarks>
/// This is the one place the dimension table lives in code. Every other component - the precedence
/// algorithm, the aggregate budget, the drift tests - reads it here rather than restating it,
/// because a table restated is a table that can disagree with itself.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C44CFF
// Broiler-Human:        PENDING
public static class VmBudgetDimensions
{
    /// <summary>The number of dimensions. The set is closed; growing it is an amendment.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=None; Security=None; Resources=0; Fingerprint=74C7BA
    // Broiler-Human:        PENDING
    public const int Count = 15;

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=None; Security=Low; Resources=1; Fingerprint=B2B751
    // Broiler-Human:        PENDING
    private static readonly VmBudgetClass[] Classes =
    [
        VmBudgetClass.Allowance, // Fuel
        VmBudgetClass.Allowance, // WallClock
        VmBudgetClass.Allowance, // AllocatedBytes
        VmBudgetClass.Allowance, // HostCalls
        VmBudgetClass.Allowance, // NestedLoadFanOut
        VmBudgetClass.Allowance, // NestedLoadBytes
        VmBudgetClass.Allowance, // VerifierWork
        VmBudgetClass.Ceiling,   // LiveBytes
        VmBudgetClass.Ceiling,   // CallDepth
        VmBudgetClass.Ceiling,   // NestedLoadDepth
        VmBudgetClass.Ceiling,   // ArtifactBytes
        VmBudgetClass.Ceiling,   // SectionCount
        VmBudgetClass.Ceiling,   // DeclaredCount
        VmBudgetClass.Ceiling,   // StructuralDepth
        VmBudgetClass.Ceiling,   // LiveRuntimes
    ];

    // Exactly eleven dimensions carry aggregate scope: all seven allowances plus LiveBytes,
    // CallDepth, NestedLoadDepth and LiveRuntimes. The four artifact-shaped ceilings do not,
    // because summing "the largest declared count any one artifact may hold" across concurrent
    // runtimes measures nothing.
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=None; Security=Low; Resources=1; Fingerprint=24C557
    // Broiler-Human:        PENDING
    private static readonly bool[] Aggregate =
    [
        true, true, true, true, true, true, true,
        true, true, true,
        false, false, false, false,
        true,
    ];

    /// <summary>Every dimension, in the frozen order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=1; Fingerprint=D927EC
    // Broiler-Human:        PENDING
    public static System.ReadOnlySpan<VmBudgetDimension> All =>
    [
        VmBudgetDimension.Fuel,
        VmBudgetDimension.WallClock,
        VmBudgetDimension.AllocatedBytes,
        VmBudgetDimension.HostCalls,
        VmBudgetDimension.NestedLoadFanOut,
        VmBudgetDimension.NestedLoadBytes,
        VmBudgetDimension.VerifierWork,
        VmBudgetDimension.LiveBytes,
        VmBudgetDimension.CallDepth,
        VmBudgetDimension.NestedLoadDepth,
        VmBudgetDimension.ArtifactBytes,
        VmBudgetDimension.SectionCount,
        VmBudgetDimension.DeclaredCount,
        VmBudgetDimension.StructuralDepth,
        VmBudgetDimension.LiveRuntimes,
    ];

    /// <summary>Whether <paramref name="dimension"/> is one of the fifteen.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A37BD7
    // Broiler-Human:        PENDING
    public static bool IsDefined(VmBudgetDimension dimension) =>
        (int)dimension is >= 0 and < Count;

    /// <summary>The arithmetic <paramref name="dimension"/> obeys.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B20A36
    // Broiler-Human:        PENDING
    public static VmBudgetClass ClassOf(VmBudgetDimension dimension) =>
        Classes[Index(dimension)];

    /// <summary>Whether <paramref name="dimension"/> carries aggregate scope.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C85EB6
    // Broiler-Human:        PENDING
    public static bool CarriesAggregateScope(VmBudgetDimension dimension) =>
        Aggregate[Index(dimension)];

    /// <summary>
    /// Whether <paramref name="dimension"/> may be declared at <paramref name="scope"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C20C01
    // Broiler-Human:        PENDING
    public static bool IsDeclarableAt(VmBudgetDimension dimension, VmBudgetScope scope) => dimension switch
    {
        VmBudgetDimension.VerifierWork =>
            scope is VmBudgetScope.Runtime or VmBudgetScope.Artifact or VmBudgetScope.Aggregate,

        VmBudgetDimension.LiveBytes =>
            scope is VmBudgetScope.Runtime or VmBudgetScope.Instance or VmBudgetScope.Aggregate,

        VmBudgetDimension.ArtifactBytes or
        VmBudgetDimension.SectionCount or
        VmBudgetDimension.DeclaredCount or
        VmBudgetDimension.StructuralDepth =>
            scope is VmBudgetScope.Runtime or VmBudgetScope.Artifact,

        VmBudgetDimension.LiveRuntimes => scope is VmBudgetScope.Aggregate,

        _ => scope is VmBudgetScope.Runtime or VmBudgetScope.Instance
            or VmBudgetScope.Invocation or VmBudgetScope.Aggregate,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4EEA2C
    // Broiler-Human:        PENDING
    private static int Index(VmBudgetDimension dimension)
    {
        if (!IsDefined(dimension))
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(dimension),
                dimension,
                "The budget dimension set is closed at fifteen members.");
        }

        return (int)dimension;
    }
}
