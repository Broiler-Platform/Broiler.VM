// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           3
// Human-reviewed:   0/13
// IP risk:          None
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- destructuring -------------------------------------------------------------------------

    /// <summary>What a pattern's leaf does with the value that reached it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D11D27
    // Broiler-Human:        PENDING
    private enum BindMode
    {
        /// <summary>Initialise a lexical binding, a parameter, or a script-level global.</summary>
        Initialise,

        /// <summary>Write the <c>var</c> binding hoisting already created.</summary>
        Var,

        /// <summary>Store through an arbitrary reference, which may be a member expression.</summary>
        Assign,
    }

    /// <summary>Declares every name a binding pattern introduces, before any of it is emitted.</summary>
    /// <remarks>
    /// At script top level nothing is declared here: a declaration there becomes a binding of the
    /// realm's global lexical environment, or a property of the global object when it is a
    /// <c>var</c>, and a slot with the same name would shadow both for the rest of the unit.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BD3639
    // Broiler-Human:        PENDING
    private void DeclarePatternNames(JsPattern pattern, bool constant)
    {
        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            return;
        }

        switch (pattern)
        {
            case JsTargetPattern { Target: JsIdentifier name }:
                scope.Declare(name.Name, constant);
                return;

            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        DeclarePatternNames(element.Target, constant);
                    }
                }

                if (array.Rest is not null)
                {
                    DeclarePatternNames(array.Rest, constant);
                }

                return;

            case JsObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    DeclarePatternNames(property.Value.Target, constant);
                }

                if (literal.Rest is not null)
                {
                    DeclarePatternNames(literal.Rest, constant);
                }

                return;

            default:
                return;
        }
    }

    /// <summary>Replaces <c>undefined</c> on the top of the stack with an initialiser's value.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2AB9C0
    // Broiler-Human:        PENDING
    private void ApplyDefault(JsExpression? initialiser, string inferred)
    {
        if (initialiser is null)
        {
            return;
        }

        var keep = NewLabel();
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadUndefined);
        Emit(JsOpcode.StrictEquals);
        Branch(JsOpcode.JumpIfFalse, keep);
        Emit(JsOpcode.Pop);

        // A DEFAULT IS ONE OF THE PLACES A NAME IS INFERRED. `function f(g = () => {})` gives the
        // arrow the name `g`, and so does `var { g = () => {} } = {}` - the binding the default is
        // for is the name, and it is the leaf's rather than the property's key.
        CompileNamedValue(initialiser, inferred);
        Mark(keep);
    }

    /// <summary>
    /// Destructures the value on top of the stack, which this consumes.
    /// </summary>
    /// <remarks>
    /// <b>One lowering serves declarations and assignments, and the mode is the whole of the
    /// difference.</b> The nesting, the defaults, the elisions and the rest handling are identical
    /// for <c>var [a, [b] = [2], ...c] = x</c> and for <c>[a, [b] = [2], ...c] = x</c>; only what a
    /// leaf does with its value differs. Writing it twice would have meant two copies of the
    /// iterator protocol, and the second copy is where the missing <c>IteratorClose</c> would have
    /// been.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C0E1B1
    // Broiler-Human:        PENDING
    private void BindPattern(JsPattern pattern, BindMode mode)
    {
        switch (pattern)
        {
            case JsTargetPattern leaf:
                BindLeaf(leaf.Target, mode);
                return;

            case JsArrayPattern array:
                BindArrayPattern(array, mode);
                return;

            case JsObjectPattern literal:
                BindObjectPattern(literal, mode);
                return;

            default:
                Emit(JsOpcode.Pop);
                return;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EF721D
    // Broiler-Human:        PENDING
    private void BindLeaf(JsExpression target, BindMode mode)
    {
        if (mode == BindMode.Assign)
        {
            CompileStoreTo(target);
            Emit(JsOpcode.Pop);
            return;
        }

        if (target is not JsIdentifier name)
        {
            Refuse(
                target.Span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "a declaration's pattern binds names, and this is not one");

            Emit(JsOpcode.Pop);
            return;
        }

        if (mode == BindMode.Var)
        {
            StoreName(target.Span, name.Name);
            Emit(JsOpcode.Pop);
            return;
        }

        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            Emit(
                programLexicals.ContainsKey(name.Name)
                    ? JsOpcode.InitialiseGlobalLexical
                    : JsOpcode.StoreGlobal,
                InternedName(name.Name));

            return;
        }

        var slot = scope.Has(name.Name)
            ? scope.SlotOf(name.Name)
            : scope.Declare(name.Name, constant: false);

        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
    }

    /// <summary>
    /// Destructures through the iteration protocol, which is what an array pattern is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The iterator record lives in a slot, and the pattern is guarded by two exception regions
    /// that close it.</b> The record was on the operand stack until [JSC-151], because the stack
    /// nests where a slot chosen at compile time appeared not to - and the appearance was wrong.
    /// The nesting depth of a pattern is known while it is being lowered, so each nesting level
    /// declares a temporary of its own exactly as a computed member target already does, and two
    /// live records never share one.
    /// </para>
    /// <para>
    /// <b>The regions are what close the iterator when a completion abandons the pattern, and their
    /// handlers are entered at the height the pattern began at rather than at zero.</b> A region row
    /// has carried its own entry height since format version 1 and the verifier has always seeded
    /// the handler at that height plus the one value the executor pushes; only the lowering wrote
    /// zero, because until this pattern every region a lowering opened began at a statement
    /// boundary. So a region CAN guard an expression, and the objection this remark used to record
    /// - that a handler is entered at a fixed height and a pattern is applied where the stack is not
    /// empty - was an objection to the constant, not to the mechanism.
    /// </para>
    /// <para>
    /// <b>Two regions and not one, because the completion decides how the iterator is closed.</b>
    /// The language closes under a throw completion QUIETLY - whatever <c>return</c> does is
    /// discarded, because the exception already travelling is the one the program is owed - and
    /// closes under every other abrupt completion loudly, where an error from <c>return</c>
    /// propagates and a <c>return</c> that answers a non-object is itself a <c>TypeError</c>. The
    /// only other completion that reaches a pattern is the forced return a generator's
    /// <c>return()</c> raises at a <c>yield</c> inside it, and a <c>finally</c>-kind region is what
    /// catches that. The catch region is recorded FIRST so that a throw finds it, and the finally
    /// region second so that a forced return, which passes catch regions over, finds that one.
    /// </para>
    /// <para>
    /// <b>An element's target reference is evaluated BEFORE the iterator is stepped</b>, which is
    /// the order the language states and the reverse of the order an assignment uses everywhere
    /// else. <c>[ {}[f()] ] = iterable</c> calls <c>f</c> and never calls <c>next</c>, and a
    /// lowering that stepped first would have called <c>next</c> once before finding out that the
    /// reference throws. It matters most at a rest element, where stepping first drains the whole
    /// iterator before the reference gets a chance to fail.
    /// </para>
    /// <para>
    /// <b>An exhausted iterator supplies <c>undefined</c> rather than ending the pattern</b>, which
    /// is what makes <c>const [a, b] = [1]</c> give <c>b</c> the value <c>undefined</c> and what
    /// lets <c>[a = 1] = []</c> take its default.
    /// </para>
    /// <para>
    /// <b>Letting the executor close what an exception left on the operand stack was the design
    /// refused.</b> It needed no lowering at all: the values between a handler's height and the
    /// live top are exactly what the abandoned expression had built, and an iterator record among
    /// them is identifiable at run time. It was refused for three reasons. It gives the frame that
    /// unwinds WITHOUT a handler nowhere to do the work - which is the case the generator tests
    /// turn on, since a forced return with no <c>finally</c> in the frame leaves through the
    /// executor's own dispatch - and buying that case back needs a filter or a catch in every
    /// frame, which is the shape that killed the process at depth and that the dispatch's own
    /// remark records. It moves a rule of the LANGUAGE into the executor, where the artifact no
    /// longer says what closes and the verifier can no longer check it. And it grows the executor,
    /// which is the one budget a lowering-only change leaves alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3BECA4
    // Broiler-Falsified-If: a pattern leaves an iterator that is not done unclosed on any completion
    // Broiler-Human:        PENDING
    private void BindArrayPattern(JsArrayPattern pattern, BindMode mode)
    {
        var owner = FunctionScope();
        var record = owner.Declare("#iterator" + owner.SlotCount, constant: false);

        Emit(JsOpcode.IterateStart);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, record);

        // THE HEIGHT IS READ HERE AND NOT AT THE FIRST ELEMENT, because this is the height the
        // handlers unwind to: everything the pattern pushes above it is what the abandoned
        // completion was holding, and the executor discards exactly that.
        var height = buffer.Height;
        var guarded = buffer.Code.Count;
        var raised = NewLabel();
        var forced = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, forced, blockDepth, JsFormat.HandlerKind.Finally, height));

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, raised, blockDepth, JsFormat.HandlerKind.Catch, height));

        foreach (var element in pattern.Elements)
        {
            var prepared = PrepareTarget(element?.Target, mode);
            var exhausted = NewLabel();
            var ready = NewLabel();

            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
            Branch(JsOpcode.IterateNext, exhausted);
            Branch(JsOpcode.Jump, ready);
            Mark(exhausted);
            Emit(JsOpcode.LoadUndefined);
            Mark(ready);

            if (element is null)
            {
                // An elision still advances the iterator, which is why the step above happens
                // before this test rather than instead of it.
                Emit(JsOpcode.Pop);
                continue;
            }

            ApplyDefault(element.Default, InferredFrom(element.Target));
            BindPrepared(element.Target, mode, prepared);
        }

        if (pattern.Rest is not null)
        {
            var prepared = PrepareTarget(pattern.Rest, mode);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
            Emit(JsOpcode.IterateRest);
            BindPrepared(pattern.Rest, mode, prepared);
        }

        // `var [] = x` protects nothing, and a region whose start equals its end is one the
        // verifier refuses and nothing could enter.
        ProtectSomething(guarded);

        // The catch region is closed first because `CloseRegion` closes the most recently ADDED,
        // and the executor takes the first region in that order whose range covers the throw.
        buffer.CloseRegion(guarded, buffer.Code.Count);
        buffer.CloseRegion(guarded, buffer.Code.Count);

        // A pattern that stopped before the iterator did owes it a `return`, and one that ran the
        // iterator out does not. The opcode reads the record's own done flag rather than being told
        // which case this is, so a rest element and an exhausted iterator both make it a no-op.
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)0);

        var after = NewLabel();
        Branch(JsOpcode.Jump, after);

        // Both handlers are entered with one value where the pattern's own operands were: the
        // thrown value, or the parked forced return. `Throw` re-raises either as what it was.
        Mark(raised);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)1);
        Emit(JsOpcode.Throw);

        Mark(forced);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)0);
        Emit(JsOpcode.Throw);

        Mark(after);

        // THE STRAIGHT-LINE PASS WALKED THROUGH TWO HANDLERS IT CANNOT REACH, each entered at a
        // height nothing before it establishes, so the model it carries here is not the height the
        // code arriving by the jump actually has.
        buffer.Rejoin(height);
    }

    /// <summary>
    /// What evaluating an assignment pattern's target reference ahead of the value produced.
    /// </summary>
    /// <param name="Holds">Whether anything was evaluated, which only an assignment ever does.</param>
    /// <param name="Base">The slot holding the object the reference reads through.</param>
    /// <param name="Key">The slot holding a computed key, or -1 where the key is not computed.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=04C660
    // Broiler-Human:        PENDING
    private readonly record struct PreparedTarget(bool Holds, int Base, int Key);

    /// <summary>
    /// Evaluates an assignment pattern element's target reference, before the iterator is stepped.
    /// </summary>
    /// <remarks>
    /// <b>A name and a nested pattern evaluate nothing, and the language says so rather than this
    /// being an optimisation.</b> The step that evaluates the target is stated only for a target
    /// that is neither an object nor an array literal, and an identifier reference is resolved
    /// where it is STORED - which is why an assignment to an undeclared name in strict code still
    /// fails at the store and not here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E88645
    // Broiler-Human:        PENDING
    private PreparedTarget PrepareTarget(JsPattern? target, BindMode mode)
    {
        if (!PreparesTarget(target, mode))
        {
            return default;
        }

        switch (((JsTargetPattern)target!).Target)
        {
            case JsPrivateMemberExpression privateAccess:
                return new PreparedTarget(true, Spill(privateAccess.Target), -1);

            // A `super` PROPERTY'S REFERENCE IS THE THIS BINDING AND THE KEY, evaluated here as for
            // any other member target and in that order; the base is the frame's own and needs no
            // slot (JSC-256).
            case JsSuperMemberExpression inherited:
            {
                var owner = FunctionScope();
                var key = owner.Declare("#held" + owner.SlotCount, constant: false);
                EmitSuperReference(inherited);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, key);
                return new PreparedTarget(true, -1, key);
            }

            case JsMemberExpression member when member.Computed is null:
                return new PreparedTarget(true, Spill(member.Target), -1);

            case JsMemberExpression member:
            {
                // THE ORDER OF THESE TWO IS THE LANGUAGE'S. A computed member reference evaluates
                // its base before its key, and both before anything the value needs.
                var basis = Spill(member.Target);
                return new PreparedTarget(true, basis, Spill(member.Computed!));
            }

            default:
                return default;
        }
    }

    /// <summary>
    /// Whether this pattern's target is a reference the language evaluates ahead of the value.
    /// </summary>
    /// <remarks>
    /// It is asked twice for one property of an object assignment pattern - once to decide whether
    /// the pattern's own computed key has to be parked in a temporary, and once by
    /// <see cref="PrepareTarget"/> - and the two must give the same answer, which is why it is a
    /// predicate rather than the same three cases written out again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CCD2EB
    // Broiler-Human:        PENDING
    private static bool PreparesTarget(JsPattern? target, BindMode mode) =>
        mode == BindMode.Assign &&
        target is JsTargetPattern
        {
            Target: JsMemberExpression or JsPrivateMemberExpression or JsSuperMemberExpression,
        };

    /// <summary>Compiles one expression and parks its value in a temporary of the function.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=64DEAE
    // Broiler-Human:        PENDING
    private int Spill(JsExpression expression)
    {
        var owner = FunctionScope();
        var slot = owner.Declare("#held" + owner.SlotCount, constant: false);
        CompileExpression(expression);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
        return slot;
    }

    /// <summary>
    /// Stores the value on top of the stack through a reference already evaluated, and consumes it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0A0FFF
    // Broiler-Human:        PENDING
    private void BindPrepared(JsPattern target, BindMode mode, in PreparedTarget prepared)
    {
        if (!prepared.Holds)
        {
            BindPattern(target, mode);
            return;
        }

        var leaf = (JsTargetPattern)target;
        var owner = FunctionScope();
        var held = owner.Declare("#held" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, held);

        if (leaf.Target is JsSuperMemberExpression)
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, prepared.Key);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
            Emit(JsOpcode.StoreSuperProperty);
            Emit(JsOpcode.Pop);
            return;
        }

        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, prepared.Base);

        switch (leaf.Target)
        {
            case JsPrivateMemberExpression privateAccess:
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.StorePrivate);
                break;

            case JsMemberExpression member when member.Computed is null:
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.SetProperty, InternedName(member.Name));
                break;

            default:
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, prepared.Key);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.SetIndex);
                break;
        }

        Emit(JsOpcode.Pop);
    }

    /// <summary>Destructures by reading properties, which is what an object pattern is.</summary>
    /// <remarks>
    /// <b>Reading properties and NOT iterating</b>, which is the whole difference from an array
    /// pattern: <c>var { 0: first } = [1]</c> reads an index and never asks the Array for an
    /// iterator. The nullish check is explicit rather than left to the first property read, because
    /// <c>var {} = undefined</c> reads nothing and still has to refuse.
    /// <para>
    /// <b>A computed key is converted once, where it is evaluated</b>, by
    /// <see cref="JsOpcode.ToPropertyKey"/>: the read and a rest property's exclusion both use the
    /// converted key. Until 2026-10-04 the value was converted twice where a rest property met a
    /// computed key, and every conversion waited for the read (JSC-258).
    /// </para>
    /// <para>
    /// <b>AN ASSIGNMENT PATTERN PREPARES EVERY TARGET REFERENCE BEFORE IT READS THE PROPERTY THAT
    /// FEEDS IT, and a declaration pattern prepares nothing</b>, which is not a symmetry this
    /// lowering chose. <c>KeyedDestructuringAssignmentEvaluation</c> evaluates the target - and
    /// only a target that is neither an object nor an array literal - BEFORE the <c>GetV</c> that
    /// supplies its value, so <c>({ a: o[k()] } = src)</c> calls <c>k</c> before it reads
    /// <c>src.a</c> and <c>({ ...o[k()] } = src)</c> calls it before <c>CopyDataProperties</c> runs
    /// at all. A declaration has no reference to evaluate: it initialises a binding, and the
    /// binding is found where it is written *(corrected: JSC-160)*.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C252A4
    // Broiler-Human:        PENDING
    private void BindObjectPattern(JsObjectPattern pattern, BindMode mode)
    {
        var named = pattern.Properties.Count != 0 && pattern.Properties[0].Computed is null
            ? pattern.Properties[0].Key
            : string.Empty;

        Emit(JsOpcode.RequireCoercible, InternedName(named));
        var excluded = new System.Collections.Generic.List<int>();

        foreach (var property in pattern.Properties)
        {
            // A COMPUTED KEY IS PARKED BEFORE THE TARGET IS PREPARED, and both happen before the
            // source is read. `PropertyName` is step 1 of the property's own evaluation and the
            // target is step 1 of the element's, so the two run in this order and the `GetV`
            // between them runs after both. Parking is what makes that possible at all: the key
            // used to be pushed on top of the source object, which is where the read needs it and
            // nowhere near where a reference evaluated before it could sit.
            //
            // ONLY WHERE THERE IS A REFERENCE TO PUT BETWEEN THEM. A name and a nested pattern
            // evaluate nothing, so for them the key still goes straight onto the stack and this
            // costs no slot - which is every shorthand property and every plain `{ a: x }`.
            var held = -1;

            if (PreparesTarget(property.Value.Target, mode) && property.Computed is not null)
            {
                // THE KEY IS CONVERTED WHERE IT IS EVALUATED: `ComputedPropertyName` performs
                // `ToPropertyKey` as its own last step, before the target is evaluated. Until
                // 2026-10-04 the conversion waited for the read (JSC-258).
                var owner = FunctionScope();
                held = owner.Declare("#held" + owner.SlotCount, constant: false);
                CompileExpression(property.Computed);
                Emit(JsOpcode.ToPropertyKey);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, held);

                if (pattern.Rest is not null)
                {
                    excluded.Add(held);
                }
            }

            var prepared = PrepareTarget(property.Value.Target, mode);
            Emit(JsOpcode.Duplicate);

            if (property.Computed is null)
            {
                Emit(JsOpcode.GetProperty, InternedName(property.Key));
            }
            else if (held >= 0)
            {
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.GetIndex);
            }
            else
            {
                CompileExpression(property.Computed);
                Emit(JsOpcode.ToPropertyKey);

                if (pattern.Rest is not null)
                {
                    // A REST PROPERTY EXCLUDES THE KEYS THE PATTERN NAMED, and a computed key is
                    // only a key once. Re-evaluating the expression to build the exclusion list
                    // would run its side effects twice, which is observable with any key whose
                    // expression is a call.
                    var owner = FunctionScope();
                    var slot = owner.Declare("#key" + owner.SlotCount, constant: false);
                    Emit(JsOpcode.Duplicate);
                    EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                    excluded.Add(slot);
                }

                Emit(JsOpcode.GetIndex);
            }

            ApplyDefault(property.Value.Default, InferredFrom(property.Value.Target));
            BindPrepared(property.Value.Target, mode, prepared);
        }

        if (pattern.Rest is null)
        {
            Emit(JsOpcode.Pop);
            return;
        }

        // THE REST TARGET IS PREPARED BEFORE THE REST OBJECT EXISTS, because
        // `RestDestructuringAssignmentEvaluation` evaluates it first and only then performs
        // `CopyDataProperties`. A source with a getter on it makes the difference visible without
        // any error at all: the getter must run after `k` in `({ ...o[k()] } = src)`.
        var restTarget = PrepareTarget(pattern.Rest, mode);

        Emit(JsOpcode.NewObject);
        Emit(JsOpcode.Pick, (byte)1);
        Emit(JsOpcode.SpreadObject);

        foreach (var property in pattern.Properties)
        {
            if (property.Computed is not null)
            {
                continue;
            }

            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.DeleteProperty, InternedName(property.Key));
            Emit(JsOpcode.Pop);
        }

        foreach (var slot in excluded)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
            Emit(JsOpcode.DeleteIndex);
            Emit(JsOpcode.Pop);
        }

        BindPrepared(pattern.Rest, mode, restTarget);
        Emit(JsOpcode.Pop);
    }
}
