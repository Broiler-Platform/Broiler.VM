using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group U: the universal bytecode's rules, over <c>Broiler.VM.Ubc</c> and every project that
/// references it.
/// </summary>
/// <remarks>
/// <para>
/// <b>The letter is the programme's.</b> The universal bytecode roadmap and ADR 0013 name these rules
/// U1 to U9 before any of them existed, and ADR 0003's twenty-second applied row cites U1 to U4 by those
/// names, so the register takes the letter the records already use rather than minting one they would
/// have to be corrected to. The one collision is recorded rather than hidden: the JavaScript profile's
/// Unicode milestone calls a work package "F07 U2", which is a clause label inside that profile's
/// JSeal plan and never a rule of this register, and every sentence in this component that names it
/// says "JSeal F07 U2" in full.
/// </para>
/// <para>
/// <b>Why a group of its own.</b> The universal bytecode is not a profile family - it holds format,
/// schema and mechanism and no language concept, and every family is written against it - so neither
/// group N nor group W reaches it, and its claims are not a family's claims. It is a second sink of the
/// shared graph beside <c>Broiler.VM.Binary</c> (U1), it exports no language's name and no family's
/// table (U2), the common family's rows are stated in one place (U4), and its public surface is frozen
/// in its own baseline (U9). U8, the diagnostic registry, is minted with the registry it holds.
/// </para>
/// </remarks>
internal static class UbcRules
{
    /// <summary>The universal bytecode assembly.</summary>
    internal const string UbcAssembly = "Broiler.VM.Ubc";

    /// <summary>The exact Broiler.VM-owned reference set the universal bytecode may have.</summary>
    /// <remarks>
    /// The two core sinks, and no third name. ADR 0013 printed these two edges before the assembly
    /// existed and ADR 0001's revision of 2026-09-25 declared them, so a third edge is a change to both
    /// records rather than to a project file.
    /// </remarks>
    internal static readonly string[] UbcReferences =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary"];

    /// <summary>The one file allowed to state a common row's width, operand shape or effect.</summary>
    internal const string CommonTableFile = "src/Broiler.VM.Ubc/UbcOpcodes.cs";

    /// <summary>The file that holds the operand shapes and the slot types.</summary>
    internal const string SlotTypesFile = "src/Broiler.VM.Ubc/UbcSlotTypes.cs";

    /// <summary>The concept whose Appendix A is the common family's specification.</summary>
    internal const string ConceptFile = "docs/universal-bytecode.md";

    /// <summary>The banned vocabulary, read by rule U2 and by <c>eng/ubc-vocabulary-scan.py</c>.</summary>
    internal const string VocabularyFile = "docs/ubc/banned-vocabulary.txt";

    // =============================================================================================
    // U1 - the reference set and the project's shape
    // =============================================================================================

    /// <summary>
    /// U1: the universal bytecode references exactly Abstractions and Binary, declares no
    /// PackageReference, opens its internals to nobody, carries the literal element IsPackable false
    /// and no PackageId, and does not set AllowUnsafeBlocks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Six claims, each reported in its own words even where the set comparison already implies one,
    /// so the message a reader gets names the property that broke rather than reporting an edge to the
    /// runtime in the same words as a typo.
    /// </para>
    /// <para>
    /// <b>The unsafe clause reads the project file's elements, not its text.</b> The project file's own
    /// comment says it names no pointer type "(no AllowUnsafeBlocks)", so a text search would fire on
    /// the sentence that promises the property. An element whose value is not literally
    /// <c>false</c> - <c>true</c>, a property reference, or a conditional definition - is reported,
    /// because the rule cannot show any of them is off.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> U1(ComponentGraph.ProjectFile project)
    {
        if (!string.Equals(project.AssemblyName, UbcAssembly, StringComparison.Ordinal))
        {
            yield break;
        }

        var referenced = project.ReferencedAssemblyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!referenced.SequenceEqual(UbcReferences, StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references [{string.Join(", ", referenced)}] rather than " +
                $"[{string.Join(", ", UbcReferences)}]";
        }

        foreach (var package in project.PackageReferences)
        {
            yield return $"{project.RelativePath} declares PackageReference {package}";
        }

        foreach (var target in project.InternalsVisibleTo)
        {
            yield return $"{project.RelativePath} opens internals to {target}";
        }

        if (!project.RawText.Contains("<IsPackable>false</IsPackable>", StringComparison.Ordinal))
        {
            yield return $"{project.RelativePath} does not carry the literal <IsPackable>false</IsPackable>";
        }

        if (project.PackageId is not null)
        {
            yield return $"{project.RelativePath} declares PackageId {project.PackageId}";
        }

        foreach (var element in XDocument.Parse(project.RawText).Descendants("AllowUnsafeBlocks"))
        {
            if (element.Attribute("Condition") is not null)
            {
                yield return
                    $"{project.RelativePath} sets AllowUnsafeBlocks under a condition, which this rule " +
                    "cannot evaluate and so cannot show is off";
            }
            else if (!string.Equals(element.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase))
            {
                yield return
                    $"{project.RelativePath} sets AllowUnsafeBlocks to {element.Value.Trim()}, and the " +
                    "universal bytecode names no pointer type";
            }
        }
    }

    // =============================================================================================
    // U2 - no banned identifier and no family row
    // =============================================================================================

    /// <summary>One identifier the assembly exports, with what it names and where.</summary>
    internal sealed record ExportedIdentifier(string Kind, string Where, string Name);

    /// <summary>One row of the banned vocabulary: a term, lowered, and the kind of match it takes.</summary>
    internal sealed record VocabularyTerm(string Term, string Kind);

    /// <summary>The three kinds a vocabulary row may name, in the script's order.</summary>
    internal static readonly string[] VocabularyKinds = ["word", "substring", "prefix"];

    /// <summary>
    /// The family-row types: a static member of either, or of any type built from either, is a family
    /// table the assembly would be shipping.
    /// </summary>
    internal static readonly string[] FamilyRowTypes =
    [
        "Broiler.VM.Ubc.UbcInstructionRow",
        "Broiler.VM.Ubc.UbcInstructionTable",
    ];

    /// <summary>
    /// Reads the banned vocabulary exactly as <c>eng/ubc-vocabulary-scan.py</c>'s
    /// <c>read_vocabulary</c> does.
    /// </summary>
    /// <remarks>
    /// Line for line: a blank line or one whose first non-blank character is <c>#</c> is skipped;
    /// every other line splits at <c>|</c> into exactly three parts whose second is a kind and whose
    /// first is not empty, and nothing is trimmed from a part; the term is lowered; an empty vocabulary
    /// is an error, because every scan over it would pass. The file is decoded without byte-order-mark
    /// detection and split at the newlines Python's universal-newline reader splits at, so a byte the
    /// script would see is a byte this sees.
    /// </remarks>
    internal static IReadOnlyList<VocabularyTerm> ReadVocabulary(string path)
    {
        var terms = new List<VocabularyTerm>();
        var number = 0;

        foreach (var line in ScriptLines(path))
        {
            number++;

            if (PythonStrip(line).Length == 0 || PythonStrip(line).StartsWith('#'))
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != 3 || !VocabularyKinds.Contains(parts[1], StringComparer.Ordinal) || parts[0].Length == 0)
            {
                throw new InvalidDataException(
                    $"{path}:{number}: a vocabulary row is term|kind|why with kind one of " +
                    string.Join(", ", VocabularyKinds));
            }

            terms.Add(new VocabularyTerm(PythonLower(parts[0]), parts[1]));
        }

