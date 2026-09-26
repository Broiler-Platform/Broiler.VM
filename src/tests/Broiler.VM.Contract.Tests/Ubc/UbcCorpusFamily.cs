using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>Which of the corpus family's verifier hooks a descriptor is built with.</summary>
/// <remarks>
/// One conforming hook and three that break the hook's contract on purpose. The walk turns a hook
/// answer it cannot trust into its own verdict rather than passing it on, and the only way to show
/// that is to hand it such an answer: a hook that answers with a code in the universal range, one that
/// answers an invalid artifact without an invalid-artifact reason, and one that answers an exhaustion.
/// </remarks>
internal enum UbcCorpusHookMode
{
    /// <summary>The corpus family's own hook: it reads FamilyData and checks constant loads.</summary>
    Standard = 0,

    /// <summary>Refuses the first family instruction with a code of the universal range.</summary>
    UniversalCode = 1,

    /// <summary>Refuses the first family instruction with a reason that is not an invalid-artifact reason.</summary>
    NoInvalidReason = 2,

    /// <summary>Answers the first family instruction with an allocated-bytes exhaustion.</summary>
    Exhausting = 3,
}

/// <summary>
/// The test-only family the universal bytecode's malformed corpus is written against. Nothing of it
/// executes at UBC-1: its handler struct answers <see cref="NotSupportedException"/>, and its executor
/// factory is a stub. What it contributes is data - two tables, a region vocabulary, a trap
/// vocabulary - and a hook, which between them reach every operand shape, both effect forms, all three
/// target forms and every instruction kind.
/// </summary>
/// <remarks>
/// <para>
/// <b>Identity.</b> <c>com.example.ubccorpus</c>, which the core's grammar admits and which lies outside
/// the reserved <c>broiler</c> namespace. The family identity and the profile identity are one string,
/// because <see cref="UbcDescriptors.Build"/> requires the declaration's profile to be the family.
/// </para>
/// <para>
/// <b>Two tables.</b> The base manifest selects table version 1; the wide manifest selects table
/// version 2, which is the base table plus one row. So a Families row can name the right family with
/// the wrong version, or the right version under the wrong manifest, and a row of one table can be
/// used under the other.
/// </para>
/// <para>
/// <b>Not registered.</b> The roadmap says no descriptor is registered in any catalog at UBC-1, and
/// none is: the corpus runner calls the built descriptor's verifier directly.
/// </para>
/// </remarks>
internal static class UbcCorpusFamily
{
    internal const string Identity = "com.example.ubccorpus";

    internal const string BaseManifestText = "com.example.ubccorpus.base";

    internal const string WideManifestText = "com.example.ubccorpus.wide";

    internal const uint BaseTableVersion = 1;

    internal const uint WideTableVersion = 2;

    /// <summary>The bound the family declares on work between two polls, and the walk's granularity.</summary>
    internal const uint MaxUnchargedWork = 256;

    /// <summary>The family code the hook refuses a constant load past the FamilyData pool with.</summary>
    internal const int ConstantPastThePool = 9001;

    /// <summary>The family code the hook refuses a FamilyData tag above <see cref="MaxConstantTag"/> with.</summary>
    internal const int ConstantTagAboveItsBound = 9002;

    /// <summary>The largest constant tag a FamilyData byte may carry.</summary>
    internal const byte MaxConstantTag = 7;

    /// <summary>The region kind whose landing pushes the exception.</summary>
    internal const byte CatchKind = 1;

    /// <summary>The region kind whose landing pushes the exception and a completion word.</summary>
    internal const byte FinallyKind = 2;

    /// <summary>The one family region the region primitives address.</summary>
    internal const byte Memory = 0;

    internal const ushort TrapDivideByZero = 1;

    internal const ushort TrapIntegerOverflow = 2;

    internal const ushort TrapOutOfBounds = 3;

    internal const ushort TrapInvalidConversion = 4;

    internal const ushort TrapUser = 100;

    internal static VmProfileId ProfileId { get; } = VmProfileId.Parse(Identity);

