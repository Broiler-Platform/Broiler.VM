# The shared JavaScript libraries programme — status ledger

**Last updated:** 2026-10-12, after the four repositories' SJL-0 changes were merged into their default
branches (no state moves). Before that: 2026-10-12, at the Broiler.JS and component records of SJL-0
(SJL-0 `In progress`:
every clause, 1 to 12, met on records and bundles in its own repository, and none accepted; every
decision of SJL-0.7's first list recorded in each repository that carries a part of it). Before that:
2026-10-11, at SJL-0's first records (SJL-0 `In progress`: Broiler.VM's records
filed, its decision rules committed and its baseline bundle `sjl-0-001` retained; the Broiler.JS and
component clauses unmet; the decisions whose records are Broiler.VM's alone recorded, and the others
recorded in Broiler.VM and still owed in Broiler.JS or a component repository). Before that:
2026-10-11, later the same day (every decision point taken, none recorded yet; milestone SJL-17 added
for collation; every milestone still `Not started`). Before that: 2026-10-11 (ledger created; every
milestone `Not started`; every decision open).

**Authority:** this file is the authoritative current-evidence ledger for the milestones in
[the programme roadmap](roadmap.md). The roadmap and the [milestone files](milestones/) define the
planned work and the objective exit gates. This ledger records whether those gates have evidence.

**Today SJL-0 and SJL-1 are `In progress` and every other milestone is `Not started`:**

- no line of a shared library exists in any repository; SJL-1's corpus, validator and runners are
  test material in Broiler.JS and a test-only mode of Broiler.VM's conformance root;
- the records of SJL-0 are filed in all four repositories and merged into each one's default branch;
- four bundles exist: Broiler.VM's [`sjl-0-001`](../evidence/sjl-0-001/README.md) and
  [`sjl-1-001`](../evidence/sjl-1-001/README.md), and Broiler.JS's `docs/evidence/sjl-0-001/` and
  `docs/evidence/sjl-1-001/`.

