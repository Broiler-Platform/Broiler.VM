// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   35
// Annotated:        35/35
// Exempt:           19
// Human-reviewed:   0/35
// IP risk:          Low
// Security risk:    Critical
// Criteria:         20/17
// Resource impact:  4/10 max
// Unverified:       35
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly family's handlers, payloads and instance state: the struct a universal bytecode
/// emitter's loop is specialised over to run a translated WebAssembly module.
/// </summary>
/// <remarks>
/// <para>
/// <b>A composition names the type and nothing else.</b> Every member is an explicit implementation of
/// <see cref="IUbcFamily"/>, reached by an emitter through the type argument of
/// <see cref="WebAssemblyProfile.Registration"/> and by nobody else, so the type is on this profile's
/// public surface and its members are not.
/// </para>
/// <para>
/// <b>The rows.</b> The numeric rows are primitives an emitter executes itself; where one does not, the
/// handler answers with this profile's own arms, <see cref="WasmReferenceNumerics"/>. A load, a store
/// and <c>memory.size</c> are the region primitive over the instance's memory, read at the access;
/// <c>memory.grow</c> is route MVP-1's growth, charged through the step's meter with its retention
/// charged before the allocation, and a growth a core budget refused ends the step; a global row reads or
/// writes the instance's global bits; <c>call_indirect</c> checks the table index, the entry and the
/// callee's module type in that order and asks the emitter to enter the callee.
/// </para>
/// <para>
/// <b>What an instantiation does</b>, in the order the base executor did it: the memory and the
/// table at their declared minimums, charged and retained; the globals from their initial bits; the
/// element segments and then the data segments, each atomic and each charged, a segment that does
/// not fit recording the trap the instantiation faults with. A start function, if the module names
/// one, is the emitter's to run once the instance is admitted. An instance the emitter does not
/// answer as instantiated gives back everything it retained, within the step that retained it. That includes an
/// instantiation the core would refuse to publish: the core answers a step whose meter latched a
/// refusal as that refusal and drops its state unabandoned, so every retention here is charged
/// before its allocation and every refusal the family meets ends the step, and no instantiation
/// the family lets complete carries a latched refusal of its own. A refusal the family never meets is
/// the one case left - an aggregate wall clock the meter accrues and latches at a poll that still
/// answers - and the core drops that state unabandoned with nothing released: a defect of the core,
/// written out in `docs/tasks/release-dropped-instantiation-retention.md`. An exception is not a case
/// left: an allocation that throws after its charges gives its retention back before the exception
/// leaves it, and the making gives back everything it retained before any exception leaves
/// <c>CreateInstance</c>, where the emitter has no state yet to abandon.
/// </para>
/// <para>
/// <b>No value plane.</b> WebAssembly's values are all words, so the family's plane holds nothing and
/// the hook refuses any artifact naming a value slot. Nothing in the table suspends, throws or lands,
/// so the members that serve those are unreachable and say so by throwing.
/// </para>
/// <para>
/// <b>The base.</b> Where these remarks name the base executor, the base interpreter or the base store,
/// they mean the bare-module path this profile carried until milestone UBC-4 retired it: its
/// descriptor's executor, the interpreter that executor ran and the store it allocated. The family
/// answers what that path answered - the traps, their codes and positions, the entry-point faults and
/// the order of an instantiation - and evidence bundle ubc-4-001 retains the base run it is held to.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=3; Fingerprint=E5586C
// Broiler-Falsified-If: a handler writes outside its row's effect, a trap is answered with a code of another meaning than the base interpreter's, or an instance the emitter does not answer as instantiated keeps retained bytes
// Broiler-Human:        PENDING
public struct WasmFamily : IUbcFamily
{
    /// <summary>Executes one family row.</summary>
    /// <remarks>
    /// A growth a core budget refused ends the step through <see cref="UbcStatusKind.Defect"/>, the one
    /// status a dynamic row has that ends it without a trap the guest could see. It is not answered as
    /// a contract violation: a refused charge or retention latched an exhaustion on the meter, a
    /// refused poll the cancellation or the wall-clock exhaustion it saw, and the core ranks what it
    /// latched above the step's kind, at an invocation and at an instantiation alike. The loop's meter
    /// polls at the declared bound, so no poll inside a growth is refused for the bound itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=2CB32A
    // Broiler-Falsified-If: a row is answered from state it does not own, a trap it raises is answered as another, or a growth a core budget refused lets the guest run on
    // Broiler-Human:        PENDING
    static UbcStatus IUbcFamily.Handle(ref UbcActivation activation, byte familyOpcode, ulong operand)
    {
        if (activation.InstanceState is not WasmInstanceState state || !WasmFamilyTable.Table.TryGetRow(familyOpcode, out var row))
        {
            return UbcStatus.Defect;
        }

        var words = activation.Words;
        var at = activation.WordArgs;

        if (familyOpcode is >= WasmFamilyTable.FirstNumeric and <= WasmFamilyTable.LastNumeric)
        {
            // A numeric row reaches its handler only where an emitter does not execute the primitive
            // itself, and the answer there is the profile's own arm - including the arms' routing
            // defect, which is answered as a defect of the family.
            if (!WasmReferenceNumerics.TryEvaluate(familyOpcode, words[at], row.Effect.Pops.Length > 1 ? words[at + 1] : 0, out var bits, out var trap))
            {
                return UbcStatus.Defect;
            }

            if (trap != 0)
            {
                return UbcStatus.Trap((ushort)trap);
            }

            words[at] = bits;
            return UbcStatus.Next;
        }

        if (familyOpcode is >= WasmFamilyTable.FirstAccess and <= WasmFamilyTable.MemorySize)
        {
            return Access(state, row, words, at, operand);
        }

        if (familyOpcode == WasmFamilyTable.MemoryGrow)
        {
            if (state.Memory is not { } memory)
            {
                return UbcStatus.Defect;
            }

            // Route MVP-1: a request above the profile's own ceiling answers minus one before any
            // charge, and the guest goes on with it. The step's meter paces the fuel with the loop's
            // own, and the retention is charged before the allocation, so every refusal by a core
            // budget is seen here. Such a refusal latched on the meter, and the core answers the
            // operation as the exhaustion or cancellation it latched whatever the step does next, so
            // the step ends here rather than running the guest past a growth it will never be told
            // about: an instantiation whose start function grows is then abandoned, and gives back
            // what it retained, rather than completed and dropped by the core with its retention.
            var answer = memory.Grow((uint)words[at], activation.Meter, out var refusedByBudget);

            if (refusedByBudget)
            {
                return UbcStatus.Defect;
            }

            words[at] = (uint)(int)answer;
            return UbcStatus.Next;
        }

        if (WasmFamilyTable.IsGlobal(familyOpcode))
        {
            if (operand >= (ulong)state.Globals.Length)
            {
                return UbcStatus.Defect;
            }

            if (WasmFamilyTable.IsGlobalSet(familyOpcode))
            {
                var kind = WasmFamilyTable.GlobalTypeOf(familyOpcode);
                state.Globals[(int)operand] = kind is WasmGlobalKind.I32 or WasmGlobalKind.F32 ? (uint)words[at] : words[at];
            }
            else
            {
                words[at] = state.Globals[(int)operand];
            }

            return UbcStatus.Next;
        }

        if (familyOpcode == WasmFamilyTable.CallIndirect)
        {
            return CallIndirect(ref activation, state, operand);
        }

        return UbcStatus.Defect;
    }

