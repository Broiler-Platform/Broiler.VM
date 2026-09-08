// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           8
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    Critical
// Criteria:         12/12
// Resource impact:  4/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What a mapping is allowed to be asked for next.</summary>
/// <remarks>
/// <b>THE STATE IS THE ENFORCEMENT AND NOT A CONVENIENCE.</b> Write-exclusive-or-execute is a
/// property of a mechanism rather than of a habit: a page that is writable and executable at the
/// same time is one in which a defect anywhere else in the process turns into arbitrary code
/// execution here. The rule cannot be enforced by remembering to call the two methods in the right
/// order, because remembering is what fails. It is enforced by there being no order in which both
/// are true - writing after <see cref="JsNativePage.Arm"/> throws, and taking an entry pointer
/// before it throws.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=89CCAB
// Broiler-Falsified-If: a mapping this type produced is both writable and executable at any instant
// Broiler-Human:        PENDING
internal enum JsNativePageState
{
    /// <summary>Mapped readable and writable, and not executable.</summary>
    Writable = 0,

    /// <summary>Mapped readable and executable, and not writable.</summary>
    Armed = 1,

    /// <summary>Unmapped. Nothing may be asked of it.</summary>
    Released = 2,
}

/// <summary>
/// The one place in this repository that makes memory executable.
/// </summary>
/// <remarks>
/// <para>
/// <b>ONE PLACE, AND THAT IS THE WHOLE DESIGN.</b> Every operating-system call that reserves,
/// protects or releases a mapping in this product is written in this file and its two
/// platform halves, and nowhere else. A capability spread over three files is a capability nobody
/// can audit; a capability in one file is one a reader can read in an afternoon and a rule can
/// assert about by name. The rule that asserts it is a source scan, in the same idiom as the one
/// that pins where a source position may be constructed.
/// </para>
/// <para>
/// <b>WRITE EXCLUSIVE-OR EXECUTE, ENFORCED BY A STATE FIELD RATHER THAN BY DISCIPLINE.</b> The
/// mapping is created readable and writable and NOT executable; <see cref="Arm"/> makes it readable
/// and executable and NOT writable; there is no method that produces a mapping which is both, no
/// argument that would ask for one, and no constant in this tree that names one. A page that is
/// writable and executable at once is the single most valuable thing an attacker can find in a
/// process, and the value of it does not depend on whether anybody meant to leave it there.
/// </para>
/// <para>
/// <b>THE TWO PROTECTION VALUES THAT WOULD MAKE A PAGE WRITABLE AND EXECUTABLE APPEAR IN NO SOURCE
/// FILE THIS COMPONENT COMPILES</b> - not in this file, not in either platform half, not in a
/// comment as an example of what not to write <i>(narrowed 2026-09-08: this read "not in this file,
/// not in a test, not in a comment", and the clause about tests was wrong - the rule's own
/// recogniser is a test source and holds both values, as the paragraph below now records)</i>. A
/// constant that exists somewhere is a constant that can be reached by an edit nobody reviews
/// closely; one that has never been typed cannot be.
/// </para>
/// <para>
/// <b>They appear in two places, neither of them an arming path</b> <i>(revised 2026-09-07: it
/// read "APPEAR NOWHERE IN THIS REPOSITORY", which was true when it was written and stopped being
/// true the moment the rule that enforces it acquired a negative control; revised again 2026-09-08,
/// because it then read "They appear in exactly one place", and the rule's own recogniser holds the
/// values too)</i>. Rule X1 refuses a read-write-execute protection reaching an arming call, and a
/// rule nobody has watched fail is a rule in name only - so the value stands in a stored witness
/// input that no compilation includes, purely so the refusal can be observed. The second place is
/// that rule's implementation, <c>src/tests/Broiler.VM.Architecture.Tests/NativeMappingRules.cs</c>,
/// which declares the write-and-execute protections as the set it recognises, because a rule cannot
/// reject a value it does not hold. <b>The distinction is the whole of it: a value on an arming
/// path is one edit from being passed to an operating system, and a value in a witness or in the
/// recogniser that reads it is the evidence that passing it would be caught.</b>
/// </para>
/// <para>
/// <b>The platform split is two partial halves and a run-time test, because rule J6 forbids a
/// preprocessor directive in a covered source file.</b> Both halves are compiled into every image,
/// which is honest rather than awkward: what a composition declares is which architectures it may
/// arm, not which system libraries its image happens to name. A declaration is not a call, and
/// neither half's imports are resolved until the half that matches the running system is used.
/// </para>
/// <para>
/// <b>Arming must complete before a verified handle is minted, and that follows from a contract
/// this profile did not write.</b> A verified state's contract requires that everything reachable
/// from it be immutable once verification returns and safe for unsynchronised concurrent readers,
/// because a shareable handle is read by several runtimes at once with no lock between them. An
/// armed mapping satisfies that; a writable one does not.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=7457A9
// Broiler-Falsified-If: any mapping this type creates is executable while it is writable, or an entry pointer is handed out for a mapping that is not armed
// Broiler-Human:        PENDING
internal sealed unsafe partial class JsNativePage : System.IDisposable
{
    /// <summary>The base of the mapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=82E226
    // Broiler-Falsified-If: this holds an address this type did not map, or it is read after the mapping was released
    // Broiler-Human:        PENDING
    private byte* address;

