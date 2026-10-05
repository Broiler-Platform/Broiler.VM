<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-10-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for the clauses of
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-10 exit gate
that were still open after its reporting half: host capabilities present and throwing, the truth of
every refusal reason, the argument-count ceiling, and the order of several named files. Each change
has a fixture or a check, and each fixture is run beside the comparison engine.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-10 is not a ledger row, it has no owner, and a stage's gate met in a working tree is not
acceptance. The allowance defaults stay a decision for whoever owns
[ADR 0004](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md)'s
budget matrix.

**Collected:** 2026-10-03, at commit `b694a2d`, from a clean tree, by `collect.py` in this directory,
against the base `4414130`.
- **The change:** `9aa80e1`.
- **Between the change and this directory's tooling:** `c893806`, which repairs the warnings-as-errors
  build [record JSP-7-001](../jsp-7-001/README.md) found failing, and `dcaee77`, that record's logs.
- **This directory's tooling:** `b694a2d`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-10-001 |
| Stage touched | JSP-10 (the clauses still open after the reporting half) |
| Correction | [JSC-240](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-240) |
| Compositions | the command-line host, the conformance root, the slice-compiler root, the execution-only root |
| Comparison engine | Node `v22.22.0` |
| Suite | the pinned test262 at `ccaac100`: the call, construction, super and template expressions, the arguments object, and the four function constructors |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The changes and what each run shows

`build.log` is the solution built with warnings as errors, exiting 0. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host: 306 command
lines, all answering as declared. `comparison.log` runs each fixture on the host and on the comparison
engine and says whether the two agree as declared. `checks.log` is the slice-compiler root's checks,
with the two this change added named.

| Clause | Change | Evidence | What it shows |
|---|---|---|---|
| The argument count | past 255, a call's, a construction's and a super call's arguments travel in one Array | `runs/a-call-past-the-operand-width.js` | three hundred arguments written out reach a call, a method call, an optional call, `new`, `super` and `Math.max`, each evaluated once and in order; the engine's value |
| The argument count | a tagged template past 254 substitutions, its strings object built by `GetTemplateObjectWide` | the same fixture | the tag gets 301 strings and 300 substitutions, the strings object is frozen, and one site answers one object twice |
| The argument count | a function past 255 parameters refused at compile time with `2301`, naming the ceiling, where the verifier refused the host's own artifact | `refused/a-function-with-three-hundred-parameters.js` | exit 3 at the function; the engine runs it |
| Truthful reasons | `GeneratorFunction`, `AsyncFunction` and `AsyncGeneratorFunction` build from source where `Function` does | `runs/the-suspending-function-constructors.js` | each builds a function of its kind with the right prototype, and each body runs; the engine's value |
| Truthful reasons | the four constructors refuse with the declining composition's reason | `checks.log`: *the suspending constructors give a declining realm's reason* | all four answer a `TypeError` saying the composition did not admit `broiler.javascript.dynamic` |
| Truthful reasons | with the surface admitted they take `eval`'s door | `checks.log`: *the suspending constructors go where Function goes* | with no provider registered, each answers the `EvalError` `eval` answers |
| Present and refusing, stated | `read`'s message, and section 13 of the roadmap | `runs/the-host-members-that-refuse.js` | `read` and `$262`'s members are functions, and each refusal says what is true of the realm; the engine has no `$262` |
| File order | none: the usage text has stated ordinal path order since 2026-09-17 | the shared-realm row naming its two files in reverse | the transcript of the row that names them in order |

`comparison.log` ends with every one of the four fixtures agreeing as declared. Two print the engine's
value. The engine runs the parameter fixture, which this host refuses by a ceiling of its own format.
The engine meets a `ReferenceError` at `$262`, which is this host's.

## 3. The suite, the corpus and the probes

`test262-base.log` and `test262-change.log` run the pinned suite on the base commit's conformance root
and on this change's. The trees are the call, `new`, `super`, tagged-template and template-literal
expressions, `test/language/arguments-object`, and the built-in subtrees of `Function`,
`GeneratorFunction`, `AsyncFunction` and `AsyncGeneratorFunction`. That is 1,165 files and 2,116
variants each. The base passes 1,964 and the change 2,024.

`test262-moves.log` lists the 60 variants whose verdict changed. **Every one went from failing to
passing, and none moved the other way.** All 60 are the constructors': 24 in `GeneratorFunction`, 24
in `AsyncGeneratorFunction`, 6 in `AsyncFunction` and 6 in `Function.prototype.toString`'s cases
for the three. No case of the pinned suite passes more than 255 arguments, so the ceiling's changes
move nothing here. What still fails in the constructors' directories is cross-realm.

- `checks.log` is the slice-compiler root's checks: 591 passing, and 3 not run on this machine and
  claimed by nothing.
- `corpus.log` is the retained corpus. It replays among the execution-only root's checks, and when the
  slice-compiler root re-derives it no entry's bytes differ from the retained ones.
- `differential.log` is the differential probes, answering ok.
- `differential-node.log` is the comparison engine over the probes, on the base host and on this one:
  212 findings on each, none added and none removed.

## 4. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds, judges, reverts it
and builds again. Eleven are judged on the rows from the first JSP-10 section of `expected.txt` to its
end, 61 command lines that hold this change's eight. Two are judged on the slice-compiler root's
checks, because only a composition declining the dynamic surface shows them. Every control failed
what it was judged on, each in exactly one row or one check.

| Control | What it reverts | What failed |
|---|---|---|
| `control-call-array` | a long call's Array, back to the spread test alone | the long-argument row: the verifier refused the artifact, exit 4 |
| `control-construct-array` | the same for `new` | the long-argument row, exit 4 |
| `control-super-array` | the same for `super` | the long-argument row: `super=44`, the count wrapped to a byte |
| `control-template-wide` | the wide strings object and the Array call for a long tagged template | the long-argument row, exit 4 |
| `control-template-site` | the site's cache, dropped at each wide evaluation | the long-argument row: `one-site=false` |
| `control-parameter-ceiling` | the compile-time refusal of a long parameter list | the parameter row: exit 4 where 3 is declared |
| `control-generator-source` | `GeneratorFunction` building from source | the constructors row: the refusal, giving the dynamic surface's reason in a realm that admitted it |
| `control-async-source` | the same for `AsyncFunction` | the constructors row |
| `control-async-generator-source` | the same for `AsyncGeneratorFunction` | the constructors row |
| `control-read-message` | `read`'s message, back to "no composition can register a reader" | the `read` row |
| `control-file-order` | the ordinal sort of named files | the reversed shared-realm row: the second script ran first and threw |
| `control-declining-reason` | the refusal in a declining composition, so the three build from source there too | the declining-realm check: three `EvalError`s |
| `control-function-message` | `Function`'s message, back to naming the wide manifest | the declining-realm check: `Function`'s message does not say the composition declined |

`rows-after-controls.log` is the 61 rows after every revert, all answering as declared. `tests.log` is
the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **Absence.** `typeof read` still answers `"function"`. The capability clause is met by its other
  branch: the profile states the opposite and why, in section 13 of the roadmap.
- **A raised parameter ceiling.** A function with more than 255 parameters before its first default or
  rest is refused, not run.
- **The allowance defaults.** They stay a decision, as the gate says.
- **A throw in shared-realm multi-file mode.** It still abandons the remaining files, which section 4.8
  of the parity roadmap records and the gate does not name.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 6. What was run

```text
python3 docs/evidence/jsp-10-001/collect.py --base 4414130 --node /opt/node22/bin/node
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.0`, the comparison engine.
