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

*(Added 2026-10-20.)* The bundle now holds part A's base dispatch, and parts B and C's base runs at the corpus
commit (section 4). "This bundle holds no run yet" was true when it was written. Nothing here is a verdict still.
No collector took the bases: part A's was imported by hand, as
[`base/hosted/imported.txt`](base/hosted/imported.txt) records, and parts B and C's runs were taken by the commands
[`base/commands.txt`](base/commands.txt) lists.

*(Added 2026-10-20.)* The bundle now also holds part B's run at the ordering commit, in `after/` (section 5).
Nothing here is a verdict still: part B is decided when its run at the after commit has been taken.

*(Added 2026-10-25.)* SJL-12.6's four items have landed, and the after commit exists (section 1). Section 3 now
also names, before any after run, how the rule's classes read the edges SJL-12.6 item 4's review found, under
"Readings named before the after runs". No after run had been taken when it was written. Nothing here is a verdict
still.

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
  *(Added 2026-10-20, when parts B and C's base runs were committed, section 4.)* The corpus commit is
  `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`, the default-branch merge of Broiler.VM#159. The ordering commit and
  the after commit do not exist yet.
  *(Added 2026-10-20, when part B's run at the ordering commit was committed, section 5.)* "The ordering commit and
  the after commit do not exist yet" was true when it was written. The ordering commit is
  `066abefea4351fb49bd8bf8e823fbdc7aac560de`, the default-branch merge of Broiler.VM#163, which lands SJL-12.6 item
  3, deterministic ordering, alone. The after commit does not exist yet.
  *(Added 2026-10-25, when the readings named before the after runs were committed, section 3.)* "The after
  commit does not exist yet" was true when it was written. The after commit is
  `1de3af6278fda8ef0cca9d1009c3803ed6fc8c2f`, the default-branch merge of Broiler.VM#167, which lands SJL-12.6
  item 4, the last of its four items. Items 1 and 2 landed between the two, in `3fa4b39` (Broiler.VM#165) and
  `daa92eb` (Broiler.VM#166). No run has been taken at the after commit.

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

*(Added 2026-10-20, when the bases were committed, section 4.)* Part A's base dispatch is in `base/hosted/`, as
bundle `sjl-10-001` placed its own: the artifact holds a `binaries.sha256` of the conformance root's assemblies, and
`base/` holds the slice compiler's under the same name. `base/logs/` holds the local base runs' logs. Part B's
control and its output are in `controls/`, beside part E's. `corpus/corpus-commit.txt` records the corpus commit as
the `corpus/` bullet above says. *(Added 2026-10-20, after the bases were first committed.)* `corpus-commit.txt`
holds the path list as counts per listed path, every path outside `src/tests/corpus/parse-1/`, and the check that
the paths under it are that folder's every file; `corpus/corpus-commit-paths.txt` holds the list whole, from git
alone. Each of `base/`, `base/hosted/`, `controls/` and `corpus/` has a `hashes.txt`, the SHA-256 of every other
file of the folder (for `base/`, with `base/logs/` and without `base/hosted/`).

*(Added 2026-10-20, when part B's run at the ordering commit was committed, section 5.)* That run's report is
[`after/parse-corpus-ordering.tsv.gz`](after/parse-corpus-ordering.tsv.gz), and its comparison with the base's run
1 is [`after/identity-base-ordering.tsv`](after/identity-base-ordering.tsv): the paths the comparison's output names.
The run's own records are in [`after/ordering/`](after/ordering/): `machine.txt`, `commands.txt`, the slice
compiler's `binaries.sha256`, and `logs/`, which holds the checkout's `git rev-parse HEAD` and
`git status --porcelain` before and after its build, as section 4 asks. [`after/hashes.txt`](after/hashes.txt) is
the SHA-256 of every other file of `after/`, with `after/ordering/`. *(Added 2026-10-20, after that run was first
committed.)* [`after/ordering/ordering-commit.txt`](after/ordering/ordering-commit.txt) records the ordering commit
from git, as `corpus/corpus-commit.txt` records the corpus commit: its identity, and the paths and blobs section 5
reads of it.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 3. Readings named before the base runs (SJL-12.4, 2026-10-20)

**Written 2026-10-20, at SJL-12.4**, with the parse corpus's copy and the slice compiler's `--parse-corpus` mode.
Nothing here revises the rule. Each reading says how the rule's own words are read where the mode had to choose,
or how a record the mode writes is read.

*(Added 2026-10-25.)* The last subsection, "Readings named before the after runs", was added at SJL-12.6's
records, before any after run. It is in this section because this is where the bundle names its readings before
the runs they read.

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

*(Added 2026-10-20, section 4.)* It is `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`, the default-branch merge of
Broiler.VM#159. The three conditions were read again at it, from git alone, in
[`corpus/corpus-commit.txt`](corpus/corpus-commit.txt), which section 4 reads.
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

### Readings named before the after runs (SJL-12.6, 2026-10-25)

**Written 2026-10-25, at SJL-12.6's records**, after its four items had landed and before any run at the after
commit. Nothing here revises the rule: [`decision-rule-area.md`](decision-rule-area.md) is the rule commit's,
unchanged. Each reading says how the rule's own words read an edge SJL-12.6 item 4's review found, or says that
they cannot and names the edge a finding against JSD-0080. Either way a row of it in the after runs is reported,
and no reading chosen after a run admits it.

**What had been run when this section was written.**
- No run of any part had been taken at the after commit. Part B's run at the ordering commit (section 5) is the
  bundle's only run after its bases.
- Item 4's local pre-checks were taken at its change's head, `3902f6f`, whose tree is the after commit's. They
  compiled the corpus in part B's shape, and the pinned suite's directories of parse- and resolution-phase negative
  tests in part A's bytecode form. They are not this bundle's runs, and it does not retain them. Each population
  below is what those pre-checks found; the after runs decide.

**How part A reads a changed verdict of neither kind.** Part A's C1 is "a variant that does not pass at the base and
passes after, of one of two kinds JSD-0080 names before any run", V1 and V2, and its last clause says that "a C1
row of neither kind is recorded in the bundle as a finding against JSD-0080". This bundle reads that clause as
saying what the bundle records of such a row, and not as a third kind the class admits:
- the class is defined by its two kinds;
- JSD-0080's decision D39 gives as its reason that "the only verdicts it may improve are the ones this record
  names";
- and the rule's own account of how part A is read says "A row the comparison cannot classify is a regression,
  even when the totals improve".

So a changed verdict of neither kind is not in C1 for part A's MET clause. It is listed unclassified and is a
regression, and the bundle also records it as a finding against JSD-0080. Edges 1 and 2 (ii) below are read so in
part A, and no other reading is taken after a run.

**Edge 1: an input the base already refused, whose diagnostic list decision D15 changes.**
- **What it is.** Item 4's parser reads a template's substitutions in place, so the nesting depth, the tree depth
  and the parameter context carry into them (decision D15). An input refused at both commits that holds such a
  substitution can then be refused by the parser inside it, which changes its diagnostic list, in two shapes.
  - **A 2101 for a `yield` or an `await` in a substitution in formal parameters.** The checks rows of
    `BinderChecks` hold such sources since `964b460`, each with its list before item 4 and after. In each, the
    `await`'s 2104 becomes the parser's 2101 at the same place. Where the binder pass reported another diagnostic
    besides the 2104, that diagnostic is absent as well, because the parser refused and the binder did not run:
    - with the `await` alone, the plainest, ``async function g() { (a = `${await 1}`) => a; }``, went from
      `[2104@1:30]` to `[2101@1:30]`, and its module form from `[2104@1:9]` to `[2101@1:9]`;
    - with a `break` the binder refuses, `[2207@1:1 2104@1:37]` became `[2101@1:37]`, and
      `[2104@1:30 2207@1:62]` became `[2101@1:30 2102@1:47]`: the binder's 2207 is absent, and in the second the
      parser's recovery from the body it stops reading adds a 2102 the base did not report;
    - with a regular expression whose flags the binder refuses, `[2104@1:30 2101@1:36]` became `[2101@1:30]`, and
      `[2104@1:31 2101@1:44]` became `[2101@1:31]`: the binder's 2101 for the flags is absent.
  - **A 2103 for nesting or tree depth carried through a substitution**, under either bound edge 2 names, in an
    input the base refused for another reason. The parser refuses at the place inside the substitution where the
    depth passes the bound, and, as in the 2101 shape, nothing after the parse runs. So a base list the lowering
    gave is absent after, and the parser's list stands in its place. A base list the parser gave keeps only the
    diagnostics the parse still reaches, in the order it reports them, with the 2103 among them. A 2102 of the
    parser's recovery from the 2103 follows it, as edge 2 says of the nesting bound's. No checks row holds this
    shape. It follows from the same carry as edge 2.
- **Neither S nor T.** T names an input accepted at the base, and this one is refused at both commits.
  - The new 2101 or 2103 is the parser's, so it is not S4's early error "the base did not report, because the
    lowering left a construct after its first refusal in it", and neither is a 2102 of the parser's recovery.
  - The base's 2104 is absent after because the parser refused, not because "the binder pass refused first", and
    it is not a diagnostic decision D12 keeps in the lowering, so S3 does not reach it. The 2207 and the
    regular-expression 2101 that are absent after are not codes S3 lists either.
  - S1 and S2 keep the codes.

  So such a row is "a diagnostic difference outside S and T", which the rule makes a regression.
- **A consequence of decision D15 that the rule does not list.** D15 lists acceptance changes. It does not say that
  an input already refused changes its list. A row of this edge in the after run is reported unclassified, and the
  bundle records it as a finding against JSD-0080 as well. Neither the reading nor the finding admits it.
- **Part A.** A negative variant of this edge whose verdict changes, refused at the base with a first 2104 and
  after with the parser's 2101 or 2103 first, is neither V1 (the base did not accept it) nor V2 (its first
  diagnostic after is the parser's, not one the binder pass reports). It is a changed verdict of neither kind,
  read as stated above: not admitted, and a finding against JSD-0080. A variant refused at both commits with any
  other first diagnostic keeps its verdict, and part A compares verdicts.
- **How it is told.** From the source and the message, at the line and column of the after list's new diagnostic:
  - a 2101 at a `yield` or an `await` inside a template substitution that lies in a formal parameter list;
  - a 2103 whose place lies inside a template substitution, as edge 2 tests it, with its bound read from its
    message as edge 2 says.

  The after comparison's classifier tests that, leaves such a row unclassified and attributes it to this edge. A
  classifier that applied S's tests by code and position alone would put the plainest rows in S3 and S4, which
  this reading refuses.
- **Population: empty.** At item 4's head, the corpus's diagnostic differences from the base were item 2's, row
  for row, so no input of the corpus moved under item 4. Item 4's part A pre-check, against a run at `f6facc2`,
  before item 2, found no negative variant whose verdict or first diagnostic changed. Item 4's scan of the pinned
  suite's `test/` for a substitution that begins with `yield` or `await` found such a substitution only in
  `language/expressions/yield/rhs-template-middle.js`, in a generator's body, not in parameters. The commit that
  shows the edge is `964b460`.

**Edge 2: T's "2103 for template nesting past the nesting bound", and V1's "template nesting past the parser's
nesting bound".**
- **(i) Nesting counted through a substitution is read as T, and as V1.** The base's sub-parse began each
  substitution at depth zero. Item 4 carries the depth into it, so the nesting around a template and the nesting
  inside its substitution add up toward the nesting bound, whether the nesting inside is another template or
  parentheses. The 2103 is the nesting bound's, for nesting that passes through a template, which is the
  correction decision D15 lists: "template nesting counts toward the nesting bound, and past it is refused with
  2103". So an input accepted at the base and refused after with that 2103 is in T, and a negative variant so
  refused is V1. The commits that show it are `964b460` (`TemplateChecks`' row for templates nested in templates)
  and `3902f6f` (its row for parentheses around a template and inside its substitution adding up). This is also
  the edge the review named "nesting adding through a substitution".
- **(ii) The tree-depth bound's 2103 is not read as T or V1, because the rule's words cannot bear it.** Item 4 also
  carries the tree depth into a substitution. So a chain inside a substitution is charged from the depth the chain
  around the template has reached, as a parenthesised operand's is (JSD-0080 decision D17's reason). An input
  accepted at the base can then be refused after, with the 2103 of the tree-depth bound.
  - T names "the nesting bound" and V1 "the parser's nesting bound". This rule names the two bounds apart, in part
    D's "a source at the tree-depth bound" beside "a source nesting templates past the nesting bound", and so do
    JSD-0080's limits. Decision D15 lists no tree-depth correction, though its account of the parser says that the
    tree depth carries.
  - So such a row is outside T, "an outcome that moves outside T", which the rule makes a regression in part B. In
    part A, a negative variant so refused is a changed verdict of neither kind, read as stated above, and is not
    admitted. In both, the bundle records it as a finding against JSD-0080, whose
    listed corrections leave it out, and whose status says that no other change either engine makes at SJL-12
    "moves an input between accepted and refused by the whole compile".
  - The commit that shows it is `964b460`, whose `TemplateChecks` row was renamed and narrowed in `3902f6f`: "a
    substitution is charged toward the tree-depth bound as a parenthesised operand is".
- **How the two bounds are told apart: by the source.** The report holds codes and positions, and both bounds
  report 2103. For each row refused after with a 2103 the base did not report, whether the base accepted the input
  or refused it for another reason (edge 1), the after comparison compiles the row's input again at the after
  commit, as the mode compiled it (its text, goal, variant and request), with a program of its own, because part B
  holds the mode's source file unchanged. It reads the message the wide parser gives with that 2103:
  - the nesting bound's says the source nests deeper than the levels "these parse options allow", from `JsParser`'s
    `Enter` and `EnterNesting`, and from `ParseTemplate` for a template past the bound;
  - the tree-depth bound's says the source builds a tree deeper than the levels "this compiler walks", from
    `Deepen`.

  Only a nesting bound's 2103 is read under (i), and only where the nesting at that place passes through a template
  substitution. WP7 tests that from the source text: the 2103's line and column lie inside a template substitution,
  after its `${` and before the `}` that closes it. A template past the bound is refused at its backtick, which lies
  inside the substitution of the template around it. Outside every substitution, item 4 changes no depth, so an
  input the base accepted gains a nesting-bound 2103 only inside one. The lowering's own 2103s, for evaluated source
  past the scope-depth ceiling and for a JSON module too deep to read, are at line 0, column 0, and neither is a
  template correction.
- **T and V1 are read on the after list's first diagnostic.** The parser's recovery from a nesting-bound 2103 adds
  a 2102 for each parenthesis open inside the substitution, as it does for parentheses past the bound outside a
  template (`TemplateChecks`' remarks since `3902f6f`). Those 2102s follow the 2103 in the after list. They are
  part of the one refusal and are not read as differences of their own. So an input is in T when its after list's
  first diagnostic is the 2101 or the nesting bound's 2103 that T names, and a negative variant is V1 when its
  first diagnostic after is. An input whose first diagnostic after is neither is not in T.
- **Population: empty, for (i) and (ii) alike.** At item 4's head, every input of the corpus had the outcome it had
  at the base, in B1 to B3, and the negative directories of the pinned suite changed no verdict.

**Records, for after the runs.** These are not readings. They change no class, and none is judged here.
- **The mode's stale remark.** `ParseCorpus.cs`'s remark on `WorkerStackBytes` says that the module walk asks
  `JsCompiler.Requests` on the caller's stack, "the one place a compilation of this run does not get the compiler's
  own declared stack". Since `2c590e4`, `Requests` parses inside `CompilationStack.Run`, so the remark is stale. It
  is kept because part B's configuration holds the mode's source file unchanged between the corpus commit and the
  after commit. A change after this bundle's runs corrects it.
- **The structured mode's memory.** Within the nesting bound, the runs of item 4's structured mode hold up to the
  nesting bound times the template's length, and are made while tokenizing, before the parse. So a source that the
  parse would refuse before a large, deeply nested template now pays for the runs first. The base's sub-parses also
  copied the text below each level, lazily and with no bound. No part of this rule reads memory, and SJL-12
  measures nothing (`964b460`, and `SliceTokenizer`'s remarks since `fb6226e`).
- **The parser's left-operand gap.** The parser does not charge a left operand's own spine to the chain it begins.
  A chain whose first operand is a parenthesised chain, or a template whose substitution holds one, compiles at the
  base and after with a tree deeper than the tree-depth bound (`TemplateChecks`' remarks since `3902f6f`). The gap
  predates SJL-12 and lies outside SJL-12.6 item 4. It is recorded so that a reader of part D's rows knows what
  they do not hold.

**Two notes from SJL-12.6 item 3**, which Broiler.VM#163 left for these records.
- **Class O's words are narrower than what the change could move.** O names bytes "located in the order of the
  global lexical declarations or of the lexical names of the eval declaration row". The two dictionaries item 3
  replaced also decided the interning of those names, the lexical names of the script-declarations row and an
  evaluated program's lexical slots. It is moot: at the ordering commit no byte differs from the base (section 5),
  so class O is empty whatever the after run shows.
- **What part C's checks row compiles.** Part C names the row "the wide lowering is deterministic". JSD-0080's
  decision D14 says the row compiles "the retained wide and module corpora", and JSB-1's gate says "the wide and
  module corpora". How the row reads them is recorded in
  [JSC-319](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-319): the retained corpus's
  wide and module rows rebuilt by their producer, with the parse corpus's stored inputs and the row's own probes.

## 4. The bases (2026-10-20)

**Written 2026-10-20.** This section reads the base runs this bundle retains. It judges nothing: the rule compares
each base with a run at the ordering commit or the after commit, and neither commit exists yet.

*(Added 2026-10-20.)* The words "neither commit exists yet" were true when this section was written. The ordering
commit now exists, and section 5 reads part B's run there. The after commit does not exist yet.

| Part | Commit | Taken | Retained in |
|---|---|---|---|
| A | The rule commit, `889ea1568f59bf37aa21c0035b423d0d030eb87f` | 2026-10-19, 22:43Z to 22:50Z, by the hosted workflow | [`base/hosted/`](base/hosted/) |
| B, with its census, and C, read and not judged | The corpus commit, `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4` | 2026-10-20, 12:41Z to 13:40Z, on the programme's local machine | [`base/`](base/) |
| B's control | Made from part B's base report alone | 2026-10-20 | [`controls/`](controls/) |

Part D's rule run at the rule commit, which section 2 also places in `base/`, is not in this bundle yet.

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

### Parts B and C's bases, at the corpus commit

| | |
|---|---|
| Commit | The corpus commit, `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`, the default-branch merge of Broiler.VM#159 ([`corpus/corpus-commit.txt`](corpus/corpus-commit.txt)) |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), unchanged since the rule commit: it is not among the paths `git diff --name-only` names between the two commits |
| Machine | The programme's local machine: four processors, .NET SDK 10.0.401, Ubuntu 24.04.5 LTS, `linux-x64` ([`base/machine.txt`](base/machine.txt)) |
| Taken | 2026-10-20: the build from 12:41Z, the census from 12:57Z, and runs 1 to 3 from 13:02Z to 13:40Z ([`base/logs/runs.tsv`](base/logs/runs.tsv)). Broiler.JS's build and runs for its own bundle were taken on the same machine from 12:38Z to 12:57Z, none of them while one of these ran |
| Tool | The slice compiler's `--parse-corpus` and `--census` modes, built in Release from a clean detached checkout of the corpus commit, with no warning ([`base/logs/build.log`](base/logs/build.log)); the SHA-256 of the assemblies that build wrote is in [`base/binaries.sha256`](base/binaries.sha256). Every command, in order, is in [`base/commands.txt`](base/commands.txt) |

**The idle machine** ([`base/machine.txt`](base/machine.txt), [`base/logs/gate.log`](base/logs/gate.log)).
- Before the build and before each run, a gate required a 1-minute load average of at most 1.0 and no `dotnet` or
  `python` process, and recorded the load, the processor count, the operating system and the SDK
  ([`base/logs/pre-run.txt`](base/logs/pre-run.txt)). Every gate of these runs went ahead at its first look. No
  other build, test or run was taken on the machine while they were taken.
- The gate read the 1-minute figure only. `machine.txt` says what the 5- and 15-minute figures carry.
- At 12:43:13Z another session fetched and fast-forwarded the main checkout `/home/user/Broiler.VM`, after the
  build had ended and before the next gate, so during no run. It was a git operation, not a build, test or run.
- Both points were corrected in `machine.txt` before it was committed, and its dated note quotes the words it
  replaced.

**How the base is bound to the corpus commit.**
- **The reports name no commit.** Each report's header names the manifest's SHA-256 and the pinned archive's, which
  bind it to the corpus and to the suite, not to the compiler's code.
- **Three records, written when the runs were taken, bind it.**
  - [`commands.txt`](base/commands.txt) makes the checkout with `git worktree add --detach` at the corpus commit,
    named by all forty digits, and builds and runs from it.
  - [`machine.txt`](base/machine.txt) records that the checkout's `git status` was empty before and after the
    build.
  - [`binaries.sha256`](base/binaries.sha256) holds the SHA-256 of the twelve assemblies that build wrote.
- **No machine-written record of the checkout's `git rev-parse HEAD` or `git status --porcelain` was retained**, and
  the checkout was removed after the runs, so none can be made now. The first two records are accounts written at
  the time. `binaries.sha256` is the one a reader can check: it is what a Release build of the corpus commit, at the
  checkout path `commands.txt` records and with the SDK `machine.txt` names, is compared with. No such build has
  been made again.
- **The corpus commit itself** is recorded from git in [`corpus/corpus-commit.txt`](corpus/corpus-commit.txt), as
  section 2 says `corpus/` holds it: its identity, its manifest's SHA-256, which is the one each report's header
  names, and the three conditions below.
- **The ordering and after runs are to retain**, in their own `logs/`, `git rev-parse HEAD` and
  `git status --porcelain` of their checkout before and after its build.

**The corpus commit's three conditions, read again when it is named**, as section 3 says they are, from git alone
([`corpus/corpus-commit.txt`](corpus/corpus-commit.txt)).
- **It descends from the rule commit.** `git merge-base --is-ancestor` exits 0.
- **It names only the listed paths.** `git diff --name-only` between the two names 2,282 paths
  ([`corpus/corpus-commit-paths.txt`](corpus/corpus-commit-paths.txt) holds every one), and each is under a path the
  condition lists:
  - 2,272 under `src/tests/corpus/parse-1/`, which are every file of that folder at the corpus commit, none of which
    existed at the rule commit;
  - `ParseCorpus.cs` and `Program.cs` of the slice compiler;
  - `eng/compare-parse-corpus.py` and `eng/check-frontend-mapping.py`;
  - the mapping, under `src/Broiler.VM.Profile.JavaScript/docs/`;
  - five files of this bundle, under `docs/`.
- **It comes before every product change.** Read at the default branch at
  `ebf93cd3e8fba11002f3da563b13b32ca54afe32`, `git log` from the rule commit over the paths the condition lists names
  no commit. With `--full-history` it names `48461e0` alone, which section 3 reads, and which is itself an ancestor of
  the corpus commit. Since the corpus commit, the default branch has changed files under `docs/evidence/sjl-3-001/`
  alone. The condition is read again at the after commit.

**The runs** ([`base/logs/runs.tsv`](base/logs/runs.tsv), each run's standard output and error in
[`base/logs/`](base/logs/)).

| Run | What it ran | Exit | Retained |
|---|---|---|---|
| Census 1 | `--census` over `src/tests/corpus/parse-1/inputs/`, and over the `test/` directory of the suite extracted from the pinned archive | 0, 0 | [`census-inputs-1.txt.gz`](base/census-inputs-1.txt.gz), [`census-test-1.txt.gz`](base/census-test-1.txt.gz) |
| Census 2 | The same over two other locations of the same files: Broiler.JS's `conformance/parse/inputs/` at its corpus commit, which the copy is byte for byte, and a second extraction of the archive at a deeper path | 0, 0 | [`census-inputs-2.txt.gz`](base/census-inputs-2.txt.gz), [`census-test-2.txt.gz`](base/census-test-2.txt.gz) |
| Run 1 | `--parse-corpus` over the whole manifest, one worker | 0 | [`parse-corpus-run1.tsv.gz`](base/parse-corpus-run1.tsv.gz) |
| Run 2 | The same | 0 | [`parse-corpus-run2.tsv.gz`](base/parse-corpus-run2.tsv.gz) |
| Run 3 | `--parse-corpus --twice-concurrent`: two workers, each compilation twice, concurrently | 0 | [`parse-corpus-run3.tsv.gz`](base/parse-corpus-run3.tsv.gz) |

- Run 1 is part B's base report, and part C's run 1. Runs 2 and 3 are part C's runs 2 and 3.
- **No run lost its process.** Each run kept a journal of every compilation it began and ended. In each of runs 1
  and 2, 560,963 compilations began and ended, and in run 3 1,121,926 did, none begun and not ended
  ([`base/logs/journal-check.txt`](base/logs/journal-check.txt)). The journals were then deleted.
- **No artifact bytes were written.** The rule retains the bytes of an input whose hash differs between two runs, and
  no hash differs between these.
- Wall-clock and peak memory are in `runs.tsv` as records. SJL-12 measures nothing.

**Part B's base.**
- **Complete.** The identity comparison holds each report to the manifest: its SHA-256, the corpus commit's, and
  every compilation the four sub-populations owe it, no more and no fewer. Each of runs 1 and 2 holds 560,963
  compilations of the manifest's 55,751 inputs (each report's header, `manifest-rows`, and each run's standard
  output, [run 1](base/logs/parse-corpus-run1.stdout.txt) and [run 2](base/logs/parse-corpus-run2.stdout.txt)), and
  none is incomplete ([`base/identity-run1-run2.tsv`](base/identity-run1-run2.tsv), which also gives the
  compilations). Nothing crashed in any run (each run's standard output). Every `not-compiled` row is in one of
  the two cases reading 2 names: 287 in B2, note `fixture`, and 13 in B4, note `no-json-goal`. The
  per-sub-population outcomes are in the comparison's header.
- **Run 1 against run 2.** `eng/compare-parse-corpus.py identity`, with both census pairs, finds 0 differences, in
  every sub-population and of every kind (outcome, artifact, diagnostics, note), and both census pairs identical
  byte for byte. It exits 0. The two reports are one file byte for byte, `05bc9801...`
  ([`base/hashes.txt`](base/hashes.txt)).
- **The census.** `census-inputs-1.txt.gz` and `census-test-1.txt.gz` are part B's base census, over the two places
  the rule names. Each second census, over another location of the same files, is byte for byte the first. This
  section quotes no census figure: the census over `test/` counts the pinned suite's files.
- **The diagnostics distribution.** Run 1 holds every refused compilation's ordered diagnostics, which are the
  base's refused-input lists that classes S and T are read through. This section draws no tally from them.

**Part C's base, read and not judged.** The rule takes part C's three runs at the corpus commit too, and reads them
without judging them (part C, "How it is read").
- `eng/compare-parse-corpus.py determinism` over runs 1, 2 and 3, with run 1 as the population report, puts
  363,555 compilations in the population: the B1 and B3 compilations run 1 accepted. It leaves 39,203 B1 and B3
  rows outside, refused in run 1.
- It finds 0 compilations whose four results are not one accepted hash, and exits 0
  ([`base/determinism-runs-1-2-3.tsv`](base/determinism-runs-1-2-3.tsv)).
- Run 3's own summary reports 0 compilations where its two concurrent compilations differ
  ([`base/logs/parse-corpus-run3.stdout.txt`](base/logs/parse-corpus-run3.stdout.txt)).
- At the after commit, part C's population is part B's after run's, not run 1's.

**Part B's control** ([`controls/`](controls/)). The rule's control is "a copy of the base report with one artifact
hash changed and one outcome flipped, which the comparison must report as two unclassified differences".
- `eng/compare-parse-corpus.py control` copied run 1, changed the last digit of the hash of `T000001 B1 strict
  bytecode`, and flipped `T000001 B1 sloppy bytecode` from accepted to refused
  ([`controls/part-b-control.out`](controls/part-b-control.out)).
- `identity` of run 1 against the copy reports 2 differences, both in B1, one artifact and one outcome, both
  unclassified, with none incomplete, and exits 1
  ([`controls/part-b-control-identity.tsv`](controls/part-b-control-identity.tsv)). That is the report the rule
  states.
- **Made twice.** The control was first made into the staged `base/` when the runs were taken. It was made again
  into `controls/` on 2026-10-20, from the retained run 1 alone, so that its output names the paths the bundle
  commits ([`base/commands.txt`](base/commands.txt)). The copy is byte for byte the first one, `c1c2807a...`, and
  the comparison's output differs from the first only in the path it names for the copy.

**What this section does not read.** SJL-12.4's integration runs, which section 3 names, are not this bundle's runs,
and nothing here is compared with them.

**Retained and checked.** In each of `base/`, `base/hosted/`, `controls/` and `corpus/`, `sha256sum -c hashes.txt`,
run in the folder, passes.

## 5. Part B at the ordering commit (2026-10-20)

**Written 2026-10-20.** This section reads part B's run at the ordering commit, which this bundle retains in
[`after/`](after/). It judges nothing. Part B is decided over its three runs, and the run at the after commit has
not been taken: part B is MET "if and only if all three runs are complete, the control is reported as stated, every
difference is in O, S, T or P and is listed, and B4 and the census show none".

| | |
|---|---|
| Commit | The ordering commit, `066abefea4351fb49bd8bf8e823fbdc7aac560de`, the default-branch merge of Broiler.VM#163. Its first parent is `01444a590147b4bcdc033ea82c60d477924c752d`, the default branch before it, and its second is `3ff6b35cea220228ea98877cf95c9b4bc49f439c`, the change's head ([`after/ordering/ordering-commit.txt`](after/ordering/ordering-commit.txt)) |
| What it lands | SJL-12.6 item 3, deterministic ordering, alone and before items 1, 2 and 4, as the rule names the ordering commit (JSD-0080 decision D14): `SourceOrderedLexicals` in the compiler, and the slice compiler's two `--checks` rows that part C names, "the wide lowering is deterministic" and "the wide lowering declares lexical names in source order". From the corpus commit to the first parent, `git diff --name-only` names paths under `docs/` alone. Against its first parent the merge changes the compiler's `JsCompiler.cs`, `JsCompiler.Eval.cs` and `JsCompiler.Hoisting.cs`, the slice compiler's `SourceFrontEndChecks.cs` and `WideLoweringChecks.cs`, and the three generated assurance records, `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and `assurance.manifest.json` ([`ordering-commit.txt`](after/ordering/ordering-commit.txt)) |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), unchanged since the rule commit: `git rev-parse` gives the same blob at the rule commit and the ordering commit ([`ordering-commit.txt`](after/ordering/ordering-commit.txt)) |
| Machine | The programme's local machine, as for the bases: four processors, .NET SDK 10.0.401, Ubuntu 24.04.5 LTS. The run recorded the processor count; the SDK and the operating system were read on the machine after the run ([`after/ordering/machine.txt`](after/ordering/machine.txt)) |
| Taken | 2026-10-20: the build from 18:23:40Z, the run from 18:24:36Z to 18:34:10Z, and the comparison with the base to 18:34:17Z ([`after/ordering/logs/driver.log`](after/ordering/logs/driver.log)) |
| Tool | The slice compiler's `--parse-corpus` mode, built in Release from a clean detached checkout of the ordering commit, with no warning ([`build.log`](after/ordering/logs/build.log)). The mode's source file, `ParseCorpus.cs`, and `eng/compare-parse-corpus.py` are the corpus commit's: `git rev-parse` gives the same blob for each at both commits, as the rule requires ([`ordering-commit.txt`](after/ordering/ordering-commit.txt)). Every command, in order, is in [`after/ordering/commands.txt`](after/ordering/commands.txt) |

**The idle machine** ([`machine.txt`](after/ordering/machine.txt), [`driver.log`](after/ordering/logs/driver.log)).
- The script's gate listed no `dotnet` or `python` process before the build. The script recorded the load average
  at the gate, before the build, before and after the run, and at its end. Unlike the bases' gate, it applied no
  load threshold and did not wait: it recorded.
- No other build, test or run was taken on the machine while the run was taken.
- During the run the orchestrating session acted in other checkouts, never in this run's. From 18:32:44Z to
  18:32:47Z it fetched, fast-forwarded and pushed the Broiler.JS checkout `/home/user/broiler.js` (its reflogs). At
  about 18:32Z to 18:33Z it removed a merged Broiler.JS worktree. And it merged Broiler.JS#1064 on GitHub at
  18:32:37Z, which ran there, not on this machine; the fetch at 18:32:44Z brought that merge. No other pull request
  of Broiler.VM, Broiler.JS, Broiler.Unicode or Broiler.Regex was merged during the run, and Broiler.VM#163, which
  made the ordering commit, was merged at 18:22:58Z, before the gate (their GitHub records). These were git and
  GitHub operations, not builds, tests or runs. `machine.txt` names each.

**How the run is bound to the ordering commit.** Section 4 asks the ordering and after runs to retain, in their own
`logs/`, `git rev-parse HEAD` and `git status --porcelain` of their checkout before and after its build. This run's
are in [`after/ordering/logs/`](after/ordering/logs/).
- [`checkout-before-build.txt`](after/ordering/logs/checkout-before-build.txt) reads the ordering commit and an
  empty `git status --porcelain --ignored`, so nothing ignored, no `bin/` or `obj/`, was in the checkout.
- [`checkout-after-build.txt`](after/ordering/logs/checkout-after-build.txt) and
  [`checkout-after-runs.txt`](after/ordering/logs/checkout-after-runs.txt) read the ordering commit and an empty
  `git status --porcelain`.
- The checkout was made with `git worktree add --detach` at the ordering commit, named by all forty digits, after
  `git merge-base --is-ancestor` had found it on the default branch ([`commands.txt`](after/ordering/commands.txt)).
- [`binaries.sha256`](after/ordering/binaries.sha256) holds the SHA-256 of the twelve assemblies the build wrote.
  Against [`base/binaries.sha256`](base/binaries.sha256), the nine Broiler.VM assemblies differ, among them the
  compiler's, `Broiler.VM.Profile.JavaScript.Compiler.dll`, which holds the change. The three of Broiler.ECMAScript
  and Broiler.Unicode are the same: they come from packages, which the build copies and does not compile.
  *(Added 2026-10-20, after this section was first committed.)* The difference does not show the change by itself.
  Seven of the nine Broiler.VM assemblies are compiled from sources that are the same at the corpus commit and the
  ordering commit, which differ outside `docs/` only in the compiler's and the slice compiler's sources and the
  three assurance records ([`ordering-commit.txt`](after/ordering/ordering-commit.txt)). So the two builds also
  wrote something else into what they compiled, likely the checkout's path or the commit, and the run is bound to
  the ordering commit by the checkout records above, not by these hashes (`machine.txt`, its dated note).

**Complete**, as part B's completeness states it for each of its three commits: "every manifest input reported in
every sub-population, variant and form at each of the three commits, the manifest's digest at each run equal to the
corpus commit's, and no run lost its process".
- The run exited 0, and its standard output reports nothing crashed
  ([exit](after/ordering/logs/parse-corpus-ordering.exit.txt),
  [standard output](after/ordering/logs/parse-corpus-ordering.stdout.txt)).
- **It did not lose its process.** Its journal records 560,963 compilations begun and 560,963 ended, none begun and
  not ended ([`journal-check.txt`](after/ordering/logs/journal-check.txt)). The journal was then deleted.
- The identity comparison holds the report to the manifest: its SHA-256, the corpus commit's, and every compilation
  the four sub-populations owe it, no more and no fewer. It finds none incomplete, so every `not-compiled` row is in
  one of the two cases reading 2 names ([`after/identity-base-ordering.tsv`](after/identity-base-ordering.tsv)).

**Against the base.** `eng/compare-parse-corpus.py identity`, base run 1 against the ordering run's report, finds 0
differences, in every sub-population, B1 to B4, and of every kind (outcome, artifact, diagnostics, note). It exits 0
([`identity.exit.txt`](after/ordering/logs/identity.exit.txt),
[`after/identity-base-ordering.tsv`](after/identity-base-ordering.tsv)).
- **The report is byte for byte the base's run 1.** Both files' SHA-256 is `05bc9801...655f`
  ([`after/hashes.txt`](after/hashes.txt), [`base/hashes.txt`](base/hashes.txt)).
- **The census is not compared.** The rule takes it at the corpus commit and the after commit only. No census was
  taken at the ordering commit, and the comparison was run with `--no-census`.
- **No artifact bytes were written.** The rule retains the bytes of an input whose hash differs between two runs, and
  no hash differs between these.

**What it means for part B's classes**, read and not judged.
- **Class O is empty.** O admits "an accepted input whose bytes differ, the difference already present at the
  ordering commit and unchanged after it". At the ordering commit no input's bytes differ from the base's, so no
  input can be in O, whatever the after run shows. The rule expected it: "Class O is expected to be empty."
- So a byte difference the after run shows is in class P, or it is a regression.
- B4, which admits no class, shows no difference at the ordering commit either.

**What this section does not read.** The two `--checks` rows the ordering commit adds are part C's, and part C reads
them at the after commit. No other part runs at the ordering commit.

**Retained and checked.** [`after/hashes.txt`](after/hashes.txt) holds the SHA-256 of every other file of `after/`,
with `after/ordering/`, and `sha256sum -c hashes.txt`, run in the folder, passes. Of the logs, `driver.log` alone was
trimmed, of the progress segments `git worktree add` wrote, as [`commands.txt`](after/ordering/commands.txt) records
with the untrimmed file's SHA-256.

## 9. Exclusions

Section 3 names readings before parts B and C's base runs. Sections 4 to 8 are added as the bundle's runs are
taken. The exclusions keep this section's number, where rule H2 reads an exclusion's definition, as in bundles
`sjl-3-001` to `sjl-8-001` and `sjl-10-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-12.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
