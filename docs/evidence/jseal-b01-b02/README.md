# Internal BigInt value and gated exact literals: JSeal slices B01 and B02

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. The wide manifest
still refuses a BigInt literal by name, there is no `BigInt` global, no shipped composition admits
the gate, and nothing here is a BigInt conformance claim.

## What changed

- Decision record [JSD-0033](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0033-the-internal-bigint-value-and-the-gated-literal.md)
  (proposed, owner decision pending) sets the representation, the two-half gate, the constant and
  its version policy, the budgets, and what each operation answers or refuses until B03-B06. It is
  indexed in the decisions README.
- **B01, the value.** `JsBigInt.cs` (new, internal) wraps the base class library's `BigInteger`
  with the realm's value ceiling (2^20 bits), a word count for charges and the one constant
  decoding. `JsType.BigInt = 8`; `JsValue` keeps its 24-byte layout. Every exhaustive value-kind
  switch was audited and now answers or refuses a BigInt by name: `typeof`, `ToBoolean`, `===`,
  `SameValueZero` and Map/Set hashing (`JsValue.cs`, `JsCollections.cs`), `ToNumber` (the
  specification's `TypeError`), `ToString` (exact, charged on its digit bound), `ToObject`,
  primitive property lookup, `Describe`, loose equality against another type (`JsEngine.cs`),
  `JSON.stringify` (`JsRealm.Json.cs`), the host crossing (`JsHostRealm.cs`), the clone walk
  (`JsRealm.Clone.cs`) and the engine-less typed-array element write (`JsBinary.cs`). Before this,
  `ToNumber` and `ToString` would have recursed without end on such a value and three other sites
  would have cast it to an object.
- **B02, the literal.** The tokenizer refuses every spelling that cannot take the suffix (fraction,
  exponent, legacy octal, leading zero, second suffix, digit or identifier after it) as
  `2003 MalformedNumericLiteral` at the literal, gated or not. With the internal compile gate
  (`JsCompileRequest.AdmitsBigIntLiterals`, reached only through an `UnsafeAccessor` in the
  slice-compiler checks) a literal is parsed exactly in every radix and lowered to constant tag 7;
  one wider than 65,536 bits is refused at the literal. Without the gate the `2104` refusal is
  unchanged. A BigInt property key is now its exact decimal spelling in both modes (it was read
  through a double: `{ 9007199254740993n: 1 }` named `"9007199254740992"`).
- **Artifact.** Constant tag 7 (sign, length, little-endian magnitude; canonical; at most 8,192
  bytes). The verifier admits it only beside the gate surface `broiler.javascript.bigint`, which is
  known and deliberately absent from `JsSurfaces.All`, so `JavaScriptProfile.Descriptor` declines it,
  and every public descriptor door (`DescriptorAdmitting`, `DescriptorReEmittingWith`,
  `DescriptorHostingRealms`) refuses the name with an `ArgumentException`; only the internal
  `DescriptorAdmittingTheBigIntGate`, reached by the checks through an `UnsafeAccessor`, admits it.
  A well-formed tag-7 constant in an otherwise valid artifact without the declaration answers
  `1301 UnknownConstantTag`, the code older builds gave (a malformed or over-wide payload answers
  `1305` or `1201` first, and the `1301` is issued after every section is read); the numeric
  manifest refuses the tag with `1301` too. New code `1305 MalformedBigIntConstant` (registry
  revision 14); an over-wide payload is `1201`. `JsNativeCompiler`'s constant reader steps over the
  new tag. Format version unchanged.
- **Public API (deliberate, baseline regenerated):** `JsFormat.ConstantTag.BigInt`,
  `JsFormat.CeilingBigIntConstantBytes`, `JsSurfaces.BigInt`, `JsArtifactWriter.BigIntConstant`,
  `JavaScriptDiagnosticCode.MalformedBigIntConstant`. No host-surface (JSD-0024) API change; a
  BigInt a guest throws through a host call now reaches the host as a `JsHostThrowException`
  carrying a `TypeError` (`JsHostRealm.Thrown`), where it first escaped as the internal `JsThrow`.
- Tests and fixtures: `BigIntChecks.cs` (38 new `--checks` rows); three retained corpus entries
  `wide-a-bigint-constant-*`; differential probe `the-bigint-literal-boundary.js`; architecture
  counts (product files 191 to 192, core codes 68 to 69, corpus-reached rows 67 to 68, registry
  revision 13 to 14).

## Executed on Windows 11, .NET SDK 10.0.401, Release

- `dotnet build Broiler.VM.slnx -c Release`, `BROILER_ASSURANCE_WRITE=1 BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release --no-build`,
  rebuild, `dotnet test Broiler.VM.slnx -c Release --no-build`. Base: Contract 267/267,
  Architecture 256/256. After: Contract 267/267, Architecture 256/256.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: the pass count moves
  from 282 to 320, the two `not-run` x86-64-sysv rows unchanged, no row failing.
  [bigint-checks.txt](bigint-checks.txt) keeps the 38 BigInt rows and the summary line.
- The checks were written with the implementation, so they were not seen failing first against the
  base (the gate they open did not exist). Falsification instead ([mutations.txt](mutations.txt)):
  reading a gated literal through a double fails four rows; that same run also replaced
  mathematical BigInt equality with reference equality, which **no row detected**, because
  constants are interned and one value is one instance until B03 computes new ones (recorded in
  JSD-0033 section 5). The mutation was reverted and the build redone.
- `Broiler.VM.Composition.JavaScript.ExecutionOnly.exe --corpus src/tests/corpus/js-1`: all
  149 retained entries replay to their recorded answers, including the three new ones.
  `python eng/corpus-integrity.py --corpus src/tests/corpus/js-1 -- <that command>` mutated two
  entries, detected both, and left the corpus unchanged.
- Differential: `python eng/run-differential.py --only the-bigint-literal-boundary --against node --timeout 20`
  passes with one declared divergence (case 16: the wide manifest refuses `eval("9007199254740993n")`
  where Node answers a BigInt). The base build answered cases 11, 13 and 14 wrongly
  ([probe-before.txt](probe-before.txt): `"9007199254740992"`, `"1E+21"`, `undefined`). The whole
  lane (`python eng/run-differential.py --timeout 60`) changes nothing else; its only mismatch is
  the known retained case 36 of `the-later-library-methods`.
- Test262, pinned suite, bytecode form, subset `test/language/literals`,
  `test/language/expressions/typeof`, `test/language/expressions/object`, `test/built-ins/BigInt`
  ([test262-comparison.txt](test262-comparison.txt)): passed 3168 to 3190, failed 151 to 149,
  unsupported 148 to 128, exhausted 8 unchanged. The 20 variants leaving `unsupported` are the
  malformed-literal negative tests of `test/language/literals/bigint`, now the correct early
  SyntaxError; the two leaving `failed` are `literal-property-name-bigint.js`. No variant moved the
  other way. Every well-formed BigInt test stays `unsupported`.

## After review (same day)

An adversarial review found the verify half of the gate open to any embedder through the public
`DescriptorAdmitting`, contrary to JSD-0033's "closed to every public caller", and a BigInt thrown
through a host call escaping as an internal exception. Both are fixed and JSD-0033 is amended
([review-fixes.txt](review-fixes.txt)):

- `bigint/b01/a-thrown-bigint-reaches-the-host-as-a-host-throw` was written first and run red
  (`escaped:JsThrow`), then passed after the `JsHostRealm.Thrown` fix.
- `bigint/b01/every-public-descriptor-door-refuses-the-gate` fails (`admitted` at all four doors)
  when the refusal is disabled, and passes with it.
- The superlinear `ToString` cost is measured, not yet charged: a loop converting the widest
  constant exhausts the default allowance after about 5.0 s, a `String('x')` loop after about
  1.05 s (ratio about 4.7), recorded in JSD-0033 section 5 as B03's baseline.
- JSD-0033 section 2.5, the `JsFormat.ConstantTag.BigInt` remarks and this record now say which
  artifacts answer `1301`; the probe's cases 1-10 are marked as regression guards only (the base
  build already answered SyntaxError there).
- Re-run after the fixes: build, `BROILER_ASSURANCE_WRITE=1 dotnet test` (write mode), rebuild,
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  `--checks`: the pass count is 320, 2 not run, no row failing. Corpus replay: 149 entries replay
  to their recorded answers. The differential probe passes with its one declared divergence. The
  Test262 subset above, re-run into a fresh output directory, gives per-variant outcomes identical
  to the first after-run.

## Not exercised

- No whole-suite Test262 run: the retained floor (`atMost unsupported 1990`) is expected to hold
  with 20 fewer unsupported variants, but that figure was not measured.
- The native execution form (pre-existing instantiation fault on this machine), Native AOT
  publishing and the fuzz host.
- Mathematical equality between two distinct BigInt instances from guest code (section 5 of
  JSD-0033); arithmetic, comparison, conversion, the global and the host kind are cards B03-B06.
