<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-13-002's area rule, revised on 2026-11-13: part D's rule I5 registers what step 2 deletes that the after commit no longer declares, each type deleted whole by type, and names by kind what its reader does not read

**Written:** 2026-11-13, before any run of step 2 that this rule judges. It is filed on the branch whose merge is to
be this rule's after commit, before that merge, and so before the base or the after commit of any part is taken; it
is also filed before the development reference that bundle `sjl-13-002`'s README names (its section 1, "Readings taken
before any run", reading 6) was taken, and was written with no run of that reference seen. This file revises
[`decision-rule-area.md`](decision-rule-area.md) as that file's section "Revisions" requires ("As bundle
`sjl-13-001`'s": a rule revised is a new dated file in this directory, which quotes the rule whole). It is a new dated
file, it quotes that file whole at its end, and that file is not edited. In this file, "this revision" is this file.
It is the first revision of this rule.

**Whose choice it is.** No owner's decision of its own is behind this revision. It writes into part D's MET line the
reading of decision D20 that JSD-0083's dated note of 2026-11-13 on that decision records, a reading taken under the
owner's standing direction of 2026-10-18 ("Please do all recommended decisions and skip waiting for review") and
recorded as the owner's dated decision under that direction, not as the owner's own dated answer. The owner may rule
otherwise.

**What was seen, and where.** Step 2 is built on the branch `sjl-13-3-step2`, cut from
`686533fad10a17600ce67e5a774892f62cb08006`; no product source has changed on it when this revision is filed. Read at
that commit:
- **What rule I5's reader reads.** `I5RetiredDuplicatesRuleTests.Declarations` (lines 144 to 173 at `686533fa`)
  yields each type declared directly in a namespace or a compilation unit, by its full name, and, of each type that
  can hold members, the names of its methods, properties, events, fields, nested types and delegates. It yields no
  constructor, no positional parameter of a record, no member of an enumeration (an enumeration is not a type
  declaration whose members it walks), and no operator or indexer. A row of the register is refused when the name it
  gives is declared in the family's product sources.
- **How the register is read.** `I5RetiredDuplicatesRuleTests.Register` (lines 120 to 125 at `686533fa`) reads
  `retired-members.json` into a dictionary keyed by the member's name, with `ToDictionary`, so the register holds one
  row per distinct name, and a second row with a name already present fails the read. The reader yields an
  overloaded member once per declaration under one name: the adapter declares `TokenBefore` three times, in
  `JsFrontendAdapter.Modules.cs`.
- **What step 2 deletes.** `JsSyntax.cs` declares this family's syntax tree as records, an abstract base and its
  derived node types, with four enumerations and the static class `JsModuleNames`; `JsFrontendAdapter.cs`,
  `JsFrontendAdapter.Expressions.cs`, `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs` declare the
  one partial class `JsFrontendAdapter`. Almost every member of `JsSyntax` is a positional parameter of a record,
  which the reader does not read. The members `JsSyntax.cs` declares in a record's body are the reader's
  (`JsMemberExpression.NameSpan`, `ParenthesisedTarget`, and `SourceStart` and `SourceEnd` on the function and class
  nodes), as are `JsModuleNames.DefaultBindingName` and `JsModuleNames.StarName`; the adapter's methods, properties
  and fields, `SawTopLevelAwait` among them, are the reader's too.
- **The clause as written.** Part D's MET line asks that "I5 registers every member of JsSyntax and of the adapter".
  Read literally, it asks for a row for each positional parameter, enumeration member and constructor, which rule
  I5's reader never yields, so such a row could refuse nothing; and a row for each declaration of the adapter's, which
  the register cannot hold where a name is overloaded. No member of a deleted type can be declared again without its
  type, so the type's row refuses each of them. Bundle `sjl-13-001`'s rule met the same question at step 1, and its
  revision of 2026-11-10 ([`../sjl-13-001/decision-rule-area-revised-2026-11-10.md`](../sjl-13-001/decision-rule-area-revised-2026-11-10.md))
  answered it with "a type deleted whole by type and every other member by member", naming what the reader does not
  read in the register's paragraph and in the cut-over record.

