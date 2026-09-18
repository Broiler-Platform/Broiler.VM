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

    static NativeAbiChecks()
    {
        Program.InitializeNativeMapping();
    }

    /// <summary>Runs the obligation table.</summary>
    internal static List<(string Name, bool Passed, string Detail)> Run()
    {
        Program.InitializeNativeMapping();
        var checks = new List<(string, bool, string)>();
        var abi = JsX64Abi.Host;

        if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture !=
            System.Runtime.InteropServices.Architecture.X64)
        {
            // NOT A PASS, AND SINCE 2026-09-08 NOT A FAILURE EITHER. A row reported green on a
            // machine that cannot run the code it is about is the shape of evidence this
            // repository exists to refuse, and that is why this was written as a refusal. But a
            // refusal is the wrong word too: it made the lane on every non-x86-64 runtime
            // identifier permanently red for a machine doing nothing wrong, and a colour that can
            // never be green is a colour nobody reads. The `not-run/` prefix is the third verdict
            // the reporter grew for exactly this row - counted on its own, printed whether or not
            // verbosity was asked for, and folded into neither of the other two.
            //
            // WHAT IS TRUE HERE AND WHAT IS THEREFORE CLAIMED: this machine is not x86-64, the
            // obligation table was not run, and NOTHING in it is held. The boolean stays `false`
            // so that a reader who removed the prefix convention would get a red lane rather than
            // a silent green one.
            checks.Add((
                "not-run/native/abi/x86-64-obligation-table",
                false,
                "this machine is not x86-64, so no row of the x86-64 obligation table was run and " +
                "none of them is claimed - the arm64 encoder's golden rows below are unaffected, " +
                "because an encoding is checked by comparing bytes rather than by running them"));

            // THE BASELINE FORM'S EXECUTION ROWS ARE NOT RUN HERE EITHER, and each is named so that
            // its absence is counted row by row: the compile-and-scan rows of the baseline form run on
            // every machine beside the template tables, and these are the ones that enter the code.
            foreach (var row in BaselineExecutionRows)
            {
                checks.Add((
                    "not-run/native/baseline/" + row,
                    false,
                    "this machine is not x86-64, so no baseline emitted code was entered and nothing " +
                    "this row asserts is claimed"));
            }

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

        checks.AddRange(TheTwoFormsAgreeOverTheWideManifest(abi));
        checks.AddRange(TheSmallestCompletingAllowanceIsOneFigure(abi));

        foreach (var row in JsX64Abi.Rows)
        {
            checks.AddRange(BaselineEntryPointsSurvive(abi, row));
        }

        checks.Add(ASwappedHandlerNeverRuns(abi));
        checks.Add(AHandlerAtTheThrowersSuccessorContinuesTheStep(abi));
        checks.AddRange(FrequentCollections(abi));

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

    // ---- the baseline form over the wide manifest, entered ------------------------------------

    /// <summary>The baseline execution rows, named once so a machine that runs none of them names each.</summary>
    private static readonly string[] BaselineExecutionRows =
    [
        "two-forms-agree-over-the-wide-manifest",
        "the-smallest-completing-allowance-is-one-figure",
        "entry-points-survive/" + JsNativeBackends.X64Windows,
        "entry-points-survive/" + JsNativeBackends.X64SystemV,
        "a-misaligned-reservation-is-caught/" + JsNativeBackends.X64Windows,
        "a-misaligned-reservation-is-caught/" + JsNativeBackends.X64SystemV,
        "a-short-reservation-loses-a-saved-register/" + JsNativeBackends.X64Windows,
        "a-swapped-handler-never-runs",
        "a-handler-at-the-throwers-successor-continues-the-step",
        "frequent-collections",
    ];

    /// <summary>The fuel allowance a two-forms run is taken under: the deterministic lane's.</summary>
    private const ulong WideFuel = 100_000_000;

    /// <summary>How long the throw from the bottom of a deep recursion may take in the native form.</summary>
    private const long DeepThrowMilliseconds = 2_000;

    /// <summary>How many times each baseline entry point is entered.</summary>
    /// <remarks>
    /// <b>A THOUSAND AND NOT A MILLION, because every call here reaches the stub</b> and the stub
    /// writes the stack pointer it saw; what the figure has to do is make a drift of eight bytes per
    /// call eight kilobytes, which no comparison either side of the last call can miss, and a thousand
    /// recorded pointers that are all the same value say the same thing a million would.
    /// </remarks>
    private const int BaselineRepetitions = 1_000;

    /// <summary>
    /// The twenty wide programs the baseline closure rows compile and scan, entered here.
    /// </summary>
    /// <remarks>
    /// <b>THESE ARE THE SAME TWENTY SOURCES AS <c>NativeTemplateScanChecks.WidePrograms</c>, COPIED
    /// AND NOT SHARED</b>, because that list is private to the file that owns the template rows and this
    /// file does not own it. A program edited there and not here narrows nothing either file claims:
    /// the scan rows still scan theirs and these rows still run these, and each list names the landing
    /// or tail it exists to reach.
    /// </remarks>
    private static readonly (string Name, string Source, string? Library)[] WidePrograms =
    [
        ("an object literal and a property read", "var o = { a: 1, b: { c: 2 } }; o.b.c + o.a;", null),
        ("string concatenation", "var s = 'a'; for (var i = 0; i < 3; i++) { s = s + i; } s + '!';", null),
        ("a closure counter", "function counter() { var n = 0; return function () { n = n + 1; return n; }; } var c = counter(); c(); c();", null),
        ("a class with super and private members", "class A { #x = 1; get x() { return this.#x; } m() { return 2; } } class B extends A { #y = 3; constructor() { super(); } m() { return super.m() + this.#y + this.x; } } new B().m();", null),
        ("try, catch and finally with a break through the finally", "var log = ''; for (var i = 0; i < 3; i++) { try { try { if (i === 1) { break; } throw new Error('e' + i); } catch (e) { log += e.message; } finally { log += 'f'; } } finally { log += 'g'; } } log;", null),
        ("a generator resumed by return() and throw() past a leaf of the tree", "function* g() { try { yield 1; yield 2; yield 3; yield 4; yield 5; yield 6; yield 7; yield 8; yield 9; yield 10; } finally { yield 11; } } var a = g(); a.next(); a.return(5); var b = g(); b.next(); try { b.throw(new Error('x')); } catch (e) { } a.next().done;", null),
        ("yield* over a generator and over an array", "function* inner() { yield 1; yield 2; } function* outer() { yield* inner(); yield* [3, 4]; } var t = 0; for (var v of outer()) { t += v; } t;", null),
        ("async and await with a rejected await", "async function f() { try { await Promise.reject(new Error('no')); } catch (e) { return e.message; } } var r; f().then(function (v) { r = v; }); r;", null),
        ("an async generator with for await and yield* into an async iterator", "async function* ag() { yield 1; yield* (async function* () { yield 2; })(); } async function run() { var t = 0; for await (var v of ag()) { t += v; } return t; } run();", null),
        ("for-in", "var o = { a: 1, b: 2 }; var k = ''; for (var p in o) { k += p; } k;", null),
        ("for-of with an early exit", "var t = 0; for (var v of [1, 2, 3, 4]) { if (v > 2) { break; } t += v; } t;", null),
        ("destructuring parameters with defaults", "function f({ a = 1, b } = {}, [c, d = 4] = []) { return a + (b || 0) + (c || 0) + d; } f() + f({ b: 2 }, [3]);", null),
        ("a module with an import", "import { add, n } from 'lib'; export const r = add(n, 2);", "export function add(a, b) { return a + b; } export let n = 1;"),
        ("a switch, a labelled continue and with", "var t = 0; outer: for (var i = 0; i < 3; i++) { for (var j = 0; j < 3; j++) { if (j === 1) { continue outer; } switch (i) { case 0: t += 1; break; case 1: t += 10; break; default: t += 100; } } } var o = { x: 5 }; with (o) { t += x; } t;", null),
        ("a long linear run with a call in its middle", "var a = 1, b = 2, c = 3; function id(x) { return x; } var t = 0; for (var i = 0; i < 50; i++) { t = t + a * b - c + id(i) + (a << 2) + (b | c); } t;", null),
        ("a loop head inside a linear run", "var t = 0, i = 0; do { t += i; t *= 2; t -= 1; i++; } while (i < 5); t;", null),
        ("getter and valueOf re-entry from the middle of a run", "var n = 0; var o = { get g() { n += 1; return n; }, valueOf() { return n * 2; } }; var t = 0; for (var i = 0; i < 5; i++) { t = t + o.g + (+o) + o.g; } t;", null),
        ("Proxy traps from the middle of a run", "var log = 0; var p = new Proxy({ a: 1 }, { get(t, k) { log += 1; return t[k]; } }); var s = 0; for (var i = 0; i < 4; i++) { s = s + p.a * 2 + p.a; } s + log;", null),
        ("a throw from the middle of a run caught in the same unit", "var r = 0; for (var i = 0; i < 3; i++) { try { r = r + 1; r += null.x; r = r + 100; } catch (e) { r = r + 10; } } r;", null),
        ("a generator resumed into the middle of a run", "function* g() { var x = 1; x = x + (yield x); x = x * 2 + (yield x); return x + 3; } var it = g(); it.next(); it.next(5); it.next(7).value;", null),
    ];

    /// <summary>
    /// The wide programs whose smallest completing allowance is bisected: every one that loads nothing - no
    /// eval, no Function, no import and no library.
    /// </summary>
    /// <remarks>
    /// <b>EVERY INDEX BUT TWELVE</b>, which is the module that imports its library. A program added to
    /// <see cref="WidePrograms"/> that loads nothing is added here by its index too.
    /// </remarks>
    private static readonly int[] Bisected = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19];

    /// <summary>
    /// The probes whose smallest completing allowance is bisected: every one that loads nothing, which is every
    /// index but five, the eval.
    /// </summary>
    private static readonly int[] BisectedProbes = [0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12];

    /// <summary>The probes each aimed at one way the native form's machine stack or unwinding could differ.</summary>
    /// <remarks>
    /// <b>A PROBE THAT RECURSES TO THE ENGINE'S BOUND SAYS SO</b>, and its row then also requires the
    /// interpreter to have answered the <c>RangeError</c> it caught: a recursion that answered anything else
    /// compared two runs that never reached what the probe is about. The recursions after the first nest
    /// through a getter, <c>valueOf</c>, a Proxy trap, <c>yield*</c>, the rendering of a thrown object and an
    /// indexed getter - each re-entering guest code from an instruction that may be inside a block, rather
    /// than from a call instruction.
    /// </remarks>
    private static readonly (string Name, string Source, bool GuestLoads, bool RecursesToRangeError)[] Probes =
    [
        ("recursion to RangeError at the engine's maximum call depth", "var depth = 0; function down() { depth = depth + 1; down(); } var caught = 'nothing'; try { down(); } catch (e) { caught = e.name + ' at depth ' + depth; } caught;", false, true),
        ("a throw from depth 5000 caught at the top", "function sink(n) { if (n === 0) { throw new Error('from the bottom'); } return sink(n - 1) + 1; } var got = 'nothing'; try { sink(5000); } catch (e) { got = e.message; } got;", false, false),
        ("a 100000-iteration throw and catch across a call", "function thrower(i) { throw i; } var caught = 0; for (var i = 0; i < 100000; i++) { try { thrower(i); } catch (e) { caught += 1; } } caught;", false, false),
        ("a generator's return() through two finallys", "var log = ''; function* g() { try { try { yield 1; } finally { log += 'inner;'; } } finally { log += 'outer;'; } } var it = g(); it.next(); var r = it.return(42); log + r.value + ':' + r.done;", false, false),
        ("an uncaught TypeError", "var nothing = null; nothing.property;", false, false),
        ("an eval of a function that is called later", "eval('function later(x) { return x * 3; }'); var answer = later(14); answer;", true, false),
        ("recursion to RangeError through a getter", "var d = 0; var o = { get down() { d = d + 1; return this.down; } }; var caught = 'nothing'; try { o.down; } catch (e) { caught = e.name + ' at depth ' + d; } caught;", false, true),
        ("recursion to RangeError through valueOf", "var d = 0; var o = { valueOf: function () { d = d + 1; return +o; } }; var caught = 'nothing'; try { +o; } catch (e) { caught = e.name + ' at depth ' + d; } caught;", false, true),
        ("recursion to RangeError through a Proxy get trap", "var d = 0; var p = new Proxy({}, { get: function () { d = d + 1; return p.x; } }); var caught = 'nothing'; try { p.x; } catch (e) { caught = e.name + ' at depth ' + d; } caught;", false, true),
        ("recursion to RangeError through yield*", "var d = 0; function* g() { d = d + 1; yield* g(); } var caught = 'nothing'; try { g().next(); } catch (e) { caught = e.name + ' at depth ' + d; } caught;", false, true),
        ("a throw from a getter caught in the same run", "var o = { get bad() { throw new Error('g'); } }; var r = ''; for (var i = 0; i < 3; i++) { try { r = r + i + o.bad; } catch (e) { r = r + e.message; } } r;", false, false),
        ("recursion to RangeError through a thrown object's rendered message", "var d = 0; var e = { get message() { d = d + 1; try { throw e; } catch (x) { if (x !== e) { throw x; } } return 'm'; } }; var caught = 'nothing'; try { try { throw e; } catch (x) { if (x !== e) { throw x; } } } catch (f) { caught = f.name + ' at depth ' + d; } caught;", false, true),
        ("recursion to RangeError through an indexed getter", "var d = 0; var k = 'down'; var o = { get down() { d = d + 1; return this[k]; } }; var caught = 'nothing'; try { o[k]; } catch (e) { caught = e.name + ' at depth ' + d; } caught;", false, true),
    ];

    /// <summary>
    /// The probes again, each printing often enough from inside the activations it exists for that a
    /// collection forced every thousand prints happens while emitted frames are on the stack.
    /// </summary>
    private static readonly (string Name, string Source, bool GuestLoads)[] Collecting =
    [
        ("printing at every level of a 3000-deep recursion", "function down(n) { var o = { n: n, s: 'v' + n }; print(o.s); if (n === 0) { return 0; } return down(n - 1) + o.n; } down(3000);", false),
        ("printing from a generator between yields and in its finally", "function* g() { try { for (var i = 0; i < 2500; i++) { print('y' + i); yield { i: i }; } } finally { print('closed'); } } var t = 0; for (var v of g()) { t += v.i; if (t > 2000000) { break; } } t;", false),
        ("printing from awaited continuations drained as jobs", "async function step(i) { await null; print('a' + i); return { i: i }; } async function run() { var t = 0; for (var i = 0; i < 2500; i++) { t += (await step(i)).i; } print('total ' + t); return t; } run();", false),
        ("printing from catches of throws across a call", "function thrower(i) { throw { i: i }; } var c = 0; for (var i = 0; i < 2500; i++) { try { thrower(i); } catch (e) { print('c' + e.i); c += e.i; } } c;", false),
        ("printing from a function an eval defined", "eval('function later(x) { print(\"l\" + x); return [x, x * 2]; }'); var t = 0; for (var i = 0; i < 2500; i++) { t += later(i)[1]; } t;", true),
        ("printing while closures and strings accumulate", "var fs = []; for (var i = 0; i < 3000; i++) { (function (k) { fs.push(function () { return 'k' + k; }); })(i); print(i); } var s = 0; for (var j = 0; j < fs.length; j++) { s += fs[j]().length; } s;", false),
        ("printing from a getter at every level of a 1500-deep getter recursion", "var n = 1500; var o = { get down() { var s = { n: n }; print('g' + n); if (n === 0) { return 0; } n = n - 1; return this.down + s.n; } }; o.down;", false),
    ];

    /// <summary>Every wide program and every probe, each run in both forms and compared.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE WHOLE ANSWER IS COMPARED, FUEL INCLUDED</b> - which the numeric form's row cannot do,
    /// because that form accounts for fuel differently by construction. The baseline form charges at
    /// the same point of the same method as the interpreter, so a fuel figure that differs is a
    /// handler that ran an instruction the interpreter did not, or skipped one it ran.
    /// </para>
    /// <para>
    /// <b>ONE EXCEPTION, AND IT IS THE ONE THE DECISION RECORD NAMES.</b> A program that loads code
    /// verifies a payload whose native half is larger, and verification work is charged to fuel, so
    /// the eval probe's fuel figure is reported and not compared.
    /// </para>
    /// </remarks>
    private static List<(string, bool, string)> TheTwoFormsAgreeOverTheWideManifest(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();
        const string Prefix = "native/baseline/two-forms-agree-over-the-wide-manifest/";

        foreach (var (name, source, library) in WidePrograms)
        {
            rows.Add(Agree(Prefix + name, abi, source, library, guestLoads: false));
        }

        for (var index = 0; index < Probes.Length; index++)
        {
            var (name, source, guestLoads, recursesToRangeError) = Probes[index];

            if (index == 1)
            {
                rows.Add(Agree(
                    Prefix + name,
                    abi,
                    source,
                    null,
                    guestLoads,
                    also: (_, _, native) =>
                        native.TotalMilliseconds < DeepThrowMilliseconds
                            ? null
                            : "the native form took " + (long)native.TotalMilliseconds + " ms, more than " + DeepThrowMilliseconds));

                continue;
            }

            rows.Add(recursesToRangeError
                ? Agree(
                    Prefix + name,
                    abi,
                    source,
                    null,
                    guestLoads,
                    also: (interpreted, _, _) =>
                        interpreted.Completion.StartsWith("RangeError at depth ", StringComparison.Ordinal)
                            ? null
                            : "the interpreter did not answer a RangeError it caught, so the row compares nothing it is about")
                : Agree(Prefix + name, abi, source, null, guestLoads));
        }

        return rows;
    }

    /// <summary>
    /// The smallest fuel ceiling at which a program runs to its end is one figure in both forms.
    /// </summary>
    /// <remarks>
    /// <b>THIS IS THE STRONGER HALF OF FUEL PARITY.</b> Equal consumption at completion says the two
    /// forms charged the same total; equal smallest ceilings say the allowance ran out on the same
    /// instruction when it was one short - which is what a guest can observe, and what a handler that
    /// charged before a check the interpreter makes first would change. Every wide program and every
    /// probe that loads no code is bisected, for the reason the two-forms rows give; a step that runs a
    /// block charges each of its instructions where the interpreter does, so an allowance one short runs
    /// out inside a block on the instruction it runs out on in the interpreter.
    /// </remarks>
    private static List<(string, bool, string)> TheSmallestCompletingAllowanceIsOneFigure(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();
        var bisected = new List<(string Name, string Source, string? Library)>();

        foreach (var index in Bisected)
        {
            bisected.Add(WidePrograms[index]);
        }

        foreach (var index in BisectedProbes)
        {
            bisected.Add((Probes[index].Name, Probes[index].Source, null));
        }

        foreach (var (name, source, library) in bisected)
        {
            var label = "native/baseline/the-smallest-completing-allowance-is-one-figure/" + name;
            var interpreted = SmallestCompletingAllowance(source, library, JsOutputForm.Bytecode, string.Empty);
            var emitted = SmallestCompletingAllowance(source, library, JsOutputForm.Native, abi.Name);

            rows.Add((
                label,
                interpreted.Figure != 0 && interpreted.Figure == emitted.Figure,
                "bytecode " + interpreted.Why + "; native " + emitted.Why));
        }

        return rows;
    }

    /// <summary>Bisects the smallest fuel ceiling at which one program completes in one form.</summary>
    private static (ulong Figure, string Why) SmallestCompletingAllowance(
        string source, string? library, JsOutputForm form, string backend)
    {
        var compiled = NativeLifecycle.CompileWide(source, form, backend, library);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (0, "did not compile");
        }

        if (form == JsOutputForm.Native && EmittedCodeMissing(compiled.Artifact) is { } missing)
        {
            return (0, missing);
        }

        NativeLifecycle.WideAnswer Attempt(ulong fuel) =>
            NativeLifecycle.RunWideArtifact(
                compiled.Artifact, form, backend, fuel, reEmit: form == JsOutputForm.Native, module: library is not null);

        var full = Attempt(WideFuel);

        if (!full.Completed)
        {
            return (0, "did not complete under " + WideFuel + ": " + full.Render(withFuel: true));
        }

        // The consumption at completion is the first guess at the ceiling, and it is only a guess: a
        // ceiling is checked when a charge is made, so the smallest completing one is searched for
        // rather than read off.
        var completing = full.Fuel != 0 && Attempt(full.Fuel).Completed ? full.Fuel : WideFuel;
        var failing = 0UL;
        var attempts = 0;

        while (completing - failing > 1)
        {
            var middle = failing + ((completing - failing) / 2);
            attempts++;

            if (Attempt(middle).Completed)
            {
                completing = middle;
            }
            else
            {
                failing = middle;
            }
        }

        return (
            completing,
            "completes at " + completing + " and not at " + failing + " (" + attempts +
                " attempts; consumption at completion " + full.Fuel + ")");
    }

    /// <summary>
    /// A baseline unit entered through a probe frame whose handler table points every slot at a stub
    /// that records the stack pointer it was called with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE UNIT IS THE BACKEND'S AND THE HANDLER IS NOT.</b> Every slot of the probe table is the same
    /// check-only stub, so the first head the unit dispatches calls it, it answers exit, and the
    /// unit leaves - which exercises the prologue, the dispatch compare, one handler call and the
    /// epilogue, and nothing of the interpreter. What the rows establish is what the numeric rows
    /// above establish for their frame: the stack pointer at the handler's entry is eight past a
    /// sixteen-byte boundary, the unit leaves it where it found it, and every register the convention
    /// makes callee-saved comes back.
    /// </para>
    /// <para>
    /// <b>UNDER WINDOWS x64 THE STUB ALSO WRITES THE LAST SLOT OF ITS SHADOW SPACE</b>, which a callee
    /// may, so a unit that reserved less than the convention asks loses a saved register or its return
    /// address to it. Only the first of those is a row: a write onto the return address sends the unit
    /// back into the stack and takes the process down, and that is what the likeliest short
    /// reservation - System V's eight, emitted under Windows - would do. Two injections sit beside the
    /// row. One reserves eight bytes MORE and requires every recorded pointer to be caught misaligned.
    /// The other reserves sixteen bytes LESS, which keeps the call aligned and lands the stub's write on
    /// the saved r14 rather than the return address, and requires R14 to be named as the register that
    /// came back changed - the witness that the shadow-space half of the stub can fail at all.
    /// </para>
    /// <para>
    /// <b>Only the convention this machine uses is entered.</b> The other convention's unit is bytes
    /// this process has no business calling; its closure, golden and cross-convention rows compare
    /// bytes on every machine, and its execution belongs to a lane whose machine uses it.
    /// </para>
    /// </remarks>
    private static List<(string, bool, string)> BaselineEntryPointsSurvive(JsX64Abi host, JsX64Abi row)
    {
        var name = "native/baseline/entry-points-survive/" + row.Name;
        var injection = "native/baseline/a-misaligned-reservation-is-caught/" + row.Name;

        // THE SHORT-RESERVATION INJECTION EXISTS ONLY WHERE THE STUB HAS A SHADOW SPACE TO WRITE.
        // Under System V the stub writes nothing above its return address, so a short reservation
        // loses nothing a row could name, and no row by this name is claimed or counted there.
        var shortName = row.ShadowSpaceBytes != 0
            ? "native/baseline/a-short-reservation-loses-a-saved-register/" + row.Name
            : null;

        if (row.Architecture != host.Architecture)
        {
            const string Why =
                "this machine uses {0}, so a unit emitted for {1} was not entered; its bytes are held " +
                "by the baseline closure, golden and cross-convention rows on every machine";

            var because = string.Format(System.Globalization.CultureInfo.InvariantCulture, Why, host.Name, row.Name);
            var notRun = new List<(string, bool, string)>
            {
                ("not-run/" + name, false, because),
                ("not-run/" + injection, false, because),
            };

            if (shortName is not null)
            {
                notRun.Add(("not-run/" + shortName, false, because));
            }

            return notRun;
        }

        var compiled = NativeLifecycle.CompileWide(
            "function f() { return 1; }\nf();\n", JsOutputForm.Native, row.Name);

        List<(string, bool, string)> AllFail(string why)
        {
            var failed = new List<(string, bool, string)> { (name, false, why), (injection, false, why) };

            if (shortName is not null)
            {
                failed.Add((shortName, false, why));
            }

            return failed;
        }

        if (compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out var code, out var symbols, out var refusal))
        {
            return AllFail(compiled.Artifact is null ? "the program did not compile" : "the artifact carried no emitted code");
        }

        // NOT LEFT TO THE READER'S CONTRACT: every verdict below starts true and is only ever made
        // false inside the loop over units, so a table with no unit in it would pass every row
        // having entered nothing.
        if (symbols.Length == 0)
        {
            return AllFail("the artifact's symbol table names no unit, so nothing would have been entered");
        }

        var survived = true;
        var survivedDetail = new List<string>();
        var caught = true;
        var caughtDetail = new List<string>();
        var lost = true;
        var lostDetail = new List<string>();

        foreach (var symbol in symbols)
        {
            var at = (int)symbol.Offset;

            // THE ENTRY PROGRAM COUNTER IS READ OFF THE UNIT'S FIRST DISPATCH COMPARE, which compares
            // against a landing: the prologue is fifteen bytes and the dispatch head eight, so the
            // twenty-fourth byte is `cmp eax, imm32`. Any landing is a place the unit may be entered.
            if (at + 28 > code.Length || code[at + 23] != 0x3D)
            {
                survived = false;
                caught = false;
                lost = false;
                survivedDetail.Add("unit " + symbol.FunctionIndex + " does not begin with a prologue and a dispatch compare");
                continue;
            }

            var entryPc = BitConverter.ToInt32(code, at + 24);
            var seen = EnterBaseline(row, code, symbol.Offset, entryPc, out var stacks);
            var misaligned = Array.FindIndex(stacks, stack => stack == 0 || (stack % 16) != 8);
            var drifted = Array.FindIndex(stacks, stack => stack != stacks[0]);
            var clobbered = FirstClobbered(row, seen);

            if (!seen.Ran || (int)seen.Answer != (int)JsBaselineStatus.Exit ||
                seen.StackBefore != seen.StackAfter || misaligned >= 0 || drifted >= 0 || clobbered is not null)
            {
                survived = false;
                survivedDetail.Add(
                    "unit " + symbol.FunctionIndex + " entered at pc " + entryPc + ": " +
                    (!seen.Ran
                        ? "the mapping could not be made or armed"
                        : "answered " + (int)seen.Answer +
                            ", stack pointer moved by " + (seen.StackAfter - seen.StackBefore) +
                            (misaligned >= 0 ? ", call " + misaligned + " reached the handler with 0x" + stacks[misaligned].ToString("x") : string.Empty) +
                            (drifted >= 0 ? ", call " + drifted + " reached it at a different stack pointer than the first" : string.Empty) +
                            (clobbered is null ? string.Empty : ", " + clobbered + " came back changed")));
            }
            else
            {
                survivedDetail.Add(
                    "unit " + symbol.FunctionIndex + " entered at pc " + entryPc + " reached its handler at 0x" +
                    stacks[0].ToString("x"));
            }

            // THE INJECTION: eight more bytes reserved and released, so the frame still balances and
            // every handler call is made from a pointer eight bytes off the boundary.
            var injected = (byte[])code.Clone();

            if (!TryResizeReservation(injected, at, row.BaselineFrameBytes, row.BaselineFrameBytes + 8))
            {
                caught = false;
                caughtDetail.Add("unit " + symbol.FunctionIndex + ": no reservation and release to widen");
            }
            else
            {
                var wrong = EnterBaseline(row, injected, symbol.Offset, entryPc, out var wrongStacks);
                var detected = wrong.Ran && wrong.StackBefore == wrong.StackAfter &&
                    Array.TrueForAll(wrongStacks, stack => stack != 0 && (stack % 16) != 8);

                caught &= detected;
                caughtDetail.Add(
                    "unit " + symbol.FunctionIndex + (detected ? " widened by eight reached its handler at 0x" : " widened by eight was NOT caught, first stack 0x") +
                    (wrongStacks.Length == 0 ? "0" : wrongStacks[0].ToString("x")));
            }

            if (shortName is null)
            {
                continue;
            }

            // THE SHORT INJECTION: sixteen bytes fewer reserved and released. Sixteen and not eight,
            // because a reservation eight short is also eight off the boundary and the stub's
            // shadow-space write would land on the unit's own return address - a process-level access
            // violation and not a failing row. Sixteen short keeps the handler call aligned and the
            // return address out of reach, and moves the stub's write down onto the word the prologue
            // pushed second: with E the stack pointer on entry, rbx is at E-8 and r14 at E-16, the
            // reservation leaves E-16-(frame-16), the call E-8-that, and the stub writes thirty-two
            // above that, which is E-16. The unit's frame pointer is what it writes, so r14 comes back
            // holding the frame rather than the caller's value, and the row requires R14 to be named.
            var shortened = (byte[])code.Clone();

            if (!TryResizeReservation(shortened, at, row.BaselineFrameBytes, row.BaselineFrameBytes - 16))
            {
                lost = false;
                lostDetail.Add("unit " + symbol.FunctionIndex + ": no reservation and release to shorten");
                continue;
            }

            var shortSeen = EnterBaseline(row, shortened, symbol.Offset, entryPc, out var shortStacks);
            var shortClobbered = FirstClobbered(row, shortSeen);
            var shortAligned = Array.TrueForAll(shortStacks, stack => stack != 0 && (stack % 16) == 8);
            var named = shortSeen.Ran && shortSeen.StackBefore == shortSeen.StackAfter && shortAligned &&
                shortClobbered == JsX64Register.R14;

            lost &= named;
            lostDetail.Add(
                "unit " + symbol.FunctionIndex + " shortened by sixteen " +
                (named
                    ? "came back with R14 changed and its stack aligned"
                    : "was NOT caught as losing R14: ran " + shortSeen.Ran + ", aligned " + shortAligned +
                        ", stack pointer moved by " + (shortSeen.StackAfter - shortSeen.StackBefore) +
                        ", first changed register " + (shortClobbered?.ToString() ?? "none")));
        }

        var rows = new List<(string, bool, string)>
        {
            (name, survived, symbols.Length + " units, each entered " + BaselineRepetitions + " times: " + string.Join("; ", survivedDetail)),
            (injection, caught, string.Join("; ", caughtDetail)),
        };

        if (shortName is not null)
        {
            rows.Add((shortName, lost, string.Join("; ", lostDetail)));
        }

        return rows;
    }

    /// <summary>Changes a baseline unit's reservation and its release from one size to another, in place.</summary>
    private static bool TryResizeReservation(byte[] code, int unit, int frameBytes, int bytes)
    {
        byte[] reserve = [0x48, 0x83, 0xEC, (byte)frameBytes];
        byte[] release = [0x48, 0x83, 0xC4, (byte)frameBytes, 0x41, 0x5E, 0x5B, 0xC3];

        if (bytes <= 0 || bytes > sbyte.MaxValue ||
            !code.AsSpan(unit + 3, reserve.Length).SequenceEqual(reserve))
        {
            return false;
        }

        var leave = code.AsSpan(unit).IndexOf(release);

        if (leave < 0)
        {
            return false;
        }

        code[unit + 6] = (byte)bytes;
        code[unit + leave + 3] = (byte)bytes;
        return true;
    }

    /// <summary>Maps a trampoline, the emitted code and the stub together and enters one unit.</summary>
    private static JsNativeAbiObservation EnterBaseline(
        JsX64Abi abi, byte[] code, uint unitOffset, int entryPc, out long[] stacks)
    {
        var trampoline = Trampoline(abi, misalign: false, entryPc);
        var stub = BaselineStub(abi);
        var codeAt = (trampoline.Length + 15) & ~15;
        var stubAt = codeAt + ((code.Length + 15) & ~15);
        var blob = new byte[stubAt + stub.Length];

        Array.Copy(trampoline, blob, trampoline.Length);
        Array.Copy(code, 0, blob, codeAt, code.Length);
        Array.Copy(stub, 0, blob, stubAt, stub.Length);

        stacks = new long[BaselineRepetitions];

        return JsNativeAbi.RunBaseline(
            blob, 0, (uint)codeAt + unitOffset, (uint)stubAt, stacks);
    }

    /// <summary>
    /// The check-only handler: it records the stack pointer in the probe frame's scratch word and
    /// answers exit.
    /// </summary>
    /// <remarks>
    /// <b>BUILT WITH THE BACKEND'S ENCODER, FOR THE REASON THE TRAMPOLINE IS.</b> The Windows x64 stub
    /// also stores its frame argument in the fourth slot of its shadow space - thirty-two bytes above
    /// its return address, which a callee owns - so a unit whose reservation did not hold the shadow
    /// space has a saved register or its return address overwritten. A saved register is what a row
    /// sees, and the sixteen-short injection is the row that sees it; a return address overwritten is
    /// an access violation that ends the process, which is why no injection reserves eight short.
    /// </remarks>
    private static byte[] BaselineStub(JsX64Abi abi)
    {
        var assembler = new JsX64Assembler();
        assembler.MovMemoryRegister(abi.FramePointerRegister, JsBaselineAbi.FrameSize, JsX64Register.Rsp);

        if (abi.ShadowSpaceBytes != 0)
        {
            assembler.MovMemoryRegister(JsX64Register.Rsp, abi.ShadowSpaceBytes, abi.FramePointerRegister);
        }

        assembler.MovEaxImm32((int)JsBaselineStatus.Exit);
        assembler.Ret();

        if (!assembler.TryFinish(out var code, out var refusal))
        {
            throw new InvalidOperationException("the baseline stub did not assemble: " + refusal);
        }

        return code;
    }

    /// <summary>
    /// A payload whose first handler call was moved to another defined opcode's slot is refused at
    /// verification by the admitting door, so it never runs; the unswapped control through the same door
    /// answers a value.
    /// </summary>
    /// <remarks>
    /// <b>THE OTHER HALF OF THE SCAN ROW THAT SHOWS EVERY VERIFIER REFUSING THE SAME SWAP.</b> The admitting
    /// door has no emitter, so nothing re-emits the payload; what refuses it is the template scan, whose
    /// layout clauses name the head's own opcode where the call's slot is, and the run answers the verifier's
    /// refusal with the scan's code. The unswapped control through the same door answers a value, so the
    /// refusal is not a door that refuses everything. The handler's own check that the byte at the program
    /// counter it was handed is its opcode is no longer reached by this payload: a build with the layout
    /// clause's slot comparison removed is what observes it firing.
    /// </remarks>
    private static (string, bool, string) ASwappedHandlerNeverRuns(JsX64Abi abi)
    {
        const string Name = "native/baseline/a-swapped-handler-never-runs";
        var compiled = NativeLifecycle.CompileWide(WidePrograms[0].Source, JsOutputForm.Native, abi.Name);

        if (compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out var code, out _, out _))
        {
            return (Name, false, "the program did not emit");
        }

        var artifact = (byte[])compiled.Artifact.Clone();
        var blob = artifact.AsSpan().IndexOf(code.AsSpan(0, Math.Min(64, code.Length)));
        var swapped = -1;
        var from = 0;
        var to = 0;

        for (var at = 0; blob >= 0 && at + 6 <= code.Length; at++)
        {
            if (code[at] != 0xFF || code[at + 1] != 0x93)
            {
                continue;
            }

            from = BitConverter.ToInt32(code, at + 2);
            to = from == (int)JsOpcode.LoadNull * 8 ? (int)JsOpcode.LoadTrue * 8 : (int)JsOpcode.LoadNull * 8;
            BitConverter.TryWriteBytes(artifact.AsSpan(blob + at + 2), to);
            swapped = at;
            break;
        }

        if (swapped < 0)
        {
            return (Name, false, "the emission carries no handler call to swap");
        }

        var control = NativeLifecycle.RunWideArtifact(
            compiled.Artifact, JsOutputForm.Native, abi.Name, WideFuel, reEmit: false, module: false);

        var run = NativeLifecycle.RunWideArtifact(
            artifact, JsOutputForm.Native, abi.Name, WideFuel, reEmit: false, module: false);

        // A verification refusal reaches the run as the verifier's outcome, reason and profile code.
        var expected = "the verifier refused: " + VmOutcome.InvalidArtifact + "/" + VmReason.InconsistentStructure +
            " code " + (int)JavaScriptDiagnosticCode.NativePayloadNotTemplateClosed;

        return (
            Name,
            control.Completed && control.Completion.Length != 0 &&
                string.Equals(run.Outcome, expected, StringComparison.Ordinal) && run.Completion.Length == 0,
            "the call at " + swapped + " moved from slot " + (from / 8) + " to slot " + (to / 8) +
                ": the unswapped control answered " + control.Render(withFuel: false) +
                "; the swapped payload answered " + run.Render(withFuel: false));
    }

    /// <summary>
    /// A catch region whose handler is the successor of the instruction that throws, written by hand, verifies
    /// and answers the same in both forms, with the throw and the fall-through meeting inside one block.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE PATH A BLOCK STEP TAKES WHEN A THROW LANDS ON THE INSTRUCTION IT WOULD HAVE RUN NEXT ANYWAY.</b> A
    /// step stops on a program counter that is not its next instruction, and a caught throw usually lands on
    /// one; here the handler is the thrower's successor, so the landed program counter is the next
    /// instruction, no stop rule holds, and the step goes on into the handler inside the block it was running.
    /// Either outcome is the interpreter's, since a step that stopped there would be dispatched to the same
    /// landing, so what the row can observe is that the two forms answer alike, fuel included, and not
    /// whether the native run dispatched again. Nothing outside the engine counts steps.
    /// </para>
    /// <para>
    /// <b>IT IS WRITTEN BY HAND BECAUSE THE LOWERING NEVER WRITES IT</b>: a lowered try body ends in a jump
    /// past its handler, so its last instruction ends a block and no handler is the successor of a throwing
    /// instruction. Unit 1 is <c>f(x)</c>: <c>16 LoadArgument 0; 19 GetProperty "a"; 22 TypeOf; 23
    /// Return</c>, with a catch region over [16, 22) whose handler is 22 at the height the fall-through
    /// reaches 22 with, so the thrown value and the property read are one operand the handler reads either
    /// way. Unit 0, the program body, answers <c>f(null) + f({})</c>: the first call throws a TypeError and
    /// lands, the second falls through, and the answer <c>objectundefined</c> says each took its own path.
    /// </para>
    /// <para>
    /// <b>The plan is read too</b>: the handler is a landing, the thrower ends no block, and the block whose
    /// run holds the thrower runs on to the handler or past it.
    /// </para>
    /// </remarks>
    private static (string, bool, string) AHandlerAtTheThrowersSuccessorContinuesTheStep(JsX64Abi abi)
    {
        const string Name = "native/baseline/a-handler-at-the-throwers-successor-continues-the-step";
        const int Callee = 16;
        const int Thrower = Callee + 3;
        const int Handler = Thrower + 3;
        const string Expected = "objectundefined";

        byte[] code =
        [
            (byte)JsOpcode.Closure, 1, 0,
            (byte)JsOpcode.LoadUndefined,
            (byte)JsOpcode.LoadNull,
            (byte)JsOpcode.Call, 1,
            (byte)JsOpcode.Closure, 1, 0,
            (byte)JsOpcode.LoadUndefined,
            (byte)JsOpcode.NewObject,
            (byte)JsOpcode.Call, 1,
            (byte)JsOpcode.Add,
            (byte)JsOpcode.Return,
            (byte)JsOpcode.LoadArgument, 0, 0,
            (byte)JsOpcode.GetProperty, 0, 0,
            (byte)JsOpcode.TypeOf,
            (byte)JsOpcode.Return,
        ];

        JsFunctionRow[] functions =
        [
            new(0, 0, 1, 4, 0, Callee, (uint)JsFormat.FunctionFlags.ProgramBody),
            new(0, 1, 1, 1, Callee, (uint)(code.Length - Callee), (uint)JsFormat.FunctionFlags.None),
        ];

        JsExceptionRegionRow[] regions =
        [
            new(1, Callee, Handler, Handler, 0, 0, JsFormat.HandlerKind.Catch),
        ];

        byte[][] constants = [JsArtifactWriter.InternedNameConstant("a")];

        if (code[Thrower] != (byte)JsOpcode.GetProperty || code[Handler] != (byte)JsOpcode.TypeOf ||
            JsBaselineBlocks.EndsBlock(JsOpcode.GetProperty))
        {
            return (Name, false, "the hand-assembled unit does not put a thrower that ends no block right before its handler");
        }

        var image = new JsNativeProgramImage(code, functions, [0], [false], 4)
        {
            Tier = JsNativeTier.Baseline,
            Regions = regions,
        };

        var grouped = JsBaselineBlocks.GroupHandlerOffsets(image);

        if (!JsBaselineBlocks.TryPlan(image, 1, grouped.Of(1), out var plan, out var planRefusal))
        {
            return (Name, false, "the partition does not plan the thrower's unit: " + planRefusal);
        }

        var landed = plan.Landings.IndexOf(Handler) >= 0;
        JsBaselineBlock? holding = null;

        foreach (var block in plan.Blocks)
        {
            if (block.Head <= Thrower && block.Last >= Thrower)
            {
                holding = block;
                break;
            }
        }

        if (!landed || holding is not { } run || run.Last < Handler)
        {
            return (
                Name,
                false,
                "the plan does not run the thrower and its handler in one block: the handler " +
                    (landed ? "is" : "is not") + " a landing, and " +
                    (holding is { } found
                        ? "the block from " + found.Head + " ends at " + found.Last
                        : "no block's run holds the thrower"));
        }

        var backend = new JsX64Backend(abi);

        if (!backend.TryEmit(
                new JsAssembledProgram(JsFormat.ManifestId, code, functions, regions, constants, 4, 1),
                out var emission,
                out var emitRefusal))
        {
            return (Name, false, "the backend refused the hand-assembled program: " + emitRefusal);
        }

        byte[] Artifact(bool native)
        {
            var sections = new List<JavaScriptArtifactWriter.Section>
            {
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits, JsArtifactWriter.Limits(4, 1, 2, 1)),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants, JsArtifactWriter.Constants(constants)),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, code),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries, JsArtifactWriter.Entries([("script0", 0u)])),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.ExceptionRegions, JsArtifactWriter.ExceptionRegions(regions)),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions, JsArtifactWriter.Positions([(0, 1, 1)])),
                new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions, JsArtifactWriter.Functions(functions)),
            };

            if (native)
            {
                sections.Add(new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces, JsArtifactWriter.Surfaces([JsSurfaces.Native])));
                sections.Add(new(
                    (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeCode,
                    JsArtifactWriter.NativeCode(
                        (uint)emission.Architecture, emission.BackendSemanticVersion, emission.CodeAlignment, emission.Code)));
                sections.Add(new(
                    (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeSymbols,
                    JsArtifactWriter.NativeSymbols(emission.Symbols)));
            }

            return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
        }

        var interpreted = NativeLifecycle.RunWideArtifact(
            Artifact(native: false), JsOutputForm.Bytecode, string.Empty, WideFuel, reEmit: false, module: false);

        var emitted = NativeLifecycle.RunWideArtifact(
            Artifact(native: true), JsOutputForm.Native, abi.Name, WideFuel, reEmit: true, module: false);

        var left = interpreted.Render(withFuel: true);
        var right = emitted.Render(withFuel: true);
        var detail = "the block from " + run.Head + " runs through the thrower at " + Thrower + " to its last instruction at " +
            run.Last + ", at or past the handler at " + Handler + ", which is a landing; bytecode answered " + left +
            "; native answered " + right;

        if (emitted.Outcome.StartsWith("the verifier refused", StringComparison.Ordinal) ||
            interpreted.Outcome.StartsWith("the verifier refused", StringComparison.Ordinal))
        {
            return (Name, false, "the verifier refused the hand-assembled program: " + detail);
        }

        return (
            Name,
            interpreted.Completed &&
                string.Equals(interpreted.Completion, Expected, StringComparison.Ordinal) &&
                string.Equals(left, right, StringComparison.Ordinal),
            detail);
    }

    /// <summary>
    /// The probes, printing, with a full compacting collection forced from the print callback every
    /// thousand prints, answer the same in both forms.
    /// </summary>
    /// <remarks>
    /// <b>THE COLLECTION HAPPENS INSIDE A HOST CALL A HANDLER MADE</b>, so every emitted frame between
    /// the entry and that handler is on the machine stack while the collector runs and moves objects.
    /// An emitted frame that held a managed address, or an activation rooted only by something the
    /// collector cannot see, would answer differently or not at all. A row whose native run forced no
    /// collection has checked nothing and fails.
    /// </remarks>
    private static List<(string, bool, string)> FrequentCollections(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();

        static void Collect() =>
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

        foreach (var (name, source, guestLoads) in Collecting)
        {
            rows.Add(Agree(
                "native/baseline/frequent-collections/" + name,
                abi,
                source,
                null,
                guestLoads,
                also: (interpreted, emitted, _) => emitted.Collections > 0 && interpreted.Collections > 0
                    ? null
                    : "a form forced no collection, so this row checked nothing",
                collect: Collect));
        }

        return rows;
    }

    /// <summary>Runs one source in both forms and requires the two answers to be one.</summary>
    private static (string, bool, string) Agree(
        string name,
        JsX64Abi abi,
        string source,
        string? library,
        bool guestLoads,
        Func<NativeLifecycle.WideAnswer, NativeLifecycle.WideAnswer, TimeSpan, string?>? also = null,
        Action? collect = null)
    {
        // THE NATIVE HALF IS COMPILED ONCE AND REQUIRED TO CARRY EMITTED CODE BEFORE IT IS RUN. The
        // comparison below is of rendered answers only, and two interpreter runs answer alike; an
        // artifact compiled for the native form with no code section in it would make every row of
        // this kind a comparison of bytecode with bytecode, so that is refused here, by name.
        var compiledNative = NativeLifecycle.CompileWide(source, JsOutputForm.Native, abi.Name, library);

        if (!compiledNative.Succeeded || compiledNative.Artifact is null)
        {
            return (
                name,
                false,
                "the native form was refused by the front end: " +
                    (compiledNative.Diagnostics.Count == 0 ? "no diagnostic" : compiledNative.Diagnostics[0].ToString()));
        }

        if (EmittedCodeMissing(compiledNative.Artifact) is { } missing)
        {
            return (name, false, missing);
        }

        var clock = System.Diagnostics.Stopwatch.StartNew();
        var interpreted = NativeLifecycle.RunWide(source, JsOutputForm.Bytecode, string.Empty, WideFuel, library, collect);
        var interpretedTime = clock.Elapsed;

        clock.Restart();
        var emitted = NativeLifecycle.RunWideArtifact(
            compiledNative.Artifact,
            JsOutputForm.Native,
            abi.Name,
            WideFuel,
            reEmit: true,
            module: library is not null,
            collect);
        var emittedTime = clock.Elapsed;

        var left = interpreted.Render(withFuel: !guestLoads);
        var right = emitted.Render(withFuel: !guestLoads);
        var agreed = string.Equals(left, right, StringComparison.Ordinal);
        var extra = also?.Invoke(interpreted, emitted, emittedTime);

        var timing = "; bytecode " + (long)interpretedTime.TotalMilliseconds + " ms, native " +
            (long)emittedTime.TotalMilliseconds + " ms" +
            (guestLoads ? "; fuel reported and not compared: bytecode " + interpreted.Fuel + ", native " + emitted.Fuel : string.Empty) +
            (collect is null ? string.Empty : "; collections forced: bytecode " + interpreted.Collections + ", native " + emitted.Collections);

        if (!interpreted.Completed)
        {
            return (name, false, "the interpreter did not run the program to its end, so nothing was compared: " + left + timing);
        }

        if (!agreed)
        {
            return (name, false, "the interpreter answered " + left + " and the native form answered " + right + timing);
        }

        return extra is null
            ? (name, true, "both answered " + left + timing)
            : (name, false, extra + "; both answered " + left + timing);
    }

    /// <summary>Why an artifact compiled for the native form carries no emitted code, or null if it does.</summary>
    private static string? EmittedCodeMissing(byte[] artifact) =>
        NativeLifecycle.TryReadEmitted(artifact, out var code, out var symbols, out var refusal) &&
            code.Length != 0 && symbols.Length != 0
            ? null
            : "the native form's artifact carried no emitted code, so the native half would have run " +
                "the interpreter alone: " + (refusal.Length == 0 ? "empty code or symbol table" : refusal);

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
    private static byte[] Trampoline(JsX64Abi abi, bool misalign, int? entryPc = null)
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

        // A BASELINE UNIT TAKES A SECOND ARGUMENT, the program counter it is entered at. It is loaded
        // last because under System V its register is one the sentinel loop above wrote - and is
        // volatile there, so no row reads it back.
        if (entryPc is { } pc)
        {
            assembler.MovArgument1Imm32(abi.SecondArgumentRegister, pc);
        }

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
