// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   31
// Annotated:        31/31
// Exempt:           38
// Human-reviewed:   0/31
// IP risk:          Low
// Security risk:    Medium
// Criteria:         3/0
// Resource impact:  3/10 max
// Unverified:       31
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One part of a formatted number: its type, its text and, in a range, its source.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=64D4C4
// Broiler-Human:        PENDING
internal readonly record struct JsNumberPart(string Type, string Value, string? Source);

/// <summary>
/// One configured <c>Intl.NumberFormat</c>'s formatting: ECMA-402's PartitionNumberPattern and
/// PartitionNumberRangePattern over the locale's CLDR patterns (JSD-0044).
/// </summary>
/// <remarks>
/// <para>
/// <b>A formatted number is built in three layers around its digits</b>, the way ICU builds one and
/// the way ECMA-402 leaves to the implementation: an inner layer, the scientific exponent; a middle
/// layer, the pattern's prefix and suffix, which carry the sign, the currency symbol, the percent
/// sign and a compact suffix; and an outer layer, a unit's or a currency's name. A range repeats or
/// shares each layer by ICU's rules - the outer when the two are the same name, the middle when the
/// two are the same text of more than one character - and puts spaces round the separator when a
/// layer is repeated, so English writes <c>$1.00 - $5.00</c> with spaces round its en dash, and <c>-$5.00-1.00</c> without.
/// </para>
/// <para>
/// <b>A field's leading and trailing spaces are its neighbours' literals</b>, as ICU trims a field
/// of default-ignorable characters, so the compact <c>1 thousand</c> is an integer, a literal and a
/// compact part.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5; IP=Low; Security=Medium; Resources=3; Fingerprint=D5ADF8
// Broiler-Falsified-If: a value formats differently from the retained ICU 77.1 dataset on a line the dataset does not name as a divergence
// Broiler-Human:        PENDING
internal sealed class JsNumberFormatter
{
    /// <summary>One run of text and its part type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E898DE
    // Broiler-Human:        PENDING
    private readonly record struct Segment(string Type, string Text);

    /// <summary>A number built in its layers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DB8255
    // Broiler-Human:        PENDING
    private sealed class Built
    {
        internal System.Collections.Generic.List<Segment> OuterPrefix { get; } = [];

        internal System.Collections.Generic.List<Segment> MiddlePrefix { get; } = [];

        internal System.Collections.Generic.List<Segment> Core { get; } = [];

        internal System.Collections.Generic.List<Segment> MiddleSuffix { get; } = [];

        internal System.Collections.Generic.List<Segment> OuterSuffix { get; } = [];

        /// <summary>The middle layer's text, which two numbers of a range share when it is equal.</summary>
        internal string MiddleKey { get; set; } = string.Empty;

        internal int MiddleLength { get; set; }

        internal int InnerLength { get; set; }

        internal int OuterLength { get; set; }

        /// <summary>Whether the prefix ends, and the suffix starts, with a currency that spacing applies to.</summary>
        internal bool PrefixSpacing { get; set; }

        internal bool SuffixSpacing { get; set; }

        /// <summary>Whether the digits begin and end with a digit, which currency spacing needs.</summary>
        internal bool CoreIsDigits { get; set; }

        /// <summary>The plural category of the formatted value, which a name is chosen by.</summary>
        internal string Plural { get; set; } = "other";
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=830B12
    // Broiler-Human:        PENDING
    private readonly JsNumberData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=401D99
    // Broiler-Human:        PENDING
    private readonly string language;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=894DA0
    // Broiler-Human:        PENDING
    private readonly string[]? digits;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CF866A
    // Broiler-Human:        PENDING
    internal JsNumberFormatter(JsNumberData data, string language, string numberingSystem)
    {
        this.data = data;
        this.language = language;
        NumberingSystem = numberingSystem;
        digits = numberingSystem == "latn" ? null : System.Collections.Generic.CollectionExtensions.GetValueOrDefault(data.NumberingSystems, numberingSystem);
    }

    // ---- the resolved options (ECMA-402 table 15) ------------------------------------------------

    internal string Locale { get; init; } = string.Empty;

    internal string NumberingSystem { get; }

    internal string Style { get; init; } = "decimal";

    internal string? Currency { get; init; }

    internal string? CurrencyDisplay { get; init; }

    internal string? CurrencySign { get; init; }

    internal string? Unit { get; init; }

    internal string? UnitDisplay { get; init; }

    internal int MinimumIntegerDigits { get; init; } = 1;

    internal int? MinimumFractionDigits { get; init; }

    internal int? MaximumFractionDigits { get; init; }

    internal int? MinimumSignificantDigits { get; init; }

    internal int? MaximumSignificantDigits { get; init; }

    /// <summary><c>fractionDigits</c>, <c>significantDigits</c>, <c>morePrecision</c> or <c>lessPrecision</c>.</summary>
    internal string RoundingType { get; init; } = "fractionDigits";

