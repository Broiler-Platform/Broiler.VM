// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           0
// Human-reviewed:   0/14
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A81EBE
    // Broiler-Human:        PENDING
    private void CompileUnary(UnaryExpression unary)
    {
        var operand = JsTreeFacts.Unwrap(unary.Operand);
        var op = JsTreeFacts.Unary(unary.Operator);

        switch (op)
        {
            // `typeof` OF AN IMPORT IS NOT `typeof` OF AN UNDECLARED NAME. The form below exists so
            // that asking about a name nothing declares answers `"undefined"` instead of throwing;
            // an imported binding IS declared, so a read of one before its module initialised it is
            // the dead-zone `ReferenceError` the language gives - and answering `"undefined"` here
            // would have hidden exactly the case a cyclic import produces.
            case SliceTokenKind.Typeof when operand is Identifier name &&
                !Resolvable(name.Name) &&
                (module is null || !module.Imports.ContainsKey(name.Name)):
                if (Shadowable(name.Name, out var absent))
                {
                    // `with (o) { typeof x }` is the object's value when it has one and the absent
                    // global's `"undefined"` when nothing has it, so the search's fall-back arm is
                    // the read that does not throw.
                    EmitDynamicName(name.Name, absent, wantsBase: false, orUndefined: true);
                }
                else if (TryEvalHops(out var evalHops))
                {
                    EmitEvalName(JsOpcode.LoadEvalNameOrUndefined, evalHops, name.Name);
                }
                else
                {
                    Emit(JsOpcode.LoadGlobalOrUndefined, InternedName(name.Name));
                }

                Emit(JsOpcode.TypeOf);
                return;

            // `delete x` INSIDE A `with` BODY DELETES A PROPERTY WHEN THE OBJECT HAS THE NAME, which
            // is the one spelling of `delete` that reaches an environment record at all. When no
            // object on the chain has it the answer is about a binding rather than about a property:
            // a slot binding is not configurable and the language answers `false`, which is what the
            // fall-back arm pushes. The operand is NOT evaluated on either path, because `delete` of
            // a reference never evaluates it.
            case SliceTokenKind.Delete when operand is Identifier bare &&
                Shadowable(bare.Name, out var reachable):
            {
                var live = buffer.Height;
                var key = InternedName(bare.Name);
                var enclosing = NewLabel();
                var settled = NewLabel();

                EmitResolve(reachable, key);
                Emit(JsOpcode.Duplicate);
                Branch(JsOpcode.JumpIfFalse, enclosing);
                Emit(JsOpcode.DeleteProperty, key);
                Branch(JsOpcode.Jump, settled);

                Mark(enclosing);
                Emit(JsOpcode.Pop);
                if (Resolvable(bare.Name))
                {
                    Emit(JsOpcode.LoadFalse);
                }
                else if (TryEvalHops(out var evalHops))
                {
                    EmitEvalName(JsOpcode.DeleteEvalName, evalHops, bare.Name);
                }
                else
                {
                    Emit(JsOpcode.DeleteGlobalBinding, InternedName(bare.Name));
                }
                Mark(settled);
                buffer.Rejoin(live + 1);
                return;
            }

            case SliceTokenKind.Delete when operand is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member:
                CompileExpression(member.Object);

                if (JsTreeFacts.Computed(member) is null)
                {
                    Emit(JsOpcode.DeleteProperty, InternedName(JsTreeFacts.PropertyName(member)));
                }
                else
                {
                    CompileExpression(JsTreeFacts.Computed(member)!);
                    Emit(JsOpcode.DeleteIndex);
                }

                return;

            // `delete a?.b` DELETES, AND WHEN `a` IS NULLISH IT ANSWERS `true`. The chain is not
            // evaluated to a value and then deleted - that would delete nothing and answer `true`
            // for a reason the language does not give. It is the same chain lowering with the
            // deletion as its last link and `true` as what its short circuit produces, which is
            // exactly what the specification says a short-circuited `delete` completes with.
            case SliceTokenKind.Delete when operand is ChainExpression chain &&
                JsTreeFacts.Unwrap(chain.Expression) is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } optional:
            {
                var end = NewLabel();
                EmitChainLink(optional.Object, end, shortIsTrue: true);

                if (optional.IsOptional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue: true);
                }

                if (JsTreeFacts.Computed(optional) is null)
                {
                    Emit(JsOpcode.DeleteProperty, InternedName(JsTreeFacts.PropertyName(optional)));
                }
                else
                {
                    CompileExpression(JsTreeFacts.Computed(optional)!);
                    Emit(JsOpcode.DeleteIndex);
                }

                Mark(end);
                return;
            }

            // `delete o.#x` IS A SYNTAX ERROR AND NOT A DELETION THAT ANSWERS `true`. A private
            // element is not a property and there is no operation that removes one, so the language
            // refuses the spelling rather than giving it a reading. Falling through to the arm
            // below would have evaluated the access - throwing on a foreign object - and then
            // answered `true` for a deletion that never happened.
            case SliceTokenKind.Delete when operand is MemberExpression { Property: PrivateIdentifier }:
                throw BinderAdmitted(JsTreeFacts.At(unary), "the deletion of a private element");

            // `delete x` FOR A BARE NAME NEVER EVALUATES `x`. The operator takes a reference and
            // asks whether it can be removed, so a lowering that read the name and threw the value
            // away answered `true` for a `var` the language says is not deletable and threw a
            // `ReferenceError` for a name nobody declared - where the language answers `true`. A
            // name this unit resolves to a SLOT is answered here, because a slot binding is never
            // deletable and the compiler already knows which names those are.
            case SliceTokenKind.Delete when operand is Identifier plain:
                if (Resolvable(plain.Name))
                {
                    Emit(JsOpcode.LoadFalse);
                    return;
                }

                // A NAME EVAL CODE DOES NOT BIND IS DELETED WHERE THE CALLER'S SCOPE BINDS IT, which
                // is `false` for a declarative binding and a deletion for a `with` object's property.
                if (TryEvalHops(out var deleteHops))
                {
                    EmitEvalName(JsOpcode.DeleteEvalName, deleteHops, plain.Name);
                    return;
                }

                Emit(JsOpcode.DeleteGlobalBinding, InternedName(plain.Name));
                return;

            // `delete super.x` AND `delete super[k]` ARE A ReferenceError, after the this binding
            // and the key expression are evaluated and before the key is converted (13.5.1.2 step
            // 5, with the key's conversion deferred since ES2024). Until 2026-10-03 the property
            // was read and `true` answered (JSC-254).
            case SliceTokenKind.Delete when operand is MemberExpression { Object: SuperExpression } inherited:
                if (!insideMethod)
                {
                    throw BinderAdmitted(JsTreeFacts.At(inherited), "a `super` property outside a method");
                }

                Emit(JsOpcode.LoadThis);
                Emit(JsOpcode.Pop);

                if (JsTreeFacts.Computed(inherited) is not null)
                {
                    CompileExpression(JsTreeFacts.Computed(inherited)!);
                    Emit(JsOpcode.Pop);
                }

                Emit(JsOpcode.ThrowReferenceError);
                return;

            case SliceTokenKind.Delete:
                CompileExpression(unary.Operand);
                Emit(JsOpcode.Pop);
                Emit(JsOpcode.LoadTrue);
                return;

            default:
                break;
        }

        CompileExpression(unary.Operand);

        Emit(op switch
        {
            SliceTokenKind.Minus => JsOpcode.Negate,
            SliceTokenKind.Plus => JsOpcode.ToNumber,
            SliceTokenKind.Bang => JsOpcode.Not,
            SliceTokenKind.Tilde => JsOpcode.BitwiseNot,
            SliceTokenKind.Typeof => JsOpcode.TypeOf,
            _ => JsOpcode.Void,
        });
    }

    /// <summary>
    /// Converts an update expression's operand: <c>ToNumeric</c> under the wide manifest, and
    /// <c>ToNumber</c> under the numeric one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A BigInt must survive the conversion</b>: <c>x++</c> on <c>5n</c> is <c>6n</c>, where
    /// <c>ToNumber</c> would throw. Behind the gate (JSeal B03) this was two <c>Negate</c>
    /// instructions and a <c>typeof</c> branch, because no instruction existed for it and adding
    /// one for a gated path was refused; admitting the surface (B05) made it every wide program's
    /// path, and a branch in every <c>i++</c> of every loop is a price no program should pay for a
    /// type it may never hold, so the format gained <see cref="JsOpcode.ToNumeric"/>,
    /// <see cref="JsOpcode.Increment"/> and <see cref="JsOpcode.Decrement"/> (JSD-0033 section 7).
    /// </para>
    /// <para>
    /// <b>The numeric manifest keeps the bytes it always had</b>: it admits no BigInt and none of
    /// the three instructions.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8C0D75
    // Broiler-Human:        PENDING
    private void EmitUpdateOperand() =>
        Emit(request.AdmitsBigIntLiterals ? JsOpcode.ToNumeric : JsOpcode.ToNumber);

    /// <summary>
    /// Adds or subtracts one from the converted operand on the stack: one of the operand's own
    /// type under the wide manifest, and the Number <c>1</c> under the numeric one.
    /// </summary>
    /// <remarks>
    /// The specification's update is <c>Number::add(x, 1)</c> or <c>BigInt::add(x, 1n)</c> by the
    /// operand's type, which is only known when the program runs, so the wide lowering hands the
    /// choice to <see cref="JsOpcode.Increment"/> or <see cref="JsOpcode.Decrement"/>; the numeric
    /// lowering loads <paramref name="one"/>, the constant <c>1</c> its caller interned.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AC9D51
    // Broiler-Human:        PENDING
    private void EmitUpdateStep(JsOpcode add, ushort one)
    {
        if (request.AdmitsBigIntLiterals)
        {
            Emit(add == JsOpcode.Add ? JsOpcode.Increment : JsOpcode.Decrement);
            return;
        }

        Emit(JsOpcode.LoadConstant, one);
        Emit(add);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3202E2
    // Broiler-Human:        PENDING
    private void CompileUpdate(UpdateExpression update)
    {
        var operand = JsTreeFacts.Unwrap(update.Operand);
        var span = JsTreeFacts.At(update);
        // THE NUMERIC LOWERING INTERNS ITS `1` HERE, FIRST, as it always did, so its artifacts keep
        // their constant pools byte for byte; the wide lowering needs no constant at all.
        var one = request.AdmitsBigIntLiterals ? (ushort)0 : NumberConstant(1);
        var add = update.Operator == UpdateOperator.Increment ? JsOpcode.Add : JsOpcode.Subtract;

        if (operand is Identifier postfixed && !update.IsPrefix && Shadowable(postfixed.Name, out _))
        {
            // THE OLD VALUE IS THE ANSWER AND THE WRITE GOES THROUGH THE REFERENCE, so the numeric
            // old value is kept in a slot of its own while the reference's two branches write.
            var owner = FunctionScope();
            var kept = owner.Declare("#update" + owner.SlotCount, constant: false);

            TryEmitShadowedReference(span, postfixed.Name, read: true, () =>
            {
                EmitUpdateOperand();
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
                EmitUpdateStep(add, one);
            });

            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
            return;
        }

        if (operand is Identifier prefixed && update.IsPrefix &&
            TryEmitShadowedReference(span, prefixed.Name, read: true, () =>
            {
                EmitUpdateOperand();
                EmitUpdateStep(add, one);
            }))
        {
            return;
        }

        if (operand is Identifier name)
        {
            LoadName(span, name.Name);
            EmitUpdateOperand();

            if (!update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitUpdateStep(add, one);
                StoreName(span, name.Name);
                Emit(JsOpcode.Pop);
                return;
            }

            EmitUpdateStep(add, one);
            StoreName(span, name.Name);
            return;
        }

        if (operand is MemberExpression { Object: SuperExpression } inherited)
        {
            var owner = FunctionScope();
            var kept = owner.Declare("#update" + owner.SlotCount, constant: false);
            CompileSuperKey(inherited);
            EmitSuperReadKeepingKey(inherited);
            EmitUpdateOperand();

            if (!update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            EmitUpdateStep(add, one);

            if (update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.StoreSuperProperty);
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
            return;
        }

        if (operand is MemberExpression { Property: PrivateIdentifier } privateOperand)
        {
            var owner = FunctionScope();
            var kept = owner.Declare("#update" + owner.SlotCount, constant: false);
            CompileExpression(privateOperand.Object);
            EmitPrivateName(JsTreeFacts.At(privateOperand), JsTreeFacts.PropertyName(privateOperand));
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.LoadPrivate);
            EmitUpdateOperand();

            if (!update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            EmitUpdateStep(add, one);

            if (update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.StorePrivate);
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
            return;
        }

        if (operand is CallExpression { Callee: not SuperExpression } called && !strict)
        {
            EmitCallTargetThrow(called);
            return;
        }

        if (operand is not MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member)
        {
            throw BinderAdmitted(span, "an update of an operand that is not a reference");
        }

        var function = FunctionScope();
        var temporary = function.Declare("#update" + function.SlotCount, constant: false);
        CompileExpression(member.Object);

        if (JsTreeFacts.Computed(member) is null)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(member)));
            EmitUpdateOperand();

            if (!update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
            }

            EmitUpdateStep(add, one);

            if (update.IsPrefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
            }

            Emit(JsOpcode.SetProperty, InternedName(JsTreeFacts.PropertyName(member)));
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, temporary);
            return;
        }

        // The key is converted ONCE, before the pair is duplicated, so the read and the write
        // below share the property key it produced rather than each running its `toString`.
        CompileExpression(JsTreeFacts.Computed(member)!);
        Emit(JsOpcode.ToPropertyKey);
        Emit(JsOpcode.DuplicateTwo);
        Emit(JsOpcode.GetIndex);
        EmitUpdateOperand();

        if (!update.IsPrefix)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
        }

        EmitUpdateStep(add, one);

        if (update.IsPrefix)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
        }

        Emit(JsOpcode.SetIndex);
        Emit(JsOpcode.Pop);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, temporary);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=74DF64
    // Broiler-Human:        PENDING
    private void CompileBinary(BinaryExpression binary)
    {
        var chain = new System.Collections.Generic.List<BinaryExpression>();
        Expression current = binary;
        while (JsTreeFacts.Unwrap(current) is BinaryExpression b)
        {
            chain.Add(b);
            current = b.Left;
        }

        CompileExpression(current);
        for (var i = chain.Count - 1; i >= 0; i--)
        {
            var node = chain[i];
            CompileExpression(node.Right);
            Emit(BinaryOpcode(JsTreeFacts.Binary(node.Operator)));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1CF528
    // Broiler-Human:        PENDING
    private void CompileMember(MemberExpression member)
    {
        var chain = new System.Collections.Generic.List<MemberExpression>();
        Expression current = member;
        while (JsTreeFacts.Unwrap(current) is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } m && !m.IsOptional)
        {
            chain.Add(m);
            current = m.Object;
        }

        CompileExpression(current);
        for (var i = chain.Count - 1; i >= 0; i--)
        {
            var m = chain[i];
            if (JsTreeFacts.Computed(m) is null)
            {
                // THE READ IS PLACED AT ITS NAME, where an error it raises is shown (JSD-0038).
                PositionAt(JsTreeFacts.NameSpan(m));
                Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(m)));
            }
            else
            {
                CompileExpression(JsTreeFacts.Computed(m)!);
                Emit(JsOpcode.GetIndex);
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=229EB0
    // Broiler-Human:        PENDING
    private void CompileLogical(LogicalExpression logical)
    {
        var chain = new System.Collections.Generic.List<LogicalExpression>();
        Expression current = logical;
        while (JsTreeFacts.Unwrap(current) is LogicalExpression l)
        {
            chain.Add(l);
            current = l.Left;
        }

        CompileExpression(current);
        for (var i = chain.Count - 1; i >= 0; i--)
        {
            var node = chain[i];
            var end = NewLabel();
            Emit(JsOpcode.Duplicate);

            switch (JsTreeFacts.Logical(node.Operator))
            {
                case SliceTokenKind.AmpersandAmpersand:
                    Branch(JsOpcode.JumpIfFalse, end);
                    break;

                case SliceTokenKind.BarBar:
                    Branch(JsOpcode.JumpIfTrue, end);
                    break;

                default:
                    EmitIsNullish();
                    Emit(JsOpcode.Not);
                    Branch(JsOpcode.JumpIfTrue, end);
                    break;
            }

            Emit(JsOpcode.Pop);
            CompileExpression(node.Right);
            Mark(end);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6536C5
    // Broiler-Human:        PENDING
    private void CompileAssignment(AssignmentExpression assignment)
    {
        var target = JsTreeFacts.AssignedTarget(assignment);
        var op = JsTreeFacts.Compound(assignment.Operator);
        var span = JsTreeFacts.At(assignment);

        if (op == SliceTokenKind.Equals)
        {
            if (target is Identifier name)
            {
                var inferred = JsTreeFacts.ParenthesisedTarget(assignment) ? string.Empty : name.Name;

                if (TryEmitShadowedReference(
                    span, name.Name, read: false, () => CompileNamedValue(assignment.Value, inferred)))
                {
                    return;
                }

                CompileNamedValue(assignment.Value, inferred);
                StoreName(span, name.Name);
                return;
            }

            if (target is MemberExpression { Object: SuperExpression } inherited)
            {
                CompileSuperKey(inherited);
                CompileExpression(assignment.Value);
                Emit(JsOpcode.StoreSuperProperty);
                return;
            }

            if (target is MemberExpression { Property: PrivateIdentifier } privateTarget)
            {
                CompileExpression(privateTarget.Object);
                EmitPrivateName(JsTreeFacts.At(privateTarget), JsTreeFacts.PropertyName(privateTarget));
                CompileExpression(assignment.Value);
                Emit(JsOpcode.StorePrivate);
                return;
            }

            if (target is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member)
            {
                CompileExpression(member.Object);

                if (JsTreeFacts.Computed(member) is null)
                {
                    CompileExpression(assignment.Value);
                    Emit(JsOpcode.SetProperty, InternedName(JsTreeFacts.PropertyName(member)));
                    return;
                }

                CompileExpression(JsTreeFacts.Computed(member)!);
                CompileExpression(assignment.Value);
                Emit(JsOpcode.SetIndex);
                return;
            }

            if (target is CallExpression { Callee: not SuperExpression } called && !strict)
            {
                EmitCallTargetThrow(called);
                return;
            }

            throw BinderAdmitted(span, "an assignment to a target that is not a reference");
        }

        if (op is SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar or
            SliceTokenKind.QuestionQuestion)
        {
            CompileLogicalAssignment(assignment);
            return;
        }

        var opcode = BinaryOpcode(op);

        if (target is Identifier identifier)
        {
            if (TryEmitShadowedReference(span, identifier.Name, read: true, () =>
            {
                CompileExpression(assignment.Value);
                Emit(opcode);
            }))
            {
                return;
            }

            LoadName(span, identifier.Name);
            CompileExpression(assignment.Value);
            Emit(opcode);
            StoreName(span, identifier.Name);
            return;
        }

        if (target is MemberExpression { Object: SuperExpression } host)
        {
            // The key is computed once and kept, so the read and the write agree about it even
            // when it is an expression with a side effect, and it is converted once.
            CompileSuperKey(host);
            EmitSuperReadKeepingKey(host);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.StoreSuperProperty);
            return;
        }

        // THE OBJECT AND THE NAME ARE PUSHED ONCE AND DUPLICATED, so `o.#x += f()` evaluates `o`
        // once - which is what the language says and what re-compiling the target for the write
        // would have got wrong for any target with a side effect.
        if (target is MemberExpression { Property: PrivateIdentifier } privateAccess)
        {
            CompileExpression(privateAccess.Object);
            EmitPrivateName(JsTreeFacts.At(privateAccess), JsTreeFacts.PropertyName(privateAccess));
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.LoadPrivate);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.StorePrivate);
            return;
        }

        if (target is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } access)
        {
            CompileExpression(access.Object);

            if (JsTreeFacts.Computed(access) is null)
            {
                Emit(JsOpcode.Duplicate);
                Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(access)));
                CompileExpression(assignment.Value);
                Emit(opcode);
                Emit(JsOpcode.SetProperty, InternedName(JsTreeFacts.PropertyName(access)));
                return;
            }

            // THE KEY IS CONVERTED ONCE AND BEFORE THE RIGHT-HAND SIDE, which is where `GetValue`
            // converts it; the write then uses the key the read used, as the reference keeps it.
            CompileExpression(JsTreeFacts.Computed(access)!);
            Emit(JsOpcode.ToPropertyKey);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.GetIndex);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.SetIndex);
            return;
        }

        if (target is CallExpression { Callee: not SuperExpression } compounded && !strict)
        {
            EmitCallTargetThrow(compounded);
            return;
        }

        throw BinderAdmitted(span, "an assignment to a target that is not a reference");
    }

    /// <summary>
    /// Lowers a call written as an assignment target in non-strict code: the call, then Annex B's
    /// <c>ReferenceError</c>.
    /// </summary>
    /// <remarks>
    /// <b>The call runs and nothing after it does</b> (JSP-7, JSC-239): its result is not converted,
    /// and the right-hand side of the assignment is never evaluated, which is what test262 asks of
    /// <c>f() = g()</c>. The instruction that throws stands where the assignment's value would be
    /// pushed, so every lowering around this one keeps the stack it expects.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C64FE4
    // Broiler-Human:        PENDING
    private void EmitCallTargetThrow(CallExpression call)
    {
        CompileExpression(call);
        Emit(JsOpcode.Pop);
        Emit(JsOpcode.ThrowReferenceError);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=40BDF1
    // Broiler-Human:        PENDING
    private void CompileLogicalAssignment(AssignmentExpression assignment)
    {
        var target = JsTreeFacts.AssignedTarget(assignment);
        var span = JsTreeFacts.At(assignment);

        if (target is MemberExpression { Object: not SuperExpression, Property: not PrivateIdentifier } member)
        {
            CompileLogicalMemberAssignment(assignment, member);
            return;
        }

        if (target is MemberExpression { Property: PrivateIdentifier } privateTarget)
        {
            CompileLogicalPrivateAssignment(assignment, privateTarget);
            return;
        }

        if (target is MemberExpression { Object: SuperExpression } superTarget)
        {
            CompileLogicalSuperAssignment(assignment, superTarget);
            return;
        }

        // ANY OTHER TARGET IS REFUSED BY THE BINDER, under 2104, before any of the unit is lowered
        // (SJL-12.6 item 2, JSD-0080 decision D12), and the parser refuses every one first today.
        if (target is not Identifier name)
        {
            throw BinderAdmitted(
                span, "a logical assignment to a target that is neither a name nor a property");
        }

        LoadName(span, name.Name);
        var end = NewLabel();
        Emit(JsOpcode.Duplicate);

        switch (JsTreeFacts.Compound(assignment.Operator))
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, end);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, end);
                break;

            default:
                EmitIsNullish();
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, end);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileNamedValue(assignment.Value, JsTreeFacts.ParenthesisedTarget(assignment) ? string.Empty : name.Name);
        StoreName(span, name.Name);
        Mark(end);
    }

    /// <summary>Lowers <c>o.x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE REFERENCE IS EVALUATED ONCE AND THE WRITE HAPPENS ONLY WHEN THE TEST WANTS IT.</b>
    /// Both halves are observable: <c>f().x ||= v</c> calls <c>f</c> exactly once, and
    /// <c>o.x ||= v</c> on a truthy <c>o.x</c> performs no <c>[[Set]]</c> at all - so a setter does
    /// not run, and a read-only property does not throw in strict mode. Lowering this as
    /// <c>o.x = o.x || v</c>, which is the rewrite it looks like, gets both wrong.
    /// </para>
    /// <para>
    /// <b>The two paths meet at one height</b>, which is what lets the verifier check this at all:
    /// the assigning path ends with the store's own result on the stack and the short-circuiting
    /// path unwinds the base - and the key, when there is one - from underneath the value it read.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D37AD2
    // Broiler-Human:        PENDING
    private void CompileLogicalMemberAssignment(
        AssignmentExpression assignment, MemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();
        CompileExpression(member.Object);

        if (JsTreeFacts.Computed(member) is not null)
        {
            CompileExpression(JsTreeFacts.Computed(member)!);
            Emit(JsOpcode.ToPropertyKey);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.GetIndex);
        }
        else
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName(JsTreeFacts.PropertyName(member)));
        }

        Emit(JsOpcode.Duplicate);

        switch (JsTreeFacts.Compound(assignment.Operator))
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                EmitIsNullish();
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);

        if (JsTreeFacts.Computed(member) is not null)
        {
            Emit(JsOpcode.SetIndex);
        }
        else
        {
            Emit(JsOpcode.SetProperty, InternedName(JsTreeFacts.PropertyName(member)));
        }

        Branch(JsOpcode.Jump, end);
        Mark(kept);

        // THE VALUE THAT WAS ALREADY THERE IS THE ANSWER, and what is under it is this lowering's
        // own working: the base, and the key when the member was computed. Both are dropped here
        // rather than left for the enclosing expression to trip over.
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);

        if (JsTreeFacts.Computed(member) is not null)
        {
            Emit(JsOpcode.Swap);
            Emit(JsOpcode.Pop);
        }

        Mark(end);
    }

    /// <summary>Lowers <c>super.x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <b>The key is computed once and kept</b>, which is what the compound form beside this one
    /// already does and for the same reason: <c>super[f()] ||= v</c> calls <c>f</c> once, and the
    /// read and the write have to agree about the key it answered. A <c>super</c> reference carries
    /// no base on the stack — the home object and the receiver are the frame's — so this is the
    /// narrowest of the four shapes and the only one whose short circuit drops a single value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F7F3D8
    // Broiler-Human:        PENDING
    private void CompileLogicalSuperAssignment(
        AssignmentExpression assignment, MemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();

        CompileSuperKey(member);
        EmitSuperReadKeepingKey(member);
        Emit(JsOpcode.Duplicate);

        switch (JsTreeFacts.Compound(assignment.Operator))
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                EmitIsNullish();
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);
        Emit(JsOpcode.StoreSuperProperty);
        Branch(JsOpcode.Jump, end);
        Mark(kept);

        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Mark(end);
    }

    /// <summary>Lowers <c>o.#x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <b>It is the computed member's shape with the private pair in place of the index pair</b>,
    /// and it is a method of its own rather than a branch inside that one because the two carry
    /// different things on the stack under the value they read: a base and a KEY there, a base and a
    /// PRIVATE NAME here. <b>The short circuit is what the suite tests</b>: <c>o.#m ??= v</c> where
    /// <c>#m</c> is a private METHOD is a program when <c>#m</c> is not nullish, because the store
    /// that would refuse never runs — so the assigning path must be the only one that stores.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=314944
    // Broiler-Human:        PENDING
    private void CompileLogicalPrivateAssignment(
        AssignmentExpression assignment, MemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();

        CompileExpression(member.Object);
        EmitPrivateName(JsTreeFacts.At(member), JsTreeFacts.PropertyName(member));
        Emit(JsOpcode.DuplicateTwo);
        Emit(JsOpcode.LoadPrivate);
        Emit(JsOpcode.Duplicate);

        switch (JsTreeFacts.Compound(assignment.Operator))
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                EmitIsNullish();
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);
        Emit(JsOpcode.StorePrivate);
        Branch(JsOpcode.Jump, end);
        Mark(kept);

        // The base and the private name are this lowering's own working, and the value that was
        // already there is the answer, so both are dropped from under it.
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Mark(end);
    }
}
