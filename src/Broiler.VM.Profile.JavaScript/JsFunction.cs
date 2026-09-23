// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           35
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       29
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
/// <para>
/// <b>ONE record kind is the exception, and it is the only one with names in it: the object
/// environment record a <c>with</c> makes.</b> It holds a guest object in <see cref="Binding"/> and
/// no slots at all, and a name is asked of it through the object's own property lookup. It sits on
/// the same chain as every other record and is counted by <see cref="Ancestor"/> like any other, so
/// a <c>(depth, slot)</c> pair emitted inside a <c>with</c> body still reaches what it was compiled
/// to reach. <b>A declarative record cannot be searched by name and that is deliberate</b>: there
/// are no names in one to search, so a dynamic lookup can reach an object a <c>with</c> put on the
/// chain and can reach nothing else.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D0AAD1
// Broiler-Falsified-If: a lookup by name reaches a slot of a declarative record
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

    /// <summary>Creates the object environment record a <c>with</c> statement puts on the chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3A6E29
    // Broiler-Human:        PENDING
    internal JsEnvironment(JsObject binding, JsEnvironment? parent)
    {
        Slots = System.Array.Empty<JsValue>();
        Parent = parent;
        Binding = binding;
    }

    /// <summary>
    /// Creates the boundary record an evaluated program is entered with, under its caller's record.
    /// </summary>
    /// <remarks>
    /// <b>It is a declarative record like any other</b> - the evaluated program's own lexical
    /// declarations, and a strict one's <c>var</c>s, are its slots - with one addition: the view it
    /// was entered with, which is what the eval name instructions read when they reach it. A global
    /// evaluation's boundary (JSeal V15) has no parent at all: nothing lies between it and the
    /// global scope, which its view says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F9EB3D
    // Broiler-Falsified-If: a boundary record is created with a parent other than the calling frame's current record
    // Broiler-Human:        PENDING
    internal JsEnvironment(int slots, JsEnvironment? parent, JsEvalView view)
        : this(slots, parent)
    {
        EvalView = view;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7EE350
    // Broiler-Human:        PENDING
    private JsEnvironment(JsValue[] slots, JsEnvironment? parent, JsObject? binding)
    {
        Slots = slots;
        Parent = parent;
        Binding = binding;
    }

    /// <summary>
    /// The view an evaluated program's boundary record was entered with, or <see langword="null"/>
    /// on every other record.
    /// </summary>
    /// <remarks>
    /// <b>It is not a way to search a declarative record by name.</b> What a name resolved through it
    /// reaches is decided by the caller's verified scope map, which names exactly the slots the
    /// caller's own instructions could address - so the property this class's falsifier states still
    /// holds of every lookup this record takes part in: nothing is found by comparing a name against
    /// a declarative record, because a declarative record still has no names in it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3D0B82
    // Broiler-Human:        PENDING
    internal JsEvalView? EvalView { get; }

    /// <summary>
    /// The bindings direct evaluations introduced into this record, when it is a function's and
    /// one has (JSeal V15, JSD-0026 step 6); <see langword="null"/> everywhere else.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the eval-variables record of the design, held by the function record rather than
    /// placed beside it.</b> A name is resolved in the record's slots first and here second, which is
    /// the order an outer record would give, because an evaluation never introduces a name the
    /// function record already binds - its declaration instantiation writes that slot instead - and
    /// holding it here moves no record, so no <c>(depth, slot)</c> pair compiled in or under the
    /// function changes.
    /// </para>
    /// <para>
    /// <b>It is created by the first evaluation that declares something here</b>, so a call of a
    /// function whose evaluations declare nothing allocates nothing for it. The lowering marks every
    /// function that may carry one, and searches it by name exactly as it searches a <c>with</c>
    /// object: this is the second, and only other, record content a lookup by name can reach.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=FF611F
    // Broiler-Falsified-If: a lookup by name reaches a slot of this record rather than a binding an evaluation introduced
    // Broiler-Human:        PENDING
    internal JsEvalVariables? EvalVariables { get; set; }

    /// <summary>The slots this record holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C8800E
    // Broiler-Human:        PENDING
    internal JsValue[] Slots { get; }

    /// <summary>
    /// The object this record binds names through, or <see langword="null"/> in a declarative one.
    /// </summary>
    /// <remarks>
    /// It is the whole of what makes this record kind different, and it is READ-ONLY here: the
    /// record never rebinds, so an object a <c>with</c> put on the chain is the object every name
    /// resolved through that record asks, for as long as the record is on it. What CAN change is
    /// the object's own properties, which is why nothing about a name resolved through one is
    /// cached anywhere.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A86796
    // Broiler-Human:        PENDING
    internal JsObject? Binding { get; }

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DEB6B7
    // Broiler-Human:        PENDING
    internal JsEnvironment Copy(int slots)
    {
        var copied = new JsValue[slots];
        var shared = System.Math.Min(slots, Slots.Length);

        for (var at = 0; at < shared; at++)
        {
            copied[at] = Slots[at];
        }

        // The binding travels with the copy. A per-iteration copy is only ever emitted for a
        // declarative record, so this arm is unreachable from this lowering - and dropping the
        // binding rather than carrying it would turn an artifact that copied an object record into
        // one whose names silently stopped resolving, which is a worse answer than the honest one.
        return new JsEnvironment(copied, Parent, Binding);
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

    /// <summary>
    /// Whether the construct body builds its instance from <c>new.target</c>'s prototype itself,
    /// so the engine must not read that prototype again after the body returns.
    /// </summary>
    /// <remarks>
    /// <b>The specification reads the prototype part-way through a constructor, not after it.</b>
    /// <c>DataView</c> and the nine typed arrays read it between converting their arguments and
    /// asking whether the buffer is still attached, and a <c>prototype</c> getter can detach the
    /// buffer in between; a constructor that says so here reads it at that point, and the engine's
    /// re-pointing after the body - which would run the getter a second time - is skipped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=461CA5
    // Broiler-Human:        PENDING
    internal bool BuildsFromNewTarget { get; set; }

    /// <summary>Whether this built-in carries an <c>[[IsHTMLDDA]]</c> slot; set only by the host surface.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3C7479
    // Broiler-Human:        PENDING
    internal bool EmulatesUndefined { get; init; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B27714
    // Broiler-Human:        PENDING
    internal override bool IsHtmlDda => EmulatesUndefined;

    /// <summary>Runs the built-in as a call.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=73DC8E
    // Broiler-Human:        PENDING
    internal JsValue Call(JsEngine engine, JsValue thisValue, JsValue[] arguments) =>
        body(engine, thisValue, arguments);

    /// <summary>Runs the built-in as a construction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E27AED
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsEngine engine, JsValue[] arguments) =>
        Construct(engine, arguments, JsValue.Undefined);

    /// <summary>Runs the built-in as a construction, telling it what was newed.</summary>
    /// <remarks>
    /// <b>The new target arrives in the receiver slot, and that is a deliberate reuse rather than a
    /// shortcut.</b> A construct call has no receiver - the language has not created the object
    /// yet - so the slot is dead weight on this path, and every built-in that ignores it goes on
    /// ignoring it. The alternative was a third parameter on <see cref="JsNativeBody"/>, which
    /// every built-in in the profile would have had to grow to serve the handful of constructors
    /// that ask.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=868CD7
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsEngine engine, JsValue[] arguments, JsValue newTarget) =>
        construct is null
            ? engine.ThrowTypeError(FunctionName + " is not a constructor")
            : construct(engine, newTarget, arguments);
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

    /// <summary>
    /// The function's <c>[[ScriptOrModule]]</c>, as the referrer a host resolves against: the key of
    /// the module, or the referrer of the script, that was running when the function was created,
    /// or empty for none (JSeal I12-upstream, JSD-0024 section 20).
    /// </summary>
    /// <remarks>
    /// <b>It is fixed at creation, which is the language's rule</b> (<c>OrdinaryFunctionCreate</c>
    /// sets it from <c>GetActiveScriptOrModule()</c>): a function a module's code made with the
    /// <c>Function</c> constructor keeps that module however it is later called, and a frame of this
    /// function answers it to eval code, to a <c>Function</c> body and to an <c>import()</c> with no
    /// referrer of its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F2A0FC
    // Broiler-Human:        PENDING
    internal string ScriptOrModule { get; init; } = string.Empty;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E71C47
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=34A6ED
    // Broiler-Human:        PENDING
    internal JsCell? LexicalThisBinding { get; set; }

    /// <summary>The <c>new.target</c> an arrow function inherited, when it is one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=04494B
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=101961
    // Broiler-Human:        PENDING
    internal JsScriptFunction? LexicalActiveFunction { get; set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0AB323
    // Broiler-Human:        PENDING
    internal override bool IsConstructor =>
        (Program.Functions[Unit].Flags & Format.JsFormat.FunctionFlags.Constructible) != 0;

    /// <summary>Whether calling this function without <c>new</c> is a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A7DFE0
    // Broiler-Human:        PENDING
    internal bool IsClassConstructor => Program.Functions[Unit].IsClassConstructor;

    /// <summary>Whether this function's <c>this</c> is created by its own <c>super()</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=57770B
    // Broiler-Human:        PENDING
    internal bool IsDerivedConstructor => Program.Functions[Unit].IsDerivedConstructor;

    /// <summary>
    /// The class elements every instance of this constructor is given, in the order the class body
    /// wrote them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>They live on the CONSTRUCTOR and not on the prototype, and the difference is which class
    /// of a chain answers.</b> <c>class B { x = 1 } class D extends B { y = 2 }</c> gives an
    /// instance both fields because B's constructor runs and installs its own, then D's
    /// <c>super()</c> returns and D's are installed - two lists, one per class, applied by the
    /// constructor each belongs to. A list on the prototype would have been found by a prototype
    /// walk and applied once with whatever it found first.
    /// </para>
    /// <para>
    /// <b>It is null on every function that is not a class constructor</b>, which is nearly all of
    /// them, so an ordinary closure carries one null field and no list.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=578942
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsClassElement>? InstanceElements { get; set; }

    /// <summary>
    /// The class elements that run once, on the constructor itself, when the class is defined.
    /// </summary>
    /// <remarks>
    /// <b>Static fields and static blocks share ONE list because their order is one order.</b>
    /// <c>class C { static a = 1; static { this.b = this.a } static c = 2 }</c> runs the three in
    /// the order written, and two lists would have had to be merged by something that knew where
    /// each element came from.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=35D9A1
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsClassElement>? StaticElements { get; set; }
}

/// <summary>
/// One recorded class element: what <c>DefineClassElement</c> stored and what applying it needs.
/// </summary>
/// <remarks>
/// <b>A record of an element and not the element itself</b>, which is the whole reason it exists.
/// A field's key is known when the class is defined and its value is not; a static block has a body
/// and no key at all. Holding the three things a class body settled early - the key, the function,
/// and which kind of element the flags say it is - is what lets the value be produced at the time
/// the language says it is produced rather than at the time the syntax was read.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F94DCE
// Broiler-Human:        PENDING
internal sealed class JsClassElement
{
    /// <summary>
    /// The property key, the private name, or <c>undefined</c> for a static block.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3EE4B3
    // Broiler-Human:        PENDING
    internal JsValue Key { get; init; }

    /// <summary>
    /// The initialiser to call, the method to install, or <c>undefined</c> for a field written with
    /// no initialiser.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0206C2
    // Broiler-Human:        PENDING
    internal JsValue Body { get; set; }

    /// <summary>The setter half, when this element is a private accessor that has one.</summary>
    /// <remarks>
    /// <b>One element carries both halves rather than the list carrying two elements.</b>
    /// <c>get #a</c> and <c>set #a</c> declare one private name, so the second half found merges
    /// into the first's record - and an instance then gets ONE element with two functions, which is
    /// what makes reading and writing <c>this.#a</c> reach the pair the class wrote.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=679B2F
    // Broiler-Human:        PENDING
    internal JsValue Setter { get; set; }

    /// <summary>The <c>DefineClassElement</c> operand bits this element was recorded with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=00F3FD
    // Broiler-Human:        PENDING
    internal byte Flags { get; set; }
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

    /// <summary>Removes one own property, refusing the ones a String wrapper synthesises.</summary>
    /// <remarks>
    /// <b>They are not in the map the base searches, and the base answers <see langword="true"/> for
    /// every key it does not find</b> - so without this, deleting <c>length</c> or a character index
    /// off a <c>new String("str")</c> reported success and deleted nothing, and the descriptor still
    /// answered afterwards. Both are non-configurable, so the honest answer is a refusal, which in
    /// strict code is the <c>TypeError</c> the language owes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B68C64
    // Broiler-Human:        PENDING
    internal override bool DeleteOwnProperty(string key)
    {
        if (Primitive.IsString)
        {
            var text = Primitive.AsString();

            if (string.Equals(key, "length", System.StringComparison.Ordinal) ||
                (IsArrayIndex(key, out var at) && at < text.Length))
            {
                return false;
            }
        }

        return base.DeleteOwnProperty(key);
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
