using Broiler.VM.Abstractions;
using Broiler.VM.Ubc;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// The checks of universal bytecode contract version 2's executor and interpreter paths, run over the
/// probe family inside the published image: a call row of the signature effect form, a family's entry
/// resolution, its admission of an instance, and an instance's start unit.
/// </summary>
/// <remarks>
/// The contract suite cannot reach these paths: the bytecode emitter is a composition root's to name,
/// and no test project may reference it. Each check states what the core answered and, where the
/// answer alone cannot show it, what the probe counted - how many states it made and how many it was
/// told to abandon - so that an executor which published a refused instance, or abandoned one twice,
/// fails the check rather than passing it with the right transcript.
/// </remarks>
internal static class ProbeChecks
{
    private const byte S = ProbeProfile.Slot;
    private const UbcSlotType I32 = UbcSlotType.I32;
    private const UbcSlotType I64 = UbcSlotType.I64;
    private const UbcSlotType V = UbcSlotType.V;

    private const string ContractViolation = "ProfileFault ProfileContractViolation";

    /// <summary>
    /// A signature call row enters the callee its top i32 names with the named signature's parameters,
    /// which lie beneath a trailing value and the i32 on each plane; a callee whose parameters differ, one
    /// whose results differ, and none at all are refused, and one of the same slots under another Types
    /// row is admitted.
    /// </summary>
    internal static (string, bool, string) SignatureCall()
    {
        var bytes = SignatureProgram();
        var failures = new List<string>();

        // The callee answers its first parameter times a thousand plus its third, and its value
        // parameter. Taken from the top of the inputs instead of their bottom, the words would be
        // 9 and the index, and the value the trailing 99.
        Expect(failures, "main", Run(bytes, "main"), "completed words=[7009,5] values=[]");
        Expect(failures, "structural", Run(bytes, "structural"), "completed words=[9,5] values=[]");
        Expect(failures, "mismatch", Run(bytes, "mismatch"), ContractViolation);
        Expect(failures, "absent", Run(bytes, "absent"), ContractViolation);

        // A callee of the named results whose parameters are three i64s: entered, it would take the
        // trailing i32 for its third parameter and answer words 7009 and 8, so only comparing the
        // parameters, and not the results alone, refuses it.
        Expect(failures, "parameters", Run(bytes, "parameters"), ContractViolation);

        return failures.Count == 0
            ? ("signature-call", true, "a signature row entered its callee with the parameters beneath its trailing value and i32 on both planes, admitted a callee of equal slots under another Types row, and refused one of other parameters, one of other results and one the program does not have")
            : ("signature-call", false, string.Join("; ", failures));
    }

