# Human Review: Broiler.VM

> **Status: PENDING.** No human reviewer has yet attested to this component. Until
> the attestation below is completed and signed, Broiler.VM must not be described as
> human-approved, and no package may be published from it.

## 1. How To Use This File

This section is the canonical mark legend for the component. Every other review
document links here rather than repeating the tables. There are two vocabularies, they
are different kinds of thing, and they must never be mixed. Both are closed sets.

### Evidence verdicts - set by the author, about evidence

| Mark | Meaning |
|---|---|
| `[MET]` | Demonstrated. An execution, artefact or log in a retained bundle shows it. |
| `[PART]` | Partly demonstrated. What is not shown is named on the same row. |
| `[UNMET]` | Not discharged. The condition is stated and not satisfied. |
| `[N/A]` | Not claimed at this milestone. The milestone that owns it is named. |

### Review verdicts - set by the reviewer, about their own reading

| Mark | Meaning |
|---|---|
| `[ ]` | Not yet reviewed. |
| `[A]` | Accepted as stated. |
| `[C]` | Accepted with a condition. The condition is recorded in the decision section. |
| `[R]` | Rejected. The defect is recorded. |
| `[?]` | Cannot be judged from what is here. What is missing is named. |

Every review verdict in this file and in the worksheet is `[ ]`. None has been
pre-filled, because nothing in this repository has been read by a human reviewer and a
filled mark would be a false record.

Section 9's decision list is not a review verdict and does not use this vocabulary. It
is one four-state choice - approved, approved with conditions, not approved, or pending
- written as ordinary markdown checkboxes. `PENDING` is checked there because pending is
the true current decision, which is the opposite of a pre-filled verdict.

**Identifiers.** Anything a reviewer might want to cite has a stable ID. `RA-n` is one
of the eight review areas, risk-ordered, listed in section 4 and given a verdict row in
section 8. `RC-nn` is a checklist item in the per-item worksheet at
[docs/review/vm-0-vm-1.md](docs/review/vm-0-vm-1.md). `AT-n` is an item flagged for the
reviewer's attention in section 6. `G-nn` is a gate clause in an evidence bundle, and
`EX-nn` is an exclusion; both live in the bundles under `docs/evidence/`. IDs are
stable, and an existing `EX-nn` is never renumbered.

**To record a verdict:** edit the mark in the area's row in section 8, add any condition
to section 10, and sign section 11.

## 2. When This Review Is Required

Ruled 2026-08-28 by the architecture and release owner, and recorded as update rule 8
in [the status ledger](docs/roadmap.status.md):

| Activity | Review |
|---|---|
| Implementing a milestone, landing it, collecting its evidence | **Optional.** Development proceeds against frozen records whether or not they are approved. |
| Publishing a package, claiming a RID, issuing a support table, moving a milestone to `Accepted` | **Mandatory.** None of them happens without a signature below for the revision in question. |

The split is deliberate and it is not a relaxation: it moves the gate to where the
irreversible act is. Nothing published, nothing claimed, and no milestone marked
complete without a human having read the revision. What it permits is building.

It also has a cost the owner should keep in view. Unreviewed work accumulates, and
review effort does not scale linearly with it: the VM-1 adversarial pass found
sixteen blocking contract violations in a single milestone, behind a suite that was
entirely green. Deferring review to the release makes that pile larger, not smaller.

This file exists so the review decision has somewhere to live and cannot be inferred
from anything else. It is deliberately unsigned:
[ADR 0001](docs/adr/0001-component-topology-and-dependency-graph.md) originally
deferred this file to VM-6 on the grounds that a template with unfilled fields invites
a false approval record, and that risk is real. Nothing here is an approval, and the
evidence section records what was collected, not what was reviewed.

## 3. Review Target

