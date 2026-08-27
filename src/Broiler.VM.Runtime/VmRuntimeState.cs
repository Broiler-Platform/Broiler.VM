namespace Broiler.VM;

/// <summary>The four states of a runtime.</summary>
/// <remarks>
/// There is no suspended state: only operations suspend. <see cref="Poisoned"/> is the terminal
/// faulted state a core defect moves a runtime to - a runtime that broke the metering contract must
/// accept only disposal, because every later answer it gave would be computed from state the core
/// no longer trusts.
/// </remarks>
public enum VmRuntimeState
{
    /// <summary>Usable.</summary>
    Ready = 0,

    /// <summary>Terminally faulted by a core defect; accepts only disposal.</summary>
    Poisoned = 1,

    /// <summary>Disposing, draining in-flight work.</summary>
    Disposing = 2,

    /// <summary>Terminal.</summary>
    Disposed = 3,
}

/// <summary>S1: the runtime-creation result.</summary>
/// <remarks>
/// Runtime creation is an envelope-bearing stage rather than a throwing operation, because a
/// runtime cannot be created once a shared parent has no remaining allowance - and that is only
/// expressible if creation can return a resource exhaustion. Two error mechanisms at one surface
/// guarantee that one of them is untested.
/// </remarks>
public readonly struct VmRuntimeCreationResult : IVmOperationResult
{
    private readonly VmRuntime? runtime;

    private VmRuntimeCreationResult(VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics, VmRuntime? runtime)
    {
        Outcome = outcome;
        Reason = reason;
        Diagnostics = diagnostics;
        this.runtime = runtime;
    }

    internal static VmRuntimeCreationResult Normal(VmRuntime runtime, VmDiagnostics diagnostics) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, diagnostics, runtime);

    internal static VmRuntimeCreationResult InvalidState(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.InvalidState, reason, diagnostics, null);

    internal static VmRuntimeCreationResult ResourceExhaustion(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.ResourceExhaustion, reason, diagnostics, null);

    internal static VmRuntimeCreationResult HostFailure(VmReason reason, VmDiagnostics diagnostics) =>
        new(VmOutcome.HostFailure, reason, diagnostics, null);

    /// <inheritdoc/>
    public VmOutcome Outcome { get; }

    /// <inheritdoc/>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public VmDiagnostics Diagnostics { get; }

    /// <inheritdoc/>
    public bool IsSuccess => Outcome is VmOutcome.Normal;

    /// <inheritdoc/>
    public bool IsSuspended => false;

    /// <summary>The runtime, available only on success.</summary>
    public bool TryGetRuntime(out VmRuntime created)
    {
        created = runtime!;
        return Outcome is VmOutcome.Normal && runtime is not null;
    }
}
