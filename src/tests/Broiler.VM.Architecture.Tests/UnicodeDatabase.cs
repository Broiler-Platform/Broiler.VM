using System.Globalization;

namespace Broiler.VM.Architecture.Tests;

/// <summary>An inclusive range of code points.</summary>
internal readonly record struct UnicodeRange(int First, int Last);

/// <summary>Reading the UCD's semicolon-separated files.</summary>
internal static class UnicodeDataFile
{
    /// <summary>The highest code point.</summary>
    internal const int MaxCodePoint = 0x10FFFF;

    /// <summary>
    /// Every data line of a UCD file as trimmed fields, with the <c>#</c> comment removed and
    /// blank lines skipped.
    /// </summary>
    internal static IEnumerable<string[]> Records(string text)
    {
        foreach (var raw in text.Split('\n'))
        {
            var hash = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (hash >= 0 ? raw[..hash] : raw).Trim();

            if (line.Length > 0)
            {
                yield return line.Split(';').Select(static field => field.Trim()).ToArray();
            }
        }
    }

    /// <summary>The comment of a data line, or the empty string.</summary>
    internal static IEnumerable<(string[] Fields, string Comment)> RecordsWithComments(string text)
    {
        foreach (var raw in text.Split('\n'))
        {
            var hash = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (hash >= 0 ? raw[..hash] : raw).Trim();

            if (line.Length > 0)
            {
                yield return (
                    line.Split(';').Select(static field => field.Trim()).ToArray(),
                    hash >= 0 ? raw[(hash + 1)..].Trim() : string.Empty);
            }
        }
    }

    /// <summary><c>0041</c> or <c>0041..005A</c>.</summary>
    internal static UnicodeRange Range(string field)
    {
        var dots = field.IndexOf("..", StringComparison.Ordinal);

        return dots < 0
            ? new UnicodeRange(Hex(field), Hex(field))
            : new UnicodeRange(Hex(field[..dots]), Hex(field[(dots + 2)..]));
    }

    internal static int Hex(string text)
    {
        var value = int.Parse(text, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

        if (value is < 0 or > MaxCodePoint)
        {
            throw new InvalidDataException($"{text} is not a code point");
        }

        return value;
    }

    /// <summary>Sorted, merged: ascending, disjoint and never adjacent.</summary>
    internal static List<UnicodeRange> Normalize(IEnumerable<UnicodeRange> ranges)
    {
        var merged = new List<UnicodeRange>();

        foreach (var range in ranges.OrderBy(static range => range.First))
        {
            if (merged.Count > 0 && range.First <= merged[^1].Last + 1)
            {
                merged[^1] = merged[^1] with { Last = Math.Max(merged[^1].Last, range.Last) };
            }
            else
            {
                merged.Add(range);
            }
        }

        return merged;
    }

    /// <summary>Every code point not in the (normalized) ranges.</summary>
    internal static List<UnicodeRange> Complement(IReadOnlyList<UnicodeRange> ranges)
    {
        var complement = new List<UnicodeRange>();
        var next = 0;

        foreach (var range in ranges)
        {
            if (range.First > next)
            {
                complement.Add(new UnicodeRange(next, range.First - 1));
            }

            next = range.Last + 1;
        }

        if (next <= MaxCodePoint)
        {
            complement.Add(new UnicodeRange(next, MaxCodePoint));
        }

        return complement;
    }

    /// <summary>The maximal runs of one value in a per-code-point array, for each value.</summary>
    internal static Dictionary<int, List<UnicodeRange>> Runs(IReadOnlyList<int> valueOf)
    {
        var runs = new Dictionary<int, List<UnicodeRange>>();
        var start = 0;

        for (var codePoint = 1; codePoint <= valueOf.Count; codePoint++)
        {
            if (codePoint == valueOf.Count || valueOf[codePoint] != valueOf[start])
            {
                var value = valueOf[start];

                if (!runs.TryGetValue(value, out var list))
                {
                    runs[value] = list = [];
                }

                list.Add(new UnicodeRange(start, codePoint - 1));
                start = codePoint;
            }
        }

        return runs;
    }
}

/// <summary>A property value: the set it resolves to, and every name the UCD gives it.</summary>
internal sealed record UnicodeValue(string ShortName, string LongName, IReadOnlyList<string> Names, List<UnicodeRange> Ranges);

/// <summary>
/// The Unicode Character Database, parsed from the verified archive into exactly what the tables
/// need, with the internal consistency checks that make a misread file a failure rather than a
/// quietly wrong table.
/// </summary>
/// <remarks>
/// Every derived property this reads from two places is compared across them: the General_Category
/// of <c>DerivedGeneralCategory.txt</c> against <c>UnicodeData.txt</c>, and the NFD and NFKD quick
/// checks of <c>DerivedNormalizationProps.txt</c> against the decompositions this computes. A parser
/// that dropped a line would pass neither.
/// </remarks>
internal sealed class UnicodeDatabase
{
    internal const int HangulSBase = 0xAC00;
    internal const int HangulLBase = 0x1100;
    internal const int HangulVBase = 0x1161;
    internal const int HangulTBase = 0x11A7;
    internal const int HangulVCount = 21;
    internal const int HangulTCount = 28;
    internal const int HangulNCount = HangulVCount * HangulTCount;
    internal const int HangulSCount = 19 * HangulNCount;

