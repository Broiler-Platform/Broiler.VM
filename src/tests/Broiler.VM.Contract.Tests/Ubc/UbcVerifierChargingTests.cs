using System.Collections.Immutable;
using System.Numerics;
using Broiler.VM;
using Broiler.VM.Ubc;
using O = Broiler.VM.Ubc.UbcOpcode;
using Op = Broiler.VM.Contract.Tests.UbcCorpusFamily.Op;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The walk's charging promises, held against a meter that refuses at a chosen point: the work of a pass
/// or a search is paid before it runs, the work a family hook charges is counted toward the poll bound
/// with the walk's own, and what the walk keeps per jump table is reserved before it is allocated.
/// </summary>
/// <remarks>
/// <para>
/// <b>One short.</b> A walk refusal is found by some pass or search. When that work is paid before it
/// runs, an allowance one unit short of what the unbounded verification spent stops the walk at that
/// work's own charge - the charge the meter refuses first is the pass's or the search's, of the amount
/// it costs - and the refusal the work would have found is never reached. When the work is paid after
/// it runs, or not at all, the charge refused is the one before it.
/// </para>
/// <para>
/// The family here declares an uncharged-work bound far above what these artifacts cost, so no charge
/// is split into pieces and the charge the meter refuses is a whole one.
/// </para>
/// </remarks>
public sealed class UbcVerifierChargingTests
{
    private const uint Unsplit = 65536;

    private static readonly VmLimitVector OpenCeilings = UbcCorpusRunner.Ceilings(new UbcCorpusConfiguration(
        ceilings: ImmutableSortedDictionary<VmBudgetDimension, ulong>.Empty
            .Add(VmBudgetDimension.AllocatedBytes, 1UL << 40)
            .Add(VmBudgetDimension.VerifierWork, 1UL << 40)
            .Add(VmBudgetDimension.ArtifactBytes, 1UL << 40)
            .Add(VmBudgetDimension.SectionCount, uint.MaxValue)
            .Add(VmBudgetDimension.DeclaredCount, uint.MaxValue)
            .Add(VmBudgetDimension.StructuralDepth, uint.MaxValue)));

    // ---- a hook's work and the poll bound (finding 0) -------------------------------------------------

    [Fact]
    public void A_Hook_Charging_Fuel_Up_To_The_Bound_Verifies_Every_Control_Without_Breaching_The_Poll_Bound()
    {
        // The core's meter counts fuel as work toward the poll bound, as it counts verifier work. A hook
        // that charges a whole bound of fuel at once, at Begin and at every instruction, stays within
        // the bound only if the walk counts that fuel with its own work and polls before adding more.
        var bound = UbcCorpusFamily.MaxUnchargedWork;
        var failures = new List<string>();

        foreach (var (id, spec, configuration) in UbcCorpus.Controls())
        {
            var hook = new ChargingHook(new UbcCorpusHook(), VmBudgetDimension.Fuel, bound);
            var meter = new RecordingMeter(pollBound: bound);
            var outcome = Verify(Profile(hook, bound), spec.Bytes(), meter, configuration.DescriptorManifest);

            if (outcome.Category is not VmOutcome.Normal || meter.PollBoundExceeded || meter.MostWorkBetweenPolls > bound)
            {
                failures.Add($"{id}: {outcome.Category} {outcome.ExhaustedDimension}, {meter.MostWorkBetweenPolls} work units between two polls, bound exceeded {meter.PollBoundExceeded}");
            }

            Assert.True(meter.Fuel >= bound, $"{id}: the hook charged {meter.Fuel} fuel");
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void A_Hook_Charge_Of_Fuel_Past_Its_Allowance_Is_Refused_As_A_Fuel_Exhaustion()
    {
        var bound = UbcCorpusFamily.MaxUnchargedWork;
        var hook = new ChargingHook(new UbcCorpusHook(), VmBudgetDimension.Fuel, bound);
        var meter = new RecordingMeter(fuelAllowance: bound - 1, pollBound: bound);

        var outcome = Verify(Profile(hook, bound), UbcCorpus.SmallestWithFamily().Bytes(), meter);

        Assert.Equal(VmOutcome.ResourceExhaustion, outcome.Category);
        Assert.Equal(VmBudgetDimension.Fuel, outcome.ExhaustedDimension);
        Assert.False(meter.PollBoundExceeded);
    }

    // ---- passes and searches paid before they run (findings 1, 2 and 4) --------------------------

    [Fact]
    public void The_Hook_Pass_Is_Paid_For_Before_The_Hook_Sees_An_Instruction()
    {
        // A suspending row, so the landing check's charge - instructions and landings - differs from the
        // hook pass's, one unit per instruction; and a constant load the hook refuses, with no pool.
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], [UbcSlotType.V]);
        var unit = spec.AddUnit(type, 1, 0, 2, UbcUnitFlags.Entry | UbcUnitFlags.Suspendable, b =>
        {
            b.F(Op.PushSmall, 1).F(Op.Await);
            var resume = b.Offset;
            b.F(Op.LoadConstant, 0).Emit(O.Drop).Emit(O.Return);
            return [resume];
        });
        spec.AddEntry("main", unit);

        var hook = new CountingHook(new UbcCorpusHook());
        var (unbounded, oneShort, refused) = OneShort(Profile(hook, Unsplit), spec.Bytes(), () => hook.Instructions = 0);

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal(UbcCorpusFamily.ConstantPastThePool, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);
        Assert.Equal(VmBudgetDimension.VerifierWork, oneShort.ExhaustedDimension);
        Assert.Equal(0, hook.Instructions);
        Assert.Equal(5UL, refused);
    }

