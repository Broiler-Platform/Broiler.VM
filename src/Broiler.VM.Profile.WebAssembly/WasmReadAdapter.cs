// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           2
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  1/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The projection between the contract's metering surface and the bounded reader's, and the one
/// between an effective limit vector and the four artifact-shaped ceilings.
/// </summary>
/// <remarks>
/// <para>
/// <c>Broiler.VM.Binary</c> names no contract vocabulary, so the party holding both performs the
/// projection. Writing it is this profile's work and not the core's.
/// </para>
/// <para>
/// <b>This one carries a fifth member the shape it was copied from does not have.</b> The bounded
/// reader's callback surface has four members because the reader charges four things: allocated
/// bytes, released bytes, verifier work and the poll. A declared count is the fifth, and it is
/// charged here rather than there because this profile reads its own variable-length integers and
/// therefore owns the count-bound comparison the core's reader would otherwise have performed. The
/// declared-count dimension is the one budget row this deviation moves from the core's side of the
/// line to this profile's, and this member is where that move is spelled.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7779B6
// Broiler-Falsified-If: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
// Broiler-Human:        PENDING
public sealed class WasmReadAdapter : IVmBoundedAllocationMeter
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    /// <summary>
    /// The four artifact-shaped ceilings this verification runs under, held where a reference to
    /// them outlives the readers built from them.
    /// </summary>
    /// <remarks>
    /// <b>IT IS A FIELD OF A CLASS ON PURPOSE AND THE REASON IS THE REF-SAFETY RULES.</b> A bounded
    /// reader takes its ceilings by reference and a reader is a ref struct, so a caller that is
    /// itself a ref struct cannot hand it one of its OWN fields: a struct's <c>this</c> is scoped,
    /// and the compiler cannot see that the two live and die together. A field of a heap object has
    /// no such limit, so the ceilings are materialized once, here, beside the meter they are spent
    /// against - which is also where the projection that computes them already lived.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1BB2CD
    // Broiler-Falsified-If: a reader is built from ceilings other than these, so two passes over one artifact run under different bounds
    // Broiler-Human:        PENDING
    internal readonly VmReadBounds Ceilings;

    /// <summary>Wraps the contract meter the core supplied and the ceilings it runs under.</summary>
    /// <remarks>
    /// The ceilings are projected here rather than by the caller so that the materialization has one
    /// site: every reader this profile builds is built from this value, and there is no second
    /// place where a limit vector could be turned into bounds a little differently.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=035C43
    // Broiler-Falsified-If: a payload byte is read before this constructor has run
    // Broiler-Human:        PENDING
    public WasmReadAdapter(IVmMeter contractMeter, VmLimitVector verificationCeilings)
    {
        meter = contractMeter;
        Ceilings = ToReadBounds(verificationCeilings);
    }

    /// <summary>Projects the four artifact-shaped ceilings out of an effective limit vector.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8F35EC
    // Broiler-Human:        PENDING
    public static VmReadBounds ToReadBounds(VmLimitVector limits) =>
        new(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FFFA26
    // Broiler-Human:        PENDING
    public bool TryReserve(ulong byteCount) =>
        meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E74062
    // Broiler-Human:        PENDING
    public void Release(ulong byteCount) =>
        meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4E1E45
    // Broiler-Human:        PENDING
    public bool TryChargeWork(ulong workUnits) =>
        meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=06A1AC
    // Broiler-Human:        PENDING
    public bool Poll() => meter.Poll();

    /// <summary>
    /// Charges one vector length against the declared-count ceiling.
    /// </summary>
    /// <remarks>
    /// It is called by the bounded-count reader and by nothing else, so every count this profile
    /// reads is charged at one site. A refusal is a ceiling reached and reaches the caller as a
    /// resource exhaustion naming <c>DeclaredCount</c>; it is never reported as a malformed
    /// artifact, because the module is not malformed and the host declined to spend the count.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1A2D6D
    // Broiler-Falsified-If: a declared count is charged anywhere but through this member, or its refusal is reported as a malformed artifact
    // Broiler-Human:        PENDING
    public bool TryChargeDeclaredCount(ulong count) =>
        meter.TryCharge(VmBudgetDimension.DeclaredCount, count);

    /// <summary>
    /// Takes one level of control nesting against the structural-depth ceiling.
    /// </summary>
    /// <remarks>
    /// <b>IT IS HALF OF A PAIR AND IT IS MEANINGLESS ALONE.</b> Structural depth is a high-water
    /// mark and not a running total: a body of ten thousand blocks that never nests two deep is one
    /// level deep, and a charge that was never released would make it ten thousand. So every call
    /// here is matched by a call to <see cref="ReleaseStructuralDepth"/> when the level closes, and
    /// the ceiling a host set is a statement about how deep an artifact may nest rather than about
    /// how many blocks it may contain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1D9970
    // Broiler-Falsified-If: a level charged here is not released when it closes, so nesting is counted as a running total
    // Broiler-Human:        PENDING
    public bool TryChargeStructuralDepth(ulong levels) =>
        meter.TryCharge(VmBudgetDimension.StructuralDepth, levels);

    /// <summary>Gives back levels of control nesting this adapter took.</summary>
    /// <remarks>
    /// It reports a release rather than a charge because only what
    /// <see cref="TryChargeStructuralDepth"/> took may come back, and the contract has no way to
    /// spell a refund.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0E44B7
    // Broiler-Falsified-If: it releases more levels than were charged, so a ceiling admits a nesting it should refuse
    // Broiler-Human:        PENDING
    public void ReleaseStructuralDepth(ulong levels) =>
        meter.ReportReleased(VmBudgetDimension.StructuralDepth, levels);
}
