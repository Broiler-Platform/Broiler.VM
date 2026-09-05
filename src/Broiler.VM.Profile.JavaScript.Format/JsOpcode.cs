// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           118
// Human-reviewed:   0/23
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The instruction set of format version 2, at the <c>broiler.javascript.wide</c> surface.
/// </summary>
/// <remarks>
/// <para>
/// Version 1's set is arithmetic over three primitive types in one frame. This one is the set a
/// program with objects, functions, closures, exceptions and a standard library needs, and it is a
/// separate format version rather than an extension because every one of those needs a section
/// version 1 does not frame - a function table, an environment model and exception regions with a
/// scope depth in them.
/// </para>
/// <para>
/// The invariant version 1 established is kept: <b>one opcode byte followed by a fixed operand
/// width</b>, so an instruction boundary is computable from the opcode alone and the verifier can
/// build the boundary set without executing anything. Where version 1 wrote jump displacements
/// relative to the following instruction, this version writes <b>absolute code offsets</b>: all
/// functions share one code section, and an absolute target is checkable against the declared
/// range of the function that contains the jump without the verifier having to reconstruct where
/// the jump came from.
/// </para>
/// <para>
/// <b>The set is deliberately not complete JavaScript.</b> There is nothing a module declaration
/// needs; that is a construct the manifest refuses at the front end rather than an opcode the
/// executor would have to answer for. What is here is what a program that runs has to have.
/// </para>
/// <para>
/// <b><c>with</c> is TWO instructions and the rest of it is instructions that were already here.</b>
/// <see cref="PushObjectScope"/> puts an object environment record on the scope chain and
/// <see cref="ResolveName"/> asks the innermost few records, by name, which of them binds a name -
/// answering with the OBJECT rather than with the value. What that object is then read with, written
/// with or deleted from is <see cref="GetProperty"/>, <see cref="SetProperty"/> and
/// <see cref="DeleteProperty"/>, unchanged, so the verifier checks every step of a <c>with</c> body
/// except which object <see cref="ResolveName"/> answered with. The alternative - one instruction
/// that resolved a name and read it - would have hidden a property read the verifier can see, and
/// would have needed a fourth operand shape to carry a name and a branch target at once.
/// </para>
/// <para>
/// <b>Spread, destructuring and <c>for … of</c> ARE here, and each earned an opcode rather than a
/// lowering to the ones already present.</b> Every one of them either has a stack effect the
/// lowering cannot know statically - a spread contributes as many values as an iterator yields, and
/// the argument count of <c>f(...xs)</c> is therefore not a <c>u8</c> the encoder can write - or
/// performs an abstract operation with an observable protocol, which is what the four
/// <c>Iterate…</c> opcodes are. Lowering those to a call sequence would have meant synthesising
/// guest-visible functions to hold the protocol, and a guest could then reach them.
/// </para>
/// <para>
/// <b>A class is fourteen instructions and not a section.</b> <see cref="NewClass"/> builds the
/// object graph the specification's <c>ClassDefinitionEvaluation</c> builds,
/// <see cref="DefineMethod"/> attaches one member and gives it its home object, and
/// <see cref="LoadSuperProperty"/>, <see cref="StoreSuperProperty"/>, <see cref="SuperCall"/>,
/// <see cref="SuperCallSpread"/>, <see cref="SuperCallForwarded"/> and
/// <see cref="LoadNewTarget"/> are what a method and a derived constructor can ask that an
/// ordinary function cannot. Everything else a class needs -
/// the closures, the property definitions, the scope holding the class's own binding - is the
/// instructions that were already here, because a class is mostly an object graph and only partly
/// a new kind of frame.
/// </para>
/// <para>
/// <b>The class BODY cost six more, and every one of them exists because a class element happens
/// at a time the class definition is not.</b> <see cref="DefineClassElement"/> records a field, a
/// private method or a static block without performing it, and <see cref="RunStaticElements"/>
/// performs the static ones later, after the class binding exists - two instructions for what looks
/// like one step because the specification makes it two. <see cref="NewPrivateName"/> mints the
/// name a class evaluation declares, and <see cref="LoadPrivate"/>, <see cref="StorePrivate"/> and
/// <see cref="HasPrivate"/> are the three things a program may do with one. <b>None of the four
/// private instructions is a property instruction with a different key</b>: a private element is
/// stored beside an object's properties rather than in the same table, which is what keeps it out
/// of every reflection surface without any of them being told that private names exist, and reading
/// an absent one is an error where reading an absent property is <c>undefined</c>.
/// </para>
/// <para>
/// <b>The three suspension opcodes are NOT section 6's suspension targets.</b> Section 6 of the
/// format frames the core's own suspend-and-resume across the host boundary, which this profile's
/// verifier refuses outright. <see cref="Yield"/>, <see cref="YieldDelegate"/> and
/// <see cref="Await"/> suspend one guest invocation and resume it from inside the same interpreter,
/// never crossing that boundary - which is why they are ordinary instructions in this set and need
/// no section of their own. What differs between them is only WHO resumes: a generator's
/// resumption comes from the guest calling <c>next</c>, and an <c>await</c>'s comes from the job
/// queue the host drains.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6DC78A
// Broiler-Human:        PENDING
public enum JsOpcode : byte
{
    /// <summary>Does nothing. Never emitted; a hole in a patched stream would be a defect.</summary>
    Nop = 0x00,

    // ---- literals ------------------------------------------------------------------------------

    /// <summary>Push <c>undefined</c>.</summary>
    LoadUndefined = 0x01,

    /// <summary>Push <c>null</c>.</summary>
    LoadNull = 0x02,

    /// <summary>Push <c>true</c>.</summary>
    LoadTrue = 0x03,

    /// <summary>Push <c>false</c>.</summary>
    LoadFalse = 0x04,

    /// <summary>Push constant pool entry <c>u16</c>.</summary>
    LoadConstant = 0x05,

    /// <summary>Push the frame's <c>this</c> binding.</summary>
    LoadThis = 0x06,

    /// <summary>Push a fresh <c>arguments</c> object built from the frame's actual arguments.</summary>
    NewArguments = 0x07,

    /// <summary>
    /// Push the frame's <c>new.target</c>: the constructor a <c>new</c> reached this frame with, or
    /// <c>undefined</c> in an ordinary call.
    /// </summary>
    /// <remarks>
    /// <b>It is a FRAME value and not a property of anything the callee already has</b>, because
    /// the answer differs between two frames running the same code unit: a base constructor reached
    /// through <c>super()</c> answers with the DERIVED class, which is the whole reason the instance
    /// it creates gets the derived prototype. It is the one thing a function can observe about HOW
    /// it was entered rather than about what it was entered with. An arrow function has none of its
    /// own and answers with the one it closed over.
    /// </remarks>
    LoadNewTarget = 0x08,

    /// <summary>
    /// Push actual argument <c>u16</c>, or <c>undefined</c> when the caller passed none there.
    /// </summary>
    /// <remarks>
    /// <b>A frame whose parameter list is not simple binds its own parameters, and this is how it
    /// reads them.</b> The frame copies arguments straight into slots only while every parameter is
    /// one name with no initialiser; a default has to run code, a rest parameter has to build an
    /// Array, and a pattern has to destructure - none of which the frame's copy loop can do. Reading
    /// through a materialised <c>arguments</c> object instead would have been the alternative, and
    /// it is wrong twice over: it allocates an object per call for a function that never mentions
    /// one, and it makes a parameter's value depend on an object the body may have rewritten.
    /// </remarks>
    LoadArgument = 0x09,

    /// <summary>Push a dense Array of the actual arguments from index <c>u16</c> onward.</summary>
    RestArguments = 0x0A,

    // ---- bindings ------------------------------------------------------------------------------

    /// <summary>Push environment slot <c>u16</c>, <c>u8</c> scopes up. Throws on an uninitialised slot.</summary>
    LoadScoped = 0x10,

    /// <summary>Pop into environment slot <c>u16</c>, <c>u8</c> scopes up. Throws on an uninitialised slot.</summary>
    StoreScoped = 0x11,

    /// <summary>Pop into environment slot <c>u16</c>, <c>u8</c> scopes up, initialising it.</summary>
    InitialiseScoped = 0x12,

    /// <summary>Push the global property named by constant <c>u16</c>. Throws when it is absent.</summary>
    LoadGlobal = 0x13,

    /// <summary>Pop into the global property named by constant <c>u16</c>, creating it if absent.</summary>
    StoreGlobal = 0x14,

    /// <summary>Push <c>undefined</c> for an absent global rather than throwing. Backs <c>typeof</c>.</summary>
    LoadGlobalOrUndefined = 0x15,

    /// <summary>Push a fresh environment of <c>u16</c> slots whose parent is the current one.</summary>
    PushScope = 0x16,

