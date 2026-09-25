using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// The probe family: a second universal bytecode family, compiled into this root alone, that overrides
/// every member a family may override and carries a call row of the signature effect form, so that the
/// bytecode emitter's paths the fixture family never takes are run inside the published image.
/// </summary>
/// <remarks>
/// <para>
/// The fixture family keeps every default - entry points from the Entries section, every instance
/// ready, no start unit, nothing to abandon - and its checks show the defaults behave as before. This
/// family is the other half: its entry resolution answers from the entry point's name, its admission
/// and start unit from its FamilyData, and it counts every state it makes and every one it is told to
/// abandon, so a check can hold the executor to abandoning each state that is not published exactly
/// once.
/// </para>
/// <para>
/// It is composed only into the runtimes the probe checks build, the way the malformed corpus family is
/// compiled into the ubc1-corpus mode, and is advertised by nothing: the composition's closure names the
/// fixture family and the ledger.
/// </para>
/// </remarks>
internal static class ProbeProfile
{
    /// <summary>The family's identity, which is also its profile identity.</summary>
    internal const string Identity = "com.example.ubcprobe";

    /// <summary>Its one feature manifest.</summary>
    internal const string ManifestIdentity = "com.example.ubcprobe.base";

    /// <summary>The family slot every probe artifact puts it in.</summary>
    internal const byte Slot = 1;

    /// <summary><c>probe.push</c>: an i64 of the operand, sign-extended. <c>I32</c>, <c>[] -&gt; [i64]</c>.</summary>
    internal const byte Push = 0x01;

    /// <summary><c>probe.box</c>: a value holding an i64. <c>[i64] -&gt; [v]</c>.</summary>
    internal const byte Box = 0x02;

    /// <summary><c>probe.unbox</c>: the i64 a value holds. <c>[v] -&gt; [i64]</c>.</summary>
    internal const byte Unbox = 0x03;

    /// <summary>
    /// <c>probe.call_signature</c>: a call whose callee the top i32 names, of the signature the operand
    /// names, with a value between the parameters and the i32. <c>U32</c>, signature form, trailing
    /// <c>[v i32]</c>.
    /// </summary>
    internal const byte CallSignature = 0x04;

    /// <summary><c>probe.record</c>: adds an i64 to the instance's record. <c>[i64] -&gt; []</c>.</summary>
    internal const byte Record = 0x05;

    /// <summary><c>probe.read</c>: the instance's record. <c>[] -&gt; [i64]</c>.</summary>
    internal const byte Read = 0x06;

    /// <summary><c>probe.crash</c>: a handler that throws. <c>[] -&gt; []</c>.</summary>
    internal const byte Crash = 0x07;

    /// <summary><c>probe.yield</c>: suspends. <c>[] -&gt; []</c>.</summary>
    internal const byte Yield = 0x08;

    /// <summary><c>probe.i64.add</c>: the primitive <c>i64.add</c>.</summary>
    internal const byte AddWords = 0x09;

    /// <summary><c>probe.i64.mul</c>: the primitive <c>i64.mul</c>.</summary>
    internal const byte MulWords = 0x0A;

    /// <summary>The one trap code of the vocabulary, which a <c>trap 1 1</c> names.</summary>
    internal const ushort TrapProbe = 1;

    /// <summary>The hook's one code: FamilyData that is not two bytes.</summary>
    internal const int FamilyDataMalformed = 8001;

    /// <summary>The payload kinds: a completion and a fault.</summary>
    internal const int ResultKindId = 801;

    /// <inheritdoc cref="ResultKindId"/>
    internal const int FaultKindId = 802;

    internal static VmProfileId Id { get; } = VmProfileId.Parse(Identity);

    internal static VmFeatureManifestId Manifest { get; } = VmFeatureManifestId.Parse(ManifestIdentity);

    /// <summary>The table its manifest selects.</summary>
    internal static UbcInstructionTable Table { get; } = BuildTable();

