<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-15-001's area rule, revised on 2026-11-16: the binding table read through one product member, the slice path's refusals put to the owner by class, rule I5's register by type and by member, the rows a window deletes, and the precedents' parts named

**Written:** 2026-11-16, before any run this rule judges and before any code it judges exists. It is filed on the
branch `sjl-15-1-records`, with the rule it revises, before that branch merges, and so before the rule commit, the tools
commit, the pin commit, or the base or after commit of either window exists. This file revises
[`decision-rule-area.md`](decision-rule-area.md) as that file's section "Revisions" requires ("A revision filed before
any run of the part it revises ... quotes this file whole and says so"). It is a new dated file, it quotes that file
whole at its end, and that file is not edited. In this file, "this revision" is this file. It is the first revision of
this rule. The rule commit, as the rule names it, is the commit that carries both files.

**Whose choice it is.** No owner's decision of its own is behind this revision, and it takes none of the owner's
questions. A review of the rule before any run found the five matters below. Where a matter needed a choice, the choice
is recorded in [JSD-0088](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0088-static-semantics-move-to-the-shared-frontend.md)
as a decision of 2026-11-16 taken as recommended under the owner's direction of 2026-10-18 ("Please do all recommended
decisions and skip waiting for review"), with its alternative, and not as the owner's own dated answer: decision
D-tables (matter 1). Matter 2 widens the question JSD-0088's decision D-census puts to the owner, and answers nothing
in it. The owner may rule otherwise on each.

**What was seen, and where.** Read at Broiler.VM `a86b6f5d`, whose product source the branch does not change:
1. **The binding table has no public producer the tools may call.** The slice front end's public result is
   `SliceCompilation(byte[]? Artifact, IReadOnlyList<SliceSourceDiagnostic> Diagnostics, bool IsStrict)`
   (`src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs:33-36`), which carries no binding table. The
   table is the answer of `SliceStaticSemantics.Validate(SliceProgram)` (`SliceStaticSemantics.cs:204`), a
   `SliceBindingTable` whose `Resolutions` are keyed by `SliceNode` (`:66-69`), each `SliceBinding(Name, Kind, Slot)`
   (`:28`) with its kind a `SliceDeclarationKind` that `SliceSyntax.cs` declares (`:136`). Inside the front end the
   program is built by `new SliceTokenizer(source)` and `new SliceParser(tokens, options)`, in the private
   `CompileOnTheDeclaredStack` (`SliceSourceCompiler.cs:209`, `:217`). The rule's tools commit may change only the
   slice compiler composition, `docs/` and `eng/compare-parse-corpus.py`, so its report code could build the table only
   by calling `SliceTokenizer`, `SliceParser` and `SliceProgram`, which window 2 deletes (decisions D60 and
   D-slicetree). Window 2's merge would then have to rewrite that code, and parts B, S and C, which run the tools code
   "as the tools commit holds it and unchanged at each after commit", could not be configured at window 2.
