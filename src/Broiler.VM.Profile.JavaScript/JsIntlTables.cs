// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           8
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The internationalization tables a composition handed over, decoded once and shared by every
/// runtime its descriptor makes (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Each table is decoded the first time a guest needs it</b>, under a thread-safe lazy: a
/// descriptor's runtimes may run on several threads, and the tables never change, so the first
/// decode is the only one and every later read is a dictionary lookup.
/// </para>
/// <para>
/// <b>Nothing here is guest-visible state.</b> The decoded tables are the composition's data in a
/// shape the profile can search; no realm, engine or guest value is held, so sharing them across
/// runtimes shares nothing a guest could write.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BAE255
// Broiler-Human:        PENDING
internal sealed class JsIntlTables
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0582E3
    // Broiler-Human:        PENDING
    private readonly Format.IJsIntlData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=678323
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, string>> likely;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E9ED15
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>> aliases;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F8026D
    // Broiler-Human:        PENDING
    private readonly System.Lazy<System.Collections.Generic.Dictionary<string, string>> extensions;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=38DA77
    // Broiler-Human:        PENDING
    private readonly System.Lazy<string[]> locales;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1EBC74
    // Broiler-Human:        PENDING
    private readonly System.Lazy<JsCollationData> collation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B4D744
    // Broiler-Human:        PENDING
    private readonly System.Lazy<(int First, int Last)[]> softDotted;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0DA634
    // Broiler-Human:        PENDING
    internal JsIntlTables(Format.IJsIntlData data)
    {
        this.data = data;
        likely = new(() => Pairs(Format.JsIntlTable.LikelySubtags));
        aliases = new(ReadAliases);
        extensions = new(ReadExtensions);
        locales = new(() => [.. Lines(Format.JsIntlTable.Locales)]);
        collation = new(() => new JsCollationData(
            data.Table(Format.JsIntlTable.CollationRoot), data.Table(Format.JsIntlTable.CollationTailorings)));
        softDotted = new(ReadRanges);
    }

    /// <summary>The CLDR release the tables come from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B6F048
    // Broiler-Human:        PENDING
    internal string CldrVersion => data.CldrVersion;

    /// <summary>The locales the data supports, in canonical form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F9D74C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> Locales => locales.Value;

    /// <summary>The collation data.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B35C2E
    // Broiler-Human:        PENDING
    internal JsCollationData Collation => collation.Value;

    /// <summary>The likely subtags of a source in CLDR's form - <c>en</c>, <c>und-Latn</c>, <c>und-US</c> - or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E6580E
    // Broiler-Human:        PENDING
    internal string? Likely(string source) => likely.Value.TryGetValue(source, out var target) ? target : null;

    /// <summary>The alias of <paramref name="from"/> of one kind - language, script, region, variant, subdivision - or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B36EB0
    // Broiler-Human:        PENDING
    internal string? Alias(string kind, string from) =>
        aliases.Value.TryGetValue(kind, out var table) && table.TryGetValue(from, out var replacement) ? replacement : null;

    /// <summary>Every language alias, for the rule matcher that reads them all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=00F4A3
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyDictionary<string, string> LanguageAliases => aliases.Value["language"];

    /// <summary>Whether <paramref name="key"/> is a key of extension <paramref name="singleton"/> the BCP 47 data defines.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8B3C4F
    // Broiler-Human:        PENDING
    internal bool IsKey(char singleton, string key) => extensions.Value.ContainsKey(singleton + "|" + key + "|");

    /// <summary>
    /// Whether <paramref name="type"/> is a type of <paramref name="key"/>, and its preferred form:
    /// itself, or the type a deprecated one was replaced by.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A7A19D
    // Broiler-Human:        PENDING
    internal bool TryType(char singleton, string key, string type, out string preferred)
    {
        if (extensions.Value.TryGetValue(singleton + "|" + key + "|" + type, out var stated))
        {
            preferred = stated.Length == 0 ? type : stated;
            return true;
        }

        preferred = type;
        return false;
    }

    /// <summary>Whether a code point is Soft_Dotted: an <c>i</c> or <c>j</c> whose dot an accent above replaces.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65A410
    // Broiler-Human:        PENDING
    internal bool IsSoftDotted(int codePoint)
    {
        var ranges = softDotted.Value;
        var low = 0;
        var high = ranges.Length - 1;

        while (low <= high)
        {
            var middle = (low + high) >>> 1;

            if (codePoint < ranges[middle].First)
            {
                high = middle - 1;
            }
            else if (codePoint > ranges[middle].Last)
            {
                low = middle + 1;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The lines of a text table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=D7F15D
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> Lines(Format.JsIntlTable table)
    {
        var text = System.Text.Encoding.ASCII.GetString(data.Table(table));
        var lines = new System.Collections.Generic.List<string>();

        foreach (var line in text.Split('\n'))
        {
            if (line.Length != 0)
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    /// <summary>A two-field table as a dictionary.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=041F11
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, string> Pairs(Format.JsIntlTable table)
    {
        var pairs = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var line in Lines(table))
        {
            var bar = line.IndexOf('|');
            pairs[line[..bar]] = line[(bar + 1)..];
        }

        return pairs;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=C4C031
    // Broiler-Human:        PENDING
    private (int First, int Last)[] ReadRanges()
    {
        var lines = Lines(Format.JsIntlTable.SoftDotted);
        var ranges = new (int First, int Last)[lines.Count];

        for (var at = 0; at < lines.Count; at++)
        {
            var bar = lines[at].IndexOf('|');
            ranges[at] = (
                int.Parse(lines[at][..bar], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(lines[at][(bar + 1)..], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture));
        }

        return ranges;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=50C7CB
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> ReadAliases()
    {
        var kinds = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>(System.StringComparer.Ordinal);

        foreach (var kind in new[] { "language", "script", "region", "variant", "subdivision" })
        {
            kinds[kind] = new(System.StringComparer.Ordinal);
        }

        foreach (var line in Lines(Format.JsIntlTable.Aliases))
        {
            var fields = line.Split('|');
            kinds[fields[0]][fields[1]] = fields[2];
        }

        return kinds;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=F3E6EB
    // Broiler-Human:        PENDING
    private System.Collections.Generic.Dictionary<string, string> ReadExtensions()
    {
        var table = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var line in Lines(Format.JsIntlTable.Extensions))
        {
            var fields = line.Split('|');
            table[fields[0] + "|" + fields[1] + "|" + fields[2]] = fields[3];
        }

        return table;
    }
}
