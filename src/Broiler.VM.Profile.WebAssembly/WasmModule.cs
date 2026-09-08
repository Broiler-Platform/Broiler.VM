// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   50
// Annotated:        50/50
// Exempt:           44
// Human-reviewed:   0/50
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  3/10 max
// Unverified:       50
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One constant expression, in the only shape this format version admits: a single instruction and
/// the byte that closes it.
/// </summary>
/// <remarks>
/// It is held decoded rather than as bytes because it has exactly two fields and re-reading it at
/// instantiation would be a second decoder for one instruction. <see cref="Bits"/> carries whatever
/// the opcode's immediate was: a sign-extended integer, a raw float bit pattern, or a global index.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=55DC20
// Broiler-Human:        PENDING
internal readonly struct WasmConstantExpression
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1D8D04
    // Broiler-Human:        PENDING
    internal WasmConstantExpression(byte opcode, ulong bits)
    {
        Opcode = opcode;
        Bits = bits;
    }

    /// <summary>The single instruction's opcode byte.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=71A8B4
    // Broiler-Human:        PENDING
    internal byte Opcode { get; }

    /// <summary>The instruction's immediate, as raw bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3E1229
    // Broiler-Human:        PENDING
    internal ulong Bits { get; }
}

/// <summary>The four kinds of entity an export can name, as the format encodes them.</summary>
/// <remarks>
/// It is public because a composition root reporting what a module declares has to name a kind, and
/// an integer with a comment beside it would be the same fact told worse.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=3014B3
// Broiler-Human:        PENDING
public enum WasmExportKind
{
    /// <summary>A function.</summary>
    Function = 0,

    /// <summary>A table.</summary>
    Table = 1,

    /// <summary>A linear memory.</summary>
    Memory = 2,

    /// <summary>A global.</summary>
    Global = 3,
}

/// <summary>One export: the name the module publishes, and what it names.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=BB97B7
// Broiler-Human:        PENDING
internal sealed class WasmExport
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E138DF
    // Broiler-Human:        PENDING
    private readonly byte[] name;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=764D77
    // Broiler-Human:        PENDING
    internal WasmExport(byte[] utf8Name, WasmExportKind kind, uint entityIndex)
    {
        name = utf8Name;
        Kind = kind;
        EntityIndex = entityIndex;
    }

    /// <summary>What kind of entity the export names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C77277
    // Broiler-Human:        PENDING
    internal WasmExportKind Kind { get; }

    /// <summary>The index of the entity inside its own index space.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0E063A
    // Broiler-Human:        PENDING
    internal uint EntityIndex { get; }

    /// <summary>The name, as the bytes the artifact carried.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FF1FD6
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<byte> Name => System.MemoryExtensions.AsSpan(name);
}

/// <summary>One global: its declared shape and the expression that initialises it.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=CB20CB
// Broiler-Human:        PENDING
internal sealed class WasmGlobal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=07AA15
    // Broiler-Human:        PENDING
    internal WasmGlobal(WasmGlobalType type, WasmConstantExpression initializer)
    {
        Type = type;
        Initializer = initializer;
    }

    /// <summary>The declared value type and mutability.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B498A1
    // Broiler-Human:        PENDING
    internal WasmGlobalType Type { get; }

    /// <summary>The initializing expression.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=705532
    // Broiler-Human:        PENDING
    internal WasmConstantExpression Initializer { get; }
}

/// <summary>One element segment: where it writes, and the function indices it writes.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=A1513E
// Broiler-Human:        PENDING
internal sealed class WasmElementSegment
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D0032F
    // Broiler-Human:        PENDING
    private readonly uint[] functionIndices;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1CB0B3
    // Broiler-Human:        PENDING
    internal WasmElementSegment(uint tableIndex, WasmConstantExpression offset, uint[] indices)
    {
        TableIndex = tableIndex;
        Offset = offset;
        functionIndices = indices;
    }

    /// <summary>The table the segment writes into.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=999B8F
    // Broiler-Human:        PENDING
    internal uint TableIndex { get; }

    /// <summary>The expression giving the offset it writes at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=491714
    // Broiler-Human:        PENDING
    internal WasmConstantExpression Offset { get; }

    /// <summary>How many entries the segment writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=38578F
    // Broiler-Human:        PENDING
    internal int EntryCount => functionIndices.Length;

    /// <summary>The function indices, as a read-only window over the array this segment keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C95098
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<uint> Entries => System.MemoryExtensions.AsSpan(functionIndices);
}

