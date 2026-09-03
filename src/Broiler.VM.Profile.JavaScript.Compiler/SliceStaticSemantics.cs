// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           16
// Human-reviewed:   0/23
// IP risk:          None
// Security risk:    High
// Criteria:         13/13
// Resource impact:  2/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>One binding: its name, how it was declared, and the frame slot it was given.</summary>
/// <remarks>
/// The slot is assigned here rather than by the lowering, because the resolution and the slot
/// allocation answer the same question - which declaration a name means - and splitting them
/// would put the answer in two places that could disagree.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=EEFECE
// Broiler-Human:        PENDING
public sealed record SliceBinding(string Name, SliceDeclarationKind Kind, int Slot);

/// <summary>
/// Identity comparison for tree nodes, which is not what a record's own equality does.
/// </summary>
/// <remarks>
/// <b>This exists because the tree is made of records and records compare by value.</b> Two
/// occurrences of the literal <c>0</c> at the same line and column are one value and two nodes;
/// a resolution table keyed by record equality would merge them, and the merge would be invisible
/// until a program had two identical subexpressions resolving to different bindings. Keying by
/// reference is the only correct answer and it has to be said out loud, because the default is
/// silently wrong here.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=3BEC56
// Broiler-Falsified-If: two distinct nodes that compare equal as records share one entry in a resolution table
// Broiler-Human:        PENDING
internal sealed class SliceNodeIdentityComparer : System.Collections.Generic.IEqualityComparer<SliceNode>
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5960FA
    // Broiler-Human:        PENDING
    internal static SliceNodeIdentityComparer Instance { get; } = new();

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2ABE04
    // Broiler-Human:        PENDING
    public bool Equals(SliceNode? left, SliceNode? right) => ReferenceEquals(left, right);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FCD4E0
    // Broiler-Human:        PENDING
    public int GetHashCode(SliceNode value) =>
        System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);
}

/// <summary>
/// What the validation stage produced: a resolution for every identifier reference, a slot count,
/// and whether the code is strict.
/// </summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=0AE15F
// Broiler-Human:        PENDING
public sealed record SliceBindingTable(
    System.Collections.Generic.IReadOnlyDictionary<SliceNode, SliceBinding> Resolutions,
    int SlotCount,
    bool IsStrict);

/// <summary>
/// <b>Every early error the manifest requires, in one stage over the tree.</b>
/// </summary>
/// <remarks>
/// <para>
/// This is roadmap section 9's consolidation. The seed splits early-error responsibility across
/// four places in two assemblies and re-tokenizes raw source text in two of them; that split is
/// workable when the consumer is a compiler and is not workable when the consumer is a verifier
/// that must answer totally, in one pass, with one diagnostic per rejection. Here there is one
/// pass, one diagnostic list, and no access to the source text at all - this type never sees the
/// string, which is a property of its constructor rather than a discipline.
/// </para>
/// <para>
/// <b>The strict-mode ruling lives here, and that is the named decision roadmap section 9 asks
/// JS-3b to take.</b> The seed's parser deliberately tracks no strict mode and the ruling is
/// split across the four places; this component's answer is that <i>recognition</i> is the
/// tokenizer's and <i>ruling</i> is this stage's. The tokenizer records that a literal had a
/// legacy-octal shape and that a string had this raw text; it never asks whether either is
/// allowed, because that depends on a directive prologue it has not reached and on a goal symbol
/// it is not told. Putting the ruling in the parser would need the parser to know the goal and to
/// have finished the prologue before tokenizing the rest, which is the seed's ambient state
/// wearing a different name.
/// </para>
/// <para>
/// The invariant the binding algorithm enforces is carried in its own words rather than
/// paraphrased: <c>VarDeclaredNames</c> and <c>LexicallyDeclaredNames</c> must not intersect at
/// any single scope.
/// </para>
/// <para>
/// <b>The free-name analysis is exact here and its soundness contract is still stated</b>, because
/// the contract is what makes it reviewable when the manifest grows: over-approximation is safe
/// and under-approximation is a miscompile. It is exact today only because the three constructs
/// that can reach a binding never mentioned at all - a direct <c>eval</c>, a <c>with</c>, and a
/// <c>debugger</c> - are each outside this manifest and refused by name. The moment one is
/// admitted, this analysis must over-approximate or it is wrong.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=47D4B5
// Broiler-Falsified-If: an early error the manifest requires is reported anywhere but this stage, or this stage reads the source text
// Broiler-Human:        PENDING
public sealed class SliceStaticSemantics
{
    /// <summary>The one directive that changes this stage's answers, matched on its raw text.</summary>
    /// <remarks>
    /// Raw text and not value: <c>"use strict"</c> is a directive and a literal spelling the same
    /// characters with an escape is not, though the two have the same value. The tokenizer
    /// recorded the raw text so that this comparison needs no second look at the source.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=F4EF6A
    // Broiler-Falsified-If: a string whose value is `use strict` but whose raw text is not one of these two enables strict code
    // Broiler-Human:        PENDING
    private static readonly string[] UseStrictRawForms = ["\"use strict\"", "'use strict'"];

