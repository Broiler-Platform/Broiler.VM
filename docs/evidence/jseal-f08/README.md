# String.prototype.normalize on the Unicode 17.0.0 tables: JSeal slice F08 (JSD-0031 U3)

Date: 2026-09-22. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Decision
[JSD-0031](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md)
is still proposed; its new closing section "U3 (F08) as built" records what this slice built. No
new decision number, opcode or diagnostic code was taken, no product file was added (the count in
`ReviewRecordRuleTests` stays 199), and no public API moved.

Checkout: a detached worktree at Broiler.VM `484f389` plus the merged waves 1-6 patch
(`base-vm-w7`) plus the U2 patch (`vm-unicode-u2`: archive, generator, tables, rules N22/N23).
Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 (Unicode 17.0) was a comparison
engine only; the pinned specification, `NormalizationTest.txt` and Test262 `ccaac100` are the oracle.

## What changed

- **`normalize`** (`src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs`): the ASCII-only refusal
  (a `TypeError` for any non-ASCII string, JSC-91) is replaced by `NormalizeText`, UAX #15 over the
  U2 lookups: full decomposition, canonical ordering, and canonical composition with blocking for
  NFC/NFKC; Hangul algorithmic. Validation is unchanged: receiver `ToString`, then the form
  (`"NFC"` when undefined), then a `RangeError` for anything but the four names. A lone surrogate is
  a class-0 code point with no mapping and passes through. No platform normalizer anywhere (N23 is
  green; the new method is named so N23's token rule does not see it).
- **Cost and bounds.** A quick check (canonical order plus `NFC_QC`/`NFKC_QC`, or "has a
  decomposition" for NFD/NFKD) returns the input unchanged without allocating; ASCII is answered a
  unit at a time, which is the kept ASCII fast path. Otherwise a counting pass measures the full
  decomposition, refuses one longer than the realm's `StringLengthCeiling` (2^24 UTF-16 units, the
  ceiling `repeat` uses) with `RangeError: Invalid string length`, and charges `StringCharge` for the
  storage and the three passes before the buffer exists. Canonical ordering is a stable counting
  sort per run of non-starters (linear in a guest-chosen run of marks).
- **Generated conformance probes.** `UnicodeNormalizationProbes` (architecture test project),
  called from `UnicodeTableGenerator.Generate`, writes 18 probes
  `src/tests/differential/the-unicode-normalization-*.js` from the verified archive in the same
  write-or-check function as the tables, so rule N22 holds them byte for byte: 9 vector probes
  (parts 0, 2, 3, 4, 5 whole; part 1 in four slices of at most 4,500 vectors; all 20,034 vectors,
  20 conformance checks each) and 9 invariant probes over spans of 0x20000 code points (part 1's
  claim that every code point not in its `c1` column is unchanged; surrogate code points as lone
  units). Parts 4 and 5 go beyond the parts 0-3 the acceptance names. Each prints a count as case 1
  and one case per failing vector or code point, ASCII only. The N22 gate test now also asserts
  20,034 generated vectors and 9 invariant probes.
- **Focused probe** `src/tests/differential/the-string-normalization.js` (56 cases): coercion order,
  `RangeError`s, Symbol receivers/forms, canonical ordering, compatibility and Hangul forms,
  supplementary characters, lone surrogates, U+FDFA repeated, long mark runs, `'e\u0301'.normalize('NFC') === '\u00e9'`.
- **Retained answers.** `the-general-surface` case 48 (`"\u00e9".normalize("NFD").length`) moves
  from `TypeError` to `2`, and its `#diverges node 48` declaration (JSC-91) is removed as stale.
- **Documentation.** JSD-0031 closing section; `JsUnicodeNormalization.cs` remarks; N23's comment;
  the differential README table; `THIRD_PARTY_NOTICES.md` names the generated probes as test files
  carrying Unicode data (not shipped). `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and
  `assurance.manifest.json` were regenerated through J5 (write mode). The profile ledgers
  (`roadmap.parity.md` section 4.3, `roadmap.status.md`, JSC-91 in `roadmap.corrections.md`) still
  describe the refusal; they were not edited (ledger rule) and need the proposed status text.

## Failing first

- Focused probe on the base build: 41 of 56 cases differed from Node, each a `TypeError` where Node answered
  ([focused-before-and-node.txt](focused-before-and-node.txt)); after the change the host's answers
  equal Node's line for line.
- Generated probes on a pre-change CLI build (`D:/wt/vm-b07-b08-basebin/cli`, whose `normalize` is the
  same refusal): `the-unicode-normalization-part0` and `-invariants-000000` exit 1 with
  "uncaught TypeError: String.prototype.normalize is implemented for ASCII only".

## Commands and results

- `dotnet build Broiler.VM.slnx -c Release`; `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build`;
  rebuild; `dotnet test Broiler.VM.slnx -c Release --no-build`: base (after regenerating the
  assurance artefacts the U2 patch leaves out) Contract 267/267, Architecture 266/266; after
  Contract 267/267, Architecture 266/266. Build: 0 warnings, 0 errors.
- Probes generated with `BROILER_UNICODE_WRITE=1 dotnet test src/tests/Broiler.VM.Architecture.Tests
  -c Release --no-build --filter FullyQualifiedName~UnicodePinRuleTests`, then gate mode green.
- `SliceCompiler.exe --checks`: 371 checks and 2 not-run rows, before and after.
  `python eng/run-cli-acceptance.py`: 225/225 before and after. `ExecutionOnly.exe --corpus
  src/tests/corpus/js-1`: 22 checks before and after (lowering unchanged; corpus not regenerated).
  `Cli.exe --host-surface`: every check passed, 73 rows before and after.
- `python eng/run-differential.py --only <probe> --against node` for each of the 19 new probes:
  all agree with Node; retained answers were then written with `--write` and reconverted to LF.
  Timings: [probe-timings.txt](probe-timings.txt). **Slowest probe: 1.33 s**
  (`the-unicode-normalization-invariants-0C0000.js`) in the full run, against the 30-second
  default timeout, CLI host default limits.
- Full lane `python eng/run-differential.py --against node`: 72 probes. Every new probe and
  `the-general-surface` pass. Still failing, all pre-existing and unchanged by this slice (the same
  failure counts on the pre-change binary): `the-later-library-methods` case 36 (cube root, the
  known console code-page issue), `the-json-date-and-regexp-surface` cases 30/34 (Node prints a
  localized time-zone name on this machine), `the-seam-between-generators-and-the-rest` (Node exits
  1), `the-settling-of-promises` (four stale Node declarations).

## Test262 (pinned, bytecode form, `--shards 1 --jobs 1`)

`--dir test/built-ins/String/prototype/normalize --dir test/staging/sm/String` (61 files, 122
variants): pass count moves from 100 to 110. The 14 `normalize` files and the 4 staging files
(`normalize-form-non-atom`, `normalize-generic`, `normalize-parameter`, `normalize-rope`) are 36
variants: 26 passing before, all 36 after. The per-variant comparison changes exactly those 10
variants (`return-normalized-string*.js` x3, `normalize-generic`, `normalize-rope`, both modes)
and nothing else. The remaining 12 failing variants are other `sm/String` files, unchanged.

## Adversarial checks (CLI host, default limits)

`'\uFDFA'.repeat(100000).normalize('NFKC').length` is 1,800,000 in about 0.3 s;
`repeat(900000)` in NFKD ends with the fuel allowance exhausted before any buffer is allocated;
`repeat(1000000)` is `RangeError: Invalid string length`; `'a' + '\u0316\u0301'.repeat(200000)`
in NFC takes about 40 ms.

## Not exercised, and not done

- The native execution form: not exercised (pre-existing ProfileFault/UnsatisfiedHostAssumption).
- Native AOT publish of the CLI host: not run for this slice (U2 ran it for the tables).
- Test262 beyond the two named directories was not run; no broader before/after comparison exists.
- The Android composition was not built.
- The Broiler.JS comparison engine was not run.
- `Broiler.Unicode` (suggested by the owner for F08/F09 because Broiler.JS uses it with Unicode 17
  data) was not used: it has no decomposition, combining-class or composition data, so it cannot
  back `normalize`, and JSD-0031 section 4(b) and U2's section 9.3 record why it is not the source.
  U2 used it as an independent cross-check of the property tables.
- Signatures: the release owner's co-signature on the notices entry and every owner signature
  remain pending; nothing here is accepted.
