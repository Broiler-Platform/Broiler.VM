using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Com.Example.Tally;

/// <summary>One program of the fixed list: its name, its artifact, the entry to invoke, and what a run is expected to answer.</summary>
public sealed class TallyProgram
{
    internal TallyProgram(string name, byte[] artifact, string entry, string expected, string note)
    {
        Name = name;
        Artifact = ImmutableArray.Create(artifact);
        Entry = entry;
        Expected = expected;
        Note = note;
    }

    /// <summary>The program's name.</summary>
    public string Name { get; }

    /// <summary>The universal bytecode artifact the lowering wrote.</summary>
    public ImmutableArray<byte> Artifact { get; }

    /// <summary>The entry a run invokes.</summary>
    public string Entry { get; }

    /// <summary>
    /// The expected transcript: one line per step - a completion's payload, a fault's, a suspension's
    /// projection and then the resumption's answer, or the verification refusal.
    /// </summary>
    public string Expected { get; }

    /// <summary>Why the program is in the list.</summary>
    public string Note { get; }
}

/// <summary>
/// The fixture family's lowering: a fixed list of programs written straight into universal bytecode
/// through <see cref="UbcArtifactWriter"/>, each with the transcript a run of it must produce.
/// </summary>
/// <remarks>
/// It is a lowering in the only sense a family that is not a language can have one: each program is a
/// sequence of instructions stated here, and the lowering is their encoding. What the list must reach
/// is the roadmap's UBC-2.4: a recursion that exhausts <c>CallDepth</c>, a suspension and resumption, a
/// region that lands, a trap, a guest load a provider answers, every common row, and the two
/// fuel-parity twins.
/// </remarks>
public static class TallyPrograms
{
    /// <summary>The iterations of the fuel-parity twins' loops.</summary>
    public const int TwinIterations = 12;

    private const byte S = TallyProfile.Slot;

    /// <summary>The name a guest load of <paramref name="operand"/> asks the provider for.</summary>
    public static string GuestName(int operand) => "guest-" + operand.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Every program, in list order.</summary>
    public static ImmutableArray<TallyProgram> All { get; } = Build();

    /// <summary>The guest program a provider answers <see cref="GuestName"/>(1) with.</summary>
    public static ImmutableArray<byte> Guest { get; } = ImmutableArray.Create(BuildGuest());

    /// <summary>The program named <paramref name="name"/>.</summary>
    public static TallyProgram Named(string name)
    {
        foreach (var program in All)
        {
            if (string.Equals(program.Name, name, System.StringComparison.Ordinal))
            {
                return program;
            }
        }

        throw new System.ArgumentException($"no program is named '{name}'", nameof(name));
    }

