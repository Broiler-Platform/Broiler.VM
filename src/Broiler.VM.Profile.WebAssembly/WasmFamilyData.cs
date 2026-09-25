// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   47
// Annotated:        47/47
// Exempt:           37
// Human-reviewed:   0/47
// IP risk:          Low
// Security risk:    Critical
// Criteria:         20/17
// Resource impact:  2/10 max
// Unverified:       47
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>The type byte of a global in the module definitions: the four word types, numbered from zero.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7258DC
// Broiler-Human:        PENDING
internal enum WasmGlobalKind : byte
{
    /// <summary>A thirty-two-bit integer global.</summary>
    I32 = 0,

    /// <summary>A sixty-four-bit integer global.</summary>
    I64 = 1,

    /// <summary>A single-precision float global.</summary>
    F32 = 2,

    /// <summary>A double-precision float global.</summary>
    F64 = 3,
}

/// <summary>One global of the module definitions: its type, whether it may be set, and its initial bits.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=54DA6C
// Broiler-Human:        PENDING
internal readonly struct WasmGlobalDefinition
{
    /// <summary>A global of <paramref name="kind"/>, starting at <paramref name="initialBits"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=281DA4
    // Broiler-Human:        PENDING
    internal WasmGlobalDefinition(WasmGlobalKind kind, bool isMutable, ulong initialBits)
    {
        Kind = kind;
        IsMutable = isMutable;
        InitialBits = initialBits;
    }

    /// <summary>The global's type.</summary>
    internal WasmGlobalKind Kind { get; }

    /// <summary>Whether a <c>global.set</c> may name it.</summary>
    internal bool IsMutable { get; }

    /// <summary>The value its constant initialiser gives it, as bits, a thirty-two-bit type's in the low half.</summary>
    internal ulong InitialBits { get; }
}

/// <summary>One active element segment: where in the table it starts, and the functions it writes.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=386089
// Broiler-Human:        PENDING
internal sealed class WasmElementDefinition
{
    /// <summary>A segment writing <paramref name="functions"/> from table entry <paramref name="offset"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EC1506
    // Broiler-Human:        PENDING
    internal WasmElementDefinition(uint offset, ImmutableArray<uint> functions)
    {
        Offset = offset;
        Functions = functions;
    }

    /// <summary>The first table entry written: the offset expression's i32 value, read as unsigned.</summary>
    internal uint Offset { get; }

    /// <summary>The function indices written, which are unit indices.</summary>
    internal ImmutableArray<uint> Functions { get; }
}

/// <summary>One active data segment: where in the memory it starts, and the bytes it writes.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DE0C43
// Broiler-Human:        PENDING
internal readonly struct WasmDataDefinition
{
    /// <summary>A segment writing <paramref name="contents"/> from byte <paramref name="offset"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=49C0CA
    // Broiler-Human:        PENDING
    internal WasmDataDefinition(uint offset, System.ReadOnlyMemory<byte> contents)
    {
        Offset = offset;
        Contents = contents;
    }

    /// <summary>The first byte written: the offset expression's i32 value, read as unsigned.</summary>
    internal uint Offset { get; }

    /// <summary>The bytes written, a window onto the definitions' own bytes rather than a copy.</summary>
    internal System.ReadOnlyMemory<byte> Contents { get; }
}

/// <summary>One export: its name's bytes, what it names, and that thing's index.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2D06B0
// Broiler-Human:        PENDING
internal readonly struct WasmExportDefinition
{
    /// <summary>An export named <paramref name="name"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C6531F
    // Broiler-Human:        PENDING
    internal WasmExportDefinition(System.ReadOnlyMemory<byte> name, WasmExportKind kind, uint index)
    {
        Name = name;
        Kind = kind;
        Index = index;
    }

    /// <summary>The name, as the UTF-8 bytes the module carried; it may be empty.</summary>
    internal System.ReadOnlyMemory<byte> Name { get; }

    /// <summary>What the export names.</summary>
    internal WasmExportKind Kind { get; }

    /// <summary>The index of what it names in its own index space: a function's is its unit's.</summary>
    internal uint Index { get; }
}

