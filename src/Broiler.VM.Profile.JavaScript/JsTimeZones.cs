// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           12
// Human-reviewed:   0/28
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       28
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>An available named time zone identifier (ECMA-402 6.5.1's Time Zone Identifier Record).</summary>
/// <param name="Identifier">The identifier in the IANA Time Zone Database's case.</param>
/// <param name="Primary">The primary identifier it resolves to; itself where it is one.</param>
/// <param name="Zone">The index of the zone whose offsets it reads.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D4E3A5
// Broiler-Human:        PENDING
internal sealed record JsTimeZoneId(string Identifier, string Primary, int Zone);

/// <summary>
/// The IANA Time Zone Database as the profile reads it (JSD-0053): every Zone and Link name, each
/// country's zones, and each zone's UTC offsets, decoded from the tables <c>TzdbTableGenerator</c>
/// writes.
/// </summary>
/// <remarks>
/// <para>
/// <b>The zone table's layout</b> is the generator's: a zone count, then per zone a zig-zag first
/// offset, a palette of offsets, a transition count and each transition as a time delta - minutes
/// with the low bit clear, seconds with it set - and a palette index, then a count of recurring rules
/// and the rules. Every integer is a LEB128 varint, and a signed one zig-zag encoded.
/// </para>
/// <para>
/// <b>A zone is decoded the first time it is read</b>, so a program that names one zone decodes one.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9CD9AE
// Broiler-Human:        PENDING
internal sealed class JsTimeZones
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2CEF10
    // Broiler-Human:        PENDING
    private readonly byte[] table;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BBCBB0
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsTimeZoneId> identifiers =
        new(System.StringComparer.OrdinalIgnoreCase);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EBA767
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<string> primaries = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E91FB6
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, string[]> regions = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=162021
    // Broiler-Human:        PENDING
    private readonly int[] starts;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0CFFB2
    // Broiler-Human:        PENDING
    private readonly JsZone?[] zones;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F39070
    // Broiler-Human:        PENDING
    internal JsTimeZones(
        System.Collections.Generic.IEnumerable<string> identifierLines,
        System.Collections.Generic.IEnumerable<string> regionLines,
        System.ReadOnlySpan<byte> zoneTable)
    {
        table = zoneTable.ToArray();

        foreach (var line in identifierLines)
        {
            var fields = line.Split('|');
            var primary = fields[2].Length == 0 ? fields[0] : fields[2];
            identifiers[fields[0]] = new JsTimeZoneId(fields[0], primary, int.Parse(fields[1], System.Globalization.CultureInfo.InvariantCulture));

            if (fields[2].Length == 0)
            {
                primaries.Add(fields[0]);
            }
        }

        // AN IDENTIFIER READS ITS PRIMARY IDENTIFIER'S OFFSETS, so two identifiers that resolve to one
        // primary never disagree (ECMA-402 6.5.1): EST5EDT, a Zone of its own in tzdb, is CLDR's
        // America/New_York and reads that zone's history.
        foreach (var (name, record) in new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, JsTimeZoneId>>(identifiers))
        {
            if (record.Primary != record.Identifier && identifiers.TryGetValue(record.Primary, out var primary))
            {
                identifiers[name] = record with { Zone = primary.Zone };
            }
        }

        foreach (var line in regionLines)
        {
            var fields = line.Split('|');
            regions[fields[0]] = fields[1..];
        }

        // ONE PASS OVER THE TABLE finds where each zone starts; nothing is decoded yet.
        var at = 0;
        var count = (int)Unsigned(table, ref at);
        starts = new int[count];
        zones = new JsZone?[count];

        for (var zone = 0; zone < count; zone++)
        {
            starts[zone] = at;
            JsZone.Skip(table, ref at);
        }
    }

    /// <summary>The identifiers whose primary identifier is themselves, in ordinal order (ECMA-402 6.5.3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E8DC38
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> PrimaryIdentifiers => primaries;

    /// <summary>The record of an identifier, matched ASCII-case-insensitively (ECMA-402 6.5.2), or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FD5A98
    // Broiler-Human:        PENDING
    internal JsTimeZoneId? Find(string identifier)
    {
        foreach (var c in identifier)
        {
            if (c > 0x7F)
            {
                return null;
            }
        }

        return identifiers.TryGetValue(identifier, out var record) ? record : null;
    }

    /// <summary>The zones <c>zone.tab</c> lists for a region, in ordinal order, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8730C0
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> Region(string region) =>
        regions.TryGetValue(region, out var zoneNames) ? zoneNames : [];

    /// <summary>The decoded zone an identifier reads.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B657BF
    // Broiler-Human:        PENDING
    internal JsZone Zone(JsTimeZoneId identifier) =>
        zones[identifier.Zone] ??= JsZone.Decode(table, starts[identifier.Zone]);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E15CC8
    // Broiler-Human:        PENDING
    internal static ulong Unsigned(byte[] bytes, ref int at)
    {
        ulong value = 0;
        var shift = 0;

        while (true)
        {
            var b = bytes[at++];
            value |= (ulong)(b & 0x7F) << shift;

            if (b < 0x80)
            {
                return value;
            }

            shift += 7;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E1B139
    // Broiler-Human:        PENDING
    internal static long Signed(byte[] bytes, ref int at)
    {
        var value = Unsigned(bytes, ref at);
        return (long)(value >> 1) ^ -(long)(value & 1);
    }
}

/// <summary>A tzdb rule a zone recurs by after its listed transitions.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=562C3C
// Broiler-Human:        PENDING
internal sealed record JsZoneRule(int Month, int OnKind, int Weekday, int Day, int At, int AtKind, int Save);

/// <summary>
/// One zone's UTC offsets: the offset before its first transition, each transition, and the rules
/// that continue them after the last listed year - the reading <c>TzdbCompiler</c> encodes and
/// rule N30 holds to <c>zic</c>'s.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=65CE07
// Broiler-Human:        PENDING
internal sealed class JsZone
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6A2187
    // Broiler-Human:        PENDING
    private const double SecondsPerYear = 31556952.0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=231FB2
    // Broiler-Human:        PENDING
    private JsZone(int initial, long[] at, int[] offsets, int standard, int lastListedYear, JsZoneRule[] rules)
    {
        Initial = initial;
        Transitions = at;
        Offsets = offsets;
        Standard = standard;
        LastListedYear = lastListedYear;
        Rules = rules;
    }

    /// <summary>The offset, in seconds, before the first transition.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3E760
    // Broiler-Human:        PENDING
    internal int Initial { get; }

    /// <summary>Each listed transition, in seconds since the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4ADC35
    // Broiler-Human:        PENDING
    internal long[] Transitions { get; }

    /// <summary>The offset after each listed transition.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C6A241
    // Broiler-Human:        PENDING
    internal int[] Offsets { get; }

    /// <summary>The standard offset the recurring rules add their saves to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=787E01
    // Broiler-Human:        PENDING
    internal int Standard { get; }

    /// <summary>The last year the listed transitions cover, when there are recurring rules.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E95ACE
    // Broiler-Human:        PENDING
    internal int LastListedYear { get; }

    /// <summary>The recurring rules; none where the last offset holds for ever.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AFBA8A
    // Broiler-Human:        PENDING
    internal JsZoneRule[] Rules { get; }

    /// <summary>Whether the recurring rules change the offset at all: they do where two of them save differently.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3D0555
    // Broiler-Human:        PENDING
    private bool RulesChangeOffset
    {
        get
        {
            for (var i = 1; i < Rules.Length; i++)
            {
                if (Rules[i].Save != Rules[0].Save)
                {
                    return true;
                }
            }

            return false;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8F7900
    // Broiler-Human:        PENDING
    internal static void Skip(byte[] table, ref int at)
    {
        JsTimeZones.Signed(table, ref at);
        var palette = (int)JsTimeZones.Unsigned(table, ref at);

        for (var i = 0; i < palette; i++)
        {
            JsTimeZones.Signed(table, ref at);
        }

        var count = (int)JsTimeZones.Unsigned(table, ref at);

        for (var i = 0; i < count; i++)
        {
            JsTimeZones.Unsigned(table, ref at);
            at++;
        }

        var rules = table[at++];

        if (rules == 0)
        {
            return;
        }

        JsTimeZones.Signed(table, ref at);
        JsTimeZones.Signed(table, ref at);

        for (var i = 0; i < rules; i++)
        {
            at += 4;
            JsTimeZones.Signed(table, ref at);
            at++;
            JsTimeZones.Signed(table, ref at);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=349055
    // Broiler-Human:        PENDING
    internal static JsZone Decode(byte[] table, int at)
    {
        var initial = (int)JsTimeZones.Signed(table, ref at);
        var palette = new int[(int)JsTimeZones.Unsigned(table, ref at)];

        for (var i = 0; i < palette.Length; i++)
        {
            palette[i] = (int)JsTimeZones.Signed(table, ref at);
        }

        var count = (int)JsTimeZones.Unsigned(table, ref at);
        var times = new long[count];
        var offsets = new int[count];
        var previous = 0L;

        for (var i = 0; i < count; i++)
        {
            var code = JsTimeZones.Unsigned(table, ref at);
            var zigzag = code >> 1;
            var delta = (long)(zigzag >> 1) ^ -(long)(zigzag & 1);
            previous += (code & 1) == 0 ? delta * 60 : delta;
            times[i] = previous;
            offsets[i] = palette[table[at++]];
        }

        var ruleCount = table[at++];

        if (ruleCount == 0)
        {
            return new JsZone(initial, times, offsets, 0, 0, []);
        }

        var standard = (int)JsTimeZones.Signed(table, ref at);
        var lastListed = (int)JsTimeZones.Signed(table, ref at);
        var rules = new JsZoneRule[ruleCount];

        for (var i = 0; i < ruleCount; i++)
        {
            int month = table[at++], kind = table[at++], weekday = table[at++], day = table[at++];
            var atTime = (int)JsTimeZones.Signed(table, ref at);
            int atKind = table[at++];
            rules[i] = new JsZoneRule(month, kind, weekday, day, atTime, atKind, (int)JsTimeZones.Signed(table, ref at));
        }

        return new JsZone(initial, times, offsets, standard, lastListed, rules);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C1EA48
    // Broiler-Human:        PENDING
    private static int YearOf(long seconds) => 1970 + (int)System.Math.Floor(seconds / SecondsPerYear);

    /// <summary>The UTC offset, in seconds, at an instant in seconds since the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B4B058
    // Broiler-Human:        PENDING
    internal int OffsetAt(long seconds)
    {
        if (Rules.Length > 0 && YearOf(seconds) > LastListedYear)
        {
            return RecurringOffsetAt(seconds);
        }

        var index = System.Array.BinarySearch(Transitions, seconds);
        index = index >= 0 ? index : ~index - 1;
        return index < 0 ? Initial : Offsets[index];
    }

    /// <summary>The UTC offset, in milliseconds, at an instant in milliseconds since the epoch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DF13E1
    // Broiler-Human:        PENDING
    internal double OffsetAtMilliseconds(double milliseconds) =>
        OffsetAt((long)System.Math.Floor(milliseconds / 1000)) * 1000.0;

    /// <summary>
    /// The first instant after <paramref name="seconds"/> at which the offset changes, or nothing;
    /// <paramref name="limit"/> bounds the years a recurring zone is searched.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D3C2AF
    // Broiler-Human:        PENDING
    internal long? NextTransition(long seconds, long limit)
    {
        var index = System.Array.BinarySearch(Transitions, seconds);
        index = index >= 0 ? index + 1 : ~index;

        // THE LISTED TRANSITIONS ARE AUTHORITATIVE through the last listed year; the recurring rules
        // answer only for the years after it.
        if (index < Transitions.Length)
        {
            return Transitions[index];
        }

        if (!RulesChangeOffset)
        {
            return null;
        }

        for (var year = System.Math.Max(YearOf(seconds) - 1, LastListedYear + 1); year <= YearOf(limit) + 1; year++)
        {
            foreach (var (at, before, after) in RecurringTransitions(year))
            {
                if (at > seconds && before != after)
                {
                    return at;
                }
            }
        }

        return null;
    }

    /// <summary>The last instant before <paramref name="seconds"/> at which the offset changes, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=69036F
    // Broiler-Human:        PENDING
    internal long? PreviousTransition(long seconds)
    {
        if (RulesChangeOffset)
        {
            for (var year = YearOf(seconds) + 1; year > LastListedYear; year--)
            {
                var transitions = RecurringTransitions(year);

                for (var i = transitions.Count - 1; i >= 0; i--)
                {
                    var (at, before, after) = transitions[i];

                    if (at < seconds && before != after)
                    {
                        return at;
                    }
                }
            }
        }

        var index = System.Array.BinarySearch(Transitions, seconds);
        index = index >= 0 ? index - 1 : ~index - 1;
        return index >= 0 ? Transitions[index] : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F28844
    // Broiler-Human:        PENDING
    private int RecurringOffsetAt(long seconds)
    {
        var year = YearOf(seconds);
        var offset = Standard + Events(year - 2)[^1].Save;

        foreach (var (at, _, after) in RecurringTransitions(year - 1, year + 1))
        {
            if (at > seconds)
            {
                break;
            }

            offset = after;
        }

        return offset;
    }

    /// <summary>The recurring rules' instants in a range of years, each with the offsets before and after it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FD1206
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<(long At, int Before, int After)> RecurringTransitions(int first, int last = int.MinValue)
    {
        last = last == int.MinValue ? first : last;
        var save = Events(first - 1)[^1].Save;
        var result = new System.Collections.Generic.List<(long At, int Before, int After)>();

        for (var year = first; year <= last; year++)
        {
            foreach (var (local, kind, next) in Events(year))
            {
                var at = kind switch
                {
                    2 => local,
                    1 => local - Standard,
                    _ => local - Standard - save,
                };

                result.Add((at, Standard + save, Standard + next));
                save = next;
            }
        }

        return result;
    }

    /// <summary>The recurring rules' occurrences in one year, in order of local time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=41E09E
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<(long Local, int Kind, int Save)> Events(int year)
    {
        var events = new System.Collections.Generic.List<(long Local, int Kind, int Save)>(Rules.Length);

        foreach (var rule in Rules)
        {
            events.Add(((Day(year, rule) * 86400L) + rule.At, rule.AtKind, rule.Save));
        }

        // STABLE, as the generator's ordering is: two rules at one local time keep their order.
        for (var i = 1; i < events.Count; i++)
        {
            var item = events[i];
            var j = i - 1;

            while (j >= 0 && events[j].Local > item.Local)
            {
                events[j + 1] = events[j];
                j--;
            }

            events[j + 1] = item;
        }

        return events;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=75F443
    // Broiler-Human:        PENDING
    private static long Day(int year, JsZoneRule rule)
    {
        long day;

        switch (rule.OnKind)
        {
            case 1:
                day = DaysFromCivil(year, rule.Month, MonthLength(year, rule.Month));

                while (Weekday(day) != rule.Weekday)
                {
                    day--;
                }

                return day;

            case 2:
            case 3:
                day = DaysFromCivil(year, rule.Month, rule.Day);

                while (Weekday(day) != rule.Weekday)
                {
                    day += rule.OnKind == 2 ? 1 : -1;
                }

                return day;

            default:
                return DaysFromCivil(year, rule.Month, rule.Day);
        }
    }

    /// <summary>Days since 1970-01-01 of a proleptic Gregorian date.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD6C08
    // Broiler-Human:        PENDING
    internal static long DaysFromCivil(long year, int month, int day)
    {
        year -= month <= 2 ? 1 : 0;
        var era = (year >= 0 ? year : year - 399) / 400;
        var yearOfEra = year - (era * 400);
        var dayOfYear = ((153 * (month + (month > 2 ? -3 : 9))) + 2) / 5 + day - 1;
        var dayOfEra = (yearOfEra * 365) + (yearOfEra / 4) - (yearOfEra / 100) + dayOfYear;
        return (era * 146097) + dayOfEra - 719468;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=226F53
    // Broiler-Human:        PENDING
    private static int MonthLength(int year, int month) =>
        month == 2
            ? (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) ? 29 : 28)
            : month is 4 or 6 or 9 or 11 ? 30 : 31;

    /// <summary>Monday 0 to Sunday 6; the epoch was a Thursday.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E5108A
    // Broiler-Human:        PENDING
    private static int Weekday(long day) => (int)(((day + 3) % 7 + 7) % 7);
}
