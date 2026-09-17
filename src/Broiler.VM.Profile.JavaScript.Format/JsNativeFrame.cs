// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           14
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  2/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>The instruction set and calling convention one emitted code section was written for.</summary>
/// <remarks>
/// <b>The ABI is part of the identity and not a second field, because the bytes differ.</b> The two
/// x86-64 conventions disagree about which register a frame pointer arrives in and about which
/// registers a callee must preserve, so one blob cannot be correct under both; naming the
/// instruction set alone would let an artifact built for one be run under the other, and the
/// failure would be silent register corruption rather than a refusal.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B8CFC5
// Broiler-Human:        PENDING
public enum JsNativeArchitecture : uint
{
    /// <summary>No architecture. An artifact naming it carries no emitted code.</summary>
    None = 0,

    /// <summary>x86-64 under the System V AMD64 convention: the frame pointer arrives in RDI.</summary>
    X64SystemV = 1,

    /// <summary>x86-64 under the Windows x64 convention: the frame pointer arrives in RCX.</summary>
    X64Windows = 2,

    /// <summary>arm64 under AAPCS64: the frame pointer arrives in X0.</summary>
    Arm64 = 3,
}

/// <summary>What an emitted code unit answers when it returns to its caller.</summary>
/// <remarks>
/// <para>
/// <b>THERE IS NO BAIL-OUT-TO-INTERPRETER CODE, WHICH IS WHY THERE IS NO CODE 1.</b> The obvious
/// design gives emitted code a way to give up and hand the rest of the unit back to the
/// interpreter, and this profile's non-goals refuse it: the output form is fixed when the artifact
/// is compiled and pinned when it is verified, one form per handle, no promotion. A guard that
/// bailed out would be selecting a form from run-time observation, which is the second execution
/// arm that paragraph names. So the gap at 1 is deliberate and permanent, and it is a gap rather
/// than a renumbering so that a reader who expects the obvious design finds the answer where they
/// look for it.
/// </para>
/// <para>
/// <b>Every value an emitted unit answers is one of three, and none of them is a JavaScript
/// value.</b> The result of a normal return is left in the frame's first operand slot, which is a
/// <c>double</c>; a throw and a fuel exhaustion carry nothing at all, because the emitted code has
/// no way to build an object and no meter to charge.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=3DF7AC
// Broiler-Human:        PENDING
public enum JsNativeReturn
{
    /// <summary>The unit returned normally. Its value is in the frame's operand slot zero.</summary>
    Returned = 0,

    /// <summary>The unit raised. The caller turns this into the guest exception the language owes.</summary>
    Threw = 2,

    /// <summary>The unit ran out of fuel. The caller ends the invocation as an exhaustion.</summary>
    FuelExhausted = 3,
}

