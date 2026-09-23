# BigInt conversion, comparison and public admission: JSeal slice B05

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Decision
[JSD-0033](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0033-the-internal-bigint-value-and-the-gated-literal.md)
is still proposed and unsigned; this slice amends it as section 7. Nothing here advances a milestone.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-4 patch
(`base-vm-w5`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was a diagnostic
comparison only; the pinned specification and the pinned Test262
(`test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`) are the oracle.

## What changed

- **Admission** (`JsSurfaces.cs`, `JavaScriptProfile.cs`, `JsCompiler.cs`). The already-minted
  identity `broiler.javascript.bigint` joins `JsSurfaces.All`, so the descriptor admitting every
  surface admits it and `JavaScriptProfile.BigIntManifest` (new, public) lets a composition decline
  it. An artifact declares it for a BigInt constant or a read of the `BigInt` global
  (`JsSurfaces.BigIntGlobals`, new, public). The wide manifest lowers BigInt literals; the numeric
  manifest keeps its named refusal. The internal gate doors (`AdmittingBigIntLiterals`,
  `DescriptorAdmittingTheBigIntGate`) are removed and the public descriptor doors no longer refuse
  the name. The reasons for this option rather than a separate advertised manifest are JSD-0033
  section 7.1.
- **The global** (`JsRealm.BigInt.cs`, new product file): `BigInt(v)` (`NumberToBigInt` for an
  integral Number, else `ToBigInt`), `new BigInt` a TypeError, `asIntN`/`asUintN` (`ToIndex`, then
  `ToBigInt`, then B04's operations), `BigInt.prototype.toString(radix)`, `toLocaleString` (the
  decimal text; JSD-0027's FIXED row, amended), `valueOf`, and `@@toStringTag`. Built only when the
  composition admits the surface.
- **Conversions and comparisons** (`JsEngine.cs`, `JsBigInt.cs`). `ToBigInt`, `StringToBigInt`
  (split, charged decimal parsing; linear radix-prefixed parsing; a value past the ceiling reported
  rather than refused), `NumberToBigInt`, `Number(x)` rounding to nearest with ties to even (the
  base class library's conversion truncates, so it is not used), exact `==` and relational
  comparison against Numbers and Strings, BigInt objects through `ToObject`, property access
  through `BigInt.prototype`, `JSON.stringify`'s `toJSON` lookup and TypeError, and the structured
  clone refusing a BigInt object by name. `ToNumber` of a BigInt stays the specification's
  TypeError. One adjacent fix in the same statement: `==` now converts an object beside a Symbol
  too, as the specification lists (two Test262 variants in `expressions/equals`).
- **Update expressions** (`JsOpcode.cs`, `JsCompiler.cs`, `JsEngine.cs`, `JsBaselineHandlers.cs`):
  three opcodes, `ToNumeric` `0xB0`, `Increment` `0xB1`, `Decrement` `0xB2`, in format version 2;
  every wide `++`/`--` uses them; the numeric manifest's bytes are unchanged; the retained corpus
  regenerates byte for byte. No new diagnostic code; no new decision number (JSD-0033 amended).
- **Inventories.** `docs/realm/globals.txt` regenerated (`BigInt` added; the `--globals` root now
  admits the BigInt surface as it admits the others). The absent-globals block rule N17 reads lost
  its `BigInt` line - **a one-line edit to the status ledger's machine-readable block**, the only
  way N17 stays true; its surrounding prose is not edited. Public API baseline: five members added.
  Product-file count 192 to 193. The CLI's help paragraph on BigInt is corrected with a dated note.
  JSD-0027 and JSD-0032 carry dated amendments. The wide floor's `atMost unsupported` row is
  re-based by hand from 1990 to 0 with a `retired` line (JSD-0033 section 7.7).

## Executed

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_API_WRITE=1` and `BROILER_ASSURANCE_WRITE=1`
  `dotnet test ... --no-build`; rebuild; `dotnet test Broiler.VM.slnx -c Release --no-build`. Base:
  Contract 267/267, Architecture 256/256. After: Contract 267/267, Architecture 256/256.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: the pass count moves
  from 353 to 363, the two `not-run` x86-64-sysv rows unchanged, no row failing
  ([bigint-checks.txt](bigint-checks.txt)). The B01-B04 rows run on the public path now; 14 rows
  are B05's.
- **Failing first** ([probe-before.txt](probe-before.txt)): the new probe is refused whole on the
  base build (2104 at its first literal), and every B05 operation reached through `eval` is a
  SyntaxError there. The new check rows name `JavaScriptProfile.BigIntManifest`, which the base does
  not have, so they could not be run against the base product code.
- **Falsification** (by hand, restored and rebuilt after): `Number(x)` through the base class
  library's conversion answers `9007199254740994` for `Number(2n ** 53n + 3n)` and
  `Number.MAX_VALUE` for `Number(2n ** 1024n - 2n ** 970n)`; comparing through a double answers
  `9007199254740993n > 9007199254740992` false and `9007199254740993n == 9007199254740992` true.
  Run through the end-user host, the programs of the `bigint/b05/number-of-...`,
  `relational-...` and `loose-equality-...` rows change their answers under each mutation;
  probe case 61 was added afterwards so the probe carries the rounding cases too.
- CLI acceptance: 224 of 224 command lines answered as declared, before and after. Host-surface
  lane: every check passed. Corpus replay (`ExecutionOnly --corpus src/tests/corpus/js-1`): 22
  checks passed; `SliceCompiler --write` into a scratch directory reproduces the retained corpus
  byte for byte, so it was not rewritten. The polyglot host (`Broiler.VM.Composition.PolyglotCli`)
  prints `3` for `print(1n + 2n)` under its default manifest and refuses the literal with 2104
  under `--numeric`.
- Differential: new probe `the-bigint-public-surface.js` (62 cases) agrees with Node except the
  declared case 60 (the B07-B08 absences). `the-bigint-literal-boundary` (case 16) and
  `the-bigint-arithmetic-boundary` (cases 21-24) now answer `"bigint"` as Node does: their retained
  answers were rewritten and their stale declarations removed. The whole lane otherwise fails
  exactly where the base build fails (Node's German Date names, a Node exception in
  `the-seam-between-generators-and-the-rest`, stale declarations in `the-settling-of-promises`, and
  the known code-page case in `the-later-library-methods`).
- Timings (Release, this machine): `BigInt` of a 315,000-digit text about 215 ms with charges
  between steps (one base-class-library parse of 315,653 digits: about 258 ms uninterrupted);
  `toString(7)` of the widest value about 381 ms; `toString(16)` about 10 ms.

## Test262

Whole pinned suite, bytecode form, four shards, base build and this build
([test262-comparison.txt](test262-comparison.txt)): unsupported moves from 1970 (one family, "a
BigInt literal") to 0; no variant that passed on the base fails now; 93 variants move from failing
to passing and 663 from unsupported to passing. The whole `built-ins/BigInt` directory moves from
none of its 154 variants passing (68 failed, 86 unsupported) to 152 of 154 (the two failures need a
second realm). Over the 1,501 files declaring the BigInt feature, the 152 `built-ins/BigInt`
variants among them all pass but the same two,
the equality, relational, `typeof` and literal subtrees pass whole, and the other operators pass
but for the two `BigUint64Array` subclass variants. What still fails is named in the comparison:
the BigInt typed arrays (B07), the DataView accessors (B08), shared memory (JSD-0028), `Intl` and
`Temporal`, `$262.createRealm`, `Error.isError`, the JSON.parse source-text reviver argument and the
`Array.prototype.toString` fallback - none of them B05's. The re-based floor holds for this run
and is crossed by the base run.

## Review fixes (same date)

An adversarial review asked for four corrections, all made in this build:

- Records outside the profile that still said BigInt was refused: `docs/compositions.md` (the
  JavaScript host's and the polyglot host's rows), `README.md` and `docs/support.md` (the wide run's
  empty `unsupported` column), and the polyglot host's help text and remarks, each with a dated
  correction quoting what it replaced. No profile figure was added to a core document.
- A new check row, `bigint/b05/a-declining-composition-refuses-bigint-through-eval`: under a
  composition admitting the dynamic surface and declining BigInt, `eval('1n')`, `eval('BigInt(1)')`,
  an indirect `eval('2n')` and `Function('return 3n')()` each throw a catchable `EvalError` naming
  `UnsupportedFeatureManifest`; under the full descriptor they answer the four BigInts. The row
  names only public members that exist on the base too, but under the base's full descriptor a
  BigInt literal is refused at compile time, so its admitting half cannot pass there; it is a new
  pin rather than a regression caught failing first.
- JSD-0033 section 7.8 gives the probe's 62 cases; section 7 item 1 records the `eval` route.
- The `built-ins/BigInt` figure is given for the whole directory as well as for the BigInt-feature
  selection.

## Not exercised

- The native execution form and its floor (`test262-wide-native.floor`, unchanged): the form cannot
  instantiate on this machine. The three new opcodes have baseline entry points and template-scan
  classes, checked by `--checks`, but no native run.
- The BigInt TypedArray and DataView tests beyond recording that they fail (cards B07-B08), the host
  crossing and clone of BigInt (B06), and any cross-realm behaviour.