    internal static VmFeatureManifestId BaseManifest { get; } = VmFeatureManifestId.Parse(BaseManifestText);

    internal static VmFeatureManifestId WideManifest { get; } = VmFeatureManifestId.Parse(WideManifestText);

    /// <summary>The family's opcodes. The comment beside each is its operand shape and its effect.</summary>
    internal static class Op
    {
        internal const byte Nop = 0x00;          // none      [] -> []                          dynamic
        internal const byte PushSmall = 0x01;    // u8        [] -> [v]                         dynamic
        internal const byte LoadConstant = 0x02; // u16       [] -> [v]                         dynamic, hook-checked
        internal const byte I32Add = 0x03;       // none      [i32 i32] -> [i32]                primitive
        internal const byte I32DivS = 0x04;      // none      [i32 i32] -> [i32]                primitive, traps
        internal const byte LoadI32 = 0x05;      // u32       [i32] -> [i32]                    region primitive
        internal const byte StoreI64 = 0x06;     // u8u32     [i32 i64] -> []                   region primitive
        internal const byte MemorySize = 0x07;   // none      [] -> [i32]                       region primitive
        internal const byte F64Add = 0x08;       // none      [f64 f64] -> [f64]                primitive
        internal const byte KeepI64 = 0x09;      // none      [i64] -> [i64]                    primitive word.keep
        internal const byte Trunc = 0x0A;        // none      [f64] -> [i32]                    primitive, traps
        internal const byte IfTrue = 0x10;       // u32       [v] -> [], taken []               branch, own pushes
        internal const byte Iterate = 0x11;      // u32       [v] -> [v], taken []              branch, distinct taken pushes
        internal const byte CallValue = 0x12;    // u8        [v, v x n] -> [v]                 call, counted x1
        internal const byte MakeTemplate = 0x13; // u16       [v x 2n] -> [v]                   dynamic, counted x2
        internal const byte Await = 0x14;        // none      [v] -> [v]                        suspend
        internal const byte Throw = 0x15;        // none      [v] -> terminal                   throw
        internal const byte ConstPair = 0x16;    // u16u16    [] -> [v]                         dynamic
        internal const byte PushI32 = 0x17;      // i32       [] -> [i32]                       dynamic
        internal const byte PushI64 = 0x18;      // i64       [] -> [i64]                       dynamic
        internal const byte PushF32 = 0x19;      // f32       [] -> [f32]                       dynamic
        internal const byte PushF64 = 0x1A;      // f64       [] -> [f64]                       dynamic
        internal const byte GetSlot = 0x1B;      // u8u8      [v] -> [v]                        dynamic
        internal const byte ScopeGet = 0x1C;     // u8u16     [] -> [v]                         dynamic
        internal const byte ToI32 = 0x1D;        // none      [v] -> [i32]                      dynamic
        internal const byte FromI32 = 0x1E;      // none      [i32] -> [v]                      dynamic
        internal const byte WideOnly = 0x20;     // none      [] -> []                          dynamic, wide table only
    }

    private static readonly ImmutableArray<UbcSlotType> None = ImmutableArray<UbcSlotType>.Empty;
    private static readonly ImmutableArray<UbcSlotType> V = ImmutableArray.Create(UbcSlotType.V);
    private static readonly ImmutableArray<UbcSlotType> I32 = ImmutableArray.Create(UbcSlotType.I32);
    private static readonly ImmutableArray<UbcSlotType> I64 = ImmutableArray.Create(UbcSlotType.I64);
    private static readonly ImmutableArray<UbcSlotType> F32 = ImmutableArray.Create(UbcSlotType.F32);
    private static readonly ImmutableArray<UbcSlotType> F64 = ImmutableArray.Create(UbcSlotType.F64);

