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
  of four rows and a stage table copy (each named in the mapping's header by its SHA-256); a registry
  at revision 19 with two codes that revision adds, `2105 BigIntWiderThanTheFormat` (a non-language
  rule that left 2104) and `2211 AwaitOutsideItsFunction` (a language rule that left 2104); a mapping
  with a `registry` row and a `code` row for each; and the four other files the checks read from
  revision 19, in the shapes this checkout's own have: `SliceSourceDiagnostics.cs.txt`,
  `LanguageErrors.cs.txt`, `Test262Run.cs.txt` (the codes `Refused` grades unsupported) and
  `source.manifest`. The C# files carry `.txt` so that no build and no rule over this repository's
  sources reads them as code. None of this is a record of this family.
- **`controls.json`**: the controls. Each fixture control is a set of edits of the fixture, each edit
  replacing a text that occurs exactly once, and the checks that must refuse the edited copy. Each
  `real` control edits copies of this checkout's own mapping and registry.

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
| C10 | 2105's registry row made defensive | E13 |
| C11 | the stage table's copy changed under its digest | E11 |
| C12 | the stage table's digest removed from the header | E11 |
| C13 | the registry's published-at-milestone list no longer names revision 19 | E12 |
| C14 | the registry's owning decision removed | E12 |
| C15 | 2211 reached by no row | E7 |
| C16 | the fixture stated at revision 18, with a split row: E10 to E14 inert | passes |
| R1 | this checkout's mapping and registry, stated at revision 19 | E10, E11, E12 |
| R2 | this checkout at revision 18, the four revision-19 inputs named where no file is | passes |

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
  also `a language rule` for each of decision D12's three language rules;
- `Test262Run`'s `Refused` names, before its first `Test262Verdict.Unsupported`, exactly the codes it
  grades unsupported.
