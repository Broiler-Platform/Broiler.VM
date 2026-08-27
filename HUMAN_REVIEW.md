# Human Review: Broiler.VM

> **Status: PENDING.** No human reviewer has yet attested to this component. Until
> the attestation below is completed and signed, Broiler.VM must not be described as
> human-approved, and no package may be published from it.

This file exists so the review decision has somewhere to live and cannot be inferred
from anything else. It is deliberately unsigned:
[ADR 0001](docs/adr/0001-component-topology-and-dependency-graph.md) originally
deferred this file to VM-6 on the grounds that a template with unfilled fields invites
a false approval record, and that risk is real. Nothing here is an approval, and the
evidence section records what was collected, not what was reviewed.

## Review Target

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

## Evidence Available To The Reviewer

Collected by automation and retained in
[the VM-0 evidence bundle](docs/evidence/vm-0/README.md) and
[the VM-1 evidence bundle](docs/evidence/vm-1/README.md). It is input to a review, not
a substitute for one.

- **Build and tests:** `dotnet build Broiler.VM.slnx -c Release` completes with 0
  warnings and 0 errors across seven projects; `dotnet test Broiler.VM.slnx -c Release`
  reports 175 passed, 0 failed, 0 skipped - 44 architecture and 131 behavioural.
- **An adversarial review has been run, and it found a great deal.** Six reviewers
  against the frozen records, every finding put to two independent refuters: 45
  findings survived, sixteen of them blockers, several confirmed by executing the
  code. All sixteen are corrected and regression-tested; twenty-nine majors and
  minors are recorded and unaddressed (EX-52). The first evidence bundle is
  superseded because it reported a green suite over a tree that contained all
  sixteen.
- **Negative controls:** four, each injected, run, reverted and re-run. A forbidden
  project edge fails A4 and A7; a deleted manifest edge fails A7; a struck name on the
  public surface fails V3; and removing the mediator's no-provider refusal fails the
  behavioural suite. Every run is retained in `negative-control.log`.
- **Native AOT:** the composition-root host publishes and **runs** as a 1,270,784-byte
  self-contained native binary on `win-x64`, and as a 162,816-byte trimmed binary,
  composing two fixture profiles through the generic contract with no trim or AOT
  warnings. It builds with `TreatWarningsAsErrors`.
- **Packaging:** `dotnet pack` still produces exactly three `.nupkg` and three
  `.snupkg`. Neither test-only project added by VM-1 packs.
- **Runtime dependencies:** the three product projects have no package references and
  no project references outside the component. Test packages are confined to the two
  test projects.
- **Security-sensitive behaviour:** this is the part that changed most, and it is where
  a reviewer's time is best spent. The product graph now decodes untrusted bytes. Every
  length, count, offset and allocation goes through the bounded reader and the
  allocation guard, which refuse before allocating; verification produces an immutable
  handle and execution consumes only that handle; a caller's buffer may be mutated
  afterwards without effect. There is still no file, network, process, native-interop,
  unsafe or code-execution path, and rule B5 asserts the absence of dynamic-loading
  APIs over compiled metadata. What has **not** been done is fuzzing, a retained
  malformed-input corpus, or any concurrency testing; those are VM-2's and VM-4's.
- **License:** Apache-2.0, byte-identical to the other Broiler components. No
  third-party runtime dependency was introduced, so no notices file is carried.

## For The Reviewer's Attention

Offered because a reviewer's time is better spent where the risk is, not because these
are known defects.

- **Three places where the implementation could not honour a record verbatim.** They
  are filed as errata in the VM-1 bundle, not as amendments, and each is a decision a
  reviewer may reverse. `VmControlResult` is a struct rather than the enum ADR 0003's
  name table records, because ADR 0004 and ADR 0009 both require it to carry a reason
  and a bare enum cannot. Stage results are constructed through hidden public factories
  rather than internal constructors, because rule A10 forbids `InternalsVisibleTo`
  while a profile package must be able to name them - an illegal stage/category cell
  still has no factory anywhere, so the matrix remains a compile-time fact. And
  `VmOperation` is a frozen public name that is used internally but not exported.
- **The four invariant resolutions** in
  [ADR 0007](docs/adr/0007-resource-authority-and-budgets.md) and
  [ADR 0009](docs/adr/0009-external-suspension-and-async-instantiation.md), now with
  code to read them against. In particular, whether `TryTakeSuspension` gives the party
  entitled to resume a path to resume in every origin case, without reintroducing the
  second admission check it was designed to remove.
- **A passing suite proved much less than it looked like it proved.** The first
  implementation passed 150 tests with no warnings and published a working Native
  AOT binary while an aggregate budget could be driven to zero with memory still
  live, an operation resumed under a spent parent, and a capability declaring that
  a fault terminates the operation terminated nothing. If a reviewer reads one
  thing besides the records, it should be the review section of the VM-1 bundle.
- **One negative control initially failed to fail.** Removing the mediator's
  no-provider refusal did not break the suite, because a null provider then threw and
  was translated, and the fixture still reported a profile fault - so tests asserting
  only the outcome category passed. Four assertions were strengthened to name the
  core's reason. The episode is retained in the bundle because it is evidence about how
  much the other assertions are worth, and a reviewer should ask the same question of
  them.
- **One of thirty-eight architecture rules asserts nothing.** Rule B3 stays Vacuous
  with its activation milestone moved to VM-3: its subject exists, but a violation is
  unreachable by construction rather than merely absent (Exclusion EX-40). Rule E5 is
  superseded by V1 and V2 and retained as a Deferred row so the supersession is
  auditable.
- **The inbound half of the legacy-boundary rule is environment-conditional**
  (Exclusion EX-01). Rule D1 scans an aggregate checkout when one is present above the
  component, and records an explicit inconclusive result when it is not.
- **Seventeen roadmap amendments are proposed and unapplied** (Exclusion EX-11). The
  records and `docs/roadmap.md` therefore disagree in the places ADR 0003's register
  lists. VM-1 implements the records.
- **The Native AOT result is not reproducible by automation** (Exclusion EX-42). It
  needs a `vcvars64` shell and `IlcUseEnvironmentalTools=true`, because the ILCompiler
  package's own toolchain discovery fails on the collecting machine.
- **The records and the implementation were drafted with AI assistance and reviewed the
  same way.** The adversarial review that produced the VM-0 revision confirmed 24
  findings, four of them blockers. That is a check on the work, not an independent
  verdict on it, and the same caveat applies to VM-1.

## Residual Risk This Review Cannot Remove

Broiler.VM has one person in every role: architecture owner, core-contract owner,
security owner and reviewer are the same individual. The status ledger's update rule 7
asks for an owner and a reviewer to confirm every exit condition, and with a single
maintainer those are not independent. That is a property of the project's size rather
than a defect in this component, but it belongs on the record rather than glossed:
**no second pair of eyes has seen this work.**

That matters more at VM-1 than it did at VM-0. VM-0 froze decisions on paper, where a
mistake is cheap to reverse. VM-1 decodes untrusted bytes, and the component is now
carrying an unreviewed parser and an unreviewed budget enforcer.

## Decision

- [ ] **APPROVED FOR PREVIEW** within the intended-use scope above.
- [ ] **APPROVED WITH CONDITIONS** listed below.
- [ ] **NOT APPROVED.**
- [x] **PENDING** - not yet reviewed.

**Conditions:** _none recorded; the review has not been performed._

## Human Attestation

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
