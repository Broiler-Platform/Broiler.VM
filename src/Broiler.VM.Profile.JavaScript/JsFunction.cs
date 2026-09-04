// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   21
// Annotated:        21/21
// Exempt:           19
// Human-reviewed:   0/21
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       21
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One environment record: a flat slot array and a link to the enclosing one.</summary>
/// <remarks>
/// <para>
/// <b>Slots, not names.</b> Every binding a code unit declares is resolved to a (depth, slot) pair
/// by the front end, so nothing at run time looks a variable up by name except a global - which is
/// a property of an object and therefore named by definition. That is what makes a closure a
/// pointer rather than a dictionary.
/// </para>
/// <para>
/// A slot holding <see cref="JsValue.Empty"/> is a binding in its temporal dead zone. Reading one
/// throws a <c>ReferenceError</c>, which is the behaviour <c>let</c> and <c>const</c> need and
/// which no representation without a distinguished empty value can produce.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D0AAD1
// Broiler-Human:        PENDING
internal sealed class JsEnvironment
{
    /// <summary>Creates an environment of <paramref name="slots"/> slots under <paramref name="parent"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E75576
    // Broiler-Human:        PENDING
    internal JsEnvironment(int slots, JsEnvironment? parent)
    {
        Slots = slots == 0 ? System.Array.Empty<JsValue>() : new JsValue[slots];
        Parent = parent;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3991BD
    // Broiler-Human:        PENDING
    private JsEnvironment(JsValue[] slots, JsEnvironment? parent)
    {
        Slots = slots;
        Parent = parent;
    }

    /// <summary>The slots this record holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C8800E
    // Broiler-Human:        PENDING
    internal JsValue[] Slots { get; }

    /// <summary>The enclosing record, or <see langword="null"/> at the top.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6449D8
    // Broiler-Human:        PENDING
    internal JsEnvironment? Parent { get; }

    /// <summary>
    /// A fresh record with the same parent and a copy of this one's slots.
    /// </summary>
    /// <remarks>
    /// This is what a per-iteration <c>let</c> binding needs: each turn of the loop closes over its
    /// own copy, so a function created in the body sees the value that turn had rather than the
    /// value the loop finished with. Copying is the specification's <c>CreatePerIterationEnvironment</c>,
    /// and getting it wrong is the single most-reproduced closure bug in the language.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EB2301
    // Broiler-Human:        PENDING
    internal JsEnvironment Copy(int slots)
    {
        var copied = new JsValue[slots];
        var shared = System.Math.Min(slots, Slots.Length);

        for (var at = 0; at < shared; at++)
        {
            copied[at] = Slots[at];
        }

        return new JsEnvironment(copied, Parent);
    }

    /// <summary>Walks <paramref name="depth"/> records outward from this one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1DA08D
    // Broiler-Human:        PENDING
    internal JsEnvironment? Ancestor(int depth)
    {
        var current = this;

        for (var step = 0; step < depth; step++)
        {
            if (current.Parent is null)
            {
                return null;
            }

            current = current.Parent;
        }

        return current;
    }
}

/// <summary>The base of every callable object.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A00ED3
// Broiler-Human:        PENDING
internal abstract class JsFunction : JsObject
{
    /// <summary>Creates a function object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5BA6AC
    // Broiler-Human:        PENDING
    private protected JsFunction(JsObject? prototype)
        : base(prototype, "Function")
    {
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=24E030
    // Broiler-Human:        PENDING
    internal override bool IsCallable => true;

    /// <summary>What <c>Function.prototype.name</c> reports.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=429732
    // Broiler-Human:        PENDING
    internal string FunctionName { get; set; } = string.Empty;

    /// <summary>What <c>Function.prototype.length</c> reports.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4C6553
    // Broiler-Human:        PENDING
    internal int DeclaredArity { get; set; }
}

/// <summary>The signature every built-in written in C# has.</summary>
/// <param name="engine">The engine the call runs on.</param>
/// <param name="thisValue">The receiver, exactly as the caller supplied it.</param>
/// <param name="arguments">
/// The actual arguments. It is never <see langword="null"/> and may be shorter than the built-in's
/// declared arity, so every built-in reads it through <c>JsArguments.At</c> rather than by index.
/// </param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FB75E7
// Broiler-Human:        PENDING
internal delegate JsValue JsNativeBody(JsEngine engine, JsValue thisValue, JsValue[] arguments);

/// <summary>A built-in function: a delegate, with no bytecode behind it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4274F1
// Broiler-Human:        PENDING
internal sealed class JsNativeFunction : JsFunction
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=43F43F
    // Broiler-Human:        PENDING
    private readonly JsNativeBody body;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=617886
    // Broiler-Human:        PENDING
    private readonly JsNativeBody? construct;

