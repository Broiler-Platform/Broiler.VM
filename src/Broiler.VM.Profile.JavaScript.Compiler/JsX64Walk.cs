// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           12
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  2/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>What the emitter knows about one operand-stack slot without running the program.</summary>
/// <remarks>
/// <b>THREE OF THE FOUR KINDS EXIST TO BE REFUSED SOMEWHERE, AND THAT IS THE POINT OF TRACKING
/// THEM.</b> A slab of <c>double</c>s can hold a Number and nothing else; a Boolean, an
/// <c>undefined</c> and a function are three things this manifest can put on an operand stack that
/// are not Numbers, and the emitter's answer to each is different. A Boolean must be consumed by
/// the branch that follows it, because a Boolean stored into a binding and read back would be
/// indistinguishable from the Number one or zero and <c>(1 &lt; 2) === 1</c> would answer true where
/// the language says false. An <c>undefined</c> may be stored and loaded and returned and may never
/// reach an arithmetic instruction. A function is not a value at run time at all: it is a code unit
/// the emitter resolves the call to, and no slot ever holds one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BF16E6
// Broiler-Falsified-If: a value of a kind other than Number reaches an arithmetic instruction this emitter wrote
// Broiler-Human:        PENDING
internal enum JsX64ValueKind
{
    /// <summary>A Number, which is what a slot physically holds.</summary>
    Number = 0,

    /// <summary>The result of a comparison or a logical negation.</summary>
    Boolean = 1,

    /// <summary>The one value this manifest admits that is not a Number.</summary>
    Undefined = 2,

    /// <summary>A function the program declared, named by its code-unit index.</summary>
    Function = 3,
}

/// <summary>One operand-stack slot as the emitter models it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3AF0E2
// Broiler-Human:        PENDING
internal readonly record struct JsX64Value(JsX64ValueKind Kind, int Unit);

/// <summary>
/// The static walk of one code unit: what the operand stack, the scope chain and the slot kinds are
/// at every reachable bytecode offset.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE SCOPE CHAIN IS FLATTENED INTO ONE SLAB, AND WHAT MAKES THAT SOUND IS THE MANIFEST AND
/// NOTHING ELSE.</b> The bytecode's scopes are a linked chain of environment records because in the
/// wide language a closure can capture one and outlive the block it belongs to. The numeric
/// manifest admits no function expression, no arrow function and no nested function declaration, so
/// NOTHING CAN CAPTURE A SCOPE, so a scope's lifetime is exactly the block it was pushed for and
/// the chain is a stack whose shape is decided by the code rather than by the run. That lets a
/// depth-and-index pair be resolved here, once, to a fixed slot; and it makes
/// <see cref="JsOpcode.CopyScope"/> - the per-iteration copy a <c>let</c> in a <c>for</c> header
/// needs - emit no instruction at all, because the only observer a copy exists for is a closure
/// this manifest does not admit. WIDEN THE MANIFEST AND THIS FILE STOPS BEING TRUE.
/// </para>
/// <para>
/// <b>Merging states requires EQUALITY and does not compute a join.</b> Two paths reaching one
/// offset with different stack heights, different scope shapes or different slot kinds is a program
/// this emitter refuses rather than one it approximates. A join would be the beginning of a data
/// flow analysis, and an analysis that is subtly wrong emits code that is subtly wrong - which is
/// the one defect class a verifier of machine code cannot catch.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6C19B6
// Broiler-Falsified-If: a depth-and-index pair resolves here to a slot other than the one the interpreter's scope chain would reach
// Broiler-Human:        PENDING
internal sealed class JsX64Walk
{
    /// <summary>The program the unit belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=ECB652
    // Broiler-Human:        PENDING
    private readonly JsNativeProgramImage image;

    /// <summary>The unit's own row.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=726530
    // Broiler-Human:        PENDING
    private readonly JsFunctionRow row;

    /// <summary>Where the unit's code starts in the code section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=73C789
    // Broiler-Human:        PENDING
    private readonly int start;

    /// <summary>The first offset after the unit's code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=24D103
    // Broiler-Human:        PENDING
    private readonly int end;

    /// <summary>The operand-stack contents at each offset, or null where nothing reaches it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E51BC9
    // Broiler-Human:        PENDING
    private readonly JsX64Value[]?[] stacks;

    /// <summary>The scope-chain sizes at each offset, innermost last.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5059D5
    // Broiler-Human:        PENDING
    private readonly int[]?[] scopes;

    /// <summary>Traces one unit of <paramref name="program"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=45AC38
    // Broiler-Human:        PENDING
    internal JsX64Walk(JsNativeProgramImage program, JsFunctionRow unit)
    {
        image = program;
        row = unit;
        start = (int)unit.CodeOffset;
        end = start + (int)unit.CodeLength;
        stacks = new JsX64Value[]?[unit.CodeLength];
        scopes = new int[]?[unit.CodeLength];
    }

