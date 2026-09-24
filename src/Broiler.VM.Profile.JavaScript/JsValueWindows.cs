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
/// <b>THE REGION IS WHERE THE WORDS LIVE, AND THE ACTIVATION'S STACK IS ONLY THE ARM'S WINDOW ONTO IT.</b>
/// A region holds the activation's arguments, then its resident bindings, then its operand stack, where
/// its unit's plan says (<see cref="Format.JsValueLayout"/>). Inline templates read and write those words
/// directly and no managed code sees them do it, so at stage JSV-2 the activation's stack is coherent with
/// the region only inside the window a helper decodes: an arm reads and writes <c>JsValue</c>s exactly as
/// the interpreter's arm does, the words it reads are decoded into that stack first, and what it leaves is
/// encoded back.
/// </para>
/// <para>
/// <b>THE READ WINDOW IS THE INSTRUCTION'S POPS AND EVERY WORD BELOW THEM IT READS</b>, named in
/// <see cref="ReadDepth"/>, and the whole operand stack in a unit that can suspend: its frame is the
/// activation's own stack, so whatever it suspends with has to have been decoded. The write window starts
/// at the lowest slot the instruction pops, or at the slot a landing pushed its value into, and ends at the
/// height the step stopped at.
/// </para>
/// <para>
/// <b>THE HEIGHT A HELPER STARTS AT IS THE PLAN'S</b>, because the inline code before it moved the height
/// without telling anybody; it is published before the safepoint, so a compaction scans every word the
/// inline code left live. Under handle-stress the safepoint compacts and every freed entry's generation
/// moves, so a word the scan failed to root decodes to a generation mismatch and fails by name.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=3F70A3
// Broiler-Falsified-If: an arm reads a stack slot below its read window, a suspending unit's helper leaves a slot of its stack undecoded, a word the arm wrote is left unencoded or unpublished at the step's end, a word outside the write window is overwritten, or a height is published after a safepoint that could compact
// Broiler-Human:        PENDING
internal static class JsValueWindows
{
    /// <summary>
    /// Encodes a newly opened region: the arguments, the resident bindings and the activation's stack; and
    /// publishes it.
    /// </summary>
    /// <remarks>
    /// <b>It runs once per entry, before any helper</b>: an ordinary call's stack is empty, and a resumed
    /// generator's holds what it suspended with and the value the resumption sent. The function's own
    /// environment, when it is resident, is read from the record the call filled with the parameters; every
    /// other resident environment has not been entered yet, and its words start as the empty word a fresh
    /// record's slots are.
    /// </remarks>
    /// <param name="act">The activation, whose region is open.</param>
    /// <param name="handles">The instance's handle table.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=C1E1CD
    // Broiler-Falsified-If: an argument word, a resident word or a slot below the activation's height is not encoded into its region before the region is published at that height
    // Broiler-Human:        PENDING
    internal static void Open(JsNativeActivation act, JsHandleTable handles)
    {
        var plan = act.Plan!;
        var segment = act.Segment!;
        var words = segment.Words;
        var first = JsValueSlab.FirstWord(act.SlabFrame);

        for (var argument = 0; argument < plan.ArgumentWords; argument++)
        {
            words[first + argument] = argument < act.Arguments.Length
                ? JsWordCodec.Encode(act.Arguments[argument], handles)
                : JsWord.Undefined;
        }

        for (var depth = 0; depth < plan.Depths; depth++)
        {
            var at = plan.ResidentBaseOf(depth);

            if (at < 0)
            {
                continue;
            }

            var own = depth == 0 && act.Scopes.Count > 0 ? act.Scopes[0].Slots : [];

            for (var slot = 0; slot < plan.ResidentSlotsOf(depth); slot++)
            {
                words[first + at + slot] = slot < own.Length
                    ? JsWordCodec.Encode(own[slot], handles)
                    : JsWord.Empty;
            }
        }

        var stackBase = first + plan.StackBase;

        for (var slot = 0; slot < act.Sp; slot++)
        {
            words[stackBase + slot] = JsWordCodec.Encode(act.Stack[slot], handles);
        }

        segment.Publish(act.SlabFrame, plan.StackBase + act.Sp);
    }