    /// <summary>Discard the current environment and continue in its parent.</summary>
    PopScope = 0x17,

    /// <summary>Replace the current environment with a copy of it, for a per-iteration binding.</summary>
    CopyScope = 0x18,

    /// <summary>Define the global named by constant <c>u16</c> as <c>undefined</c> when absent.</summary>
    DeclareGlobal = 0x19,

    /// <summary>
    /// Pop a value and continue in an <b>object environment record</b> over <c>ToObject</c> of it,
    /// whose parent is the current environment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the same scope chain and a different KIND of record, which is why it costs one
    /// opcode rather than a second chain.</b> A record made here holds an object where every other
    /// record holds slots; <see cref="PopScope"/> discards it exactly as it discards a block's, and
    /// a <see cref="LoadScoped"/> whose hop count crosses it counts it as one hop like any other -
    /// so the lowering's static addressing keeps working through a <c>with</c> body rather than
    /// being suspended inside one.
    /// </para>
    /// <para>
    /// <b>The coercion is <c>ToObject</c> and it is part of the instruction.</b> <c>with (null)</c>
    /// and <c>with (undefined)</c> are a <c>TypeError</c>, and <c>with ("abc")</c> puts a String
    /// wrapper on the chain, so <c>length</c> resolves inside the body. Coercing in the lowering
    /// instead would have needed an opcode for <c>ToObject</c> that nothing else wants.
    /// </para>
    /// </remarks>
    PushObjectScope = 0x1A,

    /// <summary>
    /// Push the object environment record binding constant <c>u16</c> within the innermost <c>u8</c>
    /// records of the scope chain, or <c>undefined</c> when none of them binds it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It answers with the OBJECT and not with the value, and that is what keeps the rest of a
    /// <c>with</c> body ordinary.</b> The lowering follows it with the instructions a property
    /// access already uses - <see cref="GetProperty"/>, <see cref="SetProperty"/>,
    /// <see cref="DeleteProperty"/> - so a getter on the <c>with</c> object runs through the same
    /// path <c>o.x</c> runs through, and the verifier sees a property read where a program has one.
    /// It also answers the receiver question for free: a call whose callee came from an object
    /// environment record is made with THAT OBJECT as its <c>this</c>, and the object is the value
    /// this instruction already pushed.
    /// </para>
    /// <para>
    /// <b>The <c>u8</c> half is a SEARCH BOUND and it is what stops a name reaching a binding the
    /// language does not give it.</b> The lowering resolves every name statically first; the bound
    /// is the number of records between the reference and the binding it resolved to, so a record
    /// beyond that binding is never asked. Searching the whole chain instead would let an outer
    /// <c>with</c> shadow a declaration that already shadows it.
    /// </para>
    /// <para>
    /// <b>Only an object record can answer.</b> A declarative record holds a <c>JsValue</c> array
    /// and no names at all, so there is nothing in one for a search by name to match: a name this
    /// instruction does not find on an object falls through to the address the lowering computed,
    /// and to nothing else.
    /// </para>
    /// </remarks>
    ResolveName = 0x1B,

    // ---- objects and properties ------------------------------------------------------------------

    /// <summary>Push a fresh ordinary object with the realm's <c>Object.prototype</c>.</summary>
    NewObject = 0x20,

    /// <summary>Pop <c>u16</c> values and push an Array holding them in order.</summary>
    NewArray = 0x21,

    /// <summary>Pop a base; push its property named by constant <c>u16</c>.</summary>
    GetProperty = 0x22,

    /// <summary>Pop a value and a base; set the named property and push the value back.</summary>
    SetProperty = 0x23,

    /// <summary>Pop a key and a base; push the property.</summary>
    GetIndex = 0x24,

    /// <summary>Pop a value, a key and a base; set the property and push the value back.</summary>
    SetIndex = 0x25,

    /// <summary>Pop a value; define constant <c>u16</c> on the object beneath it, which stays.</summary>
    DefineField = 0x26,

    /// <summary>Pop a value and a key; define it on the object beneath them, which stays.</summary>
    DefineIndexed = 0x27,

    /// <summary>Pop a base; push whether deleting constant <c>u16</c> from it succeeded.</summary>
    DeleteProperty = 0x28,

    /// <summary>Pop a key and a base; push whether the deletion succeeded.</summary>
    DeleteIndex = 0x29,

    /// <summary>
    /// Pop a function; define it as the getter for constant <c>u16</c> on the object beneath.
    /// </summary>
    /// <remarks>
    /// <b>Superseded by <see cref="DefineMethod"/> and no longer emitted.</b> It cannot give the
    /// accessor a home object and it cannot take a computed key, so every accessor now goes
    /// through the one instruction that can do both. The opcode stays defined and the executor
    /// still answers for it, because an opcode number in a published format version is not reused
    /// and an artifact produced before the change is still an artifact this reader must verify.
    /// </remarks>
    DefineGetter = 0x2A,

    /// <summary>Pop a function; define it as the setter for constant <c>u16</c> on the object beneath.</summary>
    /// <remarks><inheritdoc cref="DefineGetter" path="/remarks"/></remarks>
    DefineSetter = 0x2B,

    /// <summary>
    /// Pop a function and a key; define the member on the object beneath, which stays.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>u8</c> operand carries <see cref="JsOpcodes.MemberIsGetter"/>,
    /// <see cref="JsOpcodes.MemberIsSetter"/> and <see cref="JsOpcodes.MemberIsEnumerable"/>. The
    /// first two are exclusive and a member with both is refused by the verifier rather than
    /// resolved by precedence.
    /// </para>
    /// <para>
    /// <b>Defining a method is also what gives it its home object</b>, and that is why this is one
    /// instruction rather than a definition followed by a store. A method's <c>super</c> starts
    /// from the object the method was DEFINED on and never from the receiver a call site supplies,
    /// so a method extracted from a prototype and called against a stranger still reaches the same
    /// <c>super</c> - and an encoding in which the two steps could be separated would let a
    /// verified artifact carry a method whose home object was some other object.
    /// </para>
    /// </remarks>
    DefineMethod = 0x2C,

    /// <summary>
    /// Pop a key; push what the active function's home object inherits under it, read with the
    /// frame's <c>this</c> as the receiver.
    /// </summary>
    LoadSuperProperty = 0x2D,

    /// <summary>
    /// Pop a value and a key; assign through the home object's prototype chain onto the frame's
    /// <c>this</c>, and push the value back.
    /// </summary>
    /// <remarks>
    /// The lookup starts at the home object's prototype and the WRITE lands on <c>this</c>, which
    /// is the pair of facts that makes <c>super.x = 1</c> different from both <c>this.x = 1</c>
    /// and a write to the prototype: an inherited setter runs with <c>this</c> as its receiver,
    /// and an inherited data property is shadowed rather than replaced.
    /// </remarks>
    StoreSuperProperty = 0x2E,

    /// <summary>
    /// Pop a value and define it at the Array beneath's current <c>length</c>, which grows.
    /// </summary>
    /// <remarks>
    /// <b>An element's index stops being a compile-time constant the moment a literal spreads.</b>
    /// <c>[a, ...xs, b]</c> puts <c>b</c> wherever <c>xs</c> ended, so the index has to be read off
    /// the Array rather than counted by the encoder - which is what <see cref="DefineIndexed"/>
    /// with a constant would have done.
    /// </remarks>
    ArrayAppend = 0x2F,

    // ---- functions and calls ---------------------------------------------------------------------

    /// <summary>Push a function object over code unit <c>u16</c>, closing over the current environment.</summary>
    Closure = 0x30,

    /// <summary>Pop <c>u8</c> arguments, a receiver and a callee; push the result.</summary>
    Call = 0x31,

    /// <summary>Pop <c>u8</c> arguments and a constructor; push the constructed object.</summary>
    Construct = 0x32,

    /// <summary>
    /// Pop <c>u8</c> arguments, a receiver and a callee, where the callee expression was the bare
    /// name <c>eval</c>; push the result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It exists because the difference between a direct and an indirect <c>eval</c> is a fact
    /// about the CALL SITE and nothing else.</b> <c>eval(s)</c> and <c>(0, eval)(s)</c> reach the
    /// same function object with the same receiver and the same arguments; what differs is that the
    /// first is spelled as a call to that name and therefore, in the language, evaluates in the
    /// caller's scope. No executor can recover that from the operand stack, so the lowering has to
    /// say it, and an opcode is how the lowering says something the verifier can also check.
    /// </para>
    /// <para>
    /// <b>It is not a promise that the callee IS <c>eval</c>.</b> A program may assign to the
    /// global, and this instruction is emitted from the spelling rather than from the value; the
    /// executor compares the callee against the intrinsic and performs an ordinary call when they
    /// differ. Its stack effect is <see cref="Call"/>'s exactly, which is what lets a verifier that
    /// knows nothing about <c>eval</c> check it.
    /// </para>
    /// </remarks>
    CallEval = 0x35,

