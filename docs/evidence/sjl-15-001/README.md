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

**Correspondence:** [`correspondence.md`](correspondence.md), this family's half, read from code at Broiler.VM
`a86b6f5d` and against the model surface at Broiler.JS `6fda6ca4`. Broiler.JS's half fills its column by a dated note in
Broiler.JS's own change for SJL-15.1.

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
  correspondence;
- SJL-15.2's model in Broiler.JS, each merge before Broiler.JS's SJL-14 cut-over a dark window of SJL-14 under its
  decision D52, and none during SJL-11.5's hold;
- the owner's publication of a stream version carrying the model (JSD-0083's decision D30);
- this family's tools commit and pin commit;
- the owner's answer to JSD-0088's decision D-census, before any run of part S;
- SJL-15.3's and SJL-15.4's windows, outside SJL-11.4's window, and then this bundle's runs at each base and after commit.
