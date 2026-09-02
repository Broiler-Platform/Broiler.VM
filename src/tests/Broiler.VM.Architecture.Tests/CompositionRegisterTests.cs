using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group K: the register, the catalogs and the published closures, held to each other.
/// </summary>
/// <remarks>
/// Each rule is asserted twice, like every other group: the component is clean, and the rule
/// rejects a violating input. The violating inputs here are constructed in the test rather than
/// stored as files, except where a file is what the rule reads - the register itself and the two
/// retained bundle artefacts - because a rule over parsed rows is witnessed by a row, and writing
/// a whole second copy of the register to change one cell would make the witness harder to read
/// than the rule.
/// </remarks>
public sealed class CompositionRegisterTests
{
    private static readonly IReadOnlyList<string> Roots = ComponentGraph.Projects
        .Where(static project => project.IsComposition)
        .Select(static project => project.AssemblyName)
        .OrderBy(static name => name, StringComparer.Ordinal)
        .ToArray();

    private static readonly IReadOnlyList<CompositionRules.Row> Rows = ReadRegister();

    /// <summary>
    /// The rows that name a composition root the checkout actually has.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>K1 owns a row with no subject, and the three rules below presuppose one.</b> Each of them
    /// asks a question about a row's artefacts - its project's reference set, its catalog baseline,
    /// its retained closure - and a row naming a composition that does not exist has none of those
    /// to be wrong about.
    /// </para>
    /// <para>
    /// <b>Until 2026-09-02 they did not presuppose it, they assumed it.</b> A phantom row made K2
    /// throw <c>Sequence contains no matching element</c> and K3 throw <c>FileNotFoundException</c>,
    /// while K4 reported that the bundle had retained no closure report - a true sentence blaming
    /// the wrong file. Three tests crashed or misattributed, and the one rule with something
    /// accurate to say about the input was drowned by them. Skipping here is not a weakening: the
    /// row is still reported, by K1, which is the rule whose subject it is.
    /// </para>
    /// </remarks>
    private static IEnumerable<CompositionRules.Row> Registered =>
        Rows.Where(static row => Roots.Contains(row.Composition, StringComparer.Ordinal));

    /// <summary>How many registered compositions have a root in the checkout.</summary>
    /// <remarks>Internal so rule J12's figure catalog can cite it rather than a row typing it.</remarks>
    internal static int RegisteredCount => Registered.Count();

    [Fact]
    public void K1_The_Register_And_The_Checkout_Name_The_Same_Compositions()
    {
        Assert.NotEmpty(Roots);
        Assert.Empty(CompositionRules.K1(Roots, Rows));

        // An undocumented composition root: the entry the exit gate names.
        Assert.Contains(
            CompositionRules.K1([.. Roots, "Broiler.VM.Composition.Undocumented"], Rows),
            message => message.Contains("Undocumented", StringComparison.Ordinal));

        // A row describing a composition that is not in the checkout. Worse than silence: it reads
        // as a support claim for something that does not exist.
        Assert.Contains(
            CompositionRules.K1(
                Roots,
                [.. Rows, new CompositionRules.Row(
                    "Broiler.VM.Composition.Deleted", "demonstration", ["com.example.gone"], ["Com.Example.Gone"])]),
            message => message.Contains("not a composition root", StringComparison.Ordinal));

        // A kind outside the two the schema allows.
        Assert.Contains(
            CompositionRules.K1(
                ["Broiler.VM.Composition.Calculator"],
                [new CompositionRules.Row(
                    "Broiler.VM.Composition.Calculator", "sample", ["com.example.calculator"], ["Com.Example.Calculator"])]),
            message => message.Contains("not one of", StringComparison.Ordinal));
    }

