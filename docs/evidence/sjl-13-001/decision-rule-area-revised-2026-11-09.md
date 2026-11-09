<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-001's area rule, revised 2026-11-09: a bounded class for the first refusal's place and code, and two corrections in class F

**Written:** 2026-11-09, before any run of step 1 that this rule judges. It follows the owner's three decisions of
2026-11-09, the owner's own dated choices, asked and answered that day, which JSD-0083's dated notes of that day on
decisions D11 and D15, on its table "The corrections step 1 makes, and how each is shown" and on its section "What this
rejects, and why" record: "Revise rule: bounded class" on the places and codes of refused inputs, and "Correction row +
class F" twice, once for the parse of a `new` whose operand is a `new` with arguments and once for the early errors the
module-request path did not check. This file revises [`decision-rule-area.md`](decision-rule-area.md) as that file's
section "Revisions" requires. It is a new dated file, it quotes that file whole at its end, and that file is not
edited. The revisions of 2026-11-02,
[`decision-rule-area-revised-2026-11-02.md`](decision-rule-area-revised-2026-11-02.md) at blob
`71cf04ebcd1b3c3745d87b20c9b98461b8024dbb`, of 2026-11-04,
[`decision-rule-area-revised-2026-11-04.md`](decision-rule-area-revised-2026-11-04.md) at blob
`1995173d6d12fea1dd71f89fa53889e332bb6d5a`, and of 2026-11-08,
[`decision-rule-area-revised-2026-11-08.md`](decision-rule-area-revised-2026-11-08.md) at blob
`5ed79425e1bc6fc9096daf27f5ac62625abe26ae`, stand, and none is edited. Where this file quotes a passage that an earlier
revision changed, it quotes the passage as that revision reads it and says so. It changes nothing else that any of them
changes.

**What was seen, and where.** Step 1's second stage, the adapter, was built on a local branch that was never pushed,
`sjl-13-3-step1b`, cut from the default branch at `31c4779` with the registry commit and the admission rebuilt on it as
the revision of 2026-11-08 lists them. The stage built the adapter as a change it did not commit, and compared it with
`JsParser` and with the base's parse-corpus report on the stage's own machine. The branch, the change and its logs are
kept outside this repository. Those comparisons are a local pre-check, not evidence, and not runs that this rule
judges: part A's runs are dispatched from the default branch, and parts B to J are taken after the after commit has
merged. No run that the area rule judges has been taken, so this revision is not written after any of them. The
pre-check found three kinds of difference that part B, as filed and as revised, does not admit:
- **The places and codes of refused inputs.** With the library's first refusal (decision D11), many inputs refused at
  both commits report their one diagnostic at another place than `JsParser`'s first, most often on the same line, or at
  the same place under another registry code. That code is the one the mapping gives for the library's diagnostic,
  most often EX0008's 2101 "unexpected token" where `JsParser`'s `Expect` reported 2102 for a missing token. A smaller
  set reports, as its one diagnostic, one that the base reported after its first. Another set was refused at the
  base by the binder alone, after a parse `JsParser` accepted, for a check of a row the stage table assigns `syntax`
  (a name bound twice, a `var` that collides with a lexical name, an invalid assignment target); the library makes
  that check at the parse and reports it at another place, most often later on the same line, or under another code.
  Part B admits a code pair only under R and R2, and its sentence "no ordering, template or position class" fixes
  every place to this family's at the base.
- **One input accepted at both commits whose artifact bytes change:** `T052945`
  (`test/staging/sm/class/superCallBadNewTargetPrototype.js`), whose line 27 holds `new new Proxy(inst, {get})()`
  and whose line 30 holds `new new Proxy(defaultInst, {get})()`.
  `JsParser` reads it as `new ((new Proxy(…))())`: in `ParseMemberOnly`, a `new` operand goes to `ParseCallChain`,
  whose loop takes the second argument list as a call. The library reads it as the specification does,
  `new (new Proxy(…))()`. No row of the correspondence named this correction, so class F could not admit it.
