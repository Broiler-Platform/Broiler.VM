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
  *(2026-11-02: its first runs, on corpus revisions 33 and 34 from scratch archives, are there; section 3 reads them.)*

The correspondence's `probes/` and `runs/` stay as committed.

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
  the expected value, rendering and verdict of the revision 34 run below. No task moved it to revision 34.)*

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
here by all forty digits when it lands.
[JSD-0086](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0086-the-profile-collates-with-the-shared-intl-library.md)
records it. The branch's local pre-checks, against answers on binaries built at `68cad8b` and kept, are no run of this
bundle's rule and are kept outside the tree; no figure of them is quoted. The base's test262 runs, checks, twins and
probes were taken before any change was committed; its collation vectors ran on the kept base binaries after the
cut-over's first commit, and the cut-over's finished after the next, which changes no product source (JSD-0086,
section 4). Nothing under `src/tests/cldr/` or
`src/tests/temporal/` changes on the branch, and `JsCldrTables.g.cs` is the same file at both ends of it (rule N28). No
root's assemblies move, so the composition register's evidence stays bundle `sjl-9-001`'s `cutover/closures/`. Part A's
hosted runs, parts B and C, and the exclusion for the code that left this component's assurance coverage are SJL-17.6's.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken (section 3 since 2026-11-02). The exclusions keep this section's number, where rule H2
reads an exclusion's definition, as in bundles `sjl-3-001` to `sjl-13-001`.

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-17.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