    /// <summary>Unreachable: the table declares no region kind, so nothing lands.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=006744
    // Broiler-Human:        PENDING
    static void IUbcFamily.OnLand(ref UbcActivation activation, byte regionKind) =>
        throw new System.InvalidOperationException("the WebAssembly family declares no region kind, so no landing reaches it");

    /// <summary>Unreachable: no row of the table suspends.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=967442
    // Broiler-Human:        PENDING
    static void IUbcFamily.OnResume(ref UbcActivation activation, object reason) =>
        throw new System.InvalidOperationException("no row of the WebAssembly family suspends, so no resumption reaches it");

    /// <summary>The null plane: the family has no language value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=609310
    // Broiler-Human:        PENDING
    static IUbcValuePlane IUbcFamily.CreateValuePlane(int capacity) => WasmNullPlane.Instance;

    /// <summary>Makes the instance: the store at its minimums, the globals, and the segments applied.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=3; Fingerprint=D6DB06
    // Broiler-Falsified-If: the state answered is not the one the instance's definitions and the environment's meter make
    // Broiler-Human:        PENDING
    static object IUbcFamily.CreateInstance(UbcInstanceContext context) => WasmInstanceState.Create(context);

    /// <summary>A recorded segment trap faults the instantiation, a refused charge exhausts it, and anything else is ready.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=B1FE11
    // Broiler-Falsified-If: an instance whose segment trapped or whose charge was refused is answered ready
    // Broiler-Human:        PENDING
    static UbcInstanceAnswer IUbcFamily.AdmitInstance(object instanceState)
    {
        if (instanceState is not WasmInstanceState { Defective: false } state)
        {
            // No answer: the executor ends the step as the family's contract violation.
            return default;
        }

        if (state.SegmentTrap is { } kind)
        {
            // The base executor's position for a segment: no function, and the segment's index where
            // an instruction's offset would be.
            return UbcInstanceAnswer.Faulted(Trap(kind, -1, state.SegmentIndex));
        }

        return state.Exhausted ? UbcInstanceAnswer.Exhausted : UbcInstanceAnswer.Ready;
    }

