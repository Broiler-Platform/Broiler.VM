<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSeal slice JSD-0031-later - the Unicode lexical grammar, group names, non-`u` Canonicalize

**Date:** 2026-09-22. **Status:** local implementation validation, not accepted milestone evidence.
Decision record: [JSD-0031 section 12](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md#12-2026-09-22-the-later-separately-items-performed).
Base: Broiler.VM `HEAD` (484f389) plus `base-vm-w10.patch`; Windows 11, .NET SDK 10.0.401,
Release; comparison engine Node v24.17.0 (Unicode 17.0).

## What changed

- **Tokenizer (`SliceTokenizer`)**: identifier characters are classified by code point against the
  pinned Unicode 17.0.0 `ID_Start` / `ID_Continue` (plus `$`, `_`, ZWNJ, ZWJ), astral ones
  included; escaped identifier characters are classified the same way (they were not classified
  at all), including a private name's first character; `WhiteSpace` is TAB, VT, FF, ZWNBSP and
  `Zs`, so U+0085 and U+001C-U+001F are no longer separators. Escaped reserved words stay refused
  by the parser, as before.
- **Regular expressions**: group names read the same sets; the non-`u` Canonicalize is
  `UnicodeData.txt`'s simple upper case with the specification's two rules (generated table plus a
  generated reverse index; the platform call and the lazily built dictionary are gone); under `u`,
  `lastIndex` inside a surrogate pair starts the match at the pair; a literal's flags are checked
  early (unknown, repeated, `u`+`v` are SyntaxErrors) and a well-formed `v` is an early SyntaxError
  naming the flag as unsupported.
- **Generator** (`UnicodeTableGenerator`, `UnicodeDatabase`, rule N22): three set-id constants and
  two tables (`UpperData`, `UpperOrbitIndex`), plus two new input checks (`ID_Start`/`ID_Continue`
  against their UAX #31 derivation; simple upper case against `Changes_When_Uppercased`). Table
  data: 236,721 -> 243,897 bytes (cap 307,200). N22's table count 17 -> 19.
- **Public API**: one new public type in the Format assembly, `JsUnicodeLexical`
  (`IsIdentifierStart`, `IsIdentifierPart`, `IsWhiteSpace`), recorded in
  `src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt` (rule N10). Product files 199 -> 200.
  No opcode, diagnostic code, rule or decision number was taken.

## Commands and results

- `dotnet build Broiler.VM.slnx -c Release`, then `dotnet test Broiler.VM.slnx -c Release
  --no-build`: base Contract 267/267, Architecture 266/266. After the change: the Unicode tables
  regenerated with `BROILER_UNICODE_WRITE=1` (N22 test), the API baseline with
  `BROILER_API_WRITE=1`, the assurance artefacts with `BROILER_ASSURANCE_WRITE=1`, a rebuild, and
  the gate run: Contract 267/267, Architecture 266/266.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks`: 371 checks and 2 not-run rows,
  before and after. `python eng/run-cli-acceptance.py`: 225/225 after (the figure recorded for
  the base of this wave is 225/225; it was not re-run on the base here).
  `Broiler.VM.Composition.JavaScript.ExecutionOnly.exe --corpus src/tests/corpus/js-1`: 22 checks
  after; the lowering of the corpus did not change and the corpus was not regenerated.
  `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passed.
- **Failing first**: the new probe `src/tests/differential/the-unicode-lexical-grammar.js` (95
  cases) answers 42 cases differently on the base build, each of them where the base was wrong
  (escaped `!`, digit, emoji and surrogate identifiers admitted; astral and Unicode 15.1-17.0
  identifiers refused; U+0085 a separator; `/a/x`, `/a/gg`, `/a/v` behind `false` accepted; group
  names U+A7CE and U+30FB refused; `/\uA7CE/i` missing U+A7CF; `lastIndex` mid-pair matching half
  a pair). `python eng/run-differential.py --only the-unicode-lexical-grammar` against the base
  binaries fails; see [probe-and-checks.txt](probe-and-checks.txt), which lists base, this change
  and Node case by case.
- `python eng/run-differential.py --only the-unicode-lexical-grammar --against node --timeout 20`:
  four declared divergences (54, 55 and 61: the `v` flag this realm refuses; 85: U+1F80, below), no
  finding. The whole differential lane against the retained answers: only the known code-page
  case 36 of `the-later-library-methods` (cube root, pre-existing) differed.

## Test262 (pinned, bytecode form, `--shards 1 --jobs 1`)

`test/language/identifiers`, `test/language/white-space`, `test/language/line-terminators`,
`test/language/literals/regexp`, `test/built-ins/RegExp`, `test/annexB/built-ins/RegExp`: 2,555
files and 5,067 variants each time. The pass count moves from 4,274 to 4,489, failures from 740 to
526, exhausted 13 to 12 (one property-escape file finishing inside the wall clock), unsupported 0
and skipped 40 (`legacy-regexp`, `regexp-duplicate-named-groups`, proposed features) both times.
No variant that passed before fails after. Of the 214 variants moving from failed to passed, 156
are this slice's: all 128 previously failing `language/identifiers` variants (every
`start-unicode-*`/`part-unicode-*` file, 5.2.0 to 17.0.0, and the escaped ZWJ/ZWNJ and vertical
tilde files), 10 `white-space` and 8 `line-terminators` files that forbid an escaped separator in
an identifier, `early-err-bad-flag.js` and `early-err-dup-flag.js`, and 6 `named-groups` files. The
other 58 are negative `v`-flag variants that now pass because a `v` literal is an early
SyntaxError: that is the refusal, not `v` support. Per variant: [test262-summary.txt](test262-summary.txt).

An intermediate build refused a `v` literal as `ConstructOutsideManifest`; its run of the same
selection ended with 356 `unsupported` variants (pass 4,405), which would cross the whole-suite
floor's `atMost unsupported 0` in `src/tests/conformance/floors/test262-wide.floor`. The floor was
not changed; the refusal became the SyntaxError above.

## Known limits, stated

- **The 27 ypogegrammeni letters** (U+1F80-U+1F87, U+1F90-U+1F97, U+1FA0-U+1FA7, U+1FB3, U+1FC3,
  U+1FF3): the specification's non-`u` Canonicalize upper-cases with the full mapping, which gives
  them two code points, so they stay themselves; the simple mapping used here gives their
  title-case partner, as the platform's did before. A scratch comparison of `UnicodeData.txt`'s
  simple upper case with Node's full `toUpperCase`, over every BMP code unit and under the two
  rules, found exactly these 27. `SpecialCasing.txt` is not archived.
- **`index` for a mid-pair `lastIndex` under `u`** is the pair's first unit, not the literal
  `lastIndex` the pinned text writes (which, for an empty match, contradicts GetMatchString's
  assertion). Node answers the same.

## Not exercised, and not done

- The native execution form: not exercised (it cannot instantiate on this machine, pre-existing).
- The Native AOT publish of the end-user host and the Android composition: not run.
- Test262 outside the six subtrees above: not run; no whole-suite run was taken.
- Out of scope and unchanged: full and special case mapping for `toUpperCase`/`toLowerCase`; the
  `v` flag and properties of strings; `String.prototype.trim`'s whitespace set
  (`JsNumberFormat.IsWhiteSpace`) and the parser's template-substitution skip scanner.
