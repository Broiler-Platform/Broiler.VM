using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group H: the rules that bind the human-review record to the things it quotes.
/// </summary>
/// <remarks>
/// <para>
/// The review documents carry marks, identifiers and figures, and every one of them is a claim
/// about something else in the checkout. Unbound they rot: this component has already found a
/// transposed byte figure and two stale test counts in its own review record, and a review record
/// that has gone stale is worse than none, because it is read as current.
/// </para>
/// <para>
/// Every rule here asserts in both directions. The real documents must pass, and every clause must
/// be shown rejecting an input that breaks it and nothing else - the witness inputs under
/// witnesses/review/. The witnesses are per CLAUSE, not per rule, and each assertion names the
/// content of the violation it expects rather than merely checking that the list is non-empty. A
/// witness asserted with a bare non-empty check pins whichever clause happens to fire first, which
/// is how an earlier attempt at this group lost four independent clauses in one patch while the
/// suite stayed green.
/// </para>
/// <para>
/// Every rule here also holds its own register row to the limits it depends on. Nothing else in the
/// suite reads the <c>statement</c>, <c>evidence</c> or <c>nonVacuousWhen</c> fields, so without
/// this a row could be rewritten into an over-claim - a rule weaker than its own statement, which
/// is the standing defect the register exists to prevent - in a single edit with the suite green.
/// </para>
/// <para>
/// ADR 0012 is the nominal owner: these rules protect the support-claim and ownership surface it
/// governs. It is frozen and names none of them, which Exclusion EX-53 records rather than
/// implying a binding that does not exist.
/// </para>
/// </remarks>
public sealed class ReviewRecordRuleTests
{
    // -------------------------------------------------------------------------------------
    // The review-document set, defined once, in one place.
    // -------------------------------------------------------------------------------------

    /// <summary>
    /// The documents a reviewer reads: the generated review record, the evidence bundles, and the
    /// status ledgers - the component's own, and every profile family's. Every group H rule reads
    /// this one set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>docs/review/</c> is still enumerated although nothing is in it any more. The per-item
    /// worksheet that lived there is deleted - it cited source files by line number, and a
    /// reviewer now writes on the declaration instead - and leaving the directory in the corpus
    /// costs nothing and means a document dropped there later is read rather than unread.
    /// </para>
    /// <para>
    /// <b>A profile family's documents are in this set, and were not until JS-3a.</b> A profile
    /// lives inside this component (decision JSD-0001) and adopts its assurance and review system
    /// rather than standing up one of its own (JSD-0006) - but its ledger and its evidence bundles
    /// live under <c>src/Broiler.VM.Profile.*/docs/</c>, which this loader did not look at. So the
    /// clauses that exist because a reviewer reads these documents - no citation of a source line
    /// number, a closed mark vocabulary, every cited exclusion defined - reached the component's
    /// own ledger and not the ledger a profile reviewer actually opens. The families are
    /// discovered rather than listed, so a second profile is covered on the day its docs directory
    /// exists.
    /// </para>
    /// </remarks>
    private static IReadOnlyList<ReviewDocument> Corpus { get; } = LoadCorpus();

    /// <summary>How many review documents this component carries.</summary>
    /// <remarks>
    /// The COUNT is internal rather than the corpus, because ReviewDocument is a private nested
    /// type and widening it would export the review model to publish one number. Rule J12's
    /// figure catalog cites this: rule H1's row used to state it as "the four review documents"
    /// and to name "the two evidence-bundle READMEs", both of which stopped being true many
    /// bundles ago.
    /// </remarks>
    internal static int CorpusCount => Corpus.Count;

    private const string HumanReviewName = "HUMAN_REVIEW.md";

    /// <summary>
    /// The bundle whose logs the current milestone's figures are quoted from.
    /// </summary>
    /// <remarks>
    /// Read from the rule register's own milestone rather than written down here. A literal went
    /// stale twice: at VM-2 and again at VM-3 the register advanced, the current bundle changed,
    /// and this constant went on naming the previous one - so every document was compared against
    /// a superseded bundle's figures and the anti-deletion guards demanded the superseded
    /// bundle's numbers be kept current. That is the same defect the deferred-rule test records
    /// for its own hardcoded milestone, and it has the same fix.
    /// </remarks>
    private static string CurrentBundle => ComponentGraph.CurrentEvidenceDirectory;

    private static ReviewDocument HumanReview => Document(HumanReviewName);

    private static IEnumerable<ReviewDocument> EvidenceBundles =>
        Corpus.Where(static document => BundleReadme.IsMatch(document.Name));

    private static readonly Regex BundleReadme =
        new(@"^docs/evidence/(?<bundle>[^/]+)/README\.md$", RegexOptions.Compiled);

    private static ReviewDocument Document(string name) =>
        Corpus.SingleOrDefault(document => string.Equals(document.Name, name, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"The review-document set does not contain {name}.");

    private static IReadOnlyList<ReviewDocument> LoadCorpus()
    {
        var paths = new List<string> { Path.Combine(ComponentGraph.Root, "HUMAN_REVIEW.md") };

        var reviewDirectory = Path.Combine(ComponentGraph.Root, "docs", "review");

        if (Directory.Exists(reviewDirectory))
        {
            paths.AddRange(Directory.EnumerateFiles(reviewDirectory, "*.md", SearchOption.TopDirectoryOnly));
        }

        var evidenceDirectory = Path.Combine(ComponentGraph.Root, "docs", "evidence");

        if (Directory.Exists(evidenceDirectory))
        {
            paths.AddRange(Directory
                .EnumerateDirectories(evidenceDirectory)
                .Select(static bundle => Path.Combine(bundle, "README.md")));
        }

        paths.Add(Path.Combine(ComponentGraph.Root, "docs", "roadmap.status.md"));
        paths.AddRange(ProfileReviewDocuments());

        return paths
            .Where(File.Exists)
            .Select(static path => new ReviewDocument(
                Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'),
                File.ReadAllText(path)))
            .OrderBy(static document => document.Name, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>The docs directory of every profile family in this component, if it has one.</summary>
    internal static IEnumerable<string> ProfileDocDirectories()
    {
        var source = Path.Combine(ComponentGraph.Root, "src");

        if (!Directory.Exists(source))
        {
            yield break;
        }

        foreach (var project in Directory
            .EnumerateDirectories(source, "Broiler.VM.Profile.*", SearchOption.TopDirectoryOnly)
            .OrderBy(static path => path, StringComparer.Ordinal))
        {
            var docs = Path.Combine(project, "docs");

            if (Directory.Exists(docs))
            {
                yield return docs;
            }
        }
    }

    /// <summary>A profile family's own review documents: its status ledger and its bundles.</summary>
    private static IEnumerable<string> ProfileReviewDocuments()
    {
        foreach (var docs in ProfileDocDirectories())
        {
            yield return Path.Combine(docs, "roadmap.status.md");

            var evidence = Path.Combine(docs, "evidence");

            if (!Directory.Exists(evidence))
            {
                continue;
            }

            foreach (var bundle in Directory
                .EnumerateDirectories(evidence)
                .OrderBy(static path => path, StringComparer.Ordinal))
            {
                yield return Path.Combine(bundle, "README.md");
            }
        }
    }

    /// <summary>
    /// A witness input under witnesses/review/, read as though it were a review document. Several
    /// are fragments rather than whole documents, because the rules they witness read fragments.
    /// </summary>
    private static ReviewDocument Witness(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "review", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return new ReviewDocument(fileName, File.ReadAllText(path));
    }

    private sealed class ReviewDocument(string name, string text)
    {
        public string Name { get; } = name;

        public string Text { get; } = text;

        /// <summary>The document read once, as markdown, and cached for every rule that wants it.</summary>
        public IReadOnlyList<SourceLine> Lines { get; } = ReadLines(text);
    }

    // =====================================================================================
    // H1 - the mark vocabulary is closed
    // =====================================================================================

    /// <summary>
    /// H1. Every mark token in a review document is one of the nine the legend publishes, and the
    /// legend publishes exactly those nine.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The nine are hard-coded here and are deliberately NOT read from the document. Reading them
    /// from the legend would make the rule agree with any legend at all, including one that had
    /// grown a tenth mark, and the legend is the thing every other review document links to
    /// instead of repeating.
    /// </para>
    /// <para>
    /// The token pattern carries no length cap. An earlier attempt capped the inner text at six
    /// characters, which let through exactly the words an author reaches for - <c>[APPROVED]</c>,
    /// <c>[REJECTED]</c>, <c>[NOT MET]</c>, <c>[PENDING]</c> - while reporting a clean sweep.
    /// </para>
    /// <para>
    /// A table row is read whether or not it carries the optional leading pipe and whether or not
    /// it carries the optional trailing pipe. GFM makes both optional, and a scan gated on the
    /// leading pipe skipped a whole row - the same defect as the trailing-pipe one, one pipe over.
    /// Marks inside a block quote are read too: HUMAN_REVIEW.md opens with a quoted status banner,
    /// so a quoted list item, table row or heading is the document's own idiom and not an exotic
    /// shape.
    /// </para>
    /// </remarks>
    /// <summary>
    /// Writes what each group H rule said about this checkout, when asked to.
    /// </summary>
    /// <remarks>
    /// The mechanism lives on <see cref="RuleReport"/>. Each entry calls the SAME helper its test
    /// calls, with the same inputs - not a re-implementation, which is the only way a report and
    /// the rule it reports on cannot drift apart.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_H_Are_Written_When_Asked_For()
    {
        var current = Figures(CurrentBundle);
        var sourced = Corpus.Where(HasARetainedFigureSource).ToArray();

        RuleReport.Write("H",
        [
            ("H1", () => UnpublishedMarkViolations(Corpus)
                .Concat(LegendViolations(HumanReview, CoreLegend))),
            ("H2", () => UndefinedCitationViolations(Corpus, ExclusionDefinitions(EvidenceBundles))
                .Concat(ExclusionSectionViolations(EvidenceBundles))),
            ("H3", () => Corpus.SelectMany(SourceLineCitations)),
            ("H4", () => StatusViolations(HumanReview, AssuranceScanner.Units)
                .Concat(AliasViolations(HumanReview, AssuranceScanner.Units))),
            ("H5", () => sourced
                .SelectMany(document => FigureViolations(document, AcceptableFigures(document)))
                .Concat(RetainedFigureGuard(sourced, current))),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "H.txt")),
                "a report for group H was asked for and none was written");
        }
    }

    [Fact]
    public void H1_The_Mark_Vocabulary_Is_Closed()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H1",
            "with or without the optional leading pipe",
            "GFM task-list checkbox",
            "each family is held to its own legend",
            "only a table under a Mark/Meaning header publishes");

        Assert.Empty(UnpublishedMarkViolations(Corpus));
        Assert.Empty(LegendViolations(HumanReview, CoreLegend));

        // Every profile family publishes its own legend and is held to it. Non-vacuous: there is
        // at least one such ledger in this checkout, and it publishes a different vocabulary from
        // the component's - so the resolution below decides something.
        Assert.NotEmpty(ProfileLedgers);
        Assert.All(ProfileLedgers, static ledger => Assert.Empty(LegendViolations(ledger, ProfileLegend)));

        // Clause: the split is not a no-op. The SAME document is clean under the legend that
        // governs it and a wall of violations under the other one, in both directions, and the
        // two vocabularies share no member.
        var profileLedger = ProfileLedgers.First();

        Assert.Empty(UnpublishedMarkViolations([profileLedger], ProfileLegend));
        Assert.NotEmpty(UnpublishedMarkViolations([profileLedger], CoreLegend));
        Assert.NotEmpty(UnpublishedMarkViolations([HumanReview], ProfileLegend));
        Assert.Empty(CoreLegend.Marks.Intersect(ProfileLegend.Marks, StringComparer.Ordinal));

        // Clause: the resolution is by path, and it answers for both families.
        Assert.Equal(ProfileLegend, LegendFor(profileLedger.Name));
        Assert.Equal(CoreLegend, LegendFor(HumanReviewName));
        Assert.Equal(CoreLegend, LegendFor("docs/evidence/vm-6/README.md"));

