using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Fixtures;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The universal bytecode's retained malformed corpus (UBC-1.7), replayed against the verifier of a
/// descriptor built by <see cref="UbcDescriptors.Build"/> and registered in no catalog.
/// </summary>
/// <remarks>
/// Every assertion but the regeneration check is made against bytes read back off disk. The generator
/// and the gate are the same code, as for VM-2's corpus: in write mode the first test rewrites the
/// corpus, and otherwise every test asserts it.
/// </remarks>
public sealed class UbcMalformedCorpusTests
{
    private static readonly VmOutcome[] VerifierOutcomes =
    [
        VmOutcome.Normal,
        VmOutcome.InvalidArtifact,
        VmOutcome.ResourceExhaustion,
        VmOutcome.Cancellation,
    ];

    private static readonly Lazy<IReadOnlyList<(UbcCorpusRecord Row, byte[] Bytes, UbcCorpusObservation Observation)>> Replayed =
        new(Replay, LazyThreadSafetyMode.ExecutionAndPublication);

    [Fact]
    public void The_Corpus_On_Disk_Is_What_The_Generator_Writes()
    {
        var entries = UbcCorpus.Entries();
        var rows = Rows(entries);

        if (UbcCorpusStore.WriteRequested)
        {
            UbcCorpusStore.Write(CorpusRunner.Root, entries, rows);
            return;
        }

        var desired = UbcCorpusStore.Render(rows);
        var actual = File.ReadAllText(UbcCorpusStore.ManifestPath(CorpusRunner.Root));

        Assert.True(
            string.Equals(desired, actual, StringComparison.Ordinal),
            $"{UbcCorpusStore.RelativeDirectory}/{UbcCorpusStore.ManifestFileName} is not what the generator would write.\n" +
            $"  Run: {UbcCorpusStore.WriteVariable}=1 dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --no-build");

        foreach (var entry in entries)
        {
            var path = Path.Combine(UbcCorpusStore.Directory(CorpusRunner.Root), entry.Id + UbcCorpusStore.ArtifactExtension);
            Assert.True(File.Exists(path), $"{entry.Id} is generated and not on disk");
            Assert.True(File.ReadAllBytes(path).AsSpan().SequenceEqual(entry.Bytes), $"{entry.Id} on disk is not what the generator writes");
        }
    }

    [Fact]
    public void Two_Regenerations_Render_Byte_Identical_Manifests()
    {
        // Two independent runs of the generator and of the replay: nothing in the manifest may depend
        // on an ordering a dictionary, a clock or a thread chose.
        var first = UbcCorpusStore.Render(Rows(UbcCorpus.Entries()));
        var second = UbcCorpusStore.Render(Rows(UbcCorpus.Entries()));

        Assert.Equal(first, second);
        Assert.DoesNotContain('\r', first);
        Assert.EndsWith("}\n", first, StringComparison.Ordinal);
        Assert.All(first, character => Assert.True(character is '\n' or (>= ' ' and <= '~'), "the manifest is ASCII"));
    }

    [Fact]
    public void Every_Manifest_Row_Names_A_File_Of_The_Length_And_Hash_It_Records()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var rows = UbcCorpusStore.Read(CorpusRunner.Root);
        Assert.NotEmpty(rows);

        var mismatched = new List<string>();

        foreach (var row in rows)
        {
            var path = Path.Combine(UbcCorpusStore.Directory(CorpusRunner.Root), row.File);

            if (!File.Exists(path))
            {
                mismatched.Add($"{row.Id}: {row.File} is named by the manifest and is not on disk");
                continue;
            }

            mismatched.AddRange(IntegrityViolations(row, File.ReadAllBytes(path)));
        }

