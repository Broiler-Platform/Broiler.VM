// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           55
// Human-reviewed:   0/6
// IP risk:          None
// Security risk:    High
// Criteria:         4/4
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// Every JavaScript construct this front end can recognise, whether or not the manifest admits it.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the census vocabulary, and that is what it is for.</b> A roadmap that decides which
/// feature manifest to build next from an argument is deciding from a guess; one that decides from
/// a count of what real JavaScript actually contains is deciding from a measurement. Every member
/// here is a construct the parser produces a node for, so a walk over a parsed corpus can rank
/// them, and the ranking is the input to JS-4's, JS-5's and JS-6's scope.
/// </para>
/// <para>
/// <b>It is deliberately finer than the grammar.</b> <c>Getter</c> and <c>ObjectLiteral</c> are
/// two entries because a manifest may want one without the other, and <c>Generator</c> is separate
/// from <c>Function</c> for the same reason. A vocabulary as coarse as the grammar would answer
/// "objects are common", which nobody needed a tool to learn.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=823AAB
// Broiler-Human:        PENDING
public enum SliceConstructKind
{
    // ---- values the slice has no representation for -------------------------------------------

    /// <summary>A string used as a value.</summary>
    StringValue,

    /// <summary><c>null</c>.</summary>
    Null,

    /// <summary>A BigInt literal, which is a value kind rather than a number.</summary>
    BigInt,

    /// <summary>An object literal.</summary>
    ObjectLiteral,

    /// <summary>An array literal.</summary>
    ArrayLiteral,

    /// <summary>A regular-expression literal.</summary>
    RegularExpression,

    /// <summary>A template literal.</summary>
    Template,

    /// <summary>A tagged template.</summary>
    TaggedTemplate,

    /// <summary><c>this</c>.</summary>
    This,

    /// <summary><c>super</c>.</summary>
    Super,

    // ---- functions and calls -------------------------------------------------------------------

    /// <summary>A function declaration or expression.</summary>
    Function,

    /// <summary>An arrow function.</summary>
    ArrowFunction,

    /// <summary>A generator function.</summary>
    Generator,

    /// <summary>An <c>async</c> function.</summary>
    AsyncFunction,

    /// <summary>A call.</summary>
    Call,

    /// <summary><c>new</c>.</summary>
    New,

    /// <summary>A parameter with a default.</summary>
    DefaultParameter,

    /// <summary>A rest parameter.</summary>
    RestParameter,

    /// <summary>A spread argument or element.</summary>
    Spread,

    /// <summary>A binding pattern rather than a name.</summary>
    Destructuring,

    // ---- property access -----------------------------------------------------------------------

    /// <summary>A dotted member access.</summary>
    MemberAccess,

    /// <summary>A computed member access.</summary>
    ComputedMemberAccess,

    /// <summary>An optional chain link, <c>?.</c>.</summary>
    OptionalChain,

    /// <summary>A getter in an object or class body.</summary>
    Getter,

    /// <summary>A setter in an object or class body.</summary>
    Setter,

    /// <summary>A computed property name.</summary>
    ComputedProperty,

    /// <summary>A shorthand property.</summary>
    ShorthandProperty,

    /// <summary>A private class member, <c>#name</c>.</summary>
    PrivateName,

    // ---- operators the slice's opcode set does not carry ---------------------------------------

    /// <summary><c>==</c> or <c>!=</c>.</summary>
    LooseEquality,

    /// <summary><c>~</c>.</summary>
    BitwiseNot,

    /// <summary><c>**</c>.</summary>
    Exponentiation,

    /// <summary><c>??</c>.</summary>
    NullishCoalescing,

    /// <summary><c>++</c> or <c>--</c>.</summary>
    Update,

    /// <summary>A compound assignment such as <c>+=</c>.</summary>
    CompoundAssignment,

    /// <summary>The comma operator.</summary>
    Sequence,

    /// <summary><c>typeof</c>.</summary>
    TypeOf,

    /// <summary><c>void</c>.</summary>
    Void,

    /// <summary><c>delete</c>.</summary>
    Delete,

    /// <summary><c>instanceof</c>.</summary>
    Instanceof,

    /// <summary>The <c>in</c> operator.</summary>
    In,

    // ---- statements ----------------------------------------------------------------------------

    /// <summary><c>return</c>.</summary>
    Return,

    /// <summary><c>throw</c>.</summary>
    Throw,

    /// <summary><c>try</c>.</summary>
    Try,

    /// <summary><c>switch</c>.</summary>
    Switch,

    /// <summary>A labelled statement, or a <c>break</c> or <c>continue</c> carrying a label.</summary>
    Label,

    /// <summary><c>for … in</c>.</summary>
    ForIn,

    /// <summary><c>for … of</c>.</summary>
    ForOf,

    /// <summary><c>with</c>.</summary>
    With,

