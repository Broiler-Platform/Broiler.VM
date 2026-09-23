# The Unicode archive, generator, rules and tables: JSeal slice F07, U1 archived and U2

Date: 2026-09-22. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Decision
[JSD-0031](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md)
is still proposed; its new section 9 records U1, the owner's licence and size decisions as given in
conversation (unsigned), and what U2 built. No new decision number was taken. No guest-visible
behaviour changes: `normalize` still refuses non-ASCII input and `\p{...}` is still a SyntaxError
under `u`; those are slices U3 (F08) and U4 (F09), which build on this one.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-6 patch
(`base-vm-w7`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 (Unicode 17.0) was a
comparison engine only; the pinned specification and Test262 `ccaac100` are the oracle.

## What changed

- **Archive (U1's files).** Thirteen UCD 17.0.0 files and the Unicode licence text, copied from the
  first of two byte-identical retrievals ([`retrieval.txt`](retrieval.txt) keeps both copies' hashes
  and the comparison), under `src/tests/unicode/pins/` beside a new
  `unicode.pin` (version, sources, retrieval date, per-file bytes and SHA-256, `archived yes`) and a
  README that says why the location follows the conformance and Octane pins. Both are declared
  `binary` in `.gitattributes`. The three ECMAScript property tables are in
  `src/Broiler.VM.Profile.JavaScript/docs/specification/`, recorded in its README under the Ecma
  notice; `JavaScriptLanguageEdition.DocumentDigest` is unchanged.
- **Notices.** `THIRD_PARTY_NOTICES.md` gains the Unicode entry the owner described (data and
  derived tables under Unicode-3.0, code Apache-2.0; files, version, assemblies, packages and
  composition images named, Android included), the full licence text verbatim - which is how it
  travels, because `eng/Broiler.Packaging.props` packs this file into every package - and a dated
  correction of the stale "the profile packs nothing" reason. The composition images do not carry
  the notice yet; that is recorded as open.
- **Generator** (`src/tests/Broiler.VM.Architecture.Tests/`): `UnicodePin.cs`, `UnicodeDatabase.cs`,
  `UnicodeTableGenerator.cs`. One function: `BROILER_UNICODE_WRITE=1` writes, otherwise the N22 test
  regenerates in memory and asserts byte identity.
- **Tables and lookups** (all internal): `JsUnicodeProperties.g.cs`/`.cs` and
  `JsUnicodeCaseFolding.g.cs`/`.cs` in the Format assembly; `JsUnicodeNormalization.g.cs`/`.cs` in
  the profile. The API is listed in JSD-0031 section 9.3.
- **Rules.** N22 (the pin and the tables) and N23 (no platform normalizer in profile product
  source), with witnesses under `witnesses/register/`; the register row counts move from 96 to 98.
  The product-file count in `ReviewRecordRuleTests` moves from 193 to 199. One J5 test assertion
  that the product used the per-unit `EXEMPT=` hatch nowhere now states where it is used (JSD-0031
  section 9.3 explains why); no assurance rule's statement changed. `CODE-ASSURANCE.md`,
  `HUMAN_REVIEW.md` and `assurance.manifest.json` were regenerated through J5.

## Measured

- **Table data: 236,721 bytes** (the generator's figure, equal to a Roslyn count of the byte
  literals in the three files), under the owner's 300 KB cap (307,200 bytes); JSD-0031 section 6
  estimated about 200 KB. Per file: properties 147,331, case folding 12,096, normalization 77,294.
- Release assemblies: `Broiler.VM.Profile.JavaScript.Format.dll` 133,120 -> 297,472 bytes,
  `Broiler.VM.Profile.JavaScript.dll` 602,624 -> 684,032 bytes (base built in a second detached
  worktree from the same base).
- Generation of all three files, in the N22 test: about four seconds, most of it the first gate
  test's parse of the archive.

## Commands and results

- `dotnet build Broiler.VM.slnx -c Release` then `dotnet test Broiler.VM.slnx -c Release --no-build`:
  base Contract 267/267, Architecture 256/256. After `BROILER_ASSURANCE_WRITE=1 dotnet test ...`
  and a rebuild: Contract 267/267, Architecture 266/266 (ten new tests: eight N22, two N23).
- **Failing first.** The first gate run with the rules in place and the notices and README not
  yet written failed eight architecture tests: `N22_The_Readme_And_The_Notices_Carry_What_The_Pin_Records`,
  and the assurance, register and review-record tests that a new product file moves (J3, J5, J7,
  J12, H3, H4 and the assurance generator test). The first write run of the generator itself
  refused its input, because `PropertyValueAliases.txt` lists `Ahom ; Ahom` and a name selecting two
  entries is a stop; same-name-same-value is now one entry. Rule mutations
  ([rule-mutations.txt](rule-mutations.txt)): a one-byte hand edit to a generated table fails N22,
  and a `"x".Normalize()` call in a profile source fails N23; both were reverted.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks`: 371 checks and 2 not-run rows,
  before and after. `python eng/run-cli-acceptance.py`: 225/225 before and after.
  `Broiler.VM.Composition.JavaScript.ExecutionOnly.exe --corpus src/tests/corpus/js-1`: 22 checks
  before and after; the lowering did not change and the corpus was not regenerated.
  `Broiler.VM.Composition.JavaScript.Cli.exe --host-surface`: every check passed, 73 rows before
  and after.
- **No network.** `dotnet restore Broiler.VM.slnx`, then `dotnet build Broiler.VM.slnx -c Release
  --no-restore --no-incremental --disable-build-servers` with `HTTP_PROXY`, `HTTPS_PROXY` and
  `ALL_PROXY` pointing at a closed local port: no errors and no warnings. NuGet is not invoked with
  `--no-restore`, and the tables are committed source, so nothing in the build reads the archive
  or a network.
- **Native AOT.** `dotnet publish src/compositions/Broiler.VM.Composition.JavaScript.Cli -c Release
  -r win-x64 -p:PublishAot=true`, with the Visual Studio installer directory on `PATH` for
  `vswhere` as earlier bundles on this machine needed: exit 0, no warning of any kind in the
  output (the project treats warnings as errors). The published image answers a small probe the same
  way as the IL host: `'abc'.normalize('NFC')` is `abc`, `'é'.normalize('NFC')` a TypeError,
  `new RegExp('\\p{Letter}', 'u')` a SyntaxError, `/K/iu.test('k')` true.

## Test262 (pinned, bytecode form, `--shards 1 --jobs 1`)

`test/built-ins/RegExp` and `test/built-ins/String/prototype/normalize`, before (base build) and
after (this change): 1,893 files and 3,771 variants each time; the pass count stays at 2,318, with
13 skipped, none unsupported or exhausted, and the per-variant comparison of the two reports finds
no variant that changed. Of the variants still failing, 900 are under `property-escapes` and 6 under
`normalize`: they are exercised (the suite's `regexp-unicode-property-escapes` feature is not
treated as proposed) and are U4's and U3's targets.

## Table validation beyond the rules

A scratch console program compiled the six product files together with `Broiler.Unicode`'s
generated property data and ran against the archive, with a Node script computing Node's own sets
([table-validation.txt](table-validation.txt); sources retained as
[harness-Program.cs.txt](harness-Program.cs.txt), [harness-node-sets.js.txt](harness-node-sets.js.txt)
and [harness-uharness.csproj.txt](harness-uharness.csproj.txt)):

- a reference UAX #15 normalizer over the U3 lookups reproduces every column of all 20,034
  `NormalizationTest.txt` lines in all four forms, and every code point outside its part 1 is left
  unchanged; every `NFC_QC=No` or `NFKC_QC=No` code point changes when normalized alone; no lone
  surrogate has a class, a decomposition, a composition or a non-Yes quick check;
- 1,722 name forms (`\p{X}`, `gc=`, `General_Category=`, `sc=`, `Script=`, `scx=`,
  `Script_Extensions=` over every value and alias) resolve to exactly Node's sets, except eight:
  the `Hrkt`/`Katakana_Or_Hiragana` forms, which the specification admits because
  `PropertyValueAliases.txt` lists them and V8 refuses. Of 30 misspellings, 29 are refused by both;
  `WSpace` is refused here and accepted by V8, and the specification's table does not admit it;
- 9,049 case-folding pairs agree with Node's `/x/iu` and `/[x]/iu`;
- **`Broiler.Unicode`** - which the owner suggested on 2026-09-22 because Broiler.JS uses it with
  Unicode 17 data: 946 of 950 set comparisons against its `Broiler.Unicode.Properties` data agree,
  none differs, and the other four are the `Hrkt` forms it has no entry for. JSD-0031 section 4(b)
  still rules it out as the source (rules A2 and N1, loose name matching, no normalization or
  folding data, unpinned inputs); it is used here as an independent check.

## Not exercised, and not done

- The native execution form: not exercised (it cannot instantiate on this machine, pre-existing
  ProfileFault/UnsatisfiedHostAssumption); nothing here has a native path.
- The Android composition was not built; it carries the tables in its image by construction.
- No differential probe was added and the differential lane was not re-run: no guest-visible answer
  changed, and the tables have no guest-reachable consumer until U3 and U4.
- The release owner's co-signature on the notices entry; how a composition image carries the
  notice; any signature on the owner's decisions. All pending.
- U3 and U4 themselves, including U3's generated `NormalizationTest.txt` probes, which belong in
  `UnicodeTableGenerator` and were not written.
