// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           2
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  6/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The executor: it allocates a store, initialises it, and runs the interpreter over an exported
/// function.
/// </summary>
/// <remarks>
/// <para>
/// <b>NO EXCEPTION MAY REACH THE CORE FROM HERE, AND THAT IS ARRANGED RATHER THAN HOPED FOR.</b>
/// Every member's body is wrapped, and every escape becomes one of the five step kinds. The core
/// does catch what an executor throws, but a profile that relied on that would be reporting its own
/// defects as an unspecified profile fault with nothing said about which defect it was.
/// </para>
/// <para>
/// <b>A trap is a fault and a defect is a contract violation, and the two are answered
/// differently.</b> A trap is a guest program reaching a state the specification says traps, and it
/// leaves as <c>Faulted</c> carrying a typed payload that names which trap and where. A refusal
/// because this assembly was handed something it did not produce, or because it disagreed with what
/// its own validator proved, leaves as <c>ContractViolation</c> carrying a reason and no payload.
/// The core reports the two differently on purpose, so that a defect here is never read as a guest
/// program going wrong.
/// </para>
/// <para>
/// <b>An exhausted budget is neither.</b> When a charge is refused the meter has already latched the
/// refusal, and the core's own precedence rewrites whatever this executor answers into a resource
/// exhaustion naming the dimension and the scope. The answers below on that path exist so the member
/// returns something legal, not because a caller will see them.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=6; Fingerprint=ED8401
// Broiler-Falsified-If: any input makes a member of this type throw, or produces an answer that is not one of the five execution-step kinds
// Broiler-Human:        PENDING
public sealed class WebAssemblyExecutor : IVmProfileExecutor
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DBD8E9
    // Broiler-Human:        PENDING
    private readonly IVmExecutionEnvironment environment;

    /// <summary>Creates the executor for one profile identity and one runtime's environment.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=84B8D5
    // Broiler-Human:        PENDING
    internal WebAssemblyExecutor(VmProfileId profileId, IVmExecutionEnvironment executionEnvironment)
    {
        ProfileId = profileId;
        environment = executionEnvironment;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCA6CF
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId { get; }

    /// <summary>
    /// Allocates and initialises the store, then runs the start function if the module names one.
    /// </summary>
    /// <remarks>
    /// <b>NOTHING IS PUBLISHED UNLESS EVERY PART OF IT SUCCEEDED.</b> A segment that does not fit and
    /// a start function that traps both end the instantiation with a typed trap and no instance, so a
    /// caller never receives a handle onto a half-initialised store.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B6295D
    // Broiler-Falsified-If: an instance is published after a segment or the start function refused, or an exception escapes this member
    // Broiler-Human:        PENDING
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact, System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            return InstantiateCore(artifact, cancellationToken);
        }
        catch (System.OperationCanceledException)
        {
            return VmExecutionStep.ContractViolation(VmReason.Cancelled);
        }
        catch (System.Exception)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }
    }

    /// <summary>Resolves the entry point, pushes its arguments, and runs it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=196678
    // Broiler-Falsified-If: an exception escapes this member, or a trap leaves as anything but a typed payload
    // Broiler-Human:        PENDING
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            return InvokeCore(state, in request);
        }
        catch (System.OperationCanceledException)
        {
            return VmExecutionStep.ContractViolation(VmReason.Cancelled);
        }
        catch (System.Exception)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }
    }

    /// <summary>Refuses the resumption, because nothing here ever parks.</summary>
    /// <remarks>
    /// At every manifest this profile accepts, execution runs to completion or to a trap: no
    /// instruction of the binary format parks a frame, the descriptor declares neither asynchronous
    /// instantiation nor external suspension, and no path in this assembly returns a suspended step.
    /// A continuation arriving here is therefore a state this profile can never have been in, which
    /// is what the reason below says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=41A099
    // Broiler-Human:        PENDING
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.WrongState);

    /// <summary>
    /// Releases the store an abandoned operation was running against, and runs no guest code.
    /// </summary>
    /// <remarks>
    /// <b>THE RELEASE ARM IS REACHABLE ONLY THROUGH A CONTINUATION THIS BUILD NEVER MINTS.</b>
    /// Nothing here suspends, so no continuation of this profile exists to be abandoned, and this
    /// member does nothing today. It is written against the store rather than left empty because the
    /// store is what an unwind would have to release, and because the descriptor's abandon budget of
    /// zero is a statement that there is no guest code to run under a budget - not that there would
    /// be nothing to free.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8FAAB7
    // Broiler-Falsified-If: any guest instruction is dispatched on this path
    // Broiler-Human:        PENDING
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
        try
        {
            if (continuation is WasmContinuation parked)
            {
                parked.Store.Release(environment.Meter);
            }
        }
        catch (System.Exception)
        {
            // The core swallows what this member throws, so a throw here would be a silent one.
            // Answering nothing at all is the same outcome said out loud.
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=9A4768
    // Broiler-Falsified-If: a store outlives a refused instantiation without being released
    // Broiler-Human:        PENDING
    private VmExecutionStep InstantiateCore(
        VmVerifiedArtifact artifact, System.Threading.CancellationToken cancellationToken)
    {
        if (artifact is null || !artifact.TryGetState(out var state) || state is not WasmModule module)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        if (!module.ExecutionBoundsComputed)
        {
            // A module whose bodies validation never sealed cannot be run, and reaching one here
            // means this assembly's own verifier handed back something it should not have.
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        var pacing = new WasmPacing(environment.Meter, WebAssemblyProfile.MaxUnchargedWork);

        if (!pacing.TryCharge(1))
        {
            return Refused(pacing.Failure);
        }

        if (!WasmStore.TryAllocate(module, pacing.Meter, out var store, out var refusedByCeiling))
        {
            if (refusedByCeiling)
            {
                // THIS PROFILE'S OWN CEILING, REPORTED THROUGH THE ONLY CHANNEL THERE IS. There is
                // no reason code for "this profile declines to allocate that much", so the refusal
                // is made on the meter by asking for a quantity no level can admit; the core then
                // reports a resource exhaustion naming allocated bytes and the scope that refused.
                _ = pacing.Meter.TryCharge(VmBudgetDimension.AllocatedBytes, ulong.MaxValue);
            }

            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        for (var index = 0; index < module.GlobalCount; index++)
        {
            store!.Globals[index] = Constant(module.Globals[index].Initializer);
        }

        var elements = module.Elements;

        for (var index = 0; index < elements.Length; index++)
        {
            var segment = elements[index];
            var offset = (ulong)(uint)Constant(segment.Offset).I32;

            if (!pacing.TryReserve((ulong)segment.EntryCount + 1) ||
                !pacing.TryCharge((ulong)segment.EntryCount + 1))
            {
                store!.Release(pacing.Meter);
                return Refused(pacing.Failure);
            }

            if (!store!.Tables[(int)segment.TableIndex].TryInitialise(offset, segment.Entries))
            {
                store.Release(pacing.Meter);
                return Faulted(WasmTrapKind.OutOfBoundsTableAccess, -1, index);
            }
        }

        var data = module.Data;

        for (var index = 0; index < data.Length; index++)
        {
            var segment = data[index];
            var offset = (ulong)(uint)Constant(segment.Offset).I32;

            if (!pacing.TryReserve(((ulong)segment.ByteCount / 64) + 1) ||
                !pacing.TryCharge(((ulong)segment.ByteCount / 64) + 1))
            {
                store!.Release(pacing.Meter);
                return Refused(pacing.Failure);
            }

            if (!store!.Memories[(int)segment.MemoryIndex].TryInitialise(offset, segment.Contents))
            {
                store.Release(pacing.Meter);
                return Faulted(WasmTrapKind.OutOfBoundsMemoryAccess, -1, index);
            }
        }

        if (module.StartFunctionIndex >= 0)
        {
            var interpreter = new WasmInterpreter(module, store!, pacing);
            var status = interpreter.Call((int)module.StartFunctionIndex, [], out _);

            if (status is WasmRunStatus.Trapped)
            {
                store!.Release(pacing.Meter);

                return Faulted(
                    interpreter.TrapKind,
                    interpreter.TrapFunctionIndex,
                    interpreter.TrapOffset);
            }

            if (status is not WasmRunStatus.Completed)
            {
                store!.Release(pacing.Meter);
                return Refused(status);
            }
        }

        return VmExecutionStep.Instantiated(new WasmInstance(module, store!), null);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=5; Fingerprint=41C410
    // Broiler-Falsified-If: an entry point resolves to a function whose parameters the arguments do not match
    // Broiler-Human:        PENDING
    private VmExecutionStep InvokeCore(IVmInstanceState state, in VmInvocationRequest request)
    {
        if (state is not WasmInstance instance)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        var pacing = new WasmPacing(environment.Meter, WebAssemblyProfile.MaxUnchargedWork);

        if (!pacing.TryCharge(1))
        {
            return Refused(pacing.Failure);
        }

        System.Span<WasmValue> arguments = stackalloc WasmValue[WasmEntryPoint.MaximumArguments];
        System.Span<WasmValueType> argumentTypes =
            stackalloc WasmValueType[WasmEntryPoint.MaximumArguments];

        var text = request.EntryPoint.Utf8;

        if (!WasmEntryPoint.TryParse(
            text, arguments, argumentTypes,
            out var nameOffset, out var nameLength, out var argumentCount, out var problem))
        {
            return EntryPointFaulted(problem, argumentCount);
        }

        var name = text.Slice(nameOffset, nameLength);
        var function = -1;

        for (var index = 0; index < instance.Module.ExportCount; index++)
        {
            var export = instance.Module.Exports[index];

            if (!System.MemoryExtensions.SequenceEqual(export.Name, name))
            {
                continue;
            }

            if (export.Kind is not WasmExportKind.Function)
            {
                return EntryPointFaulted(WebAssemblyEntryPointProblem.ExportIsNotAFunction, -1);
            }

            function = (int)export.EntityIndex;
            break;
        }

        if (function < 0)
        {
            return EntryPointFaulted(WebAssemblyEntryPointProblem.UnknownExport, -1);
        }

        var signature = instance.Module.Types[
            (int)instance.Module.FunctionTypeIndices[function]];

        if (signature.ParameterCount != argumentCount)
        {
            return EntryPointFaulted(WebAssemblyEntryPointProblem.ArgumentCountMismatch, -1);
        }

        for (var index = 0; index < argumentCount; index++)
        {
            if (signature.Parameters[index] != argumentTypes[index])
            {
                return EntryPointFaulted(
                    WebAssemblyEntryPointProblem.ArgumentTypeMismatch, index);
            }
        }

        instance.InvocationCount++;

        var interpreter = new WasmInterpreter(instance.Module, instance.Store, pacing);
        var status = interpreter.Call(function, arguments[..argumentCount], out var resultCount);

        if (status is WasmRunStatus.Trapped)
        {
            return Faulted(
                interpreter.TrapKind, interpreter.TrapFunctionIndex, interpreter.TrapOffset);
        }

        if (status is not WasmRunStatus.Completed)
        {
            return Refused(status);
        }

        if (resultCount != signature.ResultCount)
        {
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        var results = resultCount == 0
            ? []
            : new WebAssemblyValue[resultCount];

        for (var index = 0; index < resultCount; index++)
        {
            results[index] = new WebAssemblyValue(
                KindOf(signature.Results[index]), interpreter.ResultAt(index).Low);
        }

        return VmExecutionStep.Completed(new WebAssemblyResults(ProfileId, results));
    }

    /// <summary>Reads a constant expression's one instruction into a value.</summary>
    /// <remarks>
    /// The decoder admits five opcodes here and validation refused the fifth, so the four below are
    /// the whole of it and there is no evaluator: a constant expression at this format version has
    /// exactly one instruction and this is it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0009BC
    // Broiler-Human:        PENDING
    private static WasmValue Constant(WasmConstantExpression expression) => WasmValue.FromBits(
        (WasmOpcode)expression.Opcode is WasmOpcode.I32Const
            ? (uint)expression.Bits
            : expression.Bits);

    /// <summary>Projects a decoded value type onto the payload vocabulary.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A81FA8
    // Broiler-Human:        PENDING
    private static WebAssemblyValueKind KindOf(WasmValueType type) => type switch
    {
        WasmValueType.I32 => WebAssemblyValueKind.I32,
        WasmValueType.I64 => WebAssemblyValueKind.I64,
        WasmValueType.F32 => WebAssemblyValueKind.F32,
        _ => WebAssemblyValueKind.F64,
    };

    /// <summary>Builds the step a trap leaves as, with its code and its position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=98D997
    // Broiler-Falsified-If: a trap kind reaches a caller without the diagnostic code its registry row names
    // Broiler-Human:        PENDING
    private VmExecutionStep Faulted(WasmTrapKind kind, int functionIndex, int offset) =>
        VmExecutionStep.Faulted(new WebAssemblyTrap(
            ProfileId,
            kind,
            (int)DiagnosticFor(kind),
            new VmSourcePosition(
                sectionIndex: (int)WasmSectionId.Code,
                byteOffset: offset < 0 ? 0 : (ulong)offset,
                profileCoordinate0: functionIndex,
                profileCoordinate1: offset)));

    /// <summary>Builds the step an unresolvable entry point leaves as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=765D98
    // Broiler-Human:        PENDING
    private VmExecutionStep EntryPointFaulted(
        WebAssemblyEntryPointProblem problem, int argumentIndex) =>
        VmExecutionStep.Faulted(
            new WebAssemblyEntryPointFault(ProfileId, problem, argumentIndex));

    /// <summary>The registry row a trap kind carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E67B2
    // Broiler-Human:        PENDING
    private static WebAssemblyDiagnosticCode DiagnosticFor(WasmTrapKind kind) => kind switch
    {
        WasmTrapKind.Unreachable => WebAssemblyDiagnosticCode.TrapUnreachable,
        WasmTrapKind.IntegerDivideByZero => WebAssemblyDiagnosticCode.TrapIntegerDivideByZero,
        WasmTrapKind.IntegerOverflow => WebAssemblyDiagnosticCode.TrapIntegerOverflow,
        WasmTrapKind.InvalidConversionToInteger =>
            WebAssemblyDiagnosticCode.TrapInvalidConversionToInteger,
        WasmTrapKind.OutOfBoundsMemoryAccess =>
            WebAssemblyDiagnosticCode.TrapOutOfBoundsMemoryAccess,
        WasmTrapKind.OutOfBoundsTableAccess =>
            WebAssemblyDiagnosticCode.TrapOutOfBoundsTableAccess,
        WasmTrapKind.UndefinedElement => WebAssemblyDiagnosticCode.TrapUndefinedElement,
        WasmTrapKind.IndirectCallTypeMismatch =>
            WebAssemblyDiagnosticCode.TrapIndirectCallTypeMismatch,
        _ => WebAssemblyDiagnosticCode.TrapUninitializedElement,
    };

    /// <summary>
    /// The step a refused charge or a refused poll leaves as, which the core then rewrites.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0EE701
    // Broiler-Human:        PENDING
    private static VmExecutionStep Refused(WasmRunStatus status) => status switch
    {
        WasmRunStatus.Cancelled => VmExecutionStep.ContractViolation(VmReason.Cancelled),
        WasmRunStatus.Exhausted => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
        _ => VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation),
    };
}
