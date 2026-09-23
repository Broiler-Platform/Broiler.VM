# JSP-6 RegExp protocols: JSeal follow-up VM-FIX-E

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the RegExp protocol defects left open by V04, F10 and F11; it does not mark JSP-6 complete.

## What changed

In `src/Broiler.VM.Profile.JavaScript/JsRealm.RegExp.cs`, plus one internal property in
`JsRealm.Iterator.cs`; no new opcode, diagnostic code, product file, public API or decision record.

1. **`String.prototype.matchAll` fallback.** After `RequireObjectCoercible(this)` an Object
   argument gets the IsRegExp-plus-`flags` check and the `Symbol.matchAll` dispatch as before; what
   does not dispatch is now `RegExpCreate(ToString(regexp), "g")` followed by
   `Invoke(rx, @@matchAll, S)`. The inlined iteration over an internal copy and the explicit
   TypeError refusal of a non-global RegExp that did not dispatch are gone.
2. **`RegExp.prototype[Symbol.matchAll]`.** Takes any Object receiver (TypeError otherwise), then
   `ToString(string)`, `SpeciesConstructor(R, %RegExp%)`, `ToString(Get(R, "flags"))`,
   `Construct(C, [R, flags])`, `ToLength(Get(R, "lastIndex"))` and a throwing `Set` on the
   matcher. The RegExp String Iterator steps through a new `RegExpExec` (a callable `exec` is
   called and must answer an Object or null; otherwise only a real RegExp runs the built-in exec);
   an empty global match reads `lastIndex` with ToLength, advances it (by code point under `u`/`v`)
   and writes it back with a throwing `Set`; a non-global matcher answers one match and is done.
   The iterator keeps the pinned `[[Done]]` semantics rather than the generator states the other
   built-in iterators share: only a `null` match or a non-global matcher's one match retires it, so
   a step that throws (from `exec`, from `match[0]`, or from the `lastIndex` read/write) leaves it
   to call `exec` again, and a custom `exec` that re-enters `next` is answered rather than refused
   (`JsBuiltinIterator.Resumable`, honoured by the shared `BuiltinIteratorNext`). The up-front
   charge of the input length the old walk took is kept in `RegExp.prototype[Symbol.matchAll]`.
3. **`RegExp` constructor.** Uses the existing `RegExpIsRegExp`. The call form returns its argument
   only when IsRegExp is true, flags are undefined and `pattern.constructor` is `RegExp` itself. A
   RegExp-like object (IsRegExp true, no matcher slot) is read for `source` and, when flags are
   undefined, `flags`. `new.target.prototype` is read after those and before conversion
   (RegExpAlloc's place); the constructor now sets `BuildsFromNewTarget` so the engine does not
   read it a second time.
4. **Primitive patterns.** `match`, `replace`, `search` and `split` ask only an Object pattern for
   its well-known Symbol (`RegExpDispatch`). `match` and `search` build their fallback RegExp with
   RegExpCreate from ToString of the argument and Invoke its Symbol. A RegExp whose Symbol was
   removed now takes the text path in `replace`/`split` and is compiled from its printed form in
   `match`/`search`/`matchAll`, as the specification says. Two small order fixes in the same text
   paths: `split` converts the separator before a zero limit answers, and `replace` converts a
   non-callable replacement before searching.

## Executed on Windows

- Build `dotnet build Broiler.VM.slnx -c Release`; assurance write run
  `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release --no-build`; rebuild; gate
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256
  (base before the change: Contract 267/267, Architecture 256/256).
- New differential probe `src/tests/differential/the-regexp-protocols.js` (35 cases). Cases 1-31
  were written first and run against the base binaries: 24 of those 31 answers differed from the
  retained answers ([probe-before.txt](probe-before.txt)). Cases 32-35 (throw then retry, and
  re-entry from a custom `exec`) were added after review; with the generator latch and re-entry
  guard still applied to this iterator all four differed (`{"done":true}:1`, `TypeError`,
  `TypeError`, `TypeError`). After the change all 35 agree with Node v24.17.0
  except the four declared divergences 24-27: the pinned Test262 `cstm-*-on-*-primitive` cases
  require that a primitive pattern is not asked for its Symbol, while Node still consults the
  primitive's prototype. `python eng/run-differential.py --only the-regexp-protocols --against node
  --timeout 20`: only the four declared divergences are reported.
- Full retained lane `python eng/run-differential.py --timeout 20`: only the pre-existing
  `the-later-library-methods` case 36 (cube root spelling) differs; the V04 probe
  `the-regexp-guards-in-string-search` keeps its retained answers unchanged.
- Pinned Test262 (`ccaac100`), bytecode form, `--dir test/built-ins/RegExp --dir
  test/built-ins/RegExpStringIteratorPrototype --dir test/built-ins/String/prototype/{match,
  matchAll,replace,replaceAll,search,split}`. The base run used a copy of the base binaries
  (`--binary-directory`). Passing variants, before -> after (15 skipped variants excluded; they
  claim the proposed `regexp-duplicate-named-groups`):

  | Directory | Variants | Before | After |
  |---|---|---|---|
  | RegExp (other than Symbol.matchAll) | 3678 | 2002 | 2020 |
  | RegExp/prototype/Symbol.matchAll | 52 | 26 | 52 |
  | RegExpStringIteratorPrototype | 34 | 20 | 34 |
  | String/prototype/match | 98 | 86 | 94 |
  | String/prototype/matchAll | 50 | 36 | 46 |
  | String/prototype/replace | 108 | 94 | 104 |
  | String/prototype/replaceAll | 90 | 88 | 88 |
  | String/prototype/search | 86 | 70 | 80 |
  | String/prototype/split | 240 | 230 | 238 |
  | total | 4436 | 2652 (1752 fail, 32 unsupported) | 2756 (1648 fail, 32 unsupported) |

  104 variants move from failing to passing and none from passing to failing
  ([test262-fixed.txt](test262-fixed.txt)). The two iterator directories move from 46/86
  (40 failing at this slice's base) to 86/86. Test262 does not exercise the iterator's
  throw-then-retry or re-entry behaviour; a re-run after the `[[Done]]` change
  (`--out D:/wt/t262/vm-regexp-protocols/after-review`) gives the same after figures, with no
  variant changing state.
- Wider regression check `--dir test/built-ins/String --dir test/built-ins/Symbol --dir
  test/annexB/built-ins/RegExp` (2724 variants), base -> this patch: the pass count moves from 2493
  to 2539, no variant moves from passing to failing.

## Not exercised or still failing

- The native form was not exercised: on this host it cannot instantiate
  (ProfileFault/UnsatisfiedHostAssumption, pre-existing). The changed built-ins have no separate
  native implementation.
- Still failing in the selection, outside this change: `*-v-u-flag` / `*-v-flag` variants of
  match, matchAll, replace and search (Unicode property escapes and the `v` flag are refused by
  the matcher), `RegExp/proto-from-ctor-realm.js` (no nested realm), and deep-nesting and
  case-folding RegExp cases.
- `RegExp.prototype[Symbol.match]`, `[Symbol.replace]`, `[Symbol.search]` and `[Symbol.split]`
  still require a real RegExp receiver and run the matcher directly rather than through
  RegExpExec, and do not read `flags`/species; a custom `exec` is observed only through
  `matchAll`. The built-in `exec` still reads `lastIndex` only for a global or sticky pattern.
  These are recorded follow-ups, not part of this slice.