    /// <summary>How many slots the flattened scope slab needs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B49CFD
    // Broiler-Human:        PENDING
    internal int ScopeSlots { get; private set; }

    /// <summary>Whether anything reaches <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=72805C
    // Broiler-Human:        PENDING
    internal bool IsReachable(int at) => stacks[at - start] is not null;

    /// <summary>The operand stack at <paramref name="at"/>, deepest slot first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=92B6C1
    // Broiler-Human:        PENDING
    internal JsX64Value[] StackAt(int at) => stacks[at - start]!;

    /// <summary>The operand-stack height at <paramref name="at"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E9F5D0
    // Broiler-Human:        PENDING
    internal int HeightAt(int at) => stacks[at - start]!.Length;

    /// <summary>
    /// The flattened slab slot a <c>depth</c> and <c>index</c> pair names at
    /// <paramref name="at"/>, or a refusal.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EDDD0C
    // Broiler-Falsified-If: an index outside the named scope's declared size resolves to a slot rather than a refusal
    // Broiler-Human:        PENDING
    internal bool TryResolveSlot(int at, int depth, int index, out int slot, out string refusal)
    {
        slot = 0;
        var sizes = scopes[at - start]!;

        if (depth >= sizes.Length)
        {
            refusal = "a scope reference reaches past the scopes this unit has pushed";
            return false;
        }

        var which = sizes.Length - 1 - depth;

        if (index >= sizes[which])
        {
            refusal = "a scope reference names a slot the scope it reaches does not declare";
            return false;
        }

        var offset = 0;

        for (var below = 0; below < which; below++)
        {
            offset += sizes[below];
        }

        slot = offset + index;
        refusal = string.Empty;
        return true;
    }

