// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           6
// Human-reviewed:   0/32
// IP risk:          Medium
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       32
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The <c>RegExp</c> intrinsic, its prototype, and the four <c>String.prototype</c> methods that
/// take one.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS AN EXPLICIT APPROXIMATION, DECLARED HERE RATHER THAN DISCOVERED IN THE FIELD.</b>
/// The pattern is not compiled by a matcher this profile owns: it is translated, lightly, and
/// handed to <see cref="System.Text.RegularExpressions.Regex"/> with
/// <see cref="System.Text.RegularExpressions.RegexOptions.ECMAScript"/> set, which is what puts
/// <c>\d</c>, <c>\w</c>, <c>\s</c>, backreferences and quantifiers onto the language's meanings
/// rather than .NET's. What that buys is a working <c>exec</c>, <c>test</c>, <c>match</c>,
/// <c>search</c>, <c>replace</c> and <c>split</c> for a fraction of the cost of writing a
/// backtracking matcher. What it costs is the list below, which is the whole of what is known to
/// differ - anything found later belongs on it.
/// </para>
/// <list type="bullet">
/// <item>
/// <b>Capture numbering when named groups are present.</b> .NET numbers the unnamed groups first
/// and the named groups after them; the language numbers every group left to right. A pattern
/// mixing <c>(a)</c> with <c>(?&lt;n&gt;b)</c> therefore reports its captures - and its
/// <c>$1</c>..<c>$9</c> substitutions - in an order the language does not. Patterns with no named
/// groups are unaffected, and there is no <c>groups</c> property on a result either way.
/// </item>
/// <item>
/// <b><c>$</c> before a final newline.</b> .NET's <c>$</c> matches at the end of the input and
/// also immediately before a trailing <c>\n</c>; the language's, without <c>m</c>, matches only at
/// the very end.
/// </item>
/// <item>
/// <b><c>y</c> is emulated, not native, and only by <c>exec</c> and <c>test</c>.</b> .NET has no
/// sticky mode, so a sticky match is a forward search whose result is discarded unless it began
/// exactly at <c>lastIndex</c>. The answer is right; the cost is not, because a failing sticky
/// match still scans to the end. The four String methods below do not apply that anchoring at
/// all - to <c>match</c>, <c>search</c>, <c>replace</c> and <c>split</c>, <c>y</c> reads as
/// absent.
/// </item>
/// <item>
/// <b><c>u</c> is recorded and otherwise inert.</b> No code-point-wise matching, no
/// <c>\u{...}</c>, no <c>\p{...}</c> property escapes, and a surrogate pair is still two units to
/// every index this file produces.
/// </item>
/// <item>
/// <b>The ECMAScript option is dropped when .NET refuses it.</b> .NET forbids
/// <c>Singleline</c> in combination with <c>ECMAScript</c>, so every pattern carrying the <c>s</c>
/// flag takes the retry path, as does any pattern .NET's ECMAScript parser rejects. On that path
/// <c>\d</c>, <c>\w</c> and <c>\s</c> widen to their Unicode meanings, which is visible: under
/// <c>/\d/s</c> a Devanagari digit matches and under <c>/\d/</c> it does not.
/// </item>
/// <item>
/// <b>The accepted grammar is neither a subset nor a superset.</b> .NET accepts constructs the
/// language has no syntax for - <c>\p{L}</c>, character-class subtraction, conditionals,
/// balancing groups - and rejects some Annex B leniencies the language accepts, such as a lone
/// <c>{</c> used as a literal or a backreference to a group that does not exist. The first is
/// silently permitted here; the second is a <c>SyntaxError</c> the language would not have
/// raised. Two classes .NET rejects outright are translated instead: <c>[]</c> becomes an
/// assertion that never matches and <c>[^]</c> becomes <c>[\s\S]</c>, and <c>[</c> inside a class
/// is escaped so .NET's subtraction syntax cannot capture it.
/// </item>
/// <item>
/// <b>No <c>Symbol.match</c>, <c>Symbol.replace</c>, <c>Symbol.search</c> or <c>Symbol.split</c>
/// protocol.</b> This surface has no Symbols at all, so the six built-ins here test for a RegExp
/// object rather than dispatching on a method: subclassing <c>RegExp</c> and overriding
/// <c>exec</c> changes nothing, and neither does an object that merely looks like one. There is
/// no <c>matchAll</c>, no <c>replaceAll</c>, and no <c>d</c> or <c>v</c> flag.
/// </item>
/// <item>
/// <b>Every matcher carries a five-second match allowance.</b> A catastrophic backtrack becomes a
/// <c>RangeError</c> the guest can see rather than a host process that stops answering. The
/// language has no such error; a hung interpreter is the worse of the two.
/// </item>
/// </list>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>How long a single match may run before the matcher abandons it.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=EFBD27
    // Broiler-Human:        PENDING
    private static readonly System.TimeSpan RegExpMatchAllowance = System.TimeSpan.FromSeconds(5);

    /// <summary>Builds <c>RegExp</c>, <c>RegExp.prototype</c>, and the String methods that take one.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=7C33C6
    // Broiler-Human:        PENDING
    private void SetupRegExp()
    {
        var prototype = RegExpPrototype;

        Constructor(
            "RegExp",
            2,
            prototype,
            (engine, thisValue, arguments) =>
            {
                var pattern = ArgOfRegExp(arguments, 0);
                var flags = ArgOfRegExp(arguments, 1);

                // CALLED AS A FUNCTION ON A REGEXP WITH NO FLAGS OF ITS OWN, `RegExp` HANDS BACK
                // THE VERY OBJECT IT WAS GIVEN. Constructed, it always produces a fresh one, which
                // is what makes `new RegExp(re)` a way to reset a shared `lastIndex`.
                if (flags.Type == JsType.Undefined && pattern.AsObjectOrNull() is RegExpObject given)
                {
                    return JsValue.Object(given);
                }

                return JsValue.Object(RegExpCreate(engine, pattern, flags));
            },
            (engine, thisValue, arguments) => JsValue.Object(
                RegExpCreate(engine, ArgOfRegExp(arguments, 0), ArgOfRegExp(arguments, 1))));

        Method(prototype, "exec", 1, (engine, thisValue, arguments) =>
        {
            var target = RegExpThis(engine, thisValue, "exec");
            return RegExpExecute(engine, target, engine.ToStringValue(ArgOfRegExp(arguments, 0)));
        });

        Method(prototype, "test", 1, (engine, thisValue, arguments) =>
        {
            var target = RegExpThis(engine, thisValue, "test");
            var input = engine.ToStringValue(ArgOfRegExp(arguments, 0));
            return JsValue.Boolean(RegExpExecute(engine, target, input).Type != JsType.Null);
        });

        // `toString` reads `source` and `flags` off the receiver rather than off a slot, which is
        // both what the specification says and what makes `RegExp.prototype.toString()` answer
        // "/(?:)/" instead of throwing.
        Method(prototype, "toString", 0, (engine, thisValue, arguments) =>
        {
            if (!thisValue.IsObject)
            {
                return engine.ThrowTypeError(
                    "RegExp.prototype.toString called on a value that is not an object");
            }

            var source = engine.ToStringValue(engine.GetProperty(thisValue, "source"));
            var flags = engine.ToStringValue(engine.GetProperty(thisValue, "flags"));
            return JsValue.String("/" + source + "/" + flags);
        });

        RegExpGetter(prototype, "source", (engine, thisValue, arguments) =>
        {
            if (thisValue.AsObjectOrNull() is RegExpObject target)
            {
                return JsValue.String(RegExpSourceText(target.Source));
            }

            return ReferenceEquals(thisValue.AsObjectOrNull(), prototype)
                ? JsValue.String("(?:)")
                : engine.ThrowTypeError("RegExp.prototype.source requires a RegExp receiver");
        });

        RegExpGetter(prototype, "flags", (engine, thisValue, arguments) =>
        {
            if (thisValue.AsObjectOrNull() is RegExpObject target)
            {
                return JsValue.String(target.Flags);
            }

            return ReferenceEquals(thisValue.AsObjectOrNull(), prototype)
                ? JsValue.String(string.Empty)
                : engine.ThrowTypeError("RegExp.prototype.flags requires a RegExp receiver");
        });

        RegExpFlagGetter(prototype, "global", 'g');
        RegExpFlagGetter(prototype, "ignoreCase", 'i');
        RegExpFlagGetter(prototype, "multiline", 'm');
        RegExpFlagGetter(prototype, "sticky", 'y');
        RegExpFlagGetter(prototype, "unicode", 'u');

        // ---- the String methods that take a RegExp ------------------------------------------
        //
        // `match` and `search` are defined here because they exist only for regular expressions.
        // `replace` and `split` are REDEFINED here: SetupString ran first and defined a
        // string-only pair, and this file runs after it and overwrites both, because the
        // specification's versions accept either a RegExp or a string and only this file knows
        // what a RegExp is. The string-pattern behaviour is not lost - each of the two tests its
        // first argument for a RegExp object and falls back to the string path when it is not
        // one, so `"a,b".split(",")` and `"aa".replace("a", "b")` answer exactly as before.

        Method(StringPrototype, "match", 1, (engine, thisValue, arguments) =>
        {
            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, ArgOfRegExp(arguments, 0));
            engine.Charge((ulong)input.Length + 16);

            if (!pattern.Global)
            {
                return RegExpExecute(engine, pattern, input);
            }

            // A GLOBAL `match` COLLECTS THE MATCHED TEXT AND NOTHING ELSE - no index, no input, no
            // captures - and leaves `lastIndex` at zero however it ends.
            pattern.LastIndex = JsValue.Number(0);
            var found = NewArray();
            var at = 0;

            while (at <= input.Length)
            {
                engine.Charge((ulong)(input.Length - at) + 4);
                var match = RegExpRun(engine, pattern, input, at);

                if (!match.Success)
                {
                    break;
                }

                found.Push(JsValue.String(match.Value));
                at = match.Length == 0 ? match.Index + 1 : match.Index + match.Length;
            }

            pattern.LastIndex = JsValue.Number(0);
            return found.Length == 0 ? JsValue.Null : JsValue.Object(found);
        });

        Method(StringPrototype, "search", 1, (engine, thisValue, arguments) =>
        {
            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, ArgOfRegExp(arguments, 0));
            engine.Charge((ulong)input.Length + 16);

            // `search` never consults and never disturbs `lastIndex`, global flag or not.
            var match = RegExpRun(engine, pattern, input, 0);
            return JsValue.Number(match.Success ? match.Index : -1);
        });

        Method(StringPrototype, "replace", 2, (engine, thisValue, arguments) =>
        {
            var input = RegExpStringThis(engine, thisValue);
            var search = ArgOfRegExp(arguments, 0);
            var replacement = ArgOfRegExp(arguments, 1);
            engine.Charge((ulong)input.Length + 16);

            return JsValue.String(
                search.AsObjectOrNull() is RegExpObject pattern
                    ? RegExpReplaceAll(engine, pattern, input, replacement)
                    : RegExpReplaceText(engine, input, engine.ToStringValue(search), replacement));
        });

        Method(StringPrototype, "split", 2, (engine, thisValue, arguments) =>
        {
            var input = RegExpStringThis(engine, thisValue);
            var separator = ArgOfRegExp(arguments, 0);
            var bound = ArgOfRegExp(arguments, 1);
            engine.Charge((ulong)input.Length + 16);

            var limit = bound.Type == JsType.Undefined ? 4294967295L : engine.ToUint32(bound);

            if (limit == 0)
            {
                return JsValue.Object(NewArray());
            }

            if (separator.AsObjectOrNull() is RegExpObject pattern)
            {
                return JsValue.Object(RegExpSplitPattern(engine, pattern, input, limit));
            }

            if (separator.Type == JsType.Undefined)
            {
                var whole = NewArray();
                whole.Push(JsValue.String(input));
                return JsValue.Object(whole);
            }

            return JsValue.Object(
                RegExpSplitText(engine, input, engine.ToStringValue(separator), limit));
        });
    }

    /// <summary>Reads one argument, which may not have been passed.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=6F7964
    // Broiler-Human:        PENDING
    private static JsValue ArgOfRegExp(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>Defines a getter-only accessor on <paramref name="host"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=861B32
    // Broiler-Human:        PENDING
    private void RegExpGetter(JsObject host, string name, JsNativeBody body) =>
        host.SetOwnProperty(
            name,
            JsProperty.Accessor(
                Native("get " + name, 0, body), null, JsPropertyAttributes.Configurable));

    /// <summary>Defines one of the Boolean flag accessors on <paramref name="host"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=22D161
    // Broiler-Human:        PENDING
    private void RegExpFlagGetter(JsObject host, string name, char flag) =>
        RegExpGetter(host, name, (engine, thisValue, arguments) =>
        {
            if (thisValue.AsObjectOrNull() is RegExpObject target)
            {
                return JsValue.Boolean(RegExpHasFlag(target.Flags, flag));
            }

            // The prototype is not a RegExp and answering `undefined` for it is what keeps
            // `RegExp.prototype.flags` - which reads all of these - from throwing.
            return ReferenceEquals(thisValue.AsObjectOrNull(), host)
                ? JsValue.Undefined
                : engine.ThrowTypeError("RegExp.prototype." + name + " requires a RegExp receiver");
        });

    /// <summary>Whether a normalised flag string carries one flag.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=498ACD
    // Broiler-Human:        PENDING
    private static bool RegExpHasFlag(string flags, char flag)
    {
        foreach (var candidate in flags)
        {
            if (candidate == flag)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The receiver of a <c>RegExp.prototype</c> method, or a TypeError.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=61A5BC
    // Broiler-Human:        PENDING
    private static RegExpObject RegExpThis(JsEngine engine, JsValue value, string name) =>
        value.AsObjectOrNull() is RegExpObject target
            ? target
            : throw engine.Error(
                "TypeError", "RegExp.prototype." + name + " called on a value that is not a RegExp");

    /// <summary>The receiver of a <c>String.prototype</c> method, coerced to a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=2B0BFC
    // Broiler-Human:        PENDING
    private static string RegExpStringThis(JsEngine engine, JsValue value)
    {
        if (value.IsString)
        {
            return value.AsString();
        }

        if (value.IsNullish)
        {
            throw engine.Error(
                "TypeError", "String.prototype method called on null or undefined");
        }

        if (value.AsObjectOrNull() is JsPrimitiveWrapper wrapper && wrapper.Primitive.IsString)
        {
            return wrapper.Primitive.AsString();
        }

        return engine.ToStringValue(value);
    }

    /// <summary>The RegExp a String method's first argument stands for.</summary>
    /// <remarks>
    /// A value that is not a RegExp is not an error: the specification builds one out of its String
    /// form, which is why <c>"a.b".match(".")</c> matches a character rather than a full stop.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=9DC393
    // Broiler-Human:        PENDING
    private RegExpObject RegExpFromArgument(JsEngine engine, JsValue value)
    {
        if (value.AsObjectOrNull() is RegExpObject given)
        {
            return given;
        }

        return RegExpBuild(
            engine,
            value.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(value),
            string.Empty);
    }

    /// <summary>Builds a RegExp out of the constructor's two arguments.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=5F03BB
    // Broiler-Human:        PENDING
    private RegExpObject RegExpCreate(JsEngine engine, JsValue pattern, JsValue flags)
    {
        if (pattern.AsObjectOrNull() is RegExpObject template)
        {
            return RegExpBuild(
                engine,
                template.Source,
                flags.Type == JsType.Undefined ? template.Flags : engine.ToStringValue(flags));
        }

        return RegExpBuild(
            engine,
            pattern.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(pattern),
            flags.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(flags));
    }

    /// <summary>Compiles one pattern and wraps it in an object.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=FAC3D1
    // Broiler-Human:        PENDING
    private RegExpObject RegExpBuild(JsEngine engine, string source, string flags)
    {
        var normalized = RegExpNormalizeFlags(engine, flags);
        engine.Charge((ulong)source.Length + 32);
        return new RegExpObject(RegExpPrototype, source, normalized, RegExpCompile(engine, source, normalized));
    }

    /// <summary>Validates a flag string and returns it in the specification's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=680F44
    // Broiler-Human:        PENDING
    private static string RegExpNormalizeFlags(JsEngine engine, string flags)
    {
        var seen = 0;

        foreach (var flag in flags)
        {
            var bit = flag switch
            {
                'g' => 1,
                'i' => 2,
                'm' => 4,
                's' => 8,
                'u' => 16,
                'y' => 32,
                _ => 0,
            };

            if (bit == 0 || (seen & bit) != 0)
            {
                throw engine.Error("SyntaxError", "Invalid regular expression flags: " + flags);
            }

            seen |= bit;
        }

        var builder = new System.Text.StringBuilder(6);

        if ((seen & 1) != 0)
        {
            builder.Append('g');
        }

        if ((seen & 2) != 0)
        {
            builder.Append('i');
        }

        if ((seen & 4) != 0)
        {
            builder.Append('m');
        }

        if ((seen & 8) != 0)
        {
            builder.Append('s');
        }

        if ((seen & 16) != 0)
        {
            builder.Append('u');
        }

        if ((seen & 32) != 0)
        {
            builder.Append('y');
        }

        return builder.ToString();
    }

    /// <summary>Translates a JavaScript pattern and compiles it.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=61BAF2
    // Broiler-Human:        PENDING
    private static System.Text.RegularExpressions.Regex RegExpCompile(
        JsEngine engine, string source, string flags)
    {
        var translated = RegExpTranslate(source);

        var options = System.Text.RegularExpressions.RegexOptions.ECMAScript |
            System.Text.RegularExpressions.RegexOptions.CultureInvariant;

        foreach (var flag in flags)
        {
            switch (flag)
            {
                case 'i':
                    options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                    break;

                case 'm':
                    options |= System.Text.RegularExpressions.RegexOptions.Multiline;
                    break;

                case 's':
                    options |= System.Text.RegularExpressions.RegexOptions.Singleline;
                    break;

                default:
                    break;
            }
        }

        try
        {
            return new System.Text.RegularExpressions.Regex(translated, options, RegExpMatchAllowance);
        }
        catch (System.ArgumentException)
        {
            // .NET REFUSES `ECMAScript` FOR MORE PATTERNS AND OPTION SETS THAN IT ACCEPTS IT FOR,
            // and `Singleline` is one it refuses outright - so every `/./s` lands here. Retrying
            // without the option is the difference between a working pattern and a SyntaxError the
            // language would never have raised; what it changes is written down in the remarks.
        }

        var relaxed = options & ~System.Text.RegularExpressions.RegexOptions.ECMAScript;

        try
        {
            return new System.Text.RegularExpressions.Regex(translated, relaxed, RegExpMatchAllowance);
        }
        catch (System.ArgumentException failure)
        {
            throw engine.Error(
                "SyntaxError",
                "Invalid regular expression: /" + source + "/: " + failure.Message);
        }
    }

    /// <summary>Rewrites the constructs .NET spells differently or refuses.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=81D6C5
    // Broiler-Human:        PENDING
    private static string RegExpTranslate(string pattern)
    {
        var builder = new System.Text.StringBuilder(pattern.Length + 8);
        var at = 0;
        var inClass = false;

        while (at < pattern.Length)
        {
            var character = pattern[at];

            if (character == '\\' && at + 1 < pattern.Length)
            {
                builder.Append(character).Append(pattern[at + 1]);
                at += 2;
                continue;
            }

            if (!inClass && character == '[')
            {
                // `[]` matches nothing and `[^]` matches anything; .NET rejects both as
                // unterminated. An assertion that cannot succeed and an explicit any-character
                // class are the two patterns that mean the same thing to .NET.
                if (at + 1 < pattern.Length && pattern[at + 1] == ']')
                {
                    builder.Append("(?!)");
                    at += 2;
                    continue;
                }

                if (at + 2 < pattern.Length && pattern[at + 1] == '^' && pattern[at + 2] == ']')
                {
                    builder.Append("[\\s\\S]");
                    at += 3;
                    continue;
                }

                inClass = true;
                builder.Append(character);
                at++;
                continue;
            }

            if (inClass && character == '[')
            {
                // Escaped, so .NET's class-subtraction syntax cannot read `-[` out of a class the
                // language reads as two ordinary members.
                builder.Append("\\[");
                at++;
                continue;
            }

            if (inClass && character == ']')
            {
                inClass = false;
            }

            builder.Append(character);
            at++;
        }

        return builder.ToString();
    }

    /// <summary>The escaped text the <c>source</c> accessor and <c>toString</c> report.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=374213
    // Broiler-Human:        PENDING
    private static string RegExpSourceText(string source)
    {
        if (source.Length == 0)
        {
            return "(?:)";
        }

        var builder = new System.Text.StringBuilder(source.Length + 2);
        var escaped = false;

        foreach (var character in source)
        {
            if (escaped)
            {
                builder.Append(character);
                escaped = false;
                continue;
            }

            switch (character)
            {
                case '\\':
                    builder.Append(character);
                    escaped = true;
                    break;

                case '/':
                    builder.Append("\\/");
                    break;

                case '\n':
                    builder.Append("\\n");
                    break;

                case '\r':
                    builder.Append("\\r");
                    break;

                default:
                    // The two Unicode line separators are line terminators to the language even
                    // though they are ordinary characters to .NET, so a source text carrying one
                    // has to escape it or stop being a single line when it is printed. They are
                    // spelled numerically because a C# source file cannot hold either one.
                    if (character == (char)0x2028)
                    {
                        builder.Append("\\u2028");
                    }
                    else if (character == (char)0x2029)
                    {
                        builder.Append("\\u2029");
                    }
                    else
                    {
                        builder.Append(character);
                    }

                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary>The <c>exec</c> protocol, which is the whole of what <c>lastIndex</c> is for.</summary>
    /// <remarks>
    /// Neither <c>g</c> nor <c>y</c>: the search starts at zero and <c>lastIndex</c> is neither read
    /// nor written. Either one: the search starts at <c>lastIndex</c>, a failure resets it to zero
    /// and a success sets it past the match. Getting the reset wrong is what makes a loop over
    /// <c>exec</c> either miss its second string or never end.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=206A84
    // Broiler-Human:        PENDING
    private JsValue RegExpExecute(JsEngine engine, RegExpObject target, string input)
    {
        engine.Charge((ulong)input.Length + 16);

        var tracks = target.Global || target.Sticky;
        var start = tracks ? engine.ToInteger(target.LastIndex) : 0;

        // The specification reads `lastIndex` through `ToLength`, which CLAMPS a negative to zero
        // rather than failing on it: `re.lastIndex = -1` searches from the start, and only a
        // `lastIndex` past the end of the input ends the search before it begins.
        if (start < 0)
        {
            start = 0;
        }

        if (start > input.Length)
        {
            if (tracks)
            {
                target.LastIndex = JsValue.Number(0);
            }

            return JsValue.Null;
        }

        var from = (int)start;
        var match = RegExpRun(engine, target, input, from);

        // .NET has no sticky mode, so an anchored match is a forward search whose result is thrown
        // away unless it began exactly where it was asked to.
        if (!match.Success || (target.Sticky && match.Index != from))
        {
            if (tracks)
            {
                target.LastIndex = JsValue.Number(0);
            }

            return JsValue.Null;
        }

        if (tracks)
        {
            target.LastIndex = JsValue.Number(match.Index + match.Length);
        }

        return JsValue.Object(RegExpResult(engine, match, input));
    }

    /// <summary>Runs the matcher, turning an exhausted allowance into something guest code sees.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=05E06F
    // Broiler-Human:        PENDING
    private static System.Text.RegularExpressions.Match RegExpRun(
        JsEngine engine, RegExpObject target, string input, int start)
    {
        try
        {
            return target.Matcher.Match(input, start);
        }
        catch (System.Text.RegularExpressions.RegexMatchTimeoutException)
        {
            throw engine.Error(
                "RangeError",
                "the regular expression /" + target.Source + "/ exceeded its match allowance");
        }
    }

    /// <summary>The Array an <c>exec</c> answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=82BBF3
    // Broiler-Human:        PENDING
    private JsArray RegExpResult(
        JsEngine engine, System.Text.RegularExpressions.Match match, string input)
    {
        var result = NewArray();
        result.Push(JsValue.String(match.Value));
        var groups = match.Groups;

        for (var at = 1; at < groups.Count; at++)
        {
            engine.Charge(1);
            var group = groups[at];

            // A GROUP THAT DID NOT PARTICIPATE IS `undefined` AND NOT THE EMPTY STRING. The two are
            // told apart by every destructuring of an exec result that has an optional group in it.
            result.Push(group.Success ? JsValue.String(group.Value) : JsValue.Undefined);
        }

        result.DefineOrdinary("index", JsValue.Number(match.Index));
        result.DefineOrdinary("input", JsValue.String(input));
        return result;
    }

    /// <summary><c>replace</c> with a RegExp on the left.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=C3ABF6
    // Broiler-Human:        PENDING
    private static string RegExpReplaceAll(
        JsEngine engine, RegExpObject pattern, string input, JsValue replacement)
    {
        var callable = replacement.AsObjectOrNull() is { IsCallable: true };
        var template = callable ? string.Empty : engine.ToStringValue(replacement);
        var builder = new System.Text.StringBuilder(input.Length);
        var at = 0;
        var copied = 0;

        if (pattern.Global)
        {
            pattern.LastIndex = JsValue.Number(0);
        }

        while (at <= input.Length)
        {
            engine.Charge((ulong)(input.Length - at) + 4);
            var match = RegExpRun(engine, pattern, input, at);

            if (!match.Success)
            {
                break;
            }

            builder.Append(input, copied, match.Index - copied);

            builder.Append(callable
                ? RegExpCallReplacement(engine, replacement, match, input)
                : RegExpExpand(template, match, input));

            copied = match.Index + match.Length;

            if (!pattern.Global)
            {
                break;
            }

            // An empty match must advance, or the replacement of "" by "-" never terminates.
            at = match.Length == 0 ? match.Index + 1 : copied;
        }

        builder.Append(input, copied, input.Length - copied);
        return builder.ToString();
    }

    /// <summary><c>replace</c> with a string on the left, which replaces the first occurrence only.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=E7072D
    // Broiler-Human:        PENDING
    private static string RegExpReplaceText(
        JsEngine engine, string input, string search, JsValue replacement)
    {
        var at = input.IndexOf(search, System.StringComparison.Ordinal);

        if (at < 0)
        {
            return input;
        }

        string produced;

        if (replacement.AsObjectOrNull() is { IsCallable: true })
        {
            produced = engine.ToStringValue(engine.Call(
                replacement,
                JsValue.Undefined,
                [JsValue.String(search), JsValue.Number(at), JsValue.String(input)]));
        }
        else
        {
            var template = engine.ToStringValue(replacement);
            var builder = new System.Text.StringBuilder(template.Length);
            var step = 0;

            while (step < template.Length)
            {
                var character = template[step];

                if (character != '$' || step + 1 >= template.Length)
                {
                    builder.Append(character);
                    step++;
                    continue;
                }

                // A string pattern has no captures, so `$1`..`$9` stay literal and only the four
                // capture-free substitutions mean anything.
                switch (template[step + 1])
                {
                    case '$':
                        builder.Append('$');
                        step += 2;
                        continue;

                    case '&':
                        builder.Append(search);
                        step += 2;
                        continue;

                    case '`':
                        builder.Append(input, 0, at);
                        step += 2;
                        continue;

                    case '\'':
                        builder.Append(input, at + search.Length, input.Length - at - search.Length);
                        step += 2;
                        continue;

                    default:
                        builder.Append(character);
                        step++;
                        continue;
                }
            }

            produced = builder.ToString();
        }

        return input.Substring(0, at) + produced + input.Substring(at + search.Length);
    }

    /// <summary>Calls a function replacement with <c>(match, p1..pn, offset, string)</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=560903
    // Broiler-Human:        PENDING
    private static string RegExpCallReplacement(
        JsEngine engine,
        JsValue replacement,
        System.Text.RegularExpressions.Match match,
        string input)
    {
        var groups = match.Groups;
        var passed = new JsValue[groups.Count + 2];
        passed[0] = JsValue.String(match.Value);

        for (var at = 1; at < groups.Count; at++)
        {
            engine.Charge(1);
            var group = groups[at];
            passed[at] = group.Success ? JsValue.String(group.Value) : JsValue.Undefined;
        }

        passed[groups.Count] = JsValue.Number(match.Index);
        passed[groups.Count + 1] = JsValue.String(input);
        return engine.ToStringValue(engine.Call(replacement, JsValue.Undefined, passed));
    }

    /// <summary>Expands the dollar substitutions of a string replacement.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=0255E9
    // Broiler-Human:        PENDING
    private static string RegExpExpand(
        string template, System.Text.RegularExpressions.Match match, string input)
    {
        var builder = new System.Text.StringBuilder(template.Length);
        var at = 0;

        while (at < template.Length)
        {
            var character = template[at];

            if (character != '$' || at + 1 >= template.Length)
            {
                builder.Append(character);
                at++;
                continue;
            }

            var next = template[at + 1];

            switch (next)
            {
                case '$':
                    builder.Append('$');
                    at += 2;
                    continue;

                case '&':
                    builder.Append(match.Value);
                    at += 2;
                    continue;

                case '`':
                    builder.Append(input, 0, match.Index);
                    at += 2;
                    continue;

                case '\'':
                    var tail = match.Index + match.Length;
                    builder.Append(input, tail, input.Length - tail);
                    at += 2;
                    continue;

                default:
                    break;
            }

            if (next is >= '0' and <= '9')
            {
                var count = match.Groups.Count;
                var number = next - '0';
                var width = 2;

                // `$12` names the twelfth group when there is one and the first group followed by a
                // "2" when there is not, which is the rule the specification states and every
                // implementation reproduces.
                if (at + 2 < template.Length && template[at + 2] is >= '0' and <= '9')
                {
                    var wider = (number * 10) + (template[at + 2] - '0');

                    if (wider >= 1 && wider < count)
                    {
                        number = wider;
                        width = 3;
                    }
                }

                if (number >= 1 && number < count)
                {
                    var group = match.Groups[number];

                    if (group.Success)
                    {
                        builder.Append(group.Value);
                    }

                    at += width;
                    continue;
                }
            }

            builder.Append(character);
            at++;
        }

        return builder.ToString();
    }

    /// <summary><c>split</c> with a RegExp separator: every match is a boundary, captures included.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=BE6FE7
    // Broiler-Human:        PENDING
    private JsArray RegExpSplitPattern(
        JsEngine engine, RegExpObject pattern, string input, long limit)
    {
        var pieces = new System.Collections.Generic.List<JsValue>();

        if (input.Length == 0)
        {
            // AN EMPTY STRING SPLITS INTO NOTHING WHEN THE SEPARATOR MATCHES IT and into one empty
            // piece when it does not. `"".split(/x/)` is `[""]` and `"".split(/(?:)/)` is `[]`.
            var probe = RegExpRun(engine, pattern, input, 0);

            if (!probe.Success)
            {
                pieces.Add(JsValue.String(input));
            }

            return NewArray(pieces);
        }

        var start = 0;
        var scan = 0;

        while (scan < input.Length)
        {
            engine.Charge((ulong)(input.Length - scan) + 4);
            var match = RegExpRun(engine, pattern, input, scan);

            if (!match.Success || match.Index >= input.Length)
            {
                break;
            }

            var end = System.Math.Min(match.Index + match.Length, input.Length);

            if (end == start)
            {
                // An empty match sitting on the start of the pending piece would produce an empty
                // piece for every position; the specification steps past it instead.
                scan = match.Index + 1;
                continue;
            }

            pieces.Add(JsValue.String(input.Substring(start, match.Index - start)));

            if (pieces.Count >= limit)
            {
                return NewArray(pieces);
            }

            var groups = match.Groups;

            for (var at = 1; at < groups.Count; at++)
            {
                engine.Charge(1);
                var group = groups[at];
                pieces.Add(group.Success ? JsValue.String(group.Value) : JsValue.Undefined);

                if (pieces.Count >= limit)
                {
                    return NewArray(pieces);
                }
            }

            start = end;
            scan = start;
        }

        pieces.Add(JsValue.String(input.Substring(start)));
        return NewArray(pieces);
    }

    /// <summary><c>split</c> with a string separator.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=9303A9
    // Broiler-Human:        PENDING
    private JsArray RegExpSplitText(JsEngine engine, string input, string separator, long limit)
    {
        var pieces = new System.Collections.Generic.List<JsValue>();

        if (separator.Length == 0)
        {
            // The empty separator splits into code units and produces no trailing piece, which is
            // why `"".split("")` is `[]` and `"ab".split("")` is `["a", "b"]`.
            for (var at = 0; at < input.Length && pieces.Count < limit; at++)
            {
                engine.Charge(1);
                pieces.Add(JsValue.String(input[at].ToString()));
            }

            return NewArray(pieces);
        }

        var start = 0;

        while (true)
        {
            engine.Charge((ulong)(input.Length - start) + 4);
            var found = input.IndexOf(separator, start, System.StringComparison.Ordinal);

            if (found < 0)
            {
                break;
            }

            pieces.Add(JsValue.String(input.Substring(start, found - start)));

            if (pieces.Count >= limit)
            {
                return NewArray(pieces);
            }

            start = found + separator.Length;
        }

        pieces.Add(JsValue.String(input.Substring(start)));
        return NewArray(pieces);
    }

    /// <summary>A compiled regular expression: the source, the flags, the matcher and the cursor.</summary>
    /// <remarks>
    /// <c>lastIndex</c> is a writable, non-enumerable, non-configurable OWN property of every
    /// instance and not a prototype accessor, because the specification says so and because guest
    /// code assigns to it directly. It is stored as a field and projected as a property rather than
    /// held in the ordinary map, so that reading it after a match costs nothing and so that the
    /// value the guest wrote - which need not be a Number - survives until <c>exec</c> coerces it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=CB29B4
    // Broiler-Human:        PENDING
    private sealed class RegExpObject : JsObject
    {
        /// <summary>Creates a compiled regular expression.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=EAC46F
        // Broiler-Human:        PENDING
        internal RegExpObject(
            JsObject? prototype,
            string source,
            string flags,
            System.Text.RegularExpressions.Regex matcher)
            : base(prototype, "RegExp")
        {
            Source = source;
            Flags = flags;
            Matcher = matcher;

            foreach (var flag in flags)
            {
                switch (flag)
                {
                    case 'g':
                        Global = true;
                        break;

                    case 'y':
                        Sticky = true;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>The pattern text, exactly as it was given.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=C6098C
        // Broiler-Human:        PENDING
        internal string Source { get; }

        /// <summary>The flags, in the specification's order and with no duplicates.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=5E11BA
        // Broiler-Human:        PENDING
        internal string Flags { get; }

        /// <summary>The compiled matcher.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=23C977
        // Broiler-Human:        PENDING
        internal System.Text.RegularExpressions.Regex Matcher { get; }

        /// <summary>Whether the <c>g</c> flag is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=5988AA
        // Broiler-Human:        PENDING
        internal bool Global { get; }

        /// <summary>Whether the <c>y</c> flag is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=F09C77
        // Broiler-Human:        PENDING
        internal bool Sticky { get; }

        /// <summary>Where the next <c>g</c> or <c>y</c> search starts, as the guest last left it.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=D28876
        // Broiler-Human:        PENDING
        internal JsValue LastIndex { get; set; } = JsValue.Number(0);

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=3C7D1D
        // Broiler-Human:        PENDING
        internal override int OwnPropertyCount => base.OwnPropertyCount + 1;

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=966612
        // Broiler-Human:        PENDING
        internal override bool TryGetOwnProperty(string key, out JsProperty property)
        {
            if (string.Equals(key, "lastIndex", System.StringComparison.Ordinal))
            {
                property = JsProperty.Data(LastIndex, JsPropertyAttributes.Writable);
                return true;
            }

            return base.TryGetOwnProperty(key, out property);
        }

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=83A231
        // Broiler-Human:        PENDING
        internal override void SetOwnProperty(string key, JsProperty property)
        {
            if (string.Equals(key, "lastIndex", System.StringComparison.Ordinal) &&
                !property.IsAccessor)
            {
                LastIndex = property.Value;
                return;
            }

            base.SetOwnProperty(key, property);
        }

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=6D0161
        // Broiler-Human:        PENDING
        internal override bool DeleteOwnProperty(string key) =>
            !string.Equals(key, "lastIndex", System.StringComparison.Ordinal) &&
            base.DeleteOwnProperty(key);

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=6AC96B
        // Broiler-Human:        PENDING
        internal override System.Collections.Generic.List<string> OwnPropertyNames()
        {
            var names = base.OwnPropertyNames();
            names.Add("lastIndex");
            return names;
        }
    }
}
