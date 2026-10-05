// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           6
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// What a format names its zone with (JSD-0058): the metazone data, the region whose golden zones
/// apply, and the locale's name of a region.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AEF4C0
// Broiler-Human:        PENDING
internal sealed record JsZoneNaming(JsZoneNames Names, string Region, System.Func<string, string?> RegionName);

/// <summary>
/// The names a time zone is written with (JSD-0058): CLDR's metazones, their golden zones and the
/// countries' primary zones, and the algorithms ICU's <c>TimeZoneFormat</c> and
/// <c>TimeZoneGenericNames</c> name a zone by, over each language's names in the date data.
/// </summary>
/// <remarks>
/// <para>
/// <b>A specific name</b> (<c>z</c>, <c>zzzz</c>) is the zone's own standard or daylight name, or its
/// metazone's at the instant; <b>a generic name</b> (<c>v</c>, <c>vvvv</c>) is the zone's own generic
/// name, or its metazone's - its standard name where the zone keeps no daylight time within 184 days,
/// and a partial location name where the metazone's golden zone for the locale's region keeps another
/// offset - or else the zone's country, where it is the country's only or primary zone, or exemplar
/// city, in the region format. A zone named neither way is written in CLDR's GMT format.
/// </para>
/// <para>
/// <b>A metazone's period</b> without a start begins at 1970-01-01 00:00 UTC and one without an end
/// ends at 9999-12-31 23:59 UTC, as ICU reads CLDR's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F5D500
// Broiler-Human:        PENDING
internal sealed class JsZoneNames
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E44F2
    // Broiler-Human:        PENDING
    private const long NoEnd = 253_402_300_740_000L;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EB587D
    // Broiler-Human:        PENDING
    private const long DaylightRange = 184L * 86_400;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1FE2A6
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(long From, long To, string Metazone)>> periods =
        new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A9EDF3
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, string> golden = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BD9D04
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, string> primaryZones = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=FBFC13
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, string> countries = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=26B61B
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, int> zonesInRegion = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F3037C
    // Broiler-Human:        PENDING
    private readonly JsTimeZones zones;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE0E32
    // Broiler-Human:        PENDING
    internal JsZoneNames(System.Collections.Generic.IEnumerable<string> metaZones, JsTimeZones zones)
    {
        this.zones = zones;

        foreach (var line in metaZones)
        {
            var fields = line.Split('|');

            switch (fields[0])
            {
                case "z":
                    if (!periods.TryGetValue(fields[1], out var list))
                    {
                        periods[fields[1]] = list = [];
                    }

                    list.Add((fields[2].Length == 0 ? 0 : Time(fields[2]), fields[3].Length == 0 ? NoEnd : Time(fields[3]), fields[4]));
                    break;

                case "g":
                    golden[fields[1] + "|" + fields[2]] = fields[3];
                    break;

                case "p":
                    primaryZones[fields[1]] = fields[2];
                    break;
            }
        }

        // A ZONE'S COUNTRY is zone.tab's, by primary identifier, and a country's zones are counted the
        // same way: a country with one is named by it.
        foreach (var region in zones.Regions)
        {
            var distinct = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

            foreach (var name in zones.Region(region))
            {
                if (zones.Find(name) is { } record)
                {
                    countries.TryAdd(record.Primary, region);
                    distinct.Add(record.Primary);
                }
            }

            zonesInRegion[region] = distinct.Count;
        }
    }

    /// <summary>A metazone period's bound, <c>yyyy-MM-dd HH:mm</c> UTC, in milliseconds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=019E48
    // Broiler-Human:        PENDING
    private static long Time(string text)
    {
        static int Number(string text, int at, int length) =>
            int.Parse(text.Substring(at, length), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture);

        var days = JsTemporalCore.EpochDays(Number(text, 0, 4), Number(text, 5, 2), Number(text, 8, 2));
        return ((days * 1440) + (Number(text, 11, 2) * 60) + Number(text, 14, 2)) * 60_000;
    }

    /// <summary>The metazone a zone uses at a time value, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=49ED10
    // Broiler-Human:        PENDING
    internal string? Metazone(string zone, long time)
    {
        if (periods.TryGetValue(zone, out var list))
        {
            foreach (var (from, to, metazone) in list)
            {
                if (time >= from && time < to)
                {
                    return metazone;
                }
            }
        }

        return null;
    }

    /// <summary>A metazone's golden zone in a region, or in the world's (<c>001</c>) where the region has none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=637F23
    // Broiler-Human:        PENDING
    internal string? Golden(string metazone, string region) =>
        golden.TryGetValue(metazone + "|" + region, out var zone) || golden.TryGetValue(metazone + "|001", out zone) ? zone : null;

    /// <summary>
    /// A zone's specific name at a time value, long or short, in a language's date data: its own name
    /// for standard or daylight time, or its metazone's; nothing where neither has one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ABB978
    // Broiler-Human:        PENDING
    internal string? Specific(JsDateData data, string language, string zone, JsZone offsets, long time, bool longForm)
    {
        var type = (longForm ? "l" : "s") + (offsets.DaylightAt(Seconds(time)) ? "d" : "s");
        return Specific(data, language, zone, time, type);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A5A6F
    // Broiler-Human:        PENDING
    private string? Specific(JsDateData data, string language, string zone, long time, string type) =>
        data.Value(language, "tz." + zone + "." + type) ??
        (Metazone(zone, time) is { } metazone ? data.Value(language, "mz." + metazone + "." + type) : null);

    /// <summary>
    /// A zone's generic name at a time value, long or short (ICU's TZGNCore::getDisplayName): its
    /// non-location name, or its location name; nothing where it has neither.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A076E
    // Broiler-Human:        PENDING
    internal string? Generic(JsDateData data, string language, string region, System.Func<string, string?> regionName, string zone, JsZone offsets, long time, bool longForm) =>
        NonLocation(data, language, region, regionName, zone, offsets, time, longForm) ?? Location(data, language, regionName, zone);

    /// <summary>ICU's formatGenericNonLocationName.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DCC578
    // Broiler-Human:        PENDING
    private string? NonLocation(JsDateData data, string language, string region, System.Func<string, string?> regionName, string zone, JsZone offsets, long time, bool longForm)
    {
        var width = longForm ? "l" : "s";

        if (data.Value(language, "tz." + zone + "." + width + "g") is { } own)
        {
            return own;
        }

        if (Metazone(zone, time) is not { } metazone)
        {
            return null;
        }

        var seconds = Seconds(time);
        var generic = data.Value(language, "mz." + metazone + "." + width + "g");

        // STANDARD TIME IS NAMED AS SUCH where the zone keeps no daylight time within 184 days of the
        // instant, unless the standard name is the generic one.
        if (!offsets.DaylightAt(seconds) && !DaylightNear(offsets, seconds) &&
            Specific(data, language, zone, time, width + "s") is { } standard &&
            (generic is null || !string.Equals(standard, generic, System.StringComparison.OrdinalIgnoreCase)))
        {
            return standard;
        }

        if (generic is null)
        {
            return null;
        }

        // A ZONE THAT DOES NOT KEEP ITS METAZONE'S GOLDEN ZONE'S TIME at the instant is named by its
        // location as well.
        if (Golden(metazone, region) is { } goldenZone && goldenZone != zone && zones.Find(goldenZone) is { } record)
        {
            var reference = zones.Zone(record);

            if (reference.OffsetAt(seconds) != offsets.OffsetAt(seconds) || reference.DaylightAt(seconds) != offsets.DaylightAt(seconds))
            {
                return Partial(data, language, regionName, zone, metazone, generic);
            }
        }

        return generic;
    }

    /// <summary>Whether the transition before or after an instant, within 184 days, borders daylight time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=497897
    // Broiler-Human:        PENDING
    private static bool DaylightNear(JsZone offsets, long seconds)
    {
        if (offsets.PreviousTransition(seconds + 1, daylight: true) is { } before && seconds - before < DaylightRange && offsets.DaylightAt(before - 1))
        {
            return true;
        }

        return offsets.NextTransition(seconds, seconds + DaylightRange, daylight: true) is { } after && after - seconds < DaylightRange && offsets.DaylightAt(after);
    }

    /// <summary>ICU's getPartialLocationName: the metazone's generic name and the zone's location in the fallback format.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B00F15
    // Broiler-Human:        PENDING
    private string Partial(JsDateData data, string language, System.Func<string, string?> regionName, string zone, string metazone, string generic)
    {
        string location;

        if (countries.TryGetValue(zone, out var country))
        {
            location = Golden(metazone, country) == zone ? regionName(country) ?? country : City(data, language, zone) ?? zone;
        }
        else
        {
            location = City(data, language, zone) ?? zone;
        }

        return Format(data.Value(language, "zone.fallbackFormat") ?? "{1} ({0})", location, generic);
    }

    /// <summary>ICU's getGenericLocationName: the zone's country, where it is the only or primary one, or its city.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7695EF
    // Broiler-Human:        PENDING
    private string? Location(JsDateData data, string language, System.Func<string, string?> regionName, string zone)
    {
        if (!countries.TryGetValue(zone, out var country))
        {
            return null;
        }

        var primary = zonesInRegion.TryGetValue(country, out var count) && count == 1 ||
            primaryZones.TryGetValue(country, out var first) && first == zone;
        var location = primary ? regionName(country) ?? country : City(data, language, zone);
        return location is null ? null : Format(data.Value(language, "zone.regionFormat") ?? "{0}", location, null);
    }

    /// <summary>A zone's exemplar city: the date data's, or the last segment of its identifier.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3E7F21
    // Broiler-Human:        PENDING
    private static string? City(JsDateData data, string language, string zone) =>
        data.Value(language, "tz." + zone + ".city") ??
        (zone.StartsWith("Etc/", System.StringComparison.Ordinal) || !zone.Contains('/', System.StringComparison.Ordinal)
            ? null
            : zone[(zone.LastIndexOf('/') + 1)..].Replace('_', ' '));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CA350E
    // Broiler-Human:        PENDING
    private static string Format(string pattern, string first, string? second)
    {
        var text = pattern.Replace("{0}", first, System.StringComparison.Ordinal);
        return second is null ? text : text.Replace("{1}", second, System.StringComparison.Ordinal);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E94CD8
    // Broiler-Human:        PENDING
    private static long Seconds(long time) => (long)System.Math.Floor(time / 1000.0);
}
