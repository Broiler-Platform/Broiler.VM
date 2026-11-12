<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-11-001 - regular expressions: Broiler.VM's rules, correspondence and evidence

**Milestone:** SJL-11 of the shared JavaScript libraries programme,
[`sjl-11-regexp.md`](../../javascript-shared-libraries/milestones/sjl-11-regexp.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-11.1 before any change it judges existed.

*(Added 2026-11-10.)* The rule is revised, before any of its runs, by
[`decision-rule-area-revised-2026-11-10.md`](decision-rule-area-revised-2026-11-10.md), filed with SJL-13's step 1's
records as JSD-0083's decision D36 asks when step 1 lands before SJL-11.4. Step 1 deletes the binder's `CheckRegExp`,
and the shared syntax library asks this family's regular-expression validator, `JsRegularExpressionValidator`, which
makes the same checks, as its request's provider; so part B's "no other class" and the header's item on part B name
that provider where they named `CheckRegExp`, and SJL-11.4 re-points the provider. Nothing else of the rule changes,
and `decision-rule-area.md` is not edited. The revision also names two sentences of the rule that no longer describe
this family after step 1 and that it leaves as written: part A's reading of row S9b's file, which step 1's row T33
corrects, and part A's hold on the evidence workflow, which the owner changed on 2026-10-30.

**Records:** [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
the area verdict, which records decision SJL-D-14 as this area applies it and the owner's decisions D1 to D41. Its
input is [`correspondence.md`](correspondence.md), with every probe and each engine's answers in
[`correspondence-runs/`](correspondence-runs/): `vm/` for this family and `js/` for Broiler.JS and for Broiler.Regex
`0.1.0-preview.4` called directly, each with a `hashes.txt` that `sha256sum -c` verifies.

**Inputs, committed with the rule:**
- [`inputs/adversarial/`](inputs/adversarial/): part D's programs, one per case, and `manifest.tsv`, written by
  Broiler.RegEx's `eng/adversarial-programs.py --engine broiler-vm` from that repository's
  `tests/Adversarial/corpus.tsv`. The manifest's header names the corpus's SHA-256,
  `1a0f8c56bb970fa784fc6e573e9968e939d298d3e912320ed8c1a407fa997dd5`, and the Broiler.RegEx commit both were read at,
  `c2aa77167cb3ded40ef50b5c3eeae26f8f7c0b54`, on its branch for SJL-11.1. Two runs of the generator there wrote
  byte-identical files, and these are a byte-for-byte copy of them. Nothing in the folder is edited here; a change of
  the corpus is a revision under the rule's "Revisions" section, and the manifest then names another SHA-256.
- [`inputs/workloads/`](inputs/workloads/README.md): part E's six workloads and six controls, with their README.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules and correspondence, 2026-10-25, at SJL-11.1; this family's engine-level runs of the
regular-expression vectors from scratch archives at corpus revisions 31 and 32, 2026-11-01, at SJL-11.3; and its run
on its pin at revision 32, 2026-11-02, at SJL-11's task 3.5 (section 3). **Nothing here is a verdict, and no run of
the rule has been taken.** *(Revised 2026-11-02, when the pin moved to revision 32. It read "rules and correspondence
only, 2026-10-25, at SJL-11.1. **Nothing here is a verdict, and nothing has been taken.**", which SJL-11.3's runs
should have moved.)*
- Every base run is taken at the admission commit, the first commit of the branch that lands SJL-11.4, which admits
  `regexp-duplicate-named-groups` and changes nothing else outside `docs/` (JSD-0081 decisions D15 and D16).
- The after runs are taken at the default-branch merge that lands SJL-11.4, whose first parent must be the admission
  commit's parent.
- A collector joins this bundle when there is a run of the rule to take. Section 3's runs are no runs of the rule,
  and were taken without one. *(Revised 2026-11-02. It read "A collector joins this bundle when there is a run to
  take.")*

Broiler.JS's clauses are judged in Broiler.JS's bundle of the same name, and the component's in Broiler.RegEx's.
This bundle reads none of their runs.

## 1. SJL-11's area rule and its correspondence (SJL-11.1)

**The correspondence.**
- It was read from code at this family's `aaa39fb`, Broiler.JS's `1f91b8f1` and Broiler.Regex's `c1422f4d`, the
  commit `0.1.0-preview.4` was built from, on 2026-10-21, and every behavioural cell was run: this family through the
  conformance root, and four cases also through the command-line root; Broiler.JS through its shell, the
  case-insensitive rows again under `tr-TR` and `lt-LT`; and the package itself where the shell's fallback hides its
  answer. Its "Revisions" section reads the default branches' later moves, among them this family's `daa92eb`, where
  the literal's check moved into the binder's `CheckRegExp` unchanged.
- **Every row is classified**, and JSD-0081 decided the classes. This family's corrections are row S1 (kind D,
  duplicate named groups) and rows S5, S10, M4 (a non-empty match), M7 and T3 (kind R). Row M4's empty match is a
  defect of the specification's text, implementation-defined by the owner's decision D41 until the text is corrected.
  *(Note of 2026-11-01, the owner's decision D-s5, JSD-0081 "The owner's decisions of 2026-11-01": S5 leaves this
  list. `regexpref` reads `\k` in a class without `u` and `v` as B.1.2's identity escape, so this family's acceptance
  of `[\k<a>](?<a>x)` is the text's answer, and the kind R rows are S10, M4, M7 and T3. The correspondence's row S5
  carries the dated note.)*
- **A stop at one of this family's matcher ceilings names no dimension.** The conformance root reports it as a
  failure whose detail is `ProfileFault/AllowanceExhausted` with no payload, and the command-line root exits with its
  "host defect" code and writes the same detail to standard error, behind its prefix `broiler-js: <path>: ` (rows B1
  and B6). The rule reads that detail, and only it, as a ceiling stop.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0081 records decisions D1 to D41 under it. It changes no programme rule: nothing is accepted without
review, rules still come before runs, and SJL-11 may not meet its gate before SJL-7 meets SJL-7's, and so not before
SJL-3 meets SJL-3's.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The admission commit** and **the after commit** do not exist yet. The rule states the conditions each must meet,
  and this README names each when it lands. The admission commit descends from the rule commit and from bundle
  `sjl-12-001`'s corpus commit, `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`.

**The rule has five parts**, each deciding alone.
- **Part A** takes test262 in both forms with the hosted workflow, at the admission commit and at the after commit
  (template T1).
  - Its corrections are of two kinds, both named in JSD-0081 before any run: kind D, a file of
    `regexp-duplicate-named-groups`, and kind R, a variant one of rows S5, S10, M4, M7 and T3 explains. The test262
    files the correspondence names for rows S10, M7 and T3 are listed in the rule. Any other correction is recorded as
    a finding that the correspondence missed.
  - *(Noted 2026-11-01, decision D-s5.)* **The rule's part A and part B still name S5 as a C1 (R) correction**, the
    class case "accepted at the base" (part A) and "accepted at the base and refused after" (part B). Read with
    decision D-s5, the base's acceptance is the specification's answer, so a move from acceptance to refusal at SJL-11.4
    would be away from the text, and no variant of test262 at `ccaac100` tries the case. The rule file is unchanged,
    as its "Revisions" requires. The dated rule file this section already asks for, before any base run, is where the
    owner can drop S5 from both lists.
  - A moved exhaustion verdict, a ceiling stop among them, is admitted under C2 only on a variant that reaches a
    regular expression, and only if the variant passes after, at ten times the allowance where it does not pass at
    the part's. A variant failing at both commits is read by its detail too, so a ceiling stop that becomes a wrong
    answer, or the reverse, is a changed verdict.
  - Class C3 holds row M4's empty match alone, by decision D41.
  - A wall-clock race is admitted under C4's (c') and (e). There is no class C5.
  - *(Noted 2026-11-01.)* Its configuration holds `.github/workflows/test262-evidence.yml` and
    `eng/test262-evidence.py` at the dispatched ref byte-identical to the rule's commit. The owner changed the
    workflow on 2026-10-30, in `9d0f80e`, by a decision of the owner's own, which
    [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md)'s
    note under its decision D37 records: each of its three jobs' `runs-on: ubuntu-latest` reads
    `runs-on: ubuntu-24.04`, and the driver did not change. So the workflow at the default branch is no longer
    byte-identical to the rule's commit, and part A cannot be taken as the rule states. The rule file is unchanged, as
    its "Revisions" requires; no run of part A is taken, and a new dated rule file here, written before any of them,
    is what can admit the change. The admission commit's parent is the default branch's head when SJL-11.4's
    branch is made, so it descends from `9d0f80e`; a rule holding the workflow byte-identical to that commit's
    would have both runs use the `ubuntu-24.04` workflow. That the base and after runs are dispatched together
    after SJL-11.4's merge is the programme plan's practice, which no rule fixes, and neither fixes one image
    release: the runners of one dispatch have run two releases of one image (bundle
    [`sjl-6-001`](../sjl-6-001/README.md)).
- **Part B** holds the compiler's literal refusals over bundle `sjl-12-001`'s parse corpus, admitting only row S1's
  duplicate names and rows S5's and S10's literals (template T6's reading).
- **Part C** holds the wide twins to their base figures, with no class: no twin reaches a regular expression
  (template T7).
- **Part D** runs the adversarial programs of [`inputs/adversarial/`](inputs/adversarial/) in both forms through the
  command-line root, each against its declared outcome and against the base. It retains each process's standard
  output and standard error, reads its verdict from its exit code and its standard-error line, the ceiling stop among
  them, and admits only the duplicate-name cases and a move between fuel and a ceiling. Its control patches the
  adapter's mapping of `LimitExceeded` and runs over the M cases, the only cases that mapping reaches; a copy of the
  report with the other classes' cases changed shows the comparison reporting them. This is validation section 7's
  control for SJL-11.
- **Part E** is this family's measurement over the workloads of [`inputs/workloads/`](inputs/workloads/README.md)
  (template T5), retained in its own sub-bundle. It is not a gate clause (decision D19).

**The pins do not move**: test262 `ccaac100`, the language edition ES2026 at `tc39/ecma262@0248456c`, this family's
nesting limit 512 (decision D7), and its frame and trail ceilings `1 << 20` and `1 << 21` (decision D10).

## 2. What this bundle will hold

- **Before any base run**: part A's table script, which reads both reports' details, and part D's comparison script,
  each committed to the bundle before the base run its part reads is taken.
- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's base report and its
  refused-input lists; part C's `fuel-parity.log`; part D's base runs in both forms; all at the admission commit.
- **`after/`**: the same at the after commit, with part A's comparisons, its table of C1, C2 and C3 rows, its C2
  re-runs and its (c') and (e) re-runs, part B's comparison, and part D's comparison output.
- **`controls/`**: part B's control report, part D's patch K1 with its output and its revert, and its report copy K2
  with the comparison's output.
- **`measurement/`**: part E's sub-bundle, with its own README and manifest.
- **`vectors/`**: this family's runs of the regular-expression vectors SJL-11.3 adds. *(2026-11-01: its first runs,
  on corpus revisions 31 and 32 from scratch archives, are there; section 3 reads them.)* *(2026-11-02: its run on its
  pin at revision 32, with its negative control and the comparison with the first run on revision 32, is there too.)*

The differential cross-check's report over `src/tests/differential/the-regexp-*.js` is retained beside the parts
without a rule (JSD-0081 decision D38).

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 3. Engine-level vectors on the regular-expression area (SJL-11.3)

*(Added 2026-11-01, at SJL-11.3; the runs ended after midnight UTC, on 2026-11-02, as their logs say.)*

Broiler.JS's corpus revision 31 adds the regular-expression area, eight files under `conformance/vectors/regexp/`.
Every expected value but those of a few object-model vectors is `regexpref`'s (`ref:regexpref@1`, JSD-0081 decision
D25), ECMA-262 22.2 with B.1.2 at `0248456c`, transliterated. Revision 32 classifies both engines' first runs on it,
on Broiler.JS's branch `sjl-11-3-regexp` (observed, not merged). *(Note of 2026-11-02: both revisions merged in
Broiler-Platform/Broiler.JS#1089 as `b2c2e2243a086285d806866a64b83297791a8ec5`, observed.)* Each difference is under a row of this bundle's
[correspondence](correspondence.md), several by the owner's decisions of 2026-11-01. They are D-s5, D-p5 and D-widen,
which [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md)
records in "The owner's decisions of 2026-11-01".
- **This family's `divergence.broiler-vm` fields are its corrections** that SJL-11.4 makes: rows S1 (kind D), S10, M4
  (a non-empty match), M7 and T3 (kind R). Row S9b's two are the exception, its scanner's, outside SJL-11. No
  vector differs in both engines.
- **Row S5's class case matches**, `[\k<a>](?<a>x)` and `[\k](?<a>x)` accepted. By decision D-s5 that is the text's
  answer, and not a correction this family makes.
- **The status vectors.** The area's `status: exhausted` vectors are library-level. The conformance root's
  `VectorsCommand` skips each one, gives it no report line, and counts and names it in its summary line, as the corpus's
  SCHEMA.md section 4 has an engine-level runner do. This change adds that skip, with harness checks run before every
  shard (`VectorsChecks`). The skip's first check failed with the skip disabled
  ([`vectors/harness-checks-skip-disabled.log`](vectors/harness-checks-skip-disabled.log)), and every check passes
  with it.
- **Broiler.JS's part D** is that engine's, and this bundle reads none of it. Its population leaves out the status
  vectors by the owner's decision D-partd, which JSD-0081 records. This bundle's own part D is its adversarial corpus.
- **The vectors pin does not move here.** It stays at corpus revision 30 until SJL-11's task 3.5 moves it, after
  Broiler.JS merges revision 32 (rule N13), and `SharedVectorsPinTests` still pass on it. *(Note of 2026-11-02: task
  3.5 moved it to revision 32, below.)*

**The runs.** Each was taken at `89760389`, this change's runner commit *(note of 2026-11-02: this change merged in
Broiler-Platform/Broiler.VM#180 as `b43d0bb2ab2e827c754c97bce8024cb717e7ceef`, its runner commit
`897603890da4aa6d1421c01d23c87009da495d6f` among its commits)*, from a scratch archive of the Broiler.JS commit
the run names. The archive was written twice by `git archive`, byte for byte the same, and the second was extracted and
hashed for a scratch pin. Neither archive nor pin is committed; the pins are kept here for the reader. The conformance
root was built in Release with `--disable-build-servers` and `-warnaserror`
([`vectors/build-conformance.log`](vectors/build-conformance.log)). Each run used `TZ=UTC`, no `LANG`, `LC_ALL` or
`LANGUAGE`, and a script that wrote each log whole: a header naming the commit, the build, the archive and its pin,
the host and the command; then the runner's output; then the exit code and the end time. Each run's negative control
is a second run with `--mutate regexp/text/t5-escape/0001`, a row T5 vector both engines match.
[`vectors/hashes.txt`](vectors/hashes.txt) holds every file's SHA-256. The logs' headers name the pin and the archive
as the run's scratch folder named them, the names Broiler.JS's bundle `engine/` keeps: `broiler-vm-revision-31.pin`
and `broiler-vm-revision-32.pin` are this bundle's `scratch-pin-revision-31.pin` and `scratch-pin-revision-32.pin`,
byte for byte, and `archive-2.tar.gz` is the second of the two scratch archives written of each run's corpus commit,
the one extracted and hashed. The logs are kept as written, since their hashes are recorded.

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 31, the first run | `d8cb8406`, scratch pin [`scratch-pin-revision-31.pin`](vectors/scratch-pin-revision-31.pin) | 63 unclassified differences, all of the regular-expression area, rows S1 (33), S10 (14), M4 (7), M7 (5), T3 (2) and S9b (2); the other areas' classified divergences as at revision 30; the status vectors skipped and named | [`run-revision-31.log`](vectors/run-revision-31.log), [`run-revision-31.tsv.gz`](vectors/run-revision-31.tsv.gz) |
| Its negative control | the same | the same, and the mutated vector one more unclassified difference | [`run-negative-control-revision-31.log`](vectors/run-negative-control-revision-31.log), [`run-negative-control-revision-31.tsv.gz`](vectors/run-negative-control-revision-31.tsv.gz) |
| Revision 32 | `cfe32852`, scratch pin [`scratch-pin-revision-32.pin`](vectors/scratch-pin-revision-32.pin) | 0 unclassified differences: the 63 vectors above reported as classified divergences, beside the 63 of the syntax and Intl areas, and no divergence that no longer holds; the status vectors skipped and named | [`run-revision-32.log`](vectors/run-revision-32.log), [`run-revision-32.tsv.gz`](vectors/run-revision-32.tsv.gz) |
| Its negative control | the same | the same, and the mutated vector the one unclassified difference | [`run-negative-control-revision-32.log`](vectors/run-negative-control-revision-32.log), [`run-negative-control-revision-32.tsv.gz`](vectors/run-negative-control-revision-32.tsv.gz) |

