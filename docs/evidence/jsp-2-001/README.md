<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-2-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for the clauses of
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-2 exit gate
that were still open: the refusal that was lost, a BigInt literal that is never a Number. The type was
admitted by JSeal B01 to B08, recorded in the `docs/evidence/jseal-b*` records. This record adds what
the gate asked for and nothing had retained: a corpus entry carrying the refusal, the type half's list
asked at the host, and a negative control for each half.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-2 is not a ledger row, it has no owner, and a stage's gate met in a working tree is not acceptance.

**Collected:** 2026-10-03, at commit `bef6710`, from a clean tree, by `collect.py` in this directory,
against the base `b694a2d`.
- **The change:** `33eeff5`.
- **This directory's tooling:** `bef6710`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-2-001 |
| Stage touched | JSP-2 (the clauses still open) |
| Correction | [JSC-241](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-241) |
| Compositions | the command-line host, the conformance root, the slice-compiler root, the execution-only root |
| Comparison engine | Node `v22.22.0` |
| Suite | the pinned test262 at `ccaac100`: `BigInt`, the BigInt literals, `JSON.stringify`, and the `typeof`, equality, strict-equality and addition expressions |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The two halves and what each run shows

`build.log` is the solution built with warnings as errors, exiting 0. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host: 311 command
lines, all answering as declared. `comparison.log` runs each fixture on the host and on the comparison
engine and says whether the two agree as declared.

| Half | Clause | Evidence | What it shows |
|---|---|---|---|
| Cheap | refused at compile time, naming the construct, and `--check` decides it | `--slice --check` and `--numeric --check refused/a-bigint-literal-under-a-narrow-manifest.js` | both exit 3 with `2104` at the literal, the slice surface naming "the construct BigInt" and the numeric manifest "a BigInt literal"; the engine runs the file to `9007199254740993n` |
| Cheap | a retained corpus entry carries it | `refuse-a-bigint-literal` in the source corpus | `checks.log`: the slice-compiler root refuses all 33 refused sources with their recorded codes, this one among them, and none carries an artifact; `corpus.log`: the corpus re-derives byte for byte with the new entry |
| Type | the literal is the exact integer under the default manifest | `refused/a-bigint-literal-under-a-narrow-manifest.js` with no option | the host prints `9007199254740993` and the engine `9007199254740993n`, the same integer |
| Type | `typeof`, mixing, the equalities, a value past the Number range, `JSON.stringify`, every literal form | `runs/a-bigint-is-not-a-number.js` | 26 answers, every one the engine's: `bigint`; three `TypeError`s for mixing; `===` false and `==` true; `2n ** 64n + 1n` round-tripped; `JSON.stringify` a `TypeError`; the hexadecimal, octal and binary forms; division, remainder, a shift past 64 bits and `asIntN`/`asUintN` |

`comparison.log` ends with every one of the four comparisons agreeing as declared.

## 3. The suite, the corpus and the probes

`test262-base.log` and `test262-change.log` run the pinned suite on the base commit's conformance root
and on this change's. The trees are `test/built-ins/BigInt`, `test/language/literals/bigint`,
`test/built-ins/JSON/stringify`, and the `typeof`, `equals`, `strict-equals` and `addition`
expressions. That is 343 files and 683 variants each, and both pass 675.

`test262-moves.log` lists no variant whose verdict changed. **That is the expected answer**: the change
touches no product code the suite reaches, so the two runs are the type half's standing taken twice.
The eight variants that fail are four files: three cross-realm cases and `JSON.stringify`'s
`property-order.js`, none of them about whether a BigInt is a Number.

- `checks.log` is the slice-compiler root's checks: 591 passing, and 3 not run on this machine and
  claimed by nothing. The two source-corpus checks are named.
- `differential.log` is the differential probes, answering ok.
- `differential-node.log` is the comparison engine over the probes, on the base host and on this one:
  212 findings on each, none added and none removed.

## 4. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds the host, judges the
rows this change added to `expected.txt` as a table of their own, reverts it and builds again. Every
control made that table fail.

| Half | Control | What it reverts | What failed |
|---|---|---|---|
| Cheap | `control-slice-literal` | the slice surface's refusal, so the literal is read as a Number | the `--slice --check` row: it compiles |
| Cheap | `control-numeric-literal` | the numeric manifest's refusal, the same way | both `--numeric` rows: the program runs and prints `9007199254740992`, the wrong integer the parity roadmap's section 4.2 recorded |
| Type | `control-wide-literal` | the wide manifest's BigInt literal, read as a Number | the type-half row, which throws converting `255` to a BigInt, and the default-manifest row, which prints `9007199254740992` |
| Type | `control-mixing` | the `TypeError` for a BigInt mixed with a Number, answering `NaN` | the type-half row: `NaN` twice where `TypeError` is declared |
| Type | `control-json` | `JSON.stringify`'s `TypeError` for a BigInt, answering `null` | the type-half row: `{"a":null}` |

`rows-after-controls.log` is the five rows after every revert, all answering as declared. `tests.log`
is the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The type's implementation.** The JSeal B records hold its evidence. This record asks the gate's
  list of it at the host and controls the items it names.
- **One wording for the refusal.** The slice surface names "the construct BigInt" and the numeric
  manifest "a BigInt literal". Both name the construct.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 6. What was run

```text
python3 docs/evidence/jsp-2-001/collect.py --base b694a2d --node /opt/node22/bin/node
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.0`, the comparison engine.