    internal string ComputedRoundingPriority { get; init; } = "auto";

    internal int RoundingIncrement { get; init; } = 1;

    internal string RoundingMode { get; init; } = "halfExpand";

    internal string TrailingZeroDisplay { get; init; } = "auto";

    internal string Notation { get; init; } = "standard";

    internal string? CompactDisplay { get; init; }

    /// <summary><c>always</c>, <c>auto</c>, <c>min2</c>, or <see langword="null"/> for <see langword="false"/>.</summary>
    internal string? UseGrouping { get; init; } = "auto";

    internal string SignDisplay { get; init; } = "auto";

    // ---- the operations ------------------------------------------------------------------------

    /// <summary>ECMA-402's FormatNumeric.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.6; IP=Low; Security=Low; Resources=2; Fingerprint=4BBFA0
    // Broiler-Human:        PENDING
    internal string Format(JsEngine engine, JsDecimal x)
    {
        var text = new System.Text.StringBuilder();

        foreach (var part in Parts(engine, x))
        {
            text.Append(part.Value);
        }

        return text.ToString();
    }

    /// <summary>ECMA-402's PartitionNumberPattern, as the parts FormatNumericToParts answers.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.4; IP=Low; Security=Medium; Resources=2; Fingerprint=B9A716
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsNumberPart> Parts(JsEngine engine, JsDecimal x)
    {
        var built = Build(engine, x, approximately: false);
        var parts = new System.Collections.Generic.List<(Segment Segment, string? Source)>();
        Emit(parts, built.OuterPrefix, built.MiddlePrefix, null);
        AddCore(parts, built, built, null);
        Emit(parts, built.MiddleSuffix, built.OuterSuffix, null);
        return Flatten(parts);
    }

    /// <summary>ECMA-402's PartitionNumberRangePattern.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.19; IP=Low; Security=Medium; Resources=3; Fingerprint=98A328
    // Broiler-Falsified-If: a range shares or repeats a layer otherwise than ICU's collapse rules, or two values that format alike are not written approximately
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsNumberPart> RangeParts(JsEngine engine, JsDecimal x, JsDecimal y)
    {
        if (string.Equals(Format(engine, x), Format(engine, y), System.StringComparison.Ordinal))
        {
            var approximate = Build(engine, x, approximately: true);
            var shared = new System.Collections.Generic.List<(Segment, string?)>();
            Emit(shared, approximate.OuterPrefix, approximate.MiddlePrefix, "shared");
            AddCore(shared, approximate, approximate, "shared");
            Emit(shared, approximate.MiddleSuffix, approximate.OuterSuffix, "shared");
            return Flatten(shared);
        }

        var first = Build(engine, x, approximately: false);
        var second = Build(engine, y, approximately: false);

        var collapseOuter = string.Equals(OuterKey(first), OuterKey(second), System.StringComparison.Ordinal);
        var collapseMiddle = collapseOuter &&
            string.Equals(first.MiddleKey, second.MiddleKey, System.StringComparison.Ordinal) &&
            first.MiddleLength > 1;

        var repeat = first.InnerLength > 0 ||
            (!collapseMiddle && first.MiddleLength > 0) ||
            (!collapseOuter && first.OuterLength > 0);

        var range = data.Value(language, "misc.range") ?? "{0}\u2013{1}";
        var open = range.IndexOf("{0}", System.StringComparison.Ordinal);
        var close = range.IndexOf("{1}", System.StringComparison.Ordinal);
        var separator = range[(open + 3)..close];

        if (repeat)
        {
            separator = (separator.Length != 0 && IsPatternWhiteSpace(separator[0]) ? string.Empty : " ") + separator +
                (separator.Length != 0 && IsPatternWhiteSpace(separator[^1]) ? string.Empty : " ");
        }

        var parts = new System.Collections.Generic.List<(Segment Segment, string? Source)>();

        // THE COLLAPSED OUTER NAME TAKES THE RANGE'S PLURAL, by the language's plural ranges.
        Built? outer = null;

        if (collapseOuter && first.OuterLength > 0)
        {
            outer = new Built();
            AddOuter(outer, data.PluralRange(language, first.Plural, second.Plural));
        }

        if (outer is not null)
        {
            Emit(parts, outer.OuterPrefix, [], "shared");
        }

        if (collapseMiddle)
        {
            Emit(parts, [], first.MiddlePrefix, "shared");
            AddCore(parts, first, null, "startRange");
            parts.Add((new Segment("literal", separator), "shared"));
            AddCore(parts, null, second, "endRange");
            Emit(parts, second.MiddleSuffix, [], "shared");
        }
        else
        {
            Emit(parts, collapseOuter ? [] : first.OuterPrefix, first.MiddlePrefix, "startRange");
            AddCore(parts, first, first, "startRange");
            Emit(parts, first.MiddleSuffix, collapseOuter ? [] : first.OuterSuffix, "startRange");
            parts.Add((new Segment("literal", separator), "shared"));
            Emit(parts, collapseOuter ? [] : second.OuterPrefix, second.MiddlePrefix, "endRange");
            AddCore(parts, second, second, "endRange");
            Emit(parts, second.MiddleSuffix, collapseOuter ? [] : second.OuterSuffix, "endRange");
        }

        if (outer is not null)
        {
            Emit(parts, [], outer.OuterSuffix, "shared");
        }

        return Flatten(parts);
    }

