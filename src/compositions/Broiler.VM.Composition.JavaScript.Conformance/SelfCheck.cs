namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>What the harness must report for one fixture, and what it did report.</summary>
internal sealed record SelfCheckCase(
    string Path,
    ConformanceStatus ExpectedStatus,
    CompletionKind ExpectedCompletion,
    ConformanceStatus ActualStatus,
    CompletionKind ActualCompletion,
    string Detail)
{
    /// <summary>Whether the harness reported what the fixture manifest says it must.</summary>
    internal bool Matched => ExpectedStatus == ActualStatus && ExpectedCompletion == ActualCompletion;
}

/// <summary>
/// The check that runs before every shard: does a failing test come back as a failure?
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the harness's first job and it is not scoring.</b> A pass rate cannot ask whether
/// the thing producing it can report a failure at all, and a scorer that reported everything as a
/// pass would publish its best number ever. The fixtures here are deliberately broken in the ways
/// this profile can be broken - a wrong value, a refusal that does not happen, a refusal with the
/// wrong code, a program that never terminates - and the manifest beside them records the verdict
/// the harness has to reach. A mismatch stops the run before a single suite test is scored.
/// </para>
/// <para>
/// <b>At least one fixture must be a control that passes</b>, and the absence of one is itself a
/// failure. Fixtures that all fail are satisfied by a harness that reports everything as a
/// failure, which is the same defect facing the other way.
/// </para>
/// <para>
/// <b>The fixtures are scored through the same code path as a suite test.</b> They are read by the
/// same metadata reader, presented through the same host modes and run by the same
/// <see cref="Execution"/>; nothing here calls a shortcut. A self-check with a path of its own
/// would be checking something the run does not use.
/// </para>
/// </remarks>
internal static class SelfCheck
{
    /// <summary>The file beside the fixtures declaring the verdict the harness must reach.</summary>
    internal const string ManifestFileName = "expected.manifest";

    /// <summary>The header that file carries.</summary>
    internal const string Header = "# broiler-js-conformance self-check 1";

    /// <summary>Runs every fixture and compares the harness's verdict with the declared one.</summary>
    internal static IReadOnlyList<SelfCheckCase> Run(
        string root,
        Execution execution,
        out IReadOnlyList<string> failures)
    {
        var complaints = new List<string>();
        var declared = ReadManifest(Path.Combine(root, ManifestFileName), complaints);
        var tests = Suite.Read(root, out var unreadable);
        var cases = new List<SelfCheckCase>();

        complaints.AddRange(unreadable);

        foreach (var test in tests.OrderBy(static test => test.Path, StringComparer.Ordinal))
        {
            if (!declared.TryGetValue(test.Path, out var expected))
            {
                complaints.Add($"{test.Path} is a fixture that {ManifestFileName} declares no verdict for");
                continue;
            }

            Observation observed;

            try
            {
                observed = execution.Run(test);
            }
            catch (Exception failure) when (failure is not OutOfMemoryException)
            {
                observed = CompletionProtocol.Escaped(failure);
            }

            cases.Add(new SelfCheckCase(
                test.Path,
                expected.Status,
                expected.Completion,
                observed.Status,
                observed.Completion,
                observed.Detail));
        }

        foreach (var orphan in declared.Keys
                     .Where(path => tests.All(test => !string.Equals(test.Path, path, StringComparison.Ordinal))))
        {
            complaints.Add($"{ManifestFileName} declares a verdict for {orphan}, which is not a fixture");
        }

        // The control clause, asserted rather than assumed. Fixtures that all fail are satisfied by
        // a harness that reports every case as a failure.
        if (cases.Count != 0 && cases.All(static one => one.ExpectedStatus != ConformanceStatus.Passed))
        {
            complaints.Add(
                $"{ManifestFileName} declares no fixture that must pass: a self-check made only of " +
                "failures is met by a harness that fails everything");
        }

        failures = complaints;
        return cases;
    }

    /// <summary>Reads the declared verdicts.</summary>
    private static Dictionary<string, (ConformanceStatus Status, CompletionKind Completion)> ReadManifest(
        string path,
        List<string> complaints)
    {
        var declared = new Dictionary<string, (ConformanceStatus, CompletionKind)>(StringComparer.Ordinal);

        if (!File.Exists(path))
        {
            complaints.Add($"{path} does not exist, so no fixture declares a verdict");
            return declared;
        }

        var seenHeader = false;

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != 3)
            {
                complaints.Add($"{path}: `{line}` is not `path|status|completion`");
                continue;
            }

            if (!Enum.TryParse<ConformanceStatus>(parts[1], out var status) ||
                !Enum.TryParse<CompletionKind>(parts[2], out var completion))
            {
                complaints.Add($"{path}: `{line}` names a status or completion kind that is not one");
                continue;
            }

            declared[Suite.Normalize(parts[0])] = (status, completion);
        }

        if (!seenHeader)
        {
            complaints.Add($"{path} does not open with `{Header}`");
        }

        return declared;
    }
}