    /// <summary>The start function's unit, or minus one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F680C3
    // Broiler-Human:        PENDING
    static int IUbcFamily.StartUnit(object instanceState) =>
        instanceState is WasmInstanceState { Defective: false } state ? state.Definitions.StartUnit : -1;

    /// <summary>Gives back what the instance retained, through the step's meter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=3447A9
    // Broiler-Falsified-If: an abandoned instance keeps any byte reported retained
    // Broiler-Human:        PENDING
    static void IUbcFamily.AbandonInstance(object instanceState)
    {
        if (instanceState is WasmInstanceState state)
        {
            state.Release();
        }
    }

    /// <summary>
    /// Resolves route MVP-6's entry point: the export of exactly the name the text carries, a function,
    /// whose parameters the arguments match in count and type.
    /// </summary>
    /// <remarks>
    /// Every refusal is the fault the base executor answered for it: a problem of the text with
    /// the index of the argument being read, a name no export carries or one naming another kind of
    /// thing with no argument index, a count mismatch with none, and a type mismatch with the argument's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=434777
    // Broiler-Falsified-If: an entry point resolves to a unit whose parameters its arguments do not match, or a refusal carries another problem or argument index than the base executor's
    // Broiler-Human:        PENDING
    static UbcEntryAnswer IUbcFamily.ResolveEntry(object instanceState, UbcVerifiedProgram program, System.ReadOnlySpan<byte> entryPoint)
    {
        if (program.FamilyState is not WasmDefinitions definitions)
        {
            return default;
        }

        System.Span<ulong> bits = stackalloc ulong[WasmEntryPoint.MaximumArguments];
        System.Span<WasmValueType> types = stackalloc WasmValueType[WasmEntryPoint.MaximumArguments];

        if (!WasmEntryPoint.TryParse(entryPoint, bits, types, out var nameOffset, out var nameLength, out var count, out var problem))
        {
            return Refused(problem, count);
        }

        if (!definitions.TryFindExport(entryPoint.Slice(nameOffset, nameLength), out var export))
        {
            return Refused(WebAssemblyEntryPointProblem.UnknownExport, -1);
        }

        if (export.Kind != WasmExportKind.Function)
        {
            return Refused(WebAssemblyEntryPointProblem.ExportIsNotAFunction, -1);
        }

        if (export.Index >= (uint)program.Units.Length)
        {
            return default;
        }

        var parameters = program.Units[(int)export.Index].Signature.Parameters;

        if (parameters.Length != count)
        {
            return Refused(WebAssemblyEntryPointProblem.ArgumentCountMismatch, -1);
        }

        for (var index = 0; index < count; index++)
        {
            if (SlotOf(types[index]) != parameters[index])
            {
                return Refused(WebAssemblyEntryPointProblem.ArgumentTypeMismatch, index);
            }
        }

        return UbcEntryAnswer.Found((int)export.Index);
    }

    /// <summary>
    /// Writes the entry point's argument bits into the entry frame's parameter locals, reading the text
    /// again as <see cref="IUbcFamily.ResolveEntry"/> read it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=0FBD82
    // Broiler-Falsified-If: a local beyond the unit's parameters is written, or an argument is bound to a parameter of another type
    // Broiler-Human:        PENDING
    static bool IUbcFamily.BindParameters(ref UbcActivation activation, System.ReadOnlySpan<byte> entryName)
    {
        System.Span<ulong> bits = stackalloc ulong[WasmEntryPoint.MaximumArguments];
        System.Span<WasmValueType> types = stackalloc WasmValueType[WasmEntryPoint.MaximumArguments];

        if (!WasmEntryPoint.TryParse(entryName, bits, types, out _, out _, out var count, out _))
        {
            return false;
        }

        var unit = activation.Program.Units[activation.Unit];
        var parameters = unit.Signature.Parameters;

        if (count != parameters.Length || count != unit.ParameterWords)
        {
            return false;
        }

        for (var index = 0; index < count; index++)
        {
            if (SlotOf(types[index]) != parameters[index])
            {
                return false;
            }

            activation.Words[activation.WordBase + index] = bits[index];
        }

        return true;
    }

