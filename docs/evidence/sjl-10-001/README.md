<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-10-001 - the Temporal core: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-10 of the shared JavaScript libraries programme,
[`sjl-10-temporal.md`](../../javascript-shared-libraries/milestones/sjl-10-temporal.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-10.1 before any code of the milestone
existed.

**Records:** [JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md),
whose input is [`correspondence.md`](correspondence.md), with every probe and each engine's answers in
[`correspondence-runs/`](correspondence-runs/): `core/` for records and limits, balancing, durations and
differences, and `syntax/` for the grammar, the calendars and serialization.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules, correspondence, the base, the vectors and the cut-over, 2026-10-18, at SJL-10.4. **Nothing here is a
verdict.**
- The base of all three parts was taken at the commit that carries the rule, `99c9c9f`, before any code of SJL-10
  landed (section 2): part A's by the hosted workflow, parts B and C locally.
- The engine-level vectors ran on this family's pin at corpus revision 25, which holds the Temporal area (section 3).
- SJL-10.4's cut-over is in place, with the checks it ran (section 4).
- The after runs are SJL-10.6's.

*(Revised 2026-10-18, at SJL-10.2, when the pin moved to revision 25. The state read "rules, correspondence and the
base", and had no item on the vectors.)* *(Revised 2026-10-18, at SJL-10.4. The state read "rules, correspondence,
the base and the vectors, 2026-10-18, at SJL-10.2", and its last item "The after runs follow SJL-10.4's cut-over.")*

Broiler.JS's clauses and the home's are judged in Broiler.JS's bundle of the same name. This bundle reads none of
its runs.

## 1. SJL-10's area rule and its correspondence (SJL-10.1)

**The correspondence.**
- It was read from code in both engines at the revisions it names, in two halves on 2026-10-18, merged the same day.
  It numbers every difference the area verdict classifies: R for records and limits, B for balancing, D for
  durations, F for differences, G for the grammar, C for the calendars and S for serialization.
- Every behavioural cell its rows rest on was run in both engines: this family through the conformance root, and
  Broiler.JS through its shell. The probes of records, balancing, durations and differences also ran under the
  proposal's polyfill at `e8cc03fc`.
- **Every difference is classified.** Each is one of these:
  - a Broiler.JS correction, the milestone's listed ones and those found beyond them, each by row;
  - the one place found where the pinned text and its polyfill disagree, where the text rules (row D8, decision D7);
  - code no input reaches, deleted with its file.
- **This family has one correction**, row R6: three constructors that end the invocation on a year above 2^53. It is
  made at SJL-10.4 (decision D9).

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0078 records decisions D1 to D9 under it. It changes no programme rule: nothing is accepted without
review, and SJL-10 may not meet its gate before SJL-6 and SJL-8 meet theirs.

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It admits no
  class C2, C3 or C5: this family's charging, its proposal revision, its test262 pin and its data do not change. Its
  only named correction is R6, and any other it finds is admitted only as a finding the correspondence missed. A
  wall-clock race is admitted under C4's (c') and (e).
- **Part B** holds this family's Temporal reference datasets under `src/tests/temporal/` unchanged:
  - the five programs, run by the slice compiler's checks at both commits;
  - their programs, answers and divergences, byte for byte.
- **Part C** holds the wide twins to their base figures, with no class. The command-line root reaches none of the
  area's code, so these twins show only that no charge moved outside it.

**The proposal revision both engines follow** is `tc39/proposal-temporal@e8cc03fc`, the one this family has followed
since JSD-0053, and the area's rules run test262 `ccaac100` (decisions D1 and D2).

## 2. The base (SJL-10.2)

[`collect-area.py`](collect-area.py) took it. It reuses bundle `sjl-8-001`'s area collector, and through it bundle
`sjl-7-001`'s, bundle `sjl-6-001`'s import of a hosted run and bundle `sjl-5-001`'s twin bisection, over the wide
twins. Part B's step is bundle `sjl-8-001`'s, over the six checks this rule names and the one folder it holds byte
for byte, `src/tests/temporal/`. It judges nothing.

| | |
|---|---|
| Commit | `99c9c9f`, the commit that carries the rule, from a clean detached checkout built in Release there ([`base/identity.txt`](base/identity.txt)). Against the default branch after SJL-8's merges, `047dcec`, it changes no file outside the documents, so its product sources are `047dcec`'s, as the rule says |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), `sha256=80dc0723...` in the identity |
| Taken | 2026-10-18: part A's base by the workflow from 11:36Z to 11:46Z; parts B and C locally from 11:42Z, on the programme's local machine (`linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv`), while other work ran beside it. Nothing in parts B and C is timed |