/// <summary>One data segment: where it writes, and the bytes it writes.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=45AF41
// Broiler-Human:        PENDING
internal sealed class WasmDataSegment
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A28EA9
    // Broiler-Human:        PENDING
    private readonly byte[] contents;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F26144
    // Broiler-Human:        PENDING
    internal WasmDataSegment(uint memoryIndex, WasmConstantExpression offset, byte[] bytes)
    {
        MemoryIndex = memoryIndex;
        Offset = offset;
        contents = bytes;
    }

    /// <summary>The memory the segment writes into.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C88001
    // Broiler-Human:        PENDING
    internal uint MemoryIndex { get; }

    /// <summary>The expression giving the offset it writes at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=491714
    // Broiler-Human:        PENDING
    internal WasmConstantExpression Offset { get; }

    /// <summary>How many bytes the segment writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=327536
    // Broiler-Human:        PENDING
    internal int ByteCount => contents.Length;

    /// <summary>The bytes, as a read-only window over the array this segment keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F8499F
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<byte> Contents => System.MemoryExtensions.AsSpan(contents);
}

/// <summary>
/// One structured instruction's resolved targets: where its <c>else</c> arm begins, where the block
/// it opens ends, and what a branch to its label costs.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT EXISTS SO THAT NOTHING SCANS FOR A MATCHING <c>end</c> AT RUN TIME.</b> The bytes of a
/// function body carry no back pointer: a <c>block</c> says where it starts and nothing says where
/// it stops, so an interpreter that did not have this table would walk forward over the body's
/// remaining bytes - skipping immediates correctly, which means decoding them - every time a branch
/// was taken. That turns a loop with a conditional exit into a quadratic walk over the body and
/// hands a guest a cost it chooses. Validation already walks every instruction once and already
/// knows, at each <c>end</c>, which opening instruction it closes, so the pairing is recorded there
/// and read here.
/// </para>
/// <para>
/// <b>The two arities are recorded for the same reason as the two offsets.</b> A branch truncates
/// the operand stack to the label's height and carries the label's values across, so an interpreter
/// needs to know how many values that is - and the only other way to learn it is to re-read the
/// block type immediate the validator already read.
/// </para>
/// <para>
/// Every offset is relative to the start of the body's own instruction bytes, and every one of them
/// is a position this validation computed rather than a number the payload carried.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4D9BAD
// Broiler-Falsified-If: an offset here is assigned from a value read out of the payload rather than from a position validation reached
// Broiler-Human:        PENDING
internal struct WasmJumpTarget
{
    /// <summary>The offset of the structured instruction's own opcode byte.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B9A1BC
    // Broiler-Human:        PENDING
    public int Offset;

    /// <summary>
    /// Where the <c>else</c> arm begins, or minus one when the instruction has none.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=15C05B
    // Broiler-Human:        PENDING
    public int ElseOffset;

    /// <summary>Just past the <c>end</c> that closes the block this instruction opened.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C25439
    // Broiler-Human:        PENDING
    public int EndOffset;

    /// <summary>
    /// Where execution continues when this block's label is branched to: the first instruction of
    /// the body for a <c>loop</c>, and <see cref="EndOffset"/> for everything else.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8EE365
    // Broiler-Human:        PENDING
    public int LabelOffset;

    /// <summary>How many values a branch to this block's label carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=681D87
    // Broiler-Human:        PENDING
    public int LabelArity;

    /// <summary>How many values the block leaves behind when it ends normally.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7FF1A0
    // Broiler-Human:        PENDING
    public int EndArity;
}

