// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           51
// Human-reviewed:   0/17
// IP risk:          None
// Security risk:    High
// Criteria:         5/5
// Resource impact:  4/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>Which of the four things a generator object is doing right now.</summary>
/// <remarks>
/// The four are the specification's, and the reason they are four rather than two is
/// <see cref="Executing"/>: a generator whose own body reaches its own <c>next</c> would otherwise
/// re-enter a frame that is already being interpreted, and two interpreters walking one operand
/// stack is a corruption rather than a diagnosable error. The language answers that with a
/// <c>TypeError</c>, and this state is what makes the answer possible.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AD7E56
// Broiler-Human:        PENDING
internal enum JsGeneratorState
{
    /// <summary>Built, and no instruction of its body has run.</summary>
    SuspendedStart = 0,

    /// <summary>Suspended at a <c>yield</c>, with a frame waiting to be resumed.</summary>
    SuspendedYield = 1,

    /// <summary>Its body is on the interpreter's stack right now.</summary>
    Executing = 2,

    /// <summary>It returned, threw, or was returned into. There is no frame any more.</summary>
    Completed = 3,
}

/// <summary>Which suspension the dispatch loop left through.</summary>
/// <remarks>
/// <para>
/// <b>It exists because ONE body can contain both, and until the async generator there was no such
/// body.</b> A generator suspends only at a <c>yield</c> and an async function only at an
/// <c>await</c>, so the driver that received the value knew what the value meant from which driver
/// it was. An async generator's body suspends at both, into the same frame, and the two mean
/// opposite things on the way out: an <c>await</c>'s value is handed to <c>PromiseResolve</c> and
/// the caller waits, a <c>yield</c>'s value settles the promise the caller is already holding. A
/// driver that confused them would resolve a request with a promise the body was waiting on.
/// </para>
/// <para>
/// <b>It is written by the instruction and read by the driver, and never by the frame itself.</b>
/// The alternative was reading the opcode byte back out of the code at <c>Pc</c>, which is exact
/// and is also a second place that knows which opcodes suspend - and the one that would drift is
/// the reader.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E74209
// Broiler-Human:        PENDING
internal enum JsSuspension
{
    /// <summary>The loop has not suspended, or the last suspension has been consumed.</summary>
    None = 0,

    /// <summary>It left through <c>Yield</c> or <c>YieldDelegate</c>: the value is yielded.</summary>
    Yield = 1,

    /// <summary>It left through <c>Await</c>: the value is awaited.</summary>
    Await = 2,
}

/// <summary>How a resumption re-enters the frame it is resuming.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=069E3C
// Broiler-Human:        PENDING
internal enum JsResumeMode
{
    /// <summary>The <c>yield</c> produces the sent value and the body continues.</summary>
    Next = 0,

    /// <summary>The <c>yield</c> raises a throw, which the body's own regions may catch.</summary>
    Throw = 1,

    /// <summary>The <c>yield</c> raises a return, which only a <c>finally</c> may intercept.</summary>
    Return = 2,
}

