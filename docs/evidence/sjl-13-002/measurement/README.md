<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-13-002, `measurement/`: part F, this family's compile measurement at step 2

**One evidence class: a measurement.** It is part F of [`decision-rule-area.md`](../decision-rule-area.md), which the
rule's revision of 2026-11-13 does not change: bundle `sjl-13-001`'s part J's population, arms, configuration,
condition, quantity, noise floor, comparison and readings, with B this rule's base commit's build and A its after
commit's. It is collected by bundle `sjl-13-001`'s
[`collect-measurement.py`](../../sjl-13-001/collect-measurement.py), unchanged, as the after commit holds it, on the
local machine. The bundle's other folders do not list this one; [`manifest.txt`](manifest.txt) and
[`hashes.txt`](hashes.txt) are its own. **Nothing here is a verdict**: part F is read under the rule in the bundle's
README, and it is not a gate clause of SJL-13 (JSD-0083 decision D24).

**Written before any lane ran.** This file was written, and made read-only, before the collector started; the
collector writes `manifest.txt`, `binaries-b.sha256` and `binaries-a.sha256` and makes them read-only before its first
lane, as part J's arms line asks.

## What is run

- **Arms.** B, the base commit `389b55c9ec9bb77409bb2487067fbebc0868849b`'s slice compiler; A, the after commit
  `467ad7c395234b335c816b8f49ae5b505740e32c`'s; A-again, A's binaries a second time under another lane name, the A/A
  lane. Each is built in Release with `--disable-build-servers` from a clean `git archive` of its commit on a tmpfs,
  under the JIT: the builds part B's runs used. The manifest names each arm's binaries by hash.
- **Workloads.** The six of [`../../sjl-13-001/inputs/workloads/`](../../sjl-13-001/inputs/workloads/README.md), each
  with its control, as the after commit's archive holds them; `measurement-inputs.txt` names their tree at the base,
  the after and the rule commit, and the manifest names every file by its SHA-256.
- **Protocol.** The collector's: in each repetition the arms run in the order B, A, A-again rotated by the
  repetition, each one child of the slice compiler's `--measure-compile` mode with `--runtime`; three warm-up
  repetitions, retained and named, are excluded from Q, and seven are retained per lane.
- **The collector's own words.** The collector is step 1's, run unchanged. Its manifest's first lines name the
  sub-bundle as `sjl-13-001/measurement` and the decision as bundle `sjl-13-001`'s part J, and its `summary.txt`'s
  first line names part J, because that is the text the collector writes; here they are this rule's part F, which
  reads part J's procedure whole.
- **When, and beside what.** The collector runs behind a gate that waits, without a time limit to fall back on, for
  a one-minute load of at most 1.0 and no dotnet, testhost, VBCSCompiler, MSBuild, ilc, crossgen or Broiler.* process,
  last of this bundle's local runs. `measurement-machine.txt` is the gate's machine record at the start and the end;
  [`machine.tsv`](machine.tsv) holds every child's start, end and load average before and after it.

## What it holds

| Record | What it holds |
|---|---|
| [`manifest.txt`](manifest.txt) | The arms, their binaries by hash, the workloads by hash, the SDK, the machine and the protocol |
| [`runs.tsv`](runs.tsv) | Every timed compilation: repetition, warm-up or retained, the arm's position, the arm, the file, the child's exit, whether its condition equals arm B's, its configuration line's number, and its compile time in milliseconds |
| [`conditions.tsv`](conditions.tsv) | Every child's condition lines, before its lane, timed and after it, each compared with the first B child's |
| [`machine.tsv`](machine.tsv) | Every child's times and the load average before and after it |
| [`lanes.txt.gz`](lanes.txt.gz) | Every child's whole output, in the order they ran |
| [`summary.txt`](summary.txt) | Q, N, D and E per workload, and the configuration lines seen; no verdict |
| `measurement-inputs.txt`, `measurement-machine.txt`, `measurement-collector.log`, `measurement-collector.stderr.txt` | What the collector was given, the gate's machine records, and the collector's own output |
