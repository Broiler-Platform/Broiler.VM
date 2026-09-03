using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>Why this profile refused a source, in the language's terms rather than this build's.</summary>
/// <remarks>
/// <para>
/// <b>This enumeration exists because a refusal is not evidence of conformance.</b> Roadmap
/// section 14 opens by saying an engine that grades itself is not evidence, and names the failure
/// it is guarding against: an engine that refused a test "for the wrong reason". A suite whose
/// negative tests declare <c>SyntaxError</c> and an engine that refuses nearly everything agree on
/// the observable outcome for reasons that have nothing to do with each other, and a harness that
/// scored the outcome would report a large, entirely false total.
/// </para>
/// <para>
/// <b>The four classes are four because they have four different futures</b>, not because a
/// two-valued "can this score" flag was too coarse to read. <see cref="OutsideManifest"/>
/// disappears as the manifest grows and is a scope input. <see cref="Divergence"/> is permanent
/// until a decision record retires it, and belongs in the published exclusions.
/// <see cref="ImplementationLimit"/> is a refusal the specification explicitly permits, so it is
/// neither a defect nor a conformance answer. Only <see cref="EarlyError"/> is this profile
/// answering the question the suite asked.
/// </para>
/// </remarks>
internal enum RefusalClass
{
    /// <summary>The source is not JavaScript, and every conforming engine must refuse it.</summary>
    /// <remarks>This is the only class that may answer a suite's negative expectation.</remarks>
    EarlyError,

    /// <summary>
    /// The source <b>is</b> JavaScript; this profile's declared feature manifest does not admit
    /// the construct.
    /// </summary>
    /// <remarks>
    /// <b>This is the dangerous one, and it is dangerous at scale.</b>
    /// <c>broiler.javascript.slice</c> admits no function, no object, no string value and no
    /// property access, so nearly every file in a real suite is refused with
    /// <see cref="SliceSourceDiagnosticCode.ConstructOutsideManifest"/> - including nearly every
    /// negative test, whose declared expectation is that a refusal happens. Matching those two
    /// facts would turn a manifest that admits almost nothing into a near-perfect conformance
    /// score.
    /// </remarks>
    OutsideManifest,

    /// <summary>
    /// This profile refuses where the language answers differently, or answers later.
    /// </summary>
    /// <remarks>
    /// A recorded divergence, not a defect and not a conformance answer. Both members are
    /// documented as divergences on their own declarations, and both would let a refusal match a
    /// negative expectation the language does not agree with.
    /// </remarks>
    Divergence,

    /// <summary>A ceiling this build declares, which the specification permits an engine to have.</summary>
    /// <remarks>
    /// A conforming engine may accept these sources and may refuse them, so neither answer is
    /// evidence about the language.
    /// </remarks>
    ImplementationLimit,
}

/// <summary>
/// Maps this profile's source-refusal codes onto what the language would say, so that an ingested
/// suite's negative expectation is matched on what it is.
/// </summary>
/// <remarks>
/// <para>
/// <b>Roadmap section 14 asks for exactly this and names the unit.</b> A negative test's uncaught
/// error is "reported by its JavaScript type name so a parse-phase syntax error is matched on what
/// it is". This profile's front end does not throw a JavaScript error - it refuses source and
/// returns a code from the registry's <c>embedder-seam</c> half - so something has to say which of
/// those codes stand for which JavaScript error, and that something has to be readable and wrong
/// in one place rather than guessed at each call site.
/// </para>
/// <para>
/// <b>The map is total over the enumeration and a test asserts that.</b> A code added to
/// <see cref="SliceSourceDiagnosticCode"/> without a class here is a code whose refusals would
/// take whatever the default arm says, and the default arm of a switch over a conformance oracle's
/// scoring rule is the last place a new refusal should land silently.
/// </para>
/// </remarks>
internal static class LanguageErrors
{
    /// <summary>The JavaScript error type an early error is reported as.</summary>
    /// <remarks>
    /// Every early error this front end can raise is a <c>SyntaxError</c>, because every stage that
    /// can raise one - the tokenizer, the parser and the static-semantics validator - refuses over
    /// the grammar and the early-error rules, which is what a <c>SyntaxError</c> is. This is a
    /// constant rather than a per-code string so that a second error type cannot be introduced by
    /// a typo; a stage that ever needs one will need a row here and a case to go with it.
    /// </remarks>
    internal const string SyntaxError = "SyntaxError";

