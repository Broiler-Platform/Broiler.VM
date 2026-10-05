using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Writes the time zone tables of <c>Broiler.VM.Profile.JavaScript.Intl</c> from the tzdb members
/// <c>tzdb.pin</c> names and CLDR's time zone keys, which <c>cldr.pin</c> names (decision JSD-0053).
/// </summary>
/// <remarks>
/// <para>
/// <b>One function from verified bytes to one file's text</b>, as <see cref="CldrTableGenerator"/>
/// is: rule N30 calls it to compare the checked-in file and, in write mode, to write it.
/// </para>
/// <para>
/// <b>Three tables.</b> <c>TimeZones</c> is binary: each Zone's offsets compiled by
/// <see cref="TzdbCompiler"/>, in the layout <c>JsTimeZones</c> in the profile states.
/// <c>TimeZoneIds</c> names every Zone and Link: the zone whose data it reads, and its primary
/// identifier where that is another (ECMA-402 6.5.1). <c>TimeZoneRegions</c> is <c>zone.tab</c>'s
/// zones of each country, for <c>Intl.Locale.prototype.getTimeZones</c>.
/// </para>
/// <para>
/// <b>The primary identifiers are CLDR's</b>, as ECMA-402 6.5's note recommends: the identifiers CLDR
/// gives one time zone key are one zone, whose primary identifier is the key's <c>_iana</c> name, or
/// its first alias where it states none. An identifier CLDR does not name resolves through tzdb's
/// Links. <c>Etc/UTC</c>, <c>Etc/GMT</c> and <c>GMT</c>, and every identifier resolving to them, resolve
/// to <c>UTC</c>.
/// </para>
/// </remarks>
internal static class TzdbTableGenerator
{
    /// <summary>The file this writes, relative to the repository root.</summary>
    internal const string OutputPath = "src/Broiler.VM.Profile.JavaScript.Intl/JsTzdbTables.g.cs";

    /// <summary>The environment variable that turns rule N30's gate into the generator's write.</summary>
    internal const string WriteVariable = "BROILER_TZDB_WRITE";

    /// <summary>The per-member exemption every generated table states.</summary>
    internal const string ExemptReason =
        "tzdb 2026e table data written by TzdbTableGenerator from the members tzdb.pin names, compared byte for byte by rule N30";

    private const string TimeZoneKeys = "cldr-48.2.0/json/cldr-bcp47/bcp47/timezone.json";

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The generated file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact Current => current.Value;

    private static readonly Lazy<UnicodeArtefact> current = new(static () =>
    {
        var tzdb = TzdbPin.Load();
        var cldr = CldrPin.Load();
        return Artefact(Generate(tzdb, tzdb.ReadArchive(), cldr, cldr.ReadArchive()));
    });

    /// <summary>The compiled zones of the pinned release, by Zone name, in ordinal order.</summary>
    internal static IReadOnlyList<(string Name, TzdbZone Zone)> Zones(TzdbCompiler.Source source) =>
        [.. source.Zones.Keys.Order(StringComparer.Ordinal).Select(name => (name, TzdbCompiler.Compile(source.Zones[name], source.Rules)))];

    /// <summary>The generated file's text, from the two verified archives.</summary>
    internal static string Generate(
        TzdbPin pin,
        IReadOnlyDictionary<string, byte[]> tzdb,
        CldrPin cldrPin,
        IReadOnlyDictionary<string, byte[]> cldr)
    {
        pin.Verified(tzdb);
        cldrPin.Verified(cldr);

        var source = TzdbCompiler.Parse(tzdb);
        var zones = Zones(source);

        var text = new StringBuilder();
        text.Append(Header).Append('\n');

        ByteTable(text, "TimeZones", "Each Zone's UTC offsets, in Zone name order: its first offset, its transitions, and the rules that continue them.", Encode(zones));
        TextTable(text, "TimeZoneIds", "Every Zone and Link name, in ordinal order: identifier, the index of the Zone whose offsets it reads, and its primary identifier where that is another.", Identifiers(source, zones, cldr));
        TextTable(text, "TimeZoneRegions", "zone.tab's zones of each country, in ordinal order: region, zones.", Regions(tzdb));

        while (text.Length > 2 && text[^1] == '\n' && text[^2] == '\n')
        {
            text.Length--;
        }

        text.Append("}\n");
        return text.ToString();
    }

