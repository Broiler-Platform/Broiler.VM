namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group U: the universal bytecode's reference set, its exported vocabulary, the common family's one
/// table and its public-API baseline.
/// </summary>
/// <remarks>
/// <para>
/// Each rule is asserted twice, as every group here is: the checkout is clean, and the rule rejects a
/// violating input. The violating inputs are witness files rather than edits, so a reader can see what a
/// violation looks like without one existing, and they are one per CLAUSE, as group N's and group W's
/// are, because a witness asserted with a bare non-empty check pins only whichever clause fires first.
/// </para>
/// <para>
/// <b>U8 is not here.</b> It holds the universal bytecode's diagnostic registry, and it is minted in the
/// change that writes the registry, because a rule over a file that does not exist yet would be a rule
/// that could only fail or only pass vacuously.
/// </para>
/// </remarks>
public sealed class UbcRuleTests
{
    private const string BaselineName = "docs/ubc/api/public-api.txt";

    private const string WriteSwitch = "BROILER_API_WRITE";

    /// <summary>Names a file the U2 test writes every identifier it scanned to, one per line.</summary>
    /// <remarks>
    /// The file is the input <c>eng/ubc-vocabulary-scan.py --identifiers</c> takes, so the script can be
    /// run over exactly what the rule read and the two answers compared - which is how the claim that
    /// the rule and the script agree is checked against the real surface rather than only against the
    /// witness.
    /// </remarks>
    private const string IdentifierSwitch = "BROILER_UBC_IDENTIFIERS";

    /// <summary>Whether this run regenerates the baseline rather than asserting against it.</summary>
    private static bool Writing =>
        string.Equals(Environment.GetEnvironmentVariable(WriteSwitch), "1", StringComparison.Ordinal);

    // =============================================================================================
    // U1
    // =============================================================================================

