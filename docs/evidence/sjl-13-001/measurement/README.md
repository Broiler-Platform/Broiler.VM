<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-13-001, `measurement/`: part J, this family's compile measurement at step 1

**One evidence class: a measurement.** It is part J of [`decision-rule-area.md`](../decision-rule-area.md), which none
of the rule's revisions changes, taken on the local machine the rule's header names, by the bundle's
[`collect-measurement.py`](../collect-measurement.py) at task 7.2 of step 1's evidence. The bundle's other folders do
not list this one; [`manifest.txt`](manifest.txt) and [`hashes.txt`](hashes.txt) are its own. **Nothing here is a
verdict**: part J is read under the rule by task 7.3, in the bundle's README, and it is not a gate clause of SJL-13
(JSD-0083 decision D24).

## What was run

- **Arms.** B, the base commit `df75f8cea79aa2452ca22892d47d8afa5a14c93f`'s slice compiler; A, the after commit
  `a8d8398bd59e7f5600cb0aa9e733ff8948859cf4`'s; A-again, A's binaries a second time under another lane name, the A/A
  lane. Each was built in Release from a clean `git archive` of its commit, under the JIT. Each arm's binaries are
  named by hash in [`manifest.txt`](manifest.txt) (`binaries-b.sha256`, `binaries-a.sha256`), which the collector
  wrote and made read-only before any lane ran.
- **Workloads.** The six of [`../inputs/workloads/`](../inputs/workloads/README.md), each with its control, as the
  rule commit `faa8af7` holds them; the base and the after commit hold the same tree, and the manifest names every
  file by its SHA-256.
- **Protocol.** In each repetition the arms ran in the order B, A, A-again rotated by the repetition, each one child
  of the slice compiler's `--measure-compile` mode with `--runtime`, which compiles every workload and then its
  control, the bytecode form, after an untimed pass over every file and with another after it. Three warm-up
  repetitions, retained and named, are excluded from Q; seven are retained per lane (`docs/baselines.md` rule 4).
- **When, and beside what.** See [`machine.tsv`](machine.tsv) (every child's start, end and load average before and
  after it) and `measurement-machine.txt` (the idle gate's record of every process on the machine when the step
  started). The step ran behind a gate that waits, without a time limit to fall back on, for a one-minute load of at
  most 1.0 and no other dotnet or python process.

## What it holds

| Record | What it holds |
|---|---|
| [`manifest.txt`](manifest.txt) | The arms, their binaries by hash, the workloads by hash, the SDK, the machine and the protocol |
| [`runs.tsv`](runs.tsv) | Every timed compilation: repetition, warm-up or retained, the arm's position, the arm, the file, the child's exit, whether its condition equals arm B's, its configuration line's number, and its compile time in milliseconds |
| [`conditions.tsv`](conditions.tsv) | Every child's condition lines, before its lane, timed and after it, each compared with the first B child's |
| [`machine.tsv`](machine.tsv) | Every child's times and the load average before and after it |
| [`lanes.txt.gz`](lanes.txt.gz) | Every child's whole output, in the order they ran |
| [`summary.txt`](summary.txt) | Q, N, D and E per workload, and the configuration lines seen; no verdict |

Task 7.3 reads part J's three clauses from these files.
