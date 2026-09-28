// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   58
// Annotated:        58/58
// Exempt:           74
// Human-reviewed:   0/58
// IP risk:          Low
// Security risk:    High
// Criteria:         10/9
// Resource impact:  0/10 max
// Unverified:       58
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>
/// What a family supplies to execute its instructions: implemented by a <c>struct</c>, and reached
/// only through static members, so that a loop generic over the family specialises per family and
/// carries no interface dispatch for it.
/// </summary>
/// <remarks>
/// <para>
/// <b>The handler is the meaning.</b> <see cref="Handle"/> is called for every family row whose kind
/// is not <see cref="UbcInstructionKind.Primitive"/>, and for a primitive row wherever an emitter does
/// not implement the primitive itself. It reads its inputs from the planes at
/// <see cref="UbcActivation.WordArgs"/> and <see cref="UbcActivation.ValueArgs"/> upward - the slots
/// its row's effect pops, bottom first - and writes the slots its row's effect pushes at the same two
/// bases. It never moves a plane's top: the loop sets the tops from the row's effect after the handler
/// answers, so a handler cannot leave the stack at a height the walk did not prove.
/// </para>
/// <para>
/// A family's code never pushes a frame, never reads the call depth, never searches a region and never
/// holds a return address. It answers a <see cref="UbcStatus"/>, and the emitter does the rest.
/// </para>
/// <para>
/// <b>Four members have defaults.</b> <see cref="ResolveEntry"/>, <see cref="AdmitInstance"/>,
/// <see cref="StartUnit"/> and <see cref="AbandonInstance"/> are static virtual, and their defaults are
/// what every family did before universal bytecode contract version 2: entry points from the artifact's
/// Entries section, every instance ready, no start unit, and nothing to release. A family overrides
/// only the ones its language needs.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=CA8AC4
// Broiler-Falsified-If: a member here lets a family move a plane's top, push a frame, or read a value of another family
// Broiler-Human:        PENDING
public interface IUbcFamily
{
    /// <summary>Executes one family instruction and answers what the emitter does next.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A2D95E
    // Broiler-Falsified-If: a handler is reached for a row whose effect the walk did not apply, or its answer moves a plane's top
    // Broiler-Human:        PENDING
    static abstract UbcStatus Handle(ref UbcActivation activation, byte familyOpcode, ulong operand);

    /// <summary>
    /// Places what a landing in a region of <paramref name="regionKind"/> pushes, at the argument bases,
    /// with the exception in flight in <see cref="UbcActivation.Pending"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E2EACB
    // Broiler-Human:        PENDING
    static abstract void OnLand(ref UbcActivation activation, byte regionKind);

    /// <summary>
    /// Places what the suspending row pushes when execution resumes after it, at the argument bases,
    /// from <paramref name="reason"/>, the continuation the row answered with and the resumer completed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7E3241
    // Broiler-Human:        PENDING
    static abstract void OnResume(ref UbcActivation activation, object reason);

    /// <summary>A value plane of the family's own value type, with room for <paramref name="capacity"/> values.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=85ECF6
    // Broiler-Human:        PENDING
    static abstract IUbcValuePlane CreateValuePlane(int capacity);

    /// <summary>The family's instance state for one instantiation of a verified program.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D9D5B4
    // Broiler-Human:        PENDING
    static abstract object CreateInstance(UbcInstanceContext context);

    /// <summary>
    /// Whether the instance state <see cref="CreateInstance"/> made may be published: ready, a fault of
    /// the guest's that the instantiation answers, or an allowance that making it exhausted.
    /// </summary>
    /// <remarks>
    /// Asked once, in the instantiation step, right after <see cref="CreateInstance"/>. Any answer but
    /// ready abandons the state through <see cref="AbandonInstance"/> before the step answers, and an
    /// answer of no kind - a default <see cref="UbcInstanceAnswer"/> - is a contract violation. A family
    /// answers exhausted only after a charge the meter refused, so the core names the dimension from its
    /// own latch. The default answers ready: a family whose instance cannot fail to be made has nothing
    /// to say here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=B96FB4
    // Broiler-Falsified-If: the default answers anything but ready, so a family that declares no admission has its instances refused
    // Broiler-Human:        PENDING
    static virtual UbcInstanceAnswer AdmitInstance(object instanceState) => UbcInstanceAnswer.Ready;

