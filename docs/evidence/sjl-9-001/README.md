<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-9-001 - locale data and the Intl algorithms: Broiler.VM's rules, correspondence and evidence

**Milestone:** SJL-9 of the shared JavaScript libraries programme,
[`sjl-9-locale-data-and-intl.md`](../../javascript-shared-libraries/milestones/sjl-9-locale-data-and-intl.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-9.1 before any code it judges existed: no
data-home generator, no shared Intl member and no cut-over.

**Records:** [JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
the area verdict, which records decisions SJL-D-22 and SJL-D-24 and the owner's decisions D1 to D35 of 2026-10-26. Its
input is [`correspondence.md`](correspondence.md), with:
- [`intl402-map.tsv`](intl402-map.tsv), every test262 `intl402` file of the base report it reads that exercises a
  row, with its rows, the locale data it needs and each binary's verdict;
- [`intl402-map-no-row.tsv.gz`](intl402-map-no-row.tsv.gz), every other `intl402` file of that report, with the
  milestone that owns it;
- [`probes/`](probes/), every probe as written and as run;
- [`runs/`](runs/), each engine's answers: `vm/` for this family, `js/` for Broiler.JS, and `witness/` for Node and
  the CLDR packages, which are witnesses and never the arbiter of a class.

`probes/` and each folder of `runs/` carry a `hashes.txt` that `sha256sum -c` verifies. A large run file is kept
gzip-compressed under its name with `.gz`, and the correspondence names it without the suffix. A run file's commands
name the machine's worktrees as run.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules and correspondence only, 2026-10-26, at SJL-9.1. **Nothing here is a verdict, and nothing has been
taken.**
- Every run of the rule is taken at the base commit or the after commit, which the rule defines before either exists
  (JSD-0082 decision D4): the after commit is the default-branch merge that lands SJL-9.4's cut-over, and the base
  commit is its first parent.
- A collector joins this bundle when there is a run to take.

Broiler.JS's clauses, and the data home's and the home's, are judged in Broiler.JS's bundle of the same name. This
bundle reads none of their runs.

## 1. SJL-9's area rule and its correspondence (SJL-9.1)

**The correspondence.**
- It was read from code at this family's `cd1928d` and Broiler.JS's `f918b040`, on 2026-10-26, and every behavioural
  cell its rows rest on was run in both engines: this family through the conformance root, in the bytecode form and
  in the native form, and Broiler.JS through its shell, the host-dependent rows again under a host culture, a host
  zone and invariant globalization.
- This family's answers on the test262 files of the three proposals it does not admit, `canonical-tz`,
  `Intl.Locale-info` and `Intl.NumberFormat-v3`, came from a scratch copy that admits them. The copy's patch is
  retained as `runs/vm/admitted-proposals.patch` and was never committed.
- **Every row is classified**, in one of seven classes, and no row's class is unknown. JSD-0082 records the classes.
- **This family has three corrections**: rows Z8 (a formatter's resolved time zone), L18 (`getCollations` for a
  language the Collator lacks) and L9 (a subdivision alias whose replacement is a region). SJL-9.4's cut-over makes
  all three (JSD-0082 decisions D15 and D32).
- **The findings against the plan and the records**, F1 to F21, are each answered in JSD-0082, by a decision of
  2026-10-26 or by the record named to carry it.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." JSD-0082 records decisions D1 to D35 under it. It changes no programme rule: nothing is accepted without
review, rules still come before runs, and SJL-9 may not meet its gate before SJL-7 and SJL-8 meet theirs, and so not
before SJL-3 meets SJL-3's.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged.
- **The base commit** and **the after commit** do not exist yet. The rule states the conditions each must meet before
  SJL-9.4 merges, and this README names each, by all forty digits, when it lands. The base commit descends from the
  rule commit and from the commit bundle `sjl-12-001` names as its after commit here, `1de3af6`.

**The rule has three parts**, each deciding alone.
- **Part A** takes test262 in both forms with the hosted workflow, at the base commit and at the after commit
  (template T1).
  - Its corrections are named in JSD-0082 before any run: row Z8's, a `canonical-tz` variant skipped at the base and
    passing after, or a variant failing at the base that passes after because the formatter reports the identifier as
    given; and rows L18 and L9, which reach no variant the part scores. Any other correction is recorded as a finding
    that the correspondence missed.
  - There is no class C2: the shared members and this family's adapters charge where this family charged, in its
    units, at the points decision D18 names. There is no class C3 or C5.
  - A wall-clock race is admitted under C4's (c') and (e).
- **Part B** holds this family's reference datasets under `src/tests/cldr/` and `src/tests/temporal/` and every Intl,
  tzdb and Temporal check of the slice compiler: every check passes at both commits, and the programs, retained
  answers and divergences files are the same byte for byte, but for two classes, each listed line by line:
  - **Z8**, the resolved-name lines of a time-zone identifier that is not its own primary;
  - **L18**, the `collations` lines of `src/tests/cldr/locales/divergences.txt` for a language the Collator lacks.
- **Part C** holds the wide twins to their base figures, with no class (template T7): the command-line root carries no
  Intl data.

**The pins do not move**: test262 `ccaac100`; this family's CLDR 48.2.0, as `src/tests/cldr/pins/cldr.pin` pins it
(rule N27), and its table, `JsCldrTables.g.cs`, byte for byte (rule N28); and the text both engines follow, ECMA-402
at `tc39/ecma402@3d4c558`, with Intl over Temporal by `tc39/proposal-temporal@e8cc03fc` and the Intl era and month
code proposal (JSD-0082 decision D14).

## 2. What this bundle will hold

- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's check runs and the
  datasets' state at the base commit; part C's `fuel-parity.log` with each twin's bisected allowance.
- **`after/`**: the same at the after commit, with part A's comparisons, its table of C1 rows by kind, row and clause,
  and its (c') and (e) re-runs; part B's comparison and its list of Z8 and L18 lines, each with the line at both
  commits and the retained answer's line; and part C's runs at each ceiling tried.
- **`vectors/`**: this family's runs of the Intl vectors SJL-9.2 adds, at the corpus revision its pin names.

The correspondence's `probes/` and `runs/` stay as committed. The plan for this bundle also named a `controls/`
folder; no part of the rule defines a control, and the folder is added only if a later step names one.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and
totals decide nothing.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-9.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
