using System.Globalization;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One generated table file: what is on disk, and what the generator says should be.</summary>
internal sealed record UnicodeArtefact(string RelativePath, string Current, string Desired)
{
    internal bool IsCurrent => string.Equals(Current, Desired, StringComparison.Ordinal);
}

/// <summary>One table member and the bytes of data it holds.</summary>
internal sealed record UnicodeTableSize(string File, string Member, int Bytes);

/// <summary>
/// What one generation produced: the table files, the size of every table in them, and the
/// JavaScript probes that carry <c>NormalizationTest.txt</c> to the end-user host.
/// </summary>
internal sealed record UnicodeGeneration(
    IReadOnlyList<UnicodeArtefact> Artefacts,
    IReadOnlyList<UnicodeTableSize> Tables,
    IReadOnlyList<UnicodeArtefact> Probes)
{
    internal int TotalBytes => Tables.Sum(static table => table.Bytes);
}

/// <summary>
/// The generator and the gate for the JavaScript profile's Unicode tables, as one function.
/// </summary>
/// <remarks>
/// <para>
/// <b>The shape is rule J5's.</b> Decision JSD-0031 section 5 puts the generator here, in the
/// architecture test project, following <see cref="AssuranceGenerator"/>: <see cref="Generate"/>
/// computes every table file from the verified archive and is pure; with
/// <c>BROILER_UNICODE_WRITE=1</c> the test writes what it computed, and without it the test asserts
/// the checked-in files are byte-identical to it. C# rather than a script, so the check runs inside
/// <c>dotnet test</c> with no interpreter and no network.
/// </para>
/// <para>
/// <b>Its inputs are the pin's files and nothing else</b>, handed over by
/// <see cref="UnicodePin.Verified"/> after their lengths, hashes and version headers have been
/// compared with the pin. <b>Its output is deterministic</b>: every table is sorted by code point
/// or by ordinal name, numbers are written with the invariant culture, lines end in LF, and nothing
/// records a time, a machine or a path outside the repository.
/// </para>
/// <para>
/// <b>It annotates its own output.</b> Each table member carries the per-unit
/// <c>EXEMPT=</c> hatch rule J3 accepts, the containing partial type carries an ordinary assessment,
/// and the text is then passed through <see cref="AssuranceGenerator.DesiredSource"/> - the function
/// the assurance gate itself uses - so the file header and the type's fingerprint are exactly what
/// J5 would write. A regenerated file therefore needs no hand edit and changes no assurance rule.
/// </para>
/// </remarks>
internal static class UnicodeTableGenerator
{
    /// <summary>Set to <c>1</c> to make the run write rather than assert.</summary>
    internal const string WriteVariable = "BROILER_UNICODE_WRITE";

    /// <summary>The owner's hard cap on table data, 2026-09-22: 300 KB, taken as 300 x 1024 bytes.</summary>
    internal const int BudgetBytes = 300 * 1024;

    internal const string PropertiesPath = "src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.g.cs";
    internal const string CaseFoldingPath = "src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeCaseFolding.g.cs";
    internal const string NormalizationPath = "src/Broiler.VM.Profile.JavaScript/JsUnicodeNormalization.g.cs";

    /// <summary>The generated files, in the order they are generated.</summary>
    internal static readonly string[] OutputPaths = [PropertiesPath, CaseFoldingPath, NormalizationPath];

    /// <summary>The per-unit exemption reason every generated table member states.</summary>
    internal const string ExemptReason =
        "Unicode 17.0.0 table data written by UnicodeTableGenerator from the files unicode.pin names, compared byte for byte by rule N22";

    internal static bool WriteRequested =>
        string.Equals(Environment.GetEnvironmentVariable(WriteVariable), "1", StringComparison.Ordinal);

    /// <summary>The generation for this checkout, from the archive on disk.</summary>
    internal static UnicodeGeneration Current => current.Value;

    private static readonly Lazy<UnicodeGeneration> current = new(static () =>
    {
        var pin = UnicodePin.Load();

        return Generate(pin, pin.ReadArchive(), path => File.Exists(Full(path)) ? File.ReadAllText(Full(path)) : string.Empty);
    });

