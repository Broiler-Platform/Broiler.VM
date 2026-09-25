using Broiler.VM;
using Broiler.VM.Ubc;
using Com.Example.Tally;
using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// The retained corpora of the programme's UBC-2.4: the program corpus with its transcripts, the guest
/// program, and the primitive input corpus with the family handler's answer for every input.
/// </summary>
/// <remarks>
/// <para>
/// <b>Written from the lowering, replayed from the files.</b> <see cref="Write"/> retains what the
/// fixed program list lowers to and what a run of it answers; <see cref="Replay"/> reads the retained
/// bytes, checks each against its recorded hash and against the lowering's bytes - one program, one
/// emission - and runs the retained bytes, the retained guest included, not the lowering's, so a
/// published image answers for the files. The retained primitive inputs must be what the table yields
/// now, and a program of the fixed list, the guest or the primitive inputs that the manifest leaves out
/// is a failure as much as one it gets wrong. It prints a failure-class table, one line per entry, for
/// the publish modes to be compared byte for byte.
/// </para>
/// <para>
/// <b>The primitive input corpus.</b> For every entry of the primitive table outside the region
/// accesses, the cross product of an edge set per operand type - every NaN class by sign and kind, both
/// signed zeros, the integer extremes and the conversion boundaries - and, for a unary entry, both
/// sides of every boundary a conversion traps, saturates or rounds at, with the answer the fixture
/// family's handler gives, which for a primitive row is the table's reference implementation, under
/// both settings of the NaN flag. <c>word.keep</c>, which keeps a word of any type, takes every type's
/// edges. It is shared by every later emitter's differential check.
/// </para>
/// </remarks>
internal static class FixtureCorpus
{
    private const string Manifest = "corpus.manifest";
    private const string Primitives = "primitives.txt";
    private static readonly string Guest = TallyPrograms.GuestName(1) + ".bubc";

    /// <summary>Retains the corpora in <paramref name="directory"/>.</summary>
    internal static int Write(string directory)
    {
        Directory.CreateDirectory(directory);
        var manifest = new StringBuilder();
        manifest.Append("# the universal bytecode fixture family's retained corpora: programs, the guest program, the primitive inputs\n");
        manifest.Append("# program|name|sha256|entry|transcript (lines joined by ' | ')\n");
        manifest.Append("# file|name|sha256\n");

        foreach (var program in TallyPrograms.All)
        {
            using var runtime = FixtureHost.Runtime();
            var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry);

            if (!string.Equals(transcript, program.Expected, StringComparison.Ordinal))
            {
                Console.WriteLine($"not written: {program.Name} answered '{transcript}' and '{program.Expected}' was expected");
                return 1;
            }

            var file = program.Name + ".bubc";
            File.WriteAllBytes(Path.Combine(directory, file), program.Artifact.ToArray());
            manifest.Append(CultureInfo.InvariantCulture, $"program|{program.Name}|{Hash(program.Artifact.AsSpan())}|{program.Entry}|{transcript.Replace("\n", " | ", StringComparison.Ordinal)}\n");
        }

        File.WriteAllBytes(Path.Combine(directory, Guest), TallyPrograms.Guest.ToArray());
        manifest.Append(CultureInfo.InvariantCulture, $"file|{Guest}|{Hash(TallyPrograms.Guest.AsSpan())}\n");

        var primitives = Encoding.UTF8.GetBytes(PrimitiveCorpus());
        File.WriteAllBytes(Path.Combine(directory, Primitives), primitives);
        manifest.Append(CultureInfo.InvariantCulture, $"file|{Primitives}|{Hash(primitives)}\n");

