using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The retained malformed-input corpus, and the properties every entry in it must have.
/// </summary>
/// <remarks>
/// <para>
/// The corpus is a file tree plus a manifest, not a theory over a generator. Update rule 4 forbids
/// promoting a subset generated to prove a contract into evidence for a corpus gate, and the
/// difference the rule is protecting is retention: a case generated inside a test exists only while
/// the test runs, so nothing can cite it, diff it, or notice when its answer changes.
/// </para>
/// <para>
/// Every assertion here is made against bytes read back off disk. Verifying the in-memory
/// declaration instead would test the declaration.
/// </para>
/// </remarks>
public sealed class MalformedCorpusTests
{
    /// <summary>The outcomes the load stage may produce. Anything outside this set is a defect.</summary>
    private static readonly VmOutcome[] LoadStageOutcomes =
    [
        VmOutcome.Normal,
        VmOutcome.UnsupportedProfile,
        VmOutcome.InvalidArtifact,
        VmOutcome.ResourceExhaustion,
        VmOutcome.Cancellation,
        VmOutcome.InvalidState,
    ];

    [Fact]
    public void The_Corpus_On_Disk_Is_What_The_Declaration_Describes()
    {
        // In write mode this rewrites the corpus and its manifest; otherwise it asserts them. The
        // generator and the gate are the same code deliberately: a generator only the author runs
        // and a checker only CI runs are two implementations that drift.
        var seeded = FixtureCorpus.Entries();
        var existing = FixtureCorpusStore.Read(CorpusRunner.Root);

        var rows = Rows(seeded, existing);

        if (FixtureCorpusStore.WriteRequested)
        {
            FixtureCorpusStore.Write(CorpusRunner.Root, seeded, rows, "VM-2", VmCoreContract.Version);
            return;
        }

        var desired = FixtureCorpusStore.Render(rows, "VM-2", VmCoreContract.Version);
        var actual = File.ReadAllText(FixtureCorpusStore.ManifestPath(CorpusRunner.Root));

        Assert.True(
            string.Equals(desired, actual, StringComparison.Ordinal),
            $"{FixtureCorpusStore.RelativeDirectory}/{FixtureCorpusStore.ManifestFileName} is not what " +
            $"the corpus would write.\n  Run: {FixtureCorpusStore.WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release");
    }

    [Fact]
    public void Every_Manifest_Row_Names_A_File_That_Hashes_To_What_It_Records()
    {
        // The hash is what makes the bytes citable. Without it the manifest would describe a file
        // that anything could have edited, and a minimized fuzz regression - which has no
        // declaration to be regenerated from - would rest on nothing at all.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var directory = FixtureCorpusStore.Directory(CorpusRunner.Root);
        var rows = FixtureCorpusStore.Read(CorpusRunner.Root);

        Assert.NotEmpty(rows);

        var mismatched = new List<string>();

        foreach (var row in rows)
        {
            var path = Path.Combine(directory, row.File);

            if (!File.Exists(path))
            {
                mismatched.Add($"{row.Id}: {row.File} is named by the manifest and is not on disk");
                continue;
            }

            var bytes = File.ReadAllBytes(path);

            if (bytes.Length != row.ByteLength)
            {
                mismatched.Add($"{row.Id}: {bytes.Length} bytes on disk, {row.ByteLength} recorded");
                continue;
            }

            var hash = FixtureCorpusStore.Hash(bytes);

            if (!string.Equals(hash, row.Sha256, StringComparison.Ordinal))
            {
                mismatched.Add($"{row.Id}: hashes to {hash}, manifest records {row.Sha256}");
            }
        }

        Assert.Empty(mismatched);
    }

    [Fact]
    public void No_Corpus_File_Is_Unnamed_By_The_Manifest()
    {
        // The other direction. A file nothing names is a case nothing runs, which is how a
        // regression gets retained and then quietly stops being tested.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var directory = FixtureCorpusStore.Directory(CorpusRunner.Root);
        var named = FixtureCorpusStore.Read(CorpusRunner.Root)
            .Select(static row => row.File)
            .ToHashSet(StringComparer.Ordinal);

        var orphans = Directory
            .GetFiles(directory, "*" + FixtureCorpusStore.ArtifactExtension)
            .Select(Path.GetFileName)
            .Where(name => !named.Contains(name!))
            .ToArray();

        Assert.Empty(orphans);
    }