**Why a fresh checkout.** A Release build of `14072b0` was at hand, whose product sources are `99c9c9f`'s. The rule
takes parts B and C "from a clean checkout of the same commit", the commit that carries it, and voids a part taken
at any other, so the base was built in a new detached worktree of `99c9c9f` instead.

**Part A's base.** [39123950609](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39123950609),
attempt 1, was dispatched from `main` at `047dcec` on 2026-10-18 at 11:36:57Z. It ran with both forms and the commit
`99c9c9f`, named by all forty digits, and completed at 11:46:23Z ([`base/hosted/run.txt`](base/hosted/run.txt)).
- **The workflow and its driver at the dispatched ref are byte-identical to the rule commit's.**
  `git diff --stat 047dcec 99c9c9f -- .github eng` prints nothing. The workflow
  `.github/workflows/test262-evidence.yml`, its driver `eng/test262-evidence.py`, and the `eng/run-test262.py`,
  `eng/collect-js-release-candidate.py` and `eng/compare-test262-forms.py` the rule names have the same blob at both
  commits.
- **The artifact** `test262-evidence` is retained unchanged in [`base/hosted/`](base/hosted/), imported by the
  collector's `base-import` step. [`base/hosted/imported.txt`](base/hosted/imported.txt) records the digest of the
  zip as downloaded, which is the digest GitHub records.
- **Every runner and the merging job** ran image `ubuntu24` 20261012.334.1.

| Run | Complete ([`base/completeness.log`](base/completeness.log)) | Totals |
|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants |
| `wide-native` | exit 4, a re-base, as the rule admits; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants |

