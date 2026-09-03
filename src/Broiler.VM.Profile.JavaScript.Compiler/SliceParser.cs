// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   59
// Annotated:        59/59
// Exempt:           6
// Human-reviewed:   0/59
// IP risk:          None
// Security risk:    High
// Criteria:         20/20
// Resource impact:  2/10 max
// Unverified:       59
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The recursive-descent parser for JavaScript.
/// </summary>
/// <remarks>
/// <para>
/// <b>It parses the language and not the manifest, which is a correction to how it started.</b>
/// JS-3b's first draft refused a <c>function</c> here, as an unparseable reserved word. That put
/// the feature manifest's boundary inside the pass that owns the grammar - contradicting this
/// front end's own decision that the parser rules on nothing - and it left the front end unable to
/// READ the JavaScript whose constructs the roadmap needs counted. The grammar is the language's
/// now; <c>SliceManifest</c> is the manifest's; and the validation stage is the only place a
/// construct is refused.
/// </para>
/// <para>
/// <b>Two shapes of node come out.</b> Constructs the slice lowers get a precise record each,
/// because the lowering is total over them and a lowering that switched on a string would not be.
/// Everything else becomes a <see cref="SliceConstructExpression"/> or
/// <see cref="SliceConstructStatement"/> carrying its kind and its children - enough to walk,
/// count and refuse by name, which is all anything does with them.
/// </para>
/// <para>
/// <b>It reads no ambient state</b> and <b>its recursion is bounded</b>; both properties are
/// unchanged and are the subject of rule N12 and of the nesting corpus respectively.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=32D6FA
// Broiler-Falsified-If: a nesting case terminates the process, a grammar switch is read from anywhere but the options value, or a construct is refused here rather than by the validation stage
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=2D78F6
    // Broiler-Falsified-If: a statement that is not an expression statement over a string literal is admitted into the directive prologue
    // Broiler-Human:        PENDING
    public SliceProgram ParseProgram()
    {
        var span = Here();
        var directives = new System.Collections.Generic.List<SliceStringLiteral>();
        var body = new System.Collections.Generic.List<SliceStatement>();

        // A string literal is a directive only when the statement ends after it. `"use strict" + 1`
        // is an ExpressionStatement and enables nothing.
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

    // ---- statements ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=13B3FB
    // Broiler-Falsified-If: a statement form the grammar has is not parsed into a node a walk can descend through
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
                case SliceTokenKind.Const:
                    return FinishSimpleStatement(ParseVariableStatement());

                case SliceTokenKind.Let:
                    // `let` is an identifier where no declaration can follow: `let = 1` and
                    // `let.x` are legal sloppy-mode programs and this grammar has to admit them
                    // rather than report a declaration with no binding.
                    return StartsDeclaration()
                        ? FinishSimpleStatement(ParseVariableStatement())
                        : ParseExpressionStatement();

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
                    {
                        Advance();
                        var thrown = ParseExpression();
                        ConsumeStatementTerminator();
                        return Statement(span, SliceConstructKind.Throw, thrown);
                    }

                case SliceTokenKind.Try:
                    return ParseTry();

                case SliceTokenKind.Switch:
                    return ParseSwitch();

                case SliceTokenKind.With:
                    {
                        Advance();
                        Expect(SliceTokenKind.OpenParen, "(");
                        var subject = ParseExpression();
                        Expect(SliceTokenKind.CloseParen, ")");
                        return Statement(span, SliceConstructKind.With, subject, ParseStatement());
                    }

                case SliceTokenKind.Debugger:
                    Advance();
                    ConsumeStatementTerminator();
                    return Statement(span, SliceConstructKind.Debugger);

                case SliceTokenKind.Function:
                    return ParseFunction(span);

                case SliceTokenKind.Class:
                    return ParseClass(span);

                case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function &&
                    !Peek(1).PrecededByLineTerminator:
                    Advance();
                    return ParseFunction(span, isAsync: true);

                case SliceTokenKind.Import:
                case SliceTokenKind.Export:
                    return ParseModuleDeclaration();

                case SliceTokenKind.ReservedWord:
                    Advance();
                    ConsumeStatementTerminator();
                    return Statement(span, SliceConstructKind.ReservedWord);

                case SliceTokenKind.Identifier when Peek(1).Kind == SliceTokenKind.Colon:
                    Advance();
                    Advance();
                    return Statement(span, SliceConstructKind.Label, ParseStatement());

                default:
                    return ParseExpressionStatement();
            }
        }
        finally
        {
            Leave();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E449B5
    // Broiler-Human:        PENDING
    private SliceStatement FinishSimpleStatement(SliceStatement statement)
    {
        ConsumeStatementTerminator();
        return statement;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=75594A
    // Broiler-Human:        PENDING
    private SliceStatement ParseExpressionStatement()
    {
        var span = Here();
        var expression = ParseExpression();
        ConsumeStatementTerminator();

        return new SliceExpressionStatement(span, expression);
    }

    /// <summary>Whether a <c>let</c> here begins a declaration rather than being an identifier.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2A4F8D
    // Broiler-Human:        PENDING
    private bool StartsDeclaration() => Peek(1).Kind is SliceTokenKind.Identifier or
        SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace or SliceTokenKind.Yield or
        SliceTokenKind.Await or SliceTokenKind.Async or SliceTokenKind.Of;

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=0212CF
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

        while (diagnostics.Count == 0)
        {
            declarators.Add(ParseDeclarator());

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        return new SliceVariableStatement(span, kind, declarators);
    }

    /// <summary>One declarator, whose binding is a name or a pattern.</summary>
    /// <remarks>
    /// A pattern parses into a construct node and the declarator's name is empty. The validation
    /// stage refuses it by name; nothing downstream has to know that a binding could have been
    /// something other than an identifier, which is what keeps the lowering total.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7C9919
    // Broiler-Falsified-If: a binding pattern is recorded as an identifier, so the validation stage cannot refuse it
    // Broiler-Human:        PENDING
    private SliceDeclarator ParseDeclarator()
    {
        var span = Here();

        if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
        {
            var pattern = ParsePrimary();
            var bound = ParseOptionalInitialiser();

            return new SliceDeclarator(
                span,
                string.Empty,
                bound is null
                    ? Expression(span, SliceConstructKind.Destructuring, pattern)
                    : Expression(span, SliceConstructKind.Destructuring, pattern, bound));
        }

        return new SliceDeclarator(span, BindingName(), ParseOptionalInitialiser());
    }

    /// <summary>The identifier a binding introduces, whatever kind of word it is spelled with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BB6EC3
    // Broiler-Human:        PENDING
    private string BindingName()
    {
        if (Current.Kind is SliceTokenKind.Identifier or SliceTokenKind.ReservedWord ||
            IsContextualKeyword(Current.Kind))
        {
            var name = Current.RawText;
            Advance();
            return name;
        }

        Refuse(SliceSourceDiagnosticCode.ExpectedToken, "a declaration needs a binding identifier");

        return string.Empty;
    }

    /// <summary>Words that are keywords in some position and ordinary identifiers in others.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=099B71
    // Broiler-Human:        PENDING
    private static bool IsContextualKeyword(SliceTokenKind kind) => kind is
        SliceTokenKind.Of or SliceTokenKind.Async or SliceTokenKind.Await or
        SliceTokenKind.Yield or SliceTokenKind.Let or SliceTokenKind.Default or
        SliceTokenKind.Case or SliceTokenKind.Catch or SliceTokenKind.Finally or
        SliceTokenKind.Extends or SliceTokenKind.Static or SliceTokenKind.Get or
        SliceTokenKind.Set;

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

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }

        return new SliceDoWhileStatement(span, body, test);
    }

    /// <summary><c>for</c>, in its three shapes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=1364F1
    // Broiler-Falsified-If: a `for … in` or `for … of` head is parsed as a three-part head, or the reverse
    // Broiler-Human:        PENDING
    private SliceStatement ParseFor()
    {
        var span = Here();
        Advance();

        if (Current.Kind == SliceTokenKind.Await)
        {
            Advance();
        }

        Expect(SliceTokenKind.OpenParen, "(");

        SliceStatement? initialiser = null;

        if (Current.Kind == SliceTokenKind.Semicolon)
        {
            Advance();
        }
        else
        {
            var isDeclaration = Current.Kind is SliceTokenKind.Var or SliceTokenKind.Const ||
                (Current.Kind == SliceTokenKind.Let && StartsDeclaration());

            var headSpan = Here();

            SliceStatement head = isDeclaration
                ? ParseVariableStatement()
                : new SliceExpressionStatement(headSpan, ParseExpression(noIn: true));

            if (Current.Kind is SliceTokenKind.In or SliceTokenKind.Of)
            {
                var kind = Current.Kind == SliceTokenKind.In
                    ? SliceConstructKind.ForIn
                    : SliceConstructKind.ForOf;

                Advance();
                var subject = ParseAssignment();
                Expect(SliceTokenKind.CloseParen, ")");

                return new SliceConstructStatement(
                    span, kind, [head, subject, ParseStatement()]);
            }

            initialiser = head;
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

    /// <summary><c>break</c> and <c>continue</c>, with or without a label.</summary>
    /// <remarks>
    /// A labelled one is a construct rather than the plain statement, because the slice's lowering
    /// has one jump target per loop and a label names another. Reporting them as the same
    /// statement would make the census say this manifest supports something it does not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8BBFA0
    // Broiler-Human:        PENDING
    private SliceStatement ParseBreakOrContinue()
    {
        var span = Here();
        var isBreak = Current.Kind == SliceTokenKind.Break;
        Advance();

        if (Current.Kind == SliceTokenKind.Identifier && !Current.PrecededByLineTerminator)
        {
            Advance();
            ConsumeStatementTerminator();

            return Statement(span, SliceConstructKind.Label);
        }

        ConsumeStatementTerminator();

        return isBreak ? new SliceBreakStatement(span) : new SliceContinueStatement(span);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5491F3
    // Broiler-Human:        PENDING
    private SliceStatement ParseReturn()
    {
        var span = Here();
        Advance();

        if (Current.Kind is SliceTokenKind.Semicolon or SliceTokenKind.CloseBrace or
            SliceTokenKind.EndOfSource || Current.PrecededByLineTerminator)
        {
            ConsumeStatementTerminator();
            return Statement(span, SliceConstructKind.Return);
        }

        var value = ParseExpression();
        ConsumeStatementTerminator();

        return Statement(span, SliceConstructKind.Return, value);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=87387F
    // Broiler-Human:        PENDING
    private SliceStatement ParseTry()
    {
        var span = Here();
        Advance();
        var children = new System.Collections.Generic.List<SliceNode> { ParseBlock() };

        if (Current.Kind == SliceTokenKind.Catch)
        {
            Advance();

            if (Current.Kind == SliceTokenKind.OpenParen)
            {
                Advance();
                children.Add(ParseBindingTarget());
                Expect(SliceTokenKind.CloseParen, ")");
            }

            children.Add(ParseBlock());
        }

        if (Current.Kind == SliceTokenKind.Finally)
        {
            Advance();
            children.Add(ParseBlock());
        }

        return new SliceConstructStatement(span, SliceConstructKind.Try, children);
    }

    /// <summary>A catch or parameter binding, which may be a name or a pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9A3D94
    // Broiler-Human:        PENDING
    private SliceNode ParseBindingTarget()
    {
        var span = Here();

        if (Current.Kind is SliceTokenKind.OpenBracket or SliceTokenKind.OpenBrace)
        {
            return Expression(span, SliceConstructKind.Destructuring, ParsePrimary());
        }

        return new SliceIdentifierReference(span, BindingName());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=22B318
    // Broiler-Human:        PENDING
    private SliceStatement ParseSwitch()
    {
        var span = Here();
        Advance();
        Expect(SliceTokenKind.OpenParen, "(");
        var children = new System.Collections.Generic.List<SliceNode> { ParseExpression() };
        Expect(SliceTokenKind.CloseParen, ")");
        Expect(SliceTokenKind.OpenBrace, "{");

        while (Current.Kind is not (SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.Case)
            {
                Advance();
                children.Add(ParseExpression());
                Expect(SliceTokenKind.Colon, ":");
            }
            else if (Current.Kind == SliceTokenKind.Default)
            {
                Advance();
                Expect(SliceTokenKind.Colon, ":");
            }
            else
            {
                children.Add(ParseStatement());
            }
        }

        Expect(SliceTokenKind.CloseBrace, "}");

        return new SliceConstructStatement(span, SliceConstructKind.Switch, children);
    }

    /// <summary>An <c>import</c> or <c>export</c> declaration.</summary>
    /// <remarks>
    /// The clause forms are scanned to the end of the statement rather than parsed. This manifest
    /// admits no module of any shape, so what a census needs is that the file contains one and
    /// where; parsing the clause grammar would build a tree nothing reads. The scan stops at a
    /// statement terminator so the rest of the file still parses, which is what keeps a module's
    /// other constructs countable.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=C37D4B
    // Broiler-Human:        PENDING
    private SliceStatement ParseModuleDeclaration()
    {
        var span = Here();
        var kind = Current.Kind == SliceTokenKind.Import
            ? SliceConstructKind.Import
            : SliceConstructKind.Export;

        // `import(` is a dynamic import and `import.meta` a meta-property: both are expressions.
        if (kind == SliceConstructKind.Import &&
            Peek(1).Kind is SliceTokenKind.OpenParen or SliceTokenKind.Dot)
        {
            return ParseExpressionStatement();
        }

        Advance();

        if (kind == SliceConstructKind.Export && Current.Kind == SliceTokenKind.Default)
        {
            Advance();
            var exported = ParseAssignment();
            ConsumeStatementTerminator();

            return Statement(span, kind, exported);
        }

        if (kind == SliceConstructKind.Export && Current.Kind is SliceTokenKind.Var or
            SliceTokenKind.Let or SliceTokenKind.Const or SliceTokenKind.Function or
            SliceTokenKind.Class)
        {
            return Statement(span, kind, ParseStatement());
        }

        var opened = 0;

        while (Current.Kind != SliceTokenKind.EndOfSource)
        {
            if (Current.Kind == SliceTokenKind.OpenBrace)
            {
                opened++;
            }
            else if (Current.Kind == SliceTokenKind.CloseBrace)
            {
                opened--;
            }
            else if (opened == 0 && Current.Kind == SliceTokenKind.Semicolon)
            {
                break;
            }

            Advance();

            if (opened == 0 && Current.PrecededByLineTerminator)
            {
                break;
            }
        }

        ConsumeStatementTerminator();

        return Statement(span, kind);
    }

    // ---- functions and classes ------------------------------------------------------------------

    /// <summary>A function declaration or expression, in any of its flavours.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=29B058
    // Broiler-Falsified-If: a generator or an async function is counted as a plain function
    // Broiler-Human:        PENDING
    private SliceConstructStatement ParseFunction(SliceSourceSpan span, bool isAsync = false)
    {
        Expect(SliceTokenKind.Function, "function");
        var isGenerator = Current.Kind == SliceTokenKind.Star;

        if (isGenerator)
        {
            Advance();
        }

        if (Current.Kind is SliceTokenKind.Identifier || IsContextualKeyword(Current.Kind))
        {
            Advance();
        }

        var children = new System.Collections.Generic.List<SliceNode>();
        children.AddRange(ParseParameters());
        children.Add(ParseBlock());

        var kind = isGenerator
            ? SliceConstructKind.Generator
            : isAsync ? SliceConstructKind.AsyncFunction : SliceConstructKind.Function;

        return new SliceConstructStatement(span, kind, children);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=962A0C
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<SliceNode> ParseParameters()
    {
        var parameters = new System.Collections.Generic.List<SliceNode>();
        Expect(SliceTokenKind.OpenParen, "(");

        while (Current.Kind is not (SliceTokenKind.CloseParen or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            var span = Here();

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                parameters.Add(new SliceConstructExpression(
                    span, SliceConstructKind.RestParameter, [ParseBindingTarget()]));
            }
            else
            {
                var target = ParseBindingTarget();

                if (Current.Kind == SliceTokenKind.Equals)
                {
                    Advance();
                    parameters.Add(new SliceConstructExpression(
                        span, SliceConstructKind.DefaultParameter, [target, ParseAssignment()]));
                }
                else
                {
                    parameters.Add(target);
                }
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

    /// <summary>A class declaration or expression, with its body walked for its members.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=79DEE6
    // Broiler-Falsified-If: a class body's members are not walked, so what is inside a class is counted as nothing
    // Broiler-Human:        PENDING
    private SliceConstructStatement ParseClass(SliceSourceSpan span)
    {
        Expect(SliceTokenKind.Class, "class");
        var children = new System.Collections.Generic.List<SliceNode>();

        if (Current.Kind is SliceTokenKind.Identifier ||
            (IsContextualKeyword(Current.Kind) && Current.Kind != SliceTokenKind.Extends))
        {
            Advance();
        }

        if (Current.Kind == SliceTokenKind.Extends)
        {
            Advance();
            children.Add(ParseCallChain());
        }

        Expect(SliceTokenKind.OpenBrace, "{");

        while (Current.Kind is not (SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            if (Current.Kind == SliceTokenKind.Semicolon)
            {
                Advance();
                continue;
            }

            children.Add(ParseMember(inClass: true));
        }

        Expect(SliceTokenKind.CloseBrace, "}");

        return new SliceConstructStatement(span, SliceConstructKind.Class, children);
    }

    /// <summary>One member of a class body or an object literal.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=236DBE
    // Broiler-Falsified-If: a property named `get`, `set`, `static` or `async` is read as an accessor or a modifier
    // Broiler-Human:        PENDING
    private SliceNode ParseMember(bool inClass)
    {
        var span = Here();
        var children = new System.Collections.Generic.List<SliceNode>();
        var kind = inClass ? SliceConstructKind.Class : SliceConstructKind.ObjectLiteral;

        // Modifiers, each of which is also a legal key on its own - `{ get: 1 }` is a property
        // named `get` - so each is consumed only when something follows that a key can precede.
        while (Current.Kind is SliceTokenKind.Static or SliceTokenKind.Async && !KeyEndsHere(1))
        {
            Advance();
        }

        if (Current.Kind == SliceTokenKind.Star)
        {
            Advance();
            kind = SliceConstructKind.Generator;
        }

        if (Current.Kind is SliceTokenKind.Get or SliceTokenKind.Set && !KeyEndsHere(1))
        {
            kind = Current.Kind == SliceTokenKind.Get
                ? SliceConstructKind.Getter
                : SliceConstructKind.Setter;

            Advance();
        }

        // The key.
        if (Current.Kind == SliceTokenKind.OpenBracket)
        {
            Advance();
            children.Add(Expression(span, SliceConstructKind.ComputedProperty, ParseAssignment()));
            Expect(SliceTokenKind.CloseBracket, "]");
        }
        else
        {
            Advance();
        }

        if (Current.Kind == SliceTokenKind.OpenParen)
        {
            children.AddRange(ParseParameters());
            children.Add(ParseBlock());

            return new SliceConstructExpression(
                span,
                kind is SliceConstructKind.Getter or SliceConstructKind.Setter or
                    SliceConstructKind.Generator ? kind : SliceConstructKind.Function,
                children);
        }

        if (Current.Kind is SliceTokenKind.Colon or SliceTokenKind.Equals)
        {
            Advance();
            children.Add(ParseAssignment());

            return new SliceConstructExpression(span, kind, children);
        }

        return new SliceConstructExpression(
            span, inClass ? kind : SliceConstructKind.ShorthandProperty, children);
    }

    /// <summary>Whether a key ends at the token <paramref name="ahead"/> from here.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A63C02
    // Broiler-Human:        PENDING
    private bool KeyEndsHere(int ahead) => Peek(ahead).Kind is SliceTokenKind.OpenParen or
        SliceTokenKind.Colon or SliceTokenKind.Comma or SliceTokenKind.CloseBrace or
        SliceTokenKind.Equals or SliceTokenKind.Semicolon;

    // ---- expressions ----------------------------------------------------------------------------

    /// <summary>An expression, comma operator included.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1467D2
    // Broiler-Human:        PENDING
    private SliceExpression ParseExpression(bool noIn = false)
    {
        var span = Here();
        var first = ParseAssignment(noIn);

        if (Current.Kind != SliceTokenKind.Comma)
        {
            return first;
        }

        var items = new System.Collections.Generic.List<SliceNode> { first };

        while (Current.Kind == SliceTokenKind.Comma && diagnostics.Count == 0)
        {
            Advance();
            items.Add(ParseAssignment(noIn));
        }

        return new SliceConstructExpression(span, SliceConstructKind.Sequence, items);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=7DFF55
    // Broiler-Falsified-If: an arrow function's head is parsed as a parenthesised expression, or a compound assignment is recorded as a plain one
    // Broiler-Human:        PENDING
    private SliceExpression ParseAssignment(bool noIn = false)
    {
        if (!Enter())
        {
            return new SliceNumericLiteral(Here(), 0, false);
        }

        try
        {
            var span = Here();

            if (ArrowFollows())
            {
                return ParseArrow(span);
            }

            if (Current.Kind == SliceTokenKind.Yield)
            {
                Advance();

                if (Current.Kind == SliceTokenKind.Star)
                {
                    Advance();
                }

                return StartsExpression()
                    ? Expression(span, SliceConstructKind.Yield, ParseAssignment(noIn))
                    : new SliceConstructExpression(span, SliceConstructKind.Yield, []);
            }

            var left = ParseConditional(noIn);

            if (Current.Kind == SliceTokenKind.Equals)
            {
                Advance();
                return new SliceAssignmentExpression(span, left, ParseAssignment(noIn));
            }

            if (Current.Kind == SliceTokenKind.CompoundAssign)
            {
                Advance();
                return Expression(
                    span, SliceConstructKind.CompoundAssignment, left, ParseAssignment(noIn));
            }

            return left;
        }
        finally
        {
            Leave();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=23F73C
    // Broiler-Human:        PENDING
    private SliceExpression ParseConditional(bool noIn)
    {
        var span = Here();
        var test = ParseBinary(0, noIn);

        if (Current.Kind != SliceTokenKind.Question)
        {
            return test;
        }

        Advance();
        var whenTrue = ParseAssignment();
        Expect(SliceTokenKind.Colon, ":");

        return new SliceConditionalExpression(span, test, whenTrue, ParseAssignment(noIn));
    }

    /// <summary>Precedence climbing over the binary, logical and relational operators.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=0FFC90
    // Broiler-Falsified-If: the tree this builds groups an operator differently from the language's precedence and associativity
    // Broiler-Human:        PENDING
    private SliceExpression ParseBinary(int minimumPrecedence, bool noIn)
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
                var op = Current.Kind;

                if (op == SliceTokenKind.In && noIn)
                {
                    return left;
                }

                var precedence = Precedence(op);

                if (precedence < 0 || precedence < minimumPrecedence || diagnostics.Count > 0)
                {
                    return left;
                }

                var span = Here();
                Advance();

                // `**` is the one right-associative binary operator, so its right operand binds at
                // its own level rather than one above it.
                var right = op == SliceTokenKind.StarStar
                    ? ParseBinary(precedence, noIn)
                    : ParseBinary(precedence + 1, noIn);

                left = Combine(span, op, left, right);
            }
        }
        finally
        {
            Leave();
        }
    }

    /// <summary>Builds the node one binary operator makes, precise or construct.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1E98FB
    // Broiler-Falsified-If: an operator outside the manifest is built as a precise node, so the validation stage never sees it
    // Broiler-Human:        PENDING
    private static SliceExpression Combine(
        SliceSourceSpan span, SliceTokenKind op, SliceExpression left, SliceExpression right) =>
        op switch
        {
            SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar =>
                new SliceLogicalExpression(span, op, left, right),
            SliceTokenKind.QuestionQuestion =>
                new SliceConstructExpression(
                    span, SliceConstructKind.NullishCoalescing, [left, right]),
            SliceTokenKind.EqualsEquals or SliceTokenKind.BangEquals =>
                new SliceConstructExpression(span, SliceConstructKind.LooseEquality, [left, right]),
            SliceTokenKind.StarStar =>
                new SliceConstructExpression(span, SliceConstructKind.Exponentiation, [left, right]),
            SliceTokenKind.Instanceof =>
                new SliceConstructExpression(span, SliceConstructKind.Instanceof, [left, right]),
            SliceTokenKind.In =>
                new SliceConstructExpression(span, SliceConstructKind.In, [left, right]),
            _ => new SliceBinaryExpression(span, op, left, right),
        };

    /// <summary>The precedence of a binary operator, or -1 when the token is not one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=793D6C
    // Broiler-Falsified-If: two operators the language separates share a level here, or the order differs from the language's
    // Broiler-Human:        PENDING
    private static int Precedence(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.QuestionQuestion or SliceTokenKind.BarBar => 1,
        SliceTokenKind.AmpersandAmpersand => 2,
        SliceTokenKind.Bar => 3,
        SliceTokenKind.Caret => 4,
        SliceTokenKind.Ampersand => 5,
        SliceTokenKind.EqualsEqualsEquals or SliceTokenKind.BangEqualsEquals or
        SliceTokenKind.EqualsEquals or SliceTokenKind.BangEquals => 6,
        SliceTokenKind.LessThan or SliceTokenKind.LessThanEquals or
        SliceTokenKind.GreaterThan or SliceTokenKind.GreaterThanEquals or
        SliceTokenKind.Instanceof or SliceTokenKind.In => 7,
        SliceTokenKind.LessThanLessThan or SliceTokenKind.GreaterThanGreaterThan or
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => 8,
        SliceTokenKind.Plus or SliceTokenKind.Minus => 9,
        SliceTokenKind.Star or SliceTokenKind.Slash or SliceTokenKind.Percent => 10,
        SliceTokenKind.StarStar => 11,
        _ => -1,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=705846
    // Broiler-Falsified-If: a unary operator outside the manifest is built as a precise node, or its operand is not walked
    // Broiler-Human:        PENDING
    private SliceExpression ParseUnary()
    {
        var span = Here();

        switch (Current.Kind)
        {
            case SliceTokenKind.Plus:
            case SliceTokenKind.Minus:
            case SliceTokenKind.Bang:
                {
                    var op = Current.Kind;
                    Advance();
                    return new SliceUnaryExpression(span, op, ParseUnary());
                }

            case SliceTokenKind.Tilde:
                Advance();
                return Expression(span, SliceConstructKind.BitwiseNot, ParseUnary());

            case SliceTokenKind.Typeof:
                Advance();
                return Expression(span, SliceConstructKind.TypeOf, ParseUnary());

            case SliceTokenKind.Void:
                Advance();
                return Expression(span, SliceConstructKind.Void, ParseUnary());

            case SliceTokenKind.Delete:
                Advance();
                return Expression(span, SliceConstructKind.Delete, ParseUnary());

            case SliceTokenKind.Await:
                Advance();
                return Expression(span, SliceConstructKind.Await, ParseUnary());

            case SliceTokenKind.PlusPlus:
            case SliceTokenKind.MinusMinus:
                Advance();
                return Expression(span, SliceConstructKind.Update, ParseUnary());

            default:
                return ParsePostfix();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A576E8
    // Broiler-Human:        PENDING
    private SliceExpression ParsePostfix()
    {
        var span = Here();
        var operand = ParseCallChain();

        if (Current.Kind is SliceTokenKind.PlusPlus or SliceTokenKind.MinusMinus &&
            !Current.PrecededByLineTerminator)
        {
            Advance();
            return Expression(span, SliceConstructKind.Update, operand);
        }

        return operand;
    }

    /// <summary>Member access, calls and <c>new</c>, left to right.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=309170
    // Broiler-Falsified-If: a link of a chain drops its target, so a walk under it counts nothing
    // Broiler-Human:        PENDING
    private SliceExpression ParseCallChain()
    {
        if (!Enter())
        {
            return new SliceNumericLiteral(Here(), 0, false);
        }

        try
        {
            var span = Here();
            SliceExpression current;

            if (Current.Kind == SliceTokenKind.New)
            {
                Advance();

                if (Current.Kind == SliceTokenKind.Dot)
                {
                    // `new.target`.
                    Advance();
                    Advance();
                    current = new SliceConstructExpression(span, SliceConstructKind.New, []);
                }
                else
                {
                    var callee = ParseCallChain();

                    var arguments = Current.Kind == SliceTokenKind.OpenParen
                        ? ParseArguments()
                        : new System.Collections.Generic.List<SliceNode>();

                    arguments.Insert(0, callee);
                    current = new SliceConstructExpression(span, SliceConstructKind.New, arguments);
                }
            }
            else
            {
                current = ParsePrimary();
            }

            while (diagnostics.Count == 0)
            {
                var linkSpan = Here();

                switch (Current.Kind)
                {
                    case SliceTokenKind.Dot:
                        Advance();
                        Advance();
                        current = Expression(linkSpan, SliceConstructKind.MemberAccess, current);
                        continue;

                    case SliceTokenKind.QuestionDot:
                        current = ParseOptionalLink(linkSpan, current);
                        continue;

                    case SliceTokenKind.OpenBracket:
                        {
                            Advance();
                            var key = ParseExpression();
                            Expect(SliceTokenKind.CloseBracket, "]");
                            current = Expression(
                                linkSpan, SliceConstructKind.ComputedMemberAccess, current, key);
                            continue;
                        }

                    case SliceTokenKind.OpenParen:
                        {
                            var arguments = ParseArguments();
                            arguments.Insert(0, current);
                            current = new SliceConstructExpression(
                                linkSpan, SliceConstructKind.Call, arguments);
                            continue;
                        }

                    case SliceTokenKind.TemplateLiteral:
                        Advance();
                        current = Expression(linkSpan, SliceConstructKind.TaggedTemplate, current);
                        continue;

                    default:
                        return current;
                }
            }

            return current;
        }
        finally
        {
            Leave();
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9240AC
    // Broiler-Human:        PENDING
    private SliceExpression ParseOptionalLink(SliceSourceSpan span, SliceExpression target)
    {
        Advance();

        if (Current.Kind == SliceTokenKind.OpenParen)
        {
            var arguments = ParseArguments();
            arguments.Insert(0, target);

            return new SliceConstructExpression(span, SliceConstructKind.OptionalChain, arguments);
        }

        if (Current.Kind == SliceTokenKind.OpenBracket)
        {
            Advance();
            var key = ParseExpression();
            Expect(SliceTokenKind.CloseBracket, "]");

            return Expression(span, SliceConstructKind.OptionalChain, target, key);
        }

        Advance();

        return Expression(span, SliceConstructKind.OptionalChain, target);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=389D24
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<SliceNode> ParseArguments()
    {
        var arguments = new System.Collections.Generic.List<SliceNode>();
        Expect(SliceTokenKind.OpenParen, "(");

        while (Current.Kind is not (SliceTokenKind.CloseParen or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            var span = Here();

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                arguments.Add(Expression(span, SliceConstructKind.Spread, ParseAssignment()));
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

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=871855
    // Broiler-Falsified-If: a literal form the grammar has produces no node, so the construct it is goes uncounted
    // Broiler-Human:        PENDING
    private SliceExpression ParsePrimary()
    {
        var span = Here();
        var token = Current;

        switch (token.Kind)
        {
            case SliceTokenKind.NumericLiteral:
                Advance();

                // A BigInt is a distinct value kind and not a number that happens to end in `n`.
                return token.RawText.EndsWith('n')
                    ? new SliceConstructExpression(span, SliceConstructKind.BigInt, [])
                    : new SliceNumericLiteral(span, token.NumericValue, token.IsLegacyOctal);

            case SliceTokenKind.True:
                Advance();
                return new SliceBooleanLiteral(span, true);

            case SliceTokenKind.False:
                Advance();
                return new SliceBooleanLiteral(span, false);

            case SliceTokenKind.Identifier:
                Advance();

                // A private name is a class member and never a binding a slice program could
                // resolve, so it is counted as the construct it is rather than as a free name.
                return token.RawText.StartsWith('#')
                    ? new SliceConstructExpression(span, SliceConstructKind.PrivateName, [])
                    : new SliceIdentifierReference(span, token.RawText);

            case SliceTokenKind.StringLiteral:
                Advance();
                return new SliceConstructExpression(
                    span,
                    SliceConstructKind.StringValue,
                    [new SliceStringLiteral(span, token.StringValue, token.RawText)]);

            case SliceTokenKind.Null:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.Null, []);

            case SliceTokenKind.This:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.This, []);

            case SliceTokenKind.Super:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.Super, []);

            case SliceTokenKind.RegularExpressionLiteral:
                Advance();
                return new SliceConstructExpression(
                    span, SliceConstructKind.RegularExpression, []);

            case SliceTokenKind.TemplateLiteral:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.Template, []);

            case SliceTokenKind.Function:
                return ToExpression(ParseFunction(span));

            case SliceTokenKind.Class:
                return ToExpression(ParseClass(span));

            case SliceTokenKind.Async when Peek(1).Kind == SliceTokenKind.Function:
                Advance();
                return ToExpression(ParseFunction(span, isAsync: true));

            case SliceTokenKind.Import:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.Import, []);

            case SliceTokenKind.OpenBracket:
                return ParseArrayLiteral(span);

            case SliceTokenKind.OpenBrace:
                return ParseObjectLiteral(span);

            case SliceTokenKind.OpenParen:
                {
                    Advance();
                    var inner = ParseExpression();
                    Expect(SliceTokenKind.CloseParen, ")");
                    return inner;
                }

            case SliceTokenKind.ReservedWord:
                Advance();
                return new SliceConstructExpression(span, SliceConstructKind.ReservedWord, []);

            default:
                if (IsContextualKeyword(token.Kind))
                {
                    Advance();
                    return new SliceIdentifierReference(span, token.RawText);
                }

                Refuse(
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    $"`{Describe(token)}` begins no expression this grammar defines");

                Advance();
                return new SliceNumericLiteral(span, 0, false);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7DD6B9
    // Broiler-Human:        PENDING
    private static SliceExpression ToExpression(SliceConstructStatement statement) =>
        new SliceConstructExpression(statement.Span, statement.Kind, statement.Children);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=718093
    // Broiler-Human:        PENDING
    private SliceExpression ParseArrayLiteral(SliceSourceSpan span)
    {
        Expect(SliceTokenKind.OpenBracket, "[");
        var elements = new System.Collections.Generic.List<SliceNode>();

        while (Current.Kind is not (SliceTokenKind.CloseBracket or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            // An elision: `[1, , 2]` has a hole, and a hole is not an expression.
            if (Current.Kind == SliceTokenKind.Comma)
            {
                Advance();
                continue;
            }

            var elementSpan = Here();

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                elements.Add(Expression(elementSpan, SliceConstructKind.Spread, ParseAssignment()));
            }
            else
            {
                elements.Add(ParseAssignment());
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }
        }

        Expect(SliceTokenKind.CloseBracket, "]");

        return new SliceConstructExpression(span, SliceConstructKind.ArrayLiteral, elements);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=ADB25D
    // Broiler-Human:        PENDING
    private SliceExpression ParseObjectLiteral(SliceSourceSpan span)
    {
        Expect(SliceTokenKind.OpenBrace, "{");
        var members = new System.Collections.Generic.List<SliceNode>();

        while (Current.Kind is not (SliceTokenKind.CloseBrace or SliceTokenKind.EndOfSource) &&
               diagnostics.Count == 0)
        {
            var memberSpan = Here();

            if (Current.Kind == SliceTokenKind.DotDotDot)
            {
                Advance();
                members.Add(Expression(memberSpan, SliceConstructKind.Spread, ParseAssignment()));
            }
            else
            {
                members.Add(ParseMember(inClass: false));
            }

            if (Current.Kind != SliceTokenKind.Comma)
            {
                break;
            }

            Advance();
        }

        Expect(SliceTokenKind.CloseBrace, "}");

        return new SliceConstructExpression(span, SliceConstructKind.ObjectLiteral, members);
    }

    // ---- arrow functions --------------------------------------------------------------------------

    /// <summary>
    /// Whether an arrow function starts here, decided by a bounded scan rather than a backtrack.
    /// </summary>
    /// <remarks>
    /// <b>The cover grammar is the one place this parser cannot decide from one token.</b>
    /// <c>(a, b)</c> is a parenthesised sequence and <c>(a, b) =&gt;</c> is a parameter list, and
    /// nothing before the <c>)</c> distinguishes them. A backtracking parser re-parses; this one
    /// scans forward over balanced brackets for the matching <c>)</c> and looks at the token after
    /// it, which is linear in the parenthesised text and never re-parses anything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=4D3013
    // Broiler-Falsified-If: a parenthesised expression is parsed as a parameter list, or an arrow's head is parsed as an expression
    // Broiler-Human:        PENDING
    private bool ArrowFollows()
    {
        var start = at;

        if (Current.Kind == SliceTokenKind.Async && !Peek(1).PrecededByLineTerminator &&
            Peek(1).Kind is SliceTokenKind.Identifier or SliceTokenKind.OpenParen)
        {
            start++;
        }

        var head = tokens[System.Math.Min(start, tokens.Length - 1)];

        if (head.Kind is SliceTokenKind.Identifier || IsContextualKeyword(head.Kind))
        {
            return tokens[System.Math.Min(start + 1, tokens.Length - 1)].Kind ==
                SliceTokenKind.EqualsGreaterThan;
        }

        if (head.Kind != SliceTokenKind.OpenParen)
        {
            return false;
        }

        var open = 0;

        for (var scan = start; scan < tokens.Length; scan++)
        {
            switch (tokens[scan].Kind)
            {
                case SliceTokenKind.OpenParen:
                case SliceTokenKind.OpenBracket:
                case SliceTokenKind.OpenBrace:
                    open++;
                    break;

                case SliceTokenKind.CloseParen:
                case SliceTokenKind.CloseBracket:
                case SliceTokenKind.CloseBrace:
                    open--;

                    if (open == 0)
                    {
                        return tokens[System.Math.Min(scan + 1, tokens.Length - 1)].Kind ==
                            SliceTokenKind.EqualsGreaterThan;
                    }

                    break;

                case SliceTokenKind.EndOfSource:
                    return false;

                default:
                    break;
            }
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=1BA262
    // Broiler-Falsified-If: an arrow's parameters or body are dropped, so what is inside it counts as nothing
    // Broiler-Human:        PENDING
    private SliceExpression ParseArrow(SliceSourceSpan span)
    {
        var isAsync = Current.Kind == SliceTokenKind.Async;

        if (isAsync)
        {
            Advance();
        }

        var children = new System.Collections.Generic.List<SliceNode>();

        if (Current.Kind == SliceTokenKind.OpenParen)
        {
            children.AddRange(ParseParameters());
        }
        else
        {
            children.Add(new SliceIdentifierReference(Here(), BindingName()));
        }

        Expect(SliceTokenKind.EqualsGreaterThan, "=>");

        children.Add(Current.Kind == SliceTokenKind.OpenBrace
            ? ParseBlock()
            : new SliceExpressionStatement(Here(), ParseAssignment()));

        return new SliceConstructExpression(
            span,
            isAsync ? SliceConstructKind.AsyncFunction : SliceConstructKind.ArrowFunction,
            children);
    }

    // ---- shared machinery -------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D87527
    // Broiler-Human:        PENDING
    private static SliceConstructStatement Statement(
        SliceSourceSpan span, SliceConstructKind kind, params SliceNode[] children) =>
        new(span, kind, children);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=0819EF
    // Broiler-Human:        PENDING
    private static SliceConstructExpression Expression(
        SliceSourceSpan span, SliceConstructKind kind, params SliceNode[] children) =>
        new(span, kind, children);

    /// <summary>Whether the current token can begin an expression.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=40C801
    // Broiler-Human:        PENDING
    private bool StartsExpression() => Current.Kind is not (SliceTokenKind.EndOfSource or
        SliceTokenKind.Semicolon or SliceTokenKind.CloseBrace or SliceTokenKind.CloseParen or
        SliceTokenKind.CloseBracket or SliceTokenKind.Comma or SliceTokenKind.Colon) &&
        !Current.PrecededByLineTerminator;

    /// <summary>Consumes a statement's terminating semicolon, inserting one where the language does.</summary>
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4CC569
    // Broiler-Human:        PENDING
    private SliceToken Peek(int ahead) => tokens[System.Math.Min(at + ahead, tokens.Length - 1)];

    /// <summary>Whether the statement would end immediately after the current token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=CA6592
    // Broiler-Falsified-If: a string literal that is not a whole statement is admitted into the directive prologue
    // Broiler-Human:        PENDING
    private bool StatementEndsAfterCurrent()
    {
        var next = Peek(1);

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

    /// <summary>Records a refusal, at most one per parse.</summary>
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
