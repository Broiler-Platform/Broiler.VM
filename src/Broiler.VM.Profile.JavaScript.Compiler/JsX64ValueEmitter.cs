// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           0
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Critical
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The emitter of the wide manifest's value form for x86-64: every code unit emitted as its value layout,
/// with the pure instructions inline over the region's words and every other one a helper call (JSD-0035,
/// stage JSV-2).
/// </summary>
/// <remarks>
/// <para>
/// <b>THE PLAN AND THE LAYOUT ARE <see cref="JsValueLayout"/>'S, AND THIS FILE ENCODES THEM.</b> Which words
/// a region holds, which bindings are resident, which instructions are inline, where a guard goes and where
/// the debt is tested are the plan's and the layout's; this file writes one template per layout entry and
/// patches each branch to the entry it names, exactly as the baseline emitter does for its layout, so the
/// template scan's comparison of a payload with the layout is a comparison with what this file would write.
/// </para>
/// <para>
/// <b>IT IS A PURE FUNCTION OF THE IMAGE</b>, residency flag included, so the verifier's re-emission of an
/// artifact's own bytecode reproduces the payload byte for byte.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=7CE21F
// Broiler-Falsified-If: an emitted value-form unit is not the encoding of JsValueLayout.Layout for the same image and residency, or writes a byte sequence the value table does not admit
// Broiler-Human:        PENDING
internal static class JsX64ValueEmitter
{
    /// <summary>What an artifact whose value form would not fit the format's ceiling is told.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A5B72C
    // Broiler-Human:        PENDING
    internal const string CeilingRefusal =
        "the value form's emitted code would exceed the format's native-code ceiling";

    /// <summary>Emits every code unit of the image, or refuses the whole artifact and says why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=D227C8
    // Broiler-Falsified-If: an emission is produced in which some code unit has no entry point, or the bytes exceed the format's native-code ceiling
    // Broiler-Human:        PENDING
    internal static bool TryEmit(
        JsNativeProgramImage image,
        JsX64Abi abi,
        uint alignment,
        out byte[] code,
        out JsNativeSymbolRow[] symbols,
        out string refusal)
    {
        code = [];
        symbols = [];

        var assembler = new JsX64Assembler();
        var table = new JsNativeSymbolRow[image.Functions.Length];
        var handlerOffsets = JsBaselineBlocks.GroupHandlerOffsets(image);

        for (var index = 0; index < table.Length; index++)
        {
            assembler.AlignTo((int)alignment);
            table[index] = new JsNativeSymbolRow((uint)index, (uint)assembler.Position);

            if (!EmitUnit(assembler, image, index, handlerOffsets.Of(index), abi, out refusal))
            {
                if (!string.Equals(refusal, CeilingRefusal, System.StringComparison.Ordinal))
                {
                    refusal = "code unit " + index + ": " + refusal;
                }

                return false;
            }
        }

        if ((uint)assembler.Position > JsFormat.CeilingNativeCodeBytes)
        {
            refusal = CeilingRefusal;
            return false;
        }

        if (!assembler.TryFinish(out code, out refusal))
        {
            code = [];
            return false;
        }

        symbols = table;
        refusal = string.Empty;
        return true;
    }

