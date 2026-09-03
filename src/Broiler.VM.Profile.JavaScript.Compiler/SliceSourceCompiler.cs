// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           8
// Human-reviewed:   0/22
// IP risk:          None
// Security risk:    High
// Criteria:         15/15
// Resource impact:  2/10 max
// Unverified:       22
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// What compiling one source text produced: an artifact, or the refusals that stopped it.
/// </summary>
/// <remarks>
/// Exactly one side is populated. A result carrying both an artifact and a diagnostic would be a
/// front end that emitted something it had already refused, which is the shape the boundary
/// decision below exists to forbid.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A28A22
// Broiler-Falsified-If: a result carries artifact bytes and a diagnostic at once
// Broiler-Human:        PENDING
public sealed record SliceCompilation(
    byte[]? Artifact,
    System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics,
    bool IsStrict)
{
    /// <summary>Whether an artifact was produced.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=65EEB8
    // Broiler-Human:        PENDING
    public bool Succeeded => Artifact is not null;
}

/// <summary>
/// The front end: source text in, one <c>broiler.javascript.slice</c> artifact out.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is where the verification boundary falls, and the answer is recorded rather than
/// implied.</b> Roadmap section 9 leaves two questions open and this type answers both.
/// </para>
/// <para>
/// <i>Does the verifier re-derive every early error from artifact bytes?</i> <b>No.</b> Source
/// carrying an early error never becomes an artifact: it is refused at the seam, with a code from
/// the registry's <c>embedder-seam</c> half, and no artifact exists for a verifier to read. What
/// the verifier checks is what bytes can be wrong about - framing, limits, opcodes, stack
/// discipline, reachability - and it checks that over every artifact whatever produced it, because
/// an artifact does not have to come from this lowering and a verifier that trusted one would be a
/// verifier for one producer. The two stages check disjoint things, and neither repeats the other.
/// </para>
/// <para>
/// <i>What answer does an artifact that is both malformed in framing and invalid in static
/// semantics get?</i> <b>The framing one, and it is not a phase-order tie-break.</b> Static
/// semantics is a property of a tree and there is no tree - the bytes were never source. The
/// phases are unfused by construction: two assemblies, two input types, runs that need not share a
/// process. A profile that fused them would score a doubly-bad input differently, and a named case
/// fails if these ever fuse.
/// </para>
/// <para>
/// <b>The lowering is deterministic.</b> The same source, lowering version and format version
/// produce a byte-identical artifact: constants are interned in first-mention order, slots are
/// allocated in declaration order, and nothing here reads a clock, a hash seed, or an identity.
/// No consumer requires this today - a host's cache keys on source and versions rather than on
/// output bytes - but retrofitting determinism means auditing every iteration order and
/// identity-derived value in a finished compiler, so it is preserved rather than engineered for.
/// </para>
/// <para>
/// <b>One shape it cannot emit, stated because it is a real program.</b> A loop whose exit is
/// reachable from nothing - <c>while (true) { }</c> with no <c>break</c> - lowers to a tail the
/// verifier refuses as unreachable code. That is the format's answer rather than this lowering's,
/// and it is a conformance exclusion the decision record carries.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=9A3F54
// Broiler-Falsified-If: two compilations of one source under one options value differ by a byte, or an early error reaches the verifier as an artifact
// Broiler-Human:        PENDING
public sealed class SliceSourceCompiler
{
    /// <summary>The entry-point name a compiled program declares.</summary>
    /// <remarks>
    /// The same name JS-1's hand-written lowering uses, deliberately: a host that could invoke a
    /// hand-built corpus entry can invoke a compiled program without learning a second convention,
    /// and the two lowerings stay comparable while both exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A4BCCA
    // Broiler-Human:        PENDING
    public const string MainEntry = "main";

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C9E66E
    // Broiler-Human:        PENDING
    private readonly SliceProgramBuilder builder = new();
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=0CBEC3
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(SliceLabel Break, SliceLabel Continue)> loops = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7C98D9
    // Broiler-Human:        PENDING
    private System.Collections.Generic.IReadOnlyDictionary<SliceNode, SliceBinding> resolutions =
        new System.Collections.Generic.Dictionary<SliceNode, SliceBinding>(
            SliceNodeIdentityComparer.Instance);
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=93A008
    // Broiler-Human:        PENDING
    private int completionSlot;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E83F7B
    // Broiler-Human:        PENDING
    private int height;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=22AB4C
    // Broiler-Human:        PENDING
    private int maximumHeight;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=71B144
    // Broiler-Human:        PENDING
    private int lastPositionOffset = -1;

