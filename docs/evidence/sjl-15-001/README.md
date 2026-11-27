<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-15-001 - shared static semantics: Broiler.VM's rule and correspondence

**Milestone:** SJL-15 of the shared JavaScript libraries programme,
[`sjl-15-static-semantics.md`](../../javascript-shared-libraries/milestones/sjl-15-static-semantics.md): its work package
SJL-15.1, for this family. The windows this bundle judges are SJL-15.3, this family's lowering on the model with the
binder pass deleted, and SJL-15.4, the slice path rebuilt on the shared tree and model.

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-15.1, before any change it judges exists, and
revised the same day, before any run and before the branch that carries both merged, by
[`decision-rule-area-revised-2026-11-16.md`](decision-rule-area-revised-2026-11-16.md), which quotes it whole and leaves
it unedited: the tools read the slice binding table through one member of `SliceCompilation` (JSD-0088's decision
D-tables); the inputs part S expects to differ at window 2 are named in three classes, H, R and C, each put to the owner
by decision D-census; rule I5 registers each type deleted whole by type and each other member by member; a `--checks`
row a window deletes is listed with what replaces it; and the precedents' parts are named by their own bundles'
letters and revisions.
*(Noted 2026-11-16, after SJL-15.1 merged.)* The rule is revised a second time the same day, before any run, by the
owner's answer to JSD-0088's decision D-census ("Admit listed rows"), in
[`decision-rule-area-revised-2026-11-16b.md`](decision-rule-area-revised-2026-11-16b.md), which quotes the rule whole
and leaves both earlier files unedited; the rule commit is the commit that carries all three files. Part S admits at
window 2 class H (row SL4, hand-offs H23 and H25), class R (rows SL2, SL3, DE3 and EE1) and class C (row SL5), each
listed by row: class H now, its inputs completed at the base run, and classes R and C from the window-2 base run's
reports, the listing committed or retained here with the time it was written before any after run of window 2 is taken;
every other byte difference still fails part S. SJL-15's clause 5 is
revised to match, before any evidence.
*(Noted 2026-11-23, by JSD-0088's decision D-ab1.)* The rule is revised a third time, before any run, by
[`decision-rule-area-revised-2026-11-23.md`](decision-rule-area-revised-2026-11-23.md), which quotes the rule whole and
leaves the rule and both revisions of 2026-11-16 unedited; the rule commit is from then on the commit that carries all
four files, and the note of 2026-11-16 above's "the rule commit is the commit that carries all three files" reads as of
2026-11-16 until 2026-11-23 and is left as written. At window 1 only, it lists row AB1, Annex B.3.3's block-function
aliases, a correction of Broiler.VM across both engines: part A gains class ED, the variants of test262's
`annexB/language/function-code/block-decl-func-skip-arguments.js` whose verdict moves from pass to fail because SJL-15.3
follows the pinned edition, the file at `ccaac100` asserting an earlier edition's reading, and any other variant that
moves for row AB1's reason, each listed with the row; and part B's class F gains row AB1, an accepted input's artifact
bytes moving for that reason alone, listed with the decoded difference. Every other line of the rule and of its
revisions stands. Decision D-ab1 is the session's, taken under the owner's direction of 2026-10-18, and the owner may
overturn it. It waits on no item of "What waits, in order" below and changes none.

**Correspondence:** [`correspondence.md`](correspondence.md), this family's half, read from code at Broiler.VM
`a86b6f5d` and against the model surface at Broiler.JS `6fda6ca4`. Broiler.JS's half fills its column by a dated note in
Broiler.JS's own change for SJL-15.1.
*(Note of 2026-11-17, after Broiler.JS's SJL-15.1 merged: by Broiler.JS's decision D-js-column, Broiler.JS's column is
held in its own bundle `sjl-15-001` as `correspondence-broiler-js.md`, and the Broiler.VM change that completes
JSD-0088's X2 and X6 copies it here; it is filled in [`correspondence.md`](correspondence.md) by that file's dated note
of this day, which carries both halves. The sentence before this note read "Broiler.JS's half fills its column by a
dated note in Broiler.JS's own change for SJL-15.1" and is left as written.)*

**Records:** [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md),
the area verdict, with decisions D57 to D66 as the documents quote them and the owner's decisions of 2026-11-16.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** a rule and a correspondence, of 2026-11-16, at SJL-15.1. **Nothing here is a verdict, and no run of the rule
has been taken.** No run can be taken yet: the rule takes every base at the first parent of a window's merge, and
neither window, nor the tools commit, nor the pin commit that admits a stream version carrying the model, exists. A
collector joins this bundle when there is a run to take.

**What the rule names before it exists**, each by definition: the rule commit (the commit that carries the rule); the
tools commit; the pin commit, which admits the version of the shared stream the owner publishes with the model and moves
the vectors pin to the `semantics` vectors; and, for each window, the after commit (the merge that lands it) and the base
(its first parent). The rule's header gives each one's conditions. **Not a run of the rule:** any output taken at
another commit, a "before" output at the rule commit among them.

**What waits, in order**, none of it in this repository but the last three:
- Broiler.JS's half of SJL-15.1: its records, its `semantics` vectors (decision D61), its rule and its column of the
  correspondence *(merged in Broiler.JS as `e4e45d38`, Broiler-Platform/Broiler.JS#1120, observed 2026-11-17, with
  its `semantics` vectors at corpus revisions 42 and 43; its column is filed in this bundle's
  [`correspondence.md`](correspondence.md), and its half of JSD-0088's X2 and X6 completed, by dated notes of that
  day; this item no longer waits)*;
- Broiler.JS's later corpus revision with the eval-code inputs of probes V029, V030 and V046, classified on both
  engines' runs, before any SJL-15.2 code *(added 2026-11-17, after Broiler.JS's SJL-15.1 merged, by its decision
  D-js-revisions' note of that day)*;
- SJL-15.2's model in Broiler.JS, each merge before Broiler.JS's SJL-14 cut-over a dark window of SJL-14 under its
  decision D52, and none during SJL-11.5's hold; and none before the Broiler.VM change that carries Broiler.JS's column
  and JSD-0088's dated note completing X2 and X6 has merged, by Broiler.JS's rule revision of 2026-11-17 *("; and none
  before ... 2026-11-17" added 2026-11-17, after Broiler.JS's SJL-15.1 merged)*;
- the owner's publication of a stream version carrying the model (JSD-0083's decision D30);
- this family's tools commit and pin commit;
- the owner's answer to JSD-0088's decision D-census, before any run of part S *(given 2026-11-16, "Admit listed rows",
  and filed by the rule's revision of that day; read "the owner's answer to JSD-0088's decision D-census, before any run
  of part S;" until then)*;
- SJL-15.3's and SJL-15.4's windows, outside SJL-11.4's window, and then this bundle's runs at each base and after commit.

*(Noted 2026-11-23, after SJL-11.5's merge in Broiler.JS, observed; the list above is left as written.)* Its third
item reads "and none during SJL-11.5's hold". On Broiler.JS's default branch, at `69a8a13b`: SJL-11.5 merged as
Broiler-Platform/Broiler.JS#1126, `b2b226cd3158f0b7d990a3f4fecbe75b259b4fa1`, whose first parent is its base commit
`5429c406`, and "The hold of decision D16 ended with that merge" (Broiler.JS's `docs/roadmap/SharedLibraries.md`,
SJL-11); its after records merged as Broiler-Platform/Broiler.JS#1127, `fefb4f9c9331b7d16349ed2a1721285f2fb69fef`.
That plan reads "That hold ended with SJL-11.5's merge", and, under Broiler.JS's reading B4 of SJL-14, "W2 merges before
any SJL-15.2 model merge, so none is such a dark window"; from W2's first base run until W2 merges nothing merges to its
default branch, by the owner's answer 9 of 2026-11-19, "Allow the W2 hold", the owner's own: "The hold then starts at
the first base run, and nothing merges to Broiler.JS main until W2 merges". W2 is not merged there. So the third item
now waits, as Broiler.JS's records state it, on W2's merge, with none of SJL-15.2's model merging during W2's hold, and
on the later corpus revision of the second item, which that branch does not yet hold; its "each merge before
Broiler.JS's SJL-14 cut-over a dark window of SJL-14 under its decision D52" has no subject under reading B4. No other
item changes.

*(Noted 2026-11-24, after W2's merge in Broiler.JS, observed; the list and the notes above are left as written.)* The
note above reads "W2 is not merged there. So the third item now waits, as Broiler.JS's records state it, on W2's merge,
with none of SJL-15.2's model merging during W2's hold, and on the later corpus revision of the second item, which that
branch does not yet hold". W2 merged as Broiler-Platform/Broiler.JS#1129, `83a9bd82a5d258d3001468095ebf43f9efc3e9a7`,
and "**The hold of the owner's answer 9 ended** with that merge" (Broiler.JS's bundle `sjl-14-001`'s README, at
`4f697339d2b360b422d4c0e32898cefca3423d95`, the merge of Broiler-Platform/Broiler.JS#1130). So of what that note names,
the third item now waits only on the second item's corpus revision, which Broiler.JS's default branch, at `4f697339`,
does not yet hold: its `conformance/vectors/REVISIONS.md` lists revision 43 last. No other item changes.

*(Noted 2026-11-24, later, at Broiler.JS's corpus revision 44, observed; the list and the notes above are left as
written.)* The note above reads "the third item now waits only on the second item's corpus revision, which Broiler.JS's
default branch, at `4f697339`, does not yet hold". That revision has merged: Broiler-Platform/Broiler.JS#1131,
`8b1fa311da6af71baac6378e8b4f8597a99c1312`, its first parent `4f697339`, adds corpus revision 44, whose row in
`conformance/vectors/REVISIONS.md` reads, "[…]" marking a figure left out, "Adds […] `DirectEval` vectors, `since: 44`,
the eval-code inputs of probes V029, V030 and V046 of Broiler.JS's bundle `docs/evidence/sjl-15-001/probes/`, which
revision 43 did not carry", and classifies both engines' runs on them in
`scripts/conformance/semantics-divergences.txt`. Of Broiler.VM's, row 44 reads "run at its origin/main `90e4cd24`
(Broiler-Platform/Broiler.VM#223) from a scratch archive of revision 44's corpus as drafted and a scratch pin", and
Broiler.JS's bundle `sjl-15-001`'s `engine/commands.txt` records Broiler.VM's run on the corpus as committed too, by the
pin `broiler-vm-revision-44.pin`. So the second item is done, and the third, SJL-15.2's model, waits on none of the items
before it; Broiler.JS's records name it next, "so SJL-15.2's model comes next, then SJL-15.5, then the evidence" (its
`docs/roadmap/SharedLibraries.md`, the note of 2026-11-24 on "The order of SJL-15's Broiler.JS work"). The fourth and
fifth items, the owner's publication and this family's tools commit and pin commit, still wait on the model. Of this
family, row 44 reads: "none of the […] vectors differs, so these vectors do not show the answer row AB1 records for
Broiler.VM on probes V029 and V030, whose sources ran as an indirect eval's code, and nothing is classified for them".
The answer [`correspondence.md`](correspondence.md)'s row AB1 records for this family on those probes was given as an
indirect eval's code, and the vectors are `DirectEval`, "as eval code a direct eval at a Script's top level evaluates"
(row 44); nothing is classified for them, no row's class changes, and a later corpus revision could add the indirect
form. These runs are Broiler.JS's, retained in its bundle `sjl-15-001`'s `engine/`; they are no run of this bundle's
rule, which takes none before the bases it names exist, and this family's vectors pin does not move. No other item
changes.

*(Noted 2026-11-25, at Broiler.JS's SJL-15.2 stage 1 and its part B, observed; the list and the notes above are left as
written.)* The note above reads "So the second item is done, and the third, SJL-15.2's model, waits on none of the items
before it". The third item's model has begun to land. SJL-15.2's stage 1, "the model's foundation", merged as
Broiler-Platform/Broiler.JS#1132, `be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`, whose first parent is `8b1fa311`, the
merge of corpus revision 44; Broiler.JS's bundle `sjl-15-001`'s README names it its first model commit, having read
before the merge that "It lands after Broiler-Platform/Broiler.VM#214 and #217, each named above with its commit, and
after corpus revision 44". Part B of that bundle's rule was taken at it and at its first parent, and read in
Broiler-Platform/Broiler.JS#1133, `2545e1818e246cad506c7b67a3d6c470c0a43375`, records only: "part B read MET on the
retained runs at the model commits taken so far (`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`), limited to them; the
part's verdict waits for the last model commit before the base", taken as recommended under the owner's direction and
"flagged for the owner", the alternatives not taken being "to read part B NOT MET at `be6c4a5c` because that run's first
attempt concluded `failure`, as bundle `sjl-13-001`'s third revision of 2026-11-03 [...] reads its own part D on a push
run's first attempt ("its first attempt decides, and every attempt is retained"), a precedent this reading does not
follow; or a dated revision of the clause saying which attempt of a run it reads, which would be the owner's, since the
runs have been seen". That is Broiler.JS's reading of its own bundle, not a run of this bundle's rule, and judges
nothing here. The rest of the model is still to come, by Broiler.JS's decision D-js-stages, "**Stages 1, 2, 3, 4, 5a,
5b, 5c, 6, 7 and 8, one merge each, in that order**, part B taken for each before the next merges".
Broiler-Platform/Broiler.JS#1134, `2259fd4a1860aefc68d49b8f6f9f0200fcb6cc4d`, a change to that repository's CI that
changes no file of `Broiler.ECMAScript/`, is no model commit, and its note reads "None of it is part of part B's
reading."

This bundle's list and this repository's ledger observe each model commit after it merges. That is a convention of this
repository's records, and no precondition of a model merge: of Broiler.VM's merges, Broiler.JS's rule revision of
2026-11-17, its bundle `sjl-15-001`'s `decision-rule-revised-2026-11-17.md`, lines 37 to 43, conditions a model commit
only on "Broiler.VM's SJL-15.1 rule has merged (Broiler-Platform/Broiler.VM#214, `0ddc146f`, observed) **and after**
the Broiler.VM change that carries this engine's column and JSD-0088's dated note completing X2 and X6 has merged
(decision D-x2), each merge observed and named, with its commit, in this bundle's README before any run at the model
commit is taken", that README being Broiler.JS's, and the change of decision D-x2 being Broiler-Platform/Broiler.VM#217
(`450c785f`). A model commit that lands before this repository observes the one before it fails none of those
conditions. The fourth and fifth items, the owner's publication and this family's tools commit and pin commit, still
wait on the model; Broiler.JS's decision D-js-packable has the owner asked about a publication "only after stage 8 and
stage 9's listing". No other item changes.

*(Noted 2026-11-25, later, at Broiler.JS's SJL-15.2 stage 2 and its part B, observed; the list and the notes above are
left as written.)* The note of 2026-11-25 above reads "The rest of the model is still to come". SJL-15.2's stage 2, "the
early-error channel and the function-context rows", merged as Broiler-Platform/Broiler.JS#1135, and Broiler.JS's bundle
`sjl-15-001`'s README, having read before the merge that "This branch's merge will be the second model commit", names it
after: "**The second model commit is `6c61879bce81b1a7badf987e16c7b0531030258d`**, the merge of
Broiler-Platform/Broiler.JS#1135 (head `8b0c233c`), whose first parent is `2259fd4a1860aefc68d49b8f6f9f0200fcb6cc4d`
(Broiler-Platform/Broiler.JS#1134, ci.yml's Test step)", and "Its first parent is no model commit: nothing from
`be6c4a5c` to `2259fd4a` changes a path under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that
bundle's rule was taken at it and at its first parent, and read in Broiler-Platform/Broiler.JS#1136,
`77fe27aa4bc6e0a8a0819b32a2b1b1158a9fe913`, records only, taken as recommended under the owner's direction: "**part B
read MET on the retained runs at the model commits taken so far (`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`,
`6c61879bce81b1a7badf987e16c7b0531030258d`), limited to them; the part's verdict waits for the last model commit before
the base.**" "At `6c61879b` the rule's "the commit's CI run of every test project passed" is read on both workflows'
push runs as they concluded, each in its first attempt, so the question of which attempt decides, flagged for the owner
at the first model commit, does not arise here. The reading at `be6c4a5c`, its basis and its flag, in the note of
2026-11-24 above, stay as written. A NOT MET at any later model commit makes part B NOT MET." That is Broiler.JS's
reading of its own bundle, not a run of this bundle's rule, and judges nothing here. Broiler.JS's note of 2026-11-25 at
stage 2 also reads its own rule's part F, "every vector the adapter answers matches", at the parse: "a reading of the
session's, taken before any run of part F, which the owner ruled on 2026-11-25 to keep ("Keep the reading at the parse
(Recommended)")", the owner's own answer; that part is Broiler.JS's, and this bundle's rule has none like it. The rest
of the model is still to come, from "**Next:** SJL-15.2 stage 3, jump targets and labels, the third model commit", by
Broiler.JS's decision D-js-stages, each stage with part B at its merge. The note above's convention holds for this merge
too: this bundle's list and this repository's ledger observe each model commit after it merges, and that is no
precondition of a model merge. The fourth and fifth items, the owner's publication and this family's tools commit and
pin commit, still wait on the model. No other item changes.

*(Noted 2026-11-25, later yet, at Broiler.JS's SJL-15.2 stage 3 and its part B, observed; the list and the notes above
are left as written.)* The note of 2026-11-25, later, above reads "The rest of the model is still to come". SJL-15.2's
stage 3, "jump targets and labels", merged as Broiler-Platform/Broiler.JS#1137, and Broiler.JS's bundle `sjl-15-001`'s
README, having read before the merge that "This branch's merge will be the third model commit", names it after: "**The
third model commit is `d87044c9db5fe2a26841782308bd53e2f9294656`**, the merge of Broiler-Platform/Broiler.JS#1137 (head
`cd92761b`), whose first parent is `77fe27aa4bc6e0a8a0819b32a2b1b1158a9fe913` (Broiler-Platform/Broiler.JS#1136, stage
2's part B records)", and "Its first parent is no model commit: nothing from `6c61879b` to `77fe27aa` changes a path
under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that bundle's rule was taken at it and at its first
parent, and read in Broiler-Platform/Broiler.JS#1138, `9ae6aee09012d6b7139856f253b43f7701cf7b8b`, records only, taken as
recommended under the owner's direction: "**part B read MET on the retained runs at the model commits taken so far
(`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`, `6c61879bce81b1a7badf987e16c7b0531030258d`,
`d87044c9db5fe2a26841782308bd53e2f9294656`), limited to them; the part's verdict waits for the last model commit before
the base.**" "At `d87044c9` the rule's "the commit's CI run of every test project passed" is read on both workflows'
push runs as they concluded, each in its first attempt, so the question of which attempt decides, flagged for the owner
at the first model commit, does not arise here. The readings at `be6c4a5c` and at `6c61879b`, and the first one's flag,
in the notes above of 2026-11-24 and 2026-11-25, stay as written. A NOT MET at any later model commit makes part B NOT
MET." That is Broiler.JS's reading of its own bundle, not a run of this bundle's rule, and judges nothing here.
Broiler.JS's note of 2026-11-25 at stage 3 also reads that its library-level adapter answers, by its decision
D-js-adapter-groups, the vectors of "correspondence group `lb1`, row LB1, which rest on EE0082 and EE0166: the
acceptances that show hand-off H20 read by the specification", each "read at the parse, the reading the owner ruled on
2026-11-25 to keep ("Keep the reading at the parse (Recommended)")"; that adapter, and the part F of Broiler.JS's rule
it answers for, "every vector the adapter answers matches", are Broiler.JS's, and this bundle's rule has none like them.
The places where the library differs from this family's binder, which Broiler.JS's plan names for
SJL-15.3, are quoted in this bundle's correspondence, its note of 2026-11-25, later yet, on rows LB1 and EE1. The rest
of the model is still to come, from "**Next:** SJL-15.2 stage 4, private-name environments, the fourth model commit", by
Broiler.JS's decision D-js-stages, each stage with part B at its merge. The convention of the note of 2026-11-25 holds
for this merge too: this bundle's list and this repository's ledger observe each model commit after it merges, and that
is no precondition of a model merge. The fourth and fifth items, the owner's publication and this family's tools commit
and pin commit, still wait on the model. No other item changes.

*(Noted 2026-11-26, at Broiler.JS's SJL-15.2 stage 4 and its part B, observed; the list and the notes above are left as
written.)* The note of 2026-11-25, later yet, above reads "The rest of the model is still to come". SJL-15.2's stage 4,
"private-name environments", merged as Broiler-Platform/Broiler.JS#1139, and Broiler.JS's bundle `sjl-15-001`'s README,
having read before the merge that "This branch's merge will be the fourth model commit", names it after: "**The fourth
model commit is `56272150fe23725a0e70978fb18e3c25f2548548`**, the merge of Broiler-Platform/Broiler.JS#1139 (head
`2c502843`), whose first parent is `9ae6aee09012d6b7139856f253b43f7701cf7b8b` (Broiler-Platform/Broiler.JS#1138, stage
3's part B records)", and "Its first parent is no model commit: nothing from `d87044c9` to `9ae6aee0` changes a path
under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that bundle's rule was taken at it and at its first
parent, and read in Broiler-Platform/Broiler.JS#1140, `746591d22142a25dd68c2a9d132930b1008cd9dd`, records only, taken as
recommended under the owner's direction: "**part B read MET on the retained runs at the model commits taken so far
(`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`, `6c61879bce81b1a7badf987e16c7b0531030258d`,
`d87044c9db5fe2a26841782308bd53e2f9294656`, `56272150fe23725a0e70978fb18e3c25f2548548`), limited to them; the part's
verdict waits for the last model commit before the base.**" "At `56272150` the rule's "the commit's CI run of every test
project passed" is read on both workflows' push runs as they concluded, each in its first attempt, so the question of
which attempt decides, flagged for the owner at the first model commit, does not arise here. The readings at `be6c4a5c`,
at `6c61879b` and at `d87044c9`, and the first one's flag, in the notes above of 2026-11-24 and 2026-11-25, stay as
written. A NOT MET at any later model commit makes part B NOT MET." That is Broiler.JS's reading of its own bundle, not
a run of this bundle's rule, and judges nothing here. Broiler.JS's note of 2026-11-25 at stage 4 also reads that its
library-level adapter "also answers the vectors of stage 4's private-name rows, EE0167 and EA0009 (EE0177 has none),
each read at the parse, the reading the owner ruled on 2026-11-25 to keep ("Keep the reading at the parse
(Recommended)")", that "a `DirectEval` vector's eval code now inherits the private names visible at the call, read from
its Script's model", and that "correspondence group `pn1` is not implemented: `pn1/0004` and `pn1/0005` expect a value
the Script computes after catching the eval code's refusal, which a reading at the parse cannot match"; that adapter,
and the part F of Broiler.JS's rule it answers for, "every vector the adapter answers matches", are Broiler.JS's, and
this bundle's rule has none like them. The places where the library differs from this family's binder and executor,
which Broiler.JS's plan names for SJL-15.3, are quoted in this bundle's correspondence, its note of 2026-11-26, on rows
PN1 and EE1. The rest of the model is still to come, from "**Next:** SJL-15.2 stage 5a, resolution and the function
environment facts, the fifth model commit", by Broiler.JS's decision D-js-stages, each stage with part B at its merge.
The convention of the note of 2026-11-25 holds for this merge too: this bundle's list and this repository's ledger
observe each model commit after it merges, and that is no precondition of a model merge. The fourth and fifth items, the
owner's publication and this family's tools commit and pin commit, still wait on the model. No other item changes.

*(Noted 2026-11-26, later, at Broiler.JS's SJL-15.2 stage 5a and its part B, observed; the list and the notes above are
left as written.)* The note of 2026-11-26 above reads "The rest of the model is still to come". SJL-15.2's stage 5a,
"resolution and the function environment facts", merged as Broiler-Platform/Broiler.JS#1141, and Broiler.JS's bundle
`sjl-15-001`'s README, having read before the merge that "This branch's merge will be the fifth model commit", names it
after: "**The fifth model commit is `4d8aa93ad5023017b66534feb248c4c16f59bf6e`**, the merge of
Broiler-Platform/Broiler.JS#1141 (head `48a987ca`), whose first parent is `746591d22142a25dd68c2a9d132930b1008cd9dd`
(Broiler-Platform/Broiler.JS#1140, stage 4's part B records)", and "Its first parent is no model commit: nothing from
`56272150` to `746591d2` changes a path under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that
bundle's rule was taken at it and at its first parent, and read in Broiler-Platform/Broiler.JS#1142,
`d3e64b4df2c35acf706fca91707c3f198b53e6cd`, records only, taken as recommended under the owner's direction: "**part B
read MET on the retained runs at the model commits taken so far (`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`,
`6c61879bce81b1a7badf987e16c7b0531030258d`, `d87044c9db5fe2a26841782308bd53e2f9294656`,
`56272150fe23725a0e70978fb18e3c25f2548548`, `4d8aa93ad5023017b66534feb248c4c16f59bf6e`), limited to them; the part's
verdict waits for the last model commit before the base.**" "At `4d8aa93a` the rule's "the commit's CI run of every test
project passed" is read on both workflows' push runs as they concluded, each in its first attempt, so the question of
which attempt decides, flagged for the owner at the first model commit, does not arise here. The readings at `be6c4a5c`,
at `6c61879b`, at `d87044c9` and at `56272150`, and the first one's flag, in the notes above of 2026-11-24 and
2026-11-25, stay as written. A NOT MET at any later model commit makes part B NOT MET." That is Broiler.JS's reading of
its own bundle, not a run of this bundle's rule, and judges nothing here. Broiler.JS's note of 2026-11-26 at stage 5a
also reads that its library-level adapter "answers no new vector: by decision D-js-environment-vectors the value vectors
of correspondence groups `ff3`, `ff4`, `ff5` and `rs2` stay not implemented, since they name no row and read at the
parse, the reading the owner ruled on 2026-11-25 to keep ("Keep the reading at the parse (Recommended)"), as the note
above at SJL-15.2 stage 2 records it, each would match by acceptance alone", and "As a proposal, the model's FF4 and FF5
facts (`FunctionFacts.HasSeparateVariableEnvironment`, `EnvironmentFacts.DirectEvalInParameters` and `DirectEvalInBody`)
are the specification's answers that the corrections C1's rows FF4 and FF5 name (`corrections/corrections.tsv`);
SJL-15.5 applies them, and `corrections.md` and its tables are unchanged." That adapter, and the part F of Broiler.JS's
rule it answers for, "every vector the adapter answers matches", are Broiler.JS's, and this bundle's rule has none like
them; the corrections those rows FF4 and FF5 name are Broiler.JS's, as this bundle's correspondence reads the two rows'
column "Broiler.JS", "Correction of Broiler.JS (V010)" and "Correction of Broiler.JS (V015)". The members that answer
this bundle's correspondence rows FF3, FF4 and FF5, which Broiler.JS's plan names for this family's class P listing, and
the places where the library differs from this family's binder, which it names for SJL-15.3, are quoted in this bundle's
correspondence, its note of 2026-11-26, later, on rows FF3, FF4, FF5, RS1 to RS4 and EE1. The rest of the model is still
to come, from "**Next:** SJL-15.2 stage 5b, Annex B block-function aliases, the sixth model commit", by Broiler.JS's
decision D-js-stages, each stage with part B at its merge. The convention of the note of 2026-11-25 holds for this merge
too: this bundle's list and this repository's ledger observe each model commit after it merges, and that is no
precondition of a model merge. The fourth and fifth items, the owner's publication and this family's tools commit and
pin commit, still wait on the model. No other item changes.

*(Noted 2026-11-26, later yet, at Broiler.JS's SJL-15.2 stage 5b and its part B, observed; the list and the notes above
are left as written.)* The note of 2026-11-26, later, above reads "The rest of the model is still to come". SJL-15.2's
stage 5b, "Annex B's block-function aliases", merged as Broiler-Platform/Broiler.JS#1143, and Broiler.JS's bundle
`sjl-15-001`'s README, having read before the merge that "This branch's merge will be the sixth model commit", names it
after: "**The sixth model commit is `84d84cc3f68e3120299eb9faa44c6b9f0f2e7b8b`**, the merge of
Broiler-Platform/Broiler.JS#1143 (head `33323754`), whose first parent is `d3e64b4df2c35acf706fca91707c3f198b53e6cd`
(Broiler-Platform/Broiler.JS#1142, stage 5a's part B records)", and "Its first parent is no model commit: nothing from
`4d8aa93a` to `d3e64b4d` changes a path under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that
bundle's rule was taken at it and at its first parent, and read in Broiler-Platform/Broiler.JS#1144,
`4e27f13d3dc1bd3c38c5f29ea28ff4391ec8771b`, records only, taken as recommended under the owner's direction: "**part B
read MET on the retained runs at the model commits taken so far (`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`,
`6c61879bce81b1a7badf987e16c7b0531030258d`, `d87044c9db5fe2a26841782308bd53e2f9294656`,
`56272150fe23725a0e70978fb18e3c25f2548548`, `4d8aa93ad5023017b66534feb248c4c16f59bf6e`,
`84d84cc3f68e3120299eb9faa44c6b9f0f2e7b8b`), limited to them; the part's verdict waits for the last model commit before
the base.**" "At `84d84cc3` the rule's "the commit's CI run of every test project passed" is read on both workflows'
push runs as they concluded, each in its first attempt, so the question of which attempt decides, flagged for the owner
at the first model commit, does not arise here. The readings at `be6c4a5c`, at `6c61879b`, at `d87044c9`, at `56272150`
and at `4d8aa93a`, and the first one's flag, in the notes above of 2026-11-24, 2026-11-25 and 2026-11-26, stay as
written. A NOT MET at any later model commit makes part B NOT MET." That is Broiler.JS's reading of its own bundle, not
a run of this bundle's rule, and judges nothing here. Broiler.JS's note of 2026-11-26 at stage 5b also reads that its
library-level adapter "answers no new vector: by decision D-js-annexb-vectors the value vectors of correspondence group
`ab1` stay not implemented, since they rest on no `semantics` row and read at the parse, the reading the owner ruled on
2026-11-25 to keep ("Keep the reading at the parse (Recommended)"), as the note above at SJL-15.2 stage 2 records it,
each would match by acceptance alone", and "Row AB1 has no row in the corrections' C1: Broiler.JS gives the
specification's answers on the probes ([...], row AB1), and `corrections.md` and its tables are unchanged." That
adapter, and the part F of Broiler.JS's rule it answers for, "every vector the adapter answers matches", are
Broiler.JS's, and this bundle's rule has none like them; the corrections' C1 is Broiler.JS's, and this bundle's
correspondence reads row AB1's column "Broiler.JS", across both engines, "Correction of Broiler.VM for V028" and "in
eval code for V029 (an intermediate `let`) and V030 (strict eval code)", which this bundle's rule lists at window 1 by
JSD-0088's decision D-ab1, in part A's class ED and part B's class F, as the note of 2026-11-23 above records.
Broiler.JS's plan reads "Broiler.VM's decision D-ab1 (JSD-0088) is Broiler.VM's, not among the decisions that bind this
repository above, and is cited, not taken, here." The places where it names the library differing from this family's
binder for row AB1, on the inputs decision D-ab1 names, and its reading that "The model meets each case D-ab1 names", a
reading that ends "(an inference)", are quoted in this bundle's correspondence, its note of 2026-11-26, later yet, on
rows AB1 and EE1. The rest of the model is still to come, from "**Next:** SJL-15.2 stage 5c, the caller's names in the
inherited context, the seventh model commit", by Broiler.JS's decision D-js-stages, each stage with part B at its merge.
The convention of the note of 2026-11-25 holds for this merge too: this bundle's list and this repository's ledger
observe each model commit after it merges, and that is no precondition of a model merge. The fourth and fifth items, the
owner's publication and this family's tools commit and pin commit, still wait on the model. No other item changes.

*(Noted 2026-11-27, at Broiler.JS's SJL-15.2 stage 5c and its part B, observed; the list and the notes above are left as
written.)* The note of 2026-11-26, later yet, above reads "The rest of the model is still to come". SJL-15.2's stage 5c,
the caller's names in the inherited context, merged as Broiler-Platform/Broiler.JS#1145, and Broiler.JS's bundle
`sjl-15-001`'s README, having read before the merge that "This branch's merge will be the seventh model commit", names
it after: "**The seventh model commit is `30af0dad83f73d2a5cf5495b2d4e3716cc7ed0df`**, the merge of
Broiler-Platform/Broiler.JS#1145 (head `628f4b38`), whose first parent is `4e27f13d3dc1bd3c38c5f29ea28ff4391ec8771b`
(Broiler-Platform/Broiler.JS#1144, stage 5b's part B records)", and "Its first parent is no model commit: nothing from
`84d84cc3` to `4e27f13d` changes a path under `Broiler.ECMAScript/Broiler.ECMAScript.Frontend*`." Part B of that
bundle's rule was taken at it and at its first parent, and read in Broiler-Platform/Broiler.JS#1146,
`59ccb11cc1279fe2a6f015acbccd7f6dbff185f2`, records only, taken as recommended under the owner's direction: "**part B
read MET on the retained runs at the model commits taken so far (`be6c4a5c3e389c7eb2aff71c34e31bdae5efe276`,
`6c61879bce81b1a7badf987e16c7b0531030258d`, `d87044c9db5fe2a26841782308bd53e2f9294656`,
`56272150fe23725a0e70978fb18e3c25f2548548`, `4d8aa93ad5023017b66534feb248c4c16f59bf6e`,
`84d84cc3f68e3120299eb9faa44c6b9f0f2e7b8b`, `30af0dad83f73d2a5cf5495b2d4e3716cc7ed0df`), limited to them; the part's
verdict waits for the last model commit before the base.**" "At `30af0dad` the rule's "the commit's CI run of every test
project passed" is read on both workflows' push runs as they concluded, each in its first attempt, so the question of
which attempt decides, flagged for the owner at the first model commit, does not arise here. The readings at `be6c4a5c`,
at `6c61879b`, at `d87044c9`, at `56272150`, at `4d8aa93a` and at `84d84cc3`, and the first one's flag, in the notes
above of 2026-11-24, 2026-11-25 and 2026-11-26, stay as written. A NOT MET at any later model commit makes part B NOT
MET." That is Broiler.JS's reading of its own bundle, not a run of this bundle's rule, and judges nothing here.
Broiler.JS's note of 2026-11-26 at stage 5c also reads that its library-level adapter "newly answers the vectors of
EA0007, read at the parse, the reading the owner ruled on 2026-11-25 to keep ("Keep the reading at the parse
(Recommended)"), as the note above at SJL-15.2 stage 2 records it, the nested parse of `ea0007/0005` included: a refusal
at any level is the answer", that "The vectors of EA0006 stay not implemented, and no EA0008 vector exists", and that
"EA0007's rows in the corrections, C1's ([...]) and E's and T's ([...]), are unchanged, and `corrections.md` and its
tables are unchanged." That adapter, and the part F of Broiler.JS's rule it answers for, "every vector the adapter
answers matches", are Broiler.JS's, and this bundle's rule has none like them; the corrections' C1 is Broiler.JS's, and
this bundle's correspondence reads row DE3's column "Broiler.JS", across both engines, "Same, but a correction of
Broiler.JS for V046 (C1's row EA0007, in eval code)", the vector `semantics/scopes/ea0007/0005` holding V046's input, as
the note of 2026-11-24, later, there records. Broiler.JS's plan reads "`CallerScopes` is the member Broiler.VM's
decision D16 leaves until SJL-15's model", D16 being JSD-0083's decision D16, and "D16, D44 and JSD-0080 are
Broiler.VM's and left as written"; that reading, and the places where it names the library differing from this family's
executor for the caller's records, among them that "Broiler.VM checks when the eval runs, through its eval map", several
of them inferences, are quoted in this bundle's correspondence, its note of 2026-11-27 on rows DE3 and EE1. The rest of
the model is still to come, from "**Next:** SJL-15.2 stage 6, the module records (correspondence rows MR1 and MR2,
hand-off H19), the eighth model commit", by Broiler.JS's decision D-js-stages, each stage with part B at its merge. The
convention of the note of 2026-11-25 holds for this merge too: this bundle's list and this repository's ledger observe
each model commit after it merges, and that is no precondition of a model merge. The fourth and fifth items, the owner's
publication and this family's tools commit and pin commit, still wait on the model. No other item changes.