The pass, fail, exhausted and skipped totals are bundle `sjl-8-001`'s after run's, figure for figure. Each form's
failure manifest lists the same rows with the same verdicts as that run's. The exhausted rows split differently
between the wall clock and fuel (and, in one row of each form, the nested-load fan-out), as hosted runs do: 9
bytecode rows and 13 native rows are exhausted on another dimension than in that run. Part A's comparison
reads them row by row, and its class C4 (c') covers that split. No base row is judged: the rule compares the base
with the after run at SJL-10.4's cut-over.

**Part B's base** ([`base/datasets/`](base/datasets/)).
- **The method:** the slice compiler, built in Release at the base commit, ran `--checks --verbose`.
  - [`checks.log`](base/datasets/checks.log) keeps its whole output, with the digest of the binary.
  - [`named-checks.txt`](base/datasets/named-checks.txt) keeps the line of each check the rule names.
  - [`files.sha256`](base/datasets/files.sha256) keeps the digest of every file under `src/tests/temporal/`, 22
    files.
- **The result:** every named check passes, each with the line count and divergences of bundle `sjl-8-001`'s base
  and after runs, and the 22 digests are that bundle's base's for the same files.

| Check | Lines |
|---|---|
| `temporal/t1/temporal-matches-the-reference-polyfill` | 1,629, none as divergences |
| `temporal/t2/intl-over-temporal-matches-the-reference-polyfill` | 1,233, 82 as divergences |
| `temporal/t3/calendars-match-the-reference-polyfill` | 3,999, 70 as divergences |
| `temporal/t3/calendar-conversions-match-icu4x` | 2,055 conversions |
| `temporal/t4/calendar-formats-match-the-reference-polyfill` | 5,951, 1,395 as divergences |
| `temporal/t5/zone-names-match-the-reference-polyfill` | 2,713, 275 as divergences |

**Four checks no rule names failed in that run, and passed when it was run again (a finding).** The slice
compiler exited 1 ([`checks.log`](base/datasets/checks.log)): `value/differential/binary`,
`value-flat/differential/binary`, `value/differential/bindings-and-the-stack` and
`value-flat/differential/bindings-and-the-stack` each reported the same first disagreement between bytecode and the
value form. It is operation 0 of each program, an addition, over `NaN` and `nan(0x7ff80000, 1)`: bytecode answered
`7ff80000.1`, the second operand's payload, and the value form `fff80000.0`.
- **Run again**, the same binary, by its SHA-256, from the same tree after part C, passed all 649 checks
  ([`checks-again.log`](base/datasets/checks-again.log), retained by hand). Bundle `sjl-8-001`'s base and after runs
  passed them too, at commits whose product sources are this one's.
- **What it suggests.** Which payload an addition of two NaNs keeps follows the operand order of the machine
  instruction. A result that moves between runs of one binary therefore points at the order the .NET JIT gives an
  addition on one side, which can differ between its tiers. A passing row prints no value, so which side moved is
  not recorded, and nothing further was run.
- **What it is not.** None of the four is a part B check, and the six part B names passed in both runs. It is
  recorded against those checks, which hold NaN payloads bit for bit, for whoever owns them.

**Part C's base** ([`base/fuel-parity.log`](base/fuel-parity.log)).
- **The method:** each wide twin's smallest completing fuel allowance was bisected under the bytecode form. Then
  both forms ran at that figure and at one less.
- **The result:** every twin completes at its figure and is refused on fuel at one less, in both forms, and both
  forms complete with the same answer.
- **Every figure and answer equals bundle `sjl-8-001`'s base.** The rule takes every wide twin; the command-line
  root carries no Intl data, so none reaches Temporal.

| Twin | Figure | Answer |
|---|---|---|
| `wide/array-loop` | 73,082 | 999000 |
| `wide/call-loop` | 28,026 | 1000 |
| `wide/closure-loop` | 31,044 | 1000 |
| `wide/fib` | 7,682 | 144 |
| `wide/generator-loop` | 40,075 | 499500 |
| `wide/numeric-loop` | 23,022 | 498497 |
| `wide/property-loop` | 29,029 | 183125 |
| `wide/string-loop` | 26,083 | 25 |
| `wide/throw-loop` | 34,026 | 1000 |
| `wide/try-loop` | 34,022 | 499500 |

[`area-manifest.json`](area-manifest.json) lists the digest of every file of the base, the collector, the rule and
the correspondence. `correspondence-runs/` is the correspondence's own record and is not listed.

## 3. Engine-level vectors on the Temporal area (SJL-10.2)

Broiler.JS's corpus revision 24 adds the Temporal area: 10,878 vectors in six files under
`conformance/vectors/temporal/`, all `normative`.
- **Where they come from.** Every expected value is the answer of the proposal's own polyfill at `e8cc03fc`
  (`ref:temporalref@1`), run under Node 22.22.0 with its dependencies pinned: JSD-0078's decision D10. Neither
  engine's code is consulted.
- **Where the pinned text and the polyfill disagree, the text rules** (decision D7). 73 vectors take the text's
  answer and record the polyfill's beside it.
- **What they cover.** The correspondence's probes with the edges of its rows: every row with a behavioural
  difference, and the rows of agreement beside them.

Revision 25 classifies both engines' first runs on revision 24.
- **This profile has six `divergence.broiler-vm` fields, all row R6**, its one correction (decision D9), which
  SJL-10.4 makes: `temporal/balancing/0078` to `0080`, `0084`, `0085` and `0088`. They are `new PlainDate` with a
  year of 2^53 + 2 or of 10^20 of either sign, `new PlainDateTime` with 2^53 + 2 or 10^20, and `new PlainMonthDay`
  with a reference year of 10^20. Each expects a `RangeError`, and this profile ends the invocation, rendered
  `abort:` with `ProfileFault/ProfileContractViolation`. The row counts five such cases; `new PlainDateTime` with
  2^53 + 2 is a sixth, of the same cause.
- **Broiler.JS's 473 are its own corrections**, which its SJL-10.5 answers. No vector differs in both engines.

**Where the first runs are.** This profile's first runs on revisions 24 and 25 were taken for Broiler.JS's
classification, at `14072b0` from scratch pins, and Broiler.JS's bundle keeps them as the basis of those fields.
This bundle does not read them. Its own run is on the committed pin, at revision 25 as Broiler.JS's default branch
carries it, `66b77088` (Broiler-Platform/Broiler.JS#1051).

The collector's `vectors` step, bundle `sjl-6-001`'s, added to [`collect-area.py`](collect-area.py) at `ad20ee5`,
built the conformance root in Release ([`vectors/build-conformance.log`](vectors/build-conformance.log)) in a clean
detached checkout of `389f3e0`, and ran it there.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 25 | At `389f3e0`, clean, on the committed pin, which moves to revision 25 there, from 16:01Z on 2026-10-18 | 103,312 vectors: 103,306 match, 6 classified divergences, the six of row R6, and 0 unclassified differences | [`run-revision-25.log`](vectors/run-revision-25.log), [`run-revision-25.tsv.gz`](vectors/run-revision-25.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 103,305 match, the same 6 classified divergences, and 1 unclassified difference, the mutated vector | [`run-negative-control-revision-25.log`](vectors/run-negative-control-revision-25.log), [`run-negative-control-revision-25.tsv.gz`](vectors/run-negative-control-revision-25.tsv.gz) |

**This profile matches every Temporal vector but row R6's six, and revision 25 classifies those six.** None is
unclassified, so the vectors find no correction of this family's that the correspondence missed. The product
sources at `389f3e0` are `14072b0`'s. The report gives every vector the rendering and verdict that the first run on
revision 25, in Broiler.JS's bundle, gave it, line for line. Only its header's corpus commit differs, with the same
content digest.

The pin is at revision 25 (`src/tests/vectors/pins/vectors.pin`) since `389f3e0`, and was at revision 23 from
`458c10a` (SJL-8.6). Its archive is taken twice, byte-identical, and its 57 files are hashed from an extracted copy.
The same method, applied to revision 23's commit, gives that pin's two digests and its count exactly.

[`area-manifest.json`](area-manifest.json) lists the digest of every file of `vectors/` with the others.

## 4. The cut-over (SJL-10.4)

*(Added 2026-10-18, at SJL-10.4.)*
[JSD-0079](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0079-the-profile-on-the-shared-temporal-core.md)
records it. The changes, each building, with the tests it touches green:

| Commit | What it does |
|---|---|
| `66b0877` | `Broiler.ECMAScript.Algorithms` and the test-only `Broiler.ECMAScript.Generation` `0.1.0-preview.6`, packed at Broiler.JS `bf81d1b7` (SJL-10.3's library at `e33b53e6`, its version moved) and not yet published, are admitted from the committed feed in place of `0.1.0-preview.5` |
| `a13a41c` | The package baseline is bundle JS-10-010's (JSC-314), as rule N37 asks |
| `f14abb7` | The profile's Temporal core is the shared `Broiler.ECMAScript.Temporal`'s: `JsTemporalCore.cs`, `JsCalendars.cs` and `JsTemporal.CalendarNonIso.cs` are deleted whole, `JsTemporalParser` is a thin wrapper over the grammar, and the realm files keep the objects, the reads and the errors. Rows R6 and D12's day window are corrected, each held by a slice-compiler check |
| `9b7eead` | Rule I5 registers the 147 deleted types and members, and the notices say where the calendars' arithmetic went |
| `c0355f4` | JSD-0079, the record |

**This family's corrections are made**, and no other answer moves:
- **Row R6** (decision D9): `new PlainDate` and `new PlainDateTime` with a year of 2^53 + 2 or ±10^20, and `new
  PlainMonthDay` with a reference year of ±10^20, throw a `RangeError` where the invocation ended.
- **Row D12's day window** (decision D11): `since` in days, and `Duration`'s `round` and `total` in days, relative to
  Pacific/Apia's 2011-12-31, throw a `RangeError` where a .NET `ArgumentOutOfRangeException` ended the invocation.

[`cutover/corrections.txt`](cutover/corrections.txt) gives both checks' lines, and each construction run alone at the
base's product sources and at the cut-over's.

**Checked at the product tree of `f14abb7`.** `9b7eead` and the record change no product file. The files are in
[`cutover/`](cutover/) and [`vectors/`](vectors/):
- **Part B's reference datasets.**
  - Each of the six named checks prints the line the base's [`base/datasets/named-checks.txt`](base/datasets/named-checks.txt)
    gives ([`cutover/named-checks.txt`](cutover/named-checks.txt)).
  - No file under `src/tests/temporal/` changed.
  - The slice compiler passes 651 checks: the base's 649, all passing this time, and the two corrections' checks.
- **The shared vectors at revision 25 match, every one**, through the conformance root on the committed pin, from the
  clean tree of `9b7eead`:
  - 103,312 vectors, 103,312 match, 0 classified divergences, 0 unclassified differences
    ([`vectors/cut-over-revision-25.log`](vectors/cut-over-revision-25.log),
    [`vectors/cut-over-revision-25.tsv.gz`](vectors/cut-over-revision-25.tsv.gz));
  - **row R6's six `divergence.broiler-vm` fields no longer hold.** `temporal/balancing/0078` to `0080`, `0084`, `0085`
    and `0088` render `error: RangeError`, the expected value, where section 3's run rendered the ended invocation, and
    the report marks each "divergence no longer holds". Retiring the fields is the corpus's to do, in Broiler.JS;
  - every other vector renders as section 3's run rendered it, line for line
    ([`vectors/cut-over-revision-25.against-base.txt`](vectors/cut-over-revision-25.against-base.txt));
  - the negative control reports the mutated vector alone
    ([`vectors/cut-over-negative-control-revision-25.log`](vectors/cut-over-negative-control-revision-25.log)).
- **The correspondence's probes**, run again through the conformance root as their scripts run them
  ([`cutover/correspondence-probes.txt`](cutover/correspondence-probes.txt)):
  - 54 of the 70 with a retained answer of this family's answer line for line as retained;
  - row R6's five answer `RangeError` where the invocation ended;
  - eleven differ in messages alone, on 44 lines, each with the same error type. JSD-0079 section 7 says why: a
    message now names the status the library answers, where this family named a cause it no longer meets.
- **Part C's twins keep their base figures.** Each of the ten wide twins completes at its base figure, with its
  answer, and is refused on fuel at one less, in both forms. The rows of
  [`cutover/fuel-parity.log`](cutover/fuel-parity.log) are the base's [`base/fuel-parity.log`](base/fuel-parity.log)
  line for line.
- **A local test262 subset, bytecode form,** over the area's directories and `Date`
  ([`cutover/test262-subset.txt`](cutover/test262-subset.txt)): 7,696 files and 15,330 variants, every one with the
  hosted base's verdict.
  - On both sides, 15,240 variants passed, 12 failed, 16 exhausted their allowance and 62 were skipped. No case reaches
    row R6 or row D12, as part A's rule expected of R6.
  - The sixteen exhausted variants are `staging/sm/Date/dst-offset-caching`'s. They read no zone, because this
    profile's local time is UTC.
  - Fourteen of them ran out of wall clock on this machine, and two out of fuel; the hosted base's were six and ten.
    With a wall clock of 120 seconds, all sixteen exhaust on fuel.
- **The tests pass:** the architecture tests, rule I5's among them over the 147 new rows, and the contract tests,
  with JSD-0079's figures. A full Release build has no warning.

**The assurance coverage.** The code the cut-over deleted left this component's assurance coverage: the 147 types and
members rule I5 registers for SJL-10.4, with the bodies JSD-0079 section 8 names. What computes them now is
`Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Temporal`, which the home's own assurance component
covers in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json`). The adapters and seams that stay are
covered here. The exclusion is filed with SJL-10.6's evidence, in this bundle's exclusion section, as SJL-8.7 filed
EX-131 for the time-zone and calendar code: rule H2 reads a definition only there.

**The published packages.** *(Added 2026-10-18, later.)* Both packages were published to nuget.org later the
same day, from Broiler.JS `fc829f8a`, a merge whose tree is `bf81d1b7`'s, and the committed feed now holds them
as published ([JSC-315](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-315)). The
checks above used the copies packed from `bf81d1b7`. With the published packages:
- a locked restore with an empty package cache succeeds, and the solution builds in Release with no warning;
- the architecture tests and the contract tests pass;
- the conformance root's run on revision 25 gives a report identical byte for byte to
  `cut-over-revision-25.tsv.gz`'s, and prints the lines `cut-over-revision-25.log` holds below its collector
  header;
- its negative control prints the lines `cut-over-negative-control-revision-25.log` holds below its collector
  header, and its report is the one the cut-over's negative control wrote, byte for byte.

Neither run is retained, since neither differs.

SJL-10.6 judges parts A to C; this section judges nothing.
