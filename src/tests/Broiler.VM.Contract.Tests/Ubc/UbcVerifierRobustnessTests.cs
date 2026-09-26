using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The verifier's one unconditional promise - no member throws on any artifact, under any bounds - held
/// against inputs the corpus does not enumerate: every corpus artifact under open ceilings, and a
/// bounded, seeded session of mutations of the controls.
/// </summary>
/// <remarks>
/// The session is deterministic: a fixed seed and a fixed count, so a failure names an iteration that
/// reproduces. It is small enough for the suite; its value is that it keeps running the walk over
/// artifacts nobody wrote by hand.
/// </remarks>
public sealed class UbcVerifierRobustnessTests
{
    private static readonly VmOutcome[] VerifierOutcomes =
        [VmOutcome.Normal, VmOutcome.InvalidArtifact, VmOutcome.ResourceExhaustion, VmOutcome.Cancellation];

    /// <summary>Ceilings a host could configure that admit far more than any corpus artifact needs.</summary>
    private static readonly UbcCorpusConfiguration Open = new(ceilings: ImmutableSortedDictionary<VmBudgetDimension, ulong>.Empty
        .Add(VmBudgetDimension.AllocatedBytes, 1UL << 40)
        .Add(VmBudgetDimension.VerifierWork, 1UL << 40)
        .Add(VmBudgetDimension.ArtifactBytes, 1UL << 40)
        .Add(VmBudgetDimension.SectionCount, uint.MaxValue)
        .Add(VmBudgetDimension.DeclaredCount, uint.MaxValue)
        .Add(VmBudgetDimension.StructuralDepth, uint.MaxValue));

    [Fact]
    public void Every_Corpus_Artifact_Answers_Under_Open_Ceilings_Without_Throwing()
    {
        foreach (var entry in UbcCorpus.Entries())
        {
            var configuration = new UbcCorpusConfiguration(
                entry.Configuration.DescriptorFormatVersion,
                0,
                entry.Configuration.DescriptorManifest,
                entry.Configuration.Hook,
                entry.Configuration.Cancelled,
                Open.Ceilings);

            var outcome = UbcCorpusRunner.Run(entry.Bytes, configuration).Outcome;
            Assert.Contains(outcome.Category, VerifierOutcomes);
        }
    }

    [Fact]
    public void A_Seeded_Mutation_Session_Over_The_Controls_Finds_No_Escape_And_Reaches_Both_Answers()
    {
        var seeds = UbcCorpus.Controls().Select(static control => control.Spec.Bytes()).ToArray();
        var random = new Random(0x5EED_0BC1);
        var outcomes = new HashSet<VmOutcome>();
        var codes = new HashSet<int>();

        for (var iteration = 0; iteration < 6_000; iteration++)
        {
            var input = Mutate(seeds[random.Next(seeds.Length)], random);
            UbcCorpusObservation observation;

            try
            {
                observation = UbcCorpusRunner.Run(input, iteration % 2 == 0 ? UbcCorpusConfiguration.Default : Open);
            }
            catch (Exception exception)
            {
                Assert.Fail($"iteration {iteration}: {exception.GetType().Name} escaped the verifier for {Convert.ToHexString(input)}");
                return;
            }

            var outcome = observation.Outcome;
            Assert.Contains(outcome.Category, VerifierOutcomes);
            Assert.Equal(outcome.Category is VmOutcome.Normal, outcome.State is UbcVerifiedProgram);
            outcomes.Add(outcome.Category);
            codes.Add(outcome.ProfileDiagnosticCode);
        }

        Assert.Contains(VmOutcome.Normal, outcomes);
        Assert.Contains(VmOutcome.InvalidArtifact, outcomes);
        Assert.True(codes.Count > 20, $"the session reached only {codes.Count} distinct codes");
    }

    [Fact]
    public void A_Seeded_Session_Of_Well_Framed_Mutations_Reaches_The_Walk_And_Finds_No_Escape()
    {
        // Byte-level edits mostly break the header, which is most of a small artifact. These edit the
        // decoded description instead - code bytes, heights, landings, regions, jump tables and
        // signatures - so the framing stays sound and the walk is what answers.
        var random = new Random(0x0BC1_5EED);
        var codes = new HashSet<int>();
        var verified = 0;

        for (var iteration = 0; iteration < 3_000; iteration++)
        {
            var controls = UbcCorpus.Controls();
            var (_, spec, configuration) = controls[random.Next(controls.Count)];

            for (var edit = random.Next(1, 4); edit > 0; edit--)
            {
                MutateStructure(spec, random);
            }

            var input = spec.Bytes();
            UbcCorpusObservation observation;

            try
            {
                observation = UbcCorpusRunner.Run(input, configuration);
            }
            catch (Exception exception)
            {
                Assert.Fail($"iteration {iteration}: {exception.GetType().Name} escaped the verifier for {Convert.ToHexString(input)}");
                return;
            }

            Assert.Contains(observation.Outcome.Category, VerifierOutcomes);
            codes.Add(observation.Outcome.ProfileDiagnosticCode);
            verified += observation.Outcome.Category is VmOutcome.Normal ? 1 : 0;
        }

        Assert.True(verified > 0, "no well-framed mutation verified");
        Assert.True(codes.Count(static code => code is >= 3300 and < 3700) > 15, "the session reached too few codes of the walk");
    }

