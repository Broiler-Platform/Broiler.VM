# Evidence bundle VM-5-001

**Milestone:** VM-5 — baseline the core's own overhead
**Collected:** 2026-08-29
**Core contract version:** 1
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition. Under update
rule 8 the work could be built and landed without that decision; it could not be released, and
nothing here is a release.

> **No language performance claim follows from any figure in this bundle.** Every measurement is of
> the **core's own overhead** around a fixture profile whose executor is a toy stack machine. A real
> language profile's cost is its own, and nothing here predicts it.

**VM-5 was supposed to change nothing and it changed the product assemblies twice.** A milestone
whose subject is measurement found two defects that eleven milestones of behavioural testing had
not, because a baseline asks a question no test asks: *how much does this cost, and does the answer
depend on anything it should not?*

| What was wrong | What the record said | How measuring found it |
|---|---|---|
| The guest-load mediator **never reset its per-operation counters**, so fan-out, cumulative nested bytes and nested verifier work were lifetime bounds on a mediator shared by every instance of one profile in one runtime | ADR 0008 and roadmap section 13: nested loads are bounded in depth and fan-out **per requesting operation**, charged to the operation that made them | A probe asked how many mediated loads one runtime admits, expected thousands, and printed **8** |
| A runtime's capability depth lived in an `AsyncLocal<int>`, whose entry **can never be released** — returning the depth to zero stores a boxed zero, which is a present value | ADR 0004: disposal is bounded and leaves nothing behind | The same instantiate-and-invoke allocated 9,960 bytes early in a run and 1,188,872 bytes after seventy thousand runtimes |

Neither is a performance finding VM-5 chose to act on. Both are **correctness** findings: a bound
that does not bound what it says, and a resource that is never released. Each was proven by a test
that fails before the fix and passes after it.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry.

| Field | Where |
|---|---|
| Identity | Section 1 |
| Source | Section 2 |
| Dependencies and corpus | Section 3 |
| Environment | Section 4 |
| Procedure | Section 5 |
| Outputs | Section 6 |
| Decision | Section 7 |
| Validity | Section 8 |

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-5-001 |
| Milestone | VM-5 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-5 gate |
| Core contract version | 1, unchanged. VM-5 adds no public member and mints no amendment. |
| Reason-registry revision | 2, unchanged. |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

---

## 2. Source

Collected with a **dirty working tree**: this bundle's own record set is uncommitted at collection
time.

What VM-5 changed in the product assemblies, in full:

| File | Change |
|---|---|
| `VmArtifactLoadMediator.cs` | `EnterScope` takes the operation's identity and there is no longer an overload that omits it |
| `VmInstanceImplementation.cs` | Both step entry points pass the operation's identity to the mediator |
| `VmInstantiation.cs` | Instantiation mints an identity for the mediator, having none of its own |
| `VmRuntime.cs` | The capability depth is held as a boxed value so that zero is the **absence** of an entry, which is what releases it |

What VM-5 added outside them: one test-only project (`Broiler.VM.Bench.Host`, authorised by ADR 0001
revision 3), the baseline register `docs/baselines.md`, rule group L with six witnesses, three
behavioural regression tests, and three negative controls.

---

## 3. Dependencies and corpus

The VM-2 corpus is unchanged and is replayed here in all three publish modes. VM-5 adds no corpus:
its subject is not what the core does with bytes but what the core costs, and the instrument for
that is a harness with controls rather than a file of inputs.

**The benchmark host references no benchmarking package**, and ADR 0001 revision 3 records why: a
framework's own warmup, pilot and outlier policies would be part of every number this component
publishes and none of them would be visible in this repository. The apparatus is a few hundred
lines a reader can check.

---

## 4. Environment

| Field | Value |
|---|---|
| OS | Linux 6.18.44, glibc 2.39 |
| Architecture | x86_64 |
| RID | `linux-x64` |
| Processors | 4 |
| Configuration | Release |
| Publish modes | JIT, trimmed self-contained, Native AOT |
| GC | Workstation, non-concurrent; tiered compilation off in the bench host |
| Lane | None. One machine, one RID, no CI. |

