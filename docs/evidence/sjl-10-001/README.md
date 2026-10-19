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

**State, 2026-10-19, at SJL-10.6: the after arm is taken at `96876432` (sections 5 to 13). Parts A, B and C of
[`decision-rule-area.md`](decision-rule-area.md) are MET as written, with no revision** (sections 6 to 8).
- **Part A:** the after run, [39261526513](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39261526513),
  against the base's. Every changed test262 verdict in either form is a race between the wall clock and another
  allowance, which class C4 (c') admits. Each row's re-runs at both commits, alone on the local machine with no other
  build, test or run on it (section 5), agree, but one's, whose (e) runs at ten times the wall clock give the same
  exhaustion on fuel at both commits. No verdict moved between a pass, a failure and an exhaustion, and no row is a
  correction, so row R6 explains none.
- **Part B:** each of the six named checks prints the base's line, and no file under `src/tests/temporal/` differs
  between `99c9c9f` and `96876432`.
- **Part C:** every wide twin completes at its base figure with its answer, and is refused one below, in both forms.
- **Every Broiler.VM clause of the milestone is met** on this bundle (section 13): rules I1 to I7 hold, test262 in both
  forms and the polyfill dataset meet the area rule, and every root is published and run in three modes. The
  engine-level vectors at corpus revision 26 all match at the after commit (section 10).
- **Nothing is accepted.** Review is deferred, and SJL-10 may not meet its gate before SJL-6 and SJL-8 meet theirs.

*(Revised 2026-10-19, at SJL-10.6, after review, when section 5 recorded that the wait's condition did not hold
throughout the re-runs. Part A's bullet read "Each row's re-runs at both commits, alone on the idle local machine, agree,
but one's".)*

*(Revised 2026-10-19, at SJL-10.6, when part A's re-runs were taken. The state read: "**State, 2026-10-19, at
SJL-10.6: the after arm's local steps are taken at `96876432` (sections 5 to 13), and part A's hosted after run is not
yet imported. Parts B and C of `decision-rule-area.md` are MET as written, with no revision (sections 7 and 8). Part A
is not yet judged (section 6).**", with the bullets "**Part A:** the after run, 39261526513, was dispatched at 13:35Z
with both forms and the tested commit `96876432`. It is imported, compared and judged when it is complete." and "**The
other clauses are met** on this bundle (section 13): rules I1 to I7 hold, and every root is published and run in three
modes. The engine-level vectors at corpus revision 26 all match at the after commit (section 10).")*

*(Revised 2026-10-19, at SJL-10.6. The state read: "**State:** rules, correspondence, the base, the vectors and the
cut-over, 2026-10-19, at SJL-10.5, when this family's pin moved to corpus revision 26. **Nothing here is a verdict.**",
with the bullets "The base of all three parts was taken at the commit that carries the rule, `99c9c9f`, before any code
of SJL-10 landed (section 2): part A's by the hosted workflow, parts B and C locally.", "The engine-level vectors ran on
this family's pin at corpus revision 25, which holds the Temporal area, and at revision 26, which retires its
divergence fields (section 3).", "SJL-10.4's cut-over is in place, with the checks it ran (section 4)." and "The after
runs are SJL-10.6's.")*

*(Revised 2026-10-18, at SJL-10.2, when the pin moved to revision 25. The state read "rules, correspondence and the
base", and had no item on the vectors.)* *(Revised 2026-10-18, at SJL-10.4. The state read "rules, correspondence,
the base and the vectors, 2026-10-18, at SJL-10.2", and its last item "The after runs follow SJL-10.4's cut-over.")* *(Revised 2026-10-19, at SJL-10.5, when the pin moved
to revision 26. The state read "rules, correspondence, the base, the vectors and the cut-over, 2026-10-18, at SJL-10.4",
and its item on the vectors "The engine-level vectors ran on this family's pin at corpus revision 25, which holds the
Temporal area (section 3).")*

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
    *(Note, 2026-10-19, at Broiler.JS's SJL-10.5. Row D12's day window and its zoned branch are reached, by a zone's
    skipped day, and the owner's decisions D11 and D12 answer a `RangeError` there. And Broiler.JS's records say that
    much of the code no input reaches was deleted from files that remain, not with its file.
    [JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md)
    records both. The item above is left as written.)*
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

*(Note, 2026-10-19, at SJL-10.5, when the pin moved to revision 26.)* **The pin is at revision 26 since `d625402`**,
at Broiler.JS `00b65dd0`, its default branch after Broiler-Platform/Broiler.JS#1054 merged. Revision 26 retires every
divergence field of the Temporal area: row R6's six, which SJL-10.4 corrected (section 4), and Broiler.JS's 473, which
its SJL-10.5 corrects. Its six Temporal files are revision 24's byte for byte, and no expected value changed.
- **The archive** was taken twice, byte-identical, and its 57 files hashed from an extracted copy. The same method,
  applied to revision 25's commit, `66b77088`, gives that pin's two digests and its count exactly.
- **The run.** The collector's `vectors` step built the conformance root in Release and ran it on the committed pin,
  from the clean tree of `d625402`, whose product sources are `e42cd5d`'s, the default branch after SJL-10.4
  (Broiler-Platform/Broiler.VM#151). Its build log is not retained:
  [`vectors/build-conformance.log`](vectors/build-conformance.log) is revision 25's run's.
  *(Note, 2026-10-19, at SJL-10.6. `vectors/build-conformance.log` is now the after commit's build, section 10's, which
  replaced revision 25's run's; that log, which both links in this section name, is in this repository's history. The
  item above is left as written.)*

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 26 | At `d625402`, clean, on the committed pin, from 12:33Z on 2026-10-19 | 103,312 vectors: 103,312 match, 0 classified divergences and 0 unclassified differences, and no divergence field that no longer holds | [`run-revision-26.log`](vectors/run-revision-26.log), [`run-revision-26.tsv.gz`](vectors/run-revision-26.tsv.gz) |
| Its negative control | The same, with `--mutate text/trim/0001` | 103,311 match, 0 classified divergences, and 1 unclassified difference, the mutated vector | [`run-negative-control-revision-26.log`](vectors/run-negative-control-revision-26.log), [`run-negative-control-revision-26.tsv.gz`](vectors/run-negative-control-revision-26.tsv.gz) |

**This profile matches every vector of revision 26.** The only rows of class `divergence` in the report are the number
area's four, `numbers/literal/0082`, `0084`, `0088` and `0090`. Their `divergence.broiler-js` fields are Broiler.JS's,
item SJL-14's, and do not concern this family, and each matches here. Every vector has the expected value and the
rendering it had in section 4's run on revision 25 at the cut-over
([`vectors/cut-over-revision-25.tsv.gz`](vectors/cut-over-revision-25.tsv.gz)), line for line. 479 rows differ only in
their class, `normative` here where it was `divergence`, and, for row R6's six, in a verdict that no longer adds
"divergence no longer holds".

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

*(Note, 2026-10-19, at Broiler.JS's SJL-10.5.)* **Row D12's zoned branch is reached too**, by the same skipped day:
the day span whose sign NudgeToZonedTime asserts is zero when a time unit is rounded under a date largest unit from
Pacific/Apia's 2011-12-31T16:15 back to an earlier instant of the same day, such as 2011-12-31T00:00, where the
difference has no date part. This family answered durations from outside the text's domain there before the
cut-over, and answers the same on `0.1.0-preview.6`, so no answer moved and "no other answer moves" above holds. The
owner's decision D12 of 2026-10-19
([JSD-0078](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0078-the-temporal-core-moves-to-the-shared-algorithms-library.md))
makes the answer a `RangeError`, which this family takes with the shared stream's next version. Broiler.JS's bundle
`sjl-10-001` retains this family's answers there at `14072b0` and at `83fef1a`, in `cutover/d12-facts.md`, observed in
Broiler-Platform/Broiler.JS#1054 at `c33933f9`, merged as `00b65dd0` on 2026-10-19; this bundle retains none.
*(Revised 2026-10-19, when the change merged; read "… at `c33933f9`, not yet merged; this bundle retains none.")*

*(Note, 2026-10-19, later.)* This family takes decision D12 with the shared stream's next version, `0.1.0-preview.7`,
published that day from Broiler.JS `5ddee404`
([JSC-317](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-317)): `since` with largest
unit `day` and smallest unit `hour` back from 2011-12-31T16:15 to 2011-12-31T00:00, `P1DT16H` on `0.1.0-preview.6`,
throws a `RangeError`, and the slice compiler's check `temporal/t1/a-day-span-a-zone-skips-is-a-range-error` holds
it. The paragraph above, which says this family takes the decision "with the shared stream's next version", is left
as written; the checks run again with that version are in "The shared stream's `0.1.0-preview.7`" below.

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

**The shared stream's `0.1.0-preview.7`.** *(Added 2026-10-19, after SJL-10.6.)* Both packages moved to
`0.1.0-preview.7`, published to nuget.org that day from Broiler.JS `5ddee404`, whose one change under
`Broiler.ECMAScript/` is decision D12 with its tests and records, and the committed feed holds them as published
([JSC-317](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-317)). With them, at the tree
of that change:
- a locked restore with an empty package cache succeeds, a restore from nuget.org alone records the pins' content
  hashes, and the solution builds in Release with no warning;
- the contract tests pass, and the architecture tests pass but rule N37, which holds again when the package
  baseline moves to the candidate pack taken at that tree, in the change after it;
- the conformance root's run on revision 26 gives a report identical byte for byte to `run-revision-26.tsv.gz`'s,
  and so to `after-revision-26.tsv.gz`'s, and prints the lines `run-revision-26.log` holds below its collector
  header; its negative control's report and lines are `run-negative-control-revision-26`'s;
- part B's six named checks print the lines [`after/datasets/named-checks.txt`](after/datasets/named-checks.txt)
  holds, the files under `src/tests/temporal/` hash as [`after/datasets/files.sha256`](after/datasets/files.sha256)
  lists them, and every slice-compiler check that ran passes, the new
  `temporal/t1/a-day-span-a-zone-skips-is-a-range-error` among them;
- decision D12's case answers a `RangeError` through the conformance root, where `main` before the change, on
  `0.1.0-preview.6`, answered `P1DT16H`; the case forward from 2011-12-31T00:00 answers `PT16H` on both.

Neither run is retained, since neither differs, and nor is the probe.

SJL-10.6 judges parts A to C; this section judges nothing.

## 5. The after arm (SJL-10.6)

*(Added 2026-10-19, at SJL-10.6. Sections 1 to 4 do not change, but for a dated note in section 3 on the conformance
root's build log. The exclusions are section 9, because rule H2 reads an exclusion's definition from a bundle's
section 9 alone.)*

The after steps were added to [`collect-area.py`](collect-area.py) at SJL-10.6, when there was an after commit to run
them at. They write into [`after/`](after/) and run bundle `sjl-8-001`'s code wherever that bundle has the step:
- the import of a hosted run, its completeness and the comparison (bundle `sjl-6-001`'s);
- the twins at the base's figures, the (c') re-runs and the (e) wall-clock race (bundles `sjl-4-001` and
  `sjl-5-001`);
- part B's after run beside its comparison with the base, over this rule's six checks and its one folder (bundle
  `sjl-8-001`'s);
- the closures, run from the tree they publish, with the digest of each Native AOT image record (bundle `sjl-8-001`'s),
  and the architecture suite (bundle `sjl-3-001`'s).

Two steps are this bundle's own:
- `after-identity`, with this bundle's names;
- `simplification`, the Temporal core's search, with the lines each C# file gained and lost.

`packages` is `sjl-6-001`'s with this bundle's name. [`area-manifest.json`](area-manifest.json) now also lists
[`after/`](after/), and names the after commit.

| | |
|---|---|
| Commit | `96876432`, the default branch after Broiler-Platform/Broiler.VM#153, a clean checkout ([`after/identity.txt`](after/identity.txt)). Its product sources are those of `e42cd5d`, the merge of Broiler-Platform/Broiler.VM#151, which carries SJL-10.4's cut-over (`66b0877`, `a13a41c`, `f14abb7` and `9b7eead`, which JSD-0079 records in `c0355f4`) with the committed feed pinned to `0.1.0-preview.6` as published in `8b6cc96` (JSC-315). Against `e42cd5d` the identity lists 5 files outside the documents, all of them `d625402`'s: this family's pin of the shared corpus at revision 26, its two archives (revision 26's added, revision 25's removed) and its README, and the notices row that names the pin. None is product code. It is the after commit by bundle `sjl-8-001` section 5's precedent: the default branch after the other engine's cut-over merged |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), with the same SHA-256 at the base and after (both identities) |
| Collector | [`collect-area.py`](collect-area.py) as this change commits it, run from this repository's checkout against a clean detached checkout of `96876432` and, for the re-runs, one of `99c9c9f` |
| Taken | Locally, on 2026-10-19: `after-identity`, `after-twins` and `after-datasets` from 13:50Z; `vectors` from 14:01Z; `simplification` at 14:15Z; `closures` from 14:17Z; `rules` from 14:53Z; `packages` from 14:55Z. Another workflow's builds and tests shared the machine throughout, and so did Broiler.JS's SJL-10.6 local steps, from 13:45Z to its `test262` and `compare` steps at 15:18Z (the "Taken" row of its bundle's section 5); none of these steps is timed. Part A's after run by the workflow, on 2026-10-19 from 13:35Z to 13:43Z; then the three steps that read its artifact, `after-import`, `after-complete` and `compare`, locally at 15:18Z, from the zip as downloaded at 14:25Z, while the other workflow's work went on, and none of them is timed either. `rerun` and `wall-race` from 16:26:59Z to 16:31:32Z, with no other build, test or run on the machine (below) |
| Machine | the base's: `linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv` |
| Base side of the re-runs | a clean checkout of the base commit `99c9c9f`, its conformance root built there in Release with a package folder of its own (below); only `rerun` and `wall-race` read it |

*(Revised 2026-10-19, at SJL-10.6, when part A's after run was imported. The "Taken" row's last sentence read: "Part
A's after run, by the workflow, and the five steps that read it, `after-import`, `after-complete`, `compare`, `rerun` and
`wall-race`, are not yet taken (section 6)".)* *(Revised again 2026-10-19, at SJL-10.6, when the re-runs were taken.
The "Taken" row's last sentence read "`rerun` and `wall-race`, which run the (c') rows again with the machine otherwise
idle, are not yet taken (section 6)"; the "Base side of the re-runs" row read "to be a clean checkout of the base commit
`99c9c9f`, its conformance root built there in Release, when the re-runs are taken. No step taken so far reads it"; and
the "Collector" row ended "against a clean detached checkout of `96876432`".)* *(Revised again 2026-10-19, at SJL-10.6,
after review. The "Taken" row's sentence on what shared the machine read "Another workflow's builds and tests shared the
machine throughout, and none of these steps is timed.")*

**The re-runs' machine.** Both conformance roots were built in Release before the re-runs, from their clean checkouts:
`96876432`'s at 15:25Z, and `99c9c9f`'s at 15:27Z. The base checkout's locked restore refused the machine's shared
package folder, which holds a `Broiler.ECMAScript.Algorithms` `0.1.0-preview.5` whose content hash is not the one the
base's lock files pin (NU1403), so it was built with an empty package folder of its own: the restore took the committed
feed's package, at the content hash the lock files pin, and the build gave no warning.

`rerun` and `wall-race` were started by a wait script, not by the collector. The script and every file it wrote are
retained in [`after/rerun-idle/`](after/rerun-idle/), each as written, the samples gzip-compressed:
[`wait-and-run.sh`](after/rerun-idle/wait-and-run.sh), its log
[`wait-and-run.log`](after/rerun-idle/wait-and-run.log), the load average and the process table when the steps started
and when they ended ([`start-loadavg.txt`](after/rerun-idle/start-loadavg.txt),
[`start-ps.txt`](after/rerun-idle/start-ps.txt), [`end-loadavg.txt`](after/rerun-idle/end-loadavg.txt),
[`end-ps.txt`](after/rerun-idle/end-ps.txt)), the samples between
([`samples.txt.gz`](after/rerun-idle/samples.txt.gz)), and the collector's console output
([`collector.log`](after/rerun-idle/collector.log)). The log's last line, "collector exit 1", is the `rerun` step's
own code: the step exits 1 when any row's two re-runs disagree, as bundle `sjl-4-001`'s step does, and here one
bytecode row's did, which the `wall-race` step then took (section 6). It is not a failure of either step.

The script waited for the machine to be otherwise idle: no process whose arguments name the other workflow's folders
(`ci-opt`, `/home/user/js-ci` or `/home/user/vm-ci`), and a one-minute load average below 1.0, twice 30 seconds
apart. That held at 16:26:29Z; `dotnet build-server shutdown` was run, the same wait held again, and the steps started
at 16:26:59Z with a load average of 0.25. The script then sampled the process table every five seconds until the steps
ended at 16:31:32Z, each sample holding the fourteen processes with the highest `%CPU` and, marked `OTHER`, every
process the wait's test matches.

**The wait's condition held when the steps started, but not throughout them.** The other workflow is the CI
optimisation, whose agents run under the same Claude Code process as this bundle's steps: its commands source the same
shell snapshot as the shells that process, `claude` (process 98 in `start-ps.txt`), started for these steps, among them
process 12124, which ran the wait script. Two samples show its commands, both during the bytecode pair's re-run at
`99c9c9f`, from 16:26:59Z to 16:28:46Z: at 16:27:35Z, a `git commit` of a merge and a `git push` to Broiler.JS; at
16:28:35Z, a `python3` edit of a merge's commit message, a `git commit` and a `git push` to Broiler.VM. **No other
build, test or run was on the machine.** Besides the steps and those commands, the samples show a short script under
the same Claude Code process reading a log of the agent that ran these steps, at the first sample; two MSBuild nodes,
idle throughout; and a compiler server an earlier build had left, idle, until it exited at 16:30Z.

*(Revised 2026-10-19, at SJL-10.6, after review, when the wait script and its files were retained. The paragraphs
above read: "`rerun` and `wall-race` then waited for the machine to be otherwise idle: no process of the other workflow,
and a one-minute load average below 1.0, twice 30 seconds apart. That held at 16:26:29Z; `dotnet build-server shutdown`
was run, the same wait held again, and the steps started at 16:26:59Z with a load average of 0.25. The process table
was sampled every five seconds until they ended at 16:31:32Z. **No other build, test or run was on the machine.**
Besides the steps themselves, the samples show only another session's `git` commands in its own checkouts, among them
commits and pushes at 16:27Z and 16:28Z, and a short script of that session reading a file at the first sample; two
MSBuild nodes, idle throughout; and a compiler server an earlier build had left, idle, until it exited at 16:30Z." The
commands were the CI optimisation's, under the same process as these steps, not another session's.)*

## 6. Test262, both forms (rule part A)

*(Written 2026-10-19, at SJL-10.6, when part A's after run was imported and compared. The section read "(To be written
when the hosted after run is imported.)")*

**MET.** All four runs are complete as the rule states it. Every changed verdict is in class C4 (c'): seventeen rows
in the bytecode pair and one in the native pair, listed below with their re-runs. Every row's re-runs agree but one's,
`staging/sm/Date/dst-offset-caching-8-of-8.js` [sloppy] in the bytecode pair, and its (e) runs give the same verdict
at both commits, an exhaustion on fuel, so (e) admits it. No row is in C1, and the comparison classified every row it
lists.

*(Revised 2026-10-19, at SJL-10.6, when the re-runs were taken. The verdict read: "**Not yet judged.** All four runs
are complete as the rule states it. Every changed verdict is in class C4 (c'): seventeen rows in the bytecode pair and
one in the native pair, listed below. No row is in C1, and the comparison classified every row it lists. The rule admits
a (c') row only when its re-runs at both commits, alone on the idle local machine, agree, or under (e) when they differ,
so part A's verdict is the re-runs'. *(The re-runs follow; see below.)*")*

**The after run.** [39261526513](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39261526513), attempt 1,
was dispatched from `main` at `96876432` on 2026-10-19 at 13:35:57Z. It ran with both forms and the commit `96876432`,
named by all forty digits, and completed at 13:43:14Z ([`after/hosted/run.txt`](after/hosted/run.txt)).
- **The workflow and its driver at the dispatched ref are byte-identical to the rule commit's.** The workflow
  `.github/workflows/test262-evidence.yml`, its driver `eng/test262-evidence.py`, and the `eng/run-test262.py`,
  `eng/collect-js-release-candidate.py` and `eng/compare-test262-forms.py` the rule names have the same blob at
  `99c9c9f` and `96876432`, and `src/tests/conformance/pins/` is unchanged. `git diff 99c9c9f 96876432 -- .github eng`
  names only the committed feed under `eng/packages/`, which SJL-10.4 moved to `0.1.0-preview.6`.
- **The artifact** `test262-evidence` is retained unchanged in [`after/hosted/`](after/hosted/), imported by the
  collector's `after-import` step. [`after/hosted/imported.txt`](after/hosted/imported.txt) records the digest of the zip
  as downloaded, `fa2a6645...`, which is the digest GitHub records for the artifact.
- **Every runner and the merging job** ran image `ubuntu24` 20261012.334.1, as at the base (`run.txt` and each form's
  `after/hosted/<run>.runners.txt`). Each form's runners built one assembly list, the same list in both forms.

| Run | Complete ([`after/completeness.log`](after/completeness.log)) | Totals | Comparison |
|---|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | the base's (section 2), figure for figure | 17 variants differ, all (c') ([`after/compare-wide-bytecode.log`](after/compare-wide-bytecode.log)) |
| `wide-native` | exit 4, a re-base, as at the base; whole, retainable, no shard finding, one assembly list | the base's (section 2), figure for figure | 1 variant differs, in (c') ([`after/compare-wide-native.log`](after/compare-wide-native.log)) |

**The comparisons.** The collector's `compare` step ran the after tree's `eng/compare-test262-forms.py --same-form`,
the base's report as control and the after's as candidate, the native pair with `--exempt-guest-loads`. The suite it
read is the after tree's archive under `src/tests/conformance/pins/`, whose SHA-256 is the pin's `archive-sha256`,
extracted as the workflow extracts it: it holds the number of files the pin's `files` line gives, and `tar --compare`
against the archive finds no difference. Every variant is in both reports, and none is ignored.

Both comparisons exit 0, and each says every difference is in an admitted class. **No variant moved between a pass, a
failure, a refusal and an exhaustion**: in every changed row both runs gave no answer, and they differ only in which
allowance ran out first. So every variant skipped at the base is skipped after, the `canonical-tz` and
`Intl.Era-monthcode` cases JSD-0054 skips among them, as the rule expects.

**No row is in C1.** No variant that failed at the base passes after. The rule's table of C1 rows, each with the clause
it follows and whether row R6 explains it, therefore has no row:

| C1 row | Clause it follows | Row R6 explains it |
|---|---|---|

**Row R6 explains nothing, because no verdict changed.** No test262 case at the pin reaches R6, as the rule expected
and section 4's subset found, so no C1 row is a finding that JSD-0078's correspondence missed. Classes C2, C3 and C5
are not in the rule, and no row would need them: every changed row has a wall-clock exhaustion on one side, so no
exhaustion moved on fuel alone, and no verdict moved because a revision, a policy or the data moved.

**Class (c').** The comparisons list these eighteen rows, every one exhausted at both commits. Paths are under
`test/`.

| # | Path | Variant | Pair | At the base, `99c9c9f` | After, `96876432` | Re-run at `99c9c9f`, at `96876432` | Class |
|---|---|---|---|---|---|---|---|
| 1 | `annexB/built-ins/RegExp/RegExp-trailing-escape-BMP.js` | strict | bytecode | Exhausted/WallClock | Exhausted/NestedLoadFanOut | Exhausted/NestedLoadFanOut, the same | **C4 (c')**, the re-runs agree |
| 2 | `built-ins/decodeURI/S15.1.3.1_A2.5_T1.js` | sloppy | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 3 | `built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js` | sloppy | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 4 | `built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js` | strict | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 5 | `staging/sm/Date/dst-offset-caching-2-of-8.js` | sloppy | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 6 | `staging/sm/Date/dst-offset-caching-2-of-8.js` | strict | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 7 | `staging/sm/Date/dst-offset-caching-3-of-8.js` | sloppy | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 8 | `staging/sm/Date/dst-offset-caching-3-of-8.js` | strict | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 9 | `staging/sm/Date/dst-offset-caching-4-of-8.js` | strict | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 10 | `staging/sm/Date/dst-offset-caching-5-of-8.js` | strict | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 11 | `staging/sm/Date/dst-offset-caching-7-of-8.js` | sloppy | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 12 | `staging/sm/Date/dst-offset-caching-7-of-8.js` | strict | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 13 | `staging/sm/Date/dst-offset-caching-8-of-8.js` | sloppy | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/Fuel, Exhausted/WallClock | **C4 (e)**, below: the re-runs differ |
| 14 | `staging/sm/Date/dst-offset-caching-8-of-8.js` | strict | bytecode | Exhausted/Fuel | Exhausted/WallClock | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 15 | `staging/sm/TypedArray/sort_large_countingsort.js` | strict | bytecode | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 16 | `staging/sm/regress/regress-610026.js` | sloppy | bytecode | Exhausted/WallClock | Exhausted/NestedLoadBytes | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 17 | `staging/sm/regress/regress-610026.js` | strict | bytecode | Exhausted/WallClock | Exhausted/NestedLoadBytes | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |
| 18 | `staging/sm/Date/dst-offset-caching-1-of-8.js` | sloppy | native | Exhausted/WallClock | Exhausted/Fuel | Exhausted/WallClock, the same | **C4 (c')**, the re-runs agree |

*(The table gained its last two columns on 2026-10-19, at SJL-10.6, when the re-runs were taken.)*

The wall clock ran out at the base in eleven rows (1, 7 to 12 and 15 to 18) and after in seven (2 to 6, 13 and 14). The
rows' re-runs at `99c9c9f` and `96876432`, each one process of the conformance root over the rows' paths, alone, under
the part's manifest, form and allowances, are the collector's `rerun` step, and (e) for any row whose re-runs differ is
its `wall-race` step. Both were taken on 2026-10-19, with no other build, test or run on the machine (section 5).
*(Revised 2026-10-19, at SJL-10.6, when the re-runs were taken. The last sentence read "Neither is taken yet.")*

**Class (c') and its re-runs** ([`after/rerun-wide-bytecode.tsv`](after/rerun-wide-bytecode.tsv),
[`after/rerun-wide-native.tsv`](after/rerun-wide-native.tsv), with their logs and every report). For each pair, one
process of the conformance root ran the rows' paths at `99c9c9f`, then one at `96876432`, each running its tests one
after another, under the wide manifest, the pair's form and the part's fuel and wall-clock allowances: the bytecode pair
from 16:26:59Z to 16:30:35Z, the native pair from 16:30:35Z to 16:31:00Z. The rule asks for each run to be "one process
of the conformance root over the row's path"; these re-runs read that, as bundles `sjl-5-001` to `sjl-8-001` did, as one
process at each commit over all of the pair's (c') rows' paths, which runs their tests one at a time, and (e)'s runs
below are one process over row 13's path, as written. *(This sentence was added 2026-10-19, at SJL-10.6, after
review.)* **Seventeen rows' re-runs agree**, so C4 (c') admits them. The two re-runs of each pair, compared with `--same-form`, differ in row 13 alone in the bytecode pair and
in no variant in the native pair.

**Class (e)** ([`after/wall-race-wide-bytecode.tsv`](after/wall-race-wide-bytecode.tsv), with its log and both
reports). Row 13, `staging/sm/Date/dst-offset-caching-8-of-8.js` [sloppy], whose re-runs gave `Exhausted/Fuel` at
`99c9c9f` and `Exhausted/WallClock` at `96876432`, ran alone once more at each commit on the same machine, with the
part's fuel allowance and ten times its wall-clock allowance, 50,000 ms, the machine otherwise idle, from 16:31:00Z to
16:31:32Z. **It gives `Exhausted/Fuel` at both commits**: the same verdict, neither a wall-clock exhaustion nor an
internal defect, so (e) admits it. Each process ran the row's path, so its strict variant, row 14, ran too and gave the
same; (e) does not read it, because row 14's re-runs agree. The native pair has no row whose re-runs differ, and the
`wall-race` step says so and writes no file for it.

**What the re-runs show, and what they do not.** On this machine the wall clock decided most rows at both commits:
sixteen rows' re-runs agree on `Exhausted/WallClock`, and row 1's on `Exhausted/NestedLoadFanOut`. The rule admits a
(c') row whose re-runs agree, whatever they agree on, and takes (e) only for a row whose re-runs differ, so no other row
was run again. Those re-runs show that each of those rows, run one at a time under the part's allowances, runs out of the
wall clock at both commits, neither commit reaching another allowance first. They do not show which allowance either
commit would reach first with more time. Row 13's (e) runs show it for one row: with ten times the wall clock, it runs
out of fuel at both commits. *(Revised 2026-10-19, at SJL-10.6, after review. The third sentence read "Those re-runs
show that each of those rows, alone under the part's allowances, runs out of the wall clock at both commits, neither
commit reaching another allowance first.")*

**What the rows say.** Every row is one of the long-running tests this family's earlier bundles saw race between the
wall clock and another allowance: the after comparisons of bundles `sjl-6-001` to `sjl-8-001` list each path, as a (c')
row or, for `regress-610026.js`, as a guest-load variant in the native pair's class (b). Eleven of the eighteen are
`staging/sm/Date/dst-offset-caching-*`, which read no zone, because this profile's local time is UTC (section 4). The
others are the `RegExp` escape test, which races the wall clock against the nested-load fan-out, `regress-610026.js`,
which races it against the nested-load bytes, `decodeURI` and `decodeURIComponent`'s long loops, and the counting sort
of a large typed array. **No changed row is in the area**: none is under `built-ins/Temporal` or
`intl402/Temporal`, and every variant under either has the same verdict at both commits, in both forms.

## 7. The reference datasets (rule part B)

**MET.** The slice compiler, built in Release at `96876432` from the clean checkout, ran `--checks --verbose`
([`after/datasets/checks.log`](after/datasets/checks.log)).
- **Each of the six named checks passes and prints the base's line**, word for word:
  [`after/datasets/named-checks.txt`](after/datasets/named-checks.txt) against
  [`base/datasets/named-checks.txt`](base/datasets/named-checks.txt). Every line of every program answers as its
  retained answer does, with the same lines named by its divergences: section 2's table, unchanged.
- **No file under `src/tests/temporal/` differs between the two commits.** `git diff --name-only 99c9c9f 96876432 --
  src/tests/temporal` names no file, and [`after/datasets/files.sha256`](after/datasets/files.sha256) is
  byte-identical to the base's, over the same 22 files.
- [`after/datasets/compare.txt`](after/datasets/compare.txt) records the three comparisons, line by line.

**One line of the comparison differs, and it is no check the rule names.** The step also compares the last line of
`named-checks.txt`, the slice compiler's exit code: 1 at the base and 0 after, so the step itself exits 1. The base's 1
is section 2's finding: four checks no rule names, the differential checks that hold NaN payloads, failed in that run
and passed when it was run again. After, all four pass. The rule reads the named checks and the files, so the exit code
decides nothing.

The slice compiler passes 651 checks: the base's 649 and the two that hold section 4's corrections,
`temporal/t1/a-year-past-2-to-the-53-is-a-range-error` and `temporal/t1/a-day-window-a-zone-skips-is-a-range-error`.
It leaves three not run, the `x86-64-win64` baseline rows, which this machine cannot enter.

## 8. The fuel-parity twins (rule part C)

**MET.** [`after/fuel-parity.log`](after/fuel-parity.log) covers every twin of `src/tests/forms/wide` that
[`base/fuel-parity.log`](base/fuel-parity.log) bisected: ten twins, each in the bytecode form and in the native form
under `x86-64-sysv`, 20 rows.
- Each completes at its base figure with the base's answer, and is refused with `AllowanceExhausted on Fuel` at one
  less. The figures and answers are section 2's table.
- **No charge moved outside the area**, as JSD-0078 requires. Every figure, verdict and answer is also section 4's
  cut-over check's.
- **No verdict rests on the wall clock.** Every row completed or was refused on fuel, under the twins' wall-clock
  allowance of 600,000 ms, the base's (both identities). None ran out of wall clock, though another workflow's builds
  shared the machine.
- The command-line root was built at `96876432` for the step ([`after/build-cli.log`](after/build-cli.log)).

## 9. Exclusions

*(Added 2026-10-19, at SJL-10.6.)*

| ID | Status | Exclusion |
|---|---|---|
| EX-132 | Open | **The Temporal core that left this component's assurance coverage.** The 147 types and members rule I5 registers for SJL-10.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), with the bodies JSD-0079 section 8 names, left the JavaScript family's sources at SJL-10.4: `JsTemporalCore.cs`, `JsCalendars.cs` and `JsTemporal.CalendarNonIso.cs` whole, `JsTemporalParser`'s productions and parse state, and `JsTemporal`'s option rules, the calendars' ISO arithmetic, the strings and the duration arithmetic. What computes them now is `Broiler.ECMAScript.Algorithms`, namespace `Broiler.ECMAScript.Temporal`. The home's own assurance component covers it in the Broiler.JS repository (`Broiler.ECMAScript/assurance.config.json` lists the project: an annotation on every relevant declaration, its manifest generated and checked in CI, rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them (JSD-0079 section 11: 246 relevant units fewer). The adapters of JSD-0079 section 3, its seams of section 2 and the reference datasets under `src/tests/temporal/` stay in this component and stay covered. EX-125, EX-127, EX-128, EX-129, EX-130 and EX-131 say the same of the earlier areas |

## 10. Engine-level vectors at the after commit

**Every vector matches.** The conformance root of `96876432`, built in Release from the clean checkout, ran over this
family's pin, corpus revision 26 (`00b65dd0`), with the collector's `vectors` step:
- **103,312 vectors, 103,312 match, 0 classified divergences, 0 unclassified differences**
  ([`vectors/after-revision-26.log`](vectors/after-revision-26.log)). Its report, decompressed, is byte-identical to
  section 3's [`vectors/run-revision-26.tsv.gz`](vectors/run-revision-26.tsv.gz), taken at `d625402`, whose product
  sources are this commit's. The compressed files differ in their gzip headers alone, which name the file.
- **The negative control** changes the expected value of `text/trim/0001` in memory. The run reports that vector, and
  only it, as an unclassified difference
  ([`vectors/after-negative-control-revision-26.log`](vectors/after-negative-control-revision-26.log)). Its report,
  decompressed, is byte-identical to section 3's too.

The step rebuilt the conformance root at `96876432`, and its build log replaces revision 25's run's in
[`vectors/build-conformance.log`](vectors/build-conformance.log), as section 3's dated note says. The earlier one is in
this repository's history.

## 11. Closures, the rules and the packages

**Every JavaScript root this machine can publish was published and run in three modes, with the packages as
published.** [`after/closures/`](after/closures/) holds, for ExecutionOnly, SliceCompiler, Conformance, Cli and
PolyglotCli:
- the closure under the JIT, trimming and Native AOT;
- the catalog each mode printed;
- the transcripts of every publish and run.

Every publish and every run exited as it should:
- the slice compiler's 651 checks pass in every mode, with three not run on this machine and claimed by nothing, as
  part B's in-tree run gives (section 7);
- the execution-only root's 22 checks pass in every mode;
- every catalog is identical across its root's three modes;
- PolyglotCli's `nottext.wasm` is refused by design;
- each Native AOT publish leaves no managed assembly beside the image.

The Android root is still exclusion EX-126.

**Each root carries what it carried after SJL-8.** Every closure and every catalog is bundle `sjl-8-001` section 11's,
file for file:

| Root | `Broiler.VM.Profile.JavaScript.Intl` | `Broiler.ECMAScript.Algorithms` |
|---|---|---|
| ExecutionOnly | not carried | under the JIT and trimming |
| SliceCompiler | under the JIT and trimming | under the JIT and trimming |
| Conformance | under the JIT and trimming | under the JIT and trimming |
| Cli | not carried | under the JIT and trimming |
| PolyglotCli | not carried | under the JIT and trimming |

The Temporal core now ships inside `Broiler.ECMAScript.Algorithms`, which every root already carried, and Temporal is
admitted only with Intl and BigInt (JSD-0054), so only the two roots that hand in Intl data reach it. No root gained or
lost an assembly. Rule I3 holds this on the real closures (below).

| Record | What it holds |
|---|---|
| [`after/closures/admitted-packages.json`](after/closures/admitted-packages.json) | Rule I7's identity of the three admitted product packages the closures carry: `Broiler.ECMAScript.Algorithms` `0.1.0-preview.6` from Broiler.JS `fc829f8a`, and `Broiler.Unicode.Algorithms` and `.Segmentation` `0.1.0-preview.5` from Broiler.Unicode `b802014`. Each content hash is the allow-list's and the committed feed's: the packages as published (JSC-311, JSC-315). `Broiler.ECMAScript.Generation` is test-only and in no closure |
| [`after/closures/mstat.sha256`](after/closures/mstat.sha256) | The SHA-256 of each root's Native AOT image record. The records are not committed, as in the bundles before this one. Rule I3 read them in the rules run, which placed them in the tree for that run alone, and the log's first line names the same five digests |

**The rules on the real closures.** [`after/rules.log`](after/rules.log) is the whole architecture suite, built and run
as the lane runs it, at `96876432`. **Every test passes**, none skipped.
- **The milestone's rules:** I1, I2, I3 (which read the five image records), I4, I5, I6 and I7.
  - I1 admits `Broiler.ECMAScript.Algorithms` and the test-only `Broiler.ECMAScript.Generation` at `0.1.0-preview.6`,
    one version of each (JSD-0079 section 1).
  - I4's four tests hold the Unicode pin and the time pins as at SJL-8, each with its witnesses.
  - **Rule I5 holds the deletion list.** Its register holds the 147 types and members SJL-10.4 deleted, beside
    SJL-3.4's 34, SJL-4.4's 44, SJL-5.4's 44, SJL-6.4's 53, SJL-7.4's 29 and SJL-8.5's 24, and it passes with its
    witness. Its test reads the files the Temporal adapters live in (JSD-0079 section 9).
- **Rules N29 to N32**, which verify the archives and regenerate the tables with the home's generator, now
  `0.1.0-preview.6`'s, pass byte for byte, each with its witnesses.

**The packages.** [`after/packages/`](after/packages/) is `eng/collect-js-packages.py` at `96876432`, under its own
predeclared decision (`manifest.txt`), with the collector's transcript in [`after/packages.log`](after/packages.log). It
holds:
- the candidate pack of the whole solution, with every package's metadata and file list;
- a pristine JavaScript consumer that restores and runs, with upstream feeds unreachable, from a local feed holding the
  admitted shared packages;
- the rollback to the published `0.1.0-preview.5` set and forward again;
- the negative control;
- the consumer as Native AOT.

**Every step is as expected.** The consumer ran in each of its three runs, the negative control failed to restore, and
the Native AOT consumer ran, its check of Temporal from its package among its six. The nuspecs declare
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.6` (the profile and the compiler), `Broiler.Unicode.Algorithms`
`0.1.0-preview.5` (the profile, Format, the compiler and the Intl data) and `Broiler.Unicode.Segmentation`
`0.1.0-preview.5` (the Intl data alone). That is what JSD-0079 section 1 and bundle JS-10-010's baseline say.
- **The nuspecs are JS-10-010's** but for each package's `repository` element. It names `96876432` where the baseline
  names `66b0877` and its branch, and it carries no branch, because the clean checkout the step ran in was detached.
  Every dependency is the same, and so is every package's list of file names.
- **Some file sizes differ, as they should.** JS-10-010 was packed at `66b0877`, before the cut-over's code landed: the
  profile's assembly is smaller here, 1,445,376 bytes against 1,559,040; two other assemblies differ by 512 bytes each;
  and every package's notices carry the rows `9b7eead`, `8b6cc96` and `d625402` changed.
- It does not replace that baseline, which rule N37 holds.

## 12. The simplification check

No clause of SJL-10 reads this section. It is taken as bundle `sjl-8-001` section 13 took it, with this area's search.

**The deletion check.** [`after/simplification/deletion-check.log`](after/simplification/deletion-check.log) searches
the simple name of every member rule I5 registers, 375 of them, as a declaration in the JavaScript family's sources.
- **Each of SJL-10.4's 147 is declared nowhere as itself.** 136 of their simple names are declared nowhere. The other
  eleven are other members of the same simple name, or words the pattern reads as a declaration:
  - `OffsetNanoseconds`: `JsTemporal.OffsetNanoseconds` in `JsTemporal.Options`, an adapter over the shared
    `TemporalGrammar.TryParseOffsetString`, which the deleted parser member's register row names as its replacement;
  - `Has`, `Negated`, `Reject`, `AsciiLower`, `IsAlpha`, `Digits`, `Peek` and `at`: other types' members of the same
    name, outside the area - the collections' and the compiler's `Has`, the regular-expression matcher's `Negated`, a
    promise reaction's `Reject`, the locale tag's `AsciiLower` and `IsAlpha`, `JsDecimal`'s, the display names' and
    the locale's `Digits`, the compiler's parsers' `Peek`, and the parsers', the regular-expression matcher's and the
    realm's `at`;
  - `day` and `second`: no member - a lambda's parameter in `JsRealm.Locale`, and a word of two documentation
    comments, in the compiler and the opcode table.
- **The earlier areas** read as bundle `sjl-8-001` section 13 read them, with five more names gone, each with code
  SJL-10.4 deleted: 27 of SJL-3.4's 34, 39 of SJL-4.4's 44, all 44 of SJL-5.4's, 50 of SJL-6.4's 53, all 29 of
  SJL-7.4's and 23 of SJL-8.5's 24 are declared nowhere. The five are SJL-4.4's `Bounded` and SJL-6.4's `DateDay`,
  whose same-named members were the parser's productions; SJL-4.4's `ToDouble`, a fraction's in
  `JsTemporal.Durations`; and SJL-6.4's `DaysInMonth` and SJL-8.5's `NewYear`, the formula calendars' in `JsCalendars`.
- Rule I5 decides by the fully qualified name, and it passes (section 11).

**The duplicate search.** [`after/simplification/duplicate-search.log`](after/simplification/duplicate-search.log)
searches the component's C# product sources (`src/`, every `tests/` excluded) in four parts, each with its pattern:
- **host:** the platform's dates and calendars a hand-written core would compute with: .NET's calendar classes, which
  SJL-D-21 keeps out of both engines, `DateOnly`, `TimeOnly` and `ISOWeek`, and `DateTime`'s month lengths, leap years
  and month and year addition;
- **algorithm:** the area's algorithms by the names the pinned text gives the operations SJL-10.4's deleted code
  implemented (JSD-0079 section 8, the correspondence's section 10), by the limits' constants, and by the formula
  calendars' epochs and rules;
- **names:** the names of the 25 types rule I5 registers for SJL-10.4, as words anywhere, a use or a comment
  included, read from the register;
- **identifiers:** a calendar identifier, or an era name of the era proposal, spelled as a string literal, which a
  hand-written list of calendars or table of eras would spell.

**96 hits. None is a copy of what the deletion list or JSD-0078's X6 names.** The same four patterns, run with `git
grep` over the base commit `99c9c9f`, find 944 hits, 298 of them in the three files SJL-10.4 deleted whole, so the
search can see the code it looks for.

| Part | Hits | Where | What they are |
|---|---|---|---|
| host | 0 | | |
| algorithm | 24 | `JsTemporal.Calendar` (6), `JsTemporal.Conversions` (8), `JsTemporalParser` (4), `JsDateTimeFormatter`, `JsRealm.Temporal`, `JsRealm.TemporalPlainDate`, `JsRealm.TemporalPlainDateTime`, `JsTemporal.Durations` and `JsTemporal.Options` (1 each) | documentation naming the proposal's operation an adapter implements or calls, each saying it is the shared library's or the shared calendars' |
| algorithm | 6 | `JsRealm.TemporalInstant` (2), `JsTemporal.Conversions` (2), `JsRealm.TemporalZonedDateTime` (2) | the adapters' calls of the shared `IsoArithmetic.RoundTemporalInstant` and `RegulateTime`, and `DurationMath.TotalTimeDuration` and `RoundTimeDurationToIncrement` (JSD-0079 section 3) |
| algorithm | 4 | `JsTemporal.Conversions` (3), `JsRealm.TemporalPlainTime` (1) | the adapter `JsTemporal.RegulateTime`, which maps the shared `IsoArithmetic.RegulateTime`'s status to a `RangeError`, and its callers |
| algorithm | 3 | `JsRealm.Date` | the `Date` time value's range, the years -271821 to 275760, in documentation: the Date area's, the same at `99c9c9f` |
| algorithm | 1 | `JsIntlData` | documentation of the Persian calendar's corrections to its 33-year rule, the table data SJL-8 hands the shared library, the same at `99c9c9f` |
| algorithm | 2 | the slice compiler's `IntlChecks` | section 4's check of row R6, whose constructions include `new Temporal.PlainDate(275760, 9, 13)` |
| names | 19 | `JsTemporal.Options` (5), `JsTemporal.Conversions` (7), `JsRealm.TemporalZonedDateTime` (3), `JsTemporal.Durations` (2), `JsRealm.TemporalPlainDateTime` (1), `JsTemporalParser` (1) | `TemporalUnit` (11) and `TemporalGoal` (8): the shared library's `Broiler.ECMAScript.Temporal` types of those names, which replaced this family's, as their register rows say. No other of the 25 names appears |
| identifiers | 24 | `JsRealm.DateTimeFormat` (12), `JsDateTimeFormatter` (8), `JsDateData` (3), `JsRealm.Locale` (1) | Intl's calendars: `Intl.supportedValuesOf`'s list, the locale data's `ca` values, CLDR's calendar data layers and its index of an era, the formatter's resolved calendar and the locale's default, each SJL-9's or ECMA-402's and present at `99c9c9f` |
| identifiers | 13 | `JsTemporal.Conversions` (6), `JsRealm.TemporalInstant` (4), `JsRealm.TemporalPlainDate`, `JsTemporal.Calendar` and `JsTemporal.Durations` (1 each) | `"iso8601"`, the calendar the specification gives `Temporal.Now`'s ISO members, `toZonedDateTimeISO`, a constructor or an argument without a calendar, and a string without an annotation |

No hand-written limit, balance, rounding, difference, calendar date, parse or string survives. Every Temporal answer
the family's core computed is the shared library's, as JSD-0079 section 8 says, and the calendar names the family still
spells are Intl's, or the specification's ISO 8601 default.

**The lines** ([`after/simplification/lines.log`](after/simplification/lines.log)), by `git diff --numstat` between
`99c9c9f` and `96876432` over every C# file:

| Group | Files | Added | Removed | Net |
|---|---|---|---|---|
| The JavaScript family's product sources | 22 | 1,119 | 5,737 | −4,618 |
| Other product sources (the slice compiler's two checks of section 4) | 1 | 57 | 0 | +57 |
| Tests (the architecture tests: rule I5's test reads the adapters' files, and the review record counts three files fewer) | 2 | 16 | 4 | +12 |

`JsTemporalCore.cs` (723 lines), `JsCalendars.cs` (942) and `JsTemporal.CalendarNonIso.cs` (541) went whole, and
`JsTemporalParser.cs` lost 1,129 lines and gained 73. The family's figures are JSD-0079 section 8's. No generated file
changed.

## 13. The Broiler.VM clauses

Each clause is judged as the milestone writes it, and parts A to C as [`decision-rule-area.md`](decision-rule-area.md)
writes them, with no revision. The milestone does not number its clauses, so each is named here by its words; its
second clause is read in three rows, one for each part of the rule that judges it.

| Clause | What it asks | Where | State |
|---|---|---|---|
| Rules | "Rules `I1` to `I7` hold." | section 11, [`after/rules.log`](after/rules.log) | **Met** at `96876432`: the whole suite passes, I1 to I7 among them. I1 admits both shared packages at `0.1.0-preview.6`; I3 reads the five image records; I5's register holds SJL-10.4's 147 |
| The area rule, part A | "test262 in both forms ... meet[s] the area rule" | section 6 | **Met** on run [39261526513](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39261526513) at `96876432` against the base's at `99c9c9f`: all four runs complete, and every changed verdict a (c') row, seventeen in the bytecode pair and one in the native; every row's re-runs agree but one's, which (e) admits with an exhaustion on fuel at both commits; no row in C1, so row R6 explains none |
| The area rule, part B | "... and the polyfill dataset meet the area rule. For Broiler.VM that is unchanged." | section 7 | **Met**: the six named checks print the base's lines, and no file under `src/tests/temporal/` differs between `99c9c9f` and `96876432` |
| The area rule, part C | the twins the rule judges under the same clause | section 8 | **Met**: every wide twin at its base figure with its answer, in both forms |
| Roots | "Every root is published and run in three modes." | section 11 | **Met** for every root this machine can publish, at `96876432`, with the packages as published; Android is EX-126 |

*(Revised 2026-10-19, at SJL-10.6, when part A's re-runs were taken. Part A's state read "**To be judged when the
hosted after run is imported** (39261526513)".)*

*(Note, 2026-10-19, later.)* The states above are the after arm's, at `96876432`, on `0.1.0-preview.6`. This family
has since moved to `0.1.0-preview.7`, which carries decision D12
([JSC-317](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-317)); section 4 says what was
run again with it. No part of the rule reads decision D12, and no state above changes.

**Also in this bundle, and no clause of their own:**
- the engine-level vectors at corpus revision 26, which match at the after commit as at `d625402` (section 10);
- the packages, every step as expected (section 11);
- the simplification check (section 12);
- EX-132, the code that left this component's assurance coverage (section 9).

Broiler.JS's clauses and the home's are judged in Broiler.JS's bundle of the same name, which this bundle does not
read. **None is accepted**: review is deferred, and SJL-10 may not meet its gate before SJL-6 and SJL-8 meet theirs.
