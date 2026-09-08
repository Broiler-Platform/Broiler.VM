// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   75
// Annotated:        75/75
// Exempt:           38
// Human-reviewed:   0/75
// IP risk:          Low
// Security risk:    Critical
// Criteria:         52/52
// Resource impact:  8/10 max
// Unverified:       75
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// A window onto the validator's own type pool: where a run of value types starts and how many
/// there are.
/// </summary>
/// <remarks>
/// <para>
/// <b>A control frame names its types by window rather than by array so that the control stack
/// stays unmanaged.</b> An unmanaged frame is one the core's bounded allocator can size, which means
/// attacker-controlled nesting is bounded and charged by the same mechanism as everything else a
/// payload asks this profile to allocate. A frame holding two arrays would have been two more
/// allocations per block, on a path whose length a guest chooses.
/// </para>
/// <para>
/// It is also why the pool is a constant size. The four numeric types occupy its first four slots
/// and one function's result types occupy the rest, so a block declaring one result names a window
/// into the prefix and appends nothing: the pool does not grow with the number of blocks a body
/// opens.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=06D083
// Broiler-Human:        PENDING
internal readonly struct WasmTypeVector
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=73B046
    // Broiler-Human:        PENDING
    internal WasmTypeVector(int offset, int count)
    {
        Offset = offset;
        Count = count;
    }

    /// <summary>Where the run starts in the validator's type pool.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E5A478
    // Broiler-Human:        PENDING
    internal int Offset { get; }

    /// <summary>How many types the run holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9734FA
    // Broiler-Human:        PENDING
    internal int Count { get; }
}

/// <summary>
/// One entry of the control stack: the specification's control frame, plus the two indices this
/// implementation needs to resolve a jump.
/// </summary>
/// <remarks>
/// <para>
/// <b>The first five members are the specification's own and carry its own names.</b> The opcode
/// that opened the frame, the types a branch to its label expects, the types it leaves when it ends,
/// the operand-stack height it was entered at, and whether the code after an unconditional branch
/// has made it polymorphic. Nothing here is an optimisation of that algorithm: it is that algorithm,
/// written down.
/// </para>
/// <para>
/// <b>The two jump indices are this implementation's and they carry no typing meaning.</b> They
/// point into the table of resolved targets being built for the body, so that the <c>end</c> that
/// closes this frame can write its own position back into the entry the opening instruction made.
/// An <c>else</c> frame carries two, because it closes one entry of its own and also finishes the
/// <c>if</c> entry that preceded it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=E68A6D
// Broiler-Falsified-If: a frame's recorded height is taken after its start types were pushed rather than before
// Broiler-Human:        PENDING
internal struct WasmControlFrame
{
    /// <summary>The instruction that opened the frame.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=F7F55D
    // Broiler-Human:        PENDING
    public byte Opcode;

    /// <summary>The types the frame is entered with, which are a loop's label types.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=9ECF02
    // Broiler-Human:        PENDING
    public WasmTypeVector StartTypes;

    /// <summary>The types the frame leaves behind, which are every other label's types.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=816B92
    // Broiler-Human:        PENDING
    public WasmTypeVector EndTypes;

    /// <summary>The operand-stack height at the moment the frame was pushed.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=14955A
    // Broiler-Human:        PENDING
    public int ValueStackHeight;

    /// <summary>Whether the rest of this frame is unreachable and therefore polymorphic.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=87C871
    // Broiler-Human:        PENDING
    public bool Unreachable;

    /// <summary>The jump entry the frame's own opening instruction made, or minus one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F2FAEB
    // Broiler-Human:        PENDING
    public int JumpIndex;

    /// <summary>The jump entry of the <c>if</c> an <c>else</c> frame continues, or minus one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=784333
    // Broiler-Human:        PENDING
    public int OpenerJumpIndex;
}

