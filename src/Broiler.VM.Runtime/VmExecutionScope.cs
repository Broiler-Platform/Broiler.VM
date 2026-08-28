// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           10
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=033A8F
    // Broiler-Human:        PENDING
    internal VmMeter? Current => current.Value;

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

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=58FA1F
    // Broiler-Human:        PENDING
    internal void Leave()
    {
        current.Value = null;
        operation.Value = null;
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
