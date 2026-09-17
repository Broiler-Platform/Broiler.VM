// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          None
// Security risk:    Critical
// Criteria:         3/3
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The emitter of the wide manifest's baseline form for x86-64: every code unit emitted, and a handler
/// call at every block head of it, with the control flow between blocks emitted.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE EMITTED CODE DECIDES ONLY WHICH HANDLER TO CALL NEXT, AND EVERYTHING A PROGRAM CAN
/// OBSERVE HAPPENS INSIDE THE HANDLER.</b> A handler runs one step - an instruction, or a block of
/// them - through the interpreter's own dispatch - the same fuel charge for every instruction, the
/// same arms, the same exception search - and answers the program counter the interpreter would have
/// gone on to, or a negative status. The unit follows that answer: into the next head's call when that
/// is what the block's last instruction falls to, a direct branch to a head it can name, and a compare
/// tree over the unit's landings for anything else. Nothing here reads a value, computes a value, or
/// knows what an opcode does, which is why no construct of the wide manifest is refused.
/// </para>
/// <para>
/// <b>THE PARTITION AND THE LAYOUT ARE <see cref="JsBaselineBlocks"/>'S, AND THIS FILE ENCODES
/// THEM.</b> Which offsets are landings and heads, where each block ends and what its tail compares
/// are the plan's; the order of the templates and the entry each branch goes to are the layout's. This
/// file writes one template per layout entry and patches each branch to the entry it names, so it keeps
/// no landing walk, no compare tree and no leaf size of its own, and the block a step runs and the
/// tail that follows its call are computed from the one stop rule the engine reads.
/// </para>
/// <para>
/// <b>AN UNEXPECTED ANSWER IS A DEFECT AND NEVER A DIFFERENT ANSWER.</b> A program counter that is
/// neither a tail's target nor its successor goes to the compare tree; one that is not a landing of
/// the unit either materialises <see cref="JsBaselineStatus.Defect"/> and leaves. The handler at the
/// other end refuses any program counter the managed side did not compute, so a mistake in the landing
/// set shows up as an internal defect on the first program that reaches it.
/// </para>
/// <para>
/// <b>IT IS A PURE FUNCTION OF THE IMAGE.</b> It reads the code, the function rows, the regions and
/// the convention row, groups the regions' handler offsets by unit once, and plans and lays out every
/// unit in bytecode order, so the verifier's re-emission of an artifact's own bytecode reproduces the
/// payload byte for byte.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=4CA3A6
// Broiler-Falsified-If: an emitted unit transfers control to a handler other than the one for the opcode at the head it passes, lands on an offset the managed side did not answer, or emits bytes other than the encoding of JsBaselineBlocks.Layout for the same image
// Broiler-Human:        PENDING
internal static class JsX64BaselineEmitter
{
    /// <summary>What an artifact whose baseline form would not fit the format's ceiling is told.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=25B021
    // Broiler-Human:        PENDING
    internal const string CeilingRefusal =
        "the baseline form's emitted code would exceed the format's native-code ceiling";

    /// <summary>Emits every code unit of the image, or refuses the whole artifact and says why.</summary>
    /// <remarks>
    /// <para>
    /// <b>ONE REFUSED UNIT REFUSES THE ARTIFACT, and verified bytecode reaches only one refusal.</b>
    /// The structural refusals - an undefined opcode, an instruction past its unit's end, a target or a
    /// landing that is not an instruction start - are the plan's, in the sentences this emitter
    /// answered them in before the plan existed, and are all things the verifier has already refused;
    /// they are there so that a producer's defect answers a sentence rather than a wrong branch. Every
    /// one of them is answered before anything of its unit is written, so a unit that is both malformed
    /// and past the ceiling answers the structural sentence. The one a verified program can reach is the
    /// ceiling: the form is a call and a tail per block head and a compare per landing, and an artifact
    /// past the format's native-code ceiling is refused whole rather than emitted in part.
    /// </para>
    /// <para>
    /// <b>THE HANDLER OFFSETS ARE GROUPED ONCE FOR THE WHOLE IMAGE</b>, and each unit is planned from its
    /// own slice, so emitting an image reads each exception region once rather than once per unit.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=D227C8
    // Broiler-Falsified-If: an emission is produced in which some code unit has no entry point, some block head has no handler call, or the bytes exceed the format's native-code ceiling
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

