using System.Globalization;

namespace Broiler.VM.Architecture.Tests;

/// <summary>A tzdb <c>Rule</c> line: the years it covers, the day and time it falls on, and its save.</summary>
/// <param name="To">The last year, or null for <c>max</c>.</param>
/// <param name="On">The ON field as written: a day, <c>lastSun</c>, <c>Sun&gt;=8</c> or <c>Sun&lt;=25</c>.</param>
/// <param name="AtKind">How AT is read: <c>w</c> wall clock, <c>s</c> standard time, <c>u</c> universal time.</param>
internal sealed record TzdbRule(int From, int? To, int Month, string On, int At, char AtKind, int Save);

/// <summary>A tzdb <c>Zone</c> line or continuation: its standard offset, its rules, and when it ends.</summary>
/// <param name="Rules">The RULES field: <c>-</c>, a fixed save such as <c>1:00</c>, or a rule set's name.</param>
/// <param name="Until">The UNTIL fields, or an empty array on the zone's last line.</param>
internal sealed record TzdbZoneLine(int StandardOffset, string Rules, string[] Until);

/// <summary>A zone's recurring rules after the last year its transitions are listed for.</summary>
internal sealed record TzdbFinalRules(int StandardOffset, int LastListedYear, IReadOnlyList<TzdbRule> Rules);

/// <summary>A compiled zone: its first offset, each later change of offset, and the rules that continue it.</summary>
/// <param name="Transitions">
/// Each instant, in seconds since the epoch, at which the UTC offset changes, the offset after it,
/// and whether that offset is daylight saving time as CLDR names it (the rearguard reading, in which
/// a negative save makes the other time of the year the daylight one).
/// </param>
/// <param name="Final">The rules for every year after the listed transitions, or null when the last offset holds for ever.</param>
internal sealed record TzdbZone(int InitialOffset, IReadOnlyList<(long At, int Offset, bool Daylight)> Transitions, TzdbFinalRules? Final);

/// <summary>
/// A compiler of tzdb source into UTC offsets, as <c>zic</c> compiles it (decision JSD-0053): each
/// zone's lines in order, each line's rules in chronological order, and <c>zic</c>'s own merge of a
/// transition whose wall clock does not advance into the one before it.
/// </summary>
/// <remarks>
/// <para>
/// <b>Offsets, and the daylight flag CLDR's names read.</b> Temporal and <c>Intl.DateTimeFormat</c>
/// read a zone's UTC offset at an instant and the instants at which it changes; abbreviations are not
/// kept. A change of the daylight flag alone is kept as a transition for the names, and the reader
/// skips it where Temporal asks for the next or previous change of offset
/// (GetNamedTimeZoneNextTransition). <c>zic</c>'s own flag, a save other than zero, is tracked while
/// compiling, because its merge compares whole types; the flag kept is the rearguard one ICU and
/// CLDR's names read (JSD-0058): where a line's rules save a negative amount, the larger save is the
/// daylight one.
/// </para>
/// <para>
/// <b>Checked against <c>zic</c></b>: on 2026-10-05 every one of 2026e's 597 identifiers gave the
/// offsets <c>zic -b fat</c> of the same nine files gives, at each of its transitions and the second
/// before, and every 41 days from 1800 to 2500 - a prototype of this compiler did, and the slice
/// compiler's check holds the profile's reading of these tables to Node's.
/// </para>
/// </remarks>
internal static class TzdbCompiler
{
    /// <summary>The source files <c>zic</c> compiles by default, in the order IANA's Makefile lists them.</summary>
    internal static readonly string[] SourceFiles =
        ["africa", "antarctica", "asia", "australasia", "europe", "northamerica", "southamerica", "etcetera", "backward"];

    private static readonly string[] MonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    private static readonly string[] DayNames = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    /// <summary>The rule sets, zones and links of the source files, by name.</summary>
    internal sealed record Source(
        IReadOnlyDictionary<string, List<TzdbRule>> Rules,
        IReadOnlyDictionary<string, List<TzdbZoneLine>> Zones,
        IReadOnlyDictionary<string, string> Links);

