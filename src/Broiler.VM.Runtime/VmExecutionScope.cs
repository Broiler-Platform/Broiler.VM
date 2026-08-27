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
internal sealed class VmExecutionScope
{
    private readonly System.Threading.AsyncLocal<VmMeter?> current = new();
    private readonly System.Threading.AsyncLocal<VmOperation?> operation = new();

    internal VmMeter? Current => current.Value;

    /// <summary>
    /// The operation the current step belongs to, so a host failure a capability produced can be
    /// latched onto the thing that will report it.
    /// </summary>
    internal VmOperation? CurrentOperation => operation.Value;

    internal void Enter(VmMeter meter, VmOperation? owner = null)
    {
        current.Value = meter;
        operation.Value = owner;
    }

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
internal sealed class VmAmbientMeter : IVmMeter
{
    private readonly VmExecutionScope scope;

    internal VmAmbientMeter(VmExecutionScope scope) => this.scope = scope;

    /// <inheritdoc/>
    public bool TryCharge(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.TryCharge(dimension, amount) ?? false;

    /// <inheritdoc/>
    public bool Poll() => scope.Current?.Poll() ?? false;

    /// <inheritdoc/>
    public void ReportRetained(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.ReportRetained(dimension, amount);

    /// <inheritdoc/>
    public void ReportReleased(VmBudgetDimension dimension, ulong amount) =>
        scope.Current?.ReportReleased(dimension, amount);
}

/// <summary>The capability table a profile executor holds, resolving through the current scope.</summary>
/// <remarks>
/// The binding table itself is immutable and fixed at runtime creation; only the meter a host call
/// is charged against changes per operation. Outside a step there is nothing to charge, so a call
/// made there is unavailable rather than silently free.
/// </remarks>
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
    public int BindingCount => bindings.Length;

    /// <inheritdoc/>
    public bool IsBound(int bindingIndex) =>
        bindingIndex >= 0 && bindingIndex < bindings.Length && bindings[bindingIndex].IsBound;

    /// <inheritdoc/>
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
