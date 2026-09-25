using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Obligation E2 of the universal bytecode for the WebAssembly family: every numeric primitive row of
/// the family's table, the profile's reference arms against the primitive table, over the retained
/// primitive input corpus.
/// </summary>
/// <remarks>
/// <para>
/// <b>Everything it compares is read, and nothing is listed here.</b> The rows are the family's own -
/// <c>WebAssemblyProfile.Registration.Tables[0].Rows</c>, every <c>Primitive</c> row whose primitive is
/// not a region access - and the NaN flag is the table's. The reference is the profile's door to its
/// arms, <see cref="WebAssemblyProfile.TryEvaluateReference"/>; the expected answer is
/// <see cref="UbcPrimitives.Evaluate"/> under the table's flag, computed here for every input line of
/// the file named on the command line that names the row's primitive. The file is never defaulted.
/// </para>
/// <para>
/// <b>How two answers agree.</b> A trap agrees when the primitive's trap, mapped through the row's own
/// trap mappings to the family's code, is the reference's <see cref="WasmTrapKind"/>; a primitive trap
/// the row maps to nothing disagrees. A value agrees when its bits are equal - except a NaN the
/// specification leaves the payload of, which is the result of a row the NaN rule applies to: with the
/// table's flag on, the reference's NaN canonicalised must equal the primitive's, which requires both to
/// be NaNs and the primitive's to be the canonical one; with it off, both must be NaNs. A sign-bit
/// operation or a reinterpretation is not such a row, and its NaN bits are compared as bits. A row no
/// input line names fails, because a comparison over nothing is no comparison - and for the same
/// reason the lane fails when the table has no numeric row outside the control, or when the control
/// is not exactly one row at each of 0x5B to 0x66.
/// </para>
/// <para>
/// <b>The twelve float comparisons, 0x5B to 0x66, are reported apart, as the control.</b> The arms the
/// reference reads are the profile's own. Until milestone UBC-4 corrected their routing they sent the
/// comparison bytes to the integer arm and had no answer for them; the predeclared rule retained this
/// lane failing on exactly those rows before the correction, and passing after (bundle
/// <c>ubc-4-002</c>). Every other row is counted and reported before them, so a disagreement outside
/// the control can never hide inside it.
/// </para>
/// <para>
/// <b>It runs only under <c>--primitives &lt;file&gt;</c></b>, after every other lane, and its headers
/// open with none of the names the lanes of population B open with.
/// </para>
/// </remarks>
internal static class PrimitiveDifferential
{
    /// <summary>The first float comparison, <c>f32.eq</c>.</summary>
    private const byte FirstComparison = 0x5B;

    /// <summary>The last float comparison, <c>f64.ge</c>.</summary>
    private const byte LastComparison = 0x66;

    /// <summary>How many disagreements of one row are printed.</summary>
    private const int Shown = 4;

    /// <summary>Compares every numeric primitive row and answers how many rows and lane checks failed.</summary>
    /// <param name="path">The input corpus named after <c>--primitives</c>, or null where the flag was the last argument.</param>
    /// <param name="verbose">Whether every agreeing row is printed too.</param>
    internal static int Report(string? path, bool verbose)
    {
        // A FLAG THAT NAMES NO FILE IS A FAILED LANE, never a skipped one: the lane is never
        // defaulted, and a run that asked for it and printed nothing of it would pass on nothing.
        if (path is null)
        {
            Console.WriteLine("# primitive-differential: no input corpus named");
            Console.WriteLine("FAIL --primitives: no file follows the flag, and this lane never defaults one");
            return 1;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine($"# primitive-differential: no input corpus at {path}");
            Console.WriteLine($"FAIL {path}: there is no primitive input corpus to compare over");
            return 1;
        }

        var inputs = new Dictionary<string, List<(ulong A, ulong B)>>(StringComparer.Ordinal);
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

            if (fields.Length < 3 ||
                !ulong.TryParse(fields[1], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var a) ||
                !ulong.TryParse(fields[2], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var b))
            {
                malformed.Add(lineNumber);
                continue;
            }

            if (!inputs.TryGetValue(fields[0], out var list))
            {
                list = [];
                inputs.Add(fields[0], list);
            }

            list.Add((a, b));
        }

