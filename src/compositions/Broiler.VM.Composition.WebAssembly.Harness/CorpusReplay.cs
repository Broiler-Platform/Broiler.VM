using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>One row of the retained corpus manifest, as this root reads it back.</summary>
internal sealed record ManifestRow(
    string Name,
    string Sha256,
    string Family,
    string Outcome,
    string Reason,
    int DiagnosticCode,
    string Dimension,
    string Scope,
    string Provenance,
    string Invariant);

/// <summary>What one replayed entry actually did, and which stage answered it.</summary>
/// <remarks>
/// The stage is <c>translation</c> where the translator refused the module and the core was never
/// asked, <c>core</c> where the module translated and the core answered over its artifact, and
/// <c>-</c> where there was no module to hand either.
/// </remarks>
internal sealed record ReplayObservation(
    string Name,
    string Outcome,
    string Reason,
    int DiagnosticCode,
    string Dimension,
    string Scope,
    string HashStatus,
    string Stage);

/// <summary>
/// The retained-corpus writer and the replay that holds it: read the bytes, re-hash them, verify
/// them, and compare the whole observed answer against what the manifest recorded.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE HASH IS RE-COMPUTED AND NOT TRUSTED.</b> A corpus whose bytes changed without its
/// manifest changing is a failure and not a quiet drift, and re-hashing on every replay is the only
/// reason a hash beside an entry is worth writing down. The integrity script beside this root
/// exercises the other direction: it moves a byte and requires the replay to notice.
/// </para>
/// <para>
/// <b>The manifest is written by one half and parsed by the other, by hand, on purpose.</b> A
/// shared serializer that round-tripped itself would agree with itself whatever either half meant,
/// and the disagreement this pair exists to catch is exactly the one a round-trip cannot see.
/// </para>
/// <para>
/// <b>What a replay compares is five fields and not three.</b> The outcome, the reason and the
/// diagnostic code are the triple; the dimension and the scope are the two the JavaScript profile's
/// corpus added and rule N11 fixed, because a resource exhaustion carries NO diagnostic code -
/// <c>ResourceExhaustion/CeilingReached/0</c> is the same answer for a declared-count ceiling and a
/// structural-depth one, and a corpus that recorded the category alone could not tell a verifier
/// that refuses the right module for the wrong reason from one that does not.
/// </para>
/// <para>
/// <b>Every answer is the translator's or the core's over the translator's artifact, and the replay
/// records which.</b> The translator decodes and validates a module with the profile's own decoder
/// and validator before it lowers anything, so every check that refused a module before the
/// universal bytecode still refuses it there, in the same fields; no check moved into the universal
/// bytecode's walk, and the class of rows the predeclared parity rule admits for one that did -
/// class (r) - is empty by construction. A module the translator admitted and the core then refused
/// is therefore a defect of the translator or of the family's verifier hook, and it fails every
/// invariant rather than being recorded as an answer.
/// </para>
/// <para>
/// <b>What is NOT compared.</b> The source position is not: a row that pinned four byte offsets
/// would turn every entry into a claim about the encoder's layout, and the first change to the
/// encoder would fail two hundred rows for a reason none of them is about. Nothing here reaches
/// execution either - every row stops at verification, and the interpreter's answers are scored by
/// the differential lane, where the expected values come from somewhere other than the interpreter.
/// </para>
/// </remarks>
internal static class CorpusReplay
{
    // =============================================================================================
    // Writing
    // =============================================================================================

