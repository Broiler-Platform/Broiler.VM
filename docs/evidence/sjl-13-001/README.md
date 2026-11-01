<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-001 - the shared syntax library and the wide path, step 1: Broiler.VM's rules, correspondence and evidence

**Milestone:** SJL-13 of the shared JavaScript libraries programme,
[`sjl-13-shared-syntax-and-vm.md`](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md), at
the first of SJL-13.3's two cut-over steps. Bundle [`sjl-13-002`](../sjl-13-002/README.md) judges the second.

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-13.1 before any change it judges existed: no
shared library, no tools commit, no registry revision, no admission, no adapter and no deletion.

**Records:** [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
the area verdict, which records the owner's decisions D2 to D39 of 2026-10-26, with D1 a reading of the owner's words
of 2026-10-25, and *(added 2026-10-27)* the owner's decisions D-asi and D-t24 of 2026-10-27, which add row P16b to
the correspondence and the answer of rows T24b and T24f where `;` or the end of input follows the literal. Its input
is [`correspondence.md`](correspondence.md), with
[`correspondence-runs/`](correspondence-runs/):
- `vm/` and `js/`: each engine's answer to every probe, its reports, logs and the programs a value probe ran;
- `probes/`: every probe's input, the plan of the runs and the script that ran them *(added 2026-10-27: with the
  probes of that day, their plan `plan-p16b.tsv` and the script that ran them, `run_p16b.sh`)*;
- `analysis/`: the derived tables the rows cite, among them the anchor lists decision D15 reads
  (`jsparser-anchors.tsv`, `binder-anchors.tsv`) and the corpus joins Broiler.JS's classifier pins by digest
  (`rows-P-baseline.tsv.gz`, `rows-P-agree.tsv.gz`).

Each of those four folders carries a `hashes.txt` that `sha256sum -c` verifies. [`.gitattributes`](.gitattributes)
keeps every byte under `correspondence-runs/` as hashed. **Some of its files are exceptions to the ledger's section 3**
("Text files are stored with LF line endings before they are hashed.") and keep the bytes they were hashed with:
- on purpose, the probe inputs whose rows test line terminators, under `probes/N/inputs/`: `crlf-lines.js`,
  `continuation-crlf.js` and `comment-lines.js` (CR LF), `lone-cr-lines.js` (a lone CR) and `template-lines.js` (CR LF
  and LF);
- as run, the worktree logs `vm/logs/worktree.log` and `js/logs/worktree.log`, whose carriage returns are git's
  progress output, and *(added 2026-10-27)* `vm/logs/p16b-worktree.log` and `js/logs/p16b-worktree.log`, the same for
  the probes of 2026-10-27.

**Running the probes again.** `probes/run_all.sh` is kept as it ran, so it expects the layout it ran in, which its
hashed bytes cannot change: `SCRATCH` names a folder holding `sjl-rest/sjl13-corr/`, whose `probes/` is
`correspondence-runs/probes/` here, whose `runs/vm/` and `runs/js/` became `correspondence-runs/vm/` and `js/`, and
whose `runner-work/` is the scratch folder the runs call `<work>`; `VM_REPO` and `JS_REPO` name the clones and
`VMW` and `JSW` the worktrees to add; and the .NET SDK is the one at `/root/.dotnet`, which the script names. The
correspondence's section "The runs" lists every rewrite of the runs' bytes. *(Added 2026-10-27.)* `probes/run_p16b.sh`
runs the probes of 2026-10-27 the same way, from a folder `sjl-rest/sjl13-corr-p16b/` under `SCRATCH` whose `probes/`
is a copy of `correspondence-runs/probes/`; its answers joined `vm/` and `js/` as new files, and the `hashes.txt` of
`probes/`, `vm/` and `js/` were written again over every file, each earlier line unchanged.

