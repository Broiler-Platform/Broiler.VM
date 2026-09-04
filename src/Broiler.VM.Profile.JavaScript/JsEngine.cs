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

    // ---- the iteration protocol ----------------------------------------------------------------

    /// <summary>
    /// The abstract operation <c>GetIterator</c>: the guest's own protocol, driven from here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here shortcuts the protocol for a value this engine happens to recognise.</b>
    /// Spreading an Array calls <c>Array.prototype[@@iterator]</c> and then calls the <c>next</c>
    /// that answered, every time - because a guest may replace either, and a program that does is
    /// entitled to see its own function run. A fast path over the dense elements would be faster
    /// and would answer the wrong thing for exactly the programs that would notice.
    /// </para>
    /// <para>
    /// <c>next</c> is read ONCE, here, and the record keeps it. The specification reads it at
    /// <c>GetIterator</c> and calls that same function at every step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIterator(JsValue iterable)
    {
        Charge(4);

        if (iterable.IsNullish)
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        var method = GetProperty(iterable, JsRealm.IteratorKey);

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        var iterator = Call(method, iterable, System.Array.Empty<JsValue>());

        if (!iterator.IsObject)
        {
            ThrowTypeError("The result of the iterator method is not an object");
        }

        return new JsIteratorRecord(iterator, GetProperty(iterator, "next"));
    }

    /// <summary>
    /// One step of <c>IteratorStep</c>: answers the next value, or that the iterator is finished.
    /// </summary>
    /// <remarks>
    /// <b>A record whose <c>next</c> threw is marked done before the exception leaves.</b> The
    /// specification does not close an iterator whose <c>next</c> failed - it has no reason to
    /// believe the object is in a state that can answer <c>return</c> - and the flag is what carries
    /// that decision to the <c>IterateClose</c> the lowering emits unconditionally.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(JsIteratorRecord record, out JsValue value)
    {
        value = JsValue.Undefined;

        if (record.Done)
        {
            return false;
        }

        Charge(2);
        JsValue result;

        try
        {
            result = Call(record.Next, record.Iterator, System.Array.Empty<JsValue>());
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        if (!result.IsObject)
        {
            record.Done = true;
            ThrowTypeError("Iterator result " + Describe(result) + " is not an object");
        }

        if (GetProperty(result, "done").ToBooleanValue())
        {
            record.Done = true;
            return false;
        }

        value = GetProperty(result, "value");
        return true;
    }

    /// <summary>
    /// <c>IteratorClose</c> under a normal or a <c>break</c>-shaped completion.
    /// </summary>
    /// <remarks>
    /// Errors from <c>return</c> propagate here, and a <c>return</c> answering a non-object is
    /// itself a <c>TypeError</c> - both of which a guest can observe and both of which a quiet close
    /// would swallow.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal void CloseIterator(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;
        var method = GetProperty(record.Iterator, "return");

        if (method.IsNullish)
        {
            return;
        }

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            ThrowTypeError("The iterator's return is not a function");
        }

        var result = Call(method, record.Iterator, System.Array.Empty<JsValue>());

        if (!result.IsObject)
        {
            ThrowTypeError("The iterator's return answered " + Describe(result) + " and not an object");
        }
    }

    /// <summary>
    /// <c>IteratorClose</c> under a throw completion, which discards whatever <c>return</c> does.
    /// </summary>
    /// <remarks>
    /// <b>The exception already in flight is the one the program is owed.</b> A <c>for … of</c>
    /// body that throws still has to give the iterator its <c>return</c>, but replacing the body's
    /// exception with one the clean-up raised would report the second failure and lose the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal void CloseIteratorQuietly(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;

        try
        {
            var method = GetProperty(record.Iterator, "return");

            if (method.IsObject && method.AsObject().IsCallable)
            {
                Call(method, record.Iterator, System.Array.Empty<JsValue>());
            }
        }
        catch (JsThrow)
        {
            // Deliberately swallowed: see the remark.
        }
    }

    /// <summary>Drains an iterable into a list, closing nothing because it ran to completion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal void IterateInto(JsValue iterable, System.Collections.Generic.List<JsValue> into)
    {
        var record = GetIterator(iterable);

        while (TryIterateNext(record, out var element))
        {
            into.Add(element);
        }
    }

    /// <summary>Drains a record that is already open, which is what a rest element takes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal JsArray DrainIterator(JsIteratorRecord record)
    {
        var rest = Realm.NewArray();

        while (TryIterateNext(record, out var element))
        {
            rest.Push(element);
        }

        return rest;
    }

    /// <summary>
    /// The abstract operation <c>CopyDataProperties</c>, which is what object spread is.
    /// </summary>
    /// <remarks>
    /// <b>Own and enumerable, in the order the source yields them, and through the ordinary read
    /// path.</b> Reading through the property path is what makes a getter on the source run once
    /// and contribute its value, which is what the language says and what copying descriptors would
    /// not do. A <c>null</c> or <c>undefined</c> source contributes nothing rather than throwing -
    /// <c>{...null}</c> is an empty object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal void CopyDataProperties(JsObject target, JsValue source)
    {
        if (source.IsNullish)
        {
            return;
        }

        var from = ToObject(source);

        foreach (var key in from.OwnPropertyNames())
        {
            Charge(1);

            if (!from.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            target.SetOwnProperty(
                key,
                JsProperty.Data(GetProperty(source, key), JsPropertyAttributes.Default));
        }
    }

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

        // A UNIT THAT BINDS ITS OWN PARAMETERS GETS NO COPY AT ALL, and the slots stay EMPTY. That
        // is not an optimisation: a default that reads a later parameter has to find a binding in
        // its temporal dead zone, and filling the slots with `undefined` here would turn that
        // ReferenceError into a silent `undefined`. For a simple parameter list `ParameterCount` is
        // both the arity and the copy count and this is the whole of parameter binding.
        if (!unit.BindsParameters)
        {
            var count = System.Math.Min(arguments.Length, (int)unit.ParameterCount);

            for (var at = 0; at < count; at++)
            {
                environment.Slots[at] = arguments[at];
            }

            for (var at = count; at < unit.ParameterCount; at++)
            {
                environment.Slots[at] = JsValue.Undefined;
            }
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

        return Execute(program, function.Unit, environment, receiver, arguments, function);
    }

    // ---- the loop ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D27125
    // Broiler-Human:        PENDING
    private JsValue Execute(
        JsProgram program,
        int unitIndex,
        JsEnvironment environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self)
    {
        var unit = program.Functions[unitIndex];
        var code = program.Code;
        var constants = program.Constants;
        var names = program.Names;
        var stack = new JsValue[unit.MaxOperandStack + 1];
        var scopes = new System.Collections.Generic.List<JsEnvironment>(4) { environment };
        var sp = 0;
        var pc = (int)unit.CodeOffset;
        var strict = unit.IsStrict;
        var current = pc;

        while (true)
        {
            try
            {
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

                        case JsOpcode.LoadArgument:
                        {
                            var at = U16(code, pc);

                            stack[sp++] = at < actualArguments.Length
                                ? actualArguments[at]
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.RestArguments:
                        {
                            var from = U16(code, pc);
                            var rest = Realm.NewArray();

                            for (var at = from; at < actualArguments.Length; at++)
                            {
                                Charge(1);
                                rest.Push(actualArguments[at]);
                            }

                            stack[sp++] = JsValue.Object(rest);
                            pc += 3;
                            break;
                        }

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

                        case JsOpcode.ArrayAppend:
                        {
                            var element = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            Charge(1);
                            array.SetIndex(array.Length, element);
                            pc++;
                            break;
                        }

                        case JsOpcode.ArrayHoles:
                        {
                            var array = (JsArray)stack[sp - 1].AsObject();
                            array.SetLength(array.Length + U16(code, pc));
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SpreadArray:
                        {
                            var source = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            var values = new System.Collections.Generic.List<JsValue>();
                            IterateInto(source, values);

                            foreach (var element in values)
                            {
                                Charge(1);
                                array.SetIndex(array.Length, element);
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.SpreadObject:
                        {
                            var source = stack[--sp];
                            CopyDataProperties(stack[sp - 1].AsObject(), source);
                            pc++;
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

                        case JsOpcode.CallSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, spread);
                            pc++;
                            break;
                        }

                        case JsOpcode.ConstructSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, spread);
                            pc++;
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

                        case JsOpcode.RequireCoercible:
                        {
                            var subject = stack[sp - 1];

                            if (subject.IsNullish)
                            {
                                var named = names[U16(code, pc)];

                                ThrowTypeError(
                                    named.Length == 0
                                        ? "Cannot destructure " + Describe(subject)
                                        : "Cannot destructure property '" + named + "' of " +
                                            Describe(subject));
                            }

                            pc += 3;
                            break;
                        }

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

                        case JsOpcode.IterateStart:
                            stack[sp - 1] = JsValue.Object(GetIterator(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.IterateNext:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (TryIterateNext(record, out var element))
                            {
                                stack[sp++] = element;
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.IterateRest:
                            stack[sp - 1] = JsValue.Object(
                                DrainIterator((JsIteratorRecord)stack[sp - 1].AsObject()));

                            pc++;
                            break;

                        case JsOpcode.IterateClose:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (code[pc + 1] == 0)
                            {
                                CloseIterator(record);
                            }
                            else
                            {
                                CloseIteratorQuietly(record);
                            }

                            pc += 2;
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
        }
    }

    /// <summary>Unpacks the argument Array a spread call built into the array a call takes.</summary>
    /// <remarks>
    /// The Array was built by this lowering and nothing else can reach it, so the dense elements
    /// are read directly. It is the one place in the iteration work where that is honest: the
    /// protocol already ran, when the spread appended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private JsValue[] ArgumentsOf(JsValue packed)
    {
        var array = (JsArray)packed.AsObject();
        var count = (int)array.Length;

        if (count == 0)
        {
            return System.Array.Empty<JsValue>();
        }

        Charge((ulong)count);
        var arguments = new JsValue[count];

        for (var at = 0; at < count; at++)
        {
            var element = at < array.DenseCount ? array.DenseAt(at) : JsValue.Undefined;
            arguments[at] = element.IsEmpty ? JsValue.Undefined : element;
        }

        return arguments;
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
