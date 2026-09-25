<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle JSV-4-001 — the value form, judged once: REFUSE

**Collected:** 2026-09-24 (the wide shapes, which the rule judges) and 2026-09-25 (Octane, reported). **Decision:** [JSD-0035](../../decisions/0035-the-value-form-emitted-semantics-over-nan-boxed-values.md)
section 10, by this folder's [decision rule](decision-rule.md). **Evidence class:** one measurement.
**Owner:** JavaScript profile owner. **Reviewer:** none.

**What this bundle is.** The one measurement the value form is judged by, collected at the commit that
completes stage JSV-4, under the rule committed before any code of stage JSV-2 existed. It holds exactly
one predeclared decision, and this README is the one place a figure from the measurement is written, as
the rule and `docs/baselines.md` rule L1 require. **Nothing in it has been reviewed.**

## The verdict

**REFUSE.** `summary.txt` computes it from the rule's fenced block and from nothing else:

| Clause | | What the measurement says |
|---|---|---|
| 1. every condition, and fuel parity | **met** | every lane of every repetition answered its expected output, the conditions before and after the timing agree, and every twin completes at one smallest allowance in the bytecode and value forms (and in the control) and refuses with `AllowanceExhausted on Fuel` at one less |
| 2. at least 5 of 10 shapes with D > N and Q(V)/Q(B) ≤ 0.90 | **not met** | **0 of 10.** The value form is slower than bytecode on every shape, and beyond the A/A floor on every one |
| 3. every shape with Q(V) ≤ 1.25 × Q(B) + N | **not met** | past it on eight shapes; only `fib.js` and `throw-loop.js` are within it |
| 4. every shape with \|E\| ≤ 0.03 × Q(B-control) | **not met** | the interpreter moved on four shapes: `array-loop.js`, `property-loop.js`, `string-loop.js`, `throw-loop.js` |
| 5. one effective configuration | **met** | every timed child reported the same line (below) |

Clauses 1 and 5 hold, so the measurement is valid. Clauses 2, 3 and 4 each refuse on their own.

## The figures

Q is the median over the seven retained repetitions of `run_ms − check_ms`, in milliseconds. N is
`|Q(V) − Q(V-again)|`, one observation of the noise floor. D is `Q(B) − Q(V)` (positive: the value form
faster). E is `Q(B) − Q(B-control)`. Read off `summary.txt`:

| Shape | Q(B-control) | Q(B) | Q(V) | Q(V-again) | Q(L) | N | D | Q(V)/Q(B) | E |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| `array-loop.js` | 4212.4 | 4075.8 | 6910.9 | 6818.3 | 5307.9 | 92.6 | −2835.1 | 1.696 | −136.6 |
| `call-loop.js` | 4737.9 | 4673.7 | 6374.8 | 6371.7 | 7173.7 | 3.2 | −1701.2 | 1.364 | −64.2 |
| `closure-loop.js` | 4685.1 | 4689.9 | 6921.7 | 6869.2 | 6879.1 | 52.5 | −2231.8 | 1.476 | 4.8 |
| `fib.js` | 4986.3 | 4922.6 | 5074.6 | 5138.1 | 8409.1 | 63.5 | −152.0 | 1.031 | −63.7 |
| `generator-loop.js` | 4797.8 | 4922.7 | 6461.1 | 6504.3 | 7171.4 | 43.2 | −1538.3 | 1.312 | 124.9 |
| `numeric-loop.js` | 4324.1 | 4280.6 | 5524.7 | 5408.4 | 5681.1 | 116.3 | −1244.1 | 1.291 | −43.5 |
| `property-loop.js` | 5436.9 | 5612.5 | 9605.8 | 9715.5 | 6644.2 | 109.7 | −3993.3 | 1.712 | 175.6 |
| `string-loop.js` | 4048.6 | 4429.1 | 6710.4 | 6703.4 | 5515.8 | 7.0 | −2281.3 | 1.515 | 380.5 |
| `throw-loop.js` | 17580.3 | 15716.5 | 15953.4 | 15890.2 | 25953.8 | 63.2 | −236.9 | 1.015 | −1863.8 |
| `try-loop.js` | 4410.0 | 4478.3 | 5976.7 | 6023.5 | 5982.8 | 46.8 | −1498.4 | 1.335 | 68.4 |

**The spread behind each median**, the least and the greatest of the seven retained `run_ms − check_ms`
of each arm, read off `runs.jsonl` (rows with `"warmup": false`):

