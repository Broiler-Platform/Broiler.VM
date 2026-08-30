# Broiler.VM baselines

> **No language performance claim follows from any figure in this document.** Every measurement
> here is of the **core's own overhead** around a fixture profile whose executor is a toy stack
> machine. A real language profile's cost is its own, and nothing here predicts it. This sentence
> is in the VM-5 gate, in the benchmark host's own documentation and here, because it is the one
> misreading these numbers invite.

What the core costs a profile, so a profile can budget against it. Every figure is produced by
`src/tests/Broiler.VM.Bench.Host` and retained in the current evidence bundle's `bench.log`, on
both lanes. Rule **L1** holds this document and that log to each other in both directions: a
measurement declared here that the log does not carry fails, a measurement the log carries that
is not declared here fails, and a figure quoted here that the log contradicts fails.

Core contract version 1.

---

## 1. What a measurement is, and what it is not

A number on its own is a property of a machine. It becomes an attribution only beside a control
that differs from it in exactly one thing, so **every measurement here is a pair**: a candidate,
and the same workload with the core's part removed.

Five rules, applied to all ten, and fixed by the harness rather than by any row:

| | Rule | Why |
|---|---|---|
| 1 | **A control that is the same workload minus the core.** The same bytes scanned without a verifier; the same dispatch loop with charging off; the same artifact without the host call. | A difference between two *different* programs is a comparison, not an attribution. |
| 2 | **Interleaved lanes.** Candidate and control alternate inside each repetition rather than running as two blocks. | A machine that gets slower - thermal, another process, a core migration - then slows both by about the same amount instead of slowing whichever block ran last. Two blocks is how a drifting machine becomes a performance claim. |
| 3 | **An A/A lane.** The candidate is measured a second time, identically. The difference between those two is this machine's noise floor on this workload. | A candidate-versus-control difference smaller than the A/A difference is not a measurement of anything. This is the one judgement the harness makes. |
| 4 | **Every repetition retained.** Seven per lane, all printed, no outlier policy and no statistical model. | A policy that discarded a repetition would be a judgement about which measurements count. The reader makes that judgement; the spread between repetitions is most of what a single figure hides. |
| 5 | **A condition checked before AND after every lane.** The operation must still do what its name says. | A measurement whose operation quietly failed is the most dangerous output a harness can produce: it is fast, it is stable, and it is a number for the refusal path. |

Rule 5 is not theoretical. It caught three lanes in this milestone timing something other than what
they were named for, each one producing a plausible figure:

- `verify-per-declared-count` timed a **refusal**, because a runtime's `AllocatedBytes` ceiling is a
  total rather than a per-operation limit and a lane of two hundred verifications had spent it.
- `meter-per-instruction` timed a **terminal instance**, because 420,000 invocations of a
  256-instruction loop spend more Fuel than the profile's hard maximum admits.
- `guest-load-mediation` timed a **fan-out refusal**, because the mediator was not resetting its
  per-operation counters at all - which turned out to be a product defect and not a harness one
  (section 5).

The fix in each case is a **reset**, run outside every timed region, that starts a lane from a
runtime which has spent nothing. A lane gets one only if the check says it needs one, because a
reset is not free: adding one to `host-call` put a hundredfold more noise into its A/A floor than
the effect it was measuring.

### What the harness deliberately does not do

- **No benchmarking framework.** A framework's warmup, pilot and outlier policies would be part of
  every figure this component publishes and none of them would be visible in this repository.
  ADR 0001 revision 3 records that as the reason the bench host references no such package.
- **No pilot phase and no adaptive iteration count.** Iteration counts are constants, stated below.
- **No mean.** The median of seven, printed beside all seven.

---

## 2. The measurements

Ten, each with the control it is attributed against. Figures are the medians the current bundle's
`bench.log` retains, on a four-processor `linux-x64` machine with workstation GC and tiered
compilation off. **They are properties of that machine**; the ratios between them travel, the
absolute values do not.

