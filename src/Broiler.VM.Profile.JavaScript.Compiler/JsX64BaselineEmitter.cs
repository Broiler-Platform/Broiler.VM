// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          None
// Security risk:    Critical
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The emitter of the wide manifest's baseline form for x86-64: every code unit, every instruction,
/// one handler call each, with the control flow between instructions emitted.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE EMITTED CODE DECIDES ONLY WHICH HANDLER TO CALL NEXT, AND EVERYTHING A PROGRAM CAN
/// OBSERVE HAPPENS INSIDE THE HANDLER.</b> A handler runs exactly one bytecode instruction through
/// the interpreter's own dispatch for it - the same fuel charge, the same arm, the same exception
/// search - and answers the program counter the interpreter would have gone on to, or a negative
/// status. The unit follows that answer: straight on to the next instruction's call when it is the
/// next instruction, a direct branch when it is the instruction's static target, and a compare tree
/// over the unit's landing offsets for anything else. Nothing here reads a value, computes a
/// value, or knows what an opcode does beyond whether it has a code target and whether it ends a
/// block, which is why no construct of the wide manifest is refused.
/// </para>
/// <para>
/// <b>AN UNEXPECTED ANSWER IS A DEFECT AND NEVER A DIFFERENT ANSWER.</b> A program counter that is
/// neither the fall-through nor the static target goes to the compare tree; one that is not a
/// landing of the unit either materialises <see cref="JsBaselineStatus.Defect"/> and leaves. The
/// handler at the other end refuses any program counter the managed side did not compute, so a
/// mistake in the landing set shows up as an internal defect on the first program that reaches it.
/// </para>
/// <para>
/// <b>THE LANDING SET IS EVERY OFFSET THE MANAGED SIDE CAN HAND BACK THAT IS NOT A NEXT OR A TARGET.</b>
/// A unit's own entry; every handler offset of an exception region of the unit, which is where a
/// caught throw puts the program counter; the instruction after each <c>Yield</c> and <c>Await</c>
/// and after <c>EnterBody</c>, which is where a normal resume re-enters; and each
/// <c>YieldDelegate</c> itself, which a delegating resume runs again.
/// </para>
/// <para>
/// <b>IT IS A PURE FUNCTION OF THE IMAGE.</b> It reads the code, the function rows, the regions and
/// the convention row, walks every unit in bytecode order and sorts every set it builds, so the
/// verifier's re-emission of an artifact's own bytecode reproduces the payload byte for byte.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=4CA3A6
// Broiler-Falsified-If: an emitted unit transfers control to a handler other than the one for the opcode at the program counter it passes, lands on an offset the managed side did not answer, or emits different bytes for the same image
// Broiler-Human:        PENDING
internal static class JsX64BaselineEmitter
{
    /// <summary>What an artifact whose baseline form would not fit the format's ceiling is told.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=25B021
    // Broiler-Human:        PENDING
    internal const string CeilingRefusal =
        "the baseline form's emitted code would exceed the format's native-code ceiling";

    /// <summary>How many compares a leaf of the landing tree makes before it gives up.</summary>
    /// <remarks>
    /// <b>Four, because a chain of four costs about what one more level of the tree would</b>, and
    /// a unit with no suspension and no handler has a single landing, its entry.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0B88FA
    // Broiler-Human:        PENDING
    private const int LeafLandings = 4;

    /// <summary>Emits every code unit of the image, or refuses the whole artifact and says why.</summary>
    /// <remarks>
    /// <b>ONE REFUSED UNIT REFUSES THE ARTIFACT, and verified bytecode reaches only one refusal.</b>
    /// The structural refusals below - an undefined opcode, an instruction past its unit's end, a
    /// target or a landing that is not an instruction start - are all things the verifier has
    /// already refused, and are here so that a producer's defect answers a sentence rather than a
    /// wrong branch. The one a verified program can reach is the ceiling: the form is about eleven
    /// bytes of machine code per bytecode byte, and an artifact past the format's native-code ceiling
    /// is refused whole rather than emitted in part.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=2FA82F
    // Broiler-Falsified-If: an emission is produced in which some code unit has no entry point, some instruction has no handler call, or the bytes exceed the format's native-code ceiling
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