        if (terms.Count == 0)
        {
            throw new InvalidDataException($"{path}: the vocabulary is empty, so every scan would pass");
        }

        return terms;
    }

    /// <summary>
    /// Reads an identifier list exactly as the script's <c>--identifiers</c> mode does: every line
    /// that is not blank, stripped, is one identifier. A line starting with <c>#</c> is an identifier
    /// too - the script has no comment syntax in this mode, so neither has this.
    /// </summary>
    internal static IReadOnlyList<string> ReadIdentifierList(string path) =>
        ScriptLines(path)
            .Select(PythonStrip)
            .Where(static line => line.Length > 0)
            .ToArray();

    /// <summary>
    /// The terms of <paramref name="terms"/> that match <paramref name="text"/>, written as the script
    /// writes them: <c>term (kind)</c>, in vocabulary order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A port of the script's <c>matches</c>, and the two must agree: a word term matches a whole
    /// segment of the text split into words at every character outside <c>[A-Za-z0-9]</c> and each
    /// word split at its case boundaries (<c>JsValue</c> is <c>js</c> and <c>value</c>, <c>Json</c> is
    /// <c>json</c>); a substring term matches anywhere in the lowered text; a prefix term matches where
    /// no letter or digit stands before it and one stands after it. The two segment patterns are the
    /// script's own, character for character, and every class in them is ASCII in both engines.
    /// </para>
    /// <para>
    /// <b>Lowering is the one place two runtimes could differ</b>, and it is handled rather than
    /// assumed: Python lowers U+0130 to two characters and .NET to one, so that character is lowered
    /// the way Python lowers it. What remains is the difference between the two runtimes' Unicode case
    /// tables for characters that lower to something outside ASCII, which no ASCII term can match -
    /// and the rule's test holds every term of the vocabulary to ASCII, so that remainder is not a
    /// difference in any answer.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> VocabularyMatches(string text, IReadOnlyList<VocabularyTerm> terms)
    {
        var found = new List<string>();
        var lowered = PythonLower(text);
        var words = Segments(text).ToHashSet(StringComparer.Ordinal);

        foreach (var (term, kind) in terms)
        {
            var matched = kind switch
            {
                "word" => words.Contains(term),
                "substring" => lowered.Contains(term, StringComparison.Ordinal),
                "prefix" => Regex.IsMatch(
                    lowered, "(?<![A-Za-z0-9])" + Regex.Escape(term) + "[A-Za-z0-9]", RegexOptions.CultureInvariant),
                _ => false,
            };

            if (matched)
            {
                found.Add($"{term} ({kind})");
            }
        }

        return found;
    }

    /// <summary>The script's <c>segments</c>: every word, split at its case boundaries, lowered.</summary>
    internal static IEnumerable<string> Segments(string text) =>
        ScriptWord.Matches(text)
            .SelectMany(static word => ScriptSegment.Matches(word.Value))
            .Select(static segment => segment.Value.ToLowerInvariant());

    private static readonly Regex ScriptSegment =
        new(@"[A-Z]+(?=[A-Z][a-z])|[A-Z]?[a-z]+|[A-Z]+|[0-9]+", RegexOptions.CultureInvariant);

    private static readonly Regex ScriptWord = new(@"[A-Za-z0-9]+", RegexOptions.CultureInvariant);

    /// <summary>
    /// U2's first clause: no identifier the assembly exports contains a term of the banned vocabulary.
    /// </summary>
    internal static IEnumerable<string> U2Identifiers(
        IEnumerable<ExportedIdentifier> identifiers, IReadOnlyList<VocabularyTerm> terms)
    {
        foreach (var identifier in identifiers)
        {
            var found = VocabularyMatches(identifier.Name, terms);

            if (found.Count > 0)
            {
                yield return
                    $"the exported {identifier.Kind} {identifier.Where} is named {identifier.Name}, " +
                    $"which the banned vocabulary matches: {string.Join(", ", found)}";
            }
        }
    }

    /// <summary>
    /// U2's second clause: the assembly exports no family row - no public or protected static field or
    /// property whose type is <c>UbcInstructionRow</c> or <c>UbcInstructionTable</c>, or any type built
    /// from either.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Read off the DESCRIBED surface, the lines rule U9 freezes, so that the rule that forbids a
    /// family table and the rule that enumerates the surface read one description of one build. A
    /// member line's signature after <c>" : "</c> opens with <c>static</c> exactly when the member is
    /// static - an abstract static member of an interface included, because it is a static property
    /// the surface declares - and a method or constructor carries a parameter list, which a field or
    /// property line does not.
    /// </para>
    /// <para>
    /// "Built from either" is wider than an array or an <c>ImmutableArray</c>: the type is reported
    /// when either family-row type appears in its written name as a whole type name, so a list, a
    /// dictionary value or a nullable of one is a family row as much as an array of one is. An
    /// INSTANCE member of either type is not a row the assembly ships - <c>UbcHookArtifact.Table</c>
    /// hands a hook the table a family registered - and a METHOD that returns or takes one is a
    /// schema, not data.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> U2FamilyRows(IEnumerable<string> describedSurface)
    {
        foreach (var raw in describedSurface)
        {
            var line = raw.Trim();

            if (line.StartsWith("type ", StringComparison.Ordinal))
            {
                continue;
            }

            var separator = line.IndexOf(" : ", StringComparison.Ordinal);

            if (separator < 0)
            {
                continue;
            }

            var declaration = line[..separator];
            var signature = line[(separator + 3)..];

            if (declaration.Contains('(', StringComparison.Ordinal) ||
                !signature.StartsWith("static ", StringComparison.Ordinal))
            {
                continue;
            }

            var isProperty = signature.Contains(" { ", StringComparison.Ordinal);
            var type = Regex.Replace(signature, @"^(?:(?:static|readonly|abstract|virtual|const)\s+)+", string.Empty);
            type = Regex.Replace(type, @"\s+(?:\{.*|=.*)$", string.Empty);

            if (FamilyRowTypes.Any(row => Regex.IsMatch(
                    type, @"(?<![\w.])" + Regex.Escape(row) + @"(?![\w.])", RegexOptions.CultureInvariant)))
            {
                var member = declaration.Split(' ', 2) is [_, var name] ? name : declaration;

                yield return
                    $"the assembly exports {member}, a static {(isProperty ? "property" : "field")} of " +
                    $"type {type}, so it ships a family row - and a family's rows are data the family's " +
                    "own assembly constructs";
            }
        }
    }

    // =============================================================================================
    // U4 - the common family's one table
    // =============================================================================================

    /// <summary>One row of Appendix A, as the rule reads it.</summary>
    internal sealed record AppendixRow(
        int Byte, string Mnemonic, string Shape, string Effect, bool Terminal, bool CodeTarget);

    /// <summary>One row of the common family's table in <see cref="CommonTableFile"/>, as the rule reads it.</summary>
    internal sealed record CommonRow(
        int Byte,
        string Opcode,
        string Mnemonic,
        string Shape,
        string Effect,
        IReadOnlyList<string> Pops,
        IReadOnlyList<string> Pushes,
        bool Terminal,
        bool CodeTarget);

    /// <summary>
    /// Appendix A's notation for every effect the table names by kind rather than by listing its pops
    /// and pushes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the rule's reading key, written once, and it is keyed on the EFFECT and not on an
    /// opcode.</b> Appendix A writes an effect in a notation of its own - <c>[t u] → [u t]</c>,
    /// <c>terminal</c> - and the table writes it as a member of <c>UbcCommonEffect</c>, so comparing the
    /// two needs a sentence saying which notation each member stands for. A <c>Listed</c> row needs
    /// none: its notation is rendered from the row's own pops and pushes. The member comments of
    /// <c>UbcCommonEffect</c> state most of these in the same notation, which is where they were taken
    /// from.
    /// </para>
    /// <para>
    /// A row whose effect is not <c>Listed</c> may still carry pops - <c>jump_table</c> carries the
    /// <c>i32</c> it pops, and the walk pops it from the row - so such a row's listed pops and pushes,
    /// when it has any, must render to the same notation as its kind, and a row whose lists disagree
    /// with its kind is reported.
    /// </para>
    /// </remarks>
    internal static readonly IReadOnlyDictionary<string, string> EffectNotation =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Trap"] = "terminal",
            ["Jump"] = "terminal",
            ["JumpTable"] = "[i32] → []",
            ["Return"] = "[… results] → terminal",
            ["Call"] = "[params] → [results]",
            ["Drop"] = "[t] → []",
            ["Dup"] = "[t] → [t t]",
            ["Dup2"] = "[t u] → [t u t u]",
            ["Swap"] = "[t u] → [u t]",
            ["Pick"] = "[t …] → [t … t]",
            ["Select"] = "[t t i32] → [t]",
            ["Squash"] = "[… n k] → [k]",
            ["LocalGet"] = "[] → [t]",
            ["LocalSet"] = "[t] → []",
            ["LocalTee"] = "[t] → [t]",
        };

    /// <summary>
    /// What clause (a) reports about the checkout today, recorded rather than silenced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Appendix A and the table disagree about whether <c>jump_table</c> is terminal, and neither
    /// is changed here.</b> The table marks it terminal - execution never falls through it, because an
    /// out-of-range selector takes the last row - and the walk relies on that: a <c>jump_table</c> at the
    /// end of a unit would otherwise be refused for running past the end. The appendix gives it the
    /// effect <c>[i32] → []</c> and its rules paragraph names <c>jump</c>, <c>trap</c> and <c>return</c>
    /// as the terminal rows. Which of the two is wrong is a decision for the concept's owner, so the
    /// rule reports the disagreement and its test pins it: the checkout must report EXACTLY this, so a
    /// second disagreement fails the rule, and resolving this one fails it too until the pin is
    /// removed with it.
    /// </para>
    /// </remarks>
    internal static readonly string[] RecordedAppendixDisagreements =
    [
        "0x05 jump_table: Appendix A says terminal is no, the table says yes",
    ];

    /// <summary>U4's first clause over the checkout: Appendix A and the table, row for row.</summary>
    internal static IReadOnlyList<string> U4AppendixViolations() =>
        U4Appendix(
            File.ReadAllText(RootPath(ConceptFile)),
            File.ReadAllText(RootPath(CommonTableFile)));

    /// <summary>
    /// U4's first clause: the table's rows equal Appendix A's row for row - opcode byte, mnemonic,
    /// operand shape, effect, terminal and code target - and no other row exists on either side.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both sides are PARSED. The appendix is the concept's table under its own heading, read by
    /// column name. The table is read from the source of <see cref="CommonTableFile"/> with Roslyn -
    /// every call of the local row function in <c>UbcOpcodes.Build</c>, with the opcode bytes taken from
    /// the <c>UbcOpcode</c> declaration - because this project may not reference the assembly and a
    /// metadata-only load cannot run the method that builds the rows. A row the rule cannot read is a
    /// failure that says so, never a row it skips.
    /// </para>
    /// <para>
    /// <b>Two readings are the rule's and are stated.</b> The appendix has no terminal column: a row is
    /// terminal when its Effect cell says <c>terminal</c>. And it has no code-target column: a row's
    /// operand is a code target when the operand is one <c>u32</c> field and the Meaning cell says the
    /// row jumps or names a code offset. A reworded Meaning cell changes the second reading, which is
    /// why the rule's message names the column it read.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> U4Appendix(string conceptText, string tableText)
    {
        var problems = new List<string>();
        var appendix = ReadAppendix(conceptText, problems);
        var table = ReadCommonTable(tableText, problems);

        if (problems.Count > 0)
        {
            return problems;
        }

        var messages = new List<string>();

        foreach (var duplicate in appendix.GroupBy(static row => row.Byte).Where(static group => group.Count() > 1))
        {
            messages.Add($"Appendix A lists 0x{duplicate.Key:X2} {duplicate.Count()} times");
        }

        foreach (var duplicate in table.GroupBy(static row => row.Byte).Where(static group => group.Count() > 1))
        {
            messages.Add($"the table describes 0x{duplicate.Key:X2} {duplicate.Count()} times");
        }

        var byByte = table.GroupBy(static row => row.Byte).ToDictionary(static group => group.Key, static group => group.First());

        foreach (var expected in appendix.OrderBy(static row => row.Byte))
        {
            if (!byByte.TryGetValue(expected.Byte, out var actual))
            {
                messages.Add(
                    $"0x{expected.Byte:X2} {expected.Mnemonic}: Appendix A lists it and the table has no row for it");
                continue;
            }

            var name = $"0x{expected.Byte:X2} {expected.Mnemonic}";

            Compare(messages, name, "mnemonic", expected.Mnemonic, actual.Mnemonic);
            Compare(messages, name, "operand", expected.Shape, actual.Shape);
            Compare(messages, name, "effect", Normalise(expected.Effect), Notation(actual, messages, name));
            Compare(messages, name, "terminal", YesNo(expected.Terminal), YesNo(actual.Terminal));
            Compare(messages, name, "code target", YesNo(expected.CodeTarget), YesNo(actual.CodeTarget));
        }

        var listed = appendix.Select(static row => row.Byte).ToHashSet();

        foreach (var extra in table.Where(row => !listed.Contains(row.Byte)).OrderBy(static row => row.Byte))
        {
            messages.Add(
                $"0x{extra.Byte:X2} {extra.Mnemonic}: the table defines it and Appendix A does not list it");
        }

        return messages;

        static void Compare(List<string> messages, string name, string column, string expected, string actual)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                messages.Add($"{name}: Appendix A says {column} is {expected}, the table says {actual}");
            }
        }

        static string YesNo(bool value) => value ? "yes" : "no";
    }

    /// <summary>The appendix's notation for one row of the table.</summary>
    private static string Notation(CommonRow row, List<string> messages, string name)
    {
        var listed = $"[{string.Join(" ", row.Pops)}] → [{string.Join(" ", row.Pushes)}]";

        if (string.Equals(row.Effect, "Listed", StringComparison.Ordinal))
        {
            return listed;
        }

        if (!EffectNotation.TryGetValue(row.Effect, out var notation))
        {
            return $"UbcCommonEffect.{row.Effect}, which the rule has no notation for";
        }

        if ((row.Pops.Count > 0 || row.Pushes.Count > 0) && !string.Equals(listed, notation, StringComparison.Ordinal))
        {
            messages.Add(
                $"{name}: the table's effect is {row.Effect} ({notation}) and the row also lists {listed}");
        }

        return notation;
    }

    private static string Normalise(string effect) =>
        Regex.Replace(effect.Replace("->", "→", StringComparison.Ordinal).Replace("...", "…", StringComparison.Ordinal), @"\s+", " ").Trim();

    /// <summary>
    /// Appendix A's rows, read from the first table under the heading that opens with "Appendix A".
    /// </summary>
    /// <remarks>
    /// Cells are split the way <c>eng/ubc-vocabulary-scan.py</c> splits a Markdown row, at every pipe
    /// not escaped with a backslash. A row whose Mnemonic cell is italic is the family-prefix or the
    /// reserved range, which the appendix lists so the byte space is complete and which are not
    /// instructions of this family.
    /// </remarks>
    internal static IReadOnlyList<AppendixRow> ReadAppendix(string conceptText, List<string> problems)
    {
        var lines = conceptText.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var heading = Array.FindIndex(lines, static line => Regex.IsMatch(line, @"^#{1,6}\s+Appendix A\b"));

        if (heading < 0)
        {
            problems.Add($"{ConceptFile} has no heading opening with Appendix A");
            return [];
        }

        var index = heading + 1;

        while (index < lines.Length && !lines[index].StartsWith('|'))
        {
            if (lines[index].StartsWith('#'))
            {
                problems.Add($"{ConceptFile}: Appendix A's heading is followed by another heading before any table");
                return [];
            }

            index++;
        }

        if (index >= lines.Length)
        {
            problems.Add($"{ConceptFile}: no table follows Appendix A's heading");
            return [];
        }

        var header = MarkdownCells(lines[index]);
        var columns = new[] { "Byte", "Mnemonic", "Operand", "Effect", "Meaning" }
            .ToDictionary(static column => column, column => header.IndexOf(column), StringComparer.Ordinal);

        foreach (var missing in columns.Where(static column => column.Value < 0))
        {
            problems.Add($"{ConceptFile}: Appendix A's table has no column {missing.Key}");
        }

        if (problems.Count > 0)
        {
            return [];
        }

        var rows = new List<AppendixRow>();

        for (index += 2; index < lines.Length && lines[index].StartsWith('|'); index++)
        {
            var cells = MarkdownCells(lines[index]);

            string Cell(string column) =>
                columns[column] < cells.Count ? cells[columns[column]] : string.Empty;

            var mnemonic = Cell("Mnemonic");

            if (mnemonic.StartsWith('*'))
            {
                continue;
            }

            var byteText = Cell("Byte").Trim('`');

            if (!byteText.StartsWith("0x", StringComparison.Ordinal) ||
                !int.TryParse(byteText[2..], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value))
            {
                problems.Add($"{ConceptFile}: Appendix A has a row whose byte {Cell("Byte")} the rule cannot read");
                continue;
            }

            var operand = Cell("Operand").Replace("`", string.Empty, StringComparison.Ordinal).Trim();
            var shape = operand is "—" or "-" or ""
                ? "None"
                : string.Concat(operand.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(static field => field.ToUpperInvariant()));

            var effect = Cell("Effect").Replace("`", string.Empty, StringComparison.Ordinal).Trim();
            var meaning = Cell("Meaning").Replace("`", string.Empty, StringComparison.Ordinal);

            rows.Add(new AppendixRow(
                Byte: value,
                Mnemonic: mnemonic.Trim('`'),
                Shape: shape,
                Effect: effect,
                Terminal: Regex.IsMatch(effect, @"\bterminal\b"),
                CodeTarget: string.Equals(shape, "U32", StringComparison.Ordinal) &&
                    Regex.IsMatch(meaning, @"\b(?:jump|code offset)\b", RegexOptions.IgnoreCase)));
        }

        if (rows.Count == 0)
        {
            problems.Add($"{ConceptFile}: Appendix A's table has no instruction row");
        }

        return rows;
    }

    /// <summary>A Markdown table row's cells, split as the vocabulary script splits them.</summary>
    private static List<string> MarkdownCells(string line)
    {
        var cells = Regex.Split(line.Trim(), @"(?<!\\)\|").ToList();

        if (cells.Count > 0 && cells[0].Length == 0)
        {
            cells.RemoveAt(0);
        }

        if (cells.Count > 0 && cells[^1].Length == 0)
        {
            cells.RemoveAt(cells.Count - 1);
        }

        return cells.Select(static cell => cell.Trim()).ToList();
    }

    /// <summary>The parameter names of <c>UbcOpcodes.Build</c>'s row function, in the order the rule reads them.</summary>
    internal static readonly string[] RowFunctionParameters =
        ["opcode", "mnemonic", "shape", "effect", "pops", "pushes", "terminal", "target"];

    /// <summary>
    /// The common family's table, read from the source of <see cref="CommonTableFile"/>.
    /// </summary>
    /// <remarks>
    /// The rows are the calls of the local function <c>Add</c> inside <c>UbcOpcodes.Build</c>, read by
    /// position after the function's own parameter list has been held to
    /// <see cref="RowFunctionParameters"/> by name; a slot-type list is an
    /// <c>ImmutableArray.Create</c> of <c>UbcSlotType</c> members, <c>ImmutableArray&lt;UbcSlotType&gt;.Empty</c>,
    /// or a local of <c>Build</c> initialised to one of those. Anything else is reported as unreadable
    /// rather than guessed at.
    /// </remarks>
    internal static IReadOnlyList<CommonRow> ReadCommonTable(string tableText, List<string> problems)
    {
        var root = AssuranceSources.Parse(tableText, CommonTableFile).GetRoot();
        var bytes = EnumValues(root, "UbcOpcode", problems);

        var build = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => type.Identifier.ValueText == "UbcOpcodes")
            .SelectMany(static type => type.Members.OfType<MethodDeclarationSyntax>())
            .SingleOrDefault(static method => method.Identifier.ValueText == "Build");

        if (build is null)
        {
            problems.Add($"{CommonTableFile} has no method UbcOpcodes.Build, which is where the rule reads the rows");
            return [];
        }

        var function = build.DescendantNodes().OfType<LocalFunctionStatementSyntax>()
            .SingleOrDefault(static function => function.Identifier.ValueText == "Add");

        var parameters = function?.ParameterList.Parameters.Select(static parameter => parameter.Identifier.ValueText).ToArray() ?? [];

        if (!parameters.SequenceEqual(RowFunctionParameters, StringComparer.Ordinal))
        {
            problems.Add(
                $"{CommonTableFile}: UbcOpcodes.Build's row function is Add({string.Join(", ", parameters)}) " +
                $"rather than Add({string.Join(", ", RowFunctionParameters)}), so the rule cannot read its " +
                "rows by position");
            return [];
        }

        var locals = build.DescendantNodes().OfType<VariableDeclaratorSyntax>()
            .Where(static local => local.Initializer is not null)
            .ToDictionary(static local => local.Identifier.ValueText, static local => local.Initializer!.Value, StringComparer.Ordinal);

        var rows = new List<CommonRow>();

        foreach (var call in build.DescendantNodes().OfType<InvocationExpressionSyntax>()
                     .Where(static call => call.Expression is IdentifierNameSyntax { Identifier.ValueText: "Add" }))
        {
            var line = call.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            var arguments = call.ArgumentList.Arguments.Select(static argument => argument.Expression).ToArray();

            if (arguments.Length != RowFunctionParameters.Length)
            {
                problems.Add($"{CommonTableFile}({line}): a row has {arguments.Length} arguments rather than {RowFunctionParameters.Length}");
                continue;
            }

            var opcode = QualifiedMember(arguments[0], "UbcOpcode");
            var shape = QualifiedMember(arguments[2], "UbcOperandShape");
            var effect = QualifiedMember(arguments[3], "UbcCommonEffect");
            var pops = SlotList(arguments[4], locals);
            var pushes = SlotList(arguments[5], locals);

            if (opcode is null || !bytes.TryGetValue(opcode, out var value) ||
                arguments[1] is not LiteralExpressionSyntax { RawKind: (int)SyntaxKind.StringLiteralExpression } mnemonic ||
                shape is null || effect is null || pops is null || pushes is null ||
                Boolean(arguments[6]) is not { } terminal || Boolean(arguments[7]) is not { } target)
            {
                problems.Add($"{CommonTableFile}({line}): the rule cannot read the row {call}");
                continue;
            }

            rows.Add(new CommonRow(value, opcode, mnemonic.Token.ValueText, shape, effect, pops, pushes, terminal, target));
        }

        if (rows.Count == 0)
        {
            problems.Add($"{CommonTableFile}: UbcOpcodes.Build describes no row the rule can read");
        }

        foreach (var member in bytes.Keys.Where(member => rows.All(row => row.Opcode != member)).Order(StringComparer.Ordinal))
        {
            problems.Add($"{CommonTableFile}: UbcOpcode.{member} is declared and the table has no row for it");
        }

        return rows;

        static bool? Boolean(ExpressionSyntax expression) => expression.Kind() switch
        {
            SyntaxKind.TrueLiteralExpression => true,
            SyntaxKind.FalseLiteralExpression => false,
            _ => null,
        };
    }

    /// <summary>The members of an enum declared in <paramref name="root"/>, with their values.</summary>
    internal static Dictionary<string, int> EnumValues(SyntaxNode root, string enumName, List<string> problems)
    {
        var values = new Dictionary<string, int>(StringComparer.Ordinal);

        var declaration = root.DescendantNodes().OfType<EnumDeclarationSyntax>()
            .SingleOrDefault(declaration => declaration.Identifier.ValueText == enumName);

        if (declaration is null)
        {
            problems.Add($"no declaration of the enum {enumName} was found where the rule reads it");
            return values;
        }

        foreach (var member in declaration.Members)
        {
            if (member.EqualsValue?.Value is LiteralExpressionSyntax { Token.Value: int value })
            {
                values[member.Identifier.ValueText] = value;
            }
            else
            {
                problems.Add($"{enumName}.{member.Identifier.ValueText} has no literal value the rule can read");
            }
        }

        return values;
    }

    /// <summary>The member of <paramref name="enumName"/> an expression names as <c>Enum.Member</c>, or null.</summary>
    private static string? QualifiedMember(ExpressionSyntax expression, string enumName) =>
        Strip(expression) is MemberAccessExpressionSyntax access &&
        string.Equals(Rightmost(access.Expression), enumName, StringComparison.Ordinal)
            ? access.Name.Identifier.ValueText
            : null;

    /// <summary>A slot-type list the table writes, resolved through <c>Build</c>'s locals; null when unreadable.</summary>
    private static IReadOnlyList<string>? SlotList(ExpressionSyntax expression, IReadOnlyDictionary<string, ExpressionSyntax> locals)
    {
        expression = Strip(expression);

        if (expression is IdentifierNameSyntax local)
        {
            return locals.TryGetValue(local.Identifier.ValueText, out var initializer) && initializer != expression
                ? SlotList(initializer, new Dictionary<string, ExpressionSyntax>())
                : null;
        }

        if (expression is MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Empty" } empty &&
            empty.Expression is GenericNameSyntax { Identifier.ValueText: "ImmutableArray" } generic &&
            generic.TypeArgumentList.Arguments.Count == 1 &&
            Rightmost(generic.TypeArgumentList.Arguments[0]) == "UbcSlotType")
        {
            return [];
        }

        if (expression is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Create" } create } invocation &&
            Rightmost(create.Expression) == "ImmutableArray")
        {
            var members = invocation.ArgumentList.Arguments
                .Select(static argument => QualifiedMember(argument.Expression, "UbcSlotType"))
                .ToArray();

            return members.All(static member => member is not null)
                ? members.Select(static member => member!.ToLowerInvariant()).ToArray()
                : null;
        }

        return null;
    }

    /// <summary>
    /// The width the one table gives each common opcode: the opcode byte and the operand width of the
    /// row's shape, read from <c>UbcOperandShapes.Width</c>.
    /// </summary>
    internal static IReadOnlyDictionary<string, int> OneTableWidths(List<string> problems)
    {
        var table = ReadCommonTable(File.ReadAllText(RootPath(CommonTableFile)), problems);
        var shapes = ShapeWidths(File.ReadAllText(RootPath(SlotTypesFile)), problems);

        return table
            .Where(row => shapes.ContainsKey(row.Shape))
            .ToDictionary(static row => row.Opcode, row => 1 + shapes[row.Shape], StringComparer.Ordinal);
    }

    /// <summary>The operand width of every shape, from the switch in <c>UbcOperandShapes.Width</c>.</summary>
    internal static IReadOnlyDictionary<string, int> ShapeWidths(string slotTypesText, List<string> problems)
    {
        var root = AssuranceSources.Parse(slotTypesText, SlotTypesFile).GetRoot();

        var width = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => type.Identifier.ValueText == "UbcOperandShapes")
            .SelectMany(static type => type.Members.OfType<MethodDeclarationSyntax>())
            .SingleOrDefault(static method => method.Identifier.ValueText == "Width");

        var widths = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var arm in width?.DescendantNodes().OfType<SwitchExpressionArmSyntax>() ?? [])
        {
            if (arm.Pattern is ConstantPatternSyntax { Expression: var key } &&
                QualifiedMember(key, "UbcOperandShape") is { } shape &&
                Strip(arm.Expression) is LiteralExpressionSyntax { Token.Value: int value })
            {
                widths[shape] = value;
            }
        }

        if (widths.Count == 0)
        {
            problems.Add($"{SlotTypesFile}: UbcOperandShapes.Width states no width the rule can read");
        }

        return widths;
    }

    /// <summary>
    /// The widths a second table states, from its switch arms keyed on single <c>UbcOpcode</c> members
    /// with an integer literal value.
    /// </summary>
    internal static IReadOnlyDictionary<string, int> StatedWidths(string text)
    {
        var root = AssuranceSources.Parse(text, "witness").GetRoot();
        var widths = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var arm in root.DescendantNodes().OfType<SwitchExpressionArmSyntax>())
        {
            if (arm.Pattern is ConstantPatternSyntax { Expression: var key } &&
                QualifiedMember(key, "UbcOpcode") is { } opcode &&
                Strip(arm.Expression) is LiteralExpressionSyntax { Token.Value: int value })
            {
                widths[opcode] = value;
            }
        }

        return widths;
    }

    /// <summary>One source file a U4 sweep reads, with the project it belongs to.</summary>
    internal sealed record UbcSource(string RelativePath, string Project, string Text);

    /// <summary>
    /// Every source file of <c>Broiler.VM.Ubc</c> and of every project whose references reach it,
    /// directly or through another project.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A project that references the universal bytecode through another one - a composition over an
    /// emitter that references it - can name the common family's opcodes just as well, because
    /// project references flow, so the sweep follows the reference closure rather than the direct
    /// edges alone. The files are every <c>*.cs</c> under the project's directory outside its own build
    /// output, and every <c>Compile</c> item the project links from elsewhere.
    /// </para>
    /// <para>
    /// The one table's own file is in the sweep's input and is excluded by <see cref="U4SecondTables"/>
    /// by path, so the exclusion is visible in one place and a test can show it working.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<UbcSource> U4Sources()
    {
        var references = ComponentGraph.Projects
            .GroupBy(static project => project.AssemblyName, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.SelectMany(static project => project.ReferencedAssemblyNames).ToArray(),
                StringComparer.Ordinal);

        bool Reaches(string assembly, HashSet<string> seen) =>
            seen.Add(assembly) &&
            references.TryGetValue(assembly, out var targets) &&
            targets.Any(target => string.Equals(target, UbcAssembly, StringComparison.Ordinal) || Reaches(target, seen));

        var sources = new List<UbcSource>();

        foreach (var project in ComponentGraph.Projects.Where(project =>
                     string.Equals(project.AssemblyName, UbcAssembly, StringComparison.Ordinal) ||
                     Reaches(project.AssemblyName, new HashSet<string>(StringComparer.Ordinal))))
        {
            var directory = Path.GetDirectoryName(project.Path)!;

            var files = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !AssuranceSources.IsProjectBuildOutput(directory, path))
                .Concat(project.SourceItemPaths.Where(static path =>
                    path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && File.Exists(path)))
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static path => path, StringComparer.Ordinal);

            foreach (var path in files)
            {
                sources.Add(new UbcSource(
                    Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'),
                    project.AssemblyName,
                    File.ReadAllText(path)));
            }
        }

        return sources;
    }

    /// <summary>
    /// The enums a second table would be written in, with their members, read from the universal
    /// bytecode's own source so a <c>using static</c> of one can be followed.
    /// </summary>
    internal static IReadOnlyDictionary<string, IReadOnlySet<string>> TableEnums()
    {
        var problems = new List<string>();
        var opcodes = AssuranceSources.Parse(File.ReadAllText(RootPath(CommonTableFile)), CommonTableFile).GetRoot();
        var slots = AssuranceSources.Parse(File.ReadAllText(RootPath(SlotTypesFile)), SlotTypesFile).GetRoot();

        return new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["UbcOpcode"] = EnumValues(opcodes, "UbcOpcode", problems).Keys.ToHashSet(StringComparer.Ordinal),
            ["UbcCommonEffect"] = EnumValues(opcodes, "UbcCommonEffect", problems).Keys.ToHashSet(StringComparer.Ordinal),
            ["UbcOperandShape"] = EnumValues(slots, "UbcOperandShape", problems).Keys.ToHashSet(StringComparer.Ordinal),
            ["UbcSlotType"] = EnumValues(slots, "UbcSlotType", problems).Keys.ToHashSet(StringComparer.Ordinal),
        };
    }

    /// <summary>
    /// U4's second clause: no source file but the one table's states a common row's width, operand
    /// shape or effect in a construct keyed on <c>UbcOpcode</c> members.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What is a second table.</b> A construct keyed on <c>UbcOpcode</c> members whose value, for
    /// a key, is a fact the common table owns: a switch-expression arm or a switch-statement section
    /// whose whole body returns or assigns it; a dictionary initialiser entry, in either spelling; an
    /// assignment to an element indexed by an opcode member, which is how an array keyed on opcodes is
    /// filled; an <c>Add</c> or <c>TryAdd</c> call with an opcode member among its arguments; and any
    /// argument list, object initialiser or tuple that pairs an opcode member with a shape, an effect
    /// or a slot type. The facts are an integer literal - a width - an operand shape, a common effect
    /// or an effect descriptor, a slot type, or a list of slot types.
    /// </para>
    /// <para>
    /// <b>Integer literals are read only where a construct is keyed.</b> An argument list or a tuple
    /// that pairs an opcode with an integer is how an instruction is EMITTED - the builder's
    /// <c>Emit(UbcOpcode.ConstI32, 7)</c> carries an operand, not a width - so a bare integer beside an
    /// opcode is a table only in a switch, a dictionary, an indexed assignment or an <c>Add</c> call.
    /// Dispatching on an opcode to behaviour is not reported either: an arm whose body does something
    /// is what an interpreter is, and only an arm whose whole body IS one of these facts is a row.
    /// </para>
    /// <para>
    /// <b>Names are followed through the file's and the project's using directives</b> - an alias of
    /// one of the four enums and a <c>using static</c> of one - and read through unicode escapes, because
    /// the rule compares identifiers' value text. What a syntax scan cannot see is stated rather than
    /// implied: a table keyed on raw opcode BYTES rather than members, a value held in a constant or
    /// returned by a helper rather than written in the arm, and a key reached through a variable.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> U4SecondTables(
        IEnumerable<UbcSource> sources, IReadOnlyDictionary<string, IReadOnlySet<string>> enums)
    {
        var files = sources.ToArray();

        var parsed = files
            .Select(file => (File: file, Root: AssuranceSources.Parse(file.Text, file.RelativePath).GetRoot()))
            .ToArray();

        // Global using directives reach every file of their project.
        var global = parsed
            .GroupBy(static unit => unit.File.Project, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.SelectMany(static unit => unit.Root.DescendantNodes()
                        .OfType<UsingDirectiveSyntax>()
                        .Where(static directive => directive.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)))
                    .ToArray(),
                StringComparer.Ordinal);

        foreach (var (file, root) in parsed)
        {
            if (string.Equals(file.RelativePath, CommonTableFile, StringComparison.Ordinal))
            {
                continue;
            }

            var scope = new Scope(enums, root.DescendantNodes().OfType<UsingDirectiveSyntax>().Concat(global[file.Project]));

            foreach (var message in SecondTablesIn(file.RelativePath, root, scope))
            {
                yield return message;
            }
        }
    }

    private static IEnumerable<string> SecondTablesIn(string path, SyntaxNode root, Scope scope)
    {
        var reported = new HashSet<TextSpan>();

        foreach (var node in root.DescendantNodes())
        {
            foreach (var (construct, key, value, kind) in Rows(node, scope))
            {
                if (!reported.Add(value.Span))
                {
                    continue;
                }

                var line = value.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

                yield return
                    $"{path}({line}): {construct} keyed on UbcOpcode.{key} states {kind} ({value}), and " +
                    $"only {CommonTableFile} may state a common row's width, operand shape or effect";
            }
        }
    }

    private static IEnumerable<(string Construct, string Key, ExpressionSyntax Value, string Kind)> Rows(SyntaxNode node, Scope scope)
    {
        switch (node)
        {
            case SwitchExpressionArmSyntax arm when OpcodeKeys(arm.Pattern, scope).FirstOrDefault() is { } key &&
                Fact(arm.Expression, scope, integers: true) is { } kind:
                yield return ("a switch arm", key, arm.Expression, kind);
                break;

            case SwitchSectionSyntax section:
            {
                var key = section.Labels.SelectMany(label => LabelKeys(label, scope)).FirstOrDefault();

                if (key is not null && SectionValue(section) is { } value && Fact(value, scope, integers: true) is { } kind)
                {
                    yield return ("a switch section", key, value, kind);
                }

                break;
            }

            case AssignmentExpressionSyntax { Left: ImplicitElementAccessSyntax access } assignment
                when access.ArgumentList.Arguments.Select(argument => scope.Member(argument.Expression, "UbcOpcode")).FirstOrDefault(static member => member is not null) is { } key &&
                Fact(assignment.Right, scope, integers: true) is { } kind:
                yield return ("a dictionary entry", key, assignment.Right, kind);
                break;

            case AssignmentExpressionSyntax { Left: ElementAccessExpressionSyntax access } assignment
                when access.ArgumentList.Arguments.Select(argument => scope.Member(argument.Expression, "UbcOpcode")).FirstOrDefault(static member => member is not null) is { } key &&
                Fact(assignment.Right, scope, integers: true) is { } kind:
                yield return ("an element indexed", key, assignment.Right, kind);
                break;

            case InitializerExpressionSyntax initializer when initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression) &&
                initializer.Expressions.Count > 1 &&
                scope.Member(initializer.Expressions[0], "UbcOpcode") is { } key:
                foreach (var value in initializer.Expressions.Skip(1))
                {
                    if (Fact(value, scope, integers: true) is { } kind)
                    {
                        yield return ("a dictionary entry", key, value, kind);
                    }
                }

                break;

            case InitializerExpressionSyntax initializer when initializer.IsKind(SyntaxKind.ObjectInitializerExpression):
                foreach (var row in Paired(
                             "an object initialiser",
                             initializer.Expressions.OfType<AssignmentExpressionSyntax>()
                                 .Where(static assignment => assignment.Left is IdentifierNameSyntax)
                                 .Select(static assignment => assignment.Right),
                             scope,
                             integers: false))
                {
                    yield return row;
                }

                break;

            case InvocationExpressionSyntax invocation:
            {
                var name = Rightmost(invocation.Expression);
                var integers = name is "Add" or "TryAdd";

                foreach (var row in Paired(
                             integers ? "a collection filled by " + name : "an argument list",
                             invocation.ArgumentList.Arguments.Select(static argument => argument.Expression),
                             scope,
                             integers))
                {
                    yield return row;
                }

                break;
            }

            case BaseObjectCreationExpressionSyntax creation when creation.ArgumentList is { } arguments:
                foreach (var row in Paired("an argument list", arguments.Arguments.Select(static argument => argument.Expression), scope, integers: false))
                {
                    yield return row;
                }

                break;

            case AttributeSyntax { ArgumentList: { } attributeArguments }:
                foreach (var row in Paired("an attribute's arguments", attributeArguments.Arguments.Select(static argument => argument.Expression), scope, integers: false))
                {
                    yield return row;
                }

                break;

            case TupleExpressionSyntax tuple:
                foreach (var row in Paired("a tuple", tuple.Arguments.Select(static argument => argument.Expression), scope, integers: false))
                {
                    yield return row;
                }

                break;
        }
    }

    /// <summary>A list of expressions in which an opcode member is paired with a fact the table owns.</summary>
    private static IEnumerable<(string Construct, string Key, ExpressionSyntax Value, string Kind)> Paired(
        string construct, IEnumerable<ExpressionSyntax> expressions, Scope scope, bool integers)
    {
        var list = expressions.ToArray();
        var key = list.Select(expression => scope.Member(expression, "UbcOpcode")).FirstOrDefault(static member => member is not null);

        if (key is null)
        {
            yield break;
        }

        foreach (var value in list)
        {
            if (scope.Member(value, "UbcOpcode") is null && Fact(value, scope, integers) is { } kind)
            {
                yield return (construct, key, value, kind);
            }
        }
    }

    /// <summary>Every <c>UbcOpcode</c> member a switch label names.</summary>
    private static IEnumerable<string> LabelKeys(SwitchLabelSyntax label, Scope scope)
    {
        switch (label)
        {
            case CaseSwitchLabelSyntax constant when scope.Member(constant.Value, "UbcOpcode") is { } member:
                yield return member;
                break;

            case CasePatternSwitchLabelSyntax pattern:
                foreach (var member in OpcodeKeys(pattern.Pattern, scope))
                {
                    yield return member;
                }

                break;
        }
    }

    /// <summary>The one value a switch section's whole body returns or assigns, or null.</summary>
    private static ExpressionSyntax? SectionValue(SwitchSectionSyntax section)
    {
        var statements = section.Statements
            .SelectMany(static statement => statement is BlockSyntax block
                ? (IEnumerable<StatementSyntax>)block.Statements
                : [statement])
            .Where(static statement => statement is not BreakStatementSyntax)
            .ToArray();

        return statements switch
        {
            [ReturnStatementSyntax { Expression: { } value }] => value,
            [ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment }] => assignment.Right,
            _ => null,
        };
    }

    /// <summary>Every <c>UbcOpcode</c> member a pattern names as a constant.</summary>
    private static IEnumerable<string> OpcodeKeys(PatternSyntax pattern, Scope scope) =>
        pattern.DescendantNodesAndSelf()
            .Select(node => node switch
            {
                ConstantPatternSyntax constant => scope.Member(constant.Expression, "UbcOpcode"),
                TypePatternSyntax { Type: QualifiedNameSyntax qualified } =>
                    scope.IsEnum(Rightmost(qualified.Left), "UbcOpcode") ? qualified.Right.Identifier.ValueText : null,
                _ => null,
            })
            .Where(static member => member is not null)
            .Select(static member => member!);

    /// <summary>What fact the common table owns an expression states, or null when it states none.</summary>
    private static string? Fact(ExpressionSyntax expression, Scope scope, bool integers)
    {
        var value = Strip(expression);

        if (value is PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.UnaryMinusExpression or (int)SyntaxKind.UnaryPlusExpression } signed)
        {
            value = Strip(signed.Operand);
        }

        if (integers && value.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            return "an integer literal";
        }

        if (scope.Member(value, "UbcOperandShape") is not null)
        {
            return "an operand shape";
        }

        if (scope.Member(value, "UbcCommonEffect") is not null)
        {
            return "a common effect";
        }

        if (value is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax effect } &&
            effect.Name.Identifier.ValueText is "Listed" or "Counted" &&
            scope.IsType(Rightmost(effect.Expression), "UbcEffect"))
        {
            return "an effect descriptor";
        }

        if (scope.Member(value, "UbcSlotType") is not null)
        {
            return "a slot type";
        }

        return IsSlotList(value, scope) ? "a slot-type list" : null;
    }

    /// <summary>Whether an expression is a list of slot types.</summary>
    private static bool IsSlotList(ExpressionSyntax value, Scope scope)
    {
        bool AllSlots(IEnumerable<ExpressionSyntax> elements)
        {
            var list = elements.ToArray();
            return list.Length > 0 && list.All(element => scope.Member(element, "UbcSlotType") is not null);
        }

        bool NamesSlotType(TypeSyntax? type) => scope.IsType(Rightmost(type), "UbcSlotType");

        bool HasSlotTypeArgument(SyntaxNode? node) =>
            node?.DescendantNodesAndSelf().OfType<GenericNameSyntax>()
                .Any(generic => generic.TypeArgumentList.Arguments.Any(NamesSlotType)) == true;

        return value switch
        {
            CollectionExpressionSyntax collection =>
                collection.Elements.All(static element => element is ExpressionElementSyntax) &&
                AllSlots(collection.Elements.OfType<ExpressionElementSyntax>().Select(static element => element.Expression)),
            ImplicitArrayCreationExpressionSyntax array => AllSlots(array.Initializer.Expressions),
            ArrayCreationExpressionSyntax array => NamesSlotType(array.Type.ElementType),
            InvocationExpressionSyntax invocation =>
                AllSlots(invocation.ArgumentList.Arguments.Select(static argument => argument.Expression)) ||
                (invocation.ArgumentList.Arguments.Count == 0 && HasSlotTypeArgument(invocation.Expression)),
            MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Empty" } empty => HasSlotTypeArgument(empty.Expression),
            BaseObjectCreationExpressionSyntax creation =>
                HasSlotTypeArgument((creation as ObjectCreationExpressionSyntax)?.Type) ||
                (creation.Initializer is { } initializer && AllSlots(initializer.Expressions)),
            _ => false,
        };
    }

    /// <summary>The expression under any parentheses, casts, checked contexts and null-forgiving operators.</summary>
    private static ExpressionSyntax Strip(ExpressionSyntax expression)
    {
        while (true)
        {
            switch (expression)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    expression = parenthesized.Expression;
                    continue;
                case CastExpressionSyntax cast:
                    expression = cast.Expression;
                    continue;
                case CheckedExpressionSyntax @checked:
                    expression = @checked.Expression;
                    continue;
                case PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.SuppressNullableWarningExpression } forgiven:
                    expression = forgiven.Operand;
                    continue;
                default:
                    return expression;
            }
        }
    }

    /// <summary>The right-most simple name of a name or member access, unescaped.</summary>
    private static string? Rightmost(SyntaxNode? node) => node switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        GenericNameSyntax generic => generic.Identifier.ValueText,
        QualifiedNameSyntax qualified => Rightmost(qualified.Right),
        AliasQualifiedNameSyntax alias => Rightmost(alias.Name),
        MemberAccessExpressionSyntax access => Rightmost(access.Name),
        _ => null,
    };

    /// <summary>What one file's using directives make of the four enum names and their members.</summary>
    private sealed class Scope
    {
        private readonly IReadOnlyDictionary<string, IReadOnlySet<string>> enums;
        private readonly Dictionary<string, string> aliases = new(StringComparer.Ordinal);
        private readonly HashSet<string> imported = new(StringComparer.Ordinal);

        internal Scope(IReadOnlyDictionary<string, IReadOnlySet<string>> enums, IEnumerable<UsingDirectiveSyntax> directives)
        {
            this.enums = enums;

            foreach (var directive in directives)
            {
                var target = Rightmost(directive.NamespaceOrType);

                if (target is null)
                {
                    continue;
                }

                if (directive.Alias is { } alias)
                {
                    aliases[alias.Name.Identifier.ValueText] = target;
                }
                else if (directive.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    imported.Add(target);
                }
            }
        }

        /// <summary>Whether a simple name refers to the named type, directly or through an alias.</summary>
        internal bool IsType(string? name, string type) =>
            name is not null &&
            (string.Equals(name, type, StringComparison.Ordinal) ||
             (aliases.TryGetValue(name, out var target) && string.Equals(target, type, StringComparison.Ordinal)));

        /// <summary>Whether a simple name refers to the named enum.</summary>
        internal bool IsEnum(string? name, string enumName) => IsType(name, enumName);

        /// <summary>The member of <paramref name="enumName"/> an expression names, or null.</summary>
        internal string? Member(ExpressionSyntax expression, string enumName)
        {
            var value = Strip(expression);

            if (value is MemberAccessExpressionSyntax access && IsType(Rightmost(access.Expression), enumName))
            {
                return access.Name.Identifier.ValueText;
            }

            if (value is IdentifierNameSyntax identifier && imported.Contains(enumName) &&
                enums.TryGetValue(enumName, out var members) && members.Contains(identifier.Identifier.ValueText))
            {
                return identifier.Identifier.ValueText;
            }

            return null;
        }
    }

    // =============================================================================================
    // Shared
    // =============================================================================================

    /// <summary>A path under the component root, written with forward slashes.</summary>
    internal static string RootPath(string relative) =>
        Path.Combine(ComponentGraph.Root, relative.Replace('/', Path.DirectorySeparatorChar));

    /// <summary>
    /// A file's lines as Python's text-mode reader yields them: UTF-8 with no byte-order-mark
    /// detection, split at <c>\n</c>, <c>\r\n</c> and <c>\r</c>.
    /// </summary>
    private static IEnumerable<string> ScriptLines(string path)
    {
        using var reader = new StreamReader(path, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: false);

        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    /// <summary>Python's <c>str.strip()</c>: its whitespace is .NET's plus the four separators U+001C to U+001F.</summary>
    private static string PythonStrip(string text)
    {
        static bool Space(char character) =>
            char.IsWhiteSpace(character) || character is >= '\u001C' and <= '\u001F';

        var start = 0;
        var end = text.Length;

        while (start < end && Space(text[start]))
        {
            start++;
        }

        while (end > start && Space(text[end - 1]))
        {
            end--;
        }

        return text[start..end];
    }

    /// <summary>
    /// Python's <c>str.lower()</c> for every character whose lowercase can be ASCII: .NET's invariant
    /// lowering, except U+0130, which Python lowers to <c>i</c> followed by a combining dot.
    /// </summary>
    internal static string PythonLower(string text)
    {
        var builder = new StringBuilder(text.Length);

        foreach (var rune in text.EnumerateRunes())
        {
            if (rune.Value == 0x0130)
            {
                builder.Append("i̇");
            }
            else
            {
                builder.Append(Rune.ToLowerInvariant(rune).ToString());
            }
        }

        return builder.ToString();
    }
}
