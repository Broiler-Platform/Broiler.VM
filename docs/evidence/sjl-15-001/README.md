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