    private const string Ucd = "ucd-17.0.0/";

    /// <summary>General_Category values (the 30) and groups (the 8), in set-id order.</summary>
    internal List<UnicodeValue> GeneralCategories { get; } = [];

    /// <summary>The number of the above that are single values rather than groups.</summary>
    internal int GeneralCategoryValueCount { get; private set; }

    /// <summary>The binary properties the language admits, in set-id order: canonical name, aliases, ranges.</summary>
    internal List<UnicodeValue> BinaryProperties { get; } = [];

    /// <summary>Script values in id order: Script ranges.</summary>
    internal List<UnicodeValue> Scripts { get; } = [];

    /// <summary>Script_Extensions ranges, by the same ids as <see cref="Scripts"/>.</summary>
    internal List<List<UnicodeRange>> ScriptExtensions { get; } = [];

    /// <summary>The non-binary property names: General_Category (0), Script (1), Script_Extensions (2).</summary>
    internal List<(string Name, int Kind)> NonBinaryNames { get; } = [];

    /// <summary>Canonical_Combining_Class per code point.</summary>
    internal int[] CombiningClass { get; } = new int[UnicodeDataFile.MaxCodePoint + 1];

    /// <summary>Each code point's full canonical decomposition, where it has one (Hangul excluded).</summary>
    internal SortedDictionary<int, int[]> CanonicalDecomposition { get; } = [];

    /// <summary>Each code point whose full compatibility decomposition differs from its full canonical one.</summary>
    internal SortedDictionary<int, int[]> CompatibilityDecomposition { get; } = [];

    /// <summary>Primary composites: (first, second) to composite, Full_Composition_Exclusion honoured, Hangul excluded.</summary>
    internal SortedDictionary<(int First, int Second), int> Compositions { get; } = [];

    /// <summary>NFC_QC=No, NFC_QC=Maybe, NFKC_QC=No, NFKC_QC=Maybe, in that order.</summary>
    internal List<List<UnicodeRange>> QuickChecks { get; } = [];

    /// <summary>CaseFolding.txt status C and S: source to target.</summary>
    internal SortedDictionary<int, int> SimpleFolding { get; } = [];

    /// <summary>UnicodeData.txt's Simple_Uppercase_Mapping (field 12), where a code point has one.</summary>
    internal SortedDictionary<int, int> SimpleUppercase { get; } = [];

