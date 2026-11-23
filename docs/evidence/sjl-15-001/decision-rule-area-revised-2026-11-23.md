<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-15-001's area rule, revised on 2026-11-23 by decision D-ab1: row AB1 listed at window 1, in part A by a class of its own, ED, and in part B's class F

**Written:** 2026-11-23, before any run this rule judges and before any code it judges exists. It is filed on the
branch `vm-ab1-2026-11-23`, before that branch merges. The rule and its two revisions of 2026-11-16 are on the default
branch: the rule and the revision of that morning through the merge `0ddc146f` (Broiler-Platform/Broiler.VM#214), the
revision of that day's answer to decision D-census through the merge `626e4ed8` (Broiler-Platform/Broiler.VM#215). No
tools commit, pin commit, base or after commit of either window exists, and no run of any part of this rule has been
taken. This file revises [`decision-rule-area.md`](decision-rule-area.md) as that file's section "Revisions" requires
("A revision filed before any run of the part it revises, the owner's answer to decision D-census among them, quotes
this file whole and says so"): it is a new dated file, it quotes that file whole at its end, and that file is not
edited. The revisions [`decision-rule-area-revised-2026-11-16.md`](decision-rule-area-revised-2026-11-16.md) and
[`decision-rule-area-revised-2026-11-16b.md`](decision-rule-area-revised-2026-11-16b.md) stand as written: this file
changes no line either of them wrote, and the lines it changes are lines of the rule that neither revision changed,
each quoted below. As the revision of 2026-11-16b named the rule commit "the commit that carries all three files",
**the rule commit, as the rule names it, is from this revision on the commit that carries all four files**, so that
the tools commit and every base, which must descend from the rule commit, descend from this revision too. In this
file, "this revision" is this file.
**Owner:** the JavaScript profile owner, with the core architecture owner, as for the rule this file revises.
**Reviewer:** none; one person holds every role (EX-30), and this revision does not claim that anyone independent has
read it.

**Whose choice it is.** This revision files decision D-ab1, recorded in
[JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md),
section "The owner's decisions of 2026-11-23", as a decision of 2026-11-23 taken as recommended under the owner's
direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review"), with its alternatives not
taken, and not as the owner's own dated answer. JSD-0088 names three kinds of entry not taken under that direction (the
start, decision D-census and each publication of the shared stream); this is none of them. **It is the session's
decision, and the owner may overturn it** by a further dated file in this directory, filed before any run of part A or
part B; until then this revision stands.

**What was seen, and where.** Read at Broiler.VM `a1d881b7` and, for Broiler.JS's records, at Broiler.JS's default
branch `69a8a13b`, read-only:
1. **Row AB1 is a correction of this family that the rule does not list.** The correspondence's row AB1 (Annex B.3.3
   block-function aliases) reads, across both engines, "Correction of Broiler.VM for V028 (a block function named
   `arguments`, the pinned edition against test262 at `ccaac100`, which the Script vector
   `semantics/annex-b/ab1/0005` also shows), and in eval code for V029 (an intermediate `let`) and V030 (strict eval
   code)", and its last cell "SJL-15.2 adds the alias decision by declaration; SJL-15.3 reads it, and `ScanAnnexB`
   goes". The programme ledger's entry of 2026-11-17 reads that "Across both engines it is a correction of Broiler.VM,
   which SJL-15.3 answers, and bundle `sjl-15-001`'s rule does not list the row among this family's corrections", and
   ends "where row AB1 stands in the rule is not decided here". Broiler.JS's `scripts/conformance/semantics-divergences.txt`
   gives its reason `vm-ab1` the words "the row is not yet among the corrections its bundle sjl-15-001's rule lists by
   row".
2. **Its test262 move is from pass to fail.** The ledger's entry of 2026-11-17 reads: "Test262's
   `annexB/language/function-code/block-decl-func-skip-arguments.js` at `ccaac100` asserts this family's answer, an
   earlier edition's reading; the vector follows the pinned edition." That file is in part A's population and in the
   directories part H names (`test/annexB/language`). So a window 1 that follows the pinned edition for row AB1 moves
   that file's verdict from pass to fail. Part A reads "every other difference is a regression, a verdict that moves
   from pass to fail among them", and its class C1 admits only "a variant whose verdict moves from fail to pass", and
   only "for the reason of one of the rows LB1 (H20), MR2 (H19) or the dynamic function's goals"; template T1 of the
   programme's validation document admits no pass-to-fail move outside class C2 ("no variant passing at base fails, is
   exhausted or is unsupported after, except under C2"), and this rule admits no C2.
3. **Its part B move is a byte difference of an accepted input.** Row AB1 changes what a block function's declaration
   does when it is evaluated, not whether the input is accepted: an input of the parse corpus that reaches it is
   accepted at both commits, and its artifact differs. Part B's class F admits "an input whose outcome or first
   diagnostic moves for that row's reason", for LB1, MR2 and the dynamic function's goals only, and part B reads "any
   byte difference of an input accepted at both commits outside P" as a regression.
4. **JSD-0088's falsifier reads such a move as falsifying it.** Its "Falsified if" reads "An artifact of window 1 that
   moves for a reason class P, F, T or S7 does not name."

## What changed

