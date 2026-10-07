// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- expressions ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2ED2A1
    // Broiler-Human:        PENDING
    private void CompileExpression(JsExpression expression)
    {
        Position(expression.Span);

        switch (expression)
        {
            case JsNumberLiteral number:
                Emit(JsOpcode.LoadConstant, NumberConstant(number.Value));
                break;

            case JsStringLiteral text:
                Emit(JsOpcode.LoadConstant, StringConstant(text.Value));
                break;

            // ONLY A PARSE ADMITTING BIGINT PRODUCES THIS NODE, and the constant it writes is what
            // declares the surface: an artifact holding a BigInt constant says so beside its
            // manifest, and a composition that declined the surface refuses it at verification.
            case JsBigIntLiteral bigInt:
                surfaces.Add(JsSurfaces.BigInt);
                Emit(JsOpcode.LoadConstant, BigIntConstant(bigInt.Value));
                break;

            case JsBooleanLiteral boolean:
                Emit(boolean.Value ? JsOpcode.LoadTrue : JsOpcode.LoadFalse);
                break;

            case JsNullLiteral:
                Emit(JsOpcode.LoadNull);
                break;

            case JsThisExpression:
                Emit(JsOpcode.LoadThis);
                break;

            case JsIdentifier identifier:
                LoadName(identifier.Span, identifier.Name);
                break;

            case JsRegExpLiteral pattern:
                CompileRegExpLiteral(pattern);
                break;

            case JsArrayLiteral array:
                CompileArray(array);
                break;

            case JsObjectLiteral literal:
                CompileObject(literal);
                break;

            case JsFunctionExpression function:
                CompileFunctionExpression(function.Function);
                break;

            case JsClassExpression definition:
                CompileClass(definition.Class, string.Empty);
                break;

            case JsSuperMemberExpression member:
                CompileSuperKey(member);
                Emit(JsOpcode.LoadSuperProperty);
                break;

            case JsSuperCallExpression call:
                CompileSuperCall(call);
                break;

            case JsNewTargetExpression target:
                // `new.target` OUTSIDE A FUNCTION IS A SYNTAX ERROR AND NOT A MANIFEST REFUSAL.
                // The manifest admits it perfectly well; the program wrote it where the language
                // has nothing for it to mean, and the diagnostic code is what tells a conformance
                // runner which of the two happened.
                if (!insideFunction)
                {
                    Refuse(
                        target.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`new.target` is only admitted inside a function");

                    Emit(JsOpcode.LoadUndefined);
                    break;
                }

                Emit(JsOpcode.LoadNewTarget);
                break;

            case JsUnaryExpression unary:
                CompileUnary(unary);
                break;

            case JsUpdateExpression update:
                CompileUpdate(update);
                break;

            case JsBinaryExpression binary:
                CompileBinary(binary);
                break;

            case JsLogicalExpression logical:
                CompileLogical(logical);
                break;

            case JsConditionalExpression conditional:
            {
                CompileExpression(conditional.Test);
                var otherwise = NewLabel();
                var end = NewLabel();
                Branch(JsOpcode.JumpIfFalse, otherwise);
                CompileExpression(conditional.WhenTrue);
                Branch(JsOpcode.Jump, end);
                Mark(otherwise);
                CompileExpression(conditional.WhenFalse);
                Mark(end);
                break;
            }

            case JsAssignmentExpression assignment:
                CompileAssignment(assignment);
                break;

            case JsMemberExpression member:
                CompileMember(member);
                break;

            // A PRIVATE READ IS NOT A PROPERTY READ AND THE ABSENT CASE IS WHY. `o.x` answers
            // `undefined` for a name nothing defined; `o.#x` on an object the declaring class never
            // constructed is a TypeError, because a private name is not a key that object could
            // have had.
            case JsPrivateMemberExpression privateAccess:
                CompileExpression(privateAccess.Target);
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                Emit(JsOpcode.LoadPrivate);
                break;

            case JsPrivateInExpression brand:
                CompileExpression(brand.Target);
                EmitPrivateName(brand.Span, brand.Name);
                Emit(JsOpcode.HasPrivate);
                break;

            case JsCallExpression call:
                CompileCall(call);
                break;

            // A DYNAMIC IMPORT DECLARES THE DYNAMIC SURFACE HERE AND THE MODULE SURFACE NOWHERE.
            // The specifier is a VALUE, so whether the module it names is one this artifact already
            // carries is not known until the call runs; a call that finds it is answered from the
            // artifact and one that does not puts the specifier to the mediator, which is the same
            // door `eval` goes through and the same surface. The module surface is declared by
            // CARRYING RECORDS and by nothing else - a script may write `import()` and carries none
            // - so this call site cannot declare it without making an artifact say it holds a graph
            // it does not hold.
            case JsImportCall imported:
                CompileExpression(imported.Specifier);

                if (imported.Options is null)
                {
                    Emit(JsOpcode.LoadUndefined);
                }
                else
                {
                    CompileExpression(imported.Options);
                }

                surfaces.Add(JsSurfaces.Dynamic);
                Emit(JsOpcode.ImportCall, InternedName(module?.Key ?? scriptReferrer));
                break;

            case JsImportMeta meta:
                if (module is null)
                {
                    Refuse(
                        meta.Span,
                        SliceSourceDiagnosticCode.ImportMetaOutsideModuleGoal,
                        "`import.meta` is only admitted inside a module");

                    break;
                }

                Emit(JsOpcode.ImportMeta, (ushort)built.IndexOf(module));
                break;

            case JsNewExpression construction:
            {
                CompileExpression(construction.Callee);

                if (CarriesArgumentsInAnArray(construction.Arguments))
                {
                    CompileArgumentArray(construction.Arguments);
                    Position(construction.Span);
                    Emit(JsOpcode.ConstructSpread);
                    break;
                }

                foreach (var argument in construction.Arguments)
                {
                    CompileExpression(argument);
                }

                // THE CONSTRUCTION IS PLACED AT ITS OWN `new` AGAIN, after its arguments placed
                // themselves, so an error it makes - or one made by the constructor it runs - is
                // placed where the construction is written (JSD-0038).
                Position(construction.Span);

                Emit(
                    JsOpcode.Construct,
                    (byte)System.Math.Min(construction.Arguments.Count, 255));

                break;
            }

            // A DESTRUCTURING ASSIGNMENT'S VALUE IS THE RIGHT-HAND SIDE AND NOT THE PATTERN'S
            // RESULT. `print([a, b] = [1, 2])` prints the array, so the value is duplicated before
            // the pattern consumes its copy.
            case JsDestructuringAssignment destructuring:
                CompileExpression(destructuring.Value);
                Emit(JsOpcode.Duplicate);
                BindPattern(destructuring.Target, BindMode.Assign);
                break;

            case JsTemplateLiteral template:
                CompileTemplate(template);
                break;

            case JsTaggedTemplate tagged:
                CompileTaggedTemplate(tagged);
                break;

            case JsChainExpression chain:
                CompileChain(chain);
                break;

            // `yield` LEAVES ONE VALUE WHERE IT TOOK ONE. The operand is pushed and the opcode
            // replaces it with whatever the resumption sent, so a `yield` in the middle of an
            // expression needs nothing around it - which is what lets it appear in an argument
            // list, a loop condition or an object literal without the lowering knowing where it is.
            case JsYieldExpression yielded:
                if (yielded.Operand is null)
                {
                    Emit(JsOpcode.LoadUndefined);
                }
                else
                {
                    CompileExpression(yielded.Operand);
                }

                // A `yield` IN AN ASYNC GENERATOR AWAITS ITS OPERAND FIRST, and it is two
                // instructions rather than a mode on one because the awaiting is a SUSPENSION with
                // its own resumption. `Yield(v)` in the specification is
                // `AsyncGeneratorYield(? Await(v))` when the generator is async - so
                // `yield Promise.resolve(1)` hands the consumer `1` and not the promise, which is
                // the single most visible difference between the two kinds of generator. The
                // `Await` leaves what it resolved on the stack and the `Yield` takes it from there,
                // so the pair needs no temporary and no new opcode.
                if (yielded.IsDelegate)
                {
                    Emit(JsOpcode.YieldDelegate);
                    break;
                }

                if ((buffer.Flags & JsFormat.FunctionFlags.Async) != 0)
                {
                    Emit(JsOpcode.Await);
                }

                Emit(JsOpcode.Yield);
                break;

            // `await` LEAVES ONE VALUE WHERE IT TOOK ONE, exactly as `yield` does, so it needs
            // nothing around it either and may stand anywhere an expression may.
            case JsAwaitExpression awaited:
                CompileExpression(awaited.Operand);

                // AND THE UNIT IS CHECKED HERE, where the alternative is the worst failure shape
                // this component has: an `Await` in a unit carrying no async flag is refused by
                // THIS HOST'S OWN VERIFIER, on bytes THIS HOST'S OWN lowering produced, which is
                // the internal-consistency failure roadmap section 3.4 names. The parser's
                // `[Await]` contexts are what make this unreachable; this is what makes it a
                // diagnostic naming the construct rather than a refused artifact if they ever stop.
                if ((buffer.Flags & JsFormat.FunctionFlags.Async) == 0)
                {
                    Refuse(
                        awaited.Span,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "`await` is only admitted inside an async function");

                    break;
                }

                Emit(JsOpcode.Await);
                break;

            case JsSequenceExpression sequence:
                for (var index = 0; index < sequence.Expressions.Count; index++)
                {
                    CompileExpression(sequence.Expressions[index]);

                    if (index != sequence.Expressions.Count - 1)
                    {
                        Emit(JsOpcode.Pop);
                    }
                }

                break;

            default:
                Refuse(
                    expression.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "this expression is not admitted by the declared feature manifest");

                Emit(JsOpcode.LoadUndefined);
                break;
        }
    }

    /// <summary>
    /// Emits a closure, giving a NAMED function expression a scope holding its own name.
    /// </summary>
    /// <remarks>
    /// <c>var f = function g() { return g; };</c> must see <c>g</c> inside the body and must not
    /// introduce it outside. The specification's answer is a one-binding environment created around
    /// the function object and holding it; this is that environment. Popping it afterwards does not
    /// destroy it - the closure captured the record itself, and what is popped is only this frame's
    /// view of the chain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FB8349
    // Broiler-Human:        PENDING
    private void CompileFunctionExpression(JsFunctionNode function)
    {
        if (function.Name.Length == 0 || function.IsArrow)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            return;
        }

        var outer = scope;
        scope = new Scope(ScopeKind.Block, outer) { IsFunctionName = true };
        blockDepth++;
        var site = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((site, scope));
        var slot = scope.Declare(function.Name, constant: false);
        Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
        Emit(JsOpcode.Duplicate);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }
}
