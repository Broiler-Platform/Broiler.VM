// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           44
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         5/3
// Resource impact:  0/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// What a verification of a universal bytecode artifact produces: the decoded artifact, the table its
/// manifest selected, the family hook's state, and every unit's instructions decoded once, with the
/// facts the walk proved about each attached so that an emitter never re-derives them.
/// </summary>
/// <remarks>
/// <para>
/// <b>Everything here is immutable once the walk returns</b>, which is the core's requirement of a
/// verified state: a shareable handle is read by several runtimes at once with no lock. The family
/// hook's state is the family's to keep immutable.
/// </para>
/// <para>
/// <b>What the walk attaches.</b> For every instruction, the operand-stack height of each plane before
/// it, how many slots of each plane it pops and pushes, the instruction index of its fall-through and
/// its code target, and the one resolved operand its row needs - a local's index within its plane, a
/// callee, a jump table, a pick's depth within its plane. An emitter reads these; it does not type the
/// stack again, and it cannot disagree with the walk about where a slot lives.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=B1A0C5
// Broiler-Falsified-If: a member here can be changed after verification returns, or answers a fact the walk did not prove
// Broiler-Human:        PENDING
public sealed class UbcVerifiedProgram : IVmVerifiedState
{
    private readonly ImmutableArray<int> entryOrder;

    internal UbcVerifiedProgram(
        UbcArtifact artifact,
        UbcInstructionTable table,
        byte familySlot,
        object? familyState,
        ImmutableArray<UbcUnitCode> units,
        ImmutableArray<ImmutableArray<int>> jumpTableTargets,
        ImmutableArray<int> entryOrder)
    {
        Artifact = artifact;
        Table = table;
        FamilySlot = familySlot;
        FamilyState = familyState;
        Units = units;
        JumpTableTargets = jumpTableTargets;
        this.entryOrder = entryOrder;
    }

    /// <summary>The decoded artifact.</summary>
    public UbcArtifact Artifact { get; }

    /// <summary>The table the artifact's manifest selected.</summary>
    public UbcInstructionTable Table { get; }

    /// <summary>The slot the family occupies, or zero when the artifact declares no family.</summary>
    public byte FamilySlot { get; }

    /// <summary>What the family hook answered from the FamilyData section, for the family's runtime.</summary>
    public object? FamilyState { get; }

    /// <summary>The form the artifact names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=347DA4
    // Broiler-Human:        PENDING
    public string Form => Artifact.Header.FormIdentity;

    /// <summary>Every unit, decoded, in unit order.</summary>
    public ImmutableArray<UbcUnitCode> Units { get; }

    /// <summary>For every jump table, its targets as instruction indices of its own unit, in row order.</summary>
    public ImmutableArray<ImmutableArray<int>> JumpTableTargets { get; }

    /// <summary>The unit an entry named <paramref name="name"/> starts, or false when no entry has that name.</summary>
    /// <remarks>A binary search over the entries in ascending byte order, which the walk established when it proved the names unique.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=77CD21
    // Broiler-Falsified-If: a name no entry carries answers a unit, or a name an entry carries answers false
    // Broiler-Human:        PENDING
    public bool TryGetEntry(System.ReadOnlySpan<byte> name, out int unit)
    {
        var entries = Artifact.Entries;
        var low = 0;
        var high = entryOrder.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            var entry = entries[entryOrder[middle]];
            var order = System.MemoryExtensions.SequenceCompareTo(entry.Name.AsSpan(), name);

            if (order == 0)
            {
                unit = (int)entry.Unit;
                return true;
            }

            if (order < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        unit = -1;
        return false;
    }
}

/// <summary>One unit, decoded: its instructions, its regions, and where its locals live.</summary>
/// <remarks>
/// A frame of the unit holds its word locals and then its word operand stack in the word plane, and its
/// value locals and then its value operand stack in the value plane. A local's index within its plane is
/// its position among the unit's locals of that plane; the parameters are the first locals, in
/// signature order, so a callee's word parameters are its first word locals and its value parameters its
/// first value locals.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=A3975A
// Broiler-Falsified-If: a local count, parameter count or result count here differs from the unit's signature and local runs
// Broiler-Human:        PENDING
public sealed class UbcUnitCode
{
    internal UbcUnitCode(
        int index,
        UbcUnit unit,
        UbcSignature signature,
        int wordLocals,
        int valueLocals,
        int parameterWords,
        int parameterValues,
        int resultWords,
        int resultValues,
        ImmutableArray<UbcInstruction> instructions,
        ImmutableArray<UbcDecodedRegion> regions)
    {
        Index = index;
        Unit = unit;
        Signature = signature;
        WordLocals = wordLocals;
        ValueLocals = valueLocals;
        ParameterWords = parameterWords;
        ParameterValues = parameterValues;
        ResultWords = resultWords;
        ResultValues = resultValues;
        Instructions = instructions;
        Regions = regions;
    }

    /// <summary>The unit's index.</summary>
    public int Index { get; }

    /// <summary>The unit's row.</summary>
    public UbcUnit Unit { get; }

    /// <summary>The unit's signature.</summary>
    public UbcSignature Signature { get; }

    /// <summary>How many of the unit's locals, parameters included, are words.</summary>
    public int WordLocals { get; }

    /// <summary>How many are values.</summary>
    public int ValueLocals { get; }

    /// <summary>How many parameters are words.</summary>
    public int ParameterWords { get; }

    /// <summary>How many parameters are values.</summary>
    public int ParameterValues { get; }

    /// <summary>How many results are words.</summary>
    public int ResultWords { get; }

    /// <summary>How many results are values.</summary>
    public int ResultValues { get; }

    /// <summary>The instructions, in code order; an instruction index is a position in this array.</summary>
    public ImmutableArray<UbcInstruction> Instructions { get; }