    /// <summary>The rows of table version 1, which the base manifest selects.</summary>
    internal static ImmutableArray<UbcInstructionRow> BaseRows()
    {
        UbcEffect Listed(ImmutableArray<UbcSlotType> pops, ImmutableArray<UbcSlotType> pushes) => UbcEffect.Listed(pops, pushes);

        var i32i32 = ImmutableArray.Create(UbcSlotType.I32, UbcSlotType.I32);

        return
        [
            new(Op.Nop, "corpus.nop", UbcOperandShape.None, Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.PushSmall, "corpus.push_small", UbcOperandShape.U8, Listed(None, V), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.LoadConstant, "corpus.load_constant", UbcOperandShape.U16, Listed(None, V), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.I32Add, "corpus.i32_add", UbcOperandShape.None, Listed(i32i32, I32), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.I32Add),
            new(Op.I32DivS, "corpus.i32_div_s", UbcOperandShape.None, Listed(i32i32, I32), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.I32DivS,
                traps: [new(UbcTrapCode.DivideByZero, TrapDivideByZero), new(UbcTrapCode.IntegerOverflow, TrapIntegerOverflow)]),
            new(Op.LoadI32, "corpus.load_i32", UbcOperandShape.U32, Listed(I32, I32), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.RegionLoadI32, region: Memory,
                traps: [new(UbcTrapCode.OutOfBounds, TrapOutOfBounds)]),
            new(Op.StoreI64, "corpus.store_i64", UbcOperandShape.U8U32, Listed(ImmutableArray.Create(UbcSlotType.I32, UbcSlotType.I64), None),
                UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.RegionStoreI64, region: Memory,
                traps: [new(UbcTrapCode.OutOfBounds, TrapOutOfBounds)]),
            new(Op.MemorySize, "corpus.memory_size", UbcOperandShape.None, Listed(None, I32), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.RegionSize, region: Memory),
            new(Op.F64Add, "corpus.f64_add", UbcOperandShape.None, Listed(ImmutableArray.Create(UbcSlotType.F64, UbcSlotType.F64), F64),
                UbcTarget.None, UbcInstructionKind.Primitive, primitive: UbcPrimitive.F64Add),
            new(Op.KeepI64, "corpus.keep_i64", UbcOperandShape.None, Listed(I64, I64), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.WordKeep),
            new(Op.Trunc, "corpus.trunc", UbcOperandShape.None, Listed(F64, I32), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.I32TruncF64S,
                traps: [new(UbcTrapCode.InvalidConversion, TrapInvalidConversion)]),
            new(Op.IfTrue, "corpus.if_true", UbcOperandShape.U32, Listed(V, None), UbcTarget.Code(), UbcInstructionKind.Branch),
            new(Op.Iterate, "corpus.iterate", UbcOperandShape.U32, Listed(V, V), UbcTarget.Code(None), UbcInstructionKind.Branch),
            new(Op.CallValue, "corpus.call_value", UbcOperandShape.U8, UbcEffect.Counted(V, UbcSlotType.V, 1, V), UbcTarget.None,
                UbcInstructionKind.Call),
            new(Op.MakeTemplate, "corpus.make_template", UbcOperandShape.U16, UbcEffect.Counted(None, UbcSlotType.V, 2, V), UbcTarget.None,
                UbcInstructionKind.Dynamic),
            new(Op.Await, "corpus.await", UbcOperandShape.None, Listed(V, V), UbcTarget.None, UbcInstructionKind.Suspend),
            new(Op.Throw, "corpus.throw", UbcOperandShape.None, Listed(V, None), UbcTarget.None, UbcInstructionKind.Throw, isTerminal: true),
            new(Op.ConstPair, "corpus.const_pair", UbcOperandShape.U16U16, Listed(None, V), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.PushI32, "corpus.push_i32", UbcOperandShape.I32, Listed(None, I32), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.PushI64, "corpus.push_i64", UbcOperandShape.I64, Listed(None, I64), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.PushF32, "corpus.push_f32", UbcOperandShape.F32, Listed(None, F32), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.PushF64, "corpus.push_f64", UbcOperandShape.F64, Listed(None, F64), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.GetSlot, "corpus.get_slot", UbcOperandShape.U8U8, Listed(V, V), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.ScopeGet, "corpus.scope_get", UbcOperandShape.U8U16, Listed(None, V), UbcTarget.None, UbcInstructionKind.Dynamic),
            new(Op.ToI32, "corpus.to_i32", UbcOperandShape.None, Listed(V, I32), UbcTarget.None, UbcInstructionKind.Dynamic, cost: 3),
            new(Op.FromI32, "corpus.from_i32", UbcOperandShape.None, Listed(I32, V), UbcTarget.None, UbcInstructionKind.Dynamic),
        ];
    }

