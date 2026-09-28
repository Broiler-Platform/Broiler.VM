<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-SPEC-001

**What this is:** a working record of the WebAssembly profile, not a milestone bundle. It retains the
evidence for four corrections the specification's core test scripts led to. They sit in three milestones'
gates: WA-2's decoder, WA-3's validator and WA-5's family. It also retains the evidence for one correction
to how the harness root's `--spec` lane scores a module assertion, which is WA-4's method. **It moves no row
of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md)**, and it demonstrates no gate clause
whole.
**Collected:** 2026-09-28, at commit `8293a2c`, from a clean tree, by `collect.py` in this directory. The
change is `12b2d76` (the profile) and `df9e2d4` (the lane); `8293a2c` adds this directory's tooling. The
commit that retains this directory also corrects the profile's ledger, and no product, harness or test file
differs from `8293a2c` in it. The base is `d380b5e`, the commit before the change, in a working tree of its
own.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-SPEC-001 |
| Milestones touched | WA-2 (custom-section names, import entries), WA-3 (a function type with two results), WA-5 (`call_indirect`'s type check), WA-4 (the lane's scoring) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md). The lane checks the extracted directory against the pin before it reads a script |
| Specification document | Not pinned. The ledger records that as its own open dependency |
| Base / after | `d380b5e` / `8293a2c` |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. What changed, and why

The scripts at `d380b5e`, as bundles `ubc-4-005` and `ubc-4-006` retain them, showed four defects the
profile carried at both commits those bundles compared:

- **A custom section's name was never read** (WA-2). The decoder stepped over the whole section, so a name
  that was not UTF-8, or that ran past its section, was admitted. The name is now read and held to the
  format's UTF-8 rule, and one longer than its section is refused as a length disagreement. The rest of the
  section is still stepped over unread.
- **An import section was refused at its count** (WA-2). A malformed import name, kind or description was
  answered as an import this profile does not admit, and so was a module malformed after its imports.
  Every entry is now read by the readers the module's own definitions use. The refusal of a declared import
  is made where the section is read, and held until the whole module has decoded.
- **A function type with two results was admitted** (WA-3). Version 1.0's validation holds a result vector
  to one. The validator refuses more with the new code 2711, `FunctionTypeResultArityAboveOne`, and the
  reason `SemanticValidationFailed`.
- **`call_indirect` compared type indices** (WA-5). The format's check is structural. The nominal check
  kept milestone UBC-4's parity with the retired interpreter, and it trapped a call to a callee declared
  under another index of the same function type. The family now compares the two Types rows parameter for
  parameter and result for result.

The lane's scoring (WA-4's method) was lenient in a way that reported failing tests as passes:

- **`assert_malformed` and `assert_invalid` passed on any refusal**, and `assert_unlinkable` on any module
  without an instance. A refusal now passes only the assertion its category answers, read from the core's
  reason. `SemanticValidationFailed` means decoded and invalid. `Truncated`, `MalformedEncoding`,
  `UnknownFormatVersion` and `InconsistentStructure` mean not decoded. A resource exhaustion, a code
  outside the profile's module codes, and `UnknownFeature` pass neither. An unlinkable module has to
  verify and then fail to instantiate.
- **The pass that found a malformation does not decide its category.** This profile's validator reads the
  function bodies, so a malformation inside one carries a validation-band code with `MalformedEncoding`,
  and it is scored as malformed.
- **The self-check grows from 19 to 26 commands with declared verdicts.** The new ones include the
  malformed-before-invalid fixture WA-4's method asks for.

## 3. Results

`comparison.log` is `compare.py`'s join of `before-run.log` and `after-run.log`. Both runs name the same
19,262 commands in the same order, each run taken twice with the same lines (`determinism.log`), under the
effective limits each log prints on its second line. Per command family, pass / fail / excluded:

| Family | Before | After |
|---|---|---|
| all | 18,369 / 463 / 430 | 18,460 / 372 / 430 |
| `assert_malformed` | 481 / 180 / 430 | 658 / 3 / 430 |
| `assert_invalid` | 985 / 4 / 0 | 955 / 34 / 0 |
| `assert_unlinkable` | 95 / 0 / 0 | 18 / 77 / 0 |
| `assert_return` | 13,736 / 158 / 0 | 13,757 / 137 / 0 |
| every other family | unchanged | unchanged |

