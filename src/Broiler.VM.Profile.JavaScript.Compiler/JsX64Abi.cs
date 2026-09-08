// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           2
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  1/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// One row of the x86-64 calling-convention table: everything the two conventions disagree about,
/// written down as data.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS A TABLE BECAUSE THE ALTERNATIVE IS HABIT, AND HABIT IS WHAT THE DISAGREEMENT PUNISHES.</b>
/// An emitter written by somebody who last worked on Linux reaches for RDI without thinking; the
/// same emitter run on Windows then reads a frame pointer out of a register that holds whatever the
/// caller left there. Nothing faults. The frame's fields are read from a wild address, the values
/// are garbage, and if the address happens to be mapped the program answers a wrong number. Writing
/// the differences as fields of a record with one instance per convention means the emitter cannot
/// express the habit: there is no register named in it, only a field looked up.
/// </para>
/// <para>
/// <b>The single most likely silent-corruption source is RDI and RSI, and it is the third and
/// fourth fields for that reason.</b> They are callee-saved under the Windows x64 convention and
/// VOLATILE under System V. An emitter that used RSI as a scratch register would be correct on
/// Linux and would quietly destroy a caller's value on Windows - and the caller here is the CLR,
/// so what is destroyed is whatever the runtime had live across the call. This backend uses
/// neither, which is a decision this record exists to make visible rather than a coincidence.
/// </para>
/// <para>
/// <b>x86-32 is absent from this table and that is a scope decision with a reason.</b> It is the
/// only callee-pops convention in the whole declared matrix and the source of the <c>ret 8</c>
/// anecdote the core roadmap retains as a fixture. Both conventions here are caller-pops, so the
/// defect class is not merely untested but unrepresentable, and adding the one convention that can
/// represent it is not a thing to do by accident.
/// </para>
/// </remarks>
/// <param name="Architecture">The identity an artifact carries for code emitted under this row.</param>
/// <param name="Name">The name a caller selects this row's backend by.</param>
/// <param name="FramePointerRegister">
/// The register the single <c>JsNativeFrame*</c> argument arrives in: RCX under Windows x64, RDI
/// under System V.
/// </param>
/// <param name="ShadowSpaceBytes">
/// The bytes a caller must reserve below its own frame before any call: thirty-two under Windows
/// x64, none under System V. It is reserved here even though this backend calls nothing outside its
/// own emitted blob, because the obligation is the CALLER'S and an emitted unit is a caller.
/// </param>
/// <param name="StackAlignmentAtCall">
/// What the stack pointer must be congruent to, modulo sixteen, at the moment a <c>call</c>
/// executes. Both conventions say zero; it is a field rather than a constant so that the test that
/// injects a violation has a row to injure.
/// </param>
/// <param name="CalleeSavedRegisters">
/// Every register a callee must leave as it found it. The two lists differ, and comparing them is
/// the whole point of holding both.
/// </param>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5B0412
// Broiler-Falsified-If: a field of a row here differs from the calling convention that row names
// Broiler-Human:        PENDING
public sealed record JsX64Abi(
    JsNativeArchitecture Architecture,
    string Name,
    JsX64Register FramePointerRegister,
    int ShadowSpaceBytes,
    int StackAlignmentAtCall,
    JsX64Register[] CalleeSavedRegisters)
{
    /// <summary>The Windows x64 convention.</summary>
    /// <remarks>
    /// <b>RDI and RSI are in this list and are absent from System V's, and that one difference is
    /// the reason this table exists.</b> Thirty-two bytes of shadow space are the caller's to
    /// reserve whether or not the callee uses them; a caller that skips it is correct until the
    /// callee spills its arguments, which is a decision the callee makes and the caller cannot see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9FC3BF
    // Broiler-Falsified-If: this row names a frame-pointer register, a shadow-space size or a callee-saved set the Windows x64 convention does not
    // Broiler-Human:        PENDING
    public static JsX64Abi Windows { get; } = new(
        JsNativeArchitecture.X64Windows,
        JsNativeBackends.X64Windows,
        JsX64Register.Rcx,
        32,
        0,
        [
            JsX64Register.Rbx,
            JsX64Register.Rbp,
            JsX64Register.Rdi,
            JsX64Register.Rsi,
            JsX64Register.R12,
            JsX64Register.R13,
            JsX64Register.R14,
            JsX64Register.R15,
        ]);

    /// <summary>The System V AMD64 convention.</summary>
    /// <remarks>
    /// <b>RDI and RSI are VOLATILE here and the frame pointer arrives in RDI</b>, so the register
    /// that carries the argument is also one a callee may destroy - which is precisely why this
    /// backend spills it to its own stack frame in the prologue and reloads it rather than trusting
    /// it to survive a call.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=38DF78
    // Broiler-Falsified-If: this row names a frame-pointer register, a shadow-space size or a callee-saved set the System V AMD64 convention does not
    // Broiler-Human:        PENDING
    public static JsX64Abi SystemV { get; } = new(
        JsNativeArchitecture.X64SystemV,
        JsNativeBackends.X64SystemV,
        JsX64Register.Rdi,
        0,
        0,
        [
            JsX64Register.Rbx,
            JsX64Register.Rbp,
            JsX64Register.R12,
            JsX64Register.R13,
            JsX64Register.R14,
            JsX64Register.R15,
        ]);

    /// <summary>Both rows, in the order their architecture values ascend.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4ACC2C
    // Broiler-Human:        PENDING
    public static JsX64Abi[] Rows => [SystemV, Windows];

    /// <summary>The convention the process this code is running in actually uses.</summary>
    /// <remarks>
    /// <b>THIS IS THE ONLY PLACE IN THE BACKEND THAT ASKS THE MACHINE ANYTHING, AND IT IS NOT ON
    /// THE COMPILATION PATH.</b> Which convention an artifact is emitted for is a property of the
    /// artifact and is chosen by the caller that names a backend; a compiler that read the host
    /// would answer one source with two artifacts on two machines and break the determinism rule
    /// re-emission equality rests on. What this property answers is the different question an
    /// EXECUTOR asks - "may this process call into a blob emitted for that convention" - and the
    /// answer to that is a fact about the running machine by definition.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BD9956
    // Broiler-Falsified-If: this is consulted anywhere on a path that decides what bytes to emit
    // Broiler-Human:        PENDING
    public static JsX64Abi Host =>
        System.OperatingSystem.IsWindows() ? Windows : SystemV;

    /// <summary>Whether <paramref name="register"/> is one a callee of this convention must preserve.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=36878D
    // Broiler-Human:        PENDING
    public bool IsCalleeSaved(JsX64Register register)
    {
        foreach (var saved in CalleeSavedRegisters)
        {
            if (saved == register)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The bytes an emitted unit subtracts from the stack pointer after saving the one callee-saved
    /// register it uses.
    /// </summary>
    /// <remarks>
    /// <b>The arithmetic is written out rather than folded to a constant, because the constant is
    /// different on the two rows and a reader has to be able to see why.</b> A <c>call</c> leaves
    /// the stack pointer eight past a sixteen-byte boundary; one <c>push</c> brings it back to the
    /// boundary; so the frame this reserves must itself be a multiple of sixteen for the next
    /// <c>call</c> to be made from a boundary. It holds the shadow space this convention makes the
    /// caller reserve, plus the eight bytes this backend spills the frame pointer into.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=715C84
    // Broiler-Falsified-If: this is not a multiple of sixteen, or it does not leave room for both the shadow space and the frame-pointer spill
    // Broiler-Human:        PENDING
    public int FrameBytes => Align16(ShadowSpaceBytes + 8);

    /// <summary>Where in the reserved frame the incoming <c>JsNativeFrame*</c> is spilled.</summary>
    /// <remarks>
    /// <b>Above the shadow space, because the shadow space belongs to the callee of the next call
    /// and not to this frame.</b> Spilling into it would be handing a callee thirty-two bytes it is
    /// entitled to overwrite and then reading the frame pointer back out of them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EB800D
    // Broiler-Falsified-If: this offset lands inside the shadow space a callee may overwrite
    // Broiler-Human:        PENDING
    public int FramePointerSlot => ShadowSpaceBytes;

    /// <summary>Rounds up to the next multiple of sixteen.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9367AF
    // Broiler-Human:        PENDING
    private static int Align16(int value) => (value + 15) & ~15;
}
