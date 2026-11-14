<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-002 - the shared syntax library and the wide path, step 2: Broiler.VM's rule and evidence

**Milestone:** SJL-13 of the shared JavaScript libraries programme,
[`sjl-13-shared-syntax-and-vm.md`](../../javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md), at
the second of SJL-13.3's two cut-over steps, in which the lowering, the numeric-manifest admission and the binder read
the shared tree directly and the adapter and `JsSyntax` go. Bundle [`sjl-13-001`](../sjl-13-001/README.md) judges the
first step and holds the correspondence both rest on.

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-13.1 with bundle `sjl-13-001`'s rule,
before any change either judges existed.

**Records:** [JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
the area verdict; its decision D19 reads step 2.

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules, with the readings taken before any run; nothing taken. The rules are of 2026-10-26, at SJL-13.1,
and the readings of 2026-11-13, at SJL-13.3's step 2 (section 1). **Nothing here is a verdict, and no run of the rule
has been taken.** A collector joins this bundle when there is a run to take. *(Revised 2026-11-13, when section 1's
readings taken before any run were filed. It read "rules only, 2026-10-26, at SJL-13.1. **Rules only. Nothing here is
a verdict, and nothing has been taken.**")*

*(Added 2026-11-14, at step 2's records, before any run of the rule.)* **Step 2 is built, and its runs are not
taken.** Its branch, `sjl-13-3-step2`, is cut from the default branch's `389b55c9ec9bb77409bb2487067fbebc0868849b`
(Broiler-Platform/Broiler.VM#205, the merge that moved this family to the shared stream's `0.1.0-preview.11` and
carried section 1's readings and the rule's revision of 2026-11-13), and lands as one merge, which is the after commit;
the merge's first parent is the base. `389b55c9ec9bb77409bb2487067fbebc0868849b` descends from the rule commit and from
bundle `sjl-13-001`'s after commit `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`, and the branch's first commit,
`db7a6f0eaf516d6113c2d11c986fe3232be9f6df`, is directly on it. The base and the after commit are named here, by all
forty digits, when the merge lands. This holds only while the default branch is still `389b55c9` when the branch
merges: if anything lands on it first, the merge's first parent is not `389b55c9`, and the branch is built again on
the new tip, with these records, before it merges; reading 1 holds the default branch meanwhile. The branch's commits
from `db7a6f0eaf516d6113c2d11c986fe3232be9f6df` to `c452294b3ca1fefa9aa5e3db41ab5e3264903d25` move the admission walk,
the numeric admission, the binder and the lowering onto the shared tree, delete `JsSyntax.cs` and `JsFrontendAdapter.cs`
with its parts, and register what they deleted in rule I5; step 2's records follow them and change no product source.
[JSD-0087](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0087-the-wide-path-parses-with-the-shared-syntax-library.md)'s
dated note of 2026-11-14, the cut-over record's note at step 2, lists them. The development checks the branch was built
against, at the base and at its commits, are not evidence: they are kept outside this repository, are no run of the
rule and decide nothing (reading 6), and no figure of them is quoted here. Of them this README says only that the
development checks' test262 comparison found only Exhausted rows whose allowance moves between the wall clock and fuel,
as the base build's own re-runs do. Every part is taken after the merge, at the base and the after commit: part A by
the hosted workflow, dispatched for both, and parts B to G locally. Nothing here is a verdict yet.

## 1. SJL-13's area rule for step 2 (SJL-13.1)

**Step 2 changes no outcome, no byte and no diagnostic** (JSD-0083 decision D19). The rule therefore admits no class
but part A's harness classes.

**The commits the rule names.**
- **The rule commit** carries [`decision-rule-area.md`](decision-rule-area.md). It is named in this section, by all
  forty digits, when the change that carries it has merged. *(Note of 2026-11-13, late, at SJL-13.3's step 2: the
  sentence "It is named in this section, by all forty digits, when the change that carries it has merged." is
  fulfilled here. That change, the SJL-13.1 records, merged in Broiler-Platform/Broiler.VM#172 as
  `faa8af78cddc32d460a7299f088b18b9e4407d7a`, the rule commit. The commit of that change's branch that added the file
  is `ef3f0567a5eba0dca7fa4fc584bd53e94ce81281`, and `af2420b292c3cbcd5476943ce92c8798f6aeb847`, on the same branch
  and before the merge, revised it (part B's sub-population B5). The file's blob is
  `a6b2a086c5775b3e4ca7450da530bb9510bc5095` at the rule commit, at bundle `sjl-13-001`'s after commit
  `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`, and at `686533fad10a17600ce67e5a774892f62cb08006`, the default branch's
  head from which step 2's branch was cut. This section did not name the rule commit when #172 merged, as it should
  have; the note names it on the branch whose merge is to be the after commit, and section 1's readings taken before
  any run record that choice.)*
- **The base commit** and **the after commit** do not exist yet. The after commit is the default-branch merge that
  lands step 2, and the base commit is its first parent, which descends from the commit bundle `sjl-13-001` names as
  its after commit. This README names each, by all forty digits, when it lands.

**The rule has seven parts**, each deciding alone: part A, test262 in both forms on hosted runners (template T1), with
class C4 only; part B, artifact identity over the parse corpus's sub-populations B1 to B5, B5's ordered module
specifiers among them, with no class (template T6); part C, determinism and the five checks rows of bundle
`sjl-13-001`'s part C (layer L13); part D, the front end's rules, with rule I5 registering every member of `JsSyntax`
and of the adapter; part E, the fuel-parity twins (template T7); part F, this family's compile measurement over bundle
`sjl-13-001`'s workloads (template T5), not a gate clause; part G, source fuzzing over bundle `sjl-13-001`'s seeds,
both sessions (layer L9).

**The pins** are bundle `sjl-13-001`'s.

**Part B's comparison reads B5 since 2026-11-01.** `eng/compare-parse-corpus.py`, which part B runs as the base commit
holds it, refused every report holding sub-population B5 until the change of 2026-11-01 before step 1's base (bundle
`sjl-13-001`'s README, section 1, names it). Since that change it compares an accepted B5 row on its ordered
specifiers, its note, with its hash and length, and refuses a report whose B5 note does not have the hash and length
its row states. Its `control` mode makes this part's control as filed with `--column --specifier with-hash`: one
artifact hash changed, one diagnostic's column moved, in place of the outcome flip bundle `sjl-12-001`'s control makes,
and one specifier of a B5 input changed with its hash and length, which the comparison reports as three differences,
the third a `specifiers` one. With `--specifier note-only` the specifier changes alone and the report is refused.
*(Added 2026-11-01.)*

### Readings taken before any run (2026-11-13)

*(Added 2026-11-13, at SJL-13.3's step 2.)* These readings are filed on the branch whose merge is to be the after
commit, before any run of step 2 that the rule judges and before the development reference that reading 6 names was
taken. Each is recorded as the owner's dated decision under the owner's standing direction of 2026-10-18 ("Please do
all recommended decisions and skip waiting for review"), taken as the recommended choice, with the alternative not
taken; none is the owner's own dated answer, asked and answered, and the owner may rule otherwise. They change no part,
class, control or condition of the rule. Part D's clause on rule I5, and its control, are revised by
[`decision-rule-area-revised-2026-11-13.md`](decision-rule-area-revised-2026-11-13.md), filed on the same branch before
any run, on the reading of decision D20 that JSD-0083's dated note of 2026-11-13 records.

1. **Every run of this bundle is taken after step 2's merge.** The base's runs and the after commit's, part A's
   dispatches among them, are taken once step 2 has merged. Nothing else merges into Broiler.VM's default branch until
   this bundle's evidence merges, so that no other change lands between the base and the after commit, or after the
   after commit while this bundle's runs are taken, and part A's dispatched ref holds the base's workflow and driver. If step 2 has to land in a second
   merge, both merges are named in this bundle, the base stays the first merge's first parent, and every after run is
   taken again at the last, as the rule's "The commits" says. Not taken: "take the base column now", at
   `686533fad10a17600ce67e5a774892f62cb08006`, before step 2's merge.
2. **The rule commit is named on this branch**, by the note above: `faa8af78cddc32d460a7299f088b18b9e4407d7a`, the
   merge of #172, which carries `ef3f0567a5eba0dca7fa4fc584bd53e94ce81281`, with the rule file's blob
   `a6b2a086c5775b3e4ca7450da530bb9510bc5095`. Not taken: "a separate docs merge before the base", which would have
   named it on the default branch before step 2's branch.
3. **Part D's clause on rule I5 is revised**, by the revision named above. Its control is pinned: a scratch copy of
   the after commit declares the type `JsMemberExpression` again, with its member `NameSpan`, and must be refused; a
   second control declares `JsNumberLiteral` again. The revision records its alternatives.
4. **Which of bundle `sjl-13-001`'s readings carry.** That bundle's section 5 took readings 1 to 11, each now the
   owner's own dated answer by the rulings of 2026-11-11 and 2026-11-12. For this bundle:
   - **Reading 10 carries**: the rule's local machine is this family's machine as replaced, each run recording its
     SDK.
   - **Reading 2 carries**: part C's control K4 is applied as configured, and the adjunct, the same patch under a
     harness narrowed to the fifth row, is used only if K4 as configured fails to make the fifth row fail.
   - **Readings 1, 3, 4, 5, 6, 7, 8 and 11 have no object at step 2.** They read part A's kind (F) of class C1, part
     B's classes R, S and N, part H and the registry commit. This rule's part A admits only class C4, its part B
     admits no class, and it has no part H and no registry commit.
   - **Reading 9 does not carry.** Every local part of this bundle is taken with no compiler server alive, as the idle
     gate reads it.
   - Not taken: "rule each again", which would have asked the owner on each before step 2's runs; and "carry them
     all", which would have carried reading 9's idle servers into this bundle.
5. **Part F, a reading; there is nothing to decide.** Part F is no gate clause (decision D24). The owner's words of
   2026-11-03, "Please defer the measurement", answered decision D48 of SJL-13's plan, on SJL-14's clause 6, for
   Broiler.JS (`docs/javascript-shared-libraries/decisions.md`, decision SJL-D-23's dated note of 2026-11-03; the
   milestone `sjl-14-broiler-js-syntax.md`, its note of that day), not this rule's part F; and the owner's ruling of
   2026-11-12 on bundle `sjl-13-001`'s reading 9 let that bundle's part J, taken after 2026-11-03, stand. Part F is
   taken after step 2's merge, as the rule's B and A arms name the base's and the after commit's builds, and is read as
   no gate clause.
6. **Development checks before the merge** are taken from scratch archives, kept outside this bundle and never cited
   by it, and decide nothing. They are not runs the rule judges. The development reference taken at
   `686533fad10a17600ce67e5a774892f62cb08006` after these readings and the revision above were filed is such a check:
   it is not a judged run, nothing of it is under `docs/evidence/`, and it is not this bundle's base. Any revision of
   the rule filed after that reference states that it was written after the reference was seen. Not taken: "no checks
   before the merge".
7. **Where the facts are computed.** At step 2, the top-level-`await` fact, the BigInt values and strictness are
   computed in the compiler assembly's post-parse code, by `JsParser`'s rule: the admission walk (`JsAdmissionWalk`)
   computes the flag and the BigInt values, and a side table keyed by the shared tree's nodes holds strictness. This
   reads JSD-0083's "the lowering's at step 2" (X6, decision D15 and decision D20), and the rule's "The lowering
   computes from the shared tree, by `JsParser`'s rules, every fact the adapter computed at step 1", as that code;
   JSD-0083's dated notes of 2026-11-13 on X6, D15 and D20 record it. Not taken: "compute them in `JsCompiler`'s own
   files", as JSD-0083's text reads.

*(Noted 2026-11-13, later, when the shared stream's `0.1.0-preview.11` was taken before step 2.)* The passage above
reads "These readings are filed on the branch whose merge is to be the after commit, before any run of step 2 that the
rule judges and before the development reference that reading 6 names was taken"; reading 1 reads "Nothing else merges
into Broiler.VM's default branch until this bundle's evidence merges"; reading 6 reads "The development reference taken
at `686533fad10a17600ce67e5a774892f62cb08006` after these readings and the revision above were filed is such a check";
section 1's note on the rule commit reads "the note names it on the branch whose merge is to be the after commit"; and
the revision's "Written" line reads "It is filed on the branch whose merge is to be this rule's after commit, before
that merge", with "Step 2 is built on the branch `sjl-13-3-step2`, cut from
`686533fad10a17600ce67e5a774892f62cb08006`". The owner published the shared stream's `0.1.0-preview.11` on 2026-11-13,
and its move was taken first, in its own merge, before step 2's base, as the owner's dated decision under the standing
direction of 2026-10-18, taken as the recommended choice (not taken: "step 2 on preview.10, the stream move after step
2's evidence merges"), because step 2's code reads the shared syntax library's tree directly and the stream's new
version changes that library's sources (in comments only, so its code is `0.1.0-preview.10`'s; the ledger's entry of
2026-11-13 records the move's own local checks). *(Revised 2026-11-13, later still: the parenthetical read "(in
comments only, so its code is `0.1.0-preview.10`'s, and the parse corpus's report over B1 to B5 and both census
outputs are byte for byte the same before and after the move, in local runs from clean archives that are no part of
this rule)". Those words are withdrawn: reading 6 keeps development checks outside this bundle and never cited by it,
and this bundle cites none of the move's runs.)*
- **Where these readings land.** The two commits that filed this section's readings and the revision,
  `233b6629c5bac0ca921dd82c9ccbc8aa1ac01d11` and `90c4d07cfb38d916bcc804c17b2f5726a01c1933`, land in that change's
  merge, with the move, before step 2's base and before any run of this rule. They are not on the branch whose merge is
  the after commit, and the sentences quoted above that say so read as of the day they were written.
- **The base.** Step 2's base is the first parent of step 2's own merge, as the rule's "The commits" says, and that
  merge descends from the move's. The base therefore holds the move, the readings and the revision; the after commit is
  still the only first-parent commit that carries any part of step 2. The move is no part of step 2, and changes none of
  this rule's pins, which are bundle `sjl-13-001`'s: test262, ECMA-262, the parse corpus's manifest, the early-error
  catalogue and the registry.
- **Reading 1** reads from step 2's branch point: the move's merge, which carries the readings themselves, lands before
  it, and the reading's purpose, that no other change lands between the base and the after commit, or after the after
  commit while this bundle's runs are taken, is unchanged.
- **Reading 6's reference.** No development reference was taken at `686533fad10a17600ce67e5a774892f62cb08006`: its
  builds were stopped when the owner's publication arrived, and nothing of them is seen or cited. Step 2's stage 1 takes
  its development reference at step 2's branch point instead, the default branch's head once the move has merged, under
  reading 6 as written otherwise.
- **The revision file is not edited.** Rule files, revisions included, are never edited once filed, so its "Written"
  line keeps its words, and this note is its dated record.

No part, class, control or condition of the rule changes, and no part is voided or judged otherwise by the move landing
before the base. The readings above are left as written.

*(Noted 2026-11-13, later still, on review of the note above, which is read with these corrections and additions.)*
- **Whose choice it is.** The order above, and the reading of reading 1 below, are each taken under the owner's
  standing direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review") as the
  recommended choice, and recorded as the owner's dated decision under that direction, with the alternative not taken;
  neither is the owner's own dated answer, asked and answered, and the owner may rule otherwise, as for the readings
  above.
- **Reading 1.** It reads "Nothing else merges into Broiler.VM's default branch until this bundle's evidence merges".
  It is read from step 2's branch point, the default branch's head once the move has merged: the move's merge, which
  carries the readings themselves, lands before that point and is no part of step 2, and the reading's purpose, that
  no other change lands between the base and the after commit, or after the after commit while this bundle's runs are
  taken, is unchanged. Not taken: "reading 1 as written when `233b6629c5bac0ca921dd82c9ccbc8aa1ac01d11` filed it",
  which would forbid the move's merge and hold the move until this bundle's evidence merges.
- **Reading 2.** It reads "The rule commit is named on this branch" and "Not taken: "a separate docs merge before the
  base", which would have named it on the default branch before step 2's branch". Section 1's note on the rule commit
  also reads "at `686533fad10a17600ce67e5a774892f62cb08006`, the default branch's head from which step 2's branch was
  cut". Under the order above, the note that names the rule commit lands on the default branch in the move's merge,
  before step 2's branch is cut, which is in substance the alternative reading 2 did not take. That follows from the
  order decision, not from a new reading of reading 2. The rule commit is as the note names it, and the rule file's
  blob, `a6b2a086c5775b3e4ca7450da530bb9510bc5095`, is the same at this change's head, which does not touch the file.
  `686533fad10a17600ce67e5a774892f62cb08006` is the head from which the branch carrying the readings was cut; step 2's
  branch is cut from the default branch's head once the move has merged. The note above's "read as of the day they
  were written" covers these sentences too.
- **No run of step 2 was taken**, and no development reference: nothing of either is seen or cited here.
- **What the revision read at `686533fad10a17600ce67e5a774892f62cb08006`.** The move changes no source file of this
  repository: no `.cs` or `.csproj` file changes between that commit and this change's head. The files the revision's
  "What was seen, and where" reads, `I5RetiredDuplicatesRuleTests`, `JsSyntax.cs` and `JsFrontendAdapter*.cs`, are
  the same at this change's head, so what it read holds at step 2's branch point once this change has merged with
  nothing between.

The note above and the readings are left as written, but for the parenthetical revised in place.

## 2. What this bundle will hold

- **`base/`** and **`after/`**: part A's dispatches, imported unchanged with the digest of each zip; parts B to E and
  G at each commit; part A's comparisons and C4 re-runs.
- **`controls/`**: part B's planted differences, part C's K1 to K4, part D's I5 control and part G's planted throws,
  each with its output.
- **`measurement/`**: part F's sub-bundle, with its own README and manifest.

**No figure is quoted in this README until its bundle retains the log it comes from**, and then only in the
section that reads that log (rule H5). A test262 suite total is never quoted.

## 9. Exclusions

Sections 3 to 8 are added as the bundle's runs are taken. The exclusions keep this section's number, where rule H2
reads an exclusion's definition. *(Added 2026-11-14: EX-135, for the code step 2 moved out of this component's
assurance coverage, joins the table, and the row of SJL-13.1 is kept as written.)*

| ID | Status | Exclusion |
|---|---|---|
| (none) | | No exclusion is defined at SJL-13.1, and none is planned. A later exclusion takes the next free identifier and is defined in this table |
| EX-135 | Open | **The adapted tree and its adapter, which left this component's assurance coverage** *(added 2026-11-14, at step 2's records, as JSD-0087's dated note of that day records)*. The types rule I5 registers for SJL-13.3's step 2 (`src/tests/Broiler.VM.Architecture.Tests/retired-members.json`) left the JavaScript family's sources at step 2: `JsSyntax.cs`, `JsFrontendAdapter.cs`, `JsFrontendAdapter.Expressions.cs`, `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs` whole, with every type in them. What the admission walk, the numeric admission, the binder and the lowering read in the adapted tree's place is `Broiler.ECMAScript.Frontend`'s own tree, namespace `Broiler.ECMAScript.Syntax`, which the wide path has parsed with since step 1. The home's own assurance covers it in the Broiler.JS repository: its generated manifest, `Broiler.ECMAScript/hairs.manifest.json`, lists every unit of the library's assembly with an annotation on each relevant declaration, checked in CI (rule SL8). No rule of this component reads those units, their human review is the home's and not `HUMAN_REVIEW.md`'s, and this component's assurance figures no longer count the adapted tree's or the adapter's units. The request and the side tables it hands on (`JsFrontend.cs`), the readings of the shared tree (`JsTreeFacts.cs`, which holds `FindConstructor` and `Unwrapped`, members rule I5 registers as `JsCompiler`'s), the admission walk (`JsAdmissionWalk.cs`), the diagnostics and their texts, the providers, the numeric admission, the binder and the lowering stay in this component and stay covered. EX-125 and EX-127 to EX-134 say the same of the earlier areas |
