// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           13
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  5/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The pass that decides, before a single instruction is encoded, what an emitted unit's frame
/// looks like at every reachable bytecode offset.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE WHOLE POINT OF THIS PASS IS THAT THE EMITTER AFTERWARDS HAS NO DECISIONS LEFT TO
/// MAKE.</b> It answers four questions per offset - is it reachable, how deep is the operand stack
/// there, which flat slot does a scoped access mean, and does a basic block start here - and once
/// they are answered every instruction's template is a fixed sequence of words with fixed
/// immediates. An emitter that computed these while encoding would be deciding a frame layout and
/// a branch target in the same loop, and the failure mode of getting that wrong is a load from the
/// wrong slot, which is a well-formed instruction.
/// </para>
/// <para>
/// <b>THE SCOPE CHAIN IS FLATTENED INTO ONE SLAB, AND THE REASON THAT IS SOUND IS A PROPERTY OF THE
/// MANIFEST RATHER THAN OF THE BACKEND.</b> The lowering pushes an environment per block and copies
/// it per loop iteration because a closure could capture one; the numeric manifest admits no
/// function expression, no arrow function and no nested function declaration, so nothing in an
/// admitted program can capture anything, and an environment that nothing can capture is
/// indistinguishable from a region of one frame. Widen the manifest to admit a closure and this
/// flattening stops being sound - which is why the two facts are documented against each other
/// rather than left as an assumption inside an emitter.
/// </para>
/// <para>
/// <b>A JOIN WHOSE TWO ARRIVALS DISAGREE IS REFUSED AND NOT MERGED.</b> Two paths reaching one
/// offset with different operand heights, different scope shapes or different sets of initialised
/// slots would mean the emitted templates at that offset are right for one path and wrong for the
/// other. A merge that took the intersection would be the beginning of a dataflow framework this
/// backend does not have and could not test; a refusal is a sentence an author can read and a
/// program a reader can rewrite.
/// </para>
/// <para>
/// <b>And a read of a slot no path has written is refused too</b>, because a slab of doubles has no
/// dead-zone representation. The language answers a read before a <c>let</c> is initialised with a
/// <c>ReferenceError</c>; emitted code over a raw slab would answer with whatever the slab held,
/// which is the previous iteration's value or nothing at all.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=BEBD3B
// Broiler-Falsified-If: an offset this pass reports a height or a flat slot for is reached by a path on which that height or that slot is different
// Broiler-Human:        PENDING
internal sealed class JsArm64Walk
{
    private readonly JsAssembledProgram program;

    private readonly int start;

    private readonly int end;

    private readonly int[] height;

    private readonly int[] flat;

    private readonly int[][] scopes;

    private readonly bool[][] initialised;

    private readonly bool[] leader;

    private readonly int[] cost;

    private readonly int parameters;

    private readonly int ownSlots;

    /// <summary>Prepares a walk over one code unit's range.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C3847A
    // Broiler-Human:        PENDING
    internal JsArm64Walk(JsAssembledProgram program, JsFunctionRow row)
    {
        this.program = program;
        start = (int)row.CodeOffset;
        end = start + (int)row.CodeLength;
        parameters = (int)row.ParameterCount;
        ownSlots = (int)row.ScopeSlots;

        var span = end - start;
        height = new int[span];
        flat = new int[span];
        scopes = new int[span][];
        initialised = new bool[span][];
        leader = new bool[span];
        cost = new int[span];

        for (var offset = 0; offset < span; offset++)
        {
            height[offset] = -1;
            flat[offset] = -1;
        }
    }

    /// <summary>Whether the emitter should encode the instruction at <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=78A608
    // Broiler-Human:        PENDING
    internal bool IsReachable(int at) => height[at - start] >= 0;

    /// <summary>Whether a basic block starts at <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=486AAF
    // Broiler-Human:        PENDING
    internal bool IsLeader(int at) => leader[at - start];

