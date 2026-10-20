<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-3-001, `revision-check/`: the workloads' answers and `to-string.js`'s texts, before part C was revised

**Seen before the revised rule was written; not read by its decision; no timing.** This directory is the check
that [`decision-rule-revised-2026-10-20.md`](../decision-rule-revised-2026-10-20.md) was written after, as the
first run's README checked every answer at `49ea00e` before its measurement ([`../README.md`](../README.md)
section 7, "Read before it ran"). It asked whether a revision of part C could be written at all: whether, at this
family's later sources, every workload but `to-string.js` still answers as at the base, whether both arms run under
one effective configuration, and whether every text `to-string.js` converts at arm A is one the specification
admits. Nothing here is evidence for SJL-3's clause 13. The revised rule's own run, `../measurement-revised/`, takes
every one of these checks again, before its lanes, and decides alone.

**The gate holds**, item by item, in [`gate.txt`](gate.txt). The revised rule was written after it.

## What was run

- **The command**, from a checkout of this branch before the rule existed:

  ```
  python3 docs/evidence/sjl-3-001/revision-check/revision-check.py --tree <a clean checkout of 95a0b81> \
      --base-tree <a clean checkout of 0087e8d73ae3833200fa263b68f71db15b059a9c> \
      --numref <Broiler.JS scripts/conformance/numref.py at 966140ad694570467ad21975f23047d0d6f0c1ce> \
      --out docs/evidence/sjl-3-001/revision-check
  ```

  [`revision-check.py`](revision-check.py) is retained beside its output; it takes part C's constants (the root,
  the allowances, the native backend, the workloads) from bundle `sjl-0-001`'s collector, as part C's own step does.
- **Arm B** is bundle `sjl-0-001`'s base commit, `0087e8d73ae3833200fa263b68f71db15b059a9c`, a clean checkout.
- **Arm A's sources** are `95a0b816a9c8030873c6a71e3ac320a7c90d6f8c`, the default branch on 2026-10-20, a clean
  checkout. Its product sources are `aaa39fbc2030e1a2554317bea75bcbb56041c696`'s but for one file,
  `eng/check-frontend-mapping.py`, SJL-12.3's mapping checker: a script the command-line root does not build
  ([`identity.txt`](identity.txt)).
- **Builds.** The command-line composition root, in Release, in each tree, with node reuse and the shared compiler
  off. Both built with no warning. The main assembly's hash differs from the first run's arm B
  ([`../measurement/manifest.txt`](../measurement/manifest.txt)) because each build embeds the path of its own
  checkout; part C names each run's binaries in its own manifest and never compares them across runs.
- **Answers.** Each of the six files of `src/tests/forms/numbers`, read from arm A's tree (the base commit predates
  them), once per arm and form, with `--runtime`, part C's allowances (fuel 100,000,000,000, wall 600,000 ms) and the
  native backend `x86-64-sysv`. The workloads are byte for byte those the bundle's `manifest.json` names.
- **Transcripts.** [`../transcript/to-string.transcript.js`](../transcript/to-string.transcript.js), once per arm
  and form, under the same options, held to [`../transcript/check.py`](../transcript/check.py) with Broiler.JS's
  `scripts/conformance/numref.py` at `966140ad694570467ad21975f23047d0d6f0c1ce` (SHA-256
  `424d557fd6ec5292b80104178f26256a7f98d629ba8f454467b0c901275f56e7`). The checker refused to start until the
  workload's loop and the program's both read as its own: the same 25 values, the bound 400, the scale 1024 and the
  eight conversions in the same order.

## What it showed

**Answers** ([`answers.tsv`](answers.tsv), beside bundle `sjl-0-001`'s base reading,
[`../../sjl-0-001/measurement/conditions.tsv`](../../sjl-0-001/measurement/conditions.tsv)):

| File | Base reading | Arm B, both forms | Arm A, both forms |
|---|---|---|---|
| `to-string.js` | 2515784 | 2515784 | **2516095** |
| `to-string.control.js` | 80000 | 80000 | 80000 |
| `to-number.js` | 85000 | 85000 | 85000 |
| `to-number.control.js` | 100000 | 100000 | 100000 |
| `literals.js` | 68000 | 68000 | 68000 |
| `literals.control.js` | 68000 | 68000 | 68000 |

Every run exited 0. Arm A's `to-string.js` answers as the first run's arm A did at `49ea00e`, in both forms; every
other file answers as at the base. So only `to-string.js` needs a condition of its own, and the changes since
`49ea00e` moved none of the other answers.

**Configuration** ([`configuration.txt`](configuration.txt)): one effective-configuration line in every run of both
arms, the first run's, character for character: `linux-x64`, X64, workstation GC, interactive latency, tiering and
PGO unset, dynamic code on, native arming installed.

**The transcript check** ([`transcript-check.txt`](transcript-check.txt)) **holds in both forms.**
- Each arm's transcript has 80,000 lines, as many as `to-string.control.js` answers at arm B; the lines' lengths sum
  to each arm's own `to-string.js` answer (2516095 at A, 2515784 at B). Each arm's transcript is byte for byte the
  same in both forms.
- Every one of arm A's 80,000 texts is one the specification admits for its value and conversion.
- The arms differ only at `toString(7)` (2,922 lines: arm B's text is outside the admitted set on 2,701 of them and
  inside it on 221) and `toString(36)` (3,003 lines: 2,175 and 828). `String`, `toString` at radixes 2 and 16,
  `toFixed`, `toExponential` and `toPrecision` agree on every line. Both are radix conversions, whose digits
  JSD-0066 section 4 says the cut-over corrects.

**Not retained:** the build logs, the binaries, the four transcripts and the per-line listing of the arms'
differences. The command writes each again. The revised rule's run retains its own transcripts and listing.

## Files

| File | What it holds |
|---|---|
| [`revision-check.py`](revision-check.py) | the check's program |
| [`identity.txt`](identity.txt) | both arms' commits and main assemblies, the build and child command lines, the workloads by digest and blob, the program, the checker and the reference by digest, the SDK and the machine |
| [`answers.tsv`](answers.tsv) | arm, form, file, exit and answer, beside the base reading |
| [`configuration.txt`](configuration.txt) | each arm's effective-configuration lines |
| [`transcript-check.txt`](transcript-check.txt) | each transcript's digest, and the check's items (i) to (iii) per form, with counts per conversion |
| [`gate.txt`](gate.txt) | the gate's three items and its result |