    /// <summary>
    /// A family's entry resolution is honoured and checked: a found unit is entered only when it exists
    /// and is flagged an entry, a missing or refused one faults with the family's payload, an answer of
    /// no kind is a contract violation, and resolving is charged one fuel unit before the family runs.
    /// </summary>
    internal static (string, bool, string) EntryResolution()
    {
        var bytes = EntryProgram();
        var failures = new List<string>();

        Expect(failures, "main", Run(bytes, "main"), "completed words=[42] values=[]");
        Expect(failures, "found:0", Run(bytes, "found:0"), "completed words=[42] values=[]");
        Expect(failures, "found:1", Run(bytes, "found:1"), ContractViolation);
        Expect(failures, "found:2", Run(bytes, "found:2"), ContractViolation);
        Expect(failures, "found:-1", Run(bytes, "found:-1"), ContractViolation);
        Expect(failures, "missing", Run(bytes, "missing"), "faulted NoSuchEntry code=0");
        Expect(failures, "refused", Run(bytes, "refused"), "faulted EntryRefusedByFamily code=0");
        Expect(failures, "unanswered", Run(bytes, "unanswered"), ContractViolation);
        Expect(failures, "bind-refuses", Run(bytes, "bind-refuses"), "faulted NoSuchEntry code=0");
        Expect(failures, "no-entry-of-that-name", Run(bytes, "no-entry-of-that-name"), "faulted NoSuchEntry code=0");

        // The first fuel ceiling at which the instance stands, and the first at which a missing entry
        // point is answered: resolving it runs no instruction and costs exactly the one unit charged
        // before the family is asked.
        var instantiable = FirstCeiling(fuel => !Run(bytes, "missing", limits => limits[(int)VmBudgetDimension.Fuel] = fuel).StartsWith("instantiation", StringComparison.Ordinal));
        var answered = FirstCeiling(fuel => Run(bytes, "missing", limits => limits[(int)VmBudgetDimension.Fuel] = fuel).StartsWith("faulted", StringComparison.Ordinal));
        var between = Run(bytes, "missing", limits => limits[(int)VmBudgetDimension.Fuel] = (ulong)instantiable);

        if (instantiable < 1 || answered != instantiable + 1 || !string.Equals(between, "exhausted Fuel", StringComparison.Ordinal))
        {
            failures.Add($"resolution fuel: the instance stands from a ceiling of {instantiable}, a missing entry is answered from {answered}, and at {instantiable} it answered '{between}'");
        }

        return failures.Count == 0
            ? ("entry-resolution", true, $"a family's found, missing and refused answers were honoured, an unflagged, absent or negative unit and an answer of no kind refused, a refused binding answered as no such entry, and resolving charged the one fuel unit between a ceiling of {instantiable} and {answered}")
            : ("entry-resolution", false, string.Join("; ", failures));
    }

    /// <summary>
    /// A family's admission decides whether an instance is published: every answer but ready - and an
    /// answer of no kind, and an exception - abandons the state exactly once within the step, releasing
    /// what it retained, and a state that was never made is not abandoned.
    /// </summary>
    internal static (string, bool, string) InstanceAdmission()
    {
        var failures = new List<string>();

        void Case(ProbeAdmission admission, string expected, int created, int abandoned)
        {
            ProbeLog.Reset();
            var answer = Run(AdmissionProgram(admission, start: 0xFF), "main");
            Expect(failures, admission.ToString(), answer, expected);
            Counted(failures, admission.ToString(), created, abandoned);
        }

        Case(ProbeAdmission.Ready, "completed words=[0] values=[]", 1, 0);
        Case(ProbeAdmission.Faulted, "instantiation faulted Admission code=0", 1, 1);
        Case(ProbeAdmission.Exhausted, "instantiation exhausted AllocatedBytes", 1, 1);
        Case(ProbeAdmission.Unanswered, "instantiation " + ContractViolation, 1, 1);
        Case(ProbeAdmission.AdmissionThrows, "instantiation " + ContractViolation, 1, 1);
        Case(ProbeAdmission.CreationThrows, "instantiation " + ContractViolation, 0, 0);
        Case(ProbeAdmission.AbandonThrows, "instantiation " + ContractViolation, 1, 1);

        // Retention: every state retains a kilobyte of live bytes and its abandonment releases it, so
        // sixty-four refused instantiations fit under a ceiling of thirty-two kilobytes and the ready
        // one after them still stands.
        ProbeLog.Reset();
        using (var runtime = Runtime(limits => limits[(int)VmBudgetDimension.LiveBytes] = 32 * 1024))
        {
            var descriptor = ProbeProfile.ArtifactDescriptor();
            var refused = AdmissionProgram(ProbeAdmission.Faulted, start: 0xFF);

            if (!runtime.Verify(in descriptor, refused, default).TryGetArtifact(out var handle))
            {
                failures.Add("the retention program did not verify");
            }
            else
            {
                for (var attempt = 0; attempt < 64; attempt++)
                {
                    var instantiated = runtime.Instantiate(handle, default);

                    if (instantiated.Outcome != VmOutcome.ProfileFault)
                    {
                        failures.Add($"refused instantiation {attempt} answered {instantiated.Outcome} {instantiated.Reason}");
                        break;
                    }
                }

                var ready = Run(runtime, AdmissionProgram(ProbeAdmission.Ready, start: 0xFF), "main");
                Expect(failures, "after sixty-four refusals", ready, "completed words=[0] values=[]");
            }
        }

        return failures.Count == 0
            ? ("instance-admission", true, "a ready instance was published; a faulted, exhausted, unanswered or throwing admission, and an abandonment that throws, each abandoned its state once within the step; a state never made was not abandoned; and sixty-four refused instances released their retention")
            : ("instance-admission", false, string.Join("; ", failures));
    }

