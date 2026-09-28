<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-SPEC-003

**What this is:** a working record of the WebAssembly profile, not a milestone bundle. It retains the
evidence for three things WA-3's gate asks for:
- **The registry.** The profile's diagnostic-code registry is published, and rule W3 binds it in both
  directions.
- **One reason per code.** One code that carried two core reasons now carries one.
- **The phase-order cases.** The corpus has named cases that fail when decoding and validation are
  fused at module granularity.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md)**,
and it demonstrates no gate clause whole.

**Collected:** 2026-09-28, at commit `b253db8`, from a clean tree, by `collect.py` in this directory.
- **The change:** `7789676`.
- **This directory's tooling:** `b253db8`. It changes no product, harness or test file.
- **The retaining commit** changes no behaviour. It adds this record, and dated notes in the profile's
  ledger.
- **The base:** `7c2b45b`, the commit before the change, run in a working tree of its own.

**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-SPEC-003 |
| Milestone touched | WA-3 (the diagnostic registry, one reason per code, and the phase-order clause) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) |
| Base / after | `7c2b45b` / `b253db8` |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. What changed, and why

**The registry.** [`registry.txt`](../../../src/Broiler.VM.Profile.WebAssembly/docs/diagnostics/registry.txt)
is revision 1. It has one row per member of `WebAssemblyDiagnosticCode`, and each row states:
- the passes that emit the code;
- the carrier it travels on: the translation's answer, the core's verifier outcome, or the trap
  payload;
- its one core reason;
- the named case that reaches it.

Its header states which fields of the core's position record each carrier fills. It records one
disagreement as found and does not resolve it: the trap payload uses the fields differently from the
verification path. It also states where the revision is recorded: the corpus manifest's header, which
the writer dates from `CorpusStore.RegistryRevision`. Section 8 of the roadmap left the field open.

**Rule W3** holds the registry to five things, all read off disk:
- **The enumeration**, both directions.
- **The profile's source.** Every emission site carries its row's reason, in the passes the row
  names. The rule reads each shape the source emits in, and reports what it cannot follow. The shapes
  are:
  - a direct call;
  - a forwarding helper;
  - the family hook's reason table;
  - the family's payload mapping from a trap kind to a code.
- **The corpus.** A corpus row names a derived entry recording its code, and every entry recording a
  code records that row's reason.
- **The execution checks.** An execution row names a trap kind the checks expect.
- **The corpus manifest's revision line.**

The rows no named case reaches are listed in the rule, in its own words, and the registry must say
them in those words.

**One code, two reasons.** Writing the registry found `FunctionBodyLengthMismatch`, 2503, emitted with
two reasons:
- `Truncated`, for a body whose declared size runs past the artifact's end;
- `InconsistentStructure`, for a body whose locals overrun its declared size.

The first now carries `Truncated`, 2106, the code a section declared past the artifact's end already
carries. Among the corpus's recorded rows only `inversion-0071` moved, from 2503 to 2106. Its reason and
its invariant are unchanged.

**The corpus** gains nine derived rows:
- **Codes no derived row reached:**
  - the tag section;
  - the data count;
  - a body whose locals overrun it;
  - a segment's first field, twice.
- **The corrected refusal:** a body declared past the end.
- **Three `phase-order` modules.** Each is invalid at a byte before the one at which it is malformed:
  - a function type with two results, before one whose tag is not `0x60`;
  - a memory whose minimum is above its maximum, before a section identifier that names nothing;
  - a body that adds with nothing on the stack, before the same.

  Each must be refused as malformed, with a decoding code.

## 3. Results

`comparison.log` is `compare.py`'s join of `before-run.log` and `after-run.log`. Both runs name the same
commands in the same order, and each run was taken twice with the same lines (`determinism.log`).

| Family | Before: pass / fail / excluded | After |
|---|---|---|
| all | 18,484 / 348 / 430 | 18,484 / 348 / 430 |
| every family | unchanged | unchanged |

**No command's answer or verdict moved.** `compare.py` names one class it would expect, (t): a script
module answered `Truncated` with 2503 before and 2106 after. It counts none. No module in the pinned
scripts reaches the corrected refusal: a function body declared past the end of the artifact.

The change is a registry, a rule, a corrected code and nine corpus rows. So this run shows that
publishing and binding the registry changed no answer the lane scores, and it shows nothing else.

**Each control fails exactly its own target, and nothing else:**
- Each of the three phase-order controls leaves 315 of the 316 entries reproducing. The one that
  fails is its own phase-order entry, answered invalid with the validation code the fused check gives:
  - 2711 for the function type;
  - 2712 for the memory;
  - 2803 for the body.
- The fourth control fails rule W3's test, which reports `FunctionBodyLengthMismatch` emitted with two
  reasons.

After every revert, the corpus reproduces whole, rule W3 passes, and the lane's self-check answers each
of its declared verdicts (`controls-passing.log`). `tests.log` is the solution's test projects, every
test passing.

## 4. Negative controls and the corpus

All nine new rows were written down before the run, and the writer agreed with each. `harness.log` is
the harness root's run over the corpus. `corpus-integrity.log` holds the corpus against its manifest both
ways, and shows four byte mutations, each detected.

| Control | Undoes | Fails on |
|---|---|---|
| `control-phase-type.patch` | the order of the phases, for one rule: the decoder refuses a function type of two results as it reads it | `phase-order-an-invalid-function-type-before-a-malformed-one` |
| `control-phase-memory.patch` | the same, for a memory's limits | `phase-order-an-invalid-memory-before-a-malformed-section` |
| `control-phase-body.patch` | the same, for every body: the decoder validates what it has read as soon as the code section is decoded, before the rest | `phase-order-an-invalid-body-before-a-malformed-section` |
| `control-two-reasons.patch` | the corrected refusal: a body declared past the end carries 2503 again | rule W3, which reports 2503 emitted with two reasons |

`collect.py` applies each patch, runs its lane, reverts the patch and rebuilds. `controls-passing.log`
holds the runs after every revert.

## 5. What this record does not demonstrate

- **WA-3's reachability clause.** The rule lists the registry's unreached rows, and the registry names
  them. No named case reaches:
  - the family hook's own codes, and the two it shares only with a retired decoder check. Every
    retained entry is a module the translator writes, so none reaches the hook's refusals of an
    artifact written some other way;
  - the translation's bounds;
  - the two defect codes;
  - the reader's malformation;
  - the trap the earlier specification revision named.
- **An execution row's code.** Its case is a check that expects a trap's KIND. The code travels beside
  the kind in the payload, and no check reads it.
- **The position disagreement.** The trap payload and the verification path fill the core's position
  record differently. The registry records this and nothing resolves it.
- **The fusion decision.** WAD-0004 is still Proposed. The phase-order cases hold the property at
  module granularity, which both routes keep. They do not decide the route.
- **A conformance result.** As WA-SPEC-001's section 5 says: the lane's totals are not WA-4's.
- **Three publish modes, a second RID, review.** Framework-dependent JIT on `linux-x64` only, and read by
  nobody but its author. The nesting corpus at and one level beyond the structural-depth ceiling is
  retained, and has not been run under Native AOT.

## 6. What was run

```text
python3 docs/evidence/wa-spec-003/collect.py
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-spec-003 --milestone WA-3 --evidence-class working-record ...
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3.