- **Component:** Broiler.VM
- **Scope:** Milestones VM-0 and VM-1 - the twelve boundary records under `docs/adr/`,
  the seven-project graph under `src/`, the architecture rules and their witnesses, the
  implementation of core contract version 1, the two test-only fixture profiles, the
  composition-root host, and the evidence bundles under `docs/evidence/vm-0/` and
  `docs/evidence/vm-1/`. There is no product language profile to review; none exists.
- **Release:** None. Broiler.VM has never been published and claims no RID.
- **Commit under review:** _to be recorded by the reviewer_
- **Reviewer:** MaiRat / Maik Ratzmer
- **Review date:** _not yet performed_

Any source change after the reviewed commit invalidates the approval until the changed
revision is reviewed again.

## 4. Review Route

The eight areas are ordered by risk, highest first. A reviewer who works down the table
and stops when the time runs out has still spent that time where a defect would cost
the most; the ordering exists so that stopping early is a defensible outcome rather than
a gap. The per-item checklist for each area is the worksheet at
[docs/review/vm-0-vm-1.md](docs/review/vm-0-vm-1.md); the times below are suggestions,
not budgets anyone is held to. The worksheet carries 35 items, numbered contiguously from
`RC-01`, with at least one for each of the eight areas; that count is stated here as well as
there so that neither document can shed an item on its own. It carried fifty-three until the
code-facing falsification criteria moved onto the declarations themselves, as a third
annotation line; the worksheet header records what moved and why, and each of the four code
areas keeps one item asking the reviewer to judge those criteria.

| ID | Area | What a defect here would cost | Suggested time | Where to start |
|---|---|---|---|---|
| RA-1 | Bounded reading of untrusted bytes | A memory-safety or denial-of-service defect, not a contract disagreement. `Broiler.VM.Binary` is the only code that touches attacker-controlled input. | 60 min | `src/Broiler.VM.Binary/VmBoundedReader.cs`, then `VmBoundedAllocator.cs` and `VmReadBounds.cs`; assertions in `src/tests/Broiler.VM.Contract.Tests/VerificationAndReaderTests.cs` |
| RA-2 | Resource authority and budgets | A budget that does not bound. This is where the adversarial review found the most blockers, including an aggregate live measure that could be driven to zero while memory was live. | 90 min | [ADR 0007](docs/adr/0007-resource-authority-and-budgets.md), then `src/Broiler.VM.Runtime/VmAggregateBudget.cs`, `VmMeter.cs`, `VmCeilingResolution.cs`; `src/tests/Broiler.VM.Contract.Tests/SuspensionAndBudgetTests.cs` |
| RA-3 | Lifecycle and state machine | An instance left usable after its stack was abandoned. Covers the mandatory outcome-to-state mapping, the precedence order, disposal and reentrancy. | 60 min | [ADR 0004](docs/adr/0004-lifecycle-and-state-machine.md) and [ADR 0005](docs/adr/0005-operation-result-envelope.md), then `src/Broiler.VM.Runtime/VmInstanceImplementation.cs`, `VmRuntimeState.cs`, `VmOperation.cs`; `LifecycleTests.cs` and `ReclamationTests.cs` |
| RA-4 | Verified-artifact ownership | Execution over bytes that were never verified, or over bytes a caller mutated after verification. Covers verification being separable from execution, the single construction site, leases and draining. | 45 min | [ADR 0006](docs/adr/0006-verified-artifact-ownership.md), then `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` and `src/Broiler.VM.Runtime/VmVerification.cs`; `VerificationAndReaderTests.cs` |
| RA-5 | Guest-initiated loads and external suspension | Mediation escaped, or an operation resumed under a spent parent. Covers mediation bounds, the deterministic no-provider refusal, the double gate and resume admission. | 45 min | [ADR 0008](docs/adr/0008-guest-initiated-loads.md) and [ADR 0009](docs/adr/0009-external-suspension-and-async-instantiation.md), then `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs` and `VmInstantiation.cs`, `src/Broiler.VM.Abstractions/VmGuestLoad.cs`; `GuestInitiatedLoadTests.cs` |
| RA-6 | The public contract surface | A frozen public name that has to change after publication. This is the hardest thing here to change later. Covers the frozen public-name table, the three errata, the stage matrix, and what a profile package can name. | 45 min | [ADR 0003](docs/adr/0003-core-contract-v1-and-amendments.md) and [ADR 0011](docs/adr/0011-source-level-profile-contract.md), then `src/Broiler.VM.Abstractions/VmCoreContract.cs`, `VmStageResults.cs`, `VmControlResult.cs`; the baseline in `src/tests/Broiler.VM.Architecture.Tests/ApiBaselineRules.cs` and the errata in the VM-1 bundle |
| RA-7 | The records themselves | A boundary record, invariant resolution or unapplied amendment that the implementation was built against is itself wrong. Covers the twelve boundary records, the four invariant resolutions, and the seventeen proposed and unapplied roadmap amendments. | 60 min | [docs/adr/README.md](docs/adr/README.md) and the twelve records beneath it; the amendment register in [ADR 0003](docs/adr/0003-core-contract-v1-and-amendments.md); [docs/roadmap.md](docs/roadmap.md) |
| RA-8 | The evidence and the rule register | Bundles that do not show what they claim, or a negative control that passes and is not treated as a finding about the suite. Covers what each register status means. | 45 min | [the VM-0 bundle](docs/evidence/vm-0/README.md) and [the VM-1 bundle](docs/evidence/vm-1/README.md) with their `negative-control.log`; `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` and `RuleRegisterTests.cs` |

