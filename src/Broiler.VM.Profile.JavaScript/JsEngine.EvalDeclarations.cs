// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           0
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  5/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// Performs the part of <c>EvalDeclarationInstantiation</c> that reaches outside a sloppy
    /// evaluation: every check first, then the caller's new bindings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing is created until every check has passed</b> (JSeal V15, JSD-0026 step 7). The walk
    /// goes from the site's innermost record to the variable environment through the caller's
    /// verified map, one row and one record at a time: a lexical binding of a declared name in any
    /// record on the way - a block's, a <c>for</c>-<c>let</c> head's, a <c>switch</c>'s, an enclosing
    /// evaluation's own, or a function's top-level <c>let</c>, <c>const</c> or <c>class</c> - is a
    /// <c>SyntaxError</c>. A <c>with</c> record is passed, because it holds no declarations, and so is
    /// a catch clause's parameter, whatever its form: the pinned ES2026 text exempts the record of a
    /// <c>Catch</c> clause (Annex B.3.4, normative-optional and supported here) and says nothing about
    /// the parameter's shape. A declared name the function record already binds as a <c>var</c>, a
    /// parameter, a function or <c>arguments</c> is that binding; everything else becomes a binding
    /// of the function's eval-variables set. At the global environment a declared name that is a
    /// global lexical declaration is a <c>SyntaxError</c> and a function or <c>var</c> the global
    /// object cannot take a <c>TypeError</c>, exactly as <c>CanDeclareGlobalFunction</c> and
    /// <c>CanDeclareGlobalVar</c> answer; what passes becomes a configurable property.
    /// </para>
    /// <para>
    /// <b>An Annex B block function is hoisted per evaluation</b>, when no lexical binding of its name
    /// lies between the call and the variable environment - a catch clause's parameter again
    /// excepted - and, at the global environment, when the name is no global lexical declaration and
    /// the global object could take a <c>var</c> of it. A hoisted name gets a binding holding
    /// <c>undefined</c> unless it is already declared, and only a hoisted name is written when its
    /// declaration is evaluated.
    /// </para>
    /// <para>
    /// <b>The order of creation is the specification's</b>: the Annex B names, then the functions -
    /// whose bindings are made here and written by the program's first instructions, which create the
    /// function objects - then the <c>var</c>s, so a global evaluation that declares both shows the
    /// function's property before the <c>var</c>'s. Each record walked and each name checked is
    /// charged, so no loop here is unmetered.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=9CDD79
    // Broiler-Falsified-If: a binding is created in the caller's scope or on the global object before a later check of the same evaluation throws, or a declared name is made a binding past a lexical binding of the same name that is not a catch parameter
    // Broiler-Human:        PENDING
    private void InstantiateEvalDeclarations(JsEvalView view, JsEnvironment? callerRecord, JsEvalDeclaration declaration)
    {
        var names = declaration.VarNames.Length + declaration.FunctionNames.Length;
        var annexB = declaration.AnnexBNames;
        var blocked = annexB.Length == 0 ? null : new bool[annexB.Length];
        JsEnvironment? variableRecord = null;
        JsEvalShape? variableShape = null;

        if (view.Program is { } program)
        {
            var map = program.EvalMap;
            var row = view.Site!.Scope;
            var record = callerRecord;

            while (true)
            {
                Charge(1 + (ulong)names + (ulong)annexB.Length);

                if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
                }

                var shape = map.Shapes[row];

                if (shape.Kind != JsFormat.EvalScopeKind.With)
                {
                    RequireNoLexical(shape, declaration.VarNames);
                    RequireNoLexical(shape, declaration.FunctionNames);

                    for (var at = 0; at < annexB.Length; at++)
                    {
                        if (shape.BindsLexically(annexB[at]))
                        {
                            blocked![at] = true;
                        }
                    }
                }

                if (shape.Kind is JsFormat.EvalScopeKind.Function or
                    JsFormat.EvalScopeKind.FunctionBody)
                {
                    if (record.Binding is not null)
                    {
                        throw new JsAbort(
                            JsAbortKind.InternalDefect, "an eval view's function row named an object record");
                    }


                    variableRecord = record;
                    variableShape = shape;
                    break;
                }

                if (shape.Kind == JsFormat.EvalScopeKind.Program)
                {
                    break;
                }

                if (shape.Kind == JsFormat.EvalScopeKind.Eval)
                {
                    // AN ENCLOSING EVALUATION IS SLOPPY, or this one could not be: its variable
                    // environment is its own caller's, so the walk goes on through the view its
                    // boundary was entered with - to the global one when that evaluation was global.
                    if (record.EvalView is not { } outer)
                    {
                        throw new JsAbort(
                            JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                    }

                    if (outer.Program is null)
                    {
                        break;
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

        var global = variableRecord is null;

        if (global)
        {
            Charge(1 + (ulong)names);
            RequireNoGlobalLexical(declaration.VarNames);
            RequireNoGlobalLexical(declaration.FunctionNames);

            // The specification visits the functions last to first; which one is named first changes
            // only the message of the TypeError, and the order kept here is that one.
            for (var at = declaration.FunctionNames.Length - 1; at >= 0; at--)
            {
                if (!CanDeclareGlobalFunction(declaration.FunctionNames[at]))
                {
                    ThrowTypeError(
                        "the global object cannot take a function named " + declaration.FunctionNames[at]);
                }
            }

            foreach (var name in declaration.VarNames)
            {
                if (!CanDeclareGlobalVar(name))
                {
                    ThrowTypeError("the global object cannot take a var named " + name);
                }
            }
        }

        var declared = new HashSet<string>(declaration.FunctionNames, StringComparer.Ordinal);

        for (var at = 0; at < annexB.Length; at++)
        {
            var name = annexB[at];
            Charge(1);

            if (blocked![at])
            {
                continue;
            }

            if (global &&
                ((Realm.HasLexicals && Realm.TryLexical(name, out _)) || !CanDeclareGlobalVar(name)))
            {
                continue;
            }

            if (!declared.Contains(name) && Array.IndexOf(declaration.VarNames, name) < 0)
            {
                if (global)
                {
                    CreateGlobalVarBinding(name);
                }
                else
                {
                    IntroduceEvalVariable(variableRecord!, variableShape!, name);
                }
            }

            declared.Add(name);
        }

        foreach (var name in declaration.FunctionNames)
        {
            Charge(1);

            if (global)
            {
                CreateGlobalFunctionBinding(name);
            }
            else
            {
                IntroduceEvalVariable(variableRecord!, variableShape!, name);
            }
        }

        foreach (var name in declaration.VarNames)
        {
            Charge(1);

            if (global)
            {
                CreateGlobalVarBinding(name);
            }
            else
            {
                IntroduceEvalVariable(variableRecord!, variableShape!, name);
            }
        }

        view.VariableRecord = variableRecord;
        view.VariableShape = variableShape;
        view.Declared = declared;
    }

    /// <summary>
    /// Performs the checks of <c>GlobalDeclarationInstantiation</c> for one script body, and the
    /// definition half of its function bindings, before the body's first instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every check runs before anything is created</b> (JSeal V15-host), in the pinned ES2026
    /// order: a lexically declared name that is already a global lexical declaration, or a
    /// non-configurable own property of the global object (<c>HasRestrictedGlobalProperty</c>), is a
    /// <c>SyntaxError</c>; a <c>var</c> or function name that is a global lexical declaration is a
    /// <c>SyntaxError</c>; a function the global object cannot take (<c>CanDeclareGlobalFunction</c>,
    /// last to first) and then a <c>var</c> it cannot take (<c>CanDeclareGlobalVar</c>) is a
    /// <c>TypeError</c>. The edition has no <c>[[VarNames]]</c> list, so a <c>let</c> over a
    /// configurable global - an eval-introduced <c>var</c>, say - is admitted.
    /// </para>
    /// <para>
    /// <b>What passes is created by the body's own instructions</b>, which follow here unchanged, with
    /// one step taken first: each function name gets the property <c>CreateGlobalFunctionBinding</c>
    /// defines - writable, enumerable and not configurable, replacing an absent or configurable one
    /// (an accessor included) and leaving a non-configurable one's attributes alone - so the body's
    /// write stores the function into the right property. The global lexical bindings the body then
    /// declares can no longer meet an existing one.
    /// </para>
    /// <para>
    /// <b>An Annex B candidate this lowering cannot skip is refused by name.</b> The specification
    /// hoists a block-level function only when its name is no global lexical declaration and the
    /// global object could take a <c>var</c> of it, and skips it otherwise; the body's instructions
    /// here create the alias and write it unconditionally, which would write an earlier script's
    /// lexical binding or grow a non-extensible global. So such a script is an <c>EvalError</c>
    /// before anything is created, rather than a wrong answer. Every name checked is charged.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=25A998
    // Broiler-Falsified-If: a binding is created on the global object or in the global lexical environment before a later check of the same script throws, or a lexical declaration is admitted over an existing global lexical declaration or a non-configurable global property
    // Broiler-Human:        PENDING
    private void InstantiateGlobalDeclarations(JsScriptDeclaration declaration)
    {
        var global = Realm.GlobalObject;

        Charge(
            1 + (ulong)declaration.LexicalNames.Length + (ulong)declaration.VarNames.Length +
            (ulong)declaration.FunctionNames.Length + (ulong)declaration.AnnexBNames.Length);

        foreach (var name in declaration.LexicalNames)
        {
            if (Realm.HasLexicals && Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError("a script cannot declare " + name + ": it is already a global lexical declaration");
            }

            if (global.TryGetOwnProperty(name, out var existing) && !existing.Configurable)
            {
                ThrowSyntaxError(
                    "a script cannot declare " + name + " lexically: it is a non-configurable property of the global object");
            }
        }

        RequireNoScriptLexical(declaration.VarNames);
        RequireNoScriptLexical(declaration.FunctionNames);

        for (var at = declaration.FunctionNames.Length - 1; at >= 0; at--)
        {
            if (!CanDeclareGlobalFunction(declaration.FunctionNames[at]))
            {
                ThrowTypeError("the global object cannot take a function named " + declaration.FunctionNames[at]);
            }
        }

        foreach (var name in declaration.VarNames)
        {
            if (!CanDeclareGlobalVar(name))
            {
                ThrowTypeError("the global object cannot take a var named " + name);
            }
        }

        foreach (var name in declaration.AnnexBNames)
        {
            if (Array.IndexOf(declaration.FunctionNames, name) >= 0 ||
                Array.IndexOf(declaration.VarNames, name) >= 0)
            {
                continue;
            }

            if ((Realm.HasLexicals && Realm.TryLexical(name, out _)) || !CanDeclareGlobalVar(name))
            {
                throw Error(
                    "EvalError",
                    "a script whose block-level function " + name + " Annex B would not hoist - a " +
                    "global lexical declaration of that name exists, or the global object cannot " +
                    "take it - is not admitted: this lowering writes the alias unconditionally");
            }
        }

        foreach (var name in declaration.FunctionNames)
        {
            if (!global.TryGetOwnProperty(name, out var existing) || existing.Configurable)
            {
                global.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable));
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a global lexical declaration has one of a script's <c>var</c>-scoped <paramref name="names"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=395810
    // Broiler-Human:        PENDING
    private void RequireNoScriptLexical(string[] names)
    {
        if (!Realm.HasLexicals)
        {
            return;
        }

        foreach (var name in names)
        {
            if (Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError("a script cannot declare var " + name + ": it is a global lexical declaration");
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a row binds one of <paramref name="names"/> lexically.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6B1830
    // Broiler-Human:        PENDING
    private void RequireNoLexical(JsEvalShape shape, string[] names)
    {
        foreach (var name in names)
        {
            if (shape.BindsLexically(name))
            {
                ThrowSyntaxError(
                    "a direct eval cannot declare var " + name +
                    ": a lexical declaration, or a parameter the evaluation's parameter list binds, of the same " +
                    "name lies between the call and its variable environment");
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a global lexical declaration has one of <paramref name="names"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EB55E4
    // Broiler-Human:        PENDING
    private void RequireNoGlobalLexical(string[] names)
    {
        if (!Realm.HasLexicals)
        {
            return;
        }

        foreach (var name in names)
        {
            if (Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError(
                    "an eval cannot declare var " + name + ": it is a global lexical declaration");
            }
        }
    }

    /// <summary>The specification's <c>CanDeclareGlobalFunction</c>, over this realm's global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C0336A
    // Broiler-Human:        PENDING
    private bool CanDeclareGlobalFunction(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.TryGetOwnProperty(name, out var existing))
        {
            return global.Extensible;
        }

        return existing.Configurable || (!existing.IsAccessor && existing.Writable && existing.Enumerable);
    }

    /// <summary>The specification's <c>CanDeclareGlobalVar</c>, over this realm's global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F88789
    // Broiler-Human:        PENDING
    private bool CanDeclareGlobalVar(string name) =>
        Realm.GlobalObject.HasOwnProperty(name) || Realm.GlobalObject.Extensible;

    /// <summary>
    /// The specification's <c>CreateGlobalVarBinding</c> with a deletable binding: a configurable
    /// property holding <c>undefined</c>, unless the global object already has one of the name.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7BBBC9
    // Broiler-Human:        PENDING
    private void CreateGlobalVarBinding(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.HasOwnProperty(name) && global.Extensible)
        {
            global.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// The definition half of the specification's <c>CreateGlobalFunctionBinding</c> with a deletable
    /// binding; the program's first instructions write the function object into it.
    /// </summary>
    /// <remarks>
    /// An absent or configurable property is replaced by a writable, enumerable, configurable data
    /// property - an accessor included, which is what "if a binding already exists, it is replaced"
    /// means; a non-configurable one, which <see cref="CanDeclareGlobalFunction"/> admitted only as a
    /// writable, enumerable data property, keeps its attributes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E28910
    // Broiler-Human:        PENDING
    private void CreateGlobalFunctionBinding(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.TryGetOwnProperty(name, out var existing) || existing.Configurable)
        {
            global.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// Gives a function's variable environment a binding of <paramref name="name"/> holding
    /// <c>undefined</c>, unless it has one: a slot the function declares, or an earlier evaluation's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=01E0CE
    // Broiler-Human:        PENDING
    private void IntroduceEvalVariable(JsEnvironment record, JsEvalShape shape, string name)
    {
        if (shape.Binds(name))
        {
            return;
        }

        var introduced = record.EvalVariables;

        if (introduced is null)
        {
            introduced = new JsEvalVariables();
            Retain(64);
            record.EvalVariables = introduced;
        }

        if (!introduced.HasOwnProperty(name))
        {
            // A BINDING NUMBER THE GUEST CONTROLS is retained like a collection entry, and it is
            // bounded by the source bytes the evaluation already paid for (JSD-0026 section 10).
            Retain(96);
            introduced.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// Writes one name of the variable environment an evaluation's boundary was entered with: the
    /// write <see cref="JsOpcode.StoreEvalVariable"/> performs.
    /// </summary>
    /// <remarks>
    /// A name the evaluation's instantiation did not declare - an Annex B alias the caller's bindings
    /// kept from hoisting - is not written. At a function's environment the write lands in the slot the
    /// function declares or in its eval-variables set, recreating a binding a <c>delete</c> removed, as
    /// <c>SetMutableBinding</c> does for a sloppy caller; at the global environment it is the global
    /// environment's <c>SetMutableBinding</c>: a global lexical binding when one has the name, and a
    /// sloppy <c>Set</c> on the global object otherwise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=19B50E
    // Broiler-Falsified-If: the write reaches a record other than the evaluation's variable environment, or writes a name its instantiation did not declare
    // Broiler-Human:        PENDING
    private void WriteEvalVariable(JsEnvironment boundary, string name, JsValue value)
    {
        var view = boundary.EvalView!;

        if (view.Declared is not { } declared || !declared.Contains(name))
        {
            return;
        }

        if (view.VariableRecord is { } record)
        {
            if (view.VariableShape!.TryFind(name, out var slot, out _, out _))
            {
                if (slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                record.Slots[slot] = value;
                return;
            }

            var introduced = record.EvalVariables;

            if (introduced is null)
            {
                introduced = new JsEvalVariables();
                Retain(64);
                record.EvalVariables = introduced;
            }

            if (!introduced.HasOwnProperty(name))
            {
                Retain(96);
            }

            introduced.SetOwnProperty(name, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            if (!bound.Initialised)
            {
                ThrowReferenceError("cannot access " + name + " before its declaration");
            }

            if (bound.Mutable)
            {
                bound.Value = value;
            }

            return;
        }

        SetProperty(JsValue.Object(Realm.GlobalObject), name, value, false);
    }
}
