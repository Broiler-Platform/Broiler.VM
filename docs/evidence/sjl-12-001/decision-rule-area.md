<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-12-001's rule: the wide path's in-place preparation, judged by test262 in both forms, by artifact identity over the parse corpus, by determinism, by the front end's rules and by the diagnostic mapping

**Written:** 2026-10-19, at milestone SJL-12 (work package SJL-12.1), with the contract record
[JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md),
before any change it judges exists: no binder pass, no ordering change, no structured template, and no parse corpus.
**Milestone judged:** SJL-12 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-12-frontend-contract.md):
its Broiler.VM clauses 4 ("The binder pass is separate from emission. The wide path's early errors run in one stage.
Ordering is deterministic.") and 5 ("test262 in both forms meets the predeclared rule. The parse corpus's artifacts
are byte-identical, or every difference is classified. Rules N5 to N9, N12 and N19 hold."), and this repository's
half of shared clauses 7 and 8. **Owner:** the JavaScript profile owner. **Reviewer:** none. Every role this rule
names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Five predeclared decisions, one per part.
- **Not an area verdict.** SJL-12 applies no extraction gate and files no X1 to X7 verdict; JSD-0080 records why, and
  the frontend area's verdict is SJL-13.1's. The file keeps the name bundles `sjl-6-001` to `sjl-10-001` gave their
  rules.
- **The templates.** Part A is in template T1 of
  [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
  and part B in T6. Part C is layer L13's determinism, part D the rule runs, and part E validation section 7's third
  control for SJL-12, "a diagnostic code mapped to two registry meanings". None of the three has a template letter.
- **Each part decides alone.** Part C's population is the inputs part B's after run accepted, and no part reads
  another's verdict.
- **There is no measurement part.** SJL-12's row of the evidence matrix marks L5 and L13 only.
- **Other rules.** The Broiler.JS repository commits its own rule for its own engine. It is not read here.
- **It holds no figure.** Where a count matters, it names the file that fixes it.

**What earlier bundles taught this rule, before any run of SJL-12.**
- **Part A follows bundle `sjl-10-001`'s part A.** The local machine for its re-runs is the one bundles `sjl-5-001` to
  `sjl-10-001` were taken on, named in the bundle by its processor count, its SDK and its operating system, with no
  other build, test or run on it while a run of this bundle is taken.
- **This family's pins do not move.** test262 `ccaac100` (`src/tests/conformance/pins/test262.pin`) and the language
  edition ES2026 at `tc39/ecma262@0248456c` (rule N14).
- **Part B is new.** No earlier bundle compared artifacts over a corpus, so it states its tool, population, controls
  and classes here, before the corpus exists.

**The commits, named before they exist.**
- **The rule commit** is the commit that carries this file. Part A's base runs are taken there.
- **The corpus commit** is the default-branch commit that adds SJL-12.4's parse corpus under
  `src/tests/corpus/parse-1/` and the slice compiler's `--parse-corpus` mode. Parts B and C's base runs are taken
  there. It must meet three conditions:
  - it descends from the rule commit;
  - `git diff --name-only` between the two names only paths under `src/tests/corpus/parse-1/`,
    `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/`, `eng/compare-parse-corpus.py`,
    `eng/check-frontend-mapping.py`, `src/Broiler.VM.Profile.JavaScript/docs/` and `docs/`;
  - it is an ancestor of every commit after the rule commit that changes a path under
    `src/Broiler.VM.Profile.JavaScript*/`, other than under `src/Broiler.VM.Profile.JavaScript/docs/`, or under
    `src/compositions/Broiler.VM.Composition.JavaScript.Conformance/`.
- **The ordering commit** is the default-branch merge that lands SJL-12.6 item 3, deterministic ordering, alone and
  before items 1, 2 and 4 (JSD-0080 decision D14). Part B also runs there.
- **The after commit** is the default-branch merge that lands the last of SJL-12.6's four items. It is named in bundle
  `sjl-12-001`.

A part whose base was taken at any other commit, or whose corpus commit fails those conditions, is void and is not
judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form; the negative syntax tests are in it, and a regression on any path fails this part
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to this file's commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the rule commit and the after runs at the after commit; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction - a variant that does not pass at the base and passes after, of one of two kinds JSD-0080 names before any run, each listed with its path, its variant, the first diagnostic at the base and after, and its kind: (V1) a template correction of SJL-12.6 item 4 - a negative variant the base accepted that is refused after, because yield or await inside a template substitution in formal parameters, or template nesting past the parser's nesting bound, is now refused (JSD-0080 decision D15); (V2) a stage move - a negative variant whose base refusal's first diagnostic was 2104 and whose after refusal's first diagnostic is a language early error the binder pass reports before the lowering runs (JSD-0080 decisions D10 to D12); a C1 row of neither kind is recorded in the bundle as a finding against JSD-0080
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's and no other build, test or run on the machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: SJL-12 changes no charging unit, so an exhaustion verdict that moves on fuel is a regression, not a class
no C3 and no C5: no specification revision, test262 pin, data or policy moves at SJL-12, so a verdict that moves because one did is a regression
every other difference is a regression, including a positive variant refused after, a variant passing at the base that fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-10-001`'s part A. The bundle retains:
- both dispatches' artifacts;
- each comparison's output, which lists every differing row with its class or as unclassified;
- a table naming each C1 row with its kind and both first diagnostics;
- each (c') and (e) row with its re-runs.

The floor verdicts the harness prints are retained and do not decide part A. Each native run exits 4, which
completeness admits. A row the comparison cannot classify is a regression, even when the totals improve.

## Part B - artifact identity over the parse corpus (template T6)

```rule
evidence class: artifact identity (not a measurement)
population: every input of src/tests/corpus/parse-1/corpus.manifest as the corpus commit holds it, the test262 inputs read from src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz and each checked against the manifest's SHA-256, the others from src/tests/corpus/parse-1/inputs/, in four sub-populations: B1 each input alone, under its goal and each of its variants (script sloppy; script strict, compiled as the conformance root compiles a strict variant under the wide manifest - the unit's ForceStrict set and its text unchanged; module; raw; JSON), under the wide manifest, in the bytecode form and in the native form under the backend x86-64-sysv on linux-x64; B2 each test262 input as the conformance root composes it - the harness files and the test as one compilation, in the order src/compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Run.cs builds its units at the corpus commit - under each of its variants, bytecode form; B3 each script-goal input whose text contains one of eval, arguments, super, new.target or #, which the manifest marks eval-family, compiled as eval code under each of eight request-flag sets, E0 None, E1 Strict, E2 InFunction, E3 InFunction and Strict, E4 InFunction and InMethod, E5 InFunction, InMethod and InDerivedConstructor, E6 InFunction, InMethod and InClassFieldInitializer, E7 InFunction, InMethod and InClassBody, bytecode form; B4 each input compiled by the slice front end under its goal; every source name and module key fixed by the manifest's header (the virtual root /test262/), and every compile request the conformance root's own for the wide manifest, source text kept as it keeps it
configuration: the slice compiler's --parse-corpus mode, built in Release from a clean checkout on the local machine this file's header names, its source file unchanged between the corpus commit and the after commit; for each input, sub-population, variant and form it writes the outcome (accepted or refused), the ordered diagnostics (code, line, column) and, for an accepted input, the artifact's SHA-256 and length; run at the corpus commit (the base), at the ordering commit and at the after commit; and the slice compiler's --census mode, over src/tests/corpus/parse-1/inputs/ and over the test/ directory of the suite extracted from that archive, at the corpus commit and at the after commit; for every input whose hash differs between two of these runs, the artifact bytes at each are written and retained
completeness: every manifest input reported in every sub-population, variant and form at each of the three commits, the manifest's digest at each run equal to the corpus commit's, and no run lost its process
controls: a copy of the base report with one artifact hash changed and one outcome flipped, which the comparison must report as two unclassified differences; retained with its output
comparison: eng/compare-parse-corpus.py, as the corpus commit holds it, joined on input, sub-population, variant and form: the outcome; for an input accepted at both commits, the artifact hash; for an input refused at both, the ordered diagnostic list; and the census output at both commits, byte for byte
admitted difference classes:
  O ordering fix by decision (JSD-0080 decision D14, the repair stage JSB-1 names) - an accepted input whose bytes differ, the difference already present at the ordering commit and unchanged after it, and located in the order of the global lexical declarations or of the lexical names of the eval declaration row; each listed with both byte files
  S a refusal that moved stage with the same codes (JSD-0080 decisions D10 to D12) - an input refused at both commits whose after diagnostic list differs from the base's only by: (S1) order - the same codes, in source order after; (S2) collapse - a diagnostic the base repeated at one position, because a finally block or another construct was lowered more than once, appears once after; (S3) suppressed - a 2104, 2301, 2302 or 2303 diagnostic of the base is absent after, because the binder pass refused first and the lowering did not run; (S4) unmasked - an early error after that the base did not report, because the lowering left a construct after its first refusal in it, with a code the binder pass reports; each listed with both lists and its kinds
  T template correction (JSD-0080 decision D15) - an input accepted at the base and refused after, with 2101 for yield or await inside a substitution in formal parameters, or with 2103 for template nesting past the nesting bound; each listed
  P position - an accepted input whose bytes differ only in the source-position section, at a node inside a template substitution that the base placed at an offset other than its token's start in the source text and that after places at it; each listed with the two positions, checked against the source text
  sub-population B4 admits no class: the slice front end's outcome, diagnostics and bytes, and the census, are the same at both commits for every input
no corrected-literal class: SJL-12 corrects no literal value
every other difference is a regression: an outcome that moves outside T, a byte difference outside O and P, a diagnostic difference outside S and T, and any difference in B4 or the census
MET if and only if all three runs are complete, the control is reported as stated, every difference is in O, S, T or P and is listed, and B4 and the census show none.
NOT MET otherwise.
```

**How it is read.**
- **Identity is by hash.** It is equality of SHA-256 over the artifact bytes. The bytes themselves are retained only
  where the hashes differ, because the accepted inputs' artifacts are too large to keep, and any commit regenerates
  them.
- **Class O is expected to be empty.** Both dictionaries are filled in source order and never lose an entry. Its
  bytes, if any, must already show at the ordering commit, so that a later item cannot hide behind it.
- **The refused-input lists and per-code tallies are this bundle's diagnostics distribution** (roadmap section 9).
  They are retained at each commit and read through classes S and T.

## Part C - determinism over two runs and two threads (layer L13)

```rule
evidence class: determinism (not a measurement)
population: every input, variant and form part B's after run accepted in sub-populations B1 and B3
configuration: the slice compiler's --parse-corpus mode at the after commit, built in Release from a clean checkout on the local machine this file's header names, in three processes - run 1 with one worker, run 2 with one worker, and run 3 with two workers compiling every input twice concurrently, once on each; and the slice compiler's --checks rows "the wide lowering is deterministic" and "the wide lowering declares lexical names in source order", which SJL-12.6 item 3 adds beside "the lowering is deterministic", at the after commit
controls: two patches, each to a scratch copy of the after commit, never merged, retained with their output: K1 reverses the global lexical declarations - the source-order row must fail, and part B's comparison over the patched build must report at least one byte difference; K2 shuffles the global lexical declarations per compilation from an unseeded source - run 1 against run 2, or the two compilations of run 3, must differ, and the determinism row must fail; each patch is reverted and the revert checked against the after commit
comparison: per input, variant and form, the four hashes - run 1, run 2 and the two of run 3
admitted difference classes: none
MET if and only if every accepted input gives one hash in all four compilations, both checks rows pass at the after commit, and both controls fail as stated and pass after their revert.
NOT MET otherwise.
```

**How it is read.** The same three runs are also taken at the corpus commit. They are retained and read, not judged.
The two checks rows are stage JSB-1's standing check: the profile's ledger judges JSB-1 on its own legend, and this
part does not complete it.

## Part D - the front end's rules (rule runs)

```rule
evidence class: rules (not a comparison)
population: rules N5, N6, N7, N8, N9, N12 and N19 of src/tests/Broiler.VM.Architecture.Tests/rules.register.json, their tests in DiagnosticRegistryRuleTests.cs and CompilationStackRuleTests.cs, and the witnesses their register rows name
configuration: dotnet test of Broiler.VM.Architecture.Tests in Release from a clean checkout of the after commit, with BROILER_RULE_MESSAGES set to a directory of the bundle, the test log and the rule-message files retained; the same at the rule commit, retained and read, not judged; and the slice compiler's --checks rows SJL-12.6 adds for N19's property - a source at the tree-depth bound compiled through the binder pass, and a source nesting templates past the nesting bound refused with 2103 - at the after commit
comparison: none between commits
MET if and only if every test of the seven rules passes at the after commit, every witness they name is refused, N12's test counts the compiler assembly's files at the after commit with a comment naming each file SJL-12.6 added, and both checks rows pass.
NOT MET otherwise.
```

## Part E - the diagnostic mapping (validation section 7's control)

```rule
evidence class: record consistency (not a comparison)
population: src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv at the after commit; the early-error catalogue it names by Broiler.JS commit and SHA-256 (Broiler.ECMAScript/docs/frontend/early-errors.tsv); and src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt at the after commit
configuration: eng/check-frontend-mapping.py, as the corpus commit holds it, run at the after commit
checks: the catalogue's digest equals the one the mapping names; every catalogue row with a shared code has exactly one mapping row; every shared code maps to exactly one registry code the registry holds; a row marked within-meaning quotes the registry row's published meaning, which covers the rule; a row marked split names the registry row it leaves and says the new number is minted at SJL-13 under JSD-0009; no registry code is reached under two meanings unless every meaning but its published one is marked split; every registry code from 2001 to 2210 and from 2401 to 2406, except 2104, is reached by a row or names the catalogue's engine-only row that explains it
controls: two copies of the mapping - one giving a shared code a second registry code, one merging a rule onto a registry row whose meaning does not cover it without marking it split; the checker must refuse each; retained with its output
MET if and only if the checker passes on the mapping and refuses both controls.
NOT MET otherwise.
```

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
and says it was written afterwards, and its decision is then weaker evidence. **Before SJL-12.6's code lands, a later
bundle shows this file unchanged.**

## What this file does not decide

- **The contract's adequacy.** JSD-0080 records it, and SJL-13 tests it.
- **The catalogue's completeness.** SJL-12.3's done-when, "every early error has a row", is held by the catalogue's own
  checks in both directions, committed with it.
- **Broiler.JS's clauses.** Broiler.JS's rule judges them.
- **This family's standing obligations:**
  - rules J1 to J12 and the assurance artefacts;
  - rule N10's baseline, which SJL-12.6 must leave unchanged;
  - three-mode publication of any root whose closure moves.

  They are read from the tree and the CI, not from a comparison this file states.
- **JSB-1's state.** The profile's ledger judges it.
- **A speed claim of any kind.** SJL-12 measures nothing.