    [Fact]
    public void U1_The_Universal_Bytecode_References_Exactly_Abstractions_And_Binary()
    {
        // Non-vacuous in both directions: the real project exists and has the set, and the rule
        // rejects each of the six ways it could stop having it. Without the first clause this would
        // pass over a checkout that contained no universal bytecode at all.
        Assert.Contains(
            ComponentGraph.Projects,
            project => string.Equals(project.AssemblyName, UbcRules.UbcAssembly, StringComparison.Ordinal));

        Assert.Empty(ComponentGraph.Projects.SelectMany(UbcRules.U1));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-references-a-third-project.csproj.witness")),
            message => message.Contains(
                "references [Broiler.VM.Abstractions, Broiler.VM.Binary, Broiler.VM.Runtime] rather than",
                StringComparison.Ordinal));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-package-reference.csproj.witness")),
            message => message.Contains("declares PackageReference System.Text.Json", StringComparison.Ordinal));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-internals-visible-to.csproj.witness")),
            message => message.Contains("opens internals to", StringComparison.Ordinal));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-packable.csproj.witness")),
            message => message.Contains("does not carry the literal <IsPackable>false</IsPackable>", StringComparison.Ordinal));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-package-id.csproj.witness")),
            message => message.Contains("declares PackageId Broiler.VM.Ubc", StringComparison.Ordinal));

        Assert.Contains(
            UbcRules.U1(ComponentGraph.Witness("U1-ubc-allows-unsafe-blocks.csproj.witness")),
            message => message.Contains("sets AllowUnsafeBlocks to true", StringComparison.Ordinal));
    }

    [Fact]
    public void U1_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = Row("U1");

        Assert.Equal("Active", row.Status);
        Assert.Equal("0013", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        // The row must name every clause the rule reports, because a clause the row does not state is
        // a clause a reader cannot know is enforced.
        foreach (var clause in new[] { "Broiler.VM.Abstractions", "Broiler.VM.Binary", "PackageReference", "IsPackable", "PackageId", "AllowUnsafeBlocks" })
        {
            Assert.Contains(clause, row.Statement, StringComparison.Ordinal);
        }
    }

    // =============================================================================================
    // U2
    // =============================================================================================

    [Fact]
    public void U2_No_Exported_Identifier_Is_In_The_Banned_Vocabulary()
    {
        var identifiers = UbcApiSurface.Identifiers();
        var terms = UbcRules.ReadVocabulary(UbcRules.RootPath(UbcRules.VocabularyFile));

        // A run that has not built the assembly reads no identifier, and a scan of nothing passes. The
        // rule fails on it instead.
        Assert.Equal(UbcApiSurface.Assemblies.Length, UbcApiSurface.Found().Count);
        Assert.NotEmpty(identifiers);

        // Non-vacuous over every kind of name the statement promises to read. A reader that silently
        // stopped reading parameters or enum members would pass this rule over half its subject.
        foreach (var kind in new[] { "namespace", "type", "generic parameter", "field", "enum member", "property", "method", "parameter" })
        {
            Assert.Contains(identifiers, identifier => string.Equals(identifier.Kind, kind, StringComparison.Ordinal));
        }

        Assert.Contains(identifiers, static identifier => identifier is { Kind: "enum member", Name: "Nop" });
        Assert.Contains(identifiers, static identifier => identifier is { Kind: "generic parameter", Name: "TFamily" });
        Assert.Contains(identifiers, static identifier => identifier is { Kind: "parameter", Name: "opcode" });

        // The agreement with the script is exact for an ASCII term, and every term is one.
        Assert.All(terms, static term => Assert.True(
            term.Term.All(static character => character < 0x80),
            $"the vocabulary term {term.Term} is not ASCII, so the rule and the script could lower it differently"));

        if (Environment.GetEnvironmentVariable(IdentifierSwitch) is { Length: > 0 } dump)
        {
            File.WriteAllLines(dump, identifiers.Select(static identifier => identifier.Name));
        }

        Assert.Empty(UbcRules.U2Identifiers(identifiers, terms));
    }

    /// <summary>
    /// The rule matches as the script matches, over the examples the vocabulary file itself gives.
    /// </summary>
    /// <remarks>
    /// The file's header says <c>JsValue</c> and <c>JS</c> match <c>js</c> and <c>Json</c> does not, and
    /// that a prefix term is the mnemonic prefix followed by a letter or a digit. Those sentences are the
    /// specification a reader of the vocabulary has, so the port is held to them by name; the witness
    /// below holds it to the script's own output over the same list.
    /// </remarks>
    [Fact]
    public void U2_Matches_The_Vocabulary_As_The_Scan_Script_Does()
    {
        var terms = UbcRules.ReadVocabulary(UbcRules.RootPath(UbcRules.VocabularyFile));

        Assert.Equal(["js (word)"], UbcRules.VocabularyMatches("JsValue", terms));
        Assert.Equal(["js (word)"], UbcRules.VocabularyMatches("JS", terms));
        Assert.Empty(UbcRules.VocabularyMatches("Json", terms));

        // The script's limit, kept rather than repaired, because the rule must agree with the script:
        // a run of capitals is one segment up to the capital that opens the next word, so the J of
        // IJSRuntime is inside the segment "ijs" and the word term does not match it. The register row
        // states it.
        Assert.Empty(UbcRules.VocabularyMatches("IJSRuntime", terms));
        Assert.Empty(UbcRules.VocabularyMatches("UbcInstructionTable", terms));
        Assert.Equal(["wasm (substring)"], UbcRules.VocabularyMatches("WasmModule", terms));
        Assert.Equal(["webassembly (substring)"], UbcRules.VocabularyMatches("IWebAssemblyHost", terms));
        Assert.Equal(["javascript (substring)"], UbcRules.VocabularyMatches("EcmaJavaScriptRealm", terms));
        Assert.Equal(["js (word)", "js. (prefix)"], UbcRules.VocabularyMatches("js.add", terms));
        Assert.Equal(["wasm (substring)", "wasm. (prefix)"], UbcRules.VocabularyMatches("wasm.i64.add", terms));

        // The one character whose lowering differs between the two runtimes is lowered as Python does.
        Assert.Equal("i̇", UbcRules.PythonLower("İ"));
    }

    [Fact]
    public void U2_Rejects_An_Exported_Identifier_Naming_A_Language()
    {
        // Read as the script's --identifiers mode reads a file: every non-blank line, stripped, is one
        // identifier. The witness therefore carries no comment - the script would scan it as a name.
        var names = UbcRules.ReadIdentifierList(Witness("U2-identifiers-naming-a-language.txt.witness"));

        var reported = UbcRules.U2Identifiers(
                names.Select(static (name, index) => new UbcRules.ExportedIdentifier("identifier", $"line {index + 1}", name)),
                UbcRules.ReadVocabulary(UbcRules.RootPath(UbcRules.VocabularyFile)))
            .ToArray();

        // Exactly the two the script reports over the same file, with the terms it names, and none of
        // the near misses: Json is not js, and a Ubc name is not a language's.
        Assert.Equal(2, reported.Length);
        Assert.Contains(reported, static message =>
            message.Contains("is named WasmModule", StringComparison.Ordinal) &&
            message.EndsWith("matches: wasm (substring)", StringComparison.Ordinal));
        Assert.Contains(reported, static message =>
            message.Contains("is named JsValue", StringComparison.Ordinal) &&
            message.EndsWith("matches: js (word)", StringComparison.Ordinal));
        Assert.Contains("JsonReader", names);
        Assert.Contains("UbcOpcode", names);
    }

    [Fact]
    public void U2_The_Assembly_Exports_No_Family_Row()
    {
        var surface = UbcApiSurface.Describe();

        Assert.Equal(UbcApiSurface.Assemblies.Length, UbcApiSurface.Found().Count);
        Assert.NotEmpty(surface);

        // Non-vacuous: the family-row types ARE on the surface - an instance property hands a hook its
        // family's table, and the table's schema is exported - so the clause is deciding over members
        // of the types it names rather than over a surface that never mentions them.
        Assert.Contains(surface, static line =>
            !line.StartsWith("type ", StringComparison.Ordinal) &&
            line.Contains(": Broiler.VM.Ubc.UbcInstructionTable { get; }", StringComparison.Ordinal));

        Assert.Empty(UbcRules.U2FamilyRows(surface));
    }

    [Fact]
    public void U2_Rejects_A_Static_Family_Table_On_The_Surface()
    {
        var reported = UbcRules.U2FamilyRows(Read(Witness("U2-a-static-family-table.txt.witness"))).ToArray();

        Assert.Contains(reported, static message =>
            message.Contains("UbcCommonFamilies.Fixture, a static property of type Broiler.VM.Ubc.UbcInstructionTable", StringComparison.Ordinal));

        Assert.Contains(reported, static message =>
            message.Contains("UbcCommonFamilies.FixtureRows, a static field of type System.Collections.Immutable.ImmutableArray<Broiler.VM.Ubc.UbcInstructionRow>", StringComparison.Ordinal));

        // And the two lines beside them that are NOT family rows stay unreported: an instance property
        // of the table's type, which is how a hook is handed a table, and a static method taking one,
        // which is schema rather than data.
        Assert.Equal(2, reported.Length);
    }

    [Fact]
    public void U2_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = Row("U2");

        Assert.Equal("Active", row.Status);
        Assert.Equal("0013", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        // The row must name the list and the script, because one list read in two places is the claim.
        Assert.Contains(UbcRules.VocabularyFile, row.Statement, StringComparison.Ordinal);
        Assert.Contains("eng/ubc-vocabulary-scan.py", row.Statement, StringComparison.Ordinal);
        Assert.Contains("family row", row.Statement, StringComparison.Ordinal);
    }

    // =============================================================================================
    // U4
    // =============================================================================================

    /// <summary>
    /// U4's first clause over the checkout, which reports one recorded disagreement.
    /// </summary>
    /// <remarks>
    /// The assertion is equality with <see cref="UbcRules.RecordedAppendixDisagreements"/>, not
    /// emptiness: the appendix and the table disagree about whether <c>jump_table</c> is terminal, the
    /// disagreement is reported rather than resolved, and neither the concept nor the table is edited to
    /// make this green. A second disagreement fails here, and so does resolving this one until the pin is
    /// removed with it.
    /// </remarks>
    [Fact]
    public void U4_The_Common_Family_Is_Appendix_A_Row_For_Row()
    {
        var problems = new List<string>();
        var appendix = UbcRules.ReadAppendix(File.ReadAllText(UbcRules.RootPath(UbcRules.ConceptFile)), problems);
        var table = UbcRules.ReadCommonTable(File.ReadAllText(UbcRules.RootPath(UbcRules.CommonTableFile)), problems);

        // Non-vacuous: both sides were read, whole. A reader that found no row on either side would
        // agree with an empty other side.
        Assert.Empty(problems);
        Assert.NotEmpty(appendix);
        Assert.Equal(appendix.Count, table.Count);
        Assert.Contains(appendix, static row => row.Mnemonic == "squash" && row.Shape == "U8U8");
        Assert.Contains(table, static row => row.Mnemonic == "const.f64" && row.Byte == 0x23);

        Assert.Equal(UbcRules.RecordedAppendixDisagreements, UbcRules.U4AppendixViolations());
    }

    [Fact]
    public void U4_Rejects_An_Appendix_That_Disagrees_With_The_Table()
    {
        // A copy of Appendix A with four perturbations, one per direction the clause decides: an
        // operand changed, an effect changed, a row the table does not have, and a row the table has
        // and the appendix dropped.
        var reported = UbcRules.U4Appendix(
            File.ReadAllText(Witness("U4-an-appendix-that-disagrees-with-the-table.md.witness")),
            File.ReadAllText(UbcRules.RootPath(UbcRules.CommonTableFile)));

        Assert.Contains("0x0C pick: Appendix A says operand is U16, the table says U8", reported);
        Assert.Contains("0x09 dup: Appendix A says effect is [t] → [t t t], the table says [t] → [t t]", reported);
        Assert.Contains("0x0F peek: Appendix A lists it and the table has no row for it", reported);
        Assert.Contains("0x23 const.f64: the table defines it and Appendix A does not list it", reported);
    }

    [Fact]
    public void U4_No_Source_But_The_One_Table_States_A_Common_Row()
    {
        var sources = UbcRules.U4Sources();

        // Non-vacuous: the sweep reads the universal bytecode itself, the one table's file among it -
        // the exclusion is by path, not by the file never being read - and a project that references the
        // assembly, because an encoder or a test is where a second table would be written.
        Assert.Contains(sources, static source => source.RelativePath == UbcRules.CommonTableFile);
        Assert.Contains(sources, static source => source.RelativePath == "src/Broiler.VM.Ubc/UbcVerifier.cs");
        Assert.Contains(sources, static source => source.Project == "Broiler.VM.Contract.Tests");

        // And the four enums a second table would be written in were read, so a using static of any
        // of them is followed rather than silently unmatched.
        var enums = UbcRules.TableEnums();

        foreach (var name in new[] { "UbcOpcode", "UbcCommonEffect", "UbcOperandShape", "UbcSlotType" })
        {
            Assert.NotEmpty(enums[name]);
        }

        Assert.Empty(UbcRules.U4SecondTables(sources, enums));
    }

    /// <summary>
    /// The roadmap's witness for U4 - a row perturbed, and the verifier and an encoder disagreeing - in
    /// the form a single-table rule takes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The witness is a second width table for the common family, written the way an encoder that did
    /// not read <c>UbcOpcodes</c> would write one, with one row perturbed: <c>jump</c> is three bytes
    /// there and five in the one table, so an encoder reading it writes a jump whose target the verifier
    /// reads two bytes into the next instruction. The test shows the perturbation is real - every other
    /// row agrees with the one table and exactly that one does not - and that the scan reports the
    /// table, at a path in a project that references the assembly.
    /// </para>
    /// <para>
    /// <b>The scan reports the table, not the perturbation, and that is the point.</b> A copy that
    /// agrees with the one table today is a copy that disagrees with it the day a row changes, so the
    /// rule reports every arm of the witness, the unperturbed ones included. And the same text read at
    /// the one table's own path is not reported, which is the exclusion working; the one table's own
    /// text read at another path is reported row by row, which is the scan recognising the form the one
    /// table is really written in.
    /// </para>
    /// </remarks>
    [Fact]
    public void U4_Rejects_A_Second_Width_Table_With_One_Row_Perturbed()
    {
        var text = File.ReadAllText(Witness("U4-a-second-width-table-with-one-row-perturbed.cs.witness"));
        var enums = UbcRules.TableEnums();

        var problems = new List<string>();
        var oneTable = UbcRules.OneTableWidths(problems);
        var stated = UbcRules.StatedWidths(text);

        Assert.Empty(problems);
        Assert.Equal(enums["UbcOpcode"].Order(StringComparer.Ordinal), stated.Keys.Order(StringComparer.Ordinal));
        Assert.Equal(["Jump"], stated.Where(width => oneTable[width.Key] != width.Value).Select(static width => width.Key));

        var reported = UbcRules.U4SecondTables(
                [new UbcRules.UbcSource("src/tests/Broiler.VM.Contract.Tests/UbcEncodingWidths.cs", "Broiler.VM.Contract.Tests", text)],
                enums)
            .ToArray();

        Assert.Contains(reported, static message =>
            message.Contains("a switch arm keyed on UbcOpcode.Jump states an integer literal (3)", StringComparison.Ordinal));
        Assert.Equal(stated.Count, reported.Length);

        Assert.Empty(UbcRules.U4SecondTables(
            [new UbcRules.UbcSource(UbcRules.CommonTableFile, UbcRules.UbcAssembly, text)],
            enums));

        var copied = UbcRules.U4SecondTables(
                [new UbcRules.UbcSource(
                    "src/Broiler.VM.Ubc/UbcCommonCopy.cs",
                    UbcRules.UbcAssembly,
                    File.ReadAllText(UbcRules.RootPath(UbcRules.CommonTableFile)))],
                enums)
            .ToArray();

        foreach (var opcode in enums["UbcOpcode"])
        {
            Assert.Contains(copied, message => message.Contains($"keyed on UbcOpcode.{opcode} ", StringComparison.Ordinal));
        }

        // The name-following the register row claims, one spelling per line: an alias, a using static,
        // a unicode escape, and a global alias declared in another file of the same project.
        const string Project = "Broiler.VM.Contract.Tests";
        const string Expected = "keyed on UbcOpcode.Jump states an integer literal (5)";

        foreach (var spelling in new[]
                 {
                     "using Op = Broiler.VM.Ubc.UbcOpcode; static class C { static int W(Op o) => o switch { Op.Jump => 5, _ => 0 }; }",
                     "using static Broiler.VM.Ubc.UbcOpcode; static class C { static int W(Broiler.VM.Ubc.UbcOpcode o) => o switch { Jump => 5, _ => 0 }; }",
                     "static class C { static int W(Broiler.VM.Ubc.UbcOpcode o) => o switch { Broiler.VM.Ubc.\\u0055bcOpcode.Jump => 5, _ => 0 }; }",
                 })
        {
            Assert.Contains(
                UbcRules.U4SecondTables([new UbcRules.UbcSource("src/tests/Broiler.VM.Contract.Tests/C.cs", Project, spelling)], enums),
                message => message.Contains(Expected, StringComparison.Ordinal));
        }

        Assert.Contains(
            UbcRules.U4SecondTables(
                [
                    new UbcRules.UbcSource("src/tests/Broiler.VM.Contract.Tests/Usings.cs", Project, "global using Op = Broiler.VM.Ubc.UbcOpcode;"),
                    new UbcRules.UbcSource("src/tests/Broiler.VM.Contract.Tests/C.cs", Project, "static class C { static int W(Op o) => o switch { Op.Jump => 5, _ => 0 }; }"),
                ],
                enums),
            message => message.Contains(Expected, StringComparison.Ordinal));
    }

    [Fact]
    public void U4_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = Row("U4");

        Assert.Equal("Active", row.Status);
        Assert.Equal("0013", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        Assert.Contains("Appendix A", row.Statement, StringComparison.Ordinal);
        Assert.Contains(UbcRules.CommonTableFile, row.Statement, StringComparison.Ordinal);

        // The recorded disagreement must be in the row, because a rule whose clean direction is pinned to
        // a non-empty answer is a rule whose row has to say what that answer is.
        Assert.Contains("jump_table", row.NonVacuousWhen, StringComparison.Ordinal);

        // And the row must say why the roadmap's witness takes the form it does here.
        Assert.Contains("encoder", row.NonVacuousWhen, StringComparison.Ordinal);
    }

    // =============================================================================================
    // U9
    // =============================================================================================

    [Fact]
    public void U9_The_Ubc_Surface_Is_Exactly_What_Its_Baseline_Declares()
    {
        var surface = UbcApiSurface.Describe();

        // A run that has not built the assembly describes nothing, and an empty surface compared
        // against an empty baseline would agree. The rule fails on it instead.
        Assert.Equal(UbcApiSurface.Assemblies.Length, UbcApiSurface.Found().Count);
        Assert.NotEmpty(surface);

        if (Writing)
        {
            Write(surface);
            return;
        }

        var violations = U9Violations(surface);

        Assert.True(
            violations.Count == 0,
            $"The universal bytecode's public surface and {BaselineName} disagree in " +
            $"{violations.Count} places. Review each, then regenerate with `{WriteSwitch}=1 " +
            $"dotnet test Broiler.VM.slnx -c Release`:{Environment.NewLine}" +
            string.Join(Environment.NewLine, violations.Take(40)));
    }

    [Fact]
    public void U9_Rejects_A_Baseline_That_Omits_An_Exported_Member()
    {
        var violations = Violations(
            UbcApiSurface.Describe(),
            Read(Witness("U9-baseline-omits-an-exported-member.txt.witness", "api")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("exported but not declared:", StringComparison.Ordinal) &&
            violation.Contains("Broiler.VM.Ubc.UbcOpcodes.TryDescribe(", StringComparison.Ordinal));
    }

    [Fact]
    public void U9_Rejects_A_Baseline_Declaring_A_Member_That_Is_Gone()
    {
        var violations = Violations(
            UbcApiSurface.Describe(),
            Read(Witness("U9-baseline-declares-a-member-that-is-gone.txt.witness", "api")));

        Assert.Contains(violations, violation =>
            violation.StartsWith("declared but not exported:", StringComparison.Ordinal) &&
            violation.Contains("Broiler.VM.Ubc.UbcOpcodes.OperandWidth(", StringComparison.Ordinal));
    }

    /// <summary>
    /// This baseline and the other three are disjoint subjects.
    /// </summary>
    /// <remarks>
    /// A surface frozen in two files is a surface whose two records can disagree, which is the reason
    /// the universal bytecode names its own list rather than widening the packable one or a family's.
    /// </remarks>
    [Fact]
    public void U9_Covers_The_Ubc_Assembly_And_Nothing_Else()
    {
        var assemblies = UbcApiSurface.Describe()
            .Where(static line => line.StartsWith("type ", StringComparison.Ordinal))
            .Select(static line => line.Split(' ')[1])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(UbcApiSurface.Assemblies.OrderBy(static name => name, StringComparer.Ordinal), assemblies);

        Assert.Empty(UbcApiSurface.Assemblies.Intersect(ApiSurface.PackableAssemblies, StringComparer.Ordinal));
        Assert.Empty(UbcApiSurface.Assemblies.Intersect(ProfileApiSurface.FamilyAssemblies, StringComparer.Ordinal));
        Assert.Empty(UbcApiSurface.Assemblies.Intersect(WebAssemblyApiSurface.FamilyAssemblies, StringComparer.Ordinal));
    }

    [Fact]
    public void U9_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = Row("U9");

        Assert.Equal("Active", row.Status);
        Assert.Equal("0013", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(BaselineName, row.Evidence, StringComparison.Ordinal);

        // The row must state the limit rather than claim a package surface: this baseline is over a
        // build output, and the assembly does not pack.
        Assert.Contains("build output", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
    }

    // =============================================================================================
    // The group report
    // =============================================================================================

    /// <summary>Writes what each group U rule said about this checkout, when asked to.</summary>
    /// <remarks>
    /// U4's report is not silent on this checkout: it carries the recorded disagreement about
    /// <c>jump_table</c>, which is what the rule says and what its test pins.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_U_Are_Written_When_Asked_For()
    {
        RuleReport.Write("U",
        [
            ("U1", () => ComponentGraph.Projects.SelectMany(UbcRules.U1)),
            ("U2", () => UbcRules.U2Identifiers(
                    UbcApiSurface.Identifiers(),
                    UbcRules.ReadVocabulary(UbcRules.RootPath(UbcRules.VocabularyFile)))
                .Concat(UbcRules.U2FamilyRows(UbcApiSurface.Describe()))),
            ("U4", () => UbcRules.U4AppendixViolations()
                .Concat(UbcRules.U4SecondTables(UbcRules.U4Sources(), UbcRules.TableEnums()))),
            ("U9", () => Writing ? [] : U9Violations(UbcApiSurface.Describe())),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "U.txt")),
                "a report for group U was asked for and none was written");
        }
    }

    // =============================================================================================
    // Helpers
    // =============================================================================================

    private static RuleRegisterTests.Rule Row(string id) =>
        RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, id, StringComparison.Ordinal));

    private static List<string> U9Violations(IReadOnlyList<string> surface)
    {
        var found = UbcApiSurface.Found().Count;

        if (found != UbcApiSurface.Assemblies.Length)
        {
            return
            [
                $"the describer found {found} of the universal bytecode's {UbcApiSurface.Assemblies.Length} " +
                "assemblies on disk, so this rule compared a partial surface",
            ];
        }

        return surface.Count == 0
            ? ["the describer produced no surface, so this rule compared nothing"]
            : Violations(surface, Read(BaselinePath()));
    }

    private static List<string> Violations(IEnumerable<string> surface, IEnumerable<string> baseline)
    {
        var exported = surface.ToHashSet(StringComparer.Ordinal);
        var declared = baseline.ToHashSet(StringComparer.Ordinal);

        var violations = exported
            .Where(line => !declared.Contains(line))
            .Select(static line => "exported but not declared: " + line.Trim())
            .ToList();

        violations.AddRange(declared
            .Where(line => !exported.Contains(line))
            .Select(static line => "declared but not exported: " + line.Trim()));

        violations.Sort(StringComparer.Ordinal);
        return violations;
    }

    private static IEnumerable<string> Read(string path) => File
        .ReadAllLines(path)
        .Where(static line => line.Length > 0 && !line.StartsWith('#'));

    private static void Write(IEnumerable<string> surface)
    {
        var path = BaselinePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var text = new System.Text.StringBuilder();

        text.Append("# The frozen public surface of Broiler.VM.Ubc, the universal bytecode: its\n");
        text.Append("# container, the common family, the family table schema, the primitive table,\n");
        text.Append("# the contracts a family implements and the verifier walk. IT DOES NOT PACK -\n");
        text.Append("# rule U1 holds IsPackable false and no PackageId, and whether it becomes a\n");
        text.Append("# package is milestone UBC-9's decision - so this file freezes what a family,\n");
        text.Append("# an emitter or a composition root in this repository can bind to, not what a\n");
        text.Append("# consumer outside it can. The packable three are frozen in docs/api/ and each\n");
        text.Append("# profile family in its own docs/api/.\n");
        text.Append("#\n");
        text.Append("# GENERATED - regenerate with:\n");
        text.Append("#   BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release\n");
        text.Append("# Rule U9 asserts it otherwise.\n");
        text.Append("#\n");
        text.Append("# Described from the build output by MetadataLoadContext, which reflects\n");
        text.Append("# without running anything: the architecture tests do not reference this\n");
        text.Append("# assembly, and loading it would run code the describer has no reason to run.\n");
        text.Append("\n");

        foreach (var line in surface)
        {
            text.Append(line).Append('\n');
        }

        File.WriteAllText(path, text.ToString(), AssuranceSources.Utf8NoBom);
    }

    private static string BaselinePath() => UbcRules.RootPath(BaselineName);

    private static string Witness(string fileName, string? directory = null) =>
        directory is null
            ? Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", fileName)
            : Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", directory, fileName);
}
