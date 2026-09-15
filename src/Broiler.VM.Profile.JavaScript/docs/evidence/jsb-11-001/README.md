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
| **Identity** | `identity.txt`, `snapshot-identity.txt` |
| **Source** | `identity.txt`, `hashes.txt`, `retained-hashes.txt` |
| **Dependencies and corpus** | `environment.txt`, `hashes.txt`; the pinned suite is named in every `test262-*.log` except the waves, and the Octane pin in every `octane-*.report` |
| **Environment** | `environment.txt` — one machine, `win-x64`, JIT, trimmed and Native AOT |
| **Procedure** | section 2 below, and the header of each log |
| **Results** | `build.log`, `suite.log`, `publish-and-run.log`, every `test262-*`, `forms-comparison*`, `frame-cost-*` and `octane-*` file, `bytecode-before-after-subtrees.log`, `floor-native.log`, `jit-execute-summary.txt`, `aot-image-sizes.txt` |
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
| `test262-native-wide.log`, `test262-native-wide.report.gz` | whole suite, native form `x86-64-win64` | `444f9c9` | fuel 100000000, wall 5000 ms | quiet |
| `test262-native-wide-60000.log` | whole suite, native form `x86-64-win64` | `0f6baec` | fuel 100000000, wall 60000 ms | loaded by other work |
| `test262-numeric-before.log` | whole suite, numeric manifest, native form, harness not loaded | main `d126172` | fuel 100000000, wall 5000 ms | loaded by other work |
| `test262-native-waves.log` | five named selections, native form, each compared with the bytecode reference | `0f6baec` | fuel 100000000, wall 60000 ms | — |
| `test262-aot-native-subset.log` | `test/language/statements/class` and `test/built-ins/Promise`, native form, run by the Native AOT conformance image | images published from `444f9c9` | fuel 100000000, wall 5000 ms | — |
| `test262-bytecode-subtrees.log` | six named subtrees, bytecode form, 32 shards across 8 processes | `0f6baec` | fuel 100000000, wall 5000 ms | — |

**The comparisons**, run for this bundle over the reports above. Each one reads two merged reports and
compares every shared variant's verdict and exhausted dimension:

```text
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-5000/test262.report   > forms-comparison.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/test262-native-wide-1/test262.report      > forms-comparison-60000.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/aot/t262-native/test262.report           > forms-comparison-aot-subset.log
python eng/compare-test262-forms.py artifacts/test262-bytecode-head/test262.report artifacts/g4-int/test262.report                    > bytecode-before-after-subtrees.log
```

The last is a bytecode run against a bytecode run, and the script prints that its classes describe a
comparison of two forms rather than this one. Only the reference and the wall-5000 native report are
retained, compressed; `retained-hashes.txt` carries the digest of those two raw reports and of the
subtree report the last comparison read.

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
| native, wall 5000 ms | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native, wall 60000 ms | 53,469 | 94,545 | 70,834 | 13,315 | 1,990 | 60 | 8,346 |
| native, Native AOT image, two subtrees | 5,044 | 9,962 | 9,669 | 228 | 12 | 0 | 53 |

**Equal totals are not the finding, because two runs can share every total and disagree about
thousands of variants.** The finding is `forms-comparison.log`. Under one manifest, one harness and one
allowance, **all 94,545 variants are present in both reports and none differs** in verdict or in
exhausted dimension, and the script exits 0. Each of the four whole-suite logs ends by saying its run may be
retained: pinned, whole, and its verdicts account for it.

**What that comparison is not.** The two runs came from **two builds**: the reference from main and the
native run from this commit. The gate asks for one build, and that condition is not met here. Each was
taken once, on the same machine, under different load. Each shard exiting 1 in the logs is the
ordinary outcome for this suite, meaning cases failed; it is not a harness failure.

