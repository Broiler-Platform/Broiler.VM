namespace Broiler.VM.Fixtures;

/// <summary>
/// The fixture profile's per-runtime executor: a stack machine with a real dispatch loop.
/// </summary>
/// <remarks>
/// <para>
/// It charges fuel per instruction at its declared granularity, polls at its declared bound,
/// suspends by capturing an instruction pointer and a stack, requests guest-initiated loads through
/// the mediator it was handed, calls host capabilities by binding index, and unwinds a continuation
/// when it is abandoned. Every one of those is a core obligation the fixture exists to exercise.
/// </para>
/// <para>
/// The deliberately non-conforming variants are here rather than in a separate mock, because a
/// refusal path proven against a mock proves that the mock is refusable.
/// </para>
/// </remarks>
public sealed class FixtureVmExecutor : IVmProfileExecutor
{
    private readonly IVmExecutionEnvironment environment;
    private readonly FixtureVmProfileVariant variant;
    private readonly uint chargingGranularity;
    private readonly FixtureExecutionGate? gate;

    internal FixtureVmExecutor(
        VmProfileId profileId,
        IVmExecutionEnvironment environment,
        FixtureVmProfileVariant variant,
        uint chargingGranularity,
        FixtureExecutionGate? gate = null)
    {
        ProfileId = profileId;
        this.environment = environment;
        this.variant = variant;
        this.chargingGranularity = chargingGranularity;
        this.gate = gate;
    }

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <summary>How many continuations this executor has been asked to unwind.</summary>
    public int UnwoundCount { get; private set; }

    /// <summary>How many guest-initiated loads this executor has requested.</summary>
    public int RequestedLoadCount { get; private set; }

    /// <summary>The outcome of the most recent guest-initiated load, for a test to inspect.</summary>
    public VmOutcome LastGuestLoadOutcome { get; private set; }

    /// <summary>The reason of the most recent guest-initiated load.</summary>
    public VmReason LastGuestLoadReason { get; private set; }

