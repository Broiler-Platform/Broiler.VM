// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   47
// Annotated:        47/47
// Exempt:           3
// Human-reviewed:   0/47
// IP risk:          None
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       47
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The base of every node in the wide surface's syntax tree.</summary>
/// <remarks>
/// <para>
/// <b>This tree is precise where the slice tree is not.</b> The slice front end parses the whole
/// grammar but folds everything outside its manifest into one <c>SliceConstructExpression</c>
/// carrying an untyped child list, because its job is to COUNT constructs and refuse them. A
/// lowering cannot be written against that: it has no way to ask which child of a call is the
/// callee. So the wide surface has its own tree, with a record per production it lowers, and the
/// two front ends share the tokenizer and nothing else.
/// </para>
/// <para>
/// Every node carries a source span, and the lowering writes the artifact's position table from
/// them.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=73EDF1
// Broiler-Human:        PENDING
internal abstract record JsNode(SliceSourceSpan Span);

/// <summary>The base of every expression.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F68CED
// Broiler-Human:        PENDING
internal abstract record JsExpression(SliceSourceSpan Span) : JsNode(Span);

/// <summary>The base of every statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B574AC
// Broiler-Human:        PENDING
internal abstract record JsStatement(SliceSourceSpan Span) : JsNode(Span);

/// <summary>A numeric literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=167200
// Broiler-Human:        PENDING
internal sealed record JsNumberLiteral(SliceSourceSpan Span, double Value, bool IsLegacyOctal)
    : JsExpression(Span);

/// <summary>A string literal, with the raw text a directive prologue needs.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D1DFE4
// Broiler-Human:        PENDING
internal sealed record JsStringLiteral(SliceSourceSpan Span, string Value, string RawText)
    : JsExpression(Span);

/// <summary>A <c>true</c> or <c>false</c> literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3F1F9A
// Broiler-Human:        PENDING
internal sealed record JsBooleanLiteral(SliceSourceSpan Span, bool Value) : JsExpression(Span);

/// <summary>The <c>null</c> literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=566619
// Broiler-Human:        PENDING
internal sealed record JsNullLiteral(SliceSourceSpan Span) : JsExpression(Span);

/// <summary>A regular-expression literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B1BF95
// Broiler-Human:        PENDING
internal sealed record JsRegExpLiteral(SliceSourceSpan Span, string Pattern, string Flags)
    : JsExpression(Span);

/// <summary>A name used as a value.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9086F5
// Broiler-Human:        PENDING
internal sealed record JsIdentifier(SliceSourceSpan Span, string Name) : JsExpression(Span);

/// <summary><c>this</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3DF5BE
// Broiler-Human:        PENDING
internal sealed record JsThisExpression(SliceSourceSpan Span) : JsExpression(Span);

/// <summary>An array literal. A <see langword="null"/> element is an elision.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3CCFD5
// Broiler-Human:        PENDING
internal sealed record JsArrayLiteral(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<JsExpression?> Elements) : JsExpression(Span);

/// <summary>What one entry of an object literal is.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9B9F52
// Broiler-Human:        PENDING
internal enum JsPropertyKind
{
    /// <summary>An ordinary <c>key: value</c> entry.</summary>
    Init = 0,

    /// <summary>A getter.</summary>
    Get = 1,

    /// <summary>A setter.</summary>
    Set = 2,
}

/// <summary>One entry of an object literal.</summary>
/// <param name="Span">Where the entry begins.</param>
/// <param name="Kind">Whether it defines a value, a getter or a setter.</param>
/// <param name="Key">The literal key, when it is not computed.</param>
/// <param name="Computed">The key expression, when it is computed.</param>
/// <param name="Value">The value, the getter or the setter.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D3377B
// Broiler-Human:        PENDING
internal sealed record JsObjectEntry(
    SliceSourceSpan Span,
    JsPropertyKind Kind,
    string Key,
    JsExpression? Computed,
    JsExpression Value) : JsNode(Span);

/// <summary>An object literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3CE3F5
// Broiler-Human:        PENDING
internal sealed record JsObjectLiteral(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<JsObjectEntry> Entries) : JsExpression(Span);

