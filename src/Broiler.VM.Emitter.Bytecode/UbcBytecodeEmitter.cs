// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           20
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  2/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Ubc;

namespace Broiler.VM.Emitter.Bytecode;

/// <summary>
/// The bytecode emitter: the form a composition names to execute verified universal bytecode as it
/// stands.
/// </summary>
/// <remarks>
/// <para>
/// Its emitting half is the identity - it emits nothing, and an artifact of this form carries no
/// Emission section, which the walk refuses if one is present. Its executing half is the interpreter,
/// one dispatch loop generic over the family, reached through <see cref="Form"/>'s executor factory.
/// </para>
/// <para>
/// A composition hands <see cref="Form"/> to <see cref="UbcEmitterSet.Create"/>; nothing else in this
/// assembly is public, because nothing else is a composition's to name.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=2FD37A
// Broiler-Human:        PENDING
public static class UbcBytecodeEmitter
{
    /// <summary>The emitter's semantic version: it moves when the interpreter executes any row differently.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4AD700
    // Broiler-Human:        PENDING
    public const int SemanticVersion = 1;

    /// <summary>The bytecode form: the identity <see cref="UbcFormat.BytecodeForm"/> and the interpreter as its executor.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2B5582
    // Broiler-Human:        PENDING
    public static UbcForm Form { get; } = new(UbcFormat.BytecodeForm, SemanticVersion, new UbcBytecodeExecutorFactory());
}

/// <summary>Makes the interpreter's executor for one family.</summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=C91D05
// Broiler-Human:        PENDING
internal sealed class UbcBytecodeExecutorFactory : IUbcExecutorFactory
{
    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CBC2CA
    // Broiler-Falsified-If: the executor made here runs a family other than the type argument, or one registration's executor serves another's programs
    // Broiler-Human:        PENDING
    public IVmProfileExecutor Create<TFamily>(
        UbcFamilyRegistration<TFamily> family,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment)
        where TFamily : struct, IUbcFamily =>
        new UbcExecutor<TFamily>(family, declaration, environment);
}

/// <summary>One instance: the verified program and the family's state for it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1863EA
// Broiler-Human:        PENDING
internal sealed class UbcInstance : IVmInstanceState
{
    internal UbcInstance(UbcVerifiedProgram program, object familyState, object owner)
    {
        Program = program;
        FamilyState = familyState;
        Owner = owner;
    }

    internal UbcVerifiedProgram Program { get; }

    internal object FamilyState { get; }

    /// <summary>The executor that made the instance: another executor's instance is refused.</summary>
    internal object Owner { get; }
}

/// <summary>
/// A suspended operation: every frame, the word plane below the suspending row's arguments, the value
/// plane below them as the family's own record, and where to continue.
/// </summary>
/// <remarks>
/// Captured through the family's frame codec rather than by keeping the operation's planes, so that
/// what a resumption restores is exactly what the codec wrote and a codec that loses a value is found
/// by the first round trip that needs it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=74C552
// Broiler-Falsified-If: a continuation restores a plane other than the one captured, or is resumed by an executor that did not capture it
// Broiler-Human:        PENDING
internal sealed class UbcContinuation : IVmProfileContinuation
{
    internal UbcContinuation(
        object owner,
        UbcFrame[] frames,
        int depth,
        ulong[] words,
        object values,
        int valueCount,
        int resumeIndex,
        object reason)
    {
        Owner = owner;
        Frames = frames;
        Depth = depth;
        Words = words;
        Values = values;
        ValueCount = valueCount;
        ResumeIndex = resumeIndex;
        Reason = reason;
    }

    internal object Owner { get; }

    internal UbcFrame[] Frames { get; }

    internal int Depth { get; }

    internal ulong[] Words { get; }

    internal object Values { get; }

    internal int ValueCount { get; }

    internal int ResumeIndex { get; }

    internal object Reason { get; }

    /// <summary>Set when a resumption has taken it: a continuation resumes once.</summary>
    internal bool Taken { get; set; }
}

/// <summary>
/// The interpreter's executor for one family: instantiation, invocation, resumption and unwinding,
/// each answered as a core step and never as a core outcome.
/// </summary>
/// <remarks>
/// A meter refusal is answered as a contract violation naming the allowance, and a false poll as one
/// naming cancellation; the core rewrites both from its own latches into the exhaustion or the
/// cancellation that actually happened, which is the only way a profile can name a dimension.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=0DCB04
// Broiler-Falsified-If: a step is answered with an instance, continuation or payload another executor made, or a guest program's fault is answered as a contract violation
// Broiler-Human:        PENDING
internal sealed class UbcExecutor<TFamily> : IVmProfileExecutor
    where TFamily : struct, IUbcFamily
{
    private readonly UbcFamilyRegistration<TFamily> family;
    private readonly UbcFamilyDeclaration declaration;
    private readonly IVmExecutionEnvironment environment;

    internal UbcExecutor(UbcFamilyRegistration<TFamily> family, UbcFamilyDeclaration declaration, IVmExecutionEnvironment environment)
    {
        this.family = family;
        this.declaration = declaration;
        this.environment = environment;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6CD237
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId => environment.ProfileId;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B958BC
    // Broiler-Falsified-If: a handle this executor's family did not verify is instantiated
    // Broiler-Human:        PENDING
    public VmExecutionStep Instantiate(VmVerifiedArtifact artifact, System.Threading.CancellationToken cancellationToken)
    {
        if (!artifact.TryGetState(out var state) || state is not UbcVerifiedProgram program ||
            !string.Equals(program.Table.FamilyIdentity, family.Identity, System.StringComparison.Ordinal))
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var familyState = TFamily.CreateInstance(new UbcInstanceContext(program, environment));
        return VmExecutionStep.Instantiated(new UbcInstance(program, familyState, this), null);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D7B803
    // Broiler-Falsified-If: an entry name no entry carries starts a unit, or another executor's instance is run
    // Broiler-Human:        PENDING
    public VmExecutionStep Invoke(IVmInstanceState state, in VmInvocationRequest request, System.Threading.CancellationToken cancellationToken)
    {
        if (state is not UbcInstance instance || !ReferenceEquals(instance.Owner, this))
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        var name = request.EntryPoint.Utf8;

        if (!instance.Program.TryGetEntry(name, out var unit))
        {
            return VmExecutionStep.Faulted(TFamily.EntryRefused(instance.FamilyState, name));
        }

        var interpreter = new UbcInterpreter<TFamily>(instance, environment.Meter, environment.Capabilities, declaration.MaxUnchargedWork);
        return interpreter.Start(unit, name);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5992DA
    // Broiler-Falsified-If: a continuation is resumed twice, or into an instance it was not captured from
    // Broiler-Human:        PENDING
    public VmExecutionStep Resume(IVmInstanceState state, IVmProfileContinuation continuation, System.Threading.CancellationToken cancellationToken)
    {
        if (state is not UbcInstance instance || !ReferenceEquals(instance.Owner, this) ||
            continuation is not UbcContinuation captured || !ReferenceEquals(captured.Owner, instance) || captured.Taken)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        captured.Taken = true;
        var interpreter = new UbcInterpreter<TFamily>(instance, environment.Meter, environment.Capabilities, declaration.MaxUnchargedWork);
        return interpreter.Resume(captured);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Nothing to unwind: a suspended operation holds heap frames and copied planes and no resource the
    /// runtime has to give back; dropping the continuation drops them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5DBDBE
    // Broiler-Human:        PENDING
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }
}