    /// <summary>Parses and cross-checks the verified archive.</summary>
    internal static UnicodeDatabase Read(IReadOnlyDictionary<string, byte[]> archive)
    {
        var database = new UnicodeDatabase();
        string Text(string name) => UnicodePin.Decode(archive[name]);

        // A spec table's name in the pin is its repository path.
        var nonBinary = UnicodeSpecTables.Rows(Text(UnicodeSpecTables.NonBinary));
        var binary = UnicodeSpecTables.Rows(Text(UnicodeSpecTables.Binary));
        var strings = UnicodeSpecTables.Rows(Text(UnicodeSpecTables.Strings));
        var disagreements = UnicodeSpecTables.Disagreements(nonBinary, binary, strings, Text(Ucd + "PropertyAliases.txt")).ToArray();

        if (disagreements.Length > 0)
        {
            throw new InvalidDataException(
                "The ECMAScript property tables and PropertyAliases.txt disagree:\n  " + string.Join("\n  ", disagreements));
        }

        var unicodeData = Text(Ucd + "UnicodeData.txt");
        database.ReadGeneralCategories(Text(Ucd + "PropertyValueAliases.txt"), Text(Ucd + "extracted/DerivedGeneralCategory.txt"), unicodeData);

        database.ReadNonBinaryNames(nonBinary);
        database.ReadBinaryProperties(binary, archive);
        database.ReadScripts(Text(Ucd + "PropertyValueAliases.txt"), Text(Ucd + "Scripts.txt"), Text(Ucd + "ScriptExtensions.txt"));
        database.ReadNormalization(unicodeData, Text(Ucd + "DerivedNormalizationProps.txt"));
        database.ReadCaseFolding(Text(Ucd + "CaseFolding.txt"));
        database.ReadSimpleUppercase(unicodeData);

        return database;
    }

    private void ReadGeneralCategories(string valueAliases, string derived, string unicodeData)
    {
        var values = new List<(string[] Fields, string Comment)>();

        foreach (var (fields, comment) in UnicodeDataFile.RecordsWithComments(valueAliases))
        {
            if (fields[0] == "gc")
            {
                values.Add((fields, comment));
            }
        }

        // The single values are the two-letter short names, in short-name order; the groups are the
        // rest, and PropertyValueAliases.txt states each group's members in its comment.
        var singles = values.Where(static value => value.Fields[1].Length == 2 && value.Comment.Length == 0)
            .OrderBy(static value => value.Fields[1], StringComparer.Ordinal).ToArray();
        var groups = values.Where(static value => value.Comment.Length > 0)
            .OrderBy(static value => value.Fields[1], StringComparer.Ordinal).ToArray();

        if (singles.Length + groups.Length != values.Count || singles.Length != 30 || groups.Length != 8)
        {
            throw new InvalidDataException($"PropertyValueAliases.txt gives {singles.Length} single General_Category values and {groups.Length} groups, not 30 and 8");
        }

        var index = singles.Select(static (value, id) => (value.Fields[1], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var unassigned = index["Cn"];
        var valueOf = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(valueOf, unassigned);

        foreach (var fields in UnicodeDataFile.Records(derived))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                valueOf[codePoint] = index[fields[1]];
            }
        }

        // UnicodeData.txt says the same thing, <..., First>/<..., Last> pairs included, and a
        // code point it does not list is Cn.
        var fromUnicodeData = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(fromUnicodeData, unassigned);

        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                fromUnicodeData[codePoint] = index[fields[2]];
            }
        }

        var differing = Enumerable.Range(0, valueOf.Length).FirstOrDefault(codePoint => valueOf[codePoint] != fromUnicodeData[codePoint], -1);

        if (differing >= 0)
        {
            throw new InvalidDataException($"U+{differing:X4} has General_Category {singles[valueOf[differing]].Fields[1]} in DerivedGeneralCategory.txt and {singles[fromUnicodeData[differing]].Fields[1]} in UnicodeData.txt");
        }

        var runs = UnicodeDataFile.Runs(valueOf);

        foreach (var (fields, _) in singles)
        {
            var id = index[fields[1]];

            GeneralCategories.Add(new UnicodeValue(fields[1], fields[2], fields.Skip(1).ToArray(), runs.TryGetValue(id, out var ranges) ? ranges : []));
        }

        GeneralCategoryValueCount = GeneralCategories.Count;

