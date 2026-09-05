// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           8
// Human-reviewed:   0/39
// IP risk:          Medium
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

using Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The <c>RegExp</c> intrinsic, its prototype, and the four <c>String.prototype</c> methods that
/// take one.
/// </summary>
/// <remarks>
/// <para>
/// <b>The pattern is compiled by a matcher this profile owns.</b> <see cref="JsRegExpMatcher"/>
/// parses the ECMAScript pattern grammar - Annex B included, because real code uses it - lowers it
/// to an instruction array and runs it on a backtracking machine with an explicit stack. Nothing
/// here translates a pattern into another engine's dialect, and no host regular-expression type is
/// named anywhere in this file or in that one. What that buys is the list of things the translation
/// got wrong and this does not: capture numbering is by opening parenthesis whether a group is
/// named or not; a quantified group is reset to <c>undefined</c> on each repetition, so
/// <c>/(?:(a)|b)+/.exec("ab")</c> reports <c>undefined</c> for its group; <c>$</c> without <c>m</c>
/// matches at the very end and not before a trailing newline; <c>y</c> is a real anchored attempt
/// at <c>lastIndex</c> that costs one attempt rather than a forward search whose answer is thrown
/// away, and <c>match</c>, <c>search</c>, <c>replace</c> and <c>split</c> all honour it; <c>u</c>
/// matches code point by code point, takes <c>\u{...}</c>, folds with the full simple case folding
/// and steps over a surrogate pair as one position; <c>s</c> is a flag rather than a reason to
/// re-parse; and an <c>exec</c> result carries <c>groups</c> when the pattern named any.
/// </para>
/// <para>
/// <b>Backtracking is metered rather than timed.</b> Every instruction the machine dispatches is
/// charged to the engine's fuel meter, which is where a spent allowance becomes an abort the guest
/// cannot catch and where cancellation is polled. A catastrophically backtracking pattern therefore
/// spends the guest's allowance and ends as a resource exhaustion with a named dimension - not as a
/// hang, and no longer as the wall-clock <c>RangeError</c> the translation carried, which was a
/// value the language does not have. The matcher's own two ceilings on the backtrack stack and the
/// undo trail are reported the same way, for a host that granted an unbounded allowance.
/// </para>
/// <para>
/// <b>What is still not here.</b> The list is shorter than it was and it is the whole of what is
/// known to differ; anything found later belongs on it.
/// </para>
/// <list type="bullet">
/// <item>
/// <b>No <c>\p{...}</c> or <c>\P{...}</c> property escapes.</b> Under <c>u</c> a pattern carrying
/// one is a <c>SyntaxError</c> naming the escape, which is a refusal the language does not make;
/// outside <c>u</c> it is the identity escape Annex B already makes it. Implementing them needs the
/// Unicode property tables and those are not shipped here.
/// </item>
/// <item>
/// <b>No <c>v</c> flag</b>, and none of the set operations, string properties or nested classes it
/// brings. A <c>v</c> in a flag string is a <c>SyntaxError</c>.
/// </item>
/// <item>
/// <b><c>d</c> is parsed, ordered and reported, and builds no <c>indices</c>.</b> The flag is
/// accepted, appears in <c>flags</c> in the specification's position, and <c>hasIndices</c> answers
/// for it - but an <c>exec</c> result has no <c>indices</c> property, so code that reads one gets
/// <c>undefined</c> rather than the array of offset pairs. The offsets exist inside the matcher;
/// what is missing is the object, and it is missing because nothing this profile runs asks for it.
/// </item>
/// <item>
/// <b>Case folding is computed, not tabulated.</b> Canonicalize is built from the invariant
/// culture's simple case mappings rather than from a shipped <c>CaseFolding</c> table, with three
/// characters written in by hand: the dotted capital I and the dotless small i, which fold to
/// themselves, and the long s, which the host's globalization-invariant mode leaves alone where
/// Unicode folds it to <c>s</c>. The whole plane was compared with those mappings present and
/// absent and nothing else moved, but a character Unicode folds differently from the invariant
/// table would be a case-insensitive match this makes and the comparison engine does not.
/// </item>
/// <item>
/// <b>A pattern may not nest more than 128 groups deep.</b> Past that the parser raises a
/// <c>SyntaxError</c> the language would not have raised. It is the declared price of a recursive
/// descent parser in a profile that has already lost a process to a stack it could not translate,
/// and it is far past anything a person writes.
/// </item>
/// <item>
/// <b>A group name spelled with a <c>\u</c> escape is refused</b>, and so is a duplicate group
/// name in alternatives that cannot both match - which the specification now admits. The second
/// refusal is what the comparison engine at the version measured against does too, so it costs
/// nothing today and will cost something the day that engine moves.
/// </item>
/// <item>
/// <b>The pattern protocol dispatches and the RegExp's own <c>exec</c> is still not consulted.</b>
/// The five String methods ask their argument for <c>Symbol.match</c>, <c>Symbol.matchAll</c>,
/// <c>Symbol.replace</c>, <c>Symbol.search</c> or <c>Symbol.split</c> and call it when it answers,
/// so a program's own object is a pattern — <i>(this read "no Symbols at all" until 2026-09-05,
/// which was true when this surface had none and had stopped being true; corrected as
/// JSC-129)</i>. What the five methods on <c>RegExp.prototype</c> do NOT do is go through the
/// receiver's own <c>exec</c>: they run the matcher directly, so a subclass overriding <c>exec</c>
/// changes what <c>re.exec(s)</c> answers and not what <c>s.match(re)</c> answers.
/// <c>replaceAll</c> takes a string pattern only.
/// </item>
/// <item>
/// <b>A required repetition is counted rather than reasoned about.</b> <c>/(?:){1000000000}/</c>
/// runs a billion empty iterations and spends the fuel for them, where an engine that noticed the
/// body cannot consume would answer at once. The answer is the same; the cost is not.
/// </item>
/// </list>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The meter the matcher charges its instructions to, built once per realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=4BE525
    // Broiler-Human:        PENDING
    private JsRegExpCharge? regExpCharge;

    /// <summary>Builds <c>RegExp</c>, <c>RegExp.prototype</c>, and the String methods that take one.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=84B942
    // Broiler-Human:        PENDING
    private void SetupRegExp()
    {
        var prototype = RegExpPrototype;

        var regExpConstructor = Constructor(
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

        RegExpFlagGetter(prototype, "hasIndices", 'd');
        RegExpFlagGetter(prototype, "global", 'g');
        SpeciesGetter(regExpConstructor);

        RegExpFlagGetter(prototype, "ignoreCase", 'i');
        RegExpFlagGetter(prototype, "multiline", 'm');
        RegExpFlagGetter(prototype, "dotAll", 's');
        RegExpFlagGetter(prototype, "unicode", 'u');
        RegExpFlagGetter(prototype, "sticky", 'y');

        // ---- the pattern protocol, which is what makes these five methods dispatchable --------
        //
        // A PATTERN IS AN OBJECT WITH THE RIGHT SYMBOL AND NOT A RegExp. `"x".replace(p, r)` asks
        // `p` for `Symbol.replace` and calls it; a program's own object answering that Symbol is a
        // pattern, and a RegExp is one because its prototype answers all five. That is the whole
        // extension point the language gives here, and this realm answered it by TESTING FOR A
        // RegExp OBJECT until 2026-09-05 - a header remark defended the difference on the grounds
        // that this surface had no Symbols, which stopped being true when it acquired them.
        RegExpSymbolMethod(prototype, MatchSymbol, "[Symbol.match]", 1,
            (engine, pattern, arguments) =>
                RegExpMatchThrough(engine, pattern, engine.ToStringValue(ArgOfRegExp(arguments, 0))));

        RegExpSymbolMethod(prototype, MatchAllSymbol, "[Symbol.matchAll]", 1,
            (engine, pattern, arguments) =>
                RegExpMatchAllThrough(
                    engine, pattern, engine.ToStringValue(ArgOfRegExp(arguments, 0))));

        RegExpSymbolMethod(prototype, SearchSymbol, "[Symbol.search]", 1,
            (engine, pattern, arguments) =>
                RegExpSearchThrough(engine, pattern, engine.ToStringValue(ArgOfRegExp(arguments, 0))));

        RegExpSymbolMethod(prototype, ReplaceSymbol, "[Symbol.replace]", 2,
            (engine, pattern, arguments) =>
                JsValue.String(
                    RegExpReplaceAll(
                        engine,
                        pattern,
                        engine.ToStringValue(ArgOfRegExp(arguments, 0)),
                        ArgOfRegExp(arguments, 1))));

        RegExpSymbolMethod(prototype, SplitSymbol, "[Symbol.split]", 2,
            (engine, pattern, arguments) =>
            {
                var input = engine.ToStringValue(ArgOfRegExp(arguments, 0));
                var bound = ArgOfRegExp(arguments, 1);
                var limit = bound.Type == JsType.Undefined ? 4294967295L : engine.ToUint32(bound);

                return limit == 0
                    ? JsValue.Object(NewArray())
                    : JsValue.Object(RegExpSplitPattern(engine, pattern, input, limit));
            });

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
            var given = ArgOfRegExp(arguments, 0);

            if (RegExpDispatch(engine, thisValue, given, engine.Realm.MatchSymbol, out var answered))
            {
                return answered;
            }

            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, given);
            engine.Charge((ulong)input.Length + 16);

            if (!pattern.Global)
            {
                return RegExpExecute(engine, pattern, input);
            }

            // A GLOBAL `match` COLLECTS THE MATCHED TEXT AND NOTHING ELSE - no index, no input, no
            // captures - and leaves `lastIndex` at zero however it ends. It goes through `exec`
            // rather than around it, so a global sticky pattern stops at its first gap.
            pattern.LastIndex = JsValue.Number(0);
            var found = NewArray();

            while (true)
            {
                var match = RegExpMatchOne(engine, pattern, input);

                if (match is null)
                {
                    break;
                }

                found.Push(JsValue.String(match.TextOf(input, 0)));

                if (match.Length == 0)
                {
                    pattern.LastIndex = JsValue.Number(
                        JsRegExpMatcher.Advance(input, match.End, pattern.Unicode));
                }
            }

            pattern.LastIndex = JsValue.Number(0);
            return found.Length == 0 ? JsValue.Null : JsValue.Object(found);
        });

        // EVERY MATCH WITH ITS CAPTURES, WHICH IS THE ONE THING A GLOBAL `match` WILL NOT GIVE.
        // `"a1".match(/(\w)(\d)/g)` answers the matched TEXT of each match and throws the groups
        // away, so a program that wants both has to loop `exec` and remember `lastIndex` itself.
        // This is that loop, as an iterator, and it is the reason the method exists.
        Method(StringPrototype, "matchAll", 1, (engine, thisValue, arguments) =>
        {
            var given = ArgOfRegExp(arguments, 0);

            // THE GLOBALITY CHECK HAPPENS BEFORE THE DISPATCH AND NOT INSIDE IT, which is the one
            // place this method's order differs from its four neighbours: the language checks that
            // a RegExp argument is global first, so `"x".matchAll(/a/)` is a TypeError even though
            // `RegExp.prototype[Symbol.matchAll]` would have accepted it.
            if (given.AsObjectOrNull() is RegExpObject checkedFirst && !checkedFirst.Global)
            {
                return engine.ThrowTypeError(
                    "String.prototype.matchAll called with a non-global RegExp argument");
            }

            if (RegExpDispatch(engine, thisValue, given, engine.Realm.MatchAllSymbol, out var answered))
            {
                return answered;
            }

            var input = RegExpStringThis(engine, thisValue);

            // A NON-GLOBAL REGULAR EXPRESSION IS A TYPE ERROR AND NOT AN ITERATOR OF ONE. The
            // language refuses it because the loop this performs would not terminate without the
            // `lastIndex` a global pattern keeps, and answering with a single match would have been
            // a different method wearing this one's name.
            if (given.AsObjectOrNull() is RegExpObject supplied && !supplied.Global)
            {
                return engine.ThrowTypeError(
                    "String.prototype.matchAll called with a non-global RegExp argument");
            }

            // THE ITERATION RUNS OVER A COPY, so the pattern the caller handed in keeps its own
            // `lastIndex`: a program that interleaves `matchAll` with `exec` on one RegExp sees
            // neither disturb the other, which is what the language says and what a shared object
            // would not give.
            var pattern = RegExpBuild(
                engine,
                given.AsObjectOrNull() is RegExpObject source ? source.Source : engine.ToStringValue(given),
                given.AsObjectOrNull() is RegExpObject held ? held.Flags : "g");

            pattern.LastIndex = given.AsObjectOrNull() is RegExpObject from
                ? JsValue.Number(engine.ToInteger(from.LastIndex))
                : JsValue.Number(0);

            engine.Charge((ulong)input.Length + 16);

            return JsValue.Object(CreateListIterator("RegExp String Iterator", slot =>
            {
                var match = RegExpMatchOne(engine, pattern, input);

                if (match is null)
                {
                    return (false, JsValue.Undefined, slot);
                }

                // AN EMPTY MATCH ADVANCES THE CURSOR BY HAND, because the matcher leaves
                // `lastIndex` where the match ended and an empty match ends where it began. Without
                // this the iterator answers the same empty match for ever.
                if (match.Length == 0)
                {
                    pattern.LastIndex = JsValue.Number(
                        JsRegExpMatcher.Advance(input, match.End, pattern.Unicode));
                }

                return (true, JsValue.Object(RegExpResult(engine, pattern, match, input)), slot + 1);
            }));
        });

        Method(StringPrototype, "search", 1, (engine, thisValue, arguments) =>
        {
            var given = ArgOfRegExp(arguments, 0);

            if (RegExpDispatch(engine, thisValue, given, engine.Realm.SearchSymbol, out var answered))
            {
                return answered;
            }

            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, given);
            engine.Charge((ulong)input.Length + 16);

            // `search` neither consults nor disturbs `lastIndex`: it saves it, searches from zero,
            // and puts it back. A sticky pattern is still anchored, which is why "aab".search(/b/y)
            // is -1.
            var saved = pattern.LastIndex;
            pattern.LastIndex = JsValue.Number(0);
            var match = RegExpRun(engine, pattern, input, 0, pattern.Sticky);
            pattern.LastIndex = saved;
            return JsValue.Number(match is null ? -1 : match.Index);
        });

        Method(StringPrototype, "replace", 2, (engine, thisValue, arguments) =>
        {
            var search = ArgOfRegExp(arguments, 0);
            var replacement = ArgOfRegExp(arguments, 1);

            if (RegExpDispatch(
                    engine, thisValue, search, engine.Realm.ReplaceSymbol, out var answered,
                    replacement))
            {
                return answered;
            }

            var input = RegExpStringThis(engine, thisValue);
            engine.Charge((ulong)input.Length + 16);

            return JsValue.String(
                search.AsObjectOrNull() is RegExpObject pattern
                    ? RegExpReplaceAll(engine, pattern, input, replacement)
                    : RegExpReplaceText(engine, input, engine.ToStringValue(search), replacement));
        });

        Method(StringPrototype, "split", 2, (engine, thisValue, arguments) =>
        {
            var separator = ArgOfRegExp(arguments, 0);
            var bound = ArgOfRegExp(arguments, 1);

            if (RegExpDispatch(
                    engine, thisValue, separator, engine.Realm.SplitSymbol, out var answered, bound))
            {
                return answered;
            }

            var input = RegExpStringThis(engine, thisValue);
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

    /// <summary>Installs one of the five pattern methods under its Symbol.</summary>
    /// <remarks>
    /// <b>Each one requires a real RegExp receiver</b>, which is not the same as requiring a RegExp
    /// argument at the call site: a program's own object may answer the Symbol however it likes, and
    /// what this refuses is a call of <c>RegExp.prototype[Symbol.match]</c> on something that is not
    /// one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=BF84D3
    // Broiler-Human:        PENDING
    private void RegExpSymbolMethod(
        JsObject host,
        JsSymbol key,
        string name,
        int arity,
        System.Func<JsEngine, RegExpObject, JsValue[], JsValue> body) =>
        host.SetOwnSymbol(
            key,
            JsProperty.Data(
                JsValue.Object(Native(name, arity, (engine, thisValue, arguments) =>
                {
                    if (thisValue.AsObjectOrNull() is not RegExpObject pattern)
                    {
                        return engine.ThrowTypeError(
                            "RegExp.prototype" + name + " called on a value that is not a RegExp");
                    }

                    return body(engine, pattern, arguments);
                })),
                JsPropertyAttributes.BuiltIn));

    /// <summary>Hands the work to the pattern when the pattern says it can do it.</summary>
    /// <remarks>
    /// <b>The read is a <c>GetMethod</c> and the ORDER matters.</b> A nullish pattern is not asked
    /// at all — <c>"x".replace(null, r)</c> replaces the text <c>"null"</c> — and a pattern whose
    /// Symbol is present but not callable is a <c>TypeError</c> rather than a fall-through, because
    /// an object that claims the protocol and cannot perform it is a mistake worth reporting. The
    /// receiver is coerced to a String only on the path that does NOT dispatch, since the method
    /// being dispatched to is handed the receiver as it stands.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=627A72
    // Broiler-Human:        PENDING
    private static bool RegExpDispatch(
        JsEngine engine,
        JsValue receiver,
        JsValue pattern,
        JsSymbol key,
        out JsValue answered,
        JsValue extra = default)
    {
        answered = JsValue.Undefined;

        if (pattern.IsNullish)
        {
            return false;
        }

        if (receiver.IsNullish)
        {
            engine.ThrowTypeError("String.prototype method called on null or undefined");
        }

        var method = engine.GetSymbol(pattern, key);

        if (method.Type == JsType.Undefined || method.Type == JsType.Null)
        {
            return false;
        }

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            engine.ThrowTypeError("the pattern's protocol method is not callable");
        }

        answered = extra.IsEmpty
            ? engine.Call(method, pattern, [receiver])
            : engine.Call(method, pattern, [receiver, extra]);

        return true;
    }

    /// <summary>What <c>String.prototype.match</c> does once the pattern is known to be one.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=E15631
    // Broiler-Human:        PENDING
    private JsValue RegExpMatchThrough(JsEngine engine, RegExpObject pattern, string input)
    {
        engine.Charge((ulong)input.Length + 16);

        if (!pattern.Global)
        {
            return RegExpExecute(engine, pattern, input);
        }

        pattern.LastIndex = JsValue.Number(0);
        var found = NewArray();

        while (true)
        {
            var match = RegExpMatchOne(engine, pattern, input);

            if (match is null)
            {
                break;
            }

            found.Push(JsValue.String(match.TextOf(input, 0)));

            if (match.Length == 0)
            {
                pattern.LastIndex = JsValue.Number(
                    JsRegExpMatcher.Advance(input, match.End, pattern.Unicode));
            }
        }

        pattern.LastIndex = JsValue.Number(0);
        return found.Length == 0 ? JsValue.Null : JsValue.Object(found);
    }

    /// <summary>The same for <c>search</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=7F4778
    // Broiler-Human:        PENDING
    private JsValue RegExpSearchThrough(JsEngine engine, RegExpObject pattern, string input)
    {
        engine.Charge((ulong)input.Length + 16);
        var saved = pattern.LastIndex;
        pattern.LastIndex = JsValue.Number(0);
        var match = RegExpRun(engine, pattern, input, 0, pattern.Sticky);
        pattern.LastIndex = saved;
        return JsValue.Number(match is null ? -1 : match.Index);
    }

    /// <summary>The same for <c>matchAll</c>, over a copy of the pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=69F556
    // Broiler-Human:        PENDING
    private JsValue RegExpMatchAllThrough(JsEngine engine, RegExpObject pattern, string input)
    {
        var walked = RegExpBuild(engine, pattern.Source, pattern.Flags);
        walked.LastIndex = JsValue.Number(engine.ToInteger(pattern.LastIndex));
        engine.Charge((ulong)input.Length + 16);

        return JsValue.Object(CreateListIterator("RegExp String Iterator", slot =>
        {
            var match = RegExpMatchOne(engine, walked, input);

            if (match is null)
            {
                return (false, JsValue.Undefined, slot);
            }

            if (match.Length == 0)
            {
                walked.LastIndex = JsValue.Number(
                    JsRegExpMatcher.Advance(input, match.End, walked.Unicode));
            }

            return (true, JsValue.Object(RegExpResult(engine, walked, match, input)), slot + 1);
        }));
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

        return new RegExpObject(
            RegExpPrototype, source, normalized, RegExpCompile(engine, source, normalized));
    }

    /// <summary>Validates a flag string and returns it in the specification's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=1BF553
    // Broiler-Human:        PENDING
    private static string RegExpNormalizeFlags(JsEngine engine, string flags)
    {
        var seen = 0;

        foreach (var flag in flags)
        {
            var bit = flag switch
            {
                'd' => 1,
                'g' => 2,
                'i' => 4,
                'm' => 8,
                's' => 16,
                'u' => 32,
                'y' => 64,
                _ => 0,
            };

            if (bit == 0 || (seen & bit) != 0)
            {
                throw engine.Error("SyntaxError", "Invalid regular expression flags: " + flags);
            }

            seen |= bit;
        }

        // THE ORDER IS THE SPECIFICATION'S AND NOT THE ORDER THEY WERE WRITTEN IN, which is what
        // makes `new RegExp("x", "yg").flags` answer "gy" and what `toString` prints.
        var builder = new System.Text.StringBuilder(7);
        var order = "dgimsuy";

        for (var at = 0; at < order.Length; at++)
        {
            if ((seen & (1 << at)) != 0)
            {
                builder.Append(order[at]);
            }
        }

        return builder.ToString();
    }

    /// <summary>Compiles a pattern with this profile's own matcher.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=C66184
    // Broiler-Human:        PENDING
    private static JsRegExpMatcher RegExpCompile(JsEngine engine, string source, string flags)
    {
        try
        {
            return JsRegExpMatcher.Compile(
                source,
                RegExpHasFlag(flags, 'i'),
                RegExpHasFlag(flags, 'm'),
                RegExpHasFlag(flags, 's'),
                RegExpHasFlag(flags, 'u'));
        }
        catch (JsRegExpSyntaxError failure)
        {
            throw engine.Error(
                "SyntaxError",
                "Invalid regular expression: /" + source + "/: " + failure.Message);
        }
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
                    // though they are ordinary characters otherwise, so a source text carrying one
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
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=AF8128
    // Broiler-Human:        PENDING
    private JsValue RegExpExecute(JsEngine engine, RegExpObject target, string input)
    {
        var match = RegExpMatchOne(engine, target, input);

        return match is null
            ? JsValue.Null
            : JsValue.Object(RegExpResult(engine, target, match, input));
    }

    /// <summary>One <c>exec</c>, answering the match itself rather than an Array.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=DAC9EB
    // Broiler-Human:        PENDING
    private static JsRegExpMatch? RegExpMatchOne(
        JsEngine engine, RegExpObject target, string input)
    {
        engine.Charge(16);

        var tracks = target.Global || target.Sticky;
        var start = tracks ? engine.ToInteger(target.LastIndex) : 0;

        // The specification reads `lastIndex` through `ToLength`, which CLAMPS a negative to zero
        // rather than failing on it: `re.lastIndex = -1` searches from the start, and only a
        // `lastIndex` past the end of the input ends the search before it begins.
        if (start < 0)
        {
            start = 0;
        }

        var match = start > input.Length
            ? null
            : RegExpRun(engine, target, input, (int)start, target.Sticky);

        if (match is null)
        {
            if (tracks)
            {
                target.LastIndex = JsValue.Number(0);
            }

            return null;
        }

        if (tracks)
        {
            target.LastIndex = JsValue.Number(match.End);
        }

        return match;
    }

    /// <summary>Runs the matcher, turning its two ceilings into a stop the guest cannot catch.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=9C3846
    // Broiler-Human:        PENDING
    private static JsRegExpMatch? RegExpRun(
        JsEngine engine, RegExpObject target, string input, int start, bool anchored)
    {
        try
        {
            return target.Matcher.Match(input, start, anchored, engine.Realm.RegExpMeter(engine));
        }
        catch (JsRegExpOverflowError failure)
        {
            throw new JsAbort(JsAbortKind.Exhausted, failure.Message);
        }
    }

    /// <summary>The delegate the matcher charges through, built once and kept.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=624853
    // Broiler-Human:        PENDING
    private JsRegExpCharge RegExpMeter(JsEngine owner) => regExpCharge ??= owner.Charge;

    /// <summary>The Array an <c>exec</c> answers with.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=C8955D
    // Broiler-Human:        PENDING
    private JsArray RegExpResult(
        JsEngine engine, RegExpObject target, JsRegExpMatch match, string input)
    {
        var result = NewArray();
        result.Push(JsValue.String(match.TextOf(input, 0)));

        for (var at = 1; at <= match.CaptureCount; at++)
        {
            engine.Charge(1);

            // A GROUP THAT DID NOT PARTICIPATE IS `undefined` AND NOT THE EMPTY STRING. The two are
            // told apart by every destructuring of an exec result that has an optional group in it.
            result.Push(match.Participated(at)
                ? JsValue.String(match.TextOf(input, at))
                : JsValue.Undefined);
        }

        result.DefineOrdinary("index", JsValue.Number(match.Index));
        result.DefineOrdinary("input", JsValue.String(input));
        result.DefineOrdinary("groups", RegExpNamedGroups(target, match, input));
        return result;
    }

    /// <summary><c>replace</c> with a RegExp on the left.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=820AB9
    // Broiler-Human:        PENDING
    private static string RegExpReplaceAll(
        JsEngine engine, RegExpObject pattern, string input, JsValue replacement)
    {
        var callable = replacement.AsObjectOrNull() is { IsCallable: true };
        var template = callable ? string.Empty : engine.ToStringValue(replacement);
        var builder = new System.Text.StringBuilder(input.Length);
        var copied = 0;

        if (pattern.Global)
        {
            pattern.LastIndex = JsValue.Number(0);
        }

        while (true)
        {
            engine.Charge(4);
            var match = RegExpMatchOne(engine, pattern, input);

            if (match is null)
            {
                break;
            }

            builder.Append(input, copied, match.Index - copied);

            builder.Append(callable
                ? RegExpCallReplacement(engine, pattern, replacement, match, input)
                : RegExpExpand(engine, pattern, template, match, input));

            copied = match.End;

            if (!pattern.Global)
            {
                break;
            }

            // An empty match must advance, or the replacement of "" by "-" never terminates.
            if (match.Length == 0)
            {
                pattern.LastIndex = JsValue.Number(
                    JsRegExpMatcher.Advance(input, match.End, pattern.Unicode));
            }
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

    /// <summary>Calls a function replacement with <c>(match, p1..pn, offset, string, groups)</c>.</summary>
    /// <remarks>
    /// The <c>groups</c> object is appended only when the pattern named a group, which is what the
    /// specification says and what keeps <c>function (whole, offset, text)</c> - the shape every
    /// replacement written before named groups existed has - working unchanged.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=1DB2AE
    // Broiler-Human:        PENDING
    private static string RegExpCallReplacement(
        JsEngine engine,
        RegExpObject pattern,
        JsValue replacement,
        JsRegExpMatch match,
        string input)
    {
        var named = pattern.Matcher.HasGroupNames;
        var count = match.CaptureCount;
        var passed = new JsValue[count + (named ? 4 : 3)];
        passed[0] = JsValue.String(match.TextOf(input, 0));

        for (var at = 1; at <= count; at++)
        {
            engine.Charge(1);
            passed[at] = match.Participated(at)
                ? JsValue.String(match.TextOf(input, at))
                : JsValue.Undefined;
        }

        passed[count + 1] = JsValue.Number(match.Index);
        passed[count + 2] = JsValue.String(input);

        if (named)
        {
            passed[count + 3] = RegExpNamedGroups(pattern, match, input);
        }

        return engine.ToStringValue(engine.Call(replacement, JsValue.Undefined, passed));
    }

    /// <summary>The <c>groups</c> object, or <c>undefined</c> when the pattern named nothing.</summary>
    /// <remarks>
    /// The specification builds it with a null prototype, and so does this: an object whose
    /// prototype is nothing at all is what keeps <c>result.groups.toString</c> from finding
    /// <c>Object.prototype</c>'s. The realm can build one, so there is no deviation to declare here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=50A993
    // Broiler-Human:        PENDING
    private static JsValue RegExpNamedGroups(
        RegExpObject pattern, JsRegExpMatch match, string input)
    {
        if (!pattern.Matcher.HasGroupNames)
        {
            return JsValue.Undefined;
        }

        var groups = new JsObject(null);

        for (var at = 1; at <= match.CaptureCount; at++)
        {
            var name = pattern.Matcher.NameOf(at);

            if (name is null)
            {
                continue;
            }

            groups.DefineOrdinary(
                name,
                match.Participated(at) ? JsValue.String(match.TextOf(input, at)) : JsValue.Undefined);
        }

        return JsValue.Object(groups);
    }

    /// <summary>Expands the dollar substitutions of a string replacement.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=7A2957
    // Broiler-Human:        PENDING
    private static string RegExpExpand(
        JsEngine engine,
        RegExpObject pattern,
        string template,
        JsRegExpMatch match,
        string input)
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
                    builder.Append(match.TextOf(input, 0));
                    at += 2;
                    continue;

                case '`':
                    builder.Append(input, 0, match.Index);
                    at += 2;
                    continue;

                case '\'':
                    builder.Append(input, match.End, input.Length - match.End);
                    at += 2;
                    continue;

                case '<':
                    if (RegExpExpandName(pattern, template, match, input, ref at, builder))
                    {
                        continue;
                    }

                    break;

                default:
                    break;
            }

            if (next is >= '0' and <= '9')
            {
                var count = match.CaptureCount;
                var number = next - '0';
                var width = 2;

                // `$12` names the twelfth group when there is one and the first group followed by a
                // "2" when there is not, which is the rule the specification states and every
                // implementation reproduces.
                if (at + 2 < template.Length && template[at + 2] is >= '0' and <= '9')
                {
                    var wider = (number * 10) + (template[at + 2] - '0');

                    if (wider >= 1 && wider <= count)
                    {
                        number = wider;
                        width = 3;
                    }
                }

                if (number >= 1 && number <= count)
                {
                    if (match.Participated(number))
                    {
                        builder.Append(match.TextOf(input, number));
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

    /// <summary>Expands one <c>$&lt;name&gt;</c> substitution, or reports that it is not one.</summary>
    /// <remarks>
    /// With no named group in the pattern, <c>$&lt;</c> is two ordinary characters and not a
    /// malformed substitution - which is what keeps a replacement text carrying a less-than sign
    /// from changing meaning when it is used with a different pattern.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=793C46
    // Broiler-Human:        PENDING
    private static bool RegExpExpandName(
        RegExpObject pattern,
        string template,
        JsRegExpMatch match,
        string input,
        ref int at,
        System.Text.StringBuilder builder)
    {
        if (!pattern.Matcher.HasGroupNames)
        {
            return false;
        }

        var close = template.IndexOf('>', at + 2);

        if (close < 0)
        {
            return false;
        }

        var name = template.Substring(at + 2, close - at - 2);
        var group = pattern.Matcher.NumberOf(name);
        at = close + 1;

        if (group >= 1 && group <= match.CaptureCount && match.Participated(group))
        {
            builder.Append(match.TextOf(input, group));
        }

        return true;
    }

    /// <summary><c>split</c> with a RegExp separator: every match is a boundary, captures included.</summary>
    /// <remarks>
    /// The specification splits with a STICKY clone of the pattern and never touches the original's
    /// <c>lastIndex</c>, and that is what this does: the anchored attempt walks the string one
    /// position at a time. What it does not do is search forward, because a separator that can match
    /// at two places must be tried at the earlier one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=98F065
    // Broiler-Human:        PENDING
    private JsArray RegExpSplitPattern(
        JsEngine engine, RegExpObject pattern, string input, long limit)
    {
        var pieces = new System.Collections.Generic.List<JsValue>();

        if (input.Length == 0)
        {
            // AN EMPTY STRING SPLITS INTO NOTHING WHEN THE SEPARATOR MATCHES IT and into one empty
            // piece when it does not. `"".split(/x/)` is `[""]` and `"".split(/(?:)/)` is `[]`.
            if (RegExpRun(engine, pattern, input, 0, true) is null)
            {
                pieces.Add(JsValue.String(input));
            }

            return NewArray(pieces);
        }

        var start = 0;
        var scan = 0;

        while (scan < input.Length)
        {
            engine.Charge(4);
            var match = RegExpRun(engine, pattern, input, scan, false);

            if (match is null || match.Index >= input.Length)
            {
                break;
            }

            var end = System.Math.Min(match.End, input.Length);

            if (end == start)
            {
                // An empty match sitting on the start of the pending piece would produce an empty
                // piece for every position; the specification steps past it instead.
                scan = JsRegExpMatcher.Advance(input, match.Index, pattern.Unicode);
                continue;
            }

            pieces.Add(JsValue.String(input.Substring(start, match.Index - start)));

            if (pieces.Count >= limit)
            {
                return NewArray(pieces);
            }

            for (var at = 1; at <= match.CaptureCount; at++)
            {
                engine.Charge(1);

                pieces.Add(match.Participated(at)
                    ? JsValue.String(match.TextOf(input, at))
                    : JsValue.Undefined);

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
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=464FC0
        // Broiler-Human:        PENDING
        internal RegExpObject(
            JsObject? prototype,
            string source,
            string flags,
            JsRegExpMatcher matcher)
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
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=C47205
        // Broiler-Human:        PENDING
        internal JsRegExpMatcher Matcher { get; }

        /// <summary>Whether the <c>g</c> flag is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=5988AA
        // Broiler-Human:        PENDING
        internal bool Global { get; }

        /// <summary>Whether the <c>y</c> flag is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=F09C77
        // Broiler-Human:        PENDING
        internal bool Sticky { get; }

        /// <summary>Whether the <c>u</c> flag is set, which decides how an index advances.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=B271D1
        // Broiler-Human:        PENDING
        internal bool Unicode => Matcher.Unicode;

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
