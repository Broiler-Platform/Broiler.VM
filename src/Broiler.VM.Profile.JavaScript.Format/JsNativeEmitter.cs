// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           9
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  1/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The two bit patterns a slab of <c>double</c>s uses for the two things that are not numbers.
/// </summary>
/// <remarks>
/// <para>
/// <b>A SLAB OF DOUBLES HAS TO SAY "NOT A NUMBER YET" SOMEHOW, AND EVERY WAY OF SAYING IT IS A
/// TRADE.</b> A parallel array of flags costs a second load on every read; a tagged union costs the
/// whole point of the numeric manifest. What is done instead is to reserve two NaN payloads, and
/// what makes that admissible rather than a hack is that the reservation is stated here, in the
/// assembly both the emitter and the executor depend on, rather than being a magic number in two
/// places that can drift apart.
/// </para>
/// <para>
/// <b>THE PAYLOADS ARE CHOSEN SO THAT NO ARITHMETIC PRODUCES THEM.</b> An SSE2 operation whose
/// operands are all numbers and whose result is not a number produces the architecture's default
/// quiet NaN, which is the NEGATIVE quiet NaN with a zero payload; these two are positive quiet
/// NaNs with payloads of seventeen and eighteen. A NaN can still be PROPAGATED - an operation with
/// a NaN operand answers with that operand quieted - which is why the emitter refuses to let either
/// of these reach an arithmetic instruction at all rather than relying on the payload to survive.
/// </para>
/// <para>
/// <b>What this does NOT buy is a general tagged value, and a reader should not read it as
/// one.</b> There are two reserved patterns and there will not be a third: Boolean is not one of
/// them, because a Boolean has to be a Number the moment anything adds one to it, and the emitter
/// answers that by refusing to let a comparison's result travel anywhere but into the branch that
/// follows it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=5F6922
// Broiler-Falsified-If: an arithmetic operation over ordinary Numbers produces either of these bit patterns
// Broiler-Human:        PENDING
public static class JsNativeValues
{
    /// <summary>
    /// The pattern a binding slot holds before anything initialises it, and reading it is a
    /// <c>ReferenceError</c>.
    /// </summary>
    /// <remarks>
    /// <b>It is what makes the temporal dead zone observable in emitted code.</b> A
    /// <c>let</c> or <c>const</c> binding read before its initialiser runs is a
    /// <c>ReferenceError</c> in the language and there is no number that means that, so the
    /// emitter compares against this pattern on every read of a realm binding and answers
    /// <see cref="JsNativeReturn.Threw"/> when it matches. Without it, a program that reads a
    /// binding early would answer a number the interpreter never would.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=10ECDD
    // Broiler-Falsified-If: a realm binding is readable by emitted code before an initialiser stored to it
    // Broiler-Human:        PENDING
    public const long UninitialisedBits = 0x7FF8_0000_0000_0011L;

    /// <summary>The pattern that means <c>undefined</c>.</summary>
    /// <remarks>
    /// <b>It exists because the completion value of a statement can be <c>undefined</c> even
    /// though this manifest admits no way to write one.</b> A lowering gives every program body a
    /// completion slot and initialises it before the first statement, so a program whose last
    /// statement completes with nothing - an <c>if</c> that did not run its consequent, say -
    /// answers <c>undefined</c>, and answering NaN instead would be a wrong answer rather than a
    /// coarser one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0D2D4C
    // Broiler-Human:        PENDING
    public const long UndefinedBits = 0x7FF8_0000_0000_0012L;

    /// <summary>The bit pattern of <paramref name="value"/> as a signed 64-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=07E662
    // Broiler-Human:        PENDING
    public static long Bits(double value) => System.BitConverter.DoubleToInt64Bits(value);

    /// <summary>The <c>double</c> whose bit pattern is <paramref name="bits"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=933A6C
    // Broiler-Human:        PENDING
    public static double FromBits(long bits) => System.BitConverter.Int64BitsToDouble(bits);
}

