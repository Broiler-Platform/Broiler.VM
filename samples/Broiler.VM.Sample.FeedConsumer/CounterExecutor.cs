using Broiler.VM;

namespace Broiler.VM.Sample.FeedConsumer;

/// <summary>
/// The profile's executor: one per runtime, holding the environment the core handed it.
/// </summary>
/// <remarks>
/// <para>
/// It validates nothing, and that is not an omission. Every state it is given came out of
/// <see cref="CounterVerifier"/>, and the core's contract is that nothing else can reach here - so
/// a check in this file would be a second verifier, which is exactly what the one-verifier property
/// exists to prevent. Two verifiers that must agree are a security defect with a schedule.
/// </para>
/// <para>
/// The environment is held; the METER is not. A meter belongs to an operation and the executor
/// outlives every operation it runs, so capturing one here would charge every later invocation
/// against whichever operation happened to create the executor. The environment's meter resolves
/// to the operation currently running, which is why it is read on each use below rather than
/// stored.
/// </para>
/// </remarks>
internal sealed class CounterExecutor : IVmProfileExecutor
{
    private readonly IVmExecutionEnvironment environment;

    internal CounterExecutor(IVmExecutionEnvironment environment) => this.environment = environment;

    /// <inheritdoc/>
    public VmProfileId ProfileId => CounterProfile.Id;

    /// <inheritdoc/>
    /// <remarks>
    /// This profile's instance state is its verified state: the decoded artifact is immutable and
    /// an instance of it adds nothing. A profile whose instances held mutable memory would build
    /// that memory here, and would be charged for it here.
    /// </remarks>
    public VmExecutionStep Instantiate(VmVerifiedArtifact artifact, CancellationToken cancellationToken)
    {
        if (!artifact.TryGetState(out var verified) || verified is not CounterState state)
        {
            // The core hands back only what this profile's own verifier produced, so this cannot
            // happen - and it is answered rather than asserted, because a profile that throws here
            // turns a core invariant into a crash in the host's process.
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        return VmExecutionStep.Instantiated(state, new CounterValue(state.Start));
    }

    /// <inheritdoc/>
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (state is not CounterState counter)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        if (!request.EntryPoint.Utf8.SequenceEqual("count"u8))
        {
            // An entry point this profile does not have. A profile fault rather than a core
            // failure: the core has no opinion about what entry points a profile offers.
            return VmExecutionStep.Faulted(new CounterValue(0));
        }

        var meter = environment.Meter;
        var value = counter.Start;

        for (var step = 0u; step < counter.Steps; step++)
        {
            // A unit of Fuel per step, charged BEFORE the work rather than after. Charging
            // afterwards means the last step is free, which for a profile whose steps are cheap is
            // a rounding error and for a profile whose steps are expensive is the whole bound.
            if (!meter.TryCharge(VmBudgetDimension.Fuel, 1))
            {
                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
            }

            // Polling is how a long-running profile stays cancellable, and the descriptor's
            // cancellation poll bound is the promise this loop is keeping. A profile that charged
            // and never polled would be refused by the core the moment it exceeded that bound,
            // which is what stops "cancellable" being a claim nobody checks.
            if (!meter.Poll())
            {
                return VmExecutionStep.ContractViolation(VmReason.Cancelled);
            }

            value++;
        }

        return VmExecutionStep.Completed(new CounterValue(value));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This profile never suspends - it declares no external suspension and its loop yields
    /// nothing - so there is no continuation it could ever be handed. The refusal is what makes
    /// that structural rather than merely true today.
    /// </remarks>
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);

    /// <inheritdoc/>
    /// <remarks>
    /// Nothing to unwind, for the same reason. A profile that held native memory across a
    /// suspension would release it here, inside the allowance it is given and without blocking.
    /// </remarks>
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }
}

/// <summary>
/// How a consumer gets a typed value back out of a profile-neutral result.
/// </summary>
/// <remarks>
/// A static accessor shipped by the profile is the shape the contract specifies, and the identity
/// check before the cast is the point of it: checking only the CLR type would accept a payload
/// minted by a different profile that happened to use the same class.
/// </remarks>
internal static class CounterResults
{
    internal static bool TryGetValue(in VmInvocationResult result, out long value)
    {
        value = 0;

        if (!result.IsSuccess || !Owns(result.PayloadIdentity))
        {
            return false;
        }

        if (!result.TryGetPayload<CounterValue>(out var payload))
        {
            return false;
        }

        value = payload.Value;
        return true;
    }

    private static bool Owns(VmPayloadIdentity identity) =>
        identity.ProfileId.Equals(CounterProfile.Id) &&
        identity.PayloadKindId == CounterProfile.ValueKindId;
}
