// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           1
// Human-reviewed:   0/9
// IP risk:          None
// Security risk:    High
// Criteria:         5/3
// Resource impact:  2/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>One source text the front end must compile, and the value it must run to.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3C461E
// Broiler-Human:        PENDING
public sealed record SliceAcceptedSource(string Name, string Source, string Completion);

/// <summary>One source text the front end must refuse, and the code it must refuse it with.</summary>
/// <remarks>
/// The options are a property with a default rather than a fourth positional field, because all
/// but one of these is an ordinary script source and spelling that out on every one of them would
/// bury the one that is not.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=044F40
// Broiler-Human:        PENDING
public sealed record SliceRefusedSource(string Name, string Source, SliceSourceDiagnosticCode Code)
{
    /// <summary>The options this source is refused under. A script parse unless stated.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=10049D
    // Broiler-Human:        PENDING
    public SliceParseOptions Options { get; init; } = SliceParseOptions.Script;
}

/// <summary>
/// The source corpus: what the front end must accept, and what it must refuse.
/// </summary>
/// <remarks>
/// <para>
/// <b>These are the programs JS-1's hand-written lowering could not be written from.</b> That
/// lowering builds bytecode directly, so every claim it makes is a claim about the executor; a
/// program here is written in JavaScript and therefore makes a claim about the tokenizer, the
/// parser, the validation stage and the lowering as well. <c>10 - 3 - 2</c> is 5 and not 9, and no
/// hand-built artifact can be wrong about that because no hand-built artifact has an
/// associativity.
/// </para>
/// <para>
/// <b>Every accepted program is chosen because a plausible front end gets it wrong.</b> One that
/// lowered unary plus away answers <c>+true</c> with <c>true</c>; one that lowered <c>||</c> to a
/// comparison answers <c>0 || 5</c> with <c>true</c>; one that evaluated both operands of
/// <c>&amp;&amp;</c> answers the short-circuit case with 1; one whose <c>continue</c> jumps to the
/// loop top rather than to the update runs a <c>for</c> loop forever. A corpus of additions would
/// find none of them.
/// </para>
/// <para>
/// <b>Every refused program names one clause of the manifest or one early error</b>, and the
/// refusals are as much of the surface as the acceptances: a front end that accepted
/// <c>const x = 1; x = 2;</c> would be a front end whose <c>const</c> means nothing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A4F715
// Broiler-Human:        PENDING
public static class SliceSourcePrograms
{
    /// <summary>Every source the front end must compile, with the value the program runs to.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=11701F
    // Broiler-Falsified-If: any program here runs to a value other than the one recorded beside it
    // Broiler-Human:        PENDING
    public static SliceAcceptedSource[] Accepted =>
    [
        // ---- the shape of the grammar --------------------------------------------------------
        new("source-multiplication-binds-tighter", "1 + 2 * 3", "7"),
        new("source-parentheses-regroup", "(1 + 2) * 3", "9"),
        new("source-subtraction-is-left-associative", "10 - 3 - 2", "5"),
        new("source-assignment-is-right-associative", "var a = 0; var b = 0; a = b = 7; a + b", "14"),
        new("source-unary-minus-binds-tighter-than-a-shift", "-1 >>> 0", "4294967295"),

        // ---- the language, as opposed to arithmetic -------------------------------------------
        new("source-unary-plus-is-to-number", "+true", "1"),
        new("source-remainder-takes-the-dividends-sign", "-5 % 3", "-2"),
        new("source-a-hexadecimal-literal", "0xff", "255"),
        new("source-a-comparison-produces-a-boolean", "1 < 2", "true"),
        new("source-strict-equality-compares-kinds", "1 === true", "false"),

        // ---- short circuiting, which is a branch and not an opcode ----------------------------
        new("source-logical-or-answers-its-operand", "0 || 5", "5"),
        new("source-logical-and-answers-its-operand", "1 && 0", "0"),
        new(
            "source-logical-and-does-not-evaluate-the-right-side",
            "var n = 0; false && (n = 1); n",
            "0"),
        new("source-a-conditional-expression", "1 < 2 ? 10 : 20", "10"),

        // ---- binding, scope and hoisting ------------------------------------------------------
        new("source-a-var-is-visible-before-its-declaration", "var first = x; var x = 5; first", "undefined"),
        new("source-a-block-scopes-a-let", "let x = 1; { let x = 2; } x", "1"),
        new("source-a-declaration-without-an-initialiser", "var x; x", "undefined"),

        // ---- structured control flow ----------------------------------------------------------
        new(
            "source-a-counting-for-loop",
            "var total = 0; for (var i = 1; i <= 10; i = i + 1) { total = total + i; } total",
            "55"),
        new(
            "source-break-leaves-the-loop",
            "var i = 0; while (true) { i = i + 1; if (i === 3) { break; } } i",
            "3"),
        new(
            "source-continue-reaches-the-update",
            "var total = 0; for (var i = 1; i <= 5; i = i + 1) { if (i === 3) { continue; } " +
            "total = total + i; } total",
            "12"),
        new("source-a-do-while-runs-once", "var n = 0; do { n = n + 1; } while (false); n", "1"),
        new("source-an-untaken-else", "var n = 0; if (1 > 2) { n = 1; } else { n = 2; } n", "2"),

        // ---- what the tokenizer has to get right ----------------------------------------------
        new("source-a-semicolon-is-inserted", "var a = 1\nvar b = 2\na + b", "3"),
        new("source-a-comment-is-not-code", "1 + /* two */ 2 // and a line comment\n", "3"),
        new("source-a-directive-prologue-is-not-the-completion-value", "\"use strict\"; 1 + 1", "2"),
    ];