    /// <summary>
    /// Every table file, from the pinned archive. Throws when the archive is not the pinned one or
    /// the data disagrees with itself; nothing is generated from input that failed a check.
    /// </summary>
    internal static UnicodeGeneration Generate(
        UnicodePin pin,
        IReadOnlyDictionary<string, byte[]> archive,
        Func<string, string> currentText)
    {
        var verified = pin.Verified(archive);
        var database = UnicodeDatabase.Read(verified);
        var tables = new List<UnicodeTableSize>();

        var artefacts = new[]
        {
            Emit(PropertiesPath, "Broiler.VM.Profile.JavaScript.Format", Properties(database), tables, currentText),
            Emit(CaseFoldingPath, "Broiler.VM.Profile.JavaScript.Format", CaseFolding(database), tables, currentText),
            Emit(NormalizationPath, "Broiler.VM.Profile.JavaScript", Normalization(database), tables, currentText),
        };

        // Slice U3's conformance probes: test files, not product source, so they carry no
        // assurance annotation and are written exactly as computed.
        var probes = UnicodeNormalizationProbes.Write(verified)
            .Select(probe => new UnicodeArtefact(probe.Path, currentText(probe.Path), probe.Text))
            .ToArray();

        return new UnicodeGeneration(artefacts, tables, probes);
    }

    /// <summary>Writes every table file and probe that is not current, and names the ones it changed.</summary>
    internal static IReadOnlyList<string> Apply(UnicodeGeneration generation)
    {
        var written = new List<string>();

        foreach (var artefact in generation.Artefacts.Concat(generation.Probes).Where(static artefact => !artefact.IsCurrent))
        {
            File.WriteAllText(Full(artefact.RelativePath), artefact.Desired, AssuranceSources.Utf8NoBom);
            written.Add(artefact.RelativePath);
        }

        return written;
    }

    /// <summary>Every artefact whose checked-in text is not what the generator writes, with where they part.</summary>
    internal static IReadOnlyList<string> Stale(IEnumerable<UnicodeArtefact> artefacts) =>
        artefacts.Where(static artefact => !artefact.IsCurrent).Select(static artefact =>
        {
            var current = artefact.Current.Split('\n');
            var desired = artefact.Desired.Split('\n');
            var line = Enumerable.Range(0, Math.Min(current.Length, desired.Length))
                .FirstOrDefault(index => !string.Equals(current[index], desired[index], StringComparison.Ordinal), Math.Min(current.Length, desired.Length));

            return $"{artefact.RelativePath} is not what the Unicode table generator writes: first difference at line {line + 1}. " +
                $"Change the generator or the pinned archive and run with {WriteVariable}=1; never edit the file.";
        }).ToList();

    private static string Full(string relativePath) =>
        Path.Combine(ComponentGraph.Root, relativePath.Replace('/', Path.DirectorySeparatorChar));

    // =============================================================================================
    // The Format assembly: property sets, names and case folding
    // =============================================================================================

