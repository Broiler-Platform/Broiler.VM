using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Obligation E2 of the universal bytecode for the WebAssembly family's region rows: every load, every
/// store and <c>memory.size</c>, the profile's reference arms against the primitive table, over the
/// retained region input corpus.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the numeric lane's other half, and it is a separate lane because a region row reads a
/// region.</b> The rows are the family's own - every <c>Primitive</c> row of
/// <c>WebAssemblyProfile.Registration.Tables[0]</c> whose primitive is a region access. The reference
/// is the profile's door to its memory arms, <see cref="WebAssemblyProfile.TryEvaluateReferenceAccess"/>,
/// and the expected answer is <see cref="UbcPrimitives.EvaluateRegion"/>, computed here for every input
/// line of the file named on the command line that names the row's primitive. The file is never
/// defaulted.
/// </para>
/// <para>
/// <b>What is compared.</b> Each input line names a region length, which is filled by the rule the
/// file's header states, and each side runs over its own copy of it. A trap agrees when the primitive's
/// trap, mapped through the row's own trap mappings to the family's code, is the reference's
/// <see cref="WasmTrapKind"/>. A value agrees when its bits are equal. A store agrees only when, in
/// addition, the two regions are equal byte for byte afterwards, so a store that wrote a byte beside its
/// window, or wrote anything when it trapped, disagrees. <c>memory.size</c> answers pages: the reference
/// runs over a memory of the line's size in pages, and the primitive is handed that size.
/// </para>
/// <para>
/// <b>It fails, and never passes over nothing</b>, when no file follows the flag, when the file is
/// missing or has a line that is not a region input, when the table has no region row, and when a
/// region row has no input line.
/// </para>
/// </remarks>
internal static class RegionDifferential
{
    /// <summary>A WebAssembly page, in bytes, which is what <c>memory.size</c> counts.</summary>
    private const int PageBytes = 65_536;

    /// <summary>How many disagreements of one row are printed.</summary>
    private const int Shown = 4;

    /// <summary>Compares every region row and answers how many rows and lane checks failed.</summary>
    /// <param name="path">The input corpus named after <c>--regions</c>, or null where the flag was the last argument.</param>
    /// <param name="verbose">Whether every agreeing row is printed too.</param>
    internal static int Report(string? path, bool verbose)
    {
        if (path is null)
        {
            Console.WriteLine("# region-differential: no input corpus named");
            Console.WriteLine("FAIL --regions: no file follows the flag, and this lane never defaults one");
            return 1;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine($"# region-differential: no input corpus at {path}");
            Console.WriteLine($"FAIL {path}: there is no region input corpus to compare over");
            return 1;
        }

        var inputs = new Dictionary<string, List<Input>>(StringComparer.Ordinal);
        var malformed = new List<int>();
        var lineNumber = 0;

        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var fields = line.Split('|');

            if (fields.Length < 6 ||
                !int.TryParse(fields[1], NumberStyles.None, CultureInfo.InvariantCulture, out var length) ||
                !uint.TryParse(fields[2], NumberStyles.None, CultureInfo.InvariantCulture, out var size) ||
                !uint.TryParse(fields[3], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var address) ||
                !uint.TryParse(fields[4], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var offset) ||
                !ulong.TryParse(fields[5], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value) ||
                length > PageBytes ||
                size > 4)
            {
                malformed.Add(lineNumber);
                continue;
            }

            if (!inputs.TryGetValue(fields[0], out var list))
            {
                list = [];
                inputs.Add(fields[0], list);
            }

            list.Add(new Input(length, size, address, offset, value));
        }

        var table = WebAssemblyProfile.Registration.Tables[0];
        var rows = table.Rows
            .Where(static row =>
                row.Kind is UbcInstructionKind.Primitive &&
                row.Primitive is { } primitive &&
                UbcPrimitives.IsRegionAccess(primitive))
            .OrderBy(static row => row.Opcode)
            .ToArray();

        Console.WriteLine(
            $"# region-differential: {rows.Length.ToString(CultureInfo.InvariantCulture)} region " +
            $"primitive rows of {table.Manifest} table version {table.TableVersion.ToString(CultureInfo.InvariantCulture)}, " +
            $"inputs from {path}");

        var failed = 0;

        foreach (var number in malformed)
        {
            failed++;
            Console.WriteLine($"FAIL {path}: line {number.ToString(CultureInfo.InvariantCulture)} is not a region input");
        }

        if (rows.Length == 0)
        {
            failed++;
            Console.WriteLine($"FAIL {table.Manifest}: the table has no region primitive row, so there is nothing to compare");
        }

        var rowFailures = 0;

        foreach (var row in rows)
        {
            rowFailures += Row(row, inputs, path, verbose);
        }

        Console.WriteLine(
            $"# region-differential: {(rows.Length - rowFailures).ToString(CultureInfo.InvariantCulture)} " +
            $"of {rows.Length.ToString(CultureInfo.InvariantCulture)} rows agreed");

