// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The two directions of the template-closure scan: everything the backends emit is accepted, and
/// what is accepted is what a backend could have emitted.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE FIRST DIRECTION IS THE ONE THAT KEEPS THE BUILD HONEST AND IT IS THE ONE A CHECK CAN
/// RUN.</b> A table that drifted from the encoders would turn the scan into theatre: a payload
/// every backend of this build emits would be refused, and the refusal would arrive in a
/// composition nobody runs these checks in. So the rows below compile real numeric programs with
/// x86-64 under both calling conventions and with arm64, read the emitted bytes back OUT OF THE
/// ARTIFACT, and require the scan to accept every one of them. A template deleted from an encoder,
/// a register a backend started using, an immediate that grew - each of them fails a row here
/// rather than an artifact in the field.
/// </para>
/// <para>
/// <b>THE SECOND DIRECTION IS A PROPERTY OF THE TABLE AND NOT OF A RUN, AND THE ARGUMENT IS STATED
/// WHERE THE TABLE IS.</b> The scanner accepts a byte only where it belongs to an instantiation of
/// a template the table enumerates with every field inside the closed set that field admits, so the
/// accepted set is exactly the set those templates generate; no run can establish that, because it
/// is a statement about every byte sequence rather than about the ones a check happened to write.
/// What the rows below add is the negative half a reader can check: byte sequences that are legal
/// x86-64 or legal A64, that no backend of this build emits, and that the scan refuses BY NAME.
/// </para>
/// <para>
/// <b>The four-zero-byte row is the one this whole lane exists for.</b> A corpus entry carrying
/// four zero bytes as an x86-64 payload verified, was armed, was jumped into and killed the process
/// with an access violation. Four zero bytes are the first row below.
/// </para>
/// </remarks>
internal static class NativeTemplateScanChecks
{
    /// <summary>The name of the row this whole lane was written around.</summary>
    /// <remarks>
    /// <b>It was a constant because the diagnostic registry's row for the scan's refusal pointed at
    /// it, and as of 2026-09-08 no registry row points at any check of this file.</b> Its summary
    /// read "the name the diagnostic registry's row for the scan's refusal points at", and it was
    /// true for one revision of that registry: code 1625 was classified `check` - a reachability
    /// kind meaning "a named check of a producer composition reaches it and no entry of the
    /// retained corpus does" - and rule N7 required its case to be a string literal of this file.
    /// Five retained corpus entries reaching 1625 then landed, the row was promoted to `corpus` and
    /// the kind was withdrawn, so the constant is kept for the name it spells and NOT for a binding
    /// it no longer carries. The row below still runs, and its refusal is still the one this lane
    /// exists for.
    /// </remarks>
    internal const string FourZeroBytes = "a-native-payload-of-four-zero-bytes";

    /// <summary>Runs both directions.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var windows = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var systemV = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var arm64 = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        var rows = new System.Collections.Generic.List<(string, bool, string)>
        {
            Closure(JsNativeBackends.X64Windows, JsNativeArchitecture.X64Windows, 16, windows),
            Closure(JsNativeBackends.X64SystemV, JsNativeArchitecture.X64SystemV, 16, systemV),
            Closure(JsNativeBackends.Arm64, JsNativeArchitecture.Arm64, 4, arm64),
            // ONE TEMPLATE IS OUT OF REACH OF EVERY PROGRAM THE MANIFEST ADMITS, and it is named
            // here rather than quietly tolerated. `movq xmm0, rax` is emitted for one bytecode
            // instruction - a return of `undefined` - and no admitted source reaches it: the
            // numeric manifest refuses a function body that can fall off its end, and a program
            // body returns its completion slot with an ordinary return whatever that slot holds.
            // The template stays in the table because the backend has one and would emit it for an
            // artifact carrying that instruction, which a producer other than this front end can
            // write; what it does not have is a program in this lane that reaches it.
            Coverage(
                JsNativeBackends.X64Windows,
                JsNativeArchitecture.X64Windows,
                windows,
                "movq xmm0, rax"),
            Coverage(
                JsNativeBackends.X64SystemV,
                JsNativeArchitecture.X64SystemV,
                systemV,
                "movq xmm0, rax"),
            Coverage(JsNativeBackends.Arm64, JsNativeArchitecture.Arm64, arm64),
            TheTableAgreesWithTheConventionTable(),
        };

