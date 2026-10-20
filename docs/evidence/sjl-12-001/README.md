<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-12-001 - the frontend contract and the wide path's in-place preparation: Broiler.VM's rules and evidence

**Milestone:** SJL-12 of the shared JavaScript libraries programme,
[`sjl-12-frontend-contract.md`](../../javascript-shared-libraries/milestones/sjl-12-frontend-contract.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-12.1 before any change it judges existed.

**Records:** [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
the contract record, which also records decisions SJL-D-11 and SJL-D-12 and the owner's decisions D1 to D42 of
2026-10-19.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules only, 2026-10-19, at SJL-12.1, and the readings SJL-12.4 names before parts B and C's base runs,
2026-10-20 (section 3). **Nothing here is a verdict, and nothing has been taken.**
- Part A's base runs are taken by the hosted workflow at the rule commit, before any product change of SJL-12.6
  lands.
- Parts B and C's base runs are taken at the corpus commit, which adds SJL-12.4's parse corpus and the slice
  compiler's `--parse-corpus` mode.
- The after runs follow the last of SJL-12.6's four items.
- A collector joins this bundle when there is a run to take.

**SJL-12 is not an area verdict.** It applies no extraction gate and files no X1 to X7 verdict. The frontend area's
verdict is SJL-13.1's (JSD-0080 decision D1).

Broiler.JS's clauses are judged in Broiler.JS's bundle of the same name. This bundle reads none of its runs.

## 1. SJL-12's rule (SJL-12.1)

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The corpus commit**, **the ordering commit** and **the after commit** do not exist yet. The rule states the
  conditions each must meet, and this README names each when it lands.

**The rule has five parts**, each deciding alone.
- **Part A** takes test262 in both forms with the hosted workflow, at the rule commit and at the after commit
  (template T1).
  - Its only named corrections are of two kinds, both named in JSD-0080 before any run: the template corrections of
    SJL-12.6 item 4, and a negative test's refusal that moves from the manifest's code to a language early error the
    binder pass reports first. Any other correction is recorded as a finding against the record.
  - A wall-clock race is admitted under C4's (c') and (e).
  - There is no class C2, C3 or C5: SJL-12 changes no charge, pin, data or policy.
- **Part B** compares the wide path's artifacts over the parse corpus by hash, at the corpus commit, the ordering
  commit and the after commit (template T6). It admits four classes and no other:
  - O, the ordering fix, which must already show at the ordering commit;
  - S, a refusal that moved stage with the same codes;
  - T, the template corrections;
  - P, a corrected position inside a template substitution.

  The slice front end and the construct census admit no difference.
- **Part C** holds determinism over two runs and two threads at the after commit, with two controls that must fail
  (layer L13).
- **Part D** runs rules N5 to N9, N12 and N19 at the after commit, with two checks rows for N19's property.
- **Part E** checks the diagnostic mapping against the early-error catalogue and the registry, with two controls the
  checker must refuse (validation section 7's control for SJL-12).

**The pins do not move**: test262 `ccaac100`, and the language edition ES2026 at `tc39/ecma262@0248456c`.

## 2. What this bundle will hold

- **`base/`**: every base run.
  - Part A's base dispatch, imported unchanged with the digest of its zip.
  - Part B's base report at the corpus commit, in every sub-population, variant and form, with the census.
  - Part C's three runs at the corpus commit, read and not judged.
  - Part D's rule run at the rule commit, read and not judged.
- **`corpus/`**: the corpus commit's identity, its manifest's digest, and the path list and ancestry the rule's
  three conditions read; and the copy of the early-error catalogue part E checks.
- **`after/`**: the reports at the ordering commit and at the after commit, the after dispatch, part C's runs, part
  D's rule run, part E's checker output, and the artifact bytes of every input whose hash differs between two runs.
- **`controls/`**: part B's control report, part C's patches K1 and K2 with their output and their reverts, and part
  E's two control copies of the mapping with the checker's refusals.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 3. Readings named before the base runs (SJL-12.4, 2026-10-20)

**Written 2026-10-20, at SJL-12.4**, with the parse corpus's copy and the slice compiler's `--parse-corpus` mode.
No run of part B or part C had been taken for this bundle. Nothing here revises the rule. Each reading says how a
record the mode writes is read under the rule's own words. The mode
(`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/ParseCorpus.cs`) states the same readings in
every report's header, and `eng/compare-parse-corpus.py` applies them.

### The corpus commit's three conditions

The corpus commit is the default-branch merge of the change that adds the copy and the mode. This section names it
by all forty digits when it lands. The change meets the three conditions as follows.
- **It descends from the rule commit.** The change's branch starts at the rule commit,
  `889ea1568f59bf37aa21c0035b423d0d030eb87f`. It took the default branch at
  `95a0b816a9c8030873c6a71e3ac320a7c90d6f8c`, which also descends from the rule commit.
- **It names only the listed paths.** Against the rule commit, the change adds or changes:
  - `src/tests/corpus/parse-1/`: the copy and its README;
  - `ParseCorpus.cs` and `Program.cs` under `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`;
  - `eng/compare-parse-corpus.py`;
  - this README.

  The merge of SJL-12.3, which the default branch held when the change took it, adds three things:
  `eng/check-frontend-mapping.py`, `src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv`, and
  files under `docs/evidence/sjl-12-001/`. Every one of these paths is under a path the condition lists. The
  condition reads the corpus commit's whole difference from the rule commit, so any other change merged onto the
  default branch before this one is held to the same list.
- **It comes before every product change.** Take `git log` from the rule commit to
  `95a0b816a9c8030873c6a71e3ac320a7c90d6f8c`, limited to two sets of paths: `src/Broiler.VM.Profile.JavaScript*/`
  outside `src/Broiler.VM.Profile.JavaScript/docs/`, and `src/compositions/Broiler.VM.Composition.JavaScript.Conformance/`.
  It names no commit.
  - With `--full-history` it names `48461e0`, SJL-12.3's merge of the rule commit into its own branch. That merge
    differs from the rule commit at none of those paths.
  - The condition therefore holds if SJL-12.6's changes, and every other change to those paths, merge after the
    corpus commit. It is read again when the corpus commit is named, and at the after commit.
- **The copy.** `src/tests/corpus/parse-1/` holds `corpus.manifest` and `inputs/` from Broiler.JS's
  `conformance/parse/`, byte for byte, at Broiler.JS's corpus commit. The manifest's SHA-256 is
  `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc`. The copy's README names that commit and says
  how the copy is checked.

### Part B: the records other than accepted and refused

The rule's configuration says that the mode writes "the outcome (accepted or refused)" for each input,
sub-population, variant and form. The mode writes two more records, `not-compiled` and `crashed`. Each is read as
follows.
- **`not-compiled`, note `fixture`, in B2.** B2 is "each test262 input as the conformance root composes it". The
  conformance root never composes a `_FIXTURE` file alone: `Test262Run` skips it as loaded by another test and never
  run on its own. So the row names no compilation of B2, and it is outside the population B2 compares: it has no
  outcome, no diagnostic list and no artifact hash.
  - The fixture is compiled in B1, and in B2 inside the module graph of each test that imports it.
  - The row is reported, not left out, so that completeness ("every manifest input reported in every
    sub-population, variant and form") is read over every row.
  - It must read `not-compiled`, note `fixture`, at every commit part B runs at. Any other answer at any of them is
    "an outcome that moves outside T", which is a regression.
- **`not-compiled`, note `no-json-goal`, in B4.** B4 is "each input compiled by the slice front end under its
  goal", one compilation per input, keyed by its goal. The slice front end has two goals, script and module, and
  none under which to compile a `json` input. So the row is outside the population B4 compares, on the same terms.
  - It must read the same at every commit. B4 admits no class, so any other answer at any of them is a regression.
  - A `json` input is compiled in B1, as a JSON module.
- **Any other `not-compiled` row, and every `crashed` row.** A `crashed` row says an exception left the compiler.
  Neither record is an outcome the rule names. So the input is not reported in that sub-population, variant and
  form, and the run that holds the row is not complete.
  - Part B is MET only when all three runs are complete, so such a row leaves part B NOT MET, whatever the
    comparison shows. The same holds for a run that lost its process.
  - This holds at the corpus commit, the ordering commit and the after commit alike.
  - No class of part B could admit such a row in any case. O, S, T and P each name an input accepted or refused at
    both commits, or accepted at the base and refused after.
  - `eng/compare-parse-corpus.py identity` lists each such row as `incomplete` and exits 2.
- **The diagnostics distribution.** A `not-compiled` or `crashed` row is not a refused input. It is in none of the
  refused-input lists or per-code tallies.

### Part C

- **Part C's population is what part B's after run accepted** in B1 and B3. A row of that run that is refused,
  `not-compiled` or `crashed` is outside it. The readings above admit no `not-compiled` row in B1 or B3.
- **Every compilation in the population must be accepted in all four compilations.** If one is refused,
  `not-compiled` or `crashed` in run 1, run 2 or either compilation of run 3, it gives no hash there. Its input then
  does not give "one hash in all four compilations", and part C is NOT MET.
  - A run that loses its process gives no hash for the compilations it did not finish, and is read the same way.
  - `eng/compare-parse-corpus.py determinism` counts each such compilation as one whose four results are not one
    accepted hash.

## 9. Exclusions

Section 3 names readings before any run. Sections 4 to 8 are added as the bundle's runs are taken. The exclusions
keep this section's number, where rule H2 reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-8-001`
and `sjl-10-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-12.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