/// <summary>
/// The validation pass: the specification's single-pass algorithm over an operand stack and a
/// control stack, run once over a decoded module.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE SPECIFICATION'S OWN ALGORITHM AND NOT A REIMPLEMENTATION OF IT.</b> The
/// operations below carry the names the specification's validation appendix gives them -
/// <c>PushOperand</c>, <c>PopOperand</c>, <c>PushControl</c>, <c>PopControl</c>,
/// <c>LabelTypes</c>, <c>MarkUnreachable</c> - so that a reader holding the appendix can check this
/// against it line by line. Where a member does something the appendix does not, the member says so
/// in its own words.
/// </para>
/// <para>
/// <b>THE SUBTLE PART IS POPPING AT A FRAME'S OWN HEIGHT, AND IT IS THE PART THE CONFORMANCE SUITE
/// IS FULL OF.</b> After an unconditional branch the enclosing frame is marked unreachable and the
/// operand stack is truncated to the height that frame was entered at. Popping AT that height from
/// an unreachable frame answers the BOTTOM type, which satisfies whatever constraint asked for it -
/// which is what makes <c>unreachable i32.add</c> a valid function body rather than a stack
/// underflow. Popping at that height from a frame that is NOT unreachable is a validation error.
/// The two are one comparison apart, and that comparison is the difference between accepting every
/// module a real toolchain emits and accepting modules that pop off the end of a block.
/// </para>
/// <para>
/// <b>Two bounds are computed here because they cannot honestly be read anywhere else.</b> The
/// deepest the operand stack goes and the deepest the control stack goes are high-water marks over
/// a walk that has to happen anyway, and they are stored on the function body so that an
/// interpreter sizes its stacks from a number this pass computed rather than from a number the
/// payload chose. The jump targets are the same argument: this pass already knows, at every
/// <c>end</c>, which opening instruction it closes, so the pairing is recorded and no interpreter
/// ever scans forward for a matching <c>end</c>.
/// </para>
/// <para>
/// <b>Nesting is charged as a high-water mark and released, and the loop is iterative.</b> A level
/// of control nesting is charged to the structural-depth ceiling when it opens and given back when
/// it closes, so a body of ten thousand sibling blocks costs one level rather than ten thousand. And
/// there is no recursion anywhere here: attacker-controlled nesting reaches an explicit array and a
/// ceiling rather than the CLR stack, because a stack overflow is not an answer a verifier can give.
/// </para>
/// <para>
/// <b>It reads the body's bytes through the core's bounded reader, like everything else.</b> That is
/// not ceremony: it is what charges the verifier-work allowance for this second walk over the code,
/// keeps the cancellation poll on its declared cadence, and lets the variable-length immediates be
/// read by <see cref="WasmLeb128"/> rather than by a second copy of the acceptance rule written for
/// a span.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=8; Fingerprint=566927
// Broiler-Falsified-If: a pop at a frame's own height answers the bottom type while the frame is not unreachable, or a module reaches an interpreter with an index, an opcode, a block or an operand stack this pass did not check
// Broiler-Human:        PENDING
internal ref struct WasmValidator
{
    /// <summary>
    /// The BOTTOM type: what a pop answers inside unreachable code, satisfying any constraint.
    /// </summary>
    /// <remarks>
    /// It is a constant of the value-type enumeration rather than a member of it, and deliberately:
    /// zero is not an encoding the binary format assigns to anything, so no byte a payload carries
    /// can decode to it, and adding it to the enumeration would have made a vocabulary of what the
    /// format encodes carry a member the format does not have.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=0; Fingerprint=B6F1EE
    // Broiler-Falsified-If: the binary format assigns this byte to a value type, so a payload can name the bottom type
    // Broiler-Human:        PENDING
    internal const WasmValueType BottomType = (WasmValueType)0x00;

    /// <summary>How many types the pool holds before a function's results are written into it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DBC411
    // Broiler-Human:        PENDING
    private const int NumericTypePrefix = 4;

    /// <summary>How many instructions this pass walks between two cancellation polls.</summary>
    /// <remarks>
    /// It is far below the uncharged-work bound the descriptor declares, because an instruction may
    /// consume several bytes and each byte is a work unit: polling every instruction would take the
    /// meter's lock on the hottest path here, and polling once per declared bound's worth would risk
    /// crossing it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=154EC0
    // Broiler-Falsified-If: the work accumulated between two polls can exceed the uncharged-work bound the descriptor declares
    // Broiler-Human:        PENDING
    private const int InstructionsBetweenPolls = 1_024;

    /// <summary>The capacity a growable stack starts at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4F6B44
    // Broiler-Human:        PENDING
    private const int InitialCapacity = 16;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9E32C1
    // Broiler-Human:        PENDING
    private readonly WasmModule module;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=92AD83
    // Broiler-Human:        PENDING
    private readonly WasmReadAdapter adapter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A92AB3
    // Broiler-Human:        PENDING
    private readonly ulong pollGranularity;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFC99B
    // Broiler-Human:        PENDING
    private VmBoundedReader reader;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5677B3
    // Broiler-Human:        PENDING
    private VmVerifierOutcome refusal;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=56B858
    // Broiler-Human:        PENDING
    private System.ReadOnlySpan<WasmValueType> parameters;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1BAF4A
    // Broiler-Human:        PENDING
    private System.ReadOnlySpan<WasmValueType> locals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5BD9BE
    // Broiler-Human:        PENDING
    private WasmValueType[] values;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3F34CC
    // Broiler-Human:        PENDING
    private WasmControlFrame[] controls;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4424EF
    // Broiler-Human:        PENDING
    private WasmJumpTarget[] jumps;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=470117
    // Broiler-Human:        PENDING
    private WasmValueType[] typePool;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB1206
    // Broiler-Human:        PENDING
    private WasmValueType[] carried;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AFFC02
    // Broiler-Human:        PENDING
    private uint[] branchLabels;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=793392
    // Broiler-Human:        PENDING
    private ulong valueBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8F3042
    // Broiler-Human:        PENDING
    private ulong controlBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BC1AAA
    // Broiler-Human:        PENDING
    private ulong jumpBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=048FD1
    // Broiler-Human:        PENDING
    private ulong typePoolBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C2C812
    // Broiler-Human:        PENDING
    private ulong carriedBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=09DA61
    // Broiler-Human:        PENDING
    private ulong branchLabelBytes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=03E85F
    // Broiler-Human:        PENDING
    private ulong chargedDepth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B80980
    // Broiler-Human:        PENDING
    private int valueCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=368E1B
    // Broiler-Human:        PENDING
    private int controlCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B6F5D2
    // Broiler-Human:        PENDING
    private int jumpCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A29CCA
    // Broiler-Human:        PENDING
    private int maxValueHeight;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FEC5C7
    // Broiler-Human:        PENDING
    private int maxControlDepth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0B6E61
    // Broiler-Human:        PENDING
    private int sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D3DEE5
    // Broiler-Human:        PENDING
    private int sectionIdentifier;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FB6F93
    // Broiler-Human:        PENDING
    private int itemOrdinal;

    /// <summary>Builds a validator over one decoded module, under the ceilings decoding ran under.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E88F05
    // Broiler-Falsified-If: any field is left uninitialised, so a failed validation reads a buffer nothing filled
    // Broiler-Human:        PENDING
    internal WasmValidator(WasmModule decoded, WasmReadAdapter meter, ulong granularity)
    {
        module = decoded;
        adapter = meter;
        pollGranularity = granularity;
        reader = default;
        refusal = default;
        parameters = default;
        locals = default;
        values = [];
        controls = [];
        jumps = [];
        typePool = [];
        carried = [];
        branchLabels = [];
        valueBytes = 0;
        controlBytes = 0;
        jumpBytes = 0;
        typePoolBytes = 0;
        carriedBytes = 0;
        branchLabelBytes = 0;
        chargedDepth = 0;
        valueCount = 0;
        controlCount = 0;
        jumpCount = 0;
        maxValueHeight = 0;
        maxControlDepth = 0;
        sinceLastPoll = 0;
        sectionIdentifier = -1;
        itemOrdinal = -1;
    }

    /// <summary>
    /// Validates the whole module, answering either that it is valid or the one refusal that
    /// stopped it.
    /// </summary>
    /// <remarks>
    /// Every scratch buffer this pass took is given back on every path, success and failure alike,
    /// and so is every level of nesting still charged when a body was abandoned mid-block. A pass
    /// that released only on success would make a host that survived one hostile artifact refuse
    /// the next legitimate one, which is the defect the core's own allocator guards against on its
    /// own path.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=E081FE
    // Broiler-Falsified-If: a scratch reservation or a charged level of nesting survives this call on any path
    // Broiler-Human:        PENDING
    internal bool TryValidate(out VmVerifierOutcome outcome)
    {
        var validated = TryValidateModule();

        ReleaseScratch();
        outcome = refusal;
        return validated;
    }

    /// <summary>The module-level checks, then one pass over each function body.</summary>
    /// <remarks>
    /// The index spaces are checked before any body is walked, because a body's <c>call</c> reads a
    /// function's declared type and a type index that addresses nothing would otherwise have to be
    /// discovered twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=210294
    // Broiler-Falsified-If: a function body is walked before the type indices it reads have been held to the type space
    // Broiler-Human:        PENDING
    private bool TryValidateModule()
    {
        if (!TryReserveScratch())
        {
            return false;
        }

        if (!TryValidateFunctionTypes() ||
            !TryValidateStartFunction() ||
            !TryValidateExports() ||
            !TryValidateGlobals() ||
            !TryValidateElementSegments() ||
            !TryValidateDataSegments())
        {
            return false;
        }

        for (var index = 0; index < module.FunctionCount; index++)
        {
            if (!TryValidateBody(index))
            {
                return false;
            }
        }

        return true;
    }

    // =============================================================================================
    // The module's own index spaces
    // =============================================================================================

    /// <summary>Holds every defined function's type index to the declared type space.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=52A978
    // Broiler-Falsified-If: a function whose type index addresses no type reaches the body walk
    // Broiler-Human:        PENDING
    private bool TryValidateFunctionTypes()
    {
        sectionIdentifier = (int)WasmSectionId.Function;

        for (var index = 0; index < module.FunctionCount; index++)
        {
            itemOrdinal = index;

            if (module.FunctionTypeIndices[index] >= (uint)module.TypeCount)
            {
                return Fail(WebAssemblyDiagnosticCode.FunctionTypeIndexOutOfRange, 0);
            }
        }

        return true;
    }

    /// <summary>Holds the start function to the function space and to taking and returning nothing.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=3E5D87
    // Broiler-Falsified-If: a start function that takes a parameter or returns a result is accepted
    // Broiler-Human:        PENDING
    private bool TryValidateStartFunction()
    {
        if (module.StartFunctionIndex < 0)
        {
            return true;
        }

        sectionIdentifier = (int)WasmSectionId.Start;
        itemOrdinal = 0;

        if (module.StartFunctionIndex >= module.FunctionCount)
        {
            return Fail(WebAssemblyDiagnosticCode.StartFunctionIndexOutOfRange, 0);
        }

        var declared = module.Types[
            (int)module.FunctionTypeIndices[(int)module.StartFunctionIndex]];

        return declared.ParameterCount == 0 && declared.ResultCount == 0 ||
            Fail(WebAssemblyDiagnosticCode.StartFunctionSignatureInvalid, 0);
    }

    /// <summary>
    /// Holds every export to the index space its kind selects, and refuses two exports of one name.
    /// </summary>
    /// <remarks>
    /// The duplicate check sorts an index vector rather than comparing every pair, because the
    /// export count is guest-controlled and the pairwise comparison a small module makes look free
    /// is quadratic in a number an artifact chooses.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=18FD2F
    // Broiler-Falsified-If: two exports publishing one name are accepted, or the check is quadratic in a guest-chosen count
    // Broiler-Human:        PENDING
    private bool TryValidateExports()
    {
        sectionIdentifier = (int)WasmSectionId.Export;

        var count = module.ExportCount;

        for (var index = 0; index < count; index++)
        {
            itemOrdinal = index;

            var export = module.Exports[index];

            var limit = export.Kind switch
            {
                WasmExportKind.Function => module.FunctionCount,
                WasmExportKind.Table => module.TableCount,
                WasmExportKind.Memory => module.MemoryCount,
                _ => module.GlobalCount,
            };

            if (export.EntityIndex >= (uint)limit)
            {
                return Fail(WebAssemblyDiagnosticCode.ExportIndexOutOfRange, 0);
            }
        }

        if (count < 2)
        {
            return true;
        }

        if (!VmBoundedAllocator.TryAllocate<int>(in adapter.Ceilings, adapter, (uint)count, out var order))
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        for (var index = 0; index < count; index++)
        {
            order[index] = index;
        }

        SortExportOrder(order, count);

        var duplicate = -1;

        for (var index = 1; index < count; index++)
        {
            if (CompareExportNames(order[index - 1], order[index]) == 0)
            {
                duplicate = order[index];
                break;
            }
        }

        adapter.Release((ulong)count * sizeof(int));

        if (duplicate < 0)
        {
            return true;
        }

        itemOrdinal = duplicate;
        return Fail(WebAssemblyDiagnosticCode.DuplicateExportName, 0);
    }

    /// <summary>Orders an index vector by the export names it addresses, in place.</summary>
    /// <remarks>
    /// A heap sort rather than a library call, because the comparison reads a span the module owns
    /// and a comparison delegate cannot close over one from inside a ref struct. It sorts in place,
    /// so it costs the vector already allocated and nothing more.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=640E14
    // Broiler-Human:        PENDING
    private readonly void SortExportOrder(int[] order, int count)
    {
        for (var root = (count / 2) - 1; root >= 0; root--)
        {
            SiftDown(order, root, count);
        }

        for (var end = count - 1; end > 0; end--)
        {
            (order[0], order[end]) = (order[end], order[0]);
            SiftDown(order, 0, end);
        }
    }

    /// <summary>Restores the heap property below one root.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5B36DA
    // Broiler-Human:        PENDING
    private readonly void SiftDown(int[] order, int root, int count)
    {
        while (true)
        {
            var child = (2 * root) + 1;

            if (child >= count)
            {
                return;
            }

            if (child + 1 < count && CompareExportNames(order[child], order[child + 1]) < 0)
            {
                child++;
            }

            if (CompareExportNames(order[root], order[child]) >= 0)
            {
                return;
            }

            (order[root], order[child]) = (order[child], order[root]);
            root = child;
        }
    }

    /// <summary>Orders two exports by their name bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9220CF
    // Broiler-Human:        PENDING
    private readonly int CompareExportNames(int left, int right) =>
        System.MemoryExtensions.SequenceCompareTo(
            module.Exports[left].Name, module.Exports[right].Name);

    /// <summary>Holds every global's initializer to the type the global declares.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=4F956A
    // Broiler-Falsified-If: an initializer whose type differs from the global it initialises is accepted
    // Broiler-Human:        PENDING
    private bool TryValidateGlobals()
    {
        sectionIdentifier = (int)WasmSectionId.Global;

        for (var index = 0; index < module.GlobalCount; index++)
        {
            itemOrdinal = index;

            var global = module.Globals[index];

            if (!TryConstantExpressionType(global.Initializer, out var produced))
            {
                return false;
            }

            if (produced != global.Type.ValueType)
            {
                return Fail(WebAssemblyDiagnosticCode.ConstantExpressionTypeMismatch, 0);
            }
        }

        return true;
    }

    /// <summary>Holds every element segment to the table and function spaces.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=4ECC1D
    // Broiler-Falsified-If: a segment writing a function index that addresses no function is accepted
    // Broiler-Human:        PENDING
    private bool TryValidateElementSegments()
    {
        sectionIdentifier = (int)WasmSectionId.Element;

        for (var index = 0; index < module.ElementSegmentCount; index++)
        {
            itemOrdinal = index;

            var segment = module.Elements[index];

            if (segment.TableIndex >= (uint)module.TableCount)
            {
                return Fail(WebAssemblyDiagnosticCode.ElementSegmentTableIndexOutOfRange, 0);
            }

            if (!TryConstantExpressionType(segment.Offset, out var produced))
            {
                return false;
            }

            if (produced != WasmValueType.I32)
            {
                return Fail(WebAssemblyDiagnosticCode.ConstantExpressionTypeMismatch, 0);
            }

            for (var entry = 0; entry < segment.EntryCount; entry++)
            {
                if (segment.Entries[entry] >= (uint)module.FunctionCount)
                {
                    return Fail(
                        WebAssemblyDiagnosticCode.ElementSegmentFunctionIndexOutOfRange, 0);
                }
            }
        }

        return true;
    }

    /// <summary>Holds every data segment to the memory space and its offset to an i32.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=55202D
    // Broiler-Falsified-If: a segment naming a memory the module does not declare is accepted
    // Broiler-Human:        PENDING
    private bool TryValidateDataSegments()
    {
        sectionIdentifier = (int)WasmSectionId.Data;

        for (var index = 0; index < module.DataSegmentCount; index++)
        {
            itemOrdinal = index;

            var segment = module.Data[index];

            if (segment.MemoryIndex >= (uint)module.MemoryCount)
            {
                return Fail(WebAssemblyDiagnosticCode.DataSegmentMemoryIndexOutOfRange, 0);
            }

            if (!TryConstantExpressionType(segment.Offset, out var produced))
            {
                return false;
            }

            if (produced != WasmValueType.I32)
            {
                return Fail(WebAssemblyDiagnosticCode.ConstantExpressionTypeMismatch, 0);
            }
        }

        return true;
    }

    /// <summary>
    /// The type one constant expression produces.
    /// </summary>
    /// <remarks>
    /// A constant expression may read an IMPORTED global and no other, because a module's own
    /// globals are themselves initialised by constant expressions and one reading another would be
    /// reading a value that does not exist yet. No manifest here admits an import, so every
    /// <c>global.get</c> arriving here names a global that cannot be read, and the answer says that
    /// rather than blaming the type.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=C58248
    // Broiler-Falsified-If: a constant expression reading a module's own global is admitted
    // Broiler-Human:        PENDING
    private bool TryConstantExpressionType(
        WasmConstantExpression expression, out WasmValueType produced)
    {
        produced = WasmValueType.I32;

        switch ((WasmOpcode)expression.Opcode)
        {
            case WasmOpcode.I32Const:
                produced = WasmValueType.I32;
                return true;

            case WasmOpcode.I64Const:
                produced = WasmValueType.I64;
                return true;

            case WasmOpcode.F32Const:
                produced = WasmValueType.F32;
                return true;

            case WasmOpcode.F64Const:
                produced = WasmValueType.F64;
                return true;

            case WasmOpcode.GlobalGet:
                return Fail(WebAssemblyDiagnosticCode.ConstantExpressionGlobalUnavailable, 0);

            default:
                // The decoder admits five opcodes here and no other, so this arm is reachable only
                // through a defect in that decoder rather than through any artifact.
                return Fail(
                    VmReason.MalformedEncoding,
                    WebAssemblyDiagnosticCode.UnsupportedConstantExpressionOpcode,
                    0);
        }
    }

    // =============================================================================================
    // One function body
    // =============================================================================================

    /// <summary>
    /// Walks one function body once, type-checking it and computing what an interpreter will need.
    /// </summary>
    /// <remarks>
    /// The outermost control frame is the function itself: a block whose label types are the
    /// function's results, which is what makes <c>return</c> a branch to the outermost label rather
    /// than a case of its own.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=5; Fingerprint=074C19
    // Broiler-Falsified-If: a body is sealed while the control stack is not empty, or bytes past the closing end are accepted
    // Broiler-Human:        PENDING
    private bool TryValidateBody(int index)
    {
        sectionIdentifier = (int)WasmSectionId.Code;
        itemOrdinal = index;

        var body = module.Bodies[index];
        var declared = module.Types[(int)module.FunctionTypeIndices[index]];

        parameters = declared.Parameters;
        locals = body.Locals;

        typePool[0] = WasmValueType.I32;
        typePool[1] = WasmValueType.I64;
        typePool[2] = WasmValueType.F32;
        typePool[3] = WasmValueType.F64;

        for (var result = 0; result < declared.ResultCount; result++)
        {
            typePool[NumericTypePrefix + result] = declared.Results[result];
        }

        valueCount = 0;
        controlCount = 0;
        jumpCount = 0;
        maxValueHeight = 0;
        maxControlDepth = 0;
        OpenReaderOver(body.Code);

        if (!TryPoll())
        {
            return false;
        }

        if (!PushControl(
            (byte)WasmOpcode.Block,
            default,
            new WasmTypeVector(NumericTypePrefix, declared.ResultCount),
            jumpIndex: -1,
            openerJumpIndex: -1))
        {
            return false;
        }

        while (controlCount > 0)
        {
            if (reader.Remaining == 0)
            {
                return Fail(WebAssemblyDiagnosticCode.UnterminatedFunctionBody, reader.Position);
            }

            if (!TryStep())
            {
                return false;
            }
        }

        if (reader.Remaining != 0)
        {
            return Fail(WebAssemblyDiagnosticCode.InstructionsAfterFunctionEnd, reader.Position);
        }

        return TrySealBody(body);
    }

    /// <summary>
    /// Points the bounded reader at one function body's instruction bytes.
    /// </summary>
    /// <remarks>
    /// <b>The attribute is what lets this member hand the ceilings to the reader at all.</b> A
    /// struct's <c>this</c> is implicitly scoped, so passing one of its own fields as a by-reference
    /// argument to something stored back into the same struct is refused by the ref-safety rules -
    /// not because anything here outlives anything else, but because the rule cannot see that it
    /// does not. Marking the member unscoped states what is true: this validator, the reader it
    /// holds and the ceilings it was built with all live and die inside one verification, and the
    /// reader keeps the ceilings by value in any case.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0947DE
    // Broiler-Falsified-If: the reader it builds outlives the ceilings value it was built from
    // Broiler-Human:        PENDING
    [System.Diagnostics.CodeAnalysis.UnscopedRef]
    private void OpenReaderOver(System.ReadOnlySpan<byte> code) =>
        reader = new VmBoundedReader(code, in adapter.Ceilings, adapter, pollGranularity);

    /// <summary>Copies the resolved jump table onto the body and records the two computed bounds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=29B0E7
    // Broiler-Falsified-If: a body is handed a jump table longer than the entries this walk resolved
    // Broiler-Human:        PENDING
    private bool TrySealBody(WasmFunctionBody body)
    {
        if (!VmBoundedAllocator.TryAllocate<WasmJumpTarget>(
            in adapter.Ceilings, adapter, (uint)jumpCount, out var table))
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        System.Array.Copy(jumps, table, jumpCount);

        // A body is validated once, so a refusal here is this assembly disagreeing with itself
        // rather than anything an artifact did, and it carries the code that says so.
        return body.TrySeal(maxValueHeight, maxControlDepth, table) ||
            Stop(WasmRefusal.Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.VerifierDefect,
                At(0)));
    }

    /// <summary>Reads one instruction and applies its typing rule.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=6; Fingerprint=B028D1
    // Broiler-Falsified-If: an opcode byte reaches a typing rule that is not the one the specification gives it
    // Broiler-Human:        PENDING
    private bool TryStep()
    {
        sinceLastPoll++;

        if (sinceLastPoll >= InstructionsBetweenPolls && !TryPoll())
        {
            return false;
        }

        var at = reader.Position;

        if (!TryReadByte(out var opcode))
        {
            return false;
        }

        switch ((WasmOpcode)opcode)
        {
            case WasmOpcode.Unreachable:
                MarkUnreachable();
                return true;

            case WasmOpcode.Nop:
                return true;

            case WasmOpcode.Block:
            case WasmOpcode.Loop:
            case WasmOpcode.If:
                return TryOpenBlock(opcode, at);

            case WasmOpcode.Else:
                return TryElse(at);

            case WasmOpcode.End:
                return TryEnd();

            case WasmOpcode.Br:
                return TryBranch(unconditional: true);

            case WasmOpcode.BrIf:
                return TryBranch(unconditional: false);

            case WasmOpcode.BrTable:
                return TryBranchTable();

            case WasmOpcode.Return:
                return TryReturn();

            case WasmOpcode.Call:
                return TryCall();

            case WasmOpcode.CallIndirect:
                return TryCallIndirect();

            case WasmOpcode.Drop:
                return PopAny(out _);

            case WasmOpcode.Select:
                return TrySelect();

            case WasmOpcode.LocalGet:
            case WasmOpcode.LocalSet:
            case WasmOpcode.LocalTee:
                return TryLocal(opcode);

            case WasmOpcode.GlobalGet:
            case WasmOpcode.GlobalSet:
                return TryGlobal(opcode);

            case WasmOpcode.MemorySize:
            case WasmOpcode.MemoryGrow:
                return TryMemoryQuery(opcode);

            case WasmOpcode.I32Const:
                return TryReadVarS32(out _) && PushOperand(WasmValueType.I32);

            case WasmOpcode.I64Const:
                return TryReadVarS64(out _) && PushOperand(WasmValueType.I64);

            case WasmOpcode.F32Const:
                return TryReadFixed32() && PushOperand(WasmValueType.F32);

            case WasmOpcode.F64Const:
                return TryReadFixed64() && PushOperand(WasmValueType.F64);

            default:
                return TryMemoryAccess(opcode) ?? TryNumeric(opcode) ?? RefuseOpcode(opcode, at);
        }
    }

    /// <summary>Separates a byte naming a later specification's instruction from one naming none.</summary>
    /// <remarks>
    /// The two are different facts, exactly as an unadmitted value type and an undefined byte are in
    /// the decoder. A module compiled for the bulk-memory, vector, atomic, reference or
    /// sign-extension surfaces is a well-formed module this manifest does not admit; a byte in no
    /// surface at all names no instruction.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=EFA3C4
    // Broiler-Falsified-If: an unadmitted instruction and a byte naming no instruction produce the same diagnostic code
    // Broiler-Human:        PENDING
    private bool RefuseOpcode(byte opcode, ulong at) =>
        opcode is (>= 0xC0 and <= 0xC4) or (>= 0xD0 and <= 0xD2) or 0xFC or 0xFD or 0xFE
            ? Fail(VmReason.UnknownFeature, WebAssemblyDiagnosticCode.OpcodeNotAdmitted, at)
            : Fail(VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.UnknownOpcode, at);

    // =============================================================================================
    // The control instructions
    // =============================================================================================

    /// <summary>Opens a block, a loop or a conditional, and records the entry its end will finish.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=89CB8B
    // Broiler-Falsified-If: a loop's label offset is anywhere but the first instruction of its own body
    // Broiler-Human:        PENDING
    private bool TryOpenBlock(byte opcode, ulong at)
    {
        if (!TryReadBlockType(out var results))
        {
            return false;
        }

        var afterImmediate = (int)reader.Position;

        if ((WasmOpcode)opcode is WasmOpcode.If && !Pop(WasmValueType.I32))
        {
            return false;
        }

        var isLoop = (WasmOpcode)opcode is WasmOpcode.Loop;

        if (!TryAppendJump(
            (int)at,
            labelOffset: isLoop ? afterImmediate : -1,
            labelArity: isLoop ? 0 : results.Count,
            endArity: results.Count,
            out var jumpIndex))
        {
            return false;
        }

        return PushControl(opcode, default, results, jumpIndex, openerJumpIndex: -1);
    }

    /// <summary>Closes the consequent of a conditional and opens its alternative.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=DDE1A2
    // Broiler-Falsified-If: an else where no if is open is accepted, or the if's else offset is not the first byte of this arm
    // Broiler-Human:        PENDING
    private bool TryElse(ulong at)
    {
        if (!PopControl(out var frame))
        {
            return false;
        }

        if ((WasmOpcode)frame.Opcode is not WasmOpcode.If)
        {
            return Fail(WebAssemblyDiagnosticCode.ElseWithoutIf, at);
        }

        if (frame.JumpIndex >= 0)
        {
            jumps[frame.JumpIndex].ElseOffset = (int)reader.Position;
        }

        if (!TryAppendJump(
            (int)at,
            labelOffset: -1,
            labelArity: frame.EndTypes.Count,
            endArity: frame.EndTypes.Count,
            out var jumpIndex))
        {
            return false;
        }

        return PushControl(
            (byte)WasmOpcode.Else, frame.StartTypes, frame.EndTypes, jumpIndex, frame.JumpIndex);
    }

    /// <summary>Closes a block, resolves the offsets waiting for it, and pushes its results.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=005AC3
    // Broiler-Falsified-If: a conditional with a result and no alternative arm is accepted, or an end resolves an entry it did not close
    // Broiler-Human:        PENDING
    private bool TryEnd()
    {
        if (!PopControl(out var frame))
        {
            return false;
        }

        var target = (int)reader.Position;

        if (frame.JumpIndex >= 0)
        {
            jumps[frame.JumpIndex].EndOffset = target;

            if (jumps[frame.JumpIndex].LabelOffset < 0)
            {
                jumps[frame.JumpIndex].LabelOffset = target;
            }
        }

        if (frame.OpenerJumpIndex >= 0)
        {
            jumps[frame.OpenerJumpIndex].EndOffset = target;
            jumps[frame.OpenerJumpIndex].LabelOffset = target;
        }

        // A CONDITIONAL WITH NO ALTERNATIVE ARM PRODUCES WHAT IT CONSUMED AND NOTHING MORE. The
        // frame is still an `if` here precisely because no `else` replaced it, so the arm that never
        // existed would have had to produce the results, and the only block type for which that is
        // satisfiable is the one whose results are its parameters.
        if ((WasmOpcode)frame.Opcode is WasmOpcode.If &&
            !System.MemoryExtensions.SequenceEqual(Types(frame.StartTypes), Types(frame.EndTypes)))
        {
            return Fail(WebAssemblyDiagnosticCode.IfWithoutElseResultMismatch, reader.Position);
        }

        return PushOperands(frame.EndTypes);
    }

    /// <summary>Type-checks a branch, conditional or not.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=398466
    // Broiler-Falsified-If: an unconditional branch does not make the rest of its frame unreachable, or a conditional one does
    // Broiler-Human:        PENDING
    private bool TryBranch(bool unconditional)
    {
        if (!TryReadVarU32(out var depth))
        {
            return false;
        }

        if (!unconditional && !Pop(WasmValueType.I32))
        {
            return false;
        }

        if (depth >= (uint)controlCount)
        {
            return Fail(WebAssemblyDiagnosticCode.BranchDepthOutOfRange, reader.Position);
        }

        var labels = LabelTypes(controls[controlCount - 1 - (int)depth]);

        if (!unconditional)
        {
            return PopThenPush(labels);
        }

        if (!PopOperands(labels))
        {
            return false;
        }

        MarkUnreachable();
        return true;
    }

    /// <summary>Type-checks a branch through a guest-supplied label vector.</summary>
    /// <remarks>
    /// Every label must carry the same number of values as the default one, which is the rule that
    /// makes one jump table implementable at all: arms leaving different numbers of values behind
    /// would have no single stack height to branch to.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=4; Fingerprint=A59028
    // Broiler-Falsified-If: two labels of one table are admitted with different arities, or the label vector is read before its count cleared its ceiling
    // Broiler-Human:        PENDING
    private bool TryBranchTable()
    {
        if (!Pop(WasmValueType.I32) || !TryReadCount(out var count))
        {
            return false;
        }

        if (count == uint.MaxValue)
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact));
        }

        if (!TryGrow(
            ref branchLabels,
            ref branchLabelBytes,
            (int)count + 1,
            adapter.Ceilings.MaxDeclaredCount,
            VmBudgetDimension.DeclaredCount))
        {
            return false;
        }

        for (var index = 0u; index <= count; index++)
        {
            if (!TryReadVarU32(out var label))
            {
                return false;
            }

            branchLabels[index] = label;
        }

        if (branchLabels[count] >= (uint)controlCount)
        {
            return Fail(WebAssemblyDiagnosticCode.BranchDepthOutOfRange, reader.Position);
        }

        var fallback = LabelTypes(controls[controlCount - 1 - (int)branchLabels[count]]);

        for (var index = 0u; index < count; index++)
        {
            if (branchLabels[index] >= (uint)controlCount)
            {
                return Fail(WebAssemblyDiagnosticCode.BranchDepthOutOfRange, reader.Position);
            }

            var labels = LabelTypes(controls[controlCount - 1 - (int)branchLabels[index]]);

            if (labels.Count != fallback.Count)
            {
                return Fail(WebAssemblyDiagnosticCode.BranchTableArityMismatch, reader.Position);
            }

            if (!PopThenPush(labels))
            {
                return false;
            }
        }

        if (!PopOperands(fallback))
        {
            return false;
        }

        MarkUnreachable();
        return true;
    }

    /// <summary>Type-checks a return, which is a branch to the outermost label.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=4E2268
    // Broiler-Falsified-If: it pops anything but the function's own result types
    // Broiler-Human:        PENDING
    private bool TryReturn()
    {
        if (!PopOperands(LabelTypes(controls[0])))
        {
            return false;
        }

        MarkUnreachable();
        return true;
    }

    /// <summary>Type-checks a direct call.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=25372F
    // Broiler-Falsified-If: a call whose index addresses no function is accepted
    // Broiler-Human:        PENDING
    private bool TryCall()
    {
        if (!TryReadVarU32(out var function))
        {
            return false;
        }

        if (function >= (uint)module.FunctionCount)
        {
            return Fail(WebAssemblyDiagnosticCode.FunctionIndexOutOfRange, reader.Position);
        }

        var declared = module.Types[(int)module.FunctionTypeIndices[(int)function]];

        return PopOperandSpan(declared.Parameters) && PushOperandSpan(declared.Results);
    }

    /// <summary>Type-checks a call through a table entry.</summary>
    /// <remarks>
    /// The signature is checked here and the ELEMENT is checked at run time, because which function
    /// a table slot holds is not known until the table has been initialised. That is the one place
    /// this surface leaves a type check to a trap, and the specification puts it there too.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=35A3F9
    // Broiler-Falsified-If: an indirect call is admitted in a module that declares no table
    // Broiler-Human:        PENDING
    private bool TryCallIndirect()
    {
        if (!TryReadVarU32(out var typeIndex) || !TryReadReservedZero())
        {
            return false;
        }

        if (module.TableCount == 0)
        {
            return Fail(WebAssemblyDiagnosticCode.TableNotDeclared, reader.Position);
        }

        if (typeIndex >= (uint)module.TypeCount)
        {
            return Fail(WebAssemblyDiagnosticCode.TypeIndexOutOfRange, reader.Position);
        }

        var declared = module.Types[(int)typeIndex];

        return Pop(WasmValueType.I32) &&
            PopOperandSpan(declared.Parameters) &&
            PushOperandSpan(declared.Results);
    }

    // =============================================================================================
    // The variable, memory and numeric instructions
    // =============================================================================================

    /// <summary>Type-checks a local read, write or write-through.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=46392A
    // Broiler-Falsified-If: an index past the parameters and locals together is accepted
    // Broiler-Human:        PENDING
    private bool TryLocal(byte opcode)
    {
        if (!TryReadVarU32(out var index))
        {
            return false;
        }

        var total = (ulong)parameters.Length + (ulong)locals.Length;

        if (index >= total)
        {
            return Fail(WebAssemblyDiagnosticCode.LocalIndexOutOfRange, reader.Position);
        }

        var type = index < (uint)parameters.Length
            ? parameters[(int)index]
            : locals[(int)(index - (uint)parameters.Length)];

        return (WasmOpcode)opcode switch
        {
            WasmOpcode.LocalGet => PushOperand(type),
            WasmOpcode.LocalSet => Pop(type),
            _ => Pop(type) && PushOperand(type),
        };
    }

    /// <summary>Type-checks a global read or write.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=55CCEC
    // Broiler-Falsified-If: a write to a global the module declared immutable is accepted
    // Broiler-Human:        PENDING
    private bool TryGlobal(byte opcode)
    {
        if (!TryReadVarU32(out var index))
        {
            return false;
        }

        if (index >= (uint)module.GlobalCount)
        {
            return Fail(WebAssemblyDiagnosticCode.GlobalIndexOutOfRange, reader.Position);
        }

        var declared = module.Globals[(int)index].Type;

        if ((WasmOpcode)opcode is WasmOpcode.GlobalGet)
        {
            return PushOperand(declared.ValueType);
        }

        return declared.IsMutable
            ? Pop(declared.ValueType)
            : Fail(WebAssemblyDiagnosticCode.GlobalIsImmutable, reader.Position);
    }

    /// <summary>Type-checks the two instructions that ask a memory about itself.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=378D70
    // Broiler-Falsified-If: either is admitted in a module that declares no memory
    // Broiler-Human:        PENDING
    private bool TryMemoryQuery(byte opcode)
    {
        if (!TryReadReservedZero())
        {
            return false;
        }

        if (module.MemoryCount == 0)
        {
            return Fail(WebAssemblyDiagnosticCode.MemoryNotDeclared, reader.Position);
        }

        return (WasmOpcode)opcode is WasmOpcode.MemoryGrow
            ? Pop(WasmValueType.I32) && PushOperand(WasmValueType.I32)
            : PushOperand(WasmValueType.I32);
    }

    /// <summary>
    /// Type-checks a load or a store, and holds its declared alignment to its access width.
    /// </summary>
    /// <remarks>
    /// <b>AN OVER-ALIGNED ACCESS IS A VALIDATION ERROR AND NOT A TRAP.</b> The alignment immediate
    /// is a hint: it does not change what the access does and an implementation may ignore it
    /// entirely. What it may not do is exceed the natural alignment of the width being accessed,
    /// because the specification bounds it there, and a build that let it through at validation
    /// would have to decide at run time what a two-byte load aligned to eight bytes means - which is
    /// a question with no answer.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=3B8775
    // Broiler-Falsified-If: an alignment above the natural alignment of the access reaches an interpreter, or a store pops its address before its value
    // Broiler-Human:        PENDING
    private bool? TryMemoryAccess(byte opcode)
    {
        if (opcode is < 0x28 or > 0x3E)
        {
            return null;
        }

        var isStore = opcode >= 0x36;

        var natural = opcode switch
        {
            0x28 or 0x2A or 0x34 or 0x35 or 0x36 or 0x38 or 0x3E => 2,
            0x29 or 0x2B or 0x37 or 0x39 => 3,
            0x2C or 0x2D or 0x30 or 0x31 or 0x3A or 0x3C => 0,
            _ => 1,
        };

        var type = opcode switch
        {
            0x28 or 0x2C or 0x2D or 0x2E or 0x2F or 0x36 or 0x3A or 0x3B => WasmValueType.I32,
            0x2A or 0x38 => WasmValueType.F32,
            0x2B or 0x39 => WasmValueType.F64,
            _ => WasmValueType.I64,
        };

        if (!TryReadVarU32(out var align) || !TryReadVarU32(out _))
        {
            return false;
        }

        if (align > (uint)natural)
        {
            return Fail(
                WebAssemblyDiagnosticCode.AlignmentAboveNaturalAlignment, reader.Position);
        }

        if (module.MemoryCount == 0)
        {
            return Fail(WebAssemblyDiagnosticCode.MemoryNotDeclared, reader.Position);
        }

        // The value sits above the address, so a store pops the value first. Reversing the two
        // would accept a module that pushed them the other way round.
        return isStore
            ? Pop(type) && Pop(WasmValueType.I32)
            : Pop(WasmValueType.I32) && PushOperand(type);
    }

    /// <summary>Type-checks the numeric, comparison and conversion instructions.</summary>
    /// <remarks>
    /// They are recognised by range rather than one case per opcode because the binary format lays
    /// them out in runs of one signature: a hundred and forty separate cases saying the same four
    /// things would be a hundred and forty places for one of them to say something else.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=632427
    // Broiler-Falsified-If: an opcode inside one of these runs is given a signature the specification does not give it
    // Broiler-Human:        PENDING
    private bool? TryNumeric(byte opcode)
    {
        if (!TryNumericSignature(opcode, out var operand, out var arity, out var result))
        {
            return null;
        }

        for (var popped = 0; popped < arity; popped++)
        {
            if (!Pop(operand))
            {
                return false;
            }
        }

        return PushOperand(result);
    }

    /// <summary>What one numeric instruction takes and what it produces.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=3; Fingerprint=1801B1
    // Broiler-Falsified-If: a run's boundary here differs from the specification's own opcode table
    // Broiler-Human:        PENDING
    private static bool TryNumericSignature(
        byte opcode, out WasmValueType operand, out int arity, out WasmValueType result)
    {
        operand = WasmValueType.I32;
        arity = 1;
        result = WasmValueType.I32;

        switch (opcode)
        {
            case 0x45:
                return true;

            case >= 0x46 and <= 0x4F:
                arity = 2;
                return true;

            case 0x50:
                operand = WasmValueType.I64;
                return true;

            case >= 0x51 and <= 0x5A:
                operand = WasmValueType.I64;
                arity = 2;
                return true;

            case >= 0x5B and <= 0x60:
                operand = WasmValueType.F32;
                arity = 2;
                return true;

            case >= 0x61 and <= 0x66:
                operand = WasmValueType.F64;
                arity = 2;
                return true;

            case >= 0x67 and <= 0x69:
                return true;

            case >= 0x6A and <= 0x78:
                arity = 2;
                return true;

            case >= 0x79 and <= 0x7B:
                operand = WasmValueType.I64;
                result = WasmValueType.I64;
                return true;

            case >= 0x7C and <= 0x8A:
                operand = WasmValueType.I64;
                arity = 2;
                result = WasmValueType.I64;
                return true;

            case >= 0x8B and <= 0x91:
                operand = WasmValueType.F32;
                result = WasmValueType.F32;
                return true;

            case >= 0x92 and <= 0x98:
                operand = WasmValueType.F32;
                arity = 2;
                result = WasmValueType.F32;
                return true;

            case >= 0x99 and <= 0x9F:
                operand = WasmValueType.F64;
                result = WasmValueType.F64;
                return true;

            case >= 0xA0 and <= 0xA6:
                operand = WasmValueType.F64;
                arity = 2;
                result = WasmValueType.F64;
                return true;

            case 0xA7:
                operand = WasmValueType.I64;
                return true;

            case 0xA8 or 0xA9 or 0xBC:
                operand = WasmValueType.F32;
                return true;

            case 0xAA or 0xAB:
                operand = WasmValueType.F64;
                return true;

            case 0xAC or 0xAD:
                result = WasmValueType.I64;
                return true;

            case 0xAE or 0xAF:
                operand = WasmValueType.F32;
                result = WasmValueType.I64;
                return true;

            case 0xB0 or 0xB1 or 0xBD:
                operand = WasmValueType.F64;
                result = WasmValueType.I64;
                return true;

            case 0xB2 or 0xB3 or 0xBE:
                result = WasmValueType.F32;
                return true;

            case 0xB4 or 0xB5:
                operand = WasmValueType.I64;
                result = WasmValueType.F32;
                return true;

            case 0xB6:
                operand = WasmValueType.F64;
                result = WasmValueType.F32;
                return true;

            case 0xB7 or 0xB8:
                result = WasmValueType.F64;
                return true;

            case 0xB9 or 0xBA or 0xBF:
                operand = WasmValueType.I64;
                result = WasmValueType.F64;
                return true;

            case 0xBB:
                operand = WasmValueType.F32;
                result = WasmValueType.F64;
                return true;

            default:
                return false;
        }
    }

    /// <summary>Type-checks the untyped three-operand choice.</summary>
    /// <remarks>
    /// The two candidates must agree, and the way the specification says so is worth keeping: the
    /// first is popped without an expectation and the second is popped expecting whatever the first
    /// turned out to be, so inside unreachable code - where the first answers the bottom type - the
    /// second is unconstrained too.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=7A8CBE
    // Broiler-Falsified-If: the two candidates are compared as concrete types inside unreachable code
    // Broiler-Human:        PENDING
    private bool TrySelect()
    {
        if (!Pop(WasmValueType.I32) || !PopAny(out var first))
        {
            return false;
        }

        return PopOperand(
                first,
                WebAssemblyDiagnosticCode.OperandStackUnderflow,
                WebAssemblyDiagnosticCode.OperandTypeMismatch,
                out var second) &&
            PushOperand(second);
    }

    // =============================================================================================
    // The specification's own operations
    // =============================================================================================

    /// <summary>Pushes one operand, and keeps the high-water mark the body will be sized from.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=5EFE15
    // Broiler-Falsified-If: the recorded maximum is below a height the stack actually reached
    // Broiler-Human:        PENDING
    private bool PushOperand(WasmValueType type)
    {
        if (!TryGrow(
            ref values,
            ref valueBytes,
            valueCount + 1,
            adapter.Ceilings.MaxDeclaredCount,
            VmBudgetDimension.DeclaredCount))
        {
            return false;
        }

        values[valueCount] = type;
        valueCount++;

        if (valueCount > maxValueHeight)
        {
            maxValueHeight = valueCount;
        }

        return true;
    }

    /// <summary>
    /// Pops one operand, answering the BOTTOM type at an unreachable frame's own height.
    /// </summary>
    /// <remarks>
    /// <b>THIS IS THE ONE THAT MATTERS.</b> At the enclosing frame's height there is nothing left
    /// that belongs to this frame. If the frame is unreachable, the code being checked is code no
    /// execution reaches, and the specification types it polymorphically: the pop answers the bottom
    /// type and whatever asked for an operand is satisfied. If the frame is NOT unreachable, the
    /// same pop is a body reaching below its own block, and it is an error. One comparison separates
    /// a validator that accepts what every toolchain emits from one that accepts what nothing should.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=6991A4
    // Broiler-Falsified-If: a pop at the frame's own height answers the bottom type while the frame is reachable, or answers an error while it is not
    // Broiler-Human:        PENDING
    private bool PopOperand(WebAssemblyDiagnosticCode underflow, out WasmValueType actual)
    {
        actual = BottomType;

        if (valueCount <= controls[controlCount - 1].ValueStackHeight)
        {
            return controls[controlCount - 1].Unreachable || Fail(underflow, reader.Position);
        }

        valueCount--;
        actual = values[valueCount];
        return true;
    }

    /// <summary>Pops one operand and holds it to an expected type, the bottom type satisfying any.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=2; Fingerprint=886224
    // Broiler-Falsified-If: a concrete type is admitted where a different concrete type was expected
    // Broiler-Human:        PENDING
    private bool PopOperand(
        WasmValueType expected,
        WebAssemblyDiagnosticCode underflow,
        WebAssemblyDiagnosticCode mismatch,
        out WasmValueType actual)
    {
        if (!PopOperand(underflow, out actual))
        {
            return false;
        }

        if (actual == BottomType)
        {
            actual = expected;
            return true;
        }

        return expected == BottomType || actual == expected || Fail(mismatch, reader.Position);
    }

    /// <summary>Pops one operand of an expected type, as an ordinary instruction does.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A30053
    // Broiler-Human:        PENDING
    private bool Pop(WasmValueType expected) =>
        PopOperand(
            expected,
            WebAssemblyDiagnosticCode.OperandStackUnderflow,
            WebAssemblyDiagnosticCode.OperandTypeMismatch,
            out _);

    /// <summary>Pops one operand of whatever type is there.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B36478
    // Broiler-Human:        PENDING
    private bool PopAny(out WasmValueType actual) =>
        PopOperand(WebAssemblyDiagnosticCode.OperandStackUnderflow, out actual);

    /// <summary>Pushes a run of operands, in order.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=13A0AB
    // Broiler-Human:        PENDING
    private bool PushOperands(WasmTypeVector types) => PushOperandSpan(Types(types));

    /// <summary>Pushes a run of operands read from anywhere, in order.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=9C8885
    // Broiler-Human:        PENDING
    private bool PushOperandSpan(System.ReadOnlySpan<WasmValueType> types)
    {
        for (var index = 0; index < types.Length; index++)
        {
            if (!PushOperand(types[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Pops a run of operands, last first.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=7A7064
    // Broiler-Falsified-If: the run is popped in declaration order rather than in reverse
    // Broiler-Human:        PENDING
    private bool PopOperands(WasmTypeVector types) => PopOperandSpan(Types(types));

    /// <summary>Pops a run of operands read from anywhere, last first.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=410518
    // Broiler-Falsified-If: the run is popped in declaration order rather than in reverse
    // Broiler-Human:        PENDING
    private bool PopOperandSpan(System.ReadOnlySpan<WasmValueType> types)
    {
        for (var index = types.Length - 1; index >= 0; index--)
        {
            if (!Pop(types[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks a label's types without consuming them, which is what a conditional branch does.
    /// </summary>
    /// <remarks>
    /// The types pushed back are the ones the pops ANSWERED rather than the ones they expected, so a
    /// bottom type popped inside unreachable code goes back as a bottom type rather than being
    /// silently made concrete.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=199BCD
    // Broiler-Falsified-If: it pushes back the expected types rather than the ones the pops answered
    // Broiler-Human:        PENDING
    private bool PopThenPush(WasmTypeVector types)
    {
        var window = Types(types);

        for (var index = window.Length - 1; index >= 0; index--)
        {
            if (!PopOperand(
                window[index],
                WebAssemblyDiagnosticCode.OperandStackUnderflow,
                WebAssemblyDiagnosticCode.OperandTypeMismatch,
                out var actual))
            {
                return false;
            }

            carried[index] = actual;
        }

        for (var index = 0; index < window.Length; index++)
        {
            if (!PushOperand(carried[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Opens a control frame, charging the level of nesting it costs.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=22C66A
    // Broiler-Falsified-If: the frame's height is recorded after its start types were pushed, or a level is opened without being charged
    // Broiler-Human:        PENDING
    private bool PushControl(
        byte opcode,
        WasmTypeVector startTypes,
        WasmTypeVector endTypes,
        int jumpIndex,
        int openerJumpIndex)
    {
        if ((ulong)controlCount >= adapter.Ceilings.MaxStructuralDepth ||
            !adapter.TryChargeStructuralDepth(1))
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact));
        }

        chargedDepth++;

        if (!TryGrow(
            ref controls,
            ref controlBytes,
            controlCount + 1,
            adapter.Ceilings.MaxStructuralDepth,
            VmBudgetDimension.StructuralDepth))
        {
            return false;
        }

        controls[controlCount] = new WasmControlFrame
        {
            Opcode = opcode,
            StartTypes = startTypes,
            EndTypes = endTypes,
            ValueStackHeight = valueCount,
            Unreachable = false,
            JumpIndex = jumpIndex,
            OpenerJumpIndex = openerJumpIndex,
        };

        controlCount++;

        if (controlCount > maxControlDepth)
        {
            maxControlDepth = controlCount;
        }

        return PushOperands(startTypes);
    }

    /// <summary>Closes a control frame, holding it to leaving exactly what it declared.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=3; Fingerprint=9EA35D
    // Broiler-Falsified-If: a frame closes while operands remain above the height it was entered at, or a closed level is not released
    // Broiler-Human:        PENDING
    private bool PopControl(out WasmControlFrame frame)
    {
        frame = default;

        if (controlCount == 0)
        {
            // The body walk stops the moment the last frame closes, so nothing can read an
            // instruction with an empty control stack. Reaching here is this assembly disagreeing
            // with itself.
            return Stop(WasmRefusal.Invalid(
                VmReason.InconsistentStructure,
                WebAssemblyDiagnosticCode.VerifierDefect,
                At(reader.Position)));
        }

        frame = controls[controlCount - 1];

        var window = Types(frame.EndTypes);

        for (var index = window.Length - 1; index >= 0; index--)
        {
            if (!PopOperand(
                window[index],
                WebAssemblyDiagnosticCode.BlockResultTypeUnmet,
                WebAssemblyDiagnosticCode.BlockResultTypeUnmet,
                out _))
            {
                return false;
            }
        }

        if (valueCount != frame.ValueStackHeight)
        {
            return Fail(WebAssemblyDiagnosticCode.BlockLeavesExtraOperands, reader.Position);
        }

        controlCount--;
        adapter.ReleaseStructuralDepth(1);
        chargedDepth--;
        return true;
    }

    /// <summary>
    /// The types a branch to one frame's label carries.
    /// </summary>
    /// <remarks>
    /// A loop's label is its own start, so branching to it means going round again and the types are
    /// the ones the loop is entered with. Every other label is the block's end, so the types are the
    /// ones it leaves. It is one line, and getting it backwards makes every loop with a result type
    /// wrong in a way nothing else notices.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=69C68C
    // Broiler-Falsified-If: a loop's label types are its end types, or any other frame's are its start types
    // Broiler-Human:        PENDING
    private static WasmTypeVector LabelTypes(in WasmControlFrame frame) =>
        (WasmOpcode)frame.Opcode is WasmOpcode.Loop ? frame.StartTypes : frame.EndTypes;

    /// <summary>Truncates to the enclosing frame's height and makes the rest of it polymorphic.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Critical; Resources=1; Fingerprint=C0EE44
    // Broiler-Falsified-If: it truncates to anything but the frame's own recorded height
    // Broiler-Human:        PENDING
    private void MarkUnreachable()
    {
        valueCount = controls[controlCount - 1].ValueStackHeight;
        controls[controlCount - 1].Unreachable = true;
    }

    /// <summary>The run of value types one vector names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=350A7B
    // Broiler-Human:        PENDING
    private readonly System.ReadOnlySpan<WasmValueType> Types(WasmTypeVector vector) =>
        System.MemoryExtensions.AsSpan(typePool, vector.Offset, vector.Count);

    // =============================================================================================
    // Immediates
    // =============================================================================================

    /// <summary>
    /// Reads a block type in the two forms this surface admits, refusing the third.
    /// </summary>
    /// <remarks>
    /// It is decoded as the signed 33-bit integer the format actually uses rather than as a single
    /// byte, because a padded encoding of a value type is legal and a single-byte test would refuse
    /// one. A non-negative value is a type index, which is the multi-value form no manifest here
    /// admits.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=308BB4
    // Broiler-Falsified-If: a padded encoding of a one-byte block type is refused, or a type index is decoded as a value type
    // Broiler-Human:        PENDING
    private bool TryReadBlockType(out WasmTypeVector types)
    {
        types = default;

        var at = reader.Position;

        if (!WasmLeb128.TryReadVarS33(ref reader, out var encoded, out var status))
        {
            return Stop(WasmRefusal.FromVarInt(status, reader.Status, At(at)));
        }

        if (encoded >= 0)
        {
            return Fail(
                VmReason.UnknownFeature, WebAssemblyDiagnosticCode.BlockTypeNotAdmitted, at);
        }

        var tag = (byte)(encoded & 0x7F);

        if (tag == WasmTypeGrammar.EmptyBlockType)
        {
            return true;
        }

        if (WasmTypeGrammar.IsNumericValueType(tag))
        {
            types = new WasmTypeVector(PoolIndexOf((WasmValueType)tag), 1);
            return true;
        }

        return WasmTypeGrammar.IsAnyValueType(tag)
            ? Fail(VmReason.UnknownFeature, WebAssemblyDiagnosticCode.ValueTypeNotAdmitted, at)
            : Fail(VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.UnknownValueType, at);
    }

    /// <summary>Where one numeric type sits in the pool's fixed prefix.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=137C67
    // Broiler-Human:        PENDING
    private static int PoolIndexOf(WasmValueType type) => type switch
    {
        WasmValueType.I32 => 0,
        WasmValueType.I64 => 1,
        WasmValueType.F32 => 2,
        _ => 3,
    };

    /// <summary>Reads an immediate the format reserves as a zero byte.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=B0F2F9
    // Broiler-Falsified-If: a non-zero reserved byte is accepted, so a later revision's meaning is decoded as this one's
    // Broiler-Human:        PENDING
    private bool TryReadReservedZero()
    {
        var at = reader.Position;

        if (!TryReadByte(out var reserved))
        {
            return false;
        }

        return reserved == 0 ||
            Fail(
                VmReason.MalformedEncoding,
                WebAssemblyDiagnosticCode.ReservedImmediateNotZero,
                at);
    }

    /// <summary>Reads one byte of the body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=198CC7
    // Broiler-Human:        PENDING
    private bool TryReadByte(out byte value) =>
        reader.TryReadByte(out value) ||
        Stop(WasmRefusal.FromReader(reader.Status, At(reader.Position)));

    /// <summary>Reads an unsigned 32-bit variable-length immediate.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6CB47F
    // Broiler-Human:        PENDING
    private bool TryReadVarU32(out uint value)
    {
        var at = reader.Position;

        return WasmLeb128.TryReadVarU32(ref reader, out value, out var status) ||
            Stop(WasmRefusal.FromVarInt(status, reader.Status, At(at)));
    }

    /// <summary>Reads a signed 32-bit variable-length immediate.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8C5CC8
    // Broiler-Human:        PENDING
    private bool TryReadVarS32(out int value)
    {
        var at = reader.Position;

        return WasmLeb128.TryReadVarS32(ref reader, out value, out var status) ||
            Stop(WasmRefusal.FromVarInt(status, reader.Status, At(at)));
    }

    /// <summary>Reads a signed 64-bit variable-length immediate.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=15099A
    // Broiler-Human:        PENDING
    private bool TryReadVarS64(out long value)
    {
        var at = reader.Position;

        return WasmLeb128.TryReadVarS64(ref reader, out value, out var status) ||
            Stop(WasmRefusal.FromVarInt(status, reader.Status, At(at)));
    }

    /// <summary>Reads a label-vector length that has cleared the declared-count ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2F7DBE
    // Broiler-Falsified-If: it answers true for a count that has not been both compared and charged
    // Broiler-Human:        PENDING
    private bool TryReadCount(out uint count)
    {
        var at = reader.Position;

        return WasmLeb128.TryReadBoundedCount(ref reader, adapter, out count, out var status) ||
            Stop(WasmRefusal.FromVarInt(status, reader.Status, At(at)));
    }

    /// <summary>Steps over a four-byte floating-point constant.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BD3952
    // Broiler-Human:        PENDING
    private bool TryReadFixed32() =>
        reader.TryReadUInt32LittleEndian(out _) ||
        Stop(WasmRefusal.FromReader(reader.Status, At(reader.Position)));

    /// <summary>Steps over an eight-byte floating-point constant.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0CA7C9
    // Broiler-Human:        PENDING
    private bool TryReadFixed64() =>
        reader.TryReadUInt64LittleEndian(out _) ||
        Stop(WasmRefusal.FromReader(reader.Status, At(reader.Position)));

    // =============================================================================================
    // Scratch, metering and refusal
    // =============================================================================================

    /// <summary>
    /// Sizes the buffers whose size is known before the walk starts.
    /// </summary>
    /// <remarks>
    /// The type pool and the carried-operand buffer are both bounded by the widest result vector any
    /// type in this module declares, which is a number decoding has already held to the
    /// declared-count ceiling. Everything else grows as the walk needs it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D26998
    // Broiler-Falsified-If: a buffer is sized from a count that has not cleared the declared-count ceiling
    // Broiler-Human:        PENDING
    private bool TryReserveScratch()
    {
        var widest = 0;

        for (var index = 0; index < module.TypeCount; index++)
        {
            var declared = module.Types[index];

            if (declared.ResultCount > widest)
            {
                widest = declared.ResultCount;
            }
        }

        return TryGrow(
                ref typePool,
                ref typePoolBytes,
                NumericTypePrefix + widest,
                adapter.Ceilings.MaxDeclaredCount,
                VmBudgetDimension.DeclaredCount) &&
            TryGrow(
                ref carried,
                ref carriedBytes,
                widest + 1,
                adapter.Ceilings.MaxDeclaredCount,
                VmBudgetDimension.DeclaredCount);
    }

    /// <summary>Gives back every scratch reservation and every level of nesting still charged.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=1AEC57
    // Broiler-Falsified-If: a reservation this pass took survives it, or more levels are released than were charged
    // Broiler-Human:        PENDING
    private void ReleaseScratch()
    {
        Give(valueBytes);
        Give(controlBytes);
        Give(jumpBytes);
        Give(typePoolBytes);
        Give(carriedBytes);
        Give(branchLabelBytes);

        valueBytes = 0;
        controlBytes = 0;
        jumpBytes = 0;
        typePoolBytes = 0;
        carriedBytes = 0;
        branchLabelBytes = 0;

        if (chargedDepth > 0)
        {
            adapter.ReleaseStructuralDepth(chargedDepth);
            chargedDepth = 0;
        }
    }

    /// <summary>Gives back one reservation, if there was one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1DD657
    // Broiler-Human:        PENDING
    private readonly void Give(ulong byteCount)
    {
        if (byteCount > 0)
        {
            adapter.Release(byteCount);
        }
    }

    /// <summary>
    /// Grows one scratch buffer, holding what it is being grown to against a named ceiling.
    /// </summary>
    /// <remarks>
    /// The old reservation is given back only after the new one succeeds, so a refused growth leaves
    /// the buffer that already exists both usable and accounted for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=Critical; Resources=3; Fingerprint=A11BA9
    // Broiler-Falsified-If: a buffer grows past the ceiling named for it, or a reservation is released before its replacement is taken
    // Broiler-Human:        PENDING
    private bool TryGrow<TElement>(
        ref TElement[] buffer,
        ref ulong reservedBytes,
        int needed,
        ulong ceiling,
        VmBudgetDimension dimension)
        where TElement : unmanaged
    {
        if (needed <= buffer.Length)
        {
            return true;
        }

        if (needed < 0 || (ulong)needed > ceiling)
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(dimension, VmBudgetScope.Artifact));
        }

        var target = buffer.Length == 0 ? InitialCapacity : buffer.Length;

        while (target < needed)
        {
            target = target >= int.MaxValue / 2 ? needed : target * 2;
        }

        if ((ulong)target > ceiling)
        {
            target = needed;
        }

        if (!VmBoundedAllocator.TryAllocate<TElement>(
            in adapter.Ceilings, adapter, (uint)target, out var grown))
        {
            return Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact));
        }

        System.Array.Copy(buffer, grown, buffer.Length);
        Give(reservedBytes);

        reservedBytes =
            (ulong)target * (ulong)System.Runtime.CompilerServices.Unsafe.SizeOf<TElement>();

        buffer = grown;
        return true;
    }

    /// <summary>Appends one unresolved jump entry and answers where it went.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=C3C9D8
    // Broiler-Falsified-If: an entry is appended at an offset below the one before it, so the table is not sorted
    // Broiler-Human:        PENDING
    private bool TryAppendJump(
        int offset, int labelOffset, int labelArity, int endArity, out int jumpIndex)
    {
        jumpIndex = jumpCount;

        if (!TryGrow(
            ref jumps,
            ref jumpBytes,
            jumpCount + 1,
            adapter.Ceilings.MaxDeclaredCount,
            VmBudgetDimension.DeclaredCount))
        {
            return false;
        }

        jumps[jumpCount] = new WasmJumpTarget
        {
            Offset = offset,
            ElseOffset = -1,
            EndOffset = -1,
            LabelOffset = labelOffset,
            LabelArity = labelArity,
            EndArity = endArity,
        };

        jumpCount++;
        return true;
    }

    /// <summary>
    /// Takes the combined budget and cancellation check on this pass's declared cadence.
    /// </summary>
    /// <remarks>
    /// A refusal is reported as an exhausted verifier-work allowance, which is what the bounded
    /// reader reports for the same three-way ambiguity: the poll folds cancellation, a spent budget
    /// and a wall-clock ceiling into one answer, and the core consults the meter's own latches to
    /// tell a caller which of them it actually was.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=1; Fingerprint=BB7926
    // Broiler-Falsified-If: the work accumulated between two calls of this member can exceed the uncharged-work bound the descriptor declares
    // Broiler-Human:        PENDING
    private bool TryPoll()
    {
        sinceLastPoll = 0;

        return adapter.Poll() ||
            Stop(VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact));
    }

    /// <summary>Records the refusal and answers false, which is the only way this pass stops.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=349598
    // Broiler-Human:        PENDING
    private bool Stop(VmVerifierOutcome outcome)
    {
        refusal = outcome;
        return false;
    }

    /// <summary>Stops with a validation rejection carrying this profile's code and a position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A5591A
    // Broiler-Human:        PENDING
    private bool Fail(WebAssemblyDiagnosticCode code, ulong offset) =>
        Fail(VmReason.SemanticValidationFailed, code, offset);

    /// <summary>Stops with a rejection whose reason is not the ordinary validation one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A034B7
    // Broiler-Human:        PENDING
    private bool Fail(VmReason reason, WebAssemblyDiagnosticCode code, ulong offset) =>
        Stop(WasmRefusal.Invalid(reason, code, At(offset)));

    /// <summary>
    /// The position encoding this pass publishes.
    /// </summary>
    /// <remarks>
    /// <b>The section index is minus one and that is a statement rather than a gap.</b> Validation
    /// runs over a decoded module and not over the section stream, so the ordinal a section had in
    /// the encoding is not something this pass holds. What it does hold is which section the failure
    /// belongs to and which item of that section it is, and those are the two profile coordinates. A
    /// byte offset inside a function body is relative to that body's own instruction bytes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8477A6
    // Broiler-Human:        PENDING
    private readonly VmSourcePosition At(ulong offset) =>
        new(sectionIndex: -1, offset, sectionIdentifier, itemOrdinal);
}