    [Fact]
    public void K2_Each_Row_Agrees_With_The_Reference_Set_And_The_Catalog()
    {
        foreach (var row in Registered)
        {
            var project = ComponentGraph.Projects.Single(candidate =>
                string.Equals(candidate.AssemblyName, row.Composition, StringComparison.Ordinal));

            Assert.Empty(CompositionRules.K2(
                row, project.ReferencedAssemblyNames.ToArray(), Baseline(row.Composition)));
        }

        var calculator = Rows.Single(row =>
            string.Equals(row.Composition, "Broiler.VM.Composition.Calculator", StringComparison.Ordinal));

        var catalog = Baseline(calculator.Composition);

        // A composition that links a profile its row does not declare. This is how a closure grows
        // silently: the project file changes, the register does not, and every document describing
        // the composition is now describing a different one.
        Assert.Contains(
            CompositionRules.K2(
                calculator,
                [.. CompositionRules.CoreAssemblies, "Com.Example.Calculator", "Com.Example.Ledger"],
                catalog),
            message => message.Contains("Com.Example.Ledger", StringComparison.Ordinal));

        // A catalog that composed something the register does not name.
        Assert.Contains(
            CompositionRules.K2(
                calculator,
                [.. CompositionRules.CoreAssemblies, "Com.Example.Calculator"],
                new CompositionRules.CatalogTable(
                    calculator.Composition, ["com.example.other"], ["Com.Example.Calculator"])),
            message => message.Contains("com.example.other", StringComparison.Ordinal));

        // The same profile twice.
        Assert.Contains(
            CompositionRules.K2(
                calculator,
                [.. CompositionRules.CoreAssemblies, "Com.Example.Calculator"],
                new CompositionRules.CatalogTable(
                    calculator.Composition,
                    ["com.example.calculator", "com.example.calculator"],
                    ["Com.Example.Calculator", "Com.Example.Calculator"])),
            message => message.Contains("2 times", StringComparison.Ordinal));

        // A consumer profile claiming the reserved first label. The core refuses this at catalog
        // construction when the package identity does not match; the record-level check is here
        // because a bundle that documented such a composition would be documenting a support
        // claim nobody made.
        Assert.Contains(
            CompositionRules.K2(
                calculator,
                [.. CompositionRules.CoreAssemblies, "Com.Example.Calculator"],
                new CompositionRules.CatalogTable(
                    calculator.Composition, ["Broiler.Vm.Calculator"], ["Com.Example.Calculator"])),
            message => message.Contains("reserved first label", StringComparison.Ordinal));
    }

    [Fact]
    public void K3_Each_Catalog_Baseline_Matches_What_The_Published_Composition_Printed()
    {
        foreach (var row in Registered)
        {
            var slug = Slug(row.Composition);

            Assert.Empty(CompositionRules.K3(
                row.Composition, BaselineText(slug), RetainedFor(row, $"catalog-{slug}.txt")));
        }

        var first = Rows[0];
        var baseline = BaselineText(Slug(first.Composition));

        // Drift in either direction is drift. A baseline that gained a profile the published
        // binary did not compose is the same defect as one that lost the profile it did.
        Assert.Contains(
            CompositionRules.K3(
                first.Composition, baseline, baseline + "\nprofile com.example.stowaway Com.Example.Stowaway 1 0"),
            message => message.Contains("drifted", StringComparison.Ordinal));

        Assert.Contains(
            CompositionRules.K3(first.Composition, baseline, string.Empty),
            message => message.Contains("retained no catalog table", StringComparison.Ordinal));
    }

    [Fact]
    public void K4_Each_Published_Closure_Contains_Exactly_What_It_Declares()
    {
        foreach (var row in Registered)
        {
            Assert.Empty(CompositionRules.K4(row, ClosureModesFor(row)));
        }

        var calculator = Rows.Single(row =>
            string.Equals(row.Composition, "Broiler.VM.Composition.Calculator", StringComparison.Ordinal));

        var clean = new[]
        {
            new CompositionRules.ClosureMode("trimmed", [
                .. CompositionRules.CoreAssemblies, calculator.Composition, .. calculator.ProfileAssemblies]),
        };

        Assert.Empty(CompositionRules.K4(calculator, clean));

        // The fixture profile in a shipped image: the failure the exit gate names by name.
        Assert.Contains(
            CompositionRules.K4(calculator, [
                new CompositionRules.ClosureMode("trimmed", [
                    .. CompositionRules.CoreAssemblies, calculator.Composition,
                    .. calculator.ProfileAssemblies, "Broiler.VM.Fixtures"])]),
            message => message.Contains("ships Broiler.VM.Fixtures", StringComparison.Ordinal));

        // A testing framework, which is the other half of that sentence.
        Assert.Contains(
            CompositionRules.K4(calculator, [
                new CompositionRules.ClosureMode("trimmed", [
                    .. CompositionRules.CoreAssemblies, calculator.Composition,
                    .. calculator.ProfileAssemblies, "xunit.core"])]),
            message => message.Contains("ships xunit.core", StringComparison.Ordinal));

        // A profile the single-profile composition does not name. A closure report that contained
        // it would mean the composition linked a profile nobody registered.
        Assert.Contains(
            CompositionRules.K4(calculator, [
                new CompositionRules.ClosureMode("trimmed", [
                    .. CompositionRules.CoreAssemblies, calculator.Composition,
                    .. calculator.ProfileAssemblies, "Com.Example.Ledger"])]),
            message => message.Contains("Com.Example.Ledger", StringComparison.Ordinal));

        // A closure missing an assembly it declares. The equality is two-sided: an image that
        // dropped a profile would run nothing and still pass a subset check.
        Assert.Contains(
            CompositionRules.K4(calculator, [
                new CompositionRules.ClosureMode("trimmed", [
                    .. CompositionRules.CoreAssemblies, calculator.Composition])]),
            message => message.Contains("rather than", StringComparison.Ordinal));

        Assert.Contains(
            CompositionRules.K4(calculator, []),
            message => message.Contains("retained no closure report", StringComparison.Ordinal));
    }

