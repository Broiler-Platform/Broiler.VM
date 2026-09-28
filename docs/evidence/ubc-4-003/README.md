<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-003

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family. This bundle is the second half of exit-gate clause 3 for the region rows: every
load, every store and `memory.size` compared with the primitive table's region primitive over a retained
region input corpus, in the three publish modes, with a negative control.
**Collected:** 2026-09-28, at commit `83593ba`, from a clean tree, by `collect.py` in this directory.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Universal bytecode format version:** 1, unchanged.
**Status of the milestone after this collection:** `In progress`. Clause 3 is met: its first half on
bundle [`ubc-4-002`](../ubc-4-002/README.md), its second half on that bundle for the numeric rows and on
this one for the region rows. Clauses 4, 5 and 6 are unmet, as `ubc-4-002` records, and nothing here
bears on them. The milestone is not accepted: under [`docs/mvp.md`](../../mvp.md) no milestone is
accepted while review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-003 |
| Milestone | UBC-4, exit-gate clause 3, second half, for the region rows |
| Commit of the change | `83593ba`, on top of `08e3346`, which merged UBC-4 as it stood at `ba726ed` into the branch of PR #102 |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, which runs the lane, and `Broiler.VM.Composition.Ubc.Fixture`, which writes and replays the corpus; the register rows of [`docs/compositions.md`](../../compositions.md), unchanged |
| RID | `linux-x64` |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |
| Evidence class | a differential check over a retained input corpus, run in three publish modes, with a negative control; a corpus replay and rule runs; no measurement of performance of any kind |

## 2. What this bundle demonstrates

| Clause | What it asks | Status | Where it is shown |
|---|---|---|---|
| 3, second half, region rows | Every `Primitive` row has a reference handler the E2 corpus reaches | met for the region rows | The harness's `--regions` lane in `run-harness-jit.log`, `run-harness-trimmed.log` and `run-harness-aot.log`: the twenty-four region rows of the `broiler.webassembly.slice` table - fourteen loads, nine stores and `memory.size`, `0x28` to `0x3F` - each reached by the input lines of `src/tests/corpus/ubc-2/regions.txt` that name its primitive, and each agreeing: the reference's trap is the row's mapping of the primitive's trap, its value bits are the primitive's, and the region the reference ran over equals the one the primitive ran over after every access, loads and traps included. The lane fails a row no input line names. `harness-modes.log`: the three modes print the same lines |
| 3, second half, numeric rows | as above | met, and unchanged | The `--primitives` lane in the same three logs: 111 rows outside the control and the 12 of the control agree, as on `ubc-4-002` |
| 3, first half | Every `WasmOpcode` member has a row of Appendix C's table or a common mapping | met on `ubc-4-002`, unchanged | `WasmLowering.cs`, `WasmFamilyTable.cs` and `WasmOpcode.cs` are byte-identical to what `ubc-4-002` read at `15d7a02` |

**The reference is the profile's own, not the table's.** The family executes a region row through the
table's region primitive, so its handler compared with the table would be the table compared with
itself. The reference the lane reads through `WebAssemblyProfile.TryEvaluateReferenceAccess` is
`WasmReferenceMemory`: the retired bare-module interpreter's `MemoryAccess` dispatch, its memory
instance's `TryLoad` and `TryStore`, and its `memory.size` arm, recovered from `origin/main` and unchanged
but for the retired value slot's conversions, which are written out. It is the same arrangement the
numeric rows have in `WasmReferenceNumerics`.

**The corpus.** `regions.txt` holds 10,203 inputs, written and replayed by the fixture root beside the
primitive inputs (`fixture-corpus.log`: `regions.txt 10203 inputs, 0 answers differ`, and every other
retained file intact). An access is tried over an empty region and a sixteen-byte one whose bytes follow
the rule its header states, at dynamic addresses on both sides of every width's last fitting byte and at
the top of the address space, with static offsets up to `0xFFFFFFFF`, and for a store with values whose
bytes differ from the fill, with high bits set and NaN payloads of both widths; `memory.size` is asked
for sizes 0, 1 and 2. Every row has inputs that answer and inputs that trap, the ten- to
hundred-and-fifty-per-row answering ones including the last fitting address of each width.

**The negative control.** `control-failing.log` applies `control.patch` to the reference arms - it makes
`i32.load8_u` sign-extend, and every store write one byte past its window - builds, runs the lane, and
reverts: the lane fails on `wasm.i32.load8_u` for its value and on all nine store rows for the regions
afterwards, fourteen of twenty-four rows agreeing, exit 1. The two faults are the two ways a region arm
can be wrong - what it answers and what it writes - and the lane catches each without the corpus's
recorded answers, which it does not read.

## 3. What this bundle does not demonstrate

- **Clauses 4, 5 and 6.** Unchanged since `ubc-4-002`; this bundle does not touch the predeclared rule,
  population A or the store's retention report.
- **That the family's handler is right.** The lane holds the profile's reference arms to the table. The
  family's handler calls the table, and whether the whole path answers what a module expects is what the
  harness's execution checks and corpus replay show, not this lane.
- **A second architecture.** One RID, `linux-x64`, and no `arm64`.
- **Review.** Nothing here has been read by anyone but its author.

## 4. What was run

```text
python3 docs/evidence/ubc-4-003/collect.py --rid linux-x64
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-4-003 --milestone UBC-4 --evidence-class differential-check-and-publish-and-run ...
```

`collect.py` refuses a tree that is not clean outside this directory, builds the solution with warnings
as errors, publishes the harness trimmed and Native AOT, runs its catalog and its `--regions` and
`--primitives` lanes in each of the three modes, compares the modes' lines, replays the fixture corpus,
runs the negative control and reverts it, runs the architecture suite, and checks the tree is clean
again. Its header says what each file is. Every log hides the checkout's path as `<root>`. The manifest
command's remaining arguments are recorded in `manifest.json`.

## 5. Environment

Ubuntu 24.04 on `x86_64`, the .NET SDK `10.0.401`, Python 3, and clang 18 with lld
for the Native AOT link; the checkout at the commit above with nothing outside this directory changed.
The JIT runs use the framework-dependent build output; the trimmed runs a self-contained publish; the
Native AOT runs the published image, an ELF executable with no managed assembly beside it.
