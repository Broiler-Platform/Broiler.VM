# Bundle JS-10-001 — the measurement lane, and the profile's two baselines

**Collected:** 2026-10-05. **Milestone:** JS-10, which it does not close.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** The first retained collection of the measurement lane JS-10 owns: the two
figures roadmap sections 16 and 18 open their questions against - **verification throughput per
byte** and **cold-start cost** - each on a JIT arm and a Native AOT arm, under roadmap
[section 17](../../roadmap.gates.md#17-measurement-discipline)'s rules. Phase F9's slice R1, decision
[JSD-0059](../../decisions/0059-the-release-under-the-mvp-programme.md). The figures are quoted in the
profile's baseline register, [`docs/baselines.md`](../../baselines.md), and rule **N33** holds the two
to each other.

**Evidence class: `baseline`.** **The one predeclared decision**, written into `manifest.txt` before
either arm ran: each figure is recorded as this profile's baseline on its arm if its
candidate-versus-control difference exceeds its A/A difference, and as below resolution otherwise.
Every one of the four exceeds it.

**What this bundle is not.** Not a comparison with anything, not a workload's score, not a claim
about another machine or RID, and **not accepted**: the reviewer field above says `none` and means it.

---

## 1. The required fields, and the file in this directory that carries each

Roadmap [section 4](../../roadmap.status.md#4-required-evidence-bundle) names nine.

| Field | Where it is |
|---|---|
| **Identity** | `manifest.txt` (`bundle`, `evidence-class`, `decision`) |
| **Source** | `manifest.txt` (`commit d9012fe`, `tree clean`, `submodules`) and `hashes.txt` |
| **Dependencies and corpus** | `manifest.txt` (the nine resolved dependencies of the conformance root, with their digests; the workload's digest) |
| **Environment** | `manifest.txt` (SDK 10.0.401, runtime 10.0.12, the machine) and the `configuration` lines of each log |
| **Procedure** | section 2 below, `eng/measure-js-baselines.py`, and the header of each log |
| **Results** | `measure-jit.log`, `measure-aot.log`; `publish-jit.log`, `publish-aot.log` |
| **Negative controls** | Rule N33's four witnesses - section 4 |
| **Closure** | **none** - section 5 |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From the checkout at `d9012fe`, a clean tree, on 2026-10-05:

```sh
python3 eng/measure-js-baselines.py --bundle JS-10-001 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-001
```

1. **The manifest first.** The script restored the conformance root for `linux-x64`, wrote
   `manifest.txt` and made it read-only before publishing anything; its SHA-256 heads every log and
   is checked again after the last arm. `hashes.txt` was written last.
2. **Two arms.** `jit` is a trimmed self-contained publish under the runtime's default tiering;
   `aot` is a Native AOT publish. Both build with trim and AOT warnings as errors, which the
   conformance root's project sets.
3. **Verification throughput**, in one child per arm: the lowering writes the 170,465-byte artifact
   for `the-statement-and-object-surface.js` once, outside every timed region; the candidate verifies
   it and the control passes it through FNV-1a; five untimed iterations each, then seven repetitions
   of candidate, control and A/A lanes of twenty iterations, interleaved, with a fresh runtime before
   each lane and the artifact verified after each.
4. **Cold start**, timed by the script: per repetition, ten launches each of the candidate
   (`--cold-start`: compose, lower `1 + 1`, verify, instantiate, run, print `completion=2`), the
   control (`--cold-start-control`: print the configuration and exit) and the candidate again as the
   A/A lane, seven repetitions.
5. **Effective configuration.** Every child printed `rid=linux-x64 arch=X64 gc=workstation
   concurrent=default tiered=default` and `aot=no` or `aot=yes` as its arm asked; the script fails an
   arm on any other.

## 3. Results

| Measurement | Unit | JIT | Native AOT | A/A, JIT | A/A, Native AOT |
|---|---|---|---|---|---|
| `verify-throughput` | byte | 40.2643 ns | 49.1986 ns | 317,440 ns over 20 verifications | 974,940 ns over 20 |
| `cold-start` | process | 276,954,782.8 ns | 4,485,046.9 ns | 15,787,255.5 ns | 257,489.3 ns |

Each per-unit figure is the difference of the medians divided by the units: 170,465 bytes, or one
process. Every repetition is in the logs. The first JIT repetition of `verify-throughput` is slower
than the rest, as code is still being tiered up; it is retained, and the median is what is quoted.

## 4. Negative controls

**Rule N33's four witnesses**, which the architecture suite runs on every build: a register that
declares a row no log carries, one that omits a measured row, one that exchanges the two arms'
figures, and a log in which the lane refused to publish `cold-start` because its A/A lane exceeded
its effect. Each is a file under `src/tests/Broiler.VM.Architecture.Tests/witnesses/js-baselines/`,
and each fails the rule with the violation it names.

**The lane's own refusals are not exercised here**: no arm in this bundle reported another
configuration or failed its condition, so the paths that fail an arm for either are read in the
script and not witnessed by a run. That is exclusion 4.

## 5. Exclusions

1. **No closure.** The arms were published to measure them and not to claim anything about their
   images; no closure report is retained, and no RID is claimed.
2. **One machine, one RID**, four processors, one run per arm.
3. **No other configuration**: server GC, other tiering and other RIDs are unmeasured.
4. **The lane's failure paths** - a configuration mismatch, a failed condition, a changed manifest -
   are not witnessed by a run, only by reading `eng/measure-js-baselines.py` and `Measurement.cs`.
5. **No host latency budget exists** to read these figures against, so the decision this bundle
   takes is the baseline and nothing else.
6. **Not reviewed.** Release gate 11 publishes nothing a named human has not read.