    /// <summary>
    /// The binary zone table: a count of zones, then each zone as a zig-zag first offset, its palette
    /// of offsets, each twice the offset and one more where it is daylight time, its transitions as a
    /// time delta and a palette index, and its recurring rules.
    /// </summary>
    /// <remarks>
    /// A time delta is in minutes where it is a whole number of them, its low bit clear, and in
    /// seconds otherwise, its low bit set; the first transition's delta is from the epoch. Each
    /// recurring rule is its month, the kind of its ON field (a day, the last weekday, a weekday on or
    /// after, a weekday on or before), the weekday, the day, its AT time and kind (wall, standard,
    /// universal) and its save.
    /// </remarks>
    internal static byte[] Encode(IReadOnlyList<(string Name, TzdbZone Zone)> zones)
    {
        var bytes = new List<byte>();
        Unsigned(bytes, (ulong)zones.Count);

        foreach (var (_, zone) in zones)
        {
            Signed(bytes, zone.InitialOffset);

            var palette = zone.Transitions.Select(static transition => (transition.Offset, transition.Daylight)).Distinct().ToList();
            Unsigned(bytes, (ulong)palette.Count);

            foreach (var (offset, daylight) in palette)
            {
                Signed(bytes, (offset * 2L) + (daylight ? 1 : 0));
            }

            Unsigned(bytes, (ulong)zone.Transitions.Count);
            var previous = 0L;

            foreach (var (at, offset, daylight) in zone.Transitions)
            {
                var delta = at - previous;
                previous = at;
                Unsigned(bytes, delta % 60 == 0 ? ZigZag(delta / 60) << 1 : (ZigZag(delta) << 1) | 1);
                bytes.Add(checked((byte)palette.IndexOf((offset, daylight))));
            }

            if (zone.Final is not { } final)
            {
                bytes.Add(0);
                continue;
            }

            bytes.Add(checked((byte)final.Rules.Count));
            Signed(bytes, final.StandardOffset);
            Signed(bytes, final.LastListedYear);

            foreach (var rule in final.Rules)
            {
                var (kind, weekday, day) = OnField(rule.On);
                bytes.Add((byte)rule.Month);
                bytes.Add((byte)kind);
                bytes.Add((byte)weekday);
                bytes.Add((byte)day);
                Signed(bytes, rule.At);
                bytes.Add(rule.AtKind switch { 'w' => 0, 's' => 1, _ => 2 });
                Signed(bytes, rule.Save);
            }
        }

        return [.. bytes];
    }

    /// <summary>An ON field as a kind (0 a day, 1 the last weekday, 2 on or after, 3 on or before), a weekday and a day.</summary>
    internal static (int Kind, int Weekday, int Day) OnField(string on)
    {
        if (on.StartsWith("last", StringComparison.Ordinal))
        {
            return (1, TzdbCompiler.Weekday(on[4..]), 0);
        }

        foreach (var (token, kind) in new[] { (">=", 2), ("<=", 3) })
        {
            var at = on.IndexOf(token, StringComparison.Ordinal);

            if (at > 0)
            {
                return (kind, TzdbCompiler.Weekday(on[..at]), int.Parse(on[(at + 2)..], CultureInfo.InvariantCulture));
            }
        }

        return (0, 0, int.Parse(on, CultureInfo.InvariantCulture));
    }

    private static ulong ZigZag(long value) => (ulong)((value << 1) ^ (value >> 63));

    private static void Signed(List<byte> bytes, long value) => Unsigned(bytes, ZigZag(value));

    private static void Unsigned(List<byte> bytes, ulong value)
    {
        while (value >= 0x80)
        {
            bytes.Add((byte)(value | 0x80));
            value >>= 7;
        }

        bytes.Add((byte)value);
    }

    /// <summary>Every identifier, with the Zone it reads and its primary identifier where that is another.</summary>
    internal static IEnumerable<string> Identifiers(
        TzdbCompiler.Source source,
        IReadOnlyList<(string Name, TzdbZone Zone)> zones,
        IReadOnlyDictionary<string, byte[]> cldr)
    {
        var index = zones.Select(static (zone, i) => (zone.Name, i)).ToDictionary(static p => p.Name, static p => p.i, StringComparer.Ordinal);
        var identifiers = source.Zones.Keys.Concat(source.Links.Keys).Order(StringComparer.Ordinal).ToArray();
        var known = identifiers.ToHashSet(StringComparer.Ordinal);

        if (identifiers.GroupBy(static id => id, StringComparer.OrdinalIgnoreCase).Any(static group => group.Count() > 1))
        {
            throw new InvalidDataException("two tzdb identifiers differ only in case, which ECMA-402 6.5.1 asserts never happens");
        }

        var primaries = new Dictionary<string, string>(StringComparer.Ordinal);

        using (var document = JsonDocument.Parse(cldr[TimeZoneKeys]))
        {
            foreach (var key in document.RootElement.GetProperty("keyword").GetProperty("u").GetProperty("tz").EnumerateObject())
            {
                if (key.Value.ValueKind != JsonValueKind.Object || !key.Value.TryGetProperty("_alias", out var alias))
                {
                    continue;
                }

                var names = alias.GetString()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var primary = key.Value.TryGetProperty("_iana", out var iana) ? iana.GetString()! : names[0];

                foreach (var name in names)
                {
                    primaries[name] = primary;
                }
            }
        }

        string Resolve(string name)
        {
            while (source.Links.TryGetValue(name, out var target))
            {
                name = target;
            }

            return name;
        }

        foreach (var identifier in identifiers)
        {
            var primary = primaries.TryGetValue(identifier, out var named) && known.Contains(named) ? named : Resolve(identifier);

            if (primary is "Etc/UTC" or "Etc/GMT" or "GMT")
            {
                primary = "UTC";
            }

            var zone = index[Resolve(identifier)];
            yield return identifier + "|" + zone.ToString(CultureInfo.InvariantCulture) + (primary == identifier ? "|" : "|" + primary);
        }
    }