| Measurement | Unit | Candidate | Control | JIT | Native AOT |
|---|---|---|---|---|---|
| `verify-throughput` | byte | Verifying a 4,000-constant artifact | A checksum pass over the same bytes | 157.6276 | 129.0454 |
| `verify-per-declared-count` | constant | Verifying a 4,000-constant pool | Verifying a 2,000-constant pool | 320.5245 | 289.3392 |
| `catalog-construction` | profile | Building a two-profile catalog | Building the same two descriptors into an array | 925.9000 | 1086.2000 |
| `catalog-lookup` | lookup | Resolving a profile by identity | Comparing the same identity | 5.0020 | 6.8290 |
| `runtime-create-dispose` | profile | Creating and disposing a two-profile runtime | The same for one profile | 1282.3500 | 627.9500 |
| `meter-per-instruction` | instruction | One invocation with fuel charging on | The same executor with charging off | 84.2966 | 83.5324 |
| `host-call` | call | An artifact making one host call | The same shape with a second push instead | 200.9300 | 245.7100 |
| `diagnostics-capture` | record | A fully identified diagnostics record | Its minimal form | 90.5790 | 86.2510 |
| `guest-load-mediation` | load | A mediated guest-initiated load | The same load performed by the host itself | 1795.3333 | 940.0000 |
| `envelope-read` | projection | Projecting a typed payload out of a result | Reading its category alone | 13.9380 | 10.9580 |

All figures are nanoseconds per unit. Every one is the **difference** between the two lanes divided
by the units in one iteration, so each is what the core adds and not what the operation costs
end to end.

### Reading the four that surprise

**`verify-throughput` is not a throughput number for a real format.** 158 ns per byte is roughly
5 MB/s, which sounds alarming until you see what it is measuring: the fixture's constant pool is
read one LEB128 varint at a time through `VmBoundedReader`, and **every byte consumed** is charged
through two interface calls into the core meter, each of which takes a lock and walks four budget
scopes. The per-byte figure is the metering discipline, not a decoder. `verify-per-declared-count`
corroborates it independently - 321 ns per constant marginal, against a 4,000-constant
verification whose whole difference from a raw pass is 1,459,519.0 ns, which is 365 ns each - and
the agreement between a total and a marginal measured different ways is the reason both are here.

**The core's per-verification framing is below the noise.** It follows from the two above: the
total for 4,000 constants and 4,000 times the marginal cost agree to within the A/A floor, so the
fixed cost of a verification - descriptor validation, handle creation, the lease - is too small for
this harness to resolve against the pool scan. That is a useful thing for a profile author to
know: verification cost is what the profile's verifier reads, not what the core wraps around it.

**`guest-load-mediation` is nearly free.** 1,795 ns is what routing a load through the mediator
adds over performing the same nested verification directly: the provider dispatch, the request and
answer marshalling, the depth and fan-out accounting, the charge against the requesting operation,
and the intersection of the nested handle's ceilings with that operation's remaining allowance. The
nested verification itself - which a caller pays either way - is in both lanes and cancels.

**`meter-per-instruction` dominates everything else.** At 84 ns per instruction the metering is
the cost of executing bytecode in this core, by an order of magnitude over any per-operation
figure here. A profile whose instructions do real work will amortise it; a profile of cheap
instructions will not.

---

## 3. Recorded figures

Not measurements. Each is a single observation with no control to attribute it against, no A/A
lane and nothing to repeat, and saying so is better than dressing an observation up as an
experiment.

| Figure | JIT | Native AOT | What it means |
|---|---|---|---|
| `startup first-verification-ms` | 112.2 | 9.8 | Process start to the first verified artifact. The AOT figure is the one a host waits for; the JIT figure includes the SDK host that launched it. |
| `image process-bytes` | 78,256 | 1,890,912 | On the JIT lane this is the shared host executable; on the AOT lane it is the image itself. |
| `image core-bytes` | 164,352 | 0 | The three packable assemblies as separate files. Native AOT links them in, so zero here reads as "not separable", never as "nothing". |
| `headroom guest-loads-per-runtime` | 64 | 64 | How many mediated loads one runtime admits before an allowance stops it, and the number the guest-load lane is sized against. Counted rather than timed, so both lanes agree. |
| `plateau held` | yes | yes | Whether the resident set stops growing under 24,000 whole lifecycles. Sampled in six rounds; the claim is that the last round is no larger than the second by more than a sixteenth. |
| `independence held` | yes | yes | Whether an operation costs the same after seventy thousand runtimes as after none. See section 5. |

Package sizes are **not** here. Packages are produced at VM-6 and a size for a package this
component does not yet build would be an invention.

---

## 4. The fan-out series

A recorded series, and the reason `guest-load-mediation` is stated per **single** load. Mediation
over one load costs about what performing that load directly costs; over several, each further
load in the same operation costs more than the last. A per-load figure averaged over six would
therefore belong to no load at all.