/// <summary>
/// One function body: its expanded local types, its instruction bytes, the two execution bounds
/// that are computed rather than read, and the jump targets validation resolved.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE TWO BOUNDS ARE NEVER READ FROM THE PAYLOAD, AND THE SENTINEL IS HOW THAT IS KEPT TRUE.</b>
/// A body the decoder has produced and validation has not yet reached carries
/// <see cref="BoundNotComputed"/> in both, because the pass that computes them is validation.
/// Nothing may size a stack, a label array or any other buffer from either field while it reads the
/// sentinel. The alternative - a declared maximum read out of the module - is the shape that lets a
/// payload choose how much memory a host commits, and the format does not even offer it here.
/// </para>
/// <para>
/// <b>The three computed fields are assigned exactly once, by validation, before verification
/// returns.</b> <see cref="TrySeal"/> refuses a second call, so a body cannot be resealed and the
/// immutability a shareable verified handle depends on is a property of this type rather than a
/// convention its callers keep. There is deliberately no setter and no other way in.
/// </para>
/// <para>
/// The local types are expanded: the format encodes them as runs of a count and a type, and the
/// count of each run is charged against the declared-count ceiling before the run is expanded.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=7D49AB
// Broiler-Falsified-If: either bound is assigned from a number the payload carried, or a body is sealed twice, or a buffer is sized from a bound while it reads the sentinel
// Broiler-Human:        PENDING
internal sealed class WasmFunctionBody
{
    /// <summary>What both bounds read until validation computes them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E5CA0F
    // Broiler-Human:        PENDING
    internal const int BoundNotComputed = -1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7F7168
    // Broiler-Human:        PENDING
    private readonly WasmValueType[] locals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B51671
    // Broiler-Human:        PENDING
    private readonly byte[] code;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4424EF
    // Broiler-Human:        PENDING
    private WasmJumpTarget[] jumps;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=459560
    // Broiler-Human:        PENDING
    private int maxOperandStack;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=27AEBA
    // Broiler-Human:        PENDING
    private int maxLabelDepth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A39CB1
    // Broiler-Human:        PENDING
    internal WasmFunctionBody(WasmValueType[] localTypes, byte[] instructions)
    {
        locals = localTypes;
        code = instructions;
        jumps = [];
        maxOperandStack = BoundNotComputed;
        maxLabelDepth = BoundNotComputed;
    }

    /// <summary>
    /// Records what validation computed about this body, once.
    /// </summary>
    /// <remarks>
    /// A second call answers <see langword="false"/> and changes nothing, which is the whole reason
    /// this is a method rather than three setters: the window in which this state may move is the
    /// dynamic extent of one verification, and a type that cannot say no would leave that window
    /// open for the life of a shared handle.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=0C343E
    // Broiler-Falsified-If: a second call changes any field, or a caller reaches these fields by any other route
    // Broiler-Human:        PENDING
    internal bool TrySeal(int operandStackBound, int labelDepthBound, WasmJumpTarget[] jumpTargets)
    {
        if (maxOperandStack != BoundNotComputed || maxLabelDepth != BoundNotComputed)
        {
            return false;
        }

        maxOperandStack = operandStackBound;
        maxLabelDepth = labelDepthBound;
        jumps = jumpTargets;
        return true;
    }

    /// <summary>
    /// The deepest the operand stack goes in this body, computed at validation and stored here.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6AF324
    // Broiler-Human:        PENDING
    internal int MaxOperandStack => maxOperandStack;

    /// <summary>
    /// The deepest the control-frame stack goes in this body, computed at validation and stored
    /// here.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=32822B
    // Broiler-Human:        PENDING
    internal int MaxLabelDepth => maxLabelDepth;

    /// <summary>How many structured instructions this body opens.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AB79D0
    // Broiler-Human:        PENDING
    internal int JumpTargetCount => jumps.Length;

    /// <summary>The resolved jump targets, ascending by the offset each one is keyed on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C76463
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmJumpTarget> JumpTargets =>
        System.MemoryExtensions.AsSpan(jumps);