/// <summary>One function: a declaration, an expression, an arrow or a program body.</summary>
/// <param name="Span">Where the function begins.</param>
/// <param name="Name">The function's name, empty when it is anonymous.</param>
/// <param name="Parameters">The parameter names, in order.</param>
/// <param name="Body">The statement list.</param>
/// <param name="IsArrow">Whether this is an arrow function, which has no <c>this</c> of its own.</param>
/// <param name="IsStrict">Whether the body is strict-mode code.</param>
/// <param name="Directives">The directive prologue, which is where <c>use strict</c> lives.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6E534E
// Broiler-Human:        PENDING
internal sealed record JsFunctionNode(
    SliceSourceSpan Span,
    string Name,
    System.Collections.Generic.IReadOnlyList<string> Parameters,
    System.Collections.Generic.IReadOnlyList<JsStatement> Body,
    bool IsArrow,
    bool IsStrict,
    System.Collections.Generic.IReadOnlyList<JsStringLiteral> Directives) : JsNode(Span);

/// <summary>A function expression.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E66830
// Broiler-Human:        PENDING
internal sealed record JsFunctionExpression(SliceSourceSpan Span, JsFunctionNode Function)
    : JsExpression(Span);

/// <summary>A unary operator applied to one operand.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=547101
// Broiler-Human:        PENDING
internal sealed record JsUnaryExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, JsExpression Operand) : JsExpression(Span);

/// <summary><c>++</c> or <c>--</c>, prefix or postfix.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=56BFCC
// Broiler-Human:        PENDING
internal sealed record JsUpdateExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, JsExpression Operand, bool Prefix)
    : JsExpression(Span);

/// <summary>A binary operator applied to two operands.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FBE472
// Broiler-Human:        PENDING
internal sealed record JsBinaryExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, JsExpression Left, JsExpression Right)
    : JsExpression(Span);

/// <summary><c>&amp;&amp;</c>, <c>||</c> or <c>??</c>, which do not evaluate their right operand.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EFB872
// Broiler-Human:        PENDING
internal sealed record JsLogicalExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, JsExpression Left, JsExpression Right)
    : JsExpression(Span);

/// <summary>An assignment, simple or compound.</summary>
/// <param name="Operator">
/// <see cref="SliceTokenKind.Equals"/> for a simple assignment, or the token of the compound form.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8CA9E6
// Broiler-Human:        PENDING
internal sealed record JsAssignmentExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, JsExpression Target, JsExpression Value)
    : JsExpression(Span);

/// <summary><c>a ? b : c</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E979E2
// Broiler-Human:        PENDING
internal sealed record JsConditionalExpression(
    SliceSourceSpan Span, JsExpression Test, JsExpression WhenTrue, JsExpression WhenFalse)
    : JsExpression(Span);

/// <summary>A property access, dotted or computed.</summary>
/// <param name="Name">The property name, when the access is dotted.</param>
/// <param name="Computed">The key expression, when the access is computed.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=691B1F
// Broiler-Human:        PENDING
internal sealed record JsMemberExpression(
    SliceSourceSpan Span, JsExpression Target, string Name, JsExpression? Computed)
    : JsExpression(Span);

/// <summary>A call.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=58E9A9
// Broiler-Human:        PENDING
internal sealed record JsCallExpression(
    SliceSourceSpan Span,
    JsExpression Callee,
    System.Collections.Generic.IReadOnlyList<JsExpression> Arguments) : JsExpression(Span);

/// <summary><c>new</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=44BD6A
// Broiler-Human:        PENDING
internal sealed record JsNewExpression(
    SliceSourceSpan Span,
    JsExpression Callee,
    System.Collections.Generic.IReadOnlyList<JsExpression> Arguments) : JsExpression(Span);

/// <summary>The comma operator.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=75893E
// Broiler-Human:        PENDING
internal sealed record JsSequenceExpression(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<JsExpression> Expressions) : JsExpression(Span);

/// <summary>One declarator of a variable statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2C29D4
// Broiler-Human:        PENDING
internal sealed record JsDeclarator(SliceSourceSpan Span, string Name, JsExpression? Initialiser)
    : JsNode(Span);

/// <summary>A <c>var</c>, <c>let</c> or <c>const</c> statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9AEB53
// Broiler-Human:        PENDING
internal sealed record JsVariableStatement(
    SliceSourceSpan Span,
    SliceDeclarationKind Kind,
    System.Collections.Generic.IReadOnlyList<JsDeclarator> Declarators) : JsStatement(Span);

/// <summary>An expression evaluated for its value.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=96699D
// Broiler-Human:        PENDING
internal sealed record JsExpressionStatement(SliceSourceSpan Span, JsExpression Expression)
    : JsStatement(Span);

/// <summary>A braced statement list.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C4B8A8
// Broiler-Human:        PENDING
internal sealed record JsBlockStatement(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<JsStatement> Body) : JsStatement(Span);