    private static void MutateStructure(UbcCorpusSpec spec, Random random)
    {
        var units = spec.Units!;
        var index = random.Next(units.Count);
        var unit = units[index];

        switch (random.Next(8))
        {
            case 0:
            case 1:
            {
                var code = spec.Code;

                if (code.Length > 0)
                {
                    code[random.Next(code.Length)] = (byte)random.Next(256);
                    spec.CodeOverride = code;
                }

                break;
            }

            case 2:
                units[index] = new UbcUnit(
                    unit.TypeIndex, unit.FamilySlot, unit.Locals,
                    (uint)Math.Max(0, (int)unit.MaxWordHeight + random.Next(-2, 2)),
                    (uint)Math.Max(0, (int)unit.MaxValueHeight + random.Next(-2, 2)),
                    unit.CodeOffset, unit.CodeLength, unit.Flags ^ (UbcUnitFlags)(1 << random.Next(2)), unit.Landings);
                break;

            case 3:
            {
                var landings = unit.Landings.ToList();

                if (landings.Count > 0 && random.Next(2) == 0)
                {
                    landings.RemoveAt(random.Next(landings.Count));
                }
                else
                {
                    landings.Add(unit.CodeOffset + (uint)random.Next((int)unit.CodeLength + 1));
                    landings.Sort();
                }

                units[index] = new UbcUnit(
                    unit.TypeIndex, unit.FamilySlot, unit.Locals, unit.MaxWordHeight, unit.MaxValueHeight,
                    unit.CodeOffset, unit.CodeLength, unit.Flags, [.. landings]);
                break;
            }

            case 4 when spec.Regions is { Count: > 0 } regions:
            {
                var at = random.Next(regions.Count);
                var region = regions[at];
                var shift = (uint)random.Next(4);

                regions[at] = random.Next(6) switch
                {
                    0 => new UbcRegion(region.Unit, region.Start + shift, region.End, region.Handler, region.WordEntryHeight, region.ValueEntryHeight, region.Kind),
                    1 => new UbcRegion(region.Unit, region.Start, region.End + shift, region.Handler, region.WordEntryHeight, region.ValueEntryHeight, region.Kind),
                    2 => new UbcRegion(region.Unit, region.Start, region.End, region.Handler + shift, region.WordEntryHeight, region.ValueEntryHeight, region.Kind),
                    3 => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, region.WordEntryHeight + shift, region.ValueEntryHeight, region.Kind),
                    4 => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, region.WordEntryHeight, region.ValueEntryHeight + shift, region.Kind),
                    _ => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, region.WordEntryHeight, region.ValueEntryHeight, (byte)random.Next(4)),
                };

                if (random.Next(4) == 0)
                {
                    regions.Reverse();
                }

                break;
            }

            case 5 when spec.JumpTables is { Count: > 0 } tables:
            {
                var at = random.Next(tables.Count);
                var table = tables[at];
                var targets = table.Targets.ToArray();
                targets[random.Next(targets.Length)] += (uint)random.Next(1, 4);
                tables[at] = new UbcJumpTable(table.Unit, [.. targets]);
                break;
            }

            case 6:
            {
                var types = spec.Types!;
                var at = random.Next(types.Count);
                var signature = types[at];
                var slot = (UbcSlotType)random.Next(1, 6);

                types[at] = random.Next(2) == 0
                    ? new UbcSignature(signature.Parameters.Add(slot), signature.Results)
                    : new UbcSignature(signature.Parameters, signature.Results.IsEmpty ? [slot] : signature.Results.SetItem(0, slot));
                break;
            }

            default:
                units[index] = new UbcUnit(
                    unit.TypeIndex, unit.FamilySlot, unit.Locals.Add(new UbcLocalRun((uint)random.Next(3), (UbcSlotType)random.Next(1, 6))),
                    unit.MaxWordHeight, unit.MaxValueHeight, unit.CodeOffset, unit.CodeLength, unit.Flags, unit.Landings);
                break;
        }
    }

    /// <summary>One to four byte-level edits: an inversion, a random byte, a deletion, an insertion, or a copied run.</summary>
    private static byte[] Mutate(byte[] seed, Random random)
    {
        var bytes = new List<byte>(seed);

        for (var edit = random.Next(1, 5); edit > 0; edit--)
        {
            if (bytes.Count == 0)
            {
                bytes.Add((byte)random.Next(256));
                continue;
            }

            var at = random.Next(bytes.Count);

            switch (random.Next(5))
            {
                case 0:
                    bytes[at] = (byte)~bytes[at];
                    break;

                case 1:
                    bytes[at] = (byte)random.Next(256);
                    break;

                case 2:
                    bytes.RemoveAt(at);
                    break;

                case 3:
                    bytes.Insert(at, (byte)random.Next(256));
                    break;

                default:
                    var length = random.Next(1, Math.Min(16, bytes.Count - at) + 1);
                    bytes.InsertRange(random.Next(bytes.Count), bytes.GetRange(at, length));
                    break;
            }
        }

        return bytes.ToArray();
    }
}