    /// <summary>How many bytecode instructions the block starting at <paramref name="at"/> holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=20298A
    // Broiler-Human:        PENDING
    internal int BlockCost(int at) => cost[at - start];

    /// <summary>The operand-stack height on entry to the instruction at <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7A9763
    // Broiler-Human:        PENDING
    internal int HeightAt(int at) => height[at - start];

    /// <summary>The flat local slot the scoped instruction at <paramref name="at"/> addresses.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3B8A7B
    // Broiler-Human:        PENDING
    internal int FlatSlot(int at) => flat[at - start];

    /// <summary>How many flat local slots this unit's emitted code addresses.</summary>
    /// <remarks>
    /// <b>IT IS THE SIZE OF THE SLAB A CALLER WOULD HAVE TO SUPPLY, AND NOTHING IN THIS REPOSITORY
    /// CHECKS THAT A CALLER DID.</b> Nothing calls an emitted arm64 unit, so the obligation has no
    /// enforcement point yet; it is computed and published here so that the obligation is a number
    /// a future caller can read rather than a fact somebody would have to re-derive from the
    /// emitter.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=108210
    // Broiler-Falsified-If: an emitted instruction addresses a local slot at or beyond this count
    // Broiler-Human:        PENDING
    internal int LocalSlots { get; private set; }

    /// <summary>How many operand slots this unit's emitted code addresses.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8DEADB
    // Broiler-Human:        PENDING
    internal int OperandSlots { get; private set; }

    /// <summary>
    /// Traces every path through the unit, or refuses and says which offset disagreed with itself.
    /// </summary>
    /// <remarks>
    /// <b>It is a worklist and not a linear scan, because a backward branch reaches an offset the
    /// scan has already passed.</b> The queue holds offsets whose entry state is known and whose
    /// instruction has not been simulated; an offset already recorded is compared rather than
    /// re-simulated, which is what makes a loop terminate and what makes a disagreement at a join
    /// visible.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=338F05
    // Broiler-Falsified-If: this method answers true for a unit in which some reachable offset is reached with two different entry states
    // Broiler-Human:        PENDING
    internal bool TryTrace(out string refusal)
    {
        var pending = new System.Collections.Generic.Queue<(int At, int[] Scopes, bool[] Slots, int Height)>();
        var entryScopes = new[] { ownSlots };
        var entrySlots = new bool[ownSlots];

        for (var slot = 0; slot < parameters && slot < ownSlots; slot++)
        {
            entrySlots[slot] = true;
        }

        pending.Enqueue((start, entryScopes, entrySlots, 0));
        leader[0] = true;

        while (pending.Count != 0)
        {
            var (at, atScopes, atSlots, atHeight) = pending.Dequeue();

            if (at < start || at >= end)
            {
                refusal = "a branch leaves the unit's own code range, reaching offset " + at;
                return false;
            }

            if (!JsOpcodes.IsDefined(program.Code[at]))
            {
                refusal = "offset " + at + " is not the first byte of an instruction this build defines";
                return false;
            }

            if (height[at - start] >= 0)
            {
                if (height[at - start] != atHeight ||
                    !Same(scopes[at - start], atScopes) ||
                    !Same(initialised[at - start], atSlots))
                {
                    refusal =
                        "offset " + at + " is reached with two different frame states, and this " +
                        "backend refuses such a join rather than merging it, because the templates " +
                        "it would emit there are right for one arrival and wrong for the other";

                    return false;
                }

                continue;
            }

            height[at - start] = atHeight;
            scopes[at - start] = atScopes;
            initialised[at - start] = atSlots;
            OperandSlots = System.Math.Max(OperandSlots, atHeight);
            LocalSlots = System.Math.Max(LocalSlots, Total(atScopes));

            if (!Step(at, atScopes, atSlots, atHeight, pending, out refusal))
            {
                return false;
            }
        }

        Blocks();
        refusal = string.Empty;
        return true;
    }