2. **The slice path's refusals move for more inputs than hand-offs H23 and H25 reach.** `SliceStaticSemantics` reports
   every refusal it finds, in walk order, under its own codes (correspondence row SL3). JSD-0088's window-2 deletion
   list deletes its checks of rows the library makes: `CheckBindingName`, the duplicate and collision checks, the
   legacy-octal and constant-initialiser checks and the assignment-target check, whose codes are
   `ReservedWordAsBinding`, `DuplicateLexicalDeclaration`, `VarAndLexicalCollision`, `LegacyOctalInStrictCode`,
   `ConstWithoutInitialiser` and `InvalidAssignmentTarget` (`SliceSourceDiagnostics.cs`). The library reports its
   first refusal, by place, under its own rows (rows EE2 and DE3; JSD-0083's decision D11), and it refuses before any
   tree reaches the slice static semantics. The census counts parse failures by the first slice diagnostic's code
   (row SL5), and a file `SliceParser` parsed that the library refuses at the parse moves it (finding F5). The rule's
   window-2 bullet names only the inputs H23 and H25 reach as expected to differ, and JSD-0088's decision D-census asked
   only about those, while row SL3's last cell says D-census puts to the owner "what that means where the library
   refuses first".
3. **Rule I5's reader and register.** `I5RetiredDuplicatesRuleTests.Declarations` (lines 143 to 177 at `a86b6f5d`)
   yields each type declared directly in a namespace or a compilation unit, and, of each type that can hold members,
   its methods, properties, events, fields, nested types and delegates; it yields no constructor, no positional
   parameter of a record and no member of an enumeration. `Register` (lines 124 to 131) reads the register with
   `ToDictionary`, keyed by name, so a second row with a name already present fails the read. Almost every record
   `SliceSyntax.cs` declares is positional (lines 28 to 236, `SliceIdentifierReference(SliceSourceSpan Span, string
   Name)` among them). The rule's part D asks I5 to register the window's deletions "member by member", the words
   bundles `sjl-13-001` and `sjl-13-002` had to revise for the same reason (their revisions of 2026-11-10 and
   2026-11-13).
4. **A row the window deletes.** The slice compiler's row "the shared and slice tokenizers agree on the slice path's
   inputs" (`TokenizerAgreementChecks.cs`) runs every program of `SliceSourcePrograms.Accepted` (`:235`) and the parse
   corpus through `new SliceTokenizer(text).Tokenize()` (`:344`, `:579`, `:658`). `SliceTokenizer` is deleted at window
   2, so that row cannot exist at window 2's after commit. Part S's MET line asks that "every --checks row passes at both
   commits", and part H admits only "a row the window's merge revises".
5. **The precedents' parts.** The rule names "bundle `sjl-13-001`'s part X" without saying which of that bundle's seven
   rule files it reads, and its header's list "part G's seeds and both fuzz sessions; part H's engine-level vector
   runner; part I's fuel-parity twins; part J's workloads" uses bundle `sjl-13-001`'s letters under a sentence that
   opens "From bundles `sjl-13-001` and `sjl-13-002`"; this rule's own letters for the same parts are F, E, G and J, and
   bundle `sjl-13-002`'s are G, (none), E and F.

## What changed

1. **The header, "SJL-13's and SJL-14's rules, applied to the semantic half", its first item.** It reads: "**From
   bundles `sjl-13-001` and `sjl-13-002`** (this family's): part A's population, configuration, comparison and C4
   classes; part B's tool, sub-populations B1 to B5, control and reading of refused inputs; part C's determinism and its
   controls; part D's rule runs; part G's seeds and both fuzz sessions; part H's engine-level vector runner; part I's
   fuel-parity twins; part J's workloads and measurement; each applied at SJL-15.3's and SJL-15.4's windows, and each
   named below where it applies." It now reads: "**From bundle `sjl-13-001`** (this family's), each part by that
   bundle's letter: its part A's population, configuration, comparison and C4 classes (this rule's part A); its part B's
   tool, sub-populations B1 to B5, control and reading of refused inputs (this rule's part B); its part C's determinism
   and its controls (this rule's part C); its part D's rule runs (this rule's part D); its part G's seeds and both fuzz
   sessions (this rule's part F); its part H's engine-level vector runner (this rule's part E); its part I's fuel-parity
   twins (this rule's part G); and its part J's workloads and measurement (this rule's part J); each applied at
   SJL-15.3's and SJL-15.4's windows, and each named below where it applies. **From bundle `sjl-13-002`**: the reading of
   part B's identity and its retained bytes, named with that bundle where it applies. **Which text of each bundle**:
   bundle `sjl-13-001`'s parts are read as its rule reads with all six of its revisions, the last of 2026-11-10
   (`decision-rule-area-revised-2026-11-10.md`), and bundle `sjl-13-002`'s as its rule reads with its revision of
   2026-11-13; every revision of either was filed before any run it judged. Wherever this rule names a part of another
   bundle, the letter is that bundle's."
2. **"What changes, named before any run", the window-1 bullet.** Its "is deleted and registered in rule I5 member by
   member" now reads "is deleted and registered in rule I5 as part D's MET line says".
3. **"What changes", the window-2 bullet and its sub-bullet.** "`SliceParser` and `SliceTokenizer` are deleted (decision
   D60) and so are `SliceSyntax.cs`'s records (decision D-slicetree)" is followed by ", each registered in rule I5 as
   part D's MET line says". The sub-bullet that begins "**The inputs the correspondence expects to differ** are those
   that reach hand-offs H23 and H25 (row SL4)" now reads:
   "**The inputs the correspondence expects to differ** at window 2 fall in three classes, each named here before any
   run:
   - **class H** (row SL4): those that reach hand-offs H23 and H25, the slice tokenizer's escape digits, its
     regular-expression body ending in a backslash, and a strict directive with `\8` or `\08`, the parse corpus's
     `J000262` and `J001009` among them, as JSD-0083 names them;
   - **class R** (row SL3, and rows DE3 and EE1 for the checks it names): a B4 input or a program of
     `SliceSourcePrograms` refused at the base whose refusal moves because the library's refusal stands for the slice
     static semantics' own: its code or its message is the library's row's where the base's was one of
     `ReservedWordAsBinding`, `DuplicateLexicalDeclaration`, `VarAndLexicalCollision`, `LegacyOctalInStrictCode`,
     `ConstWithoutInitialiser`, `InvalidAssignmentTarget`, `IllegalBreak` or `IllegalContinue`, its place is the
     library's, or its list is the library's first refusal where the base listed more than one;
   - **class C** (row SL5): a line of the census that moves because a file's parse is the library's and not
     `SliceParser`'s: a file `SliceParser` parsed that the library refuses, a file the library parses that `SliceParser`
     refused, or a parse failure counted under the library's first code instead of the first slice code.
   **Clause 5 as written admits no difference**, and part S admits none. What clause 5 means for each class is put to
   the owner (JSD-0088's decision D-census, whose question names all three): until the owner answers, any input of any
   class that differs makes part S NOT MET, and the owner's answer, if it revises clause 5, is filed with a new dated
   rule file in this directory before any run of part S. The inputs of class H are listed in this bundle before any
   run, from the correspondence; those of classes R and C are listed by input from the window-2 base's reports before
   the after commit's reports are compared."
4. **"The commits", the tools commit.** Its first sentence is followed by: "**The binding table is read through one
   product member** (JSD-0088's decision D-tables): the tools commit adds to `SliceCompilation`, in
   `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs`, one public read-only member that answers, for a
   compilation the slice static semantics validated, each binding's name, its kind as text and its slot, in slot order,
   as strings and integers and no type either window deletes, filled from the binding table the slice static semantics
   answers; it changes no artifact byte, no diagnostic and no census line. The report code reads the slice path only
   through `SliceSourceCompiler`'s public entry points, that member and `SliceConstructCensus`, and names no type the
   window-2 deletion list gives. SJL-15.4 keeps that member, its meaning and the entry points the tools call, and fills
   the member from its own slot allocation over the model, so the tools code stays unchanged at window 2." Its second
   condition reads "`git diff --name-only` between its first parent and it names only paths under
   `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/` and `docs/`, `eng/compare-parse-corpus.py`, and the
   assurance artefacts of those files; never `eng/packages/`, `eng/test262-evidence.py` or any workflow (JSD-0083's
   decision D37);". It now reads the same with
   "`src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs`, for that member and the lines that fill it
   alone," added after "`eng/compare-parse-corpus.py`,".
5. **Part S, its rule block.** The lines "population", "completeness", "comparison", "admitted difference classes",
   "the inputs the correspondence expects to differ ..." and "MET if and only if ..." now read:

   ```rule
   population: sub-population B4 of the parse corpus, each input compiled by the slice front end under its goal, as the base commit holds the manifest; the census over src/tests/corpus/parse-1/inputs/ and over the test/ directory of the suite extracted from the pinned archive, as bundle sjl-12-001's census 1 took them; and every program of SliceSourcePrograms, through the slice compiler's --checks rows that compile those programs and pin their outcomes, codes, messages and places (SourceFrontEndChecks' rows over SliceSourcePrograms' Accepted, Refused and RefusedModules lists), as the window-2 base holds them
   completeness: every B4 input reported at both commits, the manifest's digest equal at both, both census outputs whole, every row of the population run at both commits, and no run lost its process
   comparison: per B4 input, the outcome, the artifact hash, the binding table and the ordered diagnostic list, each byte for byte; each census output, byte for byte; each --checks row's verdict, and each program's pinned outcome, code, message and place, at the base against the after commit
   admitted difference classes: none
   the inputs the correspondence expects to differ at window 2, in classes H (row SL4), R (row SL3) and C (row SL5) as "What changes" names them, are listed in this bundle, class H before any run of this part from the correspondence, and classes R and C by input from the window-2 base's reports before the after commit's reports are compared; each that differs is a difference, which clause 5 as written does not admit, listed with its class
   a --checks row outside the population that the window's merge deletes or rewrites because it reads a type the window deletes (the row "the shared and slice tokenizers agree on the slice path's inputs" at window 2) is listed before any run of this part with what replaces it, or with "none" and why, and this part does not judge it
   every difference is a regression
   MET if and only if all runs of each window are complete, the control is reported as stated, no difference of any kind is reported, and every --checks row of the population passes at both commits.
   ```

   The lines "evidence class", "configuration" and "controls" are unchanged. In "How it is read", "**Should the owner
   revise clause 5 under decision D-census**, the revision is filed by a new dated rule file in this directory, quoting
   this part, before any run of it, and that file's class, not this part, judges the listed inputs" now reads the same
   with "that file's class" read as "that file's class or classes, for whichever of classes H, R and C the owner's
   answer admits".
6. **Part D, its configuration's controls.** "a re-added deleted member of JsBinder in a scratch copy of the window-1
   after commit and a re-added deleted member of SliceParser in a scratch copy of the window-2 after commit" now reads
   "the type JsBinder declared again, with one of its deleted members, in a scratch copy of the window-1 after commit,
   and the type SliceParser declared again, with one of its deleted members, in a scratch copy of the window-2 after
   commit, and the positional record SliceIdentifierReference declared again in a second scratch copy of the window-2
   after commit".
7. **Part D, its MET line.** Its clause "I5 registers every type and member the verdict's deletion list gives the
   window, member by member (JsBinder and its nested types at window 1; SliceParser, SliceTokenizer and SliceSyntax.cs's
   records at window 2);" now reads: "I5 registers every type and member the verdict's deletion list gives the window
   that the after commit no longer declares: each type deleted whole by type (JsBinder at window 1, which refuses its
   nested types and every member with it; SliceParser, SliceTokenizer and each type SliceSyntax.cs declares directly in
   a namespace at window 2), since none of its members can be declared again without it; every member deleted from a
   type the after commit still declares (SliceStaticSemantics' at window 2) whose kind rule I5's reader reads, by
   member, one row per distinct name; and the constructors, positional record parameters and enumeration members the
   reader does not read, and each check that was lines of a member that stays, named in I5's register paragraph for the
   window and in the window's cut-over record, by kind for each type deleted whole and by member or by row for a type
   that stays, and not registered (as bundle `sjl-13-001`'s revision of 2026-11-10 and bundle `sjl-13-002`'s of
   2026-11-13 read the same clause);". The rest of the MET line is unchanged, and "both controls are refused" reads
   "every control is refused".
8. **Part H, its admitted difference classes.** "a row the window's merge revises is listed with its diff, and must
   pin at the after commit what it pinned at the base, but where part B's classes P, F, T and S7 name the change, each
   such row listed with the class" is followed by "; a row the window's merge deletes is listed with its diff and with
   the row or test that replaces it, which must pin at the after commit what the deleted row pinned at the base, but for
   a listed class, and a deleted row with no replacement is listed with why none is needed and is judged a regression
   unless what it pinned is the deleted code's alone". Its MET line's "every row passes at both commits of each window"
   reads "every row the window keeps passes at both commits of each window, and every deleted row is listed as stated".
9. **"What this file does not decide", its third item.** It reads "**What clause 5 means for the inputs H23 and H25
   reach**: the owner's (JSD-0088's decision D-census)." It now reads "**What clause 5 means for the inputs of classes
   H, R and C**: the owner's (JSD-0088's decision D-census)."

## How it reads

- **Part S's configuration holds at window 2.** The tools code reads the table through a member of the product's public
  result, so the window-2 merge changes the producer and not the reader, and the configuration's "as the tools commit
  holds it and unchanged at each after commit" can be met. Part C's control K2, a scratch copy of the window-2 after
  commit whose slice lowering allocates its slots from an unseeded shuffle, must still make part S's comparison report a
  binding-table difference, which shows that the member reads the allocation the lowering makes and not a copy of the
  base's.
- **Nothing is admitted at part S that was not.** Classes H, R and C name what part S expects to see, so that the
  owner's question covers each before any run; this revision admits none of them, and part S stays NOT MET for any
  input of any class that differs until the owner answers D-census.
- **Part D asks of rule I5 what its reader reads.** Each type deleted whole is refused by its own row, and its members
  with it; the controls show both a type with a member and a positional record refused.
- **No part reads a row that cannot exist.** The tokenizer-agreement row is listed at window 2 with its replacement, or
  with "none" and why; it was bundle `sjl-13-001`'s part F, which judged step 1 and is not a part of this rule.

## What this revision does not change

Every other part, class and control; "The commits" but the tools commit's member and its path; the local machine; part
S's configuration and controls; parts A, B, C, E, F, G, I and J; and the section "What this file does not decide" but
its third item.

**Not taken.**
- **For matter 1, the window-2 merge ports the report code** and its output format is held byte-identical by a control
  that runs both versions over the window-2 base. The configuration's "unchanged at each after commit" would not hold,
  and the port would be judged by a comparison it writes itself.
- **For matter 2, SJL-15.4 keeps the slice path's own codes, messages and full walk-order lists**, those checks taken
  off JSD-0088's deletion list. The library refuses before any tree reaches the slice static semantics, so a full list
  would need a tree the library has refused or a parse of the slice path's own, which decision D60 deletes; and the
  checks kept would be a second producer of the library's rows, which JSD-0014's decision 1, one producer for each
  check, does not allow.
- **For matter 4, part S's MET line kept as "every --checks row"** with the tokenizer-agreement row rewritten to compare
  the shared tokenizer with itself. A row that compares one tokenizer with itself checks nothing.

**Owner:** the JavaScript profile owner, with the core architecture owner, as for the rule this file revises.
**Reviewer:** none; one person holds every role (EX-30), and this revision does not claim that anyone independent has
read it.

## The rule it revises, quoted whole

The quote below is [`decision-rule-area.md`](decision-rule-area.md) at blob
`2ddc5203f61bf0a470e51f53fc7917990c1f3f26`, as the Broiler.VM commit that files this revision holds it, each line
prefixed with `> ` (an empty line with `>`), as bundle `sjl-13-001`'s revisions quote its rule.

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
