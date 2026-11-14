// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           6
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    High
// Criteria:         11/11
// Resource impact:  2/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The admission walk: the policy and limit refusals the wide parser made during the parse, made over
/// the shared tree after a parse that reported nothing, and before the numeric admission and the binder
/// (JSD-0083, decision D18; milestone SJL-13.3, step 1), with the facts the wide parser recorded beside
/// the tree: each admitted BigInt's value and whether the unit awaits at its top level (decision D20, as
/// JSD-0083's notes of 2026-11-13 on X6, D15 and D20 read "the lowering's at step 2").
/// </summary>
/// <remarks>
/// <para>
/// <b>Three refusals, each this family's and none the library's</b> (JSD-0080's "What each engine
/// keeps"): a BigInt literal under a manifest that does not admit BigInt (2104, EX0017), a BigInt literal
/// or key wider than the format's constant ceiling (2106, EX0028), and a module specifier that is not
/// well-formed Unicode (2101, EX0029). Each has the code, the text and the place the wide parser gave it;
/// a BigInt key, which the wide parser read through its property-key rule, is held to the ceiling under
/// every manifest, as it was.
/// </para>
/// <para>
/// <b>It walks the shared tree itself</b>, once per node, in the order the step's adapter visited it
/// (SJL-13.3, step 2): no decorator, not a shorthand's value, no <c>import()</c> specifier as a module
/// specifier and no attribute value. Its refusals are reported in source order, a tie at one offset in
/// visit order, up to the cap a stage reports; a unit with any is refused and stops, as the parse's
/// refusal stopped it. So such a refusal is absent when the parse refuses anywhere in the unit, and no
/// longer precedes a later parse refusal (bundle <c>sjl-13-001</c>'s part B, class S3, and part A's kind
/// (B)).
/// </para>
/// <para>
/// <b>Whether a unit awaits at its top level is the wide parser's rule, by its counter</b>: an
/// <c>await</c> expression, a <c>for await</c> or an <c>await using</c> that no function other than an
/// arrow and no static block encloses. Eval code inside a function starts one function deep, and a
/// field's initialiser raises nothing. It is not the library's <c>ProgramNode.HasTopLevelAwait</c>.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=420490
// Broiler-Falsified-If: a BigInt literal outside an admitting manifest, a BigInt wider than the format's ceiling, or a module specifier that is not well-formed reaches the numeric admission, the binder or the lowering unrefused, a source the wide parser admitted is refused here, or an admitted BigInt's value or the top-level-await answer differs from the one the wide path gave at SJL-13.3 step 2's base
// Broiler-Human:        PENDING
internal sealed class JsAdmissionWalk
{
    /// <summary>Whether the request's manifest admits BigInt literals.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2BF083
    // Broiler-Human:        PENDING
    private readonly bool admitsBigInt;

    /// <summary>The refusals found so far, each with the offset it is ordered by.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7A49F2
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(int Offset, SliceSourceDiagnostic Diagnostic)> found = [];

    /// <summary>Each admitted BigInt literal's value, a key's included, by the literal's identity.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3C08ED
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<NumericLiteral, System.Numerics.BigInteger> values =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>How many functions other than arrows, and static blocks, enclose the node being visited.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=837D17
    // Broiler-Human:        PENDING
    private int functionDepth;

    /// <summary>A walk under a manifest that does or does not admit BigInt literals, from a function depth.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B3D5C5
    // Broiler-Human:        PENDING
    private JsAdmissionWalk(bool admitsBigIntLiterals, int depth)
    {
        admitsBigInt = admitsBigIntLiterals;
        functionDepth = depth;
    }

    /// <summary>How many refusals the walk has made.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=89E3D2
    // Broiler-Human:        PENDING
    internal int Count => found.Count;

    /// <summary>Whether the unit awaits outside every function, by the wide parser's rule.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=ACE465
    // Broiler-Human:        PENDING
    internal bool SawTopLevelAwait { get; private set; }

    /// <summary>Each admitted BigInt literal's value, a key's included, by the literal's identity.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=D3D3AA
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyDictionary<NumericLiteral, System.Numerics.BigInteger> BigIntValues => values;

    /// <summary>
    /// Walks an accepted parse's tree: eval code inside a function starts one function deep.
    /// </summary>
    /// <param name="tree">The library's tree, of a parse that reported nothing.</param>
    /// <param name="options">The unit's options.</param>
    /// <param name="admitsBigIntLiterals">Whether the request's manifest admits BigInt literals.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=2B626E
    // Broiler-Falsified-If: eval code inside a function is walked from depth zero, or any statement of the program is not walked
    // Broiler-Human:        PENDING
    internal static JsAdmissionWalk Walk(ProgramNode tree, SliceParseOptions options, bool admitsBigIntLiterals)
    {
        var walk = new JsAdmissionWalk(
            admitsBigIntLiterals,
            options.IsEval && (options.EvalFlags & Format.JsFormat.EvalRequestFlags.InFunction) != 0 ? 1 : 0);

        walk.Statements(tree.Body);
        return walk;
    }