    /// <summary>
    /// Finds the targets recorded for the structured instruction at one offset.
    /// </summary>
    /// <remarks>
    /// The search is a binary one because validation appends entries in the order it meets the
    /// instructions, which is ascending offset order, so the table is sorted by construction rather
    /// than by a sort nobody would see run.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3EE527
    // Broiler-Falsified-If: the table is searched as though sorted while validation appends out of offset order
    // Broiler-Human:        PENDING
    internal bool TryFindJumpTarget(int offset, out WasmJumpTarget target)
    {
        target = default;

        var low = 0;
        var high = jumps.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);

            if (jumps[middle].Offset == offset)
            {
                target = jumps[middle];
                return true;
            }

            if (jumps[middle].Offset < offset)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return false;
    }

    /// <summary>How many locals the body declares, beyond its parameters.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C69714
    // Broiler-Human:        PENDING
    internal int LocalCount => locals.Length;

    /// <summary>How many instruction bytes the body holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AB4619
    // Broiler-Human:        PENDING
    internal int CodeLength => code.Length;

    /// <summary>The expanded local types, as a read-only window over the array this body keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=337B45
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmValueType> Locals => System.MemoryExtensions.AsSpan(locals);

    /// <summary>The instruction bytes, as a read-only window over the array this body keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FD7652
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<byte> Code => System.MemoryExtensions.AsSpan(code);
}

