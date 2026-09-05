// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           24
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The front end's own stable diagnostic codes: the <b>embedder-seam</b> half of the published
/// registry.
/// </summary>
/// <remarks>
/// <para>
/// <b>These codes never cross a core result envelope, and that is the whole reason they are a
/// separate vocabulary.</b> Source carrying an early error never becomes an artifact, so its
/// rejection occupies no core profile diagnostic-code field, carries no byte offset within an
/// artifact, and reaches no <c>VmReason</c>. The published registry's <c>half</c> column has
/// carried <c>embedder-seam</c> as a declared-and-empty value since JS-3a, with the note that the
/// front end which would mint one is JS-3b's. This is that front end and these are those codes.
/// </para>
/// <para>
/// The numbers start at 2000 so that no code here can be confused with a
/// <c>JavaScriptDiagnosticCode</c> by a reader holding one number and no context, and they are
/// grouped by the stage that refuses in the same way. The two vocabularies are declared in two
/// assemblies that do not reference each other, so the disjointness is a convention the registry
/// holds rather than a fact the compiler checks - which is exactly why the registry publishes
/// both halves in one file and a rule reads it.
/// </para>
/// <para>
/// <b>A code is never reused for a different meaning.</b> A rejection whose meaning changes takes
/// a new number and the old one is retired, because a retained case that recorded a code has
/// dated it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4E385D
// Broiler-Human:        PENDING
public enum SliceSourceDiagnosticCode
{
    // ---- 2000: tokenizing ------------------------------------------------------------------

    /// <summary>A character that begins no token this grammar defines.</summary>
    UnexpectedCharacter = 2001,

    /// <summary>A block comment that reaches the end of the source without closing.</summary>
    UnterminatedComment = 2002,

    /// <summary>A numeric literal whose text is not a numeric literal.</summary>
    MalformedNumericLiteral = 2003,

    /// <summary>A string literal that reaches the end of the line or the source without closing.</summary>
    UnterminatedStringLiteral = 2004,

    /// <summary>An escape sequence a string literal does not define.</summary>
    UnknownEscapeSequence = 2005,

    /// <summary>A regular-expression literal that reaches the end of the line without closing.</summary>
    UnterminatedRegularExpression = 2006,

    /// <summary>A template literal that reaches the end of the source without closing.</summary>
    UnterminatedTemplateLiteral = 2007,

    // ---- 2100: parsing ---------------------------------------------------------------------

    /// <summary>A token that continues no production the parser was in.</summary>
    UnexpectedToken = 2101,

    /// <summary>A token the grammar required at this position is missing.</summary>
    ExpectedToken = 2102,

    /// <summary>The source nests deeper than the parse options allow.</summary>
    /// <remarks>
    /// This is a refusal and never a process termination, which is the clause roadmap section 9
    /// makes blocking. See <see cref="SliceParseOptions.MaximumNestingDepth"/>.
    /// </remarks>
    NestingTooDeep = 2103,

    /// <summary>A construct the declared feature manifest does not admit.</summary>
    /// <remarks>
    /// <b>Refused here rather than at first execution</b>, which is the exit-gate clause that
    /// distinguishes a profile with a manifest from a profile with a subset it happens to
    /// implement. The message names the construct.
    /// </remarks>
    ConstructOutsideManifest = 2104,

    // ---- 2200: static semantics ------------------------------------------------------------

    /// <summary>One scope declares a lexical name twice.</summary>
    DuplicateLexicalDeclaration = 2201,

    /// <summary>
    /// <c>VarDeclaredNames</c> and <c>LexicallyDeclaredNames</c> intersect at one scope.
    /// </summary>
    VarAndLexicalCollision = 2202,

    /// <summary>A <c>const</c> declarator with no initialiser.</summary>
    ConstWithoutInitialiser = 2203,

    /// <summary>An assignment whose target is an immutable binding.</summary>
    AssignmentToConstant = 2204,

    /// <summary>An assignment whose left side is not a valid assignment target.</summary>
    InvalidAssignmentTarget = 2205,

    /// <summary>
    /// An identifier reference that resolves to no binding, in a manifest with no global object.
    /// </summary>
    /// <remarks>
    /// <b>This is a deliberate divergence and is recorded as one.</b> In the language a free name
    /// is a runtime <c>ReferenceError</c>, because it might be a property of the global object.
    /// <c>broiler.javascript.slice</c> declares no global object and no property access at all, so
    /// a free name can never resolve at run time either and deferring the answer would only move
    /// the same refusal later. It becomes a language-conformance exclusion the moment the manifest
    /// grows a global, and the decision record says so.
    /// </remarks>
    UnresolvableIdentifier = 2206,