*(Revised 2026-10-12, at SJL-1's bundles. The paragraph read: "**Today SJL-0 is `In progress` and
every other milestone is `Not started`:**", and its first and last items read "no line of the
programme's code exists in any repository;" and "two bundles exist, Broiler.VM's `sjl-0-001` and
Broiler.JS's `docs/evidence/sjl-0-001/`, each with its own repository's decision-rule files of SJL-1
and SJL-3 beside it.")*

*(Revised 2026-10-12, at the Broiler.JS and component records. The last two items read: "Broiler.VM's
records of SJL-0 are filed, and no record in Broiler.JS, Broiler.Unicode or Broiler.RegEx is;" and
"one bundle exists, Broiler.VM's `sjl-0-001`, with the decision-rule files of SJL-1 and SJL-3 beside
it.")* *(Revised again 2026-10-12, after the merges. The second item ended: "on each repository's
branch `claude/busy-lamport-m3yprr`, none yet merged into its default branch;".)*

*(Revised 2026-10-11, at SJL-0's first records. The paragraph read "**Today every milestone is `Not
started`:**", followed by three items: "no line of the programme's code exists;", "no record it names
has been filed in any repository;" and "no bundle under any repository's `docs/evidence/sjl-*`
exists.")*

A reader who finds a later row moved should find a dated paragraph above the tables saying why,
with the paragraph it replaces kept beneath it.

**2026-10-11, later the same day: every decision point is taken, and no milestone moves.** The
owner, who holds every role, took the recommendation of each decision that has one, and chose (a)
for SJL-D-13, SJL-D-14 and SJL-D-23, whose recommendations chose no single alternative
([decisions.md](decisions.md)). The decision rows below move from `open` to `taken`. Each names the
work packages that file its record, and **a gate clause that needs a decision is met only by that
record**. Three consequences show in the milestone rows:

- **SJL-D-7's record has a date.** Until SJL-0.4 files its JSD, JSD-0005's stop condition still
  takes the Broiler.JS seed snapshot on 2026-11-30.
- **SJL-3 is no longer a point at which the programme can stop.** SJL-D-23 was taken before the
  pilot: continue. Update rule 10 is revised to match.
- **Collation is milestone SJL-17**, after SJL-9, under SJL-D-24 (c). Its row is added,
  `Not started`.

No code, record or bundle exists, so every milestone stays `Not started`. This paragraph replaces
none; the one above still holds.

**2026-10-11, at SJL-0's first records: SJL-0 moves to `In progress`, and nothing else moves.**
Broiler.VM's records are filed: [ADR 0014](../adr/0014-shared-javascript-libraries.md), its index row and the corrected count sentence;
rows 24 to 41 of ADR 0003 section 11's register, every one `Proposed`; the editorial pointer
paragraphs beside ADR 0011's standing refusals and beside roadmap section 8's table; route [MVP-16](../mvp.md#5-routes-taken-without-a-decision);
the dated corrections of the core ledger and the README; and the JavaScript profile's [JSD-0063](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md)
with correction entries JSC-291 to JSC-295. **Decision `SJL-D-7`'s record is filed before
2026-11-30**, so JSD-0005's stop condition no longer takes the seed snapshot. The decision-rule files
of SJL-1 and SJL-3 are committed in Broiler.VM, with the workloads part C of SJL-3's rule measures,
before any code they judge. Bundle [`sjl-0-001`](../evidence/sjl-0-001/README.md) retains the
inventory re-derived at named commits and every base of SJL-0.8's Broiler.VM half, at commit
`0087e8d7`, with what it excludes. **No record or bundle exists in Broiler.JS, Broiler.Unicode or
Broiler.RegEx**, so clauses 8 to 12 are unmet, and the decision rows below say which records are
still owed there. The other milestones stay `Not started`: an SJL-0 record is not their code, record
or bundle.

**2026-10-12: the Broiler.JS and component records of SJL-0 are filed, every clause is met, and SJL-0
stays `In progress`.** In Broiler.JS, `a69eeca` files the revisions SJL-0.5 names, each dated and
quoting what it replaces, and the plan and status pair `docs/roadmap/SharedLibraries.md` and
`docs/roadmap/SharedLibraries.status.md`, whose plan carries Broiler.JS's part of every decision of
SJL-0.7's first list (clauses 8 and 11). `1410079` commits Broiler.JS's decision-rule files of SJL-1
and SJL-3 and the reference-arm rule, with the per-path test262 comparison script and the
measurement's workloads, before any code they judge (clause 10). `31f92a6` retains Broiler.JS's
bundle `sjl-0-001` at base commit `44a214dc`: whole-suite test262 per path at the pin in the canonical
CI configuration, every packable project's nuspec and contents, an interim public API listing, and
the pilot's base measurement on a smoke lane, with what it excludes (clause 9). Broiler.Unicode
`6c4baf7` adds a `docs/roadmap.md` carrying items U1 to U10, and Broiler.RegEx `0814aa5` adds its
roadmap's section 6 carrying requirements R1 to R11, each with its owner (clause 12). Every one of
these commits is on its repository's branch `claude/busy-lamport-m3yprr`, as Broiler.VM's are, and
none is yet merged into its default branch. These are observed state of those repositories: each
group of clauses is met by its own repository's records, and this ledger links them. **SJL-0 stays
`In progress`**, as the status vocabulary requires of a milestone whose every clause is met while
review is deferred: nothing is accepted. The decision rows below name the records in each
repository. The other milestones stay `Not started`. This paragraph replaces none; the ones above
still hold.

**2026-10-12, later the same day: the four repositories' SJL-0 changes are merged, and nothing
moves.** Broiler-Platform/Broiler.VM#115, Broiler-Platform/Broiler.JS#1019,
Broiler-Platform/Broiler.Unicode#14 and Broiler-Platform/Broiler.Regex#5 were merged into their default
branches with merge commits (`80fe1d0`, `852e870`, `0faa1f9` and `0799342`), so every commit the
paragraph above cites is on its repository's default branch. That paragraph said none was yet merged;
this one corrects that and nothing else. SJL-0 stays `In progress`, because nothing is accepted.

**2026-10-12, later again: SJL-1 moves to `In progress`, with every clause met on retained records,
the Broiler.VM ones under a revised rule.** In Broiler.JS, on branch `claude/busy-lamport-m3yprr` and
not yet merged:
- `966140a` files the corpus home `conformance/vectors/`: its schema, README, revision log and
  negative controls;
- the same commit files the number area, generated by an exact reference computation, and the test
  project with the validator, the library-level runner and Broiler.JS's engine-level runner;
- `9b0f2dd` and `593f2c5` raise the corpus to revisions 2 and 3, classifying every difference of each
  engine's first run;
- `687a081` retains Broiler.JS's bundle `sjl-1-001`, which reads its run against Broiler.JS's
  predeclared rule as met, with one judgement it states.

In Broiler.VM:
- `src/tests/vectors/pins/` holds the archive of revision 3 and its pin;
- rule N13 lists `tests/vectors`, with a third witness;
- the conformance root gains a `--vectors` mode;
- bundle [`sjl-1-001`](../evidence/sjl-1-001/README.md) retains this engine's runs.

That bundle's README gives two decisions:
- **NOT MET** under the rule committed at SJL-0, whose class (d) does not admit the differences on
  `impl-defined` vectors and whose negative control cannot run as worded;
- **MET** under a revision written after the first run was seen, which is weaker evidence.

The run also yields **a finding for SJL-3**, which this ledger records and no plan acts on yet:
SJL-3.3's table chooses Broiler.VM's algorithm for `Number::toString` at radix 2 to 36, and the
vectors show that algorithm failing the specification in each of the three ways the bundle
lists. SJL-3.1's area verdict is where the choice is revisited.

SJL-1 stays `In progress`, because nothing is accepted. This paragraph replaces none; the ones above
still hold.

**2026-10-12, later still: SJL-1's changes are merged, and nothing moves.**
Broiler-Platform/Broiler.JS#1020 and Broiler-Platform/Broiler.VM#117 were merged into their default
branches with merge commits (`75f2e9f` and `8578c65`), so every commit the paragraph above cites is
on its repository's default branch. Broiler.JS#1020 also carries `4ab17ce`, which checks the corpus
out with LF line endings on every platform and changes no file's content. That paragraph said the
Broiler.JS commits were not yet merged; this one corrects that and nothing else. SJL-1 stays
`In progress`, because nothing is accepted.

---

## 1. Reading this ledger

The three categories Broiler.VM's core ledger keeps apart are kept apart here:

- **Plan** is proposed scope, sequencing, ownership or an exit gate. It is not evidence.
- **Observed repository state** is a reviewable fact about a checkout. It can explain a status but
  cannot by itself satisfy a gate.
- **Accepted evidence** is an immutable evidence bundle that satisfies an exit gate. It names the
  exact sources and the gate, records the commands and the environment, keeps their outputs, and
  shows every clause met. Under [`docs/mvp.md`](../mvp.md), no milestone reaches `Accepted` while
  review is deferred.

**This ledger spans repositories, and it keeps them apart by naming which is which.** Every exit
gate in this programme is split into three groups of clauses:

- Broiler.VM clauses;
- Broiler.JS clauses;
- component clauses (Broiler.Unicode and Broiler.RegEx). *(Corrected 2026-10-11: the list read
  "and Broiler.DateTime if chosen". Decision SJL-D-1 placed time data in the Broiler.JS
  repository.)*

Each group is met only by a bundle collected in its own repository. A row here may *link* to
another repository's bundle or status file as observed state of that repository. That link explains
a status and never meets a clause in another repository's group. Broiler.JS keeps its own status
file for its clauses, `docs/roadmap/SharedLibraries.status.md`, created at SJL-0. Neither ledger
advances the other's rows, and neither advances the JavaScript profile's ledger, which records
observed state under its own legend.

**No figure of any run appears here.** A row cites the bundle.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned code, record or bundle exists in any repository. Planning text does not change this state. |
| `In progress` | The milestone owns code, records or a retained bundle in at least one repository, and at least one clause is unmet. The row names the unmet clauses individually, with their repository. A milestone whose every clause is met on retained evidence, but which is not accepted because review is deferred, also stays `In progress` and says both halves. |
| `Blocked` | A named clause cannot be met because of a decision or precondition another party holds. The row names the holder and the unblock condition, and is `In progress` on the rest. |
| `Refused` | The milestone's area failed the programme's extraction gate (roadmap section 5), and its record is filed. **A met gate for the refusal, and an end for the area.** |
| `Accepted` | Every clause in every repository is covered by accepted evidence, and the owners and a reviewer have recorded a decision. Unreachable while review is deferred. |

`Refused` is the one state Broiler.VM's other ledgers do not use. It is added because this
programme's gate can legitimately end an area. The universal bytecode programme instead records a
refusal through its first milestone's verdict.

---

## 2. Current milestone status

Rows are in delivery order (roadmap section 4), not numeric order.

| Milestone | Objective, in one line | State | Unmet clauses / holder | Evidence | Last moved |
|---|---|---|---|---|---|
| SJL-0 | Records, decisions, baselines and predeclared rules in four repositories | `In progress` | **Every clause is met on records and bundles in its own repository, and none is accepted**, because review is deferred under `docs/mvp.md`. **Broiler.VM:** clauses 1 to 7, on its records and bundle `sjl-0-001`, which retains every base of SJL-0.8's Broiler.VM half at `0087e8d7` and names what it excludes (the pristine consumer's restores, which fail at that commit, and ILC's mstat records). **Broiler.JS** (observed): clauses 8 to 11, on `a69eeca`, `1410079` and its bundle `docs/evidence/sjl-0-001/` (`31f92a6`) at base `44a214dc`, which names what it excludes (a run in the CI environment, and a controlled measurement lane); its status file is `docs/roadmap/SharedLibraries.status.md`. **Components** (observed): clause 12, on Broiler.Unicode `6c4baf7` and Broiler.RegEx `0814aa5`. Every record is merged into its repository's default branch *(2026-10-12; read "Every record outside Broiler.VM is on branch `claude/busy-lamport-m3yprr`, not yet merged.")*. `SJL-D-7`'s record is filed ([JSD-0063](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md)), so the 2026-11-30 deadline is met. | Broiler.VM [`sjl-0-001`](../evidence/sjl-0-001/README.md); Broiler.JS `docs/evidence/sjl-0-001/` (observed) | 2026-10-12 (Broiler.JS's and the components' records, rules and bundle) |
| SJL-1 | The shared conformance vectors | `In progress` | **Every clause is met on retained records, and none is accepted**, because review is deferred. **Broiler.JS (home):** clauses 1 to 3 and its half of clause 6, on corpus revision 3 and bundle `docs/evidence/sjl-1-001/` (`687a081`, merged in `75f2e9f`) *(2026-10-12; read "(`687a081`, not yet merged)")*, met under its rule with one judgement its README states. **Broiler.VM:** clause 4 on the pin of revision 3 and rule N13's fourth suite directory and witness; clauses 5 and 6 met under the rule revised on 2026-10-12 after the first run was seen, and **not met under the rule as committed**. A finding for SJL-3 (SJL-3.3's radix algorithm) is recorded above | Broiler.JS `docs/evidence/sjl-1-001/`; Broiler.VM [`sjl-1-001`](../evidence/sjl-1-001/README.md) | 2026-10-12 |
| SJL-2 | The admission path | `Not started` | All. The records of decisions `SJL-D-1` to `SJL-D-5`, `SJL-D-8` and `SJL-D-9`, all taken, are SJL-0's to file, apart from `SJL-D-3`'s `docs/publishing.md`, which SJL-2.13 files. | None | 2026-10-11 (created) |
| SJL-3 | Number conversion, the pilot | `Not started` | All. Decision `SJL-D-23` was taken before the pilot (continue), so SJL-3's exit stops nothing. | None | 2026-10-11 (created) |
| SJL-12 | The frontend contract and in-place preparation | `Not started` | All. Its in-place work packages may start after SJL-0. | None | 2026-10-11 (created) |
| SJL-4 | Numerics | `Not started` | All | None | 2026-10-11 (created) |
| SJL-5 | Text algorithms | `Not started` | All | None | 2026-10-11 (created) |
| SJL-6 | Date arithmetic and the Date string grammar | `Not started` | All | None | 2026-10-11 (created) |
| SJL-7 | Unicode character data and algorithms | `Not started` | All | None | 2026-10-11 (created) |
| SJL-8 | Time-zone and calendar data | `Not started` | All | None | 2026-10-11 (created) |
| SJL-11 | Regular expressions | `Not started` | All | None | 2026-10-11 (created) |
| SJL-10 | Temporal core | `Not started` | All | None | 2026-10-11 (created) |
| SJL-9 | Locale data and Intl algorithms | `Not started` | All | None | 2026-10-11 (created) |
| SJL-17 | Collation, shared after SJL-9 | `Not started` | All | None | 2026-10-11 (created, by decision `SJL-D-24`) |
| SJL-13 | The shared syntax library and Broiler.VM's wide path | `Not started` | All | None | 2026-10-11 (created) |
| SJL-14 | Broiler.JS on the shared syntax library | `Not started` | All | None | 2026-10-11 (created) |
| SJL-15 | Shared static semantics | `Not started` | All | None | 2026-10-11 (created) |
| SJL-16 | Close-out | `Not started` | All | None | 2026-10-11 (created) |

### Decision points

"Taken" means the holder chose and the plan follows the choice. "Not recorded" means the record
that formalises it is not yet filed; the work packages named file it.

| Decision | State | Record filed by | Holder |
|---|---|---|---|
| SJL-D-1: where each library lives | **taken 2026-10-11**: (b); locale data (b-ii) and time data (b-iii) in the Broiler.JS repository; **recorded in Broiler.VM** by [ADR 0014](../adr/0014-shared-javascript-libraries.md) decision 4; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5), and **in the component repositories** by Broiler.Unicode's `docs/roadmap.md` and Broiler.RegEx's roadmap section 6 (SJL-0.6) *(2026-10-12; read "not yet recorded in the home repositories' roadmaps (SJL-0.5, SJL-0.6)")* | SJL-0.2, SJL-0.5, SJL-0.6 | Broiler.VM architecture owner, Broiler.JS maintainer, component maintainers |
| SJL-D-2: package identity and granularity | **taken 2026-10-11**: (a) `Broiler.ECMAScript.*`; **recorded in Broiler.VM** by ADR 0014 decision 4; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "not yet recorded in the Broiler.JS plan (SJL-0.5)")* | SJL-0.2, SJL-0.5 | Home release owners with Broiler.VM's R6 |
| SJL-D-3: versioning, compatibility and co-hosting | **taken 2026-10-11**: a separate stream, minimum ranges with lock files; **recorded in Broiler.VM** by ADR 0014 decision 5; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "not yet recorded in the Broiler.JS plan (SJL-0.5)")*; its `docs/publishing.md` is SJL-2.13's | SJL-0.2, SJL-0.5, SJL-2.13 | Same |
| SJL-D-4: how Broiler.VM's CI obtains shared packages | **taken 2026-10-11**: (c) both sources; **recorded** by ADR 0014 decision 5, and its Broiler.JS part (previews reach Broiler.VM through the committed feed; a Broiler.VM release's dependencies are on nuget.org first) in Broiler.JS by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(added 2026-10-12)* | SJL-0.2, SJL-0.5 | Broiler.VM's R6 |
| SJL-D-5: review and assurance regime | **taken 2026-10-11**: (a) full parity; **recorded in Broiler.VM** by ADR 0014 decision 6; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "not yet recorded in the Broiler.JS plan (SJL-0.5)")* | SJL-0.2, SJL-0.5 | Broiler.VM's R6, Broiler.JS maintainer |
| SJL-D-6: data versions and data packaging | **taken 2026-10-11**: (c) shared generators and pins; listed in ADR 0014 decision 4; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5), and **in Broiler.Unicode** by its `docs/roadmap.md`, items U4 and U5 (SJL-0.6); its area-verdict JSD is SJL-7.1's *(2026-10-12; read "its records are the Broiler.JS plan and the data homes' roadmaps (SJL-0.5, SJL-0.6), not yet filed, and the area-verdict JSD of SJL-7.1")* | SJL-0.5, SJL-0.6, SJL-7.1 | JavaScript profile owner, Broiler.JS maintainer, Broiler.Unicode maintainer |
| SJL-D-7: re-scope of JS-2 | **taken 2026-10-11**: (a) re-scope; **recorded 2026-10-11**, before the 2026-11-30 deadline, by [JSD-0063](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md) and correction entries JSC-293 and JSC-295 | SJL-0.4 | JavaScript profile owner |
| SJL-D-8: scope reading of ADR 0011 | **taken 2026-10-11**: (a) a route; **recorded** by ADR 0014 decision 1, route [MVP-16](../mvp.md#5-routes-taken-without-a-decision) and the pointer paragraphs beside ADR 0011's standing refusals and roadmap section 8's table | SJL-0.2, SJL-0.3 | Core-contract owner (R5) with the architecture owner |
| SJL-D-9: fork policy | **taken 2026-10-11**: the four-part policy; **recorded in Broiler.VM** by ADR 0014 decision 3 and JSD-0063 section 2 (JSC-291); **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "not yet recorded in the Broiler.JS plan (SJL-0.5)")* | SJL-0.2, SJL-0.4, SJL-0.5 | Architecture owner, JavaScript profile owner, Broiler.JS maintainer |
| SJL-D-10: metering contract | **taken 2026-10-11**: (a) the `WorkCharge` delegate; not recorded | SJL-2.10, SJL-3.1 | JavaScript profile owner, Broiler.JS maintainer |
| SJL-D-11: diagnostic codes and message text | **taken 2026-10-11**: codes and message identifiers; not recorded | SJL-12.2 | Same |
| SJL-D-12: canonical tree, and Broiler.JS `Ast` and `Parser` compatibility | **taken 2026-10-11**: (a) with (i); not recorded | SJL-12.2, SJL-14.4 | Broiler.JS maintainer, JavaScript profile owner |
| SJL-D-13: Broiler.VM's slice path | **taken 2026-10-11**: (a) rebuilt on the shared tree and model; not recorded | SJL-15.4 | JavaScript profile owner |
| SJL-D-14: regular-expression implementation base | **taken 2026-10-11**: (a) Broiler.Regex fixed, then both engines move; (i) duplicate named groups; listed in ADR 0014 decision 4; **recorded in Broiler.RegEx** by its roadmap section 6 (SJL-0.6) and in Broiler.JS by `docs/roadmap/SharedLibraries.md`; its area-verdict JSD is SJL-11.1's *(2026-10-12; read "not yet recorded in Broiler.RegEx's roadmap (SJL-0.6)")* | SJL-0.6, SJL-11.1 | Broiler.RegEx maintainer, JavaScript profile owner, Broiler.JS maintainer |
| SJL-D-15: BigInt width policy for Broiler.JS | **taken 2026-10-11**: (a) unbounded; not recorded | SJL-4.1 | Broiler.JS maintainer |
| SJL-D-16: observable corrections | **taken 2026-10-11**: (a), with (c) only for a known dependent consumer; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "not yet recorded in the Broiler.JS plan (SJL-0.5)")*; its pilot JSD is SJL-3.1's, then each area's changelog | SJL-0.5, SJL-3.1, each area | Broiler.JS maintainer, JavaScript profile owner |
| SJL-D-17: non-standard extensions stay in Broiler.JS | **taken 2026-10-11**: they stay; listed in ADR 0014 decision 8; **recorded in Broiler.JS** by `docs/roadmap/SharedLibraries.md` (SJL-0.5) *(2026-10-12; read "its record, the Broiler.JS plan (SJL-0.5), is not yet filed")* | SJL-0.2, SJL-0.5 | Broiler.JS maintainer |
| SJL-D-18: whole-runtime unification not planned | **taken 2026-10-11**: not planned; **recorded** by ADR 0014 decision 8, and in Broiler.JS by `docs/roadmap/SharedLibraries.md` *(added 2026-10-12)*; confirmed at SJL-16 | SJL-0.2; confirmed at SJL-16 | Architecture owner, Broiler.JS maintainer |
| SJL-D-19: host policy as provider interfaces | **taken 2026-10-11**: providers; not recorded | SJL-6.1 | JavaScript profile owner, Broiler.JS maintainer |
| SJL-D-20: time-zone dataset policy | **taken 2026-10-11**: Broiler.VM's policy; not recorded | SJL-8.1 | Same |
| SJL-D-21: calendar sources | **taken 2026-10-11**: ICU4X sources; not recorded | SJL-8.1 | Same |
| SJL-D-22: locale coverage and CLDR ownership | **taken 2026-10-11**: (a) with (c); not recorded | SJL-9.1 | JavaScript profile owner, Broiler.JS maintainer, Broiler.Unicode maintainer |
| SJL-D-23: continue past the pilot | **taken 2026-10-11, before the pilot**: (a) continue; **recorded** by ADR 0014 decision 7 and JSD-0063 section 7, and in Broiler.JS by `docs/roadmap/SharedLibraries.md` *(added 2026-10-12)* | SJL-0.2, SJL-0.4 | Architecture owner, Broiler.JS maintainer |
| SJL-D-24: collation | **taken 2026-10-11**: (c) later, as SJL-17; not recorded | SJL-9.1 | JavaScript profile owner, Broiler.JS maintainer |
| SJL-D-25: disposition of emptied Broiler.JS public types | **taken 2026-10-11**: forwarders by default; not recorded | SJL-3.5, then each area | Broiler.JS maintainer |

*(The table read `open` in every State cell until 2026-10-11, later the same day, and had no "Record
filed by" column. At SJL-0's first records the same day, the rows of the decisions SJL-0 records
gained what is recorded and where; each read "not recorded" before, and `SJL-D-7`'s also read "**the
record is due before 2026-11-30**". The decisions later milestones record still read "not recorded",
and the "Record filed by" column names the milestone for each.)*

All holders are one person, recorded in Broiler.VM as EX-30.

### Observed repository state on this ledger's date

These facts explain why rows are `Not started` and what the first milestones meet. None of them is
evidence for any clause.

Bundle `sjl-0-001` re-derived the inventory at Broiler.VM `0087e8d7`, the base its baselines were
taken at, and at the other repositories' revisions below, which had not moved.

| Repository | Revision | Fact |
|---|---|---|
| Broiler.VM | `88d8b51d` | Rules A1, A2, A3 and N1 refuse every shared package. The roadmap's sections 5, 9 and 12 and the JavaScript profile's invariant 11 refuse any edge to Broiler.JS. |
| Broiler.JS | `44a214dc` | Its documents state that no Broiler.VM integration is planned. Its `FrontEnd/Semantics` boundary is a hypothesis pending its own MOD-M2. |
| Broiler.Unicode | `2affa78f` | Published at preview.3. No roadmap, no identifier series, no assurance annotations. |
| Broiler.RegEx | `c1422f4d` | Published at preview.4. Its budget is reset per start position, and has no caller charge and no cancellation. |
| Broiler.DateTime | `8de12cc3` | No consumer anywhere. Its civil-day primitives are internal in every committed state; a change that makes them public exists only in an uncommitted stash. |

---

## 3. Required evidence bundles

Each repository with clauses retains its bundles under its own `docs/evidence/sjl-n-nnn/`, in
Broiler.VM's bundle shape:

- a `README.md` stating what the bundle demonstrates, what it excludes and what it does not claim;
- a `manifest.json` naming every input, script, package and binary by hash, and the commit each was
  built from. **For every admitted shared package: its identifier, its version, its content hash and
  the source commit from its nuspec** (rule `I7`);
- the retained logs the milestone file lists;
- where the milestone judges a comparison, the `decision-rule.md` committed before the code it
  judges.

A bundle holds figures where it must. No document outside a bundle quotes them. Text files are
stored with LF line endings before they are hashed.

---

## 4. Update rules

1. **Same change.** Update this ledger in the same change that lands, blocks, refuses, supersedes or
   narrows a milestone's claim. Keep earlier evidence links and decisions as dated history. Correct
   a sentence by quoting the superseded reading beside the new one, never by editing it away.
2. **No gate copied into the evidence column.** Link the immutable bundle, and state what it
   demonstrated, including failures and exclusions.
3. **No transitive completion.**
   - Broiler.JS's clauses met do not meet Broiler.VM's.
   - A vector agreement does not meet a test262 clause.
   - JIT, trimmed or one-mode success does not meet a Native AOT clause.
   - One area met meets no other area.
4. **`In progress` once anything exists.** A milestone that owns any code, record or bundle, in any
   repository, is `In progress`. A milestone with a clause gated on an open decision is `Blocked`
   on that clause and `In progress` on the rest, and the row says so in those words.
5. **Gate changes.** If a gate changes, record the revision in the milestone file with the
   superseded text quoted, and re-evaluate existing evidence. Evidence gathered for an older
   population is not carried forward silently.
6. **Separate ledgers.** Do not record another ledger's status here, and do not advance another
   ledger's row from here: Broiler.VM's core ledger, the JavaScript profile's ledger, Broiler.JS's
   ledgers, the component repositories' records. A link to another ledger's row is observed state.
7. **`Accepted` needs review.** A milestone moves to `Accepted` only after its owners and a reviewer
   confirm every clause in every repository is covered, with the decision date and the bundle
   identifiers in the row. Under `docs/mvp.md` no row reaches it while review is deferred.
   **Human review gates a release, not a development step.**
8. **No figures.** No count, timing, ratio or score enters this file, whether or not a bundle holds
   it.
9. **Refusals.** A refused area moves to `Refused` in the change that files its verdict. The row
   names the failed condition, what would reopen it, and where the source-level pointers are.
   Reopening is a new dated record and moves the row back to `In progress`.
10. **The pilot decision.** Decision `SJL-D-23` was taken before the pilot: the programme
    continues. Its record is filed at SJL-0, so SJL-3's row moves on SJL-3's own clauses only. The
    rows of SJL-4 to SJL-11, SJL-13 to SJL-15 and SJL-17 meet their gates on the milestones they
    wait on, and on nothing else. *(Revised 2026-10-11, when `SJL-D-23` was taken. The rule read:
    "Decision `SJL-D-23` is recorded in the change that meets or refuses SJL-3. Until it is
    recorded, the rows of SJL-4 to SJL-11 and SJL-13 to SJL-15 may start but may not meet a
    gate.")*
