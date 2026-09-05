// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   25
// Annotated:        25/25
// Exempt:           0
// Human-reviewed:   0/25
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       25
//
// GENERATED - DO NOT EDIT MANUALLY

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
    /// <summary>The characters <c>encodeURIComponent</c> leaves alone beyond the alphanumerics.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0F257C
    // Broiler-Human:        PENDING
    private const string GlobalUriUnescapedComponent = "-_.!~*'()";

    /// <summary>The characters <c>encodeURI</c> leaves alone beyond the alphanumerics.</summary>
    /// <remarks>
    /// The component set, plus <c>uriReserved</c> and <c>#</c>. A URI's own punctuation has to
    /// survive encoding or the result no longer parses as the URI it came from.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=32C003
    // Broiler-Human:        PENDING
    private const string GlobalUriUnescapedUri = "-_.!~*'();/?:@&=+$,#";

    /// <summary>The characters <c>decodeURI</c> leaves escaped when it finds them escaped.</summary>
    /// <remarks>
    /// The mirror of the extra set above: decoding <c>%2F</c> to <c>/</c> would change the
    /// structure of the URI rather than its content, so the escape is copied through verbatim.
    /// <c>decodeURIComponent</c> passes the empty set and therefore decodes everything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=320485
    // Broiler-Human:        PENDING
    private const string GlobalUriPreservedUri = ";/?:@&=+$,#";

    /// <summary>The characters Annex B's <c>escape</c> leaves alone beyond the alphanumerics.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7DB1FE
    // Broiler-Human:        PENDING
    private const string GlobalEscapeUnescaped = "@*_+-./";

    /// <summary>The escape hex alphabet, upper-case because the specification says upper-case.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=14437D
    // Broiler-Human:        PENDING
    private const string GlobalUriHexDigits = "0123456789ABCDEF";

    /// <summary>Builds the global object's non-constructor bindings.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CD15FE
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
        SetupGlobalUriFunctions(host);
        SetupGlobalHostFunctions(host);
    }

    /// <summary>Defines <c>parseInt</c>, <c>parseFloat</c>, <c>isNaN</c> and <c>isFinite</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=494845
    // Broiler-Human:        PENDING
    private void SetupGlobalNumericFunctions(JsObject host)
    {
        Method(host, "parseInt", 2, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfGlobal(arguments, 0));
            var radix = engine.ToInt32(ArgOfGlobal(arguments, 1));
            engine.Charge((ulong)text.Length + 1);

            // JsNumberFormat.ParseInt owns the whole of the specification's shape: the leading
            // whitespace, the sign, the `0x` prefix that only radix 0 and 16 admit, the
            // out-of-range radix that answers NaN, and the stop at the first unusable character.
            return JsValue.Number(JsNumberFormat.ParseInt(text, radix));
        });

        Method(host, "parseFloat", 1, static (engine, thisValue, arguments) =>
        {
            var text = engine.ToStringValue(ArgOfGlobal(arguments, 0));
            engine.Charge((ulong)text.Length + 1);
            return JsValue.Number(JsNumberFormat.ParseFloat(text));
        });

        Method(host, "isNaN", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(double.IsNaN(engine.ToNumber(ArgOfGlobal(arguments, 0)))));

        Method(host, "isFinite", 1, static (engine, thisValue, arguments) =>
            JsValue.Boolean(double.IsFinite(engine.ToNumber(ArgOfGlobal(arguments, 0)))));
    }

    /// <summary>Defines the four URI functions and Annex B's two.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B348A4
    // Broiler-Human:        PENDING
    private void SetupGlobalUriFunctions(JsObject host)
    {
        Method(host, "encodeURI", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalUriEncode(
                engine, engine.ToStringValue(ArgOfGlobal(arguments, 0)), GlobalUriUnescapedUri)));

        Method(host, "encodeURIComponent", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalUriEncode(
                engine,
                engine.ToStringValue(ArgOfGlobal(arguments, 0)),
                GlobalUriUnescapedComponent)));

        Method(host, "decodeURI", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalUriDecode(
                engine, engine.ToStringValue(ArgOfGlobal(arguments, 0)), GlobalUriPreservedUri)));

        Method(host, "decodeURIComponent", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalUriDecode(
                engine, engine.ToStringValue(ArgOfGlobal(arguments, 0)), string.Empty)));

        // Annex B. `escape` and `unescape` are not the URI functions with different sets: they
        // work over code units rather than code points, they emit the `%uXXXX` form the URI
        // functions have never had, and `unescape` throws at nothing at all - a malformed escape
        // is copied through as the literal text it is.
        Method(host, "escape", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalEscapeText(engine, engine.ToStringValue(ArgOfGlobal(arguments, 0)))));

        Method(host, "unescape", 1, static (engine, thisValue, arguments) =>
            JsValue.String(GlobalUnescapeText(engine, engine.ToStringValue(ArgOfGlobal(arguments, 0)))));
    }

    /// <summary>Defines <c>print</c>, <c>$262</c> and <c>console</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DCC649
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
        // It refuses because THIS PROFILE HAS NO SHAPE IN WHICH A HOST COULD ANSWER WITH A FILE'S
        // CONTENTS. A value capability takes bytes and answers a `long` or an opaque reference, and
        // an opaque reference is by construction not dereferenceable - so there is no registration
        // any composition could make that would let this function return text. That is a limit of
        // core contract version 1 rather than a decision of this profile's, and roadmap section 18
        // is where a profile asks the core for an amendment. Until one lands, this refuses by name.
        //
        // The shape is `$262.agent`'s, one line below, and for the same stated reason: answering
        // `undefined` would let a program proceed on a false premise.
        GlobalRefuse(
            host,
            "read",
            1,
            "read: this profile's host-capability surface cannot carry a file's contents back to a " +
            "guest, so no composition can register a reader");

        var agent = new JsObject(ObjectPrototype);

        // Every member of `$262.agent` refuses. The agent API is about workers sharing a buffer,
        // and this profile has neither, so answering `undefined` would let a test proceed on a
        // false premise and report a pass it did not earn.
        GlobalRefuse(agent, "start", 1, "$262.agent.start: this profile runs no second agent");
        GlobalRefuse(agent, "broadcast", 1, "$262.agent.broadcast: this profile runs no second agent");
        GlobalRefuse(agent, "getReport", 0, "$262.agent.getReport: this profile runs no second agent");
        GlobalRefuse(agent, "sleep", 1, "$262.agent.sleep: this profile runs no second agent");
        GlobalRefuse(
            agent, "monotonicNow", 0, "$262.agent.monotonicNow: this profile runs no second agent");

        var harness = new JsObject(ObjectPrototype);
        harness.DefineBuiltIn("global", JsValue.Object(host));
        harness.DefineBuiltIn("agent", JsValue.Object(agent));

        GlobalRefuse(harness, "createRealm", 0, "$262.createRealm: this profile creates no nested realm");
        GlobalRefuse(
            harness,
            "detachArrayBuffer",
            1,
            "$262.detachArrayBuffer: this profile has no ArrayBuffer to detach");

        GlobalRefuse(
            harness,
            "evalScript",
            1,
            "$262.evalScript: the wide manifest admits no guest-initiated load");

        // INTERPRETING.md is explicit that `gc` throws when the host exposes no collection hook,
        // and this one does not: the collector is the CLR's and nothing here can ask it to run.
        GlobalRefuse(harness, "gc", 0, "$262.gc: this host exposes no collection hook");

        host.DefineBuiltIn("$262", JsValue.Object(harness));

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DA1CB5
    // Broiler-Human:        PENDING
    private static void GlobalWriteLine(JsEngine engine, JsValue[] arguments)
    {
        var line = new System.Text.StringBuilder();

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

    /// <summary>The specification's <c>Encode</c> over an explicit unescaped set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CF8B90
    // Broiler-Human:        PENDING
    private static string GlobalUriEncode(JsEngine engine, string text, string unescaped)
    {
        var result = new System.Text.StringBuilder(text.Length);

        for (var at = 0; at < text.Length; at++)
        {
            engine.Charge(1);
            var current = text[at];

            if (GlobalUriIsAlphanumeric(current) || GlobalUriInSet(unescaped, current))
            {
                result.Append(current);
                continue;
            }

            int point;

            if (current is >= '\uD800' and <= '\uDBFF')
            {
                // A leading surrogate is only half a code point. The other half has to be there
                // and has to be a trailing surrogate; anything else is a URIError and not a
                // replacement character, which is where every delegated implementation differs.
                if (at + 1 >= text.Length || text[at + 1] is < '\uDC00' or > '\uDFFF')
                {
                    throw engine.Error("URIError", "URI malformed");
                }

                point = 0x10000 + ((current - 0xD800) << 10) + (text[at + 1] - 0xDC00);
                at++;
            }
            else if (current is >= '\uDC00' and <= '\uDFFF')
            {
                throw engine.Error("URIError", "URI malformed");
            }
            else
            {
                point = current;
            }

            GlobalUriAppendUtf8(result, point);
        }

        return result.ToString();
    }

    /// <summary>The specification's <c>Decode</c> over an explicit preserved set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8EF1A3
    // Broiler-Human:        PENDING
    private static string GlobalUriDecode(JsEngine engine, string text, string preserved)
    {
        var result = new System.Text.StringBuilder(text.Length);
        var at = 0;

        while (at < text.Length)
        {
            engine.Charge(1);

            if (text[at] != '%')
            {
                result.Append(text[at]);
                at++;
                continue;
            }

            var start = at;
            var lead = GlobalUriOctetAt(engine, text, at);
            at += 3;

            if ((lead & 0x80) == 0)
            {
                var decoded = (char)lead;

                if (GlobalUriInSet(preserved, decoded))
                {
                    // Preserved: the three characters of the escape are copied through unchanged,
                    // so a second decode of the result is not a different string.
                    result.Append(text, start, 3);
                }
                else
                {
                    result.Append(decoded);
                }

                continue;
            }

            var length = GlobalUriSequenceLength(engine, lead);
            var point = lead & (0xFF >> (length + 1));

            for (var extra = 1; extra < length; extra++)
            {
                engine.Charge(1);
                var continuation = GlobalUriOctetAt(engine, text, at);
                at += 3;

                if ((continuation & 0xC0) != 0x80)
                {
                    throw engine.Error("URIError", "URI malformed");
                }

                point = (point << 6) | (continuation & 0x3F);
            }

            // An overlong encoding, a surrogate half and anything past the last plane are all
            // sequences a naive decoder accepts and the specification calls malformed.
            var minimum = length switch
            {
                2 => 0x80,
                3 => 0x800,
                _ => 0x10000,
            };

            if (point < minimum || point > 0x10FFFF || point is >= 0xD800 and <= 0xDFFF)
            {
                throw engine.Error("URIError", "URI malformed");
            }

            GlobalUriAppendCodePoint(result, point);
        }

        return result.ToString();
    }

    /// <summary>Reads the <c>%XX</c> at <paramref name="at"/>, or throws a <c>URIError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1F9732
    // Broiler-Human:        PENDING
    private static int GlobalUriOctetAt(JsEngine engine, string text, int at)
    {
        if (at + 2 >= text.Length || text[at] != '%')
        {
            throw engine.Error("URIError", "URI malformed");
        }

        var high = GlobalUriHexValue(text[at + 1]);
        var low = GlobalUriHexValue(text[at + 2]);

        if (high < 0 || low < 0)
        {
            throw engine.Error("URIError", "URI malformed");
        }

        return (high << 4) | low;
    }

    /// <summary>How many octets the UTF-8 sequence opened by <paramref name="lead"/> has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=27F987
    // Broiler-Human:        PENDING
    private static int GlobalUriSequenceLength(JsEngine engine, int lead) => lead switch
    {
        >= 0xC0 and <= 0xDF => 2,
        >= 0xE0 and <= 0xEF => 3,
        >= 0xF0 and <= 0xF7 => 4,
        _ => throw engine.Error("URIError", "URI malformed"),
    };

    /// <summary>Appends the UTF-8 octets of a code point, each as <c>%XX</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D06635
    // Broiler-Human:        PENDING
    private static void GlobalUriAppendUtf8(System.Text.StringBuilder text, int point)
    {
        if (point <= 0x7F)
        {
            GlobalUriAppendOctet(text, point);
        }
        else if (point <= 0x7FF)
        {
            GlobalUriAppendOctet(text, 0xC0 | (point >> 6));
            GlobalUriAppendOctet(text, 0x80 | (point & 0x3F));
        }
        else if (point <= 0xFFFF)
        {
            GlobalUriAppendOctet(text, 0xE0 | (point >> 12));
            GlobalUriAppendOctet(text, 0x80 | ((point >> 6) & 0x3F));
            GlobalUriAppendOctet(text, 0x80 | (point & 0x3F));
        }
        else
        {
            GlobalUriAppendOctet(text, 0xF0 | (point >> 18));
            GlobalUriAppendOctet(text, 0x80 | ((point >> 12) & 0x3F));
            GlobalUriAppendOctet(text, 0x80 | ((point >> 6) & 0x3F));
            GlobalUriAppendOctet(text, 0x80 | (point & 0x3F));
        }
    }

    /// <summary>Appends one octet as <c>%XX</c> in upper-case hex.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DC7520
    // Broiler-Human:        PENDING
    private static void GlobalUriAppendOctet(System.Text.StringBuilder text, int octet)
    {
        text.Append('%');
        text.Append(GlobalUriHexDigits[(octet >> 4) & 0xF]);
        text.Append(GlobalUriHexDigits[octet & 0xF]);
    }

    /// <summary>Appends a code point as one code unit or as a surrogate pair.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EE4D18
    // Broiler-Human:        PENDING
    private static void GlobalUriAppendCodePoint(System.Text.StringBuilder text, int point)
    {
        if (point <= 0xFFFF)
        {
            text.Append((char)point);
            return;
        }

        var shifted = point - 0x10000;
        text.Append((char)(0xD800 + (shifted >> 10)));
        text.Append((char)(0xDC00 + (shifted & 0x3FF)));
    }

    /// <summary>Annex B's <c>escape</c>, which works over code units and never throws.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9F8521
    // Broiler-Human:        PENDING
    private static string GlobalEscapeText(JsEngine engine, string text)
    {
        var result = new System.Text.StringBuilder(text.Length);

        for (var at = 0; at < text.Length; at++)
        {
            engine.Charge(1);
            var current = text[at];

            if (GlobalUriIsAlphanumeric(current) || GlobalUriInSet(GlobalEscapeUnescaped, current))
            {
                result.Append(current);
            }
            else if (current < 256)
            {
                GlobalUriAppendOctet(result, current);
            }
            else
            {
                // The `%uXXXX` form is this function's alone: a code unit above 255 is emitted as
                // itself rather than as the UTF-8 octets of the code point it may be half of.
                result.Append("%u");
                result.Append(GlobalUriHexDigits[(current >> 12) & 0xF]);
                result.Append(GlobalUriHexDigits[(current >> 8) & 0xF]);
                result.Append(GlobalUriHexDigits[(current >> 4) & 0xF]);
                result.Append(GlobalUriHexDigits[current & 0xF]);
            }
        }

        return result.ToString();
    }

    /// <summary>Annex B's <c>unescape</c>, which copies a malformed escape through verbatim.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=65547A
    // Broiler-Human:        PENDING
    private static string GlobalUnescapeText(JsEngine engine, string text)
    {
        var result = new System.Text.StringBuilder(text.Length);
        var at = 0;

        while (at < text.Length)
        {
            engine.Charge(1);
            var current = text[at];

            if (current == '%')
            {
                var first = at + 5 < text.Length && text[at + 1] == 'u'
                    ? GlobalUriHexValue(text[at + 2])
                    : -1;

                if (first >= 0)
                {
                    var second = GlobalUriHexValue(text[at + 3]);
                    var third = GlobalUriHexValue(text[at + 4]);
                    var fourth = GlobalUriHexValue(text[at + 5]);

                    if (second >= 0 && third >= 0 && fourth >= 0)
                    {
                        current = (char)((first << 12) | (second << 8) | (third << 4) | fourth);
                        at += 5;
                        result.Append(current);
                        at++;
                        continue;
                    }
                }

                if (at + 2 < text.Length)
                {
                    var high = GlobalUriHexValue(text[at + 1]);
                    var low = GlobalUriHexValue(text[at + 2]);

                    if (high >= 0 && low >= 0)
                    {
                        current = (char)((high << 4) | low);
                        at += 2;
                    }
                }
            }

            result.Append(current);
            at++;
        }

        return result.ToString();
    }

    /// <summary>The value of one hex digit, or a negative number when it is not one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5E655A
    // Broiler-Human:        PENDING
    private static int GlobalUriHexValue(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'a' and <= 'f' => character - 'a' + 10,
        >= 'A' and <= 'F' => character - 'A' + 10,
        _ => -1,
    };

    /// <summary>Whether <paramref name="character"/> is an ASCII letter or digit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D0AF08
    // Broiler-Human:        PENDING
    private static bool GlobalUriIsAlphanumeric(char character) =>
        character is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= '0' and <= '9');

    /// <summary>Whether <paramref name="set"/> holds <paramref name="character"/>.</summary>
    /// <remarks>
    /// A loop rather than <c>IndexOf</c>: the sets are under twenty characters, and a hand-written
    /// scan has no culture to specify and no analyzer to satisfy about which comparison it means.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BACFBC
    // Broiler-Human:        PENDING
    private static bool GlobalUriInSet(string set, char character)
    {
        for (var at = 0; at < set.Length; at++)
        {
            if (set[at] == character)
            {
                return true;
            }
        }

        return false;
    }
}
