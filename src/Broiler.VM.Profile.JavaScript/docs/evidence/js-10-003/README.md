# Bundle JS-10-003 — the release-candidate run of the pinned suite

**Collected:** 2026-10-05. **Milestone:** JS-10, which it does not close.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** Release gate 9's facts
([section 22](../../roadmap.gates.md#22-release-gates)), retained once: four whole runs of the pinned
test262 revision from one binary built at one clean commit, **one run per manifest and form a run can
name**, each with its own totals, its failure manifest and the effective limit vector it was obtained
under, and the two runs that have a floor held to it. Phase F9's slice R3, decision
[JSD-0061](../../decisions/0061-packages-consumers-and-the-release-candidate.md). The
[release gate](../../release-gate.md) reads this directory file by file.

**Evidence class: `conformance`.** **The one predeclared decision**, written into `manifest.txt`
before any run: each run is recorded as this commit's release-candidate totals for its manifest and
form, and a run with a floor as not regressing it if the runner's `--floor` accepts it and as
regressing it otherwise. No figure is combined across runs and **no percentage is published**.

**What this bundle is not.** Not a support claim: no manifest is claimed, because the support table
is not issued. **Not accepted**: the reviewer field above says `none` and means it.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit 6ac061e`, `tree clean`, the binary's digest) and `hashes.txt` |
| **Dependencies and corpus** | `manifest.txt` (the suite pin's digest); each report's `run` and `edition` lines |
| **Environment** | `manifest.txt` (SDK 10.0.401, the machine, four jobs, the allowances); `limits-*.log` |
| **Procedure** | section 2 below, `eng/collect-js-release-candidate.py` and `eng/run-test262.py` |
| **Results** | `*.report.gz` (each merged report), `*.test262.json`, `*.merge.log`, `*.run.log` |
| **Negative controls** | the floors' own witnesses in the architecture suite; none new here |
| **Closure** | **none** |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From the checkout at `6ac061e`, a clean tree, on 2026-10-05:

```sh
python3 eng/collect-js-release-candidate.py --bundle JS-10-003 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-003 \
    --suite <the pinned test262 revision, unpacked> --jobs 4
```

1. **The manifest first**, made read-only, naming the commit, the clean tree and the digest of the
   conformance root built from it, which was copied aside so all four runs used the same binary.
2. **Per run, the limit vector first**: the conformance root's `--effective-limits` builds the runtime
   exactly as a variant's is built under that run's manifest and form, verifies one script, and prints
   the ceilings the verified handle froze - `limits-<run>.log`.
3. **Then the run**: `eng/run-test262.py` over sixteen shards with four processes, fuel 100,000,000
   and a 5,000 ms wall clock per variant, `--floor` where a floor exists. The merged report is
   retained compressed, the failure manifest - every variant that failed or exhausted an allowance,
   from that report - beside it.

## 3. Results

| Run | Manifest | Form | Passed | Failed | Outside the manifest | Exhausted | Skipped | Floor |
|---|---|---|---|---|---|---|---|---|
| `wide-bytecode` | `broiler.javascript.wide` | bytecode | 100,367 | 158 | 0 | 45 | 1,153 | **holds** |
| `wide-native` | `broiler.javascript.wide` | native, `x86-64-sysv` | 100,334 | 192 | 0 | 44 | 1,153 | **not compared** |
| `slice-bytecode` | `broiler.javascript.slice` | bytecode | 1,659 | 1,621 | 97,290 | 0 | 1,153 | none exists |
| `numeric-native` | `broiler.javascript.numeric` | native, `x86-64-sysv` | 6,172 | 2 | 94,396 | 0 | 1,153 | none exists |

Each run is over 53,469 files and 101,723 variants. Every run admitted every optional surface, which
each report's `manifest` line names. **The four rows are four runs; nothing here adds them.**

**The effective limit vector** is the same in all four runs: fuel 100,000,000; wall clock 5,000 ms;
allocated bytes 67,108,864; live bytes 33,554,432; host calls 1,000,000; call depth 6,144; verifier
work 100,000,000; artifact bytes 33,554,432; sections 64; declared count 4,194,304; structural depth
256; nested-load depth 4, fan-out 4,096 and bytes 16,777,216; live runtimes 4,096. Fuel and wall
clock are the runner's allowances; the rest are the profile's declared defaults, and live runtimes
is what the host's parent budget leaves. Each `limits-<run>.log` prints both the verification and the
instantiation ceiling per dimension; they agree.

**The wide bytecode run against run r32** ([JSC-284](../../roadmap.corrections.md#jsc-284)): one
variant moved, `test/built-ins/Atomics/waitAsync/no-spurious-wakeup-on-add.js` [strict], from passed
to exhausted on `LiveBytes`. Run alone six times afterwards from the same build, it exhausted once and
passed five times: what the main agent retains while it drains its job queue waiting on a second
agent depends on how long that agent takes. It is a finding, recorded in
[JSC-287](../../roadmap.corrections.md#jsc-287), and the floor deliberately holds no exhaustion.

## 4. The two floors

- **`wide-bytecode` holds `test262-wide.floor`**: `wide-bytecode.floor.log`.
- **`wide-native` could not be compared with `test262-wide-native.floor`**: that floor was set from a
  run in the `x86-64-win64` native form, this run is in the `x86-64-sysv` form, and the runner refuses
  to compare two forms' totals and exits 4 rather than calling the difference a regression. Re-basing
  is `--admit`'s, which no collection passes. So **the native form's ratchet is unchecked on
  `linux-x64`**, and the release gate names that as blocker `G9-wide-native-ratchet`. A run on
  `win-x64`, or the owner admitting a floor for the System V form, clears it.

## 5. Exclusions

1. **One machine, one RID**, four processors; the `win-x64` native form is not run.
2. **The native floor is unchecked**, section 4.
3. **The slice and numeric manifests have no floor**, so their totals are recorded and held to
   nothing.
4. **One variant's verdict depends on timing**, section 3; others may, unobserved in one run.
5. **The collector's own summary line** called the native run "not retainable as taken" on its exit
   code 4; the runner's transcript says the run may be retained, and the collector's wording has since
   been corrected to name a floor instead. The summary line is not retained here.
6. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
