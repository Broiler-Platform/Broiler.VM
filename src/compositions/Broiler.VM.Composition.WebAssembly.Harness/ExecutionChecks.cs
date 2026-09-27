using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Real modules driven through the whole core lifecycle: catalog, verify, instantiate, invoke.
/// </summary>
/// <remarks>
/// <para>
/// <b>EVERY MODULE HERE IS ASSEMBLED FROM BYTES BY THE ENCODER BESIDE THIS FILE AND GOES THROUGH THE
/// CORE.</b> Nothing calls the interpreter directly, nothing reaches inside the profile, and no
/// check asserts on a type the profile does not publish. What a check reads is what an embedder
/// would read: an outcome, a reason, and a typed payload.
/// </para>
/// <para>
/// <b>What this is not.</b> It is not the specification's conformance suite, it is not scored per
/// assertion family, and it sets no ratchet. A corpus this component wrote cannot find a rejection
/// this component never thought of, and the milestone that pins the suite is the one that fixes
/// that.
/// </para>
/// </remarks>
internal static class ExecutionChecks
{
    /// <summary>Runs every execution check and prints what each one saw.</summary>
    internal static int Report(VmRuntime runtime, bool verbose)
    {
        var checks = new List<(string Name, bool Passed, string Detail)>();

        checks.AddRange(Numeric(runtime));
        checks.AddRange(Control(runtime));
        checks.AddRange(Memory(runtime));
        checks.AddRange(Calls(runtime));
        checks.AddRange(Traps(runtime));
        checks.AddRange(GlobalsAndStart(runtime));
        checks.AddRange(EntryPointEncoding(runtime));
        checks.AddRange(LongerThanTheUnchargedWorkBound(runtime));

        var failed = 0;

        Console.WriteLine($"# execution: {checks.Count} checks");

        foreach (var (name, passed, detail) in checks)
        {
            if (!passed)
            {
                failed++;
            }

            if (verbose || !passed)
            {
                Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
            }
        }

        Console.WriteLine($"# execution: {checks.Count - failed} of {checks.Count} checks passed");
        return failed;
    }

    // =============================================================================================
    // i32, i64, f32 and f64 arithmetic and conversion
    // =============================================================================================

    private static List<(string, bool, string)> Numeric(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var binary = assembler.Type([WasmAssembler.I32, WasmAssembler.I32], [WasmAssembler.I32]);
        var wide = assembler.Type([WasmAssembler.I64, WasmAssembler.I64], [WasmAssembler.I64]);
        var single = assembler.Type([WasmAssembler.F32, WasmAssembler.F32], [WasmAssembler.F32]);
        var twin = assembler.Type([WasmAssembler.F64, WasmAssembler.F64], [WasmAssembler.F64]);
        var narrowing = assembler.Type([WasmAssembler.F64], [WasmAssembler.I32]);
        var widening = assembler.Type([WasmAssembler.I32], [WasmAssembler.F64]);

        var add = assembler.Function(binary, [], Binary(Instruction.I32Add));
        var divs = assembler.Function(binary, [], Binary(Instruction.I32DivS));
        var rems = assembler.Function(binary, [], Binary(Instruction.I32RemS));
        var shl = assembler.Function(binary, [], Binary(Instruction.I32Shl));
        var rotl = assembler.Function(binary, [], Binary(Instruction.I32Rotl));
        var mul64 = assembler.Function(wide, [], Binary(Instruction.I64Mul));
        var shr64 = assembler.Function(wide, [], Binary(Instruction.I64ShrU));
        var div32 = assembler.Function(single, [], Binary(Instruction.F32Div));
        var min32 = assembler.Function(single, [], Binary(Instruction.F32Min));
        var add64 = assembler.Function(twin, [], Binary(Instruction.F64Add));

        var trunc = assembler.Function(
            narrowing, [], Instruction.Cat(Instruction.LocalGet(0), [Instruction.I32TruncF64S]));

        var convert = assembler.Function(
            widening, [], Instruction.Cat(Instruction.LocalGet(0), [Instruction.F64ConvertI32S]));

        var sqrt = assembler.Function(
            assembler.Type([WasmAssembler.F64], [WasmAssembler.F64]),
            [],
            Instruction.Cat(Instruction.LocalGet(0), [Instruction.F64Sqrt]));

        assembler.Export("add", WasmAssembler.ExportFunction, add);
        assembler.Export("divs", WasmAssembler.ExportFunction, divs);
        assembler.Export("rems", WasmAssembler.ExportFunction, rems);
        assembler.Export("shl", WasmAssembler.ExportFunction, shl);
        assembler.Export("rotl", WasmAssembler.ExportFunction, rotl);
        assembler.Export("i64mul", WasmAssembler.ExportFunction, mul64);
        assembler.Export("i64shr", WasmAssembler.ExportFunction, shr64);
        assembler.Export("f32div", WasmAssembler.ExportFunction, div32);
        assembler.Export("f32min", WasmAssembler.ExportFunction, min32);
        assembler.Export("f64add", WasmAssembler.ExportFunction, add64);
        assembler.Export("f64sqrt", WasmAssembler.ExportFunction, sqrt);
        assembler.Export("trunc", WasmAssembler.ExportFunction, trunc);
        assembler.Export("conv", WasmAssembler.ExportFunction, convert);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "numeric", results,
        [
            (Entry("add", I32(7), I32(35)), Int32(42)),
            (Entry("divs", I32(-7), I32(2)), Int32(-3)),
            (Entry("rems", I32(-7), I32(2)), Int32(-1)),
            (Entry("shl", I32(1), I32(4)), Int32(16)),
            (Entry("rotl", I32(unchecked((int)0x80000001)), I32(1)), Int32(3)),
            (Entry("i64mul", I64(11), I64(3)), Int64(33)),
            (Entry("i64shr", I64(-1), I64(60)), Int64(15)),
            (Entry("f32div", F32(1.0f), F32(4.0f)), Single(0.25f)),
            (Entry("f32min", F32(-0.0f), F32(0.0f)), Single(-0.0f)),
            (Entry("f64add", F64(1.5), F64(2.25)), Double(3.75)),
            (Entry("f64sqrt", F64(2.0)), Double(System.Math.Sqrt(2.0))),
            (Entry("trunc", F64(-3.9)), Int32(-3)),
            (Entry("conv", I32(-5)), Double(-5.0)),
        ]);

