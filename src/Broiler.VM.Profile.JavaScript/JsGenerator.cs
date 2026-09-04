// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           26
// Human-reviewed:   0/11
// IP risk:          None
// Security risk:    High
// Criteria:         3/3
// Resource impact:  4/10 max
// Unverified:       11
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
/// <b>Only a generator invocation ever gets one, and only one frame ever has to survive.</b>
/// <c>yield</c> is a syntax error anywhere but in the generator's own body - not in a nested
/// function, not in a nested arrow - so the frame that suspends is always the frame the dispatch
/// loop is running at that instant. That is why this profile needs no continuations, no second
/// thread and no transform of the body into a state machine: the loop returns, this object holds
/// what it was holding, and the next resumption hands it all back.
/// </para>
/// <para>
/// <b>What is here is exactly what an instruction boundary needs and nothing else.</b> The operand
/// stack and its height, the instruction pointer, the chain of environments the frame is currently
/// inside, and the three things a frame is entered with - the receiver, the actual arguments and
/// the function object an <c>arguments</c> object names as its callee. A field that could be
/// recomputed from the program is not here; a field that could not is.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=4; Fingerprint=9F431B
// Broiler-Falsified-If: a generator resumed after a suspension observes an operand stack, a scope chain or an instruction pointer other than the one it suspended with
// Broiler-Human:        PENDING
internal sealed class JsFrame
{
    /// <summary>Creates the frame of a generator invocation that has not started.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=79D229
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

    /// <summary>The generator function this is an invocation of.</summary>
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

    /// <summary>Whether any instruction of the body has run yet.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=4; Fingerprint=ADA5EB
    // Broiler-Human:        PENDING
    internal bool Started { get; set; }

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