    private static GeneratedType Properties(UnicodeDatabase database)
    {
        var sets = new List<List<UnicodeRange>>();

        sets.AddRange(database.GeneralCategories.Select(static value => value.Ranges));

        var binaryBase = sets.Count;

        sets.AddRange(database.BinaryProperties.Select(static value => value.Ranges));

        var scriptBase = sets.Count;

        sets.AddRange(database.Scripts.Select(static value => value.Ranges));

        var extensionsBase = sets.Count;

        sets.AddRange(database.ScriptExtensions);

        var ranges = new ByteWriter();
        var index = new ByteWriter();
        var rangeCount = 0;

        foreach (var set in sets)
        {
            index.Int24(rangeCount).Int24(set.Count);

            foreach (var range in set)
            {
                ranges.Int24(range.First).Int24(range.Last);
                rangeCount++;
            }
        }

        var lone = database.GeneralCategories.SelectMany(static (value, id) => value.Names.Select(name => (name, id)))
            .Concat(database.BinaryProperties.SelectMany((value, id) => value.Names.Select(name => (name, id: binaryBase + id))));
        var (loneNames, loneIndex) = NameTable(lone);
        var (propertyNames, propertyIndex) = NameTable(database.NonBinaryNames.Select(static name => (name.Name, name.Kind)));
        var (scriptNames, scriptIndex) = NameTable(database.Scripts.SelectMany(static (value, id) => value.Names.Select(name => (name, id))));

        return new GeneratedType(
            "JsUnicodeProperties",
            "The Unicode 17.0.0 code point sets and the names that select them, for the matcher's property escapes.",
            [
                Constant("string", "UnicodeVersion", $"\"{UnicodePin.PinnedVersion}\"", "The Unicode version every table here was generated from."),
                Constant("int", "GeneralCategorySetCount", Number(database.GeneralCategories.Count), $"Set ids below this are General_Category: the {database.GeneralCategoryValueCount} values, then the groups."),
                Constant("int", "BinarySetBase", Number(binaryBase), $"The first of the {database.BinaryProperties.Count} binary property sets, in ordinal order of canonical name."),
                Constant("int", "ScriptSetBase", Number(scriptBase), "The first Script set; a script's id is its offset from here, in ordinal order of short name."),
                Constant("int", "ScriptExtensionsSetBase", Number(extensionsBase), "The first Script_Extensions set, by the same script ids."),
                Constant("int", "ScriptCount", Number(database.Scripts.Count), "The number of Script values, and of Script_Extensions sets."),
                Constant("int", "IdStartSet", Number(BinarySet(database, binaryBase, "ID_Start")), "The ID_Start set, which the tokenizer and group names read through JsUnicodeLexical."),
                Constant("int", "IdContinueSet", Number(BinarySet(database, binaryBase, "ID_Continue")), "The ID_Continue set, which the tokenizer and group names read through JsUnicodeLexical."),
                Constant("int", "SpaceSeparatorSet", Number(database.GeneralCategories.FindIndex(static value => value.ShortName == "Zs")), "The General_Category=Space_Separator (Zs) set, the WhiteSpace production's USP."),
                Table("RangeData", ranges, "Every set's ranges: first and last code point, inclusive, three bytes each, little-endian. Within a set they ascend and neither overlap nor touch."),
                Table("SetData", index, "Per set id: the index of its first range in RangeData and its range count, three bytes each."),
                Table("LoneNames", loneNames, "Every name the lone form admits - General_Category values and aliases, binary properties and aliases - in ordinal order: a length byte, the ASCII name, and the set id in two bytes."),
                Table("LoneNameIndex", loneIndex, "The offset of each LoneNames entry, two bytes each, in the same order."),
                Table("PropertyNames", propertyNames, "The names the name=value form admits, with 0 for General_Category, 1 for Script and 2 for Script_Extensions."),
                Table("PropertyNameIndex", propertyIndex, "The offset of each PropertyNames entry."),
                Table("ScriptNames", scriptNames, "Every Script value name and alias from PropertyValueAliases.txt, with its script id."),
                Table("ScriptNameIndex", scriptIndex, "The offset of each ScriptNames entry."),
            ]);
    }

    /// <summary>The set id of a binary property by canonical name; a name the table lacks is a stop.</summary>
    private static int BinarySet(UnicodeDatabase database, int binaryBase, string name)
    {
        var index = database.BinaryProperties.FindIndex(value => value.ShortName == name);

        return index < 0 ? throw new InvalidDataException($"the binary property table has no `{name}`") : binaryBase + index;
    }

    /// <summary>
    /// The non-<c>u</c> Canonicalize of every code unit it moves: UnicodeData.txt's simple upper
    /// case, kept only where the language keeps it.
    /// </summary>
    /// <remarks>
    /// ES2026 Canonicalize without <c>u</c> or <c>v</c> upper-cases one code unit and returns the
    /// code unit unchanged when the result is not exactly one code unit, or when it would map a
    /// non-ASCII code unit to ASCII. A surrogate code unit has no mapping. A simple mapping into
    /// the supplementary planes (none exists in 17.0.0) would be more than one code unit and is
    /// dropped by the same rule.
    /// </remarks>
    private static SortedDictionary<int, int> NonUnicodeCanonical(UnicodeDatabase database)
    {
        var canonical = new SortedDictionary<int, int>();

        foreach (var (source, upper) in database.SimpleUppercase)
        {
            if (source <= 0xFFFF && source is < 0xD800 or > 0xDFFF && upper <= 0xFFFF && !(source >= 128 && upper < 128))
            {
                canonical.Add(source, upper);
            }
        }

        // The matcher closes a class by the reverse relation, which assumes one step reaches the
        // canonical form: no target may be moved again.
        foreach (var target in canonical.Values.Where(canonical.ContainsKey))
        {
            throw new InvalidDataException($"U+{target:X4} is a non-u canonical form and is moved again");
        }

        return canonical;
    }

