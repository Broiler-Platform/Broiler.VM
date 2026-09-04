// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           10
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>What a wide-surface entry point completed with.</summary>
/// <remarks>
/// It carries rendered text rather than the engine's own value type, and that is deliberate: a
/// host prints a completion value and branches on nothing about it, so exposing the value model
/// through the payload would put a representation this milestone calls provisional into the public
/// surface, where changing it would be a breaking change rather than an edit.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F93113
// Broiler-Human:        PENDING
public sealed class JsCompletion : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=79266F
    // Broiler-Human:        PENDING
    internal JsCompletion(VmProfileId profileId, string rendered, string typeOf)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.WideCompletionKindId, 1);
        Value = rendered;
        TypeOf = typeOf;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>The completion value, rendered the way the language renders it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7C00DF
    // Broiler-Human:        PENDING
    public string Value { get; }

    /// <summary>What <c>typeof</c> answers for the completion value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C2A992
    // Broiler-Human:        PENDING
    public string TypeOf { get; }
}

/// <summary>A JavaScript exception that reached the top of an entry point.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C353E6
// Broiler-Human:        PENDING
public sealed class JsUncaught : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BE664E
    // Broiler-Human:        PENDING
    internal JsUncaught(VmProfileId profileId, string rendered, string name)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.WideFaultKindId, 1);
        Message = rendered;
        ErrorName = name;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>One line describing what was thrown.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DFE792
    // Broiler-Human:        PENDING
    public string Message { get; }

    /// <summary>
    /// The <c>name</c> of the thrown value's constructor, which is what a conformance suite's
    /// negative expectation is matched against.
    /// </summary>
    /// <remarks>
    /// Empty when the thrown value is not an object with a constructor - throwing a string is
    /// legal and a runner has to be able to tell the two cases apart.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=05F22E
    // Broiler-Human:        PENDING
    public string ErrorName { get; }
}