1. **"What changes, named before any run", the window-1 bullet, its sub-bullet "Precise facts".** Its last sentence,
   "Accepted inputs' artifacts move for these reasons only, by part B's class P.", now reads "Accepted inputs' artifacts
   move for these reasons only, by part B's class P, and for row AB1's, by part B's class F, as the next sub-bullet
   lists it."
2. **The same bullet, its sub-bullet "Corrections of this family".** It reads: "**Corrections of this family** (part A's
   and part B's class F), each by row: LB1 (`continue` to an outer label of a loop's label set, hand-off H20), MR2 (an
   `await` in an async arrow at a module's top level, H19), and, by JSD-0088's decision D-handoffs, the dynamic
   function's goals (`sjl-13-001`'s P26 and D61, H18)." It now reads: "**Corrections of this family** (part A's class C1
   and part B's class F), each by row: LB1 (`continue` to an outer label of a loop's label set, hand-off H20), MR2 (an
   `await` in an async arrow at a module's top level, H19), and, by JSD-0088's decision D-handoffs, the dynamic
   function's goals (`sjl-13-001`'s P26 and D61, H18); and, by JSD-0088's decision D-ab1, row AB1 (Annex B.3.3
   block-function aliases, read from the model's alias decision as the pinned edition gives it: a block function named
   `arguments`, whose arguments binding the base keeps, and in eval code a block function under an intermediate lexical
   declaration of its name and a block function in strict eval code, which the base aliases), by part A's class ED and
   part B's class F, because test262's `annexB/language/function-code/block-decl-func-skip-arguments.js` at the pin
   asserts the base's answer, an earlier edition's reading."
3. **Part A, its rule block.** After the line that begins "C1 correction of this family (window 1 only)", which is
   unchanged, a line is added:

   ```rule
     ED the pinned edition against the pinned suite (window 1 only, row AB1, JSD-0088's decision D-ab1) - (i) a variant of test/annexB/language/function-code/block-decl-func-skip-arguments.js whose verdict moves from pass to fail because the lowering reads the model's Annex B.3.3 alias decision, which follows the pinned edition for a block function named arguments, where the file at the pin asserts the base's answer, an earlier edition's reading; and (ii) any other variant whose verdict moves, from fail to pass or from pass to fail, because that alias decision gives the pinned edition's answer where the base's ScanAnnexB did not (a block function named arguments; in eval code, a block function under an intermediate lexical declaration of its name, or in strict eval code), a move from pass to fail only where the variant asserts the base's answer against the pinned edition's text; each variant listed with the row and its move, and each move from pass to fail with the assertion the variant makes and the pinned edition's text it contradicts
   ```

   Its line "every other difference is a regression, a verdict that moves from pass to fail among them" now reads
   "every other difference is a regression, a verdict that moves from pass to fail outside class ED among them", and its
   MET line "MET if and only if all four runs of each window are complete, and every changed verdict of either pair is
   in C1 or C4 and is listed." now reads "MET if and only if all four runs of each window are complete, and every
   changed verdict of either pair is in C1, ED or C4 and is listed." The lines "no C1 at window 2: the conformance root
   compiles nothing through the slice path" and "no C2, no C3 and no C5" are unchanged; class ED, like C1, admits
   nothing at window 2.
4. **Part B, its rule block, class F.** The line "F a correction of this family by one of the rows LB1, MR2 and the
   dynamic function's goals (H20, H19, H18) - an input whose outcome or first diagnostic moves for that row's reason;
   each listed with its row" now reads:

   ```rule
     F a correction of this family by one of the rows LB1, MR2 and the dynamic function's goals (H20, H19, H18) - an input whose outcome or first diagnostic moves for that row's reason; each listed with its row; and by row AB1 (JSD-0088's decision D-ab1) - an input accepted at both commits, the same outcome, whose artifact differs only because the lowering reads the model's Annex B.3.3 alias decision, which follows the pinned edition where the base's ScanAnnexB did not (a block function named arguments, whose arguments binding the base keeps; in eval code, a block function under an intermediate lexical declaration of its name, or in strict eval code, which the base aliases); each listed with the row, the function's place and the decoded difference (the instructions and records that move), as class P's are, and the input's verdict in part A unchanged or in part A's class ED where it is a test262 input
   ```

   Its regression line's first clause, "any byte difference of an input accepted at both commits outside P", now reads
   "any byte difference of an input accepted at both commits outside P and class F's row AB1"; the rest of that line is
   unchanged.

## How it reads

- **Row AB1 is listed before any run, at window 1 only, and by row.** A difference is admitted only where its variant or
  input is listed with row AB1 and the difference is of the class named: in part A, class ED; in part B, class F's row
  AB1. A move for any other reason, or of a variant or input not listed, is judged as the rule judged it.
- **How class ED differs from class C1.** C1 is template T1's "Correction", "The verdict improves because the shared
  implementation follows the specification where the engine did not": it admits a move from fail to pass alone, for the
  rows it names. Class ED is not C1 widened. Its part (i) admits, for the variants of one named file, a move from
  **pass to fail**, a verdict that worsens, because the pinned suite asserts an earlier edition's reading and the
  window follows the pinned edition; template T1 admits no such move outside C2, so for those listed variants this rule
  departs from the template, named here before any run. Its part (ii) admits, for row AB1's reason alone, a move in
  either direction, a move from pass to fail only for a variant that, like the named file, asserts the base's answer
  against the pinned edition's text, quoted with it. No other row gains a pass-to-fail class, and C1's words are
  unchanged.