    /// <summary>
    /// The unit an admitted instance runs before it is published, or minus one for none, which is the
    /// default.
    /// </summary>
    /// <remarks>
    /// The executor runs the unit in the instantiation step, under that step's meter, with no parameter
    /// bound and no completion payload made. It must be a unit the program has, and its signature must
    /// take and give nothing; any other unit is a contract violation. A fault there faults the
    /// instantiation with the family's payload, and any end but completion abandons the state.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=5C7C72
    // Broiler-Human:        PENDING
    static virtual int StartUnit(object instanceState) => -1;

    /// <summary>
    /// Releases what an instance state holds when it will never be published: after an admission that
    /// was not ready, a start unit that did not complete, or an exception from any family member in the
    /// instantiation step. Called at most once per state, in the step that made it. The default does
    /// nothing.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=0E3E76
    // Broiler-Human:        PENDING
    static virtual void AbandonInstance(object instanceState)
    {
    }

    /// <summary>
    /// Resolves the entry point an invocation names to the unit it starts, before any frame stands.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The executor charges one fuel unit before it asks, and it checks what a found answer names: a
    /// unit the program has, flagged <see cref="UbcUnitFlags.Entry"/>, or the step ends as a contract
    /// violation, as it does for an answer of no kind. A missing answer is faulted with
    /// <see cref="EntryRefused"/>'s payload, and a refused one with the family's own.
    /// </para>
    /// <para>
    /// The default answers from the artifact's Entries section: the unit the entry of exactly that name
    /// starts, or missing. A family whose entry points are named elsewhere - in its FamilyData, with
    /// arguments written in the name - overrides it. An answer carries a unit and not what the family
    /// parsed, so <see cref="BindParameters"/> reads the entry point again: the two readings must agree,
    /// and an entry point this member found and that one refuses is answered with
    /// <see cref="EntryRefused"/>'s payload.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=803D49
    // Broiler-Falsified-If: the default answers a unit for a name no entry of the artifact carries, or no unit for a name one carries
    // Broiler-Human:        PENDING
    static virtual UbcEntryAnswer ResolveEntry(object instanceState, UbcVerifiedProgram program, System.ReadOnlySpan<byte> entryPoint) =>
        program.TryGetEntry(entryPoint, out var unit) ? UbcEntryAnswer.Found(unit) : UbcEntryAnswer.Missing;

    /// <summary>
    /// Writes the parameters of the entry unit <paramref name="entryName"/> names into its locals, the
    /// first locals of the new frame, before its first instruction. Answers false when the family can
    /// supply none, which the executor answers with <see cref="EntryRefused"/>.
    /// </summary>
    /// <remarks>
    /// It is handed the entry point <see cref="ResolveEntry"/> resolved, and reads it again: a family
    /// that parses arguments out of the entry point parses them twice, once to choose the unit and once
    /// to bind them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2BCECF
    // Broiler-Human:        PENDING
    static abstract bool BindParameters(ref UbcActivation activation, System.ReadOnlySpan<byte> entryName);

    /// <summary>The payload of a completed invocation: the entry unit's results are at the argument bases.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E56990
    // Broiler-Human:        PENDING
    static abstract IVmProfilePayload? Completion(ref UbcActivation activation);

    /// <summary>
    /// The payload of a trap: a <c>trap</c> instruction's slot and code, or a primitive's trap mapped to
    /// the family's code. Slot zero code zero is the universal unreachable.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9881B7
    // Broiler-Human:        PENDING
    static abstract IVmProfilePayload Fault(ref UbcActivation activation, byte familySlot, ushort code);

    /// <summary>The payload of an exception no region caught, with the value in <see cref="UbcActivation.Pending"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BCBAFA
    // Broiler-Human:        PENDING
    static abstract IVmProfilePayload? Uncaught(ref UbcActivation activation);

    /// <summary>The payload a host sees when the step suspends, with the reason in <see cref="UbcActivation.Pending"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D36F78
    // Broiler-Human:        PENDING
    static abstract IVmProfilePayload? SuspendProjection(ref UbcActivation activation);

    /// <summary>The payload of an invocation that names no entry the family can bind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1A20F3
    // Broiler-Human:        PENDING
    static abstract IVmProfilePayload? EntryRefused(object instanceState, System.ReadOnlySpan<byte> entryName);

    /// <summary>The frame codec's capture half: the family's own record of <paramref name="count"/> values from <paramref name="start"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=333F7E
    // Broiler-Human:        PENDING
    static abstract object CaptureValues(IUbcValuePlane plane, int start, int count);