    /// <summary>Creates a built-in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3F38CD
    // Broiler-Human:        PENDING
    internal JsNativeFunction(
        JsObject? prototype, string name, int arity, JsNativeBody body, JsNativeBody? construct = null)
        : base(prototype)
    {
        this.body = body;
        this.construct = construct;
        FunctionName = name;
        DeclaredArity = arity;
        SetOwnProperty(
            "length", JsProperty.Data(JsValue.Number(arity), JsPropertyAttributes.Configurable));

        SetOwnProperty(
            "name", JsProperty.Data(JsValue.String(name), JsPropertyAttributes.Configurable));
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=67614C
    // Broiler-Human:        PENDING
    internal override bool IsConstructor => construct is not null;

    /// <summary>Runs the built-in as a call.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=73DC8E
    // Broiler-Human:        PENDING
    internal JsValue Call(JsEngine engine, JsValue thisValue, JsValue[] arguments) =>
        body(engine, thisValue, arguments);

    /// <summary>Runs the built-in as a construction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3B5EDE
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsEngine engine, JsValue[] arguments) =>
        construct is null
            ? engine.ThrowTypeError(FunctionName + " is not a constructor")
            : construct(engine, JsValue.Undefined, arguments);
}

/// <summary>A function whose body is bytecode in a verified program.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F9BC7A
// Broiler-Human:        PENDING
internal sealed class JsScriptFunction : JsFunction
{
    /// <summary>Creates a closure over <paramref name="environment"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C030F0
    // Broiler-Human:        PENDING
    internal JsScriptFunction(
        JsObject? prototype, JsProgram program, int unit, JsEnvironment? environment)
        : base(prototype)
    {
        Program = program;
        Unit = unit;
        Environment = environment;
        var row = program.Functions[unit];
        FunctionName = row.Name;
        DeclaredArity = (int)row.ParameterCount;
    }

    /// <summary>The program the body lives in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    /// <summary>Which code unit the body is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=90B204
    // Broiler-Human:        PENDING
    internal int Unit { get; }

    /// <summary>The environment the function closed over.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8B56F2
    // Broiler-Human:        PENDING
    internal JsEnvironment? Environment { get; }

    /// <summary>The <c>this</c> an arrow function inherited, when it is one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D1A641
    // Broiler-Human:        PENDING
    internal JsValue LexicalThis { get; set; } = JsValue.Undefined;

    /// <summary>
    /// The object this function's <c>super</c> lookups start from, or <see langword="null"/> when
    /// it is not a method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a property of the FUNCTION and not of the call.</b> A method extracted from a
    /// prototype and called against an unrelated receiver still resolves <c>super</c> through the
    /// object it was defined on, which is the whole reason the specification gives a method a home
    /// object rather than deriving <c>super</c> from <c>this</c>. A lookup through the receiver's
    /// prototype would also be an infinite regress: a method on <c>C.prototype</c> called on an
    /// instance of a subclass would find itself.
    /// </para>
    /// <para>
    /// It is the class prototype for a prototype method, the constructor itself for a
    /// <c>static</c> method, and the object literal for a shorthand method. An arrow function has
    /// none of its own and reaches the enclosing method's through
    /// <see cref="LexicalActiveFunction"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsObject? HomeObject { get; set; }

    /// <summary>
    /// The box holding the <c>this</c> an arrow function inherited, when the frame it closed over
    /// had one that could still change.
    /// </summary>
    /// <remarks>
    /// A derived constructor's <c>this</c> does not exist until <c>super()</c> returns, so an
    /// arrow created before that point cannot capture a value - there is none - and must capture
    /// the BINDING. Copying the value would have given every such arrow a permanently dead
    /// <c>this</c>, which is the shape of defect that only shows up in a constructor that creates
    /// its callbacks before it calls up.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsCell? LexicalThisBinding { get; set; }