- **Part B reads row AB1 as a byte difference.** Row AB1 moves no outcome and no diagnostic, so class F's row AB1 is
  bounded as class P is, by the decoded difference, and a difference row AB1 names that the decoded bytes do not show is
  a regression, as the rule's "How it is read" says of class P.
- **What follows in the parts this revision does not change.** Part H's test262 files "admit part A's classes only",
  so the named file's move is part H's only through class ED; a `--checks` row part H names that the window's merge
  revises for row AB1 is listed "where part B's classes P, F, T and S7 name the change", class F now including row AB1.
  Part E's class C1 already admits the vector `semantics/annex-b/ab1/0005`, "a vector whose verdict is difference or
  classified-divergence at the base and match after, listed with its row of the correspondence", and is unchanged. Part
  G's twins and part J's workloads name class P alone and are unchanged: a twin or a workload whose artifact moves for
  row AB1 is read under those parts as written.
- **The eval-code inputs.** The eval-code inputs of probes V029 and V030 are in no `semantics` vector yet, as
  Broiler.JS's records on its default branch read them, and this revision names no vector. Part A's and part B's
  listings are made from the runs, as every class of this rule is listed.

## What this revision does not change

Every other line of the rule and of its revisions of 2026-11-16: every other part, class and control; the commits and
their conditions, but for which commit is the rule commit, which "Written" above names; the local machine; part A's
population, configuration, completeness and comparison, its classes C1 and C4, and its lines "no C1 at window 2" and
"no C2, no C3 and no C5"; part B's population, configuration, completeness, controls and comparison, its classes P, T
and S7, its window-2 and B4 lines, its ordering line and the rest of its regression line; parts S, C, D, E, F, G, H, I
and J; window 2; "What this file does not decide"; JSD-0088's deletion lists and its other decisions.

**Not taken**, each JSD-0088's alternative to decision D-ab1:
- **(b) SJL-15.3 keeps Broiler.VM's current answer for `semantics/annex-b/ab1/0005`**, so that row AB1 stays a known
  divergence, the rule unrevised, the named file passing at both commits. The correspondence classes the row a
  correction of Broiler.VM across both engines, and its last cell has SJL-15.3 read the model's alias decision with
  `ScanAnnexB` gone; keeping the base's answer would need the lowering to compute the alias against the model's fact,
  a second computation of a model fact, which decision D-precision rejects for the binder's approximations.