        // Clause: a profile document using a mark from the OTHER family's legend is a violation,
        // which is what stops the two vocabularies leaking into each other.
        var borrowed = UnpublishedMarkViolations(
            [Witness("H1-profile-document-uses-a-core-mark.md.witness")], ProfileLegend);

        Assert.Equal(2, borrowed.Count);
        Assert.Single(borrowed.Where(static violation =>
            violation.Contains("table cell mark token [MET]", StringComparison.Ordinal)));
        Assert.Single(borrowed.Where(static violation =>
            violation.Contains("list item mark token [UNMET]", StringComparison.Ordinal)));
        Assert.All(borrowed, static violation => Assert.Contains(
            "which the section 2 legend does not publish", violation, StringComparison.Ordinal));

        // Clause: a profile legend is held to its three the same way the component's is held to
        // its nine, in both directions.
        var profileLegend = LegendViolations(
            Witness("H1-profile-legend-does-not-publish-the-three.md.witness"), ProfileLegend);

        Assert.Equal(2, profileLegend.Count);
        Assert.Single(profileLegend.Where(static violation =>
            violation.Contains("publishes [SOME]", StringComparison.Ordinal)));
        Assert.Single(profileLegend.Where(static violation =>
            violation.Contains("does not publish [PARTIAL]", StringComparison.Ordinal)));

        // Clause: a table cell whose whole trimmed text is a mark token. The token is eight
        // characters inside the brackets, so it also carries the no-length-cap requirement.
        var cell = Assert.Single(
            UnpublishedMarkViolations([Witness("H1-table-cell-unknown-mark.md.witness")], CoreLegend));
        Assert.Contains("H1-table-cell-unknown-mark.md.witness", cell, StringComparison.Ordinal);
        Assert.Contains("table cell mark token [APPROVED]", cell, StringComparison.Ordinal);

        // Clause: the trailing pipe is optional in GFM, so a row that omits it is still a row.
        var loose = Assert.Single(UnpublishedMarkViolations([Witness("H1-table-row-without-trailing-pipe.md.witness")], CoreLegend));
        Assert.Contains("table cell mark token [REJECTED]", loose, StringComparison.Ordinal);

        // Clause: the LEADING pipe is optional too. A row that omits it is a real row of the table
        // it sits in, and gating the scan on the leading pipe skipped it whole.
        var headless = Assert.Single(UnpublishedMarkViolations([Witness("H1-table-row-without-a-leading-pipe.md.witness")], CoreLegend));
        Assert.Contains("table cell mark token [APPROVED]", headless, StringComparison.Ordinal);

        // Clause: a bracketed token leading a list item, which is the form section 5 uses.
        var item = Assert.Single(UnpublishedMarkViolations([Witness("H1-list-item-unknown-mark.md.witness")], CoreLegend));
        Assert.Contains("list item mark token [NOT MET]", item, StringComparison.Ordinal);

        // Clause: the final token of an ATX heading.
        var heading = Assert.Single(UnpublishedMarkViolations([Witness("H1-heading-unknown-mark.md.witness")], CoreLegend));
        Assert.Contains("heading mark token [PENDING]", heading, StringComparison.Ordinal);

        // Clause: a heading mark written in backticks, which the heading branch has to unquote.
        var quoted = Assert.Single(UnpublishedMarkViolations([Witness("H1-heading-mark-in-backticks.md.witness")], CoreLegend));
        Assert.Contains("heading mark token [APPROVED]", quoted, StringComparison.Ordinal);