    /// <summary>The registration, written for and compiled against this build's contract version.</summary>
    internal static UbcFamilyRegistration<ProbeFamily> Registration { get; } =
        new(Identity, [Table], new ProbeVerifier(), authoredUbcContractVersion: 2);

    /// <summary>The descriptor over the bytecode emitter, as this root builds every family's.</summary>
    internal static VmProfileDescriptor Descriptor { get; } =
        UbcDescriptors.Build(Registration, Declaration(), UbcEmitterSet.Create(UbcBytecodeEmitter.Form));

    /// <summary>The artifact descriptor a probe artifact is verified under.</summary>
    internal static VmArtifactDescriptor ArtifactDescriptor() =>
        new(Id, UbcFormat.FormatVersion, Manifest, default, VmCallerIdentity.FromCanonicalIdentity("composition-ubc-fixture://probe"));

    /// <summary>The limits a probe runtime states: the fixture family's defaults.</summary>
    internal static ulong[] Limits() => FixtureHost.Vector(Com.Example.Tally.TallyProfile.Defaults());

    private static UbcFamilyDeclaration Declaration()
    {
        VmDiagnosticsIdentity.TryCreate(Id, Identity + ".diagnostics", out var diagnostics);
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];
        Array.Fill(rows, VmBudgetApplicability.Charged);
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;
        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);

        return new UbcFamilyDeclaration(
            profileId: Id,
            displayName: "Example UBC probe",
            descriptorRevision: 1,
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Com.Example.Tally.TallyProfile.Defaults(),
            profileHardMaxima: Com.Example.Tally.TallyProfile.Maxima(),
            budgetDeclarationMatrix: matrix,
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(800, 899),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create(Identity + ".conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity("Broiler.VM.Composition.Ubc.Fixture", "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    private static UbcInstructionTable BuildTable()
    {
        var none = ImmutableArray<UbcSlotType>.Empty;
        var i64 = ImmutableArray.Create(UbcSlotType.I64);
        var i64i64 = ImmutableArray.Create(UbcSlotType.I64, UbcSlotType.I64);
        var v = ImmutableArray.Create(UbcSlotType.V);

        UbcInstructionRow Dynamic(byte opcode, string mnemonic, UbcOperandShape shape, ImmutableArray<UbcSlotType> pops, ImmutableArray<UbcSlotType> pushes) =>
            new(opcode, mnemonic, shape, UbcEffect.Listed(pops, pushes), UbcTarget.None, UbcInstructionKind.Dynamic);

        var rows = new[]
        {
            Dynamic(Push, "probe.push", UbcOperandShape.I32, none, i64),
            Dynamic(Box, "probe.box", UbcOperandShape.None, i64, v),
            Dynamic(Unbox, "probe.unbox", UbcOperandShape.None, v, i64),
            new UbcInstructionRow(
                CallSignature, "probe.call_signature", UbcOperandShape.U32,
                UbcEffect.Signature(ImmutableArray.Create(UbcSlotType.V, UbcSlotType.I32)), UbcTarget.None, UbcInstructionKind.Call),
            Dynamic(Record, "probe.record", UbcOperandShape.None, i64, none),
            Dynamic(Read, "probe.read", UbcOperandShape.None, none, i64),
            Dynamic(Crash, "probe.crash", UbcOperandShape.None, none, none),
            new UbcInstructionRow(Yield, "probe.yield", UbcOperandShape.None, UbcEffect.Listed(none, none), UbcTarget.None, UbcInstructionKind.Suspend),
            new UbcInstructionRow(
                AddWords, "probe.i64.add", UbcOperandShape.None, UbcEffect.Listed(i64i64, i64), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.I64Add),
            new UbcInstructionRow(
                MulWords, "probe.i64.mul", UbcOperandShape.None, UbcEffect.Listed(i64i64, i64), UbcTarget.None, UbcInstructionKind.Primitive,
                primitive: UbcPrimitive.I64Mul),
        };

        var created = UbcInstructionTable.TryCreate(
            Identity, 1, Manifest, rows, [], [], [new UbcFamilyTrap(TrapProbe, "probe")], canonicaliseNaN: false, out var table, out var defect);

        return created ? table! : throw new InvalidOperationException(defect);
    }
}

/// <summary>What a probe instance's making does, as the first FamilyData byte names it.</summary>
internal enum ProbeAdmission : byte
{
    /// <summary>The instance is ready.</summary>
    Ready = 0,

    /// <summary>Admission answers a fault of the family's.</summary>
    Faulted = 1,

    /// <summary>Making the instance charges more allocated bytes than any ceiling admits, and admission answers exhausted.</summary>
    Exhausted = 2,

    /// <summary>Admission answers a default value, which has no kind.</summary>
    Unanswered = 3,

    /// <summary>Admission throws.</summary>
    AdmissionThrows = 4,

    /// <summary>Making the instance throws, so there is no state to abandon.</summary>
    CreationThrows = 5,

    /// <summary>Admission answers a fault, and abandoning the state throws.</summary>
    AbandonThrows = 6,
}

/// <summary>What the hook read from a probe artifact's FamilyData: how its instance is admitted, and its start unit.</summary>
internal sealed class ProbeDefinitions
{
    internal ProbeDefinitions(ProbeAdmission admission, int start)
    {
        Admission = admission;
        Start = start;
    }

    internal ProbeAdmission Admission { get; }

    /// <summary>The start unit, or minus one for none.</summary>
    internal int Start { get; }
}

/// <summary>
/// Every probe state made and every abandonment asked for, since the last <see cref="Reset"/>: what the
/// checks hold the executor to. The checks run one after another, so plain counters suffice.
/// </summary>
internal static class ProbeLog
{
    internal static int Created { get; private set; }

    internal static int Abandoned { get; private set; }

    /// <summary>States abandoned more than once: always zero when the executor keeps its promise.</summary>
    internal static int AbandonedTwice { get; private set; }

    internal static void Reset() => (Created, Abandoned, AbandonedTwice) = (0, 0, 0);

    internal static void Made() => Created++;

    internal static void Abandon(ProbeInstance instance)
    {
        if (instance.Abandoned)
        {
            AbandonedTwice++;
        }

        instance.Abandoned = true;
        Abandoned++;
    }
}

/// <summary>A probe instance: its definitions, the environment it reports retention through, and its record.</summary>
internal sealed class ProbeInstance
{
    /// <summary>The live bytes every probe instance retains while it stands, released when it is abandoned.</summary>
    internal const ulong Retained = 1024;

    internal ProbeInstance(ProbeDefinitions definitions, IVmExecutionEnvironment environment)
    {
        Definitions = definitions;
        Environment = environment;
    }

    internal ProbeDefinitions Definitions { get; }

    /// <summary>Read at each use: its meter is the ambient one of the step in progress.</summary>
    internal IVmExecutionEnvironment Environment { get; }

    internal bool Refused { get; set; }

    internal bool Abandoned { get; set; }

    internal long Record { get; set; }
}

/// <summary>The probe family's value plane: an array of boxed i64 amounts.</summary>
internal sealed class ProbePlane(int capacity) : IUbcValuePlane
{
    private long[] slots = new long[capacity];

    public int Capacity => slots.Length;

    public int ValueBytes => sizeof(long);

    internal long this[int index]
    {
        get => slots[index];
        set => slots[index] = value;
    }

    internal long[] Slice(int start, int count) => slots.AsSpan(start, count).ToArray();

    internal void Write(int start, long[] values) => values.CopyTo(slots.AsSpan(start));

    public void Resize(int capacity)
    {
        if (capacity > slots.Length)
        {
            Array.Resize(ref slots, capacity);
        }
    }

    public void Copy(int from, int to) => slots[to] = slots[from];

    public void Clear(int start, int count) => Array.Clear(slots, start, count);
}

/// <summary>A probe completion: the entry unit's word results and value results, in order.</summary>
internal sealed class ProbeResult(ImmutableArray<long> words, ImmutableArray<long> values) : IVmProfilePayload
{
    public VmPayloadIdentity Identity { get; } = new(ProbeProfile.Id, ProbeProfile.ResultKindId, 1);

    public override string ToString() =>
        $"completed words=[{string.Join(",", words)}] values=[{string.Join(",", values)}]";
}

/// <summary>A probe fault: what raised it, and a trap's code.</summary>
internal sealed class ProbeFault(string kind, ushort code) : IVmProfilePayload
{
    public VmPayloadIdentity Identity { get; } = new(ProbeProfile.Id, ProbeProfile.FaultKindId, 1);

    public override string ToString() => $"faulted {kind} code={code.ToString(CultureInfo.InvariantCulture)}";
}

/// <summary>The probe family's hook: FamilyData is empty, or two bytes - the admission and the start unit.</summary>
internal sealed class ProbeVerifier : IUbcFamilyVerifier
{
    public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
    {
        var data = artifact.FamilyData.Span;
        familyState = null;

        if (data.Length == 0)
        {
            familyState = new ProbeDefinitions(ProbeAdmission.Ready, -1);
            return UbcHookAnswer.Admit;
        }

        if (data.Length != 2)
        {
            return UbcHookAnswer.Refuse(ProbeProfile.FamilyDataMalformed, VmReason.InconsistentStructure);
        }

        // 0xFF names no start unit, 0xFE a unit no probe artifact has, and anything else a unit.
        var start = data[1] switch
        {
            0xFF => -1,
            0xFE => 200,
            var unit => (int)unit,
        };

        familyState = new ProbeDefinitions((ProbeAdmission)data[0], start);
        return UbcHookAnswer.Admit;
    }

    public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction) => UbcHookAnswer.Admit;

    public UbcHookAnswer End(object? familyState) => UbcHookAnswer.Admit;
}