| Shape | B-control | B | V | V-again | L |
|---|---|---|---|---|---|
| `array-loop.js` | 4102–4329 | 4029–4202 | 6674–7221 | 6656–7032 | 5151–5422 |
| `call-loop.js` | 4637–4868 | 4625–4864 | 6274–6493 | 6237–6618 | 6974–7286 |
| `closure-loop.js` | 4450–4746 | 4508–4764 | 6784–7006 | 6596–7207 | 6752–6989 |
| `fib.js` | 4794–5121 | 4777–5082 | 4848–5200 | 4925–5193 | 8001–8566 |
| `generator-loop.js` | 4631–5011 | 4634–5194 | 6342–6564 | 6139–6732 | 7062–7416 |
| `numeric-loop.js` | 4136–4460 | 4210–4364 | 5372–5600 | 5224–5804 | 5469–5748 |
| `property-loop.js` | 5320–5512 | 5497–5656 | 9450–9955 | 9302–9894 | 6424–6902 |
| `string-loop.js` | 3927–4195 | 4341–4988 | 6496–6910 | 6624–6796 | 5353–5669 |
| `throw-loop.js` | 17194–17678 | 15403–16007 | 15673–16410 | 15523–16336 | 25304–26557 |
| `try-loop.js` | 4390–4481 | 4346–4745 | 5846–6061 | 5900–6082 | 5573–6095 |

What the two tables let a reader check without trusting the verdict line:

- **Clause 2 does not turn on a median.** On eight shapes the value form's fastest retained repetition is
  slower than bytecode's slowest. On `fib.js` and `throw-loop.js` the ranges overlap, and the value form's
  median is still the slower.
- **Clause 4 is not noise on two of its four shapes.** On `string-loop.js` the candidate's bytecode is
  slower than the control's in every repetition, and on `throw-loop.js` it is faster in every one: the
  ranges do not meet. On `array-loop.js` and `property-loop.js` the drift is just past three percent and
  the ranges overlap. The rule's clause 4 has no floor term, so it counts those two as moved. **This bundle
  does not attribute the drift.** The commits between the two arms are stages JSV-2 to JSV-4 and the
  harness; among what they changed on the interpreter's own path are the call path factored for direct
  calls (`EnterCall`, `EnterDepth`, `LeaveCall`, JSV-3) and the dispatch loop's landing factored into one
  routine (`Land`, JSV-4). A cause is a claim for another bundle to make.
- **The baseline form, L, is judged by no clause**, and it is slower than bytecode on every shape too.

Every timed child reported
`runtime-identifier=linux-x64 process-architecture=X64 gc-server=False gc-latency=Interactive tiered-compilation-env=unset tiered-compilation-config=unset tiered-pgo-env=unset tiered-pgo-config=unset dynamic-code=True native-arming=installed`.

## Octane, reported and judged by no clause