    /// <summary>Reads the source files' Rule, Zone and Link lines.</summary>
    internal static Source Parse(IReadOnlyDictionary<string, byte[]> archive)
    {
        var rules = new Dictionary<string, List<TzdbRule>>(StringComparer.Ordinal);
        var zones = new Dictionary<string, List<TzdbZoneLine>>(StringComparer.Ordinal);
        var links = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var file in SourceFiles)
        {
            List<TzdbZoneLine>? zone = null;

            foreach (var raw in TzdbPin.Decode(archive[file]).Split('\n'))
            {
                var hash = raw.IndexOf('#', StringComparison.Ordinal);
                var line = (hash < 0 ? raw : raw[..hash]).TrimEnd();

                if (line.Trim().Length == 0)
                {
                    continue;
                }

                var fields = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                var continuation = line[0] is ' ' or '\t';

                if (!continuation && fields[0] == "Rule")
                {
                    var from = int.Parse(fields[2], CultureInfo.InvariantCulture);
                    int? to = fields[3].StartsWith('o') ? from : fields[3].StartsWith("ma", StringComparison.Ordinal) ? null : int.Parse(fields[3], CultureInfo.InvariantCulture);
                    var (at, kind) = Time(fields[7]);

                    if (!rules.TryGetValue(fields[1], out var set))
                    {
                        rules[fields[1]] = set = [];
                    }

                    set.Add(new TzdbRule(from, to, Month(fields[5]), fields[6], at, kind, Time(fields[8]).Seconds));
                    zone = null;
                }
                else if (!continuation && fields[0] == "Link")
                {
                    links[fields[2]] = fields[1];
                    zone = null;
                }
                else if (!continuation && fields[0] == "Zone")
                {
                    zone = [];
                    zones[fields[1]] = zone;
                    zone.Add(ZoneLine(fields[2..]));
                }
                else if (continuation && zone is not null)
                {
                    zone.Add(ZoneLine(fields));
                }
                else
                {
                    throw new InvalidDataException($"tzdb {file}: `{line}` is neither a Rule, a Zone, a Link nor a continuation");
                }
            }
        }

