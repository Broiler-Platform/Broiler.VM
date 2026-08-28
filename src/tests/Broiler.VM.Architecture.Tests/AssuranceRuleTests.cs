using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
/// <b>Watched is not the same as annotated, and J7 is where that separation lives.</b> Three
/// adversarial rounds put nearly every blocker in one place: a unit the exemption predicate treats
/// as trivial carries no annotation, therefore no fingerprint, therefore no record of any kind, so
/// a semantic change to it was invisible to J1 through J6. Narrowing the predicate case by case
/// failed three times because each fix moved the same defeat one case over. J7 holds
/// <c>assurance.manifest.json</c> to EVERY code unit instead. Whether a unit needs a human
/// annotation is still the predicate's question; whether it is watched for change is no longer a
/// question. The manifest is a change-detection record and not a review, and it says so in its own
/// header, in <c>CODE-ASSURANCE.md</c> and in J7's register row.
/// </para>
/// <para>
/// <b>A fourth round attacked the two things left, and both answers are structural as well.</b>
/// It attacked the unit ENUMERATION rather than the predicate: a unit exists only for a declaration
/// kind the scanner names, so an enum member, a type declaration header carrying a primary
/// constructor, an event field declaration and an <c>[assembly: InternalsVisibleTo]</c> were each
/// in no unit and no manifest entry. The first three are units now; the fourth is a member of
/// nothing and can never be one, so J7 also holds a fingerprint over every covered FILE, taken over
/// its complete token stream. And it attacked the GENERATED ARTEFACTS: two lines appended to the
/// manifest's own header made it assert that every unit was verified, human-reviewed and eligible
/// for release, with both modes green, because J5 compares what is on disk against what the
/// generator would write and the generator would write exactly that. J8 holds every generated
/// artefact to a hand-maintained shape and re-derives every value; J9 forbids the review vocabulary
/// unless the annotations support the occurrence. The two are deliberately independent - J8 can be
/// defeated by editing the shape as well as the generator, and J9 cannot be defeated by editing
/// anything but its term list.
/// </para>
/// <para>
/// Each rule also holds its own register row to a FIXED EXPECTED TEXT, in all three of its prose
/// fields. Nothing else in the suite reads a row's prose, and the substring check this replaced let
/// a row CLAIM a capability the tests do not implement as easily as it let one shed a limit: a
/// sentence saying CI compares every human line against the parent commit was appended to a row and
/// the suite stayed green, and no such mechanism exists in this component.
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

    /// <summary>The one unit of a witness with this exact name.</summary>
    private static AssuranceUnit WitnessUnit(string fileName, string name) =>
        WitnessUnits(fileName).Single(unit => string.Equals(unit.Name, name, StringComparison.Ordinal));

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
        AssertTheRegisterRowIsWhatTheRulesImplement("J1");

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

        // Clause: the predicate decides relevance. Eight exempt units, no annotation, no violation -
        // the half of the rule that keeps it from becoming a rule about comments. The ninth unit is
        // the carrier type declaration, which is relevant and carries an annotation of its own, as
        // it does in every witness in this directory: a witness that failed this rule on its own
        // carrier would not be a realistic input.
        var trivial = WitnessUnits("J1-exempt-units-need-no-annotation.cs.witness");
        Assert.Equal(9, trivial.Count);
        Assert.Equal(8, trivial.Count(static unit => unit.IsExempt));
        Assert.All(
            trivial.Where(static unit => unit.Declaration is not BaseTypeDeclarationSyntax),
            static unit => Assert.True(unit.IsExempt, unit.Where));
        Assert.Empty(CoverageViolations(trivial));

        // Clause: the per-unit escape hatch, for what the predicate cannot see. Shim would
        // otherwise be relevant; a reason a human wrote outranks the predicate, and the case is
        // reported apart from the eight so that it stays countable in the component report.
        var hatched = WitnessUnits("J1-an-explicit-exemption-covers-a-unit.cs.witness");
        var shim = Assert.Single(hatched.Where(static unit =>
            unit.Name.EndsWith(".Shim(int)", StringComparison.Ordinal)));
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

        // ...and the other half: EVERY field declaration is a unit, and the two that state no fixed
        // value are exempt rather than absent. Six units in that witness, `used` and `mutable`
        // among them - so a change to either one's type moves a manifest fingerprint under J7,
        // while neither needs an annotation here.
        Assert.Equal(
            new[]
            {
                "Probe.Budgeted",
                "Probe.Budgeted.MaximumEntries",
                "Probe.Budgeted.Mutable",
                "Probe.Budgeted.Used",
                "Probe.Budgeted.Widths",
                "Probe.Budgeted.mutable",
                "Probe.Budgeted.used",
            },
            constants.Select(static unit => unit.Name).OrderBy(static name => name, StringComparer.Ordinal));

        Assert.Equal(
            new[] { "Probe.Budgeted.mutable", "Probe.Budgeted.used" },
            constants
                .Where(static unit => unit.Exemption == AssuranceExemption.FieldDeclaringStorage)
                .Select(static unit => unit.Name)
                .OrderBy(static name => name, StringComparer.Ordinal));

        // Clause: exemption case 1 requires the SAME correspondence case 2 requires. A property
        // that publishes a field other than its own is the permuted-constructor defeat one screen
        // further down the predicate, and the accessor form of it is the same decision written
        // longhand. Both must be relevant; the two that return their own field stay exempt, so the
        // case is narrowed rather than deleted.
        var published = CoverageViolations(
            WitnessUnits("J1-a-property-that-publishes-another-field-is-relevant.cs.witness"));

        Assert.Equal(2, published.Count);
        Assert.Single(published.Where(static claim => claim.Contains(
            "Probe.Published.MaxSectionCount is relevant", StringComparison.Ordinal)));
        Assert.Single(published.Where(static claim => claim.Contains(
            "Probe.Published.SectionCeiling is relevant", StringComparison.Ordinal)));

        Assert.All(
            WitnessUnits("J1-a-property-that-publishes-another-field-is-relevant.cs.witness")
                .Where(static unit => unit.Name.EndsWith(".MaxDeclaredCount", StringComparison.Ordinal) ||
                    unit.Name.EndsWith(".Depth", StringComparison.Ordinal)),
            static unit => Assert.Equal(AssuranceExemption.TrivialPropertyOrAccessor, unit.Exemption));

        // Clause: exemption case 5 reaches a member that hands its question on unchanged, and a
        // negation changes it. `!left.Equals(right)` is the opposite decision, and with `!` on the
        // whitelist every operator != in the component was exempt.
        var negated = Assert.Single(CoverageViolations(
            WitnessUnits("J1-a-negated-delegation-is-relevant.cs.witness")));

        Assert.Contains(
            "Probe.Negated.operator !=(Negated, Negated) is relevant",
            negated,
            StringComparison.Ordinal);

        Assert.Equal(
            AssuranceExemption.DelegatingOverrideOrOperator,
            WitnessUnits("J1-a-negated-delegation-is-relevant.cs.witness")
                .Single(static unit => unit.Name.Contains("operator ==", StringComparison.Ordinal))
                .Exemption);

        // Clause: the escape hatch is closed to the assembly that reads untrusted input. Replacing
        // every AI line in VmBoundedReader.cs with one sentence that refers to nothing left both
        // modes green, because an exempt unit needs no annotation and an unannotated unit records
        // no fingerprint.
        Assert.Empty(AssuranceScanner.EscapeHatchViolations(ProductUnits));

        var closed = Assert.Single(AssuranceScanner.EscapeHatchViolations(AssuranceScanner.Scan(
            AssuranceProbe.Source(
                WitnessText("J1-the-escape-hatch-is-closed-to-the-binary-reader.cs.witness"),
                "J1-the-escape-hatch-is-closed-to-the-binary-reader.cs.witness",
                "Broiler.VM.Binary"))));

        Assert.Contains(
            "states EXEMPT=a bounds check that the reader performs elsewhere",
            closed,
            StringComparison.Ordinal);
        Assert.Contains(
            "Broiler.VM.Binary is closed to the per-unit exemption",
            closed,
            StringComparison.Ordinal);

        // ...and the accepting direction of the same clause: the identical hatch in an assembly
        // that is open to it is not reported here, so this is a rule about where the hatch is used
        // and not a prohibition on the hatch, which J1 witnesses being accepted two clauses above.
        Assert.Empty(AssuranceScanner.EscapeHatchViolations(AssuranceScanner.Scan(
            AssuranceProbe.Source(
                WitnessText("J1-the-escape-hatch-is-closed-to-the-binary-reader.cs.witness"),
                "J1-the-escape-hatch-is-closed-to-the-binary-reader.cs.witness",
                "Broiler.VM.Runtime"))));

        // Clause: a TYPE DECLARATION HEADER is a code unit, and a primary constructor is declared
        // in one. Round one's permuted-ceiling defeat came back verbatim through this shape: case 2
        // was narrowed so that a constructor BODY assigning its parameters to the wrong members is
        // relevant, and a primary constructor has no body - the whole decision is the declaration
        // line, which was in no unit at all.
        var headers = Assert.Single(CoverageViolations(
            WitnessUnits("J1-a-primary-constructor-permutation-is-relevant.cs.witness")));

        Assert.Contains(
            "Probe.Bounds is relevant and carries no assurance annotation",
            headers,
            StringComparison.Ordinal);

        // ...and the token streams in full, because what matters is not that the header is a unit
        // but WHAT is in its fingerprint. The parameter list is, so exchanging two parameters moves
        // the value; the member list is not, so a member's own fingerprint is what records it.
        Assert.Equal(
            "public readonly record struct Bounds ( ulong MaxSectionCount , ulong MaxDeclaredCount ) ;",
            AssuranceFingerprint.TokenStream(WitnessUnit(
                "J1-a-primary-constructor-permutation-is-relevant.cs.witness", "Probe.Bounds").Declaration));
        Assert.Equal(
            "public readonly record struct Permuted ( ulong MaxDeclaredCount , ulong MaxSectionCount ) ;",
            AssuranceFingerprint.TokenStream(WitnessUnit(
                "J1-a-primary-constructor-permutation-is-relevant.cs.witness", "Probe.Permuted").Declaration));
        Assert.Equal(
            "public sealed class Ceiling ( ulong maxSectionCount ) : System . IComparable",
            AssuranceFingerprint.TokenStream(WitnessUnit(
                "J1-a-primary-constructor-permutation-is-relevant.cs.witness", "Probe.Ceiling").Declaration));

        // Clause: an ENUM MEMBER is a unit and is exempt, because the vocabulary is the reviewable
        // thing; the enum DECLARATION is a unit whose fingerprint covers every member and every
        // value, and it is relevant. The budget dimensions and the reason codes of this component
        // are enum members, and under the old enumeration a changed value moved nothing at all.
        var vocabulary = WitnessUnits("J1-an-enum-member-is-covered-by-its-vocabulary.cs.witness");

        Assert.Equal(
            new[] { "Probe.ProbeDimension.AllocatedBytes", "Probe.ProbeDimension.VerifierWork", "Probe.ProbeReason.None" },
            vocabulary
                .Where(static unit => unit.Exemption == AssuranceExemption.EnumMemberOfADeclaredVocabulary)
                .Select(static unit => unit.Name)
                .OrderBy(static name => name, StringComparer.Ordinal));

        var unassessed = Assert.Single(CoverageViolations(vocabulary));

        Assert.Contains(
            "Probe.ProbeReason is relevant and carries no assurance annotation",
            unassessed,
            StringComparison.Ordinal);

        // ...and the enum's annotation is bound to the vocabulary and not to the word `enum`: the
        // members and their values are in the declaration's token stream, so revaluing one makes
        // the review stale.
        Assert.Equal(
            "public enum ProbeDimension { AllocatedBytes = 2 , VerifierWork = 6 , }",
            AssuranceFingerprint.TokenStream(WitnessUnit(
                "J1-an-enum-member-is-covered-by-its-vocabulary.cs.witness", "Probe.ProbeDimension").Declaration));

        // Clause: a DELEGATE DECLARATION and an EVENT FIELD DECLARATION are units, and neither is
        // exempt. The event field is the sibling of the field declarations the enumeration already
        // had, and case 7 deliberately does not reach it: a field-like event declares a public
        // broadcast point whose whole visible content is its declaration.
        var broadcast = WitnessUnits("J1-a-delegate-and-an-event-field-are-relevant.cs.witness");

        Assert.Equal(
            AssuranceExemption.None,
            broadcast.Single(static unit => unit.Name.EndsWith(".Escaped", StringComparison.Ordinal)).Exemption);
        Assert.Equal(
            AssuranceExemption.None,
            broadcast.Single(static unit => unit.Name.EndsWith(".ProbeVerifier", StringComparison.Ordinal)).Exemption);

        var broadcaster = Assert.Single(CoverageViolations(broadcast));

        Assert.Contains(
            "Probe.Broadcaster.Escaped is relevant and carries no assurance annotation",
            broadcaster,
            StringComparison.Ordinal);

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
        AssertTheRegisterRowIsWhatTheRulesImplement("J2");

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
        AssertTheRegisterRowIsWhatTheRulesImplement("J3");

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
        Assert.Equal(
            AssuranceReviewState.Stale,
            preserved.Single(static unit => unit.Name.EndsWith(".Fold(int[])", StringComparison.Ordinal)).State);
        Assert.Empty(FingerprintViolations(preserved));

        // Clause: the accepted direction. A human fingerprint that IS the current one is current,
        // whatever J4 goes on to say about this component carrying no approvals at all.
        var current = WitnessUnits("J3-a-review-of-the-current-version-is-current.cs.witness");
        Assert.Equal(
            AssuranceReviewState.Verified,
            current.Single(static unit => unit.Name.EndsWith(".Fold(int[])", StringComparison.Ordinal)).State);
        Assert.Empty(FingerprintViolations(current));

        // Clause: the specification's per-unit escape hatch records no fingerprint, and both
        // halves of this rule accept it. The comparison tolerates the missing value, and the
        // well-formedness assertion above does not reach an EXEMPT line - which it used to, so
        // the documented hatch failed J3 the moment anyone used it.
        var hatched = WitnessUnits("J3-an-exempt-annotation-records-no-fingerprint.cs.witness");
        var exempt = Assert.Single(hatched.Where(static unit =>
            unit.Name.EndsWith(".Shim(int[])", StringComparison.Ordinal)));

        Assert.Null(exempt.Annotation!.RecordedFingerprint);
        Assert.Equal(AssuranceReviewState.Exempt, exempt.State);
        Assert.Empty(FingerprintViolations(hatched));
        Assert.Empty(VocabularyViolations(hatched));
        Assert.Empty(hatched.Where(static unit =>
            unit.Annotation is { ExemptReason: null } &&
            unit.Name.EndsWith(".Shim(int[])", StringComparison.Ordinal)));
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
        AssertTheRegisterRowIsWhatTheRulesImplement("J4");

        // The clean direction, at the source and in the artefacts. Non-vacuous: 496 annotated
        // units, every one of them PENDING.
        Assert.Empty(ApprovalViolations(ProductUnits));
        Assert.Empty(ArtefactReviewerViolations(AssuranceGenerator.Current.Artefacts));
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
        Assert.Equal(
            AssuranceReviewState.AiAssessed,
            pending.Single(static unit => unit.Name.EndsWith(".Fold(int[])", StringComparison.Ordinal)).State);
        Assert.Empty(ApprovalViolations(pending));

        // Clause: the refusal. The generator is handed the transformation the policy forbids and
        // must throw, naming the reviewer, the unit and the line it was reading.
        var source = Witness("J4-the-generator-is-asked-to-name-a-reviewer.cs.witness");
        var unit = Assert.Single(AssuranceScanner.Scan(source).Where(static scanned =>
            scanned.Name.EndsWith(".Fold(int[])", StringComparison.Ordinal)));

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

        // Clause: the artefact half, REJECTING. The clean direction above ran over the real tree
        // and had no counterpart, so the whole of ArtefactReviewerViolations could be replaced by
        // an empty list with the suite green - an accepting-only assertion cannot tell a check
        // that found nothing from a check that looks for nothing.
        //
        // The witness carries a human line in one of the four defined shapes naming the version
        // that is here, so the generator carries it through and the artefact it would write
        // contains a reviewer identifier.
        var rendered = Assert.Single(ArtefactReviewerViolations(
        [
            AssuranceProbe.ArtefactOnDisk(
                WitnessText("J4-an-artefact-carries-a-reviewer.cs.witness"),
                "J4-an-artefact-carries-a-reviewer.cs.witness"),
        ]));

        Assert.Contains(
            "J4-an-artefact-carries-a-reviewer.cs.witness carries " +
            $"'// Broiler-Human: WITNESS-ONLY; Fingerprint={WitnessFingerprint}'",
            rendered,
            StringComparison.Ordinal);

        // ...and the accepting direction through the same harness, so the clause is a comparison
        // rather than a function that reports every artefact it is handed.
        Assert.Empty(ArtefactReviewerViolations(
        [
            AssuranceProbe.ArtefactOnDisk(
                WitnessText("J4-a-pending-line-claims-nothing.cs.witness"),
                "J4-a-pending-line-claims-nothing.cs.witness"),
        ]));
    }

    /// <summary>The one line of a regenerated witness that opens with the given marker.</summary>
    private static string RewrittenLine(string witness, string marker) =>
        AssuranceProbe.AnnotationLine(WitnessText(witness), ".Fold(int[])", marker);

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
    /// <para>
    /// Asserted against the rendered text rather than against the model, because what a reader
    /// trusts is the file. A defect that produced the right unit state and the wrong line would be
    /// invisible to a check that only read the model.
    /// </para>
    /// <para>
    /// It takes the artefact list as an argument so that it can be shown REJECTING one. Its only
    /// assertion used to be the accepting direction over the real tree, with no rejecting
    /// counterpart anywhere in the suite: the whole body could be replaced by
    /// <c>new List&lt;string&gt;()</c> and nothing went red, which makes it a check that reports
    /// what it is given and cannot be told apart from one that reports nothing at all.
    /// </para>
    /// </remarks>
    private static List<string> ArtefactReviewerViolations(IEnumerable<AssuranceArtefact> artefacts) => artefacts
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
        AssertTheRegisterRowIsWhatTheRulesImplement("J5");

        // Non-vacuous: one artefact per covered product file, plus the component report and the
        // manifest.
        Assert.Equal(AssuranceSources.Files.Count + 2, AssuranceGenerator.Current.Artefacts.Count);
        Assert.Contains(
            AssuranceGenerator.Current.Artefacts,
            static artefact => string.Equals(
                artefact.RelativePath, AssuranceGenerator.ReportPath, StringComparison.Ordinal));
        Assert.Contains(
            AssuranceGenerator.Current.Artefacts,
            static artefact => string.Equals(
                artefact.RelativePath, AssuranceManifest.RelativePath, StringComparison.Ordinal));

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
        Assert.Contains("on disk:   // Human-reviewed:   2/2", staleOnDisk, StringComparison.Ordinal);
        Assert.Contains("generated: // Human-reviewed:   0/2", staleOnDisk, StringComparison.Ordinal);

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
            "// Human-reviewed:   2/2",
            RegeneratedText("J5-a-second-assurance-block-below-the-header.cs.witness"),
            StringComparison.Ordinal);
        Assert.Equal(
            1,
            AssuranceGenerator.BannerCount(RegeneratedText(
                "J5-a-second-assurance-block-below-the-header.cs.witness")));

        // Clause: a forged block INDENTED inside a class body. This one input defeated both halves
        // of the defence at once - the count compared only the END of the line, so leading
        // whitespace hid the second banner, and the header stripper reads the leading comment run,
        // which a block inside a class is not part of.
        //
        // The regeneration of that witness is byte-identical to the witness, and that is asserted
        // rather than glossed: it is what makes the two clauses below necessary rather than
        // redundant. The byte comparison cannot see this forgery at all.
        Assert.Null(RegenerationDifference("J5-an-indented-assurance-block-inside-a-class.cs.witness"));

        var indented = Assert.Single(AssuranceGenerator.DuplicateAssuranceBlocks(
            [Witness("J5-an-indented-assurance-block-inside-a-class.cs.witness")]));

        Assert.Contains(
            "J5-an-indented-assurance-block-inside-a-class.cs.witness carries 2 " +
            "'Broiler Code Assurance' banners",
            indented,
            StringComparison.Ordinal);

        var below = Assert.Single(AssuranceGenerator.ForgedAssuranceBlocks(
            [Witness("J5-an-indented-assurance-block-inside-a-class.cs.witness")]));

        Assert.Contains(
            "carries the assurance summary line '// Broiler Code Assurance' below the generated header",
            below,
            StringComparison.Ordinal);

        // Clause: a forgery that drops the banner and keeps the rows. The banner count sees one
        // summary and reports nothing, and the regeneration is byte-identical, so the row labels
        // are the only thing left that recognises it. Dropping one line is the cheapest way past a
        // rule that looks for one line.
        Assert.Null(RegenerationDifference("J5-a-summary-block-that-drops-its-banner.cs.witness"));
        Assert.Equal(
            1,
            AssuranceGenerator.BannerCount(
                WitnessText("J5-a-summary-block-that-drops-its-banner.cs.witness")));
        Assert.Empty(AssuranceGenerator.DuplicateAssuranceBlocks(
            [Witness("J5-a-summary-block-that-drops-its-banner.cs.witness")]));

        var bannerless = Assert.Single(AssuranceGenerator.ForgedAssuranceBlocks(
            [Witness("J5-a-summary-block-that-drops-its-banner.cs.witness")]));

        Assert.Contains(
            "carries the assurance summary line '// Relevant units:   1' below the generated header",
            bannerless,
            StringComparison.Ordinal);

        // Clause: a forged block WORDED DIFFERENTLY and in a different case. The defence this
        // replaces was a literal-string whitelist - exact ordinal equality against the banner and
        // the marker, and an ordinal StartsWith against eight labels - so a block that shouts its
        // labels, drops the banner and names a person was not a block at all. It is indented inside
        // the class body, so the header stripper cannot reach it, the regeneration is
        // byte-identical and the banner count sees one summary: the vocabulary clause is the only
        // thing that reports it, which is asserted rather than assumed.
        Assert.Null(RegenerationDifference("J5-a-forged-block-worded-differently.cs.witness"));
        Assert.Equal(
            1,
            AssuranceGenerator.BannerCount(
                WitnessText("J5-a-forged-block-worded-differently.cs.witness")));
        Assert.Empty(AssuranceGenerator.DuplicateAssuranceBlocks(
            [Witness("J5-a-forged-block-worded-differently.cs.witness")]));

        var reworded = Assert.Single(AssuranceGenerator.ForgedAssuranceBlocks(
            [Witness("J5-a-forged-block-worded-differently.cs.witness")]));

        Assert.Contains(
            "carries the assurance summary line '// HUMAN-REVIEWED:   47/47' below the generated header",
            reworded,
            StringComparison.Ordinal);
        Assert.Contains("and 2 such line(s) sit there", reworded, StringComparison.Ordinal);

        // ...and the two halves of the test that make it a classification: the vocabulary is
        // matched case-insensitively and by containment, and it is matched on COMMENT lines only,
        // so a string literal carrying a row label is not a forged summary.
        Assert.True(AssuranceGenerator.IsAssuranceSummaryLine("    // human-REVIEWED: 47/47"));
        Assert.True(AssuranceGenerator.IsAssuranceSummaryLine("// BROILER CODE ASSURANCE"));
        Assert.False(AssuranceGenerator.IsAssuranceSummaryLine("var label = \"Human-reviewed:\";"));

        // ...and the accepting direction and the clean direction of the same function: a file
        // whose only summary is its generated header is not reported, and no covered file is.
        Assert.Empty(AssuranceGenerator.ForgedAssuranceBlocks(AssuranceSources.Files));
        Assert.Empty(AssuranceGenerator.ForgedAssuranceBlocks(
            [Witness("J5-the-header-is-current.cs.witness")]));

        // Clause: the accepted direction. A file already in the shape the generator writes is
        // regenerated to itself, so the rule is a comparison and not a rewrite.
        Assert.Null(RegenerationDifference("J5-the-header-is-current.cs.witness"));

        // Clause: a count in the header that is not true of the file it sits on. This is the whole
        // point of generating the summary - the policy forbids a hand-maintained one.
        var miscounted = RegenerationDifference("J5-header-states-a-count-that-is-not-true.cs.witness");
        Assert.NotNull(miscounted);
        Assert.Contains("on disk: '// Relevant units:   9'", miscounted, StringComparison.Ordinal);
        Assert.Contains("generated: '// Relevant units:   2'", miscounted, StringComparison.Ordinal);

        // Clause: a header claiming a human review that the annotations do not support. The same
        // machinery as above, and named separately because it is the claim that matters.
        var claimed = RegenerationDifference("J5-header-claims-a-human-review.cs.witness");
        Assert.NotNull(claimed);
        Assert.Contains("on disk: '// Human-reviewed:   2/2'", claimed, StringComparison.Ordinal);
        Assert.Contains("generated: '// Human-reviewed:   0/2'", claimed, StringComparison.Ordinal);

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

        // The report says, in the manifest's own words, that a covered fingerprint is not a
        // reviewed unit. The manifest header and rule J7's register row carry the same sentence,
        // and the three are asserted separately because a reader reaches whichever one they open.
        Assert.Contains(
            AssuranceManifest.ChangeDetectionStatement, ComponentReport, StringComparison.Ordinal);

        // One probe unit set, four report witnesses, one clause each.
        var probe = WitnessUnits("J5-one-pending-unit.cs.witness");

        Assert.Equal(
            AssuranceReviewState.HumanPending,
            probe.Single(static unit => unit.Name.EndsWith(".Fold(int[])", StringComparison.Ordinal)).State);

        var overstated = Assert.Single(ReportViolations(
            WitnessText("J5-report-claims-a-human-review.md.witness"), probe));
        Assert.Contains(
            "states Human reviewed 1 of 1 where the annotations give 0 of 2",
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
            "states Unverified 0 where the annotations give 2",
            understated,
            StringComparison.Ordinal);

        var silent = Assert.Single(ReportViolations(
            WitnessText("J5-report-drops-the-statement-of-absence.md.witness"), probe));
        Assert.Contains(
            "no unit is human-reviewed and the report does not say so in its own words",
            silent,
            StringComparison.Ordinal);

        // Clause: the per-unit escape hatch is COUNTED in the report. The hatch is a sentence
        // nothing can check, so the one thing that can be done with it is make every use visible
        // in the component's own report rather than silent in one source file.
        var hatched = WitnessUnits("J1-an-explicit-exemption-covers-a-unit.cs.witness");

        Assert.Single(AssuranceScanner.DeclaredExemptions(hatched));

        var uncounted = Assert.Single(ReportViolations(
            WitnessText("J5-report-omits-a-declared-exemption.md.witness"), hatched));
        Assert.Contains(
            "states Per-unit exemptions 0 where the annotations give 1",
            uncounted,
            StringComparison.Ordinal);

        // Clause: and NAMED. Counting a use and naming it are two different things a report can
        // fail to do, so they are two clauses with a witness each; a report that stated the right
        // count and named nothing would leave a reader unable to find the unit.
        var unnamed = Assert.Single(ReportViolations(
            WitnessText("J5-report-does-not-name-a-declared-exemption.md.witness"), hatched));
        Assert.Contains(
            "does not name the per-unit exemption on Probe.Hatched.Shim(int)",
            unnamed,
            StringComparison.Ordinal);

        // ...and the real report, which states zero and names none because this component uses the
        // hatch nowhere.
        Assert.Empty(AssuranceScanner.DeclaredExemptions(ProductUnits));
        Assert.Contains("| Per-unit exemptions | 0 |", ComponentReport, StringComparison.Ordinal);
        Assert.Contains(
            "No unit in this component states a per-unit exemption.",
            ComponentReport,
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

        var declared = AssuranceScanner.DeclaredExemptions(units);

        Count(
            "Per-unit exemptions",
            @"\|\s*Per-unit exemptions\s*\|\s*(?<n>\d+)\s*\|",
            declared.Count);

        foreach (var unit in declared.Where(unit =>
                     !report.Contains($"`{unit.Name}`", StringComparison.Ordinal)))
        {
            violations.Add($"the report does not name the per-unit exemption on {unit.Name}");
        }

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
        AssertTheRegisterRowIsWhatTheRulesImplement("J6");

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

        var guarded = Assert.Single(AssuranceScanner.Scan(region).Where(static unit =>
            unit.Name.Contains(".Admit(", StringComparison.Ordinal)));

        Assert.Contains("Conditioned.Admit(uint)", guarded.Name, StringComparison.Ordinal);
        Assert.Contains("4242", AssuranceFingerprint.TokenStream(guarded.Declaration), StringComparison.Ordinal);

        // ...and the symbol list is the build's, not a wish: every symbol the parse defines is one
        // the SDK defines for net10.0 in one of the two configurations, and the one the attack used
        // is in it.
        Assert.Contains("NET10_0_OR_GREATER", AssuranceSources.PreprocessorSymbols, StringComparer.Ordinal);
    }

    // =====================================================================================
    // J7 - the manifest covers every unit
    // =====================================================================================

    /// <summary>
    /// J7. <c>assurance.manifest.json</c> covers every code unit in the product tree, exempt and
    /// relevant alike: every unit present, no extras, every fingerprint current.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this rule exists, and why it is not another patch.</b> Three adversarial rounds
    /// returned the same defeat in five places, and every one of them had one shape: the exemption
    /// predicate answered EXEMPT, an exempt unit carries no annotation, an unannotated unit carries
    /// no fingerprint, and a unit with no fingerprint has no record of any kind - so a semantic
    /// change to it was invisible to J1 through J6. Narrowing the predicate case by case failed
    /// three times, because each fix moved the defeat one case over: the constructor case was
    /// narrowed and the property case inherited it; <c>const</c> fields became units and property
    /// initializers were still unwatched.
    /// </para>
    /// <para>
    /// The repair separates two questions that had been fused. Whether a unit needs a human
    /// ANNOTATION is decided by the predicate; whether a unit is WATCHED for change is no longer a
    /// question - every unit is, and the manifest is the record.
    /// </para>
    /// <para>
    /// <b>And why the units were not enough either.</b> A fourth round attacked the ENUMERATION
    /// rather than the predicate. A unit exists only for a declaration kind the scanner names, and
    /// four shapes walked through that whitelist with the suite green and this file byte-unchanged:
    /// an enum member, a type declaration header carrying a primary constructor, an event field
    /// declaration, and an <c>[assembly: InternalsVisibleTo]</c> that opens every internal type in
    /// <c>Broiler.VM.Runtime</c>. The first three are units now. The fourth is a member of nothing
    /// and can never be one, which is the point: a whitelist is not completed by adding to it. So
    /// the manifest carries a <c>files</c> array - one fingerprint per covered file over the
    /// complete token stream of its compilation unit - and this rule holds it to the tree exactly as
    /// it holds the units. Nothing in a covered file can change without something moving here.
    /// </para>
    /// <para>
    /// <b>What the rule does not claim.</b> The manifest is a change-detection record and not a
    /// review. A covered fingerprint is not a reviewed unit, and the sentence saying so is asserted
    /// in all three places a reader might open: the manifest's own header, the component report and
    /// this rule's register row. Detection is also not assessment - a moved fingerprint is a red
    /// suite until someone regenerates, and nothing here judges what moved. That is EX-67. A moved
    /// FILE fingerprint says even less: it does not say what moved, only that the file is not what
    /// it was, which is EX-69.
    /// </para>
    /// </remarks>
    [Fact]
    public void J7_The_Manifest_Covers_Every_Unit_In_The_Product_Tree()
    {
        AssertTheRegisterRowIsWhatTheRulesImplement("J7");

        // The clean direction, against the file ON DISK. Gate mode only, for the reason J3 and
        // J5's currency half run in gate mode only: a write run exists to refresh this file, and
        // asserting that the pre-write text equalled the post-write text would fail every
        // generation for a reason no reader could act on. Every witness below runs in both modes.
        if (!AssuranceGenerator.WriteRequested)
        {
            Assert.Empty(AssuranceManifest.Violations(
                AssuranceSources.Files, AssuranceScanner.Units, ManifestOnDisk()));
        }

        // Non-vacuous, and the whole point of the rule: the manifest covers the units that carry
        // no annotation at all, which is most of them.
        var covered = AssuranceManifest.Entries(AssuranceScanner.Units);

        Assert.Equal(AssuranceScanner.Units.Count, covered.Count);
        Assert.NotEmpty(covered.Where(static entry => entry.Exempt));
        Assert.NotEmpty(covered.Where(static entry => !entry.Exempt));
        Assert.All(covered, static entry => Assert.True(
            AssuranceFingerprint.IsWellFormed(entry.Fingerprint), entry.Name));

        // ...and every covered file is recorded WHOLE, which is the half no widening of the unit
        // enumeration can supply.
        var files = AssuranceManifest.FileEntries(AssuranceSources.Files);

        Assert.Equal(AssuranceSources.Files.Count, files.Count);
        Assert.All(files, static entry => Assert.True(
            AssuranceFingerprint.IsWellFormed(entry.Fingerprint), entry.File));

        // ...and it says, in the file a reader opens, both of the things a reader is entitled to
        // take from it: that a covered fingerprint is not a review, and that nothing in a covered
        // file can change without something moving here.
        Assert.Contains(
            AssuranceManifest.ChangeDetectionStatement,
            AssuranceManifest.Render(AssuranceSources.Files, AssuranceScanner.Units),
            StringComparison.Ordinal);
        Assert.Contains(
            AssuranceManifest.CompletenessStatement,
            AssuranceManifest.Render(AssuranceSources.Files, AssuranceScanner.Units),
            StringComparison.Ordinal);

        // The witness tree: four units, two of which carry no annotation and would be recorded
        // nowhere at all without this file.
        var treeFile = AssuranceProbe.Source(
            WitnessText("J7-the-tree-the-manifest-covers.cs.witness"),
            "J7-the-tree-the-manifest-covers.cs.witness");

        IReadOnlyList<AssuranceSourceFile> treeFiles = [treeFile];

        var tree = AssuranceScanner.Scan(treeFile);

        Assert.Equal(4, tree.Count);
        Assert.Equal(2, tree.Count(static unit => unit.IsExempt));
        Assert.Equal(2, tree.Count(static unit => unit.Annotation is not null));

        // Clause: the accepting direction. A manifest that covers its tree exactly is not reported,
        // so a clean result is a comparison rather than a function that reports nothing.
        Assert.Empty(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-the-manifest-covers-its-tree.json.witness")));

        // Clause: a unit in the tree that the manifest does not cover. The plain field is the one
        // missing, which is the population this rule exists for.
        var missing = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-a-unit-is-missing-from-the-manifest.json.witness")));

        Assert.Contains(
            "Probe.Watched.position is a code unit in the product tree and " +
            "assurance.manifest.json does not cover it",
            missing,
            StringComparison.Ordinal);

        // Clause: an entry naming a unit that is not in the tree.
        var gone = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-the-manifest-names-a-unit-that-is-gone.json.witness")));

        Assert.Contains(
            "assurance.manifest.json carries an entry for Probe.Watched.Retired(ulong), " +
            "which is not a code unit in the product tree",
            gone,
            StringComparison.Ordinal);

        // Clause: a recorded fingerprint the code does not produce. Both halves of the message
        // matter - what was recorded, and what the code says now - and the unit is the exempt
        // auto-property whose initializer states a shipped value, because for that population the
        // manifest entry is the only record the change happened.
        var stale = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-a-recorded-fingerprint-is-not-current.json.witness")));

        Assert.Contains("Probe.Watched.Accepted is recorded in", stale, StringComparison.Ordinal);
        Assert.Contains("as AB12CD and the current code computes", stale, StringComparison.Ordinal);

        // ---- The file half: the three clauses again, over the `files` array ----

        // Clause: a covered file with no file entry. Every UNIT entry is correct, so the unit half
        // of the rule is silent - and nothing watches the whole of the file, which is where a
        // declaration kind the enumeration does not name would sit.
        var unwatched = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-a-covered-file-is-missing-from-the-manifest.json.witness")));

        Assert.Contains(
            "J7-the-tree-the-manifest-covers.cs.witness is a covered file and " +
            "assurance.manifest.json records no fingerprint for it, so nothing watches the whole of it",
            unwatched,
            StringComparison.Ordinal);

        // Clause: a recorded FILE fingerprint the file does not produce. Both halves of the message
        // matter, exactly as they do for a unit.
        var moved = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-a-recorded-file-fingerprint-is-not-current.json.witness")));

        Assert.Contains(
            "J7-the-tree-the-manifest-covers.cs.witness is recorded in assurance.manifest.json as " +
            "file fingerprint 0F0F0F and the current file computes",
            moved,
            StringComparison.Ordinal);

        // Clause: a file entry naming a file that is not covered.
        var retired = Assert.Single(AssuranceManifest.Violations(
            treeFiles, tree, WitnessText("J7-the-manifest-names-a-file-that-is-gone.json.witness")));

        Assert.Contains(
            "assurance.manifest.json carries a file entry for src/Broiler.VM.Runtime/VmRetired.cs, " +
            "which is not a covered file",
            retired,
            StringComparison.Ordinal);

        // Clause: THE SHAPE NO UNIT CAN HOLD. This witness is the tree witness with one line added,
        // `[assembly: InternalsVisibleTo("anything")]`, which opens every internal type in an
        // assembly. It is a member of nothing, so it is in no unit - and that is asserted rather
        // than assumed: the four units are the same four units with the same four fingerprints,
        // so the unit half of the manifest is byte-identical and reports nothing at all.
        //
        // It is read under the tree witness's own name, so the manifest under test is the one that
        // covers that tree exactly and the only disagreement it can report is the file fingerprint.
        var attributeFile = AssuranceProbe.Source(
            WitnessText("J7-an-assembly-attribute-is-in-no-unit.cs.witness"),
            "J7-the-tree-the-manifest-covers.cs.witness");

        var attributeUnits = AssuranceScanner.Scan(attributeFile);

        Assert.Equal(
            tree.Select(static unit => $"{unit.Name}@{unit.Fingerprint}"),
            attributeUnits.Select(static unit => $"{unit.Name}@{unit.Fingerprint}"));
        Assert.Empty(AssuranceManifest.Violations(
            treeFiles, attributeUnits, WitnessText("J7-the-manifest-covers-its-tree.json.witness")));

        var opened = Assert.Single(AssuranceManifest.Violations(
            [attributeFile], attributeUnits, WitnessText("J7-the-manifest-covers-its-tree.json.witness")));

        Assert.Contains(
            "J7-the-tree-the-manifest-covers.cs.witness is recorded in assurance.manifest.json as " +
            "file fingerprint 3C1463 and the current file computes",
            opened,
            StringComparison.Ordinal);
        Assert.NotEqual(
            AssuranceFingerprint.OfFile(treeFile.Tree),
            AssuranceFingerprint.OfFile(attributeFile.Tree));

        // Clause: a file fingerprint EXCLUDES comments, and this is asserted rather than assumed
        // because the whole generation depends on it. The generator writes a header into every
        // covered file and two annotation lines above every relevant unit; if either were in the
        // stream, the file's fingerprint would depend on the header, the header would depend on the
        // fingerprint, and no generation could ever be a fixed point.
        var commented = AssuranceProbe.Source(
            "// a comment above\n" +
            WitnessText("J7-the-tree-the-manifest-covers.cs.witness")
                .Replace("public sealed class Watched", "/* inline */ public sealed class Watched", StringComparison.Ordinal) +
            "\n// a comment below\n",
            "J7-the-tree-the-manifest-covers.cs.witness");

        Assert.NotEqual(treeFile.Text, commented.Text);
        Assert.Equal(
            AssuranceFingerprint.OfFile(treeFile.Tree),
            AssuranceFingerprint.OfFile(commented.Tree));
        Assert.Empty(AssuranceManifest.Violations(
            [commented], AssuranceScanner.Scan(commented),
            WitnessText("J7-the-manifest-covers-its-tree.json.witness")));
    }

    /// <summary>The manifest as it is on disk, or the empty string when it is not there.</summary>
    private static string ManifestOnDisk()
    {
        var path = Path.Combine(ComponentGraph.Root, AssuranceManifest.RelativePath);

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // =====================================================================================
    // J8 - every generated artefact matches its declared shape
    // =====================================================================================

    /// <summary>
    /// J8. Every generated artefact - the manifest's header, every file header and
    /// <c>CODE-ASSURANCE.md</c> - is line for line what <see cref="AssuranceArtefactShape"/>
    /// declares: the fixed text hand-copied there, and every derived value derived again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What this answers that J5 cannot.</b> J5 compares the artefacts on disk against what the
    /// generator would write. It therefore cannot report a sentence the GENERATOR invents, because
    /// that sentence is on both sides of its comparison. Two lines appended to
    /// <see cref="AssuranceManifest.Header"/> made the manifest assert that every unit was
    /// <c>reviewState=VERIFIED</c>, <c>humanReviewed=true</c> with a named reviewer and that the
    /// component was eligible for release - with both modes of the gate green and every human line
    /// in the tree still reading <c>PENDING</c>.
    /// </para>
    /// <para>
    /// The mechanism is the one the register rows now use: a second copy, kept by hand, compared
    /// for equality. An edit to a generated sentence fails here until someone makes it in both
    /// places having read both. Where content is DERIVED it is derived again rather than copied,
    /// and deliberately by a different expression - the IP and Security rows walk their vocabulary
    /// backwards where the generator ranks and maximises, which is what pins
    /// <c>AssuranceSummary.Worst</c>: those two rows are the line a developer reads and nothing
    /// compared them against the annotations they claim to summarize.
    /// </para>
    /// <para>
    /// <b>The limit, stated rather than implied.</b> A change made in BOTH places passes. That is
    /// the accepted cost of a hand-maintained copy and it is EX-70; rule J9 is the independent hold
    /// on the same text, and it cannot be lifted by editing a shape.
    /// </para>
    /// </remarks>
    [Fact]
    public void J8_Every_Generated_Artefact_Matches_Its_Declared_Shape()
    {
        AssertTheRegisterRowIsWhatTheRulesImplement("J8");

        // ---- The manifest header ----

        Assert.Empty(AssuranceArtefactShape.ManifestHeaderViolations(AssuranceManifest.Header));

        // The header is prose and was the only thing held; everything Render emits below it was
        // compared against nothing, so a property added there reached the manifest unreported.
        Assert.Empty(AssuranceArtefactShape.ManifestShapeViolations(AssuranceManifest.Render(AssuranceSources.Files, AssuranceScanner.Units)));

        var forgedShape = AssuranceArtefactShape.ManifestShapeViolations(
            AssuranceManifest.Render(AssuranceSources.Files, AssuranceScanner.Units).Replace(
                "\"fingerprint\": \"",
                "\"reviewState\": \"VERIFIED\", \"fingerprint\": \"",
                StringComparison.Ordinal));

        Assert.Contains(
            forgedShape,
            violation => violation.Contains("reviewState", StringComparison.Ordinal));
        Assert.NotEmpty(AssuranceArtefactShape.ManifestHeader);

        // Clause: the two lines the attack appended. They are read from a witness and appended to
        // the DECLARED header, so the input is the forgery and not a stale copy of the shape.
        var appended = new AssuranceTextLines(WitnessText(
            "J8-the-lines-the-attack-appended-to-the-manifest-header.txt.witness"))
            .Where(static line => line.Length > 0)
            .ToArray();

        Assert.Equal(2, appended.Length);

        var forgedHeader = AssuranceArtefactShape.ManifestHeaderViolations(
            [.. AssuranceArtefactShape.ManifestHeader, .. appended]);

        Assert.Equal(2, forgedHeader.Count);
        Assert.Single(forgedHeader.Where(claim => claim.Contains(
            "reviewState=VERIFIED; humanReviewed=true; reviewer=WITNESS-ONLY for all 1169 units.",
            StringComparison.Ordinal)));
        Assert.Single(forgedHeader.Where(static claim => claim.Contains(
            "The component is eligible for release.", StringComparison.Ordinal)));
        Assert.All(forgedHeader, static claim => Assert.Contains(
            "assurance.manifest.json", claim, StringComparison.Ordinal));

        // ---- Every generated file header ----

        Assert.Empty(GeneratedSources.SelectMany(static file => AssuranceArtefactShape.FileHeaderViolations(
            file.RelativePath,
            AssuranceGenerator.GeneratedHeaderLines(file.Text),
            AssuranceScanner.Scan(file))));

        // Non-vacuous: 45 headers are read, and each is fifteen lines rather than nothing.
        Assert.NotEmpty(GeneratedSources);
        Assert.All(GeneratedSources, static file => Assert.Equal(
            15, AssuranceGenerator.GeneratedHeaderLines(file.Text).Count));

        // Clause: a header carrying a row the shape does not declare. One inserted line shifts
        // every line under it, so the violation set names the invention and then the shift; the
        // assertion picks the invention out by its text.
        var invented = AssuranceArtefactShape.FileHeaderViolations(
            "J8-a-file-header-with-an-invented-row.cs.witness",
            AssuranceGenerator.GeneratedHeaderLines(
                WitnessText("J8-a-file-header-with-an-invented-row.cs.witness")),
            WitnessUnits("J8-a-file-header-with-an-invented-row.cs.witness"));

        Assert.NotEmpty(invented);
        Assert.Single(invented.Where(static claim => claim.Contains(
            "generated: // Release gate:     open", StringComparison.Ordinal)));
        Assert.All(invented, static claim => Assert.Contains(
            "J8-a-file-header-with-an-invented-row.cs.witness", claim, StringComparison.Ordinal));

        // Clause: the IP and Security rows are the WEAKEST claim the annotations make, and the
        // vocabularies are ordered weakest-claim-last - Unknown outranks High for IP, because a
        // provenance nobody established is not a better answer than one that was. The witness
        // carries three annotations chosen so that a mistake in either direction changes the answer.
        var weakest = AssuranceArtefactShape.ExpectedFileHeader(
            WitnessUnits("J8-the-worst-assessed-risk-reaches-the-header.cs.witness"));

        Assert.Contains("// IP risk:          Unknown", weakest, StringComparer.Ordinal);
        Assert.Contains("// Security risk:    High", weakest, StringComparer.Ordinal);
        Assert.Contains("// Resource impact:  4/10 max", weakest, StringComparer.Ordinal);
        Assert.Contains("// Human-reviewed:   0/3", weakest, StringComparer.Ordinal);

        // ...and the generator agrees with the shape on that file, which is the derivation being
        // CHECKED rather than restated: two expressions of one rule, written differently.
        Assert.Equal(
            weakest,
            AssuranceGenerator.Header(
                WitnessUnits("J8-the-worst-assessed-risk-reaches-the-header.cs.witness")).ToArray());

        // ...and the rejecting direction: a header claiming a better risk than the annotations
        // support is named with both texts.
        var downgraded = AssuranceArtefactShape.FileHeaderViolations(
            "J8-the-worst-assessed-risk-reaches-the-header.cs.witness",
            [.. weakest.Select(static line => line.Replace(
                "// Security risk:    High", "// Security risk:    Low", StringComparison.Ordinal))],
            WitnessUnits("J8-the-worst-assessed-risk-reaches-the-header.cs.witness"));

        var claimedRisk = Assert.Single(downgraded);

        Assert.Contains("generated: // Security risk:    Low", claimedRisk, StringComparison.Ordinal);
        Assert.Contains("declared:  // Security risk:    High", claimedRisk, StringComparison.Ordinal);

        // ---- CODE-ASSURANCE.md ----

        Assert.Empty(AssuranceArtefactShape.ReportViolations(
            ComponentReport, AssuranceSources.Files.Count, ProductUnits));

        // Clause: a sentence the generator invents. The witness here is the REAL report with one
        // line put into it, rather than a copy on disk: a stored copy of a generated artefact would
        // be a second copy of the thing under test, would go stale at every regeneration, and would
        // be repaired by regenerating it - which is the tautology this rule exists to remove.
        var invention = "The component is verified and eligible for release.";

        var doctored = AssuranceArtefactShape.ReportViolations(
            ComponentReport.Replace(
                "## Verification\n", $"## Verification\n\n{invention}\n", StringComparison.Ordinal),
            AssuranceSources.Files.Count,
            ProductUnits);

        Assert.NotEmpty(doctored);
        Assert.Single(doctored.Where(claim =>
            claim.Contains($"generated: {invention}", StringComparison.Ordinal)));
        Assert.All(doctored, static claim => Assert.Contains(
            "CODE-ASSURANCE.md", claim, StringComparison.Ordinal));

        // Clause: the high-security list is DERIVED from the annotations, not written. It is the
        // section a reader turns to first, and a report that dropped an entry would read as a
        // component with less to look at than it has.
        var high = ProductUnits
            .Where(static unit => unit.Annotation?.Field("Security") is "High" or "Critical")
            .ToArray();

        Assert.NotEmpty(high);
        Assert.All(high, static unit => Assert.Contains(HighSecurityEntry(unit), ComponentReport, StringComparison.Ordinal));

        // ...and each entry names its FILE as well as its unit, because a name is not unique: a
        // partial type is two declarations, and `VmRuntime` stood in this list twice with nothing
        // to tell a reader which declaration they were looking at.
        Assert.Equal(
            high.Length,
            high.Select(HighSecurityEntry).Distinct(StringComparer.Ordinal).Count());

        var dropped = AssuranceArtefactShape.ReportViolations(
            ComponentReport.Replace(HighSecurityEntry(high[0]) + "\n", string.Empty, StringComparison.Ordinal),
            AssuranceSources.Files.Count,
            ProductUnits);

        Assert.NotEmpty(dropped);
        Assert.Single(dropped.Where(claim => claim.Contains(
            $"declared:  {HighSecurityEntry(high[0])}", StringComparison.Ordinal)));
    }

    // =====================================================================================
    // J9 - no generated artefact claims a review the annotations do not hold
    // =====================================================================================

    /// <summary>
    /// J9. No generated text says <c>verified</c>, <c>approved</c>, <c>reviewed by</c>, a reviewer
    /// identifier or <c>eligible for release</c> unless the annotations support it: by stating the
    /// count the annotations give for that term, or by standing behind a negation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the policy's hardest rule - CI may never turn <c>PENDING</c> into a name - applied to
    /// the ARTEFACTS rather than to the source lines. J4 holds every human line in every artefact to
    /// <c>PENDING</c> and says nothing about the prose around it, and prose is what a reader trusts.
    /// </para>
    /// <para>
    /// It is deliberately independent of J8. J8's hand-maintained shape can be defeated by editing
    /// the shape as well as the generator; this cannot be defeated by editing anything except the
    /// term list, and the term list is not what a forger reaches for. The corpus is the GENERATED
    /// text alone - the whole of the report and the manifest, and the generated header block of a
    /// source file - because <c>VmVerifiedArtifact</c> is a public type of this component and its
    /// name appears in 112 lines below the generated headers.
    /// </para>
    /// <para>
    /// <b>The limit.</b> The terms are a list, and a claim worded outside it is not seen. That is
    /// EX-71, and J8 is what covers it: a sentence of any wording is a line the declared shape does
    /// not carry.
    /// </para>
    /// </remarks>
    [Fact]
    public void J9_No_Generated_Artefact_Claims_A_Review_The_Annotations_Do_Not_Support()
    {
        AssertTheRegisterRowIsWhatTheRulesImplement("J9");

        var generated = AssuranceReviewClaims.GeneratedText(AssuranceGenerator.Current.Artefacts);

        // The clean direction, over every generated artefact.
        Assert.Empty(AssuranceReviewClaims.Violations(generated, ProductUnits));

        // Non-vacuous, and this is the assertion that keeps the rule from being a quantifier over
        // nothing: the generated corpus DOES carry the vocabulary, in every artefact kind, and
        // every occurrence of it is one the annotations support.
        Assert.Contains(
            generated,
            text => string.Equals(text.Where, AssuranceGenerator.ReportPath, StringComparison.Ordinal) &&
                text.Lines.Any(static line => line.Contains("| VERIFIED | 0 |", StringComparison.Ordinal)));
        Assert.Contains(
            generated,
            text => string.Equals(text.Where, AssuranceManifest.RelativePath, StringComparison.Ordinal) &&
                text.Lines.Any(static line => line.Contains("not an approval", StringComparison.Ordinal)));
        Assert.All(
            generated.Where(static text => text.Where.EndsWith(".cs", StringComparison.Ordinal)),
            static text => Assert.Contains(
                text.Lines,
                static line => line.StartsWith("// Human-reviewed:", StringComparison.Ordinal)));

        // Clause: the attack, verbatim. Two claims that no annotation supports - one stating a
        // count that is not the count the annotations give, one stating nothing countable at all.
        var claimed = AssuranceReviewClaims.Violations(
            [WitnessGeneratedText("J9-an-artefact-that-claims-a-review.json.witness")],
            ProductUnits);

        Assert.Equal(2, claimed.Count);
        Assert.Single(claimed.Where(static claim => claim.Contains(
            "says '\"reviewState=VERIFIED; humanReviewed=true; reviewer=WITNESS-ONLY for all 1169 units.\",'",
            StringComparison.Ordinal)));
        Assert.Single(claimed.Where(static claim => claim.Contains(
            "says '\"The component is eligible for release.\"'", StringComparison.Ordinal)));
        Assert.All(claimed, static claim => Assert.Contains(
            "and the annotations hold no such state", claim, StringComparison.Ordinal));
        Assert.Single(claimed.Where(static claim => claim.Contains(
            "the term 'verified' is stated with neither the count the annotations give (0) nor a " +
            "negation before it",
            StringComparison.Ordinal)));
        Assert.Single(claimed.Where(static claim => claim.Contains(
            "the term 'eligible for release' is stated with neither the count the annotations give " +
            "(none is defined for it) nor a negation before it",
            StringComparison.Ordinal)));

        // Clause: the accepting direction, through the same function, over lines that carry the
        // very same vocabulary - so the rule is a comparison and not a prohibition on words.
        Assert.Empty(AssuranceReviewClaims.Violations(
            [WitnessGeneratedText("J9-an-artefact-that-states-the-absence.json.witness")],
            ProductUnits));

        // Clause: the count is DERIVED and not frozen at zero. Handed a unit set in which one unit
        // is VERIFIED, the same line that was honest above becomes a violation, and the line that
        // states the true count is accepted.
        var reviewed = WitnessUnits("J3-a-review-of-the-current-version-is-current.cs.witness");

        Assert.Equal(1, reviewed.Count(static unit => unit.State == AssuranceReviewState.Verified));

        var understated = Assert.Single(AssuranceReviewClaims.Violations(
            [new AssuranceGeneratedText("probe", ["| VERIFIED | 0 |"])], reviewed));

        Assert.Contains("says '| VERIFIED | 0 |'", understated, StringComparison.Ordinal);
        Assert.Contains(
            "the count the annotations give (1)", understated, StringComparison.Ordinal);
        Assert.Empty(AssuranceReviewClaims.Violations(
            [new AssuranceGeneratedText("probe", ["| VERIFIED | 1 |"])], reviewed));

        // Clause: the corpus of a source artefact is its generated header and not its code. Without
        // that this rule would report the component for declaring a type called VmVerifiedArtifact.
        Assert.NotEmpty(GeneratedSources.Where(static file =>
            file.Text.Contains("verified", StringComparison.OrdinalIgnoreCase)));
        Assert.DoesNotContain(
            generated
                .Where(static text => text.Where.EndsWith(".cs", StringComparison.Ordinal))
                .SelectMany(static text => text.Lines),
            static line => line.Contains("VmVerifiedArtifact", StringComparison.Ordinal));
    }

    /// <summary>One line of the report's high-security list, as the generator writes it.</summary>
    private static string HighSecurityEntry(AssuranceUnit unit) =>
        $"- `{unit.Name}` in `{unit.File.RelativePath}` - " +
        $"Security={unit.Annotation!.Field("Security")}, " +
        $"state {AssuranceStateMachine.Name(unit.State)}";

    /// <summary>A witness file read as one piece of generated text.</summary>
    private static AssuranceGeneratedText WitnessGeneratedText(string fileName) =>
        new(fileName, new AssuranceTextLines(WitnessText(fileName)));

    // =====================================================================================
    // The register rows are held to the limits their rules depend on
    // =====================================================================================

    /// <summary>
    /// Holds one group J register row to the exact text <see cref="AssuranceRegisterRows"/>
    /// records for it, in all three of its prose fields.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nothing else in the suite reads a register row's prose: <c>RuleRegisterTests</c> asserts
    /// only that the fields are non-empty. The version of this helper before this one asserted
    /// that a row CONTAINED a handful of required phrases, which holds a row to stating its limits
    /// and does not hold it to stating nothing else. A row could therefore CLAIM a capability the
    /// tests do not implement: a sentence saying CI compares every human line against the parent
    /// commit was appended to a row and the suite stayed green, and no such mechanism exists in
    /// this component - there is no CI lane at all, which is EX-60. An over-claim in a row is the
    /// same defect as a rule weaker than its own statement, reached by editing the row instead of
    /// the rule.
    /// </para>
    /// <para>
    /// Equality and not containment, therefore, and over all three fields rather than over the one
    /// that happens to carry the limits: an appended sentence fails wherever it is appended. Group
    /// H keeps the substring helper it has, which is EX-58; the two are deliberately separate so
    /// that neither group's rows depend on the other group's file.
    /// </para>
    /// </remarks>
    private static void AssertTheRegisterRowIsWhatTheRulesImplement(string id)
    {
        var expected = Assert.Contains(id, AssuranceRegisterRows.Expected);
        var row = RegisterRow(id);

        Assert.Equal(expected.Statement, row.Statement);
        Assert.Equal(expected.Evidence, row.Evidence);
        Assert.Equal(expected.NonVacuousWhen, row.NonVacuousWhen);
    }

    private static AssuranceRegisterRows.Row RegisterRow(string id)
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

            return new AssuranceRegisterRows.Row(
                Field(rule, "statement"),
                Field(rule, "evidence"),
                Field(rule, "nonVacuousWhen"));
        }

        throw new InvalidOperationException($"The register has no row {id}.");

        static string Field(JsonElement rule, string name) =>
            rule.TryGetProperty(name, out var value) ? value.GetString() ?? string.Empty : string.Empty;
    }
}