    /// <summary>The frame codec's restore half: writes a captured record back into the plane from <paramref name="start"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BBC584
    // Broiler-Human:        PENDING
    static abstract void RestoreValues(IUbcValuePlane plane, int start, object captured);
}

/// <summary>A family's value plane: an array of the family's own value type, reached by index.</summary>
/// <remarks>
/// The universal bytecode never learns the value type. It asks a plane to copy one value from one index
/// to another, to clear a range so the collector does not keep what the stack has dropped, and to
/// resize before a frame needs more room; the loop charges the allocation first. A handler that owns
/// the plane's type reads and writes it directly.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=424217
// Broiler-Human:        PENDING
public interface IUbcValuePlane
{
    /// <summary>How many values the plane holds room for.</summary>
    int Capacity { get; }

    /// <summary>The bytes one value is charged at when the plane grows.</summary>
    int ValueBytes { get; }

    /// <summary>Grows the plane to at least <paramref name="capacity"/> values, keeping what it holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=401A3F
    // Broiler-Human:        PENDING
    void Resize(int capacity);

    /// <summary>Copies the value at <paramref name="from"/> to <paramref name="to"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CBBB3A
    // Broiler-Human:        PENDING
    void Copy(int from, int to);

    /// <summary>Resets <paramref name="count"/> values from <paramref name="start"/> to the family's empty value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4D143C
    // Broiler-Human:        PENDING
    void Clear(int start, int count);
}

/// <summary>What a family is given when it creates the instance state of one instantiation.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=ED1984
// Broiler-Human:        PENDING
public sealed class UbcInstanceContext
{
    /// <summary>The context of one instantiation.</summary>
    public UbcInstanceContext(UbcVerifiedProgram program, IVmExecutionEnvironment environment)
    {
        Program = program;
        Environment = environment;
    }

    /// <summary>The verified program, with the state the family's hook left on it.</summary>
    public UbcVerifiedProgram Program { get; }

    /// <summary>The execution environment: the meter, the capabilities, the load mediator.</summary>
    public IVmExecutionEnvironment Environment { get; }
}

/// <summary>What <see cref="IUbcFamily.ResolveEntry"/> answered.</summary>
/// <remarks>
/// Zero is no answer, so that a default <see cref="UbcEntryAnswer"/> - a family that returned without
/// deciding - is refused rather than read as a unit.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=5A0E38
// Broiler-Human:        PENDING
public enum UbcEntryAnswerKind : byte
{
    /// <summary>No answer: the kind of a default value, which the executor answers as a contract violation.</summary>
    Unanswered = 0,

    /// <summary>The entry point starts <see cref="UbcEntryAnswer.Unit"/>.</summary>
    Found = 1,

    /// <summary>The family knows no entry of that name; the executor faults with <see cref="IUbcFamily.EntryRefused"/>'s payload.</summary>
    Missing = 2,

    /// <summary>The family refuses the entry point with a fault of its own, <see cref="UbcEntryAnswer.Fault"/>.</summary>
    Refused = 3,
}

/// <summary>A family's answer to an entry point: the unit it starts, no such entry, or a fault of the family's.</summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=22A419
// Broiler-Falsified-If: a default value answers found, or a refusal is built without the fault it carries
// Broiler-Human:        PENDING
public readonly struct UbcEntryAnswer
{
    private UbcEntryAnswer(UbcEntryAnswerKind kind, int unit, IVmProfilePayload? fault)
    {
        Kind = kind;
        Unit = unit;
        Fault = fault;
    }

    /// <summary>What the family answered.</summary>
    public UbcEntryAnswerKind Kind { get; }

    /// <summary>For <see cref="UbcEntryAnswerKind.Found"/>, the unit the entry point starts.</summary>
    public int Unit { get; }

    /// <summary>For <see cref="UbcEntryAnswerKind.Refused"/>, the fault the invocation ends in.</summary>
    public IVmProfilePayload? Fault { get; }

    /// <summary>No entry of that name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DAA8B6
    // Broiler-Human:        PENDING
    public static UbcEntryAnswer Missing => new(UbcEntryAnswerKind.Missing, -1, null);

    /// <summary>The entry point starts <paramref name="unit"/>, which the executor checks before it enters it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=715232
    // Broiler-Human:        PENDING
    public static UbcEntryAnswer Found(int unit) => new(UbcEntryAnswerKind.Found, unit, null);

    /// <summary>The family refuses the entry point, and the invocation faults with <paramref name="fault"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B864DB
    // Broiler-Human:        PENDING
    public static UbcEntryAnswer Refused(IVmProfilePayload fault)
    {
        System.ArgumentNullException.ThrowIfNull(fault);
        return new UbcEntryAnswer(UbcEntryAnswerKind.Refused, -1, fault);
    }
}