    /// <summary>
    /// The single-profile and two-profile closures differ by exactly one assembly.
    /// </summary>
    /// <remarks>
    /// The gate asks that adding a second profile require no change to the core runtime or the
    /// execution loop. The three core assemblies are byte-identical across the two compositions -
    /// they are the same build - so what a reader can check here is the other half: the difference
    /// between the two images is the second profile and nothing else.
    /// </remarks>
    [Fact]
    public void The_Two_Closures_Differ_By_Exactly_One_Profile_Assembly()
    {
        var single = ClosureModes("calculator")
            .Single(mode => string.Equals(mode.Name, "trimmed", StringComparison.Ordinal))
            .Assemblies;

        var both = ClosureModes("workbench")
            .Single(mode => string.Equals(mode.Name, "trimmed", StringComparison.Ordinal))
            .Assemblies;

        // Every assembly of the single-profile closure except its own composition root appears in
        // the two-profile one.
        var shared = single
            .Where(name => !name.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal))
            .ToArray();

        Assert.All(shared, name => Assert.Contains(name, both));

        var extra = both
            .Where(name => !shared.Contains(name, StringComparer.Ordinal))
            .Where(name => !name.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(["Com.Example.Ledger"], extra);
    }

    /// <summary>
    /// Writes what each group K rule said about this checkout, when asked to.
    /// </summary>
    /// <remarks>
    /// The mechanism and its reasons live on <see cref="RuleReport"/>; what is here is group K's
    /// inputs, which are this class's own. The report is over exactly what the four tests above
    /// compare - a report over different inputs would answer a question nobody asked.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_K_Are_Written_When_Asked_For()
    {
        RuleReport.Write("K",
        [
            ("K1", () => CompositionRules.K1(Roots, Rows)),
            ("K2", () => Registered.SelectMany(row => CompositionRules.K2(
                row,
                ComponentGraph.Projects
                    .Single(candidate => string.Equals(
                        candidate.AssemblyName, row.Composition, StringComparison.Ordinal))
                    .ReferencedAssemblyNames.ToArray(),
                Baseline(row.Composition)))),
            ("K3", () => Registered.SelectMany(row => CompositionRules.K3(
                row.Composition,
                BaselineText(Slug(row.Composition)),
                RetainedFor(row, $"catalog-{Slug(row.Composition)}.txt")))),
            ("K4", () => Registered.SelectMany(row =>
                CompositionRules.K4(row, ClosureModesFor(row)))),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "K.txt")),
                "a report for group K was asked for and none was written");
        }
    }

    private static CompositionRules.CatalogTable Baseline(string composition) =>
        ParseCatalog(BaselineText(Slug(composition)));

    /// <summary>Reads a retained artefact out of the bundle the row itself names.</summary>
    /// <remarks>
    /// Per row rather than per repository, because two milestone series now keep two evidence
    /// trees. A row with no bundle named falls back to the core's current one, which is what every
    /// row said implicitly before the column existed.
    /// </remarks>
    private static string RetainedFor(CompositionRules.Row row, string fileName)
    {
        var bundle = row.Evidence.Length == 0
            ? Path.Combine("docs", "evidence", ComponentGraph.CurrentEvidenceDirectory)
            : row.Evidence.Replace('/', Path.DirectorySeparatorChar);

        var path = Path.Combine(ComponentGraph.Root, bundle, fileName);

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    /// <summary>Reads a retained closure report out of the bundle the row itself names.</summary>
    private static IReadOnlyList<CompositionRules.ClosureMode> ClosureModesFor(CompositionRules.Row row) =>
        ParseClosure(RetainedFor(row, $"closure-{Slug(row.Composition)}.txt"));

    private static string BaselineText(string slug) =>
        File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "catalogs",
            slug + ".catalog.txt"));

    private static string Retained(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "docs", "evidence", ComponentGraph.CurrentEvidenceDirectory, fileName);

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    /// <summary>
    /// Reads a retained closure report: a header line per mode, then one assembly name per line.
    /// </summary>
    private static IReadOnlyList<CompositionRules.ClosureMode> ClosureModes(string slug) =>
        ParseClosure(Retained($"closure-{slug}.txt"));

    private static IReadOnlyList<CompositionRules.ClosureMode> ParseClosure(string text)
    {
        if (text.Length == 0)
        {
            return [];
        }

        var modes = new List<CompositionRules.ClosureMode>();
        List<string>? current = null;
        var name = string.Empty;

        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var header = Regex.Match(line, @"^\[(?<mode>[a-z]+)\]");

            if (header.Success)
            {
                if (current is not null)
                {
                    modes.Add(new CompositionRules.ClosureMode(name, current));
                }

                name = header.Groups["mode"].Value;
                current = [];
                continue;
            }

            current?.Add(line);
        }

        if (current is not null)
        {
            modes.Add(new CompositionRules.ClosureMode(name, current));
        }

        return modes;
    }

    /// <summary>
    /// Reads the catalog table a composition prints: its own name, then one line per profile.
    /// </summary>
    private static CompositionRules.CatalogTable ParseCatalog(string text)
    {
        var composition = string.Empty;
        var ids = new List<string>();
        var packages = new List<string>();

        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2 && string.Equals(parts[0], "composition", StringComparison.Ordinal))
            {
                composition = parts[1];
            }
            else if (parts.Length >= 3 && string.Equals(parts[0], "profile", StringComparison.Ordinal))
            {
                ids.Add(parts[1]);
                packages.Add(parts[2]);
            }
        }

        return new CompositionRules.CatalogTable(composition, ids, packages);
    }

    /// <summary>
    /// Reads the composition table out of the register: the one table whose header row opens with
    /// the Composition column.
    /// </summary>
    /// <remarks>
    /// The table is found by its header rather than by position, and the search requires the header
    /// to be unique. A reader that took the first table it met would be readable from any table
    /// somebody added above it, which is the defect the group H review found four times.
    /// </remarks>
    private static IReadOnlyList<CompositionRules.Row> ReadRegister()
    {
        var path = Path.Combine(ComponentGraph.Root, "docs", "compositions.md");
        var lines = File.ReadAllLines(path);

        var headers = lines
            .Select(static (line, index) => (Line: line.Trim(), Index: index))
            .Where(static entry => entry.Line.StartsWith("| Composition | Kind |", StringComparison.Ordinal))
            .ToArray();

        Assert.Single(headers);

        // The column count the header fixes, counted the way the rows below are counted: NON-EMPTY
        // cells. Counting the split's length instead is a check that cannot fail - emptying a cell
        // leaves the pipes where they are, so the split is the same length and only the non-empty
        // count moves. The first version of this check did exactly that and passed the injection
        // it was written to stop.
        var columns = headers[0].Line
            .Split('|', StringSplitOptions.TrimEntries)
            .Count(static cell => cell.Length != 0);

        var rows = new List<CompositionRules.Row>();

        for (var index = headers[0].Index + 2; index < lines.Length; index++)
        {
            var line = lines[index].Trim();

            if (!line.StartsWith('|'))
            {
                break;
            }

            // AN EMPTY CELL IS A MALFORMED ROW AND NOT AN EMPTY VALUE, and until 2026-09-02 this
            // parser silently disagreed. It drops empty cells before indexing, so emptying one
            // does not produce an empty list - it shifts every column after it LEFT. A row whose
            // profile-assembly cell was cleared came back declaring its SIBLING as its profile
            // assembly, its capability column as its siblings, and an evidence path one column
            // over; K1 stayed green because the two counts moved together, and the rules that did
            // fire fired for reasons that had nothing to do with the edit.
            //
            // It was found by an injection that was supposed to test something else, which is the
            // only reason it was found at all: a register is hand-maintained prose, and clearing
            // a cell is exactly what someone does when a column stops applying. The count is
            // checked against the header now, and a mismatch stops the read rather than producing
            // a row nobody wrote.
            var cells = line.Split('|', StringSplitOptions.TrimEntries)
                .Where(static cell => cell.Length != 0)
                .ToArray();

            if (cells.Length != columns)
            {
                throw new InvalidOperationException(
                    $"docs/compositions.md line {index + 1} has {cells.Length} non-empty cells and " +
                    $"the header has {columns}. An empty cell is a malformed row rather than an " +
                    "empty value: this parser drops empty cells, so a cleared cell shifts every " +
                    "column after it and yields a row nobody wrote. Write the cell's value, or the " +
                    "word the column uses for nothing.");
            }

            if (cells.Length < 4)
            {
                continue;
            }

            rows.Add(new CompositionRules.Row(
                Unquote(cells[0]),
                cells[1],
                Names(cells[2]),
                Names(cells[3]),
                cells.Length > 4 && !string.Equals(cells[4], "none", StringComparison.Ordinal)
                    ? Names(cells[4])
                    : [],
                cells.Length > 7 ? Unquote(cells[7]) : string.Empty));
        }

        return rows;
    }

    private static string Slug(string composition) =>
        composition.Split('.')[^1].ToLowerInvariant();

    private static string Unquote(string cell) => cell.Trim('`', ' ');

    private static IReadOnlyList<string> Names(string cell) =>
        cell.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Unquote)
            .ToArray();
}
