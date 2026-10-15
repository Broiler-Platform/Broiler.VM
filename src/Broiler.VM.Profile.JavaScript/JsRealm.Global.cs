// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           1
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Text;
using Broiler.ECMAScript;
using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Text;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The global object's own bindings that are not constructors: three value properties, the ten
/// function properties clause 19 defines, and the host hooks the two target workloads reach for.
/// </summary>
/// <remarks>
/// <para>
/// <b>The URI functions are the specification's and not <c>System.Uri</c>'s.</b> The platform's
/// escaper has its own opinion of which characters are safe, normalises what it is given, and
/// answers a replacement character where the language answers a <c>URIError</c>. The four written
/// here are the specification's <c>Encode</c> and <c>Decode</c> over an explicit preserved set:
/// they percent-escape the UTF-8 octets of a code point in upper-case hex, they refuse a lone
/// surrogate, and they refuse a truncated, overlong or out-of-range escape sequence. A conformance
/// suite finds every one of those differences and an arithmetic corpus finds none of them.
/// </para>
/// <para>
/// <b>What is deliberately absent is as load-bearing as what is here.</b> There is no
/// <c>window</c>, no <c>document</c>, no <c>self</c>, no <c>global</c> and no <c>setTimeout</c>.
/// The Octane harness tests for <c>window.setTimeout</c> and yields to it when it finds one, so a
/// profile that defined a browser-shaped global out of politeness would run the benchmark's first
/// slice and then sit forever, having printed no score. The absence is the feature.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Builds the global object's non-constructor bindings.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A578B6
    // Broiler-Human:        PENDING
    private void SetupGlobal()
    {
        var host = GlobalObject;

        // The three value properties. `undefined`, `NaN` and `Infinity` are non-writable,
        // non-enumerable and non-configurable: assigning to one is a no-op in sloppy mode and a
        // TypeError in strict mode, which is exactly what DefineFrozen produces.
        host.DefineFrozen("undefined", JsValue.Undefined);
        host.DefineFrozen("NaN", JsValue.Number(double.NaN));
        host.DefineFrozen("Infinity", JsValue.Number(double.PositiveInfinity));

        // `globalThis` is the one that is writable and configurable, and it holds the object it
        // is defined on - which makes the property a cycle, and is the specification's intent.
        host.DefineBuiltIn("globalThis", JsValue.Object(host));

        SetupGlobalNumericFunctions(host);

        // `Number.parseInt` and `Number.parseFloat` ARE the global functions, the same objects, not
        // copies (21.1.2.12 and 21.1.2.13). `Number` is set up first and holds a placeholder in
        // each slot, so writing the global's value over it keeps the key where it was. Until
        // 2026-10-03 they were two pairs of functions (JSC-252).
        if (host.TryGetOwnProperty("Number", out var number) && number.Value.AsObjectOrNull() is { } numberObject)
        {
            foreach (var shared in (string[])["parseFloat", "parseInt"])
            {
                host.TryGetOwnProperty(shared, out var global);
                numberObject.SetOwnProperty(shared, global);
            }
        }

        SetupGlobalUriFunctions(host);

        // A SHADOW REALM'S GLOBAL HOLDS WHAT THE LANGUAGE DEFINES AND NOTHING A HOST ADDS
        // (JSD-0030 D7): no `print`, `console`, `read` or `$262`.
        if (!Shadow)
        {
            SetupGlobalHostFunctions(host);
        }
    }

    /// <summary>Defines <c>parseInt</c>, <c>parseFloat</c>, <c>isNaN</c> and <c>isFinite</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=475E3E
    // Broiler-Human:        PENDING
    private void SetupGlobalNumericFunctions(JsObject host)
    {
        Method(host, "parseInt", 2, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfGlobal(arguments, 0));
            var radix = engine.ToInt32(ArgOfGlobal(arguments, 1));
            // The library owns the whole of the specification's shape: the leading white space,
            // the sign, the `0x` prefix that only radix 0 and 16 admit, the out-of-range radix that
            // answers NaN, and the stop at the first unusable character. It charges the text's
            // length; the unit beyond it is the call's, as before.
            engine.Charge(1UL);
            return JsValue.Number(NumberParsing.ParseInt(text, radix, engine.WorkCharge));
        });

        Method(host, "parseFloat", 1, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfGlobal(arguments, 0));
            engine.Charge(1UL);
            return JsValue.Number(NumberParsing.ParseFloat(text, engine.WorkCharge));
        });

        Method(host, "isNaN", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(double.IsNaN(engine.ToNumber(ArgOfGlobal(arguments, 0)))));

        Method(host, "isFinite", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(double.IsFinite(engine.ToNumber(ArgOfGlobal(arguments, 0)))));
    }

    /// <summary>Defines the four URI functions and Annex B's two.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=93664E
    // Broiler-Human:        PENDING
    private void SetupGlobalUriFunctions(JsObject host)
    {
        // The codecs are the shared library's (JSD-0071), which charge what these functions
        // charged before. `encodeURI` leaves the reserved characters and `#` alone and `decodeURI`
        // keeps their escapes; the component functions pass the empty set.
        Method(host, "encodeURI", 1, static (engine, thisValue, arguments) =>
            GlobalUriAnswer(engine, UriCodec.Encode(engine.ToStringValue(ArgOfGlobal(arguments, 0)), UriCodec.ReservedAndNumberSign, out var result, out _, engine.WorkCharge), result));

        Method(host, "encodeURIComponent", 1, static (engine, thisValue, arguments) =>
            GlobalUriAnswer(engine, UriCodec.Encode(engine.ToStringValue(ArgOfGlobal(arguments, 0)), string.Empty, out var result, out _, engine.WorkCharge), result));

        Method(host, "decodeURI", 1, static (engine, thisValue, arguments) =>
            GlobalUriAnswer(engine, UriCodec.Decode(engine.ToStringValue(ArgOfGlobal(arguments, 0)), UriCodec.ReservedAndNumberSign, out var result, out _, engine.WorkCharge), result));

        Method(host, "decodeURIComponent", 1, static (engine, thisValue, arguments) =>
            GlobalUriAnswer(engine, UriCodec.Decode(engine.ToStringValue(ArgOfGlobal(arguments, 0)), string.Empty, out var result, out _, engine.WorkCharge), result));

        // Annex B. `escape` and `unescape` are not the URI functions with different sets: they
        // work over code units rather than code points, they emit the `%uXXXX` form the URI
        // functions have never had, and `unescape` throws at nothing at all - a malformed escape
        // is copied through as the literal text it is.
        Method(host, "escape", 1, static (engine, thisValue, arguments) =>
            JsValue.String(UriCodec.Escape(engine.ToStringValue(ArgOfGlobal(arguments, 0)), engine.WorkCharge)));

        Method(host, "unescape", 1, static (engine, thisValue, arguments) =>
            JsValue.String(UriCodec.Unescape(engine.ToStringValue(ArgOfGlobal(arguments, 0)), engine.WorkCharge)));
    }

    /// <summary>Defines <c>print</c>, <c>$262</c> and <c>console</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6B957D
    // Broiler-Human:        PENDING
    private void SetupGlobalHostFunctions(JsObject host)
    {
        // `print` in exactly the shape the conformance suite's INTERPRETING.md requires: every
        // argument through ToString, joined by one space, handed to the host in one call. When no
        // host wired an Output the call is a no-op rather than an error, because a test that
        // prints and is scored on its exit path must still reach that path.
        Method(host, "print", 1, static (engine, thisValue, arguments) =>
        {
            GlobalWriteLine(engine, arguments);
            return JsValue.Undefined;
        });

        // `read` EXISTS AND REFUSES, AND BOTH HALVES OF THAT ARE DELIBERATE.
        //
        // It exists because a shell-shaped environment probe assigns it without calling it. The
        // emscripten runtime an asm.js workload carries decides which host it is on by asking for
        // `window`, `process` and `importScripts`, concludes "a shell" when it finds none of them,
        // and then reads the global `read` to wire it into its own module object. It never calls
        // it: the data such a workload needs is embedded in the file. A host that made that
        // assignment throw would be refusing a whole program over a capability the program does not
        // use, and would answer a `ReferenceError` that names nothing a reader could act on.
        //
        // It refuses because NO CAPABILITY A COMPOSITION COULD REGISTER WOULD LET IT ANSWER. A
        // value capability takes bytes and answers a `long` or an opaque reference, and an opaque
        // reference is by construction not dereferenceable - so no registration in the capability
        // table would let this function return text. That is a limit of core contract version 1
        // rather than a decision of this profile's, and roadmap section 18 is where a profile asks
        // the core for an amendment.
        //
        // WHAT IS NO LONGER TRUE IS THAT NOTHING COULD. The host-object surface is a second door,
        // and a composition that registers it can install a `read` of its own on the global that
        // answers with whatever it likes. This one is the answer for a composition that did not:
        // the built-in exists so the environment probe finds a name, and refuses because this
        // realm has no reader unless somebody installed one *(corrected: JSC-212)*.
        //
        // The shape is `$262.agent`'s, one line below, and for the same stated reason: answering
        // `undefined` would let a program proceed on a false premise. Its message said "no
        // composition can register a reader", which the paragraph above had already retracted;
        // it says what is true of the realm now (JSP-10, JSC-240), and section 13 of the roadmap
        // states the choice of present-and-refusing over absent for every member here.
        GlobalRefuse(host, "read", 1,
            "read: no reader is installed in this realm - the host-capability table cannot carry " +
            "a file's contents back to a guest, and this composition installed none through the " +
            "host-object surface");

        var agent = new JsObject(ObjectPrototype);

        // Every member of `$262.agent` refuses. A second agent is a runtime its host starts and
        // hands a shared block to (JSD-0042), and this profile starts none, so answering
        // `undefined` would let a test proceed on a false premise and report a pass it did not
        // earn. A host that starts agents replaces the object, as the conformance harness does.
        GlobalRefuse(agent, "start", 1, "$262.agent.start: this profile runs no second agent");
        GlobalRefuse(agent, "broadcast", 1, "$262.agent.broadcast: this profile runs no second agent");
        GlobalRefuse(agent, "getReport", 0, "$262.agent.getReport: this profile runs no second agent");
        GlobalRefuse(agent, "sleep", 1, "$262.agent.sleep: this profile runs no second agent");
        GlobalRefuse(agent, "monotonicNow", 0, "$262.agent.monotonicNow: this profile runs no second agent");

        var harness = new JsObject(ObjectPrototype);
        harness.DefineBuiltIn("global", JsValue.Object(host));
        harness.DefineBuiltIn("agent", JsValue.Object(agent));

        // `createRealm` BUILDS AN ORDINARY NEW REALM ON THIS ENGINE AND ANSWERS ITS `$262`, as
        // INTERPRETING.md defines it (JSD-0030 SR-7): a global object and intrinsics of its own, the
        // same surface set, the agent's Symbols and job queue, and the cost charged to this
        // allowance. Until 2026-10-04 it refused, because an engine held one realm (JSC-264).
        Method(harness, "createRealm", 0, static (engine, thisValue, arguments) =>
            JsValue.Object(engine.CreateRealm().Harness));
        // THE LANGUAGE MAKES DETACHMENT A HOST'S ACT, and a realm whose host installed none has no
        // way to perform it for a guest; `ArrayBuffer.prototype.transfer` is the language's own
        // door and needs no help from here. A composition that means to offer it replaces this
        // member through `JsHostRealm.DetachArrayBuffer`, which is what the conformance harness
        // does. (Corrected 2026-09-21, JSeal follow-up VM-FIX-A: the message read "this profile
        // has no ArrayBuffer to detach", which stopped being true when `ArrayBuffer` landed.)
        GlobalRefuse(harness, "detachArrayBuffer", 1, "$262.detachArrayBuffer: the host of this realm installed no detach operation");

        // RUNNING A SCRIPT IS THE HOST'S ACT TOO, and the same shape: a composition that means to
        // offer it replaces this member through `JsHostRealm.EvaluateScript`, which is what the
        // conformance harness does (JSeal V15-host). (Corrected 2026-09-21: the message read "the
        // wide manifest admits no guest-initiated load", which the manifest never decided.)
        GlobalRefuse(harness, "evalScript", 1, "$262.evalScript: the host of this realm installed no script evaluation");

        // INTERPRETING.md is explicit that `gc` throws when the host exposes no collection hook,
        // and this one does not: the collector is the CLR's and nothing here can ask it to run.
        GlobalRefuse(harness, "gc", 0, "$262.gc: this host exposes no collection hook");

        host.DefineBuiltIn("$262", JsValue.Object(harness));
        Harness = harness;

        // NEITHER TARGET WORKLOAD NEEDS `console`. The conformance suite calls `print` and the
        // Octane harness calls neither, so this is here for one reason only: a person pointing the
        // command line at an ordinary script reaches for `console.log` first, and a profile that
        // answered "console is not defined" would read as broken rather than as narrow.
        var console = new JsObject(ObjectPrototype);

        Method(console, "log", 1, static (engine, thisValue, arguments) =>
        {
            GlobalWriteLine(engine, arguments);
            return JsValue.Undefined;
        });

        host.DefineBuiltIn("console", JsValue.Object(console));
    }

    /// <summary>The realm's <c>$262</c>, which <c>createRealm</c> answers for a realm it built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4D4647
    // Broiler-Human:        PENDING
    internal JsObject Harness { get; private set; } = null!;

    /// <summary>Reads one argument, answering <c>undefined</c> past the end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7DAE04
    // Broiler-Human:        PENDING
    private static JsValue ArgOfGlobal(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Defines a method on <paramref name="host"/> that refuses with a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0A172E
    // Broiler-Human:        PENDING
    private void GlobalRefuse(JsObject host, string name, int arity, string message) =>
        Method(host, name, arity, (engine, thisValue, arguments) => engine.ThrowTypeError(message));

    /// <summary>Joins the arguments with one space and hands the line to the host.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=264021
    // Broiler-Human:        PENDING
    private static void GlobalWriteLine(JsEngine engine, JsValue[] arguments)
    {
        var line = new StringBuilder();

        for (var at = 0; at < arguments.Length; at++)
        {
            engine.Charge(1);

            if (at > 0)
            {
                line.Append(' ');
            }

            line.Append(engine.ToStringValue(arguments[at]));
        }

        engine.Write(line.ToString());
    }

    /// <summary>A URI function's answer: the codec's String, or its <c>URIError</c>.</summary>
    /// <remarks>
    /// A lone surrogate to encode, and an escape that is malformed, an octet that cannot lead or
    /// continue a sequence, an overlong sequence, an encoded surrogate or a value past the last plane
    /// to decode, are all one error in the language. The library names which; this message does not.
    /// Neither does it turn a lone surrogate into a replacement character, which is where every
    /// delegated implementation differs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=143113
    // Broiler-Human:        PENDING
    private static JsValue GlobalUriAnswer(JsEngine engine, ResultStatus status, string result) =>
        status == ResultStatus.Completed
            ? JsValue.String(result)
            : throw engine.Error("URIError", "URI malformed");
}