    /// <summary>What the language says about a refusal this profile made.</summary>
    internal static RefusalClass Classify(SliceSourceDiagnosticCode code) => code switch
    {
        // ---- The tokenizer: text that begins no token, or a token that never ends. ------------
        SliceSourceDiagnosticCode.UnexpectedCharacter => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.UnterminatedComment => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.MalformedNumericLiteral => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.UnterminatedStringLiteral => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.UnknownEscapeSequence => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.UnterminatedRegularExpression => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.UnterminatedTemplateLiteral => RefusalClass.EarlyError,

        // ---- The parser: a token that continues no production. --------------------------------
        SliceSourceDiagnosticCode.UnexpectedToken => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.ExpectedToken => RefusalClass.EarlyError,

        // ---- Static semantics: the early-error rules the specification writes down. -----------
        SliceSourceDiagnosticCode.DuplicateLexicalDeclaration => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.VarAndLexicalCollision => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.ConstWithoutInitialiser => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.InvalidAssignmentTarget => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.IllegalBreak => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.IllegalContinue => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.ReservedWordAsBinding => RefusalClass.EarlyError,
        SliceSourceDiagnosticCode.LegacyOctalInStrictCode => RefusalClass.EarlyError,

        // ---- The manifest, which is not the language. -----------------------------------------
        SliceSourceDiagnosticCode.ConstructOutsideManifest => RefusalClass.OutsideManifest,

        // ---- Two refusals the language makes at a different time, or not at all. --------------
        //
        // ASSIGNMENT TO A CONSTANT IS A RUNTIME TypeError IN THE LANGUAGE, not an early error:
        // `PutValue` reaches `SetMutableBinding` on a declarative environment record, and that is
        // where the throw is. This profile answers statically, which is a divergence in both the
        // time and the kind of the answer, and a negative test declaring a SyntaxError must not be
        // satisfied by it.
        SliceSourceDiagnosticCode.AssignmentToConstant => RefusalClass.Divergence,

        // A FREE NAME IS A RUNTIME ReferenceError IN THE LANGUAGE, because it might be a property
        // of the global object. This manifest declares no global object, so the answer can never
        // change and is given early - which the code's own declaration records as a deliberate
        // divergence, and names the manifest growth that would end it.
        SliceSourceDiagnosticCode.UnresolvableIdentifier => RefusalClass.Divergence,

        // ---- Ceilings the specification permits an implementation to have. --------------------
        SliceSourceDiagnosticCode.NestingTooDeep => RefusalClass.ImplementationLimit,
        SliceSourceDiagnosticCode.TooManyLocals => RefusalClass.ImplementationLimit,
        SliceSourceDiagnosticCode.TooManyConstants => RefusalClass.ImplementationLimit,
        SliceSourceDiagnosticCode.OperandStackTooDeep => RefusalClass.ImplementationLimit,

        // A CODE WITH NO ROW ABOVE IS NOT GIVEN A CLASS HERE. The unclassified value is returned
        // so the caller reports it by name and declines to score, and the harness's own checks
        // fail on any enumeration member that reaches this arm - see `Classified`.
        _ => Unclassified,
    };

    /// <summary>The value <see cref="Classify"/> returns for a code that has no row.</summary>
    /// <remarks>
    /// It is deliberately <see cref="RefusalClass.Divergence"/> rather than
    /// <see cref="RefusalClass.EarlyError"/>: an unclassified code cannot score, so a map that
    /// falls behind the enumeration under-reports rather than inventing passes. The checks below
    /// still fail, because a silent under-report is a defect too - it is only a safer one.
    /// </remarks>
    private const RefusalClass Unclassified = RefusalClass.Divergence;