/// <summary>
/// The probe family's handlers, and its overrides of the four members every other family here leaves
/// to their defaults.
/// </summary>
internal struct ProbeFamily : IUbcFamily
{
    public static UbcStatus Handle(ref UbcActivation activation, byte familyOpcode, ulong operand)
    {
        var plane = (ProbePlane)activation.Values;
        var words = activation.Words;
        var word = activation.WordArgs;
        var value = activation.ValueArgs;
        var state = (ProbeInstance)activation.InstanceState!;

        switch (familyOpcode)
        {
            case ProbeProfile.Push:
                words[word] = unchecked((ulong)(long)(int)(uint)operand);
                return UbcStatus.Next;

            case ProbeProfile.Box:
                plane[value] = unchecked((long)words[word]);
                return UbcStatus.Next;

            case ProbeProfile.Unbox:
                words[word] = unchecked((ulong)plane[value]);
                return UbcStatus.Next;

            case ProbeProfile.CallSignature:
            {
                // The named signature's word parameters are the bottom of the word inputs, and the
                // trailing i32 - the only trailing word - lies above them.
                var named = activation.Program.Artifact.Types[(int)operand];
                var parameterWords = 0;

                foreach (var type in named.Parameters)
                {
                    parameterWords += type == UbcSlotType.V ? 0 : 1;
                }

                activation.CallRequest = new UbcCallRequest((int)(uint)words[word + parameterWords]);
                return UbcStatus.Request;
            }

            case ProbeProfile.Record:
                state.Record += unchecked((long)words[word]);
                return UbcStatus.Next;

            case ProbeProfile.Read:
                words[word] = unchecked((ulong)state.Record);
                return UbcStatus.Next;

            case ProbeProfile.Crash:
                throw new InvalidOperationException("the probe's crash row ran");

            case ProbeProfile.Yield:
                activation.Pending = "yielded";
                return UbcStatus.Suspend;

            case ProbeProfile.AddWords:
            case ProbeProfile.MulWords:
            {
                ProbeProfile.Table.TryGetRow(familyOpcode, out var row);
                words[word] = UbcPrimitives.Evaluate(row.Primitive!.Value, words[word], words[word + 1], false).Bits;
                return UbcStatus.Next;
            }

            default:
                return UbcStatus.Defect;
        }
    }

