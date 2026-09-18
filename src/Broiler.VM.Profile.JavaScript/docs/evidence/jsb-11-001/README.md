<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle JSB-11-001 — the baseline native form over the wide manifest, on one Windows workstation

**Collected:** 2026-09-15. **Stage:** [JSB-11](../../roadmap.backends.md#jsb-11--the-baseline-form-over-the-wide-manifest-and-a-frame-that-still-holds-no-managed-reference)
of the backend roadmap, which it does not close. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** The first retained record of the baseline native form that
[JSD-0025](../../decisions/0025-the-baseline-native-form-over-the-wide-manifest.md) decides: every unit
of a `broiler.javascript.wide` artifact emitted as x86-64 machine code, every instruction one call into
the interpreter's own dispatch for that instruction. It is a collection over commit `444f9c9` on **one
`win-x64` workstation**, and it demonstrates **some** of JSB-11's exit gate. Section 8 goes through the
gate clause by clause, and section 9 names everything this bundle does not show.

**It was extended the same day with two continuous-integration workflow runs** *(added 2026-09-15,
after the runs)*. The `test262 javascript profile` workflow scored the whole pinned suite once in each
form on hosted Linux runners, at commit `06e1462`. No C#, project or build file differs between
`444f9c9` and `06e1462`; only documents, this bundle and the native floor's file do. `ci-identity.txt`
names both runs and every job; their merge logs and merged reports are retained beside it, and section
3 reads them. **The pair compares clean only under the comparison script as widened after the pair was
seen. Under JSB-11's gate as written, three of its differences are in no named class, so the
conformance clause is not met on it** (section 3; [JSC-224](../../roadmap.corrections.md#jsc-224)).

**It is not an acceptance and nothing in it has been reviewed.** `assurance-release.log` is the release
gate refusing because every relevant unit is `HUMAN_PENDING`, and that refusal is the correct answer.

**It has two halves, and they are not the same kind of record.**

- **What the profile's own collector wrote.** `eng/collect-js-evidence.py` is how this profile
  produces a bundle, and the backend roadmap's section 9 asks for exactly that. It ran here and wrote
  `identity.txt`, `environment.txt`, `snapshot-identity.txt`, `build.log`, `suite.log`, both assurance
  logs, `publish-and-run.log` with every `catalog-*.txt` and `closure-*.txt`, and `hashes.txt`.
- **What was run beside it and retained by hand.** The collector has no step for a test262 run, a
  comparison of two forms, the frame-cost measure, a JIT summary, a benchmark or a floor. Bundle
  [JSW-10-001](../jsw-10-001/README.md) retains its whole-suite runs and its Octane output the same
  way. Those files are listed in section 2 with the command, the build and the conditions of each,
  and `retained-hashes.txt` carries their digests.

**The collector ran with `--skip-controls --skip-fuzz`, and that is a decision with a cost.** Its
control matrix injects into C# sources and project files of the checkout and reverts them. None of its
controls reaches a mechanism this stage adds: there is no control for the baseline scan, the handler
table, the native form or rules `X2` and `X3`. Running the matrix would have re-demonstrated older
controls while modifying sources in a checkout this collection was told not to modify. The fuzz
sessions reach the verifier, the executor, the tokenizer and parser and the matcher, and JSB-11's gate
asks for none of them. **So this bundle's negative-controls field is thin**, and section 5 states
exactly how thin. The corpus-integrity mutation sits under the same flag and did not run either.

**Every figure in this README is read off a file in this directory.** The exceptions are the commit
identities of builds taken before the collection, which the logs do not carry and which section 2
attributes to the session's own notes, and one earlier image size that `aot-image-sizes.txt` also
attributes to those notes. **No benchmark score, no timing and no ratio of any kind is written in
this README.** The ten Octane reports retained beside it carry scores, and section 6 says why they are
kept and why none is quoted.

**`identity.txt` records the working tree as DIRTY, and it lists what.** Two entries. `.broiler-review/`
is an untracked review-tool directory that no build reads. `src/tests/conformance/floors/test262-wide-native.floor`
is the native floor, set before this collection; its narrative header was written after
`identity.txt` was, and **its machine-read rows did not change**, which `floor-native.log` checks
against the retained run. **No product or test source was modified.**

---

## 1. The required fields, and the file in this directory that carries each

[Section 4 of the ledger](../../roadmap.status.md#4-required-evidence-bundle) names nine. **A field is
satisfied by a file, never by a sentence here.**

| Field | Where it is |
|---|---|
| **Identity** | `identity.txt`, `snapshot-identity.txt`; for the workflow runs and the lane job, `ci-identity.txt` |
| **Source** | `identity.txt`, `hashes.txt`, `retained-hashes.txt`; the workflow runs' head commit in `ci-identity.txt` |
| **Dependencies and corpus** | `environment.txt`, `hashes.txt`; the pinned suite is named in every `test262-*.log` except the waves and in both `ci-merge-*.log`, and the Octane pin in every `octane-*.report` |
| **Environment** | `environment.txt` — one machine, `win-x64`, JIT, trimmed and Native AOT. For the workflow runs, only the runner label the workflow names, in `ci-identity.txt`; nothing retained records those runners' environment |
| **Procedure** | section 2 below, and the header of each log |
| **Results** | `build.log`, `suite.log`, `publish-and-run.log`, every `test262-*`, `ci-merge-*`, `forms-comparison*`, `frame-cost-*` and `octane-*` file, `ci-pr-lane-linux-x64-excerpt.log`, `bytecode-before-after-subtrees.log`, `bytecode-merge-base-against-branch.log`, `bytecode-workstation-against-ci.log`, `native-workstation-against-ci.log`, `floor-native.log`, `jit-execute-summary.txt`, `aot-image-sizes.txt` |
| **Negative controls** | **Thin, and deliberately** — section 5 |
| **Closure** | `closure-*.txt` and `catalog-*.txt`, read off the images `publish-and-run.log` published and ran |
| **Exclusions** | section 9, which takes in section 8's *What is missing* column |

---

## 2. Procedure

**The collection**, from the component root on 2026-09-15, at commit `444f9c9`:

```text
python eng/collect-js-evidence.py --bundle JSB-11-001 \
  --out src/Broiler.VM.Profile.JavaScript/docs/evidence/jsb-11-001 \
  --milestone JSB-11 --skip-controls --skip-fuzz
```

**The runs retained beside it.** Each test262 run was `eng/run-test262.py` against the pinned checkout
(`tc39/test262` at `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, content digest
`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`), under `broiler.javascript.wide`
with the harness loaded unless the row says otherwise.

**Where each column comes from, because the logs are not all alike.** The four whole-suite driver logs
and `test262-bytecode-subtrees.log` open with a header recording the manifest, the form as `bytecode`
or `native`, the shard count, the process count, the fuel and the wall clock; that header does not name
the calling convention. `test262-aot-native-subset.log` has no such header: it names the form with its
convention, and its allowance is read from the header of `forms-comparison-aot-subset.log`, which reads
it off the merged report. `test262-native-waves.log` names neither allowance nor convention; its wall
clock comes from the session's notes. **Every native run's convention is read from the
`forms-comparison*.log` headers**, which read it off the reports. **The logs do not record the commit
their binary was built from, or the state of the machine.** The build and machine-state columns come
from the session's own notes, which is weaker than a log line, and a dash means the notes record
nothing.

| Retained as | What it is | Build | Allowance per variant | Machine state |
|---|---|---|---|---|
| `test262-bytecode-reference.log`, `test262-bytecode-reference.report.gz` | whole suite, bytecode form | main `d126172` | fuel 100000000, wall 5000 ms | loaded by other work |
| `test262-native-wide.log`, `test262-native-wide.report.gz` | whole suite, native form `x86-64-win64` | `444f9c9` | fuel 100000000, wall 5000 ms | quiet apart from the frame-cost measure, which ran alongside it |
| `test262-bytecode-same-build.log` | whole suite, bytecode form, from the very binaries the row above ran | `444f9c9` | fuel 100000000, wall 5000 ms | quiet apart from two runs of the test suite of about fifteen seconds each |
| `test262-native-wide-60000.log` | whole suite, native form `x86-64-win64` | `0f6baec` | fuel 100000000, wall 60000 ms | loaded by other work |
| `test262-numeric-before.log` | whole suite, numeric manifest, native form, harness not loaded | main `d126172` | fuel 100000000, wall 5000 ms | loaded by other work |
| `test262-native-waves.log` | five named selections, native form, each compared with the bytecode reference | `0f6baec` | fuel 100000000, wall 60000 ms | — |
| `test262-aot-native-subset.log` | `test/language/statements/class` and `test/built-ins/Promise`, native form, run by the Native AOT conformance image | images published from `444f9c9` | fuel 100000000, wall 5000 ms | — |
| `test262-bytecode-subtrees.log` | six named subtrees, bytecode form, 32 shards across 8 processes | `0f6baec` | fuel 100000000, wall 5000 ms | — |

**The workflow runs, retained after the collection** *(added 2026-09-15)*. Each is a `workflow_dispatch`
run of `.github/workflows/test262-javascript-profile.yml` on branch `claude/wide-native-baseline-form`.
It scores the pinned suite in shards across eight runner jobs and merges the shard reports in a ninth
job; the merge logs name 32 shard reports each. Unlike the rows above, **the build, the runner label,
each job's conclusion and the shard jobs' limit are read from `ci-identity.txt`**, not from notes. The
form, convention and allowance are read from the merged report, as the comparison headers print them.
The merged artifacts were downloaded to `artifacts/ci-test262/machinecode/` and
`artifacts/ci-test262/bytecode/` before this extension. The download command is not recorded, and the
downloaded reports were not checked against the artifact digests `ci-identity.txt` carries.

| Retained as | What it is | Build | Allowance per variant | Machine |
|---|---|---|---|---|
| `ci-merge-machinecode.log`, `test262-ci-machinecode.report.gz` | whole suite, native form `x86-64-sysv`, run `35001811041` | `06e1462` | fuel 100000000, wall 5000 ms | eight hosted `ubuntu-latest` runner jobs |
| `ci-merge-bytecode.log`, `test262-ci-bytecode.report.gz` | whole suite, bytecode form, run `35001814153` | `06e1462` | fuel 100000000, wall 5000 ms | eight hosted `ubuntu-latest` runner jobs |
| `ci-pr-lane-linux-x64-excerpt.log` | lines selected by the `grep` at its top from the pull-request lane's `quick / publish and run (linux-x64)` job of run `34999734401`: the slice compiler's count of checks, and a native run of two subtrees under `x86-64-sysv` compared with a bytecode run | `06e1462` | as the excerpt prints it | the one runner that job names |

**The comparisons**, run for this bundle over the reports above. Each one reads two merged reports and
compares every shared variant's verdict and exhausted dimension:

```text
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-5000/test262.report   > forms-comparison.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-1/test262.report      > forms-comparison-60000.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/aot/t262-native/test262.report           > forms-comparison-aot-subset.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/g4-int/test262.report                    > bytecode-before-after-subtrees.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-samebuild/test262.report artifacts/test262-native-wide-5000/test262.report --exempt-guest-loads --suite <checkout> > forms-comparison-same-build.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-bytecode-samebuild/test262.report --suite <checkout>                        > bytecode-merge-base-against-branch.log
```

The fourth and the last are bytecode runs against bytecode runs, and the script prints that its classes
describe a comparison of two forms rather than those. The last line of every one of these logs except
`bytecode-before-after-subtrees.log` is the script's exit code, appended by the shell *(corrected
2026-09-15: this read "The last line of each of the final two logs"; `forms-comparison.log`,
`forms-comparison-60000.log` and `forms-comparison-aot-subset.log` end with that line too)*. Only the reference and the wall-5000 native report are
retained, compressed; `retained-hashes.txt` carries the digest of those two raw reports, of the subtree
report the fourth comparison read, and of the same-build bytecode report, **which is not retained
because it is the reference report's bytes**: the two files have one digest.

**The comparisons added with the workflow runs** *(added 2026-09-15)*, from the component root. The
last line of each log is the script's exit code, appended by the shell:

```text
python eng/compare-test262-forms.py artifacts/ci-test262/bytecode/test262.report artifacts/ci-test262/machinecode/test262.report --exempt-guest-loads --suite <checkout> > forms-comparison-ci.log
python <scratch>/compare-06e1462.py artifacts/ci-test262/bytecode/test262.report artifacts/ci-test262/machinecode/test262.report --exempt-guest-loads --suite <checkout> > forms-comparison-ci-06e1462-script.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/ci-test262/bytecode/test262.report --suite <checkout>                    > bytecode-workstation-against-ci.log
python eng/compare-test262-forms.py artifacts/test262-native-wide-5000/test262.report artifacts/ci-test262/machinecode/test262.report --suite <checkout>             > native-workstation-against-ci.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-1/test262.report --exempt-guest-loads --suite <checkout> > forms-comparison-60000-fd3a2aa-script.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-1/test262.report                                   > forms-comparison-60000-fd3a2aa-script-unexempted.log
python <scratch>/compare-fd3a2aa.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-1/test262.report                                   > forms-comparison-60000-fd3a2aa-script-no-suite.log
```

`eng/compare-test262-forms.py` there is the script at `fd3a2aa`, blob
`c559aa8f682564256b1b971001998efebe8df987`. `<scratch>/compare-06e1462.py` is the output of
`git show 06e1462:eng/compare-test262-forms.py`, blob `e9ee1cc4cf138073800909c9414c3e39c0f610f2`: the
script as it stood at the workflow runs' commit, and at the collection. `<scratch>/compare-fd3a2aa.py`
is the output of `git show fd3a2aa:eng/compare-test262-forms.py`, the same blob as the first. The sixth
command is the retained `forms-comparison-60000.log`'s own command, under the later script. That log
was taken under the earlier script and is kept as it was.

**Where a command names no `--suite`, the script looks for the checkout itself** *(added 2026-09-15)*.
It takes the one test262 checkout under `artifacts/` beside its own `eng/` directory, and without
`--exempt-guest-loads` it proceeds with none when it finds none. Run as `eng/compare-test262-forms.py`
from the component root, it finds the pinned checkout, so the commands above that name no `--suite`
read the same checkout as those that do. `forms-comparison-60000.log` and
`forms-comparison-60000-fd3a2aa-script-unexempted.log` each reproduce byte for byte with `--suite
<checkout>` added. A copy of the script in a scratch directory finds no checkout, and the last command
uses one to show what the widened script does then: no variant is known to load a program. The third
and fourth commands compare a form with itself, and the script prints that its
classes describe a comparison of two forms instead. `forms-comparison-synthetic-shapes.log` holds four
hand-built pairs of one-row reports run through the script at `fd3a2aa`, each printed in full beside
the script's output.

**Why both workflow reports are retained, compressed, rather than only their digests.** This bundle
retains, compressed, the reports that a reading of the conformance clause rests on. It keeps only a
digest for a report that is another retained report's bytes, or that rests nothing. The workflow pair
is what the clause's workflow half now rests on. It also carries the three differences the clause
calls defects, so a reader must be able to put both versions of the script over its rows again. And the
workflow keeps its own copies for a limited time: `ci-identity.txt` records the merged artifacts
expiring 30 days after the runs and the shard artifacts after 14. Both are `gzip -n -9`, the setting
that reproduces `test262-bytecode-reference.report.gz` byte for byte from its raw report.
`retained-hashes.txt` carries the digests of the two compressed files and of the two raw reports. It
also carries the digest of the 60000 ms native report that the three new 60000 ms comparisons read,
which is still not retained.

`test262-native-waves.log` was **not** produced by that script. It came from a working comparison
script that is not in this repository, which is why section 3 leans on it least.

**The floor**, run for this bundle, is the command at the top of `floor-native.log`.

**The frame-cost measure**, run for this bundle against the JIT Release command-line host the collector
had just built, is `python eng/measure-frame-cost.py --form bytecode` and `--form native`, each at the
top of its log. `frame-cost-bytecode-merge-base.log` is the same script with `--form bytecode`, pointed
by `--binary-directory` at a JIT Release command-line host built from main `d126172` in a detached
worktree after the collection; the command is at its top.

**The JIT summary.** `jit-execute-summary.txt` is the `JsEngine` lines of two
`DOTNET_JitDisasmSummary` transcripts, selected by the `grep` its header names, from JIT Release
command-line hosts of main `d126172` and of `0f6baec` running the same loop. The transcripts
themselves are not retained, because each is a quarter of a megabyte of every method the runtime
compiled.

**The image sizes** in `aot-image-sizes.txt` are `ls -l` over the images this collection published,
over the images the Native AOT subset ran on, and over images of main `d126172` published after the
collection with the command that file records.

**The benchmark reports** `octane-before-0.report` to `octane-before-4.report` and
`octane-after-0.report` to `octane-after-4.report` were each written by `eng/run-octane.py`. A working
script outside this repository drove it over the same selection, alternating the two hosts across five
repetitions. Each report records its own host path, pin, allowances and selection. The `before` host is
a JIT Release command-line host of main `d126172` and the `after` host one of `0f6baec`; both hosts
come from the session's notes, as does the machine being quiet.

---

## 3. The conformance suite in two forms

| Run | files | variants | pass | fail | unsupported | exhausted | skipped |
|---|---|---|---|---|---|---|---|
| bytecode reference | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| bytecode, from the wall-5000 native run's own binaries | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native, wall 5000 ms | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native, wall 60000 ms | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| bytecode, workflow run, `06e1462` | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native `x86-64-sysv`, workflow run, `06e1462` | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native, Native AOT image, two subtrees | 5,044 | 9,962 | 9,669 | 228 | 12 | 0 | 53 |

**Equal totals are not the finding, because two runs can share every total and disagree about
thousands of variants.** The finding is `forms-comparison.log`. Under one manifest, one harness and one
allowance, **all 94,545 variants are present in both reports and none differs** in verdict or in
exhausted dimension, and the script exits 0. Each of the four whole-suite logs ends by saying its run may be
retained: pinned, whole, and its verdicts account for it. The two workflow merge logs carry no such
line.

**The same comparison from one build** is `forms-comparison-same-build.log`. The bytecode run was
repeated from the very binaries the wall-5000 native run used, under the same manifest, harness,
allowance and process count: **all 94,545 variants are present in both reports and none differs** in
verdict or in exhausted dimension, and the script exits 0. **The reference from main and that repeated
bytecode run are one file twice**: the two merged reports have the same SHA-256, recorded in
`retained-hashes.txt`, so the two builds answer the suite identically in bytecode, row for row and
detail for detail (section 7).

**What those comparisons are not.** Each run was taken once, on one machine, under a wall-clock
allowance rather than a deterministic one, and not every run under the same load (section 2). Each
shard exiting 1 in the logs is the ordinary outcome for this suite, meaning cases failed; it is not a
harness failure.

**The workflow pair** *(added 2026-09-15)*. `ci-identity.txt` records both runs as `completed` with
conclusion `success` at `06e14627ef51e509b219dcdd65e65392898847bd`, and every job of each as `success`.
Each run has eight shard jobs, and each shard job finished inside the `timeout-minutes: 340` that the
workflow sets on it. Each job's duration is in that file and is not quoted here.
`ci-merge-machinecode.log` merges 32 shard reports under `broiler.javascript.wide`, with the harness
loaded, in form native `x86-64-sysv`; `ci-merge-bytecode.log` merges 32 in bytecode. Both are whole.

**That is the whole conformance suite executed in the System V convention, which this bundle had not
retained before.** It is not the first execution of that convention. `ci-pr-lane-linux-x64-excerpt.log`
shows the pull-request lane scoring two subtrees in it on the same commit, with timestamps earlier than
the creation of the workflow runs in `ci-identity.txt` (section 4).

**Per variant, no verdict differs.** In `forms-comparison-ci.log`, all 94,545 variants are in both
reports and ten differ. Each of the ten is exhausted in both reports and differs only in which
allowance ran out. `native-workstation-against-ci.log` holds the workstation's wall-5000 native run
(`x86-64-win64`, `444f9c9`) against the workflow's native run (`x86-64-sysv`). There too all 94,545
variants are in both, six differ, and each of the six is exhausted in both. **So every verdict of both
workflow runs is the workstation native run's verdict**, in two conventions on two kinds of machine,
from one run of each.

**Under the script as it stood at the runs' commit, the pair fails.** `forms-comparison-ci-06e1462-script.log`
admits seven differences into class (c), each a native wall-clock exhaustion against a bytecode
exhaustion of another allowance. It leaves three unclassified and exits 1:
`test/built-ins/decodeURIComponent/S15.1.3.2_A2.5_T1.js` in both variants and
`test/staging/sm/Date/dst-offset-caching-2-of-8.js [strict]`. Each of the three is a **bytecode**
wall-clock exhaustion against a **native** fuel exhaustion.

**Under the script at `fd3a2aa` the pair passes, and that script was widened after this pair was
seen.** `forms-comparison-ci.log` puts all ten into class (c) and exits 0. The widening adds one shape
to class (c): the bytecode run exhausted the wall and the native run exhausted another allowance. It is
admitted only when a run was taken below 60000 ms. `forms-comparison-synthetic-shapes.log` shows where
the new shape stops at `fd3a2aa`:

- a native `Failed` or `Unsupported` against a bytecode wall-clock exhaustion stays unclassified;
- the new shape stays unclassified when both walls are 60000 ms;
- the new shape is admitted when both walls are 5000 ms.

**What the widening rests on.** `bytecode-workstation-against-ci.log` holds the workstation's bytecode
reference (main `d126172`) against the workflow's bytecode run, with no native run in it. All 94,545
variants are in both, and eight differ. Every one of the eight is `Exhausted/WallClock -> Exhausted/Fuel`
on a `test/staging/sm/Date/dst-offset-caching-*` variant, which is the shape the script at `06e1462`
leaves unclassified in `forms-comparison-ci-06e1462-script.log`. The two runs are of one form, built
from main and from `06e1462`. `06e1462`'s product sources are the collected commit's, and main's
bytecode report and the collected build's are one file (section 7). Split across two machines, the
same variants run out of wall clock in one run and of fuel in the other, which points at the machine
rather than the form. **That is an argument from one run of each, on two machines, and not a
demonstration** that no native defect sits behind the three rows. A variant exhausted in both forms
gave no answer to compare, and nothing retained shows what either form would have answered with the
allowance lifted.

**Under JSB-11's gate as written, those three rows are defects.** Under a wall-clock allowance, the
conformance clause admits one class beyond the deterministic ones: a wall-clock exhaustion **in the
native form**. It calls any other difference a defect, to be fixed before the clause is met rather than
classified into it. The three are wall-clock exhaustions in the bytecode form. The seven that the
earlier script admitted are native wall-clock exhaustions, which the clause names. **So this bundle does
not count the pair as comparing the way the clause asks**, and the gate's text was not amended with the
script ([JSC-224](../../roadmap.corrections.md#jsc-224)).

**The script's exit code was never the gate, at either commit** *(added 2026-09-15)*. At `06e1462`,
class (c) already admitted a second shape, a bytecode wall-clock exhaustion where the native run
passed, and that is not a wall-clock exhaustion in the native form either. A comparison can therefore
pass the script at either commit without meeting the gate. **Every clause reading in this bundle rests
on the classes the gate names, row by row, and not on the script's exit code.**

**The run at 60000 ms is recorded and rests no clause.** `forms-comparison-60000.log` exits 1 under the
script as it stood at the collection *(this sentence read "rests no clause, because
`forms-comparison-60000.log` exits 1"; revised 2026-09-15. That log's own command still exits 1 under
both scripts: `forms-comparison-60000-fd3a2aa-script-unexempted.log` is that command under the widened
script. The widened script exits 0 only when `--exempt-guest-loads` is added, or when it finds no
checkout, and the retained command did neither. The sentence now rests on the gate reading below, not
on the exit code)*. All 94,545 verdicts agree. **Forty variants differ in the exhausted dimension.** Each is exhausted in
both reports: on the wall clock in the bytecode reference, which ran at 5000 ms, and on fuel (38) or
on nested-load bytes (2) in the native run at 60000 ms. The script admits none of them into a named
class. **This bundle does not classify them either.** The allowance differs between the two runs, and
the same-allowance comparison above shows no such row, which points at the allowance rather than the
form. **Pointing is not a demonstration**, and the gate's deterministic-allowance comparison would
need a bytecode run at the same allowance, which does not exist. This is recorded because the
session's notes described that run as having no differences, which is true of verdicts and not of
dimensions.

**The same comparison under the widened script** *(added 2026-09-15)* gives three readings:

- **With `--exempt-guest-loads`, it exits 0.** `forms-comparison-60000-fd3a2aa-script.log` puts the 38
  fuel rows into class (c), as the new shape, and the 2 nested-load-byte rows into class (b).
- **Without `--exempt-guest-loads`, as the retained log's own command, it exits 1.**
  `forms-comparison-60000-fd3a2aa-script-unexempted.log` puts the 38 into class (c) and leaves the 2 in
  class (b) without the exemption.
- **With no checkout found, it exits 0.** `forms-comparison-60000-fd3a2aa-script-no-suite.log` puts all
  40 into class (c), the 2 nested-load-byte rows included, because nothing tells the script that they
  load a program. The widened shape takes any bytecode wall-clock exhaustion against any other native
  exhaustion, so where guest loads are not detected it reaches rows that class (b) would otherwise hold.

**None of the three rests the clause.** The 38 are wall-clock exhaustions in the bytecode form against
native fuel exhaustions, which the gate does not name. **The 2 are outside the gate's named classes as
well**: the bytecode side is a wall-clock exhaustion, and the native side is an exhaustion of
nested-load bytes, where the gate's deterministic class names a fuel exhaustion on a variant that loads
a program. The script's class (b) admits nested-load bytes; the gate's words do not. The two runs were
also taken at different allowances, and the widening came after the workflow pair above.

**The Native AOT subset** is `forms-comparison-aot-subset.log`: 9,962 shared variants, none differs,
exit 0, with the 84,583 variants only the whole reference carries ignored and counted as ignored. That
is the native form run by a Native AOT image rather than a JIT one.

**The waves** in `test262-native-waves.log` are five named selections of 6,332, 755, 4,507, 17,638 and
3,824 variants. Each prints no variant that passed in one form and not in the other, in either
direction. They are retained as what they are — an earlier, partial, differently scripted reading on
another build — and nothing in section 8 rests on them alone.

**The numeric form, for scale rather than comparison.** `test262-numeric-before.log` is the native form
as it stood before this work, under `broiler.javascript.numeric`: 53,469 files and 94,545 variants,
passing 5,749, failing 189 and refusing 80,261 as unsupported, **by name**, because that manifest
admits a numeric subset and nothing else. It is a different manifest and it answers a different question.

**The floor.** `floor-native.log` holds, against the wall-5000 run, the floor that
`src/tests/conformance/floors/test262-wide-native.floor` records for the `x86-64-win64` form. No
workflow reads that file.

---

## 4. The scan and the two-forms rows

`publish-and-run.log` publishes the slice compiler in three modes and runs its checks in each. **Each of
the three answers 196 checks passed, 2 not run on this machine.** The rows this stage owns are there by
name, and a reader should read them rather than this summary:

- **The scan.** Every `x86-64-win64` and every `x86-64-sysv` emission of the fourteen named wide
  programs is accepted by the baseline scan; every baseline template of both conventions is reached;
  the baseline tables write no memory and transfer indirectly only through the handler table; both
  conventions emit the retained bytes for a property read, a branch and a throw; the two conventions'
  emissions are one template sequence; every wide baseline artifact re-emits byte for byte at
  verification under both; a re-emitting verifier refuses a swapped handler call. The refusals are a
  call through a register, a call through the slot of an undefined byte, a call between two slots, a
  status no unit materialises, a unit with no prologue, a `push` in the middle of a unit, a second
  `ret`, a branch into the epilogue and into the prologue, and a numeric emission judged as baseline
  and the reverse. Beside them are the `unchanged:` rows, the numeric and arm64 rows, and the arm64
  backend refusing the wide manifest by name.
- **The two forms.** `native/baseline/two-forms-agree-over-the-wide-manifest/*` covers the fourteen
  programs and the named probes. That means recursion to `RangeError` at the call-depth ceiling, a throw
  from deep recursion caught at the top, a long throw-and-catch loop, a generator's `return()` through
  two finallys, an uncaught `TypeError`, and an `eval` of a function called later. Around them:
  `the-smallest-completing-allowance-is-one-figure/*` for three programs;
  `a-swapped-handler-is-a-defect`; `entry-points-survive`, `a-misaligned-reservation-is-caught` and
  `a-short-reservation-loses-a-saved-register` under `x86-64-win64`; and
  `frequent-collections/*`, with collections forced.
- **Not run: the two `x86-64-sysv` rows**, `native/baseline/entry-points-survive/x86-64-sysv` and
  `native/baseline/a-misaligned-reservation-is-caught/x86-64-sysv`. A Windows host cannot enter a unit
  emitted for System V, and the transcript says so in those words.
- **On a Linux runner, counted and not named** *(added 2026-09-15)*. `ci-pr-lane-linux-x64-excerpt.log`
  holds lines selected from the pull-request lane's linux-x64 job at `06e1462`, and `ci-identity.txt`
  records that job's conclusion as `success`. There the slice compiler answers **195 checks passed, 3
  not run**. The three it names are the `x86-64-win64` rows of `entry-points-survive`,
  `a-misaligned-reservation-is-caught` and `a-short-reservation-loses-a-saved-register`, each not run
  because that machine uses `x86-64-sysv`. **That transcript names no passing row**, so no retained
  file names the two `x86-64-sysv` rows as passed; they are only absent from the rows it names as not
  run. The same job ran `test/language/statements/class` and `test/built-ins/Promise` in form native
  `x86-64-sysv` at wall 60000 ms, and compared them with a bytecode run at wall 5000 ms, whose header
  the excerpt carries: 9,962 variants in both, none differs, and every difference is in an admitted
  class *(the excerpt's `grep` was widened on 2026-09-15 to keep the comparison's reference and
  candidate headers, which it had dropped; no other line changed)*. The job's own log is not retained,
  and the lane retains nothing. `ci-identity.txt` also lists a later lane run on the branch, at
  `fd3a2aa`, completed with conclusion `success`; nothing of it is retained or read here.

**The transcript prints how long each form took on each row, and this bundle reads none of it.** A
per-row wall time printed by a checks lane on one run is not a measurement, and section 6 governs it.

**The retained corpus** replays in all three images of the execution-only root: 129 entries to their
recorded answers, twice with no residue, and 22 checks passed each time. The corpus manifest carries
`wide-a-baseline-payload-calling-an-undefined-slot` and `wide-a-native-payload-for-an-architecture-no-host-arms`.

---

## 5. Rules, witnesses and the controls this bundle does not have

`suite.log` passes with no failures: Contract 209 of 209, Architecture 238 of 238. Among the
Architecture tests are the two rules this stage adds, each held against a witness on disk:

- **`X2`**, in `NativeBaselineRuleTests.X2_the_baseline_frame_holds_no_reference`, with
  `X2_A_Baseline_Frame_Field_Holding_A_Reference_Is_Reported` reading
  `X2-a-baseline-frame-field-holding-a-reference.cs.witness`.
- **`X3`**, in `NativeBaselineRuleTests.X3_Native_Code_Enters_And_Finds_Its_Activation_Only_Where_The_Record_Argues`,
  with `X3_An_Unmanaged_Entry_Outside_The_Handler_File_Is_Reported` reading
  `X3-an-unmanaged-entry-outside-the-handler-file.cs.witness` and further members for a second writer
  or reader of the slot, a type naming the slot, an alias of the activation, and an input with nothing
  to quantify over.

**A witness is a file the rule is shown failing on. It is not an injection into the tree that is
reverted and watched passing again**, and the gate asks for both halves. The collector's matrix, which
is where this profile keeps the second half, has no control for either rule, and it did not run.
**No control in this bundle was watched failing and passing after revert.**

---

## 6. The benchmark gate was measured, its reports are retained, and no number from them is quoted

**What was measured.** The interpreter no-regression benchmark comparison: the pinned Octane suite
driven by `eng/run-octane.py` through JIT Release command-line hosts in the **bytecode** form. Main
`d126172` was compared with the branch at `0f6baec` over six benchmarks, in five repetitions alternating
order, on a quiet machine. **Its ten raw reports are retained** as the `octane-*.report` files, every
one of them, whatever it shows.

**Why they are retained.** The ledger's Results field asks for raw outputs, including failures, and
says a bundle that keeps only the passing half is not a bundle. Leaving out the reports of a gate this
bundle says was run would leave out whichever half a reader cannot see. Bundle
[JSW-10-001](../jsw-10-001/README.md) retains `octane.log` with every score in it, on the terms that
the scores are numbers about one configuration and authorise no comparison with anything. These
reports are retained on the same terms.

**Why no score, median or ratio from them is written here or in any other record.** It compares two
builds of the interpreter, not the native form with the interpreter, so the sentence against stating
what a native form is worth is not the one that decides it. Four others do:

- **Each report says so of itself.** Its `not-a-baseline` field reads that it was produced by one run
  with no predeclared rule, no control, no A/A lane and no repetitions, and that no figure in the file
  may be written into a document. That field forbids writing a figure into a document, not keeping the
  file, and this README is a document. Its `retained` field is the driver's statement when it wrote
  the file, and copying the file here does not make it a baseline. Five interleaved repetitions do not
  change what each file is, and there is no A/A lane among them.
- **The profile's release gate 10** ([measurement honesty](../../roadmap.gates.md#22-release-gates))
  admits no claim without a predeclared rule, a comparable control, an A/A lane and retained
  repetitions. **The core's release gate 8** admits no language performance claimed or implied, and
  its gate 11 adds that a record may state that a native form exists and not what it is worth.
- **The backend roadmap carries no figure of any kind** and makes no claim about speed in section 8,
  and [JSD-0025](../../decisions/0025-the-baseline-native-form-over-the-wide-manifest.md) section 9
  states no outcome figure in any record it touches. So the numbers stay in the raw files and are not
  carried into a sentence from which they could travel.
- **JSB-11's own clause could not be met by it anyway.** It asks for the geometric mean to fall within
  a tolerance **predeclared in this bundle before the run it judges**. This bundle did not exist before
  that run, and a tolerance chosen after the numbers are known is not a tolerance. The design note the
  work was built from, which is outside this repository, also named a different procedure: a Native
  AOT host, a different set of benchmarks and more repetitions.

**So this section records that the gate was run, what it compared and which files hold its output,
and no reading of what it found.** The clause is unmet in section 8.

---

## 7. The interpreter's instantiation, the call depth and Native AOT

**The interpreted instantiation reaches optimised code.** `jit-execute-summary.txt` shows main's
`JsEngine.Execute` compiled `Tier-0 switched to FullOpts` with IL size 9957 and code size 31634. It
shows the branch's `JsEngine.ExecuteCore[JsInterpreted]` compiled `Tier-0 switched to FullOpts` with IL
size 10513 and code size 31034, and the branch's `JsEngine.Execute` as a `Tier0` wrapper of IL size 83
and code size 466. **The clause's tolerance was not predeclared, and the stack reservation is not in a
summary line and was not collected.**

**The bytecode form over named subtrees, before and after.** `test262-bytecode-subtrees.log` is a
bytecode run over `test/language/statements/class`, `generators` and `try`,
`test/language/expressions/object`, `test/built-ins/Array` and `test/built-ins/Promise`: 9,762 files
and 19,227 variants. `bytecode-before-after-subtrees.log` compares it with the merge base's whole-suite
bytecode reference under the same manifest, harness and allowance: **19,227 variants in both reports,
none differs** in verdict or exhausted dimension, 75,318 present only in the reference are ignored, and
the script exits 0. **The conditions are weaker than the words suggest.** The two runs are two builds,
main `d126172` and `0f6baec`, and `0f6baec` is not the collected commit. They ran at different shard
and process counts, once each, on one machine. The session's notes record an earlier before run over
the same selection on another build, whose report was not found when this bundle was written. This
retained comparison takes its place and is not a copy of it.

**The whole suite in bytecode, before and after.** `bytecode-merge-base-against-branch.log` compares the
merge base's whole-suite bytecode report with a whole-suite bytecode run from the binaries the
wall-5000 native run used (section 2): **94,545 variants in both reports, none differs**, and the
script exits 0. The two merged reports have one SHA-256, so they are the same bytes. **It is one run of
each**, on one machine, under a wall-clock allowance, the main run loaded by other work; and identical
verdict rows say nothing about speed, code size or stack reservation, which are what the clause's
tolerances ask about.

**The call depth is bounded in both forms, and at the merge base.** `frame-cost-bytecode.log`,
`frame-cost-native.log` and `frame-cost-bytecode-merge-base.log` each report a deepest returning and a
deepest throwing recursion of 5999, both stopped by the declared bound, on a declared guest stack of
100663296 bytes. **As each log says of itself, that is what the build promises and not what the stack
holds.** Equal figures under a bound are not a frame cost within a tolerance, no tolerance was
predeclared, the margin the gate names is not measured, and no capacity measurement with the bounds
lifted was taken.

**Native AOT.** `publish-and-run.log` publishes each of the four composition roots in JIT, trimmed and
Native AOT modes, with warnings as errors, and runs each. Every publish and every run exits 0.
`catalog-*.txt` and `closure-*.txt` are read off those images. `aot-image-sizes.txt` records the Native
AOT images at 4121088 bytes (command-line host), 5276672 (conformance), 3422208 (execution-only) and
4903424 (slice compiler). Those are **the same byte sizes as the images the subset of section 3 ran on,
and not the same bytes**: the file carries both sets of digests, and every pair differs.

**Main's images, for growth.** The same file records images of main `d126172` published after the
collection at 3753472, 4899840, 3066368 and 4413440 bytes in the same order. Each image at this commit
is therefore larger by 367,616, 376,832, 355,840 and 489,984 bytes. An earlier publish of main in the
session is recorded in the notes only, with the command-line host 512 bytes larger, and the file says
so. **No bound was predeclared in this bundle**, so the growth is recorded and the clause that records
it against a bound is unmet.

---

## 8. JSB-11's exit gate, clause by clause

A clause is met or it is not. **"Shown in part" is not met.**

| Clause | On this evidence | What is missing |
|---|---|---|
| The bytecode form is unchanged | **Not met** — shown in part: optimised instantiation (section 7); bytecode call depth bounded at this commit and at the merge base (section 7); verdict rows over six named subtrees identical to the merge base's, for `0f6baec`, and the whole-suite bytecode report of the collected build byte-identical to the merge base's (section 7); corpus replays (section 4); benchmark reports retained, not read (section 6) | Every tolerance predeclared in this bundle before the run it judges, without which the code size, the frame-cost measure and the benchmark geometric mean meet none; the stack reservation; the benchmark taken from the collected commit rather than `0f6baec` |
| The per-step instantiations, or the fallback | **Not met** — native call depth bounded (section 7); each Native AOT image's size retained at this commit and at the merge base (section 7); the fallback was not taken | Each instantiation's size within a bound and its absence of jump tables; the fresh-process cost of a first native variant within a bound; a fresh process recursing to the ceiling answering `RangeError`; the native frame-cost margin; a bound on image growth, predeclared |
| The scan closes over what the encoder emits | **Not met** — shown in part on `win-x64` for both conventions' bytes (section 4) | A refusal the transcript names as a call past the table; the verifier's reason on each refusal; a hand check of the retained bytes against the templates |
| The two forms agree over the wide manifest | **Not met** — the Windows half shown (section 4); on a Linux runner, the checks count 195 passed and name only `x86-64-win64` rows as not run (section 4) | **The `x86-64-sysv` rows named as passed** in any retained transcript: System V has executed over the conformance suite and over two subtrees on hosted runners (sections 3 and 4), but its checks rows appear by name only as not run on this workstation; the smallest completing fuel ceiling for every program that loads nothing, not three; a predeclared bound for the deep throw |
| Rules hold the rooting argument | **Not met** — rules and witnesses pass (section 5) | A control watched failing and passing after revert |
| The retained corpus replays unchanged | **Met on this machine** — section 4, in all three images, with the baseline-slot entry present | Nothing on this machine; no other runtime identifier replayed it for this bundle |
| The conformance suite in the two forms | **Not met** — on the workstation, no per-variant difference under one wall-clock allowance, across two builds and from one. **Observed: the machine-code and bytecode workflow runs on one commit, with every shard job of the machine-code run inside its job limit**, and no verdict differing between them (section 3) | **The workflow pair comparing the way the clause asks.** It exits 1 under the script at the runs' commit and 0 under the script widened after it was seen, and under the gate as written its three bytecode wall-clock exhaustions against native fuel exhaustions are defects, not a class. Also missing: a like-for-like run under a deterministic allowance; and a clean 60000 ms comparison, where the widened script admits 38 rows of the same shape that the gate does not name, and 2 rows of a bytecode wall-clock exhaustion against a native nested-load-byte exhaustion that the gate's words do not name either (section 3). The script's exit code is not the gate at either commit |
| A bundle retains all of it, with the audit | **Not met** | The audit: that no arm assigns a parameter or a pre-loop local other than the four the clause names, that no `continue` or `goto` bypasses the step boundary, and that every catch filter in the profile assembly is pure. **It was not performed for this bundle** |

---

## 9. Exclusions

**Every entry in section 8's *What is missing* column is an exclusion of this bundle.** Each is also
named below, with everything else this bundle does **not** show, so that this section can be read on
its own.

**The gate's clauses this bundle leaves unmet:**

- **No tolerance or bound was predeclared in this bundle before any run it would judge**, so every
  clause that names one is unmet whatever the run showed. That covers the code size of
  `JsEngine.ExecuteCore` against `Execute` at the merge base, the bytecode frame-cost measure, the
  benchmark geometric mean, each per-step instantiation's size, the fresh-process cost, the native
  frame-cost margin, the Native AOT image growth and the deep throw.
- **No stack reservation** of `JsEngine.ExecuteCore` or of `Execute` at the merge base was collected.
- **The subtree bytecode run and the benchmark were taken at `0f6baec`**, not at the collected commit,
  and the subtree comparison is across two builds at different process counts (section 7). The
  whole-suite bytecode comparison is from the collected build, and it does not stand in for the
  benchmark.
- **No per-step instantiation was inspected.** No bundle file shows any instantiation's size or whether
  it has a jump table.
- **No fresh-process cost** of a first variant in the native form against the same variant in bytecode
  was measured.
- **No fresh process was run recursing to the call-depth ceiling**, so no file shows it answering
  `RangeError` and never a call-depth abort.
- **No native frame-cost margin and no capacity measurement of the stack in either form.** The
  frame-cost logs report the declared bound.
- **The fallback was not taken**, and nothing here shows it is not needed.
- **No refusal of a call past the table named as such.** This bundle identifies no row of the
  transcript as that refusal (section 4 lists the refusals it does identify).
- **No verifier reason shown on each refusal.** This bundle does not show, refusal by refusal, the
  reason and the stated outcome the clause names.
- **No hand check of golden bytes** against the templates is recorded.
- **No `x86-64-sysv` checks row named as passed, and no System V unit entered on this workstation.**
  That convention's bytes are compiled, scanned, compared and re-emitted on this machine, and no unit
  emitted for it was entered here. WSL was not usable on this workstation. *(This entry read "No
  execution in the System V convention, anywhere", and said that none of the lane's places that would
  execute it had run for this commit. Corrected 2026-09-15: at `06e1462`, the pull-request lane's
  linux-x64 job executed it over two subtrees and the machine-code workflow run over the whole suite
  (sections 3 and 4). The lane's checks transcript names no passing row, and only selected lines of
  the lane's job log are retained.)*
- **The smallest completing fuel ceiling was compared for three programs**, not for every program that
  loads nothing.
- **No control watched failing and passing after revert**, for rule `X2`, rule `X3` or anything else
  (section 5).
- **The 60000 ms comparison rests no clause under either script.** Under the collection's script it
  exits 1, with forty differences in exhausted dimension that the script does not admit and this
  bundle does not classify. Under the widened script it exits 0 with `--exempt-guest-loads`, 1
  without, and 0 again when the script finds no checkout. Its 38 bytecode wall-clock exhaustions
  against native fuel exhaustions are a shape the gate does not name, and its 2 bytecode wall-clock
  exhaustions against native nested-load-byte exhaustions are outside the gate's words too (section 3). The same-allowance comparisons on the workstation, across two builds
  and from one, have no difference to classify. *(This entry read "The 60000 ms comparison exits 1";
  corrected 2026-09-15.)*
- **No like-for-like comparison under a deterministic allowance**: no bytecode run at the native runs'
  allowances other than 5000 ms exists.
- **The workflow pair does not compare the way the clause asks.** It is one run of each form at
  `06e1462` rather than the collected commit, on hosted runners, under a wall-clock allowance. It exits
  1 under the comparison script as it stood at its commit, and 0 only under the script widened after it
  was seen. Under the gate as written, three of its differences are defects rather than a class
  (section 3). *(This entry read "No continuous-integration run of either form, so neither the workflow
  comparison nor any shard's job limit is observed"; corrected 2026-09-15.)*
- **The comparison script was widened after the evidence it now passes was seen**, at `fd3a2aa`. The
  widening rests on an argument from one bytecode-against-bytecode comparison of two builds on two
  machines, one run of each ([JSC-224](../../roadmap.corrections.md#jsc-224)). The gate was not amended.
- **The downloaded workflow reports were not checked against the artifact digests** that GitHub
  reports in `ci-identity.txt`. What is retained and hashed is the bytes that were downloaded.
- **No workflow run of the native form was held to the floor**, and no workflow report was compared
  with any report other than those section 2 names.
- **The interpreter audit JSB-11 requires was not performed**, including the purity of catch filters
  that JSD-0025 names as a falsifier.

**Everything else:**

- **No acceptance and no review.** `assurance-release.log` refuses as it must, the review record's units
  stay `PENDING`, and no human has read a line of the form, its rules or this bundle.
- **One workstation, one runtime identifier, one run of each thing.** No test262 run, comparison or
  measurement here was repeated. The bytecode reference, the numeric run and the 60000 ms native run
  were taken while other work loaded the machine; the wall-5000 native run and the benchmark were taken
  on a quiet one; the machine state of the other runs is not recorded. The workflow runs and the lane
  job add hosted Linux runners, one run of each, and nothing about those runners is retained beyond
  the label the workflow names.
- **No benchmark figure, no timing and no statement of what either form is worth** in this README, for
  the reasons in section 6. The ten Octane reports are retained and not read, and the per-row times the
  checks transcript prints are not read either.
- **No corpus-integrity mutation, no fuzz session, no Android control and no negative-control matrix**
  ran in this collection (the flags are in section 2's command).
- **Main's images and main's frame-cost host were built after the collection**, from a worktree the
  collector did not see. `aot-image-sizes.txt` and `frame-cost-bytecode-merge-base.log` carry their
  commands, and neither the images nor the host are retained.
- **No proof of handler placement.** The scan shows every call lands in the table and the checks show a
  swapped handler answers a defect; nothing proves at verification that the right handler is at the
  right offset, as JSD-0025 section 9 says of itself.
- **No runtime identifier is claimed.** `win-x64` stays **Not claimed** in the support table, and this
  bundle is not a support claim for it, or for `linux-x64` on the strength of the workflow runs.
- **The floor is read by nothing automatic.** `test262-wide-native.floor` is held only by running the
  command in `floor-native.log`, and a System V run would re-base it rather than compare with it.
- **The waves comparison came from a script outside this repository** (section 2).
- **Build commits for the retained test262 runs, the benchmark hosts and the JIT summary come from the
  session's notes, not from the logs.**
