// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           7
// Human-reviewed:   0/9
// IP risk:          None
// Security risk:    Critical
// Criteria:         8/8
// Resource impact:  0/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>Which of the two native forms an emitted code section is, read off the artifact's manifest.</summary>
/// <remarks>
/// <para>
/// <b>THE TIER IS NOT A FIELD OF THE ARTIFACT, BECAUSE THE MANIFEST ALREADY SAYS IT.</b> An artifact
/// declaring <c>broiler.javascript.numeric</c> carries computing templates over a slab of
/// <c>double</c>; an artifact declaring <c>broiler.javascript.wide</c> carries the baseline form, whose
/// emitted code calls into the interpreter's own dispatch at every block head. A second
/// field naming the same fact would be a second place for the two to disagree, and the verifier would
/// have to choose which one to believe.
/// </para>
/// <para>
/// <b>It selects a template table and nothing else.</b> The scanner, the re-emitting verifier and the
/// emitter each ask it which table a blob is judged against; no executor asks it which arm to run,
/// because an instance of a wide artifact is always run by the engine and an instance of a numeric one
/// never is.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2E4C4B
// Broiler-Human:        PENDING
public enum JsNativeTier : byte
{
    /// <summary>The numeric manifest's computing form: values are doubles in unmanaged slabs.</summary>
    Numeric = 0,

    /// <summary>The wide manifest's baseline form: values stay in managed memory, reached only by handlers.</summary>
    Baseline = 1,
}

/// <summary>
/// What a baseline handler or a baseline unit answers: a negative status, or - from a handler only - the
/// bytecode offset the next instruction starts at.
/// </summary>
/// <remarks>
/// <para>
/// <b>A HANDLER'S ANSWER IS A PROGRAM COUNTER OR ONE OF THESE, AND THE TWO CANNOT BE CONFUSED.</b> A
/// bytecode offset is never negative, so the emitted code tests the sign once and every negative answer
/// leaves the unit. A unit itself answers only a status: it has no value of its own to return, because
/// the value a JavaScript return carries stays in the managed activation that the handler wrote it to.
/// </para>
/// <para>
/// <b>There is no bail-out status here either, for the numeric form's reason.</b> Nothing a baseline unit
/// answers asks its caller to finish the unit some other way. <see cref="Defect"/> is not a way out of
/// the form: it is what a handler answers when the emitted code asked it to run an instruction the
/// managed side did not expect, and the caller turns it into an internal defect rather than into any
/// JavaScript answer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=B08268
// Broiler-Falsified-If: a handler or unit answers a negative value other than these three, or a non-negative value that is not the offset of an instruction start
// Broiler-Human:        PENDING
public enum JsBaselineStatus
{
    /// <summary>
    /// The activation left the unit through a return or a suspension; its value is in the activation.
    /// </summary>
    Exit = -1,

    /// <summary>
    /// An exception escaped the unit's own exception regions; the activation holds it for the caller to raise.
    /// </summary>
    Threw = -2,

    /// <summary>
    /// The emitted code asked for an instruction the managed side did not compute; nothing ran.
    /// </summary>
    Defect = -3,
}

/// <summary>
/// The frame a baseline unit is entered with: the one structure the emitted code and the handlers share.
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING IN THIS STRUCTURE IS A MANAGED REFERENCE, AND NEITHER IS ANYTHING IT POINTS AT.</b>
/// <see cref="Handlers"/> is the address of a table of function pointers in unmanaged memory that lives
/// for the process, and <see cref="Cookie"/> is an integer. Every JavaScript value an instruction reads or
/// writes lives in a managed activation that the handler reaches from managed code and checks against
/// the cookie before it touches anything; the emitted code never holds the activation, never addresses a
/// value and never dereferences anything but the table. That is how the wide manifest's native form
/// answers the rule that a profile which cannot say where its emitted code's references are rooted has
/// not earned the form: the references are rooted where the collector can see them - by the managed
/// frame below the emitted one - and the emitted frame carries none.
/// </para>
/// <para>
/// <b>The layout is sequential and stated</b>: <see cref="Handlers"/> at 0 and <see cref="Cookie"/> at 8,
/// sixteen bytes on a 64-bit target, restated as <see cref="JsBaselineAbi"/>'s constants so the emitter in
/// the lowering assembly computes nothing about it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=0; Fingerprint=654AD3
// Broiler-Falsified-If: a field of this structure is, or contains, a reference the collector traces, or its offsets differ from the constants in JsBaselineAbi
// Broiler-Human:        PENDING
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct JsBaselineFrame
{
    /// <summary>The base address of the handler table: one function pointer per opcode byte.</summary>
    public nint Handlers;

    /// <summary>The identity of the activation this frame was built for.</summary>
    public long Cookie;
}

/// <summary>The baseline form's frame layout and stack reservation, as the emitter and the runtime both read them.</summary>
/// <remarks>
/// <b>One statement of each number, in the assembly both halves reference.</b> The emitter is in the
/// lowering assembly and the handlers are in the profile assembly; the profile may not reference its
/// lowering (rule N1) and the lowering does not reference the profile, so a constant restated in each
/// would be two constants that could drift.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5343D7
// Broiler-Falsified-If: a constant here disagrees with the layout the runtime gives JsBaselineFrame, or with the reservation the emitted prologue makes
// Broiler-Human:        PENDING
public static class JsBaselineAbi
{
    /// <summary>The offset of <see cref="JsBaselineFrame.Handlers"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=A0D518
    // Broiler-Falsified-If: the runtime places JsBaselineFrame.Handlers at any other offset
    // Broiler-Human:        PENDING
    public const int HandlersOffset = 0;

    /// <summary>The offset of <see cref="JsBaselineFrame.Cookie"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5C496D
    // Broiler-Falsified-If: the runtime places JsBaselineFrame.Cookie at any other offset
    // Broiler-Human:        PENDING
    public const int CookieOffset = 8;

    /// <summary>The size of <see cref="JsBaselineFrame"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=168EAF
    // Broiler-Falsified-If: the runtime gives JsBaselineFrame any other size
    // Broiler-Human:        PENDING
    public const int FrameSize = 16;

    /// <summary>How many slots the handler table has: one per value an opcode byte can take.</summary>
    /// <remarks>
    /// <b>Indexed by the opcode byte itself, so there is no slot numbering to drift from the opcode
    /// table.</b> A byte no opcode takes holds a handler that answers <see cref="JsBaselineStatus.Defect"/>
    /// and touches nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7D95B4
    // Broiler-Falsified-If: an emitted call can index the table at or beyond this many slots
    // Broiler-Human:        PENDING
    public const int HandlerSlots = 256;

    /// <summary>The bytes a baseline unit's prologue subtracts from the stack pointer after its two pushes.</summary>
    /// <remarks>
    /// <b>Forty on Windows x64 and eight under System V, and both leave the stack sixteen-aligned at every
    /// call.</b> The entry leaves the stack at eight modulo sixteen, two pushes keep it there, and the
    /// reservation brings it to zero: under Windows the forty bytes are the thirty-two the callee may use
    /// as shadow space plus eight of alignment, and under System V there is no shadow space to reserve.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=ED86E7
    // Broiler-Falsified-If: a baseline unit calls a handler with the stack pointer not sixteen-aligned or with less shadow space than the convention requires
    // Broiler-Human:        PENDING
    public static int FrameBytes(JsNativeArchitecture architecture) =>
        architecture == JsNativeArchitecture.X64Windows ? 40 : 8;
}
