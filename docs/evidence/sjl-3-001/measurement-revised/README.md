<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-3-001, `measurement-revised/`: part C again, under the rule revised after its first run

**One evidence class: a measurement.** It is part C of
[`decision-rule-revised-2026-10-20.md`](../decision-rule-revised-2026-10-20.md). That rule was written after part
C's first run, [`../measurement/`](../measurement/summary.txt), had been seen and read NOT MET, so its decision is
**weaker evidence** than one under the rule as written, [`decision-rule.md`](../decision-rule.md). It moves arm A to
this family at `aaa39fb`'s product sources, gives `to-string.js` a condition of its own, held to the specification by
a transcript, and makes this the second and last measurement of part C. `collect.py --only measure-revised` took it,
once, on the local machine part C's first run was taken on. The bundle's `manifest.json` does not hash this
directory; [`manifest.txt`](manifest.txt) and [`hashes.txt`](hashes.txt) are its own. This README is in neither: it
was written after the step, as bundle `sjl-7-001`'s
[`measurement-revised/README.md`](../../sjl-7-001/measurement-revised/README.md) is not in its `hashes.txt`. The first
run's directory, `../measurement/`, has no README; the bundle's [`README.md`](../README.md) reads it in section 7.

**Part C, as revised, is NOT MET, on its clause 3 alone.** Every answer check holds, the transcript check holds in
both forms, and every effective-configuration line is the same, so clauses 1 and 2 hold. D <= N holds for four of the
six workloads and forms, and not for `to-number.js` in the bytecode form or `literals.js` in the native form. The
revised rule says what this does not show: "A NOT MET is therefore not a finding of a slowdown, and a MET is not a
speed claim." It is the second and last measurement of part C, so clause 13 stays open. The first run's NOT MET
stands as the reading of arm A at `49ea00e`.

## What was run

- **Arms.**
  - B, the base: the command-line composition root built in Release from a clean checkout of
    `0087e8d73ae3833200fa263b68f71db15b059a9c`, bundle `sjl-0-001`'s base commit, as in the first run.
  - A: built from a clean checkout of `ebf93cd3e8fba11002f3da563b13b32ca54afe32`, the merge of
    Broiler-Platform/Broiler.VM#160, which lands the revised rule on the default branch. Its first parent, `865b69b`,
    does not carry the rule; its second, `7e887a1`, is the rule's branch. Outside the documents it differs from
    `aaa39fb` only under the four paths the rule's header names (`src/tests/corpus/parse-1/`, the slice compiler's
    root, `eng/compare-parse-corpus.py` and `eng/check-frontend-mapping.py`), which SJL-12's #158 and #159 changed and
    which neither the root's closure nor a build file names. The manifest lists every one of those files.
  - A-again: A's binaries, measured a second time under another lane name, the A/A lane.

  [`manifest.txt`](manifest.txt) names each arm's main assembly by hash, and
  [`binaries-b.sha256`](binaries-b.sha256) and [`binaries-a.sha256`](binaries-a.sha256) every file of each arm's
  build output. It also names the revised rule, the rule it revises, the collector, the transcript checker, the
  transcript program and the reference by digest; the root's closure; and every commit since the first run's arm A,
  `49ea00e`, with its files outside the documents. It was written and made read-only after both builds, before any
  answer check, transcript or lane.
- **Workloads.** The three of `src/tests/forms/numbers` as the original rule commits them, `to-string.js`,
  `to-number.js` and `literals.js`, each with its control, in the bytecode form and in the native form under
  `x86-64-sysv`. The collector checked each file byte for byte against the blob the bundle's `manifest.json` names,
  and the manifest names each by digest and blob.
- **The transcript check**, taken before any lane. Each arm's binary ran
  [`../transcript/to-string.transcript.js`](../transcript/to-string.transcript.js) once in each form; the four
  transcripts are retained, `transcript-<arm>-<form>.txt.gz`. [`../transcript/check.py`](../transcript/check.py) held
  them to the rule's items (i) to (iii), arm A's texts against Broiler.JS's `scripts/conformance/numref.py` at
  `966140ad694570467ad21975f23047d0d6f0c1ce` (SHA-256
  `424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7`), into
  [`transcript-check.txt`](transcript-check.txt) and [`transcript-check.tsv`](transcript-check.tsv).
