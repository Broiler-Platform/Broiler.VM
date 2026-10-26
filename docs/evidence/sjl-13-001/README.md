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
of 2026-10-25. Its input is [`correspondence.md`](correspondence.md), with
[`correspondence-runs/`](correspondence-runs/):
- `vm/` and `js/`: each engine's answer to every probe, its reports, logs and the programs a value probe ran;
- `probes/`: every probe's input, the plan of the runs and the script that ran them;
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
  progress output.

**Running the probes again.** `probes/run_all.sh` is kept as it ran, so it expects the layout it ran in, which its
hashed bytes cannot change: `SCRATCH` names a folder holding `sjl-rest/sjl13-corr/`, whose `probes/` is
`correspondence-runs/probes/` here, whose `runs/vm/` and `runs/js/` became `correspondence-runs/vm/` and `js/`, and
whose `runner-work/` is the scratch folder the runs call `<work>`; `VM_REPO` and `JS_REPO` name the clones and
`VMW` and `JSW` the worktrees to add; and the .NET SDK is the one at `/root/.dotnet`, which the script names. The
correspondence's section "The runs" lists every rewrite of the runs' bytes.

The rule's own inputs are [`inputs/fuzz-seeds.txt`](inputs/fuzz-seeds.txt), part G's seeds, and
[`inputs/workloads/`](inputs/workloads/README.md), part J's workloads, each with its control.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules only, 2026-10-26, at SJL-13.1. **Rules only. Nothing here is a verdict, and nothing has been taken.**
- Every run of the rule is taken at a commit the rule defines before it exists: the tools commit, the base commit, the
  registry commit and the after commit (JSD-0083 decisions D27 and D35).
- A collector joins this bundle when there is a run to take.

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
review." JSD-0083 records decisions D2 to D39 under it. It changes no programme rule: nothing is accepted without
review, rules still come before runs, and SJL-13 may not meet its gate before SJL-12, SJL-3 and SJL-7 meet theirs.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The tools commit**, **the base commit**, **the registry commit** and **the after commit** do not exist yet. The
  rule states the conditions each must meet, and this README names each, by all forty digits, when it lands. The base
  commit descends from the rule commit, from the tools commit and from the commit bundle `sjl-12-001` names as its
  after commit here, `1de3af6`; the registry commit is the first commit of the branch step 1's merge lands, directly on
  the base.

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
- **`inputs/`** and **`correspondence-runs/`** stay as committed with the rule.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-13.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