## 5. Evidence Available To The Reviewer

Collected by automation and retained in
[the VM-0 evidence bundle](docs/evidence/vm-0/README.md),
[the VM-1 evidence bundle](docs/evidence/vm-1/README.md),
[the VM-2 evidence bundle](docs/evidence/vm-2/README.md),
[the VM-3 evidence bundle](docs/evidence/vm-3/README.md) and
[the VM-4 evidence bundle](docs/evidence/vm-4/README.md). It is input to a review, not
a substitute for one. Each line carries an evidence verdict as defined in section 1;
these are the author's marks about evidence, not review verdicts.

**Four milestones are now waiting on one reading.** This file was written for VM-0's records
and VM-1's implementation. VM-2 has since landed - the limit-precedence layers, a retained
malformed-input corpus, a fuzz target, and the guest-load bounds that were carried in a
descriptor and read nowhere - and VM-3 after it: two consumer profiles written against the
public contract alone, two named composition roots, and the register and closure reports that
say what each of them contains. VM-4 has landed after it, and it is the one a reviewer should
start with: it changed the product assemblies for the first time since VM-1, and it changed them
because four rules the records froze were being enforced nowhere. Nobody has read any of it. The
lines below say which milestone each figure belongs to rather than merging them, because a reviewer
needs to know what a number is about before it is worth anything.

- `[MET]` **Build and tests, as the tree stands:** `dotnet build Broiler.VM.slnx -c Release`
  completes with 0 warnings and 0 errors across thirteen projects; `dotnet test Broiler.VM.slnx
  -c Release` reports 293 tests passed, 0 failed, 0 skipped - 99 architecture and 194
  behavioural. At VM-3 the same commands reported 262 passed over twelve projects, of which 97
  architecture and 165 behavioural; VM-4 adds twenty-nine behavioural tests and two architecture
  rules, which is the shape of a milestone whose subject is behaviour under concurrency.
  **Read that number with section 5's VM-3 line: the two consumer profiles VM-3 added are
  referenced by no test project at all, by rule, so what exercises them is two published binaries
  and not the suite.**
- `[PART]` **An adversarial review has been run, and it found a great deal.** Six reviewers
  against the frozen records, every finding put to two independent refuters: 45
  findings survived, sixteen of them blockers, several confirmed by executing the
  code. All sixteen are corrected and regression-tested; twenty-nine majors and
  minors are recorded and unaddressed (EX-52). The first evidence bundle is
  superseded because it reported a green suite over a tree that contained all
  sixteen.