/// <summary>What <see cref="IUbcFamily.AdmitInstance"/> answered.</summary>
/// <remarks>
/// Zero is no answer, so that a default <see cref="UbcInstanceAnswer"/> is refused rather than read as
/// ready, and an instance whose making failed is never published by a family that forgot to say so.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=CBF8AC
// Broiler-Human:        PENDING
public enum UbcInstanceAnswerKind : byte
{
    /// <summary>No answer: the kind of a default value, which the executor answers as a contract violation.</summary>
    Unanswered = 0,

    /// <summary>The instance may be published.</summary>
    Ready = 1,

    /// <summary>Making the instance faulted the guest; the instantiation faults with <see cref="UbcInstanceAnswer.Fault"/>.</summary>
    Faulted = 2,

    /// <summary>Making the instance was refused a charge; the core names the dimension from its latch.</summary>
    Exhausted = 3,
}

/// <summary>A family's answer on an instance it made: ready, faulted with a payload, or exhausted.</summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=AF0376
// Broiler-Falsified-If: a default value answers ready, or a fault is built without the payload it carries
// Broiler-Human:        PENDING
public readonly struct UbcInstanceAnswer
{
    private UbcInstanceAnswer(UbcInstanceAnswerKind kind, IVmProfilePayload? fault)
    {
        Kind = kind;
        Fault = fault;
    }

    /// <summary>What the family answered.</summary>
    public UbcInstanceAnswerKind Kind { get; }

    /// <summary>For <see cref="UbcInstanceAnswerKind.Faulted"/>, the fault the instantiation ends in.</summary>
    public IVmProfilePayload? Fault { get; }

    /// <summary>The instance may be published.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4D2138
    // Broiler-Human:        PENDING
    public static UbcInstanceAnswer Ready => new(UbcInstanceAnswerKind.Ready, null);

    /// <summary>A charge made while the instance was made was refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B93757
    // Broiler-Human:        PENDING
    public static UbcInstanceAnswer Exhausted => new(UbcInstanceAnswerKind.Exhausted, null);

    /// <summary>Making the instance faulted, and the instantiation faults with <paramref name="fault"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=290FA8
    // Broiler-Human:        PENDING
    public static UbcInstanceAnswer Faulted(IVmProfilePayload fault)
    {
        System.ArgumentNullException.ThrowIfNull(fault);
        return new UbcInstanceAnswer(UbcInstanceAnswerKind.Faulted, fault);
    }
}

/// <summary>What a handler answers.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=46CB28
// Broiler-Human:        PENDING
public enum UbcStatusKind : byte
{
    /// <summary>Continue at the next instruction. For a call row, the call completed in the handler.</summary>
    Next = 0,

    /// <summary>A branch row: continue at the row's target, with the taken edge's pushes.</summary>
    Taken = 1,

    /// <summary>A call row: the activation holds a call request the emitter performs.</summary>
    Request = 2,

    /// <summary>A suspending row: the step suspends with the reason in <see cref="UbcActivation.Pending"/>.</summary>
    Suspend = 3,

    /// <summary>An exception is in flight, held in <see cref="UbcActivation.Pending"/>; the emitter unwinds.</summary>
    Threw = 4,

    /// <summary>A trap with a family code; no region catches it.</summary>
    Trap = 5,

    /// <summary>The family found an internal defect; the step ends as a contract violation.</summary>
    Defect = 6,
}

