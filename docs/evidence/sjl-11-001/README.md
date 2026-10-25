<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-11-001 - regular expressions: Broiler.VM's rules, correspondence and evidence

**Milestone:** SJL-11 of the shared JavaScript libraries programme,
[`sjl-11-regexp.md`](../../javascript-shared-libraries/milestones/sjl-11-regexp.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-11.1 before any change it judges existed.

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

**State:** rules and correspondence only, 2026-10-25, at SJL-11.1. **Nothing here is a verdict, and nothing has been
taken.**
- Every base run is taken at the admission commit, the first commit of the branch that lands SJL-11.4, which admits
  `regexp-duplicate-named-groups` and changes nothing else outside `docs/` (JSD-0081 decisions D15 and D16).
- The after runs are taken at the default-branch merge that lands SJL-11.4, whose first parent must be the admission
  commit's parent.
- A collector joins this bundle when there is a run to take.

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
- **A stop at one of this family's matcher ceilings names no dimension.** The conformance root reports it as a
  failure whose detail is `ProfileFault/AllowanceExhausted` with no payload, and the command-line root exits with its
  "host defect" code (rows B1 and B6). The rule reads that report, and only it, as a ceiling stop.

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
  - A moved exhaustion verdict, a ceiling stop among them, is admitted under C2 only on a variant that reaches a
    regular expression, and only if the variant passes after, at ten times the allowance where it does not pass at
    the part's.
  - Class C3 holds row M4's empty match alone, by decision D41.
  - A wall-clock race is admitted under C4's (c') and (e). There is no class C5.
- **Part B** holds the compiler's literal refusals over bundle `sjl-12-001`'s parse corpus, admitting only row S1's
  duplicate names and rows S5's and S10's literals (template T6's reading).
- **Part C** holds the wide twins to their base figures, with no class: no twin reaches a regular expression
  (template T7).
- **Part D** runs the adversarial programs of [`inputs/adversarial/`](inputs/adversarial/) in both forms through the
  command-line root, each against its declared outcome and against the base. It reads each process from its exit code
  and its line, the ceiling stop among them, and admits only the duplicate-name cases and a move between fuel and a
  ceiling. Its control patches the adapter's mapping of `LimitExceeded` and runs over the M cases, the only cases that
  mapping reaches; a copy of the report with the other classes' cases changed shows the comparison reporting them.
  This is validation section 7's control for SJL-11.
- **Part E** is this family's measurement over the workloads of [`inputs/workloads/`](inputs/workloads/README.md)
  (template T5), retained in its own sub-bundle. It is not a gate clause (decision D19).

**The pins do not move**: test262 `ccaac100`, the language edition ES2026 at `tc39/ecma262@0248456c`, this family's
nesting limit 512 (decision D7), and its frame and trail ceilings `1 << 20` and `1 << 21` (decision D10).

## 2. What this bundle will hold

- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's base report and its
  refused-input lists; part C's `fuel-parity.log`; part D's base runs in both forms; all at the admission commit.
- **`after/`**: the same at the after commit, with part A's comparisons, its table of C1, C2 and C3 rows, its C2
  re-runs and its (c') and (e) re-runs, part B's comparison, and part D's comparison script and output.
- **`controls/`**: part B's control report, part D's patch K1 with its output and its revert, and its report copy K2
  with the comparison's output.
- **`measurement/`**: part E's sub-bundle, with its own README and manifest.
- **`vectors/`**: this family's runs of the regular-expression vectors SJL-11.3 adds.

The differential cross-check's report over `src/tests/differential/the-regexp-*.js` is retained beside the parts
without a rule (JSD-0081 decision D38).

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-11.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