    /// <summary>The walk's refusals in source order, up to the cap a stage reports.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=99297B
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Refusals()
    {
        var ordered = new System.Collections.Generic.List<(int Offset, int Order, SliceSourceDiagnostic Diagnostic)>(found.Count);

        for (var at = 0; at < found.Count; at++)
        {
            ordered.Add((found[at].Offset, at, found[at].Diagnostic));
        }

        ordered.Sort(static (a, b) => a.Offset != b.Offset ? a.Offset.CompareTo(b.Offset) : a.Order.CompareTo(b.Order));

        var refusals = new System.Collections.Generic.List<SliceSourceDiagnostic>(System.Math.Min(ordered.Count, SliceSourceDiagnostic.MaximumPerStage));

        foreach (var entry in ordered)
        {
            if (refusals.Count >= SliceSourceDiagnostic.MaximumPerStage)
            {
                break;
            }

            refusals.Add(entry.Diagnostic);
        }

        return refusals;
    }

    // ---- the visit, in the step's adapter's order ------------------------------------------------

    /// <summary>A statement list, in order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8A094A
    // Broiler-Human:        PENDING
    private void Statements(System.Collections.Generic.IReadOnlyList<Statement> statements)
    {
        foreach (var statement in statements)
        {
            Statement(statement);
        }
    }

    /// <summary>
    /// One statement, its children in the adapter's order: a <c>for … in</c> head, then its Annex B
    /// initialiser, then the object, then the body; an <c>await using</c> or a <c>for await</c> is a
    /// top-level await outside every function.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=8E313E
    // Broiler-Falsified-If: a statement's child that the walk visited at SJL-13.3 step 2's base is not visited, is visited in another order, or a child it did not visit there is visited
    // Broiler-Human:        PENDING
    private void Statement(Statement statement)
    {
        switch (statement)
        {
            case BlockStatement block:
                Statements(block.Body);
                break;
            case VariableDeclaration declaration:
                Variables(declaration);
                break;
            case ExpressionStatement expression:
                Expression(expression.Expression);
                break;
            case IfStatement branch:
                Expression(branch.Test);
                Statement(branch.Consequent);
                Optional(branch.Alternate);
                break;
            case WithStatement with:
                Expression(with.Object);
                Statement(with.Body);
                break;
            case WhileStatement loop:
                Expression(loop.Test);
                Statement(loop.Body);
                break;
            case DoWhileStatement loop:
                Statement(loop.Body);
                Expression(loop.Test);
                break;
            case ForStatement loop:
                switch (loop.Init)
                {
                    case VariableDeclaration declaration:
                        Variables(declaration);
                        break;
                    case Expression expression:
                        Expression(expression);
                        break;
                }

                Optional(loop.Test);
                Optional(loop.Update);
                Statement(loop.Body);
                break;
            case ForInStatement loop:
                Head(loop.Left);
                Expression(loop.Right);
                Statement(loop.Body);
                break;
            case ForOfStatement loop:
                if (loop.IsAwait)
                {
                    SawTopLevelAwait |= functionDepth == 0;
                }

                Head(loop.Left);
                Expression(loop.Right);
                Statement(loop.Body);
                break;
            case ReturnStatement exit:
                Optional(exit.Argument);
                break;
            case ThrowStatement exit:
                Expression(exit.Argument);
                break;
            case TryStatement attempt:
                Statements(attempt.Block.Body);

                if (attempt.Handler is { } handler)
                {
                    if (handler.Parameter is { } parameter and not BindingIdentifier)
                    {
                        Pattern(parameter);
                    }

                    Statements(handler.Body.Body);
                }

                if (attempt.Finalizer is { } finaliser)
                {
                    Statements(finaliser.Body);
                }

                break;
            case SwitchStatement choice:
                Expression(choice.Discriminant);

                foreach (var clause in choice.Cases)
                {
                    Optional(clause.Test);
                    Statements(clause.Consequent);
                }

                break;
            case LabelledStatement labelled:
                Statement(labelled.Body);
                break;
            case FunctionDeclaration declared:
                Function(declared.Function);
                break;
            case ClassDeclaration declared:
                Class(declared.Class);
                break;
            case ImportDeclaration import:
                ModuleSpecifier(import.Source);
                break;
            case ExportNamedDeclaration export:
                if (export.Declaration is { } exported)
                {
                    Statement(exported);
                }
                else if (export.Source is { } source)
                {
                    ModuleSpecifier(source);
                }

                break;
            case ExportDefaultDeclaration export:
                switch (export.Declaration)
                {
                    case ClassDeclaration declared:
                        Class(declared.Class);
                        break;
                    case FunctionDeclaration declared:
                        Function(declared.Function);
                        break;
                    case Expression value:
                        Expression(value);
                        break;
                }

                break;
            case ExportAllDeclaration export:
                ModuleSpecifier(export.Source);
                break;
        }
    }