/// <summary>
/// Everything an emitter of machine code reads, and nothing else: the one input a backend is a
/// pure function of.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT LIVES IN THE FORMAT ASSEMBLY BECAUSE TWO PARTIES BUILD IT AND NEITHER MAY SEE THE
/// OTHER.</b> The lowering builds one from the program it has just assembled; the verifier builds
/// one from the artifact it has just read. Re-emission equality - recompile the carried bytecode
/// and require the same bytes - is a claim about those two projections agreeing, and it can only be
/// stated at all if both are projections onto the SAME structure. Put this record in the lowering
/// and the verifier cannot reach it; put it in the profile and the lowering cannot; the format is
/// the pivot both already depend on.
/// </para>
/// <para>
/// <b>The constants arrive DECODED here where they arrive encoded at the backend seam, and the
/// difference is which question is being asked.</b> A backend is handed a whole program and asked
/// what form to give it, so it is handed the pool as the artifact carries it; an emitter is handed
/// what it will actually reference, which for the numeric manifest is one <c>double</c> per entry
/// and a flag saying whether that entry was a Number at all. An emitter that had to decode tags
/// would be a second decoder of a section the verifier has already decoded, and two decoders of one
/// section is two chances to disagree.
/// </para>
/// </remarks>
/// <param name="Code">Every code unit's bytecode, back to back, exactly as the artifact carries it.</param>
/// <param name="Functions">One row per code unit, in code-unit order, tiling <paramref name="Code"/>.</param>
/// <param name="ConstantValues">
/// One <c>double</c> per constant-pool entry: the entry's value where it is a Number, and zero
/// where it is not. A zero here means nothing on its own - <paramref name="ConstantIsNumber"/> is
/// what says whether the slot carries a value at all.
/// </param>
/// <param name="ConstantIsNumber">
/// Whether each pool entry is a Number. An emitter for the numeric manifest refuses a program that
/// reads an entry this array says false for, rather than substituting a value for it.
/// </param>
/// <param name="MaximumOperandStack">The deepest operand stack any unit declares.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E64DAB
// Broiler-Human:        PENDING
public sealed record JsNativeProgramImage(
    byte[] Code,
    JsFunctionRow[] Functions,
    double[] ConstantValues,
    bool[] ConstantIsNumber,
    uint MaximumOperandStack);

/// <summary>
/// An encoder of machine code for one architecture, seen from the side that only wants the bytes.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE INTERFACE A VERIFIER HOLDS AND <c>IJsNativeBackend</c> IS THE ONE A COMPILER
/// HOLDS, AND THEY ARE TWO INTERFACES ON PURPOSE.</b> A compiler asks a backend to give an
/// artifact a form and receives a refusal it turns into a source diagnostic; a verifier asks an
/// emitter to reproduce bytes it is holding and receives an answer it turns into an accept or a
/// reject. The verifier assembly cannot reference the lowering assembly at all - that absence is
/// what makes an execution-only composition free of a compiler - so the shared half is declared
/// here and the lowering's own seam stays where the lowering is.
/// </para>
/// <para>
/// <b>An implementation must be a PURE FUNCTION of its argument.</b> Re-emission equality is the
/// only sense in which machine code carried by an artifact is verifiable at all, and it works by
/// running the emitter again and comparing. An emitter that read a clock, a hash order, an
/// environment variable or the host's own architecture would make that comparison fail for a
/// correct artifact, which is worse than not making it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A64BA7
// Broiler-Human:        PENDING
public interface IJsNativeEmitter
{
    /// <summary>The instruction set and calling convention this emitter writes for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BFE8C5
    // Broiler-Human:        PENDING
    JsNativeArchitecture Architecture { get; }

    /// <summary>This emitter's version, which an artifact carries and a reader compares.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A5ABAA
    // Broiler-Human:        PENDING
    uint SemanticVersion { get; }

    /// <summary>The alignment every code unit's entry point is written at.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=96662D
    // Broiler-Human:        PENDING
    uint CodeAlignment { get; }

