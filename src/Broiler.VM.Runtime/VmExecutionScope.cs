// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           14
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Medium
// Criteria:         3/0
// Resource impact:  2/10 max
// Unverified:       17
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

    // The answer the last lookup gave, together with the execution context it was read under. Read
    // and written without a lock: every value ever stored is a true pair, so a thread that loses a
    // race only misses the cache. One field rather than one per thread, and that is where the cost
    // can go the wrong way: two operations of ONE scope charging on two threads at once - two
    // instances of one profile in one runtime - displace each other's pair, so every lookup misses,
    // allocates and writes a shared field. The answer stays right and the speed does not. The
    // fallback, if a concurrent measurement ever finds that case slower than no cache at all, is
    // three thread-static fields (scope, context, meter) that Leave clears for the leaving thread.
    private volatile Resolution? resolution;

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
    /// A thread whose flow is suppressed captures null, and that lookup is answered but not cached -
    /// deliberately, and that is the load-bearing half of it: null is a key every suppressed flow
    /// shares, so a pair published under it would be handed straight to any other thread that looks
    /// up with its own flow suppressed, including one that is inside no step and must be refused.
    /// </para>
    /// <para>
    /// What is cached is at most one context object and one meter per scope at a time. <see
    /// cref="Leave"/> releases the pair, and the next lookup takes one again - including a lookup
    /// from a thread the step left running, which republishes the finished step's own context and
    /// meter, and through them that operation's levels, until a later lookup displaces them or the
    /// runtime is disposed and drops the scope. So the pin is bounded to one pair and is never
    /// per-thread, but it is not over when the step is. Nothing is stored on a thread: an
    /// <c>AsyncLocal</c> entry, a value-changed handler or a thread-static would each be the VM-5
    /// leak class, which a disposed runtime must leave nothing of behind.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=785C21
    // Broiler-Falsified-If: a lookup returns a meter other than the one the thread's AsyncLocal holds
    // Broiler-Human:        PENDING
    internal VmMeter? Current
    {
        get
        {
            var context = System.Threading.ExecutionContext.Capture();
            var cached = resolution;

            if (cached is not null && ReferenceEquals(cached.Context, context))
            {
                return cached.Meter;
            }

            var meter = current.Value;

            if (context is not null)
            {
                resolution = new Resolution(context, meter);
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

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=AB35B4
    // Broiler-Falsified-If: a step's own thread returns from here with the scope still holding the context that step ran under
    // Broiler-Human:        PENDING
    internal void Leave()
    {
        current.Value = null;
        operation.Value = null;

        // Releases the finished step's context at once. Not needed for the answer: the two writes
        // above have already put this thread under a new context object, so no stale hit is
        // possible on it, and a thread the step started keeps the meter it captured either way.
        // A release and not a seal: a thread the step left running that looks up after this puts
        // the finished step's pair back, and it stays until the next lookup or until the runtime
        // drops the scope.
        resolution = null;
    }

    /// <summary>One lookup's answer, and the execution context it is the answer for.</summary>
    /// <remarks>
    /// One object rather than two fields, so a reader sees a context and a meter that were read
    /// together. It is immutable and published by the volatile write of the field that holds it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=32D733
    // Broiler-Falsified-If: a pair is published whose meter is not what the AsyncLocal held under its context
    // Broiler-Human:        PENDING
    private sealed class Resolution
    {
        internal Resolution(System.Threading.ExecutionContext context, VmMeter? meter)
        {
            Context = context;
            Meter = meter;
        }

        internal System.Threading.ExecutionContext Context { get; }

        internal VmMeter? Meter { get; }
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