        return failed + rowFailures;
    }

    /// <summary>Compares one row over every input naming its primitive, prints its line, and answers 1 when it failed.</summary>
    private static int Row(UbcInstructionRow row, Dictionary<string, List<Input>> inputs, string path, bool verbose)
    {
        var primitive = row.Primitive!.Value;
        var name = $"{row.Mnemonic} (0x{row.Opcode:X2}, {primitive})";

        if (!inputs.TryGetValue(primitive.ToString(), out var lines) || lines.Count == 0)
        {
            Console.WriteLine($"FAIL {name}: no input line of {path} names {primitive}");
            return 1;
        }

        var disagreements = new List<string>();
        var disagreed = 0;
        var trapped = 0;

        foreach (var input in lines)
        {
            var complaint = Compare(row, primitive, input, out var trap);
            trapped += trap ? 1 : 0;

            if (complaint is null)
            {
                continue;
            }

            disagreed++;

            if (disagreements.Count < Shown)
            {
                disagreements.Add(
                    $"(length {input.Length.ToString(CultureInfo.InvariantCulture)}, address {input.Address:X8}, " +
                    $"offset {input.Offset:X8}, value {input.Value:X16}) {complaint}");
            }
        }

        if (disagreed == 0)
        {
            if (verbose)
            {
                Console.WriteLine(
                    $"ok   {name}: {lines.Count.ToString(CultureInfo.InvariantCulture)} inputs agreed, " +
                    $"{trapped.ToString(CultureInfo.InvariantCulture)} of them traps, 0 disagreed");
            }

            return 0;
        }

        Console.WriteLine(
            $"FAIL {name}: {(lines.Count - disagreed).ToString(CultureInfo.InvariantCulture)} inputs agreed, " +
            $"{disagreed.ToString(CultureInfo.InvariantCulture)} disagreed: {string.Join("; ", disagreements)}");

        return 1;
    }

    /// <summary>Whether the reference and the primitive agree over one input, and what differs where they do not.</summary>
    private static string? Compare(UbcInstructionRow row, UbcPrimitive primitive, Input input, out bool trapped)
    {
        trapped = false;
        var sizing = primitive == UbcPrimitive.RegionSize;

        var expectedRegion = sizing ? [] : Fill(input.Length);
        var referenceRegion = sizing ? new byte[checked((int)input.Size * PageBytes)] : Fill(input.Length);

        var expected = UbcPrimitives.EvaluateRegion(
            primitive, expectedRegion, input.Address, input.Offset, input.Value, input.Size);

        var answered = WebAssemblyProfile.TryEvaluateReferenceAccess(
            row.Opcode, referenceRegion, input.Address, input.Offset, input.Value, out var bits, out var trap);

        if (!answered)
        {
            return $"the reference arms have no answer, and the primitive answers {Describe(expected)}";
        }

        if (expected.Trap != UbcTrapCode.None)
        {
            trapped = true;
            var mapped = row.Traps.Where(mapping => mapping.Universal == expected.Trap).ToArray();

            if (mapped.Length != 1)
            {
                return $"the primitive raises {expected.Trap}, which the row maps to no trap";
            }

            if (trap == 0 || (ushort)trap != mapped[0].FamilyCode)
            {
                return $"the reference answers {Describe(bits, trap)}, and the primitive raises {expected.Trap}, " +
                       $"the family's trap {((WasmTrapKind)mapped[0].FamilyCode).ToString()}";
            }
        }
        else if (trap != 0)
        {
            return $"the reference traps {trap}, and the primitive answers {Describe(expected)}";
        }
        else if (bits != expected.Bits)
        {
            return $"the reference answers {Describe(bits, trap)}, and the primitive {Describe(expected)}";
        }

        // A store's answer is what it wrote, and so is a load's or a trap's: nothing. The regions are
        // compared whatever the row, so a write beside the window or on a trap is a disagreement.
        if (!sizing && !referenceRegion.AsSpan().SequenceEqual(expectedRegion))
        {
            return "the two regions differ afterwards";
        }

        return null;
    }

    /// <summary>The region the corpus's header states: byte <c>i</c> is <c>0xA5</c> exclusive-or <c>0x3B * i</c>, modulo 256.</summary>
    private static byte[] Fill(int length)
    {
        var region = new byte[length];

        for (var index = 0; index < length; index++)
        {
            region[index] = (byte)(0xA5 ^ (0x3B * index));
        }

        return region;
    }

    private static string Describe(UbcPrimitiveResult result) =>
        result.Trap != UbcTrapCode.None ? $"trap {result.Trap}" : result.Bits.ToString("X16", CultureInfo.InvariantCulture);

    private static string Describe(ulong bits, WasmTrapKind trap) =>
        trap != 0 ? $"trap {trap}" : bits.ToString("X16", CultureInfo.InvariantCulture);

    /// <summary>One input line: the region's length, a size for <c>memory.size</c>, and the access.</summary>
    private readonly record struct Input(int Length, uint Size, uint Address, uint Offset, ulong Value);
}
