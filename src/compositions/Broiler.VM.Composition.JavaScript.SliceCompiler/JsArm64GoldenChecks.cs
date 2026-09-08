// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The golden-byte record for the arm64 backend: what its encoder wrote, retained beside the
/// derivation of every constant.
/// </summary>
/// <remarks>
/// <para>
/// <b>A GOLDEN BYTE IS A CLAIM ABOUT WHAT THE ENCODER WROTE AND NOT A CLAIM ABOUT WHAT A PROCESSOR
/// WOULD DO.</b> Every row below compares bytes this build produced against bytes retained here; a
/// green row says the encoder is stable and that its word matches an encoding derived by hand from
/// the architecture's field layout, and it says NOTHING about whether that word computes what this
/// profile's interpreter computes. That is the whole difference between this backend's evidence and
/// an executed backend's, and it must not be smudged: no figure, no support-table row and no
/// capability claim attaches to arm64 on the strength of anything in this file.
/// </para>
/// <para>
/// <b>WHY THE EVIDENCE STOPS THERE IS STATED IN THE BACKEND AND NOT INVENTED HERE.</b> The arm64
/// backend is emitting-only because the instruction-cache maintenance sequence an arm64 processor
/// requires between writing code as data and fetching it as code has no managed expression and no
/// dependable library export. Nothing in this file, and nothing the backend reaches, maps memory or
/// changes a page protection.
/// </para>
/// <para>
/// <b>Every expected word carries its field decomposition beside it, so a reader can CHECK the
/// constant rather than trust it.</b> A retained hexadecimal with no derivation is a record of what
/// the encoder did on the day it was recorded, which is exactly as true of a wrong encoder as of a
/// right one.
/// </para>
/// <para>
/// <b>The whole-program rows go through the real front end.</b> The assembled program a backend
/// consumes is an intermediate the compiler does not publish, so these rows compile a source under
/// the numeric manifest and rebuild that intermediate from the artifact the same compiler wrote -
/// which means a change to the LOWERING moves these bytes too. That is intended: a golden sequence
/// that only tracked the encoder would go green while the thing being encoded changed underneath
/// it.
/// </para>
/// </remarks>
internal static class JsArm64GoldenChecks
{
    /// <summary>Runs every golden row and every refusal row.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var rows = new System.Collections.Generic.List<(string, bool, string)>();
        rows.AddRange(Encodings());
        rows.AddRange(Programs());
        rows.AddRange(Refusals());
        rows.Add(ABranchTooFarIsRefusedRatherThanTruncated());
        rows.Add(ABranchAtTheLimitIsStillEncoded());
        rows.Add(TheDeclaredFrameOffsetsTileASequentialLayout());
        return rows;
    }

    // ---- one instruction at a time --------------------------------------------------------

    /// <summary>
    /// One row per instruction the encoder writes, each against a word derived by hand.
    /// </summary>
    /// <remarks>
    /// <b>THE DERIVATION IS THE POINT AND THE HEXADECIMAL IS THE SUMMARY.</b> Each row states the
    /// base word's bit range and the fields or-ed into it, so checking a row is arithmetic a reader
    /// can do rather than a comparison against whatever this encoder happened to emit.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Encodings() =>
    [
        // stp x29, x30, [sp, #-64]!  -- bits 31..22 = 1010100110 (0xA9800000), imm7 = -64/8 = -8
        // (0x78 << 15 = 0x3C0000), Rt2 = 30 (0x7800), Rn = 31 (0x3E0), Rt = 29 (0x1D).
        One("stp x29, x30, [sp, #-64]!", 0xA9BC7BFDu, a => a.StorePairPreIndex(29, 30, 31, -64)),

        // stp x19, x20, [sp, #16]  -- bits 31..22 = 1010100100 (0xA9000000), imm7 = 2 (0x10000),
        // Rt2 = 20 (0x5000), Rn = 31 (0x3E0), Rt = 19 (0x13).
        One("stp x19, x20, [sp, #16]", 0xA90153F3u, a => a.StorePair(19, 20, 31, 16)),

        // ldp x21, x22, [sp, #32]  -- bits 31..22 = 1010100101 (0xA9400000), imm7 = 4 (0x20000),
        // Rt2 = 22 (0x5800), Rn = 31 (0x3E0), Rt = 21 (0x15).
        One("ldp x21, x22, [sp, #32]", 0xA9425BF5u, a => a.LoadPair(21, 22, 31, 32)),

        // ldp x29, x30, [sp], #64  -- bits 31..22 = 1010100011 (0xA8C00000), imm7 = 8 (0x40000),
        // Rt2 = 30 (0x7800), Rn = 31 (0x3E0), Rt = 29 (0x1D).
        One("ldp x29, x30, [sp], #64", 0xA8C47BFDu, a => a.LoadPairPostIndex(29, 30, 31, 64)),

        // ret  -- bits 31..10 = 1101011001011111000000 (0xD65F0000), Rn = 30 (0x3C0). THE FIELD
        // FOR AN IMMEDIATE DOES NOT EXIST, which is why the `ret 8` defect class has no A64
        // spelling.
        One("ret", 0xD65F03C0u, a => a.Return()),

        // mov x22, x0  -- ORR (shifted register): bits 31..21 = 10101010000 (0xAA000000),
        // Rm = 0, shift = 0, Rn = 31 the ZERO register (0x3E0), Rd = 22 (0x16).
        One("mov x22, x0", 0xAA0003F6u, a => a.Move(22, 0)),

        // mov x29, sp  -- ADD (immediate): bits 31..23 = 100100010 (0x91000000), imm12 = 0,
        // Rn = 31 the STACK POINTER (0x3E0), Rd = 29 (0x1D). A different instruction from the row
        // above precisely because register 31 means two different registers in the two encodings.
        One("mov x29, sp", 0x910003FDu, a => a.MoveFromStackPointer(29)),

        // movz x9, #0x7FF8, lsl #48  -- bits 31..23 = 110100101 (0xD2800000), hw = 3 (0x600000),
        // imm16 = 0x7FF8 (0xFFF00), Rd = 9. This is the high halfword of a quiet NaN, which is how
        // this backend spells `undefined` in a slab of doubles.
        One("movz x9, #0x7FF8, lsl #48", 0xD2EFFF09u, a => a.MoveWide(9, 0x7FF8, 3)),

        // movz w0, #3  -- as above with sf = 0 (0x52800000), hw = 0, imm16 = 3 (0x60), Rd = 0.
        // Three is JsNativeReturn.FuelExhausted.
        One("movz w0, #3", 0x52800060u, a => a.MoveWide32(0, 3)),

        // movk w9, #1, lsl #16  -- bits 31..23 = 011100101 (0x72800000), hw = 1 (0x200000),
        // imm16 = 1 (0x20), Rd = 9. It KEEPS the bits it does not write, which is the difference
        // from movz and the reason a two-instruction pair can build a 32-bit offset.
        One("movk w9, #1, lsl #16", 0x72A00029u, a => a.MoveKeep32High(9, 1)),

        // fmov d0, x9  -- bits 31..21 = 10011110011 (0x9E600000), opcode = 111 (0x70000), Rn = 9
        // (0x120), Rd = 0. It moves BITS: this is what makes the NaN above a NaN and not a 32760.
        One("fmov d0, x9", 0x9E670120u, a => a.MoveBitsToDouble(0, 9)),

        // scvtf d0, x9  -- as above with opcode = 010 (0x20000). It CONVERTS: the zero or one a
        // cset produced becomes the 0.0 or 1.0 a Boolean is represented by here.
        One("scvtf d0, x9", 0x9E620120u, a => a.ConvertToDouble(0, 9)),

        // cset x9, mi  -- CSINC x9, xzr, xzr, pl: bits 31..21 = 10011010100 (0x9A800000),
        // Rm = 31 (0x1F0000), cond = 5 (0x5000), bits 11..10 = 01 (0x400), Rn = 31 (0x3E0),
        // Rd = 9. THE CONDITION IN THE WORD IS 5 AND THE CONDITION ASKED FOR IS 4, because CSINC
        // increments when its condition fails and inverting an A64 condition is exclusive-or one.
        One("cset x9, mi", 0x9A9F57E9u, a => a.SetIfCondition(9, JsArm64Condition.Minus)),

        // cset x10, vs  -- as above with cond 6 inverted to 7 (0x7000) and Rd = 10.
        One("cset x10, vs", 0x9A9F77EAu, a => a.SetIfCondition(10, JsArm64Condition.Overflow)),

        // subs x9, x9, #8  -- bits 31..23 = 111100010 (0xF1000000), sh = 0, imm12 = 8 (0x2000),
        // Rn = 9 (0x120), Rd = 9. The S bit is what makes the fuel check a compare as well as a
        // subtract.
        One("subs x9, x9, #8", 0xF1002129u, a => a.SubtractSetting(9, 9, 8)),

        // orr x9, x9, x10  -- bits 31..21 = 10101010000 (0xAA000000), Rm = 10 (0xA0000),
        // shift = 0, Rn = 9 (0x120), Rd = 9.
        One("orr x9, x9, x10", 0xAA0A0129u, a => a.Or(9, 9, 10)),

        // ldr x19, [x22, #0]  -- bits 31..22 = 1111100101 (0xF9400000), imm12 = 0, Rn = 22
        // (0x2C0), Rt = 19 (0x13).
        One("ldr x19, [x22, #0]", 0xF94002D3u, a => a.LoadRegister(19, 22, 0)),

        // ldr x23, [x22, #40]  -- as above with imm12 = 40/8 = 5 (0x1400) and Rt = 23 (0x17).
        // Forty is where JsNativeFrame keeps the fuel counter's address.
        One("ldr x23, [x22, #40]", 0xF94016D7u, a => a.LoadRegister(23, 22, 40)),

        // str x23, [sp, #48]  -- bits 31..22 = 1111100100 (0xF9000000), imm12 = 6 (0x1800),
        // Rn = 31 (0x3E0), Rt = 23 (0x17).
        One("str x23, [sp, #48]", 0xF9001BF7u, a => a.StoreRegister(23, 31, 48)),

        // str w9, [x22, #48]  -- size = 10 (0xB9000000), imm12 = 48/4 = 12 (0x3000), Rn = 22
        // (0x2C0), Rt = 9. The immediate scales by FOUR here and by eight above, which is why the
        // two forms cannot share one method.
        One("str w9, [x22, #48]", 0xB90032C9u, a => a.StoreWord(9, 22, 48)),

        // ldr d0, [x21, #8]  -- bits 31..22 = 1111110101 (0xFD400000), imm12 = 1 (0x400),
        // Rn = 21 (0x2A0), Rt = 0. Bit 26 is the ONLY difference from the integer load, and it is
        // what decides which register file the value lands in.
        One("ldr d0, [x21, #8]", 0xFD4006A0u, a => a.LoadDouble(0, 21, 8)),

        // str d0, [x19, #16]  -- bits 31..22 = 1111110100 (0xFD000000), imm12 = 2 (0x800),
        // Rn = 19 (0x260), Rt = 0.
        One("str d0, [x19, #16]", 0xFD000A60u, a => a.StoreDouble(0, 19, 16)),

        // fadd d0, d0, d1  -- bits 31..21 = 00011110011 (0x1E600000), Rm = 1 (0x10000),
        // bits 15..10 = 001010 (0x2800), Rn = 0, Rd = 0.
        One("fadd d0, d0, d1", 0x1E612800u, a => a.AddDouble(0, 0, 1)),

        // fsub d0, d0, d1  -- as above with bits 15..10 = 001110 (0x3800).
        One("fsub d0, d0, d1", 0x1E613800u, a => a.SubtractDouble(0, 0, 1)),

        // fmul d0, d0, d1  -- as above with bits 15..10 = 000010 (0x0800).
        One("fmul d0, d0, d1", 0x1E610800u, a => a.MultiplyDouble(0, 0, 1)),

        // fdiv d0, d0, d1  -- as above with bits 15..10 = 000110 (0x1800).
        One("fdiv d0, d0, d1", 0x1E611800u, a => a.DivideDouble(0, 0, 1)),

        // fneg d0, d0  -- one source: bits 31..21 = 00011110011 (0x1E600000), opcode = 000010
        // (0x10000), bits 14..10 = 10000 (0x4000), Rn = 0, Rd = 0.
        One("fneg d0, d0", 0x1E614000u, a => a.NegateDouble(0, 0)),

        // fcmp d0, d1  -- bits 31..21 = 00011110011 (0x1E600000), Rm = 1 (0x10000),
        // bits 15..10 = 001000 (0x2000), Rn = 0, opcode2 = 00000.
        One("fcmp d0, d1", 0x1E612000u, a => a.CompareDouble(0, 1)),

        // fcmp d0, #0.0  -- as above with Rm = 0 and opcode2 = 01000, which sets bit 3.
        One("fcmp d0, #0.0", 0x1E602008u, a => a.CompareDoubleWithZero(0)),

        // b .+8  -- bits 31..26 = 000101 (0x14000000), imm26 = 2 WORDS. A displacement counted in
        // bytes would branch four times too far.
        Branch("b .+8", 0x14000002u),

        // b.ge .+20  -- bits 31..24 = 01010100 (0x54000000), imm19 = 5 words (0xA0), bit 4 = 0,
        // cond = 1010 = GE. The word reads 0x540000AA, whose low byte carries both the condition
        // and the bottom bit of the displacement.
        ConditionalBranch("b.ge .+20", 0x540000AAu),
    ];

    /// <summary>Emits one instruction and compares the four bytes it wrote.</summary>
    private static (string, bool, string) One(
        string name, uint expected, System.Action<JsArm64Assembler> emit)
    {
        var assembler = new JsArm64Assembler();
        emit(assembler);

        if (!assembler.TryFix(out var refusal))
        {
            return ("the encoder writes `" + name + "`", false, refusal);
        }

        var bytes = assembler.ToArray();

        if (bytes.Length != 4)
        {
            return (
                "the encoder writes `" + name + "`",
                false,
                "the encoder wrote " + bytes.Length + " bytes where one A64 instruction is four");
        }

        var written = (uint)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));

        return (
            "the encoder writes `" + name + "`",
            written == expected,
            written == expected
                ? "0x" + expected.ToString("X8") + ", little-endian " + Hex(bytes)
                : "expected 0x" + expected.ToString("X8") + " and the encoder wrote 0x" +
                    written.ToString("X8"));
    }

    /// <summary>Emits an unconditional branch over one filler word and compares the patched word.</summary>
    private static (string, bool, string) Branch(string name, uint expected)
    {
        var assembler = new JsArm64Assembler();
        var label = assembler.DefineLabel();
        assembler.Branch(label);
        assembler.Word(0xD503201Fu);
        assembler.Bind(label);

        return Patched(name, expected, assembler);
    }

    /// <summary>Emits a conditional branch over four filler words and compares the patched word.</summary>
    private static (string, bool, string) ConditionalBranch(string name, uint expected)
    {
        var assembler = new JsArm64Assembler();
        var label = assembler.DefineLabel();
        assembler.BranchIf(JsArm64Condition.GreaterOrEqual, label);

        for (var filler = 0; filler < 4; filler++)
        {
            assembler.Word(0xD503201Fu);
        }

        assembler.Bind(label);
        return Patched(name, expected, assembler);
    }

    /// <summary>Fixes up a branch stream and compares the branch word it patched.</summary>
    private static (string, bool, string) Patched(
        string name, uint expected, JsArm64Assembler assembler)
    {
        if (!assembler.TryFix(out var refusal))
        {
            return ("the encoder writes `" + name + "`", false, refusal);
        }

        var bytes = assembler.ToArray();
        var written = (uint)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));

        return (
            "the encoder writes `" + name + "`",
            written == expected,
            written == expected
                ? "0x" + expected.ToString("X8")
                : "expected 0x" + expected.ToString("X8") + " and the encoder wrote 0x" +
                    written.ToString("X8"));
    }

    // ---- whole programs ---------------------------------------------------------------------

    /// <summary>
    /// One row per complete numeric program, against the whole byte sequence it emitted.
    /// </summary>
    /// <remarks>
    /// <b>THE FOUR PROGRAMS ARE FOUR SHAPES AND NOT FOUR SAMPLES.</b> The first is straight-line
    /// arithmetic with no branch at all; the second adds a block scope and repeated local reads;
    /// the third adds a forward branch and a join, which is where an operand height that disagreed
    /// with itself would show; the fourth adds a BACKWARD branch, which is the one displacement a
    /// forward-only encoder would get wrong and the one place a loop's fuel charge is repaid.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Programs() =>
    [
        Program(
            "straight-line arithmetic",
            "1 + 2;",
            "FD7BBCA9F35301A9F55B02A9F71B00F9FD030091F60300AAD30240F9D40A40F9D51240F9" +
            "D71640F9E90240F9292100F1E90200F9AA000054090080520900A072C93200B916000014" +
            "09FFEFD22001679E600200FD600240FD800200FDA00240FD600200FDA00640FD600600FD" +
            "600240FD610640FD0028611E600200FD600240FD800200FD800240FD600200FD600240FD" +
            "600200FD000080520200001460008052F71B40F9F55B42A9F35341A9FD7BC4A8C0035FD6"),

        Program(
            "a block scope and repeated local reads",
            "{ let a = 3; let b = 4; a * a + b * b; }",
            "FD7BBCA9F35301A9F55B02A9F71B00F9FD030091F60300AAD30240F9D40A40F9D51240F9" +
            "D71640F9E90240F9294900F1E90200F9AA000054090080520900A072C93200B92A000014" +
            "09FFEFD22001679E600200FD600240FD800200FDA00240FD600200FD600240FD800600FD" +
            "A00640FD600200FD600240FD800A00FD800640FD600200FD800640FD600600FD600240FD" +
            "610640FD0008611E600200FD800A40FD600600FD800A40FD600A00FD600640FD610A40FD" +
            "0008611E600600FD600240FD610640FD0028611E600200FD600240FD800200FD800240FD" +
            "600200FD600240FD600200FD000080520200001460008052F71B40F9F55B42A9F35341A9" +
            "FD7BC4A8C0035FD6"),

        Program(
            "a forward branch and a join",
            "{ let a = 1; if (a < 2) { a = a * 10; } else { a = a - 1; } a; }",
            "FD7BBCA9F35301A9F55B02A9F71B00F9FD030091F60300AAD30240F9D40A40F9D51240F9" +
            "D71640F9E90240F9292D00F1E90200F9AA000054090080520900A072C93200B95C000014" +
            "09FFEFD22001679E600200FD600240FD800200FDA00240FD600200FD600240FD800600FD" +
            "09FFEFD22001679E600200FD600240FD800200FD800640FD600200FDA00640FD600600FD" +
            "600240FD610640FD0020611EE9579F9A2001629E600200FD600240FD0820601E20030054" +
            "06030054E90240F9291D00F1E90200F9AA000054290480520900A072C93200B938000014" +
            "800640FD600200FDA00A40FD600600FD600240FD610640FD0008611E600200FD600240FD" +
            "600600FD600640FD800600FD600240FD800200FD17000014E90240F9291900F1E90200F9" +
            "AA000054E90680520900A072C93200B921000014800640FD600200FDA00240FD600600FD" +
            "600240FD610640FD0038611E600200FD600240FD600600FD600640FD800600FD600240FD" +
            "800200FDE90240F9291500F1E90200F9AA000054090980520900A072C93200B90B000014" +
            "800640FD600200FD600240FD800200FD800240FD600200FD600240FD600200FD00008052" +
            "0200001460008052F71B40F9F55B42A9F35341A9FD7BC4A8C0035FD6"),

        Program(
            "a backward branch and a loop",
            "{ let t = 0; let i = 0; while (i < 10) { t = t + i * i; i = i + 1; } t; }",
            "FD7BBCA9F35301A9F55B02A9F71B00F9FD030091F60300AAD30240F9D40A40F9D51240F9" +
            "D71640F9E90240F9292500F1E90200F9AA000054090080520900A072C93200B966000014" +
            "09FFEFD22001679E600200FD600240FD800200FDA00240FD600200FD600240FD800600FD" +
            "A00240FD600200FD600240FD800A00FD09FFEFD22001679E600200FD600240FD800200FD" +
            "E90240F9291100F1E90200F9AA000054690380520900A072C93200B94C000014800A40FD" +
            "600200FDA00640FD600600FD600240FD610640FD0020611EE9579F9A2001629E600200FD" +
            "600240FD0820601EA005005486050054E90240F9293D00F1E90200F9AA00005409058052" +
            "0900A072C93200B936000014800640FD600200FD800A40FD600600FD800A40FD600A00FD" +
            "600640FD610A40FD0008611E600600FD600240FD610640FD0028611E600200FD600240FD" +
            "600600FD600640FD800600FD600240FD800200FD800A40FD600200FDA00A40FD600600FD" +
            "600240FD610640FD0028611E600200FD600240FD600600FD600640FD800A00FD600240FD" +
            "800200FDC0FFFF17E90240F9291500F1E90200F9AA000054A90A80520900A072C93200B9" +
            "0B000014800640FD600200FD600240FD800200FD800240FD600200FD600240FD600200FD" +
            "000080520200001460008052F71B40F9F55B42A9F35341A9FD7BC4A8C0035FD6"),
    ];

    /// <summary>Compiles one source under the numeric manifest and compares the emitted bytes.</summary>
    private static (string, bool, string) Program(string name, string source, string expected)
    {
        var label = "arm64 emits the retained bytes for " + name;

        if (!TryAssemble(source, out var assembled, out var detail))
        {
            return (label, false, detail);
        }

        if (!new JsArm64Backend().TryEmit(assembled, out var emission, out var refusal))
        {
            return (label, false, "the arm64 backend refused: " + refusal);
        }

        var written = Hex(emission.Code);

        if (!string.Equals(written, expected, System.StringComparison.Ordinal))
        {
            return (
                label,
                false,
                "the emitted bytes are not the retained ones; " + emission.Code.Length +
                " bytes emitted against " + (expected.Length / 2) + " retained, and the emission " +
                "reads " + written);
        }

        if (emission.Symbols.Length != assembled.Functions.Count)
        {
            return (
                label,
                false,
                "the emission carries " + emission.Symbols.Length + " symbols for " +
                assembled.Functions.Count + " code units");
        }

        return (
            label,
            true,
            emission.Code.Length + " bytes, " + emission.Symbols.Length +
            " symbols, alignment " + emission.CodeAlignment +
            " - a record of what the encoder wrote and not of what a processor would do");
    }

    // ---- refusals ---------------------------------------------------------------------------

    /// <summary>
    /// One row per family of instruction the arm64 backend refuses, each naming its own reason.
    /// </summary>
    /// <remarks>
    /// <b>A REFUSAL IS EVIDENCE AND NOT AN ABSENCE, so it is retained like an encoding.</b> Each
    /// row compiles a source that reaches the family, asks the backend for machine code, and
    /// requires the refusal to name the instruction. A backend that quietly grew a template for one
    /// of these would go green on every golden row above and would be wrong about the one input
    /// nobody thought to write down.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Refusals() =>
    [
        Refused("a remainder is refused: no A64 instruction has its semantics",
            "{ let n = 3; n % 2; }", "Remainder"),
        Refused("an exponentiation is refused: no A64 instruction has its semantics",
            "{ let n = 3; n ** 2; }", "Exponent"),
        Refused("a bitwise or is refused: ToInt32 is not what fcvtzs does",
            "{ let n = 3; n | 1; }", "BitwiseOr"),
        Refused("a shift is refused: ToInt32 is not what fcvtzs does",
            "{ let n = 3; n << 1; }", "ShiftLeft"),
        Refused("a strict equality is refused: a slab of doubles cannot tell undefined from NaN",
            "{ let n = 3; n === 3; }", "StrictEquals"),
        Refused("a loose equality is refused: a slab of doubles cannot tell undefined from NaN",
            "{ let n = 3; n == 3; }", "LooseEquals"),
        Refused("a global lexical binding is refused: the realm is an object graph",
            "let g = 1;", "DeclareGlobalLet"),
        // A FUNCTION DECLARATION IS REFUSED AT ITS PROGRAM BODY AND NOT AT ITS CLOSURE, and the
        // row says so rather than claiming the more interesting refusal. A top-level function
        // binds a global before anything makes a closure of it, so the realm sentence is the one
        // that arrives first; the closure refusal exists and no source of this manifest can reach
        // it without reaching the realm first.
        Refused("a function declaration is refused: its program body reaches the realm",
            "function f(a) { return a * 2; }", "DeclareGlobal"),
    ];

    /// <summary>Compiles one source and requires the arm64 backend to refuse it by name.</summary>
    private static (string, bool, string) Refused(string name, string source, string instruction)
    {
        if (!TryAssemble(source, out var assembled, out var detail))
        {
            return (name, false, detail);
        }

        if (new JsArm64Backend().TryEmit(assembled, out _, out var refusal))
        {
            return (name, false, "the arm64 backend emitted machine code for `" + instruction + "`");
        }

        return (
            name,
            refusal.Contains("`" + instruction + "`", System.StringComparison.Ordinal),
            refusal);
    }

    // ---- the range checks --------------------------------------------------------------------

    /// <summary>
    /// A conditional branch one word beyond its reach is refused rather than truncated.
    /// </summary>
    /// <remarks>
    /// <b>THIS IS THE ROW THE WHOLE ENCODER EXISTS TO MAKE POSSIBLE.</b> A truncated displacement
    /// is a legal instruction word that branches somewhere else: the artifact would verify, every
    /// golden row above would stay green, and the failure would arrive as a jump into the middle of
    /// an unrelated block. Nothing downstream of the patcher can see it, so the patcher has to
    /// refuse, and this row is what says it does.
    /// </remarks>
    private static (string, bool, string) ABranchTooFarIsRefusedRatherThanTruncated()
    {
        var assembler = new JsArm64Assembler();
        var label = assembler.DefineLabel();
        assembler.Bind(label);

        for (var filler = 0; filler <= JsArm64Assembler.ConditionalBranchReach + 1; filler++)
        {
            assembler.Word(0xD503201Fu);
        }

        assembler.BranchIf(JsArm64Condition.Equal, label);
        var patched = assembler.TryFix(out var refusal);

        return (
            "a conditional branch past its reach is refused",
            !patched && refusal.Contains("truncated", System.StringComparison.Ordinal),
            patched ? "the encoder patched a displacement the field cannot carry" : refusal);
    }

    /// <summary>
    /// A conditional branch exactly at its reach is still encoded, so the refusal is not a habit.
    /// </summary>
    /// <remarks>
    /// <b>A range check with no row at the boundary is a range check that could be off by one in
    /// the safe direction and nobody would know.</b> The field is signed, so the negative end is one
    /// further than the positive one; this row sits on the negative end.
    /// </remarks>
    private static (string, bool, string) ABranchAtTheLimitIsStillEncoded()
    {
        var assembler = new JsArm64Assembler();
        var label = assembler.DefineLabel();
        assembler.Bind(label);

        for (var filler = 0; filler <= JsArm64Assembler.ConditionalBranchReach; filler++)
        {
            assembler.Word(0xD503201Fu);
        }

        assembler.BranchIf(JsArm64Condition.Equal, label);

        if (!assembler.TryFix(out var refusal))
        {
            return ("a conditional branch at its reach is still encoded", false, refusal);
        }

        // The site is the last word; its displacement is -(reach + 1) = -262144, which is the most
        // negative value a signed 19-bit field carries. Encoded, imm19 is 0x40000 and the word
        // reads 0x54800000 with cond = 0.
        var bytes = assembler.ToArray();
        var site = bytes.Length - 4;

        var written = (uint)(
            bytes[site] | (bytes[site + 1] << 8) | (bytes[site + 2] << 16) | (bytes[site + 3] << 24));

        return (
            "a conditional branch at its reach is still encoded",
            written == 0x54800000u,
            "0x" + written.ToString("X8"));
    }

    /// <summary>
    /// The frame offsets the backend computes with tile a sequential layout of the declared fields.
    /// </summary>
    /// <remarks>
    /// <b>The structure states its layout in prose and the backend restates it as numbers, and two
    /// records of one fact can part company.</b> This row does the arithmetic the structure's own
    /// remarks describe - three pointers, two counts, one pointer, one integer, with a 64-bit
    /// target's padding after each count that precedes a pointer - and compares it to the constants
    /// an instruction word actually carries.
    /// </remarks>
    private static (string, bool, string) TheDeclaredFrameOffsetsTileASequentialLayout()
    {
        var expected = new[] { 0, 8, 16, 24, 32, 40, 48 };

        var declared = new[]
        {
            JsArm64Frame.OperandsOffset,
            JsArm64Frame.OperandCountOffset,
            JsArm64Frame.LocalsOffset,
            JsArm64Frame.LocalCountOffset,
            JsArm64Frame.ConstantsOffset,
            JsArm64Frame.FuelOffset,
            JsArm64Frame.BailoutPcOffset,
        };

        for (var field = 0; field < expected.Length; field++)
        {
            if (declared[field] != expected[field])
            {
                return (
                    "the arm64 frame offsets tile the declared layout",
                    false,
                    "field " + field + " is at " + declared[field] + " and the declared layout puts " +
                    "it at " + expected[field]);
            }
        }

        return (
            "the arm64 frame offsets tile the declared layout",
            JsArm64Frame.StackFrameBytes % 16 == 0,
            "seven fields at 0, 8, 16, 24, 32, 40 and 48, and a stack frame of " +
            JsArm64Frame.StackFrameBytes + " bytes");
    }

    // ---- rebuilding the intermediate ----------------------------------------------------------

    /// <summary>
    /// Compiles one source under the numeric manifest and rebuilds the program a backend consumes.
    /// </summary>
    /// <remarks>
    /// <b>IT READS THE ARTIFACT BACK BECAUSE THE INTERMEDIATE IS NOT PUBLISHED, and that is a
    /// weakness of these rows worth naming.</b> The compiler builds a <c>JsAssembledProgram</c>,
    /// hands it to a backend and then writes it into an artifact; nothing exposes the middle. So
    /// these rows reconstruct it from the sections the same compilation wrote, which is faithful
    /// only for as long as the reconstruction below and the writer agree about the encoding. A
    /// disagreement would show as a refusal or as a golden row that moved, not as a silent pass.
    /// </remarks>
    private static bool TryAssemble(
        string source, out JsAssembledProgram assembled, out string detail)
    {
        assembled = null!;

        var compilation = JsCompiler.Compile(
            [new JsScriptUnit("golden.js", source, new SliceParseOptions(SliceGoal.Script))],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Bytecode, string.Empty));

        if (!compilation.Succeeded || compilation.Artifact is null)
        {
            detail = "the front end refused the source: " + string.Join("; ", compilation.Diagnostics);
            return false;
        }

        var reader = new Sections(compilation.Artifact);
        assembled = reader.Assembled();
        detail = string.Empty;
        return true;
    }

    /// <summary>A reader for the three artifact sections a backend's input is made of.</summary>
    private sealed class Sections
    {
        private readonly byte[] artifact;

        private int at;

        internal Sections(byte[] artifact) => this.artifact = artifact;

        /// <summary>Rebuilds the assembled program from the code, functions and constant sections.</summary>
        internal JsAssembledProgram Assembled()
        {
            at = 4;
            Variable();
            var name = (int)Variable();
            var manifest = System.Text.Encoding.UTF8.GetString(artifact, at, name);
            at += name;
            var sections = (int)Variable();
            byte[] code = [];
            byte[] functions = [];
            byte[] constants = [];

            for (var section = 0; section < sections; section++)
            {
                var kind = (int)Variable();
                var length = (int)Variable();
                var body = new byte[length];
                System.Array.Copy(artifact, at, body, 0, length);
                at += length;

                if (kind == (int)JsFormat.SectionKind.Code)
                {
                    code = body;
                }
                else if (kind == (int)JsFormat.SectionKind.Functions)
                {
                    functions = body;
                }
                else if (kind == (int)JsFormat.SectionKind.Constants)
                {
                    constants = body;
                }
            }

            var pool = Pool(constants);
            var rows = Rows(functions, out var stack, out var slots);
            return new JsAssembledProgram(manifest, code, rows, [], pool, stack, slots);
        }

        private ulong Variable()
        {
            ulong value = 0;
            var shift = 0;

            while (true)
            {
                var piece = artifact[at++];
                value |= (ulong)(piece & 0x7F) << shift;

                if ((piece & 0x80) == 0)
                {
                    return value;
                }

                shift += 7;
            }
        }

        private static System.Collections.Generic.List<byte[]> Pool(byte[] constants)
        {
            var entries = new System.Collections.Generic.List<byte[]>();
            var cursor = 0;
            var count = (int)Read(constants, ref cursor);

            for (var entry = 0; entry < count; entry++)
            {
                var start = cursor;
                var tag = constants[cursor++];

                if (tag == (byte)JsFormat.ConstantTag.Number)
                {
                    cursor += 8;
                }
                else if (tag == (byte)JsFormat.ConstantTag.Boolean)
                {
                    cursor += 1;
                }
                else if (tag == (byte)JsFormat.ConstantTag.InternedName ||
                    tag == (byte)JsFormat.ConstantTag.String)
                {
                    cursor += (int)Read(constants, ref cursor);
                }

                var bytes = new byte[cursor - start];
                System.Array.Copy(constants, start, bytes, 0, bytes.Length);
                entries.Add(bytes);
            }

            return entries;
        }

        private static System.Collections.Generic.List<JsFunctionRow> Rows(
            byte[] functions, out uint stack, out uint slots)
        {
            var rows = new System.Collections.Generic.List<JsFunctionRow>();
            var cursor = 0;
            var count = (int)Read(functions, ref cursor);
            stack = 1;
            slots = 1;

            for (var row = 0; row < count; row++)
            {
                var built = new JsFunctionRow(
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor),
                    (uint)Read(functions, ref cursor));

                rows.Add(built);
                stack = System.Math.Max(stack, built.MaxOperandStack);
                slots = System.Math.Max(slots, built.ScopeSlots);
            }

            return rows;
        }

        private static ulong Read(byte[] body, ref int cursor)
        {
            ulong value = 0;
            var shift = 0;

            while (true)
            {
                var piece = body[cursor++];
                value |= (ulong)(piece & 0x7F) << shift;

                if ((piece & 0x80) == 0)
                {
                    return value;
                }

                shift += 7;
            }
        }
    }

    /// <summary>Renders bytes as upper-case hexadecimal, which is the form the goldens retain.</summary>
    private static string Hex(byte[] bytes) => System.Convert.ToHexString(bytes);
}