**Every figure in section 6.1 is a property of this machine.** The ratios between them travel; the
absolute values do not. That is exclusion EX-45 and it applies to the baselines with more force
than to anything else in this repository, because a number invites a comparison a pass/fail result
does not.

---

## 5. Procedure

Everything below was produced by `python eng/collect-evidence.py --bundle VM-5-001 --out
docs/evidence/vm-5`, run from the component root.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4-6 | Publish and run the fixtures host under JIT, trimming and Native AOT | `publish-jit-and-trimmed.log`, `publish-aot.log` |
| 7 | Replay the corpus from each of the three modes and compare the tables | `corpus-replay.log` |
| 8 | Eight seeded fuzz sessions of 250,000 iterations each | `fuzz.log` |
| 8b | Publish and run each composition in three modes; compare catalogs; list closures | `composition-*.log`, `catalog-*.txt`, `closure-*.txt` |
| 8c | The soak run: 400,000 lifecycle cycles across four workers, sampled | `soak.log` |
| 8d | **The baselines: ten measurements on the JIT lane and on the Native AOT lane.** Retained rather than re-measured unless `--rebench` is passed - see below | `bench.log` |
| 9 | Seventeen negative controls, each injected, run, reverted, and run again | `negative-control.log` |
| 10 | Environment and hashes | `environment.txt`, `hashes.txt` |

**Step 8d does not re-measure by default, and that default is load-bearing.** Rule L1 binds
`docs/baselines.md` to `bench.log` by value, and a benchmark - unlike a test count or an image
size - produces different numbers every run. A collection that re-measured every time would leave
the register permanently one collection behind its own log, with no state in which both are true.
So the figures move when someone decides to move them and the register moves in the same change.
`--rebench` re-measures. The log in this bundle is the run that produced the figures in the
register; the other logs here are from the collection that retained it.

---

## 6. Outputs

**Build.** Fourteen projects build Release with 0 warnings and 0 errors.

**Tests.** 304 tests pass, 0 failed, 0 skipped — 107 architecture and 197 behavioural. VM-4 retained
293; VM-5 adds three behavioural tests and eight architecture tests, which is the shape of a
milestone that mints one rule and finds two defects.

**Pack.** Exactly three `.nupkg` and three `.snupkg`. None of the nine test-only projects packs and
neither composition root does.

**Publish and run.** The fixtures host publishes and runs in all three modes. The trimmed image is
78256 bytes, unchanged; the Native AOT image is 1565576 bytes, 16 bytes smaller than VM-4's.

**Both compositions** still publish and run in all three modes and pass their twelve checks, and
their closures are unchanged: five non-framework assemblies for the single-profile composition and
six for the two-profile one.

**The soak** is unchanged from VM-4 and still plateaus.

**Fuzz.** Eight sessions, 250,000 iterations each, no counterexample.

### 6.1 The baselines

Ten measurements, on two lanes, every one valid — the A/A lane inside its effect in all twenty
cases. `docs/baselines.md` is the register: it carries every figure, the control each is attributed
against, and what each one means. Rule **L1** holds the register and `bench.log` to each other in
both directions, so a figure nothing measured cannot be published and a measurement nobody declared
cannot appear.

The four worth reading here:

| Figure | JIT | AOT | What it says |
|---|---|---|---|
| `meter-per-instruction` | 98.36 ns | 94.77 ns | **Metering is the cost of executing bytecode in this core**, by an order of magnitude over any other per-operation figure. A profile of cheap instructions will not amortise it. |
| `verify-throughput` | 186.18 ns/byte | 189.97 ns/byte | Not a decoder figure. Every byte consumed is charged through two interface calls into the core meter, each taking a lock and walking four budget scopes; the per-byte number is the metering discipline. |
| `guest-load-mediation` | 1614.67 ns | 1434.33 ns | What routing a load through the mediator adds **over performing the same nested verification directly**. The nested verification is in both lanes and cancels. |
| `startup first-verification-ms` | 137.4 | 9.7 | Process start to the first verified artifact. The AOT figure is the one a host waits for; the JIT figure includes the SDK host that launched it. |