    public static void OnLand(ref UbcActivation activation, byte regionKind)
    {
    }

    public static void OnResume(ref UbcActivation activation, object reason)
    {
    }

    public static IUbcValuePlane CreateValuePlane(int capacity) => new ProbePlane(capacity);

    public static object CreateInstance(UbcInstanceContext context)
    {
        var definitions = context.Program.FamilyState as ProbeDefinitions ?? new ProbeDefinitions(ProbeAdmission.Ready, -1);

        if (definitions.Admission == ProbeAdmission.CreationThrows)
        {
            throw new InvalidOperationException("the probe was told to fail making its instance");
        }

        var instance = new ProbeInstance(definitions, context.Environment);
        ProbeLog.Made();
        instance.Environment.Meter.ReportRetained(VmBudgetDimension.LiveBytes, ProbeInstance.Retained);

        if (definitions.Admission == ProbeAdmission.Exhausted)
        {
            instance.Refused = !instance.Environment.Meter.TryCharge(VmBudgetDimension.AllocatedBytes, 1UL << 62);
        }

        return instance;
    }

    public static UbcInstanceAnswer AdmitInstance(object instanceState)
    {
        var instance = (ProbeInstance)instanceState;

        return instance.Definitions.Admission switch
        {
            ProbeAdmission.Faulted or ProbeAdmission.AbandonThrows => UbcInstanceAnswer.Faulted(new ProbeFault("Admission", 0)),
            ProbeAdmission.Exhausted => instance.Refused ? UbcInstanceAnswer.Exhausted : UbcInstanceAnswer.Ready,
            ProbeAdmission.Unanswered => default,
            ProbeAdmission.AdmissionThrows => throw new InvalidOperationException("the probe was told to fail its admission"),
            _ => UbcInstanceAnswer.Ready,
        };
    }

