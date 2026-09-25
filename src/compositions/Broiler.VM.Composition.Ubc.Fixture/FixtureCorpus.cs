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
/// emission - and runs the retained bytes, not the lowering's, so a published image answers for the
/// files. It prints a failure-class table, one line per entry, for the publish modes to be compared
/// byte for byte.
/// </para>
/// <para>
/// <b>The primitive input corpus.</b> For every entry of the primitive table outside the region
/// accesses, the cross product of an edge set per operand type - every NaN class, both signed zeros,
/// the integer extremes and the conversion boundaries - with the answer the fixture family's handler
/// gives, which for a primitive row is the table's reference implementation, under both settings of
/// the NaN flag. It is shared by every later emitter's differential check.
/// </para>
/// </remarks>
internal static class FixtureCorpus
{
    private const string Manifest = "corpus.manifest";
    private const string Primitives = "primitives.txt";

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

        File.WriteAllBytes(Path.Combine(directory, "guest-1.bubc"), TallyPrograms.Guest.ToArray());
        manifest.Append(CultureInfo.InvariantCulture, $"file|guest-1.bubc|{Hash(TallyPrograms.Guest.AsSpan())}\n");

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
                var lowered = TallyPrograms.Named(name).Artifact.AsSpan();
                string answer;

                if (!string.Equals(Hash(bytes), hash, StringComparison.Ordinal))
                {
                    answer = "MUTATED: the retained bytes do not hash to the recorded hash";
                }
                else if (!lowered.SequenceEqual(bytes))
                {
                    answer = "MOVED: the lowering no longer writes the retained bytes";
                }
                else
                {
                    using var runtime = FixtureHost.Runtime();
                    answer = FixtureHost.Transcript(runtime, bytes, entry).Replace("\n", " | ", StringComparison.Ordinal);
                }

                var agrees = string.Equals(answer, recorded, StringComparison.Ordinal);
                failures += agrees ? 0 : 1;
                Console.WriteLine($"{(agrees ? "ok  " : "FAIL")} {name} {answer}");
                continue;
            }

            var file = fields[1];
            var content = File.ReadAllBytes(Path.Combine(directory, file));
            var intact = string.Equals(Hash(content), fields[2], StringComparison.Ordinal);

            if (intact && file == Primitives)
            {
                var (checkedCount, wrong) = CheckPrimitives(Encoding.UTF8.GetString(content));
                intact = wrong == 0;
                Console.WriteLine($"{(intact ? "ok  " : "FAIL")} {file} {checkedCount.ToString(CultureInfo.InvariantCulture)} inputs, {wrong.ToString(CultureInfo.InvariantCulture)} answers differ");
            }
            else
            {
                Console.WriteLine($"{(intact ? "ok  " : "FAIL")} {file} {(intact ? "intact" : "MUTATED")}");
            }

            failures += intact ? 0 : 1;
        }

        Console.WriteLine($"broiler-vm-composition-ubc-fixture corpus: {entries.ToString(CultureInfo.InvariantCulture)} entries, {failures.ToString(CultureInfo.InvariantCulture)} failures");
        _ = verbose;
        return failures == 0 && entries > 0 ? 0 : 1;
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

    private static readonly ulong[] F32Edges =
    [
        0x0000_0000, 0x8000_0000, 0x3F80_0000, 0xBF80_0000, 0x3F00_0000, 0x3FC0_0000, 0x4020_0000, 0xC020_0000,
        0x7F7F_FFFF, 0x0080_0000, 0x0000_0001, 0x007F_FFFF, 0x7F80_0000, 0xFF80_0000, 0x7FC0_0000, 0xFFC0_0000,
        0x7F80_0001, 0x7FC1_2345, 0x4F00_0000, 0xCF00_0000, 0x4F80_0000, 0x5F00_0000, 0x5F80_0000, 0xDF00_0000,
    ];

    private static readonly ulong[] F64Edges =
    [
        0x0000_0000_0000_0000, 0x8000_0000_0000_0000, 0x3FF0_0000_0000_0000, 0xBFF0_0000_0000_0000, 0x3FE0_0000_0000_0000,
        0x3FF8_0000_0000_0000, 0x4004_0000_0000_0000, 0xC004_0000_0000_0000, 0x7FEF_FFFF_FFFF_FFFF, 0x0010_0000_0000_0000,
        0x0000_0000_0000_0001, 0x000F_FFFF_FFFF_FFFF, 0x7FF0_0000_0000_0000, 0xFFF0_0000_0000_0000, 0x7FF8_0000_0000_0000,
        0xFFF8_0000_0000_0000, 0x7FF0_0000_0000_0001, 0x7FF8_0000_1234_5678, 0x41E0_0000_0000_0000, 0xC1E0_0000_0000_0000,
        0x41F0_0000_0000_0000, 0x43E0_0000_0000_0000, 0x43F0_0000_0000_0000, 0xC3E0_0000_0000_0000,
    ];

    private static ulong[] EdgesOf(UbcSlotType type) => type switch
    {
        UbcSlotType.I32 => I32Edges,
        UbcSlotType.I64 => I64Edges,
        UbcSlotType.F32 => F32Edges,
        _ => F64Edges,
    };

    private static string PrimitiveCorpus()
    {
        var text = new StringBuilder();
        text.Append("# primitive|a|b|answer|answer-with-nan-canonicalised - operands and answers as hexadecimal word bits,\n");
        text.Append("# an answer as 'trap:<universal code>' where the primitive traps; b is 0 for a unary primitive\n");

        foreach (var primitive in Enum.GetValues<UbcPrimitive>())
        {
            if (!UbcPrimitives.IsDefined(primitive) || UbcPrimitives.IsRegionAccess(primitive) ||
                !UbcPrimitives.TryGetSignature(primitive, out var operands, out _))
            {
                continue;
            }

            var first = operands.Length > 0 ? EdgesOf(operands[0]) : [0UL];
            var second = operands.Length > 1 ? EdgesOf(operands[1]) : [0UL];

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