**What the discipline caught.** The harness checks that each measurement's operation still does what
its name says **before and after every timed lane**, and that check found three lanes timing
something other than what they were named for, each producing a plausible, stable, entirely wrong
figure: a verification refused for want of allowance, a terminal instance out of Fuel, and a
fan-out refusal. The first two are properties of budgets being totals rather than per-operation
limits, and are fixed by resetting a lane's runtime outside the timed region. The third was the
product defect above.

**What is recorded rather than measured.** Image sizes, the startup figure, the resident-set
plateau, the guest-load headroom and the independence check are single observations with no control
to attribute them against, and the register says so rather than dressing an observation up as an
experiment. The fan-out series is likewise recorded: it shows the cost of a mediated load rising
with the number of loads the same operation has already made, which is why the measurement is
stated per single load and which is the one thing here that funds future optimisation work.

### 6.2 What the two defects cost, in figures

The capability-depth defect is worth stating numerically, because its severity is invisible in any
single measurement:

| After | An instantiate-and-invoke | One async-local write |
|---|---|---|
| 231 runtimes | 9,960 bytes | 72 bytes |
| 70,547 runtimes | 1,188,872 bytes | 393,072 bytes |

The benchmark itself took **528 seconds** before the fix and **43** after it. Nothing observable
failed at any point: what grew was the cost of every async-local write on the thread, because each
one copies the whole execution-context map and the map gained an entry per runtime that nothing
ever released. The bench host now carries an `independence` check — the same work sampled at the
start of a run and again at the end — which fails the run if per-operation cost grows with the
number of runtimes created.

### 6.3 The negative controls

Seventeen, each injected, run, reverted and run again. Three are new at VM-5 and each pins one of
this milestone's own corrections: a mediator that stops resetting its per-operation counters, a
runtime that stores a capability depth of zero instead of releasing it, and a baseline register
that quotes a figure the retained log contradicts.

---

## 7. Decision