    /// <inheritdoc/>
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken)
    {
        gate?.Reached(FixtureGatePoint.Instantiate);

        if (variant is FixtureVmProfileVariant.SuspendsDuringInstantiation)
        {
            // Parking here is legal only where the descriptor declares asynchronous instantiation.
            // The non-declaring variant exists so that the refusal is demonstrable.
            return VmExecutionStep.Suspended(new FixtureContinuation(0, new long[1], 0), null);
        }

        if (!artifact.TryGetState(out var state) || state is not FixtureVerifiedState verified)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        return VmExecutionStep.Instantiated(new FixtureInstanceState(verified), null);
    }

    /// <inheritdoc/>
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken)
    {
        gate?.Reached(FixtureGatePoint.Invoke);

        if (state is not FixtureInstanceState fixtureState)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        fixtureState.InvocationCount++;

        return Run(fixtureState, 0, new long[32], 0, cancellationToken);
    }

    /// <inheritdoc/>
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken)
    {
        if (state is not FixtureInstanceState fixtureState || continuation is not FixtureContinuation parked)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        return Run(fixtureState, parked.InstructionPointer, parked.Stack, parked.StackDepth, cancellationToken);
    }

    /// <inheritdoc/>
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
        gate?.Reached(FixtureGatePoint.Unwind);

        if (variant is FixtureVmProfileVariant.NoUnwindEntryPoint)
        {
            // A profile that declares no unwinding leaves this empty; the core drops the
            // continuation deterministically rather than waiting for it.
            return;
        }

        UnwoundCount++;

        if (continuation is FixtureContinuation parked)
        {
            parked.Unwound = true;
        }
    }

    private VmExecutionStep Run(
        FixtureInstanceState state,
        int instructionPointer,
        long[] stack,
        int stackDepth,
        System.Threading.CancellationToken cancellationToken)
    {
        var code = state.Verified.Code;
        var constants = state.Verified.Constants;
        var meter = environment.Meter;
        var sinceLastPoll = 0u;

        while (instructionPointer < code.Length)
        {
            if (variant is not FixtureVmProfileVariant.NonCharging &&
                !meter.TryCharge(VmBudgetDimension.Fuel, chargingGranularity))
            {
                // The profile learns its limit by reaching it and being refused. There is no
                // remaining-fuel reader it could have branched on instead.
                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
            }

            sinceLastPoll += chargingGranularity;

            if (variant is not FixtureVmProfileVariant.PollBoundBreaker && !meter.Poll())
            {
                return VmExecutionStep.ContractViolation(VmReason.Cancelled);
            }

            if (variant is not FixtureVmProfileVariant.PollBoundBreaker)
            {
                sinceLastPoll = 0;
            }

            var opcode = code[instructionPointer];
            instructionPointer++;

            switch (opcode)
            {
                case FixtureFormat.OpNop:
                    break;

                case FixtureFormat.OpPushConst:
                    stack[stackDepth++] = constants[code[instructionPointer++]];
                    break;

                case FixtureFormat.OpAdd:
                    stack[stackDepth - 2] = unchecked(stack[stackDepth - 2] + stack[stackDepth - 1]);
                    stackDepth--;
                    break;

                case FixtureFormat.OpSub:
                    stack[stackDepth - 2] = unchecked(stack[stackDepth - 2] - stack[stackDepth - 1]);
                    stackDepth--;
                    break;

                case FixtureFormat.OpMul:
                    stack[stackDepth - 2] = unchecked(stack[stackDepth - 2] * stack[stackDepth - 1]);
                    stackDepth--;
                    break;

                case FixtureFormat.OpHostCall:
                {
                    var binding = code[instructionPointer++];
                    var argument = stackDepth > 0 ? stack[--stackDepth] : 0;
                    System.Span<long> arguments = stackalloc long[1];
                    arguments[0] = argument;

                    var outcome = environment.Capabilities.Invoke(binding, arguments, out var answer);

                    if (outcome is not VmHostCallOutcome.Completed)
                    {
                        return VmExecutionStep.Faulted(
                            new FixtureFault(ProfileId, (long)outcome, "host call did not complete"));
                    }

                    stack[stackDepth++] = answer;
                    break;
                }

                case FixtureFormat.OpYield:
                    return VmExecutionStep.Suspended(
                        new FixtureContinuation(instructionPointer, stack, stackDepth),
                        new FixtureSuspensionProjection(ProfileId, instructionPointer, stackDepth));

                case FixtureFormat.OpFault:
                    return VmExecutionStep.Faulted(
                        new FixtureFault(ProfileId, constants[code[instructionPointer]], "fixture fault"));

                case FixtureFormat.OpLoad:
                {
                    var specifierIndex = code[instructionPointer++];
                    var step = RequestLoad(constants[specifierIndex], cancellationToken);

                    if (step is not null)
                    {
                        return step.Value;
                    }

                    break;
                }

                case FixtureFormat.OpSpin:
                {
                    var units = (ulong)constants[code[instructionPointer++]];

                    if (variant is not FixtureVmProfileVariant.NonCharging &&
                        !meter.TryCharge(VmBudgetDimension.Fuel, units))
                    {
                        return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
                    }

                    break;
                }

                case FixtureFormat.OpAllocate:
                {
                    var elements = (ulong)constants[code[instructionPointer++]];

                    if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, elements * sizeof(long)))
                    {
                        return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
                    }

                    break;
                }

                case FixtureFormat.OpRetain:
                {
                    var bytes = (ulong)constants[code[instructionPointer++]];
                    meter.ReportRetained(VmBudgetDimension.LiveBytes, bytes);
                    state.RetainedBytes += bytes;
                    break;
                }

                case FixtureFormat.OpRelease:
                {
                    var bytes = (ulong)constants[code[instructionPointer++]];
                    meter.ReportReleased(VmBudgetDimension.LiveBytes, bytes);
                    state.RetainedBytes = bytes >= state.RetainedBytes ? 0 : state.RetainedBytes - bytes;
                    break;
                }

                case FixtureFormat.OpReturn:
                    return VmExecutionStep.Completed(
                        new FixtureValue(ProfileId, stackDepth > 0 ? stack[stackDepth - 1] : 0));

                default:
                    return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
            }
        }

        return VmExecutionStep.Completed(
            new FixtureValue(ProfileId, stackDepth > 0 ? stack[stackDepth - 1] : 0));
    }

    /// <summary>
    /// Requests one guest-initiated load, and converts its result into this profile's own terms.
    /// </summary>
    /// <remarks>
    /// A terminal nested outcome must be converted rather than swallowed: a nested cancellation or
    /// exhaustion ends the requesting operation too, because the budget it exhausted was the
    /// requesting operation's own. The misconverting variant exists so that the core's enforcement
    /// of that is demonstrable.
    /// </remarks>
    private VmExecutionStep? RequestLoad(long specifier, System.Threading.CancellationToken cancellationToken)
    {
        RequestedLoadCount++;

        if (!environment.TryGetArtifactLoadMediator(out var mediator))
        {
            // A non-declaring profile is handed nothing at all, so this branch is unreachable for
            // one - which is the point: an undeclared request is structurally unrepresentable.
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        System.Span<byte> specifierBytes = stackalloc byte[8];
        System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(specifierBytes, specifier);

        var request = new VmArtifactRequest(
            ProfileId,
            default,
            default,
            1,
            default,
            cancellationToken,
            new VmBytes(specifierBytes));

        var result = mediator.RequestLoad(in request);

        LastGuestLoadOutcome = result.Outcome;
        LastGuestLoadReason = result.Reason;

        if (result.IsSuccess)
        {
            return null;
        }

        if (variant is FixtureVmProfileVariant.MisconvertingNestedOutcome)
        {
            // Deliberately swallowing a terminal nested outcome. The core notices, because the
            // operation it charged is the one that is out of budget.
            return null;
        }

        return result.Outcome switch
        {
            VmOutcome.Cancellation => VmExecutionStep.ContractViolation(VmReason.Cancelled),
            VmOutcome.ResourceExhaustion => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
            _ => VmExecutionStep.Faulted(
                new FixtureFault(ProfileId, (long)result.Reason, "guest-initiated load failed")),
        };
    }
}