    /// <summary>The entry unit's results, each with the kind its result type names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68A5E4
    // Broiler-Falsified-If: a result is reported with another kind than its type, or from another slot than the argument bases
    // Broiler-Human:        PENDING
    static IVmProfilePayload? IUbcFamily.Completion(ref UbcActivation activation)
    {
        var results = activation.Program.Units[activation.Unit].Signature.Results;
        var values = results.Length == 0 ? [] : new WebAssemblyValue[results.Length];

        for (var index = 0; index < values.Length; index++)
        {
            values[index] = new WebAssemblyValue(KindOf(results[index]), activation.Words[activation.WordArgs + index]);
        }

        return new WebAssemblyResults(WebAssemblyProfile.Id, values);
    }

    /// <summary>
    /// A trap's payload: the universal unreachable or a family code as the <see cref="WasmTrapKind"/> of
    /// the same value, its registry code, and the position the translation recorded for the row.
    /// </summary>
    /// <remarks>
    /// The position is the base executor's: the code section, the instruction's offset inside its own
    /// function body, and the function index. It is read from the Positions row of the trapping
    /// instruction's unit and absolute offset; a row with none answers the unit's index and offset zero.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=83277A
    // Broiler-Falsified-If: a trap's kind, registry code or position differs from what the base executor answered for the same instruction
    // Broiler-Human:        PENDING
    static IVmProfilePayload IUbcFamily.Fault(ref UbcActivation activation, byte familySlot, ushort code)
    {
        var kind = familySlot != 0 && code is >= (ushort)WasmTrapKind.Unreachable and <= (ushort)WasmTrapKind.UninitializedElement
            ? (WasmTrapKind)code
            : WasmTrapKind.Unreachable;

        var function = activation.Unit;
        var offset = 0;

        if (activation.Program.FamilyState is WasmDefinitions definitions &&
            definitions.Positions.TryFind(activation.Unit, activation.Pc, out var recordedFunction, out var recordedOffset))
        {
            function = recordedFunction;
            offset = recordedOffset;
        }

        return Trap(kind, function, offset);
    }

    /// <summary>Unreachable: no row of the table throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4EB822
    // Broiler-Human:        PENDING
    static IVmProfilePayload? IUbcFamily.Uncaught(ref UbcActivation activation) =>
        throw new System.InvalidOperationException("no row of the WebAssembly family throws, so nothing is uncaught");

    /// <summary>Unreachable: no row of the table suspends.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E51CE1
    // Broiler-Human:        PENDING
    static IVmProfilePayload? IUbcFamily.SuspendProjection(ref UbcActivation activation) =>
        throw new System.InvalidOperationException("no row of the WebAssembly family suspends, so nothing is projected");

    /// <summary>The fault an entry point no export resolves to answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=12DCEA
    // Broiler-Human:        PENDING
    static IVmProfilePayload? IUbcFamily.EntryRefused(object instanceState, System.ReadOnlySpan<byte> entryName) =>
        new WebAssemblyEntryPointFault(WebAssemblyProfile.Id, WebAssemblyEntryPointProblem.UnknownExport, -1);

    /// <summary>Unreachable: nothing suspends, so no frame is captured.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CB5862
    // Broiler-Human:        PENDING
    static object IUbcFamily.CaptureValues(IUbcValuePlane plane, int start, int count) =>
        throw new System.InvalidOperationException("no row of the WebAssembly family suspends, so no frame is captured");

    /// <summary>Unreachable: nothing suspends, so no frame is restored.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A218D0
    // Broiler-Human:        PENDING
    static void IUbcFamily.RestoreValues(IUbcValuePlane plane, int start, object captured) =>
        throw new System.InvalidOperationException("no row of the WebAssembly family suspends, so no frame is restored");

    /// <summary>
    /// A load, a store or <c>memory.size</c>: the region primitive over the memory's current bytes, the
    /// address and a store's value at the argument base, the static offset the operand's second field.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=0; Fingerprint=9BE7E2
    // Broiler-Falsified-If: an access reads or writes a byte the region primitive's bounds check refused, or a view of the memory is held past the access
    // Broiler-Human:        PENDING
    private static UbcStatus Access(WasmInstanceState state, UbcInstructionRow row, ulong[] words, int at, ulong operand)
    {
        if (state.Memory is not { } memory || row.Primitive is not { } primitive)
        {
            return UbcStatus.Defect;
        }

        var value = row.Effect.Pops.Length > 1 ? words[at + 1] : 0;
        var offset = (uint)UbcOperandShapes.Second(row.Shape, operand);

        // The base is read here, at the access, and the view is gone when the call returns: a growth
        // republishes it, and nothing here outlives the instruction.
        var result = UbcPrimitives.EvaluateRegion(primitive, memory.Bytes, (uint)words[at], offset, value, memory.PageCount);

        if (result.Trap != UbcTrapCode.None)
        {
            foreach (var mapping in row.Traps)
            {
                if (mapping.Universal == result.Trap)
                {
                    return UbcStatus.Trap(mapping.FamilyCode);
                }
            }

            return UbcStatus.Defect;
        }

        if (row.Effect.Pushes.Length > 0)
        {
            words[at] = result.Bits;
        }

        return UbcStatus.Next;
    }