    /// <summary><c>debugger</c>.</summary>
    Debugger,

    /// <summary>A class declaration or expression.</summary>
    Class,

    /// <summary><c>yield</c>.</summary>
    Yield,

    /// <summary><c>await</c>.</summary>
    Await,

    /// <summary>An <c>import</c> declaration.</summary>
    Import,

    /// <summary>An <c>export</c> declaration.</summary>
    Export,

    /// <summary>A reserved word this parser gives no production.</summary>
    ReservedWord,
}

/// <summary>
/// A construct the grammar recognises and this manifest does not admit, with its children.
/// </summary>
/// <remarks>
/// <para>
/// <b>One node type for everything outside the manifest, and precise types for everything inside
/// it.</b> The slice's own constructs each have a record of their own because the lowering is
/// total over them and a lowering that switched on a string would not be; everything else needs
/// to be walked and counted rather than lowered, and forty records nothing consumes would be forty
/// records to keep in step with a grammar for no gain.
/// </para>
/// <para>
/// <b>The children matter.</b> A census that stopped at the outermost construct would report that
/// Octane contains functions, which is not news; walking into them reports what is inside the
/// functions, which is the question. So a construct node carries its parsed children and the walk
/// descends through it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=53E46E
// Broiler-Falsified-If: a construct node drops a child the parser read, so a walk under it counts nothing
// Broiler-Human:        PENDING
public sealed record SliceConstructExpression(
    SliceSourceSpan Span,
    SliceConstructKind Kind,
    System.Collections.Generic.IReadOnlyList<SliceNode> Children) : SliceExpression(Span);

/// <summary>The statement form of <see cref="SliceConstructExpression"/>.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=44C22E
// Broiler-Falsified-If: a construct node drops a child the parser read, so a walk under it counts nothing
// Broiler-Human:        PENDING
public sealed record SliceConstructStatement(
    SliceSourceSpan Span,
    SliceConstructKind Kind,
    System.Collections.Generic.IReadOnlyList<SliceNode> Children) : SliceStatement(Span);

/// <summary>What the declared feature manifest admits, and the one place that knows.</summary>
/// <remarks>
/// <b>The manifest is a validation-stage clause and not a grammar restriction</b>, which is a
/// correction to how JS-3b's first draft worked: the parser refused a <c>function</c> as an
/// unparseable reserved word, which put the manifest boundary in the pass that owns the grammar
/// and left this front end unable to READ the JavaScript whose constructs it needs to count.
/// The grammar is now the language's, and this table is the manifest's.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=9ADE31
// Broiler-Falsified-If: a construct this returns true for has no lowering, or one it returns false for is lowered anyway
// Broiler-Human:        PENDING
public static class SliceManifest
{
    /// <summary>
    /// Whether <c>broiler.javascript.slice</c> admits <paramref name="kind"/>.
    /// </summary>
    /// <remarks>
    /// It admits none of them. That is not a placeholder: the slice is numbers, booleans,
    /// <c>undefined</c>, local bindings, the operators the format has opcodes for, and structured
    /// control flow, and every member of the construct vocabulary is by construction something
    /// outside that. The method exists as the seam a wider manifest grows through, and it is a
    /// method rather than a constant so that the day it stops returning false is a day with a diff.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=18FE9D
    // Broiler-Falsified-If: this admits a kind for which no lowering exists
    // Broiler-Human:        PENDING
    public static bool Admits(SliceConstructKind kind)
    {
        _ = kind;

        return false;
    }

    /// <summary>The name a refusal reports, which is the name the census counts.</summary>
    /// <remarks>
    /// The two must be the same string. A refusal that said "a function" while the census counted
    /// <c>Function</c> would make the two halves of this tool describe one construct in two
    /// vocabularies, and a reader comparing them would be comparing nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=659DF0
    // Broiler-Human:        PENDING
    public static string Describe(SliceConstructKind kind) => kind switch
    {
        SliceConstructKind.StringValue => "a string value",
        SliceConstructKind.ObjectLiteral => "an object literal",
        SliceConstructKind.ArrayLiteral => "an array literal",
        SliceConstructKind.RegularExpression => "a regular-expression literal",
        SliceConstructKind.Template => "a template literal",
        SliceConstructKind.Function => "a function",
        SliceConstructKind.ArrowFunction => "an arrow function",
        SliceConstructKind.Call => "a call",
        SliceConstructKind.MemberAccess => "a property access",
        SliceConstructKind.ComputedMemberAccess => "a computed property access",
        SliceConstructKind.LooseEquality =>
            "loose equality, which is not the strict comparison with a different spelling",
        SliceConstructKind.BitwiseNot => "the bitwise-not operator `~`",
        SliceConstructKind.ReservedWord => "a reserved word this grammar gives no production",
        _ => "the construct " + kind,
    };
}
