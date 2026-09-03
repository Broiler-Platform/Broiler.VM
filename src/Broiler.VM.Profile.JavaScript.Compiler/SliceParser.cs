// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           7
// Human-reviewed:   0/28
// IP risk:          None
// Security risk:    High
// Criteria:         8/8
// Resource impact:  2/10 max
// Unverified:       28
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The recursive-descent parser for the slice grammar.
/// </summary>
/// <remarks>
/// <para>
/// <b>It rules on nothing.</b> Every early error - a duplicate lexical name, an assignment to a
/// constant, a legacy octal in strict code, a <c>break</c> outside a loop - is the validator's,
/// and this parser accepts each of them into a tree. That is the consolidation roadmap section 9
/// asks for: the seed splits early-error responsibility across four places in two assemblies, and
/// a verifier that must answer totally in one pass with one diagnostic per rejection cannot be
/// built over a split like that. What this refuses is what is not a tree at all.
/// </para>
/// <para>
/// <b>It reads no ambient state.</b> The goal symbol and the top-level-await permission arrive in
/// <see cref="SliceParseOptions"/>, by value, and two parses with different goals in one process
/// cannot reach each other's switches. The seed reads both out of async-local state in another
/// assembly; that is the shape this replaces.
/// </para>
/// <para>
/// <b>Its recursion is bounded and the bound is a refusal.</b> Every production that can nest
/// increments a depth counter, and exceeding the option's bound produces
/// <see cref="SliceSourceDiagnosticCode.NestingTooDeep"/> rather than a stack overflow. Roadmap
/// section 9 makes a process termination on a nesting case a blocking failure, so the bound is
/// the mechanism and the nesting corpus is the evidence.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=32D6FA
// Broiler-Falsified-If: a nesting case terminates the process, or a grammar switch is read from anywhere but the options value
// Broiler-Human:        PENDING
public sealed class SliceParser
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=840A20
    // Broiler-Human:        PENDING
    private readonly SliceToken[] tokens;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B380E2
    // Broiler-Human:        PENDING
    private readonly SliceParseOptions options;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=98C321
    // Broiler-Human:        PENDING
    private int at;
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=329437
    // Broiler-Human:        PENDING
    private int depth;

    /// <summary>Creates a parser over <paramref name="tokens"/> under <paramref name="options"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9C7AE3
    // Broiler-Human:        PENDING
    public SliceParser(SliceToken[] tokens, SliceParseOptions options)
    {
        this.tokens = tokens ?? throw new System.ArgumentNullException(nameof(tokens));
        this.options = options;
    }

    /// <summary>Every refusal this pass produced, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=162F14
    // Broiler-Human:        PENDING
    public System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics => diagnostics;

    /// <summary>Parses the whole token stream as a program.</summary>
    /// <remarks>
    /// It answers a tree even when it refused, and the caller reads
    /// <see cref="Diagnostics"/> to know which. A parser that answered null on refusal would make
    /// every caller write the same null check and would lose the partial tree a later stage could
    /// have reported more from.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=2D78F6
    // Broiler-Falsified-If: a statement that is not an expression statement over a string literal is admitted into the directive prologue
    // Broiler-Human:        PENDING
    public SliceProgram ParseProgram()
    {
        var span = Here();
        var directives = new System.Collections.Generic.List<SliceStringLiteral>();
        var body = new System.Collections.Generic.List<SliceStatement>();

        // The directive prologue: expression statements over string literals, decided here so the
        // validator never has to look at source text to tell one from a string expression.
        //
        // A string literal is a directive only when the whole statement is that literal. `"use
        // strict" + 1` is an ExpressionStatement and enables nothing, and a parser that took its
        // first token as a directive would turn on strict mode for a program that never asked -
        // which is a wrong answer rather than a wrong diagnostic.
        while (Current.Kind == SliceTokenKind.StringLiteral && StatementEndsAfterCurrent() &&
            diagnostics.Count == 0)
        {
            var literalSpan = Here();
            var token = Current;
            Advance();
            ConsumeStatementTerminator();
            directives.Add(new SliceStringLiteral(literalSpan, token.StringValue, token.RawText));
        }

        while (Current.Kind != SliceTokenKind.EndOfSource && diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        return new SliceProgram(span, directives, body);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=0E32ED
    // Broiler-Falsified-If: a statement form the manifest excludes is parsed into a tree instead of being named as a construct refusal
    // Broiler-Human:        PENDING
    private SliceStatement ParseStatement()
    {
        if (!Enter())
        {
            return new SliceEmptyStatement(Here());
        }

        try
        {
            var span = Here();

            switch (Current.Kind)
            {
                case SliceTokenKind.Semicolon:
                    Advance();
                    return new SliceEmptyStatement(span);

                case SliceTokenKind.OpenBrace:
                    return ParseBlock();

                case SliceTokenKind.Var:
                case SliceTokenKind.Let:
                case SliceTokenKind.Const:
                    {
                        var statement = ParseVariableStatement();
                        ConsumeStatementTerminator();
                        return statement;
                    }

                case SliceTokenKind.If:
                    return ParseIf();

                case SliceTokenKind.While:
                    return ParseWhile();

                case SliceTokenKind.Do:
                    return ParseDoWhile();

                case SliceTokenKind.For:
                    return ParseFor();

                case SliceTokenKind.Break:
                    Advance();
                    ConsumeStatementTerminator();
                    return new SliceBreakStatement(span);

                case SliceTokenKind.Continue:
                    Advance();
                    ConsumeStatementTerminator();
                    return new SliceContinueStatement(span);

                case SliceTokenKind.ReservedWord:
                    RefuseConstruct($"the reserved word `{Current.RawText}`");
                    Advance();
                    return new SliceEmptyStatement(span);

                default:
                    {
                        var expression = ParseExpression();
                        ConsumeStatementTerminator();
                        return new SliceExpressionStatement(span, expression);
                    }
            }
        }
        finally
        {
            Leave();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=FF0C6F
    // Broiler-Human:        PENDING
    private SliceBlockStatement ParseBlock()
    {
        var span = Here();
        Expect(SliceTokenKind.OpenBrace, "{");
        var body = new System.Collections.Generic.List<SliceStatement>();

        while (Current.Kind is not (SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            body.Add(ParseStatement());
        }

        Expect(SliceTokenKind.CloseBrace, "}");

        return new SliceBlockStatement(span, body);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=970EF7
    // Broiler-Human:        PENDING
    private SliceVariableStatement ParseVariableStatement()
    {
        var span = Here();

        var kind = Current.Kind switch
        {
            SliceTokenKind.Var => SliceDeclarationKind.Var,
            SliceTokenKind.Let => SliceDeclarationKind.Let,
            _ => SliceDeclarationKind.Const,
        };

        Advance();
        var declarators = new System.Collections.Generic.List<SliceDeclarator>();

        while (true)
        {
            var nameSpan = Here();

            if (Current.Kind != SliceTokenKind.Identifier)
            {
                // A reserved word as a binding is a real program shape and belongs to the
                // validator, so it is accepted here and named there. Anything else is not a
                // declarator at all.
                if (Current.Kind == SliceTokenKind.ReservedWord)
                {
                    var reserved = Current.RawText;
                    Advance();
                    declarators.Add(new SliceDeclarator(nameSpan, reserved, ParseOptionalInitialiser()));
                }
                else
                {
                    Refuse(
                        SliceSourceDiagnosticCode.ExpectedToken,
                        "a declaration needs a binding identifier");

                    break;
                }
            }
            else
            {
                var name = Current.RawText;
                Advance();
                declarators.Add(new SliceDeclarator(nameSpan, name, ParseOptionalInitialiser()));
            }

            if (Current.Kind != SliceTokenKind.Comma || diagnostics.Count > 0)
            {
                break;
            }

            Advance();
        }

        return new SliceVariableStatement(span, kind, declarators);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BB4EAE
    // Broiler-Human:        PENDING
    private SliceExpression? ParseOptionalInitialiser()
    {
        if (Current.Kind != SliceTokenKind.Equals)
        {
            return null;
        }

        Advance();

        return ParseAssignment();
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F3B448
    // Broiler-Human:        PENDING
    private SliceStatement ParseIf()
    {
        var span = Here();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");
        var consequent = ParseStatement();
        SliceStatement? alternate = null;

        if (Current.Kind == SliceTokenKind.Else)
        {
            Advance();
            alternate = ParseStatement();
        }

        return new SliceIfStatement(span, test, consequent, alternate);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5CFB94
    // Broiler-Human:        PENDING
    private SliceStatement ParseWhile()
    {
        var span = Here();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");

        return new SliceWhileStatement(span, test, ParseStatement());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CCFAFC
    // Broiler-Human:        PENDING
    private SliceStatement ParseDoWhile()
    {
        var span = Here();
        Advance();
        var body = ParseStatement();
        Expect(SliceTokenKind.While, "while");
        Expect(SliceTokenKind.OpenParen, "(");
        var test = ParseExpression();
        Expect(SliceTokenKind.CloseParen, ")");

        // The language inserts a semicolon after `do … while (…)` unconditionally, which is the
        // one place semicolon insertion is not about a line terminator.
        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }

        return new SliceDoWhileStatement(span, body, test);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=060EBC
    // Broiler-Human:        PENDING
    private SliceStatement ParseFor()
    {
        var span = Here();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");

        SliceStatement? initialiser = null;

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }
        else if (Current.Kind is SliceTokenKind.Var or SliceTokenKind.Let or SliceTokenKind.Const)
        {
            initialiser = ParseVariableStatement();
            Expect(SliceTokenKind.Semicolon, ";");
        }
        else
        {
            var expressionSpan = Here();
            initialiser = new SliceExpressionStatement(expressionSpan, ParseExpression());
            Expect(SliceTokenKind.Semicolon, ";");
        }

        SliceExpression? test = null;

        if (Current.Kind != SliceTokenKind.Semicolon)
        {
            test = ParseExpression();
        }

        Expect(SliceTokenKind.Semicolon, ";");

        SliceExpression? update = null;

        if (Current.Kind != SliceTokenKind.CloseParen)
        {
            update = ParseExpression();
        }

        Expect(SliceTokenKind.CloseParen, ")");

        return new SliceForStatement(span, initialiser, test, update, ParseStatement());
    }

    /// <summary>An expression, which at this manifest is exactly an assignment expression.</summary>
    /// <remarks>
    /// The comma operator is not admitted. It is a construct outside the manifest rather than a
    /// gap: a comma in expression position would silently discard its left operand, and a slice
    /// whose only observable is a completion value would report that discard as a wrong answer
    /// rather than as a refusal.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A819A0
    // Broiler-Human:        PENDING
    private SliceExpression ParseExpression() => ParseAssignment();

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=AF7AFD
    // Broiler-Human:        PENDING
    private SliceExpression ParseAssignment()
    {
        if (!Enter())
        {
            return new SliceNumericLiteral(Here(), 0, false);
        }

        try
        {
            var span = Here();
            var left = ParseConditional();

            if (Current.Kind != SliceTokenKind.Equals)
            {
                return left;
            }

            Advance();

            // Assignment is right-associative, so the right side is another assignment and not a
            // conditional. `a = b = 1` is `a = (b = 1)`.
            return new SliceAssignmentExpression(span, left, ParseAssignment());
        }
        finally
        {
            Leave();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9A888C
    // Broiler-Human:        PENDING
    private SliceExpression ParseConditional()
    {
        var span = Here();
        var test = ParseBinary(0);

        if (Current.Kind != SliceTokenKind.Question)
        {
            return test;
        }

        Advance();
        var whenTrue = ParseAssignment();
        Expect(SliceTokenKind.Colon, ":");

        return new SliceConditionalExpression(span, test, whenTrue, ParseAssignment());
    }

    /// <summary>
    /// Precedence climbing over the binary and logical operators.
    /// </summary>
    /// <remarks>
    /// One table and one loop rather than a cascade of a dozen mutually recursive methods. The
    /// cascade costs one stack frame per precedence level for every expression however simple,
    /// which interacts badly with a depth bound: the bound would then be a bound on precedence
    /// levels rather than on the source's own nesting, and a flat expression would consume it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=E7AD1A
    // Broiler-Falsified-If: the tree this builds groups an operator differently from the language's precedence and associativity
    // Broiler-Human:        PENDING
    private SliceExpression ParseBinary(int minimumPrecedence)
    {
        if (!Enter())
        {
            return new SliceNumericLiteral(Here(), 0, false);
        }

        try
        {
            var left = ParseUnary();

            while (true)
            {
                var precedence = Precedence(Current.Kind);

                if (precedence < 0 || precedence < minimumPrecedence || diagnostics.Count > 0)
                {
                    return left;
                }

                var span = Here();
                var op = Current.Kind;
                Advance();

                // Every operator here is left-associative, so the right operand binds only tighter
                // things: `1 - 2 - 3` is `(1 - 2) - 3`.
                var right = ParseBinary(precedence + 1);

                left = op is SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar
                    ? new SliceLogicalExpression(span, op, left, right)
                    : new SliceBinaryExpression(span, op, left, right);
            }
        }
        finally
        {
            Leave();
        }
    }

    /// <summary>The precedence of a binary operator, or -1 when the token is not one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D70DCC
    // Broiler-Falsified-If: two operators the language separates share a level here, or the order differs from the language's
    // Broiler-Human:        PENDING
    private static int Precedence(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.BarBar => 1,
        SliceTokenKind.AmpersandAmpersand => 2,
        SliceTokenKind.Bar => 3,
        SliceTokenKind.Caret => 4,
        SliceTokenKind.Ampersand => 5,
        SliceTokenKind.EqualsEqualsEquals or SliceTokenKind.BangEqualsEquals or
        SliceTokenKind.EqualsEquals or SliceTokenKind.BangEquals => 6,
        SliceTokenKind.LessThan or SliceTokenKind.LessThanEquals or
        SliceTokenKind.GreaterThan or SliceTokenKind.GreaterThanEquals => 7,
        SliceTokenKind.LessThanLessThan or SliceTokenKind.GreaterThanGreaterThan or
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => 8,
        SliceTokenKind.Plus or SliceTokenKind.Minus => 9,
        SliceTokenKind.Star or SliceTokenKind.Slash or SliceTokenKind.Percent => 10,
        _ => -1,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B51C42
    // Broiler-Human:        PENDING
    private SliceExpression ParseUnary()
    {
        var span = Here();

        if (Current.Kind is SliceTokenKind.Plus or SliceTokenKind.Minus or SliceTokenKind.Bang)
        {
            var op = Current.Kind;
            Advance();

            return new SliceUnaryExpression(span, op, ParseUnary());
        }

        if (Current.Kind == SliceTokenKind.Tilde)
        {
            RefuseConstruct("the bitwise-not operator `~`");
            Advance();

            return new SliceNumericLiteral(span, 0, false);
        }

        return ParsePrimary();
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E933BD
    // Broiler-Human:        PENDING
    private SliceExpression ParsePrimary()
    {
        var span = Here();
        var token = Current;

        switch (token.Kind)
        {
            case SliceTokenKind.NumericLiteral:
                Advance();
                return new SliceNumericLiteral(span, token.NumericValue, token.IsLegacyOctal);

            case SliceTokenKind.True:
                Advance();
                return new SliceBooleanLiteral(span, true);

            case SliceTokenKind.False:
                Advance();
                return new SliceBooleanLiteral(span, false);

            case SliceTokenKind.Identifier:
                Advance();
                return new SliceIdentifierReference(span, token.RawText);

            case SliceTokenKind.StringLiteral:
                // Outside the directive prologue there is no string value in this manifest, so
                // this is a construct refusal and not a parse failure - the difference a reader
                // needs is "this profile has no strings", not "a string cannot appear here".
                RefuseConstruct("a string value");
                Advance();
                return new SliceStringLiteral(span, token.StringValue, token.RawText);

            case SliceTokenKind.OpenParen:
                {
                    Advance();
                    var inner = ParseExpression();
                    Expect(SliceTokenKind.CloseParen, ")");
                    return inner;
                }

            case SliceTokenKind.OpenBrace:
                RefuseConstruct("an object literal");
                Advance();
                return new SliceNumericLiteral(span, 0, false);

            case SliceTokenKind.ReservedWord:
                RefuseConstruct($"the reserved word `{token.RawText}`");
                Advance();
                return new SliceNumericLiteral(span, 0, false);

            default:
                Refuse(
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    $"`{Describe(token)}` begins no expression this grammar defines");

                Advance();
                return new SliceNumericLiteral(span, 0, false);
        }
    }

    /// <summary>
    /// Consumes a statement's terminating semicolon, inserting one where the language does.
    /// </summary>
    /// <remarks>
    /// The three insertion sites: an offending token preceded by a line terminator, a <c>}</c>,
    /// and the end of the source. The first is answered from the token's own recorded flag rather
    /// than by looking back at the source text, which is the third re-scan this front end does not
    /// have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=C160FB
    // Broiler-Falsified-If: a semicolon is inserted where the language does not insert one, or omitted where it does
    // Broiler-Human:        PENDING
    private void ConsumeStatementTerminator()
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
            SliceSourceDiagnosticCode.ExpectedToken,
            $"a statement ends with `;` or a line terminator, and `{Describe(Current)}` is neither");
    }

    /// <summary>Takes one level of the depth allowance, refusing once when it runs out.</summary>
    /// <remarks>
    /// It refuses <b>once</b>: the first over-deep production reports, and every deeper one
    /// unwinds silently, because a bound that reported per level would answer a 10,000-deep source
    /// with 9,936 identical diagnostics.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0D6A78
    // Broiler-Falsified-If: recursion continues after this answers false
    // Broiler-Human:        PENDING
    private bool Enter()
    {
        if (depth >= options.MaximumNestingDepth)
        {
            Refuse(
                SliceSourceDiagnosticCode.NestingTooDeep,
                $"the source nests deeper than the {options.MaximumNestingDepth} levels these " +
                "parse options allow");

            return false;
        }

        depth++;

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1DEC1A
    // Broiler-Human:        PENDING
    private void Leave() => depth--;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9F7F73
    // Broiler-Human:        PENDING
    private SliceToken Current => tokens[System.Math.Min(at, tokens.Length - 1)];

    /// <summary>
    /// Whether the statement would end immediately after the current token.
    /// </summary>
    /// <remarks>
    /// <b>The one place this parser looks ahead, and the directive prologue is why.</b> A string
    /// literal is a directive only when the whole statement is that literal, so telling
    /// <c>"use strict";</c> from <c>"use strict" + 1</c> needs the token after the literal and
    /// cannot be done from the literal alone. It reads the same three insertion sites
    /// <see cref="ConsumeStatementTerminator"/> does, from the next token's own recorded flag,
    /// which is why neither of them looks at the source text.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BF6120
    // Broiler-Falsified-If: a string literal that is not a whole statement is admitted into the directive prologue
    // Broiler-Human:        PENDING
    private bool StatementEndsAfterCurrent()
    {
        var next = tokens[System.Math.Min(at + 1, tokens.Length - 1)];

        return next.Kind is SliceTokenKind.Semicolon or SliceTokenKind.CloseBrace or
            SliceTokenKind.EndOfSource || next.PrecededByLineTerminator;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=263E48
    // Broiler-Human:        PENDING
    private SliceSourceSpan Here() => new(Current.Line, Current.Column);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B440C1
    // Broiler-Human:        PENDING
    private void Advance()
    {
        if (at < tokens.Length - 1)
        {
            at++;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8CBE2C
    // Broiler-Human:        PENDING
    private void Expect(SliceTokenKind kind, string text)
    {
        if (Current.Kind == kind)
        {
            Advance();
            return;
        }

        Refuse(
            SliceSourceDiagnosticCode.ExpectedToken,
            $"`{text}` was required here and `{Describe(Current)}` is what is there");
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B30B14
    // Broiler-Human:        PENDING
    private void RefuseConstruct(string what) =>
        Refuse(
            SliceSourceDiagnosticCode.ConstructOutsideManifest,
            $"{what} is not admitted by the declared feature manifest");

    /// <summary>Records a refusal, at most one per parse.</summary>
    /// <remarks>
    /// <b>One, deliberately.</b> Recovering from a syntax error and continuing produces a second
    /// diagnostic about a state the source never described, and a front end whose contract is
    /// "a validated tree or a refusal" gains nothing by guessing. The first refusal is the true
    /// one; a caller fixes it and asks again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2A6112
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceDiagnosticCode code, string message)
    {
        if (diagnostics.Count > 0)
        {
            return;
        }

        diagnostics.Add(new SliceSourceDiagnostic(code, message, Current.Line, Current.Column));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AB2B33
    // Broiler-Human:        PENDING
    private static string Describe(SliceToken token) =>
        token.Kind == SliceTokenKind.EndOfSource ? "end of source" : token.RawText;
}