/// <summary>
/// A WebAssembly module's definitions as a translation writes them into the family's FamilyData
/// section, and as the family's hook answers them: the family state a verified program carries.
/// </summary>
/// <remarks>
/// <para>
/// Everything a module declares that is not code: its one memory and one table and their limits, its
/// globals, its element and data segments, its exports and its start function. The code is the
/// artifact's units; a function index is a unit index, because no module this profile admits imports
/// a function.
/// </para>
/// <para>
/// <b>Immutable once the hook answers.</b> A verified program is shared by every instance of every
/// runtime that holds it, so nothing here changes after <see cref="WasmFamilyVerifier"/> returns it:
/// the arrays are immutable and the segments' bytes are windows onto the artifact's own immutable
/// section bytes. What an instance changes - the memory, the table, the globals - it copies out at
/// instantiation into its own state.
/// </para>
/// <para>
/// Beside what the section says, the hook's answer carries two indexes it built while checking: the
/// exports in ascending order of their names, which is how an entry point's name is found, and the
/// artifact's Positions rows by unit and offset, which is how a trap finds its position.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=3DE38C
// Broiler-Falsified-If: a member here can change after the hook has answered it, or an export found by name is not the one of exactly that name
// Broiler-Human:        PENDING
internal sealed class WasmDefinitions
{
    /// <summary>The start field's value when the module names no start function.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B99834
    // Broiler-Human:        PENDING
    internal const uint NoStart = 0xFFFF_FFFF;

    /// <summary>Definitions of the given parts, with the two indexes the hook builds given or left empty.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A1B76E
    // Broiler-Human:        PENDING
    internal WasmDefinitions(
        bool hasMemory,
        WasmLimits memory,
        bool hasTable,
        WasmLimits table,
        ImmutableArray<WasmGlobalDefinition> globals,
        ImmutableArray<WasmElementDefinition> elements,
        ImmutableArray<WasmDataDefinition> data,
        ImmutableArray<WasmExportDefinition> exports,
        uint start,
        ImmutableArray<int> exportOrder = default,
        WasmPositionIndex? positions = null)
    {
        HasMemory = hasMemory;
        Memory = memory;
        HasTable = hasTable;
        Table = table;
        Globals = globals;
        Elements = elements;
        Data = data;
        Exports = exports;
        Start = start;
        ExportOrder = exportOrder.IsDefault ? ImmutableArray<int>.Empty : exportOrder;
        Positions = positions ?? WasmPositionIndex.Empty;
    }

    /// <summary>Whether the module declares its one memory.</summary>
    internal bool HasMemory { get; }

    /// <summary>The memory's limits in pages, when it is declared.</summary>
    internal WasmLimits Memory { get; }

    /// <summary>Whether the module declares its one table.</summary>
    internal bool HasTable { get; }

    /// <summary>The table's limits in entries, when it is declared.</summary>
    internal WasmLimits Table { get; }

    /// <summary>The globals, in index order.</summary>
    internal ImmutableArray<WasmGlobalDefinition> Globals { get; }

    /// <summary>The element segments, in the order the module applies them.</summary>
    internal ImmutableArray<WasmElementDefinition> Elements { get; }

    /// <summary>The data segments, in the order the module applies them.</summary>
    internal ImmutableArray<WasmDataDefinition> Data { get; }

    /// <summary>The exports, in the order the module declares them.</summary>
    internal ImmutableArray<WasmExportDefinition> Exports { get; }

    /// <summary>The start function's unit, or <see cref="NoStart"/>.</summary>
    internal uint Start { get; }

    /// <summary>The indices of <see cref="Exports"/> in ascending byte order of their names, which are unique.</summary>
    internal ImmutableArray<int> ExportOrder { get; }

    /// <summary>The artifact's Positions rows, found by unit and offset.</summary>
    internal WasmPositionIndex Positions { get; }

    /// <summary>The start unit, or minus one when the module names none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5DBEA4
    // Broiler-Human:        PENDING
    internal int StartUnit => Start == NoStart ? -1 : (int)Start;

    /// <summary>The same definitions carrying <paramref name="positions"/>: the hook's answer, once it has built the index.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=48A6D7
    // Broiler-Human:        PENDING
    internal WasmDefinitions WithPositions(WasmPositionIndex positions) =>
        new(HasMemory, Memory, HasTable, Table, Globals, Elements, Data, Exports, Start, ExportOrder, positions);

