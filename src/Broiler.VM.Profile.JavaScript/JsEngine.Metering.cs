// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           1
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  5/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;

using Broiler.ECMAScript;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// Ends the operation when the runtime's stack probe refuses, naming the dimension it refused
    /// on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The refusal is real and it was reported as nothing.</b> A call whose stack probe says no
    /// has nowhere left to build an error object, so it ends the operation rather than throwing a
    /// <c>RangeError</c> — that half is <see cref="MaximumCallDepth"/>'s remarks and is right. What
    /// it did NOT do was say what ran out: the abort became
    /// <c>ProfileFault</c>/<c>AllowanceExhausted</c>, which names no dimension and no scope, and
    /// release gate 4 asks that a call-stack overflow be <b>reported as a resource exhaustion
    /// naming a dimension</b> and not be fatal. Half of that held — nothing terminated — and the
    /// naming half did not.
    /// </para>
    /// <para>
    /// <b>The meter is how a profile names a dimension, because a profile may not mint a core
    /// outcome.</b> Charging a quantity of <c>CallDepth</c> no level can admit is the truthful
    /// statement of what happened — this interpreter cannot take another frame — and it is the
    /// only statement of it the contract has. The core's own precedence then reports
    /// <c>ResourceExhaustion</c>/<c>CeilingReached</c> carrying <c>CallDepth</c> and the scope that
    /// refused, ahead of whatever step this abort produces. Nothing is committed by a refused
    /// charge, so the meter is left exactly as it was.
    /// </para>
    /// <para>
    /// <b>What it costs a reader is worth stating.</b> The dimension named is the one the guest was
    /// spending and not the machine resource that ran out, so an operator who raises
    /// <c>CallDepth</c> after meeting this gets no further — the stack, not the ceiling, is what
    /// refused. That is the same trade the counted bound makes in the other direction, and the
    /// alternative was an answer that named nothing at all.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=94E6AA
    // Broiler-Falsified-If: the stack backstop produces a result naming no dimension, or a refused charge commits anything
    // Broiler-Human:        PENDING
    private JsAbort StackBackstopReached()
    {
        // The whole range, so that no ceiling a host could grant admits it and the outermost level
        // that refuses is the one reported.
        _ = meter.TryCharge(VmBudgetDimension.CallDepth, ulong.MaxValue);

        return new JsAbort(JsAbortKind.Exhausted, "the call-depth backstop was reached");
    }

    /// <summary>
    /// Charges for work whose size is a number of characters, never less than one unit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The comparison and conversion families charged a flat amount, and a flat charge over an
    /// input the guest chooses is a budget that bounds nothing.</b> Roadmap section 8 names
    /// <i>string concatenation and comparison</i> and <i>numeric conversion of large values</i>
    /// among the families whose cost grows with their input, and asks each to declare a monotone
    /// non-decreasing charging function. Concatenation had one and comparison did not: comparing
    /// two strings of any length cost sixteen units, measured, and so did reading a number out of a
    /// string of any length.
    /// </para>
    /// <para>
    /// <b>The unit is a character and the declared function is the work's own bound.</b> An ordinal
    /// comparison stops at the first difference and therefore cannot read past the shorter operand,
    /// so its function is <c>min(|a|, |b|) + 1</c>; reading a number out of a string trims and scans
    /// the whole of it, so its function is <c>|s| + 1</c>. Both are monotone non-decreasing in the
    /// magnitude of the input, and the declared granularity is one, so the charge is the function
    /// itself rather than a ceiling over a window.
    /// </para>
    /// <para>
    /// The <c>+ 1</c> is not rounding: a comparison of two empty strings is still a comparison, and
    /// a family whose charge can be zero is a family a program can perform without limit.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=626201
    // Broiler-Falsified-If: a text operation's charge does not grow with the input the guest controls
    // Broiler-Human:        PENDING
    internal void ChargeText(int units) => Charge(units <= 0 ? 1UL : (ulong)units + 1UL);

    /// <summary>
    /// This engine's <see cref="Charge"/> as the shared libraries' meter (decision SJL-D-10 (a)), made
    /// once per engine so that a conversion allocates no delegate.
    /// </summary>
    /// <remarks>
    /// A shared parsing operation charges the text's length, before reading it, when the text is not
    /// empty; an adapter that charged <see cref="ChargeText"/> before charges the one unit beyond it
    /// itself, so every total, and every exhaustion's point, is what it was (JSD-0065, decision 2).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BD45AA
    // Broiler-Falsified-If: a shared conversion charges through it a total, or exhausts at a point, other than this engine's own charge for the same text did before SJL-3.4
    // Broiler-Human:        PENDING
    internal WorkCharge WorkCharge => workCharge ??= Charge;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6CED30
    // Broiler-Human:        PENDING
    private WorkCharge? workCharge;

    /// <summary>Charges one crossing of the host surface: a host call, and its fuel.</summary>
    /// <remarks>
    /// <para>
    /// <b>It charges <c>HostCalls</c> as well as <c>Fuel</c>, and that is what makes the host
    /// surface cost what the capability channel costs.</b> A crossing does not go through
    /// <c>IVmHostCapabilityInvoker</c> - a host object is an ordinary object in this realm and
    /// calling its method is an ordinary call - so the boundary charge
    /// <c>VmCapabilityBinding.TryEnter</c> applies is not applied for us. Left uncharged, an
    /// embedder could move an unbounded amount of work across the seam and pay for none of it,
    /// which would make a host-call ceiling a ceiling on the wrong thing.
    /// </para>
    /// <para>
    /// The fuel is proportional to what the crossing carries - an argument count, a text length -
    /// because a flat charge over a guest-controlled quantity is a charge a guest can dilute to
    /// nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=62A22B
    // Broiler-Falsified-If: a crossing of the host surface completes without charging HostCalls
    // Broiler-Human:        PENDING
    internal void ChargeHostCrossing(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
        }

        Charge(units);
    }

    /// <summary>Charges fuel, aborting when the allowance is spent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4AFB70
    // Broiler-Human:        PENDING
    internal void Charge(ulong units)
    {
        while (units > PollWindow)
        {
            ChargeOnce(PollWindow);
            units -= PollWindow;
        }

        ChargeOnce(units);
    }

    /// <summary>Charges the debt value-form emitted code ran up in pure instructions, or nothing for none.</summary>
    /// <remarks>
    /// <b>It is <see cref="Charge"/>, in the poll window's steps</b>, so a debt is charged with the existing
    /// member only and never asks for fuel nothing has spent, and a debt that crosses a poll window polls
    /// cancellation and the wall clock where the interpreter would have (JSD-0035 section 7).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=6EA938
    // Broiler-Falsified-If: a debt is charged other than through Charge, or a zero debt charges or polls anything
    // Broiler-Human:        PENDING
    internal void ChargeDebt(long debt)
    {
        if (debt < 0)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "value-form emitted code handed back a negative debt");
        }

        if (debt > 0)
        {
            Charge((ulong)debt);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8CD7C8
    // Broiler-Human:        PENDING
    private void ChargeOnce(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.Fuel, units))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the instruction allowance is spent");
        }

        sinceLastPoll += units;

        if (sinceLastPoll < PollWindow)
        {
            return;
        }

        sinceLastPoll = 0;

        if (cancellation.IsCancellationRequested)
        {
            throw new JsAbort(JsAbortKind.Cancelled, "cancellation was requested");
        }

        if (!meter.Poll())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "a budget dimension was reached");
        }
    }

    /// <summary>Reports bytes an allocation retained, so LiveBytes stays a ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2C6F27
    // Broiler-Human:        PENDING
    internal void Retain(ulong bytes) => meter.ReportRetained(VmBudgetDimension.LiveBytes, bytes);

    /// <summary>
    /// Retains bytes only if every level admits them, aborting BEFORE the allocation otherwise.
    /// </summary>
    /// <remarks>
    /// <b><see cref="Retain"/> reports after the fact, and a host that is handed a value cannot be
    /// told one operation later that it should not have been.</b> A retention report returns
    /// nothing and its refusal is latched for the next charge or poll, which is right for a guest
    /// allocation whose result the same operation is about to fail anyway. An embedder building a
    /// buffer through the host surface would already hold the value by then, so this gates on
    /// <c>TryCharge</c> - which is the meter's own statement of how a caller that must observe a
    /// ceiling refusal does it - and the refusal is decided before a byte is allocated.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=13932D
    // Broiler-Falsified-If: a retention the LiveBytes ceiling refuses returns normally or leaves the allocation to happen
    // Broiler-Human:        PENDING
    internal void RetainOrAbort(ulong bytes)
    {
        if (!meter.TryCharge(VmBudgetDimension.LiveBytes, bytes))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the live-bytes ceiling does not admit the allocation");
        }
    }

    // ---- throwing ------------------------------------------------------------------------------

    /// <summary>Throws a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D6FB0C
    // Broiler-Human:        PENDING
    internal JsValue ThrowTypeError(string message) => throw Error("TypeError", message);

    /// <summary>Throws a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=92484D
    // Broiler-Human:        PENDING
    internal JsValue ThrowRangeError(string message) => throw Error("RangeError", message);

    /// <summary>Throws a <c>ReferenceError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=97EC2F
    // Broiler-Human:        PENDING
    internal JsValue ThrowReferenceError(string message) => throw Error("ReferenceError", message);

    /// <summary>Throws a <c>SyntaxError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=173F3B
    // Broiler-Human:        PENDING
    internal JsValue ThrowSyntaxError(string message) => throw Error("SyntaxError", message);

    /// <summary>Builds a throw carrying a fresh Error of the named intrinsic kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=66C1DC
    // Broiler-Human:        PENDING
    internal JsThrow Error(string kind, string message)
    {
        var error = Realm.CreateError(kind, message);
        return new JsThrow(error, kind + ": " + message);
    }
}
