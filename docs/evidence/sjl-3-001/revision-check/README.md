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

## The controls of the revised rule's step

**When they were taken.** First after the gate held and the revised rule, the collector's `measure-revised` step and
its `--check-only` option were written. Then again, on 2026-10-20, after a review of the rule commit changed the rule
and the step, and those are the controls kept here. The review's changes to the step:
- the comparison with `aaa39fb` now takes no rename, so a moved file differs at the path it left;
- an ignored file a build may read now refuses the run, and every ignored `bin/` and `obj/` directory is removed
  before each build;
- arm A must now be on the default branch's first-parent history, named by `--default-branch`.

The first taking's pass controls ran at the rule's own branch commit, which the step now refuses (control 12).

**What arm A was.** Arm A is the merge that lands the rule on the default branch. So every control starts from a
scratch merge, with `--no-ff`, of this branch at `24062d26195bfb775298ea057bfec49e03cdeaa7` into the default branch
at `865b69b9ede430d5a548e5b97b85e5a7b5e1aed4`, as the pull request's merge commit will be:
`b6f0dd81fb7a2d4c510fcdf161f53df66e31810a`. A control that changes a file amends that merge, keeping both its
parents. Each control names its own arm A as `--default-branch`, but for control 12. The commit that carries this
directory differs from `24062d2` only by `controls/` and this section. The rule, the collector, the checker and the
program are the same blobs, which each run's output names under "files".

**How they ran.** Every control ran from a scratch worktree with `--check-only`, so none could reach a transcript,
an answer or a lane. Each file in [`controls/`](controls/) names what was changed, what was expected, the command,
the step's output and its exit code. The run itself (task 1.4) is taken from a clean checkout of the merge, with
`--default-branch origin/main`.

**Every refusal fails before anything is built** (exit 2, every problem printed, and no refusal but the ones
expected):

| File | What was changed | What the step said |
|---|---|---|
| [`refuse-1-product-file.txt`](controls/refuse-1-product-file.txt) | one product file, a comment line in `src/Broiler.VM.Runtime/` | arm A differs from `aaa39fb` outside the documents in a path the rule does not exempt |
| [`refuse-2-workload-byte.txt`](controls/refuse-2-workload-byte.txt) | one byte of `to-number.js` | the workload's blob is not the one `manifest.json` names; the path is also a product path |
| [`refuse-3-numref-digest.txt`](controls/refuse-3-numref-digest.txt) | one byte of the reference | its SHA-256 is not the rule's |
| [`refuse-4-dirty-tree.txt`](controls/refuse-4-dirty-tree.txt) | an uncommitted line in a document of arm A's tree | arm A's tree is not clean |
| [`refuse-5-base-commit.txt`](controls/refuse-5-base-commit.txt) | arm B's tree at the base commit's parent | arm B is not at bundle `sjl-0-001`'s base commit |
| [`refuse-6-later-commit.txt`](controls/refuse-6-later-commit.txt) | a documents-only commit on top of the merge | arm A is the commit that adds the rule, not a later one |
| [`refuse-7-closure-reference.txt`](controls/refuse-7-closure-reference.txt) | the command-line root made to reference the slice compiler's root | the root's closure holds a project under an exempted path and names it, and the project file is a product path |
| [`refuse-8-build-server.txt`](controls/refuse-8-build-server.txt) | a process named `VBCSCompiler` running | a .NET build server or build is running |
| [`refuse-9-moved-under-exempt-path.txt`](controls/refuse-9-moved-under-exempt-path.txt) | `src/Broiler.VM.Profile.JavaScript/AssemblyMarker.cs` moved into the slice compiler's root, an exempted path | the path it left differs from `aaa39fb` and is not exempted |
| [`refuse-10-moved-into-docs.txt`](controls/refuse-10-moved-into-docs.txt) | `src/Broiler.VM.Runtime/VmNativePipeline.cs` moved into a `docs/` directory as a `.txt` file | the path it left differs from `aaa39fb` and is not exempted |
| [`refuse-11-ignored-build-input.txt`](controls/refuse-11-ignored-build-input.txt) | the command-line root's `.csproj.user` written, setting `Optimize` to false: ignored by `.gitignore`, imported by MSBuild | arm A's tree holds an ignored file a build may read |
| [`refuse-12-not-on-default-branch.txt`](controls/refuse-12-not-on-default-branch.txt) | `--default-branch` naming `865b69b`, the default branch before the merge | arm A is not on the default branch's first-parent history |

Controls 9 and 10 are the two moves the review found passing the first taking's step, which read git's rename
detection and so listed only the path a file took.

**The transcript checker refuses a wrong transcript** (exit 1), on the bytecode transcripts of the check above. The
checker has not changed since these were taken, and they were not taken again:

| File | What was changed | What the check said |
|---|---|---|
| [`checker-1-inadmissible-line.txt`](controls/checker-1-inadmissible-line.txt) | arm A's first line, `String(0)`, made `1`: the same length | (i) and (ii) hold, (iii) fails on that line alone; the check does not hold |
| [`checker-2-line-removed.txt`](controls/checker-2-line-removed.txt) | one line of arm A removed | (i), (ii) and (iii) fail; the check does not hold |

**The step passes where it should** (exit 0; both arms built, then it stopped):

| File | Arm A | What it showed |
|---|---|---|
| [`pass-1-merge-into-default-branch.txt`](controls/pass-1-merge-into-default-branch.txt) | the scratch merge itself | `--only measure-revised --check-only`: every refusal check passes, both arms build, and no transcript, answer or lane is run |
| [`pass-2-again-after-builds.txt`](controls/pass-2-again-after-builds.txt) | the same, on the same two trees again, holding pass control 1's `bin/` and `obj/` directories and Python's caches | those are allowed; each ignored `bin/` and `obj/` directory is removed before its arm's build, and both arms build again |

**SJL-12's VM corpus commit merged before the rule's, and does not void the run.** On 2026-10-20 the default branch
merged SJL-12's VM corpus branch at `2c0b110` as `865b69b` (#159), so every control's arm A carries it. Outside the
documents, every file it changes is under the four exempted paths. The root's closure and build files do not reach
them, and pass control 1 passes and builds.

**The arm A main assembly's hash** is the same in both pass controls, which built one commit in one checkout. It
differs from the check's above because each build embeds its commit in the assembly's informational version
(`0.1.0-preview.1+<commit>`) and the path of its checkout. Arm B's, built from one checkout at one commit, is the
same in every run.

## Files

| File | What it holds |
|---|---|
| [`revision-check.py`](revision-check.py) | the check's program |
| [`identity.txt`](identity.txt) | both arms' commits and main assemblies, the build and child command lines, the workloads by digest and blob, the program, the checker and the reference by digest, the SDK and the machine |
| [`answers.tsv`](answers.tsv) | arm, form, file, exit and answer, beside the base reading |
| [`configuration.txt`](configuration.txt) | each arm's effective-configuration lines |
| [`transcript-check.txt`](transcript-check.txt) | each transcript's digest, and the check's items (i) to (iii) per form, with counts per conversion |
| [`gate.txt`](gate.txt) | the gate's three items and its result |
| [`controls/`](controls/) | the revised rule's step and checker, each watched refusing and passing, as the section above lists |