**The same runs are retained in Broiler.JS's bundle `sjl-11-001`**, in its `engine/`, as the basis of its fields, with
Broiler.JS's own runs on both revisions.

### The pin at revision 32 (SJL-11's task 3.5)

*(Added 2026-11-02.)* Broiler-Platform/Broiler.JS#1089 merged both revisions into Broiler.JS's default branch as
`b2c2e224` (observed). Its corpus differs from that of `cfe32852`, from which the run on revision 32 above was taken,
only in the reasons of 74 `divergence.broiler-js` fields, SCHEMA.md section 4's dated note and the revision log's row
32, which review reworded in `6f7d5651` and `de8d7b54`: no `out`, class, `per-engine` or `divergence.broiler-vm` line
changed.

**The pin** is at revision 32 since `a4ce976`, and was at revision 30 from `6c6d9bf` (SJL-13). Its archive was taken
twice, byte-identical, and its files hashed from an extracted copy; the same method, applied to revision 30's commit,
`ed494b2e`, gives that pin's two digests and its count exactly, and applied to `cfe32852` it gives
[`vectors/scratch-pin-revision-32.pin`](vectors/scratch-pin-revision-32.pin)'s. Against revision 30 the archive adds the
regular-expression area's eight files and changes the corpus's README, revision log and schema; every other file is
revision 30's byte for byte.

