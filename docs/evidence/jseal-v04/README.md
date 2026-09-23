# JSP-6 string methods: JSeal slice V04

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the RegExp guards in the String search methods; it does not mark JSP-6 complete.

## What changed

- `startsWith`, `endsWith` and `includes` run the language's IsRegExp (`Symbol.match` read first,
  then the RegExp brand) after `RequireObjectCoercible(this)`/`ToString(this)` and before
  `ToString(search)`, and throw a TypeError for a regular expression. A RegExp with
  `Symbol.match = false` (or `null`) is searched for as a string; an object with a truthy
  `Symbol.match` is refused; a throwing `Symbol.match` getter propagates.
- `replaceAll` is redefined beside `replace` in `JsRealm.RegExp.cs`: receiver check, then for a
  non-nullish pattern IsRegExp plus a READ of `flags` (`RequireObjectCoercible`, `ToString`,
  TypeError without "g"), then `Symbol.replace` dispatch for an Object pattern, then the existing
  string path. Before this change it never consulted `Symbol.replace`, so
  `"aaa".replaceAll(/a/g, "b")` answered `"aaa"`.
- `matchAll`'s up-front rejection uses the same IsRegExp-plus-`flags` check instead of the internal
  global flag, and its `Symbol.matchAll` dispatch asks only Object patterns.
- Review fix: that change let a non-global RegExp with `Symbol.match = false` reach
  `RegExp.prototype[Symbol.matchAll]`, whose iterator never reported done for a non-global
  pattern (a defect already on main), so `Array.from("aa".matchAll(r))` ran until the allowance
  ran out where main had thrown a TypeError. The RegExp String Iterator now answers one match and
  then done for a non-global pattern, as `%RegExpStringIteratorPrototype%.next` says. A non-global
  RegExp that does not answer `Symbol.matchAll` is still refused with a TypeError (see below).

## Executed on Windows

- Differential probe `src/tests/differential/the-regexp-guards-in-string-search.js` (41 cases).
  Before the fix the host answered 21 of its first 34 cases differently from Node
  ([probe-before.txt](probe-before.txt); cases 35-41 were added after). Cases 37-41 (non-global
  RegExp String Iterator) were added at review: before the iterator fix case 37 did not settle
  ("AllowanceExhausted on WallClock"); after it they answer as Node does. After the fix all 41
  agree with Node except the two declared divergences 35/36: the pinned Test262
  (`cstm-replaceall-on-number-primitive.js`, `cstm-matchall-on-number-primitive.js`) requires
  that a primitive pattern is not asked for `Symbol.replace`/`Symbol.matchAll`; Node v24.17.0
  still consults `Number.prototype`.
- Full retained lane (`python eng/run-differential.py --timeout 10`, console code page 65001): only
  the pre-existing `the-later-library-methods` case 36 (cube root spelling) fails. Under the
  default code page 850 two further probes fail on non-ASCII output encoding, unrelated to this
  change.
- Pinned Test262 (`ccaac100`), bytecode form, per directory, passing variants before -> after:

  | Directory | Variants | Before | After |
  |---|---|---|---|
  | startsWith | 42 | 38 | 42 |
  | endsWith | 54 | 50 | 54 |
  | includes | 54 | 50 | 54 |
  | replaceAll | 90 | 66 | 84 |
  | matchAll | 50 | 30 | 36 |
  | total | 290 | 234 (52 fail, 4 unsupported) | 270 (16 fail, 4 unsupported) |

  No variant that passed before fails after. The iterator fix left these 290 unchanged.
- Iterator directories (`RegExp/prototype/Symbol.matchAll`, `RegExpStringIteratorPrototype`,
  86 variants), before -> after the iterator fix: the pass count moves from 40 to 42
  (`RegExpStringIteratorPrototype/next/next-iteration.js`, both modes), no regressions.
- Wider selection `test/built-ins/String`, `RegExp/prototype`, `RegExpStringIteratorPrototype`
  (3445 variants), HEAD -> this patch: the pass count moves from 2911 to 2949, 38 Failed -> Passed, 0 Passed ->
  Failed.
- `dotnet test Broiler.VM.slnx -c Release --no-build` (after the assurance write run and a
  rebuild): Contract 267/267, Architecture 256/256.

## Not exercised or still failing

- The native form could not be measured here: every variant fails with "the artifact would not
  instantiate: ProfileFault/UnsatisfiedHostAssumption" on this host. The changed built-ins have
  no separate native implementation.
- `replaceAll/searchValue-replacer-RegExp-call{,-fn}.js` (4 variants) fail on
  `super[Symbol.replace]` inside a class method ("Cannot convert a Symbol value to a string"),
  a super-property defect outside this slice.
- `matchAll` still inlines its fallback instead of `RegExpCreate(regexp, "g")` followed by
  `Invoke(rx, @@matchAll, S)` (10 variants), and `regexp-prototype-matchAll-v-u-flag.js` needs
  Unicode property escapes (excluded). Until that fallback exists, a non-global RegExp that does
  not answer `Symbol.matchAll` (including one with `Symbol.match = false`) is refused with a
  TypeError instead of being re-created with "g".
- The two iterator directories still fail 44 of 86 variants, mostly in
  `RegExp.prototype[Symbol.matchAll]` (IsRegExp and species handling of its receiver, the `flags`
  read); only the non-global termination was fixed here.
- `match`, `replace`, `search` and `split` still ask primitive patterns for their Symbols; their
  Test262 subsets were not run.