- **Protocol.** The first run's, but for when the first checks are taken (the revised rule's item 2):
  - every arm's first check of every file, in each form, was taken before any lane, and every answer was checked
    again after its workload's lanes ([`conditions.tsv`](conditions.tsv));
  - the expected answer is arm B's first for every file but `to-string.js`, and for `to-string.js` the lane's own
    arm's first, arm A's for A-again;
  - in each repetition the arms ran in an order that rotated by repetition, each running the workload and then its
    control;
  - three warm-up repetitions, retained in [`runs.tsv`](runs.tsv) and named, are excluded from Q, and seven are
    retained per lane;
  - every timed child ran with `--runtime`, fuel 100,000,000,000 and a wall allowance of 600,000 ms.
- **When, and beside what.** On 2026-10-20 the step ran once, from 14:19:22Z to 14:28:55Z, and exited 0. Its manifest
  was written at 14:20:38Z, after both builds, and [`summary.txt`](summary.txt) dates the measurement from then to the
  step's end.
  - Nothing else ran on the machine beside it: no other build, test or run. The programme's previous local runs,
    SJL-12's parse-corpus baselines, had ended at 14:04Z. The step's builds ran with node reuse, the MSBuild server
    and the shared compiler off, so no build process outlived them.
  - The load average was 0.07 0.10 0.20 at the step's start, 4.67 1.49 0.67 when the manifest was written after both
    builds, and 0.85 1.07 0.82 at the end.
  - The machine is `linux-x64` with four logical processors and .NET SDK 10.0.401 ([`manifest.txt`](manifest.txt)).
    The rule compares arms within one run only, and this README compares no figure with the first run's but where it
    says so below.

## The reading

| Clause | On this run |
|---|---|
| 1. Every lane's condition passes, before and after, for every workload and form | **Holds.** No answer check differs from the answer it is held to, in 72 checks, and no timed child's answer differs, in 360. `to-string.js` answers 2516095 at A and A-again and 2515784 at B, in both forms, each arm's own first answer; every other file answers as at bundle `sjl-0-001`'s base reading. The transcript check holds in both forms |
| 2. Every effective-configuration line identical across arms, lanes and repetitions | **Holds.** One line in every timed child, every check and every transcript: `linux-x64`, X64, workstation GC, interactive latency, tiering and PGO unset, dynamic code on, native arming installed |
| 3. For every workload and form, D <= N | **Fails for two of six**: `to-number.js` in the bytecode form, D = 29.061 ms against N = 6.490 ms, and `literals.js` in the native form, D = 66.553 ms against N = 38.373 ms. It holds for the other four |

**The transcript check holds in both forms** ([`transcript-check.txt`](transcript-check.txt)):
- (i) each arm's transcript has 80000 lines, as many as `to-string.control.js` answers at arm B's first check;
- (ii) the lengths of arm A's lines sum to 2516095 and arm B's to 2515784, each that arm's `to-string.js` answer at
  its first check;
- (iii) every one of arm A's 80000 texts is one the specification admits for its value and conversion.

The arms' transcripts differ only at `toString(7)`, on 2922 lines (arm B's text is outside the admitted set on 2701 of
them and inside it on 221), and at `toString(36)`, on 3003 lines (2175 and 828), in each form.
[`transcript-check.tsv`](transcript-check.tsv) lists every one. `String`, `toString` at radixes 2 and 16, `toFixed`,
`toExponential` and `toPrecision` agree on every line. Each arm's transcript has the same digest in both forms. This
is what the check before the revision found at `95a0b81` ([`../revision-check/`](../revision-check/README.md)).

[`summary.txt`](summary.txt), in milliseconds of wall time per process, the median of the seven retained
repetitions:

| Form | Workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N |
|---|---|---|---|---|---|---|---|
| bytecode | to-string | 866.366 | 867.648 | 934.782 | 67.134 | 1.282 | yes |
| bytecode | to-number | 856.157 | 885.219 | 891.709 | 6.490 | **29.061** | **no** |
| bytecode | literals | 1467.179 | 1461.548 | 1476.509 | 14.961 | -5.631 | yes |
| native | to-string | 1022.370 | 1024.105 | 994.129 | 29.976 | 1.736 | yes |
| native | to-number | 987.439 | 972.604 | 956.562 | 16.042 | -14.834 | yes |
| native | literals | 1749.586 | 1816.139 | 1777.766 | 38.373 | **66.553** | **no** |

The attribution, retained beside D and deciding nothing: each control's median, and E, the workload's median less its
control's, per arm.