    /// <summary>The rows of table version 2: the base rows and one more.</summary>
    internal static ImmutableArray<UbcInstructionRow> WideRows() =>
        BaseRows().Add(new UbcInstructionRow(
            Op.WideOnly, "corpus.wide_only", UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic));

    /// <summary>The region kinds: a catch landing pushes the exception, a finally landing the exception and a word.</summary>
    internal static ImmutableArray<UbcRegionKindRow> RegionKinds() =>
    [
        new(CatchKind, "catch", V),
        new(FinallyKind, "finally", ImmutableArray.Create(UbcSlotType.V, UbcSlotType.I32)),
    ];

    internal static ImmutableArray<UbcRegionDeclaration> Regions() => [new(Memory, "memory0")];

    internal static ImmutableArray<UbcFamilyTrap> Traps() =>
    [
        new(TrapDivideByZero, "divide-by-zero"),
        new(TrapIntegerOverflow, "integer-overflow"),
        new(TrapOutOfBounds, "out-of-bounds"),
        new(TrapInvalidConversion, "invalid-conversion"),
        new(TrapUser, "user"),
    ];

    /// <summary>Both tables, built through the schema check; a defect here is the corpus author's and throws.</summary>
    internal static ImmutableArray<UbcInstructionTable> Tables() =>
    [
        Table(BaseManifest, BaseTableVersion, BaseRows(), canonicaliseNaN: true),
        Table(WideManifest, WideTableVersion, WideRows(), canonicaliseNaN: false),
    ];

    /// <summary>The table the base manifest selects.</summary>
    internal static UbcInstructionTable BaseTable() => Tables()[0];

    private static UbcInstructionTable Table(
        VmFeatureManifestId manifest,
        uint version,
        ImmutableArray<UbcInstructionRow> rows,
        bool canonicaliseNaN)
    {
        if (!UbcInstructionTable.TryCreate(Identity, version, manifest, rows, RegionKinds(), Regions(), Traps(), canonicaliseNaN,
                out var table, out var defect))
        {
            throw new InvalidOperationException($"the corpus family's table for {manifest} is refused: {defect}");
        }

        return table!;
    }

    /// <summary>The hook of one mode.</summary>
    internal static IUbcFamilyVerifier Hook(UbcCorpusHookMode mode) => mode switch
    {
        UbcCorpusHookMode.Standard => new UbcCorpusHook(),
        _ => new UbcCorpusMisbehavingHook(mode),
    };

    /// <summary>The family's registration, with the hook of <paramref name="mode"/>.</summary>
    internal static UbcFamilyRegistration<UbcCorpusHandlers> Registration(
        UbcCorpusHookMode mode = UbcCorpusHookMode.Standard,
        int authoredUbcContractVersion = UbcContract.Version,
        int builtAgainstUbcContractVersion = UbcContract.Version) =>
        new(Identity, Tables(), Hook(mode), authoredUbcContractVersion, builtAgainstUbcContractVersion);