## What changed

1. **Part D, the MET line, its fourth clause.** It reads: "I5 registers every member of JsSyntax and of the
   adapter". It now reads: "I5 registers every type and member step 2 deletes that the after commit no longer
   declares: each type that `JsSyntax.cs`, `JsFrontendAdapter.cs`, `JsFrontendAdapter.Expressions.cs`,
   `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs` declare directly in a namespace by type, which
   refuses each member of it, since none can be declared again without its type; every member, of a type the after
   commit still declares, that step 2 deletes and whose kind rule I5's reader reads, by member, one row per distinct
   name; and the positional record parameters, enumeration members and constructors that the reader does not read are
   named in I5's register paragraph for SJL-13.3's step 2, by kind for each type deleted whole and member by member
   for a type that stays, and are not registered (decision D20, as JSD-0083's note of 2026-11-13 on it reads it)".
   The rest of the MET line is unchanged.

## How it reads

- **Part D's control is pinned.** The configuration's "rule I5's control, a re-added deleted member of JsSyntax in a
  scratch copy, which must be refused" is a scratch copy of the after commit that declares the type
  `JsMemberExpression` again, with its member `NameSpan`, and it must be refused: rule I5 registers
  `JsMemberExpression` by type. A second control declares `JsNumberLiteral` again, a record whose members are all
  positional, and it must be refused too, by its type's row. Each control is retained with its output, as the
  bundle's README says of part D's I5 control.
- **What the type rows cover.** `JsModuleNames.DefaultBindingName` and `JsModuleNames.StarName`, which step 2 moves to
  the lowering's module code, and `JsFrontendAdapter.SawTopLevelAwait`, whose fact the compiler assembly's post-parse
  code computes at step 2, are covered by their types' rows, `JsModuleNames` and `JsFrontendAdapter`. Those type names
  are not declared again; the members' names are free to be declared on another type.
- **Nothing is admitted by this revision that a part judges.** It changes what part D's fourth clause asks of rule
  I5's register and pins the control the configuration names; it changes no test, no witness, no population and no
  other part. Rule I5's test, at the after commit, is the test part D runs, and the register's rows are those the
  branch commits.

## What this revision does not change

Every other part, class and control; "The commits" and their conditions; part D's population, its configuration but
for the control pinned above, and its other clauses: every test passes at the after commit, every witness is refused,
N12 counts the compiler assembly's files with a comment naming each file step 2 deleted and added, I6 finds no shared
type in this family's public API baseline, and the control is refused.

**Not taken.**
- **"Register each adapter member the reader reads as well."** The register would have held a row for each method,
  property and field of `JsFrontendAdapter` beside its type's row. None can be declared again without the type, which
  its row refuses, and an overloaded name such as `TokenBefore` takes one row however often it is declared, so the rows
  would add nothing the type's row does not refuse.
- **"A row for every positional parameter."** The register would have held a row for each positional parameter of each
  record of `JsSyntax`. Rule I5's reader yields no positional parameter, so no such row could ever refuse a
  declaration, and the register would claim a refusal the rule cannot make.

**Owner:** the JavaScript profile owner, with the core architecture owner, as for the rule this file revises.
**Reviewer:** none; one person holds every role (EX-30), and this revision does not claim that anyone independent has
read it.

## The rule it revises, quoted whole

The quote below is [`decision-rule-area.md`](decision-rule-area.md) at blob
`a6b2a086c5775b3e4ca7450da530bb9510bc5095`, as the Broiler.VM commit that files this revision holds it, each line
prefixed with `> ` (an empty line with `>`), as bundle `sjl-13-001`'s revisions quote its rule.

> <!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
> <!-- SPDX-License-Identifier: Apache-2.0 -->
>
> # Bundle SJL-13-002's area rule: the wide path on the shared syntax library, step 2, judged by test262 parity in both forms on hosted runners, by artifact identity over the parse corpus, by determinism, by the front end's rules, by the fuel-parity twins, by this family's measurement, and by source fuzzing
>
> **Written:** 2026-10-26, at milestone SJL-13 (work package SJL-13.1), with the area verdict JSD-0083 and bundle
> `sjl-13-001`'s rule, before any change either judges exists. **Milestone judged:** SJL-13, at the second of its two
> cut-over steps, in which the lowering, the numeric-manifest admission and the binder read the shared tree directly and
> the adapter and `JsSyntax` go: its Broiler.VM clauses 4, 5 and 6 at that step, clause 7's rules N5 to N9, N12 and N19,
> clause 9's fuzzing at that step, and SJL-13.6's measurement. **Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:**
> none. Every role this rule names is held by one person (EX-30), and this rule does not claim that anyone independent
> has read it.
>
> **What this file is.** Seven predeclared decisions, one per part: part A in template T1, part B in T6, part E in T7,
> part F in T5, part C layer L13, part D the rule runs and part G layer L9. Each part decides alone. Part F is not a gate clause
> (decision D24). It holds no figure.
>
> **Step 2 changes no outcome, no byte and no diagnostic** (decision D19). Every input's outcome, every accepted input's
> artifact, every module-goal input's ordered specifiers from `JsCompiler.Requests`, every refused input's diagnostic
> list and every test262 verdict is expected the same at both commits. The
> lowering computes from the shared tree, by `JsParser`'s rules, every fact the adapter computed at step 1, the
> top-level-`await` rule included (decision D15), and places every diagnostic as step 1 did. This rule therefore
> admits no class but part A's harness classes.
>
> **The commits, named before they exist.**
> - **The rule commit** carries this file.
> - **The after commit** is the default-branch merge that lands SJL-13.3's step 2, one merge, named in bundle
>   `sjl-13-002`.
> - **The base commit** is the after commit's first parent. It must meet three conditions: it descends from the rule
>   commit; it descends from the commit bundle `sjl-13-001` names as its after commit; and the after commit is the only
>   first-parent commit that carries any part of step 2. If step 2 had to land in more than one merge, each is named in
>   bundle `sjl-13-002`, the base is the first one's first parent, and the after runs are taken at the last one.
> - Part A's runs are dispatched at those commits at any time after each exists; the other parts are taken locally from
>   clean checkouts.
>
> A part whose base was taken at any other commit, or whose base commit fails those conditions, is void.
>
> ## Part A - test262 parity, in both forms, on hosted runners (template T1)
>
> ```rule
> evidence class: conformance parity (not a measurement)
> population: bundle sjl-13-001's part A's
> configuration: bundle sjl-13-001's part A's, the base runs taken at this file's base commit and the after runs at its after commit, the workflow and the driver at the dispatched ref byte-identical to the base commit's
> completeness: bundle sjl-13-001's part A's
> comparison: per variant, joined on path and variant, the verdict and any exhausted dimension, through eng/compare-test262-forms.py --same-form, once for the bytecode pair and once for the native pair with --exempt-guest-loads over the extracted suite
> admitted difference classes:
>   C4 harness and environment - bundle sjl-13-001's part A's classes (a), (b), (c') and (e), with their re-runs on the local machine
> no C1, no C2, no C3 and no C5: step 2 changes no outcome
> every other difference is a regression
> MET if and only if all four runs are complete, and every changed verdict of either pair is in C4 and is listed.
> NOT MET otherwise.
> ```
>
> ## Part B - artifact identity over the parse corpus (template T6)
>
> ```rule
> evidence class: artifact identity (not a measurement)
> population: bundle sjl-13-001's part B's, sub-populations B1 to B5, B5 being each module-goal input through JsCompiler.Requests under the module options, as the base commit holds the manifest
> configuration: the slice compiler's --parse-corpus mode, as the base commit holds it and unchanged at the after commit, built in Release from a clean checkout on the local machine, reporting for each input, sub-population, variant and form what bundle sjl-13-001's part B reports, in B5 the ordered specifiers of an accepted input; run at the base commit and at the after commit, and with --census at both
> completeness: every manifest input reported in every sub-population, variant and form at both commits, the manifest's digest equal at both, and no run lost its process
> controls: a copy of the base report with one artifact hash changed, one diagnostic's column changed and one specifier of a B5 input changed, which the comparison must report as three differences; retained
> comparison: eng/compare-parse-corpus.py, as the base commit holds it: the outcome; the artifact hash of an input accepted at both, or in B5 its ordered specifiers; the ordered diagnostic list of an input refused at both; the census, byte for byte
> admitted difference classes: none
> every difference is a regression, a specifier list that differs for a B5 input accepted at both among them
> MET if and only if both runs are complete, the control is reported as stated, and no difference of any kind is reported.
> NOT MET otherwise.
> ```
>
> ## Part C - determinism (layer L13)
>
> ```rule
> evidence class: determinism (not a measurement)
> population and configuration: bundle sjl-13-001's part C's, at this file's after commit, its five checks rows at both commits
> controls: bundle sjl-13-001's part C's K1 to K4, applied to a scratch copy of this file's after commit
> admitted difference classes: none
> MET if and only if every accepted input gives one hash in all four compilations, all five rows pass at both commits, and the controls fail as stated and pass after their revert.
> NOT MET otherwise.
> ```
>
> ## Part D - the front end's rules (rule runs)
>
> ```rule
> evidence class: rules (not a comparison)
> population: rules N5 to N9, N12, N19, I5, I6 and A10 of the register, their tests and witnesses
> configuration: dotnet test of Broiler.VM.Architecture.Tests in Release at the after commit, BROILER_RULE_MESSAGES set, logs retained; the same at the base commit, read, not judged; rule I5's control, a re-added deleted member of JsSyntax in a scratch copy, which must be refused
> MET if and only if every test passes at the after commit, every witness is refused, N12 counts the compiler assembly's files with a comment naming each file step 2 deleted and added, I5 registers every member of JsSyntax and of the adapter, I6 finds no shared type in this family's public API baseline, and the control is refused.
> NOT MET otherwise.
> ```
>
> ## Part E - the fuel-parity twins (template T7)
>
> ```rule
> evidence class: resource accounting (not a measurement)
> population, configuration and ceilings tried: bundle sjl-13-001's part I's, the bisect taken at this file's base commit and retained as sjl-13-002/base/fuel-parity.log
> admitted difference classes: none
> MET if and only if every twin, in both forms, gives after the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
> NOT MET otherwise.
> ```
>
> ## Part F - this family's compile measurement (template T5)
>
> ```rule
> evidence class: measurement
> population, arms, configuration, condition, quantity, noise floor, comparison and readings: bundle sjl-13-001's part J's, with its workloads, B this file's base commit's build and A this file's after commit's; retained in sjl-13-002/measurement/
> admitted difference classes: none
> MET if and only if every lane's condition passes, every effective-configuration line is identical, and for every workload D <= N.
> NOT MET otherwise.
> ```
>
> **What part F decides.** Whether reading the shared tree directly compiles more slowly than through the adapter on
> this machine. Not a gate clause (decision D24).
>
> ## Part G - source fuzzing (layer L9)
>
> ```rule
> evidence class: robustness (not a measurement)
> population, configuration, reading and controls: bundle sjl-13-001's part G's, both sessions, with its seeds file, at this file's after commit, and the base's sessions at this file's base commit retained and read, not judged; the wide control's throw placed in a lowering or binder walk over the shared tree that the seeds reach, since the adapter is gone
> MET if and only if neither of the after commit's sessions reports a finding and both controls are reported as stated.
> NOT MET otherwise.
> ```
>
> **How it is read.** Step 2 moves the lowering's and the binder's walks onto the shared tree, which no session reached
> before this file's after commit (clause 9 judged on SJL-13's final state).
>
> ## Revisions
>
> As bundle `sjl-13-001`'s.
>
> ## What this file does not decide
>
> Step 1 (bundle `sjl-13-001`); the home clauses; clause 8's publication (the collector's runs at this file's after
> commit); JS-2's closure, which the profile's ledger records on both bundles; a speed claim of any kind.
