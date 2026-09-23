// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   53
// Annotated:        53/53
// Exempt:           12
// Human-reviewed:   0/53
// IP risk:          Medium
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  4/10 max
// Unverified:       53
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
/// <b>Property escapes are <c>u</c>-mode only.</b> Under <c>u</c>, <c>\p{...}</c> and <c>\P{...}</c>
/// resolve against the pinned Unicode 17.0.0 tables by the exact names ES2026 admits (JSeal F09);
/// the seven properties of strings are <c>SyntaxError</c>s, as the language makes them under
/// <c>u</c>. Outside <c>u</c> a <c>\p</c> is the identity escape Annex B makes it.
/// </item>
/// <item>
/// <b>No <c>v</c> flag</b>, and none of the set operations, string properties or nested classes it
/// brings. A literal carrying it is an early <c>SyntaxError</c> when the source is compiled, whose
/// message names the flag as unsupported, and a flag string holding it is a <c>SyntaxError</c>
/// from the constructor; neither is what the language answers. A literal's other flags are
/// checked when it is compiled too: a letter outside <c>dgimsuyv</c>, a repeat, or <c>u</c> with
/// <c>v</c> is the language's early <c>SyntaxError</c> (JSeal slice JSD-0031-later).
/// </item>
/// <item>
/// <b><c>d</c> is parsed, ordered and reported, and builds no <c>indices</c>.</b> The flag is
/// accepted, appears in <c>flags</c> in the specification's position, and <c>hasIndices</c> answers
/// for it - but an <c>exec</c> result has no <c>indices</c> property, so code that reads one gets
/// <c>undefined</c> rather than the array of offset pairs. The offsets exist inside the matcher;
/// what is missing is the object, and it is missing because nothing this profile runs asks for it.
/// </item>
/// <item>
/// <b>Canonicalize reads the pinned tables, and differs in one known place.</b> Under <c>u</c> it
/// is <c>CaseFolding.txt</c>'s simple folding; without it, <c>UnicodeData.txt</c>'s simple upper
/// case of one code unit with the language's two exceptions (JSeal F09 and slice
/// JSD-0031-later), so neither depends on the host's globalization mode and <c>/\uA7CE/i</c>
/// matches U+A7CF. The specification's non-<c>u</c> upper-casing is the full one, under which the
/// 27 Greek letters with a ypogegrammeni (U+1F80 and its neighbours) become two code points and
/// so stay themselves; <c>SpecialCasing.txt</c> is not archived, and they match their title-case
/// partner here.
/// </item>
/// <item>
/// <b>A pattern may not nest more than 128 groups deep.</b> Past that the parser raises a
/// <c>SyntaxError</c> the language would not have raised. It is the declared price of a recursive
/// descent parser in a profile that has already lost a process to a stack it could not translate,
/// and it is far past anything a person writes.
/// </item>
/// <item>
/// <b>Under <c>u</c>, a <c>lastIndex</c> inside a surrogate pair reports the pair's start.</b>
/// The match starts at the code point holding <c>lastIndex</c>, as RegExpBuiltinExec says, and its
/// <c>index</c> is that code point's first unit rather than the mid-pair <c>lastIndex</c> the
/// pinned text writes there, which for an empty match would place the start after the end
/// GetMatchString asserts it precedes. The comparison engine answers the same (JSeal slice
/// JSD-0031-later).
/// </item>
/// <item>
/// <b>A duplicate group name in alternatives that cannot both match is refused</b>, which the
/// specification now admits. Group names themselves - literal, <c>\u</c>-escaped, or outside the
/// Basic Multilingual Plane - are classified by the pinned Unicode 17.0.0 <c>ID_Start</c> and
/// <c>ID_Continue</c> the tokenizer also reads (JSeal slice JSD-0031-later). The second
/// refusal is what the comparison engine at the version measured against does too, so it costs
/// nothing today and will cost something the day that engine moves.
/// </item>
/// <item>
/// <b>The pattern protocol dispatches, and the RegExp's own <c>exec</c> answers for it.</b>
/// The five String methods ask their argument for <c>Symbol.match</c>, <c>Symbol.matchAll</c>,
/// <c>Symbol.replace</c>, <c>Symbol.search</c> or <c>Symbol.split</c> and call it when it answers,
/// so a program's own object is a pattern — <i>(this read "no Symbols at all" until 2026-09-05,
/// which was true when this surface had none and had stopped being true; corrected as
/// JSC-129)</i>. Only an Object pattern is asked, and what does not dispatch is built into a fresh
/// RegExp from its String form whose own Symbol is then Invoked. The five methods on
/// <c>RegExp.prototype</c>, and <c>test</c>, take any Object receiver, read its <c>flags</c>
/// property and go through RegExpExec, so a subclass overriding <c>exec</c> changes what
/// <c>s.match(re)</c> answers as well as <c>re.exec(s)</c>; <c>split</c> and <c>matchAll</c> build
/// their matcher through the species <i>(matchAll since VM-FIX-E, the other four and <c>test</c>
/// since VM-FIX-G, both 2026-09-21; until then they ran the matcher directly)</i>. Where
/// RegExpExec is certain to be the built-in one - a RegExp of this realm with no own <c>exec</c>
/// under an untouched <c>%RegExp.prototype%</c> - <c>replace</c> and <c>split</c> run the matcher
/// without building result Arrays, which nothing can observe. <c>replaceAll</c> dispatches the
/// same way, after refusing a RegExp whose <c>flags</c> lack "g" <i>(it took a string pattern only
/// until 2026-09-21; JSeal slice V04)</i>.
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

    /// <summary>
    /// <c>%RegExp%</c>: the constructor the call-form identity check compares with and the default
    /// <c>RegExp.prototype[Symbol.matchAll]</c>'s species lookup falls back to.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=01A144
    // Broiler-Human:        PENDING
    private JsNativeFunction regExpFunction = null!;

    /// <summary>
    /// <c>%RegExp.prototype.exec%</c>: the function a receiver's <c>exec</c> must still be for
    /// <c>Symbol.replace</c> and <c>Symbol.split</c> to run the matcher directly.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B5C19D
    // Broiler-Human:        PENDING
    private JsObject regExpExecFunction = null!;

    /// <summary>
    /// The flag properties the <c>flags</c> getter reads, in the specification's order, with the
    /// letter each one contributes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6B52B5
    // Broiler-Human:        PENDING
    private static readonly (string Name, char Flag)[] RegExpFlagProperties =
    [
        ("hasIndices", 'd'),
        ("global", 'g'),
        ("ignoreCase", 'i'),
        ("multiline", 'm'),
        ("dotAll", 's'),
        ("unicode", 'u'),
        ("unicodeSets", 'v'),
        ("sticky", 'y'),
    ];

    /// <summary>Builds <c>RegExp</c>, <c>RegExp.prototype</c>, and the String methods that take one.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=98E366
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
                var patternIsRegExp = RegExpIsRegExp(engine, pattern);

                // CALLED AS A FUNCTION ON A REGEXP WITH NO FLAGS OF ITS OWN, `RegExp` HANDS BACK
                // THE VERY OBJECT IT WAS GIVEN - but only when that object's `constructor` IS this
                // function. "A RegExp" is the language's IsRegExp, so a RegExp-like object with a
                // truthy `Symbol.match` and `constructor: RegExp` comes back as itself, and a real
                // RegExp whose `constructor` was changed gets a fresh copy. Constructed, it always
                // produces a fresh one, which is what makes `new RegExp(re)` a way to reset a
                // shared `lastIndex`.
                if (patternIsRegExp && flags.Type == JsType.Undefined)
                {
                    var patternConstructor = engine.GetProperty(pattern, "constructor");

                    if (ReferenceEquals(patternConstructor.AsObjectOrNull(), regExpFunction))
                    {
                        return pattern;
                    }
                }

                return JsValue.Object(
                    RegExpConstruct(engine, pattern, flags, patternIsRegExp, JsValue.Undefined));
            },
            (engine, thisValue, arguments) =>
            {
                // The new target arrives in the receiver slot (JsNativeFunction.Construct).
                var pattern = ArgOfRegExp(arguments, 0);

                return JsValue.Object(RegExpConstruct(
                    engine,
                    pattern,
                    ArgOfRegExp(arguments, 1),
                    RegExpIsRegExp(engine, pattern),
                    thisValue));
            });

        // `RegExp` READS `new.target.prototype` ITSELF, after the pattern's `source` and `flags`
        // and before either is converted, which is where the language's RegExpAlloc puts it; the
        // engine's own re-pointing would read it a second time, after the pattern was compiled.
        regExpConstructor.BuildsFromNewTarget = true;
        regExpFunction = regExpConstructor;

        // `RegExp.escape` TAKES A STRING AND NOTHING ELSE. The specification converts nothing here:
        // a number, a String wrapper and an object with its own `toString` are each a TypeError,
        // and the object's `toString` is never called. The answer is pattern text that matches the
        // argument literally wherever it is placed - see RegExpEscapeText for the rules.
        Method(regExpConstructor, "escape", 1, static (engine, thisValue, arguments) =>
        {
            _ = thisValue;
            var given = ArgOfRegExp(arguments, 0);

            if (!given.IsString)
            {
                return engine.ThrowTypeError("RegExp.escape requires a String argument");
            }

            return JsValue.String(RegExpEscapeText(engine, given.AsString()));
        });

        Method(prototype, "exec", 1, (engine, thisValue, arguments) =>
        {
            var target = RegExpThis(engine, thisValue, "exec");
            return RegExpExecute(engine, target, engine.ToStringValue(ArgOfRegExp(arguments, 0)));
        });

        prototype.TryGetOwnProperty("exec", out var builtinExec);
        regExpExecFunction = builtinExec.Value.AsObject();

        // `test` IS RegExpExec AND A COMPARISON WITH `null`, so it takes any Object receiver and a
        // custom `exec` answers for it; only `exec` itself requires the matcher slot.
        Method(prototype, "test", 1, (engine, thisValue, arguments) =>
        {
            if (!thisValue.IsObject)
            {
                return engine.ThrowTypeError(
                    "RegExp.prototype.test called on a value that is not an object");
            }

            var input = engine.ToStringValue(ArgOfRegExp(arguments, 0));
            return JsValue.Boolean(RegExpExec(engine, thisValue, input).Type != JsType.Null);
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

        // `flags` ASKS THE RECEIVER FOR EACH FLAG BY NAME, in the specification's order, and reads
        // no slot: any Object is a receiver, an own `global` of `false` drops the "g", and
        // `RegExp.prototype.flags` is "" because every flag getter answers `undefined` there (it
        // answered the internal flags of a real RegExp until 2026-09-21; JSeal follow-up
        // VM-FIX-G). `unicodeSets` is read like the others; no getter answers it here, because no
        // RegExp this matcher builds can carry `v`.
        RegExpGetter(prototype, "flags", (engine, thisValue, arguments) =>
        {
            if (!thisValue.IsObject)
            {
                return engine.ThrowTypeError(
                    "RegExp.prototype.flags called on a value that is not an object");
            }

            var builder = new System.Text.StringBuilder(8);

            foreach (var (name, flag) in RegExpFlagProperties)
            {
                if (engine.GetProperty(thisValue, name).ToBooleanValue())
                {
                    builder.Append(flag);
                }
            }

            return JsValue.String(builder.ToString());
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
        //
        // ALL FIVE TAKE ANY OBJECT AND ASK IT, NOT ITS SLOTS. Each reads the receiver's `flags`
        // property and runs every match through RegExpExec, so an own `flags`, a subclass's
        // `exec` and a RegExp-like object are what answer; `split` and `matchAll` also build their
        // matcher through the receiver's species. Until 2026-09-21 (JSeal follow-up VM-FIX-G)
        // `match`, `replace`, `search` and `split` required a real RegExp and ran its matcher
        // directly, so a custom `exec` changed what `re.exec(s)` answered and not `s.match(re)`.
        RegExpSymbolMethod(prototype, MatchSymbol, "[Symbol.match]", 1,
            (engine, receiver, arguments) =>
                RegExpSymbolMatch(engine, receiver, ArgOfRegExp(arguments, 0)));

        RegExpSymbolMethod(prototype, MatchAllSymbol, "[Symbol.matchAll]", 1,
            (engine, receiver, arguments) =>
                RegExpMatchAllThrough(engine, receiver, ArgOfRegExp(arguments, 0)));

        RegExpSymbolMethod(prototype, SearchSymbol, "[Symbol.search]", 1,
            (engine, receiver, arguments) =>
                RegExpSymbolSearch(engine, receiver, ArgOfRegExp(arguments, 0)));

        RegExpSymbolMethod(prototype, ReplaceSymbol, "[Symbol.replace]", 2,
            (engine, receiver, arguments) =>
                RegExpSymbolReplace(
                    engine, receiver, ArgOfRegExp(arguments, 0), ArgOfRegExp(arguments, 1)));

        RegExpSymbolMethod(prototype, SplitSymbol, "[Symbol.split]", 2,
            (engine, receiver, arguments) =>
                RegExpSymbolSplit(
                    engine, receiver, ArgOfRegExp(arguments, 0), ArgOfRegExp(arguments, 1)));

        // ---- the String methods that take a RegExp ------------------------------------------
        //
        // `match` and `search` are defined here because they exist only for regular expressions.
        // `replace`, `replaceAll` and `split` are REDEFINED here: SetupString ran first and defined
        // string-only versions, and this file runs after it and overwrites them, because the
        // specification's versions accept either a RegExp or a string and only this file knows
        // what a RegExp is. The string-pattern behaviour is not lost - each asks an Object
        // argument for its Symbol and falls back to the string path when it does not answer, so
        // `"a,b".split(",")` and `"aa".replace("a", "b")` answer exactly as before.

        Method(StringPrototype, "match", 1, (engine, thisValue, arguments) =>
        {
            var given = ArgOfRegExp(arguments, 0);

            if (RegExpDispatch(engine, thisValue, given, engine.Realm.MatchSymbol, out var answered))
            {
                return answered;
            }

            // WHAT DID NOT DISPATCH IS BUILT INTO A FRESH RegExp AND ASKED AGAIN. The language's
            // RegExpCreate converts the argument with ToString - a RegExp whose `Symbol.match` was
            // removed is compiled from its printed form, "/a/g" - and then Invokes the new
            // object's `Symbol.match`, so a replaced `RegExp.prototype[Symbol.match]` is the one
            // that answers. A global `match` collects the matched text and leaves `lastIndex` at
            // zero; that is RegExpSymbolMatch.
            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, given, string.Empty);
            return RegExpInvoke(engine, pattern, engine.Realm.MatchSymbol, input);
        });

        // EVERY MATCH WITH ITS CAPTURES, WHICH IS THE ONE THING A GLOBAL `match` WILL NOT GIVE.
        // `"a1".match(/(\w)(\d)/g)` answers the matched TEXT of each match and throws the groups
        // away, so a program that wants both has to loop `exec` and remember `lastIndex` itself.
        // This is that loop, as an iterator, and it is the reason the method exists.
        Method(StringPrototype, "matchAll", 1, (engine, thisValue, arguments) =>
        {
            var given = ArgOfRegExp(arguments, 0);

            if (thisValue.IsNullish)
            {
                return engine.ThrowTypeError("String.prototype method called on null or undefined");
            }

            // ONLY AN OBJECT IS CHECKED OR ASKED. The globality check comes before the dispatch and
            // not inside it, which is the one place this method's order differs from `match`,
            // `search` and `split`: the language checks that a RegExp argument is global first, so
            // `"x".matchAll(/a/)` is a TypeError even though `RegExp.prototype[Symbol.matchAll]`
            // would have accepted it. The check is the language's IsRegExp and a READ of `flags`,
            // not the internal flag, so a RegExp-like object is held to it and an own `flags` of ""
            // refuses a pattern built with `g`. A primitive pattern never reaches a
            // `Symbol.matchAll` a program put on its prototype.
            if (given.IsObject)
            {
                RegExpRequireGlobal(engine, given, "matchAll");

                if (RegExpDispatch(
                        engine, thisValue, given, engine.Realm.MatchAllSymbol, out var answered))
                {
                    return answered;
                }
            }

            // WHAT DID NOT DISPATCH BECOMES A FRESH "g" RegExp BUILT FROM ITS STRING FORM, and that
            // RegExp is asked for `Symbol.matchAll` in turn - the language's RegExpCreate and
            // Invoke. A RegExp whose `Symbol.matchAll` was removed is therefore iterated as the
            // pattern its `toString` prints, not as itself; there is no copy of the original here.
            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, given, "g");
            return RegExpInvoke(engine, pattern, engine.Realm.MatchAllSymbol, input);
        });

        Method(StringPrototype, "search", 1, (engine, thisValue, arguments) =>
        {
            var given = ArgOfRegExp(arguments, 0);

            if (RegExpDispatch(engine, thisValue, given, engine.Realm.SearchSymbol, out var answered))
            {
                return answered;
            }

            // As in `match`: a fresh RegExp from the argument's String form, asked for
            // `Symbol.search`. RegExpSymbolSearch restores any `lastIndex` it disturbs, and
            // a sticky pattern is still anchored, which is why "aab".search(/b/y) is -1.
            var input = RegExpStringThis(engine, thisValue);
            var pattern = RegExpFromArgument(engine, given, string.Empty);
            return RegExpInvoke(engine, pattern, engine.Realm.SearchSymbol, input);
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

            // WHAT DID NOT DISPATCH IS SEARCHED FOR AS TEXT, a RegExp included: one whose
            // `Symbol.replace` was removed is looked for as the characters its `toString` prints.
            var input = RegExpStringThis(engine, thisValue);
            engine.Charge((ulong)input.Length + 16);

            return JsValue.String(
                RegExpReplaceText(engine, input, engine.ToStringValue(search), replacement));
        });

        // `replaceAll` IS `replace` WITH A GUARD IN FRONT. A RegExp argument whose `flags` lack "g"
        // is refused before the pattern is asked for `Symbol.replace`, because a non-global
        // pattern would replace once under a name that promises every occurrence. What is not a
        // RegExp, or does not answer the Symbol, falls to the string path SetupString defined.
        Method(StringPrototype, "replaceAll", 2, (engine, thisValue, arguments) =>
        {
            var search = ArgOfRegExp(arguments, 0);
            var replacement = ArgOfRegExp(arguments, 1);

            if (thisValue.IsNullish)
            {
                return engine.ThrowTypeError("String.prototype method called on null or undefined");
            }

            if (!search.IsNullish)
            {
                RegExpRequireGlobal(engine, search, "replaceAll");
            }

            // ONLY AN OBJECT IS ASKED FOR THE SYMBOL. The language skips the lookup for a primitive
            // pattern, so `"1".replaceAll(1, r)` replaces the text "1" even when a program has put
            // a `Symbol.replace` on `Number.prototype`.
            if (search.IsObject &&
                RegExpDispatch(
                    engine, thisValue, search, engine.Realm.ReplaceSymbol, out var answered,
                    replacement))
            {
                return answered;
            }

            return StringReplaceWith(engine, thisValue, arguments, true);
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

            // A separator that did not dispatch is text, a RegExp whose `Symbol.split` was removed
            // included, exactly as in `replace`. It is converted BEFORE a zero limit answers, which
            // is the language's order: a throwing `toString` throws even from `split(x, 0)`.
            var text = engine.ToStringValue(separator);

            if (limit == 0)
            {
                return JsValue.Object(NewArray());
            }

            if (separator.Type == JsType.Undefined)
            {
                var whole = NewArray();
                whole.Push(JsValue.String(input));
                return JsValue.Object(whole);
            }

            return JsValue.Object(RegExpSplitText(engine, input, text, limit));
        });
    }

    /// <summary>Installs one of the five pattern methods under its Symbol.</summary>
    /// <remarks>
    /// <b>Each one requires an Object receiver and nothing more</b> - the language's first step in
    /// all five. What the receiver can do is asked through its properties: <c>flags</c>,
    /// <c>lastIndex</c>, <c>exec</c> and, for <c>split</c> and <c>matchAll</c>, its species
    /// <i>(a real RegExp was required until 2026-09-21; JSeal follow-up VM-FIX-G)</i>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=2893A6
    // Broiler-Human:        PENDING
    private void RegExpSymbolMethod(
        JsObject host,
        JsSymbol key,
        string name,
        int arity,
        System.Func<JsEngine, JsValue, JsValue[], JsValue> body) =>
        host.SetOwnSymbol(
            key,
            JsProperty.Data(
                JsValue.Object(Native(name, arity, (engine, thisValue, arguments) =>
                {
                    if (!thisValue.IsObject)
                    {
                        return engine.ThrowTypeError(
                            "RegExp.prototype" + name + " called on a value that is not an object");
                    }

                    return body(engine, thisValue, arguments);
                })),
                JsPropertyAttributes.BuiltIn));

    /// <summary>Hands the work to the pattern when the pattern says it can do it.</summary>
    /// <remarks>
    /// <b>The read is a <c>GetMethod</c> and the ORDER matters.</b> Only an Object pattern is asked
    /// at all — <c>"x".replace(null, r)</c> replaces the text <c>"null"</c>, and a Symbol a program
    /// put on <c>Number.prototype</c> or <c>String.prototype</c> is never reached through a
    /// primitive pattern <i>(every primitive was asked until 2026-09-21; the pinned Test262
    /// <c>cstm-*-on-*-primitive</c> cases)</i> — and a pattern whose
    /// Symbol is present but not callable is a <c>TypeError</c> rather than a fall-through, because
    /// an object that claims the protocol and cannot perform it is a mistake worth reporting. The
    /// receiver is coerced to a String only on the path that does NOT dispatch, since the method
    /// being dispatched to is handed the receiver as it stands.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=172590
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

        if (!pattern.IsObject)
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

    /// <summary>The language's <c>IsRegExp</c>: whether a value asks to be a regular expression.</summary>
    /// <remarks>
    /// <b>The Symbol answers before the brand does.</b> An object whose <c>Symbol.match</c> is
    /// anything but <c>undefined</c> is a RegExp exactly when that value is truthy, so a RegExp with
    /// <c>Symbol.match = false</c> is not one and a plain object with <c>Symbol.match = true</c> is;
    /// only when the Symbol is absent does the real matcher decide. The read is an ordinary
    /// <c>Get</c>, so a getter runs and a throwing one propagates.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=21B49B
    // Broiler-Human:        PENDING
    private static bool RegExpIsRegExp(JsEngine engine, JsValue value)
    {
        if (!value.IsObject)
        {
            return false;
        }

        var matcher = engine.GetSymbol(value, engine.Realm.MatchSymbol);

        if (matcher.Type != JsType.Undefined)
        {
            return matcher.ToBooleanValue();
        }

        return value.AsObject() is RegExpObject;
    }

    /// <summary>Refuses a regular-expression argument whose <c>flags</c> do not contain "g".</summary>
    /// <remarks>
    /// <b>The flags are READ, not looked up.</b> <c>replaceAll</c> and <c>matchAll</c> ask the
    /// argument for its <c>flags</c> property and convert it, so an own <c>flags</c> that says ""
    /// refuses a pattern built with <c>g</c>, a missing one is a <c>TypeError</c>, and an object that
    /// is only RegExp-like is held to what its own property says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=DE940A
    // Broiler-Human:        PENDING
    private static void RegExpRequireGlobal(JsEngine engine, JsValue pattern, string method)
    {
        if (!RegExpIsRegExp(engine, pattern))
        {
            return;
        }

        var flags = engine.GetProperty(pattern, "flags");

        if (flags.IsNullish)
        {
            engine.ThrowTypeError(
                "String.prototype." + method + " called with a RegExp whose flags are null or undefined");
        }

        if (!engine.ToStringValue(flags).Contains('g'))
        {
            engine.ThrowTypeError(
                "String.prototype." + method + " called with a non-global RegExp argument");
        }
    }

    /// <summary>Refuses a regular-expression argument to a method that searches for a String.</summary>
    /// <remarks>
    /// <c>startsWith</c>, <c>endsWith</c> and <c>includes</c> call this AFTER converting the receiver
    /// and BEFORE converting the argument, which is the language's order: a regular expression is
    /// a <c>TypeError</c> rather than a search for the characters of its source.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=09D1BA
    // Broiler-Human:        PENDING
    private static void RegExpRefuseAsSearchString(JsEngine engine, JsValue search, string method)
    {
        if (RegExpIsRegExp(engine, search))
        {
            engine.ThrowTypeError(
                "First argument to String.prototype." + method + " must not be a regular expression");
        }
    }

    /// <summary><c>RegExp.prototype[Symbol.match]</c>: one RegExpExec, or every match's text.</summary>
    /// <remarks>
    /// <b>Global is what the <c>flags</c> PROPERTY says</b>, so an own <c>flags</c> of "g" collects
    /// every match of a pattern built without it. A global walk sets <c>lastIndex</c> to zero with a
    /// throwing <c>Set</c>, runs RegExpExec until it answers <c>null</c>, and steps past an EMPTY
    /// match by reading <c>lastIndex</c> through <c>ToLength</c> and writing it back one code unit -
    /// or one code point under "u" - further on. The final <c>null</c> is what leaves a built-in
    /// matcher's <c>lastIndex</c> at zero.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1B6653
    // Broiler-Human:        PENDING
    private JsValue RegExpSymbolMatch(JsEngine engine, JsValue receiver, JsValue argument)
    {
        var input = engine.ToStringValue(argument);
        engine.Charge((ulong)input.Length + 16);
        var flags = engine.ToStringValue(engine.GetProperty(receiver, "flags"));

        if (!flags.Contains('g', System.StringComparison.Ordinal))
        {
            return RegExpExec(engine, receiver, input);
        }

        var fullUnicode = RegExpFullUnicode(flags);
        engine.SetProperty(receiver, "lastIndex", JsValue.Number(0), strict: true);
        var found = NewArray();

        while (true)
        {
            engine.Charge(4);
            var result = RegExpExec(engine, receiver, input);

            if (result.Type == JsType.Null)
            {
                return found.Length == 0 ? JsValue.Null : JsValue.Object(found);
            }

            var matched = engine.ToStringValue(engine.GetProperty(result, "0"));
            found.Push(JsValue.String(matched));

            if (matched.Length == 0)
            {
                RegExpStepPastEmpty(engine, receiver, input, fullUnicode);
            }
        }
    }

    /// <summary><c>RegExp.prototype[Symbol.search]</c>: one RegExpExec from index zero.</summary>
    /// <remarks>
    /// <b><c>lastIndex</c> is saved, zeroed and restored with property operations</b>, each skipped
    /// when the value already is what it would be set to - the language's <c>SameValue</c> tests,
    /// so a <c>lastIndex</c> of <c>-0</c> is written and an object one is never converted here. The
    /// answer is the result's <c>index</c> property, read as it stands.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=272DF4
    // Broiler-Human:        PENDING
    private JsValue RegExpSymbolSearch(JsEngine engine, JsValue receiver, JsValue argument)
    {
        var input = engine.ToStringValue(argument);
        engine.Charge((ulong)input.Length + 16);
        var previous = engine.GetProperty(receiver, "lastIndex");

        if (!ObjectSameValue(previous, JsValue.Number(0)))
        {
            engine.SetProperty(receiver, "lastIndex", JsValue.Number(0), strict: true);
        }

        var result = RegExpExec(engine, receiver, input);
        var current = engine.GetProperty(receiver, "lastIndex");

        if (!ObjectSameValue(current, previous))
        {
            engine.SetProperty(receiver, "lastIndex", previous, strict: true);
        }

        return result.Type == JsType.Null
            ? JsValue.Number(-1)
            : engine.GetProperty(result, "index");
    }

    /// <summary>
    /// <c>RegExp.prototype[Symbol.replace]</c>: every result RegExpExec answers, then the
    /// replacement built from each result's properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Two passes, as the language has them.</b> The first collects results - one, or every one
    /// until <c>null</c> when the <c>flags</c> property carries "g" - stepping past empty matches
    /// as <c>match</c> does. The second reads each result as an ordinary object: its length gives
    /// the capture count, <c>0</c> the matched text, <c>index</c> the position (clamped to the
    /// input), each capture converted unless <c>undefined</c>, and <c>groups</c>. A result whose
    /// position went backwards contributes nothing, which is how an ill-behaved <c>exec</c> is
    /// survived rather than trusted.
    /// </para>
    /// <para>
    /// <b>A function replacement gets <c>groups</c> whenever the result has one</b>, and a text
    /// replacement expands <c>$&lt;name&gt;</c> against <c>ToObject(groups)</c> - through
    /// <see cref="RegExpSubstitute"/>, the language's GetSubstitution. Each capture and each piece
    /// of the answer is charged, and the answer is held to <c>StringLengthCeiling</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=26749C
    // Broiler-Human:        PENDING
    private JsValue RegExpSymbolReplace(
        JsEngine engine, JsValue receiver, JsValue argument, JsValue replaceValue)
    {
        var input = engine.ToStringValue(argument);
        engine.Charge((ulong)input.Length + 16);
        var functional = replaceValue.AsObjectOrNull() is { IsCallable: true };
        var template = functional ? string.Empty : engine.ToStringValue(replaceValue);
        var flags = engine.ToStringValue(engine.GetProperty(receiver, "flags"));
        var global = flags.Contains('g', System.StringComparison.Ordinal);

        if (global)
        {
            engine.SetProperty(receiver, "lastIndex", JsValue.Number(0), strict: true);
        }

        // A PRISTINE RECEIVER'S RESULTS ARE KEPT AS MATCHES rather than built into Arrays and read
        // back: its `exec` is the built-in one reached as plain data, no guest code runs between
        // the attempts, and every property of a fresh result is an own data property, so neither
        // the attempts nor the reads below can be told apart from the language's.
        var pristine = RegExpPristineExec(receiver);
        var results = new System.Collections.Generic.List<JsValue>();
        var matches = new System.Collections.Generic.List<JsRegExpMatch>();

        while (true)
        {
            engine.Charge(4);
            bool empty;

            if (pristine is not null)
            {
                var match = RegExpMatchOne(engine, pristine, input);

                if (match is null)
                {
                    break;
                }

                matches.Add(match);
                empty = match.Length == 0;
            }
            else
            {
                var result = RegExpExec(engine, receiver, input);

                if (result.Type == JsType.Null)
                {
                    break;
                }

                results.Add(result);
                empty = global && engine.ToStringValue(engine.GetProperty(result, "0")).Length == 0;
            }

            if (!global)
            {
                break;
            }

            if (empty)
            {
                RegExpStepPastEmpty(engine, receiver, input, RegExpFullUnicode(flags));
            }
        }

        var builder = new System.Text.StringBuilder(input.Length);
        var nextSource = 0;
        var count = pristine is not null ? matches.Count : results.Count;

        for (var step = 0; step < count; step++)
        {
            string matched;
            int position;
            JsValue named;

            // ONE FUEL UNIT PER CAPTURE, CHARGED AS THE LIST GROWS: the count is the result's own
            // `length`, which a custom `exec` controls, as `apply`'s argument list does.
            var captures = new System.Collections.Generic.List<JsValue>();

            if (pristine is not null)
            {
                var match = matches[step];
                matched = match.TextOf(input, 0);
                position = match.Index;

                for (var at = 1; at <= match.CaptureCount; at++)
                {
                    engine.Charge(1);
                    captures.Add(match.Participated(at)
                        ? JsValue.String(match.TextOf(input, at))
                        : JsValue.Undefined);
                }

                named = RegExpNamedGroups(pristine, match, input);
            }
            else
            {
                var result = results[step];
                var captureCount = System.Math.Max(
                    ArrayToLength(engine, engine.GetProperty(result, "length")) - 1, 0);
                matched = engine.ToStringValue(engine.GetProperty(result, "0"));
                position = (int)System.Math.Clamp(
                    engine.ToInteger(engine.GetProperty(result, "index")), 0, input.Length);

                for (var at = 1d; at <= captureCount; at++)
                {
                    engine.Charge(1);
                    var capture = engine.GetIndexed(result, JsValue.Number(at));
                    captures.Add(capture.Type == JsType.Undefined
                        ? capture
                        : JsValue.String(engine.ToStringValue(capture)));
                }

                named = engine.GetProperty(result, "groups");
            }

            string replacement;

            if (functional)
            {
                var passed = new JsValue[captures.Count + (named.Type == JsType.Undefined ? 3 : 4)];
                passed[0] = JsValue.String(matched);
                captures.CopyTo(passed, 1);
                passed[captures.Count + 1] = JsValue.Number(position);
                passed[captures.Count + 2] = JsValue.String(input);

                if (named.Type != JsType.Undefined)
                {
                    passed[captures.Count + 3] = named;
                }

                replacement = engine.ToStringValue(engine.Call(replaceValue, JsValue.Undefined, passed));
            }
            else
            {
                if (named.Type != JsType.Undefined)
                {
                    named = JsValue.Object(engine.ToObject(named));
                }

                replacement = RegExpSubstitute(
                    engine, matched, input, position, captures, named, template);
            }

            if (position >= nextSource)
            {
                RegExpAppend(engine, builder, input, nextSource, position - nextSource);
                RegExpAppend(engine, builder, replacement);
                nextSource = position + matched.Length;
            }
        }

        if (nextSource < input.Length)
        {
            RegExpAppend(engine, builder, input, nextSource, input.Length - nextSource);
        }

        return JsValue.String(builder.ToString());
    }

    /// <summary>
    /// <c>RegExp.prototype[Symbol.split]</c>: a sticky splitter built through the receiver's
    /// species, tried at each position in turn.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The splitter is the language's.</b> <c>SpeciesConstructor(rx, %RegExp%)</c> is
    /// constructed with <c>(rx, flags + "y")</c> - "y" added only when the <c>flags</c> property
    /// lacks it - and every attempt sets its <c>lastIndex</c> to the position with a throwing
    /// <c>Set</c>, runs RegExpExec, and on success reads <c>lastIndex</c> back through
    /// <c>ToLength</c> for the end of the separator. The receiver's own <c>lastIndex</c> is never
    /// touched. A separator ending where the pending piece starts is stepped over, which is why
    /// <c>/(?:)/</c> splits into code units rather than into empty pieces.
    /// </para>
    /// <para>
    /// <b>A pristine splitter is walked by the matcher's own forward search.</b> When the splitter
    /// is a sticky RegExp of this realm whose <c>exec</c> is the built-in one reached as plain data
    /// through <c>%RegExp.prototype%</c>, whose unicode flag agrees with the one the positions
    /// advance by, and whose <c>lastIndex</c> is writable, none of the per-position steps can run
    /// guest code or be observed, and failing attempts do nothing but move on; the forward search
    /// is those attempts, in the same order, without a native call per position. The splitter's
    /// <c>lastIndex</c> is left where the per-position walk would have left it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4F965D
    // Broiler-Human:        PENDING
    private JsValue RegExpSymbolSplit(
        JsEngine engine, JsValue receiver, JsValue argument, JsValue bound)
    {
        var input = engine.ToStringValue(argument);
        engine.Charge((ulong)input.Length + 16);
        var species = BinarySpeciesConstructor(engine, receiver, regExpFunction);
        var flags = engine.ToStringValue(engine.GetProperty(receiver, "flags"));
        var unicodeMatching = RegExpFullUnicode(flags);
        var stickyFlags = flags.Contains('y', System.StringComparison.Ordinal) ? flags : flags + "y";
        var splitter = engine.Construct(
            species, [receiver, JsValue.String(stickyFlags)], species);
        var limit = bound.Type == JsType.Undefined ? 4294967295L : engine.ToUint32(bound);
        var pieces = new System.Collections.Generic.List<JsValue>();

        if (limit == 0)
        {
            return JsValue.Object(NewArray());
        }

        if (input.Length == 0)
        {
            // AN EMPTY STRING SPLITS INTO NOTHING WHEN THE SEPARATOR MATCHES IT and into one empty
            // piece when it does not. `"".split(/x/)` is `[""]` and `"".split(/(?:)/)` is `[]`.
            if (RegExpExec(engine, splitter, input).Type == JsType.Null)
            {
                pieces.Add(JsValue.String(input));
            }

            return JsValue.Object(NewArray(pieces));
        }

        var pristine = RegExpPristineSplitter(splitter, unicodeMatching);
        var size = input.Length;
        var start = 0;
        var at = 0;

        while (at < size)
        {
            engine.Charge(4);
            int end;
            JsRegExpMatch? match = null;
            JsValue result = JsValue.Null;

            if (pristine is not null)
            {
                match = RegExpRun(engine, pristine, input, at, false);

                if (match is null || match.Index >= size)
                {
                    // Every attempt from here to the end failed, and the last one zeroed it.
                    pristine.LastIndex = JsValue.Number(0);
                    break;
                }

                at = match.Index;
                pristine.LastIndex = JsValue.Number(match.End);
                end = System.Math.Min(match.End, size);
            }
            else
            {
                engine.SetProperty(splitter, "lastIndex", JsValue.Number(at), strict: true);
                result = RegExpExec(engine, splitter, input);

                if (result.Type == JsType.Null)
                {
                    at = (int)RegExpAdvanceIndex(input, at, unicodeMatching);
                    continue;
                }

                end = (int)System.Math.Min(
                    ArrayToLength(engine, engine.GetProperty(splitter, "lastIndex")), size);
            }

            if (end == start)
            {
                at = (int)RegExpAdvanceIndex(input, at, unicodeMatching);
                continue;
            }

            pieces.Add(JsValue.String(input.Substring(start, at - start)));

            if (pieces.Count >= limit)
            {
                return JsValue.Object(NewArray(pieces));
            }

            start = end;

            var captureCount = match is not null
                ? match.CaptureCount
                : System.Math.Max(ArrayToLength(engine, engine.GetProperty(result, "length")) - 1, 0);

            for (var group = 1d; group <= captureCount; group++)
            {
                engine.Charge(1);

                if (match is not null)
                {
                    var number = (int)group;
                    pieces.Add(match.Participated(number)
                        ? JsValue.String(match.TextOf(input, number))
                        : JsValue.Undefined);
                }
                else
                {
                    pieces.Add(engine.GetIndexed(result, JsValue.Number(group)));
                }

                if (pieces.Count >= limit)
                {
                    return JsValue.Object(NewArray(pieces));
                }
            }

            at = start;
        }

        pieces.Add(JsValue.String(input.Substring(start)));
        return JsValue.Object(NewArray(pieces));
    }

    /// <summary>
    /// The splitter <see cref="RegExpSymbolSplit"/> may walk with one forward search, or
    /// <c>null</c> when a per-position step could be observed.
    /// </summary>
    /// <remarks>
    /// Past <see cref="RegExpPristineExec"/>, every condition is one the per-position walk could
    /// otherwise tell apart: a splitter that is not sticky, a unicode flag the positions do not
    /// advance by, and a closed <c>lastIndex</c> whose first write would throw.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=665F26
    // Broiler-Human:        PENDING
    private RegExpObject? RegExpPristineSplitter(JsValue splitter, bool unicodeMatching) =>
        RegExpPristineExec(splitter) is { Sticky: true, LastIndexWritable: true } candidate &&
        candidate.Unicode == unicodeMatching
            ? candidate
            : null;

    /// <summary>
    /// <paramref name="value"/> as a RegExp of this realm whose RegExpExec is certain to be the
    /// built-in one without running guest code, or <c>null</c>.
    /// </summary>
    /// <remarks>
    /// The object has no own <c>exec</c>, its prototype is <c>%RegExp.prototype%</c>, and that
    /// prototype's <c>exec</c> is a data property holding <c>%RegExp.prototype.exec%</c>: the
    /// <c>Get</c> RegExpExec performs then runs nothing and answers the built-in, so a caller may
    /// run the matcher directly. A getter, a replaced method and a subclass instance all fail it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B97FF1
    // Broiler-Human:        PENDING
    private RegExpObject? RegExpPristineExec(JsValue value)
    {
        if (value.AsObjectOrNull() is not RegExpObject candidate ||
            !ReferenceEquals(candidate.Prototype, RegExpPrototype) ||
            candidate.TryGetOwnProperty("exec", out _) ||
            !RegExpPrototype.TryGetOwnProperty("exec", out var exec) ||
            exec.IsAccessor ||
            !ReferenceEquals(exec.Value.AsObjectOrNull(), regExpExecFunction))
        {
            return null;
        }

        return candidate;
    }

    /// <summary>Whether a <c>flags</c> string asks for code point steps: "u" or "v".</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=C493C1
    // Broiler-Human:        PENDING
    private static bool RegExpFullUnicode(string flags) =>
        flags.Contains('u', System.StringComparison.Ordinal) ||
        flags.Contains('v', System.StringComparison.Ordinal);

    /// <summary>The language's <c>AdvanceStringIndex</c> over an index that may be past the input.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FD5506
    // Broiler-Human:        PENDING
    private static double RegExpAdvanceIndex(string input, double index, bool fullUnicode) =>
        fullUnicode && index + 1 < input.Length
            ? JsRegExpMatcher.Advance(input, (int)index, true)
            : index + 1;

    /// <summary>
    /// Moves <c>lastIndex</c> past an empty match: read through <c>ToLength</c>, advanced, and
    /// written back with a throwing <c>Set</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=E528E4
    // Broiler-Human:        PENDING
    private static void RegExpStepPastEmpty(
        JsEngine engine, JsValue matcher, string input, bool fullUnicode)
    {
        var thisIndex = ArrayToLength(engine, engine.GetProperty(matcher, "lastIndex"));

        engine.SetProperty(
            matcher,
            "lastIndex",
            JsValue.Number(RegExpAdvanceIndex(input, thisIndex, fullUnicode)),
            strict: true);
    }

    /// <summary>Appends one piece of a replacement's answer, charged and held to the ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=4435FC
    // Broiler-Human:        PENDING
    private static void RegExpAppend(
        JsEngine engine, System.Text.StringBuilder builder, string text, int start = 0, int count = -1)
    {
        if (count < 0)
        {
            count = text.Length - start;
        }

        engine.Charge((ulong)count + 1);

        if ((long)builder.Length + count > StringLengthCeiling)
        {
            throw engine.Error("RangeError", "Invalid string length");
        }

        builder.Append(text, start, count);
    }

    /// <summary>
    /// <c>RegExp.prototype[Symbol.matchAll]</c>: a matcher built through the receiver's species,
    /// walked by a RegExp String Iterator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every step is the language's, in its order, and each one is observable.</b> The receiver
    /// need only be an Object. The argument is converted; <c>SpeciesConstructor(R, %RegExp%)</c>
    /// reads <c>constructor</c> and its <c>Symbol.species</c>; the receiver's <c>flags</c> PROPERTY
    /// is read and converted - not its internal flags, so an own <c>flags</c> decides - and the
    /// matcher is constructed from <c>(R, flags)</c>; then <c>lastIndex</c> is read through
    /// <c>ToLength</c> and written to the matcher with a throwing <c>Set</c>.
    /// </para>
    /// <para>
    /// <b>The matcher is a copy, so the receiver's own <c>lastIndex</c> is never moved</b>: a
    /// program that interleaves <c>matchAll</c> with <c>exec</c> on one RegExp sees neither disturb
    /// the other. This replaced a walk over an internal copy of the receiver's source and flags
    /// that honoured no species, no <c>flags</c> property and no custom <c>exec</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2A4A81
    // Broiler-Human:        PENDING
    private JsValue RegExpMatchAllThrough(JsEngine engine, JsValue receiver, JsValue argument)
    {
        // RegExpSymbolMethod has refused a receiver that is not an Object.
        var input = engine.ToStringValue(argument);
        engine.Charge((ulong)input.Length + 16);
        var species = BinarySpeciesConstructor(engine, receiver, regExpFunction);
        var flags = engine.ToStringValue(engine.GetProperty(receiver, "flags"));
        var matcher = engine.Construct(species, [receiver, JsValue.String(flags)], species);
        var lastIndex = ArrayToLength(engine, engine.GetProperty(receiver, "lastIndex"));
        engine.SetProperty(matcher, "lastIndex", JsValue.Number(lastIndex), strict: true);

        return JsValue.Object(RegExpStringIterator(
            matcher,
            input,
            flags.Contains('g', System.StringComparison.Ordinal),
            RegExpFullUnicode(flags)));
    }

    /// <summary>
    /// The language's <c>CreateRegExpStringIterator</c>: an iterator whose every step is one
    /// <c>RegExpExec</c> of <paramref name="matcher"/> over <paramref name="input"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The steps go through <c>exec</c></b>, so a matcher whose <c>exec</c> a program replaced -
    /// on the instance, on a subclass, or on <c>RegExp.prototype</c> - is what answers; and every
    /// read and write of <c>lastIndex</c> is an ordinary property operation on the matcher.
    /// </para>
    /// <para>
    /// <b>A non-global matcher answers one match and then is done.</b> It does not keep
    /// <c>lastIndex</c>, so without that the iterator answered its first match for ever. A global
    /// one advances past an EMPTY match by hand - reading <c>lastIndex</c> with <c>ToLength</c> and
    /// stepping one code unit, or one code point under <paramref name="fullUnicode"/> - or it would
    /// answer the same empty match for ever.
    /// </para>
    /// <para>
    /// <b>Only a <c>null</c> match or the non-global matcher's one match retires it</b> - the
    /// <c>[[Done]]</c> slot of the pinned <c>next</c>. It is a <see cref="JsBuiltinIterator.Resumable"/>
    /// one, so a step that throws - from <c>exec</c>, from reading or converting <c>match[0]</c>, or
    /// from the <c>lastIndex</c> read and write - leaves it to run <c>exec</c> again on the next
    /// call, and a custom <c>exec</c> that calls <c>next</c> again is answered rather than refused
    /// as a running generator would be.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B95B85
    // Broiler-Human:        PENDING
    private JsObject RegExpStringIterator(
        JsValue matcher, string input, bool global, bool fullUnicode)
    {
        var answered = false;

        return new JsBuiltinIterator(
            RegExpStringIteratorPrototype, "RegExp String Iterator", stepEngine =>
        {
            stepEngine.Charge(1);

            if (answered && !global)
            {
                return (false, JsValue.Undefined);
            }

            var match = RegExpExec(stepEngine, matcher, input);

            if (match.Type == JsType.Null)
            {
                return (false, JsValue.Undefined);
            }

            if (!global)
            {
                // [[Done]] IS SET WITH THE ONE MATCH, so a re-entrant next() from here on is done.
                answered = true;
                return (true, match);
            }

            if (stepEngine.ToStringValue(stepEngine.GetProperty(match, "0")).Length == 0)
            {
                RegExpStepPastEmpty(stepEngine, matcher, input, fullUnicode);
            }

            return (true, match);
        })
        {
            Resumable = true,
        };
    }

    /// <summary>
    /// The language's <c>RegExpExec</c>: the receiver's own <c>exec</c> when it has a callable one,
    /// and the built-in one otherwise.
    /// </summary>
    /// <remarks>
    /// A custom <c>exec</c> must answer an Object or <c>null</c>, and anything else is a
    /// <c>TypeError</c>. Without a callable <c>exec</c> only a real RegExp can be executed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=35101B
    // Broiler-Human:        PENDING
    private JsValue RegExpExec(JsEngine engine, JsValue receiver, string input)
    {
        var exec = engine.GetProperty(receiver, "exec");

        if (exec.IsObject && exec.AsObject().IsCallable)
        {
            var result = engine.Call(exec, receiver, [JsValue.String(input)]);

            if (!result.IsObject && result.Type != JsType.Null)
            {
                return engine.ThrowTypeError("a RegExp's exec answered neither an object nor null");
            }

            return result;
        }

        if (receiver.AsObjectOrNull() is not RegExpObject target)
        {
            return engine.ThrowTypeError("RegExpExec called on an object that is not a RegExp");
        }

        return RegExpExecute(engine, target, input);
    }

    /// <summary>
    /// The language's <c>Invoke(rx, key, « input »)</c> for the String methods' fallback: the
    /// fresh RegExp is asked for the Symbol like any other object, so a program's replacement of
    /// the method on <c>RegExp.prototype</c> is the one that answers.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D30BB8
    // Broiler-Human:        PENDING
    private static JsValue RegExpInvoke(
        JsEngine engine, RegExpObject pattern, JsSymbol key, string input)
    {
        var target = JsValue.Object(pattern);
        return engine.Call(engine.GetSymbol(target, key), target, [JsValue.String(input)]);
    }

    /// <summary>Reads one argument, which may not have been passed.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=6F7964
    // Broiler-Human:        PENDING
    private static JsValue ArgOfRegExp(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The body of <c>RegExp.escape</c>: <paramref name="text"/> rewritten, code point by code
    /// point, as pattern text that matches it literally.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These are the pinned specification's steps, written out rather than delegated: no platform
    /// regular-expression escaper is consulted, because none of them escapes the same set. A
    /// LEADING ASCII letter or digit becomes <c>\xHH</c>, so the answer placed after <c>\c</c>,
    /// <c>\0</c> or a back reference such as <c>\1</c> cannot extend that escape; every later code
    /// point goes through <see cref="RegExpEscapeCodePoint"/>. A surrogate pair is one code point
    /// and is copied; a lone surrogate is a code point of its own and is escaped.
    /// </para>
    /// <para>
    /// The charge is one unit per input code unit before any work and one per output code unit
    /// after it, and the answer is held to the same <c>StringLengthCeiling</c> that <c>repeat</c>
    /// and <c>padStart</c> keep - an escaped lone surrogate is six units for one, so the answer can
    /// be six times its argument.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=13C313
    // Broiler-Human:        PENDING
    private static string RegExpEscapeText(JsEngine engine, string text)
    {
        StringCharge(engine, text.Length);
        var builder = new System.Text.StringBuilder(text.Length + 8);

        for (var at = 0; at < text.Length; at++)
        {
            int codePoint = text[at];

            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length &&
                char.IsLowSurrogate(text[at + 1]))
            {
                codePoint = char.ConvertToUtf32(text[at], text[at + 1]);
                at++;
            }

            if (builder.Length == 0 && codePoint < 0x80 && char.IsAsciiLetterOrDigit((char)codePoint))
            {
                RegExpEscapeHex(builder, codePoint);
            }
            else
            {
                RegExpEscapeCodePoint(builder, codePoint);
            }

            if (builder.Length > StringLengthCeiling)
            {
                throw engine.Error("RangeError", "Invalid string length");
            }
        }

        StringCharge(engine, builder.Length);
        return builder.ToString();
    }

    /// <summary>
    /// The specification's <c>EncodeForRegExpEscape</c>: appends the pattern text matching
    /// <paramref name="codePoint"/> to <paramref name="builder"/>.
    /// </summary>
    /// <remarks>
    /// A SyntaxCharacter or <c>/</c> takes a backslash; the five characters with a ControlEscape
    /// take that letter; the other punctuators <c>,-=&lt;&gt;#&amp;!%:;@~'`"</c>, white space, line
    /// terminators and lone surrogates take <c>\xHH</c> up to 0xFF and <c>\uHHHH</c> per code unit
    /// past it; everything else is itself. The punctuators are escaped as hex rather than with a
    /// backslash because an identity escape of most of them is a SyntaxError under <c>u</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F4FC59
    // Broiler-Human:        PENDING
    private static void RegExpEscapeCodePoint(System.Text.StringBuilder builder, int codePoint)
    {
        switch (codePoint)
        {
            case '^' or '$' or '\\' or '.' or '*' or '+' or '?' or '(' or ')' or '[' or ']' or
                '{' or '}' or '|' or '/':
                builder.Append('\\').Append((char)codePoint);
                return;
            case '\t':
                builder.Append("\\t");
                return;
            case '\n':
                builder.Append("\\n");
                return;
            case '\u000B':
                builder.Append("\\v");
                return;
            case '\u000C':
                builder.Append("\\f");
                return;
            case '\r':
                builder.Append("\\r");
                return;
            case ',' or '-' or '=' or '<' or '>' or '#' or '&' or '!' or '%' or ':' or ';' or
                '@' or '~' or '\'' or '`' or '"':
                RegExpEscapeHex(builder, codePoint);
                return;
        }

        if (codePoint > 0xFFFF)
        {
            builder.Append(char.ConvertFromUtf32(codePoint));
            return;
        }

        var unit = (char)codePoint;

        // WhiteSpace is TAB, VT, FF, ZWNBSP and every Zs character; LineTerminator is LF, CR, LS
        // and PS. The ones with a ControlEscape were answered above.
        if (unit is '\uFEFF' or '\u2028' or '\u2029' || char.IsSurrogate(unit) ||
            System.Globalization.CharUnicodeInfo.GetUnicodeCategory(unit) ==
                System.Globalization.UnicodeCategory.SpaceSeparator)
        {
            RegExpEscapeHex(builder, codePoint);
            return;
        }

        builder.Append(unit);
    }

    /// <summary>
    /// Appends <c>\xHH</c> for a code point up to 0xFF and <c>\uHHHH</c> for a BMP code unit past
    /// it, in the lower-case hex <c>Number::toString</c> and <c>UnicodeEscape</c> produce.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F6C7B
    // Broiler-Human:        PENDING
    private static void RegExpEscapeHex(System.Text.StringBuilder builder, int codePoint)
    {
        var digits = codePoint <= 0xFF ? 2 : 4;
        builder.Append('\\').Append(digits == 2 ? 'x' : 'u');
        builder.Append(codePoint.ToString(
            digits == 2 ? "x2" : "x4", System.Globalization.CultureInfo.InvariantCulture));
    }

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

    /// <summary>
    /// The RegExp a String method's first argument stands for once it did not dispatch: the
    /// language's <c>RegExpCreate(value, flags)</c>.
    /// </summary>
    /// <remarks>
    /// A value that is not a RegExp is not an error: the specification builds one out of its String
    /// form, which is why <c>"a.b".match(".")</c> matches a character rather than a full stop. A
    /// RegExp that reaches here - one whose Symbol was removed - is converted the same way, so it
    /// is compiled from what its <c>toString</c> prints and not reused
    /// <i>(it was reused as itself until 2026-09-21)</i>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=48FB12
    // Broiler-Human:        PENDING
    private RegExpObject RegExpFromArgument(JsEngine engine, JsValue value, string flags) =>
        RegExpBuild(
            engine,
            value.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(value),
            flags);

    /// <summary>
    /// The body of the <c>RegExp</c> constructor once the call-form identity check has passed:
    /// the source and flags chosen, the instance allocated from the new target, the pattern
    /// compiled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Three sources for the pattern, in the language's order.</b> A real RegExp gives its own
    /// source and - when no flags were passed - its own flags, read from its slots with no property
    /// access. A RegExp-LIKE object, one IsRegExp accepted without the slots, is asked for its
    /// <c>source</c> property and then, when no flags were passed, its <c>flags</c> property.
    /// Anything else is the pattern text itself.
    /// </para>
    /// <para>
    /// <b><c>new.target.prototype</c> is read after those and before either is converted</b>, which
    /// is RegExpAlloc's place; the conversions, the flag check and the compile follow. A
    /// <paramref name="newTarget"/> that is not an object - the call form, an internal
    /// construction - answers <c>%RegExp.prototype%</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9C36B2
    // Broiler-Human:        PENDING
    private RegExpObject RegExpConstruct(
        JsEngine engine, JsValue pattern, JsValue flags, bool patternIsRegExp, JsValue newTarget)
    {
        JsValue source;
        JsValue chosen;

        if (pattern.AsObjectOrNull() is RegExpObject template)
        {
            source = JsValue.String(template.Source);
            chosen = flags.Type == JsType.Undefined ? JsValue.String(template.Flags) : flags;
        }
        else if (patternIsRegExp)
        {
            source = engine.GetProperty(pattern, "source");
            chosen = flags.Type == JsType.Undefined ? engine.GetProperty(pattern, "flags") : flags;
        }
        else
        {
            source = pattern;
            chosen = flags;
        }

        var prototype = BinaryPrototypeFrom(engine, newTarget, RegExpPrototype);

        var made = RegExpBuild(
            engine,
            source.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(source),
            chosen.Type == JsType.Undefined ? string.Empty : engine.ToStringValue(chosen));

        made.Prototype = prototype;
        return made;
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
    /// <c>lastIndex</c> is always read through <c>ToLength</c>, so its <c>valueOf</c> runs and may
    /// throw <i>(a pattern with neither flag skipped the read until 2026-09-21; JSeal follow-up
    /// VM-FIX-G)</i>. Neither <c>g</c> nor <c>y</c>: the search then starts at zero and
    /// <c>lastIndex</c> is not written. Either one: the search starts at <c>lastIndex</c>, a failure
    /// resets it to zero and a success sets it past the match. Getting the reset wrong is what makes
    /// a loop over <c>exec</c> either miss its second string or never end.
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
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=66030A
    // Broiler-Human:        PENDING
    private static JsRegExpMatch? RegExpMatchOne(
        JsEngine engine, RegExpObject target, string input)
    {
        engine.Charge(16);

        // The specification reads `lastIndex` through `ToLength` WHATEVER THE FLAGS, which CLAMPS a
        // negative to zero rather than failing on it: `re.lastIndex = -1` searches from the start,
        // and only a `lastIndex` past the end of the input ends the search before it begins. The
        // flags are consulted after the read, as RegExpBuiltinExec consults [[OriginalFlags]].
        var start = ArrayToLength(engine, target.LastIndex);
        var tracks = target.Global || target.Sticky;

        if (!tracks)
        {
            start = 0;
        }

        // UNDER `u` THE MATCH STARTS AT THE CODE POINT THAT HOLDS `lastIndex`, which is what
        // RegExpBuiltinExec's "the character that was obtained from element lastIndex of S" means
        // when `lastIndex` names the trailing half of a surrogate pair: the input is a list of code
        // points, and that element belongs to the pair's. Starting at the trailing unit let
        // `/\udf06/gu` match half of U+1D306 (JSeal slice JSD-0031-later). The match's start is
        // then the pair's first unit - the only start at which the matched substring and the
        // captures are code points, and the answer V8 gives - rather than the
        // mid-pair `lastIndex` the step writes into "index", which for an empty match would put the
        // start after the end that GetMatchString asserts it precedes.
        if (target.Matcher.Unicode &&
            start > 0 && start < input.Length &&
            char.IsLowSurrogate(input[(int)start]) &&
            char.IsHighSurrogate(input[(int)start - 1]))
        {
            start--;
        }

        var match = start > input.Length
            ? null
            : RegExpRun(engine, target, input, (int)start, target.Sticky);

        // THE TWO WRITES BELOW ARE THE SPECIFICATION'S `Set(R, "lastIndex", …, true)` AND THROW
        // WHERE THE PROPERTY IS CLOSED. `lastIndex` became closable when it gained an attribute
        // bit of its own, and a `g` pattern that had been frozen would otherwise go on advancing a
        // cursor it reports as non-writable. A pattern that tracks nothing never writes and is
        // unaffected, which is why `/x/.exec` on a frozen RegExp still works.
        if (tracks && !target.LastIndexWritable)
        {
            engine.ThrowTypeError("Cannot assign to read only property 'lastIndex'");
        }

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

    /// <summary><c>replace</c> with a string on the left, which replaces the first occurrence only.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=0B573D
    // Broiler-Human:        PENDING
    private static string RegExpReplaceText(
        JsEngine engine, string input, string search, JsValue replacement)
    {
        // A TEXT REPLACEMENT IS CONVERTED BEFORE THE SEARCH, so its `toString` runs once even when
        // nothing is found - the language's order.
        var callable = replacement.AsObjectOrNull() is { IsCallable: true };
        var converted = callable ? string.Empty : engine.ToStringValue(replacement);
        var at = input.IndexOf(search, System.StringComparison.Ordinal);

        if (at < 0)
        {
            return input;
        }

        string produced;

        if (callable)
        {
            produced = engine.ToStringValue(engine.Call(
                replacement,
                JsValue.Undefined,
                [JsValue.String(search), JsValue.Number(at), JsValue.String(input)]));
        }
        else
        {
            var template = converted;
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

    /// <summary>
    /// The language's <c>GetSubstitution</c>: a text replacement with its dollar substitutions
    /// expanded against one result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Everything comes from the result, not from a matcher.</b> <paramref name="captures"/> are
    /// the result's converted captures and <paramref name="namedCaptures"/> its <c>groups</c>
    /// (already through <c>ToObject</c>, or <c>undefined</c>), so a custom <c>exec</c>'s answer is
    /// expanded exactly as a built-in one is.
    /// </para>
    /// <para>
    /// <b>The digit rule is the pinned edition's.</b> <c>$nn</c> is taken as two digits first and
    /// falls back to one only when the two-digit number is past the capture count, so <c>$01</c>
    /// is the first capture, <c>$10</c> with one capture is the first capture then "0", and
    /// <c>$0</c> and <c>$00</c> stay literal. <c>$&lt;name&gt;</c> means something only when the
    /// result has <c>groups</c> and a later "&gt;" closes it; the name is then read with an ordinary
    /// <c>Get</c> and an <c>undefined</c> value expands to nothing. <c>$'</c> starts at the end of
    /// the matched text, clamped to the input, since a custom result may claim more text than is
    /// left. The template is charged once up front and each expansion as it is appended.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=061B45
    // Broiler-Human:        PENDING
    private static string RegExpSubstitute(
        JsEngine engine,
        string matched,
        string input,
        int position,
        System.Collections.Generic.List<JsValue> captures,
        JsValue namedCaptures,
        string template)
    {
        engine.Charge((ulong)template.Length + 1);
        var builder = new System.Text.StringBuilder(template.Length);
        var count = captures.Count;
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
                    RegExpAppend(engine, builder, matched);
                    at += 2;
                    continue;

                case '`':
                    RegExpAppend(engine, builder, input, 0, position);
                    at += 2;
                    continue;

                case '\'':
                    var tail = System.Math.Min(position + matched.Length, input.Length);
                    RegExpAppend(engine, builder, input, tail, input.Length - tail);
                    at += 2;
                    continue;

                case '<':
                    var close = template.IndexOf('>', at + 2);

                    if (close < 0 || namedCaptures.Type == JsType.Undefined)
                    {
                        builder.Append("$<");
                        at += 2;
                        continue;
                    }

                    var capture = engine.GetProperty(
                        namedCaptures, template.Substring(at + 2, close - at - 2));

                    if (capture.Type != JsType.Undefined)
                    {
                        RegExpAppend(engine, builder, engine.ToStringValue(capture));
                    }

                    at = close + 1;
                    continue;

                default:
                    break;
            }

            if (next is >= '0' and <= '9')
            {
                var digits = at + 2 < template.Length && template[at + 2] is >= '0' and <= '9' ? 2 : 1;
                var index = digits == 2 ? ((next - '0') * 10) + (template[at + 2] - '0') : next - '0';

                if (index > count && digits == 2)
                {
                    digits = 1;
                    index = next - '0';
                }

                if (index >= 1 && index <= count)
                {
                    var value = captures[index - 1];

                    if (value.Type != JsType.Undefined)
                    {
                        RegExpAppend(engine, builder, value.AsString());
                    }
                }
                else
                {
                    builder.Append(template, at, 1 + digits);
                }

                at += 1 + digits;
                continue;
            }

            builder.Append(character);
            at++;
        }

        return builder.ToString();
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

        /// <summary>Whether <c>lastIndex</c> may still be written.</summary>
        /// <remarks>
        /// <b>It is a bit of its own because the property is projected rather than stored</b>, and
        /// a projection has nowhere to keep an attribute. Without it
        /// <c>Object.defineProperty(re, "lastIndex", { writable: false })</c> returned having
        /// changed nothing, and the property went on reporting itself writable and going on being
        /// written - which is the same defect <see cref="JsArray"/> answers with
        /// <c>lengthWritable</c>, in the same shape, for the same reason.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=FB58E5
        // Broiler-Human:        PENDING
        private bool lastIndexWritable = true;

        /// <summary>Whether the <c>exec</c> protocol may still move the cursor.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=D46EC6
        // Broiler-Human:        PENDING
        internal bool LastIndexWritable => lastIndexWritable;

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=3C7D1D
        // Broiler-Human:        PENDING
        internal override int OwnPropertyCount => base.OwnPropertyCount + 1;

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=461B43
        // Broiler-Human:        PENDING
        internal override bool TryGetOwnProperty(string key, out JsProperty property)
        {
            if (string.Equals(key, "lastIndex", System.StringComparison.Ordinal))
            {
                property = JsProperty.Data(
                    LastIndex,
                    lastIndexWritable ? JsPropertyAttributes.Writable : JsPropertyAttributes.None);

                return true;
            }

            return base.TryGetOwnProperty(key, out property);
        }

        /// <inheritdoc/>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=4; Fingerprint=082176
        // Broiler-Human:        PENDING
        internal override void SetOwnProperty(string key, JsProperty property)
        {
            if (string.Equals(key, "lastIndex", System.StringComparison.Ordinal) &&
                !property.IsAccessor)
            {
                // A DEFINITION MAY CLOSE THE PROPERTY AND AN ASSIGNMENT MAY NOT, and both arrive
                // here - which is the same arrangement `JsArray` makes for `length`. The checked
                // path hands down whatever attributes the descriptor asked for, so losing
                // `Writable` is how a definition says so; an ordinary assignment carries the
                // attributes the property already had, which its caller read above.
                LastIndex = property.Value;
                lastIndexWritable = (property.Attributes & JsPropertyAttributes.Writable) != 0;
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
