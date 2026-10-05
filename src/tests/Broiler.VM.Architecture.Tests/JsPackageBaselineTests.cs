using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N37: the JavaScript family's package baseline is what the checkout packs and what the
/// candidate pack produced, in both directions, and the bundle it names shows a pristine consumer
/// restoring, running, rolling back and forward, and refusing what only upstream holds (phase F9
/// slice R3, decision JSD-0061).
/// </summary>
/// <remarks>
/// <para>
/// <b>Release gate 8's baseline of its own.</b> Four directions are held: the baseline's packages
/// against the family projects that carry <c>IsPackable</c> true; each package's dependencies against
/// its project's references; each package's dependencies and files against the <c>.nuspec</c> and
/// file list the bundle retained; and every <c>.nuspec</c> the candidate produced against naming
/// nothing outside <c>Broiler.VM.</c>. So a package added or removed, a reference added, a file
/// gained or a foreign dependency declared each fails.
/// </para>
/// <para>
/// <b>The same limit as rule L1.</b> The bundle's metadata is read against the document, and the
/// document against the checkout's project files; a pack taken today is not re-run. A stale bundle
/// and a stale baseline that agree would pass, and what covers that is the bundle's recertification
/// triggers.
/// </para>
/// </remarks>
public sealed class JsPackageBaselineTests
{
    private const string BaselineName = "src/Broiler.VM.Profile.JavaScript/docs/packages.md";

    private static readonly Regex BundleLink = new(
        @"\*\*bundle \[(?<id>JS-10-[0-9]{3})\]\(evidence/(?<directory>js-10-[0-9]{3})/README\.md\)\*\*",
        RegexOptions.Compiled);

    private sealed record Row(string Package, string[] Dependencies, string[] Files);

    [Fact]
    public void N37_The_Package_Baseline_Is_What_The_Checkout_Packs_And_The_Bundle_Retained()
    {
        var rows = Rows(Baseline());

        Assert.Equal(4, rows.Count);
        Assert.Empty(Violations(rows, Bundle("nuspecs.txt"), Bundle("contents.txt"), Bundle("consumer.log")));
    }

    [Fact]
    public void N37_Rejects_A_Package_The_Checkout_Does_Not_Pack()
    {
        Assert.Contains(Violations(Rows(Witness("N37-baseline-adds-a-package.md.witness")), Bundle("nuspecs.txt"), Bundle("contents.txt"), Bundle("consumer.log")), violation =>
            violation.Contains("Broiler.VM.Profile.JavaScript.Workers", StringComparison.Ordinal) &&
            violation.Contains("no packable project", StringComparison.Ordinal));
    }

    [Fact]
    public void N37_Rejects_A_Baseline_That_Omits_A_Packed_Package()
    {
        Assert.Contains(Violations(Rows(Witness("N37-baseline-omits-a-package.md.witness")), Bundle("nuspecs.txt"), Bundle("contents.txt"), Bundle("consumer.log")), violation =>
            violation.Contains("Broiler.VM.Profile.JavaScript.Intl", StringComparison.Ordinal) &&
            violation.Contains("has no row", StringComparison.Ordinal));
    }

    [Fact]
    public void N37_Rejects_A_Dependency_Neither_The_Project_Nor_The_Pack_Declares()
    {
        var violations = Violations(Rows(Witness("N37-baseline-misstates-a-dependency.md.witness")), Bundle("nuspecs.txt"), Bundle("contents.txt"), Bundle("consumer.log"));

        Assert.Contains(violations, violation =>
            violation.Contains("Broiler.VM.Profile.JavaScript.Compiler", StringComparison.Ordinal) &&
            violation.Contains("project", StringComparison.Ordinal));
        Assert.Contains(violations, violation =>
            violation.Contains("Broiler.VM.Profile.JavaScript.Compiler", StringComparison.Ordinal) &&
            violation.Contains(".nuspec", StringComparison.Ordinal));
    }

