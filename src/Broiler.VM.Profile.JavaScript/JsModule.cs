// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           36
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What an exported name, once resolved, actually names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=54701B
// Broiler-Human:        PENDING
internal enum JsBindingKind
{
    /// <summary>A slot of one module's environment.</summary>
    Slot = 0,

    /// <summary>One module's namespace object.</summary>
    Namespace = 1,
}

/// <summary>
/// One resolved binding: what an import or an exported name reads through at run time.
/// </summary>
/// <remarks>
/// <b>It is a place and not a value, and that is the whole of a live binding.</b> Resolution
/// happens once, at verification, and answers with a module and a slot; every read goes to that
/// slot afresh, so a write the exporting module makes after an importer was evaluated is seen.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=369AD9
// Broiler-Human:        PENDING
internal readonly struct JsBinding(int module, int slot, JsBindingKind kind, string name)
{
    /// <summary>Which module of the artifact holds the binding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C61B94
    // Broiler-Human:        PENDING
    internal int Module { get; } = module;

    /// <summary>Which slot of that module's environment, when this is a slot binding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=897E94
    // Broiler-Human:        PENDING
    internal int Slot { get; } = slot;

    /// <summary>Whether the binding is a slot or a whole namespace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=90EEB8
    // Broiler-Human:        PENDING
    internal JsBindingKind Kind { get; } = kind;

    /// <summary>The name, kept so a dead-zone read can say which binding it was.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C26712
    // Broiler-Human:        PENDING
    internal string Name { get; } = name;
}

/// <summary>One verified module record: the immutable half of a module.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=454AC3
// Broiler-Human:        PENDING
internal sealed class JsModuleRecord(
    string key,
    uint bodyUnit,
    uint initialiserUnit,
    string[] requestSpecifiers,
    int[] requests,
    string[] exportNames,
    JsBinding[] exportBindings)
{
    /// <summary>The key the composition resolved this module to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=71D82D
    // Broiler-Human:        PENDING
    internal string Key { get; } = key;

    /// <summary>The code unit that is this module's body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ECDEAB
    // Broiler-Human:        PENDING
    internal uint BodyUnit { get; } = bodyUnit;

    /// <summary>The code unit that initialises this module's environment.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=04946D
    // Broiler-Human:        PENDING
    internal uint InitialiserUnit { get; } = initialiserUnit;

    /// <summary>The specifiers this module requested, as the source wrote them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=99FC7E
    // Broiler-Human:        PENDING
    internal string[] RequestSpecifiers { get; } = requestSpecifiers;

    /// <summary>The module each request resolved to, by index into the artifact's records.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=437BE9
    // Broiler-Human:        PENDING
    internal int[] Requests { get; } = requests;

    /// <summary>Every name this module publishes, in ascending ordinal order.</summary>
    /// <remarks>
    /// Sorted because a namespace object's own property order is the specification's, which is the
    /// sorted order of its exported names - not the order the source happened to declare them in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=28F4AD
    // Broiler-Human:        PENDING
    internal string[] ExportNames { get; } = exportNames;

    /// <summary>What each published name resolved to, parallel to <see cref="ExportNames"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E6DE6
    // Broiler-Human:        PENDING
    internal JsBinding[] ExportBindings { get; } = exportBindings;
}

/// <summary>Where one module of one instance has got to.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9B665D
// Broiler-Human:        PENDING
internal enum JsModuleState
{
    /// <summary>Its environment exists and holds nothing.</summary>
    Created = 0,

    /// <summary>Its declarations are in place and its body has not run.</summary>
    Initialised = 1,

    /// <summary>Its body is running, which a module of a cycle observes about another.</summary>
    Evaluating = 2,

    /// <summary>
    /// It, or a module it waits on, has a top-level <c>await</c> that has not finished: the
    /// specification's <c>~evaluating-async~</c> (JSeal I11-async).
    /// </summary>
    /// <remarks>
    /// A state of its own because a module that has left the walk and is still awaiting is neither
    /// under way on the walk nor finished: a later evaluation that meets it depends on its cycle
    /// root's completion, and must not read its bindings as if its body had run.
    /// </remarks>
    EvaluatingAsync = 4,

