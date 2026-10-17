<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# sjl-7-001, `measurement/`: part C, this family's measurement of the Unicode area

**One evidence class: a measurement.** It is part C of [`decision-rule-area.md`](../decision-rule-area.md),
taken on the local machine the rule's header names, by `collect-area.py --only measure` at SJL-7.6. The bundle's
`area-manifest.json` does not list this directory; [`manifest.txt`](manifest.txt) and [`hashes.txt`](hashes.txt)
are its own.

**Part C is NOT MET, on clause 3.** Clauses 1 and 2 hold. Clause 3 holds for four of the six workloads and forms,
and fails for casing and for identifier scanning in the native form. By the rule's reading, JSD-0031's fifth
reason, the owner's runtime cost, **stays open for this engine** for those two, in the native form.

## What was run

- **Arms.** B, the base: the command-line composition root built in Release from `7b44dc7`, the commit that
  carries the rule. A, the after commit `be0b610`, which lands SJL-7.4's cut-over on the packages as published.
  A-again: A's binaries, measured a second time under another lane name, the A/A lane. Each arm's binaries are
  named by hash in [`manifest.txt`](manifest.txt) (`binaries-b.sha256`, `binaries-a.sha256`), which was written
  and made read-only before any lane ran.
- **Workloads.** The three of `src/tests/forms/unicode` as the rule commits them, `normalize.js`, `casing.js`
  and `identifiers.js`, each with its control, in the bytecode form and in the native form under `x86-64-sysv`.
- **Protocol.** In each repetition the arms ran in an order that rotated by repetition, each running the workload
  and then its control. Three warm-up repetitions, retained in [`runs.tsv`](runs.tsv) and named, are excluded from
  Q; seven are retained per lane (docs/baselines.md rule 4). Every timed child ran with `--runtime`, with fuel
  100,000,000,000 and a wall allowance of 600,000 ms, the same for every lane. Every arm's answer was checked
  before and after its repetitions ([`conditions.tsv`](conditions.tsv)).
- **When, and beside what.** 2026-10-17, the lanes from 00:54:00Z to 00:58:31Z, after both arms' builds. No other
  build, test or run of this programme was running: the Broiler.JS collector beside it was asked to hold its
  builds until the step ended, and the machine's load average was 0.47 when the step started and 1.19, its own
  one timed process, when it ended. The machine is the base's: `linux-x64`, four logical processors, .NET SDK
  10.0.401.

## The reading

| Clause | On this run |
|---|---|
| 1. Every lane's condition passes, before and after, for every workload and form | **Holds.** No answer check differs from arm B's, in 72 checks, and no timed child's answer differs |
| 2. Every effective-configuration line identical across arms, lanes and repetitions | **Holds.** One line in every timed child and every check: `linux-x64`, X64, workstation GC, interactive latency, tiering and PGO unset, dynamic code on, native arming installed |
| 3. For every workload and form, D <= N | **Fails for two of six**: native casing and native identifiers |

[`summary.txt`](summary.txt), in milliseconds of wall time per process, the median of the seven retained
repetitions:

| Form | Workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N |
|---|---|---|---|---|---|---|---|
| bytecode | normalize | 515.225 | 514.619 | 498.451 | 16.169 | -0.606 | yes |
| bytecode | casing | 445.425 | 441.070 | 472.356 | 31.287 | -4.356 | yes |
| bytecode | identifiers | 805.120 | 791.853 | 804.841 | 12.988 | -13.266 | yes |
| native | normalize | 649.048 | 636.600 | 643.022 | 6.422 | -12.448 | yes |
| native | casing | 544.277 | 599.409 | 573.848 | 25.561 | **+55.132** | **no** |
| native | identifiers | 1,018.929 | 1,058.440 | 1,043.625 | 14.815 | **+39.511** | **no** |

**What the two failures are, as a reading that decides nothing.** The rule has no tolerance but the A/A lane, and
it allows no re-run for a clause that fails, so the figures stand as they are.
- **Native casing**: arm A's seven retained runs spread from 521 to 650 ms, against arm A-again's 565 to 585 and
  arm B's 497 to 571. Arm A-again's median is also above arm B's, by 29.6 ms. That is more than the floor of
  25.6 ms, though the rule does not judge A-again against B.
- **Native identifiers**: arm A's runs spread from 979 to 1,357 ms, the widest of any lane, against A-again's
  946 to 1,072 and B's 962 to 1,104. A-again's median is above B's by 24.7 ms, which is also more than the
  floor of 14.8 ms.
- **The controls moved little**: native casing's control has medians of 424.1, 422.5 and 441.5 ms in B, A and
  A-again, and native identifiers' 996.5, 1,005.3 and 1,023.9. The attribution E(A) − E(B), retained beside D,
  is +56.8 ms for casing and +30.8 ms for identifiers. It decides nothing.
- **Each figure is a whole process**: start, the JIT, and the workload. Each workload is a few hundred
  milliseconds above its control in the native form. The difference this run reports is a few per cent of that
  process.

**What this does not show.** It is not a claim that the cut-over made casing or identifier scanning slower, and
not a claim that it did not. One run on this machine finds the after arm slower than the base by more than the
A/A floor for two workloads in the native form, and the rule reads that as NOT MET. A second run would be a new
measurement under a dated revision of the rule, which is weaker evidence. The owner decides whether to take
one, or to leave JSD-0031's fifth reason open for those two in the native form.

## Files

| File | What it is |
|---|---|
| [`manifest.txt`](manifest.txt) | the rule, the collector and the arms by digest, the protocol and the machine; written and made read-only before the first lane |
| [`binaries-b.sha256`](binaries-b.sha256), [`binaries-a.sha256`](binaries-a.sha256) | every file of each arm's build output |
| [`build-cli-b.log`](build-cli-b.log), [`build-cli-a.log`](build-cli-a.log) | each arm's build |
| [`runs.tsv`](runs.tsv) | every timed child, 360 of them, warm-up repetitions named |
| [`conditions.tsv`](conditions.tsv) | every answer check, 72 of them |
| [`summary.txt`](summary.txt) | Q, N, D and E per workload and form, and every effective-configuration line |
| [`hashes.txt`](hashes.txt) | the SHA-256 of every file above; this README was written after the step |
