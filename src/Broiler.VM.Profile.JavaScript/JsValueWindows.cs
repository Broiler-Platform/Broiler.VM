// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           0
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Critical
// Criteria:         13/13
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// What a value helper adds to an arm: the words an instruction reads, decoded into the activation's
/// stack before the arm runs, and the words it wrote, encoded back and published after (JSD-0035
/// section 5).
/// </summary>
/// <remarks>
/// <para>
/// <b>THE ACTIVATION'S STACK IS THE MIRROR, AND THE SLAB IS WHERE THE WORDS LIVE.</b> An arm reads and
/// writes <c>JsValue</c>s in the activation's own stack, exactly as the interpreter's arm does; the
/// words it reads are decoded into that stack first, and what it leaves is encoded back, so every value
/// that crosses an instruction goes through the codec and the handle table. At stage JSV-1 every
/// instruction is a helper, so the mirror and the slab hold the same values below the stack's height at
/// every step boundary, and a generator's frame, whose stack the mirror is, suspends with the right
/// values; the frame codec that keeps that true once inline templates write words the mirror never
/// sees is stage JSV-4's.
/// </para>
/// <para>
/// <b>THE READ WINDOW IS THE INSTRUCTION'S POPS AND EVERY WORD BELOW THEM IT READS.</b> Most arms read
/// exactly what they pop; the ones that read a value which stays - the object under a definition, the
/// Array under an append, the record under an asynchronous step, the constructor and home object under a
/// class element, and what a duplication or a pick copies - are named in <see cref="ReadDepth"/>. The
/// write window starts at the lowest slot the instruction pops, or at the slot a landing pushed its value
/// into, and ends at the height the step stopped at.
/// </para>
/// <para>
/// <b>UNDER HANDLE-STRESS EVERY DECODED WORD IS ALSO COMPARED WITH THE MIRROR</b>, which at this stage
/// still holds the value the word was encoded from: a word that decodes to another kind, another
/// reference or another Number (a NaN's payload aside) is an internal defect by name. So a codec or
/// publication mistake fails the variant it happens in rather than surviving as a wrong answer that a
/// coherent mirror would hide.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=3F70A3
// Broiler-Falsified-If: an arm reads a stack slot below its read window, a word the arm wrote is left unencoded or unpublished at the step's end, or a word outside the write window is overwritten
// Broiler-Human:        PENDING
internal static class JsValueWindows
{
    /// <summary>Encodes the activation's whole stack into its newly opened region and publishes it.</summary>
    /// <remarks>
    /// <b>It runs once per entry, before any helper</b>: an ordinary call's stack is empty, and a resumed
    /// generator's holds what it suspended with and the value the resumption sent.
    /// </remarks>
    /// <param name="act">The activation, whose region is open.</param>
    /// <param name="handles">The instance's handle table.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=BCC649
    // Broiler-Falsified-If: a slot below the activation's height is not encoded into its region before the region is published at that height
    // Broiler-Human:        PENDING
    internal static void Open(JsNativeActivation act, JsHandleTable handles)
    {
        var segment = act.Segment!;
        var words = segment.Words;
        var first = JsValueSlab.FirstWord(act.SlabFrame);

        for (var slot = 0; slot < act.Sp; slot++)
        {
            words[first + slot] = JsWordCodec.Encode(act.Stack[slot], handles);
        }

        segment.Publish(act.SlabFrame, act.Sp);
    }

    /// <summary>
    /// The helper's entry: the safepoint, then the instruction's input words decoded into the stack; it
    /// answers how many of them the instruction pops, which is where <see cref="Leave"/> encodes from.
    /// </summary>
    /// <remarks>
    /// <b>A DELEGATION THAT RESUMES INSIDE ITS OWN INSTRUCTION POPS AND READS NOTHING.</b> A
    /// <c>yield*</c> pops its iterable when it starts and keeps the iterator in the frame, and every
    /// resumption re-enters the same instruction at the height the first entry left, one below the
    /// verifier's; so a frame that is delegating when the step starts is read from nowhere and written
    /// from that height up.
    /// </remarks>
    /// <param name="act">The activation.</param>
    /// <param name="pc">The instruction's offset.</param>
    /// <param name="height">The stack's height before the instruction, which is its published live length.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=85C17C
    // Broiler-Falsified-If: a slot of the read window is left holding anything but its word's decoding, a word is decoded before the safepoint, or under handle-stress a word decoding to another value than the mirror's passes
    // Broiler-Human:        PENDING
    internal static int Enter(JsNativeActivation act, int pc, int height)
    {
        var handles = act.Engine.ValueHandles!;
        handles.Safepoint();

        var resumed = act.Frame is { Delegating: true } && act.Code[pc] == (byte)JsOpcode.YieldDelegate;
        var pops = resumed ? 0 : Pops(act.Code, pc);
        var from = height - (resumed ? 0 : ReadDepth(act.Code, pc));
        var stress = handles.Stress;

        if (from < 0)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form instruction reads below its frame's operand stack");
        }

