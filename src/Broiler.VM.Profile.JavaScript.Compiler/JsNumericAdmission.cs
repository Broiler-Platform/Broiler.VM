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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=609E43
    // Broiler-Human:        PENDING
    internal static System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Judge(
        JsProgramNode program)
    {
        var pass = new JsNumericAdmission();

        foreach (var statement in program.Body)
        {
            if (statement is JsFunctionDeclaration declaration &&
                declaration.Function.Name.Length != 0)
            {
                pass.functions.Add(declaration.Function.Name);
            }
        }

        foreach (var statement in program.Body)
        {
            pass.Statement(statement, insideFunction: false);
        }

        return pass.diagnostics;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3B90C8
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, string what)
    {
        if (diagnostics.Count >= 64)
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=4954C3
    // Broiler-Falsified-If: a statement kind this pass does not recognise reaches the lowering
    // Broiler-Human:        PENDING
    private void Statement(JsStatement statement, bool insideFunction)
    {
        switch (statement)
        {
            case JsVariableStatement declaration:
                foreach (var declarator in declaration.Declarators)
                {
                    if (declarator.Pattern is not null)
                    {
                        Refuse(declarator.Span, "a destructuring binding");
                        continue;
                    }

                    // A BINDING WITH NO INITIALISER IS A BINDING OF `undefined`, which is not a
                    // Number. Admitting it would have made "bindings of numbers" untrue for the one
                    // spelling that is easiest to write by accident.
                    if (declarator.Initialiser is null)
                    {
                        Refuse(declarator.Span, "a binding with no initialiser");
                        continue;
                    }

                    Expression(declarator.Initialiser);
                }

                return;

            case JsExpressionStatement expression:
                Expression(expression.Expression);
                return;

            case JsBlockStatement block:
                foreach (var inner in block.Body)
                {
                    Statement(inner, insideFunction);
                }

                return;

            case JsIfStatement branch:
                Expression(branch.Test);
                Statement(branch.Consequent, insideFunction);

                if (branch.Alternate is not null)
                {
                    Statement(branch.Alternate, insideFunction);
                }

                return;

            case JsWhileStatement loop:
                Expression(loop.Test);
                Statement(loop.Body, insideFunction);
                return;

            case JsDoWhileStatement loop:
                Statement(loop.Body, insideFunction);
                Expression(loop.Test);
                return;

            case JsForStatement loop:
                if (loop.Initialiser is not null)
                {
                    Statement(loop.Initialiser, insideFunction);
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

            case JsBreakStatement jump when jump.Label.Length == 0:
                return;

            case JsContinueStatement jump when jump.Label.Length == 0:
                return;

            case JsEmptyStatement:
                return;

            case JsReturnStatement returned:
                if (!insideFunction)
                {
                    Refuse(returned.Span, "a `return` outside a function");
                    return;
                }

                // A `return` WITH NO VALUE RETURNS `undefined`, and the manifest says a function's
                // return is a Number. The two spellings differ by one token and by one kind of
                // value, so the one that is not a Number is named rather than tolerated.
                if (returned.Value is null)
                {
                    Refuse(returned.Span, "a `return` with no value");
                    return;
                }

                Expression(returned.Value);
                return;

            case JsFunctionDeclaration declaration:
                Function(declaration, insideFunction);
                return;

            case JsClassDeclaration classed:
                Refuse(classed.Span, "a class declaration");
                return;

            case JsForInStatement forIn:
                Refuse(forIn.Span, "a `for`-`in` statement");
                return;

            case JsForOfStatement forOf:
                Refuse(forOf.Span, "a `for`-`of` statement");
                return;

            case JsTryStatement guarded:
                Refuse(guarded.Span, "a `try` statement");
                return;

            case JsThrowStatement thrown:
                Refuse(thrown.Span, "a `throw` statement");
                return;

            case JsSwitchStatement switched:
                Refuse(switched.Span, "a `switch` statement");
                return;

            case JsLabelledStatement labelled:
                Refuse(labelled.Span, "a labelled statement");
                return;

            case JsBreakStatement labelledBreak:
                Refuse(labelledBreak.Span, "a labelled `break`");
                return;

            case JsContinueStatement labelledContinue:
                Refuse(labelledContinue.Span, "a labelled `continue`");
                return;

            case JsWithStatement scoped:
                Refuse(scoped.Span, "a `with` statement");
                return;

            case JsDebuggerStatement halted:
                Refuse(halted.Span, "a `debugger` statement");
                return;

            case JsImportDeclaration imported:
                Refuse(imported.Span, "an `import` declaration");
                return;

            case JsExportDeclaration exported:
                Refuse(exported.Span, "an `export` declaration");
                return;

            default:
                Refuse(statement.Span, "this statement");
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=DBB3D5
    // Broiler-Falsified-If: a function body that can fall off its end is admitted, or a function that closes over an enclosing binding is
    // Broiler-Human:        PENDING
    private void Function(JsFunctionDeclaration declaration, bool insideFunction)
    {
        var function = declaration.Function;

        if (insideFunction)
        {
            Refuse(declaration.Span, "a nested function declaration");
            return;
        }

        if (function.IsGenerator)
        {
            Refuse(declaration.Span, "a generator function");
            return;
        }

        if (function.IsAsync)
        {
            Refuse(declaration.Span, "an async function");
            return;
        }

        foreach (var parameter in function.Parameters)
        {
            if (parameter.IsRest)
            {
                Refuse(parameter.Span, "a rest parameter");
                continue;
            }

            if (parameter.Default is not null)
            {
                Refuse(parameter.Span, "a parameter with a default");
                continue;
            }

            if (parameter.Target is not JsTargetPattern { Target: JsIdentifier })
            {
                Refuse(parameter.Span, "a destructuring parameter");
            }
        }

        foreach (var statement in function.Body)
        {
            Statement(statement, insideFunction: true);
        }

        if (!TerminatesInAReturn(function.Body))
        {
            Refuse(declaration.Span, "a function body that can fall off its end");
        }
    }

    /// <summary>Whether every path through <paramref name="body"/> ends at a <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A05066
    // Broiler-Human:        PENDING
    private static bool TerminatesInAReturn(
        System.Collections.Generic.IReadOnlyList<JsStatement> body) =>
        body.Count != 0 && Terminates(body[^1]);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1834BF
    // Broiler-Human:        PENDING
    private static bool Terminates(JsStatement statement) => statement switch
    {
        JsReturnStatement => true,
        JsBlockStatement block => TerminatesInAReturn(block.Body),
        JsIfStatement { Alternate: not null } branch =>
            Terminates(branch.Consequent) && Terminates(branch.Alternate),
        _ => false,
    };

    /// <summary>Judges one expression.</summary>
    /// <remarks>
    /// <b>The default arm refuses, for the reason the statement walk's does.</b> An expression kind
    /// nobody decided on is not admitted by a manifest that has not decided on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=3DC8F3
    // Broiler-Falsified-If: an expression kind this pass does not recognise reaches the lowering
    // Broiler-Human:        PENDING
    private void Expression(JsExpression expression)
    {
        switch (expression)
        {
            case JsNumberLiteral:
                return;

            case JsIdentifier:
                return;

            case JsUnaryExpression unary:
                if (!AdmitsUnary(unary.Operator))
                {
                    Refuse(unary.Span, "the `" + Spelling(unary.Operator) + "` operator");
                    return;
                }

                Expression(unary.Operand);
                return;

            case JsUpdateExpression update:
                if (update.Operand is not JsIdentifier)
                {
                    Refuse(update.Span, "an update of anything but a binding");
                    return;
                }

                return;

            case JsBinaryExpression binary:
                if (!AdmitsBinary(binary.Operator))
                {
                    Refuse(binary.Span, "the `" + Spelling(binary.Operator) + "` operator");
                    return;
                }

                Expression(binary.Left);
                Expression(binary.Right);
                return;

            case JsAssignmentExpression assignment:
                if (assignment.Target is not JsIdentifier)
                {
                    Refuse(assignment.Span, "an assignment to anything but a binding");
                    return;
                }

                if (assignment.Operator != SliceTokenKind.Equals &&
                    !AdmitsBinary(assignment.Operator))
                {
                    Refuse(
                        assignment.Span,
                        "the `" + Spelling(assignment.Operator) + "=` compound assignment");

                    return;
                }

                Expression(assignment.Value);
                return;

            case JsCallExpression call:
                Call(call);
                return;

            case JsStringLiteral text:
                Refuse(text.Span, "a String literal");
                return;

            case JsBooleanLiteral truth:
                Refuse(truth.Span, "a Boolean literal");
                return;

            case JsNullLiteral nothing:
                Refuse(nothing.Span, "the `null` literal");
                return;

            case JsRegExpLiteral pattern:
                Refuse(pattern.Span, "a regular-expression literal");
                return;

            case JsTemplateLiteral template:
                Refuse(template.Span, "a template literal");
                return;

            case JsTaggedTemplate tagged:
                Refuse(tagged.Span, "a tagged template");
                return;

            case JsArrayLiteral array:
                Refuse(array.Span, "an Array literal");
                return;

            case JsObjectLiteral literal:
                Refuse(literal.Span, "an object literal");
                return;

            case JsMemberExpression member:
                Refuse(member.Span, "a property access");
                return;

            case JsPrivateMemberExpression member:
                Refuse(member.Span, "a private member access");
                return;

            case JsPrivateInExpression member:
                Refuse(member.Span, "a private-name `in` test");
                return;

            case JsSuperMemberExpression member:
                Refuse(member.Span, "a `super` property access");
                return;

            case JsSuperCallExpression call:
                Refuse(call.Span, "a `super` call");
                return;

            case JsChainExpression chain:
                Refuse(chain.Span, "an optional chain");
                return;

            case JsThisExpression self:
                Refuse(self.Span, "`this`");
                return;

            case JsNewTargetExpression target:
                Refuse(target.Span, "`new.target`");
                return;

            case JsNewExpression construction:
                Refuse(construction.Span, "a `new` expression");
                return;

            case JsFunctionExpression function:
                Refuse(
                    function.Span,
                    function.Function.IsArrow ? "an arrow function" : "a function expression");

                return;

            case JsClassExpression classed:
                Refuse(classed.Span, "a class expression");
                return;

            case JsLogicalExpression logical:
                Refuse(logical.Span, "the `" + Spelling(logical.Operator) + "` operator");
                return;

            case JsConditionalExpression conditional:
                Refuse(conditional.Span, "a conditional expression");
                return;

            case JsSequenceExpression sequence:
                Refuse(sequence.Span, "a comma expression");
                return;

            case JsSpreadElement spread:
                Refuse(spread.Span, "a spread element");
                return;

            case JsDestructuringAssignment destructuring:
                Refuse(destructuring.Span, "a destructuring assignment");
                return;

            case JsYieldExpression yielded:
                Refuse(yielded.Span, "a `yield` expression");
                return;

            case JsAwaitExpression awaited:
                Refuse(awaited.Span, "an `await` expression");
                return;

            case JsImportCall imported:
                Refuse(imported.Span, "a dynamic `import()`");
                return;

            case JsImportMeta meta:
                Refuse(meta.Span, "`import.meta`");
                return;

            default:
                Refuse(expression.Span, "this expression");
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=87953E
    // Broiler-Falsified-If: a call whose callee is not a function this program declares is admitted
    // Broiler-Human:        PENDING
    private void Call(JsCallExpression call)
    {
        if (call.Optional)
        {
            Refuse(call.Span, "an optional call");
            return;
        }

        if (call.Callee is not JsIdentifier callee || !functions.Contains(callee.Name))
        {
            Refuse(call.Span, "a call to anything but a function this program declares");
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
