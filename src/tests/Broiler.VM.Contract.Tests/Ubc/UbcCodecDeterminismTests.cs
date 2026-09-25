using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// UBC-1.8: the codec is deterministic. Every control, every entry that verifies and a set of sample
/// artifacts reads with <see cref="UbcArtifactReader.TryRead"/> and writes back with
/// <see cref="UbcArtifactWriter.Write"/> to the same bytes, and does so twice.
/// </summary>
/// <remarks>
/// The samples go further than the corpus: every entry the reader admits must round-trip, whatever
/// the walk later says about it, because the round trip is a property of the framing alone - and an
/// artifact carrying every section kind, the Emission section and several FamilyData sections
/// included, is written here and nowhere in the corpus.
/// </remarks>
public sealed class UbcCodecDeterminismTests
{
    private static readonly VmReadBounds Generous = new(1UL << 30, 1024, 1UL << 20, 64);

    [Fact]
    public void Every_Control_And_Every_Verifying_Entry_Reads_And_Rewrites_To_The_Same_Bytes_Twice()
    {
        var checkedEntries = 0;

        foreach (var entry in UbcCorpus.Entries())
        {
            var verifies = UbcCorpusRunner.Run(entry.Bytes, entry.Configuration).Outcome.Category is VmOutcome.Normal;

            if (entry.Family != "control" && !verifies)
            {
                continue;
            }

            AssertRoundTripsTwice(entry.Id, entry.Bytes);
            checkedEntries++;
        }

        Assert.True(checkedEntries >= 16, $"only {checkedEntries} controls and verifying entries were round-tripped");
    }

    [Fact]
    public void Every_Entry_The_Reader_Admits_Rewrites_To_The_Same_Bytes()
    {
        // The framing's own determinism: an artifact the reader decodes is one the canonical writer
        // reproduces, so the walk's refusals and the sweeps' survivors are held to it too.
        var admitted = 0;

        foreach (var entry in UbcCorpus.Entries())
        {
            if (TryRead(entry.Bytes, out _))
            {
                AssertRoundTripsTwice(entry.Id, entry.Bytes);
                admitted++;
            }
        }

        Assert.True(admitted > 100, $"only {admitted} entries were admitted by the reader");
    }

    [Fact]
    public void Sample_Artifacts_Of_Every_Section_Kind_Round_Trip_Twice()
    {
        foreach (var (name, bytes) in Samples())
        {
            AssertRoundTripsTwice(name, bytes);
        }
    }

    [Fact]
    public void The_Sample_Of_Every_Section_Kind_Decodes_To_What_Was_Written()
    {
        var sample = EverySection();
        Assert.True(TryRead(sample.Bytes(), out var artifact), "the every-section sample is refused by the reader");

        foreach (var kind in new[]
                 {
                     UbcSectionKind.Families, UbcSectionKind.Types, UbcSectionKind.Units, UbcSectionKind.Code,
                     UbcSectionKind.JumpTables, UbcSectionKind.Regions, UbcSectionKind.Entries, UbcSectionKind.Positions,
                     UbcSectionKind.FamilyDataFirst, UbcSectionKind.FamilyDataLast, UbcSectionKind.Emission,
                 })
        {
            Assert.True(artifact!.HasSection(kind), $"{kind} was written and not read");
        }

        Assert.Equal(UbcCorpusFamily.Identity, artifact!.Header.ProfileIdentity);
        Assert.Equal(7u, artifact.Header.TranslatorVersion);
        Assert.Equal(new[] { 1, 3, (int)UbcFormat.MaxFamilySlot }, artifact.FamilyData.Select(static data => (int)data.Slot));
        Assert.Equal(sample.Emission!.ShapeHash, artifact.Emission!.ShapeHash);
        Assert.Equal(sample.Emission.Bytes, artifact.Emission.Bytes);
        Assert.Equal(sample.Emission.Symbols, artifact.Emission.Symbols);
        Assert.Equal(int.MaxValue, artifact.Positions[0].Coordinate0);
        Assert.Equal(sample.Units![0].Locals, artifact.Units[0].Locals);
        Assert.Equal(sample.Units[0].Landings, artifact.Units[0].Landings);
    }

    [Fact]
    public void A_Hand_Built_Artifact_Writes_The_Same_Bytes_As_Its_Decoded_Form()
    {
        // The writer's other input: a model built by hand rather than read. Its present sections are
        // derived from its collections, and it must agree with what the spec wrote section by section.
        var spec = EverySection();
        var bytes = spec.Bytes();
        Assert.True(TryRead(bytes, out var read));

        var rebuilt = new UbcArtifact(
            read!.Header, read.Families, read.Types, read.Units, read.Code, read.JumpTables, read.Regions,
            read.Entries, read.Positions, read.FamilyData, read.Emission);

        Assert.Equal(bytes, UbcArtifactWriter.Write(rebuilt));
    }