    /// <summary>How many bytes the mapping covers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=8F7E00
    // Broiler-Falsified-If: this differs from the number of bytes the mapping was actually made with
    // Broiler-Human:        PENDING
    private readonly nuint length;

    /// <summary>Which of the three things this mapping currently is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=EF3CDD
    // Broiler-Falsified-If: this says Armed for a mapping the operating system still admits a write to, or Writable for one it admits an execute from
    // Broiler-Human:        PENDING
    private JsNativePageState state;

    /// <summary>Maps <paramref name="bytes"/> bytes, readable and writable and not executable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=29EF7E
    // Broiler-Falsified-If: a mapping is created with any execute permission
    // Broiler-Human:        PENDING
    private JsNativePage(byte* mapped, nuint bytes)
    {
        address = mapped;
        length = bytes;
        state = JsNativePageState.Writable;
    }

    /// <summary>What this mapping currently is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=77BB7D
    // Broiler-Falsified-If: this reports a state the mapping is not actually in
    // Broiler-Human:        PENDING
    internal JsNativePageState State => state;

    /// <summary>How many bytes this mapping covers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9C9616
    // Broiler-Human:        PENDING
    internal nuint Length => length;

    /// <summary>
    /// Maps a writable, non-executable region for <paramref name="code"/> and copies it in.
    /// </summary>
    /// <remarks>
    /// <b>THE BYTES ARE COPIED IN BY THE FACTORY AND THERE IS NO PUBLIC WRITE.</b> A type with an
    /// <c>Allocate</c> and a separate <c>Write</c> has a window in which a caller can be handed a
    /// writable mapping and forget to arm it, or arm it and then be handed a way to write to it
    /// again. One method that maps, copies and returns a mapping still to be armed is the smallest
    /// surface that does the job.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=E8DB67
    // Broiler-Falsified-If: a caller can write to a mapping this method returned after Arm has run
    // Broiler-Human:        PENDING
    internal static JsNativePage? TryMap(System.ReadOnlySpan<byte> code)
    {
        if (code.Length == 0)
        {
            return null;
        }

        var bytes = (nuint)code.Length;

        var mapped = System.OperatingSystem.IsWindows()
            ? MapWindows(bytes)
            : MapUnix(bytes);

        if (mapped is null)
        {
            return null;
        }

        var page = new JsNativePage(mapped, bytes);
        code.CopyTo(new System.Span<byte>(mapped, code.Length));
        return page;
    }

