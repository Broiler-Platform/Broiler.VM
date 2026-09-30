<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-3-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for every family of
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-3 exit gate,
the static semantics the wide front end did not have. Each family has a fixture in the host's acceptance
table, and each fixture is run beside the comparison engine.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-3 is not a ledger row, it has no owner, and a stage's gate met in a working tree is not acceptance.

**Collected:** 2026-09-30, at commit `c1a75e6`, from a clean tree, by `collect.py` in this directory,
against the base `55ca2bb`.
- **The change:** `2e2f51f`.
- **This directory's tooling:** `c1a75e6`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-3-001 |
| Stage touched | JSP-3 (every family of the exit gate) |
| Correction | [JSC-234](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-234) |
| Compositions | the command-line host, the conformance root, the slice-compiler root, the execution-only root |
| Comparison engine | Node `v22.22.2` |
| Suite | the pinned test262 at `ccaac100`, `test/language` and `test/annexB` |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The families and what each run shows

`build.log` is the solution built with warnings as errors. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host, all answering as
declared. `comparison.log` runs each JSP-3 fixture on the host and on the comparison engine and says
whether the two agree as declared.

| Family | Fixtures | What they show |
|---|---|---|
| Legacy escapes in a string literal | `runs/a-legacy-octal-escape-in-sloppy-code.js`; `refused/a-legacy-octal-escape-in-strict-code.js`, `…-before-use-strict.js`, `a-non-octal-decimal-escape-in-a-class-body.js` | sloppy code decodes each per Annex B to the engine's code units; strict code, a prologue a later directive makes strict and a class body refuse each with `2210`, as the engine does |
| The same, in a class element's name | `refused/a-legacy-octal-escape-in-a-class-element-name.js` | refused with `2210`; the engine runs it, and `comparison.log` records that as the declared disagreement |
| A tagged template's illegal escape | `runs/a-tagged-template-with-an-invalid-escape.js`; `refused/an-octal-escape-in-an-untagged-template.js` | the chunk cooks to `undefined` and keeps its raw text, the engine's answer; an untagged template refuses an octal escape with `2005` |
| A second `constructor` | `refused/a-second-constructor.js`; `runs/a-class-with-one-constructor-and-a-static-one.js` | refused with `2201`; a static method and a computed key named `constructor` still compile |
| A nested duplicate label | `refused/a-label-nested-in-itself.js`; `runs/a-label-reused-where-it-is-not-in-force.js` | refused with `2201`; sibling labels and a label inside a function inside the labelled statement run |
| `yield` and `await` as labels, and jumps to them | `runs/yield-and-await-as-labels.js` | `yield:`, `await:`, `continue yield`, `break await` and `break of` run, where each was refused |
| `??` beside `||` or `&&` | `refused/a-coalesce-beside-an-or.js`; `runs/a-coalesce-mixed-with-parentheses.js` | refused with `2101` naming the construct; with parentheses on either side it runs |
| A line terminator before `=>` | `refused/a-line-break-before-an-arrow.js`; `runs/an-arrow-body-on-the-next-line.js` | refused with `2101` naming the construct; a body on the next line runs |
| Strict-mode names at every binding position | `refused/a-restricted-name-as-a-function-name-under-its-directive.js`, `a-future-reserved-word-as-a-class-name.js`, `eval-as-a-parameter-under-a-body-directive.js` | each refused with `2209`; held before this change |
| A unary operand of `**` | `refused/a-unary-operand-of-exponentiation.js` | refused with `2101` naming the construct; held before this change |
| The dead zone in a function body | `dead-zone/a-read-before-the-initialiser-in-a-function-body.js` | `typeof` of the name throws the `ReferenceError`; held before this change |

`comparison.log` ends with every one of the twenty-one fixtures agreeing as declared: the same value
for a run, a `SyntaxError` for a refusal, the same `ReferenceError` for the dead zone, and for the class
element name the declared disagreement.

## 3. The suite, the corpus and the probes

`test262-base.log` and `test262-change.log` run the pinned suite's `test/language` and `test/annexB`
on the base commit's conformance root and on this change's: 24,995 files and 45,629 variants each. The
whole of both trees is run rather than the subtrees named after the families, because a label or a jump
can stand in any statement.

`test262-moves.log` lists the 52 variants whose verdict changed. **Every one went from failing to
passing, and none moved the other way.** They sit in the families' own directories:
- string literals, templates and tagged templates, including Annex B's template cases;
- the class early errors for a duplicate constructor;
- labelled statements, a module's duplicate labels and a static block's;
- `??` beside `||` and `&&`;
- the arrow function's line-terminator early errors.

What still fails in those directories is outside the families: HTML-like comments, a call expression as
an assignment target, tail calls, cross-realm and `SharedArrayBuffer` cases, among others.

- `checks.log` is the slice-compiler root's checks, all 589 passing.
- `corpus.log` is the retained corpus. It replays among the execution-only root's checks, and when the
  slice-compiler root re-derives it no entry's bytes differ from the retained ones. The change moved no
  lowered byte of a program the front end already accepted.
- `differential.log` is the differential probes, answering ok.

## 4. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds the host, judges the
rows this change added to `expected.txt` as a table of their own, reverts it and builds again. Every
control made that table fail.

| Control | What it reverts | What failed |
|---|---|---|
| `control-octal-decode` | Annex B's decoding: the escape's first digit is kept as a character and no digit after it is read | the sloppy-code row, and only it |
| `control-strict-escape` | the refusal of a legacy escape passed in strict code | the strict-code, class-body and class-element-name rows; the prologue row is still refused by its own check |
| `control-prologue` | the refusal of a prologue entry before a later `use strict` | the prologue row, and only it |
| `control-tagged-template` | the spoiled chunk of a tagged template, refused as in an untagged one | the tagged template's row: the program refused again |
| `control-template-octal` | the octal escapes' refusal in a template, cooked as their digit | the untagged template's row, which runs; and the tagged row, whose `\01` and `\1` chunks cook to strings |
| `control-constructor` | the refusal of a second `constructor` | both rows of the second-constructor fixture |
| `control-label` | the refusal of a label already in force | both rows of the nested-label fixture |
| `control-label-scope` | a function body's own set of labels | the reused-label row: the label inside the inner function refused as already in force |
| `control-coalesce` | the refusal of `??` beside `||` or `&&` | both rows of the `??` fixture, which runs |
| `control-parenthesised` | the parentheses remembered around a logical expression | the parenthesised row: refused though its parentheses were written |
| `control-arrow` | the refusal of a line terminator before `=>` | both rows of the line-break fixture, which runs |
| `control-yield-await-label` | `yield` and `await` read as labels | the labels row: `yield:` refused as a missing semicolon |
| `control-jump-label` | a jump's label read as any label name, back to a plain identifier | the labels row: `continue yield` refused as a missing semicolon |

`rows-after-controls.log` is the added rows after every revert, all answering as declared. `tests.log`
is the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The rest of section 4.6 of the parity roadmap.** Global and direct `eval`, `for … in` with a `let`
  binding and the function-expression name are not examined.
- **A named refusal for `yield:` in a generator or `await:` in an async function.** Both are refused,
  as they must be, with a token-level message.
- **The comparison engine as an oracle for class element names.** On a legacy octal escape in a class
  element's string name the engine and the specification disagree, and this host follows the
  specification. That fixture's row asserts the refusal and claims no engine answer.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 6. What was run

```text
python3 docs/evidence/jsp-3-001/collect.py --base 55ca2bb --node /opt/node22/bin/node
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.2`, the comparison engine.