        return results;
    }

    // =============================================================================================
    // Locals and every structured control form
    // =============================================================================================

    private static List<(string, bool, string)> Control(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);

        // A counted loop: block { loop { if n == 0 break; sum += n; n -= 1; continue } }
        var sum = assembler.Function(unary, [WasmAssembler.I32], Instruction.Cat(
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.Loop(Instruction.EmptyBlock),
            Instruction.LocalGet(0),
            [Instruction.I32Eqz],
            Instruction.BrIf(1),
            Instruction.LocalGet(1),
            Instruction.LocalGet(0),
            [Instruction.I32Add],
            Instruction.LocalSet(1),
            Instruction.LocalGet(0),
            Instruction.I32Const(1),
            [Instruction.I32Sub],
            Instruction.LocalSet(0),
            Instruction.Br(0),
            Instruction.End(),
            Instruction.End(),
            Instruction.LocalGet(1)));

        // Four nested blocks and one branch table selecting between them.
        var classify = assembler.Function(unary, [], Instruction.Cat(
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.LocalGet(0),
            Instruction.BrTable([0, 1, 2], 3),
            Instruction.End(),
            Instruction.I32Const(10),
            [Instruction.Return],
            Instruction.End(),
            Instruction.I32Const(20),
            [Instruction.Return],
            Instruction.End(),
            Instruction.I32Const(30),
            [Instruction.Return],
            Instruction.End(),
            Instruction.I32Const(99)));

        var conditional = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0),
            Instruction.If(WasmAssembler.I32),
            Instruction.I32Const(111),
            Instruction.Else(),
            Instruction.I32Const(222),
            Instruction.End()));

        var select = assembler.Function(unary, [], Instruction.Cat(
            Instruction.I32Const(5),
            Instruction.I32Const(6),
            Instruction.LocalGet(0),
            [Instruction.Select]));

        var tee = assembler.Function(unary, [WasmAssembler.I32], Instruction.Cat(
            Instruction.LocalGet(0),
            Instruction.I32Const(3),
            [Instruction.I32Mul],
            Instruction.LocalTee(1),
            Instruction.LocalGet(1),
            [Instruction.I32Add]));

        assembler.Export("sum", WasmAssembler.ExportFunction, sum);
        assembler.Export("classify", WasmAssembler.ExportFunction, classify);
        assembler.Export("iff", WasmAssembler.ExportFunction, conditional);
        assembler.Export("sel", WasmAssembler.ExportFunction, select);
        assembler.Export("tee", WasmAssembler.ExportFunction, tee);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "control", results,
        [
            (Entry("sum", I32(5)), Int32(15)),
            (Entry("sum", I32(0)), Int32(0)),
            (Entry("classify", I32(0)), Int32(10)),
            (Entry("classify", I32(1)), Int32(20)),
            (Entry("classify", I32(2)), Int32(30)),
            (Entry("classify", I32(7)), Int32(99)),
            (Entry("iff", I32(1)), Int32(111)),
            (Entry("iff", I32(0)), Int32(222)),
            (Entry("sel", I32(1)), Int32(5)),
            (Entry("sel", I32(0)), Int32(6)),
            (Entry("tee", I32(4)), Int32(24)),
        ]);

        return results;
    }

    // =============================================================================================
    // One linear memory: loads, stores, a data segment, size, growth and the refusal
    // =============================================================================================

    private static List<(string, bool, string)> Memory(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        assembler.Memory(1, null);
        assembler.Data(0, [0x41, 0x42, 0x43]);

        var nullary = assembler.Type([], [WasmAssembler.I32]);
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);

        var roundtrip = assembler.Function(nullary, [], Instruction.Cat(
            Instruction.I32Const(8),
            Instruction.I32Const(123456),
            Instruction.Load(0x36, 2, 0),
            Instruction.I32Const(8),
            Instruction.Load(0x28, 2, 0)));

        var readByte = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0),
            Instruction.Load(0x2D, 0, 0)));

        var size = assembler.Function(
            nullary, [], Instruction.Cat([Instruction.MemorySize], [0x00]));

        var grow = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0), [Instruction.MemoryGrow], [0x00]));

        var outOfBounds = assembler.Function(nullary, [], Instruction.Cat(
            Instruction.I32Const(int.MaxValue - 8),
            Instruction.Load(0x28, 2, 0)));

        assembler.Export("roundtrip", WasmAssembler.ExportFunction, roundtrip);
        assembler.Export("readbyte", WasmAssembler.ExportFunction, readByte);
        assembler.Export("size", WasmAssembler.ExportFunction, size);
        assembler.Export("grow", WasmAssembler.ExportFunction, grow);
        assembler.Export("oob", WasmAssembler.ExportFunction, outOfBounds);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "memory", results,
        [
            (Entry("roundtrip"), Int32(123456)),
            (Entry("readbyte", I32(0)), Int32(0x41)),
            (Entry("readbyte", I32(2)), Int32(0x43)),
            (Entry("size"), Int32(1)),
            (Entry("grow", I32(1)), Int32(1)),
            (Entry("size"), Int32(2)),

            // THE GUEST-OBSERVABLE REFUSAL. The profile's own page ceiling refuses it, no core
            // budget was asked for anything, and the operation completes normally with the module
            // holding the minus one the specification promises it.
            (Entry("grow", I32(2000)), Int32(-1)),
            (Entry("size"), Int32(2)),
            (Entry("oob"), Trap(WasmTrapKind.OutOfBoundsMemoryAccess)),
        ]);

        return results;
    }

    // =============================================================================================
    // Direct calls, a table, an element segment and every indirect-call trap
    // =============================================================================================

    private static List<(string, bool, string)> Calls(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        assembler.Table(4);

        var nullary = assembler.Type([], [WasmAssembler.I32]);
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);
        var binary = assembler.Type([WasmAssembler.I32, WasmAssembler.I32], [WasmAssembler.I32]);

        var seven = assembler.Function(nullary, [], Instruction.I32Const(7));

        var twice = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0), Instruction.I32Const(2), [Instruction.I32Mul]));

        var direct = assembler.Function(nullary, [], Instruction.Cat(
            Instruction.Call(seven), Instruction.I32Const(1), [Instruction.I32Add]));

        var indirect = assembler.Function(binary, [], Instruction.Cat(
            Instruction.LocalGet(0),
            Instruction.LocalGet(1),
            Instruction.CallIndirect(unary)));

        var deep = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0),
            Instruction.I32Const(0),
            [Instruction.I32Eq],
            Instruction.If(WasmAssembler.I32),
            Instruction.I32Const(0),
            Instruction.Else(),
            Instruction.LocalGet(0),
            Instruction.LocalGet(0),
            Instruction.I32Const(1),
            [Instruction.I32Sub],
            Instruction.Call(4),
            [Instruction.I32Add],
            Instruction.End()));

        assembler.Element(0, twice, seven);
        assembler.Export("direct", WasmAssembler.ExportFunction, direct);
        assembler.Export("indirect", WasmAssembler.ExportFunction, indirect);
        assembler.Export("deep", WasmAssembler.ExportFunction, deep);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "calls", results,
        [
            (Entry("direct"), Int32(8)),
            (Entry("indirect", I32(21), I32(0)), Int32(42)),
            (Entry("indirect", I32(21), I32(1)), Trap(WasmTrapKind.IndirectCallTypeMismatch)),
            (Entry("indirect", I32(21), I32(2)), Trap(WasmTrapKind.UninitializedElement)),
            (Entry("indirect", I32(21), I32(9)), Trap(WasmTrapKind.OutOfBoundsTableAccess)),

            // Recursion, which on a heap frame stack costs call depth and never the CLR stack.
            (Entry("deep", I32(100)), Int32(5050)),
        ]);

        return results;
    }

    // =============================================================================================
    // The trap list, one named case each
    // =============================================================================================

    private static List<(string, bool, string)> Traps(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var nullary = assembler.Type([], [WasmAssembler.I32]);
        var binary = assembler.Type([WasmAssembler.I32, WasmAssembler.I32], [WasmAssembler.I32]);
        var narrowing = assembler.Type([WasmAssembler.F64], [WasmAssembler.I32]);

        var unreachable = assembler.Function(nullary, [], Instruction.Op(Instruction.Unreachable));
        var divide = assembler.Function(binary, [], Binary(Instruction.I32DivS));

        var badConversion = assembler.Function(
            narrowing, [], Instruction.Cat(Instruction.LocalGet(0), [Instruction.I32TruncF64S]));

        assembler.Export("unreach", WasmAssembler.ExportFunction, unreachable);
        assembler.Export("divs", WasmAssembler.ExportFunction, divide);
        assembler.Export("badconv", WasmAssembler.ExportFunction, badConversion);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "traps", results,
        [
            (Entry("unreach"), Trap(WasmTrapKind.Unreachable)),
            (Entry("divs", I32(1), I32(0)), Trap(WasmTrapKind.IntegerDivideByZero)),
            (Entry("divs", I32(int.MinValue), I32(-1)), Trap(WasmTrapKind.IntegerOverflow)),
            (Entry("divs", I32(int.MinValue), I32(1)), Int32(int.MinValue)),
            (Entry("badconv", F64(double.NaN)), Trap(WasmTrapKind.InvalidConversionToInteger)),
            (Entry("badconv", F64(1e300)), Trap(WasmTrapKind.IntegerOverflow)),
        ]);

        return results;
    }

    // =============================================================================================
    // Globals, the start function, and the two ways an instantiation refuses
    // =============================================================================================

    private static List<(string, bool, string)> GlobalsAndStart(VmRuntime runtime)
    {
        var results = new List<(string, bool, string)>();

        var withStart = new WasmAssembler();
        var counter = withStart.Global(WasmAssembler.I32, mutable: true, Instruction.I32Const(100));
        var effect = withStart.Type([], []);
        var nullary = withStart.Type([], [WasmAssembler.I32]);

        var initialise = withStart.Function(effect, [], Instruction.Cat(
            Instruction.I32Const(777), Instruction.GlobalSet(counter)));

        var read = withStart.Function(nullary, [], Instruction.GlobalGet(counter));

        var bump = withStart.Function(nullary, [], Instruction.Cat(
            Instruction.GlobalGet(counter),
            Instruction.I32Const(1),
            [Instruction.I32Add],
            Instruction.GlobalSet(counter),
            Instruction.GlobalGet(counter)));

        withStart.Start(initialise);
        withStart.Export("readglobal", WasmAssembler.ExportFunction, read);
        withStart.Export("bump", WasmAssembler.ExportFunction, bump);

        Invoke(runtime, withStart.Build(), "globals-and-start", results,
        [
            // Seven hundred and seventy-seven rather than a hundred is the start function's
            // signature: the initialiser wrote the hundred and the start function replaced it.
            (Entry("readglobal"), Int32(777)),
            (Entry("bump"), Int32(778)),
            (Entry("bump"), Int32(779)),
        ]);

        var trapping = new WasmAssembler();
        var trappingEffect = trapping.Type([], []);
        trapping.Start(trapping.Function(trappingEffect, [], Instruction.Op(Instruction.Unreachable)));
        results.Add(AStartFunctionThatTrapsPublishesNoInstance(runtime, trapping.Build()));

        var overrun = new WasmAssembler();
        overrun.Memory(1, null);
        overrun.Data(0, [0x41, 0x42]);
        overrun.Data(65530, new byte[10]);
        var overrunNullary = overrun.Type([], [WasmAssembler.I32]);
        overrun.Function(overrunNullary, [], Instruction.I32Const(0));
        results.Add(AnOverrunningSegmentPublishesNoInstance(runtime, overrun.Build()));

        return results;
    }

    private static (string, bool, string) AStartFunctionThatTrapsPublishesNoInstance(
        VmRuntime runtime, byte[] module)
    {
        const string Name = "a-trapping-start-function-publishes-no-instance";
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, module, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return (Name, false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);
            var published = instantiated.TryGetInstance(out _);
            var carried = WebAssemblyProfile.TryGetTrap(in instantiated, out var trap);

            var passed = !published && carried && trap.Kind is WasmTrapKind.Unreachable;

            return (Name, passed,
                $"{instantiated.Outcome}/{instantiated.Reason} " +
                $"trap={(carried ? trap.Kind.ToString() : "none")} instance-published={published}");
        }
    }

    private static (string, bool, string) AnOverrunningSegmentPublishesNoInstance(
        VmRuntime runtime, byte[] module)
    {
        const string Name = "an-overrunning-data-segment-publishes-no-instance";
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, module, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return (Name, false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);
            var published = instantiated.TryGetInstance(out _);
            var carried = WebAssemblyProfile.TryGetTrap(in instantiated, out var trap);

            var passed =
                !published && carried && trap.Kind is WasmTrapKind.OutOfBoundsMemoryAccess;

            // THE SECOND HALF OF THE SEGMENT RULE IS NOT OBSERVED HERE AND IS NOT CLAIMED. The
            // first segment was applied before the second was refused, which is what per-segment
            // atomicity means - but no instance is published and this build imports no memory, so
            // there is nothing left holding the bytes the first segment wrote and no way from
            // outside to read them. The milestone that lands imported memories is the one that can
            // assert it.
            return (Name, passed,
                $"{instantiated.Outcome}/{instantiated.Reason} " +
                $"trap={(carried ? trap.Kind.ToString() : "none")} instance-published={published}");
        }
    }

    // =============================================================================================
    // The entry-point encoding over names that contain its own separators
    // =============================================================================================

    private static List<(string, bool, string)> EntryPointEncoding(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);
        var nullary = assembler.Type([], [WasmAssembler.I32]);

        var increment = assembler.Function(unary, [], Instruction.Cat(
            Instruction.LocalGet(0), Instruction.I32Const(1), [Instruction.I32Add]));

        var five = assembler.Function(nullary, [], Instruction.I32Const(5));

        // BOTH NAMES ARE LEGAL WEBASSEMBLY EXPORT NAMES AND BOTH BREAK A SEPARATOR-DELIMITED
        // ENCODING. The first reads as a type-annotated name, the second is byte-for-byte an
        // argument group. Counting bytes is what makes them ordinary.
        assembler.Export("f(i32)", WasmAssembler.ExportFunction, increment);
        assembler.Export("a:3:i32", WasmAssembler.ExportFunction, five);

        var results = new List<(string, bool, string)>();

        Invoke(runtime, assembler.Build(), "entry-point-encoding", results,
        [
            (Entry("f(i32)", I32(41)), Int32(42)),
            (Entry("a:3:i32"), Int32(5)),
        ]);

        results.Add(AnUnknownEntryPointIsATypedFault(runtime, assembler.Build()));
        return results;
    }

    private static (string, bool, string) AnUnknownEntryPointIsATypedFault(
        VmRuntime runtime, byte[] module)
    {
        const string Name = "an-unknown-entry-point-is-a-typed-fault-and-not-a-trap";
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, module, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return (Name, false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return (Name, false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var utf8 = System.Text.Encoding.UTF8.GetBytes(Entry("nowhere"));
            var request = new VmInvocationRequest(new VmUtf8Text(utf8));
            var answered = instance.Invoke(in request, CancellationToken.None);

            var carried = WebAssemblyProfile.TryGetEntryPointFault(in answered, out var fault);
            var noTrap = !WebAssemblyProfile.TryGetTrap(in answered, out _);

            var passed =
                carried && noTrap &&
                fault.Problem is WebAssemblyEntryPointProblem.UnknownExport;

            return (Name, passed,
                $"{answered.Outcome}/{answered.Reason} " +
                $"problem={(carried ? fault.Problem.ToString() : "none")}");
        }
    }

    // =============================================================================================
    // Bodies, segments, sections and names longer than the uncharged-work bound
    // =============================================================================================

    /// <summary>
    /// Valid modules, each longer than the descriptor's uncharged-work bound in a different place.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>EVERY MODULE HERE IS VALID, AND EVERY ONE WAS REFUSED UNTIL THE READS WERE PACED.</b> The
    /// descriptor declares 65,536 units of work between two polls, the decoder charges one unit per
    /// byte it consumes, and it read a function body, a data segment's contents, a name and a skipped
    /// custom section in one charge each, polling only once the work since the last poll had reached
    /// the bound. So a run longer than the bound, or one begun short of a poll and ending past it,
    /// carried the work between two polls past the bound; the meter latched the breach, the next poll
    /// failed, and a well-formed module was answered as an exhausted verifier-work allowance. Three
    /// runs of 40 KiB are here because no single run has to cross the bound for their sum to.
    /// </para>
    /// <para>
    /// <b>The last two are the same defect in an eight-byte read.</b> A fixed-width constant begun one
    /// unit short of a poll ended seven units past the bound, in the decoder and in the validator
    /// alike. Each module is padded so that its constant is read exactly there, and each one checks
    /// that it still is, because a module that drifted off the edge would pass without testing it.
    /// </para>
    /// </remarks>
    private static List<(string, bool, string)> LongerThanTheUnchargedWorkBound(VmRuntime runtime)
    {
        const int Run = 100 * 1024;
        const int Share = 40 * 1024;

        var results = new List<(string, bool, string)>();

        var nops = new WasmAssembler();
        var nopsNullary = nops.Type([], [WasmAssembler.I32]);

        nops.Export("nops", WasmAssembler.ExportFunction, nops.Function(
            nopsNullary, [], Instruction.Cat(Repeat([Instruction.Nop], Run), Instruction.I32Const(42))));

        Invoke(runtime, nops.Build(), "a-100-kib-body-of-nops", results,
        [
            (Entry("nops"), Int32(42)),
        ]);

        var pairs = new WasmAssembler();
        var pairsNullary = pairs.Type([], [WasmAssembler.I32]);
        var pair = Instruction.Cat(Instruction.I32Const(7), [Instruction.Drop]);

        pairs.Export("pairs", WasmAssembler.ExportFunction, pairs.Function(
            pairsNullary, [], Instruction.Cat(Repeat(pair, Run / pair.Length), Instruction.I32Const(43))));

        Invoke(runtime, pairs.Build(), "a-100-kib-body-of-constant-and-drop-pairs", results,
        [
            (Entry("pairs"), Int32(43)),
        ]);

        var shares = new WasmAssembler();
        var sharesNullary = shares.Type([], [WasmAssembler.I32]);

        for (var index = 1; index <= 3; index++)
        {
            shares.Export($"share{index}", WasmAssembler.ExportFunction, shares.Function(
                sharesNullary, [], Instruction.Cat(Repeat([Instruction.Nop], Share), Instruction.I32Const(index))));
        }

        Invoke(runtime, shares.Build(), "three-40-kib-bodies", results,
        [
            (Entry("share1"), Int32(1)),
            (Entry("share2"), Int32(2)),
            (Entry("share3"), Int32(3)),
        ]);

        var contents = new byte[Run];

        for (var index = 0; index < contents.Length; index++)
        {
            contents[index] = (byte)((index * 7) + 3);
        }

        var segment = new WasmAssembler();
        segment.Memory(2, null);
        segment.Data(0, contents);

        segment.Export("readbyte", WasmAssembler.ExportFunction, segment.Function(
            segment.Type([WasmAssembler.I32], [WasmAssembler.I32]),
            [],
            Instruction.Cat(Instruction.LocalGet(0), Instruction.Load(0x2D, 0, 0))));

        Invoke(runtime, segment.Build(), "a-100-kib-data-segment", results,
        [
            (Entry("readbyte", I32(0)), Int32(contents[0])),
            (Entry("readbyte", I32(Run - 1)), Int32(contents[Run - 1])),
        ]);

        var custom = new WasmAssembler();
        custom.Custom("pacing", new byte[Run]);

        custom.Export("five", WasmAssembler.ExportFunction, custom.Function(
            custom.Type([], [WasmAssembler.I32]), [], Instruction.I32Const(5)));

        Invoke(runtime, custom.Build(), "a-100-kib-custom-section", results,
        [
            (Entry("five"), Int32(5)),
        ]);

        // Two-byte scalar values over an odd read window, so a piece boundary falls inside one: the
        // UTF-8 rule has to be applied to the whole name and not to a piece of it.
        var named = new WasmAssembler();
        var six = named.Function(named.Type([], [WasmAssembler.I32]), [], Instruction.I32Const(6));
        named.Export(string.Concat(Enumerable.Repeat("é", Run / 2)), WasmAssembler.ExportFunction, six);
        named.Export("short", WasmAssembler.ExportFunction, six);

        Invoke(runtime, named.Build(), "a-100-kib-export-name", results,
        [
            (Entry("short"), Int32(6)),
        ]);

        results.Add(AnEightByteConstantOneUnitShortOfAPoll(runtime));
        results.Add(AnEightByteImmediateOneUnitShortOfAPoll(runtime));

        return results;
    }

    /// <summary>
    /// A global's eight-byte initialiser, read by the decoder with the work since its last poll one
    /// unit short of the bound.
    /// </summary>
    /// <remarks>
    /// The payload bytes are the decoder's work units, and nothing before the immediate is long
    /// enough to be polled inside under the old cadence, so the immediate's offset is the work the
    /// decoder had done unpolled when it began reading it. A leading custom section pads it there.
    /// </remarks>
    private static (string, bool, string) AnEightByteConstantOneUnitShortOfAPoll(VmRuntime runtime)
    {
        const string Name = "an-eight-byte-global-initialiser-read-one-unit-short-of-a-poll";
        const double Avogadro = 6.02214076e23;
        var target = (int)WebAssemblyProfile.MaxUnchargedWork - 1;
        var immediate = BitConverter.GetBytes(BitConverter.DoubleToUInt64Bits(Avogadro));

        byte[] Build(int padding)
        {
            var assembler = new WasmAssembler();
            assembler.Custom("pad", new byte[padding]);
            var global = assembler.Global(WasmAssembler.F64, mutable: false, Instruction.F64Const(Avogadro));

            assembler.Export("avogadro", WasmAssembler.ExportFunction, assembler.Function(
                assembler.Type([], [WasmAssembler.F64]), [], Instruction.GlobalGet(global)));

            return assembler.Build();
        }

        var padding = 0;
        var module = Build(padding);
        var at = module.AsSpan().IndexOf(immediate);

        // Growing the payload can lengthen the section's own length field, so the padding is found
        // by adjusting rather than by one subtraction.
        for (var attempt = 0; attempt < 3 && at != target; attempt++)
        {
            padding += target - at;
            module = Build(padding);
            at = module.AsSpan().IndexOf(immediate);
        }

        return Placed(runtime, Name, module, at, target, "avogadro", Double(Avogadro));
    }

    /// <summary>
    /// The same eight-byte read in the validator, which walks a body behind a fresh reader of its own.
    /// </summary>
    /// <remarks>
    /// The validator polls as it opens a body and every thousand and twenty-four instructions after,
    /// and a branch table is one instruction however many labels it carries, so a table of one-byte
    /// labels is how the bytes before the constant grow without a poll among them.
    /// </remarks>
    private static (string, bool, string) AnEightByteImmediateOneUnitShortOfAPoll(VmRuntime runtime)
    {
        const string Name = "an-eight-byte-immediate-validated-one-unit-short-of-a-poll";
        const double Marker = -1.0 / 3.0;
        var target = (int)WebAssemblyProfile.MaxUnchargedWork - 1;
        var immediate = BitConverter.GetBytes(BitConverter.DoubleToUInt64Bits(Marker));

        // Everything before the labels and the constant's opcode after them: a block, a constant, the
        // branch table's opcode, its three-byte label count and its default label.
        var labels = target - 10;

        var code = Instruction.Cat(
            Instruction.Block(Instruction.EmptyBlock),
            Instruction.I32Const(0),
            Instruction.BrTable(new uint[labels], 0),
            Instruction.F64Const(Marker),
            [Instruction.Drop],
            Instruction.End(),
            Instruction.I32Const(9));

        var assembler = new WasmAssembler();

        assembler.Export("table", WasmAssembler.ExportFunction, assembler.Function(
            assembler.Type([], [WasmAssembler.I32]), [], code));

        return Placed(
            runtime, Name, assembler.Build(), code.AsSpan().IndexOf(immediate), target, "table", Int32(9));
    }

    /// <summary>
    /// Verifies, instantiates and invokes a module whose eight-byte constant must sit at one offset,
    /// failing the check first if it does not.
    /// </summary>
    private static (string, bool, string) Placed(
        VmRuntime runtime, string name, byte[] module, int at, int target, string export, Expectation expected)
    {
        if (at != target)
        {
            return (name, false, $"the constant is read at offset {at} and the check needs it at {target}");
        }

        var results = new List<(string, bool, string)>();
        Invoke(runtime, module, name, results, [(Entry(export), expected)]);

        var failed = results.Where(result => !result.Item2).ToList();

        return failed.Count == 0
            ? (name, true, $"constant at offset {at}; {results[^1].Item3}")
            : (name, false, $"{failed[0].Item1[(name.Length + 2)..]}: {failed[0].Item3}");
    }

    private static byte[] Repeat(byte[] unit, int times)
    {
        var encoded = new byte[unit.Length * times];

        for (var index = 0; index < times; index++)
        {
            unit.CopyTo(encoded, index * unit.Length);
        }

        return encoded;
    }

    // =============================================================================================
    // The driver every check above shares
    // =============================================================================================

    private static void Invoke(
        VmRuntime runtime,
        byte[] module,
        string label,
        List<(string, bool, string)> results,
        (string Entry, Expectation Expected)[] calls)
    {
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, module, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            // An exhaustion carries no diagnostic code and no position; what it carries is the
            // dimension and the scope that refused, and those are what tell one from another.
            results.Add((
                $"{label}: verification",
                false,
                verified.Outcome is VmOutcome.ResourceExhaustion
                    ? $"{verified.Outcome}/{verified.Reason}/" +
                      $"{verified.Diagnostics.ExhaustedDimension}/{verified.Diagnostics.ExhaustedScope}"
                    : $"{verified.Outcome}/{verified.Reason}/" +
                      $"{verified.Diagnostics.ProfileDiagnosticCode} at " +
                      $"offset {verified.Diagnostics.SourcePosition.ByteOffset}"));

            return;
        }

        results.Add((
            $"{label}: verification",
            true,
            $"{verified.Outcome}/{verified.Reason}, {module.Length} bytes"));

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                results.Add((
                    $"{label}: instantiation",
                    false,
                    $"{instantiated.Outcome}/{instantiated.Reason}"));

                return;
            }

            results.Add((
                $"{label}: instantiation", true, $"{instantiated.Outcome}/{instantiated.Reason}"));

            foreach (var (entry, expected) in calls)
            {
                var utf8 = System.Text.Encoding.UTF8.GetBytes(entry);
                var request = new VmInvocationRequest(new VmUtf8Text(utf8));
                var answered = instance.Invoke(in request, CancellationToken.None);
                results.Add(Judge($"{label}: {entry}", answered, expected));
            }
        }
    }

    private static (string, bool, string) Judge(
        string name, in VmInvocationResult answered, Expectation expected)
    {
        if (expected.Trap is not null)
        {
            var carried = WebAssemblyProfile.TryGetTrap(in answered, out var trap);

            return (name,
                carried &&
                    trap.Kind == expected.Trap.Value &&
                    answered.Outcome is VmOutcome.ProfileFault,
                $"{answered.Outcome}/{answered.Reason} " +
                $"trap={(carried ? $"{trap.Kind}/{trap.DiagnosticCode}" : "none")} " +
                $"expected trap={expected.Trap}");
        }

        if (!WebAssemblyProfile.TryGetResults(in answered, out var results))
        {
            return (name, false, $"{answered.Outcome}/{answered.Reason}, no results payload");
        }

        if (results.Count != 1 || !results.TryGetValue(0, out var value))
        {
            return (name, false, $"{results.Count} results, expected one");
        }

        var matched = value.Kind == expected.Kind && value.Bits == expected.Bits;

        return (name, matched && answered.Outcome is VmOutcome.Normal,
            $"{answered.Outcome}/{answered.Reason} {Describe(value)}" +
            (matched ? string.Empty : $", expected {expected.Kind} 0x{expected.Bits:X}"));
    }

    private static string Describe(WebAssemblyValue value) => value.Kind switch
    {
        WebAssemblyValueKind.I32 =>
            $"i32 {value.AsInt32.ToString(CultureInfo.InvariantCulture)}",
        WebAssemblyValueKind.I64 =>
            $"i64 {value.AsInt64.ToString(CultureInfo.InvariantCulture)}",
        WebAssemblyValueKind.F32 =>
            $"f32 {value.AsSingle.ToString("R", CultureInfo.InvariantCulture)} (0x{(uint)value.Bits:X8})",
        _ =>
            $"f64 {value.AsDouble.ToString("R", CultureInfo.InvariantCulture)} (0x{value.Bits:X16})",
    };

    private static byte[] Binary(byte opcode) => Instruction.Cat(
        Instruction.LocalGet(0), Instruction.LocalGet(1), [opcode]);

    private static VmArtifactDescriptor Descriptor() =>
        new(WebAssemblyProfile.Id, 1, WebAssemblyProfile.SliceManifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-wasm-harness://execution"));

    // =============================================================================================
    // The entry-point encoding, written once so no check spells a byte count by hand
    // =============================================================================================

    private static string Entry(string name, params string[] arguments) =>
        string.Concat(
            System.Text.Encoding.UTF8.GetByteCount(name).ToString(CultureInfo.InvariantCulture),
            ":",
            name,
            string.Concat(arguments));

    private static string I32(int value) => Argument("i32", value.ToString(CultureInfo.InvariantCulture));

    private static string I64(long value) => Argument("i64", value.ToString(CultureInfo.InvariantCulture));

    private static string F32(float value) =>
        Argument("f32", BitConverter.SingleToUInt32Bits(value).ToString("X8", CultureInfo.InvariantCulture));

    private static string F64(double value) =>
        Argument("f64", BitConverter.DoubleToUInt64Bits(value).ToString("X16", CultureInfo.InvariantCulture));

    private static string Argument(string type, string literal) =>
        $"{type}:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";

    private readonly struct Expectation
    {
        internal Expectation(WebAssemblyValueKind kind, ulong bits, WasmTrapKind? trap)
        {
            Kind = kind;
            Bits = bits;
            Trap = trap;
        }

        internal WebAssemblyValueKind Kind { get; }

        internal ulong Bits { get; }

        internal WasmTrapKind? Trap { get; }
    }

    private static Expectation Int32(int value) =>
        new(WebAssemblyValueKind.I32, (uint)value, null);

    private static Expectation Int64(long value) =>
        new(WebAssemblyValueKind.I64, (ulong)value, null);

    private static Expectation Single(float value) =>
        new(WebAssemblyValueKind.F32, BitConverter.SingleToUInt32Bits(value), null);

    private static Expectation Double(double value) =>
        new(WebAssemblyValueKind.F64, BitConverter.DoubleToUInt64Bits(value), null);

    private static Expectation Trap(WasmTrapKind kind) =>
        new(WebAssemblyValueKind.I32, 0, kind);
}