    /// <summary>
    /// Simulates one instruction's effect on the frame and queues the offsets it can reach.
    /// </summary>
    /// <remarks>
    /// <b>IT REFUSES THE THINGS AN ENCODING CANNOT SAY, AND LEAVES THE THINGS AN ENCODING HAS NO
    /// TEMPLATE FOR TO THE EMITTER.</b> A slot past the reach of a twelve-bit scaled immediate, a
    /// scope depth deeper than the chain, a constant that is not a Number and a read of an
    /// uninitialised slot are all refused here, because they are properties of the frame rather
    /// than of the instruction; an opcode with no template is refused by the emitter, where the
    /// sentence that names it lives.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=76E929
    // Broiler-Falsified-If: this method reports a frame effect for an instruction that differs from what this profile's interpreter does to its operand stack and scopes
    // Broiler-Human:        PENDING
    private bool Step(
        int at,
        int[] atScopes,
        bool[] atSlots,
        int atHeight,
        System.Collections.Generic.Queue<(int At, int[] Scopes, bool[] Slots, int Height)> pending,
        out string refusal)
    {
        var opcode = (JsOpcode)program.Code[at];
        var next = at + JsOpcodes.InstructionWidth(opcode);
        refusal = string.Empty;

        switch (opcode)
        {
            case JsOpcode.LoadUndefined:
                return Push(at, atScopes, atSlots, atHeight, next, pending, out refusal);

            case JsOpcode.Duplicate:
                return Depth(at, atHeight, 1, out refusal) &&
                    Push(at, atScopes, atSlots, atHeight, next, pending, out refusal);

            case JsOpcode.LoadConstant:
            {
                var pool = program.Code[at + 1] | (program.Code[at + 2] << 8);

                if (pool >= program.Constants.Count)
                {
                    refusal = "offset " + at + " names constant " + pool + ", which the pool has not";
                    return false;
                }

                if (pool > JsArm64Backend.SlotReach)
                {
                    refusal =
                        "offset " + at + " names constant " + pool + ", which is past the reach of " +
                        "the twelve-bit scaled immediate a load carries";

                    return false;
                }

                var entry = program.Constants[pool];

                if (entry.Length == 0 || entry[0] != (byte)JsFormat.ConstantTag.Number)
                {
                    refusal =
                        "offset " + at + " loads constant " + pool + ", which is not a Number, and " +
                        "a slab of doubles has no representation for one";

                    return false;
                }

                return Push(at, atScopes, atSlots, atHeight, next, pending, out refusal);
            }

            case JsOpcode.LoadScoped:
            case JsOpcode.StoreScoped:
            case JsOpcode.InitialiseScoped:
            {
                var depth = program.Code[at + 1];
                var slot = program.Code[at + 2] | (program.Code[at + 3] << 8);

                if (depth >= atScopes.Length)
                {
                    refusal =
                        "offset " + at + " reaches " + depth + " environments out and the chain " +
                        "here is " + atScopes.Length + " deep";

                    return false;
                }

                var which = atScopes.Length - 1 - depth;

                if (slot >= atScopes[which])
                {
                    refusal =
                        "offset " + at + " names slot " + slot + " of an environment declaring " +
                        atScopes[which];

                    return false;
                }

                var index = Base(atScopes, which) + slot;

                if (index > JsArm64Backend.SlotReach)
                {
                    refusal =
                        "offset " + at + " reaches flat local slot " + index + ", which is past the " +
                        "reach of the twelve-bit scaled immediate a load carries";

                    return false;
                }

                flat[at - start] = index;

                if (opcode == JsOpcode.LoadScoped)
                {
                    if (!atSlots[index])
                    {
                        refusal =
                            "offset " + at + " reads flat local slot " + index + " on a path that " +
                            "has not written it, and a slab of doubles has no dead-zone value to " +
                            "answer such a read with";

                        return false;
                    }

                    return Push(at, atScopes, atSlots, atHeight, next, pending, out refusal);
                }

                var written = (bool[])atSlots.Clone();
                written[index] = true;
                pending.Enqueue((next, atScopes, written, atHeight - 1));
                return Depth(at, atHeight, 1, out refusal);
            }

            case JsOpcode.PushScope:
            {
                var declared = program.Code[at + 1] | (program.Code[at + 2] << 8);
                var grown = new int[atScopes.Length + 1];
                atScopes.CopyTo(grown, 0);
                grown[^1] = declared;

                var total = Total(grown);

                if (total > JsArm64Backend.SlotReach + 1)
                {
                    refusal =
                        "offset " + at + " pushes an environment that takes the unit's flat slot " +
                        "count to " + total + ", which is past the reach of a twelve-bit scaled " +
                        "immediate";

                    return false;
                }

                var widened = new bool[total];
                atSlots.CopyTo(widened, 0);
                pending.Enqueue((next, grown, widened, atHeight));
                return true;
            }

            case JsOpcode.PopScope:
            {
                if (atScopes.Length == 1)
                {
                    refusal = "offset " + at + " pops the unit's own environment";
                    return false;
                }

                var shrunk = new int[atScopes.Length - 1];
                System.Array.Copy(atScopes, shrunk, shrunk.Length);
                var kept = new bool[Total(shrunk)];
                System.Array.Copy(atSlots, kept, kept.Length);
                pending.Enqueue((next, shrunk, kept, atHeight));
                return true;
            }

            case JsOpcode.CopyScope:
            {
                var declared = program.Code[at + 1] | (program.Code[at + 2] << 8);

                if (declared != atScopes[^1])
                {
                    refusal =
                        "offset " + at + " copies an environment of " + declared + " slots over " +
                        "one of " + atScopes[^1];

                    return false;
                }

                pending.Enqueue((next, atScopes, atSlots, atHeight));
                return true;
            }

            case JsOpcode.Add:
            case JsOpcode.Subtract:
            case JsOpcode.Multiply:
            case JsOpcode.Divide:
            case JsOpcode.LessThan:
            case JsOpcode.LessThanOrEqual:
            case JsOpcode.GreaterThan:
            case JsOpcode.GreaterThanOrEqual:
                if (!Depth(at, atHeight, 2, out refusal))
                {
                    return false;
                }

                pending.Enqueue((next, atScopes, atSlots, atHeight - 1));
                return true;

            case JsOpcode.Negate:
            case JsOpcode.Not:
            case JsOpcode.ToNumber:
                if (!Depth(at, atHeight, 1, out refusal))
                {
                    return false;
                }

                pending.Enqueue((next, atScopes, atSlots, atHeight));
                return true;

            case JsOpcode.Pop:
                if (!Depth(at, atHeight, 1, out refusal))
                {
                    return false;
                }

                pending.Enqueue((next, atScopes, atSlots, atHeight - 1));
                return true;

            case JsOpcode.Jump:
                Mark(next);
                pending.Enqueue((Target(at), atScopes, atSlots, atHeight));
                Mark(Target(at));
                return true;

            case JsOpcode.JumpIfFalse:
            case JsOpcode.JumpIfTrue:
                if (!Depth(at, atHeight, 1, out refusal))
                {
                    return false;
                }

                pending.Enqueue((Target(at), atScopes, atSlots, atHeight - 1));
                pending.Enqueue((next, atScopes, atSlots, atHeight - 1));
                Mark(Target(at));
                Mark(next);
                return true;

            case JsOpcode.Return:
                if (!Depth(at, atHeight, 1, out refusal))
                {
                    return false;
                }

                Mark(next);
                return true;

            case JsOpcode.ReturnUndefined:
                Mark(next);
                return true;

            // AN INSTRUCTION THIS BACKEND HAS NO TEMPLATE FOR STOPS THE TRACE AND DOES NOT GUESS
            // WHAT IT WOULD HAVE DONE TO THE STACK. Continuing past it would mean inventing an
            // operand-height effect for an instruction whose meaning this pass does not implement,
            // and every offset after it would carry a height derived from that invention. The
            // offset itself stays reachable, so the emitter meets it and refuses it by name, which
            // is where the sentence that explains it lives.
            default:
                return true;
        }
    }

