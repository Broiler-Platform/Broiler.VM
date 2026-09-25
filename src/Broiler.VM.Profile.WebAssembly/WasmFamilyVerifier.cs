// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           0
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Critical
// Criteria:         8/6
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly family's verifier hook: what only this profile knows about a universal bytecode
/// artifact, checked over its module definitions and at every family instruction the walk admitted.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS A TRUST BOUNDARY, AND IT IS TOTAL.</b> Any artifact that names <c>broiler.webassembly</c>
/// reaches this hook once the walk has admitted its structure, whether a translation wrote it or a
/// hand did, so every byte it reads is untrusted. It answers for every input and throws for none:
/// every entry point is wrapped, a cancellation that escapes is answered as the verifier-work stop
/// the walk turns into a cancellation, and any other escape is a defect of this assembly answered with
/// the reserved code <see cref="WebAssemblyDiagnosticCode.VerifierDefect"/> - the core deliberately
/// does not catch a verifier's exception, so an escape would reach the host as a crash.
/// </para>
/// <para>
/// <b>What it checks at the start.</b> The artifact carries its module definitions in the FamilyData
/// section of family slot one; <see cref="WasmFamilyData.TryRead"/> reads them and checks everything
/// they say about themselves. The hook then checks what they say about the artifact: every Types row
/// and every unit's locals are word types, because the family has no value plane; every element
/// segment's functions and every function export name a unit, and an exported function's unit is
/// flagged as an entry; the start function names a unit whose signature takes and gives nothing; and
/// the Positions rows ascend strictly, which lets a trap find its position by one search.
/// </para>
/// <para>
/// <b>What it checks at each instruction.</b> A memory access, <c>memory.size</c> and
/// <c>memory.grow</c> need the memory declared; the latter two carry a reserved byte of zero; a load's
/// or store's alignment is at most its natural one; a global row names a global the definitions
/// declare, of the row's type, and a set names a mutable one; <c>call_indirect</c> needs the table.
/// Each is constant work, which the walk paid for when it charged the hook pass one unit per
/// instruction. The numeric rows need nothing: the walk typed them from their effects.
/// </para>
/// <para>
/// <b>Its codes.</b> A refusal that means exactly what a decoder or validator code of this profile
/// means carries that code and its reason; the rest are the 2850 band of
/// <see cref="WebAssemblyDiagnosticCode"/>. Every refusal carries an invalid-artifact reason, which the
/// walk requires of a hook.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=0B3876
// Broiler-Falsified-If: any input makes a member of this type throw, or an artifact whose definitions or family rows break a check listed here is admitted
// Broiler-Human:        PENDING
internal sealed class WasmFamilyVerifier : IUbcFamilyVerifier
{
    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=D7C76C
    // Broiler-Falsified-If: an exception escapes this member, or a family state is answered alongside a refusal
    // Broiler-Human:        PENDING
    public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
    {
        familyState = null;

        try
        {
            var answer = BeginCore(artifact, out var definitions);

            if (!answer.Refused)
            {
                familyState = definitions;
            }

            return answer;
        }
        catch (System.OperationCanceledException)
        {
            return UbcHookAnswer.Exhaust(VmBudgetDimension.VerifierWork);
        }
        catch (System.Exception)
        {
            return Defect();
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=C5BC7C
    // Broiler-Falsified-If: an exception escapes this member, or an instruction breaking a check listed on the type is admitted
    // Broiler-Human:        PENDING
    public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction)
    {
        try
        {
            return familyState is WasmDefinitions definitions
                ? CheckInstructionCore(definitions, instruction.Opcode, instruction.Operand, instruction.Row)
                : Defect();
        }
        catch (System.OperationCanceledException)
        {
            return UbcHookAnswer.Exhaust(VmBudgetDimension.VerifierWork);
        }
        catch (System.Exception)
        {
            return Defect();
        }
    }

    /// <inheritdoc/>
    /// <remarks>Nothing is left to check once every instruction has been seen.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=90C99A
    // Broiler-Human:        PENDING
    public UbcHookAnswer End(object? familyState) =>
        familyState is WasmDefinitions ? UbcHookAnswer.Admit : Defect();

    /// <summary>A refusal with <paramref name="code"/> and the one core reason that code carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3E9812
    // Broiler-Falsified-If: a code is answered with a reason other than the one its every other emission carries
    // Broiler-Human:        PENDING
    internal static UbcHookAnswer Refuse(WebAssemblyDiagnosticCode code) =>
        UbcHookAnswer.Refuse((int)code, ReasonOf(code));

    /// <summary>
    /// The core reason each code the hook emits carries: the reason its decoder or validator
    /// emission carries for a code the hook shares, and one fixed reason for each of the hook's own.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=994351
    // Broiler-Falsified-If: a shared code's reason differs from the reason the decoder or validator gives it
    // Broiler-Human:        PENDING
    internal static VmReason ReasonOf(WebAssemblyDiagnosticCode code) => code switch
    {
        WebAssemblyDiagnosticCode.ModuleDefinitionsTruncated => VmReason.Truncated,
        WebAssemblyDiagnosticCode.ModuleDefinitionsVersionUnsupported => VmReason.UnknownFormatVersion,
        WebAssemblyDiagnosticCode.ModuleDefinitionsMalformedPresence
            or WebAssemblyDiagnosticCode.MalformedLimitsFlag
            or WebAssemblyDiagnosticCode.UnknownValueType
            or WebAssemblyDiagnosticCode.MalformedMutabilityFlag
            or WebAssemblyDiagnosticCode.MalformedNameEncoding
            or WebAssemblyDiagnosticCode.UnknownExternalKind
            or WebAssemblyDiagnosticCode.ReservedImmediateNotZero => VmReason.MalformedEncoding,
        WebAssemblyDiagnosticCode.StartFunctionIndexOutOfRange
            or WebAssemblyDiagnosticCode.StartFunctionSignatureInvalid
            or WebAssemblyDiagnosticCode.ExportIndexOutOfRange
            or WebAssemblyDiagnosticCode.DuplicateExportName
            or WebAssemblyDiagnosticCode.ElementSegmentTableIndexOutOfRange
            or WebAssemblyDiagnosticCode.ElementSegmentFunctionIndexOutOfRange
            or WebAssemblyDiagnosticCode.DataSegmentMemoryIndexOutOfRange
            or WebAssemblyDiagnosticCode.GlobalIndexOutOfRange
            or WebAssemblyDiagnosticCode.GlobalIsImmutable
            or WebAssemblyDiagnosticCode.MemoryNotDeclared
            or WebAssemblyDiagnosticCode.TableNotDeclared
            or WebAssemblyDiagnosticCode.AlignmentAboveNaturalAlignment
            or WebAssemblyDiagnosticCode.GlobalRowTypeMismatch => VmReason.SemanticValidationFailed,
        _ => VmReason.InconsistentStructure,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=68F0ED
    // Broiler-Human:        PENDING
    private static UbcHookAnswer Defect() => Refuse(WebAssemblyDiagnosticCode.VerifierDefect);

    /// <summary>Reads the definitions, checks them against the artifact, and indexes its positions.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=BE194D
    // Broiler-Falsified-If: definitions are answered from an artifact without the section of family slot one, or before every check against the artifact has passed
    // Broiler-Human:        PENDING
    private static UbcHookAnswer BeginCore(UbcHookArtifact artifact, out WasmDefinitions? definitions)
    {
        definitions = null;

        // The walk has refused a FamilyData section of any slot the artifact does not declare, so the
        // family in slot one and a section of slot one are the one section of kind nine.
        var present = false;

        foreach (var section in artifact.Artifact.FamilyData)
        {
            present |= section.Slot == WasmFamilyTable.Slot;
        }

        if (artifact.Slot != WasmFamilyTable.Slot || !present)
        {
            return Refuse(WebAssemblyDiagnosticCode.ModuleDefinitionsMissing);
        }

        var meter = new WasmHookMeter(artifact.Meter);
        var answer = WasmFamilyData.TryRead(artifact.FamilyData, meter, out var read);

        if (answer.Refused)
        {
            return answer;
        }

        answer = CheckAgainstArtifact(artifact.Artifact, read!, meter);

        if (answer.Refused)
        {
            return answer;
        }

        answer = WasmPositionIndex.TryBuild(artifact.Artifact.Positions, meter, out var positions);

        if (answer.Refused)
        {
            return answer;
        }

        definitions = read!.WithPositions(positions);
        return UbcHookAnswer.Admit;
    }

    /// <summary>
    /// What the definitions say about the artifact: its types are words, its segments and exports name
    /// its units, an exported function is an entry, and its start unit takes and gives nothing.
    /// </summary>
    /// <remarks>
    /// Called after the walk's structural layer, which has proved every unit's type index names a
    /// Types row, and before the code walk. Its passes are charged before any runs: one unit per Types
    /// row, unit, element entry and export, and one per slot and local run it reads.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=1370ED
    // Broiler-Falsified-If: a value slot, an element or export naming no unit, an exported function without the entry flag, or a start unit that takes or gives anything is admitted
    // Broiler-Human:        PENDING
    private static UbcHookAnswer CheckAgainstArtifact(UbcArtifact artifact, WasmDefinitions definitions, WasmHookMeter meter)
    {
        var units = artifact.Units;
        var types = artifact.Types;
        var entries = 0UL;

        foreach (var element in definitions.Elements)
        {
            entries += (ulong)element.Functions.Length;
        }

        if (!meter.TryChargeWork((ulong)types.Length + (ulong)units.Length + (ulong)definitions.Elements.Length + entries + (ulong)definitions.Exports.Length + 1))
        {
            return meter.Exhausted();
        }

        foreach (var type in types)
        {
            if (!meter.TryChargeWork((ulong)type.Parameters.Length + (ulong)type.Results.Length))
            {
                return meter.Exhausted();
            }

            if (type.Parameters.Contains(UbcSlotType.V) || type.Results.Contains(UbcSlotType.V))
            {
                return Refuse(WebAssemblyDiagnosticCode.ValueSlotNotAdmitted);
            }
        }

        foreach (var unit in units)
        {
            if (!meter.TryChargeWork((ulong)unit.Locals.Length))
            {
                return meter.Exhausted();
            }

            foreach (var run in unit.Locals)
            {
                if (run.Type == UbcSlotType.V)
                {
                    return Refuse(WebAssemblyDiagnosticCode.ValueSlotNotAdmitted);
                }
            }
        }

        foreach (var element in definitions.Elements)
        {
            foreach (var function in element.Functions)
            {
                if (function >= (uint)units.Length)
                {
                    return Refuse(WebAssemblyDiagnosticCode.ElementSegmentFunctionIndexOutOfRange);
                }
            }
        }

        foreach (var export in definitions.Exports)
        {
            if (export.Kind != WasmExportKind.Function)
            {
                continue;
            }

            if (export.Index >= (uint)units.Length)
            {
                return Refuse(WebAssemblyDiagnosticCode.ExportIndexOutOfRange);
            }

            if ((units[(int)export.Index].Flags & UbcUnitFlags.Entry) == 0)
            {
                return Refuse(WebAssemblyDiagnosticCode.ExportedFunctionNotAnEntry);
            }
        }

        if (definitions.Start != WasmDefinitions.NoStart)
        {
            if (definitions.Start >= (uint)units.Length)
            {
                return Refuse(WebAssemblyDiagnosticCode.StartFunctionIndexOutOfRange);
            }

            var typeIndex = units[(int)definitions.Start].TypeIndex;

            if (typeIndex >= (uint)types.Length)
            {
                // The walk's structural layer refused this before the hook was called.
                return Defect();
            }

            var signature = types[(int)typeIndex];

            if (signature.Parameters.Length != 0 || signature.Results.Length != 0)
            {
                return Refuse(WebAssemblyDiagnosticCode.StartFunctionSignatureInvalid);
            }
        }

        return UbcHookAnswer.Admit;
    }

    /// <summary>The checks at one family instruction, in the order the validator makes the same checks.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=0; Fingerprint=293B18
    // Broiler-Falsified-If: a memory row with no memory, a nonzero reserved byte, an alignment above natural, a global row naming no global, a global of another type or an immutable one it sets, or an indirect call with no table is admitted
    // Broiler-Human:        PENDING
    private static UbcHookAnswer CheckInstructionCore(WasmDefinitions definitions, byte opcode, ulong operand, UbcInstructionRow row)
    {
        if (opcode is WasmFamilyTable.MemorySize or WasmFamilyTable.MemoryGrow)
        {
            if (operand != 0)
            {
                return Refuse(WebAssemblyDiagnosticCode.ReservedImmediateNotZero);
            }

            return definitions.HasMemory ? UbcHookAnswer.Admit : Refuse(WebAssemblyDiagnosticCode.MemoryNotDeclared);
        }

        if (row.Primitive is { } primitive && UbcPrimitives.IsRegionAccess(primitive))
        {
            var alignment = UbcOperandShapes.First(row.Shape, operand);
            var natural = (ulong)System.Numerics.BitOperations.Log2((uint)UbcPrimitives.AccessWidth(primitive));

            if (alignment > natural)
            {
                return Refuse(WebAssemblyDiagnosticCode.AlignmentAboveNaturalAlignment);
            }

            return definitions.HasMemory ? UbcHookAnswer.Admit : Refuse(WebAssemblyDiagnosticCode.MemoryNotDeclared);
        }

        if (WasmFamilyTable.IsGlobal(opcode))
        {
            if (operand >= (ulong)definitions.Globals.Length)
            {
                return Refuse(WebAssemblyDiagnosticCode.GlobalIndexOutOfRange);
            }

            var global = definitions.Globals[(int)operand];

            if (global.Kind != WasmFamilyTable.GlobalTypeOf(opcode))
            {
                return Refuse(WebAssemblyDiagnosticCode.GlobalRowTypeMismatch);
            }

            return WasmFamilyTable.IsGlobalSet(opcode) && !global.IsMutable
                ? Refuse(WebAssemblyDiagnosticCode.GlobalIsImmutable)
                : UbcHookAnswer.Admit;
        }

        if (opcode == WasmFamilyTable.CallIndirect)
        {
            return definitions.HasTable ? UbcHookAnswer.Admit : Refuse(WebAssemblyDiagnosticCode.TableNotDeclared);
        }

        return UbcHookAnswer.Admit;
    }
}