    private static GeneratedType CaseFolding(UnicodeDatabase database)
    {
        var folds = database.SimpleFolding.ToArray();
        var data = new ByteWriter();

        foreach (var (source, target) in folds)
        {
            data.Int24(source).Int24(target);
        }

        var orbit = new ByteWriter();

        foreach (var entry in folds.Select(static (fold, index) => (fold.Value, fold.Key, index)).Order())
        {
            orbit.UInt16(entry.index);
        }

        // The largest set of code points sharing one folding, the target itself included.
        var maxOrbit = folds.GroupBy(static fold => fold.Value).Max(static group => group.Count() + 1);

        var uppers = NonUnicodeCanonical(database).ToArray();
        var upperData = new ByteWriter();

        foreach (var (source, target) in uppers)
        {
            upperData.UInt16(source).UInt16(target);
        }

        var upperOrbit = new ByteWriter();

        foreach (var entry in uppers.Select(static (upper, index) => (upper.Value, upper.Key, index)).Order())
        {
            upperOrbit.UInt16(entry.index);
        }

        var maxUpperOrbit = uppers.GroupBy(static upper => upper.Value).Max(static group => group.Count() + 1);

        return new GeneratedType(
            "JsUnicodeCaseFolding",
            "The two Canonicalize mappings: CaseFolding.txt's simple foldings (status C and S) under the u flag, and UnicodeData.txt's simple upper case of one code unit without it.",
            [
                Constant("int", "MaxOrbit", Number(maxOrbit), "The most code points that share one simple folding, the folding itself included."),
                Constant("int", "MaxUpperOrbit", Number(maxUpperOrbit), "The most code units that share one non-u canonical form, that form itself included."),
                Table("FoldData", data, "Every code point with a C or S folding and its folding, three bytes each, in code point order."),
                Table("OrbitIndex", orbit, "The FoldData entries ordered by folding and then by code point, as two-byte entry numbers."),
                Table("UpperData", upperData, "Every code unit the non-u Canonicalize moves and the code unit it moves it to, two bytes each, in code unit order: the simple upper case, less mappings to ASCII from outside it."),
                Table("UpperOrbitIndex", upperOrbit, "The UpperData entries ordered by canonical form and then by code unit, as two-byte entry numbers."),
            ]);
    }

    // =============================================================================================
    // The profile assembly: normalization
    // =============================================================================================