    /// <summary>Queues the next offset with one more operand on the stack, or refuses the height.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8231AC
    // Broiler-Human:        PENDING
    private bool Push(
        int at,
        int[] atScopes,
        bool[] atSlots,
        int atHeight,
        int next,
        System.Collections.Generic.Queue<(int At, int[] Scopes, bool[] Slots, int Height)> pending,
        out string refusal)
    {
        if (atHeight + 1 > JsArm64Backend.SlotReach)
        {
            refusal =
                "offset " + at + " pushes the operand stack to " + (atHeight + 1) +
                ", which is past the reach of the twelve-bit scaled immediate a store carries";

            return false;
        }

        refusal = string.Empty;
        pending.Enqueue((next, atScopes, atSlots, atHeight + 1));
        return true;
    }

    /// <summary>Refuses an instruction that would pop more operands than the stack holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4FE861
    // Broiler-Human:        PENDING
    private static bool Depth(int at, int atHeight, int needed, out string refusal)
    {
        if (atHeight >= needed)
        {
            refusal = string.Empty;
            return true;
        }

        refusal =
            "offset " + at + " takes " + needed + " operands and the stack there holds " + atHeight;

        return false;
    }

    /// <summary>The absolute code offset a branch at <paramref name="at"/> names.</summary>
    /// <remarks>
    /// <b>Version 2 of this format writes ABSOLUTE code offsets rather than displacements</b>, which
    /// is the single property that lets a backend derive a block map without reconstructing where
    /// each branch came from.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=55688F
    // Broiler-Human:        PENDING
    private int Target(int at) =>
        program.Code[at + 1] |
        (program.Code[at + 2] << 8) |
        (program.Code[at + 3] << 16) |
        (program.Code[at + 4] << 24);

