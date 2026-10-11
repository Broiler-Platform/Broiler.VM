// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           6
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  5/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>Where one of the caller's names lives, as an eval view resolved it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=99F935
    // Broiler-Human:        PENDING
    private readonly struct JsEvalBinding(JsEnvironment? record, int slot, bool immutable, JsObject? holder, bool functionName = false, JsProgram? importer = null)
    {
        /// <summary>The declarative record the name is a slot of, or null.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=240892
        // Broiler-Human:        PENDING
        internal JsEnvironment? Record { get; } = record;

        /// <summary>The slot, when <see cref="Record"/> is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=897E94
        // Broiler-Human:        PENDING
        internal int Slot { get; } = slot;

        /// <summary>Whether a write to the slot is a <c>TypeError</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=936BE6
        // Broiler-Human:        PENDING
        internal bool Immutable { get; } = immutable;

        /// <summary>
        /// Whether the slot is a named function expression's own name, which sloppy code's write
        /// leaves alone rather than throwing (VM-FIX-D).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C09ECA
        // Broiler-Human:        PENDING
        internal bool FunctionName { get; } = functionName;

        /// <summary>The <c>with</c> object that has the name, or null.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3A0FD4
        // Broiler-Human:        PENDING
        internal JsObject? Object { get; } = holder;

        /// <summary>
        /// The program whose import table <see cref="Slot"/> indexes, when the name is one of a
        /// module's imports (JSeal V15-module), or null.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3D2B2D
        // Broiler-Human:        PENDING
        internal JsProgram? Importer { get; } = importer;
    }

    /// <summary>
    /// Resolves one of the caller's names outward from an eval boundary, through the view it was
    /// entered with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The map and the chain are walked in step, one row and one record at a time</b>, starting at
    /// the site's innermost row and the record the evaluation was entered under. A declarative row
    /// answers with its slot of the record, a <c>with</c> row asks its record's object -
    /// <c>HasProperty</c>, then <c>Symbol.unscopables</c>, as <see cref="ResolveName"/> does - and a
    /// root ends the walk: a program row in the realm's global scope, which the caller answers with
    /// a binding of neither kind, and an eval row, after its own names, by continuing through the
    /// view ITS boundary record was entered with. Nothing is cached.
    /// </para>
    /// <para>
    /// <b>A disagreement between the map and the chain is an internal defect</b>: a row the map does
    /// not have, a <c>with</c> row over a record with no object, a slot the record does not have. The
    /// verifier admitted the map against the code that pushes the records, so a disagreement means
    /// one of them was not what the verifier read, and the walk stops rather than reading elsewhere.
    /// </para>
    /// <para>
    /// <b>Each record walked is charged one unit</b>, as <see cref="ResolveName"/> charges, and the
    /// walk is bounded by the chain, which the scope-depth ceiling bounds per unit.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=412316
    // Broiler-Falsified-If: a name is answered from a slot the view's site row chain does not name, or from a record past a declarative binding of the same name
    // Broiler-Human:        PENDING
    private JsEvalBinding ResolveEvalName(JsEnvironment boundary, string name)
    {
        var view = boundary.EvalView!;

        // A GLOBAL EVALUATION SEES THE GLOBAL SCOPE AND NOTHING ELSE (JSeal V15).
        if (view.Program is null)
        {
            Charge(1);
            return default;
        }

        var record = boundary.Parent;
        var map = view.Program.EvalMap;
        var row = view.Site!.Scope;

        while (true)
        {
            Charge(1);

            if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
            {
                throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
            }

            var shape = map.Shapes[row];

            if (shape.Kind == JsFormat.EvalScopeKind.With)
            {
                if (record.Binding is not { } holder)
                {
                    throw new JsAbort(
                        JsAbortKind.InternalDefect, "an eval view's with row named a declarative record");
                }

                if (HasProperty(holder, name) && !Unscopable(holder, name))
                {
                    return new JsEvalBinding(null, 0, false, holder);
                }
            }
            else if (shape.TryFind(name, out var slot, out var immutable, out var functionName))
            {
                if (record.Binding is not null || slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                return new JsEvalBinding(record, slot, immutable, null, functionName);
            }
            else if (shape.TryFindImport(name, out var entry))
            {
                // A MODULE'S IMPORT IS READ THROUGH THE CALLER'S IMPORT TABLE, whose entry the
                // verifier bounded (JSeal V15-module); the record is the module's own and holds
                // no slot for it.
                if (record.Binding is not null)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval view's module row named an object record");
                }

                return new JsEvalBinding(null, entry, true, null, false, view.Program);
            }
            else if (shape.Kind is (JsFormat.EvalScopeKind.Function or
                    JsFormat.EvalScopeKind.FunctionBody) &&
                record.EvalVariables is { } introduced &&
                introduced.HasOwnProperty(name))
            {
                // A NAME AN EVALUATION INTRODUCED INTO THE FUNCTION is found after the function's own
                // slots and before anything outside it, which is where its variable environment is
                // (JSeal V15).
                return new JsEvalBinding(null, 0, false, introduced);
            }

            // A MODULE'S RECORD ENDS ITS CHAIN AS A SCRIPT BODY'S DOES, in the realm's global scope
            // (JSeal V15-module).
            if (shape.Kind is JsFormat.EvalScopeKind.Program or JsFormat.EvalScopeKind.Module)
            {
                return default;
            }

            if (shape.Kind == JsFormat.EvalScopeKind.Eval)
            {
                if (record.EvalView is not { } outer)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                }

                if (outer.Program is null)
                {
                    return default;
                }

                view = outer;
                map = outer.Program.EvalMap;
                row = outer.Site!.Scope;
                record = record.Parent;
                continue;
            }

            row = shape.Parent;
            record = record.Parent;
        }
    }

    /// <summary>
    /// Whether a class around an eval boundary declares a private name, as the map's declarative
    /// rows answer it.
    /// </summary>
    /// <remarks>
    /// The walk is <see cref="ResolveEvalName"/>'s, row by row and record by record, but a
    /// <c>with</c> row is passed over without asking its object and a function's evaluation-introduced
    /// names are not consulted: only a class declares a private name, and the check this answers is
    /// an early error, which runs no guest code and never shows guest code the profile's internal
    /// <c>##</c> spelling (JSeal V15-finish).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F7224D
    // Broiler-Falsified-If: it asks a with object or runs guest code, or answers true for a name no declarative row on the site's row chain names
    // Broiler-Human:        PENDING
    private bool EvalPrivateNameDeclared(JsEnvironment boundary, string name)
    {
        var view = boundary.EvalView!;

        if (view.Program is null)
        {
            Charge(1);
            return false;
        }

        var record = boundary.Parent;
        var map = view.Program.EvalMap;
        var row = view.Site!.Scope;

        while (true)
        {
            Charge(1);

            if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
            {
                throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
            }

            var shape = map.Shapes[row];

            if (shape.Kind != JsFormat.EvalScopeKind.With &&
                shape.TryFind(name, out var slot, out _, out _))
            {
                if (record.Binding is not null || slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                return true;
            }

            if (shape.Kind is JsFormat.EvalScopeKind.Program or JsFormat.EvalScopeKind.Module)
            {
                return false;
            }

            if (shape.Kind == JsFormat.EvalScopeKind.Eval)
            {
                if (record.EvalView is not { } outer)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                }

                if (outer.Program is null)
                {
                    return false;
                }

                map = outer.Program.EvalMap;
                row = outer.Site!.Scope;
                record = record.Parent;
                continue;
            }

            row = shape.Parent;
            record = record.Parent;
        }
    }

    /// <summary>Reads a resolved binding, or the realm's global scope when it resolved to neither kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=940B72
    // Broiler-Human:        PENDING
    private JsValue ReadEvalBinding(JsEvalBinding binding, string name, bool orUndefined)
    {
        if (binding.Record is { } record)
        {
            // A BINDING IN ITS DEAD ZONE THROWS FOR `typeof` TOO, which is the one respect in which
            // `typeof` of a declared name differs from `typeof` of one nobody declared.
            if (record.Slots[binding.Slot].IsEmpty)
            {
                ThrowReferenceError("Cannot access a binding before initialisation");
            }

            return record.Slots[binding.Slot];
        }

        // AN IMPORT IS READ WHERE IT LIVES, every time: a live binding, in its dead zone until the
        // exporting module initialises it (JSeal V15-module).
        if (binding.Importer is { } importer)
        {
            return JsModuleNamespace.Read(importer.ImportBindings[binding.Slot], Graph(importer), this);
        }

        if (binding.Object is { } holder)
        {
            return GetProperty(JsValue.Object(holder), name);
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            return ReadLexical(name, bound);
        }

        if (!HasProperty(Realm.GlobalObject, name))
        {
            return orUndefined ? JsValue.Undefined : ThrowReferenceError(name + " is not defined");
        }

        return GetProperty(JsValue.Object(Realm.GlobalObject), name);
    }

    /// <summary>Reads one of the caller's names from eval code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6D2647
    // Broiler-Human:        PENDING
    private JsValue ReadEvalName(JsEnvironment boundary, string name, bool orUndefined) =>
        ReadEvalBinding(ResolveEvalName(boundary, name), name, orUndefined);

    /// <summary>Writes one of the caller's names from eval code.</summary>
    /// <remarks>
    /// <b>The rules are the caller's own instructions' rules</b>: a slot in its dead zone is a
    /// <c>ReferenceError</c> and an immutable one a <c>TypeError</c>, as <c>StoreScoped</c> and
    /// <c>ThrowImmutable</c> answer; a <c>with</c> object's property is set on the object; and a name
    /// nothing binds is <c>StoreGlobal</c>'s question, asked with the evaluated program's strictness
    /// - so strict eval code cannot create a global by assignment and sloppy eval code can.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=290144
    // Broiler-Falsified-If: a write through the map reaches a slot the map does not name, or succeeds on an immutable or uninitialised binding
    // Broiler-Human:        PENDING
    private void WriteEvalName(JsEnvironment boundary, string name, JsValue value, bool strict)
    {
        var binding = ResolveEvalName(boundary, name);

        if (binding.Record is { } record)
        {
            if (record.Slots[binding.Slot].IsEmpty)
            {
                ThrowReferenceError("Cannot access a binding before initialisation");
            }

            if (binding.Immutable)
            {
                // A NAMED FUNCTION EXPRESSION'S OWN NAME IS IMMUTABLE AND NOT STRICT: sloppy eval
                // code's `g = 1` is ignored, as the lowering ignores the same store outside eval,
                // and strict eval code's throws. Until VM-FIX-D's review the slot was written.
                if (binding.FunctionName && !strict)
                {
                    return;
                }

                throw Error("TypeError", "assignment to constant variable " + name);
            }

            record.Slots[binding.Slot] = value;
            return;
        }

        // AN IMPORT IS AN IMMUTABLE BINDING, and assigning to one is the TypeError it is outside
        // eval code (JSeal V15-module).
        if (binding.Importer is not null)
        {
            throw Error("TypeError", "assignment to constant variable " + name);
        }

        if (binding.Object is { } holder)
        {
            SetProperty(JsValue.Object(holder), name, value, strict);
            return;
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            if (!bound.Initialised)
            {
                ThrowReferenceError("cannot access " + name + " before its declaration");
            }

            if (!bound.Mutable)
            {
                throw Error("TypeError", "assignment to constant variable " + name);
            }

            bound.Value = value;
            return;
        }

        if (strict && !HasProperty(Realm.GlobalObject, name))
        {
            ThrowReferenceError(name + " is not defined");
        }

        SetProperty(JsValue.Object(Realm.GlobalObject), name, value, strict);
    }

    /// <summary>Answers <c>delete</c> of one of the caller's names from eval code.</summary>
    /// <remarks>
    /// A declarative binding is never deletable and answers <c>false</c>; a <c>with</c> object's
    /// property is deleted from the object, with <c>DeleteProperty</c>'s strict rule; a global
    /// answers what <c>DeleteGlobalBinding</c> answers; and a name nothing binds answers
    /// <c>true</c>, because there was nothing to keep.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BA5477
    // Broiler-Human:        PENDING
    private JsValue DeleteEvalName(JsEnvironment boundary, string name, bool strict)
    {
        var binding = ResolveEvalName(boundary, name);

        if (binding.Record is not null || binding.Importer is not null)
        {
            return JsValue.False;
        }

        if (binding.Object is { } holder)
        {
            var went = holder.DeleteOwnProperty(name);

            if (!went && strict)
            {
                ThrowTypeError("Cannot delete property '" + name + "'");
            }

            return JsValue.Boolean(went);
        }

        return JsValue.Boolean(
            (!Realm.HasLexicals || !Realm.TryLexical(name, out _)) &&
            (!Realm.GlobalObject.HasOwnProperty(name) || Realm.GlobalObject.DeleteOwnProperty(name)));
    }

    /// <summary>Reads a global lexical binding, or refuses because it is in its dead zone.</summary>
    /// <remarks>
    /// <b>The dead zone is a state and the refusal is what makes it observable.</b> A binding that
    /// has been declared and not yet initialised holds nothing, and the language says a read of one
    /// is a <c>ReferenceError</c> naming it - which is the whole difference between a script-level
    /// <c>let</c> and the <c>var</c> whose read before the declaration answers <c>undefined</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=630988
    // Broiler-Human:        PENDING
    private JsValue ReadLexical(string name, JsLexicalBinding binding)
    {
        if (!binding.Initialised)
        {
            ThrowReferenceError("cannot access " + name + " before its declaration");
        }

        return binding.Value;
    }

    /// <summary>
    /// The object environment record that binds <paramref name="name"/> within the innermost
    /// <paramref name="limit"/> records, or <c>undefined</c> when none of them does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONLY LOOKUP BY NAME IN THE SCOPE CHAIN, AND IT CAN REACH NOTHING BUT AN
    /// OBJECT A <c>with</c> PUT THERE.</b> A declarative record is a <c>JsValue</c> array with no
    /// names in it, so this walk has nothing to compare a name against and skips it; a name the
    /// objects do not have therefore falls through to the <c>(depth, slot)</c> address the lowering
    /// computed from the language's own scope rules, and to nothing else. That is what stops a
    /// <c>with</c> body reaching an enclosing function's binding by naming it.
    /// </para>
    /// <para>
    /// <b><paramref name="limit"/> is the lowering's, and it is why an outer <c>with</c> cannot
    /// shadow an inner declaration.</b> It counts the records between the reference and the binding
    /// the lowering resolved, so a record past that binding is never asked. Walking the whole chain
    /// would make <c>with (a) { function f() { var x; with (b) { x } } }</c> read <c>a.x</c>.
    /// </para>
    /// <para>
    /// <b>Nothing here is cached, and that is the behaviour rather than a missing optimisation.</b>
    /// Each mention of a name inside a <c>with</c> body runs this walk again, so an object that
    /// gains the property between two reads is read from on the second and not on the first, and an
    /// object that loses it goes back to the enclosing binding. An implementation that remembered
    /// where a name resolved last time would answer the first reading for both.
    /// </para>
    /// <para>
    /// <b><c>Symbol.unscopables</c> is consulted per record and per name</b>, which is where the
    /// specification consults it - inside <c>HasBinding</c> - and is why a name listed truthily
    /// there does not stop the walk. It is a property read on the guest's object and can therefore
    /// run a getter, which is charged like any other call this walk makes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=21066F
    // Broiler-Falsified-If: this walk answers with anything but an object a `PushObjectScope` placed on the chain
    // Broiler-Human:        PENDING
    private JsValue ResolveName(List<JsEnvironment> scopes, int limit, string name)
    {
        var current = scopes[^1];

        for (var step = 0; step < limit && current is not null; step++)
        {
            Charge(1);

            if (current.Binding is { } bound && HasProperty(bound, name) && !Unscopable(bound, name))
            {
                return JsValue.Object(bound);
            }

            // A FUNCTION'S EVAL VARIABLES ARE ASKED AS A `with` OBJECT IS, without its
            // `Symbol.unscopables`: an own-property test of an object nobody else holds (JSeal V15).
            if (current.EvalVariables is { } introduced && introduced.HasOwnProperty(name))
            {
                return JsValue.Object(introduced);
            }

            current = current.Parent;
        }

        return JsValue.Undefined;
    }

    /// <summary>Whether <paramref name="bound"/> hides <paramref name="name"/> from a <c>with</c>.</summary>
    /// <remarks>
    /// <b>The blocklist is read off the object every time, prototype chain included.</b> It is what
    /// lets an Array put <c>values</c>, <c>keys</c> and <c>flat</c> on
    /// <c>Array.prototype[Symbol.unscopables]</c> so that <c>with (someArray) { values }</c> reaches
    /// an outer <c>values</c> rather than the method - which is the whole reason the Symbol was
    /// added to the language, and is a compatibility rule rather than a nicety.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=47A445
    // Broiler-Human:        PENDING
    private bool Unscopable(JsObject bound, string name)
    {
        var blocked = GetSymbol(JsValue.Object(bound), Realm.UnscopablesSymbol);
        return blocked.IsObject && GetProperty(blocked, name).ToBooleanValue();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=FAD55F
    // Broiler-Human:        PENDING
    private static JsEnvironment Slot(List<JsEnvironment> scopes, int depth, int index, out bool found)
    {
        var current = scopes[^1];

        for (var step = 0; step < depth; step++)
        {
            if (current.Parent is null)
            {
                found = false;
                return current;
            }

            current = current.Parent;
        }

        found = index < current.Slots.Length;
        return current;
    }
}