    /// <summary>Compiles <paramref name="source"/> under <paramref name="options"/>.</summary>
    /// <remarks>
    /// The three stages run in order and the first to refuse stops the rest. A tokenizing failure
    /// is not handed to the parser and a parse failure is not handed to the validator, because a
    /// stage fed a tree the previous stage did not vouch for reports about a program the source
    /// never contained.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=4076ED
    // Broiler-Falsified-If: a stage runs over a tree the previous stage refused
    // Broiler-Human:        PENDING
    public static SliceCompilation Compile(string source, SliceParseOptions options)
    {
        System.ArgumentNullException.ThrowIfNull(source);

        var tokenizer = new SliceTokenizer(source);
        var tokens = tokenizer.Tokenize();

        if (tokenizer.Diagnostics.Count > 0)
        {
            return new SliceCompilation(null, [.. tokenizer.Diagnostics], options.GoalIsStrict);
        }

        var parser = new SliceParser(tokens, options);
        var program = parser.ParseProgram();

        if (parser.Diagnostics.Count > 0)
        {
            return new SliceCompilation(null, [.. parser.Diagnostics], options.GoalIsStrict);
        }

        var semantics = new SliceStaticSemantics(options);
        var bindings = semantics.Validate(program);

        if (semantics.Diagnostics.Count > 0)
        {
            return new SliceCompilation(null, [.. semantics.Diagnostics], bindings.IsStrict);
        }

        return new SliceSourceCompiler().Lower(program, bindings, options);
    }

    /// <summary>Compiles <paramref name="source"/> as a script.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2D62E5
    // Broiler-Human:        PENDING
    public static SliceCompilation Compile(string source) => Compile(source, SliceParseOptions.Script);

    /// <summary>Lowers a validated tree into artifact bytes.</summary>
    /// <remarks>
    /// <b>The completion value lives in a frame slot and never on the operand stack.</b> The
    /// alternative - keeping it on the stack across every statement - makes each statement's
    /// lowering responsible for a value it does not otherwise touch, and makes every jump join a
    /// place where two paths could disagree about a height they never meant to carry. With a slot,
    /// every statement begins and ends at height zero, so the verifier's join check has nothing to
    /// reconcile and a lowering bug shows up as a stack error at the statement that caused it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=80A056
    // Broiler-Falsified-If: the operand stack is not empty at any statement boundary
    // Broiler-Human:        PENDING
    private SliceCompilation Lower(
        SliceProgram program, SliceBindingTable bindings, SliceParseOptions options)
    {
        resolutions = bindings.Resolutions;

        // One slot beyond the bindings, for the completion value.
        var slots = (long)bindings.SlotCount + 1;

        if (slots > JavaScriptFormat.CeilingLocals)
        {
            Refuse(
                SliceSourceDiagnosticCode.TooManyLocals,
                $"the program needs {slots} local slots and the format admits " +
                $"{JavaScriptFormat.CeilingLocals}",
                program.Span);

            return new SliceCompilation(null, diagnostics, bindings.IsStrict);
        }

        for (var slot = 0; slot < slots; slot++)
        {
            _ = builder.DeclareLocal();
        }

        completionSlot = bindings.SlotCount;

        builder.Entry(MainEntry);

        // The completion value of a program with no statement is `undefined`, and the format
        // supplies no implicit one.
        Position(program.Span);
        builder.LoadUndefined();
        Push(1);
        builder.StoreLocal(completionSlot);
        Pop(1);

        foreach (var statement in program.Body)
        {
            LowerStatement(statement, options);
        }

        builder.LoadLocal(completionSlot);
        Push(1);
        builder.Emit(JavaScriptOpcode.Return);
        Pop(1);

        if (diagnostics.Count > 0)
        {
            return new SliceCompilation(null, diagnostics, bindings.IsStrict);
        }

        if (builder.ConstantEntries().Length > JavaScriptFormat.CeilingConstants)
        {
            Refuse(
                SliceSourceDiagnosticCode.TooManyConstants,
                "the program mentions more distinct constants than the format admits",
                program.Span);

            return new SliceCompilation(null, diagnostics, bindings.IsStrict);
        }

        if (maximumHeight > JavaScriptFormat.CeilingOperandStack)
        {
            Refuse(
                SliceSourceDiagnosticCode.OperandStackTooDeep,
                $"the program needs an operand stack of {maximumHeight} and the format admits " +
                $"{JavaScriptFormat.CeilingOperandStack}",
                program.Span);

            return new SliceCompilation(null, diagnostics, bindings.IsStrict);
        }

        var bytes = builder.ToArtifact(
            SliceLowering.SliceManifestId, (uint)System.Math.Max(maximumHeight, 1));

        return new SliceCompilation(bytes, diagnostics, bindings.IsStrict);
    }

