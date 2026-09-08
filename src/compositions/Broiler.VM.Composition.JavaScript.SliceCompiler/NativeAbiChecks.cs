using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The calling-convention obligation table, as rows with an injected violation each.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE TEST THAT WOULD HAVE CAUGHT <c>ret 8</c>, AND IT IS IN THE FAST DETERMINISTIC
/// SUITE RATHER THAN IN THE SOAK FOR THE EXACT REASON THE ANECDOTE TEACHES.</b> That defect was a
/// callee removing the wrong number of bytes from its caller's stack, and what made it expensive
/// was that nothing failed at the call: the process ran for hours and died somewhere unrelated to
/// the defect. A delayed death has to be FORCED TO ARRIVE EARLY. So the stack pointer is compared
/// immediately either side of the call, and the call is then repeated enough times that a leak too
/// small to see once cannot survive.
/// </para>
/// <para>
/// <b>EVERY ROW HAS AN INJECTION AND THE INJECTION IS ASSERTED TO BE CAUGHT.</b> An obligation
/// checked only over conforming code is an obligation nobody has shown can fail, which is the same
/// standing as no check at all - and it is the standing the original claim had. So each row here
/// runs twice: once over code that honours the obligation, which must pass, and once over code
/// written to violate it, which must fail. A row whose injection passes is a row this file reports
/// as broken.
/// </para>
/// <para>
/// <b>The trampoline is built with the backend's own encoder, which is why this file is in a
/// composition and not in the profile.</b> The profile owns the mapping and owns nothing that can
/// encode an instruction; the lowering owns the encoder and can map nothing. A check that needs
/// both belongs where both are already named, which is a composition root that carries a compiler.
/// </para>
/// <para>
/// <b>RBP IS THE ONE CALLEE-SAVED REGISTER THIS TRAMPOLINE CANNOT OBSERVE, and the reason is the
/// injection it exists to survive.</b> A trampoline that trusted the stack pointer after the call
/// could not return at all from a callee that shifted it - which is the <c>ret 8</c> injection
/// exactly - so it keeps its own frame in RBP and restores the stack pointer from there. A register
/// used to survive a defect cannot also be a witness to a different one.
/// </para>
/// </remarks>
internal static class NativeAbiChecks
{
    /// <summary>The sentinel each observed callee-saved register is loaded with before the call.</summary>
    /// <remarks>
    /// Distinct per register and distinct from anything a program of this manifest can compute, so
    /// a register that comes back holding another register's sentinel is as visible as one holding
    /// a number.
    /// </remarks>
    private static readonly long[] Sentinels =
    [
        0x1111_1111_1111_1111L,
        0x2222_2222_2222_2222L,
        0x3333_3333_3333_3333L,
        0x4444_4444_4444_4444L,
        0x5555_5555_5555_5555L,
        0x6666_6666_6666_6666L,
        0x7777_7777_7777_7777L,
        unchecked((long)0x8888_8888_8888_8888UL),
    ];

    /// <summary>The registers the trampoline loads sentinels into and reports back, in order.</summary>
    private static readonly JsX64Register[] Observed =
    [
        JsX64Register.Rbx,
        JsX64Register.Rbp,
        JsX64Register.Rsi,
        JsX64Register.Rdi,
        JsX64Register.R12,
        JsX64Register.R13,
        JsX64Register.R14,
        JsX64Register.R15,
    ];

    /// <summary>The register whose slot the trampoline uses as its own frame and cannot witness.</summary>
    private const int FramePointerSlot = 1;

    /// <summary>What the other convention's argument register is loaded with before the call.</summary>
    private const long WrongRegisterSentinel = 0x0BAD_0BAD_0BAD_0BADL;

