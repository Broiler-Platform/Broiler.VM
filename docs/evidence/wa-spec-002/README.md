<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-SPEC-002

**What this is:** a working record of the WebAssembly profile, not a milestone bundle. It retains the
evidence for moving three validation rules from the decoder, which answered them as malformations, to
the validator, where the format puts them. They are WA-3's: the gate asks that every validation rule
the manifest requires be produced by a named case and map onto exactly one core invalid-artifact
reason. **It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md)**,
and it demonstrates no gate clause whole.
**Collected:** 2026-09-28, at commit `ec7f4f9`, from a clean tree, by `collect.py` in this directory.
- **The change:** `f941766`.
- **This directory's tooling:** `d086a06`.
- **Draft decision WAD-0004:** `ec7f4f9`. It changes no product, harness or test file.
- **The retaining commit** corrects two notes in the diagnostics registry and the profile's ledger,
  and changes no behaviour.
- **The base:** `f842c2f`, the commit before the change, run in a working tree of its own.

**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-SPEC-002 |
| Milestone touched | WA-3 (validation rules, and the fusion decision its gate asks for, drafted as [WAD-0004](../../../src/Broiler.VM.Profile.WebAssembly/docs/decisions/0004-decode-and-validate-fused-within-a-function-body.md)) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) |
| Base / after | `f842c2f` / `ec7f4f9` |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. What changed, and why

Record [WA-SPEC-001](../wa-spec-001/README.md) scored the specification's scripts by what a refusal says.
It found the decoder refusing, as malformations, three things the format decodes and validation
refuses:

- **Limits whose minimum exceeds their maximum.** Limits are now read whole, and the validator refuses
  a table's or a memory's with the new code 2712, `TableOrMemoryMinimumAboveMaximum`.
- **A memory of more than 65,536 pages.** The validator refuses it with the new code 2713,
  `MemoryPagesAboveFormatLimit`.
- **Constant expressions of other than one constant instruction.** The decoder read exactly one
  constant instruction and its closing byte. It now reads the format's instruction sequence, up to the
  `end` that closes no level, reading the immediates of every instruction this format version defines.
  - The validator refuses an expression holding an instruction that is not constant with the new code
    2714, `ConstantExpressionNotConstant`.
  - It refuses one leaving other than one value with the existing type mismatch, 2709.
  - A byte a later version defines is refused in decoding as unadmitted, with the new code 2604. A byte
    naming no instruction is refused as malformed, with the new code 2605. The validator draws the same
    line inside a function body.

Codes 2601 and 2602 are emitted by nothing now, and stay numbered. Codes 2305 and 2306 are no
longer emitted by the decoder. The family's verifier hook still emits them, over the limits a universal
bytecode artifact's module definitions carry, and `control-limits.log` shows it doing so: with the
validator's check removed, the translator admits the four limit rows, and the core refuses each through
the hook. The replay names that as a defect of the translator or the hook, because the corpus holds that
refusal to be empty. *(Corrected before this directory was committed. `f941766`'s registry notes and
commit message said 2305 and 2306 were emitted by nothing, and the retaining commit corrects the
registry's notes; it changes no behaviour.)*

## 3. Results

`comparison.log` is `compare.py`'s join of `before-run.log` and `after-run.log`. Both runs name the same
19,262 commands in the same order, and each run was taken twice with the same lines (`determinism.log`).

| Family | Before: pass / fail / excluded | After |
|---|---|---|
| all | 18,460 / 372 / 430 | 18,484 / 348 / 430 |
| `assert_invalid` | 955 / 34 / 0 | 979 / 10 / 0 |
| every other family | unchanged | unchanged |

**24 commands moved, every one from fail to pass. Each is in a class `compare.py` names from its two
answers, and none is in no class:**

| Class | What moved | Commands |
|---|---|---|
| (l) | Refused before as not decoded, with 2305 or 2306. Refused after as invalid, with 2712 or 2713. All are in `memory.wast` | 7 |
| (k) | Refused before with 2601 or 2602. Refused after as invalid, with 2714 or 2709. They are in `data.wast`, `elem.wast`, `func_ptrs.wast` and `globals.wast` | 17 |

**The ten `assert_invalid` failures left are all features this profile does not admit.**
- Eight are imports (2403). Linking is WA-6's.
- Two are a second memory or table (2504, 2505), which version 1.0 calls invalid.

Each is answered as an unadmitted feature before its validity is judged.

## 4. Negative controls and the corpus

The corpus re-derives four rows and adds nine:
- **Re-derived:** the two limit rows, which were refused in decoding and are now refused by the
  validator, and two constant-expression rows.
- **Added:** two limit rows (a memory's maximum, and a table's limits), and seven constant-expression
  rows. The seven are:
  - two constant instructions;
  - none;
  - a block, which needs the nesting counted;
  - a load, whose second immediate's byte names no instruction;
  - a branch table;
  - an instruction a later version defines;
  - a byte naming no instruction.

All thirteen were written down before the run, and the writer agreed with each. Nine recorded
inversion rows moved within their `sound-either-way` invariant. `harness.log` is the harness root's run
over the 307 entries. `corpus-integrity.log` holds the corpus against its manifest both ways, and shows
four byte mutations each detected.

| Control | Undoes | Fails on |
|---|---|---|
| `control-limits.patch` | the validator's limits and page checks | the four limit rows, which the family's hook then refuses with 2305 and 2306 |
| `control-constant.patch` | the validator's constancy and count checks | the constant-expression rows the validator refuses |
| `control-depth.patch` | the decoder's count of nested levels | the block row, whose expression ends early |
| `control-immediates.patch` | a load's second immediate | the load row, whose offset byte is read as an instruction |

`collect.py` applies each patch, runs the corpus, reverts the patch and rebuilds. `controls-passing.log`
is the run after every revert.

## 5. What this record does not demonstrate

- **WA-3's gate.** The registry is still not published and bound in both directions, and the nesting
  corpus is not written. The fusion decision is drafted as WAD-0004 and not taken.
- **A conformance result.** As WA-SPEC-001's section 5 says: the lane's totals are not WA-4's.
- **Three publish modes, a second RID, review.** Framework-dependent JIT on `linux-x64` only, and read by
  nobody but its author.

## 6. What was run

```text
python3 docs/evidence/wa-spec-002/collect.py
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-spec-002 --milestone WA-3 --evidence-class working-record ...
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3.