**681 commands moved, and each is in a class `compare.py` names from its two answers alone. None is in no
class:**

| Class | What moved | Commands |
|---|---|---|
| (c) | an `assert_malformed` module admitted before, refused after as not decoded: every one a custom-section name - `utf8-custom-section-id.wast`, two in `custom.wast`, two in `binary-leb128.wast` | 180, fail to pass |
| (i) | a module refused before as an unadmitted import, refused after as not decoded: the import names of `utf8-import-field.wast` and `utf8-import-module.wast`, and ten in `binary-leb128.wast`, `binary.wast` and `globals.wast` | 362, pass to pass |
| (r) | a module admitted before, refused after with code 2711: `func.wast` and `type.wast` | 4, fail to pass |
| (t) | an action that trapped before as an indirect call type mismatch and returned after: `call_indirect.wast`, `func_ptrs.wast`, `func.wast` | 21, fail to pass |
| (s) | the same answer, a verdict the scoring moved | 114, pass to fail |

The 114 in (s) were failures reported as passes. They are:

- **77 `assert_unlinkable`**: every module that imports, refused at verification. Nothing links, and
  linking is WA-6's.
- **34 `assert_invalid`, in two kinds:**
  - 16 answered as not decoded where the format validates. The decoder refuses a table or memory whose
    minimum is above its maximum (2305), a memory above the format's page maximum (2306), and a constant
    expression of other than one instruction (2602).
  - 18 answered as unadmitted features: imports (2403), a constant expression's instruction (2601), and
    two memories or two tables (2504, 2505).
- **3 `assert_malformed`**, each a finding of its own:
  - `binary.wast:48` declares more locals than the host's declared-count ceiling and is answered as an
    exhaustion before the format's own limit is reached.
  - `binary.wast:73` declares one element segment more than it has; the decoder reads the next section's
    byte as a segment's form and refuses it as unadmitted.
  - `binary.wast:81` is invalid before it is malformed inside one function body; the validator reads the
    body and answers the invalid half first.

## 4. Negative controls

Each correction has a patch in this directory that undoes it. `collect.py` applies each patch, runs the lane
that has to fail, reverts the patch and rebuilds. `controls-passing.log` is the same lanes after every
revert.

| Control | Undoes | Fails on |
|---|---|---|
| `control-custom-name.patch` | the custom-section name read | the harness root's corpus: the three derived custom-section rows and the five recorded inversion rows in the canonical module's custom-section name |
| `control-import-early.patch` | holding the import refusal until the module has decoded | the corpus: the three derived import-entry rows and the import in a module malformed after it |
| `control-result-arity.patch` | the two-result refusal | the corpus: the derived two-result row |
| `control-nominal.patch` | the structural type check | the harness root's call checks: a callee declared under another index of the same type |
| `control-scoring.patch` | the scoring by category | the `--spec` lane's self-check, which stops the run |

`harness.log` is the harness root's whole run over the retained corpus, and `corpus-integrity.log` is
`eng/wasm-corpus-integrity.py` over it: the manifest against the directory both ways, and four byte
mutations each detected.

## 5. What this record does not demonstrate

- **Any milestone's gate.** Each correction is one clause's worth of one gate at most. The ledger's rows
  stay where they are.
- **A conformance result.** The lane's per-command-family totals are not WA-4's per-assertion-family
  totals: there is no selection pipeline, no shard, no scope manifest, no skipped or timed-out count, no
  configuration failure, no failure queue and no ratchet per family.
- **Three publish modes, a second RID, review.** The lane and the harness ran framework-dependent on
  `linux-x64` only, and nothing here has been read by anyone but its author.
- **Anything about bundles `ubc-4-005` and `ubc-4-006`.** They compared answers under their own rules, and
  their verdicts were scored the old way. Those bundles are immutable, and this record reads them only as
  where the defects were first seen.

## 6. What was run

```text
python3 docs/evidence/wa-spec-001/collect.py
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-spec-001 --milestone WA-2,WA-3,WA-4,WA-5 --evidence-class working-record ...
```

The manifest is written by the universal bytecode programme's manifest script, which judges nothing and
names every source by its git blob at `8293a2c` and every retained file by hash. Logs hide the checkout's
path as `<root>` and the scratch directory as `<scratch>`.

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3.