    /// <summary>The export named exactly <paramref name="name"/>, or false when none is.</summary>
    /// <remarks>
    /// A binary search over <see cref="ExportOrder"/>, which the hook sorted and proved unique; the
    /// comparison is byte for byte, so an empty name is found like any other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=8B6C8F
    // Broiler-Falsified-If: a name no export carries answers an export, or a name one carries answers false
    // Broiler-Human:        PENDING
    internal bool TryFindExport(System.ReadOnlySpan<byte> name, out WasmExportDefinition export)
    {
        var low = 0;
        var high = ExportOrder.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            var candidate = Exports[ExportOrder[middle]];
            var order = System.MemoryExtensions.SequenceCompareTo(candidate.Name.Span, name);

            if (order == 0)
            {
                export = candidate;
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

        export = default;
        return false;
    }
}

/// <summary>
/// The artifact's Positions rows, keyed by unit and absolute code offset: what a trap's position is
/// read from.
/// </summary>
/// <remarks>
/// The hook builds it once, charged, after checking the rows ascend strictly by unit and then by
/// offset, so a trap finds its row by one binary search rather than by scanning the section; the walk
/// has already refused a row that names a unit the artifact lacks or an offset that is not one of its
/// instruction boundaries.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=92508E
// Broiler-Falsified-If: a lookup answers the coordinates of a row of another unit or another offset
// Broiler-Human:        PENDING
internal sealed class WasmPositionIndex
{
    private readonly ulong[] keys;
    private readonly ImmutableArray<UbcPosition> rows;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=89C7A5
    // Broiler-Human:        PENDING
    private WasmPositionIndex(ulong[] keys, ImmutableArray<UbcPosition> rows)
    {
        this.keys = keys;
        this.rows = rows;
    }

    /// <summary>An index of no rows.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EF3633
    // Broiler-Human:        PENDING
    internal static WasmPositionIndex Empty { get; } = new([], ImmutableArray<UbcPosition>.Empty);

    /// <summary>The estimated bytes one row of the index keeps, and those of the array holding them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F024B2
    // Broiler-Human:        PENDING
    private const ulong ArrayBytes = 32;

    /// <summary>
    /// Builds the index over <paramref name="positions"/>, charging its work and its bytes before
    /// either is spent, or refuses the rows when they do not strictly ascend.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AF9449
    // Broiler-Falsified-If: rows out of order or stated twice are indexed, or the key array is allocated before its bytes are reserved
    // Broiler-Human:        PENDING
    internal static UbcHookAnswer TryBuild(ImmutableArray<UbcPosition> positions, WasmHookMeter meter, out WasmPositionIndex index)
    {
        index = Empty;

        if (positions.IsDefaultOrEmpty)
        {
            return UbcHookAnswer.Admit;
        }

        if (!meter.TryChargeWork((ulong)positions.Length) || !meter.TryReserve(ArrayBytes + ((ulong)positions.Length * sizeof(ulong))))
        {
            return meter.Exhausted();
        }

        var keys = new ulong[positions.Length];

        for (var at = 0; at < keys.Length; at++)
        {
            keys[at] = Key(positions[at].Unit, positions[at].Offset);

            if (at > 0 && keys[at] <= keys[at - 1])
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.PositionsNotOrdered);
            }
        }

        index = new WasmPositionIndex(keys, positions);
        return UbcHookAnswer.Admit;
    }

    /// <summary>The coordinates of the row of <paramref name="unit"/> at <paramref name="offset"/>, or false when there is none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=792161
    // Broiler-Falsified-If: a unit and offset no row states answers coordinates
    // Broiler-Human:        PENDING
    internal bool TryFind(int unit, uint offset, out int coordinate0, out int coordinate1)
    {
        var at = unit < 0 ? -1 : System.Array.BinarySearch(keys, Key((uint)unit, offset));

        if (at < 0)
        {
            coordinate0 = 0;
            coordinate1 = 0;
            return false;
        }

        coordinate0 = rows[at].Coordinate0;
        coordinate1 = rows[at].Coordinate1;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B62D74
    // Broiler-Human:        PENDING
    private static ulong Key(uint unit, uint offset) => ((ulong)unit << 32) | offset;
}

/// <summary>
/// The projection between the meter the family's hook is handed and the three things reading the
/// module definitions charges: verifier work, declared counts and the bytes the definitions keep.
/// </summary>
/// <remarks>
/// Shaped like <see cref="WasmReadAdapter"/>, whose remark says why a declared count is charged by
/// this profile rather than by the core's reader: the hook reads the section itself, so the count
/// comparison is its own. It remembers which dimension refused, so the hook answers the exhaustion
/// the meter latched rather than guessing one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=FA12FE
// Broiler-Falsified-If: a charge made through this adapter reaches a dimension other than the one named, or a refusal is answered as an exhaustion of another dimension
// Broiler-Human:        PENDING
internal sealed class WasmHookMeter
{
    private readonly IVmMeter meter;