    private static GeneratedType Normalization(UnicodeDatabase database)
    {
        var combining = new ByteWriter();
        var start = 0;

        for (var codePoint = 1; codePoint <= database.CombiningClass.Length; codePoint++)
        {
            if (codePoint == database.CombiningClass.Length || database.CombiningClass[codePoint] != database.CombiningClass[start])
            {
                if (database.CombiningClass[start] != 0)
                {
                    combining.Int24(start).Int24(codePoint - 1).Byte(database.CombiningClass[start]);
                }

                start = codePoint;
            }
        }

        var pool = new ByteWriter();
        var poolLength = 0;

        ByteWriter Index(SortedDictionary<int, int[]> decompositions)
        {
            var index = new ByteWriter();

            foreach (var (codePoint, mapping) in decompositions)
            {
                index.Int24(codePoint).UInt16(poolLength).Byte(mapping.Length);

                foreach (var part in mapping)
                {
                    pool.Int24(part);
                }

                poolLength += mapping.Length;
            }

            return index;
        }

        var canonical = Index(database.CanonicalDecomposition);
        var compatibility = Index(database.CompatibilityDecomposition);
        var compositions = new ByteWriter();

        foreach (var ((first, second), composite) in database.Compositions)
        {
            compositions.Int24(first).Int24(second).Int24(composite);
        }

        var quick = new ByteWriter();
        var quickIndex = new ByteWriter();
        var quickCount = 0;

        foreach (var set in database.QuickChecks)
        {
            quickIndex.UInt16(quickCount).UInt16(set.Count);

            foreach (var range in set)
            {
                quick.Int24(range.First).Int24(range.Last);
                quickCount++;
            }
        }

        var longest = database.CanonicalDecomposition.Values.Concat(database.CompatibilityDecomposition.Values).Max(static mapping => mapping.Length);

        return new GeneratedType(
            "JsUnicodeNormalization",
            "The Unicode 17.0.0 normalization data behind String.prototype.normalize: combining classes, full decompositions, primary composites and the composed-form quick checks.",
            [
                Constant("int", "MaxDecompositionLength", Number(longest), "The longest full decomposition, in code points; a Hangul syllable's is at most three."),
                Table("CombiningClassData", combining, "Every run of one non-zero Canonical_Combining_Class: first and last code point, three bytes each, and the class in one byte."),
                Table("CanonicalIndex", canonical, "Every code point with a canonical decomposition, Hangul excluded: the code point in three bytes, its offset in DecompositionPool in two and its length in one. The decomposition is full: applied recursively, not reordered."),
                Table("CompatibilityIndex", compatibility, "Every code point whose full compatibility decomposition differs from its full canonical one, in CanonicalIndex's format."),
                Table("DecompositionPool", pool, "The code points of every full decomposition, three bytes each."),
                Table("CompositionData", compositions, "Every primary composite, Full_Composition_Exclusion and Hangul excluded: first, second and composite code point, three bytes each, ordered by first and then second."),
                Table("QuickCheckData", quick, "The ranges of NFC_QC=No, NFC_QC=Maybe, NFKC_QC=No and NFKC_QC=Maybe, in RangeData's format."),
                Table("QuickCheckIndex", quickIndex, "For each of those four sets: its first range and its range count, two bytes each."),
            ]);
    }

    // =============================================================================================
    // Encoding
    // =============================================================================================