    /// <summary>The <c>new.target</c> an arrow function inherited, when it is one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue LexicalNewTarget { get; set; } = JsValue.Undefined;

    /// <summary>
    /// The function an arrow's <c>super</c> belongs to, which is the nearest enclosing
    /// non-arrow function.
    /// </summary>
    /// <remarks>
    /// One field rather than two: the home object a <c>super</c> property needs and the superclass
    /// a <c>super()</c> needs are both reached from the same function object, so carrying that
    /// function is what an arrow inherits and the two answers follow from it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsScriptFunction? LexicalActiveFunction { get; set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0AB323
    // Broiler-Human:        PENDING
    internal override bool IsConstructor =>
        (Program.Functions[Unit].Flags & Format.JsFormat.FunctionFlags.Constructible) != 0;

    /// <summary>Whether calling this function without <c>new</c> is a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool IsClassConstructor => Program.Functions[Unit].IsClassConstructor;

    /// <summary>Whether this function's <c>this</c> is created by its own <c>super()</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool IsDerivedConstructor => Program.Functions[Unit].IsDerivedConstructor;
}

/// <summary>The result of <c>Function.prototype.bind</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=40B37C
// Broiler-Human:        PENDING
internal sealed class JsBoundFunction : JsFunction
{
    /// <summary>Creates a bound function.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=83F8C9
    // Broiler-Human:        PENDING
    internal JsBoundFunction(
        JsObject? prototype, JsObject target, JsValue boundThis, JsValue[] boundArguments)
        : base(prototype)
    {
        Target = target;
        BoundThis = boundThis;
        BoundArguments = boundArguments;
    }

    /// <summary>The function the bound one calls.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=242A37
    // Broiler-Human:        PENDING
    internal JsObject Target { get; }

    /// <summary>The receiver every call uses.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2D30C0
    // Broiler-Human:        PENDING
    internal JsValue BoundThis { get; }

    /// <summary>The arguments every call is prefixed with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=02DA4C
    // Broiler-Human:        PENDING
    internal JsValue[] BoundArguments { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=35044D
    // Broiler-Human:        PENDING
    internal override bool IsConstructor => Target.IsConstructor;
}

/// <summary>A primitive wrapped in an object: <c>new Number</c>, <c>new String</c>, <c>new Boolean</c>.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BB8095
// Broiler-Human:        PENDING
internal sealed class JsPrimitiveWrapper : JsObject
{
    /// <summary>Creates a wrapper.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=28A11E
    // Broiler-Human:        PENDING
    internal JsPrimitiveWrapper(JsObject? prototype, string className, JsValue primitive)
        : base(prototype, className) => Primitive = primitive;

    /// <summary>The wrapped value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F1165A
    // Broiler-Human:        PENDING
    internal JsValue Primitive { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6FF15C
    // Broiler-Human:        PENDING
    internal override bool TryGetOwnProperty(string key, out JsProperty property)
    {
        if (Primitive.IsString)
        {
            var text = Primitive.AsString();

            if (string.Equals(key, "length", System.StringComparison.Ordinal))
            {
                property = JsProperty.Data(JsValue.Number(text.Length), JsPropertyAttributes.None);
                return true;
            }

            if (IsArrayIndex(key, out var at) && at < text.Length)
            {
                property = JsProperty.Data(
                    JsValue.String(text[(int)at].ToString()), JsPropertyAttributes.Enumerable);

                return true;
            }
        }

        return base.TryGetOwnProperty(key, out property);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=64CA9C
    // Broiler-Human:        PENDING
    internal override System.Collections.Generic.List<string> OwnPropertyNames()
    {
        var names = new System.Collections.Generic.List<string>();

        if (Primitive.IsString)
        {
            var text = Primitive.AsString();

            for (var at = 0; at < text.Length; at++)
            {
                names.Add(JsNumberFormat.ToUintString((uint)at));
            }

            names.Add("length");
        }

        names.AddRange(base.OwnPropertyNames());
        return names;
    }
}

/// <summary>A mutable box the engine uses to hold a value a native closure has to share.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6E4C89
// Broiler-Human:        PENDING
internal sealed class JsCell
{
    /// <summary>The held value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8DB3A8
    // Broiler-Human:        PENDING
    internal JsValue Value { get; set; } = JsValue.Undefined;
}