    /// <summary>
    /// How many times each entry point is called.
    /// </summary>
    /// <remarks>
    /// <b>A MILLION, BECAUSE A MILLION TIMES EIGHT BYTES IS MORE STACK THAN A THREAD HAS.</b> A
    /// callee that shifts the stack pointer by one slot per call passes any comparison that happens
    /// to be made against a value the same defect already moved; it cannot survive a million calls.
    /// The figure is chosen from the arithmetic and not from taste: eight megabytes of drift
    /// exceeds every default thread stack this product runs on.
    /// </remarks>
    private const int Repetitions = 1_000_000;

    /// <summary>Runs the obligation table.</summary>
    internal static List<(string Name, bool Passed, string Detail)> Run()
    {
        var checks = new List<(string, bool, string)>();
        var abi = JsX64Abi.Host;

        if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture !=
            System.Runtime.InteropServices.Architecture.X64)
        {
            // A REFUSAL AND NOT A SKIP THAT LOOKS LIKE A PASS. A row reported green on a machine
            // that cannot run the code it is about is the shape of evidence this repository exists
            // to refuse.
            checks.Add((
                "native/abi/architecture",
                false,
                "this machine is not x86-64, so no row of the x86-64 obligation table was run and " +
                "none of them may be reported as held"));

            return checks;
        }

        checks.Add(StackPointerIsUnchanged(abi));
        checks.Add(CallerPopsIsDetectable(abi));
        checks.Add(CalleeSavedAreUnchanged(abi));
        checks.Add(ClobberIsDetectable(abi));
        checks.Add(FramePointerArrivesInTheRightRegister(abi));
        checks.Add(WrongFramePointerRegisterIsDetectable(abi));
        checks.Add(StackIsAlignedAtTheCall(abi));
        checks.Add(MisalignmentIsDetectable(abi));
        checks.Add(ShadowSpaceIsReserved(abi));
        checks.Add(TheTwoConventionsDisagreeAboutRdiAndRsi());
        checks.Add(FrameOffsetsMatchTheRuntime());
        checks.Add(EmittedEntryPointsSurviveAMillionCalls(abi));
        checks.Add(TheTwoFormsAgree(abi));
        checks.Add(ReEmissionIsByteIdentical(abi));

