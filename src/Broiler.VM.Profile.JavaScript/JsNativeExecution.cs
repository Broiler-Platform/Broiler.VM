// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           9
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Critical
// Criteria:         18/18
// Resource impact:  4/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One instantiation of an artifact whose form is machine code: an armed mapping and the three
/// slabs the emitted code reads and writes.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE MAPPING IS ARMED WHEN THE INSTANCE IS BUILT AND IS NEVER WRITABLE AGAIN.</b> Everything
/// this instance holds is either an armed mapping or a slab of <c>double</c>s. There is no managed
/// reference anywhere emitted code can reach - not in the frame, not in a slab, not behind a
/// pointer in one - which is how this profile answers the rule that a profile unable to say where
/// its emitted code's references are rooted has not earned the form. There is nothing to root.
/// </para>
/// <para>
/// <b>THE SLABS ARE ALLOCATED PINNED AND STAY AT ONE ADDRESS FOR THE INSTANCE'S LIFETIME.</b>
/// Emitted code holds their addresses across a call it cannot be interrupted inside safely, so a
/// slab the collector could move would be a slab whose address emitted code was still using after
/// it moved. Pinned allocation costs a segment the collector does not compact and buys a guarantee
/// no amount of care about pinning scopes would.
/// </para>
/// <para>
/// <b>The realm of a numeric artifact is a slab of <c>double</c>s and that is the whole of it.</b>
/// A binding of this manifest is a Number; there is no global object, no prototype chain and no
/// standard library, because the manifest admits no syntax that could reach one. What that costs is
/// stated where the manifest is defined: the admitted language is small and it is not JavaScript.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=7ABDB0
// Broiler-Falsified-If: anything reachable from this instance holds a managed reference emitted code can dereference
// Broiler-Human:        PENDING
internal sealed unsafe class JsNativeInstance : IVmInstanceState, System.IDisposable
{
    /// <summary>How many operand-slab slots one instance is given.</summary>
    /// <remarks>
    /// <para>
    /// <b>THIS NUMBER IS THE RECURSION BOUND AND NOT A CAPACITY HINT.</b> Every emitted unit's
    /// prologue takes its whole region out of this slab and answers an exhaustion when the slab
    /// runs out, so a program that recurses without end stops here rather than on the machine
    /// stack. Bounding recursion by the machine stack instead would mean bounding it by a resource
    /// this component cannot measure, cannot charge for and cannot survive exhausting.
    /// </para>
    /// <para>
    /// <b>It is stated rather than derived, and the consequence is that it is a different bound
    /// from the interpreter's.</b> The interpreter's recursion bound is a guest stack it sizes
    /// itself; this one is a slab. The two forms therefore refuse a deep recursion at different
    /// depths, which is a real difference between the forms rather than a defect in either.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A12001
    // Broiler-Falsified-If: an emitted unit can be entered when fewer slots remain than its region needs
    // Broiler-Human:        PENDING
    internal const int OperandSlabSlots = 1 << 16;

    /// <summary>
    /// The fuel one invocation of an emitted artifact is given before it must stop.
    /// </summary>
    /// <remarks>
    /// <b>EMITTED CODE CHARGES A COUNTER AND MANAGED CODE CHARGES THE METER, once, afterwards, for
    /// what the counter says was spent.</b> The interpreter charges its meter once per instruction,
    /// which emitted code cannot do without a managed call per instruction - the transition alone
    /// would cost more than the instruction. So the counter is the accounting emitted code can
    /// actually do, and this ceiling is what bounds a run between two charges. THE CONSEQUENCE IS
    /// THAT THE TWO FORMS EXHAUST AT DIFFERENT POINTS FOR THE SAME PROGRAM, and a retained corpus
    /// row that pins an exhaustion pins it for one form.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=5FA9CD
    // Broiler-Falsified-If: an invocation runs past this many charged events without the meter being consulted
    // Broiler-Human:        PENDING
    internal const long FuelPerInvocation = 1L << 32;

    /// <summary>Creates the instance, arming the mapping and filling the slabs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=E54A21
    // Broiler-Falsified-If: an instance is constructed around a mapping that is not armed
    // Broiler-Human:        PENDING
    private JsNativeInstance(
        JsProgram program,
        JsNativePage page,
        IVmExecutionEnvironment environment,
        double[] operands,
        double[] bindings,
        double[] constants,
        long[] fuel)
    {
        Program = program;
        Page = page;
        Environment = environment;
        Operands = operands;
        Bindings = bindings;
        Constants = constants;
        Fuel = fuel;
    }