    /// <summary>Emits the whole image, or refuses it and says why.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=795E8A
    // Broiler-Human:        PENDING
    bool TryEmit(
        JsNativeProgramImage image,
        out byte[] code,
        out JsNativeSymbolRow[] symbols,
        out string refusal);
}

/// <summary>
/// What a trampoline records about one call into emitted code: the stack pointer either side of it
/// and every callee-saved register afterwards.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS STRUCTURE EXISTS BECAUSE OF THE <c>ret 8</c> ANECDOTE THE CORE ROADMAP RETAINS AS A
/// FIXTURE.</b> That defect was a callee that removed the wrong number of bytes from the stack, and
/// what made it expensive was that nothing failed at the call: the process ran for hours and then
/// died somewhere unrelated. The lesson the fixture carries is that a delayed death has to be
/// forced to arrive early, and the way to force it is to look at the stack pointer immediately
/// either side of the call rather than to wait for the consequence.
/// </para>
/// <para>
/// <b>Neither x86-64 convention can represent that defect, and the observation is made anyway.</b>
/// Both are caller-pops and neither has a return form that takes a count, so the specific defect is
/// unrepresentable here - but "the convention makes it impossible" is a claim, and a claim with no
/// test is exactly how the original one was made. What the observation DOES catch on this
/// architecture is the neighbouring family: a prologue and epilogue that do not agree, a
/// callee-saved register a callee forgot to restore, and a stack that is not where the convention
/// says it should be at a call.
/// </para>
/// <para>
/// <b>The register array is all sixteen-slot-worth of the callee-saved candidates and not one
/// convention's list</b>, because the two conventions disagree about RDI and RSI and the whole
/// value of recording them is to be able to check the row for the convention actually in use.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=68CB0A
// Broiler-Falsified-If: a field offset here differs from the offset the trampoline that fills it computes
// Broiler-Human:        PENDING
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public unsafe struct JsNativeAbiProbe
{
    /// <summary>The address the trampoline calls.</summary>
    public void* Target;

    /// <summary>The frame the trampoline passes to it.</summary>
    public JsNativeFrame* Frame;

    /// <summary>The stack pointer immediately before the call.</summary>
    public long StackBefore;

    /// <summary>The stack pointer immediately after it returned.</summary>
    public long StackAfter;

    /// <summary>What the call answered.</summary>
    public long Answer;

    /// <summary>
    /// RBX, RBP, RSI, RDI, R12, R13, R14 and R15 as they were when the call returned, in that
    /// order.
    /// </summary>
    public fixed long Saved[8];
}

/// <summary>Where each field of <see cref="JsNativeAbiProbe"/> sits, for the trampoline to use.</summary>
/// <remarks>
/// <b>A trampoline is machine code and machine code computes with numbers.</b> The structure states
/// its layout by being sequential; these are the same fact in the form an encoder uses, and a check
/// beside them measures the real structure so the two cannot drift apart silently.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A42497
// Broiler-Falsified-If: an offset here differs from the offset the runtime gives that field
// Broiler-Human:        PENDING
public static class JsNativeAbiProbeLayout
{
    /// <summary>Where the target address sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2F56B7
    // Broiler-Human:        PENDING
    public const int TargetOffset = 0;

    /// <summary>Where the frame pointer sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=20E757
    // Broiler-Human:        PENDING
    public const int FrameOffset = 8;

    /// <summary>Where the stack pointer before the call is recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CB21B9
    // Broiler-Human:        PENDING
    public const int StackBeforeOffset = 16;

    /// <summary>Where the stack pointer after the call is recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E66DD9
    // Broiler-Human:        PENDING
    public const int StackAfterOffset = 24;

    /// <summary>Where the answer is recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2C7240
    // Broiler-Human:        PENDING
    public const int AnswerOffset = 32;

    /// <summary>Where the first saved register is recorded; the rest follow eight bytes apart.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D18692
    // Broiler-Human:        PENDING
    public const int SavedOffset = 40;

    /// <summary>How many bytes the whole structure occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8CE7FC
    // Broiler-Human:        PENDING
    public const int Bytes = 104;
}