    public static int StartUnit(object instanceState) => ((ProbeInstance)instanceState).Definitions.Start;

    public static void AbandonInstance(object instanceState)
    {
        var instance = (ProbeInstance)instanceState;
        ProbeLog.Abandon(instance);
        instance.Environment.Meter.ReportReleased(VmBudgetDimension.LiveBytes, ProbeInstance.Retained);

        if (instance.Definitions.Admission == ProbeAdmission.AbandonThrows)
        {
            throw new InvalidOperationException("the probe was told to fail its abandonment");
        }
    }

    /// <summary>
    /// Answers from the entry point's name: <c>missing</c>, <c>refused</c> and <c>unanswered</c> answer
    /// as they say, <c>found:N</c> names unit N whatever it is, <c>bind-refuses</c> finds <c>main</c>
    /// and is then refused its binding, and every other name is looked up in the Entries section.
    /// </summary>
    public static UbcEntryAnswer ResolveEntry(object instanceState, UbcVerifiedProgram program, ReadOnlySpan<byte> entryPoint)
    {
        var name = Encoding.UTF8.GetString(entryPoint);

        if (name.StartsWith("found:", StringComparison.Ordinal))
        {
            return UbcEntryAnswer.Found(int.Parse(name.AsSpan(6), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));
        }

        return name switch
        {
            "missing" => UbcEntryAnswer.Missing,
            "refused" => UbcEntryAnswer.Refused(new ProbeFault("EntryRefusedByFamily", 0)),
            "unanswered" => default,
            "bind-refuses" => program.TryGetEntry("main"u8, out var main) ? UbcEntryAnswer.Found(main) : UbcEntryAnswer.Missing,
            _ => program.TryGetEntry(entryPoint, out var unit) ? UbcEntryAnswer.Found(unit) : UbcEntryAnswer.Missing,
        };
    }