| Form | Workload | Q(control, B) | Q(control, A) | Q(control, A-again) | E(B) | E(A) | E(A-again) |
|---|---|---|---|---|---|---|---|
| bytecode | to-string | 507.421 | 569.027 | 546.618 | 358.945 | 298.621 | 388.164 |
| bytecode | to-number | 657.405 | 633.354 | 645.913 | 198.752 | 251.864 | 245.795 |
| bytecode | literals | 1176.252 | 1242.049 | 1205.707 | 290.926 | 219.499 | 270.802 |
| native | to-string | 653.271 | 654.699 | 669.997 | 369.098 | 369.406 | 324.132 |
| native | to-number | 726.024 | 761.435 | 735.033 | 261.415 | 211.169 | 221.529 |
| native | literals | 1443.541 | 1521.230 | 1442.632 | 306.045 | 294.909 | 335.134 |

**What the figures show, as a reading that decides nothing.**
- **Bytecode `to-number`**: D is 29.061 ms against a floor of 6.490. The floor is the distance between two medians of
  the same binaries, which here lay close together; the lanes' retained runs spread widely and overlap: A 790 to 946
  ms, A-again 839 to 1022, B 721 to 910 ([`runs.tsv`](runs.tsv)). Its control's median is lower at A than at B, and E
  is higher at A and A-again than at B. The first run, at `49ea00e`, failed clause 3 on the same workload and form,
  with a D of about the same size against a floor of 3.411 ms (the bundle's README, section 7). Neither rule reads
  either run as a finding of a slowdown, and nothing here decides whether there is one.
- **Native `literals`**: D is 66.553 ms against a floor of 38.373. Its control moved further than the workload did:
  the control's median is 77.689 ms higher at A than at B, and E(A) is below E(B). The spreads overlap widely: A 1633
  to 1912 ms, A-again 1717 to 1837, B 1688 to 1949. In the first run this workload and form had D well below zero.
- **The other four** are below resolution: |D| <= N for each, and none is beyond the floor in either direction.
- **Each figure is a whole process**: start, the JIT, and the workload. Each workload is at most a few hundred
  milliseconds above its control, so the differences here are a few per cent of each process.

**What this does not show.**
- **A slowdown, or a speed claim.** The revised rule records it in its header: under this tolerance, "a change that
  makes no difference at all reads NOT MET more often than MET: each workload and form must have D at most N, and N is
  only the distance between two medians of the same binaries. **A NOT MET is therefore not a finding of a slowdown,
  and a MET is not a speed claim.**"
- **Anything about SJL-3's cut-over alone.** Arm A is this family at `aaa39fb`'s product sources, with the changes
  of SJL-4 to SJL-8 and SJL-10 and the shared stream's re-pins from `0.1.0-preview.1` to `0.1.0-preview.7` since
  `49ea00e`. In the rule's words, "A NOT MET cannot be laid on SJL-3's cut-over alone either."
- **That the first run was wrong.** `../measurement/` stands as the reading of arm A at `49ea00e`, NOT MET on clauses
  1 and 3.
- **Any comparison with Broiler.JS**, or anything about another machine or RID.
- **A next measurement.** This is the last measurement of part C: "a NOT MET here is not followed by a third under a
  further revision, and leaves clause 13 open until the owner's next dated choice under decision SJL-D-23." No
  tolerance is changed.

## Files

| File | What it is |
|---|---|
| [`manifest.txt`](manifest.txt) | the revised rule and the rule it revises, the collector, the checker, the program and the reference by digest, each arm's commit and main assembly, the files under the four paths, every commit since the first run's arm A, the workloads, the protocol, the machine and the load average at the start; written and made read-only before any check, transcript or lane |
| [`binaries-b.sha256`](binaries-b.sha256), [`binaries-a.sha256`](binaries-a.sha256) | every file of each arm's build output |
| [`build-cli-b.log`](build-cli-b.log), [`build-cli-a.log`](build-cli-a.log) | each arm's build |
| `transcript-b-bytecode.txt.gz`, `transcript-b-native.txt.gz`, `transcript-a-bytecode.txt.gz`, `transcript-a-native.txt.gz` | each arm's transcript in each form, as the program printed it |
| [`transcript-check.txt`](transcript-check.txt) | each transcript's digest and configuration line, and the check's items (i) to (iii) per form, with counts per conversion |
| [`transcript-check.tsv`](transcript-check.tsv) | every line on which the arms' transcripts differ, with its form, value, conversion, both texts, and whether each is one the specification admits |
| [`runs.tsv`](runs.tsv) | every timed child, 360 of them, warm-up repetitions named |
| [`conditions.tsv`](conditions.tsv) | every answer check, 72 of them, before and after each workload's lanes, with the answer each is held to |
| [`summary.txt`](summary.txt) | Q, N, D and E per workload and form, every effective-configuration line, and the load average at the start and end |
| [`hashes.txt`](hashes.txt) | the SHA-256 of every file above; this README was written after the step and is not in it |