    /// <summary>
    /// A start unit runs in the instantiation step, once, before the instance is published, and asks the
    /// family for no completion payload: a completed run publishes it with the run's effect, and a trap,
    /// a signature that takes or gives anything, a unit the program does not have, an exhausted
    /// allowance, a handler's exception and a suspension each abandon the state once - the exception
    /// giving back the call depth its frames held, as does one the value plane throws before the start
    /// unit's frame stands.
    /// </summary>
    internal static (string, bool, string) StartUnit()
    {
        var failures = new List<string>();

        void Case(string name, byte start, string expected, int abandoned, int completions, Action<ulong[]>? adjust = null)
        {
            ProbeLog.Reset();
            var answer = Run(AdmissionProgram(ProbeAdmission.Ready, start), "main", adjust);
            Expect(failures, name, answer, expected);
            Counted(failures, name, 1, abandoned);
            Completions(failures, name, completions);
        }

        // Only an invocation's return asks for a completion payload: the start run's return does not,
        // so the one that completes is asked for one payload, by the invocation of main.
        Case("completes", 1, "completed words=[5] values=[]", 0, 1);
        Case("traps", 2, "instantiation faulted Trap code=1", 1, 0);
        Case("gives a result", 3, "instantiation " + ContractViolation, 1, 0);
        Case("is no unit", 0xFE, "instantiation " + ContractViolation, 1, 0);
        Case("never ends", 4, "instantiation exhausted Fuel", 1, 0, limits => limits[(int)VmBudgetDimension.Fuel] = 5_000);
        Case("throws", 5, "instantiation " + ContractViolation, 1, 0);
        Case("suspends", 6, "instantiation " + ContractViolation, 1, 0);

        // The start unit ran once, before publication, and not again for each invocation, and the two
        // invocations' returns were the only ones asked for a payload.
        using (var runtime = Runtime())
        {
            var descriptor = ProbeProfile.ArtifactDescriptor();
            ProbeLog.Reset();

            if (runtime.Verify(in descriptor, AdmissionProgram(ProbeAdmission.Ready, start: 1), default).TryGetArtifact(out var handle) &&
                runtime.Instantiate(handle, default).TryGetInstance(out var instance))
            {
                using (instance)
                {
                    var first = Invoke(instance, "main");
                    var second = Invoke(instance, "main");
                    Expect(failures, "second invocation", $"{first} | {second}", "completed words=[5] values=[] | completed words=[5] values=[]");
                    Completions(failures, "second invocation", 2);
                }
            }
            else
            {
                failures.Add("the once program did not instantiate");
            }
        }

        // A start run whose handler threw gave back its frame's depth: under a call depth of one, a
        // start run after it still enters its frame.
        using (var shallow = Runtime(limits => limits[(int)VmBudgetDimension.CallDepth] = 1))
        {
            var crashed = Run(shallow, AdmissionProgram(ProbeAdmission.Ready, start: 5), "main");
            var after = Run(shallow, AdmissionProgram(ProbeAdmission.Ready, start: 1), "main");
            Expect(failures, "under a call depth of one", $"{crashed} | {after}", $"instantiation {ContractViolation} | completed words=[5] values=[]");
        }

        // So did one whose value plane threw while the start unit's frame was being entered - after its
        // depth was charged and before the frame stood - which the core, settling only operations,
        // would otherwise leave charged in the runtime for good.
        using (var shallow = Runtime(limits => limits[(int)VmBudgetDimension.CallDepth] = 1))
        {
            string grew;
            ProbeLog.Reset();
            ProbePlane.FailGrowth = true;

            try
            {
                grew = Run(shallow, AdmissionProgram(ProbeAdmission.Ready, start: 1), "main");
            }
            finally
            {
                ProbePlane.FailGrowth = false;
            }

            Counted(failures, "a plane that throws", 1, 1);
            var after = Run(shallow, AdmissionProgram(ProbeAdmission.Ready, start: 1), "main");
            var again = Run(shallow, AdmissionProgram(ProbeAdmission.Ready, start: 1), "main");
            Expect(
                failures, "after a plane that throws", $"{grew} | {after} | {again}",
                $"instantiation {ContractViolation} | completed words=[5] values=[] | completed words=[5] values=[]");
        }

        return failures.Count == 0
            ? ("start-unit", true, "a start unit ran once before publication and asked for no payload; a trap, a result, an absent unit, fuel run out, a handler's exception and a suspension each abandoned the state once; and a handler's exception, and a plane's before the frame stood, gave the depth back")
            : ("start-unit", false, string.Join("; ", failures));
    }