    /// <summary>Pop one value and return it from the current frame.</summary>
    Return = 0x33,

    /// <summary>Return <c>undefined</c> from the current frame.</summary>
    ReturnUndefined = 0x34,

    /// <summary>
    /// Pop the constructor - and the heritage beneath it when the <c>u8</c> operand carries
    /// <see cref="JsOpcodes.ClassIsDerived"/> - and push the class.
    /// </summary>
    /// <remarks>
    /// <b>One instruction because the graph it builds is not separable.</b> A class is a
    /// constructor whose <c>prototype</c> is a fresh object that is not writable, not enumerable
    /// and not configurable; whose prototype's <c>constructor</c> points back and is not
    /// enumerable; whose own <c>[[Prototype]]</c> is the superclass rather than
    /// <c>Function.prototype</c>; and whose home object is that same fresh prototype. Emitting
    /// those as separate property definitions would leave a verified artifact able to describe a
    /// half-built class - one whose <c>prototype</c> is writable, say - which is a shape the
    /// language has no way to make and this executor would then have to answer for.
    /// </remarks>
    NewClass = 0x38,

    /// <summary>
    /// Pop <c>u8</c> arguments; construct the superclass with this frame's <c>new.target</c>, bind
    /// the result as this frame's <c>this</c>, and push it.
    /// </summary>
    /// <remarks>
    /// It throws a <c>ReferenceError</c> when this frame's <c>this</c> is already bound, which is
    /// what makes calling <c>super()</c> twice an error rather than a second construction.
    /// </remarks>
    SuperCall = 0x36,

    /// <summary>
    /// As <see cref="SuperCall"/>, with this frame's own actual arguments rather than a count of
    /// them from the operand stack.
    /// </summary>
    /// <remarks>
    /// <b>This exists for the implicit constructor of a derived class and for nothing else.</b>
    /// That constructor is <c>constructor(...args) { super(...args); }</c>, and although the
    /// surface now spells both halves of that, lowering it that way would have made a class with
    /// no constructor allocate a rest Array on every construction and would have given it a
    /// declared parameter count of one. <b>The forwarding is the instruction because the implicit
    /// constructor has no source text to lower</b>: its declared parameter count is zero, which is
    /// what the language reports for the length of an implicit derived constructor.
    /// </remarks>
    SuperCallForwarded = 0x37,

    // ---- spread ------------------------------------------------------------------------------

    /// <summary>Extend the Array on top of the stack by <c>u16</c> holes, defining nothing.</summary>
    ArrayHoles = 0x39,

    /// <summary>Pop an iterable; append everything it yields to the Array beneath, which stays.</summary>
    SpreadArray = 0x3A,

    /// <summary>
    /// Pop a source; copy its own enumerable properties onto the object beneath, which stays.
    /// </summary>
    /// <remarks>
    /// <b>This is <c>CopyDataProperties</c> and NOT the iteration protocol.</b> Object spread reads
    /// properties and array spread iterates, which is why they are two opcodes rather than one with
    /// a mode: <c>{...[1,2]}</c> is an object with the keys <c>0</c> and <c>1</c>, and
    /// <c>{...null}</c> is an empty object rather than a <c>TypeError</c>.
    /// </remarks>
    SpreadObject = 0x3B,

    /// <summary>Pop an Array of arguments, a receiver and a callee; push the result.</summary>
    /// <remarks>
    /// <b><see cref="Call"/> cannot express <c>f(...xs)</c> and no widening of its operand would
    /// help</b>, because the count is not known until <c>xs</c> has been iterated. So the arguments
    /// arrive as one Array the lowering built with <see cref="ArrayAppend"/> and
    /// <see cref="SpreadArray"/>, and the stack effect is fixed again.
    /// </remarks>
    CallSpread = 0x3C,

    /// <summary>Pop an Array of arguments and a constructor; push the constructed object.</summary>
    ConstructSpread = 0x3D,

    /// <summary>
    /// As <see cref="SuperCall"/>, with the arguments arriving as one Array rather than as a count
    /// of them on the operand stack.
    /// </summary>
    /// <remarks>
    /// <b>It is <see cref="SuperCall"/>'s answer to what <see cref="CallSpread"/> answers for
    /// <see cref="Call"/>, and it exists because the two families MEET.</b> <c>super(...args)</c>
    /// is the composition of two constructs this manifest admits separately, and neither of the
    /// instructions that admit them can express it: <see cref="SuperCall"/>'s argument count is a
    /// <c>u8</c> the encoder cannot know once a spread is in the list, and
    /// <see cref="CallSpread"/> takes a callee and a receiver from the stack where a super call
    /// takes both from the frame. Refusing the composition would have been a surface that admits
    /// each half and not the whole.
    /// </remarks>
    SuperCallSpread = 0x3E,

    /// <summary>
    /// Pop a value; make it the prototype of the object beneath, which stays. A value that is
    /// neither an object nor <c>null</c> is discarded and the object is left alone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b><c>{ __proto__: p }</c> is not a property definition and cannot be lowered as one.</b> The
    /// language spells one member of an object literal with a name and gives it an entirely
    /// different meaning: it sets the object's prototype rather than defining a key. Lowering it to
    /// <see cref="DefineField"/> makes an own property called <c>__proto__</c>, which is observable
    /// as a wrong answer from <c>Object.keys</c>, from <c>JSON.stringify</c>, and from every
    /// prototype the literal was supposed to have.
    /// </para>
    /// <para>
    /// <b>It is an instruction rather than a store through the accessor of the same name</b>,
    /// because the two are not the same operation. The accessor lives on
    /// <c>Object.prototype</c>, so a program that deletes it, or a literal that spread an own
    /// <c>__proto__</c> onto itself first, would change what the literal means; the language says
    /// the literal form sets the prototype directly and answers to nothing on the chain.
    /// </para>
    /// <para>
    /// <b>The shorthand does not reach this opcode.</b> <c>{ __proto__ }</c> defines a property, and
    /// only the <c>name: value</c> and <c>"name": value</c> forms set the prototype — a distinction
    /// the lowering makes, since by here the two would look alike.
    /// </para>
    /// </remarks>
    SetPrototypeLiteral = 0x3F,

    // ---- operators ---------------------------------------------------------------------------------

    /// <summary>The <c>+</c> operator, which concatenates when either operand is a String.</summary>
    Add = 0x40,

    /// <summary>The <c>-</c> operator.</summary>
    Subtract = 0x41,

    /// <summary>The <c>*</c> operator.</summary>
    Multiply = 0x42,

    /// <summary>The <c>/</c> operator.</summary>
    Divide = 0x43,

    /// <summary>The <c>%</c> operator, whose sign follows the dividend.</summary>
    Remainder = 0x44,

    /// <summary>The <c>**</c> operator.</summary>
    Exponent = 0x45,

    /// <summary>Unary <c>-</c>.</summary>
    Negate = 0x46,

    /// <summary>Unary <c>+</c>, which is <c>ToNumber</c> and not a no-op.</summary>
    ToNumber = 0x47,

    /// <summary>Unary <c>!</c>.</summary>
    Not = 0x48,

    /// <summary>Unary <c>~</c>.</summary>
    BitwiseNot = 0x49,

    /// <summary>The <c>&lt;</c> comparison.</summary>
    LessThan = 0x4A,

    /// <summary>The <c>&lt;=</c> comparison.</summary>
    LessThanOrEqual = 0x4B,

    /// <summary>The <c>&gt;</c> comparison.</summary>
    GreaterThan = 0x4C,

    /// <summary>The <c>&gt;=</c> comparison.</summary>
    GreaterThanOrEqual = 0x4D,

    /// <summary>The <c>===</c> comparison.</summary>
    StrictEquals = 0x4E,

    /// <summary>The <c>!==</c> comparison.</summary>
    StrictNotEquals = 0x4F,

    /// <summary>The <c>==</c> comparison.</summary>
    LooseEquals = 0x50,

    /// <summary>The <c>!=</c> comparison.</summary>
    LooseNotEquals = 0x51,

    /// <summary>The <c>|</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseOr = 0x52,

    /// <summary>The <c>&amp;</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseAnd = 0x53,

    /// <summary>The <c>^</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseXor = 0x54,

    /// <summary>The <c>&lt;&lt;</c> operator.</summary>
    ShiftLeft = 0x55,

    /// <summary>The <c>&gt;&gt;</c> operator.</summary>
    ShiftRight = 0x56,