**The run.** The conformance root was built in Release with `--disable-build-servers` in the clean tree of `a4ce976`
([`vectors/build-conformance-pin.log`](vectors/build-conformance-pin.log)), whose product code is `b43d0bb`'s: the
change touches only the pin's folder and the notices. It was run there on the committed pin under `TZ=UTC`, with no
`LANG`, `LANGUAGE` or `LC_ALL`, by a script that wrote each log whole: a header naming the commit, the build, the pin
and its archive with their SHA-256, the host and the command; then the runner's output; then the exit code and the end
time. The report was written outside the tree, which stayed clean before and after each run, and is kept here
gzip-compressed; the logs name the reports by the names the run's scratch folder gave them, `run-revision-32.tsv` and
`run-negative-control-revision-32.tsv`, which are here as `run-pin-revision-32.tsv.gz` and
`run-negative-control-pin-revision-32.tsv.gz`. The negative control is the one above, `--mutate
regexp/text/t5-escape/0001`. [`vectors/hashes.txt`](vectors/hashes.txt) holds every file's SHA-256 and `sha256sum -c`
verifies it.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 32 | At `a4ce976`, clean, on the committed pin, from 07:39Z on 2026-11-02 | 108,759 vectors: 108,633 match, 126 classified divergences and 0 unclassified differences, and no divergence that no longer holds; the 18 status vectors skipped and named in the summary line, and given no report line | [`run-pin-revision-32.log`](vectors/run-pin-revision-32.log), [`run-pin-revision-32.tsv.gz`](vectors/run-pin-revision-32.tsv.gz) |
| Its negative control | The same, with `--mutate regexp/text/t5-escape/0001` | 108,632 match, the same 126 classified divergences, and 1 unclassified difference, the mutated vector; the same 18 skipped | [`run-negative-control-pin-revision-32.log`](vectors/run-negative-control-pin-revision-32.log), [`run-negative-control-pin-revision-32.tsv.gz`](vectors/run-negative-control-pin-revision-32.tsv.gz) |

