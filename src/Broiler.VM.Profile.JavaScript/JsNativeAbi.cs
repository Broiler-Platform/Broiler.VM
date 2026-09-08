// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Critical
// Criteria:         3/3
// Resource impact:  5/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What one run of a trampoline over an emitted entry point observed.</summary>
/// <param name="Ran">Whether the mapping was made and the call happened at all.</param>
/// <param name="StackBefore">The stack pointer immediately before the call.</param>
/// <param name="StackAfter">The stack pointer immediately after it returned.</param>
/// <param name="Answer">What the emitted entry answered.</param>
/// <param name="Saved">
/// RBX, RBP, RSI, RDI, R12, R13, R14 and R15 as they were when the call returned, in that order.
/// </param>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AB829C
// Broiler-Falsified-If: a field here reports something the trampoline did not record
// Broiler-Human:        PENDING
public readonly record struct JsNativeAbiObservation(
    bool Ran, long StackBefore, long StackAfter, long Answer, long[] Saved);

/// <summary>
/// Runs a caller-supplied trampoline over a caller-supplied entry point and reports what the
/// calling convention actually did.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS MAPS BYTES AND EXECUTES THEM AND IT VERIFIES NOTHING ABOUT THEM, AND THAT IS SAID FIRST
/// BECAUSE IT IS THE MOST IMPORTANT THING ABOUT THIS TYPE.</b> Nothing in the product calls it; no
/// execution path reaches it; the executor maps only the emitted code of a VERIFIED handle and does
/// so through its own path. This exists for one reason, and the reason is that a calling-convention
/// obligation cannot be TESTED without a way to run code that violates it. An obligation asserted
/// only over conforming code is an obligation nobody has shown can fail, and the whole lesson of
/// the <c>ret 8</c> fixture the core roadmap retains is that a check nobody proved could fail is a
/// check nobody should trust.
/// </para>
/// <para>
/// <b>The trampoline is the caller's and not this assembly's, and that keeps a second encoder out
/// of this component.</b> The obligations being checked are the emitter's, the emitter has an
/// encoder, and a hand-written byte array here would be a second statement of the same encodings
/// that could disagree with the first. So the caller builds the trampoline with the same encoder
/// the backend uses, and this method maps it beside the target and calls it.
/// </para>
/// <para>
/// <b>The two are mapped as ONE blob because a call between them is a rel32 the caller computed.</b>
/// The trampoline reaches the target through the probe structure rather than through a
/// displacement, so their addresses need not be related - but mapping once means one arming, one
/// release, and one thing to get wrong.
/// </para>
/// <para>
/// <b>Write exclusive-or execute holds here exactly as it does on the execution path</b>, because
/// this goes through the same mapping type and there is no other. The blob is written while the
/// mapping is writable and not executable, armed once, and released.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=2AA164
// Broiler-Falsified-If: any product execution path calls this, or a mapping it makes is writable and executable at once
// Broiler-Human:        PENDING
public static unsafe class JsNativeAbi
{
    /// <summary>
    /// Maps <paramref name="blob"/>, arms it, and calls its trampoline over its target
    /// <paramref name="repetitions"/> times, reporting what the last call observed.
    /// </summary>
    /// <remarks>
    /// <b>REPEATING THE CALL IS THE HALF OF THE CHECK THAT CATCHES A SMALL LEAK.</b> A callee that
    /// moves the stack pointer by eight bytes per call passes a single-call comparison whenever the
    /// comparison is made against a value the same defect already shifted; it cannot pass a million
    /// calls, because a million times eight bytes is more stack than any thread has. So the two
    /// halves are complementary: the comparison names the defect and the repetition forces a defect
    /// too small to see in one call to arrive while a test is watching.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=AAE7EE
    // Broiler-Falsified-If: this reports a stack pointer the trampoline did not record, or it returns without releasing the mapping
    // Broiler-Human:        PENDING
    public static JsNativeAbiObservation Run(
        System.ReadOnlySpan<byte> blob,
        uint trampolineOffset,
        uint targetOffset,
        int operandSlots,
        int bindingSlots,
        double[] constants,
        long fuel,
        int repetitions)
    {
        var page = JsNativePage.TryMap(blob);

        if (page is null)
        {
            return new JsNativeAbiObservation(false, 0, 0, 0, []);
        }

        using (page)
        {
            if (!page.Arm())
            {
                return new JsNativeAbiObservation(false, 0, 0, 0, []);
            }

            var operands = System.GC.AllocateArray<double>(
                System.Math.Max(1, operandSlots), pinned: true);

            var bindings = System.GC.AllocateArray<double>(
                System.Math.Max(1, bindingSlots), pinned: true);

            var pool = System.GC.AllocateArray<double>(
                System.Math.Max(1, constants.Length), pinned: true);

            var counter = System.GC.AllocateArray<long>(1, pinned: true);
            System.Array.Copy(constants, pool, constants.Length);

            var uninitialised = JsNativeValues.FromBits(JsNativeValues.UninitialisedBits);

            for (var index = 0; index < bindings.Length; index++)
            {
                bindings[index] = uninitialised;
            }

            var probe = default(JsNativeAbiProbe);
            var saved = new long[8];

            fixed (double* operandBase = operands)
            fixed (double* bindingBase = bindings)
            fixed (double* constantBase = pool)
            fixed (long* fuelBase = counter)
            {
                var frame = new JsNativeFrame
                {
                    Operands = operandBase,
                    OperandCount = operands.Length,
                    Locals = bindingBase,
                    LocalCount = bindings.Length,
                    Constants = constantBase,
                    Fuel = fuelBase,
                    BailoutPc = 0,
                };

                var trampoline = page.Entry(trampolineOffset);
                probe.Target = (void*)page.At(targetOffset);
                probe.Frame = &frame;

                for (var attempt = 0; attempt < repetitions; attempt++)
                {
                    // THE FUEL IS RESET PER CALL so that a repetition is a repetition of the same
                    // work rather than a run that gets shorter as the counter drains. A test whose
                    // later iterations do less than its earlier ones is a test whose figure means
                    // nothing.
                    counter[0] = fuel;

                    // The frame is handed to the trampoline through the probe and the trampoline is
                    // handed the probe, so the signature this profile calls through is the probe's
                    // and not the emitted unit's - which is what lets the trampoline stand between
                    // the two and watch.
                    ((delegate* unmanaged<JsNativeAbiProbe*, int>)trampoline)(&probe);
                }

                for (var index = 0; index < saved.Length; index++)
                {
                    saved[index] = probe.Saved[index];
                }
            }

            return new JsNativeAbiObservation(
                true, probe.StackBefore, probe.StackAfter, probe.Answer, saved);
        }
    }
}