    [Fact]
    public void N37_Rejects_A_Foreign_Dependency_In_Produced_Metadata()
    {
        Assert.Contains(Violations(Rows(Baseline()), Witness("N37-nuspecs-declare-a-foreign-dependency.txt.witness"), Bundle("contents.txt"), Bundle("consumer.log")), violation =>
            violation.Contains("Newtonsoft.Json", StringComparison.Ordinal) &&
            violation.Contains("foreign", StringComparison.Ordinal));
    }

    [Fact]
    public void N37_Rejects_A_Transcript_Whose_Rollback_Did_Not_Run_Clean()
    {
        Assert.Contains(Violations(Rows(Baseline()), Bundle("nuspecs.txt"), Bundle("contents.txt"), Witness("N37-consumer-log-rollback-failed.log.witness")), violation =>
            violation.Contains("ROLL BACK", StringComparison.Ordinal));
    }

    [Fact]
    public void N37_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, "N37", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(BaselineName, row.Evidence, StringComparison.Ordinal);
    }

    private static List<string> Violations(IReadOnlyList<Row> rows, string nuspecs, string contents, string consumer)
    {
        var violations = new List<string>();
        var projects = PackableFamilyProjects();
        var produced = Nuspecs(nuspecs);

        foreach (var row in rows)
        {
            if (!projects.TryGetValue(row.Package, out var references))
            {
                violations.Add($"{BaselineName} names {row.Package}, and the checkout has no packable project of that name");
                continue;
            }

            Both(violations, row.Dependencies, references, $"{row.Package}'s dependencies", "its project's references");

            if (!produced.TryGetValue(row.Package, out var declared))
            {
                violations.Add($"the bundle {BaselineName} names carries no .nuspec for {row.Package}");
                continue;
            }

            Both(violations, row.Dependencies, declared, $"{row.Package}'s dependencies", "its .nuspec");

            var files = contents.Split('\n')
                .Select(static line => line.Trim().Split(' '))
                .Where(parts => parts.Length == 3 && parts[0].StartsWith(row.Package + ".0", StringComparison.Ordinal) &&
                                !string.Equals(parts[1], row.Package + ".nuspec", StringComparison.Ordinal))
                .Select(static parts => parts[1])
                .ToArray();

            Both(violations, row.Files, files, $"{row.Package}'s files", "the package the bundle retained");
        }

        foreach (var project in projects.Keys.Where(project => !rows.Any(row => row.Package == project)))
        {
            violations.Add($"the checkout packs {project}, and {BaselineName} has no row for it");
        }

        foreach (var (package, dependencies) in produced)
        {
            foreach (var dependency in dependencies.Where(static id => !id.StartsWith("Broiler.VM.", StringComparison.Ordinal)))
            {
                violations.Add($"the candidate's {package}.nuspec declares the foreign dependency {dependency}");
            }
        }

        violations.AddRange(ConsumerViolations(consumer));
        return violations;
    }

    private static void Both(List<string> violations, IEnumerable<string> stated, IEnumerable<string> actual, string what, string against)
    {
        foreach (var missing in actual.Except(stated, StringComparer.Ordinal))
        {
            violations.Add($"{BaselineName} omits {missing} from {what}, and {against} names it");
        }

        foreach (var extra in stated.Except(actual, StringComparer.Ordinal))
        {
            violations.Add($"{BaselineName} names {extra} in {what}, and {against} does not");
        }
    }

    /// <summary>The transcript's five steps: three clean runs, a refused restore, and a clean native run.</summary>
    private static IEnumerable<string> ConsumerViolations(string consumer)
    {
        var sections = Regex.Split(consumer, @"(?m)^--- ").Skip(1).ToList();

        string? Section(string start) => sections.FirstOrDefault(section => section.StartsWith(start, StringComparison.Ordinal));

        foreach (var start in new[] { "restore and run the candidate", "ROLL BACK to the published", "roll forward to the candidate", "run the Native AOT consumer" })
        {
            var section = Section(start);

            if (section is null || !Regex.IsMatch(section, @"checks passed\s*\nexit code: 0\s*$"))
            {
                yield return $"the bundle's consumer transcript does not show `{start}` passing every check and exiting 0";
            }
        }

        // THE BODY, NOT THE TITLE: the title names nuget.org as what holds the version.
        var control = Section("NEGATIVE CONTROL") is { } found ? found[(found.IndexOf('\n') + 1)..] : null;

        if (control is null || !control.Contains("NU1102", StringComparison.Ordinal) ||
            control.Contains("nuget.org", StringComparison.Ordinal) || Regex.IsMatch(control, @"exit code: 0\s*$"))
        {
            yield return "the bundle's consumer transcript does not show the negative control refused for a version the local feed lacks, with no other source searched";
        }
    }

    /// <summary>Every family project that packs, with the assembly names of the projects it references.</summary>
    private static Dictionary<string, string[]> PackableFamilyProjects()
    {
        var projects = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var directory in Directory.GetDirectories(Path.Combine(ComponentGraph.Root, "src"), "Broiler.VM.Profile.JavaScript*"))
        {
            var name = Path.GetFileName(directory);
            var path = Path.Combine(directory, name + ".csproj");

            if (!File.Exists(path))
            {
                continue;
            }

            var text = File.ReadAllText(path);

            if (!Regex.IsMatch(text, @"<IsPackable>\s*true\s*</IsPackable>"))
            {
                continue;
            }

            projects[name] = Regex.Matches(text, @"<ProjectReference\s+Include=""(?<path>[^""]+)""")
                .Select(static match => Path.GetFileNameWithoutExtension(match.Groups["path"].Value.Replace('\\', '/')))
                .ToArray();
        }

        return projects;
    }

    /// <summary>Each produced package's dependency identifiers, read from the retained .nuspec text.</summary>
    private static Dictionary<string, string[]> Nuspecs(string text)
    {
        var packages = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var block in Regex.Split(text, @"(?m)^=== ").Skip(1))
        {
            var id = Regex.Match(block, @"<id>(?<id>[^<]+)</id>");

            if (id.Success)
            {
                packages[id.Groups["id"].Value] = Regex.Matches(block, @"<dependency id=""(?<id>[^""]+)""")
                    .Select(static match => match.Groups["id"].Value)
                    .ToArray();
            }
        }

        return packages;
    }

    /// <summary>The baseline table's rows: a backticked package, its dependencies or <c>none</c>, its files.</summary>
    private static List<Row> Rows(string document)
    {
        var rows = new List<Row>();

        foreach (var line in document.Split('\n'))
        {
            var cells = line.Trim().Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (!line.StartsWith('|') || cells.Length != 3 || !Regex.IsMatch(cells[0], @"^`Broiler\.VM\.[A-Za-z.]+`$"))
            {
                continue;
            }

            static string[] Names(string cell) => Regex.Matches(cell, "`([^`]+)`").Select(static match => match.Groups[1].Value).ToArray();

            rows.Add(new Row(cells[0].Trim('`'), Names(cells[1]), Names(cells[2])));
        }

        return rows;
    }

    private static string Baseline() => File.ReadAllText(Path.Combine(ComponentGraph.Root, BaselineName));

    private static string Bundle(string fileName)
    {
        var link = BundleLink.Match(Baseline());
        Assert.True(link.Success, $"{BaselineName} names no bundle in the form **bundle [JS-10-nnn](evidence/js-10-nnn/README.md)**");

        var path = Path.Combine(ComponentGraph.Root, "src", "Broiler.VM.Profile.JavaScript", "docs", "evidence", link.Groups["directory"].Value, fileName);
        Assert.True(File.Exists(path), $"bundle {link.Groups["id"].Value} retains no {fileName}");
        return File.ReadAllText(path);
    }

    private static string Witness(string fileName)
    {
        var path = Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "js-packages", fileName);
        Assert.True(File.Exists(path), $"Missing witness input {path}.");
        return File.ReadAllText(path);
    }
}
