namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W3: the WebAssembly profile's published diagnostic registry, held in both directions to the
/// enumeration, the emitting source, the corpus, the execution checks and the corpus writer.
/// </summary>
/// <remarks>
/// Each clause is asserted twice, as every group here is: the checkout is clean, and the clause
/// rejects a violating input. The violating inputs are a witness per clause, asserted on the content
/// of its messages, and the real inputs with one thing altered, which is the rule reaching the files
/// that ship rather than small files shaped like them.
/// </remarks>
public sealed class WebAssemblyRegistryRuleTests
{
    private static readonly WebAssemblyRegistryRules.Registry Registry = WebAssemblyRegistryRules.ReadRegistry(
        File.ReadAllText(UbcRules.RootPath(WebAssemblyRegistryRules.RegistryFile)), WebAssemblyRegistryRules.RegistryFile);

    private static readonly IReadOnlyList<(string Name, int Value)> Vocabulary = ReadVocabulary();

    private static readonly WebAssemblyRegistryRules.Emissions Emissions =
        WebAssemblyRegistryRules.ReadEmissions(WebAssemblyRegistryRules.ProfileSourceFiles());

    private static readonly string ManifestText =
        File.ReadAllText(UbcRules.RootPath(WebAssemblyRegistryRules.CorpusManifestFile));

    private static readonly IReadOnlyList<WebAssemblyRegistryRules.CorpusEntry> Corpus = ReadCorpus(ManifestText);

    private static readonly IReadOnlySet<string> CheckedKinds = WebAssemblyRegistryRules.NamedTrapKinds(
        WebAssemblyRegistryRules.Parse(WebAssemblyRegistryRules.ExecutionChecksFile));

    private static readonly IReadOnlyList<WebAssemblyRegistryRules.PayloadArm> CheckedCodes = ReadCheckedCodes();

    private static readonly IReadOnlyList<WebAssemblyRegistryRules.HookCheck> HookChecks = WebAssemblyRegistryRules.HookChecks(
        WebAssemblyRegistryRules.Parse(WebAssemblyRegistryRules.HookChecksFile));

    private static readonly IReadOnlyDictionary<string, int> CoreReasons = WebAssemblyRegistryRules.CoreReasons();

    [Fact]
    public void W3_The_Checkout_Holds_Every_Clause()
    {
        Assert.Empty(WebAssemblyRegistryRules.W3Violations());
    }

