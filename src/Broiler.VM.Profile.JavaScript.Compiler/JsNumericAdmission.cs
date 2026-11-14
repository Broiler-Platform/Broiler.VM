// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           2
// Human-reviewed:   0/12
// IP risk:          None
// Security risk:    High
// Criteria:         5/5
// Resource impact:  2/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The <c>broiler.javascript.numeric</c> manifest's admission pass: it walks a parsed program and
/// names every construct the manifest does not admit.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT REFUSES BY NAME AND AT COMPILE TIME, WHICH IS WHAT ROADMAP SECTION 6 MAKES A FIRST-CLASS
/// ANSWER.</b> A manifest whose exclusions were discovered at run time would be a manifest an
/// author learns about from a fault; a manifest whose exclusions were discovered at verification
/// would be a manifest an author learns about from bytes. Every construct outside this one is
/// visible in the source, so the source is where it is refused, with the construct's own name in
/// the message. That is the same shape and the same diagnostic code the slice manifest's admission
/// already uses.
/// </para>
/// <para>
/// <b>It is a pass over the tree and not a set of checks scattered through the lowering.</b> The
/// wide lowering is a hundred and twenty mutually recursive methods carrying eight pieces of
/// ambient state, and a manifest expressed as conditions inside it would be a manifest nobody could
/// read off in one place. This class is the manifest, written down: what it does not visit, it
/// admits, and what it names, it refuses.
/// </para>
/// <para>
/// <b>EVERY OCCURRENCE IS REPORTED AND NOT THE FIRST.</b> A tree that parsed is a real program and
/// every fact about it is real, so an author who wrote three refused constructs is told about three
/// rather than being sent round the loop twice. That is the rule the slice surface's admission
/// already applies, for the reason it gives.
/// </para>
/// <para>
/// <b>THE CALL RULE IS THE ONE THAT NEEDS TWO PASSES, and it is the one that makes "direct calls to
/// them" true.</b> The manifest admits a call to a function this program declares and nothing else,
/// so the declared names are collected before any expression is judged; without that, a call
/// written above the declaration it names would be refused for the order it was written in, which
/// the language does not care about.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=27925C
// Broiler-Falsified-If: a construct outside the numeric manifest reaches the lowering unrefused, or a construct the manifest admits is refused
// Broiler-Human:        PENDING
internal sealed class JsNumericAdmission
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];

    /// <summary>The names this program declares as functions, which are the only callable ones.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=654112
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<string> functions =
        new(System.StringComparer.Ordinal);

    /// <summary>Judges one program against the manifest and answers every refusal it earned.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=90E7A6
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Judge(
        ProgramNode program)
    {
        var pass = new JsNumericAdmission();

        foreach (var statement in program.Body)
        {
            if (statement is FunctionDeclaration { Function.Name: { } name } && name.Name.Length != 0)
            {
                pass.functions.Add(name.Name);
            }
        }

        // THE DIRECTIVE PROLOGUE IS NOT A STATEMENT HERE, as the wide parser kept it apart from the body.
        for (var at = JsTreeFacts.PrologueLength(program.Body); at < program.Body.Count; at++)
        {
            pass.Statement(program.Body[at], insideFunction: false);
        }

        return pass.diagnostics;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=5A4AB9
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, string what)
    {
        if (diagnostics.Count >= SliceSourceDiagnostic.MaximumPerStage)
        {
            return;
        }

        diagnostics.Add(new SliceSourceDiagnostic(
            SliceSourceDiagnosticCode.ConstructOutsideManifest,
            what + " is not admitted by the declared feature manifest",
            span.Line,
            span.Column));
    }

    /// <summary>Judges one statement.</summary>
    /// <remarks>
    /// <b>The default arm refuses rather than admits, and that direction is the whole safety of the
    /// pass.</b> A statement kind nobody thought about is a statement kind this manifest has not
    /// decided on, and a walk that admitted the unrecognised would grow the manifest every time the
    /// parser grew a production.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=E77E9B
    // Broiler-Falsified-If: a statement kind this pass does not recognise reaches the lowering
    // Broiler-Human:        PENDING
    private void Statement(Statement statement, bool insideFunction)
    {
        var span = JsTreeFacts.At(statement);

        switch (statement)
        {
            // A RESOURCE DECLARATION IS REFUSED BY NAME: its value is an object with a disposer, and
            // the scope it registers with is nothing this manifest's instructions can express.
            case VariableDeclaration { DeclarationKind: DeclarationKind.Using or DeclarationKind.AwaitUsing }:
                Refuse(span, "a `using` declaration");
                return;

            case VariableDeclaration declaration:
                foreach (var declarator in declaration.Declarations)
                {
                    if (declarator.Target is not BindingIdentifier)
                    {
                        Refuse(JsTreeFacts.At(declarator), "a destructuring binding");
                        continue;
                    }

                    // A BINDING WITH NO INITIALISER IS A BINDING OF `undefined`, which is not a
                    // Number. Admitting it would have made "bindings of numbers" untrue for the one
                    // spelling that is easiest to write by accident.
                    if (declarator.Initializer is null)
                    {
                        Refuse(JsTreeFacts.At(declarator), "a binding with no initialiser");
                        continue;
                    }

                    Expression(declarator.Initializer);
                }

                return;

            case ExpressionStatement expression:
                Expression(expression.Expression);
                return;

            case BlockStatement block:
                foreach (var inner in block.Body)
                {
                    Statement(inner, insideFunction);
                }

                return;

            case IfStatement branch:
                Expression(branch.Test);
                Statement(branch.Consequent, insideFunction);

                if (branch.Alternate is not null)
                {
                    Statement(branch.Alternate, insideFunction);
                }

                return;

            case WhileStatement loop:
                Expression(loop.Test);
                Statement(loop.Body, insideFunction);
                return;

            case DoWhileStatement loop:
                Statement(loop.Body, insideFunction);
                Expression(loop.Test);
                return;

            case ForStatement loop:
                switch (loop.Init)
                {
                    case Statement initialiser:
                        Statement(initialiser, insideFunction);
                        break;

                    // AN EXPRESSION HEAD IS JUDGED AS THE STATEMENT THE WIDE PARSER MADE OF IT.
                    case Expression initialiser:
                        Expression(initialiser);
                        break;
                }

                if (loop.Test is not null)
                {
                    Expression(loop.Test);
                }

                if (loop.Update is not null)
                {
                    Expression(loop.Update);
                }

                Statement(loop.Body, insideFunction);
                return;

            case BreakStatement { Label: null }:
                return;

            case ContinueStatement { Label: null }:
                return;

            case EmptyStatement:
                return;

            case ReturnStatement returned:
                if (!insideFunction)
                {
                    Refuse(span, "a `return` outside a function");
                    return;
                }

                // A `return` WITH NO VALUE RETURNS `undefined`, and the manifest says a function's
                // return is a Number. The two spellings differ by one token and by one kind of
                // value, so the one that is not a Number is named rather than tolerated.
                if (returned.Argument is null)
                {
                    Refuse(span, "a `return` with no value");
                    return;
                }

                Expression(returned.Argument);
                return;

            case FunctionDeclaration declaration:
                Function(declaration, insideFunction);
                return;

            case ClassDeclaration:
                Refuse(span, "a class declaration");
                return;

            case ForInStatement:
                Refuse(span, "a `for`-`in` statement");
                return;

            case ForOfStatement:
                Refuse(span, "a `for`-`of` statement");
                return;

            case TryStatement:
                Refuse(span, "a `try` statement");
                return;

            case ThrowStatement:
                Refuse(span, "a `throw` statement");
                return;

            case SwitchStatement:
                Refuse(span, "a `switch` statement");
                return;

            case LabelledStatement:
                Refuse(span, "a labelled statement");
                return;

            case BreakStatement:
                Refuse(span, "a labelled `break`");
                return;

            case ContinueStatement:
                Refuse(span, "a labelled `continue`");
                return;

            case WithStatement:
                Refuse(span, "a `with` statement");
                return;

            case DebuggerStatement:
                Refuse(span, "a `debugger` statement");
                return;

            case ImportDeclaration:
                Refuse(span, "an `import` declaration");
                return;

            case ExportNamedDeclaration or ExportDefaultDeclaration or ExportAllDeclaration:
                Refuse(span, "an `export` declaration");
                return;

            default:
                Refuse(span, "this statement");
                return;
        }
    }

    /// <summary>Judges one function declaration and its body.</summary>
    /// <remarks>
    /// <para>
    /// <b>A NESTED FUNCTION DECLARATION IS REFUSED, AND THE REASON IS THE CLOSURE.</b> A function
    /// declared inside another can read the enclosing function's bindings, which makes its
    /// environment a chain rather than a frame - and a frame that is a chain is a frame an emitted
    /// unit cannot hold in a slab of doubles. Every function this manifest admits is declared at
    /// the program's top level, so every one of them reads its own slots and the realm's top-level
    /// bindings and nothing in between.
    /// </para>
    /// <para>
    /// <b>THE BODY MUST TERMINATE IN A <c>return</c>, and that is the clause that makes "returns
    /// are numbers" true rather than nearly true.</b> A body that can fall off its end answers
    /// <c>undefined</c> on the path that does, which is not a Number; the test is conservative -
    /// a return terminates, a block terminates when its last statement does, an <c>if</c>
    /// terminates when both of its arms do - so a body that terminates for a reason this pass
    /// cannot see is refused rather than admitted. A conservative refusal is a smaller language;
    /// a conservative admission would be an untrue manifest.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=2378ED
    // Broiler-Falsified-If: a function body that can fall off its end is admitted, or a function that closes over an enclosing binding is
    // Broiler-Human:        PENDING
    private void Function(FunctionDeclaration declaration, bool insideFunction)
    {
        var function = declaration.Function;
        var span = JsTreeFacts.At(declaration);

        if (insideFunction)
        {
            Refuse(span, "a nested function declaration");
            return;
        }

        if (function.IsGenerator)
        {
            Refuse(span, "a generator function");
            return;
        }

        if (function.IsAsync)
        {
            Refuse(span, "an async function");
            return;
        }

        foreach (var parameter in function.Parameters)
        {
            if (parameter is RestElement)
            {
                Refuse(JsTreeFacts.At(parameter), "a rest parameter");
                continue;
            }

            if (parameter is AssignmentPattern)
            {
                Refuse(JsTreeFacts.At(parameter), "a parameter with a default");
                continue;
            }

            if (!JsTreeFacts.IsSimpleName(parameter, out _))
            {
                Refuse(JsTreeFacts.At(parameter), "a destructuring parameter");
            }
        }

        // THE DIRECTIVE PROLOGUE IS NOT A STATEMENT OF THE BODY, as the wide parser kept it apart.
        var body = function.Body;
        var first = JsTreeFacts.PrologueLength(body);

        for (var at = first; at < body.Count; at++)
        {
            Statement(body[at], insideFunction: true);
        }

        if (!TerminatesInAReturn(body, first))
        {
            Refuse(span, "a function body that can fall off its end");
        }
    }

    /// <summary>Whether every path through <paramref name="body"/> ends at a <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=6AE351
    // Broiler-Human:        PENDING
    private static bool TerminatesInAReturn(
        System.Collections.Generic.IReadOnlyList<Statement> body, int first) =>
        body.Count > first && Terminates(body[^1]);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=58D920
    // Broiler-Human:        PENDING
    private static bool Terminates(Statement statement) => statement switch
    {
        ReturnStatement => true,
        BlockStatement block => TerminatesInAReturn(block.Body, 0),
        IfStatement { Alternate: not null } branch =>
            Terminates(branch.Consequent) && Terminates(branch.Alternate),
        _ => false,
    };

    /// <summary>Judges one expression.</summary>
    /// <remarks>
    /// <b>The default arm refuses, for the reason the statement walk's does.</b> An expression kind
    /// nobody decided on is not admitted by a manifest that has not decided on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=742965
    // Broiler-Falsified-If: an expression kind this pass does not recognise reaches the lowering
    // Broiler-Human:        PENDING
    private void Expression(Expression written)
    {
        // A PARENTHESIS IS THE EXPRESSION IT HOLDS, as the wide parser read it, and its place is that
        // expression's.
        var expression = JsTreeFacts.Unwrap(written);
        var span = JsTreeFacts.At(expression);

        switch (expression)
        {
            case NumericLiteral number when (number.Flags & NumericLiteralFlags.BigInt) == 0:
                return;

            case Identifier:
                return;

            case UnaryExpression unary:
            {
                var op = JsTreeFacts.Unary(unary.Operator);

                if (!AdmitsUnary(op))
                {
                    Refuse(span, "the `" + Spelling(op) + "` operator");
                    return;
                }

                Expression(unary.Operand);
                return;
            }

            case UpdateExpression update:
                if (JsTreeFacts.Unwrap(update.Operand) is not Identifier)
                {
                    Refuse(span, "an update of anything but a binding");
                    return;
                }

                return;

            case BinaryExpression binary:
            {
                Expression current = binary;
                while (JsTreeFacts.Unwrap(current) is BinaryExpression b)
                {
                    var op = JsTreeFacts.Binary(b.Operator);

                    if (!AdmitsBinary(op))
                    {
                        Refuse(JsTreeFacts.At(b), "the `" + Spelling(op) + "` operator");
                    }

                    Expression(b.Right);
                    current = b.Left;
                }

                Expression(current);
                return;
            }

            case AssignmentExpression { Target: ArrayPattern or ObjectPattern }:
                Refuse(span, "a destructuring assignment");
                return;

            case AssignmentExpression assignment:
            {
                if (!JsTreeFacts.IsSimpleName(assignment.Target, out _))
                {
                    Refuse(span, "an assignment to anything but a binding");
                    return;
                }

                var op = JsTreeFacts.Compound(assignment.Operator);

                if (op != SliceTokenKind.Equals &&
                    !AdmitsBinary(op))
                {
                    Refuse(
                        span,
                        "the `" + Spelling(op) + "=` compound assignment");

                    return;
                }

                Expression(assignment.Value);
                return;
            }

            case CallExpression { Callee: SuperExpression }:
                Refuse(span, "a `super` call");
                return;

            case CallExpression call:
                Call(call, span);
                return;

            case StringLiteral:
                Refuse(span, "a String literal");
                return;

            case BooleanLiteral:
                Refuse(span, "a Boolean literal");
                return;

            case NullLiteral:
                Refuse(span, "the `null` literal");
                return;

            case RegularExpressionLiteral:
                Refuse(span, "a regular-expression literal");
                return;

            case TemplateLiteral:
                Refuse(span, "a template literal");
                return;

            case TaggedTemplateExpression:
                Refuse(span, "a tagged template");
                return;

            case ArrayLiteral:
                Refuse(span, "an Array literal");
                return;

            case ObjectLiteral:
                Refuse(span, "an object literal");
                return;

            case MemberExpression { Object: SuperExpression }:
                Refuse(span, "a `super` property access");
                return;

            case MemberExpression { Property: PrivateIdentifier }:
                Refuse(span, "a private member access");
                return;

            case MemberExpression:
                Refuse(span, "a property access");
                return;

            case PrivateInExpression:
                Refuse(span, "a private-name `in` test");
                return;

            case ChainExpression:
                Refuse(span, "an optional chain");
                return;

            case ThisExpression:
                Refuse(span, "`this`");
                return;

            case MetaProperty { Meta.Name: "new" }:
                Refuse(span, "`new.target`");
                return;

            case MetaProperty:
                Refuse(span, "`import.meta`");
                return;

            case NewExpression:
                Refuse(span, "a `new` expression");
                return;

            case ArrowFunctionExpression:
                Refuse(span, "an arrow function");
                return;

            case FunctionExpression:
                Refuse(span, "a function expression");
                return;

            case ClassExpression:
                Refuse(span, "a class expression");
                return;

            case LogicalExpression logical:
                Refuse(span, "the `" + Spelling(JsTreeFacts.Logical(logical.Operator)) + "` operator");
                return;

            case ConditionalExpression:
                Refuse(span, "a conditional expression");
                return;

            case SequenceExpression:
                Refuse(span, "a comma expression");
                return;

            case SpreadElement:
                Refuse(span, "a spread element");
                return;

            case YieldExpression:
                Refuse(span, "a `yield` expression");
                return;

            case AwaitExpression:
                Refuse(span, "an `await` expression");
                return;

            case ImportCall:
                Refuse(span, "a dynamic `import()`");
                return;

            default:
                Refuse(span, "this expression");
                return;
        }
    }

    /// <summary>Judges one call.</summary>
    /// <remarks>
    /// <b>A CALLEE THAT IS NOT THE NAME OF A FUNCTION THIS PROGRAM DECLARES IS REFUSED, AND THAT IS
    /// WHAT "DIRECT CALLS TO THEM" MEANS.</b> A call through a property, a call of a parameter, a
    /// call of a global the realm supplies - each of them is a call to something whose value is not
    /// a function this manifest admitted, so each would be a call into the rest of the language
    /// through a name. Refusing them here is what keeps the call graph of an admitted program
    /// closed over the program itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=E63526
    // Broiler-Falsified-If: a call whose callee is not a function this program declares is admitted
    // Broiler-Human:        PENDING
    private void Call(CallExpression call, SliceSourceSpan span)
    {
        if (call.IsOptional)
        {
            Refuse(span, "an optional call");
            return;
        }

        if (JsTreeFacts.Unwrap(call.Callee) is not Identifier callee || !functions.Contains(callee.Name))
        {
            Refuse(span, "a call to anything but a function this program declares");
            return;
        }

        foreach (var argument in call.Arguments)
        {
            Expression(argument);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=9D52C3
    // Broiler-Human:        PENDING
    private static bool AdmitsUnary(SliceTokenKind op) => op switch
    {
        SliceTokenKind.Plus => true,
        SliceTokenKind.Minus => true,
        SliceTokenKind.Bang => true,
        SliceTokenKind.Tilde => true,
        _ => false,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1EAB26
    // Broiler-Human:        PENDING
    private static bool AdmitsBinary(SliceTokenKind op) => op switch
    {
        SliceTokenKind.Plus => true,
        SliceTokenKind.Minus => true,
        SliceTokenKind.Star => true,
        SliceTokenKind.Slash => true,
        SliceTokenKind.Percent => true,
        SliceTokenKind.StarStar => true,
        SliceTokenKind.LessThan => true,
        SliceTokenKind.LessThanEquals => true,
        SliceTokenKind.GreaterThan => true,
        SliceTokenKind.GreaterThanEquals => true,
        SliceTokenKind.EqualsEqualsEquals => true,
        SliceTokenKind.BangEqualsEquals => true,
        SliceTokenKind.EqualsEquals => true,
        SliceTokenKind.BangEquals => true,
        SliceTokenKind.Bar => true,
        SliceTokenKind.Ampersand => true,
        SliceTokenKind.Caret => true,
        SliceTokenKind.LessThanLessThan => true,
        SliceTokenKind.GreaterThanGreaterThan => true,
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => true,
        _ => false,
    };

    /// <summary>How an operator token is written, for a message that names it.</summary>
    /// <remarks>
    /// <b>The map is only over the operators this manifest can refuse</b>, and a token it has no
    /// spelling for answers with the token's own name rather than with an empty string - a message
    /// that named nothing would be worse than one that named a token kind.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=48C7CF
    // Broiler-Human:        PENDING
    private static string Spelling(SliceTokenKind op) => op switch
    {
        SliceTokenKind.Typeof => "typeof",
        SliceTokenKind.Void => "void",
        SliceTokenKind.Delete => "delete",
        SliceTokenKind.Instanceof => "instanceof",
        SliceTokenKind.In => "in",
        SliceTokenKind.AmpersandAmpersand => "&&",
        SliceTokenKind.BarBar => "||",
        SliceTokenKind.QuestionQuestion => "??",
        _ => op.ToString(),
    };
}
