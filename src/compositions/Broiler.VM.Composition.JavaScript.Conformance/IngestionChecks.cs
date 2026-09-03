using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The harness's own checks over the ingestion path: the dialect reader, the translation, and the
/// rule that decides whether a refusal answered anything.
/// </summary>
/// <remarks>
/// <para>
/// <b>These are the checks the scoring bug would have to get past.</b> An engine admitting almost
/// none of the language and a suite whose negative tests all declare a refusal will agree on the
/// observable outcome nearly every time they meet, for reasons that have nothing to do with each
/// other. Nothing in the arithmetic of a total notices that; only a rule about which refusals are
/// language answers does, so that rule gets checks naming the code and the verdict rather than a
/// check that some skip happened.
/// </para>
/// <para>
/// <b>No profile is composed here either.</b> The scoring rule is exercised through
/// <see cref="Execution.Compare"/> with observations written out by hand, which is what lets these
/// checks fail for exactly one reason: the rule changed.
/// </para>
/// </remarks>
internal static class IngestionChecks
{
    /// <summary>Runs every check over the ingestion path.</summary>
    internal static IReadOnlyList<(string Name, bool Passed, string Detail)> Run() =>
    [
        TheLanguageClassMapIsTotalOverTheVocabulary(),
        OnlyAnEarlyErrorMayAnswerAQuestionAboutTheLanguage(),
        AManifestRefusalIsNotAConformanceAnswer(),
        TheRuleAppliesToAnIngestedTestAndNotToThisComponentsOwn(),
        AnUnearnedRefusalCannotBecomeAPassInEitherDirection(),
        ARefusalNamingNoKnownCodeIsAFailureRatherThanASkip(),
        TheDialectReaderTakesANestedNegativeAndAFoldedDescription(),
        TheDialectReaderTakesBothListSpellings(),
        TheDialectReaderRefusesWhatWouldChangeHowAFileRuns(),
        TheDialectReaderSkipsAKeyItDoesNotKnow(),
        TheRawFlagDoesNotMeanArtifactBytes(),
        AFileWithNoStrictnessDeclaredIsReadBothWays(),
        ADeclaredStrictnessIsReadOneWay(),
        AParseNegativeIsSelectableEvenWhenItNamesHarnessFiles(),
        EveryFileThisHarnessCannotRunIsStillCounted(),
        AHarnessFileIsDeclinedRatherThanRefused(),
        TheTwoNewExpectationSpellingsRoundTrip(),
    ];

    private static (string, bool, string) TheLanguageClassMapIsTotalOverTheVocabulary()
    {
        // Written as a set difference in BOTH directions. A count comparison alone would be
        // satisfied by a map that classified one code twice and another not at all.
        var missing = LanguageErrors.All.Where(code => !LanguageErrors.Classified.Contains(code)).ToArray();
        var extra = LanguageErrors.Classified.Where(code => !LanguageErrors.All.Contains(code)).ToArray();

        return (
            "every-source-refusal-has-a-declared-language-class",
            missing.Length == 0 && extra.Length == 0,
            missing.Length == 0 && extra.Length == 0
                ? $"{LanguageErrors.All.Count} codes, each with a class"
                : $"unclassified: [{string.Join(", ", missing)}]; not in the vocabulary: [{string.Join(", ", extra)}]");
    }

    private static (string, bool, string) OnlyAnEarlyErrorMayAnswerAQuestionAboutTheLanguage()
    {
        var wrong = LanguageErrors.All
            .Where(code =>
                LanguageErrors.MayScore(code) != (LanguageErrors.Classify(code) == RefusalClass.EarlyError))
            .ToArray();

        // Each class must actually be populated. A map that had collapsed to one class would pass
        // the agreement test above and would have quietly stopped distinguishing anything.
        var populated = Enum.GetValues<RefusalClass>()
            .All(kind => LanguageErrors.All.Any(code => LanguageErrors.Classify(code) == kind));

        var counts = string.Join(", ", Enum.GetValues<RefusalClass>()
            .Select(kind => $"{kind}={LanguageErrors.All.Count(code => LanguageErrors.Classify(code) == kind)}"));

        return (
            "only-an-early-error-may-score-and-every-class-is-populated",
            wrong.Length == 0 && populated,
            wrong.Length == 0 && populated
                ? counts
                : $"disagreeing: [{string.Join(", ", wrong)}]; classes populated: {populated}");
    }