    /// <summary>What identifies a number's outer layer: its name, without its plural.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3A93D3
    // Broiler-Human:        PENDING
    private string OuterKey(Built built) => built.OuterLength == 0 ? string.Empty : Style + "|" + Unit + "|" + Currency;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F2607F
    // Broiler-Human:        PENDING
    private static void Emit(
        System.Collections.Generic.List<(Segment, string?)> parts,
        System.Collections.Generic.List<Segment> first,
        System.Collections.Generic.List<Segment> second,
        string? source)
    {
        foreach (var segment in first)
        {
            parts.Add((segment, source));
        }

        foreach (var segment in second)
        {
            parts.Add((segment, source));
        }
    }

    /// <summary>
    /// The digits with the currency spacing on each side: the spacing before them is decided by
    /// <paramref name="prefixOf"/>'s prefix and after them by <paramref name="suffixOf"/>'s suffix.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s4.2; IP=Low; Security=Low; Resources=1; Fingerprint=296D08
    // Broiler-Human:        PENDING
    private void AddCore(System.Collections.Generic.List<(Segment, string?)> parts, Built? prefixOf, Built? suffixOf, string? source)
    {
        var core = prefixOf ?? suffixOf!;

        if (prefixOf is { PrefixSpacing: true, CoreIsDigits: true })
        {
            parts.Add((new Segment("literal", data.Value(language, "currency.currencySpacing.beforeCurrency.insertBetween") ?? "\u00a0"), source));
        }

        foreach (var segment in core.Core)
        {
            parts.Add((segment, source));
        }

        if (suffixOf is { SuffixSpacing: true, CoreIsDigits: true })
        {
            parts.Add((new Segment("literal", data.Value(language, "currency.currencySpacing.afterCurrency.insertBetween") ?? "\u00a0"), source));
        }
    }

    /// <summary>
    /// The parts a list of segments is: each field trimmed of the default-ignorable characters at its
    /// ends, which become literals, and adjacent literals joined, shared where their sources differ.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=36881F
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsNumberPart> Flatten(System.Collections.Generic.List<(Segment Segment, string? Source)> segments)
    {
        var trimmed = new System.Collections.Generic.List<(Segment Segment, string? Source)>();

        foreach (var (segment, source) in segments)
        {
            if (segment.Text.Length == 0)
            {
                continue;
            }

            if (segment.Type is "literal" or "group")
            {
                trimmed.Add((segment, source));
                continue;
            }

            var start = 0;
            var end = segment.Text.Length;

            while (start < end && IsDefaultIgnorable(segment.Text[start]))
            {
                start++;
            }

            while (end > start && IsDefaultIgnorable(segment.Text[end - 1]))
            {
                end--;
            }

            if (start > 0)
            {
                trimmed.Add((new Segment("literal", segment.Text[..start]), source));
            }

            if (end > start)
            {
                trimmed.Add((new Segment(segment.Type, segment.Text[start..end]), source));
            }

            if (end < segment.Text.Length && end >= start)
            {
                trimmed.Add((new Segment("literal", segment.Text[end..]), source));
            }
        }

        var parts = new System.Collections.Generic.List<JsNumberPart>(trimmed.Count);

        foreach (var (segment, source) in trimmed)
        {
            if (segment.Type == "literal" && parts.Count != 0 && parts[^1].Type == "literal")
            {
                var previous = parts[^1];
                parts[^1] = new JsNumberPart(
                    "literal",
                    previous.Value + segment.Text,
                    previous.Source == source ? source : "shared");
                continue;
            }

            parts.Add(new JsNumberPart(segment.Type, segment.Text, source));
        }

        return parts;
    }

    /// <summary>ICU's default-ignorable set: space separators, the tab, the bidirectional controls and the variation selectors.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8C71F7
    // Broiler-Human:        PENDING
    private static bool IsDefaultIgnorable(char c) =>
        c is ' ' or '\t' or '\u00a0' or '\u1680' or '\u202f' or '\u205f' or '\u3000' or '\u061c' or '\u200e' or '\u200f'
        || c is >= '\u2000' and <= '\u200a'
        || c is >= '\u202a' and <= '\u202e'
        || c is >= '\u2066' and <= '\u2069'
        || c is >= '\ufe00' and <= '\ufe0f'
        || c is >= '\u180b' and <= '\u180d' or '\u180f';

