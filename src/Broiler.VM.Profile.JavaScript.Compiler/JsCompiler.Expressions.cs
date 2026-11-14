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

using Broiler.ECMAScript.Syntax;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- expressions ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=21BC13
    // Broiler-Human:        PENDING
    private void CompileExpression(Expression written)
    {
        // A PARENTHESIS IS THE EXPRESSION IT HOLDS, seen through by a loop and placed where the
        // expression it holds begins, as the wide parser's tree held it (SJL-13.3, step 2).
        var expression = JsTreeFacts.Unwrap(written);
        Position(JsTreeFacts.At(expression));

        switch (expression)
        {
            // ONLY A PARSE ADMITTING BIGINT ADMITS ONE, and the constant it writes is what declares the
            // surface: its value is the admission walk's (SJL-13.3, step 2).
            case NumericLiteral bigInt when (bigInt.Flags & NumericLiteralFlags.BigInt) != 0:
                surfaces.Add(JsSurfaces.BigInt);
                Emit(JsOpcode.LoadConstant, BigIntConstant(unitParse.BigInts[bigInt]));
                break;

            case NumericLiteral number:
                Emit(JsOpcode.LoadConstant, NumberConstant(number.Value));
                break;

            case StringLiteral text:
                Emit(JsOpcode.LoadConstant, StringConstant(text.Value));
                break;

            case BooleanLiteral boolean:
                Emit(boolean.Value ? JsOpcode.LoadTrue : JsOpcode.LoadFalse);
                break;

            case NullLiteral:
                Emit(JsOpcode.LoadNull);
                break;

            case ThisExpression:
                Emit(JsOpcode.LoadThis);
                break;

            case Identifier identifier:
                LoadName(JsTreeFacts.At(identifier), identifier.Name);
                break;

            case RegularExpressionLiteral pattern:
                CompileRegExpLiteral(pattern);
                break;

            case ArrayLiteral array:
                CompileArray(array);
                break;

            case ObjectLiteral literal:
                CompileObject(literal);
                break;

            case FunctionExpression function:
                CompileFunctionExpression(function.Function, JsTreeFacts.At(function));
                break;

            case ArrowFunctionExpression arrow:
                CompileFunctionExpression(arrow.Function, JsTreeFacts.At(arrow));
                break;

            case ClassExpression definition:
                CompileClass(definition.Class, string.Empty);
                break;

            case MemberExpression { Object: SuperExpression } member:
                CompileSuperKey(member);
                Emit(JsOpcode.LoadSuperProperty);
                break;

            case CallExpression { Callee: SuperExpression } call:
                CompileSuperCall(call);
                break;

            case MetaProperty { Meta.Name: "new" } target:
                // `new.target` OUTSIDE A FUNCTION IS A SYNTAX ERROR AND NOT A MANIFEST REFUSAL.
                // The manifest admits it perfectly well; the program wrote it where the language
                // has nothing for it to mean, and the diagnostic code is what tells a conformance
                // runner which of the two happened. The binder refuses it under that code before
                // any of the unit is lowered (SJL-12.6 item 2); here the lowering only asserts it.
                if (!insideFunction)
                {
                    throw BinderAdmitted(JsTreeFacts.At(target), "`new.target` outside a function");
                }

                Emit(JsOpcode.LoadNewTarget);
                break;

            case UnaryExpression unary:
                CompileUnary(unary);
                break;

            case UpdateExpression update:
                CompileUpdate(update);
                break;

            case BinaryExpression binary:
                CompileBinary(binary);
                break;

            case LogicalExpression logical:
                CompileLogical(logical);
                break;

            case ConditionalExpression conditional:
            {
                CompileExpression(conditional.Test);
                var otherwise = NewLabel();
                var end = NewLabel();
                Branch(JsOpcode.JumpIfFalse, otherwise);
                CompileExpression(conditional.Consequent);
                Branch(JsOpcode.Jump, end);
                Mark(otherwise);
                CompileExpression(conditional.Alternate);
                Mark(end);
                break;
            }

            // A DESTRUCTURING ASSIGNMENT'S VALUE IS THE RIGHT-HAND SIDE AND NOT THE PATTERN'S
            // RESULT. `print([a, b] = [1, 2])` prints the array, so the value is duplicated before
            // the pattern consumes its copy.
            case AssignmentExpression { Target: ArrayPattern or ObjectPattern } destructuring:
                CompileExpression(destructuring.Value);
                Emit(JsOpcode.Duplicate);
                BindPattern((Pattern)destructuring.Target, BindMode.Assign);
                break;

            case AssignmentExpression assignment:
                CompileAssignment(assignment);
                break;

            case MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member:
                CompileMember(member);
                break;

            // A PRIVATE READ IS NOT A PROPERTY READ AND THE ABSENT CASE IS WHY. `o.x` answers
            // `undefined` for a name nothing defined; `o.#x` on an object the declaring class never
            // constructed is a TypeError, because a private name is not a key that object could
            // have had.
            case MemberExpression { Property: PrivateIdentifier } privateAccess:
                CompileExpression(privateAccess.Object);
                EmitPrivateName(JsTreeFacts.At(privateAccess), JsTreeFacts.PropertyName(privateAccess));
                Emit(JsOpcode.LoadPrivate);
                break;

            case PrivateInExpression brand:
                CompileExpression(brand.Right);
                EmitPrivateName(JsTreeFacts.At(brand), brand.Name.Name);
                Emit(JsOpcode.HasPrivate);
                break;

            case CallExpression call:
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
            case ImportCall imported:
                CompileExpression(imported.Source);

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

            case MetaProperty meta:
                if (module is null)
                {
                    throw BinderAdmitted(JsTreeFacts.At(meta), "`import.meta` outside a module");
                }

                Emit(JsOpcode.ImportMeta, (ushort)built.IndexOf(module));
                break;

            case NewExpression construction:
            {
                CompileExpression(construction.Callee);

                if (CarriesArgumentsInAnArray(construction.Arguments))
                {
                    CompileArgumentArray(construction.Arguments);
                    Position(JsTreeFacts.At(construction));
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
                Position(JsTreeFacts.At(construction));

                Emit(
                    JsOpcode.Construct,
                    (byte)System.Math.Min(construction.Arguments.Count, 255));

                break;
            }

            case TemplateLiteral template:
                CompileTemplate(template);
                break;

            case TaggedTemplateExpression tagged:
                CompileTaggedTemplate(tagged);
                break;

            case ChainExpression chain:
                CompileChain(chain);
                break;

            // `yield` LEAVES ONE VALUE WHERE IT TOOK ONE. The operand is pushed and the opcode
            // replaces it with whatever the resumption sent, so a `yield` in the middle of an
            // expression needs nothing around it - which is what lets it appear in an argument
            // list, a loop condition or an object literal without the lowering knowing where it is.
            case YieldExpression yielded:
                if (yielded.Argument is null)
                {
                    Emit(JsOpcode.LoadUndefined);
                }
                else
                {
                    CompileExpression(yielded.Argument);
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
            case AwaitExpression awaited:
                CompileExpression(awaited.Argument);

                // AND THE UNIT IS CHECKED HERE, where the alternative is the worst failure shape
                // this component has: an `Await` in a unit carrying no async flag is refused by
                // THIS HOST'S OWN VERIFIER, on bytes THIS HOST'S OWN lowering produced, which is
                // the internal-consistency failure roadmap section 3.4 names. The parser's
                // `[Await]` contexts keep it out of every unit - a template substitution's included,
                // since SJL-12.6 item 4 has the parser read it in place (JSD-0080 decision D15) - and
                // the binder refuses what would reach it, under 2104, before any of the unit is
                // lowered (SJL-12.6 item 2, decision D12); here the lowering only asserts it.
                if ((buffer.Flags & JsFormat.FunctionFlags.Async) == 0)
                {
                    throw BinderAdmitted(JsTreeFacts.At(awaited), "an `await` outside an async function");
                }

                Emit(JsOpcode.Await);
                break;

            case SequenceExpression sequence:
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
                    JsTreeFacts.At(expression),
                    SliceSourceDiagnosticCode.ConstructTheLoweringHasNoArmFor,
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=172EFF
    // Broiler-Human:        PENDING
    private void CompileFunctionExpression(FunctionNode function, SliceSourceSpan span)
    {
        var name = function.Name?.Name ?? string.Empty;

        if (name.Length == 0 || function.IsArrow)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function, span, name));
            return;
        }

        var outer = scope;
        scope = new Scope(ScopeKind.Block, outer) { IsFunctionName = true };
        blockDepth++;
        var site = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((site, scope));
        var slot = scope.Declare(name, constant: false);
        Emit(JsOpcode.Closure, (ushort)CompileFunction(function, span, name));
        Emit(JsOpcode.Duplicate);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }
}
