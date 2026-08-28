using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group J: the rules that hold the Broiler Code Assurance record to the code it describes.
/// </summary>
/// <remarks>
/// <para>
/// The assurance record is a set of claims about the product tree - which units were assessed, at
/// what risk, against which version of the implementation, and by whom. Every one of those claims
/// is checkable against the tree, and an unchecked one rots exactly the way a review document
/// rots: it keeps being read as current long after it stopped being true.
/// </para>
/// <para>
/// <b>What this component's record currently says, and why that matters here.</b> Every relevant
/// unit is assessed and <b>nothing has been reviewed by a human</b>. The whole value of the system
/// at this milestone is that it records that absence precisely, which means the rule that matters
/// most is J4: no approval may exist that a human did not make. A generator that could manufacture
/// one would turn an honest record of no review into a false record of review, and a false record
/// is worse than no record because it is trusted.
/// </para>
/// <para>
/// Every rule here asserts in both directions. The real tree must pass, and every clause must be
/// shown rejecting an input that breaks it - the witness inputs under
/// <c>witnesses/assurance/</c>. The witnesses are per CLAUSE, not per rule, and each assertion
/// names the CONTENT of the violation it expects rather than checking that some list is non-empty.
/// A witness asserted with a bare non-empty check pins only whichever clause happens to fire
/// first, which is how the group H attempt that preceded this one lost four independent clauses in
/// a single patch with the suite green. Group J is written to the standard that pass established.
/// </para>
/// <para>
/// <b>The reviewer identifier in the witnesses is not a person.</b> Several witnesses carry
/// <c>WITNESS-ONLY</c> on a human line, because a rule about false approvals cannot be witnessed
/// without an input that contains one. It appears nowhere outside this directory, and J4 asserts
/// that nothing in the product tree or in any generated artefact carries a name at all.
/// </para>
/// <para>
/// Each rule also holds its own register row to the limits it depends on, for the reason group H
/// does: nothing else in the suite reads a row's prose, so a row could be rewritten into an
/// over-claim - a rule weaker than its own statement - in one edit with the suite green.
/// </para>
/// </remarks>
public sealed class AssuranceRuleTests
{
    // =====================================================================================
    // The corpus: the product tree as the generator would leave it, and the witness inputs.
    // =====================================================================================

    /// <summary>
    /// The product source files as the generator would write them.
    /// </summary>
    /// <remarks>
    /// The post-generation text, not the text on disk, for the same reason
    /// <see cref="AssurancePlan.Units"/> is the post-generation unit set: every property these
    /// rules assert is a property of the tree the generator produces, and J5 separately asserts
    /// that the tree on disk IS that tree. Reading disk here would make J1 to J4 fail during a
    /// write run for a reason no reader could act on.
    /// </remarks>
    private static IReadOnlyList<AssuranceSourceFile> GeneratedSources { get; } =
        AssuranceGenerator.Current.Artefacts
            .Where(static artefact => artefact.RelativePath.EndsWith(".cs", StringComparison.Ordinal))
            .Select(static artefact => AssuranceSources.WithText(
                AssuranceSources.File(artefact.RelativePath),
                artefact.Desired))
            .ToArray();

    private static IReadOnlyList<AssuranceUnit> ProductUnits => AssuranceGenerator.Current.Units;

    private static string ComponentReport => AssuranceGenerator.Current.Artefacts
        .Single(static artefact => string.Equals(
            artefact.RelativePath, AssuranceGenerator.ReportPath, StringComparison.Ordinal))
        .Desired;

    /// <summary>
    /// A witness input under <c>witnesses/assurance/</c>, read as though it were a product file.
    /// </summary>
    /// <remarks>
    /// The C# witnesses carry a <c>.cs.witness</c> extension so the SDK never globs them into the
    /// build, exactly as the group A project-file witnesses do. Their relative path is the file
    /// name, so every violation message names the witness it came from.
    /// </remarks>
    private static AssuranceSourceFile Witness(string fileName) =>
        AssuranceProbe.Source(WitnessText(fileName), fileName);

    private static string WitnessText(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "assurance", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");

        return File.ReadAllText(path);
    }

    private static IReadOnlyList<AssuranceUnit> WitnessUnits(string fileName) =>
        AssuranceScanner.Scan(Witness(fileName));

    /// <summary>The fingerprint of the one method every witness that needs a real one declares.</summary>
    /// <remarks>
    /// Baked rather than computed, because a witness that derived the value it is supposed to
    /// record would agree with itself whatever the fingerprinter did. It is asserted against the
    /// fingerprinter in J3, so a change to normalization fails there and names this constant
    /// rather than quietly making four witnesses vacuous.
    /// </remarks>
    private const string WitnessFingerprint = "44EBF3";

    // =====================================================================================
    // J1 - coverage
    // =====================================================================================