        File.WriteAllText(Path.Combine(directory, Manifest), manifest.ToString(), new UTF8Encoding(false));
        Console.WriteLine($"written: {TallyPrograms.All.Length.ToString(CultureInfo.InvariantCulture)} programs, the guest program and the primitive input corpus");
        return 0;
    }

    /// <summary>
    /// Replays the corpora retained in <paramref name="directory"/>: hashes, the lowering's bytes, every
    /// program's transcript, and every primitive answer. Prints the failure-class table.
    /// </summary>
    internal static int Replay(string directory, bool verbose)
    {
        var failures = 0;
        var entries = 0;
        var programs = new HashSet<string>(StringComparer.Ordinal);
        var files = new HashSet<string>(StringComparer.Ordinal);

        // The programs that load a guest are run against the retained guest, not the lowering's.
        var guestPath = Path.Combine(directory, Guest);
        ReadOnlyMemory<byte>? guest = File.Exists(guestPath) ? File.ReadAllBytes(guestPath) : null;

        foreach (var line in File.ReadAllLines(Path.Combine(directory, Manifest)))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var fields = line.Split('|');
            entries++;

            if (fields[0] == "program")
            {
                var (name, hash, entry, recorded) = (fields[1], fields[2], fields[3], string.Join('|', fields[4..]));
                var bytes = File.ReadAllBytes(Path.Combine(directory, name + ".bubc"));
                var known = TallyPrograms.All.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.Ordinal));
                string answer;

                if (!programs.Add(name))
                {
                    answer = "DUPLICATE: the manifest names the program twice";
                }
                else if (!string.Equals(Hash(bytes), hash, StringComparison.Ordinal))
                {
                    answer = "MUTATED: the retained bytes do not hash to the recorded hash";
                }
                else if (known is null)
                {
                    answer = "UNKNOWN: the fixed list has no program of this name";
                }
                else if (!known.Artifact.AsSpan().SequenceEqual(bytes))
                {
                    answer = "MOVED: the lowering no longer writes the retained bytes";
                }
                else
                {
                    using var runtime = FixtureHost.Runtime(guest: guest);
                    answer = FixtureHost.Transcript(runtime, bytes, entry).Replace("\n", " | ", StringComparison.Ordinal);
                }

                var agrees = string.Equals(answer, recorded, StringComparison.Ordinal);
                failures += agrees ? 0 : 1;
                Console.WriteLine($"{(agrees ? "ok  " : "FAIL")} {name} {answer}");
                continue;
            }

            var file = fields[1];
            var content = File.ReadAllBytes(Path.Combine(directory, file));
            string verdict;

            if (!files.Add(file))
            {
                verdict = "DUPLICATE: the manifest names the file twice";
            }
            else if (!string.Equals(Hash(content), fields[2], StringComparison.Ordinal))
            {
                verdict = "MUTATED";
            }
            else if (file == Guest)
            {
                verdict = TallyPrograms.Guest.AsSpan().SequenceEqual(content)
                    ? "intact"
                    : "MOVED: the lowering no longer writes the retained guest";
            }
            else if (file == Primitives)
            {
                var text = Encoding.UTF8.GetString(content);
                var (checkedCount, wrong) = CheckPrimitives(text);
                verdict = wrong != 0
                    ? $"{checkedCount.ToString(CultureInfo.InvariantCulture)} inputs, {wrong.ToString(CultureInfo.InvariantCulture)} answers differ"
                    : !string.Equals(text, PrimitiveCorpus(), StringComparison.Ordinal)
                        ? "MOVED: the primitive table no longer yields the retained inputs"
                        : $"{checkedCount.ToString(CultureInfo.InvariantCulture)} inputs, 0 answers differ";
            }
            else
            {
                verdict = "UNKNOWN: the corpus retains no file of this name";
            }

            var intact = verdict == "intact" || verdict.EndsWith(" 0 answers differ", StringComparison.Ordinal);
            failures += intact ? 0 : 1;
            Console.WriteLine($"{(intact ? "ok  " : "FAIL")} {file} {verdict}");
        }

        // What the manifest leaves out is as much a failure as what it gets wrong.
        foreach (var program in TallyPrograms.All.Where(p => !programs.Contains(p.Name)))
        {
            failures++;
            Console.WriteLine($"FAIL {program.Name} MISSING: the fixed list has a program the corpus does not retain");
        }

        foreach (var file in new[] { Guest, Primitives }.Where(f => !files.Contains(f)))
        {
            failures++;
            Console.WriteLine($"FAIL {file} MISSING: the corpus does not retain it");
        }

        Console.WriteLine($"broiler-vm-composition-ubc-fixture corpus: {entries.ToString(CultureInfo.InvariantCulture)} entries, {failures.ToString(CultureInfo.InvariantCulture)} failures");
        _ = verbose;
        return failures == 0 ? 0 : 1;
    }

    /// <summary>The same artifact with its header naming another form.</summary>
    internal static byte[] WithForm(ReadOnlySpan<byte> artifact, string form)
    {
        var bounds = new VmReadBounds((ulong)artifact.Length, 64, 1 << 20, 16);

        if (!UbcArtifactReader.TryRead(artifact, in bounds, new Unmetered(), 4096, UbcFormat.FormatVersion, out var decoded, out var refusal))
        {
            throw new InvalidOperationException($"the lowering's own artifact did not read: {refusal.Code.ToString(CultureInfo.InvariantCulture)}");
        }

        var a = decoded!;
        var header = new UbcHeader(a.Header.FormatVersion, a.Header.ProfileIdentity, a.Header.ManifestIdentity, form, a.Header.TranslatorIdentity, a.Header.TranslatorVersion);
        return UbcArtifactWriter.Write(new UbcArtifact(
            header, a.Families, a.Types, a.Units, a.Code, a.JumpTables, a.Regions, a.Entries, a.Positions, a.FamilyData, a.Emission, a.PresentSections));
    }

    private static string Hash(ReadOnlySpan<byte> bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

    // ---- the primitive input corpus ---------------------------------------------------------------

    private static readonly ulong[] I32Edges =
    [
        0, 1, 2, 0xFFFF_FFFF, 0x7FFF_FFFF, 0x8000_0000, 0x8000_0001, 31, 32, 33, 0x5555_5555, 0xAAAA_AAAA, 0x0000_FFFF, 0xFFFF_0000,
    ];

    private static readonly ulong[] I64Edges =
    [
        0, 1, 2, ulong.MaxValue, 0x7FFF_FFFF_FFFF_FFFF, 0x8000_0000_0000_0000, 0x8000_0000_0000_0001, 63, 64, 65,
        0x5555_5555_5555_5555, 0xAAAA_AAAA_AAAA_AAAA, 0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_0000_0000, 0x0000_0000_8000_0000,
    ];

    // Every NaN class by sign and kind: quiet and signalling, positive and negative, and a payload.
    private static readonly ulong[] F32Edges =
    [
        0x0000_0000, 0x8000_0000, 0x3F80_0000, 0xBF80_0000, 0x3F00_0000, 0x3FC0_0000, 0x4020_0000, 0xC020_0000,
        0x7F7F_FFFF, 0x0080_0000, 0x0000_0001, 0x007F_FFFF, 0x7F80_0000, 0xFF80_0000, 0x7FC0_0000, 0xFFC0_0000,
        0x7F80_0001, 0xFF80_0001, 0x7FC1_2345, 0x4F00_0000, 0xCF00_0000, 0x4F80_0000, 0x5F00_0000, 0x5F80_0000, 0xDF00_0000,
    ];

    private static readonly ulong[] F64Edges =
    [
        0x0000_0000_0000_0000, 0x8000_0000_0000_0000, 0x3FF0_0000_0000_0000, 0xBFF0_0000_0000_0000, 0x3FE0_0000_0000_0000,
        0x3FF8_0000_0000_0000, 0x4004_0000_0000_0000, 0xC004_0000_0000_0000, 0x7FEF_FFFF_FFFF_FFFF, 0x0010_0000_0000_0000,
        0x0000_0000_0000_0001, 0x000F_FFFF_FFFF_FFFF, 0x7FF0_0000_0000_0000, 0xFFF0_0000_0000_0000, 0x7FF8_0000_0000_0000,
        0xFFF8_0000_0000_0000, 0x7FF0_0000_0000_0001, 0xFFF0_0000_0000_0001, 0x7FF8_0000_1234_5678, 0x41E0_0000_0000_0000,
        0xC1E0_0000_0000_0000, 0x41F0_0000_0000_0000, 0x43E0_0000_0000_0000, 0x43F0_0000_0000_0000, 0xC3E0_0000_0000_0000,
    ];

    // The other side of every boundary a conversion traps, saturates or rounds at, for the unary
    // entries alone, so that the binary entries' cross products do not square with them.
    private static readonly ulong[] F32Boundaries =
    [
        0xBF00_0000, 0xBF7F_FFFF,                  // inside (-1, 0): an unsigned truncation's zero, not its trap
        0x4EFF_FFFF, 0xCF00_0001,                  // below 2^31, and the first value past -2^31
        0x4F7F_FFFF,                               // below 2^32
        0x5EFF_FFFF, 0xDF00_0001,                  // below 2^63, and the first value past -2^63
        0x5F7F_FFFF,                               // below 2^64
    ];

    private static readonly ulong[] F64Boundaries =
    [
        0xBFE0_0000_0000_0000, 0xBFEF_FFFF_FFFF_FFFF,                        // inside (-1, 0)
        0x41DF_FFFF_FFC0_0000, 0x41DF_FFFF_FFE0_0000,                        // 2^31 - 1 and 2^31 - 0.5
        0xC1E0_0000_0010_0000, 0xC1E0_0000_0020_0000,                        // -2^31 - 0.5 and -2^31 - 1
        0x41EF_FFFF_FFE0_0000, 0x41EF_FFFF_FFF0_0000,                        // 2^32 - 1 and 2^32 - 0.5
        0x43DF_FFFF_FFFF_FFFF, 0xC3E0_0000_0000_0001, 0x43EF_FFFF_FFFF_FFFF, // the neighbours of 2^63, -2^63 and 2^64
        0x47EF_FFFF_E000_0000, 0x47EF_FFFF_E800_0000, 0x47EF_FFFF_EFFF_FFFF, // the binary32 maximum, and above it rounding down
        0x47EF_FFFF_F000_0000, 0xC7EF_FFFF_F000_0000,                        // the tie that rounds to infinity, both signs
    ];

    // Integers a conversion to binary32 must round once, not through binary64.
    private static readonly ulong[] I64Boundaries =
    [
        0x0020_0000_2000_0001, 0x8000_0080_0000_0001, 0x8000_0000_0000_0401,
    ];

    private static ulong[] EdgesOf(UbcSlotType type) => type switch
    {
        UbcSlotType.I32 => I32Edges,
        UbcSlotType.I64 => I64Edges,
        UbcSlotType.F32 => F32Edges,
        _ => F64Edges,
    };

    private static ulong[] BoundariesOf(UbcSlotType type) => type switch
    {
        UbcSlotType.I64 => I64Boundaries,
        UbcSlotType.F32 => F32Boundaries,
        UbcSlotType.F64 => F64Boundaries,
        _ => [],
    };

    private static string PrimitiveCorpus()
    {
        var text = new StringBuilder();
        text.Append("# primitive|a|b|answer|answer-with-nan-canonicalised - operands and answers as hexadecimal word bits,\n");
        text.Append("# an answer as 'trap:<universal code>' where the primitive traps; b is 0 for a unary primitive\n");

        foreach (var primitive in Covered())
        {
            ulong[] first;
            ulong[] second;

            if (primitive == UbcPrimitive.WordKeep)
            {
                // It keeps a word of any one type: every type's edges, each bit pattern once.
                first = [.. I32Edges.Concat(I64Edges).Concat(F32Edges).Concat(F64Edges).Distinct()];
                second = [0UL];
            }
            else if (UbcPrimitives.TryGetSignature(primitive, out var operands, out _))
            {
                first = operands.Length switch
                {
                    0 => [0UL],
                    1 => [.. EdgesOf(operands[0]).Concat(BoundariesOf(operands[0]))],
                    _ => EdgesOf(operands[0]),
                };
                second = operands.Length > 1 ? EdgesOf(operands[1]) : [0UL];
            }
            else
            {
                throw new InvalidOperationException($"the primitive table's entry {primitive} has no signature and no corpus rule");
            }

            foreach (var a in first)
            {
                foreach (var b in second)
                {
                    text.Append(CultureInfo.InvariantCulture, $"{primitive}|{a:X16}|{b:X16}|{Answer(primitive, a, b, false)}|{Answer(primitive, a, b, true)}\n");
                }
            }
        }

        return text.ToString();
    }

    /// <summary>Every entry of the primitive table outside the region accesses, in table order.</summary>
    private static IEnumerable<UbcPrimitive> Covered() =>
        Enum.GetValues<UbcPrimitive>().Where(static p => UbcPrimitives.IsDefined(p) && !UbcPrimitives.IsRegionAccess(p));

    private static string Answer(UbcPrimitive primitive, ulong a, ulong b, bool canonicalise)
    {
        var result = UbcPrimitives.Evaluate(primitive, a, b, canonicalise);
        return result.Trap != UbcTrapCode.None
            ? "trap:" + result.Trap.ToString()
            : result.Bits.ToString("X16", CultureInfo.InvariantCulture);
    }

    private static (int Checked, int Wrong) CheckPrimitives(string text)
    {
        var count = 0;
        var wrong = 0;

        foreach (var line in text.Split('\n'))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var fields = line.Split('|');
            var primitive = Enum.Parse<UbcPrimitive>(fields[0]);
            var a = ulong.Parse(fields[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var b = ulong.Parse(fields[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            count++;

            if (!string.Equals(Answer(primitive, a, b, false), fields[3], StringComparison.Ordinal) ||
                !string.Equals(Answer(primitive, a, b, true), fields[4], StringComparison.Ordinal))
            {
                wrong++;
            }
        }

        return (count, wrong);
    }

    /// <summary>A meter for reading the root's own artifacts, which charges nothing.</summary>
    private sealed class Unmetered : IVmBoundedAllocationMeter
    {
        public bool TryReserve(ulong byteCount) => true;

        public void Release(ulong byteCount)
        {
        }

        public bool TryChargeWork(ulong workUnits) => true;

        public bool Poll() => true;
    }
}