    /// <summary>
    /// Writes every entry's bytes and the manifest that describes them, and answers a process exit
    /// code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A derived row that the profile contradicts STOPS THE WRITE.</b> The declaration wins:
    /// where a person wrote the triple down before asking, a disagreement is a finding and the one
    /// thing that must not happen is the writer quietly recording the profile's answer over the
    /// person's. Nothing is written at all in that case - not the manifest and not the bytes - so a
    /// half-regenerated corpus cannot exist on disk.
    /// </para>
    /// <para>
    /// <b>A violated invariant stops it too, derived or recorded.</b> The invariant is the only
    /// thing holding a recorded row, so a recorded row whose invariant fails is a row with nothing
    /// behind it.
    /// </para>
    /// </remarks>
    internal static int Write(VmRuntime runtime, string directory)
    {
        var entries = CorpusStore.Entries();
        var rows = new List<string>();
        var complaints = new List<string>();

        foreach (var entry in entries)
        {
            var observed = Observe(runtime, entry.Bytes, entry.Name, "match");
            var invariant = CheckInvariant(entry.Invariant, observed);

            if (invariant is not null)
            {
                complaints.Add($"{entry.Name}: {invariant}");
                continue;
            }

            var derived = entry.Outcome is not null;

            if (derived)
            {
                // A declaration names an answer and not the stage that gives it, so the stage the
                // observation recorded is carried over; the invariant above has already refused the
                // one stage that may not answer a refusal.
                var declared = new ReplayObservation(
                    entry.Name,
                    entry.Outcome!.Value.ToString(),
                    entry.Reason!.Value.ToString(),
                    (int)entry.Code,
                    entry.Dimension?.ToString() ?? "-",
                    entry.Scope?.ToString() ?? "-",
                    "match",
                    observed.Stage);

                if (declared != observed)
                {
                    complaints.Add(
                        $"{entry.Name}: declared {Format(declared)}, profile answered {Format(observed)}");

                    continue;
                }
            }

            rows.Add(string.Join(
                '|',
                entry.Name,
                Convert.ToHexStringLower(SHA256.HashData(entry.Bytes)),
                entry.Family,
                observed.Outcome,
                observed.Reason,
                observed.DiagnosticCode.ToString(CultureInfo.InvariantCulture),
                observed.Dimension,
                observed.Scope,
                derived ? "derived" : "recorded",
                entry.Invariant));
        }

        if (complaints.Count > 0)
        {
            Console.WriteLine(
                $"broiler-wasm-harness: {complaints.Count} entries would have been written with an " +
                "answer nobody declared, and nothing was written");

            foreach (var complaint in complaints)
            {
                Console.WriteLine($"FAIL {complaint}");
            }

            return 1;
        }

        Directory.CreateDirectory(directory);

        foreach (var entry in entries)
        {
            File.WriteAllBytes(
                Path.Combine(directory, entry.Name + CorpusStore.Extension), entry.Bytes);
        }

        var manifest = new List<string>
        {
            "# broiler.webassembly retained corpus, feature manifest " +
                WebAssemblyProfile.SliceManifest + ", binary version 1",
            "# registry-revision: " +
                CorpusStore.RegistryRevision.ToString(CultureInfo.InvariantCulture),
            "# registry-revision is the revision of " +
                "src/Broiler.VM.Profile.WebAssembly/docs/diagnostics/registry.txt every code below " +
                "is read against",
            "# name|sha256|family|outcome|reason|diagnostic|dimension|scope|provenance|invariant",
            "# diagnostic is the profile diagnostic code, and 0 where the answer carries none",
            "# dimension and scope are the budget dimension a resource exhaustion named and the " +
                "scope that refused, or - where the row is not an exhaustion",
            "# provenance derived: a person wrote the answer down before the profile was asked, " +
                "and a disagreement stops the write",
            "# provenance recorded: the answer came from the profile when this file was written. " +
                "It detects a change between regenerations and proves no correctness; what holds " +
                "the row is its invariant",
        };

        manifest.AddRange(rows);

        // LF and not the platform's separator. The corpus is pinned by hash and this file is read
        // back by two implementations; a line ending that depends on where the writer ran is a
        // difference nobody chose.
        File.WriteAllText(
            Path.Combine(directory, CorpusStore.ManifestFileName),
            string.Join('\n', manifest) + "\n");

        var derivedCount = rows.Count(row => row.Contains("|derived|", StringComparison.Ordinal));

        Console.WriteLine(
            $"broiler-wasm-harness: wrote {rows.Count} entries to {directory} " +
            $"({derivedCount} derived, {rows.Count - derivedCount} recorded)");

        return 0;
    }

    // =============================================================================================
    // Reading and replaying
    // =============================================================================================

    /// <summary>Parses the manifest the writer wrote.</summary>
    internal static ManifestRow[] ReadManifest(string path)
    {
        var rows = new List<ManifestRow>();

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != 10)
            {
                throw new InvalidOperationException(
                    $"corpus manifest row has {parts.Length} columns: {line}");
            }