    /// <summary>Every source the front end must refuse, with the code it must refuse it with.</summary>
    /// <remarks>
    /// <b>These have no artifact and therefore no corpus entry.</b> A refused source produces
    /// nothing for a replay to read, which is exactly the boundary answer: an early error never
    /// becomes bytes. They are judged by the composition that carries the front end, and the
    /// execution-only image - which has no front end - could not judge them and does not claim to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=6AE934
    // Broiler-Falsified-If: any source here compiles, or is refused with a code other than the one recorded beside it
    // Broiler-Human:        PENDING
    public static SliceRefusedSource[] Refused =>
    [
        // ---- tokenizing ------------------------------------------------------------------------
        new("refuse-an-unexpected-character", "1 @ 2", SliceSourceDiagnosticCode.UnexpectedCharacter),
        new("refuse-an-unterminated-comment", "1 /* and no close", SliceSourceDiagnosticCode.UnterminatedComment),
        new("refuse-an-exponent-with-no-digits", "1e", SliceSourceDiagnosticCode.MalformedNumericLiteral),
        new(
            "refuse-a-literal-touching-an-identifier",
            "3in",
            SliceSourceDiagnosticCode.MalformedNumericLiteral),
        new(
            "refuse-an-unterminated-string",
            "\"use strict\n",
            SliceSourceDiagnosticCode.UnterminatedStringLiteral),
        // An INCOMPLETE escape, not an unrecognised one. `\q` is `q` in the language, and the
        // first census caught this front end refusing eight of Octane's twenty-four files over
        // exactly that; what is genuinely malformed is a hexadecimal escape with too few digits.
        new(
            "refuse-an-unterminated-regular-expression",
            "var re = /ab\nre",
            SliceSourceDiagnosticCode.UnterminatedRegularExpression),
        new(
            "refuse-an-unterminated-template-literal",
            "var t = `ab",
            SliceSourceDiagnosticCode.UnterminatedTemplateLiteral),
        new(
            "refuse-an-incomplete-hexadecimal-escape",
            "\"a\\x1\"; 1",
            SliceSourceDiagnosticCode.UnknownEscapeSequence),

        // ---- parsing -----------------------------------------------------------------------------
        new("refuse-a-missing-operand", "1 +", SliceSourceDiagnosticCode.UnexpectedToken),
        new("refuse-a-missing-close-paren", "(1 + 2", SliceSourceDiagnosticCode.ExpectedToken),
        new(
            "refuse-two-expressions-on-one-line",
            "var a = 1; a a",
            SliceSourceDiagnosticCode.ExpectedToken),

        // ---- the manifest, refused at verification and not at first execution ------------------
        new("refuse-a-function", "function f() { return 1; }", SliceSourceDiagnosticCode.ConstructOutsideManifest),
        new("refuse-a-string-value", "var s = \"text\"; s", SliceSourceDiagnosticCode.ConstructOutsideManifest),
        new("refuse-an-object-literal", "var o = {}; o", SliceSourceDiagnosticCode.ConstructOutsideManifest),
        new("refuse-loose-equality", "1 == true", SliceSourceDiagnosticCode.ConstructOutsideManifest),
        new("refuse-bitwise-not", "~0", SliceSourceDiagnosticCode.ConstructOutsideManifest),
        new("refuse-typeof", "typeof 1", SliceSourceDiagnosticCode.ConstructOutsideManifest),

        // ---- static semantics --------------------------------------------------------------------
        new(
            "refuse-a-duplicate-lexical-declaration",
            "let x = 1; let x = 2; x",
            SliceSourceDiagnosticCode.DuplicateLexicalDeclaration),
        new(
            "refuse-a-var-and-lexical-collision",
            "var x = 1; let x = 2; x",
            SliceSourceDiagnosticCode.VarAndLexicalCollision),
        new(
            "refuse-a-constant-without-an-initialiser",
            "const x; x",
            SliceSourceDiagnosticCode.ConstWithoutInitialiser),
        new(
            "refuse-an-assignment-to-a-constant",
            "const x = 1; x = 2; x",
            SliceSourceDiagnosticCode.AssignmentToConstant),
        new(
            "refuse-an-invalid-assignment-target",
            "var x = 1; 1 = x; x",
            SliceSourceDiagnosticCode.InvalidAssignmentTarget),
        new(
            "refuse-an-unresolvable-name",
            "undeclared + 1",
            SliceSourceDiagnosticCode.UnresolvableIdentifier),
        new("refuse-a-break-outside-a-loop", "break;", SliceSourceDiagnosticCode.IllegalBreak),
        new("refuse-a-continue-outside-a-loop", "continue;", SliceSourceDiagnosticCode.IllegalContinue),

        // ---- strict mode, which is this stage's ruling and the tokenizer's recognition ---------
        new(
            "refuse-a-legacy-octal-in-strict-code",
            "\"use strict\"; 0123",
            SliceSourceDiagnosticCode.LegacyOctalInStrictCode),
        new(
            "refuse-a-strict-reserved-name-as-a-binding",
            "\"use strict\"; var eval = 1; eval",
            SliceSourceDiagnosticCode.ReservedWordAsBinding),

        // ---- two shapes an earlier draft of this front end got wrong -----------------------------
        //
        // A leading string literal is a directive only when the whole statement is that literal.
        // The first draft took any leading string as one, so `"use strict" + 1` enabled strict
        // mode for a program that never asked and then failed on the `+` with a syntax error - a
        // wrong ANSWER wearing a wrong diagnostic. It is refused for the string, which is what a
        // manifest with no string value owes it.
        new(
            "refuse-a-string-expression-that-looks-like-a-directive",
            "\"use strict\" + 1",
            SliceSourceDiagnosticCode.ConstructOutsideManifest),

        // A `var` in a loop body hoists straight past a `let` in the loop head, so the two are one
        // scope's worth of colliding names. The first draft collected the head's var names only,
        // which made this legal here and an error in every other implementation.
        new(
            "refuse-a-var-in-a-loop-body-colliding-with-a-let-in-its-head",
            "for (let i = 0; i < 1; i = i + 1) { var i; } 0",
            SliceSourceDiagnosticCode.VarAndLexicalCollision),

        // ---- the parse options' own bound -------------------------------------------------------
        new(
            "refuse-nesting-past-the-bound",
            Nested(SliceParseOptions.DefaultMaximumNestingDepth + 4),
            SliceSourceDiagnosticCode.NestingTooDeep),

        // ---- the format's ceilings, reached by a generated source rather than declared defensive -
        //
        // These three are the only sources here nobody wrote by hand, and generating them is the
        // point. Each is REACHABLE - a program really can declare more locals than the frame
        // admits - so recording them as rows no case reaches would be recording something untrue
        // about them. A source of sixty-five thousand declarations is not a reviewable text, but a
        // three-line generator is, which is the trade this makes.
        new(
            "refuse-more-locals-than-the-frame-admits",
            Declarations(65_536),
            SliceSourceDiagnosticCode.TooManyLocals),
        new(
            "refuse-more-constants-than-the-pool-admits",
            DistinctConstants(65_536),
            SliceSourceDiagnosticCode.TooManyConstants),

        // WHAT IS NOT HERE, and why it is a finding rather than a gap. The third lowering code,
        // `OperandStackTooDeep`, has no entry, because no source can reach it. Right-nested
        // addition is the only shape in this manifest whose operand stack grows with its nesting -
        // a left-nested chain runs at a height of two however long it is, and parentheses add no
        // instruction at all - so reaching the format's ceiling of 1,024 operands takes more than
        // a thousand levels of nesting, and the parse depth bound refuses at a measured maximum of
        // 512 counter units, about 170 levels. THE PARSE BOUND DOMINATES THE STACK CEILING. The
        // code stays declared and the registry records it as defensive with that reason, which is
        // a true statement about this build rather than a source nobody can write.
    ];

