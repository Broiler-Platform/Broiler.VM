// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           13
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  2/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The bridge between a per-runtime executor and the per-operation meter it must charge.
/// </summary>
/// <remarks>
/// <para>
/// A profile's executor is created once per runtime and handed one execution environment, but
/// budgets, cancellation and diagnostics all belong to the <em>operation</em>, and a runtime may run
/// many. Capturing the meter that happened to exist when the executor was created would charge every
/// later invocation against the instantiation's allowance and would hand it the instantiation's
/// cancellation token - so an invocation cancelled by its caller would never notice.
/// </para>
/// <para>
/// This scope is what the environment's meter and capability table resolve through. The runtime
/// opens it immediately before calling into the executor and closes it immediately after, so the
/// executor always sees the operation it is actually running, and sees nothing at all outside a
/// step. <c>AsyncLocal</c> rather than a field, because two instances of one profile in one runtime
/// may execute on two threads at once.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=51852E
// Broiler-Human:        PENDING
internal sealed class VmExecutionScope
{
    private readonly System.Threading.AsyncLocal<VmMeter?> current = new();
    private readonly System.Threading.AsyncLocal<VmOperation?> operation = new();

    // The answer the last lookup ON THIS THREAD gave, with the scope and the execution context it was
    // the answer for. Three thread-static fields rather than one field on the scope, because a field
    // on the scope is displaced by every other thread charging through the same scope: two instances
    // of one profile in one runtime, running at once, made every lookup miss, allocate a pair and
    // write a shared field, and the concurrent measurement this change was made for found that case
    // slower at every thread count of two or more than holding no answer at all. Per thread, a miss
    // costs the AsyncLocal read and nothing besides, and no thread can displace another's answer.
    //
    // The scope is one of the three because the fields are the thread's rather than the scope's, and
    // two scopes may be entered on one thread.
    //
    // This is not the leak class VM-5 found. That was an AsyncLocal entry per runtime that nothing
    // released, so the map every later write copies grew without bound. These three hold exactly one
    // triple however many runtimes have run on the thread: the next lookup overwrites it and Leave
    // clears it, so nothing accumulates and a disposed runtime leaves nothing of itself behind. What
    // this variant does hold, until the thread's next lookup or its Leave, is one context object on a
    // thread a step left running - one per thread, which is what makes it the fallback and not the
    // first choice.
    [System.ThreadStatic]
    private static VmExecutionScope? resolvedScope;

    [System.ThreadStatic]
    private static System.Threading.ExecutionContext? resolvedContext;

    [System.ThreadStatic]
    private static VmMeter? resolvedMeter;

    /// <summary>The meter of the step this thread is running inside, or null outside a step.</summary>
    /// <remarks>
    /// <para>
    /// The answer is the <c>AsyncLocal</c>'s, and only the reading is different. An
    /// <c>ExecutionContext</c> is immutable: setting any <c>AsyncLocal</c> builds a new context
    /// object and installs it on the thread, and the value map inside it is copy-on-write. So a
    /// context object determines every <c>AsyncLocal</c> value, and while a thread runs under the
    /// context object an earlier lookup was made under - on whichever thread that context has
    /// flowed to - the <c>AsyncLocal</c> still holds exactly what that lookup read.
    /// </para>
    /// <para>
    /// Two cases fall back to the <c>AsyncLocal</c> and are answered as before. A thread with no
    /// context of its own captures <c>ExecutionContext.Default</c>, which every such thread shares;
    /// that is a consistent pair with the null meter such a thread reads, and no pair of
    /// <c>Default</c> with a meter can ever be written, because entering a step installs a context.
    /// A thread whose flow is suppressed captures null, and that lookup is answered but not held -
    /// deliberately, and that is the load-bearing half of it: null is a key every suppressed lookup
    /// shares, so a triple stored under it would be handed back to this thread's next suppressed
    /// lookup whatever it is now inside, including one inside no step, which must be refused.
    /// </para>
    /// <para>
    /// What is held is at most one triple - scope, context and meter - per thread. <see
    /// cref="Leave"/> clears the leaving thread's, and the next lookup on that thread takes one
    /// again - including a lookup from a thread the step left running, which puts the finished
    /// step's own context and meter back, and through them that operation's levels, until a later
    /// lookup on that same thread displaces them. So the pin is bounded to one triple per thread,
    /// and it is not over when the step is. It is not the leak class VM-5 found, which was an
    /// <c>AsyncLocal</c> entry per runtime that nothing released and so grew the map every later
    /// write copies: nothing here grows with the number of runtimes a thread has run, and a
    /// disposed runtime leaves nothing of itself behind.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=BCD550
    // Broiler-Falsified-If: a lookup returns a meter other than the one the thread's AsyncLocal holds
    // Broiler-Human:        PENDING
    internal VmMeter? Current
    {
        get
        {
            var context = System.Threading.ExecutionContext.Capture();

            if (context is not null &&
                ReferenceEquals(resolvedScope, this) &&
                ReferenceEquals(resolvedContext, context))
            {
                return resolvedMeter;
            }

            var meter = current.Value;

            if (context is not null)
            {
                resolvedScope = this;
                resolvedContext = context;
                resolvedMeter = meter;
            }

            return meter;
        }
    }

