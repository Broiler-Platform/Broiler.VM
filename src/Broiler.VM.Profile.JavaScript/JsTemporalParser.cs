// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           29
// Human-reviewed:   0/39
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The goal symbols of the proposal's ISO 8601 / RFC 9557 grammar (s13.31) a string is parsed as.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F57452
// Broiler-Human:        PENDING
internal enum TemporalGoal
{
    DateTime,
    ZonedDateTime,
    Instant,
    Time,
    YearMonth,
    MonthDay,
}

/// <summary>An ISO Date-Time Parse Record (s13.34) and the source text of what it was read from.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E6C0D5
// Broiler-Human:        PENDING
internal sealed class JsTemporalParse
{
    /// <summary>The year, or null where a month-day string omitted it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8AA56F
    // Broiler-Human:        PENDING
    internal long? Year { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F19BD6
    // Broiler-Human:        PENDING
    internal int Month { get; set; } = 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2490EF
    // Broiler-Human:        PENDING
    internal int Day { get; set; } = 1;

    /// <summary>Whether the string held a DateDay.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=86E3C5
    // Broiler-Human:        PENDING
    internal bool HasDay { get; set; }

    /// <summary>The time, or null for start-of-day.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=69FF33
    // Broiler-Human:        PENDING
    internal JsTimeRecord? Time { get; set; }

    /// <summary>The ISO String Time Zone Parse Record's [[Z]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=80B3D2
    // Broiler-Human:        PENDING
    internal bool Z { get; set; }

    /// <summary>The UTC offset's source text, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37AE33
    // Broiler-Human:        PENDING
    internal string? Offset { get; set; }

    /// <summary>The time zone annotation's identifier, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=34BD29
    // Broiler-Human:        PENDING
    internal string? TimeZone { get; set; }

    /// <summary>The calendar annotation, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E76345
    // Broiler-Human:        PENDING
    internal string? Calendar { get; set; }
}

/// <summary>
/// A recursive-descent parser for the proposal's grammar (s13.31): each production a method that
/// advances over its text or restores the position and fails, so the alternatives backtrack as the
/// grammar's do.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0D9E09
// Broiler-Human:        PENDING
internal sealed class JsTemporalParser
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A42375
    // Broiler-Human:        PENDING
    private readonly string text;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=98C321
    // Broiler-Human:        PENDING
    private int at;