    /// <summary>Pattern_White_Space, which the range separator is tested against.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B3E3F5
    // Broiler-Human:        PENDING
    private static bool IsPatternWhiteSpace(char c) =>
        c is ' ' or '\t' or '\n' or '\u000b' or '\f' or '\r' or '\u0085' or '\u200e' or '\u200f' or '\u2028' or '\u2029';

    // ---- building one number -------------------------------------------------------------------

    /// <summary>ECMA-402's PartitionNumberPattern, layered.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.4; IP=Low; Security=Medium; Resources=3; Fingerprint=C3D131
    // Broiler-Human:        PENDING
    private Built Build(JsEngine engine, JsDecimal x, bool approximately)
    {
        var built = new Built();
        var exponent = 0;
        string n;
        JsDecimal rounded;

        if (x.Kind == JsDecimalKind.NaN)
        {
            n = Symbol("nan", "NaN");
            rounded = x;
        }
        else if (!x.IsFinite)
        {
            n = Symbol("infinity", "\u221e");
            rounded = x;
        }
        else
        {
            if (!(x.IsZero && x.Negative))
            {
                if (Style == "percent")
                {
                    x = x.Scale(2);
                }

                exponent = ComputeExponent(engine, x);
                x = x.Scale(-exponent);
            }

            (rounded, n) = FormatNumericToString(engine, x);
        }

        // THE SIGNUM, AS ICU READS IT FROM THE ROUNDED VALUE: not-a-number is a positive zero.
        var signum = rounded.Kind switch
        {
            JsDecimalKind.NaN => 1,
            JsDecimalKind.PositiveInfinity => 2,
            JsDecimalKind.NegativeInfinity => -2,
            _ when rounded.IsZero => rounded.Negative ? -1 : 1,
            _ => rounded.Negative ? -2 : 2,
        };

        // POS 0, POS_SIGN 1, NEG 2 (ICU's PatternSignType, from the sign display).
        var signType = SignDisplay switch
        {
            "always" => signum < 0 ? 2 : 1,
            "exceptZero" => signum == -2 ? 2 : signum == 2 ? 1 : 0,
            "negative" => signum == -2 ? 2 : 0,
            "never" => 0,
            _ => signum < 0 ? 2 : 0,
        };

        var compactKey = Notation == "compact" && rounded.IsFinite && !rounded.IsZero
            ? System.Math.Min(exponent + rounded.Magnitude, 14)
            : 0;

        var operands = JsPluralOperands.Of(rounded.IsFinite ? n : "0", Notation == "compact" ? exponent : 0);
        built.Plural = data.Plural(language, operands);

        var (pattern, patternIsCompact, joined) = Patterns(compactKey, exponent, built.Plural);

        // THE MIDDLE LAYER: the pattern's prefix and suffix with the sign written where ICU writes it.
        var prefix = Affix(pattern, patternIsCompact, joined, isPrefix: true, signType, approximately, out var prefixEndsInCurrency);
        var suffix = Affix(pattern, patternIsCompact, joined, isPrefix: false, signType, approximately, out var suffixStartsWithCurrency);
        built.MiddlePrefix.AddRange(prefix);
        built.MiddleSuffix.AddRange(suffix);
        built.PrefixSpacing = prefixEndsInCurrency;
        built.SuffixSpacing = suffixStartsWithCurrency;

        var key = new System.Text.StringBuilder();
        var length = 0;

        foreach (var segment in prefix)
        {
            key.Append(segment.Type).Append('\u0001').Append(segment.Text).Append('\u0002');
            length += CodePoints(segment.Text);
        }

        key.Append('\u0003');

        foreach (var segment in suffix)
        {
            key.Append(segment.Type).Append('\u0001').Append(segment.Text).Append('\u0002');
            length += CodePoints(segment.Text);
        }

        built.MiddleKey = key.ToString();
        built.MiddleLength = length;

        // THE CORE: digits, grouping, the decimal separator, and the exponent as the inner layer.
        if (x.Kind == JsDecimalKind.NaN)
        {
            built.Core.Add(new Segment("nan", n));
        }
        else if (!x.IsFinite)
        {
            built.Core.Add(new Segment("infinity", n));
        }
        else
        {
            AddDigits(built, n);
            built.CoreIsDigits = true;

            if (Notation is "scientific" or "engineering")
            {
                var separator = Symbol("exponential", "E");
                built.Core.Add(new Segment("exponentSeparator", separator));
                built.InnerLength += CodePoints(separator);

                if (exponent < 0)
                {
                    var minus = Symbol("minusSign", "-");
                    built.Core.Add(new Segment("exponentMinusSign", minus));
                    built.InnerLength += CodePoints(minus);
                }

                var power = Transliterate(System.Math.Abs((long)exponent).ToString(System.Globalization.CultureInfo.InvariantCulture));
                built.Core.Add(new Segment("exponentInteger", power));
                built.InnerLength += CodePoints(power);
            }
        }

        // THE OUTER LAYER: a unit's or a currency's name.
        AddOuter(built, built.Plural);
        return built;
    }