- `[MET]` **Negative controls:** four, each injected, run, reverted and re-run. One of the four
  did not fail on its first run; that episode is AT-4, and it is about what the suite proved
  rather than whether the control ran. A forbidden
  project edge fails A4 and A7; a deleted manifest edge fails A7; a struck name on the
  public surface fails V3; and removing the mediator's no-provider refusal fails the
  behavioural suite. Every run is retained in `negative-control.log`.
- `[PART]` **Native AOT:** the composition-root host publishes and **runs** as a 1,279,488-byte
  self-contained native binary on `win-x64`, and as a 162,816-byte trimmed binary,
  composing two fixture profiles through the generic contract with no trim or AOT
  warnings. It builds with `TreatWarningsAsErrors`. What is not shown is a reproduction
  by automation: see AT-8.
- `[MET]` **Packaging:** `dotnet pack` still produces exactly three `.nupkg` and three
  `.snupkg`. None of the seven test-only projects packs, and neither composition root does:
  the advertised composition set is empty at core contract version 1, and
  [the composition register](docs/compositions.md) says so in its first section.
- `[MET]` **Runtime dependencies:** the three product projects have no package references and
  no project references outside the component. Test packages are confined to the two
  test projects.
- `[PART]` **Security-sensitive behaviour:** this is the part that changed most, and it is where
  a reviewer's time is best spent. The product graph now decodes untrusted bytes. Every
  length, count, offset and allocation goes through the bounded reader and the
  allocation guard, which refuse before allocating; verification produces an immutable
  handle and execution consumes only that handle; a caller's buffer may be mutated
  afterwards without effect. There is still no file, network, process, native-interop,
  unsafe or code-execution path, and rule B5 asserts the absence of dynamic-loading
  APIs over compiled metadata. VM-2 adds the retained malformed-input corpus and the fuzz
  target that VM-1 recorded as absent: eighty-seven artifacts with their hashes and their
  expected answers, and two million seeded iterations that found nothing. What has still
  **not** been done is any concurrency testing, which is VM-4's.
- `[PART]` **VM-2's own boundary work.** The precedence algorithm's artifact-request clamp and
  its instance and invocation overrides are implemented, where VM-1 left them as a reason no
  code path could produce; the materialization ordering is asserted for every corpus artifact
  including every failing one; the caller's buffer is now overwritten concurrently as well as
  mutated; and the fourth guest-load bound is enforced. Two clauses of the VM-2 gate are
  `[PART]` rather than `[MET]` and the bundle names both: no fuzz session has found a
  regression to retain (EX-79), and the nesting-depth bound is unreachable at contract
  version 1 (EX-78).
- `[PART]` **VM-3's own composition work.** Two application-local consumer profiles were written
  against `Broiler.VM.Abstractions` and `Broiler.VM.Binary` and nothing else - no runtime
  reference, no `InternalsVisibleTo` in either direction, no package reference, no reflection -
  and composed by direct typed registration into two named roots that publish and run under JIT,
  trimming and Native AOT. The closure of each published image is listed off the published
  output: five non-framework assemblies for the single-profile composition, six for the
  two-profile one, differing by exactly the second profile. **No file under any of the three
  product project directories changed, and both published image sizes are byte-identical to
  VM-2's**, which is the gate's central clause in numeric form. It is `[PART]` rather than
  `[MET]` for one reason worth a reviewer's attention: neither consumer profile is fuzzed, and
  neither is reachable from the test suite, so everything true of them is demonstrated by two
  transcripts rather than by assertions a suite re-runs.