        // Clause: block-quote markers are stripped before a line is classified, so a quoted list
        // item, table row and heading are each still what they are.
        var quotedBlock = UnpublishedMarkViolations(
            [Witness("H1-marks-inside-a-block-quote.md.witness")], CoreLegend);
        Assert.Equal(3, quotedBlock.Count);
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("list item mark token [APPROVED]", StringComparison.Ordinal)));
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("heading mark token [WAIVED]", StringComparison.Ordinal)));
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("table cell mark token [REJECTED]", StringComparison.Ordinal)));

        // Clause: the legend itself, parsed independently of the body scan, in both directions.
        var legend = LegendViolations(Witness("H1-legend-does-not-publish-the-nine.md.witness"), CoreLegend);
        Assert.Equal(2, legend.Count);
        Assert.Single(legend.Where(static violation =>
            violation.Contains("publishes [WAIVED]", StringComparison.Ordinal)));
        Assert.Single(legend.Where(static violation =>
            violation.Contains("does not publish [?]", StringComparison.Ordinal)));

        // Clause: the legend scan reads a row that omits the leading pipe, so a tenth mark cannot
        // be published past the equality check by dropping one character.
        var legendRow = Assert.Single(
            LegendViolations(Witness("H1-legend-row-without-a-leading-pipe.md.witness"), CoreLegend));
        Assert.Contains("publishes [WAIVED]", legendRow, StringComparison.Ordinal);

        // Clause: the legend publishes each of the nine once.
        var twice = Assert.Single(
            LegendViolations(Witness("H1-legend-publishes-a-mark-twice.md.witness"), CoreLegend));
        Assert.Contains("publishes [MET] more than once", twice, StringComparison.Ordinal);

        // Clause: exactly one legend section. Section() returning its first match with no
        // uniqueness check let a decoy heading above the real one become the section under test.
        var twoLegends = Assert.Single(
            LegendViolations(Witness("H1-legend-section-appears-twice.md.witness"), CoreLegend));
        Assert.Contains("carries 2 'section 1 legend' headings", twoLegends, StringComparison.Ordinal);
    }

    /// <summary>
    /// One mark legend: the document that publishes it, the section it lives in, and the marks it
    /// must publish.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>There is more than one, and that is the point.</b> A mark is only readable against the
    /// legend a reader of THAT document would look up, and this component holds two document
    /// families with two vocabularies. The component's own review record publishes nine marks over
    /// evidence verdicts and review verdicts. A profile's status ledger publishes three - its
    /// evidence verdict over a milestone row - and says in its own words that the vocabulary is
    /// closed and has three members.
    /// </para>
    /// <para>
    /// Folding them into one set of twelve would be the obvious repair and it is the wrong one: it
    /// would admit <c>[FULL]</c> into the component's review record, where nothing defines it, and
    /// <c>[MET]</c> into a profile ledger whose own section says it has three marks. Each family
    /// is held to the legend that governs it, and a mark that is legal in one is a violation in
    /// the other.
    /// </para>
    /// </remarks>
    private sealed record MarkLegend(string Publisher, Regex Section, string Label, string[] Marks);

    /// <summary>The component's own legend: HUMAN_REVIEW.md section 1, nine marks.</summary>
    private static readonly MarkLegend CoreLegend = new(
        HumanReviewName,
        new Regex(@"^##\s+1\.\s", RegexOptions.Compiled),
        "section 1 legend",
        ["[MET]", "[PART]", "[UNMET]", "[N/A]", "[ ]", "[A]", "[C]", "[R]", "[?]"]);

    /// <summary>A profile family's legend: its status ledger's section 2, three marks.</summary>
    private static readonly MarkLegend ProfileLegend = new(
        "roadmap.status.md",
        new Regex(@"^##\s+2\.\s", RegexOptions.Compiled),
        "section 2 legend",
        ["[NONE]", "[PARTIAL]", "[FULL]"]);

    /// <summary>
    /// The legend that governs a document, decided by where the document lives.
    /// </summary>
    /// <remarks>
    /// By path and not by content, deliberately. Deciding from what a document contains would let
    /// a document choose its own vocabulary by using it, which is the check inverted.
    /// </remarks>
    private static MarkLegend LegendFor(string documentName) =>
        documentName.StartsWith("src/Broiler.VM.Profile.", StringComparison.Ordinal)
            ? ProfileLegend
            : CoreLegend;

    /// <summary>Every profile status ledger in the checkout, each of which publishes a legend.</summary>
    private static IEnumerable<ReviewDocument> ProfileLedgers => Corpus
        .Where(static document =>
            document.Name.StartsWith("src/Broiler.VM.Profile.", StringComparison.Ordinal) &&
            document.Name.EndsWith("/roadmap.status.md", StringComparison.Ordinal));

    /// <summary>
    /// A mark token is a bracketed run with no closing bracket and no line break inside it. No
    /// length cap: membership decides, not shape.
    /// </summary>
    private static readonly Regex MarkToken = new(@"^\[[^\]\r\n]*\]$", RegexOptions.Compiled);

    /// <summary>
    /// A list item led by a bracketed token, with or without surrounding backticks. The trailing
    /// lookahead is what keeps a markdown link out: <c>[text](url)</c> and <c>[text][ref]</c> are
    /// followed by <c>(</c> or <c>[</c> and are not marks.
    /// </summary>
    private static readonly Regex ListItemMark = new(
        @"^(?:[-*+]|\d+[.)])\s+(?<tick>`?)(?<token>\[[^\]\r\n]*\])\k<tick>(?=$|\s|[,.;:!?)])",
        RegexOptions.Compiled);

    /// <summary>Every mark in the corpus, each judged against the legend that governs its file.</summary>
    private static List<string> UnpublishedMarkViolations(IEnumerable<ReviewDocument> corpus) =>
        corpus.SelectMany(document => UnpublishedMarkViolations([document], LegendFor(document.Name)))
            .ToList();

    /// <summary>
    /// The same scan against one named legend, which is how a witness input is judged.
    /// </summary>
    /// <remarks>
    /// A witness sits under <c>witnesses/review/</c> and would resolve to the component's own
    /// legend by path whatever it is witnessing, so the legend is a parameter here. It is also
    /// what lets one witness be shown legal under one legend and a violation under the other,
    /// which is the clause that keeps the split from being a no-op.
    /// </remarks>
    private static List<string> UnpublishedMarkViolations(
        IEnumerable<ReviewDocument> corpus, MarkLegend legend)
    {
        var published = legend.Marks.ToHashSet(StringComparer.Ordinal);

        return corpus
            .SelectMany(static document => MarkTokens(document.Lines)
                .Select(mark => (document.Name, Mark: mark)))
            .Where(found => !published.Contains(found.Mark.Token))
            .Select(found =>
                $"{found.Name}:{found.Mark.Line} uses {found.Mark.Kind} mark token {found.Mark.Token}, " +
                $"which the {legend.Label} does not publish")
            .ToList();
    }

    private static IEnumerable<MarkOccurrence> MarkTokens(IEnumerable<SourceLine> lines)
    {
        foreach (var line in lines)
        {
            if (line.IsTableRow)
            {
                foreach (var cell in TableCells(line.Text))
                {
                    var token = Unquote(cell);

                    if (MarkToken.IsMatch(token))
                    {
                        yield return new MarkOccurrence("table cell", line.Number, token);
                    }
                }

                continue;
            }

            if (HashCount(line.Text) > 0)
            {
                var heading = line.Text.TrimStart('#').Trim().TrimEnd('#').Trim();
                var last = heading
                    .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault();

                if (last is not null && MarkToken.IsMatch(Unquote(last)))
                {
                    yield return new MarkOccurrence("heading", line.Number, Unquote(last));
                }

                continue;
            }

            var listItem = ListItemMark.Match(line.Text);

            if (!listItem.Success)
            {
                continue;
            }

            var leading = listItem.Groups["token"].Value;

            // A GFM task-list checkbox is not a mark. HUMAN_REVIEW.md section 1 says so in prose:
            // section 9's four-state decision list is "written as ordinary markdown checkboxes"
            // and deliberately does not use this vocabulary. Only the three checkbox spellings are
            // exempt, so [APPROVED] leading a list item is still a mark and still a violation.
            if (listItem.Groups["tick"].Value.Length == 0 &&
                leading is "[ ]" or "[x]" or "[X]")
            {
                continue;
            }

            yield return new MarkOccurrence("list item", line.Number, leading);
        }
    }

    /// <summary>
    /// Reads the legend rows out of HUMAN_REVIEW.md section 1 independently of the body scan, so
    /// that a mark added to the legend is visible to the equality check. Sharing the body scanner
    /// is what made an earlier attempt's legend check unable to see a tenth mark.
    /// </summary>
    /// <summary>The header a legend table opens with. Only a table under it publishes marks.</summary>
    /// <remarks>
    /// <b>Only the legend TABLE publishes, and the section is not enough on its own.</b> An
    /// earlier revision took any table row in the legend section whose first cell was mark-shaped,
    /// which was true of the component's section 1 by accident - it contains nothing but legend
    /// tables - and false the moment a profile ledger arrived, whose section 2 carries the legend
    /// and the milestone table together. Under the old reading every <c>[NONE]</c> in the status
    /// table counted as a legend row, so a ledger with nine of them published <c>[NONE]</c> nine
    /// times. The header is what distinguishes a legend from a table that happens to be beside
    /// one, and rule H2 already draws exactly this distinction for the exclusion table.
    /// </remarks>
    private static readonly string[] LegendHeader = ["Mark", "Meaning"];

    private static List<string> LegendMarks(
        ReviewDocument document, MarkLegend legend, List<string> violations)
    {
        var section = Section(document, legend.Section, legend.Label, violations);
        var marks = new List<string>();
        var inLegendTable = false;

        foreach (var line in section)
        {
            if (!line.IsTableRow)
            {
                inLegendTable = false;
                continue;
            }

            var cells = TableCells(line.Text).Select(Unquote).ToArray();

            if (cells.Length == LegendHeader.Length &&
                cells.SequenceEqual(LegendHeader, StringComparer.OrdinalIgnoreCase))
            {
                inLegendTable = true;
                continue;
            }

            if (!inLegendTable)
            {
                continue;
            }

            var first = cells.FirstOrDefault() ?? string.Empty;

            // The delimiter row under the header is part of the table and publishes nothing.
            if (first.Length > 0 && first.Trim('-', ':', ' ').Length == 0)
            {
                continue;
            }

            if (MarkToken.IsMatch(first))
            {
                marks.Add(first);
            }
        }

        return marks;
    }

    /// <summary>
    /// A published legend held to the marks it must carry, in both directions and without
    /// repetition.
    /// </summary>
    private static List<string> LegendViolations(ReviewDocument document, MarkLegend legend)
    {
        var violations = new List<string>();
        var published = LegendMarks(document, legend, violations);

        violations.AddRange(published
            .Distinct(StringComparer.Ordinal)
            .Where(token => !legend.Marks.Contains(token, StringComparer.Ordinal))
            .Select(token =>
                $"{document.Name} legend publishes {token}, which is not one of the " +
                $"{legend.Marks.Length}"));

        violations.AddRange(legend.Marks
            .Where(token => !published.Contains(token, StringComparer.Ordinal))
            .Select(token => $"{document.Name} legend does not publish {token}"));

        violations.AddRange(published
            .GroupBy(static token => token, StringComparer.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(group => $"{document.Name} legend publishes {group.Key} more than once"));

        return violations;
    }

    private sealed record MarkOccurrence(string Kind, int Line, string Token);

    // =====================================================================================
    // H2 - every exclusion cited is defined
    // =====================================================================================

    /// <summary>
    /// H2. Every <c>EX-</c> identifier cited in a review document is defined by a row of the
    /// section 9 exclusion table of an evidence bundle, or by a bolded list item in that section.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the exclusion table defines, and only in section 9. A bundle also carries a gate
    /// table, an errata table, a negative-control table and a hash table, and an earlier attempt
    /// accepted any table row in a bundle whose first cell was an <c>EX-</c> identifier - so any
    /// of those tables could launder an undefined citation into a definition. Section 9 is the
    /// LAST section of a bundle, so "inside section 9" alone is not enough either: any subsection
    /// appended to the end of the file is inside it, and a worked example in a fenced code block
    /// would otherwise mint definitions for the identifiers it illustrates.
    /// </para>
    /// <para>
    /// The citation pattern accepts one digit as well as two, and does not require a word boundary
    /// in front of the <c>E</c>: <c>_EX-04_</c> is a citation an author writes, and a leading
    /// underscore is a word character, so <c>\bEX-\d+\b</c> could not see it at all. <c>EX-4</c>
    /// written for <c>EX-04</c> is the commonest way this citation goes wrong, and a
    /// two-or-more-digit pattern ignores it silently, which is worse than not checking at all.
    /// </para>
    /// </remarks>
    [Fact]
    public void H2_Every_Exclusion_Cited_Is_Defined()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H2",
            "exclusion table",
            "fenced code block",
            "subsection",
            "a profile bundle is a different document shape");

        var defined = ExclusionDefinitions(EvidenceBundles);

        Assert.NotEmpty(defined);
        Assert.Empty(UndefinedCitationViolations(Corpus, defined));
        Assert.Empty(ExclusionSectionViolations(EvidenceBundles));

        // Clause: a citation with no definition anywhere is reported, with its file and its id.
        var undefined = Witness("H2-cited-exclusion-is-undefined.md.witness");
        var missing = Assert.Single(UndefinedCitationViolations([undefined], ExclusionDefinitions([undefined])));
        Assert.Contains("H2-cited-exclusion-is-undefined.md.witness", missing, StringComparison.Ordinal);
        Assert.Contains("EX-99", missing, StringComparison.Ordinal);

        // Clause: a one-digit citation is read, not skipped.
        var single = Witness("H2-single-digit-citation.md.witness");
        var shortForm = Assert.Single(UndefinedCitationViolations([single], ExclusionDefinitions([single])));
        Assert.Contains("EX-4,", shortForm, StringComparison.Ordinal);

        // Clause: an italicised citation is read. The underscore is a word character, so a
        // \b-anchored pattern does not merely mis-parse _EX-95_, it never sees it.
        var italic = Witness("H2-citation-inside-italics.md.witness");
        var emphasised = Assert.Single(UndefinedCitationViolations([italic], ExclusionDefinitions([italic])));
        Assert.Contains("EX-95,", emphasised, StringComparison.Ordinal);

        // Clause: only rows inside section 9 define. EX-77 sits in a gate table and is cited.
        var elsewhere = Witness("H2-definition-outside-the-exclusion-table.md.witness");
        var laundered = Assert.Single(UndefinedCitationViolations([elsewhere], ExclusionDefinitions([elsewhere])));
        Assert.Contains("EX-77", laundered, StringComparison.Ordinal);

        // Clause: inside section 9, only the EXCLUSION table defines. EX-98 sits in an errata
        // table whose header is not the exclusion table's, and is cited.
        var errata = Witness("H2-definition-in-another-section-9-table.md.witness");
        var corrected = Assert.Single(UndefinedCitationViolations([errata], ExclusionDefinitions([errata])));
        Assert.Contains("EX-98", corrected, StringComparison.Ordinal);

        // Clause: a subsection of section 9 does not define, however exclusion-shaped its table.
        // Section 9 is the last section of a bundle, so everything appended lands inside it.
        var appended = Witness("H2-definition-in-a-section-9-subsection.md.witness");
        var subsection = Assert.Single(UndefinedCitationViolations([appended], ExclusionDefinitions([appended])));
        Assert.Contains("EX-97", subsection, StringComparison.Ordinal);

        // Clause: a fenced code block does not define. A worked example showing how to add an
        // exclusion would otherwise mint the identifier it illustrates.
        var fenced = Witness("H2-definition-inside-a-code-fence.md.witness");
        var example = Assert.Single(UndefinedCitationViolations([fenced], ExclusionDefinitions([fenced])));
        Assert.Contains("EX-96", example, StringComparison.Ordinal);

        // Clause: the list-item form inside section 9 defines too. EX-61 is a list item and EX-62
        // a table row; only EX-63, which is defined nowhere, may be reported.
        var listed = Witness("H2-exclusion-defined-as-a-list-item.md.witness");
        var definitions = ExclusionDefinitions([listed]);
        Assert.Contains("EX-61", definitions);
        Assert.Contains("EX-62", definitions);
        var undefinedOnly = Assert.Single(UndefinedCitationViolations([listed], definitions));
        Assert.Contains("EX-63", undefinedOnly, StringComparison.Ordinal);

        // Clause: exactly one exclusion section per bundle.
        var twice = Assert.Single(ExclusionSectionViolations([Witness("H2-exclusion-section-appears-twice.md.witness")]));
        Assert.Contains("carries 2 'section 9 exclusions' headings", twice, StringComparison.Ordinal);
    }

    /// <summary>
    /// A cited exclusion identifier. There is deliberately no <c>\b</c> in front of the
    /// <c>E</c>: markdown emphasis wraps a citation in underscores, and an underscore is a word
    /// character, so the anchored form matched nothing at all on <c>_EX-04_</c>.
    /// </summary>
    private static readonly Regex ExclusionCitation =
        new(@"(?<![A-Za-z0-9])EX-\d+(?!\d)", RegexOptions.Compiled);

    private static readonly Regex ExclusionSection =
        new(@"^##\s+9\.\s*Exclusions\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ExclusionIdentifier = new(@"^EX-\d+$", RegexOptions.Compiled);

    private static readonly Regex ExclusionDefinitionItem =
        new(@"^(?:[-*+]|\d+[.)])\s+\*\*(?<id>EX-\d+)\*\*", RegexOptions.Compiled);

    private static List<string> ExclusionSectionViolations(IEnumerable<ReviewDocument> bundles)
    {
        var violations = new List<string>();

        foreach (var bundle in bundles)
        {
            Section(bundle, ExclusionSection, "section 9 exclusions", violations);
        }

        return violations;
    }

    private static HashSet<string> ExclusionDefinitions(IEnumerable<ReviewDocument> bundles)
    {
        var defined = new HashSet<string>(StringComparer.Ordinal);

        foreach (var bundle in bundles)
        {
            var section = Section(bundle, ExclusionSection, "section 9 exclusions", []);
            var block = int.MinValue;
            var inExclusionTable = false;

            foreach (var line in section)
            {
                // Section 9 is the last section of a bundle, so its body runs to the end of the
                // file and every appended subsection is inside it. Definitions stop at the first
                // subheading, which is what keeps an appended errata or hash subsection out.
                if (HashCount(line.Text) > 0)
                {
                    break;
                }

                if (line.TableBlock >= 0)
                {
                    // The first line of a table is its header, and the header is what says
                    // whether this table is the exclusion table. A header defines nothing itself.
                    if (line.TableBlock != block)
                    {
                        block = line.TableBlock;
                        inExclusionTable = IsExclusionTableHeader(TableCells(line.Text));
                        continue;
                    }

                    if (!inExclusionTable)
                    {
                        continue;
                    }

                    var first = Unquote(TableCells(line.Text).FirstOrDefault() ?? string.Empty);

                    if (ExclusionIdentifier.IsMatch(first))
                    {
                        defined.Add(first);
                    }

                    continue;
                }

                block = -1;
                inExclusionTable = false;

                // A pipe-led line that no delimiter row binds into a table is not a row of the
                // exclusion table either.
                if (line.IsTableRow)
                {
                    continue;
                }

                var item = ExclusionDefinitionItem.Match(line.Text);

                if (item.Success)
                {
                    defined.Add(item.Groups["id"].Value);
                }
            }
        }

        return defined;
    }

    /// <summary>
    /// The exclusion table is the one whose header names an identifier column and an exclusion
    /// column. A bundle's errata, gate, negative-control and hash tables all fail this and so
    /// cannot launder a definition, which is the whole point of reading the header at all.
    /// </summary>
    private static bool IsExclusionTableHeader(IEnumerable<string> cells)
    {
        var header = cells.Select(Plain).ToArray();

        return header.Length >= 2 &&
            string.Equals(header[0], "ID", StringComparison.OrdinalIgnoreCase) &&
            header.Any(static cell => string.Equals(cell, "Exclusion", StringComparison.OrdinalIgnoreCase));
    }

    private static List<string> UndefinedCitationViolations(
        IEnumerable<ReviewDocument> corpus,
        IReadOnlySet<string> defined) =>
        corpus
            .SelectMany(static document => document.Lines
                .SelectMany(static line => ExclusionCitation.Matches(line.Text).Select(match => match.Value))
                .Distinct(StringComparer.Ordinal)
                .Select(id => (document.Name, Id: id)))
            .Where(cited => !defined.Contains(cited.Id))
            .Select(static cited =>
                $"{cited.Name} cites {cited.Id}, which no evidence bundle defines in its exclusion table")
            .ToList();

    // =====================================================================================
    // H3 - no review document points at a line, and the record's coverage is the tree's
    // =====================================================================================

    /// <summary>
    /// H3. No review document cites a source file by line number, anywhere; and the generated
    /// review record's coverage table names every covered file exactly once, names nothing that is
    /// not a covered file, and gives each one the figures the annotations give for it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What this rule used to be, and why it is not that any more.</b> It held a hand-written
    /// review route of eight areas to a hand-written worksheet of numbered items, and the two to
    /// each other's counts. Both documents are gone. The worksheet cited source files by LINE
    /// NUMBER, and its first item cited a checked multiplication that the annotations had since
    /// turned into a parameter declaration - the rule checked identifiers, areas and counts, and
    /// never whether a citation meant anything. A reviewer now writes in one place, the
    /// <c>Broiler-Human</c> line on the declaration they read, and the record is generated from
    /// those lines.
    /// </para>
    /// <para>
    /// So the clause worth keeping is the one that was there and was too narrow: the line-number
    /// scan. It ran over the worksheet's <c>Read</c> rows alone, which was safe only while the
    /// worksheet was the only document that pointed anywhere. It now runs over every line of every
    /// review document, which is the mechanical form of the rule the owner stated: a review record
    /// does not carry pointers that rot.
    /// </para>
    /// <para>
    /// <b>And the clause that replaces the route.</b> The eight areas were a fixed enumeration
    /// precisely because deriving them from the document under test let an entire risk area be
    /// deleted from both tables at once. The coverage table has the same weakness and a better
    /// anchor: it is compared against <see cref="AssuranceSources.Files"/> and against the
    /// annotations, which are not the document. A file dropped from the record is a file the
    /// release decision cannot see, and that is the same defect as a deleted review area, reached
    /// through a generated document instead of a written one.
    /// </para>
    /// <para>
    /// <b>Exclusion EX-55 closes here.</b> It recorded that nothing outside two documents said how
    /// many items the review ought to have. The extent of the review is now the tree.
    /// </para>
    /// </remarks>
    [Fact]
    public void H3_No_Review_Document_Points_At_A_Line_And_The_Record_Covers_The_Tree()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H3",
            "not derived from the document under test",
            "EX-65");

        // Clause: no review document cites a source line number. The whole corpus, every line -
        // the worksheet whose Read rows were the old scope no longer exists, and a pointer in a
        // bundle or in the ledger rots in exactly the same way.
        Assert.All(Corpus, static document => Assert.Empty(SourceLineCitations(document)));

        var cited = SourceLineCitations(Witness("H3-a-document-cites-a-source-line-number.md.witness"));

        Assert.Equal(2, cited.Count);
        Assert.Single(cited.Where(static violation => violation.Contains(
            "`src/Broiler.VM.Binary/VmBoundedAllocator.cs` 34-68", StringComparison.Ordinal)));
        Assert.Single(cited.Where(static violation => violation.Contains(
            "`src/Broiler.VM.Binary/VmBoundedReader.cs:57`", StringComparison.Ordinal)));
        Assert.All(cited, static violation => Assert.Contains(
            "a line number does not survive an edit - name the member instead",
            violation,
            StringComparison.Ordinal));

        // Clause: the coverage table is the tree. Every covered file, once, with true figures.
        Assert.Empty(CoverageViolations(HumanReview, AssuranceSources.Files, AssuranceScanner.Units));
        Assert.Empty(FileCountViolations(HumanReview, AssuranceSources.Files));

        // Non-vacuous: the table is 61 rows over a tree of 61 files, so a clean result is a
        // comparison and not a quantifier over nothing. JS-0 added three assembly markers, JS-1
        // added seven more files - the format, the profile and the lowering - and JS-3a adds the
        // position encoding, each covered for the same reason every other product file is, which
        // is that it compiles into an assembly this component builds.
        Assert.Equal(61, AssuranceSources.Files.Count);
        Assert.All(
            AssuranceSources.Files,
            static file => Assert.Contains(
                $"| `{file.RelativePath}` |", HumanReview.Text, StringComparison.Ordinal));

        // The rejecting directions are the real record with one thing altered, and each is asserted
        // by the CONTENT of the violation it expects. A stored copy of a generated artefact would be
        // a second copy of the thing under test, would go stale at every regeneration, and would be
        // repaired by regenerating it - the same reason rule J8's report clauses mutate rather than
        // store.
        var dropped = AssuranceSources.Files[0].RelativePath;

        var omitted = CoverageViolations(
            Doctored(HumanReview, $"| `{dropped}` |", "| `src/Broiler.VM.Elsewhere/Gone.cs` |"),
            AssuranceSources.Files,
            AssuranceScanner.Units);

        Assert.Single(omitted.Where(violation => violation.Contains(
            $"does not name the covered file {dropped}", StringComparison.Ordinal)));
        Assert.Single(omitted.Where(static violation => violation.Contains(
            "names src/Broiler.VM.Elsewhere/Gone.cs, which is not a covered file",
            StringComparison.Ordinal)));

        // Clause: a file named twice. One row duplicated is a row whose figures nobody can act on.
        var twice = CoverageViolations(
            Doctored(
                HumanReview,
                $"| `{dropped}` |",
                $"| `{dropped}` | 0 | 0 | 0 | 0 | Low | Low | 0/0 |\n| `{dropped}` |"),
            AssuranceSources.Files,
            AssuranceScanner.Units);

        Assert.Single(twice.Where(violation => violation.Contains(
            $"names {dropped} 2 times", StringComparison.Ordinal)));

        // Clause: a row whose figures are not the annotations'. This is the row a reader uses to
        // decide where to spend their time, and nothing else compares it with the tree.
        var relevant = AssuranceScanner.Units.Count(unit =>
            unit.IsRelevant &&
            string.Equals(unit.File.RelativePath, dropped, StringComparison.Ordinal));

        var untrue = CoverageViolations(
            Doctored(
                HumanReview,
                $"| `{dropped}` | 13 | {relevant} |",
                $"| `{dropped}` | 13 | {relevant + 7} |"),
            AssuranceSources.Files,
            AssuranceScanner.Units);

        Assert.Single(untrue.Where(violation => violation.Contains(
            $"gives {dropped} {relevant + 7} relevant units; the tree declares {relevant}",
            StringComparison.Ordinal)));

        // Clause: exactly one coverage section. A decoy heading above the real one is how every
        // section-reading clause in this group was defeated at once, and a generated document is no
        // less readable from a decoy than a written one.
        var twoSections = CoverageViolations(
            Doctored(HumanReview, "## 6. Coverage By File", "## 6. Coverage By File\n\n## 6. Coverage By File"),
            AssuranceSources.Files,
            AssuranceScanner.Units);

        Assert.Single(twoSections.Where(static violation => violation.Contains(
            "carries 2 'section 6 coverage' headings", StringComparison.Ordinal)));

        // Clause: the summary's own file count agrees with the tree, so dropping a row and
        // correcting the count is not a way through.
        var miscounted = FileCountViolations(
            Doctored(
                HumanReview,
                $"| Files scanned | {AssuranceSources.Files.Count} |",
                "| Files scanned | 44 |"),
            AssuranceSources.Files);

        Assert.Single(miscounted.Where(violation => violation.Contains(
            $"states 44 files scanned; the tree carries {AssuranceSources.Files.Count}",
            StringComparison.Ordinal)));
    }

    private static readonly Regex CoverageSection = new(@"^##\s+6\.\s", RegexOptions.Compiled);

    private static readonly Regex FilesScannedRow =
        new(@"^\|\s*Files scanned\s*\|\s*(?<count>\d+)\s*\|", RegexOptions.Compiled);

    /// <summary>
    /// Every citation that pins a source file to a line number, anywhere in a review document.
    /// </summary>
    /// <remarks>
    /// A member name survives an edit and a line number does not. This is the one form of pointer
    /// this repository has already watched rot unnoticed, and the scan is the whole document
    /// because the document that carried them is gone and the next one will not be a worksheet.
    /// </remarks>
    private static List<string> SourceLineCitations(ReviewDocument document)
    {
        var violations = new List<string>();

        foreach (var line in document.Lines)
        {
            foreach (Match match in SourceLineCitation.Matches(line.Text))
            {
                violations.Add(
                    $"{document.Name} line {line.Number} cites {match.Value.Trim()}, and a line " +
                    "number does not survive an edit - name the member instead");
            }
        }

        return violations;
    }

    private static readonly Regex SourceLineCitation =
        new(@"`[^`]+\.cs`[ ]\d+(?:-\d+)?|`[^`]+\.cs:\d+`", RegexOptions.Compiled);

    /// <summary>One row of the record's coverage table, as the document states it.</summary>
    private sealed record CoverageRow(string Path, int Relevant, int Unverified);

    /// <summary>
    /// Every disagreement between the record's coverage table and the tree it describes.
    /// </summary>
    /// <remarks>
    /// The expectation is the covered file set and the annotations, and never the document: the
    /// eight review areas this replaced were a fixed enumeration for exactly that reason, because
    /// an expectation read from the document under test agrees with it whatever it says.
    /// </remarks>
    private static List<string> CoverageViolations(
        ReviewDocument document,
        IReadOnlyList<AssuranceSourceFile> files,
        IReadOnlyList<AssuranceUnit> units)
    {
        var violations = new List<string>();
        var covered = files.Select(static file => file.RelativePath).ToHashSet(StringComparer.Ordinal);
        var declared = new List<CoverageRow>();

        foreach (var line in Section(document, CoverageSection, "section 6 coverage", violations))
        {
            if (!line.IsTableRow || IsDelimiterRow(line.Text))
            {
                continue;
            }

            var cells = TableCells(line.Text).ToArray();

            if (cells.Length < 5)
            {
                continue;
            }

            var path = Unquote(cells[0]);

            if (!path.EndsWith(".cs", StringComparison.Ordinal))
            {
                continue;
            }

            declared.Add(new CoverageRow(path, ParseCount(cells[2]) ?? -1, ParseCount(cells[4]) ?? -1));
        }

        violations.AddRange(covered
            .Where(path => !declared.Any(row => string.Equals(row.Path, path, StringComparison.Ordinal)))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(path => $"{document.Name} section 6 does not name the covered file {path}"));

        foreach (var group in declared.GroupBy(static row => row.Path, StringComparer.Ordinal))
        {
            if (!covered.Contains(group.Key))
            {
                violations.Add($"{document.Name} section 6 names {group.Key}, which is not a covered file");

                continue;
            }

            if (group.Count() > 1)
            {
                violations.Add($"{document.Name} section 6 names {group.Key} {group.Count()} times");
            }
        }

        foreach (var row in declared.Where(row => covered.Contains(row.Path)))
        {
            var owned = units
                .Where(unit => string.Equals(unit.File.RelativePath, row.Path, StringComparison.Ordinal))
                .ToArray();

            var relevant = owned.Count(static unit => unit.IsRelevant);
            var unverified = owned.Count(static unit =>
                unit.IsRelevant && AssuranceStateMachine.BlocksRelease(unit.State));

            if (row.Relevant != relevant)
            {
                violations.Add(
                    $"{document.Name} section 6 gives {row.Path} {row.Relevant} relevant units; " +
                    $"the tree declares {relevant}");
            }

            if (row.Unverified != unverified)
            {
                violations.Add(
                    $"{document.Name} section 6 gives {row.Path} {row.Unverified} unverified units; " +
                    $"the tree declares {unverified}");
            }
        }

        return violations;
    }

    /// <summary>The record's own count of covered files, against the tree.</summary>
    private static List<string> FileCountViolations(
        ReviewDocument document,
        IReadOnlyList<AssuranceSourceFile> files)
    {
        var stated = document.Lines
            .Select(static line => FilesScannedRow.Match(line.Text))
            .Where(static match => match.Success)
            .Select(static match => int.Parse(match.Groups["count"].Value, CultureInfo.InvariantCulture))
            .ToArray();

        if (stated.Length != 1)
        {
            return
            [
                $"{document.Name} states {stated.Length} 'Files scanned' rows; exactly one is required",
            ];
        }

        return stated[0] == files.Count
            ? []
            : [$"{document.Name} states {stated[0]} files scanned; the tree carries {files.Count}"];
    }

    private static int? ParseCount(string cell) =>
        int.TryParse(Plain(cell), NumberStyles.None, CultureInfo.InvariantCulture, out var count)
            ? count
            : null;

    /// <summary>The same document with one substitution, for a rejecting direction.</summary>
    private static ReviewDocument Doctored(ReviewDocument document, string find, string replace) =>
        new(document.Name, document.Text.Replace(find, replace, StringComparison.Ordinal));

    // =====================================================================================
    // H4 - the record states no decision the annotations do not hold
    // =====================================================================================

    /// <summary>
    /// H4. The review record's status is the aggregate the annotations give, its stated figures are
    /// the annotations' figures, and every alias it names is one a <c>Broiler-Human</c> line in the
    /// product tree carries - with none of them left out.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The risk this answers is the one it always answered.</b> The record used to end in a
    /// four-state decision list and a four-field attestation, and the cheapest false approval was to
    /// tick <c>APPROVED</c> over unfilled fields - or to delete the signature line, which the rule
    /// could not see until its field set was fixed. There is no attestation now: a decision is the
    /// <c>Broiler-Human</c> line on a declaration, and the record is computed from those lines. The
    /// same false record is therefore reached differently - by a record that states a status the
    /// states do not give, or that names somebody no line in the tree names - and that is what this
    /// rule is now.
    /// </para>
    /// <para>
    /// <b>Both directions, and the second one matters as much.</b> An alias in the record that the
    /// tree does not carry is an invented reviewer. An alias in the tree that the record does not
    /// carry is a decision the release record cannot see, which is how a person's work disappears
    /// from the document that decides a publish. Neither is reported by rule J4, which reads human
    /// LINES and says nothing about a document.
    /// </para>
    /// <para>
    /// <b>What FILLED still does not mean.</b> An alias that reads plausibly and belongs to nobody
    /// is accepted here exactly as an invented name in the old attestation was: this rule holds the
    /// record to the tree, and nothing in this component holds the tree to a person. That is the
    /// same limit ADR 0001 named when it deferred this file to VM-6, and EX-60 records what a lane
    /// would have to do to close it.
    /// </para>
    /// </remarks>
    [Fact]
    public void H4_The_Record_States_No_Decision_The_Annotations_Do_Not_Hold()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H4",
            "does not mean",
            "EX-60");

        // The clean direction over this checkout: whatever the human lines say, the record says the
        // same. The status word is derived rather than written out, because "the record says
        // PENDING" is a fact about this milestone and the first alias anybody records would make it
        // false - which is the shape of defect this whole change is repairing, so it must not be
        // reintroduced in the rule that guards against it.
        Assert.Empty(StatusViolations(HumanReview, AssuranceScanner.Units));
        Assert.Empty(AliasViolations(HumanReview, AssuranceScanner.Units));
        Assert.Contains(
            $"**Status: {AssuranceHumanReview.Status(AssuranceScanner.Units)}.**",
            HumanReview.Text,
            StringComparison.Ordinal);

        // Non-vacuous, and this is the whole of the rule's subject: a tree two people have read.
        // Nothing in this component is in that state, so the accepting direction has to be
        // synthesized - and a rule about a record of decisions that has only ever seen a record of
        // none is a rule nobody has watched work.
        var read = AssuranceProbe.Source(
            SourceWitness("J8-a-record-names-every-alias-in-the-tree.cs.witness"),
            "H4-a-tree-two-people-have-read.cs");

        var units = AssuranceScanner.Scan(read);
        var record = new ReviewDocument(
            HumanReviewName, AssuranceHumanReview.Render([read], units));

        Assert.Empty(StatusViolations(record, units));
        Assert.Empty(AliasViolations(record, units));
        Assert.Contains("| WITNESS-ONLY | 1 | 1 | 1 | 0 |", record.Text, StringComparison.Ordinal);
        Assert.Contains("| WITNESS-TWO | 1 | 1 | 0 | 1 |", record.Text, StringComparison.Ordinal);

        // Clause: a status the states do not give. This is the old ticked APPROVED box, reached
        // through the one line that now carries the decision.
        var overstated = StatusViolations(
            Doctored(record, "**Status: PARTIAL.**", "**Status: COMPLETE.**"), units);

        Assert.Single(overstated.Where(static violation => violation.Contains(
            "records the decision COMPLETE; the annotations give PARTIAL", StringComparison.Ordinal)));

        // Clause: a figure the annotations do not give, beside a status word that does.
        var miscounted = StatusViolations(
            Doctored(record, "Human-reviewed: 1 of 3", "Human-reviewed: 2 of 3"), units);

        Assert.Single(miscounted.Where(static violation => violation.Contains(
            "states 2 units carrying a decision; the annotations give 1", StringComparison.Ordinal)));

        // Clause: exactly one status line, so a decoy above the real one is not the line under test.
        var twoStatuses = StatusViolations(
            Doctored(record, "> **Status: PARTIAL.**", "> **Status: COMPLETE.** Human-reviewed: 3 of 3 relevant units.\n> **Status: PARTIAL.**"),
            units);

        Assert.Single(twoStatuses.Where(static violation => violation.Contains(
            "states 2 status lines; exactly one is required", StringComparison.Ordinal)));

        // Clause: an invented alias. Nobody in the tree is called this, and the record says they
        // read one unit.
        var invented = AliasViolations(
            Doctored(record, "| WITNESS-ONLY | 1 | 1 | 1 | 0 |", "| WITNESS-ONLY | 1 | 1 | 1 | 0 |\n| NOBODY | 1 | 1 | 1 | 0 |"),
            units);

        Assert.Single(invented.Where(static violation => violation.Contains(
            "names the alias NOBODY, and no human line in the product tree carries it",
            StringComparison.Ordinal)));

        // Clause: the other direction. An alias dropped from the record is a decision the release
        // record cannot see, and deleting a row is cheaper than inventing one.
        var lost = AliasViolations(
            Doctored(record, "| WITNESS-TWO | 1 | 1 | 0 | 1 |\n", string.Empty), units);

        Assert.Single(lost.Where(static violation => violation.Contains(
            "does not name the alias WITNESS-TWO, which a human line in the product tree carries",
            StringComparison.Ordinal)));

        // Clause: a row whose counts are not the lines' counts - the shape that keeps an alias in
        // the record while overstating what they read.
        var inflated = AliasViolations(
            Doctored(record, "| WITNESS-ONLY | 1 | 1 | 1 | 0 |", "| WITNESS-ONLY | 9 | 1 | 1 | 0 |"),
            units);

        Assert.Single(inflated.Where(static violation => violation.Contains(
            "gives WITNESS-ONLY 9 units; the tree gives 1", StringComparison.Ordinal)));

        // Clause: exactly one alias section.
        var twoSections = AliasViolations(
            Doctored(record, "## 5. Aliases In The Tree", "## 5. Aliases In The Tree\n\n## 5. Aliases In The Tree"),
            units);

        Assert.Single(twoSections.Where(static violation => violation.Contains(
            "carries 2 'section 5 alias' headings", StringComparison.Ordinal)));
    }

    private static readonly Regex AliasSection = new(@"^##\s+5\.\s", RegexOptions.Compiled);

    /// <summary>
    /// The record's headline: the status word and the two figures beside it. The block-quote marker
    /// is optional because the document reader strips it, and the corpus writes the line quoted.
    /// </summary>
    private static readonly Regex StatusLine = new(
        @"^(?:>\s*)?\*\*Status:\s*(?<status>[A-Z]+)\.\*\*\s*Human-reviewed:\s*(?<reviewed>\d+)\s+of\s+(?<relevant>\d+)\s+relevant units\b",
        RegexOptions.Compiled);

    /// <summary>
    /// Every disagreement between the record's headline and the states the annotations give.
    /// </summary>
    private static List<string> StatusViolations(
        ReviewDocument document,
        IReadOnlyList<AssuranceUnit> units)
    {
        var stated = document.Lines
            .Select(static line => StatusLine.Match(line.Text))
            .Where(static match => match.Success)
            .ToArray();

        if (stated.Length != 1)
        {
            return [$"{document.Name} states {stated.Length} status lines; exactly one is required"];
        }

        var violations = new List<string>();
        var relevant = units.Count(static unit => unit.IsRelevant);
        var reviewed = units.Count(static unit =>
            unit.IsRelevant && unit.State == AssuranceReviewState.Verified);

        var expected = reviewed == 0
            ? "PENDING"
            : reviewed < relevant ? "PARTIAL" : "COMPLETE";

        var recorded = stated[0].Groups["status"].Value;

        if (!string.Equals(recorded, expected, StringComparison.Ordinal))
        {
            violations.Add(
                $"{document.Name} records the decision {recorded}; the annotations give {expected}");
        }

        var statedReviewed = int.Parse(stated[0].Groups["reviewed"].Value, CultureInfo.InvariantCulture);
        var statedRelevant = int.Parse(stated[0].Groups["relevant"].Value, CultureInfo.InvariantCulture);

        if (statedReviewed != reviewed)
        {
            violations.Add(
                $"{document.Name} states {statedReviewed} units carrying a decision; the " +
                $"annotations give {reviewed}");
        }

        if (statedRelevant != relevant)
        {
            violations.Add(
                $"{document.Name} states {statedRelevant} relevant units; the tree declares {relevant}");
        }

        return violations;
    }

    /// <summary>
    /// Every disagreement between the aliases the record names and the aliases the human lines
    /// carry, in both directions and including the per-alias counts.
    /// </summary>
    private static List<string> AliasViolations(
        ReviewDocument document,
        IReadOnlyList<AssuranceUnit> units)
    {
        var violations = new List<string>();

        var inTree = units
            .Select(AssuranceHumanReview.AliasOn)
            .Where(static alias => alias is not null)
            .Select(static alias => alias!)
            .ToHashSet(StringComparer.Ordinal);

        var declared = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var line in Section(document, AliasSection, "section 5 alias", violations))
        {
            if (!line.IsTableRow || IsDelimiterRow(line.Text))
            {
                continue;
            }

            var cells = TableCells(line.Text).ToArray();

            if (cells.Length != 5)
            {
                continue;
            }

            var alias = Unquote(cells[0]);

            if (alias.Length == 0 ||
                string.Equals(alias, "Alias", StringComparison.Ordinal) ||
                ParseCount(cells[1]) is not { } stated)
            {
                continue;
            }

            declared[alias] = stated;
        }

        violations.AddRange(declared.Keys
            .Where(alias => !inTree.Contains(alias))
            .OrderBy(static alias => alias, StringComparer.Ordinal)
            .Select(alias =>
                $"{document.Name} section 5 names the alias {alias}, and no human line in the " +
                "product tree carries it"));

        violations.AddRange(inTree
            .Where(alias => !declared.ContainsKey(alias))
            .OrderBy(static alias => alias, StringComparer.Ordinal)
            .Select(alias =>
                $"{document.Name} section 5 does not name the alias {alias}, which a human line " +
                "in the product tree carries"));

        foreach (var (alias, stated) in declared.Where(entry => inTree.Contains(entry.Key)))
        {
            var carried = units.Count(unit => string.Equals(
                AssuranceHumanReview.AliasOn(unit), alias, StringComparison.Ordinal));

            if (stated != carried)
            {
                violations.Add(
                    $"{document.Name} section 5 gives {alias} {stated} units; the tree gives {carried}");
            }
        }

        return violations;
    }

    /// <summary>
    /// A source witness under <c>witnesses/assurance/</c>, read as text.
    /// </summary>
    /// <remarks>
    /// Group H's own witnesses are markdown fragments, because its rules read documents. This one
    /// rule reads a document against a TREE, so its input has to be a tree - and the tree it needs
    /// already exists as rule J8's alias witness. One input, two rules, exactly as A7 and A8 share
    /// one project file: the same synthesized tree violates neither rule and both rules need it to
    /// have anything to say.
    /// </remarks>
    private static string SourceWitness(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "assurance", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return File.ReadAllText(path);
    }

    // =====================================================================================
    // H5 - quoted figures match the retained logs
    // =====================================================================================

    /// <summary>
    /// H5. Every headline figure a review document quotes in a recognised phrasing matches the
    /// value in the log the evidence bundle retains for it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each figure is bound to its own log. An earlier attempt tested set membership across both
    /// publish logs, so SWAPPING the two image sizes passed - which is the likeliest form of that
    /// error, since the two numbers sit two lines apart in one sentence.
    /// </para>
    /// <para>
    /// The four anti-deletion guards compare values. Two of them previously compared none, so
    /// deleting the real figures passed: the VM-0 bundle's historical <c>35 passed</c> kept the
    /// pattern alive on its own.
    /// </para>
    /// <para>
    /// The phrasing lists are the load-bearing part and are kept minimal and non-overlapping, one
    /// witness per entry. Every entry that another entry already subsumed was removed rather than
    /// left in place: an entry no witness can distinguish is an entry that can be deleted with the
    /// suite green, which is exactly the defect this group exists to prevent. What the lists do
    /// NOT recognise is not checked, and the register row says so rather than implying otherwise.
    /// </para>
    /// <para>
    /// <b>The limit, stated rather than glossed.</b> This rule checks document against log. It does
    /// not check log against checkout: a stale log and a stale document agree with each other, and
    /// nothing here re-runs the suite. The bundle's expiry clause and its recertification triggers
    /// are what cover that case. Exclusion EX-54 records it.
    /// </para>
    /// </remarks>
    [Fact]
    public void H5_Quoted_Figures_Match_The_Retained_Logs()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H5",
            "in a recognised phrasing",
            "EX-54",
            "EX-56",
            "EX-81",
            "cannot source the retained figures");

        var current = Figures(CurrentBundle);

        Assert.NotEmpty(current.PerAssembly);
        Assert.NotNull(current.Architecture);
        Assert.NotNull(current.Behavioural);
        Assert.NotNull(current.NativeImageSize);
        Assert.NotNull(current.TrimmedImageSize);

        // H5 reads documents it can source figures FOR, and a profile family's are not among
        // them. The exclusion is asserted rather than assumed: the excluded set is non-empty, and
        // every member of it is a profile document, so it cannot quietly grow to cover a document
        // whose figures this rule could have checked.
        var sourced = Corpus.Where(HasARetainedFigureSource).ToArray();
        var unsourced = Corpus.Where(static document => !HasARetainedFigureSource(document)).ToArray();

        Assert.NotEmpty(sourced);
        Assert.NotEmpty(unsourced);
        Assert.All(unsourced, static document => Assert.StartsWith(
            ProfileDocumentPrefix, document.Name, StringComparison.Ordinal));

        Assert.Empty(sourced.SelectMany(static document => FigureViolations(document, AcceptableFigures(document))));
        Assert.Empty(RetainedFigureGuard(sourced, current));

        // Clause: a quoted suite total is compared against the per-assembly totals and their sum.
        var total = Assert.Single(FigureViolations(
            Witness("H5-suite-total-does-not-match-the-log.md.witness"), current));
        Assert.Contains("quotes a suite total of 222", total, StringComparison.Ordinal);

        // Clause, one per recognised suite-total phrasing. Each sentence is matched by exactly one
        // entry and carries its own number, so deleting any entry drops its violation. The first
        // is the phrasing HUMAN_REVIEW.md, both bundle READMEs and the ledger actually use.
        AssertRecognisedFigure("H5-suite-total-in-every-phrasing.md.witness", current, 4,
            "quotes a suite total of 901",
            "quotes a suite total of 902",
            "quotes a suite total of 903",
            "quotes a suite total of 904");

        // Clause: a quoted architecture/behavioural split is compared per assembly, so the two
        // halves cannot be exchanged for each other either.
        var split = Assert.Single(FigureViolations(
            Witness("H5-split-does-not-match-the-log.md.witness"), current));
        Assert.Contains("quotes a split of 90 architecture and 130 behavioural", split, StringComparison.Ordinal);

        // Clause: each half of the split is bound to the assembly its own Passed: line names, so
        // exchanging the two halves fails even though both numbers are in the log.
        var exchanged = Assert.Single(FigureViolations(
            Witness("H5-split-halves-are-exchanged.md.witness"), current));
        Assert.Contains("quotes a split of 131 architecture and 44 behavioural", exchanged, StringComparison.Ordinal);

        // Clause: the split written behavioural half first. The same false figure in the other
        // word order matched nothing at all, so Splits() returned empty and nothing was compared.
        var reversed = Assert.Single(FigureViolations(
            Witness("H5-split-written-behavioural-first.md.witness"), current));
        Assert.Contains("quotes a split of 131 architecture and 44 behavioural", reversed, StringComparison.Ordinal);

        // Clause: each image size is bound to the log that records it, so a swap fails twice.
        var swapped = FigureViolations(Witness("H5-image-sizes-are-swapped.md.witness"), current);
        Assert.Equal(2, swapped.Count);
        Assert.Single(swapped.Where(static violation => violation.Contains(
            "quotes a Native AOT image size of 78256 bytes", StringComparison.Ordinal)));
        Assert.Single(swapped.Where(static violation => violation.Contains(
            "quotes a trimmed image size of 1565576 bytes", StringComparison.Ordinal)));

        // Clause, one per recognised native-size phrasing.
        AssertRecognisedFigure("H5-native-image-size-in-every-phrasing.md.witness", current, 3,
            "quotes a Native AOT image size of 911111 bytes",
            "quotes a Native AOT image size of 922222 bytes",
            "quotes a Native AOT image size of 933333 bytes");

        // Clause, one per recognised trimmed-size phrasing.
        AssertRecognisedFigure("H5-trimmed-image-size-in-every-phrasing.md.witness", current, 3,
            "quotes a trimmed image size of 944444 bytes",
            "quotes a trimmed image size of 955555 bytes",
            "quotes a trimmed image size of 966666 bytes");

        // Clause: the corpus must still quote the suite total, comparing the value.
        var noTotal = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-suite-total.md.witness")], current));
        Assert.Contains("no review document quotes the current suite total 318", noTotal, StringComparison.Ordinal);

        // Clause: the corpus must still quote the split, comparing the values.
        var noSplit = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-split.md.witness")], current));
        Assert.Contains("split of 121 architecture and 197 behavioural", noSplit, StringComparison.Ordinal);

        // Clause: the corpus must still quote the Native AOT image size, comparing the value.
        var noNative = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-native-image-size.md.witness")], current));
        Assert.Contains("Native AOT image size 1565576 bytes", noNative, StringComparison.Ordinal);

        // Clause: and the trimmed size, which had no guard at all - so nothing forced the corpus
        // to keep quoting it correctly, or at all, once it had been reworded.
        var noTrimmed = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-trimmed-image-size.md.witness")], current));
        Assert.Contains("trimmed image size 78256 bytes", noTrimmed, StringComparison.Ordinal);
    }

    private static void AssertRecognisedFigure(
        string witness,
        LogFigures figures,
        int expected,
        params string[] quoted)
    {
        var violations = FigureViolations(Witness(witness), figures);

        Assert.Equal(expected, violations.Count);

        foreach (var phrase in quoted)
        {
            Assert.Single(violations.Where(violation => violation.Contains(phrase, StringComparison.Ordinal)));
        }
    }

    /// <summary>
    /// The figures a bundle's retained logs record. The architecture and behavioural totals are
    /// bound to the assembly each <c>Passed:</c> line names, so a document that exchanges the two
    /// halves of the split fails as surely as one that invents a number.
    /// </summary>
    private sealed record LogFigures(
        string Bundle,
        IReadOnlyList<int> PerAssembly,
        int Sum,
        int? Architecture,
        int? Behavioural,
        long? NativeImageSize,
        long? TrimmedImageSize,
        IReadOnlyList<long> OtherNativeImageSizes,
        IReadOnlyList<long> OtherTrimmedImageSizes);

    // These three are declared ahead of BundleFigures on purpose: static field initializers run in
    // textual order, and LoadBundleFigures reads all three while the type initializer is running.
    private static readonly Regex PassedPerAssembly = new(
        @"Passed:\s*(?<count>\d+).*?-\s*(?<assembly>[A-Za-z0-9_.]+\.dll)",
        RegexOptions.Compiled);

    private static readonly Regex NativeImageSizeInLog =
        new(@"native image size:\s*(?<size>[\d.,]+)\s*bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ImageSizeInLog =
        new(@"(?:^|\s)image size:\s*(?<size>[\d.,]+)\s*bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // The trimmed spelling, and only it. A composition log records both of its modes and the two
    // lines differ by one word, so a pattern that matched "image size:" anywhere would read an
    // AOT image as a trimmed one and admit the two figures into each other's set - which is the
    // exchange the swapped-sizes witness exists to catch.
    private static readonly Regex TrimmedOnlyImageSizeInLog =
        new(@"(?:^|\n)image size:\s*(?<size>[\d.,]+)\s*bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static IReadOnlyDictionary<string, LogFigures> BundleFigures { get; } = LoadBundleFigures();

    private static LogFigures Figures(string bundle) =>
        BundleFigures.TryGetValue(bundle, out var figures)
            ? figures
            : throw new InvalidOperationException($"No evidence bundle named {bundle}.");

    /// <summary>
    /// A bundle README quotes its own retained logs, so it is compared against them; every other
    /// review document speaks for the current milestone and is compared against the current
    /// bundle's logs.
    /// </summary>
    private static LogFigures FiguresFor(ReviewDocument document)
    {
        var bundle = BundleReadme.Match(document.Name);

        return bundle.Success && BundleFigures.TryGetValue(bundle.Groups["bundle"].Value, out var own)
            ? own
            : Figures(CurrentBundle);
    }

    /// <summary>
    /// Every bundle a document may legitimately quote: the one it speaks for, plus every bundle
    /// whose README it links.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The status ledger is a dated history as well as a current-state record: update rule 1
    /// requires earlier evidence links and decisions to be preserved, so a superseded milestone's
    /// row goes on quoting the figures its own bundle retained. Comparing every line of it against
    /// the current bundle would make preserving that history a violation, which would be the rule
    /// pushing the document to delete exactly what the ledger's own rules require it to keep.
    /// </para>
    /// <para>
    /// The link is what makes it a citation rather than a loophole. A figure is admitted only from
    /// a bundle the document points a reader at, so a number from a bundle the document never
    /// mentions is still caught, and so is a number from no bundle at all.
    /// </para>
    /// <para>
    /// <b>The limit, stated rather than glossed.</b> Within a document that links several bundles
    /// this cannot tell which row a figure belongs to: a figure correct for VM-1 quoted in the row
    /// for VM-2 is admitted. Exclusion EX-81 records it. What is not weakened is the anti-deletion
    /// guard, which still compares the CURRENT bundle's values and so still fails if no document
    /// quotes them.
    /// </para>
    /// </remarks>
    /// <summary>Where a profile family's review documents live, as a path prefix.</summary>
    internal const string ProfileDocumentPrefix = "src/Broiler.VM.Profile.";

    /// <summary>
    /// Whether this rule can source the retained figures a document would be compared against.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A profile family's documents cannot be, and saying so is better than comparing them
    /// against the wrong logs.</b> The figure loader reads a bundle's <c>test.log</c>,
    /// <c>publish-aot.log</c> and <c>publish-jit-and-trimmed.log</c> and parses a suite total out
    /// of an English "Passed:" line. A profile bundle is collected by a different script: its logs
    /// are <c>suite.log</c> and <c>publish-and-run.log</c>, and the totals in them are whatever
    /// the collecting machine's SDK printed, which on the machine that has collected every profile
    /// bundle so far is German. Nothing in the loader would match, so every profile bundle would
    /// resolve to an empty figure set.
    /// </para>
    /// <para>
    /// What made this worth an exclusion rather than a shrug is that the fallback is not
    /// harmless. A document the loader has no bundle for falls back to the CURRENT bundle's
    /// figures, so a profile bundle README would have been compared against the component's own
    /// logs - a comparison that passes today only because there is one suite and its totals are
    /// the same number in both, and that is a coincidence of the moment rather than a property.
    /// A rule that passes for a reason unrelated to what it checks is worse than one that says it
    /// does not reach a document.
    /// </para>
    /// <para>
    /// Decision JSD-0010 records the condition that closes it: a profile bundle is sourced when
    /// the loader reads that family's log names and parses a suite total without depending on the
    /// collecting machine's locale. Until then this is a stated limit and the assertion above
    /// keeps it from covering anything else.
    /// </para>
    /// </remarks>
    private static bool HasARetainedFigureSource(ReviewDocument document) =>
        !document.Name.StartsWith(ProfileDocumentPrefix, StringComparison.Ordinal);

    private static IReadOnlyList<LogFigures> AcceptableFigures(ReviewDocument document)
    {
        var primary = FiguresFor(document);
        var acceptable = new List<LogFigures> { primary };

        if (BundleReadme.IsMatch(document.Name))
        {
            // A bundle README speaks for one bundle and links no other as a source of figures.
            return acceptable;
        }

        foreach (var pair in BundleFigures)
        {
            if (!string.Equals(pair.Value.Bundle, primary.Bundle, StringComparison.Ordinal) &&
                document.Text.Contains($"evidence/{pair.Key}/README.md", StringComparison.Ordinal))
            {
                acceptable.Add(pair.Value);
            }
        }

        return acceptable;
    }

    private static IReadOnlyDictionary<string, LogFigures> LoadBundleFigures()
    {
        var evidence = Path.Combine(ComponentGraph.Root, "docs", "evidence");
        var figures = new Dictionary<string, LogFigures>(StringComparer.Ordinal);

        if (!Directory.Exists(evidence))
        {
            return figures;
        }

        foreach (var directory in Directory.EnumerateDirectories(evidence))
        {
            var bundle = Path.GetFileName(directory);
            var perAssembly = new List<int>();
            int? architecture = null;
            int? behavioural = null;

            var testLog = Path.Combine(directory, "test.log");

            if (File.Exists(testLog))
            {
                foreach (Match match in PassedPerAssembly.Matches(File.ReadAllText(testLog)))
                {
                    var count = int.Parse(match.Groups["count"].Value, CultureInfo.InvariantCulture);
                    perAssembly.Add(count);

                    if (match.Groups["assembly"].Value.Contains("Architecture.Tests", StringComparison.OrdinalIgnoreCase))
                    {
                        architecture = count;
                    }
                    else
                    {
                        behavioural = count;
                    }
                }
            }

            figures[bundle] = new LogFigures(
                Bundle: bundle,
                PerAssembly: perAssembly,
                Sum: perAssembly.Sum(),
                Architecture: architecture,
                Behavioural: behavioural,
                NativeImageSize: SizeFromLog(Path.Combine(directory, "publish-aot.log"), NativeImageSizeInLog),
                TrimmedImageSize: SizeFromLog(Path.Combine(directory, "publish-jit-and-trimmed.log"), ImageSizeInLog),

                // A bundle may publish more than one image. VM-3 publishes three - the fixtures
                // host and two composition roots - and a rule that knew only the first would call
                // a correctly quoted composition size a violation, which is the rule failing
                // rather than the document. Every size the bundle retained is admitted; the
                // anti-deletion guard still holds the corpus to the two PRIMARY figures, so
                // widening what is admitted does not weaken what must be quoted.
                OtherNativeImageSizes: SizesFromLogs(directory, "composition-*.log", NativeImageSizeInLog),
                OtherTrimmedImageSizes: SizesFromLogs(directory, "composition-*.log", TrimmedOnlyImageSizeInLog));
        }

        return figures;
    }

    /// <summary>
    /// Every distinct size the named logs record, in file order. Unlike
    /// <see cref="SizeFromLog"/> this does not refuse a log with several: a composition log
    /// deliberately records both of its modes, and the caller is asking which sizes the bundle
    /// retained rather than binding one figure to one log.
    /// </summary>
    private static IReadOnlyList<long> SizesFromLogs(string directory, string pattern, Regex expression)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(directory, pattern)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .SelectMany(path => expression.Matches(File.ReadAllText(path))
                .Select(match => Grouped(match.Groups["size"].Value)))
            .Distinct()
            .ToArray();
    }

    private static long? SizeFromLog(string path, Regex pattern)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var sizes = pattern
            .Matches(File.ReadAllText(path))
            .Select(match => Grouped(match.Groups["size"].Value))
            .Distinct()
            .ToArray();

        // More than one distinct size in a log the rule is about to bind a figure to would make
        // the binding meaningless, so it is refused rather than guessed at.
        return sizes.Length == 1 ? sizes[0] : null;
    }

    /// <summary>
    /// Reads a grouped byte figure. The collecting machine's locale groups digits with dots, so
    /// <c>1.279.488</c> and <c>1,279,488</c> are the same number written twice.
    /// </summary>
    private static long Grouped(string text) => long.Parse(
        text.Replace(".", string.Empty, StringComparison.Ordinal)
            .Replace(",", string.Empty, StringComparison.Ordinal),
        CultureInfo.InvariantCulture);

    // The recognised phrasings. A figure written some other way is not checked, which the register
    // row says in as many words rather than implying full coverage. Each entry is exercised by a
    // witness sentence that no OTHER entry matches, so no entry can be deleted silently; an entry
    // that could not be distinguished that way was subsumed into a broader one instead of being
    // kept as unwitnessed decoration.
    private static readonly Regex[] SuiteTotalPhrasings =
    [
        new(@"(?<n>\d[\d,]*)\s+(?:tests?\s+)?pass(?:ed|ing|es)?\s*,\s*\d+\s+fail\w*", RegexOptions.IgnoreCase),
        new(@"(?<n>\d[\d,]*)\s+passing\s+tests?\b", RegexOptions.IgnoreCase),
        new(@"(?<n>\d[\d,]*)\s+tests?\s+pass(?:ed|ing|es)?\b", RegexOptions.IgnoreCase),
        new(@"(?<n>\d[\d,]*)\s+tests?\s*,\s*\d+\s+(?:failures?|failed|failing)\b", RegexOptions.IgnoreCase),
    ];

    private static readonly Regex[] SplitPhrasings =
    [
        new(@"(?<a>\d[\d,]*)\s+architecture(?:\s+tests?)?\s*(?:,|and)\s*(?<b>\d[\d,]*)\s+behavioural(?:\s+tests?)?\b",
            RegexOptions.IgnoreCase),
        new(@"(?<b>\d[\d,]*)\s+behavioural(?:\s+tests?)?\s*(?:,|and)\s*(?<a>\d[\d,]*)\s+architecture(?:\s+tests?)?\b",
            RegexOptions.IgnoreCase),
    ];

    private static readonly Regex[] NativeSizePhrasings =
    [
        new(@"(?<size>\d[\d.,]*)\s*-\s*byte\s+(?:\S+\s+){0,3}?native\b", RegexOptions.IgnoreCase),
        new(@"\bnative(?:\s+aot)?\s+(?:\S+\s+){0,3}?(?:binary|image|executable)\b[^.|]{0,32}?(?<size>\d[\d.,]*)\s*bytes\b",
            RegexOptions.IgnoreCase),
        new(@"\b(?:native\s+aot|native|aot)\s+(?:\S+\s+){0,2}?size\b[^.|]{0,24}?(?<size>\d[\d.,]*)\s*bytes\b",
            RegexOptions.IgnoreCase),
    ];

    private static readonly Regex[] TrimmedSizePhrasings =
    [
        new(@"(?<size>\d[\d.,]*)\s*-\s*byte\s+(?:\S+\s+){0,3}?trimmed\b", RegexOptions.IgnoreCase),
        new(@"\btrimmed(?:\s+self-contained)?\s+(?:\S+\s+){0,3}?(?:binary|image|executable)\b[^.|]{0,32}?(?<size>\d[\d.,]*)\s*bytes\b",
            RegexOptions.IgnoreCase),
        new(@"\btrimmed\s+(?:\S+\s+){0,2}?size\b[^.|]{0,24}?(?<size>\d[\d.,]*)\s*bytes\b", RegexOptions.IgnoreCase),
    ];

    private static List<int> SuiteTotals(ReviewDocument document) =>
        Captures(document, SuiteTotalPhrasings, "n")
            .Select(static value => (int)Grouped(value))
            .ToList();

    private static List<(int Architecture, int Behavioural)> Splits(ReviewDocument document)
    {
        var splits = new List<(int, int)>();

        foreach (var line in LogicalLines(document.Lines))
        {
            foreach (var pattern in SplitPhrasings)
            {
                foreach (Match match in pattern.Matches(line))
                {
                    splits.Add(((int)Grouped(match.Groups["a"].Value), (int)Grouped(match.Groups["b"].Value)));
                }
            }
        }

        return splits;
    }

    private static List<long> NativeSizes(ReviewDocument document) =>
        Captures(document, NativeSizePhrasings, "size").Select(Grouped).ToList();

    private static List<long> TrimmedSizes(ReviewDocument document) =>
        Captures(document, TrimmedSizePhrasings, "size").Select(Grouped).ToList();

    private static List<string> Captures(ReviewDocument document, IEnumerable<Regex> patterns, string group)
    {
        var captured = new List<string>();

        foreach (var line in LogicalLines(document.Lines))
        {
            foreach (var pattern in patterns)
            {
                foreach (Match match in pattern.Matches(line))
                {
                    captured.Add(match.Groups[group].Value);
                }
            }
        }

        return captured;
    }

    /// <summary>
    /// The violations that survive every bundle the document may quote. A figure admitted by one
    /// acceptable bundle is admitted; a figure admitted by none is reported against the first,
    /// which is the bundle the document speaks for.
    /// </summary>
    private static List<string> FigureViolations(
        ReviewDocument document,
        IReadOnlyList<LogFigures> acceptable)
    {
        var reported = FigureViolations(document, acceptable[0]);

        for (var index = 1; index < acceptable.Count && reported.Count > 0; index++)
        {
            var alternative = FigureViolations(document, acceptable[index])
                .Select(Subject)
                .ToHashSet(StringComparer.Ordinal);

            reported = reported.Where(violation => alternative.Contains(Subject(violation))).ToList();
        }

        return reported;

        // The part of a violation that names WHICH figure it is about, so the same figure judged
        // against two bundles is recognised as one figure rather than two messages.
        static string Subject(string violation) => violation.Split(';')[0];
    }

    private static List<string> FigureViolations(ReviewDocument document, LogFigures figures)
    {
        var violations = new List<string>();
        var accepted = figures.PerAssembly.Append(figures.Sum).ToHashSet();

        foreach (var total in SuiteTotals(document).Distinct())
        {
            if (!accepted.Contains(total))
            {
                violations.Add(
                    $"{document.Name} quotes a suite total of {total}; docs/evidence/{figures.Bundle}/test.log " +
                    $"records {string.Join(" and ", figures.PerAssembly)} passed, {figures.Sum} in total");
            }
        }

        foreach (var split in Splits(document).Distinct())
        {
            if (split.Architecture != figures.Architecture || split.Behavioural != figures.Behavioural)
            {
                violations.Add(
                    $"{document.Name} quotes a split of {split.Architecture} architecture and " +
                    $"{split.Behavioural} behavioural tests; docs/evidence/{figures.Bundle}/test.log records " +
                    $"{Show(figures.Architecture)} and {Show(figures.Behavioural)}");
            }
        }

        foreach (var size in NativeSizes(document).Distinct())
        {
            if (size != figures.NativeImageSize && !figures.OtherNativeImageSizes.Contains(size))
            {
                violations.Add(
                    $"{document.Name} quotes a Native AOT image size of {size} bytes; " +
                    $"docs/evidence/{figures.Bundle}/publish-aot.log records {Show(figures.NativeImageSize)}");
            }
        }

        foreach (var size in TrimmedSizes(document).Distinct())
        {
            if (size != figures.TrimmedImageSize && !figures.OtherTrimmedImageSizes.Contains(size))
            {
                violations.Add(
                    $"{document.Name} quotes a trimmed image size of {size} bytes; " +
                    $"docs/evidence/{figures.Bundle}/publish-jit-and-trimmed.log records " +
                    $"{Show(figures.TrimmedImageSize)}");
            }
        }

        return violations;

        static string Show(long? value) =>
            value?.ToString(CultureInfo.InvariantCulture) ?? "no such figure";
    }

    /// <summary>
    /// The anti-deletion guard. Every one of the four compares a VALUE: a guard that only asks
    /// whether a pattern matched somewhere is satisfied by any surviving figure in the corpus,
    /// including a superseded bundle's.
    /// </summary>
    private static List<string> RetainedFigureGuard(IEnumerable<ReviewDocument> corpus, LogFigures figures)
    {
        var documents = corpus.ToArray();
        var violations = new List<string>();

        if (!documents.Any(document => SuiteTotals(document).Contains(figures.Sum)))
        {
            violations.Add($"no review document quotes the current suite total {figures.Sum}");
        }

        if (!documents.Any(document => Splits(document)
                .Any(split => split.Architecture == figures.Architecture &&
                    split.Behavioural == figures.Behavioural)))
        {
            violations.Add(
                $"no review document quotes the current split of {figures.Architecture} architecture " +
                $"and {figures.Behavioural} behavioural tests");
        }

        if (!documents.Any(document => NativeSizes(document)
                .Any(size => size == figures.NativeImageSize)))
        {
            violations.Add($"no review document quotes the current Native AOT image size {figures.NativeImageSize} bytes");
        }

        if (!documents.Any(document => TrimmedSizes(document)
                .Any(size => size == figures.TrimmedImageSize)))
        {
            violations.Add($"no review document quotes the current trimmed image size {figures.TrimmedImageSize} bytes");
        }

        return violations;
    }

    /// <summary>
    /// Joins a markdown paragraph's wrapped lines back into one line, so a figure and the words
    /// that qualify it are matchable even when the author's wrap put them on different lines.
    /// Table rows, headings and list items each stay a line of their own, which is what keeps one
    /// table row's figure from being read against the next row's words.
    /// </summary>
    private static string[] LogicalLines(IEnumerable<SourceLine> lines)
    {
        var joined = new List<string>();
        var current = new StringBuilder();

        foreach (var line in lines)
        {
            if (line.Text.Length == 0)
            {
                Flush();
                continue;
            }

            if (current.Length > 0 && StartsBlock(line))
            {
                Flush();
            }

            if (current.Length > 0)
            {
                current.Append(' ');
            }

            current.Append(line.Text);
        }

        Flush();

        return joined.ToArray();

        void Flush()
        {
            if (current.Length > 0)
            {
                joined.Add(current.ToString());
            }

            current.Clear();
        }
    }

    private static bool StartsBlock(SourceLine line) =>
        line.IsTableRow ||
        HashCount(line.Text) > 0 ||
        BlockStart.IsMatch(line.Text);

    private static readonly Regex BlockStart = new(@"^(?:[-*+]|\d+[.)])\s", RegexOptions.Compiled);

    // =====================================================================================
    // The register rows are held to the limits their rules depend on
    // =====================================================================================

    /// <summary>
    /// Nothing else in the suite reads a register row's prose. <c>RuleRegisterTests</c>
    /// deserializes id, owningAdr, status, activationMilestone and witness and nothing else, so
    /// the one field carrying the honest limits of a rule could be rewritten into an over-claim in
    /// a single edit with the suite green - and an over-claim is by definition a rule weaker than
    /// its own statement, the standing defect the register exists to prevent. Each group H rule
    /// therefore names the limits its own row must keep stating.
    /// </summary>
    private static void AssertTheRegisterRowStatesItsLimits(string id, params string[] required)
    {
        var prose = RegisterRowProse(id);

        foreach (var phrase in required)
        {
            Assert.True(
                prose.Contains(phrase, StringComparison.OrdinalIgnoreCase),
                $"The register row {id} no longer states the limit \"{phrase}\", so the row claims more " +
                "than the rule delivers.");
        }
    }

    private static string RegisterRowProse(string id)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json");

        using var register = JsonDocument.Parse(
            File.ReadAllText(path),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });

        foreach (var rule in register.RootElement.GetProperty("rules").EnumerateArray())
        {
            if (!string.Equals(rule.GetProperty("id").GetString(), id, StringComparison.Ordinal))
            {
                continue;
            }

            return string.Join(
                "\n",
                new[] { "statement", "evidence", "nonVacuousWhen" }
                    .Select(field => rule.TryGetProperty(field, out var value) ? value.GetString() ?? string.Empty : string.Empty));
        }

        throw new InvalidOperationException($"The register has no row {id}.");
    }

    // =====================================================================================
    // Shared markdown reading
    // =====================================================================================

    /// <summary>
    /// One line of a review document, read as markdown: block-quote markers stripped, fenced code
    /// dropped entirely, and table membership resolved.
    /// </summary>
    /// <param name="Number">The 1-based line number in the file, for the violation message.</param>
    /// <param name="Text">The line's content, trimmed and unquoted.</param>
    /// <param name="TableBlock">
    /// The index of the table this line belongs to, or -1. A line is in a table when a delimiter
    /// row binds it to one; the leading pipe is optional in GFM and a scan gated on it skipped a
    /// real row whole, which is how a tenth mark was published past the legend equality check.
    /// </param>
    private sealed record SourceLine(int Number, string Text, int TableBlock)
    {
        /// <summary>
        /// A line is read as a table row when a delimiter row binds it into a table, or when it
        /// opens with a pipe. The second half is what keeps a hand-written row outside a
        /// well-formed table from being skipped.
        /// </summary>
        public bool IsTableRow { get; } = TableBlock >= 0 || Text.StartsWith('|');
    }

    private static string[] SplitLines(string text) =>
        text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

    /// <summary>
    /// Reads a document into lines once. Fenced code is dropped, because an example in a fence is
    /// an illustration and not a claim - a worked example showing how to write an exclusion row
    /// would otherwise define the identifier it illustrates. Block-quote markers are stripped,
    /// because a quoted list item is still a list item and HUMAN_REVIEW.md opens with a quoted
    /// banner, so the quoted forms are the document's own idiom.
    /// </summary>
    private static IReadOnlyList<SourceLine> ReadLines(string text)
    {
        var raw = SplitLines(text);
        var content = new string[raw.Length];
        var fenced = new bool[raw.Length];
        var block = new int[raw.Length];
        string? open = null;

        for (var index = 0; index < raw.Length; index++)
        {
            var line = Unquoted(raw[index]);
            content[index] = line;
            block[index] = -1;

            var fence = FenceMarker(line);

            if (open is null)
            {
                if (fence is not null)
                {
                    open = fence;
                    fenced[index] = true;
                }

                continue;
            }

            fenced[index] = true;

            if (fence is not null && fence[0] == open[0] && fence.Length >= open.Length)
            {
                open = null;
            }
        }

        var blocks = 0;

        // Fenced lines are not excluded here, and deliberately so: they are dropped from the list
        // this returns, so a table found inside a fence reaches no caller, and a guard here would
        // be a clause no witness could distinguish from its absence - which is the kind of clause
        // that gets deleted in someone else's patch with the suite green.
        for (var index = 1; index < raw.Length; index++)
        {
            if (block[index] >= 0 || !IsDelimiterRow(content[index]))
            {
                continue;
            }

            var header = index - 1;

            if (content[header].Length == 0 ||
                HashCount(content[header]) > 0 || !content[header].Contains('|'))
            {
                continue;
            }

            block[header] = blocks;
            block[index] = blocks;

            for (var row = index + 1; row < raw.Length; row++)
            {
                if (content[row].Length == 0 ||
                    HashCount(content[row]) > 0 || !content[row].Contains('|'))
                {
                    break;
                }

                block[row] = blocks;
            }

            blocks++;
        }

        var lines = new List<SourceLine>(raw.Length);

        for (var index = 0; index < raw.Length; index++)
        {
            if (!fenced[index])
            {
                lines.Add(new SourceLine(index + 1, content[index], block[index]));
            }
        }

        return lines;
    }

    /// <summary>Strips any depth of block-quote marker, then trims.</summary>
    private static string Unquoted(string line)
    {
        var text = line.Trim();

        while (text.StartsWith('>'))
        {
            text = text[1..].TrimStart();
        }

        return text;
    }

    private static string? FenceMarker(string text)
    {
        foreach (var marker in new[] { '`', '~' })
        {
            var run = 0;

            while (run < text.Length && text[run] == marker)
            {
                run++;
            }

            if (run >= 3)
            {
                return text[..run];
            }
        }

        return null;
    }

    /// <summary>
    /// A GFM table's delimiter row: pipes, dashes, colons and space, with at least one of each of
    /// the first two. It is the delimiter row, not the leading pipe, that makes a table a table.
    /// </summary>
    private static bool IsDelimiterRow(string text) =>
        text.Contains('|') &&
        text.Contains('-') &&
        text.All(static character => character is '|' or '-' or ':' or ' ' or '\t');

    /// <summary>
    /// The body of the section whose heading matches, up to the next heading at the same level or
    /// above, together with how many headings matched at all. Subsections are part of their
    /// parent, which is why section 1's two legend tables are both read.
    /// </summary>
    /// <remarks>
    /// The count is the point. Returning the FIRST match and never asking whether it was the only
    /// one let a decoy heading inserted above the real one become the section under test: a full
    /// decoy route table hid an RA-8 row deleted from the real one, a decoy progress table hid a
    /// falsified total, and a decoy heading carrying a ticked PENDING box hid an APPROVED record
    /// with four unsigned attestation fields. Every caller reports a duplicate as a violation of
    /// its own rule.
    /// </remarks>
    private static IReadOnlyList<SourceLine> Section(
        ReviewDocument document,
        Regex heading,
        string label,
        List<string> violations)
    {
        var lines = document.Lines;
        var start = -1;
        var end = lines.Count;
        var level = 0;
        var matches = 0;

        for (var index = 0; index < lines.Count; index++)
        {
            var depth = HashCount(lines[index].Text);

            if (depth > 0 && heading.IsMatch(lines[index].Text))
            {
                matches++;

                if (start < 0)
                {
                    start = index + 1;
                    level = depth;
                    end = lines.Count;
                    continue;
                }
            }

            if (start >= 0 && end == lines.Count && index >= start && depth > 0 && depth <= level)
            {
                end = index;
            }
        }

        if (matches > 1)
        {
            violations.Add($"{document.Name} carries {matches} '{label}' headings; exactly one is required");
        }

        return start < 0 ? [] : lines.Skip(start).Take(end - start).ToArray();
    }

    private static int HashCount(string trimmed)
    {
        var hashes = 0;

        while (hashes < trimmed.Length && trimmed[hashes] == '#')
        {
            hashes++;
        }

        return hashes > 0 && hashes < trimmed.Length && char.IsWhiteSpace(trimmed[hashes]) ? hashes : 0;
    }

    /// <summary>
    /// The cells of a table row, accepting a row with or without the optional leading pipe and
    /// with or without the optional trailing pipe. GFM makes both optional; an earlier attempt
    /// required both, so a row written either shorter way was skipped whole.
    /// </summary>
    private static IEnumerable<string> TableCells(string row)
    {
        var body = row.Trim();

        if (body.StartsWith('|'))
        {
            body = body[1..];
        }

        if (body.EndsWith('|'))
        {
            body = body[..^1];
        }

        return body.Split('|').Select(static cell => cell.Trim());
    }

    private static string Unquote(string value)
    {
        var text = value.Trim();

        // Backticks and markdown emphasis are decoration, not part of the token. Stripping only
        // backticks left `**[APPROVED]**` outside both the body scan and the legend-equality
        // check, so a tenth mark could be published and used with the suite green. Bold is the
        // third decoration this corpus already uses, so it is decoration here too.
        var changed = true;

        while (changed && text.Length >= 2)
        {
            changed = false;

            if (text[0] == '`' && text[^1] == '`')
            {
                text = text[1..^1].Trim();
                changed = true;
                continue;
            }

            foreach (var wrapper in EmphasisWrappers)
            {
                if (text.Length >= 2 * wrapper.Length &&
                    text.StartsWith(wrapper, StringComparison.Ordinal) &&
                    text.EndsWith(wrapper, StringComparison.Ordinal))
                {
                    text = text[wrapper.Length..^wrapper.Length].Trim();
                    changed = true;
                    break;
                }
            }
        }

        return text;
    }

    private static readonly string[] EmphasisWrappers = ["**", "__", "*", "_"];

    /// <summary>Strips backticks and markdown emphasis, leaving the value itself.</summary>
    private static string Plain(string value) => Unquote(value).Trim('*', '_').Trim();
}