    /// <summary>The <c>&gt;&gt;&gt;</c> operator, over <c>ToUint32</c>.</summary>
    ShiftRightUnsigned = 0x57,

    /// <summary>Pop one; push the <c>typeof</c> string for it.</summary>
    TypeOf = 0x58,

    /// <summary>The <c>instanceof</c> operator.</summary>
    InstanceOf = 0x59,

    /// <summary>The <c>in</c> operator.</summary>
    In = 0x5A,

    /// <summary>Pop one and push <c>undefined</c>. The <c>void</c> operator.</summary>
    Void = 0x5B,

    /// <summary>
    /// Throw a <c>TypeError</c> naming constant <c>u16</c> when the top of the stack is
    /// <c>null</c> or <c>undefined</c>; otherwise leave it alone.
    /// </summary>
    /// <remarks>
    /// <b><c>RequireObjectCoercible</c>, and it exists because an EMPTY object pattern still
    /// checks.</b> <c>var {a} = x</c> gets the check for free from the property read, but
    /// <c>var {} = x</c> reads nothing and must still refuse <c>undefined</c>. Constructing the
    /// error from the global <c>TypeError</c> instead would have let a guest that reassigned that
    /// name decide what a destructuring failure throws.
    /// </remarks>
    RequireCoercible = 0x5C,

    // ---- control flow ---------------------------------------------------------------------------------

    /// <summary>Continue at absolute code offset <c>u32</c>.</summary>
    Jump = 0x60,

    /// <summary>Pop one; jump to <c>u32</c> when <c>ToBoolean</c> of it is false.</summary>
    JumpIfFalse = 0x61,

    /// <summary>Pop one; jump to <c>u32</c> when <c>ToBoolean</c> of it is true.</summary>
    JumpIfTrue = 0x62,

    /// <summary>Pop one and throw it as a JavaScript exception.</summary>
    Throw = 0x63,

    /// <summary>Pop an object; push an enumerator over its <c>for…in</c> property names.</summary>
    ForInStart = 0x64,

    /// <summary>
    /// Pop an enumerator; push its next name, or jump to <c>u32</c> when it is exhausted.
    /// </summary>
    /// <remarks>
    /// <b>It pops rather than peeks, and the enumerator lives in a scope slot between turns.</b>
    /// Keeping it on the operand stack across the loop body was the obvious encoding and it is
    /// wrong: a <c>break</c> out of the loop would leave it behind, so the break target is reached
    /// at two different heights, and a <c>return</c> from inside the body would reach
    /// <see cref="Return"/> with two values on the stack. Both are refused by the verifier, which
    /// is the good outcome - but the code they refuse is code a correct program produced.
    /// </remarks>
    ForInNext = 0x65,

    /// <summary>Pop an iterable; push the iterator record <c>GetIterator</c> answers with.</summary>
    /// <remarks>
    /// The record is a value on this stack rather than a hidden register because the same lowering
    /// has to hold two of them at once - <c>for (const [a] of pairs)</c> destructures each element
    /// while the loop's own iterator is still live.
    /// </remarks>
    IterateStart = 0x66,

    /// <summary>
    /// Pop an iterator record; push the next value, or jump to <c>u32</c> when it is exhausted.
    /// </summary>
    /// <remarks>
    /// The same two-effect shape <see cref="ForInNext"/> has, and for the same reason: on the taken
    /// branch nothing arrives, so the target is one below this instruction's height. An exhausted
    /// record is marked done, which is what makes a later <see cref="IterateClose"/> a no-op - the
    /// specification does not call <c>return</c> on an iterator that already said it was finished.
    /// </remarks>
    IterateNext = 0x67,

    /// <summary>Pop an iterator record; push a dense Array of everything it has left.</summary>
    /// <remarks>
    /// The record is left done, because a rest element consumes the iterator and the pattern that
    /// contains it must not then close it.
    /// </remarks>
    IterateRest = 0x68,

    /// <summary>
    /// Pop an iterator record and perform <c>IteratorClose</c>, unless it is already done.
    /// </summary>
    /// <remarks>
    /// <b>The <c>u8</c> operand says which completion is closing it, and the difference is
    /// observable.</b> Zero is a normal or a <c>break</c>-shaped completion: an error from
    /// <c>return</c> propagates, and a <c>return</c> that answers a non-object is itself a
    /// <c>TypeError</c>. Anything else is a throw completion: whatever <c>return</c> does is
    /// discarded, because the exception already in flight is the one the program is owed.
    /// </remarks>
    IterateClose = 0x69,

    // ---- suspension -------------------------------------------------------------------------------------

    /// <summary>
    /// Pop one value, suspend the frame yielding it, and push what the resumption sent.
    /// </summary>
    /// <remarks>
    /// <b>Its stack effect is net zero, and the pushed value comes from OUTSIDE the unit.</b> Nothing
    /// in the instruction stream put the resume value there - <c>next(v)</c> did, from the host side
    /// of the generator object - so a reader tracing the code will not find its producer. It is
    /// counted as one pop and one push anyway, because that is what the frame's operand stack does
    /// across the suspension, and the verifier's abstract height has to agree with it at the
    /// instruction after.
    /// </remarks>
    Yield = 0x6A,

    /// <summary>
    /// Pop an iterable, drive it to exhaustion yielding each value, and push the iterator's own
    /// return value. The <c>yield*</c> operator.
    /// </summary>
    /// <remarks>
    /// <b>One opcode rather than a lowered loop, because the loop has to survive an abrupt
    /// resumption.</b> <c>return</c> and <c>throw</c> arriving while the delegation is suspended are
    /// forwarded to the inner iterator, and a lowering would have to catch each of them at a
    /// <see cref="Yield"/> it emitted - which the exception regions of this format cannot express
    /// without a handler kind for "resumed abruptly". Its stack effect is one pop and one push, the
    /// same as <see cref="Yield"/>, and for the same reason.
    /// </remarks>
    YieldDelegate = 0x6B,

    /// <summary>
    /// Pop one value, suspend the frame on it, and push what the resumption sent. The
    /// <c>await</c> operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Its stack effect is <see cref="Yield"/>'s exactly, which is why one resumption path
    /// serves both.</b> What differs is entirely outside the operand stack: a <c>yield</c> hands
    /// its value to whoever called <c>next</c>, and an <c>await</c> hands its value to
    /// <c>PromiseResolve</c> and registers the frame's own continuation as that promise's reaction.
    /// The frame that comes back is the same frame either way. The two happen to be the same WIDTH
    /// as well, and the resumption deliberately does not rely on that: it steps past the
    /// instruction actually at the pointer, so a suspension with an operand could be added without
    /// silently resuming one byte inside it.
    /// </para>
    /// <para>
    /// <b>It is a SEPARATE opcode from <see cref="Yield"/> rather than a flag on it</b>, because
    /// the verifier's answer differs: a suspension is admitted only in a unit whose flag says the
    /// executor allocated it a heap frame, and the two flags are different bits naming two
    /// different drivers. One opcode with a mode operand would have made a unit flagged
    /// <c>Async</c> able to encode a <c>yield</c>, which no driver of this profile can resume.
    /// </para>
    /// </remarks>
    Await = 0x6C,

    // ---- the class body -------------------------------------------------------------------------

    /// <summary>
    /// Pop an initialiser and a key; record one class element on the constructor two below them,
    /// which stays with the home object above it. The operand is a bit set of
    /// <see cref="JsOpcodes.ElementBits"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It RECORDS a class element and does not define anything yet</b>, and the delay is the
    /// whole reason it exists. A field's key is evaluated once, when the class is; its initialiser
    /// runs once per instance, later, with a <c>this</c> that does not exist while the class body
    /// is being evaluated. A static element's key is evaluated with the other keys, in body order,
    /// and its initialiser runs after every member has been defined and the class binding
    /// initialised — so <c>class C { static [k()] = C.name }</c> sees a <c>C</c> that a definition
    /// performed in place could not. An instruction that defined the property here would have had
    /// to choose one of the two times and would have been wrong at the other.
    /// </para>
    /// <para>
    /// <b>It reads TWO values under the key rather than one, and that is deliberate.</b> The
    /// constructor is where every element is recorded — the specification puts <c>[[Fields]]</c> on
    /// the constructor — and the home object is what the initialiser's <c>super</c> reads through,
    /// which is the PROTOTYPE for an instance element and the constructor for a static one. The
    /// class lowering already holds exactly that pair on the stack while it defines members, so
    /// naming both costs nothing; the alternative — one host value, with the other reached through
    /// <c>constructor.prototype</c> — would have gone through a property a guest can watch.
    /// </para>
    /// <para>
    /// <b>A static block reaches the executor as this instruction too.</b> It has no key, so the
    /// lowering pushes <c>undefined</c> for one and sets <see cref="JsOpcodes.ElementIsBlock"/>;
    /// everything else about it — that it runs at class-definition time, in body order, with
    /// <c>this</c> as the constructor — is a static field's behaviour exactly, and the ordered list
    /// the two share is what makes them interleave the way the source wrote them.
    /// </para>
    /// </remarks>
    DefineClassElement = 0x6F,