/// <summary>
/// The frame of ONE suspended guest invocation: what <c>Execute</c> holds across an instruction
/// boundary, moved to the heap.
/// </summary>
/// <remarks>
/// <para>
/// <b>A generator invocation and an async one get one, nothing else does, and only one frame ever
/// has to survive.</b> <c>yield</c> is a syntax error anywhere but in the generator's own body and
/// <c>await</c> is a syntax error anywhere but in the async function's own - not in a nested
/// function, not in a nested arrow - so the frame that suspends is always the frame the dispatch
/// loop is running at that instant. That is why this profile needs no continuations, no second
/// thread and no transform of the body into a state machine: the loop returns, this object holds
/// what it was holding, and the next resumption hands it all back.
/// </para>
/// <para>
/// <b>The two differ in WHO resumes and in nothing this class can see.</b> A generator's frame is
/// held by a guest-visible generator object and resumed by the guest calling <c>next</c>; an async
/// call's frame is held by a promise reaction and resumed by the job queue. The frame is the same
/// frame, which is the whole reason <c>await</c> cost one opcode rather than a second suspension
/// mechanism.
/// </para>
/// <para>
/// <b>What is here is exactly what an instruction boundary needs and nothing else.</b> The operand
/// stack and its height, the instruction pointer, the chain of environments the frame is currently
/// inside, and the things a frame is entered with - the receiver, the actual arguments, the
/// function object an <c>arguments</c> object names as its callee, and for an async ARROW the
/// <c>new.target</c> and <c>this</c> box it closed over. A field that could be recomputed from the
/// program is not here; a field that could not is.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=9F431B
// Broiler-Falsified-If: a generator resumed after a suspension observes an operand stack, a scope chain or an instruction pointer other than the one it suspended with
// Broiler-Human:        PENDING
internal sealed class JsFrame
{
    /// <summary>Creates the frame of a suspendable invocation that has not started.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=D70DE1
    // Broiler-Human:        PENDING
    internal JsFrame(
        JsProgram program,
        int unitIndex,
        JsEnvironment environment,
        JsValue thisValue,
        JsValue[] arguments,
        JsScriptFunction function)
    {
        Program = program;
        UnitIndex = unitIndex;
        ThisValue = thisValue;
        Arguments = arguments;
        Function = function;
        Stack = new JsValue[program.Functions[unitIndex].MaxOperandStack + 1];
        Scopes = new System.Collections.Generic.List<JsEnvironment>(4) { environment };
        Pc = (int)program.Functions[unitIndex].CodeOffset;

        // `undefined` AND NOT `default`, because the default of the value struct is EMPTY - the
        // marker an uninitialised binding holds - and a frame that handed that to `new.target`
        // would make `new.target` read as a temporal-dead-zone value rather than as the absence
        // the language says an ordinary call gives it.
        NewTarget = JsValue.Undefined;
    }

    /// <summary>The program the body lives in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    /// <summary>Which code unit the body is.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=998E2E
    // Broiler-Human:        PENDING
    internal int UnitIndex { get; }

    /// <summary>The receiver the body sees as <c>this</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=4D7954
    // Broiler-Human:        PENDING
    internal JsValue ThisValue { get; }

    /// <summary>The actual arguments, which an <c>arguments</c> object is built from.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=CCCC5C
    // Broiler-Human:        PENDING
    internal JsValue[] Arguments { get; }

    /// <summary>The generator or async function this is an invocation of.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=558BB7
    // Broiler-Human:        PENDING
    internal JsScriptFunction Function { get; }

    /// <summary>The operand stack, sized from the height verification computed.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=00A7B4
    // Broiler-Human:        PENDING
    internal JsValue[] Stack { get; }

    /// <summary>The environments the frame is currently inside, outermost first.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=C29A8C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsEnvironment> Scopes { get; }

    /// <summary>Where the next instruction is.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=745D36
    // Broiler-Human:        PENDING
    internal int Pc { get; set; }

    /// <summary>How many operand-stack entries are live.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=35ED5B
    // Broiler-Human:        PENDING
    internal int Sp { get; set; }

    /// <summary>Whether the loop left by suspending rather than by completing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=5EBCB2
    // Broiler-Human:        PENDING
    internal bool Suspended { get; set; }

    /// <summary>Which suspension the loop left through, when it left through one.</summary>
    /// <remarks>
    /// It is only ever read by the async generator driver, which is the one driver whose frame can
    /// suspend two ways. The generator driver and the async driver each serve a body that has only
    /// one kind of suspension in it and never ask.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=1B626A
    // Broiler-Human:        PENDING
    internal JsSuspension Suspension { get; set; }

    /// <summary>How the next re-entry into the loop presents itself at the suspension point.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=DC6827
    // Broiler-Human:        PENDING
    internal JsResumeMode ResumeMode { get; set; }

