// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   26
// Annotated:        26/26
// Exempt:           3
// Human-reviewed:   0/26
// IP risk:          None
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  0/10 max
// Unverified:       26
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>Where a node begins in the source text, one-based.</summary>
/// <remarks>
/// Every node carries one. A refusal from the validator names a position the reader can find, and
/// the lowering writes the artifact's canonical position table from these, which is what makes the
/// table mean something once there is a source to point back at.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=0E1495
// Broiler-Human:        PENDING
public readonly record struct SliceSourceSpan(int Line, int Column);

/// <summary>The base of every node in the validated tree.</summary>
/// <remarks>
/// <para>
/// <b>The tree is the front-end contract's return, and it is deliberately not the seed's
/// expression-tree type.</b> The seed's compiler plug-in interface returns that type, which means
/// a bytecode back end physically cannot implement it; roadmap section 9 does not copy it. What
/// this returns is a validated tree the lowering consumes, and the lowering is the only consumer.
/// </para>
/// <para>
/// Nodes are records rather than a visitor hierarchy with mutable state. A validated tree is a
/// value: the validator answers questions about it and annotates nothing, and the lowering reads
/// it once. The one thing the validator does record - which binding an identifier resolved to - is
/// carried in a side table keyed by node identity rather than by mutating the node, because a
/// mutable field on a shared node is how a second parse of the same source stops being
/// deterministic.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E2FCC0
// Broiler-Human:        PENDING
public abstract record SliceNode(SliceSourceSpan Span);

/// <summary>The base of every expression.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2ED6BA
// Broiler-Human:        PENDING
public abstract record SliceExpression(SliceSourceSpan Span) : SliceNode(Span);

/// <summary>The base of every statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D3D0F0
// Broiler-Human:        PENDING
public abstract record SliceStatement(SliceSourceSpan Span) : SliceNode(Span);

/// <summary>A numeric literal, carrying whether its shape was a legacy octal.</summary>
/// <remarks>
/// The flag is the tokenizer's recorded fact, travelling to the validator on the tree. Deleting
/// it would put the source text back in the validator's hands, which is the re-scan roadmap
/// section 9 asks to be removed.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F457B3
// Broiler-Human:        PENDING
public sealed record SliceNumericLiteral(SliceSourceSpan Span, double Value, bool IsLegacyOctal)
    : SliceExpression(Span);

/// <summary>A <c>true</c> or <c>false</c> literal.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CDB2A8
// Broiler-Human:        PENDING
public sealed record SliceBooleanLiteral(SliceSourceSpan Span, bool Value) : SliceExpression(Span);

/// <summary>A string literal, carrying its raw text as well as its value.</summary>
/// <remarks>
/// The slice manifest admits no string value, so a string literal is refused everywhere except in
/// a directive prologue. It is a node anyway because the prologue is a list of expression
/// statements over string literals, and a grammar that could not represent one could not have a
/// prologue to rule on.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4DD2D7
// Broiler-Human:        PENDING
public sealed record SliceStringLiteral(SliceSourceSpan Span, string Value, string RawText)
    : SliceExpression(Span);

/// <summary>An <c>IdentifierReference</c>: a name used as a value.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=48947C
// Broiler-Human:        PENDING
public sealed record SliceIdentifierReference(SliceSourceSpan Span, string Name) : SliceExpression(Span);

/// <summary>A unary operator applied to one operand.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6027A6
// Broiler-Human:        PENDING
public sealed record SliceUnaryExpression(SliceSourceSpan Span, SliceTokenKind Operator, SliceExpression Operand)
    : SliceExpression(Span);

/// <summary>A binary operator applied to two operands.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=16AE70
// Broiler-Human:        PENDING
public sealed record SliceBinaryExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, SliceExpression Left, SliceExpression Right)
    : SliceExpression(Span);

/// <summary><c>&amp;&amp;</c> or <c>||</c>, which are not binary operators.</summary>
/// <remarks>
/// They are a separate node because they do not evaluate their right operand, and the lowering
/// for a short-circuit is a branch rather than an opcode. Folding them into
/// <see cref="SliceBinaryExpression"/> is how an implementation ends up evaluating both sides.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FA0108
// Broiler-Falsified-If: the lowering for either operator evaluates the right operand unconditionally
// Broiler-Human:        PENDING
public sealed record SliceLogicalExpression(
    SliceSourceSpan Span, SliceTokenKind Operator, SliceExpression Left, SliceExpression Right)
    : SliceExpression(Span);

/// <summary>A conditional expression, <c>a ? b : c</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A339EC
// Broiler-Human:        PENDING
public sealed record SliceConditionalExpression(
    SliceSourceSpan Span, SliceExpression Test, SliceExpression WhenTrue, SliceExpression WhenFalse)
    : SliceExpression(Span);