    /// <summary>Wraps the meter the walk handed the hook.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C84B96
    // Broiler-Human:        PENDING
    internal WasmHookMeter(IVmMeter meter) => this.meter = meter;

    /// <summary>The dimension the last refused charge named, or null.</summary>
    internal VmBudgetDimension? Refused { get; private set; }

    /// <summary>Charges verifier work before it is done.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4FD91C
    // Broiler-Human:        PENDING
    internal bool TryChargeWork(ulong units) => units == 0 || Charge(VmBudgetDimension.VerifierWork, units);

    /// <summary>Reserves the bytes an array the definitions keep will hold, before it exists.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8E22CC
    // Broiler-Human:        PENDING
    internal bool TryReserve(ulong bytes) => bytes == 0 || Charge(VmBudgetDimension.AllocatedBytes, bytes);

    /// <summary>Charges one count the definitions declare against the declared-count ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E205E9
    // Broiler-Human:        PENDING
    internal bool TryChargeDeclaredCount(ulong count) => count == 0 || Charge(VmBudgetDimension.DeclaredCount, count);

    /// <summary>The answer after a refused charge: an exhaustion of the dimension that refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=95D896
    // Broiler-Human:        PENDING
    internal UbcHookAnswer Exhausted() => UbcHookAnswer.Exhaust(Refused ?? VmBudgetDimension.VerifierWork);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AAAB41
    // Broiler-Human:        PENDING
    private bool Charge(VmBudgetDimension dimension, ulong amount)
    {
        if (meter.TryCharge(dimension, amount))
        {
            return true;
        }

        Refused = dimension;
        return false;
    }
}

/// <summary>
/// The FamilyData section of the WebAssembly family, kind nine: the one codec for the module
/// definitions, written by a translation and read by the family's hook, so the two cannot disagree
/// about a byte.
/// </summary>
/// <remarks>
/// <para>
/// <b>The layout</b>, every integer fixed-width and little-endian:
/// </para>
/// <code>
/// u32 version = 1
/// u8  memoryPresent; if 1: u32 minPages, u8 hasMax, [u32 maxPages]
/// u8  tablePresent;  if 1: u32 minEntries, u8 hasMax, [u32 maxEntries]
/// u32 globalCount;   each: u8 type (0 i32, 1 i64, 2 f32, 3 f64), u8 mutable (0/1), u64 initBits
/// u32 elementCount;  each: u32 offset, u32 n, n x u32 function (unit) index
/// u32 dataCount;     each: u32 offset, u32 length, length bytes
/// u32 exportCount;   each: u32 nameLength, name bytes (UTF-8, may be empty), u8 kind, u32 index
/// u32 start          (unit index, or 0xFFFFFFFF for none)
/// </code>
/// <para>
/// <b>Reading is bounded like every read of an untrusted artifact.</b> The section's bytes are charged
/// as verifier work before the first is read; every count is compared with what the remaining bytes
/// could hold at the smallest size one item takes, refused as truncated when they cannot, and charged
/// to the declared-count ceiling; every array the definitions keep is reserved before it exists; and
/// the section must end exactly where its start field does. What the section says is checked here as
/// far as the section alone can say it; what it says about the artifact's units and types is the
/// hook's to check against them.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=4EE470
// Broiler-Falsified-If: a section the layout does not describe is read as definitions, a count is trusted past the bytes that remain, an array is allocated before its bytes are reserved, or a written section reads back as other definitions
// Broiler-Human:        PENDING
internal static class WasmFamilyData
{
    /// <summary>The one layout version this build writes and reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=34728B
    // Broiler-Human:        PENDING
    internal const uint Version = 1;

    // The smallest bytes one item of each counted list takes, which bound a count by what remains.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FC106E
    // Broiler-Human:        PENDING
    private const ulong GlobalItemBytes = 10;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=528AC0
    // Broiler-Human:        PENDING
    private const ulong SegmentItemBytes = 8;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2B5A81
    // Broiler-Human:        PENDING
    private const ulong ExportItemBytes = 9;

    // Estimates of what the definitions keep per item and per array beyond its elements, stated once
    // so every allocation is charged alike, as the walk states its own. They are not measurements.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F024B2
    // Broiler-Human:        PENDING
    private const ulong ArrayBytes = 32;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=734756
    // Broiler-Human:        PENDING
    private const ulong ItemBytes = 32;