    /// <summary>
    /// Push a private name that no other evaluation of this instruction has produced, described by
    /// constant <c>u16</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A private name is created per CLASS EVALUATION and not per class body</b>, which is why
    /// it is an instruction rather than a constant-pool entry. Two evaluations of one class
    /// expression declare two unrelated <c>#x</c>: an instance of the first is not branded by the
    /// second, and <c>#x in o</c> asked from inside the second must answer <c>false</c> for it. A
    /// pooled constant would have made the two the same name and the brand check meaningless.
    /// </para>
    /// <para>
    /// <b>What it pushes is held in an ordinary scope slot, and that is what a method closes over.</b>
    /// The specification's PrivateEnvironment is a scope chain of exactly this shape, so this
    /// profile needs no second environment kind: the class scope holds one slot per declared
    /// private name, and every method, field initialiser and static block in the body captures it
    /// the way it captures the class's own binding.
    /// </para>
    /// <para>
    /// <b>No guest expression can reach the value.</b> The lowering emits a load of one of those
    /// slots only immediately before <see cref="LoadPrivate"/>, <see cref="StorePrivate"/>,
    /// <see cref="HasPrivate"/> or <see cref="DefineClassElement"/>, and the slot's name begins
    /// with <c>#</c>, which the front end never produces for an identifier expression. That is what
    /// keeps a private name out of guest hands without giving the value representation an eighth
    /// type.
    /// </para>
    /// </remarks>
    NewPrivateName = 0x75,

    /// <summary>
    /// Pop a private name and an object; push the private element's value, or throw a
    /// <c>TypeError</c> when the object carries no element of that name.
    /// </summary>
    /// <remarks>
    /// <b>The absent case throws where <see cref="GetProperty"/> answers <c>undefined</c></b>, and
    /// that is the whole difference between the two instructions. A private name is not a key an
    /// arbitrary object could have: an object that does not carry it was not constructed by the
    /// class that declared it, and the language reports that as an error rather than as a missing
    /// property, so a brand check has something to be the alternative to.
    /// </remarks>
    LoadPrivate = 0x76,

    /// <summary>
    /// Pop a value, a private name and an object; write the private element and push the value
    /// back. Throws a <c>TypeError</c> when the element is absent, is a method, or is an accessor
    /// with no setter.
    /// </summary>
    /// <remarks>
    /// <b>A private method is not writable and the refusal is a <c>TypeError</c> in every mode</b>,
    /// sloppy included. An ordinary non-writable property assignment is silent outside strict code;
    /// this one never is, because the class body a private name can be written in is strict code by
    /// definition and there is no reading of the assignment that could succeed.
    /// </remarks>
    StorePrivate = 0x77,

    /// <summary>
    /// Pop a private name and an object; push whether the object carries an element of that name.
    /// </summary>
    /// <remarks>
    /// <b>This is <c>#x in o</c>, and it is the ONLY instruction of the three that asks the
    /// question without also demanding an answer.</b> It is what a program uses to find out whether
    /// <see cref="LoadPrivate"/> would throw. <b>It still throws for a non-object</b>, which is the
    /// one thing about it a reader guesses wrong: the form is the <see cref="In"/> operator with a
    /// name the grammar spells differently, and <c>"x" in 5</c> is a <c>TypeError</c> too, so
    /// answering <c>false</c> there would have made the private form the more permissive of the two.
    /// </remarks>
    HasPrivate = 0x78,

    /// <summary>
    /// Run the static elements recorded on the constructor at the top of the stack, in the order
    /// they were recorded, with <c>this</c> bound to it. The constructor stays.
    /// </summary>
    /// <remarks>
    /// <b>It is a separate instruction from the ones that recorded them because the two happen at
    /// different points of the class's own evaluation.</b> Every key in the body is evaluated
    /// first, in source order; then the class binding is initialised; and only then do the static
    /// initialisers and blocks run — so a static block may name the class, and a computed key may
    /// not. Running each element where it was recorded would have collapsed those three steps into
    /// one and made <c>class C { static [C.name] = 1 }</c> succeed, which the language says is a
    /// reference to a binding still in its dead zone.
    /// </remarks>
    RunStaticElements = 0x79,

    // ---- asynchronous iteration -------------------------------------------------------------------------
    //
    // FIVE INSTRUCTIONS AND NOT ONE, BECAUSE AN `await` HAS TO STAND BETWEEN THEM. A `for await`
    // head calls `next`, awaits what it answered, and only then asks whether the iteration is done -
    // and the await is a SUSPENSION, which leaves the dispatch loop entirely. A single instruction
    // that did the whole step would have had to suspend in the middle of itself, which this
    // profile's one frame cannot express; splitting at each suspension point is what lets the
    // existing `Await` do the awaiting and the frame come back at an instruction boundary.

    /// <summary>
    /// Pop an iterable; push the iterator record <c>GetIterator(obj, ~async~)</c> answers with.
    /// </summary>
    /// <remarks>
    /// <b>It is a separate opcode from <see cref="IterateStart"/> rather than an operand on it,
    /// because the two read different Symbols and one of them may build an object.</b> This reads
    /// <c>Symbol.asyncIterator</c> and, when the value has none, wraps the SYNCHRONOUS iterator in
    /// an <c>%AsyncFromSyncIteratorPrototype%</c> object so that every value the wrapper answers has
    /// already been awaited. A flag on <see cref="IterateStart"/> would have made one instruction
    /// able to allocate or not depending on a byte, which is exactly the kind of instruction a
    /// reader of the lowering mis-reads.
    /// </remarks>
    IterateStartAsync = 0x7A,

    /// <summary>
    /// Read the async iterator record on top; push what its <c>next</c> answered, unawaited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What it pushes is the RAW answer and not a value</b>: an async iterator's <c>next</c>
    /// answers a promise, and the instruction that follows this one is the <see cref="Await"/> that
    /// resolves it. Nothing here reads <c>done</c> or <c>value</c>, because neither exists yet.
    /// </para>
    /// <para>
    /// <b>THE RECORD STAYS UNDERNEATH, and that is what carries it across the suspension.</b>
    /// <see cref="IterateAwaitStep"/> needs it - a step that has a value un-marks the record so that
    /// an abrupt exit from the body closes it - and the frame's operand stack is exactly where a
    /// value survives an <c>await</c>. Popping it here and re-reading it from the loop's slot would
    /// have worked and would have said, wrongly, that the record and the step are unrelated.
    /// </para>
    /// </remarks>
    IterateNextAsync = 0x7B,

    /// <summary>
    /// Pop an awaited iteration result and the record under it; push the result's <c>value</c>, or
    /// jump to <c>u32</c> when it is done.
    /// </summary>
    /// <remarks>
    /// The two-effect shape <see cref="IterateNext"/> has, one value wider: it consumes two and
    /// leaves one on the fall-through and none on the branch, so the target is TWO below this
    /// instruction's height. A result that is not an object is a <c>TypeError</c> here rather than a
    /// silent completion, which is what the specification asks of a <c>for await</c> head whose
    /// iterator answered a primitive.
    /// </remarks>
    IterateAwaitStep = 0x7C,

    /// <summary>
    /// Pop an async iterator record; push what its <c>return</c> answered, or jump to <c>u32</c>
    /// when there was nothing to call.
    /// </summary>
    /// <remarks>
    /// <b>The jump is what keeps an iterator with no <c>return</c> from spending a turn of the job
    /// queue.</b> <c>AsyncIteratorClose</c> awaits the result of <c>return</c> and returns
    /// immediately when there is no <c>return</c> to call - so a lowering that awaited
    /// unconditionally would have added a microtask tick to every <c>break</c> out of a
    /// <c>for await</c> over an iterator that defines none, which a program interleaving with
    /// <c>then</c> can count.
    /// </remarks>
    IterateCloseAsync = 0x7D,

    /// <summary>
    /// Pop an awaited close result and require it to be an object.
    /// </summary>
    /// <remarks>
    /// It is the last step of <c>AsyncIteratorClose</c> under a normal or <c>break</c>-shaped
    /// completion, and it is a separate instruction because the value it checks arrives from the
    /// <see cref="Await"/> before it rather than from the close. Under a THROW completion the
    /// lowering does not emit it at all: the specification discards everything the close did,
    /// including this check, because the exception already in flight is the one the program is owed.
    /// </remarks>
    IterateCloseCheck = 0x7E,

    // ---- stack ------------------------------------------------------------------------------------------