    /// <summary>The outer layer for a plural: a unit's pattern, or a currency's name in its pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=A7436F
    // Broiler-Human:        PENDING
    private void AddOuter(Built built, string plural)
    {
        string? pattern = null;
        string? name = null;
        var nameType = "unit";

        if (Style == "unit" && Unit is not null && Unit != "percent")
        {
            pattern = UnitPattern(Unit, UnitDisplay ?? "short", plural);
        }
        else if (Style == "currency" && CurrencyDisplay == "name" && Currency is not null)
        {
            pattern = data.Value(language, "currency.unitPattern-count-" + plural) ??
                data.Value(language, "currency.unitPattern-count-other") ?? "{0} {1}";
            var names = System.Collections.Generic.CollectionExtensions.GetValueOrDefault(data.Currencies, language + "|" + Currency);
            name = names is null ? Currency
                : (plural == "one" ? names.One : names.Other) is { Length: > 0 } counted ? counted
                : names.Other.Length != 0 ? names.Other
                : names.Name.Length != 0 ? names.Name : Currency;
            nameType = "currency";
        }

        if (pattern is null)
        {
            return;
        }

        var at = pattern.IndexOf("{0}", System.StringComparison.Ordinal);

        if (at < 0)
        {
            return;
        }

        void Add(System.Collections.Generic.List<Segment> into, string text)
        {
            if (text.Length == 0)
            {
                return;
            }

            if (name is null)
            {
                into.Add(new Segment(nameType, text));
                built.OuterLength += CodePoints(text);
                return;
            }

            // A CURRENCY'S NAME PATTERN: {1} is the name, and the rest is literal.
            var slot = text.IndexOf("{1}", System.StringComparison.Ordinal);

            if (slot < 0)
            {
                into.Add(new Segment("literal", text));
            }
            else
            {
                into.Add(new Segment("literal", text[..slot]));
                into.Add(new Segment("currency", name));
                into.Add(new Segment("literal", text[(slot + 3)..]));
            }

            built.OuterLength += CodePoints(text);
        }

        Add(built.OuterPrefix, pattern[..at]);
        Add(built.OuterSuffix, pattern[(at + 3)..]);
    }

    /// <summary>
    /// A unit's pattern in a width and a plural: CLDR's own for a simple unit or a compound it names,
    /// or the numerator's pattern inside the denominator's per-unit pattern or the width's compound
    /// pattern.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 2 s6.1; IP=Low; Security=Low; Resources=2; Fingerprint=6BDC09
    // Broiler-Human:        PENDING
    private string? UnitPattern(string unit, string width, string plural)
    {
        string? Field(string id, string field)
        {
            foreach (var w in width == "narrow" ? new[] { "narrow", "short", "long" } : width == "short" ? ["short", "long"] : ["long"])
            {
                if (data.Units.TryGetValue(language + "|" + w + "|" + id + "|" + field, out var value))
                {
                    return value;
                }
            }

            return null;
        }

        // A WIDTH'S OWN OTHER FORM BEFORE A WIDER WIDTH: CLDR gives German's short nanosecond only
        // `{0} ns`, and one nanosecond is `1 ns`, as ICU writes it, not the long `1 Nanosekunde`.
        string? Pattern(string id)
        {
            foreach (var w in width == "narrow" ? new[] { "narrow", "short", "long" } : width == "short" ? ["short", "long"] : ["long"])
            {
                if (data.Units.TryGetValue(language + "|" + w + "|" + id + "|unitPattern-count-" + plural, out var value) ||
                    data.Units.TryGetValue(language + "|" + w + "|" + id + "|unitPattern-count-other", out value))
                {
                    return value;
                }
            }

            return null;
        }

        if (Pattern(unit) is { } direct)
        {
            return direct;
        }

        var per = unit.IndexOf("-per-", System.StringComparison.Ordinal);

        if (per < 0)
        {
            return null;
        }

        var numerator = Pattern(unit[..per]);
        var denominator = unit[(per + 5)..];

        if (numerator is null)
        {
            return null;
        }

        if (Field(denominator, "perUnitPattern") is { } perUnit)
        {
            return perUnit.Replace("{0}", numerator, System.StringComparison.Ordinal);
        }

        var compound = Field("per", "compoundUnitPattern") ?? "{0}/{1}";
        var single = (Field(denominator, "unitPattern-count-one") ?? Field(denominator, "unitPattern-count-other") ?? denominator)
            .Replace("{0}", string.Empty, System.StringComparison.Ordinal).Trim();

        return compound.Replace("{0}", numerator, System.StringComparison.Ordinal).Replace("{1}", single, System.StringComparison.Ordinal);
    }