    /// <summary>Every code the enumeration declares.</summary>
    internal static IReadOnlyList<SliceSourceDiagnosticCode> All { get; } =
        Enum.GetValues<SliceSourceDiagnosticCode>();

    /// <summary>
    /// The codes that have an explicit row above, which is what makes the map's totality checkable.
    /// </summary>
    /// <remarks>
    /// <see cref="Classify"/> cannot report this itself - its default arm returns a real class -
    /// so the set is written a second time and the check asserts the two agree in size. Two
    /// spellings of one fact can disagree, which is the point: they disagree loudly here and the
    /// suite goes red, rather than quietly at a scoring call site.
    /// </remarks>
    internal static IReadOnlySet<SliceSourceDiagnosticCode> Classified { get; } =
        new HashSet<SliceSourceDiagnosticCode>
        {
            SliceSourceDiagnosticCode.UnexpectedCharacter,
            SliceSourceDiagnosticCode.UnterminatedComment,
            SliceSourceDiagnosticCode.MalformedNumericLiteral,
            SliceSourceDiagnosticCode.UnterminatedStringLiteral,
            SliceSourceDiagnosticCode.UnknownEscapeSequence,
            SliceSourceDiagnosticCode.UnterminatedRegularExpression,
            SliceSourceDiagnosticCode.UnterminatedTemplateLiteral,
            SliceSourceDiagnosticCode.UnexpectedToken,
            SliceSourceDiagnosticCode.ExpectedToken,
            SliceSourceDiagnosticCode.NestingTooDeep,
            SliceSourceDiagnosticCode.ConstructOutsideManifest,
            SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
            SliceSourceDiagnosticCode.VarAndLexicalCollision,
            SliceSourceDiagnosticCode.ConstWithoutInitialiser,
            SliceSourceDiagnosticCode.AssignmentToConstant,
            SliceSourceDiagnosticCode.InvalidAssignmentTarget,
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            SliceSourceDiagnosticCode.IllegalBreak,
            SliceSourceDiagnosticCode.IllegalContinue,
            SliceSourceDiagnosticCode.ReservedWordAsBinding,
            SliceSourceDiagnosticCode.LegacyOctalInStrictCode,
            SliceSourceDiagnosticCode.TooManyLocals,
            SliceSourceDiagnosticCode.TooManyConstants,
            SliceSourceDiagnosticCode.OperandStackTooDeep,
        };

    /// <summary>
    /// Whether a refusal with this code may answer a suite's declared negative expectation.
    /// </summary>
    internal static bool MayScore(SliceSourceDiagnosticCode code) =>
        Classified.Contains(code) && Classify(code) == RefusalClass.EarlyError;

    /// <summary>Why a refusal could not answer the expectation, in one line, for the report.</summary>
    /// <remarks>
    /// The reason names the code, because a reader triaging a large skipped total needs to know
    /// which refusal is dominating it - and for a real suite the answer is expected to be
    /// <see cref="SliceSourceDiagnosticCode.ConstructOutsideManifest"/> by a wide margin.
    /// </remarks>
    internal static string WhyItCannotScore(SliceSourceDiagnosticCode code)
    {
        if (!Classified.Contains(code))
        {
            return $"{code} has no declared language class, so no refusal it carries is scored";
        }

        return Classify(code) switch
        {
            RefusalClass.OutsideManifest =>
                $"{code}: the source is JavaScript and this manifest declines the construct, " +
                "so the refusal answers a different question from the one the test asked",
            RefusalClass.Divergence =>
                $"{code}: this profile answers where the language answers differently or later, " +
                "which is a recorded divergence and not a conformance answer",
            RefusalClass.ImplementationLimit =>
                $"{code}: a ceiling the specification permits, so neither refusing nor accepting " +
                "is evidence about the language",
            _ => string.Empty,
        };
    }
}
