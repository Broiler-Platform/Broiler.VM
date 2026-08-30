# Evidence bundle VM-6-001

**Milestone:** VM-6 — package, publish, and continuously recertify the core
**Collected:** 2026-08-30
**Core contract version:** 1
**Status of the milestone after this collection:** In progress, unaccepted — **and one gate clause
cannot be met by anyone who wrote this.**

This bundle records what was run and what happened. It does not accept a milestone.

**VM-6's gate contains a clause no amount of engineering closes.** It asks that "notices and
reviews are complete". The notices are complete. The review is not, and cannot be: `HUMAN_REVIEW.md`
is unsigned and `PENDING`, every one of its eight area verdicts is unset, and the person who would
sign it is the person who wrote the work. ADR 0001 states the binding rule — **no Broiler.VM
package is published without a completed review naming the reviewed commit** — and this bundle does
not change that. Everything below is what a reviewer would read; none of it is a substitute for one.

That is not a footnote. It is the honest headline of a milestone whose subject is *release*, and
putting it anywhere else in this document would be the untruthful support claim roadmap section 16
makes a stop condition.

---

## Field coverage

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
| Evidence bundle ID | VM-6-001 |
| Milestone | VM-6 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-6 gate and section 15's release gates |
| Core contract version | 1, unchanged. VM-6 adds no public member and mints no amendment — and now enumerates the surface so that claim is checkable |
| Reason-registry revision | 2, unchanged |
| Package version | 0.1.0-preview.1, with 0.1.0-preview.2 packed alongside it for the rollback evidence |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set |

---

## 2. Source

Collected with a **dirty working tree**: this bundle's own record set is uncommitted at collection
time.

**VM-6 changes no product assembly.** Not one line of `Broiler.VM.Abstractions`,
`Broiler.VM.Binary` or `Broiler.VM.Runtime` differs from VM-5, which is what a packaging milestone
ought to be able to say and the first milestone since VM-3 that can.

What it adds:

| Addition | What it is |
|---|---|
| `docs/api/public-api.txt` | The frozen public surface, 1,251 lines, enumerated rather than asserted about |
| `samples/Broiler.VM.Sample.FeedConsumer` | A whole profile written against the packages, restored from a feed, with no project reference |
| `docs/support.md` | The public support table |
| `THIRD_PARTY_NOTICES.md` | The notices and the dependency review |
| `.github/workflows/broiler-vm.yml` | CI, which has never run |
| Rule group **M**, rule **A14** | The API baseline, and the rule that keeps narrowing group A honest |
| Rules **C1**, **C2**, **C3** | Promoted from Deferred; their subject exists for the first time |

---

## 3. Dependencies and corpus

The VM-2 corpus is unchanged and is replayed in all three publish modes.

**The dependency review is in `THIRD_PARTY_NOTICES.md` and its result is worth stating here:** the
three packages carry no third-party code and no third-party package dependency.
`Broiler.VM.Runtime` depends on the other two and on nothing else. Five `PackageReference`s exist
in the whole repository and every one is development-time.

**A claim in this area was wrong and a rule caught it.** The first draft of the support table and
the notices said the packages "declare no package dependency at all". They do not —
`Broiler.VM.Runtime` declares the other two, which is the entire shape of the graph — and rule C2,
reading the produced `.nuspec` files, failed on the assertion written beside that prose. It is
recorded rather than quietly corrected because an untruthful support claim is a stop condition, and
this one was the author's.

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
| Lane | **None. The CI workflow this milestone adds has never executed.** |

---

## 5. Procedure