    private static void AssertRoundTripsTwice(string name, byte[] bytes)
    {
        Assert.True(TryRead(bytes, out var first), $"{name} is refused by the reader");
        var written = UbcArtifactWriter.Write(first!);
        Assert.True(bytes.AsSpan().SequenceEqual(written), $"{name}: the first rewrite differs from the bytes read");

        Assert.True(TryRead(written, out var second), $"{name}: the rewrite is refused by the reader");
        var again = UbcArtifactWriter.Write(second!);
        Assert.True(written.AsSpan().SequenceEqual(again), $"{name}: the second rewrite differs from the first");
    }

    private static bool TryRead(byte[] bytes, out UbcArtifact? artifact) =>
        UbcArtifactReader.TryRead(bytes, in Generous, new OpenMeter(), 4096, null, out artifact, out _);

    private static IEnumerable<(string Name, byte[] Bytes)> Samples()
    {
        yield return ("every-section-kind", EverySection().Bytes());

        var longest = UbcCorpus.Smallest();
        longest.Profile = new string('p', UbcFormat.MaxIdentityBytes);
        longest.Manifest = new string('m', UbcFormat.MaxIdentityBytes);
        longest.Translator = new string('t', UbcFormat.MaxIdentityBytes);
        yield return ("identities-of-the-longest-length", longest.Bytes());

        var multiByte = UbcCorpus.Smallest();
        multiByte.CodeOverride = [.. Enumerable.Repeat((byte)UbcOpcode.Nop, 300), (byte)UbcOpcode.Return];
        multiByte.ChangeUnit(0, static unit => new UbcUnit(
            unit.TypeIndex, unit.FamilySlot, unit.Locals, 200, 300, 0, 301, unit.Flags, [127, 128, 16383, 16384]));
        yield return ("integers-of-two-and-three-bytes", multiByte.Bytes());

        var empties = UbcCorpus.Smallest();
        empties.Families = [];
        empties.JumpTables = [];
        empties.Regions = [];
        empties.Positions = [];
        empties.FamilyData[2] = [];
        empties.Emission = new UbcEmission("x86-64", 0, 1, 0, [], []);
        yield return ("every-optional-section-empty", empties.Bytes());

        yield return ("the-suspendable-control", UbcCorpus.SuspendableUnit().Bytes());
        yield return ("the-every-family-row-control", UbcCorpus.EveryFamilyRow().Bytes());
    }

    /// <summary>An artifact holding every section kind, rows in each; it is framed well and means nothing.</summary>
    private static UbcCorpusSpec EverySection()
    {
        var spec = UbcCorpus.EveryFamilyRow();
        spec.TranslatorVersion = 7;
        spec.Families!.Add(new UbcFamilyEntry(3, "com.example.second", 9, UbcCorpusFamily.WideManifestText));
        spec.Types!.Add(new UbcSignature([UbcSlotType.I32, UbcSlotType.I64, UbcSlotType.F32, UbcSlotType.F64, UbcSlotType.V], [UbcSlotType.V]));
        spec.ChangeUnit(0, static unit => new UbcUnit(
            unit.TypeIndex, unit.FamilySlot,
            [new UbcLocalRun(0, UbcSlotType.F32), new UbcLocalRun(70_000, UbcSlotType.V)],
            unit.MaxWordHeight, unit.MaxValueHeight, unit.CodeOffset, unit.CodeLength, unit.Flags | (UbcUnitFlags)0xAB00, unit.Landings));
        spec.JumpTables = [new UbcJumpTable(0, [0, 1, 2]), new UbcJumpTable(9, [uint.MaxValue])];
        spec.Regions = [new UbcRegion(0, 0, 5, 6, 1, 2, UbcCorpusFamily.CatchKind), new UbcRegion(4, 1, 2, 3, 4, 5, 255)];
        spec.Positions = [new UbcPosition(0, 0, int.MaxValue, 0), new UbcPosition(1, 2, 3, 4)];
        spec.FamilyData[3] = [0xDE, 0xAD];
        spec.FamilyData[UbcFormat.MaxFamilySlot] = [.. Enumerable.Range(0, 300).Select(static value => (byte)value)];
        spec.Emission = new UbcEmission(
            "x86-64", 3, 16, 0xFEDC_BA98_7654_3210,
            [.. Enumerable.Range(0, 200).Select(static value => (byte)(value * 7))],
            [new UbcSymbol(0, 0), new UbcSymbol(1, 64)]);
        return spec;
    }

    private sealed class OpenMeter : IVmBoundedAllocationMeter
    {
        public bool TryReserve(ulong byteCount) => true;

        public void Release(ulong byteCount)
        {
        }

        public bool TryChargeWork(ulong workUnits) => true;

        public bool Poll() => true;
    }
}