    /// <summary>The value the resumption carries: what was sent, thrown, or returned.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=7DA6AA
    // Broiler-Human:        PENDING
    internal JsValue ResumeValue { get; set; }

    /// <summary>
    /// The iterator record a <c>yield*</c> is part-way through, or <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is in the frame and not on the operand stack, for the reason the <c>for…in</c>
    /// enumerator is in a scope slot.</b> A value the operand stack holds across a suspension would
    /// have to be at the abstract height the verifier computed for the instruction after the
    /// suspension - and the delegation resumes at the SAME instruction, so there is no such height.
    /// </para>
    /// <para>
    /// <b>It is the RECORD and not the iterator object</b>, so that a delegation steps the
    /// <c>next</c> that was read once when the iterator was acquired and carries the done flag the
    /// rest of the iteration protocol carries. Holding the object alone would have meant re-reading
    /// <c>next</c> at every resumption, which a program that replaces its own <c>next</c> mid-loop
    /// can see.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=AC73B2
    // Broiler-Human:        PENDING
    internal JsIteratorRecord? Delegate { get; set; }

    /// <summary>Whether the pending resumption re-enters a <c>yield*</c> rather than a <c>yield</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=0CD375
    // Broiler-Human:        PENDING
    internal bool Delegating { get; set; }

    /// <summary>
    /// Where inside an ASYNC delegation the pending resumption re-enters, or zero.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A synchronous <c>yield*</c> needs no such field because it has ONE suspension point: the
    /// inner value going out.</b> An asynchronous one has five, because every inner step is awaited
    /// before it is examined - so a resumption can arrive at the yield of an inner value, at the
    /// await of what <c>next</c> or <c>throw</c> answered, at the await of what <c>return</c>
    /// answered, at the await of a close performed because the inner iterator has no <c>throw</c>,
    /// or at the await the language performs on the RESUMPTION's own value when the inner iterator
    /// has no <c>return</c>. All five re-enter the same instruction, and this is what tells them
    /// apart.
    /// </para>
    /// <para>
    /// <b>Zero is the yield, and it is zero rather than a named member for the reason
    /// <see cref="Delegating"/> is a bool</b>: the field is written and read in one method, and a
    /// resumption that arrives with it unset is the ordinary one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=881772
    // Broiler-Human:        PENDING
    internal int DelegateStage { get; set; }

    /// <summary>Whether any instruction of the body has run yet.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=ADA5EB
    // Broiler-Human:        PENDING
    internal bool Started { get; set; }

    /// <summary>
    /// Whether the dispatch loop is running this frame's parameter-binding prologue rather than
    /// its body, and must stop at the seam.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is set for exactly one entry into the loop and cleared by the seam that ends it.</b>
    /// A generator over a unit that binds its own parameters is entered twice before it yields
    /// anything - once by the CALL, which runs the defaults, the rest parameter and the patterns
    /// and stops at <c>EnterBody</c>, and once by the first <c>next</c>, which starts the body.
    /// Both entries run the same instruction stream over the same frame, and this is the only
    /// thing that tells them apart.
    /// </para>
    /// <para>
    /// <b>A prologue that does not reach the seam leaves this set, and the caller reads it as the
    /// artifact being malformed rather than as anything to recover from.</b> The lowering emits the
    /// seam in every generator unit that binds its parameters and emits no suspension before it, so
    /// the only way to arrive at the end of a prologue run with this still true is an artifact
    /// nothing in this checkout produced.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=DBFABE
    // Broiler-Human:        PENDING
    internal bool BindingParameters { get; set; }