    /// <summary>
    /// The pattern the value is written by, and a compact pattern whose affix joins it next to the
    /// number. A compact currency pattern carries its own currency, so it is the pattern itself; any
    /// other compact pattern's affix joins the style's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=C28EAB
    // Broiler-Human:        PENDING
    private (JsNumberPattern Pattern, bool PatternIsCompact, JsNumberPattern? Joined) Patterns(int compactKey, int exponent, string plural)
    {
        var main = JsNumberPattern.Parse(MainPattern());

        if (Notation != "compact" || exponent == 0 || CompactPattern(compactKey, plural) is not { } compact)
        {
            return (main, false, null);
        }

        return Style == "currency" && CurrencyDisplay != "name"
            ? (JsNumberPattern.Parse(compact), true, null)
            : (main, false, JsNumberPattern.Parse(compact));
    }

    /// <summary>The style's pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=739047
    // Broiler-Human:        PENDING
    private string MainPattern() => Style switch
    {
        "percent" => data.Value(language, "percent.standard") ?? "#,##0%",
        "unit" when Unit == "percent" => data.Value(language, "percent.standard") ?? "#,##0%",
        "currency" when CurrencyDisplay == "name" => data.Value(language, "decimal.standard") ?? "#,##0.###",
        "currency" => (CurrencySign == "accounting" ? data.Value(language, "currency.accounting") : null) ??
            data.Value(language, "currency.standard") ?? "\u00a4#,##0.00",
        _ => data.Value(language, "decimal.standard") ?? "#,##0.###",
    };

    /// <summary>The compact pattern for a magnitude and a plural, or nothing where the locale does not compact it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37F655
    // Broiler-Human:        PENDING
    private string? CompactPattern(int magnitude, string plural)
    {
        if (magnitude < 3)
        {
            return null;
        }

        var key = "1" + new string('0', System.Math.Min(magnitude, 14));
        var root = Style == "currency" && CurrencyDisplay != "name"
            ? "currency.short.standard."
            : CompactDisplay == "long" ? "decimal.long.decimalFormat." : "decimal.short.decimalFormat.";

        var pattern = data.Value(language, root + key + "-count-" + plural) ?? data.Value(language, root + key + "-count-other");
        return pattern is null or "0" ? null : pattern;
    }

    /// <summary>
    /// ECMA-402's ComputeExponentForMagnitude: 0 for the standard notation, the magnitude for the
    /// scientific, its multiple of three below for the engineering, and the compact pattern's.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.14; IP=Low; Security=Low; Resources=1; Fingerprint=575CD7
    // Broiler-Human:        PENDING
    private int ExponentForMagnitude(int magnitude)
    {
        switch (Notation)
        {
            case "scientific":
                return magnitude;
            case "engineering":
                return (int)System.Math.Floor(magnitude / 3.0) * 3;
            case "compact":
                var pattern = CompactPattern(magnitude, "other");

                if (pattern is null)
                {
                    return 0;
                }

                var zeros = 0;

                foreach (var c in pattern)
                {
                    if (c == '0')
                    {
                        zeros++;
                    }
                }

                return System.Math.Min(magnitude, 14) - (zeros - 1);
            default:
                return 0;
        }
    }

    /// <summary>ECMA-402's ComputeExponent.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.13; IP=Low; Security=Low; Resources=2; Fingerprint=AC7BD6
    // Broiler-Human:        PENDING
    private int ComputeExponent(JsEngine engine, JsDecimal x)
    {
        if (x.IsZero || Notation == "standard")
        {
            return 0;
        }

        var magnitude = x.Magnitude;
        var exponent = ExponentForMagnitude(magnitude);
        var (rounded, _) = FormatNumericToString(engine, x.Abs().Scale(-exponent));

        if (rounded.IsZero)
        {
            return exponent;
        }

        return rounded.Magnitude == magnitude - exponent ? exponent : ExponentForMagnitude(magnitude + 1);
    }

    /// <summary>ECMA-402's FormatNumericToString: the rounded value with its sign, and its decimal string.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.3; IP=Low; Security=Medium; Resources=2; Fingerprint=789311
    // Broiler-Falsified-If: a rounding type, a priority, the trailing-zero display or the minimum integer digits is applied otherwise than FormatNumericToString states
    // Broiler-Human:        PENDING
    internal (JsDecimal Rounded, string Formatted) FormatNumericToString(JsEngine engine, JsDecimal x)
    {
        var negative = x.Negative;
        var magnitude = x.Abs();
        engine.Charge((ulong)(magnitude.Digits.Length / 16) + 1);

        var mode = Unsigned(RoundingMode, negative);
        JsRawFormat result;

        switch (RoundingType)
        {
            case "significantDigits":
                result = JsRawDecimal.ToRawPrecision(magnitude, MinimumSignificantDigits!.Value, MaximumSignificantDigits!.Value, mode);
                break;
            case "fractionDigits":
                result = JsRawDecimal.ToRawFixed(magnitude, MinimumFractionDigits!.Value, MaximumFractionDigits!.Value, RoundingIncrement, mode);
                break;
            default:
                var precision = JsRawDecimal.ToRawPrecision(magnitude, MinimumSignificantDigits!.Value, MaximumSignificantDigits!.Value, mode);
                var fixedResult = JsRawDecimal.ToRawFixed(magnitude, MinimumFractionDigits!.Value, MaximumFractionDigits!.Value, RoundingIncrement, mode);
                var fixedIsMorePrecise = fixedResult.RoundingMagnitude < precision.RoundingMagnitude;
                result = RoundingType == "morePrecision"
                    ? (fixedIsMorePrecise ? fixedResult : precision)
                    : (fixedIsMorePrecise ? precision : fixedResult);
                break;
        }

        var text = result.Formatted;

        if (TrailingZeroDisplay == "stripIfInteger" && IsInteger(result.Rounded))
        {
            var point = text.IndexOf('.');

            if (point >= 0)
            {
                text = text[..point];
            }
        }

        if (result.IntegerDigits < MinimumIntegerDigits)
        {
            text = new string('0', MinimumIntegerDigits - result.IntegerDigits) + text;
        }

        return (result.Rounded.WithSign(negative), text);
    }