Produced by `python eng/collect-evidence.py --bundle VM-6-001 --out docs/evidence/vm-6`.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack`, and **every produced `.nuspec` extracted** | `pack.log`, `nuspecs.txt` |
| 4-6 | Publish and run the fixtures host under JIT, trimming and Native AOT | `publish-jit-and-trimmed.log`, `publish-aot.log` |
| 7 | Replay the corpus from each of the three modes and compare the tables | `corpus-replay.log` |
| 8 | Eight seeded fuzz sessions of 250,000 iterations each | `fuzz.log` |
| 8b | Publish and run each composition in three modes; compare catalogs; list closures | `composition-*.log`, `catalog-*.txt`, `closure-*.txt` |
| 8c | The soak run | `soak.log` |
| 8d | The baselines on both lanes (retained unless `--rebench`) | `bench.log` |
| 8e | **Pack two versions to a feed, restore a consumer with no project reference, run it, roll back, publish it Native AOT** | `feed-consumer.log` |
| 9 | **Nineteen** negative controls, each injected, run, reverted, and run again | `negative-control.log` |
| 10 | Environment and hashes | `environment.txt`, `hashes.txt` |

---

## 6. Outputs

**Build.** Fourteen projects build Release with 0 warnings and 0 errors. The fifteenth project file
in the checkout is the sample, which is outside the solution by construction and builds separately.

**Tests.** 318 tests pass, 0 failed, 0 skipped — 121 architecture and 197 behavioural. VM-6 adds
fourteen architecture tests and no behavioural test, which is the right shape for a milestone that
changes no product code.

**Pack.** Exactly three `.nupkg` and three `.snupkg`. Rule C1 asserts it against the retained log
and the CI workflow asserts it again against a pack it runs itself, so the two can disagree and the
day they do is the day the bundle went stale.

**Publish and run.** The fixtures host publishes and runs in all three modes; the trimmed image is
78256 bytes and the Native AOT image is 1565576 bytes, both unchanged from VM-5 — which follows
from VM-6 changing no product code and is the check on that claim.

### 6.1 The pristine feed consumer

The gate's load-bearing clause: *every advertised package restores from a feed without repository
project references, and samples use public APIs only.*

`samples/Broiler.VM.Sample.FeedConsumer` has three `PackageReference`s and no `ProjectReference`.
Its `NuGet.config` clears every source and adds back one: a directory of `.nupkg` files.
**nuget.org is not reachable from it.** Its `samples/Directory.Build.props` is deliberately empty,
so it inherits none of the component's build properties — a sample built under our own properties
would prove that our packages work inside our build, which is not the claim.

It is not a compile test. It defines a **whole profile** against the public contracts alone —
format, verifier with a bounded reader and a metering adapter, executor charging Fuel per step,
payload projection with an identity check — and runs four checks:

| Check | Result |
|---|---|
| A verified artifact instantiates, invokes and returns a typed value | ok — 40 plus 2 steps = 42 |
| Bytes of another format are refused before anything runs | ok — `InvalidArtifact/MalformedEncoding` |
| An unknown format version is refused deterministically, twice identically | ok — `InvalidArtifact/UnsupportedProfileFormatVersion` |
| A host ceiling bounds the work a profile may do | ok — `ResourceExhaustion/AllowanceExhausted (Fuel/Runtime)` |

Published with trim and AOT warnings as errors it becomes a single self-contained file of
**2,048,520 bytes**, and that file runs all four checks. That is gate 5 and gate 6 of section 15
met from a consumer's position rather than from the repository's. (It is the sample's own image and
not the fixtures host's, whose size is in section 6 above and in `publish-aot.log`.)

**The fourth check is the one worth reading.** Its first version asked for a million steps and never
reached the executor: a declared count of a million is past the profile's own `DeclaredCount` bound,
so the artifact was refused during *verification*. That refusal is correct and is a different
claim — a hostile count costs nothing proportional to the number an attacker wrote down. Seeing the
Fuel bound required a count the verifier accepts, which is what makes the two bounds visibly
separate rather than one bound wearing two names.

### 6.2 Rollback, exercised

Two package sets are packed to one feed. The consumer restores `0.1.0-preview.2`, runs, is rolled
back to `0.1.0-preview.1`, and runs again — and it prints the **informational version of each
assembly it actually loaded**, so the transcript shows which set answered rather than which one was
asked for. Both versions carry the same SourceLink commit stamp, which is what makes them a rollback
of packaging rather than of code.

### 6.3 The API baseline

1,251 lines enumerating every exported type and every public and protected member of the three
packable assemblies, with constants carrying their values.

Group V has fixed named *properties* of the surface since VM-1 and none of them is a claim about
what is there. A member added tomorrow that breaks no V rule was an addition nothing noticed; a
member removed tomorrow was a breaking change nothing noticed. Rule **M1** compares the file against
the built assemblies in both directions, so both are failures now, and a signature change is both at
once.

It is regenerated by `BROILER_API_WRITE=1`, deliberately: a baseline that regenerated itself would
agree with every change. The diff is what a reviewer reads, and a reviewer who regenerates without
reading has defeated it — EX-99.

### 6.4 What group A had to give up, and what replaced it

The feed consumer carries a `PackageReference` to all three Broiler.VM packages, which **rule A2
forbids**. It cannot be a solution project, because restoring it requires a pack to have already
happened.

The rules are not weakened to admit it. `ComponentGraph.Projects` now reads `Broiler.VM.slnx`
instead of globbing the tree, narrowing group A's subject to what it always meant — the frozen
graph. Narrowing a rule's subject is how rules quietly stop covering things, so rule **A14** is the
complement: every project file the solution does not list sits under `samples/`, carries **no**
`ProjectReference`, and references exactly the three packages. A project in neither place would be
governed by nothing, which is worse than being disallowed.

### 6.5 The negative controls

Nineteen, each injected, run, reverted and run again. Two are new at VM-6 and each pins one of this
milestone's own claims: a public type added without the API baseline being regenerated, which rule
M1 must see as an addition; and a project reference from the pristine feed consumer back into the
repository, which rule A14 must refuse — because a sample that reached into the build would be a
consumer of the build rather than of the packages, and the feed claim with it.

---

## 7. Decision

The VM-6 gate and section 15's release gates, clause by clause. Marks are evidence verdicts set by
the author; no reviewer has set anything.

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[MET]` | Every advertised package restores from a feed without repository project references | Section 6.1. Three `PackageReference`s, no `ProjectReference`, one restore source, nuget.org unreachable |
| `[MET]` | Samples use public APIs only | The sample defines a whole profile against the public contracts. Rule A14 asserts it carries no project reference |
| `[MET]` | The public API matches the baseline | Rule M1, in both directions, over 1,251 enumerated members |
| `[MET]` | The package graph matches the baseline | Rules C1, C2 and C3 against the retained pack log and `.nuspec` files; rule A6 fixes the packable set at three |
| `[MET]` | All malformed-input and contract suites pass | 318 tests, 0 failed. The VM-2 corpus replays identically in all three publish modes |
| `[PART]` | **Every claimed RID publishes and runs the declared compositions with warnings as errors** | On `linux-x64`: both compositions and the sample, JIT, trimmed and Native AOT, warnings as errors, all run. **No other RID is claimed**, so the clause is met for the claimed matrix and the matrix is one entry. `docs/support.md` section 3 says so rather than implying breadth |
| `[PART]` | **Notices and reviews are complete** | Notices are complete: `THIRD_PARTY_NOTICES.md`, with the dependency review and its limits. **The review is not and cannot be** — see this document's header |
| `[MET]` | Rollback is tested | Section 6.2. Exercised, not described |
| `[MET]` | Recertification triggers are documented | Section 8 here, section 6 of `docs/support.md`, and per-bundle triggers in every earlier bundle |
| `[MET]` | The support table states the core contract version and that the core ships no language profile | `docs/support.md`, first block quote and first line |
| `[MET]` | Graph, catalog, AOT and contract drift checks wired into CI | `.github/workflows/broiler-vm.yml`. The drift rules live in `dotnet test`, which the workflow runs; the feed restore, the rollback and the per-RID AOT publish are what the workflow adds |
| `[UNMET]` | **Wired into REQUIRED CI** | A workflow file is not a required check. No branch protection exists, no run has happened, and nothing prevents a merge today. EX-102 |
| `[MET]` | Measurement honesty: core overhead published with its method, no language performance claimed | `docs/baselines.md`, and the disclaimer in the gate, the register, the support table and the bench host |