    /// <summary>Its body has run, or its evaluation has failed.</summary>
    Evaluated = 3,
}

/// <summary>One module of one instance: its environment, its namespace and its state.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9866BE
// Broiler-Human:        PENDING
internal sealed class JsModuleInstance(JsEnvironment environment, JsProgram program, int index)
{
    /// <summary>
    /// The artifact that created this instance, whose record, body and requests are the ones every
    /// evaluation uses (JSeal I11-async).
    /// </summary>
    /// <remarks>
    /// An instance is shared by every artifact that carries its module, and each of those numbers
    /// the module differently; the evaluation follows one module's requests from one place, so it
    /// reads them where the instance was made, and the modules they name are the realm's instances
    /// of the same keys.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D89DD8
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; } = program;

    /// <summary>The module's index among <see cref="Program"/>'s records.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7F4989
    // Broiler-Human:        PENDING
    internal int Index { get; } = index;

    /// <summary>
    /// The module's own environment, which outlives its evaluation.
    /// </summary>
    /// <remarks>
    /// A script's frame is discarded when the script returns and a module's is not: an importer
    /// reads through a slot of this record long after the module that declared it finished, so the
    /// environment lives for as long as the instance does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=419CB2
    // Broiler-Human:        PENDING
    internal JsEnvironment Environment { get; } = environment;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CAFA03
    // Broiler-Human:        PENDING
    internal JsModuleState State { get; set; }

    /// <summary>The namespace object, built the first time something asks for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=77839A
    // Broiler-Human:        PENDING
    internal JsModuleNamespace? Namespace { get; set; }

    /// <summary>What the module's body completed with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=62D6FD
    // Broiler-Human:        PENDING
    internal JsValue Completion { get; set; } = JsValue.Undefined;

    /// <summary>
    /// What the module's evaluation threw, or <see langword="null"/> while it has thrown nothing.
    /// </summary>
    /// <remarks>
    /// <b>An evaluation that threw is FINISHED, and every later request for the module answers the
    /// same thrown value.</b> That is the language's <c>[[EvaluationError]]</c>: the body is not run
    /// a second time, and a second <c>import()</c> rejects with the identical value rather than
    /// resolving to a namespace whose bindings are still in their dead zone. A module that depends
    /// on one that threw is given the same value, because its body cannot run either.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1E9BEE
    // Broiler-Human:        PENDING
    internal JsValue? EvaluationError { get; set; }

    /// <summary>
    /// The specification's <c>[[DFSIndex]]</c>: the order in which the evaluation walk reached the
    /// module.
    /// </summary>
    /// <remarks>
    /// Kept on the instance, as the specification keeps it on the record, because only one
    /// evaluation walk is ever under way in a realm: an evaluation asked for while one runs is
    /// deferred to a job (see <c>JsEngine.EvaluateInto</c>).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=61B180
    // Broiler-Human:        PENDING
    internal int DfsIndex { get; set; }

    /// <summary>
    /// The specification's <c>[[DFSAncestorIndex]]</c>: the lowest walk index the module reaches
    /// through modules still on the walk's stack.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A6AFBF
    // Broiler-Human:        PENDING
    internal int DfsAncestorIndex { get; set; }

    /// <summary>
    /// The specification's <c>[[PendingAsyncDependencies]]</c>: how many async dependencies the
    /// module still waits on before its body may run.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=372302
    // Broiler-Human:        PENDING
    internal int PendingAsyncDependencies { get; set; }

    /// <summary>
    /// The specification's <c>[[AsyncEvaluationOrder]]</c>: 0 while unset, the realm's count at
    /// the moment the module became async, or <see cref="AsyncEvaluationDone"/>.
    /// </summary>
    /// <remarks>
    /// <b>The modules one completion releases run in this order</b>, which is the order the walk
    /// first reached them; sorting by it is what makes two parents of one async module run in the
    /// order they were imported, whichever of them the completion happens to list first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=77AD24
    // Broiler-Human:        PENDING
    internal long AsyncEvaluationOrder { get; set; }

    /// <summary>The <see cref="AsyncEvaluationOrder"/> of a module whose async evaluation ended.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=62FFF1
    // Broiler-Human:        PENDING
    internal const long AsyncEvaluationDone = -1;

    /// <summary>
    /// The specification's <c>[[AsyncParentModules]]</c>: the modules waiting on this one's async
    /// evaluation, or <see langword="null"/> while none is, and again once it has ended.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=37A066
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsModuleInstance>? AsyncParentModules { get; set; }

    /// <summary>
    /// The specification's <c>[[TopLevelCapability]]</c>: the promise an evaluation that began at
    /// this module answers, which every later evaluation of its component answers too.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=329C96
    // Broiler-Human:        PENDING
    internal JsPromiseObject? TopLevelCapability { get; set; }

    /// <summary>Whether the module's body has a top-level <c>await</c> (<c>[[HasTLA]]</c>).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=453146
    // Broiler-Human:        PENDING
    internal bool HasTla => Program.Functions[(int)Program.Modules[Index].BodyUnit].IsAsync;

    /// <summary>
    /// The root of the strongly connected component this module was evaluated in, or
    /// <see langword="null"/> for a module whose evaluation has not left the walk (or failed on it).
    /// </summary>
    /// <remarks>
    /// <b>A MEMBER OF A CYCLE IS FINISHED ONLY WHEN ITS ROOT IS</b> (the specification's
    /// <c>[[CycleRoot]]</c>, ES2026 <c>Evaluate</c> step 3). A member whose own body ran reads as
    /// evaluated while the root it cycles with is still awaiting; a later evaluation of the member,
    /// and every module that depends on it, is sent to the root instead.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A51F5A
    // Broiler-Human:        PENDING
    internal JsModuleInstance? CycleRoot { get; set; }

    /// <summary>The module's <c>import.meta</c> object, built the first time it is asked for.</summary>
    /// <remarks>
    /// <b>It lives on the INSTANCE and not on the record, which is the whole of what the language
    /// says about it.</b> Two evaluations of <c>import.meta</c> in one module answer the same
    /// object, so a guest may hang a value on it in one function and read it back in another; a
    /// fresh object per evaluation would make every such program silently answer
    /// <c>undefined</c>. The record is shared by every realm that runs the artifact and the
    /// instance is not, which is also why two realms running one module get two of these.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5719BF
    // Broiler-Human:        PENDING
    internal JsObject? Meta { get; set; }
}