- **(c) The rule left as it is.** A move of row AB1 at window 1 is then a regression of part A (the named file from pass
  to fail) and of part B (an accepted input's bytes outside P), so either part is NOT MET for the correction the
  correspondence names, and JSD-0088's falsifier, "An artifact of window 1 that moves for a reason class P, F, T or S7
  does not name", holds; a revision written after the runs were seen would then be weaker evidence.
- **Stretching class C1 to row AB1.** C1 admits a verdict that improves; the named file's verdict worsens, and reading
  C1 to admit it would change what C1 means for the rows it already lists.

## The rule it revises, quoted whole

The quote below is [`decision-rule-area.md`](decision-rule-area.md) at blob
`2ddc5203f61bf0a470e51f53fc7917990c1f3f26`, as the Broiler.VM commit that files this revision holds it, each line
prefixed with `> ` (an empty line with `>`), as the revisions of 2026-11-16 quote it. Those revisions, at blobs
`6fac0ec7d68b647b94b36415b4b6ee3df8547952` and `b8da705a25a7226aecb4bd4063c61ceea293b303`, are not quoted whole: they
stand as written beside this file, and this file replaces no line of either.

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-15-001's area rule: this family on the shared semantic model, SJL-15.3's and SJL-15.4's windows, judged by test262 parity in both forms on hosted runners, by artifact identity over the parse corpus, by the slice corpus byte for byte, by determinism, by the front end's rules, by the `semantics` vectors, by the closure-sensitive suites, by source fuzzing, by the fuel-parity twins, by publication in three modes, and by this family's measurement
>
> **Written:** 2026-11-16, at milestone SJL-15 (work package SJL-15.1), with the area verdict
> [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md)
> and this bundle's [`correspondence.md`](correspondence.md), before any change it judges exists: no semantic model beyond
> the declarations walk SJL-13.2 began, no stream version carrying one, no tools commit, no cut-over and no deletion.
> **Milestone judged:** SJL-15 of
> [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-15-static-semantics.md),
> its Broiler.VM clauses: 4 ("The binder pass is gone, and the lowering reads the model."), 5 ("The slice path is rebuilt
> on the shared model: `SliceParser` is gone, and the slice corpus's census counts, slot order and refusals are
> byte-identical before and after."), 6 ("Artifacts are byte-identical, or every difference is classified."), 7 ("test262
> in both forms meets the rule."), 8 ("Rules N5 to N9, N12, N16 and N19 hold.") and 9 ("Every root is published and run in
> three modes."); and SJL-15.6's items for this family: determinism in two runs and on two threads, fuzzing, three modes,
> and this family's own measurement. **Owner:** the JavaScript profile owner, with the core architecture owner.
> **Reviewer:** none. Every role this rule names is held by one person (EX-30), and this rule does not claim that anyone
> independent has read it.
>
> **What this file is.** Eleven predeclared decisions, one per part, each of one evidence class.
> - **The templates.** Part A is template T1 of
>   [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
>   part B T6, part S T6 over the slice corpus, part E T3, part G T7 and part J T5. Part C is layer L13's determinism,
>   part D the rule runs, part F layer L9, part H the closure-sensitive suites and part I the publication.
> - **Each part decides alone**, and no part reads another's verdict.
> - **Part J is the only measurement. It is a rule part, not a gate clause** (JSD-0088's decision D-measure, as SJL-13
>   read its own, the milestone's note of 2026-10-26 under SJL-13.6): SJL-15 has no measurement gate clause, and part J
>   decides no clause.
> - **The correspondence this rule classifies by** is [`correspondence.md`](correspondence.md) beside it, its rows named
>   by their letters (SC, DE, AB, FF, RS, LB, PN, MR, NE, FC, EE, SL). A row of bundle `sjl-13-001`'s correspondence is
>   named with that bundle.
> - **It holds no figure.** Where a count matters, it names the file that fixes it.
>
> **SJL-13's and SJL-14's rules, applied to the semantic half.** The milestone's SJL-15.1 says "The rules are those of
> SJL-13 and SJL-14, applied to the semantic half". This rule reads that so:
> - **From bundles `sjl-13-001` and `sjl-13-002`** (this family's): part A's population, configuration, comparison and C4
>   classes; part B's tool, sub-populations B1 to B5, control and reading of refused inputs; part C's determinism and its
>   controls; part D's rule runs; part G's seeds and both fuzz sessions; part H's engine-level vector runner; part I's
>   fuel-parity twins; part J's workloads and measurement; each applied at SJL-15.3's and SJL-15.4's windows, and each
>   named below where it applies.
> - **From Broiler.JS's bundle `sjl-14-001`** (observed, merged in Broiler-Platform/Broiler.JS#1100 as `d8f14602`): class T
>   (decision D64: "an error raised before evaluation that the base raised later, or not at all"), which D64 takes "at
>   SJL-15 for the rest", is applied to this family's half for the rows the correspondence names (row PN1). Class E (an
>   early error moving from a compiler stage to the parse) has no subject here: this family's early errors are raised by
>   one stage before any of a unit is lowered (`sjl-13-001`'s correspondence, the binder's B rows). Decision D52's dark
>   windows bind merges to the library in Broiler.JS, not this family, which takes the library only through a published
>   stream version it admits (JSD-0083's decision D30).
> - **Added by the milestone:** the slice corpus's census counts, slot order and refusals, byte-identical before and after
>   (part S), and the closure-sensitive suites (part H): this family's, and, for Broiler.JS, its own rule's.
>
> **What changes, named before any run.**
> - **SJL-15.3's window** (JSD-0088's decision D-windows): the lowering reads the model; `JsBinder`, in `JsBinder.cs`,
>   `JsBinder.Walks.cs` and `JsBinder.EarlyErrors.cs`, is deleted and registered in rule I5 member by member.
>   - **Precise facts** (correspondence rows FF3, FF4 and FF5): the `arguments` object is materialised only where the
>     model says `arguments` is needed; a function body gets a variable environment of its own where the model says so; a
>     function is contaminated by direct `eval` only where the model says so. Accepted inputs' artifacts move for these
>     reasons only, by part B's class P.
>   - **Corrections of this family** (part A's and part B's class F), each by row: LB1 (`continue` to an outer label of a
>     loop's label set, hand-off H20), MR2 (an `await` in an async arrow at a module's top level, H19), and, by JSD-0088's
>     decision D-handoffs, the dynamic function's goals (`sjl-13-001`'s P26 and D61, H18).
>   - **Timing** (class T, decision D64's second half): eval code's undeclared private name is refused when the eval code
>     is compiled, from the visible private names the request then carries, where the base's executor checked it (row PN1).
>   - **What does not change:** every other outcome, every other accepted input's bytes, and the code, message and place
>     of every early error the binder raised at the base, but where the library's first refusal now stands for the
>     binder's list (class S7, as `sjl-13-001`'s), and the slice path.
> - **SJL-15.4's window:** the slice path parses with the shared library; `SliceParser` and `SliceTokenizer` are deleted
>   (decision D60) and so are `SliceSyntax.cs`'s records (decision D-slicetree); `SliceStaticSemantics` reads the model,
>   its refusal of unresolvable identifiers and of an assignment to a constant becoming admission rules over the model's
>   resolution (rows RS3, RS4); the slice lowering keeps its slot allocation, in the base's order (row SL1). **The slice
>   corpus is expected byte-identical** (part S), and **the wide path does not change** (part B admits nothing at this
>   window outside B4).
>   - **The inputs the correspondence expects to differ** are those that reach hand-offs H23 and H25 (row SL4): the slice
>   tokenizer's escape digits, its regular-expression body ending in a backslash, and a strict directive with `\8` or
>   `\08`, the parse corpus's `J000262` and `J001009` among them, as JSD-0083 names them. **Clause 5 as written admits no
>   difference**, and part S admits none. What clause 5 means for them is put to the owner (JSD-0088's decision
>   D-census): until the owner answers, any of them that differs makes part S NOT MET, and the owner's answer, if it
>   revises clause 5, is filed with a new dated rule file in this directory before any run of part S.
>
> **The local machine** is the one bundle `sjl-13-002`'s local runs were taken on, as that bundle names it by processor
> count, SDK and operating system, with no other build, test or run on it while a run of this bundle is taken. Another
> machine counts only by a reading recorded in this bundle before its runs, which the owner may overturn, as bundle
> `sjl-13-001`'s machine reading was ruled on 2026-11-12.
>
> **The commits, named before they exist.**
> - **The rule commit** carries this file.
> - **The tools commit** is the default-branch merge that adds to the slice compiler's `--parse-corpus` mode, for
>   sub-population B4, each refused input's diagnostic messages beside their codes and places, and each accepted input's
>   binding table (each binding's name, kind and slot, in slot order), and adds nothing else to any report. It must meet
>   three conditions:
>   - it descends from the rule commit;
>   - `git diff --name-only` between its first parent and it names only paths under
>     `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/` and `docs/`, `eng/compare-parse-corpus.py`, and
>     the assurance artefacts of those files; never `eng/packages/`, `eng/test262-evidence.py` or any workflow (JSD-0083's
>     decision D37);
>   - it is an ancestor of every base commit below.
> - **The pin commit** is the default-branch merge that admits the version of the shared stream that carries the model,
>   as the owner published it (JSD-0083's decision D30), and moves this family's vectors pin to the corpus revision that
>   carries the `semantics` vectors (decision D61). It changes no product source but `Directory.Packages.props`, the
>   allow-list, the notices and the vectors pin, and it is an ancestor of every base commit below. If SJL-15.4 needs a
>   later published version than SJL-15.3, a second pin commit of the same kind precedes its base.
> - **The window-1 after commit** is the default-branch merge that lands SJL-15.3, one merge, named in this bundle; **the
>   window-1 base** is its first parent. **The window-2 after commit** is the default-branch merge that lands SJL-15.4, one
>   merge, named in this bundle; **the window-2 base** is its first parent. Each base must meet four conditions:
>   - it descends from the rule commit, the tools commit and the pin commit that window reads, and from bundle
>     `sjl-13-002`'s after commit `467ad7c395234b335c816b8f49ae5b505740e32c`;
>   - its after commit is the only first-parent commit that carries any part of its window; if a window had to land in
>     more than one merge, each is named in this bundle, the base is the first one's first parent, and the after runs are
>     taken at the last one;
>   - if both windows have landed, the later window's base descends from the earlier window's after commit;
>   - neither the base nor the after commit lies inside SJL-11.4's window as bundle `sjl-11-001`'s rule defines it (its
>     admission commit to its after commit), whose bases that rule voids if another merge lands there.
> - **Part A's base and after runs** are dispatched at those commits at any time after each exists. **The other parts**
>   are taken locally from a clean checkout of each commit they name, after the window's after commit has merged.
> - **No run is taken before the base it names exists.** A run at any other commit, a "before" output taken at the rule
>   commit among them, is not a run of this rule and decides nothing.
>
> A part whose base was taken at any other commit, or whose commits fail those conditions, is void and is not judged.
>
> ## Part A - test262 parity, in both forms, on hosted runners (template T1)
>
> ```rule
> evidence class: conformance parity (not a measurement)
> population: bundle sjl-13-001's part A's, at the pin the base commit holds
> configuration: bundle sjl-13-001's part A's, at each window, the base runs at that window's base and the after runs at its after commit, the workflow and the driver at the dispatched ref byte-identical to the base commit's
> completeness: bundle sjl-13-001's part A's
> comparison: per variant, joined on path and variant, the verdict and any exhausted dimension, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair with --exempt-guest-loads over the extracted suite
> admitted difference classes:
>   C1 correction of this family (window 1 only) - a variant whose verdict moves from fail to pass for the reason of one of the rows LB1 (H20), MR2 (H19) or the dynamic function's goals (sjl-13-001's P26 and D61, H18), each listed with its row; or (T) a variant whose verdict moves because eval code's undeclared private name is refused when the eval code is compiled rather than by the executor (row PN1, decision D64's second half), listed with the row
>   C4 harness and environment - bundle sjl-13-001's part A's classes (a), (b), (c') and (e), with their re-runs on the local machine
> no C1 at window 2: the conformance root compiles nothing through the slice path
> no C2, no C3 and no C5
> every other difference is a regression, a verdict that moves from pass to fail among them
> MET if and only if all four runs of each window are complete, and every changed verdict of either pair is in C1 or C4 and is listed.
> NOT MET otherwise.
> ```
>
> ## Part B - artifact identity over the parse corpus (template T6)
>
> ```rule
> evidence class: artifact identity (not a measurement)
> population: bundle sjl-13-001's part B's, sub-populations B1 to B5, as the base commit holds the manifest
> configuration: the slice compiler's --parse-corpus mode, as the tools commit holds it and unchanged at each after commit, built in Release from a clean checkout on the local machine; at each window, run at its base and at its after commit, and with --census at both; for every input whose hash differs, the artifact bytes at both commits written and retained
> completeness: every manifest input reported in every sub-population, variant and form at both commits, the manifest's digest equal at both, and no run lost its process
> controls: a copy of the base report with one artifact hash changed, one outcome flipped and one diagnostic's code changed, which the comparison must report as three unclassified differences; retained with its output
> comparison: eng/compare-parse-corpus.py, as the tools commit holds it, joined on input, sub-population, variant and form: the outcome; for an input accepted at both commits, the artifact hash, or in B5 its ordered specifiers; for an input refused at both, the ordered diagnostic list; the census, byte for byte
> admitted difference classes, window 1, in B1, B2, B3 and B5 only:
>   P precision (correspondence rows FF3, FF4, FF5) - an input accepted at both commits, the same outcome, whose artifact differs only because the model's fact is narrower than the base binder's approximation: an arguments object no longer materialised where the base's binder needed one only for a mention of eval; a function body's variable environment created or not where the model's fact and the base binder's differ; a function no longer treated as direct-eval contaminated for a mention of eval that is not a direct call; each listed with its row, the function's place and the decoded difference (the instructions and records that move), and the input's verdict in part A unchanged where it is a test262 input
>   F a correction of this family by one of the rows LB1, MR2 and the dynamic function's goals (H20, H19, H18) - an input whose outcome or first diagnostic moves for that row's reason; each listed with its row
>   T timing (row PN1) - an eval-family input refused at the after commit, under a flag set whose caller is in a class, with the binder's code for a private name no class declares (sjl-13-001's B23), where the base accepted it and its executor refused it at run time; each listed with its flag set
>   S7 first refusal - an input refused at both commits whose list at the after commit is the library's first refusal alone, now that the binder's semantics rows are the library's (correspondence row EE1), and every diagnostic of the base's list at or after that place in source order is absent; each listed with both lists
> admitted difference classes, window 2: none in B1, B2, B3 and B5
> sub-population B4 and the census admit no class at window 1: the slice path does not change there; at window 2 they are part S's
> no ordering, template or position class: the model's declarations are in source order, as the binder's were, and the library places every early error at the place the binder placed it
> every other difference is a regression: any byte difference of an input accepted at both commits outside P; a specifier list that differs for a B5 input accepted at both; an outcome that moves outside F and T; a diagnostic difference outside F, T and S7
> MET if and only if both runs of each window are complete, the control is reported as stated, and every difference is in the class its window admits and is listed.
> NOT MET otherwise.
> ```
>
> **How it is read.** As bundle `sjl-13-002`'s part B: identity is equality of SHA-256 over the artifact bytes, and bytes
> are retained only where hashes differ. Class P is the milestone's "precise instead of an over-approximation", named here
> before any run because bundle `sjl-13-002` admitted no byte difference and clause 6 asks that every difference be
> classified (correspondence finding F2). A difference class P names that the decoded bytes do not show is a regression.
>
> ## Part S - the slice corpus, byte for byte (template T6, the acceptance check of SJL-D-13 (a))
>
> ```rule
> evidence class: artifact identity (not a measurement)
> population: sub-population B4 of the parse corpus, each input compiled by the slice front end under its goal, as the base commit holds the manifest; the census over src/tests/corpus/parse-1/inputs/ and over the test/ directory of the suite extracted from the pinned archive, as bundle sjl-12-001's census 1 took them; and every program of SliceSourcePrograms, through the slice compiler's --checks rows that pin their outcomes, codes, messages and places
> configuration: the slice compiler's --parse-corpus mode and its --census mode, as the tools commit holds them and unchanged at each after commit, built in Release from a clean checkout on the local machine; for each B4 input the outcome, for an accepted input the artifact's SHA-256 and length and the binding table (each binding's name, kind and slot, in slot order), for a refused input the ordered diagnostics (code, message, line, column); at each window's base and after commit; the --checks rows at both
> completeness: every B4 input reported at both commits, the manifest's digest equal at both, both census outputs whole, and no run lost its process
> controls: a copy of the window-2 base report with two bindings of one accepted input's table swapped, one refused input's message changed, and one census line changed, which the comparison must report as three differences; retained with its output
> comparison: per B4 input, the outcome, the artifact hash, the binding table and the ordered diagnostic list, each byte for byte; each census output, byte for byte; each --checks row's verdict
> admitted difference classes: none
> the inputs the correspondence expects to differ at window 2 (row SL4: hand-offs H23 and H25, J000262 and J001009 among them) are listed in this bundle before any run of this part, from the correspondence; each that differs is a difference, which clause 5 as written does not admit
> every difference is a regression
> MET if and only if all runs of each window are complete, the control is reported as stated, no difference of any kind is reported, and every --checks row passes at both commits.
> NOT MET otherwise.
> ```
>
> **How it is read.** This part is clause 5's "the slice corpus's census counts, slot order and refusals are
> byte-identical before and after", and SJL-D-13 (a)'s "keeping its census counts, slot order and refusals byte for byte".
> The slot order is read twice: in the binding table the tools commit adds, and in the artifact bytes, which encode each
> slot. A refusal is its code, its message, its place and its order. The census is read whole, its parse half included
> (correspondence row SL5): a file `SliceParser` parsed that the library refuses moves it. **Should the owner revise
> clause 5 under decision D-census**, the revision is filed by a new dated rule file in this directory, quoting this
> part, before any run of it, and that file's class, not this part, judges the listed inputs; otherwise this part stands.
>
> ## Part C - determinism (layer L13)
>
> ```rule
> evidence class: determinism (not a measurement)
> population: every input, variant and form part B's after run accepted in sub-populations B1 and B3, at window 1; every input part S's after run accepted in B4, at window 2; and the slice compiler's --checks rows "the wide lowering is deterministic" and "the wide lowering declares lexical names in source order" at both commits of each window
> configuration: the slice compiler's --parse-corpus mode at each after commit, built in Release from a clean checkout on the local machine, in three processes - run 1 with one worker, run 2 with one worker, and run 3 with two workers compiling every input twice concurrently, once on each
> controls: K1, a scratch copy of the window-1 after commit in which the lowering reads the model's declarations of the global scope in reverse - the source-order row must fail, and part B's comparison over the patched build must report a byte difference; K2, a scratch copy of the window-2 after commit in which the slice lowering allocates its slots from an unseeded shuffle - run 1 against run 2, or the two compilations of run 3, must differ, and part S's comparison must report a binding-table difference; each reverted, and the revert checked
> comparison: per input, variant and form, the four hashes - run 1, run 2 and the two of run 3 - and, at window 2, the four binding tables
> admitted difference classes: none
> MET if and only if every accepted input gives one hash, and at window 2 one binding table, in all four compilations, both rows pass at both commits of each window, and both controls fail as stated and pass after their revert.
> NOT MET otherwise.
> ```
>
> ## Part D - the front end's rules (rule runs)
>
> ```rule
> evidence class: rules (not a comparison)
> population: rules N5, N6, N7, N8, N9, N12, N16 and N19, I1 to I7, A2 and A7 of src/tests/Broiler.VM.Architecture.Tests/rules.register.json, their tests, and the witnesses their register rows name
> configuration: dotnet test of Broiler.VM.Architecture.Tests in Release from a clean checkout of each after commit, with BROILER_RULE_MESSAGES set to a directory of the bundle, the test log and the rule-message files retained; the same at each base, retained and read, not judged; rule I5's controls, a re-added deleted member of JsBinder in a scratch copy of the window-1 after commit and a re-added deleted member of SliceParser in a scratch copy of the window-2 after commit, each of which must be refused
> comparison: none between commits
> MET if and only if every test of the rules named passes at each after commit; every witness they name is refused; N12's test counts the compiler assembly's files with a comment naming each file the window deleted and added; N16's test holds the slice manifest on its own path, unrevised; I5 registers every type and member the verdict's deletion list gives the window, member by member (JsBinder and its nested types at window 1; SliceParser, SliceTokenizer and SliceSyntax.cs's records at window 2); I3's register row names the frontend clause's subject; and both controls are refused.
> NOT MET otherwise.
> ```
>
> ## Part E - the `semantics` vectors at engine level (template T3, decision D61)
>
> ```rule
> evidence class: conformance (not a measurement)
> population: every vector of the semantics area at the corpus revision src/tests/vectors/pins pins at the window-1 base, which the pin commit moved to, in schema 1
> configuration: bundle sjl-13-001's part H's runner and reading, at the window-1 base and after commit
> controls: a copy of one vector's expected outcome changed to this engine's opposite answer; the runner must report it as a difference; retained
> comparison: per vector, the verdict at the base against the verdict after
> admitted difference classes: C1 - a vector whose verdict is difference or classified-divergence at the base and match after, listed with its row of the correspondence; a divergence field that no longer holds is retired by the next corpus revision (decision D61, the route the syntax vectors took)
> every other change is a regression, including a vector that matches at the base and does not after
> MET if and only if every vector's verdict after is match or classified-divergence, every change is in C1 and is listed, and the control is reported.
> NOT MET otherwise.
> ```
>
> **How it is read.** The `semantics` vectors land in Broiler.JS's corpus before the shared code lands (the milestone
> SJL-1.6), and this family's pin moves to them in the pin commit, before window 1's base, so no product merge of this
> window carries them.
>
> ## Part F - source fuzzing (layer L9)
>
> ```rule
> evidence class: robustness (not a measurement)
> population, configuration and reading: bundle sjl-13-001's part G's, both sessions, with its seeds file, at each after commit, and the same sessions at each base, retained and read, not judged
> controls: a scratch copy of the window-1 after commit in which the lowering throws on one model fact the wide seeds reach, and one of the window-2 after commit in which the slice static semantics throws on one model fact the slice seeds reach; each session must report a finding under its control; reverted, and each revert checked
> MET if and only if no session of either after commit reports a finding and both controls are reported as stated.
> NOT MET otherwise.
> ```
>
> ## Part G - the fuel-parity twins (template T7)
>
> ```rule
> evidence class: resource accounting (not a measurement)
> population, configuration and ceilings tried: bundle sjl-13-001's part I's, the bisect taken at the window-1 base and retained as sjl-15-001/base/fuel-parity.log
> admitted difference classes: none; a twin whose artifact moves by part B's class P is listed, and must still give the base's verdict at each ceiling tried
> MET if and only if every twin, in both forms, gives at the window-1 after commit the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
> NOT MET otherwise.
> ```
>
> ## Part H - the closure-sensitive suites
>
> ```rule
> evidence class: conformance (not a measurement)
> population: this family's closure-sensitive suites: every --checks row of the slice compiler's EvalScopeCorpus, ScriptDeclarationCorpus, ScriptReferrerCorpus, WideLoweringChecks, FinallyMatrixChecks and BinderChecks, as the base of each window holds them; and the test262 files of part A's population under test/language/eval-code, test/language/arguments-object, test/language/function-code, test/language/block-scope, test/language/statements/for, test/language/statements/for-in, test/language/statements/for-of, test/language/statements/with, test/annexB/language, test/language/statements/class/elements and test/language/expressions/class/elements
> configuration: the --checks rows at each base and each after commit, built in Release from a clean checkout on the local machine; the test262 files read from part A's runs, not run again
> comparison: each row's verdict, and each named row's pinned outcome, code, message and place, at the base against the after commit; each test262 variant's verdict, from part A's comparison
> admitted difference classes: a row the window's merge revises is listed with its diff, and must pin at the after commit what it pinned at the base, but where part B's classes P, F, T and S7 name the change, each such row listed with the class; the test262 files admit part A's classes only
> every other difference is a regression, a row that fails at the after commit among them
> MET if and only if every row passes at both commits of each window, every revised row is listed and pins what it did but for a listed class, and no test262 variant of the named directories changes outside part A's classes.
> NOT MET otherwise.
> ```
>
> **How it is read.** The milestone's SJL-15.1 names "Broiler.JS's closure-sensitive suites"; Broiler.JS's rule for
> SJL-15, in its own bundle `sjl-15-001`, judges those. This part names this family's own, which the lowering's reading of
> resolution, `arguments`, direct `eval`, Annex B, private names and labels reaches. The test262 directories are read
> from part A's runs, so they are part A's population named twice, and this part adds no run of them.
>
> ## Part I - publication in three modes
>
> ```rule
> evidence class: deployment (not a comparison)
> population: every JavaScript root this machine can publish, each root that carries the JavaScript profile, the Android root being EX-126's
> configuration: each root published in Release at each after commit, and run in three modes (JIT, trimmed and Native AOT), on the local machine, with its closure listed
> MET if and only if every root of the population publishes and runs in all three modes at each after commit, and rule I3 holds on each Native AOT image's closure.
> NOT MET otherwise.
> ```
>
> **How it is read.** Clause 9 reads "Every root is published and run in three modes." This part takes "every root" as
> every JavaScript root this machine can publish, as the readings of SJL-3, SJL-8, SJL-9, SJL-10, SJL-13 and SJL-17 took
> the same words (JSD-0088's decision D-roots). Not taken: every root the composition register lists, the roots that
> carry no JavaScript profile among them. Bundle `sjl-13-001` left this clause to the collector's runs, read and not
> judged; this rule makes it a part.
>
> ## Part J - this family's compile measurement (template T5)
>
> ```rule
> evidence class: measurement
> population, arms, configuration, condition, quantity, noise floor, comparison and readings: bundle sjl-13-001's part J's, with its workloads, at each window, B that window's base build and A its after commit's; retained in sjl-15-001/measurement/
> condition: as bundle sjl-13-001's part J's, except that at window 1 a workload whose artifact moves by part B's class P is compared against the after arm's own hash in its A/A lane, and listed
> admitted difference classes: none
> MET if and only if every lane's condition passes, every effective-configuration line is identical, and for every workload D <= N.
> NOT MET otherwise.
> ```
>
> **What part J decides, and what it does not.** Whether reading the model compiles more slowly than the binder did, or
> the rebuilt slice path more slowly than `SliceParser`, beyond the A/A floor, on this machine. **It is a rule part, not a
> gate clause** (JSD-0088's decision D-measure): SJL-15's exit gate has no measurement clause, and SJL-15.6's "each
> compiler's own measurement" is read, as SJL-13 read its own, as a part of each bundle's rule. On a lane that is not
> controlled its runs are retained and read, and decide no speed claim.
>
> ## Revisions
>
> A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
> and says it was written afterwards, and its decision is then weaker evidence. A revision filed before any run of the
> part it revises, the owner's answer to decision D-census among them, quotes this file whole and says so. **Before
> SJL-15.3's code lands, a later bundle shows this file unchanged.**
>
> ## What this file does not decide
>
> - **The home clauses** (1 to 3) and **Broiler.JS's clauses** (10 and 11): Broiler.JS's bundle `sjl-15-001` decides them,
>   under its own rule, its closure-sensitive suites and its deletion search among them.
> - **The records**: the profile's JSD for decision `SJL-D-13`, with its JSC entries, filed at SJL-15.4; Broiler.JS's
>   record closing invariant 3.
> - **What clause 5 means for the inputs H23 and H25 reach**: the owner's (JSD-0088's decision D-census).
> - **Each publication of the shared stream**: the owner's (JSD-0083's decision D30).
> - **A speed claim of any kind.**