    /// <summary>Writes <paramref name="definitions"/> in the layout, exports in their declared order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FEA876
    // Broiler-Falsified-If: a field is written at a width or in an order other than the layout's, or two writes of one definitions differ
    // Broiler-Human:        PENDING
    internal static ImmutableArray<byte> Write(WasmDefinitions definitions)
    {
        var output = new System.Collections.Generic.List<byte>();

        U32(output, Version);
        Limits(output, definitions.HasMemory, definitions.Memory);
        Limits(output, definitions.HasTable, definitions.Table);

        U32(output, (uint)definitions.Globals.Length);

        foreach (var global in definitions.Globals)
        {
            output.Add((byte)global.Kind);
            output.Add(global.IsMutable ? (byte)1 : (byte)0);
            U64(output, global.InitialBits);
        }

        U32(output, (uint)definitions.Elements.Length);

        foreach (var element in definitions.Elements)
        {
            U32(output, element.Offset);
            U32(output, (uint)element.Functions.Length);

            foreach (var function in element.Functions)
            {
                U32(output, function);
            }
        }

        U32(output, (uint)definitions.Data.Length);

        foreach (var segment in definitions.Data)
        {
            U32(output, segment.Offset);
            U32(output, (uint)segment.Contents.Length);
            output.AddRange(segment.Contents.ToArray());
        }

        U32(output, (uint)definitions.Exports.Length);

        foreach (var export in definitions.Exports)
        {
            U32(output, (uint)export.Name.Length);
            output.AddRange(export.Name.ToArray());
            output.Add((byte)export.Kind);
            U32(output, export.Index);
        }

        U32(output, definitions.Start);
        return ImmutableArray.Create(output.ToArray());

        static void Limits(System.Collections.Generic.List<byte> output, bool present, WasmLimits limits)
        {
            output.Add(present ? (byte)1 : (byte)0);

            if (!present)
            {
                return;
            }

            U32(output, limits.Minimum);
            output.Add(limits.HasMaximum ? (byte)1 : (byte)0);

            if (limits.HasMaximum)
            {
                U32(output, limits.Maximum);
            }
        }
    }