/// <summary>
/// A module namespace object: the exported names of one module, read through to their bindings.
/// </summary>
/// <remarks>
/// <para>
/// <b>Its properties are computed on every read rather than copied in when it is built.</b> A
/// namespace is the one place a live binding is observed through the ordinary property path, so
/// <c>ns.counter</c> after the exporting module incremented its counter has to answer the new
/// value. Building it out of data properties would have frozen the values at link time and made
/// every one of those reads answer what the module had before it ran.
/// </para>
/// <para>
/// <b>It is not extensible and its properties are not writable</b>, which the language requires
/// and which this expresses by refusing the write rather than by carrying attributes something
/// else would have to honour.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=11A1AB
// Broiler-Human:        PENDING
internal sealed class JsModuleNamespace : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D8943F
    // Broiler-Human:        PENDING
    private readonly JsModuleRecord record;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=392625
    // Broiler-Human:        PENDING
    private readonly JsModuleInstance[] instances;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3283F6
    // Broiler-Human:        PENDING
    private readonly JsEngine engine;

    /// <summary>Builds the namespace of one module.</summary>
    /// <remarks>
    /// <para>
    /// The prototype is <see langword="null"/>, which the specification requires: a namespace
    /// inherits nothing, so <c>ns.toString</c> is <c>undefined</c> rather than
    /// <c>Object.prototype</c>'s.
    /// </para>
    /// <para>
    /// <b>IT IS NOT EXTENSIBLE FROM THE MOMENT IT EXISTS, and nothing ever makes it so.</b> A
    /// namespace's property set is the module's export set and that set is fixed at link; a
    /// namespace left extensible would let a program add a property that shadows nothing, and then
    /// answer <c>Object.isExtensible</c> and <c>Reflect.setPrototypeOf</c> the way an ordinary
    /// object does rather than the way the language says a namespace does.
    /// </para>
    /// <para>
    /// <b><c>@@toStringTag</c> is an own property of the namespace and not of a prototype</b>,
    /// because a namespace has no prototype to carry it. It is the one Symbol-keyed property a
    /// namespace has, it is <c>"Module"</c>, and it is frozen: the specification pins all three
    /// attributes off, which is what makes <c>Object.prototype.toString.call(ns)</c> answer
    /// <c>[object Module]</c> and makes a redefinition of it a refusal.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D007E4
    // Broiler-Human:        PENDING
    internal JsModuleNamespace(
        JsModuleRecord module, JsModuleInstance[] moduleInstances, JsEngine realm)
        : base(prototype: null, "Module")
    {
        record = module;
        instances = moduleInstances;
        engine = realm;
        Extensible = false;

        SetOwnSymbol(
            realm.Realm.ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Module"), JsPropertyAttributes.None));
    }

    /// <summary>Whether this namespace publishes a name, without reading what it is bound to.</summary>
    /// <remarks>
    /// <b>ASKING WHETHER A NAME IS EXPORTED IS NOT READING IT, and a namespace is where the two
    /// come apart.</b> <c>'x' in ns</c> is true for an export whose module has not run, while
    /// <c>ns.x</c> on the same name is a <c>ReferenceError</c>; answering the first by attempting
    /// the second turns a question about the export set into a throw.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9C4B31
    // Broiler-Human:        PENDING
    internal bool Exports(string key)
    {
        foreach (var name in record.ExportNames)
        {
            if (string.Equals(name, key, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FAB29B
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        for (var index = 0; index < record.ExportNames.Length; index++)
        {
            if (!string.Equals(record.ExportNames[index], key, System.StringComparison.Ordinal))
            {
                continue;
            }

            property = JsProperty.Data(
                Read(record.ExportBindings[index], instances, engine),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable);

            return true;
        }

        property = default;
        return false;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// A namespace's properties are not writable and it is not extensible, so a write is dropped
    /// here rather than allowed to add a property that would then shadow an export.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DDF7B5
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property)
    {
        _ = key;
        _ = property;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EFA521
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        foreach (var name in record.ExportNames)
        {
            if (string.Equals(name, key, System.StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <b>THE ORDER IS SORTED AND NOT THE ORDER THE EXPORTS WERE WRITTEN IN.</b> Every other
    /// object answers this in insertion order, and a namespace is the one place the language says
    /// otherwise: its keys are the export names in code-unit order, so two modules that export the
    /// same names in different orders have namespaces a program cannot tell apart.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8CEE60
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>(record.ExportNames.Length);
        names.AddRange(record.ExportNames);
        names.Sort(System.StringComparer.Ordinal);
        return names;
    }

    /// <summary>Reads one resolved binding, refusing a read before its initialisation.</summary>
    /// <remarks>
    /// <b>The temporal dead zone crosses a module boundary and this is where it is enforced.</b>
    /// A cyclic import can reach a <c>let</c> of a module whose body has not run, and the answer
    /// the language gives is a <c>ReferenceError</c> rather than <c>undefined</c> - which is the
    /// difference between a program that is told what is wrong and one that goes on with a value
    /// that means nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B395A3
    // Broiler-Human:        PENDING
    internal static JsValue Read(
        in JsBinding binding, JsModuleInstance[] instances, JsEngine engine)
    {
        var instance = instances[binding.Module];

        if (binding.Kind == JsBindingKind.Namespace)
        {
            return JsValue.Object(instance.Namespace!);
        }

        var value = instance.Environment.Slots[binding.Slot];

        if (value.IsEmpty)
        {
            engine.ThrowReferenceError(
                "Cannot access '" + binding.Name + "' before initialisation");
        }

        return value;
    }
}