        var table = WebAssemblyProfile.Registration.Tables[0];
        var rows = table.Rows
            .Where(static row =>
                row.Kind is UbcInstructionKind.Primitive &&
                row.Primitive is { } primitive &&
                !UbcPrimitives.IsRegionAccess(primitive))
            .OrderBy(static row => row.Opcode)
            .ToArray();

        var others = rows.Where(static row => !IsControl(row.Opcode)).ToArray();
        var control = rows.Where(static row => IsControl(row.Opcode)).ToArray();

        Console.WriteLine(
            $"# primitive-differential: {rows.Length.ToString(CultureInfo.InvariantCulture)} numeric " +
            $"primitive rows of {table.Manifest} table version {table.TableVersion.ToString(CultureInfo.InvariantCulture)}, " +
            $"NaN canonicalised {(table.CanonicaliseNaN ? "yes" : "no")}, inputs from {path}");

        var failed = 0;

        foreach (var number in malformed)
        {
            failed++;
            Console.WriteLine($"FAIL {path}: line {number.ToString(CultureInfo.InvariantCulture)} is not a primitive input");
        }

        // A LANE OVER NO ROWS IS NO COMPARISON. The rows are read from the table, so a table whose
        // numeric rows stopped reading as primitives would otherwise leave this lane agreeing over
        // nothing, and a control that lost a comparison would shrink without a line saying so.
        if (others.Length == 0)
        {
            failed++;
            Console.WriteLine(
                $"FAIL {table.Manifest}: the table has no numeric primitive row outside the control, " +
                "so there is nothing to compare");
        }

        var otherFailures = 0;

        foreach (var row in others)
        {
            otherFailures += Row(row, table.CanonicaliseNaN, inputs, path, verbose);
        }

        Console.WriteLine(
            $"# primitive-differential: {(others.Length - otherFailures).ToString(CultureInfo.InvariantCulture)} " +
            $"of {others.Length.ToString(CultureInfo.InvariantCulture)} rows outside the control agreed");

        Console.WriteLine(
            $"# primitive-differential control: {control.Length.ToString(CultureInfo.InvariantCulture)} rows, " +
            $"the float comparisons 0x{FirstComparison:X2}-0x{LastComparison:X2}, " +
            "against the profile's reference arms");

        for (var opcode = FirstComparison; opcode <= LastComparison; opcode++)
        {
            var at = opcode;

            if (control.Count(row => row.Opcode == at) != 1)
            {
                failed++;
                Console.WriteLine(
                    $"FAIL {table.Manifest}: the table has no single numeric primitive row at 0x{at:X2}, " +
                    $"so the control is not the {(LastComparison - FirstComparison + 1).ToString(CultureInfo.InvariantCulture)} " +
                    "float comparisons");
            }
        }

        var controlFailures = 0;

        foreach (var row in control)
        {
            controlFailures += Row(row, table.CanonicaliseNaN, inputs, path, verbose);
        }

        Console.WriteLine(
            $"# primitive-differential control: {(control.Length - controlFailures).ToString(CultureInfo.InvariantCulture)} " +
            $"of {control.Length.ToString(CultureInfo.InvariantCulture)} rows agreed");

