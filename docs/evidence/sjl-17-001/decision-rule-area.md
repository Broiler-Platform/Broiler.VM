<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-17-001's area rule: collation, judged by test262 parity in both forms on hosted runners, by the orderings dataset and the collation checks, and by the fuel-parity twins

**Written:** 2026-11-01, at milestone SJL-17 (work package SJL-17.1), with the area verdict [JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md),
before any shared collation member or cut-over exists. **Milestone judged:** SJL-17 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-17-collation.md): its
Broiler.VM clause 7 ("Whole-suite test262 in both forms, and the `orderings` reference dataset, meet the area rule.").
**Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule
names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class.
- **The templates.** Part A in template T1 of
  [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
  part B a vector agreement in T3's sense over the programs and files this family retains, part C in T7.
- **Each part decides alone.**
- **There is no measurement part** (JSD-0084's decision E10).
- **Other rules.** Broiler.JS's rule is its own, and is not read here.
- **It holds no figure.**

**What earlier bundles taught this rule, before any run of SJL-17.**
- **Part A follows bundle `sjl-9-001`'s part A**, on the same local machine for re-runs.
- **This family's tables do not move.** Rule N28 holds the collation tables byte for byte: the root, root `search`, and
  German `phonebook` and `search`.
- **The pins do not move.** CLDR 48.2.0, its UCA, and test262 `ccaac100`.
- **The canonical-tz proposal** stays admitted as SJL-9.4 left it.
- **This family has no correction in this area** that JSD-0084 names. A C1 row is recorded as a finding against its
  correspondence.

**The commits, named before they exist.**
- **The rule commit** carries this file.
- **The after commit** is the default-branch merge that lands SJL-17.4's cut-over, one merge, named in bundle
  `sjl-17-001`.
- **The base commit** is its first parent. It must meet three conditions:
  - it descends from the rule commit;
  - it descends from the after commit bundle `sjl-9-001` names in this repository;
  - the after commit is its only first-parent successor that changes a path under `src/Broiler.VM.Profile.JavaScript*/`,
    `src/compositions/`, `eng/packages/` or `Directory.Packages.props`.

A part whose base was taken at any other commit, or whose base commit fails those conditions, is void.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant the wide manifest scores at the base commit, once in the bytecode form and once in the native form; test/intl402/Collator and test/built-ins/String/prototype/localeCompare are in it, and a regression on any other path fails this part too
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs at the base commit and the after runs at the after commit; each dispatch's artifact test262-evidence imported unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared collator follows the pinned text where this family did not; JSD-0084 names none, so each is listed with its path, its variant and its clause, and recorded as a finding that JSD-0084's correspondence missed
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine bundle sjl-9-001's rule names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and one whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's and no other build, test or run on the machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: the collator charges where and as this family's did (JSD-0084 decision E7), so an exhaustion verdict that moves on fuel is a regression
no C3 and no C5: the tables are N28's, byte for byte, and no choice the specification leaves open moves
every other difference is a regression, including a variant passing at the base that fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

## Part B - the orderings dataset, the collation checks, and every other Intl check unchanged

```rule
evidence class: conformance parity (not a measurement)
population: src/tests/cldr/orderings/ with its retained answer, the archived CollationTest files under src/tests/cldr/pins/, and, as guards, every other program under src/tests/cldr/ and src/tests/temporal/, as committed at the base commit
configuration: the slice compiler's --checks, built in Release from a clean checkout, at the base commit and at the after commit, on the local machine; the area's checks are intl/i1/german-phonebook-and-search-order, intl/i1/german-and-english-orderings-match-icu and every intl/i1/the-root-collation-orders-collationtest-* check; the guards are every other check whose name begins intl/, tzdb/ or temporal/ at the base commit; each verdict and output retained
comparison: per check, its verdict at the base against after; and git diff between the two commits over src/tests/cldr/ and src/tests/temporal/
admitted difference classes: none; the shared collator must order every line as this family did, over the same tables
MET if and only if every check named passes at both commits, and no file under src/tests/cldr/ or src/tests/temporal/ differs between them.
NOT MET otherwise.
```

## Part C - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, as committed at the base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine; at the base commit, each twin's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-17-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin, its base figure and one less
comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none
MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
and says it was written afterwards, and its decision is then weaker evidence. Before SJL-17.3's code lands, a later
bundle shows this file unchanged.

## What this file does not decide

- **The home clauses**: the collator under the conventions, its vectors, the UCA test files and the reference dataset at
  library level, the SL rules and the probe. They are judged in Broiler.JS.
- **Broiler.JS's clauses.**
- **This family's other clauses** (6, 8 and 9), each by its own mechanism: rules I1 to I7 and I5's rows, three-mode
  publication, optionality under I3, and N28's budget.
- **A speed claim of any kind.**
