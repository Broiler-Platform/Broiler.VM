// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What an exported name, once resolved, actually names.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
internal readonly struct JsBinding(int module, int slot, JsBindingKind kind, string name)
{
    /// <summary>Which module of the artifact holds the binding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal int Module { get; } = module;

    /// <summary>Which slot of that module's environment, when this is a slot binding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal int Slot { get; } = slot;

    /// <summary>Whether the binding is a slot or a whole namespace.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsBindingKind Kind { get; } = kind;

    /// <summary>The name, kept so a dead-zone read can say which binding it was.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal string Name { get; } = name;
}

/// <summary>One verified module record: the immutable half of a module.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal string Key { get; } = key;

    /// <summary>The code unit that is this module's body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal uint BodyUnit { get; } = bodyUnit;

    /// <summary>The code unit that initialises this module's environment.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal uint InitialiserUnit { get; } = initialiserUnit;

    /// <summary>The specifiers this module requested, as the source wrote them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal string[] RequestSpecifiers { get; } = requestSpecifiers;

    /// <summary>The module each request resolved to, by index into the artifact's records.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal int[] Requests { get; } = requests;

    /// <summary>Every name this module publishes, in ascending ordinal order.</summary>
    /// <remarks>
    /// Sorted because a namespace object's own property order is the specification's, which is the
    /// sorted order of its exported names - not the order the source happened to declare them in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal string[] ExportNames { get; } = exportNames;

    /// <summary>What each published name resolved to, parallel to <see cref="ExportNames"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsBinding[] ExportBindings { get; } = exportBindings;
}

/// <summary>Where one module of one instance has got to.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
internal enum JsModuleState
{
    /// <summary>Its environment exists and holds nothing.</summary>
    Created = 0,

    /// <summary>Its declarations are in place and its body has not run.</summary>
    Initialised = 1,

    /// <summary>Its body is running, which a module of a cycle observes about another.</summary>
    Evaluating = 2,

    /// <summary>Its body has run.</summary>
    Evaluated = 3,
}

/// <summary>One module of one instance: its environment, its namespace and its state.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsEnvironment Environment { get; } = environment;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsModuleState State { get; set; }

    /// <summary>The namespace object, built the first time something asks for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsModuleNamespace? Namespace { get; set; }

    /// <summary>What the module's body completed with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue Completion { get; set; } = JsValue.Undefined;
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
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
internal sealed class JsModuleNamespace : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly JsModuleRecord record;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly JsModuleInstance[] instances;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly JsEngine engine;

    /// <summary>Builds the namespace of one module.</summary>
    /// <remarks>
    /// The prototype is <see langword="null"/>, which the specification requires: a namespace
    /// inherits nothing, so <c>ns.toString</c> is <c>undefined</c> rather than
    /// <c>Object.prototype</c>'s.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsModuleNamespace(
        JsModuleRecord module, JsModuleInstance[] moduleInstances, JsEngine realm)
        : base(prototype: null, "Module")
    {
        record = module;
        instances = moduleInstances;
        engine = realm;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override void SetOwnProperty(string key, JsProperty property)
    {
        _ = key;
        _ = property;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>(record.ExportNames.Length);
        names.AddRange(record.ExportNames);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
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
