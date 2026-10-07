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

using Broiler.VM.Abstractions;
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
    /// <remarks>
    /// Version 2 executes call rows of the signature effect form, traps an out-of-range truncation as an
    /// integer overflow rather than an invalid conversion, charges one fuel unit before it resolves an
    /// invocation's entry point, and runs a family's admission and start unit in the instantiation step.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B84AAF
    // Broiler-Human:        PENDING
    public const int SemanticVersion = 2;

    /// <summary>
    /// The bytecode form: the identity <see cref="UbcFormat.BytecodeForm"/> and the interpreter as its
    /// executor, written for universal bytecode contract version 2 and carrying the version this
    /// assembly was compiled against, which the descriptor factory compares with its own.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=798ACE
    // Broiler-Human:        PENDING
    public static UbcForm Form { get; } = new(UbcFormat.BytecodeForm, SemanticVersion, new UbcBytecodeExecutorFactory(), authoredUbcContractVersion: 2);
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
/// <para>
/// A meter refusal is answered as a contract violation naming the allowance, and a false poll as one
/// naming cancellation; the core rewrites both from its own latches into the exhaustion or the
/// cancellation that actually happened, which is the only way a profile can name a dimension.
/// </para>
/// <para>
/// <b>What a family chooses is checked before it is used.</b> A unit a family names - the unit an entry
/// point resolved to, an instance's start unit - is entered only when the program has it, an entry
/// only when its unit is flagged as one, and an answer of no kind is a contract violation. An instance
/// this executor will not answer as instantiated is abandoned through the family before the step
/// answers, whatever ended it: an admission that was not ready, a start unit that did not complete, or
/// an exception. An exception from the family's <c>CreateInstance</c> itself leaves no state to
/// abandon, so what the family charged before it threw is the family's to give back before the
/// exception leaves it. One it does answer as instantiated is the core's to publish, and the core drops one
/// whose meter latched a refusal or a cancellation during the step without abandoning it: a defect of
/// the core, written out in `docs/tasks/release-dropped-instantiation-retention.md`. <i>(Corrected
/// 2026-09-28: the core now releases what the instance level holds on every instantiation path that
/// publishes nothing, in `45778cf`, so what the family retained is no longer left counted; it still
/// drops the state without abandoning it through this executor.)</i>
/// </para>
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
    /// <remarks>
    /// <para>
    /// The family makes the instance state, then says whether it may be published. A ready instance
    /// with a start unit runs that unit here, in this step and under its meter, and is published only
    /// when the unit completes: a fault of the start unit is the instantiation's fault, and every other
    /// end is answered as the run answered it or, for a suspension, as a contract violation.
    /// </para>
    /// <para>
    /// Every path on which this executor does not answer the state as instantiated abandons it through
    /// the family first, an
    /// exception from any family member included, so what the state retained is released within the
    /// step that retained it. An exception other than a cancellation is answered as a contract
    /// violation; a cancellation is passed on to the core once the state is abandoned.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=2A89A8
    // Broiler-Falsified-If: a handle this executor's family did not verify is instantiated, an instance is published whose admission was not ready or whose start unit did not complete, a unit the program does not have is entered, or a state this executor does not answer as instantiated outlives the step without being abandoned
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

        object? familyState = null;
        UbcInterpreter<TFamily>? interpreter = null;
        var abandoned = false;

        try
        {
            familyState = TFamily.CreateInstance(new UbcInstanceContext(program, environment));
            var admission = TFamily.AdmitInstance(familyState);

            switch (admission.Kind)
            {
                case UbcInstanceAnswerKind.Ready:
                    break;

                case UbcInstanceAnswerKind.Faulted:
                    Abandon();
                    return VmExecutionStep.Faulted(admission.Fault);

                case UbcInstanceAnswerKind.Exhausted:
                    Abandon();
                    return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);

                default:
                    Abandon();
                    return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
            }

            var instance = new UbcInstance(program, familyState, this);
            var start = TFamily.StartUnit(familyState);

            if (start == -1)
            {
                return VmExecutionStep.Instantiated(instance, null);
            }

            if ((uint)start >= (uint)program.Units.Length)
            {
                Abandon();
                return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
            }

            interpreter = new UbcInterpreter<TFamily>(instance, environment.Meter, environment.Capabilities, declaration.MaxUnchargedWork);
            var ran = interpreter.RunStart(start);

            switch (ran.Kind)
            {
                case VmExecutionStepKind.Completed:
                    return VmExecutionStep.Instantiated(instance, null);

                case VmExecutionStepKind.Faulted:
                case VmExecutionStepKind.ContractViolation:
                    Abandon();
                    return ran;

                default:
                    Abandon();
                    return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
            }
        }
        catch (System.Exception failure)
        {
            // A handler that threw left the start run's frames standing; their depth is given back
            // here, in the step that charged it, as an ending the run answered would have given it.
            interpreter?.ReleaseDepth();

            if (familyState is not null)
            {
                Abandon();
            }

            if (failure is System.OperationCanceledException)
            {
                throw;
            }

            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        // At most once per state, even when abandoning it is what threw.
        void Abandon()
        {
            if (!abandoned)
            {
                abandoned = true;
                TFamily.AbandonInstance(familyState!);
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// One fuel unit is charged before the family resolves the entry point: resolving it is work the
    /// invocation asks for before any frame stands to pay for it. A unit the family names is entered
    /// only when the program has it and it is flagged as an entry.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=274F83
    // Broiler-Falsified-If: an entry point starts a unit the program does not have or one not flagged as an entry, an answer of no kind starts anything, the family resolves before the fuel unit is charged, or another executor's instance is run
    // Broiler-Human:        PENDING
    public VmExecutionStep Invoke(IVmInstanceState state, in VmInvocationRequest request, System.Threading.CancellationToken cancellationToken)
    {
        if (state is not UbcInstance instance || !ReferenceEquals(instance.Owner, this))
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var name = request.EntryPoint.Utf8;
        var answer = TFamily.ResolveEntry(instance.FamilyState, instance.Program, name);

        switch (answer.Kind)
        {
            case UbcEntryAnswerKind.Found:
            {
                var units = instance.Program.Units;

                if ((uint)answer.Unit >= (uint)units.Length || (units[answer.Unit].Unit.Flags & UbcUnitFlags.Entry) == 0)
                {
                    return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
                }

                var interpreter = new UbcInterpreter<TFamily>(instance, environment.Meter, environment.Capabilities, declaration.MaxUnchargedWork);
                return interpreter.Start(answer.Unit, name);
            }

            case UbcEntryAnswerKind.Missing:
                return VmExecutionStep.Faulted(TFamily.EntryRefused(instance.FamilyState, name));

            case UbcEntryAnswerKind.Refused:
                return VmExecutionStep.Faulted(answer.Fault);

            default:
                return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }
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
    /// Nothing to unwind: a suspended operation holds heap frames and copied planes, whose bytes were
    /// charged to an allowance that is never given back, and no depth, which the suspension gave back
    /// when it parked; dropping the continuation drops them. Nothing could be given back here in any
    /// case, because the core calls this outside any step, where no meter answers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5DBDBE
    // Broiler-Human:        PENDING
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }
}