        var words = act.Segment!.Words;
        var first = JsValueSlab.FirstWord(act.SlabFrame);
        var stack = act.Stack;

        for (var slot = from; slot < height; slot++)
        {
            var word = words[first + slot];

            // A NUMBER IS DECODED HERE, WHERE IT IS ONE SHIFT AND ONE COMPARE, and every other word by the
            // codec; the two answer the same value for a Number word, which is what the codec's own
            // Number arm is.
            var decoded = JsWord.IsNumber(word)
                ? JsValue.Number(JsWord.ToNumber(word))
                : JsWordCodec.Decode(word, handles);

            if (stress && !Same(decoded, stack[slot]))
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect,
                    "a value-form word decodes to a value other than the one the interpreter's stack holds there");
            }

            stack[slot] = decoded;
        }

        return pops;
    }

    /// <summary>The helper's exit: every slot the step wrote encoded into the region, and the new height published.</summary>
    /// <param name="act">The activation.</param>
    /// <param name="height">The stack's height before the instruction.</param>
    /// <param name="pops">What <see cref="Enter"/> answered: how many slots the instruction popped.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=FE76D7
    // Broiler-Falsified-If: a slot from the lowest one the step could write up to the height it stopped at is not encoded before that height is published, or a slot below that lowest one is written
    // Broiler-Human:        PENDING
    internal static void Leave(JsNativeActivation act, int height, int pops)
    {
        var handles = act.Engine.ValueHandles!;
        var after = act.Sp;

        // A LANDING WROTE ONE SLOT, AT THE REGION'S HEIGHT, and that can be below the instruction's own
        // inputs; every other step wrote from its lowest popped slot upward.
        var from = act.Landed ? after - 1 : height - pops;

        if (from < 0 || after > act.Stack.Length)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form step left its operand stack outside its region");
        }

        var segment = act.Segment!;
        var words = segment.Words;
        var first = JsValueSlab.FirstWord(act.SlabFrame);

        var stack = act.Stack;

        for (var slot = from; slot < after; slot++)
        {
            ref readonly var value = ref stack[slot];

            words[first + slot] = value.Type == JsType.Number
                ? JsWord.FromNumber(value.AsNumber())
                : JsWordCodec.Encode(value, handles);
        }

        segment.Publish(act.SlabFrame, after);
    }

    /// <summary>How many slots below the stack's height the instruction at <paramref name="pc"/> reads.</summary>
    /// <param name="code">The code section.</param>
    /// <param name="pc">The instruction's offset.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=5C2997
    // Broiler-Falsified-If: for some opcode the arm reads a slot deeper than the depth this answers
    // Broiler-Human:        PENDING
    internal static int ReadDepth(byte[] code, int pc)
    {
        var opcode = (JsOpcode)code[pc];
        var fixedDepth = FixedReads[(int)opcode];

        if (fixedDepth >= 0)
        {
            return fixedDepth;
        }

        var operand = Operand(code, pc, opcode);
        return Depth(opcode, operand, Pops(opcode, operand));
    }

    /// <summary>The read depth of one instruction, given its operand and its pop count.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=790494
    // Broiler-Falsified-If: for some opcode the arm reads a slot deeper than the depth this answers
    // Broiler-Human:        PENDING
    private static int Depth(JsOpcode opcode, uint operand, int pops)
    {
        var peeks = opcode switch
        {
            // What a duplication or a pick copies, and the one value an arm reads without popping.
            JsOpcode.Duplicate or JsOpcode.ArrayHoles or JsOpcode.RunStaticElements or
                JsOpcode.IterateNextAsync => 1,
            JsOpcode.DuplicateTwo => 2,
            JsOpcode.Pick => checked((int)operand + 1),

            // The object, the Array or the scope that stays under what is popped.
            JsOpcode.DefineField or JsOpcode.DefineGetter or JsOpcode.DefineSetter or
                JsOpcode.SetPrototypeLiteral or JsOpcode.ArrayAppend or JsOpcode.SpreadArray or
                JsOpcode.SpreadObject or JsOpcode.DisposeAdd => pops + 1,
            JsOpcode.DefineIndexed or JsOpcode.DefineMethod => pops + 1,

            // The constructor and the home object under the key and the value.
            JsOpcode.DefineClassElement => pops + 2,
            _ => 0,
        };

        return System.Math.Max(pops, peeks);
    }

    /// <summary>How many slots the instruction at <paramref name="pc"/> pops, as the verifier counted them.</summary>
    /// <param name="code">The code section.</param>
    /// <param name="pc">The instruction's offset.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0FA930
    // Broiler-Falsified-If: it answers other than JsOpcodes.TryDescribe's pops for the instruction and its operand
    // Broiler-Human:        PENDING
    internal static int Pops(byte[] code, int pc)
    {
        var opcode = (JsOpcode)code[pc];
        var fixedPops = FixedPops[(int)opcode];
        return fixedPops >= 0 ? fixedPops : Pops(opcode, Operand(code, pc, opcode));
    }

    /// <summary>Each opcode's pop count where its operand does not move it, and minus one where it does.</summary>
    /// <remarks>
    /// <b>Built from <see cref="JsOpcodes.TryDescribe"/> and nothing else</b>: an opcode is fixed when two
    /// operands far apart give the same count, and every opcode whose count is read off its operand -
    /// a call's arguments, an array's elements, a class's heritage - gives two different ones. An
    /// undefined byte is minus one, so it reaches <see cref="Pops(JsOpcode, uint)"/>'s refusal.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FF9330
    // Broiler-Falsified-If: an entry other than minus one differs from TryDescribe's pops for some operand of that opcode
    // Broiler-Human:        PENDING
    private static readonly int[] FixedPops = BuildFixed(reads: false);

    /// <summary>Each opcode's read depth where its operand does not move it, and minus one where it does.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=63C412
    // Broiler-Falsified-If: an entry other than minus one differs from the read depth the slow path answers for some operand of that opcode
    // Broiler-Human:        PENDING
    private static readonly int[] FixedReads = BuildFixed(reads: true);

    /// <summary>Builds one of the two fixed tables by asking the slow path at two operands far apart.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D2B3E8
    // Broiler-Falsified-If: an opcode whose count varies with its operand gets an entry other than minus one
    // Broiler-Human:        PENDING
    private static int[] BuildFixed(bool reads)
    {
        var table = new int[256];

        for (var value = 0; value < table.Length; value++)
        {
            var opcode = (JsOpcode)value;

            if (!JsOpcodes.IsDefined((byte)value) ||
                !JsOpcodes.TryDescribe(opcode, 0, out var low, out _) ||
                !JsOpcodes.TryDescribe(opcode, 1, out var one, out _) ||
                !JsOpcodes.TryDescribe(opcode, 200, out var high, out _) ||
                low != high || low != one || opcode == JsOpcode.Pick)
            {
                table[value] = -1;
                continue;
            }

            table[value] = reads ? Depth(opcode, 0, low) : low;
        }

        return table;
    }

    /// <summary>Whether two values are the same value, a NaN's payload aside.</summary>
    /// <param name="left">One value.</param>
    /// <param name="right">The other.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=2DA5C0
    // Broiler-Falsified-If: it answers true for two values of different kinds, different references, different Booleans or Numbers whose bits differ and are not both NaN
    // Broiler-Human:        PENDING
    internal static bool Same(in JsValue left, in JsValue right)
    {
        if (left.Type != right.Type)
        {
            return false;
        }

        return left.Type switch
        {
            JsType.Number =>
                System.BitConverter.DoubleToInt64Bits(left.AsNumber()) ==
                    System.BitConverter.DoubleToInt64Bits(right.AsNumber()) ||
                (double.IsNaN(left.AsNumber()) && double.IsNaN(right.AsNumber())),
            JsType.Boolean => left.AsBoolean() == right.AsBoolean(),
            JsType.String => ReferenceEquals(left.AsString(), right.AsString()),
            JsType.Object => ReferenceEquals(left.AsObject(), right.AsObject()),
            JsType.Symbol => ReferenceEquals(left.AsSymbol(), right.AsSymbol()),
            JsType.BigInt => ReferenceEquals(left.AsBigInt(), right.AsBigInt()),
            _ => true,
        };
    }

    /// <summary>The verifier's pop count, which a verified instruction always has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=BA50DF
    // Broiler-Falsified-If: it answers a count for an opcode TryDescribe does not describe
    // Broiler-Human:        PENDING
    private static int Pops(JsOpcode opcode, uint operand) =>
        JsOpcodes.TryDescribe(opcode, operand, out var pops, out _)
            ? pops
            : throw new JsAbort(JsAbortKind.InternalDefect, "a value helper ran an undefined opcode");

    /// <summary>The instruction's operand, decoded as the verifier decodes it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B4C746
    // Broiler-Falsified-If: for some operand shape it answers other than the verifier's decoding of the same bytes
    // Broiler-Human:        PENDING
    private static uint Operand(byte[] code, int pc, JsOpcode opcode) =>
        JsOpcodes.Shape(opcode) switch
        {
            JsOperandShape.U8 => code[pc + 1],
            JsOperandShape.U16 => (uint)(code[pc + 1] | (code[pc + 2] << 8)),
            JsOperandShape.U32 => (uint)(
                code[pc + 1] |
                (code[pc + 2] << 8) |
                (code[pc + 3] << 16) |
                (code[pc + 4] << 24)),
            JsOperandShape.U8U16 => (uint)(
                (code[pc + 1] << 16) | code[pc + 2] | (code[pc + 3] << 8)),
            _ => 0,
        };
}