    /// <summary>The verified program this instance runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    /// <summary>The armed mapping the emitted code lives in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=574100
    // Broiler-Falsified-If: this hands out a mapping that is not armed
    // Broiler-Human:        PENDING
    internal JsNativePage Page { get; }

    /// <summary>The environment the instance was created against.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=1C7767
    // Broiler-Human:        PENDING
    internal IVmExecutionEnvironment Environment { get; }

    /// <summary>The operand slab, which is also every unit's scope storage.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=489648
    // Broiler-Falsified-If: this slab is not pinned, so the collector can move it while emitted code holds its address
    // Broiler-Human:        PENDING
    internal double[] Operands { get; }

    /// <summary>The realm's numeric bindings, one slot per constant-pool entry.</summary>
    /// <remarks>
    /// <b>It is indexed by the constant that NAMES the binding rather than by a dense binding
    /// index, and the waste is deliberate.</b> The instructions that read and write a realm binding
    /// carry the name's constant index and nothing else, so indexing by it means the emitted code
    /// computes a displacement from an operand it already has. A dense index would need a map both
    /// the emitter and this file agreed on, which is one more thing two parties can disagree about
    /// for the sake of a few slots.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=75149F
    // Broiler-Falsified-If: this slab is not pinned, or a slot of it is readable by emitted code before an initialiser stored to it
    // Broiler-Human:        PENDING
    internal double[] Bindings { get; }

    /// <summary>The constant pool's Numbers, in pool order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=3047A3
    // Broiler-Human:        PENDING
    internal double[] Constants { get; }

    /// <summary>The one-element slab holding the fuel counter emitted code decrements.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=5C814B
    // Broiler-Falsified-If: this slab is not pinned, so the collector can move it while emitted code is decrementing it
    // Broiler-Human:        PENDING
    internal long[] Fuel { get; }

    /// <summary>How many invocations this instance has answered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=218248
    // Broiler-Human:        PENDING
    internal int InvocationCount { get; set; }

    /// <summary>Maps and arms the artifact's code, and builds the slabs it reads.</summary>
    /// <remarks>
    /// <b>THE ARMING HAPPENS HERE AND NOT AT VERIFICATION, and the reason is which contract each
    /// obeys.</b> A verified state is shareable: it may be read by several runtimes at once with no
    /// lock between them, and it must be immutable once verification returns. A mapping is neither
    /// of those while it is being written, so the artifact's verified state holds the BYTES and an
    /// instance holds the MAPPING - which means one artifact instantiated twice maps twice, and
    /// that is the price of a handle that several runtimes can hold.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=CA4D68
    // Broiler-Falsified-If: an instance is produced whose mapping is not armed
    // Broiler-Human:        PENDING
    internal static JsNativeInstance? TryCreate(
        JsProgram program, IVmExecutionEnvironment environment)
    {
        var page = JsNativePage.TryMap(program.NativeCode);

        if (page is null)
        {
            return null;
        }

        if (!page.Arm())
        {
            page.Dispose();
            return null;
        }

        var operands = System.GC.AllocateArray<double>(OperandSlabSlots, pinned: true);
        var bindings = System.GC.AllocateArray<double>(
            System.Math.Max(1, program.Constants.Length), pinned: true);

        var constants = System.GC.AllocateArray<double>(
            System.Math.Max(1, program.Constants.Length), pinned: true);

        var fuel = System.GC.AllocateArray<long>(1, pinned: true);

        // EVERY BINDING STARTS UNINITIALISED AND READING ONE IS A ReferenceError. That is what the
        // language says a `let` or `const` does before its initialiser runs, and the emitted code
        // compares against this pattern on every read of a binding rather than assuming an
        // initialiser ran first.
        var uninitialised = JsNativeValues.FromBits(JsNativeValues.UninitialisedBits);

        for (var index = 0; index < bindings.Length; index++)
        {
            bindings[index] = uninitialised;
        }

        for (var index = 0; index < program.Constants.Length; index++)
        {
            constants[index] = program.Constants[index].IsNumber
                ? program.Constants[index].AsNumber()
                : 0;
        }

        return new JsNativeInstance(
            program, page, environment, operands, bindings, constants, fuel);
    }

    /// <summary>Releases the mapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=5D9DD4
    // Broiler-Falsified-If: a mapping outlives the instance that owns it, or is released while an invocation is still inside it
    // Broiler-Human:        PENDING
    public void Dispose() => Page.Dispose();
}