    /// <summary>zone.tab's zones of each country code.</summary>
    internal static IEnumerable<string> Regions(IReadOnlyDictionary<string, byte[]> tzdb) =>
        TzdbPin.Decode(tzdb["zone.tab"]).Split('\n')
            .Where(static line => line.Length > 0 && line[0] != '#')
            .Select(static line => line.Split('\t'))
            .GroupBy(static fields => fields[0], StringComparer.Ordinal)
            .OrderBy(static group => group.Key, StringComparer.Ordinal)
            .Select(static group => group.Key + "|" + string.Join('|', group.Select(static fields => fields[2]).Order(StringComparer.Ordinal)));

    /// <summary>Generated text passed through the function the assurance gate uses, beside the file on disk.</summary>
    internal static UnicodeArtefact Artefact(string raw)
    {
        var full = Path.Combine(ComponentGraph.Root, OutputPath.Replace('/', Path.DirectorySeparatorChar));
        var assembly = OutputPath.Split('/')[1];
        var file = new AssuranceSourceFile(full, OutputPath, assembly, raw, "\n", AssuranceSources.Parse(raw, full));

        return new UnicodeArtefact(
            OutputPath,
            File.Exists(full) ? File.ReadAllText(full) : string.Empty,
            AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file)));
    }

    private const string Header =
        """
        using System;

        namespace Broiler.VM.Profile.JavaScript.Intl;

        // Written by TzdbTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the IANA Time
        // Zone Database 2026e members that src/tests/tzdb/pins/tzdb.pin names and CLDR 48.2.0's time zone
        // keys, under decision JSD-0053. Rule N30 regenerates this file and compares it byte for byte, so
        // a change belongs in the generator or a pinned archive and never here: run the architecture
        // tests with BROILER_TZDB_WRITE=1.
        //
        // The offsets are derived from the IANA Time Zone Database, which is in the public domain, and
        // the primary identifiers from CLDR data under the Unicode License v3 (SPDX Unicode-3.0), whose
        // text THIRD_PARTY_NOTICES.md carries.

        /// <summary>The tzdb 2026e tables, as the generator wrote them.</summary>
        // Broiler-AI:           Origin=Derived; Spec=JSD-0053; IP=Low; Security=Low; Resources=1; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal static class JsTzdbTables
        {
            /// <summary>The tzdb release the tables were generated from.</summary>
            // Broiler-AI:           EXEMPT=tzdb 2026e table data written by TzdbTableGenerator from the members tzdb.pin names, compared byte for byte by rule N30
            // Broiler-Human:        PENDING
            internal const string Version = "2026e";

        """;

    private static void TextTable(StringBuilder text, string name, string summary, IEnumerable<string> lines)
    {
        text.Append("    /// <summary>").Append(summary).Append("</summary>\n");
        text.Append("    // Broiler-AI:           EXEMPT=").Append(ExemptReason).Append('\n');
        text.Append("    // Broiler-Human:        PENDING\n");
        text.Append("    internal static ReadOnlySpan<byte> ").Append(name).Append(" =>\n");
        text.Append("        \"\"\"\n");

        foreach (var line in lines)
        {
            if (line.Contains('"', StringComparison.Ordinal) || line.Any(static c => c > 0x7E || c < 0x20))
            {
                throw new InvalidDataException($"a {name} line is not plain ASCII: `{line}`");
            }

            text.Append("        ").Append(line).Append('\n');
        }

        text.Append("        \"\"\"u8;\n\n");
    }

    private static void ByteTable(StringBuilder text, string name, string summary, byte[] data)
    {
        text.Append("    /// <summary>").Append(summary).Append("</summary>\n");
        text.Append("    // Broiler-AI:           EXEMPT=").Append(ExemptReason).Append('\n');
        text.Append("    // Broiler-Human:        PENDING\n");
        text.Append("    internal static ReadOnlySpan<byte> ").Append(name).Append(" => new byte[]\n    {\n");

        for (var at = 0; at < data.Length; at += 32)
        {
            text.Append("        ");

            for (var i = at; i < Math.Min(at + 32, data.Length); i++)
            {
                text.Append(data[i].ToString(CultureInfo.InvariantCulture)).Append(',');
                text.Append(i + 1 < Math.Min(at + 32, data.Length) ? " " : string.Empty);
            }

            text.Append('\n');
        }

        text.Append("    };\n\n");
    }
}
