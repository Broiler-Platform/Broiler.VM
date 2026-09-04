// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   45
// Annotated:        45/45
// Exempt:           8
// Human-reviewed:   0/45
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/1
// Resource impact:  5/10 max
// Unverified:       45
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The wide-surface engine: one realm, the abstract operations over it, and the dispatch loop.
/// </summary>
/// <remarks>
/// <para>
/// <b>One class holds the operations and the loop because they call each other in both
/// directions.</b> <c>ToPrimitive</c> calls <c>valueOf</c>, which may be a bytecode function, which
/// runs on the loop, which calls <c>ToPrimitive</c>. Splitting them would mean an interface between
/// two halves of one thing, and the interface would be a delegate field on each side.
/// </para>
/// <para>
/// <b>Fuel is charged per instruction and per call.</b> Not per second: two runs of the same
/// program on two machines stop at the same instruction. A built-in that does bounded work charges
/// once; a built-in whose work is proportional to an argument - sorting, joining, matching - charges
/// proportionally, so a program cannot buy unbounded work with one instruction.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3E740C
// Broiler-Human:        PENDING
internal sealed class JsEngine
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A41ED2
    // Broiler-Human:        PENDING
    private const int FuelPerInstruction = 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F04339
    // Broiler-Human:        PENDING
    private readonly System.Threading.CancellationToken cancellation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=329437
    // Broiler-Human:        PENDING
    private int depth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2AB034
    // Broiler-Human:        PENDING
    private ulong sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2E76EA
    // Broiler-Human:        PENDING
    private readonly IVmHostCapabilityInvoker? capabilities;

    /// <summary>Creates an engine over a fresh realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=10A074
    // Broiler-Human:        PENDING
    internal JsEngine(
        IVmMeter contractMeter,
        System.Threading.CancellationToken token,
        IVmHostCapabilityInvoker? invoker = null)
    {
        meter = contractMeter;
        cancellation = token;
        capabilities = invoker;
        Realm = new JsRealm(this);
    }

    /// <summary>
    /// Writes one line of text to whatever the composition registered, or nowhere.
    /// </summary>
    /// <remarks>
    /// The two sinks are deliberately different things. <see cref="Output"/> is an in-process hook
    /// a test host sets to capture what a program printed; the capability is the host boundary a
    /// real composition registers. A program that prints reaches both when both exist and neither
    /// when neither does, and in no case does it reach a console this profile opened itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5B318E
    // Broiler-Human:        PENDING
    internal void Write(string text)
    {
        Output?.Invoke(text);

        if (capabilities is null ||
            capabilities.BindingCount <= JavaScriptProfile.WriteBindingIndex ||
            !capabilities.IsBound(JavaScriptProfile.WriteBindingIndex))
        {
            return;
        }

        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        capabilities.InvokeBytes(
            JavaScriptProfile.WriteBindingIndex, new VmBytes(bytes), out _);
    }

    /// <summary>The realm this engine runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B48D28
    // Broiler-Human:        PENDING
    internal JsRealm Realm { get; }

    /// <summary>Whatever the host wired to <c>print</c>, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EED672
    // Broiler-Human:        PENDING
    internal System.Action<string>? Output { get; set; }

    /// <summary>
    /// The deepest the call stack may go before a <c>RangeError</c> is thrown.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the backstop and not the ordinary answer.</b> A recursing program is refused
    /// first by the <c>CallDepth</c> budget, whose default is lower than this and whose exhaustion
    /// is a resource exhaustion the guest cannot catch - which is what roadmap section 8 asks for
    /// in those words. This bound exists for the case that ceiling does not cover: a host may grant
    /// a call depth up to the profile's own declared maximum, which is far larger than the native
    /// stack a guest invocation runs on can hold.
    /// </para>
    /// <para>
    /// <b>It is a counted number and not a stack probe</b>, because a probe promises nothing under
    /// Native AOT. The figure is chosen against the stack this profile declares for one guest
    /// invocation and the cost of one interpreter frame; the margin is stated where that stack size
    /// is declared.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=B5AF10
    // Broiler-Falsified-If: a program recursing past this bound terminates the process rather than ending the operation
    // Broiler-Human:        PENDING
    internal int MaximumCallDepth { get; set; } = 3000;

    // ---- metering ------------------------------------------------------------------------------

    /// <summary>
    /// How much work this engine performs between two polls.
    /// </summary>
    /// <remarks>
    /// It is HALF the profile's declared cancellation poll bound, and the halving is what makes the
    /// declaration true rather than nearly true: a charge is added before the poll is considered,
    /// so the most work that can accumulate between two polls is one window plus the charge that
    /// crossed it. A single charge larger than a window is split, because a built-in charging
    /// proportionally to a megabyte-long string would otherwise breach the bound in one call - which
    /// is what the RegExp benchmark did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=733ADC
    // Broiler-Human:        PENDING
    private const ulong PollWindow = 16_384;

    /// <summary>Charges fuel, aborting when the allowance is spent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4AFB70
    // Broiler-Human:        PENDING
    internal void Charge(ulong units)
    {
        while (units > PollWindow)
        {
            ChargeOnce(PollWindow);
            units -= PollWindow;
        }

        ChargeOnce(units);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8CD7C8
    // Broiler-Human:        PENDING
    private void ChargeOnce(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.Fuel, units))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the instruction allowance is spent");
        }

        sinceLastPoll += units;

        if (sinceLastPoll < PollWindow)
        {
            return;
        }

        sinceLastPoll = 0;

        if (cancellation.IsCancellationRequested)
        {
            throw new JsAbort(JsAbortKind.Cancelled, "cancellation was requested");
        }

        if (!meter.Poll())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "a budget dimension was reached");
        }
    }

    /// <summary>Reports bytes an allocation retained, so LiveBytes stays a ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2C6F27
    // Broiler-Human:        PENDING
    internal void Retain(ulong bytes) =>
        meter.ReportRetained(VmBudgetDimension.LiveBytes, bytes);

    // ---- throwing ------------------------------------------------------------------------------

    /// <summary>Throws a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D6FB0C
    // Broiler-Human:        PENDING
    internal JsValue ThrowTypeError(string message) => throw Error("TypeError", message);

    /// <summary>Throws a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=92484D
    // Broiler-Human:        PENDING
    internal JsValue ThrowRangeError(string message) => throw Error("RangeError", message);

    /// <summary>Throws a <c>ReferenceError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=97EC2F
    // Broiler-Human:        PENDING
    internal JsValue ThrowReferenceError(string message) => throw Error("ReferenceError", message);

    /// <summary>Throws a <c>SyntaxError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=173F3B
    // Broiler-Human:        PENDING
    internal JsValue ThrowSyntaxError(string message) => throw Error("SyntaxError", message);

    /// <summary>Builds a throw carrying a fresh Error of the named intrinsic kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=66C1DC
    // Broiler-Human:        PENDING
    internal JsThrow Error(string kind, string message)
    {
        var error = Realm.CreateError(kind, message);
        return new JsThrow(error, kind + ": " + message);
    }

    // ---- conversions ---------------------------------------------------------------------------

    /// <summary>The abstract operation <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8D15BE
    // Broiler-Human:        PENDING
    internal JsValue ToPrimitive(JsValue value, string hint)
    {
        if (!value.IsObject)
        {
            return value;
        }

        var order = string.Equals(hint, "string", System.StringComparison.Ordinal)
            ? new[] { "toString", "valueOf" }
            : ["valueOf", "toString"];

        foreach (var name in order)
        {
            var method = GetProperty(value, name);

            if (method.IsObject && method.AsObject().IsCallable)
            {
                var result = Call(method, value, System.Array.Empty<JsValue>());

                if (!result.IsObject)
                {
                    return result;
                }
            }
        }

        return ThrowTypeError("Cannot convert object to primitive value");
    }

    /// <summary>The abstract operation <c>ToNumber</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=9D28A5
    // Broiler-Human:        PENDING
    internal double ToNumber(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Undefined => double.NaN,
        JsType.Null => 0,
        JsType.String => JsNumberFormat.ToNumber(value.AsString()),
        _ => ToNumber(ToPrimitive(value, "number")),
    };

    /// <summary>The abstract operation <c>ToString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4C1DFD
    // Broiler-Human:        PENDING
    internal string ToStringValue(JsValue value) => value.Type switch
    {
        JsType.String => value.AsString(),
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",
        JsType.Undefined => "undefined",
        JsType.Null => "null",
        _ => ToStringValue(ToPrimitive(value, "string")),
    };

    /// <summary>The abstract operation <c>ToObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2750E2
    // Broiler-Human:        PENDING
    internal JsObject ToObject(JsValue value) => value.Type switch
    {
        JsType.Object => value.AsObject(),
        JsType.String => Realm.WrapString(value.AsString()),
        JsType.Number => new JsPrimitiveWrapper(Realm.NumberPrototype, "Number", value),
        JsType.Boolean => new JsPrimitiveWrapper(Realm.BooleanPrototype, "Boolean", value),
        _ => (JsObject)ThrowTypeError("Cannot convert undefined or null to object").AsObject(),
    };

    /// <summary>The abstract operation <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1BA637
    // Broiler-Human:        PENDING
    internal int ToInt32(JsValue value) => JsValue.ToInt32(ToNumber(value));

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DBC89F
    // Broiler-Human:        PENDING
    internal uint ToUint32(JsValue value) => JsValue.ToUint32(ToNumber(value));

    /// <summary>The abstract operation <c>ToIntegerOrInfinity</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F80693
    // Broiler-Human:        PENDING
    internal double ToInteger(JsValue value) => JsValue.ToInteger(ToNumber(value));

    /// <summary>The abstract operation <c>ToPropertyKey</c>, over the string keys this surface has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7A65ED
    // Broiler-Human:        PENDING
    internal string ToPropertyKey(JsValue value) =>
        value.Type == JsType.String ? value.AsString() : ToStringValue(value);

    // ---- properties ----------------------------------------------------------------------------

    /// <summary>The prototype a primitive's property lookup starts from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4D0DC2
    // Broiler-Human:        PENDING
    private JsObject? PrototypeFor(JsValue value) => value.Type switch
    {
        JsType.String => Realm.StringPrototype,
        JsType.Number => Realm.NumberPrototype,
        JsType.Boolean => Realm.BooleanPrototype,
        _ => null,
    };

    /// <summary>Reads a property off any value, walking the prototype chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A45301
    // Broiler-Human:        PENDING
    internal JsValue GetProperty(JsValue baseValue, string key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading '" + key + "')");
        }

        if (baseValue.IsString)
        {
            var text = baseValue.AsString();

            if (string.Equals(key, "length", System.StringComparison.Ordinal))
            {
                return JsValue.Number(text.Length);
            }

            if (JsObject.IsArrayIndex(key, out var at))
            {
                return at < text.Length
                    ? JsValue.String(text[(int)at].ToString())
                    : JsValue.Undefined;
            }
        }

        var start = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);
        return start is null ? JsValue.Undefined : Lookup(start, key, baseValue);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F27834
    // Broiler-Human:        PENDING
    private JsValue Lookup(JsObject start, string key, JsValue receiver)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a property on any value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1B6117
    // Broiler-Human:        PENDING
    internal void SetProperty(JsValue baseValue, string key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting '" + key + "')");

            return;
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);
        var target = baseValue.AsObjectOrNull();

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnProperty(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The <c>in</c> operator's lookup: does any object in the chain have the key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7C48F4
    // Broiler-Human:        PENDING
    internal bool HasProperty(JsObject start, string key)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    // ---- calling -------------------------------------------------------------------------------

    /// <summary>Calls <paramref name="callee"/>, whatever kind of callable it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8B6385
    // Broiler-Human:        PENDING
    internal JsValue Call(JsValue callee, JsValue thisValue, JsValue[] arguments)
    {
        if (!callee.IsObject || !callee.AsObject().IsCallable)
        {
            return ThrowTypeError(Describe(callee) + " is not a function");
        }

        Charge(4);

        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return ThrowRangeError("Maximum call stack size exceeded");
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;

        try
        {
            switch (callee.AsObject())
            {
                case JsNativeFunction native:
                    return native.Call(this, thisValue, arguments);

                case JsBoundFunction bound:
                    return Call(
                        JsValue.Object(bound.Target),
                        bound.BoundThis,
                        Concat(bound.BoundArguments, arguments));

                case JsScriptFunction script:
                    return Invoke(script, thisValue, arguments, null);

                default:
                    return ThrowTypeError("value is not a function");
            }
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>Constructs with <paramref name="callee"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2DDACB
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments)
    {
        if (!callee.IsObject || !callee.AsObject().IsConstructor)
        {
            return ThrowTypeError(Describe(callee) + " is not a constructor");
        }

        Charge(8);
        var target = callee.AsObject();

        if (target is JsNativeFunction native)
        {
            return native.Construct(this, arguments);
        }

        if (target is JsBoundFunction bound)
        {
            return Construct(JsValue.Object(bound.Target), Concat(bound.BoundArguments, arguments));
        }

        var prototype = GetProperty(callee, "prototype");

        var instance = new JsObject(
            prototype.IsObject ? prototype.AsObject() : Realm.ObjectPrototype);

        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return ThrowRangeError("Maximum call stack size exceeded");
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;

        try
        {
            var returned = Invoke((JsScriptFunction)target, JsValue.Object(instance), arguments, null);

            // A CONSTRUCTOR THAT RETURNS AN OBJECT RETURNS THAT OBJECT, and one that returns
            // anything else returns the instance. Getting this backwards makes every factory
            // written as a constructor produce the wrong thing.
            return returned.IsObject ? returned : JsValue.Object(instance);
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=609082
    // Broiler-Human:        PENDING
    private static JsValue[] Concat(JsValue[] first, JsValue[] second)
    {
        if (first.Length == 0)
        {
            return second;
        }

        var joined = new JsValue[first.Length + second.Length];
        System.Array.Copy(first, joined, first.Length);
        System.Array.Copy(second, 0, joined, first.Length, second.Length);
        return joined;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DC0CB5
    // Broiler-Human:        PENDING
    private string Describe(JsValue value) => value.Type switch
    {
        JsType.Undefined => "undefined",
        JsType.Null => "null",
        JsType.String => "\"" + value.AsString() + "\"",
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",
        _ => value.AsObject().IsCallable ? "function" : "object",
    };

    /// <summary>Runs a program's entry point and answers what it completed with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7EF362
    // Broiler-Human:        PENDING
    internal JsValue RunEntry(JsProgram program, uint unit)
    {
        var code = program.Functions[(int)unit];
        var environment = new JsEnvironment((int)code.ScopeSlots, null);

        return Execute(
            program,
            (int)unit,
            environment,
            JsValue.Object(Realm.GlobalObject),
            System.Array.Empty<JsValue>(),
            null,
            null);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=ECD444
    // Broiler-Human:        PENDING
    private JsValue Invoke(
        JsScriptFunction function, JsValue thisValue, JsValue[] arguments, JsValue[]? ignored)
    {
        _ = ignored;
        var program = function.Program;
        var unit = program.Functions[function.Unit];
        var environment = new JsEnvironment((int)unit.ScopeSlots, function.Environment);

        var count = System.Math.Min(arguments.Length, (int)unit.ParameterCount);

        for (var at = 0; at < count; at++)
        {
            environment.Slots[at] = arguments[at];
        }

        for (var at = count; at < unit.ParameterCount; at++)
        {
            environment.Slots[at] = JsValue.Undefined;
        }

        var receiver = unit.IsArrow
            ? function.LexicalThis
            : unit.IsStrict
                ? thisValue
                : thisValue.IsNullish
                    ? JsValue.Object(Realm.GlobalObject)
                    : thisValue.IsObject
                        ? thisValue
                        : JsValue.Object(ToObject(thisValue));

        // CALLING A GENERATOR FUNCTION RUNS NONE OF ITS BODY. The environment above is built and
        // the parameters are bound - both are observable, and both happen at the call - and then
        // the frame is put on the heap and handed back inside a generator object instead of being
        // interpreted. One bit test is what an ordinary call pays for that.
        if (unit.IsGenerator)
        {
            var frame = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            // THE FRAME IS CHARGED IN PROPORTION TO ITS SIZE, which is the rule this engine already
            // applies to a built-in whose work is proportional to an argument: one instruction may
            // not buy unbounded work. A generator over a unit verified to need a deep operand stack
            // costs more to build than one over a shallow unit, and a program that builds a million
            // of them has spent a million times that.
            Charge((frame.FrameBytes / 64) + 4);
            return JsValue.Object(Realm.CreateGenerator(function, frame));
        }

        return Execute(program, function.Unit, environment, receiver, arguments, function, null);
    }

    // ---- generators ----------------------------------------------------------------------------

    /// <summary>
    /// The one entry every resumption goes through: <c>next</c>, <c>return</c> and <c>throw</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The state is decided before the frame is touched.</b> Four states and three methods make
    /// twelve cases, and eleven of them answer without running a single instruction: a completed
    /// generator answers or rethrows, a generator that has not started swallows a <c>return</c> and
    /// rethrows a <c>throw</c> without ever entering its body, and one that is already on the
    /// interpreter's stack is a <c>TypeError</c>. The twelfth is the resumption.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one - a second interpreter frame on
    /// the same native stack.</b> Fuel covers the re-entry, so driving a generator a million steps
    /// cannot buy a million frame switches for nothing; and the CALL-DEPTH dimension covers the
    /// frame, which is what makes a <c>yield*</c> chain thousands deep end in a named exhaustion
    /// rather than in a stack overflow. It is charged here and not left to the <c>next</c> call
    /// that reached this method, because that call's frame returns as soon as the generator
    /// suspends and this one does not: a delegation chain holds one of each per level, so counting
    /// only the call would say a chain is half as deep as it is - and the measured difference is
    /// the difference between an answer and a terminated process.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=TBF
    // Broiler-Falsified-If: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
    // Broiler-Human:        PENDING
    internal JsValue ResumeGenerator(JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsGenerator generator)
        {
            return ThrowTypeError(
                "Generator.prototype." + method + " called on a value that is not a generator");
        }

        if (generator.State == JsGeneratorState.Executing)
        {
            return ThrowTypeError("Generator is already running");
        }

        if (generator.State == JsGeneratorState.Completed || generator.Frame is null)
        {
            return mode switch
            {
                JsResumeMode.Throw => throw new JsThrow(sent, Render(sent)),
                JsResumeMode.Return => JsValue.Object(Realm.IteratorResult(sent, done: true)),
                _ => JsValue.Object(Realm.IteratorResult(JsValue.Undefined, done: true)),
            };
        }

        // A GENERATOR THAT HAS NOT STARTED HAS NO `try` TO RUN, so an abrupt resumption completes
        // it where it stands. Resuming into the body first and then unwinding would run the
        // parameter bindings' side effects a second time, which nothing in the language asks for.
        if (generator.State == JsGeneratorState.SuspendedStart && mode != JsResumeMode.Next)
        {
            CompleteGenerator(generator);

            return mode == JsResumeMode.Throw
                ? throw new JsThrow(sent, Render(sent))
                : JsValue.Object(Realm.IteratorResult(sent, done: true));
        }

        Charge(4);

        // THE DEPTH IS TAKEN BEFORE THE STATE MOVES, so a resumption refused for depth leaves the
        // generator suspended and resumable rather than half-entered. A generator that could not
        // be resumed because the stack was full has not run any of its body, and completing it
        // would be a stronger claim than what happened.
        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return ThrowRangeError("Maximum call stack size exceeded");
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
        var frame = generator.Frame;
        frame.ResumeMode = mode;
        frame.ResumeValue = sent;
        frame.Suspended = false;
        generator.State = JsGeneratorState.Executing;

        try
        {
            var completed = Execute(
                frame.Program,
                frame.UnitIndex,
                null,
                frame.ThisValue,
                frame.Arguments,
                frame.Function,
                frame);

            if (frame.Suspended)
            {
                generator.State = JsGeneratorState.SuspendedYield;
                frame.Started = true;
                return JsValue.Object(Realm.IteratorResult(completed, done: false));
            }

            CompleteGenerator(generator);
            return JsValue.Object(Realm.IteratorResult(completed, done: true));
        }
        catch (JsReturnSignal forced)
        {
            // THE RETURN THE `finally` BLOCKS DID NOT OVERRIDE. It reaches here having run every
            // enclosing finaliser on the way out, which is the whole reason it travels as an
            // exception rather than as a returned flag.
            return JsValue.Object(Realm.IteratorResult(forced.Value, done: true));
        }
        finally
        {
            // ANY OTHER WAY OUT OF THE BODY COMPLETES THE GENERATOR - a throw the body did not
            // catch, an allowance spent mid-instruction, a stack the runtime could not grow. The
            // test is on the STATE rather than on the exception type, because a catch clause per
            // type is a list that a new type is added to by forgetting: the one that got away
            // would leave a generator reading `already running` for the rest of the program, and
            // every later resumption of it would be a TypeError with no cause a reader could find.
            if (generator.State == JsGeneratorState.Executing)
            {
                CompleteGenerator(generator);
            }

            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>
    /// Retires a generator: no frame, no state to resume, and the operand stack let go of.
    /// </summary>
    /// <remarks>
    /// <b>Dropping the frame reference is the point and not the tidiness.</b> A completed generator
    /// object may stay reachable for the rest of the program - somebody kept the variable - and
    /// without this it would keep its operand-stack array, its scope chain and everything those
    /// reach alive with it. Clearing the field is what makes exhausting a generator release what it
    /// was holding, at the instant it is exhausted rather than at the collector's convenience.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static void CompleteGenerator(JsGenerator generator)
    {
        generator.Frame = null;
        generator.State = JsGeneratorState.Completed;
    }

    /// <summary>
    /// The iterator a <c>yield*</c> steps, for the iterables this manifest has.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is NOT <c>GetIterator</c>, and the difference is <c>Symbol</c>'s absence.</b> The
    /// specification reads <c>obj[@@iterator]</c> and calls it; this realm has no Symbol, so there
    /// is no key a program could put an iterator method under and no key this could read. What it
    /// does instead is recognise the iterables the realm itself builds - a generator, a String, an
    /// Array, an <c>arguments</c> object - and refuse everything else with the <c>TypeError</c> the
    /// language gives for a value that is not iterable. The difference a program can observe is
    /// therefore exactly this: an object that would have been iterable through a
    /// <c>Symbol.iterator</c> it cannot define is not iterable here.
    /// </para>
    /// <para>
    /// <b>A generator is returned as itself</b>, which is what <c>%GeneratorPrototype%[@@iterator]</c>
    /// does, so <c>yield*</c> over one forwards through the ordinary <c>next</c>, <c>return</c> and
    /// <c>throw</c> properties and a program that replaced one of them sees its replacement used.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsValue GetIterator(JsValue value)
    {
        if (value.IsString)
        {
            return JsValue.Object(
                new JsSourceIterator(Realm.StringIteratorPrototype, "String Iterator", value));
        }

        if (value.AsObjectOrNull() is not { } target)
        {
            return ThrowTypeError(Describe(value) + " is not iterable");
        }

        if (target is JsGenerator)
        {
            return value;
        }

        if (target is JsArray ||
            string.Equals(target.ClassName, "Arguments", System.StringComparison.Ordinal))
        {
            return JsValue.Object(
                new JsSourceIterator(Realm.ArrayIteratorPrototype, "Array Iterator", value));
        }

        if (target is JsPrimitiveWrapper wrapper && wrapper.Primitive.IsString)
        {
            return JsValue.Object(
                new JsSourceIterator(
                    Realm.StringIteratorPrototype, "String Iterator", wrapper.Primitive));
        }

        return ThrowTypeError(Describe(value) + " is not iterable");
    }

    // ---- the loop ------------------------------------------------------------------------------

    /// <summary>
    /// The dispatch loop, over an ordinary frame or over a generator's heap-allocated one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A <see langword="null"/> <paramref name="frame"/> is the ordinary path and it is
    /// unchanged.</b> The operand stack, the scope chain, the height and the instruction pointer
    /// are locals exactly as they were; the frame is read once, at entry, and never looked at again
    /// unless the unit actually suspends. What an ordinary call now pays is that one test and the
    /// argument that carries it.
    /// </para>
    /// <para>
    /// <b>An abrupt resumption is raised at the top of the try, not at the suspension point.</b>
    /// <c>gen.throw</c> and <c>gen.return</c> re-enter at the instruction the frame suspended at
    /// and must be seen by whatever exception region encloses it - so the raise happens inside the
    /// same try the dispatch loop runs in, with <c>current</c> already set to that instruction. The
    /// existing region search then runs the same <c>catch</c> and <c>finally</c> blocks it would
    /// have run for a throw from the instruction itself, and no unwinding is reimplemented.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D27125
    // Broiler-Human:        PENDING
    private JsValue Execute(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsFrame? frame)
    {
        var unit = program.Functions[unitIndex];
        var code = program.Code;
        var constants = program.Constants;
        var names = program.Names;
        var stack = frame is null ? new JsValue[unit.MaxOperandStack + 1] : frame.Stack;

        var scopes = frame is null
            ? new System.Collections.Generic.List<JsEnvironment>(4) { environment! }
            : frame.Scopes;

        var sp = frame is null ? 0 : frame.Sp;
        var pc = frame is null ? (int)unit.CodeOffset : frame.Pc;
        var strict = unit.IsStrict;
        var current = pc;

        // A DELEGATION RESUMES INSIDE ITS OWN OPCODE, whatever mode it resumes in: `return` and
        // `throw` arriving mid-`yield*` are forwarded to the inner iterator rather than raised
        // here, so only a plain `yield` reaches either of the two arms below.
        var abrupt = frame is { Started: true, Delegating: false } &&
            frame.ResumeMode != JsResumeMode.Next;

        // THE NORMAL RESUMPTION IS FINISHED HERE AND NOT IN THE OPCODE. The instruction that
        // suspended has already run its pop; what re-entry owes it is the push of the sent value
        // and the step past it, and doing that here keeps the `Yield` case a straight-line suspend.
        if (frame is { Started: true, Delegating: false } && !abrupt)
        {
            stack[sp++] = frame.ResumeValue;
            pc += JsOpcodes.InstructionWidth(JsOpcode.Yield);
            current = pc;
        }

        while (true)
        {
            try
            {
                if (abrupt)
                {
                    abrupt = false;
                    current = pc;
                    var carried = frame!.ResumeValue;

                    if (frame.ResumeMode == JsResumeMode.Throw)
                    {
                        throw new JsThrow(carried, Render(carried));
                    }

                    throw new JsReturnSignal(carried);
                }

                while (true)
                {
                    current = pc;
                    Charge(FuelPerInstruction);
                    var opcode = (JsOpcode)code[pc];

                    switch (opcode)
                    {
                        case JsOpcode.Nop:
                            pc++;
                            break;

                        case JsOpcode.LoadUndefined:
                            stack[sp++] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.LoadNull:
                            stack[sp++] = JsValue.Null;
                            pc++;
                            break;

                        case JsOpcode.LoadTrue:
                            stack[sp++] = JsValue.True;
                            pc++;
                            break;

                        case JsOpcode.LoadFalse:
                            stack[sp++] = JsValue.False;
                            pc++;
                            break;

                        case JsOpcode.LoadConstant:
                            stack[sp++] = constants[U16(code, pc)];
                            pc += 3;
                            break;

                        case JsOpcode.LoadThis:
                            stack[sp++] = thisValue;
                            pc++;
                            break;

                        case JsOpcode.NewArguments:
                            stack[sp++] = JsValue.Object(
                                Realm.CreateArguments(actualArguments, self));

                            pc++;
                            break;

                        case JsOpcode.LoadScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped read named no slot");
                            }

                            if (slot.Slots[U16(code, pc + 1)].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            stack[sp++] = slot.Slots[U16(code, pc + 1)];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.StoreScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped write named no slot");
                            }

                            var index = U16(code, pc + 1);

                            if (slot.Slots[index].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            slot.Slots[index] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.InitialiseScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped initialiser named no slot");
                            }

                            slot.Slots[U16(code, pc + 1)] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.LoadGlobal:
                        {
                            var name = names[U16(code, pc)];

                            if (!HasProperty(Realm.GlobalObject, name))
                            {
                                ThrowReferenceError(name + " is not defined");
                            }

                            stack[sp++] = GetProperty(JsValue.Object(Realm.GlobalObject), name);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.LoadGlobalOrUndefined:
                        {
                            var name = names[U16(code, pc)];

                            stack[sp++] = HasProperty(Realm.GlobalObject, name)
                                ? GetProperty(JsValue.Object(Realm.GlobalObject), name)
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.StoreGlobal:
                            SetProperty(
                                JsValue.Object(Realm.GlobalObject),
                                names[U16(code, pc)],
                                stack[--sp],
                                strict);

                            pc += 3;
                            break;

                        case JsOpcode.DeclareGlobal:
                        {
                            var name = names[U16(code, pc)];

                            if (!Realm.GlobalObject.HasOwnProperty(name))
                            {
                                Realm.GlobalObject.SetOwnProperty(
                                    name,
                                    JsProperty.Data(
                                        JsValue.Undefined,
                                        JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable));
                            }

                            pc += 3;
                            break;
                        }

                        case JsOpcode.PushScope:
                            scopes.Add(new JsEnvironment(U16(code, pc), scopes[^1]));
                            pc += 3;
                            break;

                        case JsOpcode.PopScope:
                            scopes.RemoveAt(scopes.Count - 1);
                            pc++;
                            break;

                        case JsOpcode.CopyScope:
                            scopes[^1] = scopes[^1].Copy(U16(code, pc));
                            pc += 3;
                            break;

                        case JsOpcode.NewObject:
                            stack[sp++] = JsValue.Object(new JsObject(Realm.ObjectPrototype));
                            pc++;
                            break;

                        case JsOpcode.NewArray:
                        {
                            var count = U16(code, pc);
                            var array = new JsArray(Realm.ArrayPrototype);

                            for (var at = 0; at < count; at++)
                            {
                                array.Push(stack[sp - count + at]);
                            }

                            sp -= count;
                            stack[sp++] = JsValue.Object(array);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetProperty:
                        {
                            var target = stack[--sp];
                            stack[sp++] = GetProperty(target, names[U16(code, pc)]);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SetProperty:
                        {
                            var value = stack[--sp];
                            var target = stack[--sp];
                            SetProperty(target, names[U16(code, pc)], value, strict);
                            stack[sp++] = value;
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];
                            stack[sp++] = GetIndexed(target, key);
                            pc++;
                            break;
                        }

                        case JsOpcode.SetIndex:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            var target = stack[--sp];
                            SetIndexed(target, key, value, strict);
                            stack[sp++] = value;
                            pc++;
                            break;
                        }

                        case JsOpcode.DefineField:
                            stack[sp - 2].AsObject().SetOwnProperty(
                                names[U16(code, pc)],
                                JsProperty.Data(stack[sp - 1], JsPropertyAttributes.Default));

                            sp--;
                            pc += 3;
                            break;

                        case JsOpcode.DefineIndexed:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            SetIndexed(stack[sp - 1], key, value, strict: false);
                            pc++;
                            break;
                        }

                        case JsOpcode.DefineGetter:
                        case JsOpcode.DefineSetter:
                        {
                            var accessor = stack[--sp].AsObject();
                            var host = stack[sp - 1].AsObject();
                            var key = names[U16(code, pc)];
                            host.TryGetOwnProperty(key, out var existing);

                            host.SetOwnProperty(
                                key,
                                JsProperty.Accessor(
                                    opcode == JsOpcode.DefineGetter ? accessor : existing.Getter,
                                    opcode == JsOpcode.DefineSetter ? accessor : existing.Setter,
                                    JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable));

                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeleteProperty:
                        {
                            var target = stack[--sp];
                            stack[sp++] = JsValue.Boolean(
                                !target.IsObject ||
                                target.AsObject().DeleteOwnProperty(names[U16(code, pc)]));

                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeleteIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];

                            stack[sp++] = JsValue.Boolean(
                                !target.IsObject ||
                                target.AsObject().DeleteOwnProperty(ToPropertyKey(key)));

                            pc++;
                            break;
                        }

                        case JsOpcode.Closure:
                            stack[sp++] = JsValue.Object(
                                Realm.CreateClosure(program, U16(code, pc), scopes[^1], thisValue));

                            pc += 3;
                            break;

                        case JsOpcode.Call:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.Construct:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.Return:
                            return stack[--sp];

                        case JsOpcode.ReturnUndefined:
                            return JsValue.Undefined;

                        case JsOpcode.Add:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = Add(left, right);
                            pc++;
                            break;
                        }

                        case JsOpcode.Subtract:
                            Binary(stack, ref sp, static (a, b) => a - b, this);
                            pc++;
                            break;

                        case JsOpcode.Multiply:
                            Binary(stack, ref sp, static (a, b) => a * b, this);
                            pc++;
                            break;

                        case JsOpcode.Divide:
                            Binary(stack, ref sp, static (a, b) => a / b, this);
                            pc++;
                            break;

                        case JsOpcode.Remainder:
                            Binary(stack, ref sp, static (a, b) => a % b, this);
                            pc++;
                            break;

                        case JsOpcode.Exponent:
                            Binary(stack, ref sp, static (a, b) => System.Math.Pow(a, b), this);
                            pc++;
                            break;

                        case JsOpcode.Negate:
                            stack[sp - 1] = JsValue.Number(-ToNumber(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.ToNumber:
                            stack[sp - 1] = JsValue.Number(ToNumber(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.Not:
                            stack[sp - 1] = JsValue.Boolean(!stack[sp - 1].ToBooleanValue());
                            pc++;
                            break;

                        case JsOpcode.BitwiseNot:
                            stack[sp - 1] = JsValue.Number(~ToInt32(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.LessThan:
                        case JsOpcode.LessThanOrEqual:
                        case JsOpcode.GreaterThan:
                        case JsOpcode.GreaterThanOrEqual:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(Relational(opcode, left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(!left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(!LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseOr:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left | right);
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseAnd:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left & right);
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseXor:
                        {
                            var right = ToInt32(stack[--sp]);
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left ^ right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftLeft:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left << (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftRight:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToInt32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left >> (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.ShiftRightUnsigned:
                        {
                            var right = ToUint32(stack[--sp]) & 31;
                            var left = ToUint32(stack[--sp]);
                            stack[sp++] = JsValue.Number(left >> (int)right);
                            pc++;
                            break;
                        }

                        case JsOpcode.TypeOf:
                            stack[sp - 1] = JsValue.String(stack[sp - 1].TypeOf());
                            pc++;
                            break;

                        case JsOpcode.InstanceOf:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(InstanceOf(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.In:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];

                            if (!right.IsObject)
                            {
                                ThrowTypeError("Cannot use 'in' operator to search for a key");
                            }

                            stack[sp++] = JsValue.Boolean(
                                HasProperty(right.AsObject(), ToPropertyKey(left)));

                            pc++;
                            break;
                        }

                        case JsOpcode.Void:
                            stack[sp - 1] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.Jump:
                            pc = (int)U32(code, pc);
                            break;

                        case JsOpcode.JumpIfFalse:
                            pc = !stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.JumpIfTrue:
                            pc = stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.Throw:
                        {
                            var thrown = stack[--sp];

                            // THE ONE VALUE THIS INSTRUCTION DOES NOT THROW. A `finally` that a
                            // forced return passed through re-raises what it parked, using this
                            // instruction because the lowering has no other; a forced return
                            // parked here comes back out as a forced return, so an outer `catch`
                            // still never sees it.
                            if (thrown.AsObjectOrNull() is JsForcedReturn forced)
                            {
                                throw new JsReturnSignal(forced.Value);
                            }

                            throw new JsThrow(thrown, Render(thrown));
                        }

                        case JsOpcode.ForInStart:
                        {
                            var target = stack[--sp];
                            stack[sp++] = JsValue.Object(Realm.CreateEnumerator(this, target));
                            pc++;
                            break;
                        }

                        case JsOpcode.ForInNext:
                        {
                            var enumerator = (JsEnumerator)stack[--sp].AsObject();

                            if (enumerator.TryNext(out var key))
                            {
                                stack[sp++] = JsValue.String(key);
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.Yield:
                        {
                            // THE WHOLE OF SUSPENDING. The yielded value leaves on the return, the
                            // height and the pointer stay behind in the frame, and the pointer is
                            // left AT this instruction rather than after it - so a resumption that
                            // arrives abruptly raises its throw or its return at a point the
                            // enclosing exception regions actually cover. Nothing clears the
                            // delegation flag here because nothing can have set it: a suspended
                            // `yield*` always resumes at its own instruction, never at this one.
                            var yielded = stack[--sp];
                            frame!.Sp = sp;
                            frame.Pc = pc;
                            frame.Suspended = true;
                            return yielded;
                        }

                        case JsOpcode.YieldDelegate:
                        {
                            var step = Delegate(frame!, stack, ref sp, pc);

                            if (frame!.Suspended)
                            {
                                return step;
                            }

                            stack[sp++] = step;
                            pc++;
                            break;
                        }

                        case JsOpcode.Pop:
                            sp--;
                            pc++;
                            break;

                        case JsOpcode.Duplicate:
                            stack[sp] = stack[sp - 1];
                            sp++;
                            pc++;
                            break;

                        case JsOpcode.DuplicateTwo:
                            stack[sp] = stack[sp - 2];
                            stack[sp + 1] = stack[sp - 1];
                            sp += 2;
                            pc++;
                            break;

                        case JsOpcode.Swap:
                        {
                            (stack[sp - 1], stack[sp - 2]) = (stack[sp - 2], stack[sp - 1]);
                            pc++;
                            break;
                        }

                        case JsOpcode.Pick:
                            stack[sp] = stack[sp - 1 - code[pc + 1]];
                            sp++;
                            pc += 2;
                            break;

                        default:
                            throw new JsAbort(
                                JsAbortKind.InternalDefect, "a verified opcode had no case here");
                    }
                }
            }
            catch (JsThrow thrown)
            {
                if (!TryFindHandler(program, unitIndex, current, out var region))
                {
                    throw;
                }

                while (scopes.Count > region.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)region.StackHeight;
                stack[sp++] = thrown.Value;
                pc = (int)region.Handler;
            }
            catch (JsReturnSignal forced)
            {
                // A FORCED RETURN RUNS EVERY `finally` AND NO `catch`. The search skips catch
                // regions rather than taking the innermost region of either kind, which is the one
                // place the two completions differ; taking the innermost would let
                // `try { yield } catch (e) {}` swallow a `gen.return()` as though somebody had
                // thrown, and the generator would carry on running instead of ending.
                if (!TryFindFinally(program, unitIndex, current, out var region))
                {
                    throw;
                }

                while (scopes.Count > region.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)region.StackHeight;
                stack[sp++] = JsValue.Object(new JsForcedReturn(forced.Value));
                pc = (int)region.Handler;
            }
        }
    }

    /// <summary>
    /// The innermost <c>finally</c> region of <paramref name="unit"/> covering <paramref name="pc"/>.
    /// </summary>
    /// <remarks>
    /// The regions of one unit are recorded innermost-first by the lowering, so the first match in
    /// order is the innermost - the same property <see cref="TryFindHandler"/> relies on. What
    /// differs is only that a <c>catch</c> region is passed over rather than taken.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static bool TryFindFinally(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit &&
                candidate.Kind == JsFormat.HandlerKind.Finally &&
                pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    /// <summary>
    /// One turn of a <c>yield*</c>: step the inner iterator, or forward an abrupt resumption to it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the specification's delegation loop, entered afresh at every resumption.</b> The
    /// iterator lives in the frame, so the loop's whole state between two resumptions is "which
    /// iterator" and "still delegating" - and re-entering at the same instruction is what lets a
    /// <c>return</c> or a <c>throw</c> that arrives mid-delegation be handed to the inner iterator
    /// rather than raised in the outer body.
    /// </para>
    /// <para>
    /// <b>The two missing-method cases are where an engine is most often wrong.</b> An inner
    /// iterator with no <c>return</c> - which is every Array and String iterator - does not swallow
    /// the outer <c>return</c>: the outer generator returns, running its own finalisers. An inner
    /// iterator with no <c>throw</c> is closed first and then the delegation raises a
    /// <c>TypeError</c>, so a program that throws into one is told what is missing rather than
    /// silently getting its own exception back.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=TBF
    // Broiler-Falsified-If: a `return` or a `throw` that arrives while a `yield*` is suspended is not offered to the inner iterator first
    // Broiler-Human:        PENDING
    private JsValue Delegate(JsFrame frame, JsValue[] stack, ref int sp, int pc)
    {
        if (!frame.Delegating)
        {
            frame.Delegate = GetIterator(stack[--sp]);
            frame.ResumeMode = JsResumeMode.Next;
            frame.ResumeValue = JsValue.Undefined;
        }

        // THE FLAG IS CLEARED ON THE WAY IN AND SET AGAIN ONLY BY AN ACTUAL SUSPENSION, so every
        // other way out of this method ends the delegation - including the ways that leave by
        // throwing. Clearing it only on the paths that return normally left it set when an inner
        // `throw` method threw and the outer body caught: the next resumption would then re-enter
        // a delegation that no longer existed, at an instruction that was no longer a `yield*`.
        frame.Delegating = false;
        var iterator = frame.Delegate;
        var mode = frame.ResumeMode;
        var sent = frame.ResumeValue;
        frame.ResumeMode = JsResumeMode.Next;
        frame.ResumeValue = JsValue.Undefined;
        JsValue step;

        switch (mode)
        {
            case JsResumeMode.Throw:
            {
                var thrower = GetProperty(iterator, "throw");

                if (!thrower.IsObject || !thrower.AsObject().IsCallable)
                {
                    CloseIterator(iterator);
                    return ThrowTypeError("The iterator does not provide a 'throw' method.");
                }

                step = Call(thrower, iterator, [sent]);
                break;
            }

            case JsResumeMode.Return:
            {
                var returner = GetProperty(iterator, "return");

                // AN INNER ITERATOR WITH NO `return` DOES NOT SWALLOW THE OUTER ONE. Every Array
                // and String iterator is in this case, so it is the common one rather than the
                // exotic one: the outer generator returns, and its own finalisers run on the way.
                if (!returner.IsObject || !returner.AsObject().IsCallable)
                {
                    throw new JsReturnSignal(sent);
                }

                step = Call(returner, iterator, [sent]);

                if (!step.IsObject)
                {
                    return ThrowTypeError("iterator result is not an object");
                }

                if (GetProperty(step, "done").ToBooleanValue())
                {
                    throw new JsReturnSignal(GetProperty(step, "value"));
                }

                break;
            }

            default:
            {
                var next = GetProperty(iterator, "next");
                step = Call(next, iterator, [sent]);
                break;
            }
        }

        if (!step.IsObject)
        {
            return ThrowTypeError("iterator result is not an object");
        }

        if (GetProperty(step, "done").ToBooleanValue())
        {
            // THE INNER ITERATOR'S OWN RETURN VALUE IS WHAT `yield*` EVALUATES TO, which is the
            // half of delegation a loop written by hand always forgets. An Array iterator's is
            // `undefined`; an inner generator's is whatever it returned.
            frame.Delegate = JsValue.Undefined;
            return GetProperty(step, "value");
        }

        frame.Delegating = true;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;
        return GetProperty(step, "value");
    }

    /// <summary>Closes an inner iterator, ignoring an absent <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void CloseIterator(JsValue iterator)
    {
        var returner = GetProperty(iterator, "return");

        if (returner.IsObject && returner.AsObject().IsCallable)
        {
            _ = Call(returner, iterator, System.Array.Empty<JsValue>());
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EAA946
    // Broiler-Human:        PENDING
    private static bool TryFindHandler(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit && pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6F7A88
    // Broiler-Human:        PENDING
    private static JsEnvironment Slot(
        System.Collections.Generic.List<JsEnvironment> scopes, int depth, int index, out bool found)
    {
        var current = scopes[^1];

        for (var step = 0; step < depth; step++)
        {
            if (current.Parent is null)
            {
                found = false;
                return current;
            }

            current = current.Parent;
        }

        found = index < current.Slots.Length;
        return current;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D7D299
    // Broiler-Human:        PENDING
    private static void Binary(
        JsValue[] stack, ref int sp, System.Func<double, double, double> operation, JsEngine engine)
    {
        var right = engine.ToNumber(stack[--sp]);
        var left = engine.ToNumber(stack[--sp]);
        stack[sp++] = JsValue.Number(operation(left, right));
    }

    /// <summary>The <c>+</c> operator, which is concatenation when either side is a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=414277
    // Broiler-Human:        PENDING
    internal JsValue Add(JsValue left, JsValue right)
    {
        var primitiveLeft = ToPrimitive(left, "default");
        var primitiveRight = ToPrimitive(right, "default");

        if (primitiveLeft.IsString || primitiveRight.IsString)
        {
            return JsValue.String(ToStringValue(primitiveLeft) + ToStringValue(primitiveRight));
        }

        return JsValue.Number(ToNumber(primitiveLeft) + ToNumber(primitiveRight));
    }

    /// <summary>The four relational operators, through one abstract comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C40206
    // Broiler-Human:        PENDING
    internal bool Relational(JsOpcode opcode, JsValue left, JsValue right)
    {
        // THE ORDER OF EVALUATION IS THE SPECIFICATION'S: `<` and `<=` convert left first, `>` and
        // `>=` convert RIGHT first. It is observable through a valueOf with a side effect, and it
        // is the kind of thing only a conformance suite ever notices.
        JsValue first;
        JsValue second;

        if (opcode is JsOpcode.LessThan or JsOpcode.LessThanOrEqual)
        {
            first = ToPrimitive(left, "number");
            second = ToPrimitive(right, "number");
        }
        else
        {
            second = ToPrimitive(right, "number");
            first = ToPrimitive(left, "number");
        }

        if (first.IsString && second.IsString)
        {
            var order = string.CompareOrdinal(first.AsString(), second.AsString());

            return opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        var a = ToNumber(first);
        var b = ToNumber(second);

        return opcode switch
        {
            JsOpcode.LessThan => a < b,
            JsOpcode.LessThanOrEqual => a <= b,
            JsOpcode.GreaterThan => a > b,
            _ => a >= b,
        };
    }

    /// <summary>The abstract equality comparison, <c>==</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=BED8C4
    // Broiler-Human:        PENDING
    internal bool LooselyEquals(JsValue left, JsValue right)
    {
        if (left.Type == right.Type)
        {
            return left.StrictlyEquals(right);
        }

        if (left.IsNullish && right.IsNullish)
        {
            return true;
        }

        if (left.IsNullish || right.IsNullish)
        {
            return false;
        }

        if (left.Type == JsType.Number && right.Type == JsType.String)
        {
            return left.AsNumber() == ToNumber(right);
        }

        if (left.Type == JsType.String && right.Type == JsType.Number)
        {
            return ToNumber(left) == right.AsNumber();
        }

        if (left.Type == JsType.Boolean)
        {
            return LooselyEquals(JsValue.Number(left.AsBoolean() ? 1 : 0), right);
        }

        if (right.Type == JsType.Boolean)
        {
            return LooselyEquals(left, JsValue.Number(right.AsBoolean() ? 1 : 0));
        }

        if (left.IsObject && right.Type is JsType.Number or JsType.String)
        {
            return LooselyEquals(ToPrimitive(left, "default"), right);
        }

        if (right.IsObject && left.Type is JsType.Number or JsType.String)
        {
            return LooselyEquals(left, ToPrimitive(right, "default"));
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=503104
    // Broiler-Human:        PENDING
    private bool InstanceOf(JsValue left, JsValue right)
    {
        if (!right.IsObject || !right.AsObject().IsCallable)
        {
            ThrowTypeError("Right-hand side of 'instanceof' is not callable");
        }

        if (right.AsObject() is JsBoundFunction bound)
        {
            return InstanceOf(left, JsValue.Object(bound.Target));
        }

        if (!left.IsObject)
        {
            return false;
        }

        var prototype = GetProperty(right, "prototype");

        if (!prototype.IsObject)
        {
            ThrowTypeError("Function has non-object prototype in instanceof");
        }

        var target = prototype.AsObject();
        var walk = left.AsObject().Prototype;

        while (walk is not null)
        {
            if (ReferenceEquals(walk, target))
            {
                return true;
            }

            walk = walk.Prototype;
        }

        return false;
    }

    /// <summary>Reads an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=67BB23
    // Broiler-Human:        PENDING
    internal JsValue GetIndexed(JsValue target, JsValue key)
    {
        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < array.DenseCount)
            {
                var element = array.DenseAt(at);

                // A HOLE IS NOT AN ANSWER. It may be a hole, or it may be a slot the array vacated
                // when the element was given attributes it could not carry, in which case the value
                // is in the ordinary map and the general path finds it.
                if (!element.IsEmpty)
                {
                    return element;
                }
            }
        }

        if (target.IsString && key.Type == JsType.Number)
        {
            var text = target.AsString();
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < text.Length)
            {
                return JsValue.String(text[at].ToString());
            }
        }

        return GetProperty(target, ToPropertyKey(key));
    }

    /// <summary>Writes an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=30B5B7
    // Broiler-Human:        PENDING
    internal void SetIndexed(JsValue target, JsValue key, JsValue value, bool strict)
    {
        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            // THE FAST PATH IS FOR AN ELEMENT THAT IS STILL AN ELEMENT. A dense slot the array
            // vacated is a property with attributes living in the ordinary map, and writing the
            // slot would step straight over a `writable: false` that somebody asked for - which is
            // what made a frozen array assignable while reporting itself frozen. Appending is a
            // fast path too, and only while the array is extensible.
            if (at == number && at >= 0)
            {
                if (at < array.DenseCount)
                {
                    if (!array.DenseAt(at).IsEmpty)
                    {
                        array.SetIndex((uint)at, value);
                        return;
                    }
                }
                else if (array.Extensible && at == array.Length)
                {
                    array.SetIndex((uint)at, value);
                    return;
                }
            }
        }

        SetProperty(target, ToPropertyKey(key), value, strict);
    }

    /// <summary>Renders a thrown value for a host that has to describe it in one line.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1C82D0
    // Broiler-Human:        PENDING
    internal string Render(JsValue value)
    {
        if (!value.IsObject)
        {
            return value.IsString ? value.AsString() : ToStringValue(value);
        }

        var name = GetProperty(value, "name");
        var message = GetProperty(value, "message");

        if (!name.IsNullish || !message.IsNullish)
        {
            var head = name.IsNullish ? "Error" : ToStringValue(name);
            var tail = message.IsNullish ? string.Empty : ToStringValue(message);
            return tail.Length == 0 ? head : head + ": " + tail;
        }

        return ToStringValue(value);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C54D2D
    // Broiler-Human:        PENDING
    private static ushort U16(byte[] code, int at) => (ushort)(code[at + 1] | (code[at + 2] << 8));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B2766
    // Broiler-Human:        PENDING
    private static uint U32(byte[] code, int at) => (uint)(
        code[at + 1] | (code[at + 2] << 8) | (code[at + 3] << 16) | (code[at + 4] << 24));
}
