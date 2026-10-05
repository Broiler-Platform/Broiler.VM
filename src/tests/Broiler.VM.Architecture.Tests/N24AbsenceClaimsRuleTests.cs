// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// No document of the JavaScript profile calls a global absent that the realm publishes, in any of
/// the three shapes a claim of absence takes in these documents.
/// </summary>
/// <remarks>
/// <para>
/// <b>Rule N17 decides one document, and the staleness it exists to stop happened in another.</b>
/// N17 compares the ledger's fenced absent-globals block with the set the realm publishes. The
/// workload roadmap's section 3.2 said in prose that <c>Proxy</c> and <c>Reflect</c> were absent after
/// both were present, and nothing checked it (correction JSC-187, and the parity roadmap's section
/// 4.1). The parity roadmap's JSP-1 asks for a rule that "reads every document that claims a name is
/// absent rather than the ledger's block alone, and is watched failing against an injected stale
/// claim". This is that rule, beside N17 rather than inside it, so N17's one-block comparison keeps
/// its own witness.
/// </para>
/// <para>
/// <b>It decides three shapes, and they are the shapes these documents use.</b> An
/// <c>absent-globals</c> fenced block in any document, not only the ledger. A clause that names a
/// global in backticks and then says it "is absent" or "are absent", optionally "still" or "now". And
/// a bullet whose bold lead is a backticked global, under a heading that says "absent". A name counts
/// only when the realm publishes it in <c>docs/realm/globals.txt</c>, so a claim about a genuinely
/// absent global passes, and so does prose that uses the word about anything else.
/// </para>
/// <para>
/// <b>Text a document keeps as it was written is marked, and the mark is dated.</b> These documents
/// do not delete a superseded reading; they keep it and say beside it what replaced it. Such a span
/// sits between <c>&lt;!-- as-written, superseded YYYY-MM-DD --&gt;</c> and
/// <c>&lt;!-- /as-written --&gt;</c>, each on a line of its own. The rule reads nothing inside one, and
/// refuses a marker of any other form, one opened inside another, one never closed and a close with
/// nothing open, so an exemption cannot be taken silently.
/// </para>
/// <para>
/// <b>Two places are outside it, both by what they are.</b> The corrections file's entries state what
/// the plan said so that a reader who planned against it can find the retraction, so every one of
/// them quotes a superseded claim. A retained evidence bundle is immutable. Everything else under the
/// profile's directory that is Markdown is read. <b>STATED LIMIT.</b> A claim phrased in any other
/// way, such as "lacks", "has no" or "answers <c>undefined</c> for", is not seen; the three shapes are
/// the ones the documents used when this rule was written, and a new phrasing is a reason to widen it.
/// </para>
/// </remarks>
public sealed partial class N24AbsenceClaimsRuleTests
{
    /// <summary>The profile's directory, every Markdown file under which is read.</summary>
    private const string ProfileDirectory = "src/Broiler.VM.Profile.JavaScript";

    /// <summary>Where the realm writes what it admits.</summary>
    private const string PublishedPath = "src/Broiler.VM.Profile.JavaScript/docs/realm/globals.txt";

    /// <summary>The fence that opens a machine-readable absent list.</summary>
    private const string AbsentFence = "```absent-globals";

    /// <summary>How far back from "is absent" a clause is read for the names it is about.</summary>
    private const int ClauseWindow = 160;

    /// <summary>Nothing any document of the profile calls absent is a name the realm publishes.</summary>
    [Fact]
    public void N24_No_Document_Calls_A_Present_Global_Absent()
    {
        var published = Published();
        var documents = Documents();

        // Non-vacuous: the three documents that carry absence claims are among those read, and the
        // two places outside the rule are not.
        Assert.Contains("src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md", documents);
        Assert.Contains("src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md", documents);
        Assert.Contains("src/Broiler.VM.Profile.JavaScript/docs/roadmap.workloads.md", documents);
        Assert.DoesNotContain(documents, static path => path.EndsWith("/roadmap.corrections.md", StringComparison.Ordinal));
        Assert.DoesNotContain(documents, static path => path.Contains("/docs/evidence/", StringComparison.Ordinal));

        var findings = documents
            .SelectMany(path => Findings(path, File.ReadAllText(Path.Combine(ComponentGraph.Root, path)), published))
            .ToArray();

        Assert.True(findings.Length == 0, string.Join(Environment.NewLine, findings));
    }