    /// <summary>Descriptor rows 1 to 3 and 8 to 30, which the family owns.</summary>
    internal static UbcFamilyDeclaration Declaration(VmProfileId? profileId = null, uint maxUnchargedWork = MaxUnchargedWork)
    {
        var owner = profileId ?? ProfileId;
        VmDiagnosticsIdentity.TryCreate(owner, owner + ".diagnostics", out var diagnostics);

        return new UbcFamilyDeclaration(
            profileId: owner,
            displayName: "UBC corpus family",
            descriptorRevision: 3,
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: maxUnchargedWork,
            abandonBudget: 777,
            limitDefaults: Vector(Defaults),
            profileHardMaxima: Vector(Maxima),
            budgetDeclarationMatrix: Matrix(),
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(900, 999),
            authoredCoreContractVersion: VmCoreContract.Version,
            conformanceManifestId: VmConformanceManifestId.Create(owner + ".conformance"),
            conformanceManifestVersion: 2,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity("Broiler.VM.Contract.Tests", "0.1.0-preview.1", "broiler-vm-core-tests"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: maxUnchargedWork,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    // The fixture profile's vectors, stated here rather than borrowed from Broiler.VM.Fixtures: this file
    // is also compiled into the universal bytecode's fixture composition, which replays the corpus in
    // every publish mode and may not link a test fixture. The corpus's recorded answers bind the two
    // copies of nothing, because there is one copy.
    private static readonly ulong[] Defaults =
    [
        1_000_000, 30_000, 8 * 1024 * 1024, 10_000, 8, 64 * 1024, 1_000_000, 8 * 1024 * 1024, 256, 4,
        1024 * 1024, 64, 65_536, 16, 64,
    ];

    private static readonly ulong[] Maxima =
    [
        100_000_000, 300_000, 64L * 1024 * 1024, 1_000_000, 64, 1024 * 1024, 100_000_000, 64L * 1024 * 1024,
        4096, 16, 16L * 1024 * 1024, 1024, 1_048_576, 64, ulong.MaxValue,
    ];

    private static VmLimitVector Vector(ulong[] byDimension)
    {
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = byDimension[(int)dimension];
        }

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];
        Array.Fill(rows, VmBudgetApplicability.Charged);
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;
        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }

    /// <summary>The emitter set: the bytecode form, whose executor factory is a stub.</summary>
    internal static UbcEmitterSet Forms(UbcCorpusExecutorFactory? factory = null) =>
        UbcEmitterSet.Create(new UbcForm(UbcFormat.BytecodeForm, 1, factory ?? new UbcCorpusExecutorFactory()));

    /// <summary>The descriptor the corpus is verified by, built and never registered.</summary>
    internal static VmProfileDescriptor Descriptor(
        UbcCorpusHookMode mode = UbcCorpusHookMode.Standard,
        uint maxUnchargedWork = MaxUnchargedWork) =>
        UbcDescriptors.Build(Registration(mode), Declaration(maxUnchargedWork: maxUnchargedWork), Forms());
}

/// <summary>
/// The corpus family's handler struct. Nothing executes at UBC-1, so every member answers
/// <see cref="NotSupportedException"/>; the struct exists because a registration is generic over one.
/// </summary>
internal struct UbcCorpusHandlers : IUbcFamily
{
    public static UbcStatus Handle(ref UbcActivation activation, byte familyOpcode, ulong operand) => throw Idle();

    public static void OnLand(ref UbcActivation activation, byte regionKind) => throw Idle();

    public static void OnResume(ref UbcActivation activation, object reason) => throw Idle();

    public static IUbcValuePlane CreateValuePlane(int capacity) => throw Idle();

    public static object CreateInstance(UbcInstanceContext context) => throw Idle();

    public static bool BindParameters(ref UbcActivation activation, ReadOnlySpan<byte> entryName) => throw Idle();

    public static IVmProfilePayload? Completion(ref UbcActivation activation) => throw Idle();

    public static IVmProfilePayload Fault(ref UbcActivation activation, byte familySlot, ushort code) => throw Idle();

    public static IVmProfilePayload? Uncaught(ref UbcActivation activation) => throw Idle();

    public static IVmProfilePayload? SuspendProjection(ref UbcActivation activation) => throw Idle();

    public static IVmProfilePayload? EntryRefused(object instanceState, ReadOnlySpan<byte> entryName) => throw Idle();

    public static object CaptureValues(IUbcValuePlane plane, int start, int count) => throw Idle();

