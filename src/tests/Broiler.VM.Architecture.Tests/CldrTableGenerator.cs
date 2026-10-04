using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Writes the internationalization tables of <c>Broiler.VM.Profile.JavaScript.Intl</c> from the files
/// <c>cldr.pin</c> names, and the UCD files <c>unicode.pin</c> names (decision JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>One function from verified bytes to one file's text</b>, as <see cref="UnicodeTableGenerator"/>
/// is: rule N28 calls it to compare the checked-in file and, in write mode, to write it. Nothing here
/// reads a clock, the environment or a file the two pins do not name.
/// </para>
/// <para>
/// <b>The text tables are lines of fields separated by <c>|</c></b>, and the collation tables are
/// binary in the layouts their readers in the profile state (<c>JsCollationData</c>). The root
/// collation is <c>allkeys_CLDR.txt</c> as written, its single-element mappings folded into runs; the
/// tailorings - the root's <c>search</c> type and German's <c>phonebook</c> and <c>search</c> - are
/// built here from their rules into weights scaled by 256, so that a tailored weight has room between
/// two root weights without moving either.
/// </para>
/// </remarks>
internal static class CldrTableGenerator
{
    /// <summary>The file this writes, relative to the repository root.</summary>
    internal const string OutputPath = "src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs";

    /// <summary>The locales the data supports, in canonical form and ordinal order (JSD-0027 section 5 item 4).</summary>
    internal static readonly string[] SupportedLocales = ["de", "de-DE", "en", "en-US", "und"];

    /// <summary>
    /// UCA 17.0.0's implicit-weight ranges for the siniform ideographic scripts, which DUCET states in
    /// its <c>@implicitweights</c> lines and <c>allkeys_CLDR.txt</c> leaves out; quoted from
    /// <c>https://www.unicode.org/Public/17.0.0/uca/allkeys.txt</c>. The archived CollationTest files
    /// are what hold them to the root collation.
    /// </summary>
    internal static readonly (int First, int Last, int Base)[] SiniformRanges =
    [
        (0x17000, 0x187FF, 0xFB00),
        (0x18800, 0x18AFF, 0xFB01),
        (0x18D00, 0x18D7F, 0xFB00),
        (0x18D80, 0x18DFF, 0xFB01),
        (0x1B170, 0x1B2FF, 0xFB02),
        (0x18B00, 0x18CFF, 0xFB03),
    ];

    private const string Root = "cldr-48.2.0/";

    private static readonly IReadOnlyDictionary<string, string> NoImports = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>The generated file's text, from the two verified archives.</summary>
    internal static string Generate(
        CldrPin pin,
        IReadOnlyDictionary<string, byte[]> cldr,
        UnicodePin unicodePin,
        IReadOnlyDictionary<string, byte[]> ucd)
    {
        pin.Verified(cldr);
        unicodePin.Verified(ucd);

        var database = UnicodeDatabase.Read(ucd);
        var root = RootCollation.Read(CldrPin.Decode(cldr[Root + "common/uca/allkeys_CLDR.txt"]), database);
        var rootXml = CldrPin.Decode(cldr[Root + "common/collation/root.xml"]);
        var deXml = CldrPin.Decode(cldr[Root + "common/collation/de.xml"]);

        var rootSearch = Tailoring.Build(root, database, CollationRules(rootXml, "search"), imports: NoImports);
        var dePhonebook = Tailoring.Build(root, database, CollationRules(deXml, "phonebook"), imports: NoImports);
        var deSearch = Tailoring.Build(
            root,
            database,
            CollationRules(deXml, "search"),
            imports: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["und-u-co-search"] = CollationRules(rootXml, "search"),
                ["de-u-co-phonebk"] = CollationRules(deXml, "phonebook"),
            });

        var text = new StringBuilder();
        text.Append(Header).Append('\n');

        TextTable(text, "LikelySubtags", "CLDR's likely subtags: from, to.", LikelySubtags(cldr));
        TextTable(text, "Aliases", "CLDR's language, script, region, variant and subdivision aliases: kind, from, replacement.", Aliases(cldr));
        TextTable(text, "Extensions", "The BCP 47 extension keys and their types: extension, key, type, preferred type.", Extensions(cldr));
        TextTable(text, "Locales", "The locales this data supports.", SupportedLocales);
        TextTable(
            text,
            "SoftDotted",
            "The ranges of Unicode 17.0.0's Soft_Dotted, which Lithuanian's casing reads: first, last, in hexadecimal.",
            database.BinaryProperties.Single(static property => property.ShortName == "Soft_Dotted").Ranges
                .Select(static range => range.First.ToString("X4", CultureInfo.InvariantCulture) + "|" + range.Last.ToString("X4", CultureInfo.InvariantCulture)));
        TextTable(text, "NumberLocales", "The number data of each supported language for the latn numbering system, flattened: language, key, value.", NumberLocales(cldr));
        TextTable(text, "Currencies", "The currency names of each supported language: language, code, symbol, narrow symbol, singular name, plural name, name, and whether each symbol's first and last characters are symbols or separators.", Currencies(cldr, database));
        TextTable(text, "CurrencyDigits", "The currencies whose fraction digits are not 2: code, digits.", CurrencyDigits(cldr));
        TextTable(text, "NumberingSystems", "The numbering systems with a simple digit mapping: name, digits.", NumberingSystems(cldr));
        TextTable(text, "Plurals", "The cardinal plural rules of each supported language: language, category, rule.", Plurals(cldr));
        TextTable(text, "PluralRanges", "The plural range rules of each supported language: language, start, end, result.", PluralRanges(cldr));
        TextTable(text, "Units", "The sanctioned units' patterns of each supported language: language, width, unit, field, value.", Units(cldr));
        TextTable(text, "DateLocales", "The Gregorian calendar, date field and zone name data of each supported language, flattened: language, key, value.", DateLocales(cldr));
        TextTable(text, "TimeData", "The hour cycles allowed and preferred in each region: region, allowed, preferred.", TimeData(cldr));
        TextTable(text, "DayPeriods", "The day period rules of each supported language: language, period, at or from, before.", DayPeriods(cldr));
        TextTable(text, "WeekData", "Each region's week: region, first day, weekend start, weekend end, minimal days; empty where the world's applies.", WeekData(cldr));
        TextTable(text, "Scripts", "The line direction of each script CLDR states one for: script, right to left (YES) or not (NO).", Scripts(cldr));
        ByteTable(text, "CollationRoot", "The root collation: allkeys_CLDR.txt in runs and entries, the implicit-weight ranges and the unified ideographs.", root.Encode(database));
        ByteTable(
            text,
            "CollationTailorings",
            "The tailorings the supported locales use, each in weights scaled by 256: root search, German phonebook and German search.",
            Tailoring.Encode([("und-search", rootSearch), ("de-phonebk", dePhonebook), ("de-search", deSearch)]));

