// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           9
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         5/4
// Resource impact:  0/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>What kind of answer a <see cref="UbcRefusal"/> carries.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=317287
// Broiler-Human:        PENDING
public enum UbcRefusalKind : byte
{
    /// <summary>The artifact is invalid: a core reason, a diagnostic code and a position.</summary>
    Invalid = 0,

    /// <summary>A budget dimension refused: an exhaustion at artifact scope.</summary>
    Exhausted = 1,
}

/// <summary>
/// A refusal of the reader or the walk, before it becomes the core's verifier outcome: exactly one of
/// an invalid artifact carrying a core reason, a diagnostic code and a position, or a resource
/// exhaustion naming one budget dimension at artifact scope.
/// </summary>
/// <remarks>
/// <para>
/// <b>Positions.</b> <see cref="VmSourcePosition.SectionIndex"/> is the kind byte of the section the
/// refusal is about (<see cref="UbcSectionKind"/>), or minus one for the header.
/// <see cref="VmSourcePosition.ByteOffset"/> is the offset in the payload the reader had reached, or,
/// for a refusal of the walk, the absolute offset of the instruction in the Code section.
/// <see cref="VmSourcePosition.ProfileCoordinate0"/> is the unit index a walk refusal is about, and minus
/// one otherwise. The second coordinate is the index of the row a refusal of a decoded section's row is
/// about - its offset is then zero, because the row was read before the walk refused it - and minus one
/// otherwise.
/// </para>
/// <para>
/// A refusal the family's hook gives carries a family code outside the universal range, and is built
/// through the same factory.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=395B7D
// Broiler-Human:        PENDING
public readonly struct UbcRefusal
{
    private UbcRefusal(UbcRefusalKind kind, int code, VmReason reason, VmSourcePosition position, VmBudgetDimension dimension)
    {
        Kind = kind;
        Code = code;
        Reason = reason;
        Position = position;
        Dimension = dimension;
    }

    /// <summary>What the refusal is.</summary>
    public UbcRefusalKind Kind { get; }

    /// <summary>For an invalid artifact, the diagnostic code: a <see cref="UbcDiagnosticCode"/>, or a family's own.</summary>
    public int Code { get; }

    /// <summary>For an invalid artifact, the core reason.</summary>
    public VmReason Reason { get; }

    /// <summary>For an invalid artifact, where.</summary>
    public VmSourcePosition Position { get; }

    /// <summary>For an exhaustion, the dimension that refused.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>An invalid artifact, refused by the universal bytecode.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B6E9D4
    // Broiler-Human:        PENDING
    public static UbcRefusal Invalid(UbcDiagnosticCode code, VmReason reason, VmSourcePosition position) =>
        new(UbcRefusalKind.Invalid, (int)code, reason, position, VmBudgetDimension.Fuel);

    /// <summary>An invalid artifact, refused by a family's hook with the family's own code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F24C9B
    // Broiler-Human:        PENDING
    public static UbcRefusal InvalidByFamily(int familyCode, VmReason reason, VmSourcePosition position) =>
        new(UbcRefusalKind.Invalid, familyCode, reason, position, VmBudgetDimension.Fuel);

    /// <summary>An exhaustion of <paramref name="dimension"/> at artifact scope.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C7C9D3
    // Broiler-Human:        PENDING
    public static UbcRefusal Exhausted(VmBudgetDimension dimension) =>
        new(UbcRefusalKind.Exhausted, 0, VmReason.None, default, dimension);

    /// <summary>The core's verifier outcome for this refusal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=047645
    // Broiler-Falsified-If: an exhaustion is answered as an invalid artifact, or the reverse
    // Broiler-Human:        PENDING
    public VmVerifierOutcome ToOutcome() => Kind == UbcRefusalKind.Exhausted
        ? VmVerifierOutcome.ResourceExhaustion(Dimension, VmBudgetScope.Artifact)
        : VmVerifierOutcome.InvalidArtifact(Reason, Code, Position);

    /// <summary>The position a header refusal carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A8F246
    // Broiler-Human:        PENDING
    public static VmSourcePosition InHeader(ulong offset) => new(-1, offset, -1, -1);

    /// <summary>The position a refusal about a section carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=49824A
    // Broiler-Human:        PENDING
    public static VmSourcePosition InSection(UbcSectionKind kind, ulong offset) => new((int)kind, offset, -1, -1);

    /// <summary>The position a refusal of the walk carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=98B1C1
    // Broiler-Human:        PENDING
    public static VmSourcePosition InCode(int unit, uint codeOffset) => new((int)UbcSectionKind.Code, codeOffset, unit, -1);

    /// <summary>The position a refusal of one decoded row carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5943E8
    // Broiler-Human:        PENDING
    public static VmSourcePosition InRow(UbcSectionKind kind, int row) => new((int)kind, 0, -1, row);
}

/// <summary>
/// The bounded-reading meter the reader and the walk charge through: the canonical projection of ADR
/// 0011 onto the core meter, written once more here because the canonical copy is test-only.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=High; Resources=0; Fingerprint=A570A8
// Broiler-Falsified-If: a member charges a dimension other than the one ADR 0011's projection names for it
// Broiler-Human:        PENDING
internal sealed class UbcReadAdapter : IVmBoundedAllocationMeter
{
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AE850E
    // Broiler-Human:        PENDING
    internal UbcReadAdapter(IVmMeter contractMeter) =>
        meter = contractMeter ?? throw new System.ArgumentNullException(nameof(contractMeter));

    /// <summary>Projects a limit vector onto the four ceilings a bounded read is performed under.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=High; Resources=0; Fingerprint=A50120
    // Broiler-Falsified-If: a ceiling is read from a dimension other than its own
    // Broiler-Human:        PENDING
    internal static VmReadBounds ToReadBounds(VmLimitVector limits) => new(
        limits[VmBudgetDimension.ArtifactBytes],
        limits[VmBudgetDimension.SectionCount],
        limits[VmBudgetDimension.DeclaredCount],
        limits[VmBudgetDimension.StructuralDepth]);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=FFFA26
    // Broiler-Falsified-If: a reservation is charged to a dimension other than allocated bytes
    // Broiler-Human:        PENDING
    public bool TryReserve(ulong byteCount) => meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E74062
    // Broiler-Human:        PENDING
    public void Release(ulong byteCount) => meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=4E1E45
    // Broiler-Falsified-If: verifier work is charged to a dimension other than verifier work
    // Broiler-Human:        PENDING
    public bool TryChargeWork(ulong workUnits) => meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=06A1AC
    // Broiler-Human:        PENDING
    public bool Poll() => meter.Poll();
}