        return failed + otherFailures + controlFailures;
    }

    private static bool IsControl(byte opcode) => opcode is >= FirstComparison and <= LastComparison;

    /// <summary>Compares one row over every input naming its primitive, prints its line, and answers 1 when it failed.</summary>
    private static int Row(
        UbcInstructionRow row,
        bool canonicaliseNaN,
        Dictionary<string, List<(ulong A, ulong B)>> inputs,
        string path,
        bool verbose)
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

        foreach (var (a, b) in lines)
        {
            var complaint = Compare(row, primitive, canonicaliseNaN, a, b);

            if (complaint is null)
            {
                continue;
            }

            disagreed++;

            if (disagreements.Count < Shown)
            {
                disagreements.Add($"({a:X16}, {b:X16}) {complaint}");
            }
        }

        if (disagreed == 0)
        {
            if (verbose)
            {
                Console.WriteLine(
                    $"ok   {name}: {lines.Count.ToString(CultureInfo.InvariantCulture)} inputs agreed, 0 disagreed");
            }

            return 0;
        }

        Console.WriteLine(
            $"FAIL {name}: {(lines.Count - disagreed).ToString(CultureInfo.InvariantCulture)} inputs agreed, " +
            $"{disagreed.ToString(CultureInfo.InvariantCulture)} disagreed: {string.Join("; ", disagreements)}");

        return 1;
    }

    /// <summary>Whether the reference and the primitive agree over one input, and what differs where they do not.</summary>
    private static string? Compare(UbcInstructionRow row, UbcPrimitive primitive, bool canonicaliseNaN, ulong a, ulong b)
    {
        var expected = UbcPrimitives.Evaluate(primitive, a, b, canonicaliseNaN);
        var answered = WebAssemblyProfile.TryEvaluateReference(row.Opcode, a, b, out var bits, out var trap);

        if (!answered)
        {
            return $"the reference arms have no answer, and the primitive answers {Describe(expected)}";
        }

        if (expected.Trap != UbcTrapCode.None)
        {
            var mapped = row.Traps.Where(mapping => mapping.Universal == expected.Trap).ToArray();

            if (mapped.Length != 1)
            {
                return $"the primitive raises {expected.Trap}, which the row maps to no trap";
            }

            return trap != 0 && (ushort)trap == mapped[0].FamilyCode
                ? null
                : $"the reference answers {Describe(bits, trap)}, and the primitive raises {expected.Trap}, " +
                  $"the family's trap {((WasmTrapKind)mapped[0].FamilyCode).ToString()}";
        }

        if (trap != 0)
        {
            return $"the reference traps {trap}, and the primitive answers {Describe(expected)}";
        }

        return Agrees(primitive, canonicaliseNaN, bits, expected.Bits)
            ? null
            : $"the reference answers {Describe(bits, trap)}, and the primitive {Describe(expected)}";
    }

    /// <summary>Whether two value answers agree under the NaN rule this lane states.</summary>
    private static bool Agrees(UbcPrimitive primitive, bool canonicaliseNaN, ulong reference, ulong expected)
    {
        if (!UbcPrimitives.Canonicalises(primitive) ||
            !UbcPrimitives.TryGetSignature(primitive, out _, out var results) ||
            results.Length != 1)
        {
            return reference == expected;
        }

        var single = results[0] is UbcSlotType.F32;
        var expectedNaN = single
            ? float.IsNaN(BitConverter.UInt32BitsToSingle((uint)expected))
            : double.IsNaN(BitConverter.UInt64BitsToDouble(expected));

        if (!expectedNaN)
        {
            return reference == expected;
        }

        var referenceNaN = single
            ? float.IsNaN(BitConverter.UInt32BitsToSingle((uint)reference)) && reference <= uint.MaxValue
            : double.IsNaN(BitConverter.UInt64BitsToDouble(reference));

        if (!canonicaliseNaN)
        {
            return referenceNaN;
        }

        var canonical = single ? 0x7FC0_0000UL : 0x7FF8_0000_0000_0000UL;
        return referenceNaN && canonical == expected;
    }

    private static string Describe(UbcPrimitiveResult result) =>
        result.Trap != UbcTrapCode.None ? $"trap {result.Trap}" : result.Bits.ToString("X16", CultureInfo.InvariantCulture);

    private static string Describe(ulong bits, WasmTrapKind trap) =>
        trap != 0 ? $"trap {trap}" : bits.ToString("X16", CultureInfo.InvariantCulture);
}
