// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   143
// Annotated:        143/143
// Exempt:           17
// Human-reviewed:   0/143
// IP risk:          None
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       143
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The wide surface's parser: the tokenizer's stream in, a precise syntax tree out.
/// </summary>
/// <remarks>
/// <para>
/// <b>It shares the tokenizer with the slice front end and nothing else.</b> The token stream is
/// the language's and has no manifest in it, so one pass over the characters serves both surfaces;
/// the trees differ because the two front ends do different things with them. This one produces a
/// record per production, because a lowering has to be able to ask which child of a call is the
/// callee.
/// </para>
/// <para>
/// <b>What it refuses, it refuses by name.</b> The wide manifest admits no
/// <c>async</c> function, module declaration, class field, private name, class static
/// block, decorator or generator MEMBER of a class body. Each is parsed far enough to be
/// recognised and then reported as a construct
/// outside the manifest, at its own position - not as an unexpected token, which would send a
/// reader looking for a typo. <c>await</c> and <c>yield</c> are not on that list and never were:
/// they are contextual keywords, and where the goal or the strictness reserves one the answer is
/// the reserved-word syntax error the language gives rather than a refusal this manifest owns.
/// </para>
/// <para>
/// <b>Twelve families left that list on 2026-09-04 and are now PARSED rather than named.</b> A
/// template literal and a tagged template become a <see cref="JsTemplateLiteral"/> with its chunks
/// split and its substitutions parsed; an optional chain becomes links carrying an
/// <c>Optional</c> flag inside one <see cref="JsChainExpression"/>, which is the node that owns
/// where the short circuit lands; <c>new.target</c> becomes a node of its own, admitted only where
/// the grammar admits it - inside an ordinary function body, and not at the top level of a script
/// even through an arrow; a class declaration, a class expression and <c>super</c> become nodes of
/// their own; and parameter defaults, rest parameters, spread, destructuring - in a declaration,
/// an assignment, a parameter and a catch clause alike - and <c>for … of</c> are admitted too.
/// </para>
/// <para>
/// <b>Admitting a family removes the refusal something was relying on, and the risk is never that
/// the family stops working: it is that a SIBLING family stops being refused BY NAME</b> and comes
/// back as a surprise token, which the conformance runner scores as a failure and which turns a
/// negative test expecting a <c>SyntaxError</c> into a false pass. Two things follow. The branches
/// that recognise a still-refused construct stay ahead of the ones that now parse - <c>*</c>
/// before a key in a CLASS BODY is still a refused generator member where the same <c>*</c> in an
/// object literal is now a generator method, and <c>...</c> in an object literal is a spread -
/// and every new EXPRESSION POSITION the change opened has to answer the same way the old ones do:
/// a parameter default, a pattern's default, the source of a <c>for … of</c> and the argument of a
/// spread all reach the same primary-expression path, and a shorthand binding's key goes through
/// the reserved-word answer <see cref="BindingName"/> gives one step away. The class family is the
/// case in point on the other side of the same rule: before classes were admitted, every construct
/// that can only appear inside a class body was covered by one refusal naming the class, and
/// admitting the body means each of them now needs a refusal of its own, in the position it
/// appears in - a field wherever a member may stand, a private name in a member, in a property
/// access and as the left operand of <c>in</c>, a static block, and a decorator, which nothing
/// named before because the class refusal had covered it. Nothing else moved: every other refusal
/// above is still spelled where it was, because the conformance runner grades the manifest
/// boundary on the diagnostic code and a refusal removed by accident is a manifest change nobody
/// declared.
/// </para>
/// <para>
/// <b>The generator family left that list on 2026-09-04 and nothing else moved onto it.</b>
/// <c>function*</c>, <c>{ *m() {} }</c>, <c>yield</c> and <c>yield*</c> are parsed rather than
/// refused; a generator MEMBER of a class body is not, and is still refused by name, because the
/// class family and the generator family were admitted by two different bundles and nothing has
/// yet written the member that is both. <c>async function*</c> is still refused and is now named
/// as an async GENERATOR function rather than as an async one, which is a narrower name for the
/// same code and the same position.
/// The one thing a reader should check when reading the <c>yield</c> arms below is that a
/// <c>yield</c> that is NOT in a generator still answers exactly what it answered before - a name
/// in sloppy code, a reserved word in strict code - because admitting a construct must not change
/// what an unrelated program is told.
/// </para>
/// <para>
/// <b>And it refuses by name in EVERY position the construct can appear in, which is a stronger
/// claim than it sounds and was not true until 2026-09-04.</b> The refusals were written where a
/// construct usually appears - <c>async function</c> at statement position, a destructuring
/// pattern in a declarator - so the same construct one level deeper came back as an unexpected
/// or expected token. That is not cosmetic. The conformance runner decides its
/// <c>unsupported</c> verdict on the diagnostic CODE, so a construct refused under any other code
/// is scored a failure; and a negative test expecting a <c>SyntaxError</c> at parse is scored a
/// PASS, because a syntax error was indeed produced - for entirely the wrong reason. An audit of
/// every construct family against every syntactic position it admits found the leak in six of
/// them, and what closed it is: <c>async</c> functions and arrows in expression position,
/// <c>async</c> and generator methods in an object literal, <c>for await</c>, dynamic
/// <c>import()</c> and <c>import.meta</c> as expressions, <c>await</c> and <c>yield</c> as the
/// callee of a <c>new</c>, a destructuring assignment without a declaration, <c>let</c> before a
/// binding pattern, and a label that is a contextual keyword.
/// </para>
/// <para>
/// <b>The <c>with</c> statement left that list on 2026-09-04 and opened a POSITION rather than an
/// expression.</b> A <c>with</c> body is an ordinary <c>Statement</c>, so every construct still
/// refused can now be written one level inside one, and each has to answer there with its own name
/// exactly as it does at the top level - which it does, because a <c>with</c> body is parsed by
/// <see cref="ParseStatement"/> and by nothing of its own. What <c>with</c> adds to this list
/// instead is two refusals of its own, both <c>2101</c> rather than <c>2104</c> because the
/// manifest now admits the statement and the LANGUAGE is what has nothing here: <c>with</c> in
/// strict code, and a declaration as its body.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3939D8
// Broiler-Human:        PENDING
internal sealed class JsParser
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=840A20
    // Broiler-Human:        PENDING
    private readonly SliceToken[] tokens;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B380E2
    // Broiler-Human:        PENDING
    private readonly SliceParseOptions options;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=98C321
    // Broiler-Human:        PENDING
    private int at;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=329437
    // Broiler-Human:        PENDING
    private int depth;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=989AE3
    // Broiler-Human:        PENDING
    private bool strict;

    /// <summary>
    /// How many ordinary function bodies enclose the cursor, which is what <c>new.target</c> needs.
    /// </summary>
    /// <remarks>
    /// <b>An arrow does not count, and that is the whole reason this is a count of ORDINARY
    /// bodies.</b> An arrow has no <c>new.target</c> of its own - it reads the enclosing function's,
    /// exactly as it reads the enclosing <c>this</c> - so <c>function f() { return () =&gt;
    /// new.target; }</c> is admitted while <c>() =&gt; new.target</c> at the top level is the
    /// syntax error every engine reports. A counter that incremented for arrows would answer the
    /// second one wrong in the direction that matters, by admitting a program the language does not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=837D17
    // Broiler-Human:        PENDING
    private int functionDepth;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E41B2B
    // Broiler-Human:        PENDING
    private bool sawTopLevelAwait;

    /// <summary>
    /// Whether the parser is inside a generator's own <c>[+Yield]</c> context, where <c>yield</c>
    /// is the operator and cannot be a name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is true in a generator's own body and its own parameter list, and NOWHERE else.</b>
    /// A nested ordinary function is a fresh <c>[~Yield]</c> context; so is a nested arrow, whose
    /// concise and braced bodies the grammar both parse under <c>[~Yield]</c> even though its
    /// PARAMETERS inherit <c>[?Yield]</c>. That is exactly the rule that makes one heap frame
    /// enough for a suspension: the only code that can suspend a generator's frame is the
    /// generator's own.
    /// </para>
    /// <para>
    /// <b>One flag rather than two, because where <c>yield</c> is the operator it is also the
    /// reserved word.</b> The pairing that looks necessary - an arrow inside a generator, where it
    /// is not the operator - is not: <c>function* g() { var f = () =&gt; yield; }</c> parses in
    /// every engine, with <c>yield</c> an ordinary identifier reference, and it is strict mode
    /// rather than the enclosing generator that reserves the name there.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=64C71A
    // Broiler-Human:        PENDING
    private bool yieldIsOperator;

    /// <summary>
    /// Whether the parser is inside an async function's own <c>[+Await]</c> context, where
    /// <c>await</c> is the operator and cannot be a name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the <c>yield</c> flag's twin in every respect but one, and the one is the
    /// arrow.</b> It is true in an async function's own body and its own parameter list and nowhere
    /// else - a nested ordinary function is a fresh <c>[~Await]</c> context, and so is a nested
    /// ordinary arrow. But an ASYNC arrow's body is <c>[+Await]</c>, which a generator has no
    /// counterpart for, so this flag is set by the arrow path as well as by the function path. That
    /// is the whole reason the two flags are separate rather than one "may suspend" bit.
    /// </para>
    /// <para>
    /// <b>Where <c>await</c> is NOT the operator, nothing about it changed.</b> It is an ordinary
    /// identifier in a script and a reserved word in a module, exactly as it was before this
    /// manifest admitted an async function, and the answer to a program that binds it outside an
    /// async body is still the answer that program has always had. Admitting a construct must not
    /// change what an unrelated program is told.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9D6B6E
    // Broiler-Human:        PENDING
    private bool awaitIsOperator;

    /// <summary>Creates a parser over an already-tokenized source.</summary>
    /// <param name="stream">The tokens to read.</param>
    /// <param name="parseOptions">The goal and the ceilings this parse is held to.</param>
    /// <param name="forceStrict">
    /// <b>Strictness the caller imposes rather than the source declaring it</b>, which is what a
    /// conformance runner does to produce the strict variant of a test. It has to reach the PARSE
    /// and not only the lowering, because strict mode changes the GRAMMAR: <c>yield</c> becomes a
    /// reserved word, a legacy octal literal becomes a syntax error, and both are early errors a
    /// lowering never gets to see because the parse already succeeded. Until 2026-09-04 this flag
    /// reached the lowering only, so every strict-only early error was invisible in exactly the
    /// variant that exists to test it.
    /// </param>
    /// <param name="enclosingFunctionDepth">
    /// How many ordinary function bodies enclose the tokens this parser is given. It is non-zero
    /// only for the sub-parse of a template substitution, whose tokens are a slice of a source
    /// this parser never sees the rest of - so the nesting it sits inside has to be handed to it.
    /// </param>
    /// <param name="enclosingYieldIsOperator">
    /// Whether the enclosing parse was inside a generator's own body.
    /// </param>
    /// <param name="enclosingAwaitIsOperator">
    /// <b>Whether the enclosing parse was inside an async function's own body</b>, which a
    /// substitution's sub-parse has to be told for the same reason it has to be told the nesting
    /// depth: <c>`x${await p}`</c> inside an async function is an await expression, and a
    /// sub-parser that started in a fresh context read it as the identifier <c>await</c> followed
    /// by a surprise. The <c>[Yield]</c> half beside it had the same hole and is closed with it.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F34641
    // Broiler-Human:        PENDING
    internal JsParser(
        SliceToken[] stream,
        SliceParseOptions parseOptions,
        bool forceStrict = false,
        int enclosingFunctionDepth = 0,
        bool enclosingYieldIsOperator = false,
        bool enclosingAwaitIsOperator = false)
    {
        tokens = stream;
        options = parseOptions;
        strict = forceStrict;
        functionDepth = enclosingFunctionDepth;
        yieldIsOperator = enclosingYieldIsOperator;
        awaitIsOperator = enclosingAwaitIsOperator;
    }

    /// <summary>Every refusal this pass produced, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BB02CF
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics => diagnostics;

    /// <summary>Whether the program's directive prologue asked for strict mode.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0E6F1A
    // Broiler-Human:        PENDING
    internal bool IsStrict => strict;

    /// <summary>Whether an <c>await</c> appeared outside every function of this parse.</summary>
    /// <remarks>
    /// It is a property of the PARSE and not of the tree, because the tree records an await
    /// expression without recording how deep in functions it was, and walking the tree afterwards
    /// to work that out would be a second implementation of a fact the parser had in its hand.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=76E556
    // Broiler-Human:        PENDING
    internal bool SawTopLevelAwait => sawTopLevelAwait;

    /// <summary>Parses a whole program.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F0558F
    // Broiler-Human:        PENDING
    internal JsProgramNode Parse()
    {
        var span = Span();
        var directives = ParseDirectives();

        if (options.Goal == SliceGoal.Module)
        {
            strict = true;

            // TOP-LEVEL `await` IS AN OPERATOR IN A MODULE AND AN IDENTIFIER IN A SCRIPT, and this
            // one line is the whole of the grammar difference. A module's top level is an async
            // context by the goal symbol rather than by an enclosing `async function`, so the same
            // switch every async body sets is set here - which is what makes `await p` at a
            // module's top level an await expression instead of two identifiers in a row.
            awaitIsOperator = true;
        }

        // NOTHING TURNS STRICTNESS OFF. A directive prologue can only add it, the module goal can
        // only add it, and a caller that imposed it keeps it - so a `"use strict"` inside a
        // function cannot be undone by an inner function without one, and neither can this.

        var body = new System.Collections.Generic.List<JsStatement>();

        while (Current.Kind != SliceTokenKind.EndOfSource && diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        // A SCRIPT'S OWN TOP LEVEL IS A DECLARATIVE SCOPE and its declarations obey the same two
        // rules a function body's do. It has no parameters to collide with, which is the only part
        // of the check a program has nothing for.
        ValidateVarScope(body, null);

        return new JsProgramNode(span, directives, body, strict);
    }

    // ---- statements ----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9594DE
    // Broiler-Human:        PENDING
    private JsStatement ParseStatement()
    {
        if (!Enter(out var deep))
        {
            return deep;
        }

        try
        {
            var span = Span();

            switch (Current.Kind)
            {
                case SliceTokenKind.Semicolon:
                    Advance();
                    return new JsEmptyStatement(span);

                case SliceTokenKind.OpenBrace:
                    return ParseBlock();

                case SliceTokenKind.Var:
                case SliceTokenKind.Const:
                    return ParseVariableStatement();

                // `let [` AND `let {` ARE DECLARATIONS AND NOT INDEXING. Without them here,
                // `let [a] = b` parsed as a member expression assigned to, and came back as an
                // invalid assignment target rather than as the destructuring pattern it is. The
                // declaration path refuses the pattern by name, which is the answer this manifest
                // owes. `let` followed by anything else stays an identifier, which it is.
                case SliceTokenKind.Let when BeginsLetDeclaration():
                    return ParseVariableStatement();

                case SliceTokenKind.If:
                    return ParseIf();

                case SliceTokenKind.While:
                    return ParseWhile();

                case SliceTokenKind.Do:
                    return ParseDoWhile();

                case SliceTokenKind.For:
                    return ParseFor();

                case SliceTokenKind.Break:
                case SliceTokenKind.Continue:
                    return ParseBreakOrContinue();

                case SliceTokenKind.Return:
                    return ParseReturn();

                case SliceTokenKind.Throw:
                    return ParseThrow();

                case SliceTokenKind.Try:
                    return ParseTry();

                case SliceTokenKind.Switch:
                    return ParseSwitch();

                case SliceTokenKind.Function:
                    Advance();
                    return new JsFunctionDeclaration(span, ParseFunctionRest(span, declaration: true));

                case SliceTokenKind.Debugger:
                    Advance();
                    Semicolon();
                    return new JsDebuggerStatement(span);

                case SliceTokenKind.With:
                    return ParseWith();

                case SliceTokenKind.Class:
                    return new JsClassDeclaration(span, ParseClass(span, declaration: true));

                // A DYNAMIC `import()` AND `import.meta` ARE EXPRESSIONS AND REACH THIS POSITION.
                // Both are refused by name below, in ParsePrimary, and sending them into the
                // declaration parser here would report a missing brace instead of the construct.
                case SliceTokenKind.Import when Peek(1).Kind is not SliceTokenKind.OpenParen
                    and not SliceTokenKind.Dot:
                case SliceTokenKind.Export:
                    return ModuleItem(span);

                // ONE ARM FOR BOTH, WHERE THERE WERE TWO. An async generator was refused here by a
                // case that tested for the `*` before this one; it is admitted now, and the `*` is
                // read by `ParseFunctionRest` exactly as it is for `function*` - so the two
                // constructs differ in one bit of the node rather than in a branch of the parser.
                case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                    !Peek(1).PrecededByLineTerminator:
                    Advance();
                    Advance();

                    return new JsFunctionDeclaration(
                        span, ParseFunctionRest(span, declaration: true, isAsync: true));

                // A CONTEXTUAL KEYWORD IS A LEGAL LABEL. `of: for (var x of []) ;` did not
                // reach the `for … of` refusal at all, because `of` was not recognised as a label
                // and the colon became the surprise instead of the construct after it.
                case SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
                    SliceTokenKind.Of or SliceTokenKind.Static or SliceTokenKind.Async or
                    SliceTokenKind.Let when Peek(1).Kind == SliceTokenKind.Colon:
                {
                    _ = RefuseEscapedReservedWord(Current);
                    var label = Current.RawText;
                    Advance();
                    Advance();

                    // A LABELLED ITEM IS A `Statement` OR A FUNCTION DECLARATION AND NOTHING ELSE,
                    // so `label: let x;` is a syntax error while `label: function f() {}` is the
                    // Annex B form every engine admits in sloppy code.
                    return new JsLabelledStatement(
                        span, label, ParseNestedStatement("a label", functionAllowed: !strict));
                }

                default:
                    return ParseExpressionStatement();
            }
        }
        finally
        {
            depth--;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=074893
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsStringLiteral> ParseDirectives()
    {
        var directives = new System.Collections.Generic.List<JsStringLiteral>();

        // A DIRECTIVE MAY BE THE LAST THING IN A BODY, and until now one was not recognised there.
        // `function f() { "use strict" }` has a prologue - the string is an expression statement
        // whose semicolon the closing brace inserts - and reading it as an ordinary statement made
        // the body sloppy, which is a whole strictness silently lost. It also hid the rule that a
        // parameter list with a default in it may not be given a `use strict` directive, because
        // the directive the rule is about had not been seen.
        while (Current.Kind == SliceTokenKind.StringLiteral &&
            Peek(1).Kind is SliceTokenKind.Semicolon or SliceTokenKind.EndOfSource or
                SliceTokenKind.CloseBrace ||
            (Current.Kind == SliceTokenKind.StringLiteral && Peek(1).PrecededByLineTerminator))
        {
            var token = Current;
            var literal = new JsStringLiteral(
                Span(), token.StringValue, token.RawText);

            directives.Add(literal);

            if (string.Equals(token.RawText, "\"use strict\"", System.StringComparison.Ordinal) ||
                string.Equals(token.RawText, "'use strict'", System.StringComparison.Ordinal))
            {
                strict = true;
            }

            Advance();

            if (Current.Kind == SliceTokenKind.Semicolon)
            {
                Advance();
            }
        }

        return directives;
    }

    // ---- the module goal's declarations ---------------------------------------------------------

    /// <summary>
    /// The synthetic name a default export is bound to when the source gives it none.
    /// </summary>
    /// <remarks>
    /// It carries characters no identifier may contain, so it can never collide with a name the
    /// source declares - which matters because <c>export default 1 + 1</c> and
    /// <c>export default function f() {}</c> both bind a module slot, and only the second names it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=8B78FE
    // Broiler-Human:        PENDING
    internal const string DefaultBindingName = "*default*";

    /// <summary>The export name a bare <c>export * from</c> is recorded under.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=828A3E
    // Broiler-Human:        PENDING
    internal const string StarName = "*";

    /// <summary>
    /// Parses a module declaration where one may appear, and refuses one where it may not.
    /// </summary>
    /// <remarks>
    /// <b>A module declaration is a MODULE ITEM and not a statement, and the difference is a
    /// position rather than a spelling.</b> <c>() =&gt; { export default null; }</c> is a syntax
    /// error in every engine, because the grammar admits an export only at a module's top level -
    /// and a parser that recognised the declaration wherever a statement may stand accepted it,
    /// leaving the lowering to refuse a construct it had no way to place. That refusal named the
    /// manifest, which was wrong twice over: the manifest admits the declaration, and a conformance
    /// case expecting a syntax error was scored as a construct nobody implements.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2B5F1F
    // Broiler-Human:        PENDING
    private JsStatement ModuleItem(SliceSourceSpan span)
    {
        if (depth <= 1)
        {
            return Current.Kind == SliceTokenKind.Export
                ? ParseExportDeclaration(span)
                : ParseImportDeclaration(span);
        }

        Refuse(
            span,
            SliceSourceDiagnosticCode.UnexpectedToken,
            "`" + Describe(Current) + "` may appear only at a module's top level");

        Advance();
        return new JsEmptyStatement(span);
    }

    /// <summary>Parses an <c>import</c> declaration in every form the grammar has.</summary>
    /// <remarks>
    /// <b>The bindingless form is a form and not a degenerate case.</b> <c>import "./m.mjs";</c>
    /// requests a module for what evaluating it does and binds nothing, so a parser that required a
    /// clause would refuse a program the language admits - and the request still has to reach the
    /// module record, because the whole point of the statement is that the module is evaluated.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9ADEFA
    // Broiler-Human:        PENDING
    private JsStatement ParseImportDeclaration(SliceSourceSpan span)
    {
        if (!RequireModuleGoal(span, "import"))
        {
            return new JsEmptyStatement(span);
        }

        Advance();
        var specifiers = new System.Collections.Generic.List<JsImportSpecifier>();

        // THE BINDINGLESS FORM CARRIES A CLAUSE TOO. `import './m.mjs' with {};` requests a module
        // for what evaluating it does and binds nothing, and the attributes go on the REQUEST
        // rather than on the bindings - so a reading that returned here without looking for one
        // reported a missing semicolon against a program the grammar admits.
        if (Current.Kind == SliceTokenKind.StringLiteral)
        {
            var only = Current.StringValue;
            Advance();
            var bare = Attributes();
            Semicolon();
            return new JsImportDeclaration(span, only, specifiers, bare);
        }

        if (IsIdentifierName(Current.Kind))
        {
            var at = Span();
            specifiers.Add(new JsImportSpecifier(at, "default", BindingName(), Namespace: false));

            if (Current.Kind == SliceTokenKind.Comma)
            {
                Advance();
                ParseImportClause(specifiers);
            }
        }
        else
        {
            ParseImportClause(specifiers);
        }

        ExpectContextual("from");
        var from = ModuleSpecifier();
        var attributes = Attributes();
        Semicolon();
        return new JsImportDeclaration(span, from, specifiers, attributes);
    }

    /// <summary>
    /// Parses an import-attribute clause, with the two early errors the grammar states.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Parsing the clause and honouring an attribute are two different questions, and this
    /// answers only the first.</b> <c>import x from './m' with { type: 'json' }</c> is a program
    /// the grammar admits whatever a host can load, so refusing it as a construct outside the
    /// manifest said something false: the syntax is not what this profile declines. What it
    /// declines is the ATTRIBUTE, and the lowering says so where the module's loading requirements
    /// are settled — which for a static import is before a byte of the artifact is written.
    /// </para>
    /// <para>
    /// <b>Both early errors are the grammar's rather than this host's.</b> An attribute value must
    /// be a string literal, because the clause is read before anything is evaluated and an
    /// expression there would be a value nobody could have computed yet; and a key may not be
    /// written twice, because the clause is a map and a duplicate is a program that means two
    /// things. Neither is about what a host can load.
    /// </para>
    /// <para>
    /// <b>The legacy <c>assert</c> spelling is not accepted at all</b>, and that is a
    /// straightforward reading of the edition this front end is written against: the keyword was
    /// removed, so <c>assert</c> after a specifier is an identifier where the grammar expects a
    /// semicolon, which is the ordinary syntax error and not a refusal of a surface.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9B9E2E
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsImportAttribute> Attributes()
    {
        var attributes = new System.Collections.Generic.List<JsImportAttribute>();

        if (Current.Kind != SliceTokenKind.With)
        {
            return attributes;
        }

        Advance();
        Expect(SliceTokenKind.OpenBrace, "{");

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            var at = Span();

            // THE KEY IS AN IdentifierName AND NOT AN Identifier, which is the same rule an export
            // name obeys and for the same reason: nothing binds it, so a reserved word is a
            // perfectly good attribute name. The string form is in the grammar beside it.
            if (Current.Kind != SliceTokenKind.StringLiteral &&
                (Current.RawText.Length == 0 || !char.IsLetter(Current.RawText[0])))
            {
                Refuse(
                    at,
                    SliceSourceDiagnosticCode.ExpectedToken,
                    "an import attribute is named by an identifier or by a string");

                break;
            }

            var key = Current.Kind == SliceTokenKind.StringLiteral
                ? Current.StringValue
                : Current.RawText;

            Advance();
            Expect(SliceTokenKind.Colon, ":");

            if (Current.Kind != SliceTokenKind.StringLiteral)
            {
                Refuse(
                    Span(),
                    SliceSourceDiagnosticCode.ExpectedToken,
                    "an import attribute's value is a string literal");

                break;
            }

            foreach (var earlier in attributes)
            {
                if (string.Equals(earlier.Key, key, System.StringComparison.Ordinal))
                {
                    Refuse(
                        at,
                        SliceSourceDiagnosticCode.DuplicateExportName,
                        "the import attribute `" + key + "` is given twice");

                    break;
                }
            }

            attributes.Add(new JsImportAttribute(at, key, Current.StringValue));
            Advance();

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        return attributes;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=48B6E3
    // Broiler-Human:        PENDING
    private void ParseImportClause(
        System.Collections.Generic.List<JsImportSpecifier> specifiers)
    {
        if (Current.Kind == SliceTokenKind.Star)
        {
            var at = Span();
            Advance();
            ExpectContextual("as");
            specifiers.Add(new JsImportSpecifier(at, string.Empty, BindingName(), Namespace: true));
            return;
        }

        Expect(SliceTokenKind.OpenBrace, "{");

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            var at = Span();
            var imported = ModuleExportName();
            var local = imported;

            if (IsContextual("as"))
            {
                Advance();
                local = BindingName();
            }

            specifiers.Add(new JsImportSpecifier(at, imported, local, Namespace: false));

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");
    }

    /// <summary>Parses an <c>export</c> declaration in every form the grammar has.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=FF4160
    // Broiler-Human:        PENDING
    private JsStatement ParseExportDeclaration(SliceSourceSpan span)
    {
        if (!RequireModuleGoal(span, "export"))
        {
            return new JsEmptyStatement(span);
        }

        Advance();

        return Current.Kind switch
        {
            SliceTokenKind.Star => ParseExportAll(span),
            SliceTokenKind.OpenBrace => ParseExportClause(span),
            SliceTokenKind.Default => ParseExportDefault(span),
            _ => ParseExportDeclared(span),
        };
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DF31CB
    // Broiler-Human:        PENDING
    private JsStatement ParseExportAll(SliceSourceSpan span)
    {
        Advance();
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        if (IsContextual("as"))
        {
            var at = Span();
            Advance();
            specifiers.Add(new JsExportSpecifier(at, StarName, ModuleExportName()));
        }

        ExpectContextual("from");
        var from = ModuleSpecifier();
        var attributes = Attributes();
        Semicolon();

        return new JsExportDeclaration(
            span, JsExportKind.All, from, specifiers, null, null, attributes);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E39779
    // Broiler-Human:        PENDING
    private JsStatement ParseExportClause(SliceSourceSpan span)
    {
        Advance();
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            var at = Span();
            var local = ModuleExportName();
            var exported = local;

            if (IsContextual("as"))
            {
                Advance();
                exported = ModuleExportName();
            }

            specifiers.Add(new JsExportSpecifier(at, local, exported));

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        var from = string.Empty;
        System.Collections.Generic.List<JsImportAttribute>? attributes = null;

        if (IsContextual("from"))
        {
            Advance();
            from = ModuleSpecifier();
            attributes = Attributes();
        }

        Semicolon();

        return new JsExportDeclaration(
            span, JsExportKind.Named, from, specifiers, null, null, attributes);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=46D14A
    // Broiler-Human:        PENDING
    private JsStatement ParseExportDefault(SliceSourceSpan span)
    {
        Advance();

        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        // A DEFAULT-EXPORTED CLASS IS A DECLARATION AND NOT AN EXPRESSION, which is why it is
        // taken here rather than left to the assignment parser below. `export default class {}`
        // binds the class in the module and publishes it as `default`; parsed as an expression it
        // would still evaluate, and the anonymous form would take the name `default` from a rule
        // about expressions rather than from the one about declarations that actually applies.
        if (Current.Kind == SliceTokenKind.Class)
        {
            var at = Span();
            var declared = ParseClass(at, declaration: false);

            if (declared.Name.Length == 0)
            {
                declared = declared with { Name = "default" };
            }

            specifiers.Add(new JsExportSpecifier(span, declared.Name, "default"));

            return new JsExportDeclaration(
                span,
                JsExportKind.Default,
                string.Empty,
                specifiers,
                new JsClassDeclaration(at, declared),
                null);
        }

        // ONE ARM FOR BOTH, WHERE THERE WERE TWO. `export default async function* () {}` was
        // refused by a case that tested for the `*` ahead of this one, and it was refused after the
        // async generator itself had been admitted everywhere else — a gap in `export default`
        // rather than in the family. `ParseFunctionRest` reads the `*` exactly as it does for
        // `function*`, so the two constructs differ in one bit of the node and in no branch here.
        if (Current.Kind == SliceTokenKind.Async && Peek(1).Kind == SliceTokenKind.Function &&
            !Peek(1).PrecededByLineTerminator)
        {
            var at = Span();
            Advance();
            Advance();
            var asyncFunction = ParseFunctionRest(at, declaration: false, isAsync: true);

            if (asyncFunction.Name.Length == 0)
            {
                asyncFunction = asyncFunction with { Name = "default" };
            }

            specifiers.Add(new JsExportSpecifier(span, asyncFunction.Name, "default"));

            return new JsExportDeclaration(
                span,
                JsExportKind.Default,
                string.Empty,
                specifiers,
                new JsFunctionDeclaration(at, asyncFunction),
                null);
        }

        // A NAMED DEFAULT-EXPORTED FUNCTION IS ALSO AN ORDINARY DECLARATION. `export default
        // function f() {}` binds `f` in the module and publishes it as `default`, so the name the
        // export reads from is the function's own where it has one. Only the anonymous form needs
        // the synthetic name, and giving the named form one too would leave `f` unbound.
        if (Current.Kind == SliceTokenKind.Function)
        {
            var at = Span();
            Advance();
            var function = ParseFunctionRest(at, declaration: false);

            // AN ANONYMOUS DEFAULT-EXPORTED FUNCTION IS NAMED `default`, which the language says
            // and which a program can observe through the function's own `name`. The binding it is
            // held in is named that too; `default` is a reserved word, so no source can reference
            // it and no collision with a name somebody wrote is possible.
            if (function.Name.Length == 0)
            {
                function = function with { Name = "default" };
            }

            specifiers.Add(new JsExportSpecifier(span, function.Name, "default"));

            return new JsExportDeclaration(
                span,
                JsExportKind.Default,
                string.Empty,
                specifiers,
                new JsFunctionDeclaration(at, function),
                null);
        }

        var value = ParseAssignment();
        Semicolon();
        specifiers.Add(new JsExportSpecifier(span, DefaultBindingName, "default"));

        return new JsExportDeclaration(
            span, JsExportKind.Default, string.Empty, specifiers, null, value);
    }

    /// <summary>Collects the names a binding pattern binds, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D61F0A
    // Broiler-Human:        PENDING
    private static void PatternNames(
        JsPattern pattern, System.Collections.Generic.List<string> names)
    {
        switch (pattern)
        {
            case JsTargetPattern { Target: JsIdentifier identifier }:
                names.Add(identifier.Name);
                return;

            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        PatternNames(element.Target, names);
                    }
                }

                if (array.Rest is not null)
                {
                    PatternNames(array.Rest, names);
                }

                return;

            case JsObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    PatternNames(property.Value.Target, names);
                }

                if (literal.Rest is not null)
                {
                    PatternNames(literal.Rest, names);
                }

                return;

            default:
                return;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4D6FDD
    // Broiler-Human:        PENDING
    private JsStatement ParseExportDeclared(SliceSourceSpan span)
    {
        var specifiers = new System.Collections.Generic.List<JsExportSpecifier>();

        switch (Current.Kind)
        {
            case SliceTokenKind.Class:
            {
                var at = Span();
                var declared = ParseClass(at, declaration: true);
                specifiers.Add(new JsExportSpecifier(at, declared.Name, declared.Name));

                return new JsExportDeclaration(
                    span,
                    JsExportKind.Declaration,
                    string.Empty,
                    specifiers,
                    new JsClassDeclaration(at, declared),
                    null);
            }

            // AND THE SAME ONE ARM HERE. `export async function* g() {}` is the declaration form of
            // what the line above admits, and it was refused by the same superseded case.
            case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                !Peek(1).PrecededByLineTerminator:
            {
                var at = Span();
                Advance();
                Advance();
                var asyncFunction = ParseFunctionRest(at, declaration: true, isAsync: true);
                specifiers.Add(new JsExportSpecifier(at, asyncFunction.Name, asyncFunction.Name));

                return new JsExportDeclaration(
                    span,
                    JsExportKind.Declaration,
                    string.Empty,
                    specifiers,
                    new JsFunctionDeclaration(at, asyncFunction),
                    null);
            }

            case SliceTokenKind.Function:
            {
                var at = Span();
                Advance();
                var function = ParseFunctionRest(at, declaration: true);
                specifiers.Add(new JsExportSpecifier(at, function.Name, function.Name));

                return new JsExportDeclaration(
                    span,
                    JsExportKind.Declaration,
                    string.Empty,
                    specifiers,
                    new JsFunctionDeclaration(at, function),
                    null);
            }

            case SliceTokenKind.Var:
            case SliceTokenKind.Let:
            case SliceTokenKind.Const:
            {
                var at = Span();
                var declaration = ParseVariableStatement();

                // A DESTRUCTURING DECLARATOR EXPORTS EVERY NAME ITS PATTERN BINDS, not one name of
                // its own - it has none. `export var [a, b] = c;` publishes `a` and `b`, and a
                // declarator that destructures carries an empty `Name`, so reading that field alone
                // would publish one export named the empty string and none of the real ones.
                foreach (var declarator in declaration.Declarators)
                {
                    if (declarator.Pattern is null)
                    {
                        specifiers.Add(new JsExportSpecifier(at, declarator.Name, declarator.Name));
                        continue;
                    }

                    var bound = new System.Collections.Generic.List<string>();
                    PatternNames(declarator.Pattern, bound);

                    foreach (var name in bound)
                    {
                        specifiers.Add(new JsExportSpecifier(at, name, name));
                    }
                }

                return new JsExportDeclaration(
                    span, JsExportKind.Declaration, string.Empty, specifiers, declaration, null);
            }

            default:
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`" + Describe(Current) + "` begins no export declaration");

                Advance();
                return new JsEmptyStatement(span);
        }
    }

    /// <summary>
    /// Refuses a module declaration in source presented as a script, and says which it was.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AA8E31
    // Broiler-Human:        PENDING
    private bool RequireModuleGoal(SliceSourceSpan span, string what)
    {
        if (options.Goal == SliceGoal.Module)
        {
            return true;
        }

        Refuse(
            span,
            SliceSourceDiagnosticCode.ModuleDeclarationOutsideModuleGoal,
            "`" + what + "` is a module declaration and this source was presented as a script");

        Advance();
        return false;
    }

    /// <summary>
    /// Reads a name an <c>import</c> or <c>export</c> clause may use on either side of <c>as</c>.
    /// </summary>
    /// <remarks>
    /// <b>It is an IdentifierName and not an Identifier</b>, so <c>export { x as default }</c> and
    /// <c>import { default as d }</c> parse - a reserved word is a perfectly good export name and
    /// only the LOCAL side of a binding has to be a name the goal admits as an identifier. The
    /// string form is admitted too, because <c>export { x as "a b" }</c> is in the grammar.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=6F84A0
    // Broiler-Human:        PENDING
    private string ModuleExportName()
    {
        if (Current.Kind == SliceTokenKind.StringLiteral)
        {
            var text = Current.StringValue;
            Advance();
            return text;
        }

        if (Current.Kind is SliceTokenKind.Identifier or SliceTokenKind.ReservedWord ||
            Current.RawText.Length != 0 && char.IsLetter(Current.RawText[0]))
        {
            return MemberName();
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.UnexpectedToken,
            "`" + Describe(Current) + "` is not an export name");

        Advance();
        return "#invalid";
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F682E3
    // Broiler-Human:        PENDING
    private string ModuleSpecifier()
    {
        if (Current.Kind == SliceTokenKind.StringLiteral)
        {
            var text = Current.StringValue;
            Advance();
            return text;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ExpectedToken,
            "a module specifier was expected and `" + Describe(Current) + "` was found");

        Advance();
        return string.Empty;
    }

    /// <summary>Whether the current token is the contextual keyword <paramref name="word"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C69905
    // Broiler-Human:        PENDING
    private bool IsContextual(string word) =>
        Current.Kind is SliceTokenKind.Identifier or SliceTokenKind.Of or SliceTokenKind.Get or
            SliceTokenKind.Set or SliceTokenKind.Async or SliceTokenKind.Static or
            SliceTokenKind.Let &&
        string.Equals(Current.RawText, word, System.StringComparison.Ordinal);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1A1D77
    // Broiler-Human:        PENDING
    private void ExpectContextual(string word)
    {
        if (IsContextual(word))
        {
            Advance();
            return;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ExpectedToken,
            "`" + word + "` was expected and `" + Describe(Current) + "` was found");
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E472A5
    // Broiler-Human:        PENDING
    private JsStatement ParseExpressionStatement()
    {
        var span = Span();
        var expression = ParseExpression();
        Semicolon();
        return new JsExpressionStatement(span, expression);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D1B875
    // Broiler-Human:        PENDING
    private JsBlockStatement ParseBlock()
    {
        var span = Span();
        Expect(SliceTokenKind.OpenBrace, "{");
        var body = new System.Collections.Generic.List<JsStatement>();

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        ValidateBlockScope(body);
        return new JsBlockStatement(span, body);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=839871
    // Broiler-Human:        PENDING
    private JsVariableStatement ParseVariableStatement()
    {
        var span = Span();
        var kind = Current.Kind switch
        {
            SliceTokenKind.Var => SliceDeclarationKind.Var,
            SliceTokenKind.Let => SliceDeclarationKind.Let,
            _ => SliceDeclarationKind.Const,
        };

        Advance();
        var declarators = ParseDeclarators(noIn: false);
        Semicolon();
        ValidateBindingList(kind, declarators);
        return new JsVariableStatement(span, kind, declarators);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B5197D
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsDeclarator> ParseDeclarators(bool noIn)
    {
        var declarators = new System.Collections.Generic.List<JsDeclarator>();

        while (true)
        {
            var span = Span();
            JsPattern? pattern = null;
            var name = string.Empty;

            if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
            {
                pattern = ParseBindingPattern();
            }
            else
            {
                name = BindingName();
            }

            JsExpression? initialiser = null;

            if (Current.Kind == SliceTokenKind.Equals)
            {
                Advance();
                initialiser = ParseAssignment(noIn);
            }

            declarators.Add(new JsDeclarator(span, name, pattern, initialiser));

            if (Current.Kind != SliceTokenKind.Comma)
            {
                return declarators;
            }

            Advance();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A7885F
    // Broiler-Human:        PENDING
    private JsStatement ParseIf()
    {
        var span = Span();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");

        // AN `if` CLAUSE IS THE ONE POSITION ANNEX B ADMITS A FUNCTION DECLARATION IN, and it
        // admits it in sloppy code only. Everything else a declaration can begin with is refused
        // here, which is the whole of the `statements/if` family.
        var consequent = ParseNestedStatement("an `if`", functionAllowed: !strict);
        JsStatement? alternate = null;

        if (Current.Kind == SliceTokenKind.Else)
        {
            Advance();
            alternate = ParseNestedStatement("an `else`", functionAllowed: !strict);
        }

        return new JsIfStatement(span, test, consequent, alternate);
    }

    /// <summary>Parses <c>with</c>, and refuses it where the language has no such statement.</summary>
    /// <remarks>
    /// <para>
    /// <b>Strict code has no <c>with</c> statement, and the refusal is an EARLY ERROR rather than a
    /// manifest refusal.</b> The two are different claims and a conformance runner scores them
    /// differently: <c>2104</c> says this profile declines a construct it could otherwise run, and
    /// would take every strict-mode <c>with</c> case out of both the pass and the fail column. The
    /// manifest admits <c>with</c>, so a program that writes one in strict code is wrong about the
    /// LANGUAGE — which is <c>2101</c>, exactly as a <c>super</c> property outside a method is.
    /// </para>
    /// <para>
    /// <b>A function body, a class body and a module are all strict</b>, and the parser already
    /// tracks that: a directive prologue sets it before the body's statements are read, a class body
    /// sets it before its heritage, the module goal sets it at the top, and a caller may impose it.
    /// So this test needs no walk of anything.
    /// </para>
    /// <para>
    /// <b>The body is a <c>Statement</c> and a declaration is not one.</b> <c>with (o) let x = 1;</c>
    /// and <c>with (o) function f() { }</c> are syntax errors in the language, and the reason they
    /// are refused here rather than lowered is not tidiness: a lexical declaration whose only
    /// enclosing scope is the object environment record would have nowhere to put its slot.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=061AA1
    // Broiler-Human:        PENDING
    private JsStatement ParseWith()
    {
        var span = Span();

        if (strict)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "the `with` statement is a syntax error in strict code");

            return new JsEmptyStatement(span);
        }

        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var target = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");

        if (Current.Kind is SliceTokenKind.Function or SliceTokenKind.Class or
                SliceTokenKind.Const ||
            (Current.Kind == SliceTokenKind.Let && BeginsLetDeclaration()))
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a declaration is not a statement, so it cannot be the body of a `with`");

            return new JsEmptyStatement(span);
        }

        return new JsWithStatement(span, target, ParseStatement());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5E3C87
    // Broiler-Human:        PENDING
    private JsStatement ParseWhile()
    {
        var span = Span();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");
        return new JsWhileStatement(span, test, ParseNestedStatement("a `while`"));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=00F97E
    // Broiler-Human:        PENDING
    private JsStatement ParseDoWhile()
    {
        var span = Span();
        Advance();
        var body = ParseNestedStatement("a `do`");
        Expect(SliceTokenKind.While, "while");
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }

        return new JsDoWhileStatement(span, body, test);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=74B58B
    // Broiler-Human:        PENDING
    private JsStatement ParseFor()
    {
        var span = Span();
        Advance();

        // `for await` IS READ HERE AND CARRIED TO THE `of`, and only an `of` head may have it. The
        // token is consumed before the parenthesis because after it the head reads as an ordinary
        // one; what the flag then decides is which iteration protocol the lowering drives.
        var isAwait = false;

        if (Current.Kind == SliceTokenKind.Await)
        {
            // AND IT IS AN EARLY ERROR OUTSIDE A BODY THAT MAY AWAIT, not a manifest refusal. The
            // manifest admits `for await`; a program that writes one in an ordinary function is
            // wrong about the LANGUAGE, exactly as a bare `await` there is - and the conformance
            // runner grades the two codes differently, so answering `2104` would take every
            // negative test for this out of both columns.
            if (!awaitIsOperator)
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`for await` is only admitted inside an async function or an async generator");

                return new JsEmptyStatement(span);
            }

            isAwait = true;
            Advance();
        }

        Expect(SliceTokenKind.OpenParen, "(");

        JsStatement? initialiser = null;

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Var or SliceTokenKind.Const ||
            (Current.Kind == SliceTokenKind.Let && BeginsLetDeclaration()))
        {
            var headSpan = Span();
            var kind = Current.Kind switch
            {
                SliceTokenKind.Var => SliceDeclarationKind.Var,
                SliceTokenKind.Let => SliceDeclarationKind.Let,
                _ => SliceDeclarationKind.Const,
            };

            Advance();
            var declarators = ParseDeclarators(noIn: true);
            ValidateBindingList(kind, declarators);

            if (Current.Kind is SliceTokenKind.In or SliceTokenKind.Of && declarators.Count == 1)
            {
                // THE `of` HEAD TAKES AN AssignmentExpression AND THE `in` HEAD TAKES AN
                // Expression, which is not a distinction anybody would guess: `for (x of a, b)` is
                // a syntax error and `for (x in a, b)` iterates the keys of `b`.
                var isOf = Current.Kind == SliceTokenKind.Of;

                // AN ENUMERATING HEAD DECLARES A BINDING AND DOES NOT INITIALISE IT. The value comes
                // from the object or the iterator, so `for (let x = 3 in o)` names a value that
                // nothing could ever read. The one form the language keeps is Annex B's, and it is
                // narrow: a `var` with a plain name, an `in` head, and sloppy code - which is the
                // exact shape the web was written with before the rule existed.
                if (declarators[0].Initialiser is not null &&
                    (kind != SliceDeclarationKind.Var || isOf || strict ||
                        declarators[0].Pattern is not null))
                {
                    Refuse(
                        declarators[0].Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "a `for … in` or `for … of` head declares its binding and does not " +
                            "initialise it");

                    return new JsEmptyStatement(span);
                }

                Advance();
                var source = isOf ? ParseAssignment() : ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");

                // AN `in` HEAD WITH `for await` IN FRONT OF IT IS NOT A LOOP THE LANGUAGE HAS.
                // The production is `for await ( … of … )` and nothing else, so a `for await (x in
                // o)` is refused rather than quietly iterating property names asynchronously - and
                // it is refused as a language error, because the manifest is not what forbids it.
                if (isAwait && !isOf)
                {
                    return AwaitOnlyIterates(span);
                }

                var iterated = ParseNestedStatement("a `for` loop");
                ValidateLoopHead(kind, declarators, iterated);

                return isOf
                    ? new JsForOfStatement(
                        span, kind, declarators[0].Name, declarators[0].Pattern, null, source,
                        iterated, isAwait)
                    : new JsForInStatement(
                        span, kind, declarators[0].Name, declarators[0].Pattern, null, source,
                        iterated);
            }

            initialiser = new JsVariableStatement(headSpan, kind, declarators);
            Expect(SliceTokenKind.Semicolon, ";");
        }
        else
        {
            var headSpan = Span();
            var expression = ParseExpression(noIn: true);

            if (Current.Kind is SliceTokenKind.In or SliceTokenKind.Of)
            {
                var isOf = Current.Kind == SliceTokenKind.Of;
                Advance();

                // A LITERAL IN A HEAD IS A PATTERN AND NOT A VALUE. `for ([a, b] of pairs)` reached
                // here as an array literal because nothing before the `of` could have told it
                // apart from one, so the reinterpretation happens where the `of` finally does.
                var pattern = expression is JsArrayLiteral or JsObjectLiteral
                    ? ToPattern(expression)
                    : null;

                var head = pattern is null ? expression : null;
                var source = isOf ? ParseAssignment() : ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");

                if (isAwait && !isOf)
                {
                    return AwaitOnlyIterates(span);
                }

                var iterated = ParseNestedStatement("a `for` loop");

                return isOf
                    ? new JsForOfStatement(
                        span, null, string.Empty, pattern, head, source, iterated, isAwait)
                    : new JsForInStatement(
                        span, null, string.Empty, pattern, head, source, iterated);
            }

            initialiser = new JsExpressionStatement(headSpan, expression);
            Expect(SliceTokenKind.Semicolon, ";");
        }

        JsExpression? test = null;

        if (Current.Kind != SliceTokenKind.Semicolon)
        {
            test = ParseExpression();
        }

        Expect(SliceTokenKind.Semicolon, ";");
        JsExpression? update = null;

        if (Current.Kind != SliceTokenKind.CloseParen)
        {
            update = ParseExpression();
        }

        Expect(SliceTokenKind.CloseParen, ")");

        // A THREE-PART HEAD WITH `for await` IN FRONT OF IT IS NOT A LOOP EITHER, for the same
        // reason, and it is the case a reader is likeliest to write by accident: `for await (let i
        // = 0; ; )` reads as an ordinary counted loop right up to the point where nothing awaits.
        if (isAwait)
        {
            return AwaitOnlyIterates(span);
        }

        var counted = ParseNestedStatement("a `for` loop");

        if (initialiser is JsVariableStatement declared)
        {
            ValidateLoopHead(declared.Kind, declared.Declarators, counted);
        }

        return new JsForStatement(span, initialiser, test, update, counted);
    }

    /// <summary>What a <c>for await</c> over a head that is not an <c>of</c> head is told.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0D2333
    // Broiler-Human:        PENDING
    private JsStatement AwaitOnlyIterates(SliceSourceSpan span)
    {
        Refuse(
            span,
            SliceSourceDiagnosticCode.UnexpectedToken,
            "`for await` is only a production with an `of` head");

        return new JsEmptyStatement(span);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BEAD2B
    // Broiler-Human:        PENDING
    private JsStatement ParseBreakOrContinue()
    {
        var span = Span();
        var isBreak = Current.Kind == SliceTokenKind.Break;
        Advance();
        var label = string.Empty;

        if (Current.Kind == SliceTokenKind.Identifier && !Current.PrecededByLineTerminator)
        {
            _ = RefuseEscapedReservedWord(Current);
            label = Current.RawText;
            Advance();
        }

        Semicolon();

        return isBreak
            ? new JsBreakStatement(span, label)
            : new JsContinueStatement(span, label);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3E18D6
    // Broiler-Human:        PENDING
    private JsStatement ParseReturn()
    {
        var span = Span();
        Advance();
        JsExpression? value = null;

        if (Current.Kind is not SliceTokenKind.Semicolon and not SliceTokenKind.CloseBrace and
            not SliceTokenKind.EndOfSource && !Current.PrecededByLineTerminator)
        {
            value = ParseExpression();
        }

        Semicolon();
        return new JsReturnStatement(span, value);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E48CC2
    // Broiler-Human:        PENDING
    private JsStatement ParseThrow()
    {
        var span = Span();
        Advance();

        if (Current.PrecededByLineTerminator)
        {
            Refuse(span, SliceSourceDiagnosticCode.UnexpectedToken, "`throw` needs an expression on its own line");
            return new JsEmptyStatement(span);
        }

        var value = ParseExpression();
        Semicolon();
        return new JsThrowStatement(span, value);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C759FB
    // Broiler-Human:        PENDING
    private JsStatement ParseTry()
    {
        var span = Span();
        Advance();
        var block = ParseBlock();
        var parameter = string.Empty;
        JsPattern? catchPattern = null;
        JsBlockStatement? handler = null;
        JsBlockStatement? finaliser = null;

        if (Current.Kind == SliceTokenKind.Catch)
        {
            Advance();

            if (Current.Kind == SliceTokenKind.OpenParen)
            {
                Advance();

                if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
                {
                    catchPattern = ParseBindingPattern();
                }
                else
                {
                    parameter = BindingName();
                }

                Expect(SliceTokenKind.CloseParen, ")");
            }

            handler = ParseBlock();
            ValidateCatchParameter(parameter, catchPattern, handler);
        }

        if (Current.Kind == SliceTokenKind.Finally)
        {
            Advance();
            finaliser = ParseBlock();
        }

        if (handler is null && finaliser is null)
        {
            Refuse(span, SliceSourceDiagnosticCode.ExpectedToken, "`try` needs a `catch` or a `finally`");
        }

        return new JsTryStatement(span, block, parameter, catchPattern, handler, finaliser);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D070D8
    // Broiler-Human:        PENDING
    private JsStatement ParseSwitch()
    {
        var span = Span();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var discriminant = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");
        Expect(SliceTokenKind.OpenBrace, "{");
        var clauses = new System.Collections.Generic.List<JsSwitchClause>();
        var sawDefault = false;

        while (Current.Kind is SliceTokenKind.Case or SliceTokenKind.Default && diagnostics.Count == 0)
        {
            var clauseSpan = Span();
            JsExpression? test = null;

            if (Current.Kind == SliceTokenKind.Case)
            {
                Advance();
                test = ParseExpression();
            }
            else
            {
                // ONE `switch` HAS AT MOST ONE `default`, and the grammar says so rather than the
                // semantics: a `CaseBlock` is case clauses, one optional default clause, and case
                // clauses again. A second one is a syntax error and not a clause that never runs.
                if (sawDefault)
                {
                    Refuse(
                        Span(),
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "a `switch` has at most one `default` clause");

                    break;
                }

                sawDefault = true;
                Advance();
            }

            Expect(SliceTokenKind.Colon, ":");
            var body = new System.Collections.Generic.List<JsStatement>();

            while (Current.Kind is not SliceTokenKind.Case and not SliceTokenKind.Default and
                not SliceTokenKind.CloseBrace and not SliceTokenKind.EndOfSource &&
                diagnostics.Count == 0)
            {
                body.Add(ParseStatement());
            }

            clauses.Add(new JsSwitchClause(clauseSpan, test, body));
        }

        Expect(SliceTokenKind.CloseBrace, "}");

        // EVERY CLAUSE OF ONE `switch` SHARES ONE BLOCK SCOPE, which is the whole reason this is
        // checked here and not clause by clause. The `CaseBlock` is the scope and a clause is not,
        // so `switch (x) { case 1: let a; case 2: let a; }` declares one name twice - and a check
        // that ran per clause would have found nothing wrong with either of them.
        var whole = new System.Collections.Generic.List<JsStatement>();

        foreach (var clause in clauses)
        {
            whole.AddRange(clause.Body);
        }

        ValidateBlockScope(whole);
        return new JsSwitchStatement(span, discriminant, clauses);
    }

    // ---- functions -----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=72103F
    // Broiler-Human:        PENDING
    private JsFunctionNode ParseFunctionRest(
        SliceSourceSpan span, bool declaration, bool isAsync = false)
    {
        var isGenerator = Current.Kind == SliceTokenKind.Star;

        if (isGenerator)
        {
            Advance();
        }

        // THE NAME HAS ITS OWN `[Yield]` CONTEXT, AND IT IS NOT THE BODY'S. A declaration's name
        // inherits the enclosing one, so `function* g() { function yield() {} }` is refused; an
        // ordinary function EXPRESSION's name is `[~Yield]`, so
        // `function* g() { (function yield() {}); }` is admitted in sloppy code and every engine
        // runs it; and a generator expression's name is `[+Yield]`, so
        // `function* g() { (function* yield() {}); }` is refused again. Three cases from one
        // expression, and reading the name under the body's context collapses all three into the
        // wrong one.
        // AND THE NAME'S `[Await]` CONTEXT IS THE SAME THREE CASES, one word further along. A
        // declaration's name inherits the enclosing context, an ordinary function expression's is
        // `[~Await]`, and an ASYNC function expression's is `[+Await]` - so
        // `async function f() { (async function await(){}); }` is refused and
        // `async function f() { (function await(){}); }` is admitted, which is what every engine
        // does and is the opposite of what "await is reserved inside an async function" suggests.
        var outerOperator = yieldIsOperator;
        var outerAwait = awaitIsOperator;
        yieldIsOperator = declaration ? outerOperator : isGenerator;
        awaitIsOperator = declaration ? outerAwait : isAsync;
        var name = string.Empty;

        if (IsIdentifierName(Current.Kind))
        {
            _ = RefuseEscapedReservedWord(Current);
            name = Current.RawText;
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Yield or SliceTokenKind.Await)
        {
            // A RESERVED WORD WHERE A NAME BELONGS IS A RESERVED WORD, not a missing name. The two
            // answers carry different diagnostic codes, and the conformance runner grades on the
            // code: "this declaration needs a name" for `function* g() { function yield() {} }`
            // sends a reader looking for a name that is right there in front of them.
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.ReservedWordAsBinding,
                "`" + Current.RawText + "` is not a binding name");

            Advance();
        }
        else if (declaration)
        {
            Refuse(span, SliceSourceDiagnosticCode.ExpectedToken, "a function declaration needs a name");
        }

        yieldIsOperator = isGenerator;
        awaitIsOperator = isAsync;
        var body = ParseFunctionBody(span, name, ParseParameters(), isArrow: false, isGenerator, isAsync);
        yieldIsOperator = outerOperator;
        awaitIsOperator = outerAwait;
        return body;
    }

    /// <summary>
    /// Parses a formal parameter list in a <c>[~Await]</c> context, whatever encloses it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every parameter list that is not an ASYNC function's own goes through this, and the
    /// reason is not tidiness.</b> A method, an accessor and an ordinary arrow written inside an
    /// async function all take their parameters outside the async context - the language says so
    /// for the first two and makes an <c>await</c> in the third an early error - and a parser that
    /// let the flag leak would have parsed <c>await x</c> there as the operator. The tree would
    /// then carry an await expression belonging to a unit with no async flag, the lowering would
    /// emit the instruction, and THE VERIFIER WOULD REFUSE AN ARTIFACT THIS FRONT END HAD JUST
    /// PRODUCED - which is the failure shape roadmap section 3.4 records as the worst kind, because
    /// it is discovered by a workload rather than by a diagnostic.
    /// </para>
    /// <para>
    /// What a program written that way is told instead is the syntax error two identifiers in a row
    /// gives, which is the category the language puts it in.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3D4BD5
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsParameter> ParseOrdinaryParameters()
    {
        var outerAwait = awaitIsOperator;
        awaitIsOperator = false;

        try
        {
            return ParseParameters();
        }
        finally
        {
            awaitIsOperator = outerAwait;
        }
    }

    /// <summary>Parses a formal parameter list, defaults, patterns and a rest parameter included.</summary>
    /// <remarks>
    /// <b>A rest parameter ends the list</b>, and breaking rather than looping again is what makes
    /// <c>f(...a, b)</c> answer "`)` was expected" at <c>,</c> instead of silently accepting a
    /// parameter after the one that takes everything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=202F04
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsParameter> ParseParameters()
    {
        var parameters = new System.Collections.Generic.List<JsParameter>();
        Expect(SliceTokenKind.OpenParen, "(");

        while (Current.Kind != SliceTokenKind.CloseParen &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            var span = Span();

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                parameters.Add(new JsParameter(span, ParseBindingTarget(), null, IsRest: true));
                break;
            }

            var target = ParseBindingTarget();
            JsExpression? initialiser = null;

            if (Current.Kind == SliceTokenKind.Equals)
            {
                Advance();
                initialiser = ParseAssignment();
            }

            parameters.Add(new JsParameter(span, target, initialiser, IsRest: false));

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseParen, ")");
        return parameters;
    }

    /// <summary>Parses one binding pattern: <c>[…]</c> or <c>{…}</c> where a NAME is bound.</summary>
    /// <remarks>
    /// <b>This is the declaration half of destructuring and its leaves are names, not
    /// references.</b> <c>var [o.x] = y</c> is a syntax error while <c>[o.x] = y</c> is an ordinary
    /// assignment, and the difference is which of the two entry points the pattern came through:
    /// this one, or the reinterpretation in <see cref="ToPattern"/>. Sharing the tree and splitting
    /// the entry points is what keeps that rule in one place each.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=810915
    // Broiler-Human:        PENDING
    private JsPattern ParseBindingPattern()
    {
        var span = Span();

        // A PATTERN NESTS WITHOUT PASSING THROUGH ParseAssignment, so the depth guard every other
        // recursive production borrows from `Enter` does not cover it. `var [[[[…]]]] = x` would
        // recurse here until the parser's own stack ran out, which is a process termination rather
        // than a refusal.
        if (!EnterNesting())
        {
            return new JsTargetPattern(span, new JsIdentifier(span, "#invalid"));
        }

        try
        {
            return Current.Kind == SliceTokenKind.OpenBracket
                ? ParseArrayBindingPattern(span)
                : ParseObjectBindingPattern(span);
        }
        finally
        {
            depth--;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A55543
    // Broiler-Human:        PENDING
    private JsPattern ParseArrayBindingPattern(SliceSourceSpan span)
    {
        Expect(SliceTokenKind.OpenBracket, "[");
        var elements = new System.Collections.Generic.List<JsPatternElement?>();
        JsPattern? rest = null;

        while (Current.Kind != SliceTokenKind.CloseBracket &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.Comma)
            {
                Advance();
                elements.Add(null);
                continue;
            }

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                rest = ParseBindingTarget();
                break;
            }

            elements.Add(ParseBindingElement());

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBracket, "]");
        return new JsArrayPattern(span, elements, rest);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2D455B
    // Broiler-Human:        PENDING
    private JsPattern ParseObjectBindingPattern(SliceSourceSpan span)
    {
        Expect(SliceTokenKind.OpenBrace, "{");
        var properties = new System.Collections.Generic.List<JsPatternProperty>();
        JsPattern? rest = null;

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                rest = ParseBindingTarget();
                break;
            }

            var entrySpan = Span();
            var keyToken = Current;
            var key = PropertyKey(out var computed);

            if (Current.Kind == SliceTokenKind.Colon)
            {
                Advance();
                properties.Add(new JsPatternProperty(entrySpan, key, computed, ParseBindingElement()));
            }
            else
            {
                if (computed is not null ||
                    keyToken.Kind is SliceTokenKind.StringLiteral or SliceTokenKind.NumericLiteral)
                {
                    Refuse(
                        entrySpan,
                        SliceSourceDiagnosticCode.ExpectedToken,
                        "`:` was expected and `" + Describe(Current) + "` was found");
                }
                else if (IsIdentifierName(keyToken.Kind))
                {
                    // A SHORTHAND'S KEY IS AN Identifier AND NOT AN IdentifierName, so it answers
                    // for an escaped reserved word the way every other binding position does.
                    _ = RefuseEscapedReservedWord(keyToken);
                }
                else
                {
                    // A SHORTHAND'S KEY IS ALSO ITS BINDING NAME, so a word this goal or this
                    // strictness reserves is answered for as a reserved word - the same answer
                    // `BindingName` gives one step away - and not as a missing colon. The
                    // difference is not cosmetic: `"use strict"; var { yield } = {};` is a test
                    // about the reservation, and a missing-colon diagnostic scores it as a failure
                    // rather than as the syntax error it asked for.
                    Refuse(
                        entrySpan,
                        SliceSourceDiagnosticCode.ReservedWordAsBinding,
                        "`" + Describe(keyToken) + "` is not a binding name");
                }

                JsExpression? initialiser = null;

                if (Current.Kind == SliceTokenKind.Equals)
                {
                    Advance();
                    initialiser = ParseAssignment();
                }

                properties.Add(new JsPatternProperty(
                    entrySpan,
                    key,
                    null,
                    new JsPatternElement(
                        entrySpan,
                        new JsTargetPattern(entrySpan, new JsIdentifier(entrySpan, key)),
                        initialiser)));
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        return new JsObjectPattern(span, properties, rest);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7732DD
    // Broiler-Human:        PENDING
    private JsPatternElement ParseBindingElement()
    {
        var span = Span();
        var target = ParseBindingTarget();
        JsExpression? initialiser = null;

        if (Current.Kind == SliceTokenKind.Equals)
        {
            Advance();
            initialiser = ParseAssignment();
        }

        return new JsPatternElement(span, target, initialiser);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F338BC
    // Broiler-Human:        PENDING
    private JsPattern ParseBindingTarget()
    {
        var span = Span();

        return Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace
            ? ParseBindingPattern()
            : new JsTargetPattern(span, new JsIdentifier(span, BindingName()));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9C328C
    // Broiler-Human:        PENDING
    private JsFunctionNode ParseFunctionBody(
        SliceSourceSpan span,
        string name,
        System.Collections.Generic.List<JsParameter> parameters,
        bool isArrow,
        bool isGenerator = false,
        bool isAsync = false,
        bool uniqueParameters = false)
    {
        var outer = strict;

        // EVERY BODY DECIDES ITS OWN `[Await]` CONTEXT AND NONE OF THEM INHERITS ONE. A method, an
        // accessor, a generator method, an ordinary nested function and an ordinary nested arrow
        // are all `[~Await]` however deeply an async function encloses them, so the flag is set
        // from this body's own kind here rather than left at whatever the enclosing parse had.
        // Without this, `async function f(){ class C { m(){ await 1; } } }` would have parsed
        // `await` as the operator and lowered an `Await` instruction into a unit carrying no async
        // flag - which the verifier refuses, so the front end would have been producing artifacts
        // this host then rejected.
        var outerAwait = awaitIsOperator;
        awaitIsOperator = isAsync;
        Expect(SliceTokenKind.OpenBrace, "{");
        var directives = ParseDirectives();
        var body = new System.Collections.Generic.List<JsStatement>();

        if (!isArrow)
        {
            functionDepth++;
        }

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        if (!isArrow)
        {
            functionDepth--;
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        var inner = strict;

        // THE BODY IS CHECKED WHILE ITS OWN STRICTNESS IS STILL IN FORCE. Nothing in the check below
        // reads it today, because a function declaration at the top of a body is a `var` name in
        // both modes and the Annex B relaxation has nothing to relax there - but the strictness of
        // the code being judged is the strictness of the code it was written in, and restoring the
        // caller's first would have made that accidental rather than stated. The parameter rules DO
        // read it, and read it for the same reason: `function f(a, a) { "use strict"; }` is refused
        // by a directive that has only just been seen.
        ValidateParameters(parameters, uniqueParameters, inner, DeclaresUseStrict(directives));
        ValidateVarScope(body, parameters);
        strict = outer;
        awaitIsOperator = outerAwait;

        return new JsFunctionNode(
            span, name, parameters, body, isArrow, inner, directives, isGenerator, isAsync);
    }

    // ---- classes -------------------------------------------------------------------------------

    /// <summary>Parses a class, in either of the two forms that share this body.</summary>
    /// <remarks>
    /// <para>
    /// <b>A class body is strict code whether or not anything around it is</b>, and the strictness
    /// has to be set HERE rather than in the lowering because it changes the grammar: inside a
    /// class body <c>yield</c> is a reserved word and a legacy octal literal is a syntax error,
    /// and both are early errors that a lowering never gets to see because the parse already
    /// succeeded. It is set before the heritage as well as the body, which is what the
    /// specification's <c>ClassTail</c> covers.
    /// </para>
    /// <para>
    /// The heritage is a <c>LeftHandSideExpression</c> and not an assignment expression, which is
    /// why <c>class D extends a.b() { }</c> parses and <c>class D extends a = b { }</c> does not.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C0F049
    // Broiler-Human:        PENDING
    private JsClassNode ParseClass(SliceSourceSpan span, bool declaration)
    {
        Expect(SliceTokenKind.Class, "class");
        var outer = strict;
        strict = true;
        var name = string.Empty;

        if (IsIdentifierName(Current.Kind))
        {
            _ = RefuseEscapedReservedWord(Current);
            name = Current.RawText;
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Yield or SliceTokenKind.Await)
        {
            // A RESERVED WORD WHERE A NAME BELONGS IS A RESERVED WORD, the same answer the
            // function path already gives. `class await {}` inside an async function reached the
            // arm below and was told a class declaration needs a name, which sends a reader
            // looking for a name that is right there - and the conformance runner grades on the
            // CODE, so the two answers are not interchangeable. The class body is strict code, so
            // `yield` is always in this case here and `await` is in it whenever an async function
            // or the module goal encloses the class.
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.ReservedWordAsBinding,
                "`" + Current.RawText + "` is not a binding name");

            Advance();
        }
        else if (declaration)
        {
            Refuse(span, SliceSourceDiagnosticCode.ExpectedToken, "a class declaration needs a name");
        }

        JsExpression? heritage = null;
        var hasHeritage = false;

        if (Current.Kind == SliceTokenKind.Extends)
        {
            Advance();
            hasHeritage = true;
            heritage = ParseCallChain();
        }

        Expect(SliceTokenKind.OpenBrace, "{");
        var members = new System.Collections.Generic.List<JsClassMember>();

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            // A LONE SEMICOLON IS A CLASS ELEMENT AND DEFINES NOTHING, which is why it is skipped
            // here rather than refused as an empty field.
            if (Current.Kind == SliceTokenKind.Semicolon)
            {
                Advance();
                continue;
            }

            if (ParseClassMember() is { } member)
            {
                members.Add(member);
            }
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        strict = outer;
        ValidateClassBody(members);
        return new JsClassNode(span, name, heritage, hasHeritage, members);
    }

    /// <summary>Rules on the names a class body may not give its elements.</summary>
    /// <remarks>
    /// <para>
    /// <b>Every rule here is about a name the class DEFINITION itself creates or needs</b>, which
    /// is why they are one check rather than four. <c>constructor</c> is the class's own
    /// constructor and a body may name a member that only as a plain non-static method — a getter,
    /// a generator, an async method or a field of that name would define over the thing being
    /// built. <c>prototype</c> is defined on the constructor by the definition, so a static member
    /// of that name would be defining it twice.
    /// </para>
    /// <para>
    /// <b>A private name may be declared twice only as the two halves of one accessor</b>, and only
    /// when both halves are on the same side of <c>static</c>. Two fields of one name would mint one
    /// private name and install two elements under it; a getter and a setter mint one name and
    /// install one element with two functions, which is the case the language admits and the only
    /// one.
    /// </para>
    /// <para>
    /// <b>They are checked after the body is parsed rather than as each member arrives</b>, because
    /// the duplicate rule needs the whole body: a getter is legal until a second getter appears.
    /// </para>
    /// <para>
    /// <b>All but one take the DUPLICATE code rather than a code of their own</b>, and it is the
    /// accurate one rather than the nearest one: each is a second declaration of a name something
    /// has already declared - the class definition for <c>constructor</c> and <c>prototype</c>, the
    /// body itself for a repeated private name. <c>#constructor</c> is the exception and takes the
    /// reserved-name code, because nothing declared it first: it is a private name a class body may
    /// not spell at all.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=893659
    // Broiler-Human:        PENDING
    private void ValidateClassBody(System.Collections.Generic.List<JsClassMember> members)
    {
        var seen =
            new System.Collections.Generic.Dictionary<string, (JsMethodKind Kind, bool Static)>(
                System.StringComparer.Ordinal);

        foreach (var member in members)
        {
            if (member.Kind == JsMethodKind.StaticBlock)
            {
                continue;
            }

            if (member.IsPrivate)
            {
                ValidatePrivateElement(member, seen);
                continue;
            }

            if (member.Computed is not null)
            {
                continue;
            }

            if (member.IsStatic)
            {
                if (string.Equals(member.Key, "prototype", System.StringComparison.Ordinal))
                {
                    Refuse(
                        member.Span,
                        SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                        "the class definition declares `prototype` on the constructor, so a static " +
                            "member cannot declare it again");

                    continue;
                }

                // A STATIC FIELD MAY NOT BE NAMED `constructor` EITHER, and a static METHOD may.
                // `static constructor() {}` is an ordinary method of the constructor object that
                // happens to be called that, so the arm above skipped every static member and let
                // `static constructor;` through - a field of the name the class definition owns.
                if (member.Kind != JsMethodKind.Field ||
                    !string.Equals(member.Key, "constructor", System.StringComparison.Ordinal))
                {
                    continue;
                }

                Refuse(
                    member.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "the class definition declares `constructor`, so a field cannot declare it " +
                        "again");

                continue;
            }

            if (!string.Equals(member.Key, "constructor", System.StringComparison.Ordinal))
            {
                continue;
            }

            if (member.Kind != JsMethodKind.Method ||
                member.Function is { } body && (body.IsGenerator || body.IsAsync))
            {
                Refuse(
                    member.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "the class definition declares `constructor`, so a field, an accessor, a " +
                        "generator or an async method cannot declare it again");
            }
        }
    }

    /// <summary>Rules on one private element against the ones already declared.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D76607
    // Broiler-Human:        PENDING
    private void ValidatePrivateElement(
        JsClassMember member,
        System.Collections.Generic.Dictionary<string, (JsMethodKind Kind, bool Static)> seen)
    {
        if (string.Equals(member.Key, "#constructor", System.StringComparison.Ordinal))
        {
            Refuse(
                member.Span,
                SliceSourceDiagnosticCode.ReservedWordAsBinding,
                "`#constructor` is a private name a class body reserves and cannot declare");

            return;
        }

        // THE KEY IS THE BARE NAME AND `static` IS NOT PART OF IT. One class body declares ONE
        // private name per spelling however its elements are spread across `static`, because the
        // name lives in the class's scope and not on either object - so `#m` and `static #m` are a
        // duplicate, and the accessor pair below has to agree about `static` rather than being
        // allowed to straddle it. Keying on the pair instead let `get #a` and `static set #a`
        // through, which would have minted one name and installed its two halves on two different
        // objects.
        if (!seen.TryGetValue(member.Key, out var already))
        {
            seen[member.Key] = (member.Kind, member.IsStatic);
            return;
        }

        if (already.Static == member.IsStatic &&
            (already.Kind == JsMethodKind.Get && member.Kind == JsMethodKind.Set ||
                already.Kind == JsMethodKind.Set && member.Kind == JsMethodKind.Get))
        {
            seen[member.Key] = (JsMethodKind.Method, member.IsStatic);
            return;
        }

        Refuse(
            member.Span,
            SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
            "`" + member.Key + "` is declared more than once by this class body");
    }

    /// <summary>Parses one class element, or refuses one this manifest does not admit.</summary>
    /// <remarks>
    /// <b>Every modifier is settled before the key is read, and the discriminator is the token
    /// AFTER it.</b> <c>static</c>, <c>get</c>, <c>set</c> and <c>async</c> are all legal member
    /// names as well as modifiers, so <c>static() { }</c> is a method called <c>static</c> and
    /// <c>static m() { }</c> is a static method - and reading the key first would have made the
    /// second one a field called <c>static</c> followed by a surprise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C1DE41
    // Broiler-Human:        PENDING
    private JsClassMember? ParseClassMember()
    {
        var span = Span();
        var isStatic = false;

        if (Current.Kind == SliceTokenKind.Static && !IsMemberNameEnd(Peek(1).Kind))
        {
            if (Peek(1).Kind == SliceTokenKind.OpenBrace)
            {
                Advance();
                return ParseStaticBlock(span);
            }

            isStatic = true;
            Advance();
        }

        var isGenerator = false;
        var isAsync = false;

        if (Current.Kind == SliceTokenKind.Async &&
            !Peek(1).PrecededByLineTerminator &&
            !IsMemberNameEnd(Peek(1).Kind) &&
            Peek(1).Kind != SliceTokenKind.Equals)
        {
            isAsync = true;
            Advance();
        }

        if (Current.Kind == SliceTokenKind.Star)
        {
            // THE TWO MODIFIERS COMBINE, AND THE ONE THAT REFUSED THE COMBINATION IS GONE. An
            // `async *m() {}` member sets both bits and reaches the same body parse every other
            // member does; what it changes is the `[Yield]` and `[Await]` contexts of the parameter
            // list and the body, which are set from the two bits a few lines below.
            isGenerator = true;
            Advance();
        }

        var kind = JsMethodKind.Method;

        // `get` AND `set` ARE MODIFIERS ONLY BEFORE A PROPERTY NAME, and `*` is not one. `class C {
        // get\n*a(){} }` declares a FIELD named `get` and then a generator method - which is what
        // the grammar says and what every engine does - so testing only for the tokens that end a
        // member name made the `*` a surprise inside an accessor that had no parameter list.
        if (!isAsync && !isGenerator &&
            Current.Kind is SliceTokenKind.Get or SliceTokenKind.Set &&
            !IsMemberNameEnd(Peek(1).Kind) &&
            Peek(1).Kind != SliceTokenKind.Star)
        {
            kind = Current.Kind == SliceTokenKind.Get ? JsMethodKind.Get : JsMethodKind.Set;
            Advance();
        }

        var isPrivate = IsPrivateName(Current);
        string key;
        JsExpression? computed = null;

        if (isPrivate)
        {
            key = Current.RawText;
            Advance();
        }
        else
        {
            key = PropertyKey(out computed);
        }

        // A CLASS FIELD IS EVERY MEMBER THAT IS NOT FOLLOWED BY A PARAMETER LIST, and naming it
        // that way covers `x = 1`, a bare `x`, and `x` followed by a newline in one answer. The
        // alternative - deciding on the `=` alone - would have let a bare field come back as a
        // missing `(`, which names the punctuation rather than the construct.
        if (Current.Kind != SliceTokenKind.OpenParen)
        {
            // A MODIFIER WITH NO PARAMETER LIST AFTER IT IS NOT A FIELD, it is a malformed method.
            // `class C { *x = 1 }` has a `*` that only a method may carry, and calling the result a
            // field would define one named `x` and silently drop the `*` the source wrote.
            if (isGenerator || isAsync || kind != JsMethodKind.Method)
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.ExpectedToken,
                    "a class element with a `*`, `async`, `get` or `set` modifier needs a " +
                        "parameter list");

                return null;
            }

            return ParseFieldTail(span, isStatic, key, computed, isPrivate);
        }

        var outerOperator = yieldIsOperator;
        var outerAwait = awaitIsOperator;
        yieldIsOperator = isGenerator;
        awaitIsOperator = isAsync;

        // A METHOD'S PARAMETER LIST IS THE ORDINARY ONE AND AN ACCESSOR'S IS NOT OPTIONAL ABOUT IT:
        // both paths go through the same parse, and which parameter forms a member admits is the
        // static-semantics pass's question rather than this one's.
        var parameters = ParseParameters();

        if (kind is JsMethodKind.Get or JsMethodKind.Set)
        {
            ValidateAccessorParameters(kind == JsMethodKind.Get, parameters, span);
        }

        var body = ParseFunctionBody(
            span, key, parameters, isArrow: false, isGenerator, isAsync, uniqueParameters: true);

        yieldIsOperator = outerOperator;
        awaitIsOperator = outerAwait;
        return new JsClassMember(span, kind, isStatic, key, computed, isPrivate, body);
    }

    /// <summary>Parses the tail of a field, from just after its key.</summary>
    /// <remarks>
    /// <b>A field's initialiser is an <c>AssignmentExpression</c> and not an expression</b>, so the
    /// comma in <c>class C { x = 1, y = 2 }</c> is not a sequence operator joining two initialisers:
    /// it is the token after a field, and the language has no class element separator spelled that
    /// way. Parsing the wider production here would have accepted a body no engine accepts.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C98C95
    // Broiler-Human:        PENDING
    private JsClassMember ParseFieldTail(
        SliceSourceSpan span, bool isStatic, string key, JsExpression? computed, bool isPrivate)
    {
        JsFunctionNode? initialiser = null;

        if (Current.Kind == SliceTokenKind.Equals)
        {
            Advance();

            // `arguments` AND `yield` ARE NOT WRITABLE IN AN INITIALISER, and `await` is not either.
            // The initialiser is its own function body in the specification, so a `yield` inside one
            // is a `yield` outside any generator however the class was reached; the two flags are
            // cleared here rather than in the lowering because it is the PARSE that decides whether
            // the word is an operator.
            var outerOperator = yieldIsOperator;
            var outerAwait = awaitIsOperator;
            yieldIsOperator = false;
            awaitIsOperator = false;
            var value = ParseAssignment();
            yieldIsOperator = outerOperator;
            awaitIsOperator = outerAwait;

            initialiser = new JsFunctionNode(
                span,
                key,
                System.Array.Empty<JsParameter>(),
                [new JsReturnStatement(span, value)],
                IsArrow: false,
                IsStrict: true,
                System.Array.Empty<JsStringLiteral>());
        }

        ConsumeFieldTerminator();
        return new JsClassMember(
            span, JsMethodKind.Field, isStatic, key, computed, isPrivate, initialiser);
    }

    /// <summary>Parses <c>static { … }</c>, from just after the <c>static</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1A0E82
    // Broiler-Human:        PENDING
    private JsClassMember ParseStaticBlock(SliceSourceSpan span)
    {
        // A STATIC BLOCK IS NOT A GENERATOR, NOT ASYNC, AND HAS NO ARGUMENTS OBJECT, and all three
        // are decided here: `yield` and `await` are ordinary identifiers nowhere inside it, because
        // the block is its own function body and the specification makes both of them Syntax Errors
        // there. Leaving the enclosing flags alone would have let `class C { static { await x; } }`
        // inside an async function parse as an await the block cannot perform.
        var outerOperator = yieldIsOperator;
        var outerAwait = awaitIsOperator;
        yieldIsOperator = false;
        awaitIsOperator = false;

        var body = ParseFunctionBody(
            span, string.Empty, new System.Collections.Generic.List<JsParameter>(), isArrow: false);

        yieldIsOperator = outerOperator;
        awaitIsOperator = outerAwait;

        return new JsClassMember(
            span, JsMethodKind.StaticBlock, IsStatic: true, string.Empty, null, IsPrivate: false,
            body);
    }

    /// <summary>Reads the token that ends a field, which is usually nothing at all.</summary>
    /// <remarks>
    /// <b>A field is terminated the way a statement is, by automatic semicolon insertion</b>, so
    /// <c>class C { x = 1 }</c> and <c>class C { x = 1\ny = 2 }</c> both end their fields with no
    /// token. Demanding a semicolon would have refused most of the class bodies anybody writes;
    /// accepting anything at all would have made <c>class C { x = 1 y = 2 }</c> - two fields on one
    /// line with nothing between them - a body this host admits and no other does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=50E3D9
    // Broiler-Human:        PENDING
    private void ConsumeFieldTerminator()
    {
        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
            return;
        }

        if (Current.Kind is SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource ||
            Current.PrecededByLineTerminator)
        {
            return;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ExpectedToken,
            "a class field ends with `;`, a line break or `}`");
    }


    /// <summary>
    /// Whether a token can only follow a member NAME, so that the word before it was the name.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D515C9
    // Broiler-Human:        PENDING
    private static bool IsMemberNameEnd(SliceTokenKind kind) =>
        kind is SliceTokenKind.OpenParen or SliceTokenKind.Equals or SliceTokenKind.Semicolon or
            SliceTokenKind.CloseBrace;

    /// <summary>Parses what follows <c>super</c>, which is never nothing.</summary>
    /// <remarks>
    /// <c>super</c> alone is not an expression: the grammar admits it only as the target of a
    /// property access or as the callee of a call, and refusing anything else HERE is what keeps
    /// <c>super + 1</c> from becoming a value this surface would then have to have a meaning for.
    /// Whether the position admits it at all - a method for a property, a derived constructor for a
    /// call - is decided by the lowering, which is the pass that knows what it is inside of.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9F1EC2
    // Broiler-Human:        PENDING
    private JsExpression ParseSuper(SliceSourceSpan span)
    {
        Advance();

        switch (Current.Kind)
        {
            case SliceTokenKind.OpenParen:
                return new JsSuperCallExpression(span, ParseArguments());

            case SliceTokenKind.Dot:
                Advance();

                // `super.#x` HAS NO READING AND IS NOT A PRIVATE ACCESS ON THE PROTOTYPE. A private
                // element belongs to an object and `super` names no object - it names where a
                // lookup starts - so the grammar admits an ordinary property name after it and
                // nothing else. Left to `MemberName` the `#x` would have become a property called
                // `#x` on the parent, which the source did not ask for.
                if (IsPrivateName(Current))
                {
                    Refuse(
                        Span(),
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`super` has no private elements of its own");

                    Advance();
                    return new JsNullLiteral(span);
                }

                return new JsSuperMemberExpression(span, MemberName(), null);

            case SliceTokenKind.OpenBracket:
            {
                Advance();
                var key = ParseExpression();
                Expect(SliceTokenKind.CloseBracket, "]");
                return new JsSuperMemberExpression(span, string.Empty, key);
            }

            default:
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`super` is followed by `.`, `[` or `(` and by nothing else");

                return new JsNullLiteral(span);
        }
    }

    // ---- the early errors about declared names --------------------------------------------------

    /// <summary>One name a scope declares, where the source declared it, and by what.</summary>
    /// <param name="Name">The bound name.</param>
    /// <param name="Span">Where a collision this name is half of is reported.</param>
    /// <param name="ByFunction">
    /// <b>Whether a FUNCTION DECLARATION bound it</b>, which is the one fact the duplicate rule needs
    /// and the only one it cannot recover from the name. Sloppy code may declare one function twice
    /// in a block and may not declare anything else twice there, so a pair of colliding entries has
    /// to remember what declared each of them.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6B14D5
    // Broiler-Human:        PENDING
    private readonly record struct JsDeclaredName(
        string Name, SliceSourceSpan Span, bool ByFunction);

    /// <summary>Every name a statement list declares LEXICALLY, in source order.</summary>
    /// <remarks>
    /// <para>
    /// <b>A function declaration is lexical in a BLOCK and var-scoped at the TOP of a body, and that
    /// one difference is the whole of <paramref name="topLevel"/>.</b> It is why
    /// <c>function f() {} function f() {}</c> is a program at a script's top level and
    /// <c>{ function f() {} let f; }</c> is not, and why <c>function f() {} let f;</c> is refused by
    /// the var rule rather than by the duplicate rule.
    /// </para>
    /// <para>
    /// <b>It descends into nothing but a label.</b> A block, a loop body and a clause each open a
    /// scope of their own and are checked when THEY close; a label opens none, so
    /// <c>{ label: function f() {} let f; }</c> is a duplicate and every engine says so.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9A2BA2
    // Broiler-Human:        PENDING
    private static void LexicallyDeclaredNames(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        bool topLevel,
        System.Collections.Generic.List<JsDeclaredName> into)
    {
        foreach (var statement in body)
        {
            switch (statement)
            {
                case JsVariableStatement variable when variable.Kind != SliceDeclarationKind.Var:
                    foreach (var declarator in variable.Declarators)
                    {
                        DeclaratorNames(declarator, into);
                    }

                    break;

                case JsClassDeclaration declared when declared.Class.Name.Length != 0:
                    into.Add(new JsDeclaredName(declared.Class.Name, declared.Span, false));
                    break;

                // A GENERATOR AND AN ASYNC FUNCTION ARE LEXICAL NAMES THAT ANNEX B DOES NOT COVER.
                // The web-compatibility relaxation below is written for `FunctionDeclaration` and
                // for nothing else, so `{ function* f() {} function* f() {} }` is refused where
                // `{ function f() {} function f() {} }` is not - which is why the flag records the
                // PLAIN form rather than "a function of some kind declared it".
                case JsFunctionDeclaration declaration
                    when !topLevel && declaration.Function.Name.Length != 0:
                    into.Add(new JsDeclaredName(
                        declaration.Function.Name,
                        declaration.Span,
                        !declaration.Function.IsGenerator && !declaration.Function.IsAsync));

                    break;

                case JsLabelledStatement labelled:
                    LexicallyDeclaredNames([labelled.Body], topLevel, into);
                    break;

                // AN EXPORTED DECLARATION IS A DECLARATION OF THE MODULE, and the `export` in front
                // of it changes what the name is published as rather than where it is bound.
                case JsExportDeclaration { Declaration: { } exported }:
                    LexicallyDeclaredNames([exported], topLevel, into);
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Every name a statement list declares with <c>var</c>, in source order.</summary>
    /// <remarks>
    /// <para>
    /// <b>It descends through every statement and stops at every FUNCTION</b>, because a <c>var</c>
    /// anywhere inside a body belongs to that body and a <c>var</c> inside a nested function belongs
    /// to the nested one. That is what makes <c>{ { var f; } let f; }</c> a collision and
    /// <c>{ function g() { var f; } let f; }</c> a program.
    /// </para>
    /// <para>
    /// <b>It is not <see cref="JsCompiler"/>'s walk of the same shape and must not become it.</b>
    /// That one collects what a hoisting prologue has to declare, so it takes a function declaration
    /// at EVERY level, which is the web-compatibility behaviour of a block-level function. This one
    /// answers a question about the grammar, where a block-level function declaration is a lexical
    /// name and not a var one - and folding the two together would refuse
    /// <c>{ function f() {} } var f;</c>, which every engine runs.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4D47F3
    // Broiler-Human:        PENDING
    private static void VarDeclaredNames(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        bool topLevel,
        System.Collections.Generic.List<JsDeclaredName> into)
    {
        foreach (var statement in body)
        {
            switch (statement)
            {
                case JsVariableStatement variable when variable.Kind == SliceDeclarationKind.Var:
                    foreach (var declarator in variable.Declarators)
                    {
                        DeclaratorNames(declarator, into);
                    }

                    break;

                case JsFunctionDeclaration declaration
                    when topLevel && declaration.Function.Name.Length != 0:
                    into.Add(new JsDeclaredName(declaration.Function.Name, declaration.Span, true));
                    break;

                // A LABEL CARRIES THE TOP-LEVEL-NESS THROUGH IT and every other statement ends it,
                // which is why this one arm passes `topLevel` on and the rest pass `false`.
                case JsLabelledStatement labelled:
                    VarDeclaredNames([labelled.Body], topLevel, into);
                    break;

                case JsBlockStatement block:
                    VarDeclaredNames(block.Body, false, into);
                    break;

                case JsIfStatement conditional:
                    VarDeclaredNames([conditional.Consequent], false, into);

                    if (conditional.Alternate is not null)
                    {
                        VarDeclaredNames([conditional.Alternate], false, into);
                    }

                    break;

                case JsWhileStatement loop:
                    VarDeclaredNames([loop.Body], false, into);
                    break;

                case JsDoWhileStatement loop:
                    VarDeclaredNames([loop.Body], false, into);
                    break;

                case JsWithStatement scoped:
                    VarDeclaredNames([scoped.Body], false, into);
                    break;

                case JsForStatement loop:
                    if (loop.Initialiser is not null)
                    {
                        VarDeclaredNames([loop.Initialiser], false, into);
                    }

                    VarDeclaredNames([loop.Body], false, into);
                    break;

                case JsForInStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(loop.Name, loop.Pattern, loop.Span, into);
                    }

                    VarDeclaredNames([loop.Body], false, into);
                    break;

                case JsForOfStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        HeadNames(loop.Name, loop.Pattern, loop.Span, into);
                    }

                    VarDeclaredNames([loop.Body], false, into);
                    break;

                case JsTryStatement guarded:
                    VarDeclaredNames(guarded.Block.Body, false, into);

                    if (guarded.Handler is not null)
                    {
                        VarDeclaredNames(guarded.Handler.Body, false, into);
                    }

                    if (guarded.Finaliser is not null)
                    {
                        VarDeclaredNames(guarded.Finaliser.Body, false, into);
                    }

                    break;

                case JsSwitchStatement switched:
                    foreach (var clause in switched.Clauses)
                    {
                        VarDeclaredNames(clause.Body, false, into);
                    }

                    break;

                case JsExportDeclaration { Declaration: { } exported }:
                    VarDeclaredNames([exported], topLevel, into);
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>The names one declarator binds, whether it names one or destructures.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=748E65
    // Broiler-Human:        PENDING
    private static void DeclaratorNames(
        JsDeclarator declarator, System.Collections.Generic.List<JsDeclaredName> into) =>
        HeadNames(declarator.Name, declarator.Pattern, declarator.Span, into);

    /// <summary>The names a loop head or a declarator binds, given its two possible shapes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6400A7
    // Broiler-Human:        PENDING
    private static void HeadNames(
        string name,
        JsPattern? pattern,
        SliceSourceSpan span,
        System.Collections.Generic.List<JsDeclaredName> into)
    {
        if (pattern is null)
        {
            if (name.Length != 0)
            {
                into.Add(new JsDeclaredName(name, span, false));
            }

            return;
        }

        var names = new System.Collections.Generic.List<string>();
        PatternNames(pattern, names);

        foreach (var bound in names)
        {
            into.Add(new JsDeclaredName(bound, span, false));
        }
    }

    /// <summary>
    /// The two rules every declarative scope obeys: no lexical name twice, and no lexical name that
    /// a <c>var</c> of the same scope also declares.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The second rule is not the first one with a wider set.</b> Two <c>var</c> declarations of
    /// one name are a program - <c>{ var f; var f; }</c> runs everywhere - because a <c>var</c>
    /// binding is created once and the second declaration finds it. A lexical binding is created by
    /// its declaration, so a second one has nothing to do, and the specification refuses the source
    /// rather than choosing which declaration wins.
    /// </para>
    /// <para>
    /// <b><paramref name="functionsMayRepeat"/> is Annex B and applies to a BLOCK in sloppy code
    /// only.</b> <c>{ function f() {} function f() {} }</c> is a source the web is full of, so the
    /// specification's web-compatibility annex removes the duplicate error for it - and removes it
    /// only when EVERY declaration of the name is a function declaration and only outside strict
    /// code, which is why the flag alone is not enough and each entry remembers what declared it.
    /// The collision with a <c>var</c> is NOT relaxed by that annex, so
    /// <c>{ function f() {} var f; }</c> is refused in sloppy code too.
    /// </para>
    /// <para>
    /// <b>It stops at the first collision.</b> The parser's statement loops already halt on the
    /// first diagnostic, so a second one from the same scope would report a name whose declaration a
    /// reader is about to be told about anyway.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1D0837
    // Broiler-Human:        PENDING
    private void ValidateDeclaredNames(
        System.Collections.Generic.List<JsDeclaredName> lexical,
        System.Collections.Generic.List<JsDeclaredName> vars,
        bool functionsMayRepeat)
    {
        var seen = new System.Collections.Generic.Dictionary<string, bool>(
            System.StringComparer.Ordinal);

        foreach (var entry in lexical)
        {
            if (entry.Name.Length == 0)
            {
                continue;
            }

            if (!seen.TryGetValue(entry.Name, out var onlyFunctions))
            {
                seen[entry.Name] = entry.ByFunction;
                continue;
            }

            if (functionsMayRepeat && onlyFunctions && entry.ByFunction)
            {
                continue;
            }

            Refuse(
                entry.Span,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + entry.Name + "` is declared twice in one scope");

            return;
        }

        foreach (var entry in vars)
        {
            if (entry.Name.Length != 0 && seen.ContainsKey(entry.Name))
            {
                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    "`" + entry.Name + "` is declared both lexically and as a `var`");

                return;
            }
        }
    }

    /// <summary>Rules on a BLOCK's statement list: a block, a <c>catch</c> body or a case block.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2C6C4E
    // Broiler-Human:        PENDING
    private void ValidateBlockScope(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        // NOTHING IS CHECKED ONCE THE PARSE HAS REFUSED. A statement that failed to parse leaves a
        // placeholder in the list and a binding name that failed leaves a synthetic name, so a
        // second rule applied to that list would report a collision the source does not contain.
        if (diagnostics.Count != 0 || body.Count == 0)
        {
            return;
        }

        var lexical = new System.Collections.Generic.List<JsDeclaredName>();
        LexicallyDeclaredNames(body, topLevel: false, lexical);

        if (lexical.Count == 0)
        {
            return;
        }

        var vars = new System.Collections.Generic.List<JsDeclaredName>();
        VarDeclaredNames(body, topLevel: false, vars);
        ValidateDeclaredNames(lexical, vars, functionsMayRepeat: !strict);
    }

    /// <summary>
    /// Rules on the top of a VAR SCOPE - a script, a module, a function body or a static block -
    /// and on the parameters that scope was entered with.
    /// </summary>
    /// <remarks>
    /// <b>A parameter is not a lexical name of the body and collides with one anyway.</b>
    /// <c>function f(a) { let a; }</c> is refused by a rule of its own, because the parameter list
    /// and the body are two environments and the language forbids the inner one from shadowing the
    /// outer at its own top level; <c>function f(a) { { let a; } }</c> is a program, because a block
    /// inside the body is a third environment and shadowing is what it is for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DA4BD9
    // Broiler-Human:        PENDING
    private void ValidateVarScope(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.IReadOnlyList<JsParameter>? parameters)
    {
        if (diagnostics.Count != 0)
        {
            return;
        }

        var lexical = new System.Collections.Generic.List<JsDeclaredName>();
        LexicallyDeclaredNames(body, topLevel: true, lexical);

        if (lexical.Count == 0)
        {
            return;
        }

        var vars = new System.Collections.Generic.List<JsDeclaredName>();
        VarDeclaredNames(body, topLevel: true, vars);

        // A FUNCTION DECLARATION AT THIS LEVEL IS ONE OF THE `var` NAMES, so the duplicate rule can
        // never see two of them and the Annex B relaxation has nothing to relax here.
        ValidateDeclaredNames(lexical, vars, functionsMayRepeat: false);

        if (parameters is null || parameters.Count == 0 || diagnostics.Count != 0)
        {
            return;
        }

        var bound = new System.Collections.Generic.List<string>();

        foreach (var parameter in parameters)
        {
            PatternNames(parameter.Target, bound);
        }

        foreach (var entry in lexical)
        {
            foreach (var name in bound)
            {
                if (!string.Equals(name, entry.Name, System.StringComparison.Ordinal))
                {
                    continue;
                }

                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "`" + entry.Name + "` is a parameter of this function, so its body cannot " +
                        "declare it again");

                return;
            }
        }
    }

    /// <summary>Rules on a <c>catch</c> parameter against the block it guards.</summary>
    /// <remarks>
    /// <b>A <c>var</c> may reach through a SIMPLE catch parameter and nothing else may.</b>
    /// <c>try {} catch (e) { var e; }</c> is a source the web is full of and the specification's
    /// web-compatibility annex keeps it working; the moment the parameter destructures -
    /// <c>catch ([e])</c> - the annex stops applying and the same body is refused. A lexical
    /// declaration of the name is refused either way, and so is a function declaration of it,
    /// because a function declaration in a block is a lexical name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=6994B4
    // Broiler-Human:        PENDING
    private void ValidateCatchParameter(
        string parameter, JsPattern? pattern, JsBlockStatement handler)
    {
        if (diagnostics.Count != 0 || (parameter.Length == 0 && pattern is null))
        {
            return;
        }

        var bound = new System.Collections.Generic.List<JsDeclaredName>();
        HeadNames(parameter, pattern, handler.Span, bound);

        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var entry in bound)
        {
            if (seen.Add(entry.Name))
            {
                continue;
            }

            Refuse(
                entry.Span,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + entry.Name + "` is bound twice by one `catch` parameter");

            return;
        }

        var lexical = new System.Collections.Generic.List<JsDeclaredName>();
        LexicallyDeclaredNames(handler.Body, topLevel: false, lexical);

        foreach (var entry in lexical)
        {
            if (!seen.Contains(entry.Name))
            {
                continue;
            }

            Refuse(
                entry.Span,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + entry.Name + "` is the `catch` parameter, so the handler cannot declare it " +
                    "again");

            return;
        }

        if (pattern is null)
        {
            return;
        }

        var vars = new System.Collections.Generic.List<JsDeclaredName>();
        VarDeclaredNames(handler.Body, topLevel: false, vars);

        foreach (var entry in vars)
        {
            if (!seen.Contains(entry.Name))
            {
                continue;
            }

            Refuse(
                entry.Span,
                SliceSourceDiagnosticCode.VarAndLexicalCollision,
                "`" + entry.Name + "` is bound by a destructuring `catch` parameter, so the " +
                    "handler cannot declare it with `var`");

            return;
        }
    }

    /// <summary>Rules on one <c>let</c> or <c>const</c> declaration's own binding list.</summary>
    /// <remarks>
    /// <para>
    /// <b>Both rules are the declaration's own and neither is about the scope around it.</b>
    /// <c>let a, a;</c> is refused where <c>var a, a;</c> is a program, for the reason two lexical
    /// declarations of one name in a block are refused; and <c>let let;</c> is refused because the
    /// grammar excludes the word from a lexical declaration's bound names in sloppy code as well as
    /// strict, which is the one place <c>let</c> is reserved without strictness having anything to
    /// do with it.
    /// </para>
    /// <para>
    /// It is applied to a <c>for</c> head as well as to a statement, because
    /// <c>for (let let of []) ;</c> and <c>for (let x = 1, x = 2;;) ;</c> are the same two rules one
    /// production along.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=85ED13
    // Broiler-Human:        PENDING
    private void ValidateBindingList(
        SliceDeclarationKind kind,
        System.Collections.Generic.IReadOnlyList<JsDeclarator> declarators)
    {
        if (kind == SliceDeclarationKind.Var || diagnostics.Count != 0)
        {
            return;
        }

        var bound = new System.Collections.Generic.List<JsDeclaredName>();

        foreach (var declarator in declarators)
        {
            DeclaratorNames(declarator, bound);
        }

        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var entry in bound)
        {
            if (string.Equals(entry.Name, "let", System.StringComparison.Ordinal))
            {
                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.ReservedWordAsBinding,
                    "`let` is not a name a `let` or `const` declaration may bind");

                return;
            }

            if (seen.Add(entry.Name))
            {
                continue;
            }

            Refuse(
                entry.Span,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + entry.Name + "` is declared twice by one declaration");

            return;
        }
    }

    /// <summary>
    /// Rules on a loop head's lexical bindings against the <c>var</c> declarations of its body.
    /// </summary>
    /// <remarks>
    /// <b>The head and the body are two scopes and this is the one rule that spans them.</b>
    /// <c>for (let x; false; ) { let x; }</c> is a program, because the body's block is a scope of
    /// its own; <c>for (let x; false; ) { var x; }</c> is not, because a <c>var</c> in the body
    /// belongs to the enclosing function and would be the same binding the head just declared
    /// lexically.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9B0E82
    // Broiler-Human:        PENDING
    private void ValidateLoopHead(
        SliceDeclarationKind? kind,
        System.Collections.Generic.IReadOnlyList<JsDeclarator> declarators,
        JsStatement body)
    {
        if (kind is null or SliceDeclarationKind.Var || diagnostics.Count != 0)
        {
            return;
        }

        var bound = new System.Collections.Generic.List<JsDeclaredName>();

        foreach (var declarator in declarators)
        {
            DeclaratorNames(declarator, bound);
        }

        if (bound.Count == 0)
        {
            return;
        }

        var vars = new System.Collections.Generic.List<JsDeclaredName>();
        VarDeclaredNames([body], topLevel: false, vars);

        foreach (var entry in vars)
        {
            foreach (var head in bound)
            {
                if (!string.Equals(entry.Name, head.Name, System.StringComparison.Ordinal))
                {
                    continue;
                }

                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    "`" + entry.Name + "` is declared by this loop's head, so its body cannot " +
                        "declare it with `var`");

                return;
            }
        }
    }

    /// <summary>
    /// Rules on a formal parameter list: when a name may appear in it twice, and when the body it
    /// belongs to may declare <c>use strict</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Whether one name may be bound twice depends on three separate things and the answer is
    /// "no" if any of them says so.</b> A METHOD, an ACCESSOR and an ARROW take
    /// <c>UniqueFormalParameters</c>, so <c>({ m(a, a) {} })</c> and <c>(a, a) =&gt; 1</c> are
    /// refused however sloppy the code around them is; STRICT code refuses a duplicate in any
    /// function; and a parameter list that is not SIMPLE - one with a default, a rest or a pattern
    /// in it - refuses a duplicate in sloppy code too, because the arguments object can no longer
    /// be the mapped one that made the legacy behaviour meaningful. Only the plain sloppy function
    /// with a plain parameter list keeps <c>function f(a, a) {}</c>, and it keeps it because the web
    /// is full of it.
    /// </para>
    /// <para>
    /// <b>A body may not declare <c>use strict</c> over a parameter list that is not simple</b>, and
    /// the reason is an ordering the specification could not resolve: a default's expression is
    /// evaluated as the function is entered, so it would have to be strict or sloppy code before
    /// the directive that decides which has been reached. The language refuses the source rather
    /// than choosing, and it is a rule this parser can only apply here - after the prologue has
    /// been read, with the parameter list still in hand.
    /// </para>
    /// </remarks>
    /// <param name="parameters">The list as the source wrote it.</param>
    /// <param name="unique">Whether the production takes <c>UniqueFormalParameters</c>.</param>
    /// <param name="strictHere">Whether the parameter list is strict-mode code.</param>
    /// <param name="declaredUseStrict">Whether the body's own prologue declared strictness.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5EBBDC
    // Broiler-Human:        PENDING
    private void ValidateParameters(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters,
        bool unique,
        bool strictHere,
        bool declaredUseStrict)
    {
        if (diagnostics.Count != 0 || parameters.Count == 0)
        {
            return;
        }

        var simple = true;

        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null ||
                parameter.Target is not JsTargetPattern { Target: JsIdentifier })
            {
                simple = false;
                break;
            }
        }

        if (!simple && declaredUseStrict)
        {
            Refuse(
                parameters[0].Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a body cannot declare `use strict` over a parameter list that has a default, a " +
                    "rest parameter or a pattern in it");

            return;
        }

        if (!unique && simple && !strictHere)
        {
            return;
        }

        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var parameter in parameters)
        {
            var names = new System.Collections.Generic.List<string>();
            PatternNames(parameter.Target, names);

            foreach (var name in names)
            {
                if (seen.Add(name))
                {
                    continue;
                }

                Refuse(
                    parameter.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "`" + name + "` is bound twice by one parameter list");

                return;
            }
        }
    }

    /// <summary>Rules on an accessor's parameter list, which the grammar fixes exactly.</summary>
    /// <remarks>
    /// <b>An accessor's arity is grammar and not convention.</b> A getter is
    /// <c>get PropertyName ( ) { … }</c> with an empty list in the production itself, and a setter
    /// is <c>set PropertyName ( PropertySetParameterList ) { … }</c> where that list is ONE
    /// <c>FormalParameter</c> - which admits a pattern and a default and admits no rest, because a
    /// setter is called with exactly one argument and a rest parameter would be asking for a count
    /// the caller never varies.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4BB581
    // Broiler-Human:        PENDING
    private void ValidateAccessorParameters(
        bool isGetter,
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters,
        SliceSourceSpan span)
    {
        if (diagnostics.Count != 0)
        {
            return;
        }

        if (isGetter)
        {
            if (parameters.Count != 0)
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "a getter takes no parameters");
            }

            return;
        }

        if (parameters.Count != 1)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.ExpectedToken,
                "a setter takes exactly one parameter");

            return;
        }

        if (parameters[0].IsRest)
        {
            Refuse(
                parameters[0].Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a setter's one parameter cannot be a rest parameter");
        }
    }

    /// <summary>Whether a directive prologue asked for strict mode.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E30EEC
    // Broiler-Human:        PENDING
    private static bool DeclaresUseStrict(
        System.Collections.Generic.IReadOnlyList<JsStringLiteral> directives)
    {
        foreach (var directive in directives)
        {
            if (string.Equals(directive.RawText, "\"use strict\"", System.StringComparison.Ordinal) ||
                string.Equals(directive.RawText, "'use strict'", System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    // ---- a statement position, where a declaration may not stand --------------------------------

    /// <summary>
    /// Parses the single <c>Statement</c> a loop, an <c>if</c> or a label takes as its body, and
    /// refuses a DECLARATION written there.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The grammar admits a <c>Statement</c> in these positions and a declaration is not one</b>,
    /// which is why <c>if (x) let y;</c> and <c>while (x) class C {}</c> are syntax errors rather
    /// than one-statement scopes. The reason is the same one the <c>with</c> body already gives: a
    /// lexical declaration whose only enclosing scope is the loop's single statement would have
    /// nowhere to put its slot and nothing could ever read it.
    /// </para>
    /// <para>
    /// <b>A plain function declaration is the one exception, and it is Annex B rather than the
    /// grammar.</b> <c>if (x) function f() {}</c> and <c>label: function f() {}</c> are sources the
    /// web is full of, so the web-compatibility annex admits them in sloppy code - and admits
    /// exactly those two positions and no others, which is why <c>while (x) function f() {}</c> is
    /// refused in sloppy code too. A generator, an async function and an async generator are never
    /// admitted by that annex in any position, so <paramref name="functionAllowed"/> covers the
    /// plain form alone.
    /// </para>
    /// </remarks>
    /// <param name="position">What the body belongs to, for the message.</param>
    /// <param name="functionAllowed">
    /// Whether a plain function declaration is admitted here, which is true for an <c>if</c> clause
    /// and a labelled item in sloppy code and false everywhere else.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C9E0A0
    // Broiler-Human:        PENDING
    private JsStatement ParseNestedStatement(string position, bool functionAllowed = false)
    {
        // `let` IS THE ONE WORD WHOSE ANSWER HERE TURNS ON A LINE BREAK, and getting it wrong
        // refuses a program rather than accepting one. An `ExpressionStatement` may not BEGIN with
        // `let [` and may begin with `let` followed by anything else, so `if (false) let` followed
        // by a newline and `x = 1;` is the identifier `let`, a semicolon nobody wrote, and a
        // separate assignment - which every engine runs and the conformance suite writes down in
        // seven places. Only `let [` and a `let` on the same line as the name it would bind are the
        // declaration this position has no room for.
        if (Current.Kind == SliceTokenKind.Let && BeginsLetDeclaration() &&
            Peek(1).Kind != SliceTokenKind.OpenBracket && Peek(1).PrecededByLineTerminator)
        {
            return ParseExpressionStatement();
        }

        if (!BeginsRefusedDeclaration(functionAllowed))
        {
            return ParseStatement();
        }

        var span = Span();

        Refuse(
            span,
            SliceSourceDiagnosticCode.UnexpectedToken,
            "a declaration is not a statement, so it cannot be the body of " + position);

        return new JsEmptyStatement(span);
    }

    /// <summary>Whether the token at the cursor begins a declaration this position refuses.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1039B3
    // Broiler-Human:        PENDING
    private bool BeginsRefusedDeclaration(bool functionAllowed)
    {
        switch (Current.Kind)
        {
            case SliceTokenKind.Const:
            case SliceTokenKind.Class:
                return true;

            case SliceTokenKind.Let:
                return BeginsLetDeclaration();

            // A GENERATOR IS NEVER THE ANNEX B FORM. `if (x) function* g() {}` is a syntax error in
            // every engine and in every mode, so the `*` is tested even where a plain function
            // declaration is admitted.
            case SliceTokenKind.Function:
                return !functionAllowed || Peek(1).Kind == SliceTokenKind.Star;

            case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                !Peek(1).PrecededByLineTerminator:
                return true;

            default:
                return false;
        }
    }

    // ---- expressions ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D7E858
    // Broiler-Human:        PENDING
    private JsExpression ParseExpression(bool noIn = false)
    {
        var span = Span();
        var first = ParseAssignment(noIn);

        if (Current.Kind != SliceTokenKind.Comma)
        {
            return first;
        }

        var all = new System.Collections.Generic.List<JsExpression> { first };

        while (Current.Kind == SliceTokenKind.Comma)
        {
            Advance();
            all.Add(ParseAssignment(noIn));
        }

        return new JsSequenceExpression(span, all);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5D9CD7
    // Broiler-Human:        PENDING
    private JsExpression ParseAssignment(bool noIn = false)
    {
        if (!Enter(out var deep))
        {
            return new JsNumberLiteral(deep.Span, 0, false);
        }

        try
        {
            var span = Span();

            // `yield` IS AN ASSIGNMENT-LEVEL PRODUCTION, so it is recognised here and its operand
            // is parsed at the same level. Putting it in ParsePrimary would have made
            // `yield a + b` parse as `(yield a) + b`, which is the wrong tree and a silently wrong
            // program rather than a refusal.
            if (yieldIsOperator && Current.Kind == SliceTokenKind.Yield)
            {
                return ParseYield(noIn);
            }

            if (TryParseArrow(span, out var arrow))
            {
                return arrow;
            }

            var target = ParseConditional(noIn);

            if (Current.Kind is SliceTokenKind.Equals or SliceTokenKind.CompoundAssign)
            {
                var op = Current.Kind == SliceTokenKind.Equals
                    ? SliceTokenKind.Equals
                    : CompoundOperator(Current.RawText);

                // `[a, b] = c` IS A DESTRUCTURING ASSIGNMENT, and nothing before the `=` could
                // have said so: the left-hand side was read as an array literal because that is
                // the only thing it could have been so far. This is where the cover grammar is
                // resolved, and it is the only place a literal turns into a pattern.
                if (target is JsArrayLiteral or JsObjectLiteral)
                {
                    if (op != SliceTokenKind.Equals)
                    {
                        Refuse(
                            span,
                            SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                            "a destructuring assignment has no compound form");
                    }

                    Advance();
                    return new JsDestructuringAssignment(
                        span, ToPattern(target), ParseAssignment(noIn));
                }

                // A PRIVATE ACCESS IS A REFERENCE AND IS THE FOURTH KIND HERE. `this.#x = 1` is an
                // ordinary assignment whose target happens to be stored somewhere other than the
                // property table, and leaving it off this list refused the write at the parse -
                // before any of the three passes that know what a private name is could see it.
                if (target is not JsIdentifier and not JsMemberExpression and
                    not JsSuperMemberExpression and not JsPrivateMemberExpression)
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "the left-hand side of an assignment is not a reference");
                }

                // AND IN STRICT CODE THE TWO RESTRICTED NAMES ARE NOT TARGETS. It is the same rule
                // the binding names obey, on the other side of the reference: a program may read
                // `arguments` in strict code and may not replace it.
                if (strict && target is JsIdentifier assigned &&
                    IsRestrictedInStrictCode(assigned.Name))
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "`" + assigned.Name +
                            "` is not the target of an assignment in strict code");
                }

                Advance();
                var value = ParseAssignment(noIn);
                return new JsAssignmentExpression(span, op, target, value);
            }

            return target;
        }
        finally
        {
            depth--;
        }
    }

    /// <summary>
    /// Parses <c>yield</c>, <c>yield expr</c> or <c>yield* expr</c>.
    /// </summary>
    /// <remarks>
    /// <b>A line terminator after <c>yield</c> ends it</b>, which is a restricted production and
    /// not automatic semicolon insertion: <c>yield \n 1;</c> is a bare <c>yield</c> followed by the
    /// expression statement <c>1;</c>, and a parser that read across the newline would turn two
    /// statements into one and yield the wrong value. The other terminators are the tokens that
    /// begin no expression, which is where an operand stops being optional.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=997C18
    // Broiler-Human:        PENDING
    private JsExpression ParseYield(bool noIn)
    {
        var span = Span();
        Advance();

        // `yield [no LineTerminator here] *` IS THE PRODUCTION, so a newline before the star does
        // not make a delegation - it makes a bare `yield` whose statement then continues with a
        // `*` that begins no expression. That is the syntax error every engine gives, and reading
        // across the newline instead would silently turn two statements into one delegation.
        if (Current.Kind == SliceTokenKind.Star && !Current.PrecededByLineTerminator)
        {
            Advance();
            return new JsYieldExpression(span, ParseAssignment(noIn), IsDelegate: true);
        }

        if (Current.PrecededByLineTerminator || Current.Kind is SliceTokenKind.CloseParen or
            SliceTokenKind.CloseBracket or SliceTokenKind.CloseBrace or SliceTokenKind.Comma or
            SliceTokenKind.Semicolon or SliceTokenKind.Colon or SliceTokenKind.EndOfSource)
        {
            return new JsYieldExpression(span, null, IsDelegate: false);
        }

        return new JsYieldExpression(span, ParseAssignment(noIn), IsDelegate: false);
    }

    /// <summary>
    /// Answers whether a token is a name this goal and this strictness admit as an identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b><c>await</c> and <c>yield</c> are CONTEXTUAL and were treated as unconditional
    /// keywords.</b> The tokenizer gives each its own kind, which is right, and the parser then
    /// refused both everywhere, which is not: <c>var await = 1;</c> is an ordinary program in a
    /// script and every engine runs it. Refusing it said this manifest does not admit a construct
    /// when the construct is an identifier the manifest admits perfectly well.
    /// </para>
    /// <para>
    /// <b>Where they ARE reserved, the answer is a syntax error and not a manifest refusal</b>, and
    /// the difference matters to the conformance runner. <c>await</c> is reserved in a module and
    /// ordinary in a script; <c>yield</c> is reserved in strict code and ordinary in sloppy. A test
    /// asserting either of those reservations is a test this profile can pass rather than one it
    /// has to decline, and it now passes for the reason the test names.
    /// </para>
    /// <para>
    /// <b><c>await</c> gained a third context on the day async functions were admitted</b>, and it
    /// is the mirror of <c>yield</c>'s: a name in a script, a reserved word in a module, and a
    /// reserved word inside an async function's own body and parameter list whatever the goal.
    /// Outside an async body nothing moved - <c>await x</c> in a script is still two identifiers in
    /// a row and still the syntax error every engine reports it as - because admitting a construct
    /// must not change what an unrelated program is told.
    /// </para>
    /// <para>
    /// <b><c>yield</c> gained a third context on the day generators were admitted.</b> It is a name
    /// in sloppy code, a reserved word in strict code, and a reserved word inside a generator
    /// whatever the strictness - so the answer here is no longer strictness alone. The third case
    /// is still a syntax error rather than a manifest refusal, for the same reason the second is:
    /// the manifest admits generators, and it is the LANGUAGE that says a generator's own body may
    /// not bind that name.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F50ACC
    // Broiler-Human:        PENDING
    private bool IsIdentifierName(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
            SliceTokenKind.Of or SliceTokenKind.Async or SliceTokenKind.Static or
            SliceTokenKind.Let => true,
        SliceTokenKind.Await => options.Goal != SliceGoal.Module && !awaitIsOperator,
        SliceTokenKind.Yield => !strict && !yieldIsOperator,
        _ => false,
    };

    /// <summary>
    /// Whether the <c>let</c> at the cursor begins a DECLARATION rather than being an identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The three places that asked this asked it with a hard-coded token list, and the list was
    /// short by two.</b> <c>yield</c> and <c>await</c> are ordinary binding names where the
    /// enclosing context does not make them operators, so <c>let yield = 4;</c> in sloppy
    /// non-generator code is a lexical declaration every engine runs - and this parser answered
    /// "`;` was expected and `yield` was found", because `let` fell through to the identifier arm
    /// and the name after it became the surprise. It is a refusal of a correct program, and the
    /// conformance runner scores that as a failure rather than as anything the manifest declines.
    /// </para>
    /// <para>
    /// <b>So the question is asked of <see cref="IsIdentifierName"/>, which already knows both
    /// context rules</b>, plus the two bracket tokens a destructuring declaration begins with. One
    /// predicate rather than three copies is also what stops the next name added to the identifier
    /// set from being added to two of the three.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3A6447
    // Broiler-Human:        PENDING
    private bool BeginsLetDeclaration() =>
        IsIdentifierName(Peek(1).Kind) ||
        Peek(1).Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace;

    /// <summary>
    /// Recognises an arrow function, which the grammar cannot see coming from its first token.
    /// </summary>
    /// <remarks>
    /// <c>(a, b) => a + b</c> and <c>(a, b)</c> begin identically, so the decision needs either a
    /// cover grammar and a reinterpretation pass or a bounded scan. This is the scan: from an
    /// identifier, one token of lookahead settles it; from an open parenthesis, the matching close
    /// is found by counting brackets and the token after it settles it. The scan reads no
    /// expression and allocates nothing, so a source with many parenthesised expressions costs a
    /// bracket count each and not a speculative parse.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DC2432
    // Broiler-Human:        PENDING
    private bool TryParseArrow(SliceSourceSpan span, out JsExpression arrow)
    {
        arrow = null!;

        // AN ASYNC ARROW IS RECOGNISED HERE RATHER THAN IN ParsePrimary, and the position is the
        // point. `async(1)` is a call of a function named `async` and `{ async: 1 }` is a
        // property, both of which this manifest admits; only the `=>` that the scan below finds
        // tells an arrow from either. Deciding on the first token would have broken two admitted
        // programs to reach one construct.
        if (Current.Kind == SliceTokenKind.Async && IsAsyncArrowHead())
        {
            Advance();

            // ITS PARAMETERS ARE `[+Await]` AND SO IS ITS BODY, which is the one place an arrow's
            // context is not simply cleared: an ordinary arrow's body is `[~Yield, ~Await]`
            // whatever encloses it, and an async arrow's is `[~Yield, +Await]` because the arrow
            // ITSELF is what supplies the async context. `async (await) => 1` is therefore refused
            // for its parameter, and `async () => await x` parses.
            var outerAwait = awaitIsOperator;
            awaitIsOperator = true;

            try
            {
                if (Current.Kind != SliceTokenKind.OpenParen)
                {
                    var only = new System.Collections.Generic.List<JsParameter>
                    {
                        new(
                            Span(),
                            new JsTargetPattern(Span(), new JsIdentifier(Span(), Current.RawText)),
                            null,
                            IsRest: false),
                    };

                    if (!IsIdentifierName(Current.Kind))
                    {
                        Refuse(
                            Span(),
                            SliceSourceDiagnosticCode.ReservedWordAsBinding,
                            "`" + Current.RawText + "` is not a binding name");
                    }

                    Advance();
                    Expect(SliceTokenKind.EqualsGreaterThan, "=>");
                    arrow = ParseArrowBody(span, only, isAsync: true);
                    return true;
                }

                var asyncParameters = ParseParameters();
                Expect(SliceTokenKind.EqualsGreaterThan, "=>");
                arrow = ParseArrowBody(span, asyncParameters, isAsync: true);
                return true;
            }
            finally
            {
                awaitIsOperator = outerAwait;
            }
        }

        if (Current.Kind is SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
            SliceTokenKind.Of or SliceTokenKind.Static)
        {
            if (Peek(1).Kind != SliceTokenKind.EqualsGreaterThan)
            {
                return false;
            }

            var single = new System.Collections.Generic.List<JsParameter>
            {
                new(
                    span,
                    new JsTargetPattern(span, new JsIdentifier(span, Current.RawText)),
                    null,
                    IsRest: false),
            };

            Advance();
            Advance();
            arrow = ParseArrowBody(span, single);
            return true;
        }

        if (Current.Kind != SliceTokenKind.OpenParen)
        {
            return false;
        }

        var open = 1;
        var scan = at + 1;

        while (open != 0 && tokens[scan].Kind != SliceTokenKind.EndOfSource)
        {
            open += tokens[scan].Kind switch
            {
                SliceTokenKind.OpenParen or SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace => 1,
                SliceTokenKind.CloseParen or SliceTokenKind.CloseBracket or SliceTokenKind.CloseBrace => -1,
                _ => 0,
            };

            scan++;
        }

        if (tokens[scan].Kind != SliceTokenKind.EqualsGreaterThan)
        {
            return false;
        }

        var parameters = ParseOrdinaryParameters();
        Expect(SliceTokenKind.EqualsGreaterThan, "=>");
        arrow = ParseArrowBody(span, parameters);
        return true;
    }

    /// <summary>
    /// Answers whether an <c>async</c> at the cursor begins an arrow function rather than an
    /// identifier this manifest admits.
    /// </summary>
    /// <remarks>
    /// The same bounded scan <see cref="TryParseArrow"/> uses, shifted one token: from
    /// <c>async x</c> the token after settles it, and from <c>async (</c> the matching close is
    /// found by counting brackets and the token after that settles it. A line terminator between
    /// <c>async</c> and its parameter list makes it not an async arrow, which is the same rule
    /// automatic semicolon insertion applies to <c>async function</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BED60B
    // Broiler-Human:        PENDING
    private bool IsAsyncArrowHead()
    {
        if (Peek(1).PrecededByLineTerminator)
        {
            return false;
        }

        if (Peek(1).Kind is SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
            SliceTokenKind.Of or SliceTokenKind.Static or SliceTokenKind.Let)
        {
            return Peek(2).Kind == SliceTokenKind.EqualsGreaterThan;
        }

        if (Peek(1).Kind != SliceTokenKind.OpenParen)
        {
            return false;
        }

        var open = 1;
        var scan = at + 2;

        while (open != 0 && tokens[scan].Kind != SliceTokenKind.EndOfSource)
        {
            open += tokens[scan].Kind switch
            {
                SliceTokenKind.OpenParen or SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace => 1,
                SliceTokenKind.CloseParen or SliceTokenKind.CloseBracket or SliceTokenKind.CloseBrace => -1,
                _ => 0,
            };

            scan++;
        }

        return tokens[scan].Kind == SliceTokenKind.EqualsGreaterThan;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=460536
    // Broiler-Human:        PENDING
    private JsExpression ParseArrowBody(
        SliceSourceSpan span,
        System.Collections.Generic.List<JsParameter> parameters,
        bool isAsync = false)
    {
        // AN ARROW'S BODY IS `[~Yield]` EVEN INSIDE A GENERATOR, and its parameter list is NOT -
        // which is why the flag is cleared here rather than in TryParseArrow, after the parameters
        // have already been read. So `function* g() { var f = (yield) => 1; }` is refused for its
        // parameter and `function* g() { var f = () => yield; }` parses, with `yield` an ordinary
        // identifier reference - which is what every engine does, and the opposite of what a
        // reading of "yield may only appear in the generator's own body" suggests.
        //
        // THE `[Await]` HALF IS NOT SYMMETRIC AND IS SET RATHER THAN CLEARED. An ordinary arrow
        // inside an async function is `[~Await]`, so `async function f(){ var g = () => await 1; }`
        // is the two-identifiers-in-a-row syntax error every engine gives; an ASYNC arrow is
        // `[+Await]` whatever encloses it, because it is itself the async context.
        var outerOperator = yieldIsOperator;
        var outerAwait = awaitIsOperator;
        yieldIsOperator = false;
        awaitIsOperator = isAsync;

        try
        {
            if (Current.Kind == SliceTokenKind.OpenBrace)
            {
                return new JsFunctionExpression(
                    span,
                    ParseFunctionBody(
                        span, string.Empty, parameters, isArrow: true, isGenerator: false, isAsync,
                        uniqueParameters: true));
            }

            var value = ParseAssignment();

            // A CONCISE BODY REACHES NO `ParseFunctionBody`, so the parameter rules are applied
            // here as well. An arrow takes `UniqueFormalParameters` in both of its two body forms,
            // and a concise body declares no prologue, so `use strict` cannot be the reason.
            ValidateParameters(parameters, unique: true, strict, declaredUseStrict: false);

            var body = new System.Collections.Generic.List<JsStatement>
            {
                new JsReturnStatement(span, value),
            };

            return new JsFunctionExpression(
                span,
                new JsFunctionNode(
                    span, string.Empty, parameters, body, true, strict, [], false, isAsync));
        }
        finally
        {
            yieldIsOperator = outerOperator;
            awaitIsOperator = outerAwait;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5BCABC
    // Broiler-Human:        PENDING
    private JsExpression ParseConditional(bool noIn)
    {
        var span = Span();
        var test = ParseBinary(1, noIn);

        if (Current.Kind != SliceTokenKind.Question)
        {
            return test;
        }

        Advance();
        var whenTrue = ParseAssignment();
        Expect(SliceTokenKind.Colon, ":");
        var whenFalse = ParseAssignment(noIn);
        return new JsConditionalExpression(span, test, whenTrue, whenFalse);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C342F9
    // Broiler-Human:        PENDING
    private JsExpression ParseBinary(int minimum, bool noIn)
    {
        var span = Span();

        // `#x in obj` IS RECOGNISED HERE AND NOWHERE BELOW, because the grammar gives it a
        // production at exactly this level - `RelationalExpression : PrivateIdentifier in
        // ShiftExpression` - and the private name in it is not an operand of anything. Reaching it
        // from the primary parse instead would have meant pushing a private name as a value, which
        // no instruction of this profile can do and no other position of the grammar asks for; the
        // result then rejoins the loop as an ordinary left operand, so `#x in a === b` associates
        // the way the precedence table already says it does.
        if (IsPrivateName(Current) && Peek(1).Kind == SliceTokenKind.In && !noIn)
        {
            var privateName = Current.RawText;
            Advance();
            Advance();

            return Continue(
                new JsPrivateInExpression(span, privateName, ParseBinary(Relational + 1, noIn)),
                span,
                minimum,
                noIn);
        }

        return Continue(ParseUnary(), span, minimum, noIn);
    }

    /// <summary>The precedence of <c>in</c>, <c>instanceof</c> and the four comparisons.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2AB4A2
    // Broiler-Human:        PENDING
    private const int Relational = 8;

    /// <summary>Extends an already-parsed left operand with every operator that may follow it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2C6F23
    // Broiler-Human:        PENDING
    private JsExpression Continue(JsExpression left, SliceSourceSpan span, int minimum, bool noIn)
    {
        while (true)
        {
            var kind = Current.Kind;

            if (kind == SliceTokenKind.In && noIn)
            {
                return left;
            }

            var precedence = Precedence(kind);

            if (precedence < minimum)
            {
                return left;
            }

            Advance();

            // `**` is the one right-associative operator, so its right operand is parsed at the
            // SAME precedence rather than one above it.
            var next = kind == SliceTokenKind.StarStar ? precedence : precedence + 1;
            var right = ParseBinary(next, noIn);

            left = kind is SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar or
                SliceTokenKind.QuestionQuestion
                ? new JsLogicalExpression(span, kind, left, right)
                : new JsBinaryExpression(span, kind, left, right);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=6D1A55
    // Broiler-Human:        PENDING
    private static int Precedence(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.QuestionQuestion => 1,
        SliceTokenKind.BarBar => 2,
        SliceTokenKind.AmpersandAmpersand => 3,
        SliceTokenKind.Bar => 4,
        SliceTokenKind.Caret => 5,
        SliceTokenKind.Ampersand => 6,
        SliceTokenKind.EqualsEquals or SliceTokenKind.BangEquals or
        SliceTokenKind.EqualsEqualsEquals or SliceTokenKind.BangEqualsEquals => 7,
        SliceTokenKind.LessThan or SliceTokenKind.LessThanEquals or
        SliceTokenKind.GreaterThan or SliceTokenKind.GreaterThanEquals or
        SliceTokenKind.Instanceof or SliceTokenKind.In => 8,
        SliceTokenKind.LessThanLessThan or SliceTokenKind.GreaterThanGreaterThan or
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => 9,
        SliceTokenKind.Plus or SliceTokenKind.Minus => 10,
        SliceTokenKind.Star or SliceTokenKind.Slash or SliceTokenKind.Percent => 11,
        SliceTokenKind.StarStar => 12,
        _ => 0,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5C81EB
    // Broiler-Human:        PENDING
    private JsExpression ParseUnary()
    {
        var span = Span();

        // `await` IS A UNARY-LEVEL PRODUCTION AND `yield` IS AN ASSIGNMENT-LEVEL ONE, which is why
        // the two are recognised in different methods rather than beside each other. The grammar is
        // `await UnaryExpression`, so `await a + b` is `(await a) + b` and `await -x` is
        // `await (-x)`; recognising it at assignment level, where `yield` lives, would have made
        // the first of those `await (a + b)` - a silently wrong program rather than a refusal.
        if (awaitIsOperator && Current.Kind == SliceTokenKind.Await)
        {
            // AN `await` OUTSIDE EVERY FUNCTION IS THE MODULE'S OWN, and the lowering has to be
            // told: a module body containing one is entered as an async frame and one that does not
            // is entered as an ordinary program body. The parser is the only pass that can tell
            // them apart cheaply, because it is the one that already knows how deep in functions it
            // is.
            sawTopLevelAwait |= functionDepth == 0;

            Advance();
            return new JsAwaitExpression(span, ParseUnary());
        }

        switch (Current.Kind)
        {
            case SliceTokenKind.Plus:
            case SliceTokenKind.Minus:
            case SliceTokenKind.Bang:
            case SliceTokenKind.Tilde:
            case SliceTokenKind.Typeof:
            case SliceTokenKind.Void:
            case SliceTokenKind.Delete:
            {
                var op = Current.Kind;
                Advance();
                var operand = ParseUnary();

                // `delete x` IS A SYNTAX ERROR IN STRICT CODE, whatever `x` names. The operator
                // deletes a PROPERTY, and a bare name is a binding: sloppy code answers `false`
                // for one that is not configurable and strict code refuses to ask, because the
                // question was almost certainly meant about something else.
                if (strict && op == SliceTokenKind.Delete && operand is JsIdentifier deleted)
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`delete " + deleted.Name + "` is a syntax error in strict code: a bare " +
                            "name is a binding and `delete` removes a property");
                }

                return new JsUnaryExpression(span, op, operand);
            }

            case SliceTokenKind.PlusPlus:
            case SliceTokenKind.MinusMinus:
            {
                var op = Current.Kind;
                Advance();
                var operand = ParseUnary();
                RefuseRestrictedUpdate(span, operand);
                return new JsUpdateExpression(span, op, operand, Prefix: true);
            }

            default:
                return ParsePostfix();
        }
    }

    /// <summary>
    /// Refuses <c>++eval</c> and its three siblings, which strict code has no production for.
    /// </summary>
    /// <remarks>
    /// An update is a read and a write of the same reference, so the write half is what the rule is
    /// about and it is the same rule an assignment obeys. Both prefix and postfix ask, because the
    /// difference between them is what the expression ANSWERS and not what it writes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9EE713
    // Broiler-Human:        PENDING
    private void RefuseRestrictedUpdate(SliceSourceSpan span, JsExpression operand)
    {
        if (strict && operand is JsIdentifier name && IsRestrictedInStrictCode(name.Name))
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "`" + name.Name + "` is not the operand of an update in strict code");
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=EC4727
    // Broiler-Human:        PENDING
    private JsExpression ParsePostfix()
    {
        var span = Span();
        var operand = ParseCallChain();

        if (Current.Kind is SliceTokenKind.PlusPlus or SliceTokenKind.MinusMinus &&
            !Current.PrecededByLineTerminator)
        {
            var op = Current.Kind;
            Advance();
            RefuseRestrictedUpdate(span, operand);
            return new JsUpdateExpression(span, op, operand, Prefix: false);
        }

        return operand;
    }

    /// <summary>
    /// Parses a run of member accesses, calls, tagged templates and optional links from one head.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The whole run is one chain, and the wrap at the end is what says so.</b> An optional
    /// link short-circuits past EVERY remaining link and not merely past the next one, so the node
    /// that has to exist is one enclosing the outermost link - which is the value this loop holds
    /// when it stops. Wrapping only when a <c>?.</c> was actually seen keeps every chain-free
    /// program's tree byte-for-byte what it was.
    /// </para>
    /// <para>
    /// <b>And where the loop stops IS the parenthesis.</b> <c>(a?.b).c</c> reaches this method
    /// twice: the inner call wraps and returns, and the outer one sees a wrapped value with a
    /// <c>.</c> after it and no <c>?.</c> of its own. So the outer access is ordinary and throws on
    /// a nullish <c>a</c>, which is what the language says and what a chain that leaked past its
    /// parentheses would get wrong.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3806DF
    // Broiler-Human:        PENDING
    private JsExpression ParseCallChain()
    {
        var span = Span();
        JsExpression current;
        var optional = false;

        if (Current.Kind == SliceTokenKind.New)
        {
            Advance();

            // `new.target` IS A MEMBER EXPRESSION AND NOT A FINISHED ONE, so it joins the loop
            // below rather than returning. `new.target.name` and `new.target === C` are both
            // ordinary programs; returning here would hand the `.` to a caller that has no reading
            // for it and report a syntax error against a construct the language admits.
            if (Current.Kind == SliceTokenKind.Dot)
            {
                current = ParseNewTarget(span);
            }
            else
            {
                var callee = ParseMemberOnly();

                var arguments = Current.Kind == SliceTokenKind.OpenParen
                    ? ParseArguments()
                    : [];

                current = new JsNewExpression(span, callee, arguments);

                // `new a?.b()` HAS NO READING. The language forbids an optional link directly on a
                // `new` expression, and the reason is that `new` needs a reference the chain has
                // already agreed may not exist. Saying so here is the honest answer; letting the
                // loop below take it would silently produce `(new a)?.b()`, a different program.
                if (Current.Kind == SliceTokenKind.QuestionDot)
                {
                    Refuse(
                        Span(),
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "an optional chain does not begin at a `new` expression");

                    return current;
                }
            }
        }
        else
        {
            current = ParsePrimary();
        }

        while (true)
        {
            switch (Current.Kind)
            {
                case SliceTokenKind.Dot:
                    Advance();
                    current = AfterDot(span, current, optional: false);
                    break;

                case SliceTokenKind.QuestionDot:
                {
                    optional = true;
                    Advance();

                    if (Current.Kind == SliceTokenKind.OpenParen)
                    {
                        current = new JsCallExpression(
                            span, current, ParseArguments(), Optional: true);

                        break;
                    }

                    if (Current.Kind == SliceTokenKind.OpenBracket)
                    {
                        Advance();
                        var optionalKey = ParseExpression();
                        Expect(SliceTokenKind.CloseBracket, "]");

                        current = new JsMemberExpression(
                            span, current, string.Empty, optionalKey, Optional: true);

                        break;
                    }

                    if (Current.Kind == SliceTokenKind.TemplateLiteral)
                    {
                        Refuse(
                            Span(),
                            SliceSourceDiagnosticCode.UnexpectedToken,
                            "a tagged template has no reading inside an optional chain");

                        return new JsChainExpression(span, current);
                    }

                    current = AfterDot(span, current, optional: true);
                    break;
                }

                case SliceTokenKind.OpenBracket:
                {
                    Advance();
                    var key = ParseExpression();
                    Expect(SliceTokenKind.CloseBracket, "]");
                    current = new JsMemberExpression(span, current, string.Empty, key);
                    break;
                }

                case SliceTokenKind.OpenParen:
                    current = new JsCallExpression(span, current, ParseArguments());
                    break;

                case SliceTokenKind.TemplateLiteral:
                {
                    // THE SAME PROHIBITION FROM THE OTHER SIDE. `a?.b`c`` is refused wherever the
                    // template sits in the chain, because a tag receives a reference and an
                    // optional chain is the one expression that may have declined to produce one.
                    if (optional)
                    {
                        Refuse(
                            Span(),
                            SliceSourceDiagnosticCode.UnexpectedToken,
                            "a tagged template has no reading inside an optional chain");

                        return new JsChainExpression(span, current);
                    }

                    current = new JsTaggedTemplate(span, current, ParseTemplate());
                    break;
                }

                default:
                    return optional ? new JsChainExpression(span, current) : current;
            }
        }
    }

    /// <summary>Parses a dynamic <c>import()</c>, which is a call and not a reference.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is admitted under both goals, and that is the difference between it and every static
    /// import form.</b> <c>import x from './m'</c> declares a binding of a module and is a module
    /// declaration; <c>import('./m')</c> declares nothing, answers a promise, and is a
    /// <c>CallExpression</c> a script may write as freely as a module may. So there is no
    /// <see cref="RequireModuleGoal"/> here, deliberately.
    /// </para>
    /// <para>
    /// <b>The arguments are <c>AssignmentExpression</c>s and not an argument list</b>, which is the
    /// whole reason this is parsed here rather than left to the call loop. Two consequences the
    /// suite tests directly: a spread is not admitted, because <c>import(...args)</c> has no
    /// production; and a trailing comma after the second argument is, because the grammar writes
    /// one in. There is no third argument.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=63C76E
    // Broiler-Human:        PENDING
    private JsExpression ParseImportCall(SliceSourceSpan span)
    {
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");

        if (Current.Kind == SliceTokenKind.CloseParen)
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a dynamic `import()` needs a module specifier");

            Advance();
            return new JsNullLiteral(span);
        }

        var specifier = ParseAssignment();
        JsExpression? options = null;

        if (Current.Kind == SliceTokenKind.Comma)
        {
            Advance();

            if (Current.Kind != SliceTokenKind.CloseParen)
            {
                options = ParseAssignment();

                if (Current.Kind == SliceTokenKind.Comma)
                {
                    Advance();
                }
            }
        }

        Expect(SliceTokenKind.CloseParen, ")");
        return new JsImportCall(span, specifier, options);
    }

    /// <summary>Parses the <c>import.meta</c> meta-property.</summary>
    /// <remarks>
    /// <b>It is admitted only in a module, and the refusal says which rule that is.</b> A script
    /// has no module record for a host to have populated, so <c>import.meta</c> there is a syntax
    /// error in every engine rather than a value that happens to be empty — and it is refused with
    /// the module-goal code rather than as a construct outside the manifest, because the manifest
    /// is not what forbids it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D5F63F
    // Broiler-Human:        PENDING
    private JsExpression ParseImportMeta(SliceSourceSpan span)
    {
        Advance();
        Advance();

        if (!string.Equals(Current.RawText, "meta", System.StringComparison.Ordinal))
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`import.` is followed by `meta` and by nothing else");

            Advance();
            return new JsNullLiteral(span);
        }

        Advance();

        if (options.Goal != SliceGoal.Module)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.ImportMetaOutsideModuleGoal,
                "`import.meta` names a module's own metadata and this source was presented as " +
                    "a script");

            return new JsNullLiteral(span);
        }

        return new JsImportMeta(span);
    }

    /// <summary>Parses <c>new.target</c>, having already consumed the <c>new</c>.</summary>
    /// <remarks>
    /// <b>It is admitted only where the grammar admits it.</b> <c>new.target</c> at the top level
    /// of a script is a syntax error in every engine, and so is one inside a top-level arrow -
    /// because the arrow reads the enclosing function's, and there is none. Reporting that as an
    /// unexpected token rather than as a construct outside the manifest is the point: the manifest
    /// is not what forbids it, the language is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B6B03F
    // Broiler-Human:        PENDING
    private JsExpression ParseNewTarget(SliceSourceSpan span)
    {
        Advance();

        if (!string.Equals(Current.RawText, "target", System.StringComparison.Ordinal))
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`new.` is followed by `target` and by nothing else");

            return new JsNullLiteral(span);
        }

        Advance();

        if (functionDepth == 0)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`new.target` names how a function was called and no function encloses this one");

            return new JsNullLiteral(span);
        }

        return new JsNewTargetExpression(span);
    }

    /// <summary>Parses the callee of a <c>new</c>, which stops before the argument list.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=571CC1
    // Broiler-Human:        PENDING
    private JsExpression ParseMemberOnly()
    {
        var span = Span();

        // `new import('')` HAS NO PRODUCTION AND IS NOT MERELY A CONSTRUCTOR THAT WILL THROW. A
        // `new` takes a MemberExpression and `ImportCall` is a CallExpression, so the grammar stops
        // here rather than at the missing `[[Construct]]` — which is why the suite expects a parse
        // error and not a `TypeError`. `new (import(''))` is a different program: the parentheses
        // make it a PrimaryExpression, it parses, and it throws when it runs.
        if (Current.Kind == SliceTokenKind.Import && Peek(1).Kind == SliceTokenKind.OpenParen)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a dynamic `import()` is a call expression and `new` takes a member expression");

            Advance();
            return new JsNullLiteral(span);
        }

        var current = Current.Kind == SliceTokenKind.New ? ParseCallChain() : ParsePrimary();

        while (true)
        {
            switch (Current.Kind)
            {
                case SliceTokenKind.Dot:
                    Advance();
                    current = AfterDot(span, current, optional: false);
                    break;

                case SliceTokenKind.OpenBracket:
                {
                    Advance();
                    var key = ParseExpression();
                    Expect(SliceTokenKind.CloseBracket, "]");
                    current = new JsMemberExpression(span, current, string.Empty, key);
                    break;
                }

                // A TAGGED TEMPLATE IS A MEMBER EXPRESSION, so `new tag`x`` constructs with the
                // TAG's result and not with the `new` expression tagged afterwards. The two
                // readings differ in which function is called with what, so the grammar's answer
                // is the one to keep.
                case SliceTokenKind.TemplateLiteral:
                    current = new JsTaggedTemplate(span, current, ParseTemplate());
                    break;

                default:
                    return current;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=FD5C14
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsExpression> ParseArguments()
    {
        var arguments = new System.Collections.Generic.List<JsExpression>();
        Expect(SliceTokenKind.OpenParen, "(");

        while (Current.Kind != SliceTokenKind.CloseParen &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                var spreadSpan = Span();
                Advance();
                arguments.Add(new JsSpreadElement(spreadSpan, ParseAssignment()));
            }
            else
            {
                arguments.Add(ParseAssignment());
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseParen, ")");
        return arguments;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=098BA5
    // Broiler-Human:        PENDING
    private JsExpression ParsePrimary()
    {
        var span = Span();
        var token = Current;

        switch (token.Kind)
        {
            case SliceTokenKind.NumericLiteral:
                Advance();

                if (strict && token.IsLegacyOctal)
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.LegacyOctalInStrictCode,
                        "a legacy octal literal is not admitted in strict code");
                }

                return new JsNumberLiteral(span, token.NumericValue, token.IsLegacyOctal);

            case SliceTokenKind.StringLiteral:
                Advance();
                return new JsStringLiteral(span, token.StringValue, token.RawText);

            case SliceTokenKind.True:
                Advance();
                return new JsBooleanLiteral(span, true);

            case SliceTokenKind.False:
                Advance();
                return new JsBooleanLiteral(span, false);

            case SliceTokenKind.Null:
                Advance();
                return new JsNullLiteral(span);

            case SliceTokenKind.This:
                Advance();
                return new JsThisExpression(span);

            // A CONSTRUCT OUTSIDE THE MANIFEST IS REFUSED BY NAME WHEREVER IT APPEARS, and until
            // 2026-09-04 these three were refused at statement position only. `f(async function
            // () {})` came back as "`)` was expected and `function` was found", because `async`
            // fell into the identifier arm below and the next token was a surprise. That is not a
            // cosmetic difference: the conformance runner grades the manifest boundary on the
            // DIAGNOSTIC CODE, so a construct refused under any other code is scored a failure -
            // and a negative test expecting a `SyntaxError` at parse is scored a PASS, which is
            // the false point the unsupported verdict exists to prevent.
            case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                !Peek(1).PrecededByLineTerminator:
                Advance();
                Advance();

                return new JsFunctionExpression(
                    span, ParseFunctionRest(span, declaration: false, isAsync: true));

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.OpenParen:
                return ParseImportCall(span);

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.Dot:
                return ParseImportMeta(span);

            case SliceTokenKind.Await when options.Goal != SliceGoal.Module && !awaitIsOperator:
            case SliceTokenKind.Yield when !strict && !yieldIsOperator:
                Advance();
                return new JsIdentifier(span, token.RawText);

            // RESERVED HERE, ORDINARY THERE. Where the goal, the strictness, the enclosing
            // generator or the enclosing async function makes one of these a reserved word, the
            // honest answer is the syntax error every engine gives and NOT a
            // construct-outside-the-manifest refusal: the manifest is not what forbids it. A
            // `yield` inside a generator and an `await` inside an async function reach this arm
            // from the one place that does not go through unary or assignment level - the callee of
            // a `new` - which is exactly where the language admits neither operator either.
            case SliceTokenKind.Await:
            case SliceTokenKind.Yield:
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.ReservedWordAsBinding,
                    "`" + token.RawText + "` is a reserved word " +
                        (token.Kind == SliceTokenKind.Await
                            ? awaitIsOperator ? "in an async function" : "in a module"
                            : yieldIsOperator && !strict ? "in a generator" : "in strict code"));

                Advance();
                return new JsNullLiteral(span);
            }

            // A PRIVATE NAME REACHING HERE HAS ALREADY FAILED THE ONE PRODUCTION THAT ADMITS IT.
            // `#x in obj` is caught at the relational level and `obj.#x` after the dot, so what is
            // left is a private name standing where a value belongs - `#x + 1`, `f(#x)` - which is
            // a syntax error rather than a surface this manifest declines. Left to the arm below it
            // would have become a free name and a run-time `ReferenceError`.
            case SliceTokenKind.Identifier when IsPrivateName(token):
                Advance();

                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "a private name is read through `.` or tested with `in`, and is not a value");

                return new JsNullLiteral(span);

            case SliceTokenKind.Identifier:
            case SliceTokenKind.Get:
            case SliceTokenKind.Set:
            case SliceTokenKind.Of:
            case SliceTokenKind.Static:
            case SliceTokenKind.Async:
            case SliceTokenKind.Let:
                // AN IDENTIFIER REFERENCE IS AN Identifier AND NOT AN IdentifierName, so a reserved
                // word reached through an escape is refused here exactly as a binding position
                // refuses one. `break` read as a free variable would have been a run-time
                // `ReferenceError` about a name the source never meant to write.
                _ = RefuseEscapedReservedWord(token);
                Advance();
                return new JsIdentifier(span, token.RawText);

            case SliceTokenKind.RegularExpressionLiteral:
            {
                Advance();
                var body = token.RawText;
                var lastSlash = body.LastIndexOf('/');

                return new JsRegExpLiteral(
                    span,
                    lastSlash > 0 ? body[1..lastSlash] : string.Empty,
                    lastSlash > 0 && lastSlash + 1 < body.Length ? body[(lastSlash + 1)..] : string.Empty);
            }

            case SliceTokenKind.OpenParen:
            {
                Advance();
                var inner = ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");
                return inner;
            }

            case SliceTokenKind.OpenBracket:
                return ParseArrayLiteral();

            case SliceTokenKind.OpenBrace:
                return ParseObjectLiteral();

            case SliceTokenKind.Function:
                Advance();
                return new JsFunctionExpression(span, ParseFunctionRest(span, declaration: false));

            case SliceTokenKind.Class:
                return new JsClassExpression(span, ParseClass(span, declaration: false));

            case SliceTokenKind.Super:
                return ParseSuper(span);

            case SliceTokenKind.TemplateLiteral:
                return ParseTemplate();

            default:
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`" + Describe(token) + "` begins no expression");

                Advance();
                return new JsNumberLiteral(span, 0, false);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B37576
    // Broiler-Human:        PENDING
    private JsExpression ParseArrayLiteral()
    {
        var span = Span();
        Expect(SliceTokenKind.OpenBracket, "[");
        var elements = new System.Collections.Generic.List<JsExpression?>();

        while (Current.Kind != SliceTokenKind.CloseBracket &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.Comma)
            {
                Advance();
                elements.Add(null);
                continue;
            }

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                var spreadSpan = Span();
                Advance();
                elements.Add(new JsSpreadElement(spreadSpan, ParseAssignment()));
            }
            else
            {
                elements.Add(ParseAssignment());
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBracket, "]");
        return new JsArrayLiteral(span, elements);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4E3F41
    // Broiler-Human:        PENDING
    private JsExpression ParseObjectLiteral()
    {
        var span = Span();
        Expect(SliceTokenKind.OpenBrace, "{");
        var entries = new System.Collections.Generic.List<JsObjectEntry>();

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            entries.Add(ParseObjectEntry());

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        return new JsObjectLiteral(span, entries);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F36B21
    // Broiler-Human:        PENDING
    private JsObjectEntry ParseObjectEntry()
    {
        var span = Span();

        if (Current.Kind == SliceTokenKind.DotDotDot)
        {
            Advance();

            return new JsObjectEntry(
                span, JsPropertyKind.Spread, string.Empty, null, ParseAssignment());
        }

        // A METHOD MODIFIER IS NOT A PROPERTY KEY. `{ *m() {} }` and `{ async m() {} }` reached
        // PropertyKey, which takes any token's text as a name, and the diagnostic a reader got
        // was a missing colon. The discriminator for `async` is the one the accessor branch below
        // already uses: a modifier is followed by a key, while a property named `async` is
        // followed by `:`, `,`, `}` or `(`.
        if (Current.Kind == SliceTokenKind.Star)
        {
            Advance();
            var generatorKey = PropertyKey(out var generatorComputed);
            var outerOperator = yieldIsOperator;
            yieldIsOperator = true;
            var generatorParameters = ParseOrdinaryParameters();

            var generatorBody = ParseFunctionBody(
                span, generatorKey, generatorParameters, isArrow: false, isGenerator: true,
                uniqueParameters: true);

            yieldIsOperator = outerOperator;

            return new JsObjectEntry(
                span,
                JsPropertyKind.Init,
                generatorKey,
                generatorComputed,
                new JsFunctionExpression(span, generatorBody));
        }

        if (Current.Kind == SliceTokenKind.Async &&
            !Peek(1).PrecededByLineTerminator &&
            Peek(1).Kind is not SliceTokenKind.Colon and not SliceTokenKind.Comma and
                not SliceTokenKind.CloseBrace and not SliceTokenKind.OpenParen)
        {
            Advance();

            // THE `*` IS READ HERE AND SETS BOTH BITS. An async generator method's parameter list
            // and body are `[+Yield, +Await]` where an ordinary async method's are `[~Yield,
            // +Await]`, so the flag has to be known before either is parsed - which is why it is
            // read at this point rather than by whatever parses the member.
            var asyncIsGenerator = Current.Kind == SliceTokenKind.Star;

            if (asyncIsGenerator)
            {
                Advance();
            }

            var asyncKey = PropertyKey(out var asyncComputed);
            var outerOperator = yieldIsOperator;
            var outerAwait = awaitIsOperator;
            yieldIsOperator = asyncIsGenerator;
            awaitIsOperator = true;
            var asyncParameters = ParseParameters();

            var asyncBody = ParseFunctionBody(
                span, asyncKey, asyncParameters, isArrow: false, asyncIsGenerator, isAsync: true,
                uniqueParameters: true);

            yieldIsOperator = outerOperator;
            awaitIsOperator = outerAwait;

            return new JsObjectEntry(
                span,
                JsPropertyKind.Init,
                asyncKey,
                asyncComputed,
                new JsFunctionExpression(span, asyncBody),
                IsMethod: true);
        }

        if (Current.Kind is SliceTokenKind.Get or SliceTokenKind.Set &&
            Peek(1).Kind is not SliceTokenKind.Colon and not SliceTokenKind.Comma and
            not SliceTokenKind.CloseBrace and not SliceTokenKind.OpenParen)
        {
            var kind = Current.Kind == SliceTokenKind.Get ? JsPropertyKind.Get : JsPropertyKind.Set;
            Advance();
            var accessorKey = PropertyKey(out var accessorComputed);
            var parameters = ParseOrdinaryParameters();
            ValidateAccessorParameters(kind == JsPropertyKind.Get, parameters, span);

            var body = ParseFunctionBody(
                span, accessorKey, parameters, isArrow: false, uniqueParameters: true);

            return new JsObjectEntry(
                span,
                kind,
                accessorKey,
                accessorComputed,
                new JsFunctionExpression(span, body),
                IsMethod: true);
        }

        var keyToken = Current;
        var key = PropertyKey(out var computed);

        if (Current.Kind == SliceTokenKind.OpenParen)
        {
            var parameters = ParseOrdinaryParameters();

            var body = ParseFunctionBody(
                span, key, parameters, isArrow: false, uniqueParameters: true);

            return new JsObjectEntry(
                span,
                JsPropertyKind.Init,
                key,
                computed,
                new JsFunctionExpression(span, body),
                IsMethod: true);
        }

        if (Current.Kind is SliceTokenKind.Comma or SliceTokenKind.CloseBrace)
        {
            // A shorthand property: `{ x }` is `{ x: x }`, and the key is a name in scope. It is
            // recorded AS a shorthand because the equivalence has one exception: `{ __proto__ }`
            // defines a property where `{ __proto__: p }` sets the prototype.
            //
            // AND BECAUSE THE KEY IS ALSO A NAME, a reserved word reached through an escape is
            // refused here as it is in every other Identifier position. `{ break: 42 }` is a
            // property and `{ break }` is a reference to a binding no source can have.
            _ = RefuseEscapedReservedWord(keyToken);

            return new JsObjectEntry(
                span, JsPropertyKind.Init, key, computed, new JsIdentifier(span, key),
                Shorthand: true);
        }

        // `{ a = 1 }` IS NOT AN OBJECT LITERAL AND MAY STILL BE A LEGAL PROGRAM. It is the cover
        // grammar of `({ a = 1 } = o)`, and whether this brace is a literal or a pattern is not
        // settled until the token after the closing one. Refusing it here would refuse the
        // assignment too; the entry is marked instead and the LOWERING refuses whichever of these
        // reached it, which is exactly the set that was never reinterpreted.
        if (Current.Kind == SliceTokenKind.Equals && computed is null && IsIdentifierName(keyToken.Kind))
        {
            _ = RefuseEscapedReservedWord(keyToken);
            Advance();

            return new JsObjectEntry(
                span,
                JsPropertyKind.Init,
                key,
                null,
                new JsAssignmentExpression(
                    span, SliceTokenKind.Equals, new JsIdentifier(span, key), ParseAssignment()),
                Cover: true);
        }

        Expect(SliceTokenKind.Colon, ":");
        return new JsObjectEntry(span, JsPropertyKind.Init, key, computed, ParseAssignment());
    }

    /// <summary>Parses what follows a <c>.</c> or a <c>?.</c>, which is one of two things.</summary>
    /// <remarks>
    /// <b>A private access is a different node and not a member access with a <c>#</c> in its
    /// name</b>, and the two are told apart here because this is the only position where both are
    /// grammatical. What follows from the choice is not cosmetic: <c>o.x</c> answers
    /// <c>undefined</c> for a name nothing defined and <c>o.#x</c> is a <c>TypeError</c>, so a
    /// single node would have had to re-derive the difference from the first character of a string
    /// at every use.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=8DB9AA
    // Broiler-Human:        PENDING
    private JsExpression AfterDot(SliceSourceSpan span, JsExpression target, bool optional)
    {
        if (!IsPrivateName(Current))
        {
            return new JsMemberExpression(span, target, MemberName(), null, optional);
        }

        var name = Current.RawText;
        Advance();
        return new JsPrivateMemberExpression(span, target, name, optional);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=99DC6F
    // Broiler-Human:        PENDING
    private string PropertyKey(out JsExpression? computed)
    {
        computed = null;
        var token = Current;

        switch (token.Kind)
        {
            case SliceTokenKind.StringLiteral:
                Advance();
                return token.StringValue;

            case SliceTokenKind.NumericLiteral:
                Advance();
                return NumberKey(token.NumericValue);

            case SliceTokenKind.OpenBracket:
            {
                Advance();
                computed = ParseExpression();
                Expect(SliceTokenKind.CloseBracket, "]");
                return string.Empty;
            }

            default:
                Advance();
                return token.RawText;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=673640
    // Broiler-Human:        PENDING
    private static string NumberKey(double value) =>
        value == System.Math.Floor(value) && !double.IsInfinity(value) &&
        value >= 0 && value < 4294967295
            ? ((uint)value).ToString(System.Globalization.CultureInfo.InvariantCulture)
            : value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DC0C0B
    // Broiler-Human:        PENDING
    private string MemberName()
    {
        var token = Current;

        // A PRIVATE NAME IS A SYNTAX ERROR HERE AND NOT AN UNADMITTED CONSTRUCT. This is the
        // OBJECT LITERAL's key, and `({ #x: 1 })` is a program no edition of the language has ever
        // had a meaning for - the grammar admits a private name in a class body and after a `.`
        // and nowhere else. Refusing it against the manifest would say the surface is missing when
        // what is missing is a production.
        if (IsPrivateName(token))
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a private name belongs to a class body and cannot be a property key here");
        }

        Advance();
        return token.RawText;
    }

    /// <summary>Whether a token is a private name, which this manifest admits nowhere.</summary>
    /// <remarks>
    /// <para>
    /// The tokenizer gives <c>#x</c> the identifier kind deliberately - an escaped keyword and a
    /// private name are both "an identifier spelled oddly" as far as scanning goes - so the leading
    /// <c>#</c> is what tells them apart, and this is the one place that asks.
    /// </para>
    /// <para>
    /// <b>A lone <c>#</c> is not a private name and must not be refused as one.</b> It is what a
    /// hashbang written anywhere but at offset zero scans as, and that source is a syntax error
    /// about a character rather than a construct this manifest declines - which is the difference
    /// the acceptance suite's second hashbang row exists to hold.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7729F3
    // Broiler-Human:        PENDING
    private static bool IsPrivateName(SliceToken token) =>
        token.RawText.Length > 1 && token.RawText[0] == '#';

    /// <summary>
    /// Whether <paramref name="name"/> is one of the two names strict code neither binds nor
    /// assigns to.
    /// </summary>
    /// <remarks>
    /// <b>They are restricted rather than reserved, and the difference is what makes this a test on
    /// a string rather than a token kind.</b> `eval` and `arguments` are ordinary identifiers in
    /// every position that only READS them - `"use strict"; eval("1")` and `arguments.length` are
    /// programs - and are refused exactly where a program would change what they name: a binding
    /// that shadows one, and an assignment that replaces one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7C2BA8
    // Broiler-Human:        PENDING
    private static bool IsRestrictedInStrictCode(string name) =>
        string.Equals(name, "eval", System.StringComparison.Ordinal) ||
        string.Equals(name, "arguments", System.StringComparison.Ordinal);

    /// <summary>
    /// Refuses an identifier that reaches a word this context reserves through a unicode escape.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>An escape changes how a name is written and not what it is, which is exactly why this
    /// rule exists.</b> The tokenizer resolves the escape, so <c>await</c> spelled with one and
    /// <c>await</c> spelled without one are the same characters by the time the parser sees them -
    /// and the language nevertheless refuses the escaped form wherever the plain one is reserved,
    /// so that a program cannot smuggle a keyword into an identifier position. The suite writes the
    /// case out for every reserved word and every position, which is why answering it is worth a
    /// bit on the token.
    /// </para>
    /// <para>
    /// <b>It asks the SAME predicate every ordinary identifier position asks</b>, so a word that is
    /// a name here stays a name here however it was spelled: <c>await</c> in a sloppy script is an
    /// ordinary identifier and its escaped spelling is one too, while the same two spellings inside
    /// an async function are both refused. The two exceptions the predicate does not cover are
    /// <c>let</c> and <c>static</c>, which strict code reserves and which
    /// <see cref="IsIdentifierName"/> answers for as names because a property key may spell them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=286654
    // Broiler-Human:        PENDING
    private bool RefuseEscapedReservedWord(SliceToken token)
    {
        if (!token.IsEscaped)
        {
            return false;
        }

        var spelled = SliceTokenizer.KeywordKind(token.RawText);

        if (spelled == SliceTokenKind.Identifier ||
            (IsIdentifierName(spelled) &&
                !(strict && spelled is SliceTokenKind.Let or SliceTokenKind.Static)))
        {
            return false;
        }

        Refuse(
            new SliceSourceSpan(token.Line, token.Column),
            SliceSourceDiagnosticCode.ReservedWordAsBinding,
            "`" + token.RawText + "` is a reserved word here, and a unicode escape does not make " +
                "it a name");

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B80262
    // Broiler-Human:        PENDING
    private string BindingName()
    {
        var token = Current;

        if (IsIdentifierName(token.Kind))
        {
            _ = RefuseEscapedReservedWord(token);

            // STRICT CODE BINDS NEITHER `eval` NOR `arguments`, and this is the one funnel every
            // binding name passes through - a declarator, a parameter, a catch parameter, a
            // pattern's leaf and an import's local name - so the rule is stated once. It is an
            // EARLY error and not a manifest refusal: the manifest admits `var`, and a program
            // that writes `"use strict"; var eval = 1;` is wrong about the language.
            if (strict && IsRestrictedInStrictCode(token.RawText))
            {
                Refuse(
                    Span(),
                    SliceSourceDiagnosticCode.ReservedWordAsBinding,
                    "`" + token.RawText + "` is not a binding name in strict code");
            }

            // AND STRICT CODE BINDS NO `let` EITHER, which is a RESERVATION rather than the
            // restriction above: `let` is a contextual keyword in sloppy code, where `var let = 1;`
            // is a program every engine runs, and strict code reserves the word outright. The test
            // is on the token kind rather than the text because the tokenizer already recognises
            // the word, and it is here rather than in `IsIdentifierName` because that predicate
            // also answers for a property key and a member name, which strict code spells freely.
            if (strict && token.Kind == SliceTokenKind.Let)
            {
                Refuse(
                    Span(),
                    SliceSourceDiagnosticCode.ReservedWordAsBinding,
                    "`let` is a reserved word in strict code and is not a binding name");
            }

            Advance();
            return token.RawText;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ReservedWordAsBinding,
            "`" + Describe(token) + "` is not a binding name");

        Advance();
        return "#invalid";
    }

    // ---- templates -----------------------------------------------------------------------------

    /// <summary>Splits the template at the cursor into its chunks and its substitutions.</summary>
    /// <remarks>
    /// <para>
    /// <b>The tokenizer hands a template over WHOLE - one token, backtick to backtick - so this is
    /// where it is taken apart.</b> That is a deliberate division and not an oversight: the slice
    /// front end, which shares the tokenizer, admits no template of any shape and only needs to
    /// count one, so a tokenizer that split templates into parts would have to invent a part token
    /// that one of its two consumers has no use for. The cost is that the substitutions are scanned
    /// a second time here, bounded by the length of the template itself.
    /// </para>
    /// <para>
    /// <b>Both spellings of every chunk are kept, because a tagged template needs both.</b> The
    /// cooked chunk is what the escapes mean and the raw chunk is what they were written as, and
    /// the one normalisation applied to both is that a carriage return - alone or before a line
    /// feed - becomes a line feed, which is what the language says the value of a template line
    /// break is regardless of how the file was saved.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BF0E92
    // Broiler-Human:        PENDING
    private JsTemplateLiteral ParseTemplate()
    {
        var token = Current;
        var span = new SliceSourceSpan(token.Line, token.Column);
        Advance();

        var cooked = new System.Collections.Generic.List<string>();
        var raw = new System.Collections.Generic.List<string>();
        var substitutions = new System.Collections.Generic.List<JsExpression>();
        var reader = new TemplateReader(token.RawText, token.Line, token.Column);
        var chunk = new System.Text.StringBuilder();

        reader.Step();
        var chunkStart = reader.At;

        while (!reader.AtEnd)
        {
            var c = reader.Current;

            if (c == '\\')
            {
                if (!CookEscape(reader, chunk, span))
                {
                    return new JsTemplateLiteral(span, [string.Empty], [string.Empty], []);
                }

                continue;
            }

            if (c == '`')
            {
                break;
            }

            if (c == '$' && reader.Peek(1) == '{')
            {
                raw.Add(Normalise(reader.Slice(chunkStart, reader.At)));
                cooked.Add(chunk.ToString());
                chunk.Clear();
                reader.Step();
                reader.Step();

                var innerLine = reader.Line;
                var innerColumn = reader.Column;
                var innerStart = reader.At;
                reader.ScanSubstitution();
                var innerEnd = System.Math.Max(innerStart, reader.At - 1);

                substitutions.Add(
                    ParseInterpolation(reader.Slice(innerStart, innerEnd), innerLine, innerColumn));

                chunkStart = reader.At;
                continue;
            }

            if (c == '\r')
            {
                chunk.Append('\n');
                reader.Step();
                continue;
            }

            chunk.Append(c);
            reader.Step();
        }

        raw.Add(Normalise(reader.Slice(chunkStart, reader.At)));
        cooked.Add(chunk.ToString());
        return new JsTemplateLiteral(span, cooked, raw, substitutions);
    }

    /// <summary>Reads one escape of a template chunk, appending what it means.</summary>
    /// <remarks>
    /// The same rule a string literal follows, and for the same reason: every escape the language
    /// does not name is the character itself, so <c>\d</c> is <c>d</c> and a backslash before a
    /// line break is a continuation that contributes nothing at all. Only <c>\x</c> and <c>\u</c>
    /// can be malformed, because only those two promise digits they may not have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=441AD8
    // Broiler-Human:        PENDING
    private bool CookEscape(
        TemplateReader reader, System.Text.StringBuilder cooked, SliceSourceSpan span)
    {
        reader.Step();

        if (reader.AtEnd)
        {
            return true;
        }

        var escape = reader.Current;

        switch (escape)
        {
            case 'n': cooked.Append('\n'); reader.Step(); return true;
            case 't': cooked.Append('\t'); reader.Step(); return true;
            case 'r': cooked.Append('\r'); reader.Step(); return true;
            case 'b': cooked.Append('\b'); reader.Step(); return true;
            case 'f': cooked.Append('\f'); reader.Step(); return true;
            case 'v': cooked.Append('\v'); reader.Step(); return true;
            case '0': cooked.Append('\0'); reader.Step(); return true;

            case 'x':
            {
                reader.Step();
                var value = 0;

                for (var digit = 0; digit < 2; digit++)
                {
                    if (reader.AtEnd || !System.Uri.IsHexDigit(reader.Current))
                    {
                        Refuse(
                            span,
                            SliceSourceDiagnosticCode.UnknownEscapeSequence,
                            "a hexadecimal escape with fewer than two digits");

                        return false;
                    }

                    value = (value * 16) + System.Uri.FromHex(reader.Current);
                    reader.Step();
                }

                cooked.Append((char)value);
                return true;
            }

            case 'u':
            {
                reader.Step();

                if (!ReadUnicodeEscape(reader, out var scalar))
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.UnknownEscapeSequence,
                        "a unicode escape that names no code point");

                    return false;
                }

                AppendScalar(cooked, scalar);
                return true;
            }

            default:
                if (escape is '\n' or '\r' or '\u2028' or '\u2029')
                {
                    reader.Step();
                    return true;
                }

                cooked.Append(escape);
                reader.Step();
                return true;
        }
    }

    /// <summary>Reads the body of a <c>\u</c> escape, braced or not.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1FE911
    // Broiler-Human:        PENDING
    private static bool ReadUnicodeEscape(TemplateReader reader, out int scalar)
    {
        scalar = 0;

        if (!reader.AtEnd && reader.Current == '{')
        {
            reader.Step();
            var digits = 0;

            while (!reader.AtEnd && reader.Current != '}')
            {
                if (!System.Uri.IsHexDigit(reader.Current))
                {
                    return false;
                }

                scalar = (scalar * 16) + System.Uri.FromHex(reader.Current);
                digits++;

                if (scalar > 0x10FFFF)
                {
                    return false;
                }

                reader.Step();
            }

            if (digits == 0 || reader.AtEnd)
            {
                return false;
            }

            reader.Step();
            return true;
        }

        for (var digit = 0; digit < 4; digit++)
        {
            if (reader.AtEnd || !System.Uri.IsHexDigit(reader.Current))
            {
                return false;
            }

            scalar = (scalar * 16) + System.Uri.FromHex(reader.Current);
            reader.Step();
        }

        return true;
    }

    /// <summary>Appends one code point as UTF-16, a lone surrogate included.</summary>
    /// <remarks>
    /// <c>char.ConvertFromUtf32</c> is not usable here: <c>\uD800</c> is a legal escape naming a
    /// lone surrogate, and a JavaScript String is a sequence of code UNITS that may hold one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C1BAF6
    // Broiler-Human:        PENDING
    private static void AppendScalar(System.Text.StringBuilder cooked, int scalar)
    {
        if (scalar <= 0xFFFF)
        {
            cooked.Append((char)scalar);
            return;
        }

        var shifted = scalar - 0x10000;
        cooked.Append((char)(0xD800 + (shifted >> 10)));
        cooked.Append((char)(0xDC00 + (shifted & 0x3FF)));
    }

    /// <summary>Turns every carriage return of a raw chunk into the line feed the language sees.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4D167D
    // Broiler-Human:        PENDING
    private static string Normalise(string text) =>
        text.IndexOf('\r', System.StringComparison.Ordinal) < 0
            ? text
            : text
                .Replace("\r\n", "\n", System.StringComparison.Ordinal)
                .Replace("\r", "\n", System.StringComparison.Ordinal);

    /// <summary>Parses one substitution, whose text is a slice of the source this parser was given.</summary>
    /// <remarks>
    /// <para>
    /// <b>Every token it produces is moved back to where it really is.</b> The substitution is
    /// tokenized on its own, so its first line begins at column one - and a diagnostic reported at
    /// that column would name a position in a file the reader is looking at and point at the wrong
    /// character. The first line is shifted by the column the substitution starts at and every
    /// later line by the line count alone, which is exactly the arithmetic of pasting a slice back
    /// into the text it came out of.
    /// </para>
    /// <para>
    /// <b>The strictness and the function nesting cross the boundary with it</b>, because both
    /// change the GRAMMAR and neither is recoverable from the slice: <c>`${yield}`</c> in strict
    /// code is a reserved word and <c>`${new.target}`</c> is admitted only where a function
    /// encloses it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E2FD0F
    // Broiler-Human:        PENDING
    private JsExpression ParseInterpolation(string text, int line, int column)
    {
        var tokenizer = new SliceTokenizer(text);
        var stream = tokenizer.Tokenize();

        if (tokenizer.Diagnostics.Count != 0)
        {
            foreach (var diagnostic in tokenizer.Diagnostics)
            {
                Refuse(
                    Move(new SliceSourceSpan(diagnostic.Line, diagnostic.Column), line, column),
                    diagnostic.Code,
                    diagnostic.Message);
            }

            return new JsNullLiteral(new SliceSourceSpan(line, column));
        }

        for (var index = 0; index < stream.Length; index++)
        {
            var token = stream[index];
            var moved = Move(new SliceSourceSpan(token.Line, token.Column), line, column);
            stream[index] = token with { Line = moved.Line, Column = moved.Column };
        }

        var inner = new JsParser(
            stream, options, strict, functionDepth, yieldIsOperator, awaitIsOperator);
        var value = inner.ParseInterpolationBody();

        foreach (var diagnostic in inner.Diagnostics)
        {
            Refuse(
                new SliceSourceSpan(diagnostic.Line, diagnostic.Column),
                diagnostic.Code,
                diagnostic.Message);
        }

        return value;
    }

    /// <summary>Where a position inside a substitution's own text is in the whole source.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=105752
    // Broiler-Human:        PENDING
    private static SliceSourceSpan Move(SliceSourceSpan inner, int line, int column) =>
        inner.Line <= 1
            ? new SliceSourceSpan(line, column + inner.Column - 1)
            : new SliceSourceSpan(line + inner.Line - 1, inner.Column);

    /// <summary>Parses the one expression a substitution is, and nothing after it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D4EC22
    // Broiler-Human:        PENDING
    private JsExpression ParseInterpolationBody()
    {
        var span = Span();

        if (Current.Kind == SliceTokenKind.EndOfSource)
        {
            Refuse(
                span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a template substitution holds no expression");

            return new JsNullLiteral(span);
        }

        var value = ParseExpression();

        if (Current.Kind != SliceTokenKind.EndOfSource && diagnostics.Count == 0)
        {
            Refuse(
                Span(),
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`" + Describe(Current) + "` follows the expression of a template substitution");
        }

        return value;
    }

    // ---- the cover grammar ---------------------------------------------------------------------

    /// <summary>
    /// Reinterprets an already-parsed literal as the assignment pattern it turned out to be.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Reinterpretation rather than a speculative parse, and the choice is forced.</b>
    /// <c>[a, b]</c> and <c>[a, b] = c</c> are the same characters until the <c>=</c>, and no
    /// bounded lookahead settles it: the left-hand side can be arbitrarily long and can contain
    /// arbitrary expressions. Parsing it twice would mean an unbounded re-parse and a source with
    /// nested literals could make it quadratic. So it is parsed once as a literal and rewritten in
    /// place, which is exactly what the specification's cover grammar prescribes.
    /// </para>
    /// <para>
    /// <b>The leaves an ASSIGNMENT pattern admits are wider than a declaration's</b> - <c>o.x</c>
    /// and <c>a[i]</c> are references and are legal here - which is why this is a separate entry
    /// point from <see cref="ParseBindingPattern"/> rather than a flag on it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7D89FB
    // Broiler-Human:        PENDING
    private JsPattern ToPattern(JsExpression expression)
    {
        switch (expression)
        {
            case JsArrayLiteral array:
            {
                var elements = new System.Collections.Generic.List<JsPatternElement?>();
                JsPattern? rest = null;

                for (var index = 0; index < array.Elements.Count; index++)
                {
                    var element = array.Elements[index];

                    if (element is null)
                    {
                        elements.Add(null);
                        continue;
                    }

                    if (element is JsSpreadElement spread)
                    {
                        if (index != array.Elements.Count - 1)
                        {
                            Refuse(
                                spread.Span,
                                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                                "a rest element is admitted only as the last element of a pattern");
                        }

                        rest = ToPattern(spread.Argument);
                        continue;
                    }

                    elements.Add(ToPatternElement(element));
                }

                return new JsArrayPattern(array.Span, elements, rest);
            }

            case JsObjectLiteral literal:
            {
                var properties = new System.Collections.Generic.List<JsPatternProperty>();
                JsPattern? rest = null;

                for (var index = 0; index < literal.Entries.Count; index++)
                {
                    var entry = literal.Entries[index];

                    if (entry.Kind == JsPropertyKind.Spread)
                    {
                        if (index != literal.Entries.Count - 1)
                        {
                            Refuse(
                                entry.Span,
                                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                                "a rest property is admitted only as the last entry of a pattern");
                        }

                        rest = ToPattern(entry.Value);
                        continue;
                    }

                    if (entry.Kind != JsPropertyKind.Init)
                    {
                        Refuse(
                            entry.Span,
                            SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                            "an accessor is not an assignment target");

                        continue;
                    }

                    properties.Add(new JsPatternProperty(
                        entry.Span, entry.Key, entry.Computed, ToPatternElement(entry.Value)));
                }

                return new JsObjectPattern(literal.Span, properties, rest);
            }

            case JsIdentifier:
            case JsMemberExpression:

            // `({ a: this.#x } = o)` IS A DESTRUCTURING ASSIGNMENT and its target is a reference
            // like any other. It was refused here while every other position admitted the access,
            // so a program could write `this.#x = o.a` and not the destructuring that means the
            // same thing.
            case JsPrivateMemberExpression:
                return new JsTargetPattern(expression.Span, expression);

            default:
                Refuse(
                    expression.Span,
                    SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                    "the left-hand side of a destructuring assignment is not a reference");

                return new JsTargetPattern(expression.Span, expression);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7DE10D
    // Broiler-Human:        PENDING
    private JsPatternElement ToPatternElement(JsExpression expression) =>
        expression is JsAssignmentExpression { Operator: SliceTokenKind.Equals } assignment
            ? new JsPatternElement(
                expression.Span, ToPattern(assignment.Target), assignment.Value)
            : new JsPatternElement(expression.Span, ToPattern(expression), null);

    // ---- token plumbing ------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0ABBA3
    // Broiler-Human:        PENDING
    private SliceToken Current => tokens[at];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DE9B1C
    // Broiler-Human:        PENDING
    private SliceToken Peek(int ahead) =>
        at + ahead < tokens.Length ? tokens[at + ahead] : tokens[^1];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B440C1
    // Broiler-Human:        PENDING
    private void Advance()
    {
        if (at < tokens.Length - 1)
        {
            at++;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D32F16
    // Broiler-Human:        PENDING
    private SliceSourceSpan Span() => new(Current.Line, Current.Column);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=EE2BCB
    // Broiler-Human:        PENDING
    private void Expect(SliceTokenKind kind, string text)
    {
        if (Current.Kind == kind)
        {
            Advance();
            return;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ExpectedToken,
            "`" + text + "` was expected and `" + Describe(Current) + "` was found");
    }

    /// <summary>Consumes a statement's terminating semicolon, inserting one where the rules allow.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C0B453
    // Broiler-Human:        PENDING
    private void Semicolon()
    {
        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
            return;
        }

        if (Current.Kind is SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource ||
            Current.PrecededByLineTerminator)
        {
            return;
        }

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.ExpectedToken,
            "`;` was expected and `" + Describe(Current) + "` was found");
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D1B2C4
    // Broiler-Human:        PENDING
    private bool Enter(out JsStatement refusal)
    {
        depth++;

        if (depth <= options.MaximumNestingDepth)
        {
            refusal = null!;
            return true;
        }

        depth--;
        var span = Span();

        Refuse(
            span,
            SliceSourceDiagnosticCode.NestingTooDeep,
            "the source nests deeper than the " + options.MaximumNestingDepth +
                " levels these parse options allow");

        refusal = new JsEmptyStatement(span);
        return false;
    }

    /// <summary>The same nesting guard <see cref="Enter"/> applies, for a production with no node.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9C3B5D
    // Broiler-Human:        PENDING
    private bool EnterNesting()
    {
        depth++;

        if (depth <= options.MaximumNestingDepth)
        {
            return true;
        }

        depth--;

        Refuse(
            Span(),
            SliceSourceDiagnosticCode.NestingTooDeep,
            "the source nests deeper than the " + options.MaximumNestingDepth +
                " levels these parse options allow");

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CD54D0
    // Broiler-Human:        PENDING
    private JsStatement OutsideStatement(SliceSourceSpan span, string what)
    {
        Refuse(span, what);
        return new JsEmptyStatement(span);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5A818B
    // Broiler-Human:        PENDING
    private JsExpression OutsideExpression(SliceSourceSpan span, string what)
    {
        Refuse(span, what);
        return new JsNullLiteral(span);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BDC6EB
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, string what) =>
        Refuse(
            span,
            SliceSourceDiagnosticCode.ConstructOutsideManifest,
            what + " is not admitted by the declared feature manifest");

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3032B3
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, SliceSourceDiagnosticCode code, string message)
    {
        if (diagnostics.Count >= 64)
        {
            return;
        }

        diagnostics.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F22A9C
    // Broiler-Human:        PENDING
    private static SliceTokenKind CompoundOperator(string text) => text switch
    {
        "+=" => SliceTokenKind.Plus,
        "-=" => SliceTokenKind.Minus,
        "*=" => SliceTokenKind.Star,
        "/=" => SliceTokenKind.Slash,
        "%=" => SliceTokenKind.Percent,
        "<<=" => SliceTokenKind.LessThanLessThan,
        ">>=" => SliceTokenKind.GreaterThanGreaterThan,
        ">>>=" => SliceTokenKind.GreaterThanGreaterThanGreaterThan,
        "&=" => SliceTokenKind.Ampersand,
        "|=" => SliceTokenKind.Bar,
        "^=" => SliceTokenKind.Caret,
        "**=" => SliceTokenKind.StarStar,
        "&&=" => SliceTokenKind.AmpersandAmpersand,
        "||=" => SliceTokenKind.BarBar,
        "??=" => SliceTokenKind.QuestionQuestion,
        _ => SliceTokenKind.Equals,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AB2B33
    // Broiler-Human:        PENDING
    private static string Describe(SliceToken token) =>
        token.Kind == SliceTokenKind.EndOfSource ? "end of source" : token.RawText;

    /// <summary>
    /// A cursor over one template's raw text that knows where in the whole source it is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It repeats the tokenizer's substitution scan on purpose, and the repetition is the
    /// contract.</b> The tokenizer already found where this template ends, using brace counting
    /// that skips a string, a comment, a regular expression and a nested template whole; if this
    /// cursor disagreed with it about where a substitution ends, the two would carve the same text
    /// differently and the parse would be of something the tokenizer never saw. So the rules here
    /// are the tokenizer's rules, including its <c>/</c> heuristic - and where that heuristic is
    /// wrong, both are wrong together, which is a defect a reader can find rather than a
    /// disagreement they cannot.
    /// </para>
    /// <para>
    /// <b>It counts lines while it goes because the substitutions need it.</b> A diagnostic inside
    /// <c>`${x +</c> … <c>}`</c> that spans lines has to name the line it is really on, and the only
    /// place that is known is here, one character at a time, with CRLF counted once.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=09F45E
    // Broiler-Falsified-If: this cursor ends a substitution at a different character than the tokenizer did
    // Broiler-Human:        PENDING
    private sealed class TemplateReader(string text, int line, int column)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BDDF6F
        // Broiler-Human:        PENDING
        private readonly string source = text;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=98C321
        // Broiler-Human:        PENDING
        private int at;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F54545
        // Broiler-Human:        PENDING
        private int currentLine = line;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=53EEF2
        // Broiler-Human:        PENDING
        private int currentColumn = column;

        /// <summary>How far into the template's text the cursor is.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=992608
        // Broiler-Human:        PENDING
        internal int At => at;

        /// <summary>The line of the whole source the cursor is on.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C5FF9A
        // Broiler-Human:        PENDING
        internal int Line => currentLine;

        /// <summary>The column of the whole source the cursor is at.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=02AF1F
        // Broiler-Human:        PENDING
        internal int Column => currentColumn;

        /// <summary>Whether the cursor has run out of template.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=680D30
        // Broiler-Human:        PENDING
        internal bool AtEnd => at >= source.Length;

        /// <summary>The character under the cursor.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4020AA
        // Broiler-Human:        PENDING
        internal char Current => source[at];

        /// <summary>The character <paramref name="ahead"/> past the cursor, or NUL past the end.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3B5C11
        // Broiler-Human:        PENDING
        internal char Peek(int ahead) => at + ahead < source.Length ? source[at + ahead] : '\0';

        /// <summary>The template's text between two cursor positions.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6A62E5
        // Broiler-Human:        PENDING
        internal string Slice(int start, int end) =>
            source[System.Math.Min(start, source.Length)..System.Math.Min(end, source.Length)];

        /// <summary>Advances one character, counting CRLF as the one line break it is.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=79BAEA
        // Broiler-Human:        PENDING
        internal void Step()
        {
            if (at >= source.Length)
            {
                return;
            }

            var c = source[at];

            if (c is '\n' or '\r' or '\u2028' or '\u2029')
            {
                if (c == '\r' && at + 1 < source.Length && source[at + 1] == '\n')
                {
                    at++;
                }

                at++;
                currentLine++;
                currentColumn = 1;
                return;
            }

            at++;
            currentColumn++;
        }

        /// <summary>Advances past the brace that closes the substitution the cursor is inside.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=90E31B
        // Broiler-Falsified-If: a brace inside a string, a comment, a nested template or an object literal closes the substitution
        // Broiler-Human:        PENDING
        internal void ScanSubstitution()
        {
            var braces = 0;
            var previous = '\0';

            while (!AtEnd)
            {
                var c = Current;

                if (c == '/' && Peek(1) == '/')
                {
                    while (!AtEnd && Current is not '\n' and not '\r' and not '\u2028' and not '\u2029')
                    {
                        Step();
                    }

                    continue;
                }

                if (c == '/' && Peek(1) == '*')
                {
                    Step();
                    Step();

                    while (!AtEnd && !(Current == '*' && Peek(1) == '/'))
                    {
                        Step();
                    }

                    Step();
                    Step();
                    continue;
                }

                if (c == '/' && StartsRegularExpression(previous))
                {
                    ScanRegularExpression();
                    previous = '/';
                    continue;
                }

                if (c is '"' or '\'')
                {
                    ScanString(c);
                    previous = c;
                    continue;
                }

                if (c == '`')
                {
                    Step();
                    ScanTemplateBody();
                    previous = '`';
                    continue;
                }

                if (c == '{')
                {
                    braces++;
                    Step();
                    previous = '{';
                    continue;
                }

                if (c == '}')
                {
                    Step();

                    if (braces == 0)
                    {
                        return;
                    }

                    braces--;
                    previous = '}';
                    continue;
                }

                if (!char.IsWhiteSpace(c))
                {
                    previous = c;
                }

                Step();
            }
        }

        /// <summary>Advances past the backtick that closes a nested template.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B8B0F5
        // Broiler-Human:        PENDING
        private void ScanTemplateBody()
        {
            while (!AtEnd)
            {
                var c = Current;

                if (c == '\\')
                {
                    Step();
                    Step();
                    continue;
                }

                if (c == '`')
                {
                    Step();
                    return;
                }

                if (c == '$' && Peek(1) == '{')
                {
                    Step();
                    Step();
                    ScanSubstitution();
                    continue;
                }

                Step();
            }
        }

        /// <summary>Advances past the quote that closes a string literal.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8183F6
        // Broiler-Human:        PENDING
        private void ScanString(char quote)
        {
            Step();

            while (!AtEnd)
            {
                var c = Current;

                if (c == '\\')
                {
                    Step();
                    Step();
                    continue;
                }

                if (c == quote)
                {
                    Step();
                    return;
                }

                if (c is '\n' or '\r' or '\u2028' or '\u2029')
                {
                    return;
                }

                Step();
            }
        }

        /// <summary>Advances past a regular-expression literal and its flags.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F86139
        // Broiler-Human:        PENDING
        private void ScanRegularExpression()
        {
            Step();
            var inClass = false;

            while (!AtEnd)
            {
                var c = Current;

                if (c is '\n' or '\r' or '\u2028' or '\u2029')
                {
                    return;
                }

                if (c == '\\')
                {
                    Step();
                    Step();
                    continue;
                }

                if (c == '[')
                {
                    inClass = true;
                }
                else if (c == ']')
                {
                    inClass = false;
                }
                else if (c == '/' && !inClass)
                {
                    Step();

                    while (!AtEnd && (char.IsLetterOrDigit(Current) || Current is '$' or '_'))
                    {
                        Step();
                    }

                    return;
                }

                Step();
            }
        }

        /// <summary>
        /// Whether a <c>/</c> after this character opens a regular expression rather than dividing.
        /// </summary>
        /// <remarks>
        /// The tokenizer's own heuristic, character for character. It is consulted only to find
        /// where a substitution ends, and both scans have to reach the same answer or they carve
        /// the source differently.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=17FBC0
        // Broiler-Human:        PENDING
        private static bool StartsRegularExpression(char previous) =>
            previous is '\0' or '(' or ',' or '=' or ':' or '[' or '!' or '&' or '|' or '?' or
                '{' or '}' or ';' or '+' or '-' or '*' or '%' or '<' or '>' or '~' or '^';
    }
}
