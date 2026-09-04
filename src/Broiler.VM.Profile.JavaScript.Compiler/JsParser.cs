// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   94
// Annotated:        94/94
// Exempt:           15
// Human-reviewed:   0/94
// IP risk:          None
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       94
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
/// <c>async</c> function, module declaration, <c>with</c>, class field, private name, class static
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

    /// <summary>Parses a whole program.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B8251A
    // Broiler-Human:        PENDING
    internal JsProgramNode Parse()
    {
        var span = Span();
        var directives = ParseDirectives();

        if (options.Goal == SliceGoal.Module)
        {
            strict = true;
        }

        // NOTHING TURNS STRICTNESS OFF. A directive prologue can only add it, the module goal can
        // only add it, and a caller that imposed it keeps it - so a `"use strict"` inside a
        // function cannot be undone by an inner function without one, and neither can this.

        var body = new System.Collections.Generic.List<JsStatement>();

        while (Current.Kind != SliceTokenKind.EndOfSource && diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        return new JsProgramNode(span, directives, body, strict);
    }

    // ---- statements ----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7CC575
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
                case SliceTokenKind.Let when Peek(1).Kind is SliceTokenKind.Identifier or
                    SliceTokenKind.Let or SliceTokenKind.Get or SliceTokenKind.Set or
                    SliceTokenKind.Of or SliceTokenKind.Async or SliceTokenKind.Static or
                    SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace:
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
                    return OutsideStatement(span, "the `with` statement");

                case SliceTokenKind.Class:
                    return new JsClassDeclaration(span, ParseClass(span, declaration: true));

                case SliceTokenKind.Import:
                case SliceTokenKind.Export:
                    return OutsideStatement(span, "a module declaration");

                // AN ASYNC GENERATOR IS STILL REFUSED AND THE TEST FOR IT COMES FIRST. The two
                // constructs begin identically and only the `*` after `function` tells them apart,
                // so an arm that parsed on `async function` alone would have swallowed the
                // refusal - and a family that stops being refused by name comes back as a surprise
                // token, which is the failure the audit exists to catch.
                case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                    !Peek(1).PrecededByLineTerminator && Peek(2).Kind == SliceTokenKind.Star:
                    return OutsideStatement(span, "an async generator function");

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
                    var label = Current.RawText;
                    Advance();
                    Advance();
                    return new JsLabelledStatement(span, label, ParseStatement());
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=13C0B4
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsStringLiteral> ParseDirectives()
    {
        var directives = new System.Collections.Generic.List<JsStringLiteral>();

        while (Current.Kind == SliceTokenKind.StringLiteral &&
            Peek(1).Kind is SliceTokenKind.Semicolon or SliceTokenKind.EndOfSource ||
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E472A5
    // Broiler-Human:        PENDING
    private JsStatement ParseExpressionStatement()
    {
        var span = Span();
        var expression = ParseExpression();
        Semicolon();
        return new JsExpressionStatement(span, expression);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D651CF
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
        return new JsBlockStatement(span, body);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=478F25
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C43D03
    // Broiler-Human:        PENDING
    private JsStatement ParseIf()
    {
        var span = Span();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");
        var consequent = ParseStatement();
        JsStatement? alternate = null;

        if (Current.Kind == SliceTokenKind.Else)
        {
            Advance();
            alternate = ParseStatement();
        }

        return new JsIfStatement(span, test, consequent, alternate);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CEFC11
    // Broiler-Human:        PENDING
    private JsStatement ParseWhile()
    {
        var span = Span();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");
        return new JsWhileStatement(span, test, ParseStatement());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F2DD64
    // Broiler-Human:        PENDING
    private JsStatement ParseDoWhile()
    {
        var span = Span();
        Advance();
        var body = ParseStatement();
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=986E89
    // Broiler-Human:        PENDING
    private JsStatement ParseFor()
    {
        var span = Span();
        Advance();

        // `for await` is refused before the parenthesis, because after it the head reads as an
        // ordinary one and the diagnostic would name whatever token followed `await` instead.
        if (Current.Kind == SliceTokenKind.Await)
        {
            return OutsideStatement(span, "`for await`");
        }

        Expect(SliceTokenKind.OpenParen, "(");

        JsStatement? initialiser = null;

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Var or SliceTokenKind.Const ||
            (Current.Kind == SliceTokenKind.Let && Peek(1).Kind is SliceTokenKind.Identifier or
                SliceTokenKind.Let or SliceTokenKind.Get or SliceTokenKind.Set or
                SliceTokenKind.Of or SliceTokenKind.Async or SliceTokenKind.Static or
                SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace))
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

            if (Current.Kind is SliceTokenKind.In or SliceTokenKind.Of && declarators.Count == 1)
            {
                // THE `of` HEAD TAKES AN AssignmentExpression AND THE `in` HEAD TAKES AN
                // Expression, which is not a distinction anybody would guess: `for (x of a, b)` is
                // a syntax error and `for (x in a, b)` iterates the keys of `b`.
                var isOf = Current.Kind == SliceTokenKind.Of;
                Advance();
                var source = isOf ? ParseAssignment() : ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");

                return isOf
                    ? new JsForOfStatement(
                        span, kind, declarators[0].Name, declarators[0].Pattern, null, source,
                        ParseStatement())
                    : new JsForInStatement(
                        span, kind, declarators[0].Name, declarators[0].Pattern, null, source,
                        ParseStatement());
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

                return isOf
                    ? new JsForOfStatement(
                        span, null, string.Empty, pattern, head, source, ParseStatement())
                    : new JsForInStatement(
                        span, null, string.Empty, pattern, head, source, ParseStatement());
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
        return new JsForStatement(span, initialiser, test, update, ParseStatement());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BB6569
    // Broiler-Human:        PENDING
    private JsStatement ParseBreakOrContinue()
    {
        var span = Span();
        var isBreak = Current.Kind == SliceTokenKind.Break;
        Advance();
        var label = string.Empty;

        if (Current.Kind == SliceTokenKind.Identifier && !Current.PrecededByLineTerminator)
        {
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0CEFB0
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2EBD7F
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
        return new JsSwitchStatement(span, discriminant, clauses);
    }

    // ---- functions -----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CEBED1
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=43342A
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
                else if (!IsIdentifierName(keyToken.Kind))
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CED958
    // Broiler-Human:        PENDING
    private JsFunctionNode ParseFunctionBody(
        SliceSourceSpan span,
        string name,
        System.Collections.Generic.List<JsParameter> parameters,
        bool isArrow,
        bool isGenerator = false,
        bool isAsync = false)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=FB1030
    // Broiler-Human:        PENDING
    private JsClassNode ParseClass(SliceSourceSpan span, bool declaration)
    {
        Expect(SliceTokenKind.Class, "class");
        var outer = strict;
        strict = true;
        var name = string.Empty;

        if (IsIdentifierName(Current.Kind))
        {
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
        return new JsClassNode(span, name, heritage, hasHeritage, members);
    }

    /// <summary>Parses one class element, or refuses one this manifest does not admit.</summary>
    /// <remarks>
    /// <b>Every modifier is settled before the key is read, and the discriminator is the token
    /// AFTER it.</b> <c>static</c>, <c>get</c>, <c>set</c> and <c>async</c> are all legal member
    /// names as well as modifiers, so <c>static() { }</c> is a method called <c>static</c> and
    /// <c>static m() { }</c> is a static method - and reading the key first would have made the
    /// second one a field called <c>static</c> followed by a surprise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E0DF0D
    // Broiler-Human:        PENDING
    private JsClassMember? ParseClassMember()
    {
        var span = Span();
        var isStatic = false;

        if (Current.Kind == SliceTokenKind.Static && !IsMemberNameEnd(Peek(1).Kind))
        {
            if (Peek(1).Kind == SliceTokenKind.OpenBrace)
            {
                Refuse(span, "a class static block");
                return null;
            }

            isStatic = true;
            Advance();
        }

        if (Current.Kind == SliceTokenKind.Star)
        {
            Refuse(span, "a generator method");
            return null;
        }

        if (Current.Kind == SliceTokenKind.Async &&
            !Peek(1).PrecededByLineTerminator &&
            !IsMemberNameEnd(Peek(1).Kind))
        {
            // AN ASYNC GENERATOR MEMBER IS STILL REFUSED AND ITS TEST COMES FIRST, for the reason
            // the object literal's does: the two differ by one token and an arm that parsed on
            // `async` alone would have turned a refusal into a surprise.
            if (Peek(1).Kind == SliceTokenKind.Star)
            {
                Refuse(span, "an async generator method");
                return null;
            }

            Advance();
            var asyncKey = PropertyKey(out var asyncComputed);
            var outerOperator = yieldIsOperator;
            var outerAwait = awaitIsOperator;
            yieldIsOperator = false;
            awaitIsOperator = true;
            var asyncParameters = ParseParameters();

            var asyncBody = ParseFunctionBody(
                span, asyncKey, asyncParameters, isArrow: false, isGenerator: false, isAsync: true);

            yieldIsOperator = outerOperator;
            awaitIsOperator = outerAwait;

            return new JsClassMember(
                span, JsMethodKind.Method, isStatic, asyncKey, asyncComputed, asyncBody);
        }

        var kind = JsMethodKind.Method;

        if (Current.Kind is SliceTokenKind.Get or SliceTokenKind.Set &&
            !IsMemberNameEnd(Peek(1).Kind))
        {
            kind = Current.Kind == SliceTokenKind.Get ? JsMethodKind.Get : JsMethodKind.Set;
            Advance();
        }

        if (IsPrivateName(Current))
        {
            Refuse(span, "a private name");
            return null;
        }

        var key = PropertyKey(out var computed);

        // A CLASS FIELD IS EVERY MEMBER THAT IS NOT FOLLOWED BY A PARAMETER LIST, and naming it
        // that way covers `x = 1`, a bare `x`, and `x` followed by a newline in one answer. The
        // alternative - refusing on the `=` alone - would have let a bare field come back as a
        // missing `(`, which names the punctuation rather than the construct.
        if (Current.Kind != SliceTokenKind.OpenParen)
        {
            Refuse(span, "a class field");
            return null;
        }

        var parameters = ParseOrdinaryParameters();
        var body = ParseFunctionBody(span, key, parameters, isArrow: false);
        return new JsClassMember(span, kind, isStatic, key, computed, body);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E2BBEB
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=016E6A
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

                if (target is not JsIdentifier and not JsMemberExpression and
                    not JsSuperMemberExpression)
                {
                    Refuse(
                        span,
                        SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                        "the left-hand side of an assignment is not a reference");
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=613434
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
                        span, string.Empty, parameters, isArrow: true, isGenerator: false, isAsync));
            }

            var value = ParseAssignment();

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=54E14C
    // Broiler-Human:        PENDING
    private JsExpression ParseBinary(int minimum, bool noIn)
    {
        var span = Span();
        var left = ParseUnary();

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=041EFB
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
                return new JsUnaryExpression(span, op, ParseUnary());
            }

            case SliceTokenKind.PlusPlus:
            case SliceTokenKind.MinusMinus:
            {
                var op = Current.Kind;
                Advance();
                return new JsUpdateExpression(span, op, ParseUnary(), Prefix: true);
            }

            default:
                return ParsePostfix();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=29102D
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=750BDD
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
                    current = new JsMemberExpression(span, current, MemberName(), null);
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

                    current = new JsMemberExpression(
                        span, current, MemberName(), null, Optional: true);

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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5B0A3D
    // Broiler-Human:        PENDING
    private JsExpression ParseMemberOnly()
    {
        var span = Span();
        var current = Current.Kind == SliceTokenKind.New ? ParseCallChain() : ParsePrimary();

        while (true)
        {
            switch (Current.Kind)
            {
                case SliceTokenKind.Dot:
                    Advance();
                    current = new JsMemberExpression(span, current, MemberName(), null);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AE848D
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
                !Peek(1).PrecededByLineTerminator && Peek(2).Kind == SliceTokenKind.Star:
                return OutsideExpression(span, "an async generator function");

            case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                !Peek(1).PrecededByLineTerminator:
                Advance();
                Advance();

                return new JsFunctionExpression(
                    span, ParseFunctionRest(span, declaration: false, isAsync: true));

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.OpenParen:
                return OutsideExpression(span, "a dynamic `import()`");

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.Dot:
                return OutsideExpression(span, "`import.meta`");

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

            // `#x in obj` is the one production that writes a private name where an expression is
            // expected, and it reaches here as an identifier whose first character is `#`. Left to
            // the arm below it would have become a free name and a run-time `ReferenceError`.
            case SliceTokenKind.Identifier when IsPrivateName(token):
                Advance();
                return OutsideExpression(span, "a private name");

            case SliceTokenKind.Identifier:
            case SliceTokenKind.Get:
            case SliceTokenKind.Set:
            case SliceTokenKind.Of:
            case SliceTokenKind.Static:
            case SliceTokenKind.Async:
            case SliceTokenKind.Let:
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F08DF1
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
                span, generatorKey, generatorParameters, isArrow: false, isGenerator: true);

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
            // THE `*` IS TESTED BEFORE ANYTHING IS PARSED, because an async generator method is
            // still refused by name and the two constructs differ by that one token. An arm that
            // parsed on `async` alone would have made `{ async *m(){} }` a surprise token where a
            // refusal belongs.
            if (Peek(1).Kind == SliceTokenKind.Star)
            {
                Refuse(span, "an async generator method");

                return new JsObjectEntry(
                    span, JsPropertyKind.Init, string.Empty, null, new JsNullLiteral(span));
            }

            Advance();
            var asyncKey = PropertyKey(out var asyncComputed);
            var outerOperator = yieldIsOperator;
            var outerAwait = awaitIsOperator;
            yieldIsOperator = false;
            awaitIsOperator = true;
            var asyncParameters = ParseParameters();

            var asyncBody = ParseFunctionBody(
                span, asyncKey, asyncParameters, isArrow: false, isGenerator: false, isAsync: true);

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
            var body = ParseFunctionBody(span, accessorKey, parameters, isArrow: false);

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
            var body = ParseFunctionBody(span, key, parameters, isArrow: false);

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=67F4FE
    // Broiler-Human:        PENDING
    private string MemberName()
    {
        var token = Current;

        // A PRIVATE NAME IS REFUSED WHERE IT IS WRITTEN AND NOT WHERE IT IS DECLARED. `this.#x`
        // reads as an ordinary property access whose name happens to begin with `#`, so before
        // classes were admitted it produced a run-time `undefined` and named nothing; with the
        // class body admitted it is the one position left where a private name could still slip
        // through under a diagnostic that does not name it.
        if (IsPrivateName(token))
        {
            Refuse(Span(), "a private name");
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3664C9
    // Broiler-Human:        PENDING
    private string BindingName()
    {
        var token = Current;

        if (IsIdentifierName(token.Kind))
        {
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9DE154
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
