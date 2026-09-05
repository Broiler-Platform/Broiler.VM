// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           26
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       13
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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=951220
// Broiler-Human:        PENDING
internal enum JsModuleState
{
    /// <summary>Its environment exists and holds nothing.</summary>
    Created = 0,

    /// <summary>
    /// It has been put into an evaluation order and its body has not been entered.
    /// </summary>
    /// <remarks>
    /// A state of its own rather than a second use of <see cref="Evaluating"/>, because the walk
    /// that builds the order runs before any body does: a module marked as under way by the
    /// ORDERING walk would be indistinguishable from one whose body had actually started, and the
    /// difference is what a cyclic graph is decided on.
    /// </remarks>
    Ordered = 4,

    /// <summary>Its declarations are in place and its body has not run.</summary>
    Initialised = 1,

    /// <summary>Its body is running, which a module of a cycle observes about another.</summary>
    Evaluating = 2,

    /// <summary>Its body has run.</summary>
    Evaluated = 3,
}

/// <summary>One module of one instance: its environment, its namespace and its state.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C800DB
// Broiler-Human:        PENDING
internal sealed class JsModuleInstance(JsEnvironment environment)
{
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
