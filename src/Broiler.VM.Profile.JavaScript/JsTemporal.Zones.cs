// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           0
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Time;
using BigInteger = System.Numerics.BigInteger;
using ZoneDisambiguation = Broiler.ECMAScript.Time.Disambiguation;

namespace Broiler.VM.Profile.JavaScript;

// THE TIME-ZONE OPERATIONS ARE THE SHARED LIBRARY'S since SJL-8.5 (JSD-0076, JSD-0077): ZonedTime
// computes the offsets, the possible instants, their disambiguation, the start of a day, the
// transitions and the equality of two zones, over the rules of a named zone the Intl data's
// TimeZoneDatabase answers or of an offset zone FixedOffsetTimeZoneRules answers, and OffsetTimeZones
// reads and writes the offset identifiers. What stays here is the engine's: the conversion of the
// arguments, the parsing of ISO strings and their annotations, a realm without Intl data knowing UTC
// alone, and each status mapped to the RangeError this family threw before, with the same message.

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    /// <summary>UTC's rules: the one named zone a realm without Intl data reads (JSD-0053), and every offset zone of zero.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3765AE
    // Broiler-Human:        PENDING
    private static readonly FixedOffsetTimeZoneRules UtcRules = new(0);

    /// <summary>
    /// The proposal's ParseTimeZoneIdentifier (s11.1.16) of an identifier already known to parse: the
    /// IANA name, or the offset in minutes, which the shared library's offset branch reads.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=61A8B5
    // Broiler-Human:        PENDING
    internal static (string? Name, long OffsetMinutes) ParseZone(string identifier) =>
        OffsetTimeZones.TryParseOffsetIdentifier(identifier, out var minutes) ? (null, minutes) : (identifier, 0);

    /// <summary>The proposal's ToTemporalTimeZoneIdentifier (s11.1.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=08291D
    // Broiler-Human:        PENDING
    internal static string ToTimeZoneIdentifier(JsEngine engine, JsValue value)
    {
        if (value.AsObjectOrNull() is JsZonedDateTimeObject zoned)
        {
            return zoned.TimeZone;
        }

        if (!value.IsString)
        {
            throw engine.Error("TypeError", "Temporal: a time zone must be a string or a Temporal.ZonedDateTime");
        }

        var text = value.AsString();
        string? name;
        long? offsetMinutes = null;

        // PARSETEMPORALTIMEZONESTRING (s13.38): an identifier, or the zone of an ISO string.
        if (JsTemporalParser.IsTimeZoneIdentifier(text))
        {
            (name, var minutes) = ParseZone(text);
            offsetMinutes = name is null ? minutes : null;
        }
        else
        {
            var parsed = JsTemporalParser.Parse(
                engine,
                text,
                TemporalGoal.ZonedDateTime,
                TemporalGoal.DateTime,
                TemporalGoal.Instant,
                TemporalGoal.Time,
                TemporalGoal.MonthDay,
                TemporalGoal.YearMonth);

            if (parsed.TimeZone is { } annotation)
            {
                (name, var minutes) = ParseZone(annotation);
                offsetMinutes = name is null ? minutes : null;
            }
            else if (parsed.Z)
            {
                name = "UTC";
            }
            else if (parsed.Offset is { } offsetText)
            {
                if (!JsTemporalParser.IsUtcOffset(offsetText, subMinute: false))
                {
                    throw engine.Error("RangeError", "Temporal: a time zone offset cannot have seconds: " + offsetText);
                }

                name = null;
                offsetMinutes = JsTemporalParser.OffsetNanoseconds(offsetText) / 60_000_000_000L;
            }
            else
            {
                throw engine.Error("RangeError", "Temporal: " + text + " names no time zone");
            }
        }

        if (offsetMinutes is { } offset)
        {
            return OffsetTimeZones.FormatOffsetIdentifier((int)offset);
        }

        return NamedZone(engine, name!);
    }

    /// <summary>
    /// The time zone the ZonedDateTime constructor accepts (s6.1.1): ParseTimeZoneIdentifier of the
    /// string, then the available identifier or the normalized offset.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=837E22
    // Broiler-Human:        PENDING
    internal static string TimeZoneFromIdentifier(JsEngine engine, string text)
    {
        if (!JsTemporalParser.IsTimeZoneIdentifier(text))
        {
            throw engine.Error("RangeError", "Temporal: " + text + " is not a time zone identifier");
        }

        var (name, minutes) = ParseZone(text);
        return name is null ? OffsetTimeZones.FormatOffsetIdentifier((int)minutes) : NamedZone(engine, name);
    }

    /// <summary>
    /// GetAvailableNamedTimeZoneIdentifier (s11.1.1), or the RangeError: the identifier in the database's
    /// case, which the shared database's lookup finds under the policy's case rule; <c>UTC</c> alone where
    /// the realm carries no Intl data.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AB9C0C
    // Broiler-Human:        PENDING
    private static string NamedZone(JsEngine engine, string name)
    {
        if (engine.Intl is { } intl && intl.TimeZones.TryFind(name, out var id))
        {
            return id.Identifier;
        }

        return string.Equals(name, "UTC", System.StringComparison.OrdinalIgnoreCase)
            ? "UTC"
            : throw engine.Error("RangeError", "Temporal: " + name + " is not an available time zone");
    }

    /// <summary>
    /// The rules of a zone: an offset zone's fixed offset, a named zone's from the shared database, which
    /// reads its primary identifier's zone, or UTC's in a realm without Intl data; the RangeError for a
    /// name that is not available.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2B4DCD
    // Broiler-Human:        PENDING
    private static ITimeZoneRules RulesOf(JsEngine engine, string timeZone)
    {
        var (name, minutes) = ParseZone(timeZone);

        if (name is null)
        {
            return minutes == 0 ? UtcRules : new FixedOffsetTimeZoneRules((int)minutes * 60);
        }

        if (engine.Intl is { } intl && intl.TimeZones.TryFind(name, out var id))
        {
            return intl.TimeZones.Rules(id);
        }

        return string.Equals(name, "UTC", System.StringComparison.OrdinalIgnoreCase)
            ? UtcRules
            : throw engine.Error("RangeError", "Temporal: " + name + " is not an available time zone");
    }

    /// <summary>
    /// The RangeError a status other than completion is, with the message this family has always given
    /// it (JSD-0077): an instant or a wall time outside the range, a time a transition repeats or skips
    /// refused under <c>reject</c>, and a day with no first instant.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7CFE76
    // Broiler-Human:        PENDING
    private static JsThrow ZoneError(JsEngine engine, TimeStatus status, string timeZone) => status switch
    {
        TimeStatus.Ambiguous => engine.Error("RangeError", "Temporal: the local time is ambiguous in " + timeZone),
        TimeStatus.Skipped => engine.Error("RangeError", "Temporal: the local time does not exist in " + timeZone),
        TimeStatus.NoStartOfDay => engine.Error("RangeError", "Temporal: the start of the day cannot be found in " + timeZone),
        _ => engine.Error("RangeError", "Temporal: the date-time is outside the representable range"),
    };

    /// <summary>
    /// An offset zone's CheckISODaysRange (s11.1.13 step 1), the RangeError naming the date that this
    /// family gives before the shared operation's, which reports the same wall time as an instant out of
    /// range.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AD5082
    // Broiler-Human:        PENDING
    private static void CheckOffsetZoneDays(JsEngine engine, string timeZone, JsIsoDateTime dateTime)
    {
        var (name, minutes) = ParseZone(timeZone);

        if (name is null &&
            !JsTemporalCore.DaysWithinRange(JsTemporalCore.BalanceDateTime(dateTime.Date, JsTemporalCore.TimeNs(dateTime.Time) - (minutes * 60_000_000_000L)).Date))
        {
            throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
        }
    }

    /// <summary>A date-time as the shared library's record; the time's overflow days are not part of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B66B6D
    // Broiler-Human:        PENDING
    private static IsoDateTime IsoOf(JsIsoDateTime dateTime) =>
        new(
            new IsoDate(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day),
            new IsoTime(dateTime.Time.Hour, dateTime.Time.Minute, dateTime.Time.Second, dateTime.Time.Millisecond, dateTime.Time.Microsecond, dateTime.Time.Nanosecond));

    /// <summary>The disambiguation option's value as the shared library's mode; <c>reject</c> for any other.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F10C6C
    // Broiler-Human:        PENDING
    private static ZoneDisambiguation ModeOf(string disambiguation) => disambiguation switch
    {
        "compatible" => ZoneDisambiguation.Compatible,
        "earlier" => ZoneDisambiguation.Earlier,
        "later" => ZoneDisambiguation.Later,
        _ => ZoneDisambiguation.Reject,
    };

    /// <summary>The proposal's GetOffsetNanosecondsFor (s11.1.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3F2F8
    // Broiler-Human:        PENDING
    internal static long OffsetNanosecondsFor(JsEngine engine, string timeZone, BigInteger epochNs) =>
        ZonedTime.GetOffsetNanosecondsFor(RulesOf(engine, timeZone), epochNs);

    /// <summary>The proposal's GetISODateTimeFor (s11.1.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0D19DB
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime IsoDateTimeFor(JsEngine engine, string timeZone, BigInteger epochNs) =>
        JsTemporalCore.FromEpochNs(epochNs + OffsetNanosecondsFor(engine, timeZone, epochNs));

    /// <summary>The proposal's GetPossibleEpochNanoseconds (s11.1.13), ascending.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD908E
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<BigInteger> PossibleEpochNs(JsEngine engine, string timeZone, JsIsoDateTime dateTime)
    {
        var rules = RulesOf(engine, timeZone);
        CheckOffsetZoneDays(engine, timeZone, dateTime);

        System.Span<BigInteger> found = new BigInteger[ZonedTime.MaxPossibleEpochNanoseconds];
        var status = ZonedTime.GetPossibleEpochNanoseconds(rules, IsoOf(dateTime), found, out var count);

        if (status != TimeStatus.Completed)
        {
            throw ZoneError(engine, status, timeZone);
        }

        return [.. found[..count]];
    }

    /// <summary>The proposal's DisambiguatePossibleEpochNanoseconds (s11.1.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BBAD44
    // Broiler-Human:        PENDING
    internal static BigInteger Disambiguate(
        JsEngine engine, System.Collections.Generic.List<BigInteger> possible, string timeZone, JsIsoDateTime dateTime, string disambiguation)
    {
        var status = ZonedTime.DisambiguatePossibleEpochNanoseconds(
            RulesOf(engine, timeZone), System.Runtime.InteropServices.CollectionsMarshal.AsSpan(possible), IsoOf(dateTime), ModeOf(disambiguation), out var epochNs);
        return status == TimeStatus.Completed ? epochNs : throw ZoneError(engine, status, timeZone);
    }

    /// <summary>The proposal's GetEpochNanosecondsFor (s11.1.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=798290
    // Broiler-Human:        PENDING
    internal static BigInteger EpochNsFor(JsEngine engine, string timeZone, JsIsoDateTime dateTime, string disambiguation)
    {
        var rules = RulesOf(engine, timeZone);
        CheckOffsetZoneDays(engine, timeZone, dateTime);

        var status = ZonedTime.GetEpochNanosecondsFor(rules, IsoOf(dateTime), ModeOf(disambiguation), out var epochNs);
        return status == TimeStatus.Completed ? epochNs : throw ZoneError(engine, status, timeZone);
    }

    /// <summary>The proposal's GetStartOfDay (s11.1.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7B912C
    // Broiler-Human:        PENDING
    internal static BigInteger StartOfDay(JsEngine engine, string timeZone, JsIsoDate date)
    {
        var rules = RulesOf(engine, timeZone);
        CheckOffsetZoneDays(engine, timeZone, new JsIsoDateTime(date, JsTimeRecord.Midnight));

        var status = ZonedTime.GetStartOfDay(rules, new IsoDate(date.Year, date.Month, date.Day), out var epochNs);
        return status == TimeStatus.Completed ? epochNs : throw ZoneError(engine, status, timeZone);
    }

    /// <summary>
    /// The proposal's TimeZoneEquals (s11.1.15), over the shared database; a realm without Intl data
    /// stores no named zone but <c>UTC</c>, so its zones are equal as strings.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=609F53
    // Broiler-Human:        PENDING
    internal static bool TimeZoneEquals(JsEngine engine, string one, string two) =>
        engine.Intl is { } intl
            ? ZonedTime.TimeZoneEquals(intl.TimeZones, one, two)
            : string.Equals(one, two, System.StringComparison.Ordinal);

    /// <summary>The proposal's GetNamedTimeZoneNextTransition (s11.1.3), or null for an offset zone or none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=781C93
    // Broiler-Human:        PENDING
    internal static BigInteger? NextTransition(JsEngine engine, string timeZone, BigInteger epochNs) =>
        ZonedTime.GetNextTransition(RulesOf(engine, timeZone), epochNs);

    /// <summary>The proposal's GetNamedTimeZonePreviousTransition (s11.1.4), or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0D91D3
    // Broiler-Human:        PENDING
    internal static BigInteger? PreviousTransition(JsEngine engine, string timeZone, BigInteger epochNs) =>
        ZonedTime.GetPreviousTransition(RulesOf(engine, timeZone), epochNs);
}
