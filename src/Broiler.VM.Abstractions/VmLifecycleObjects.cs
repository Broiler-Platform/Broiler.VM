namespace Broiler.VM;

/// <summary>The seven states of an instance.</summary>
/// <remarks>
/// One <see cref="Suspended"/> state for all three suspension origins, deliberately unlike the
/// operation, which has two. An instance does not care who parked its operation; the operation
/// does, because who parked it decides who may resume it.
/// </remarks>
public enum VmInstanceState
{
    /// <summary>Being instantiated.</summary>
    Instantiating = 0,

    /// <summary>Live and idle.</summary>
    Live = 1,

    /// <summary>Executing an operation.</summary>
    Executing = 2,

    /// <summary>Holding a suspended operation.</summary>
    Suspended = 3,

    /// <summary>Faulted; accepts only disposal.</summary>
    Faulted = 4,

    /// <summary>Disposing.</summary>
    Disposing = 5,

    /// <summary>Terminal.</summary>
    Disposed = 6,
}

/// <summary>The seven states of an operation.</summary>
/// <remarks>
/// <see cref="SuspendedByGuest"/> and <see cref="SuspendedByHost"/> are separate states rather than
/// one state with a reason field, because their legal successors differ: a guest suspension resumes
/// on the profile's own terms and rides the caller's result, while a host suspension is delivered
/// once through the control handle and can be abandoned.
/// </remarks>
public enum VmOperationState
{
    /// <summary>Executing.</summary>
    Running = 0,

    /// <summary>Parked by the guest, or by an asynchronous instantiation.</summary>
    SuspendedByGuest = 1,

    /// <summary>Parked by the host through a control handle.</summary>
    SuspendedByHost = 2,

    /// <summary>Core-owned: finishing, after the profile returned and before the result is published.</summary>
    Completing = 3,

    /// <summary>Terminal.</summary>
    Completed = 4,

    /// <summary>Core-owned: abandoned, its continuation dropped or unwound.</summary>
    Orphaned = 5,

    /// <summary>Terminal.</summary>
    Disposed = 6,
}

/// <summary>The three kinds of operation.</summary>
/// <remarks>
/// Resume is deliberately not a fourth kind. At resume the operation that suspended continues,
/// keeping its identity, its budget remainder and its nested-load counters - a fourth kind would
/// imply a new operation and therefore a fresh allowance.
/// </remarks>
public enum VmOperationKind
{
    /// <summary>A verification.</summary>
    Verify = 0,

    /// <summary>An instantiation.</summary>
    Instantiate = 1,

    /// <summary>An invocation.</summary>
    Invoke = 2,
}

/// <summary>Who caused an operation to park.</summary>
public enum VmSuspensionOrigin
{
    /// <summary>The guest suspended on its own terms.</summary>
    Guest = 0,

    /// <summary>The host requested it through a control handle.</summary>
    External = 1,

    /// <summary>An asynchronous instantiation parked.</summary>
    Instantiation = 2,
}

/// <summary>
/// The single-use resumption object for one suspended operation.
/// </summary>
/// <remarks>
/// <para>
/// Bound to exactly one runtime, one instance and one operation, and minted only when an operation
/// has actually parked - never speculatively. It exposes identity, state, origin and the profile's
/// opaque projection and nothing else: it has no state table of its own, and it is valid only while
/// the operation it addresses is suspended.
/// </para>
/// <para>
/// There is one resume entry point on the runtime and no second path. Two entry points would mean
/// two admission checks and a race between them, which is why <c>RequestResume</c> on the control
/// handle was struck in favour of taking the object and resuming through the runtime.
/// </para>
/// </remarks>
public sealed class VmSuspension
{
    private readonly IVmProfilePayload? projection;
    private int consumed;

    /// <summary>The single construction site for a suspension object.</summary>
    /// <remarks>Hidden, and asserted by an architecture rule to have one call site in the runtime.</remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static VmSuspension Create(
        VmObjectId objectId,
        VmObjectId operationId,
        VmObjectId runtimeId,
        VmSuspensionOrigin origin,
        VmStage suspendedStage,
        IVmProfilePayload? projection) =>
        new(objectId, operationId, runtimeId, origin, suspendedStage, projection);

    private VmSuspension(
        VmObjectId objectId,
        VmObjectId operationId,
        VmObjectId runtimeId,
        VmSuspensionOrigin origin,
        VmStage suspendedStage,
        IVmProfilePayload? projection)
    {
        ObjectId = objectId;
        OperationId = operationId;
        RuntimeId = runtimeId;
        Origin = origin;
        SuspendedStage = suspendedStage;
        this.projection = projection;
    }

    /// <summary>This object's identity.</summary>
    public VmObjectId ObjectId { get; }

    /// <summary>The operation it resumes.</summary>
    public VmObjectId OperationId { get; }

    /// <summary>The runtime it belongs to. Presented elsewhere it is a foreign handle.</summary>
    public VmObjectId RuntimeId { get; }

    /// <summary>Who parked the operation.</summary>
    public VmSuspensionOrigin Origin { get; }

    /// <summary>Which stage suspended, so a resume result can carry it.</summary>
    public VmStage SuspendedStage { get; }

    /// <summary>Whether this object has already been used to resume.</summary>
    public bool IsConsumed => System.Threading.Volatile.Read(ref consumed) != 0;

    /// <summary>The profile's opaque projection of what it exposes while parked, where it offered one.</summary>
    public bool TryGetProjection(out IVmProfilePayload payload)
    {
        payload = projection!;
        return projection is not null;
    }

    /// <summary>
    /// Marks the object used. It is single-use: the second attempt to resume from one suspension
    /// is refused rather than admitted, which is what makes double-resume a contract error instead
    /// of a race.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public bool TryConsume() => System.Threading.Interlocked.Exchange(ref consumed, 1) == 0;
}