    /// <summary>
    /// A source nested <paramref name="depth"/> parenthesised levels deep.
    /// </summary>
    /// <remarks>
    /// Built rather than written out, because the depths that matter are the ones on either side
    /// of the parse options' bound and a literal of ten thousand parentheses is not a reviewable
    /// thing. The bound must <b>refuse</b> this rather than survive it: roadmap section 9 makes a
    /// process termination on a nesting case a blocking failure, so the case that proves the bound
    /// works is a case that would otherwise overflow the stack.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=4CD3B4
    // Broiler-Falsified-If: this source terminates the process at any depth instead of being refused
    // Broiler-Human:        PENDING
    public static string Nested(int depth)
    {
        var text = new System.Text.StringBuilder(new string('(', depth));
        text.Append('1');
        text.Append(')', depth);

        return text.ToString();
    }

    /// <summary>A source declaring <paramref name="count"/> distinct variables.</summary>
    /// <remarks>
    /// One <c>var</c> per name rather than one statement with many declarators, because the frame
    /// is sized from the binding count and a reader checking that claim should be able to count
    /// the declarations.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=821DC4
    // Broiler-Human:        PENDING
    public static string Declarations(int count)
    {
        var text = new System.Text.StringBuilder(count * 12);

        for (var at = 0; at < count; at++)
        {
            text.Append("var v").Append(at).Append(";\n");
        }

        return text.Append("0\n").ToString();
    }

