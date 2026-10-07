// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Intl;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Checks of the internationalization surface (JSD-0043, phase F7): the data boundary, the root
/// collation against UCA's own conformance files, the German tailorings, and the retained German
/// and English orderings against ICU.
/// </summary>
/// <remarks>
/// <para>
/// <b>The conformance files are read where they are archived</b>, under
/// <c>src/tests/cldr/pins</c>, found by walking up from the working directory; a machine without the
/// checkout reports those two rows as not run rather than as passed. Each file's lines are in
/// collation order, and the check asks <c>Intl.Collator</c> whether every line compares at or below
/// the next - in chunks, one program each, so no single operation's allowance bounds the file.
/// </para>
/// </remarks>
internal static class IntlChecks
{
    private const string Caller = "js-slice-compiler://intl";

    /// <summary>The lines one program compares.</summary>
    private const int Chunk = 12_000;

    /// <summary>Every internationalization check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        ADoorHandedNoDataBuildsNoIntl(),
        ADoorNamingIntlWithoutDataIsRefused(),
        GermanPhonebookAndSearchOrder(),
        GermanAndEnglishOrderingsMatchIcu(),
        GermanAndEnglishNumbersMatchIcu(),
        GermanAndEnglishDatesMatchIcu(),
        LocalesMatchIcu(),
        PluralsMatchIcu(),
        BreakTestFile("grapheme", "GraphemeBreakTest.txt", 700),
        BreakTestFile("word", "WordBreakTest.txt", 1800),
        BreakTestFile("sentence", "SentenceBreakTest.txt", 500),
        ListsMatchIcu(),
        RelativeTimesMatchIcu(),
        SegmentsMatchIcu(),
        DurationsMatchIcu(),
        DisplayNamesMatchIcu(),
        TimeZonesMatchIcu(),
        ADoorHandedNoDataBuildsNoTemporal(),
        TemporalWithoutIntlAndBigIntIsRefused(),
        TemporalMatchesTheReferencePolyfill(),
        IntlOverTemporalMatchesTheReferencePolyfill(),
        CalendarsMatchTheReferencePolyfill(),
        CalendarConversionsMatchIcu4x(),
        CalendarFormatsMatchTheReferencePolyfill(),
        ZoneNamesMatchTheReferencePolyfill(),
        CanonicalizationReplacesAliases(),
        ConformanceFile("non-ignorable", "CollationTest_CLDR_NON_IGNORABLE_SHORT.txt", "{ sensitivity: 'variant' }"),
        ConformanceFile("shifted", "CollationTest_CLDR_SHIFTED_SHORT.txt", "{ sensitivity: 'variant', ignorePunctuation: true }"),
    ];

    /// <summary>A composition handed no data builds no <c>Intl</c>, whatever it admits.</summary>
    private static (string, bool, string) ADoorHandedNoDataBuildsNoIntl()
    {
        const string Name = "intl/i0/a-door-handed-no-data-builds-no-intl";
        var answer = Evaluate("typeof Intl + ':' + 'b'.localeCompare('a');", JavaScriptProfile.DescriptorHostingRealms(new QuietSurface()));

        return (Name, answer == "undefined:1", $"answered `{answer}`: every surface the composition could build, and Intl is not one without its data");
    }

    /// <summary>A door that names the surface and was handed no data is refused when it is built.</summary>
    private static (string, bool, string) ADoorNamingIntlWithoutDataIsRefused()
    {
        const string Name = "intl/i0/a-door-naming-intl-without-data-is-refused";

        try
        {
            _ = JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.IntlManifest);
            return (Name, false, "a descriptor admitting broiler.javascript.intl with no data was built");
        }
        catch (System.ArgumentException refused)
        {
            var built = JavaScriptProfile.DescriptorComposing(new JsComposition
            {
                Surfaces = [JavaScriptProfile.IntlManifest],
                IntlData = JsCldrData.Instance,
            }) is not null;

            return (
                Name,
                built && refused.Message.Contains("IJsIntlData", System.StringComparison.Ordinal),
                "the door was refused, naming the data it needs, and DescriptorComposing handed the data built");
        }
    }

    /// <summary>German's phonebook and search tailorings, as the suite's non-normative cases state them.</summary>
    private static (string, bool, string) GermanPhonebookAndSearchOrder()
    {
        const string Name = "intl/i1/german-phonebook-and-search-order";
        var answer = Evaluate(
            "var p = ['A', 'b', 'Af', 'Ab', 'od', 'off', '\\u00c4', '\\u00f6'].sort(new Intl.Collator('de-u-co-phonebk').compare).join(',');" +
            "var r = ['A', 'b', 'Af', 'Ab', 'od', 'off', '\\u00c4', '\\u00f6'].sort(new Intl.Collator('de').compare).join(',');" +
            "var s = ['AE', '\\u00c4'].sort(new Intl.Collator('de', { usage: 'search' }).compare).join(',');" +
            "p + '|' + r + '|' + s + '|' + new Intl.Collator('de-u-co-phonebk').resolvedOptions().locale;",
            Composing());

        const string Expected = "A,Ab,\u00c4,Af,b,od,\u00f6,off|A,\u00c4,Ab,Af,b,\u00f6,od,off|AE,\u00c4|de-u-co-phonebk";
        return (Name, answer == Expected, $"answered `{answer}` (`{Expected}` expected): phonebook puts A-umlaut with AE, the root with A, and German search equates them but for the accent");
    }

    /// <summary>
    /// The retained German and English orderings (JSD-0027 section 7, slice I1): the program under
    /// <c>src/tests/cldr/orderings</c>, run here, answers every line ICU 77.1 answered.
    /// </summary>
    private static (string, bool, string) GermanAndEnglishOrderingsMatchIcu()
    {
        const string Name = "intl/i1/german-and-english-orderings-match-icu";

        if (Archived("src/tests/cldr/orderings/orderings.js") is not { } program ||
            Archived("src/tests/cldr/orderings/orderings.icu-77.1.txt") is not { } retained)
        {
            return ("not-run/" + Name, false, "the retained orderings are not under this working directory");
        }

        var expected = System.IO.File.ReadAllText(retained).TrimEnd('\n').Split('\n');
        var answered = Evaluate(System.IO.File.ReadAllText(program), Composing()).Split('\n');
        var differing = new System.Collections.Generic.List<string>();

        for (var line = 0; line < System.Math.Max(expected.Length, answered.Length); line++)
        {
            var want = line < expected.Length ? expected[line] : "(no line)";
            var got = line < answered.Length ? answered[line] : "(no line)";

            if (!string.Equals(want, got, System.StringComparison.Ordinal))
            {
                differing.Add($"line {line + 1}: `{got}` where ICU answered `{want}`");
            }
        }

        return (
            Name,
            differing.Count == 0 && expected.Length >= 20,
            differing.Count == 0
                ? $"all {expected.Length} collators order the words as ICU 77.1 did"
                : $"{differing.Count} of {expected.Length} lines differ, first {differing[0]}");
    }

    /// <summary>
    /// The retained German and English number formatting (JSD-0027 section 7, slice I2): the program
    /// under <c>src/tests/cldr/numbers</c>, run here, answers every line ICU 77.1 answered, but for the
    /// lines <c>divergences.txt</c> names, which it answers as written there.
    /// </summary>
    private static (string, bool, string) GermanAndEnglishNumbersMatchIcu() =>
        RetainedMatchesIcu("intl/i2/german-and-english-numbers-match-icu", "numbers", 2000);

    /// <summary>
    /// The retained German and English date formatting (JSD-0027 section 7, slice I3): the program
    /// under <c>src/tests/cldr/dates</c>, run here, answers every line ICU 77.1 answered, but for the
    /// lines <c>divergences.txt</c> names, which it answers as written there.
    /// </summary>
    private static (string, bool, string) GermanAndEnglishDatesMatchIcu() =>
        RetainedMatchesIcu("intl/i3/german-and-english-dates-match-icu", "dates", 1000);

    /// <summary>
    /// The retained Locales (slice I4, JSD-0046): the program under <c>src/tests/cldr/locales</c>,
    /// run here, answers every line Node answered, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) LocalesMatchIcu() =>
        RetainedMatchesIcu("intl/i4/locales-match-icu", "locales", 700);

    /// <summary>
    /// The retained German and English plural categories (slice I4, JSD-0047): the program under
    /// <c>src/tests/cldr/plurals</c>, run here, answers every line Node answered, but for the lines
    /// <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) PluralsMatchIcu() =>
        RetainedMatchesIcu("intl/i4/german-and-english-plurals-match-icu", "plurals", 1200);

    /// <summary>
    /// The retained German and English lists (slice I4, JSD-0048): the program under
    /// <c>src/tests/cldr/lists</c>, run here, answers every line Node answered, but for the lines
    /// <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) ListsMatchIcu() =>
        RetainedMatchesIcu("intl/i4/german-and-english-lists-match-icu", "lists", 450);

    /// <summary>
    /// The retained German and English relative times (slice I4, JSD-0049): the program under
    /// <c>src/tests/cldr/relativetimes</c>, run here, answers every line Node answered, but for the
    /// lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) RelativeTimesMatchIcu() =>
        RetainedMatchesIcu("intl/i4/german-and-english-relative-times-match-icu", "relativetimes", 2400);

    /// <summary>
    /// The retained segments (slice I4, JSD-0050): the program under <c>src/tests/cldr/segments</c>,
    /// run here, answers every line Node answered, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) SegmentsMatchIcu() =>
        RetainedMatchesIcu("intl/i4/segments-match-icu", "segments", 110);

    /// <summary>
    /// The retained German and English durations (slice I4, JSD-0051): the program under
    /// <c>src/tests/cldr/durations</c>, run here, answers every line Node answered, but for the lines
    /// <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) DurationsMatchIcu() =>
        RetainedMatchesIcu("intl/i4/german-and-english-durations-match-icu", "durations", 500);

    /// <summary>
    /// The retained German and English display names (slice I4, JSD-0052): the program under
    /// <c>src/tests/cldr/displaynames</c>, run here, answers every line Node answered, but for the
    /// lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) DisplayNamesMatchIcu() =>
        RetainedMatchesIcu("intl/i4/german-and-english-display-names-match-icu", "displaynames", 800);

    /// <summary>
    /// The retained time zones (phase F8, JSD-0053): the program under <c>src/tests/cldr/timezones</c>,
    /// run here, answers every line Node answered, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) TimeZonesMatchIcu() =>
        RetainedMatchesIcu("tzdb/time-zone-offsets-match-icu", "timezones", 600);

    /// <summary>A composition handed no data builds no <c>Temporal</c> either (JSD-0054): its time zones are that data.</summary>
    private static (string, bool, string) ADoorHandedNoDataBuildsNoTemporal()
    {
        const string Name = "temporal/t1/a-door-handed-no-data-builds-no-temporal";
        var answer = Evaluate(
            "typeof Temporal + ':' + typeof Date.prototype.toTemporalInstant;",
            JavaScriptProfile.DescriptorHostingRealms(new QuietSurface()));

        return (Name, answer == "undefined:undefined", $"answered `{answer}`: every surface the composition could build, and Temporal is not one without the data");
    }

    /// <summary>JSD-0054: a descriptor naming the Temporal surface without Intl or without BigInt is refused.</summary>
    private static (string, bool, string) TemporalWithoutIntlAndBigIntIsRefused()
    {
        const string Name = "temporal/t1/temporal-without-intl-and-bigint-is-refused";
        var refusals = 0;

        foreach (var surfaces in new[]
        {
            new[] { JavaScriptProfile.TemporalManifest, JavaScriptProfile.BigIntManifest },
            new[] { JavaScriptProfile.TemporalManifest, JavaScriptProfile.IntlManifest },
        })
        {
            try
            {
                _ = JavaScriptProfile.DescriptorComposing(new JsComposition { Surfaces = surfaces, IntlData = JsCldrData.Instance });
            }
            catch (System.ArgumentException refused) when (refused.Message.Contains("broiler.javascript.temporal", System.StringComparison.Ordinal))
            {
                refusals++;
            }
        }

        var answer = Evaluate(
            "typeof Temporal.Instant.from('2020-01-01T00:00Z').epochNanoseconds;",
            JavaScriptProfile.DescriptorComposing(new JsComposition
            {
                Surfaces = [JavaScriptProfile.TemporalManifest, JavaScriptProfile.IntlManifest, JavaScriptProfile.BigIntManifest],
                IntlData = JsCldrData.Instance,
            }));

        return (
            Name,
            refusals == 2 && answer == "bigint",
            $"{refusals} of the 2 descriptors lacking Intl or BigInt were refused, and one naming all three answered `{answer}`");
    }

    /// <summary>
    /// The retained Temporal answers (phase F8 T1, JSD-0054): the program under <c>src/tests/temporal</c>,
    /// run here, answers every line the reference polyfill at the pinned proposal revision answered.
    /// </summary>
    private static (string, bool, string) TemporalMatchesTheReferencePolyfill() =>
        RetainedMatches(
            "temporal/t1/temporal-matches-the-reference-polyfill",
            "src/tests/temporal",
            "temporal",
            "temporal.polyfill-e8cc03fc.txt",
            "the reference polyfill",
            1500);

    /// <summary>
    /// The retained answers of Intl over Temporal objects (phase F8 T2, JSD-0055): the program under
    /// <c>src/tests/temporal/intl</c>, run here, answers every line the reference polyfill answered
    /// through ICU 77.1, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) IntlOverTemporalMatchesTheReferencePolyfill() =>
        RetainedMatches(
            "temporal/t2/intl-over-temporal-matches-the-reference-polyfill",
            "src/tests/temporal/intl",
            "temporal-intl",
            "temporal-intl.polyfill-e8cc03fc.txt",
            "the reference polyfill",
            1200);

    /// <summary>
    /// The retained answers of Temporal in the CLDR calendars (phase F8 T3, JSD-0056): the program under
    /// <c>src/tests/temporal/calendars</c>, run here, answers every line the reference polyfill answered
    /// through ICU4C 77.1, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) CalendarsMatchTheReferencePolyfill() =>
        RetainedMatches(
            "temporal/t3/calendars-match-the-reference-polyfill",
            "src/tests/temporal/calendars",
            "temporal-calendars",
            "temporal-calendars.polyfill-e8cc03fc.txt",
            "the reference polyfill",
            3500);

    /// <summary>
    /// The retained answers of Intl.DateTimeFormat in the CLDR calendars (phase F8 T4, JSD-0057): the
    /// program under <c>src/tests/temporal/calendars-intl</c>, run here, answers every line the
    /// reference polyfill answered through ICU 77.1, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) CalendarFormatsMatchTheReferencePolyfill() =>
        RetainedMatches(
            "temporal/t4/calendar-formats-match-the-reference-polyfill",
            "src/tests/temporal/calendars-intl",
            "calendar-formats",
            "calendar-formats.polyfill-e8cc03fc.txt",
            "the reference polyfill",
            5500);

    /// <summary>
    /// The retained answers of the time zones' names (phase F8 T5, JSD-0058): the program under
    /// <c>src/tests/temporal/zone-names</c>, run here, answers every line the reference polyfill answered
    /// through ICU 77.1, but for the lines <c>divergences.txt</c> names.
    /// </summary>
    private static (string, bool, string) ZoneNamesMatchTheReferencePolyfill() =>
        RetainedMatches(
            "temporal/t5/zone-names-match-the-reference-polyfill",
            "src/tests/temporal/zone-names",
            "zone-names",
            "zone-names.polyfill-e8cc03fc.txt",
            "the reference polyfill",
            2600);

    /// <summary>
    /// The same program's conversion lines against ICU4X 2.3.0's (JSD-0056): every line ICU4X wrote is
    /// one the profile answers, the divergences from the polyfill included.
    /// </summary>
    private static (string, bool, string) CalendarConversionsMatchIcu4x()
    {
        const string Name = "temporal/t3/calendar-conversions-match-icu4x";

        if (Archived("src/tests/temporal/calendars/temporal-calendars.js") is not { } program ||
            Archived("src/tests/temporal/calendars/temporal-calendars.icu4x-2.3.0.txt") is not { } retained)
        {
            return ("not-run/" + Name, false, "the retained calendar conversions are not under this working directory");
        }

        var answered = new System.Collections.Generic.HashSet<string>(
            Evaluate(System.IO.File.ReadAllText(program), Composing()).Split('\n'), System.StringComparer.Ordinal);
        var expected = System.IO.File.ReadAllText(retained).TrimEnd('\n').Split('\n');
        var missing = System.Array.FindAll(expected, line => !answered.Contains(line));

        return (
            Name,
            missing.Length == 0 && expected.Length > 2000,
            missing.Length == 0
                ? $"all {expected.Length} conversions answer as ICU4X 2.3.0 did"
                : $"{missing.Length} of {expected.Length} conversions differ, first `{missing[0]}`");
    }

    /// <summary>
    /// A retained dataset under <c>src/tests/cldr/<paramref name="dataset"/></c>: its program, run
    /// here, answers every line of its ICU 77.1 answers, but for the lines its <c>divergences.txt</c>
    /// names, each of which must be used.
    /// </summary>
    private static (string, bool, string) RetainedMatchesIcu(string name, string dataset, int least) =>
        RetainedMatches(name, $"src/tests/cldr/{dataset}", dataset, $"{dataset}.icu-77.1.txt", "ICU 77.1", least);

    /// <summary>
    /// A retained dataset under <paramref name="directory"/>: its program, run here, answers every
    /// line of the answers <paramref name="reference"/> gave, but for the lines its
    /// <c>divergences.txt</c> names, each of which must be used.
    /// </summary>
    private static (string, bool, string) RetainedMatches(string name, string directory, string dataset, string answers, string reference, int least)
    {
        if (Archived($"{directory}/{dataset}.js") is not { } program ||
            Archived($"{directory}/{answers}") is not { } retained ||
            Archived($"{directory}/divergences.txt") is not { } divergences)
        {
            return ("not-run/" + name, false, $"the retained {dataset} are not under this working directory");
        }

        static string Key(string line)
        {
            var arrow = line.IndexOf(" => ", System.StringComparison.Ordinal);
            var resolved = line.IndexOf(" resolved ", System.StringComparison.Ordinal);
            return arrow >= 0 ? line[..arrow] : resolved >= 0 ? line[..resolved] : line;
        }

        var replaced = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

        foreach (var line in System.IO.File.ReadAllLines(divergences))
        {
            if (line.Length != 0 && line[0] != '#')
            {
                replaced[Key(line)] = line;
            }
        }

        var expected = System.IO.File.ReadAllText(retained).TrimEnd('\n').Split('\n');
        var answered = Evaluate(System.IO.File.ReadAllText(program), Composing()).Split('\n');
        var differing = new System.Collections.Generic.List<string>();
        var used = 0;

        for (var line = 0; line < System.Math.Max(expected.Length, answered.Length); line++)
        {
            var want = line < expected.Length ? expected[line] : "(no line)";

            if (replaced.TryGetValue(Key(want), out var divergent))
            {
                want = divergent;
                used++;
            }

            var got = line < answered.Length ? answered[line] : "(no line)";

            if (!string.Equals(want, got, System.StringComparison.Ordinal))
            {
                differing.Add($"line {line + 1}: `{got}` where `{want}` was expected");
            }
        }

        return (
            name,
            differing.Count == 0 && used == replaced.Count && expected.Length > least,
            differing.Count == 0
                ? $"all {expected.Length} lines answer as {reference} did, {used} of them as the divergences name"
                : $"{differing.Count} of {expected.Length} lines differ, first {differing[0]}");
    }

    /// <summary>UTS #35's alias replacement and extension canonicalization, by a few of each kind.</summary>
    private static (string, bool, string) CanonicalizationReplacesAliases()
    {
        const string Name = "intl/i0/canonicalization-replaces-aliases";
        var answer = Evaluate(
            "Intl.getCanonicalLocales(['cmn-hans-cn-u-ca-t-ca-x-t-u', 'sgn-GR', 'ru-SU', 'hy-SU', 'sl-rozaj-biske-1994', " +
            "'und-u-kb-yes', 'und-u-ca-ethiopic-amete-alem', 'en-t-iw', 'und-u-rg-no23']).join(' ');",
            Composing());

        const string Expected = "zh-Hans-CN-t-ca-u-ca-x-t-u gss ru-RU hy-AM sl-1994-biske-rozaj und-u-kb und-u-ca-ethioaa en-t-he und-u-rg-no50";
        return (Name, answer == Expected, $"answered `{answer}` (`{Expected}` expected)");
    }

    /// <summary>One UCA conformance file: every line compares at or below the next.</summary>
    private static (string, bool, string) ConformanceFile(string kind, string file, string options)
    {
        var name = $"intl/i1/the-root-collation-orders-collationtest-{kind}";

        if (Archived("src/tests/cldr/pins/cldr-48.2.0/common/uca/" + file) is not { } path)
        {
            return ("not-run/" + name, false, "the archived CollationTest file is not under this working directory");
        }

        var lines = new System.Collections.Generic.List<string>();

        foreach (var raw in System.IO.File.ReadLines(path))
        {
            if (raw.Length == 0 || raw[0] == '#')
            {
                continue;
            }

            var literal = new System.Text.StringBuilder("'");

            foreach (var part in raw.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
            {
                literal.Append("\\u{").Append(part).Append('}');
            }

            lines.Add(literal.Append('\'').ToString());
        }

        var compared = 0;
        var failures = new System.Collections.Generic.List<string>();

        for (var start = 0; start < lines.Count - 1; start += Chunk)
        {
            var end = System.Math.Min(lines.Count, start + Chunk + 1);
            var source =
                $"var c = new Intl.Collator('en', {options}); var s = [{string.Join(',', lines.GetRange(start, end - start))}];" +
                "var bad = []; for (var i = 1; i < s.length; i++) { if (c.compare(s[i - 1], s[i]) > 0 && bad.length < 5) bad.push(i); }" +
                "bad.join(',');";

            var answer = Evaluate(source, Composing());

            if (answer.Length != 0)
            {
                foreach (var index in answer.Split(','))
                {
                    if (!int.TryParse(index, out var at))
                    {
                        failures.Add(answer);
                        break;
                    }

                    failures.Add($"line {start + at}: {lines[start + at - 1]} > {lines[start + at]}");
                }
            }

            compared += end - start - 1;
        }

        return (
            name,
            failures.Count == 0 && compared > 100_000,
            failures.Count == 0
                ? $"{compared} consecutive pairs of {file} compared in order under {options}"
                : $"{failures.Count} pairs out of order, first {string.Join("; ", failures.GetRange(0, System.Math.Min(5, failures.Count)))}");
    }

    /// <summary>
    /// A UAX #29 conformance file of the pinned UCD (slice I4, JSD-0050): every line's string,
    /// segmented by an <c>Intl.Segmenter</c> of the granularity, begins its segments at exactly the
    /// boundaries the line marks with a division sign.
    /// </summary>
    private static (string, bool, string) BreakTestFile(string granularity, string file, int least)
    {
        var name = $"intl/i4/the-segmenter-passes-{granularity}-break-test";

        if (Archived("src/tests/unicode/pins/ucd-17.0.0/auxiliary/" + file) is not { } path)
        {
            return ("not-run/" + name, false, "the archived break test file is not under this working directory");
        }

        var cases = new System.Collections.Generic.List<(string Literal, string Boundaries, string Line)>();

        foreach (var raw in System.IO.File.ReadLines(path))
        {
            var line = raw.Split('#')[0].Trim();

            if (line.Length == 0)
            {
                continue;
            }

            var literal = new System.Text.StringBuilder("'");
            var boundaries = new System.Collections.Generic.List<int>();
            var units = 0;

            foreach (var token in line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (token == "\u00F7")
                {
                    boundaries.Add(units);
                }
                else if (token != "\u00D7")
                {
                    var codePoint = int.Parse(token, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    literal.Append("\\u{").Append(token).Append('}');
                    units += codePoint > 0xFFFF ? 2 : 1;
                }
            }

            cases.Add((literal.Append('\'').ToString(), string.Join(',', boundaries), line));
        }

        var failures = new System.Collections.Generic.List<string>();

        for (var start = 0; start < cases.Count; start += Chunk)
        {
            var chunk = cases.GetRange(start, System.Math.Min(Chunk, cases.Count - start));
            var source =
                $"var g = new Intl.Segmenter('en', {{ granularity: '{granularity}' }});" +
                $"var t = [{string.Join(',', chunk.ConvertAll(static c => "[" + c.Literal + ",'" + c.Boundaries + "']"))}];" +
                "var bad = []; for (var i = 0; i < t.length; i++) { var s = t[i][0]; var b = [];" +
                " for (var x of g.segment(s)) b.push(x.index); b.push(s.length);" +
                " if (b.join(',') !== t[i][1] && bad.length < 5) bad.push(i + ':' + b.join(',')); }" +
                "bad.join(' ');";

            var answer = Evaluate(source, Composing());

            if (answer.Length != 0)
            {
                foreach (var entry in answer.Split(' '))
                {
                    var colon = entry.IndexOf(':');

                    if (colon < 0 || !int.TryParse(entry[..colon], out var at))
                    {
                        failures.Add(answer);
                        break;
                    }

                    failures.Add($"{chunk[at].Line} gave {entry[(colon + 1)..]}");
                }
            }
        }

        return (
            name,
            failures.Count == 0 && cases.Count >= least,
            failures.Count == 0
                ? $"{cases.Count} lines of {file} segmented at the boundaries they mark"
                : $"{failures.Count} lines differ, first: {string.Join("; ", failures.GetRange(0, System.Math.Min(5, failures.Count)))}");
    }

    /// <summary>A file under the checkout this process runs in, found by walking up from the working directory.</summary>
    private static string? Archived(string relative)
    {
        for (var directory = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            var candidate = System.IO.Path.Combine(directory.FullName, relative.Replace('/', System.IO.Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>A descriptor admitting every surface, with the internationalization data.</summary>
    private static VmProfileDescriptor Composing() =>
        JavaScriptProfile.DescriptorComposing(new JsComposition { IntlData = JsCldrData.Instance });

    /// <summary>Compiles <paramref name="source"/>, runs it as one script under <paramref name="descriptor"/>, and answers its completion.</summary>
    private static string Evaluate(string source, VmProfileDescriptor descriptor)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)], [], new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return "the source was refused: " + (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());
        }

        var created = VmRuntime.Create(VmCatalog.CreateBuilder().Add(descriptor).Build(), Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return $"the runtime refused creation: {created.Outcome}/{created.Reason}";
        }

        using (runtime)
        {
            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in artifactDescriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return $"verification refused: {verified.Outcome}/{verified.Reason}";
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, System.Threading.CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));
                    var result = instance.Invoke(in request, System.Threading.CancellationToken.None);

                    return JavaScriptProfile.TryGetWideCompletion(in result, out var completion) ? completion.Value
                        : JavaScriptProfile.TryGetUncaught(in result, out var uncaught) ? "uncaught " + uncaught.Message
                        : $"{result.Outcome}/{result.Reason}";
                }
            }
        }
    }

    /// <summary>Generous ceilings: a conformance chunk compares twelve thousand pairs in one operation.</summary>
    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, 2_000_000_000),
                VmBudgetDimension.WallClock => VmCeilingSpec.Value(dimension, 120_000),
                VmBudgetDimension.LiveBytes => VmCeilingSpec.Value(dimension, 1_000_000_000),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Enabled,
            capabilities: []);
    }

    /// <summary>An embedder that installs nothing.</summary>
    private sealed class QuietSurface : IJsHostSurface
    {
        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm)
        {
        }
    }
}
