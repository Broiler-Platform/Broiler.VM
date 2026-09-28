<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record WA-MODES-001

**What this is:** a working record of the WebAssembly profile, not a milestone bundle. It retains the
profile's two composition roots published and run in the three modes several of its gates name:
- **JIT:** the framework-dependent build;
- **trimmed:** a trimmed, self-contained publish;
- **Native AOT.**

The publishes treat trim and AOT warnings as errors. The record also retains what each run printed,
compared line by line, and the closure read off each published output.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md)**,
and it demonstrates no gate clause whole. **No runtime identifier is claimed**: claiming one is a
release decision, and every gate that names publish modes names them "on every claimed runtime
identifier".

**Collected:** 2026-09-28, at commit `165057b`, from a clean tree, by `collect.py` in this directory. That
commit adds `collect.py` and changes no product, harness or test file. Its parent is `25b5603`.

**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Feature manifest:** `broiler.webassembly.slice`, the only one the family's table carries.

## 1. Identity

| Field | Value |
|---|---|
| Record | WA-MODES-001 |
| Milestones touched | WA-0 (the roots published and run in three modes, the closure read off the output); WA-2 (the corpus replayed in three modes, and a mutation detected by the Native AOT replay); WA-3 (the invalid half and the nesting entries replayed in three modes) |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness` and `Broiler.VM.Composition.WebAssembly.Execution`, both never advertised |
| RID | `linux-x64`, unclaimed |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |

## 2. Results

| Root | Mode | Publish | Run | Prints the JIT run's lines |
|---|---|---|---|---|
| harness | JIT | the solution's build, warnings as errors (`build.log`) | exit 0 (`harness-jit.log`) | - |
| harness | trimmed | exit 0, no warning (`publish.log`) | exit 0 (`harness-trimmed.log`) | yes |
| harness | Native AOT | exit 0, no warning | exit 0 (`harness-aot.log`) | yes |
| execution | JIT | the solution's build | exit 0 (`execution-jit.log`) | - |
| execution | trimmed | exit 0, no warning | exit 0 (`execution-trimmed.log`) | yes |
| execution | Native AOT | exit 0, no warning | exit 0 (`execution-aot.log`) | yes |

`modes.log` holds the comparison. The harness root's `--closure` report prints the same lines in all
three modes as well.

**What the harness root's run is.**
- It replays the retained corpus twice and prints one line per entry: the entry's name, its answer and
  its provenance. It then prints the replay's summary.
- It then runs its execution and differential checks.

So the three logs are the three tables the gates ask to be compared, and they compare equal line for
line. Every entry reproduces its recorded answer in every mode. That includes the corpus's invalid half
and the two nesting entries:
- `ceiling-nesting-at-the-structural-depth-ceiling` is accepted;
- `ceiling-nesting-one-level-beyond-the-structural-depth-ceiling` is answered
  `ResourceExhaustion/CeilingReached` naming `StructuralDepth` at artifact scope, under Native AOT as
  under JIT. The process is not terminated.

Each replay ends with its own summary line: the replay found no residue, and no entry ended the run.

**The closure, read off the output** (`closure.txt`):
- **Trimmed publishes.** Each holds exactly seven Broiler.VM assemblies: the three core assemblies,
  the profile, the universal bytecode, its bytecode emitter, and the root itself. That is the reference
  set each project file states.
- **Native AOT publishes.** Each holds one native executable and no managed assembly. The symbol and
  documentation files beside it are the build's, not the image's.

**The integrity check with the Native AOT image as the replay** (`corpus-integrity-aot.log`) passes all
three of its passes:
- the manifest's shape;
- every file against its digest, both ways;
- four entries flipped by one byte, each of which the Native AOT replay must fail on, and does.

`tests.log` is the solution's test projects, every test passing.

## 3. What this record does not demonstrate

- **A claimed runtime identifier.** One RID, unclaimed, on one machine. Every gate clause here says
  "every claimed runtime identifier", and no RID is claimed until a release decision is recorded.
- **WA-0's gate.** The publishes and the closure are the part of WA-0's action this shows. WA-0's gate
  also needs decisions this record does not take, and the licence obligation's owner.
- **WA-1's five verifier outcomes and five execution-step kinds from named cases.** The runs here
  include checks that produce several of each, but this record has not audited them against the list.
- **WA-2's other clauses.** The section-order table's pairs, the metering-split correction, and the
  specification pin are not touched here.
- **WA-3's other clauses.** The reachability rows rule W3 lists as unreached, and the fusion decision,
  are not touched here.
- **The specification's scripts in three modes.** Record [UBC-4-006](../ubc-4-006/README.md) ran the
  `--spec` lane trimmed and Native AOT at an earlier commit. This record does not repeat it.
- **Review.** Read by nobody but its author.

## 4. What was run

```text
python3 docs/evidence/wa-modes-001/collect.py
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/wa-modes-001 --milestone WA-0 --evidence-class working-record ...
```

## 5. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- clang 18 with lld, for the Native AOT link;
- Python 3.
