// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           10
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Text;
using System.Threading;
using Broiler.VM;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=947864
// Broiler-Human:        PENDING
internal sealed unsafe class MachineCodeInstance : IVmInstanceState, IDisposable
{
    public MachineCodeProgram Program { get; }
    public VmNativePage Page { get; }
    public IVmExecutionEnvironment Environment { get; }
    public double[] Operands { get; }
    public double[] Locals { get; }
    public double[] Constants { get; }
    public long[] Fuel { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=EAF862
    // Broiler-Human:        PENDING
    internal const long InitialFuelAllowance = 1L << 32;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=AC8FE7
    // Broiler-Human:        PENDING
    public MachineCodeInstance(
        MachineCodeProgram program,
        VmNativePage page,
        IVmExecutionEnvironment environment,
        double[] operands,
        double[] locals,
        double[] constants,
        long[] fuel)
    {
        Program = program;
        Page = page;
        Environment = environment;
        Operands = operands;
        Locals = locals;
        Constants = constants;
        Fuel = fuel;
        Fuel[0] = InitialFuelAllowance;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2EB2AA
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        Page.Dispose();
    }
}

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=918783
// Broiler-Human:        PENDING
public sealed unsafe class MachineCodeExecutor : IVmProfileExecutor
{
    public VmProfileId ProfileId { get; }
    private readonly IVmExecutionEnvironment environment;

    public MachineCodeExecutor(VmProfileId profileId, IVmExecutionEnvironment environment)
    {
        ProfileId = profileId;
        this.environment = environment;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3401B9
    // Broiler-Human:        PENDING
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        CancellationToken cancellationToken)
    {
        if (!artifact.TryGetState(out var rawState) || rawState is not MachineCodeProgram program)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        var page = VmNativePage.TryMap(program.Code);
        if (page is null)
        {
            return VmExecutionStep.ContractViolation(VmReason.ResourceExhaustionUnspecified);
        }

        if (!page.Arm())
        {
            page.Dispose();
            return VmExecutionStep.ContractViolation(VmReason.ResourceExhaustionUnspecified);
        }

        var operands = GC.AllocateArray<double>(
            Math.Max(1, (int)program.Limits.MaximumOperandStack),
            pinned: true);

        var locals = GC.AllocateArray<double>(
            Math.Max(1, (int)program.Limits.MaximumScopeSlots),
            pinned: true);

        var constants = GC.AllocateArray<double>(
            Math.Max(1, program.Constants.Length),
            pinned: true);

        Array.Copy(program.Constants, constants, program.Constants.Length);

        var fuel = GC.AllocateArray<long>(1, pinned: true);

        var instance = new MachineCodeInstance(program, page, environment, operands, locals, constants, fuel);
        return VmExecutionStep.Instantiated(instance, null);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=5BC078
    // Broiler-Human:        PENDING
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        CancellationToken cancellationToken)
    {
        if (state is not MachineCodeInstance instance)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        var entryName = Encoding.UTF8.GetString(request.EntryPoint.Utf8);
        if (!instance.Program.TryFindEntry(entryName, out var entry))
        {
            return VmExecutionStep.Faulted(null);
        }

        if (!instance.Program.TryFindSymbol(entry.FunctionIndex, out var symbol))
        {
            return VmExecutionStep.Faulted(null);
        }

        var entryAddress = instance.Page.At(symbol.Offset);
        var initialFuel = instance.Fuel[0];

        fixed (double* operandsPtr = instance.Operands)
        fixed (double* localsPtr = instance.Locals)
        fixed (double* constantsPtr = instance.Constants)
        fixed (long* fuelPtr = instance.Fuel)
        {
            var frame = new VmNativeFrame
            {
                Operands = operandsPtr,
                OperandCount = instance.Operands.Length,
                Locals = localsPtr,
                LocalCount = instance.Locals.Length,
                Constants = constantsPtr,
                Fuel = fuelPtr,
                BailoutPc = 0,
            };

            var func = (delegate* unmanaged<VmNativeFrame*, VmNativeReturn>)entryAddress;
            var result = func(&frame);

            var spent = initialFuel - instance.Fuel[0];
            if (spent > 0)
            {
                instance.Environment.Meter.TryCharge(VmBudgetDimension.Fuel, (ulong)spent);
            }

            if (result == VmNativeReturn.Returned)
            {
                return VmExecutionStep.Completed(null);
            }

            return VmExecutionStep.Faulted(null);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CF2405
    // Broiler-Human:        PENDING
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        CancellationToken cancellationToken) =>
        VmExecutionStep.Completed(null);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5DBDBE
    // Broiler-Human:        PENDING
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }
}
