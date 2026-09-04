// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           17
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  5/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>The completion value an entry point finished with.</summary>
/// <remarks>
/// The core never names this type, never calls a member on it and inspects nothing about it except
/// the identity every payload carries. A consumer reaches the concrete type through this profile's
/// own static accessor, which is the projection shape the contract specifies - and which is why
/// adding a language feature never adds a case to a core result enum.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=232052
// Broiler-Human:        PENDING
public sealed class JavaScriptCompletion : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0EBA4C
    // Broiler-Human:        PENDING
    internal JavaScriptCompletion(VmProfileId profileId, JavaScriptValue value)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.CompletionKindId, 1);
        Value = value;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>The value the entry point returned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4391BB
    // Broiler-Human:        PENDING
    public JavaScriptValue Value { get; }
}

/// <summary>The error kinds this profile's language faults can be.</summary>
/// <remarks>
/// A closed vocabulary rather than a string, because the kind is what a caller branches on and a
/// message is what a person reads. The slice surface reaches exactly one of them; the rest are
/// declared here so that a later manifest widens a vocabulary rather than inventing one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DDD5C8
// Broiler-Human:        PENDING
public enum JavaScriptErrorKind
{
    /// <summary>A name was resolved and nothing was bound to it.</summary>
    ReferenceError = 1,

    /// <summary>An operation was applied to a value of the wrong type.</summary>
    TypeError = 2,

    /// <summary>A value was outside the range an operation admits.</summary>
    RangeError = 3,
}

/// <summary>
/// A language-defined fault: a JavaScript error, carried as a typed payload.
/// </summary>
/// <remarks>
/// <b>A language throw is not a core category.</b> The core's categories describe what happened to
/// the operation, not what the program computed, so a JavaScript error rides behind the
/// profile-neutral fault answer as a payload this profile owns. The core acquires no case for
/// <c>ReferenceError</c> by hosting a profile that has one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8003C6
// Broiler-Human:        PENDING
public sealed class JavaScriptFault : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9A5B20
    // Broiler-Human:        PENDING
    internal JavaScriptFault(VmProfileId profileId, JavaScriptErrorKind kind, string message)
    {
        Identity = new VmPayloadIdentity(profileId, JavaScriptProfile.FaultKindId, 1);
        Kind = kind;
        Message = message;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>Which error this is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7EEA51
    // Broiler-Human:        PENDING
    public JavaScriptErrorKind Kind { get; }

    /// <summary>
    /// What this profile calls the fault.
    /// </summary>
    /// <remarks>
    /// Roadmap section 6 lists the contents and format of error messages among the surfaces the
    /// specification leaves to the implementation. This profile <b>declares message text varying
    /// and excludes it from the retained corpus by name</b>; a corpus entry pins the error KIND
    /// and never this string.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DFE792
    // Broiler-Human:        PENDING
    public string Message { get; }
}