    private static ImmutableArray<TallyProgram> Build()
    {
        var list = ImmutableArray.CreateBuilder<TallyProgram>();

        void Add(string name, byte[] artifact, string expected, string note, string entry = "main") =>
            list.Add(new TallyProgram(name, artifact, entry, expected, note));

        Add("sum", Single(b => b
                .EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 2)
                .EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 3)
                .EmitFamily(S, TallyTable.Add, UbcOperandShape.None)
                .EmitFamily(S, TallyTable.Count, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "completed words=[5] tallies=[]",
            "a dynamic row that reads and writes the value plane, and one that moves a value to a word");

        Add("arithmetic", Single(b => b
                .Emit(UbcOpcode.ConstI64, 6)
                .Emit(UbcOpcode.ConstI64, 7)
                .EmitFamily(S, TallyTable.MulWords, UbcOperandShape.None)
                .Emit(UbcOpcode.ConstI64, unchecked((ulong)-2L))
                .EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "completed words=[40] tallies=[]",
            "primitive rows over i64 words, executed by the emitter from the primitive table");

        Add("divide-by-zero", Single(b => b
                .Emit(UbcOpcode.ConstI64, 1)
                .Emit(UbcOpcode.ConstI64, 0)
                .EmitFamily(S, TallyTable.DivWords, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "faulted Trap code=1 amount=0",
            "a primitive's trap, answered with the family's code for it");

        Add("overflow", Single(b => b
                .Emit(UbcOpcode.ConstI64, unchecked((ulong)long.MinValue))
                .Emit(UbcOpcode.ConstI64, unchecked((ulong)-1L))
                .EmitFamily(S, TallyTable.DivWords, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "faulted Trap code=2 amount=0",
            "the one signed division whose quotient does not fit");

        Add("unreachable", Single(b => b.Emit(UbcOpcode.Trap, 0)),
            "faulted Unreachable code=0 amount=0",
            "the universal unreachable, trap 0 0");

        Add("explicit-trap", Single(b => b.Emit(UbcOpcode.Trap, S | ((ulong)TallyTable.TrapExplicit << 8))),
            "faulted Trap code=3 amount=0",
            "a trap instruction naming the family's slot and a code of its vocabulary");

        Add("catch", Catch(), "completed words=[5] tallies=[]",
            "a throw a region of the same unit covers: the planes truncated to the region's entry heights, the landing pushing the thrown tally");

        Add("uncaught", Single(b => b
                .EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 7)
                .EmitFamily(S, TallyTable.Throw, UbcOperandShape.None)),
            "faulted Uncaught code=0 amount=7",
            "a throw no region covers, answered with the family's uncaught payload");

        Add("throw-across-frames", ThrowAcrossFrames(), "completed words=[9] tallies=[]",
            "a throw in a callee, caught by the caller's region covering the call: the callee's frame popped and its depth released");

        Add("recursion", Recursion(), "exhausted CallDepth",
            "a guest recursion with no bound, refused by the meter naming CallDepth and never by the process's stack");

        Add("suspend", Suspend(), "suspended amount=1\ncompleted words=[121] tallies=[]",
            "a suspension and a resumption: both planes and every local restored through the family's frame codec");

        Add("suspend-in-callee", SuspendInCallee(), "suspended amount=5\ncompleted words=[26] tallies=[]",
            "a suspension two frames deep whose caller, once the callee returns, climbs higher than the callee ever did: the resumption restores room for every frame and not only the top one");

        Add("call-request", CallRequest(), "completed words=[14] tallies=[]",
            "a call row whose handler answers a request the emitter performs as a frame push");

        Add("branch", Branch(3), "completed words=[1] tallies=[]",
            "a branch row whose handler answers taken");

        Add("branch-not-taken", Branch(-3), "completed words=[0] tallies=[]",
            "a branch row whose handler answers next");

        Add("guest-load", GuestLoad(1), "completed words=[105] tallies=[]",
            "a guest load the composition's provider answers: verified by the same descriptor, entered by a call request in the same emitter");

        Add("guest-foreign", GuestLoad(2), "faulted Trap code=4 amount=0 load=ProviderProfileMismatch",
            "a guest load the provider answers with another profile's artifact: the core refuses it as a breach and the family traps");

        Add("common-tour", CommonTour(), "completed words=[84] tallies=[]",
            "every row of the common family, each executed with Appendix A's meaning over the word plane");

        Add("value-tour", ValueTour(), "completed words=[24] tallies=[]",
            "the shuffles, the select and the locals over the value plane, and a swap across the planes");

        Add("twin-inline", Twin(inline: true), $"completed words=[{TwinIterations}] tallies=[]",
            "a fuel-parity twin: a counting loop whose accumulator is a word and whose addition is a primitive row");

        Add("twin-dynamic", Twin(inline: false), $"completed words=[{TwinIterations}] tallies=[]",
            "its twin: the same instruction count, the accumulator a tally and its addition a dynamic row");

        Add("countdown", Countdown(20), "completed words=[20] tallies=[]",
            "a bounded recursion twenty frames deep: under the family's own call depth it completes, and the hostile-neighbour check runs it where a neighbour's defaults were adopted");

        Add("bad-request", Single(b => b
                .EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 1)
                .EmitFamily(S, TallyTable.CallUnit, UbcOperandShape.U32, 0)
                .EmitFamily(S, TallyTable.Count, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "ProfileFault ProfileContractViolation",
            "a call request naming a unit whose signature is not the row's: the emitter refuses it as the family's contract violation, never as the guest's fault");

        Add("spin", Spin(), "exhausted Fuel",
            "a loop with no exit, stopped by the fuel allowance; the cancellation check runs it with a cancelled token instead");

        Add("long-code", Single(b =>
            {
                for (var index = 0; index < 300; index++)
                {
                    b.Emit(UbcOpcode.Nop);
                }

                b.Emit(UbcOpcode.ConstI64, 7).Emit(UbcOpcode.Return);
            }),
            "completed words=[7] tallies=[]",
            "a code section longer than the family's uncharged-work bound: the reader reads it in pieces the bound admits, polling between them");

        Add("long-entry", LongEntry(), "completed words=[7] tallies=[]",
            "an entry name longer than the family's uncharged-work bound, read and matched the same way",
            entry: LongEntryName);

        Add("resume-long", ResumeLong(), "suspended amount=1" + NewLine + "completed words=[1] tallies=[]",
            "work close to the poll bound before a suspension and past it after: the resumed step's count starts from a poll, so the two steps' work is never added unseen");

        Add("load-then-run", LoadThenRun(), "completed words=[105] tallies=[]",
            "a guest load whose nested verification charges the same operation's meter, followed by more work than one poll interval: the verification leaves nothing unpolled behind it");

        Add("hook-refusal", Single(b => b
                .EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 2_000_000)
                .EmitFamily(S, TallyTable.Count, UbcOperandShape.None)
                .Emit(UbcOpcode.Return)),
            "refused InvalidArtifact SemanticValidationFailed 7001",
            "the family hook's one refusal: a tally.const operand outside the declared range");

        Add("no-such-entry", Single(b => b.Emit(UbcOpcode.ConstI64, 1).Emit(UbcOpcode.Return)),
            "faulted NoSuchEntry code=0 amount=0",
            "an invocation naming an entry the program does not have",
            entry: "absent");

        return list.ToImmutable();
    }

    // ---- the programs ----------------------------------------------------------------------------

    private static byte[] Single(System.Action<UbcCodeBuilder> body)
    {
        var assembly = new TallyAssembly();
        var unit = assembly.Begin();
        body(unit);
        assembly.End(unit, assembly.Type([], [UbcSlotType.I64]), [], 8, 8, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private const string NewLine = "\n";

    /// <summary>The entry name of the long-entry program: longer than the family's uncharged-work bound.</summary>
    public static string LongEntryName => new('e', 300);

    /// <summary>A counting loop over word local <paramref name="local"/>, <paramref name="iterations"/> times round.</summary>
    private static void Loop(UbcCodeBuilder b, int iterations, ushort local)
    {
        var top = b.NewLabel();
        var done = b.NewLabel();
        b.Emit(UbcOpcode.ConstI64, (ulong)iterations);
        b.Emit(UbcOpcode.LocalSet, local);
        b.Mark(top);
        b.Emit(UbcOpcode.LocalGet, local);
        b.EmitFamily(S, TallyTable.IsZero, UbcOperandShape.None);
        b.EmitTo(UbcOpcode.JumpIfNonZero, done);
        b.Emit(UbcOpcode.LocalGet, local);
        b.Emit(UbcOpcode.ConstI64, unchecked((ulong)-1L));
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        b.Emit(UbcOpcode.LocalSet, local);
        b.EmitTo(UbcOpcode.Jump, top);
        b.Mark(done);
    }

    private static byte[] LongEntry()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.Emit(UbcOpcode.ConstI64, 7);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 2, 0, UbcUnitFlags.Entry);
        assembly.Entry(LongEntryName, 0);
        return assembly.Write();
    }

    private static byte[] ResumeLong()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        Loop(b, 30, 0);
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 1);
        b.EmitFamily(S, TallyTable.Yield, UbcOperandShape.None);
        var resume = b.Offset;
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        Loop(b, 40, 0);
        b.Emit(UbcOpcode.Return);
        assembly.End(
            b,
            assembly.Type([], [UbcSlotType.I64]),
            [new UbcLocalRun(1, UbcSlotType.I64)],
            4,
            1,
            UbcUnitFlags.Entry | UbcUnitFlags.Suspendable,
            [resume]);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] LoadThenRun()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 5);
        b.EmitFamily(S, TallyTable.Load, UbcOperandShape.U16, 1);
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        Loop(b, 40, 0);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [new UbcLocalRun(1, UbcSlotType.I64)], 4, 1, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Countdown(int depth)
    {
        var assembly = new TallyAssembly();
        var main = assembly.Begin();
        main.Emit(UbcOpcode.ConstI64, (ulong)depth);
        main.Emit(UbcOpcode.Call, 1);
        main.Emit(UbcOpcode.Return);
        assembly.End(main, assembly.Type([], [UbcSlotType.I64]), [], 2, 0, UbcUnitFlags.Entry);

        var f = assembly.Begin();
        var bottom = f.NewLabel();
        f.Emit(UbcOpcode.LocalGet, 0);
        f.EmitFamily(S, TallyTable.IsZero, UbcOperandShape.None);
        f.EmitTo(UbcOpcode.JumpIfNonZero, bottom);
        f.Emit(UbcOpcode.LocalGet, 0);
        f.Emit(UbcOpcode.ConstI64, unchecked((ulong)-1L));
        f.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        f.Emit(UbcOpcode.Call, 1);
        f.Emit(UbcOpcode.ConstI64, 1);
        f.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        f.Emit(UbcOpcode.Return);
        f.Mark(bottom);
        f.Emit(UbcOpcode.ConstI64, 0);
        f.Emit(UbcOpcode.Return);
        assembly.End(f, assembly.Type([UbcSlotType.I64], [UbcSlotType.I64]), [], 2, 0, UbcUnitFlags.None);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Spin()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var loop = b.NewLabel();
        b.Mark(loop);
        b.Emit(UbcOpcode.Nop);
        b.EmitTo(UbcOpcode.Jump, loop);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 0, 0, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Catch()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var start = b.Offset;
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 5);
        b.EmitFamily(S, TallyTable.Throw, UbcOperandShape.None);
        var handler = b.Offset;
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 4, 4, UbcUnitFlags.Entry, [handler]);
        assembly.Region(0, start, handler, handler, 0, 0, TallyTable.Catch);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] ThrowAcrossFrames()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var start = b.Offset;
        b.Emit(UbcOpcode.Call, 1);
        var end = b.Offset;
        b.Emit(UbcOpcode.ConstI64, 0);
        b.Emit(UbcOpcode.Return);
        var handler = b.Offset;
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 4, 4, UbcUnitFlags.Entry, [handler]);
        assembly.Region(0, start, end, handler, 0, 0, TallyTable.Catch);

        var thrower = assembly.Begin();
        thrower.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 9);
        thrower.EmitFamily(S, TallyTable.Throw, UbcOperandShape.None);
        assembly.End(thrower, assembly.Type([], []), [], 2, 2, UbcUnitFlags.None);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Recursion()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.Emit(UbcOpcode.Call, 0);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], []), [], 2, 2, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Suspend()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.Emit(UbcOpcode.ConstI64, 100);
        b.Emit(UbcOpcode.LocalSet, 0);
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 20);
        b.Emit(UbcOpcode.LocalSet, 1);
        b.Emit(UbcOpcode.ConstI64, 7);
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 1);
        b.EmitFamily(S, TallyTable.Yield, UbcOperandShape.None);
        var resume = b.Offset;
        b.Emit(UbcOpcode.LocalGet, 1);
        b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        b.Emit(UbcOpcode.LocalGet, 0);
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        b.Emit(UbcOpcode.Swap);
        b.Emit(UbcOpcode.Drop);
        b.Emit(UbcOpcode.Return);
        assembly.End(
            b,
            assembly.Type([], [UbcSlotType.I64]),
            [new UbcLocalRun(1, UbcSlotType.I64), new UbcLocalRun(1, UbcSlotType.V)],
            4,
            4,
            UbcUnitFlags.Entry | UbcUnitFlags.Suspendable,
            [resume]);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] SuspendInCallee()
    {
        var assembly = new TallyAssembly();
        var main = assembly.Begin();
        main.Emit(UbcOpcode.ConstI64, 1);
        main.Emit(UbcOpcode.Call, 1);

        for (var value = 2UL; value <= 6; value++)
        {
            main.Emit(UbcOpcode.ConstI64, value);
        }

        for (var add = 0; add < 6; add++)
        {
            main.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        }

        main.Emit(UbcOpcode.Return);
        assembly.End(main, assembly.Type([], [UbcSlotType.I64]), [], 8, 0, UbcUnitFlags.Entry);

        var callee = assembly.Begin();
        callee.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 5);
        callee.EmitFamily(S, TallyTable.Yield, UbcOperandShape.None);
        var resume = callee.Offset;
        callee.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        callee.Emit(UbcOpcode.Return);
        assembly.End(callee, assembly.Type([], [UbcSlotType.I64]), [], 1, 1, UbcUnitFlags.Suspendable, [resume]);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] CallRequest()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 4);
        b.EmitFamily(S, TallyTable.CallUnit, UbcOperandShape.U32, 1);
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 4, 4, UbcUnitFlags.Entry);

        var callee = assembly.Begin();
        callee.Emit(UbcOpcode.LocalGet, 0);
        callee.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 10);
        callee.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);
        callee.Emit(UbcOpcode.Return);
        assembly.End(callee, assembly.Type([UbcSlotType.V], [UbcSlotType.V]), [], 2, 4, UbcUnitFlags.None);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Branch(int amount)
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var taken = b.NewLabel();
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, unchecked((uint)amount));
        b.EmitFamilyTo(S, TallyTable.IfPositive, taken);
        b.Emit(UbcOpcode.ConstI64, 0);
        b.Emit(UbcOpcode.Return);
        b.Mark(taken);
        b.Emit(UbcOpcode.ConstI64, 1);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 4, 4, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] GuestLoad(int name)
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 5);
        b.EmitFamily(S, TallyTable.Load, UbcOperandShape.U16, (ulong)name);
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [], 4, 4, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] BuildGuest()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.Emit(UbcOpcode.LocalGet, 0);
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 100);
        b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([UbcSlotType.V], [UbcSlotType.V]), [], 2, 4, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] CommonTour()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var zero = b.NewLabel();
        var one = b.NewLabel();
        var two = b.NewLabel();
        var bad = b.NewLabel();
        var end = b.NewLabel();

        b.Emit(UbcOpcode.Nop);
        b.Emit(UbcOpcode.ConstI64, 10);                   // [10]
        b.Emit(UbcOpcode.Dup);                            // [10 10]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [20]
        b.Emit(UbcOpcode.ConstI64, 3);                    // [20 3]
        b.Emit(UbcOpcode.Swap);                           // [3 20]
        b.Emit(UbcOpcode.ConstI64, 2);                    // [3 20 2]
        b.EmitFamily(S, TallyTable.DivWords, UbcOperandShape.None); // [3 10]
        b.EmitFamily(S, TallyTable.MulWords, UbcOperandShape.None); // [30]
        b.Emit(UbcOpcode.LocalSet, 0);                    // [] l0=30
        b.Emit(UbcOpcode.ConstI64, 5);                    // [5]
        b.Emit(UbcOpcode.LocalTee, 1);                    // [5] l1=5
        b.Emit(UbcOpcode.ConstI64, 7);                    // [5 7]
        b.Emit(UbcOpcode.Dup2);                           // [5 7 5 7]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [5 7 12]
        b.Emit(UbcOpcode.Pick, 2);                        // [5 7 12 5]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [5 7 17]
        b.Emit(UbcOpcode.Squash, 2 | (1UL << 8));         // [17]
        b.Emit(UbcOpcode.LocalGet, 0);                    // [17 30]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [47]
        b.Emit(UbcOpcode.ConstF32, System.BitConverter.SingleToUInt32Bits(1.5f)); // [47 f32]
        b.Emit(UbcOpcode.Drop);                           // [47]
        b.Emit(UbcOpcode.ConstF64, System.BitConverter.DoubleToUInt64Bits(2.5)); // [47 f64]
        b.Emit(UbcOpcode.LocalSet, 2);                    // [47]
        b.Emit(UbcOpcode.ConstI64, 30);                   // [47 30]
        b.Emit(UbcOpcode.ConstI64, 99);                   // [47 30 99]
        b.Emit(UbcOpcode.ConstI32, 0);                    // [47 30 99 0]
        b.Emit(UbcOpcode.Select);                         // [47 99]  - zero keeps the shallower
        b.Emit(UbcOpcode.ConstI64, 64);                   // [47 99 64]
        b.Emit(UbcOpcode.ConstI32, 1);                    // [47 99 64 1]
        b.Emit(UbcOpcode.Select);                         // [47 99]  - non-zero keeps the deeper
        b.Emit(UbcOpcode.LocalGet, 1);                    // [47 99 5]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [47 104]
        b.Emit(UbcOpcode.ConstI64, unchecked((ulong)-71L));        // [47 104 -71]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [47 33]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [80]
        b.Emit(UbcOpcode.ConstI32, 7);                    // [80 7] - out of range: the last row
        b.Emit(UbcOpcode.JumpTable, 0);
        var zeroAt = b.Offset;
        b.Mark(zero);
        b.Emit(UbcOpcode.Trap, 0);
        var oneAt = b.Offset;
        b.Mark(one);
        b.Emit(UbcOpcode.Trap, S | ((ulong)TallyTable.TrapExplicit << 8));
        var twoAt = b.Offset;
        b.Mark(two);                                      // [80]
        b.Emit(UbcOpcode.ConstI32, 0);
        b.EmitTo(UbcOpcode.JumpIfNonZero, bad);           // not taken
        b.Emit(UbcOpcode.ConstI32, 1);
        b.EmitTo(UbcOpcode.JumpIfZero, bad);              // not taken
        b.EmitTo(UbcOpcode.Jump, end);
        b.Mark(bad);
        b.Emit(UbcOpcode.Trap, 0);
        b.Mark(end);
        b.Emit(UbcOpcode.Call, 1);                        // [84]
        b.Emit(UbcOpcode.Return);
        var main = assembly.End(
            b,
            assembly.Type([], [UbcSlotType.I64]),
            [new UbcLocalRun(2, UbcSlotType.I64), new UbcLocalRun(1, UbcSlotType.F64)],
            6,
            0,
            UbcUnitFlags.Entry);
        assembly.JumpTable(main, [zeroAt, oneAt, twoAt]);

        var callee = assembly.Begin();
        callee.Emit(UbcOpcode.LocalGet, 0);
        callee.Emit(UbcOpcode.ConstI64, 4);
        callee.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        callee.Emit(UbcOpcode.Return);
        assembly.End(callee, assembly.Type([UbcSlotType.I64], [UbcSlotType.I64]), [], 2, 0, UbcUnitFlags.None);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] ValueTour()
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 1);  // [v1]
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 2);  // [v1 v2]
        b.Emit(UbcOpcode.Swap);                                     // [v2 v1]
        b.Emit(UbcOpcode.Pick, 1);                                  // [v2 v1 v2]
        b.Emit(UbcOpcode.Dup);                                      // [v2 v1 v2 v2]
        b.Emit(UbcOpcode.Dup2);                                     // [v2 v1 v2 v2 v2 v2]
        b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);      // [v2 v1 v2 v2 v4]
        b.Emit(UbcOpcode.Squash, 2 | (1UL << 8));                   // [v2 v1 v4]
        b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);      // [v2 v5]
        b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);      // [v7]
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);    // [7]
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 3);  // [7 v3]
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 9);  // [7 v3 v9]
        b.Emit(UbcOpcode.ConstI32, 1);                              // [7 v3 v9 1]
        b.Emit(UbcOpcode.Select);                                   // [7 v3]
        b.Emit(UbcOpcode.LocalTee, 0);                              // [7 v3] l0=v3
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);    // [7 3]
        b.EmitFamily(S, TallyTable.MulWords, UbcOperandShape.None); // [21]
        b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 40); // [21 v40]
        b.Emit(UbcOpcode.Swap);                                     // [v40 21] - across the planes
        b.Emit(UbcOpcode.LocalGet, 0);                              // [v40 21 v3]
        b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);    // [v40 21 3]
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None); // [v40 24]
        b.Emit(UbcOpcode.Swap);                                     // [24 v40]
        b.Emit(UbcOpcode.Drop);                                     // [24]
        b.Emit(UbcOpcode.Return);
        assembly.End(b, assembly.Type([], [UbcSlotType.I64]), [new UbcLocalRun(1, UbcSlotType.V)], 4, 8, UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }

    private static byte[] Twin(bool inline)
    {
        var assembly = new TallyAssembly();
        var b = assembly.Begin();
        var loop = b.NewLabel();
        var done = b.NewLabel();

        if (inline)
        {
            b.Emit(UbcOpcode.ConstI64, 0);
            b.Emit(UbcOpcode.LocalSet, 0);
        }
        else
        {
            b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 0);
            b.Emit(UbcOpcode.LocalSet, 2);
        }

        b.Emit(UbcOpcode.ConstI64, TwinIterations);
        b.Emit(UbcOpcode.LocalSet, 1);
        b.Mark(loop);
        b.Emit(UbcOpcode.LocalGet, 1);
        b.EmitFamily(S, TallyTable.IsZero, UbcOperandShape.None);
        b.EmitTo(UbcOpcode.JumpIfNonZero, done);

        if (inline)
        {
            b.Emit(UbcOpcode.LocalGet, 0);
            b.Emit(UbcOpcode.ConstI64, 1);
            b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
            b.Emit(UbcOpcode.LocalSet, 0);
        }
        else
        {
            b.Emit(UbcOpcode.LocalGet, 2);
            b.EmitFamily(S, TallyTable.Const, UbcOperandShape.I32, 1);
            b.EmitFamily(S, TallyTable.Add, UbcOperandShape.None);
            b.Emit(UbcOpcode.LocalSet, 2);
        }

        b.Emit(UbcOpcode.LocalGet, 1);
        b.Emit(UbcOpcode.ConstI64, unchecked((ulong)-1L));
        b.EmitFamily(S, TallyTable.AddWords, UbcOperandShape.None);
        b.Emit(UbcOpcode.LocalSet, 1);
        b.EmitTo(UbcOpcode.Jump, loop);
        b.Mark(done);

        if (inline)
        {
            b.Emit(UbcOpcode.LocalGet, 0);
            b.Emit(UbcOpcode.Nop);
        }
        else
        {
            b.Emit(UbcOpcode.LocalGet, 2);
            b.EmitFamily(S, TallyTable.Count, UbcOperandShape.None);
        }

        b.Emit(UbcOpcode.Return);
        assembly.End(
            b,
            assembly.Type([], [UbcSlotType.I64]),
            [new UbcLocalRun(2, UbcSlotType.I64), new UbcLocalRun(1, UbcSlotType.V)],
            4,
            4,
            UbcUnitFlags.Entry);
        assembly.Entry("main", 0);
        return assembly.Write();
    }
}