    /// <summary>Emits one unit: the prologue, the unit's value layout, and the epilogue.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=859ACC
    // Broiler-Falsified-If: the unit's body is not one template per entry of JsValueLayout.Layout in its order, or a branch site is left unpatched or patched to a position other than that of the entry its target names
    // Broiler-Human:        PENDING
    private static bool EmitUnit(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        int index,
        System.ReadOnlySpan<uint> handlerOffsets,
        JsX64Abi abi,
        out string refusal)
    {
        if (!JsValueLayout.TryPlan(image, index, handlerOffsets, image.ResidentBindings, out var plan, out refusal))
        {
            return false;
        }

        var layout = JsValueLayout.Layout(plan);

        // NO TEMPLATE A LAYOUT NAMES IS SHORTER THAN TWO BYTES, so a layout of more entries than half the
        // ceiling's bytes would pass the ceiling whatever it holds, and is refused before it is written.
        if (assembler.Position + (2L * layout.Length) > JsFormat.CeilingNativeCodeBytes)
        {
            refusal = CeilingRefusal;
            return false;
        }

        var leave = layout.Length;
        var bound = new int[layout.Length + 1];
        var sites = new int[layout.Length];

        // ---- the prologue ------------------------------------------------------------------------
        assembler.Push(JsX64Register.Rbx);
        assembler.Push(JsX64Register.R14);
        assembler.Push(JsX64Register.R15);
        assembler.Push(JsX64Register.R12);
        assembler.SubRspImm8((sbyte)JsValueAbi.FrameBytes(abi.Architecture));
        assembler.MovRegisterRegister(JsX64Register.R14, abi.FramePointerRegister);
        assembler.MovRbxFromR14();
        assembler.MovRegisterMemory(JsX64Register.R15, JsX64Register.R14, JsValueAbi.RegionOffset);
        assembler.MovRegisterMemory(JsX64Register.R12, JsX64Register.R14, JsValueAbi.DebtOffset);
        assembler.MovEaxFromArgument1(abi.SecondArgumentRegister);

        // ---- the layout ----------------------------------------------------------------------------
        for (var at = 0; at < layout.Length; at++)
        {
            bound[at] = assembler.Position;
            sites[at] = Encode(assembler, abi, layout[at]);

            if (sites[at] == Unwritten)
            {
                refusal =
                    "the value layout names a template this emitter does not write, which is a defect in the " +
                    "emitter rather than in the program being compiled";

                return false;
            }

            if ((uint)assembler.Position > JsFormat.CeilingNativeCodeBytes)
            {
                refusal = CeilingRefusal;
                return false;
            }
        }

        // ---- the epilogue --------------------------------------------------------------------------
        bound[leave] = assembler.Position;
        assembler.AddRspImm8((sbyte)JsValueAbi.FrameBytes(abi.Architecture));
        assembler.Pop(JsX64Register.R12);
        assembler.Pop(JsX64Register.R15);
        assembler.Pop(JsX64Register.R14);
        assembler.Pop(JsX64Register.Rbx);
        assembler.Ret();

        for (var at = 0; at < layout.Length; at++)
        {
            if (sites[at] < 0)
            {
                continue;
            }

            var target = layout[at].Target;

            if (target != JsBaselineInstruction.Leave && (uint)target >= (uint)layout.Length)
            {
                refusal =
                    "the emitter branched to a label it never bound, which is a defect in the " +
                    "emitter rather than in the program being compiled";

                return false;
            }

            assembler.PatchRel32(sites[at], bound[target == JsBaselineInstruction.Leave ? leave : target]);
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>What <see cref="Encode"/> answers for an entry that is not a branch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E40900
    // Broiler-Human:        PENDING
    private const int NoSite = -1;

    /// <summary>What <see cref="Encode"/> answers for a template it does not write.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=011360
    // Broiler-Human:        PENDING
    private const int Unwritten = -2;

    /// <summary>
    /// Writes one entry's template, answering the site of its displacement for a branch, <see cref="NoSite"/>
    /// otherwise, and <see cref="Unwritten"/> for a template this emitter has no encoding for.
    /// </summary>
    /// <remarks>
    /// <b>EVERY ENCODING HERE IS ONE ROW OF THE VALUE TABLE, WITH ITS REGISTERS FIXED</b>; the rows the
    /// assembler already spells are written through it, and the rest through its byte door with the row's
    /// derivation beside it. The template scan's closure rows compile real programs and scan what this
    /// writes, which is what holds the two to each other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=3A42CF
    // Broiler-Falsified-If: an encoding here differs from the value-table row of the same name, or a branch answers a site that is not the first byte of its displacement
    // Broiler-Human:        PENDING
    private static int Encode(JsX64Assembler assembler, JsX64Abi abi, JsValueInstruction entry)
    {
        var operand = entry.Operand;

        switch (entry.Template)
        {
            case JsValueTemplate.TestEax:
                assembler.TestRegister32(JsX64Register.Rax, JsX64Register.Rax);
                return NoSite;

            case JsValueTemplate.JsLeave:
                return assembler.JccRel32(JsX64JumpCondition.Sign);

            case JsValueTemplate.CmpEaxPc:
                assembler.CmpEaxImm32((int)operand);
                return NoSite;

            case JsValueTemplate.Je:
                return assembler.JccRel32(JsX64JumpCondition.Equal);

            case JsValueTemplate.Jne:
                return assembler.JccRel32(JsX64JumpCondition.NotEqual);

            case JsValueTemplate.Ja:
                return assembler.JccRel32(JsX64JumpCondition.Above);

            case JsValueTemplate.Jae:
                return assembler.JccRel32(JsX64JumpCondition.AboveOrEqual);

            case JsValueTemplate.Jp:
                return assembler.JccRel32(JsX64JumpCondition.Parity);

            case JsValueTemplate.Jmp:
                return assembler.JmpRel32();

            case JsValueTemplate.MovStatus:
                assembler.MovEaxImm32((int)operand);
                return NoSite;

            case JsValueTemplate.MovArg0R14:
                assembler.MovRegisterRegister(abi.FramePointerRegister, JsX64Register.R14);
                return NoSite;

            case JsValueTemplate.MovArg1Pc:
                assembler.MovArgument1Imm32(abi.SecondArgumentRegister, (int)operand);
                return NoSite;

            case JsValueTemplate.CallSlot:
                assembler.CallRbxDisp32((int)operand);
                return NoSite;

            case JsValueTemplate.CallSettle:
                assembler.CallRbxDisp32(JsValueAbi.SettleSlot * 8);
                return NoSite;

            case JsValueTemplate.SpillDebt:
                assembler.MovMemoryRegister(JsX64Register.R14, JsValueAbi.DebtOffset, JsX64Register.R12);
                return NoSite;

            case JsValueTemplate.ClearDebt:
                // xor r12d, r12d: REX.RB 31 /r, ModRM 0xE4 = 0xC0 | (100 << 3) | 100.
                Bytes(assembler, 0x45, 0x31, 0xE4);
                return NoSite;

            case JsValueTemplate.IncDebt:
                // inc r12: REX.WB FF /0, ModRM 0xC4.
                Bytes(assembler, 0x49, 0xFF, 0xC4);
                return NoSite;

            case JsValueTemplate.DecDebt:
                // dec r12: REX.WB FF /1, ModRM 0xCC.
                Bytes(assembler, 0x49, 0xFF, 0xCC);
                return NoSite;

            case JsValueTemplate.CmpDebt:
                // cmp r12, imm32: REX.WB 81 /7, ModRM 0xFC.
                Bytes(assembler, 0x49, 0x81, 0xFC);
                assembler.EmitInt32((int)operand);
                return NoSite;

            case JsValueTemplate.LoadRax:
                assembler.MovRegisterMemory(JsX64Register.Rax, JsX64Register.R15, (int)operand);
                return NoSite;

            case JsValueTemplate.LoadRdx:
                assembler.MovRegisterMemory(JsX64Register.Rdx, JsX64Register.R15, (int)operand);
                return NoSite;

            case JsValueTemplate.StoreRax:
                assembler.MovMemoryRegister(JsX64Register.R15, (int)operand, JsX64Register.Rax);
                return NoSite;

            case JsValueTemplate.StoreRdx:
                assembler.MovMemoryRegister(JsX64Register.R15, (int)operand, JsX64Register.Rdx);
                return NoSite;

            case JsValueTemplate.MovRaxWord:
                assembler.MovRegisterImmediate64(JsX64Register.Rax, operand);
                return NoSite;

            case JsValueTemplate.MovRcxWord:
                assembler.MovRegisterImmediate64(JsX64Register.Rcx, operand);
                return NoSite;

            case JsValueTemplate.CmpRaxRcx:
                assembler.CmpRegisterRegister(JsX64Register.Rax, JsX64Register.Rcx);
                return NoSite;

            case JsValueTemplate.CmpRdxRcx:
                assembler.CmpRegisterRegister(JsX64Register.Rdx, JsX64Register.Rcx);
                return NoSite;

            case JsValueTemplate.MovqXmm0Rax:
                assembler.MovqToXmm(0, JsX64Register.Rax);
                return NoSite;

            case JsValueTemplate.MovqXmm1Rdx:
                assembler.MovqToXmm(1, JsX64Register.Rdx);
                return NoSite;

            case JsValueTemplate.MovqXmm1Rcx:
                assembler.MovqToXmm(1, JsX64Register.Rcx);
                return NoSite;

            case JsValueTemplate.MovqRaxXmm0:
                assembler.MovqFromXmm(JsX64Register.Rax, 0);
                return NoSite;

            case JsValueTemplate.Addsd:
                assembler.Addsd(0, 1);
                return NoSite;

            case JsValueTemplate.Subsd:
                assembler.Subsd(0, 1);
                return NoSite;

            case JsValueTemplate.Mulsd:
                assembler.Mulsd(0, 1);
                return NoSite;

            case JsValueTemplate.Divsd:
                assembler.Divsd(0, 1);
                return NoSite;

            case JsValueTemplate.Ucomisd01:
                assembler.Ucomisd(0, 1);
                return NoSite;

            case JsValueTemplate.Ucomisd10:
                assembler.Ucomisd(1, 0);
                return NoSite;

            case JsValueTemplate.XorpdXmm1:
                assembler.Xorpd(1, 1);
                return NoSite;

            case JsValueTemplate.SetccAl:
                assembler.SetCondition((JsX64Condition)(int)operand, JsX64Register.Rax);
                return NoSite;

            case JsValueTemplate.SetccDl:
                assembler.SetCondition((JsX64Condition)(int)operand, JsX64Register.Rdx);
                return NoSite;

            case JsValueTemplate.AndAlDl:
                assembler.And8(JsX64Register.Rax, JsX64Register.Rdx);
                return NoSite;

            case JsValueTemplate.OrAlDl:
                assembler.Or8(JsX64Register.Rax, JsX64Register.Rdx);
                return NoSite;

            case JsValueTemplate.MovzxEaxAl:
                assembler.MovZeroExtend8To32(JsX64Register.Rax, JsX64Register.Rax);
                return NoSite;

            case JsValueTemplate.AddRaxRcx:
                // add rax, rcx: REX.W 01 /r, ModRM 0xC8 = 0xC0 | (001 << 3) | 000.
                Bytes(assembler, 0x48, 0x01, 0xC8);
                return NoSite;

            case JsValueTemplate.XorRaxRcx:
                assembler.XorRegisterRegister(JsX64Register.Rax, JsX64Register.Rcx);
                return NoSite;

            case JsValueTemplate.XorRax1:
                // xor rax, 1: REX.W 83 /6 ib, ModRM 0xF0.
                Bytes(assembler, 0x48, 0x83, 0xF0, 0x01);
                return NoSite;

            case JsValueTemplate.Cvttsd2siEax:
                // cvttsd2si eax, xmm0: F2 0F 2C /r, ModRM 0xC0.
                Bytes(assembler, 0xF2, 0x0F, 0x2C, 0xC0);
                return NoSite;

            case JsValueTemplate.Cvttsd2siEcx:
                // cvttsd2si ecx, xmm1: ModRM 0xC9 = 0xC0 | (001 << 3) | 001.
                Bytes(assembler, 0xF2, 0x0F, 0x2C, 0xC9);
                return NoSite;

            case JsValueTemplate.CmpEaxMin:
                // cmp eax, 0x80000000: 81 /7 id, ModRM 0xF8.
                Bytes(assembler, 0x81, 0xF8, 0x00, 0x00, 0x00, 0x80);
                return NoSite;

            case JsValueTemplate.CmpEcxMin:
                Bytes(assembler, 0x81, 0xF9, 0x00, 0x00, 0x00, 0x80);
                return NoSite;

            case JsValueTemplate.OrEaxEcx:
                // or, and and xor eax, ecx: 09, 21 and 31 /r, ModRM 0xC8.
                Bytes(assembler, 0x09, 0xC8);
                return NoSite;

            case JsValueTemplate.AndEaxEcx:
                Bytes(assembler, 0x21, 0xC8);
                return NoSite;

            case JsValueTemplate.XorEaxEcx:
                Bytes(assembler, 0x31, 0xC8);
                return NoSite;

            case JsValueTemplate.ShlEaxCl:
                // shl, sar and shr eax, cl: D3 /4, /7 and /5.
                Bytes(assembler, 0xD3, 0xE0);
                return NoSite;

            case JsValueTemplate.SarEaxCl:
                Bytes(assembler, 0xD3, 0xF8);
                return NoSite;

            case JsValueTemplate.ShrEaxCl:
                Bytes(assembler, 0xD3, 0xE8);
                return NoSite;

            case JsValueTemplate.NotEax:
                // not eax: F7 /2.
                Bytes(assembler, 0xF7, 0xD0);
                return NoSite;

            case JsValueTemplate.MovsxdRaxEax:
                // movsxd rax, eax: REX.W 63 /r.
                Bytes(assembler, 0x48, 0x63, 0xC0);
                return NoSite;

            case JsValueTemplate.Cvtsi2sd:
                assembler.Cvtsi2sd(0, JsX64Register.Rax);
                return NoSite;

            // ---- the direct call (stage JSV-3); each is the value-table row of its name ------------------
            case JsValueTemplate.LeaArg2Callee:
                if (Windows(abi))
                {
                    // lea r8, [rsp+32]: REX.WR 8D /r, ModRM 0x44, SIB 0x24, disp8.
                    Bytes(assembler, 0x4C, 0x8D, 0x44, 0x24, (byte)JsValueAbi.CalleeOffset(abi.Architecture));
                }
                else
                {
                    // mov rdx, rsp: REX.W 89 /r, ModRM 0xE2.
                    Bytes(assembler, 0x48, 0x89, 0xE2);
                }

                return NoSite;

            case JsValueTemplate.CallPrepare:
                assembler.CallRbxDisp32(JsValueAbi.PrepareSlot * 8);
                return NoSite;

            case JsValueTemplate.CallFinish:
                assembler.CallRbxDisp32(JsValueAbi.FinishSlot * 8);
                return NoSite;

            case JsValueTemplate.CmpEaxDirect:
                assembler.CmpEaxImm32(JsValueAbi.DirectCall);
                return NoSite;

            case JsValueTemplate.LeaArg0Callee:
                if (Windows(abi))
                {
                    // lea rcx, [rsp+32]: REX.W 8D /r, ModRM 0x4C, SIB 0x24, disp8.
                    Bytes(assembler, 0x48, 0x8D, 0x4C, 0x24, (byte)JsValueAbi.CalleeOffset(abi.Architecture));
                }
                else
                {
                    // mov rdi, rsp: REX.W 89 /r, ModRM 0xE7.
                    Bytes(assembler, 0x48, 0x89, 0xE7);
                }

                return NoSite;

            case JsValueTemplate.MovArg1EntryPc:
                // mov edx or mov esi, [rsp+disp8]: 8B /r, ModRM 0x54 or 0x74, SIB 0x24.
                Bytes(
                    assembler,
                    0x8B,
                    Windows(abi) ? (byte)0x54 : (byte)0x74,
                    0x24,
                    (byte)(JsValueAbi.CalleeOffset(abi.Architecture) + JsValueAbi.EntryPcOffset));

                return NoSite;

            case JsValueTemplate.CallEntry:
                // call qword [rsp+disp8]: FF /2, ModRM 0x54, SIB 0x24.
                Bytes(
                    assembler,
                    0xFF,
                    0x54,
                    0x24,
                    (byte)(JsValueAbi.CalleeOffset(abi.Architecture) + JsValueAbi.EntryOffset));

                return NoSite;

            case JsValueTemplate.MovReturned:
                assembler.MovEaxImm32(JsValueAbi.Returned);
                return NoSite;

            case JsValueTemplate.MovArg3Status:
                if (Windows(abi))
                {
                    // mov r9d, eax: REX.B 89 /r, ModRM 0xC1.
                    Bytes(assembler, 0x41, 0x89, 0xC1);
                }
                else
                {
                    // mov ecx, eax: 89 /r, ModRM 0xC1.
                    Bytes(assembler, 0x89, 0xC1);
                }

                return NoSite;

            default:
                return Unwritten;
        }
    }

    /// <summary>Whether the row is Windows x64's, whose argument registers and shadow space differ.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0A6D05
    // Broiler-Human:        PENDING
    private static bool Windows(JsX64Abi abi) => abi.Architecture == JsNativeArchitecture.X64Windows;

    /// <summary>Writes a fixed encoding through the assembler's byte door.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0C9176
    // Broiler-Human:        PENDING
    private static void Bytes(JsX64Assembler assembler, params byte[] bytes)
    {
        foreach (var value in bytes)
        {
            assembler.Emit(value);
        }
    }
}