    /// <summary>
    /// The <c>new.target</c> the frame was entered with. Always <c>undefined</c> for a generator.
    /// </summary>
    /// <remarks>
    /// <b>It is here for the ASYNC ARROW and for nothing else.</b> A generator is neither an arrow
    /// nor a constructor - the verifier refuses both pairings - so its <c>new.target</c> is
    /// <c>undefined</c> and was passed as a literal. An async arrow is an arrow, so it reads the
    /// <c>new.target</c> its closure recorded, and a value re-entered across a suspension has to
    /// come from the frame rather than from the call that resumed it: the resumption is a job, and
    /// a job knows nothing about the function that suspended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=2B84B5
    // Broiler-Human:        PENDING
    internal JsValue NewTarget { get; set; }

    /// <summary>
    /// The box the frame reads <c>this</c> through, or <see langword="null"/> when it reads the
    /// value it was entered with.
    /// </summary>
    /// <remarks>
    /// The same reason as <see cref="NewTarget"/>, and the case that needs it is an async arrow
    /// inside a DERIVED constructor: its <c>this</c> does not exist until <c>super()</c> returns,
    /// so the arrow reads a box rather than a value and the box has to survive the suspension.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=2B35A7
    // Broiler-Human:        PENDING
    internal JsCell? ThisBinding { get; set; }

    /// <summary>
    /// What this frame costs to hold, in bytes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is bounded and it is bounded by verification.</b> The operand stack is exactly the
    /// height the abstract pass computed for the unit, at the value representation's own stated
    /// twenty-four bytes, so the worst a single suspended generator can pin is the format's
    /// <c>CeilingOperandStack</c> entries and no more. The flat term covers the object, the scope
    /// list and the list's own header; the environment RECORDS it points at are not counted here,
    /// because a closure created in the same body pins exactly those and this profile has never
    /// counted them against a closure.
    /// </para>
    /// <para>
    /// <b>It is charged as FUEL at creation and is deliberately NOT reported as a live-memory
    /// retention.</b> A retention would have to be released when the generator becomes garbage, and
    /// this profile has no reachability signal that is not the collector's - and a budget that
    /// moved with the collector would make two runs of one program stop in different places, which
    /// is the property the whole metering model exists to keep. What a retention bought in
    /// exchange was a false refusal: a loop that builds and drops a hundred thousand generators
    /// spent a live-bytes ceiling that a loop building and dropping a hundred thousand ORDINARY
    /// OBJECTS does not, because this profile meters no guest heap at all. Singling out the one
    /// object that happens to carry an array would have made a program every engine runs stop.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=3EF5A7
    // Broiler-Human:        PENDING
    internal ulong FrameBytes => (ulong)(Stack.Length * 24) + 512;
}

/// <summary>
/// The value a forced return travels as while it is inside a <c>finally</c> block.
/// </summary>
/// <remarks>
/// <para>
/// <b>The lowering of <c>finally</c> already parks a pending completion in a slot and re-raises it
/// afterwards, and this is what makes a forced return use that machinery instead of a second
/// one.</b> <c>gen.return(v)</c> raises a return at the suspension point; the innermost enclosing
/// <c>finally</c> region catches it as this object, runs its block, loads it back and re-raises it
/// with the ordinary <c>Throw</c> instruction, which recognises it and turns it back into a return.
/// A <c>catch</c> clause never sees it, because the search for a handler skips <c>catch</c>
/// regions entirely - which is the whole difference between <c>return</c> and <c>throw</c>.
/// </para>
/// <para>
/// <b>Guest code cannot reach it</b>, exactly as guest code cannot reach a <c>for…in</c>
/// enumerator: it exists only between the frame slot the lowering wrote it to and the instruction
/// that consumes it, and no source expression names that slot.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4AB7F8
// Broiler-Falsified-If: a `catch` clause in a generator body observes the value a `return()` forced, or a `finally` block does not run for one
// Broiler-Human:        PENDING
internal sealed class JsForcedReturn : JsObject
{
    /// <summary>Wraps the value a forced return is carrying.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E3EA10
    // Broiler-Human:        PENDING
    internal JsForcedReturn(JsValue value)
        : base(null, "ForcedReturn") => Value = value;