/// <summary>A non-blocking, non-mutating view of an operation's current state.</summary>
/// <remarks>
/// The two latches are exposed because they are observable facts a test must be able to key on
/// through the public surface - rule A10 leaves no internal route - and because "the latch is
/// monotonic and is never cleared" is a contract property rather than an implementation detail.
/// </remarks>
public readonly struct VmOperationStateSnapshot
{
    /// <summary>Creates a snapshot.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public VmOperationStateSnapshot(
        VmObjectId operationId,
        VmOperationKind kind,
        VmOperationState state,
        VmSuspensionOrigin origin,
        bool hasSuspensionOrigin,
        bool cancellationRequested,
        bool externalSuspendRequested)
    {
        OperationId = operationId;
        Kind = kind;
        State = state;
        Origin = origin;
        HasSuspensionOrigin = hasSuspensionOrigin;
        CancellationRequested = cancellationRequested;
        ExternalSuspendRequested = externalSuspendRequested;
    }

    /// <summary>The operation.</summary>
    public VmObjectId OperationId { get; }

    /// <summary>What kind of operation it is.</summary>
    public VmOperationKind Kind { get; }

    /// <summary>Its current state.</summary>
    public VmOperationState State { get; }

    /// <summary>Who parked it, meaningful only when <see cref="HasSuspensionOrigin"/>.</summary>
    public VmSuspensionOrigin Origin { get; }

    /// <summary>Whether the operation is parked at all.</summary>
    public bool HasSuspensionOrigin { get; }

    /// <summary>Whether cancellation has been requested. Monotonic; never cleared.</summary>
    public bool CancellationRequested { get; }

    /// <summary>Whether an external suspension has been requested. Monotonic; never cleared.</summary>
    public bool ExternalSuspendRequested { get; }
}

/// <summary>
/// The control surface for one in-flight operation, handed to the caller that started it.
/// </summary>
/// <remarks>
/// <para>
/// Exactly four members. Authority is possession, not identity: the core authenticates nobody and
/// defines no principal, permission, claim, policy or "diagnostic client". Whoever holds the handle
/// may control the operation, and the caller that received it alone decides who else gets it.
/// </para>
/// <para>
/// Disposing a handle that still holds an untaken external suspension latches the operation
/// cancelled, so a debugger that pauses an operation and then goes away cannot park it forever. A
/// handle that is merely dropped is not observable to the core, and that case is bounded by the
/// runtime's maximum suspended residency instead - deliberately, because a finalizer closing the
/// hole would make disposal timing depend on the garbage collector.
/// </para>
/// </remarks>
public abstract class VmOperationControlHandle : System.IDisposable
{
    /// <summary>For the runtime's implementation.</summary>
    protected VmOperationControlHandle()
    {
    }

    /// <summary>
    /// Asks the operation to park at its next polling point. Answers unsupported where the profile
    /// did not declare external suspension or the runtime did not enable it - the double gate.
    /// </summary>
    public abstract VmControlResult RequestSuspend();

    /// <summary>Asks the operation to cancel at its next polling point. Monotonic.</summary>
    public abstract VmControlResult RequestCancel();

    /// <summary>Reads the operation's state without blocking or mutating anything.</summary>
    public abstract VmOperationStateSnapshot QueryState();

    /// <summary>
    /// Takes the resumption object for an externally suspended operation, once.
    /// </summary>
    /// <remarks>
    /// This exists so that the party entitled to resume has a path to resume in every origin case
    /// without a second admission check. A guest suspension delivers its object on the caller's
    /// result; an external one has no such result to ride, so it is taken from here.
    /// </remarks>
    public abstract VmControlResult TryTakeSuspension(out VmSuspension suspension);

    /// <summary>Releases the handle, latching an untaken external suspension as cancelled.</summary>
    public abstract VmControlResult Dispose();

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Dispose();
}

/// <summary>
/// Profile-owned mutable state instantiated from one verified artifact, owned by its runtime.
/// </summary>
/// <remarks>
/// Declared here so a stage result can name it, and implemented in <c>Broiler.VM.Runtime</c>, which
/// is the only assembly permitted to know the lifecycle implementation. An instance is published
/// only when instantiation completes normally: there is no half-instantiated instance a caller can
/// obtain and then wonder about.
/// </remarks>
public abstract class VmInstance : System.IDisposable
{
    /// <summary>For the runtime's implementation.</summary>
    protected VmInstance()
    {
    }

    /// <summary>This instance's identity.</summary>
    public abstract VmObjectId ObjectId { get; }

    /// <summary>The profile that owns it.</summary>
    public abstract VmProfileId ProfileId { get; }

    /// <summary>Its current state.</summary>
    public abstract VmInstanceState State { get; }

    /// <summary>Invokes an entry point.</summary>
    public abstract VmInvocationResult Invoke(
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken);

    /// <summary>Invokes an entry point, also returning the control handle for the operation.</summary>
    public abstract VmInvocationResult Invoke(
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken,
        out VmOperationControlHandle controlHandle);

    /// <summary>Requests cancellation of whatever this instance is running.</summary>
    public abstract VmControlResult RequestCancel();

    /// <summary>Disposes the instance. Idempotent.</summary>
    public abstract VmControlResult Dispose();

    /// <inheritdoc/>
    void System.IDisposable.Dispose() => Dispose();
}