    /// <summary>A statement that may be absent.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6C250B
    // Broiler-Human:        PENDING
    private void Optional(Statement? statement)
    {
        if (statement is not null)
        {
            Statement(statement);
        }
    }

    /// <summary>
    /// A variable declaration: an <c>await using</c> outside every function is a top-level await, and each
    /// declarator's pattern, when it is not a name, comes before its initialiser.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DBF3FC
    // Broiler-Human:        PENDING
    private void Variables(VariableDeclaration declaration)
    {
        if (declaration.DeclarationKind == DeclarationKind.AwaitUsing)
        {
            SawTopLevelAwait |= functionDepth == 0;
        }

        foreach (var declarator in declaration.Declarations)
        {
            if (declarator.Target is not BindingIdentifier)
            {
                Pattern(declarator.Target);
            }

            Optional(declarator.Initializer);
        }
    }

    /// <summary>The left side of a <c>for … in</c> or <c>for … of</c> head.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=32A9C3
    // Broiler-Human:        PENDING
    private void Head(SyntaxNode left)
    {
        switch (left)
        {
            case VariableDeclaration declaration:
                Variables(declaration);
                break;
            case ArrayPattern or ObjectPattern:
                Pattern((Pattern)left);
                break;
            case TargetPattern target:
                Expression(target.Target);
                break;
            case Expression expression:
                Expression(expression);
                break;
        }
    }

    /// <summary>
    /// A function other than an arrow, one function deeper: its parameters, then its body.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=69A9D1
    // Broiler-Falsified-If: a function other than an arrow leaves the depth an await inside it is read at unraised, or leaves it raised after it
    // Broiler-Human:        PENDING
    private void Function(FunctionNode function)
    {
        functionDepth++;

        try
        {
            Parameters(function.Parameters);
            Statements(function.Body);
        }
        finally
        {
            functionDepth--;
        }
    }

    /// <summary>An arrow, at the depth it stands at: its parameters, then its body or its expression.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=72322A
    // Broiler-Human:        PENDING
    private void Arrow(FunctionNode function)
    {
        Parameters(function.Parameters);

        if (function.ExpressionBody is { } value)
        {
            Expression(value);
        }
        else
        {
            Statements(function.Body);
        }
    }

    /// <summary>A formal parameter list: a rest's pattern, a default's target before its initialiser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CD8F27
    // Broiler-Human:        PENDING
    private void Parameters(System.Collections.Generic.IReadOnlyList<Pattern> parameters)
    {
        foreach (var parameter in parameters)
        {
            switch (parameter)
            {
                case RestElement rest:
                    Pattern(rest.Argument);
                    break;
                case AssignmentPattern defaulted:
                    Pattern(defaulted.Target);
                    Expression(defaulted.Initializer);
                    break;
                default:
                    Pattern(parameter);
                    break;
            }
        }
    }

    /// <summary>
    /// A binding or assignment pattern: a reference leaf is an expression, an object pattern's key comes
    /// before its value, and a default's target before its initialiser.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A09662
    // Broiler-Falsified-If: a pattern's key, default or reference that the walk visited at SJL-13.3 step 2's base is not visited, or is visited in another order
    // Broiler-Human:        PENDING
    private void Pattern(Pattern pattern)
    {
        switch (pattern)
        {
            case TargetPattern target:
                Expression(target.Target);
                break;
            case AssignmentPattern defaulted:
                Pattern(defaulted.Target);
                Expression(defaulted.Initializer);
                break;
            case RestElement rest:
                Pattern(rest.Argument);
                break;
            case ArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        Pattern(element);
                    }
                }