`octane-bytecode.log`, `octane-native.log` and `octane-value.log` are `eng/run-octane.py` over the candidate
arm's own files, one form at a time, after the harness's run, on the same machine with nothing else
running, and with the two artifact allowances in every form (`--artifact-bytes 134217728
--nested-load-bytes 134217728`, which JSV-1 found `mandreel`'s value-form artifact needs). The `.json`
files beside them are the runner's `--report`. A score is the suite's own (higher is faster), and the
seconds are each benchmark process's wall clock as the runner prints it. Each form ran once, with no
repetition and no A/A lane, which is one reason the rule judges none of it.

| Benchmark | bytecode | baseline (L) | value (V) |
|---|---:|---:|---:|
| `richards` | 63.7 (3 s) | 46.5 (5 s) | 39.5 (5 s) |
| `deltablue` | 69.4 (5 s) | 46.9 (7 s) | 42.4 (8 s) |
| `crypto` | 59.1 (70 s) | 51.7 (77 s) | 195 (25 s) |
| `raytrace` | 190 (14 s) | 144 (18 s) | 127 (21 s) |
| `earley-boyer` | 214 (53 s) | 157 (70 s) | 141 (75 s) |
| `regexp` | 134 (15 s) | 108 (18 s) | 122 (17 s) |
| `splay` | 611 (7 s) | 552 (8 s) | 444 (9 s) |
| `navier-stokes` | 130 (20 s) | 100 (26 s) | 236 (13 s) |
| `pdfjs` | 315 (18 s) | 229 (24 s) | 233 (27 s) |
| `mandreel` | 96.5 (229 s) | 79.9 (272 s) | 105 (205 s) |
| `gbemu` | 359 (31 s) | 271 (40 s) | 305 (38 s) |
| `code-load` | 1079 (3 s) | 726 (5 s) | 321 (8 s) |
| `box2d` | 291 (16 s) | 238 (19 s) | 219 (22 s) |
| `zlib` | 80.1 (1907 s) | 68.9 (2219 s) | 117 (1306 s) |
| `typescript` | 1165 (111 s) | 846 (152 s) | 732 (177 s) |
| **Octane's aggregate** | **205.8** | **161.2** | **176.5** |

**Fifteen of fifteen benchmarks report a score and exit zero in every form.** The value form's score is
above bytecode's on four benchmarks - `crypto`, `navier-stokes`, `mandreel` and `zlib` - and below it on
the other eleven, and its aggregate lies between the baseline form's and bytecode's. The owner ruled
before any candidate existed that Octane is reported beside the verdict and judged by no clause, so none
of this moves the verdict, and this bundle attributes none of it.

## How it was collected

- **The arms.** `manifest.json`, committed as `972334e` before any lane was timed, names both commits and
  every file of each arm by SHA-256. The candidate is `99a849f`, the commit that completes stage JSV-4 and
  records its exit gate; B, V, V-again and L are its bytecode, value, value-again and baseline forms. The
  control is `f490be0`, the commit that added the rule; B-control is its bytecode form. Each arm is
  `dotnet build … Broiler.VM.Composition.JavaScript.Cli.csproj -c Release -o <dir>`, run in a clean
  detached worktree of its commit.
- **The harness.** `eng/measure-value-form.py` as of `91afa05`, whose blob id the manifest pins. It read
  its constants out of the rule's fenced block, and it refused to start unless the rule was byte for byte
  the file the control commit added and the shapes were the control commit's.
- **The run.** 2026-09-24, 20:59 to 22:14 UTC, on the machine the manifest names: a cloud container with
  four logical processors of an `Intel(R) Xeon(R) Processor @ 2.80GHz`, Linux x64, .NET
  `Microsoft.NETCore.App 10.0.12`, with nothing else running (`machine-state.log` samples the load before
  and after every repetition). Ten shapes, ten lanes per shape per repetition in the fixed order
  `Bc, B, V, V-again, L`, each a check lane and a run lane; three warm-up repetitions, retained in
  `runs.jsonl` and excluded from Q, and seven retained: 1,000 timed processes, every one with condition
  `ok`.
- **The files.** `console.log`, `harness.log`, `condition-before.log`, `condition-after.log`,
  `machine-state.log`, `runs.jsonl`, `runtime-reports.log.gz` and `summary.txt` are the harness's output,
  written to the session's scratch directory and copied here unchanged (`harness.log` records the path it
  was written to); `runtime-reports.log` is compressed and nothing else is. The Octane logs and reports
  are `eng/run-octane.py`'s.

## Departures, stated rather than left to be found

1. **Every child was started by the `dotnet` host, not the application host.** On this machine the
   runtime is found by the application host only through `DOTNET_ROOT`, which the rule's discipline
   scrubs from every child. The harness's header says so, and the manifest pins the host by SHA-256.
2. **The shapes folder's README states twin allowances this tree no longer charges.** Every twin's
   smallest completing allowance is lower than section 4 of `src/tests/forms/wide/README.md` says - in
   the control commit as in the candidate, so it is not the value form's doing. The harness bisects its
   own figure under B and holds the other arms to it, so the rule is unaffected; the figures it used are
   in `condition-before.log`.
3. **The harness was dry-run once before the manifest existed**, over three of the twins renamed as
   shapes, with two repetitions, in the session's scratch directory, and nothing from that run is used
   or retained.

## Disclosure: what had been seen when this was collected

The rule discloses what had been seen when it was written. Since then, and before this collection:

- **Informal single timings of the wide shapes in the bytecode and value forms**, outside any harness,
  on this machine, during stages JSV-2 to JSV-4, and a sampled profile of `numeric-loop.js` in the value
  form. They showed the value form slower than bytecode on every shape. They could not shape the rule,
  which was committed before them, or the verdict, which the harness computes from that rule; no figure
  from them is used here.
- **Each stage's exit-gate runs**: the whole pinned suite in the value form, under handle-stress and in the
  flat control, and all fifteen Octane benchmarks in the value form, whose scores were printed.

## What the verdict does

The rule says, and this bundle only records: **JSD-0035 is refused on this measurement. The commits of
stages JSV-2, JSV-3 and JSV-4 are reverted together in one new commit; whether stages JSV-0 and JSV-1,
which make no speed claim, stay in the tree is the owner's to rule.** The population is not narrowed
until the rule passes. This bundle, its rule, its manifest and its harness are evidence and are not
among the commits the revert names.

*(Recorded beside this section on 2026-09-25, and nothing above it is edited: **the owner ruled not to
take the revert**, and to keep every stage in the tree as an unadopted, opt-in form. The verdict is
unchanged - it is this measurement's and not the owner's - and JSD-0035's status paragraph says what the
ruling keeps.)*
