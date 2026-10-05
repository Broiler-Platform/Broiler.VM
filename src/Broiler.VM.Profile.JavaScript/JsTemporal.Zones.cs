// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           0
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9FF4AD
// Broiler-Human:        PENDING
internal static partial class JsTemporal
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=97E0CF
    // Broiler-Human:        PENDING
    private static readonly BigInteger NsPerSecond = new(1_000_000_000L);

    /// <summary>
    /// The proposal's ParseTimeZoneIdentifier (s11.1.16) of an identifier already known to parse: the
    /// IANA name, or the offset in minutes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=88EF1F
    // Broiler-Human:        PENDING
    internal static (string? Name, long OffsetMinutes) ParseZone(string identifier) =>
        identifier.Length > 0 && identifier[0] is '+' or '-'
            ? (null, JsTemporalParser.OffsetNanoseconds(identifier) / 60_000_000_000L)
            : (identifier, 0);

    /// <summary>The proposal's ToTemporalTimeZoneIdentifier (s11.1.8).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BEC91B
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
            return JsTemporalCore.FormatOffsetMinutes(offset);
        }

        return NamedZone(engine, name!).Identifier;
    }

    /// <summary>
    /// The time zone the ZonedDateTime constructor accepts (s6.1.1): ParseTimeZoneIdentifier of the
    /// string, then the available identifier or the normalized offset.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E3AFAA
    // Broiler-Human:        PENDING
    internal static string TimeZoneFromIdentifier(JsEngine engine, string text)
    {
        if (!JsTemporalParser.IsTimeZoneIdentifier(text))
        {
            throw engine.Error("RangeError", "Temporal: " + text + " is not a time zone identifier");
        }

        var (name, minutes) = ParseZone(text);
        return name is null ? JsTemporalCore.FormatOffsetMinutes(minutes) : NamedZone(engine, name).Identifier;
    }

    /// <summary>GetAvailableNamedTimeZoneIdentifier (s11.1.1), or the RangeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EC1B8A
    // Broiler-Human:        PENDING
    private static JsTimeZoneId NamedZone(JsEngine engine, string name) =>
        engine.Intl?.TimeZones.Find(name) ??
        (string.Equals(name, "UTC", System.StringComparison.OrdinalIgnoreCase) ? new JsTimeZoneId("UTC", "UTC", -1) : null) ??
        throw engine.Error("RangeError", "Temporal: " + name + " is not an available time zone");

    /// <summary>The offset, in seconds, of a named zone at an instant.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C126E9
    // Broiler-Human:        PENDING
    private static long NamedOffsetSeconds(JsEngine engine, string name, BigInteger epochNs)
    {
        var record = NamedZone(engine, name);

        if (record.Zone < 0 || engine.Intl is null)
        {
            return 0;
        }

        var seconds = BigInteger.Divide(epochNs, NsPerSecond);

        if (epochNs.Sign < 0 && !(seconds * NsPerSecond).Equals(epochNs))
        {
            seconds -= 1;
        }

        return engine.Intl.TimeZones.Zone(record).OffsetAt((long)seconds);
    }

    /// <summary>The proposal's GetOffsetNanosecondsFor (s11.1.9).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=2FE396
    // Broiler-Human:        PENDING
    internal static long OffsetNanosecondsFor(JsEngine engine, string timeZone, BigInteger epochNs)
    {
        var (name, minutes) = ParseZone(timeZone);
        return name is null ? minutes * 60_000_000_000L : NamedOffsetSeconds(engine, name, epochNs) * 1_000_000_000L;
    }

    /// <summary>The proposal's GetISODateTimeFor (s11.1.10).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0D19DB
    // Broiler-Human:        PENDING
    internal static JsIsoDateTime IsoDateTimeFor(JsEngine engine, string timeZone, BigInteger epochNs) =>
        JsTemporalCore.FromEpochNs(epochNs + OffsetNanosecondsFor(engine, timeZone, epochNs));

    /// <summary>The proposal's GetPossibleEpochNanoseconds (s11.1.13), ascending.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8CEEBA
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.List<BigInteger> PossibleEpochNs(JsEngine engine, string timeZone, JsIsoDateTime dateTime)
    {
        var (name, minutes) = ParseZone(timeZone);
        var possible = new System.Collections.Generic.List<BigInteger>(2);

        if (name is null)
        {
            var balanced = JsTemporalCore.BalanceDateTime(dateTime.Date, JsTemporalCore.TimeNs(dateTime.Time) - (minutes * 60_000_000_000L));

            if (!JsTemporalCore.DaysWithinRange(balanced.Date))
            {
                throw engine.Error("RangeError", "Temporal: the date is outside the representable range");
            }

            possible.Add(JsTemporalCore.UtcEpochNs(balanced));
        }
        else
        {
            // GETNAMEDTIMEZONEEPOCHNANOSECONDS: each offset in force within a day either side, kept
            // where the instant it implies does have that offset. A named zone's branch checks no day
            // range: a wall time a day past the limit can still name a valid instant, west of UTC.
            var utc = JsTemporalCore.UtcEpochNs(dateTime);
            var offsets = new System.Collections.Generic.SortedSet<long>
            {
                NamedOffsetSeconds(engine, name, utc - JsTemporalCore.NsPerDay),
                NamedOffsetSeconds(engine, name, utc),
                NamedOffsetSeconds(engine, name, utc + JsTemporalCore.NsPerDay),
            };

            foreach (var offset in offsets)
            {
                var candidate = utc - (offset * NsPerSecond);

                if (NamedOffsetSeconds(engine, name, candidate) == offset)
                {
                    possible.Add(candidate);
                }
            }

            possible.Sort();
        }

        foreach (var candidate in possible)
        {
            if (!JsTemporalCore.IsValidEpochNs(candidate))
            {
                throw engine.Error("RangeError", "Temporal: the date-time is outside the representable range");
            }
        }

        return possible;
    }

    /// <summary>The proposal's DisambiguatePossibleEpochNanoseconds (s11.1.12).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=59BBB9
    // Broiler-Human:        PENDING
    internal static BigInteger Disambiguate(
        JsEngine engine, System.Collections.Generic.List<BigInteger> possible, string timeZone, JsIsoDateTime dateTime, string disambiguation)
    {
        if (possible.Count == 1)
        {
            return possible[0];
        }

        if (possible.Count != 0)
        {
            return disambiguation switch
            {
                "earlier" or "compatible" => possible[0],
                "later" => possible[^1],
                _ => throw engine.Error("RangeError", "Temporal: the local time is ambiguous in " + timeZone),
            };
        }

        if (disambiguation == "reject")
        {
            throw engine.Error("RangeError", "Temporal: the local time does not exist in " + timeZone);
        }

        // A GAP: the offsets before and after it, a day either side.
        var utc = JsTemporalCore.UtcEpochNs(dateTime);
        var before = OffsetNanosecondsFor(engine, timeZone, utc - JsTemporalCore.NsPerDay);
        var after = OffsetNanosecondsFor(engine, timeZone, utc + JsTemporalCore.NsPerDay);
        var nanoseconds = after - before;

        if (disambiguation == "earlier")
        {
            var earlier = JsTemporalCore.BalanceDateTime(dateTime.Date, JsTemporalCore.TimeNs(dateTime.Time) - nanoseconds);
            return PossibleEpochNs(engine, timeZone, earlier)[0];
        }

        var later = JsTemporalCore.BalanceDateTime(dateTime.Date, JsTemporalCore.TimeNs(dateTime.Time) + nanoseconds);
        return PossibleEpochNs(engine, timeZone, later)[^1];
    }

    /// <summary>The proposal's GetEpochNanosecondsFor (s11.1.11).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C39F64
    // Broiler-Human:        PENDING
    internal static BigInteger EpochNsFor(JsEngine engine, string timeZone, JsIsoDateTime dateTime, string disambiguation) =>
        Disambiguate(engine, PossibleEpochNs(engine, timeZone, dateTime), timeZone, dateTime, disambiguation);

    /// <summary>The proposal's GetStartOfDay (s11.1.14).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37756F
    // Broiler-Human:        PENDING
    internal static BigInteger StartOfDay(JsEngine engine, string timeZone, JsIsoDate date)
    {
        var midnight = new JsIsoDateTime(date, JsTimeRecord.Midnight);
        var possible = PossibleEpochNs(engine, timeZone, midnight);

        if (possible.Count != 0)
        {
            return possible[0];
        }

        // MIDNIGHT IS IN A GAP: the day starts at the transition, the first instant after it.
        var utc = JsTemporalCore.UtcEpochNs(midnight);
        var transition = NextTransition(engine, timeZone, utc - JsTemporalCore.NsPerDay) ??
            throw engine.Error("RangeError", "Temporal: the start of the day cannot be found in " + timeZone);
        return transition;
    }

    /// <summary>The proposal's TimeZoneEquals (s11.1.15).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ABD481
    // Broiler-Human:        PENDING
    internal static bool TimeZoneEquals(JsEngine engine, string one, string two)
    {
        if (one == two)
        {
            return true;
        }

        var (nameOne, _) = ParseZone(one);
        var (nameTwo, _) = ParseZone(two);

        if (nameOne is not null && nameTwo is not null)
        {
            return NamedZone(engine, nameOne).Primary == NamedZone(engine, nameTwo).Primary;
        }

        return false;
    }

    /// <summary>The proposal's GetNamedTimeZoneNextTransition (s11.1.3), or null for an offset zone or none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AC9E6A
    // Broiler-Human:        PENDING
    internal static BigInteger? NextTransition(JsEngine engine, string timeZone, BigInteger epochNs)
    {
        var (name, _) = ParseZone(timeZone);

        if (name is null || engine.Intl is null)
        {
            return null;
        }

        var record = NamedZone(engine, name);

        if (record.Zone < 0)
        {
            return null;
        }

        // TRANSITIONS FALL ON WHOLE SECONDS: the next after the instant is the next after its second.
        var seconds = FloorSeconds(epochNs);
        var limitSeconds = (long)(JsTemporalCore.NsMaxInstant / NsPerSecond);
        var next = engine.Intl.TimeZones.Zone(record).NextTransition(seconds, limitSeconds);

        if (next is not { } found)
        {
            return null;
        }

        var ns = found * NsPerSecond;
        return ns <= JsTemporalCore.NsMaxInstant ? ns : null;
    }

    /// <summary>The proposal's GetNamedTimeZonePreviousTransition (s11.1.4), or null.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CDACC7
    // Broiler-Human:        PENDING
    internal static BigInteger? PreviousTransition(JsEngine engine, string timeZone, BigInteger epochNs)
    {
        var (name, _) = ParseZone(timeZone);

        if (name is null || engine.Intl is null)
        {
            return null;
        }

        var record = NamedZone(engine, name);

        if (record.Zone < 0)
        {
            return null;
        }

        // STRICTLY BEFORE: an instant within a second after a transition sees that transition.
        var seconds = FloorSeconds(epochNs);
        var exact = (seconds * NsPerSecond) == epochNs;
        var previous = engine.Intl.TimeZones.Zone(record).PreviousTransition(exact ? seconds : seconds + 1);

        if (previous is not { } found)
        {
            return null;
        }

        var ns = found * NsPerSecond;
        return ns >= JsTemporalCore.NsMinInstant ? ns : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CBBF6D
    // Broiler-Human:        PENDING
    private static long FloorSeconds(BigInteger epochNs)
    {
        var seconds = BigInteger.Divide(epochNs, NsPerSecond);

        if (epochNs.Sign < 0 && !(seconds * NsPerSecond).Equals(epochNs))
        {
            seconds -= 1;
        }

        return (long)seconds;
    }
}