        rows.AddRange(Refusals());
        rows.Add(APayloadDeclaredForTheOtherConvention());
        rows.AddRange(Mutations());
        rows.AddRange(Verification());
        return rows;
    }

    // ---- everything the backends emit, the scanner accepts ---------------------------------

    /// <summary>
    /// The numeric programs the closure rows compile: one shape each rather than one sample each.
    /// </summary>
    /// <remarks>
    /// <b>EACH ONE REACHES A FAMILY OF TEMPLATES NO OTHER ONE DOES.</b> Straight-line arithmetic
    /// reaches the prologue, the slab loads and stores and the four arithmetic instructions; the
    /// block scope reaches the local slots; the conditional reaches a forward branch and a join;
    /// the loop reaches a BACKWARD branch and the fuel charge that pays for it; the float mix
    /// reaches the constant slab; the negation reaches the one sixty-four-bit pattern a unit
    /// exclusive-ors with; the comparisons reach every condition code both encoders spell; and the
    /// two function programs reach the direct call, the argument staging and the propagate epilogue
    /// - which the arm64 backend refuses, so those two rows count for x86-64 alone.
    /// </remarks>
    private static readonly (string Name, string Source)[] Programs =
    [
        ("straight-line arithmetic", "1 + 2 * 3 - 4 / 5;"),
        ("a block scope and repeated local reads", "{ let a = 3; let b = 4; a * a + b * b; }"),
        ("a forward branch and a join", "{ let a = 1; if (a < 2) { a = a * 10; } else { a = a - 1; } a; }"),
        ("a backward branch and a loop", "{ let t = 0; let i = 0; while (i < 10) { t = t + i * i; i = i + 1; } t; }"),
        ("nested loops", "{ let t = 0; let i = 0; while (i < 4) { let j = 0; while (j < 4) { t = t + 1; j = j + 1; } i = i + 1; } t; }"),
        ("a float mix and a constant pool", "{ let x = 1.5; let y = 2.25; x * y - x / y + 0.125; }"),
        ("a negation and a unary not", "{ let a = 1; let b = 2; if (!(a >= b)) { a = -a + b; } a; }"),
        ("every comparison", "{ let a = 1; let b = 2; let n = 0; if (a < b) { n = n + 1; } if (a <= b) { n = n + 1; } if (a > b) { n = n + 1; } if (a >= b) { n = n + 1; } if (a === b) { n = n + 1; } if (a !== b) { n = n + 1; } n; }"),
        ("a declared function and a direct call", "function f(a) { return a * 2; }\nfunction g(a, b) { return f(a) + b; }\ng(3, 4);"),
        ("a recursive function", "function fact(n) { if (n < 2) { return 1; } return n * fact(n - 1); }\nfact(5);"),
        ("a realm binding read and written", "let g = 1; g = g + 2; g;"),
        ("a function body that can complete with no value", "function f(a) { if (a > 0) { return a; } } f(1) + 0;"),
        ("the four relational comparisons", "{ let a = 1; let b = 2; let n = 0; if (a < b) { n = n + 1; } if (a <= b) { n = n + 1; } if (a > b) { n = n + 1; } if (a >= b) { n = n + 1; } n; }"),
        ("a program body whose completion is undefined", "{ let a = 1; }"),
    ];

    /// <summary>
    /// Compiles every program this backend admits, reads the emitted bytes out of the artifact and
    /// requires the scan to accept all of them.
    /// </summary>
    /// <remarks>
    /// <b>A REFUSED PROGRAM IS NOT A FAILING ROW AND IT IS NOT A SILENT PASS EITHER.</b> Each
    /// backend compiles a closed subset of the numeric manifest and refuses the rest by name - arm64
    /// refuses every program that declares a function, because a realm binding is an object graph a
    /// frame of doubles cannot hold - so a refusal is counted and reported rather than judged. What
    /// IS judged is that the count of scanned images is not zero: a row that scanned nothing would
    /// be a green tick for a table nobody exercised.
    /// </remarks>
    private static (string, bool, string) Closure(
        string backend,
        JsNativeArchitecture architecture,
        uint alignment,
        System.Collections.Generic.HashSet<string> instantiated)
    {
        var name = "everything `" + backend + "` emits, the template scan accepts";
        var scanned = 0;
        var refused = 0;
        var bytes = 0;

        foreach (var (label, source) in Programs)
        {
            var compiled = JsCompiler.Compile(
                [new JsScriptUnit("closure.js", source, SliceParseOptions.Script)],
                [],
                new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Native, backend));

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                refused++;
                continue;
            }

            if (!NativeLifecycle.TryReadEmitted(
                compiled.Artifact, out var code, out var symbols, out var refusal))
            {
                return (name, false, label + ": " + refusal);
            }

            var result = JsNativeScan.Scan(architecture, code, symbols, alignment, instantiated);

            if (!result.Accepted)
            {
                return (
                    name,
                    false,
                    label + ": the scan refused " + code.Length + " emitted bytes with " +
                    result.Outcome + " at offset " + result.Offset + " - " + result.Reason +
                    "; the bytes there read " + Around(code, result.Offset));
            }

            scanned++;
            bytes += code.Length;
        }

        return (
            name,
            scanned > 0,
            scanned == 0
                ? "every one of the " + Programs.Length + " programs was refused by this backend, " +
                    "so this row scanned nothing and establishes nothing"
                : scanned + " of " + Programs.Length + " programs emitted and scanned, " + bytes +
                    " bytes in all, " + refused + " refused by the backend before any byte was " +
                    "emitted");
    }

    /// <summary>
    /// Every template of this architecture's table is instantiated by at least one of the programs.
    /// </summary>
    /// <remarks>
    /// <b>THE CLOSURE ROW IS ONLY AS STRONG AS THE PROGRAMS IT COMPILES, AND THIS IS THE ROW THAT
    /// SAYS SO.</b> A template no program instantiates is a template the encoders could have
    /// contradicted with every row above still green: the direction "everything the backends emit,
    /// the scanner accepts" is a statement about emissions, and an emission nobody produced proves
    /// nothing about the entry that would have decoded it. So this row names the uncovered
    /// templates rather than reporting a number, and it passes only where the list is empty or
    /// every name on it is one the backends cannot reach for a reason stated below.
    /// </remarks>
    private static (string, bool, string) Coverage(
        string backend,
        JsNativeArchitecture architecture,
        System.Collections.Generic.HashSet<string> instantiated,
        params string[] outOfReach)
    {
        var name = "every template `" + backend + "` has is reached by one of these programs";
        var uncovered = new System.Collections.Generic.List<string>();
        var table = JsNativeTemplates.For(architecture);

        foreach (var template in table)
        {
            if (!instantiated.Contains(template.Text) &&
                System.Array.IndexOf(outOfReach, template.Text) < 0)
            {
                uncovered.Add(template.Text);
            }
        }

        var excepted = outOfReach.Length == 0
            ? string.Empty
            : ", with " + outOfReach.Length + " named out of reach of this manifest (" +
                string.Join(", ", outOfReach) + ")";

        return (
            name,
            uncovered.Count == 0,
            uncovered.Count == 0
                ? instantiated.Count + " of " + table.Length + " templates instantiated" + excepted
                : instantiated.Count + " of " + table.Length +
                    " templates instantiated; unreached and unaccounted for: " +
                    string.Join(", ", uncovered));
    }

    /// <summary>The sixteen bytes around an offset, so a failing row names what it met.</summary>
    /// <remarks>
    /// <b>A row that says only "no template matched" sends its reader back to the encoder with
    /// nothing to look for.</b> The bytes are what a reader decodes by hand against the table, and
    /// printing them is the difference between a failure that can be acted on and one that can only
    /// be reproduced.
    /// </remarks>
    private static string Around(byte[] code, uint offset)
    {
        var from = (int)System.Math.Max(0, (long)offset - 4);
        var to = (int)System.Math.Min(code.Length, offset + 12);
        var text = new System.Text.StringBuilder();

        for (var index = from; index < to; index++)
        {
            text.Append(code[index].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
        }

        return text.ToString() + " (from " + from + ")";
    }

    /// <summary>
    /// The three constants the table restates from the calling-convention table agree with it.
    /// </summary>
    /// <remarks>
    /// <b>THE TABLE CANNOT READ THE CONVENTION TABLE AND THAT IS WHY THIS ROW EXISTS.</b> The
    /// templates live in the format assembly, which references nothing, and the convention table
    /// lives beside the backends; a verifier that had to reference the lowering to know where a
    /// frame pointer is spilled would be a verifier an execution-only image could not carry. So the
    /// three numbers are written twice, and this row - which is in the one place that can see both -
    /// is what stops the two copies parting company.
    /// </remarks>
    private static (string, bool, string) TheTableAgreesWithTheConventionTable()
    {
        const string Name = "the template table's frame constants are the convention table's";
        var failures = new System.Collections.Generic.List<string>();

        foreach (var abi in JsX64Abi.Rows)
        {
            if (JsNativeTemplates.X64FrameBytes(abi.Architecture) != abi.FrameBytes)
            {
                failures.Add(
                    abi.Name + ": the table reserves " +
                    JsNativeTemplates.X64FrameBytes(abi.Architecture) +
                    " bytes and the convention reserves " + abi.FrameBytes);
            }

            if (JsNativeTemplates.X64FramePointerSlot(abi.Architecture) != abi.FramePointerSlot)
            {
                failures.Add(
                    abi.Name + ": the table spills at " +
                    JsNativeTemplates.X64FramePointerSlot(abi.Architecture) +
                    " and the convention spills at " + abi.FramePointerSlot);
            }

            if (JsNativeTemplates.X64FramePointerRegister(abi.Architecture) !=
                (int)abi.FramePointerRegister)
            {
                failures.Add(
                    abi.Name + ": the table names register " +
                    JsNativeTemplates.X64FramePointerRegister(abi.Architecture) +
                    " and the convention names " + (int)abi.FramePointerRegister);
            }
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? "both conventions agree about the frame size, the spill slot and the register " +
                    "the frame pointer arrives in"
                : string.Join("; ", failures));
    }

    // ---- what the scanner refuses ------------------------------------------------------------

    /// <summary>
    /// Byte sequences that are legal machine code, that no backend of this build emits, and that
    /// the scan refuses by name.
    /// </summary>
    /// <remarks>
    /// <b>EVERY ROW HERE IS A SEQUENCE A PROCESSOR WOULD HAVE EXECUTED.</b> That is the point: the
    /// architecture check asks which machine the bytes are for and not whether they are code, so
    /// each of these would have passed every structural clause the verifier had before the scan
    /// existed - and the first of them did, was armed, and killed a process.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Refusals() =>
    [
        // FOUR ZERO BYTES DECLARED AS x86-64. They decode as `add [rax], al` twice, which faults on
        // a null base and stores through any other. This is the payload JSC-208 records.
        Refused(
            FourZeroBytes,
            JsNativeArchitecture.X64Windows,
            [0x00, 0x00, 0x00, 0x00],
            JsNativeScanOutcome.NoTemplate),

        // FOUR ZERO BYTES DECLARED AS arm64, which decode as a permanently undefined instruction.
        Refused(
            "an arm64 payload of four zero bytes",
            JsNativeArchitecture.Arm64,
            [0x00, 0x00, 0x00, 0x00],
            JsNativeScanOutcome.NoTemplate),

        // AN INDIRECT CALL THROUGH A REGISTER. The encoder can spell it and the backends emit none:
        // every call in an emitted artifact is a direct branch to a code unit of the same artifact,
        // which is what keeps a value a guest computed from ever becoming an address. The table has
        // no template for it, so a payload carrying one is refused.
        Refused(
            "an indirect call through a register",
            JsNativeArchitecture.X64Windows,
            [0xFF, 0xD0, 0xC3],
            JsNativeScanOutcome.NoTemplate),

        // A SYSCALL, which is two bytes and reaches the operating system.
        Refused(
            "a syscall instruction",
            JsNativeArchitecture.X64Windows,
            [0x0F, 0x05, 0xC3],
            JsNativeScanOutcome.NoTemplate),

        // A FRAME DISPLACEMENT THAT NAMES NO FIELD. `mov rax, [r11+33]` is a legal encoding of an
        // instruction the backend emits at five other displacements, and 33 reads the second half
        // of one field and the first half of another.
        Refused(
            "a frame displacement between two declared fields",
            JsNativeArchitecture.X64Windows,
            [0x49, 0x8B, 0x83, 0x21, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // A SLAB DISPLACEMENT OFF THE EIGHT-BYTE GRID. `mov rdx, [rbx+4]` reads four bytes of one
        // double and four of the next.
        Refused(
            "a slab displacement off the eight-byte grid",
            JsNativeArchitecture.X64Windows,
            [0x48, 0x8B, 0x93, 0x04, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // AN IMMEDIATE NO UNIT MATERIALISES. `mov rax, 1` is the bail-out code this profile does not
        // have, and one is exactly the value the refused design would have used.
        Refused(
            "a materialised immediate no unit answers with",
            JsNativeArchitecture.X64Windows,
            [0x48, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // A UNIT THAT DOES NOT END IN A RETURN, so control would run off its last instruction into
        // whatever the allocator left after it.
        Refused(
            "a unit that runs off its own end",
            JsNativeArchitecture.X64Windows,
            [0x53, 0x5B],
            JsNativeScanOutcome.UnitDoesNotEndInAReturn),

        // PADDING THAT IS NOT PADDING: a no-op run after the return with something else inside it.
        Refused(
            "a byte hidden in a unit's alignment padding",
            JsNativeArchitecture.X64Windows,
            [0xC3, 0x90, 0x00],
            JsNativeScanOutcome.PaddingNotAlignment),

        // A BRANCH OUT OF THE UNIT. `jmp .+16` from a one-unit payload lands past the blob.
        Refused(
            "a branch that leaves the unit",
            JsNativeArchitecture.X64Windows,
            [0xE9, 0x10, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.BranchLeavesUnit),

        // A BRANCH INTO THE MIDDLE OF AN INSTRUCTION. The displacement is legal, the target is
        // inside the unit, and it lands on the second byte of a ten-byte move.
        Refused(
            "a branch into the middle of an instruction",
            JsNativeArchitecture.X64Windows,
            [
                0xE9, 0x01, 0x00, 0x00, 0x00,
                0x48, 0xB8, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x7F,
                0xC3,
            ],
            JsNativeScanOutcome.BranchIntoInstruction),
    ];

    /// <summary>Requires the scan to refuse one payload with one named outcome.</summary>
    private static (string, bool, string) Refused(
        string name, JsNativeArchitecture architecture, byte[] code, JsNativeScanOutcome expected)
    {
        var label = "the template scan refuses " + name;

        var result = JsNativeScan.Scan(
            architecture, code, [new JsNativeSymbolRow(0, 0)], alignment: 16);

        if (result.Accepted)
        {
            return (label, false, "the scan accepted " + code.Length + " bytes it should refuse");
        }

        return (
            label,
            result.Outcome == expected,
            result.Outcome == expected
                ? result.Outcome + " at offset " + result.Offset + ": " + result.Reason
                : "expected " + expected + " and the scan answered " + result.Outcome + " at " +
                    result.Offset + ": " + result.Reason);
    }

    // ---- and the mutations of a real emission --------------------------------------------------

    /// <summary>
    /// A real emitted image, mutated one byte at a time, is refused where the mutation matters.
    /// </summary>
    /// <remarks>
    /// <b>A HAND-WRITTEN REFUSAL ROW PROVES THE CLAUSE AND A MUTATED IMAGE PROVES THE SCAN REACHES
    /// IT.</b> The rows above are byte strings somebody wrote; these start from bytes a backend of
    /// this build actually emitted for a real program and change one thing, which is the shape a
    /// corrupted or forged payload has. What they cannot do is tell a wrong instruction from a right
    /// one: a mutation that turns one admitted template into another admitted template is accepted,
    /// and that is the scan's limit rather than this row's.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Mutations()
    {
        var rows = new System.Collections.Generic.List<(string, bool, string)>();

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit(
                "mutate.js",
                "{ let t = 0; let i = 0; while (i < 10) { t = t + i; i = i + 1; } t; }",
                SliceParseOptions.Script)],
            [],
            new JsCompileRequest(
                JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64Windows));

        var code = System.Array.Empty<byte>();
        var symbols = System.Array.Empty<JsNativeSymbolRow>();
        var refusal = "the loop program was refused by the front end or the backend";

        if (!compiled.Succeeded || compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out code, out symbols, out refusal))
        {
            rows.Add((
                "a real emitted image can be mutated",
                false,
                "the loop program did not compile to machine code: " + refusal));

            return rows;
        }

        rows.Add((
            "the unmutated image is accepted",
            JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16).Accepted,
            code.Length + " bytes emitted for one code unit"));

        // THE LAST BYTE OF A UNIT IS ITS RETURN, and an image one byte shorter is a unit that runs
        // off its own end.
        var truncated = code[..^1];

        rows.Add(Mutated(
            "an image with its last return removed",
            truncated,
            symbols,
            JsNativeScanOutcome.UnitDoesNotEndInAReturn));

        // ONE BIT OF ONE BRANCH DISPLACEMENT. The scan walks the image looking for the first branch
        // it can shift by one byte, which turns a target that is an instruction into a target that
        // is inside one.
        var shifted = (byte[])code.Clone();
        var moved = -1;

        for (var at = 0; at + 5 < shifted.Length; at++)
        {
            if (shifted[at] != 0xE9)
            {
                continue;
            }

            shifted[at + 1]++;
            moved = at;
            break;
        }

        rows.Add(moved < 0
            ? ("an image with one branch displacement moved", false, "the image carries no `jmp rel32`")
            : Mutated(
                "an image with one branch displacement moved",
                shifted,
                symbols,
                JsNativeScanOutcome.BranchIntoInstruction));

        return rows;
    }

    /// <summary>
    /// An image emitted for one calling convention and declared as the other is refused.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE TWO CONVENTIONS DIFFER IN THREE CONSTANTS AND THOSE THREE CONSTANTS ARE IN THE FIXED
    /// BYTES OF THE TABLE, so a mislabelled payload is a payload whose prologue matches no
    /// template.</b> A System V unit spills its frame pointer with <c>mov [rsp+0], rdi</c> and a
    /// Windows one with <c>mov [rsp+32], rcx</c>; neither is an instantiation of the other's table.
    /// The bytes are perfectly good machine code and the architecture value is one this build
    /// names, which is exactly the shape of payload the framing layer cannot see through.
    /// </para>
    /// <para>
    /// <b>WHAT THIS IS NOT is the defect the core retains as a fixture.</b> That one was a CALLER
    /// using the wrong convention to call code that was correct - a property of a call site and not
    /// of an artifact - and no scan of a payload can see it. What this row catches is the artifact
    /// that says it was built for a machine it was not built for.
    /// </para>
    /// </remarks>
    private static (string, bool, string) APayloadDeclaredForTheOtherConvention()
    {
        const string Name =
            "the template scan refuses an image emitted for the other calling convention";

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("convention.js", "1 + 2;", SliceParseOptions.Script)],
            [],
            new JsCompileRequest(
                JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64SystemV));

        var code = System.Array.Empty<byte>();
        var symbols = System.Array.Empty<JsNativeSymbolRow>();
        var refusal = "the front end or the backend refused the program";

        if (!compiled.Succeeded || compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out code, out symbols, out refusal))
        {
            return (Name, false, "the program did not compile for System V: " + refusal);
        }

        var honest = JsNativeScan.Scan(JsNativeArchitecture.X64SystemV, code, symbols, 16);
        var mislabelled = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16);

        return (
            Name,
            honest.Accepted && mislabelled.Outcome == JsNativeScanOutcome.NoTemplate,
            !honest.Accepted
                ? "the System V image was refused by its OWN table: " + honest.Outcome + " at " +
                    honest.Offset
                : "the same " + code.Length + " bytes: accepted as System V, and as Windows " +
                    mislabelled.Outcome + " at offset " + mislabelled.Offset);
    }

    /// <summary>Requires a mutated image to be refused with a named outcome.</summary>
    private static (string, bool, string) Mutated(
        string name, byte[] code, JsNativeSymbolRow[] symbols, JsNativeScanOutcome expected)
    {
        var label = "the template scan refuses " + name;
        var result = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16);

        return (
            label,
            result.Outcome == expected,
            result.Outcome == expected
                ? result.Outcome + " at offset " + result.Offset + ": " + result.Reason
                : "expected " + expected + " and the scan answered " + result.Outcome + " at " +
                    result.Offset + ": " + result.Reason);
    }

    // ---- and the same payload through the whole verifier ---------------------------------------

    /// <summary>
    /// The four zero bytes, in a whole artifact, refused by the verifier before anything is armed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE ROWS ABOVE CALL THE SCAN AND THESE CALL THE VERIFIER, and the difference is the whole
    /// reason for running both.</b> A scan that refuses a byte string proves the table; it proves
    /// nothing about whether the verifier reaches the scan, which arm it reaches it from, or what a
    /// composition is told when it does. These build an artifact of the shape the retained corpus
    /// carries - a native surface, an emitted-code section, a symbol for its one code unit - and
    /// hand it to a runtime built from the ORDINARY descriptor, which carries no emitter. That is
    /// the execution-only position exactly: no re-emission is possible, the framing is impeccable,
    /// and the payload is not code.
    /// </para>
    /// <para>
    /// <b>NOTHING HERE IS INSTANTIATED, and the accepting row names an architecture no host
    /// arms.</b> Verification is where the claim is; instantiating a hand-written payload is how
    /// the accident this lane exists for happened, so the control carries an A64 return that this
    /// build refuses to instantiate on every machine it runs on, and it stops at the verifier in
    /// any case.
    /// </para>
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Verification() =>
    [
        // THE PAYLOAD JSC-208 RECORDS, DECLARED FOR THE ARCHITECTURE THAT ARMED IT.
        Verified(
            "an x86-64 payload of four zero bytes is refused at verification",
            [0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.X64Windows,
            refused: true),

        // THE SAME FOUR BYTES DECLARED FOR arm64, so the refusal is a fact about the bytes rather
        // than about the calling convention the host happens to have.
        Verified(
            "an arm64 payload of four zero bytes is refused at verification",
            [0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.Arm64,
            refused: true),

        // AND THE CONTROL. Two refusals are satisfied by a verifier that refuses every artifact
        // carrying these sections at all, which is the easiest wrong implementation to write: this
        // one carries a single A64 return, which is one template instantiation, and it verifies.
        Verified(
            "a payload that is one instruction still verifies",
            [0xC0, 0x03, 0x5F, 0xD6],
            JsNativeArchitecture.Arm64,
            refused: false),
    ];

    /// <summary>Verifies one hand-built artifact and requires the answer this row expects.</summary>
    private static (string, bool, string) Verified(
        string name, byte[] payload, JsNativeArchitecture architecture, bool refused)
    {
        var artifact = Artifact(payload, architecture);
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(catalog, new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: []));

        if (!created.TryGetRuntime(out var runtime))
        {
            return (name, false, "the runtime refused creation: " + created.Outcome);
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("com.example.broiler.slice-compiler"));

            var verified = runtime.Verify(
                in descriptor, artifact, System.Threading.CancellationToken.None);

            var code = verified.Diagnostics.ProfileDiagnosticCode;
            var accepted = verified.TryGetArtifact(out _);

            if (!refused)
            {
                return (
                    name,
                    accepted,
                    accepted
                        ? "verified, and NOTHING INSTANTIATED IT: the row is about the verifier's " +
                            "answer, and the architecture it names is one no host of this build arms"
                        : "the verifier refused it: " + verified.Outcome + "/" + verified.Reason +
                            " code " + code);
            }

            return (
                name,
                !accepted && code == (int)JavaScriptDiagnosticCode.NativePayloadNotTemplateClosed,
                accepted
                    ? "THE VERIFIER ACCEPTED FOUR BYTES THAT ARE NOT CODE"
                    : verified.Outcome + "/" + verified.Reason + " code " + code);
        }
    }

    /// <summary>
    /// A version-2 artifact that is well formed in every respect except the payload it is handed.
    /// </summary>
    /// <remarks>
    /// <b>It is built here rather than borrowed from the retained corpus, because the corpus is a
    /// retained artefact and a check is not.</b> The shape is the corpus's own - one code unit, one
    /// constant, one entry point, the native surface declared, an emitted-code section and a symbol
    /// row for the unit - so what these rows exercise is the arrangement a real native artifact has
    /// rather than a special case built to be refused.
    /// </remarks>
    private static byte[] Artifact(byte[] payload, JsNativeArchitecture architecture)
    {
        byte[] body = [(byte)JsOpcode.LoadConstant, 0x00, 0x00, (byte)JsOpcode.Return];

        JavaScriptArtifactWriter.Section[] sections =
        [
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants([JsArtifactWriter.NumberConstant(1)])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, body),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", 0u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
                JsArtifactWriter.Positions([(0u, 1u, 1u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
                JsArtifactWriter.Functions(
                [
                    new JsFunctionRow(
                        0, 0, 1, 16, 0, (uint)body.Length,
                        (uint)JsFormat.FunctionFlags.ProgramBody),
                ])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces([JsSurfaces.Native])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeCode,
                JsArtifactWriter.NativeCode(
                    (uint)architecture,
                    backendSemanticVersion: 0,
                    codeAlignment: 1,
                    payload,
                    declaredByteLength: null)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeSymbols,
                JsArtifactWriter.NativeSymbols([new JsNativeSymbolRow(0, 0)])),
        ];

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections);
    }
}