    /// <summary>Names no binding may take in strict code.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=45F091
    // Broiler-Human:        PENDING
    private static readonly string[] StrictReservedNames =
    [
        "implements", "interface", "let", "package", "private", "protected", "public", "static",
        "yield", "eval", "arguments",
    ];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=47646D
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<SliceNode, SliceBinding> resolutions =
        new(SliceNodeIdentityComparer.Instance);
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=8FF828
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<Scope> scopes = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B380E2
    // Broiler-Human:        PENDING
    private readonly SliceParseOptions options;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A9D12B
    // Broiler-Human:        PENDING
    private int slotCount;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1BEC50
    // Broiler-Human:        PENDING
    private int iterationDepth;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=989AE3
    // Broiler-Human:        PENDING
    private bool strict;

    /// <summary>One scope: what it declares lexically, and which <c>var</c> names appear inside it.</summary>
    /// <remarks>
    /// <b>The two are separate collections because the invariant is about their intersection</b>,
    /// and because a <c>var</c> name is bound in the enclosing hoisting scope while still counting
    /// as declared <i>within</i> every block it is written in. <c>{ let x; var x; }</c> is an
    /// error, and a scope that only knew about its own bindings could not see it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E072AC
    // Broiler-Falsified-If: a `var` name written inside a block is not recorded against that block's own scope
    // Broiler-Human:        PENDING
    private sealed class Scope
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=77A782
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, SliceBinding> Lexical { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>The bindings <c>var</c> declared, present only on a hoisting scope.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=83213D
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, SliceBinding> VarBindings { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>Every <c>var</c> name written anywhere inside this scope, with where.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AB5B3C
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, SliceSourceSpan> VarNamesWithin { get; } =
            new(System.StringComparer.Ordinal);

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=88AE68
        // Broiler-Human:        PENDING
        internal bool IsHoistingScope { get; init; }
    }

    /// <summary>Creates the stage for a parse run under <paramref name="options"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=ADA47B
    // Broiler-Human:        PENDING
    public SliceStaticSemantics(SliceParseOptions options) => this.options = options;

    /// <summary>Every early error found, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=162F14
    // Broiler-Human:        PENDING
    public System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics => diagnostics;

    /// <summary>Validates <paramref name="program"/> and answers its binding table.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=7FE2B1
    // Broiler-Falsified-If: strictness is decided after a name or a literal has been ruled on against it
    // Broiler-Human:        PENDING
    public SliceBindingTable Validate(SliceProgram program)
    {
        System.ArgumentNullException.ThrowIfNull(program);

        // Strictness is decided before any name is looked at, because it changes which names are
        // legal bindings and which literals are legal at all.
        strict = options.GoalIsStrict;

        foreach (var directive in program.Directives)
        {
            if (System.Array.IndexOf(UseStrictRawForms, directive.RawText) >= 0)
            {
                strict = true;
            }
        }

        var top = EnterScope(hoisting: true, program.Body);

        // `var` names hoist to the enclosing hoisting scope and are visible before their
        // declaration; lexical names are not. Both sets exist before any statement is visited,
        // which is what makes a forward reference resolve rather than refuse.
        HoistVarBindings(program.Body);

        foreach (var statement in program.Body)
        {
            DeclareLexicalNames(statement);
        }

        CheckVarLexicalIntersection(top);

        foreach (var statement in program.Body)
        {
            VisitStatement(statement);
        }

        LeaveScope();

        return new SliceBindingTable(resolutions, slotCount, strict);
    }

    /// <summary>
    /// Binds every <c>VarDeclaredName</c> reachable without crossing a hoisting-scope boundary.
    /// </summary>
    /// <remarks>
    /// It descends through blocks and loop bodies because <c>var</c> is not block-scoped, and it
    /// crosses no function boundary because this manifest has none. When functions arrive, the
    /// recursion stops at one and this remark becomes the reason why.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=EADB67
    // Broiler-Falsified-If: a `var` declared inside a block is not visible to a reference outside it
    // Broiler-Human:        PENDING
    private void HoistVarBindings(System.Collections.Generic.IReadOnlyList<SliceStatement> body)
    {
        foreach (var (declarator, _) in VarDeclaratorsWithin(body))
        {
            DeclareVar(declarator);
        }
    }

    /// <summary>Every <c>var</c> declarator written inside <paramref name="body"/>, in source order.</summary>
    /// <remarks>
    /// One walk serving two callers - the hoisting pass and the intersection check - so that a
    /// statement kind added to the grammar cannot be reached by one and missed by the other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=D34BB0
    // Broiler-Falsified-If: a statement kind that can contain a `var` is walked by one caller and not the other
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<(SliceDeclarator Declarator, SliceSourceSpan Span)>
        VarDeclaratorsWithin(System.Collections.Generic.IReadOnlyList<SliceStatement> body)
    {
        var found = new System.Collections.Generic.List<(SliceDeclarator, SliceSourceSpan)>();

        foreach (var statement in body)
        {
            Walk(statement, found);
        }

        return found;

        static void Walk(
            SliceStatement statement,
            System.Collections.Generic.List<(SliceDeclarator, SliceSourceSpan)> into)
        {
            switch (statement)
            {
                case SliceVariableStatement { Kind: SliceDeclarationKind.Var } declaration:
                    foreach (var declarator in declaration.Declarators)
                    {
                        into.Add((declarator, declarator.Span));
                    }

                    break;

                case SliceBlockStatement block:
                    foreach (var inner in block.Body)
                    {
                        Walk(inner, into);
                    }

                    break;

                case SliceIfStatement branch:
                    Walk(branch.Consequent, into);

                    if (branch.Alternate is not null)
                    {
                        Walk(branch.Alternate, into);
                    }

                    break;

                case SliceWhileStatement loop:
                    Walk(loop.Body, into);
                    break;

                case SliceDoWhileStatement loop:
                    Walk(loop.Body, into);
                    break;

                case SliceForStatement loop:
                    if (loop.Initialiser is not null)
                    {
                        Walk(loop.Initialiser, into);
                    }

                    Walk(loop.Body, into);
                    break;

                default:
                    break;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C6B3A0
    // Broiler-Human:        PENDING
    private void DeclareVar(SliceDeclarator declarator)
    {
        var scope = HoistingScope();

        CheckBindingName(declarator);

        // A repeated `var` of one name is one binding and not an error: `var x; var x;` is legal
        // and declares one variable.
        if (scope.VarBindings.TryGetValue(declarator.Name, out var existing))
        {
            resolutions[declarator] = existing;
            return;
        }

        var binding = new SliceBinding(declarator.Name, SliceDeclarationKind.Var, slotCount++);
        scope.VarBindings[declarator.Name] = binding;
        resolutions[declarator] = binding;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=557B23
    // Broiler-Human:        PENDING
    private void DeclareLexicalNames(SliceStatement statement)
    {
        if (statement is not SliceVariableStatement declaration ||
            declaration.Kind == SliceDeclarationKind.Var)
        {
            return;
        }

        foreach (var declarator in declaration.Declarators)
        {
            DeclareLexical(declarator, declaration.Kind);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4497B3
    // Broiler-Falsified-If: a second lexical declaration of one name in one scope allocates a second slot instead of refusing
    // Broiler-Human:        PENDING
    private void DeclareLexical(SliceDeclarator declarator, SliceDeclarationKind kind)
    {
        var scope = scopes[^1];

        CheckBindingName(declarator);

        if (kind == SliceDeclarationKind.Const && declarator.Initialiser is null)
        {
            Refuse(
                SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                $"the constant `{declarator.Name}` is declared without an initialiser, and nothing " +
                "can ever give it one",
                declarator.Span);
        }

        if (scope.Lexical.ContainsKey(declarator.Name))
        {
            Refuse(
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                $"`{declarator.Name}` is already declared in this scope",
                declarator.Span);

            return;
        }

        var binding = new SliceBinding(declarator.Name, kind, slotCount++);
        scope.Lexical[declarator.Name] = binding;
        resolutions[declarator] = binding;
    }

    /// <summary>
    /// <c>VarDeclaredNames</c> and <c>LexicallyDeclaredNames</c> must not intersect at any single
    /// scope.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7AE000
    // Broiler-Falsified-If: a name declared both by `var` and by `let` in one scope is accepted
    // Broiler-Human:        PENDING
    private void CheckVarLexicalIntersection(Scope scope)
    {
        foreach (var (name, span) in scope.VarNamesWithin)
        {
            if (scope.Lexical.ContainsKey(name))
            {
                Refuse(
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    $"`{name}` is in both VarDeclaredNames and LexicallyDeclaredNames of one scope",
                    span);
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DDCA24
    // Broiler-Human:        PENDING
    private void CheckBindingName(SliceDeclarator declarator)
    {
        if (strict && System.Array.IndexOf(StrictReservedNames, declarator.Name) >= 0)
        {
            Refuse(
                SliceSourceDiagnosticCode.ReservedWordAsBinding,
                $"`{declarator.Name}` may not be a binding name in strict code",
                declarator.Span);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=BC22AF
    // Broiler-Falsified-If: a `break` or `continue` inside a loop body is reported as having no enclosing loop, or one outside every loop is not
    // Broiler-Human:        PENDING
    private void VisitStatement(SliceStatement statement)
    {
        switch (statement)
        {
            case SliceVariableStatement declaration:
                foreach (var declarator in declaration.Declarators)
                {
                    if (declarator.Initialiser is not null)
                    {
                        VisitExpression(declarator.Initialiser);
                    }
                }

                break;

            case SliceExpressionStatement expression:
                VisitExpression(expression.Expression);
                break;

            case SliceBlockStatement block:
                {
                    var scope = EnterScope(hoisting: false, block.Body);

                    foreach (var inner in block.Body)
                    {
                        DeclareLexicalNames(inner);
                    }

                    CheckVarLexicalIntersection(scope);

                    foreach (var inner in block.Body)
                    {
                        VisitStatement(inner);
                    }

                    LeaveScope();
                    break;
                }

            case SliceIfStatement branch:
                VisitExpression(branch.Test);
                VisitStatement(branch.Consequent);

                if (branch.Alternate is not null)
                {
                    VisitStatement(branch.Alternate);
                }

                break;

            case SliceWhileStatement loop:
                VisitExpression(loop.Test);
                iterationDepth++;
                VisitStatement(loop.Body);
                iterationDepth--;
                break;

            case SliceDoWhileStatement loop:
                iterationDepth++;
                VisitStatement(loop.Body);
                iterationDepth--;
                VisitExpression(loop.Test);
                break;

            case SliceForStatement loop:
                {
                    // The head is its own scope: `for (let i = 0; …)` binds `i` for the loop and
                    // not for the statement after it.
                    //
                    // Its var-name set covers the BODY as well as the head, because a `var` in the
                    // body hoists straight past a `let` in the head and the two are then one
                    // scope's worth of colliding names. Collecting only the head would have made
                    // `for (let i = 0; ;) { var i; }` legal here and an error everywhere else.
                    var head = loop.Initialiser is null
                        ? EnterScope(hoisting: false, [loop.Body])
                        : EnterScope(hoisting: false, [loop.Initialiser, loop.Body]);

                    if (loop.Initialiser is not null)
                    {
                        DeclareLexicalNames(loop.Initialiser);
                        CheckVarLexicalIntersection(head);
                        VisitStatement(loop.Initialiser);
                    }

                    if (loop.Test is not null)
                    {
                        VisitExpression(loop.Test);
                    }

                    if (loop.Update is not null)
                    {
                        VisitExpression(loop.Update);
                    }

                    iterationDepth++;
                    VisitStatement(loop.Body);
                    iterationDepth--;
                    LeaveScope();
                    break;
                }

            case SliceBreakStatement:
                if (iterationDepth == 0)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.IllegalBreak,
                        "`break` has no enclosing breakable statement",
                        statement.Span);
                }

                break;

            case SliceContinueStatement:
                if (iterationDepth == 0)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.IllegalContinue,
                        "`continue` has no enclosing iteration statement",
                        statement.Span);
                }

                break;

            default:
                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=AED67E
    // Broiler-Falsified-If: a subexpression is not visited, so an early error inside it goes unreported
    // Broiler-Human:        PENDING
    private void VisitExpression(SliceExpression expression)
    {
        switch (expression)
        {
            case SliceNumericLiteral literal:
                if (strict && literal.IsLegacyOctal)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.LegacyOctalInStrictCode,
                        "a legacy octal literal is not admitted in strict code",
                        literal.Span);
                }

                break;

            case SliceIdentifierReference reference:
                Resolve(reference);
                break;

            case SliceUnaryExpression unary:
                VisitExpression(unary.Operand);
                break;

            case SliceBinaryExpression binary:
                VisitExpression(binary.Left);
                VisitExpression(binary.Right);
                break;

            case SliceLogicalExpression logical:
                VisitExpression(logical.Left);
                VisitExpression(logical.Right);
                break;

            case SliceConditionalExpression conditional:
                VisitExpression(conditional.Test);
                VisitExpression(conditional.WhenTrue);
                VisitExpression(conditional.WhenFalse);
                break;

            case SliceAssignmentExpression assignment:
                VisitAssignmentTarget(assignment);
                VisitExpression(assignment.Value);
                break;

            default:
                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=DB4749
    // Broiler-Falsified-If: an assignment to a `const` binding is accepted
    // Broiler-Human:        PENDING
    private void VisitAssignmentTarget(SliceAssignmentExpression assignment)
    {
        if (assignment.Target is not SliceIdentifierReference reference)
        {
            Refuse(
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "the left side of an assignment is not a valid assignment target",
                assignment.Target.Span);

            return;
        }

        var binding = Resolve(reference);

        if (binding is { Kind: SliceDeclarationKind.Const })
        {
            Refuse(
                SliceSourceDiagnosticCode.AssignmentToConstant,
                $"`{reference.Name}` is a constant and cannot be assigned",
                assignment.Span);
        }
    }

    /// <summary>
    /// Resolves an identifier reference to a binding, refusing when there is none.
    /// </summary>
    /// <remarks>
    /// <b>An unresolvable name is an early error in this profile and a runtime error in the
    /// language</b>, and the divergence is deliberate. In the language a free name might be a
    /// property of the global object, so the answer waits for run time; this manifest declares no
    /// global object and no property access at all, so a name that resolves to nothing here can
    /// never resolve, and deferring the answer would move the same refusal to a later stage with
    /// less to say about it. It becomes a conformance exclusion the day the manifest grows a
    /// global, and the decision record carries it as one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=725C01
    // Broiler-Falsified-If: a name with no binding is accepted, or a name shadowed in an inner scope resolves to the outer one
    // Broiler-Human:        PENDING
    private SliceBinding? Resolve(SliceIdentifierReference reference)
    {
        for (var at = scopes.Count - 1; at >= 0; at--)
        {
            if (scopes[at].Lexical.TryGetValue(reference.Name, out var lexical))
            {
                resolutions[reference] = lexical;
                return lexical;
            }

            if (scopes[at].VarBindings.TryGetValue(reference.Name, out var declared))
            {
                resolutions[reference] = declared;
                return declared;
            }
        }

        Refuse(
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            $"`{reference.Name}` resolves to no binding, and this feature manifest declares no " +
            "global object for it to be a property of",
            reference.Span);

        return null;
    }

    /// <summary>Pushes a scope, recording every <c>var</c> name written inside it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=515568
    // Broiler-Human:        PENDING
    private Scope EnterScope(bool hoisting, System.Collections.Generic.IReadOnlyList<SliceStatement> body)
    {
        var scope = new Scope { IsHoistingScope = hoisting };

        foreach (var (declarator, span) in VarDeclaratorsWithin(body))
        {
            scope.VarNamesWithin.TryAdd(declarator.Name, span);
        }

        scopes.Add(scope);

        return scope;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=22616B
    // Broiler-Human:        PENDING
    private void LeaveScope() => scopes.RemoveAt(scopes.Count - 1);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E7735A
    // Broiler-Human:        PENDING
    private Scope HoistingScope()
    {
        for (var at = scopes.Count - 1; at >= 0; at--)
        {
            if (scopes[at].IsHoistingScope)
            {
                return scopes[at];
            }
        }

        return scopes[0];
    }

    /// <summary>Records one early error. Every one is reported, unlike the parser's first-only rule.</summary>
    /// <remarks>
    /// The difference is not an inconsistency. A syntax error leaves the parser guessing about a
    /// program the source never described, so a second diagnostic is invented; a tree that parsed
    /// is a real program and every early error in it is a real fact about it, so reporting one and
    /// hiding the rest would make a caller fix them one at a time.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=FA3241
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceDiagnosticCode code, string message, SliceSourceSpan span) =>
        diagnostics.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
}