        return checks;
    }

    // ---- the rows -----------------------------------------------------------------------------

    private static (string, bool, string) StackPointerIsUnchanged(JsX64Abi abi)
    {
        var seen = Observe(abi, Conforming(abi), 0, 1);

        return (
            "native/abi/stack-pointer-unchanged",
            seen.Ran && seen.StackBefore == seen.StackAfter,
            seen.Ran
                ? "before 0x" + seen.StackBefore.ToString("x") +
                    ", after 0x" + seen.StackAfter.ToString("x")
                : "the mapping could not be made or armed");
    }

    private static (string, bool, string) CallerPopsIsDetectable(JsX64Abi abi)
    {
        // THE INJECTION IS LITERALLY `ret 8`, hand-written as three bytes rather than through the
        // encoder. The encoder has no immediate return form and its documentation says so; adding
        // one so that a test could use it would make the documentation false. `C2 imm16` is the
        // callee-pops return the anecdote is about, and on x86-64 it is legal to encode and wrong
        // to use.
        var injected = Conforming(abi, tail: assembler =>
        {
            assembler.Emit(0xC2);
            assembler.Emit(0x08);
            assembler.Emit(0x00);
        });

        var seen = Observe(abi, injected, 0, 1);

        return (
            "native/abi/caller-pops-injection-is-caught",
            seen.Ran && seen.StackAfter == seen.StackBefore + 8,
            seen.Ran
                ? "a callee that removed eight bytes moved the stack pointer by " +
                    (seen.StackAfter - seen.StackBefore).ToString()
                : "the mapping could not be made or armed");
    }

    private static (string, bool, string) CalleeSavedAreUnchanged(JsX64Abi abi)
    {
        var seen = Observe(abi, Conforming(abi), 0, 1);
        var wrong = FirstClobbered(abi, seen);

        return (
            "native/abi/callee-saved-unchanged",
            seen.Ran && wrong is null,
            wrong is null
                ? "every register this convention makes callee-saved came back as it went in"
                : "register " + wrong + " came back changed");
    }

    private static (string, bool, string) ClobberIsDetectable(JsX64Abi abi)
    {
        // R12 is callee-saved under BOTH conventions, so this injection is caught by either row.
        var injected = Conforming(abi, body: assembler =>
            assembler.MovRegisterImmediate64(JsX64Register.R12, 0x0DEF_0DEF_0DEF_0DEFL));

        var seen = Observe(abi, injected, 0, 1);
        var wrong = FirstClobbered(abi, seen);

        return (
            "native/abi/clobber-injection-is-caught",
            seen.Ran && wrong == JsX64Register.R12,
            wrong is null
                ? "a callee that destroyed R12 was NOT detected"
                : "the clobbered register was reported as " + wrong);
    }

    private static (string, bool, string) FramePointerArrivesInTheRightRegister(JsX64Abi abi)
    {
        // The target answers with the low half of the register the convention says the argument
        // arrives in. Nothing is dereferenced, so a wrong answer is a wrong answer and never a
        // fault.
        var target = Conforming(abi, body: assembler =>
            assembler.MovRegisterRegister(JsX64Register.Rax, abi.FramePointerRegister));

        var seen = Observe(abi, target, 0, 1);
        var held = seen.Ran && seen.Answer != WrongRegisterSentinel && seen.Answer != 0;

        return (
            "native/abi/frame-pointer-register",
            held,
            "the frame pointer arrives in " + abi.FramePointerRegister + " under " + abi.Name +
                ", and the callee read 0x" + seen.Answer.ToString("x"));
    }

    private static (string, bool, string) WrongFramePointerRegisterIsDetectable(JsX64Abi abi)
    {
        // THE INJECTION IS THE HABIT THE CONVENTION TABLE EXISTS TO PREVENT: reading the argument
        // out of the register the OTHER convention uses. On a real frame that is a dereference of
        // whatever the caller left behind, which faults if you are lucky and answers a wrong number
        // if you are not; here it is a move, so the wrong answer is observable and nothing faults.
        var other = abi.FramePointerRegister == JsX64Register.Rcx
            ? JsX64Register.Rdi
            : JsX64Register.Rcx;

        var injected = Conforming(abi, body: assembler =>
            assembler.MovRegisterRegister(JsX64Register.Rax, other));

        var seen = Observe(abi, injected, 0, 1);

        return (
            "native/abi/wrong-frame-pointer-register-is-caught",
            seen.Ran && seen.Answer == WrongRegisterSentinel,
            "reading the argument from " + other + " under " + abi.Name + " answered 0x" +
                seen.Answer.ToString("x"));
    }

    private static (string, bool, string) StackIsAlignedAtTheCall(JsX64Abi abi)
    {
        var seen = Observe(abi, Conforming(abi), 0, 1);

        return (
            "native/abi/stack-aligned-at-the-call",
            seen.Ran && (seen.StackBefore % 16) == abi.StackAlignmentAtCall,
            "the stack pointer at the call was 0x" + seen.StackBefore.ToString("x") +
                ", which is " + (seen.StackBefore % 16) + " modulo sixteen");
    }

    private static (string, bool, string) MisalignmentIsDetectable(JsX64Abi abi)
    {
        // The injection is on the CALLER'S side, because alignment at a call is the caller's
        // obligation: a trampoline that reserved eight bytes more makes the call from an address
        // the convention forbids.
        var seen = Observe(abi, Conforming(abi), 0, 1, misalign: true);

        return (
            "native/abi/misalignment-is-caught",
            seen.Ran && (seen.StackBefore % 16) != abi.StackAlignmentAtCall,
            "a caller that reserved eight bytes more called with a stack pointer " +
                (seen.StackBefore % 16) + " modulo sixteen");
    }

    private static (string, bool, string) ShadowSpaceIsReserved(JsX64Abi abi)
    {
        // A STRUCTURAL ROW AND NOT A DYNAMIC ONE, AND THE DIFFERENCE IS SAID RATHER THAN HIDDEN.
        // What the obligation says is that a caller reserves thirty-two bytes below its own frame
        // that the callee may spill its arguments into; violating it destroys whatever the caller
        // had there, which is a corruption rather than an observation. So this row reads the frame
        // the emitter reserves and checks that the slot it spills the frame pointer into is ABOVE
        // the shadow space rather than inside it - which is the property that makes the corruption
        // impossible rather than unlikely.
        var held = abi.FrameBytes >= abi.ShadowSpaceBytes + 8 &&
            (abi.FrameBytes % 16) == 0 &&
            abi.FramePointerSlot >= abi.ShadowSpaceBytes;

        return (
            "native/abi/shadow-space-reserved",
            held,
            abi.Name + " reserves " + abi.ShadowSpaceBytes + " bytes of shadow space, and an " +
                "emitted prologue claims " + abi.FrameBytes + " bytes with its own spill at " +
                abi.FramePointerSlot);
    }

    private static (string, bool, string) TheTwoConventionsDisagreeAboutRdiAndRsi()
    {
        // THE ROW THE WHOLE TABLE EXISTS FOR. RDI and RSI are callee-saved under Windows x64 and
        // volatile under System V; an emitter that used either as scratch would be correct on one
        // system and would quietly destroy a caller's register on the other. Asserting the
        // disagreement here means a table edited to make the two rows agree fails rather than
        // silently widening what an emitter may touch.
        var windows = JsX64Abi.Windows;
        var systemV = JsX64Abi.SystemV;

        var held =
            windows.IsCalleeSaved(JsX64Register.Rdi) &&
            windows.IsCalleeSaved(JsX64Register.Rsi) &&
            !systemV.IsCalleeSaved(JsX64Register.Rdi) &&
            !systemV.IsCalleeSaved(JsX64Register.Rsi) &&
            windows.FramePointerRegister == JsX64Register.Rcx &&
            systemV.FramePointerRegister == JsX64Register.Rdi &&
            windows.ShadowSpaceBytes == 32 &&
            systemV.ShadowSpaceBytes == 0;

        return (
            "native/abi/the-two-conventions-disagree",
            held,
            "RDI and RSI are callee-saved under Windows x64 and volatile under System V, the " +
                "frame pointer arrives in RCX and RDI respectively, and only Windows x64 asks a " +
                "caller for shadow space");
    }

    private static unsafe (string, bool, string) FrameOffsetsMatchTheRuntime()
    {
        // The emitter computes with numbers and the two structures state a layout; this is the
        // check that the two agree, made against the layout the RUNTIME gives rather than against
        // the comment that describes it. The runtime is asked where it put each field, which is the
        // same question an encoder needs answered and is the only form of the question that cannot
        // be wrong in the same direction as the encoder.
        //
        // IT ASKS WITH ADDRESSES AND NOT WITH `Marshal`, and the difference is the whole reason
        // this method is `unsafe` *(2026-09-08)*. It used `Marshal.OffsetOf` and `Marshal.SizeOf`,
        // which answer where a field would sit AFTER MARSHALLING and need interop data the ILC
        // generates only for structures it sees crossing a marshalling boundary. Neither of these
        // ever does - both are handed to emitted code as a raw address - so the published Native
        // AOT image threw `NotSupportedException: StructMarshalling_MissingInteropData` on the one
        // configuration the lane publishes, while every JIT build passed. Two structures that are
        // blittable and sequentially laid out have one layout rather than two, so subtracting
        // addresses asks the same question of the same runtime, needs no interop data, and is the
        // form of the question that matches what the emitted code will actually see.
        var frame = default(JsNativeFrame);
        var probe = default(JsNativeAbiProbe);

        var frameBase = (byte*)&frame;
        var probeBase = (byte*)&probe;

        var held =
            (int)((byte*)&frame.Operands - frameBase) == JsX64Frame.OperandsOffset &&
            (int)((byte*)&frame.OperandCount - frameBase) == JsX64Frame.OperandCountOffset &&
            (int)((byte*)&frame.Locals - frameBase) == JsX64Frame.LocalsOffset &&
            (int)((byte*)&frame.LocalCount - frameBase) == JsX64Frame.LocalCountOffset &&
            (int)((byte*)&frame.Constants - frameBase) == JsX64Frame.ConstantsOffset &&
            (int)((byte*)&frame.Fuel - frameBase) == JsX64Frame.FuelOffset &&
            (int)((byte*)&probe.Target - probeBase) == JsNativeAbiProbeLayout.TargetOffset &&
            (int)((byte*)&probe.Frame - probeBase) == JsNativeAbiProbeLayout.FrameOffset &&
            (int)((byte*)&probe.StackBefore - probeBase) == JsNativeAbiProbeLayout.StackBeforeOffset &&
            (int)((byte*)&probe.StackAfter - probeBase) == JsNativeAbiProbeLayout.StackAfterOffset &&
            (int)((byte*)&probe.Answer - probeBase) == JsNativeAbiProbeLayout.AnswerOffset &&
            (int)((byte*)probe.Saved - probeBase) == JsNativeAbiProbeLayout.SavedOffset &&
            sizeof(JsNativeAbiProbe) == JsNativeAbiProbeLayout.Bytes;

        return (
            "native/abi/frame-offsets-match-the-runtime",
            held,
            "the offsets an encoder computes with are the offsets the runtime gives the " +
                "structures they describe");
    }

    private static (string, bool, string) EmittedEntryPointsSurviveAMillionCalls(JsX64Abi abi)
    {
        // THE REAL BACKEND'S OUTPUT AND NOT A STUB. Every row above is about the convention; this
        // one is about what the emitter actually wrote, and it is the row that would have caught
        // the anecdote's defect in the code it was in rather than in a model of it.
        if (!TryEmit(
            abi,
            "function f(n) { return n + 1; }\nf(1);\n",
            out var code,
            out var symbols,
            out var why))
        {
            return ("native/abi/emitted-entry-points-survive", false, why);
        }

        foreach (var symbol in symbols)
        {
            var seen = Observe(abi, code, symbol.Offset, Repetitions);

            if (!seen.Ran || seen.StackBefore != seen.StackAfter)
            {
                return (
                    "native/abi/emitted-entry-points-survive",
                    false,
                    "code unit " + symbol.FunctionIndex + " moved the stack pointer by " +
                        (seen.StackAfter - seen.StackBefore) + " over " + Repetitions + " calls");
            }

            if (FirstClobbered(abi, seen) is { } wrong)
            {
                return (
                    "native/abi/emitted-entry-points-survive",
                    false,
                    "code unit " + symbol.FunctionIndex + " came back with " + wrong + " changed");
            }
        }

        return (
            "native/abi/emitted-entry-points-survive",
            true,
            symbols.Length + " emitted entry points, each called " + Repetitions +
                " times, each leaving the stack pointer and every callee-saved register as it " +
                "found them");
    }

    private static (string, bool, string) TheTwoFormsAgree(JsX64Abi abi)
    {
        // THE COMPENSATING CONTROL THE CORE ROADMAP SAYS A BACKEND MAY NOT SHIP WITHOUT. The same
        // source is lowered by the same front end into both forms and both are run through the
        // whole core lifecycle; what is compared is the VALUE, because the two forms account for
        // fuel differently by construction and a budget outcome would legitimately differ.
        var programs = new (string Name, string Text)[]
        {
            ("integer-loop", "let s = 0;\nfor (let i = 0; i < 1000; i = i + 1) { s = s + i; }\ns;\n"),
            ("nested-call-fibonacci",
                "function fib(n) { if (n < 2) { return n; } return fib(n - 1) + fib(n - 2); }\nfib(20);\n"),
            ("floating-point-kernel",
                "function step(x) { return x * 0.5 + 1.25; }\nlet v = 1;\nlet k = 0;\n" +
                "while (k < 40) { v = step(v); k = k + 1; }\nv;\n"),
            ("division-by-zero",
                "function f(a, b) { if (a < b) { return a / b; } return b / a; }\nlet z = 0;\nf(z, z);\n"),
        };

        foreach (var (name, text) in programs)
        {
            var interpreted = NativeLifecycle.Run(text, JsOutputForm.Bytecode, string.Empty);
            var emitted = NativeLifecycle.Run(text, JsOutputForm.Native, abi.Name);

            if (!string.Equals(interpreted, emitted, StringComparison.Ordinal))
            {
                return (
                    "native/differential/the-two-forms-agree",
                    false,
                    name + ": the interpreter answered " + interpreted + " and the emitted form " +
                        "answered " + emitted);
            }
        }

        return (
            "native/differential/the-two-forms-agree",
            true,
            programs.Length + " programs, each lowered by one front end into both forms and run " +
                "through the whole core lifecycle, answering the same value");
    }

    private static (string, bool, string) ReEmissionIsByteIdentical(JsX64Abi abi)
    {
        // Determinism now ranges over machine code, and re-emission equality is what that clause
        // buys. Emitting the same program twice must give the same bytes, or the verifier's
        // re-emission check would refuse artifacts that were correct when they were written.
        const string Text =
            "function fib(n) { if (n < 2) { return n; } return fib(n - 1) + fib(n - 2); }\nfib(8);\n";

        if (!TryEmit(abi, Text, out var first, out var firstSymbols, out var why) ||
            !TryEmit(abi, Text, out var second, out var secondSymbols, out why))
        {
            return ("native/backend/re-emission-is-byte-identical", false, why);
        }

        var same = first.AsSpan().SequenceEqual(second) &&
            firstSymbols.Length == secondSymbols.Length;

        for (var index = 0; same && index < firstSymbols.Length; index++)
        {
            same = firstSymbols[index] == secondSymbols[index];
        }

        return (
            "native/backend/re-emission-is-byte-identical",
            same,
            same
                ? first.Length + " bytes, emitted twice, identical"
                : "the same program emitted twice gave different bytes");
    }

    // ---- the machinery ------------------------------------------------------------------------

    /// <summary>
    /// What each observed register should hold when the call returns, under this convention.
    /// </summary>
    /// <remarks>
    /// <b>IT IS NOT SIMPLY THE SENTINEL TABLE, AND THE ONE ROW THAT DIFFERS IS THE ONE THE TWO
    /// CONVENTIONS DISAGREE ABOUT.</b> The trampoline loads the OTHER convention's argument
    /// register with a recognisable value, so that a callee reading its argument out of the wrong
    /// register answers with something a reader can name. Under Windows x64 that other register is
    /// RDI - which is also callee-saved there - so the value RDI is expected to come back holding
    /// is the recognisable one rather than RDI's own sentinel. It is still a witness: what the row
    /// asserts is that the register came back as it went in, and the value it went in with is what
    /// this method says.
    /// </remarks>
    private static long[] Expected(JsX64Abi abi)
    {
        var expected = (long[])Sentinels.Clone();
        var other = abi.FramePointerRegister == JsX64Register.Rcx
            ? JsX64Register.Rdi
            : JsX64Register.Rcx;

        for (var index = 0; index < Observed.Length; index++)
        {
            if (Observed[index] == other)
            {
                expected[index] = WrongRegisterSentinel;
            }
        }

        return expected;
    }

    /// <summary>Which callee-saved register of this convention came back changed, if any.</summary>
    private static JsX64Register? FirstClobbered(JsX64Abi abi, JsNativeAbiObservation seen)
    {
        if (!seen.Ran)
        {
            return null;
        }

        var expected = Expected(abi);

        for (var index = 0; index < Observed.Length; index++)
        {
            // RBP IS SKIPPED AND THE REASON IS STRUCTURAL: the trampoline uses it as its own frame
            // pointer so that it can survive a callee that shifted the stack, which is the very
            // injection the row above depends on. A register used to survive one defect cannot
            // also witness another.
            if (index == FramePointerSlot || !abi.IsCalleeSaved(Observed[index]))
            {
                continue;
            }

            if (seen.Saved[index] != expected[index])
            {
                return Observed[index];
            }
        }

        return null;
    }

    /// <summary>Maps the trampoline and the target together and runs the trampoline.</summary>
    private static JsNativeAbiObservation Observe(
        JsX64Abi abi, byte[] target, uint targetOffset, int repetitions, bool misalign = false)
    {
        var trampoline = Trampoline(abi, misalign);
        var padded = (trampoline.Length + 15) & ~15;
        var blob = new byte[padded + target.Length];
        System.Array.Copy(trampoline, blob, trampoline.Length);
        System.Array.Copy(target, 0, blob, padded, target.Length);

        return JsNativeAbi.Run(
            blob,
            0,
            (uint)padded + targetOffset,
            operandSlots: 4096,
            bindingSlots: 64,
            constants: new double[64],
            fuel: 1 << 20,
            repetitions: repetitions);
    }

    /// <summary>
    /// The trampoline: it loads sentinels, records the stack pointer either side of the call, and
    /// writes every callee-saved register back into the probe.
    /// </summary>
    private static byte[] Trampoline(JsX64Abi abi, bool misalign)
    {
        var assembler = new JsX64Assembler();
        var other = abi.FramePointerRegister == JsX64Register.Rcx
            ? JsX64Register.Rdi
            : JsX64Register.Rcx;

        foreach (var register in Observed)
        {
            assembler.Push(register);
        }

        // THE FRAME POINTER IS TAKEN BEFORE THE RESERVATION AND IS HOW THIS TRAMPOLINE SURVIVES A
        // CALLEE THAT MOVED THE STACK. Every other way of finding the way back is a read through
        // the stack pointer, which is the thing the injection breaks.
        assembler.MovRegisterRegister(JsX64Register.Rbp, JsX64Register.Rsp);

        // Eight pushes from an entry whose stack pointer is eight past a sixteen-byte boundary
        // leaves it eight past one again; forty more brings it to a boundary, which is where the
        // convention says a call is made from. Eight extra is the misalignment injection.
        var reservation = misalign ? 0x30 : 0x28;
        assembler.SubRegisterImmediate32(JsX64Register.Rsp, reservation);
        assembler.MovMemoryRegister(JsX64Register.Rsp, 0x20, abi.FramePointerRegister);

        for (var index = 0; index < Observed.Length; index++)
        {
            if (index == FramePointerSlot)
            {
                continue;
            }

            assembler.MovRegisterImmediate64(Observed[index], Sentinels[index]);
        }

        // The other convention's argument register gets a value nothing else can produce, so a
        // callee that read the argument out of it answers with something recognisable rather than
        // with whatever happened to be there.
        assembler.MovRegisterImmediate64(other, WrongRegisterSentinel);

        assembler.MovRegisterMemory(JsX64Register.Rax, JsX64Register.Rsp, 0x20);

        assembler.MovMemoryRegister(
            JsX64Register.Rax, JsNativeAbiProbeLayout.StackBeforeOffset, JsX64Register.Rsp);

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rax, JsNativeAbiProbeLayout.TargetOffset);

        assembler.MovRegisterMemory(
            abi.FramePointerRegister, JsX64Register.Rax, JsNativeAbiProbeLayout.FrameOffset);

        assembler.CallRegister(JsX64Register.R11);

        // The stack pointer as the callee left it, captured before anything restores it.
        assembler.MovRegisterRegister(JsX64Register.R11, JsX64Register.Rsp);
        assembler.LeaRegisterMemory(JsX64Register.Rsp, JsX64Register.Rbp, -reservation);
        assembler.MovRegisterMemory(JsX64Register.R10, JsX64Register.Rsp, 0x20);

        assembler.MovMemoryRegister(
            JsX64Register.R10, JsNativeAbiProbeLayout.StackAfterOffset, JsX64Register.R11);

        assembler.MovMemoryRegister(
            JsX64Register.R10, JsNativeAbiProbeLayout.AnswerOffset, JsX64Register.Rax);

        for (var index = 0; index < Observed.Length; index++)
        {
            assembler.MovMemoryRegister(
                JsX64Register.R10,
                JsNativeAbiProbeLayout.SavedOffset + (index * 8),
                Observed[index]);
        }

        assembler.MovRegisterRegister(JsX64Register.Rsp, JsX64Register.Rbp);

        for (var index = Observed.Length - 1; index >= 0; index--)
        {
            assembler.Pop(Observed[index]);
        }

        assembler.XorRegisterRegister(JsX64Register.Rax, JsX64Register.Rax);
        assembler.Ret();

        if (!assembler.TryFinish(out var code, out var refusal))
        {
            throw new InvalidOperationException("the trampoline did not assemble: " + refusal);
        }

        return code;
    }

    /// <summary>
    /// A target shaped like an emitted unit: it saves the one callee-saved register the backend
    /// uses, claims the frame the convention asks for, does what it was given, and returns.
    /// </summary>
    private static byte[] Conforming(
        JsX64Abi abi, Action<JsX64Assembler>? body = null, Action<JsX64Assembler>? tail = null)
    {
        var assembler = new JsX64Assembler();
        assembler.Push(JsX64Register.Rbx);
        assembler.SubRegisterImmediate32(JsX64Register.Rsp, abi.FrameBytes);

        assembler.MovMemoryRegister(
            JsX64Register.Rsp, abi.FramePointerSlot, abi.FramePointerRegister);

        assembler.XorRegisterRegister(JsX64Register.Rax, JsX64Register.Rax);
        body?.Invoke(assembler);
        assembler.AddRegisterImmediate32(JsX64Register.Rsp, abi.FrameBytes);
        assembler.Pop(JsX64Register.Rbx);

        if (tail is null)
        {
            assembler.Ret();
        }
        else
        {
            tail(assembler);
        }

        if (!assembler.TryFinish(out var code, out var refusal))
        {
            throw new InvalidOperationException("the target did not assemble: " + refusal);
        }

        return code;
    }

    /// <summary>Compiles one numeric source and reads the emitted bytes back out of the artifact.</summary>
    /// <remarks>
    /// <b>THE BYTES ARE READ OUT OF THE ARTIFACT AND NOT TAKEN FROM THE EMITTER.</b> What a row
    /// should call into is what an artifact actually carries; bytes taken straight from the emitter
    /// would check the emitter against itself with the whole write-and-read path removed, which is
    /// exactly the path a truncation or an off-by-one in the framing would live in.
    /// </remarks>
    private static bool TryEmit(
        JsX64Abi abi,
        string text,
        out byte[] code,
        out JsNativeSymbolRow[] symbols,
        out string refusal)
    {
        code = [];
        symbols = [];

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("script0", text, SliceParseOptions.Script)],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Native, abi.Name));

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            refusal = "the front end refused a program this row needs: " +
                (compiled.Diagnostics.Count == 0
                    ? "no diagnostic"
                    : compiled.Diagnostics[0].ToString());

            return false;
        }

        return NativeLifecycle.TryReadEmitted(compiled.Artifact, out code, out symbols, out refusal);
    }
}