    /// <summary>
    /// Reads the definitions in <paramref name="body"/>, charging <paramref name="meter"/>, and answers
    /// admit with them, a refusal with this profile's code, or the exhaustion of a refused charge.
    /// </summary>
    /// <remarks>
    /// It never throws for any body: a truncation, a byte outside a field's range, a count the bytes
    /// cannot hold and trailing bytes are each answered.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=E3BDF6
    // Broiler-Falsified-If: a body the layout does not describe exactly is admitted, a count is used before it is bounded by the remaining bytes and charged, or a read runs past the body
    // Broiler-Human:        PENDING
    internal static UbcHookAnswer TryRead(System.ReadOnlyMemory<byte> body, WasmHookMeter meter, out WasmDefinitions? definitions)
    {
        definitions = null;

        // One unit of work per byte, and one for the section, before any byte is read.
        if (!meter.TryChargeWork((ulong)body.Length + 1))
        {
            return meter.Exhausted();
        }

        var reader = new Cursor(body);

        if (!reader.TryU32(out var version))
        {
            return Truncated();
        }

        if (version != Version)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ModuleDefinitionsVersionUnsupported);
        }

        var answer = ReadLimits(ref reader, memory: true, out var hasMemory, out var memory);

        if (answer.Refused)
        {
            return answer;
        }

        answer = ReadLimits(ref reader, memory: false, out var hasTable, out var table);

        if (answer.Refused)
        {
            return answer;
        }

        answer = ReadGlobals(ref reader, meter, out var globals);

        if (answer.Refused)
        {
            return answer;
        }

        answer = ReadElements(ref reader, meter, hasTable, out var elements);

        if (answer.Refused)
        {
            return answer;
        }

        answer = ReadData(ref reader, meter, hasMemory, out var data);

        if (answer.Refused)
        {
            return answer;
        }

        answer = ReadExports(ref reader, meter, hasMemory, hasTable, globals.Length, out var exports, out var order);

        if (answer.Refused)
        {
            return answer;
        }

        if (!reader.TryU32(out var start))
        {
            return Truncated();
        }

        if (reader.Remaining != 0)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ModuleDefinitionsTrailingBytes);
        }

        definitions = new WasmDefinitions(hasMemory, memory, hasTable, table, globals, elements, data, exports, start, order);
        return UbcHookAnswer.Admit;
    }

    /// <summary>Reads one presence byte and, when present, one set of limits.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=0; Fingerprint=E4BA58
    // Broiler-Falsified-If: a presence or maximum flag other than zero or one is read as a flag, a minimum above its maximum passes, or a memory's limits pass above the format's page maximum
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadLimits(ref Cursor reader, bool memory, out bool present, out WasmLimits limits)
    {
        present = false;
        limits = default;

        if (!reader.TryU8(out var flag))
        {
            return Truncated();
        }

        if (flag > 1)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ModuleDefinitionsMalformedPresence);
        }

        if (flag == 0)
        {
            return UbcHookAnswer.Admit;
        }

        if (!reader.TryU32(out var minimum) || !reader.TryU8(out var hasMaximum))
        {
            return Truncated();
        }

        if (hasMaximum > 1)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.MalformedLimitsFlag);
        }

        var maximum = 0u;

        if (hasMaximum == 1 && !reader.TryU32(out maximum))
        {
            return Truncated();
        }

        if (hasMaximum == 1 && minimum > maximum)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.LimitsMinimumAboveMaximum);
        }

        if (memory && (minimum > WasmTypeGrammar.MaximumMemoryPages || (hasMaximum == 1 && maximum > WasmTypeGrammar.MaximumMemoryPages)))
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.MemoryPagesAboveFormatMaximum);
        }

        present = true;
        limits = new WasmLimits(minimum, maximum, hasMaximum == 1);
        return UbcHookAnswer.Admit;
    }

    /// <summary>Reads the globals: each type, mutability and initial bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=ED6CF0
    // Broiler-Falsified-If: a type byte outside the four, a mutability byte other than zero or one, or a thirty-two-bit global's initial bits above its width is read as a global
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadGlobals(ref Cursor reader, WasmHookMeter meter, out ImmutableArray<WasmGlobalDefinition> globals)
    {
        globals = ImmutableArray<WasmGlobalDefinition>.Empty;
        var answer = ReadCount(ref reader, meter, GlobalItemBytes, ItemBytes, out var count);

        if (answer.Refused)
        {
            return answer;
        }

        var read = new WasmGlobalDefinition[count];

        for (var index = 0; index < read.Length; index++)
        {
            if (!reader.TryU8(out var type) || !reader.TryU8(out var mutable) || !reader.TryU64(out var bits))
            {
                return Truncated();
            }

            if (type > (byte)WasmGlobalKind.F64)
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.UnknownValueType);
            }

            if (mutable > 1)
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.MalformedMutabilityFlag);
            }

            // A thirty-two-bit value lives in the low half of its word, the high half zero, as every
            // producer of one leaves it: a global that started otherwise would put on the word plane a
            // word no instruction could have made.
            if ((WasmGlobalKind)type is WasmGlobalKind.I32 or WasmGlobalKind.F32 && bits > uint.MaxValue)
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.GlobalInitialValueOutOfRange);
            }

            read[index] = new WasmGlobalDefinition((WasmGlobalKind)type, mutable == 1, bits);
        }

        globals = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(read);
        return UbcHookAnswer.Admit;
    }

    /// <summary>Reads the element segments; a segment needs the table it writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1261CA
    // Broiler-Falsified-If: a segment is read when no table is declared, or a function list is read before its count is bounded and charged
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadElements(ref Cursor reader, WasmHookMeter meter, bool hasTable, out ImmutableArray<WasmElementDefinition> elements)
    {
        elements = ImmutableArray<WasmElementDefinition>.Empty;
        var answer = ReadCount(ref reader, meter, SegmentItemBytes, ItemBytes, out var count);

        if (answer.Refused)
        {
            return answer;
        }

        if (count > 0 && !hasTable)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ElementSegmentTableIndexOutOfRange);
        }

        var read = new WasmElementDefinition[count];

        for (var index = 0; index < read.Length; index++)
        {
            if (!reader.TryU32(out var offset))
            {
                return Truncated();
            }

            answer = ReadCount(ref reader, meter, sizeof(uint), sizeof(uint), out var functions);

            if (answer.Refused)
            {
                return answer;
            }

            var entries = new uint[functions];

            for (var entry = 0; entry < entries.Length; entry++)
            {
                if (!reader.TryU32(out entries[entry]))
                {
                    return Truncated();
                }
            }

            read[index] = new WasmElementDefinition(offset, System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(entries));
        }

        elements = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(read);
        return UbcHookAnswer.Admit;
    }

    /// <summary>Reads the data segments, each a window onto the section's own bytes; a segment needs the memory it writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=360308
    // Broiler-Falsified-If: a segment is read when no memory is declared, or its bytes are taken past the end of the section
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadData(ref Cursor reader, WasmHookMeter meter, bool hasMemory, out ImmutableArray<WasmDataDefinition> data)
    {
        data = ImmutableArray<WasmDataDefinition>.Empty;
        var answer = ReadCount(ref reader, meter, SegmentItemBytes, ItemBytes, out var count);

        if (answer.Refused)
        {
            return answer;
        }

        if (count > 0 && !hasMemory)
        {
            return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.DataSegmentMemoryIndexOutOfRange);
        }

        var read = new WasmDataDefinition[count];

        for (var index = 0; index < read.Length; index++)
        {
            if (!reader.TryU32(out var offset) || !reader.TryU32(out var length) || !reader.TryTake(length, out var contents))
            {
                return Truncated();
            }

            read[index] = new WasmDataDefinition(offset, contents);
        }

        data = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(read);
        return UbcHookAnswer.Admit;
    }

    /// <summary>
    /// Reads the exports, each name a window onto the section's bytes, checks every name is
    /// well-formed UTF-8 and every non-function index addresses what the definitions declare, and sorts
    /// them by name, refusing a name stated twice.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A function export's index is checked by the hook against the artifact's units, which this
    /// codec does not see.
    /// </para>
    /// <para>
    /// <b>The sort is charged at what its comparisons can cost, before it runs.</b> A comparison of two
    /// names stops at the first byte that differs, so it costs at most the shorter name's length, and
    /// the introspective sort's partitions compare each name about once per level. So each of twice
    /// its levels is charged one step per name and the names' bytes in eight-byte steps, and the pass
    /// that numbers the rows before it and the pass that compares neighbours after it are charged with
    /// it. The walk charges its own sort at the longest name's length for every comparison instead;
    /// here that would price a thousand short names at one long name's length each, and refuse at
    /// verification a module the profile's own validator admits.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=ABE35E
    // Broiler-Falsified-If: a name that is not well-formed UTF-8, a kind byte outside the four, an index of a table, memory or global the definitions do not declare, or a name stated twice is admitted, or the sort runs before its charge
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadExports(
        ref Cursor reader,
        WasmHookMeter meter,
        bool hasMemory,
        bool hasTable,
        int globalCount,
        out ImmutableArray<WasmExportDefinition> exports,
        out ImmutableArray<int> order)
    {
        exports = ImmutableArray<WasmExportDefinition>.Empty;
        order = ImmutableArray<int>.Empty;

        // The export rows and their sort order: an item and an index each.
        var answer = ReadCount(ref reader, meter, ExportItemBytes, ItemBytes + sizeof(int), out var count);

        if (answer.Refused)
        {
            return answer;
        }

        var read = new WasmExportDefinition[count];
        var nameBytes = 0UL;

        for (var index = 0; index < read.Length; index++)
        {
            if (!reader.TryU32(out var length) || !reader.TryTake(length, out var name) || !reader.TryU8(out var kind) || !reader.TryU32(out var target))
            {
                return Truncated();
            }

            if (!WasmName.IsWellFormed(name.Span))
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.MalformedNameEncoding);
            }

            if (kind > (byte)WasmExportKind.Global)
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.UnknownExternalKind);
            }

            var addressed = (WasmExportKind)kind switch
            {
                WasmExportKind.Table => hasTable && target == 0,
                WasmExportKind.Memory => hasMemory && target == 0,
                WasmExportKind.Global => target < (uint)globalCount,
                _ => true,
            };

            if (!addressed)
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ExportIndexOutOfRange);
            }

            read[index] = new WasmExportDefinition(name, (WasmExportKind)kind, target);
            nameBytes += (ulong)name.Length;
        }

        // One step per name and its bytes in eight-byte steps: what one level of the sort, and the
        // neighbour pass, can spend. The names are windows onto the section, so the sum cannot overflow.
        var levels = Depth(read.Length);
        var weight = (ulong)read.Length + (nameBytes / 8);

        if (!meter.TryChargeWork((ulong)read.Length + (2 * levels * weight) + weight))
        {
            return meter.Exhausted();
        }

        var sorted = new int[read.Length];

        for (var index = 0; index < sorted.Length; index++)
        {
            sorted[index] = index;
        }

        System.Array.Sort(sorted, (left, right) =>
        {
            var byName = System.MemoryExtensions.SequenceCompareTo(read[left].Name.Span, read[right].Name.Span);
            return byName != 0 ? byName : left.CompareTo(right);
        });

        for (var index = 1; index < sorted.Length; index++)
        {
            if (System.MemoryExtensions.SequenceEqual(read[sorted[index]].Name.Span, read[sorted[index - 1]].Name.Span))
            {
                return WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.DuplicateExportName);
            }
        }

        exports = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(read);
        order = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(sorted);
        return UbcHookAnswer.Admit;
    }

    /// <summary>
    /// Reads a count, refuses it as truncated when the remaining bytes cannot hold that many items of
    /// <paramref name="itemBytes"/> each, then charges it to the declared-count ceiling and reserves
    /// <paramref name="keptBytes"/> for each item the definitions will keep.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=F23076
    // Broiler-Falsified-If: a count the remaining bytes cannot hold is answered, or one is answered before it is charged and its array reserved
    // Broiler-Human:        PENDING
    private static UbcHookAnswer ReadCount(ref Cursor reader, WasmHookMeter meter, ulong itemBytes, ulong keptBytes, out int count)
    {
        count = 0;

        if (!reader.TryU32(out var declared))
        {
            return Truncated();
        }

        // Sixty-four bits cannot overflow here: a thirty-two-bit count times a small item size.
        if ((ulong)declared * itemBytes > (ulong)reader.Remaining)
        {
            return Truncated();
        }

        if (!meter.TryChargeDeclaredCount(declared) || !meter.TryReserve(ArrayBytes + ((ulong)declared * keptBytes)))
        {
            return meter.Exhausted();
        }

        count = (int)declared;
        return UbcHookAnswer.Admit;
    }

    /// <summary>How many levels deep an introspective sort of <paramref name="count"/> items can go, at least one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A56423
    // Broiler-Human:        PENDING
    private static ulong Depth(int count) => count <= 1 ? 1UL : (ulong)System.Numerics.BitOperations.Log2((uint)count) + 2;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E12B8F
    // Broiler-Human:        PENDING
    private static UbcHookAnswer Truncated() =>
        WasmFamilyVerifier.Refuse(WebAssemblyDiagnosticCode.ModuleDefinitionsTruncated);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1A7445
    // Broiler-Human:        PENDING
    private static void U32(System.Collections.Generic.List<byte> output, uint value)
    {
        for (var shift = 0; shift < 32; shift += 8)
        {
            output.Add((byte)(value >> shift));
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=67BDDE
    // Broiler-Human:        PENDING
    private static void U64(System.Collections.Generic.List<byte> output, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
        {
            output.Add((byte)(value >> shift));
        }
    }

    /// <summary>A position in the section's bytes and the fixed-width reads over them, each answering false rather than reading past the end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=ABC8AF
    // Broiler-Falsified-If: a read answers bytes past the end of the section, or a window it takes is longer than what remains
    // Broiler-Human:        PENDING
    private struct Cursor(System.ReadOnlyMemory<byte> body)
    {
        private int at;

        /// <summary>How many bytes remain after the position.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2FC4A8
        // Broiler-Human:        PENDING
        internal readonly int Remaining => body.Length - at;

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=1831FB
        // Broiler-Falsified-If: a byte past the end of the section is answered
        // Broiler-Human:        PENDING
        internal bool TryU8(out byte value)
        {
            value = 0;

            if (Remaining < 1)
            {
                return false;
            }

            value = body.Span[at];
            at++;
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=2C9FAC
        // Broiler-Falsified-If: a field running past the end of the section is answered, or its bytes are read in another order than little-endian
        // Broiler-Human:        PENDING
        internal bool TryU32(out uint value)
        {
            value = 0;

            if (Remaining < 4)
            {
                return false;
            }

            value = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(body.Span.Slice(at, 4));
            at += 4;
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=DABF2D
        // Broiler-Falsified-If: a field running past the end of the section is answered, or its bytes are read in another order than little-endian
        // Broiler-Human:        PENDING
        internal bool TryU64(out ulong value)
        {
            value = 0;

            if (Remaining < 8)
            {
                return false;
            }

            value = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(body.Span.Slice(at, 8));
            at += 8;
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=60146D
        // Broiler-Falsified-If: a window longer than the bytes that remain is answered
        // Broiler-Human:        PENDING
        internal bool TryTake(uint length, out System.ReadOnlyMemory<byte> window)
        {
            window = System.ReadOnlyMemory<byte>.Empty;

            if ((ulong)length > (ulong)Remaining)
            {
                return false;
            }

            window = body.Slice(at, (int)length);
            at += (int)length;
            return true;
        }
    }
}