        for (var index = 0; index < table.Length; index++)
        {
            assembler.AlignTo((int)alignment);
            table[index] = new JsNativeSymbolRow((uint)index, (uint)assembler.Position);

            if (!EmitUnit(assembler, image, index, abi, out refusal))
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
    /// Emits one unit: the prologue, the dispatch over its landings, the defect block, one block per
    /// instruction in bytecode order, and the epilogue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE LABELS ARE AN ARRAY INDEXED BY BYTECODE OFFSET AND THE BRANCHES ARE A LIST OF SITES.</b>
    /// Every instruction start is bound, reachable or not, because every instruction is emitted; the
    /// three blocks and the tree's right-hand subtrees take the indices past the unit's length. The
    /// sites are patched once the unit is complete, and a site whose label nothing bound refuses the
    /// artifact as an emitter defect.
    /// </para>
    /// <para>
    /// <b>EVERY INSTRUCTION'S TAIL IS CHOSEN BY TWO PREDICATES OF THE OPCODE TABLE AND NOTHING
    /// ELSE.</b> An instruction with a code target compares against it and branches there directly;
    /// one that does not end its block compares against the next instruction and falls into its
    /// block, or goes to the dispatch when it is the unit's last; one that ends its block goes to the
    /// dispatch. No opcode is named here except the four that define landings.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=9C8358
    // Broiler-Falsified-If: a unit's block for an instruction calls a slot other than eight times that instruction's opcode, passes a program counter other than that instruction's offset, or a branch site is left unpatched
    // Broiler-Human:        PENDING
    private static bool EmitUnit(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        int index,
        JsX64Abi abi,
        out string refusal)
    {
        var row = image.Functions[index];
        var bytecode = image.Code;
        var first = (long)row.CodeOffset;
        var end = first + row.CodeLength;

        if (row.CodeLength == 0 || end > bytecode.Length)
        {
            refusal = "the unit's code range is empty or runs past the artifact's code";
            return false;
        }

        var length = (int)row.CodeLength;
        var starts = new bool[length];

        // THE FIRST WALK FINDS THE INSTRUCTION STARTS AND THE LANDINGS, and emits nothing: the
        // compare tree comes before the instructions in the unit and names offsets the walk has to
        // have validated first.
        var landings = new System.Collections.Generic.List<long> { first };

        for (var at = first; at < end;)
        {
            if (!JsOpcodes.IsDefined(bytecode[at]))
            {
                refusal = "the byte at " + at + " is not an instruction this format version defines";
                return false;
            }

            var opcode = (JsOpcode)bytecode[at];
            var width = JsOpcodes.InstructionWidth(opcode);

            if (at + width > end)
            {
                refusal = "the `" + opcode + "` at " + at + " runs past the end of its unit";
                return false;
            }

            starts[at - first] = true;

            switch (opcode)
            {
                case JsOpcode.Yield or JsOpcode.Await or JsOpcode.EnterBody:
                    landings.Add(at + width);
                    break;

                case JsOpcode.YieldDelegate:
                    landings.Add(at);
                    break;
            }

            at += width;
        }

        foreach (var region in image.Regions)
        {
            if (region.FunctionIndex == (uint)index)
            {
                landings.Add(region.HandlerOffset);
            }
        }

        landings.Sort();
        var sorted = new System.Collections.Generic.List<int>(landings.Count);

        foreach (var landing in landings)
        {
            if (landing < first || landing >= end || !starts[landing - first])
            {
                refusal =
                    "the offset " + landing + " is a landing of the unit and not the start of one " +
                    "of its instructions";

                return false;
            }

            if (sorted.Count == 0 || sorted[^1] != (int)landing)
            {
                sorted.Add((int)landing);
            }
        }

        // Labels 0..length-1 are bytecode offsets; the three blocks follow, then one per subtree.
        var dispatch = length;
        var defect = length + 1;
        var leave = length + 2;
        var bound = new int[length + 3 + sorted.Count];
        System.Array.Fill(bound, -1);
        var next = length + 3;
        var sites = new System.Collections.Generic.List<(int Site, int Label)>();

        // ---- the prologue ------------------------------------------------------------------------
        assembler.Push(JsX64Register.Rbx);
        assembler.Push(JsX64Register.R14);
        assembler.SubRspImm8((sbyte)abi.BaselineFrameBytes);
        assembler.MovRegisterRegister(JsX64Register.R14, abi.FramePointerRegister);
        assembler.MovRbxFromR14();
        assembler.MovEaxFromArgument1(abi.SecondArgumentRegister);

        // ---- the dispatch: a negative answer leaves, anything else is looked up ------------------
        bound[dispatch] = assembler.Position;
        assembler.TestRegister32(JsX64Register.Rax, JsX64Register.Rax);
        sites.Add((assembler.JccRel32(JsX64JumpCondition.Sign), leave));
        Tree(assembler, sorted, 0, sorted.Count - 1, (int)first, defect, bound, ref next, sites);

        // ---- the defect: an answer with no landing ------------------------------------------------
        bound[defect] = assembler.Position;
        assembler.MovEaxImm32((int)JsBaselineStatus.Defect);
        sites.Add((assembler.JmpRel32(), leave));

        // ---- one block per instruction, in bytecode order ----------------------------------------
        for (var at = (int)first; at < end;)
        {
            var opcode = (JsOpcode)bytecode[at];
            var width = JsOpcodes.InstructionWidth(opcode);
            var following = at + width;
            var atEnd = following == end;

            bound[at - (int)first] = assembler.Position;
            assembler.MovRegisterRegister(abi.FramePointerRegister, JsX64Register.R14);
            assembler.MovArgument1Imm32(abi.SecondArgumentRegister, at);
            assembler.CallRbxDisp32((int)opcode * 8);

            if (JsOpcodes.HasCodeTarget(opcode))
            {
                var target = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(
                    System.MemoryExtensions.AsSpan(bytecode, at + 1));

                if (target < first || target >= end || !starts[target - first])
                {
                    refusal =
                        "the `" + opcode + "` at " + at + " targets " + target + ", which is not " +
                        "the start of an instruction of its unit";

                    return false;
                }

                assembler.CmpEaxImm32((int)target);
                sites.Add((assembler.JccRel32(JsX64JumpCondition.Equal), (int)(target - first)));
            }

            if (JsOpcodes.IsTerminal(opcode) || atEnd)
            {
                sites.Add((assembler.JmpRel32(), dispatch));
            }
            else
            {
                assembler.CmpEaxImm32(following);
                sites.Add((assembler.JccRel32(JsX64JumpCondition.NotEqual), dispatch));
            }

            if ((uint)assembler.Position > JsFormat.CeilingNativeCodeBytes)
            {
                refusal = CeilingRefusal;
                return false;
            }

            at = following;
        }

        // ---- the epilogue --------------------------------------------------------------------------
        bound[leave] = assembler.Position;
        assembler.AddRspImm8((sbyte)abi.BaselineFrameBytes);
        assembler.Pop(JsX64Register.R14);
        assembler.Pop(JsX64Register.Rbx);
        assembler.Ret();

        foreach (var (site, label) in sites)
        {
            if (bound[label] < 0)
            {
                refusal =
                    "the emitter branched to a label it never bound, which is a defect in the " +
                    "emitter rather than in the program being compiled";

                return false;
            }

            assembler.PatchRel32(site, bound[label]);
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>The compare tree over a sorted range of a unit's landings.</summary>
    /// <remarks>
    /// <b>A LEAF IS A CHAIN AND AN INNER NODE SPLITS ON ITS MIDDLE, WITH AN UNSIGNED COMPARE.</b> The
    /// dispatch has already sent every negative answer out of the unit, so what reaches the tree is
    /// a non-negative offset and <c>ja</c> orders it correctly. A chain ends in the defect, so an
    /// offset the unit has no landing for never falls into whatever block happens to follow.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=CB5D94
    // Broiler-Falsified-If: an offset in the landing set is dispatched anywhere but its own instruction's block, or an offset outside it anywhere but the defect block
    // Broiler-Human:        PENDING
    private static void Tree(
        JsX64Assembler assembler,
        System.Collections.Generic.List<int> landings,
        int low,
        int high,
        int first,
        int defect,
        int[] bound,
        ref int next,
        System.Collections.Generic.List<(int Site, int Label)> sites)
    {
        if (high - low + 1 <= LeafLandings)
        {
            for (var position = low; position <= high; position++)
            {
                assembler.CmpEaxImm32(landings[position]);
                sites.Add((assembler.JccRel32(JsX64JumpCondition.Equal), landings[position] - first));
            }

            sites.Add((assembler.JmpRel32(), defect));
            return;
        }

        var middle = (low + high) / 2;
        var right = next++;

        assembler.CmpEaxImm32(landings[middle]);
        sites.Add((assembler.JccRel32(JsX64JumpCondition.Equal), landings[middle] - first));
        sites.Add((assembler.JccRel32(JsX64JumpCondition.Above), right));
        Tree(assembler, landings, low, middle - 1, first, defect, bound, ref next, sites);
        bound[right] = assembler.Position;
        Tree(assembler, landings, middle + 1, high, first, defect, bound, ref next, sites);
    }
}
