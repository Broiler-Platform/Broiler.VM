using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group C: the rules about what a pack produces, promoted at VM-6 against a real pack.
/// </summary>
/// <remarks>
/// <para>
/// C1, C2 and C3 were minted at VM-0 with activation milestone VM-6 and were Deferred ever since,
/// for the honest reason that the component ran no pack step: a rule about produced packages
/// cannot be asserted where no package is produced. VM-6 produces them, so the three are asserted
/// here and their register rows move to Active.
/// </para>
/// <para>
/// They read the evidence bundle rather than running a pack. That is the same shape as rules K3
/// and K4, and it is chosen for the same reason: a pack inside a test would be a second build with
/// its own properties, and what a consumer restores is what the retained collection produced.
/// The limit that comes with it is the limit those rules carry - the comparison is against the
/// last collection and not against the working tree - and it is EX-86, restated here as EX-101.
/// </para>
/// <para>
/// The nuspecs are retained by the collection script precisely so C2 and C3 have something to
/// read. A pack transcript says a package was created; it says nothing about what the package
/// promises, and the promise is the part that matters.
/// </para>
/// </remarks>
public sealed class PackageRuleTests
{
    /// <summary>The three package identities, written out rather than derived.</summary>
    /// <remarks>
    /// A list derived from the pack output would agree with any pack output, which is the defect
    /// the whole rule register exists to prevent. ADR 0001's budget section fixes these three and
    /// says a fourth requires a dated revision, so this is that number in the one place a test can
    /// see it.
    /// </remarks>
    private static readonly string[] Packages =
    [
        "Broiler.VM.Abstractions",
        "Broiler.VM.Binary",
        "Broiler.VM.Runtime",
    ];