**The run at 60000 ms is recorded and rests no clause, because `forms-comparison-60000.log` exits 1.**
All 94,545 verdicts agree. **Forty variants differ in the exhausted dimension.** Each is exhausted in
both reports: on the wall clock in the bytecode reference, which ran at 5000 ms, and on fuel (38) or
on nested-load bytes (2) in the native run at 60000 ms. The script admits none of them into a named
class. **This bundle does not classify them either.** The allowance differs between the two runs, and
the same-allowance comparison above shows no such row, which points at the allowance rather than the
form. **Pointing is not a demonstration**, and the gate's deterministic-allowance comparison would
need a bytecode run at the same allowance, which does not exist. This is recorded because the
session's notes described that run as having no differences, which is true of verdicts and not of
dimensions.

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
| The bytecode form is unchanged | **Not met** — shown in part: optimised instantiation (section 7); bytecode call depth bounded at this commit and at the merge base (section 7); verdict rows over six named subtrees identical to the merge base's, for `0f6baec` (section 7); corpus replays (section 4); benchmark reports retained, not read (section 6) | Every tolerance predeclared in this bundle before the run it judges, without which the code size, the frame-cost measure and the benchmark geometric mean meet none; the stack reservation; the subtree run and the benchmark taken from the collected commit rather than `0f6baec` |
| The per-step instantiations, or the fallback | **Not met** — native call depth bounded (section 7); each Native AOT image's size retained at this commit and at the merge base (section 7); the fallback was not taken | Each instantiation's size within a bound and its absence of jump tables; the fresh-process cost of a first native variant within a bound; a fresh process recursing to the ceiling answering `RangeError`; the native frame-cost margin; a bound on image growth, predeclared |
| The scan closes over what the encoder emits | **Not met** — shown in part on `win-x64` for both conventions' bytes (section 4) | A refusal the transcript names as a call past the table; the verifier's reason on each refusal; a hand check of the retained bytes against the templates |
| The two forms agree over the wide manifest | **Not met** — the Windows half shown (section 4) | **Any execution under `x86-64-sysv`**; the smallest completing fuel ceiling for every program that loads nothing, not three; a predeclared bound for the deep throw |
| Rules hold the rooting argument | **Not met** — rules and witnesses pass (section 5) | A control watched failing and passing after revert |
| The retained corpus replays unchanged | **Met on this machine** — section 4, in all three images, with the baseline-slot entry present | Nothing on this machine; no other runtime identifier replayed it for this bundle |
| The conformance suite in the two forms | **Not met** — no per-variant difference under one wall-clock allowance (section 3) | One build for both forms; a like-for-like run under a deterministic allowance; the forty unadmitted, unclassified differences of the 60000 ms comparison, which the gate calls defects until shown otherwise; the machine-code and bytecode workflow runs on one commit; every shard inside its job limit |
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
  and the subtree comparison is across two builds at different process counts (section 7).
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
- **No execution in the System V convention, anywhere.** That convention's bytes are compiled, scanned,
  compared and re-emitted on this machine, and **no unit emitted for it was entered**. WSL was not
  usable on this workstation. The only places that would execute it are the continuous-integration
  lane's `linux-x64` native test262 step and its checks rows, and **none of them has run** for this
  commit.
- **The smallest completing fuel ceiling was compared for three programs**, not for every program that
  loads nothing.
- **No control watched failing and passing after revert**, for rule `X2`, rule `X3` or anything else
  (section 5).
- **The two forms' whole-suite comparison is across two builds**, and the 60000 ms comparison exits 1
  with forty differences in exhausted dimension that the script does not admit and this bundle does not
  classify (section 3).
- **No like-for-like comparison under a deterministic allowance**: no bytecode run at the native runs'
  allowances other than 5000 ms exists.
- **No continuous-integration run of either form**, so neither the workflow comparison nor any shard's
  job limit is observed.
- **The interpreter audit JSB-11 requires was not performed**, including the purity of catch filters
  that JSD-0025 names as a falsifier.

**Everything else:**

- **No acceptance and no review.** `assurance-release.log` refuses as it must, the review record's units
  stay `PENDING`, and no human has read a line of the form, its rules or this bundle.
- **One workstation, one runtime identifier, one run of each thing.** No test262 run, comparison or
  measurement here was repeated. The bytecode reference, the numeric run and the 60000 ms native run
  were taken while other work loaded the machine; the wall-5000 native run and the benchmark were taken
  on a quiet one; the machine state of the other runs is not recorded.
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
  bundle is not a support claim for it.
- **The floor is read by nothing automatic.** `test262-wide-native.floor` is held only by running the
  command in `floor-native.log`, and a System V run would re-base it rather than compare with it.
- **The waves comparison came from a script outside this repository** (section 2).
- **Build commits for the retained test262 runs, the benchmark hosts and the JIT summary come from the
  session's notes, not from the logs.**