/// <summary>The native form's half of the executor: instantiate, invoke, and report.</summary>
/// <remarks>
/// <b>IT IS A SECOND ARM OF ONE EXECUTOR AND NOT A SECOND EXECUTOR, and the difference is where the
/// choice is made.</b> Which arm runs is decided by what the artifact carried and was pinned when
/// it was verified: a handle whose payload carried emitted code has the emitted form for as long as
/// it exists, and a handle whose payload did not never acquires one. Nothing here observes a run,
/// counts a call or promotes anything, because a code path that selected a form from run-time
/// observation would be the second execution arm this profile's non-goals refuse.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=65CC7E
// Broiler-Falsified-If: the form an invocation runs under differs from the form its handle carried when it was minted
// Broiler-Human:        PENDING
internal static unsafe class JsNativeExecution
{
    /// <summary>Whether this program's form is machine code this image can run.</summary>
    /// <remarks>
    /// <b>TWO QUESTIONS AND BOTH ARE ABOUT THE ARTIFACT EXCEPT THE LAST, WHICH IS ABOUT THE
    /// MACHINE.</b> Whether an artifact carries emitted code is a fact about the artifact; whether
    /// this process can call into code emitted for that architecture and that convention is a fact
    /// about the running machine, and it is the only run-time observation on this path. It selects
    /// nothing - an artifact whose architecture this machine is not answers a refusal, not the
    /// interpreter.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=2492DE
    // Broiler-Falsified-If: this answers true for an artifact emitted for a convention this process does not use
    // Broiler-Human:        PENDING
    internal static bool CarriesEmittedCode(JsProgram program) => program.NativeCode.Length != 0;