    /// <summary>A source mentioning <paramref name="count"/> distinct numeric constants.</summary>
    /// <remarks>
    /// Distinct, because the pool interns: a source repeating one literal a million times fills
    /// one pool entry, and a generator that produced one would be testing the interning rather
    /// than the ceiling.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=558F30
    // Broiler-Falsified-If: two of the literals this produces intern to one pool entry
    // Broiler-Human:        PENDING
    public static string DistinctConstants(int count)
    {
        var text = new System.Text.StringBuilder(count * 8);

        for (var at = 0; at < count; at++)
        {
            text.Append(at).Append(";\n");
        }

        return text.ToString();
    }

    /// <summary>
    /// <c>1+(1+(1+…))</c>, nested <paramref name="depth"/> deep.
    /// </summary>
    /// <remarks>
    /// <b>The one shape whose operand stack grows with its nesting.</b> Left-nested addition
    /// evaluates to a running total two deep however long it is; parentheses alone add no
    /// instruction at all. Only a right-nested operator holds one value per level, which is why
    /// the stack-ceiling case has to be this and not the simpler-looking alternatives.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E76A52
    // Broiler-Falsified-If: the operand stack this needs does not grow with the depth
    // Broiler-Human:        PENDING
    public static string RightNestedAddition(int depth)
    {
        var text = new System.Text.StringBuilder(depth * 4);

        for (var at = 0; at < depth; at++)
        {
            text.Append("1+(");
        }

        text.Append('1');
        text.Append(')', depth);

        return text.ToString();
    }
}