- `[PART]` **VM-4's corrections, and why they are where a reviewer should start.** Four rules that
  were frozen at VM-0, implemented at VM-1 and enforced nowhere are now enforced, and each was found
  by a test that could reach a second thread rather than by anyone reading the code. A running host
  capability refused every *other* thread's call into the runtime; disposal returned while a profile
  was still executing and then released the artifact lease under it; the declared thread affinity
  was carried in every descriptor and read by nothing; and an instantiation racing disposal
  registered its instance into a runtime that had already walked its list. The pattern is the same
  one VM-2 found three times - a bound that is declared, carried and read nowhere - and a reviewer
  should ask what else is in that category rather than reading these four as isolated slips. It is
  `[PART]` because the evidence was collected on four processors (EX-88) and because affinity is
  enforced only where the core can see a thread (EX-89).
- `[MET]` **A declared memory plateau, measured rather than metered.** 400,000 lifecycle cycles
  across four workers, sampled throughout, with the settled managed heap returning to within tens of
  kilobytes of its starting figure after gigabytes of allocation. The behavioural suite already
  asserted the *metered* plateau, which says the core's accounting balances and nothing about
  whether the process grows; this is the other claim.
- `[MET]` **License:** Apache-2.0, byte-identical to the other Broiler components. No
  third-party runtime dependency was introduced, so no notices file is carried.

## 6. For The Reviewer's Attention

Offered because a reviewer's time is better spent where the risk is, not because these
are known defects. Each item has a stable ID so it can be cited in a note or a
condition.

- **AT-1 - Three places where the implementation could not honour a record verbatim.** They
  are filed as errata in the VM-1 bundle, not as amendments, and each is a decision a
  reviewer may reverse. `VmControlResult` is a struct rather than the enum ADR 0003's
  name table records, because ADR 0004 and ADR 0009 both require it to carry a reason
  and a bare enum cannot. Stage results are constructed through hidden public factories
  rather than internal constructors, because rule A10 forbids `InternalsVisibleTo`
  while a profile package must be able to name them - an illegal stage/category cell
  still has no factory anywhere, so the matrix remains a compile-time fact. And
  `VmOperation` is a frozen public name that is used internally but not exported.
- **AT-2 - The four invariant resolutions.** In
  [ADR 0007](docs/adr/0007-resource-authority-and-budgets.md) and
  [ADR 0009](docs/adr/0009-external-suspension-and-async-instantiation.md), now with
  code to read them against. In particular, whether `TryTakeSuspension` gives the party
  entitled to resume a path to resume in every origin case, without reintroducing the
  second admission check it was designed to remove.
- **AT-3 - A passing suite proved much less than it looked like it proved.** The first
  implementation passed 150 tests with no warnings and published a working Native
  AOT binary while an aggregate budget could be driven to zero with memory still
  live, an operation resumed under a spent parent, and a capability declaring that
  a fault terminates the operation terminated nothing. If a reviewer reads one
  thing besides the records, it should be the review section of the VM-1 bundle.
- **AT-4 - One negative control initially failed to fail.** Removing the mediator's
  no-provider refusal did not break the suite, because a null provider then threw and
  was translated, and the fixture still reported a profile fault - so tests asserting
  only the outcome category passed. Four assertions were strengthened to name the
  core's reason. The episode is retained in the bundle because it is evidence about how
  much the other assertions are worth, and a reviewer should ask the same question of
  them.
- **AT-5 - Five of fifty-two architecture rules assert nothing.** One is Vacuous and four
  are Deferred, which is the count VM-0 reported the same way when it said nine rules awaited
  their subject. Rule B3 stays Vacuous
  with its activation milestone moved to VM-3: its subject exists, but a violation is
  unreachable by construction rather than merely absent (Exclusion EX-40). Rule E5 is
  superseded by V1 and V2 and retained as a Deferred row so the supersession is
  auditable.
- **AT-6 - The inbound half of the legacy-boundary rule is environment-conditional**
  (Exclusion EX-01). Rule D1 scans an aggregate checkout when one is present above the
  component, and records an explicit inconclusive result when it is not.