    /// <summary>The architecture and convention this process can call into.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=658D99
    // Broiler-Falsified-If: this names a convention other than the one this process actually uses
    // Broiler-Human:        PENDING
    internal static JsNativeArchitecture HostArchitecture
    {
        get
        {
            if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture !=
                System.Runtime.InteropServices.Architecture.X64)
            {
                return JsNativeArchitecture.None;
            }

            return System.OperatingSystem.IsWindows()
                ? JsNativeArchitecture.X64Windows
                : JsNativeArchitecture.X64SystemV;
        }
    }

    /// <summary>Maps and arms the artifact, or refuses because this image cannot run it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=DC113E
    // Broiler-Falsified-If: an artifact emitted for another architecture is instantiated
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Instantiate(
        JsProgram program, IVmExecutionEnvironment environment)
    {
        if (program.NativeArchitecture != HostArchitecture)
        {
            // A REFUSAL AND NOT A FALLBACK. The bytecode is in the same artifact and this arm will
            // not run it: the form was fixed when the artifact was compiled and pinned when it was
            // verified, and an executor that quietly ran the other form would be choosing a form
            // from a run-time observation.
            return VmExecutionStep.ContractViolation(VmReason.UnsatisfiedHostAssumption);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var instance = JsNativeInstance.TryCreate(program, environment);

        if (instance is null)
        {
            return VmExecutionStep.ContractViolation(VmReason.UnsatisfiedHostAssumption);
        }

        environment.Meter.ReportRetained(
            VmBudgetDimension.LiveBytes,
            (ulong)(instance.Operands.Length + instance.Bindings.Length) * 8);

        return VmExecutionStep.Instantiated(instance, null);
    }

    /// <summary>Calls one emitted entry point and turns what it answers into a step.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE CALL IS A <c>calli</c> THROUGH A FUNCTION POINTER AND IT TAKES THE GARBAGE-COLLECTOR
    /// TRANSITION.</b> Suppressing the transition would leave this thread cooperative for the whole
    /// of an emitted run, which blocks every collection for its duration and deadlocks any
    /// collection attempted during it - a thread in cooperative mode inside code with no safe
    /// points never reaches one. The transition costs a few nanoseconds per invocation and is what
    /// makes an emitted frame preemptible.
    /// </para>
    /// <para>
    /// <b>The meter is charged once, afterwards, for what the counter says was spent.</b> That is
    /// the coarser accounting an emitted form can do; it is not the interpreter's and it is not
    /// pretending to be.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=09609E
    // Broiler-Falsified-If: a value is reported that the emitted code did not leave in the frame's first operand slot
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Invoke(
        VmProfileId profileId, JsNativeInstance instance, in VmInvocationRequest request)
    {
        var name = System.Text.Encoding.UTF8.GetString(request.EntryPoint.Utf8);

        // THE DRAIN IS ANSWERED AND NOT REFUSED, AND THE REASON IS THE MANIFEST RATHER THAN A
        // CONVENIENCE. A host that drives an event loop asks for `#drain-jobs` after the last
        // script, because it cannot know whether the guest queued anything. A native artifact is
        // compiled under `broiler.javascript.numeric`, which admits no promise, no job and no queue
        // to drain - so the honest answer to "run whatever is queued" is that nothing is, and that
        // is a completion rather than a missing entry point. Refusing it instead would make every
        // ordinary host fail on its last call after the program had already produced the right
        // answer, which is what this arm was written against.
        //
        // IT REPORTS ZERO PENDING JOBS BECAUSE ZERO IS THE TRUE COUNT, not because the count is
        // unavailable. A form that could queue work and reported zero here would be lying; this one
        // cannot queue work at all, and the manifest is what makes that a property rather than an
        // observation.
        if (string.Equals(name, JavaScriptProfile.DrainEntryPoint, System.StringComparison.Ordinal))
        {
            // IT ANSWERS THE SAME PAYLOAD SHAPE THE INTERPRETER'S DRAIN ANSWERS, and that is a
            // requirement rather than a courtesy: a host reads one payload type for a completion,
            // so a form that answered a different one here would make the drain a host defect on
            // the last call of every otherwise-correct program. The interpreter's drain completes
            // with `undefined`; so does this one, for the stronger reason that there was never
            // anything to run.
            return VmExecutionStep.Completed(new JsCompletion(profileId, "undefined", "undefined"));
        }

        if (!instance.Program.TryFindEntry(name, out var unit))
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "entry point is not defined", "ReferenceError"));
        }

        if (!TryFindSymbol(instance.Program, unit, out var offset))
        {
            // AN ARTIFACT THAT REACHED HERE WITHOUT A SYMBOL FOR ONE OF ITS UNITS IS ONE THE
            // VERIFIER SHOULD HAVE REFUSED, because the symbol table must name every code unit.
            // Answering a contract violation rather than jumping into the blob is the difference
            // between reporting a defect and running one.
            return VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);
        }

        instance.InvocationCount++;
        instance.Fuel[0] = JsNativeInstance.FuelPerInvocation;

        int answer;
        double result;

        fixed (double* operands = instance.Operands)
        fixed (double* bindings = instance.Bindings)
        fixed (double* constants = instance.Constants)
        fixed (long* fuel = instance.Fuel)
        {
            var frame = new JsNativeFrame
            {
                Operands = operands,
                OperandCount = instance.Operands.Length,
                Locals = bindings,
                LocalCount = instance.Bindings.Length,
                Constants = constants,
                Fuel = fuel,
                BailoutPc = 0,
            };

            answer = instance.Page.Entry(offset)(&frame);
            result = instance.Operands[0];
        }

        var spent = (ulong)(JsNativeInstance.FuelPerInvocation - instance.Fuel[0]);

        if (!instance.Environment.Meter.TryCharge(VmBudgetDimension.Fuel, spent))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        return (JsNativeReturn)answer switch
        {
            JsNativeReturn.Returned => VmExecutionStep.Completed(
                new JsCompletion(profileId, Render(result), TypeOf(result))),

            JsNativeReturn.Threw => VmExecutionStep.Faulted(
                new JsUncaught(
                    profileId,
                    "ReferenceError: Cannot access a binding before initialization",
                    "ReferenceError")),

            _ => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
        };
    }

    /// <summary>Where one code unit's emitted code starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=B97A49
    // Broiler-Falsified-If: an offset is returned for a unit the symbol table does not name
    // Broiler-Human:        PENDING
    private static bool TryFindSymbol(JsProgram program, uint unit, out uint offset)
    {
        foreach (var symbol in program.NativeSymbols)
        {
            if (symbol.FunctionIndex == unit)
            {
                offset = symbol.Offset;
                return true;
            }
        }

        offset = 0;
        return false;
    }

    /// <summary>How a slot of the emitted form is written out.</summary>
    /// <remarks>
    /// <b>THE ONE RESERVED PATTERN THAT CAN REACH HERE IS <c>undefined</c>, and it is written as
    /// <c>undefined</c> rather than as a NaN.</b> A slab of doubles has one bit pattern for it, and
    /// a completion that reported "NaN" where the interpreter reports "undefined" would be a wrong
    /// answer for the sake of not looking at the bits. The uninitialised pattern cannot reach here:
    /// emitted code answers a throw rather than returning it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=A768C5
    // Broiler-Falsified-If: a completion of undefined is reported as a Number, or a Number as undefined
    // Broiler-Human:        PENDING
    private static string Render(double value) =>
        JsNativeValues.Bits(value) == JsNativeValues.UndefinedBits
            ? "undefined"
            : JsNumberFormat.ToJsString(value);

    /// <summary>What <c>typeof</c> answers for a slot of the emitted form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=0FC6E8
    // Broiler-Falsified-If: this disagrees with what the interpreter's typeof answers for the same value
    // Broiler-Human:        PENDING
    private static string TypeOf(double value) =>
        JsNativeValues.Bits(value) == JsNativeValues.UndefinedBits ? "undefined" : "number";
}