    [Fact]
    public void W3_The_Registry_And_The_Code_Vocabulary_Are_The_Same_Set()
    {
        Assert.Empty(WebAssemblyRegistryRules.W3Vocabulary(Registry, Vocabulary));

        // Non-vacuous: both sides were read whole. Ninety-six members from the preamble's 2001 to the
        // last trap, 3009, and a row for each at the registry's first revision.
        Assert.Empty(Registry.Problems);
        Assert.Equal(96, Vocabulary.Count);
        Assert.Equal(Vocabulary.Count, Registry.Rows.Count);
        Assert.Equal(1, Registry.Revision);
        Assert.Contains(Vocabulary, static member => member is ("WrongMagic", 2001));
        Assert.Contains(Vocabulary, static member => member is ("TrapUninitializedElement", 3009));

        var reported = WebAssemblyRegistryRules.W3Vocabulary(Read("W3-registry-omits-a-declared-code.txt.witness"), Vocabulary).ToArray();

        Assert.Contains(reported, static message => message.Contains(
            "WebAssemblyDiagnosticCode declares UnsupportedBinaryVersion = 2002 and the registry has no row for it", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.Contains(
            "the row for 2002 names UnsupportedBinaryVersionExtended, which is not a member of that number", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.Contains(
            "the row for 2101 dates from revision 2, and the registry is at revision 1", StringComparison.Ordinal));

        // The real registry with one row taken out reports exactly that member...
        Assert.Equal(
            ["WebAssemblyDiagnosticCode declares DataCountMismatch = 2502 and the registry has no row for it"],
            WebAssemblyRegistryRules.W3Vocabulary(
                Registry with { Rows = Registry.Rows.Where(static row => row.Code != 2502).ToArray() }, Vocabulary));

        // ...a row given twice is reported as twice, and a registry that states no revision is
        // reported rather than read as one that happens to say nothing.
        Assert.Contains(
            WebAssemblyRegistryRules.W3Vocabulary(Registry with { Rows = [.. Registry.Rows, Registry.Rows[0]] }, Vocabulary),
            static message => message.Contains("the registry has 2 rows for code 2001", StringComparison.Ordinal));
        Assert.Contains(
            WebAssemblyRegistryRules.W3Vocabulary(Registry with { Revision = -1 }, Vocabulary),
            static message => message.Contains("states no revision of its own", StringComparison.Ordinal));
    }

    [Fact]
    public void W3_Every_Row_States_Its_Passes_Carrier_And_Reason_In_The_Registrys_Words()
    {
        Assert.Empty(WebAssemblyRegistryRules.W3Columns(Registry, CoreReasons));

        // Non-vacuous: every carrier and every pass is used by a real row, so the carrier clause
        // compares real sets. Codes the hook shares with a pass that reads the module travel on two.
        Assert.Contains(Registry.Rows, static row => row.Carrier == "translation");
        Assert.Contains(Registry.Rows, static row => row.Carrier == "outcome");
        Assert.Contains(Registry.Rows, static row => row.Carrier == "payload");
        Assert.Contains(Registry.Rows, static row => row is { Code: 2302, Pass: "decode+validate+hook", Carrier: "translation+outcome" });

        var reported = WebAssemblyRegistryRules.W3Columns(Read("W3-registry-rows-that-misstate-their-columns.txt.witness"), CoreReasons).ToArray();

        foreach (var expected in new[]
                 {
                     "the row for 2001 WrongMagic names its passes as validate+decode rather than decode+validate",
                     "the row for 2102 SectionOutOfOrder names the pass parse, which is not one",
                     "the row for 2302 UnknownValueType names the carrier translation, and its passes travel on translation+outcome",
                     "the row for 2303 ValueTypeNotAdmitted names the reason MalformedBytes, which is not a member of VmReason",
                     "the row for 2101 UnknownSectionId names Cancelled, which is not an invalid-artifact reason",
                     "the row for 3001 TrapUnreachable travels in the payload and names MalformedEncoding",
                     "the row for 2003 UnsupportedArtifactFormatVersion names the retired bare-module verifier and is not retired",
                     "the row for 2403 ImportNotAdmitted claims corpus and names no case",
                 })
        {
            Assert.Contains(reported, message => message.Contains(expected, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void W3_Every_Code_Maps_Onto_Exactly_One_Core_Reason_In_The_Passes_Its_Row_Names()
    {
        Assert.Empty(WebAssemblyRegistryRules.W3Reasons(Registry, Vocabulary, Emissions));

        // Non-vacuous: the sites are read out of the assembly's own sources, there are more of them
        // than codes, every one has a reason the rule read, nothing naming the code type was left
        // unfollowed, and both admitted uses were found where the rule lists them.
        Assert.True(Emissions.Sites.Count > Vocabulary.Count);
        Assert.DoesNotContain(Emissions.Sites, static site => site.Reason == WebAssemblyRegistryRules.NoReason);
        Assert.Empty(Emissions.Unreadable);
        Assert.Equal(WebAssemblyRegistryRules.AdmittedUses.Length, Emissions.Admitted.Distinct().Count());

        // Every shape the rule follows is read, each shown by a code only that shape emits: the
        // validator's Fail and PopOperand helpers, the hook's reason table by an arm and by its discard
        // arm, the lowering's Refuse - which shares a name with the hook's and supplies another reason -
        // the decoder and validator's shared mapping, and the family's payload mapping.
        foreach (var (code, reason, file) in new[]
                 {
                     ("FunctionTypeIndexOutOfRange", "SemanticValidationFailed", "WasmValidator.cs"),
                     ("OperandStackUnderflow", "SemanticValidationFailed", "WasmValidator.cs"),
                     ("ModuleDefinitionsTruncated", "Truncated", "WasmFamilyData.cs"),
                     ("PositionsNotOrdered", "InconsistentStructure", "WasmFamilyData.cs"),
                     ("TranslationLocalsAboveMaximum", "UnknownFeature", "WasmLowering.cs"),
                     ("IntegerTooLarge", "MalformedEncoding", "WebAssemblyDiagnostics.cs"),
                     ("TrapUnreachable", "ProfileFaultUnspecified", "WasmFamily.cs"),
                 })
        {
            Assert.Equal(
                [reason],
                Emissions.Sites.Where(site => site.Code == code).Select(static site => site.Reason).Distinct());
            Assert.Contains(Emissions.Sites, site => site.Code == code && site.File == "src/Broiler.VM.Profile.WebAssembly/" + file);
        }

        // The witness: one code with three reasons, the second through a helper and the third through a
        // reason table. Read alone, it is exactly three sites in source order.
        var witness = AssuranceSources.ReadFile(
            WitnessPath("W3-a-source-emitting-one-code-with-three-reasons.cs.witness"), WebAssemblyFamilyRules.ProfileAssembly);

        Assert.Equal(
            [("WrongMagic", "MalformedEncoding"), ("WrongMagic", "InconsistentStructure"), ("WrongMagic", "Truncated")],
            WebAssemblyRegistryRules.ReadEmissions([witness]).Sites.Select(static site => (site.Code, site.Reason)));

        // Read beside the real files, the hook's own table is still the one its helper answers with -
        // the witness's table of the same name does not replace it - and the code is reported with its
        // three reasons.
        var beside = WebAssemblyRegistryRules.ReadEmissions([.. WebAssemblyRegistryRules.ProfileSourceFiles(), witness]);
        var reported = WebAssemblyRegistryRules.W3Reasons(Registry, Vocabulary, beside).ToArray();

        Assert.Equal(
            ["Truncated"],
            beside.Sites.Where(static site => site.Code == "ModuleDefinitionsTruncated").Select(static site => site.Reason).Distinct());
        Assert.Contains(reported, static message => message.StartsWith("WrongMagic is emitted with 3 reasons: ", StringComparison.Ordinal));
        Assert.Contains(reported, static message => message.Contains(
            "the reason table WasmMagicWitness.ReasonOf gives WrongMagic the reason Truncated, and the registry says MalformedEncoding",
            StringComparison.Ordinal));

        // The defect this rule was minted beside: a body declared past the payload's end carried
        // FunctionBodyLengthMismatch with Truncated while its locals check carried the code with
        // InconsistentStructure. Put back, it is one code with two reasons.
        var doubled = Emissions with
        {
            Sites = [.. Emissions.Sites, new("src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs", 1, "FunctionBodyLengthMismatch", "Truncated")],
        };

        Assert.Contains(
            WebAssemblyRegistryRules.W3Reasons(Registry, Vocabulary, doubled),
            static message => message.StartsWith("FunctionBodyLengthMismatch is emitted with 2 reasons", StringComparison.Ordinal));

        // The registry's side: a pass the row leaves out, a reason the hook's table contradicts, a
        // retired row whose code the source still names, and a row nothing emits.
        var doctored = Registry with
        {
            Rows =
            [
                .. Registry.Rows.Select(static row => row.Code switch
                {
                    2302 => row with { Pass = "decode+validate", Carrier = "translation" },
                    2304 => row with { Reason = "InconsistentStructure" },
                    2001 => row with { Reachability = "retired", Case = "-" },
                    _ => row,
                }),
                new(0, 2999, "NeverEmitted", "decode", "translation", "MalformedEncoding", "unreached", "-", 1),
            ],
        };

        var disagreements = WebAssemblyRegistryRules.W3Reasons(doctored, [.. Vocabulary, ("NeverEmitted", 2999)], Emissions).ToArray();

        foreach (var expected in new[]
                 {
                     "the row for 2302 UnknownValueType names the passes decode+validate, and the source emits it in decode+validate+hook",
                     "the reason table WasmFamilyVerifier.ReasonOf gives MalformedLimitsFlag the reason MalformedEncoding, and the registry says InconsistentStructure",
                     "the row for 2001 WrongMagic is retired, and src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs(",
                     "NeverEmitted is declared and nothing in Broiler.VM.Profile.WebAssembly emits it",
                 })
        {
            Assert.Contains(disagreements, message => message.Contains(expected, StringComparison.Ordinal));
        }
    }

    /// <summary>
    /// The reading of the source fails closed: every way of writing an emission the rule cannot read
    /// the reason of is a message, so a second spelling of the same emission cannot hide a second reason.
    /// </summary>
    [Fact]
    public void W3_Reports_An_Emission_It_Cannot_Read_Rather_Than_Skipping_It()
    {
        foreach (var (text, expected) in new[]
                 {
                     ("static class C { static object M() { var code = WebAssemblyDiagnosticCode.WrongMagic; return code; } }",
                         "uses WebAssemblyDiagnosticCode.WrongMagic outside the argument list of a call"),
                     ("using static Broiler.VM.Profile.WebAssembly.WebAssemblyDiagnosticCode;",
                         "names WebAssemblyDiagnosticCode in a using static directive"),
                     ("using Code = Broiler.VM.Profile.WebAssembly.WebAssemblyDiagnosticCode;",
                         "names WebAssemblyDiagnosticCode in an alias directive"),
                     ("static class C { static VmVerifierOutcome M() => WasmRefusal.Invalid(VmReason.MalformedEncoding, (WebAssemblyDiagnosticCode)2001, default); }",
                         "names WebAssemblyDiagnosticCode in a cast"),
                     ("static class C { static VmVerifierOutcome M(VmReason reason) => WasmRefusal.Invalid(reason, WebAssemblyDiagnosticCode.WrongMagic, default); }",
                         "emits WrongMagic with no reason the rule can read"),
                     ("static class C { static VmVerifierOutcome M(bool late) => WasmRefusal.Invalid(late ? VmReason.Truncated : VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.WrongMagic, default); }",
                         "WrongMagic is emitted with 2 reasons"),
                     ("static class C { static VmReason M(WebAssemblyDiagnosticCode code) { return VmReason.Truncated; } }",
                         "which is not a switch over the code the rule can read"),
                 })
        {
            var file = new AssuranceSourceFile(
                "src/Broiler.VM.Profile.WebAssembly/C.cs", "src/Broiler.VM.Profile.WebAssembly/C.cs",
                WebAssemblyFamilyRules.ProfileAssembly, text, "\n",
                AssuranceSources.Parse(text, "src/Broiler.VM.Profile.WebAssembly/C.cs"));

            Assert.Contains(
                WebAssemblyRegistryRules.W3Reasons(
                    Registry, Vocabulary, WebAssemblyRegistryRules.ReadEmissions([.. WebAssemblyRegistryRules.ProfileSourceFiles(), file])),
                message => message.Contains(expected, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void W3_Every_Row_Is_Reachable_From_A_Named_Case_Or_Listed_As_Unreached()
    {
        Assert.Empty(WebAssemblyRegistryRules.W3Reachability(
            Registry, Corpus, CheckedKinds, Emissions.Payload, CheckedCodes, HookChecks, WebAssemblyRegistryRules.Unreached));

        // Non-vacuous, and the figures that matter: sixty-four rows name a derived corpus entry, twelve
        // name a check of the hook lane, eight name a trap kind the execution checks expect, four are
        // retired, and eight are the rows the RULE lists as unreached - which is the count this file
        // fixes, not the registry. Those eight are the part of WA-3's reachability clause that is not
        // met: the translation's bounds, the two defect codes, the reader's malformation, and the trap
        // the earlier specification revision named. The twelve hook rows stood among them until
        // 2026-09-29, when the hook lane gave each a check of its own.
        Assert.Equal(64, Registry.Rows.Count(static row => row.Reachability == "corpus"));
        Assert.Equal(12, Registry.Rows.Count(static row => row.Reachability == "hook-check"));
        Assert.Equal(8, Registry.Rows.Count(static row => row.Reachability == "execution"));
        Assert.Equal(4, Registry.Rows.Count(static row => row.Reachability == "retired"));
        Assert.Equal(8, WebAssemblyRegistryRules.Unreached.Length);
        Assert.Equal(8, Registry.Rows.Count(static row => row.Reachability == "unreached"));
        Assert.True(Corpus.Count > Registry.Rows.Count);

        // The hook lane was read whole: its twelve refusals and its three controls, each control
        // expecting the translator's own artifact, rewritten, to verify.
        Assert.Equal(15, HookChecks.Count);
        Assert.Equal(3, HookChecks.Count(static check => check is { Outcome: "Normal", Code: null }));

        var reported = WebAssemblyRegistryRules.W3Reachability(
                Read("W3-registry-names-a-case-the-corpus-does-not-have.txt.witness"),
                Corpus, CheckedKinds, Emissions.Payload, CheckedCodes, HookChecks, WebAssemblyRegistryRules.Unreached)
            .ToArray();

        foreach (var expected in new[]
                 {
                     "the row for 2001 WrongMagic names the entry a-corpus-entry-nobody-wrote, and the corpus manifest has no entry of that name",
                     "the row for 2002 UnsupportedBinaryVersion names the entry preamble-wrong-magic, which records InvalidArtifact with code 2001",
                     "the row for 2106 Truncated names the entry inversion-0071, whose answer is recorded rather than derived",
                     "the row for 2105 TagSectionNotAdmitted claims no named case reaches it, and is not one of the rows this rule lists",
                     "the corpus entry feature-a-tag-section records 2105 TagSectionNotAdmitted, and the registry says it is unreached",
                     "the row for 2901 VerifierDefect says why no named case reaches it in words the rule's list does not",
                     "the row for 2107 ReaderMalformedEncoding is listed by the rule as unreached, and claims corpus",
                     "the row for 3001 TrapUnreachable names WasmTrapKind.IntegerOverflow, which the payload mapping maps onto TrapIntegerOverflow",
                     "the row for 3002 TrapIntegerDivideByZero names WasmTrapKind.UndefinedElement, and no execution check expects a trap of that kind",
                     "the row for 2851 ModuleDefinitionsMissing names the check hook-a-check-nobody-wrote, and the hook lane has no check of that name",
                     "the row for 2852 ModuleDefinitionsTruncated names the check hook-module-definitions-missing, which expects InvalidArtifact with InconsistentStructure and ModuleDefinitionsMissing",
                 })
        {
            Assert.Contains(reported, message => message.Contains(expected, StringComparison.Ordinal));
        }

        // The real inputs with one thing altered: an entry recording a code with another reason than its
        // row's, and execution checks that stopped expecting a trap kind.
        var drifted = Corpus
            .Select(static entry => entry.Name == "preamble-wrong-magic" ? entry with { Reason = "Truncated" } : entry)
            .ToArray();

        Assert.Contains(
            WebAssemblyRegistryRules.W3Reachability(Registry, drifted, CheckedKinds, Emissions.Payload, CheckedCodes, HookChecks, WebAssemblyRegistryRules.Unreached),
            static message => message.Contains(
                "the corpus entry preamble-wrong-magic records 2001 WrongMagic as InvalidArtifact with Truncated, and the registry says InvalidArtifact with MalformedEncoding",
                StringComparison.Ordinal));

        Assert.Contains(
            WebAssemblyRegistryRules.W3Reachability(
                Registry, Corpus, CheckedKinds.Where(static kind => kind != "Unreachable").ToHashSet(StringComparer.Ordinal),
                Emissions.Payload, CheckedCodes, HookChecks, WebAssemblyRegistryRules.Unreached),
            static message => message.Contains(
                "the row for 3001 TrapUnreachable names WasmTrapKind.Unreachable, and no execution check expects a trap of that kind",
                StringComparison.Ordinal));

        // A hook check that expects a code the registry has no row for is reported, as a corpus entry's is.
        Assert.Contains(
            WebAssemblyRegistryRules.W3Reachability(
                Registry, Corpus, CheckedKinds, Emissions.Payload, CheckedCodes,
                [.. HookChecks, new("hook-an-invented-refusal", "InvalidArtifact", "InconsistentStructure", "NoSuchCode")],
                WebAssemblyRegistryRules.Unreached),
            static message => message.Contains(
                "the hook check hook-an-invented-refusal expects NoSuchCode, which the registry has no row for", StringComparison.Ordinal));

        // The execution checks read a trap's code against a table of their own: one arm per execution
        // row, and a discard arm naming no row's code. A table that forgot a kind, or mapped it onto
        // another code, is a check that would pass a payload carrying the wrong code.
        Assert.Equal(8, CheckedCodes.Count(static arm => arm.Kind is not null));

        var forgetful = CheckedCodes.Where(static arm => arm.Kind != "Unreachable").ToArray();
        var crossed = CheckedCodes
            .Select(static arm => arm.Kind == "IntegerOverflow" ? arm with { Code = "TrapIntegerDivideByZero" } : arm)
            .ToArray();

        Assert.Contains(
            WebAssemblyRegistryRules.W3Reachability(Registry, Corpus, CheckedKinds, Emissions.Payload, forgetful, HookChecks, WebAssemblyRegistryRules.Unreached),
            static message => message.Contains(
                "the row for 3001 TrapUnreachable names WasmTrapKind.Unreachable, and the execution checks' table of codes has no arm for it",
                StringComparison.Ordinal));
        Assert.Contains(
            WebAssemblyRegistryRules.W3Reachability(Registry, Corpus, CheckedKinds, Emissions.Payload, crossed, HookChecks, WebAssemblyRegistryRules.Unreached),
            static message => message.Contains(
                "the row for 3003 TrapIntegerOverflow names WasmTrapKind.IntegerOverflow, which the execution checks' table of codes maps onto TrapIntegerDivideByZero",
                StringComparison.Ordinal));
    }

    [Fact]
    public void W3_The_Corpus_Manifest_Is_Dated_By_The_Registrys_Revision()
    {
        var writer = WebAssemblyRegistryRules.WriterRevision(WebAssemblyRegistryRules.Parse(WebAssemblyRegistryRules.CorpusWriterFile));

        Assert.Empty(WebAssemblyRegistryRules.W3Revision(Registry.Revision, WebAssemblyRegistryRules.ManifestRevision(ManifestText), writer));

        // Non-vacuous: the manifest states a revision and the writer has a constant, both read as 1.
        Assert.Equal(1, WebAssemblyRegistryRules.ManifestRevision(ManifestText));
        Assert.Equal(1, writer);

        var witness = File.ReadAllText(WitnessPath("W3-a-corpus-manifest-dated-past-the-registry.txt.witness"));

        Assert.Contains(
            WebAssemblyRegistryRules.W3Revision(Registry.Revision, WebAssemblyRegistryRules.ManifestRevision(witness), writer),
            static message => message.Contains(
                "the corpus manifest is dated by registry revision 2, and the registry is at revision 1", StringComparison.Ordinal));

        // The real manifest with its date taken out, and a writer that would date the next one with a
        // revision the registry is not at.
        var undated = string.Join('\n', ManifestText.Split('\n').Where(static line => !line.StartsWith("# registry-revision:", StringComparison.Ordinal)));

        Assert.Contains(
            WebAssemblyRegistryRules.W3Revision(Registry.Revision, WebAssemblyRegistryRules.ManifestRevision(undated), writer),
            static message => message.Contains("the corpus manifest states no registry revision", StringComparison.Ordinal));
        Assert.Contains(
            WebAssemblyRegistryRules.W3Revision(Registry.Revision, 1, 2),
            static message => message.Contains(
                "the corpus writer dates a manifest with registry revision 2, and the registry is at revision 1", StringComparison.Ordinal));
    }

    [Fact]
    public void W3_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(static rule => string.Equals(rule.Id, "W3", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);

        // The row must name what the rule cannot do as well as what it does: it lists the unreached
        // rows itself, and it reads source as syntax.
        Assert.Contains("both directions", row.Statement, StringComparison.Ordinal);
        Assert.Contains("unreached", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);

        foreach (var witness in Directory.GetFiles(Path.GetDirectoryName(WitnessPath("x"))!, "W3-*.witness"))
        {
            Assert.Contains("witnesses/diagnostics/" + Path.GetFileName(witness), row.Witness, StringComparison.Ordinal);
        }
    }

    private static IReadOnlyList<(string Name, int Value)> ReadVocabulary()
    {
        var problems = new List<string>();
        var vocabulary = WebAssemblyRegistryRules.Vocabulary(AssuranceSources.File(WebAssemblyRegistryRules.DiagnosticsFile).Tree, problems);

        Assert.Empty(problems);
        return vocabulary;
    }

    private static IReadOnlyList<WebAssemblyRegistryRules.CorpusEntry> ReadCorpus(string text)
    {
        var problems = new List<string>();
        var corpus = WebAssemblyRegistryRules.ReadCorpus(text, problems);

        Assert.Empty(problems);
        return corpus;
    }

    private static IReadOnlyList<WebAssemblyRegistryRules.PayloadArm> ReadCheckedCodes()
    {
        var problems = new List<string>();
        var arms = WebAssemblyRegistryRules.CheckedCodes(WebAssemblyRegistryRules.ExecutionChecksFile, problems);

        Assert.Empty(problems);
        return arms;
    }

    private static WebAssemblyRegistryRules.Registry Read(string witness) =>
        WebAssemblyRegistryRules.ReadRegistry(File.ReadAllText(WitnessPath(witness)), witness);

    private static string WitnessPath(string fileName) => Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "diagnostics", fileName);
}