    /// <summary>The unit's regions, in section order, which puts every region before any region enclosing it.</summary>
    public ImmutableArray<UbcDecodedRegion> Regions { get; }

    /// <summary>The instruction index at absolute code offset <paramref name="pc"/>, or minus one when it is not a boundary.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=21B1ED
    // Broiler-Human:        PENDING
    public int IndexOf(uint pc) => IndexOf(Instructions, pc);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5B989C
    // Broiler-Falsified-If: an offset between two boundaries answers an index
    // Broiler-Human:        PENDING
    internal static int IndexOf(ImmutableArray<UbcInstruction> instructions, uint pc)
    {
        var low = 0;
        var high = instructions.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            var at = instructions[middle].Pc;

            if (at == pc)
            {
                return middle;
            }

            if (at < pc)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }
}

/// <summary>
/// One decoded instruction and what the walk proved about it. Which of the resolved members an
/// instruction uses is fixed by its opcode, as the member documentation says.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=411334
// Broiler-Falsified-If: a count here differs from the effect the walk applied to the typed stack at this instruction
// Broiler-Human:        PENDING
public readonly struct UbcInstruction
{
    internal UbcInstruction(
        uint pc,
        byte opcode,
        UbcInstructionRow? row,
        ulong operand,
        int next,
        int target,
        int index,
        UbcPlane plane,
        int wordHeight,
        int valueHeight,
        int wordPops,
        int valuePops,
        int wordPushes,
        int valuePushes,
        int takenWordPushes,
        int takenValuePushes)
    {
        Pc = pc;
        Opcode = opcode;
        Row = row;
        Operand = operand;
        Next = next;
        Target = target;
        Index = index;
        Plane = plane;
        WordHeight = wordHeight;
        ValueHeight = valueHeight;
        WordPops = wordPops;
        ValuePops = valuePops;
        WordPushes = wordPushes;
        ValuePushes = valuePushes;
        TakenWordPushes = takenWordPushes;
        TakenValuePushes = takenValuePushes;
    }

    /// <summary>The absolute code offset of the instruction.</summary>
    public uint Pc { get; }

    /// <summary>The opcode byte: a <see cref="UbcOpcode"/> when <see cref="Row"/> is null, the family opcode otherwise.</summary>
    public byte Opcode { get; }

    /// <summary>The family row, or null for a common instruction.</summary>
    public UbcInstructionRow? Row { get; }

    /// <summary>True when the instruction is a family instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F993C2
    // Broiler-Human:        PENDING
    public bool IsFamily => Row is not null;

    /// <summary>The operand, little-endian and zero-extended; a <c>const</c>'s bits as written.</summary>
    public ulong Operand { get; }

    /// <summary>The instruction index of the fall-through successor, or minus one when there is none.</summary>
    public int Next { get; }

    /// <summary>The instruction index of the code target of a jump or a branch row, or minus one.</summary>
    public int Target { get; }

    /// <summary>
    /// The resolved operand: for <c>call</c>, the callee's unit; for <c>jump_table</c>, the table; for
    /// <c>local.*</c>, the local's index within its plane; for <c>pick</c>, the copied slot's depth
    /// within its plane, zero being the plane's top. Minus one for every other instruction.
    /// </summary>
    public int Index { get; }

    /// <summary>For <c>local.*</c> and <c>pick</c>, the plane the slot lives in.</summary>
    public UbcPlane Plane { get; }

    /// <summary>The word plane's operand height before the instruction.</summary>
    public int WordHeight { get; }

    /// <summary>The value plane's operand height before the instruction.</summary>
    public int ValueHeight { get; }

    /// <summary>
    /// Word slots the instruction pops. For <c>squash</c>, the words it discards and keeps together; for
    /// <c>dup</c> and <c>dup2</c>, the words it copies; for <c>return</c>, the unit's word results.
    /// </summary>
    public int WordPops { get; }

    /// <summary>Value slots the instruction pops, under the same reading.</summary>
    public int ValuePops { get; }

    /// <summary>Word slots it pushes on the fall-through edge. For <c>squash</c>, the words it keeps.</summary>
    public int WordPushes { get; }

    /// <summary>Value slots it pushes on the fall-through edge.</summary>
    public int ValuePushes { get; }

    /// <summary>Word slots a branch row pushes on its taken edge.</summary>
    public int TakenWordPushes { get; }

    /// <summary>Value slots a branch row pushes on its taken edge.</summary>
    public int TakenValuePushes { get; }
}

/// <summary>A region of a unit, with its bounds and handler as instruction indices.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0FF556
// Broiler-Human:        PENDING
public readonly struct UbcDecodedRegion
{
    internal UbcDecodedRegion(int start, int end, int handler, int wordEntryHeight, int valueEntryHeight, byte kind)
    {
        Start = start;
        End = end;
        Handler = handler;
        WordEntryHeight = wordEntryHeight;
        ValueEntryHeight = valueEntryHeight;
        Kind = kind;
    }

    /// <summary>The first covered instruction's index.</summary>
    public int Start { get; }

    /// <summary>The index one past the last covered instruction.</summary>
    public int End { get; }

    /// <summary>The handler's first instruction index.</summary>
    public int Handler { get; }

    /// <summary>The word plane's operand height a landing truncates to.</summary>
    public int WordEntryHeight { get; }

    /// <summary>The value plane's operand height a landing truncates to.</summary>
    public int ValueEntryHeight { get; }

    /// <summary>The family's region kind.</summary>
    public byte Kind { get; }

    /// <summary>True when the instruction at <paramref name="index"/> is covered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D40D24
    // Broiler-Human:        PENDING
    public bool Covers(int index) => index >= Start && index < End;
}
