// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           21
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  2/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The number, currency, unit and plural data the composition handed over, decoded once and shared
/// by every runtime its descriptor makes (JSD-0044).
/// </summary>
/// <remarks>
/// <para>
/// <b>Keyed by language</b>: the tables carry <c>de</c> and <c>en</c>, and every supported locale
/// reads its language's data, as CLDR's inheritance gives <c>de-DE</c> and <c>en-US</c> nothing of
/// their own for numbers.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=JSD-0044; IP=Low; Security=Low; Resources=2; Fingerprint=FF77C6
// Broiler-Human:        PENDING
internal sealed class JsNumberData
{
    /// <summary>Each language's flattened number data, by dotted key.</summary>
    internal System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> Locales { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Each language's currency names, by <c>language|code</c>.</summary>
    internal System.Collections.Generic.Dictionary<string, JsCurrencyNames> Currencies { get; } = new(System.StringComparer.Ordinal);

    /// <summary>The currencies whose fraction digits are not 2.</summary>
    internal System.Collections.Generic.Dictionary<string, int> CurrencyDigits { get; } = new(System.StringComparer.Ordinal);

    /// <summary>The numbering systems with a simple digit mapping, each digit a string of one code point.</summary>
    internal System.Collections.Generic.Dictionary<string, string[]> NumberingSystems { get; } = new(System.StringComparer.Ordinal);

    /// <summary>Each language's cardinal plural rules, in the table's order.</summary>
    internal System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(string Category, JsPluralRule Rule)>> Plurals { get; } =
        new(System.StringComparer.Ordinal);

    /// <summary>Each language's plural range results, by <c>language|start|end</c>.</summary>
    internal System.Collections.Generic.Dictionary<string, string> PluralRanges { get; } = new(System.StringComparer.Ordinal);

    /// <summary>Each language's unit patterns, by <c>language|width|unit|field</c>.</summary>
    internal System.Collections.Generic.Dictionary<string, string> Units { get; } = new(System.StringComparer.Ordinal);

    /// <summary>One value of a language's number data, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=31907F
    // Broiler-Human:        PENDING
    internal string? Value(string language, string key) =>
        Locales.TryGetValue(language, out var locale) && locale.TryGetValue(key, out var value) ? value : null;

    /// <summary>The plural category of a formatted number in a language: the first rule that holds, or <c>other</c>.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s5.1; IP=Low; Security=Low; Resources=1; Fingerprint=928AE2
    // Broiler-Human:        PENDING
    internal string Plural(string language, JsPluralOperands operands)
    {
        if (Plurals.TryGetValue(language, out var rules))
        {
            foreach (var (category, rule) in rules)
            {
                if (category != "other" && rule.Holds(operands))
                {
                    return category;
                }
            }
        }

        return "other";
    }

    /// <summary>The plural category of a range, by the language's range rules, or <c>other</c> where they name none, as ICU resolves one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B2FD5F
    // Broiler-Human:        PENDING
    internal string PluralRange(string language, string start, string end) =>
        PluralRanges.TryGetValue(language + "|" + start + "|" + end, out var result) ? result : "other";
}

/// <summary>A currency's names in one language; an empty field is one CLDR does not give.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=44E511
// Broiler-Human:        PENDING
internal sealed record JsCurrencyNames(
    string Symbol, string Narrow, string One, string Other, string Name, string SymbolEnds, string NarrowEnds);

/// <summary>One token of a pattern's prefix or suffix.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=282FBC
// Broiler-Human:        PENDING
internal enum JsAffixKind
{
    /// <summary>Text, as written.</summary>
    Literal,

    /// <summary>The currency, U+00A4 CURRENCY SIGN.</summary>
    Currency,

    /// <summary>The percent sign, <c>%</c>.</summary>
    Percent,

    /// <summary>The per-mille sign.</summary>
    PerMille,

    /// <summary>The sign's place, <c>-</c>.</summary>
    Minus,

    /// <summary>A plus sign, <c>+</c>.</summary>
    Plus,
}

/// <summary>One affix token.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EB9582
// Broiler-Human:        PENDING
internal readonly record struct JsAffixToken(JsAffixKind Kind, string Text);

/// <summary>
/// A CLDR number pattern, as UTS #35 Part 3 section 3 reads one: a positive subpattern and an
/// optional negative one, each a prefix, a number and a suffix.
/// </summary>
/// <remarks>
/// <para>
/// <b>Only what a formatter reads from it is kept</b>: the affixes, the grouping sizes and, for a
/// compact pattern, how many digits it shows. The digit counts of the number part are ECMA-402's
/// options and not the pattern's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s3; IP=Low; Security=Medium; Resources=2; Fingerprint=4379D5
// Broiler-Human:        PENDING
internal sealed class JsNumberPattern
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8DDCE7
    // Broiler-Human:        PENDING
    private JsNumberPattern()
    {
    }

    internal System.Collections.Generic.List<JsAffixToken> PositivePrefix { get; private set; } = [];

    internal System.Collections.Generic.List<JsAffixToken> PositiveSuffix { get; private set; } = [];

    internal System.Collections.Generic.List<JsAffixToken>? NegativePrefix { get; private set; }

    internal System.Collections.Generic.List<JsAffixToken>? NegativeSuffix { get; private set; }

    /// <summary>The primary grouping size, or -1 where the pattern does not group.</summary>
    internal int Grouping1 { get; private set; } = -1;

    /// <summary>The secondary grouping size.</summary>
    internal int Grouping2 { get; private set; } = -1;

    /// <summary>How many <c>0</c> digits the number part shows, which a compact pattern's exponent is read from.</summary>
    internal int Zeros { get; private set; }