    public static void RestoreValues(IUbcValuePlane plane, int start, object captured) => throw Idle();

    private static NotSupportedException Idle() =>
        new("the universal bytecode corpus family executes nothing at UBC-1");
}

/// <summary>
/// The executor factory of the corpus family's one form. It records the type argument it was asked
/// for, so a test can show that row 7 reaches it specialised for the family, and then refuses.
/// </summary>
internal sealed class UbcCorpusExecutorFactory : IUbcExecutorFactory
{
    internal Type? RequestedFamily { get; private set; }

    internal UbcFamilyDeclaration? RequestedDeclaration { get; private set; }

    public IVmProfileExecutor Create<TFamily>(
        UbcFamilyRegistration<TFamily> family,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment)
        where TFamily : struct, IUbcFamily
    {
        RequestedFamily = typeof(TFamily);
        RequestedDeclaration = declaration;
        throw new NotSupportedException("the universal bytecode corpus family has no executor at UBC-1");
    }
}

/// <summary>What the corpus hook keeps from a FamilyData section: how many constants the pool holds.</summary>
internal sealed class UbcCorpusFamilyState
{
    internal UbcCorpusFamilyState(int constants) => Constants = constants;

    internal int Constants { get; }
}

/// <summary>
/// The corpus family's hook. The FamilyData section is a constant pool of one tag byte per constant,
/// each at most <see cref="UbcCorpusFamily.MaxConstantTag"/>; <c>corpus.load_constant</c> must name a
/// constant the pool holds. The first is refused with 9002, the second with 9001 - codes of the
/// family's own, outside the universal range.
/// </summary>
internal sealed class UbcCorpusHook : IUbcFamilyVerifier
{
    public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
    {
        familyState = null;
        var data = artifact.FamilyData.Span;

        // The hook's own work goes through the meter it is handed, which counts it with the walk's.
        if (!artifact.Meter.TryCharge(VmBudgetDimension.VerifierWork, (ulong)data.Length + 1))
        {
            return UbcHookAnswer.Exhaust(VmBudgetDimension.VerifierWork);
        }

        foreach (var tag in data)
        {
            if (tag > UbcCorpusFamily.MaxConstantTag)
            {
                return UbcHookAnswer.Refuse(UbcCorpusFamily.ConstantTagAboveItsBound, VmReason.InconsistentStructure);
            }
        }

        familyState = new UbcCorpusFamilyState(data.Length);
        return UbcHookAnswer.Admit;
    }

    public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction)
    {
        var constants = (familyState as UbcCorpusFamilyState)?.Constants ?? 0;

        return instruction.Opcode == UbcCorpusFamily.Op.LoadConstant && instruction.Operand >= (ulong)constants
            ? UbcHookAnswer.Refuse(UbcCorpusFamily.ConstantPastThePool, VmReason.SemanticValidationFailed)
            : UbcHookAnswer.Admit;
    }

    public UbcHookAnswer End(object? familyState) => UbcHookAnswer.Admit;
}

/// <summary>A hook that breaks the hook contract at the first family instruction, in the way its mode names.</summary>
internal sealed class UbcCorpusMisbehavingHook : IUbcFamilyVerifier
{
    private readonly UbcCorpusHookMode mode;

    internal UbcCorpusMisbehavingHook(UbcCorpusHookMode mode) => this.mode = mode;

    public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
    {
        familyState = null;
        return UbcHookAnswer.Admit;
    }

    public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction) => mode switch
    {
        UbcCorpusHookMode.UniversalCode => UbcHookAnswer.Refuse((int)UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure),
        UbcCorpusHookMode.NoInvalidReason => UbcHookAnswer.Refuse(9005, VmReason.NormalCompleted),
        UbcCorpusHookMode.Exhausting => UbcHookAnswer.Exhaust(VmBudgetDimension.AllocatedBytes),
        _ => UbcHookAnswer.Admit,
    };

    public UbcHookAnswer End(object? familyState) => UbcHookAnswer.Admit;
}