    /// <summary>Pop one and discard it.</summary>
    Pop = 0x70,

    /// <summary>Push a second copy of the top of the stack.</summary>
    Duplicate = 0x71,

    /// <summary>Push copies of the top two values, in order.</summary>
    DuplicateTwo = 0x72,

    /// <summary>Exchange the top two values.</summary>
    Swap = 0x73,

    /// <summary>Push a copy of the value <c>u8</c> places below the top.</summary>
    Pick = 0x74,
}

/// <summary>The operand shape that follows an opcode byte.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A52162
// Broiler-Human:        PENDING
public enum JsOperandShape : byte
{
    /// <summary>No operand.</summary>
    None = 0,

    /// <summary>One unsigned byte.</summary>
    U8 = 1,

    /// <summary>One little-endian unsigned 16-bit integer.</summary>
    U16 = 2,

    /// <summary>One little-endian unsigned 32-bit code offset.</summary>
    U32 = 3,

    /// <summary>An unsigned byte followed by a little-endian unsigned 16-bit integer.</summary>
    U8U16 = 4,
}

/// <summary>
/// What the verifier, the encoder and the executor must all agree about a version-2 opcode.
/// </summary>
/// <remarks>
/// One table, in the format assembly, for the same reason version 1 has one: the executor and the
/// lowering must agree about the bytecode and neither may depend on the other. A stack effect that
/// varies with an operand - a call's argument count, an array literal's element count - is reported
/// through <see cref="TryDescribe"/> rather than through a pair of constants, because a constant
/// that is right for one operand and wrong for another is worse than no constant.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=32AB60
// Broiler-Human:        PENDING
public static class JsOpcodes
{
    /// <summary>The <see cref="JsOpcode.NewClass"/> operand bit that says the class has a heritage.</summary>
    /// <remarks>
    /// It changes the instruction's stack effect - a derived class pops its superclass as well as
    /// its constructor - which is why it is an operand bit rather than a second opcode: the
    /// verifier reads the effect from the operand it is looking at, and a pair of opcodes would
    /// have said the same thing twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=144C80
    // Broiler-Human:        PENDING
    public const byte ClassIsDerived = 1;

    /// <summary>The <see cref="JsOpcode.DefineMethod"/> operand bit for a getter.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=668250
    // Broiler-Human:        PENDING
    public const byte MemberIsGetter = 1;

    /// <summary>The <see cref="JsOpcode.DefineMethod"/> operand bit for a setter.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=3BA507
    // Broiler-Human:        PENDING
    public const byte MemberIsSetter = 2;

    /// <summary>
    /// The <see cref="JsOpcode.DefineMethod"/> operand bit that makes the member enumerable.
    /// </summary>
    /// <remarks>
    /// An object literal's members are enumerable and a class's are not, and that difference is
    /// the whole of why the bit exists: <c>for…in</c> over an instance must not reach the class's
    /// methods, and <c>Object.keys</c> over an object literal must reach its.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FB6D81
    // Broiler-Human:        PENDING
    public const byte MemberIsEnumerable = 4;

    /// <summary>Every operand bit <see cref="JsOpcode.DefineMethod"/> defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=CF6964
    // Broiler-Human:        PENDING
    public const byte MemberBits = MemberIsGetter | MemberIsSetter | MemberIsEnumerable;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit that puts the element on the constructor
    /// rather than on every instance.
    /// </summary>
    /// <remarks>
    /// It decides two things at once and they always agree: WHERE the element lands, and WHICH of
    /// the constructor's two ordered lists records it — the instance list the executor replays for
    /// every construction, or the static list <see cref="JsOpcode.RunStaticElements"/> replays
    /// exactly once. It also selects the home object, which is the constructor for a static
    /// element and the prototype for an instance one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7AB77D
    // Broiler-Human:        PENDING
    public const byte ElementIsStatic = 1;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit that says the key is a private name.
    /// </summary>
    /// <remarks>
    /// <b>It is not a spelling of the key and it changes where the element is stored.</b> A private
    /// element goes in a table beside the object's properties and not in it, which is what keeps it
    /// out of <c>Object.keys</c>, <c>Reflect.ownKeys</c>, <c>JSON.stringify</c> and <c>for…in</c>
    /// without any of those having to know that private names exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=248820
    // Broiler-Human:        PENDING
    public const byte ElementIsPrivate = 2;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit for a <c>static { … }</c> block, whose key
    /// is nothing and whose body is called for its effect.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=D2B1B3
    // Broiler-Human:        PENDING
    public const byte ElementIsBlock = 4;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit that says the pushed value IS the element
    /// rather than a function producing it.
    /// </summary>
    /// <remarks>
    /// A field's initialiser is called once per instance and a private method is installed on every
    /// instance unchanged, so without this bit the executor would call the method itself and
    /// install whatever it returned. It also makes the element non-writable, which is what makes
    /// <c>this.#m = 1</c> a <c>TypeError</c> rather than a replacement.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5E4168
    // Broiler-Human:        PENDING
    public const byte ElementIsMethod = 8;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit for the getter half of a private accessor.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F4107F
    // Broiler-Human:        PENDING
    public const byte ElementIsGetter = 16;

    /// <summary>
    /// The <see cref="JsOpcode.DefineClassElement"/> bit for the setter half of a private accessor.
    /// </summary>
    /// <remarks>
    /// <b>The two halves are two instructions and one element.</b> <c>get #a</c> and <c>set #a</c>
    /// written in one body declare ONE private name with two functions on it, so the second
    /// instruction merges into the element the first recorded rather than replacing it — which is
    /// why the halves are separate bits and not one accessor bit with a pair on the stack.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7521A3
    // Broiler-Human:        PENDING
    public const byte ElementIsSetter = 32;

    /// <summary>Every operand bit <see cref="JsOpcode.DefineClassElement"/> defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A69539
    // Broiler-Human:        PENDING
    public const byte ElementBits = ElementIsStatic | ElementIsPrivate | ElementIsBlock |
        ElementIsMethod | ElementIsGetter | ElementIsSetter;

    /// <summary>Every opcode format version 2 defines, in ascending numeric order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F3CD57
    // Broiler-Human:        PENDING
    public static readonly JsOpcode[] All =
    [
        JsOpcode.Nop,
        JsOpcode.LoadUndefined, JsOpcode.LoadNull, JsOpcode.LoadTrue, JsOpcode.LoadFalse,
        JsOpcode.LoadConstant, JsOpcode.LoadThis, JsOpcode.NewArguments, JsOpcode.LoadNewTarget,
        JsOpcode.LoadArgument, JsOpcode.RestArguments,
        JsOpcode.LoadScoped, JsOpcode.StoreScoped, JsOpcode.InitialiseScoped,
        JsOpcode.LoadGlobal, JsOpcode.StoreGlobal, JsOpcode.LoadGlobalOrUndefined,
        JsOpcode.PushScope, JsOpcode.PopScope, JsOpcode.CopyScope, JsOpcode.DeclareGlobal,
        JsOpcode.PushObjectScope, JsOpcode.ResolveName,
        JsOpcode.NewObject, JsOpcode.NewArray,
        JsOpcode.GetProperty, JsOpcode.SetProperty, JsOpcode.GetIndex, JsOpcode.SetIndex,
        JsOpcode.DefineField, JsOpcode.DefineIndexed,
        JsOpcode.DeleteProperty, JsOpcode.DeleteIndex,
        JsOpcode.DefineGetter, JsOpcode.DefineSetter, JsOpcode.DefineMethod,
        JsOpcode.LoadSuperProperty, JsOpcode.StoreSuperProperty, JsOpcode.ArrayAppend,
        JsOpcode.Closure, JsOpcode.Call, JsOpcode.Construct,
        JsOpcode.Return, JsOpcode.ReturnUndefined, JsOpcode.CallEval,
        JsOpcode.SuperCall, JsOpcode.SuperCallForwarded, JsOpcode.NewClass,
        JsOpcode.ArrayHoles, JsOpcode.SpreadArray, JsOpcode.SpreadObject,
        JsOpcode.CallSpread, JsOpcode.ConstructSpread, JsOpcode.SuperCallSpread,
        JsOpcode.SetPrototypeLiteral,
        JsOpcode.Add, JsOpcode.Subtract, JsOpcode.Multiply, JsOpcode.Divide,
        JsOpcode.Remainder, JsOpcode.Exponent, JsOpcode.Negate, JsOpcode.ToNumber,
        JsOpcode.Not, JsOpcode.BitwiseNot,
        JsOpcode.LessThan, JsOpcode.LessThanOrEqual, JsOpcode.GreaterThan,
        JsOpcode.GreaterThanOrEqual, JsOpcode.StrictEquals, JsOpcode.StrictNotEquals,
        JsOpcode.LooseEquals, JsOpcode.LooseNotEquals,
        JsOpcode.BitwiseOr, JsOpcode.BitwiseAnd, JsOpcode.BitwiseXor,
        JsOpcode.ShiftLeft, JsOpcode.ShiftRight, JsOpcode.ShiftRightUnsigned,
        JsOpcode.TypeOf, JsOpcode.InstanceOf, JsOpcode.In, JsOpcode.Void,
        JsOpcode.RequireCoercible,
        JsOpcode.Jump, JsOpcode.JumpIfFalse, JsOpcode.JumpIfTrue, JsOpcode.Throw,
        JsOpcode.ForInStart, JsOpcode.ForInNext,
        JsOpcode.IterateStart, JsOpcode.IterateNext, JsOpcode.IterateRest, JsOpcode.IterateClose,
        JsOpcode.Yield, JsOpcode.YieldDelegate, JsOpcode.Await,
        JsOpcode.IterateStartAsync, JsOpcode.IterateNextAsync, JsOpcode.IterateAwaitStep,
        JsOpcode.IterateCloseAsync, JsOpcode.IterateCloseCheck,
        JsOpcode.DefineClassElement, JsOpcode.NewPrivateName,
        JsOpcode.LoadPrivate, JsOpcode.StorePrivate, JsOpcode.HasPrivate,
        JsOpcode.RunStaticElements,
        JsOpcode.Pop, JsOpcode.Duplicate, JsOpcode.DuplicateTwo, JsOpcode.Swap, JsOpcode.Pick,
    ];

