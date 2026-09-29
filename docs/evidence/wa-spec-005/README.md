<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-SPEC-005

**What this is:** a working record of the WebAssembly profile. It retains the evidence for WA-4's oracle
machinery, over the pinned scripts:
- **The selection pipeline and its scope manifest.**
- **Content-independent sharding, and the merge** that proves the shards covered the whole selection.
- **The failure queue.**
- **The tooling's own regression suite.**

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md).**
WA-4 is already `In progress`, and this record demonstrates no gate whole.

**Collected:** 2026-09-29, at commit `33feb0d`, from a clean tree, by `collect.py` in this directory.
- **The change:** `c4841b4`.
- **This directory's tooling:** `33feb0d`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and the failure queue at
  [`src/tests/wasm/spec/wasm-spec.queue`](../../../src/tests/wasm/spec/wasm-spec.queue), byte for byte the
  file the merge wrote. It also adds dated notes in the profile's ledger.

**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-SPEC-005 |
| Milestone touched | WA-4 (the selection pipeline, sharding, the merge, the failure queue, the tooling's regression suite) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) |
| Scope | [`wasm-spec.slice.scope`](../../../src/tests/wasm/spec/wasm-spec.slice.scope): every script of the revision |
| Shards | four |
| Commit | `33feb0d` |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. The pipeline, the shards and the merge

`whole-run.log` is the whole selection, one answer line per command. It is held to the ratchet that
[record WA-SPEC-004](../wa-spec-004/README.md) set, and holds. Its selection line records the pipeline stage
by stage:
- 73 candidates;
- none known incorrect;
- none out of the slice's scope;
- none unselectable;
- 73 selected before sharding.

The scope manifest names every script. Scoping one out, such as a script whose modules all import, is WA-4's
owner's to decide, so none is; those scripts' failures are in the queue.

The same selection runs in four shards. `shard-<i>.log` holds each shard's own lines, and `shard-<i>.report`
its report. A script's shard is the first eight bytes of the SHA-256 of its name, read big-endian, modulo
four. The shards run 12, 17, 27 and 17 scripts. `comparison.log` says the four shards' 19,260 answer lines are
the whole run's, line for line, and the merge's family totals are the whole run's.

`merge.log` is the merge of the four reports. It states that they cover the 73 scripts selected before
sharding, holds the merged totals to the ratchet, and writes the failure queue.

## 3. The failure queue

`wasm-spec.queue` lists the 15 scripts with a failing command, and how many each has. It is the file the
merge wrote. `merge-queue.log` is the merge again, held to it: every entry confirmed, and no failure
unlisted.

A listed script leaves the queue only in a change that carries its regression. So a run reports a listed
script that passes now. It also reports a listed name the selection does not hold, and a failing script the
queue does not list.

## 4. The refusals

`refusals.log` runs each one, with the exit code each must answer: four for a configuration failure, one for
a queue that disagrees with the run.

| Case | Exit |
|---|---|
| A merge missing one shard's report: *incomplete coverage* | 4 |
| A merge of reports run under different limits: *inconsistent shard configuration* | 4 |
| A merge of a report cut short before its closing line: *a shard that did not finish* | 4 |
| A scope manifest naming a script the suite does not contain | 4 |
| A ratchet held by one shard, when a ratchet is held by a whole selection | 4 |
| A queue holding a hand-written entry for a script the suite does not have | 1 |
| A queue listing a script that passes | 1 |
| A queue missing a script that fails | 1 |

## 5. The tooling's regression suite, and its control

Every run of the lane and every merge first runs the tooling's 13 checks, before any script or report is
read. They hold the sharding, the scope reader, the merge, the merge's classifier of a shard that did not
finish, and the queue to recorded inputs with declared answers. The shard assignments they pin were computed
apart from the harness, from each name's SHA-256.

`control-partial-merge.patch` removes the merge's two coverage checks: the missing shard index, and the scripts
that do not add up to the selected count. `control-partial-merge.log` applies it and merges the reports with
one missing. The suite's `merge-reports-a-missing-shard-as-incomplete-coverage` check fails, so the lane
refuses to merge anything (exit three). The patch is then reverted.

`harness.log` is the harness root's own run over the retained corpus after the revert, every lane passing.
`tests.log` is the solution's test projects, every test passing.

## 6. What this record does not demonstrate

- **The co-signature.** The release owner's confirmation that the core's third-party claim stays scoped is
  owed, as the ledger's section 3 records.
- **Known-incorrect entries.** The pipeline has the stage and its failure, and no script is listed, because
  none of the pinned revision's scripts is known to be wrong.
- **A cache.** The pin is re-read and re-digested by every shard before it reads a script; nothing is cached
  under a key.
- **An audit command.** WA-4's next-action list names one. It is not built, and this record does not say what
  it would check.
- **Three publish modes, a second RID, review.** Framework-dependent JIT on `linux-x64` only, and read by
  nobody but its author.

## 7. What was run

```text
python3 docs/evidence/wa-spec-005/collect.py
cp docs/evidence/wa-spec-005/wasm-spec.queue src/tests/wasm/spec/wasm-spec.queue
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-spec-005 --milestone WA-4 --evidence-class working-record ...
```

## 8. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3.
