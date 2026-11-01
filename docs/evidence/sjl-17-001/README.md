<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-17-001 - collation: Broiler.VM's rule, correspondence and evidence

**Milestone:** SJL-17 of the shared JavaScript libraries programme,
[`sjl-17-collation.md`](../../javascript-shared-libraries/milestones/sjl-17-collation.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-17.1 before any code it judges existed: no
shared collation member and no cut-over.

**Records:** [JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md),
the area verdict, which records the owner's decisions E1 to E14 of 2026-11-01 and amends JSD-0043 §5 to §7 where they
move. Its input is [`correspondence.md`](correspondence.md), with:
- [`probes/`](probes/), every probe as written and as run, the two generators, and the list of test262 files run;
- [`runs/`](runs/), the harness scripts and each column's answers: `vm/` for this family, `js/` for Broiler.JS, and
  `witness/` for the reference Node.js release, which is a witness and never the arbiter of a class.

`probes/` and each folder of `runs/` carry a `hashes.txt` that `sha256sum -c` verifies; `probes/generated.sha256`
records the two CollationTest probes, which the bundle does not retain and `probes/scripts/make_collationtest_probes.py`
writes again byte for byte. A run file's commands name the machine's worktrees as run.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rule and correspondence only, 2026-11-01, at SJL-17.1. **Nothing here is a verdict, and no run of the rule
has been taken.**
- Every run of the rule is taken at the base commit or the after commit, which the rule defines before either exists
  (JSD-0084's decision E14): the after commit is the default-branch merge that lands SJL-17.4's cut-over, and the base
  commit is its first parent.
- A collector joins this bundle when there is a run of the rule to take.

Broiler.JS's clauses, and the home's, are judged in Broiler.JS's bundle of the same name. This bundle reads none of
their runs.

## 1. SJL-17's area rule and its correspondence (SJL-17.1)

**The correspondence.**
- It was read from code at this family's `83290f9` and Broiler.JS's `61cb15b5`, on 2026-11-01, and every behavioural
  cell its rows rest on was run in both engines: this family through the conformance root, in the bytecode form and
  in the native form, and Broiler.JS through its shell, every probe again under invariant globalization, and the rows
  that read the default locale under a German and a Turkish host culture.
- **Every row is classified**, in one of the seven classes bundle `sjl-9-001` used, and no row's class is unknown.
- **This family has no correction in this area.** Row O3, Han across blocks, is a reading SJL-17.2's vectors check
  (finding F1).
- **X1 is an adoption** (decision E2): Broiler.JS implements no collation algorithm.
- **The findings against the plan and the records**, F1 to F7, are each answered in JSD-0084.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0084 records decisions E1 to E14 under it. It changes no programme rule: nothing is accepted without
review, rules still come before runs, and SJL-17 may not meet its gate before SJL-9 meets SJL-9's.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The base commit** and **the after commit** do not exist yet. The rule states the conditions each must meet, and
  this README names each, by all forty digits, when it lands. The base commit descends from the rule commit and from
  the commit bundle `sjl-9-001` names as its after commit here, which does not exist yet either.

**The rule has three parts**, each deciding alone.
- **Part A** takes test262 in both forms with the hosted workflow, at the base commit and at the after commit
  (template T1). JSD-0084 names no correction of this family, so a C1 row is a finding against its correspondence.
  There is no class C2, C3 or C5. A wall-clock race is admitted under C4's (c') and (e).
- **Part B** holds `src/tests/cldr/orderings/` with its retained answer, the archived CollationTest files, the
  collation checks of the slice compiler, and every other Intl, tzdb and Temporal check as a guard: every check passes
  at both commits, and nothing under `src/tests/cldr/` or `src/tests/temporal/` changes. No class is admitted.
- **Part C** holds the wide twins to their base figures, with no class (template T7).

**The pins do not move**: test262 `ccaac100`; this family's CLDR 48.2.0, as `src/tests/cldr/pins/cldr.pin` pins it
(rule N27), its UCA, and its collation tables in `JsCldrTables.g.cs`, byte for byte (rule N28): the root, root
`search`, and German `phonebook` and `search`.

## 2. What this bundle will hold

- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's check runs and the
  datasets' state at the base commit; part C's `fuel-parity.log` with each twin's bisected allowance.
- **`after/`**: the same at the after commit, with part A's comparisons and its (c') and (e) re-runs, part B's
  comparison, and part C's runs at each ceiling tried.
- **`vectors/`**: this family's runs of the collation vectors SJL-17.2 adds, at the corpus revision its pin names.

The correspondence's `probes/` and `runs/` stay as committed.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the section
that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and totals decide
nothing.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-13-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-17.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