    [Fact]
    public void Every_Corpus_Artifact_Answers_Inside_The_Closed_Set_And_Produces_No_Handle_Unless_It_Verified()
    {
        // The property that holds for every row whatever its pinning, and the one a fuzz-found
        // regression is retained for: the load stage answered with a category it is allowed to
        // answer with, it did not throw, it did not hang, and success and a handle agree.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var violations = new List<string>();

        foreach (var row in FixtureCorpusStore.Read(CorpusRunner.Root))
        {
            var observation = Observe(row);

            if (!LoadStageOutcomes.Contains(observation.Outcome))
            {
                violations.Add($"{row.Id}: answered {observation.Outcome}, which the load stage may not produce");
            }

            if (observation.ProducedHandle != (observation.Outcome is VmOutcome.Normal))
            {
                violations.Add(
                    $"{row.Id}: answered {observation.Outcome} and " +
                    (observation.ProducedHandle ? "produced a handle" : "produced no handle"));
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void Every_Exactly_Pinned_Artifact_Answers_What_Its_Declaration_Says()
    {
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var violations = new List<string>();

        foreach (var row in FixtureCorpusStore.Read(CorpusRunner.Root))
        {
            if (row.Pinning is not FixtureCorpusPinning.Exact)
            {
                continue;
            }

            var observation = Observe(row);

            if (observation.Outcome != row.ExpectedOutcome)
            {
                violations.Add($"{row.Id}: expected {row.ExpectedOutcome}, answered {observation.Outcome}");
                continue;
            }

            if (row.ExpectedReason is not VmReason.None && observation.Reason != row.ExpectedReason)
            {
                violations.Add($"{row.Id}: expected reason {row.ExpectedReason}, answered {observation.Reason}");
            }

            if (row.ExpectedOutcome is VmOutcome.InvalidArtifact &&
                observation.ProfileDiagnosticCode != row.ExpectedProfileDiagnosticCode)
            {
                violations.Add(
                    $"{row.Id}: expected profile diagnostic code {row.ExpectedProfileDiagnosticCode}, " +
                    $"answered {observation.ProfileDiagnosticCode}");
            }

            if (row.NamesDimension &&
                (observation.Dimension != row.ExpectedDimension || observation.Scope != row.ExpectedScope))
            {
                violations.Add(
                    $"{row.Id}: expected {row.ExpectedDimension}/{row.ExpectedScope}, " +
                    $"answered {observation.Dimension}/{observation.Scope}");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void Every_Recorded_Artifact_Still_Answers_What_Was_Recorded()
    {
        // A weaker claim than the pinned one, and stated as such: it cannot show the answer is
        // right, and it does show the answer has not moved without somebody accepting a diff.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var violations = new List<string>();

        foreach (var row in FixtureCorpusStore.Read(CorpusRunner.Root))
        {
            if (row.Pinning is not FixtureCorpusPinning.Recorded)
            {
                continue;
            }

            var observation = Observe(row);

            if (observation.Outcome != row.RecordedOutcome ||
                observation.Reason != row.RecordedReason ||
                observation.ProfileDiagnosticCode != row.RecordedProfileDiagnosticCode ||
                observation.Dimension != row.RecordedDimension ||
                observation.Scope != row.RecordedScope)
            {
                violations.Add(
                    $"{row.Id}: recorded {row.RecordedOutcome}/{row.RecordedReason}/" +
                    $"{row.RecordedProfileDiagnosticCode}, answered {observation.Outcome}/" +
                    $"{observation.Reason}/{observation.ProfileDiagnosticCode}");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void No_Corpus_Artifact_Allocates_Before_The_Policy_That_Bounds_It_Exists()
    {
        // The materialization ordering, asserted for every artifact including every failing one.
        // Two claims, and the second is the one an implementation can get wrong: the frozen policy
        // reaches the verifier before it reads or allocates anything, and no allocation is reserved
        // before a byte has been read - a verifier that sized a buffer from a count it had not read
        // yet would reserve first.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var violations = new List<string>();

        foreach (var row in FixtureCorpusStore.Read(CorpusRunner.Root))
        {
            var observation = Observe(row);
            var recorder = observation.Recorder;

            if (!recorder.PolicyPrecededEveryRead)
            {
                violations.Add($"{row.Id}: a payload byte was read or an allocation reserved before the policy was frozen");
            }

            if (!recorder.NoReservationPrecededTheFirstRead)
            {
                violations.Add($"{row.Id}: an allocation was reserved before any payload byte was read");
            }

            if (recorder.PolicyObserved &&
                recorder.ReservedBytes > recorder.ObservedPolicy[VmBudgetDimension.AllocatedBytes])
            {
                violations.Add(
                    $"{row.Id}: reserved {recorder.ReservedBytes} bytes against an allocation ceiling of " +
                    $"{recorder.ObservedPolicy[VmBudgetDimension.AllocatedBytes]}");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void The_Effective_Policy_The_Verifier_Receives_Is_The_Intersection_And_Never_Above_The_Host_Ceiling()
    {
        // Computed here from the three layers independently of the runtime, so the assertion is an
        // oracle rather than an echo. Every dimension of what the verifier was handed must equal the
        // minimum of the runtime ceiling, the profile maximum and the artifact request, and no
        // dimension of it may exceed the host's own ceiling.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var violations = new List<string>();

        foreach (var row in FixtureCorpusStore.Read(CorpusRunner.Root))
        {
            var observation = Observe(row);

            if (!observation.Recorder.PolicyObserved)
            {
                continue;
            }

            var request = CorpusRunner.RequestedLimits(row.ArtifactBytesRequest);
            var observed = observation.Recorder.ObservedPolicy;

            foreach (var dimension in VmBudgetDimensions.All)
            {
                var profileMaximum = FixtureDescriptorFactory.Maxima()[dimension];

                // The runtime adopts the profile's declared default for every dimension but the
                // live-runtime count, which is meaningful only against a parent and resolves to TOP
                // without one; either way a profile hard maximum tightens what the host asked for.
                var host = dimension is VmBudgetDimension.LiveRuntimes
                    ? Math.Min(ulong.MaxValue, profileMaximum)
                    : Math.Min(FixtureDescriptorFactory.Defaults()[dimension], profileMaximum);

                var asked = request.IsEmpty ? ulong.MaxValue : request[dimension];
                var expected = Math.Min(Math.Min(host, profileMaximum), asked);

                if (observed[dimension] != expected)
                {
                    violations.Add(
                        $"{row.Id}: {dimension} materialized to {observed[dimension]}, the intersection is {expected}");
                }

                if (observed[dimension] > host)
                {
                    violations.Add($"{row.Id}: {dimension} materialized above the host ceiling");
                }
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void The_Corpus_Covers_Every_Family_The_Gate_Names()
    {
        // The gate names six kinds of hostile artifact. A corpus that had quietly lost one would
        // still pass every assertion above, because every assertion above is about the entries that
        // are there.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var families = FixtureCorpusStore.Read(CorpusRunner.Root)
            .Select(static row => row.Family)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var required in new[]
                 {
                     "control", "prefix", "magic", "format-version", "framing", "constants",
                     "code", "ceiling", "truncation-sweep", "corruption-sweep",
                 })
        {
            Assert.Contains(required, families);
        }
    }

    [Fact]
    public void The_Corpus_Can_Fail_In_Both_Directions()
    {
        // A corpus in which everything fails passes just as happily under a verifier that rejects
        // whatever it is handed. This is the assertion that rules that verifier out.
        if (FixtureCorpusStore.WriteRequested)
        {
            // The corpus is being regenerated, so reading it back would read what this run just
            // wrote. The gate runs in the mode a release runs.
            return;
        }

        var outcomes = FixtureCorpusStore.Read(CorpusRunner.Root)
            .Select(row => Observe(row).Outcome)
            .ToArray();

        Assert.Contains(VmOutcome.Normal, outcomes);
        Assert.Contains(VmOutcome.InvalidArtifact, outcomes);
        Assert.Contains(VmOutcome.ResourceExhaustion, outcomes);
    }

    private static CorpusObservation Observe(FixtureCorpusRecord row)
    {
        var path = Path.Combine(FixtureCorpusStore.Directory(CorpusRunner.Root), row.File);

        return CorpusRunner.Run(
            File.ReadAllBytes(path), row.DescriptorFormatVersion, row.ArtifactBytesRequest);
    }

    /// <summary>
    /// The rows the manifest should hold: every seeded entry with a fresh observation, then every
    /// minimized regression already retained, by identifier.
    /// </summary>
    private static IReadOnlyList<FixtureCorpusRecord> Rows(
        IReadOnlyList<FixtureCorpusEntry> seeded,
        IReadOnlyList<FixtureCorpusRecord> existing)
    {
        var rows = new List<FixtureCorpusRecord>(seeded.Count + existing.Count);

        foreach (var entry in seeded)
        {
            var observation = CorpusRunner.Run(
                entry.Bytes, entry.DescriptorFormatVersion, entry.ArtifactBytesRequest);

            rows.Add(FixtureCorpusStore.RowFor(entry).WithObservation(
                observation.Outcome, observation.Reason, observation.ProfileDiagnosticCode,
                observation.Dimension, observation.Scope));
        }

        var minimized = existing
            .Where(static row => row.Provenance is FixtureCorpusProvenance.Minimized)
            .OrderBy(static row => row.Id, StringComparer.Ordinal);

        foreach (var row in minimized)
        {
            var path = Path.Combine(FixtureCorpusStore.Directory(CorpusRunner.Root), row.File);

            if (!File.Exists(path))
            {
                continue;
            }

            var bytes = File.ReadAllBytes(path);
            var observation = CorpusRunner.Run(bytes, row.DescriptorFormatVersion, row.ArtifactBytesRequest);

            rows.Add(row.WithObservation(
                observation.Outcome, observation.Reason, observation.ProfileDiagnosticCode,
                observation.Dimension, observation.Scope));
        }

        return rows;
    }
}