/// <summary>
/// The frame an emitted code unit is called with: the one structure machine code and managed code
/// share, and the reason the collector never has to know that machine code is running.
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING IN THIS STRUCTURE IS A MANAGED REFERENCE, AND THAT IS THE WHOLE POINT OF IT.</b> The
/// core roadmap's rule for a native form is that a profile which cannot state where its emitted
/// code's references are rooted has not earned the form, whatever its benchmarks say. This profile
/// answers that BY CONSTRUCTION rather than by a rooting scheme: every field here is a pointer to
/// unmanaged storage or an integer, so there is no reference for the collector to trace, no object
/// for it to move out from under an instruction, and no write barrier for emitted code to have
/// forgotten. A scheme the collector had to be taught would be a scheme that could be wrong; this
/// cannot be wrong because there is nothing for it to be wrong about.
/// </para>
/// <para>
/// <b>What makes that possible is <see cref="JsNumericManifest"/> and nothing else.</b> This
/// profile's ordinary value is a struct carrying a <c>double</c>, a type tag and an
/// <c>object</c> reference, and a frame of those could not be handed to emitted code without the
/// scheme just refused. The numeric manifest admits no value that carries a reference - no object,
/// no string, no closure, no exception - so a frame of <c>double</c> is not a restriction of the
/// value model for this manifest; it IS the value model for this manifest. Widen the manifest and
/// this structure stops being sound, which is why the two are documented against each other.
/// <i>(Corrected 2026-09-15. That sentence remains true of THIS frame, and it was read as saying the
/// wide manifest could have no native form. It has one, decided by JSD-0025, and it neither widens
/// this manifest nor uses this structure: a baseline unit is entered with JsBaselineFrame, which
/// holds a handler table's address and an activation cookie, and every value it reaches stays in
/// managed memory.)</i>
/// </para>
/// <para>
/// <b>The layout is sequential and the field order is the declared one, so an emitter computes
/// offsets rather than being told them.</b> On a 64-bit target that is
/// <c>operands</c> at 0, <c>operandCount</c> at 8, <c>locals</c> at 16, <c>localCount</c> at 24,
/// <c>constants</c> at 32, <c>fuel</c> at 40 and <c>bailoutPc</c> at 48, with four bytes of padding
/// after each <c>int</c> that precedes a pointer and four at the tail - fifty-six bytes in all.
/// The padding is stated rather than avoided by reordering, because the field order is the order a
/// reader of the contract expects and a layout nobody wrote down is a layout two encoders can
/// disagree about.
/// </para>
/// <para>
/// <b><c>fuel</c> is a pointer to a counter and not a meter.</b> The interpreter charges its meter
/// once per instruction, which emitted code cannot do without a managed call per instruction; what
/// emitted code can do is decrement an unmanaged counter and answer
/// <see cref="JsNativeReturn.FuelExhausted"/> when it goes negative. THE CONSEQUENCE IS THAT THE
/// TWO FORMS EXHAUST AT DIFFERENT POINTS, and that is a real difference between the forms rather
/// than an implementation detail: it is stated here so that a corpus row pinning a resource
/// exhaustion is not read as pinning it for both.
/// <i>(Corrected 2026-09-15: the two forms meant here are the interpreter and the numeric form. The
/// wide manifest's baseline form makes the managed calls this paragraph calls out of reach for
/// emitted code - one at every block head, whose step charges each instruction at the interpreter's
/// own point - so its exhaustion is
/// exact except where a guest-loaded program's verification is charged - JSC-219.)</i>
/// </para>
/// <para>
/// <b><c>bailoutPc</c> is named for the field the refused design would have used and it is not that
/// field.</b> Nothing resumes from it, because there is nothing to resume into. It records the
/// bytecode offset the emitted code had reached when it stopped, so that a throw or an exhaustion
/// can be reported against a position in the unit the artifact also carries; a caller that treated
/// it as a resumption point would be building the second execution arm this profile refuses.
/// </para>
/// <para>
/// <b>This build declares the contract and calls nothing through it.</b> No page is mapped, no byte
/// is emitted and no pointer here is ever dereferenced by anything in this repository. The
/// structure exists so that the two encoders that will fill it are written against a contract that
/// was decided once, in the open, rather than against whichever one of them was written first.
/// <i>(Corrected 2026-09-15. Every factual clause of this paragraph was already overtaken on
/// 2026-09-07, when the numeric form's encoders, the arming type and the numeric executor landed and
/// began filling this structure and calling emitted code through it; the paragraph is marked rather
/// than deleted, and the reason in its last sentence is what survives. This is the numeric form's
/// frame only: the wide manifest's baseline form is entered through JsBaselineFrame.)</i>
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=B132E7
// Broiler-Falsified-If: a field of this structure holds a reference the collector traces, or its declared offsets differ from the layout the runtime gives it
// Broiler-Human:        PENDING
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public unsafe struct JsNativeFrame
{
    /// <summary>The operand stack: unmanaged storage the emitted unit pushes and pops through.</summary>
    public double* Operands;

    /// <summary>How many operand slots <see cref="Operands"/> holds.</summary>
    public int OperandCount;

    /// <summary>The unit's environment slots, one <c>double</c> each.</summary>
    public double* Locals;

    /// <summary>How many slots <see cref="Locals"/> holds.</summary>
    public int LocalCount;

    /// <summary>The unit's numeric constants, in constant-pool order.</summary>
    public double* Constants;

    /// <summary>The remaining fuel, decremented by the emitted code itself.</summary>
    public long* Fuel;

    /// <summary>The bytecode offset the emitted code had reached when it stopped.</summary>
    public int BailoutPc;
}