    /// <summary>
    /// The helper's entry: the height the plan gives, published, then the safepoint, then the instruction's
    /// input words decoded into the stack; it answers how many of them the instruction pops, which is where
    /// <see cref="Leave"/> encodes from.
    /// </summary>
    /// <remarks>
    /// <b>A DELEGATION THAT RESUMES INSIDE ITS OWN INSTRUCTION POPS AND READS NOTHING.</b> A <c>yield*</c>
    /// pops its iterable when it starts and keeps the iterator in the frame, and every resumption re-enters
    /// the same instruction at the height the first entry left, one below the verifier's; so a frame that is
    /// delegating when the step starts is at that height, read from nowhere and written from there up.
    /// </remarks>
    /// <param name="act">The activation.</param>
    /// <param name="pc">The instruction's offset, a reached instruction start of the activation's unit.</param>
    /// <param name="height">The stack's height before the instruction, as the plan gives it.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=80886E
    // Broiler-Falsified-If: a slot of the read window is left holding anything but its word's decoding, a word is decoded before the safepoint, the height the safepoint scans is not the plan's, or the activation's program counter and height are not the instruction's when the arm starts
    // Broiler-Human:        PENDING
    internal static int Enter(JsNativeActivation act, int pc, out int height)
    {
        var plan = act.Plan!;
        var handles = act.Engine.ValueHandles!;
        var resumed = act.Frame is { Delegating: true } && act.Code[pc] == (byte)JsOpcode.YieldDelegate;

        height = plan.HeightAt(pc) - (resumed ? 1 : 0);

        if (height < 0 || height >= act.Stack.Length)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form instruction starts outside its frame's operand stack");
        }

        act.Pc = pc;
        act.Sp = height;

        var segment = act.Segment!;
        segment.Publish(act.SlabFrame, plan.StackBase + height);
        handles.Safepoint();

        var pops = resumed ? 0 : Pops(act.Code, pc);
        var from = plan.WholeStack ? 0 : height - (resumed ? 0 : ReadDepth(act.Code, pc));

        if (from < 0)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form instruction reads below its frame's operand stack");
        }

        var words = segment.Words;
        var stackBase = JsValueSlab.FirstWord(act.SlabFrame) + plan.StackBase;
        var stack = act.Stack;

        for (var slot = from; slot < height; slot++)
        {
            var word = words[stackBase + slot];

            // A NUMBER IS DECODED HERE, WHERE IT IS ONE SHIFT AND ONE COMPARE, and every other word by the
            // codec; the two answer the same value for a Number word, which is what the codec's own
            // Number arm is.
            stack[slot] = JsWord.IsNumber(word)
                ? JsValue.Number(JsWord.ToNumber(word))
                : JsWordCodec.Decode(word, handles);
        }

        return pops;
    }

    /// <summary>
    /// The helper's exit: every slot the step wrote encoded into the region, a newly pushed resident
    /// environment's words emptied, and the new height published.
    /// </summary>
    /// <remarks>
    /// <b>A <c>PushScope</c> THAT OPENS A RESIDENT ENVIRONMENT EMPTIES ITS WORDS</b>, because the record the
    /// arm pushed starts with every slot empty and the region's words for that depth are the record's slots:
    /// a binding the block declares is in its dead zone again every time the block is entered.
    /// </remarks>
    /// <param name="act">The activation.</param>
    /// <param name="pc">The instruction's offset.</param>
    /// <param name="height">The stack's height before the instruction.</param>
    /// <param name="pops">What <see cref="Enter"/> answered: how many slots the instruction popped.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=65BD6E
    // Broiler-Falsified-If: a slot from the lowest one the step could write up to the height it stopped at is not encoded before that height is published, a slot below that lowest one is written, or a resident environment a PushScope opened keeps a word of an earlier record
    // Broiler-Human:        PENDING
    internal static void Leave(JsNativeActivation act, int pc, int height, int pops)
    {
        var plan = act.Plan!;
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
        var stackBase = first + plan.StackBase;
        var stack = act.Stack;

        for (var slot = from; slot < after; slot++)
        {
            words[stackBase + slot] = JsWordCodec.Encode(stack[slot], handles);
        }

        if (!act.Landed && act.Code[pc] == (byte)JsOpcode.PushScope)
        {
            var depth = plan.DepthAt(pc) + 1;
            var at = plan.ResidentBaseOf(depth);

            for (var slot = 0; at >= 0 && slot < plan.ResidentSlotsOf(depth); slot++)
            {
                words[first + at + slot] = JsWord.Empty;
            }
        }

        segment.Publish(act.SlabFrame, plan.StackBase + after);
    }

    /// <summary>The value an inline return left in the activation's first region word (stage JSV-3).</summary>
    /// <param name="act">The activation, whose region is still open.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=F0E783
    // Broiler-Falsified-If: it decodes any word but the region's first, or decodes after the region closed
    // Broiler-Human:        PENDING
    internal static JsValue Returned(JsNativeActivation act) =>
        JsWordCodec.Decode(act.Segment!.Words[JsValueSlab.FirstWord(act.SlabFrame)], act.Engine.ValueHandles!);

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