    /// <summary>A name table in ordinal order: a length byte, the ASCII name and a two-byte value, plus two-byte offsets.</summary>
    private static (ByteWriter Names, ByteWriter Index) NameTable(IEnumerable<(string Name, int Value)> entries)
    {
        var names = new ByteWriter();
        var index = new ByteWriter();
        // PropertyValueAliases.txt writes a value whose short and long names coincide twice
        // (`Ahom ; Ahom`); the same name for the same value is one entry, a name for two is a stop.
        var ordered = entries.Distinct().OrderBy(static entry => entry.Name, StringComparer.Ordinal).ToArray();

        foreach (var duplicate in ordered.GroupBy(static entry => entry.Name, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            throw new InvalidDataException($"the name `{duplicate.Key}` selects more than one table entry");
        }

        foreach (var (name, value) in ordered)
        {
            if (name.Length is 0 or > 255 || name.Any(static character => character is < '!' or > '~'))
            {
                throw new InvalidDataException($"the name `{name}` is not 1 to 255 printable ASCII characters");
            }

            index.UInt16(names.Count);
            names.Byte(name.Length);

            foreach (var character in name)
            {
                names.Byte(character);
            }

            names.UInt16(value);
        }

        return (names, index);
    }

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static Member Constant(string type, string name, string value, string summary) =>
        new(name, summary, $"internal const {type} {name} = {value};", null);

    private static Member Table(string name, ByteWriter data, string summary) =>
        new(name, summary, $"private static ReadOnlySpan<byte> {name} => new byte[]", data);

    private sealed record Member(string Name, string Summary, string Declaration, ByteWriter? Data);

    private sealed record GeneratedType(string Name, string Summary, IReadOnlyList<Member> Members);

    private static UnicodeArtefact Emit(
        string relativePath,
        string ns,
        GeneratedType type,
        List<UnicodeTableSize> sizes,
        Func<string, string> currentText)
    {
        var text = new StringBuilder();

        text.Append("using System;\n\n");
        text.Append("namespace ").Append(ns).Append(";\n\n");
        text.Append("// Written by UnicodeTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the Unicode\n");
        text.Append("// Character Database 17.0.0 files that src/tests/unicode/pins/unicode.pin names, under decision\n");
        text.Append("// JSD-0031. Rule N22 regenerates this file and compares it byte for byte, so a change belongs in\n");
        text.Append("// the generator or the pinned archive and never here: run the architecture tests with\n");
        text.Append("// BROILER_UNICODE_WRITE=1.\n");
        text.Append("//\n");
        text.Append("// The table data is derived from Unicode data files and is subject to the Unicode License v3\n");
        text.Append("// (SPDX Unicode-3.0), whose text THIRD_PARTY_NOTICES.md carries. The SPDX lines above are written\n");
        text.Append("// on every product file by the code assurance generator and describe this file's code.\n\n");
        text.Append("/// <summary>").Append(type.Summary).Append("</summary>\n");
        text.Append("// Broiler-AI:           Origin=Derived; Spec=JSD-0031 s5; IP=Low; Security=Low; Resources=1; Fingerprint=TBF\n");
        text.Append("// Broiler-Human:        PENDING\n");
        text.Append("internal static partial class ").Append(type.Name).Append('\n');
        text.Append("{\n");

        for (var index = 0; index < type.Members.Count; index++)
        {
            var member = type.Members[index];

            if (index > 0)
            {
                text.Append('\n');
            }

            text.Append("    /// <summary>").Append(member.Summary).Append("</summary>\n");
            text.Append("    // Broiler-AI:           EXEMPT=").Append(ExemptReason).Append('\n');
            text.Append("    // Broiler-Human:        PENDING\n");
            text.Append("    ").Append(member.Declaration).Append('\n');

            if (member.Data is { } data)
            {
                sizes.Add(new UnicodeTableSize(relativePath, $"{type.Name}.{member.Name}", data.Count));
                data.Render(text);
            }
        }

        text.Append("}\n");

        var full = Full(relativePath);
        var assembly = relativePath.Split('/')[1];
        var raw = text.ToString();
        var file = new AssuranceSourceFile(full, relativePath, assembly, raw, "\n", AssuranceSources.Parse(raw, full));

        return new UnicodeArtefact(
            relativePath,
            currentText(relativePath),
            AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file)));
    }

    /// <summary>Little-endian bytes, bounds-checked as they are written.</summary>
    private sealed class ByteWriter
    {
        private readonly List<byte> bytes = [];

        internal int Count => bytes.Count;

        internal ByteWriter Byte(int value)
        {
            if (value is < 0 or > 0xFF)
            {
                throw new InvalidDataException($"{value} does not fit in a byte");
            }

            bytes.Add((byte)value);
            return this;
        }

        internal ByteWriter UInt16(int value)
        {
            if (value is < 0 or > 0xFFFF)
            {
                throw new InvalidDataException($"{value} does not fit in two bytes");
            }

            bytes.Add((byte)value);
            bytes.Add((byte)(value >> 8));
            return this;
        }

        internal ByteWriter Int24(int value)
        {
            if (value is < 0 or > 0xFFFFFF)
            {
                throw new InvalidDataException($"{value} does not fit in three bytes");
            }

            bytes.Add((byte)value);
            bytes.Add((byte)(value >> 8));
            bytes.Add((byte)(value >> 16));
            return this;
        }

        /// <summary>The initializer, at most 100 columns per line, in decimal.</summary>
        internal void Render(StringBuilder text)
        {
            text.Append("    {\n");

            var line = new StringBuilder("       ");

            foreach (var value in bytes)
            {
                var item = value.ToString(CultureInfo.InvariantCulture);

                if (line.Length + item.Length + 2 > 100)
                {
                    text.Append(line).Append('\n');
                    line.Clear().Append("       ");
                }

                line.Append(' ').Append(item).Append(',');
            }

            if (bytes.Count > 0)
            {
                text.Append(line).Append('\n');
            }

            text.Append("    };\n");
        }
    }
}