    /// <summary>
    /// C1's clean direction: the pack produced exactly the three declared packages, with symbols.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Rewritten from equalities into messages on 2026-09-02</b>, so a report can say WHICH
    /// package the pack got wrong rather than only that it got something wrong. Four bundles
    /// recorded C1 as unreportable because it compared two sequences rather than producing a list,
    /// and that was a true description of how it was written rather than of what it claims.
    /// </para>
    /// <para>
    /// <b>The rewrite preserves the rule's strength, and the length clause is why.</b> Two ordered
    /// sequences compared with <c>Assert.Equal</c> disagree when one holds a duplicate; a
    /// membership check over the same two does not. A pack that emitted
    /// <c>Broiler.VM.Runtime.nupkg</c> twice and nothing for <c>Broiler.VM.Binary</c> would have
    /// failed the old assertion, so the count of matched lines is checked as well as their
    /// membership. Losing that would be exactly the quiet weakening this whole reporting exercise
    /// exists to prevent.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> C1Violations()
    {
        var log = Retained("pack.log");

        var produced = Regex.Matches(log, @"^(?<name>[A-Za-z][A-Za-z0-9]*(?:\.[A-Za-z][A-Za-z0-9]*)*)\.(?<version>\d[^\s]*)\.(?<kind>s?nupkg)$",
                RegexOptions.Multiline)
            .Select(match => (Name: match.Groups["name"].Value, Kind: match.Groups["kind"].Value))
            .ToArray();

        // Both kinds, on the same terms. A symbol package for each, and one apiece: a package
        // without symbols is a package a consumer cannot step into, and the pack step is the only
        // place that can be noticed.
        foreach (var kind in new[] { "nupkg", "snupkg" })
        {
            var names = produced
                .Where(entry => string.Equals(entry.Kind, kind, StringComparison.Ordinal))
                .Select(static entry => entry.Name)
                .ToArray();

            foreach (var stray in names
                .Where(static name => !Packages.Contains(name, StringComparer.Ordinal))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static name => name, StringComparer.Ordinal))
            {
                yield return
                    $"the pack produced a {kind} for {stray}, which is not one of the three " +
                    "declared packages";
            }

            foreach (var missing in Packages
                .Where(name => !names.Contains(name, StringComparer.Ordinal))
                .OrderBy(static name => name, StringComparer.Ordinal))
            {
                yield return $"the pack produced no {kind} for {missing}";
            }

            if (names.Length != Packages.Length)
            {
                yield return
                    $"the pack log names {names.Length} {kind} files for {Packages.Length} " +
                    "declared packages";
            }

            // And nothing else packed. Nine test-only projects and two composition roots are in
            // the solution, and every one of them would be a package here if IsPackable had been
            // forgotten. The counts the pack step wrote out are READ rather than recomputed from
            // the names above, so the two halves of this rule can disagree.
            var counted = Count(log, kind + ": ");

            if (counted != Packages.Length)
            {
                yield return
                    $"the pack log counts {counted} {kind}, and {Packages.Length} packages are " +
                    "declared";
            }
        }
    }

    [Fact]
    public void C1_A_Pack_Produces_Exactly_The_Three_Declared_Packages() =>
        Assert.Empty(C1Violations());

    /// <summary>C2's clean direction: no Broiler dependency outside the three packages.</summary>
    /// <remarks>
    /// Takes the dependency ids rather than reading the nuspecs, because the test also asserts a
    /// STRONGER property over the same list - that every dependency is one of the three - and a
    /// function that re-read the file would give the two assertions two readings of it.
    /// </remarks>
    private static IEnumerable<string> C2Violations(IEnumerable<string> dependencies) => dependencies
        .Where(static id => id.StartsWith("Broiler.", StringComparison.Ordinal))
        .Where(static id => !Packages.Contains(id, StringComparer.Ordinal))
        .Select(static id => $"a produced package declares the Broiler dependency {id}");

    /// <summary>The dependency ids the retained nuspecs declare.</summary>
    private static IReadOnlyList<string> NuspecDependencies() => Regex
        .Matches(Retained("nuspecs.txt"), @"<dependency\s+id=""(?<id>[^""]+)""")
        .Select(static match => match.Groups["id"].Value)
        .Distinct(StringComparer.Ordinal)
        .ToArray();

    /// <summary>
    /// Writes what group C's rules said about this checkout, when asked to.
    /// </summary>
    /// <remarks>
    /// <b>All three are here since 2026-09-02.</b> C1 and C3 were excluded while they were written
    /// as equalities and as an absence; they are written as message lists now, and the decision to
    /// restate them is recorded in Bundle JS-ANDROID-012 rather than taken quietly here.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_C_Are_Written_When_Asked_For()
    {
        RuleReport.Write("C",
        [
            ("C1", C1Violations),
            ("C2", () => C2Violations(NuspecDependencies())),
            ("C3", C3Violations),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "C.txt")),
                "a report for group C was asked for and none was written");
        }
    }

    [Fact]
    public void C2_No_Produced_Package_Declares_A_Broiler_Dependency_Outside_The_Three()
    {
        var manifests = Retained("nuspecs.txt");

        var dependencies = Regex.Matches(manifests, @"<dependency\s+id=""(?<id>[^""]+)""")
            .Select(static match => match.Groups["id"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(C2Violations(dependencies));

        // Stronger than the rule states, and it is the property the pristine feed consumer relies
        // on: EVERY declared dependency is one of the three, so nothing outside this component has
        // to be reachable for a restore to succeed. That is what lets the sample clear every
        // package source and add back only a directory holding these packages.
        //
        // The first version of this assertion claimed the packages declare no dependency AT ALL,
        // and that was simply false - Broiler.VM.Runtime depends on Abstractions and Binary, which
        // is the whole shape of the graph. It was written into the support table and the notices
        // too, and this rule is what found it. Worth recording rather than quietly fixing: an
        // untruthful support claim is a stop condition, and the claim was mine.
        Assert.All(dependencies, dependency =>
            Assert.Contains(dependency, Packages, StringComparer.Ordinal));
    }

    /// <summary>
    /// The language names a produced package may not carry.
    /// </summary>
    /// <remarks>
    /// The core ships no language profile, and roadmap section 14 makes "a language capability
    /// implied by package or API" a packaging failure. A package whose description mentioned one
    /// would be implying exactly that to everyone who reads a feed listing.
    /// </remarks>
    private static readonly string[] Languages =
    [
        "javascript", "ecmascript", "typescript", "python", "lua", "ruby", "wasm",
        "webassembly", "java", "c#", "csharp", "php", "perl",
    ];

    /// <summary>
    /// C3's clean direction: no produced package's text names a language.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Rewritten from an absence into messages on 2026-09-02.</b> Four bundles recorded C3 as
    /// unreportable because its clean direction is a search whose success is nothing being found,
    /// "with no message list behind it". That was wrong on its own terms: the list of languages the
    /// search DID find was already computed and then thrown at <c>Assert.Empty</c>, so the messages
    /// existed and were merely never phrased as any. An absence is reportable exactly when
    /// something can be named as present.
    /// </para>
    /// <para>
    /// The empty-text clause is the one that matters more than the languages. A rule that searched
    /// no text would find no language and report silence, and silence from a rule that looked at
    /// nothing is indistinguishable from silence from a rule that looked and was satisfied - which
    /// is the misreading the whole reporting mechanism exists to prevent, so it is a message here
    /// rather than a bare assertion in a test the report cannot see.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> C3Violations()
    {
        var text = string.Join(
            "\n",
            Regex.Matches(
                    Retained("nuspecs.txt"),
                    @"<(?<tag>id|title|description|tags|releaseNotes)>(?<text>[^<]*)</\k<tag>>")
                .Select(static match => match.Groups["text"].Value));

        if (text.Length == 0)
        {
            yield return
                "the retained nuspecs carry no id, title, description, tags or release notes " +
                "text, so this rule searched nothing";

            yield break;
        }

        foreach (var language in Languages
            .Where(language => text.Contains(language, StringComparison.OrdinalIgnoreCase)))
        {
            yield return
                $"a produced package's id, title, description, tags or release notes names " +
                $"the language {language}";
        }
    }

    [Fact]
    public void C3_No_Produced_Package_Names_A_Language_In_Its_Text() =>
        Assert.Empty(C3Violations());

    [Fact]
    public void B3_No_Product_Assembly_Names_A_Broiler_Assembly_Outside_The_Component()
    {
        // Vacuous since VM-0 with activation milestone VM-6, and it stays Vacuous: nothing in the
        // graph can violate it, because ADR 0001's rule A1 stops a Broiler.* PROJECT reference
        // arriving in the first place and rule A2 stops a Broiler.* PACKAGE reference. What VM-6
        // adds is not a way to break it but a place it could have been broken - the packages
        // themselves - and C2 above is that half. The register row keeps saying Vacuous, which is
        // the honest state of a rule whose subject cannot yet be broken.
        foreach (var assembly in AssemblyFacts.Product)
        {
            var stray = assembly.AssemblyReferences
                .Where(static reference => reference.StartsWith("Broiler.", StringComparison.Ordinal))
                .Where(reference => !Packages.Contains(reference, StringComparer.Ordinal))
                .ToArray();

            Assert.Empty(stray);
        }
    }

    /// <summary>
    /// A14's clean direction: every project outside the named solutions is a sample.
    /// </summary>
    /// <remarks>
    /// Extracted so a report can call it, and the test calls it rather than keeping a copy: two
    /// implementations of one rule is the drift a report exists to prevent.
    /// </remarks>
    private static IEnumerable<string> A14Violations() =>
        ComponentGraph.ProjectsOutsideTheSolution
            .Where(static path => !path.StartsWith("samples/", StringComparison.Ordinal));

    /// <summary>Writes what A14 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_A14_Are_Written_When_Asked_For()
    {
        RuleReport.Write("A14", [("A14", A14Violations)]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "A14.txt")),
                "a report for A14 was asked for and none was written");
        }
    }

    [Fact]
    public void A14_Every_Project_Outside_The_Solution_Is_A_Sample()
    {
        // The loophole that narrowing group A to the solutions would otherwise open. The graph
        // rules read the solutions ComponentGraph.SolutionFiles names rather than globbing the
        // tree, so a project file that is in none of them and not under samples/ is a project
        // NOTHING governs - not A1's reference
        // rules, not A5's packability rule, not A7's manifest equality. It would be invisible
        // rather than allowed, which is worse.
        //
        // ADR 0001 revision 4 authorises exactly one place for a project outside the graph, and it
        // authorises it for one reason: a pristine feed consumer cannot be a solution project,
        // because restoring it requires a pack to have already happened.
        Assert.Empty(A14Violations());

        // And samples/ is not an empty exemption someone could hide behind: the consumer that
        // justifies it is really there.
        Assert.Contains(
            "samples/Broiler.VM.Sample.FeedConsumer/Broiler.VM.Sample.FeedConsumer.csproj",
            ComponentGraph.ProjectsOutsideTheSolution,
            StringComparer.Ordinal);

        // NEITHER IS THE VENDORED-COMPONENT EXCLUSION, and this is the half that would rot. A
        // directory named in that list and no longer on disk, or on disk and holding no project,
        // is an exclusion that has stopped excluding anything and would let the next arrival
        // through under a name that no longer means what it meant - which is the shape rule N7's
        // unreachable-code list and the solution list are both written against.
        foreach (var directory in ComponentGraph.VendoredComponents)
        {
            var full = Path.Combine(ComponentGraph.Root, directory.Replace('/', Path.DirectorySeparatorChar));

            Assert.True(
                Directory.Exists(full),
                $"{directory} is excluded as a vendored component and is not in the checkout");

            Assert.NotEmpty(Directory.EnumerateFiles(full, "*.csproj", SearchOption.AllDirectories));
        }
    }

    [Fact]
    public void A14_Every_Sample_Reaches_The_Component_Only_Through_Packages()
    {
        // The other half. A sample is outside the graph so that it can carry a PackageReference;
        // the price of that exemption is that it may carry NOTHING ELSE - a project reference back
        // into the repository would make it a consumer of the build rather than of the packages,
        // and the whole feed claim with it.
        foreach (var relative in ComponentGraph.ProjectsOutsideTheSolution)
        {
            var text = File.ReadAllText(Path.Combine(ComponentGraph.Root, relative));

            Assert.DoesNotContain("<ProjectReference", text, StringComparison.Ordinal);

            var referenced = Regex.Matches(text, @"<PackageReference\s+Include=""(?<id>[^""]+)""")
                .Select(static match => match.Groups["id"].Value)
                .Where(static id => id.StartsWith("Broiler.", StringComparison.Ordinal))
                .OrderBy(static id => id, StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(Packages.OrderBy(static name => name, StringComparer.Ordinal), referenced);
        }
    }

    [Fact]
    public void The_Pack_Rules_Hold_Their_Own_Register_Rows_To_What_They_Prove()
    {
        foreach (var id in new[] { "C1", "C2", "C3" })
        {
            var row = RuleRegisterTests.Loaded.Rules.Single(
                rule => string.Equals(rule.Id, id, StringComparison.Ordinal));

            Assert.Equal("Active", row.Status);

            // A promoted rule may not still name its activation milestone: a row that is Active
            // and says "activates at VM-6" is a row nobody finished promoting.
            Assert.Null(row.ActivationMilestone);
        }

        var vacuous = RuleRegisterTests.Loaded.Rules.Single(
            rule => string.Equals(rule.Id, "B3", StringComparison.Ordinal));

        Assert.Equal("Vacuous", vacuous.Status);
    }

    private static int Count(string log, string prefix) =>
        int.TryParse(
            log.Split('\n')
                .First(line => line.Trim().StartsWith(prefix, StringComparison.Ordinal))
                .Trim()[prefix.Length..],
            out var count)
            ? count
            : -1;

    private static string Retained(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "docs", "evidence",
            ComponentGraph.CurrentEvidenceDirectory, fileName);

        Assert.True(File.Exists(path), $"The current evidence bundle retains no {fileName} at {path}.");
        return File.ReadAllText(path);
    }
}
