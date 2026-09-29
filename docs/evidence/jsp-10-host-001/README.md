<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-10-HOST-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for four clauses
of the reporting half of [the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s
JSP-10, the host surface an embedder meets first:
- the default allowance, documented with a program that reaches it;
- the constant pool's ceiling, refused naming it and where it was met;
- top-level `await` in a template substitution, which the roadmap listed as a ceiling and which was a
  parser defect;
- the source-encoding set, stated, with a file outside it refused by naming its encoding.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-10 is not a ledger row, it has no owner, and a stage's clauses met in a working tree are not
acceptance.

**Collected:** 2026-09-29, at commit `1e1bd41`, from a clean tree, by `collect.py` in this directory.
- **The change:** `f2d13b0`.
- **This directory's tooling:** `1e1bd41`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and a dated note in the profile's ledger.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-10-HOST-001 |
| Stage touched | JSP-10 (four clauses of the reporting half) |
| Correction | [JSC-233](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-233) |
| Compositions | the command-line host, the slice-compiler root |
| Comparison engine | Node `v22.22.2`, for the fixtures' answers only |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The four clauses and what each run shows

`build.log` is the solution built with warnings as errors. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host, all answering as
declared. `probes.log` runs each clause through the host's command line and keeps what it printed.

| Clause | Evidence | What it shows |
|---|---|---|
| The default allowance is documented and reachable | `limits/an-ordinary-loop-past-the-default-allowance.js`; the `--help` line | three million additions exit 5 at the default; with `--fuel 1000000000` the same file answers node's sum; `--help` states 50,000,000, read from the descriptor |
| The constant pool's ceiling is named where it is met | the slice-compiler root's check, in `checks.log` | 70,000 distinct constants refused once, with `2302` naming 65,535, at the line where the ceiling was met; 60,000 compile |
| Top-level `await` in a template substitution runs | `modules/a-top-level-await-in-a-template.mjs` | plain, tagged and nested substitutions answer node's value, exit 0 |
| The encoding set is stated; a file outside it is refused by name | `encoding/utf-16le-with-a-byte-order-mark.js`, `encoding/utf-16be-with-a-byte-order-mark.js`; the `--help` lines | each UTF-16 file refused naming its encoding, exit 6; a UTF-8 file with a mark runs; bytes that are not UTF-8 are refused as before |

**The constant pool is checked in the slice-compiler root and not by a fixture file** because a program
that reaches it is some hundreds of kilobytes. The check writes one: one distinct number per statement,
one statement per line, so the line the refusal names says how far the program had got.

**The arguments ceiling was already refused by name and at the call**, and it is recorded here only by
the roadmap's section 7, which now states it with the other two limits a source program can meet.

## 3. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds, runs what it must
fail, reverts it and builds again. The host controls are judged against the rows this change added to
`expected.txt`, as a table of their own.

| Control | What it reverts | What failed |
|---|---|---|
| `control-template-await` | the substitution's await flag carried out | the fixture's row: the refusal "only admitted inside an async function", exit 3, in place of the value |
| `control-encoding` | the byte-order-mark detection | both UTF-16 rows: "not valid UTF-8" in place of the encoding's name |
| `control-usage-fuel` | the figure read from the descriptor, typed as 5,000,000 | the `--help` row holding the figure |
| `control-constant-pool` | the refusal's position, back to 0:0 | the constant-pool check, and only it |

`rows-after-controls.log` is the added rows after every revert, all answering as declared. `tests.log`
is the solution's test projects, every test passing.

## 4. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The rest of JSP-10.** The host's `read` and `$262`'s `createRealm`, `evalScript` and
  `detachArrayBuffer` are still present and throwing; the truth of every refusal reason is not audited;
  the allowance defaults themselves are a decision this record does not take.
- **The comparison engine's reading of a UTF-16 file.** The parity roadmap says it runs one. Node 22 on
  this machine refuses the UTF-16LE fixture, so that half of the section's claim was not reproduced.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 5. What was run

```text
python3 docs/evidence/jsp-10-host-001/collect.py
```

## 6. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.2`, for the fixtures' answers.