/// <summary><c>if</c>, with an optional <c>else</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=AAB0D2
// Broiler-Human:        PENDING
internal sealed record JsIfStatement(
    SliceSourceSpan Span, JsExpression Test, JsStatement Consequent, JsStatement? Alternate)
    : JsStatement(Span);

/// <summary><c>while</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=28E41E
// Broiler-Human:        PENDING
internal sealed record JsWhileStatement(SliceSourceSpan Span, JsExpression Test, JsStatement Body)
    : JsStatement(Span);

/// <summary><c>do … while</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6E5715
// Broiler-Human:        PENDING
internal sealed record JsDoWhileStatement(SliceSourceSpan Span, JsStatement Body, JsExpression Test)
    : JsStatement(Span);

/// <summary><c>for (init; test; update) body</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F23D8B
// Broiler-Human:        PENDING
internal sealed record JsForStatement(
    SliceSourceSpan Span,
    JsStatement? Initialiser,
    JsExpression? Test,
    JsExpression? Update,
    JsStatement Body) : JsStatement(Span);

/// <summary><c>for (left in right) body</c>.</summary>
/// <param name="Declaration">The declaration kind, when the head declares its binding.</param>
/// <param name="Name">The bound name, when the head names one directly.</param>
/// <param name="Target">The assignment target, when the head is an expression.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0520FF
// Broiler-Human:        PENDING
internal sealed record JsForInStatement(
    SliceSourceSpan Span,
    SliceDeclarationKind? Declaration,
    string Name,
    JsExpression? Target,
    JsExpression Right,
    JsStatement Body) : JsStatement(Span);

/// <summary><c>break</c>, with an optional label.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=095D71
// Broiler-Human:        PENDING
internal sealed record JsBreakStatement(SliceSourceSpan Span, string Label) : JsStatement(Span);

/// <summary><c>continue</c>, with an optional label.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EC24C9
// Broiler-Human:        PENDING
internal sealed record JsContinueStatement(SliceSourceSpan Span, string Label) : JsStatement(Span);

/// <summary><c>return</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=604079
// Broiler-Human:        PENDING
internal sealed record JsReturnStatement(SliceSourceSpan Span, JsExpression? Value)
    : JsStatement(Span);

/// <summary><c>throw</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A05C96
// Broiler-Human:        PENDING
internal sealed record JsThrowStatement(SliceSourceSpan Span, JsExpression Value)
    : JsStatement(Span);

/// <summary><c>try</c>, with a <c>catch</c> clause, a <c>finally</c> block, or both.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5332A9
// Broiler-Human:        PENDING
internal sealed record JsTryStatement(
    SliceSourceSpan Span,
    JsBlockStatement Block,
    string CatchParameter,
    JsBlockStatement? Handler,
    JsBlockStatement? Finaliser) : JsStatement(Span);

/// <summary>One clause of a <c>switch</c>. A <see langword="null"/> test is the default clause.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3B7BAC
// Broiler-Human:        PENDING
internal sealed record JsSwitchClause(
    SliceSourceSpan Span,
    JsExpression? Test,
    System.Collections.Generic.IReadOnlyList<JsStatement> Body) : JsNode(Span);

/// <summary><c>switch</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6D83AE
// Broiler-Human:        PENDING
internal sealed record JsSwitchStatement(
    SliceSourceSpan Span,
    JsExpression Discriminant,
    System.Collections.Generic.IReadOnlyList<JsSwitchClause> Clauses) : JsStatement(Span);

/// <summary>A labelled statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B51425
// Broiler-Human:        PENDING
internal sealed record JsLabelledStatement(SliceSourceSpan Span, string Label, JsStatement Body)
    : JsStatement(Span);

/// <summary>A lone <c>;</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A5D726
// Broiler-Human:        PENDING
internal sealed record JsEmptyStatement(SliceSourceSpan Span) : JsStatement(Span);

/// <summary><c>debugger</c>, which this profile runs as a no-op.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=ED25E1
// Broiler-Human:        PENDING
internal sealed record JsDebuggerStatement(SliceSourceSpan Span) : JsStatement(Span);

/// <summary>A function declaration.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A5FCF2
// Broiler-Human:        PENDING
internal sealed record JsFunctionDeclaration(SliceSourceSpan Span, JsFunctionNode Function)
    : JsStatement(Span);

/// <summary>A whole program: a directive prologue and a statement list.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5F7C89
// Broiler-Human:        PENDING
internal sealed record JsProgramNode(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<JsStringLiteral> Directives,
    System.Collections.Generic.IReadOnlyList<JsStatement> Body,
    bool IsStrict) : JsNode(Span);
