using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rules N34, N35 and N36: the JavaScript profile's support table, drafted and not issued, is held to
/// the checkout it describes (phase F9 slice R2, decision JSD-0060).
/// </summary>
/// <remarks>
/// <para>
/// <b>N34</b> reads every cell release gate 1 names from where the checkout states it - the core
/// contract versions, the format range, the manifest set, the conformance manifest, the pinned edition
/// and suite, the declared defaults - and holds the table's amendment section to roadmap section 18's
/// rows in both directions, the <c>WebAssembly</c> row to the realm's globals, and every row of an
/// evidenced table to a rule or a retained artifact. <b>N35</b> is the scan JS-10's gate asks for over
/// the extraction-gate state: no verdict, and no identifier of another profile component. <b>N36</b>
/// holds the suppression inventory to a scan of every shipped JavaScript source and project.
/// </para>
/// <para>
/// <b>What none of them does</b>: decide that a row's prose is true. They hold the cells a mechanism can
/// read to the places those cells come from, and the rest of the table to having evidence at all.
/// </para>
/// </remarks>
public sealed class JsSupportTableTests
{
    private const string TableName = "src/Broiler.VM.Profile.JavaScript/docs/support.md";

    private static string Root => ComponentGraph.Root;

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine([Root, .. parts]));

    private static string Table() => Read("src", "Broiler.VM.Profile.JavaScript", "docs", "support.md");

    // -------------------------------------------------------------------------------------
    // N34
    // -------------------------------------------------------------------------------------

    [Fact]
    public void N34_The_Support_Table_States_What_The_Checkout_Declares()
    {
        Assert.Empty(Violations(Table()));
    }

    [Fact]
    public void N34_Rejects_A_Table_Claiming_Another_Core_Contract_Version()
    {
        Assert.Contains(Violations(Witness("N34-table-claims-core-contract-2.md.witness")), violation =>
            violation.Contains("Core contract version implemented", StringComparison.Ordinal));
    }

    [Fact]
    public void N34_Rejects_A_Table_That_Omits_An_Amendment_Row()
    {
        Assert.Contains(Violations(Witness("N34-table-omits-an-amendment-row.md.witness")), violation =>
            violation.Contains("A charging hook for work done inside a host capability", StringComparison.Ordinal));
    }

    [Fact]
    public void N34_Rejects_A_Table_That_Misquotes_A_Default()
    {
        Assert.Contains(Violations(Witness("N34-table-misquotes-a-default.md.witness")), violation =>
            violation.Contains("CallDepth", StringComparison.Ordinal));
    }

    [Fact]
    public void N34_Rejects_A_Row_Without_Evidence_And_A_Bare_Yes()
    {
        var violations = Violations(Witness("N34-table-has-a-bare-yes-and-no-evidence.md.witness"));

        Assert.Contains(violations, violation => violation.Contains("bare", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains("names no rule and no retained artifact", StringComparison.Ordinal));
    }

    // -------------------------------------------------------------------------------------
    // N35
    // -------------------------------------------------------------------------------------

    [Fact]
    public void N35_The_Extraction_Gate_State_Carries_No_Verdict_And_No_Other_Profile()
    {
        Assert.Empty(ExtractionViolations(Table()));
    }

    [Fact]
    public void N35_Rejects_A_Verdict_And_Another_Profiles_Identifier()
    {
        var violations = ExtractionViolations(Witness("N35-section-carries-a-verdict.md.witness"));

        Assert.Contains(violations, violation => violation.Contains("verdict", StringComparison.Ordinal));
        Assert.Contains(violations, violation => violation.Contains("broiler.webassembly", StringComparison.Ordinal));
    }

    // -------------------------------------------------------------------------------------
    // N36
    // -------------------------------------------------------------------------------------

    [Fact]
    public void N36_Every_Suppression_In_A_Shipped_JavaScript_Source_Is_Inventoried()
    {
        var found = ShippedSources().SelectMany(static path => Suppressions(path, File.ReadAllText(path))).ToList();
        Assert.Empty(InventoryViolations(Table(), found));
    }

    [Fact]
    public void N36_Rejects_A_Suppression_The_Table_Does_Not_Inventory()
    {
        var found = Suppressions("src/Broiler.VM.Profile.JavaScript/JsWitness.cs", Witness("N36-source-suppresses-a-warning.cs.witness")).ToList();

        Assert.NotEmpty(found);
        Assert.Contains(InventoryViolations(Table(), found), violation => violation.Contains("JsWitness.cs", StringComparison.Ordinal));
    }

    [Fact]
    public void N34_N35_N36_Hold_Their_Own_Register_Rows()
    {
        foreach (var id in new[] { "N34", "N35", "N36" })
        {
            var row = RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, id, StringComparison.Ordinal));
            Assert.Equal("Active", row.Status);
            Assert.Null(row.ActivationMilestone);
            Assert.Contains(TableName, row.Evidence, StringComparison.Ordinal);
        }
    }

    // -------------------------------------------------------------------------------------
    // N34's reading
    // -------------------------------------------------------------------------------------

    private static List<string> Violations(string table)
    {
        var violations = new List<string>();

        if (!table.Contains("DRAFTED, NOT ISSUED", StringComparison.Ordinal))
        {
            violations.Add("the table does not say it is drafted and not issued");
        }

        var profile = Read("src", "Broiler.VM.Profile.JavaScript", "JavaScriptProfile.cs");
        var core = Read("src", "Broiler.VM.Abstractions", "VmCoreContract.cs");
        var format = Read("src", "Broiler.VM.Profile.JavaScript.Format", "JavaScriptFormat.cs");
        var wide = Read("src", "Broiler.VM.Profile.JavaScript.Format", "JsFormat.cs");
        var numeric = Read("src", "Broiler.VM.Profile.JavaScript.Format", "JsNumericManifest.cs");
        var surfaces = Read("src", "Broiler.VM.Profile.JavaScript.Format", "JsSurfaces.cs");

        void Cell(string field, string expected)
        {
            var row = Row(table, field);

            if (row is null || !row[1].Contains(expected, StringComparison.Ordinal))
            {
                violations.Add($"{TableName}'s row `{field}` does not state {expected}, which the checkout declares");
            }
        }

        Cell("Core contract version implemented", $"**{Match(profile, @"authoredCoreContractVersion: (\d+)")}**");
        Cell("Minimum core contract version accepted", $"**{Match(core, @"MinimumSupportedVersion = (\d+)")}**");
        Cell("Accepted format-version range", $"**{Match(format, @"MinimumFormatVersion = (\d+)")} to {Match(wide, @"public const uint FormatVersion = (\d+)")}**");
        Cell("Conformance manifest", $"`{Match(profile, "VmConformanceManifestId.Create\\(\"([^\"]+)\"\\)")}`, version **{Match(profile, @"conformanceManifestVersion: (\d+)")}**");

        var specification = Read("src", "Broiler.VM.Profile.JavaScript", "docs", "specification", "README.md");
        Cell("Pinned language-specification edition", Match(specification, @"\*\*Revision\*\* \| `([0-9a-f]{40})`"));
        Cell("Pinned language-specification edition", Match(specification, @"\*\*SHA-256\*\* \| `([0-9a-f]{64})`"));

        var suite = Read("src", "tests", "conformance", "pins", "test262.pin");
        Cell("Pinned conformance suite", Match(suite, @"(?m)^revision ([0-9a-f]{40})$"));
        Cell("Pinned conformance suite", Match(suite, @"(?m)^archive-sha256 ([0-9a-f]{64})$"));

        // THE MANIFEST SET, both directions within its row, and a section 3 row for each.
        var declared = new SortedSet<string>(StringComparer.Ordinal)
        {
            Match(wide, "ManifestId = \"(broiler\\.javascript\\.[a-z]+)\""),
            Match(numeric, "ManifestId = \"(broiler\\.javascript\\.[a-z]+)\""),
        };

        foreach (Match manifest in Regex.Matches(profile, "VmFeatureManifestId\\.Parse\\(\"(broiler\\.javascript\\.[a-z]+)\"\\)"))
        {
            declared.Add(manifest.Groups[1].Value);
        }

        foreach (Match surface in Regex.Matches(surfaces, "public const string [A-Za-z]+ = \"(broiler\\.javascript\\.[a-z]+)\""))
        {
            declared.Add(surface.Groups[1].Value);
        }

        Assert.Contains("broiler.javascript.slice", declared);

        var manifestRow = Row(table, "Accepted manifest set");
        var stated = manifestRow is null
            ? new SortedSet<string>(StringComparer.Ordinal)
            : new SortedSet<string>(Regex.Matches(manifestRow[1], @"`(broiler\.javascript\.[a-z]+)`").Select(static m => m.Groups[1].Value), StringComparer.Ordinal);

        foreach (var id in declared.Except(stated))
        {
            violations.Add($"{TableName}'s accepted manifest set omits `{id}`, which the checkout declares");
        }

        foreach (var id in stated.Except(declared))
        {
            violations.Add($"{TableName}'s accepted manifest set names `{id}`, which the checkout does not declare");
        }

        foreach (var id in declared.Where(id => Row(table, $"`{id}`") is null))
        {
            violations.Add($"{TableName} section 3 has no row for `{id}`");
        }

        // THE WEBASSEMBLY ROW, and the realm that makes it true.
        var webAssembly = Row(table, "The `WebAssembly` host-object surface");

        if (webAssembly is null || !webAssembly[1].Contains("Not provided", StringComparison.Ordinal))
        {
            violations.Add($"{TableName} does not name the `WebAssembly` host-object surface as not provided");
        }

        if (Read("src", "Broiler.VM.Profile.JavaScript", "docs", "realm", "globals.txt").Contains("WebAssembly", StringComparison.Ordinal))
        {
            violations.Add("the realm's globals name WebAssembly, and the table says it is not provided");
        }

        // THE AMENDMENT REGISTER, both directions.
        var roadmap = Read("src", "Broiler.VM.Profile.JavaScript", "docs", "roadmap.md");
        var register = FirstCells(Section(roadmap, "## 18."));
        var published = FirstCells(Section(table, "## 5."));

        foreach (var row in register.Except(published))
        {
            violations.Add($"{TableName} section 5 publishes no state for the amendment row `{row}`");
        }

        foreach (var row in published.Except(register))
        {
            violations.Add($"{TableName} section 5 publishes `{row}`, which roadmap section 18 does not carry");
        }

        if (!Section(table, "## 5.").Contains("unexecutable", StringComparison.Ordinal))
        {
            violations.Add($"{TableName} section 5 does not state that the amendment procedure is unexecutable");
        }

        // THE DECLARED DEFAULTS, each against Defaults().
        foreach (var (dimension, value) in Defaults(profile))
        {
            var row = Row(table, $"`{dimension}`");
            var quoted = row is null ? null : Regex.Replace(row[1], "[^0-9]", string.Empty);

            if (quoted != value.ToString(CultureInfo.InvariantCulture))
            {
                violations.Add($"{TableName} quotes {(row is null ? "nothing" : row[1])} for the default `{dimension}`, and Defaults() declares {value}");
            }
        }

        if (!table.Contains("**No runtime identifier is claimed.**", StringComparison.Ordinal))
        {
            violations.Add($"{TableName} does not state that no runtime identifier is claimed");
        }

        if (!table.Contains("## 12. What this table does not say", StringComparison.Ordinal))
        {
            violations.Add($"{TableName} closes with no section stating what it does not say");
        }

        violations.AddRange(EvidenceViolations(table));
        return violations;
    }

    /// <summary>Every row of a table with an Evidence column names a rule or a retained artifact, and no cell is a bare yes.</summary>
    private static IEnumerable<string> EvidenceViolations(string table)
    {
        var rules = RuleRegisterTests.Loaded.Rules.Select(static rule => rule.Id).ToHashSet(StringComparer.Ordinal);
        var asserted = typeof(JsSupportTableTests).Assembly.GetTypes()
            .SelectMany(static type => type.GetMethods())
            .Where(static method => method.GetCustomAttributes(typeof(FactAttribute), false).Length > 0)
            .Select(static method => method.Name.Split('_')[0])
            .ToHashSet(StringComparer.Ordinal);
        var directory = Path.Combine(Root, "src", "Broiler.VM.Profile.JavaScript", "docs");
        var evidence = -1;

        foreach (var line in table.Split('\n'))
        {
            if (!line.StartsWith('|'))
            {
                evidence = -1;
                continue;
            }

            var cells = line.Trim().Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (cells.All(static cell => Regex.IsMatch(cell, "^-+$")))
            {
                continue;
            }

            if (evidence < 0)
            {
                evidence = Array.IndexOf(cells, "Evidence");
                continue;
            }

            foreach (var cell in cells)
            {
                if (Regex.IsMatch(cell.Trim('*'), "^(?i:yes|supported)$"))
                {
                    yield return $"{TableName}'s row `{cells[0]}` has a bare `{cell}` cell";
                }
            }

            if (evidence >= cells.Length)
            {
                continue;
            }

            var cell2 = cells[evidence];
            var linked = Regex.Matches(cell2, @"\]\(([^)#]+)(?:#[^)]*)?\)").Any(m => Path.Exists(Path.GetFullPath(Path.Combine(directory, m.Groups[1].Value))));
            var ruled = Regex.Matches(cell2, @"\b([A-Z][0-9]{1,2}[a-z]?)\b").Any(m => rules.Contains(m.Groups[1].Value) || asserted.Contains(m.Groups[1].Value));

            if (!linked && !ruled)
            {
                yield return $"{TableName}'s row `{cells[0]}` names no rule and no retained artifact in its evidence cell";
            }
        }
    }

    // -------------------------------------------------------------------------------------
    // N35's reading
    // -------------------------------------------------------------------------------------

    private static List<string> ExtractionViolations(string table)
    {
        var section = Section(table, "## 6.");
        var violations = new List<string>();

        if (!section.Contains("first condition", StringComparison.Ordinal) || !section.Contains("What would satisfy it", StringComparison.Ordinal))
        {
            violations.Add($"{TableName} section 6 does not record the first condition's state and what would satisfy it");
        }

        foreach (var word in new[] { "verdict", "approved", "rejected", "extracted", "extract it" })
        {
            if (section.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                violations.Add($"{TableName} section 6 carries a verdict word, `{word}`");
            }
        }

        foreach (Match identifier in Regex.Matches(section, @"\b(broiler\.(?!javascript\b)[a-z][a-z.-]*|Broiler\.VM\.Profile\.(?!JavaScript\b)[A-Za-z.]+|com\.example\.[a-z.]+|WebAssembly|Wasm[A-Za-z]*)\b"))
        {
            violations.Add($"{TableName} section 6 names another profile component's identifier, `{identifier.Value}`");
        }

        return violations;
    }

    // -------------------------------------------------------------------------------------
    // N36's reading
    // -------------------------------------------------------------------------------------

    private static readonly Regex Suppression = new(
        @"#pragma\s+warning\s+disable|\[(?:assembly:\s*)?(?:Unconditional)?SuppressMessage|<NoWarn>|<WarningsNotAsErrors>",
        RegexOptions.Compiled);

    private static IEnumerable<string> ShippedSources()
    {
        var roots = Directory.GetDirectories(Path.Combine(Root, "src"), "Broiler.VM.Profile.JavaScript*")
            .Append(Path.Combine(Root, "src", "Broiler.VM.Profile.MachineCode"))
            .Concat(Directory.GetDirectories(Path.Combine(Root, "src", "compositions"), "Broiler.VM.Composition.JavaScript.*"));

        foreach (var root in roots)
        {
            foreach (var path in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(Root, path).Replace('\\', '/');

                if ((path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal)) &&
                    !relative.Contains("/bin/", StringComparison.Ordinal) && !relative.Contains("/obj/", StringComparison.Ordinal))
                {
                    yield return path;
                }
            }
        }
    }

    private static IEnumerable<string> Suppressions(string path, string text)
    {
        var relative = Path.IsPathRooted(path) ? Path.GetRelativePath(Root, path).Replace('\\', '/') : path;
        var lines = text.Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            if (Suppression.IsMatch(lines[i]))
            {
                yield return $"{relative}:{i + 1}";
            }
        }
    }

    private static List<string> InventoryViolations(string table, IReadOnlyList<string> found)
    {
        var section = Section(table, "## 11.");
        var violations = new List<string>();

        foreach (var site in found)
        {
            var file = site[..site.LastIndexOf(':')];

            if (!section.Contains(file, StringComparison.Ordinal))
            {
                violations.Add($"{site} suppresses a warning that {TableName} section 11 does not inventory");
            }
        }

        if (found.Count == 0 && !Regex.IsMatch(section, @"\|\s*\*\*None\*\*"))
        {
            violations.Add($"{TableName} section 11 does not say that no shipped source suppresses a warning");
        }

        return violations;
    }

    // -------------------------------------------------------------------------------------
    // Reading markdown and C#
    // -------------------------------------------------------------------------------------

    /// <summary>The table row whose first cell is <paramref name="first"/>, as its cells, or nothing.</summary>
    private static string[]? Row(string document, string first)
    {
        foreach (var line in document.Split('\n'))
        {
            if (!line.StartsWith('|'))
            {
                continue;
            }

            var cells = line.Trim().Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (cells.Length > 1 && string.Equals(cells[0], first, StringComparison.Ordinal))
            {
                return cells;
            }
        }

        return null;
    }

    /// <summary>The text from a heading that starts with <paramref name="heading"/> to the next heading of its level.</summary>
    private static string Section(string document, string heading)
    {
        var start = document.IndexOf("\n" + heading, StringComparison.Ordinal);

        if (start < 0)
        {
            return string.Empty;
        }

        var end = document.IndexOf("\n## ", start + 1, StringComparison.Ordinal);
        return end < 0 ? document[start..] : document[start..end];
    }

    /// <summary>The first cells of a section's table rows, header and delimiter rows aside.</summary>
    private static HashSet<string> FirstCells(string section)
    {
        var cells = new HashSet<string>(StringComparer.Ordinal);
        var header = true;

        foreach (var line in section.Split('\n'))
        {
            if (!line.StartsWith('|'))
            {
                header = true;
                continue;
            }

            var first = line.Trim().Trim('|').Split('|')[0].Trim();

            if (header)
            {
                header = false;
                continue;
            }

            if (!Regex.IsMatch(first, "^-+$"))
            {
                cells.Add(first);
            }
        }

        return cells;
    }

    private static string Match(string text, string pattern)
    {
        var match = Regex.Match(text, pattern);
        Assert.True(match.Success, $"the checkout does not declare what the pattern {pattern} reads");
        return match.Groups[1].Value;
    }

    /// <summary>Each dimension Defaults() sets, with its value: a literal, or a product of literals.</summary>
    private static IEnumerable<(string Dimension, BigInteger Value)> Defaults(string profile)
    {
        var body = profile[profile.IndexOf("private static VmLimitVector Defaults()", StringComparison.Ordinal)..];
        body = body[..body.IndexOf("VmLimitVector.TryCreate", StringComparison.Ordinal)];

        foreach (Match row in Regex.Matches(body, @"values\[\(int\)VmBudgetDimension\.(?<dimension>[A-Za-z]+)\] = (?<value>[^;]+);"))
        {
            var value = row.Groups["value"].Value
                .Split('*')
                .Select(static factor => BigInteger.Parse(factor.Trim().TrimEnd('L').Replace("_", string.Empty, StringComparison.Ordinal), CultureInfo.InvariantCulture))
                .Aggregate(BigInteger.One, static (left, right) => left * right);

            yield return (row.Groups["dimension"].Value, value);
        }
    }

    private static string Witness(string fileName)
    {
        var path = Path.Combine(Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "js-support", fileName);
        Assert.True(File.Exists(path), $"Missing witness input {path}.");
        return File.ReadAllText(path);
    }
}