    /// <summary>Lowers one statement, beginning and ending at operand-stack height zero.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=A3B928
    // Broiler-Falsified-If: any statement lowering leaves the operand stack at a different height than it entered with
    // Broiler-Human:        PENDING
    private void LowerStatement(SliceStatement statement, SliceParseOptions options)
    {
        switch (statement)
        {
            case SliceEmptyStatement:
                break;

            case SliceVariableStatement declaration:
                foreach (var declarator in declaration.Declarators)
                {
                    Position(declarator.Span);

                    if (declarator.Initialiser is null)
                    {
                        // A declaration with no initialiser still writes its slot: a slot the
                        // executor never wrote and a slot holding `undefined` must not be
                        // distinguishable, and only the latter is something it guarantees.
                        builder.LoadUndefined();
                        Push(1);
                    }
                    else
                    {
                        LowerExpression(declarator.Initialiser);
                    }

                    builder.StoreLocal(SlotOf(declarator, declarator.Name, declarator.Span));
                    Pop(1);
                }

                break;

            case SliceExpressionStatement expression:
                Position(expression.Span);
                LowerExpression(expression.Expression);
                builder.StoreLocal(completionSlot);
                Pop(1);
                break;

            case SliceBlockStatement block:
                foreach (var inner in block.Body)
                {
                    LowerStatement(inner, options);
                }

                break;

            case SliceIfStatement branch:
                LowerIf(branch, options);
                break;

            case SliceWhileStatement loop:
                LowerWhile(loop, options);
                break;

            case SliceDoWhileStatement loop:
                LowerDoWhile(loop, options);
                break;

            case SliceForStatement loop:
                LowerFor(loop, options);
                break;

            case SliceBreakStatement:
                if (loops.Count == 0)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.IllegalBreak,
                        "`break` reached the lowering with no enclosing loop, which the validation " +
                        "stage should have refused",
                        statement.Span);

                    break;
                }

                Position(statement.Span);
                builder.Branch(JavaScriptOpcode.Jump, loops[^1].Break);
                break;