    /// <summary>
    /// <c>call_indirect</c>: the table index is the top slot, above the named type's parameters; an
    /// index past the table, a null entry and a callee of another module type each trap, in that
    /// order, and a callee that fits is requested.
    /// </summary>
    /// <remarks>
    /// The type check is nominal, as the base interpreter's is: the callee's unit must carry the very
    /// type index the instruction names, so two identical signatures declared under two indices do not
    /// match. The translation writes the module's types one for one, so the unit's type index is the
    /// module's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=0; Fingerprint=422CCD
    // Broiler-Falsified-If: an index past the table, a null entry or a callee of another type index is requested rather than trapped, or the traps are raised in another order than the base interpreter's
    // Broiler-Human:        PENDING
    private static UbcStatus CallIndirect(ref UbcActivation activation, WasmInstanceState state, ulong operand)
    {
        var program = activation.Program;

        if (state.Table is not { } table || operand >= (ulong)program.Artifact.Types.Length)
        {
            return UbcStatus.Defect;
        }

        var parameterWords = program.Artifact.Types[(int)operand].Parameters.Length;
        var index = (uint)activation.Words[activation.WordArgs + parameterWords];

        if (!table.TryRead(index, out var entry))
        {
            return UbcStatus.Trap((ushort)WasmTrapKind.OutOfBoundsTableAccess);
        }

        if (entry == WasmTableInstance.NullFunctionReference)
        {
            return UbcStatus.Trap((ushort)WasmTrapKind.UninitializedElement);
        }

        if ((uint)entry >= (uint)program.Units.Length)
        {
            return UbcStatus.Defect;
        }

        if (program.Units[entry].Unit.TypeIndex != operand)
        {
            return UbcStatus.Trap((ushort)WasmTrapKind.IndirectCallTypeMismatch);
        }

        activation.CallRequest = new UbcCallRequest(entry);
        return UbcStatus.Request;
    }

    /// <summary>A trap payload at a function index and a body-relative offset, as the base executor built one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DE4B9F
    // Broiler-Human:        PENDING
    private static WebAssemblyTrap Trap(WasmTrapKind kind, int function, int offset) =>
        new(
            WebAssemblyProfile.Id,
            kind,
            (int)DiagnosticFor(kind),
            new VmSourcePosition(
                sectionIndex: (int)WasmSectionId.Code,
                byteOffset: offset < 0 ? 0 : (ulong)offset,
                profileCoordinate0: function,
                profileCoordinate1: offset));

    /// <summary>The registry row a trap kind carries, as the base executor mapped it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E67B2
    // Broiler-Falsified-If: a trap kind is mapped to a registry row of another trap
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

    /// <summary>An entry point refused with <paramref name="problem"/> at <paramref name="argumentIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AAF54C
    // Broiler-Human:        PENDING
    private static UbcEntryAnswer Refused(WebAssemblyEntryPointProblem problem, int argumentIndex) =>
        UbcEntryAnswer.Refused(new WebAssemblyEntryPointFault(WebAssemblyProfile.Id, problem, argumentIndex));

    /// <summary>The slot type an argument of <paramref name="type"/> binds to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4D5B27
    // Broiler-Human:        PENDING
    private static UbcSlotType SlotOf(WasmValueType type) => type switch
    {
        WasmValueType.I32 => UbcSlotType.I32,
        WasmValueType.I64 => UbcSlotType.I64,
        WasmValueType.F32 => UbcSlotType.F32,
        WasmValueType.F64 => UbcSlotType.F64,
        _ => UbcSlotType.V,
    };

    /// <summary>The payload kind of a result of <paramref name="type"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D18C47
    // Broiler-Human:        PENDING
    private static WebAssemblyValueKind KindOf(UbcSlotType type) => type switch
    {
        UbcSlotType.I32 => WebAssemblyValueKind.I32,
        UbcSlotType.I64 => WebAssemblyValueKind.I64,
        UbcSlotType.F32 => WebAssemblyValueKind.F32,
        _ => WebAssemblyValueKind.F64,
    };
}