/// <summary>
/// The decoded and validated module verification produces, and the state a verified artifact
/// carries.
/// </summary>
/// <remarks>
/// <para>
/// <b>A MODULE REACHING A CALLER HAS BEEN THROUGH BOTH PASSES, AND THE TWO ASSERT DIFFERENT
/// THINGS.</b> Decoding asserts that the bytes are a well-formed encoding: the magic and version
/// are right, the sections are in the canonical order with no non-custom section twice, every
/// section body consumed exactly its declared length, every variable-length integer was inside its
/// byte budget, every name was well-formed UTF-8, and the function and code counts agree.
/// Validation asserts the rest: that every index addresses something in the space it names, that
/// every instruction byte names an instruction this surface admits, that every operand stack
/// balances with the types the instructions declare, that every block nests and closes, and that
/// every load and store aligns within its access width. The class carries no flag for the first
/// pass because a module that failed it was never constructed, and it carries
/// <see cref="ExecutionBoundsComputed"/> for the second because a body's computed bounds are what
/// prove validation reached it.
/// </para>
/// <para>
/// <b>What neither pass asserts is anything about the guest's behaviour</b> - only that the body is
/// walkable and its bounds are computed. Running it is the executor's job, and the executor beside
/// this class runs it: it allocates a store, evaluates the start function if the module declares
/// one, and answers a completed step carrying results on invocation.
/// <i>(Corrected 2026-09-08. This paragraph read "<b>What it still does not assert is that the
/// module can be run</b>, because running it needs an interpreter and there is none in this
/// assembly. The executor beside this class refuses every step and says so in its own words." Both
/// halves were true when written and stopped being true when <see cref="WasmInterpreter"/> landed
/// in this assembly and <see cref="WebAssemblyExecutor"/> began running it. The superseded reading
/// is quoted rather than deleted, because a record that revises itself silently is a record a
/// reader cannot audit.)</i>
/// </para>
/// <para>
/// <b>Everything reachable from it is immutable once verification returns.</b> That is what makes a
/// shareable handle safe for two runtimes reading it at once with no synchronisation between them.
/// No array is handed out: the arrays are private, the internal accessors return read-only windows,
/// and the public surface is counts and copies. There is no cache slot, no interned identity and no
/// process-local object anywhere behind it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=25C6A2
// Broiler-Falsified-If: anything reachable from this state can be mutated after verification returns, or a caller reads one of these fields as though a validation pass had run
// Broiler-Human:        PENDING
public sealed class WasmModule : IVmVerifiedState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E975C8
    // Broiler-Human:        PENDING
    private readonly WasmFuncType[] types;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DF09A7
    // Broiler-Human:        PENDING
    private readonly uint[] functionTypeIndices;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60A619
    // Broiler-Human:        PENDING
    private readonly WasmTableType[] tables;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=33C0B8
    // Broiler-Human:        PENDING
    private readonly WasmMemoryType[] memories;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4AFC3A
    // Broiler-Human:        PENDING
    private readonly WasmGlobal[] globals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=484BE3
    // Broiler-Human:        PENDING
    private readonly WasmExport[] exports;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A0AA2F
    // Broiler-Human:        PENDING
    private readonly WasmElementSegment[] elements;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=44DB50
    // Broiler-Human:        PENDING
    private readonly WasmDataSegment[] data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=746CE6
    // Broiler-Human:        PENDING
    private readonly WasmFunctionBody[] bodies;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4753FD
    // Broiler-Human:        PENDING
    internal WasmModule(
        WasmFuncType[] declaredTypes,
        uint[] declaredFunctionTypeIndices,
        WasmTableType[] declaredTables,
        WasmMemoryType[] declaredMemories,
        WasmGlobal[] declaredGlobals,
        WasmExport[] declaredExports,
        WasmElementSegment[] declaredElements,
        WasmDataSegment[] declaredData,
        WasmFunctionBody[] declaredBodies,
        long startFunctionIndex,
        bool declaresDataCount,
        int customSectionCount)
    {
        types = declaredTypes;
        functionTypeIndices = declaredFunctionTypeIndices;
        tables = declaredTables;
        memories = declaredMemories;
        globals = declaredGlobals;
        exports = declaredExports;
        elements = declaredElements;
        data = declaredData;
        bodies = declaredBodies;
        StartFunctionIndex = startFunctionIndex;
        DeclaresDataCount = declaresDataCount;
        CustomSectionCount = customSectionCount;
    }

    /// <summary>How many function types the module declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=60F48A
    // Broiler-Human:        PENDING
    public int TypeCount => types.Length;

    /// <summary>How many functions the module defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D5FAED
    // Broiler-Human:        PENDING
    public int FunctionCount => functionTypeIndices.Length;

    /// <summary>How many tables the module defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5D7C83
    // Broiler-Human:        PENDING
    public int TableCount => tables.Length;

    /// <summary>How many linear memories the module defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A1D353
    // Broiler-Human:        PENDING
    public int MemoryCount => memories.Length;

    /// <summary>How many globals the module defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=710892
    // Broiler-Human:        PENDING
    public int GlobalCount => globals.Length;

    /// <summary>How many names the module exports.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0C14DE
    // Broiler-Human:        PENDING
    public int ExportCount => exports.Length;

    /// <summary>How many element segments the module declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=473313
    // Broiler-Human:        PENDING
    public int ElementSegmentCount => elements.Length;

    /// <summary>How many data segments the module declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C72AE4
    // Broiler-Human:        PENDING
    public int DataSegmentCount => data.Length;

    /// <summary>
    /// The index of the start function, or minus one when the module declares none.
    /// </summary>
    /// <remarks>
    /// It is wider than the index space it names on purpose. The payload encodes it as an unsigned
    /// 32-bit number and the decoder that reads it has not yet checked that it addresses a function,
    /// so narrowing it there would let the largest legal encoding collide with the value that means
    /// "no start function". Validation performs the range check, which is why a module a caller
    /// holds carries an index that addresses a function or carries minus one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C005DB
    // Broiler-Human:        PENDING
    public long StartFunctionIndex { get; }

    /// <summary>Whether the module carries a data count section.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5A8406
    // Broiler-Human:        PENDING
    public bool DeclaresDataCount { get; }

    /// <summary>How many custom sections were read past and ignored.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=456367
    // Broiler-Human:        PENDING
    public int CustomSectionCount { get; }

    /// <summary>How many instruction bytes all the function bodies hold between them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1A613E
    // Broiler-Human:        PENDING
    public int TotalCodeBytes
    {
        get
        {
            var total = 0;

            for (var index = 0; index < bodies.Length; index++)
            {
                total += bodies[index].CodeLength;
            }

            return total;
        }
    }

    /// <summary>
    /// Whether the per-body execution bounds have been computed, which they have on every module
    /// verification hands back.
    /// </summary>
    /// <remarks>
    /// A caller reads this rather than inferring it from a count, because a zero bound would be
    /// indistinguishable from a function that happens to need no stack. It is false only on a module
    /// that decoding produced and validation has not yet sealed, which is a state that exists inside
    /// one verification and never outside it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A79497
    // Broiler-Falsified-If: it reports true while any body still carries the not-computed sentinel
    // Broiler-Human:        PENDING
    public bool ExecutionBoundsComputed
    {
        get
        {
            for (var index = 0; index < bodies.Length; index++)
            {
                if (bodies[index].MaxOperandStack == WasmFunctionBody.BoundNotComputed ||
                    bodies[index].MaxLabelDepth == WasmFunctionBody.BoundNotComputed)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>Describes one export without handing out anything the module holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B03C5
    // Broiler-Human:        PENDING
    public bool TryDescribeExport(int index, out WasmExportKind kind, out uint entityIndex, out int nameLength)
    {
        kind = WasmExportKind.Function;
        entityIndex = 0;
        nameLength = 0;

        if (index < 0 || index >= exports.Length)
        {
            return false;
        }

        kind = exports[index].Kind;
        entityIndex = exports[index].EntityIndex;
        nameLength = exports[index].Name.Length;
        return true;
    }

    /// <summary>Copies one export's name bytes into a caller-supplied buffer.</summary>
    /// <remarks>
    /// A copy rather than a window, because a window into a private array is a handle onto state a
    /// shared verified module owns and this member is on the public surface.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=82B42B
    // Broiler-Falsified-If: it writes past the destination, or hands back a view onto the array the module keeps
    // Broiler-Human:        PENDING
    public bool TryCopyExportName(int index, System.Span<byte> destination, out int written)
    {
        written = 0;

        if (index < 0 || index >= exports.Length)
        {
            return false;
        }

        var name = exports[index].Name;

        if (destination.Length < name.Length)
        {
            return false;
        }

        name.CopyTo(destination);
        written = name.Length;
        return true;
    }

    /// <summary>Describes one function body's shape.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFDD63
    // Broiler-Human:        PENDING
    public bool TryDescribeFunctionBody(int index, out int localCount, out int codeLength)
    {
        localCount = 0;
        codeLength = 0;

        if (index < 0 || index >= bodies.Length)
        {
            return false;
        }

        localCount = bodies[index].LocalCount;
        codeLength = bodies[index].CodeLength;
        return true;
    }

    /// <summary>The declared function types, for the pass that will validate against them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1F3B2D
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmFuncType> Types => System.MemoryExtensions.AsSpan(types);

    /// <summary>Each defined function's type index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=279C6C
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<uint> FunctionTypeIndices =>
        System.MemoryExtensions.AsSpan(functionTypeIndices);

    /// <summary>The declared tables.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EFD7AC
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmTableType> Tables => System.MemoryExtensions.AsSpan(tables);

    /// <summary>The declared memories.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=ABDC7A
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmMemoryType> Memories => System.MemoryExtensions.AsSpan(memories);

    /// <summary>The declared globals.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AE8FE0
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmGlobal> Globals => System.MemoryExtensions.AsSpan(globals);

    /// <summary>The declared exports.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=743649
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmExport> Exports => System.MemoryExtensions.AsSpan(exports);

    /// <summary>The declared element segments.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=64A95E
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmElementSegment> Elements =>
        System.MemoryExtensions.AsSpan(elements);

    /// <summary>The declared data segments.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2A799C
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmDataSegment> Data => System.MemoryExtensions.AsSpan(data);

    /// <summary>The decoded function bodies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=00B3AC
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmFunctionBody> Bodies => System.MemoryExtensions.AsSpan(bodies);
}