/// <summary>The lowering's assembler: types, units tiling the Code section, regions, jump tables and entries.</summary>
internal sealed class TallyAssembly
{
    private readonly System.Collections.Generic.List<UbcSignature> types = new();
    private readonly System.Collections.Generic.List<UbcUnit> units = new();
    private readonly System.Collections.Generic.List<byte> code = new();
    private readonly System.Collections.Generic.List<UbcRegion> regions = new();
    private readonly System.Collections.Generic.List<UbcJumpTable> tables = new();
    private readonly System.Collections.Generic.List<UbcEntry> entries = new();

    internal UbcCodeBuilder Begin() => new((uint)code.Count);

    internal int Type(UbcSlotType[] parameters, UbcSlotType[] results)
    {
        for (var index = 0; index < types.Count; index++)
        {
            if (System.Linq.Enumerable.SequenceEqual(types[index].Parameters, parameters) &&
                System.Linq.Enumerable.SequenceEqual(types[index].Results, results))
            {
                return index;
            }
        }

        types.Add(new UbcSignature(ImmutableArray.Create(parameters), ImmutableArray.Create(results)));
        return types.Count - 1;
    }

    internal int End(
        UbcCodeBuilder builder,
        int type,
        UbcLocalRun[] locals,
        uint maxWords,
        uint maxValues,
        UbcUnitFlags flags,
        uint[]? landings = null)
    {
        var bytes = builder.ToArray();
        units.Add(new UbcUnit(
            (uint)type,
            TallyProfile.Slot,
            ImmutableArray.Create(locals),
            maxWords,
            maxValues,
            builder.BaseOffset,
            (uint)bytes.Length,
            flags,
            landings is null ? ImmutableArray<uint>.Empty : ImmutableArray.Create(landings)));
        code.AddRange(bytes);
        return units.Count - 1;
    }