/// <summary>The family's value plane: it holds nothing, because every WebAssembly value is a word.</summary>
/// <remarks>
/// Its capacity is unbounded because it needs no room, so a loop never grows it; its values are charged
/// at zero bytes; and copying or clearing any count of nothing does nothing. The hook refuses an
/// artifact naming a value slot, so no instruction reaches it with a value.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Low; Resources=0; Fingerprint=700D19
// Broiler-Human:        PENDING
internal sealed class WasmNullPlane : IUbcValuePlane
{
    /// <summary>The one plane, shared: it has no state.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=14F980
    // Broiler-Human:        PENDING
    internal static WasmNullPlane Instance { get; } = new();

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6E4E42
    // Broiler-Human:        PENDING
    public int Capacity => int.MaxValue;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CEB476
    // Broiler-Human:        PENDING
    public int ValueBytes => 0;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AAAE43
    // Broiler-Human:        PENDING
    public void Resize(int capacity)
    {
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3B26B7
    // Broiler-Human:        PENDING
    public void Copy(int from, int to)
    {
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C74E68
    // Broiler-Human:        PENDING
    public void Clear(int start, int count)
    {
    }
}

/// <summary>
/// One instance's store under the universal bytecode: its memory, its table and its globals, what its
/// making recorded, and the environment whose meter it charges and releases through.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is the base store's allocation, in the base store's order and at its charges</b> - the
/// store the bare-module executor allocated until milestone UBC-4 retired it, which this state
/// replaced. The memory is a pinned array at its declared minimum (decision WAD-0001), charged to
/// allocated bytes and retained as live bytes; a minimum above the profile's own page ceiling is
/// refused by forcing the latched exhaustion the base executor forced. The table is an array of
/// unit indices at its minimum, null throughout, charged and retained the same way under its own
/// ceiling. The globals are not charged, as they never were. Each element and then each data
/// segment is charged under the profile's pacing - its entries plus one, its bytes over sixty-four
/// plus one - and applied whole or not at all, and the first that does not fit stops the making
/// with its trap recorded.
/// </para>
/// <para>
/// <b>Two departures from the base store, both about a refusal it did not see or a bound it did not
/// keep.</b> The retention of the memory and of the table is charged before the array exists, where
/// the base store reported it after: a report returns nothing, so the base store went on past a
/// refused retention to an instantiation the core then answered as that exhaustion and dropped, with
/// everything retained still counted. And a segment's charge is made in pieces no larger than the
/// uncharged-work bound, each polled for as the pacing requires, where the base store made it whole:
/// one charge above the bound is work the core measures as unpolled.
/// </para>
/// <para>
/// <b>Nothing it retained outlives a refusal it answers.</b> Every path the emitter does not answer as
/// instantiated releases the memory and the table through the environment's meter, read at the release, within the
/// instantiation step: at once where the base executor released at once, and through the family's
/// abandon otherwise. Releasing is idempotent, so the two cannot release twice. An exception in the
/// making releases before it leaves, and an allocation that throws after its charges gives back the
/// retention it was charged for. A state the emitter answers as instantiated and the core then drops
/// is not released here; the class remarks say when.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=6E21EB
// Broiler-Falsified-If: an array is allocated before its allocation and retention charges return true, a segment is applied in part, more fuel than the uncharged-work bound is charged between two polls, or a state the emitter does not answer as instantiated keeps a byte reported retained
// Broiler-Human:        PENDING
internal sealed class WasmInstanceState
{
    private bool released;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=10980F
    // Broiler-Human:        PENDING
    private WasmInstanceState(UbcVerifiedProgram program, WasmDefinitions definitions, IVmExecutionEnvironment environment, bool defective)
    {
        Program = program;
        Definitions = definitions;
        Environment = environment;
        Defective = defective;
        Globals = defective || definitions.Globals.IsEmpty ? System.Array.Empty<ulong>() : new ulong[definitions.Globals.Length];
    }

    /// <summary>The verified program the instance runs.</summary>
    internal UbcVerifiedProgram Program { get; }

    /// <summary>The module definitions the hook answered.</summary>
    internal WasmDefinitions Definitions { get; }

    /// <summary>The environment of the instantiation, whose meter every charge and release goes through.</summary>
    internal IVmExecutionEnvironment Environment { get; }

    /// <summary>True when the program carries no definitions this family's hook made, which is a defect.</summary>
    internal bool Defective { get; }

    /// <summary>The memory, when the module declares one and it was allocated.</summary>
    internal WasmMemoryInstance? Memory { get; private set; }

    /// <summary>The table, when the module declares one and it was allocated.</summary>
    internal WasmTableInstance? Table { get; private set; }

    /// <summary>The globals' bits, a thirty-two-bit global's in the low half.</summary>
    internal ulong[] Globals { get; }

    /// <summary>The trap a segment that did not fit recorded, or null.</summary>
    internal WasmTrapKind? SegmentTrap { get; private set; }

    /// <summary>The index of the segment that recorded <see cref="SegmentTrap"/>.</summary>
    internal int SegmentIndex { get; private set; }

    /// <summary>True when a charge or a poll of the making was refused.</summary>
    internal bool Exhausted { get; private set; }

    /// <summary>Makes an instance's state from its context, as the type's remarks describe.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=013B1D
    // Broiler-Falsified-If: a store is allocated before its charges, a segment is applied past a refused charge or poll, a minimum above a profile ceiling is allocated, or an exception leaves the making with a byte reported retained
    // Broiler-Human:        PENDING
    internal static WasmInstanceState Create(UbcInstanceContext context)
    {
        if (context.Program.FamilyState is not WasmDefinitions definitions)
        {
            return new WasmInstanceState(context.Program, new WasmDefinitions(false, default, false, default, [], [], [], [], WasmDefinitions.NoStart), context.Environment, defective: true);
        }

        var state = new WasmInstanceState(context.Program, definitions, context.Environment, defective: false);

        // An exception that leaves here leaves the emitter with no state to abandon, so what the making
        // retained is given back first.
        try
        {
            return Make(state, definitions, context.Environment.Meter);
        }
        catch (System.Exception)
        {
            state.Release();
            throw;
        }
    }

    /// <summary>Allocates, initialises and applies the segments of <paramref name="state"/>, as the type's remarks describe.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=596338
    // Broiler-Falsified-If: a store is allocated before its charges, a segment is applied past a refused charge or poll, or a minimum above a profile ceiling is allocated
    // Broiler-Human:        PENDING
    private static WasmInstanceState Make(WasmInstanceState state, WasmDefinitions definitions, IVmMeter meter)
    {
        var pacing = new WasmPacing(meter, WebAssemblyProfile.MaxUnchargedWork);

        // The executor charged the instantiation's one fuel unit before it asked for this state, as the
        // base executor charged it through its own pacing first: counted here toward the poll bound.
        pacing.Observe(1);

        if (!state.AllocateMemory(meter) || !state.AllocateTable(meter))
        {
            return state;
        }

        for (var index = 0; index < state.Globals.Length; index++)
        {
            var global = definitions.Globals[index];
            state.Globals[index] = global.Kind is WasmGlobalKind.I32 or WasmGlobalKind.F32 ? (uint)global.InitialBits : global.InitialBits;
        }

        for (var index = 0; index < definitions.Elements.Length; index++)
        {
            var segment = definitions.Elements[index];

            if (!TryChargePaced(pacing, (ulong)segment.Functions.Length + 1))
            {
                state.Refuse();
                return state;
            }

            if (state.Table is not { } table || !table.TryInitialise(segment.Offset, segment.Functions.AsSpan()))
            {
                state.Trap(WasmTrapKind.OutOfBoundsTableAccess, index);
                return state;
            }
        }

        for (var index = 0; index < definitions.Data.Length; index++)
        {
            var segment = definitions.Data[index];

            if (!TryChargePaced(pacing, ((ulong)segment.Contents.Length / 64) + 1))
            {
                state.Refuse();
                return state;
            }

            if (state.Memory is not { } memory || !memory.TryInitialise(segment.Offset, segment.Contents.Span))
            {
                state.Trap(WasmTrapKind.OutOfBoundsMemoryAccess, index);
                return state;
            }
        }

        return state;
    }

    /// <summary>Reports every byte the state retained as released, once, through the environment's meter as it is now.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5D7247
    // Broiler-Falsified-If: a byte is reported released twice, or one reported retained is never released on a path the emitter does not answer as instantiated
    // Broiler-Human:        PENDING
    internal void Release()
    {
        if (released)
        {
            return;
        }

        released = true;
        var meter = Environment.Meter;
        Memory?.Release(meter);

        if (Table is { } table)
        {
            meter.ReportReleased(VmBudgetDimension.LiveBytes, (ulong)table.EntryCount * sizeof(int));
        }
    }

    /// <summary>
    /// Charges <paramref name="cost"/> fuel under <paramref name="pacing"/> in pieces no larger than the
    /// profile's uncharged-work bound, polling before any piece that would cross it.
    /// </summary>
    /// <remarks>
    /// The pacing buys headroom for one charge by polling first, and a poll cannot make room for a
    /// charge larger than the bound itself: charged whole, a segment of seventy thousand entries or
    /// of four mebibytes and more is work the core measures as unpolled, and the next poll refuses.
    /// The pieces add up to the whole cost, so the fuel a segment spends is the base store's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6D280A
    // Broiler-Falsified-If: a piece larger than the uncharged-work bound is charged, the pieces add up to anything but the cost, or a refused piece is answered as charged
    // Broiler-Human:        PENDING
    private static bool TryChargePaced(WasmPacing pacing, ulong cost)
    {
        while (cost > 0)
        {
            var piece = System.Math.Min(cost, WebAssemblyProfile.MaxUnchargedWork);

            if (!pacing.TryCharge(piece))
            {
                return false;
            }

            cost -= piece;
        }

        return true;
    }

    /// <summary>Allocates the declared memory at its minimum, or answers false with the refusal recorded.</summary>
    /// <remarks>
    /// Its retention is charged before the array exists, so a live-bytes ceiling that refuses it
    /// refuses the instantiation here rather than latching a report the making went on past.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=6B3988
    // Broiler-Falsified-If: the array exists before the allocation and retention charges returned true, a minimum above the page ceiling is allocated, or an allocation that throws leaves its retention reported
    // Broiler-Human:        PENDING
    private bool AllocateMemory(IVmMeter meter)
    {
        if (!Definitions.HasMemory)
        {
            return true;
        }

        var declared = Definitions.Memory;
        var maximum = declared.HasMaximum ? declared.Maximum : WasmTypeGrammar.MaximumMemoryPages;

        if (declared.Minimum > WasmMemoryInstance.ProfileMaximumPages)
        {
            RefuseByProfileCeiling(meter);
            return false;
        }

        var initialBytes = (ulong)declared.Minimum * WasmMemoryInstance.PageBytes;

        if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, initialBytes) ||
            !meter.TryCharge(VmBudgetDimension.LiveBytes, initialBytes))
        {
            Refuse();
            return false;
        }

        byte[] bytes;

        try
        {
            bytes = WasmMemoryInstance.Allocate(initialBytes);
        }
        catch (System.Exception)
        {
            // Nothing holds the retention this charge was for, so nothing else would give it back.
            meter.ReportReleased(VmBudgetDimension.LiveBytes, initialBytes);
            throw;
        }

        Memory = new WasmMemoryInstance(bytes, maximum);
        return true;
    }