- **AT-7 - Seventeen roadmap amendments are proposed and unapplied** (Exclusion EX-11). The
  records and `docs/roadmap.md` therefore disagree in the places ADR 0003's register
  lists. VM-1 implements the records.
- **AT-8 - The Native AOT result is not reproducible by automation** (Exclusion EX-42). It
  needs a `vcvars64` shell and `IlcUseEnvironmentalTools=true`, because the ILCompiler
  package's own toolchain discovery fails on the collecting machine.
- **AT-10 - Three of VM-2's corrections were found by its own corpus and suite rather than by a
  reviewer.** The uncharged-work counter summed every budget dimension, so one correctly
  metered in-bounds allocation breached a poll bound instantly and a core unit conflation was
  billed to the profile as a broken metering contract; the cumulative nested verifier-work
  bound was validated at runtime creation and read nowhere; and the artifact-limit clamp the
  precedence algorithm requires was computed and discarded. None was found by reading. A
  reviewer should ask what else is in the same category, since the mechanism that found these
  three is the mechanism VM-2 added rather than one that was already looking.
- **AT-11 - The review worksheet covers VM-0 and VM-1 and has no VM-2 or VM-3 items.**
  `docs/review/vm-0-vm-1.md` is the checklist a reviewer walks, and it was written before
  VM-2 existed. Its items are still true and none of them covers the corpus, the fuzz target,
  the precedence layers or the guest-load bounds VM-2 added, nor the consumer profiles, the
  composition register or the closure reports VM-3 added. Rule H3 holds the
  worksheet's own counts and its area coverage and cannot notice that the component has
  outgrown it.
- **AT-12 - VM-3 found a contract property nobody had written down, and it was found by
  composing rather than by reading.** A runtime ceiling is clamped to the tightest profile hard
  maximum in the CATALOG, and adopting a profile default resolves to the tightest default in the
  catalog; both are catalog-wide facts rather than per-profile ones. So a profile that declares
  its own usage as its hard maximum silently caps every profile composed beside it, and the
  failure surfaces as a resource refusal inside an innocent verifier. The two consumer profiles
  were written that way first and the two-profile composition could not verify a ledger artifact
  until both were corrected. Nothing in the core changed and nothing here is proposed as a
  defect; what a reviewer should decide is whether the core should be able to say so - a
  catalog-construction diagnostic when one profile's maxima bind another's defaults would turn a
  confusing runtime refusal into a composition-time message. `docs/compositions.md` section 5
  is where a profile author currently finds this out, which is documentation rather than a
  mechanism.
- **AT-14 - The four VM-4 corrections were all record-versus-implementation gaps, not design
  errors.** Every one of them is a case where a frozen record says a thing is enforced and nothing
  enforced it, and all four survived three milestones and an adversarial review. Two of them -
  `DisposeDrainBudget` and `VmThreadAffinity.OperationThreadPinned` - were carried in the public
  surface the whole time, so a host could configure them and a profile could declare them, and
  neither did anything. A reviewer's most useful question is not whether these four fixes are right
  but what mechanism would have found them earlier: the answer this component has is the unused-
  reason sweep that found `ThreadAffinityViolation`, and it is a script rather than a rule.
- **AT-15 - The in-capability scope change is a deviation from ADR 0011 F5 and a reviewer may
  reverse it.** The record says "per-runtime in-capability flag". Read literally that is what was
  implemented, and it made any blocking host capability stop the whole runtime for every other
  thread. The reading taken here is that the record's own sentence - a capability must not call back
  into *the invoking runtime* - is about a call stack. If a reviewer prefers the literal reading, the
  fix is to amend the sentence rather than to restore the behaviour, because the behaviour it
  produced is an availability failure with no compensating safety property.
