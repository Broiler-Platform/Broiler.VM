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
    /// The documents a reviewer reads: the review record itself, the per-item worksheets, the
    /// evidence bundles, and the status ledger. Every group H rule reads this one set.
    /// </summary>
    private static IReadOnlyList<ReviewDocument> Corpus { get; } = LoadCorpus();

    private const string HumanReviewName = "HUMAN_REVIEW.md";
    private const string WorksheetName = "docs/review/vm-0-vm-1.md";

    /// <summary>The bundle whose logs the current milestone's figures are quoted from.</summary>
    private const string CurrentBundle = "vm-1";

    private static ReviewDocument HumanReview => Document(HumanReviewName);

    private static ReviewDocument Worksheet => Document(WorksheetName);

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

        return paths
            .Where(File.Exists)
            .Select(static path => new ReviewDocument(
                Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'),
                File.ReadAllText(path)))
            .OrderBy(static document => document.Name, StringComparer.Ordinal)
            .ToArray();
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
    [Fact]
    public void H1_The_Mark_Vocabulary_Is_Closed()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H1",
            "with or without the optional leading pipe",
            "GFM task-list checkbox");

        Assert.Empty(UnpublishedMarkViolations(Corpus));
        Assert.Empty(LegendViolations(HumanReview));

        // Clause: a table cell whose whole trimmed text is a mark token. The token is eight
        // characters inside the brackets, so it also carries the no-length-cap requirement.
        var cell = Assert.Single(UnpublishedMarkViolations([Witness("H1-table-cell-unknown-mark.md.witness")]));
        Assert.Contains("H1-table-cell-unknown-mark.md.witness", cell, StringComparison.Ordinal);
        Assert.Contains("table cell mark token [APPROVED]", cell, StringComparison.Ordinal);

        // Clause: the trailing pipe is optional in GFM, so a row that omits it is still a row.
        var loose = Assert.Single(UnpublishedMarkViolations([Witness("H1-table-row-without-trailing-pipe.md.witness")]));
        Assert.Contains("table cell mark token [REJECTED]", loose, StringComparison.Ordinal);

        // Clause: the LEADING pipe is optional too. A row that omits it is a real row of the table
        // it sits in, and gating the scan on the leading pipe skipped it whole.
        var headless = Assert.Single(UnpublishedMarkViolations([Witness("H1-table-row-without-a-leading-pipe.md.witness")]));
        Assert.Contains("table cell mark token [APPROVED]", headless, StringComparison.Ordinal);

        // Clause: a bracketed token leading a list item, which is the form section 5 uses.
        var item = Assert.Single(UnpublishedMarkViolations([Witness("H1-list-item-unknown-mark.md.witness")]));
        Assert.Contains("list item mark token [NOT MET]", item, StringComparison.Ordinal);

        // Clause: the final token of an ATX heading.
        var heading = Assert.Single(UnpublishedMarkViolations([Witness("H1-heading-unknown-mark.md.witness")]));
        Assert.Contains("heading mark token [PENDING]", heading, StringComparison.Ordinal);

        // Clause: a heading mark written in backticks, which the heading branch has to unquote.
        var quoted = Assert.Single(UnpublishedMarkViolations([Witness("H1-heading-mark-in-backticks.md.witness")]));
        Assert.Contains("heading mark token [APPROVED]", quoted, StringComparison.Ordinal);

        // Clause: block-quote markers are stripped before a line is classified, so a quoted list
        // item, table row and heading are each still what they are.
        var quotedBlock = UnpublishedMarkViolations([Witness("H1-marks-inside-a-block-quote.md.witness")]);
        Assert.Equal(3, quotedBlock.Count);
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("list item mark token [APPROVED]", StringComparison.Ordinal)));
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("heading mark token [WAIVED]", StringComparison.Ordinal)));
        Assert.Single(quotedBlock.Where(static violation =>
            violation.Contains("table cell mark token [REJECTED]", StringComparison.Ordinal)));

        // Clause: the legend itself, parsed independently of the body scan, in both directions.
        var legend = LegendViolations(Witness("H1-legend-does-not-publish-the-nine.md.witness"));
        Assert.Equal(2, legend.Count);
        Assert.Single(legend.Where(static violation =>
            violation.Contains("publishes [WAIVED]", StringComparison.Ordinal)));
        Assert.Single(legend.Where(static violation =>
            violation.Contains("does not publish [?]", StringComparison.Ordinal)));

        // Clause: the legend scan reads a row that omits the leading pipe, so a tenth mark cannot
        // be published past the equality check by dropping one character.
        var legendRow = Assert.Single(LegendViolations(Witness("H1-legend-row-without-a-leading-pipe.md.witness")));
        Assert.Contains("publishes [WAIVED]", legendRow, StringComparison.Ordinal);

        // Clause: the legend publishes each of the nine once.
        var twice = Assert.Single(LegendViolations(Witness("H1-legend-publishes-a-mark-twice.md.witness")));
        Assert.Contains("publishes [MET] more than once", twice, StringComparison.Ordinal);

        // Clause: exactly one legend section. Section() returning its first match with no
        // uniqueness check let a decoy heading above the real one become the section under test.
        var twoLegends = Assert.Single(LegendViolations(Witness("H1-legend-section-appears-twice.md.witness")));
        Assert.Contains("carries 2 'section 1 legend' headings", twoLegends, StringComparison.Ordinal);
    }

    /// <summary>The nine marks the legend publishes, as a fixed set.</summary>
    private static readonly string[] PublishedMarks =
        ["[MET]", "[PART]", "[UNMET]", "[N/A]", "[ ]", "[A]", "[C]", "[R]", "[?]"];

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

    private static List<string> UnpublishedMarkViolations(IEnumerable<ReviewDocument> corpus)
    {
        var published = PublishedMarks.ToHashSet(StringComparer.Ordinal);

        return corpus
            .SelectMany(static document => MarkTokens(document.Lines)
                .Select(mark => (document.Name, Mark: mark)))
            .Where(found => !published.Contains(found.Mark.Token))
            .Select(static found =>
                $"{found.Name}:{found.Mark.Line} uses {found.Mark.Kind} mark token {found.Mark.Token}, " +
                "which the legend does not publish")
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
    private static List<string> LegendMarks(ReviewDocument document, List<string> violations)
    {
        var section = Section(document, LegendSection, "section 1 legend", violations);
        var marks = new List<string>();

        foreach (var line in section)
        {
            if (!line.IsTableRow)
            {
                continue;
            }

            var first = Unquote(TableCells(line.Text).FirstOrDefault() ?? string.Empty);

            if (MarkToken.IsMatch(first))
            {
                marks.Add(first);
            }
        }

        return marks;
    }

    private static readonly Regex LegendSection = new(@"^##\s+1\.\s", RegexOptions.Compiled);

    private static List<string> LegendViolations(ReviewDocument document)
    {
        var violations = new List<string>();
        var legend = LegendMarks(document, violations);

        violations.AddRange(legend
            .Distinct(StringComparer.Ordinal)
            .Where(token => !PublishedMarks.Contains(token, StringComparer.Ordinal))
            .Select(token => $"{document.Name} legend publishes {token}, which is not one of the nine"));

        violations.AddRange(PublishedMarks
            .Where(token => !legend.Contains(token, StringComparer.Ordinal))
            .Select(token => $"{document.Name} legend does not publish {token}"));

        violations.AddRange(legend
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
            "subsection");

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
    // H3 - the review route is covered, contiguous and counted
    // =====================================================================================

    /// <summary>
    /// H3. The eight review areas are declared in the review record, every one has at least one
    /// worksheet item, every item names one of the eight and agrees with the area heading it sits
    /// under, item identifiers are contiguous from <c>RC-01</c>, and the worksheet's own counts
    /// are true and agree with the count the review record states.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The eight areas are a fixed enumeration here. An earlier attempt derived them from the
    /// document under test, so deleting <c>RA-8</c> from the route table, the verdict table, the
    /// worksheet heading and its six items passed the suite: an entire risk area vanished
    /// silently. That is structurally the same defect as the V4 one this register was created to
    /// prevent, so the expectation is the enumeration and never the document.
    /// </para>
    /// <para>
    /// Fixing the enumeration was not enough on its own. Every section this rule reads is
    /// identified by a heading pattern, and a section lookup that returned its FIRST match let a
    /// decoy heading inserted above the real one become the section under test - so a full decoy
    /// route table above the real one hid an RA-8 row deleted from the real one. Each section is
    /// therefore required to be unique.
    /// </para>
    /// <para>
    /// The item COUNT is not an enumeration and cannot be: the worksheet grows. It is held by
    /// agreement between two documents instead - the worksheet's own progress table and the count
    /// HUMAN_REVIEW.md states - so no single-file edit can drop an item and stay green. Exclusion
    /// EX-55 records what that does not reach.
    /// </para>
    /// </remarks>
    [Fact]
    public void H3_The_Review_Route_Is_Covered_Contiguous_And_Counted()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H3",
            "fixed enumeration",
            "EX-55");

        Assert.Empty(RouteViolations(HumanReview));
        Assert.Empty(WorksheetViolations(Worksheet));
        Assert.Empty(ItemCountViolations(HumanReview, Worksheet));

        // Clause: section 4 declares exactly the eight.
        var route = Assert.Single(RouteViolations(Witness("H3-route-table-omits-an-area.md.witness")));
        Assert.Contains("section 4 does not declare RA-8", route, StringComparison.Ordinal);

        // Clause: section 8 declares exactly the eight.
        var verdicts = Assert.Single(RouteViolations(Witness("H3-verdict-table-omits-an-area.md.witness")));
        Assert.Contains("section 8 does not declare RA-8", verdicts, StringComparison.Ordinal);

        // Clause: EXACTLY the eight - a ninth area declared in the route table is a violation too.
        // This is the clause that keeps the fixed enumeration self-checking against the document.
        var ninth = Assert.Single(RouteViolations(Witness("H3-route-table-declares-a-ninth-area.md.witness")));
        Assert.Contains(
            "section 4 declares RA-9, which is not one of the eight review areas",
            ninth,
            StringComparison.Ordinal);

        // Clause: no area is declared twice.
        var repeatedArea = Assert.Single(RouteViolations(Witness("H3-verdict-table-declares-an-area-twice.md.witness")));
        Assert.Contains("section 8 declares RA-8 more than once", repeatedArea, StringComparison.Ordinal);

        // Clause: exactly one route section, and exactly one verdict section.
        var twoRoutes = Assert.Single(RouteViolations(Witness("H3-route-section-appears-twice.md.witness")));
        Assert.Contains("carries 2 'section 4 route' headings", twoRoutes, StringComparison.Ordinal);

        var twoVerdicts = Assert.Single(RouteViolations(Witness("H3-verdict-section-appears-twice.md.witness")));
        Assert.Contains("carries 2 'section 8 area verdict' headings", twoVerdicts, StringComparison.Ordinal);

        // Clause: every area has at least one item.
        var uncovered = Assert.Single(WorksheetViolations(Witness("H3-area-with-no-worksheet-item.md.witness")));
        Assert.Contains("has no item for review area RA-8", uncovered, StringComparison.Ordinal);

        // Clause: every item names an area in the enumeration.
        var foreignArea = Assert.Single(WorksheetViolations(
            Witness("H3-item-names-an-area-outside-the-enumeration.md.witness")));
        Assert.Contains("item RC-09 names RA-9", foreignArea, StringComparison.Ordinal);

        // Clause: an item names the area of the heading it sits under. Without this the worksheet
        // can be left self-contradictory - an item filed under RA-8 declaring RA-7 - with every
        // count still true.
        var misfiled = Assert.Single(WorksheetViolations(
            Witness("H3-item-contradicts-its-area-heading.md.witness")));
        Assert.Contains("item RC-09 sits under RA-8 but names RA-7", misfiled, StringComparison.Ordinal);

        // Clause: the identifiers are contiguous from RC-01.
        var gap = Assert.Single(WorksheetViolations(Witness("H3-item-identifiers-skip-a-number.md.witness")));
        Assert.Contains("item RC-09 appears where RC-08 was expected", gap, StringComparison.Ordinal);

        // Clause: no identifier is used twice. The positional check fires too, so the duplicate
        // message is singled out by name rather than by being the only violation.
        var repeated = WorksheetViolations(Witness("H3-item-identifier-is-used-twice.md.witness"));
        Assert.Single(repeated.Where(static violation =>
            violation.Contains("declares item identifier RC-07 more than once", StringComparison.Ordinal)));

        // Clause: every item declares an Area row.
        var noArea = Assert.Single(WorksheetViolations(Witness("H3-item-declares-no-area-row.md.witness")));
        Assert.Contains("item RC-03 declares no Area row", noArea, StringComparison.Ordinal);

        // Clause: no item repeats the Area row.
        var twoAreas = Assert.Single(WorksheetViolations(Witness("H3-item-declares-two-area-rows.md.witness")));
        Assert.Contains("item RC-03 declares the Area row 2 times", twoAreas, StringComparison.Ordinal);

        // Clause: an Area row is a two-cell row, read with or without the optional trailing pipe.
        // Stripping that pipe is what makes the row two cells rather than three, so this input
        // stops being read at all if the strip is removed - and so does every row of the real
        // worksheet.
        Assert.Empty(WorksheetViolations(Witness("H3-item-area-row-without-a-trailing-pipe.md.witness")));

        // Clause: the per-area counts in the progress table are true.
        var count = Assert.Single(WorksheetViolations(Witness("H3-progress-count-is-wrong.md.witness")));
        Assert.Contains("progress table gives RA-4 2 items; the worksheet declares 1", count, StringComparison.Ordinal);

        // Clause: the progress table gives a row for every area.
        var noRow = Assert.Single(WorksheetViolations(Witness("H3-progress-table-omits-a-row.md.witness")));
        Assert.Contains("progress table has no row for RA-8", noRow, StringComparison.Ordinal);

        // Clause: the total in the progress table is true.
        var total = Assert.Single(WorksheetViolations(Witness("H3-progress-total-is-wrong.md.witness")));
        Assert.Contains("progress table gives a total of 9 items; the worksheet declares 8", total, StringComparison.Ordinal);

        // Clause: the progress table gives a total at all.
        var noTotal = Assert.Single(WorksheetViolations(Witness("H3-progress-table-records-no-total.md.witness")));
        Assert.Contains("progress table records no total", noTotal, StringComparison.Ordinal);

        // Clause: exactly one progress section.
        var twoProgress = Assert.Single(WorksheetViolations(Witness("H3-progress-section-appears-twice.md.witness")));
        Assert.Contains("carries 2 'Progress' headings", twoProgress, StringComparison.Ordinal);

        // Clause: the count HUMAN_REVIEW.md states is the number of items the worksheet carries,
        // so deleting an item and correcting the worksheet's own two number cells is not enough.
        var wrongCount = Assert.Single(ItemCountViolations(
            Witness("H3-record-states-a-wrong-item-count.md.witness"), Worksheet));
        Assert.Contains("states that the worksheet carries 52 items", wrongCount, StringComparison.Ordinal);
        Assert.Contains("docs/review/vm-0-vm-1.md declares 53", wrongCount, StringComparison.Ordinal);

        // Clause: the anti-deletion half. Removing the sentence must not be a way past the check.
        var noCount = Assert.Single(ItemCountViolations(
            Witness("H3-record-states-no-item-count.md.witness"), Worksheet));
        Assert.Contains("states no worksheet item count", noCount, StringComparison.Ordinal);
    }

    /// <summary>The eight review areas, risk-ordered. A fixed enumeration, never derived.</summary>
    private static readonly string[] ReviewAreas =
        ["RA-1", "RA-2", "RA-3", "RA-4", "RA-5", "RA-6", "RA-7", "RA-8"];

    private static readonly Regex RouteSection = new(@"^##\s+4\.\s", RegexOptions.Compiled);
    private static readonly Regex VerdictSection = new(@"^##\s+8\.\s", RegexOptions.Compiled);
    private static readonly Regex ProgressSection = new(@"^##\s+Progress\s*$", RegexOptions.Compiled);
    private static readonly Regex AreaIdentifier = new(@"^RA-\d+$", RegexOptions.Compiled);
    private static readonly Regex AreaInCell = new(@"RA-\d+", RegexOptions.Compiled);
    private static readonly Regex ItemHeading = new(@"^###\s+(?<id>RC-\d+)\b", RegexOptions.Compiled);
    private static readonly Regex AreaHeading = new(@"^##\s+(?<area>RA-\d+)\b", RegexOptions.Compiled);

    /// <summary>The sentence in HUMAN_REVIEW.md section 4 that states how large the worksheet is.</summary>
    private static readonly Regex StatedItemCount =
        new(@"\bworksheet\s+carries\s+(?<count>\d+)\s+items\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static List<string> RouteViolations(ReviewDocument document)
    {
        var violations = new List<string>();

        Compare(RouteSection, "section 4 route", "section 4");
        Compare(VerdictSection, "section 8 area verdict", "section 8");

        return violations;

        void Compare(Regex heading, string sectionLabel, string label)
        {
            var declared = DeclaredAreas(document, heading, sectionLabel, violations);

            violations.AddRange(ReviewAreas
                .Where(area => !declared.Contains(area, StringComparer.Ordinal))
                .Select(area => $"{document.Name} {label} does not declare {area}"));

            violations.AddRange(declared
                .Distinct(StringComparer.Ordinal)
                .Where(area => !ReviewAreas.Contains(area, StringComparer.Ordinal))
                .Select(area => $"{document.Name} {label} declares {area}, which is not one of the eight review areas"));

            violations.AddRange(declared
                .GroupBy(static area => area, StringComparer.Ordinal)
                .Where(static group => group.Count() > 1)
                .Select(group => $"{document.Name} {label} declares {group.Key} more than once"));
        }
    }

    private static List<string> DeclaredAreas(
        ReviewDocument document,
        Regex heading,
        string label,
        List<string> violations)
    {
        var declared = new List<string>();

        foreach (var line in Section(document, heading, label, violations))
        {
            if (!line.IsTableRow)
            {
                continue;
            }

            var first = Unquote(TableCells(line.Text).FirstOrDefault() ?? string.Empty);

            if (AreaIdentifier.IsMatch(first))
            {
                declared.Add(first);
            }
        }

        return declared;
    }

    private static List<string> WorksheetViolations(ReviewDocument document)
    {
        var violations = new List<string>();
        var items = WorksheetItems(document.Lines);

        foreach (var item in items)
        {
            if (item.Areas.Count == 0)
            {
                violations.Add($"{document.Name} item {item.Id} declares no Area row");
            }
            else if (item.Areas.Count > 1)
            {
                violations.Add($"{document.Name} item {item.Id} declares the Area row {item.Areas.Count} times");
            }

            violations.AddRange(item.Areas
                .Distinct(StringComparer.Ordinal)
                .Where(area => !ReviewAreas.Contains(area, StringComparer.Ordinal))
                .Select(area => $"{document.Name} item {item.Id} names {area}, which is not one of the eight review areas"));

            // An item that names an area outside the enumeration is already reported above; saying
            // it twice would make one defect look like two and would break the one-clause-per-
            // witness assertions. The contradiction clause is about a VALID area filed under the
            // wrong heading, which no other clause can see.
            violations.AddRange(item.Areas
                .Distinct(StringComparer.Ordinal)
                .Where(area => ReviewAreas.Contains(area, StringComparer.Ordinal))
                .Where(area => item.Heading is not null && !string.Equals(area, item.Heading, StringComparison.Ordinal))
                .Select(area => $"{document.Name} item {item.Id} sits under {item.Heading} but names {area}"));
        }

        violations.AddRange(items
            .GroupBy(static item => item.Id, StringComparer.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(group => $"{document.Name} declares item identifier {group.Key} more than once"));

        for (var index = 0; index < items.Count; index++)
        {
            var expected = $"RC-{index + 1:D2}";

            if (!string.Equals(items[index].Id, expected, StringComparison.Ordinal))
            {
                violations.Add($"{document.Name} item {items[index].Id} appears where {expected} was expected");
            }
        }

        var covered = items
            .SelectMany(static item => item.Areas)
            .ToHashSet(StringComparer.Ordinal);

        violations.AddRange(ReviewAreas
            .Where(area => !covered.Contains(area))
            .Select(area => $"{document.Name} has no item for review area {area}"));

        var (perArea, total) = ProgressCounts(document, violations);

        foreach (var area in ReviewAreas)
        {
            var actual = items.Count(item => item.Areas.Contains(area, StringComparer.Ordinal));

            if (!perArea.TryGetValue(area, out var declared))
            {
                violations.Add($"{document.Name} progress table has no row for {area}");
            }
            else if (declared != actual)
            {
                violations.Add($"{document.Name} progress table gives {area} {declared} items; the worksheet declares {actual}");
            }
        }

        if (total is null)
        {
            violations.Add($"{document.Name} progress table records no total");
        }
        else if (total != items.Count)
        {
            violations.Add($"{document.Name} progress table gives a total of {total} items; the worksheet declares {items.Count}");
        }

        return violations;
    }

    /// <summary>
    /// The one clause of H3 that reads two documents at once. The number of worksheet items is not
    /// an enumeration and cannot be - the worksheet grows - so it is held by agreement between the
    /// review record and the worksheet instead. Without it, H3 clause 5 is entirely
    /// self-referential: deleting an item and editing the worksheet's own two number cells leaves
    /// every count true and every identifier contiguous.
    /// </summary>
    private static List<string> ItemCountViolations(ReviewDocument record, ReviewDocument worksheet)
    {
        var violations = new List<string>();
        var items = WorksheetItems(worksheet.Lines).Count;

        // Read from the rejoined paragraph rather than the raw line, because the review record is
        // hard-wrapped and the sentence is longer than its column width.
        var stated = LogicalLines(record.Lines)
            .SelectMany(static line => StatedItemCount.Matches(line).Select(
                static match => int.Parse(match.Groups["count"].Value, CultureInfo.InvariantCulture)))
            .Distinct()
            .ToArray();

        if (stated.Length == 0)
        {
            violations.Add(
                $"{record.Name} states no worksheet item count, so nothing outside {worksheet.Name} " +
                $"holds it to the {items} items it carries");

            return violations;
        }

        violations.AddRange(stated
            .Where(count => count != items)
            .Select(count =>
                $"{record.Name} states that the worksheet carries {count} items; {worksheet.Name} declares {items}"));

        return violations;
    }

    private static IReadOnlyList<WorksheetItem> WorksheetItems(IReadOnlyList<SourceLine> lines)
    {
        var items = new List<WorksheetItem>();
        string? current = null;
        string? area = null;
        var areas = new List<string>();

        foreach (var line in lines)
        {
            var depth = HashCount(line.Text);

            if (depth > 0)
            {
                var heading = ItemHeading.Match(line.Text);

                if (heading.Success)
                {
                    Flush();
                    current = heading.Groups["id"].Value;
                    continue;
                }

                if (depth > 3)
                {
                    continue;
                }

                Flush();

                // The area heading an item sits under is remembered so that an item filed under
                // one heading while naming another is visible. Without it the worksheet's own
                // "## RA-n" headings are bound to nothing and the document can be left
                // self-contradictory with every count still true.
                if (depth <= 2)
                {
                    var section = AreaHeading.Match(line.Text);
                    area = section.Success ? section.Groups["area"].Value : null;
                }

                continue;
            }

            if (current is null || !line.IsTableRow)
            {
                continue;
            }

            // An Area row is a two-cell table row. Reading it through the cell splitter rather
            // than a bespoke regex is what binds the optional-trailing-pipe handling to something
            // real: without it, every row of the worksheet parses as three cells and no item
            // declares an area at all.
            var cells = TableCells(line.Text).ToArray();

            if (cells.Length == 2 && string.Equals(Plain(cells[0]), "Area", StringComparison.Ordinal))
            {
                areas.Add(Unquote(cells[1]));
            }
        }

        Flush();

        return items;

        void Flush()
        {
            if (current is not null)
            {
                items.Add(new WorksheetItem(current, area, areas.ToArray()));
            }

            current = null;
            areas = [];
        }
    }

    private static (Dictionary<string, int> PerArea, int? Total) ProgressCounts(
        ReviewDocument document,
        List<string> violations)
    {
        var perArea = new Dictionary<string, int>(StringComparer.Ordinal);
        int? total = null;

        foreach (var line in Section(document, ProgressSection, "Progress", violations))
        {
            if (!line.IsTableRow)
            {
                continue;
            }

            var cells = TableCells(line.Text).ToArray();

            if (cells.Length < 3)
            {
                continue;
            }

            var count = ParseCount(cells[2]);

            if (count is null)
            {
                continue;
            }

            var area = AreaInCell.Match(Unquote(cells[0]));

            if (area.Success)
            {
                // Plain assignment was last-wins: a false row placed ABOVE the true one was
                // overwritten and never compared, so the table a reviewer reads could publish
                // any count at all. A repeated key is itself the violation.
                if (perArea.TryGetValue(area.Value, out var already))
                {
                    violations.Add(
                        $"{document.Name} progress table records {area.Value} twice, as {already} and as {count.Value}");
                    continue;
                }

                perArea[area.Value] = count.Value;
                continue;
            }

            if (string.Equals(Plain(cells[1]), "Total", StringComparison.OrdinalIgnoreCase))
            {
                if (total is not null)
                {
                    violations.Add(
                        $"{document.Name} progress table records a total twice, as {total} and as {count.Value}");
                    continue;
                }

                total = count.Value;
            }
        }

        return (perArea, total);
    }

    private static int? ParseCount(string cell) =>
        int.TryParse(Plain(cell), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;

    private sealed record WorksheetItem(string Id, string? Heading, IReadOnlyList<string> Areas);

    // =====================================================================================
    // H4 - an unsigned attestation cannot record an approval
    // =====================================================================================

    /// <summary>
    /// H4. If HUMAN_REVIEW.md records any decision other than <c>PENDING</c>, every field of its
    /// human attestation is filled; and exactly one decision is recorded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The four field names are a fixed enumeration, so a MISSING field is a violation. An earlier
    /// attempt read whatever bullets happened to be present and failed only when the list was
    /// empty, which made deleting the signature line as good as filling it - the cheapest possible
    /// way to record a false approval was the one path the rule could not see.
    /// </para>
    /// <para>
    /// Both markdown italic forms count as placeholders. Only <c>_..._</c> was recognised before,
    /// and <c>*not yet signed*</c> renders identically. A value carrying no letter and no digit -
    /// a bare hyphen, say - is a placeholder too: the closed word list cannot enumerate every way
    /// of writing nothing, and an approval attested with four hyphens is the outcome this rule
    /// exists to refuse.
    /// </para>
    /// <para>
    /// The decision section is required to be unique. A lookup that took its first match let a
    /// decoy heading above the real one - <c>## Decision vocabulary</c>, carrying one ticked
    /// PENDING box - become the section under test, so an APPROVED record with four unsigned
    /// fields passed.
    /// </para>
    /// <para>
    /// This is the false-approval risk ADR 0001 named when it deferred HUMAN_REVIEW.md to VM-6.
    /// </para>
    /// </remarks>
    [Fact]
    public void H4_An_Unsigned_Attestation_Cannot_Record_An_Approval()
    {
        AssertTheRegisterRowStatesItsLimits(
            "H4",
            "fixed enumeration",
            "at least one letter or digit");

        Assert.Empty(AttestationViolations(HumanReview));

        // The rule must not reject a complete approval, or it says nothing about an incomplete one.
        Assert.Empty(AttestationViolations(Witness("H4-approved-with-a-complete-attestation.md.witness")));

        // Clause: a field that is absent entirely.
        var absent = Assert.Single(AttestationViolations(Witness("H4-approved-with-a-missing-field.md.witness")));
        Assert.Contains("attestation field 'Signature or attributable commit' is missing", absent, StringComparison.Ordinal);
        Assert.Contains("APPROVED FOR PREVIEW", absent, StringComparison.Ordinal);

        // Clause: the enumeration's second member, which every other H4 witness fills.
        var alias = Assert.Single(AttestationViolations(Witness("H4-approved-with-no-reviewer-alias.md.witness")));
        Assert.Contains("attestation field 'Reviewer alias' is missing", alias, StringComparison.Ordinal);

        // Clause: a field present with no value.
        var empty = Assert.Single(AttestationViolations(Witness("H4-approved-with-an-empty-field.md.witness")));
        Assert.Contains("attestation field 'Date' is empty", empty, StringComparison.Ordinal);

        // Clause: a field whose value carries no letter and no digit.
        var hyphens = AttestationViolations(Witness("H4-approved-with-hyphens-for-fields.md.witness"));
        Assert.Equal(4, hyphens.Count);
        foreach (var field in AttestationFields)
        {
            Assert.Single(hyphens.Where(violation => violation.Contains(
                $"attestation field '{field}' has no letter or digit in it: -", StringComparison.Ordinal)));
        }

        // Clause: the underscore italic form.
        var underscore = Assert.Single(AttestationViolations(Witness("H4-approved-with-an-underscore-placeholder.md.witness")));
        Assert.Contains("attestation field 'Name' is a placeholder: _Maik Ratzmer_", underscore, StringComparison.Ordinal);

        // Clause: the asterisk italic form, which renders identically.
        var asterisk = Assert.Single(AttestationViolations(Witness("H4-approved-with-an-asterisk-placeholder.md.witness")));
        Assert.Contains("attestation field 'Name' is a placeholder: *Maik Ratzmer*", asterisk, StringComparison.Ordinal);

        // Clause: the placeholder word list, case-insensitively - one assertion per word, because
        // an unexercised word can be deleted from the array with the suite green, and the word
        // most likely to appear in a false approval is the one the live document already uses.
        AssertPlaceholderWord("H4-approved-with-a-word-placeholder.md.witness", "Date", "TBD");
        AssertPlaceholderWord("H4-approved-with-word-placeholders.md.witness", "Name", "None");
        AssertPlaceholderWord("H4-approved-with-word-placeholders.md.witness", "Reviewer alias", "N/A");
        AssertPlaceholderWord("H4-approved-with-word-placeholders.md.witness", "Signature or attributable commit", "Pending");
        AssertPlaceholderWord("H4-approved-with-word-placeholders.md.witness", "Date", "not yet signed");
        AssertPlaceholderWord("H4-approved-with-more-word-placeholders.md.witness", "Name", "Not yet performed");
        AssertPlaceholderWord("H4-approved-with-more-word-placeholders.md.witness", "Reviewer alias", "not yet assigned");
        AssertPlaceholderWord(
            "H4-approved-with-more-word-placeholders.md.witness",
            "Signature or attributable commit",
            "To be recorded by the reviewer");

        // Clause: at most one decision is recorded.
        var two = Assert.Single(AttestationViolations(Witness("H4-two-decisions-are-recorded.md.witness")));
        Assert.Contains("ticks 2 decision boxes; exactly one is required", two, StringComparison.Ordinal);

        // Clause: at least one decision is recorded.
        var none = Assert.Single(AttestationViolations(Witness("H4-no-decision-is-recorded.md.witness")));
        Assert.Contains("ticks 0 decision boxes; exactly one is required", none, StringComparison.Ordinal);

        // Clause: exactly one decision section. A decoy heading above the real one carrying a
        // ticked PENDING box was enough to make an APPROVED, wholly unsigned record pass.
        var twoDecisions = AttestationViolations(Witness("H4-decision-section-appears-twice.md.witness"));
        Assert.Single(twoDecisions.Where(static violation =>
            violation.Contains("carries 2 'decision' headings", StringComparison.Ordinal)));

        // Clause: exactly one attestation section.
        var twoAttestations = Assert.Single(AttestationViolations(
            Witness("H4-attestation-section-appears-twice.md.witness")));
        Assert.Contains("carries 2 'human attestation' headings", twoAttestations, StringComparison.Ordinal);
    }

    private static void AssertPlaceholderWord(string witness, string field, string value)
    {
        var violations = AttestationViolations(Witness(witness));

        Assert.Single(violations.Where(violation => violation.Contains(
            $"attestation field '{field}' is a placeholder: {value}", StringComparison.Ordinal)));
    }

    /// <summary>The four attestation field names, as a fixed enumeration.</summary>
    private static readonly string[] AttestationFields =
        ["Name", "Reviewer alias", "Signature or attributable commit", "Date"];

    /// <summary>
    /// The placeholder words, matched case-insensitively against the field value once markdown
    /// emphasis and backticks are stripped from it.
    /// </summary>
    private static readonly string[] PlaceholderWords =
    [
        "not yet signed", "not yet performed", "not yet assigned", "none",
        "n/a", "tbd", "to be recorded by the reviewer", "pending",
    ];

    private static readonly Regex DecisionSection =
        new(@"^##\s+\d*\.?\s*Decision\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AttestationSection =
        new(@"^##\s+\d*\.?\s*Human Attestation\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex DecisionBox =
        new(@"^(?:[-*+]|\d+[.)])\s+\[(?<box>[ xX])\]\s*(?<label>.*)$", RegexOptions.Compiled);

    private static readonly Regex AttestationField =
        new(@"^(?:[-*+]|\d+[.)])\s+\*\*(?<field>[^*:]+):\*\*(?<value>.*)$", RegexOptions.Compiled);

    private static readonly Regex BoldLabel = new(@"\*\*(?<label>[^*]+)\*\*", RegexOptions.Compiled);

    private static List<string> AttestationViolations(ReviewDocument document)
    {
        var violations = new List<string>();
        var ticked = TickedDecisions(document, violations);

        if (ticked.Count != 1)
        {
            violations.Add($"{document.Name} ticks {ticked.Count} decision boxes; exactly one is required");
        }

        // The field requirement is conditional: PENDING is the true current decision and is not an
        // approval, so an unfilled attestation under PENDING is honest rather than false.
        if (ticked.Count != 1 || IsPending(ticked[0]))
        {
            return violations;
        }

        var decision = ticked[0];
        var fields = AttestationFieldValues(document, violations);

        foreach (var field in AttestationFields)
        {
            if (!fields.TryGetValue(field, out var value))
            {
                violations.Add($"{document.Name} records decision {decision} but attestation field '{field}' is missing");
                continue;
            }

            var trimmed = value.Trim();

            if (trimmed.Length == 0)
            {
                violations.Add($"{document.Name} records decision {decision} but attestation field '{field}' is empty");
                continue;
            }

            if (!Plain(trimmed).Any(char.IsLetterOrDigit))
            {
                violations.Add(
                    $"{document.Name} records decision {decision} but attestation field '{field}' has no letter " +
                    $"or digit in it: {trimmed}");
                continue;
            }

            if (IsPlaceholder(trimmed))
            {
                violations.Add(
                    $"{document.Name} records decision {decision} but attestation field '{field}' is a placeholder: {trimmed}");
            }
        }

        return violations;
    }

    private static List<string> TickedDecisions(ReviewDocument document, List<string> violations)
    {
        var ticked = new List<string>();

        foreach (var line in Section(document, DecisionSection, "decision", violations))
        {
            var box = DecisionBox.Match(line.Text);

            if (!box.Success || box.Groups["box"].Value == " ")
            {
                continue;
            }

            var label = box.Groups["label"].Value.Trim();
            var bold = BoldLabel.Match(label);

            ticked.Add((bold.Success ? bold.Groups["label"].Value : label).Trim().TrimEnd('.').Trim());
        }

        return ticked;
    }

    private static Dictionary<string, string> AttestationFieldValues(
        ReviewDocument document,
        List<string> violations)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in Section(document, AttestationSection, "human attestation", violations))
        {
            var field = AttestationField.Match(line.Text);

            if (field.Success)
            {
                fields[field.Groups["field"].Value.Trim()] = field.Groups["value"].Value.Trim();
            }
        }

        return fields;
    }

    /// <summary>
    /// The decision is PENDING only when the label IS the word, not when it merely contains it.
    /// A substring test let "APPROVED FOR PREVIEW - conditions PENDING" take the early exit and
    /// skip every attestation check, which recorded an approval over four unsigned fields with
    /// the suite green - the exact false approval this rule exists to refuse.
    /// </summary>
    private static bool IsPending(string decision) =>
        string.Equals(Plain(decision).Trim(), "PENDING", StringComparison.OrdinalIgnoreCase);

    private static bool IsPlaceholder(string value)
    {
        // Both markdown italic forms, and only the italic forms: **bold** is not emphasis-as-excuse
        // and a bolded real name must not be read as an unfilled field.
        if (IsItalic(value, '_') || IsItalic(value, '*'))
        {
            return true;
        }

        return PlaceholderWords.Contains(Plain(value).TrimEnd('.').Trim(), StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsItalic(string value, char marker) =>
        value.Length >= 3 &&
        value[0] == marker &&
        value[^1] == marker &&
        value[1] != marker &&
        value[^2] != marker;

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
            "EX-56");

        var current = Figures(CurrentBundle);

        Assert.NotEmpty(current.PerAssembly);
        Assert.NotNull(current.Architecture);
        Assert.NotNull(current.Behavioural);
        Assert.NotNull(current.NativeImageSize);
        Assert.NotNull(current.TrimmedImageSize);

        Assert.Empty(Corpus.SelectMany(document => FigureViolations(document, FiguresFor(document))));
        Assert.Empty(RetainedFigureGuard(Corpus, current));

        // Clause: a quoted suite total is compared against the per-assembly totals and their sum.
        var total = Assert.Single(FigureViolations(
            Witness("H5-suite-total-does-not-match-the-log.md.witness"), current));
        Assert.Contains("quotes a suite total of 221", total, StringComparison.Ordinal);

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
        Assert.Contains("quotes a split of 89 architecture and 130 behavioural", split, StringComparison.Ordinal);

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
            "quotes a Native AOT image size of 162816 bytes", StringComparison.Ordinal)));
        Assert.Single(swapped.Where(static violation => violation.Contains(
            "quotes a trimmed image size of 1279488 bytes", StringComparison.Ordinal)));

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
        Assert.Contains("no review document quotes the current suite total 220", noTotal, StringComparison.Ordinal);

        // Clause: the corpus must still quote the split, comparing the values.
        var noSplit = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-split.md.witness")], current));
        Assert.Contains("split of 89 architecture and 131 behavioural", noSplit, StringComparison.Ordinal);

        // Clause: the corpus must still quote the Native AOT image size, comparing the value.
        var noNative = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-native-image-size.md.witness")], current));
        Assert.Contains("Native AOT image size 1279488 bytes", noNative, StringComparison.Ordinal);

        // Clause: and the trimmed size, which had no guard at all - so nothing forced the corpus
        // to keep quoting it correctly, or at all, once it had been reworded.
        var noTrimmed = Assert.Single(RetainedFigureGuard(
            [Witness("H5-corpus-omits-the-trimmed-image-size.md.witness")], current));
        Assert.Contains("trimmed image size 162816 bytes", noTrimmed, StringComparison.Ordinal);
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
        long? TrimmedImageSize);

    // These three are declared ahead of BundleFigures on purpose: static field initializers run in
    // textual order, and LoadBundleFigures reads all three while the type initializer is running.
    private static readonly Regex PassedPerAssembly = new(
        @"Passed:\s*(?<count>\d+).*?-\s*(?<assembly>[A-Za-z0-9_.]+\.dll)",
        RegexOptions.Compiled);

    private static readonly Regex NativeImageSizeInLog =
        new(@"native image size:\s*(?<size>[\d.,]+)\s*bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ImageSizeInLog =
        new(@"(?:^|\s)image size:\s*(?<size>[\d.,]+)\s*bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
                TrimmedImageSize: SizeFromLog(Path.Combine(directory, "publish-jit-and-trimmed.log"), ImageSizeInLog));
        }

        return figures;
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
            if (size != figures.NativeImageSize)
            {
                violations.Add(
                    $"{document.Name} quotes a Native AOT image size of {size} bytes; " +
                    $"docs/evidence/{figures.Bundle}/publish-aot.log records {Show(figures.NativeImageSize)}");
            }
        }

        foreach (var size in TrimmedSizes(document).Distinct())
        {
            if (size != figures.TrimmedImageSize)
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
