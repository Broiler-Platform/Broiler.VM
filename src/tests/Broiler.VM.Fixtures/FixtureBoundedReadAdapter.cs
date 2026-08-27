namespace Broiler.VM.Fixtures;

/// <summary>
/// The projection every profile needs between the contract's metering surface and the bounded
/// reader's.
/// </summary>
/// <remarks>
/// <para>
/// <c>Broiler.VM.Binary</c> deliberately names no contract vocabulary - bounded reading is mechanism
/// and must not acquire it - so its allocation meter takes plain byte counts and its bounds take
/// plain numbers. The party that holds both vocabularies performs the projection. For the core that
/// is its own meter; for a profile it is these four lines, shipped once here so that no profile
/// author invents a different version of them.
/// </para>
/// <para>
/// This is the visible price of keeping the mechanism assembly a graph sink, and it is a small one:
/// a mapping from two dimensions onto two methods, and a mapping from a limit vector onto four
/// numbers.
/// </para>
/// </remarks>
public sealed class FixtureBoundedReadAdapter : IVmBoundedAllocationMeter
{
    private readonly IVmMeter meter;

    /// <summary>Wraps a contract meter as a bounded-reading meter.</summary>
    public FixtureBoundedReadAdapter(IVmMeter meter) => this.meter = meter;

    /// <summary>Projects the four artifact-shaped ceilings out of an effective limit vector.</summary>
    public static VmReadBounds ToReadBounds(VmLimitVector limits) =>
        new(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

    /// <inheritdoc/>
    public bool TryReserve(ulong byteCount) =>
        meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public void Release(ulong byteCount) =>
        meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public bool TryChargeWork(ulong workUnits) =>
        meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    public bool Poll() => meter.Poll();
}
