# JSP-7 RegExp.escape: JSeal slice F10

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers `RegExp.escape` only; it does not mark JSP-7 or JSW-6 complete.

## What changed

- `RegExp.escape` (length 1, writable, non-enumerable, configurable, not a constructor) is defined
  on the `RegExp` constructor in `JsRealm.RegExp.cs`. It follows the pinned ES2026 steps
  (`RegExp.escape` and `EncodeForRegExpEscape`) directly and uses no platform regex escaper:
  - a non-String argument is a TypeError with no coercion (a String wrapper and an object with a
    `toString` are refused, and that `toString` is not called);
  - a leading ASCII letter or digit becomes `\xHH`;
  - a SyntaxCharacter or `/` takes a backslash;
  - TAB, LF, VT, FF and CR become `\t \n \v \f \r`;
  - the other punctuators ``,-=<>#&!%:;@~'`"``, other white space (ZWNBSP and every Zs), LS, PS
    and lone surrogates become `\xHH` up to 0xFF and `\uHHHH` per code unit past it;
  - every other code point, including a surrogate pair, is copied unchanged.
- Fuel: one unit per input code unit before the work and one per output code unit after it. The
  answer is held to the realm's existing 2^24 string ceiling (RangeError past it), as `repeat`
  and `padStart` are. An escaped lone surrogate is six units for one.

## Executed on Windows

- New differential probe `src/tests/differential/the-regexp-escape.js` (36 cases): shape and
  descriptor, each escape class, lone surrogates and pairs, non-String arguments, `new`, the
  literal round trip of all 128 ASCII characters with and without `u`, a character-class
  embedding, and the answer placed after `\c`, `\0`, a back reference `\1`, `\x` and `{`.
  Before the change the host answered 29 of the 36 cases differently from Node v24.17.0
  (`RegExp.escape` was `undefined`; [probe-before.txt](probe-before.txt)). After it all 36 agree
  with Node and with the retained answers:
  `python eng/run-differential.py --only the-regexp-escape --against node --timeout 20`.
  Case 14 prints the surrogate pair as code units because this machine's console code page
  mangles non-ASCII output.
- Pinned Test262 (`ccaac100`), bytecode form, `--shards 1 --jobs 1`. `RegExp.escape` is in the
  suite's standard feature section, so its tests are selected and run rather than skipped.

  - `test/built-ins/RegExp/escape` (40 variants): the pass count moves from 0 to 38; the two
    remaining failures are the cross-realm variants listed below.
  - `test/built-ins/RegExp` (3743 variants): the pass count moves from 1986 to 2024.

  Both `test/built-ins/RegExp` runs report 20 unsupported and 13 skipped. Comparing per variant,
  38 moved Failed -> Passed, all under `RegExp/escape`; none moved Passed -> Failed. The "before"
  `RegExp` run used the build of the unchanged base (HEAD plus the wave-1 patch).
- `dotnet test Broiler.VM.slnx -c Release --no-build` on the base: Contract 267/267,
  Architecture 256/256. After the change, the assurance write run and a rebuild: Contract
  267/267, Architecture 256/256.

## Not exercised or still failing

- `RegExp/escape/cross-realm.js` (2 variants) fails because the harness's `$262.createRealm`
  answers "this profile creates no nested realm"; that is pre-existing and not specific to
  `RegExp.escape`.
- The native execution form was not exercised: it cannot instantiate on this host
  (ProfileFault/UnsatisfiedHostAssumption, pre-existing). `RegExp.escape` has no separate native
  implementation.
- The full retained differential lane was not re-run; only the new probe was.
