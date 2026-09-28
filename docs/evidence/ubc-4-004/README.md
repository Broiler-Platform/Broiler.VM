<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-004

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family. This bundle is exit-gate clause 6 on the gate as revised on 2026-09-28: the
`LiveBytes` amounts an instance retains at instantiation, at a growth, at each kind of refused growth
and at disposal, and the fuel a refused growth spends, asserted by the harness over the store, in the
three publish modes, with two negative controls.
**Collected:** 2026-09-28, at commit `93c867e`, from a clean tree, by `collect.py` in this directory.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Universal bytecode format version:** 1, unchanged.
**Status of the milestone after this collection:** `In progress`. Clause 6 is met on its revised gate.
Clauses 4 and 5 are unmet, as `ubc-4-002` records, and nothing here bears on them. The milestone is not
accepted: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while review is deferred, and
nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-004 |
| Milestone | UBC-4, exit-gate clause 6, as revised on 2026-09-28 |
| Commit of the change | `93c867e`, on top of `8d097dc`, the commit that retained bundle `ubc-4-003` on the branch of PR #102 |
| Gate revision | The roadmap's section 11, clause 6, revised under the programme ledger's update rule 5 on the WebAssembly profile owner's decision of 2026-09-28 to keep the store's order; the superseded clause is quoted beside the revised one. The revision and this bundle land in the same change |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, which runs the checks; its register row of [`docs/compositions.md`](../../compositions.md), unchanged |
| RID | `linux-x64` |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |
| Evidence class | execution checks over a module driven through the core and read off the runtime's own budget, run in three publish modes, with two negative controls; the solution's test suites; no measurement of performance of any kind |

## 2. What this bundle demonstrates

The revised clause asks for six things. Every line named below is printed identically by
`run-harness-jit.log`, `run-harness-trimmed.log` and `run-harness-aot.log`, and `harness-modes.log`
says the three modes printed the same lines. The checks read the runtime's `LiveBytes` account after
every step of one module - a memory of one page with no declared maximum, and a table of three entries -
so an amount is what a host reads and not what the profile says it reported.

| What the revised clause asks | Status | Where it is shown |
|---|---|---|
| The refusal against the profile's own page ceiling is unchanged: nothing charged, minus one to the guest | met | `retention: grow(2000), past the page ceiling, retains nothing`: `i32 -1`, the account unchanged by it. The `memory` group's `grow(2000)` and `size` lines, which `ubc-4-002` read, print what they printed there |
| A page's bytes for each page of a memory's minimum, and four bytes for each table entry, at instantiation | met | `retention: instantiation retains its minimum page and four bytes a table entry`: `live bytes +65548`, a page and three entries of four bytes. The same amount at the start of both refused runs |
| The pages a growth adds | met | `retention: grow(2) retains the two pages it added`: `i32 1`, `live bytes +196620`, two pages more; `size after both` answers `3` |
| Nothing for a growth either kind of refusal refuses | met | The page ceiling's, above. A core budget's: `retention-refused-by-live-bytes: grow(1) ...` and `retention-refused-by-allocated-bytes: grow(1) ...`, each `live bytes +65548`, what instantiation took, under an instance ceiling half a page over it on the one dimension |
| All of it given back at disposal | met | The three `disposal gives every byte back` lines, `live bytes +0`, one after the admitted growth and one after each refusal. Section 3 says what these do not answer for |
| A growth a core budget refuses ends the operation as an exhaustion naming the dimension and the scope, and no guest code runs past it | met | The two `grow(1)` lines answer `ResourceExhaustion/CeilingReached/LiveBytes/Instance` and `ResourceExhaustion/AllowanceExhausted/AllocatedBytes/Instance`. The module's export grows and then runs a loop of fifty thousand iterations. Grown by none, under the same ceiling, it answers and spends `fuel 250006`; refused, it spends `fuel 4`, which is the growth and the instructions before it. The `the instance is faulted after the refusal and still holds its bytes` lines answer `InvalidState/TerminalFault` with the account unchanged: the core faults an instance whose operation was exhausted, always |
| The harness's existing memory checks pass over the store unchanged | met | The `memory` group in the three run logs: its nine calls answer, line for line, what they answer in `ubc-4-002`'s `after-run.log`. `# execution: 166 of 166 checks passed` in every mode, with the differential checks and the retained corpus replay after them |