            case SliceContinueStatement:
                if (loops.Count == 0)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.IllegalContinue,
                        "`continue` reached the lowering with no enclosing loop, which the " +
                        "validation stage should have refused",
                        statement.Span);

                    break;
                }

                Position(statement.Span);
                builder.Branch(JavaScriptOpcode.Jump, loops[^1].Continue);
                break;

            default:
                Refuse(
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    $"the statement kind {statement.GetType().Name} has no lowering",
                    statement.Span);

                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=7A389E
    // Broiler-Falsified-If: the two arms of a branch reach the join at different operand-stack heights
    // Broiler-Human:        PENDING
    private void LowerIf(SliceIfStatement branch, SliceParseOptions options)
    {
        Position(branch.Span);
        LowerExpression(branch.Test);

        var otherwise = builder.DefineLabel();
        builder.Branch(JavaScriptOpcode.JumpIfFalse, otherwise);
        Pop(1);

        LowerStatement(branch.Consequent, options);

        if (branch.Alternate is null)
        {
            builder.MarkLabel(otherwise);
            return;
        }

        var done = builder.DefineLabel();
        builder.Branch(JavaScriptOpcode.Jump, done);
        builder.MarkLabel(otherwise);
        LowerStatement(branch.Alternate, options);
        builder.MarkLabel(done);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=4881F6
    // Broiler-Falsified-If: the body is reachable with the test false, or a `break` does not leave the loop
    // Broiler-Human:        PENDING
    private void LowerWhile(SliceWhileStatement loop, SliceParseOptions options)
    {
        var top = builder.DefineLabel();
        var exit = builder.DefineLabel();

        builder.MarkLabel(top);
        Position(loop.Span);
        LowerExpression(loop.Test);
        builder.Branch(JavaScriptOpcode.JumpIfFalse, exit);
        Pop(1);

        loops.Add((exit, top));
        LowerStatement(loop.Body, options);
        loops.RemoveAt(loops.Count - 1);

        builder.Branch(JavaScriptOpcode.Jump, top);
        builder.MarkLabel(exit);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=526468
    // Broiler-Falsified-If: the body runs zero times, or a `continue` reaches the loop top rather than the test
    // Broiler-Human:        PENDING
    private void LowerDoWhile(SliceDoWhileStatement loop, SliceParseOptions options)
    {
        var top = builder.DefineLabel();
        var test = builder.DefineLabel();
        var exit = builder.DefineLabel();

        builder.MarkLabel(top);

        loops.Add((exit, test));
        LowerStatement(loop.Body, options);
        loops.RemoveAt(loops.Count - 1);

        // `continue` in a do-while goes to the test and not to the top, which is the one place a
        // loop's two labels are different offsets rather than the same one.
        builder.MarkLabel(test);
        Position(loop.Test.Span);
        LowerExpression(loop.Test);
        builder.Branch(JavaScriptOpcode.JumpIfTrue, top);
        Pop(1);
        builder.MarkLabel(exit);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=BE7F3A
    // Broiler-Falsified-If: a `continue` skips the update expression, which turns a counting loop into an endless one
    // Broiler-Human:        PENDING
    private void LowerFor(SliceForStatement loop, SliceParseOptions options)
    {
        if (loop.Initialiser is not null)
        {
            LowerStatement(loop.Initialiser, options);
        }

        var top = builder.DefineLabel();
        var next = builder.DefineLabel();
        var exit = builder.DefineLabel();

        builder.MarkLabel(top);

        if (loop.Test is not null)
        {
            Position(loop.Test.Span);
            LowerExpression(loop.Test);
            builder.Branch(JavaScriptOpcode.JumpIfFalse, exit);
            Pop(1);
        }

        loops.Add((exit, next));
        LowerStatement(loop.Body, options);
        loops.RemoveAt(loops.Count - 1);

        builder.MarkLabel(next);

        if (loop.Update is not null)
        {
            Position(loop.Update.Span);
            LowerExpression(loop.Update);
            builder.Emit(JavaScriptOpcode.Pop);
            Pop(1);
        }

        builder.Branch(JavaScriptOpcode.Jump, top);
        builder.MarkLabel(exit);
    }

    /// <summary>Lowers an expression, leaving exactly one value on the stack.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=04A129
    // Broiler-Falsified-If: any expression lowering leaves other than exactly one value on the stack
    // Broiler-Human:        PENDING
    private void LowerExpression(SliceExpression expression)
    {
        switch (expression)
        {
            case SliceNumericLiteral literal:
                builder.LoadNumber(literal.Value);
                Push(1);
                break;

            case SliceBooleanLiteral literal:
                builder.LoadBoolean(literal.Value);
                Push(1);
                break;

            case SliceIdentifierReference reference:
                builder.LoadLocal(SlotOf(reference, reference.Name, reference.Span));
                Push(1);
                break;

            case SliceUnaryExpression unary:
                LowerExpression(unary.Operand);
                builder.Emit(unary.Operator switch
                {
                    SliceTokenKind.Minus => JavaScriptOpcode.Negate,
                    SliceTokenKind.Bang => JavaScriptOpcode.Not,

                    // Unary plus is `ToNumber` and is not a no-op: `+true` is 1. Lowering it away
                    // would be correct only for a manifest whose every value is already a number,
                    // and this one carries Boolean as a distinct kind precisely so it is not.
                    _ => JavaScriptOpcode.ToNumber,
                });

                break;

            case SliceBinaryExpression binary:
                LowerExpression(binary.Left);
                LowerExpression(binary.Right);
                builder.Emit(OpcodeFor(binary.Operator, binary.Span));
                Pop(1);
                break;

            case SliceLogicalExpression logical:
                LowerLogical(logical);
                break;

            case SliceConditionalExpression conditional:
                LowerConditional(conditional);
                break;

            case SliceAssignmentExpression assignment:
                LowerAssignment(assignment);
                break;

            default:
                Refuse(
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    $"the expression kind {expression.GetType().Name} has no lowering",
                    expression.Span);

                builder.LoadUndefined();
                Push(1);
                break;
        }
    }

    /// <summary>
    /// Lowers <c>&amp;&amp;</c> or <c>||</c>, evaluating the right operand only when it is reached.
    /// </summary>
    /// <remarks>
    /// The value of <c>a || b</c> is <c>a</c> when <c>a</c> is truthy, and not <c>true</c>, so the
    /// left value is duplicated for the test and kept when the branch is taken. An implementation
    /// that emitted a comparison here would answer <c>0 || 5</c> with <c>true</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=CBC769
    // Broiler-Falsified-If: the value of either operator is coerced to Boolean, or the right operand is evaluated when the left short-circuits
    // Broiler-Human:        PENDING
    private void LowerLogical(SliceLogicalExpression logical)
    {
        var done = builder.DefineLabel();

        LowerExpression(logical.Left);
        builder.Emit(JavaScriptOpcode.Duplicate);
        Push(1);

        builder.Branch(
            logical.Operator == SliceTokenKind.AmpersandAmpersand
                ? JavaScriptOpcode.JumpIfFalse
                : JavaScriptOpcode.JumpIfTrue,
            done);

        Pop(1);

        // The kept copy is discarded on the path that evaluates the right operand, so both paths
        // reach the label with exactly one value.
        builder.Emit(JavaScriptOpcode.Pop);
        Pop(1);
        LowerExpression(logical.Right);
        builder.MarkLabel(done);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=6BD8B0
    // Broiler-Falsified-If: both arms are evaluated, or the two arms leave different heights at the join
    // Broiler-Human:        PENDING
    private void LowerConditional(SliceConditionalExpression conditional)
    {
        var otherwise = builder.DefineLabel();
        var done = builder.DefineLabel();

        LowerExpression(conditional.Test);
        builder.Branch(JavaScriptOpcode.JumpIfFalse, otherwise);
        Pop(1);

        var atBranch = height;
        LowerExpression(conditional.WhenTrue);
        builder.Branch(JavaScriptOpcode.Jump, done);

        // The second arm is walked at the height the first one started from, because the two are
        // alternatives and not a sequence.
        height = atBranch;
        builder.MarkLabel(otherwise);
        LowerExpression(conditional.WhenFalse);
        builder.MarkLabel(done);
    }

    /// <summary>
    /// Lowers an assignment, whose value is the assigned value.
    /// </summary>
    /// <remarks>
    /// <c>x = 1</c> is an expression worth 1, so the value is duplicated before the store rather
    /// than re-read from the slot afterwards. Re-reading would be one instruction longer and would
    /// also be wrong the day a binding has an observable write barrier.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=CA1550
    // Broiler-Falsified-If: an assignment expression's value is not the value assigned
    // Broiler-Human:        PENDING
    private void LowerAssignment(SliceAssignmentExpression assignment)
    {
        LowerExpression(assignment.Value);
        builder.Emit(JavaScriptOpcode.Duplicate);
        Push(1);

        if (assignment.Target is not SliceIdentifierReference reference)
        {
            Refuse(
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "an assignment target that is not an identifier reached the lowering, which the " +
                "validation stage should have refused",
                assignment.Span);

            builder.Emit(JavaScriptOpcode.Pop);
            Pop(1);
            return;
        }

        builder.StoreLocal(SlotOf(reference, reference.Name, reference.Span));
        Pop(1);
    }

    /// <summary>The opcode for a binary operator, refusing the two the manifest excludes.</summary>
    /// <remarks>
    /// <c>==</c> and <c>!=</c> are refused rather than lowered onto the strict comparisons. Loose
    /// equality is a different operation - <c>0 == false</c> is true and <c>0 === false</c> is
    /// not - and lowering one onto the other would make this profile answer a conformance case
    /// wrongly rather than decline it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=40B5FC
    // Broiler-Falsified-If: a loose equality is lowered onto a strict one
    // Broiler-Human:        PENDING
    private JavaScriptOpcode OpcodeFor(SliceTokenKind op, SliceSourceSpan span)
    {
        switch (op)
        {
            case SliceTokenKind.Plus: return JavaScriptOpcode.Add;
            case SliceTokenKind.Minus: return JavaScriptOpcode.Subtract;
            case SliceTokenKind.Star: return JavaScriptOpcode.Multiply;
            case SliceTokenKind.Slash: return JavaScriptOpcode.Divide;
            case SliceTokenKind.Percent: return JavaScriptOpcode.Rem;
            case SliceTokenKind.LessThan: return JavaScriptOpcode.LessThan;
            case SliceTokenKind.LessThanEquals: return JavaScriptOpcode.LessThanOrEqual;
            case SliceTokenKind.GreaterThan: return JavaScriptOpcode.GreaterThan;
            case SliceTokenKind.GreaterThanEquals: return JavaScriptOpcode.GreaterThanOrEqual;
            case SliceTokenKind.EqualsEqualsEquals: return JavaScriptOpcode.StrictEquals;
            case SliceTokenKind.BangEqualsEquals: return JavaScriptOpcode.StrictNotEquals;
            case SliceTokenKind.Bar: return JavaScriptOpcode.BitwiseOr;
            case SliceTokenKind.Ampersand: return JavaScriptOpcode.BitwiseAnd;
            case SliceTokenKind.Caret: return JavaScriptOpcode.BitwiseXor;
            case SliceTokenKind.LessThanLessThan: return JavaScriptOpcode.ShiftLeft;
            case SliceTokenKind.GreaterThanGreaterThan: return JavaScriptOpcode.ShiftRight;
            case SliceTokenKind.GreaterThanGreaterThanGreaterThan:
                return JavaScriptOpcode.ShiftRightUnsigned;

            default:
                Refuse(
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    op is SliceTokenKind.EqualsEquals or SliceTokenKind.BangEquals
                        ? "loose equality is not admitted by the declared feature manifest, and is " +
                          "not the strict comparison with a different spelling"
                        : "this operator is not admitted by the declared feature manifest",
                    span);

                return JavaScriptOpcode.StrictEquals;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=EA3668
    // Broiler-Human:        PENDING
    private int SlotOf(SliceNode node, string name, SliceSourceSpan span)
    {
        if (resolutions.TryGetValue(node, out var binding))
        {
            return binding.Slot;
        }

        // Unreachable while the validator refuses every unresolvable name, which is what makes it
        // worth stating: this is the assertion that the two stages agree, not a fallback.
        Refuse(
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            $"`{name}` reached the lowering with no resolution, which the validation stage should " +
            "have refused",
            span);

        return completionSlot;
    }

    /// <summary>Records a canonical position row for the next instruction, where one is admissible.</summary>
    /// <remarks>
    /// <para>
    /// <b>The table finally means something.</b> JS-1's builder wrote whatever line and column a
    /// caller stated, because nothing was lowered from source; these rows carry the source position
    /// the instruction came from, so a diagnostic naming a code offset can name a line.
    /// </para>
    /// <para>
    /// Two of the format's rules are enforced here rather than discovered at verification: offsets
    /// are strictly ascending, so a second row at one offset is dropped rather than written, and a
    /// coordinate of zero is the encoding's "not known" and may not be minted, so a span without
    /// one records nothing. Emitting either and letting the verifier refuse would make the
    /// lowering's own output an artifact it cannot itself run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=804A28
    // Broiler-Falsified-If: this writes a row at an offset not greater than the previous row's, or a row with a zero coordinate
    // Broiler-Human:        PENDING
    private void Position(SliceSourceSpan span)
    {
        if (span.Line <= 0 || span.Column <= 0 || builder.Offset <= lastPositionOffset)
        {
            return;
        }

        lastPositionOffset = builder.Offset;
        builder.Position((uint)span.Line, (uint)span.Column);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C21C9D
    // Broiler-Human:        PENDING
    private void Push(int count)
    {
        height += count;

        if (height > maximumHeight)
        {
            maximumHeight = height;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B26D78
    // Broiler-Human:        PENDING
    private void Pop(int count) => height -= count;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=FA3241
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceDiagnosticCode code, string message, SliceSourceSpan span) =>
        diagnostics.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
}