- **Inputs accepted at the base and refused after, in sub-population B5:** the module-request path
  `JsCompiler.Requests` parses and never runs the binder, so at the base it answers the requests of a module the
  specification makes an early error, among them `T040801` (a name bound twice by one import), `T042027` (`return` at
  a module's top level) and `T041820` (`0++;`). From step 1 the library makes those checks at the parse, so the path
  refuses them. That is an outcome move outside classes F, L and L2.

**What the owner decided.**
- **"Revise rule: bounded class".** The rule is revised, on the default branch, before any run it judges, by a class
  of part B that holds only inputs refused at both commits. It admits the after commit's one diagnostic at the
  library's place, or under the code the mapping gives for the library's diagnostic, or both, each such input listed
  with both lists. The bytes of every accepted input and every outcome stay fixed by it. Decision D15's reason, that
  the lowering reads each anchor into the artifact, concerns accepted inputs, and the class admits nothing for them.
- **"Correction row + class F"** for `T052945`. The correspondence gains a dated row, a correction of Broiler.VM, for
  `JsParser`'s reading of a `new` operand through the call chain, and class F admits its inputs, byte differences
  included.
- **"Correction row + class F"** for the module-request path. The correspondence gains a dated row, a correction of
  Broiler.VM, recording that the base's `JsCompiler.Requests` accepted inputs the specification makes early errors,
  and class F admits the outcome moves it explains, each listed.

The two rows are the correspondence's **P37** and **D64**, the next free numbers of its sub-areas P and D, each added
with a dated note, and its section "The rows by class" lists each under "Correction of Broiler.VM" (see that file).

1. **Part B, a new class N, after class L2.** It reads:

   "N the first refusal's place and code (decision D11; decision D15, as JSD-0083's note of 2026-11-09 on it bounds it)
   - an input refused at the base commit, at the registry commit and at the after commit, whose list at the after
   commit holds exactly one diagnostic, the library's first refusal (decision D11), whose code equals the
   registry_code of the mapping's `code` row, at the mapping's revision at the after commit, whose shared code is that
   diagnostic's library message's, and whose two lists, at the registry commit and at the after commit, S7 would
   admit, read with R, R2 and S's other kinds, but for that diagnostic's place, its code, or both. The library's
   message is the shared code (JSEnnnn) and the condition identifier that one run of the after commit's front end over
   that unit under this family's request reports for that diagnostic, one process, retained. Write r for the
   diagnostic of the registry commit's list that the after commit's one diagnostic stands for: of that list's
   diagnostics that S6 does not remove, and that S3 does not remove or that are the binder's check of a row the stage
   table assigns `syntax` (a check the library makes at the parse from step 1; decisions D10 and D20), the one first
   in the source text, by line and then column, and among several at one place the first in the list. A binder
   diagnostic of a `syntax` row is thus r's candidate, and is not removed by S3 when it is r. The input's kind is
   (place) when the after commit's diagnostic stands at another line or column than r, with r's code or one R or R2
   pairs with it; (code) when it stands at r's line and column, with a code that is neither r's nor one R or R2 pairs
   with r's; and (place and code) when it stands at another line or column, with such a code. An input whose registry
   list leaves no r is not in N. In B1 to B3 and B5 only; never an input accepted at either commit, never an outcome
   that moves, never B4 or the census; each listed with both lists, its kind, r and the stage that reported it at the
   registry commit (the parse or the binder), the library's message and the mapping row that gives its code"

   It is called N because no class of this rule holds that letter (R, R2, S, F, L and L2 are part B's, C1 to C6 part
   A's), and because the letter before it, M, is SJL-14's class for Broiler.JS's message changes (decision D46), which
   Broiler.JS's bundle `sjl-14-001` keeps: N follows it, so that the two front ends' classes for what the shared
   library moves are not read as one. It is a class of part B, not a row of the correspondence's sub-area N.

   **How N stands to the other classes.**
   - **S7 is unchanged.** It names what the first refusal removes: the diagnostics the registry commit's parse reported
     at or after the after commit's one diagnostic, the cascade of the statement in progress among them. It does not
     move the diagnostic that remains, which, by the sentence item 2 narrows, stands where this family's front end
     placed it at the base, with the code R or R2 gives it. N is the class for that one diagnostic when it stands
     elsewhere or carries another code. An input whose after diagnostic is r, its code read through R and R2, is S7's
     (and S8's, for two declared-name conflicts of one scope) and not N's. An input of N is listed with S's kinds too
     where they apply (S3 for a suppressed diagnostic of the numeric admission, the binder or the lowering other than
     r, S6 for a lexical one). Where the registry commit's list holds only the binder's diagnostics, as for an input
     `JsParser` accepted and the binder refused, r is the first of them that checks a `syntax` row, and the input is
     N's when the library reports that rule at another place or under another code.
   - **R and R2 are unchanged.** A code pair that R admits (a renumbering by revision 19, already present at the
     registry commit) or R2 admits (a consolidation at a site that serves several rules) is theirs. N's kinds (code) and
     (place and code) take only a code that is neither r's nor one of those pairs, and only when it equals the
     registry_code of the mapping's `code` row for the library's message's shared code. R is still the only class
     between the base and the registry commit, and in B4 and the census.
   - **"Every other difference is a regression" is narrowed by N alone**, and only as item 4 says. A refused input whose
     after list holds more than one diagnostic, whose one diagnostic is not the library's (a refusal of the admission
     walk, the numeric admission, the binder or the lowering after a parse the library accepted), or whose registry list
     leaves no r, is not in N. **A diagnostic whose code is not the registry_code of the mapping's `code` row for its
     library message's shared code, or whose shared code has no `code` row, stays a regression**: N admits it under no
     kind, and neither does any other class. A code that the row lists only among its `wide_codes`, `slice_codes` or
     `other_codes` is not its registry_code, and is such a code.
2. **Part B, the sentence "no ordering, template or position class".** It reads: "no ordering, template or position
   class: step 1 reorders no declaration, changes no template's reading, the adapter reproduces every anchor and offset,
   and the library places every diagnostic where this family's front end placed it at the base, except the binder's
   report at no place, which class R2 admits (decision D15)". It now reads: "no ordering or template class, and no
   position class but N: step 1 reorders no declaration, changes no template's reading, and the adapter reproduces every
   anchor and offset of every input accepted at both commits, the trees of row P37's inputs excepted under class F; the
   library places every diagnostic where this family's front end placed it at the base, except the binder's report at
   no place, which class R2 admits, and the one diagnostic of an input refused at both commits that class N lists
   (decision D15, as JSD-0083's note of 2026-11-09 on it bounds it)". Ordering and template classes stay excluded, and
   no place of an accepted input moves.
3. **Part B, class F, and part A's kind (F), which names its rows.** Part A's kind (F) reads "(F) a correction of this
   family that step 1 makes, by one of the rows the correspondence names as the corrections SJL-13 delivers - T6, T7,
   T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32 and D40, from its columns "Correction of Broiler.VM" and
   "Correction of both" (decision D25) - a variant failing at the base that passes after, listed with that row;". Its
   row list now reads "T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32, D40, P37 and D64", and the rest of the
   kind is unchanged. Part B's class F reads "F a correction of this family that step 1 makes, by one of the rows part
   A's kind (F) names (T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32 and D40) - an input whose outcome or
   first diagnostic moves between the registry commit and the after commit for that row's reason; the corpus holds
   inputs of P15b, and the others are shown by the correspondence's probes and part H's vectors; in B1 to B3 and B5
   only; each listed with its row". It now reads: "F a correction of this family that step 1 makes, by one of the rows
   part A's kind (F) names (T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32, D40, P37 and D64) - an input whose
   outcome or first diagnostic moves between the registry commit and the after commit for that row's reason, or, by row
   P37 alone, an input accepted at both commits whose artifact bytes differ between them because the after commit
   reads as the specification does a `new` whose operand begins with a `new` with an argument list and goes on to
   another argument list, at once or after property accesses or tagged templates; the corpus holds inputs of
   P15b, of P37 (in B1 to B3) and of D64 (in B5), and the others are shown by the correspondence's probes and part H's
   vectors; in B1 to B3 and B5 only; each listed with its row, and an input of P37 with the place of each such `new`
   and its bytes at both commits".
4. **Part B, its regressions and its MET line.** The regressions read, as the revision of 2026-11-02 reads them:
   "every other difference is a regression: any byte difference of an input accepted at both commits; a specifier list
   that differs for a B5 input accepted at both; an outcome that moves outside F, L and L2; a diagnostic difference
   outside R, R2, S and F; any difference between the base and the registry commit outside R; and any difference in B4
   or the census outside R". They now read: "every other difference is a regression: any byte difference of an input
   accepted at both commits outside class F's row P37; a specifier list that differs for a B5 input accepted at both;
   an outcome that moves outside F, L and L2; a diagnostic difference outside R, R2, S, F and N, among them a refused
   input's diagnostic whose code is not the registry_code of the mapping's `code` row for its library message's
   shared code, or whose shared code has no `code` row; any difference between the base and
   the registry commit outside R; and any difference in B4 or the census outside R". The MET line, "every difference
   is in R, R2, S, F, L or L2 and is listed" as the revision of 2026-11-02 reads it, now reads "every difference is in
   R, R2, S, F, L, L2 or N and is listed".
5. **Part B, "How it is read".** "**Every accepted input's bytes are expected identical**; a byte difference anywhere
   is NOT MET." now reads "**Every accepted input's bytes are expected identical** but those of row P37's inputs, which
   class F lists; any other byte difference is NOT MET." "From the after commit a refused input's list holds the
   library's first refusal alone, and classes S7 and S8 name what that removes." now reads "From the after commit a
   refused input's list holds the library's first refusal alone, classes S7 and S8 name what that removes, and class N
   names that refusal where it stands elsewhere than r, or carries a code that is neither r's nor one R or R2 pairs
   with it (item 1)." "read through R, R2, S and F" now reads "read through R, R2, S, F and N".
6. **Part A, "How it is read".** Its sentence on the kinds, as the revision of 2026-11-04 reads it, ends "no input of
   the parse corpus reaches a BigInt refusal (the correspondence's F6), and no test262 file reaches a correction step 1
   makes (its F19)." That end now reads "no input of the parse corpus reaches a BigInt refusal (the correspondence's
   F6), and no test262 file is expected to move its verdict by a correction step 1 makes (its F19, and rows P37 and
   D64): `T052945` reaches row P37, but both readings of its `new` throw the TypeError it asserts, and the module tests
   row D64's inputs come from are compiled by the conformance root through `JsCompiler.Compile` after the request path,
   whose binder refuses them at the base." Kind (F) is still expected to list nothing; a variant it lists is listed with
   its row, as before.
7. **"What step 1 changes", the bullet "What does not change".** Its clause "every anchor, source offset and computed
   fact the adapter reproduces, `JsParser`'s top-level-`await` rule included, and every diagnostic's place, which the
   library takes from this family's front end at the base, except the binder's report at no place (decision D15);" now
   reads "every anchor, source offset and computed fact the adapter reproduces, `JsParser`'s top-level-`await` rule
   included, except the tree of a `new` of the shape the correspondence's row P37 names, a `new` whose operand begins
   with a `new` with an argument list and goes on to another argument list, which the adapter builds as the
   specification reads it; and every diagnostic's place, which the library takes from this family's front end at the
   base, except the binder's report at no place and the one diagnostic of an input refused at both commits that part
   B's class N lists (decision D15, as JSD-0083's note of 2026-11-09 on it bounds it);". The bullet's other clauses, as
   the revision of 2026-11-08 reads its last one, are unchanged. The revision of 2026-11-08 says of its own change to
   that last clause, "That renumbering is the only narrowing of "What does not change"." That sentence holds of that
   revision alone: from this revision the bullet has three narrowings, that renumbering and the two exceptions this
   item adds, row P37's tree and class N's one diagnostic.

   A `new` whose operand begins with a `new` with an argument list and goes on to an optional chain,
   `new new C(a)?.b`, is outside row P37 and this item: the specification's grammar does not admit it, and `JsParser`,
   as read from its code at `31c4779`, takes the optional chain into the operand and accepts it, so its outcome would
   move, and that move stays a regression. No input of the parse corpus holds it.

**What this revision does to parts A to J, and to the records beside them.**
- **Part A:** only kind (F)'s row list (item 3) and the expectation of "How it is read" (item 6). Its population,
  configuration, completeness, comparison, other kinds and classes, and MET line are unchanged. Part A admits no
  class for a code or a place that moves as such: it compares verdicts, and reads first diagnostics only to name a
  kind of C1. A verdict that moves because a code or a place moved, as class N admits in part B, stays a regression
  of part A unless one of its existing kinds, (R), (G), (B) or (F), names it. So part A gains no counterpart to N.
- **Part B:** items 1 to 5. Its population, configuration, completeness, control, comparison, and classes R, R2, S, L
  and L2 are unchanged. The control's three changes stay unclassified, as it states: the control is a copy of the
  base report, changed, and compared with the base report, so only class R, the one class between the base and the
  registry commit, could apply to it, and none of its three changes is a renumbering by revision 19. Classes F and N
  compare the registry commit with the after commit, and never read the control.
- **Parts C, D, E, F, G and I** are unchanged. Part C reads the after commit's accepted inputs, row P37's among them,
  only against each other. Part E's checker and controls are unchanged: class N reads the mapping, and adds no check to
  it. No fuel-parity twin of part I holds the text `new new`.
- **Part H** is unchanged: its C1 already admits a vector by its row of the correspondence, rows P37 and D64 among
  them should a vector carry one, and part H reads no place or code.
- **Part J** is unchanged. No workload of `inputs/workloads/` holds the text `new new`, and none is compiled through
  the module-request path, so its condition, each lane's outcome and hash equal to the base arm's, is unchanged.
- **"The commits"** is unchanged: the registry commit's list, as the revision of 2026-11-08 gives it, holds no path
  this revision needs, and this file is filed on the default branch before step 1's base.
- **The lane's CLI acceptance table**, `src/tests/cli/expected.txt`, is no part of this rule. Its rows that pin a
  refused input's place or code move in the commit of step 1 that moves them, so that the lane passes on the branch,
  as JSD-0083's decision D12 asks; this rule reads them nowhere.
- **SJL-14's class M** is Broiler.JS's bundle `sjl-14-001`'s class for Broiler.JS's message changes (decision D46).
  It is not read here, and this revision does not touch it; class N admits nothing for Broiler.JS.
- **Bundle `sjl-13-002`'s rule** is not revised. Its base is this rule's after commit, whose places and codes it holds.

**What does not change.** Every other part, class and control; "The commits" and their conditions; and the revisions
of 2026-11-02, 2026-11-04 and 2026-11-08, except that item 4 of this file quotes passages the revision of 2026-11-02
changed and changes them further, item 6 a passage the revision of 2026-11-04 changed, and item 7 a bullet whose last
clause the revision of 2026-11-08 changed and this file leaves as that revision reads it; that revision's sentence
"That renumbering is the only narrowing of "What does not change"." holds of that revision alone, and from this
revision the bullet has the three narrowings item 7 names. The bytes of every
input accepted at both commits but row P37's, and every outcome but those classes F, L and L2 admit, stay fixed.
Ordering and template classes stay excluded.

**Not taken.**
- **"Library fix to D15 places".** Broiler.JS's library would have been changed to report this family's places and
  codes, decision D15 as filed, in a new preview of the shared stream, `0.1.0-preview.11`, which this family would then
  admit again. The differences that are inherent in the two front ends' detection, a refusal the library detects at
  another token than `JsParser`'s, would have remained.
- **"Measure causes first".** The differences would have been measured and sorted by cause before any revision, and
  the rule revised afterwards.
- **"Adapter copies the misparse".** The adapter would have rebuilt `JsParser`'s tree for row P37's shape, keeping the
  bytes and a known defect of this family on the wide path.
- **"Record as finding".** The Requests path's outcome moves would have been recorded as a finding, and part B would
  not have been met.

**Owner:** the JavaScript profile owner, with the core architecture owner, as for the rule this file revises.
**Reviewer:** none; one person holds every role (EX-30), and this revision does not claim that anyone independent has
read it.

## The rule it revises, quoted whole

The quote below is [`decision-rule-area.md`](decision-rule-area.md) at blob
`6639d7bafc55b33e005329fff9b06fc17178a4ef`, as the Broiler.VM commit that files this revision holds it, each line
prefixed with `> ` (an empty line with `>`), as bundle `sjl-3-001`'s revision quotes its rule.

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-13-001's area rule: the wide path on the shared syntax library, step 1, judged by test262 parity in both forms on hosted runners, by artifact identity over the parse corpus, by determinism and the source front end's gates, by the front end's rules, by the registry and its mapping, by the tokenizers' agreement, by source fuzzing, by the syntax vectors, by the fuel-parity twins, and by this family's measurement
>
> **Written:** 2026-10-26, at milestone SJL-13 (work package SJL-13.1), with the area verdict
> [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
> before any change it judges exists: no shared library, no tools commit, no registry revision, no admission, no adapter
> and no deletion. Committed in the same change: part G's seeds ([`inputs/fuzz-seeds.txt`](inputs/fuzz-seeds.txt)) and
> part J's workloads ([`inputs/workloads/`](inputs/workloads/README.md)). **Milestone judged:** SJL-13 of
> [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md),
> at the first of its two cut-over steps, the adapter: its Broiler.VM clauses 4 ("`JsParser` is gone, and the wide path
> parses with the frontend."), 5 ("Artifacts are byte-identical over the parse corpus, or every difference is classified
> under the rule."), 6 ("test262 in both forms meets the rule."), 7 ("Rules N5 to N9, N12 and N19 hold. The registry
> revision is filed, if any."), 9 ("Fuzzing runs clean under the rule."), and clause 8's rule `I3`; SJL-13.6's
> concurrency gate of the source front end, and this family's measurement. Bundle `sjl-13-002` judges the second step.
> **Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule
> names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.
>
> **What this file is.** Ten predeclared decisions, one per part, each of one evidence class.
> - **The templates.** Part A is template T1 of
>   [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
>   part B T6, part H T3, part I T7 and part J T5. Part C is layer L13's determinism with the front end's gates, part D
>   the rule runs, part E validation section 7's control for SJL-12 to SJL-15 ("a diagnostic code mapped to two
>   registry meanings"), part F the milestone's "the shared tokenizer's behaviour must match the slice path's", and part
>   G layer L9.
> - **Each part decides alone**, and no part reads another's verdict.
> - **Part J is the only measurement.** Its runs are retained in a sub-bundle of their own, `sjl-13-001/measurement/`,
>   with its own README and manifest. It is not a gate clause of SJL-13 (decision D24).
> - **Other rules.** Bundle `sjl-13-002` judges step 2. Broiler.JS's bundle `sjl-13-001` judges the library in its home.
>   Neither is read here.
> - **The correspondence this rule classifies by** is [`correspondence.md`](correspondence.md) beside it, its rows named
>   by letter: T (tokenizer), P (parser and tree), N (positions), D (diagnostics and the parse-time early errors) and R
>   (request, switches, limits and providers).
> - **It holds no figure.** Where a count matters, it names the file that fixes it.
>
> **What step 1 changes, named before any run** (the verdict's decisions D7, D10, D11, D12, D14, D15, D17, D18 and
> D20).
> - **Registry revision 19.** Every row the mapping marks `split` leaves its registry code for a new number, one per
>   meaning, as the mapping's revision at the registry commit names them; among them the three language rules JSD-0080's
>   decision D13 found coded 2104 (`await`, `using`, and a logical assignment to a target that is neither a name nor a
>   property). A meaning is the kind of fact a registry row's name and declaration state, not a catalogue condition, so
>   a binder site that serves several split conditions keeps one constant: one meaning for every misplaced `super`, one
>   for every regular-expression literal its validator refuses (its nesting bound included), and one for every private
>   name no class declares (the dynamic function's included) (decision D12). A renumbered code keeps its stage. The
>   conformance root grades each new code as it graded the code it left, except those three, which it grades as
>   language early errors (decision D12). Where `JsParser` or the binder emits a split code at a site that serves one
>   rule alone, the registry commit changes the code there to the one the mapping's revision gives that rule: a
>   relation-split row's code (`Attributes`' repeated key, `ParseNewTarget`'s refusal, `BigIntLiteral`'s and
>   `BigIntKey`'s width refusals) and a split other code (`ValidateAccessorParameters`' setter arity, the private-name
>   refusals of `ParseFunctionRest` and `BindingName`, and the binder's refusals of a `delete` of a private reference
>   and of a destructuring declaration without an initialiser).
> - **Consolidation at the parse.** Where the mapping marks `split` an other code that a site serving several rules
>   reaches (`JsParser`'s `ValidateDeclaredNames`, `Expect`, `BindingName`, `ParsePrimary`, `ParseObjectEntry`,
>   `ParseObjectBindingPattern` and `TryParseArrow` sites, and the binder's check of an imported name also declared, which reports at no place), the
>   input reaches the row's own registry code only from the after commit, when the library emits the shared code, and
>   the binder's report at no place gains the place the library reports (decision D12). At an arrow head its cover
>   does not cover, the library reports EE0087 (EE0155 for an async arrow head) where `JsParser`'s generic sites
>   reported EX0009's or EX0008's codes.
> - **The parse.** The wide path parses with `Broiler.ECMAScript.Frontend`; an adapter builds `JsSyntax` from its tree,
>   so the lowering is unchanged. The parse-time static checks `JsParser` made, and the binder's checks of the rows the
>   stage table assigns `syntax`, are the library's, with the codes the mapping gives them; the binder's copies of those
>   checks are deleted (decisions D10 and D20). Top-level `await` (EX0023) is the library's, by the request's switch:
>   with the switch off, the library reads a module's top level as it reads a non-async function's body in module code
>   and reports what it reports there, so EX0023 has no diagnostic of its own and the codes and texts stay as they are
>   (decisions D10 and D11). An import attribute this family cannot honour (EX0021) stays the lowering's refusal. The
>   regular-expression literal is validated during the parse, through the library's provider, by the validator the
>   binder used, with the literal token's span (decision D7). The decorator refusal is reported where the parse
>   reaches it, no longer by the whole-input tokenizer (decision D17). Lexical refusals are reported where the parse
>   reaches them, so a parse refusal earlier in the text may come first. The library charges a left operand's spine to
>   the chain it begins, which closes the gap bundle `sjl-12-001` carried here, in the four shapes the correspondence's
>   row R14 names (decision D14).
> - **The first refusal** (decision D11). This family's request asks the library for its first refusal only. The
>   library reports a refusal as the parse reaches its construct, and a scope's declared-name conflicts when the scope
>   closes, in the source order of the conflicting declarations. So a refused input's list holds one diagnostic from
>   the after commit: the cascade `JsParser` reported after its first refusal is gone, and a check the library takes
>   from the binder no longer comes with the binder's later refusals.
> - **The admission walk** (decision D18). The policy and limit refusals `JsParser` made during the parse (the BigInt
>   literal under a manifest that does not admit BigInt, the BigInt width ceiling, and a module specifier that is not
>   well-formed Unicode, EX0029) are made after a parse that reported nothing, over the shared tree, before the numeric
>   admission and the binder, on every entry point that parsed with `JsParser` (`JsCompiler.Compile`, eval code and
>   `JsCompiler.Requests`). So they are absent when the parse refuses anywhere in the unit, and they no longer precede a
>   later parse refusal. No input of the parse corpus reaches one of them.
> - **What does not change.** The numeric admission and the binder run in today's order, after the admission walk; the
>   binder's other checks and codes, except the renumbering; the lowering; every anchor, source offset and computed
>   fact the adapter reproduces, `JsParser`'s top-level-`await` rule included, and every diagnostic's place, which the
>   library takes from this family's front end at the base, except the binder's report at no place (decision D15);
>   every message text of a code whose meaning stays (decision D11); the slice path, its tokenizer and its census.
>
> **What earlier bundles taught this rule, before any run of SJL-13.**
> - **Part A follows bundle `sjl-11-001`'s part A** for its classes C4 (a), (b), (c') and (e). The local machine for its
>   re-runs is the one bundles `sjl-5-001` to `sjl-12-001` were taken on, named in the bundle by its processor count, its
>   SDK and its operating system, with no other build, test or run on it while a run of this bundle is taken.
> - **Part A's workflow and driver do not move.** Bundle `sjl-11-001`'s rule holds them byte-identical to its rule commit
>   until its after runs, and this rule asks no change of them; the per-variant reading of first diagnostics is done by
>   one-process re-runs on the local machine.
> - **Part B follows bundle `sjl-12-001`'s part B**: its tool, its sub-populations B1 to B4, its control and its reading
>   of refused inputs' diagnostic lists, with sub-population B5 (`JsCompiler.Requests`), which the tools commit adds.
>   Its classes O, T and P are not admitted here; its class S is, re-stated for the stage changes above.
> - **Part C follows bundle `sjl-12-001`'s part C**, with the two rows the tools commit adds, judged at both commits, and
>   the row step 1 adds, judged at the after commit: bundle `sjl-12-001`'s README records that the base compiles two
>   of its shapes past the tree-depth bound, and the correspondence's row R14 the other two, so it cannot pass there.
> - **Part I follows bundle `sjl-10-001`'s part C.** The compiler charges no fuel, and a compilation is charged by the
>   realm before the compiler runs, so the twins reach the change only through artifact bytes, which part B judges.
> - **Part J follows bundle `sjl-11-001`'s part E.**
>
> **The commits, named before they exist** (decision D35).
> - **The rule commit** carries this file.
> - **The tools commit** is the default-branch merge that adds the slice compiler's rows "two goals on the wide path in
>   one process do not reach each other" and "a deep-nesting corpus is refused on the wide path rather than survived",
>   its modes `--fuzz-wide` and `--measure-compile`, sub-population B5 of `--parse-corpus`, rule N19's widened pattern and
>   the mapping checker's revision-19 checks. It must meet three conditions:
>   - it descends from the rule commit;
>   - `git diff --name-only` between its first parent and it names only paths under
>     `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/` and `docs/`, and the files
>     `src/tests/Broiler.VM.Architecture.Tests/CompilationStackRuleTests.cs`,
>     `src/tests/Broiler.VM.Architecture.Tests/rules.register.json`, N19's witness file, `eng/check-frontend-mapping.py`
>     and its control files, `.github/workflows/broiler-vm-lane.yml`, and the assurance artefacts of those files; never
>     `eng/packages/`, `eng/test262-evidence.py` or any workflow but the lane's;
>   - it is an ancestor of the base commit.
> - **The after commit** is the default-branch merge that lands SJL-13.3's step 1, one merge (decision D35). It is named
>   in bundle `sjl-13-001`.
> - **The base commit** is the after commit's first parent. It must meet three conditions:
>   - it descends from the rule commit and from the tools commit;
>   - it descends from the commit bundle `sjl-12-001` names as its after commit in this repository;
>   - the after commit is the only first-parent commit that carries any part of step 1 (registry revision 19, the
>     admission, the adapter, the admission walk or the deletions). If step 1 had to land in more than one merge, each is
>     named in bundle `sjl-13-001`, the base is the first one's first parent, the registry commit is the first one's first
>     commit, and the after runs are taken at the last one.
> - **The registry commit** is the first commit of the branch the after commit merges. Its parent is the base commit. It
>   changes `registry.txt`, `frontend-mapping.tsv`, `SliceSourceDiagnosticCode`, the emission constants of the codes
>   revision 19 renumbers in the binder and the lowering, the code at each `JsParser` or binder site that serves one
>   rule alone and whose code revision 19 changes (a relation-split row's, or a split other code's; decision D12 names
>   the sites), and no other line of `JsParser`, the conformance root's grading and classification of the new codes,
>   the retained source corpus's cases for them and the corpus producer's mirror, the codes and texts the slice
>   compiler's `--checks` rows pin where revision 19 changes them (`BinderChecks.cs`, `BigIntChecks.cs` and
>   `SourceFrontEndChecks.cs`), and nothing else outside `docs/`. If its parent is not the base commit, part B's run
>   there is void and is taken again at a registry commit that meets this condition, before any after run of part B
>   is seen.
> - **Part A's base and after runs** are dispatched at those commits, at any time after each exists. **Parts B to J** are
>   taken locally from a clean checkout of each commit they name, after the after commit has merged.
>
> A part whose base was taken at any other commit, or whose commits fail those conditions, is void and is not judged.
>
> ## Part A - test262 parity, in both forms, on hosted runners (template T1)
>
> ```rule
> evidence class: conformance parity (not a measurement)
> population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant the wide manifest scores at each commit, once in the bytecode form and once in the native form; the negative syntax tests are in it, and a regression on any path fails this part
> configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the base commit and the after runs at the after commit; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
> completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log); the comparison's count of variants present in only one report is zero
> comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite; for every row the comparison reports whose class below turns on a diagnostic, the row is run again alone at both commits on the local machine this file's header names, one process of the conformance root over the row's path under the part's manifest and form, and the first diagnostic each run reports is retained; that is this bundle's per-variant comparison of early-error verdicts
> admitted difference classes:
>   C1 correction or regrade, of four kinds named before any run, each listed with its path, its variant, the first diagnostic at both commits and its kind: (R) a regrade by registry revision 19 - a negative variant whose phase is parse and whose type is SyntaxError, Unsupported at the base because its first diagnostic was 2104 for one of the three language rules JSD-0080's decision D13 names, and Passed after with the code revision 19 mints for that rule as its first diagnostic; (G) the decorator's stage - a negative variant Unsupported at the base because the tokenizer's 2104 at an @ came first, and Passed after because a language refusal earlier in the source text now comes first (decision D17); (B) the admission walk - a negative variant Unsupported at the base because a BigInt refusal JsParser made during the parse came first, and Passed after because a language refusal elsewhere in the source text, which now stops the unit before the admission walk, comes first (decision D18); (F) a correction of this family that step 1 makes, by one of the rows the correspondence names as the corrections SJL-13 delivers - T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32 and D40, from its columns "Correction of Broiler.VM" and "Correction of both" (decision D25) - a variant failing at the base that passes after, listed with that row; a row of those columns that step 1 does not correct (P26, D30, D61, R19 and P32's item 10, which the correspondence hands on) admits nothing; a C1 row of no kind is recorded in the bundle as a finding that the correspondence missed, and is a regression
>   C3 implementation limit - a variant refused after with the contract's nesting diagnostic at the tree-depth bound, at an expression of one of the four shapes whose left operand's spine the base did not charge (a binary chain whose first operand is a parenthesised chain, a template whose substitution holds one, a binary chain whose first operand is a member or call chain, and a member or call chain whose object is a parenthesised binary chain; the correspondence's row R14, decision D14); never kind (F); listed with its depth as the library reports it; expected empty
>   C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
>   C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's and no other build, test or run on the machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
> no C2: the compiler charges no fuel and the realm's charge for a compilation does not move, so an exhaustion verdict that moves on fuel is a regression
> no C5: no data, pin or policy moves at step 1
> every other difference is a regression, including a variant that passes at the base and fails, is exhausted or is unsupported after; a variant Passed at the base and Unsupported after; a variant Unsupported at the base whose verdict moves outside (R), (G) and (B); a positive variant refused after outside C3; and an internal defect on either side
> MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1, C3 or C4 and is listed, and the native pair meets the same conditions.
> NOT MET otherwise.
> ```
>
> **How it is read.** As bundle `sjl-11-001`'s part A. The bundle retains both dispatches' artifacts, each comparison's
> output, the table of every C1 and C3 row with its first diagnostics at both commits, and each (c') and (e) row with
> its re-runs. The floor verdicts the harness prints are retained and do not decide part A; a floor the regrades cross
> is re-based by hand only as decision D13 says, in the after commit, with its reason. The native floor's
> `atMost unsupported` row, which still holds the BigInt row the bytecode floor retired by hand on 2026-09-21, is
> re-set by hand to the bytecode floor's row in the after commit, with its reason (decision D13). Each native run
> exits 4, which completeness admits. A row the comparison cannot classify is a regression even when the totals
> improve. Kinds (B) and (F) are expected to list nothing: no input of the parse corpus reaches a BigInt refusal (the
> correspondence's F6), and no test262 file reaches a correction step 1 makes (its F19).
>
> ## Part B - artifact identity over the parse corpus (template T6)
>
> ```rule
> evidence class: artifact identity (not a measurement)
> population: every input of src/tests/corpus/parse-1/corpus.manifest as the base commit holds it, which includes the retained source corpus's inputs; the test262 inputs read from src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz and each checked against the manifest's SHA-256, the others from src/tests/corpus/parse-1/inputs/; in bundle sjl-12-001's sub-populations B1 (each input alone, under its goal and each of its variants, the wide manifest, the bytecode form and the native form under the backend x86-64-sysv on linux-x64), B2 (each test262 input as the conformance root composes it, bytecode form), B3 (each eval-family input as eval code under each of the request-flag sets E0 to E7, bytecode form) and B4 (each input compiled by the slice front end under its goal), with that rule's source names, module keys, variants, strict prologue and request flags; and B5, which the tools commit adds: each module-goal input through JsCompiler.Requests under the module options
> configuration: the slice compiler's --parse-corpus mode, as the base commit holds it and unchanged at the registry commit and at the after commit, built in Release from a clean checkout on the local machine this file's header names; for each input, sub-population, variant and form, the outcome (accepted or refused), the ordered diagnostics (code, line, column) and, for an accepted input, the artifact's SHA-256 and length, or in B5 the ordered specifiers; run at the base commit, at the registry commit and at the after commit, and with --census at the base commit and at the after commit; for every input whose hash differs between two of these runs, the artifact bytes at each are written and retained
> completeness: every manifest input reported in every sub-population, variant and form at each of the three commits, the manifest's digest at each run equal to the base commit's, and no run lost its process
> controls: a copy of the base report with one artifact hash changed, one outcome flipped and one diagnostic code changed to a code revision 19 does not give that input's rule, which the comparison must report as three unclassified differences; retained with its output
> comparison: eng/compare-parse-corpus.py, as the base commit holds it, joined on input, sub-population, variant and form: the outcome; for an input accepted at both commits, the artifact hash, or in B5 the ordered specifiers; for an input refused at both, the ordered diagnostic list; and the census output at both commits, byte for byte; each difference between the base and the after commit is attributed by the registry commit's run, which lies between them
> admitted difference classes:
>   R renumbered by registry revision 19 (decision D12) - an input refused at both commits whose diagnostic lists differ only in codes, each pair of codes being one the mapping's revision at the registry commit gives for that diagnostic's rule; the difference is already present between the base and the registry commit, and unchanged between the registry commit and the after commit; in any sub-population, B4 included; each listed with both lists
>   R2 consolidated at the parse (decision D12) - an input refused at both commits whose lists differ between the registry commit and the after commit only in codes, each pair being a code the mapping's revision at the registry commit marks a split other code reached at a site that serves several rules (JsParser's ValidateDeclaredNames, Expect, BindingName, ParsePrimary, ParseObjectEntry, ParseObjectBindingPattern and TryParseArrow sites, and the binder's check of an imported name also declared, as decision D12 names them), and the registry code of that diagnostic's row, which the library reports (at an arrow head its cover does not cover, EE0087's or EE0155's); and, for the binder's check of an imported name also declared, which reports at no place, in the place the library reports; in B1 to B3 and B5 only; each listed with both lists and the row
>   S stage - an input refused at both commits whose list at the after commit differs from the registry commit's only by: (S1) order - the same diagnostics, in source order after; (S3) suppressed - a diagnostic of the registry commit's list from the numeric-manifest admission, the binder, the lowering, or one of the policy and limit refusals JsParser made during the parse that the admission walk makes after (the BigInt refusals and EX0029's module specifier; decision D18), absent after because the parse refused first; (S4) unmasked - a diagnostic after that the registry commit did not report because a refusal at an earlier stage stopped the unit, with a code the mapping gives a syntax or request row of the stage table; (S5) moved - a regular-expression literal's refusal reported after at the parse, with the code and the span the registry commit's binder reported it with (decision D7); (S6) lexical - a lexical diagnostic of the registry commit's list, the whole-input tokenizer's 2104 at an @ included, absent after because a parse refusal earlier in the source text stopped the unit (decision D17); (S7) first refusal - the after commit's list holds one diagnostic, the library's first refusal (decision D11), and every diagnostic the registry commit's parse reported at or after that diagnostic's place in the source text, the cascade of the statement in progress included, is absent after; (S8) scope order - the after commit's diagnostic and the registry commit's first are two declared-name conflicts of one scope, the library taking that scope's conflicts in the source order of the conflicting declarations and JsParser in its own (decision D11); in sub-populations B1 to B3 and B5 only; each listed with both lists and its kinds
>   F a correction of this family that step 1 makes, by one of the rows part A's kind (F) names (T6, T7, T8, T13, T16, T24d to T24f, P05a, P15b, P31, D32 and D40) - an input whose outcome or first diagnostic moves between the registry commit and the after commit for that row's reason; the corpus holds inputs of P15b, and the others are shown by the correspondence's probes and part H's vectors; in B1 to B3 and B5 only; each listed with its row
>   L implementation limit (decision D14) - an input accepted at the base and refused after with the contract's nesting diagnostic at the tree-depth bound, at an expression of one of the four shapes part A's C3 names; never class F; in B1 to B3 and B5 only; each listed with the depth the library reports
> sub-population B4 and the census admit no class but R: the slice front end's outcomes, bytes and diagnostics, and the census, are otherwise the same at all three commits
> no ordering, template or position class: step 1 reorders no declaration, changes no template's reading, the adapter reproduces every anchor and offset, and the library places every diagnostic where this family's front end placed it at the base, except the binder's report at no place, which class R2 admits (decision D15)
> every other difference is a regression: any byte difference of an input accepted at both commits; a specifier list that differs for a B5 input accepted at both; an outcome that moves outside F and L; a diagnostic difference outside R, R2, S and F; any difference between the base and the registry commit outside R; and any difference in B4 or the census outside R
> MET if and only if all three runs are complete, the control is reported as stated, every difference is in R, R2, S, F or L and is listed, and B4 and the census show none but R.
> NOT MET otherwise.
> ```
>
> **How it is read.** As bundle `sjl-12-001`'s part B: identity is equality of SHA-256 over the artifact bytes, and bytes
> are retained only where hashes differ. The registry commit's run attributes every renumbering to revision 19 before
> the parse changes, so that class S can name codes as the registry commit gives them; only a consolidation at a
> `JsParser` or binder site that serves several rules waits for the after commit, and class R2 names it. From the after
> commit a refused input's list holds the library's first refusal alone, and classes S7 and S8 name what that removes.
> **Every accepted input's bytes are
> expected identical**; a byte difference anywhere is NOT MET. The refused-input lists and the per-code tallies at each
> commit are this bundle's diagnostics distribution (roadmap section 9), retained and read through R, R2, S and F.
>
> ## Part C - determinism and the source front end's gates (layer L13)
>
> ```rule
> evidence class: determinism (not a measurement)
> population: every input, variant and form part B's after run accepted in sub-populations B1 and B3; and the slice compiler's --checks rows "the wide lowering is deterministic", "the wide lowering declares lexical names in source order", "two goals on the wide path in one process do not reach each other" and "a deep-nesting corpus is refused on the wide path rather than survived", as the tools commit adds or holds them; and the row "a left operand's spine is charged to the chain it begins", which step 1 adds (the four shapes the correspondence's row R14 names, each past the tree-depth bound: a binary chain whose first operand is a parenthesised chain, a template whose substitution holds one, a binary chain whose first operand is a member or call chain, and a member or call chain whose object is a parenthesised binary chain; each refused with the contract's nesting diagnostic, the process alive)
> configuration: the slice compiler's --parse-corpus mode at the after commit, built in Release from a clean checkout on the local machine this file's header names, in three processes - run 1 with one worker, run 2 with one worker, and run 3 with two workers compiling every input twice concurrently, once on each; the first four checks rows at the base commit and at the after commit; the fifth at the after commit only, because the base compiles its inputs (bundle sjl-12-001's README; the correspondence's row R14)
> controls: three patches, each to a scratch copy of the after commit, never merged, retained with their output: K1 reverses the global lexical declarations - the source-order row must fail, and part B's comparison over the patched build must report at least one byte difference; K2 shuffles the global lexical declarations per compilation from an unseeded source - run 1 against run 2, or the two compilations of run 3, must differ, and the determinism row must fail; K3 shares one parse request between the two goals of the two-goals row - that row must fail; K4 removes the library's charge of a left operand's spine, through a scratch copy of the library packed into a scratch feed - the fifth row must fail; each patch is reverted and the revert checked against the after commit
> comparison: per input, variant and form, the four hashes - run 1, run 2 and the two of run 3
> admitted difference classes: none
> MET if and only if every accepted input gives one hash in all four compilations, the first four checks rows pass at both commits, the fifth passes at the after commit, and the four controls fail as stated and pass after their revert.
> NOT MET otherwise.
> ```
>
> **How it is read.** The fifth row's inputs at the base are part B's class L inputs where the parse corpus holds them;
> the row itself does not exist there. The two rows the tools commit adds are JS-2's "two parses with different goals run concurrently" and "a deep-nesting
> corpus refused rather than terminating the process" on the wide path, which no row held before the tools commit
> (decision D23). The first two rows are stage JSB-1's standing check; the profile's ledger judges JSB-1 (decision D34).
>
> ## Part D - the front end's rules (rule runs)
>
> ```rule
> evidence class: rules (not a comparison)
> population: rules N5, N6, N7, N8, N9, N12 and N19 (as the tools commit revises N19's pattern), I1 to I7, K4, A2, A7, B3 and N37 of src/tests/Broiler.VM.Architecture.Tests/rules.register.json, their tests, and the witnesses their register rows name
> configuration: dotnet test of Broiler.VM.Architecture.Tests in Release from a clean checkout of the after commit, with BROILER_RULE_MESSAGES set to a directory of the bundle, the test log and the rule-message files retained; the same at the base commit, retained and read, not judged; rule N37's transcript of the pristine package consumer at the after commit, and its control, the consumer restored without Broiler.ECMAScript.Frontend on its feed, which must fail with the missing-package error; rule I5's two controls, a re-added deleted member of JsParser and a re-added deleted binder member that checked a syntax row, each in a scratch copy, each of which must be refused
> comparison: none between commits
> MET if and only if every test of the rules named passes at the after commit; every witness they name is refused; N12's test counts the compiler assembly's files with a comment naming each file step 1 deleted and added; rule I3's register row names the frontend clause's subject; I5 registers every member the verdict's deletion list gives step 1, the binder's checks of syntax rows among them, member by member (decision D20); and all three controls behave as stated.
> NOT MET otherwise.
> ```
>
> ## Part E - the registry and its mapping (validation section 7's control)
>
> ```rule
> evidence class: record consistency (not a comparison)
> population: src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv and registry.txt at the after commit; the early-error catalogue the mapping names by Broiler.JS commit and SHA-256 (Broiler.ECMAScript/docs/frontend/early-errors.tsv); the stage table it names likewise (Broiler.ECMAScript/docs/frontend/stages.tsv); SliceSourceDiagnosticCode, LanguageErrors.Classified and Test262Run's grading at the after commit
> configuration: eng/check-frontend-mapping.py as the tools commit holds it, run at the after commit
> checks: the catalogue's and the stage table's digests equal the ones the mapping names; the registry states revision 19 and its owning decision; every catalogue row with a shared code has exactly one mapping row; every shared code maps to exactly one registry code the registry holds; no mapping row is marked split; a row marked within-meaning quotes the registry row's published meaning, which covers the rule; every code revision 19 adds has a member of SliceSourceDiagnosticCode, a registry row with its stage and reachability, a case of the retained source corpus that reaches it, a class in LanguageErrors.Classified, and the grading decision D12 gives it in Test262Run; every registry code is reached by a row or names the catalogue's engine-only row that explains it
> controls: three copies - one giving a shared code a second registry code; one leaving a split row in place; one removing a new code's LanguageErrors class - the checker must refuse each; retained with its output
> MET if and only if the checker passes at the after commit and refuses all three controls.
> NOT MET otherwise.
> ```
>
> ## Part F - the two tokenizers agree on the slice path's inputs
>
> ```rule
> evidence class: agreement (not a measurement)
> population: every input part B's after run accepted in sub-population B4, and every program of SliceSourcePrograms.Accepted, at the after commit
> configuration: the slice compiler's --checks row "the shared and slice tokenizers agree on the slice path's inputs", which step 1 adds, at the after commit; for each input it takes SliceTokenizer's stream of the text, and the token stream the library's parse of the text produced under the same goal with HTML-like comments off, as the slice path sets them (so that each / is read as the parser's goal read it), and compares them token by token through the row's fixed field map (decision D21): the kind, through the row's fixed kind map applied to the library's kind and escaped flag, an escaped reserved word mapping to an identifier; for an identifier, SliceToken's raw text against the library token's cooked name, and the text between SliceToken's offset and end against the library token's raw spelling; for every other token, the raw text; the numeric value of a numeric literal and the string value of a string literal; line, column, offset and end; the preceded-by-line-terminator and legacy-octal flags; the escaped flag of an identifier; and whether a / began a regular-expression literal; a template, one token in SliceTokenizer's stream, is compared with the library's run of template tokens for it (its head, middles and tail, each substitution's tokens between them) one to n: the template's span against the run's first start and last end, each cooked and raw chunk against the library's chunk, and each substitution's tokens token by token as above
> controls: a scratch copy of the after commit in which the library stream's token kinds are shifted at one punctuator before the comparison; the row must fail naming that input; reverted, and the revert checked
> admitted difference classes: none
> MET if and only if the row passes and the control fails as stated.
> NOT MET otherwise.
> ```
>
> **How it is read.** The milestone keeps the slice path on `SliceTokenizer` until SJL-15 and asks that the shared
> tokenizer's behaviour match it "which the corpus shows". This part has no base: the library is absent there. An input
> the slice path refuses is not compared, because its refusal may come before the slice tokenizer's stream is whole.
> `SliceTokenizer` keeps an identifier's cooked name as its raw text, gives an identifier no string value, and marks
> escapes on identifiers only (the correspondence's rows T31 and T32); the field map compares each fact where each
> tokenizer keeps it, and admits nothing else. The slice tokenizer's acceptance of white space inside an escape's
> hexadecimal digits (row T13) is a defect handed to SJL-15.4 (hand-off H23); an input that reached it would be a
> disagreement, which this part does not admit.
>
> ## Part G - source fuzzing on the wide path (layer L9)
>
> ```rule
> evidence class: robustness (not a measurement)
> population: the seeds docs/evidence/sjl-13-001/inputs/fuzz-seeds.txt names, as committed with this rule, for two sessions - the wide session, each seed with its goal, its eval-flag sets and its count of mutants, and the slice session (--fuzz), each with its count - and the wall bound per compilation that file states
> configuration: the slice compiler's --fuzz-wide mode, as the tools commit holds it, and its --fuzz mode, built in Release, at the after commit on the local machine this file's header names, and the same sessions at the base commit, retained and read, not judged; in the wide session each mutant compiled through JsCompiler.Compile on CompilationStack under its goal, and for eval-family seeds as eval code under each named flag set; the slice session as SourceFuzzing runs it, through SliceTokenizer, whose wide-only members step 1 deletes
> reading: each mutant is accepted, or refused with diagnostics that are each a SliceSourceDiagnostic with a code the registry holds; a finding is an unhandled exception, a process that ended without its exit code, a refusal whose code the registry does not hold, or a compilation past the wall bound
> controls: a scratch copy of the after commit in which the adapter throws on one node kind the wide seeds produce, and one in which SliceTokenizer throws on one token kind the slice seeds produce; each session must report a finding under its control; reverted, and each revert checked
> MET if and only if neither of the after commit's sessions reports a finding and both controls are reported as stated.
> NOT MET otherwise.
> ```
>
> **How it is read.** No Broiler.VM session reached the wide parser before the tools commit; the slice session reached
> `SliceTokenizer`, which step 1 removes from the wide path (decision D22). The home's fuzz target is Broiler.JS's
> bundle's. A finding is a defect fixed before the cut-over is read, with a new after commit and new runs.
>
> ## Part H - the syntax vectors at engine level (template T3)
>
> ```rule
> evidence class: conformance (not a measurement)
> population: every vector of the syntax area at the corpus revision src/tests/vectors/pins pins at the base commit, which is the revision that classified both engines' first runs over the syntax vectors (the revision after the one bundle sjl-13-001's home rule names as its vectors commit's), in schema 1 (decision D25)
> configuration: this family's engine-level vector runner, the conformance root's vectors command as the base commit holds it and unchanged at the after commit, built in Release, at the base commit and at the after commit; each vector's script evaluated and rendered as conformance/vectors/SCHEMA.md section 7 says, and its verdict - match, classified-divergence or difference - given as section 8 says, an impl-defined vector against this engine's per-engine field and a divergence field naming this engine classifying a difference; no registry code is read
> controls: a copy of one vector's expected outcome changed to this engine's opposite answer; the runner must report it as a difference; retained
> comparison: per vector, the verdict at the base against the verdict after
> admitted difference classes: C1 - a vector whose verdict is difference or classified-divergence at the base and match after, listed with its row of the correspondence; a divergence field that no longer holds is retired by the next corpus revision (decision D25)
> every other change is a regression, including a vector that matches at the base and does not after
> MET if and only if every vector's verdict after is match or classified-divergence, every change is in C1 and is listed, and the control is reported.
> NOT MET otherwise.
> ```
>
> **How it is read.** Schema 1 carries no goal, switch or code, so the vectors judge outcomes of Script and eval-code
> sources only; first codes are judged by part B's diagnostic lists and part A's first diagnostics, and module-goal
> sources by part B's B1 and B5. Broiler.VM's runner is used as the base commit holds it, so no product change carries
> the vectors. The lexical and `syntax` vectors carry the corrections step 1 makes that a Script or a direct eval can
> carry (rows T6, T7, T8, T13, T16, T24d to T24f, D32 and P31; decision D25), so this part's C1 names each with its
> row; P05a and D40, which only the Module goal reaches, are shown by the correspondence's probes.
>
> ## Part I - the fuel-parity twins (template T7)
>
> ```rule
> evidence class: resource accounting (not a measurement)
> population: every *.small.js twin in src/tests/forms/wide, as committed at the base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
> configuration: the command-line composition root built in Release, on the local machine this file's header names; at the base commit, each twin's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-13-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
> ceilings tried: for each twin, its base figure and one less
> comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
> admitted difference classes: none; the compiler charges no fuel, so a moved figure is a byte difference part B also reports, or a defect
> MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
> NOT MET otherwise.
> ```
>
> ## Part J - this family's compile measurement (template T5)
>
> ```rule
> evidence class: measurement
> population: the workloads of docs/evidence/sjl-13-001/inputs/workloads as committed with this rule, each a source the wide path accepts at the base with its goal, each with its control, the *.control.js file beside it of the same goal; the bytecode form; the slice compiler's --measure-compile mode as the tools commit holds it, built in Release under JIT; the local machine this file's header names, and one RID, both named in the sub-bundle's manifest
> arms: B the base commit's build; A the after commit's; A-again the A arm measured a second time under another lane name, the A/A lane; each arm's binaries named by hash in the manifest, which is written and made read-only before any lane runs
> configuration: the core's measurement rules (docs/baselines.md section 1, restated by roadmap.gates.md section 17): for each workload, in every repetition, B, A and A-again each compile the workload and its control, interleaved inside the repetition; repetitions the count docs/baselines.md rule 4 fixes per lane, every one retained; warm-up repetitions as eng/compare-forms.py fixes them, retained and named as excluded from Q; no other build, test or run on the machine
> condition: each lane's outcome and artifact hash for each workload equal the base arm's, checked before and after every lane; a lane that differs fails the condition
> configuration check: every timed child is run with --runtime, and its effective-configuration line is identical across arms, lanes and repetitions
> quantity: Q(workload, arm) = median over retained repetitions of the workload's compile wall time
> noise floor: N(workload) = |Q(workload, A) - Q(workload, A-again)|
> comparison: D(workload) = Q(workload, A) - Q(workload, B), against N(workload); one engine's after arm against the same engine's base arm, never against another engine
> attribution, retained beside D and deciding nothing: E(arm) = Q(workload, arm) - Q(control, arm)
> admitted difference classes: none; a measurement admits no class, and a difference within the A/A floor is below resolution rather than admitted
> MET if and only if all of: every lane's condition passes, before and after, for every workload; every effective-configuration line is identical; for every workload, D <= N.
> NOT MET otherwise.
> readings: a workload whose |D| <= N is below resolution; this rule states no percentage; a failed condition is a NOT MET, not a re-run; an interrupted collection may be run again from the start, with the stopped run retained beside it and named as stopped; with several workloads, a NOT MET under no change is likely, and the bundle reads it so
> ```
>
> **What part J decides, and what it does not.** Whether the wide path compiles more slowly through the adapter than
> through `JsParser`, beyond the A/A floor, on this machine. It is not a gate clause (decision D24, as JSD-0081's
> decision D19 read SJL-11's). A NOT MET is recorded with its failing workloads and is not read as a finding that the
> library is slower. It decides no speed claim and nothing about another machine.
>
> ## Revisions
>
> A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
> and says it was written afterwards, and its decision is then weaker evidence. **Before step 1's code lands, a later
> bundle shows this file unchanged.**
>
> ## What this file does not decide
>
> - **The home clauses** (1 to 3): Broiler.JS's bundle `sjl-13-001` decides them.
> - **Step 2**, and clauses 4 to 6 at step 2: bundle `sjl-13-002`.
> - **Clause 8's publication** of every root in three modes, PolyglotCli's included (it has no CI lane), with the
>   Android root under exclusion EX-126: read from the collector's retained runs, closures and mstat files.
> - **Clause 10, JS-2:** the profile's ledger closes it on this bundle and `sjl-13-002` (decision D33).
> - **The differential cross-check** (template T8): its report over `src/tests/differential/` is retained beside these
>   parts without a rule, as bundle `sjl-3-001` retained its own.
> - **A speed claim of any kind.**
