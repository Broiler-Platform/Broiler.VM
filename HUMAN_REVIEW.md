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
not budgets anyone is held to. The worksheet carries 53 items, numbered contiguously from
`RC-01`, with at least one for each of the eight areas; that count is stated here as well as
there so that neither document can shed an item on its own.

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
[the VM-0 evidence bundle](docs/evidence/vm-0/README.md) and
[the VM-1 evidence bundle](docs/evidence/vm-1/README.md). It is input to a review, not
a substitute for one. Each line carries an evidence verdict as defined in section 1;
these are the author's marks about evidence, not review verdicts.

- `[MET]` **Build and tests:** `dotnet build Broiler.VM.slnx -c Release` completes with 0
  warnings and 0 errors across seven projects; `dotnet test Broiler.VM.slnx -c Release`
  reports 220 passed, 0 failed, 0 skipped - 89 architecture and 131 behavioural.
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
  `.snupkg`. Neither test-only project added by VM-1 packs.
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
  APIs over compiled metadata. What has **not** been done is fuzzing, a retained
  malformed-input corpus, or any concurrency testing; those are VM-2's and VM-4's.
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