/// <summary>A handler's answer: a kind, and for a trap the family's code.</summary>
/// <remarks>
/// A status the row's kind does not admit is a defect of the family, and the emitter ends the step as a
/// contract violation rather than guessing what was meant: a dynamic row does not answer taken, a
/// branch row does not answer a request, only a suspending row answers suspend, and a terminal row does
/// not answer next.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=31699E
// Broiler-Human:        PENDING
public readonly struct UbcStatus : System.IEquatable<UbcStatus>
{
    private UbcStatus(UbcStatusKind kind, ushort code)
    {
        Kind = kind;
        Code = code;
    }

    /// <summary>The kind.</summary>
    public UbcStatusKind Kind { get; }

    /// <summary>For a trap, the family's trap code; zero otherwise.</summary>
    public ushort Code { get; }

    /// <summary>Continue at the next instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2AD6FA
    // Broiler-Human:        PENDING
    public static UbcStatus Next => new(UbcStatusKind.Next, 0);

    /// <summary>Take the branch.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2BD07C
    // Broiler-Human:        PENDING
    public static UbcStatus Taken => new(UbcStatusKind.Taken, 0);

    /// <summary>Perform the call request the activation holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=553C46
    // Broiler-Human:        PENDING
    public static UbcStatus Request => new(UbcStatusKind.Request, 0);

    /// <summary>Suspend the step.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=43B00C
    // Broiler-Human:        PENDING
    public static UbcStatus Suspend => new(UbcStatusKind.Suspend, 0);

    /// <summary>Unwind the exception the activation holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=414EAE
    // Broiler-Human:        PENDING
    public static UbcStatus Threw => new(UbcStatusKind.Threw, 0);

    /// <summary>An internal defect of the family.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=53B8E5
    // Broiler-Human:        PENDING
    public static UbcStatus Defect => new(UbcStatusKind.Defect, 0);

    /// <summary>A trap of the family's code <paramref name="code"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=92636E
    // Broiler-Human:        PENDING
    public static UbcStatus Trap(ushort code) => new(UbcStatusKind.Trap, code);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DAE7A6
    // Broiler-Human:        PENDING
    public bool Equals(UbcStatus other) => Kind == other.Kind && Code == other.Code;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UbcStatus other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => System.HashCode.Combine(Kind, Code);

    /// <summary>Value equality.</summary>
    public static bool operator ==(UbcStatus left, UbcStatus right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=047A46
    // Broiler-Human:        PENDING
    public static bool operator !=(UbcStatus left, UbcStatus right) => !left.Equals(right);
}

/// <summary>
/// A call request: the unit a call row's handler asks the emitter to enter - of the calling program, or
/// of another verified program of the same family, such as one a guest load answered with.
/// </summary>
/// <remarks>
/// <para>
/// The callee's parameters are slots of the row's input region, in the callee's signature's order, per
/// plane. For a row whose effect is listed or counted they are the region's top slots, and the callee's
/// signature must match the row's typed pops there and give exactly the row's pushes. For a row whose
/// effect is the signature form they are the region's bottom slots, beneath the row's trailing pops,
/// where the walk typed the named signature's parameters, and the callee's signature must equal that
/// named signature type for type. The handler may rearrange the region before it answers, and the
/// emitter copies the parameters into the callee's locals. When the callee returns, the row's inputs
/// are discarded and the callee's results are placed at the row's argument bases. A callee that does
/// not fit is a defect.
/// </para>
/// <para>
/// A program of another family is a defect of the handler that names it: the family boundary is not
/// crossed by a call, and the emitter refuses the request rather than run it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D9C29E
// Broiler-Falsified-If: a request naming a program of another family, or a unit outside the named program, can be performed
// Broiler-Human:        PENDING
public readonly struct UbcCallRequest : System.IEquatable<UbcCallRequest>
{
    /// <summary>A request to enter <paramref name="unit"/> of the calling program.</summary>
    public UbcCallRequest(int unit) => Unit = unit;

    /// <summary>A request to enter <paramref name="unit"/> of <paramref name="program"/>, a program of the same family.</summary>
    public UbcCallRequest(UbcVerifiedProgram program, int unit)
    {
        Program = program;
        Unit = unit;
    }

    /// <summary>The program the callee belongs to, or null for the calling program.</summary>
    public UbcVerifiedProgram? Program { get; }

    /// <summary>The callee's unit index.</summary>
    public int Unit { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=69A4B3
    // Broiler-Human:        PENDING
    public bool Equals(UbcCallRequest other) => Unit == other.Unit && ReferenceEquals(Program, other.Program);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UbcCallRequest other && Equals(other);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=85C392
    // Broiler-Human:        PENDING
    public override int GetHashCode() => System.HashCode.Combine(Unit, Program);

    /// <summary>Value equality.</summary>
    public static bool operator ==(UbcCallRequest left, UbcCallRequest right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=66BC2D
    // Broiler-Human:        PENDING
    public static bool operator !=(UbcCallRequest left, UbcCallRequest right) => !left.Equals(right);
}

/// <summary>
/// Everything a handler is given: its unit and instruction, both planes and where its inputs sit, the
/// family's instance state, the meter and the capability invoker - and the two outputs a handler may
/// set. Passed by reference, so a handler's writes are the emitter's to read.
/// </summary>
/// <remarks>
/// <para>
/// Fields rather than properties, deliberately: the activation is the one structure every instruction
/// of every family touches, and the loop and the handler share it by reference.
/// </para>
/// <para>
/// <b>What a handler may do with it.</b> Read <see cref="Program"/>, <see cref="Unit"/>, <see cref="Pc"/>,
/// <see cref="InstanceState"/>, <see cref="Meter"/> and <see cref="Capabilities"/>; read its inputs and
/// write its outputs in <see cref="Words"/> and <see cref="Values"/> at and above
/// <see cref="WordArgs"/> and <see cref="ValueArgs"/>, within its row's effect; set
/// <see cref="CallRequest"/> or <see cref="Pending"/> before answering the status that reads them. It
/// may not write any other field, and a loop is free to check that it did not.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=BAFCB2
// Broiler-Falsified-If: a handler reaches a value of another frame or another family through this structure, or the loop reads an output a handler did not answer the status for
// Broiler-Human:        PENDING
public struct UbcActivation
{
    /// <summary>The family's instance state, created at instantiation.</summary>
    public object? InstanceState;

    /// <summary>The meter the step charges.</summary>
    public IVmMeter Meter;

    /// <summary>The host capabilities the family's declaration imports, bound by index.</summary>
    public IVmHostCapabilityInvoker Capabilities;

    /// <summary>The word plane: every numeric slot of every frame, as bits.</summary>
    public ulong[] Words;

    /// <summary>The value plane: every language-value slot of every frame.</summary>
    public IUbcValuePlane Values;

    /// <summary>
    /// The program being executed: the instance's, or one a guest load answered with and a call request
    /// entered. Its <see cref="UbcVerifiedProgram.FamilyState"/> is what the hook read from that
    /// program's FamilyData.
    /// </summary>
    public UbcVerifiedProgram Program;

    /// <summary>The unit being executed, an index into <see cref="Program"/>'s units.</summary>
    public int Unit;

    /// <summary>The absolute code offset of the instruction being executed.</summary>
    public uint Pc;

    /// <summary>The index of the current frame's first word local.</summary>
    public int WordBase;

    /// <summary>The index of the current frame's first value local.</summary>
    public int ValueBase;

    /// <summary>The index of the first word the instruction pops, and where its word pushes begin.</summary>
    public int WordArgs;

    /// <summary>The index of the first value the instruction pops, and where its value pushes begin.</summary>
    public int ValueArgs;

    /// <summary>A call row's request, read when the handler answers <see cref="UbcStatusKind.Request"/>.</summary>
    public UbcCallRequest CallRequest;

    /// <summary>
    /// The exception in flight, read when the handler answers <see cref="UbcStatusKind.Threw"/>; or the
    /// suspension's reason, read when it answers <see cref="UbcStatusKind.Suspend"/>.
    /// </summary>
    public object? Pending;
}

/// <summary>What a family's hook answers: admit, a refusal with the family's own code, or an exhaustion.</summary>
/// <remarks>The hook may refuse and may not admit what the walk refused: it is called only for what the walk admitted.</remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3129A3
// Broiler-Human:        PENDING
public readonly struct UbcHookAnswer
{
    private UbcHookAnswer(bool refused, int familyCode, VmReason reason, VmBudgetDimension? exhausted)
    {
        Refused = refused;
        FamilyCode = familyCode;
        Reason = reason;
        Exhausted = exhausted;
    }

    /// <summary>True when the hook refused.</summary>
    public bool Refused { get; }

    /// <summary>The family's diagnostic code for a refusal.</summary>
    public int FamilyCode { get; }

    /// <summary>The core reason of a refusal; one reason per code, under the family's registry rules.</summary>
    public VmReason Reason { get; }

    /// <summary>The dimension, when the refusal is an exhaustion rather than an invalid artifact.</summary>
    public VmBudgetDimension? Exhausted { get; }

    /// <summary>Nothing the family knows refuses the instruction or section.</summary>
    public static UbcHookAnswer Admit => default;

    /// <summary>A refusal with the family's code and the one core reason it carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8E6A32
    // Broiler-Human:        PENDING
    public static UbcHookAnswer Refuse(int familyCode, VmReason reason) => new(true, familyCode, reason, null);

    /// <summary>A refusal because a budget the hook charged refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=66DE39
    // Broiler-Human:        PENDING
    public static UbcHookAnswer Exhaust(VmBudgetDimension dimension) => new(true, 0, VmReason.None, dimension);
}

/// <summary>
/// A family's verifier hook: what only the family knows, checked over its FamilyData section and at
/// every one of its instructions, after the walk has admitted them. It may refuse and may not admit.
/// </summary>
/// <remarks>
/// It is charged to the same verifier-work budget as the walk, through the meter it is handed, and it
/// is versioned with the family table it belongs to. It is not a second verifier: it cannot run without
/// the walk and it cannot contradict a refusal.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=4939EC
// Broiler-Falsified-If: an answer of this hook can turn a refusal of the walk into an admission
// Broiler-Human:        PENDING
public interface IUbcFamilyVerifier
{
    /// <summary>
    /// Reads the family's FamilyData section (empty when the artifact carries none) before any
    /// instruction is checked, and answers the state the later calls and the runtime receive.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6C3B6A
    // Broiler-Human:        PENDING
    UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState);

    /// <summary>Checks one family instruction the walk admitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=573648
    // Broiler-Human:        PENDING
    UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction);

    /// <summary>Checks what can only be checked once every instruction has been seen.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A1528E
    // Broiler-Human:        PENDING
    UbcHookAnswer End(object? familyState);
}

/// <summary>What the hook is shown of the artifact before the code walk.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=09F610
// Broiler-Human:        PENDING
public sealed class UbcHookArtifact
{
    /// <summary>The hook's view of an artifact.</summary>
    public UbcHookArtifact(UbcArtifact artifact, byte slot, UbcInstructionTable table, System.ReadOnlyMemory<byte> familyData, IVmMeter meter)
    {
        Artifact = artifact;
        Slot = slot;
        Table = table;
        FamilyData = familyData;
        Meter = meter;
    }

    /// <summary>The decoded artifact, framed and not yet walked.</summary>
    public UbcArtifact Artifact { get; }

    /// <summary>The slot the family occupies, or zero when the artifact declares no family.</summary>
    public byte Slot { get; }

    /// <summary>The table the artifact's manifest selects.</summary>
    public UbcInstructionTable Table { get; }

    /// <summary>The family's FamilyData section, empty when absent.</summary>
    public System.ReadOnlyMemory<byte> FamilyData { get; }

    /// <summary>The verification meter the hook charges its work to.</summary>
    public IVmMeter Meter { get; }
}

/// <summary>One family instruction the walk admitted, and the typed stack before it, read-only.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2F9CB8
// Broiler-Human:        PENDING
public readonly struct UbcHookInstruction
{
    private readonly UbcStackNode? stack;

    internal UbcHookInstruction(int unit, uint pc, byte opcode, ulong operand, UbcInstructionRow row, UbcStackNode? stack)
    {
        Unit = unit;
        Pc = pc;
        Opcode = opcode;
        Operand = operand;
        Row = row;
        this.stack = stack;
    }

    /// <summary>The unit.</summary>
    public int Unit { get; }

    /// <summary>The instruction's absolute code offset.</summary>
    public uint Pc { get; }

    /// <summary>The family opcode.</summary>
    public byte Opcode { get; }

    /// <summary>The operand, zero-extended.</summary>
    public ulong Operand { get; }

    /// <summary>The row the table describes the opcode with.</summary>
    public UbcInstructionRow Row { get; }

    /// <summary>The operand-stack height before the instruction, both planes together.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1FCAA2
    // Broiler-Human:        PENDING
    public int Height => stack?.Height ?? 0;

    /// <summary>The type of the slot <paramref name="depth"/> places below the top, zero being the top.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4FB4C5
    // Broiler-Falsified-If: a depth at or beyond the height answers a type rather than throwing
    // Broiler-Human:        PENDING
    public UbcSlotType TypeAt(int depth)
    {
        var node = stack;

        for (var index = 0; index < depth && node is not null; index++)
        {
            node = node.Below;
        }

        return node?.Type ?? throw new System.ArgumentOutOfRangeException(nameof(depth), depth, "below the bottom of the stack");
    }
}
