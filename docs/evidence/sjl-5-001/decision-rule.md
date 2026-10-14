<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-5-001's decision rule: the text area's test262 parity, its fuel-parity twins and probes, and its JSON modules' artifacts

**Written:** 2026-10-14, at milestone SJL-5 (work package SJL-5.1), with the area verdict
[JSD-0070](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md),
before any shared member, adapter or cut-over of the area exists, and with part B's probes
(`src/tests/forms/text`), part C's documents (`src/tests/forms/text/json-modules`) and part C's harness
([`json-modules.cs`](json-modules.cs)) committed in the same change. **Milestone judged:** SJL-5 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-5-text-algorithms.md),
its Broiler.VM clauses 7 and 9. **Owner:** the JavaScript profile owner, with the core architecture
owner. **Reviewer:** none. Every role this rule names is held by one person (EX-30), and this rule
does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class, in the
templates T1, T7 and T6 of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules).
**Each part decides alone**, and no part's runs are read by another. **There is no measurement part**:
SJL-5's exit gate has no measurement clause. The Broiler.JS repository commits its own rule for its own
engine, and the library-level reference-arm rule lives in the home; neither is read here. **It holds no
figure.** Where a count or an allowance matters, it names the file that fixes it, or states it as a
parameter of the rule.

**What SJL-4 taught this rule, before any run of SJL-5.** Bundle `sjl-4-001`'s part A met two native
rows whose re-runs gave a different exhausted dimension at each commit, and its revised rule of
2026-10-14 judged such a race by how often each commit reached each allowance, which turned on the
machine's speed. One of those rows was `decodeURI`'s, which this area moves. Fuel is deterministic and
the wall clock is not, so this rule judges a row whose re-runs differ **by running it once more with the
wall clock out of the way**, and asks whether the two commits then agree. It is written before any run
it judges, so its decision is not weaker evidence.

**The base.** SJL-5's base is this family after SJL-4, **the commit that carries this rule**, whose
product sources are the default branch's after SJL-4's merge (`1f253ff`). Its runs are taken by this
bundle's collector **before any code of SJL-5 lands**, from a clean checkout of that commit on an
otherwise idle machine, and retained in `sjl-5-001/base/`. A part whose base runs are not retained there,
or were taken after any code of SJL-5 landed, is void and is not judged.

## Part A - test262 parity, in both forms (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant, under the wide manifest, once in the bytecode form and once in the native form
configuration: eng/run-test262.py at the pin, through the conformance composition root built in Release, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs; the base runs are taken at the base commit as this file's header says and retained in sjl-5-001/base/; the after runs are taken at the commit that lands SJL-5's Broiler.VM cut-over, named in bundle sjl-5-001; both on the same RID, each on an otherwise idle machine
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair
admitted difference classes:
  C1 correction - a variant failing at the base passes after, because the shared implementation follows the specification where Broiler.VM did not; each one listed with its path, its variant and the vector, the specification clause or the row of JSD-0070's correspondence (P1, P2, S1) that names the correction
  C3 implementation-defined - the verdict changes on a surface the specification leaves to the implementation, and the area's vectors classify it; each one listed with its vector
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's, on an otherwise idle machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: JSD-0070's decision 3 keeps this family's charging units, so an exhaustion verdict that moves on fuel is a regression, not a class
every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs were retainable as whole runs (the runner's own retention conditions hold, and its exit code is neither 2 nor 3), every changed verdict of the bytecode pair is in C1, C3 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** The bundle retains each comparison's output, which lists every differing row with
its class or as unclassified, and a table naming each C1 and C3 row with its reason, and each (c') and
(e) row with its re-runs. The floor verdicts the runner prints are retained and do not decide part A: a
floor is a ratchet over totals, and this part judges variants. A row the comparison cannot classify is a
regression even when the totals improve. **The exhaustion verdicts** clause 7 requires are this part's
exhausted dimensions: a row exhausted on fuel at the base must be exhausted on fuel after, and a row that
completes at the base must complete after, or be a listed C1. Under (e), a row exhausted on fuel at both
commits once the wall clock is out of the way agrees, which is what this family's charging units promise.

## Part B - the fuel-parity twins and probes (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, and every *.small.js probe in src/tests/forms/text, as committed with this rule, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release; at the base commit, each twin's and probe's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-5-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin and probe, its base figure and one less
comparison: per twin or probe and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; JSD-0070's decision 3 keeps this family's charging units, so a moved figure is a defect of a shared member or of an adapter, not a class
MET if and only if every twin and probe, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-4-001`'s part B: one verdict at every ceiling tried, at the base and
after, in both forms, and nothing admits a charge that moved. The probes' answers are chosen so that no
correction of SJL-5 changes them (their README says how); an answer that changes is read as a moved
verdict. The wide twins show that the cut-over moved no charge outside the area.

## Part C - the JSON modules' artifacts (template T6)

```rule
evidence class: artifact identity (not a measurement)
population: every document in src/tests/forms/text/json-modules, as committed with this rule, and every file whose name ends in .json under test/ of the pinned test262 suite, read from the archive src/tests/conformance/pins/test262.pin names
configuration: docs/evidence/sjl-5-001/json-modules.cs as committed with this rule, run with dotnet run against the Broiler.VM.Profile.JavaScript.Compiler project of the commit under test, built in Release; it compiles each document alone, as a module whose type is JsFormat.JsonModuleType under SliceParseOptions.Module, through JsCompiler.Compile, and prints per document its outcome (compiled or refused), the SHA-256 of the artifact's bytes and the first diagnostic's code; it is run twice at the base commit and twice at the commit that lands SJL-5's Broiler.VM cut-over, and all four outputs are retained
comparison: per document, the outcome, the artifact's hash and the diagnostic's code at the base against the same after; and each commit's two runs against each other
admitted difference classes:
  C1 correction - a document is admitted or refused, or a value is written, differently after because the shared validator follows the JSON grammar (25.5.1) where JsJsonModule's reader did not; each one listed with its document and the clause
every other difference is a regression, including a moved refusal code and a document whose artifact changes when it is admitted at both commits; a commit whose two runs disagree on any document is a defect of determinism, and the part is NOT MET
MET if and only if all four runs covered the whole population, each commit's two runs agree on every document, and every document gives the same outcome, artifact hash and diagnostic code at the base and after, or is a listed C1.
NOT MET otherwise.
```

**How it is read.** Clause 9 asks for byte-identical artifacts, and the hash of the whole artifact is
that comparison. The translation's writer stays in this family (JSD-0070, "What is shared and what is
not"), so after the cut-over only its reader changes, and an unchanged writer over an unchanged reading
writes the same bytes. Correspondence row J8 is why most of the documents nest shallowly: past about
sixty levels this family refuses a JSON module at both commits, and the corpus holds documents on both
sides of that edge so that the refusal's code is compared too.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
this one whole and saying it was written afterwards; its decision is then weaker evidence. **Before
SJL-5.3's code lands, a later bundle shows this file unchanged**, as SJL-3.2 and SJL-4 did for theirs.

## What this file does not decide

- **The library-level reference-arm comparison.** Its rule, `reference-arms.md`, lives in the home,
  in bundle `sjl-5-001` there.
- **Broiler.JS's test262 parity.** Broiler.JS's own rule judges it.
- **The vectors, the closures, the packages and the `I` rules.** Each is a clause of SJL-5 judged by
  its own mechanism (clauses 6 and 8, and the home clauses), not by a comparison this file states.
- **A speed claim of any kind.** SJL-5 measures nothing, and part A's (e) raises the wall clock so that
  speed decides nothing.
