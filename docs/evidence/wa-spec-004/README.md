<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-SPEC-004

**What this is:** a working record of the WebAssembly profile, and the run that sets WA-4's ratchet. It
retains the evidence for four clauses of WA-4's exit gate:
- **The ratchet.** The malformed and invalid families run to completion and publish their totals from an
  exact commit, an exact suite revision and a published effective limit vector, and that run sets the
  ratchet for those two families and no others.
- **The self-check's negative control.** A scoring regression is injected, observed and reverted, on
  every run.
- **The configuration failures** a lane without shards or scope manifests can show, each named and each
  a failure.
- **The per-family totals,** in the roadmap's six counts.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md).**
WA-4 is already `In progress`, and this record demonstrates no gate whole.

**Collected:** 2026-09-29, at commit `dc80b67`, from a clean tree, by `collect.py` in this directory.
- **The change:** `668afb0` (the self-check's control, the configuration failures and the family totals)
  and `b089bc9` (the ratchet).
- **This directory's tooling:** `dc80b67`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs, the ratchet at
  [`src/tests/wasm/spec/wasm-spec.ratchet`](../../../src/tests/wasm/spec/wasm-spec.ratchet) - byte for
  byte the file this run wrote - and dated notes in the profile's ledger.

**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-SPEC-004 |
| Milestone touched | WA-4 (the ratchet, the self-check's negative control, the configuration failures, the family totals) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) |
| Commit | `dc80b67` |
| Effective limit vector | as `set-run.log` prints it and the ratchet records it |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. The ratchet

Roadmap section 15: "The first per-family totals admitted for a manifest by the milestone that scores it
are the floor. No later run of that manifest regresses against them." *Admitted* is not the ledger's
`Accepted`: correction WAC-19 records that a ratchet only an accepted milestone could set is one nothing
could ever set.

`set-run.log` is the run that sets it: every command of the pinned scripts on one line, then each
family's totals, then the ratchet written. It holds two floors, the passing totals of the two families
WA-4 scores:

| Family | Selected | Executed | Passed (the floor) | Failed | Skipped |
|---|---|---|---|---|---|
| malformed | 1091 | 661 | 658 | 3 | 430 |
| invalid | 989 | 989 | 979 | 10 | 0 |

The skipped malformed commands are quoted text modules. The profile never receives text, so the lane
does not score them, as record UBC-4-005's rule first set out. Timed out is zero for every family by
construction: the lane runs no timer, and every command is bounded by its runtime's budget.

The ratchet records the suite revision, the feature manifest and the effective limit vector it was set
under, and the lane compares a run with it only when all three match. `hold-run.log` is the same scripts
held to it: both families at their floors, held. `determinism.log` says the two runs printed the same
lines, the ratchet's own excepted.

**No other family is ratcheted.** The other families' totals are published beside these, and a ratchet
file naming one of them is refused.

## 3. The refusals

`refusals.log` runs each way the lane refuses, with the exit code each must answer: one for a ratchet
regression or a floor that is not comparable, four for a configuration failure.

| Case | Exit |
|---|---|
| A floor above the run: malformed raised by a thousand | 1 |
| A floor of another revision | 1 |
| A floor set under other limits: a tighter fuel ceiling | 1 |
| A floor naming the trap family, which WA-4 does not ratchet | 4 |
| A write lower than the floor it would replace: invalid raised by a thousand, then written over | 1 |
| A ratchet asked for with no pinned revision: *missing suite revision* | 4 |
| An empty selection | 4 |
| A selection whose every command is skipped: *no executed tests*, and the malformed family selecting and executing none | 4 |

The roadmap's closed set of configuration failures also holds inconsistent shard configuration and a
scope manifest naming a file the suite does not contain. This lane runs no shards and reads no scope
manifest, so neither can arise, and neither is shown.

## 4. The self-check's negative control

Every run of the lane, `set-run.log` and `hold-run.log` among them, runs the self-check against the built
profile before any script, then runs its negative control. The scorer is made to pass a malformed or an
invalid assertion on any refusal, which is how it scored until 2026-09-28. The same self-check script
must then disagree with its declared verdicts, and it disagrees with three. The injection is reverted
before any script of the suite is read.

`control-ignored-injection.patch` makes the scorer ignore the injection. `control-ignored-injection.log`
applies it, runs the lane, and reverts it. The control disagrees with nothing, so the self-check fails and
no script runs (exit three).

`harness.log` is the harness root's own run over the retained corpus after the revert, every lane
passing. `tests.log` is the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Shards and their merge.** No test is assigned a shard. So a missing shard report, an inconsistent
  shard configuration and a pre-sharding selected count are not shown.
- **The selection pipeline and scope manifests.** Discovery, known-incorrect exclusion, manifest scope
  filtering and per-file selectability are not recorded as stages. Only the quoted-module exclusion
  exists.
- **The failure queue.** No failure manifest exists, so none is proved a queue.
- **The tooling's regression tests.** The self-check and its control test the scorer. The harness, the
  merge, the audit and the scope tooling carry no regression suite of their own.
- **The co-signature.** The release owner's confirmation that the core's third-party claim stays scoped
  is owed, as the ledger's section 3 records.
- **Resolved once before any shard.** The pin is checked by re-digesting the extracted directory before
  any script runs, but there is no shard for it to precede.
- **Three publish modes, a second RID, review.** Framework-dependent JIT on `linux-x64` only, and read by
  nobody but its author.

## 6. What was run

```text
python3 docs/evidence/wa-spec-004/collect.py
cp docs/evidence/wa-spec-004/wasm-spec.ratchet src/tests/wasm/spec/wasm-spec.ratchet
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-spec-004 --milestone WA-4 --evidence-class working-record ...
```

A later run is held to the floor with the lane's `--ratchet src/tests/wasm/spec/wasm-spec.ratchet`.

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3.