    /// <summary>Whether a rounded value is an integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E2FFE4
    // Broiler-Human:        PENDING
    private static bool IsInteger(JsDecimal value) => value.IsZero || value.Point >= value.Digits.Length;

    /// <summary>ECMA-402's GetUnsignedRoundingMode.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.17; IP=Low; Security=Low; Resources=0; Fingerprint=4D378A
    // Broiler-Human:        PENDING
    private static JsUnsignedRounding Unsigned(string mode, bool negative) => mode switch
    {
        "ceil" => negative ? JsUnsignedRounding.Zero : JsUnsignedRounding.Infinity,
        "floor" => negative ? JsUnsignedRounding.Infinity : JsUnsignedRounding.Zero,
        "expand" => JsUnsignedRounding.Infinity,
        "trunc" => JsUnsignedRounding.Zero,
        "halfCeil" => negative ? JsUnsignedRounding.HalfZero : JsUnsignedRounding.HalfInfinity,
        "halfFloor" => negative ? JsUnsignedRounding.HalfInfinity : JsUnsignedRounding.HalfZero,
        "halfTrunc" => JsUnsignedRounding.HalfZero,
        "halfEven" => JsUnsignedRounding.HalfEven,
        _ => JsUnsignedRounding.HalfInfinity,
    };

    /// <summary>
    /// One affix as ICU's patternInfoToStringBuilder writes it: the positive or negative subpattern's
    /// tokens, with the sign, a plus sign or the approximately sign at the sign's place.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s3.2; IP=Low; Security=Medium; Resources=2; Fingerprint=D3CA2E
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<Segment> Affix(
        JsNumberPattern pattern,
        bool patternIsCompact,
        JsNumberPattern? joined,
        bool isPrefix,
        int signType,
        bool approximately,
        out bool currencyAtNumber)
    {
        var plusReplacesMinus = signType == 1 && !pattern.PositiveHasPlus;
        var useNegative = pattern.NegativePrefix is not null &&
            (signType == 2 || (pattern.NegativeHasMinus && !plusReplacesMinus && !approximately));

        var tokens = new System.Collections.Generic.List<(JsAffixToken Token, bool Compact)>();

        foreach (var token in useNegative ? (isPrefix ? pattern.NegativePrefix! : pattern.NegativeSuffix!) : (isPrefix ? pattern.PositivePrefix : pattern.PositiveSuffix))
        {
            tokens.Add((token, patternIsCompact && token.Kind == JsAffixKind.Literal));
        }

        // A JOINED COMPACT AFFIX SITS NEXT TO THE NUMBER, its text the compact part.
        if (joined is not null)
        {
            var compactTokens = new System.Collections.Generic.List<(JsAffixToken, bool)>();

            foreach (var token in isPrefix ? joined.PositivePrefix : joined.PositiveSuffix)
            {
                compactTokens.Add((token, token.Kind == JsAffixKind.Literal));
            }

            if (isPrefix)
            {
                tokens.AddRange(compactTokens);
            }
            else
            {
                tokens.InsertRange(0, compactTokens);
            }
        }

        var prependSign = isPrefix && !useNegative && (signType == 2 || plusReplacesMinus || approximately);
        var signs = approximately
            ? (plusReplacesMinus ? "~+" : signType == 2 ? "~-" : "~")
            : plusReplacesMinus ? "+" : "-";

        var segments = new System.Collections.Generic.List<Segment>();

        void Sign()
        {
            foreach (var c in signs)
            {
                segments.Add(c switch
                {
                    '~' => new Segment("approximatelySign", Symbol("approximatelySign", "~")),
                    '+' => new Segment("plusSign", Symbol("plusSign", "+")),
                    _ => new Segment("minusSign", Symbol("minusSign", "-")),
                });
            }
        }

        if (prependSign)
        {
            Sign();
        }

        currencyAtNumber = false;

        for (var index = 0; index < tokens.Count; index++)
        {
            var (token, isCompact) = tokens[index];

            switch (token.Kind)
            {
                case JsAffixKind.Minus:
                    Sign();
                    break;
                case JsAffixKind.Plus:
                    segments.Add(new Segment("plusSign", Symbol("plusSign", "+")));
                    break;
                case JsAffixKind.Percent:
                    segments.Add(new Segment(Style == "unit" ? "unit" : "percentSign", Symbol("percentSign", "%")));
                    break;
                case JsAffixKind.PerMille:
                    segments.Add(new Segment("literal", Symbol("perMille", "\u2030")));
                    break;
                case JsAffixKind.Currency:
                    var (text, ends) = CurrencyText(token.Text.Length);
                    segments.Add(new Segment("currency", text));

                    // CLDR'S CURRENCY SPACING: only a currency next to the number, and only when its
                    // character there is neither a symbol nor a separator.
                    if (isPrefix ? index == tokens.Count - 1 : index == 0)
                    {
                        currencyAtNumber = (ends.Length == 2 ? ends[isPrefix ? 1 : 0] : 'L') == 'L';
                    }

                    break;
                default:
                    segments.Add(new Segment(isCompact ? "compact" : "literal", token.Text));
                    break;
            }
        }

        return segments;
    }