    /// <summary>
    /// The operation the current step belongs to, so a host failure a capability produced can be
    /// latched onto the thing that will report it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=BD09F0
    // Broiler-Human:        PENDING
    internal VmOperation? CurrentOperation => operation.Value;

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=F0C37E
    // Broiler-Human:        PENDING
    internal void Enter(VmMeter meter, VmOperation? owner = null)
    {
        current.Value = meter;
        operation.Value = owner;
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=884E92
    // Broiler-Falsified-If: a step's own thread returns from here still holding the context that step ran under
    // Broiler-Human:        PENDING
    internal void Leave()
    {
        current.Value = null;
        operation.Value = null;

        // Releases the finished step's context on the thread that is leaving. Not needed for the
        // answer: the two writes above have already put this thread under a new context object, so no
        // stale hit is possible on it, and a thread the step started keeps the meter it captured
        // either way. It is the leaving thread's triple and no other: a thread the step left running
        // still holds the one its own last lookup wrote, until its next lookup displaces it.
        resolvedScope = null;
        resolvedContext = null;
        resolvedMeter = null;
    }

}

/// <summary>
/// The meter a profile executor holds: a stable object that resolves to whichever operation is
/// currently running.
/// </summary>
/// <remarks>
/// Outside a step there is no operation to charge, so every method refuses. A profile that stashed
/// its meter and used it later is therefore refused rather than charged against an unrelated
/// operation - which would be worse than either failing or succeeding, because the bill would land
/// somewhere nobody was looking.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=593128
// Broiler-Human:        PENDING
internal sealed class VmAmbientMeter : IVmMeter
{
    private readonly VmExecutionScope scope;

    internal VmAmbientMeter(VmExecutionScope scope) => this.scope = scope;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=88CA2E
    // Broiler-Human:        PENDING
    public bool TryCharge(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.TryCharge(dimension, amount) ?? false;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=630A13
    // Broiler-Human:        PENDING
    public bool Poll() => scope.Current?.Poll() ?? false;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=ADA564
    // Broiler-Human:        PENDING
    public void ReportRetained(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.ReportRetained(dimension, amount);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=B47DE7
    // Broiler-Human:        PENDING
    public void ReportReleased(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.ReportReleased(dimension, amount);
}

/// <summary>The capability table a profile executor holds, resolving through the current scope.</summary>
/// <remarks>
/// The binding table itself is immutable and fixed at runtime creation; only the meter a host call
/// is charged against changes per operation. Outside a step there is nothing to charge, so a call
/// made there is unavailable rather than silently free.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=78B1C2
// Broiler-Human:        PENDING
internal sealed class VmAmbientCapabilityInvoker : IVmHostCapabilityInvoker
{
    private readonly VmCapabilityBinding[] bindings;
    private readonly VmRuntime owner;
    private readonly VmExecutionScope scope;

    internal VmAmbientCapabilityInvoker(VmCapabilityBinding[] bindings, VmRuntime owner, VmExecutionScope scope)
    {
        this.bindings = bindings;
        this.owner = owner;
        this.scope = scope;
    }

    /// <summary>The reason the most recent failing call produced.</summary>
    internal VmReason LastFailure { get; private set; } = VmReason.None;

    /// <summary>The capability the most recent failing call named.</summary>
    internal VmCapabilityId LastFailureCapability { get; private set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=08A7E3
    // Broiler-Human:        PENDING
    public int BindingCount => bindings.Length;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Low; Resources=0; Fingerprint=B14302
    // Broiler-Human:        PENDING
    public bool IsBound(int bindingIndex) =>
        bindingIndex >= 0 && bindingIndex < bindings.Length && bindings[bindingIndex].IsBound;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=2; Fingerprint=C3C1A1
    // Broiler-Human:        PENDING
    public VmHostCallOutcome Invoke(int bindingIndex, System.ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;
        var meter = scope.Current;

        if (meter is null)
        {
            LastFailure = VmReason.MediatorOutOfScope;
            return VmHostCallOutcome.Unavailable;
        }

        var invoker = new VmCapabilityInvoker(bindings, owner, meter);
        var outcome = invoker.Invoke(bindingIndex, arguments, out result);

        Latch(invoker, bindingIndex);
        return outcome;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=2; Fingerprint=DF28EB
    // Broiler-Human:        PENDING
    public VmHostCallOutcome InvokeBytes(int bindingIndex, VmBytes argument, out VmOpaqueRef result)
    {
        result = default;
        var meter = scope.Current;

        if (meter is null)
        {
            LastFailure = VmReason.MediatorOutOfScope;
            return VmHostCallOutcome.Unavailable;
        }

        var invoker = new VmCapabilityInvoker(bindings, owner, meter);
        var outcome = invoker.InvokeBytes(bindingIndex, argument, out result);

        Latch(invoker, bindingIndex);
        return outcome;
    }

    /// <summary>
    /// Records an unconverted host failure on the operation that will report it.
    /// </summary>
    /// <remarks>
    /// Only where the capability declared that a fault terminates the operation. Where it declared
    /// an observable fault, the profile is handed the refusal and is expected to convert it, and
    /// whatever it produces is the answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=1615F4
    // Broiler-Human:        PENDING
    private void Latch(VmCapabilityInvoker invoker, int bindingIndex)
    {
        LastFailure = invoker.LastFailure;
        LastFailureCapability = invoker.LastFailureCapability;

        if (invoker.LastFailure is VmReason.None || !invoker.TerminatesOperation)
        {
            return;
        }

        var version = bindingIndex >= 0 && bindingIndex < bindings.Length
            ? bindings[bindingIndex].Import.Descriptor.Version
            : 0;

        scope.CurrentOperation?.LatchHostFailure(invoker.LastFailure, invoker.LastFailureCapability, version);
    }
}
