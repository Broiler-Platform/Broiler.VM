// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4D0D1E
    // Broiler-Human:        PENDING
    private static class Walk
    {
        /// <summary>
        /// Whether <paramref name="root"/> makes a closure where it stands - a function or arrow
        /// expression, a class, an object literal's method - or mentions <c>eval</c>, whose source
        /// may make one (JSeal V15-finish).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DED9D9
        // Broiler-Human:        PENDING
        internal static bool MakesClosure(JsNode root)
        {
            var stack = new System.Collections.Generic.Stack<JsNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                switch (node)
                {
                    case JsFunctionExpression:
                    case JsClassExpression:
                    case JsIdentifier { Name: "eval" }:
                        return true;

                    default:
                        foreach (var child in Children(node))
                        {
                            if (child is not null)
                            {
                                stack.Push(child);
                            }
                        }

                        break;
                }
            }

            return false;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6BF03A
        // Broiler-Human:        PENDING
        internal static bool Mentions(JsNode root, string name)
        {
            var stack = new System.Collections.Generic.Stack<JsNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                switch (node)
                {
                    case JsIdentifier identifier:
                        if (string.Equals(identifier.Name, name, System.StringComparison.Ordinal))
                        {
                            return true;
                        }
                        break;

                    // AN ARROW FUNCTION IS NOT A BOUNDARY FOR THIS SEARCH, AND AN ORDINARY FUNCTION IS.
                    // The question this walk answers is whether the enclosing function has to
                    // materialise an `arguments` object, and an arrow has no `arguments` of its own -
                    // a mention inside one reaches the enclosing function's. Stopping at arrows the way
                    // this stopped at every function-like node left `function f() { return () =>
                    // arguments[0]; }` with no `arguments` slot at all, so the inner reference fell
                    // through to a global read and threw a `ReferenceError` at run time
                    // *(corrected: JSC-83)*.
                    case JsFunctionExpression expression:
                        if (expression.Function.IsArrow)
                        {
                            foreach (var statement in expression.Function.Body)
                            {
                                stack.Push(statement);
                            }
                        }
                        break;

                    case JsFunctionDeclaration:
                        break;

                    default:
                        foreach (var child in Children(node))
                        {
                            if (child is not null)
                            {
                                stack.Push(child);
                            }
                        }
                        break;
                }
            }

            return false;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7BE90C
        // Broiler-Human:        PENDING
        private static System.Collections.Generic.IEnumerable<JsNode?> Children(JsNode node)
        {
            switch (node)
            {
                case JsExpressionStatement statement:
                    yield return statement.Expression;
                    break;

                case JsVariableStatement statement:
                    foreach (var declarator in statement.Declarators)
                    {
                        yield return declarator.Pattern;
                        yield return declarator.Initialiser;
                    }

                    break;

                case JsBlockStatement statement:
                    foreach (var inner in statement.Body)
                    {
                        yield return inner;
                    }

                    break;

                case JsIfStatement statement:
                    yield return statement.Test;
                    yield return statement.Consequent;
                    yield return statement.Alternate;
                    break;

                case JsWithStatement statement:
                    yield return statement.Object;
                    yield return statement.Body;
                    break;

                case JsWhileStatement statement:
                    yield return statement.Test;
                    yield return statement.Body;
                    break;

                case JsDoWhileStatement statement:
                    yield return statement.Body;
                    yield return statement.Test;
                    break;

                case JsForStatement statement:
                    yield return statement.Initialiser;
                    yield return statement.Test;
                    yield return statement.Update;
                    yield return statement.Body;
                    break;

                case JsForInStatement statement:
                    yield return statement.Target;
                    yield return statement.Pattern;
                    yield return statement.Initialiser;
                    yield return statement.Right;
                    yield return statement.Body;
                    break;

                case JsForOfStatement statement:
                    yield return statement.Target;
                    yield return statement.Pattern;
                    yield return statement.Right;
                    yield return statement.Body;
                    break;

                case JsReturnStatement statement:
                    yield return statement.Value;
                    break;

                case JsThrowStatement statement:
                    yield return statement.Value;
                    break;

                case JsTryStatement statement:
                    yield return statement.Block;
                    yield return statement.Handler;
                    yield return statement.Finaliser;
                    break;

                case JsSwitchStatement statement:
                    yield return statement.Discriminant;

                    foreach (var clause in statement.Clauses)
                    {
                        yield return clause.Test;

                        foreach (var inner in clause.Body)
                        {
                            yield return inner;
                        }
                    }

                    break;

                case JsLabelledStatement statement:
                    yield return statement.Body;
                    break;

                case JsUnaryExpression expression:
                    yield return expression.Operand;
                    break;

                case JsUpdateExpression expression:
                    yield return expression.Operand;
                    break;

                case JsBinaryExpression expression:
                    yield return expression.Left;
                    yield return expression.Right;
                    break;

                case JsLogicalExpression expression:
                    yield return expression.Left;
                    yield return expression.Right;
                    break;

                case JsAssignmentExpression expression:
                    yield return expression.Target;
                    yield return expression.Value;
                    break;

                case JsConditionalExpression expression:
                    yield return expression.Test;
                    yield return expression.WhenTrue;
                    yield return expression.WhenFalse;
                    break;

                case JsMemberExpression expression:
                    yield return expression.Target;
                    yield return expression.Computed;
                    break;

                case JsCallExpression expression:
                    yield return expression.Callee;

                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsNewExpression expression:
                    yield return expression.Callee;

                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsSequenceExpression expression:
                    foreach (var inner in expression.Expressions)
                    {
                        yield return inner;
                    }

                    break;

                // A `yield` HAS A CHILD, and leaving it out of this walk would have made
                // `function* g() { yield arguments[0]; }` decide it does not use `arguments` - so
                // the frame would not materialise one and the read would fail at run time.
                case JsYieldExpression expression:
                    yield return expression.Operand;
                    break;

                // AND SO DOES AN `await`, for the same reason: without this arm,
                // `async function f(){ await arguments[0]; }` would decide it does not use
                // `arguments`, the frame would materialise none, and the read would fail at run
                // time inside a construct that has nothing to do with `arguments`.
                case JsAwaitExpression expression:
                    yield return expression.Operand;
                    break;

                // AND SO DOES A DYNAMIC IMPORT, in both of its arguments: `import(arguments[0])` is
                // as much a mention as any other, and a walk that stopped at the call would leave
                // the enclosing function with no `arguments` object for it to read.
                case JsImportCall expression:
                    yield return expression.Specifier;

                    if (expression.Options is not null)
                    {
                        yield return expression.Options;
                    }

                    break;

                case JsArrayLiteral expression:
                    foreach (var element in expression.Elements)
                    {
                        yield return element;
                    }

                    break;

                case JsObjectLiteral expression:
                    foreach (var entry in expression.Entries)
                    {
                        yield return entry.Computed;
                        yield return entry.Value;
                    }

                    break;

                // A NODE WITHOUT AN ARM HERE IS OPAQUE TO THIS WALK, WHICH IS NOT A GAP THAT
                // FAILS LOUDLY. The one question the walk answers is whether a function mentions
                // `arguments`, so a template whose substitution is `arguments[0]` and an optional
                // chain whose head is `arguments` would each have left the enclosing function with
                // no `arguments` object at all - and the mention would have fallen through to a
                // global read that throws at run time. `new.target` has no children and needs no
                // arm; every other node this change adds does.
                case JsTemplateLiteral expression:
                    foreach (var substitution in expression.Substitutions)
                    {
                        yield return substitution;
                    }

                    break;

                // A CLASS BODY IS NOT PART OF THE ENCLOSING FUNCTION AND ITS HEAD IS. The heritage
                // and every computed member key are evaluated where the class is written, so an
                // `arguments` in one belongs to the enclosing function; a member's body is a
                // function of its own and has its own. Yielding the members but not their bodies
                // is how the walk gets the second half right.
                case JsClassExpression expression:
                    yield return expression.Class;
                    break;

                case JsClassDeclaration statement:
                    yield return statement.Class;
                    break;

                case JsClassNode definition:
                    yield return definition.Heritage;

                    foreach (var member in definition.Members)
                    {
                        yield return member;
                    }

                    break;

                case JsTaggedTemplate expression:
                    yield return expression.Tag;
                    yield return expression.Quasi;
                    break;

                case JsChainExpression expression:
                    yield return expression.Chain;
                    break;

                case JsClassMember member:
                    yield return member.Computed;
                    break;

                case JsSuperMemberExpression expression:
                    yield return expression.Computed;
                    break;

                // ONLY THE OBJECT IS A CHILD, because a private name is not an expression: it is
                // spelled where a property name is spelled and evaluates nothing. `a[i].#x` still
                // has to be walked, which is what the target is here for.
                case JsPrivateMemberExpression expression:
                    yield return expression.Target;
                    break;

                case JsPrivateInExpression expression:
                    yield return expression.Target;
                    break;

                case JsSuperCallExpression expression:
                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsSpreadElement expression:
                    yield return expression.Argument;
                    break;

                case JsDestructuringAssignment expression:
                    yield return expression.Target;
                    yield return expression.Value;
                    break;

                // The pattern arms exist so a mention inside a DEFAULT is found. Nothing else in a
                // pattern is an expression, and a leaf's identifier is a binding rather than a
                // read - but reporting `function f({arguments}) {}` as a mention only costs an
                // arguments object nobody looks at, where missing one would be wrong.
                case JsTargetPattern pattern:
                    yield return pattern.Target;
                    break;

                case JsPatternElement element:
                    yield return element.Target;
                    yield return element.Default;
                    break;

                case JsArrayPattern pattern:
                    foreach (var element in pattern.Elements)
                    {
                        yield return element;
                    }

                    yield return pattern.Rest;
                    break;

                case JsPatternProperty property:
                    yield return property.Computed;
                    yield return property.Value;
                    break;

                case JsObjectPattern pattern:
                    foreach (var property in pattern.Properties)
                    {
                        yield return property;
                    }

                    yield return pattern.Rest;
                    break;

                default:
                    break;
            }
        }
    }
}
