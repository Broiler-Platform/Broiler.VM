<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-9-001 - locale data and the Intl algorithms: Broiler.VM's rules, correspondence and evidence

**Milestone:** SJL-9 of the shared JavaScript libraries programme,
[`sjl-9-locale-data-and-intl.md`](../../javascript-shared-libraries/milestones/sjl-9-locale-data-and-intl.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-9.1 before any code it judges existed: no
data-home generator, no shared Intl member and no cut-over.

*(Added 2026-11-04.)* The rule is revised, before any of its runs, by
[`decision-rule-area-revised-2026-11-04.md`](decision-rule-area-revised-2026-11-04.md), on the owner's decision of
2026-11-04, which JSD-0082's dated note on decision D15 records: part B's class Z8 also admits the guard
`temporal/t1/data-without-the-tz-release-reads-its-zones`, whose expected literal and output move from the primary
`Asia/Kolkata` to the identifier as given, `Asia/Calcutta`, its verdict passing at both commits, and nothing else.
`decision-rule-area.md` is not edited, and section 1's summary of part B describes it as filed.

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

**State:** rules and correspondence, 2026-10-26, at SJL-9.1; this family's engine-level run of the vectors on its pin
at corpus revision 28, 2026-10-27, at SJL-9.2 (section 3); and its runs on the pin before and after it moved to corpus
revision 38, 2026-11-11, at SJL-9.6 (section 5); and the run of the rule, its parts A, B and C, taken and judged on
2026-11-11, at SJL-9.6 (sections 2, 6, 7 and 8). **Parts A, B and C of the rule, as its revision of 2026-11-04 reads
it, are each MET**, on the readings section 2 lists, those a verdict turns on as the owner ruled on 2026-11-11.
**Nothing is accepted**: review is deferred, these verdicts are not SJL-9's exit reading, and they meet no clause by
themselves.
*(Revised 2026-11-11, at the owner's rulings of that day, which section 2 records at the end of its readings. It read
"on the readings section 2 lists.")*
*(Revised 2026-11-11, at SJL-9.6, when the run of the rule was taken and judged. The state read "rules and
correspondence, 2026-10-26, at SJL-9.1; this family's engine-level run of the vectors on its pin at corpus revision
28, 2026-10-27, at SJL-9.2 (section 3); and its runs on the pin before and after it moved to corpus revision 38,
2026-11-11, at SJL-9.6 (section 5). **Nothing here is a verdict, and no run of the rule has been taken.**")*
*(Revised 2026-10-27, at SJL-9.2, when the pin moved to revision 28. It read "rules and correspondence only,
2026-10-26, at SJL-9.1. **Nothing here is a verdict, and nothing has been taken.**")* *(Revised 2026-11-11, at SJL-9.6,
when the pin moved to revision 38. It read "rules and correspondence, 2026-10-26, at SJL-9.1, and this family's
engine-level run of the vectors on its pin at corpus revision 28, 2026-10-27, at SJL-9.2 (section 3).")*
- Every run of the rule is taken at the base commit or the after commit, which the rule defines before either exists
  (JSD-0082 decision D4): the after commit is the default-branch merge that lands SJL-9.4's cut-over, and the base
  commit is its first parent.
- A collector joins this bundle when there is a run of the rule to take. Section 3's run is no run of the rule, and
  was taken without one. *(Revised 2026-10-27, at SJL-9.2. It read "A collector joins this bundle when there is a run
  to take.")* Section 5's runs are no run of the rule either, and were taken without one. *(Added 2026-11-11, at
  SJL-9.6.)* *(Noted 2026-11-11, at SJL-9.6's run of the rule.)* No collector joined: the run was taken, as bundle
  `sjl-13-001`'s judgement was, by short scripts kept as written in [`after/scripts/`](after/scripts/), each named
  where its output is read; section 2 says how they ran. The sentence above is left as written.

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

*(Noted 2026-11-11, at SJL-9.6.)* The three commits, by all forty digits, as section 2 reads them: the rule commit
`4cf9ca449fc96a1a5be810f6d0ca31c42b855c8f`, the merge of Broiler-Platform/Broiler.VM#171, which section 3 names as
`4cf9ca4`; the after commit `68cad8baf7403125d44d5007441d30ff07bc4edf`, the merge of Broiler-Platform/Broiler.VM#190,
SJL-9.4's cut-over; and the base commit `3b33530d354431675667087a0044648b8f617c2e`, its first parent, the merge of
Broiler-Platform/Broiler.VM#189. The items above are left as written.

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

## 2. What this bundle holds, and the run of the rule (SJL-9.6)

*(Heading revised 2026-11-11, at SJL-9.6, when the run of the rule was taken. It read "2. What this bundle will
hold". The section's first paragraphs, below, are left as written; the subsections after them are added.)*

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

### What the folders hold (2026-11-11, at SJL-9.6)

*(Added 2026-11-11, at SJL-9.6.)* The folders the list above names now hold the run of the rule:
- **[`base/`](base/)**: [`hosted/`](base/hosted/), part A's base dispatch, imported unchanged, with
  [`imported.txt`](base/hosted/imported.txt) naming the digest of its zip; [`datasets/`](base/datasets/), part B's
  check run (`checks.log`, `named-checks.txt`, and `named-check-names.txt`, the names the base commit's `IntlChecks.cs`
  spells) and the datasets' state (`files.sha256`); [`fuel-parity.log`](base/fuel-parity.log), part C's bisection;
  and [`local/`](base/local/), every local step's record at the base: the archive, the builds, and each run's gate log,
  machine before and after, command, exit code and wall time.
- **[`after/`](after/)**: [`hosted/`](after/hosted/), part A's after dispatch, imported the same way;
  [`parta/`](after/parta/), the completeness of all four runs, both comparisons, every changed verdict with its class,
  the table of C1 rows, the rows the rule's "How it is read" names, and the (c') re-runs and the (e) race with every
  report and record; [`datasets/`](after/datasets/), part B's after run, its comparison with the base, and every
  changed line under `src/tests/cldr/` and `src/tests/temporal/` with its class, its line at both commits and the
  retained answer's line; [`fuel-parity.log`](after/fuel-parity.log), part C at each ceiling tried;
  [`commits/`](after/commits/), the git reads behind the commit conditions; [`local/`](after/local/), as at the base;
  and [`scripts/`](after/scripts/), every script that took or read these files, as run, each named `.txt`.
- [`base/hashes.txt`](base/hashes.txt) and [`after/hashes.txt`](after/hashes.txt) hold the SHA-256 of every file of
  their folder outside `hosted/`, and each `hosted/hashes.txt` every file of that folder; `sha256sum -c` verifies each.
  [`manifest.json`](manifest.json) lists every file of the bundle at `collected-at`, the commit its digests describe,
  which holds the evidence; this README and the manifest are committed on top of it.

No `controls/` folder is added: no part of the rule defines a control.

### The commits, and the conditions the rule's header sets

[`after/commits/commit-conditions.txt`](after/commits/commit-conditions.txt) is the output of
[`commit-conditions.sh.txt`](after/scripts/commit-conditions.sh.txt), git reads alone. Every condition holds on those
reads, on reading 1 below for the third of the base's conditions. So no part is void.

| Commit | Condition (the rule's header) | The git read, and what it shows |
|---|---|---|
| Rule commit `4cf9ca449fc96a1a5be810f6d0ca31c42b855c8f` (Broiler-Platform/Broiler.VM#171) | carries `decision-rule-area.md`; a later bundle shows the file unchanged | `git log --diff-filter=A` names `f0023e28`, and the file's two later edits, `61fab9fb` and `8edfcb79`, are SJL-9.1's too; all three are in `4cf9ca4` and none in its first parent. The file's blob is `feb83f64…` at the rule commit, the base, the after commit and `797c637b`, this branch's head when the read was taken; no later commit of this branch changes it (reading 2) |
| After commit `68cad8baf7403125d44d5007441d30ff07bc4edf` (#190) | the default-branch merge that lands SJL-9.4's cut-over, one merge (decision D3) | on the default branch's first-parent line; parents `3b33530d` and `791d8b9d`, the branch that carries SJL-9.4's code, records, rule revision and closures |
| Base commit `3b33530d354431675667087a0044648b8f617c2e` (#189) | the after commit's first parent | `git rev-parse 68cad8b^1` is `3b33530d` |
| | descends from the rule commit | `git merge-base --is-ancestor 4cf9ca4 3b33530d`: yes |
| | descends from bundle `sjl-12-001`'s after commit `1de3af6` | yes |
| | the after commit is its only first-parent successor that changes a path under `src/Broiler.VM.Profile.JavaScript*/`, `src/compositions/`, `eng/packages/` or `Directory.Packages.props` | `git rev-list --first-parent 3b33530d..68cad8b` is `68cad8b` alone, which changes paths under all four; the cut-over landed in one merge (reading 1) |
| Part A's runs | dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base commit's | each `imported.txt` and `run.txt` names its tested commit and inputs; `.github/workflows/test262-evidence.yml` is blob `740a70f8…` and `eng/test262-evidence.py` blob `55f71ee3…` at the base, at the after commit, at `0ffc037f` (the base run's dispatched ref, the merge of #198) and at `11c53897` (the after run's, the merge of #199); so are `eng/run-test262.py`, `eng/collect-js-release-candidate.py`, `eng/compare-test262-forms.py` and the suite pin at all four |
| Parts B and C | taken locally from a clean checkout of each commit | each from a clean `git archive` of its commit on tmpfs ([`base/local/archive.txt`](base/local/archive.txt), [`after/local/archive.txt`](after/local/archive.txt)), built in Release there |

**The data and the admitted proposals** (the same file). `JsCldrTables.g.cs` and `src/tests/cldr/pins/cldr.pin` have
the same blob at both commits (rule N28). `SuiteFeatures.cs` differs between them by the `canonical-tz` entry, naming
JSD-0082, and its note, and nothing else, as part A's population says.

**The two dispatched refs differ**, `0ffc037f` for the base run and `11c53897` for the after run, because the after
run was dispatched once #199 had merged; the rule asks that the workflow and the driver at the dispatched ref be the
base commit's, which both are.

### The machine and the gate

The local steps were taken on 2026-11-11 on this family's local machine: `linux-x64`, four logical processors,
Ubuntu 24.04.5 LTS, .NET SDK 10.0.404, the native backend `x86-64-sysv` (reading 9). Each tree is a clean
`git archive` of its commit on a tmpfs, and each root was built there in Release with `--disable-build-servers`,
`MSBUILDDISABLENODEREUSE=1` and shared compilation off ([`build-roots.sh.txt`](after/scripts/build-roots.sh.txt)): the
slice compiler, the command-line root and the conformance root, one build at a time, with no warning in any build
log. Every build and every run ran behind a hard gate ([`gate.sh.txt`](after/scripts/gate.sh.txt), with
[`run.sh.txt`](after/scripts/run.sh.txt)) that waits, with no time limit, for a one-minute load of at most 1.0 and no
process whose executable (read from `/proc/<pid>/exe`, not a shell's command line) is `dotnet`, `testhost`,
`VBCSCompiler`, `MSBuild`, `ilc`, `crossgen` or a `Broiler.*` program. Its `stop_servers` kills only a process whose
own environment carries the run's tag; no build left one, and every gate log's `stop-servers` lines name none. Every
check found no such process; where the load stood above 1.0 after a build, the gate waited until it fell.

| Step | Taken (UTC) | Records |
|---|---|---|
| Part A's hosted runs | base 09:16:10 to 09:24:00, after 09:24:18 to 09:30:34 | `base/hosted/`, `after/hosted/` |
| Builds at the base | 10:01:49 to 10:04:11 | `base/local/builds/` |
| Part B at the base | 10:05:19 to 10:10:34 | `base/local/partb/`, `base/datasets/` |
| Part C at the base | 10:10:35 to 10:12:45 | `base/local/partc/`, `base/fuel-parity.log` |
| Builds at the after commit | 10:12:56 to 10:16:05 | `after/local/builds/` |
| Part B at the after commit | 10:16:36 to 10:21:18 | `after/local/partb/`, `after/datasets/` |
| Part C at the after commit | 10:21:19 to 10:21:35 | `after/local/partc/`, `after/fuel-parity.log` |
| Part A's comparisons (Python only) | 10:22:09 to 10:22:11 | `after/parta/commands.log` |
| Part A's (c') re-runs | 10:22:20 to 10:30:18 | `after/parta/rerun/` |
| Part A's (e) race | 10:30:37 to 10:30:59 | `after/parta/race/` |

No other build, test or run was on the machine. The agent that took the steps also read files beside some of them,
none a build, test or run of an engine: `git` reads and Python reads of the imported reports during part B's base run
and the builds, and copies of finished files into this bundle during the re-runs. *(This is recorded so that the
condition "no other build, test or run" is read against what happened, not against a claim of an idle machine.)*

### The verdicts

Each part decides alone, on the rule as its revision of 2026-11-04 reads it, from the files named with it.

| Part | Verdict | What it reads |
|---|---|---|
| A, test262 in both forms (section 6) | **MET**, on readings 1, 3, 6 and 11 below, as the owner ruled on 2026-11-11 | [`after/parta/`](after/parta/): `completeness.txt`, `compare-wide-bytecode.log`, `compare-wide-native.log`, `changed-verdicts.tsv`, `c1-table.tsv`, `rows-named.txt`, `rerun/`, `race/` |
| B, the reference datasets and the Intl checks (section 7) | **MET**, on readings 1, 3, 7, 8 and 12 below, as the owner ruled on 2026-11-11 | [`base/datasets/`](base/datasets/), [`after/datasets/`](after/datasets/): `checks.log`, `named-checks.txt`, `files.sha256`, `compare.txt`, `part-b-lines.tsv`, `part-b-summary.txt` |
| C, the fuel-parity twins (section 8) | **MET**, on reading 1 below, as the owner ruled on 2026-11-11 | [`base/fuel-parity.log`](base/fuel-parity.log), [`after/fuel-parity.log`](after/fuel-parity.log), each side's `local/partc/attempts.tsv` |

*(Noted 2026-11-11, at the owner's rulings of that day, which the end of this section records.)* Each row's verdict
read "**MET**", without the readings it rests on. Each now names those of the readings below that the part's verdict
rests on, as `manifest.json` lists them, and that the owner ruled on, and ends "as the owner ruled on 2026-11-11": part
A's "**MET**, on readings 1, 3, 6 and 11 below", part B's "**MET**, on readings 1, 3, 7, 8 and 12 below" and part C's
"**MET**, on reading 1 below". No verdict moves. Sections 6 to 8 are left as written.

No part needs an exclusion (section 9).

### Readings the rule does not settle

Each verdict rests on these readings. For each, the reading taken, why, and the alternative, for the owner to rule
on. No reading changes a verdict as far as the files show, except where the item says so.

1. **"The after commit is its only first-parent successor that changes" a named path.** *Taken:* the first-parent
   successors from the base up to the after commit, `git rev-list --first-parent 3b33530d..68cad8b`, which is the
   after commit alone. *Why:* the clause, with its next sentence ("If the cut-over had to land in more than one merge,
   each is SJL-9.4's ... and the base is the first one's first parent"), asks that the cut-over be one merge whose
   first parent is the base; a condition read over every later commit would change its answer with each merge after
   the cut-over. *Alternative:* every first-parent descendant of the base on the default branch today. Eight of the
   nine merges after `68cad8b` change such paths ([`commit-conditions.txt`](after/commits/commit-conditions.txt)), the
   SJL-13 and record merges among them, so the condition would fail and **every part would be void**. This change
   would count too: it changes JSD-0082, under `src/Broiler.VM.Profile.JavaScript/docs/`, and so would every later
   records change to that file.
2. **The rule commit.** *Taken:* `4cf9ca4`, the merge that carries the file into the default branch, as section 3
   names it. *Alternative:* `f0023e28`, the commit that adds the file, or `8edfcb79`, its last edit. All three are
   ancestors of the base and the file's blob is the same at each that holds it, so nothing turns on it.
3. **The revision of 2026-11-04 is in the after commit, not the base.** It was filed on SJL-9.4's branch and merged
   with the cut-over; its file says it was written before any run it judges, which the dispatch times bear out.
   *Taken:* the rule as revised judges every part. *Alternative:* the rule as the base holds it, unrevised; part B's
   guard `temporal/t1/data-without-the-tz-release-reads-its-zones` would then be a regression and part B NOT MET.
4. **Part A's rows in one report only.** The comparison joins on path and variant and ignores a row present in one
   report only, and says how many. A `canonical-tz` file is one row with no variant at the base, skipped, and two
   rows after. *Taken:* those rows read from the reports themselves and listed beside the comparison's rows, so that
   class C1's first kind, which names exactly them, is listed ([`part-a-tables.py.txt`](after/scripts/part-a-tables.py.txt));
   any other row in one report only would have been a regression, and there is none. *Alternative:* the comparison's
   rows alone; the C1 rows would go unlisted, and "every changed verdict ... is listed" could not be met.
5. **How many re-runs (C4 (c')).** *Taken:* three one-process runs at each commit per path, alternating the commits,
   as bundle `sjl-13-001` took them, the row admitted when all six agree. A process over a path runs both its
   variants, and each row is read in its own variant. *Alternative:* one re-run at each commit, as bundle `sjl-10-001`
   took them; then every row's two re-runs agree, the first pair of row 9 in section 6's table included, and no race
   is needed. Either way every (c') row is admitted.
6. **A Fuel-against-WallClock split under "no C2".** *Taken:* class (c') admits "a wall-clock exhaustion on either
   side", so a row exhausted on fuel on one side and on the wall clock on the other is (c'), decided by its re-runs and
   (e); "an exhaustion verdict that moves on fuel" is read as one with no wall-clock exhaustion on either side.
   *Alternative:* any row with fuel on one side is a regression under "no C2"; then the (c') rows on fuel at one side
   would be regressions and part A NOT MET.
7. **Part B: the area checks' outputs.** Two area checks print a different count after, with their verdicts ok:
   `intl/i4/locales-match-icu` and `tzdb/time-zone-offsets-match-icu` read more lines as the divergences name them.
   *Taken:* part B compares each check's verdict and holds the programs, retained answers and divergences files byte
   for byte; a count that grows by exactly the lines class L18 or Z8 adds less those it removes (section 7) is no
   difference outside those classes. JSD-0082's note of 2026-11-04 on decision D15 says the same of the tzdb check.
   It says that of the tzdb check alone: neither it nor any other part of JSD-0082, its note of 2026-11-11 on the
   settling condition included, says anything of `intl/i4/locales-match-icu`'s count moving under decision D32's L18
   lines. That is a gap in the record, listed here for the owner; JSD-0082 is not edited to cover it.
   *Alternative:* each check's output line held byte for byte; the two checks would be regressions and part B NOT MET.
8. **Part B: whose primary, and which language.** *Taken:* a Z8 key's primary is the name the base answers for it,
   since the base reports a zone's primary (decision D15); an L18 key's language is the options' `language` when they
   give one, else the tag's first subtag, since the options' `language` is the language of the locale built.
   *Alternative:* the primary read from the time-zone tables, and the language after alias replacement. No line's
   class depends on that alternative: no alias in the file replaces to `und`, `de` or `en`. *A second alternative,*
   the language read from the tag's first subtag alone, moves one line: the added
   `61 en-US {"language":"fr","script":"Cyrl","region":"BE","variants":"Fonipa-1996"} collations => []` of
   `src/tests/cldr/locales/divergences.txt`, whose tag's language is `en`, which the Collator supports, while its
   options give `fr` and the locale built is `fr-Cyrl-BE-1996-fonipa`, as the same file's line for that key without a
   method reads it (`=> fr-Cyrl-BE-1996-fonipa base=fr-Cyrl-BE-1996-fonipa language=fr ...`). Its class rests on
   reading the language of the locale built: under the taken reading it is L18, and read from the tag alone it would be
   outside L18 and part B NOT MET.
9. **The local machine.** The rule names it "by its processor count, its SDK and its operating system", the machine
   bundles `sjl-5-001` to `sjl-10-001` were taken on. This one has the same processor count, operating system and
   native backend, and .NET SDK 10.0.404 where those bundles had 10.0.401; it was installed again after the machine
   was replaced, as bundle `sjl-13-001` records. *Taken:* the same machine by the rule's description, the SDK recorded.
   *Alternative:* a different machine; the local parts and part A's re-runs would then not be the rule's.
10. **The wall allowances.** *Taken:* part C's "ample" wall allowance is 600,000 ms at both commits, as bundles
    `sjl-5-001` to `sjl-13-001` took it; part A's re-runs use the part's 5,000 ms and the race ten times it, 50,000 ms.
11. **A (c') row whose re-runs agree on a wall-clock exhaustion.** *(Added 2026-11-11, at SJL-9.6's review.)* Row 13
    of section 6's (c') table, `staging/sm/TypedArray/sort_large_countingsort.js` [strict] in the native pair, is
    exhausted on the wall clock in all six of its re-runs
    ([`rerun-wide-native.tsv`](after/parta/rerun/rerun-wide-native.tsv)).
    *Taken:* "a (c') row whose re-runs agree is admitted", whatever they agree on, so the row is admitted without (e),
    as bundle `sjl-10-001` admitted rows whose re-runs agree on the wall clock. *Why:* the clause names agreement
    alone, and sends to (e) only "a (c') row whose re-runs differ". *Alternative:* an agreement on a wall-clock
    exhaustion settles nothing, for the reason (e) refuses a wall-clock verdict, so the row is raced under (e) or counts
    as a regression. Under it, part A's verdict rests on this one native row and is open: no race of it was taken, so
    its result is not known.
12. **Part B: how much of a header "rewritten" covers in class Z8.** *(Added 2026-11-11, at SJL-9.6's review.)* The
    time-zone divergences' group header is six comment lines at the base and fifteen after: eight that give decision
    D15's reason, and a seven-line dated note quoting the old header
    ([`part-b-lines.tsv`](after/datasets/part-b-lines.tsv), the comment rows `base:4` to `base:9` and `after:4` to
    `after:18`). Class Z8 admits "the comment lines that head those lines, rewritten to give decision D15's reason";
    class L18 reads "rewritten or added". *Taken:* a header rewritten at greater length, with its dated quote of the old
    text, is "rewritten" under Z8; the dated quote is how this family rewrites any text it keeps a record of. *Why:*
    the lines are one header, rewritten in one change to give D15's reason, and the note is that rewriting's own record.
    *Alternative:* the comment lines beyond the base header's six, or the quoting note's lines, are outside Z8, since
    Z8, unlike L18, does not say "added"; they would be differences outside Z8 and L18 and part B NOT MET.
13. **Which figures section 2 quotes (rule H5).** *(Added 2026-11-11, at SJL-9.6's review.)* *Taken:* section 2
    reads the logs it quotes, the machine records and the allowances the runs' commands set, which sections 6 and 8 read
    too; and an archive's file count, which section 6 quotes from [`commands.log`](after/parta/commands.log), is a
    property of the pin, not a suite total, as bundle `vm-5-002` quotes the same count. *Alternative:* H5 read strictly,
    under which readings 9 and 10's figures move to sections 6 and 8 and the file count is dropped. No verdict turns on
    it.

**The owner's rulings of 2026-11-11** *(2026-11-11)*. On 2026-11-11, after Broiler-Platform/Broiler.VM#200 had opened
with the readings above, the owner ruled on readings 1, 3, 6, 7, 8, 11 and 12, each time taking the reading this
section took; the alternative named with each was not taken. Each of the seven is now the owner's own dated answer, and
this section's opening, "Each verdict rests on these readings. For each, the reading taken, why, and the alternative,
for the owner to rule on.", reads for them with this paragraph. The readings above are left as written.
- **Reading 1, the base's third condition: "Up to after commit".** The reading reads "*Taken:* the first-parent
  successors from the base up to the after commit, `git rev-list --first-parent 3b33530d..68cad8b`, which is the after
  commit alone." The owner ruled that the base's first-parent successors are read from the base up to the after commit,
  so the condition holds and no part is void. Not taken: "All later merges", under which every part would be void.
- **Reading 3, the revision of 2026-11-04 in the after commit and not the base: "Revised rule governs".** The reading
  reads "*Taken:* the rule as revised judges every part." The owner ruled that the revision of 2026-11-04 judges every
  part. Not taken: "Base's unrevised rule", under which part B would be NOT MET.
- **Readings 6 and 11, part A: "Accept both".** Reading 6 reads "*Taken:* class (c') admits "a wall-clock exhaustion
  on either side", so a row exhausted on fuel on one side and on the wall clock on the other is (c'), decided by its
  re-runs and (e)". Reading 11 reads "*Taken:* "a (c') row whose re-runs agree is admitted", whatever they agree on, so
  the row is admitted without (e)". The owner ruled that a split between a fuel exhaustion and a wall-clock exhaustion
  is (c'), and that a (c') row whose re-runs agree on a wall-clock exhaustion is admitted without (e). Not taken:
  "Strict readings". With the question, the owner was told that the stage's verifier had raced row 13 of section 6's
  (c') table once more at each commit, with the allowances of section 6's race, and that it gave a fuel exhaustion at
  both; this bundle retains no record of that race and reads nothing from it, so reading 11's "no race of it was taken"
  stays true of this bundle's runs and is left as written.
- **Readings 7, 8 and 12, part B: "Accept all three".** Reading 7 reads "*Taken:* part B compares each check's verdict
  and holds the programs, retained answers and divergences files byte for byte; a count that grows by exactly the lines
  class L18 or Z8 adds less those it removes (section 7) is no difference outside those classes." Reading 8 reads
  "*Taken:* a Z8 key's primary is the name the base answers for it, since the base reports a zone's primary (decision
  D15); an L18 key's language is the options' `language` when they give one, else the tag's first subtag, since the
  options' `language` is the language of the locale built." Reading 12 reads "*Taken:* a header rewritten at greater
  length, with its dated quote of the old text, is "rewritten" under Z8; the dated quote is how this family rewrites any
  text it keeps a record of." The owner ruled that all three stand as taken. Not taken: "Strict readings", under which
  part B would be NOT MET.

**Parts A, B and C's verdicts now rest on the owner's rulings for readings 1, 3, 6, 7, 8, 11 and 12, not on open
readings**: each part on readings 1 and 3, part A also on readings 6 and 11, and part B also on readings 7, 8 and 12.
No verdict moves; the State paragraph at the head of this README and the verdict table above say so by dated notes.
**Readings 2, 4, 5, 9, 10 and 13 stay as taken, unruled**: the owner was not asked about them, since the question
named only the readings a verdict was found to turn on, and each stands as SJL-9.6 took it, none the owner's own dated
answer. Two of their alternatives say more than that: reading 4's says that "every changed verdict ... is listed"
could not be met, and reading 9's that the local parts and part A's re-runs would not be the rule's. Neither was put to
the owner, and neither is taken here.

## 3. Engine-level vectors on the Intl area (SJL-9.2)

Broiler.JS's corpus revision 27 adds the Intl area, five files under `conformance/vectors/intl/`, one per sub-area of
JSD-0082, and pins ECMA-402 at decision D14's commit in `conformance/vectors/sources/ecma402.pin`. Revision 28
classifies both engines' first runs on revision 27. Both revisions are on Broiler.JS's default branch since
Broiler-Platform/Broiler.JS#1080 merged as `ff54e1d8`, whose corpus is that of `0c8feb64`, revision 28 as corrected
before the merge.
- **Where the expected values come from.** ICU's answers, through the reference Node.js release Broiler.JS's bundle
  `sjl-9-001` names (`ref:intlref@1`, decision D17), except where the pinned text rules: there the vector takes the
  text's answer and records ICU's. A date-time string carries U+202F where ICU's pattern has it (decision D17 (b)).
  A region's hour cycles, DurationFormat's `digital` data and this family's composition of a compound unit are read
  from CLDR 48.2.0 (`ref:intlcldr@1`, decisions D34 and D35).
- **This family's `divergence.broiler-vm` fields are its three corrections**, which SJL-9.4 makes (decisions D15 and
  D32): row Z8, the formatter resolving an identifier that is not its own primary to the primary, in
  `intl/date-times/zones/`; row L9, a subdivision whose replacement is a region written as the region, in
  `intl/locales/subdivision/`; and row L18, `getCollations` of a locale whose language the Collator lacks, in
  `intl/locales/collations/`. Broiler.JS's fields are its own, which its SJL-9.5 answers.

**Where the first runs are.** This family's first runs on revisions 27 and 28 were taken for Broiler.JS's
classification, at the rule commit `4cf9ca4`, from scratch archives and scratch pins, and Broiler.JS's bundle
`sjl-9-001` keeps them in its `engine/` as the basis of those fields. This section's run is on the committed pin, at
revision 28 as Broiler.JS's default branch carries it.

**The pin** is at revision 28 since `da4c07c`, and was at revision 26 from `d625402` (SJL-10.5). Its archive was taken
twice, byte-identical, and its files hashed from an extracted copy; the same method, applied to revision 26's commit,
`00b65dd0`, gives that pin's two digests and its count exactly. Against revision 26 the archive adds the Intl area's
five files and `sources/ecma402.pin`, and changes the corpus's README, revision log and schema; every other file is
revision 26's byte for byte.

**The run.** The conformance root was built in Release with `--disable-build-servers` in the clean tree of `da4c07c`
([`vectors/build-conformance.log`](vectors/build-conformance.log)), whose code is the rule commit's, `4cf9ca4`'s: the
two differ in documents, the pin's folder and the notices only. It was run there on the committed pin under
`TZ=UTC`, with no `LANG` and no `LANGUAGE`, by a script that wrote each log whole: a header naming the commit, the
build, the pin and its archive with their SHA-256, the host and the command; then the runner's output; then the exit
code and the end time. The command names the report by its file name: it was written outside the tree, which stayed
clean, and is kept here gzip-compressed. This family's Intl reads no host's ICU: the root is built with invariant
globalization and reads its own CLDR 48.2.0 tables. [`vectors/hashes.txt`](vectors/hashes.txt) holds every file's
SHA-256, which `sha256sum -c` verifies.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 28 | At `da4c07c`, clean, on the committed pin, from 05:57Z on 2026-10-27 | 106,808 vectors: 106,767 match, 41 classified divergences and 0 unclassified differences, and no divergence that no longer holds | [`run-revision-28.log`](vectors/run-revision-28.log), [`run-revision-28.tsv.gz`](vectors/run-revision-28.tsv.gz) |
| Its negative control | The same, with `--mutate intl/numbers-and-text/decimal/0001` | 106,766 match, the same 41 classified divergences, and 1 unclassified difference, the mutated vector | [`run-negative-control-revision-28.log`](vectors/run-negative-control-revision-28.log), [`run-negative-control-revision-28.tsv.gz`](vectors/run-negative-control-revision-28.tsv.gz) |

**This family matches every vector of revision 28 or differs exactly as its `divergence.broiler-vm` field says.** The
41 classified divergences are the 41 vectors that carry such a field: 18 of row Z8 and 18 of row L9, `normative` at
revision 27 and `divergence` since revision 28, and 5 of row L18, `impl-defined` at both. Each is reported as a
classified divergence, so none is unclassified and none no longer holds. The 23 of rows L9 and L18 carry Broiler.JS's
field as well. Every other vector of class `divergence` in the report carries Broiler.JS's field alone, the number
area's four among them, and matches here. So the vectors find no difference of this family's that the correspondence
does not classify, which JSD-0082 names as falsifying it.

**The run agrees row for row with the one Broiler.JS's bundle retains** of this family on revision 28 at `4cf9ca4`,
on the corpus at `2f240fec` (its `engine/broiler-vm-run-revision-28.tsv.gz`).
[`vectors/comparison-revision-28.txt`](vectors/comparison-revision-28.txt) holds the comparison.
- Every row has the same id, expected value, rendering and verdict in both, the Intl area's 3,496 and every other.
- The rows of the other areas are the same byte for byte.
- 11 rows of the Intl area differ, in their class alone: `normative` there, `divergence` here, and a match in both.
  They are the 11 vectors to which `0c8feb64` gave a `divergence.broiler-js` field after that run, for the
  differences CI's hosts showed in Broiler.JS, which this family's root, reading no host's ICU, does not show.
- `497158a3`, the other correction since that run, renames the op of two vectors of row L23; an engine-level run
  evaluates a vector's script, not its op, and the report does not name the op.

This section is no part of the rule's parts A to C, and nothing in it is a verdict.

## 4. The closures at SJL-9.4's first stage (`cutover/`)

*(Added 2026-11-03, at SJL-9.4.)* [`cutover/`](cutover/README.md) holds the closures of every JavaScript root in three
modes, collected at the branch commit that cuts sub-area (a) over, which the composition register's rows name as their
evidence since the profile references `Broiler.ECMAScript.Intl` (rules K3, K4 and I7). It is no run of this bundle's
rule: section 2's runs are still to be taken at the base and after commits. *(Noted 2026-11-04, at SJL-9.4's records: the closures were
taken again at the branch's last change to a product source, from a clean checkout of `b3fd2ba`, and `cutover/`'s
README says so; this paragraph, which names the first collection, is kept as written.)* *(Noted 2026-11-04, later:
one change to an architecture test, `65ad719`, follows `b3fd2ba`; `cutover/`'s README records a retake after it that
reproduces every closure, and why the collection from `b3fd2ba` stays.)*

## 5. Engine-level vectors on the pin at corpus revision 38 (SJL-9.6)

*(Added 2026-11-11, at SJL-9.6.)* Broiler.JS's corpus revision 38 retires both
engines' Intl divergence fields that the SJL-9 cut-overs answer, this family's among them: the fields of rows Z8, L9
and L18, which SJL-9.4 corrected (Broiler-Platform/Broiler.VM#190, merged as `68cad8b`). It also makes row Z8's
per-engine expectations one, as JSD-0082's decision D15 says it would: the time-zone area's vectors of the row are
`normative`, and their `out` is the identifier as given, case-normalised. It is on Broiler.JS's default branch since
Broiler-Platform/Broiler.JS#1112 merged as `3493462a`, whose corpus is that of `5a15483f`, revision 38 as completed
before the merge. The retired fields are listed, with their engine and reason, in Broiler.JS's
`docs/evidence/corpus-revision-38/retired-divergences-revision-38.tsv`. Between revision 32, where the pin stood, and
revision 38:
- revision 33 adds the collation vectors, which bundle [`sjl-17-001`](../sjl-17-001/README.md) reads, and revision 34
  classifies both engines' first runs on them, with no difference of this family's;
- revisions 35, 36 and 37 classify Intl vectors for Broiler.JS alone: those whose answer depends on whether the host
  lists one of decision D5's extra locales (35 and 37, decision D7), and those the pinned CLDR 48.2.0 contradicts (36);
- revision 38 is the retirement above.

**The pin** is at revision 38 since `61394d0`, and was at revision 32 from `a4ce976` (SJL-11's task 3.5). Its archive
was taken twice, byte-identical, and its files hashed from an extracted copy; the same method, applied to revision 32's
commit, `b2c2e224`, gives that pin's two digests and its count exactly. Its content digest is that of the scratch pin
of `5a15483f` from which revision 38's row in Broiler.JS's revision log records this family's run on the revision.
Against revision 32 the archive adds the collation vectors' four files and changes ten: the corpus's README, revision
log and schema, the Intl area's five files and the time-zone area's `identifiers.vectors` and `lookup.vectors`; every
other file is revision 32's byte for byte. No vector's `out`, script, input or operation changes. This family's
expectations move only where row Z8's `per-engine.broiler-vm` fields go.

**The runs.** Each was taken by a script that wrote its log whole: a header naming the commit, the tree, the build, the
pin and its archive with their SHA-256, the host and the command; then the runner's output; then the exit code and the
end time. Each was run under `TZ=UTC`, with no `LANG`, `LANGUAGE` or `LC_ALL`, with the report written outside the tree
and kept here gzip-compressed. This family's Intl reads no host's ICU. [`vectors/hashes.txt`](vectors/hashes.txt) holds
every file's SHA-256, which `sha256sum -c` verifies.
- **The base** is `0ffc037f`, this repository's default branch (the merge of Broiler-Platform/Broiler.VM#198), on the
  pin it commits, revision 32. Its answers were taken first, before the pin moved, in a clean checkout of that commit,
  by a script whose log had no header. The run kept here was taken again afterwards, so that its log has one, in a tree
  that `git archive` wrote of that commit, `docs/evidence/` left out, where the conformance root was built in Release
  with `--disable-build-servers` ([`vectors/build-conformance-base-revision-32.log`](vectors/build-conformance-base-revision-32.log));
  its report is the first one's byte for byte.
- **The run on revision 38** was taken at `61394d0`, clean, on the committed pin, with the root built in Release with
  `--disable-build-servers` in that tree ([`vectors/build-conformance-revision-38.log`](vectors/build-conformance-revision-38.log)).
  `61394d0`'s code is `0ffc037f`'s: the two differ in the pin's folder and the notices alone.
  *(Added 2026-11-11, at review.)* The SHA-256 its log records for `vectors.pin` is that of the pin `61394d0` commits.
  The later commits of this change edit the pin's header comments alone, so the file at their head hashes otherwise; its
  data lines, the `revision`, `archive-sha256`, `content-sha256`, `files` and `archived-at` lines that the runner and
  `SharedVectorsPinTests` read, are unchanged, and a run on the pin as they leave it, with the root built in Release at
  their head, writes this run's report byte for byte.

| Run | Taken | Result | Files |
|---|---|---|---|
| Revision 32, the base | At `0ffc037f`, from its archive, on its pin, from 07:50Z on 2026-11-11 | 108,759 vectors: 108,541 match, 65 of them where a divergence no longer holds, 61 classified divergences and 157 unclassified differences; exit code 1 | [`run-base-revision-32.log`](vectors/run-base-revision-32.log), [`run-base-revision-32.tsv.gz`](vectors/run-base-revision-32.tsv.gz) |
| Revision 38 | At `61394d0`, clean, on the committed pin, from 07:34Z on 2026-11-11 | 111,157 vectors: 111,096 match, 24 of them where a divergence no longer holds, 61 classified divergences and 0 unclassified differences | [`run-revision-38.log`](vectors/run-revision-38.log), [`run-revision-38.tsv.gz`](vectors/run-revision-38.tsv.gz) |
| Its negative control | The same, with `--mutate time-zones/formatter-zone/0001` | 111,095 match, the same 61 classified divergences, and 1 unclassified difference, the mutated vector | [`run-negative-control-revision-38.log`](vectors/run-negative-control-revision-38.log), [`run-negative-control-revision-38.tsv.gz`](vectors/run-negative-control-revision-38.tsv.gz) |

The 18 vectors whose `out` is a status are skipped in each run and named in its summary line.

**This family matches every vector of revision 38 or differs exactly as its `divergence.broiler-vm` field says, and
leaves none unclassified.** [`vectors/comparison-revision-38.txt`](vectors/comparison-revision-38.txt), written by
[`vectors/compare-revision-38.py`](vectors/compare-revision-38.py), compares the two runs row by row and reads the two
corpora record by record. Every verdict that moves is attributed there, and none is left unattributed:
- **the 41 vectors of the retired fields**, 18 of row Z8 in `intl/date-times/zones/`, 18 of row L9 in
  `intl/locales/subdivision/` and 5 of row L18 in `intl/locales/collations/`, move from a match where the divergence no
  longer holds to a match. Each matched at the base, as since SJL-9.4, and revision 38 removes the field, so none is
  reported as no longer holding;
- **the 157 time-zone vectors of row Z8**, 152 in `time-zones/formatter-zone/` and 5 in `time-zones/case/`, move from an
  unclassified difference to a match. The rendering is the same at both pins, the identifier as given; at revision 32
  the expectation was the vector's `per-engine.broiler-vm` field, the primary, and at revision 38 it is `out`. These
  are the differences the runs on revision 32 have shown since SJL-9.4 (JSD-0085, section 9).

No other verdict moves, and no vector's expected value or rendering changes but those 157 expected values. The 2,398
collation vectors, which only revision 38's run holds, all match, and each has the expected value, rendering and verdict
of bundle `sjl-17-001`'s run on revision 34 (its `vectors/run-revision-34.tsv.gz`). *(Added 2026-11-11, at review.)* Of
those, 35 are `normative` in that run and `divergence` here: they change class alone, through the
`divergence.broiler-js` fields revision 34 adds from CI's `windows-latest` host after the scratch pin that run read
(`60fc3a5b`), which this family's run does not read. Other vectors change only in their class, each matching at both
pins: those whose last divergence field, Broiler.JS's, revision 38 retires become `normative`; and 16 become
`divergence` through a `divergence.broiler-js` field Broiler.JS's revisions 36 and 37 add, which this family's run does
not read. The 61 classified divergences, of the regular-expression area, and the 24 matches where a divergence no longer
holds, 2 of the regular-expression area and 22 of the syntax area, are the same vectors at both pins: SJL-11's and
SJL-13's fields, which no revision since 32 touches. The figures of the run on revision 38 are those revision 38's row
gives for this family's run at `a8d8398` over a scratch archive of the revision: 111,072 matching, 24 matching where a
divergence no longer holds and 61 classified divergences. Its negative control reports the mutated vector alone.

So the vectors find no difference of this family's that the correspondence does not classify, which JSD-0082 names as
falsifying it. This section is no part of the rule's parts A to C, and nothing in it is a verdict.

## 6. Test262, both forms (rule part A)

*(Added 2026-11-11, at SJL-9.6.)*

**MET.** "MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1
or C4 and is listed, and the native pair meets the same conditions."
- **All four runs are complete** ([`after/parta/completeness.txt`](after/parta/completeness.txt), read off the
  imported artifacts by [`part-a-tables.py.txt`](after/scripts/part-a-tables.py.txt)). In each form at each commit:
  sixteen shard logs of sixteen, and the merge took sixteen shard reports; the run log's exit code is 1 in the bytecode
  form and 4 in the native form, both of which completeness admits; the merged report's retention object says
  retainable, its coverage is whole and it accounts for every variant; the binaries log has one distinct list of
  assemblies. Every runner and the merging job ran image `ubuntu24` 20261101.360.1 with SDK 10.0.404 at both runs.
- **The comparisons** ([`compare-wide-bytecode.log`](after/parta/compare-wide-bytecode.log),
  [`compare-wide-native.log`](after/parta/compare-wide-native.log)) are `eng/compare-test262-forms.py --same-form` as the
  base commit holds it (the same blob after), the base's report as control and the after's as candidate, the native
  pair with `--exempt-guest-loads` over the pin's archive extracted whole: its SHA-256 is the archive's, it holds the
  pin's 56,560 files and `tar --compare` finds no difference ([`commands.log`](after/parta/commands.log)). Each exits 0:
  every row each lists is in class (c'). Each also ignores 57 rows present in one report only, which section 2's
  reading 4 reads.
- [`changed-verdicts.tsv`](after/parta/changed-verdicts.tsv) lists every changed row of both pairs with its class, and
  [`c1-table.tsv`](after/parta/c1-table.tsv) every C1 row with its path, variant, row and clause.

**C1, row Z8, its first kind: 38 variants in each pair**, both variants of each of the 19 files below. Each claims
`canonical-tz`, is one row at the base, skipped with "the test claims the proposed feature `canonical-tz`", and passes
in both variants after, in both forms. They are the only rows in one report only: the base report's 19 are these
files' skipped rows, and the after report's 38 their variants. No `canonical-tz` file is skipped after, and none fails,
is exhausted or is unsupported. Clause: "a variant of a file whose features include canonical-tz, skipped at the base
because the proposal was not admitted and passing after" (class C1, kind Z8). Paths are under `test/intl402/`.

| File | Variants | Row | Base, after (both pairs) |
|---|---|---|---|
| `DateTimeFormat/canonicalize-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `DateTimeFormat/canonicalize-utc-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `DateTimeFormat/timezone-case-insensitive.js` | strict, sloppy | Z8 | Skipped, Passed |
| `DateTimeFormat/timezone-not-canonicalized.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/etc-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/from/argument-valid.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/from/do-not-canonicalize-iana-identifiers.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/from/etc-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/from/timezone-case-insensitive.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/iana-legacy-names.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/links-backward.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/links-backzone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/links-etcetera.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/links.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/non-canonical-utc.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/prototype/equals/canonicalize-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/prototype/equals/canonicalize-utc-timezone.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/timezone-case-insensitive.js` | strict, sloppy | Z8 | Skipped, Passed |
| `Temporal/ZonedDateTime/timezone-ids-basic.js` | strict, sloppy | Z8 | Skipped, Passed |

**C1's other kinds list nothing.** No variant fails at the base and passes after, in either pair, so row Z8's second
kind ("a variant failing at the base that passes after because the formatter reports the identifier as given") has
no row, and neither have rows L18 and L9, which the rule expects to reach no scored variant. **No C1 row is of a kind
the rule does not name, so none is a finding against JSD-0082's correspondence.**

**What the rule's "How it is read" names** ([`rows-named.txt`](after/parta/rows-named.txt), from
[`part-a-rows.py.txt`](after/scripts/part-a-rows.py.txt)). Rows S7's and T8's four files pass in both variants at both
commits, in both forms, so they keep their base verdicts. Every `getCollations` file claims `Intl.Locale-info` and is
skipped at both commits. The `Intl.NumberFormat-v3` and `Intl.Locale-info` skips are the same rows at both commits.

**C4 (c'): two rows in the bytecode pair and eleven in the native pair**, every one exhausted at both commits, on the
wall clock at one side. Their re-runs ([`rerun/`](after/parta/rerun/), by
[`part-a-rerun.sh.txt`](after/scripts/part-a-rerun.sh.txt)) are each one process of the conformance root over the
row's path, built in Release from a clean archive of its commit, under the wide manifest, the row's form, fuel
100,000,000 and a wall clock of 5,000 ms, with `--expect` against the commit's retained pin, three at each commit,
alternating the commits (section 2's reading 5). Every run's log reads the pin's content digest, and each exits 1, as
the conformance root does when no variant reaches a verdict about the engine. Paths are under `test/`.

| # | Path | Variant | Pair | Base, after (hosted) | Re-runs at `3b33530d` | Re-runs at `68cad8b` | Class |
|---|---|---|---|---|---|---|---|
| 1 | `staging/sm/Date/dst-offset-caching-7-of-8.js` | sloppy | bytecode | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 2 | `staging/sm/Date/dst-offset-caching-7-of-8.js` | strict | bytecode | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 3 | `annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js` | sloppy | native | Exhausted/WallClock, Exhausted/NestedLoadFanOut | NestedLoadFanOut, three times | NestedLoadFanOut, three times | **C4 (c')**, the re-runs agree |
| 4 | `annexB/built-ins/RegExp/RegExp-leading-escape-BMP.js` | strict | native | Exhausted/WallClock, Exhausted/NestedLoadFanOut | NestedLoadFanOut, three times | NestedLoadFanOut, three times | **C4 (c')**, the re-runs agree |
| 5 | `staging/sm/Date/dst-offset-caching-1-of-8.js` | sloppy | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 6 | `staging/sm/Date/dst-offset-caching-1-of-8.js` | strict | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 7 | `staging/sm/Date/dst-offset-caching-2-of-8.js` | strict | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 8 | `staging/sm/Date/dst-offset-caching-3-of-8.js` | sloppy | native | Exhausted/Fuel, Exhausted/WallClock | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 9 | `staging/sm/Date/dst-offset-caching-3-of-8.js` | strict | native | Exhausted/Fuel, Exhausted/WallClock | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 10 | `staging/sm/Date/dst-offset-caching-4-of-8.js` | sloppy | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 11 | `staging/sm/Date/dst-offset-caching-4-of-8.js` | strict | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, WallClock, Fuel | Fuel, Fuel, WallClock | **C4 (e)**, below: the re-runs differ |
| 12 | `staging/sm/TypedArray/sort_large_countingsort.js` | sloppy | native | Exhausted/WallClock, Exhausted/Fuel | Fuel, Fuel, Fuel | Fuel, Fuel, Fuel | **C4 (c')**, the re-runs agree |
| 13 | `staging/sm/TypedArray/sort_large_countingsort.js` | strict | native | Exhausted/WallClock, Exhausted/Fuel | WallClock, WallClock, WallClock | WallClock, WallClock, WallClock | **C4 (c')**, the re-runs agree |

Every re-run is an exhaustion; the table gives its dimension. **Twelve rows' re-runs agree**, so C4 (c') admits them.
Row 13's agree on the wall clock: the rule admits a (c') row whose re-runs agree, whatever they agree on, as bundle
`sjl-10-001` read it, so it is not raced (section 2's reading 11, which gives the alternative).

**C4 (e)** ([`race/`](after/parta/race/)). Row 11, `staging/sm/Date/dst-offset-caching-4-of-8.js` [strict] in the
native pair, ran alone once more at each commit with the part's fuel allowance and ten times its wall clock, 50,000
ms, nothing else on the machine, from 10:30:37Z to 10:30:59Z. **It gives Exhausted/Fuel at both commits**: the same
verdict, neither a wall-clock exhaustion nor an internal defect, so (e) admits it. The process ran the path, so its
sloppy variant, row 10, ran too and gave the same; (e) does not read it.

**No other class has a row.** No row is in (a) or (b); no row is unclassified; no variant moved between a pass, a
failure, a refusal and an exhaustion but the C1 rows; and every changed row outside C1 has a wall-clock exhaustion on
one side, so none moved on fuel alone (no C2) and none because a choice, a policy or the data moved (no C3, no C5).
Every changed row outside C1 is one of the long-running tests this family's earlier bundles saw race between the wall
clock and another allowance. The floor verdicts in the imported artifacts decide nothing.

## 7. The reference datasets and the Intl checks (rule part B)

*(Added 2026-11-11, at SJL-9.6.)*

**MET.** "MET if and only if every check named passes at both commits, and every difference between the two commits
under src/tests/cldr/ and src/tests/temporal/ is in Z8 or L18 and is listed."
- **Every named check passes at both commits.** The slice compiler, built in Release at each commit, ran `--checks
  --verbose` once ([`base/datasets/checks.log`](base/datasets/checks.log), [`after/datasets/checks.log`](after/datasets/checks.log),
  each with the digest of its binary), and exited 0 at both. [`named-checks.txt`](after/datasets/named-checks.txt) at
  each commit holds the 34 lines of the checks the rule names, every check of `IntlChecks.cs` under `intl/i0/` to
  `intl/i4/`, `tzdb/` and `temporal/` as the base commit spells them
  ([`named-check-names.txt`](base/datasets/named-check-names.txt); its two templates, `{kind}` and `{granularity}`,
  stand for the two root-collation checks and the three break-test checks), the area's and the guards': every one is
  `ok` at both, by [`datasets.py.txt`](after/scripts/datasets.py.txt). `IntlChecks.cs` names the same checks at both
  commits.
- **Every difference under `src/tests/cldr/` and `src/tests/temporal/` is in Z8 or L18, and is listed**
  ([`after/datasets/part-b-lines.tsv`](after/datasets/part-b-lines.tsv), each line with its class, its line at both
  commits and the retained answer's line, and [`part-b-summary.txt`](after/datasets/part-b-summary.txt), by
  [`part-b-classify.py.txt`](after/scripts/part-b-classify.py.txt), git reads alone). Two files differ, the two
  divergences files below; no program, no retained answer, no README and nothing under `src/tests/temporal/` changes
  ([`compare.txt`](after/datasets/compare.txt), and `files.sha256` at both commits, 181 files each, two digests
  different).

| File | Class | Lines | How |
|---|---|---|---|
| `src/tests/cldr/timezones/divergences.txt` | **Z8** | 152 data lines | 7 rewritten, 19 removed where the retained answer already resolves the identifier to itself, 126 added where it resolves it to another name |
| | header of Z8 | 21 comment lines | the group's header rewritten to give decision D15's reason, with a dated note quoting the old one (section 2's reading 12) |
| `src/tests/cldr/locales/divergences.txt` | **L18** | 38 data lines | 12 rewritten from `[emoji,eor]`, 26 added where the retained answer reads `[emoji,eor]`; none removed |
| | header of L18 | 10 comment lines | the group's header rewritten to give decision D32's reading of CollationsOfLocale, with a dated note quoting the old one |

Every Z8 line's key is a time-zone identifier whose answer at the base names another identifier, its primary
(section 2's reading 8), and whose answer after names the identifier itself, the rest of the line, its offsets,
unchanged: `Asia/Calcutta`, which the base resolves to `Asia/Kolkata`, is removed, since the retained line
already reads `Asia/Calcutta => Asia/Calcutta`; `Pacific/Yap => Pacific/Chuuk` is rewritten to
`Pacific/Yap => Pacific/Yap`; `CET => CET` is added, where the retained answer reads `CET => Europe/Brussels`. Every
L18 line's key asks `collations` of a locale with no `-co-` keyword and no `collation` option, whose language is
neither `und` nor `de` nor `en`, and its answer moves from `[emoji,eor]` to `[]` alone. No line is outside the two
classes.

**The one guard in Z8, as the revision of 2026-11-04 admits it.** `temporal/t1/data-without-the-tz-release-reads-its-zones`
passes at both commits; its output reads "the family's data answered `-04:00 3 Asia/Kolkata`, and the same data
without the release `-04:00 3 Asia/Kolkata`" at the base and "`-04:00 3 Asia/Calcutta`" in both places after, and the
literal it is compared with in `IntlChecks.cs` moves from `own == "-04:00 3 Asia/Kolkata"` to
`own == "-04:00 3 Asia/Calcutta"`, with a comment, and nothing else in the file moves (the whole diff is in
[`compare.txt`](after/datasets/compare.txt)). Every other guard prints the base's line word for word.

**Two area checks print another count, with their verdicts ok** (section 2's reading 7):
`intl/i4/locales-match-icu` reads 158 lines as the divergences name them where the base read 132, the 26 L18 lines
added; `tzdb/time-zone-offsets-match-icu` reads 158 where the base read 51, the 126 Z8 lines added less the 19
removed. Every other named check prints the base's line word for word.

## 8. The fuel-parity twins (rule part C)

*(Added 2026-11-11, at SJL-9.6.)*

**MET.** "MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling
tried, and completes with the base's answer where it completes."
- **The base** ([`base/fuel-parity.log`](base/fuel-parity.log)): each of the ten `*.small.js` twins of
  `src/tests/forms/wide` had its smallest completing fuel allowance bisected under the bytecode form, with the
  command-line root built in Release at the base. Each completes at that figure and is refused with
  `AllowanceExhausted on Fuel` at one less, in both forms, with the same answer in both.
- **After** ([`after/fuel-parity.log`](after/fuel-parity.log)): each twin, in the bytecode form and in the native form
  under `x86-64-sysv`, completes at its base figure with the base's answer and is refused on fuel at one less, as at
  the base, 20 rows of 20.
- The wall allowance is 600,000 ms at both commits, and no row ran out of it. Every attempt's command, exit code and
  last line is in each side's `local/partc/attempts.tsv`, by [`twins.py.txt`](after/scripts/twins.py.txt), which restates
  bundles `sjl-5-001`'s and `sjl-4-001`'s twin steps.
- Every figure and answer is bundle `sjl-13-001`'s base's and bundle `sjl-10-001`'s, so the cut-over moved no charge
  outside the area, as decision D18 requires.

| Twin | Figure | Answer |
|---|---|---|
| `wide/array-loop` | 73,082 | 999000 |
| `wide/call-loop` | 28,026 | 1000 |
| `wide/closure-loop` | 31,044 | 1000 |
| `wide/fib` | 7,682 | 144 |
| `wide/generator-loop` | 40,075 | 499500 |
| `wide/numeric-loop` | 23,022 | 498497 |
| `wide/property-loop` | 29,029 | 183125 |
| `wide/string-loop` | 26,083 | 25 |
| `wide/throw-loop` | 34,026 | 1000 |
| `wide/try-loop` | 34,022 | 499500 |

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-12-001`. *(Added 2026-11-11, at SJL-9.6's close-out:
EX-134, for the code SJL-9.4 moved out of this component's assurance coverage, joins the table, and the row of SJL-9.1
is kept as written.)*

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-9.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
| EX-134 | Open | **The Intl code that left this component's assurance coverage** *(added 2026-11-11, at SJL-9.6's close-out, as JSD-0085 section 11 names it)*. The types and members rule I5 registers for SJL-9.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`), with the bodies JSD-0085 section 8 names, left the JavaScript family's sources at SJL-9.4's cut-over, the after commit `68cad8b`: `JsLocaleTag.cs`, `JsDecimal.cs`, `JsPluralRule.cs`, `JsNumberData.cs`, `JsNumberFormatter.cs`, `JsDateData.cs`, `JsDatePatternGenerator.cs`, `JsDateIntervalFormat.cs`, `JsDateTimeFormatter.cs`, `JsZoneNames.cs` and `JsLocaleInfo.cs` whole, with every type in them; every `JsIntlTables` decoder and its lazy; the cores of `JsRealm.Intl`, `JsRealm.Locale`, `JsRealm.NumberFormat`, `JsRealm.PluralRules`, `JsRealm.RelativeTimeFormat`, `JsRealm.ListFormat`, `JsRealm.DisplayNames`, `JsRealm.DateTimeFormat` (with `JsTemporalDateFormats`) and `JsRealm.DurationFormat`; the fields of the Intl objects the library's records replace; and the two renamed members, `JsRealm.DateTimeComponents` and `JsRealm.DateTimeValue`. What computes them now is `Broiler.ECMAScript.Intl`, namespace `Broiler.ECMAScript.Intl`. The home's own assurance covers it in the Broiler.JS repository: `Broiler.ECMAScript/hairs.config.json` lists the library's project, and its generated manifest, `Broiler.ECMAScript/hairs.manifest.json`, lists every unit of the library's assembly with an annotation on each relevant declaration, checked in CI (rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters of JSD-0085 section 3 and its seams of section 2 (`JsIntlDataSet.cs` and `JsIntlTables`' `LocaleData`, `FormatData` and `DateTimeData`), the Intl objects with their option reads, coercion, errors, charges and locale policies, the data this family hands the library through `IJsIntlData` and `JsCldrTables.g.cs`, the reference datasets under `src/tests/cldr/`, and `NewCollator` and `CollatorKeyData` (collation, SJL-17's) and `JsRealm.Segmenter.cs` (SJL-7's) stay in this component and stay covered. EX-125 and EX-127 to EX-133 say the same of the earlier areas |

*(Noted 2026-11-11, at SJL-9.6's run of the rule.)* No part of the rule needs an exclusion, and none is defined by it:
parts A, B and C are MET without one (sections 6 to 8). The ledger's entry of 2026-11-04 also lists for SJL-9.6 "the
exclusion for the code that left this component's assurance coverage", as EX-125 and EX-127 to EX-133 say for the
earlier areas. It is no part of the rule, and is not defined here; it takes the next free identifier when it is.
The table above is left as written.

*(Noted 2026-11-11, at SJL-9.6's close-out.)* That exclusion is now defined, as EX-134 in the table above, the next
free identifier: EX-133 is bundle `sjl-13-001`'s, and no bundle defines a later one. It is still no part of the rule:
parts A, B and C read nothing of this component's assurance coverage, and no verdict of sections 6 to 8 moves with it.
The note above is left as written.