    /// <summary>
    /// Emits one unit: the prologue, the unit's layout - its dispatch over the landings, the defect
    /// block, and a call and a tail per block head - and the epilogue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>ONE TEMPLATE PER LAYOUT ENTRY, AND A LABEL IS AN ENTRY'S INDEX.</b> Each entry's position is
    /// recorded as it is written, one past the last entry stands for the epilogue's first instruction,
    /// and each branch's site is kept at its entry's index; once the unit is complete every site is
    /// patched to the position of the entry its target names. Both arrays are exactly the layout's
    /// length, and a branch whose target is neither an entry nor the epilogue refuses the artifact as a
    /// defect in the emitter rather than being read as an index.
    /// </para>
    /// <para>
    /// <b>A LAYOUT THAT CANNOT FIT IS REFUSED BEFORE IT IS BUILT.</b> No template a layout names is
    /// shorter than two bytes, so a layout of more entries than half the ceiling's bytes would take the
    /// emission past the ceiling whatever it holds; refusing it then answers what writing it would have
    /// answered, and keeps what a unit allocates bounded by the ceiling rather than by its bytecode.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=FF794B
    // Broiler-Falsified-If: a unit's call for a block head names a slot other than eight times that head's opcode or passes a program counter other than the head's offset, the unit's body is not one template per entry of JsBaselineBlocks.Layout in its order, or a branch site is left unpatched or patched to a position other than that of the entry its target names
    // Broiler-Human:        PENDING
    private static bool EmitUnit(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        int index,
        System.ReadOnlySpan<uint> handlerOffsets,
        JsX64Abi abi,
        out string refusal)
    {
        if (!JsBaselineBlocks.TryPlan(image, index, handlerOffsets, out var plan, out refusal))
        {
            return false;
        }

        if (assembler.Position + (2L * JsBaselineBlocks.LayoutLength(plan)) > JsFormat.CeilingNativeCodeBytes)
        {
            refusal = CeilingRefusal;
            return false;
        }

        var layout = JsBaselineBlocks.Layout(plan);
        var leave = layout.Length;
        var bound = new int[layout.Length + 1];
        var sites = new int[layout.Length];

        // ---- the prologue ------------------------------------------------------------------------
        assembler.Push(JsX64Register.Rbx);
        assembler.Push(JsX64Register.R14);
        assembler.SubRspImm8((sbyte)abi.BaselineFrameBytes);
        assembler.MovRegisterRegister(JsX64Register.R14, abi.FramePointerRegister);
        assembler.MovRbxFromR14();
        assembler.MovEaxFromArgument1(abi.SecondArgumentRegister);

        // ---- the layout: the dispatch, the defect, and a call and a tail per head -----------------
        for (var at = 0; at < layout.Length; at++)
        {
            var entry = layout[at];
            bound[at] = assembler.Position;
            sites[at] = -1;

            switch (entry.Template)
            {
                case JsBaselineTemplate.TestEax:
                    assembler.TestRegister32(JsX64Register.Rax, JsX64Register.Rax);
                    break;

                case JsBaselineTemplate.JsLeave:
                    sites[at] = assembler.JccRel32(JsX64JumpCondition.Sign);
                    break;

                case JsBaselineTemplate.CmpEaxPc:
                    assembler.CmpEaxImm32(entry.Operand);
                    break;

                case JsBaselineTemplate.Je:
                    sites[at] = assembler.JccRel32(JsX64JumpCondition.Equal);
                    break;

                case JsBaselineTemplate.Jne:
                    sites[at] = assembler.JccRel32(JsX64JumpCondition.NotEqual);
                    break;

                case JsBaselineTemplate.Ja:
                    sites[at] = assembler.JccRel32(JsX64JumpCondition.Above);
                    break;

                case JsBaselineTemplate.Jmp:
                    sites[at] = assembler.JmpRel32();
                    break;

                case JsBaselineTemplate.MovStatus:
                    assembler.MovEaxImm32(entry.Operand);
                    break;

                case JsBaselineTemplate.MovArg0R14:
                    assembler.MovRegisterRegister(abi.FramePointerRegister, JsX64Register.R14);
                    break;

                case JsBaselineTemplate.MovArg1Pc:
                    assembler.MovArgument1Imm32(abi.SecondArgumentRegister, entry.Operand);
                    break;

                case JsBaselineTemplate.CallSlot:
                    assembler.CallRbxDisp32(entry.Operand);
                    break;

                default:
                    refusal =
                        "the layout names a template this emitter does not write, which is a defect in " +
                        "the emitter rather than in the program being compiled";

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
        assembler.AddRspImm8((sbyte)abi.BaselineFrameBytes);
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
}