    /// <summary>The value the generator will complete with once the finalisers have run.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CF3706
    // Broiler-Human:        PENDING
    internal JsValue Value { get; }
}

/// <summary>A forced return in flight, on the CLR's own unwinding mechanism.</summary>
/// <remarks>
/// It is a separate exception type from <see cref="JsThrow"/> and not a flag on it, because the
/// two are caught by different things: a <c>catch</c> clause catches one and never the other. A
/// flag would have made every existing handler site responsible for testing it, and the one that
/// forgot would have let <c>gen.return()</c> be swallowed by an unrelated <c>catch</c>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1B595F
// Broiler-Human:        PENDING
internal sealed class JsReturnSignal : System.Exception
{
    /// <summary>Creates a forced return carrying <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=24A81E
    // Broiler-Human:        PENDING
    internal JsReturnSignal(JsValue value)
        : base("a generator was returned into") => Value = value;

    /// <summary>The value the generator completes with.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CF3706
    // Broiler-Human:        PENDING
    internal JsValue Value { get; }
}

/// <summary>
/// A generator object: the state machine, and the one frame it may resume.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is an ordinary object with a private field, and not an exotic one.</b> Everything the
/// language says about it - that it is iterable, that its prototype chain reaches the generator
/// function's <c>prototype</c>, that <c>Object.prototype.toString</c> answers <c>Generator</c> - is
/// ordinary object behaviour once the prototypes are wired the way the specification wires them.
/// What is not ordinary is the frame, and the frame is unreachable from guest code.
/// </para>
/// <para>
/// <b>The state is checked before the frame is touched, in every one of the three methods.</b> A
/// generator that is already executing is a <c>TypeError</c> rather than a re-entry, and a
/// generator that has completed answers without running anything - including <c>throw</c>, which
/// throws its argument at the caller rather than into a body that is no longer there.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=FDED37
// Broiler-Falsified-If: a generator whose body is on the interpreter's stack can be resumed again, or a completed generator runs any part of its body
// Broiler-Human:        PENDING
internal sealed class JsGenerator : JsObject
{
    /// <summary>Creates a generator object over a frame that has not started.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=98E438
    // Broiler-Human:        PENDING
    internal JsGenerator(JsObject? prototype, JsFrame frame)
        : base(prototype, "Generator") => Frame = frame;

    /// <summary>The suspended frame, or <see langword="null"/> once the generator completed.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=5E59D5
    // Broiler-Human:        PENDING
    internal JsFrame? Frame { get; set; }

    /// <summary>Which of the four states the generator is in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=560ECF
    // Broiler-Human:        PENDING
    internal JsGeneratorState State { get; set; } = JsGeneratorState.SuspendedStart;
}