    internal void Region(int unit, uint start, uint end, uint handler, uint words, uint values, byte kind) =>
        regions.Add(new UbcRegion((uint)unit, start, end, handler, words, values, kind));

    internal void JumpTable(int unit, uint[] targets) =>
        tables.Add(new UbcJumpTable((uint)unit, ImmutableArray.Create(targets)));

    internal void Entry(string name, int unit) =>
        entries.Add(new UbcEntry(ImmutableArray.Create(System.Text.Encoding.UTF8.GetBytes(name)), (uint)unit));

    internal byte[] Write()
    {
        var header = new UbcHeader(
            UbcFormat.FormatVersion,
            TallyTable.Identity,
            TallyTable.ManifestIdentity,
            UbcFormat.BytecodeForm,
            "com.example.tally.lowering",
            1);
        var writer = new UbcArtifactWriter(header)
            .Families([new UbcFamilyEntry(TallyProfile.Slot, TallyTable.Identity, TallyTable.Table.TableVersion, TallyTable.ManifestIdentity)])
            .Types(types)
            .Units(units)
            .Code(code.ToArray());

        if (tables.Count > 0)
        {
            writer.JumpTables(tables);
        }

        if (regions.Count > 0)
        {
            writer.Regions(regions);
        }

        return writer.Entries(entries).ToArray();
    }
}