- **AT-13 - VM-3's two consumer profiles are exercised by published binaries and by no test
  project.** Rule A11 forbids a project outside a composition root to reference a profile
  assembly, and demonstrating that rule is part of what VM-3 is for, so the suite cannot reach
  either profile. Twelve checks across two composition roots run in three publish modes each and
  their transcripts are retained, but nothing re-runs them in `dotnet test`, and neither profile
  is fuzzed. A reviewer should decide whether that trade is right: the alternative is a weaker
  A11 and a larger suite.
- **AT-9 - The records and the implementation were drafted with AI assistance and reviewed the
  same way.** The adversarial review that produced the VM-0 revision confirmed 24
  findings, four of them blockers. That is a check on the work, not an independent
  verdict on it, and the same caveat applies to VM-1.

## 7. Residual Risk This Review Cannot Remove

Broiler.VM has one person in every role: architecture owner, core-contract owner,
security owner and reviewer are the same individual. The status ledger's update rule 7
asks for an owner and a reviewer to confirm every exit condition, and with a single
maintainer those are not independent. That is a property of the project's size rather
than a defect in this component, but it belongs on the record rather than glossed:
**no second pair of eyes has seen this work.**

That matters more at VM-1 than it did at VM-0. VM-0 froze decisions on paper, where a
mistake is cheap to reverse. VM-1 decodes untrusted bytes, and the component is now
carrying an unreviewed parser and an unreviewed budget enforcer.

It matters more again at VM-2, and in a way worth stating plainly rather than leaving to be
inferred, and VM-3 adds a third turn of the same screw: the two consumer profiles are the
component's own demonstration that its contract is usable by someone outside it, and they were
written by the same person who wrote the contract. A profile author who did not already know
what the core expects is exactly the reader they were meant to stand in for. VM-2's corpus and fuzz target are exactly the mechanisms that make an unreviewed
parser bearable, and they are themselves unreviewed: the corpus's expectations are one
person's reading of what the verifier ought to answer, the fuzz target's invariants are the
same person's list of what must never happen, and the negative controls show only that each
mechanism rejects the defect it was written to reject. Two million iterations finding nothing
is worth what the invariants are worth. Ledger update rule 8's second consequence is the one
being observed here: unreviewed work accumulates, and there is now more of it to read in one
sitting than there was.

## 8. Area Verdicts

One row per review area from section 4, filled in by the reviewer using the review
verdicts in section 1. A row left at `[ ]` means that area was not reviewed; it does not
mean the area was accepted.

| ID | Area | Verdict | Reviewer note |
|---|---|---|---|
| RA-1 | Bounded reading of untrusted bytes | `[ ]` | |
| RA-2 | Resource authority and budgets | `[ ]` | |
| RA-3 | Lifecycle and state machine | `[ ]` | |
| RA-4 | Verified-artifact ownership | `[ ]` | |
| RA-5 | Guest-initiated loads and external suspension | `[ ]` | |
| RA-6 | The public contract surface | `[ ]` | |
| RA-7 | The records themselves | `[ ]` | |
| RA-8 | The evidence and the rule register | `[ ]` | |

## 9. Decision

- [ ] **APPROVED FOR PREVIEW** within the intended-use scope above.
- [ ] **APPROVED WITH CONDITIONS** listed below.
- [ ] **NOT APPROVED.**
- [x] **PENDING** - not yet reviewed.

## 10. Conditions

Every area marked `[C]` in section 8 records its condition here.

_None recorded; the review has not been performed._

## 11. Human Attestation

_Unsigned. To be completed by the reviewer named above, who confirms that they are a
human developer, that they personally reviewed the revision and evidence identified
above, and that the decision is their own. The attestation is a scoped engineering
review, not a warranty or a claim that the component is free of defects or
vulnerabilities._

- **Name:** _not yet signed_
- **Reviewer alias:** _not yet signed_
- **Signature or attributable commit:** _not yet signed_
- **Date:** _not yet signed_

AI tools may help assemble evidence, but the review decision, reviewer identity, and
attestation are attributable to the human reviewer alone.