/// <summary>
/// ONE call of an async function that has not finished: the frame it will resume into, and the
/// promise it will settle.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is not an object and no guest expression can name it.</b> A generator object is
/// guest-visible because the language says so - it is what <c>g()</c> evaluates to, and the guest
/// drives it by calling <c>next</c>. Nothing drives an async call from the guest side: what
/// <c>f()</c> evaluates to is the PROMISE, and the only thing that ever resumes the frame is a
/// reaction on some other promise. So this is a plain record held by those reactions, and when the
/// last of them has run it becomes garbage with the frame it was holding.
/// </para>
/// <para>
/// <b>What bounds a suspended async call is what bounds a suspended generator, plus one thing
/// more.</b> The frame is <see cref="JsFrame.FrameBytes"/> - an operand stack of exactly the
/// height verification computed, so at most the format's <c>CeilingOperandStack</c> entries -
/// charged as fuel where the call is made. The one thing more is that a suspended async call is
/// only reachable from a reaction that is itself charged and retained when it is registered, so a
/// program cannot accumulate suspended calls without also accumulating reactions the allowance is
/// already counting.
/// </para>
/// <para>
/// <b><see cref="Running"/> is the same guard <c>JsGeneratorState.Executing</c> is, and it is here
/// for a case that looks impossible and is not.</b> An <c>await</c> whose operand is a thenable
/// runs guest code - the thenable's <c>then</c> - and that code is handed this call's own
/// <c>resolve</c>. A thenable that calls it twice, or calls it synchronously from inside the job
/// that is already resuming this frame, would re-enter a frame the interpreter is walking. The
/// promise machinery's own latch stops most of it; this stops the rest, and two interpreters on
/// one operand stack is a corruption rather than a diagnosable error.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=17AAD5
// Broiler-Falsified-If: an async call whose body is on the interpreter's stack is resumed again, or a suspended async call is reachable from anything the allowance is not already counting
// Broiler-Human:        PENDING
internal sealed class JsAsyncCall
{
    /// <summary>Creates the record of one async call over a frame that has not run yet.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=7D1457
    // Broiler-Human:        PENDING
    internal JsAsyncCall(JsFrame frame, JsPromiseObject promise)
    {
        Frame = frame;
        Promise = promise;
    }

    /// <summary>The suspended frame, or <see langword="null"/> once the call has settled.</summary>
    /// <remarks>
    /// Dropping it is the point rather than the tidiness, exactly as it is for a completed
    /// generator: the promise this call settled may be kept for the rest of the program, and
    /// without this it would keep the operand stack, the scope chain and everything those reach
    /// alive with it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=5E59D5
    // Broiler-Human:        PENDING
    internal JsFrame? Frame { get; set; }

    /// <summary>The promise the call's own <c>return</c> or <c>throw</c> settles.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=8A7CF2
    // Broiler-Human:        PENDING
    internal JsPromiseObject Promise { get; }

    /// <summary>Whether the body is on the interpreter's stack right now.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=832C69
    // Broiler-Human:        PENDING
    internal bool Running { get; set; }
}

/// <summary>Which of the five things an async generator object is doing right now.</summary>
/// <remarks>
/// <b>They are five where a synchronous generator's are four, and the extra one is the whole reason
/// the queue exists.</b> A synchronous generator finishes a resumption before its <c>next</c>
/// returns, so there is no state between "running" and "suspended". An asynchronous one answers a
/// promise and goes on running across jobs, so a second <c>next</c> can arrive at any point - and
/// <see cref="DrainingQueue"/> is the state in which the body is GONE but requests may still be
/// waiting, each of which is owed an answer in the order it was made.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=2B18DF
// Broiler-Human:        PENDING
internal enum JsAsyncGeneratorState
{
    /// <summary>Built, and no instruction of its body has run.</summary>
    SuspendedStart = 0,

    /// <summary>Suspended at a <c>yield</c>, with a frame waiting to be resumed.</summary>
    SuspendedYield = 1,

    /// <summary>Its body is running, or is waiting on an <c>await</c> that will resume it.</summary>
    Executing = 2,

    /// <summary>The body has finished and the requests it did not answer are being answered.</summary>
    DrainingQueue = 3,

    /// <summary>There is no body and no request left. Every later request answers at once.</summary>
    Completed = 4,
}

/// <summary>
/// ONE call of <c>next</c>, <c>return</c> or <c>throw</c> on an async generator that has not been
/// answered yet.
/// </summary>
/// <remarks>
/// <b>It is the pairing a synchronous generator never needs: what was asked, and where to put the
/// answer.</b> <c>gen.next()</c> on a synchronous generator asks and is answered in one call, so
/// the question never has to be written down. On an async generator the answer is a promise made
/// now and settled later - possibly several jobs later, possibly after other requests that arrived
/// first - so both halves have to survive in a list.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=ADB28B
// Broiler-Human:        PENDING
internal sealed class JsAsyncGeneratorRequest
{
    /// <summary>Records one unanswered request.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=AFF929
    // Broiler-Human:        PENDING
    internal JsAsyncGeneratorRequest(JsResumeMode mode, JsValue value, JsPromiseObject promise)
    {
        Mode = mode;
        Value = value;
        Promise = promise;
    }