The VM-5 gate, clause by clause. The marks are evidence verdicts set by the author about what the
retained evidence shows; no reviewer has set anything.

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[MET]` | Each measurement has a **predeclared rule** | `docs/baselines.md` section 1 fixes five rules that apply to all ten, and section 2 states each measurement's unit, candidate and control. The rules are properties of the harness rather than of any row, so no measurement can be given its own. |
| `[MET]` | Each measurement has a **comparable control** | Every control is the same workload with the core's part removed - the same bytes without a verifier, the same dispatch loop with charging off, the same artifact with a push where the host call was. Where the two artifacts differ they are matched in instruction count and byte length, which an earlier version was not: a control one instruction shorter attributed a dispatch iteration to the host call and inflated it by a fifth. |
| `[MET]` | Each measurement has an **A/A lane validity check** | The candidate runs twice, identically, and the difference is the noise floor. All twenty lanes are inside their effect. The check is applied twice - by the harness for the reader, and independently over the same measurements for the exit code - so a formatting bug cannot decide whether a lane was valid. |
| `[MET]` | **Retained repetitions** | Seven per lane, every one printed in `bench.log`, no outlier policy and no statistical model. |
| `[MET]` | Verification throughput per byte, catalog construction and lookup, runtime creation and disposal, budget metering per operation and per host call, diagnostics capture, startup | Eight of the ten measurements, on both lanes. |
| `[MET]` | Guest-initiated-load mediation | `guest-load-mediation`, with a control that performs the same nested verification directly so the difference is the mediation and not the load. |
| `[PART]` | Envelope read and write | Answered for the **operation-result** envelope: `envelope-read` measures the projection a consumer performs, and `diagnostics-capture` measures what the core fills into an envelope beyond the payload reference. The **persisted** envelope is admitted by contract and implemented by no milestone, so there is nothing to measure - EX-25, restated as EX-96. |
| `[MET]` | Image size | Recorded on both lanes and for the three packable assemblies separately. Package sizes are not here: packages are produced at VM-6 and a size for a package this component does not build would be an invention. |
| `[MET]` | Resident-set plateau | Six rounds of 4,000 whole lifecycles on each lane; the last round no larger than the second by more than a sixteenth, on both. Held on both lanes. |
| `[MET]` | Measured on JIT and Native AOT with the fixture profile | Both lanes, all ten measurements, in one log. |
| `[MET]` | The core publishes what it costs so a profile can budget against it | `docs/baselines.md`, bound to the log by rule L1 in both directions. |
| `[MET]` | States plainly that no language performance claim follows | In the gate, in the register's opening block quote, in this bundle's header, and in the benchmark host's own documentation. |
| `[MET]` | Optimization is funded only against one of these baselines | No optimisation was performed. The two product changes are correctness fixes with behavioural witnesses, not performance work, and the fan-out series is published as the thing a future optimisation would be funded against. |

### 7.1 Deviations recorded rather than amended

| Deviation | Why it is an erratum |
|---|---|
| `guest-load-mediation` runs one load per operation where every other invocation lane runs thousands of iterations against one runtime | Forced by a bound rather than chosen: fan-out is charged at runtime scope as a lifetime total and the fixture profile's hard maximum is 64, which no host may raise. The runtime is rebuilt inside the timed region and the control rebuilds one too, so the fixture cost cancels. Exclusion EX-97. |
| The register's section 3 recorded figures are quoted from the log and bound by nothing | Each is a single observation with no unit and no per-operation figure to compare against, so L1 has nothing to compare. Exclusion EX-95. |

---

## 8. Validity

**Reproduction.** `python eng/collect-evidence.py --bundle VM-5-001 --out docs/evidence/vm-5`.

**Expiry.** The figures here are true of the logs as retained. Rules H5 and L1 hold every quoted
figure to those logs and cannot hold the logs to the checkout. Exclusion EX-54 records that.

**A benchmark expires faster than a test result.** A passing test is true of the code; a figure is
true of the code *and* the machine, the SDK, the collector's mood and whatever else was running.
The recertification triggers below are therefore wider than any earlier bundle's.

**Recertification triggers.** Any of these invalidates this bundle:

- a change to any file in `hashes.txt`;
- a change to the core contract version or the reason-registry revision;
- a change to the metering path, the bounded reader, or the guest-load mediator, which are what
  three of the ten measurements are dominated by;
- **any change of machine at all** for the figures in section 6.1, not merely a materially
  different one: these are absolute times, and EX-45 covers what that means;
- an SDK change, since none is pinned, and a JIT or AOT codegen change moves every figure here.

---

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. `environment.txt` records what this machine resolved. |
| EX-25 | Open | **The persisted envelope is admitted by the contract and implemented by no milestone.** Unchanged. |
| EX-42 | Open | The Native AOT publish on `win-x64` needs a `vcvars64` environment. It did not apply to this collection. |
| EX-45 | Open | **One RID, one machine, one lane.** It binds hardest here: every figure in section 6.1 is an absolute time on one four-processor machine. |
| EX-52 | Open | **Twenty-nine review findings of major and minor severity remain unaddressed.** VM-5 did not work that list. |
| EX-54 | Open | Rules H5 and L1 check document against log, not log against checkout. |
| EX-78 | Open | **The guest-load nesting-depth bound is unreachable at contract version 1.** Unchanged. |
| EX-79 | Open | **The corpus retains no minimized regression, because no session has found one.** Unchanged. |
| EX-80 | Open | **The fuzz session varies the payload and never the descriptor**, and fuzzes no consumer profile. Unchanged. |
| EX-81 | Open | **Rule H5 admits a figure from any bundle a document links.** Unchanged. |
| EX-82 | Open | **A verification that fails records no clamp.** Unchanged. |
| EX-83 | Open | **A refused budget override populates a diagnostics group ADR 0005 annotates for a different category.** Unchanged. |
| EX-84 | Open | **The cross-mode table is produced under one descriptor.** Unchanged. |
| EX-85 | Open | **`DescriptorMismatch` is unreachable through the public path** for all three profiles. Unchanged. |
| EX-86 | Open | **Rules K3 and K4 compare against the last collection, not against the working tree.** Unchanged. |
| EX-87 | Open | **The closure report excludes framework assemblies by name prefix.** Unchanged. |
| EX-88 | Open | **Every concurrency result was collected on four processors.** Unchanged, and it now also bounds what the baselines mean: four processors with workstation GC is a desktop-shaped machine and not a server-shaped one. |
| EX-89 | Open | **Thread affinity is enforced where the core can see a thread, and nowhere else.** Unchanged. |
| EX-90 | Open | **The in-capability flag is call-stack scoped, where ADR 0011 F5 says per-runtime.** Unchanged. VM-5 changed how the depth is stored and not what it is scoped to. |
| EX-91 | Open | **A refused capability re-entry answers a more specific reason than ADR 0011 F5 names.** Unchanged. |
| EX-92 | Open | **`VmReason.UnwindTimedOut` cannot be produced at contract version 1.** Unchanged. |
| EX-93 | Open | **`ForeignOpaqueRef` and `StaleOpaqueRef` cannot be produced at contract version 1.** Unchanged. |
| EX-94 | Open | **ADR 0001 owns rule group L nominally and names no rule of it.** The ADR authorises the benchmark host whose output L1 binds; it is frozen and predates the rule. The same shape as EX-53 for group H. Closed by: a revision naming the rule, or a decision that a register rule needs no owning ADR. |
| EX-95 | Open | **Only the baseline register's measurement table is bound to the log.** Section 3's recorded figures - startup, image sizes, the plateau verdict, the guest-load headroom, the independence verdict - are quoted from the same log and checked by nobody, because each is a single observation with no unit and no per-operation figure for L1 to compare. A wrong one there is a typo nothing catches. Closed by: extending L1 with a phrasing table per recorded figure, which is what rule H5 does for the figures it binds. |
| EX-96 | Open | **No persisted-envelope figure exists because no persisted envelope exists.** ADR 0010 decision 4 admits it as contract and not as a release feature, and release 1 exposes no envelope member. The gate's "envelope read and write" is answered for the operation-result envelope instead, which is a different object with the same word in its name; that substitution is stated in section 7 rather than left to be inferred. Closed by: the persistence gate no milestone yet defines. |
| EX-98 | Open | **The benchmark is re-measured deliberately rather than on every collection.** Step 8d retains `bench.log` unless `--rebench` is passed, because rule L1 binds the register to it by value and a benchmark produces different numbers every run - so a collection that re-measured would leave the register and its own log permanently unable to agree. The cost is that `bench.log` can be older than the rest of the bundle: the other logs say the suite was green at collection time and the bench log says what the figures were when someone last chose to take them. The recertification triggers in section 8 are what bound that gap, and a reader comparing two bundles should check the hash of the bench host rather than the date of the collection. Closed by: a deterministic measurement, which no benchmark is. |
| EX-97 | Open | **The guest-load lane rebuilds a runtime inside its timed region.** Every other invocation lane holds one runtime across thousands of iterations; this one cannot, because fan-out is charged at runtime scope as a lifetime total whose profile maximum is 64 and no host may raise it. The control rebuilds a runtime too, so the fixture cost cancels in the difference - but the lane's absolute figures are a whole lifecycle rather than an invocation, and its A/A floor is correspondingly larger. Closed by: a fixture profile declaring a larger fan-out maximum, which would make this lane look like the others. |