    // ---- the programs -----------------------------------------------------------------------------

    /// <summary>
    /// Five callers and four callees. Each caller pushes 7, a value of 5, 9 and a value of 99, then
    /// the unit it calls, and calls through the signature row naming Types row 1, [i64 v i64] to
    /// [i64 v]; it answers the callee's i64 and the value's amount. Unit 5 has row 1, unit 6 a row of
    /// the same parameters and one result, unit 7 a row of exactly row 1's slots, and unit 8 a row of
    /// row 1's results whose parameters are three i64s. No unit 9 exists.
    /// </summary>
    private static byte[] SignatureProgram()
    {
        var assembly = new ProbeAssembly();
        var main = assembly.Type([], [I64, I64]);
        var named = assembly.Type([I64, V, I64], [I64, V]);
        var fewer = assembly.Type([I64, V, I64], [I64]);
        var same = assembly.Type([I64, V, I64], [I64, V]);

        var words = assembly.Type([I64, I64, I64], [I64, V]);

        foreach (var (entry, callee) in new[] { ("main", 5), ("structural", 7), ("mismatch", 6), ("absent", 9), ("parameters", 8) })
        {
            var b = assembly.Begin();
            b.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 7);
            b.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 5);
            b.EmitFamily(S, ProbeProfile.Box, UbcOperandShape.None);
            b.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 9);
            b.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 99);
            b.EmitFamily(S, ProbeProfile.Box, UbcOperandShape.None);
            b.Emit(UbcOpcode.ConstI32, (ulong)callee);
            b.EmitFamily(S, ProbeProfile.CallSignature, UbcOperandShape.U32, (ulong)named);
            b.EmitFamily(S, ProbeProfile.Unbox, UbcOperandShape.None);
            b.Emit(UbcOpcode.Return);
            assembly.Entry(entry, assembly.End(b, main, 3, 2, UbcUnitFlags.Entry));
        }

        var answer = assembly.Begin();
        answer.Emit(UbcOpcode.LocalGet, 0);
        answer.Emit(UbcOpcode.ConstI64, 1000);
        answer.EmitFamily(S, ProbeProfile.MulWords, UbcOperandShape.None);
        answer.Emit(UbcOpcode.LocalGet, 2);
        answer.EmitFamily(S, ProbeProfile.AddWords, UbcOperandShape.None);
        answer.Emit(UbcOpcode.LocalGet, 1);
        answer.Emit(UbcOpcode.Return);
        assembly.End(answer, named, 2, 1, UbcUnitFlags.None);

        var other = assembly.Begin();
        other.Emit(UbcOpcode.LocalGet, 0);
        other.Emit(UbcOpcode.Return);
        assembly.End(other, fewer, 1, 0, UbcUnitFlags.None);

        var equal = assembly.Begin();
        equal.Emit(UbcOpcode.LocalGet, 2);
        equal.Emit(UbcOpcode.LocalGet, 1);
        equal.Emit(UbcOpcode.Return);
        assembly.End(equal, same, 1, 1, UbcUnitFlags.None);

        // Its first parameter times a thousand plus its second, and its third boxed.
        var confused = assembly.Begin();
        confused.Emit(UbcOpcode.LocalGet, 0);
        confused.Emit(UbcOpcode.ConstI64, 1000);
        confused.EmitFamily(S, ProbeProfile.MulWords, UbcOperandShape.None);
        confused.Emit(UbcOpcode.LocalGet, 1);
        confused.EmitFamily(S, ProbeProfile.AddWords, UbcOperandShape.None);
        confused.Emit(UbcOpcode.LocalGet, 2);
        confused.EmitFamily(S, ProbeProfile.Box, UbcOperandShape.None);
        confused.Emit(UbcOpcode.Return);
        assembly.End(confused, words, 2, 1, UbcUnitFlags.None);

        return assembly.Write();
    }

    /// <summary>Unit 0, the entry <c>main</c>, answers 42; unit 1, not flagged an entry, answers 43.</summary>
    private static byte[] EntryProgram()
    {
        var assembly = new ProbeAssembly();
        var type = assembly.Type([], [I64]);

        foreach (var (amount, flags) in new[] { (42UL, UbcUnitFlags.Entry), (43UL, UbcUnitFlags.None) })
        {
            var b = assembly.Begin();
            b.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, amount);
            b.Emit(UbcOpcode.Return);
            assembly.End(b, type, 1, 0, flags);
        }

        assembly.Entry("main", 0);
        return assembly.Write();
    }

    /// <summary>
    /// The entry <c>main</c> answers the instance's record; the other units are start units - one that
    /// records 5, one that traps, one that gives a result, one that never ends, one whose handler
    /// throws and one that suspends. FamilyData names the admission and the start unit.
    /// </summary>
    private static byte[] AdmissionProgram(ProbeAdmission admission, byte start)
    {
        var assembly = new ProbeAssembly();
        var reads = assembly.Type([], [I64]);
        var nothing = assembly.Type([], []);

        var main = assembly.Begin();
        main.EmitFamily(S, ProbeProfile.Read, UbcOperandShape.None);
        main.Emit(UbcOpcode.Return);
        assembly.Entry("main", assembly.End(main, reads, 1, 0, UbcUnitFlags.Entry));

        var records = assembly.Begin();
        records.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 5);
        records.EmitFamily(S, ProbeProfile.Record, UbcOperandShape.None);
        records.Emit(UbcOpcode.Return);
        assembly.End(records, nothing, 1, 0, UbcUnitFlags.None);

        var traps = assembly.Begin();
        traps.Emit(UbcOpcode.Trap, S | ((ulong)ProbeProfile.TrapProbe << 8));
        assembly.End(traps, nothing, 0, 0, UbcUnitFlags.None);

        var gives = assembly.Begin();
        gives.EmitFamily(S, ProbeProfile.Push, UbcOperandShape.I32, 1);
        gives.Emit(UbcOpcode.Return);
        assembly.End(gives, reads, 1, 0, UbcUnitFlags.None);

        var spins = assembly.Begin();
        var top = spins.NewLabel();
        spins.Mark(top);
        spins.EmitTo(UbcOpcode.Jump, top);
        assembly.End(spins, nothing, 0, 0, UbcUnitFlags.None);

        var crashes = assembly.Begin();
        crashes.EmitFamily(S, ProbeProfile.Crash, UbcOperandShape.None);
        crashes.Emit(UbcOpcode.Return);
        assembly.End(crashes, nothing, 0, 0, UbcUnitFlags.None);

        var yields = assembly.Begin();
        yields.EmitFamily(S, ProbeProfile.Yield, UbcOperandShape.None);
        var resume = yields.Offset;
        yields.Emit(UbcOpcode.Return);
        assembly.End(yields, nothing, 0, 0, UbcUnitFlags.Suspendable, resume);

        return assembly.Write([(byte)admission, start]);
    }

    // ---- running them -----------------------------------------------------------------------------

    private static VmRuntime Runtime(Action<ulong[]>? adjust = null)
    {
        var limits = ProbeProfile.Limits();
        adjust?.Invoke(limits);
        return FixtureHost.Create(FixtureHost.Catalog(ProbeProfile.Descriptor), FixtureHost.Explicit(limits), withProvider: false);
    }

    /// <summary>Verifies, instantiates and invokes <paramref name="entry"/> in a runtime of its own.</summary>
    private static string Run(byte[] artifact, string entry, Action<ulong[]>? adjust = null)
    {
        using var runtime = Runtime(adjust);
        return Run(runtime, artifact, entry);
    }

    private static string Run(VmRuntime runtime, byte[] artifact, string entry)
    {
        var descriptor = ProbeProfile.ArtifactDescriptor();
        var verified = runtime.Verify(in descriptor, artifact, default);

        if (!verified.TryGetArtifact(out var handle))
        {
            return $"refused {verified.Outcome} {verified.Reason} {verified.Diagnostics.ProfileDiagnosticCode.ToString(CultureInfo.InvariantCulture)}";
        }

        var instantiated = runtime.Instantiate(handle, default);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return "instantiation " + Line(
                instantiated.Outcome, instantiated.Reason, instantiated.Diagnostics,
                instantiated.TryGetPayload<IVmProfilePayload>(out var fault) ? fault : null);
        }

        using (instance)
        {
            return Invoke(instance, entry);
        }
    }

    private static string Invoke(VmInstance instance, string entry)
    {
        var request = new VmInvocationRequest(new VmUtf8Text(Encoding.UTF8.GetBytes(entry)));
        var result = instance.Invoke(in request, default);
        return Line(result.Outcome, result.Reason, result.Diagnostics, result.TryGetPayload<IVmProfilePayload>(out var payload) ? payload : null);
    }

    private static string Line(VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics, IVmProfilePayload? payload) => outcome switch
    {
        VmOutcome.Normal => payload?.ToString() ?? "completed",
        VmOutcome.ProfileFault when payload is not null => payload.ToString()!,
        VmOutcome.ResourceExhaustion => $"exhausted {diagnostics.ExhaustedDimension}",
        _ => $"{outcome} {reason}",
    };

    private static int FirstCeiling(Func<ulong, bool> holds)
    {
        for (var fuel = 1; fuel <= 64; fuel++)
        {
            if (holds((ulong)fuel))
            {
                return fuel;
            }
        }

        return -1;
    }

    private static void Expect(List<string> failures, string name, string actual, string expected)
    {
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            failures.Add($"{name}: expected '{expected}', got '{actual}'");
        }
    }

    private static void Completions(List<string> failures, string name, int completions)
    {
        if (ProbeLog.Completions != completions)
        {
            failures.Add($"{name}: the family was asked for {ProbeLog.Completions} completion payloads, where {completions} were due");
        }
    }

    private static void Counted(List<string> failures, string name, int created, int abandoned)
    {
        if (ProbeLog.Created != created || ProbeLog.Abandoned != abandoned || ProbeLog.AbandonedTwice != 0)
        {
            failures.Add(
                $"{name}: {ProbeLog.Created} states made and {ProbeLog.Abandoned} abandoned ({ProbeLog.AbandonedTwice} twice), " +
                $"where {created} and {abandoned} were due");
        }
    }
}
