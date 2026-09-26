<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-2-001

**Milestone:** UBC-2 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
`Broiler.VM.Emitter.Bytecode` and the fixture family.
**Collected:** 2026-09-25, at commit `912c476`, from a clean tree, by `collect.py` in this directory.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 1.
**Status of the milestone after this collection:** `In progress`, every exit-gate clause met on the
evidence below, and unaccepted: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while
review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-2-001 |
| Milestone | UBC-2, exit gate clauses 1 to 9 |
| Commits of the milestone | `02e4e65` the emitter, the fixture family and the fixture composition; `80645f1`, `151a590`, `3e69488`, `9fedd35`, `0a03433` and `912c476` the corrections two adversarial reviews and their checks found |
| Composition | `Broiler.VM.Composition.Ubc.Fixture`, the register row of [`docs/compositions.md`](../../compositions.md) |
| RID | `win-x64`, which is not a supported RID |
| Owner | the core architecture owner with the security owner |
| Reviewer | none |
| Evidence class | publish-and-run logs, retained corpora replayed, disassemblies and rule runs; no measurement of performance of any kind |

## 2. What this bundle demonstrates, clause by clause

| Clause | What it asks | Where it is shown |
|---|---|---|
| 1 | The root publishes and runs under JIT, trimming and Native AOT, and its closure read off the published output holds exactly its register row's assemblies with `Broiler.VM.Ubc` in the sibling cell (K1 to K5) | `publish.log` (both publishes exit 0), `run-jit.log`, `run-trimmed.log`, `run-aot.log`; `catalog-fixture.txt` and `catalog-modes.log` (the three modes print one table); `closure-fixture.txt` (the trimmed publish's managed assemblies; the Native AOT image is one native file and carries none); `architecture-tests.log`, rules K1 to K5 in detail, run after the catalog and closure were written |
| 2 | E1, E3, E4 and E6 over the program corpus and the twins on all three modes; E2's primitive input corpus retained with the handler's answers | E1: the `program-corpus` check in each run log, every program of the fixed list, among them `common-tour` and `common-edges` which between them execute every common row and each meaning Appendix A gives it; and `corpus-*.log`, the retained corpus replayed per mode. E3: the replay checks every retained artifact against the lowering's bytes, and the bytecode form emits nothing, so one program has one emission by construction. E4: the bytecode emitter answers with the whole verified artifact or with the verifier's refusal, never per unit; `form-not-composed` shows the one refusal its form layer has, by name. E6: the `fuel-parity` check, one verdict at every fuel ceiling tried, and `frame-fuel`. E2: `src/tests/corpus/ubc-2/primitives.txt`, hashed in the corpus manifest, replayed per mode |
| 3 | The descriptor's rows 1 to 3 and 8 to 30 are the declaration's and 4 to 7 the universal bytecode's, and the catalog admits it; another built-against contract version is refused at catalog construction | `descriptor-rows` and `contract-version-refused` in each run log |
| 4 | Guest recursion refused naming `CallDepth` on every mode; a suspension restores both planes; cancellation lands within the declared bound | `recursion`, `suspend` and `suspend-in-callee` in `program-corpus`; `cancellation-within-bound`, which measures the fuel charged after the operation's token was cancelled against the declared bound and is watched failing over a loop whose polls are swallowed; `parked-holds-no-depth` |
| 5 | The UBC-1 corpus replays to its recorded answers across the three modes with byte-identical failure-class tables | `ubc1-corpus-jit.log`, `ubc1-corpus-trimmed.log`, `ubc1-corpus-aot.log`, and `ubc1-corpus-modes.log` |
| 6 | The two-profile hostile-neighbour test exists in the root and passes | `hostile-neighbour` in each run log: the ledger's maximum does not reach the family, and its default, adopted, refuses a call its maximum admits |
| 7 | Route UBC-R7's fold check: the JIT half asserted, the Native AOT half asserted or excluded by name | `disasm-jit.txt` and `disasm-aot.txt` retain `Run`, `Start`, `Resume`, `Unwind` and `Suspend` of the fixture instantiation; `fold-check.log` asserts both halves, naming the family members each method calls directly |
| 8 | A11, A12 and A13 revised with witnesses watched in both directions; A7 and A15 agree with the tree | `architecture-tests.log`, and the witnesses named in the rule register |
| 9 | The ledger's UBC-2 row names each unmet clause while any is unmet | The programme ledger, committed with this bundle |

The other UBC-2.7 contract checks are in the same run logs: every executor step kind read from the
executor itself (`every-step-kind`), and a guest-loaded artifact of another profile refused as a
provider breach (`guest-foreign` in the program corpus).

## 3. What was run

```text
dotnet build Broiler.VM.slnx -c Release
python docs/evidence/ubc-2-001/collect.py --vcvars "C:\Program Files\Microsoft Visual Studio\18\Professional\VC\Auxiliary\Build\vcvars64.bat"
python eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-2-001 --milestone UBC-2 --evidence-class publish-and-run ...
```

`collect.py` publishes trimmed and Native AOT, runs the root's checks, its `--closure`, `--corpus` and
`--ubc1-corpus` modes in each of the three modes, extracts the disassemblies and asserts the fold, and
then runs the architecture suite. Its header says what each file is. The manifest command's full
argument list is in `manifest.json`'s `inputs`.

## 4. Environment

Windows 11 Enterprise, the .NET SDK `10.0.401`, Python `3.11.9`, the Professional installation's
`vcvars64.bat` for the Native AOT link and for `dumpbin`, the checkout at the commit above with nothing
outside this directory changed. The JIT run is the framework-dependent build output; the trimmed run
is a self-contained publish; the Native AOT run is the published image.

## 5. The departures from the concept this milestone is responsible for

[Appendix G of the concept](../../universal-bytecode.md) records where the code departs from the text,
and each bundle repeats the list it is responsible for. This one's rows are: the family call request
and its fit (5.9); the value plane reached through `IUbcValuePlane` (7.2); the operand as a `ulong`
(7.2); the forms the emitter set composes (6.3 and 7.1); the fixture family's rows (8.3); the version
refusal at catalog construction (13); the frame fuel and the unwinding charges beside Appendix A's one
unit a row; and what a suspended operation holds. The rest of the list is UBC-1's.

## 6. What is NOT here, named rather than left as an absence

- **One RID, one machine, one lane.** `win-x64` is not a supported RID, and nothing here was run
  anywhere else.
- **No emitted code.** The bytecode emitter emits nothing, so E3 and E4 hold by construction rather
  than by a comparison of two emissions, and E5, E7 and E8 have no subject. E2's differential check
  itself first has an emitter to run against at UBC-6a, which cannot meet its gate while ADR 0013's
  note on candidate B stands; only its corpus is retained here.
- **No language.** The fixture family proves the contract and is shaped to fit it; the roadmap's
  caveat about fixture agreement applies, and nothing here is evidence about a language family.
- **The fold check reads disassembler output by pattern.** It asserts the absence of the helpers and
  names a type test or a family-contract dispatch takes in each listing, and names the direct calls
  it finds; it is not a proof over the compiled code, and a member inlined into its caller has no
  call to find.
- **Reservations are estimates.** The reader's and the walk's reservations of decoded rows are stated
  constants, not measurements; a check of the walk's share for jump tables is in the contract tests,
  and the reader's per-row estimate is below what it allocates for some row kinds, within a small
  constant factor.
- **No review.** Every record and line of code here is unreviewed, and the author and the owner are
  one person.

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