### 7.1 Deviations recorded rather than amended

| Deviation | Why it is an erratum |
|---|---|
| The checkout has fifteen `.csproj` files and the frozen graph has fourteen | The sample cannot be a solution project. ADR 0001 revision 4 authorises `samples/` as the one place outside the graph, and rule A14 stops that becoming a hiding place. EX-103 |
| The package boundary is unchanged from VM-0's hypothesis | "Finalize only the boundaries justified by VM-0 evidence" is satisfied by leaving them alone: no evidence in six milestones argued for a fourth package or a merge. Recorded because "finalized" and "unchanged" look identical in a table and are different decisions |

---

## 8. Validity

**Reproduction.** `python eng/collect-evidence.py --bundle VM-6-001 --out docs/evidence/vm-6`.

**Expiry.** The figures here are true of the logs as retained. Rules H5, L1, C1, C2, C3, K3 and K4
hold quoted figures and baselines to those logs and cannot hold the logs to the checkout — EX-54.

**Recertification triggers.** Any of these invalidates this bundle:

- a change to any file in `hashes.txt`;
- **any change to `docs/api/public-api.txt`**, which is a public API change by definition and the
  one trigger this milestone adds;
- a change to the core contract version, the reason-registry revision, or the package graph;
- a change to the host capability surface, the Native AOT settings, the RID matrix, the resource
  defaults, or the representative workload — the list roadmap section 15 fixes;
- an SDK change, since none is pinned;
- for the figures in `docs/baselines.md`, any change of machine at all.

