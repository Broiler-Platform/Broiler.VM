// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   79
// Annotated:        79/79
// Exempt:           13
// Human-reviewed:   0/79
// IP risk:          None
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       79
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
/// <c>async</c> function, module declaration, destructuring pattern, spread, <c>for…of</c> or
/// <c>with</c>. Each is parsed far enough to be recognised and then reported as a construct
/// outside the manifest, at its own position - not as an unexpected token, which would send a
/// reader looking for a typo.
/// </para>
/// <para>
/// <b>Four families left that list on 2026-09-04 and are now PARSED rather than named.</b> A
/// template literal and a tagged template become a <see cref="JsTemplateLiteral"/> with its chunks
/// split and its substitutions parsed; an optional chain becomes links carrying an
/// <c>Optional</c> flag inside one <see cref="JsChainExpression"/>, which is the node that owns
/// where the short circuit lands; and <c>new.target</c> becomes a node of its own, admitted only
/// where the grammar admits it - inside an ordinary function body, and not at the top level of a
/// script even through an arrow. Nothing else moved: every other refusal above is still spelled
/// where it was, because the conformance runner grades the manifest boundary on the diagnostic
/// code and a refusal removed by accident is a manifest change nobody declared.
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A0C1D5
    // Broiler-Human:        PENDING
    internal JsParser(
        SliceToken[] stream,
        SliceParseOptions parseOptions,
        bool forceStrict = false,
        int enclosingFunctionDepth = 0)
    {
        tokens = stream;
        options = parseOptions;
        strict = forceStrict;
        functionDepth = enclosingFunctionDepth;
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=02ADFD
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7FB015
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F13E14
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

        var inner = new JsParser(stream, options, strict, functionDepth);
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