The rule's own inputs are [`inputs/fuzz-seeds.txt`](inputs/fuzz-seeds.txt), part G's seeds, and
[`inputs/workloads/`](inputs/workloads/README.md), part J's workloads, each with its control.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules, 2026-10-26, at SJL-13.1, and this family's engine-level run of the vectors on its pin at corpus
revision 30, 2026-11-01, at SJL-13.2's `syntax` vectors (section 3). **Nothing here is a verdict, and no run of the
rule has been taken.** *(Revised 2026-11-01, when the pin moved to revision 30. It read "rules only, 2026-10-26, at
SJL-13.1. **Rules only. Nothing here is a verdict, and nothing has been taken.**")*
- Every run of the rule is taken at a commit the rule defines before it exists: the tools commit, the base commit, the
  registry commit and the after commit (JSD-0083 decisions D27 and D35).
- A collector joins this bundle when there is a run of the rule to take. Section 3's run is no run of the rule, and
  was taken without one. *(Revised 2026-11-01. It read "A collector joins this bundle when there is a run to take.")*

The home's clauses 1 to 3 are judged in Broiler.JS's bundle of the same name, under its own rule, which this bundle
does not read.

## 1. SJL-13's area rule for step 1, and its correspondence (SJL-13.1)

**The correspondence.**
- It was read from code at this family's `4cf9ca4` and Broiler.JS's `d6795fc8`, on 2026-10-26, and every row whose
  answer bundle `sjl-12-001`'s after baselines did not already hold was probed in both engines, from one Release
  build of each in a scratch worktree, serially. Broiler.JS's default branch then moved to `8a95076c`, and the
  correspondence says why no row's answer can differ there.
- **Every row is classified**, in one of seven classes, and no row's class is unknown. JSD-0083 records the classes.
- **This family's corrections that step 1 makes are named before any run**: rows T6, T7, T8, T13, T16, T24d to T24f,
  P05a, P15b, P31, D32 and D40. Part A's kind (F) and part B's class F name exactly these.
- **The findings against the plan and the records**, F1 to F32, are each answered in JSD-0083, by a decision of
  2026-10-26 or by the record named to carry it.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0083 records decisions D2 to D39 under it, and *(added 2026-10-27)* decisions D-asi and D-t24 of
2026-10-27. It changes no programme rule: nothing is accepted without
review, rules still come before runs, and SJL-13 may not meet its gate before SJL-12, SJL-3 and SJL-7 meet theirs.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged. *(Note of 2026-11-01: that change, the SJL-13.1 records,
  merged in Broiler-Platform/Broiler.VM#172 as `faa8af78cddc32d460a7299f088b18b9e4407d7a`, the rule commit; this
  section did not name it then, as it should have.)*
- **The tools commit** is `58b0c84edb20ccbfc386a40e2566ee29b2a55695`, the default-branch merge of
  Broiler-Platform/Broiler.VM#174, which adds the two wide-path rows, `--fuzz-wide`, `--measure-compile`,
  sub-population B5, rule N19's widened pattern and the mapping checker's revision-19 checks. Of the rule's three
  conditions, read here and not judged: it descends from the rule commit; `git diff --name-only` between its first
  parent, `b95a86a`, and it names only `.github/workflows/broiler-vm-lane.yml`, `eng/check-frontend-mapping.py` and its
  control files under `eng/check-frontend-mapping-controls/`, six files under
  `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`, `CompilationStackRuleTests.cs`,
  `rules.register.json` and N19's witness file; and whether it is an ancestor of the base commit is read when the base
  exists. *(Added 2026-11-01. It merged on 2026-10-27, and this section did not name it then, as it should have.)*
- **The correspondence's revision of 2026-10-27**, row P16b and the answer of rows T24b and T24f before a semicolon,
  with JSD-0083's records of decisions D-asi and D-t24, merged in Broiler-Platform/Broiler.VM#175 as
  `9813d7ca83e0ef43238652984294ae6e1d6e4028`. It is none of the commits the rule names. *(Added 2026-11-01.)*
- **The base commit**, **the registry commit** and **the after commit** do not exist yet. The rule states the
  conditions each must meet, and this README names each, by all forty digits, when it lands. *(Revised 2026-11-01. It
  began "**The tools commit**, **the base commit**, **the registry commit** and **the after commit** do not exist
  yet.")* The base commit descends from the rule commit, from the tools commit and from the commit bundle `sjl-12-001`
  names as its after commit here, `1de3af6`; the registry commit is the first commit of the branch step 1's merge
  lands, directly on the base.

**The rule has ten parts**, each deciding alone.
- **Part A**: test262 in both forms with the hosted workflow, at the base commit and at the after commit (template
  T1). Its C1 kinds are named before any run: (R) the regrade of the three language rules coded 2104, (G) the
  decorator's stage, (B) the admission walk and (F) the corrections step 1 makes, by row; C3 for the left-operand
  charge, in the four shapes row R14 names; no C2 and no C5.
- **Part B**: artifact identity over the parse corpus, sub-populations B1 to B5, at the base, the registry commit and
  the after commit (template T6), with classes R, R2, S (S1 and S3 to S8), F and L, each named before any run.
- **Part C**: determinism, and the source front end's checks rows, the two the tools commit adds and the one step 1
  adds (layer L13).
- **Part D**: the front end's rules N5 to N9, N12 and N19, I1 to I7, K4, A2, A7, B3 and N37, with rule I5 registering
  every member step 1 deletes, the binder's checks of `syntax` rows member by member.
- **Part E**: the registry at revision 19 and its mapping, through the mapping checker's revision-19 checks.
- **Part F**: the shared and slice tokenizers agree on every input the slice path accepts, by a field map the rule
  fixes, with no class.
- **Part G**: source fuzzing, the wide session and the slice session, over [`inputs/fuzz-seeds.txt`](inputs/fuzz-seeds.txt)
  (layer L9).
- **Part H**: the `syntax` vectors at engine level, at the corpus revision this family's pin names at the base
  (template T3).
- **Part I**: the fuel-parity twins (template T7).
- **Part J**: this family's compile measurement over [`inputs/workloads/`](inputs/workloads/README.md) (template T5).
  It is not a gate clause (JSD-0083 decision D24), and its runs are retained in a sub-bundle of their own.

**The pins do not move**: test262 `ccaac100`; ECMA-262 ES2026 at `tc39/ecma262@0248456c`; the parse corpus's manifest
at SHA-256 `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc`; the early-error catalogue at SHA-256
`7a9109b0db685df98a7327cd95b9337cc538337f834fe4f22692b249058aa4f1`. The registry is at revision 18 until step 1's
registry commit mints revision 19 (JSD-0083 decision D12).

## 2. What this bundle will hold

- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's runs at the base;
  parts C, D and G at the base, read and not judged; part H's vector run; part I's `fuel-parity.log` with each twin's
  bisected allowance.
- **`registry/`**: part B's run at the registry commit, which attributes every renumbering to revision 19 before the
  parse changes.
- **`after/`**: the same at the after commit, with part A's comparisons, its table of C1 and C3 rows with their first
  diagnostics at both commits, and its (c') and (e) re-runs; part B's comparison and its lists by class; parts C to I
  judged; part E's checker run.
- **`controls/`**: every control the rule defines, each with its output: part B's planted differences, part C's K1
  to K4, part D's I5 controls and the pristine consumer without the frontend on its feed, part E's three copies, part
  F's shifted kind, part G's planted throws and part H's changed vector.
- **`measurement/`**: part J's sub-bundle, with its own README and manifest.
- **`vectors/`**: this family's run of the `syntax` vectors at the corpus revision its pin names, outside the rule
  (section 3). *(Added 2026-11-01.)*
- **`inputs/`** and **`correspondence-runs/`** stay as committed with the rule.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 3. Engine-level vectors on the `syntax` area (SJL-13.2's vectors)

Broiler.JS's corpus revision 29 adds the `syntax` area, six files under `conformance/vectors/syntax/`, in schema 1
with no new key (JSD-0083 decision D25). Revision 30 classifies both engines' first runs on it, corrects the vectors
those runs and review showed to be defective, and adds the delimiting vectors review found missing. Both revisions are
on Broiler.JS's default branch since Broiler-Platform/Broiler.JS#1082 merged as `ed494b2e`, whose corpus is that of
`dc9e09e5`, revision 30 as completed before the merge; nothing on that branch since, to `b22e7d6a`
(Broiler-Platform/Broiler.JS#1083), touches `conformance/`.
- **Where the expected values come from.** ECMA-262 ES2026 at `0248456c`, each vector citing the clause that decides
  it, and, for the explicit-resource-management vectors, ECMA-262 main at `4feb0272`; where a test262 test at
  `ccaac100` asserts the outcome, the vector names its path. Each source is written from the grammar. The
  `impl-defined` vectors are Annex B's, each engine's `per-engine` expectation the extension's outcome, since both
  engines' requests turn Annex B on for scripts (JSD-0083 decision D17).
- **This family's `divergence.broiler-vm` fields of the area are its corrections that step 1 makes** (JSD-0083,
  "The corrections step 1 makes"): rows T6, T7, T8, T13, T16, T24d to T24f, D32, P15b and P31. No field is a refusal
  of the wide manifest. Broiler.JS's fields are its own, the corrections SJL-14 answers and row T18's engine policy,
  and among them row P16b's, which the owner's decision D-asi of 2026-10-27 added to the correspondence (JSD-0083,
  "The owner's decisions of 2026-10-27").

**Where the first runs are.** This family's first runs on revisions 29 and 30 were taken for Broiler.JS's
classification, at `faa8af7`, the rule commit, from scratch archives and scratch pins, and Broiler.JS's bundle
`sjl-13-001` keeps them in its `engine/` as the basis of those fields. This section's run is on the committed pin, at
revision 30 as Broiler.JS's default branch carries it.

**The pin** is at revision 30 since `6c6d9bf`, and was at revision 28 from `da4c07c` (SJL-9.2). Its archive was taken
twice, byte-identical, and its files hashed from an extracted copy; the same method, applied to revision 28's commit,
`ff54e1d8`, gives that pin's two digests and its count exactly. Against revision 28 the archive adds the `syntax`
area's six files and changes the corpus's README and revision log; every other file is revision 28's byte for byte.

**The run.** The conformance root was built in Release with `--disable-build-servers` in the clean tree of `6c6d9bf`
([`vectors/build-conformance.log`](vectors/build-conformance.log)). Its product code is `faa8af7`'s: between the two,
only the slice compiler's root, the architecture tests, the tools and the lane workflow, the documents, the pin's
folder and the notices change, and the conformance root references neither of the first two. It was run there on the
committed pin under `TZ=UTC`, with no `LANG` and no `LANGUAGE`, by a script that wrote each log whole: a header naming
the commit, the build, the pin and its archive with their SHA-256, the host and the command; then the runner's output;
then the exit code and the end time. The report was written outside the tree, which stayed clean, and is kept here
gzip-compressed. A first run and its control were taken before these and set aside, because this README and the ledger
were edited in the tree while they ran, so their logs could not say the tree stayed clean; this bundle keeps neither
their logs nor their reports, and nothing here rests on them. The build's outputs were removed, and the build and both
runs taken again on the clean tree, whose logs say so before and after each run.
[`vectors/hashes.txt`](vectors/hashes.txt) holds every file's SHA-256, which `sha256sum -c` verifies.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 30 | At `6c6d9bf`, clean, on the committed pin, from 13:34Z on 2026-11-01 | 107,357 vectors: 107,294 match, 63 classified divergences and 0 unclassified differences, and no divergence that no longer holds | [`run-revision-30.log`](vectors/run-revision-30.log), [`run-revision-30.tsv.gz`](vectors/run-revision-30.tsv.gz) |
| Its negative control | The same, with `--mutate syntax/scripts/ex0008/0001` | 107,293 match, the same 63 classified divergences, and 1 unclassified difference, the mutated vector | [`run-negative-control-revision-30.log`](vectors/run-negative-control-revision-30.log), [`run-negative-control-revision-30.tsv.gz`](vectors/run-negative-control-revision-30.tsv.gz) |

**This family matches every vector of revision 30 or differs exactly as its `divergence.broiler-vm` field says.** The
63 classified divergences are the 63 vectors that carry such a field, and no other: 22 of the `syntax` area, rows T13
(4), T16 (3), T6, T8, D32, P15b, P31 and T24e (2 each), and T7, T24d and T24f (1 each), 21 `divergence` and one
`impl-defined`; and the Intl area's 41 of rows Z8, L9 and L18, as at revision 28 (bundle `sjl-9-001`, section 3). Each
is reported as a classified divergence, so none is unclassified and none no longer holds. So the vectors find no
difference of this family's that the correspondence does not classify: each field names the correspondence row that
holds its difference, as decision D25 of JSD-0083 has the `syntax` vectors carry each correction step 1 makes with that
row in its provenance.

**The run agrees with the one Broiler.JS's bundle retains** of this family on revision 30 at `faa8af7`, on the corpus
at `dc9e09e5` (its `engine/broiler-vm-run-revision-30.tsv.gz`), row for row and byte for byte:
[`vectors/comparison-revision-30.txt`](vectors/comparison-revision-30.txt) holds the comparison.
- Every row is the same whole, its id, class, expected value, rendering and verdict, the `syntax` area's and every
  other.
- The two reports' summary lines differ in the corpus commit alone, `dc9e09e5` there and `ed494b2e` here, with the
  same content digest, since the merge changes nothing under `conformance/vectors/`.
- The negative controls agree the same way: each reports the mutated vector, `syntax/scripts/ex0008/0001`, as its one
  unclassified difference, and every other row is the run's.
- The logs' host lines differ, and the retained one is wrong for its build: it says that .NET's globalization loads the
  host's ICU, but the conformance root's project file sets `InvariantGlobalization` at `faa8af7` as it does at `6c6d9bf`
  (the same line of `Broiler.VM.Composition.JavaScript.Conformance.csproj` at both), so that root, like this one, does
  not load it and reads its own CLDR 48.2.0 tables, as this log and bundle `sjl-9-001`'s logs say. The misdescription is
  the retained log's host line only, and Broiler.JS's bundle is that repository's to correct; no report row differs.

This section is no part of the rule's parts A to J, and nothing in it is a verdict. Part H reads the same area at the
base commit, at the revision the pin names there.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-13.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