/// <summary>
/// One instance of a wide-surface program: a realm, and the engine that runs in it.
/// </summary>
/// <remarks>
/// <b>The realm outlives one invocation, and that is what makes several scripts one program.</b>
/// The conformance suite requires its harness files to be separate scripts evaluated in the test's
/// realm; the artifact carries one entry point per script and the host invokes them in order
/// against this one instance, so the second sees what the first declared. A fresh instance is a
/// fresh realm.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=792E06
// Broiler-Human:        PENDING
internal sealed class JsInstance : IVmInstanceState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4D1751
    // Broiler-Human:        PENDING
    internal JsInstance(JsProgram program, JsEngine engine)
    {
        Program = program;
        Engine = engine;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9D1393
    // Broiler-Human:        PENDING
    internal JsProgram Program { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=295C08
    // Broiler-Human:        PENDING
    internal JsEngine Engine { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=218248
    // Broiler-Human:        PENDING
    internal int InvocationCount { get; set; }
}

/// <summary>The wide surface's half of the executor: instantiate, invoke, and report.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E1ACB5
// Broiler-Human:        PENDING
internal static class JsExecution
{
    /// <summary>Builds an instance and its realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D1E94E
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Instantiate(
        JsProgram program,
        IVmExecutionEnvironment environment,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var engine = new JsEngine(environment.Meter, cancellationToken, environment.Capabilities);
        var instance = new JsInstance(program, engine);

        // A realm is the largest thing this profile retains, and it is retained rather than
        // consumed: it lives for the instance's lifetime and is released with it.
        environment.Meter.ReportRetained(VmBudgetDimension.LiveBytes, 262_144);

        return VmExecutionStep.Instantiated(instance, null);
    }

    /// <summary>Runs one entry point against an existing realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D2CB5B
    // Broiler-Human:        PENDING
    internal static VmExecutionStep Invoke(
        VmProfileId profileId, JsInstance instance, in VmInvocationRequest request)
    {
        var name = System.Text.Encoding.UTF8.GetString(request.EntryPoint.Utf8);

        if (!instance.Program.TryFindEntry(name, out var unit))
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "entry point is not defined", "ReferenceError"));
        }

        instance.InvocationCount++;

        try
        {
            var value = RunOnGuestStack(instance, unit);

            return VmExecutionStep.Completed(
                new JsCompletion(
                    profileId, instance.Engine.ToStringValue(value), value.TypeOf()));
        }
        catch (JsThrow thrown)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(
                    profileId, instance.Engine.Render(thrown.Value), ConstructorName(instance, thrown.Value)));
        }
        catch (JsAbort abort)
        {
            return abort.Kind switch
            {
                JsAbortKind.Cancelled => VmExecutionStep.ContractViolation(VmReason.Cancelled),
                JsAbortKind.Exhausted => VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted),
                _ => VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation),
            };
        }
        catch (System.InsufficientExecutionStackException)
        {
            return VmExecutionStep.Faulted(
                new JsUncaught(profileId, "Maximum call stack size exceeded", "RangeError"));
        }
    }

    /// <summary>
    /// How much native stack one guest invocation gets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The profile chooses the stack the guest runs on, because otherwise the call-depth ceiling
    /// means a different thing on every host.</b> This interpreter recurses once per JavaScript
    /// call, and a frame of it is over a kilobyte; on the one-megabyte stack a Windows process
    /// hands its main thread, a program recursing 776 deep ended the PROCESS rather than the
    /// operation - and a stack overflow is the one failure the CLR cannot turn into an exception,
    /// so no meter, no ceiling and no probe can report it after the fact.
    /// </para>
    /// <para>
    /// <b>Sixteen megabytes is chosen against the declared ceiling and not against a benchmark.</b>
    /// The call-depth allowance a host may grant is bounded by the descriptor's own maximum, and a
    /// frame of this interpreter plus the two frames beneath it cost about four kilobytes; the
    /// figure leaves room for the built-ins that recurse in C# without going through a call - a
    /// comparison function driving a sort, a cycle-free walk of a deep object in JSON - and for the
    /// stack a host has already used before it reached this profile.
    /// </para>
    /// <para>
    /// <b>A thread per invocation rather than one per instance.</b> An instance can outlive many
    /// invocations, and a thread parked between them would be a resource this profile holds while
    /// doing nothing; a fresh one costs a fraction of a millisecond against a program that runs for
    /// milliseconds at least, and it starts every invocation with the same stack whatever the
    /// previous one did.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4A5981
    // Broiler-Human:        PENDING
    private const int GuestStackBytes = 16 * 1024 * 1024;

    /// <summary>Runs one entry point on a thread whose stack this profile declared.</summary>
    /// <remarks>
    /// Whatever the guest raises is carried back and rethrown here, so the caller sees the same
    /// exception it would have seen had the interpreter run on its own thread. The profile declares
    /// <c>Agile</c> thread affinity, so the core pins no operation to a thread, and the one host
    /// capability this profile imports declares caller-thread affinity - which this satisfies: the
    /// thread that calls it is the thread the guest is running on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0DA764
    // Broiler-Falsified-If: guest code runs on the caller's stack, or an exception the guest raised does not reach the caller
    // Broiler-Human:        PENDING
    private static JsValue RunOnGuestStack(JsInstance instance, uint unit)
    {
        var completed = JsValue.Undefined;
        System.Runtime.ExceptionServices.ExceptionDispatchInfo? raised = null;

        var worker = new System.Threading.Thread(
            () =>
            {
                try
                {
                    completed = instance.Engine.RunEntry(instance.Program, unit);
                }
                catch (System.Exception failure)
                {
                    raised = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure);
                }
            },
            GuestStackBytes)
        {
            IsBackground = true,
            Name = "broiler-js-guest",
        };

        worker.Start();
        worker.Join();
        raised?.Throw();
        return completed;
    }

    /// <summary>
    /// The name of the thrown value's constructor, which is what a negative expectation matches.
    /// </summary>
    /// <remarks>
    /// It is read through the ordinary property path rather than off a C# type, because the value
    /// a program throws may be an object of its own whose constructor is a guest function - which
    /// is exactly the case the conformance suite's own <c>Test262Error</c> is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4CF0BB
    // Broiler-Human:        PENDING
    private static string ConstructorName(JsInstance instance, JsValue value)
    {
        if (!value.IsObject)
        {
            return string.Empty;
        }

        try
        {
            var constructor = instance.Engine.GetProperty(value, "constructor");

            if (!constructor.IsObject)
            {
                return string.Empty;
            }

            var name = instance.Engine.GetProperty(constructor, "name");
            return name.IsString ? name.AsString() : string.Empty;
        }
        catch (JsThrow)
        {
            return string.Empty;
        }
        catch (JsAbort)
        {
            return string.Empty;
        }
    }
}