    /// <summary>Allocates the declared table at its minimum, every entry null, or answers false with the refusal recorded.</summary>
    /// <remarks>Its retention is charged before the array exists, as the memory's is.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=A2B1CB
    // Broiler-Falsified-If: the array exists before the allocation and retention charges returned true, a minimum above the entry ceiling is allocated, or an allocation that throws leaves its retention reported
    // Broiler-Human:        PENDING
    private bool AllocateTable(IVmMeter meter)
    {
        if (!Definitions.HasTable)
        {
            return true;
        }

        var declared = Definitions.Table;

        if (declared.Minimum > WasmTableInstance.ProfileMaximumTableEntries)
        {
            RefuseByProfileCeiling(meter);
            return false;
        }

        var entryBytes = (ulong)declared.Minimum * sizeof(int);

        if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, entryBytes) ||
            !meter.TryCharge(VmBudgetDimension.LiveBytes, entryBytes))
        {
            Refuse();
            return false;
        }

        int[] entries;

        try
        {
            entries = new int[(int)declared.Minimum];
        }
        catch (System.Exception)
        {
            meter.ReportReleased(VmBudgetDimension.LiveBytes, entryBytes);
            throw;
        }

        System.Array.Fill(entries, WasmTableInstance.NullFunctionReference);
        Table = new WasmTableInstance(entries);
        return true;
    }

    /// <summary>
    /// A minimum above one of the profile's own ceilings: what was retained is released, and the
    /// refusal is made on the meter by asking for a quantity no level can admit, so the core reports an
    /// exhaustion of allocated bytes naming the scope that refused - the base executor's channel.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=801282
    // Broiler-Falsified-If: the refusal leaves the meter unlatched, or retained bytes behind it
    // Broiler-Human:        PENDING
    private void RefuseByProfileCeiling(IVmMeter meter)
    {
        Release();
        _ = meter.TryCharge(VmBudgetDimension.AllocatedBytes, ulong.MaxValue);
        Exhausted = true;
    }

    /// <summary>A refused charge or poll: what was retained is released, and the admission answers exhausted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4462E1
    // Broiler-Human:        PENDING
    private void Refuse()
    {
        Release();
        Exhausted = true;
    }

    /// <summary>A segment that did not fit: what was retained is released, and the trap is recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DB4FCB
    // Broiler-Human:        PENDING
    private void Trap(WasmTrapKind kind, int segment)
    {
        Release();
        SegmentTrap = kind;
        SegmentIndex = segment;
    }
}