        return new Source(rules, zones, links);
    }

    private static TzdbZoneLine ZoneLine(string[] fields) =>
        new(Time(fields[0]).Seconds, fields[1], fields.Length > 3 ? fields[3..] : []);

    /// <summary>A month field, by its first three letters.</summary>
    internal static int Month(string text) =>
        Array.IndexOf(MonthNames, char.ToUpperInvariant(text[0]) + text[1..3].ToLowerInvariant()) + 1 is var month and > 0
            ? month
            : throw new InvalidDataException($"tzdb: `{text}` is not a month");

    /// <summary>A weekday, Monday 0, by its first three letters.</summary>
    internal static int Weekday(string text) =>
        Array.IndexOf(DayNames, char.ToUpperInvariant(text[0]) + text[1..3].ToLowerInvariant()) is var day and >= 0
            ? day
            : throw new InvalidDataException($"tzdb: `{text}` is not a day");

    /// <summary>A time field, <c>[-]h[:mm[:ss]]</c> with an optional suffix: seconds, and <c>w</c>, <c>s</c> or <c>u</c>.</summary>
    internal static (int Seconds, char Kind) Time(string text)
    {
        var kind = 'w';

        if (text.Length > 0 && text[^1] is 'w' or 's' or 'u' or 'g' or 'z' or 'd')
        {
            kind = text[^1] switch { 'g' or 'z' => 'u', 'd' => 'w', var c => c };
            text = text[..^1];
        }

        if (text is "-" or "")
        {
            return (0, kind);
        }

        var sign = 1;

        if (text[0] == '-')
        {
            sign = -1;
            text = text[1..];
        }

        var parts = text.Split(':');
        var seconds = int.Parse(parts[0], CultureInfo.InvariantCulture) * 3600;

        if (parts.Length > 1)
        {
            seconds += int.Parse(parts[1], CultureInfo.InvariantCulture) * 60;
        }

        if (parts.Length > 2)
        {
            seconds += (int)double.Parse(parts[2], CultureInfo.InvariantCulture);
        }

        return (sign * seconds, kind);
    }

    /// <summary>Days since 1970-01-01 of a proleptic Gregorian date.</summary>
    internal static long DaysFromCivil(long year, int month, int day)
    {
        year -= month <= 2 ? 1 : 0;
        var era = (year >= 0 ? year : year - 399) / 400;
        var yearOfEra = year - (era * 400);
        var dayOfYear = ((153 * (month + (month > 2 ? -3 : 9))) + 2) / 5 + day - 1;
        var dayOfEra = (yearOfEra * 365) + (yearOfEra / 4) - (yearOfEra / 100) + dayOfYear;
        return (era * 146097) + dayOfEra - 719468;
    }

    private static int MonthLength(long year, int month) =>
        month == 2
            ? (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) ? 29 : 28)
            : month is 4 or 6 or 9 or 11 ? 30 : 31;

    /// <summary>Monday 0 to Sunday 6, of a day since the epoch, which was a Thursday.</summary>
    private static int DayOfWeek(long days) => (int)(((days + 3) % 7 + 7) % 7);

    /// <summary>The day, since the epoch, an ON field names in a year and month.</summary>
    internal static long OnDay(long year, int month, string on)
    {
        if (on.StartsWith("last", StringComparison.Ordinal))
        {
            var weekday = Weekday(on[4..]);
            var day = DaysFromCivil(year, month, MonthLength(year, month));

            while (DayOfWeek(day) != weekday)
            {
                day--;
            }

            return day;
        }

        var after = on.IndexOf(">=", StringComparison.Ordinal);
        var before = on.IndexOf("<=", StringComparison.Ordinal);

        if (after > 0 || before > 0)
        {
            var at = after > 0 ? after : before;
            var weekday = Weekday(on[..at]);
            var day = DaysFromCivil(year, month, int.Parse(on[(at + 2)..], CultureInfo.InvariantCulture));

            while (DayOfWeek(day) != weekday)
            {
                day += after > 0 ? 1 : -1;
            }

            return day;
        }

        return DaysFromCivil(year, month, int.Parse(on, CultureInfo.InvariantCulture));
    }

    private static int YearOf(long seconds) => 1970 + (int)Math.Floor(seconds / 31556952.0);

    private static (long Local, char Kind) UntilLocal(string[] until)
    {
        var year = int.Parse(until[0], CultureInfo.InvariantCulture);
        var month = until.Length > 1 ? Month(until[1]) : 1;
        var day = until.Length > 2 ? OnDay(year, month, until[2]) : DaysFromCivil(year, month, 1);
        var (time, kind) = until.Length > 3 ? Time(until[3]) : (0, 'w');
        return ((day * 86400) + time, kind);
    }

    private static long ToUniversal((long Local, char Kind) at, int standard, int save) =>
        at.Kind switch
        {
            'u' => at.Local,
            's' => at.Local - standard,
            _ => at.Local - standard - save,
        };

    /// <summary>Each rule's occurrence in the years given, in chronological order of local time.</summary>
    internal static List<(long Local, char Kind, int Save)> Events(IEnumerable<TzdbRule> rules, long firstYear, long lastYear)
    {
        var events = new List<(long Local, char Kind, int Save)>();

        for (var year = firstYear; year <= lastYear; year++)
        {
            foreach (var rule in rules)
            {
                if (rule.From <= year && (rule.To is null || year <= rule.To))
                {
                    events.Add(((OnDay(year, rule.Month, rule.On) * 86400) + rule.At, rule.AtKind, rule.Save));
                }
            }
        }

        // STABLE, so two rules at one local time keep the order the file gives them.
        return [.. events.Select(static (e, i) => (e, i)).OrderBy(static p => p.e.Local).ThenBy(static p => p.i).Select(static p => p.e)];
    }

    /// <summary>Compiles one zone's lines against the rule sets.</summary>
    internal static TzdbZone Compile(IReadOnlyList<TzdbZoneLine> lines, IReadOnlyDictionary<string, List<TzdbRule>> rules)
    {
        var raw = new List<(long At, int Offset, bool Daylight, bool Named)>();
        int? initial = null;
        long? start = null;
        TzdbFinalRules? final = null;
        var fixedForever = false;

        void Emit(long at, int offset, int standard, bool named)
        {
            if (initial is null)
            {
                initial = offset;
                return;
            }

            raw.Add((at, offset, offset != standard, named));
        }

        foreach (var line in lines)
        {
            var standard = line.StandardOffset;
            var last = line.Until.Length == 0;

            if (line.Rules == "-" || char.IsAsciiDigit(line.Rules[0]) || line.Rules[0] == '-')
            {
                var save = line.Rules == "-" ? 0 : Time(line.Rules).Seconds;
                Emit(start ?? long.MinValue, standard + save, standard, save > 0);

                if (last)
                {
                    fixedForever = true;
                    break;
                }

                start = ToUniversal(UntilLocal(line.Until), standard, save);
                continue;
            }

            var set = rules[line.Rules];
            var firstYear = set.Min(static rule => rule.From);
            int lastYear;
            var cut = 0;

            if (last)
            {
                var finite = set.Max(static rule => rule.To ?? rule.From);
                cut = Math.Max(finite, start is { } from ? YearOf(from) : 0) + 1;
                lastYear = cut;
            }
            else
            {
                lastYear = YearOf(UntilLocal(line.Until).Local) + 1;
            }

            var events = Events(set, firstYear, lastYear);

            // THE REARGUARD DAYLIGHT FLAG: a negative save still to come on the line is its standard
            // time, and a larger save until then is daylight time (Ireland's summer, Namibia's until
            // 2017); a save with no negative one after it is daylight only where it is positive.
            // Only the line's own events count: one after its UNTIL is another line's.
            var recurringLeast = last ? set.Where(static rule => rule.To is null).Select(static rule => rule.Save).DefaultIfEmpty(0).Min() : 0;
            var least = new int[events.Count + 1];
            least[events.Count] = Math.Min(0, recurringLeast);
            var untilLocal = last ? long.MaxValue : UntilLocal(line.Until).Local;

            for (var i = events.Count - 1; i >= 0; i--)
            {
                least[i] = events[i].Local < untilLocal ? Math.Min(least[i + 1], events[i].Save) : least[i + 1];
            }

            // THE SAVE IN FORCE AT THE LINE'S START is the latest rule's at or before it, however
            // many years before, as zic's start offset is; with none, standard time.
            var current = 0;
            var next = 0;

            if (start is { } begin)
            {
                for (; next < events.Count; next++)
                {
                    if (ToUniversal((events[next].Local, events[next].Kind), standard, current) > begin)
                    {
                        break;
                    }

                    current = events[next].Save;
                }
            }

            Emit(start ?? long.MinValue, standard + current, standard, current > least[next]);

            for (; next < events.Count; next++)
            {
                var at = ToUniversal((events[next].Local, events[next].Kind), standard, current);

                if (!last && at >= ToUniversal(UntilLocal(line.Until), standard, current))
                {
                    break;
                }

                Emit(at, standard + events[next].Save, standard, events[next].Save > least[next]);
                current = events[next].Save;
            }

            if (last)
            {
                var recurring = set.Where(static rule => rule.To is null).ToArray();

                if (recurring.Length > 0)
                {
                    final = new TzdbFinalRules(standard, cut, recurring);
                }
                else
                {
                    fixedForever = true;
                }

                break;
            }

            start = ToUniversal(UntilLocal(line.Until), standard, current);
        }

        // ZIC'S MERGE (writezone): a transition whose wall clock, in the offset before it, is not
        // after the previous transition's wall clock, in the offset before that, gives the previous
        // transition its type and is dropped; and a transition to the type already in force is no
        // transition.
        var first = initial ?? throw new InvalidDataException("a zone without lines");
        var merged = new List<(long At, int Offset, bool Daylight, bool Named)>();

        foreach (var entry in raw.Select(static (e, i) => (e, i)).OrderBy(static p => p.e.At).ThenBy(static p => p.i).Select(static p => p.e))
        {
            if (merged.Count > 0)
            {
                var before = merged[^1].Offset;
                var beforePrevious = merged.Count > 1 ? merged[^2].Offset : first;

                if (entry.At + before <= merged[^1].At + beforePrevious)
                {
                    merged[^1] = (merged[^1].At, entry.Offset, entry.Daylight, entry.Named);
                    continue;
                }
            }

            var previous = merged.Count > 0 ? (merged[^1].Offset, merged[^1].Daylight) : (first, false);

            if ((entry.Offset, entry.Daylight) != previous)
            {
                merged.Add(entry);
            }
        }

        var transitions = new List<(long At, int Offset, bool Daylight)>();

        // A CHANGE OF OFFSET OR OF THE NAMED DAYLIGHT FLAG is kept: a zone that moves to standard time
        // at its daylight offset (America/Chihuahua in 2022) is named for standard time after it.
        foreach (var (at, offset, _, named) in merged)
        {
            if ((offset, named) != (transitions.Count > 0 ? (transitions[^1].Offset, transitions[^1].Daylight) : (first, false)))
            {
                transitions.Add((at, offset, named));
            }
        }

        return new TzdbZone(first, transitions, fixedForever ? null : final);
    }

    /// <summary>
    /// A compiled zone's offset at an instant: the listed transitions, then the recurring rules, the
    /// way the profile's reader computes it; the generator's own check of what it encodes.
    /// </summary>
    internal static int OffsetAt(TzdbZone zone, long at)
    {
        if (zone.Final is { } final && YearOf(at) > final.LastListedYear)
        {
            return RecurringOffsetAt(final, at);
        }

        var low = 0;
        var high = zone.Transitions.Count;

        while (low < high)
        {
            var middle = (low + high) / 2;

            if (zone.Transitions[middle].At <= at)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low == 0 ? zone.InitialOffset : zone.Transitions[low - 1].Offset;
    }

    /// <summary>The offset the recurring rules give in a year after the listed ones.</summary>
    internal static int RecurringOffsetAt(TzdbFinalRules final, long at)
    {
        var year = YearOf(at);
        var save = Events(final.Rules, year - 2, year - 2)[^1].Save;
        var offset = final.StandardOffset + save;

        foreach (var (local, kind, next) in Events(final.Rules, year - 1, year + 1))
        {
            if (ToUniversal((local, kind), final.StandardOffset, save) > at)
            {
                break;
            }

            offset = final.StandardOffset + next;
            save = next;
        }

        return offset;
    }
}