    public static bool BindParameters(ref UbcActivation activation, ReadOnlySpan<byte> entryName)
    {
        if (entryName.SequenceEqual("bind-refuses"u8))
        {
            return false;
        }

        var unit = activation.Program.Units[activation.Unit];
        Array.Clear(activation.Words, activation.WordBase, unit.ParameterWords);
        activation.Values.Clear(activation.ValueBase, unit.ParameterValues);
        return true;
    }

    public static IVmProfilePayload? Completion(ref UbcActivation activation)
    {
        var unit = activation.Program.Units[activation.Unit];
        var plane = (ProbePlane)activation.Values;
        var words = ImmutableArray.CreateBuilder<long>(unit.ResultWords);

        for (var index = 0; index < unit.ResultWords; index++)
        {
            words.Add(unchecked((long)activation.Words[activation.WordArgs + index]));
        }

        return new ProbeResult(words.MoveToImmutable(), [.. plane.Slice(activation.ValueArgs, unit.ResultValues)]);
    }

    public static IVmProfilePayload Fault(ref UbcActivation activation, byte familySlot, ushort code) =>
        new ProbeFault(familySlot == 0 && code == 0 ? "Unreachable" : "Trap", code);

    public static IVmProfilePayload? Uncaught(ref UbcActivation activation) => new ProbeFault("Uncaught", 0);

    public static IVmProfilePayload? SuspendProjection(ref UbcActivation activation) => null;

    public static IVmProfilePayload? EntryRefused(object instanceState, ReadOnlySpan<byte> entryName) => new ProbeFault("NoSuchEntry", 0);

    public static object CaptureValues(IUbcValuePlane plane, int start, int count) => ((ProbePlane)plane).Slice(start, count);

    public static void RestoreValues(IUbcValuePlane plane, int start, object captured) => ((ProbePlane)plane).Write(start, (long[])captured);
}

/// <summary>Writes a probe artifact: units, their signatures (never merged), entries, and FamilyData when given.</summary>
internal sealed class ProbeAssembly
{
    private readonly List<UbcSignature> types = new();
    private readonly List<UbcUnit> units = new();
    private readonly List<byte> code = new();
    private readonly List<UbcEntry> entries = new();

    internal UbcCodeBuilder Begin() => new((uint)code.Count);

    /// <summary>A Types row of its own, even when another row has the same slots: rows are named by index.</summary>
    internal int Type(UbcSlotType[] parameters, UbcSlotType[] results)
    {
        types.Add(new UbcSignature(ImmutableArray.Create(parameters), ImmutableArray.Create(results)));
        return types.Count - 1;
    }

    internal int End(UbcCodeBuilder builder, int type, uint maxWords, uint maxValues, UbcUnitFlags flags, params uint[] landings)
    {
        var bytes = builder.ToArray();
        units.Add(new UbcUnit(
            (uint)type, ProbeProfile.Slot, ImmutableArray<UbcLocalRun>.Empty, maxWords, maxValues, builder.BaseOffset, (uint)bytes.Length,
            flags, ImmutableArray.Create(landings)));
        code.AddRange(bytes);
        return units.Count - 1;
    }

    internal void Entry(string name, int unit) =>
        entries.Add(new UbcEntry(ImmutableArray.Create(Encoding.UTF8.GetBytes(name)), (uint)unit));

    internal byte[] Write(byte[]? familyData = null)
    {
        var header = new UbcHeader(
            UbcFormat.FormatVersion, ProbeProfile.Identity, ProbeProfile.ManifestIdentity, UbcFormat.BytecodeForm, "com.example.ubcprobe.lowering", 1);
        var writer = new UbcArtifactWriter(header)
            .Families([new UbcFamilyEntry(ProbeProfile.Slot, ProbeProfile.Identity, ProbeProfile.Table.TableVersion, ProbeProfile.ManifestIdentity)])
            .Types(types)
            .Units(units)
            .Code(code.ToArray())
            .Entries(entries);

        if (familyData is not null)
        {
            writer.FamilyData(ProbeProfile.Slot, familyData);
        }

        return writer.ToArray();
    }
}