        Assert.Empty(mismatched);
    }

    [Fact]
    public void A_Mutated_Entry_Is_Detected()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        // The check the previous test makes, handed bytes that differ from what the manifest pins by
        // one inverted bit, one byte dropped and one byte appended: each must be seen.
        var row = UbcCorpusStore.Read(CorpusRunner.Root).First(static row => row.Id == "control-every-family-row");
        var bytes = File.ReadAllBytes(Path.Combine(UbcCorpusStore.Directory(CorpusRunner.Root), row.File));

        Assert.Empty(IntegrityViolations(row, bytes));

        var flipped = (byte[])bytes.Clone();
        flipped[flipped.Length / 2] ^= 0x01;
        Assert.NotEmpty(IntegrityViolations(row, flipped));

        Assert.NotEmpty(IntegrityViolations(row, bytes[..^1]));
        Assert.NotEmpty(IntegrityViolations(row, [.. bytes, 0x00]));

        // And a mutation the hash alone would catch, shown to change the answer too: the pinned answer
        // is what a mutated entry would have to fake, and it cannot.
        var observed = UbcCorpusRunner.Run(flipped, row.Configuration).Answer;
        Assert.NotEqual(row.Expected, observed);
    }

    [Fact]
    public void No_Corpus_File_Is_Unnamed_By_The_Manifest()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var named = UbcCorpusStore.Read(CorpusRunner.Root).Select(static row => row.File).ToHashSet(StringComparer.Ordinal);
        var orphans = Directory
            .GetFiles(UbcCorpusStore.Directory(CorpusRunner.Root), "*" + UbcCorpusStore.ArtifactExtension)
            .Select(Path.GetFileName)
            .Where(name => !named.Contains(name!))
            .ToArray();

        Assert.Empty(orphans);
    }

    [Fact]
    public void Every_Exactly_Pinned_Entry_Answers_What_Its_Declaration_Says()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var violations = new List<string>();

        foreach (var (row, _, observation) in Replayed.Value)
        {
            if (row.Pinning is not FixtureCorpusPinning.Exact)
            {
                continue;
            }

            var expected = row.Expected;
            var answer = observation.Answer;

            if (answer.Outcome != expected.Outcome || answer.Reason != expected.Reason)
            {
                violations.Add($"{row.Id}: expected {expected.Outcome}/{expected.Reason}, answered {answer.Outcome}/{answer.Reason}");
                continue;
            }

            if (expected.Outcome is VmOutcome.InvalidArtifact && answer.ProfileDiagnosticCode != expected.ProfileDiagnosticCode)
            {
                violations.Add($"{row.Id}: expected code {expected.ProfileDiagnosticCode}, answered {answer.ProfileDiagnosticCode}");
            }

            if (row.NamesDimension && (answer.Dimension != expected.Dimension || answer.Scope != expected.Scope))
            {
                violations.Add($"{row.Id}: expected {expected.Dimension}/{expected.Scope}, answered {answer.Dimension}/{answer.Scope}");
            }

            if (expected.Position != UbcCorpusAnswer.Unpinned && answer.Position != expected.Position)
            {
                violations.Add($"{row.Id}: expected position {expected.Position}, answered {answer.Position}");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void Every_Recorded_Entry_Still_Answers_What_Was_Recorded()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var violations = new List<string>();
        var recorded = 0;

        foreach (var (row, _, observation) in Replayed.Value)
        {
            if (row.Pinning is not FixtureCorpusPinning.Recorded)
            {
                continue;
            }

            recorded++;

            if (observation.Answer != row.Recorded)
            {
                violations.Add($"{row.Id}: recorded {row.Recorded}, answered {observation.Answer}");
            }
        }

        Assert.Empty(violations);
        Assert.True(recorded > 0, "the corpus has no recorded sweep");
    }

    [Fact]
    public void Every_Entry_Answers_Inside_The_Closed_Set_And_Only_A_Verified_Answer_Carries_A_Program()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var violations = new List<string>();

        foreach (var (row, _, observation) in Replayed.Value)
        {
            var outcome = observation.Outcome;

            if (!VerifierOutcomes.Contains(outcome.Category))
            {
                violations.Add($"{row.Id}: answered {outcome.Category}, which a verifier may not answer");
            }

            if ((outcome.State is not null) != (outcome.Category is VmOutcome.Normal))
            {
                violations.Add($"{row.Id}: answered {outcome.Category} with{(outcome.State is null ? "out" : string.Empty)} a verified state");
            }

            if (outcome.Category is VmOutcome.Normal && observation.Program is null)
            {
                violations.Add($"{row.Id}: verified, and its state is not a universal bytecode program");
            }

            if (outcome.Category is VmOutcome.InvalidArtifact && outcome.ProfileDiagnosticCode == 0)
            {
                violations.Add($"{row.Id}: refused with no diagnostic code");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void Every_Control_Verifies()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var controls = Replayed.Value.Where(static replay => replay.Row.Family == "control").ToArray();

        Assert.True(controls.Length >= 10, "the corpus has too few controls to fail in both directions");
        Assert.All(controls, replay => Assert.True(
            replay.Observation.Outcome.Category is VmOutcome.Normal,
            $"{replay.Row.Id} answered {replay.Observation.Answer}"));
    }

    [Fact]
    public void The_Corpus_Can_Fail_In_Both_Directions()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var outcomes = Replayed.Value.Select(static replay => replay.Observation.Outcome.Category).ToHashSet();

        Assert.Contains(VmOutcome.Normal, outcomes);
        Assert.Contains(VmOutcome.InvalidArtifact, outcomes);
        Assert.Contains(VmOutcome.ResourceExhaustion, outcomes);
        Assert.Contains(VmOutcome.Cancellation, outcomes);
    }

    [Fact]
    public void Every_Diagnostic_Code_But_The_Defensive_Ones_Is_The_Expected_Code_Of_A_Named_Exact_Entry()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var rows = UbcCorpusStore.Read(CorpusRunner.Root);
        var pinned = rows
            .Where(static row => row.Pinning is FixtureCorpusPinning.Exact && row.Expected.Outcome is VmOutcome.InvalidArtifact)
            .GroupBy(static row => row.Expected.ProfileDiagnosticCode)
            .ToDictionary(static group => group.Key, static group => group.Select(static row => row.Id).ToArray());

        var defensive = UbcCorpusStore.DefensiveCodes.Select(static code => code.Code).ToHashSet();
        var unreached = new List<string>();

        foreach (var code in Enum.GetValues<UbcDiagnosticCode>())
        {
            if (defensive.Contains(code))
            {
                Assert.False(pinned.ContainsKey((int)code), $"{code} is listed as defensive and an entry reaches it");
                continue;
            }

            if (!pinned.ContainsKey((int)code))
            {
                unreached.Add($"{code} ({(int)code})");
            }
        }

        Assert.Empty(unreached);

        // The manifest lists the same defensive codes the store does, so a reader of the file alone
        // sees what the gate excused.
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(UbcCorpusStore.ManifestPath(CorpusRunner.Root)));
        var listed = document.RootElement.GetProperty("defensiveCodes").EnumerateArray()
            .Select(static element => Enum.Parse<UbcDiagnosticCode>(element.GetProperty("name").GetString()!))
            .ToHashSet();

        Assert.Equal(defensive, listed);

        // Every universal code an entry pins is a member of the vocabulary; every other pinned code is
        // the family's own and lies outside the universal range.
        foreach (var code in pinned.Keys)
        {
            Assert.True(
                Enum.IsDefined((UbcDiagnosticCode)code) || code is < 3000 or > 3999,
                $"{code} is in the universal range and is not a member of the vocabulary");
        }
    }

    [Fact]
    public void Every_Operand_Shape_Effect_Form_Target_Form_And_Kind_Is_Exercised_By_A_Verifying_Entry()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var shapes = new HashSet<UbcOperandShape>();
        var familyShapes = new HashSet<UbcOperandShape>();
        var effects = new HashSet<(UbcEffectForm Form, bool Multiplied)>();
        var targets = new HashSet<string>();
        var kinds = new HashSet<UbcInstructionKind>();
        var regionPrimitive = false;

        foreach (var instruction in VerifiedInstructions())
        {
            if (instruction.Row is { } row)
            {
                shapes.Add(row.Shape);
                familyShapes.Add(row.Shape);
                effects.Add((row.Effect.Form, row.Effect.Form is UbcEffectForm.Counted && row.Effect.Multiplier > 1));
                targets.Add(!row.Target.IsCode ? "none" : row.Target.HasDistinctTakenEdge ? "code-distinct-taken" : "code-own");
                kinds.Add(row.Kind);
                regionPrimitive |= row.Primitive is { } primitive && UbcPrimitives.IsRegionAccess(primitive);
            }
            else
            {
                shapes.Add(UbcOpcodes.Row((UbcOpcode)instruction.Opcode).Shape);
            }
        }

        Assert.Equal(Enum.GetValues<UbcOperandShape>().ToHashSet(), familyShapes);
        Assert.Equal(Enum.GetValues<UbcOperandShape>().ToHashSet(), shapes);
        Assert.Contains((UbcEffectForm.Listed, false), effects);
        Assert.Contains((UbcEffectForm.Counted, false), effects);
        Assert.Contains((UbcEffectForm.Counted, true), effects);
        Assert.Equal(new HashSet<string> { "none", "code-own", "code-distinct-taken" }, targets);
        Assert.Equal(Enum.GetValues<UbcInstructionKind>().ToHashSet(), kinds);
        Assert.True(regionPrimitive, "no verifying entry uses a region primitive");
    }

    [Fact]
    public void Every_Common_Opcode_And_Every_Family_Row_Of_Both_Tables_Is_Used_By_A_Verifying_Entry()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var common = new HashSet<UbcOpcode>();
        var family = new HashSet<byte>();

        foreach (var instruction in VerifiedInstructions())
        {
            if (instruction.Row is null)
            {
                common.Add((UbcOpcode)instruction.Opcode);
            }
            else
            {
                family.Add(instruction.Opcode);
            }
        }

        Assert.Equal(UbcOpcodes.All.Select(static row => row.Opcode).ToHashSet(), common);
        Assert.Equal(UbcCorpusFamily.WideRows().Select(static row => row.Opcode).ToHashSet(), family);
    }

    [Fact]
    public void The_Controls_Reach_Regions_Landings_Jump_Tables_Calls_Entries_And_Positions()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var programs = Replayed.Value
            .Where(static replay => replay.Row.Family == "control")
            .Select(static replay => replay.Observation.Program!)
            .ToArray();

        var regionKinds = programs.SelectMany(static program => program.Units).SelectMany(static unit => unit.Regions)
            .Select(static region => region.Kind).ToHashSet();

        Assert.Equal(new HashSet<byte> { UbcCorpusFamily.CatchKind, UbcCorpusFamily.FinallyKind }, regionKinds);
        Assert.Contains(programs, static program => program.Units.Any(static unit => unit.Regions.Any(static region => region.WordEntryHeight > 0)));
        Assert.Contains(programs, static program => program.Units.Any(static unit => unit.Regions.Any(static region => region.ValueEntryHeight > 0)));
        Assert.Contains(programs, static program => program.Units.Any(static unit =>
            (unit.Unit.Flags & UbcUnitFlags.Suspendable) != 0 && unit.Unit.Landings.Length >= 2));
        Assert.Contains(programs, static program => program.JumpTableTargets.Length >= 3);
        Assert.Contains(programs, static program => program.Artifact.Positions.Length >= 4 && program.Artifact.Entries.Length >= 3);
        Assert.Contains(programs, static program => program.Units.Any(static unit =>
            unit.Instructions.Any(static instruction => instruction.Row is null && instruction.Opcode == (byte)UbcOpcode.Call)));
        Assert.Contains(programs, static program => program.Table.TableVersion == UbcCorpusFamily.WideTableVersion);
        Assert.Contains(programs, static program => program.FamilySlot == UbcFormat.MaxFamilySlot);
    }

    [Fact]
    public void The_Local_Control_Places_Every_Local_In_Its_Plane()
    {
        // What the walk proved about each local instruction, read from the verified program: a local's
        // index within its plane skips the other plane and the run of length zero.
        var observation = UbcCorpusRunner.Run(UbcCorpus.LocalsOfBothPlanes().Bytes(), UbcCorpusConfiguration.Default);
        var unit = Assert.IsType<UbcVerifiedProgram>(observation.Outcome.State).Units[0];

        var locals = unit.Instructions
            .Where(static instruction => instruction.Row is null && instruction.Opcode is (byte)UbcOpcode.LocalGet or (byte)UbcOpcode.LocalSet or (byte)UbcOpcode.LocalTee)
            .Select(static instruction => (Local: (int)instruction.Operand, instruction.Plane, instruction.Index))
            .Distinct()
            .OrderBy(static local => local.Local)
            .ToArray();

        (int Local, UbcPlane Plane, int Index)[] expected =
        [
            (0, UbcPlane.Value, 0),
            (1, UbcPlane.Word, 0),
            (2, UbcPlane.Value, 1),
            (3, UbcPlane.Value, 2),
            (4, UbcPlane.Word, 1),
            (5, UbcPlane.Word, 2),
            (6, UbcPlane.Word, 3),
        ];

        Assert.Equal(expected, locals);
        Assert.Equal(4, unit.WordLocals);
        Assert.Equal(3, unit.ValueLocals);
        Assert.Equal(1, unit.ParameterWords);
        Assert.Equal(1, unit.ParameterValues);
    }

    [Fact]
    public void The_Corpus_Covers_Every_Group_It_Is_Built_From()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var groups = UbcCorpusStore.Read(CorpusRunner.Root).Select(static row => row.Family).ToHashSet(StringComparer.Ordinal);

        foreach (var required in new[]
                 {
                     "control", "prefix", "header", "framing", "families", "units", "walk", "regions",
                     "tables-entries-positions", "form", "hook", "ceiling", "truncation-sweep", "corruption-sweep",
                 })
        {
            Assert.Contains(required, groups);
        }
    }

    [Fact]
    public void Every_Ceiling_And_Allowance_A_Verification_Runs_Under_Has_An_Exhaustion_Entry()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        var exhausted = UbcCorpusStore.Read(CorpusRunner.Root)
            .Where(static row => row.Pinning is FixtureCorpusPinning.Exact && row.Expected.Outcome is VmOutcome.ResourceExhaustion)
            .Select(static row => (row.Expected.Dimension, row.Expected.Scope))
            .ToHashSet();

        foreach (var dimension in new[]
                 {
                     VmBudgetDimension.DeclaredCount, VmBudgetDimension.SectionCount, VmBudgetDimension.StructuralDepth,
                     VmBudgetDimension.ArtifactBytes, VmBudgetDimension.AllocatedBytes, VmBudgetDimension.VerifierWork,
                 })
        {
            Assert.Contains((dimension, VmBudgetScope.Artifact), exhausted);
        }

        Assert.Contains(
            UbcCorpusStore.Read(CorpusRunner.Root),
            static row => row.Pinning is FixtureCorpusPinning.Exact && row.Expected.Outcome is VmOutcome.Cancellation && row.Configuration.Cancelled);
    }

    [Fact]
    public void No_Verification_Works_Longer_Between_Two_Polls_Than_The_Family_Declares()
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        // Obligation CO-4: the work between two polls is bounded by the uncharged-work bound the
        // descriptor declares, for every entry, failing ones included.
        var bound = UbcCorpusFamily.Declaration().MaxUnchargedWork;
        var violations = Replayed.Value
            .Where(replay => replay.Observation.Meter.MostWorkBetweenPolls > bound)
            .Select(replay => $"{replay.Row.Id}: {replay.Observation.Meter.MostWorkBetweenPolls} work units between two polls")
            .ToArray();

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(2u)]
    [InlineData(3u)]
    [InlineData(8u)]
    [InlineData(65u)]
    [InlineData(1000u)]
    public void Every_Declared_Bound_Holds_The_Work_Between_Two_Polls_And_Changes_No_Answer(uint bound)
    {
        if (UbcCorpusStore.WriteRequested)
        {
            return;
        }

        // The same obligation under bounds far tighter and looser than the family's own: a read or a
        // walk step that charged more than the bound in one piece would show here first. The answer
        // must not move with the bound either - the bound decides only how often a verification polls.
        var violations = new List<string>();
        var profiles = new Dictionary<UbcCorpusHookMode, VmProfileDescriptor>();

        foreach (var (row, bytes, observation) in Replayed.Value)
        {
            var mode = row.Configuration.Hook;

            if (!profiles.TryGetValue(mode, out var profile))
            {
                profile = UbcCorpusFamily.Descriptor(mode, bound);
                profiles.Add(mode, profile);
            }

            var bounded = UbcCorpusRunner.Run(bytes, row.Configuration, profile);

            if (bounded.Meter.MostWorkBetweenPolls > bound)
            {
                violations.Add($"{row.Id}: {bounded.Meter.MostWorkBetweenPolls} work units between two polls under a bound of {bound}");
            }

            if (!row.Configuration.Cancelled && bounded.Answer != observation.Answer)
            {
                violations.Add($"{row.Id}: answered {bounded.Answer} under a bound of {bound} and {observation.Answer} under the family's");
            }
        }

        Assert.Empty(violations);
    }

    // ---- helpers ------------------------------------------------------------------------------------

    private static IEnumerable<UbcInstruction> VerifiedInstructions() =>
        Replayed.Value
            .Where(static replay => replay.Observation.Program is not null)
            .SelectMany(static replay => replay.Observation.Program!.Units)
            .SelectMany(static unit => unit.Instructions);

    private static IEnumerable<string> IntegrityViolations(UbcCorpusRecord row, byte[] bytes)
    {
        if (bytes.Length != row.ByteLength)
        {
            yield return $"{row.Id}: {bytes.Length} bytes, {row.ByteLength} recorded";
        }

        var hash = UbcCorpusStore.Hash(bytes);

        if (!string.Equals(hash, row.Sha256, StringComparison.Ordinal))
        {
            yield return $"{row.Id}: hashes to {hash}, the manifest records {row.Sha256}";
        }
    }

    private static IReadOnlyList<UbcCorpusRecord> Rows(IReadOnlyList<UbcCorpusEntry> entries) =>
        entries
            .Select(static entry => UbcCorpusStore.RowFor(entry, UbcCorpusRunner.Run(entry.Bytes, entry.Configuration).Answer))
            .ToArray();

    private static IReadOnlyList<(UbcCorpusRecord Row, byte[] Bytes, UbcCorpusObservation Observation)> Replay()
    {
        var directory = UbcCorpusStore.Directory(CorpusRunner.Root);

        return UbcCorpusStore.Read(CorpusRunner.Root)
            .Select(row =>
            {
                var bytes = File.ReadAllBytes(Path.Combine(directory, row.File));
                return (row, bytes, UbcCorpusRunner.Run(bytes, row.Configuration));
            })
            .ToArray();
    }
}