| Loads in one operation | JIT mediation (ns) | AOT mediation (ns) |
|---|---|---|
| 0 | 12 | -216 |
| 1 | 3,918 | 216 |
| 2 | 3,084 | 2,332 |
| 3 | 5,050 | 1,754 |
| 4 | 6,758 | 2,178 |
| 6 | 6,542 | 5,771 |
| 8 | 7,336 | 5,878 |

The row at zero loads is the control for the series itself: with no load to mediate, the two lanes
are the same program and the difference is noise in both directions.

This is what the VM-5 gate means by *optimization is funded only against one of these baselines*.
Nothing in this milestone acts on the series; anyone who later wants to has the numbers and the
shape, and a re-run of the bench host reproduces both.

---

## 5. What measuring found

Two product defects, both invisible to the behavioural suite, both found because a baseline asked
a question no test asked. Both are fixed, both now have a witness in
`Broiler.VM.Contract.Tests`, and both witnesses were confirmed against the defect before the fix
was kept.

### The per-operation guest-load bounds were per-runtime bounds

The mediator resets its fan-out, cumulative-bytes and nested-verifier-work counters when the
operation changes. It has an `EnterScope` overload that takes the operation's identity, and it had
another that did not - and **every call site used the second one**, passing `default`. Every step
therefore compared equal to the last and the reset never ran once.

The mediator is one object per profile per runtime, so what is documented as a per-operation bound
behaved as a lifetime bound shared by every instance of that profile: a runtime admitted its
fan-out limit worth of loads in total and never another. The bench host's headroom probe expected
a number in the thousands and printed **8**.

Three existing tests cover fan-out and all three passed, because all three invoke exactly once.
The fix removes the overload, so the identity cannot be omitted again. Witnesses:
`Fan_Out_Is_Refreshed_By_A_New_Invocation` and
`Fan_Out_Is_Not_Shared_Between_Two_Instances_Of_One_Profile`.

### A disposed runtime left per-thread state behind for good

A runtime kept its capability depth in an `AsyncLocal<int>`. An async-local entry is released from
a thread's execution context when it is set to `null`, and only then - and a value-typed one can
never be set to null: returning the depth to zero stores a boxed `0`, which is a present value. The
entry stayed on the thread for the life of the process, one per runtime that ever ran a capability
or a provider call there, released by nothing, **not even by disposing the runtime**.

Nothing observable failed, which is why no test caught it. What grew was the cost of every later
async-local write on that thread, because each one copies the whole map:

| After | An instantiate-and-invoke | One async-local write |
|---|---|---|
| 231 runtimes | 9,960 bytes | 72 bytes |
| 70,547 runtimes | 1,188,872 bytes | 393,072 bytes |

The whole benchmark took **528 seconds**; it now takes **43**. Zero is represented by the absence
of a value now, and the bench host carries an `independence` check - the same work sampled at the
start of a run and again at the end - which fails the run if per-operation cost grows with the
number of runtimes created. Witness: `A_Disposed_Runtime_Leaves_No_Per_Thread_State_Behind`.

Neither defect is a performance finding that VM-5 chose to act on. Both are correctness findings:
a bound that does not bound what it says, and a resource that is never released.

---

## 6. What these figures do not cover

| | Limit |
|---|---|
| **One machine, one RID.** | Every figure is from a single four-processor `linux-x64` machine. Nothing here is a cross-platform claim, and the absolute values do not travel. |
| **One profile, and it is a toy.** | The fixture profile's executor is a switch over eight opcodes. It is the right vehicle for measuring what the *core* adds - it adds almost nothing of its own to attribute away - and the wrong one for predicting a real language. |
| **Not a CI lane.** | These are collected by hand into an evidence bundle, not tracked per commit. Regression detection between milestones is by comparing bundles. |
| **No persisted-envelope figures.** | ADR 0010 decision 4 admits the persisted envelope as contract and not as a release feature, and release 1 exposes no envelope member. There is nothing to measure. Exclusion EX-25. The gate's "envelope read and write" is answered for the **operation-result** envelope instead: `envelope-read` measures the projection, and `diagnostics-capture` measures what the core fills into an envelope beyond the payload reference. |
| **A/A is a floor, not an interval.** | The harness reports whether an effect exceeds this machine's noise, not a confidence interval. It has seven repetitions and no distributional model, and claiming one would be arithmetic dressed as statistics. |
