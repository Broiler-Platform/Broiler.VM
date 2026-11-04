<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The controls of `eng/check-frontend-mapping.py`'s revision-19 checks

Written 2026-10-27, at SJL-13's tools commit (JSD-0083 decisions D12 and D27), with the checks they
control: E10 to E14, which the checker applies once `registry.txt` states revision 19 or more, and the
widening of E7 to the codes revision 19 adds. Part E of bundle `sjl-13-001`'s rule runs the checker
"as the tools commit holds it" at step 1's after commit, so these checks are written before the
registry commit they read exists. While the registry is at revision 18 they are inert, and the checker
refuses exactly what it refused before.

Run them with:

```sh
python3 eng/check-frontend-mapping.py --controls eng/check-frontend-mapping-controls
```

It prints one line per control and exits 0 only when every control behaves as `controls.json` states.

## What is here

- **`fixture/`**: the smallest world at registry revision 19 that the checker passes. A catalogue copy
  of five rows and a stage table copy (each named in the mapping's header by its SHA-256); a registry
  at revision 19 with three codes that revision adds, `2105 BigIntWiderThanTheFormat` (a non-language
  rule that left 2104), `2211 AwaitOutsideItsFunction` (a language rule that left 2104) and, *(added
  2026-11-04)*, `2304 ConstructTheLoweringHasNoArmFor` (a defensive code that left 2104, for the
  catalogue's `EX0024`); a mapping with a `registry` row and a `code` row for each; and the four other files the checks read from
  revision 19, in the shapes this checkout's own have: `SliceSourceDiagnostics.cs.txt`,
  `LanguageErrors.cs.txt`, `Test262Run.cs.txt` (the codes `Refused` grades unsupported) and
  `source.manifest`. The C# files carry `.txt` so that no build and no rule over this repository's
  sources reads them as code. None of this is a record of this family.
- **`controls.json`**: the controls. Each fixture control is a set of edits of the fixture, each edit
  replacing a text that occurs exactly once, and the checks that must refuse the edited copy. Each
  `real` control edits copies of this checkout's own mapping and registry. *(Added 2026-11-04.)* An
  edit may name a one-line `pattern` instead of an `old` text, for a text that differs between registry
  revisions, and it too must match exactly once. A `real` control may state its expectation per
  registry revision (`by-revision`), keyed by the revision this checkout's `registry.txt` states, and an
  expectation may be that the run cannot read its inputs (`unreadable`). A revision a control names no
  expectation for is not judged: the controls exit 2.

## The controls

| Control | Edit | Refused at |
|---|---|---|
| F0 | none: the fixture as committed | passes |
| C1 | a split row left in place (part E's second control) | E10 |
| C2 | a shared code given a second registry code (part E's first control) | E3 |
| C3 | a new code's `LanguageErrors.Classified` entry removed (part E's third control) | E14 |
| C4 | 2105, which left 2104, no longer graded unsupported | E14 |
| C5 | 2211, a language rule, graded unsupported | E14 |
| C6 | 2105 classed `ImplementationLimit`, where the code it left is `OutsideManifest` | E14 |
| C7 | 2211's member of `SliceSourceDiagnosticCode` removed | E13 |
| C8 | 2211's case removed from the retained source corpus | E13 |
| C9 | 2105's mapping row no longer names the code it left | E13 |
| C10 | 2105's registry row made defensive, for `EX0002`'s meaning | E13 |
| C11 | the stage table's copy changed under its digest | E11 |
| C12 | the stage table's digest removed from the header | E11 |
| C13 | the registry's published-at-milestone list no longer names revision 19 | E12 |
| C14 | the registry's owning decision removed | E12 |
| C15 | 2211 reached by no row | E7 |
| C16 | the fixture stated at revision 18, with a split row: E10 to E14 inert | passes |
| C17 | 2304, defensive, whose mapping row does not say it is defensive *(added 2026-11-04)* | E13 |
| C18 | 2304, defensive, with no reason in its registry row *(added 2026-11-04)* | E13 |
| C19 | 2304, defensive, also reached by `EX0002`, a meaning E13 does not admit *(added 2026-11-04)* | E13 |
| R1 | this checkout's mapping and registry, stated at revision 19 | at 18: E10, E11, E12; at 19: passes |
| R2 | this checkout's mapping and registry, the four revision-19 inputs named where no file is | at 18: passes; at 19: unreadable |

C10, a new code made defensive for `EX0002`'s meaning, is the control that the widening of E13 still
refuses what it refused before: `EX0002` is not one of the two meanings E13 admits. F0, which carries
2304, shows the widening admits the one case it was made for.

**R1 and R2 were restated on 2026-11-04**, on the owner's decision of that day on registry revision 19
(JSD-0083's note on decision D12). They edited and expected revision 18 only: R1 replaced the literal
`# registry-revision: 18`, which no longer occurs once the registry commit states 19, and R2 expected a
pass from inputs the checker does not read below 19. Each now states what it expects at 18 and at 19.
At 18 they are what they were: R1 is refused at E10, E11 and E12, and R2 passes. At 19, R1, which sets
the revision line to 19 whatever it was, reads the records the registry commit wrote and must pass, and
R2 must be unreadable, because from 19 the four inputs are read and an absent one is an error, never a
pass. They read: "R1 | this checkout's mapping and registry, stated at revision 19 | E10, E11, E12" and
"R2 | this checkout at revision 18, the four revision-19 inputs named where no file is | passes".

A control lists the checks that must be among the refusals; an edit that breaks more than one check
(C1, C2 and C15 do) is refused by those too, and the line the checker prints names them.

## What revision 19 must state, so that these checks can read it

The registry commit of step 1 writes the files these checks read, and it may not change the checker
(bundle `sjl-13-001`'s rule, "The registry commit"). So the checker's header states what it reads:

- the mapping's header names the stage table by `stage-table-path`, `stage-table-commit`,
  `stage-table-sha256` and `stage-table-copy`, beside the catalogue's four keys;
- `registry.txt` keeps its `owning-decision` line and names the revision it states in its
  `published-at-milestone` list;
- each code the registry adds has `since` 19, a `source` row whose case is a `refused` entry of the
  retained source corpus answered with its name, a member of `SliceSourceDiagnosticCode`, a class in
  `LanguageErrors`, and a `registry` row in the mapping whose `notes` reads `left <code> <Name>`, and
  also `a language rule` for each of decision D12's three language rules *(noted 2026-11-04: revision 19
  mints no code for those three, on the owner's decision of that day, so no row says it; the check and
  the fixture's 2211 stay)*;
- *(added 2026-11-04)* a code the registry adds may instead be a `defensive` row, whose case states why
  no source reaches it, when the mapping maps onto it no catalogue row but `EE0021` or `EX0024` and its
  `registry` row's `notes` says `defensive`; rule N7's `DefensiveRows` admits the same two;
- `Test262Run`'s `Refused` names, before its first `Test262Verdict.Unsupported`, exactly the codes it
  grades unsupported.