    private static (string, bool, string) AManifestRefusalIsNotAConformanceAnswer()
    {
        // THE CENTREPIECE, NAMED. The declaration and the answer both say "it was refused", which
        // is the agreement a scorer comparing outcomes would call a pass. It is not one: the source
        // is valid JavaScript, and this profile declined the construct without ever reaching the
        // thing the test was about.
        var observed = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            SliceSourceDiagnosticCode.ConstructOutsideManifest);

        var right =
            observed.Status == ConformanceStatus.Skipped &&
            observed.Detail.Contains("ConstructOutsideManifest", StringComparison.Ordinal) &&
            !LanguageErrors.MayScore(SliceSourceDiagnosticCode.ConstructOutsideManifest);

        return (
            "a-refusal-the-manifest-made-is-not-a-pass-on-a-negative-test",
            right,
            $"declared a SyntaxError, was refused as ConstructOutsideManifest, scored {observed.Status}");
    }

    private static (string, bool, string) TheRuleAppliesToAnIngestedTestAndNotToThisComponentsOwn()
    {
        // The same answer, twice, under the two dialects. This component's own fixture asked
        // whether THIS FRONT END refuses a construct outside its manifest - a question the refusal
        // answers exactly - so it must still be scored, and scored as a pass.
        var native = Score(
            new ConformanceTest(
                "own.js",
                "a fixture of this component's own",
                HostMode.Script,
                new ConformanceExpectation(
                    ExpectationKind.RefusedBySource,
                    nameof(SliceSourceDiagnosticCode.ConstructOutsideManifest)),
                "f();",
                null,
                [],
                string.Empty),
            SliceSourceDiagnosticCode.ConstructOutsideManifest);

        var ingested = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            SliceSourceDiagnosticCode.ConstructOutsideManifest);

        return (
            "the-same-refusal-scores-for-a-native-fixture-and-not-for-an-ingested-one",
            native.Status == ConformanceStatus.Passed && ingested.Status == ConformanceStatus.Skipped,
            $"native fixture {native.Status}, ingested test {ingested.Status}");
    }

    private static (string, bool, string) AnUnearnedRefusalCannotBecomeAPassInEitherDirection()
    {
        // The rule runs ahead of the comparison, so no declaration can be written that gets past
        // it. A positive test refused by the manifest is unscorable for the same reason a negative
        // one is, and both are checked rather than the negative alone.
        var positive = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.CompletesWithoutFault, string.Empty)),
            SliceSourceDiagnosticCode.ConstructOutsideManifest);

        var divergence = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            SliceSourceDiagnosticCode.UnresolvableIdentifier);

        var limit = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            SliceSourceDiagnosticCode.NestingTooDeep);

        var earned = Score(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            SliceSourceDiagnosticCode.ConstWithoutInitialiser);

        return (
            "an-unearned-refusal-is-unscorable-and-an-earned-one-passes",
            positive.Status == ConformanceStatus.Skipped &&
                divergence.Status == ConformanceStatus.Skipped &&
                limit.Status == ConformanceStatus.Skipped &&
                earned.Status == ConformanceStatus.Passed,
            $"positive={positive.Status}, divergence={divergence.Status}, " +
                $"limit={limit.Status}, early error={earned.Status}");
    }

    private static (string, bool, string) ARefusalNamingNoKnownCodeIsAFailureRatherThanASkip()
    {
        // A front end that refused without naming a diagnostic is a defect in this component.
        // Reporting it as unscorable would file it under "out of scope", where a growing count of
        // real defects would look like a growing count of things the manifest does not admit.
        var observed = Execution.Compare(
            Ingested(new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError")),
            new ConformanceExpectation(ExpectationKind.RefusedBySource, "NoDiagnostic"),
            [],
            "the front end refused and named no diagnostic");

        return (
            "a-refusal-that-names-no-code-is-reported-as-a-failure",
            observed.Status == ConformanceStatus.Failed,
            $"an unnamed refusal scored {observed.Status}");
    }

    private static (string, bool, string) TheDialectReaderTakesANestedNegativeAndAFoldedDescription()
    {
        const string Text = """
            /*---
            esid: sec-something
            description: >
              A description folded over
              two lines.
            info: |
              A literal block whose contents are not metadata:
              flags: [module]
            negative:
              phase: parse
              type: SyntaxError
            ---*/
            const x;
            """;

        var read = Test262Metadata.TryRead("a.js", Text, out var front, out var why);

        // The `info` block matters: it contains a line that LOOKS like a flags declaration, and a
        // reader that did not follow block scalars would have taken it as one and run the file
        // under the module goal.
        var right = read &&
            front.Description == "A description folded over two lines." &&
            front.Esid == "sec-something" &&
            front.Negative is { Phase: "parse", Type: "SyntaxError" } &&
            front.Flags.Count == 0;

        return (
            "a-nested-negative-and-a-folded-description-are-read-and-a-block-scalar-is-not-metadata",
            right,
            read ? $"description `{front.Description}`, flags {front.Flags.Count}" : why);
    }

    private static (string, bool, string) TheDialectReaderTakesBothListSpellings()
    {
        const string Inline = """
            /*---
            description: inline
            flags: [onlyStrict, module]
            features: [let, const]
            ---*/
            """;

        const string Block = """
            /*---
            description: block
            flags:
              - onlyStrict
              - module
            features:
              - let
              - const
            ---*/
            """;

        var first = Test262Metadata.TryRead("a.js", Inline, out var inline, out _);
        var second = Test262Metadata.TryRead("b.js", Block, out var block, out _);

        var agree = first && second &&
            inline.Flags.SequenceEqual(block.Flags, StringComparer.Ordinal) &&
            inline.Features.SequenceEqual(block.Features, StringComparer.Ordinal) &&
            inline.Flags.Count == 2;

        return (
            "the-two-list-spellings-read-to-the-same-list",
            agree,
            agree ? $"[{string.Join(", ", inline.Flags)}] both ways" : "the spellings disagree");
    }

    private static (string, bool, string) TheDialectReaderRefusesWhatWouldChangeHowAFileRuns()
    {
        var refusals = new (string Name, string Text)[]
        {
            ("an unknown flag", "/*---\ndescription: d\nflags: [teleport]\n---*/\n"),
            ("both strictnesses", "/*---\ndescription: d\nflags: [onlyStrict, noStrict]\n---*/\n"),
            ("a negative with no type", "/*---\ndescription: d\nnegative:\n  phase: parse\n---*/\n"),
            ("a negative with no phase", "/*---\ndescription: d\nnegative:\n  type: SyntaxError\n---*/\n"),
            ("an unknown phase", "/*---\ndescription: d\nnegative:\n  phase: link\n  type: SyntaxError\n---*/\n"),
            ("a key under negative that is neither", "/*---\ndescription: d\nnegative:\n  phase: parse\n  type: SyntaxError\n  when: later\n---*/\n"),
            ("no description", "/*---\nesid: sec-x\n---*/\n"),
            ("an unclosed block", "/*---\ndescription: d\n"),
        };

        var accepted = refusals
            .Where(one => Test262Metadata.TryRead("a.js", one.Text, out _, out _))
            .Select(one => one.Name)
            .ToArray();

        return (
            "the-dialect-reader-refuses-metadata-it-cannot-honour",
            accepted.Length == 0,
            accepted.Length == 0
                ? $"{refusals.Length} malformed blocks, each refused"
                : "accepted: " + string.Join("; ", accepted));
    }

    private static (string, bool, string) TheDialectReaderSkipsAKeyItDoesNotKnow()
    {
        // The asymmetry with the check above, stated as its own case: the key set is open and the
        // flag set is not. A reader that refused unknown keys would refuse most real files.
        const string Text = """
            /*---
            author: somebody
            es5id: 1.2.3
            timeout: 200
            locale: [en-US]
            description: a file carrying keys this harness has no use for
            ---*/
            """;

        var read = Test262Metadata.TryRead("a.js", Text, out var front, out var why);

        return (
            "a-key-this-harness-has-no-use-for-is-skipped-and-a-flag-is-not",
            read && front.Description.StartsWith("a file carrying", StringComparison.Ordinal),
            read ? $"read past 4 unknown keys to `{front.Description}`" : why);
    }

    private static (string, bool, string) TheRawFlagDoesNotMeanArtifactBytes()
    {
        // THE TWO DIALECTS USE THIS FLAG FOR DIFFERENT THINGS. Carrying it across would put source
        // into the host mode reserved for artifact bytes, where the verifier would be handed a
        // JavaScript file and every such test would fail for a reason nobody wrote down.
        var translated = Test262Adapter.TryTranslate(
            "a.js",
            "/*---\ndescription: a raw test\nflags: [raw]\n---*/\n1 + 2;\n",
            out var tests,
            out var why);

        var right = translated &&
            tests.Count == 1 &&
            tests[0].Mode == HostMode.Script &&
            tests[0].Bytes is null &&
            tests[0].Source.Length != 0;

        return (
            "the-suites-raw-flag-is-source-and-not-artifact-bytes",
            right,
            translated
                ? $"{tests.Count} case in {tests[0].Mode} mode carrying {(tests[0].Bytes is null ? "source" : "bytes")}"
                : why);
    }

    private static (string, bool, string) AFileWithNoStrictnessDeclaredIsReadBothWays()
    {
        var translated = Test262Adapter.TryTranslate(
            "a.js",
            "/*---\ndescription: d\nnegative:\n  phase: parse\n  type: SyntaxError\n---*/\nbreak;\n",
            out var tests,
            out var why);

        var strict = tests.FirstOrDefault(test => test.Path.EndsWith(Test262Adapter.StrictSuffix, StringComparison.Ordinal));
        var sloppy = tests.FirstOrDefault(test => test.Path.EndsWith(Test262Adapter.SloppySuffix, StringComparison.Ordinal));

        var right = translated &&
            tests.Count == 2 &&
            strict is not null &&
            sloppy is not null &&
            strict.Source.StartsWith(Test262Adapter.StrictPrologue, StringComparison.Ordinal) &&
            !sloppy.Source.StartsWith(Test262Adapter.StrictPrologue, StringComparison.Ordinal);

        return (
            "a-file-declaring-no-strictness-becomes-two-cases-and-only-one-carries-the-prologue",
            right,
            translated ? $"{tests.Count} cases: {string.Join(", ", tests.Select(test => test.Path))}" : why);
    }

    private static (string, bool, string) ADeclaredStrictnessIsReadOneWay()
    {
        var cases = new (string Name, string Flags, string Suffix, HostMode Mode)[]
        {
            ("onlyStrict", "flags: [onlyStrict]\n", Test262Adapter.StrictSuffix, HostMode.Script),
            ("noStrict", "flags: [noStrict]\n", Test262Adapter.SloppySuffix, HostMode.Script),
            ("module", "flags: [module]\n", string.Empty, HostMode.Module),
            ("raw", "flags: [raw]\n", string.Empty, HostMode.Script),
        };

        var wrong = new List<string>();

        foreach (var one in cases)
        {
            // A PARSE NEGATIVE, because a positive test is declined before a strictness or a goal
            // is ever chosen for it. The first draft of this check used one, and was measuring the
            // decline rather than the reading.
            var text = "/*---\ndescription: d\nnegative:\n  phase: parse\n  type: SyntaxError\n" +
                one.Flags + "---*/\nbreak;\n";

            if (!Test262Adapter.TryTranslate("a.js", text, out var tests, out _) ||
                tests.Count != 1 ||
                tests[0].Mode != one.Mode ||
                !tests[0].Path.EndsWith(one.Suffix, StringComparison.Ordinal))
            {
                wrong.Add(one.Name);
            }
        }

        return (
            "a-file-declaring-its-strictness-or-its-goal-becomes-one-case",
            wrong.Count == 0,
            wrong.Count == 0 ? "onlyStrict, noStrict, module and raw each give one case" : "wrong: " + string.Join(", ", wrong));
    }

    private static (string, bool, string) AParseNegativeIsSelectableEvenWhenItNamesHarnessFiles()
    {
        // This is the arm that makes the whole slice scorable. The file must fail before it runs,
        // so a harness file it names is never reached and needing one costs nothing.
        var withIncludes = Test262Adapter.TryTranslate(
            "a.js",
            "/*---\ndescription: d\nincludes: [assert.js, propertyHelper.js]\nnegative:\n  phase: parse\n  type: SyntaxError\n---*/\nbreak;\n",
            out var negative,
            out _);

        var positive = Test262Adapter.TryTranslate(
            "b.js",
            "/*---\ndescription: d\nincludes: [assert.js]\n---*/\n1 + 2;\n",
            out var declined,
            out _);

        var right = withIncludes && positive &&
            negative.All(test => test.Unselectable.Length == 0) &&
            declined.All(test => test.Unselectable.Length != 0);

        return (
            "a-parse-negative-naming-harness-files-is-selectable-and-a-positive-one-is-not",
            right,
            right
                ? $"{negative.Count} selectable negative case(s), {declined.Count} declined positive"
                : "the includes rule did not split the two");
    }

    private static (string, bool, string) EveryFileThisHarnessCannotRunIsStillCounted()
    {
        // A translator that returned nothing for what it declined would shrink the candidate count,
        // which is the figure a merge proves its coverage against.
        var declines = new[]
        {
            "/*---\ndescription: d\n---*/\n1 + 2;\n",
            "/*---\ndescription: d\nflags: [async]\nincludes: [doneprintHandle.js]\n---*/\n1;\n",
            "/*---\ndescription: d\nflags: [CanBlockIsFalse]\n---*/\n1;\n",
            "/*---\ndescription: d\nnegative:\n  phase: resolution\n  type: SyntaxError\nflags: [module]\n---*/\n1;\n",
            "/*---\ndescription: d\nnegative:\n  phase: parse\n  type: ReferenceError\n---*/\n1;\n",
            "/*---\ndescription: d\nnegative:\n  phase: runtime\n  type: URIError\n---*/\n1;\n",
        };

        var silent = declines
            .Where(text =>
                !Test262Adapter.TryTranslate("a.js", text, out var tests, out _) ||
                tests.Count == 0 ||
                tests.Any(test => test.Unselectable.Length == 0))
            .ToArray();

        return (
            "a-file-this-harness-cannot-run-is-counted-and-named-rather-than-dropped",
            silent.Length == 0,
            silent.Length == 0
                ? $"{declines.Length} declines, each a counted candidate carrying a reason"
                : $"{silent.Length} of {declines.Length} were dropped or wrongly selected");
    }

    private static (string, bool, string) AHarnessFileIsDeclinedRatherThanRefused()
    {
        // A suite ships its assertion library and its module fixtures beside its tests, as source
        // with no metadata block. Refusing one refuses the whole suite, because a suite is read
        // whole - so a real checkout would score nothing over the presence of the files that must
        // be there.
        var translated = Test262Adapter.TryTranslate(
            "harness/assert.js",
            "// no metadata block here\nfunction assert() { }\n",
            out var tests,
            out var why);

        return (
            "a-suite-file-carrying-no-metadata-block-is-declined-and-not-refused",
            translated && tests.Count == 1 && tests[0].Unselectable.Length != 0,
            translated ? $"declined: {tests[0].Unselectable}" : "refused: " + why);
    }

    private static (string, bool, string) TheTwoNewExpectationSpellingsRoundTrip()
    {
        var expectations = new[]
        {
            new ConformanceExpectation(ExpectationKind.RefusedAsEarlyError, "SyntaxError"),
            new ConformanceExpectation(ExpectationKind.CompletesWithoutFault, string.Empty),
        };

        var broken = expectations
            .Where(one =>
                !ConformanceExpectation.TryParse(one.ToString(), out var read, out _) || read != one)
            .ToArray();

        return (
            "the-two-ingested-expectation-spellings-parse-back-to-themselves",
            broken.Length == 0,
            broken.Length == 0
                ? string.Join(", ", expectations.Select(one => $"`{one}`"))
                : "did not round-trip: " + string.Join(", ", broken.Select(one => one.ToString())));
    }

    /// <summary>An ingested test declaring one thing, with nothing else that matters set.</summary>
    private static ConformanceTest Ingested(ConformanceExpectation expectation) =>
        new(
            "suite/a.js",
            "an ingested test",
            HostMode.Script,
            expectation,
            "source",
            null,
            [],
            string.Empty,
            string.Empty,
            Ingested: true);

    /// <summary>Scores a test against a front-end refusal carrying one code.</summary>
    private static Observation Score(ConformanceTest test, SliceSourceDiagnosticCode code) =>
        Execution.Compare(
            test,
            new ConformanceExpectation(ExpectationKind.RefusedBySource, code.ToString()),
            [],
            "a refusal this check wrote out by hand");
}