**What it shows** ([`vectors/comparison-pin-revision-32.txt`](vectors/comparison-pin-revision-32.txt)):
- **The 126 classified divergences are exactly the vectors that carry a `divergence.broiler-vm` field** in the pinned
  archive: the regular-expression area's 63, of rows S1 (33), S10 (14), M4 (7), M7 (5), T3 (2) and S9b (2), the syntax
  area's 22 and the Intl area's 41, as at revision 30. Every other vector matches.
- **The 18 vectors the summary line names as skipped are exactly those whose `out` is a status** in the pinned archive,
  and none of them has a report row.
- **Every row of the report is the same, byte for byte**, as in the run on revision 32 above, `run-revision-32.tsv.gz`,
  which Broiler.JS's bundle `sjl-11-001` retains byte for byte as `engine/broiler-vm-run-revision-32.tsv.gz`, and so in
  the negative controls. The reports' headers differ in the corpus commit and its content digest alone, as the two
  corpora differ in Broiler.JS's reasons alone, which this family's report does not carry.
- **The negative control differs from the run in the mutated vector's row alone.**

So the conformance root on the pin leaves no vector unclassified, which is task 3.5's check. The run is no part of
the area rule, and nothing in it is a verdict.

*(Noted 2026-11-12, at SJL-13's work package 9.4, when this family's vectors pin moved to corpus revision 39.)* This
section's first bullet reads "Row S9b's two are the exception, its scanner's, outside SJL-11.", and the runs above
count "S9b (2)" among the regular-expression area's differences and classified divergences. Step 1 of SJL-13.3, this
family's wide path on the shared syntax library (Broiler-Platform/Broiler.VM#196, merged as `a8d8398`), corrects row
S9b: the library delimits a literal that ends in a backslash at the end of the source, bundle
[`sjl-13-001`](../sjl-13-001/README.md)'s row T33, and that bundle's section 5 found row S9b's two vectors matching
with their fields no longer holding. Broiler.JS's corpus revision 39, merged in Broiler-Platform/Broiler.JS#1116 as
`e4b6b49ed290983e02525ca1a9318fd46517dc2c` (observed), retires both fields,
`regexp/literals/s9b-literal-grammar/0001` and `0002`, and this family's pin moved to it in `b781ebb`, where the
conformance root reports both as a plain match. So row S9b is now a plain match, and no longer an exception. This
family's `divergence.broiler-vm` fields of the area that remain are its corrections that SJL-11.4 makes, the 61 of rows
S1 (33), S10 (14), M4 (7), M7 (5) and T3 (2), each the field revision 32 wrote; no other field of the area moved for
this family. The bullet and the runs above, of revisions 31 and 32, are left as written, and no rule is revised.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken (section 3 since 2026-11-01). The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-11.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
