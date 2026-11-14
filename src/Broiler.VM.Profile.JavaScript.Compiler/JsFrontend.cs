// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           4
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The wide path's parse: the shared syntax library's request built from this family's options, its
/// answer mapped onto this family's registry, and its tree handed to the adapter (JSD-0083, decisions
/// D10, D11, D15, D16 and D18; milestone SJL-13.3, step 1).
/// </summary>
/// <remarks>
/// <para>
/// <b><see cref="SliceParseOptions"/> stays this family's options, and the request is built here</b>
/// (decision D16): the goal and the nesting bound from the options, the eval flags as forced strictness
/// and the inherited context, and top-level <c>await</c> from the module options, as the wide parser
/// read them. No shared type crosses this assembly's public surface (rule I6).
/// </para>
/// <para>
/// <b>The order is the wide parser's, with one stage added</b> (decision D18): the library's parse, and
/// when it reported nothing, the admission walk over the shared tree, which makes the BigInt refusals
/// and the module specifier's well-formedness that the wide parser made during the parse, and records
/// each admitted BigInt's value and whether the unit awaits at its top level. A refusal of either stops
/// the unit before the numeric admission and the binder, as the parser's did.
/// </para>
/// <para>
/// <b>A refused unit's list holds the library's first refusal alone, and it is this family's</b>
/// (decision D11): the request asks the library for its first refusal only, the library's shared code
/// becomes the registry code the mapping gives it, and its text is the one this family rendered for that
/// rule.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=008648
// Broiler-Falsified-If: a source the wide path parsed is parsed under another goal, strictness, function context, top-level-await switch or nesting bound than its options and eval flags give, or a unit the parse or the admission walk refused reaches the numeric admission or the binder
// Broiler-Human:        PENDING
internal static class JsFrontend
{
    /// <summary>
    /// The answer of one parse: the trees, the refusal, the module's top-level await, and the side tables
    /// the lowering reads beside the shared tree, each keyed by a node's identity (SJL-13.3, step 2).
    /// </summary>
    /// <param name="Tree">The library's tree, or <see langword="null"/> when the unit was refused.</param>
    /// <param name="Diagnostics">
    /// The library's first refusal alone (decision D11), or the admission walk's refusals, in the order
    /// this family reports them; empty for an accepted unit.
    /// </param>
    /// <param name="SawTopLevelAwait">
    /// Whether the unit awaits outside every function, by the wide parser's rule (decision D20).
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B3A11F
    // Broiler-Human:        PENDING
    internal sealed record Parsed(
        ProgramNode? Tree,
        System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics,
        bool SawTopLevelAwait)
    {
        /// <summary>The unit's source text, which a string literal's raw text and the end of the source are read from.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C91EFB
        // Broiler-Human:        PENDING
        internal string Text { get; init; } = string.Empty;

        /// <summary>A module's retained tokens, whose anchors the tree does not carry; empty for a script.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1A4886
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<Token> Tokens { get; init; } = [];

        /// <summary>Each admitted BigInt literal's value, a key's included, by the literal's identity: the admission walk's.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6A8A0C
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyDictionary<NumericLiteral, System.Numerics.BigInteger> BigInts { get; init; } =
            new System.Collections.Generic.Dictionary<NumericLiteral, System.Numerics.BigInteger>();

        /// <summary>The strictness the library decided for each function and for the program, by the node's identity.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4D39DE
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyDictionary<SyntaxNode, bool> Strictness { get; init; } =
            new System.Collections.Generic.Dictionary<SyntaxNode, bool>();

    }

