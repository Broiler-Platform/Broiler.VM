using Broiler.VM;

namespace Com.Example.Calculator;

/// <summary>The calculator profile's answer: the number left on the stack.</summary>
/// <remarks>
/// The core never names this type, never calls a member on it, and inspects nothing about it except
/// the identity every payload carries. A consumer gets the concrete type back through this profile's
/// own static accessor, which is the projection shape the contract specifies.
/// </remarks>
public sealed class CalculatorAnswer : IVmProfilePayload
{
    internal CalculatorAnswer(VmProfileId profileId, long value)
    {
        Identity = new VmPayloadIdentity(profileId, CalculatorProfile.AnswerKindId, 1);
        Value = value;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The number the calculator finished with.</summary>
    public long Value { get; }
}

/// <summary>The calculator profile's language-defined fault.</summary>
/// <remarks>
/// Division by zero is a fact about arithmetic, not about the core. It rides behind the
/// profile-neutral fault category as a typed payload, so it reaches a caller in full without the
/// core acquiring a case for it.
/// </remarks>
public sealed class CalculatorFault : IVmProfilePayload
{
    internal CalculatorFault(VmProfileId profileId, string description)
    {
        Identity = new VmPayloadIdentity(profileId, CalculatorProfile.FaultKindId, 1);
        Description = description;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>What this profile calls the fault.</summary>
    public string Description { get; }
}

/// <summary>The calculator profile's mutable per-instance state.</summary>
public sealed class CalculatorInstance : IVmInstanceState
{
    internal CalculatorInstance(CalculatorProgram program) => Program = program;

    internal CalculatorProgram Program { get; }

    /// <summary>How many times this instance has been invoked.</summary>
    public int InvocationCount { get; internal set; }
}

/// <summary>
/// The calculator profile's per-runtime executor.
/// </summary>
/// <remarks>
/// <para>
/// One per runtime, created by the factory the descriptor names directly, so the type is rooted for
/// trimming and Native AOT by an ordinary reference rather than by a linker annotation.
/// </para>
/// <para>
/// It charges fuel per token and polls at the profile's declared granularity. Nothing here reaches
/// the core beyond the four metering members and the capability table, because nothing else is
/// reachable from an execution environment.
/// </para>
/// </remarks>
public sealed class CalculatorExecutor : IVmProfileExecutor
{
    private readonly IVmExecutionEnvironment environment;

    internal CalculatorExecutor(VmProfileId profileId, IVmExecutionEnvironment executionEnvironment)
    {
        ProfileId = profileId;
        environment = executionEnvironment;
    }

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!artifact.TryGetState(out var state) || state is not CalculatorProgram program)
        {
            // A handle this profile did not produce, or one that has been disposed. Either way it
            // is not something to run, and saying so is a contract violation rather than a fault:
            // no guest program caused it.
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        return VmExecutionStep.Instantiated(new CalculatorInstance(program), null);
    }

    /// <inheritdoc/>
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken)
    {
        if (state is not CalculatorInstance instance)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        // One entry point, named "evaluate". A name this profile does not know is a language-level
        // fault rather than a core one: what an entry point means is the profile's business, and
        // the core carried the bytes without decoding them.
        if (!System.MemoryExtensions.SequenceEqual(request.EntryPoint.Utf8, "evaluate"u8))
        {
            return VmExecutionStep.Faulted(new CalculatorFault(ProfileId, "no such entry point"));
        }

        instance.InvocationCount++;
        return Evaluate(instance.Program, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This profile never suspends, so a resume can only be the core handing back a continuation it
    /// was never given. Answering with a contract violation rather than pretending to resume is what
    /// keeps a defect in the core or in this profile from being reported as a completed operation.
    /// </remarks>
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);

    /// <inheritdoc/>
    /// <remarks>Nothing to unwind: this profile parks nowhere and holds no resource across a step.</remarks>
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }

    private VmExecutionStep Evaluate(
        CalculatorProgram program,
        System.Threading.CancellationToken cancellationToken)
    {
        // Sized from the depth VERIFICATION computed and stored, never from a number the payload
        // chose. That is the difference between a bound and a hope.
        var stack = new long[program.MaximumDepth];
        var top = 0;
        var offset = 0;

        while (offset < program.Tokens.Length)
        {
            if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
            {
                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
            }

            // One poll per token, which is well inside the uncharged-work bound this profile's
            // descriptor declares. A profile that declared a bound and did not poll to it would be
            // promising a cancellation latency it does not keep.
            if (!environment.Meter.Poll())
            {
                return VmExecutionStep.ContractViolation(VmReason.Cancelled);
            }

            var token = program.Tokens[offset];
            offset++;

            switch (token)
            {
                case CalculatorFormat.TokenPush:
                    stack[top] = program.Operands[program.Tokens[offset]];
                    top++;
                    offset++;
                    continue;

                case CalculatorFormat.TokenAdd:
                    stack[top - 2] = unchecked(stack[top - 2] + stack[top - 1]);
                    top--;
                    continue;

                case CalculatorFormat.TokenMultiply:
                    stack[top - 2] = unchecked(stack[top - 2] * stack[top - 1]);
                    top--;
                    continue;

                case CalculatorFormat.TokenDivide:
                    if (stack[top - 1] == 0)
                    {
                        return VmExecutionStep.Faulted(new CalculatorFault(ProfileId, "division by zero"));
                    }

                    // The one case .NET's own division throws on rather than answering. A language
                    // fault is this profile's to define, so it is defined here rather than allowed
                    // to escape as an exception the core would have to translate.
                    if (stack[top - 2] == long.MinValue && stack[top - 1] == -1)
                    {
                        return VmExecutionStep.Faulted(new CalculatorFault(ProfileId, "quotient is out of range"));
                    }

                    stack[top - 2] = stack[top - 2] / stack[top - 1];
                    top--;
                    continue;

                case CalculatorFormat.TokenNegate:
                    stack[top - 1] = unchecked(-stack[top - 1]);
                    continue;

                default:
                    return VmExecutionStep.Completed(new CalculatorAnswer(ProfileId, stack[top - 1]));
            }
        }

        // Unreachable for a verified program: validation refuses one that does not end in a halt.
        return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
    }
}