    /// <summary>A <c>break</c> with no enclosing breakable statement.</summary>
    IllegalBreak = 2207,

    /// <summary>A <c>continue</c> with no enclosing iteration statement.</summary>
    IllegalContinue = 2208,

    /// <summary>A binding whose name is reserved in the code's strictness.</summary>
    ReservedWordAsBinding = 2209,

    /// <summary>A legacy octal literal in strict code.</summary>
    /// <remarks>
    /// The tokenizer recognises the shape and records it on the token; this stage rules on it.
    /// That split is what lets the rule live in the validator without a second scan of the source
    /// text - see <c>SliceStaticSemantics</c>.
    /// </remarks>
    LegacyOctalInStrictCode = 2210,

    // 2211 was declared and never emitted. A duplicate bound name is the two rows above - one
    // scope declaring a lexical name twice, or a `var` and a lexical form colliding - and a third
    // code for the same fact would have been a number no rejection could reach. It is retired
    // rather than reused, because a code is never given a second meaning.

    // ---- 2300: lowering --------------------------------------------------------------------

    /// <summary>The program needs more local slots than the format's frame admits.</summary>
    TooManyLocals = 2301,

    /// <summary>The program needs more constant-pool entries than the format admits.</summary>
    TooManyConstants = 2302,

    /// <summary>The program needs a deeper operand stack than the format admits.</summary>
    OperandStackTooDeep = 2303,

    // ---- 2400: the module goal's early errors ----------------------------------------------
    //
    // A NEW BLOCK RATHER THAN A CONTINUATION OF 2200, and the reason is what the numbers are for.
    // Every one of these is an error about a MODULE, which is a goal symbol the front end did not
    // have when 2201 through 2210 were minted; a reader holding one of these numbers should be
    // able to tell from the number alone that the source was presented as a module, because that
    // is the fact that makes the rejection make sense.

    /// <summary>An <c>import</c> or <c>export</c> declaration in source presented as a script.</summary>
    /// <remarks>
    /// <b>This is a syntax error and NOT a construct outside the manifest, and until the module
    /// goal existed it was the other one.</b> The distinction is what the conformance runner grades
    /// on: a manifest refusal is scored <c>unsupported</c> and kept out of both columns, and a
    /// script containing <c>import</c> is a program every engine rejects - so scoring it
    /// <c>unsupported</c> would have declined a test this profile can answer. The manifest admits
    /// the declaration; this goal does not.
    /// </remarks>
    ModuleDeclarationOutsideModuleGoal = 2401,

    /// <summary>One module publishes the same export name twice.</summary>
    DuplicateExportName = 2402,

    /// <summary>An export clause names a binding the module does not declare.</summary>
    /// <remarks>
    /// <c>export { a };</c> with no <c>a</c> in the module is an early error rather than a run-time
    /// <c>ReferenceError</c>, because the clause names a BINDING and not an expression - there is
    /// no evaluation in which it could come to exist. A re-export - <c>export { a } from './m'</c> -
    /// names a binding of the OTHER module and never reaches this code; whether that name exists
    /// there is settled at verification, where the whole graph is present.
    /// </remarks>
    ExportNameNotDeclared = 2403,
}

/// <summary>One refusal of source text: a code, a message, and where in the source it happened.</summary>
/// <remarks>
/// <para>
/// The position is a <b>source</b> line and column, one-based, and not a
/// <c>VmSourcePosition</c>. The core's position record is for positions inside an artifact, and
/// rule N9 keeps every construction of it inside the profile assembly's two factories; source
/// carrying an early error has no artifact for a position to be inside of. Reusing the core's
/// record here would put a second meaning on two fields whose meaning JSD-0009 has already
/// written down.
/// </para>
/// <para>
/// A column counts UTF-16 code units from the line's start, which is what a text editor counts
/// and what the tokenizer advances by. It is not a code-point count and does not pretend to be.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=74E348
// Broiler-Human:        PENDING
public sealed record SliceSourceDiagnostic(
    SliceSourceDiagnosticCode Code,
    string Message,
    int Line,
    int Column)
{
    /// <summary>The one-line form a check or a log prints.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7972D6
    // Broiler-Human:        PENDING
    public override string ToString() =>
        string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"{(int)Code}:{Code} at {Line}:{Column}: {Message}");
}