        foreach (var (fields, comment) in groups)
        {
            var members = comment.Split('|', StringSplitOptions.TrimEntries);

            if (fields[1].Length == 1 && !members.Order(StringComparer.Ordinal).SequenceEqual(
                    index.Keys.Where(key => key[0] == fields[1][0]).Order(StringComparer.Ordinal), StringComparer.Ordinal))
            {
                throw new InvalidDataException($"General_Category group {fields[1]} lists {comment}, which is not every value starting {fields[1]}");
            }

            GeneralCategories.Add(new UnicodeValue(
                fields[1],
                fields[2],
                fields.Skip(1).ToArray(),
                UnicodeDataFile.Normalize(members.SelectMany(member => GeneralCategories[index[member]].Ranges))));
        }
    }

    private void ReadNonBinaryNames(IReadOnlyList<UnicodeSpecName> nonBinary)
    {
        string[] canonical = ["General_Category", "Script", "Script_Extensions"];

        foreach (var row in nonBinary)
        {
            var kind = Array.IndexOf(canonical, row.Canonical);

            if (kind < 0)
            {
                throw new InvalidDataException($"the non-binary property table names `{row.Canonical}`, which no table here is generated for");
            }

            NonBinaryNames.Add((row.Name, kind));
        }

        if (NonBinaryNames.Select(static name => name.Kind).Distinct().Count() != 3)
        {
            throw new InvalidDataException("the non-binary property table does not name all three of General_Category, Script and Script_Extensions");
        }
    }

    private void ReadBinaryProperties(IReadOnlyList<UnicodeSpecName> binary, IReadOnlyDictionary<string, byte[]> archive)
    {
        string[] sources =
        [
            Ucd + "PropList.txt",
            Ucd + "DerivedCoreProperties.txt",
            Ucd + "extracted/DerivedBinaryProperties.txt",
            Ucd + "emoji/emoji-data.txt",
            Ucd + "DerivedNormalizationProps.txt",
        ];

        // Every two-field line of every source, by property name and by the file that states it.
        var stated = new Dictionary<string, (string File, List<UnicodeRange> Ranges)>(StringComparer.Ordinal);

        foreach (var source in sources)
        {
            foreach (var fields in UnicodeDataFile.Records(UnicodePin.Decode(archive[source])))
            {
                if (fields.Length != 2)
                {
                    continue;
                }

                if (stated.TryGetValue(fields[1], out var found) && found.File != source)
                {
                    throw new InvalidDataException($"`{fields[1]}` is stated by both {found.File} and {source}");
                }

                if (found.Ranges is null)
                {
                    stated[fields[1]] = found = (source, []);
                }

                found.Ranges.Add(UnicodeDataFile.Range(fields[0]));
            }
        }

        var unassigned = GeneralCategories.Single(static value => value.ShortName == "Cn").Ranges;

        foreach (var group in binary.GroupBy(static row => row.Canonical, StringComparer.Ordinal).OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            var ranges = group.Key switch
            {
                "Any" => [new UnicodeRange(0, UnicodeDataFile.MaxCodePoint)],
                "ASCII" => [new UnicodeRange(0, 0x7F)],
                "Assigned" => UnicodeDataFile.Complement(unassigned),
                _ => stated.TryGetValue(group.Key, out var found)
                    ? UnicodeDataFile.Normalize(found.Ranges)
                    : throw new InvalidDataException($"the language admits `{group.Key}`, and none of the pinned property files states it"),
            };

            var names = group.Select(static row => row.Name).ToArray();

            BinaryProperties.Add(new UnicodeValue(group.Key, group.Key, names, ranges));
        }

        CheckIdentifierProperties(stated);

        if (BinaryProperties.Count != 53)
        {
            throw new InvalidDataException($"the binary property table admits {BinaryProperties.Count} properties, and JSD-0031 section 7 names 53");
        }
    }

    /// <summary>
    /// Holds DerivedCoreProperties.txt's <c>ID_Start</c> and <c>ID_Continue</c>, which the tokenizer
    /// and the group-name parser read, to their definitions in UAX #31 over the other pinned files.
    /// </summary>
    /// <remarks>
    /// <c>ID_Start</c> is the letters and <c>Nl</c> plus <c>Other_ID_Start</c>, minus
    /// <c>Pattern_Syntax</c> and <c>Pattern_White_Space</c>; <c>ID_Continue</c> adds <c>Mn</c>,
    /// <c>Mc</c>, <c>Nd</c>, <c>Pc</c> and <c>Other_ID_Continue</c> under the same subtraction. A
    /// misread line in any of the three files would make the two computations part.
    /// </remarks>
    private void CheckIdentifierProperties(Dictionary<string, (string File, List<UnicodeRange> Ranges)> stated)
    {
        var category = GeneralCategories.Take(GeneralCategoryValueCount).ToDictionary(static value => value.ShortName, static value => value.Ranges, StringComparer.Ordinal);

        List<UnicodeRange> Stated(string name) =>
            stated.TryGetValue(name, out var found) ? found.Ranges : throw new InvalidDataException($"no pinned file states `{name}`");

        List<UnicodeRange> Derive(IEnumerable<List<UnicodeRange>> parts)
        {
            var member = new bool[UnicodeDataFile.MaxCodePoint + 1];

            foreach (var range in parts.SelectMany(static part => part))
            {
                Array.Fill(member, true, range.First, range.Last - range.First + 1);
            }

            foreach (var range in Stated("Pattern_Syntax").Concat(Stated("Pattern_White_Space")))
            {
                Array.Fill(member, false, range.First, range.Last - range.First + 1);
            }

            return UnicodeDataFile.Normalize(Enumerable.Range(0, member.Length).Where(codePoint => member[codePoint]).Select(static codePoint => new UnicodeRange(codePoint, codePoint)));
        }

        var start = Derive(new[] { "Lu", "Ll", "Lt", "Lm", "Lo", "Nl" }.Select(name => category[name]).Append(Stated("Other_ID_Start")));
        var part = Derive(new[] { "Lu", "Ll", "Lt", "Lm", "Lo", "Nl", "Mn", "Mc", "Nd", "Pc" }.Select(name => category[name]).Append(Stated("Other_ID_Start")).Append(Stated("Other_ID_Continue")));

        if (!start.SequenceEqual(BinaryProperties.Single(static value => value.ShortName == "ID_Start").Ranges))
        {
            throw new InvalidDataException("DerivedCoreProperties.txt's ID_Start is not UAX #31's derivation over UnicodeData.txt and PropList.txt");
        }

        if (!part.SequenceEqual(BinaryProperties.Single(static value => value.ShortName == "ID_Continue").Ranges))
        {
            throw new InvalidDataException("DerivedCoreProperties.txt's ID_Continue is not UAX #31's derivation over UnicodeData.txt and PropList.txt");
        }
    }

    /// <summary>
    /// Reads UnicodeData.txt's simple upper-case field, which the non-<c>u</c> Canonicalize reads,
    /// and holds it to <c>Changes_When_Uppercased</c>: every code point with a simple upper case
    /// other than itself must be one that upper-casing changes.
    /// </summary>
    private void ReadSimpleUppercase(string unicodeData)
    {
        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            if (fields[12].Length == 0)
            {
                continue;
            }

            if (first != last)
            {
                throw new InvalidDataException($"UnicodeData.txt gives the range {fields[0]} a simple upper case");
            }

            var upper = UnicodeDataFile.Hex(fields[12]);

            if (upper == first || !SimpleUppercase.TryAdd(first, upper))
            {
                throw new InvalidDataException($"UnicodeData.txt gives {fields[0]} a simple upper case that is itself, or two of them");
            }
        }

        var changes = BinaryProperties.Single(static value => value.ShortName == "Changes_When_Uppercased").Ranges;

        foreach (var source in SimpleUppercase.Keys.Where(source => !changes.Any(range => range.First <= source && source <= range.Last)))
        {
            throw new InvalidDataException($"U+{source:X4} has a simple upper case in UnicodeData.txt and is not Changes_When_Uppercased");
        }
    }

    private void ReadScripts(string valueAliases, string scripts, string extensions)
    {
        var values = UnicodeDataFile.Records(valueAliases)
            .Where(static fields => fields[0] == "sc")
            .OrderBy(static fields => fields[1], StringComparer.Ordinal)
            .ToArray();

        var byLong = values.Select(static (fields, id) => (fields[2], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var byShort = values.Select(static (fields, id) => (fields[1], id)).ToDictionary(static pair => pair.Item1, static pair => pair.id, StringComparer.Ordinal);
        var unknown = byShort["Zzzz"];
        var scriptOf = new int[UnicodeDataFile.MaxCodePoint + 1];

        Array.Fill(scriptOf, unknown);

        foreach (var fields in UnicodeDataFile.Records(scripts))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                scriptOf[codePoint] = byLong[fields[1]];
            }
        }

        var runs = UnicodeDataFile.Runs(scriptOf);

        foreach (var fields in values)
        {
            var id = byShort[fields[1]];

            Scripts.Add(new UnicodeValue(fields[1], fields[2], fields.Skip(1).ToArray(), runs.TryGetValue(id, out var ranges) ? ranges : []));
        }

        // Script_Extensions: a listed code point has exactly the listed scripts, which need not
        // include its Script; an unlisted one has its Script alone.
        var extended = new List<UnicodeRange>[values.Length];
        var listed = new bool[UnicodeDataFile.MaxCodePoint + 1];

        for (var id = 0; id < extended.Length; id++)
        {
            extended[id] = [];
        }

        foreach (var fields in UnicodeDataFile.Records(extensions))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
            {
                if (listed[codePoint])
                {
                    throw new InvalidDataException($"ScriptExtensions.txt lists U+{codePoint:X4} twice");
                }

                listed[codePoint] = true;
            }

            foreach (var script in fields[1].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                extended[byShort[script]].Add(range);
            }
        }

        for (var id = 0; id < extended.Length; id++)
        {
            foreach (var range in Scripts[id].Ranges)
            {
                var start = -1;

                for (var codePoint = range.First; codePoint <= range.Last + 1; codePoint++)
                {
                    var member = codePoint <= range.Last && !listed[codePoint];

                    if (member && start < 0)
                    {
                        start = codePoint;
                    }
                    else if (!member && start >= 0)
                    {
                        extended[id].Add(new UnicodeRange(start, codePoint - 1));
                        start = -1;
                    }
                }
            }

            ScriptExtensions.Add(UnicodeDataFile.Normalize(extended[id]));
        }
    }

    private void ReadNormalization(string unicodeData, string derived)
    {
        var canonical = new Dictionary<int, int[]>();
        var compatibility = new Dictionary<int, int[]>();

        foreach (var (first, last, fields) in UnicodeDataEntries(unicodeData))
        {
            var combining = int.Parse(fields[3], CultureInfo.InvariantCulture);

            for (var codePoint = first; codePoint <= last; codePoint++)
            {
                CombiningClass[codePoint] = combining;
            }

            if (fields[5].Length == 0)
            {
                continue;
            }

            if (first != last)
            {
                throw new InvalidDataException($"the UnicodeData.txt range at U+{first:X4} carries a decomposition");
            }

            var parts = fields[5].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var tagged = parts[0][0] == '<';
            var mapping = parts.Skip(tagged ? 1 : 0).Select(UnicodeDataFile.Hex).ToArray();

            (tagged ? compatibility : canonical)[first] = mapping;
        }

        var exclusions = new HashSet<int>();
        var quickChecks = new Dictionary<string, List<UnicodeRange>>(StringComparer.Ordinal);

        foreach (var fields in UnicodeDataFile.Records(derived))
        {
            var range = UnicodeDataFile.Range(fields[0]);

            if (fields.Length == 2 && fields[1] == "Full_Composition_Exclusion")
            {
                for (var codePoint = range.First; codePoint <= range.Last; codePoint++)
                {
                    exclusions.Add(codePoint);
                }
            }
            else if (fields.Length == 3 && fields[1].EndsWith("_QC", StringComparison.Ordinal))
            {
                var key = $"{fields[1]}={fields[2]}";

                if (!quickChecks.TryGetValue(key, out var list))
                {
                    quickChecks[key] = list = [];
                }

                list.Add(range);
            }
        }

        int[] Full(int codePoint, bool compat)
        {
            if (codePoint >= HangulSBase && codePoint < HangulSBase + HangulSCount)
            {
                return Hangul(codePoint);
            }

            var mapping = canonical.TryGetValue(codePoint, out var found)
                ? found
                : compat && compatibility.TryGetValue(codePoint, out found) ? found : null;

            return mapping is null
                ? [codePoint]
                : mapping.SelectMany(part => Full(part, compat)).ToArray();
        }

        foreach (var codePoint in canonical.Keys.Concat(compatibility.Keys).Order())
        {
            var nfd = Full(codePoint, compat: false);
            var nfkd = Full(codePoint, compat: true);

            if (canonical.ContainsKey(codePoint))
            {
                CanonicalDecomposition[codePoint] = nfd;
            }

            if (!nfkd.SequenceEqual(nfd))
            {
                CompatibilityDecomposition[codePoint] = nfkd;
            }
        }

        foreach (var (codePoint, mapping) in canonical)
        {
            if (mapping.Length == 2 && !exclusions.Contains(codePoint))
            {
                Compositions.Add((mapping[0], mapping[1]), codePoint);
            }
        }

        // The quick checks the generator does not store are the ones it can compute; comparing
        // them with the file is what shows the decompositions above were read completely.
        var hangul = new UnicodeRange(HangulSBase, HangulSBase + HangulSCount - 1);

        Compare("NFD_QC=N", UnicodeDataFile.Normalize(CanonicalDecomposition.Keys.Select(static cp => new UnicodeRange(cp, cp)).Append(hangul)));
        Compare("NFKD_QC=N", UnicodeDataFile.Normalize(CanonicalDecomposition.Keys.Concat(CompatibilityDecomposition.Keys).Select(static cp => new UnicodeRange(cp, cp)).Append(hangul)));

        foreach (var key in new[] { "NFC_QC=N", "NFC_QC=M", "NFKC_QC=N", "NFKC_QC=M" })
        {
            QuickChecks.Add(UnicodeDataFile.Normalize(quickChecks.TryGetValue(key, out var list) ? list : throw new InvalidDataException($"DerivedNormalizationProps.txt states no {key}")));
        }

        void Compare(string key, List<UnicodeRange> computed)
        {
            if (!UnicodeDataFile.Normalize(quickChecks[key]).SequenceEqual(computed))
            {
                throw new InvalidDataException($"DerivedNormalizationProps.txt's {key} is not the set of code points the UnicodeData.txt decompositions change");
            }
        }
    }

    private void ReadCaseFolding(string text)
    {
        foreach (var fields in UnicodeDataFile.Records(text))
        {
            if (fields[1] is not ("C" or "S"))
            {
                continue;
            }

            var target = fields[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (target.Length != 1 || !SimpleFolding.TryAdd(UnicodeDataFile.Hex(fields[0]), UnicodeDataFile.Hex(target[0])))
            {
                throw new InvalidDataException($"CaseFolding.txt gives {fields[0]} a C or S mapping that is not one code point, or two of them");
            }
        }

        // Folding is idempotent: nothing a code point folds to folds again.
        foreach (var target in SimpleFolding.Values.Where(SimpleFolding.ContainsKey))
        {
            throw new InvalidDataException($"U+{target:X4} is a simple folding target and has a folding of its own");
        }
    }

    /// <summary>The full Hangul syllable decomposition, LV or LVT.</summary>
    internal static int[] Hangul(int syllable)
    {
        var index = syllable - HangulSBase;
        var leading = HangulLBase + (index / HangulNCount);
        var vowel = HangulVBase + (index % HangulNCount / HangulTCount);
        var trailing = HangulTBase + (index % HangulTCount);

        return trailing == HangulTBase ? [leading, vowel] : [leading, vowel, trailing];
    }

    /// <summary>
    /// <c>UnicodeData.txt</c>'s records, with each <c>&lt;..., First&gt;</c>/<c>&lt;..., Last&gt;</c>
    /// pair joined into one range.
    /// </summary>
    internal static IEnumerable<(int First, int Last, string[] Fields)> UnicodeDataEntries(string text)
    {
        string[]? opened = null;

        foreach (var raw in text.Split('\n'))
        {
            if (raw.Length == 0)
            {
                continue;
            }

            var fields = raw.Split(';');

            if (fields.Length != 15)
            {
                throw new InvalidDataException($"a UnicodeData.txt line has {fields.Length} fields, not 15: {raw}");
            }

            if (fields[1].EndsWith(", First>", StringComparison.Ordinal))
            {
                opened = fields;
                continue;
            }

            if (fields[1].EndsWith(", Last>", StringComparison.Ordinal))
            {
                if (opened is null)
                {
                    throw new InvalidDataException($"UnicodeData.txt closes a range it did not open: {raw}");
                }

                yield return (UnicodeDataFile.Hex(opened[0]), UnicodeDataFile.Hex(fields[0]), opened);
                opened = null;
                continue;
            }

            yield return (UnicodeDataFile.Hex(fields[0]), UnicodeDataFile.Hex(fields[0]), fields);
        }
    }
}
