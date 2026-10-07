// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           0
// Human-reviewed:   0/19
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=909A84
    // Broiler-Human:        PENDING
    private void CompileStoreTo(JsExpression target)
    {
        switch (target)
        {
            case JsIdentifier name:
                StoreName(name.Span, name.Name);
                break;

            case JsPrivateMemberExpression privateAccess:
            {
                var owner = FunctionScope();
                var kept = owner.Declare("#target" + owner.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
                CompileExpression(privateAccess.Target);
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
                Emit(JsOpcode.StorePrivate);
                break;
            }

            case JsMemberExpression member when member.Computed is null:
            {
                var function = FunctionScope();
                var slot = function.Declare("#target" + function.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                CompileExpression(member.Target);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
                Emit(JsOpcode.SetProperty, InternedName(member.Name));
                break;
            }

            // A `super` PROPERTY IS A REFERENCE TOO, as the target of `for (super.x of xs)` and
            // of a pattern's leaf (JSC-256): the this binding is read and the key evaluated, then
            // the value is written as `super.x = v` writes it.
            case JsSuperMemberExpression inherited:
            {
                var function = FunctionScope();
                var slot = function.Declare("#target" + function.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                EmitSuperReference(inherited);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
                Emit(JsOpcode.StoreSuperProperty);
                break;
            }

            case JsMemberExpression member:
            {
                var function = FunctionScope();
                var slot = function.Declare("#target" + function.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                CompileExpression(member.Target);
                CompileExpression(member.Computed!);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
                Emit(JsOpcode.SetIndex);
                break;
            }

            // A CALL AS A `for … in` OR `for … of` HEAD IN NON-STRICT CODE RUNS THE CALL FOR EACH
            // VALUE AND THROWS BEFORE THE BODY (JSP-7, JSC-239); the value it would have written is
            // dropped first, and the instruction that throws stands in for it.
            case JsCallExpression called when !strict:
                Emit(JsOpcode.Pop);
                EmitCallTargetThrow(called);
                break;

            default:
                Refuse(
                    target.Span,
                    SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                    "the left-hand side of an assignment is not a reference");

                break;
        }
    }

    // ---- names ---------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A3878
    // Broiler-Human:        PENDING
    private void LoadName(SliceSourceSpan span, string name)
    {
        _ = span;

        if (Shadowable(name, out var limit))
        {
            EmitDynamicName(name, limit, wantsBase: false, orUndefined: false);
            return;
        }

        EmitStaticLoad(name, orUndefined: false);
    }

    /// <summary>Pushes a name the way the enclosing scopes alone would resolve it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7426D8
    // Broiler-Human:        PENDING
    private void EmitStaticLoad(string name, bool orUndefined)
    {
        if (TryResolve(name, out var hops, out var slot, out _))
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
            return;
        }

        // AN IMPORT IS CONSULTED AFTER THE SCOPES AND BEFORE THE GLOBAL, and that order is the
        // shadowing rule. A nearer declaration of the same name wins - a function may declare a
        // local called `a` while the module imports an `a` - and a name that is neither is still a
        // global, because a module's code sees the realm's globals like any other code.
        if (module is { } importing && importing.Imports.TryGetValue(name, out var entry))
        {
            Emit(JsOpcode.LoadImport, (ushort)entry);
            return;
        }

        // A FREE NAME OF EVAL CODE IS ITS CALLER'S, and the caller's scope ends in the global one
        // rather than starting there (JSD-0026 section 5).
        if (TryEvalHops(out var evalHops))
        {
            EmitEvalName(
                orUndefined ? JsOpcode.LoadEvalNameOrUndefined : JsOpcode.LoadEvalName, evalHops, name);

            return;
        }

        DeclareSurfaceOf(name);

        Emit(
            orUndefined ? JsOpcode.LoadGlobalOrUndefined : JsOpcode.LoadGlobal,
            InternedName(name));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EDF5A4
    // Broiler-Human:        PENDING
    private void StoreName(SliceSourceSpan span, string name)
    {
        StoreNameAfterTheValue(span, name);
    }

    /// <summary>
    /// Lowers an assignment, or a read and then an assignment, through a name a <c>with</c> body
    /// can shadow, resolving the name ONCE, before anything else is evaluated.
    /// </summary>
    /// <param name="span">Where the assignment is, for the static store's diagnostics.</param>
    /// <param name="name">The name assigned.</param>
    /// <param name="read">Whether the reference's value is read first, as a compound assignment
    /// or an update reads it.</param>
    /// <param name="produce">Emits the new value, with the old one on top when
    /// <paramref name="read"/>; it is emitted once, between the read and the write.</param>
    /// <returns>False when no <c>with</c> can shadow the name, and nothing was emitted.</returns>
    /// <remarks>
    /// <b>The language resolves the reference first and writes through it last</b>: the binding
    /// the write reaches is the one the name meant when the assignment began, even when a getter
    /// on the <c>with</c> object deleted it, or the right-hand side added one to a nearer object.
    /// Resolving again at the write - as this lowering did until 2026-10-03 - wrote the enclosing
    /// variable instead (test262's S11.13.1_A5/A6 and S11.13.2_A5/A6). The object the search
    /// answered stays on the stack under the value, and <see cref="JsOpcode.SetObjectBinding"/>
    /// writes it as an object environment record does, which is a ReferenceError in strict code
    /// when the property is gone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6ACC9D
    // Broiler-Human:        PENDING
    private bool TryEmitShadowedReference(SliceSourceSpan span, string name, bool read, System.Action produce)
    {
        if (!Shadowable(name, out var limit))
        {
            return false;
        }

        var live = buffer.Height;
        var key = InternedName(name);
        var staticWrite = NewLabel();
        var done = NewLabel();

        EmitResolve(limit, key);

        if (read)
        {
            var staticRead = NewLabel();
            var readDone = NewLabel();

            Emit(JsOpcode.Duplicate);
            Branch(JsOpcode.JumpIfFalse, staticRead);
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetObjectBinding, key);
            Branch(JsOpcode.Jump, readDone);

            Mark(staticRead);
            buffer.Rejoin(live + 1);
            EmitStaticLoad(name, orUndefined: false);

            Mark(readDone);
            buffer.Rejoin(live + 2);
        }

        produce();

        // [reference, value]: the value goes under the reference so the test can consume a copy.
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Duplicate);
        Branch(JsOpcode.JumpIfFalse, staticWrite);
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.SetObjectBinding, key);
        Branch(JsOpcode.Jump, done);

        Mark(staticWrite);
        buffer.Rejoin(live + 2);
        Emit(JsOpcode.Pop);
        EmitStaticStore(span, name);

        Mark(done);
        buffer.Rejoin(live + 1);
        return true;
    }

    /// <summary>Writes the value on the stack to a name, resolving it now.</summary>
    /// <remarks>
    /// For the writes that have no right-hand side to order against - a declaration's initialiser
    /// and a loop variable - where resolving at the write is resolving first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8E88CB
    // Broiler-Human:        PENDING
    private void StoreNameAfterTheValue(SliceSourceSpan span, string name)
    {
        // A WRITE ASKS THE SAME OBJECTS A READ ASKS, AND THE TWO ANSWERS DIFFER. `with (o) { x = 1 }`
        // sets `o.x` when the object has the name and reaches the enclosing binding when it does
        // not, so the write is the read's shape with `SetProperty` where `GetProperty` was. Both
        // branches leave the assigned value on the stack, because an assignment is an expression.
        if (Shadowable(name, out var limit))
        {
            // The value is already on the stack and both branches give it back, so the height at
            // the join is the height here - which the straight-line model cannot see, because the
            // path it walks is only one of the two.
            var live = buffer.Height;
            var key = InternedName(name);
            var enclosing = NewLabel();
            var done = NewLabel();

            EmitResolve(limit, key);
            Emit(JsOpcode.Duplicate);
            Branch(JsOpcode.JumpIfFalse, enclosing);

            // The value is under the base and `SetProperty` wants it above, so one exchange turns
            // [value, base] into [base, value] and the instruction gives the value back.
            Emit(JsOpcode.Swap);
            Emit(JsOpcode.SetProperty, key);
            Branch(JsOpcode.Jump, done);

            Mark(enclosing);
            Emit(JsOpcode.Pop);
            EmitStaticStore(span, name);
            Mark(done);
            buffer.Rejoin(live);
            return;
        }

        EmitStaticStore(span, name);
    }

    /// <summary>Writes a name the way the enclosing scopes alone would resolve it.</summary>
    /// <remarks>
    /// <b>A WRITE TO AN IMMUTABLE BINDING IS AN INSTRUCTION AND NOT A REFUSAL, inside a
    /// <c>with</c> body and everywhere else.</b> This front end refused it at compile time until
    /// 2026-09-05, including in a <c>with</c> body where the answer was defended as consistency
    /// with the other occurrences; the consistency was real and the rule it was consistent with was
    /// wrong. Every path through here now emits <see cref="JsOpcode.ThrowImmutable"/>, so a
    /// <c>with</c> around the assignment changes which branch runs rather than what the rule is —
    /// which is what that remark always wanted and did not have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=52AA67
    // Broiler-Human:        PENDING
    private void EmitStaticStore(SliceSourceSpan span, string name)
    {
        if (TryResolve(name, out var hops, out var slot, out var constant))
        {
            // A WRITE TO A CONSTANT COMPILES AND THROWS, and until 2026-09-05 it was refused at the
            // front end instead. The language makes it a run-time `TypeError`, which is why
            // `assert.throws(TypeError, function () { x = 1; })` is a program every engine runs -
            // and refusing it said this manifest does not admit an assignment, when what it does
            // not admit is the assignment SUCCEEDING.
            if (constant)
            {
                Emit(JsOpcode.Duplicate);
                Emit(JsOpcode.ThrowImmutable, InternedName(name));
                return;
            }

            // A NAMED FUNCTION EXPRESSION'S OWN NAME IS IMMUTABLE AND NOT STRICT, the one binding
            // of that kind the language makes: `var f = function g() { g = 1; return g; }` answers
            // the function in sloppy code, because the write is ignored, and throws a `TypeError`
            // in strict code. The slot was written until VM-FIX-D, which made the first answer 1.
            // The value stays on the stack either way, because an assignment is an expression.
            if (ResolvesToFunctionName(hops))
            {
                if (strict)
                {
                    Emit(JsOpcode.Duplicate);
                    Emit(JsOpcode.ThrowImmutable, InternedName(name));
                }

                return;
            }

            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.StoreScoped, (byte)hops, slot);
            return;
        }

        // AN IMPORTED BINDING IS IMMUTABLE, and a write to one fails the same way a write to a
        // constant does: at the moment it runs, with a `TypeError`. It is not an early error, and
        // the conformance suite is emphatic about it - a whole family of its module tests wraps the
        // assignment in `assert.throws(TypeError, ...)`, which needs the program to compile.
        if (module is { } importing && importing.Imports.ContainsKey(name))
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.ThrowImmutable, InternedName(name));
            return;
        }

        if (TryEvalHops(out var evalHops))
        {
            Emit(JsOpcode.Duplicate);
            EmitEvalName(JsOpcode.StoreEvalName, evalHops, name);
            return;
        }

        Emit(JsOpcode.Duplicate);
        DeclareSurfaceOf(name);
        Emit(JsOpcode.StoreGlobal, InternedName(name));
    }

    /// <summary>
    /// Whether an enclosing <c>with</c> could bind <paramref name="name"/>, and over how many
    /// records the executor must look for one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the whole of what admitting <c>with</c> costs the binding model, and it costs it
    /// in exactly the names it has to.</b> The walk is the ordinary resolution walk with one extra
    /// question asked at each step: is this record an object one. A name that reaches its binding
    /// without passing an object record is lowered to the <c>(depth, slot)</c> pair it always was
    /// and pays nothing; a name that passes one is lowered to a search, a branch and then that same
    /// pair on the branch the search did not take.
    /// </para>
    /// <para>
    /// <b>The bound is the OUTERMOST object record before the binding, and not the binding.</b>
    /// Searching further would let an outer <c>with</c> shadow a declaration that already shadows
    /// it; searching less far would miss one. A name that resolves to nothing is a global, and then
    /// every record on the chain is between the reference and it.
    /// </para>
    /// <para>
    /// <b>It crosses function boundaries, because the scope chain does.</b> A closure created inside
    /// a <c>with</c> body captures the object record, so a free name in its body has to ask that
    /// object when the closure is CALLED — long after the <c>with</c> statement finished. The
    /// compile-time chain spans functions exactly as the run-time one does, which is what makes the
    /// hop count answerable at all.
    /// </para>
    /// <para>
    /// <b>A chain deeper than the format's scope-depth ceiling is clamped, and the clamp is safe in
    /// one direction only.</b> Clamping searches FEWER records, so a name falls through to the
    /// static address the language's own rules give it; the opposite clamp would have let a record
    /// past the binding answer. The same ceiling already bounds a hop count, so a chain this deep
    /// has a wrong <c>LoadScoped</c> in it before it has a wrong search.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=B1DA0C
    // Broiler-Falsified-If: the bound reaches a record at or beyond the binding this name resolves to
    // Broiler-Human:        PENDING
    private bool Shadowable(string name, out int limit) => Shadowable(name, out limit, out _);

    /// <summary>
    /// <see cref="Shadowable(string, out int)"/>, also answering whether the search can pass a
    /// function's eval variables, whose answer is never a call's receiver.
    /// </summary>
    /// <remarks>
    /// <b>A sloppy function whose own code may call <c>eval</c> directly is a search point too</b>
    /// (JSeal V15), for every name it does not bind itself: an evaluation may have introduced the name
    /// into its variable environment, and the executor searches the function's record for such a
    /// binding exactly as it searches a <c>with</c> object. A name the function binds is not searched
    /// there, because an evaluation never introduces one - its declaration writes the existing slot.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=7112BF
    // Broiler-Falsified-If: the bound reaches a record at or beyond the binding this name resolves to, or a function that may hold eval variables inside the bound is not reported
    // Broiler-Human:        PENDING
    private bool Shadowable(string name, out int limit, out bool evalVariables)
    {
        limit = 0;
        evalVariables = false;
        var outermost = -1;
        var hops = 0;
        var current = scope;

        while (current is not null)
        {
            if (current.Kind == ScopeKind.With)
            {
                outermost = hops;
            }
            else if (current.Has(name))
            {
                break;
            }
            else if (current.EvalVariables)
            {
                outermost = hops;
                evalVariables = true;
            }

            hops++;
            current = current.Parent;
        }

        if (outermost < 0)
        {
            return false;
        }

        limit = System.Math.Min(outermost + 1, (int)JsFormat.CeilingScopeDepth);
        return true;
    }

    /// <summary>
    /// Lowers one dynamically resolved name: ask the objects, and fall back to the static address.
    /// </summary>
    /// <param name="name">The name being resolved.</param>
    /// <param name="limit">How many records the search covers.</param>
    /// <param name="wantsBase">
    /// Whether the receiver is wanted above the value, which is what a CALL through such a name
    /// needs: <c>with (o) { f() }</c> calls <c>o.f</c> with <c>o</c> as its <c>this</c>, and the
    /// object the search answered with is that receiver.
    /// </param>
    /// <param name="orUndefined">
    /// Whether an absent global answers <c>undefined</c> rather than throwing, which is what
    /// <c>typeof</c> needs and nothing else does.
    /// </param>
    /// <remarks>
    /// <b>A name inside a <c>with</c> body costs a search, a duplicate, a branch and a property
    /// read, and it costs that EVERY TIME it is mentioned.</b> Nothing is cached and nothing can be:
    /// the object may gain or lose the property between two reads in the same body, and the
    /// language says the second read sees that. That is the price of the construct rather than a
    /// shortcoming of this lowering, and it is why nothing outside a <c>with</c> body pays any of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=97592B
    // Broiler-Human:        PENDING
    private void EmitDynamicName(string name, int limit, bool wantsBase, bool orUndefined)
    {
        var live = buffer.Height;
        var key = InternedName(name);
        var enclosing = NewLabel();
        var done = NewLabel();

        EmitResolve(limit, key);
        Emit(JsOpcode.Duplicate);
        Branch(JsOpcode.JumpIfFalse, enclosing);

        if (wantsBase)
        {
            Emit(JsOpcode.Duplicate);
        }

        // GetBindingValue OF AN OBJECT ENVIRONMENT RECORD, which asks for the property again: one
        // a `Symbol.unscopables` getter removed since the search is a ReferenceError in strict code.
        Emit(JsOpcode.GetObjectBinding, key);

        if (wantsBase)
        {
            // The receiver is under the callee and the calling convention wants it above, exactly
            // as it does for `o.f()`.
            Emit(JsOpcode.Swap);

            // AND IT IS `undefined` WHEN A FUNCTION'S EVAL VARIABLES ANSWERED, an object no guest code
            // may hold (JSeal V15). Only a search that can pass one pays the instruction.
            if (Shadowable(name, out _, out var evalVariables) && evalVariables)
            {
                Emit(JsOpcode.WithBaseObject);
            }
        }

        Branch(JsOpcode.Jump, done);

        Mark(enclosing);
        Emit(JsOpcode.Pop);

        // A CALLEE EVAL CODE DOES NOT BIND IS RESOLVED WITH ITS BASE, because the caller's own `with`
        // records lie past the boundary and the receiver is whichever of them answered.
        if (wantsBase && !Resolvable(name) && TryEvalHops(out var evalHops))
        {
            EmitEvalName(JsOpcode.LoadEvalNameWithBase, evalHops, name);
        }
        else
        {
            EmitStaticLoad(name, orUndefined);

            if (wantsBase)
            {
                Emit(JsOpcode.LoadUndefined);
            }
        }

        Mark(done);

        // One value arrives, or two when the receiver was wanted. The straight-line model walked
        // both branches in sequence and would otherwise carry their sum onward.
        buffer.Rejoin(live + (wantsBase ? 2 : 1));
    }

    /// <summary>Emits the search itself, with its bound and the name it is looking for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C4992C
    // Broiler-Human:        PENDING
    private void EmitResolve(int limit, ushort key) =>
        EmitScoped(JsOpcode.ResolveName, (byte)limit, key);

    /// <summary>
    /// Records that this artifact reaches an optional surface, when the free name belongs to one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A free name is the only evidence there is for a surface made of globals.</b> A construct
    /// the manifest refuses — <c>eval</c> as a direct call, a module declaration — is refused at the
    /// parse and never reaches an artifact at all. A typed array constructor is a name, and a
    /// program that reads it is, byte for byte, a program that reads a name. So the lowering
    /// records the surface here, in the one place a free name is resolved to a global, and
    /// <see cref="Assemble"/> writes what it recorded.
    /// </para>
    /// <para>
    /// <b>A name that resolves to a binding declares nothing</b>, which is why this is below the
    /// resolution rather than beside the parse. A program with its own <c>var Uint8Array</c> is a
    /// program about its own variable and reaches no surface at all.
    /// </para>
    /// <para>
    /// <b>And a <c>typeof</c> declares nothing either</b>, because it does not come through here:
    /// the lowering emits <c>LoadGlobalOrUndefined</c> for it, which is a different instruction
    /// with a different answer for an absent name. That is what keeps
    /// <c>typeof Uint8Array === "undefined"</c> — the shape a machine-generated program uses to
    /// find out whether it may go on — a question this profile answers rather than an artifact it
    /// refuses.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FA0293
    // Broiler-Human:        PENDING
    private void DeclareSurfaceOf(string name)
    {
        if (JsSurfaces.TryOwner(name, out var manifestId))
        {
            surfaces.Add(manifestId);
        }

        // A BIGINT TYPED ARRAY BELONGS TO TWO SURFACES (JSeal B07): its constructor is binary and
        // its elements are BigInts, so naming it declares the BigInt surface beside the binary one
        // that TryOwner answered.
        if (System.Array.IndexOf(JsSurfaces.BigIntGlobals, name) >= 0)
        {
            surfaces.Add(JsSurfaces.BigInt);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2CC181
    // Broiler-Human:        PENDING
    private bool Resolvable(string name) => TryResolve(name, out _, out _, out _);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6F5338
    // Broiler-Human:        PENDING
    private bool TryResolve(string name, out int hops, out int slot, out bool constant)
    {
        hops = 0;
        var current = scope;

        while (current is not null)
        {
            if (current.TryGet(name, out slot, out constant))
            {
                return true;
            }

            hops++;
            current = current.Parent;
        }

        slot = 0;
        constant = false;
        return false;
    }

    /// <summary>
    /// Whether the record <paramref name="hops"/> records out is a named function expression's own.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B076A6
    // Broiler-Human:        PENDING
    private bool ResolvesToFunctionName(int hops)
    {
        var current = scope;

        for (var index = 0; index < hops && current is not null; index++)
        {
            current = current.Parent;
        }

        return current is { IsFunctionName: true };
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=719EFB
    // Broiler-Human:        PENDING
    private Scope FunctionScope()
    {
        var current = scope;

        // A `with` scope is walked through exactly as a block is: a temporary the lowering needs
        // belongs to the function, and a `with` record has nowhere to put one. A body's own
        // variable environment is walked through too, because a temporary is addressed at the
        // unit's own record, `blockDepth` hops out (JSeal V15-finish).
        while (current.Kind is ScopeKind.Block or ScopeKind.With or ScopeKind.Body &&
            current.Parent is not null)
        {
            current = current.Parent;
        }

        return current;
    }

    /// <summary>
    /// The nearest variable environment: a function body's own record when it has one, otherwise
    /// what <see cref="FunctionScope"/> answers.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=503227
    // Broiler-Human:        PENDING
    private Scope VariableScope()
    {
        var current = scope;

        while (current.Kind is ScopeKind.Block or ScopeKind.With && current.Parent is not null)
        {
            current = current.Parent;
        }

        return current;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BE4181
    // Broiler-Human:        PENDING
    private int Hops(Scope target)
    {
        var hops = 0;
        var current = scope;

        while (current is not null && !ReferenceEquals(current, target))
        {
            hops++;
            current = current.Parent;
        }

        return hops;
    }
}