    /// <summary>
    /// J1. Every relevant unit in the product graph carries an assurance annotation, where the
    /// exemption predicate decides which units are relevant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Relevance is not a property of the annotation. It is decided by
    /// <see cref="AssuranceScanner.ExemptionFor"/> - one predicate, in one place, which is the
    /// reviewable artefact of the whole exemption scheme - and a unit it answers
    /// <see cref="AssuranceExemption.None"/> for must carry a block. That is also the rule's
    /// limit, recorded as an exclusion rather than implied: a unit the predicate wrongly exempts
    /// is not reported by this rule, because this rule cannot see it.
    /// </para>
    /// <para>
    /// The three violating witnesses are the three ways a block stops covering the unit it was
    /// written for: never written, written below the declaration instead of above it, and written
    /// as half a block. In every one of them the assurance comment is still in the file, which is
    /// what makes them worth witnessing - a coverage check that looked for the text rather than
    /// for the attachment would pass all three.
    /// </para>
    /// </remarks>
    [Fact]
    public void J1_Every_Relevant_Unit_Carries_An_Annotation()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J1",
            "exemption predicate",
            "wrongly exempts",
            "EX-62",
            "EX-63");

        // The clean direction, over the tree the generator would leave behind.
        Assert.Empty(CoverageViolations(ProductUnits));

        // Non-vacuous: the predicate answers both ways over the checkout, so the clean result is
        // a classification and not a predicate that exempts everything.
        Assert.NotEmpty(ProductUnits.Where(static unit => unit.IsRelevant));
        Assert.NotEmpty(ProductUnits.Where(static unit => unit.IsExempt));
        Assert.All(
            ProductUnits.Where(static unit => unit.IsRelevant),
            static unit => Assert.NotNull(unit.Annotation));

        // Clause: a relevant unit with no annotation anywhere near it is named.
        var uncovered = Assert.Single(CoverageViolations(
            WitnessUnits("J1-relevant-unit-with-no-annotation.cs.witness")));
        Assert.Contains("J1-relevant-unit-with-no-annotation.cs.witness", uncovered, StringComparison.Ordinal);
        Assert.Contains(
            "Probe.Uncovered.Fold(int[]) is relevant and carries no assurance annotation",
            uncovered,
            StringComparison.Ordinal);

        // Clause: a block is an annotation on the declaration it sits ABOVE. Below it, it is
        // leading trivia of the closing brace and annotates nothing.
        var below = Assert.Single(CoverageViolations(
            WitnessUnits("J1-annotation-below-its-declaration.cs.witness")));
        Assert.Contains(
            "Probe.Drifted.Fold(int[]) is relevant and carries no assurance annotation",
            below,
            StringComparison.Ordinal);

        // Clause: the block is two lines. An AI line with no human line under it records an
        // assessment and no review state, and must not read as coverage.
        var halved = Assert.Single(CoverageViolations(
            WitnessUnits("J1-half-a-block-does-not-annotate.cs.witness")));
        Assert.Contains(
            "Probe.Halved.Fold(int[]) is relevant and carries no assurance annotation",
            halved,
            StringComparison.Ordinal);

        // Clause: the predicate decides relevance. Six exempt units, no annotation, no violation -
        // the half of the rule that keeps it from becoming a rule about comments.
        var trivial = WitnessUnits("J1-exempt-units-need-no-annotation.cs.witness");
        Assert.Equal(6, trivial.Count);
        Assert.All(trivial, static unit => Assert.True(unit.IsExempt, unit.Where));
        Assert.Empty(CoverageViolations(trivial));

        // Clause: the per-unit escape hatch, for what the predicate cannot see. Shim would
        // otherwise be relevant; a reason a human wrote outranks the predicate, and the case is
        // reported apart from the six so that it stays countable in the component report.
        var hatched = WitnessUnits("J1-an-explicit-exemption-covers-a-unit.cs.witness");
        var shim = Assert.Single(hatched);
        Assert.Equal(AssuranceExemption.DeclaredInSource, shim.Exemption);
        Assert.Equal(AssuranceReviewState.Exempt, shim.State);
        Assert.Empty(CoverageViolations(hatched));

        // Clause: exemption case 2 requires the assigned member to CORRESPOND to the parameter.
        // A permutation is a decision about which ceiling holds which value, and the predicate
        // used to call it trivial because both right-hand sides were parameters of the same
        // constructor.
        var permuted = Assert.Single(CoverageViolations(
            WitnessUnits("J1-a-permuted-constructor-assignment-is-relevant.cs.witness")));
        Assert.Contains(
            "Probe.Permuted.Permuted(ulong, ulong) is relevant and carries no assurance annotation",
            permuted,
            StringComparison.Ordinal);

        // Clause: exemption case 3 reaches only a delegation that forwards its own parameters. A
        // literal width and an enum member naming a budget dimension are values the member
        // SUPPLIES, and in this component the supplied value is the policy.
        var supplied = CoverageViolations(
            WitnessUnits("J1-a-delegation-that-supplies-a-value-is-relevant.cs.witness"));
        Assert.Equal(2, supplied.Count);
        Assert.Single(supplied.Where(static claim => claim.Contains(
            "Probe.Routed.TryReadWide(out ulong) is relevant", StringComparison.Ordinal)));
        Assert.Single(supplied.Where(static claim => claim.Contains(
            "Probe.Routed.TryReserve(ulong) is relevant", StringComparison.Ordinal)));

        // ...and the accepting half of the same witness: the member that forwards its parameters
        // unchanged stays exempt, so the case is narrowed rather than deleted.
        Assert.Equal(
            AssuranceExemption.TrivialExpressionBodiedMember,
            WitnessUnits("J1-a-delegation-that-supplies-a-value-is-relevant.cs.witness")
                .Single(static unit => unit.Name.Contains(".TryChargeAt(", StringComparison.Ordinal))
                .Exemption);

        // Clause: a const or static readonly field with an initializer is a code unit. Both of the
        // ones in the witness are budgets, and neither was a unit at all until this changed.
        var constants = WitnessUnits("J1-an-initialized-constant-is-a-code-unit.cs.witness");
        var uninitialized = CoverageViolations(constants);
        Assert.Equal(2, uninitialized.Count);
        Assert.Single(uninitialized.Where(static claim => claim.Contains(
            "Probe.Budgeted.MaximumEntries is relevant", StringComparison.Ordinal)));
        Assert.Single(uninitialized.Where(static claim => claim.Contains(
            "Probe.Budgeted.Widths is relevant", StringComparison.Ordinal)));

        // ...and the other half: a plain instance field is not a unit, so this is not a rule about
        // every field. Four units in that witness - the two constants and the two trivial
        // properties - and neither `used` nor `mutable` is one of them.
        Assert.Equal(
            new[]
            {
                "Probe.Budgeted.MaximumEntries",
                "Probe.Budgeted.Mutable",
                "Probe.Budgeted.Used",
                "Probe.Budgeted.Widths",
            },
            constants.Select(static unit => unit.Name).OrderBy(static name => name, StringComparer.Ordinal));

        // Clause: the corpus this rule reads is the compiler's, not a directory heuristic. Only a
        // project's OWN bin and obj are build output; a product file under a nested directory that
        // happens to be called obj compiles into the shipped assembly, and dropping it made every
        // rule in this group read a source set the compiler does not.
        var project = Path.Combine(ComponentGraph.Root, "src", "Broiler.VM.Runtime");

        Assert.False(
            AssuranceSources.IsProjectBuildOutput(
                project, Path.Combine(project, "Internal", "obj", "VmHiddenGate.cs")),
            "A product file under a nested directory named obj is compiled and must be covered.");
        Assert.True(
            AssuranceSources.IsProjectBuildOutput(
                project, Path.Combine(project, "obj", "Release", "net10.0", "Generated.cs")),
            "The project's own obj directory is build output and must not be covered.");
        Assert.True(
            AssuranceSources.IsProjectBuildOutput(
                project, Path.Combine(project, "bin", "Release", "net10.0", "Ref.cs")),
            "The project's own bin directory is build output and must not be covered.");
    }

    private static List<string> CoverageViolations(IEnumerable<AssuranceUnit> units) => units
        .Where(static unit => unit.IsRelevant && unit.Annotation is null)
        .Select(static unit => $"{unit.Where} is relevant and carries no assurance annotation")
        .ToList();

    // =====================================================================================
    // J2 - well-formedness
    // =====================================================================================

    /// <summary>
    /// J2. Every annotation parses, and every field value is in its closed vocabulary.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two halves, and both are needed. An annotation that does not parse is reported as stranded
    /// rather than ignored, because a block the reader believes covers a unit and the scanner
    /// cannot read is the worst of both records. An annotation that parses is held to its field
    /// set: <c>Origin</c>, <c>IP</c> and <c>Security</c> to their tables, <c>Resources</c> to an
    /// integer 0 to 10, <c>Fingerprint</c> to <c>TBF</c> or six uppercase hex characters, and the
    /// field set itself is closed - a field this system does not define is a claim nothing checks.
    /// </para>
    /// <para>
    /// Every value clause has a witness of its own. A single witness carrying four bad fields
    /// would be satisfied by a check that reported the first one, and three of the four
    /// vocabularies could then be deleted with the suite green.
    /// </para>
    /// </remarks>
    [Fact]
    public void J2_Every_Annotation_Parses_And_Every_Field_Is_In_Its_Vocabulary()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J2",
            "closed vocabulary",
            "stranded",
            "Spec is optional",
            "EX-65");

        // The clean direction: every annotation in the product tree parses and every field is in
        // vocabulary. Non-vacuous - the tree carries 496 of them.
        Assert.Empty(VocabularyViolations(ProductUnits));
        Assert.Empty(ParseViolations(GeneratedSources));
        Assert.Empty(SpecViolations(ProductUnits));
        Assert.NotEmpty(ProductUnits.Where(static unit => unit.Annotation is not null));

        // Non-vacuous for the citation clause specifically: the tree cites records, so the check
        // resolves something rather than never running.
        Assert.NotEmpty(ProductUnits.Where(static unit =>
            unit.Annotation?.Field("Spec") is { } spec &&
            spec.StartsWith("ADR-", StringComparison.Ordinal)));

        // Clause, one per closed vocabulary and one per required field.
        AssertOneVocabularyProblem(
            "J2-origin-outside-its-vocabulary.cs.witness",
            "Origin=Robot is outside its vocabulary");
        AssertOneVocabularyProblem(
            "J2-ip-risk-outside-its-vocabulary.cs.witness",
            "IP=Minimal is outside its vocabulary");
        AssertOneVocabularyProblem(
            "J2-security-risk-outside-its-vocabulary.cs.witness",
            "Security=Severe is outside its vocabulary");
        AssertOneVocabularyProblem(
            "J2-resources-above-the-scale.cs.witness",
            "Resources=11 is not an integer 0 to 10");
        AssertOneVocabularyProblem(
            "J2-resources-is-not-an-integer.cs.witness",
            "Resources=high is not an integer 0 to 10");
        AssertOneVocabularyProblem(
            "J2-a-required-field-is-missing.cs.witness",
            "no Security field");
        AssertOneVocabularyProblem(
            "J2-fingerprint-is-not-six-hex.cs.witness",
            "Fingerprint=ABC is neither TBF nor six uppercase hex characters");
        AssertOneVocabularyProblem(
            "J2-a-field-this-system-does-not-define.cs.witness",
            "Confidence=High is not a field this system defines");

        // Clause: EXEMPT is a reason or it is nothing.
        AssertOneVocabularyProblem(
            "J2-an-exemption-carries-no-reason.cs.witness",
            "EXEMPT carries no reason");

        // Clause: EXEMPT is not an assessment, so it may not be written beside one.
        AssertOneVocabularyProblem(
            "J2-an-exemption-stated-beside-an-assessment.cs.witness",
            "EXEMPT is stated beside other fields");

        // Clause: a field with no '=' does not parse, so the block is not an annotation. Both of
        // its lines are reported - the human line because a human line alone opens no block.
        var unparsed = ParseViolations([Witness("J2-a-field-carries-no-equals-sign.cs.witness")]);
        Assert.Equal(2, unparsed.Count);
        Assert.Single(unparsed.Where(static problem =>
            problem.Contains("has a field with no '=': 'IP Low'", StringComparison.Ordinal)));
        Assert.Single(unparsed.Where(static problem =>
            problem.Contains("does not open with '// Broiler-AI:'", StringComparison.Ordinal)));

        // Clause: a well-formed block attached to no declaration. The only unit in that witness is
        // an auto-property the predicate exempts, so J1 has nothing to say about it and this is
        // the only rule that sees the block at all.
        var stranded = ParseViolations([Witness("J2-an-annotation-attached-to-no-declaration.cs.witness")]);
        Assert.Equal(2, stranded.Count);
        Assert.Single(stranded.Where(static problem =>
            problem.Contains("attached to no declaration", StringComparison.Ordinal)));
        Assert.Single(stranded.Where(static problem =>
            problem.Contains("does not open with '// Broiler-AI:'", StringComparison.Ordinal)));
        Assert.Empty(CoverageViolations(
            WitnessUnits("J2-an-annotation-attached-to-no-declaration.cs.witness")));

        // Clause: a Spec field that cites an ADR must cite one that exists. Membership of a closed
        // vocabulary cannot answer this - Spec has no vocabulary, it has a referent - so an
        // annotation could name ADR-9999 s42 and read, to anyone skimming, as though a frozen
        // record had been consulted.
        var uncited = Assert.Single(SpecViolations(
            WitnessUnits("J2-spec-names-a-record-that-does-not-exist.cs.witness")));
        Assert.Contains(
            "J2-spec-names-a-record-that-does-not-exist.cs.witness", uncited, StringComparison.Ordinal);
        Assert.Contains(
            "cites Spec=ADR-9999 s42, and docs/adr/ holds no record 9999",
            uncited,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// Every <c>Spec=ADR-nnnn</c> citation that names no record under <c>docs/adr/</c>.
    /// </summary>
    /// <remarks>
    /// The rule is deliberately narrow, and the register row says so: it resolves the ADR NUMBER
    /// and nothing else. Whether the section exists, and whether the record says what the
    /// annotation implies it says, are not checked - the first would need a section index the
    /// records do not carry uniformly, and the second is a review.
    /// </remarks>
    private static List<string> SpecViolations(IEnumerable<AssuranceUnit> units)
    {
        var records = Directory
            .EnumerateFiles(Path.Combine(ComponentGraph.Root, "docs", "adr"), "*.md")
            .Select(static path => Path.GetFileName(path).Split('-')[0])
            .ToHashSet(StringComparer.Ordinal);

        var violations = new List<string>();

        foreach (var unit in units.Where(static unit => unit.Annotation?.Field("Spec") is not null))
        {
            var spec = unit.Annotation!.Field("Spec")!;

            if (!spec.StartsWith("ADR-", StringComparison.Ordinal))
            {
                continue;
            }

            var number = new string(spec["ADR-".Length..].TakeWhile(char.IsAsciiDigit).ToArray());

            if (number.Length == 4 && records.Contains(number))
            {
                continue;
            }

            violations.Add(
                $"{unit.Where} cites Spec={spec}, and docs/adr/ holds no record " +
                $"{(number.Length == 0 ? "with a four-digit number" : number)}");
        }

        return violations;
    }

    private static void AssertOneVocabularyProblem(string witness, string expected)
    {
        var problem = Assert.Single(VocabularyViolations(WitnessUnits(witness)));

        Assert.Contains(witness, problem, StringComparison.Ordinal);
        Assert.Contains(expected, problem, StringComparison.Ordinal);
    }

    private static List<string> VocabularyViolations(IEnumerable<AssuranceUnit> units) => units
        .Where(static unit => unit.Annotation is not null)
        .SelectMany(static unit => unit.Annotation!
            .VocabularyProblems()
            .Select(problem => $"{unit.Where}: {problem}"))
        .ToList();

    private static List<string> ParseViolations(IEnumerable<AssuranceSourceFile> files) => files
        .SelectMany(static file => AssuranceScanner.OrphanAnnotations(file, AssuranceScanner.Scan(file)))
        .ToList();

    // =====================================================================================
    // J3 - fingerprint currency
    // =====================================================================================

    /// <summary>
    /// J3. Every recorded AI fingerprint is the one the current code produces, and no human
    /// fingerprint names a version that is neither current nor preserved as <c>Previous</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the clause that makes the policy's core principle mechanical: a human reviewer
    /// certifies a specific version of an implementation, not a comment. Without it the two lines
    /// are prose that outlives the code they describe.
    /// </para>
    /// <para>
    /// <c>TBF</c> is reported separately from a wrong value, because they are different facts. A
    /// wrong value means someone edited a machine field; the placeholder means the generator has
    /// not run, so nothing is bound to anything yet.
    /// </para>
    /// <para>
    /// A <c>Previous=</c> fingerprint is history and is accepted. Reporting it as a mismatch would
    /// punish the generator for recording what the policy requires it to record, and would push a
    /// reviewer towards deleting the history to get a green suite.
    /// </para>
    /// <para>
    /// The rule's limit is the width of the value, and it is stated rather than glossed: six hex
    /// characters answer <em>did this unit change since it was reviewed</em> and nothing else.
    /// </para>
    /// </remarks>
    [Fact]
    public void J3_Every_Recorded_Fingerprint_Is_The_One_The_Code_Produces()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J3",
            "not a cryptographic commitment",
            "collision-free",
            "as read from disk",
            "EX-61");

        // The clean direction, over the units AS THEY ARE ON DISK.
        //
        // Reading the generator's output here made the rule vacuous: every Fingerprint field in
        // AssuranceGenerator.Current.Units has already been rewritten to the recomputed value, so
        // the comparison was a recomputed value against itself and could never report a stale
        // recorded fingerprint on the real tree. Editing a literal inside an annotated body left
        // J3 green.
        //
        // It runs in gate mode only, for the reason J5's currency half does: a write run exists to
        // refresh a fingerprint the code has outrun, and a rule asserting the pre-write value
        // equalled the post-write value would fail every generation for a reason no reader could
        // act on. The register row records that, and the witnesses run in both modes.
        if (!AssuranceGenerator.WriteRequested)
        {
            Assert.Empty(FingerprintViolations(AssuranceScanner.Units));
        }

        // Non-vacuous, and stronger than "no mismatch": after a generation every unit carrying a
        // full assessment records a real fingerprint rather than the placeholder. An EXEMPT= line
        // is excluded, because it records no fingerprint and the specification says it does not
        // have to - the same shape FingerprintViolations tolerates two screens down. Applying this
        // to every annotated unit made the documented per-unit escape hatch unusable anywhere in
        // the product tree while J1 went on witnessing it being accepted.
        var annotated = ProductUnits
            .Where(static unit => unit.Annotation is { ExemptReason: null })
            .ToArray();

        Assert.NotEmpty(annotated);
        Assert.All(annotated, static unit => Assert.True(
            AssuranceFingerprint.IsWellFormed(unit.Annotation!.RecordedFingerprint ?? string.Empty),
            $"{unit.Where} records {unit.Annotation!.RecordedFingerprint}"));

        // The baked witness constant is the fingerprinter's own answer, so a change to
        // normalization fails here and names it rather than making four witnesses vacuous.
        Assert.Equal(
            WitnessFingerprint,
            AssuranceProbe.Fingerprint(WitnessText("J3-a-review-of-the-current-version-is-current.cs.witness"), "Fold"));

        // Clause: a recorded value the code does not produce. Both halves of the message matter -
        // what was recorded, and what the code says now.
        var wrong = Assert.Single(FingerprintViolations(
            WitnessUnits("J3-recorded-fingerprint-is-not-the-current-one.cs.witness")));
        Assert.Contains("records Fingerprint=ABCDEF", wrong, StringComparison.Ordinal);
        Assert.Contains($"the current code computes {WitnessFingerprint}", wrong, StringComparison.Ordinal);

        // Clause: the placeholder, reported as its own fact.
        var placeholder = Assert.Single(FingerprintViolations(
            WitnessUnits("J3-fingerprint-is-still-the-placeholder.cs.witness")));
        Assert.Contains("still records the placeholder TBF", placeholder, StringComparison.Ordinal);

        // Clause: the human half. The AI line is current, so nothing but the approval is wrong -
        // which is exactly the state a reviewed unit enters when its code changes.
        var outrun = Assert.Single(FingerprintViolations(
            WitnessUnits("J3-human-approves-a-version-that-is-not-here.cs.witness")));
        Assert.Contains("approves Fingerprint=112233", outrun, StringComparison.Ordinal);
        Assert.Contains(
            $"which is neither the current {WitnessFingerprint} nor preserved as Previous",
            outrun,
            StringComparison.Ordinal);

        // Clause: Previous is history, not a live approval, and is accepted.
        var preserved = WitnessUnits("J3-a-stale-line-preserves-the-version-it-approved.cs.witness");
        Assert.Equal(AssuranceReviewState.Stale, Assert.Single(preserved).State);
        Assert.Empty(FingerprintViolations(preserved));

        // Clause: the accepted direction. A human fingerprint that IS the current one is current,
        // whatever J4 goes on to say about this component carrying no approvals at all.
        var current = WitnessUnits("J3-a-review-of-the-current-version-is-current.cs.witness");
        Assert.Equal(AssuranceReviewState.Verified, Assert.Single(current).State);
        Assert.Empty(FingerprintViolations(current));

        // Clause: the specification's per-unit escape hatch records no fingerprint, and both
        // halves of this rule accept it. The comparison tolerates the missing value, and the
        // well-formedness assertion above does not reach an EXEMPT line - which it used to, so
        // the documented hatch failed J3 the moment anyone used it.
        var hatched = WitnessUnits("J3-an-exempt-annotation-records-no-fingerprint.cs.witness");
        var exempt = Assert.Single(hatched);

        Assert.Null(exempt.Annotation!.RecordedFingerprint);
        Assert.Equal(AssuranceReviewState.Exempt, exempt.State);
        Assert.Empty(FingerprintViolations(hatched));
        Assert.Empty(VocabularyViolations(hatched));
        Assert.Empty(hatched.Where(static unit => unit.Annotation is { ExemptReason: null }));
    }

    private static List<string> FingerprintViolations(IEnumerable<AssuranceUnit> units)
    {
        var violations = new List<string>();

        foreach (var unit in units.Where(static unit => unit.Annotation is not null))
        {
            var annotation = unit.Annotation!;
            var recorded = annotation.RecordedFingerprint;

            // An EXEMPT= line records no fingerprint and is not supposed to. Whether it may omit
            // one is J2's question, not this rule's.
            if (recorded is not null)
            {
                if (string.Equals(recorded, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal))
                {
                    violations.Add(
                        $"{unit.Where} still records the placeholder {AssuranceFingerprint.ToBeFilled}, " +
                        "so no recorded fingerprint binds anything");
                }
                else if (!string.Equals(recorded, unit.Fingerprint, StringComparison.Ordinal))
                {
                    violations.Add(
                        $"{unit.Where} records Fingerprint={recorded} and " +
                        $"the current code computes {unit.Fingerprint}");
                }
            }

            var approved = annotation.HumanFingerprint;

            if (approved is null ||
                string.Equals(approved, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal) ||
                string.Equals(approved, unit.Fingerprint, StringComparison.Ordinal) ||
                (annotation.Previous is { } previous &&
                 string.Equals(previous.Fingerprint, approved, StringComparison.Ordinal)))
            {
                continue;
            }

            violations.Add(
                $"{unit.Where} approves Fingerprint={approved}, which is neither " +
                $"the current {unit.Fingerprint} nor preserved as Previous");
        }

        return violations;
    }

    // =====================================================================================
    // J4 - no approval a human did not make
    // =====================================================================================

    /// <summary>
    /// J4. No unit is <c>VERIFIED</c> and no unit names a reviewer unless a human line says so,
    /// and no generated artefact carries a reviewer identifier the source does not carry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the policy's hardest rule: CI may never turn <c>Broiler-Human: PENDING</c> into
    /// <c>Broiler-Human: EB</c>. It has two halves and both are witnessed.
    /// </para>
    /// <para>
    /// <b>The generator cannot manufacture an approval.</b> Four inputs, four outputs, and no
    /// fifth. <c>PENDING</c> comes out <c>PENDING</c>; a reviewer who left <c>TBF</c> gets the
    /// fingerprint filled and nothing else; a review the code has outrun becomes
    /// <c>STALE; Previous=...</c> with the reviewer and the approved version preserved; and a
    /// <c>STALE</c> line stays <c>STALE</c>, because the policy's forbidden edge - stale straight
    /// to verified - is cleared only by a human replacing the line. The refusal is an exception
    /// rather than a report, so a bug that reached for a name fails loudly instead of quietly
    /// writing an approval nobody gave.
    /// </para>
    /// <para>
    /// <b>This component names nobody.</b> Nothing in Broiler.VM has been reviewed by a human. A
    /// reviewer identifier in the tree - live, or preserved as <c>Previous</c> - would be a false
    /// record, so its absence is asserted at the source and again in every artefact generated from
    /// it, rather than assumed from the first.
    /// </para>
    /// <para>
    /// What the rule cannot do is tell a real reviewer from an invented one. It refuses a name the
    /// tooling produced; it cannot refuse a name a human typed for someone else. That is the same
    /// limit rule H4 records for the attestation, and the policy answers it outside the source, in
    /// the pull-request history.
    /// </para>
    /// </remarks>
    [Fact]
    public void J4_No_Approval_Exists_That_A_Human_Did_Not_Make()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J4",
            "invented one",
            "PENDING",
            "defined shapes",
            "pull-request history");

        // The clean direction, at the source and in the artefacts. Non-vacuous: 496 annotated
        // units, every one of them PENDING.
        Assert.Empty(ApprovalViolations(ProductUnits));
        Assert.Empty(ArtefactReviewerViolations());
        Assert.NotEmpty(ProductUnits.Where(static unit => unit.Annotation is not null));
        Assert.All(
            ProductUnits.Where(static unit => unit.Annotation is not null),
            static unit => Assert.True(unit.Annotation!.HumanIsPending, unit.Where));

        // Clause: a live reviewer identifier. Two facts, reported separately, because deleting
        // either check would leave the other reporting the same input and look like coverage.
        var named = ApprovalViolations(WitnessUnits("J4-a-unit-names-a-reviewer.cs.witness"));
        Assert.Equal(2, named.Count);
        Assert.Single(named.Where(static claim =>
            claim.Contains("names reviewer 'WITNESS-ONLY' on its human line", StringComparison.Ordinal)));
        Assert.Single(named.Where(static claim => claim.Contains(
            "carries a human line reading 'WITNESS-ONLY; Fingerprint=TBF'", StringComparison.Ordinal)));

        // Clause: the VERIFIED state itself, which is what makes a unit eligible for release.
        var verified = ApprovalViolations(WitnessUnits("J4-a-unit-is-verified.cs.witness"));
        Assert.Equal(3, verified.Count);
        Assert.Single(verified.Where(static claim => claim.Contains("is VERIFIED", StringComparison.Ordinal)));
        Assert.Single(verified.Where(static claim =>
            claim.Contains("names reviewer 'WITNESS-ONLY'", StringComparison.Ordinal)));

        // Clause: a reviewer preserved as history. J3 accepts it and J4 does not, and the
        // difference is the point: Previous here would say a human once reviewed this code.
        var previous = ApprovalViolations(
            WitnessUnits("J4-a-stale-line-preserves-a-previous-reviewer.cs.witness"));
        Assert.Equal(2, previous.Count);
        Assert.Single(previous.Where(static claim => claim.Contains(
            "preserves previous reviewer 'WITNESS-ONLY' on its human line", StringComparison.Ordinal)));

        // Clause: the accepted direction, and the state of every unit in this component.
        var pending = WitnessUnits("J4-a-pending-line-claims-nothing.cs.witness");
        Assert.Equal(AssuranceReviewState.AiAssessed, Assert.Single(pending).State);
        Assert.Empty(ApprovalViolations(pending));

        // Clause: the refusal. The generator is handed the transformation the policy forbids and
        // must throw, naming the reviewer, the unit and the line it was reading.
        var source = Witness("J4-the-generator-is-asked-to-name-a-reviewer.cs.witness");
        var unit = Assert.Single(AssuranceScanner.Scan(source).Where(static scanned => scanned.IsRelevant));

        Assert.True(unit.Annotation!.HumanIsPending);

        var refused = Assert.Throws<InvalidOperationException>(() => AssuranceGenerator.RefuseInventedApproval(
            unit,
            unit.Annotation!.HumanBody,
            $"WITNESS-ONLY; Fingerprint={WitnessFingerprint}"));

        Assert.Contains("tried to write reviewer 'WITNESS-ONLY'", refused.Message, StringComparison.Ordinal);
        Assert.Contains("whose human line reads 'PENDING'", refused.Message, StringComparison.Ordinal);
        Assert.Contains("Only a human may create an approval.", refused.Message, StringComparison.Ordinal);

        // Clause: the refusal is WIRED, not merely present. The witness above calls the guard
        // directly, which pins the function and not its call site - deleting the one line in
        // RefreshedAnnotations that invokes it left the suite green and the guard unreachable
        // during a real generation. This drives a witness through DesiredSource instead, so the
        // call site is what fails when it goes.
        //
        // The input is a human line outside the four shapes the policy defines. It is the shape
        // that defeated the guard: `PENDING; Fingerprint=TBF` named nobody on either side of the
        // name comparison, so the comparison was satisfied, while the annotation reader answered
        // "not pending" and handed the head token PENDING back as a reviewer identifier.
        var unshaped = Witness("J4-a-human-line-outside-the-defined-shapes.cs.witness");

        var wired = Assert.Throws<InvalidOperationException>(() =>
            AssuranceGenerator.DesiredSource(unshaped, AssuranceScanner.Scan(unshaped)));

        Assert.Contains("will not rewrite the human line on", wired.Message, StringComparison.Ordinal);
        Assert.Contains(
            "which reads 'PENDING; Fingerprint=TBF'", wired.Message, StringComparison.Ordinal);
        Assert.Contains("Only a human may create an approval.", wired.Message, StringComparison.Ordinal);

        // ...and the four defined shapes are accepted by the same guard, so the clause above is a
        // classification and not a refusal of everything.
        Assert.All(
            new[]
            {
                AssuranceAnnotation.Pending,
                "WITNESS-ONLY",
                $"WITNESS-ONLY; Fingerprint={AssuranceFingerprint.ToBeFilled}",
                $"WITNESS-ONLY; Fingerprint={WitnessFingerprint}",
                "STALE; Previous=WITNESS-ONLY@112233",
            },
            static body => Assert.True(
                AssuranceGenerator.IsDefinedHumanBody(body), $"'{body}' is a defined human line"));

        Assert.All(
            new[] { "PENDING; Fingerprint=TBF", "STALE", "STALE; Fingerprint=112233", "Fingerprint=112233", string.Empty },
            static body => Assert.False(
                AssuranceGenerator.IsDefinedHumanBody(body), $"'{body}' is not a defined human line"));

        // Clause: a generation over a PENDING unit fills the fingerprint and touches nothing else.
        Assert.Equal(
            "// Broiler-Human: PENDING",
            RewrittenLine("J4-the-generator-leaves-pending-alone.cs.witness", AssuranceAnnotation.HumanMarker));
        Assert.Equal(
            $"// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint={WitnessFingerprint}",
            RewrittenLine("J4-the-generator-leaves-pending-alone.cs.witness", AssuranceAnnotation.AiMarker));

        // Clause: STALE is not cleared by a recomputation. The AI fingerprint is refreshed and the
        // human line is left exactly as it was - the forbidden edge is unreachable.
        Assert.Equal(
            "// Broiler-Human: STALE; Previous=WITNESS-ONLY@112233",
            RewrittenLine("J4-the-generator-does-not-promote-a-stale-review.cs.witness", AssuranceAnnotation.HumanMarker));
        Assert.Equal(
            $"// Broiler-AI:    Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint={WitnessFingerprint}",
            RewrittenLine("J4-the-generator-does-not-promote-a-stale-review.cs.witness", AssuranceAnnotation.AiMarker));

        // Clause: a review the code has outrun becomes STALE, and who approved what is preserved
        // rather than deleted. The generator writes a reviewer's name here and is allowed to,
        // because it is copying one the source already carried.
        Assert.Equal(
            "// Broiler-Human: STALE; Previous=WITNESS-ONLY@112233",
            RewrittenLine(
                "J4-the-generator-refreshes-a-review-the-code-has-outrun.cs.witness",
                AssuranceAnnotation.HumanMarker));
    }

    /// <summary>The one line of a regenerated witness that opens with the given marker.</summary>
    private static string RewrittenLine(string witness, string marker) =>
        AssuranceProbe.AnnotationLine(WitnessText(witness), marker);

    private static List<string> ApprovalViolations(IEnumerable<AssuranceUnit> units)
    {
        var violations = new List<string>();

        foreach (var unit in units.Where(static unit => unit.Annotation is not null))
        {
            var annotation = unit.Annotation!;

            if (annotation.Reviewer is { } reviewer)
            {
                violations.Add($"{unit.Where} names reviewer '{reviewer}' on its human line");
            }

            if (annotation.Previous is { } previous)
            {
                violations.Add(
                    $"{unit.Where} preserves previous reviewer '{previous.Reviewer}' on its human line");
            }

            if (unit.State == AssuranceReviewState.Verified)
            {
                violations.Add($"{unit.Where} is VERIFIED");
            }

            if (!annotation.HumanIsPending)
            {
                violations.Add($"{unit.Where} carries a human line reading '{annotation.HumanBody}'");
            }
        }

        return violations;
    }

    /// <summary>
    /// Every human line in a generated artefact that reads as anything but <c>PENDING</c>.
    /// </summary>
    /// <remarks>
    /// Asserted against the rendered text rather than against the model, because what a reader
    /// trusts is the file. A defect that produced the right unit state and the wrong line would be
    /// invisible to a check that only read the model.
    /// </remarks>
    private static List<string> ArtefactReviewerViolations() => AssuranceGenerator.Current.Artefacts
        .SelectMany(static artefact => new AssuranceTextLines(artefact.Desired)
            .Where(static line => line.Trim().StartsWith(AssuranceAnnotation.HumanMarker, StringComparison.Ordinal))
            .Where(static line => !string.Equals(
                line.Trim(),
                $"{AssuranceAnnotation.HumanMarker} {AssuranceAnnotation.Pending}",
                StringComparison.Ordinal))
            .Select(line => $"{artefact.RelativePath} carries '{line.Trim()}'"))
        .ToList();

    // =====================================================================================
    // J5 - the generated artefacts are current
    // =====================================================================================

    /// <summary>
    /// J5. Every generated file header and <c>CODE-ASSURANCE.md</c> is byte-identical to what the
    /// generator would produce from the annotations that are in the tree now.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The comparison is by bytes, because "equivalent" is what a stale summary always claims to
    /// be. The policy is explicit that a developer must never maintain a value like
    /// <c>Human-reviewed: 47/47</c> by hand, and this is the check that makes the prohibition
    /// enforceable rather than advisory.
    /// </para>
    /// <para>
    /// The currency half runs in gate mode only. Under <c>BROILER_ASSURANCE_WRITE=1</c> the same
    /// plan is being APPLIED by the generator harness, and a rule that asserted the pre-write text
    /// equalled the post-write text would fail every generation for no reason a reader could act
    /// on. The witnesses run in both modes, so the rule is never a no-op.
    /// </para>
    /// <para>
    /// The report's own figures are checked separately, against the unit set they were computed
    /// from. A byte comparison catches a report that has gone stale; it cannot catch a generator
    /// that would compute the wrong figure and write it consistently, and the figure that matters
    /// most here is the one that says nothing has been reviewed.
    /// </para>
    /// </remarks>
    [Fact]
    public void J5_Every_Generated_Artefact_Is_What_The_Generator_Would_Write()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J5",
            "byte-identical",
            "gate mode",
            "no CI lane",
            "EX-60",
            "EX-64");

        // Non-vacuous: one artefact per covered product file, plus the component report.
        Assert.Equal(AssuranceSources.Files.Count + 1, AssuranceGenerator.Current.Artefacts.Count);
        Assert.Contains(
            AssuranceGenerator.Current.Artefacts,
            static artefact => string.Equals(
                artefact.RelativePath, AssuranceGenerator.ReportPath, StringComparison.Ordinal));

        // The primary clause: what is on disk is what the generator would write. It is
        // AssuranceGenerator.StaleArtefacts and not an expression written out here, because the
        // generator harness asserts the same property and two hand-written copies of one property
        // is how a property ends up asserted nowhere.
        if (!AssuranceGenerator.WriteRequested)
        {
            Assert.Empty(AssuranceGenerator.StaleArtefacts(AssuranceGenerator.Current.Artefacts));
        }

        // Clause: the primary comparison, witnessed. A witness is written to a real file and read
        // back, so the READ half of "desired versus what is actually on disk" runs - every other
        // J5 witness compares a string this test is holding against its own regeneration, which
        // left the clause the register row describes as the gate with no witness at all.
        var staleOnDisk = Assert.Single(AssuranceGenerator.StaleArtefacts(
        [
            AssuranceProbe.ArtefactOnDisk(
                WitnessText("J5-an-artefact-on-disk-is-stale.cs.witness"),
                "J5-an-artefact-on-disk-is-stale.cs.witness"),
        ]));

        Assert.Contains(
            "J5-an-artefact-on-disk-is-stale.cs.witness(9) is not what the generator would write",
            staleOnDisk,
            StringComparison.Ordinal);
        Assert.Contains("on disk:   // Human-reviewed:   1/1", staleOnDisk, StringComparison.Ordinal);
        Assert.Contains("generated: // Human-reviewed:   0/1", staleOnDisk, StringComparison.Ordinal);

        // ...and the accepting direction of the same function, through the same harness: an
        // artefact whose bytes on disk already are the generated ones is not reported.
        Assert.Empty(AssuranceGenerator.StaleArtefacts(
        [
            AssuranceProbe.ArtefactOnDisk(
                WitnessText("J5-the-header-is-current.cs.witness"),
                "J5-the-header-is-current.cs.witness"),
        ]));

        // Clause: exactly one generated block per covered file, said as its own fact. A second
        // block pasted below the first used to survive regeneration verbatim and become a fixed
        // point, so the byte comparison was satisfied by a file publishing a forged summary.
        Assert.Empty(AssuranceGenerator.DuplicateAssuranceBlocks(AssuranceSources.Files));

        var doubled = Assert.Single(AssuranceGenerator.DuplicateAssuranceBlocks(
            [Witness("J5-a-second-assurance-block-below-the-header.cs.witness")]));

        Assert.Contains(
            "J5-a-second-assurance-block-below-the-header.cs.witness carries 2 'Broiler Code Assurance' banners",
            doubled,
            StringComparison.Ordinal);

        // ...and the forgery is removed rather than republished: regenerating the same witness
        // deletes the second block instead of reproducing it.
        var forged = RegenerationDifference("J5-a-second-assurance-block-below-the-header.cs.witness");
        Assert.NotNull(forged);
        Assert.Contains(
            "J5-a-second-assurance-block-below-the-header.cs.witness(17) is not what the generator would write",
            forged,
            StringComparison.Ordinal);
        Assert.Contains("on disk: '// Broiler Code Assurance'", forged, StringComparison.Ordinal);
        Assert.Contains("generated: 'namespace Probe;'", forged, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "// Human-reviewed:   1/1",
            RegeneratedText("J5-a-second-assurance-block-below-the-header.cs.witness"),
            StringComparison.Ordinal);
        Assert.Equal(
            1,
            AssuranceGenerator.BannerCount(RegeneratedText(
                "J5-a-second-assurance-block-below-the-header.cs.witness")));

        // Clause: the accepted direction. A file already in the shape the generator writes is
        // regenerated to itself, so the rule is a comparison and not a rewrite.
        Assert.Null(RegenerationDifference("J5-the-header-is-current.cs.witness"));

        // Clause: a count in the header that is not true of the file it sits on. This is the whole
        // point of generating the summary - the policy forbids a hand-maintained one.
        var miscounted = RegenerationDifference("J5-header-states-a-count-that-is-not-true.cs.witness");
        Assert.NotNull(miscounted);
        Assert.Contains("on disk: '// Relevant units:   9'", miscounted, StringComparison.Ordinal);
        Assert.Contains("generated: '// Relevant units:   1'", miscounted, StringComparison.Ordinal);

        // Clause: a header claiming a human review that the annotations do not support. The same
        // machinery as above, and named separately because it is the claim that matters.
        var claimed = RegenerationDifference("J5-header-claims-a-human-review.cs.witness");
        Assert.NotNull(claimed);
        Assert.Contains("on disk: '// Human-reviewed:   1/1'", claimed, StringComparison.Ordinal);
        Assert.Contains("generated: '// Human-reviewed:   0/1'", claimed, StringComparison.Ordinal);

        // Clause: a file with annotations and no header at all gets one.
        var bare = RegenerationDifference("J5-file-carries-no-generated-header.cs.witness");
        Assert.NotNull(bare);
        Assert.Contains("on disk: 'namespace Probe;'", bare, StringComparison.Ordinal);
        Assert.Contains(
            "generated: '// SPDX-FileCopyrightText: 2026 Broiler Platform contributors'",
            bare,
            StringComparison.Ordinal);

        // Clause: a hand-written SPDX pair with no generated marker under it is REPLACED, not
        // stacked on top of. Leaving it in place published the licence declaration twice, which is
        // the shape every file in this component was in before its first generation.
        var adopting = Witness("J5-spdx-header-without-the-generated-marker.cs.witness");
        var adopted = AssuranceGenerator.DesiredSource(adopting, AssuranceScanner.Scan(adopting));

        Assert.NotEqual(adopting.Text, adopted);
        Assert.Equal(1, new AssuranceTextLines(adopted).Count(static line =>
            line.StartsWith("// SPDX-FileCopyrightText:", StringComparison.Ordinal)));
        Assert.Equal(1, new AssuranceTextLines(adopted).Count(static line =>
            line.StartsWith("// SPDX-License-Identifier:", StringComparison.Ordinal)));

        // ---- The report's own figures, against the units they were computed from ----

        Assert.Empty(ReportViolations(ComponentReport, ProductUnits));

        // The report says, in its own words, that nothing here has been reviewed. Asserted on the
        // text a reader sees, because that sentence is the report's whole subject.
        Assert.Contains(
            "**Nothing in this component has been reviewed by a human.**",
            ComponentReport,
            StringComparison.Ordinal);
        Assert.Contains("| Human reviewed | 0 of ", ComponentReport, StringComparison.Ordinal);
        Assert.Contains("| VERIFIED | 0 |", ComponentReport, StringComparison.Ordinal);
        Assert.Contains("no CI lane", ComponentReport, StringComparison.Ordinal);

        // One probe unit set, four report witnesses, one clause each.
        var probe = WitnessUnits("J5-one-pending-unit.cs.witness");

        Assert.Equal(AssuranceReviewState.HumanPending, Assert.Single(probe).State);

        var overstated = Assert.Single(ReportViolations(
            WitnessText("J5-report-claims-a-human-review.md.witness"), probe));
        Assert.Contains(
            "states Human reviewed 1 of 1 where the annotations give 0 of 1",
            overstated,
            StringComparison.Ordinal);

        var invented = Assert.Single(ReportViolations(
            WitnessText("J5-report-claims-a-verified-unit.md.witness"), probe));
        Assert.Contains(
            "states VERIFIED 1 where the annotations give 0",
            invented,
            StringComparison.Ordinal);

        var understated = Assert.Single(ReportViolations(
            WitnessText("J5-report-understates-the-unverified-count.md.witness"), probe));
        Assert.Contains(
            "states Unverified 0 where the annotations give 1",
            understated,
            StringComparison.Ordinal);

        var silent = Assert.Single(ReportViolations(
            WitnessText("J5-report-drops-the-statement-of-absence.md.witness"), probe));
        Assert.Contains(
            "no unit is human-reviewed and the report does not say so in its own words",
            silent,
            StringComparison.Ordinal);
    }

    /// <summary>The first line at which a witness and its own regeneration part company.</summary>
    private static string? RegenerationDifference(string witness)
    {
        var file = Witness(witness);

        return Difference(witness, file.Text, AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file)));
    }

    /// <summary>What the generator would write for a witness.</summary>
    private static string RegeneratedText(string witness)
    {
        var file = Witness(witness);

        return AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file));
    }

    private static string? Difference(string name, string current, string desired)
    {
        var onDisk = new AssuranceTextLines(current);
        var generated = new AssuranceTextLines(desired);

        for (var line = 0; line < Math.Max(onDisk.Count, generated.Count); line++)
        {
            var left = line < onDisk.Count ? onDisk[line] : "<end of file>";
            var right = line < generated.Count ? generated[line] : "<end of file>";

            if (!string.Equals(left, right, StringComparison.Ordinal))
            {
                return $"{name}({line + 1}) is not what the generator would write. " +
                    $"on disk: '{left}'; generated: '{right}'";
            }
        }

        return onDisk.Count == generated.Count ? null : $"{name} differs in length only";
    }

    /// <summary>
    /// The figures the component report states, against the unit set it was generated from.
    /// </summary>
    /// <remarks>
    /// Three counts and one sentence. The counts are the ones a reader takes a decision on, and
    /// the sentence is the one this milestone exists to publish; each is compared by VALUE against
    /// the annotations, so a report that kept the shape and changed the number is reported.
    /// </remarks>
    private static List<string> ReportViolations(string report, IReadOnlyList<AssuranceUnit> units)
    {
        var summary = AssuranceSummary.Of(units);
        var violations = new List<string>();

        var reviewed = Regex.Match(report, @"\|\s*Human reviewed\s*\|\s*(?<part>\d+) of (?<whole>\d+)");

        if (!reviewed.Success)
        {
            violations.Add("the report states no 'Human reviewed' row");
        }
        else
        {
            var part = Number(reviewed.Groups["part"].Value);
            var whole = Number(reviewed.Groups["whole"].Value);

            if (part != summary.Verified || whole != summary.Relevant)
            {
                violations.Add(
                    $"the report states Human reviewed {part} of {whole} where " +
                    $"the annotations give {summary.Verified} of {summary.Relevant}");
            }
        }

        Count("VERIFIED", @"\|\s*VERIFIED\s*\|\s*(?<n>\d+)\s*\|",
            units.Count(static unit => unit.State == AssuranceReviewState.Verified));

        Count("Unverified", @"\|\s*Unverified\s*\|\s*(?<n>\d+)\s*\|", summary.Unverified);

        if (summary.Verified == 0 &&
            !report.Contains(
                "**Nothing in this component has been reviewed by a human.**",
                StringComparison.Ordinal))
        {
            violations.Add("no unit is human-reviewed and the report does not say so in its own words");
        }

        return violations;

        void Count(string label, string pattern, int expected)
        {
            var row = Regex.Match(report, pattern);

            if (!row.Success)
            {
                violations.Add($"the report states no '{label}' row");
            }
            else if (Number(row.Groups["n"].Value) != expected)
            {
                violations.Add(
                    $"the report states {label} {Number(row.Groups["n"].Value)} " +
                    $"where the annotations give {expected}");
            }
        }

        static int Number(string text) => int.Parse(text, CultureInfo.InvariantCulture);
    }

    // =====================================================================================
    // J6 - no covered file carries a preprocessor directive
    // =====================================================================================

    /// <summary>
    /// J6. No covered product source file contains a preprocessor directive of any kind, and the
    /// scanner's parse defines the conditional symbols the build defines.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This rule exists because a directive is trivia, and trivia is what the fingerprint excludes
    /// so that <c>dotnet format</c> cannot invalidate a review. The exclusion is right for
    /// whitespace and comments and catastrophic for a conditional region: parsed with no symbol
    /// defined, a <c>#if NET10_0_OR_GREATER</c> block the build COMPILES becomes disabled text, so
    /// a backdoor could be added to an annotated method with its recorded fingerprint unmoved, and
    /// a whole public method could ship outside the unit set. That is not a 24-bit collision; it
    /// is the fingerprint's central claim - a semantically relevant change must invalidate -
    /// failing outright.
    /// </para>
    /// <para>
    /// <b>Two defences, deliberately independent.</b> The parse now defines a superset of the
    /// build's symbols, so a region that ships is scanned, annotated and fingerprinted like any
    /// other code; and this rule forbids the directive in a covered file at all, so the situation
    /// does not arise. Either alone would close the hole found; both are here because each fails
    /// differently - the symbol list can fall behind a new target framework, and a rule can be
    /// deleted. The product tree carries zero directives today, which is why the prohibition costs
    /// nothing to adopt.
    /// </para>
    /// <para>
    /// The limit is the corpus, and it is the same limit rule J1 carries: this rule reads the files
    /// the scanner enumerates, so a compiled file outside that set can carry any directive it likes
    /// and is not examined. That is EX-63.
    /// </para>
    /// </remarks>
    [Fact]
    public void J6_No_Covered_Source_File_Carries_A_Preprocessor_Directive()
    {
        AssertTheRegisterRowStatesItsLimits(
            "J6",
            "any preprocessor directive",
            "conditional symbols",
            "trivia",
            "EX-63");

        // The clean direction. Non-vacuous: 45 covered files are read, and the check is over the
        // parse rather than over a text search, so a directive inside a string literal is not one.
        Assert.Empty(AssuranceSources.DirectiveViolations(AssuranceSources.Files));
        Assert.NotEmpty(AssuranceSources.Files);

        // Clause: a conditional region. Both the opening and the closing directive are named,
        // because a rule that reported only the `#if` would accept a file whose region was opened
        // somewhere it could not see.
        var conditional = AssuranceSources.DirectiveViolations(
            [Witness("J6-a-conditional-region-hides-code.cs.witness")]);

        Assert.Equal(2, conditional.Count);
        Assert.Single(conditional.Where(static problem => problem.Contains(
            "carries the preprocessor directive '#if NET10_0_OR_GREATER'", StringComparison.Ordinal)));
        Assert.Single(conditional.Where(static problem => problem.Contains(
            "carries the preprocessor directive '#endif'", StringComparison.Ordinal)));

        // Clause: a directive that compiles nothing away is still a directive. The rule is about
        // the whole class, not about `#if`.
        var pragma = AssuranceSources.DirectiveViolations(
            [Witness("J6-a-directive-that-is-not-conditional.cs.witness")]);

        Assert.Equal(2, pragma.Count);
        Assert.Single(pragma.Where(static problem => problem.Contains(
            "carries the preprocessor directive '#pragma warning disable CA1822'",
            StringComparison.Ordinal)));
        Assert.Single(pragma.Where(static problem => problem.Contains(
            "carries the preprocessor directive '#pragma warning restore CA1822'",
            StringComparison.Ordinal)));

        // Clause: the second defence, asserted apart from the first. Under the scanner's parse the
        // guarded region is ACTIVE - no disabled text anywhere in the witness - so the method
        // inside it is a unit with a fingerprint over its real tokens, which is precisely what the
        // default parse options did not give.
        var region = Witness("J6-a-conditional-region-hides-code.cs.witness");

        Assert.DoesNotContain(
            region.Tree.GetRoot().DescendantTrivia(descendIntoTrivia: true),
            static trivia => trivia.IsKind(SyntaxKind.DisabledTextTrivia));

        var guarded = Assert.Single(AssuranceScanner.Scan(region));

        Assert.Contains("Conditioned.Admit(uint)", guarded.Name, StringComparison.Ordinal);
        Assert.Contains("4242", AssuranceFingerprint.TokenStream(guarded.Declaration), StringComparison.Ordinal);

        // ...and the symbol list is the build's, not a wish: every symbol the parse defines is one
        // the SDK defines for net10.0 in one of the two configurations, and the one the attack used
        // is in it.
        Assert.Contains("NET10_0_OR_GREATER", AssuranceSources.PreprocessorSymbols, StringComparer.Ordinal);
    }

    // =====================================================================================
    // The register rows are held to the limits their rules depend on
    // =====================================================================================

    /// <summary>
    /// Nothing else in the suite reads a register row's prose: <c>RuleRegisterTests</c> asserts
    /// only that the fields are non-empty. The one field carrying a rule's honest limits could
    /// therefore be rewritten into an over-claim in a single edit with the suite green, and an
    /// over-claim is by definition a rule weaker than its own statement - the standing defect the
    /// register exists to prevent. Group H carries the same helper for the same reason; the two
    /// are deliberately separate so that neither group's rows depend on the other group's file.
    /// </summary>
    private static void AssertTheRegisterRowStatesItsLimits(string id, params string[] required)
    {
        var prose = RegisterRowProse(id);

        foreach (var phrase in required)
        {
            Assert.True(
                prose.Contains(phrase, StringComparison.OrdinalIgnoreCase),
                $"The register row {id} no longer states the limit \"{phrase}\", so the row claims more " +
                "than the rule delivers.");
        }
    }

    private static string RegisterRowProse(string id)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json");

        using var register = JsonDocument.Parse(
            File.ReadAllText(path),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });

        foreach (var rule in register.RootElement.GetProperty("rules").EnumerateArray())
        {
            if (!string.Equals(rule.GetProperty("id").GetString(), id, StringComparison.Ordinal))
            {
                continue;
            }

            return string.Join(
                "\n",
                new[] { "statement", "evidence", "nonVacuousWhen" }.Select(field =>
                    rule.TryGetProperty(field, out var value) ? value.GetString() ?? string.Empty : string.Empty));
        }

        throw new InvalidOperationException($"The register has no row {id}.");
    }
}