            rows.Add(new ManifestRow(
                parts[0],
                parts[1],
                parts[2],
                parts[3],
                parts[4],
                int.Parse(parts[5], CultureInfo.InvariantCulture),
                parts[6],
                parts[7],
                parts[8],
                parts[9]));
        }

        return [.. rows];
    }

    /// <summary>Replays every row and answers what each one did.</summary>
    internal static ReplayObservation[] Replay(
        VmRuntime runtime, string directory, ManifestRow[] rows)
    {
        var observations = new ReplayObservation[rows.Length];

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            var path = Path.Combine(directory, row.Name + CorpusStore.Extension);

            if (!File.Exists(path))
            {
                observations[index] = new ReplayObservation(
                    row.Name, "Missing", "NoSuchFile", 0, "-", "-", "MISSING", "-");

                continue;
            }

            var bytes = File.ReadAllBytes(path);
            var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));

            observations[index] = Observe(
                runtime,
                bytes,
                row.Name,
                string.Equals(hash, row.Sha256, StringComparison.Ordinal) ? "match" : "MISMATCH");
        }

        return observations;
    }

    private static ReplayObservation Observe(
        VmRuntime runtime, byte[] bytes, string name, string hashStatus)
    {
        var verified = ModuleVerification.Verify(
            runtime, bytes, "composition-wasm-harness://corpus", "corpus " + name);

        // A DIAGNOSTIC CODE IS READ ONLY WHERE THE OUTCOME CARRIES ONE. An exhaustion and a normal
        // completion both leave the field at whatever the diagnostics record defaults to, and
        // formatting it unconditionally would put a number beside two hundred rows that answered
        // with none - a value that looks like an observation and is not one.
        var code = verified.Outcome is VmOutcome.InvalidArtifact ? verified.Code : 0;

        var exhausted = verified.Outcome is VmOutcome.ResourceExhaustion;

        return new ReplayObservation(
            name,
            verified.Outcome.ToString(),
            verified.Reason.ToString(),
            code,
            exhausted ? verified.Dimension.ToString() : "-",
            exhausted ? verified.Scope.ToString() : "-",
            hashStatus,
            verified.Stage is ModuleStage.Core ? CoreStage : TranslationStage);
    }

    /// <summary>The stage a module the translator refused was answered at.</summary>
    internal const string TranslationStage = "translation";

    /// <summary>The stage a translated module was answered at: the core, over its artifact.</summary>
    internal const string CoreStage = "core";

    /// <summary>Whether an observation is the ANSWER its manifest row recorded.</summary>
    /// <remarks>
    /// The digest is deliberately not one of the comparisons. It is checked beside this, and folding
    /// it in here would make every hash mismatch also report an answer mismatch - so a mutated entry
    /// whose answer did not move would print "expected X, observed X" beside its real complaint, and
    /// the integrity script could no longer tell which half of the replay had noticed. That
    /// distinction is the strongest argument for recording a digest at all, and a report that hid it
    /// would be arguing against its own column.
    /// <para>
    /// The stage is not one of the comparisons either. A manifest row records the answer a module
    /// gets, which did not change when the translator started giving it; which stage answered is held
    /// by the invariant, where a refusal the core gave a translated module fails whatever the row says.
    /// </para>
    /// </remarks>
    internal static bool Agrees(ManifestRow expected, ReplayObservation observed) =>
        string.Equals(expected.Outcome, observed.Outcome, StringComparison.Ordinal) &&
        string.Equals(expected.Reason, observed.Reason, StringComparison.Ordinal) &&
        expected.DiagnosticCode == observed.DiagnosticCode &&
        string.Equals(expected.Dimension, observed.Dimension, StringComparison.Ordinal) &&
        string.Equals(expected.Scope, observed.Scope, StringComparison.Ordinal);

    /// <summary>
    /// Whether an observation satisfies the hand-written invariant its family carries, and what
    /// went wrong where it does not.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the half of a row that survives a regeneration.</b> Regenerating the corpus
    /// rewrites every recorded triple, and a recorded triple that was rewritten against a broken
    /// build records the break as the expectation. The invariant is written by a person from the
    /// format, is never regenerated, and is checked on every replay as well as on every write - so
    /// a regeneration that moved two hundred rows onto answers the format forbids fails at the
    /// moment it is attempted.
    /// </para>
    /// <para>
    /// <b>A refusal the core gave a translated module fails every invariant, before any other clause is
    /// read.</b> The translator refuses everything the profile's decoder and validator refuse, so the
    /// core's walk and the family's hook have nothing left to refuse in a module that translated: class
    /// (r) is empty by construction, and a universal code - 3001 to 3009 among them, which are also
    /// this profile's trap codes, and every one of them in the validation band by number - must never
    /// satisfy a band or an enumeration clause below by coincidence.
    /// </para>
    /// <para>
    /// <b>The decode-and-validate split is observable because this profile numbered it.</b>
    /// Diagnostic codes below 2700 are the decoder's and codes from 2700 up are the validator's, so
    /// "this module was refused before validation began" is a claim an outside caller can check
    /// rather than a claim about the profile's internal call order.
    /// </para>
    /// </remarks>
    internal static string? CheckInvariant(string invariant, ReplayObservation observed)
    {
        var refused = string.Equals(observed.Outcome, "InvalidArtifact", StringComparison.Ordinal);
        var accepted = string.Equals(observed.Outcome, "Normal", StringComparison.Ordinal);
        var exhausted = string.Equals(
            observed.Outcome, "ResourceExhaustion", StringComparison.Ordinal);

        if (string.Equals(observed.Stage, CoreStage, StringComparison.Ordinal) && !accepted)
        {
            return $"the core answered {Format(observed)} for a module the translator admitted; " +
                "class (r) is empty by construction, so this is a defect of the translator or of " +
                "the family's verifier hook and not an answer";
        }

        switch (invariant)
        {
            case CorpusStore.Accepts:
                return accepted ? null : $"must verify, and answered {Format(observed)}";

            case CorpusStore.RefusesDecoding:
                return refused && observed.DiagnosticCode is > 0 and < 2700
                    ? null
                    : $"must be refused by the decoder, and answered {Format(observed)}";

            case CorpusStore.RefusesValidation:
                return refused && observed.DiagnosticCode >= 2700
                    ? null
                    : $"must be refused by the validator, and answered {Format(observed)}";

            case CorpusStore.Exhausts:
                return exhausted && !string.Equals(observed.Dimension, "-", StringComparison.Ordinal)
                    ? null
                    : $"must exhaust a named dimension, and answered {Format(observed)}";

            case CorpusStore.Refuses:
                return refused || exhausted
                    ? null
                    : $"must be refused, and answered {Format(observed)}";

            case CorpusStore.SoundEitherWay:
                if (!accepted && !refused && !exhausted)
                {
                    return $"answered {Format(observed)}, which is neither an acceptance, a " +
                        "refusal, nor an exhaustion";
                }

                if (refused && !PublishedCodes.Contains(observed.DiagnosticCode))
                {
                    return $"was refused with code {observed.DiagnosticCode}, which no published " +
                        "enumeration holds";
                }

                return refused &&
                    observed.DiagnosticCode == (int)WebAssemblyDiagnosticCode.VerifierDefect
                        ? "was refused with the verifier reporting its own defect"
                        : null;

            default:
                return $"names the invariant {invariant}, which this replay does not know";
        }
    }

    /// <summary>
    /// Every diagnostic code the profile publishes, read off the enumeration itself.
    /// </summary>
    /// <remarks>
    /// Read off the type rather than listed here. A list written by hand would have to be kept in
    /// step with the profile by somebody remembering to, and the failure would be a sweep row
    /// reported as carrying an unpublished code when the code had merely been added.
    /// </remarks>
    private static readonly ImmutableHashSet<int> PublishedCodes =
        [.. Enum.GetValues<WebAssemblyDiagnosticCode>().Select(code => (int)code)];

    internal static string Format(ReplayObservation observed) =>
        $"{observed.Outcome}/{observed.Reason}/{observed.DiagnosticCode}/" +
        $"{observed.Dimension}/{observed.Scope}";

    private static string Format(ManifestRow row) =>
        $"{row.Outcome}/{row.Reason}/{row.DiagnosticCode}/{row.Dimension}/{row.Scope}";

    // =============================================================================================
    // The lane
    // =============================================================================================

    /// <summary>
    /// Replays the retained corpus TWICE, holds every row to its recorded answer and its family's
    /// invariant, and reports one line per family.
    /// </summary>
    /// <remarks>
    /// Twice, because a replay that left residue - a verifier holding state across artifacts, a
    /// meter that never reset - would pass the first time and the property being claimed is that it
    /// does not. The two passes are compared row by row, so a difference is reported as the entry
    /// that moved rather than as a changed total.
    /// </remarks>
    internal static int Report(VmRuntime runtime, string directory, bool verbose)
    {
        var manifestPath = Path.Combine(directory, CorpusStore.ManifestFileName);

        if (!File.Exists(manifestPath))
        {
            Console.WriteLine($"FAIL no {CorpusStore.ManifestFileName} in {directory}");
            return 1;
        }

        var rows = ReadManifest(manifestPath);
        var first = Replay(runtime, directory, rows);
        var second = Replay(runtime, directory, rows);

        var failed = 0;
        var hashesChecked = 0;

        // THE RESIDUE CLAUSE OF THE SUMMARY IS COUNTED RATHER THAN ASSERTED. Every other clause of
        // that line is derived from what the passes did; this one was a bare literal, so a
        // residue-only failure printed its own FAIL row and then, four lines later, the words "no
        // residue" in the summary a reader quotes.
        var moved = 0;

        Console.WriteLine($"# retained corpus: {rows.Length} entries under {directory}");

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            var observed = first[index];
            var complaints = new List<string>();

            if (observed.HashStatus != "match")
            {
                complaints.Add($"hash {observed.HashStatus}");
            }
            else
            {
                hashesChecked++;
            }

            if (!Agrees(row, observed))
            {
                complaints.Add($"expected {Format(row)}, observed {Format(observed)}");
            }

            var invariant = CheckInvariant(row.Invariant, observed);

            if (invariant is not null)
            {
                complaints.Add($"invariant {row.Invariant}: {invariant}");
            }

            if (first[index] != second[index])
            {
                moved++;
                complaints.Add(
                    $"the second pass answered {Format(second[index])} where the first answered " +
                    Format(observed));
            }

            if (complaints.Count > 0)
            {
                failed++;
                Console.WriteLine($"FAIL {row.Name}: {string.Join("; ", complaints)}");
            }
            else if (verbose)
            {
                Console.WriteLine($"ok   {row.Name}: {Format(observed)} [{row.Provenance}]");
            }
        }

        // A corpus in which nothing passes would not notice a verifier that refuses everything, so
        // the accepting rows are counted rather than assumed.
        var accepting = rows.Count(
            row => string.Equals(row.Outcome, "Normal", StringComparison.Ordinal));

        if (accepting == 0)
        {
            failed++;
            Console.WriteLine("FAIL the corpus contains no entry that verifies");
        }

        foreach (var family in rows.GroupBy(row => row.Family).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var derived = family.Count(
                row => string.Equals(row.Provenance, "derived", StringComparison.Ordinal));

            Console.WriteLine(
                $"  {family.Key,-22} {family.Count(),4} entries  " +
                $"{derived,4} derived  {family.Count() - derived,4} recorded  " +
                $"invariants {string.Join(",", family.Select(row => row.Invariant).Distinct().Order(StringComparer.Ordinal))}");
        }

        Console.WriteLine(
            $"# retained corpus: {rows.Length - failed} of {rows.Length} entries reproduced, " +
            $"{hashesChecked} hashes re-computed and matched, {accepting} entries verify, " +
            (moved == 0
                ? "replayed twice with no residue"
                : $"replayed twice and {moved} entries answered differently on the second pass"));

        // WHICH STAGE ANSWERED, COUNTED FROM THE FIRST PASS, and what that makes of class (r). The
        // count is derived rather than asserted, like the residue clause above: a core refusal of a
        // translated module has already failed its row, and this line says how many there were.
        var translated = first.Count(o => string.Equals(o.Stage, CoreStage, StringComparison.Ordinal));
        var refusedAtTranslation = first.Count(o => string.Equals(o.Stage, TranslationStage, StringComparison.Ordinal));
        var refusedByCore = first.Count(o =>
            string.Equals(o.Stage, CoreStage, StringComparison.Ordinal) &&
            !string.Equals(o.Outcome, "Normal", StringComparison.Ordinal));

        Console.WriteLine(
            $"# retained corpus: {refusedAtTranslation} entries answered by the translator, " +
            $"{translated} translated and answered by the core, {refusedByCore} of them refused - " +
            (refusedByCore == 0
                ? "class (r) is empty"
                : "class (r) must be empty, and each is a FAIL above"));

        return failed;
    }
}