    /// <summary>Whether <paramref name="value"/> is an opcode format version 2 defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=827F19
    // Broiler-Human:        PENDING
    public static bool IsDefined(byte value) => Shape((JsOpcode)value) is not null;

    /// <summary>How many operand bytes follow <paramref name="opcode"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B480F7
    // Broiler-Human:        PENDING
    public static int OperandWidth(JsOpcode opcode) => Shape(opcode) switch
    {
        JsOperandShape.U8 => 1,
        JsOperandShape.U16 => 2,
        JsOperandShape.U32 => 4,
        JsOperandShape.U8U16 => 3,
        _ => 0,
    };

    /// <summary>The whole width of one instruction, opcode byte included.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9DEEA7
    // Broiler-Human:        PENDING
    public static int InstructionWidth(JsOpcode opcode) => 1 + OperandWidth(opcode);

    /// <summary>Whether this opcode ends a basic block by transferring control unconditionally.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5989AC
    // Broiler-Human:        PENDING
    public static bool IsTerminal(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Jump or JsOpcode.Return or JsOpcode.ReturnUndefined or JsOpcode.Throw => true,
        _ => false,
    };

    /// <summary>Whether this opcode's <c>u32</c> operand is a code offset the verifier must check.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=227EC2
    // Broiler-Human:        PENDING
    public static bool HasCodeTarget(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Jump or JsOpcode.JumpIfFalse or JsOpcode.JumpIfTrue or
        JsOpcode.ForInNext or JsOpcode.IterateNext or
        JsOpcode.IterateAwaitStep or JsOpcode.IterateCloseAsync => true,
        _ => false,
    };

    /// <summary>
    /// The operand shape of <paramref name="opcode"/>, or <see langword="null"/> when this format
    /// version does not define it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=00562B
    // Broiler-Human:        PENDING
    public static JsOperandShape? Shape(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Nop or
        JsOpcode.LoadUndefined or JsOpcode.LoadNull or JsOpcode.LoadTrue or JsOpcode.LoadFalse or
        JsOpcode.LoadThis or JsOpcode.NewArguments or JsOpcode.LoadNewTarget or
        JsOpcode.PopScope or JsOpcode.PushObjectScope or
        JsOpcode.NewObject or
        JsOpcode.GetIndex or JsOpcode.SetIndex or JsOpcode.DefineIndexed or JsOpcode.DeleteIndex or
        JsOpcode.LoadSuperProperty or JsOpcode.StoreSuperProperty or
        JsOpcode.Return or JsOpcode.ReturnUndefined or JsOpcode.SuperCallForwarded or
        JsOpcode.Add or JsOpcode.Subtract or JsOpcode.Multiply or JsOpcode.Divide or
        JsOpcode.Remainder or JsOpcode.Exponent or JsOpcode.Negate or JsOpcode.ToNumber or
        JsOpcode.Not or JsOpcode.BitwiseNot or
        JsOpcode.LessThan or JsOpcode.LessThanOrEqual or JsOpcode.GreaterThan or
        JsOpcode.GreaterThanOrEqual or JsOpcode.StrictEquals or JsOpcode.StrictNotEquals or
        JsOpcode.LooseEquals or JsOpcode.LooseNotEquals or
        JsOpcode.BitwiseOr or JsOpcode.BitwiseAnd or JsOpcode.BitwiseXor or
        JsOpcode.ShiftLeft or JsOpcode.ShiftRight or JsOpcode.ShiftRightUnsigned or
        JsOpcode.TypeOf or JsOpcode.InstanceOf or JsOpcode.In or JsOpcode.Void or
        JsOpcode.Throw or JsOpcode.ForInStart or
        JsOpcode.ArrayAppend or JsOpcode.SpreadArray or JsOpcode.SpreadObject or
        JsOpcode.CallSpread or JsOpcode.ConstructSpread or JsOpcode.SuperCallSpread or
        JsOpcode.SetPrototypeLiteral or
        JsOpcode.IterateStart or JsOpcode.IterateRest or
        JsOpcode.Yield or JsOpcode.YieldDelegate or JsOpcode.Await or
        JsOpcode.IterateStartAsync or JsOpcode.IterateNextAsync or JsOpcode.IterateCloseCheck or
        JsOpcode.LoadPrivate or JsOpcode.StorePrivate or JsOpcode.HasPrivate or
        JsOpcode.RunStaticElements or
        JsOpcode.Pop or JsOpcode.Duplicate or JsOpcode.DuplicateTwo or JsOpcode.Swap
            => JsOperandShape.None,

        JsOpcode.Call or JsOpcode.CallEval or JsOpcode.Construct or JsOpcode.Pick or
        JsOpcode.DefineMethod or JsOpcode.NewClass or JsOpcode.SuperCall or
        JsOpcode.IterateClose or JsOpcode.DefineClassElement
            => JsOperandShape.U8,

        JsOpcode.LoadConstant or
        JsOpcode.LoadGlobal or JsOpcode.StoreGlobal or JsOpcode.LoadGlobalOrUndefined or
        JsOpcode.PushScope or JsOpcode.CopyScope or JsOpcode.DeclareGlobal or
        JsOpcode.NewArray or JsOpcode.ArrayHoles or
        JsOpcode.GetProperty or JsOpcode.SetProperty or JsOpcode.DefineField or
        JsOpcode.DeleteProperty or JsOpcode.DefineGetter or JsOpcode.DefineSetter or
        JsOpcode.Closure or
        JsOpcode.LoadArgument or JsOpcode.RestArguments or JsOpcode.RequireCoercible or
        JsOpcode.NewPrivateName
            => JsOperandShape.U16,

        JsOpcode.Jump or JsOpcode.JumpIfFalse or JsOpcode.JumpIfTrue or
        JsOpcode.ForInNext or JsOpcode.IterateNext or
        JsOpcode.IterateAwaitStep or JsOpcode.IterateCloseAsync
            => JsOperandShape.U32,

        // `ResolveName` shares the shape the three slot instructions use, and for the same reason:
        // one instruction carries a depth and a sixteen-bit index. Here the depth is a search bound
        // rather than a hop count and the index names a constant rather than a slot, which is a
        // difference in meaning and not in encoding.
        JsOpcode.LoadScoped or JsOpcode.StoreScoped or JsOpcode.InitialiseScoped or
        JsOpcode.ResolveName
            => JsOperandShape.U8U16,

        _ => null,
    };