    /// <summary>The currency's text for one, two or three currency signs in a pattern, and its ends' kinds for spacing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E26E20
    // Broiler-Human:        PENDING
    private (string Text, string Ends) CurrencyText(int count)
    {
        var code = Currency ?? "XXX";
        var names = System.Collections.Generic.CollectionExtensions.GetValueOrDefault(data.Currencies, language + "|" + code);

        if (count >= 2 || CurrencyDisplay == "code" || names is null)
        {
            return (code, "LL");
        }

        if (CurrencyDisplay == "narrowSymbol" && names.Narrow.Length != 0)
        {
            return (names.Narrow, names.NarrowEnds);
        }

        return names.Symbol.Length != 0 ? (names.Symbol, names.SymbolEnds) : (code, "LL");
    }

    /// <summary>Adds the digits of a formatted decimal string: integer groups, the decimal separator, the fraction.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s16.5.5; IP=Low; Security=Low; Resources=2; Fingerprint=25F2CB
    // Broiler-Human:        PENDING
    private void AddDigits(Built built, string n)
    {
        var point = n.IndexOf('.');
        var integer = point < 0 ? n : n[..point];
        var fraction = point < 0 ? null : n[(point + 1)..];

        var main = JsNumberPattern.Parse(MainPattern());
        var grouping1 = main.Grouping1;
        var grouping2 = main.Grouping2 <= 0 ? grouping1 : main.Grouping2;
        var minimumGrouping = UseGrouping switch
        {
            "min2" => 2,
            "auto" => int.TryParse(data.Value(language, "minimumGroupingDigits"), out var locale) ? locale : 1,
            _ => 1,
        };

        // ICU'S GROUPING TEST: a separator follows the digit at position p (0 for the ones) when
        // p - g1 is a nonnegative multiple of g2, and the number groups at all only when its leading
        // group holds at least the minimum grouping digits.
        var upper = integer.Length - 1;
        var groups = UseGrouping is not null && grouping1 > 0 && upper - grouping1 + 1 >= minimumGrouping;
        var group = Symbol("group", ",");
        var start = 0;

        for (var index = 0; groups && index < integer.Length; index++)
        {
            var position = upper - index;

            if (position >= grouping1 && (position - grouping1) % grouping2 == 0)
            {
                built.Core.Add(new Segment("integer", Transliterate(integer[start..(index + 1)])));
                built.Core.Add(new Segment("group", group));
                start = index + 1;
            }
        }

        built.Core.Add(new Segment("integer", Transliterate(integer[start..])));

        if (fraction is not null)
        {
            built.Core.Add(new Segment("decimal", Symbol("decimal", ".")));
            built.Core.Add(new Segment("fraction", Transliterate(fraction)));
        }
    }

    /// <summary>ASCII digits as the numbering system's.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 table 14; IP=Low; Security=Low; Resources=1; Fingerprint=3C9325
    // Broiler-Human:        PENDING
    private string Transliterate(string ascii)
    {
        if (digits is null)
        {
            return ascii;
        }

        var text = new System.Text.StringBuilder(ascii.Length);

        foreach (var c in ascii)
        {
            text.Append(char.IsAsciiDigit(c) ? digits[c - '0'] : c.ToString());
        }

        return text.ToString();
    }

    /// <summary>A symbol of the locale's <c>latn</c> symbols, which every numbering system writes with here.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=206FE3
    // Broiler-Human:        PENDING
    private string Symbol(string name, string fallback) => data.Value(language, "symbols." + name) ?? fallback;

    /// <summary>The code points of a string.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7340D5
    // Broiler-Human:        PENDING
    private static int CodePoints(string text)
    {
        var count = 0;

        for (var at = 0; at < text.Length; at++)
        {
            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]))
            {
                at++;
            }

            count++;
        }

        return count;
    }
}