    /// <summary>Which of the three methods was called.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=7287F2
    // Broiler-Human:        PENDING
    internal JsResumeMode Mode { get; }

    /// <summary>The one argument it was called with, or <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=CF3706
    // Broiler-Human:        PENDING
    internal JsValue Value { get; }

    /// <summary>The promise that call answered with, still pending.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=8A7CF2
    // Broiler-Human:        PENDING
    internal JsPromiseObject Promise { get; }
}

/// <summary>
/// An async generator object: the state machine, the one frame it may resume, and the queue of
/// requests it has not answered.
/// </summary>
/// <remarks>
/// <para>
/// <b>The queue is the difference between this and <see cref="JsGenerator"/>, and it is not an
/// optimisation.</b> <c>const g = agen(); g.next(); g.next();</c> makes two calls before the first
/// has settled anything, and both are entitled to an answer, in that order, from a body only one of
/// them may be inside. Without the queue the second call would either re-enter a running frame -
/// two interpreters on one operand stack - or be answered out of order, and a program that awaits
/// both promises can tell.
/// </para>
/// <para>
/// <b>The request at the FRONT is the one being served, and it is removed when it is answered
/// rather than when it is taken.</b> That is the specification's arrangement and it is load-bearing
/// twice: a <c>yield</c> answers the front request and then looks at the queue again, continuing
/// WITHOUT suspending if another request is already waiting; and a body that finishes answers the
/// front request and then drains the rest, each with a done step, until it reaches a
/// <c>return</c> - which is awaited before it is answered.
/// </para>
/// <para>
/// <b>What bounds the queue is what bounds every other list a guest can grow: the fuel each call
/// spends.</b> A request is enqueued by a call of <c>next</c>, and a call is charged; a program
/// that queues a million requests has made a million calls and paid for them. Nothing here is
/// charged twice for being asynchronous.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=F8D803
// Broiler-Falsified-If: two requests made before the first settles are answered out of order, or a request reaches a body that is already on the interpreter's stack
// Broiler-Human:        PENDING
internal sealed class JsAsyncGenerator : JsObject
{
    /// <summary>Creates an async generator object over a frame that has not started.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=E761B7
    // Broiler-Human:        PENDING
    internal JsAsyncGenerator(JsObject? prototype, JsFrame frame)
        : base(prototype, "AsyncGenerator") => Frame = frame;

    /// <summary>The suspended frame, or <see langword="null"/> once the body has finished.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=5E59D5
    // Broiler-Human:        PENDING
    internal JsFrame? Frame { get; set; }

    /// <summary>Which of the five states the async generator is in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=DBD730
    // Broiler-Human:        PENDING
    internal JsAsyncGeneratorState State { get; set; } = JsAsyncGeneratorState.SuspendedStart;

    /// <summary>The requests made and not yet answered, oldest first.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=A43500
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsAsyncGeneratorRequest> Queue { get; } = [];

    /// <summary>Whether the body is on the interpreter's stack at this instant.</summary>
    /// <remarks>
    /// <b>It is NOT the same question <see cref="State"/> answers, and that is why it is a second
    /// field.</b> <see cref="JsAsyncGeneratorState.Executing"/> covers the whole span from a
    /// resumption to the settlement that ends it, including the jobs an <c>await</c> waits through -
    /// during which nothing is on the stack and the driver is free to be re-entered. This is true
    /// only while the dispatch loop is actually inside the body, which is the condition that makes
    /// a re-entry a corruption rather than a queued request.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=832C69
    // Broiler-Human:        PENDING
    internal bool Running { get; set; }
}
