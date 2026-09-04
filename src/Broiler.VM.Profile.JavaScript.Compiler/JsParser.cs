// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   57
// Annotated:        57/57
// Exempt:           7
// Human-reviewed:   0/57
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       57
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
/// <b>What it refuses, it refuses by name.</b> The wide manifest admits no class, generator,
/// <c>async</c> function, module declaration, destructuring pattern, spread, template literal,
/// <c>for…of</c>, <c>with</c> or optional chain. Each is parsed far enough to be recognised and
/// then reported as a construct outside the manifest, at its own position - not as an unexpected
/// token, which would send a reader looking for a typo.
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

    /// <summary>Creates a parser over an already-tokenized source.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9D7915
    // Broiler-Human:        PENDING
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
    internal JsParser(SliceToken[] stream, SliceParseOptions parseOptions, bool forceStrict = false)
    {
        tokens = stream;
        options = parseOptions;
        strict = forceStrict;
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4C80E3
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
                    return OutsideStatement(span, "a class declaration");

                case SliceTokenKind.Import:
                case SliceTokenKind.Export:
                    return OutsideStatement(span, "a module declaration");

                case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                    !Peek(1).PrecededByLineTerminator:
                    return OutsideStatement(span, "an async function");

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A23C93
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<JsDeclarator> ParseDeclarators(bool noIn)
    {
        var declarators = new System.Collections.Generic.List<JsDeclarator>();

        while (true)
        {
            var span = Span();

            if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
            {
                Refuse(span, "a destructuring pattern");
                return declarators;
            }

            var name = BindingName();
            JsExpression? initialiser = null;

            if (Current.Kind == SliceTokenKind.Equals)
            {
                Advance();
                initialiser = ParseAssignment(noIn);
            }

            declarators.Add(new JsDeclarator(span, name, initialiser));

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AAEC79
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
        SliceDeclarationKind? declaration = null;
        var name = string.Empty;
        JsExpression? target = null;

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Var or SliceTokenKind.Const ||
            (Current.Kind == SliceTokenKind.Let && Peek(1).Kind != SliceTokenKind.OpenBracket))
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

            if (Current.Kind == SliceTokenKind.In && declarators.Count == 1)
            {
                Advance();
                declaration = kind;
                name = declarators[0].Name;
                var right = ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");
                return new JsForInStatement(span, declaration, name, null, right, ParseStatement());
            }

            if (Current.Kind == SliceTokenKind.Of)
            {
                return OutsideStatement(span, "a `for … of` statement");
            }

            initialiser = new JsVariableStatement(headSpan, kind, declarators);
            Expect(SliceTokenKind.Semicolon, ";");
        }
        else
        {
            var headSpan = Span();
            var expression = ParseExpression(noIn: true);

            if (Current.Kind == SliceTokenKind.In)
            {
                Advance();
                target = expression;
                var right = ParseExpression();
                Expect(SliceTokenKind.CloseParen, ")");
                return new JsForInStatement(span, null, string.Empty, target, right, ParseStatement());
            }

            if (Current.Kind == SliceTokenKind.Of)
            {
                return OutsideStatement(span, "a `for … of` statement");
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4DCE56
    // Broiler-Human:        PENDING
    private JsStatement ParseTry()
    {
        var span = Span();
        Advance();
        var block = ParseBlock();
        var parameter = string.Empty;
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
                    return OutsideStatement(span, "a destructuring catch parameter");
                }

                parameter = BindingName();
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

        return new JsTryStatement(span, block, parameter, handler, finaliser);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=30402D
    // Broiler-Human:        PENDING
    private JsFunctionNode ParseFunctionRest(SliceSourceSpan span, bool declaration)
    {
        if (Current.Kind == SliceTokenKind.Star)
        {
            Refuse(span, "a generator function");
            Advance();
        }

        var name = string.Empty;

        if (IsIdentifierName(Current.Kind))
        {
            name = Current.RawText;
            Advance();
        }
        else if (declaration)
        {
            Refuse(span, SliceSourceDiagnosticCode.ExpectedToken, "a function declaration needs a name");
        }

        var parameters = ParseParameters();
        return ParseFunctionBody(span, name, parameters, isArrow: false);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D130A2
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> ParseParameters()
    {
        var parameters = new System.Collections.Generic.List<string>();
        Expect(SliceTokenKind.OpenParen, "(");

        while (Current.Kind != SliceTokenKind.CloseParen && Current.Kind != SliceTokenKind.EndOfSource)
        {
            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Refuse(Span(), "a rest parameter");
                return parameters;
            }

            if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
            {
                Refuse(Span(), "a destructuring parameter");
                return parameters;
            }

            parameters.Add(BindingName());

            if (Current.Kind == SliceTokenKind.Equals)
            {
                Refuse(Span(), "a parameter default");
                return parameters;
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseParen, ")");
        return parameters;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=581A91
    // Broiler-Human:        PENDING
    private JsFunctionNode ParseFunctionBody(
        SliceSourceSpan span,
        string name,
        System.Collections.Generic.List<string> parameters,
        bool isArrow)
    {
        var outer = strict;
        Expect(SliceTokenKind.OpenBrace, "{");
        var directives = ParseDirectives();
        var body = new System.Collections.Generic.List<JsStatement>();

        while (Current.Kind != SliceTokenKind.CloseBrace &&
            Current.Kind != SliceTokenKind.EndOfSource &&
            diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        Expect(SliceTokenKind.CloseBrace, "}");
        var inner = strict;
        strict = outer;
        return new JsFunctionNode(span, name, parameters, body, isArrow, inner, directives);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E12D71
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

                if (target is JsArrayLiteral or JsObjectLiteral)
                {
                    // `[a, b] = c` IS A DESTRUCTURING ASSIGNMENT, which this manifest does not
                    // admit - and calling it an invalid assignment target said the program was
                    // wrong when the program is fine and this front end is the one that is
                    // narrow. The two answers differ in their diagnostic code, which is what the
                    // conformance runner grades the manifest boundary on.
                    Refuse(span, "a destructuring assignment");
                }
                else if (target is not JsIdentifier and not JsMemberExpression)
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
    /// Neither can be an OPERATOR in anything this manifest admits, because it admits no async
    /// function and no generator. So <c>await x</c> in a script is two identifiers in a row, which
    /// is the syntax error every engine reports it as, and no refusal by name is owed for it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=937960
    // Broiler-Human:        PENDING
    private bool IsIdentifierName(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
            SliceTokenKind.Of or SliceTokenKind.Async or SliceTokenKind.Static or
            SliceTokenKind.Let => true,
        SliceTokenKind.Await => options.Goal != SliceGoal.Module,
        SliceTokenKind.Yield => !strict,
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7ACBBC
    // Broiler-Human:        PENDING
    private bool TryParseArrow(SliceSourceSpan span, out JsExpression arrow)
    {
        arrow = null!;

        // AN ASYNC ARROW IS REFUSED HERE RATHER THAN IN ParsePrimary, and the position is the
        // point. `async(1)` is a call of a function named `async` and `{ async: 1 }` is a
        // property, both of which this manifest admits; only the `=>` that the scan below finds
        // tells an arrow from either. Refusing on the first token would have broken two admitted
        // programs to name one unadmitted construct.
        if (Current.Kind == SliceTokenKind.Async && IsAsyncArrowHead())
        {
            arrow = OutsideExpression(span, "an async arrow function");
            return true;
        }

        if (Current.Kind is SliceTokenKind.Identifier or SliceTokenKind.Get or SliceTokenKind.Set or
            SliceTokenKind.Of or SliceTokenKind.Static)
        {
            if (Peek(1).Kind != SliceTokenKind.EqualsGreaterThan)
            {
                return false;
            }

            var single = new System.Collections.Generic.List<string> { Current.RawText };
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

        var parameters = ParseParameters();
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7A5DA0
    // Broiler-Human:        PENDING
    private JsExpression ParseArrowBody(
        SliceSourceSpan span, System.Collections.Generic.List<string> parameters)
    {
        if (Current.Kind == SliceTokenKind.OpenBrace)
        {
            return new JsFunctionExpression(
                span, ParseFunctionBody(span, string.Empty, parameters, isArrow: true));
        }

        var value = ParseAssignment();

        var body = new System.Collections.Generic.List<JsStatement>
        {
            new JsReturnStatement(span, value),
        };

        return new JsFunctionExpression(
            span,
            new JsFunctionNode(span, string.Empty, parameters, body, true, strict, []));
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=50D392
    // Broiler-Human:        PENDING
    private JsExpression ParseUnary()
    {
        var span = Span();

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4A7D70
    // Broiler-Human:        PENDING
    private JsExpression ParseCallChain()
    {
        var span = Span();
        JsExpression current;

        if (Current.Kind == SliceTokenKind.New)
        {
            Advance();

            if (Current.Kind == SliceTokenKind.Dot)
            {
                return OutsideExpression(span, "`new.target`");
            }

            var callee = ParseMemberOnly();

            var arguments = Current.Kind == SliceTokenKind.OpenParen
                ? ParseArguments()
                : [];

            current = new JsNewExpression(span, callee, arguments);
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
                    return OutsideExpression(span, "an optional chain");

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
                    return OutsideExpression(span, "a tagged template");

                default:
                    return current;
            }
        }
    }

    /// <summary>Parses the callee of a <c>new</c>, which stops before the argument list.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=95FA4C
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

                default:
                    return current;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0E7AC7
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
                Refuse(Span(), "a spread argument");
                return arguments;
            }

            arguments.Add(ParseAssignment());

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseParen, ")");
        return arguments;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1C93C8
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
                return OutsideExpression(span, "an async function");

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.OpenParen:
                return OutsideExpression(span, "a dynamic `import()`");

            case SliceTokenKind.Import when Peek(1).Kind == SliceTokenKind.Dot:
                return OutsideExpression(span, "`import.meta`");

            case SliceTokenKind.Await when options.Goal != SliceGoal.Module:
            case SliceTokenKind.Yield when !strict:
                Advance();
                return new JsIdentifier(span, token.RawText);

            // RESERVED HERE, ORDINARY THERE. Where the goal and the strictness make one of these
            // a reserved word, the honest answer is the syntax error every engine gives and NOT a
            // construct-outside-the-manifest refusal: the manifest is not what forbids it.
            case SliceTokenKind.Await:
            case SliceTokenKind.Yield:
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.ReservedWordAsBinding,
                    "`" + token.RawText + "` is a reserved word " +
                        (token.Kind == SliceTokenKind.Await ? "in a module" : "in strict code"));

                Advance();
                return new JsNullLiteral(span);
            }

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
                return OutsideExpression(span, "a class expression");

            case SliceTokenKind.Super:
                return OutsideExpression(span, "`super`");

            case SliceTokenKind.TemplateLiteral:
                return OutsideExpression(span, "a template literal");

            default:
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`" + Describe(token) + "` begins no expression");

                Advance();
                return new JsNumberLiteral(span, 0, false);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=43DB2A
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
                Refuse(Span(), "a spread element");
                break;
            }

            elements.Add(ParseAssignment());

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=12DD63
    // Broiler-Human:        PENDING
    private JsObjectEntry ParseObjectEntry()
    {
        var span = Span();

        if (Current.Kind == SliceTokenKind.DotDotDot)
        {
            Refuse(span, "a spread property");
            Advance();
            return new JsObjectEntry(span, JsPropertyKind.Init, string.Empty, null, new JsNullLiteral(span));
        }

        // A METHOD MODIFIER IS NOT A PROPERTY KEY. `{ *m() {} }` and `{ async m() {} }` reached
        // PropertyKey, which takes any token's text as a name, and the diagnostic a reader got
        // was a missing colon. The discriminator for `async` is the one the accessor branch below
        // already uses: a modifier is followed by a key, while a property named `async` is
        // followed by `:`, `,`, `}` or `(`.
        if (Current.Kind == SliceTokenKind.Star)
        {
            Refuse(span, "a generator method");
            return new JsObjectEntry(span, JsPropertyKind.Init, string.Empty, null, new JsNullLiteral(span));
        }

        if (Current.Kind == SliceTokenKind.Async &&
            Peek(1).Kind is not SliceTokenKind.Colon and not SliceTokenKind.Comma and
                not SliceTokenKind.CloseBrace and not SliceTokenKind.OpenParen)
        {
            Refuse(
                span,
                Peek(1).Kind == SliceTokenKind.Star ? "an async generator method" : "an async method");

            return new JsObjectEntry(span, JsPropertyKind.Init, string.Empty, null, new JsNullLiteral(span));
        }

        if (Current.Kind is SliceTokenKind.Get or SliceTokenKind.Set &&
            Peek(1).Kind is not SliceTokenKind.Colon and not SliceTokenKind.Comma and
            not SliceTokenKind.CloseBrace and not SliceTokenKind.OpenParen)
        {
            var kind = Current.Kind == SliceTokenKind.Get ? JsPropertyKind.Get : JsPropertyKind.Set;
            Advance();
            var accessorKey = PropertyKey(out var accessorComputed);
            var parameters = ParseParameters();
            var body = ParseFunctionBody(span, accessorKey, parameters, isArrow: false);

            return new JsObjectEntry(
                span, kind, accessorKey, accessorComputed, new JsFunctionExpression(span, body));
        }

        var key = PropertyKey(out var computed);

        if (Current.Kind == SliceTokenKind.OpenParen)
        {
            var parameters = ParseParameters();
            var body = ParseFunctionBody(span, key, parameters, isArrow: false);

            return new JsObjectEntry(
                span, JsPropertyKind.Init, key, computed, new JsFunctionExpression(span, body));
        }

        if (Current.Kind is SliceTokenKind.Comma or SliceTokenKind.CloseBrace)
        {
            // A shorthand property: `{ x }` is `{ x: x }`, and the key is a name in scope.
            return new JsObjectEntry(
                span, JsPropertyKind.Init, key, computed, new JsIdentifier(span, key));
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D7DEDB
    // Broiler-Human:        PENDING
    private string MemberName()
    {
        var token = Current;
        Advance();
        return token.RawText;
    }

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
}
