# JSP-6 RegExp Symbol methods: JSeal follow-up VM-FIX-G

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the RegExp protocol defects VM-FIX-E left open (see
[jseal-regexp-protocols](../jseal-regexp-protocols/README.md)) and the V04 leftover; it does not mark
JSP-6 complete.

## What changed

Only `src/Broiler.VM.Profile.JavaScript/JsRealm.RegExp.cs`; no new opcode, diagnostic code, product
file, public API or decision record.

1. **`RegExp.prototype[Symbol.match]`, `[Symbol.replace]`, `[Symbol.search]`, `[Symbol.split]`**
   take any Object receiver (TypeError otherwise) and follow the pinned algorithms: `ToString`
   of the argument, `ToString(Get(rx, "flags"))` for globality and code point stepping, every match
   through RegExpExec (a callable `exec` answers and must return an Object or `null`), empty
   matches stepped past with `ToLength(Get(rx, "lastIndex"))` and a throwing `Set`.
   `search` saves, zeroes and restores `lastIndex` under the `SameValue` tests and answers the
   result's `index`. `replace` collects results, then reads each as an object (length, `0`,
   `index` clamped, captures, `groups`) and expands text replacements with a new GetSubstitution
   (`RegExpSubstitute`) using the pinned two-digit rule and `$<name>` against `ToObject(groups)`;
   the answer is charged per piece and held to the String length ceiling. `split` constructs its
   splitter with `SpeciesConstructor(rx, %RegExp%)` and `flags` plus "y", sets `lastIndex` per
   position and reads the separator end back through `ToLength`.
2. **`RegExp.prototype.flags`** reads `hasIndices`, `global`, `ignoreCase`, `multiline`, `dotAll`,
   `unicode`, `unicodeSets` and `sticky` from any Object receiver, in that order (it answered a
   real RegExp's internal flags before). No getter answers `unicodeSets`; the matcher still refuses
   `v`.
3. **The built-in `exec`** (RegExpBuiltinExec) reads `lastIndex` through `ToLength` for every
   pattern, so a `valueOf` runs (and may throw) even without `g`/`y`. **`test`** takes any Object
   and goes through RegExpExec.
4. **Unobservable fast paths.** When RegExpExec is certain to be the built-in one (a RegExp of this
   realm, prototype `%RegExp.prototype%`, no own `exec`, the prototype's `exec` the intrinsic as a
   data property), `replace` keeps matcher results instead of building and re-reading Arrays, and
   `split` (additionally: sticky, matching unicode flag, writable `lastIndex`) walks the splitter
   with the matcher's forward search. No guest code can run between those attempts. On a
   200000-match workload (`"abc,".repeat(200000)`) the base took split 490 ms, replace 494 ms,
   match 417 ms; this change takes 263, 308 and 345 ms (single run each, same machine; not a
   benchmark claim). Without the replace fast path the replace took 1427 ms.
5. **V04 leftover.** `super[Symbol.replace]` inside a class extending RegExp was already fixed at
   this base by VM-FIX-B: `replaceAll/searchValue-replacer-RegExp-call*.js` pass before and after;
   probe cases 42-43 keep it covered.

Matcher fuel charging is unchanged; each Symbol-method loop iteration and each capture read
charges fuel, as `apply`'s argument list does.

## Executed on Windows

- Base (HEAD plus `base-vm-w4.patch`): `dotnet build Broiler.VM.slnx -c Release`;
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  After: build, `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release --no-build`,
  rebuild, gate `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267,
  Architecture 256/256.
- Slice compiler `--checks`: 327 passed, 2 not run, before and after. CLI acceptance
  `python eng/run-cli-acceptance.py --binary-directory src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0`:
  219 of 219 command lines as declared. `--host-surface`: every check passed.
- New probe `src/tests/differential/the-regexp-symbol-methods.js` (45 cases), written first. Run
  against the base binaries, 26 of the 45 answers differ from the retained answers (cases 5-16, 18,
  23, 25, 26, 31-33, 35-40, 45; [probe-before.txt](probe-before.txt)).
  `python eng/run-differential.py --only the-regexp-symbol-methods --against node --timeout 20`:
  only the four declared divergences (8, 13, 15, 16) differ from Node v24.17.0. Node answers
  those as though it read `global` rather than `flags`, and reads `sticky` before `unicodeSets`;
  the pinned text decides them (`sec-regexp.prototype-%symbol.replace%` and
  `sec-get-regexp.prototype.flags`; Test262 `Symbol.replace/get-flags-err.js`).
- Full retained lane `python eng/run-differential.py --timeout 20`: only the pre-existing
  `the-later-library-methods` case 36 (cube root spelling) differs.
- Pinned Test262 (`ccaac100`), bytecode form, `--dir test/built-ins/RegExp --dir
  test/built-ins/RegExpStringIteratorPrototype --dir test/built-ins/String/prototype/{match,
  matchAll,replace,replaceAll,search,split}`, base binaries via `--binary-directory`. Passing
  variants, before -> after (15 skipped variants excluded; they claim the proposed
  `regexp-duplicate-named-groups`):

  | Directory | Variants | Before | After |
  |---|---|---|---|
  | RegExp/prototype/Symbol.match | 106 | 70 | 106 |
  | RegExp/prototype/Symbol.replace | 138 | 66 | 136 |
  | RegExp/prototype/Symbol.search | 46 | 24 | 46 |
  | RegExp/prototype/Symbol.split | 88 | 38 | 86 |
  | RegExp/prototype/exec | 154 | 148 | 152 |
  | RegExp/prototype/test | 90 | 90 | 90 |
  | RegExp/prototype/flags | 32 | 10 | 28 |
  | RegExp (other) | 3076 | 1626 | 1630 |
  | RegExpStringIteratorPrototype | 34 | 34 | 34 |
  | String/prototype/{match,matchAll,replace,replaceAll,search,split} | 672 | 650 | 650 |
  | total | 4436 | 2756 (1648 fail, 32 unsupported) | 2958 (1446 fail, 32 unsupported) |

  202 variants move from failing to passing and none from passing to failing
  ([test262-fixed.txt](test262-fixed.txt)).
- Wider regression check `--dir test/built-ins/String --dir test/built-ins/Symbol --dir
  test/annexB/built-ins/RegExp --dir test/annexB/built-ins/String` (2952 variants): the pass count
  stays at 2581 before and after, no variant changes state (the four `exhausted` annexB
  leading/trailing-escape variants are the same before and after).

## Not exercised or still failing

- The native form was not exercised: on this host it cannot instantiate
  (ProfileFault/UnsatisfiedHostAssumption, pre-existing). The changed built-ins have no separate
  native implementation.
- Still failing in the selection, outside this change: `Symbol.replace/named-groups.js` (a group
  name outside the BMP is refused by the pattern parser), `Symbol.split/splitter-proto-from-ctor-realm.js`
  (no nested realm), `exec/regexp-builtin-exec-v-u-flag.js`, `flags/this-val-regexp.js` and the
  String `*-v-flag`/`*-v-u-flag` cases (no `v` flag), and the `*-bigint-primitive` and
  `flags/this-val-non-obj.js` variants (BigInt literals unsupported by this manifest).
- RegExp results still carry no `indices` for `d`; `Symbol.split`'s fast path relies on the
  matcher's forward search stepping exactly as AdvanceStringIndex does, which the probe and the
  `Symbol.split` Test262 directory cover but no mutation test was run for.