    /// <summary>
    /// The injected stale claims are reported, one per shape, and the ones the rule must pass are not.
    /// </summary>
    [Fact]
    public void N24_Reports_Each_Injected_Stale_Claim_By_Its_Shape()
    {
        var published = Published();
        const string Name = "N24-a-document-says-a-present-global-is-absent.md.witness";
        var path = Path.Combine(ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/witnesses/register", Name);

        Assert.True(File.Exists(path), $"the witness {Name} is not on disk");

        var findings = Findings(Name, File.ReadAllText(path), published);

        // Each shape, reported as the name it is about.
        Assert.Contains(findings, static finding => finding.Contains("a clause calls `Proxy` absent", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("a clause calls `Reflect` absent", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("a bullet under an absent heading leads with `Promise`", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("an absent-globals block names `Object`", StringComparison.Ordinal));

        // Each malformed marker, reported rather than honoured.
        // The undated marker opens nothing, so the close after it is reported as closing nothing.
        Assert.Contains(findings, static finding => finding.Contains("an as-written marker without a date", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("an as-written span closed with none open", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("an as-written span opened inside another", StringComparison.Ordinal));
        Assert.Contains(findings, static finding => finding.Contains("an as-written span that is never closed", StringComparison.Ordinal));

        // And the passing directions: a genuinely absent global, a present one inside a marked span,
        // a present one the clause does not call absent, and a bullet under a heading that is not
        // about absence.
        Assert.DoesNotContain(findings, static finding => finding.Contains("`WebAssembly`", StringComparison.Ordinal));
        Assert.DoesNotContain(findings, static finding => finding.Contains("`Map`", StringComparison.Ordinal));
        Assert.DoesNotContain(findings, static finding => finding.Contains("`Symbol`", StringComparison.Ordinal));
        Assert.DoesNotContain(findings, static finding => finding.Contains("`Array`", StringComparison.Ordinal));
        Assert.Equal(8, findings.Count);
    }

    /// <summary>Every finding one document holds, each naming its file and line.</summary>
    internal static List<string> Findings(string source, string text, IReadOnlySet<string> published)
    {
        var findings = new List<string>();
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var kept = Mask(source, lines, findings);
        var heading = string.Empty;
        var inBlock = false;

        for (var index = 0; index < kept.Length; index++)
        {
            var line = kept[index];
            var trimmed = line.Trim();

            if (trimmed.Equals(AbsentFence, StringComparison.Ordinal))
            {
                inBlock = true;
                continue;
            }

            if (inBlock)
            {
                if (trimmed.Equals("```", StringComparison.Ordinal))
                {
                    inBlock = false;
                }
                else if (published.Contains(trimmed))
                {
                    findings.Add($"{source}:{index + 1}: an absent-globals block names `{trimmed}`, which the realm publishes");
                }

                continue;
            }

            if (line.StartsWith('#'))
            {
                heading = line;
                continue;
            }

            if (heading.Contains("absent", StringComparison.OrdinalIgnoreCase) &&
                LeadBullet().Match(line) is { Success: true } bullet &&
                published.Contains(bullet.Groups[1].Value))
            {
                findings.Add($"{source}:{index + 1}: a bullet under an absent heading leads with `{bullet.Groups[1].Value}`, which the realm publishes");
            }
        }

        // A clause may run across lines, so it is read over each paragraph with its line breaks
        // made spaces; a finding names the line the verb stands on.
        var start = 0;

        for (var index = 0; index <= kept.Length; index++)
        {
            if (index < kept.Length && kept[index].Trim().Length != 0)
            {
                continue;
            }

            if (index > start)
            {
                Clauses(source, kept, start, index, published, findings);
            }

            start = index + 1;
        }

        return findings;
    }

    /// <summary>Every clause of one paragraph that calls a published name absent.</summary>
    private static void Clauses(
        string source, string[] lines, int first, int end, IReadOnlySet<string> published, List<string> findings)
    {
        var paragraph = string.Join(' ', lines[first..end]);

        foreach (Match verb in AbsentVerb().Matches(paragraph))
        {
            var from = Math.Max(0, verb.Index - ClauseWindow);
            var window = paragraph[from..verb.Index];
            var boundary = SentenceEnd().Matches(window).LastOrDefault();

            if (boundary is not null)
            {
                window = window[(boundary.Index + boundary.Length)..];
            }

            var line = LineOf(lines, first, verb.Index);

            foreach (Match name in BacktickedName().Matches(window))
            {
                if (published.Contains(name.Groups[1].Value))
                {
                    findings.Add($"{source}:{line}: a clause calls `{name.Groups[1].Value}` absent, and the realm publishes it");
                }
            }
        }
    }

    /// <summary>The line a paragraph offset falls on, the paragraph's lines having been joined by one space each.</summary>
    private static int LineOf(string[] lines, int first, int offset)
    {
        var at = 0;

        for (var index = first; ; index++)
        {
            at += lines[index].Length + 1;

            if (offset < at || index == lines.Length - 1)
            {
                return index + 1;
            }
        }
    }

    /// <summary>
    /// The document with every as-written span blanked, line for line, and every malformed marker
    /// reported.
    /// </summary>
    private static string[] Mask(string source, string[] lines, List<string> findings)
    {
        var kept = new string[lines.Length];
        var open = -1;

        for (var index = 0; index < lines.Length; index++)
        {
            var trimmed = lines[index].Trim();
            var marker = trimmed.StartsWith("<!--", StringComparison.Ordinal) &&
                trimmed.Contains("as-written", StringComparison.Ordinal);

            if (marker && OpenMarker().IsMatch(trimmed))
            {
                if (open >= 0)
                {
                    findings.Add($"{source}:{index + 1}: an as-written span opened inside another, which opened at line {open + 1}");
                }

                open = index;
                kept[index] = string.Empty;
                continue;
            }

            if (marker && trimmed.Equals("<!-- /as-written -->", StringComparison.Ordinal))
            {
                if (open < 0)
                {
                    findings.Add($"{source}:{index + 1}: an as-written span closed with none open");
                }

                open = -1;
                kept[index] = string.Empty;
                continue;
            }

            if (marker)
            {
                findings.Add($"{source}:{index + 1}: an as-written marker without a date, or of another form: `{trimmed}`");
            }

            kept[index] = open >= 0 ? string.Empty : lines[index];
        }

        if (open >= 0)
        {
            findings.Add($"{source}:{open + 1}: an as-written span that is never closed");
        }

        return kept;
    }

    /// <summary>Every Markdown document under the profile's directory, but the two outside the rule.</summary>
    private static string[] Documents() =>
        Directory.EnumerateFiles(Path.Combine(ComponentGraph.Root, ProfileDirectory), "*.md", SearchOption.AllDirectories)
            .Select(static path => Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'))
            .Where(static path => !path.Contains("/bin/", StringComparison.Ordinal) &&
                                  !path.Contains("/obj/", StringComparison.Ordinal) &&
                                  !path.Contains("/docs/evidence/", StringComparison.Ordinal) &&
                                  !path.EndsWith("/roadmap.corrections.md", StringComparison.Ordinal))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToArray();

    /// <summary>The names the realm publishes.</summary>
    private static HashSet<string> Published()
    {
        var path = Path.Combine(ComponentGraph.Root, PublishedPath);

        Assert.True(File.Exists(path), $"the realm has published nothing at {PublishedPath}");

        var names = File.ReadAllLines(path)
            .Select(static line => line.Trim())
            .Where(static line => line.Length != 0 && !line.StartsWith('#'))
            .ToHashSet(StringComparer.Ordinal);

        // Non-vacuous: the published set is a real one, with the names the witness is written about.
        Assert.Contains("Proxy", names);
        Assert.Contains("Promise", names);
        Assert.Contains("Intl", names);
        Assert.Contains("Temporal", names);
        Assert.DoesNotContain("WebAssembly", names);
        return names;
    }

    [GeneratedRegex(@"\b(?:is|are)\s+(?:still\s+|now\s+)?absent\b")]
    private static partial Regex AbsentVerb();

    [GeneratedRegex(@"[.;:!?](?:\s|$)")]
    private static partial Regex SentenceEnd();

    [GeneratedRegex(@"`([A-Za-z_$][A-Za-z0-9_$]*)`")]
    private static partial Regex BacktickedName();

    [GeneratedRegex(@"^\s*[-*]\s+\*\*`([A-Za-z_$][A-Za-z0-9_$]*)`")]
    private static partial Regex LeadBullet();

    [GeneratedRegex(@"^<!-- as-written, superseded \d{4}-\d{2}-\d{2} -->$")]
    private static partial Regex OpenMarker();
}
