<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-002 - the shared syntax library and the wide path, step 2: Broiler.VM's rule and evidence

**Milestone:** SJL-13 of the shared JavaScript libraries programme,
[`sjl-13-shared-syntax-and-vm.md`](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md), at
the second of SJL-13.3's two cut-over steps, in which the lowering, the numeric-manifest admission and the binder read
the shared tree directly and the adapter and `JsSyntax` go. Bundle [`sjl-13-001`](../sjl-13-001/README.md) judges the
first step and holds the correspondence both rest on.

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-13.1 with bundle `sjl-13-001`'s rule,
before any change either judges existed.

**Records:** [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
the area verdict; its decision D19 reads step 2.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules only, 2026-10-26, at SJL-13.1. **Rules only. Nothing here is a verdict, and nothing has been taken.**
A collector joins this bundle when there is a run to take.

## 1. SJL-13's area rule for step 2 (SJL-13.1)

**Step 2 changes no outcome, no byte and no diagnostic** (JSD-0083 decision D19). The rule therefore admits no class
but part A's harness classes.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The base commit** and **the after commit** do not exist yet. The after commit is the default-branch merge that
  lands step 2, and the base commit is its first parent, which descends from the commit bundle `sjl-13-001` names as
  its after commit. This README names each, by all forty digits, when it lands.

**The rule has seven parts**, each deciding alone: part A, test262 in both forms on hosted runners (template T1), with
class C4 only; part B, artifact identity over the parse corpus's sub-populations B1 to B5, B5's ordered module
specifiers among them, with no class (template T6); part C, determinism and the five checks rows of bundle
`sjl-13-001`'s part C (layer L13); part D, the front end's rules, with rule I5 registering every member of `JsSyntax`
and of the adapter; part E, the fuel-parity twins (template T7); part F, this family's compile measurement over bundle
`sjl-13-001`'s workloads (template T5), not a gate clause; part G, source fuzzing over bundle `sjl-13-001`'s seeds,
both sessions (layer L9).

**The pins** are bundle `sjl-13-001`'s.

## 2. What this bundle will hold

- **`base/`** and **`after/`**: part A's dispatches, imported unchanged with the digest of each zip; parts B to E and
  G at each commit; part A's comparisons and C4 re-runs.
- **`controls/`**: part B's planted differences, part C's K1 to K4, part D's I5 control and part G's planted throws,
  each with its output.
- **`measurement/`**: part F's sub-bundle, with its own README and manifest.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-13.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