    /// <summary>
    /// The stack effect of one instruction, given the operand actually encoded with it.
    /// </summary>
    /// <remarks>
    /// <paramref name="pops"/> and <paramref name="pushes"/> are the counts the executor performs,
    /// and the verifier's abstract height is computed from them alone. A false answer means the
    /// opcode is not one this format version defines - not that its effect is unknown.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A12200
    // Broiler-Human:        PENDING
    public static bool TryDescribe(JsOpcode opcode, uint operand, out int pops, out int pushes)
    {
        pops = 0;
        pushes = 0;

        switch (opcode)
        {
            case JsOpcode.Nop:
            case JsOpcode.PopScope:
            case JsOpcode.PushScope:
            case JsOpcode.CopyScope:
            case JsOpcode.DeclareGlobal:
            case JsOpcode.ArrayHoles:
                return true;

            case JsOpcode.LoadUndefined:
            case JsOpcode.LoadNull:
            case JsOpcode.LoadTrue:
            case JsOpcode.LoadFalse:
            case JsOpcode.LoadConstant:
            case JsOpcode.LoadThis:
            case JsOpcode.LoadNewTarget:
            case JsOpcode.NewArguments:
            case JsOpcode.LoadScoped:
            case JsOpcode.ResolveName:
            case JsOpcode.LoadGlobal:
            case JsOpcode.LoadGlobalOrUndefined:
            case JsOpcode.NewObject:
            case JsOpcode.Closure:
            case JsOpcode.Duplicate:
            case JsOpcode.Pick:
            case JsOpcode.SuperCallForwarded:
            case JsOpcode.LoadArgument:
            case JsOpcode.RestArguments:
                pushes = 1;
                return true;

            case JsOpcode.DuplicateTwo:
                pushes = 2;
                return true;

            case JsOpcode.StoreScoped:
            case JsOpcode.InitialiseScoped:
            case JsOpcode.StoreGlobal:
            case JsOpcode.Pop:
            case JsOpcode.PushObjectScope:
            case JsOpcode.Throw:
            case JsOpcode.Return:
            case JsOpcode.JumpIfFalse:
            case JsOpcode.JumpIfTrue:
            case JsOpcode.IterateClose:
            case JsOpcode.IterateCloseCheck:
                pops = 1;
                return true;

            // The Array or the object underneath stays, exactly as it does for DefineField: a
            // literal is a run of these over one value, and re-pushing it between them would double
            // the code for nothing.
            case JsOpcode.ArrayAppend:
            case JsOpcode.SpreadArray:
            case JsOpcode.SpreadObject:
            case JsOpcode.SetPrototypeLiteral:
                pops = 1;
                return true;

            case JsOpcode.Jump:
            case JsOpcode.ReturnUndefined:
                return true;

            case JsOpcode.NewArray:
                pops = checked((int)operand);
                pushes = 1;
                return true;

            case JsOpcode.GetProperty:
            case JsOpcode.DeleteProperty:
            case JsOpcode.Negate:
            case JsOpcode.ToNumber:
            case JsOpcode.Not:
            case JsOpcode.BitwiseNot:
            case JsOpcode.TypeOf:
            case JsOpcode.Void:
            case JsOpcode.ForInStart:
            case JsOpcode.LoadSuperProperty:
            case JsOpcode.RequireCoercible:
            case JsOpcode.IterateStart:
            case JsOpcode.IterateRest:
            case JsOpcode.IterateStartAsync:
                pops = 1;
                pushes = 1;
                return true;

            // THE RECORD UNDERNEATH STAYS, exactly as the object under DefineField does, because
            // the instruction that consumes the awaited step needs it and an `await` stands
            // between the two.
            case JsOpcode.IterateNextAsync:
                pushes = 1;
                return true;

            case JsOpcode.GetIndex:
            case JsOpcode.DeleteIndex:
            case JsOpcode.SetProperty:
            case JsOpcode.Add:
            case JsOpcode.Subtract:
            case JsOpcode.Multiply:
            case JsOpcode.Divide:
            case JsOpcode.Remainder:
            case JsOpcode.Exponent:
            case JsOpcode.LessThan:
            case JsOpcode.LessThanOrEqual:
            case JsOpcode.GreaterThan:
            case JsOpcode.GreaterThanOrEqual:
            case JsOpcode.StrictEquals:
            case JsOpcode.StrictNotEquals:
            case JsOpcode.LooseEquals:
            case JsOpcode.LooseNotEquals:
            case JsOpcode.BitwiseOr:
            case JsOpcode.BitwiseAnd:
            case JsOpcode.BitwiseXor:
            case JsOpcode.ShiftLeft:
            case JsOpcode.ShiftRight:
            case JsOpcode.ShiftRightUnsigned:
            case JsOpcode.InstanceOf:
            case JsOpcode.In:
                pops = 2;
                pushes = 1;
                return true;

            case JsOpcode.SetIndex:
                pops = 3;
                pushes = 1;
                return true;

            // The key goes and the value comes back, so a compound `super.x += 1` needs no
            // temporary: the key is duplicated once and consumed by the read and the write.
            case JsOpcode.StoreSuperProperty:
                pops = 2;
                pushes = 1;
                return true;

            // The object stays. A literal is a run of definitions over one object, and popping it
            // between them would mean re-pushing it, which doubles the code for no gain.
            case JsOpcode.DefineField:
            case JsOpcode.DefineGetter:
            case JsOpcode.DefineSetter:
                pops = 1;
                return true;

            case JsOpcode.DefineIndexed:
                pops = 2;
                return true;

            // The key and the function go and the object stays, exactly as DefineIndexed leaves
            // it, so a run of members over one object is a run of these and nothing else.
            case JsOpcode.DefineMethod:
                pops = 2;
                return true;

            // A derived class pops its superclass too, which is the one thing the operand bit
            // changes about this instruction and the reason the bit is not a second opcode.
            case JsOpcode.NewClass:
                pops = (operand & ClassIsDerived) != 0 ? 2 : 1;
                pushes = 1;
                return true;

            case JsOpcode.SuperCall:
                pops = checked((int)operand);
                pushes = 1;
                return true;

            // Callee, receiver and the arguments go; one result comes back. A direct `eval` has
            // exactly this effect, which is the whole reason it can be a separate opcode at all:
            // the verifier checks it without knowing what it means.
            case JsOpcode.Call:
            case JsOpcode.CallEval:
                pops = checked((int)operand) + 2;
                pushes = 1;
                return true;

            case JsOpcode.Construct:
                pops = checked((int)operand) + 1;
                pushes = 1;
                return true;

            // The argument Array carries the count, so these two have a fixed effect where Call and
            // Construct have one that varies.
            case JsOpcode.CallSpread:
                pops = 3;
                pushes = 1;
                return true;

            case JsOpcode.ConstructSpread:
                pops = 2;
                pushes = 1;
                return true;

            // The superclass and the `new.target` both come from the frame, so only the argument
            // Array is popped - which is what makes this SuperCall's shape and not CallSpread's.
            case JsOpcode.SuperCallSpread:
                pops = 1;
                pushes = 1;
                return true;

            // The FALLTHROUGH effect: the enumerator goes and a name arrives. On the taken
            // branch nothing arrives, so the target's height is one below this instruction's - a
            // rule the verifier applies at the target rather than here, because one instruction
            // with two effects is exactly what a stack-height check exists to pin down.
            case JsOpcode.ForInNext:
            case JsOpcode.IterateNext:

            // THE SAME TWO-EFFECT SHAPE, reached when there was no `return` to call: nothing
            // arrives on the taken branch, so the target is one below this instruction's height.
            case JsOpcode.IterateCloseAsync:
                pops = 1;
                pushes = 1;
                return true;

            // AND THE SAME SHAPE ONE VALUE WIDER. It consumes the awaited step AND the record the
            // step was taken from, so its target is TWO below rather than one - which is a rule the
            // verifier applies at the target, because one instruction with two effects is exactly
            // what a stack-height check exists to pin down.
            case JsOpcode.IterateAwaitStep:
                pops = 2;
                pushes = 1;
                return true;

            // NET ZERO, AND THE PUSH HAS NO PRODUCER IN THE CODE. What a suspension pushes is what
            // the resumption sent it, which arrived from outside the unit entirely; counting it as
            // a push is what makes the abstract height at the following instruction equal the
            // height at this one, which is what a reader of the lowering expects `yield` to be.
            case JsOpcode.Yield:
            case JsOpcode.YieldDelegate:
            case JsOpcode.Await:
                pops = 1;
                pushes = 1;
                return true;

            case JsOpcode.Swap:
                pops = 2;
                pushes = 2;
                return true;

            // THE PAIR UNDERNEATH STAYS AND IS NOT COUNTED, exactly as the object under
            // DefineMethod is. The instruction READS the constructor and the home object below the
            // key rather than consuming them, so a class body's element list is a run of these over
            // one pair - and the verifier still knows both are there, because the height it has
            // computed at this instruction is at least four.
            case JsOpcode.DefineClassElement:
                pops = 2;
                return true;

            case JsOpcode.NewPrivateName:
                pushes = 1;
                return true;

            case JsOpcode.LoadPrivate:
            case JsOpcode.HasPrivate:
                pops = 2;
                pushes = 1;
                return true;

            case JsOpcode.StorePrivate:
                pops = 3;
                pushes = 1;
                return true;

            // The constructor stays, so a class lowering can run its static elements and then go on
            // using the value it already had.
            case JsOpcode.RunStaticElements:
                return true;

            default:
                return false;
        }
    }
}