/// <summary>
/// The mutable per-instance state: one realm's worth of locals over one verified program.
/// </summary>
/// <remarks>
/// A realm is this profile's object and not the core's, and at the slice surface a realm is a
/// frame of locals and nothing else - there are no intrinsics to hold because there is no object
/// model. JS-4 and JS-13 give this type its real contents; what matters here is that it is
/// per-instance, that nothing in it is reachable from the verified state, and that two instances
/// over one shared handle observe nothing of each other.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E818FA
// Broiler-Falsified-If: two instances over one shared handle observe each other's locals, or any instance state is reachable from that handle
// Broiler-Human:        PENDING
public sealed class JavaScriptInstance : IVmInstanceState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D18231
    // Broiler-Human:        PENDING
    internal JavaScriptInstance(JavaScriptProgram program)
    {
        Program = program;
        Locals = new JavaScriptValue[program.LocalCount];

        // Every local starts as `undefined`, which is what `var` does in the language: a hoisted
        // binding exists from the moment its scope is entered and holds `undefined` until an
        // assignment runs. That is what lets the lowering stop emitting a block's statements after
        // one control cannot pass - a `var` whose initialiser is unreachable reads exactly what
        // the language says it should, because these writes already happened.
        //
        // THIS COMMENT SAID THE SLICE HAS NO TEMPORAL DEAD ZONE, which stopped being true when the
        // format grew an instruction that can fail. A lexical binding read or written before its
        // initialiser is a ReferenceError now, and it is the lowering's guard that raises it - not
        // the value in the slot, which is why every slot may still start as `undefined` here.
        for (var index = 0; index < Locals.Length; index++)
        {
            Locals[index] = JavaScriptValue.Undefined;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6D2F76
    // Broiler-Human:        PENDING
    internal JavaScriptProgram Program { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=28F6C6
    // Broiler-Human:        PENDING
    internal JavaScriptValue[] Locals { get; }

    /// <summary>How many times this instance has been invoked.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D7D16B
    // Broiler-Human:        PENDING
    public int InvocationCount { get; internal set; }
}

/// <summary>
/// A captured suspension of this profile.
/// </summary>
/// <remarks>
/// <b>The slice surface produces none, and this type is not a stub pretending otherwise.</b> It
/// exists because the contract has seven profile-facing types and a profile implements all of
/// them; it carries the frame state a resume would need, so that JS-7 fills a shape that is
/// already the right one rather than inventing it late. No path in this milestone constructs one,
/// the ledger says so, and <see cref="JavaScriptExecutor.Resume"/> answers a contract violation
/// rather than pretending to resume - because a resume here can only be the core handing back a
/// continuation it was never given.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=38E70F
// Broiler-Falsified-If: this milestone constructs one, or a resume presented with one is answered as anything but a contract violation
// Broiler-Human:        PENDING
public sealed class JavaScriptContinuation : IVmProfileContinuation
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50E926
    // Broiler-Human:        PENDING
    internal JavaScriptContinuation(int resumeOffset, JavaScriptValue[] operandStack, int operandStackDepth)
    {
        ResumeOffset = resumeOffset;
        OperandStack = operandStack;
        OperandStackDepth = operandStackDepth;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=890F70
    // Broiler-Human:        PENDING
    internal int ResumeOffset { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7350F4
    // Broiler-Human:        PENDING
    internal JavaScriptValue[] OperandStack { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A33221
    // Broiler-Human:        PENDING
    internal int OperandStackDepth { get; }
}

/// <summary>
/// The JavaScript profile's per-runtime executor: one interpreter over the verified program.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every answer is one of the five step kinds and no profile code names a core outcome
/// category.</b> A language error is a typed payload behind the fault answer; a defect in this
/// profile is a contract violation; and the two are never each other, so that a defect here is
/// never reported as a defect in the guest program.
/// </para>
/// <para>
/// <b>No exception escapes.</b> The dispatch loop performs no operation that throws: arithmetic is
/// IEEE and total, division by zero is <c>Infinity</c> rather than a trap, and every index it uses
/// was proved in range at verification. The one place the language would throw - resolving a name
/// that is not bound - is answered as a typed fault.
/// </para>
/// <para>
/// <b>Fuel and polling are placed, not sprinkled.</b> One fuel unit and one poll per instruction,
/// which is well inside the uncharged-work bound the descriptor declares. JS-5 replaces this with
/// measured numbers and the proportional families roadmap section 8 names; at the slice there is
/// no operation whose cost grows with its input, which is why this milestone ships no
/// proportionality fixture and says so rather than shipping a flat charge that looks like one.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E9D7AE
// Broiler-Falsified-If: any input makes a member here throw, or an answer is produced that is not one of the five step kinds
// Broiler-Human:        PENDING
public sealed class JavaScriptExecutor : IVmProfileExecutor
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DBD8E9
    // Broiler-Human:        PENDING
    private readonly IVmExecutionEnvironment environment;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FC50CA
    // Broiler-Human:        PENDING
    internal JavaScriptExecutor(VmProfileId profileId, IVmExecutionEnvironment executionEnvironment)
    {
        ProfileId = profileId;
        environment = executionEnvironment;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCA6CF
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F2CD86
    // Broiler-Falsified-If: a handle this profile did not verify produces an instance
    // Broiler-Human:        PENDING
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken)
    {
        if (artifact.TryGetState(out var wide) && wide is JsProgram wideProgram)
        {
            return JsExecution.Instantiate(wideProgram, environment, cancellationToken);
        }

        if (!artifact.TryGetState(out var state) || state is not JavaScriptProgram program)
        {
            // A handle this profile did not produce, or one that has been disposed. Either way it
            // is not something to run, and saying so is a contract violation rather than a fault:
            // no guest program caused it.
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        var instance = new JavaScriptInstance(program);

        // Retained rather than consumed: a realm's locals live for the instance's lifetime and are
        // released when it is disposed, which is what makes LiveBytes a ceiling and not an
        // allowance.
        environment.Meter.ReportRetained(
            VmBudgetDimension.LiveBytes, (ulong)program.LocalCount * OneValueInBytes);

        return VmExecutionStep.Instantiated(instance, null);
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=59C786
    // Broiler-Falsified-If: an unknown entry point is reported as anything but a language fault, or a foreign instance state runs
    // Broiler-Human:        PENDING
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken)
    {
        if (state is JsInstance wide)
        {
            return JsExecution.Invoke(ProfileId, wide, in request);
        }

        if (state is not JavaScriptInstance instance)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        if (!instance.Program.TryFindEntry(request.EntryPoint.Utf8, out var offset))
        {
            // Resolving a name and finding nothing bound to it is a ReferenceError in the
            // language, and what an entry-point name means is this profile's business - the core
            // carried the bytes without decoding them. So it is a language fault and not a core
            // answer.
            return VmExecutionStep.Faulted(
                new JavaScriptFault(ProfileId, JavaScriptErrorKind.ReferenceError, "entry point is not defined"));
        }

        instance.InvocationCount++;
        return Run(instance, offset);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The slice surface never suspends, so a resume can only be the core handing back a
    /// continuation it was never given. Answering with a contract violation rather than pretending
    /// to resume is what keeps a defect in the core or in this profile from being reported as a
    /// completed operation. JS-7 replaces this body.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=980DF9
    // Broiler-Human:        PENDING
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);

    /// <inheritdoc/>
    /// <remarks>
    /// Nothing to unwind: this surface parks nowhere, holds no resource across a step, and has no
    /// <c>finally</c> block because it has no exception regions. JS-7 is where this stops being
    /// empty, and roadmap section 12 fixes what it must then do - run under the tighter of the two
    /// budgets and run no guest code able to request a load or to suspend.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5DBDBE
    // Broiler-Human:        PENDING
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }

    /// <summary>What one operand slot costs, for the retention report.</summary>
    /// <remarks>
    /// A figure this profile states about its own representation rather than measures, and JSD-0011
    /// decides the representation that replaces it at JS-4. It is used only to report retention,
    /// never to size anything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BA2AAF
    // Broiler-Human:        PENDING
    private const ulong OneValueInBytes = 16;

    /// <summary>The interpreter.</summary>
    /// <remarks>
    /// The stack is sized from the maximum <b>verification</b> computed and stored, never from a
    /// number the payload chose. Every index the loop uses - constant, local, jump target - was
    /// proved in range before this method could be reached, which is why no bound is re-checked
    /// here and why re-checking one would be the tell that the boundary is not trusted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=1C127C
    // Broiler-Falsified-If: the operand stack is sized from anything but the maximum the verifier computed, or an index used here was not proved in range before execution
    // Broiler-Human:        PENDING
    private VmExecutionStep Run(JavaScriptInstance instance, int offset)
    {
        var program = instance.Program;
        var code = program.Code;
        var constants = program.Constants;
        var locals = instance.Locals;
        var stack = new JavaScriptValue[program.MaximumOperandStack];
        var top = 0;

        while (true)
        {
            if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
            {
                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
            }

            if (!environment.Meter.Poll())
            {
                return VmExecutionStep.ContractViolation(VmReason.Cancelled);
            }

            var opcode = (JavaScriptOpcode)code[offset];
            var operandAt = offset + 1;
            offset += JavaScriptOpcodes.InstructionWidth(opcode);

            switch (opcode)
            {
                case JavaScriptOpcode.LoadConstant:
                    stack[top++] = constants[ReadIndex(code, operandAt)];
                    continue;

                case JavaScriptOpcode.LoadLocal:
                    stack[top++] = locals[ReadIndex(code, operandAt)];
                    continue;

                case JavaScriptOpcode.StoreLocal:
                    locals[ReadIndex(code, operandAt)] = stack[--top];
                    continue;

                // THE TEMPORAL DEAD ZONE. The lowering puts this immediately before the
                // `LoadLocal` or `StoreLocal` that would have touched a lexical binding whose
                // initialiser has not run, and the frame is abandoned before that instruction is
                // reached. Reading such a slot used to answer `undefined`, which is what the
                // language reserves for a binding that HAS been initialised and holds nothing;
                // writing one used to succeed.
                //
                // The message says USED rather than read, because both halves throw here: `x; let
                // x;` and `x = 1; let x;` are each a ReferenceError. It names no binding because it
                // cannot - naming one needs an interned name and the constant pool's interned-name
                // tag is admitted by no manifest yet - and the position table carries the line.
                case JavaScriptOpcode.ThrowUninitializedBinding:
                    return VmExecutionStep.Faulted(
                        new JavaScriptFault(
                            ProfileId,
                            JavaScriptErrorKind.ReferenceError,
                            "a binding was used before its initialiser ran"));

                case JavaScriptOpcode.Add:
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToNumber() + stack[top - 1].ToNumber());
                    top--;
                    continue;

                case JavaScriptOpcode.Subtract:
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToNumber() - stack[top - 1].ToNumber());
                    top--;
                    continue;

                case JavaScriptOpcode.Multiply:
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToNumber() * stack[top - 1].ToNumber());
                    top--;
                    continue;

                case JavaScriptOpcode.Divide:
                    // No guard, and the absence is the language. Division by zero is an infinity
                    // and zero over zero is NaN; a profile that faulted here would be answering
                    // for a language that is not this one.
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToNumber() / stack[top - 1].ToNumber());
                    top--;
                    continue;

                case JavaScriptOpcode.Rem:
                    // The remainder operator, whose result takes the sign of the DIVIDEND. The
                    // double operator answers that; a floored modulo would not, and -5 % 3 is the
                    // case that tells them apart.
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToNumber() % stack[top - 1].ToNumber());
                    top--;
                    continue;

                case JavaScriptOpcode.Negate:
                    stack[top - 1] = JavaScriptValue.Number(-stack[top - 1].ToNumber());
                    continue;

                case JavaScriptOpcode.ToNumber:
                    // Unary plus is a conversion and not a no-op: applied to `true` it produces 1
                    // and applied to `undefined` it produces NaN, and both change the kind.
                    stack[top - 1] = JavaScriptValue.Number(stack[top - 1].ToNumber());
                    continue;

                case JavaScriptOpcode.Not:
                    stack[top - 1] = JavaScriptValue.Boolean(!stack[top - 1].ToBooleanValue());
                    continue;

                case JavaScriptOpcode.LessThan:
                    stack[top - 2] = JavaScriptValue.Boolean(
                        JavaScriptValue.LessThan(stack[top - 2], stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.LessThanOrEqual:
                    stack[top - 2] = JavaScriptValue.Boolean(
                        JavaScriptValue.LessThanOrEqual(stack[top - 2], stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.GreaterThan:
                    stack[top - 2] = JavaScriptValue.Boolean(
                        JavaScriptValue.GreaterThan(stack[top - 2], stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.GreaterThanOrEqual:
                    stack[top - 2] = JavaScriptValue.Boolean(
                        JavaScriptValue.GreaterThanOrEqual(stack[top - 2], stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.StrictEquals:
                    stack[top - 2] = JavaScriptValue.Boolean(stack[top - 2].StrictlyEquals(stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.StrictNotEquals:
                    stack[top - 2] = JavaScriptValue.Boolean(!stack[top - 2].StrictlyEquals(stack[top - 1]));
                    top--;
                    continue;

                case JavaScriptOpcode.BitwiseOr:
                    stack[top - 2] = JavaScriptValue.Number(stack[top - 2].ToInt32() | stack[top - 1].ToInt32());
                    top--;
                    continue;

                case JavaScriptOpcode.BitwiseAnd:
                    stack[top - 2] = JavaScriptValue.Number(stack[top - 2].ToInt32() & stack[top - 1].ToInt32());
                    top--;
                    continue;

                case JavaScriptOpcode.BitwiseXor:
                    stack[top - 2] = JavaScriptValue.Number(stack[top - 2].ToInt32() ^ stack[top - 1].ToInt32());
                    top--;
                    continue;

                case JavaScriptOpcode.ShiftLeft:
                    // The shift count is masked to five bits by the language, not by the platform.
                    // C#'s own shift on int masks to 31 as well, but relying on that would be
                    // relying on an agreement rather than implementing a rule.
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToInt32() << (int)(stack[top - 1].ToUint32() & 31));
                    top--;
                    continue;

                case JavaScriptOpcode.ShiftRight:
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToInt32() >> (int)(stack[top - 1].ToUint32() & 31));
                    top--;
                    continue;

                case JavaScriptOpcode.ShiftRightUnsigned:
                    // Zero-filling, over ToUint32, and the result is a Number that may exceed
                    // int.MaxValue - which is why it is widened through uint and not through int.
                    stack[top - 2] = JavaScriptValue.Number(
                        stack[top - 2].ToUint32() >> (int)(stack[top - 1].ToUint32() & 31));
                    top--;
                    continue;

                case JavaScriptOpcode.Jump:
                    offset += ReadDisplacement(code, operandAt);
                    continue;

                case JavaScriptOpcode.JumpIfFalse:
                    if (!stack[--top].ToBooleanValue())
                    {
                        offset += ReadDisplacement(code, operandAt);
                    }

                    continue;

                case JavaScriptOpcode.JumpIfTrue:
                    if (stack[--top].ToBooleanValue())
                    {
                        offset += ReadDisplacement(code, operandAt);
                    }

                    continue;

                case JavaScriptOpcode.Pop:
                    top--;
                    continue;

                case JavaScriptOpcode.Duplicate:
                    stack[top] = stack[top - 1];
                    top++;
                    continue;

                default:
                    // Return, and nothing else can reach here: verification refused every byte
                    // that is not an opcode this switch has an arm for.
                    return VmExecutionStep.Completed(new JavaScriptCompletion(ProfileId, stack[--top]));
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B8A814
    // Broiler-Human:        PENDING
    private static int ReadIndex(byte[] code, int at) =>
        System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(
            System.MemoryExtensions.AsSpan(code, at, 2));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4D5454
    // Broiler-Human:        PENDING
    private static int ReadDisplacement(byte[] code, int at) =>
        System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(
            System.MemoryExtensions.AsSpan(code, at, 4));
}