        // NO BLANK LINE BEFORE THE CLOSING BRACE, which every table but the last is followed by.
        while (text.Length > 2 && text[^1] == '\n' && text[^2] == '\n')
        {
            text.Length--;
        }

        text.Append("}\n");
        return text.ToString();
    }

    /// <summary>The environment variable that turns rule N28's gate into the generator's write.</summary>
    internal const string WriteVariable = "BROILER_CLDR_WRITE";

    /// <summary>The per-member exemption every generated table states.</summary>
    internal const string ExemptReason =
        "CLDR 48.2.0 table data written by CldrTableGenerator from the files cldr.pin names, compared byte for byte by rule N28";

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The generated file for this checkout, as the assurance generator would annotate it.</summary>
    internal static UnicodeArtefact Current => current.Value;

    private static readonly Lazy<UnicodeArtefact> current = new(static () =>
    {
        var pin = CldrPin.Load();
        var unicode = UnicodePin.Load();
        return Artefact(Generate(pin, pin.ReadArchive(), unicode, unicode.ReadArchive()));
    });

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

        // Written by CldrTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the CLDR 48.2.0
        // files that src/tests/cldr/pins/cldr.pin names and the Unicode Character Database 17.0.0 files
        // that src/tests/unicode/pins/unicode.pin names, under decision JSD-0043. Rule N28 regenerates
        // this file and compares it byte for byte, so a change belongs in the generator or a pinned
        // archive and never here: run the architecture tests with BROILER_CLDR_WRITE=1.
        //
        // The table data is derived from Unicode data files and is subject to the Unicode License v3
        // (SPDX Unicode-3.0), whose text THIRD_PARTY_NOTICES.md carries. The SPDX lines above are written
        // on every product file by the code assurance generator and describe this file's code.

        /// <summary>The CLDR 48.2.0 tables, as the generator wrote them.</summary>
        // Broiler-AI:           Origin=Derived; Spec=JSD-0043; IP=Low; Security=Low; Resources=1; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal static class JsCldrTables
        {
            /// <summary>The CLDR release the tables were generated from.</summary>
            // Broiler-AI:           EXEMPT=CLDR 48.2.0 table data written by CldrTableGenerator from the files cldr.pin names, compared byte for byte by rule N28
            // Broiler-Human:        PENDING
            internal const string Version = "48.2.0";

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

    private static JsonElement Json(IReadOnlyDictionary<string, byte[]> cldr, string path) =>
        JsonDocument.Parse(cldr[Root + path]).RootElement.Clone();

    /// <summary>Likely subtags, ordinal by source.</summary>
    internal static IEnumerable<string> LikelySubtags(IReadOnlyDictionary<string, byte[]> cldr) =>
        Json(cldr, "json/cldr-core/supplemental/likelySubtags.json")
            .GetProperty("supplemental").GetProperty("likelySubtags")
            .EnumerateObject()
            .Select(static entry => entry.Name + "|" + entry.Value.GetString())
            .Order(StringComparer.Ordinal);

    /// <summary>Every alias kind ECMA-402's canonicalization reads.</summary>
    internal static IEnumerable<string> Aliases(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var alias = Json(cldr, "json/cldr-core/supplemental/aliases.json")
            .GetProperty("supplemental").GetProperty("metadata").GetProperty("alias");

        var lines = new List<string>();

        foreach (var (kind, property) in new[]
                 {
                     ("language", "languageAlias"),
                     ("script", "scriptAlias"),
                     ("region", "territoryAlias"),
                     ("variant", "variantAlias"),
                     ("subdivision", "subdivisionAlias"),
                 })
        {
            foreach (var entry in alias.GetProperty(property).EnumerateObject())
            {
                lines.Add(kind + "|" + entry.Name + "|" + entry.Value.GetProperty("_replacement").GetString());
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    /// <summary>Every BCP 47 key and type the <c>bcp47</c> files define, with a deprecated type's preferred one.</summary>
    internal static IEnumerable<string> Extensions(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var lines = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var name in cldr.Keys.Where(static name => name.StartsWith(Root + "json/cldr-bcp47/bcp47/", StringComparison.Ordinal)))
        {
            var keyword = JsonDocument.Parse(cldr[name]).RootElement.GetProperty("keyword");

            foreach (var extension in keyword.EnumerateObject())
            {
                foreach (var key in extension.Value.EnumerateObject())
                {
                    lines.Add($"{extension.Name}|{key.Name}||");

                    var types = key.Value.EnumerateObject()
                        .Where(static type => !type.Name.StartsWith('_'))
                        .ToList();

                    foreach (var type in types)
                    {
                        var preferred = type.Value.TryGetProperty("_preferred", out var value) ? value.GetString() : string.Empty;
                        lines.Add($"{extension.Name}|{key.Name}|{type.Name}|{preferred}");
                    }

                    // A TYPE'S ALIASES ARE THE OTHER SPELLINGS IT ACCEPTS - `yes` for `true` - and
                    // canonicalization replaces each by the type, or by the type's own preferred
                    // one when the type is deprecated. Only an alias that is a well-formed type is
                    // one: a time zone's aliases are IANA names, and those of one segment - `Eire`,
                    // `EST5EDT` - are types once lower-cased, as UTS #35 reads a BCP 47 value.
                    foreach (var type in types)
                    {
                        if (!type.Value.TryGetProperty("_alias", out var alias))
                        {
                            continue;
                        }

                        var target = type.Value.TryGetProperty("_preferred", out var preferredType)
                            ? preferredType.GetString()!
                            : type.Name;

                        foreach (var written in alias.GetString()!.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var spelling = key.Name == "tz" ? written.ToLowerInvariant() : written;

                            if (WellFormedType.IsMatch(spelling) &&
                                !types.Any(other => string.Equals(other.Name, spelling, StringComparison.Ordinal)))
                            {
                                lines.Add($"{extension.Name}|{key.Name}|{spelling}|{target}");
                            }
                        }
                    }
                }
            }
        }

        return lines;
    }

    /// <summary>The languages whose number data the tables carry: the supported locales' languages.</summary>
    internal static readonly string[] NumberLanguages = ["de", "en"];

    /// <summary>ECMA-402's sanctioned single units (table 2 of the pinned edition).</summary>
    internal static readonly string[] SanctionedUnits =
    [
        "acre", "bit", "byte", "celsius", "centimeter", "day", "degree", "fahrenheit", "fluid-ounce", "foot",
        "gallon", "gigabit", "gigabyte", "gram", "hectare", "hour", "inch", "kilobit", "kilobyte", "kilogram",
        "kilometer", "liter", "megabit", "megabyte", "meter", "microsecond", "mile", "mile-scandinavian",
        "milliliter", "millimeter", "millisecond", "minute", "month", "nanosecond", "ounce", "percent", "petabyte",
        "pound", "second", "stone", "terabit", "terabyte", "week", "yard", "year",
    ];

    /// <summary>
    /// A table value with every character a line cannot carry written as <c>\uXXXX</c>: what is not
    /// printable ASCII, and the backslash, the bar and the quote.
    /// </summary>
    internal static string Escape(string value)
    {
        var escaped = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            if (c < 0x20 || c > 0x7E || c is '\\' or '|' or '"')
            {
                escaped.Append("\\u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
            else
            {
                escaped.Append(c);
            }
        }

        return escaped.ToString();
    }

    /// <summary>Each language's number data for <c>latn</c>, its JSON flattened to dotted keys.</summary>
    internal static IEnumerable<string> NumberLocales(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var lines = new List<string>();

        foreach (var language in NumberLanguages)
        {
            var numbers = Json(cldr, $"json/cldr-numbers-full/main/{language}/numbers.json")
                .GetProperty("main").GetProperty(language).GetProperty("numbers");

            foreach (var property in numbers.EnumerateObject())
            {
                var key = property.Name switch
                {
                    "symbols-numberSystem-latn" => "symbols",
                    "decimalFormats-numberSystem-latn" => "decimal",
                    "percentFormats-numberSystem-latn" => "percent",
                    "scientificFormats-numberSystem-latn" => "scientific",
                    "currencyFormats-numberSystem-latn" => "currency",
                    "miscPatterns-numberSystem-latn" => "misc",
                    "defaultNumberingSystem" or "minimumGroupingDigits" => property.Name,
                    _ => null,
                };

                if (key is not null)
                {
                    Flatten(language, key, property.Value, lines);
                }
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    private static void Flatten(string language, string key, JsonElement value, List<string> lines)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in value.EnumerateObject())
            {
                Flatten(language, key + "." + property.Name, property.Value, lines);
            }

            return;
        }

        lines.Add($"{language}|{key}|{Escape(value.GetString()!)}");
    }

    /// <summary>Each language's currency symbols and names.</summary>
    internal static IEnumerable<string> Currencies(IReadOnlyDictionary<string, byte[]> cldr, UnicodeDatabase database)
    {
        var lines = new List<string>();
        var symbolic = database.GeneralCategories
            .Where(static category => category.ShortName is "Sm" or "Sc" or "Sk" or "So" or "Zs" or "Zl" or "Zp")
            .SelectMany(static category => category.Ranges)
            .ToArray();

        // CLDR'S CURRENCY SPACING asks whether a symbol's character next to the number is
        // [[:^S:]&[:^Z:]]; the profile has no General_Category table, so each symbol states it here:
        // `S` for a symbol or separator, `L` for anything else, first character then last.
        string Ends(string symbol)
        {
            if (symbol.Length == 0)
            {
                return string.Empty;
            }

            char Kind(int codePoint) => symbolic.Any(range => range.First <= codePoint && codePoint <= range.Last) ? 'S' : 'L';
            return new string([Kind(char.ConvertToUtf32(symbol, 0)), Kind(char.IsLowSurrogate(symbol[^1]) ? char.ConvertToUtf32(symbol, symbol.Length - 2) : symbol[^1])]);
        }

        foreach (var language in NumberLanguages)
        {
            var currencies = Json(cldr, $"json/cldr-numbers-full/main/{language}/currencies.json")
                .GetProperty("main").GetProperty(language).GetProperty("numbers").GetProperty("currencies");

            foreach (var currency in currencies.EnumerateObject())
            {
                string Field(string name) =>
                    currency.Value.TryGetProperty(name, out var field) ? Escape(field.GetString()!) : string.Empty;

                string Raw(string name) =>
                    currency.Value.TryGetProperty(name, out var field) ? field.GetString()! : string.Empty;

                lines.Add(string.Join('|',
                    language, currency.Name, Field("symbol"), Field("symbol-alt-narrow"),
                    Field("displayName-count-one"), Field("displayName-count-other"), Field("displayName"),
                    Ends(Raw("symbol")), Ends(Raw("symbol-alt-narrow"))));
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    /// <summary>CLDR's currency fraction digits that differ from its default of 2.</summary>
    internal static IEnumerable<string> CurrencyDigits(IReadOnlyDictionary<string, byte[]> cldr) =>
        Json(cldr, "json/cldr-core/supplemental/currencyData.json")
            .GetProperty("supplemental").GetProperty("currencyData").GetProperty("fractions")
            .EnumerateObject()
            .Where(static currency => currency.Name != "DEFAULT" && currency.Value.GetProperty("_digits").GetString() != "2")
            .Select(static currency => currency.Name + "|" + currency.Value.GetProperty("_digits").GetString())
            .Order(StringComparer.Ordinal);

    /// <summary>The numeric numbering systems and their digits.</summary>
    internal static IEnumerable<string> NumberingSystems(IReadOnlyDictionary<string, byte[]> cldr) =>
        Json(cldr, "json/cldr-core/supplemental/numberingSystems.json")
            .GetProperty("supplemental").GetProperty("numberingSystems")
            .EnumerateObject()
            .Where(static system => system.Value.GetProperty("_type").GetString() == "numeric")
            .Select(static system => system.Name + "|" + Escape(system.Value.GetProperty("_digits").GetString()!))
            .Order(StringComparer.Ordinal);

    /// <summary>Each language's cardinal plural rules, without their samples.</summary>
    internal static IEnumerable<string> Plurals(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var rules = Json(cldr, "json/cldr-core/supplemental/plurals.json")
            .GetProperty("supplemental").GetProperty("plurals-type-cardinal");

        foreach (var language in NumberLanguages)
        {
            foreach (var rule in rules.GetProperty(language).EnumerateObject())
            {
                var text = rule.Value.GetString()!;
                var samples = text.IndexOf('@', StringComparison.Ordinal);
                yield return $"{language}|{rule.Name["pluralRule-count-".Length..]}|{Escape((samples < 0 ? text : text[..samples]).Trim())}";
            }
        }
    }

    /// <summary>Each language's plural range rules.</summary>
    internal static IEnumerable<string> PluralRanges(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var ranges = Json(cldr, "json/cldr-core/supplemental/pluralRanges.json")
            .GetProperty("supplemental").GetProperty("plurals");

        foreach (var language in NumberLanguages)
        {
            foreach (var range in ranges.GetProperty(language).EnumerateObject())
            {
                // pluralRange-start-<start>-end-<end>
                var parts = range.Name.Split('-');
                yield return $"{language}|{parts[2]}|{parts[4]}|{range.Value.GetString()}";
            }
        }
    }

    /// <summary>
    /// Each language's patterns for the sanctioned units and the compound units CLDR names whose
    /// two halves are sanctioned, in the three widths, and each width's compound pattern.
    /// </summary>
    internal static IEnumerable<string> Units(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var lines = new List<string>();

        bool Wanted(string unit)
        {
            if (SanctionedUnits.Contains(unit, StringComparer.Ordinal))
            {
                return true;
            }

            var per = unit.IndexOf("-per-", StringComparison.Ordinal);
            return per > 0 &&
                SanctionedUnits.Contains(unit[..per], StringComparer.Ordinal) &&
                SanctionedUnits.Contains(unit[(per + "-per-".Length)..], StringComparer.Ordinal);
        }

        foreach (var language in NumberLanguages)
        {
            var units = Json(cldr, $"json/cldr-units-full/main/{language}/units.json")
                .GetProperty("main").GetProperty(language).GetProperty("units");

            foreach (var width in new[] { "long", "short", "narrow" })
            {
                foreach (var entry in units.GetProperty(width).EnumerateObject())
                {
                    if (entry.Name == "per")
                    {
                        lines.Add($"{language}|{width}|per|compoundUnitPattern|{Escape(entry.Value.GetProperty("compoundUnitPattern").GetString()!)}");
                        continue;
                    }

                    var dash = entry.Name.IndexOf('-');

                    if (dash < 0 || entry.Value.ValueKind != JsonValueKind.Object || !Wanted(entry.Name[(dash + 1)..]))
                    {
                        continue;
                    }

                    foreach (var field in entry.Value.EnumerateObject())
                    {
                        if (field.Name is "displayName" or "perUnitPattern" || field.Name.StartsWith("unitPattern-count-", StringComparison.Ordinal))
                        {
                            lines.Add($"{language}|{width}|{entry.Name[(dash + 1)..]}|{field.Name}|{Escape(field.Value.GetString()!)}");
                        }
                    }
                }
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    /// <summary>
    /// Each language's Gregorian calendar data and the zone names a UTC or fixed-offset zone uses,
    /// flattened to dotted keys: the month, day, day period and era names; the date, time and
    /// date-time patterns; the available formats, append items and interval formats; the date
    /// fields' display names; and the GMT formats and the names of UTC and of the GMT zone. Alternate
    /// forms (<c>-alt-</c>) and plural-dependent formats (<c>-count-</c>) are not carried.
    /// </summary>
    internal static IEnumerable<string> DateLocales(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var lines = new List<string>();

        foreach (var language in NumberLanguages)
        {
            var gregorian = Json(cldr, $"json/cldr-dates-full/main/{language}/ca-gregorian.json")
                .GetProperty("main").GetProperty(language).GetProperty("dates").GetProperty("calendars").GetProperty("gregorian");

            foreach (var name in new[] { "months", "days", "dayPeriods", "eras", "dateFormats", "timeFormats" })
            {
                FlattenDates(language, name, gregorian.GetProperty(name), lines);
            }

            var dateTime = gregorian.GetProperty("dateTimeFormats");

            foreach (var style in new[] { "full", "long", "medium", "short" })
            {
                lines.Add($"{language}|dateTime.{style}|{Escape(dateTime.GetProperty(style).GetString()!)}");
                lines.Add($"{language}|atTime.{style}|{Escape(gregorian.GetProperty("dateTimeFormats-atTime").GetProperty("standard").GetProperty(style).GetString()!)}");
            }

            FlattenDates(language, "available", dateTime.GetProperty("availableFormats"), lines);
            FlattenDates(language, "append", dateTime.GetProperty("appendItems"), lines);

            foreach (var entry in dateTime.GetProperty("intervalFormats").EnumerateObject())
            {
                if (entry.Name == "intervalFormatFallback")
                {
                    lines.Add($"{language}|interval.fallback|{Escape(entry.Value.GetString()!)}");
                }
                else if (!entry.Name.Contains("-alt-", StringComparison.Ordinal))
                {
                    FlattenDates(language, "interval." + entry.Name, entry.Value, lines);
                }
            }

            var fields = Json(cldr, $"json/cldr-dates-full/main/{language}/dateFields.json")
                .GetProperty("main").GetProperty(language).GetProperty("dates").GetProperty("fields");

            foreach (var field in new[] { "era", "year", "quarter", "month", "week", "weekOfMonth", "weekday", "dayOfYear", "weekdayOfMonth", "day", "dayperiod", "hour", "minute", "second", "zone" })
            {
                lines.Add($"{language}|field.{field}|{Escape(fields.GetProperty(field).GetProperty("displayName").GetString()!)}");
            }

            var zones = Json(cldr, $"json/cldr-dates-full/main/{language}/timeZoneNames.json")
                .GetProperty("main").GetProperty(language).GetProperty("dates").GetProperty("timeZoneNames");

            foreach (var name in new[] { "hourFormat", "gmtFormat", "gmtZeroFormat" })
            {
                lines.Add($"{language}|zone.{name}|{Escape(zones.GetProperty(name).GetString()!)}");
            }

            foreach (var (key, names) in new[]
            {
                ("utc", zones.GetProperty("zone").GetProperty("Etc").GetProperty("UTC")),
                ("gmt", zones.GetProperty("metazone").GetProperty("GMT")),
            })
            {
                foreach (var width in new[] { "long", "short" })
                {
                    if (names.TryGetProperty(width, out var named))
                    {
                        lines.Add($"{language}|zone.{key}.{width}|{Escape(named.GetProperty("standard").GetString()!)}");
                    }
                }
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    private static void FlattenDates(string language, string key, JsonElement value, List<string> lines)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in value.EnumerateObject())
            {
                if (!property.Name.Contains("-alt-", StringComparison.Ordinal) && !property.Name.Contains("-count-", StringComparison.Ordinal))
                {
                    FlattenDates(language, key + "." + property.Name, property.Value, lines);
                }
            }

            return;
        }

        lines.Add($"{language}|{key}|{Escape(value.GetString()!)}");
    }

    /// <summary>The hour cycles CLDR allows and prefers in each region it names: region, allowed, preferred.</summary>
    internal static IEnumerable<string> TimeData(IReadOnlyDictionary<string, byte[]> cldr) =>
        Json(cldr, "json/cldr-core/supplemental/timeData.json").GetProperty("supplemental").GetProperty("timeData")
            .EnumerateObject()
            .Select(static entry => $"{entry.Name}|{entry.Value.GetProperty("_allowed").GetString()}|{entry.Value.GetProperty("_preferred").GetString()}")
            .Order(StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Each region's week: region, first day, weekend start, weekend end and minimal days in the first
    /// week, a field empty where CLDR states none and the world's (<c>001</c>) applies.
    /// </summary>
    internal static IEnumerable<string> WeekData(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var week = Json(cldr, "json/cldr-core/supplemental/weekData.json").GetProperty("supplemental").GetProperty("weekData");
        var fields = new[] { "firstDay", "weekendStart", "weekendEnd", "minDays" };
        var regions = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var field in fields)
        {
            foreach (var entry in week.GetProperty(field).EnumerateObject())
            {
                if (!entry.Name.Contains("-alt-", StringComparison.Ordinal))
                {
                    regions.Add(entry.Name);
                }
            }
        }

        return regions.Select(region => region + string.Concat(fields.Select(field =>
            "|" + (week.GetProperty(field).TryGetProperty(region, out var value) ? value.GetString() : string.Empty)))).ToList();
    }

    /// <summary>Each script whose line direction CLDR states: script, and whether it is right to left (<c>YES</c>) or not (<c>NO</c>).</summary>
    internal static IEnumerable<string> Scripts(IReadOnlyDictionary<string, byte[]> cldr) =>
        Json(cldr, "json/cldr-core/scriptMetadata.json").GetProperty("scriptMetadata")
            .EnumerateObject()
            .Where(static entry => entry.Value.GetProperty("rtl").GetString() is "YES" or "NO")
            .Select(static entry => entry.Name + "|" + entry.Value.GetProperty("rtl").GetString())
            .Order(StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Each supported language's day period rules: language, period, and the time it is at or the
    /// time it is from and the time it is before.
    /// </summary>
    internal static IEnumerable<string> DayPeriods(IReadOnlyDictionary<string, byte[]> cldr)
    {
        var rules = Json(cldr, "json/cldr-core/supplemental/dayPeriods.json").GetProperty("supplemental").GetProperty("dayPeriodRuleSet");
        var lines = new List<string>();

        foreach (var language in NumberLanguages)
        {
            foreach (var period in rules.GetProperty(language).EnumerateObject())
            {
                lines.Add(period.Value.TryGetProperty("_at", out var at)
                    ? $"{language}|{period.Name}|{at.GetString()}|"
                    : $"{language}|{period.Name}|{period.Value.GetProperty("_from").GetString()}|{period.Value.GetProperty("_before").GetString()}");
            }
        }

        return lines.Order(StringComparer.Ordinal);
    }

    /// <summary>A BCP 47 type: subtags of three to eight lowercase letters and digits.</summary>
    private static readonly Regex WellFormedType = new("^[a-z0-9]{3,8}(-[a-z0-9]{3,8})*$", RegexOptions.CultureInvariant);

    /// <summary>The rules of one collation type of an LDML collation file, with comments and markup removed.</summary>
    internal static string CollationRules(string xml, string type)
    {
        var open = Regex.Match(xml, $"<collation type=['\"]{Regex.Escape(type)}['\"][^>]*>");

        if (!open.Success)
        {
            throw new InvalidDataException($"no collation of type {type}");
        }

        var start = xml.IndexOf("<![CDATA[", open.Index, StringComparison.Ordinal) + "<![CDATA[".Length;
        var end = xml.IndexOf("]]></cr>", start, StringComparison.Ordinal);
        return xml[start..end];
    }

    /// <summary>A code point sequence in NFD, decomposed with the archived UCD and ordered canonically.</summary>
    internal static int[] Nfd(UnicodeDatabase database, IEnumerable<int> codePoints)
    {
        var result = new List<int>();

        void Decompose(int codePoint)
        {
            if (codePoint is >= 0xAC00 and <= 0xD7A3)
            {
                result.AddRange(UnicodeDatabase.Hangul(codePoint));
                return;
            }

            if (database.CanonicalDecomposition.TryGetValue(codePoint, out var parts))
            {
                foreach (var part in parts)
                {
                    Decompose(part);
                }

                return;
            }

            result.Add(codePoint);
        }

        foreach (var codePoint in codePoints)
        {
            Decompose(codePoint);
        }

        // CANONICAL ORDERING: a stable sort of each run of non-starters by combining class.
        for (var i = 1; i < result.Count; i++)
        {
            var at = i;

            while (at > 0 &&
                   database.CombiningClass[result[at]] != 0 &&
                   database.CombiningClass[result[at - 1]] > database.CombiningClass[result[at]])
            {
                (result[at - 1], result[at]) = (result[at], result[at - 1]);
                at--;
            }
        }

        return [.. result];
    }

    /// <summary>The code points of a string.</summary>
    internal static IEnumerable<int> CodePoints(string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            var codePoint = char.ConvertToUtf32(text, i);

            if (codePoint > 0xFFFF)
            {
                i++;
            }

            yield return codePoint;
        }
    }

    /// <summary>One collation element in root weights: primary, secondary, tertiary and whether it is variable.</summary>
    internal readonly record struct Element(int Primary, int Secondary, int Tertiary, bool Variable);

    /// <summary>The root collation as <c>allkeys_CLDR.txt</c> states it.</summary>
    internal sealed class RootCollation
    {
        private RootCollation(Dictionary<string, Element[]> entries) => Entries = entries;

        /// <summary>Every mapping, keyed by its code points joined with spaces in hex.</summary>
        internal Dictionary<string, Element[]> Entries { get; }

        internal static RootCollation Read(string text, UnicodeDatabase database)
        {
            var entries = new Dictionary<string, Element[]>(StringComparer.Ordinal);
            var element = new Regex(@"\[([.*])([0-9A-F]{4})\.([0-9A-F]{4})\.([0-9A-F]{4})\]", RegexOptions.CultureInvariant);

            foreach (var raw in text.Split('\n'))
            {
                var line = raw.TrimEnd('\r');

                if (line.Length == 0 || line[0] is '#' or '@')
                {
                    continue;
                }

                var semicolon = line.IndexOf(';', StringComparison.Ordinal);
                var key = string.Join(' ', line[..semicolon].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                var weights = line[semicolon..];
                var hash = weights.IndexOf('#', StringComparison.Ordinal);

                if (hash >= 0)
                {
                    weights = weights[..hash];
                }

                var ces = element.Matches(weights)
                    .Select(static match => new Element(
                        int.Parse(match.Groups[2].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                        int.Parse(match.Groups[3].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                        int.Parse(match.Groups[4].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                        match.Groups[1].Value == "*"))
                    .ToArray();

                if (ces.Length == 0 || !entries.TryAdd(key, ces))
                {
                    throw new InvalidDataException($"allkeys_CLDR.txt: `{line}` maps nothing or repeats a key");
                }

                if (ces.Any(static ce => ce.Secondary > 0x1FFF || ce.Tertiary > 0x1F))
                {
                    throw new InvalidDataException($"allkeys_CLDR.txt: `{line}` has a weight wider than its encoding");
                }
            }

            return new RootCollation(entries);
        }

        /// <summary>A key's code points.</summary>
        internal static int[] Key(string key) =>
            [.. key.Split(' ').Select(static part => int.Parse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture))];

        /// <summary>The key of some code points.</summary>
        internal static string KeyOf(IEnumerable<int> codePoints) =>
            string.Join(' ', codePoints.Select(static codePoint => codePoint.ToString("X4", CultureInfo.InvariantCulture)));

        /// <summary>UCA 17's implicit weights for a code point no key names.</summary>
        internal static Element[] Implicit(int codePoint, UnicodeDatabase database)
        {
            foreach (var (first, last, @base) in SiniformRanges)
            {
                if (codePoint >= first && codePoint <= last)
                {
                    var origin = SiniformRanges.Where(range => range.Base == @base).Min(static range => range.First);
                    return [new Element(@base, 0x20, 0x02, false), new Element((codePoint - origin) | 0x8000, 0, 0, false)];
                }
            }

            var unified = UnifiedIdeographs(database).Any(range => codePoint >= range.First && codePoint <= range.Last);
            var core = codePoint is (>= 0x4E00 and <= 0x9FFF) or (>= 0xF900 and <= 0xFAFF);
            var primary = unified && core ? 0xFB40 : unified ? 0xFB80 : 0xFBC0;

            return [new Element(primary + (codePoint >> 15), 0x20, 0x02, false), new Element((codePoint & 0x7FFF) | 0x8000, 0, 0, false)];
        }

        /// <summary>The Unified_Ideograph ranges of the archived PropList.txt.</summary>
        internal static List<UnicodeRange> UnifiedIdeographs(UnicodeDatabase database) =>
            database.BinaryProperties.Single(static property => property.LongName == "Unified_Ideograph").Ranges;

        /// <summary>
        /// The binary table: runs of single elements whose primary rises with the code point, then
        /// every other mapping, then the siniform ranges, then the unified ideographs.
        /// </summary>
        internal byte[] Encode(UnicodeDatabase database)
        {
            var singles = Entries
                .Where(static entry => !entry.Key.Contains(' ', StringComparison.Ordinal) && entry.Value.Length == 1)
                .Select(static entry => (CodePoint: Key(entry.Key)[0], Element: entry.Value[0]))
                .OrderBy(static entry => entry.CodePoint)
                .ToList();

            var runs = new List<(int First, int Length, Element Element)>();

            foreach (var (codePoint, element) in singles)
            {
                if (runs.Count > 0)
                {
                    var (first, length, start) = runs[^1];

                    if (codePoint == first + length && length < 0xFFFF &&
                        element == start with { Primary = start.Primary + length } &&
                        start.Primary + length <= 0xFFFF)
                    {
                        runs[^1] = (first, length + 1, start);
                        continue;
                    }
                }

                runs.Add((codePoint, 1, element));
            }

            var others = Entries
                .Where(static entry => entry.Key.Contains(' ', StringComparison.Ordinal) || entry.Value.Length != 1)
                .OrderBy(static entry => entry.Key, StringComparer.Ordinal)
                .ToList();

            var bytes = new List<byte>();
            UInt(bytes, runs.Count);

            foreach (var (first, length, element) in runs)
            {
                UInt24(bytes, first);
                UInt16(bytes, length);
                Weights(bytes, element);
            }

            UInt(bytes, others.Count);

            foreach (var (key, ces) in others)
            {
                var codePoints = Key(key);
                bytes.Add(checked((byte)codePoints.Length));

                foreach (var codePoint in codePoints)
                {
                    UInt24(bytes, codePoint);
                }

                bytes.Add(checked((byte)ces.Length));

                foreach (var ce in ces)
                {
                    Weights(bytes, ce);
                }
            }

            UInt(bytes, SiniformRanges.Length);

            foreach (var (first, last, @base) in SiniformRanges)
            {
                UInt24(bytes, first);
                UInt24(bytes, last);
                UInt16(bytes, @base);
            }

            var unified = UnifiedIdeographs(database);
            UInt(bytes, unified.Count);

            foreach (var range in unified)
            {
                UInt24(bytes, range.First);
                UInt24(bytes, range.Last);
            }

            return [.. bytes];
        }

        /// <summary>One element in five bytes: primary, secondary, then tertiary with the variable flag in its top bit.</summary>
        private static void Weights(List<byte> bytes, Element element)
        {
            UInt16(bytes, element.Primary);
            UInt16(bytes, element.Secondary);
            bytes.Add((byte)(element.Tertiary | (element.Variable ? 0x80 : 0)));
        }
    }

    /// <summary>
    /// A tailoring built from CLDR collation rules over the root, in weights scaled by 256 (the
    /// <c>Scaled</c> layout <c>JsCollationData</c> reads).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The subset of the rule syntax the supported tailorings use</b>: resets, the special reset
    /// <c>[last primary ignorable]</c>, the relations <c>&lt;</c>, <c>&lt;&lt;</c>, <c>&lt;&lt;&lt;</c>
    /// and <c>=</c>, quoting, <c>\u</c> escapes, comments, <c>[import]</c>,
    /// <c>[suppressContractions]</c> and <c>[normalization on]</c>. Anything else is refused by name,
    /// so a re-pin that brings syntax this engine does not know fails generation instead of
    /// generating a different order.
    /// </para>
    /// <para>
    /// <b>A relation takes the weight one above its predecessor at its level</b>, and the common
    /// weight at the levels below it: every root weight is a multiple of 256, so a tailored weight
    /// lands between its predecessor and the next root weight. A relation whose weights another
    /// tailored item already holds is moved past it; that is where this engine and ICU's would place
    /// a later rule differently, and none of the supported tailorings reaches it twice from one reset.
    /// </para>
    /// </remarks>
    internal sealed class Tailoring
    {
        /// <summary>The scaled weight of the root's common secondary, 0x0020.</summary>
        private const long CommonSecondary = 0x20 << 8;

        /// <summary>The scaled weight of the root's common tertiary, 0x0002.</summary>
        private const long CommonTertiary = 0x02 << 8;

        private Tailoring()
        {
        }

        /// <summary>Every tailored mapping: NFD key to scaled elements.</summary>
        internal SortedDictionary<string, ScaledElement[]> Entries { get; } = new(StringComparer.Ordinal);

        /// <summary>The first code points whose root contractions this tailoring suppresses.</summary>
        internal SortedSet<int> Suppressed { get; } = [];

        /// <summary>
        /// One element in scaled weights, and its case: 0 lower or uncased, 1 mixed, 2 upper, or -1
        /// where nothing stated it yet.
        /// </summary>
        internal readonly record struct ScaledElement(long Primary, long Secondary, long Tertiary, bool Variable, int Case = -1)
        {
            internal static ScaledElement From(Element element) =>
                new((long)element.Primary << 8, (long)element.Secondary << 8, (long)element.Tertiary << 8, element.Variable);
        }

        /// <summary>The root tertiaries that mark an upper-case character, as the profile's reader reads them.</summary>
        internal static readonly int[] UpperTertiaries = [0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x1D];

        /// <summary>
        /// A tailored string's elements with their case stated, as ICU's <c>CollationBuilder::setCaseBits</c>
        /// states it: the string's own root elements give their case, the first ones to the tailored
        /// primaries in order and the rest, together, to the last; a secondary element is uncased and a
        /// tertiary one upper. A tailored weight says nothing about case, since it sits between two root
        /// weights that may disagree.
        /// </summary>
        internal static ScaledElement[] WithCase(ScaledElement[] tailored, IReadOnlyList<Element> rootOfString)
        {
            var primaries = tailored.Count(static element => element.Primary != 0);
            var cases = new int[Math.Max(primaries, 1)];
            var seen = 0;
            var lastCase = 0;

            foreach (var element in rootOfString.Where(static element => element.Primary != 0))
            {
                seen++;
                var stated = UpperTertiaries.Contains(element.Tertiary) ? 2 : 0;

                if (seen < primaries)
                {
                    cases[seen - 1] = stated;
                }
                else if (seen == primaries)
                {
                    lastCase = stated;
                }
                else if (stated != lastCase)
                {
                    lastCase = 1;
                    break;
                }
            }

            if (primaries > 0 && seen >= primaries)
            {
                cases[primaries - 1] = lastCase;
            }

            var result = new ScaledElement[tailored.Length];
            var primary = 0;

            for (var index = 0; index < tailored.Length; index++)
            {
                var element = tailored[index];
                result[index] = element with
                {
                    Case = element.Primary != 0 ? cases[primary++] : element.Secondary != 0 ? 0 : 2,
                };
            }

            return result;
        }

        internal static Tailoring Build(
            RootCollation root, UnicodeDatabase database, string rules, IReadOnlyDictionary<string, string> imports)
        {
            var tailoring = new Tailoring();
            tailoring.Apply(root, database, rules, imports);
            return tailoring;
        }

        private void Apply(RootCollation root, UnicodeDatabase database, string rules, IReadOnlyDictionary<string, string> imports)
        {
            var used = new HashSet<string>(StringComparer.Ordinal);
            ScaledElement[]? current = null;
            var at = 0;

            // THE ROOT'S OWN ELEMENTS OF A STRING, by longest match, which a tailored string's case is read from.
            List<Element> RootOf(int[] key)
            {
                var result = new List<Element>();
                var index = 0;

                while (index < key.Length)
                {
                    var matched = false;

                    for (var length = key.Length - index; length > 0; length--)
                    {
                        if (root.Entries.TryGetValue(RootCollation.KeyOf(key.Skip(index).Take(length)), out var ces))
                        {
                            result.AddRange(ces);
                            index += length;
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        result.AddRange(RootCollation.Implicit(key[index], database));
                        index++;
                    }
                }

                return result;
            }

            ScaledElement[] Lookup(int[] key)
            {
                // A TAILORED KEY WINS OVER THE ROOT'S, by longest match, as at run time.
                var result = new List<ScaledElement>();
                var index = 0;

                while (index < key.Length)
                {
                    var matched = false;

                    for (var length = key.Length - index; length > 0; length--)
                    {
                        var part = RootCollation.KeyOf(key.Skip(index).Take(length));

                        if (Entries.TryGetValue(part, out var tailored))
                        {
                            result.AddRange(tailored);
                            index += length;
                            matched = true;
                            break;
                        }

                        if (root.Entries.TryGetValue(part, out var ces))
                        {
                            result.AddRange(ces.Select(ScaledElement.From));
                            index += length;
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        result.AddRange(RootCollation.Implicit(key[index], database).Select(ScaledElement.From));
                        index++;
                    }
                }

                return [.. result];
            }

            while (true)
            {
                SkipSpace(rules, ref at);

                if (at >= rules.Length)
                {
                    return;
                }

                if (rules[at] == '[')
                {
                    var option = Bracketed(rules, ref at);

                    if (option.StartsWith("import ", StringComparison.Ordinal))
                    {
                        var name = option["import ".Length..].Trim();

                        if (!imports.TryGetValue(name, out var imported))
                        {
                            throw new InvalidDataException($"collation rules import `{name}`, which the generator was not given");
                        }

                        Apply(root, database, imported, imports);
                        continue;
                    }

                    if (option == "normalization on")
                    {
                        continue;
                    }

                    if (option.StartsWith("suppressContractions ", StringComparison.Ordinal))
                    {
                        foreach (var codePoint in UnicodeSet(option["suppressContractions ".Length..].Trim()))
                        {
                            Suppressed.Add(codePoint);
                        }

                        continue;
                    }

                    throw new InvalidDataException($"collation rules use the option `[{option}]`, which this generator does not implement");
                }

                if (rules[at] == '&')
                {
                    at++;
                    SkipSpace(rules, ref at);

                    if (rules[at] == '[')
                    {
                        var position = Bracketed(rules, ref at);

                        if (position != "last primary ignorable")
                        {
                            throw new InvalidDataException($"collation rules reset to `[{position}]`, which this generator does not implement");
                        }

                        // THE GREATEST ELEMENT THAT IS PRIMARY-IGNORABLE AND NOT SECONDARY-IGNORABLE.
                        var last = root.Entries.Values
                            .Where(static ces => ces.Length == 1 && ces[0].Primary == 0 && ces[0].Secondary != 0)
                            .Select(static ces => ces[0])
                            .MaxBy(static ce => (ce.Secondary, ce.Tertiary));

                        current = [ScaledElement.From(last)];
                        continue;
                    }

                    current = Lookup(Nfd(database, Text(rules, ref at)));
                    continue;
                }

                if (current is null)
                {
                    throw new InvalidDataException("collation rules relate a string before any reset");
                }

                var strength = Relation(rules, ref at);
                var target = Nfd(database, Text(rules, ref at));

                if (target.Length == 0)
                {
                    throw new InvalidDataException("collation rules relate an empty string");
                }

                var next = (ScaledElement[])current.Clone();

                if (strength != 0)
                {
                    var last = next.Length - 1;

                    do
                    {
                        var element = next[last];

                        next[last] = strength switch
                        {
                            1 => element with { Primary = element.Primary + 1, Secondary = CommonSecondary, Tertiary = CommonTertiary },
                            2 => element with { Secondary = element.Secondary + 1, Tertiary = CommonTertiary },
                            _ => element with { Tertiary = element.Tertiary + 1 },
                        };
                    }
                    while (!used.Add(Describe(next)));

                    var overflowed = strength switch
                    {
                        1 => next[last].Primary % 256 == 0,
                        2 => next[last].Secondary % 256 == 0,
                        _ => next[last].Tertiary % 256 == 0,
                    };

                    if (overflowed)
                    {
                        throw new InvalidDataException("a tailored weight ran into the next root weight");
                    }
                }

                next = WithCase(next, RootOf(target));
                Entries[RootCollation.KeyOf(target)] = next;
                current = next;
            }
        }

        private static string Describe(ScaledElement[] elements) =>
            string.Join(',', elements.Select(static element => $"{element.Primary}.{element.Secondary}.{element.Tertiary}"));

        private static void SkipSpace(string rules, ref int at)
        {
            while (at < rules.Length)
            {
                if (char.IsWhiteSpace(rules[at]))
                {
                    at++;
                    continue;
                }

                if (rules[at] == '#')
                {
                    while (at < rules.Length && rules[at] != '\n')
                    {
                        at++;
                    }

                    continue;
                }

                break;
            }
        }

        private static string Bracketed(string rules, ref int at)
        {
            var depth = 0;
            var start = at + 1;

            for (; at < rules.Length; at++)
            {
                if (rules[at] == '[')
                {
                    depth++;
                }
                else if (rules[at] == ']' && --depth == 0)
                {
                    at++;
                    return rules[start..(at - 1)];
                }
            }

            throw new InvalidDataException("collation rules leave a bracket open");
        }

        /// <summary>The relation at <paramref name="at"/>: 1, 2 or 3 for its level, 0 for <c>=</c>.</summary>
        private static int Relation(string rules, ref int at)
        {
            if (rules[at] == '=')
            {
                at++;
                return 0;
            }

            var level = 0;

            while (at < rules.Length && rules[at] == '<')
            {
                level++;
                at++;
            }

            if (level is < 1 or > 3 || at < rules.Length && rules[at] == '*')
            {
                throw new InvalidDataException("collation rules use a relation this generator does not implement");
            }

            return level;
        }

        /// <summary>The text after a reset or relation, up to the next operator, with quotes and escapes resolved.</summary>
        private static int[] Text(string rules, ref int at)
        {
            var text = new StringBuilder();
            SkipSpace(rules, ref at);

            while (at < rules.Length)
            {
                var c = rules[at];

                if (c is '&' or '<' or '=' or '#' or '[' || char.IsWhiteSpace(c))
                {
                    break;
                }

                if (c is '|' or '/')
                {
                    throw new InvalidDataException("collation rules use a prefix or an extension, which this generator does not implement");
                }

                if (c == '\'')
                {
                    var close = rules.IndexOf('\'', at + 1);
                    text.Append(close == at + 1 ? "'" : rules[(at + 1)..close]);
                    at = close + 1;
                    continue;
                }

                if (c == '\\')
                {
                    text.Append(Escape(rules, ref at));
                    continue;
                }

                text.Append(c);
                at++;
            }

            return [.. CodePoints(text.ToString())];
        }

        private static string Escape(string rules, ref int at)
        {
            if (rules[at + 1] == 'u')
            {
                var value = int.Parse(rules.AsSpan(at + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                at += 6;
                return char.ConvertFromUtf32(value);
            }

            if (rules[at + 1] == 'U')
            {
                var value = int.Parse(rules.AsSpan(at + 2, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                at += 10;
                return char.ConvertFromUtf32(value);
            }

            at += 2;
            return rules[at - 1].ToString();
        }

        /// <summary>A UnicodeSet of single characters and ranges, as <c>[suppressContractions]</c> writes one.</summary>
        private static IEnumerable<int> UnicodeSet(string set)
        {
            if (set.Length < 2 || set[0] != '[' || set[^1] != ']')
            {
                throw new InvalidDataException($"`{set}` is not a set this generator reads");
            }

            var body = set[1..^1];
            var codePoints = new List<int>();
            var at = 0;

            while (at < body.Length)
            {
                if (char.IsWhiteSpace(body[at]))
                {
                    at++;
                    continue;
                }

                var first = body[at] == '\\' ? char.ConvertToUtf32(Escape(body, ref at), 0) : Next(body, ref at);

                if (at < body.Length && body[at] == '-')
                {
                    at++;
                    var last = body[at] == '\\' ? char.ConvertToUtf32(Escape(body, ref at), 0) : Next(body, ref at);

                    for (var codePoint = first; codePoint <= last; codePoint++)
                    {
                        codePoints.Add(codePoint);
                    }
                }
                else
                {
                    codePoints.Add(first);
                }
            }

            return codePoints;

            static int Next(string text, ref int at)
            {
                var codePoint = char.ConvertToUtf32(text, at);
                at += codePoint > 0xFFFF ? 2 : 1;
                return codePoint;
            }
        }

        /// <summary>
        /// The binary table: per tailoring its name, its entries in scaled weights packed into eight
        /// bytes (primary 24 bits, secondary 17, tertiary 13, the variable flag in the lowest bit), and
        /// the code points whose root contractions it suppresses.
        /// </summary>
        internal static byte[] Encode(IReadOnlyList<(string Name, Tailoring Tailoring)> tailorings)
        {
            var bytes = new List<byte>();
            UInt(bytes, tailorings.Count);

            foreach (var (name, tailoring) in tailorings)
            {
                bytes.Add(checked((byte)name.Length));
                bytes.AddRange(Encoding.ASCII.GetBytes(name));
                UInt(bytes, tailoring.Entries.Count);

                foreach (var (key, elements) in tailoring.Entries)
                {
                    var codePoints = RootCollation.Key(key);
                    bytes.Add(checked((byte)codePoints.Length));

                    foreach (var codePoint in codePoints)
                    {
                        UInt24(bytes, codePoint);
                    }

                    bytes.Add(checked((byte)elements.Length));

                    foreach (var element in elements)
                    {
                        if (element.Primary >= 1L << 24 || element.Secondary >= 1L << 17 || element.Tertiary >= 1L << 13)
                        {
                            throw new InvalidDataException("a scaled weight is wider than its packing");
                        }

                        if (element.Case is < 0 or > 2)
                        {
                            throw new InvalidDataException("a tailored element's case was never stated");
                        }

                        // THE CASE IS STATED IN BITS 1 AND 2, AND BIT 3 SAYS IT IS: a root element's reader
                        // derives its case from its tertiary, and a tailored one's cannot.
                        var packed = (element.Primary << 40) | (element.Secondary << 23) | (element.Tertiary << 10) |
                                     ((long)element.Case << 1) | (1L << 3) | (element.Variable ? 1L : 0L);

                        for (var shift = 0; shift < 64; shift += 8)
                        {
                            bytes.Add((byte)(packed >> shift));
                        }
                    }
                }

                UInt(bytes, tailoring.Suppressed.Count);

                foreach (var codePoint in tailoring.Suppressed)
                {
                    UInt24(bytes, codePoint);
                }
            }

            return [.. bytes];
        }
    }

    private static void UInt(List<byte> bytes, int value)
    {
        bytes.Add((byte)value);
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value >> 16));
        bytes.Add((byte)(value >> 24));
    }

    private static void UInt24(List<byte> bytes, int value)
    {
        bytes.Add((byte)value);
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value >> 16));
    }

    private static void UInt16(List<byte> bytes, int value)
    {
        bytes.Add((byte)value);
        bytes.Add((byte)(value >> 8));
    }
}
