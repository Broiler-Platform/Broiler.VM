<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-7-001, `measurement-revised/`: part C again, under the rule revised after its first run

**One evidence class: a measurement.** It is part C of
[`decision-rule-area-revised-2026-10-17.md`](../decision-rule-area-revised-2026-10-17.md). That rule was written
after part C's first run, [`../measurement/`](../measurement/README.md), had been seen and read NOT MET, so its
decision is **weaker evidence** than one under the rule as written. It moves arm A to this family on Broiler.Unicode
`0.1.0-preview.5` and changes nothing else. `collect-area.py --only measure-revised` took it on the local machine the
original rule's header names. The bundle's `area-manifest.json` does not list this directory;
[`manifest.txt`](manifest.txt) and [`hashes.txt`](hashes.txt) are its own.

**Part C, as revised, is MET.** All three clauses hold. For every workload and form the after arm's median is
below the base's, so D is negative in all six. Under the revised rule's reading, **JSD-0031's fifth reason, the
owner's runtime cost, is answered for this engine on `0.1.0-preview.5`**, as weaker evidence. The first run's NOT
MET stands as the reading of arm A at `be0b610`, on `0.1.0-preview.4`.

## What was run

- **Arms.**
  - B, the base: the command-line composition root built in Release from `7b44dc7`, the commit that carries the
    original rule, as in the first run. Its binaries' listing, [`binaries-b.sha256`](binaries-b.sha256), names the
    same files as the first run's, 16 of them with other digests, the main assembly among them, because the build was
    taken again in another directory.
  - A: built from `6cc39c8`, the commit that carries the revised rule. That commit descends from `9508bd5` and
    differs from it in no file outside the documents.
  - A-again: A's binaries, measured a second time under another lane name, the A/A lane.

  [`manifest.txt`](manifest.txt) names each arm's binaries by hash, and every commit since the first run's after
  commit `be0b610` with its files outside the documents. Two merges carry product changes: #137, the
  `Intl.Locale` and Temporal casing defects, and #138, the family on `0.1.0-preview.5`. The manifest was written
  and made read-only before any lane ran.
- **Workloads.** The three of `src/tests/forms/unicode` as the original rule commits them, `normalize.js`,
  `casing.js` and `identifiers.js`, each with its control, in the bytecode form and in the native form under
  `x86-64-sysv`. The collector checked every file byte for byte the same in both arms' trees, and the manifest
  names each by digest and blob.
- **Protocol.** The first run's, unchanged:
  - in each repetition the arms ran in an order that rotated by repetition, each running the workload and then its
    control;
  - three warm-up repetitions, retained in [`runs.tsv`](runs.tsv) and named, are excluded from Q, and seven are
    retained per lane;
  - every timed child ran with `--runtime`, fuel 100,000,000,000 and a wall allowance of 600,000 ms;
  - every arm's answer was checked before and after its repetitions ([`conditions.tsv`](conditions.tsv)).
- **When, and beside what.** On 2026-10-17 the step started at 12:54:19Z, and its lanes ran from 12:55:28Z to
  13:00:43Z after both arms' builds.
  - The step started about half a minute after Broiler.Unicode's library run for its bundle's third reading
    ended. The load average was 2.99 at the start, still decaying from that run, and 1.21 at the end.
  - Nothing else of this programme ran beside the step. Its builds ran with node reuse, the MSBuild server and the
    shared compiler off, so no build process outlived them.
  - The machine is the first run's kind, `linux-x64` with four logical processors and .NET SDK 10.0.401. It was
    a fresh container, restarted since the first run, so **absolute figures here are not comparable with the first
    run's**: arm B's medians are 11 to 27 per cent above that run's. The rule compares arms within one run only.

## The reading

| Clause | On this run |
|---|---|
| 1. Every lane's condition passes, before and after, for every workload and form | **Holds.** No answer check differs from arm B's, in 72 checks, and no timed child's answer differs, in 360 |
| 2. Every effective-configuration line identical across arms, lanes and repetitions | **Holds.** One line in every timed child and every check: `linux-x64`, X64, workstation GC, interactive latency, tiering and PGO unset, dynamic code on, native arming installed |
| 3. For every workload and form, D <= N | **Holds for six of six** |

[`summary.txt`](summary.txt), in milliseconds of wall time per process, the median of the seven retained
repetitions:

| Form | Workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N |
|---|---|---|---|---|---|---|---|
| bytecode | normalize | 623.358 | 595.870 | 554.975 | 40.894 | -27.488 | yes |
| bytecode | casing | 495.772 | 491.167 | 480.832 | 10.335 | -4.604 | yes |
| bytecode | identifiers | 970.704 | 935.489 | 929.732 | 5.757 | -35.216 | yes |
| native | normalize | 732.163 | 690.478 | 692.476 | 1.998 | -41.685 | yes |
| native | casing | 688.902 | 598.935 | 626.604 | 27.669 | -89.967 | yes |
| native | identifiers | 1,259.123 | 1,250.671 | 1,213.009 | 37.663 | -8.451 | yes |

**What the figures show, as a reading that decides nothing.**
- **The two workloads that failed the first run.**
  - **Native casing**: arm A's seven runs spread from 590 to 644 ms and A-again's from 607 to 671, both below
    arm B's 659 to 719. The after arm is faster than the base by more than the A/A floor.
  - **Native identifiers**: D is −8.5 ms against a floor of 37.7. The arms' spreads overlap widely: A 1,159 to
    1,282 ms, A-again 1,104 to 1,391, B 1,148 to 1,530. This one is below resolution, not shown faster.
- **The other four**: two have D beyond the floor in the after arm's favour, bytecode identifiers and native
  normalization. Bytecode normalization and bytecode casing are below resolution.
- **The attribution E**, the workload less its control, is retained beside D in `summary.txt` and decides nothing.
  It is below the base's in A for five of six. For native identifiers, the controls moved more than the workloads
  did between arms, and E(A) is above E(B).
- **Each figure is a whole process**: start, the JIT, and the workload. Each workload is at most about two hundred
  milliseconds above its control, so the differences here are a few per cent of each process.

**What this does not show.**
- It is not a speed claim. The rule states none, and it holds the after arm to the base within this machine's
  noise.
- It does not say that the first run was wrong. That run measured arm A on `0.1.0-preview.4`, and this one
  measures `0.1.0-preview.5`.
- It is one run under a rule revised after the first was seen. The revised rule makes this the last measurement
  of part C.

## Files

| File | What it is |
|---|---|
| [`manifest.txt`](manifest.txt) | the revised rule and the rule it revises, the collector and the arms by digest, every commit since the first run's after commit, the protocol and the machine; written and made read-only before the first lane |
| [`binaries-b.sha256`](binaries-b.sha256), [`binaries-a.sha256`](binaries-a.sha256) | every file of each arm's build output |
| [`build-cli-b.log`](build-cli-b.log), [`build-cli-a.log`](build-cli-a.log) | each arm's build |
| [`runs.tsv`](runs.tsv) | every timed child, 360 of them, warm-up repetitions named |
| [`conditions.tsv`](conditions.tsv) | every answer check, 72 of them |
| [`summary.txt`](summary.txt) | Q, N, D and E per workload and form, and every effective-configuration line |
| [`hashes.txt`](hashes.txt) | the SHA-256 of every file above; this README was written after the step |