    /// <summary>Whether the positive subpattern writes a plus sign itself.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=419E87
    // Broiler-Human:        PENDING
    internal bool PositiveHasPlus => PositivePrefix.Exists(static t => t.Kind == JsAffixKind.Plus) || PositiveSuffix.Exists(static t => t.Kind == JsAffixKind.Plus);

    /// <summary>Whether the negative subpattern has a sign's place.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4B9307
    // Broiler-Human:        PENDING
    internal bool NegativeHasMinus =>
        NegativePrefix is not null &&
        (NegativePrefix.Exists(static t => t.Kind == JsAffixKind.Minus) || NegativeSuffix!.Exists(static t => t.Kind == JsAffixKind.Minus));

    /// <summary>Reads a pattern; a malformed one reads as its literal text around no number.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s3.2; IP=Low; Security=Medium; Resources=2; Fingerprint=AC1A90
    // Broiler-Falsified-If: a pattern's affix reads other than UTS #35 states, or a quoted character is read as a symbol
    // Broiler-Human:        PENDING
    internal static JsNumberPattern Parse(string pattern)
    {
        var result = new JsNumberPattern();
        var split = SplitSubpatterns(pattern);

        var (prefix, suffix, number) = ParseSubpattern(split.Positive);
        result.PositivePrefix = prefix;
        result.PositiveSuffix = suffix;
        result.ReadNumber(number);

        if (split.Negative is { } negative)
        {
            var (negativePrefix, negativeSuffix, _) = ParseSubpattern(negative);
            result.NegativePrefix = negativePrefix;
            result.NegativeSuffix = negativeSuffix;
        }

        return result;
    }

    /// <summary>The positive and negative subpatterns, split at the first unquoted <c>;</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=109BB3
    // Broiler-Human:        PENDING
    private static (string Positive, string? Negative) SplitSubpatterns(string pattern)
    {
        var quoted = false;

        for (var at = 0; at < pattern.Length; at++)
        {
            if (pattern[at] == '\'')
            {
                quoted = !quoted;
            }
            else if (pattern[at] == ';' && !quoted)
            {
                return (pattern[..at], pattern[(at + 1)..]);
            }
        }

        return (pattern, null);
    }

    /// <summary>One subpattern's prefix tokens, suffix tokens and number part.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=929ADB
    // Broiler-Human:        PENDING
    private static (System.Collections.Generic.List<JsAffixToken> Prefix, System.Collections.Generic.List<JsAffixToken> Suffix, string Number) ParseSubpattern(string text)
    {
        var prefix = new System.Collections.Generic.List<JsAffixToken>();
        var suffix = new System.Collections.Generic.List<JsAffixToken>();
        var number = new System.Text.StringBuilder();
        var stage = 0;
        var literal = new System.Text.StringBuilder();

        void Flush(System.Collections.Generic.List<JsAffixToken> into)
        {
            if (literal.Length != 0)
            {
                into.Add(new JsAffixToken(JsAffixKind.Literal, literal.ToString()));
                literal.Clear();
            }
        }

        for (var at = 0; at < text.Length; at++)
        {
            var c = text[at];
            var into = stage == 0 ? prefix : suffix;

            if (c == '\'')
            {
                var close = text.IndexOf('\'', at + 1);

                if (close < 0)
                {
                    close = text.Length;
                }

                literal.Append(close == at + 1 ? "'" : text[(at + 1)..close]);
                at = close;
                continue;
            }

            if (stage != 2 && (c is '#' or '0' or '@' or ',' or '.' || char.IsAsciiDigit(c) || (stage == 1 && c is 'E' or '+')))
            {
                if (stage == 0)
                {
                    Flush(prefix);
                    stage = 1;
                }

                number.Append(c);
                continue;
            }

            if (stage == 1)
            {
                stage = 2;
                into = suffix;
            }

            switch (c)
            {
                case '\u00a4':
                    Flush(into);
                    var count = 1;

                    while (at + 1 < text.Length && text[at + 1] == '\u00a4')
                    {
                        count++;
                        at++;
                    }

                    into.Add(new JsAffixToken(JsAffixKind.Currency, new string('\u00a4', count)));
                    break;
                case '%':
                    Flush(into);
                    into.Add(new JsAffixToken(JsAffixKind.Percent, "%"));
                    break;
                case '\u2030':
                    Flush(into);
                    into.Add(new JsAffixToken(JsAffixKind.PerMille, "\u2030"));
                    break;
                case '-':
                    Flush(into);
                    into.Add(new JsAffixToken(JsAffixKind.Minus, "-"));
                    break;
                case '+':
                    Flush(into);
                    into.Add(new JsAffixToken(JsAffixKind.Plus, "+"));
                    break;
                default:
                    literal.Append(c);
                    break;
            }
        }

        Flush(stage == 0 ? prefix : suffix);
        return (prefix, suffix, number.ToString());
    }

    /// <summary>Reads the grouping sizes and the count of shown zeros from the number part.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A31F46
    // Broiler-Human:        PENDING
    private void ReadNumber(string number)
    {
        var integerEnd = number.IndexOfAny(['.', 'E']);
        var integer = integerEnd < 0 ? number : number[..integerEnd];

        foreach (var c in number)
        {
            if (c == '0')
            {
                Zeros++;
            }
        }

        var last = integer.LastIndexOf(',');

        if (last < 0)
        {
            return;
        }

        Grouping1 = integer.Length - last - 1;
        var previous = integer.LastIndexOf(',', last - 1);
        Grouping2 = previous < 0 ? Grouping1 : last - previous - 1;
    }
}