    /// <summary>Traces the unit, or refuses it and says why.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=63023E
    // Broiler-Falsified-If: an offset is marked reachable with a state some path to it does not produce
    // Broiler-Human:        PENDING
    internal bool TryTrace(out string refusal)
    {
        var work = new System.Collections.Generic.Stack<(int At, JsX64Value[] Stack, int[] Scopes)>();
        work.Push((start, [], [(int)row.ScopeSlots]));

        while (work.Count != 0)
        {
            var (at, stack, chain) = work.Pop();

            if (at < start || at >= end)
            {
                refusal = "control reaches an offset outside the unit's own code";
                return false;
            }

            var seen = stacks[at - start];

            if (seen is not null)
            {
                if (!Same(seen, stack) || !Same(scopes[at - start]!, chain))
                {
                    refusal =
                        "two paths reach bytecode offset " + at + " with different operand or " +
                        "scope shapes, which this emitter refuses rather than approximates";

                    return false;
                }

                continue;
            }

            stacks[at - start] = stack;
            scopes[at - start] = chain;
            ScopeSlots = System.Math.Max(ScopeSlots, Total(chain));

            if (!Step(at, stack, chain, work, out refusal))
            {
                return false;
            }
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>Applies one instruction's effect and queues its successors.</summary>
    /// <remarks>
    /// <b>THE EFFECT ON THE HEIGHT IS TAKEN FROM THE FORMAT'S OWN TABLE AND THE EFFECT ON THE KINDS
    /// IS NOT</b>, because the format has a table for one and no opinion about the other. Reading
    /// the heights from <see cref="JsOpcodes.TryDescribe"/> is what keeps this walk from being a
    /// second, disagreeing statement of what the bytecode does; the kinds are this emitter's own
    /// model and are stated here because nothing else in the tree has a use for them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=98D4B8
    // Broiler-Falsified-If: this walk's height for an instruction differs from the format's own stack-effect table
    // Broiler-Human:        PENDING
    private bool Step(
        int at,
        JsX64Value[] stack,
        int[] chain,
        System.Collections.Generic.Stack<(int, JsX64Value[], int[])> work,
        out string refusal)
    {
        var opcode = (JsOpcode)image.Code[at];
        var width = JsOpcodes.InstructionWidth(opcode);
        var operand = Operand(at, opcode);

        if (!JsOpcodes.TryDescribe(opcode, operand, out var pops, out var pushes))
        {
            refusal = "the unit carries an instruction this format version does not define";
            return false;
        }

        if (pops > stack.Length)
        {
            refusal = "an instruction at bytecode offset " + at + " pops past the stack's bottom";
            return false;
        }

        var next = new System.Collections.Generic.List<JsX64Value>(
            stack[..(stack.Length - pops)]);

        switch (opcode)
        {
            case JsOpcode.LoadUndefined:
                next.Add(new JsX64Value(JsX64ValueKind.Undefined, 0));
                break;

            case JsOpcode.Closure:
                next.Add(new JsX64Value(JsX64ValueKind.Function, (int)operand));
                break;

            case JsOpcode.Duplicate:
                next.Add(stack[^1]);
                break;

            case JsOpcode.Not:
            case JsOpcode.LessThan:
            case JsOpcode.LessThanOrEqual:
            case JsOpcode.GreaterThan:
            case JsOpcode.GreaterThanOrEqual:
            case JsOpcode.StrictEquals:
            case JsOpcode.StrictNotEquals:
            case JsOpcode.LooseEquals:
            case JsOpcode.LooseNotEquals:
                next.Add(new JsX64Value(JsX64ValueKind.Boolean, 0));
                break;

            case JsOpcode.LoadGlobal:
                next.Add(GlobalKind((int)operand));
                break;

            case JsOpcode.PushScope:
                chain = [.. chain, (int)operand];
                break;

            case JsOpcode.CopyScope:
                if (chain.Length == 0 || chain[^1] != (int)operand)
                {
                    refusal =
                        "a scope copy at bytecode offset " + at + " names a size the scope it " +
                        "copies does not have";

                    return false;
                }

                break;

            case JsOpcode.PopScope:
                if (chain.Length <= 1)
                {
                    refusal = "a scope pop at bytecode offset " + at + " has no pushed scope to pop";
                    return false;
                }

                chain = chain[..^1];
                break;

            default:
                for (var pushed = 0; pushed < pushes; pushed++)
                {
                    next.Add(new JsX64Value(JsX64ValueKind.Number, 0));
                }

                break;
        }

        // The cases above that push their own answer have already done so; the default arm is the
        // only one that pushes Numbers, so a case that pushed nothing and declared a push is a
        // defect this assertion turns into a refusal rather than a wrong slab layout.
        if (next.Count != stack.Length - pops + pushes)
        {
            refusal =
                "the emitter's model of instruction `" + opcode + "` disagrees with the format's " +
                "own stack-effect table";

            return false;
        }

        var after = next.ToArray();

        if (JsOpcodes.HasCodeTarget(opcode))
        {
            work.Push(((int)operand, after, chain));
        }

        if (opcode is JsOpcode.Return or JsOpcode.ReturnUndefined or JsOpcode.Jump)
        {
            refusal = string.Empty;
            return true;
        }

        if (at + width >= end)
        {
            refusal =
                "the unit's code runs off its end at bytecode offset " + at +
                ", which is a unit no lowering of this manifest produces";

            return false;
        }

        work.Push((at + width, after, chain));
        refusal = string.Empty;
        return true;
    }

    /// <summary>What a global read pushes: a function where one was declared, a Number otherwise.</summary>
    /// <remarks>
    /// <b>A function global is resolved HERE and never becomes a slot</b>, which is what lets a
    /// call be a direct <c>call rel32</c> to a code unit rather than an indirect call through a
    /// value. The whole call graph of a program of this manifest is closed over the program itself,
    /// so every callee is known when the artifact is compiled.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D03370
    // Broiler-Human:        PENDING
    private JsX64Value GlobalKind(int nameConstant) =>
        Functions.TryGetValue(nameConstant, out var unit)
            ? new JsX64Value(JsX64ValueKind.Function, unit)
            : new JsX64Value(JsX64ValueKind.Number, 0);

    /// <summary>Which code unit each function-valued realm binding holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9A890D
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<int, int> Functions { get; init; } = [];

    /// <summary>Reads the instruction's operand as an unsigned value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=701BA9
    // Broiler-Human:        PENDING
    internal uint Operand(int at, JsOpcode opcode)
    {
        var shape = JsOpcodes.Shape(opcode);

        return shape switch
        {
            JsOperandShape.U8 => image.Code[at + 1],
            JsOperandShape.U16 => (uint)(image.Code[at + 1] | (image.Code[at + 2] << 8)),
            JsOperandShape.U8U16 => (uint)(image.Code[at + 2] | (image.Code[at + 3] << 8)),
            JsOperandShape.U32 => (uint)(
                image.Code[at + 1] |
                (image.Code[at + 2] << 8) |
                (image.Code[at + 3] << 16) |
                (image.Code[at + 4] << 24)),
            _ => 0,
        };
    }

    /// <summary>The scope depth a three-byte slot instruction carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C51238
    // Broiler-Human:        PENDING
    internal int Depth(int at) => image.Code[at + 1];

    /// <summary>Whether two operand stacks are the same shape and the same kinds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CD7F4F
    // Broiler-Human:        PENDING
    private static bool Same(JsX64Value[] left, JsX64Value[] right)
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

    /// <summary>Whether two scope chains are the same shape.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9BFF9F
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

    /// <summary>How many slots a whole scope chain occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0E944C
    // Broiler-Human:        PENDING
    private static int Total(int[] sizes)
    {
        var sum = 0;

        foreach (var size in sizes)
        {
            sum += size;
        }

        return sum;
    }
}