    /// <summary>Makes the mapping readable and executable, and no longer writable.</summary>
    /// <remarks>
    /// <b>The instruction cache is flushed on the platform that has an export for it and not on the
    /// one that does not, and the difference is not an oversight.</b> x86-64 keeps its instruction
    /// cache coherent with stores in hardware, so no maintenance sequence is architecturally
    /// required on either system; the Windows call is made anyway because the platform documents it
    /// as the supported way to publish freshly written code and because a documented call costs
    /// nothing once per artifact. THIS REASONING IS ABOUT x86-64 AND DOES NOT CARRY TO arm64, where
    /// the maintenance sequence is architecturally required and has no managed expression - which
    /// is exactly why no arm64 payload is executed by anything in this repository.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=AE60F1
    // Broiler-Falsified-If: this leaves the mapping writable, or it succeeds without removing write permission
    // Broiler-Human:        PENDING
    internal bool Arm()
    {
        if (state != JsNativePageState.Writable)
        {
            throw new System.InvalidOperationException(
                "a mapping can be armed only once, and only while it is writable");
        }

        var armed = System.OperatingSystem.IsWindows()
            ? ArmWindows(address, length)
            : ArmUnix(address, length);

        if (!armed)
        {
            return false;
        }

        state = JsNativePageState.Armed;
        return true;
    }

    /// <summary>The entry point at <paramref name="offset"/>, which only an armed mapping has.</summary>
    /// <remarks>
    /// <para>
    /// <b>IT IS A FUNCTION POINTER AND NOT A DELEGATE, AND THE CALL IT PRODUCES IS A <c>calli</c>
    /// WITH A REAL TRANSITION.</b> A delegate would mean a marshalling stub and a reference this
    /// profile has no reason to keep alive; a function pointer is the address and the signature and
    /// nothing else.
    /// </para>
    /// <para>
    /// <b>THERE IS DELIBERATELY NO <c>SuppressGCTransition</c> ON THIS SIGNATURE, AND THAT IS A
    /// SAFETY DECISION RATHER THAN AN OMISSION.</b> Suppressing the transition keeps the calling
    /// thread in cooperative mode for the whole call, which means a long-running emitted function
    /// blocks every collection for its duration and a collection attempted during it waits for a
    /// thread that will not reach a safe point. That is a deadlock whose likelihood rises with how
    /// useful the emitted code is. Taking the transition costs a few nanoseconds per call and is
    /// what makes an emitted frame preemptible, which is what makes it safe to run for a long time.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=41FDB2
    // Broiler-Falsified-If: an entry pointer is produced for a mapping that is not armed
    // Broiler-Human:        PENDING
    internal delegate* unmanaged<JsNativeFrame*, int> Entry(uint offset)
    {
        if (state != JsNativePageState.Armed)
        {
            throw new System.InvalidOperationException(
                "an entry point exists only for an armed mapping");
        }

        if (offset >= length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(offset));
        }

        return (delegate* unmanaged<JsNativeFrame*, int>)(address + offset);
    }

    /// <summary>The address at <paramref name="offset"/>, which only an armed mapping has.</summary>
    /// <remarks>
    /// <b>It is an address and not an entry point, and the two are separated because one of them
    /// is a promise about a signature.</b> A trampoline is handed the address of the code it will
    /// call and calls it under a signature the trampoline itself decided; asking for an entry point
    /// here would be this file making a claim about a signature it has no way to check.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=FEC06D
    // Broiler-Falsified-If: an address is produced for a mapping that is not armed
    // Broiler-Human:        PENDING
    internal nint At(uint offset)
    {
        if (state != JsNativePageState.Armed)
        {
            throw new System.InvalidOperationException(
                "an address inside a mapping is handed out only once it is armed");
        }

        if (offset >= length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(offset));
        }

        return (nint)(address + offset);
    }

    /// <summary>Releases the mapping.</summary>
    /// <remarks>
    /// <b>It is idempotent and it nulls the address, because a double release of a mapping is a
    /// release of whatever the system put there next.</b> That is a defect whose symptom appears in
    /// an unrelated part of the process, which is the shape of defect this whole file is written to
    /// avoid.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=A80C3A
    // Broiler-Falsified-If: a second call releases a mapping a second time
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (state == JsNativePageState.Released || address is null)
        {
            return;
        }

        state = JsNativePageState.Released;
        var released = address;
        address = null;

        if (System.OperatingSystem.IsWindows())
        {
            ReleaseWindows(released);
            return;
        }

        ReleaseUnix(released, length);
    }
}