    // THE PARSE NODES ParseISODateTime READS, captured as the productions match.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4283DE
    // Broiler-Human:        PENDING
    private string? year;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6EE8D7
    // Broiler-Human:        PENDING
    private string? month;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DED7DA
    // Broiler-Human:        PENDING
    private string? day;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C5E6AF
    // Broiler-Human:        PENDING
    private string? hour;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7A73A8
    // Broiler-Human:        PENDING
    private string? minute;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5350F0
    // Broiler-Human:        PENDING
    private string? second;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1557DA
    // Broiler-Human:        PENDING
    private string? fraction;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5A7380
    // Broiler-Human:        PENDING
    private bool z;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=15D4B3
    // Broiler-Human:        PENDING
    private string? offset;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=54EAF8
    // Broiler-Human:        PENDING
    private string? timeZone;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A7A2EE
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(bool Critical, string Key, string Value)> annotations = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3509EC
    // Broiler-Human:        PENDING
    private JsTemporalParser(string text) => this.text = text;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1D9D41
    // Broiler-Human:        PENDING
    private char Peek(int ahead = 0) => at + ahead < text.Length ? text[at + ahead] : '\0';

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6079DB
    // Broiler-Human:        PENDING
    private bool Eat(char c)
    {
        if (Peek() == c)
        {
            at++;
            return true;
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3F973
    // Broiler-Human:        PENDING
    private bool EatAny(string set)
    {
        if (at < text.Length && set.Contains(text[at], System.StringComparison.Ordinal))
        {
            at++;
            return true;
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3DF33
    // Broiler-Human:        PENDING
    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=05C4FA
    // Broiler-Human:        PENDING
    private static bool IsAlpha(char c) => c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z');

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C9FA64
    // Broiler-Human:        PENDING
    private string? Digits(int count)
    {
        if (at + count > text.Length)
        {
            return null;
        }

        for (var i = 0; i < count; i++)
        {
            if (!IsDigit(text[at + i]))
            {
                return null;
            }
        }

        var result = text.Substring(at, count);
        at += count;
        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=03BA5F
    // Broiler-Human:        PENDING
    private string? Bounded(int maximum)
    {
        var start = at;
        var digits = Digits(2);

        if (digits is null || int.Parse(digits, System.Globalization.CultureInfo.InvariantCulture) > maximum)
        {
            at = start;
            return null;
        }

        return digits;
    }

    // ---- Dates ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=67445B
    // Broiler-Human:        PENDING
    private bool DateYear()
    {
        var start = at;

        if (EatAny("+-"))
        {
            var digits = Digits(6);

            if (digits is null || (text[start] == '-' && digits == "000000"))
            {
                at = start;
                return false;
            }

            year = text[start..at];
            return true;
        }

        var four = Digits(4);

        if (four is null)
        {
            at = start;
            return false;
        }

        year = four;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A0DD74
    // Broiler-Human:        PENDING
    private bool DateMonth()
    {
        var start = at;
        var digits = Digits(2);

        if (digits is null || digits == "00" || int.Parse(digits, System.Globalization.CultureInfo.InvariantCulture) > 12)
        {
            at = start;
            return false;
        }

        month = digits;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=863AE3
    // Broiler-Human:        PENDING
    private bool DateDay()
    {
        var start = at;
        var digits = Digits(2);

        if (digits is null || digits == "00" || int.Parse(digits, System.Globalization.CultureInfo.InvariantCulture) > 31)
        {
            at = start;
            return false;
        }

        day = digits;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E699A3
    // Broiler-Human:        PENDING
    private bool ValidMonthDay() =>
        !(day == "31" && month is "02" or "04" or "06" or "09" or "11") && !(month == "02" && day == "30");

    /// <summary>DateSpec[Extended] with IsValidDate.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8720EA
    // Broiler-Human:        PENDING
    private bool DateSpec()
    {
        var start = at;

        foreach (var extended in new[] { true, false })
        {
            at = start;

            if (DateYear() && (!extended || Eat('-')) && DateMonth() && (!extended || Eat('-')) && DateDay())
            {
                if (!ValidMonthDay() ||
                    (month == "02" && day == "29" && !JsTemporalCore.IsLeapYear(long.Parse(year!, System.Globalization.CultureInfo.InvariantCulture))))
                {
                    // AN EARLY ERROR, not an alternative: the date matched and is not a date.
                    at = start;
                    return false;
                }

                return true;
            }
        }

        at = start;
        year = month = day = null;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8447C7
    // Broiler-Human:        PENDING
    private bool DateSpecYearMonth()
    {
        var start = at;

        if (DateYear() && (Eat('-') || true) && DateMonth())
        {
            return true;
        }

        at = start;
        year = month = null;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4F4BCC
    // Broiler-Human:        PENDING
    private bool DateSpecMonthDay()
    {
        var start = at;

        if (Peek() == '-' && Peek(1) == '-')
        {
            at += 2;
        }

        var afterDashes = at;

        foreach (var extended in new[] { true, false })
        {
            at = afterDashes;

            if (DateMonth() && (!extended || Eat('-')) && DateDay())
            {
                if (!ValidMonthDay())
                {
                    at = start;
                    return false;
                }

                return true;
            }
        }

        at = start;
        month = day = null;
        return false;
    }

    // ---- Times ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D72B76
    // Broiler-Human:        PENDING
    private bool Fraction()
    {
        var start = at;

        if (!EatAny(".,"))
        {
            return false;
        }

        var digitsStart = at;

        while (at < text.Length && IsDigit(text[at]) && at - digitsStart < 9)
        {
            at++;
        }

        if (at == digitsStart || (at < text.Length && IsDigit(text[at])))
        {
            at = start;
            return false;
        }

        fraction ??= text[start..at];
        return true;
    }

    /// <summary>TimeSpec[Extended] of either form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D1D2D2
    // Broiler-Human:        PENDING
    private bool TimeSpec()
    {
        var start = at;

        foreach (var extended in new[] { true, false })
        {
            at = start;
            var h = Bounded(23);

            if (h is null)
            {
                return false;
            }

            hour = h;
            var afterHour = at;

            if ((!extended || Eat(':')) && Bounded(59) is { } m)
            {
                minute = m;
                var afterMinute = at;

                if (!extended || Eat(':'))
                {
                    var secondStart = at;
                    var s = Digits(2);

                    if (s is not null && (int.Parse(s, System.Globalization.CultureInfo.InvariantCulture) <= 59 || s == "60"))
                    {
                        second = s;
                        Fraction();
                        return true;
                    }

                    at = secondStart;
                }

                at = afterMinute;

                // HOUR AND MINUTE ALONE, in the form that wrote its separator.
                if (extended || PeekNotDigit())
                {
                    return true;
                }

                at = afterMinute;
                return true;
            }

            at = afterHour;
            minute = null;

            if (!extended)
            {
                return true;
            }
        }

        at = start;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A240F3
    // Broiler-Human:        PENDING
    private bool PeekNotDigit() => !IsDigit(Peek());

    // ---- Offsets and time zones ---------------------------------------------------------------

    /// <summary>UTCOffset[SubMinutePrecision]: its source text, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=815B96
    // Broiler-Human:        PENDING
    private string? UtcOffset(bool subMinute)
    {
        var start = at;

        if (!EatAny("+-") || Bounded(23) is null)
        {
            at = start;
            return null;
        }

        var afterHour = at;

        foreach (var extended in new[] { true, false })
        {
            at = afterHour;

            if ((extended && !Eat(':')) || Bounded(59) is null)
            {
                continue;
            }

            var afterMinute = at;

            if (subMinute && (!extended || Eat(':')) && Bounded(59) is not null)
            {
                var savedFraction = fraction;
                fraction = null;
                Fraction();
                fraction = savedFraction;
                return text[start..at];
            }

            at = afterMinute;
            return text[start..at];
        }

        at = afterHour;
        return text[start..at];
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1B966
    // Broiler-Human:        PENDING
    private bool TimeZoneIanaName()
    {
        var start = at;

        while (true)
        {
            if (!(IsAlpha(Peek()) || Peek() is '.' or '_'))
            {
                at = start;
                return false;
            }

            at++;

            while (IsAlpha(Peek()) || IsDigit(Peek()) || Peek() is '.' or '_' or '-' or '+')
            {
                at++;
            }

            if (!Eat('/'))
            {
                return true;
            }
        }
    }

    /// <summary>TimeZoneIdentifier: an offset without seconds or an IANA name; its source text, or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3B3BFF
    // Broiler-Human:        PENDING
    private string? TimeZoneIdentifier()
    {
        var start = at;

        if (UtcOffset(subMinute: false) is { } offsetText)
        {
            return offsetText;
        }

        at = start;
        return TimeZoneIanaName() ? text[start..at] : null;
    }

    /// <summary>A bracketed suffix: a time zone annotation where one is allowed, then key-value annotations.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=03233C
    // Broiler-Human:        PENDING
    private bool Annotations(bool allowTimeZone)
    {
        var first = true;

        while (Peek() == '[')
        {
            var start = at;
            at++;
            var critical = Eat('!');
            var close = text.IndexOf(']', at);

            if (close < 0)
            {
                at = start;
                return false;
            }

            var inner = text[at..close];

            if (inner.Contains('=', System.StringComparison.Ordinal))
            {
                var equals = inner.IndexOf('=', System.StringComparison.Ordinal);
                var key = inner[..equals];
                var value = inner[(equals + 1)..];

                if (!IsAnnotationKey(key) || !IsAnnotationValue(value))
                {
                    at = start;
                    return false;
                }

                annotations.Add((critical, key, value));
                at = close + 1;
            }
            else
            {
                if (!first || !allowTimeZone)
                {
                    at = start;
                    return false;
                }

                var identifierStart = at;

                if (TimeZoneIdentifier() is null || at != close)
                {
                    at = start;
                    return false;
                }

                timeZone = text[identifierStart..close];
                at = close + 1;
            }

            first = false;
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E724F9
    // Broiler-Human:        PENDING
    private static bool IsAnnotationKey(string key)
    {
        if (key.Length == 0 || !(key[0] is (>= 'a' and <= 'z') or '_'))
        {
            return false;
        }

        foreach (var c in key)
        {
            if (!(c is (>= 'a' and <= 'z') or '_' or '-' || IsDigit(c)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The grammar's AnnotationValue: alphanumeric components joined by hyphens.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BDE7F9
    // Broiler-Human:        PENDING
    internal static bool IsAnnotationValue(string value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        foreach (var component in value.Split('-'))
        {
            if (component.Length == 0)
            {
                return false;
            }

            foreach (var c in component)
            {
                if (!(IsAlpha(c) || IsDigit(c)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // ---- Goals ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DE5FED
    // Broiler-Human:        PENDING
    private void Reset()
    {
        at = 0;
        year = month = day = hour = minute = second = fraction = offset = timeZone = null;
        z = false;
        annotations.Clear();
    }

    /// <summary>DateTimeUTCOffset[Z]: a Z where allowed, or an offset with sub-minute precision.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1E35BE
    // Broiler-Human:        PENDING
    private bool DateTimeOffset(bool allowZ)
    {
        if (allowZ && EatAny("Zz"))
        {
            z = true;
            return true;
        }

        if (Peek() is 'Z' or 'z')
        {
            return false;
        }

        if (UtcOffset(subMinute: true) is { } text)
        {
            offset = text;
            return true;
        }

        return true;
    }

    /// <summary>DateTime[Z, TimeRequired] and its suffixes, for the date-time goals.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4721DE
    // Broiler-Human:        PENDING
    private bool AnnotatedDateTime(bool zoned, bool timeRequired, bool instant)
    {
        Reset();

        if (!DateSpec())
        {
            return false;
        }

        if (EatAny(" Tt"))
        {
            if (!TimeSpec())
            {
                return false;
            }

            if (!DateTimeOffset(allowZ: zoned || instant))
            {
                return false;
            }

            if (instant && !z && offset is null)
            {
                return false;
            }
        }
        else if (timeRequired || instant)
        {
            return false;
        }

        if (!Annotations(allowTimeZone: true))
        {
            return false;
        }

        if (zoned && timeZone is null)
        {
            return false;
        }

        return at == text.Length;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EB19EB
    // Broiler-Human:        PENDING
    private bool AnnotatedTime()
    {
        Reset();
        var designated = EatAny("Tt");
        var timeStart = at;

        if (!TimeSpec() || !DateTimeOffset(allowZ: false))
        {
            return false;
        }

        var timeEnd = at;

        if (!Annotations(allowTimeZone: true) || at != text.Length)
        {
            return false;
        }

        if (!designated)
        {
            // THE EARLY ERRORS OF s13.31.3: a time without its designator must not also be a
            // month-day or a year-month.
            var span = text[timeStart..timeEnd];

            if (IsWhole(span, static parser => parser.DateSpecMonthDay()) || IsWhole(span, static parser => parser.DateSpecYearMonth()))
            {
                return false;
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=89B8CF
    // Broiler-Human:        PENDING
    private static bool IsWhole(string span, System.Func<JsTemporalParser, bool> production)
    {
        var parser = new JsTemporalParser(span);
        return production(parser) && parser.at == span.Length;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=43CEB8
    // Broiler-Human:        PENDING
    private bool AnnotatedYearMonth()
    {
        Reset();
        return DateSpecYearMonth() && Annotations(allowTimeZone: true) && at == text.Length;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=69985C
    // Broiler-Human:        PENDING
    private bool AnnotatedMonthDay()
    {
        Reset();
        return DateSpecMonthDay() && Annotations(allowTimeZone: true) && at == text.Length;
    }

    /// <summary>
    /// The proposal's ParseISODateTime (s13.35): the first of <paramref name="goals"/> the string
    /// parses as, with its annotations checked, or the RangeError.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E11A28
    // Broiler-Human:        PENDING
    internal static JsTemporalParse Parse(JsEngine engine, string text, params TemporalGoal[] goals)
    {
        var parser = new JsTemporalParser(text);
        string? calendar = null;
        var yearAbsent = false;
        var matched = false;

        foreach (var goal in goals)
        {
            matched = goal switch
            {
                TemporalGoal.DateTime => parser.AnnotatedDateTime(zoned: false, timeRequired: false, instant: false),
                TemporalGoal.ZonedDateTime => parser.AnnotatedDateTime(zoned: true, timeRequired: false, instant: false),
                TemporalGoal.Instant => parser.AnnotatedDateTime(zoned: false, timeRequired: true, instant: true),
                TemporalGoal.Time => parser.AnnotatedTime() || parser.AnnotatedDateTime(zoned: false, timeRequired: true, instant: false),
                TemporalGoal.YearMonth => parser.AnnotatedYearMonth() || parser.AnnotatedDateTime(zoned: false, timeRequired: false, instant: false),
                _ => parser.AnnotatedMonthDay() || parser.AnnotatedDateTime(zoned: false, timeRequired: false, instant: false),
            };

            if (!matched)
            {
                continue;
            }

            var calendarWasCritical = false;

            foreach (var (critical, key, value) in parser.annotations)
            {
                if (key == "u-ca")
                {
                    if (calendar is null)
                    {
                        calendar = value;
                        calendarWasCritical = critical;
                    }
                    else if (critical || calendarWasCritical)
                    {
                        throw engine.Error("RangeError", "Temporal: a critical calendar annotation conflicts with another in " + text);
                    }
                }
                else if (critical)
                {
                    throw engine.Error("RangeError", "Temporal: the critical annotation " + key + " is not recognized in " + text);
                }
            }

            if (goal == TemporalGoal.YearMonth && parser.day is null &&
                calendar is not null && !string.Equals(calendar, "iso8601", System.StringComparison.OrdinalIgnoreCase))
            {
                throw engine.Error("RangeError", "Temporal: a year-month without a day must be in the ISO 8601 calendar: " + text);
            }

            if (goal == TemporalGoal.MonthDay && parser.year is null)
            {
                if (calendar is not null && !string.Equals(calendar, "iso8601", System.StringComparison.OrdinalIgnoreCase))
                {
                    throw engine.Error("RangeError", "Temporal: a month-day without a year must be in the ISO 8601 calendar: " + text);
                }

                yearAbsent = true;
            }

            break;
        }

        if (!matched)
        {
            throw engine.Error("RangeError", "Temporal: cannot parse " + Quoted(text));
        }

        static int Number(string? digits, int fallback) =>
            digits is null ? fallback : int.Parse(digits, System.Globalization.CultureInfo.InvariantCulture);

        var result = new JsTemporalParse
        {
            Year = yearAbsent || parser.year is null ? null : long.Parse(parser.year, System.Globalization.CultureInfo.InvariantCulture),
            Month = Number(parser.month, 1),
            Day = Number(parser.day, 1),
            HasDay = parser.day is not null,
            Z = parser.z,
            Offset = parser.offset,
            TimeZone = parser.timeZone,
            Calendar = calendar,
        };

        if (parser.hour is not null)
        {
            var secondValue = Number(parser.second, 0);
            var digits = parser.fraction is null ? "000000000" : (parser.fraction[1..] + "000000000")[..9];
            result.Time = new JsTimeRecord(
                0,
                Number(parser.hour, 0),
                Number(parser.minute, 0),
                secondValue == 60 ? 59 : secondValue,
                int.Parse(digits[..3], System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(digits[3..6], System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(digits[6..], System.Globalization.CultureInfo.InvariantCulture));
        }

        return result;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=12D3D8
    // Broiler-Human:        PENDING
    private static string Quoted(string text) => "'" + (text.Length > 64 ? text[..64] + "..." : text) + "'";

    /// <summary>Whether the whole string is a TimeZoneIdentifier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4D8473
    // Broiler-Human:        PENDING
    internal static bool IsTimeZoneIdentifier(string text)
    {
        var parser = new JsTemporalParser(text);
        return parser.TimeZoneIdentifier() is not null && parser.at == text.Length;
    }

    /// <summary>Whether the whole string is a UTCOffset of the given precision.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75F33D
    // Broiler-Human:        PENDING
    internal static bool IsUtcOffset(string text, bool subMinute)
    {
        var parser = new JsTemporalParser(text);
        return parser.UtcOffset(subMinute) is not null && parser.at == text.Length;
    }

    /// <summary>
    /// The proposal's ParseDateTimeUTCOffset (s14.6.11) of a string the grammar's UTCOffset with
    /// sub-minute precision matches: its nanoseconds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=245405
    // Broiler-Human:        PENDING
    internal static long OffsetNanoseconds(string offset)
    {
        var sign = offset[0] == '-' ? -1L : 1L;
        var digits = new System.Text.StringBuilder();
        string? fractionText = null;

        for (var i = 1; i < offset.Length; i++)
        {
            if (offset[i] is '.' or ',')
            {
                fractionText = offset[(i + 1)..];
                break;
            }

            if (IsDigit(offset[i]))
            {
                digits.Append(offset[i]);
            }
        }

        var all = digits.ToString();
        var hours = long.Parse(all[..2], System.Globalization.CultureInfo.InvariantCulture);
        var minutes = all.Length >= 4 ? long.Parse(all[2..4], System.Globalization.CultureInfo.InvariantCulture) : 0;
        var seconds = all.Length >= 6 ? long.Parse(all[4..6], System.Globalization.CultureInfo.InvariantCulture) : 0;
        var nanoseconds = fractionText is null ? 0 : long.Parse((fractionText + "000000000")[..9], System.Globalization.CultureInfo.InvariantCulture);
        return sign * ((((hours * 60) + minutes) * 60 + seconds) * 1_000_000_000L + nanoseconds);
    }

    /// <summary>
    /// The proposal's ParseTemporalDurationString (s13.37): the ten fields, exact, or null where the
    /// string is not a duration.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=05591E
    // Broiler-Human:        PENDING
    internal static double[]? Duration(string text)
    {
        var at = 0;
        var negative = false;

        if (at < text.Length && text[at] is '+' or '-')
        {
            negative = text[at] == '-';
            at++;
        }

        if (at >= text.Length || text[at] is not ('P' or 'p'))
        {
            return null;
        }

        at++;

        System.Numerics.BigInteger years = 0, months = 0, weeks = 0, days = 0, hours = 0, minutes = 0, seconds = 0;
        System.Numerics.BigInteger totalSubNs = 0;
        var any = false;
        var timeAny = false;
        var inTime = false;
        var order = 0;
        var fractionSeen = false;

        // EACH COMPONENT IS DIGITS, AN OPTIONAL FRACTION (time units only, and only the last), AND
        // ITS DESIGNATOR, in the grammar's order.
        while (at < text.Length)
        {
            if (text[at] is 'T' or 't')
            {
                if (inTime)
                {
                    return null;
                }

                inTime = true;
                at++;
                continue;
            }

            if (fractionSeen)
            {
                return null;
            }

            var start = at;

            while (at < text.Length && IsDigit(text[at]))
            {
                at++;
            }

            if (at == start)
            {
                return null;
            }

            var whole = System.Numerics.BigInteger.Parse(text[start..at], System.Globalization.CultureInfo.InvariantCulture);
            string? fractionDigits = null;

            if (at < text.Length && text[at] is '.' or ',')
            {
                var fractionStart = ++at;

                while (at < text.Length && IsDigit(text[at]))
                {
                    at++;
                }

                if (at == fractionStart || at - fractionStart > 9)
                {
                    return null;
                }

                fractionDigits = text[fractionStart..at];
            }

            if (at >= text.Length)
            {
                return null;
            }

            var designator = char.ToUpperInvariant(text[at++]);
            int rank;

            if (!inTime)
            {
                rank = designator switch { 'Y' => 1, 'M' => 2, 'W' => 3, 'D' => 4, _ => 0 };

                if (rank == 0 || fractionDigits is not null)
                {
                    return null;
                }
            }
            else
            {
                rank = designator switch { 'H' => 5, 'M' => 6, 'S' => 7, _ => 0 };

                if (rank == 0)
                {
                    return null;
                }

                timeAny = true;
            }

            if (rank <= order)
            {
                return null;
            }

            order = rank;
            any = true;

            // THE NANOSECONDS A FRACTION CONTRIBUTES, exactly: a fraction of an hour or minute is
            // carried into the smaller units by s13.37's steps, which together keep its exact value.
            System.Numerics.BigInteger fractionNs = 0;

            if (fractionDigits is not null)
            {
                fractionSeen = true;
                var unitNs = rank switch { 5 => 3_600_000_000_000L, 6 => 60_000_000_000L, _ => 1_000_000_000L };
                var scale = System.Numerics.BigInteger.Pow(10, fractionDigits.Length);
                fractionNs = System.Numerics.BigInteger.Parse(fractionDigits, System.Globalization.CultureInfo.InvariantCulture) * unitNs / scale;
            }

            switch (rank)
            {
                case 1: years = whole; break;
                case 2: months = whole; break;
                case 3: weeks = whole; break;
                case 4: days = whole; break;
                case 5: hours = whole; totalSubNs = fractionNs; break;
                case 6: minutes = whole; totalSubNs = fractionNs; break;
                default: seconds = whole; totalSubNs = fractionNs; break;
            }
        }

        if (!any || (inTime && !timeAny))
        {
            return null;
        }

        // s13.37 SPLITS THE FRACTION'S NANOSECONDS INTO WHOLE SECONDS ONLY WHERE THE FRACTION WAS AN
        // HOUR'S OR MINUTE'S; a seconds fraction is milliseconds and smaller.
        System.Numerics.BigInteger extraMinutes = 0, extraSeconds = 0;

        if (order == 5)
        {
            extraMinutes = totalSubNs / 60_000_000_000L;
            totalSubNs %= 60_000_000_000L;
        }

        if (order >= 5 && order <= 6)
        {
            extraSeconds = totalSubNs / 1_000_000_000L;
            totalSubNs %= 1_000_000_000L;
        }

        var milliseconds = totalSubNs / 1_000_000;
        var microseconds = totalSubNs / 1000 % 1000;
        var nanoseconds = totalSubNs % 1000;
        var factor = negative ? -1 : 1;

        double D(System.Numerics.BigInteger value) => JsTemporalCore.ToDouble(value * factor);

        return
        [
            D(years), D(months), D(weeks), D(days), D(hours), D(minutes + extraMinutes), D(seconds + extraSeconds),
            D(milliseconds), D(microseconds), D(nanoseconds),
        ];
    }
}