**The amounts are the retired executor's.** The store this profile carried until UBC-4, on
`origin/main` at `35a4d90`, reported a memory's minimum in pages of `PageBytes` and four bytes a table
entry at instantiation (`WasmStore.cs`), and a growth's added pages after its allocation
(`WasmMemory.cs`). The checks spell those amounts out rather than reading them from the profile.

**The negative controls.** Each is applied to the store, built, run, reverted and built again, and each
log records all five steps.

- `control-retired-order.log` applies `control-retired-order.patch`, which puts back the retired
  executor's growth, in the order `origin/main`'s `WasmMemory.Grow` has it: a refused fuel or allocation
  charge answers the guest minus one and lets it go on, and the retention is reported after the
  allocation instead of being charged before it. **Two checks fail, 164 of 166 pass, exit 1**: the two
  `grow(1) ... runs nothing after it` lines, each with `fuel 250007`, the whole loop. **Every amount check
  passes under it**, and that is the core's doing, not the checks' blindness. The meter tests a
  retention at every level before it commits any of it, and a retention that a level refuses is
  latched, not committed (`VmMeter.ReportRetained`). So the account reads the same whichever order
  the store uses. What the retired order changes is that the store has already allocated the page the
  account refused, and the guest runs on. The first of those is not observable from outside an
  instance the core has faulted, and it is not claimed here. The second is what the fuel shows.
- `control-amounts.log` applies `control-amounts.patch`, which makes a table entry retain eight bytes
  rather than four, at instantiation and at release alike. **Twelve checks fail, 154 of 166 pass, exit
  1**: every check that reads an amount while the instance is alive, each twelve bytes over what it
  expects. The three disposal checks pass, for the reason section 3 gives.

**The suites.** `tests.log`: the contract and architecture suites, at the commit, both passing.

## 3. What this bundle does not demonstrate

- **Clauses 4 and 5.** Unchanged since `ubc-4-002`; this bundle does not touch the predeclared rule
  or population A.
- **That the store gives back what it retained.** The core releases whatever an instance's level still
  holds when the instance is disposed. So the disposal checks answer for what a host sees, and a store
  that forgot its own release would pass them. A control that dropped the table's release was run while
  the checks were written, and it passed them. It is not retained, because it shows nothing about the
  checks except this.
- **A refused growth's own fuel charge, and ceilings at other scopes.** A core budget's refusal is
  checked on `LiveBytes` and `AllocatedBytes` at the instance's scope, not on `Fuel`, and not under a
  runtime, invocation or aggregate ceiling.
- **Other module shapes.** One memory with no declared maximum, one table, and nothing imported. A
  declared maximum is refused by the same comparison as the page ceiling, but no check here declares
  one, and this build imports no memory.
- **That the retired order would allocate uncounted bytes.** As the control's paragraph says, this is
  not observable from outside.
- **A second architecture.** One RID, `linux-x64`, and no `arm64`.
- **Review.** Nothing here has been read by anyone but its author.

## 4. What was run

```text
python3 docs/evidence/ubc-4-004/collect.py --rid linux-x64
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-4-004 --milestone UBC-4 --evidence-class execution-check-and-publish-and-run ...
```

`collect.py` refuses to collect from a tree that is not clean outside this directory. It then:

1. builds the solution with warnings as errors;
2. publishes the harness trimmed and Native AOT;
3. in each of the three modes, runs the harness's catalog and then a whole run, with the retained corpus
   and verbose output;
4. compares the lines the modes printed;
5. applies, runs and reverts each negative control;
6. runs the solution's test projects;
7. checks that the tree is clean again.

Its header says what each file is. Every log hides the checkout's path as `<root>`. The manifest
command's remaining arguments are recorded in `manifest.json`.

## 5. Environment

The collection ran on:

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- clang 18 with lld, for the Native AOT link;
- the checkout at the commit above, with nothing outside this directory changed.

The JIT runs use the framework-dependent build output, and the trimmed runs a self-contained publish. The
Native AOT runs the published image, an ELF executable with no managed assembly beside it.