    /// <summary>
    /// Parses one unit of the wide path through the shared library, then makes the admission walk over
    /// its tree and adapts it to <see cref="JsSyntax"/>.
    /// </summary>
    /// <param name="text">The unit's source text.</param>
    /// <param name="options">The unit's parse options, eval flags included.</param>
    /// <param name="forceStrict">
    /// Whether the unit is strict code by the caller's choice: the script's forced strictness, and for
    /// eval code its request's <c>Strict</c> flag, which the caller folds in once for the parse and the
    /// binder alike (decision D16).
    /// </param>
    /// <param name="admitsBigInt">Whether the request's manifest admits BigInt literals.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=498599
    // Broiler-Falsified-If: a unit with any library diagnostic or admission refusal answers a tree, a unit with none answers a refusal, or an accepted unit's top-level-await answer is not the admission walk's
    // Broiler-Human:        PENDING
    internal static Parsed Parse(string text, SliceParseOptions options, bool forceStrict, bool admitsBigInt)
    {
        var result = SyntaxParser.Parse(Request(text, options, forceStrict));

        if (result.Diagnostics.Count != 0 || result.Tree is null)
        {
            var refusals = new System.Collections.Generic.List<SliceSourceDiagnostic>(result.Diagnostics.Count);

            foreach (var diagnostic in result.Diagnostics)
            {
                if (refusals.Count >= SliceSourceDiagnostic.MaximumPerStage)
                {
                    break;
                }

                refusals.Add(JsFrontendDiagnostics.Map(diagnostic, text));
            }

            return new Parsed(null, refusals, false);
        }

        var admission = JsAdmissionWalk.Walk(result.Tree, options, admitsBigInt);

        if (admission.Count != 0)
        {
            return new Parsed(null, admission.Refusals(), false);
        }

        var tables = result.Declarations ?? throw new System.InvalidOperationException(
            "the library answered an accepted parse with no declaration tables");

        var strictness = new System.Collections.Generic.Dictionary<SyntaxNode, bool>(
            System.Collections.Generic.ReferenceEqualityComparer.Instance);

        foreach (var facts in tables.Functions)
        {
            strictness[facts.Node] = facts.IsStrict;
        }

        return new Parsed(result.Tree, [], admission.SawTopLevelAwait)
        {
            Text = text,
            Tokens = result.Tokens,
            BigInts = admission.BigIntValues,
            Strictness = strictness,
        };
    }

    /// <summary>The library's request for one unit, from this family's options (decision D16).</summary>
    /// <remarks>
    /// <para>
    /// <b>Eval code is the eval goal</b>, the forced strictness is the caller's (its <c>Strict</c> flag
    /// already folded in, the one site of that mapping being the compiler's, which hands the binder the
    /// same value), and its function, method, derived-constructor and field-initialiser flags are the
    /// inherited context. The visible private names are not passed (decision D16): an evaluated
    /// program's undeclared private name stays the executor's run-time check.
    /// </para>
    /// <para>
    /// <b>Top-level <c>await</c> is the module options' switch</b>, as the wide parser read it: a
    /// script never awaits at its top level, whatever its options say.
    /// </para>
    /// <para>
    /// <b>HTML-like comments by goal and Annex B on</b> are the library's defaults for each goal
    /// (decision D17), and so are the absent proposals and lexer extensions: this family admits none.
    /// The library is asked for its first refusal only (decision D11 as revised on 2026-10-26; the
    /// all-errors mode stays the library's parameter for an engine that asks for it later), and the
    /// regular-expression literal is validated by this family's validator (decision D7). Tokens are
    /// retained for the module goal alone, whose anchors the tree does not carry (decision D15).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8B2805
    // Broiler-Falsified-If: the caller's forced strictness does not force strict code, an eval unit's function flag does not admit new.target, a script's options admit top-level await, a module's nesting bound differs from its options', or a refused unit reports more than one parse diagnostic
    // Broiler-Human:        PENDING
    private static ParseRequest Request(string text, SliceParseOptions options, bool forceStrict)
    {
        var flags = options.EvalFlags;
        var evaluated = options.IsEval;
        var goal = options.Goal == SliceGoal.Module
            ? ParseGoal.Module
            : evaluated ? ParseGoal.Eval : ParseGoal.Script;

        var switches = ParseSwitches.For(goal) with
        {
            ForcedStrict = forceStrict,
            TopLevelAwait = goal == ParseGoal.Module && options.AllowTopLevelAwait,
        };

        var limits = new ParseLimits
        {
            NestingDepth = new Broiler.ECMAScript.DepthLimit(options.MaximumNestingDepth),
            DiagnosticCap = SliceSourceDiagnostic.MaximumPerStage,
            Reporting = ErrorReporting.FirstError,
        };

        var source = new SourceText(text);

        return evaluated
            ? new ParseRequest(source, goal, JsCompilerUnicode.Tables, JsRegularExpressionValidator.Instance)
            {
                Switches = switches,
                Limits = limits,
                Context = new InheritedContext
                {
                    InFunction = (flags & Format.JsFormat.EvalRequestFlags.InFunction) != 0,
                    InMethod = (flags & Format.JsFormat.EvalRequestFlags.InMethod) != 0,
                    InDerivedConstructor = (flags & Format.JsFormat.EvalRequestFlags.InDerivedConstructor) != 0,
                    InClassFieldInitializer = (flags & Format.JsFormat.EvalRequestFlags.InClassFieldInitializer) != 0,
                },
            }
            : new ParseRequest(source, goal, JsCompilerUnicode.Tables, JsRegularExpressionValidator.Instance)
            {
                Switches = switches,
                Limits = limits,
                RetainTokens = goal == ParseGoal.Module,
            };
    }
}