---

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. `environment.txt` records what this machine resolved |
| EX-06 | **Closed** | **Closed 2026-08-30 by this bundle.** VM-0 recorded that the component ran no CI of its own. `.github/workflows/broiler-vm.yml` exists. What the closure does **not** cover is recorded as EX-102: the workflow has never run and is not a required check |
| EX-25 | Open | **The persisted envelope is admitted by the contract and implemented by no milestone.** Unchanged |
| EX-42 | Open | The Native AOT publish on `win-x64` needs a `vcvars64` environment. `docs/support.md` claims no `win-x64` support |
| EX-45 | Open | **One RID, one machine, one lane.** The support table's RID row is one entry because of it |
| EX-52 | Open | **Twenty-nine review findings of major and minor severity remain unaddressed.** VM-6 did not work that list |
| EX-54 | Open | Rules H5, L1 and the pack rules check document against log, not log against checkout |
| EX-78 | Open | **The guest-load nesting-depth bound is unreachable at contract version 1.** Unchanged |
| EX-79 | Open | **The corpus retains no minimized regression.** Unchanged |
| EX-80 | Open | **The fuzz session varies the payload and never the descriptor.** It also does not fuzz the sample profile |
| EX-81 | Open | **Rule H5 admits a figure from any bundle a document links.** Unchanged |
| EX-82 | Open | **A verification that fails records no clamp.** Unchanged |
| EX-83 | Open | **A refused budget override populates a diagnostics group ADR 0005 annotates for a different category.** Unchanged |
| EX-84 | Open | **The cross-mode table is produced under one descriptor.** Unchanged |
| EX-85 | Open | **`DescriptorMismatch` is unreachable through the public path.** The sample's verifier has a `DescriptorMismatch` path and it is unreachable there too: the core refuses a descriptor outside the profile's declared format range first. A fourth profile, same result |
| EX-86 | Open | **Rules K3 and K4 compare against the last collection, not the working tree.** Unchanged |
| EX-87 | Open | **The closure report excludes framework assemblies by name prefix.** Unchanged |
| EX-88 | Open | **Every concurrency result was collected on four processors.** Unchanged |
| EX-89 | Open | **Thread affinity is enforced where the core can see a thread, and nowhere else.** Unchanged |
| EX-90 | Open | **The in-capability flag is call-stack scoped, where ADR 0011 F5 says per-runtime.** Unchanged |
| EX-91 | Open | **A refused capability re-entry answers a more specific reason than ADR 0011 F5 names.** Unchanged |
| EX-92 | Open | **`VmReason.UnwindTimedOut` cannot be produced at contract version 1.** Unchanged |
| EX-93 | Open | **`ForeignOpaqueRef` and `StaleOpaqueRef` cannot be produced at contract version 1.** Unchanged |
| EX-94 | Open | **ADR 0001 owns rule groups L and M nominally and names no rule of either.** Revision 4 authorises the projects and the baselines those rules bind; it does not name the rules. Closed by: a revision naming them, or a decision that a register rule needs no owning ADR |
| EX-95 | Open | **Only the baseline register's measurement table is bound to the log.** Unchanged |
| EX-96 | Open | **No persisted-envelope figure exists because no persisted envelope exists.** Unchanged |
| EX-97 | Open | **The guest-load lane rebuilds a runtime inside its timed region.** Unchanged |
| EX-98 | Open | **The benchmark is re-measured deliberately rather than on every collection.** Unchanged |
| EX-99 | Open | **The API baseline is regenerated by an environment variable, so a reviewer who regenerates without reading has defeated rule M1.** The alternative - a baseline only editable by hand - is edited by hand wrongly, which is why the Code Assurance generator makes the same trade. What makes it survivable is that the diff is small, mechanical and reviewable, and that any change to it is a recertification trigger. Closed by: a review process that requires the API diff to be quoted in the change that makes it |
| EX-100 | Open | **Rule M1 compares text, not semantics.** A change that leaves every signature identical is invisible to it: a default parameter value, an attribute, a nullability annotation that does not reach the signature string, a documented behaviour. Those are real breaking changes and no rule here sees them. Closed by: a binary-compatibility tool, which is a dependency this component does not have |
| EX-101 | Open | **The pack rules read the last collection, not the working tree.** Rules C1, C2 and C3 assert against `pack.log` and `nuspecs.txt` as retained. A pack that changed since would agree with a stale bundle. The CI workflow packs and counts independently, which narrows it but does not close it: CI has never run. Closed by: the same thing that closes EX-86 |
| EX-102 | Open | **CI exists, has never run, and is not required.** The workflow was written against a repository with no branch protection, no hosted runner has executed it, and its RID matrix is aspirational for every entry except `linux-x64`. Nothing in `docs/support.md` claims a platform on the strength of it. A workflow that has not run is a plan. Closed by: a run on a hosted runner, and a required-check setting on the default branch |
| EX-103 | Open | **The checkout carries one project file the frozen graph does not.** The pristine feed consumer cannot be a solution project. Rule A14 asserts every unlisted project sits under `samples/`, carries no project reference and references exactly the three packages, so the exemption cannot widen silently - but a reader counting `.csproj` files finds fifteen where `graph.manifest.json` describes fourteen. Closed by: nothing; it is the price of proving the packages work from outside the build |
