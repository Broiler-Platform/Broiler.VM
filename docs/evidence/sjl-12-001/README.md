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
2026-10-20 (section 3). **Nothing here is a verdict, and this bundle holds no run yet.**
- Part A's base runs are taken by the hosted workflow at the rule commit, before any product change of SJL-12.6
  lands.
- Parts B and C's base runs are taken at the corpus commit, which adds SJL-12.4's parse corpus and the slice
  compiler's `--parse-corpus` mode.
- The after runs follow the last of SJL-12.6's four items.
- A collector joins this bundle when there is a run to take.

*(Added 2026-10-20.)* The bundle now holds part A's base dispatch (section 4). "This bundle holds no run yet" was
true when it was written. Nothing here is a verdict still. No collector took the base: it was imported by hand, as
[`base/hosted/imported.txt`](base/hosted/imported.txt) records.

**SJL-12 is not an area verdict.** It applies no extraction gate and files no X1 to X7 verdict. The frontend area's
verdict is SJL-13.1's (JSD-0080 decision D1).

Broiler.JS's clauses are judged in Broiler.JS's bundle of the same name. This bundle reads none of its runs.

## 1. SJL-12's rule (SJL-12.1)

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
  *(Added 2026-10-20, when part A's base was imported, section 4.)* It is
  `889ea1568f59bf37aa21c0035b423d0d030eb87f`, the default-branch merge of Broiler.VM#157.
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
Nothing here revises the rule. Each reading says how the rule's own words are read where the mode had to choose,
or how a record the mode writes is read.

**What had been run when this section was written.**
- No base run of part B or part C had been taken for this bundle. Those are taken at the corpus commit.
- Part A's base dispatch had been taken at the rule commit on 2026-10-19.
- SJL-12.4's integration check had run the mode over the whole corpus on 2026-10-20, in part B's shape and in part
  C's three. The mode was built at this change's commit `b89885f`, with the corpus copied in. Those runs were read
  before this section was written. They are not this bundle's runs, and it does not retain them.
- The mode's readings 1 to 3 were in its code at `b89885f`, before those runs.

**The mode's readings and this section's.** The mode
(`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/ParseCorpus.cs`) writes three readings in every
report's header. This section names all three:
- reading 1 is under "Part B: what B1 compiles";
- readings 2 and 3 are under "Part B: the records other than accepted and refused".

The reading of B2's composition, and those for the diagnostics distribution and for part C, are this section's
own. `eng/compare-parse-corpus.py` applies the readings for part B's records and for part C, as the bullets below
say.

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
  - With `--full-history` it names `48461e0`, SJL-12.3's merge of the rule commit into its own branch. Its first
    parent, `3fa24e3`, does not descend from the rule commit. Against that parent the merge changes two lock files,
    `src/Broiler.VM.Profile.JavaScript/packages.lock.json` and
    `src/Broiler.VM.Profile.JavaScript.Compiler/packages.lock.json`. It brings into SJL-12.3's branch the move to
    Broiler.ECMAScript 0.1.0-preview.7 that the rule commit already holds. At every listed path it equals its
    second parent, the rule commit.
  - **How "changes a path" is read.** The rule does not say what a merge changes. This section reads it as git
    does: a commit changes a path when it differs there from every one of its parents (git's TREESAME rule, which
    `git log -- <paths>` applies without `--full-history`). A merge that equals one of its parents at a path takes
    that parent's content there and changes nothing. Under this reading `48461e0` changes none of the listed
    paths. Read against the rule commit instead, it changes none of them either.
  - The condition therefore holds if SJL-12.6's changes, and every other change to those paths, merge after the
    corpus commit. It is read again when the corpus commit is named, and at the after commit.
- **The copy.** `src/tests/corpus/parse-1/` holds `corpus.manifest` and `inputs/` from Broiler.JS's
  `conformance/parse/`, byte for byte, at Broiler.JS's corpus commit,
  `1fe542ff459935586cdf495e4a0e3d798248948a`. The manifest's SHA-256 is
  `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc`. The copy's README names that commit and says
  how the copy is checked.

### Part B: what B1 compiles (the mode's reading 1)

B1 is "each input alone, under its goal and each of its variants". The rule sets "alone" against B2, "each test262
input as the conformance root composes it - the harness files and the test as one compilation". It does not say
whether a module's own imports are part of a module input alone. The mode reads "alone" as nothing composed around
the input.
- A script variant is one unit, the input's text, with no harness file.
- A JSON input is one JSON module.
- A module is compiled with the graph the conformance root's resolution reaches from it through its static
  `import` and `export ... from` declarations, as the root compiles a module test. The wide path compiles a module
  graph into one artifact whose entry is the root module, and every other module is reached through a request.
  - So a module's hash covers every module it reaches. A change in a reached module shows in each module that
    reaches it, as well as in the reached module's own B1 row.
  - The words also admit a second reading: the module compiled with its requests left unresolved. The mode does
    not take it, because the conformance root never compiles a module test that way.

This reading fixes every B1 module row's hash at the base. It is named here so that it is fixed before the base
runs, and it cannot change between the corpus commit and the after commit, because the rule holds the mode's source
file unchanged between them.

**The owner's decision of 2026-10-20.** Of the two readings the words admit, this one is taken, as recommended,
under the owner's direction of 2026-10-18: "Please do all recommended decisions and skip waiting for review." The
direction changes no rule of the programme, and this decision changes no word of the rule: it chooses between two
readings the rule's own words allow, before any base run of part B. It is taken because it is the only one under
which a B1 module row is what the conformance root itself compiles, so that B1 and B2 judge the same compilations
of a module test.

### Part B: B2's composition

B2 compiles each test262 input "as the conformance root composes it ... in the order
src/compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Run.cs builds its units at the corpus
commit". The composition is read as the units `Test262Run.RunVariant` builds: the harness files unless the variant
is raw, then the test, or the module graph it roots.
- **It is applied to every test262 input whose front matter the root's reader reads, except a `_FIXTURE` file.**
- **What decides whether the root runs a composed test is not composition, and is not applied.** The root skips a
  test that claims a proposed feature, and one tagged `host-gc-required`, before it builds its units. Each such
  test still has the composition `RunVariant` would build, and B2 compiles and compares it.
- **A `_FIXTURE` file is never composed.** test262's `INTERPRETING.md` says such files "MUST NOT be interpreted as
  standalone tests; they are intended to be referenced by test files". The root never builds units with one as the
  test. That is a property of the file, not a run policy like the skips above.
- **A test whose front matter the reader cannot read has no composition the root could build**, because the
  includes and flags come from the front matter. Unlike a fixture, it is a test. The mode reports it `not-compiled`,
  note `frontmatter`, which is not one of the two cases its reading 2 names. It is read under "any other
  `not-compiled` row" below.

### Part B: the records other than accepted and refused

The rule's configuration says that the mode writes "the outcome (accepted or refused)" for each input,
sub-population, variant and form. The mode writes two more records, `not-compiled` and `crashed`. Each is read as
follows.
- **`not-compiled`, note `fixture`, in B2.** A `_FIXTURE` file is never composed, as above. So its B2 row names no
  compilation, and it is outside the population B2 compares: it has no outcome, no diagnostic list and no artifact
  hash.
  - The fixture is compiled in B1 on its own row. Under reading 1 it is also compiled inside the B1 graph of each
    module input that imports it statically.
  - In B2 it is compiled inside the graph of each module test that imports it statically, through `import` or
    `export ... from`. A fixture reached only by `import()` or by `ShadowRealm.prototype.importValue` is in no B2
    compilation, and it is compiled in B1 alone.
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
  - The mode's other notes are `frontmatter` and `no-harness-file`, in B2, and `graph-too-large` and
    `graph-unreadable`, for a module graph in B1 or B2.
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
  - Whether part B's after run is complete is part B's question. The rule says "each part decides alone", so an
    incomplete after run leaves part B NOT MET and does not stop part C from being judged.
  - `eng/compare-parse-corpus.py determinism` builds the population from the accepted B1 and B3 rows of part B's
    after report, and nothing else. It holds that report to the manifest, as it does every report, but does not
    refuse it for a `crashed` row or a `not-compiled` row outside reading 2. Its header counts the B1 and B3 rows
    left outside the population, and the rows that leave part B's run incomplete.
- **Every compilation in the population must be accepted in all four compilations.** If one is refused,
  `not-compiled` or `crashed` in run 1, run 2 or either compilation of run 3, it gives no hash there. Its input then
  does not give "one hash in all four compilations", and part C is NOT MET.
  - A run that loses its process gives no hash for the compilations it did not finish, and is read the same way.
  - `eng/compare-parse-corpus.py determinism` counts each such compilation as one whose four results are not one
    accepted hash.

## 4. The bases (2026-10-20)

**Written 2026-10-20.** This section reads the base runs this bundle retains. It judges nothing: the rule compares
each base with a run at the ordering commit or the after commit, and neither commit exists yet.

| Part | Commit | Taken | Retained in |
|---|---|---|---|
| A | The rule commit, `889ea1568f59bf37aa21c0035b423d0d030eb87f` | 2026-10-19, 22:43Z to 22:50Z, by the hosted workflow | [`base/hosted/`](base/hosted/) |

### Part A's base, at the rule commit

**The dispatch.** [39336895555](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/39336895555), attempt 1,
was dispatched from `main` on 2026-10-19 at 22:43:09Z, with both forms and the rule commit named by all forty
digits. It completed at 22:50:42Z ([`base/hosted/run.txt`](base/hosted/run.txt),
[`base/hosted/imported.txt`](base/hosted/imported.txt)).
- **The dispatched ref is the rule commit itself.** `run.txt` reads `dispatched ref: refs/heads/main at
  889ea1568f59bf37aa21c0035b423d0d030eb87f`, the same tested commit, and `tested tree: clean`. So the workflow and
  its driver at the dispatched ref are the rule commit's, byte for byte, as part A's configuration requires, and no
  product change had merged when the base was taken.
- **The artifact** `test262-evidence` is retained unchanged in [`base/hosted/`](base/hosted/), its own folder, as
  bundle `sjl-10-001` placed its own. [`imported.txt`](base/hosted/imported.txt) records the run, the artifact's id
  and the digest of the zip as downloaded, `a843a089d009ecd20552c7cd8234c0e4bff3b30a8b60eef47d57bb3a1508aa10`, which
  is the digest GitHub records for the artifact. Each of its 54 files was compared with its entry in the zip, and
  none differs. [`hashes.txt`](base/hosted/hashes.txt) holds the SHA-256 of every other file of the folder, and
  `sha256sum -c hashes.txt`, run in the folder, passes.
- **The partition and the allowance** are the workflow's: four runners of four shards per form, fuel 100,000,000
  and a wall clock of 5,000 ms, at the suite pin `test262.pin` with `sha256=794eea96...` (`run.txt`).

**Complete and retainable in both forms**, as the rule's completeness states it: every shard of the partition
reported, the run log's exit code 0, 1 or 4, the merged report's retention object retainable, and one distinct
assembly list in the form's binaries log.

| Run | Exit code (`<run>.run.log`) | Coverage, retention and shard findings (`<run>.test262.json`) | Assembly lists (`<run>.binaries.log`) |
|---|---|---|---|
| `wide-bytecode` | 1, the floor holds ([run log](base/hosted/wide-bytecode.run.log)) | whole; retainable, no reason given; no finding ([report](base/hosted/wide-bytecode.test262.json)) | 4 runners and the merging job, 1 distinct ([log](base/hosted/wide-bytecode.binaries.log)) |
| `wide-native` | 4, a re-base: the floor was set in the `x86-64-win64` native form and this run is `x86-64-sysv` ([run log](base/hosted/wide-native.run.log)) | whole; retainable, no reason given; no finding ([report](base/hosted/wide-native.test262.json)) | 4 runners and the merging job, 1 distinct ([log](base/hosted/wide-native.binaries.log)) |

- The native run's exit 4 is the one the rule admits ("Each native run exits 4, which completeness admits").
- Both forms' runners and the merging job built one list, `77b67996...`, which is the SHA-256 of the artifact's
  [`binaries.sha256`](base/hosted/binaries.sha256), the conformance root's twelve assemblies.

**The runners' images, recorded and not judged.** The merging job ran image `ubuntu24` 20261018.339.1. Of the eight
runners, six ran 20261012.334.1, and two ran 20261018.339.1: `slice-bytecode-1` and `slice-native-3`
([`wide-bytecode.runners.txt`](base/hosted/wide-bytecode.runners.txt),
[`wide-native.runners.txt`](base/hosted/wide-native.runners.txt)). Every runner reports the SDK 10.0.402 and the
native backend `x86-64-sysv`. Part A's completeness reads the assembly lists, which agree; the rule names no image.

**No row is judged.** The rule compares this base with the after dispatch at the after commit, per variant, through
`eng/compare-test262-forms.py --same-form`. The floor verdicts each run log prints are retained and do not decide
part A, and this section quotes no total (section 2).

## 9. Exclusions

Section 3 names readings before parts B and C's base runs. Sections 4 to 8 are added as the bundle's runs are
taken. The exclusions keep this section's number, where rule H2 reads an exclusion's definition, as in bundles
`sjl-3-001` to `sjl-8-001` and `sjl-10-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-12.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