    [Theory]
    [InlineData("jump")]
    [InlineData("jump_if_zero")]
    [InlineData("family branch")]
    public void A_Branch_Target_Search_Is_Paid_For_At_The_Unit_Depth_Before_It_Runs(string form)
    {
        const int Nops = 40;
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], []);
        var instructions = 0;
        var unit = spec.AddUnit(type, 1, 1, 1, UbcUnitFlags.Entry, b =>
        {
            // Offset one lies inside the const.i32 at offset zero, so no instruction starts there.
            b.Emit(O.ConstI32, 7).Emit(O.Drop);

            for (var index = 0; index < Nops; index++)
            {
                b.Emit(O.Nop);
            }

            switch (form)
            {
                case "jump":
                    b.Emit(O.Jump, 1);
                    instructions = 1;
                    break;

                case "jump_if_zero":
                    b.Emit(O.ConstI32, 0).Emit(O.JumpIfZero, 1);
                    instructions = 2;
                    break;

                default:
                    b.F(Op.PushSmall, 1).F(Op.IfTrue, 1);
                    instructions = 2;
                    break;
            }

            b.Emit(O.Return);
            return [];
        });
        spec.AddEntry("main", unit);
        instructions += 2 + Nops + 1;

        var (unbounded, oneShort, refused) = OneShort(UbcCorpusFamily.Descriptor(maxUnchargedWork: Unsplit), spec.Bytes());

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal((int)UbcDiagnosticCode.NotAnInstructionBoundary, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);
        Assert.Equal(Depth(instructions), refused);
    }

    [Fact]
    public void A_Region_Search_Is_Paid_For_At_The_Unit_Depth_Before_It_Runs()
    {
        const int Nops = 40;
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], []);
        uint end = 0;
        var unit = spec.AddUnit(type, 1, 1, 1, UbcUnitFlags.Entry, b =>
        {
            b.Emit(O.ConstI32, 7).Emit(O.Drop);
            end = b.Offset;

            for (var index = 0; index < Nops; index++)
            {
                b.Emit(O.Nop);
            }

            b.Emit(O.Return);
            return [];
        });
        spec.AddEntry("main", unit);

        // The handler lies inside the const.i32 at offset zero.
        spec.Regions = [new UbcRegion(0, 0, end, 1, 0, 0, UbcCorpusFamily.CatchKind)];
        var instructions = 2 + Nops + 1;

        var (unbounded, oneShort, refused) = OneShort(UbcCorpusFamily.Descriptor(maxUnchargedWork: Unsplit), spec.Bytes());

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal((int)UbcDiagnosticCode.NotAnInstructionBoundary, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);

        // The charge refused is the one the searches are paid by: with the sort of the regions and the
        // pass over the instructions, it holds the region's three searches - start, end and handler -
        // each at the depth of the instructions, not of the one region.
        var searches = 3 * Depth(instructions);
        Assert.True(refused >= searches + (ulong)instructions, $"the charge refused, {refused}, does not pay for three searches of depth {Depth(instructions)} and a pass over {instructions} instructions");
    }

    [Fact]
    public void A_Position_Is_Charged_Its_Search_Alone_With_No_Unpaid_Pass_Over_The_Units()
    {
        const int Nops = 40;
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([], []);
        var main = spec.AddUnit(type, 0, 1, 0, UbcUnitFlags.Entry, b =>
        {
            b.Emit(O.ConstI32, 7).Emit(O.Drop);

            for (var index = 0; index < Nops; index++)
            {
                b.Emit(O.Nop);
            }

            b.Emit(O.Return);
            return [];
        });

        for (var other = 0; other < 3; other++)
        {
            spec.AddUnit(type, 0, 0, 0, UbcUnitFlags.None, b =>
            {
                b.Emit(O.Return);
                return [];
            });
        }

        spec.AddEntry("main", main);
        spec.Positions = [new UbcPosition(0, 1, 1, 1)];

        var (unbounded, oneShort, refused) = OneShort(UbcCorpusFamily.Descriptor(maxUnchargedWork: Unsplit), spec.Bytes());

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal((int)UbcDiagnosticCode.NotAnInstructionBoundary, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);

        // One search over the longest unit's boundaries; the longest unit is known from the unit walks.
        Assert.Equal(Depth(2 + Nops + 1), refused);
    }

    [Fact]
    public void A_Local_Run_Search_Is_Paid_For_At_The_Depth_Of_The_Runs_Before_It_Runs()
    {
        const int Runs = 40;
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([], []);
        var runs = Enumerable.Range(0, Runs).Select(static index => new UbcLocalRun(1, UbcSlotType.I64)).ToArray();
        var unit = spec.AddUnit(type, 0, 1, 0, UbcUnitFlags.Entry, b =>
        {
            // One past the last local.
            b.Emit(O.LocalGet, Runs).Emit(O.Drop).Emit(O.Return);
            return [];
        }, runs);
        spec.AddEntry("main", unit);

        var (unbounded, oneShort, refused) = OneShort(UbcCorpusFamily.Descriptor(maxUnchargedWork: Unsplit), spec.Bytes());

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal((int)UbcDiagnosticCode.LocalIndexOutOfRange, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);
        Assert.Equal(Depth(Runs), refused);
    }

    [Fact]
    public void A_Required_Landing_Is_Searched_For_At_The_Depth_Of_The_Landings_Before_The_Search_Runs()
    {
        // Three resume points and a landing list that omits the last of them.
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], [UbcSlotType.V]);
        var unit = spec.AddUnit(type, 1, 0, 1, UbcUnitFlags.Entry | UbcUnitFlags.Suspendable, b =>
        {
            var landings = new List<uint>();
            b.F(Op.PushSmall, 1);

            for (var index = 0; index < 3; index++)
            {
                b.F(Op.Await);
                landings.Add(b.Offset);
            }

            b.Emit(O.Return);
            return landings.Take(2);
        });
        spec.AddEntry("main", unit);

        var (unbounded, oneShort, refused) = OneShort(UbcCorpusFamily.Descriptor(maxUnchargedWork: Unsplit), spec.Bytes());

        Assert.Equal(VmOutcome.InvalidArtifact, unbounded.Category);
        Assert.Equal((int)UbcDiagnosticCode.LandingsInvalid, unbounded.ProfileDiagnosticCode);
        Assert.Equal(VmOutcome.ResourceExhaustion, oneShort.Category);
        Assert.Equal(Depth(2), refused);
    }

    // ---- what the walk keeps per jump table (finding 3) ---------------------------------------------

    [Fact]
    public void What_The_Walk_Allocates_Per_Jump_Table_Stays_Within_What_It_Reserves_Per_Jump_Table()
    {
        // The walk's share is the whole verification less the read of the same bytes, so the reader's own
        // estimates do not enter it; the per-table figure is the difference between two table counts, so
        // nothing constant does either.
        const int Fewer = 2048;
        const int More = 8192;
        var profile = UbcCorpusFamily.Descriptor();

        Walk(profile, JumpTables(Fewer));
        Walk(profile, JumpTables(More));

        var fewer = Walk(profile, JumpTables(Fewer));
        var more = Walk(profile, JumpTables(More));

        var allocated = (double)(more.Allocated - fewer.Allocated) / (More - Fewer);
        var reserved = (double)((long)more.Reserved - (long)fewer.Reserved) / (More - Fewer);

        Assert.True(allocated <= reserved, $"the walk allocates {allocated:F2} bytes per jump table and reserves {reserved:F2}");
    }

    // ---- helpers --------------------------------------------------------------------------------------

    /// <summary>The probes a binary search over <paramref name="length"/> sorted items makes at most, as the walk charges it.</summary>
    private static ulong Depth(int length) => (ulong)BitOperations.Log2((uint)length + 1) + 1;

    private static VmProfileDescriptor Profile(IUbcFamilyVerifier hook, uint maxUnchargedWork) =>
        UbcDescriptors.Build(
            new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, UbcCorpusFamily.Tables(), hook, UbcContract.Version),
            UbcCorpusFamily.Declaration(maxUnchargedWork: maxUnchargedWork),
            UbcCorpusFamily.Forms());

    private static VmVerifierOutcome Verify(VmProfileDescriptor profile, byte[] payload, IVmMeter meter, string manifest = UbcCorpusFamily.BaseManifestText)
    {
        var descriptor = new VmArtifactDescriptor(
            UbcCorpusFamily.ProfileId,
            UbcFormat.FormatVersion,
            VmFeatureManifestId.Parse(manifest),
            default,
            VmCallerIdentity.FromCanonicalIdentity("corpus://charging"));

        return profile.Verifier.Verify(in descriptor, payload, new Context(meter), CancellationToken.None);
    }

    /// <summary>
    /// Verifies with no allowance, then with one unit of verifier work less than that spent; answers both
    /// outcomes and the first charge the short allowance refused.
    /// </summary>
    private static (VmVerifierOutcome Unbounded, VmVerifierOutcome OneShort, ulong Refused) OneShort(
        VmProfileDescriptor profile,
        byte[] payload,
        Action? between = null)
    {
        var unboundedMeter = new RecordingMeter();
        var unbounded = Verify(profile, payload, unboundedMeter);
        between?.Invoke();

        var shortMeter = new RecordingMeter(workAllowance: unboundedMeter.Work - 1);
        var oneShort = Verify(profile, payload, shortMeter);

        return (unbounded, oneShort, shortMeter.FirstRefusedWork ?? 0);
    }

    /// <summary>One unit - const.i32, jump_table 0, return - and <paramref name="count"/> tables of one target each.</summary>
    private static byte[] JumpTables(int count)
    {
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([], []);
        uint target = 0;
        var unit = spec.AddUnit(type, 0, 1, 0, UbcUnitFlags.Entry, b =>
        {
            b.Emit(O.ConstI32, 0).Emit(O.JumpTable, 0);
            target = b.Offset;
            b.Emit(O.Return);
            return [];
        });
        spec.AddEntry("main", unit);
        spec.JumpTables = Enumerable.Range(0, count).Select(_ => new UbcJumpTable(0, [target])).ToList();
        return spec.Bytes();
    }

    /// <summary>The bytes the walk allocated and reserved for <paramref name="payload"/>: the verification's less the read's.</summary>
    private static (long Allocated, ulong Reserved) Walk(VmProfileDescriptor profile, byte[] payload)
    {
        var meter = new RecordingMeter();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var outcome = Verify(profile, payload, meter);
        var verification = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(VmOutcome.Normal, outcome.Category);

        var read = new ReadMeter();
        var bounds = new VmReadBounds(1UL << 40, uint.MaxValue, uint.MaxValue, uint.MaxValue);
        before = GC.GetAllocatedBytesForCurrentThread();
        var readable = UbcArtifactReader.TryRead(payload, in bounds, read, Unsplit, UbcFormat.FormatVersion, out var artifact, out _);
        var reading = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(readable);
        GC.KeepAlive(artifact);
        GC.KeepAlive(outcome.State);
        return (verification - reading, meter.Reserved - read.Reserved);
    }

    /// <summary>
    /// A meter that enforces a verifier-work and a fuel allowance and nothing else, counts both toward
    /// the poll bound as the core's meter does, refuses a poll once more than the bound was charged
    /// since the last, and remembers the first verifier-work charge it refused.
    /// </summary>
    private sealed class RecordingMeter(ulong workAllowance = ulong.MaxValue, ulong fuelAllowance = ulong.MaxValue, ulong pollBound = 0) : IVmMeter
    {
        private ulong sinceLastPoll;

        internal ulong Work { get; private set; }

        internal ulong Fuel { get; private set; }

        internal ulong Reserved { get; private set; }

        internal ulong? FirstRefusedWork { get; private set; }

        internal ulong MostWorkBetweenPolls { get; private set; }

        internal bool PollBoundExceeded { get; private set; }

        public bool TryCharge(VmBudgetDimension dimension, ulong amount)
        {
            switch (dimension)
            {
                case VmBudgetDimension.VerifierWork:
                    if (amount > workAllowance - Work)
                    {
                        FirstRefusedWork ??= amount;
                        return false;
                    }

                    Work += amount;
                    break;

                case VmBudgetDimension.Fuel:
                    if (amount > fuelAllowance - Fuel)
                    {
                        return false;
                    }

                    Fuel += amount;
                    break;

                case VmBudgetDimension.AllocatedBytes:
                    Reserved += amount;
                    return true;

                default:
                    return true;
            }

            sinceLastPoll += amount;
            MostWorkBetweenPolls = Math.Max(MostWorkBetweenPolls, sinceLastPoll);
            return true;
        }

        public bool Poll()
        {
            if (PollBoundExceeded || (pollBound > 0 && sinceLastPoll > pollBound))
            {
                PollBoundExceeded = true;
                return false;
            }

            sinceLastPoll = 0;
            return true;
        }

        public void ReportRetained(VmBudgetDimension dimension, ulong amount)
        {
        }

        public void ReportReleased(VmBudgetDimension dimension, ulong amount)
        {
        }
    }

    /// <summary>The reader's meter for a read measured alone: it admits everything and adds up what was reserved.</summary>
    private sealed class ReadMeter : IVmBoundedAllocationMeter
    {
        internal ulong Reserved { get; private set; }

        public bool TryReserve(ulong byteCount)
        {
            Reserved += byteCount;
            return true;
        }

        public void Release(ulong byteCount)
        {
        }

        public bool TryChargeWork(ulong workUnits) => true;

        public bool Poll() => true;
    }

    private sealed class Context(IVmMeter meter) : IVmVerificationContext
    {
        public VmEffectiveCeilings Ceilings { get; } = new(OpenCeilings, OpenCeilings);

        public IVmMeter Meter => meter;

        public ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities => ImmutableArray<VmHostCapabilityDescriptor>.Empty;

        public bool TryGetCapabilityDescriptor(VmCapabilityId capabilityId, int version, out VmHostCapabilityDescriptor descriptor)
        {
            descriptor = default;
            return false;
        }
    }

    /// <summary>The corpus hook, charging a fixed amount of one dimension through the meter it is handed at Begin and at every instruction.</summary>
    private sealed class ChargingHook(IUbcFamilyVerifier inner, VmBudgetDimension dimension, ulong amount) : IUbcFamilyVerifier
    {
        private IVmMeter? meter;

        public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
        {
            familyState = null;
            meter = artifact.Meter;
            return meter.TryCharge(dimension, amount) ? inner.Begin(artifact, out familyState) : UbcHookAnswer.Exhaust(dimension);
        }

        public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction) =>
            meter!.TryCharge(dimension, amount) ? inner.CheckInstruction(familyState, in instruction) : UbcHookAnswer.Exhaust(dimension);

        public UbcHookAnswer End(object? familyState) => inner.End(familyState);
    }

    /// <summary>The corpus hook, counting the instructions it is shown.</summary>
    private sealed class CountingHook(IUbcFamilyVerifier inner) : IUbcFamilyVerifier
    {
        internal int Instructions { get; set; }

        public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState) => inner.Begin(artifact, out familyState);

        public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction)
        {
            Instructions++;
            return inner.CheckInstruction(familyState, in instruction);
        }

        public UbcHookAnswer End(object? familyState) => inner.End(familyState);
    }
}