    /// <summary>Records that a basic block starts at <paramref name="at"/>, if it is in range.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=16D674
    // Broiler-Human:        PENDING
    private void Mark(int at)
    {
        if (at >= start && at < end)
        {
            leader[at - start] = true;
        }
    }

    /// <summary>Counts how many bytecode instructions each block holds, which is what fuel costs.</summary>
    /// <remarks>
    /// <b>Only REACHABLE instructions are counted, because only reachable instructions are
    /// emitted.</b> A block whose tail the lowering left dead would otherwise charge for
    /// instructions no emitted word corresponds to, which would make the two forms disagree about
    /// fuel for a reason that has nothing to do with the difference this backend is entitled to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B90416
    // Broiler-Human:        PENDING
    private void Blocks()
    {
        var current = -1;

        for (var at = start; at < end;)
        {
            var width = JsOpcodes.InstructionWidth((JsOpcode)program.Code[at]);

            if (height[at - start] >= 0)
            {
                if (leader[at - start])
                {
                    current = at;
                }

                if (current >= 0)
                {
                    cost[current - start]++;
                }
            }

            at += width;
        }
    }

    /// <summary>Where one environment's slots start in the unit's flat slab.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5AAEAB
    // Broiler-Human:        PENDING
    private static int Base(int[] sizes, int which)
    {
        var total = 0;

        for (var index = 0; index < which; index++)
        {
            total += sizes[index];
        }

        return total;
    }

    /// <summary>How many flat slots a whole scope chain occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ED38D3
    // Broiler-Human:        PENDING
    private static int Total(int[] sizes) => Base(sizes, sizes.Length);

    /// <summary>Whether two recorded scope chains are the same chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9BFF9F
    // Broiler-Human:        PENDING
    private static bool Same(int[] left, int[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        for (var index = 0; index < left.Length; index++)
        {
            if (left[index] != right[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Whether two recorded initialisation sets are the same set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C9B2A1
    // Broiler-Human:        PENDING
    private static bool Same(bool[] left, bool[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        for (var index = 0; index < left.Length; index++)
        {
            if (left[index] != right[index])
            {
                return false;
            }
        }

        return true;
    }
}