                break;
            case ObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    switch (property)
                    {
                        case RestElement rest:
                            Pattern(rest.Argument);
                            break;
                        case PatternProperty entry:
                            Key(entry.Key);
                            Pattern(entry.Value);
                            break;
                    }
                }

                break;
        }
    }

    /// <summary>
    /// A class: its heritage, then each member in source order, a member's key before its function; a
    /// method and a static block one function deeper, and a field's initialiser at the class's depth.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0A336A
    // Broiler-Falsified-If: a class's heritage, key or member function that the walk visited at SJL-13.3 step 2's base is not visited or is visited in another order, a decorator is visited, or a field's initialiser is read one function deeper
    // Broiler-Human:        PENDING
    private void Class(ClassNode declared)
    {
        Optional(declared.Heritage);

        foreach (var element in declared.Elements)
        {
            switch (element)
            {
                case MethodDefinition method:
                    Key(method.Key);
                    Function(method.Function);
                    break;
                case FieldDefinition field:
                    Key(field.Key);
                    Optional(field.Value);
                    break;
                case StaticBlock block:
                    functionDepth++;

                    try
                    {
                        Statements(block.Body);
                    }
                    finally
                    {
                        functionDepth--;
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// A property key: a BigInt held to the ceiling alone, under every manifest, and a computed key's
    /// expression; any other key holds nothing the walk reads.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DE00FA
    // Broiler-Falsified-If: a BigInt key is refused as outside the manifest, or is admitted past the format's ceiling
    // Broiler-Human:        PENDING
    private void Key(SyntaxNode key)
    {
        switch (key)
        {
            case NumericLiteral number when (number.Flags & NumericLiteralFlags.BigInt) != 0:
                BigIntKey(number);
                break;
            case ComputedPropertyName name:
                Expression(name.Expression);
                break;
        }
    }

    /// <summary>An expression that may be absent.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B8355C
    // Broiler-Human:        PENDING
    private void Optional(Expression? expression)
    {
        if (expression is not null)
        {
            Expression(expression);
        }
    }

    /// <summary>
    /// One expression, its children in the adapter's order; a parenthesis is the expression it holds, a
    /// shorthand's value is not visited, and an <c>await</c> outside every function is a top-level await.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D296C2
    // Broiler-Falsified-If: an expression's child that the walk visited at SJL-13.3 step 2's base is not visited, is visited in another order, or a child it did not visit there is visited
    // Broiler-Human:        PENDING
    private void Expression(Expression expression)
    {
        switch (expression)
        {
            case ParenthesizedExpression parenthesised:
                Expression(parenthesised.Expression);
                break;
            case NumericLiteral number when (number.Flags & NumericLiteralFlags.BigInt) != 0:
                BigIntLiteral(number);
                break;
            case TemplateLiteral template:
                Expressions(template.Expressions);
                break;
            case TaggedTemplateExpression tagged:
                Expression(tagged.Tag);
                Expressions(tagged.Quasi.Expressions);
                break;
            case ArrayLiteral array:
                foreach (var element in array.Elements)
                {
                    Optional(element);
                }

                break;
            case ObjectLiteral literal:
                foreach (var property in literal.Properties)
                {
                    switch (property)
                    {
                        case SpreadElement spread:
                            Expression(spread.Argument);
                            break;
                        case PropertyDefinition definition:
                            Key(definition.Key);

                            switch (definition.PropertyKind)
                            {
                                case PropertyKind.Shorthand:
                                    break;
                                case PropertyKind.Init:
                                    Expression(definition.Value);
                                    break;
                                default:
                                    if (definition.Value is FunctionExpression method)
                                    {
                                        Function(method.Function);
                                    }

                                    break;
                            }

                            break;
                    }
                }

                break;
            case SpreadElement spread:
                Expression(spread.Argument);
                break;
            case FunctionExpression function:
                Function(function.Function);
                break;
            case ArrowFunctionExpression arrow:
                Arrow(arrow.Function);
                break;
            case ClassExpression declared:
                Class(declared.Class);
                break;
            case MemberExpression member:
                if (member.Object is not SuperExpression)
                {
                    Expression(member.Object);
                }

                if (member.Property is Expression key)
                {
                    Expression(key);
                }

                break;
            case CallExpression call:
                if (call.Callee is not SuperExpression)
                {
                    Expression(call.Callee);
                }

                Expressions(call.Arguments);
                break;
            case ChainExpression chain:
                Expression(chain.Expression);
                break;
            case NewExpression construct:
                Expression(construct.Callee);
                Expressions(construct.Arguments);
                break;
            case ImportCall import:
                Expression(import.Source);
                Optional(import.Options);
                break;
            case UpdateExpression update:
                Expression(update.Operand);
                break;
            case UnaryExpression unary:
                Expression(unary.Operand);
                break;
            case AwaitExpression awaited:
                SawTopLevelAwait |= functionDepth == 0;
                Expression(awaited.Argument);
                break;
            case BinaryExpression binary:
                Expression(binary.Left);
                Expression(binary.Right);
                break;
            case LogicalExpression logical:
                Expression(logical.Left);
                Expression(logical.Right);
                break;
            case PrivateInExpression brand:
                Expression(brand.Right);
                break;
            case ConditionalExpression conditional:
                Expression(conditional.Test);
                Expression(conditional.Consequent);
                Expression(conditional.Alternate);
                break;
            case AssignmentExpression assignment:
                switch (assignment.Target)
                {
                    case ArrayPattern or ObjectPattern:
                        Pattern((Pattern)assignment.Target);
                        break;
                    case TargetPattern target:
                        Expression(target.Target);
                        break;
                    case Expression target:
                        Expression(target);
                        break;
                }

                Expression(assignment.Value);
                break;
            case SequenceExpression sequence:
                Expressions(sequence.Expressions);
                break;
            case YieldExpression yield:
                Optional(yield.Argument);
                break;
        }
    }

    /// <summary>An expression list, in order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=88D971
    // Broiler-Human:        PENDING
    private void Expressions(System.Collections.Generic.IReadOnlyList<Expression> expressions)
    {
        foreach (var expression in expressions)
        {
            Expression(expression);
        }
    }

    // ---- the refusals ------------------------------------------------------------------------------

    /// <summary>
    /// A BigInt literal in an expression: refused under a manifest that does not admit BigInt, and
    /// otherwise held to the format's ceiling; its value recorded when it is admitted.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DC1BCC
    // Broiler-Falsified-If: a BigInt literal is admitted under a manifest that does not admit BigInt, or past the format's ceiling
    // Broiler-Human:        PENDING
    private void BigIntLiteral(NumericLiteral literal)
    {
        if (!admitsBigInt)
        {
            Refuse(
                literal,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a BigInt literal is not admitted by the declared feature manifest");

            return;
        }

        BigIntKey(literal);
    }

    /// <summary>A BigInt literal held to the format's ceiling, as a key is under every manifest; its value recorded when it fits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1E4939
    // Broiler-Falsified-If: a BigInt wider than the format's constant ceiling is admitted, or one within it is refused
    // Broiler-Human:        PENDING
    private void BigIntKey(NumericLiteral literal)
    {
        var status = NumericLiterals.Evaluate(
            literal.Text,
            false,
            new Broiler.ECMAScript.LengthLimit(Format.JsFormat.CeilingBigIntConstantBytes * 8L),
            out var evaluated,
            out _);

        if (status == Broiler.ECMAScript.ResultStatus.Completed && evaluated.Kind == NumericLiteralKind.BigInt)
        {
            values[literal] = evaluated.BigInt;
            return;
        }

        Refuse(
            literal,
            SliceSourceDiagnosticCode.BigIntWiderThanTheFormat,
            "a BigInt literal wider than " +
                (Format.JsFormat.CeilingBigIntConstantBytes * 8).ToString(System.Globalization.CultureInfo.InvariantCulture) +
                " bits is not admitted by the declared feature manifest");
    }

    /// <summary>
    /// A module specifier, refused where it holds a lone surrogate (EX0029, with its code's one text, JSE9309's).
    /// <i>(Revised 2026-11-09, later that day, with JSD-0083's note of 2026-11-09, later that day, on decision D11: it read "with the wide
    /// parser's text", which was the text of an export name's refusal, "an export name holds a lone surrogate, which is
    /// not well-formed Unicode", and is false of a module specifier.)</i>
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=304210
    // Broiler-Falsified-If: a module specifier holding a lone surrogate is admitted, or a well-formed one refused
    // Broiler-Human:        PENDING
    private void ModuleSpecifier(StringLiteral specifier)
    {
        if (!IsWellFormed(specifier.Value))
        {
            Refuse(
                specifier,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a module specifier holds a lone surrogate, which is not well-formed Unicode");
        }
    }

    /// <summary>Whether <paramref name="text"/> pairs every surrogate it holds.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B9C1FD
    // Broiler-Human:        PENDING
    private static bool IsWellFormed(string text)
    {
        for (var at = 0; at < text.Length; at++)
        {
            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]))
            {
                at++;
            }
            else if (char.IsSurrogate(text[at]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Records one refusal at a node's first character.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6FC4F9
    // Broiler-Human:        PENDING
    private void Refuse(SyntaxNode node, SliceSourceDiagnosticCode code, string message) =>
        found.Add((node.Span.Offset, new SliceSourceDiagnostic(code, message, node.Span.Line, node.Span.Column)));
}
