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
has been taken.** *(2026-11-02, at SJL-17.2: the engine-level vector runs of section 3 are retained, and the
correspondence carries dated notes on rows O3 and O5b; neither is a run of the rule.)* *(2026-11-02, later, before
the merge of Broiler.JS's change: and on row O6, by the owner's decision E-o6w, section 3's last note.)*
*(2026-11-02, later, at SJL-17.3: row O3's reading (F1), which section 3 says "stays open for SJL-17.3", is settled by
the owner's decision E-o3h, which JSD-0084 records. It is not a run of the rule.)*
- Every run of the rule is taken at the base commit or the after commit, which the rule defines before either exists
  (JSD-0084's decision E14): the after commit is the default-branch merge that lands SJL-17.4's cut-over, and the base
  commit is its first parent.
- A collector joins this bundle when there is a run of the rule to take.

*(Added 2026-11-15, at SJL-17.6, this family's run of the rule.)* **The rule is run and judged.** Section 1's readings
were filed before any run was read; part A's hosted runs at the base and the after commit are imported, parts B and C
and part A's re-runs are taken locally, and section 5 judges each part: **parts A, B and C are each MET**, on the
readings section 5 names, none the owner's own answer. The closures at the after commit, with every root published and
run in three modes and rules I1 to I7, I3 on the image records and N28 run there, are retained in
[`after-closures/`](after-closures/README.md) and read in section 10, not judged; section 9 defines EX-136. **Nothing
is accepted**: review is deferred, these verdicts are not SJL-17's exit reading, which follows Broiler.JS's SJL-17.5,
and they meet no clause by themselves. The "State" line above reads "rule and correspondence only" and "**Nothing here
is a verdict, and no run of the rule has been taken.**", and the bullet above "A collector joins this bundle when there
is a run of the rule to take."; each reads as of the day it was written, and is left as written.
*(Noted 2026-11-16, at SJL-17's exit reading.)* The paragraph above says that these verdicts "are not SJL-17's exit
reading, which follows Broiler.JS's SJL-17.5, and they meet no clause by themselves". Broiler.JS's SJL-17.5 and
SJL-17.6 have merged (Broiler-Platform/Broiler.JS#1118 and #1119, observed), and section 11 is this family's half of
that reading: it reads the milestone's clauses 6 to 9 met on this bundle, and the programme ledger's entry of
2026-11-16 holds the record over both repositories. That meets no gate, and nothing is accepted. The paragraph above is
left as written.

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
  forty digits, when the change that carries it has merged. *(Note of 2026-11-15: that change, the SJL-17.1 records,
  added the file in `c71b4a889c208366844671a3d7f144caed0ae780` and merged in Broiler-Platform/Broiler.VM#179 as
  `4e6a41e9d3860b8ca79d5d3ea6de215c1592b0c0`, the rule commit, the default-branch merge, as bundles `sjl-12-001` and
  `sjl-13-001` name theirs. The file's blob, `7217a3f2ba7e96aa6a78db1346bc5091bf304073`, is the same at both, at the base
  and the after commit named below, and at the default branch's `1e8f648b`, and the evidence workflow and its driver
  are the same at both. This section did not name it then, as it should have.)*
- **The base commit** and **the after commit** do not exist yet. The rule states the conditions each must meet, and
  this README names each, by all forty digits, when it lands. The base commit descends from the rule commit and from
  the commit bundle `sjl-9-001` names as its after commit here, which does not exist yet either. *(Note of 2026-11-15:
  both exist since 2026-11-04, and this section did not name them when they landed, as it should have. The after
  commit is `983d690adac63f6d117ff2cd2d9cca61fc16e3d2`, the default-branch merge of Broiler-Platform/Broiler.VM#191
  (observed), which lands SJL-17.4's cut-over in one merge (section 4). The base commit is its first parent,
  `68cad8baf7403125d44d5007441d30ff07bc4edf`, the merge of Broiler-Platform/Broiler.VM#190, which is itself the commit
  bundle `sjl-9-001` names as its after commit. The rule's three conditions on the base are read here and not judged,
  since no run of the rule is taken: `git merge-base --is-ancestor 4e6a41e9 68cad8b` exits 0; the base is the commit
  bundle `sjl-9-001` names, not a later one, and whether "descends from" takes the commit itself is a reading SJL-17.6's
  evidence states; and `git rev-list --first-parent 68cad8b..983d690a` names the after commit alone.)*

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

### Readings taken before any run is read (2026-11-15, at SJL-17.6)

*(Added 2026-11-15, at SJL-17.6, on the branch `sjl-17-6-evidence`, cut from the default branch's
`4f914a3f7b9c65bd6066bc9e75f723b78d410a68`.)* These readings are filed before this stage reads any output of a run the
rule judges. Part A's runs of 2026-11-04 (reading 5) existed when they were filed; of them this stage had read only
each artifact's `run.txt`, which names the run, the dispatched ref, the tested commit and the inputs, and none of
their reports, logs or verdicts. No record in this repository names those runs, so whether anyone read them before
is not recorded. No local run of the rule had been taken. Each reading is recorded as the owner's dated decision under
the owner's standing direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review"), taken
as the recommended choice, with the alternative not taken and what it would do to the verdict; none is the owner's own
dated answer, asked and answered, and the owner may rule otherwise. They change no part, class or condition of the
rule, and the rule file is not edited.

1. **"The local machine bundle `sjl-9-001`'s rule names"** (parts A's re-runs, B and C). That rule names it as the
   machine bundles `sjl-5-001` to `sjl-10-001` were taken on, "by its processor count, its SDK and its operating
   system". Bundle `sjl-9-001` took its own local parts on this family's machine as replaced, four logical processors,
   Ubuntu 24.04.5 LTS and .NET SDK 10.0.404 (its reading 9), as bundle `sjl-13-001` did under its reading 10, which the
   owner ruled on 2026-11-12 ("Accept all five"). *Taken:* this cloud machine, with four logical processors, Ubuntu
   24.04.5 LTS, .NET SDK 10.0.404 and the native backend `x86-64-sysv`, is the rule's local machine, each run
   recording its SDK and its machine. *Not taken:* another machine, the one the earlier bundles named before the
   machine was replaced; no part could then be taken here, parts B and C and part A's re-runs would not be the
   rule's, and parts B and C would be NOT MET until a run on that machine, as would part A if it had any (c′) row.
2. **The hosted runs' SDK and images** (part A). The workflow asks `actions/setup-dotnet` for `10.0.x`, and the
   repository pins no SDK, so the runner image's SDK may be newer: bundle `sjl-13-002`'s hosted runs of 2026-11-14
   built with 11.0.100 (its section 7). The rule's configuration names the workflow and the driver at the dispatched
   ref, byte-identical to the base commit's, and "the conformance composition root that commit builds in Release on
   every runner"; it names no SDK and no image. *Taken:* a run is as configured whatever SDK and image the workflow
   resolves, so long as both runs of a pair record theirs; `imported.txt` records them, as bundle `sjl-13-002`'s reading
   3 took it. *Not taken:* hold the hosted runs to the local machine's SDK; the workflow, frozen by decision D37 of
   JSD-0083 and by the rule's own configuration, cannot be changed to do so, so no run could meet the configuration
   and part A would be NOT MET.
3. **Base condition 2, "it descends from the after commit bundle `sjl-9-001` names".** The base, `68cad8b`, *is* that
   commit (section 1's note of 2026-11-15). *Taken:* "descends from" takes the commit itself, as git's ancestry does
   (`git merge-base --is-ancestor 68cad8b 68cad8b` exits 0) and as the condition's purpose reads it, that SJL-9.4's
   cut-over is in the base. *Not taken:* a strict descendant; the base would fail the condition and every part would
   be void, though no other first parent of the after commit could exist.
4. **Base condition 3, "the after commit is its only first-parent successor that changes" a named path.** *Taken:* the
   first-parent successors from the base up to the after commit, `git rev-list --first-parent 68cad8b..983d690a`,
   which is the after commit alone, as the owner ruled on the same words in bundle `sjl-9-001`'s reading 1 on
   2026-11-11 ("Up to after commit"). *Not taken:* every first-parent descendant of the base on the default branch
   today; SJL-13's step 1 and its moves (`a8d8398b`, `389b55c9`, and before them `9775735a`) change such paths, so the
   condition would fail and every part would be void.
5. **Part A's runs, dispatched before this bundle's records.** On 2026-11-04, after the after commit merged, three runs
   of `.github/workflows/test262-evidence.yml` were dispatched from the default branch at `983d690a`, one at a time,
   each with `forms both` and a forty-digit commit: run 41823557887 tested `3b33530d`, bundle `sjl-9-001`'s base, and
   is no run of this rule; run 41825524024 tested the base, `68cad8b`; run 41827589989 tested the after commit,
   `983d690a`. The rule sets their configuration, "dispatched from the default branch with forms both and the tested
   commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base
   commit's", and the commits each runs at; it sets no time of dispatch and asks for no record before it, and its
   "Revisions" section guards a revision of the rule after a run is seen, not a reading. *Taken:* runs 41825524024 and
   41827589989 are part A's base and after runs, imported unchanged with the digests of their zips. *Not taken:* only
   runs dispatched after these readings; part A would rest on two new dispatches, whose runners would build with the
   image's newer SDK under reading 2, and these two runs would be left unread. Which runs part A reads is fixed here,
   before any report of either is read, and is not changed by what they show.
6. **Part A's C4 rows** (the rule's (c′) and (e), the same words as bundle `sjl-9-001`'s rule).
   - *How many re-runs:* three one-process runs at each commit per path, alternating the commits, a process over a path
     running each of its variants in the row's form, the row admitted when all six give one verdict and one exhausted
     dimension, as bundles `sjl-9-001` (its reading 5) and `sjl-13-002` took them. *Not taken:* one re-run at each
     commit; a row whose first pair agrees would need no more.
   - *A split between fuel and the wall clock:* a row exhausted on fuel on one side and on the wall clock on the other is
     (c′), decided by its re-runs and (e); "an exhaustion verdict that moves on fuel" under "no C2" is one with no
     wall-clock exhaustion on either side, as the owner ruled for bundle `sjl-9-001`'s reading 6 ("Accept both").
     *Not taken:* any such row is a regression, and part A NOT MET if there is one.
   - *Re-runs that agree on a wall-clock exhaustion:* admitted without (e), since the clause sends to (e) only "a (c′)
     row whose re-runs differ", as the owner ruled for bundle `sjl-9-001`'s reading 11. *Not taken:* such a row is
     raced under (e) or is a regression.
   - *(e):* run only for a row whose re-runs differ. *Not taken:* race every (c′) row.
   - *Allowances:* the part's fuel and wall clock as the hosted runs' `run.txt` states them (100,000,000 units and
     5,000 ms), the race's wall clock ten times that, under the manifest `broiler.javascript.wide`, each re-run with
     `--expect` against the tested tree's retained pin.
7. **Part A's rows in one report only.** `eng/compare-test262-forms.py` joins on path and variant and ignores a row
   present in one report only, saying how many. *Taken:* every such row is read from the reports themselves and listed,
   and, since this rule admits no class that adds or removes a variant, each is a regression. *Not taken:* the
   comparison's rows alone; a variant missing after would go unread, and part A could not show "a regression on any
   other path" absent.
8. **Part B's checks.** *Taken:* the area's checks are the three names the rule gives, the last a prefix that takes
   every check the base's `IntlChecks.cs` writes under it; the guards are every other check whose name, as the base
   commit's `--checks --verbose` prints it, begins `intl/`, `tzdb/` or `temporal/`; a check "passes" when its line
   reads `ok`, so a check the machine does not run is no pass. *Not taken:* names read from the source's literals
   alone, which miss the names a template writes. Either reading is checked against the other, and a check present in
   one list and not the other is reported.
9. **Part C's allowances and backend.** *Taken:* the wall allowance "ample and the same at both commits" is 600,000 ms,
   as bundles `sjl-5-001` to `sjl-13-002` took it; the bisection runs between 1 and 1,000,000,000 units, as bundle
   `sjl-9-001`'s did; the native form is under `x86-64-sysv`, the backend this `linux-x64` host arms. *Not taken:* any
   other wall allowance; a twin that completes inside the fuel ceiling is not near it.
10. **Clause 8's "every root", for the run this stage takes beside the rule.** *Taken:* every JavaScript root this
   machine can publish, ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, the Android root being EX-126,
   as bundles `sjl-9-001` and `sjl-13-002` read it. *Not taken:* the Android root too, which no machine of the
   programme publishes. No part of this rule turns on it.

*(Noted 2026-11-15, at SJL-17.6's review; the readings above are left as filed.)*
- **Reading 4's alternative** names "SJL-13's step 1 and its moves (`a8d8398b`, `389b55c9`, and before them
  `9775735a`)". That is loose. Of those merges, `9775735a` (#192) merged SJL-13's control C22, `a8d8398b` (#196) the
  review fixes to SJL-13 step 1's records, and `389b55c9` (#205) a records change of SJL-9, so they are not all SJL-13
  step 1's moves; and most first-parent merges after the after commit change named paths, as
  [`after/commits/commit-conditions.txt`](after/commits/commit-conditions.txt) lists them, each with how many. What the
  alternative would do, every part void, stands.
- **Reading 6** calls (c′) and (e) "the same words as bundle `sjl-9-001`'s rule". Clause (e) is word for word. The (c′)
  clause differs in two places: it names the machine as "the local machine bundle sjl-9-001's rule names" where that
  rule says "the local machine this file's header names", and it writes "one whose re-runs differ" for "a (c') row
  whose re-runs differ". Neither changes what the re-runs must do: the mechanism is the same.
- **The preamble** says that of part A's runs "this stage had read only each artifact's `run.txt`". To read those
  files, the three artifacts were downloaded and extracted whole by 20:02Z on 2026-11-15; the readings were committed
  at 20:07Z (`896e33ff`); the first script that reads a report or a log, `part-a-compare.sh`, was written at 20:08Z and
  run at 20:28Z (section 5's table). The times are the file times of this stage's scratch directory, which is not
  retained, and the commit's date: they show the order of these steps, not what was read before the commit.

## 2. What this bundle will hold

- **`base/`**: part A's base dispatch, imported unchanged with the digest of its zip; part B's check runs and the
  datasets' state at the base commit; part C's `fuel-parity.log` with each twin's bisected allowance.
- **`after/`**: the same at the after commit, with part A's comparisons and its (c') and (e) re-runs, part B's
  comparison, and part C's runs at each ceiling tried.
- **`vectors/`**: this family's runs of the collation vectors SJL-17.2 adds, at the corpus revision its pin names.
  *(2026-11-02: its first runs, on corpus revisions 33 and 34 from scratch archives, are there; section 3 reads them.)*

The correspondence's `probes/` and `runs/` stay as committed.

*(Noted 2026-11-15, at SJL-17.6.)* `base/` and `after/` hold what the list above names, and more, as section 5's "What
the folders hold" says: part B's comparison is `after/datasets/`, part A's comparisons, re-runs and tables
`after/parta/`, and the git reads and the scripts `after/commits/` and `after/scripts/`. The closures at the after
commit and the rule checks there are [`after-closures/`](after-closures/README.md), which the list does not name.
[`manifest.json`](manifest.json) lists every file of the bundle with its SHA-256. The list is left as written.

*(Noted 2026-11-16, at SJL-17's exit reading.)* [`exit-reading/`](exit-reading/) holds the head's rule checks, the
deletion searches at the head and the verification of this bundle's hashes and manifest that section 11 reads, with the
scripts that took them; its own [`hashes.txt`](exit-reading/hashes.txt) holds the SHA-256 of every other file of it,
which `sha256sum -c` verifies, and `manifest.json`, regenerated after it, lists it. The list and the note above are left
as written.

*(2026-11-02, at SJL-17.3: [`han-sizing/`](han-sizing/README.md) retains the sizing of Han's radical-stroke order
that the owner's decision E-o3h rests on, its scripts, inputs named and outputs; section 3 reads it. It is not a run of
the rule.)*

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the section
that reads that log (rule H5). A test262 suite total is never quoted: part A is decided per variant, and totals decide
nothing.

## 3. Engine-level vectors on collation (SJL-17.2)

*(Added 2026-11-02, at SJL-17.2.)*

Broiler.JS's corpus revision 33 adds the collation vectors, four files under `conformance/vectors/collation/`, of the
area `intl`. Every `out` is the Intl area's reference's, ICU 78.3 with CLDR 48.0 through Node.js v26.12.0
(`ref:intlref@1`, JSD-0082 decision D17); the pairs of CLDR 48.2.0's archived CollationTest files cite those files
too, and the root's Han vectors a second reference, the UCA over `allkeys_CLDR.txt` with UCA 17's implicit weights.
Revision 34 classifies both engines' first runs, on Broiler.JS's branch `sjl-17-2-vectors` (observed, not merged).
- **This family's run has no difference on the area.** The 34 Han vectors whose answer moves with the Han order are
  `impl-defined`, and this family meets its own expectation, its tables' implicit weights, on each, by the owner's
  decision E-han of 2026-11-02, which
  [JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md)
  records. The vectors under `es`, `sv` and `th` expect of it the `en-US` answer that its locale list resolves (rule
  N28), and it gives that too. So revision 34 writes no `divergence.broiler-vm` field, and row O3's reading (F1) stays
  open for SJL-17.3. *(Note of 2026-11-02, later, at SJL-17.3: it is settled by the owner's decision E-o3h of that
  day. The implicit weights by block stay, and this family's expectations in revision 34 do not move. The sizing the
  decision rests on is retained in [`han-sizing/`](han-sizing/README.md), and this section's last paragraph states
  its figures.)*
- **Broiler.JS's differences** are its own, each under a row of the [correspondence](correspondence.md): O2, O3,
  O5a, O5b, O7 and O8. Two rows carry dated notes for answers the run showed, by the owner's decisions E-o5b and E-o3j
  of 2026-11-02. Broiler.JS's bundle of the same name reads them, and this bundle does not.
- **The vectors pin does not move here.** It stays where SJL-11's task 3.5 leaves it until a later task moves it to
  revision 34, after Broiler.JS merges it (rule N13). *(Note of 2026-11-11: SJL-9's work package SJL-9.6 moved it from
  revision 32 to revision 38 (`61394d0`), which carries revisions 33 and 34's collation vectors unchanged; bundle
  [`sjl-9-001`](../sjl-9-001/README.md)'s section 5 retains the run on it, where every collation vector matches with
  the expected value, rendering and verdict of the revision 34 run below. Those revision 34 classified from CI's
  `windows-latest` host after that run's scratch pin are `divergence` there where they are `normative` in that run,
  through `divergence.broiler-js` fields this family's run does not read. No task moved it to revision 34.)*

**The runs.** Each was taken at `b43d0bb`, this repository's default branch (Broiler-Platform/Broiler.VM#180), from a
scratch archive of the Broiler.JS commit the run names. The archive was written twice by `git archive`, byte for byte
the same, and the second was hashed for a scratch pin. Neither archive nor pin is committed to this repository's pins;
the pins are kept here for the reader. The conformance root was built in Release, in a scratch tree written by `git archive` of `b43d0bb`, with `--disable-build-servers` and
`-warnaserror` ([`vectors/build-conformance.log`](vectors/build-conformance.log)). Each run used `TZ=UTC`, no `LANG`,
`LC_ALL` or `LANGUAGE`, and a script that wrote each log whole: a header naming the commit, the build, the archive and
its pin, the host and the command; then the runner's output; then the exit code and the end time. Each run's negative
control is a second run with `--mutate intl/collation/options/sensitivity/0001`, a row O6 vector both engines match.
[`vectors/hashes.txt`](vectors/hashes.txt) holds every file's SHA-256. The logs' headers name the archive and the pin as
the run's scratch folder named them: `broiler-vm-revision-33.pin` and `broiler-vm-revision-34.pin` are this bundle's
`scratch-pin-revision-33.pin` and `scratch-pin-revision-34.pin`, byte for byte, and `corpus-r33.tar.gz` and
`corpus-r34.tar.gz` are the second of the two archives of each run's corpus commit. The logs are kept as written,
since their hashes are recorded.

| Run | Corpus | Result | Files |
|---|---|---|---|
| Revision 33, the first run | `d85da200`, scratch pin [`scratch-pin-revision-33.pin`](vectors/scratch-pin-revision-33.pin) | no difference in the collation vectors and no unclassified difference anywhere; the other areas' classified divergences as at revision 32; the status vectors skipped and named | [`run-revision-33.log`](vectors/run-revision-33.log), [`run-revision-33.tsv.gz`](vectors/run-revision-33.tsv.gz) |
| Its negative control | the same | the same, and the mutated vector the one unclassified difference | [`run-negative-control-revision-33.log`](vectors/run-negative-control-revision-33.log), [`run-negative-control-revision-33.tsv.gz`](vectors/run-negative-control-revision-33.tsv.gz) |
| Revision 34 | `60fc3a5b`, scratch pin [`scratch-pin-revision-34.pin`](vectors/scratch-pin-revision-34.pin) | 0 unclassified differences: no collation vector differs, so none is classified for this family, and the other areas' classified divergences are as at revision 33, none no longer holding; the status vectors skipped and named | [`run-revision-34.log`](vectors/run-revision-34.log), [`run-revision-34.tsv.gz`](vectors/run-revision-34.tsv.gz) |
| Its negative control | the same | the same, and the mutated vector the one unclassified difference | [`run-negative-control-revision-34.log`](vectors/run-negative-control-revision-34.log), [`run-negative-control-revision-34.tsv.gz`](vectors/run-negative-control-revision-34.tsv.gz) |

**The same runs are retained in Broiler.JS's bundle `sjl-17-001`**, in its `engine/`, as the basis of its fields, with
Broiler.JS's own runs on both revisions.

*(2026-11-02, at the change's review: after these runs, Broiler.JS corrected the texts of two of its reasons in revision
34 (`f3bb5588`, observed on its branch). That rewrote 19 `divergence.broiler-js` fields and nothing else. No field of
this family changed, so the revision 34 run above stands for revision 34 as it now is. The scratch pin's archive is of
`60fc3a5b`, as the table says.)*

*(2026-11-02, before the merge of Broiler.JS's change. Broiler.JS's CI run 41418502645
(Broiler-Platform/Broiler.JS#1091) ran its corpus revision 34 on `engine vectors (windows-latest)`, job 125188274552:
Windows Server 2025, image `windows-2025-vs2026` 20261021.278.1, where .NET's globalization is the operating system's
`icu.dll`, of a version neither the job's log nor the image's software list states. The job failed on differences of
Broiler.JS that the host above did not show: that host's collator applies none of German phonebook, German search and
root search under `en`. Broiler.JS's revision 34, corrected in place before its merge, classifies them for Broiler.JS
under rows O16 and O6, row O6 read for that host by the owner's decision E-o6w of 2026-11-02, which
[JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md)
records with a dated note on row O6 in the [correspondence](correspondence.md) (observed on Broiler.JS's branch
`sjl-17-2-win`). That adds `divergence.broiler-js` fields, and those vectors' `class` reads `divergence` where it read
`normative`. This family's verdict on a vector reads only its own fields (`VectorsCommand.cs`), and its answers there
are its tables', which read no host, so the revision 34 runs above give its verdicts on revision 34 as it now is. A
run taken again would give the same verdicts; its report would differ from theirs in that `class` column, and in the
corpus commit and content hash its header names, since it would be run from another scratch archive and pin than the
`60fc3a5b` ones the table names.)*

**Han's radical-stroke order, sized (2026-11-02, at SJL-17.3).** The sizing that the owner's decision E-o3h in
[JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md)
rests on is retained in [`han-sizing/`](han-sizing/README.md), whose README names its inputs and commands; every figure
here is in [`han-sizing/size-han.txt`](han-sizing/size-han.txt), and those of the forms `size_rs.py` gives in
[`han-sizing/size_rs.txt`](han-sizing/size_rs.txt) too. From Unicode 17.0.0's `Unihan.zip` (SHA-256
`f7a48b2b545acfaa77b2d607ae28747404ce02baefee16396c5d2d7a8ef34b5e`), every one of the 101,996 unified ideographs
carries a `kRSUnicode` value. In radical-stroke order they form 20,494 runs of consecutive code points, which take
102,470 bytes as a fixed-width run list, 73,953 bytes delta-coded and 42,800 bytes compressed with xz. A rank per code
point takes 216,742 bytes packed. Rule N28's headroom is 11,603 bytes (840,365 of 851,968; bundle
[`sjl-8-001`](../sjl-8-001/README.md)'s section 12). So every form measured is several times that headroom, about 3.7
times at the smallest; the same run list big-endian, which the decision did not weigh, compresses to 38,908 bytes,
still more than three times it. The 35 ideographs the collation vectors of corpus revision 34 reach would take 210
bytes, but a table of those alone orders no other ideograph.

## 4. SJL-17.4's cut-over, on its branch

*(Added 2026-11-04, at SJL-17.4.)* This family's cut-over is made on the branch `sjl-17-4-cutover`, cut from `68cad8b`
(this repository's default branch after SJL-9.4's merge, the commit bundle `sjl-9-001` names as its after commit when it
records it), and lands in one merge: that merge is this bundle's after commit, and its first parent the base, each named
here by all forty digits when it lands. *(Note of 2026-11-15: the branch merged on 2026-11-04 as `983d690a`
(Broiler-Platform/Broiler.VM#191, observed), its first parent `68cad8b`, and section 1 names both by all forty digits,
which this section did not do when it landed, as it should have.)*
[JSD-0086](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0086-the-profile-collates-with-the-shared-intl-library.md)
records it. The branch's local pre-checks, against answers on binaries built at `68cad8b` and kept, are no run of this
bundle's rule and are kept outside the tree; no figure of them is quoted. The base's test262 runs, checks, twins and
probes were taken before any change was committed; its collation vectors ran on the kept base binaries after the
cut-over's first commit, and the cut-over's finished after the next, which changes no product source (JSD-0086,
section 4). Nothing under `src/tests/cldr/` or
`src/tests/temporal/` changes on the branch, and `JsCldrTables.g.cs` is the same file at both ends of it (rule N28). No
root's assemblies move, so the composition register's evidence stays bundle `sjl-9-001`'s `cutover/closures/`. Part A's
hosted runs, parts B and C, and the exclusion for the code that left this component's assurance coverage are SJL-17.6's.
*(Note of 2026-11-15, at SJL-17.6: they are taken, in sections 5 to 9 and 10; the exclusion is EX-136. This section
is left as written.)*

## 5. The run of the rule (SJL-17.6)

*(Added 2026-11-15, at SJL-17.6.)* Every part of the rule is taken at the base `68cad8baf7403125d44d5007441d30ff07bc4edf`
and the after commit `983d690adac63f6d117ff2cd2d9cca61fc16e3d2` and judged here, on section 1's readings taken before
any run was read. Each part decides alone, from the files named with it, and no part reads another's verdict. No
test262 suite total is quoted: part A is decided per variant.

### What the folders hold

- **[`base/`](base/)**: [`hosted/`](base/hosted/imported.txt), part A's base run, imported unchanged;
  [`datasets/`](base/datasets/), part B's run (`checks.log`, the whole output with the binary's digest;
  `named-checks.txt`, the line of each check the rule names; `named-check-names.txt`, those names as the base's run
  prints them; `source-names.txt`, the same names read from the base's sources, section 1's reading 8;
  `files.sha256`, every file under `src/tests/cldr/` and `src/tests/temporal/`); [`fuel-parity.log`](base/fuel-parity.log),
  part C's bisection; and [`local/`](base/local/), the archive, the builds and each run's command, gate log, machine
  record before and after with the whole process list, exit code, wall time and peak memory.
- **[`after/`](after/)**: [`hosted/`](after/hosted/imported.txt), part A's after run, imported the same way;
  [`parta/`](after/parta/), the completeness of all four runs, both comparisons with `commands.log`, every changed
  verdict with its class (`changed-verdicts.tsv`), the named collation directories' rows (`rows-collation.txt`), the
  (c′) re-runs with every report and record (`rerun/`), and the note that no row is raced (`race-note.txt`);
  [`datasets/`](after/datasets/), part B's after run and its comparison with the base (`compare.txt`);
  [`fuel-parity.log`](after/fuel-parity.log), part C at each ceiling tried; [`commits/`](after/commits/), the git reads
  behind the commit conditions; [`local/`](after/local/), as at the base; and [`scripts/`](after/scripts/), every
  script that took or read these files, as run, each named `.txt`.
- [`base/hashes.txt`](base/hashes.txt) and [`after/hashes.txt`](after/hashes.txt) hold the SHA-256 of every file of
  their folder outside `hosted/`, and each `hosted/hashes.txt` every file of that folder; `sha256sum -c` verifies each.

### The commits, and the conditions the rule's header sets

[`after/commits/commit-conditions.txt`](after/commits/commit-conditions.txt) is the output of
[`commit-conditions.sh.txt`](after/scripts/commit-conditions.sh.txt), git reads alone. Every condition holds on those
reads, on section 1's readings 3 and 4 for the base's second and third conditions, so no part is void.

| Commit | Condition (the rule's "The commits") | The git read, and what it shows |
|---|---|---|
| Rule commit `4e6a41e9d3860b8ca79d5d3ea6de215c1592b0c0` (Broiler-Platform/Broiler.VM#179) | carries `decision-rule-area.md` | `git log --diff-filter=A` names `c71b4a889c208366844671a3d7f144caed0ae780`, in the rule commit and not in its first parent, and no later commit changes the file; its blob is `7217a3f2ba7e96aa6a78db1346bc5091bf304073` at the rule commit, the base, the after commit and this branch's head when the read was taken; the bundle holds no dated revision of the rule |
| After commit `983d690adac63f6d117ff2cd2d9cca61fc16e3d2` (#191) | the default-branch merge that lands SJL-17.4's cut-over, one merge | on the default branch's first-parent line; parents `68cad8b` and `ad03dfab`, the branch `sjl-17-4-cutover` that JSD-0086 records |
| Base `68cad8baf7403125d44d5007441d30ff07bc4edf` (#190) | the after commit's first parent | `git rev-parse 983d690a^1` is `68cad8b` |
| | 1. descends from the rule commit | `git merge-base --is-ancestor 4e6a41e9 68cad8b`: yes |
| | 2. descends from the after commit bundle `sjl-9-001` names, `68cad8b` | the base is that commit, and `git merge-base --is-ancestor` exits 0 (section 1's reading 3) |
| | 3. the after commit is its only first-parent successor that changes a named path | `git rev-list --first-parent 68cad8b..983d690a` is `983d690a` alone (section 1's reading 4); the read lists every later first-parent merge to the default branch's `4f914a3f` with how many such paths each changes, most of them some |
| Part A's runs | dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base's | each `imported.txt` and `run.txt` names its tested commit and inputs, both dispatched from the default branch at the after commit; `.github/workflows/test262-evidence.yml` is blob `740a70f8…` and `eng/test262-evidence.py` blob `55f71ee3…` at the base and the after commit, and so are `eng/run-test262.py`, `eng/collect-js-release-candidate.py`, `eng/compare-test262-forms.py` and the suite pin (section 1's reading 5) |
| Parts B and C | built in Release from a clean checkout of each commit, on the local machine | each from a clean `git archive` of its commit on a tmpfs ([`base/local/archive.txt`](base/local/archive.txt), [`after/local/archive.txt`](after/local/archive.txt)), on section 1's reading 1 |

**The data and the pins** (the same file). `JsCldrTables.g.cs`, `src/tests/cldr/pins/cldr.pin`, the test262 pin, the
vectors pin, `Directory.Packages.props` and `SuiteFeatures.cs` have the same blob at both commits, and `git diff`
between them over `src/tests/cldr/`, `src/tests/temporal/`, `src/Broiler.VM.Profile.JavaScript.Intl/` and
`src/tests/forms/` is empty: the after commit changes, under the rule's named paths, the five product files JSD-0086
names and four of the profile's documents.

### The machine and the gate

The local steps were taken on 2026-11-15 on the machine section 1's reading 1 takes as the rule's: `linux-x64`, four
logical processors, Ubuntu 24.04.5 LTS, .NET SDK 10.0.404, the native backend `x86-64-sysv`. Each tree is a clean
`git archive` of its commit on a tmpfs, and each root was built there in Release with `--disable-build-servers`,
`MSBUILDDISABLENODEREUSE=1` and shared compilation off ([`build-roots.sh.txt`](after/scripts/build-roots.sh.txt)): the
slice compiler, the command-line root and the conformance root, one build at a time, with no warning in any build log
(`<side>/local/builds/*.warnings.txt`, each empty). Every build and every run ran under an exclusive lock that the
other stages working on this machine take too, so that one run is taken at a time, and behind a hard gate
([`gate.sh.txt`](after/scripts/gate.sh.txt), with [`run.sh.txt`](after/scripts/run.sh.txt)) that waits, with no time
limit, for a one-minute load of at most 1.0 and no process whose executable is `dotnet`, `testhost`,
`VBCSCompiler`, `MSBuild`, `ilc`, `crossgen` or a `Broiler.*` program; every gate pass in these folders found none
and a load of at most 1.0, and no `stop-servers` line names a leftover process. Other stages' runs took the lock
between this bundle's steps, never during one. Part A's (c′) re-runs took the lock once for the whole batch, so that
no other stage's run came between them, and each re-run still passed the gate first
([`part-a-rerun.sh.txt`](after/scripts/part-a-rerun.sh.txt), [`run-inner.sh.txt`](after/scripts/run-inner.sh.txt), which
is `run.sh` without its own lock).

| Step | Taken (UTC) | Records |
|---|---|---|
| Part A's hosted runs | 2026-11-04: base 12:00:17 to 12:08:05, after 12:14:43 to 12:22:30 | `base/hosted/`, `after/hosted/` |
| Part A's comparisons (Python only) | 20:28:48 to 20:28:50 | `after/parta/commands.log`, `after/parta/logs/` |
| Builds at the base | 20:40:44 to 21:06:01 | `base/local/builds/` |
| Part B at the base | 21:15:55 to 21:20:12 | `base/local/partb/`, `base/datasets/` |
| Part C at the base | 21:24:47 to 21:26:31 | `base/local/partc/`, `base/fuel-parity.log` |
| Builds at the after commit | 21:27:21 to 21:36:41 | `after/local/builds/` |
| Part B at the after commit | 21:37:10 to 21:41:23 | `after/local/partb/`, `after/datasets/` |
| Part C at the after commit | 21:41:49 to 21:42:03 | `after/local/partc/`, `after/fuel-parity.log` |
| Part A's (c′) re-runs | 21:42:26 to 21:50:51 | `after/parta/rerun/` |

### The verdicts

| Part | Verdict | What it reads |
|---|---|---|
| A, test262 in both forms (section 6) | **MET**, on section 1's readings 1 to 6 | [`after/parta/`](after/parta/): `completeness.txt`, `compare-wide-bytecode.log`, `compare-wide-native.log`, `changed-verdicts.tsv`, `rows-collation.txt`, `rerun/`, `race-note.txt` |
| B, the orderings dataset, the collation checks and the guards (section 7) | **MET**, on section 1's readings 1, 3, 4 and 8 | [`base/datasets/`](base/datasets/), [`after/datasets/`](after/datasets/): `checks.log`, `named-checks.txt`, `files.sha256`, `compare.txt` |
| C, the fuel-parity twins (section 8) | **MET**, on section 1's readings 1, 3, 4 and 9 | [`base/fuel-parity.log`](base/fuel-parity.log), [`after/fuel-parity.log`](after/fuel-parity.log), each side's `local/partc/attempts.tsv` |

**Which readings each verdict turns on.** Every reading is section 1's, taken before any run was read, under the
owner's standing direction of 2026-10-18 and not the owner's own answer. Readings 3 and 4 would void every part if
ruled the other way. Reading 1 would leave parts B and C untaken and part A's fifteen (c′) rows unadmitted, so each
part NOT MET until runs on another machine. Under reading 2's alternative part A could not be taken; under reading 5's
it would rest on two new dispatches. Of reading 6, the split between fuel and the wall clock decides part A: fourteen
of its fifteen rows are exhausted on fuel at one commit and on the wall clock at the other, and would be regressions
under the alternative; no row's re-runs agree on a wall-clock exhaustion and no row's re-runs differ, so its other
items decide nothing here. Reading 7 decides nothing, since no row is in one report only, and neither does reading 8,
whose two lists are the same. Reading 9 is the precedent's allowance. No verdict needs an exclusion (section 9).

## 6. Test262, both forms (rule part A)

*(Added 2026-11-15, at SJL-17.6.)*

**MET.** "MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1
or C4 and is listed, and the native pair meets the same conditions."
- **All four runs are complete** ([`after/parta/completeness.txt`](after/parta/completeness.txt), read off the imported
  artifacts by [`part-a-tables.py.txt`](after/scripts/part-a-tables.py.txt)). In each form at each commit: sixteen
  shard logs of sixteen, and the merge took sixteen shard reports; the run log's exit code is 1 in the bytecode form
  and 4 in the native form, both of which completeness admits; the merged report's retention object says retainable,
  its coverage is whole and it accounts for every variant; the binaries log has one distinct list of assemblies. Every
  runner and the merging job of both runs report .NET SDK 10.0.403; the images are `ubuntu24` 20261026.354.1 but one
  runner of the base run and two of the after run on 20261101.360.1, as each `imported.txt` records (section 1's
  reading 2).
- **The comparisons** ([`compare-wide-bytecode.log`](after/parta/compare-wide-bytecode.log),
  [`compare-wide-native.log`](after/parta/compare-wide-native.log)) are `eng/compare-test262-forms.py --same-form` as
  the base commit holds it (the same blob after), the base's report as control and the after's as candidate, the native
  pair with `--exempt-guest-loads` over the pin's archive extracted whole: its SHA-256 is the archive's, it holds the
  pin's 56,560 files and `tar --compare` finds no difference ([`commands.log`](after/parta/commands.log)). Each exits 0,
  every row each lists in class (c′), and neither ignores a row: no variant is in one report only (section 1's reading
  7). The bytecode pair lists 8 changed variants and the native pair 7.
- **No C1 row**, and so no finding against JSD-0084's correspondence: no variant fails at the base and passes after,
  in either pair. **No C4 (a) or (b) row.**
- **The named collation directories** ([`rows-collation.txt`](after/parta/rows-collation.txt)): in both forms, every
  variant of `test/intl402/Collator/`, `test/built-ins/String/prototype/localeCompare/` and
  `test/intl402/String/prototype/localeCompare/` is in both reports and gives the same verdict at both commits. The
  four `Collator` variants that fail, both variants of `resolvedOptions/ignorePunctuation-default.js` (Thai's default)
  and of `resolvedOptions/resolved-collation-unicode-extensions-and-options.js` (`de` with the collation `eor`), fail
  alike at the base and after; this family carries neither Thai nor `eor` (JSD-0084's decision E3).
- **C4 (c′): the fifteen rows** ([`changed-verdicts.tsv`](after/parta/changed-verdicts.tsv)): in the bytecode pair `annexB/built-ins/RegExp/RegExp-trailing-escape-BMP.js`
  [strict], exhausted on `NestedLoadFanOut` at the base and on the wall clock after, and both variants of
  `staging/sm/Date/dst-offset-caching-3-of-8.js`, `-4-of-8.js` and `-7-of-8.js` with
  `staging/sm/TypedArray/sort_large_countingsort.js` [strict], each on fuel at the base and on the wall clock after; in
  the native pair both variants of `dst-offset-caching-4-of-8.js`, `-7-of-8.js` and `sort_large_countingsort.js`, each
  on the wall clock at the base and on fuel after, and `dst-offset-caching-6-of-8.js` [sloppy], on fuel at the base and
  on the wall clock after. Each row's three one-process re-runs at each commit, alternating the commits, with
  `--expect` against the tree's retained pin and every log naming the pin
  ([`rerun/`](after/parta/rerun/)), give one verdict and one exhausted dimension in all six: exhausted on fuel, or on
  `NestedLoadFanOut` for the `RegExp` row, never on the wall clock
  ([`rerun-wide-bytecode.tsv`](after/parta/rerun/rerun-wide-bytecode.tsv),
  [`rerun-wide-native.tsv`](after/parta/rerun/rerun-wide-native.tsv)). The re-runs agree, which (c′) admits (section
  1's reading 6). Each re-run exits 1, the conformance root's answer when its selection executes no test to
  completion (`NoExecutedTests`), and its report gives the verdict.
- **C4 (e)** has no row: no row's re-runs differ, and the race runs only such rows
  ([`race-note.txt`](after/parta/race-note.txt)).
- **No C2**: fourteen rows are exhausted on fuel at one commit and on the wall clock at the other; under section 1's
  reading 6 such a row is (c′), decided by its re-runs, and none moves on fuel with no wall-clock exhaustion on either
  side. The `RegExp` row moves between `NestedLoadFanOut` and the wall clock. No variant is a regression.

## 7. The orderings dataset and the collation checks (rule part B)

*(Added 2026-11-15, at SJL-17.6.)*

**MET.** "MET if and only if every check named passes at both commits, and no file under src/tests/cldr/ or
src/tests/temporal/ differs between them."
- **The checks named** ([`base/datasets/named-checks.txt`](base/datasets/named-checks.txt),
  [`after/datasets/named-checks.txt`](after/datasets/named-checks.txt)), on section 1's reading 8: the area's four,
  `intl/i1/german-phonebook-and-search-order`, `intl/i1/german-and-english-orderings-match-icu` (the `orderings`
  dataset against its retained answer) and `intl/i1/the-root-collation-orders-collationtest-non-ignorable` and
  `-shifted` (the archived CollationTest files); and thirty guards, every other check whose name begins `intl/`,
  `tzdb/` or `temporal/` as the base's run prints them. The names the base's sources spell, with the two templates
  each check of theirs takes, are the same thirty-four, none printed without a source and none spelled and not printed
  ([`source-names.txt`](base/datasets/source-names.txt)).
- **Every one passes at both commits** ([`after/datasets/compare.txt`](after/datasets/compare.txt)): each prints `ok`
  in the slice compiler's `--checks --verbose`, built in Release from a clean archive of each commit, and each check's
  whole line, its answer included, is the same at both. Each run exits 0, every check of the run passing or not run on
  this machine and claimed by nothing ([`checks.log`](after/datasets/checks.log)); the three not run are native rows
  of the `x86-64-win64` backend, which this host does not arm, as at bundle `sjl-9-001`.
- **No file differs**: `git diff` between the commits over `src/tests/cldr/` and `src/tests/temporal/` is empty, and the
  181 files under them hash alike at both commits (`files.sha256`, the same file at both).

## 8. The fuel-parity twins (rule part C)

*(Added 2026-11-15, at SJL-17.6.)*

**MET.** "MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling
tried, and completes with the base's answer where it completes."
- **The base** ([`base/fuel-parity.log`](base/fuel-parity.log)): the ten `*.small.js` twins of `src/tests/forms/wide`,
  each one's smallest completing fuel allowance bisected under the bytecode form with the command-line root, then each
  form at that figure and one less, the native form under `x86-64-sysv`, with the wall allowance 600,000 ms (section 1's
  reading 9); every attempt is in [`base/local/partc/attempts.tsv`](base/local/partc/attempts.tsv). Each twin completes
  at its figure and is refused with `AllowanceExhausted` on fuel one below, in both forms, with one answer in both.
- **After** ([`after/fuel-parity.log`](after/fuel-parity.log), [`attempts.tsv`](after/local/partc/attempts.tsv)): every
  twin, in both forms, completes at its base figure with the base's answer and is refused on fuel one below, so it gives
  the base's verdict at each ceiling tried. The figures are those bundle `sjl-9-001` retains for this commit as its
  after commit.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken (section 3 since 2026-11-02). The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-13-001`. *(Added 2026-11-15, at SJL-17.6: EX-136,
for the collation code SJL-17.4 moved out of this component's assurance coverage, joins the table, and the row of
SJL-17.1 is kept as written.)* *(Noted 2026-11-15, at SJL-17.6's review: the first sentence, "Sections 3 to 8 are
added as the bundle's runs are taken", is left as written; SJL-17.6's runs are in sections 5 to 8, and section 10
follows this one.)*

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-17.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
| EX-136 | Open | **The collation code that left this component's assurance coverage** *(added 2026-11-15, at SJL-17.6, as JSD-0086 section 7 names it)*. The types and members rule I5 registers for SJL-17.4 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at SJL-17.4's cut-over, the after commit `983d690a`: `JsCollator.cs`, with `JsCollator` and `JsCaseFirst`, and `JsCollationData.cs`, with `JsCollationData` and its nested `Tailoring`, whole; `JsRealm.CollatorKeyData`; `JsIntlTables.Collation` with its lazy `collation`; and `NewCollator`'s tailoring core, lines of a member that stays (JSD-0086 section 5). What computes them now is `Broiler.ECMAScript.Intl`, namespace `Broiler.ECMAScript.Intl`: `UcaCollator`, `UcaCollatorOptions`, `UcaCollationData`, `CollationStrength` and `CollationCaseFirst`. The home's own assurance covers it in the Broiler.JS repository: `Broiler.ECMAScript/hairs.config.json` lists the library's project, and its generated manifest, `Broiler.ECMAScript/hairs.manifest.json`, lists every unit of the library's assembly with an annotation on each relevant declaration, checked in CI (rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count them. The adapters of JSD-0086 section 2 stay in this component and stay covered: `NewCollator` with every option read in the specification's order, `CompareStrings`, `JsIntlTables.CollationData`, `CollatorLocaleData`, the `Intl.Collator` object and prototype, `JsCollatorObject` with its resolved options, the bound `compare` and its caching, `localeCompare`'s coercion, the errors and their messages, the default-locale and available-locale providers, and the collation tables this family hands the library through `IJsIntlData` in `JsCldrTables.g.cs`. EX-134 named `NewCollator` and `CollatorKeyData` as staying in this component at SJL-9.4, as they did until SJL-17.4. EX-125 and EX-127 to EX-135 say the same of the earlier areas |

*(Noted 2026-11-15, at SJL-17.6.)* EX-136 is the next free identifier: EX-135 is bundle `sjl-13-002`'s, and no bundle
defines a later one. It is no part of the rule: parts A, B and C read nothing of this component's assurance coverage,
and no verdict of sections 6 to 8 needs it or moves with it. It changes no generated assurance file, which no
exclusion of this kind is cited in. The table above is left as written but for its new row.

## 10. The closures and clauses 6, 8 and 9's runs at the after commit (SJL-17.6)

*(Added 2026-11-15, at SJL-17.6.)* The rule's "What this file does not decide" leaves this family's clauses 6, 8 and 9
to their own mechanisms: rules I1 to I7 and I5's rows, three-mode publication, optionality under I3, and N28's budget.
JSD-0086 section 6 leaves the composition register's closures to SJL-17.6. Each is taken at the after commit
`983d690a` in [`after-closures/`](after-closures/README.md), from a clean checkout behind the lock and the gate of
section 5, and **read here, not judged**: no part of the rule reads them, and SJL-17's exit reading, which reads
clauses 6 to 9, follows Broiler.JS's SJL-17.5.

| Clause | Its words | What `after-closures/` holds at `983d690a` |
|---|---|---|
| 6 | "Rules `I1` to `I7` hold, and rule `I5` holds the deletion list." | The whole architecture suite passes, every test of rules I1 to I7 among them ([`rules.tsv`](after-closures/rules.tsv)). SJL-17.4's deletion list is absent from the tree, and rule I5's register holds a row of SJL-17.4 for each of its types and members ([`deletion-search.txt`](after-closures/deletion-search.txt); the same search finds each at the base, [`deletion-search-base.txt`](after-closures/deletion-search-base.txt)) |
| 8 | "Every root is published and run in three modes." | ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, each published and run under the JIT, trimming and Native AOT, every publish and run exiting as it should; the Android root is EX-126 (section 1's reading 10). Every closure, catalog and admitted package's identity is the base's byte for byte, as bundle `sjl-9-001` collected them at `68cad8b`, and SJL-9.4's (`cutover/closures/`), so no root's assemblies move at SJL-17.4; every other difference is attributed ([`compare.txt`](after-closures/compare.txt)) |
| 9 | "Optionality is unchanged, as rule `I3` shows, and N28's budget holds for the collation tables." | Rule I3 holds on the Native AOT images of ExecutionOnly, SliceCompiler, Conformance and Cli as on the project graph, in a suite that passes whole with the image records in place; rule N28's tests pass, the one that holds the table data under its budget among them ([`rules.tsv`](after-closures/rules.tsv)); `JsCldrTables.g.cs` is the same blob at the base and the after commit ([`commit-conditions.txt`](after/commits/commit-conditions.txt)), so the collation tables in it, the root, root `search`, and German `phonebook` and `search`, do not move. The collation data's size is restated by citation: bundle [`sjl-8-001`](../sjl-8-001/README.md)'s section 12 gives the table data's size against N28's budget, as section 3 of this README quotes it, and the tables have not moved since |

**The composition register.** The rows of the four roots that compile name bundle `sjl-13-001`'s
`after/publication/closures/`, a later tree, and the execution-only root's row names bundle `sjl-9-001`'s
`cutover/closures/`, whose closure and catalog for that root this collection reproduces byte for byte; rule K4 holds
the rows at the after commit, where the architecture suite passes. No row moves.

*(Noted 2026-11-15, at SJL-17.6's review; the table above is left as written.)*
- **A clean clone, not a clean archive.** The brief that set this stage's work asked for each run "from a clean `git
  archive` of its commit". The closures, the solution's build for rule I3 and the architecture suite ran instead in a
  `git clone --no-checkout` of this repository with the after commit checked out detached and no line of `git status
  --porcelain` ([`local/checkout.txt`](after-closures/local/checkout.txt)), because the collector reads git:
  `collect-area.py` calls `git rev-parse HEAD`, `git status --porcelain` and `git ls-tree -r HEAD`. Bundle
  `sjl-9-001`'s `after-closures/` was taken the same way. The build and the suite then ran in that same tree, after the
  collector's publishes in it ([`closures.sh.txt`](after/scripts/closures.sh.txt)). Only the head checks were taken
  from an archive. Nothing was re-run.
- **Clause 6's deletion list.** The row's "SJL-17.4's deletion list is absent from the tree" first rested on
  [`deletion-search.sh.txt`](after/scripts/deletion-search.sh.txt), which searches the two files, the types
  `JsCollator`, `JsCaseFirst` and `JsCollationData`, and the members `JsRealm.CollatorKeyData` and
  `JsIntlTables.Collation` with its lazy. It does not search `JsCollationData`'s nested `Tailoring` or `NewCollator`'s
  tailoring core, which the milestone's SJL-17.4 and JSD-0086 section 5 also list.
  [`deletion-search-tailoring.txt`](after-closures/deletion-search-tailoring.txt), by
  [`deletion-search-tailoring.sh.txt`](after/scripts/deletion-search-tailoring.sh.txt), git reads alone, searches
  them. At the after commit nothing declares `Tailoring` or `TailoringNamed`, the lookup the core called, and
  `NewCollator` takes its tailoring from `data.TailoringFor(...)` (`JsRealm.Intl.cs` line 239), which no file of this
  repository declares: the library's `UcaCollationData` answers it. At the base
  ([`deletion-search-tailoring-base.txt`](after-closures/deletion-search-tailoring-base.txt)) `JsCollationData.cs`
  line 108 declares `Tailoring`, and `NewCollator` chooses a tailoring by name and builds a `JsCollator` with it
  (`JsRealm.Intl.cs` lines 211 to 222). The tailoring core is lines of `NewCollator`, a member that stays, so rule I5
  has no row for it, as JSD-0086 section 5 says; and `Tailoring` left with `JsCollationData`, whose row rule I5 holds,
  with no row of its own. The row's "a row of SJL-17.4 for each of its types and members" reads so.
- **Rule I3's image records in place** is inferred, not recorded. `closures.sh` copies the five records into the
  tree's `src/i3-image-records/` with no check and no listing, and rule I3's test asserts nothing over a folder that
  holds no `mstat-*.mstat`. The copy's source held all five when it ran (`closures/mstat.sha256`, written just
  before); no record of the destination was retained. A later run should retain a listing and the digests of
  `src/i3-image-records/` taken just before the suite.
- **`compare.txt`'s labels.** [`compare-closures.py.txt`](after/scripts/compare-closures.py.txt) labels the folder it
  is given first `cutover/` whatever it is. In [`compare.txt`](after-closures/compare.txt)'s first block, which reads
  bundle `sjl-9-001`'s `after-closures/closures/`, "cutover/" and "--- cutover" mean that folder.
- **Clause 8's roots.** Section 1's reading 10 only scopes this run, to every JavaScript root this machine can
  publish. The composition register (`docs/compositions.md` section 3) also lists roots that are not JavaScript's,
  `Calculator`, `Workbench`, `WebAssembly.Execution`, `WebAssembly.Harness` and `Ubc.Fixture`, none of them published
  here, and `PolyglotCli`, which is. Whether clause 8's "every root" reaches them is for SJL-17's exit reading, as
  bundles `sjl-9-001` and `sjl-13-002` read the clause for every JavaScript root.

## 11. SJL-17's exit reading, Broiler.VM's half, 2026-11-16

*(Added 2026-11-16, after this bundle's SJL-17.6 evidence merged in Broiler-Platform/Broiler.VM#210 and Broiler.JS's
SJL-17.5 and SJL-17.6 merged in Broiler-Platform/Broiler.JS#1118 and #1119, observed.)* It reads the milestone's
clauses 6 to 9, this family's, against this bundle's evidence at the commit each piece was taken. It changes no rule,
widens none, revises no clause and declares nothing of SJL-17 met. The home's clauses 1 and 2 and Broiler.JS's clauses
3 to 5 are read in Broiler.JS's bundle `sjl-17-001` (observed). The record over both repositories, each clause 1 to 9
with its reading and where it is recorded, JSD-0084's settling condition read and the open items, is the programme
ledger's entry of this date, as that ledger placed SJL-9's and SJL-13's (its entries of 2026-11-12 and of 2026-11-15,
later still), each bundle reading its own repository's half.

**What it reads.** The milestone's words are
[`sjl-17-collation.md`](../../javascript-shared-libraries/milestones/sjl-17-collation.md) at the default branch's
`44511066bfb38952c12c81557f56a34bd21ee41e` (the merge of Broiler-Platform/Broiler.VM#212), the head when this reading
was taken, with the notes of 2026-11-01 and 2026-11-04 under its SJL-17.4. The evidence is this bundle's:
- the run of the rule at the base `68cad8b` and the after commit `983d690a` (sections 5 to 8), judged on section 1's
  readings;
- the closures and the rule checks at the after commit, [`after-closures/`](after-closures/README.md), read in section
  10 with that section's notes of SJL-17.6's review;
- **the head's rule checks**, [`exit-reading/`](exit-reading/), taken for this reading before any change of it, at
  `44511066`: a clean `git archive` on a tmpfs ([`local/archive.txt`](exit-reading/local/archive.txt)), whose scratch
  folder is named `base` in the retained records because the checks are this reading's answers before it changed
  anything, though they hold the head and not the rule's base `68cad8b`; the solution built there in Release with
  `--disable-build-servers`, with no warning ([`build-solution.out.txt`](exit-reading/local/build-solution.out.txt),
  [`build-warnings.txt`](exit-reading/local/build-warnings.txt), empty), then the architecture suite and the contract
  tests with `--no-build`, each passing ([`test-architecture.out.txt`](exit-reading/local/test-architecture.out.txt),
  [`test-contract.out.txt`](exit-reading/local/test-contract.out.txt)). Each wrote a TRX of every test, which is not
  retained ([`trx.sha256`](exit-reading/trx.sha256)); the architecture suite's rows for rules I1 to I7, N27 and N28 are
  [`rules.tsv`](exit-reading/rules.tsv), each `Passed`, by
  [`rules-from-trx.py.txt`](exit-reading/scripts/rules-from-trx.py.txt). Each step ran under the exclusive lock and
  behind the hard idle gate of section 5 ([`gate.log`](exit-reading/local/gate.log),
  [`checks.sh.txt`](exit-reading/scripts/checks.sh.txt) with [`run.sh.txt`](exit-reading/scripts/run.sh.txt) and
  [`gate.sh.txt`](exit-reading/scripts/gate.sh.txt)), whose `stop-servers` lines name no leftover process, on the
  machine section 1's reading 1 names (each run's `machine.txt`);
- [`exit-reading/deletion-search.txt`](exit-reading/deletion-search.txt), git reads alone at `44511066`
  ([`deletion-search.sh.txt`](exit-reading/scripts/deletion-search.sh.txt)): section 10's two searches of SJL-17.4's
  deletion list, `deletion-search.sh` and `deletion-search-tailoring.sh`, run one after the other, unchanged but for
  the repository they read;
- [`exit-reading/verify-hashes.txt`](exit-reading/verify-hashes.txt): every `hashes.txt` of this bundle checked with
  `sha256sum -c` in a clean archive of `44511066`, and [`manifest.json`](manifest.json) checked against the tree of the
  commit it was collected at, all of them verifying ([`verify-hashes.sh.txt`](exit-reading/scripts/verify-hashes.sh.txt),
  [`verify-manifest.py.txt`](exit-reading/scripts/verify-manifest.py.txt)).

**The head is not the after commit.** It carries the merges after it, SJL-13's step 2 and the shared stream's move to
`0.1.0-preview.11` of 2026-11-13 among them, so `Broiler.ECMAScript.Intl`, which computes this family's collation, is
at `0.1.0-preview.11` there where the after commit has `0.1.0-preview.9`. `JsCldrTables.g.cs`,
`src/tests/cldr/pins/cldr.pin`, `JsRealm.Intl.cs` and `JsIntlTables.cs` keep their blobs from the after commit to the
head, and rule I5's register keeps SJL-17.4's rows, gaining SJL-13.3's. So each clause below is read at the commit its
evidence was taken, the after commit for SJL-17.4's state, and the head's checks show that the rules clauses 6 and 9
name still hold there. The commits that carry this reading change documents and `exit-reading/` alone.
[`exit-reading/hashes.txt`](exit-reading/hashes.txt) holds the SHA-256 of every other file of that folder, which
`sha256sum -c` verifies. *(Added 2026-11-16, later, on the same branch.)* The architecture suite also reads documents (rule
I7 reads bundles), so the same checks were taken again at this change's later head
`f096dceae0d0df515f3befeebb4c36f06449361b`, which carries the reading's documents, from a clean `git archive` by the
same scripts, behind the same lock and gate: the solution built with no warning, both suites passed, the rows of rules
I1 to I7, N27 and N28 are those of [`rules.tsv`](exit-reading/rules.tsv), each `Passed`, and every `hashes.txt` of the
bundle and its manifest verified. That run is not retained; `exit-reading/` keeps the run at `44511066`.

| Clause | Its words | Read from, at the commit | Reading, and what it rests on |
|---|---|---|---|
| 6 | "Rules `I1` to `I7` hold, and rule `I5` holds the deletion list." | [`after-closures/rules.tsv`](after-closures/rules.tsv), [`deletion-search.txt`](after-closures/deletion-search.txt) and [`deletion-search-tailoring.txt`](after-closures/deletion-search-tailoring.txt) at `983d690a`, with their base searches at `68cad8b`; [`exit-reading/rules.tsv`](exit-reading/rules.tsv) and [`exit-reading/deletion-search.txt`](exit-reading/deletion-search.txt) at `44511066` | **Read met** at `983d690a` and at `44511066`: every test of rules I1 to I7 passes, in a suite that passes whole, at the after commit with the image records in place. SJL-17.4's deletion list, as the milestone's SJL-17.4 and JSD-0086 section 5 give it, is absent at both: no path `JsCollator.cs` or `JsCollationData.cs`; no declaration of `JsCollator`, `JsCaseFirst`, `JsCollationData` or its nested `Tailoring`, nor of `JsRealm.CollatorKeyData` or of `JsIntlTables.Collation` and its lazy; and `NewCollator` takes its tailoring from `data.TailoringFor(...)`, which no file of this repository declares, the library's `UcaCollationData` answering it, where at the base it chose a tailoring by name and built a `JsCollator`. The same searches find each item at the base, which shows they can. Rule I5's register holds a row of SJL-17.4 for each of its types and members, at both commits, and I5's tests, which refuse a registered member declared again and report the witness of one, pass. It rests on no ruling and no revision. It rests on JSD-0084's decision E11, taken on 2026-11-01 under the owner's standing direction of 2026-10-18, which reads the milestone's "the collation core of the `JsRealm.*` Intl files" as `NewCollator`'s tailoring core and `CollatorKeyData`, and on reading 3 below for what rule I5 can hold |
| 7 | "Whole-suite test262 in both forms, and the `orderings` reference dataset, meet the area rule." | parts A and B of [`decision-rule-area.md`](decision-rule-area.md), at `68cad8b` and `983d690a` (sections 5 to 7) | **Read met.** Part A is MET (section 6): all four hosted runs complete, every changed verdict of either pair in class C4 (c′), its re-runs at both commits agreeing, no row in class C1, and every variant of the `Collator` and `localeCompare` directories the same verdict at both commits. Part B is MET (section 7): every named check passes at both commits with the same line, `intl/i1/german-and-english-orderings-match-icu`, the `orderings` dataset against its retained answer, among them, and no file under `src/tests/cldr/` or `src/tests/temporal/` differs. Part C, the rule's third part, is MET too (section 8). It rests on no ruling and on no revision, the rule having none. It rests on section 1's readings, each taken under the owner's standing direction of 2026-10-18 before any run was read, none the owner's own answer: part A on readings 1 to 6, and **it turns on reading 6**, under whose alternative its rows exhausted on fuel at one commit and on the wall clock at the other would be regressions and part A not met; part B on readings 1, 3, 4 and 8. Under reading 3's or reading 4's alternative every part would be void, and under reading 1's parts B and C would be untaken and part A's (c′) rows unadmitted |
| 8 | "Every root is published and run in three modes." | [`after-closures/`](after-closures/README.md), at `983d690a` | **Read met** at `983d690a` for every JavaScript root this machine can publish, ExecutionOnly, SliceCompiler, Conformance, Cli and PolyglotCli, each published and run under the JIT, trimming and Native AOT, every publish and run exiting as it should, the collector's last line reading "every root published and run in three modes: True"; every closure, catalog and admitted package's identity is the base's byte for byte, as bundle `sjl-9-001` collected them at `68cad8b`, and every other difference is attributed ([`compare.txt`](after-closures/compare.txt)). It rests on no ruling and no revision, and on an exclusion, EX-126 for the Android root (bundle `sjl-3-001`), as SJL-9's clause 14 and SJL-13's clause 8 did; and on section 1's reading 10, which scoped the run, and reading 1 below, which reads the clause, both taken under the owner's standing direction of 2026-10-18. That the closures were taken in a clean clone and not a clean archive (section 10's notes), as bundle `sjl-9-001`'s were, moves no answer |
| 9 | "Optionality is unchanged, as rule `I3` shows, and N28's budget holds for the collation tables." | [`after-closures/`](after-closures/README.md)'s rule I3 on the image records and [`rules.tsv`](after-closures/rules.tsv) at `983d690a`; [`exit-reading/rules.tsv`](exit-reading/rules.tsv) at `44511066`; the blob reads of section 5 and above | **Read met.** At the after commit rule I3 holds on the Native AOT images of ExecutionOnly, SliceCompiler, Conformance and Cli as on the project graph, in a suite that passes whole, on reading 2 below: no compiler-free root holds the shared frontend, and no root carries optional data `docs/compositions.md` does not declare. At the head it holds on the graph, with its witnesses, the image half's among them. Every closure is the base's (clause 8), so no root gains or loses an assembly at SJL-17.4. Rule N28's tests pass at both commits, the one that holds the table data under its budget among them, and `JsCldrTables.g.cs` is the same blob at `68cad8b`, `983d690a` and `44511066`, so the collation tables in it, the root, root `search`, and German `phonebook` and `search`, do not move; their size is restated by citation of bundle [`sjl-8-001`](../sjl-8-001/README.md)'s section 12, as section 3 quotes it. It rests on no ruling and no revision |

**The readings this exit reading takes.** Each is taken under the owner's standing direction of 2026-10-18 ("Please do
all recommended decisions and skip waiting for review") as the recommended choice, recorded as the owner's dated
decision of 2026-11-16 and not the owner's own answer, with the alternative not taken; the owner may rule otherwise.
None changes a rule, a clause or a verdict of sections 6 to 8.
1. **Clause 8's "every root".** The composition register (`docs/compositions.md` section 3) lists roots that are not
   JavaScript's: `Calculator`, `Workbench`, `WebAssembly.Execution`, `WebAssembly.Harness` and `Ubc.Fixture`, none
   carrying an assembly of the JavaScript profile, and `PolyglotCli`, which does and was published here. *Taken:* every
   JavaScript root this machine can publish, each root that carries the JavaScript profile, the Android root being
   EX-126, as the readings of SJL-3, SJL-8, SJL-10, SJL-9 and SJL-13 took the same words; SJL-17 changes the JavaScript
   profile alone, and no assembly of the other roots moves at SJL-17.4. *Not taken:* every root the register lists;
   those five would have to be published and run in three modes at the after commit, and the clause would not be read
   met until they were.
2. **Clause 9's "as rule `I3` shows", on the image records.** Section 10's notes say that the records' placement under
   `src/i3-image-records/` is inferred, not recorded: `closures.sh` wrote the digests of the five records
   ([`mstat.sha256`](after-closures/closures/mstat.sha256)) and then copied the same folder's records into the tree
   before the suite ran, with no listing of the destination. *Taken:* rule I3 read the four records whose root it
   reads, on that script as run, the suite passing whole. *Not taken:* the image half unshown at the after commit, so
   that the clause rests on rule I3 over the project graph alone, at the after commit and the head, as SJL-9's clause
   15 read the head; the clause would still read met on that form, or, if the clause asks for the images, not until a
   run retains a listing and the digests of that folder taken just before the suite.
3. **Clause 6's "rule `I5` holds the deletion list".** The list holds `NewCollator`'s tailoring core, lines of a
   member that stays, and `JsCollationData`'s nested `Tailoring`. *Taken:* rule I5 holds the list's types and members,
   each by its own row, and the tailoring core and the nested type are read by the searches, the core being no member
   rule I5 can register and the nested type leaving with the type whose row it holds, as JSD-0086 section 5 says.
   *Not taken:* a row of rule I5 for every item of the list; no row can register lines of a member, so the clause could
   not be read met as written for the tailoring core, and would stay unmet until the list or the clause were revised.

**So, of this family's clauses 6 to 9:** each reads met on this bundle. Clause 6 rests on JSD-0084's decision E11 and
on reading 3 above; clause 7 on section 1's readings, part A turning on reading 6; clause 8 on EX-126 for the Android
root and on reading 1 above, with section 1's reading 10; clause 9 on reading 2 above; each reading taken under the
owner's standing direction of 2026-10-18 and none the owner's own answer, and no clause on a ruling or a revision.
This reading meets no gate and accepts nothing: SJL-17 may not meet its gate before SJL-9 meets SJL-9's, and so not
before SJL-7, SJL-8 and SJL-3 meet theirs, and review is deferred (EX-30). The programme ledger's entry of this date
gives the record over both repositories, JSD-0084's settling condition read, and the open items.