/// <summary>A simple assignment, <c>a = b</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A1D837
// Broiler-Human:        PENDING
public sealed record SliceAssignmentExpression(
    SliceSourceSpan Span, SliceExpression Target, SliceExpression Value) : SliceExpression(Span);

/// <summary>How a binding was introduced.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B9C307
// Broiler-Human:        PENDING
public enum SliceDeclarationKind
{
    /// <summary><c>var</c>: a member of the enclosing hoisting scope's <c>VarDeclaredNames</c>.</summary>
    Var = 0,

    /// <summary><c>let</c>: a member of the enclosing scope's <c>LexicallyDeclaredNames</c>.</summary>
    Let = 1,

    /// <summary><c>const</c>: lexical and immutable.</summary>
    Const = 2,
}

/// <summary>One declarator of a variable statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D3D830
// Broiler-Human:        PENDING
public sealed record SliceDeclarator(SliceSourceSpan Span, string Name, SliceExpression? Initialiser)
    : SliceNode(Span);

/// <summary>A <c>var</c>, <c>let</c> or <c>const</c> statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B574D7
// Broiler-Human:        PENDING
public sealed record SliceVariableStatement(
    SliceSourceSpan Span,
    SliceDeclarationKind Kind,
    System.Collections.Generic.IReadOnlyList<SliceDeclarator> Declarators) : SliceStatement(Span);

/// <summary>An expression evaluated for its value, which is the program's completion value.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C2C117
// Broiler-Human:        PENDING
public sealed record SliceExpressionStatement(SliceSourceSpan Span, SliceExpression Expression)
    : SliceStatement(Span);

/// <summary>A braced statement list, which is a lexical scope and not a hoisting scope.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=91533A
// Broiler-Human:        PENDING
public sealed record SliceBlockStatement(
    SliceSourceSpan Span, System.Collections.Generic.IReadOnlyList<SliceStatement> Body)
    : SliceStatement(Span);

/// <summary><c>if</c>, with an optional <c>else</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=ED7AA1
// Broiler-Human:        PENDING
public sealed record SliceIfStatement(
    SliceSourceSpan Span, SliceExpression Test, SliceStatement Consequent, SliceStatement? Alternate)
    : SliceStatement(Span);

/// <summary><c>while</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C6E711
// Broiler-Human:        PENDING
public sealed record SliceWhileStatement(SliceSourceSpan Span, SliceExpression Test, SliceStatement Body)
    : SliceStatement(Span);

/// <summary><c>do … while</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DC5050
// Broiler-Human:        PENDING
public sealed record SliceDoWhileStatement(SliceSourceSpan Span, SliceStatement Body, SliceExpression Test)
    : SliceStatement(Span);

/// <summary><c>for (init; test; update) body</c>, each of the three parts optional.</summary>
/// <remarks>
/// The initialiser is a statement rather than an expression so that <c>for (let i = 0; …)</c> is
/// representable, and its <c>let</c> bindings scope to the loop - which is a scope the block
/// statement cannot express, because the head is not inside the body's braces.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=56146C
// Broiler-Human:        PENDING
public sealed record SliceForStatement(
    SliceSourceSpan Span,
    SliceStatement? Initialiser,
    SliceExpression? Test,
    SliceExpression? Update,
    SliceStatement Body) : SliceStatement(Span);

/// <summary><c>break</c>, without a label: the manifest admits no labelled statement.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1DA15A
// Broiler-Human:        PENDING
public sealed record SliceBreakStatement(SliceSourceSpan Span) : SliceStatement(Span);

/// <summary><c>continue</c>, without a label.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A8878B
// Broiler-Human:        PENDING
public sealed record SliceContinueStatement(SliceSourceSpan Span) : SliceStatement(Span);

/// <summary>A lone <c>;</c>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A3AD83
// Broiler-Human:        PENDING
public sealed record SliceEmptyStatement(SliceSourceSpan Span) : SliceStatement(Span);

/// <summary>
/// The whole program: a directive prologue, then a statement list.
/// </summary>
/// <remarks>
/// <b>The prologue is a separate list and not the first few statements.</b> The parser decides
/// where the prologue ends - it ends at the first statement that is not an expression statement
/// over a string literal - and records the decision here. That keeps the "is this a directive"
/// question in the pass that had the tokens, and leaves the validator asking only "is this
/// directive <c>use strict</c>", which it answers from the recorded raw text.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5C8A13
// Broiler-Human:        PENDING
public sealed record SliceProgram(
    SliceSourceSpan Span,
    System.Collections.Generic.IReadOnlyList<SliceStringLiteral> Directives,
    System.Collections.Generic.IReadOnlyList<SliceStatement> Body) : SliceNode(Span);
