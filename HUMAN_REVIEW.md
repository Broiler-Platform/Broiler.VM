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
- **Scope:** Milestone VM-0 only - the twelve boundary records under `docs/adr/`, the
  five-project shell graph under `src/`, the architecture rules and their witnesses,
  and the evidence bundle under `docs/evidence/vm-0/`. There is no runtime, catalog,
  verifier, budget or profile to review; none exists.
- **Release:** None. Broiler.VM has never been published and claims no RID.
- **Commit under review:** _to be recorded by the reviewer_
- **Reviewer:** MaiRat / Maik Ratzmer
- **Review date:** _not yet performed_

Any source change after the reviewed commit invalidates the approval until the changed
revision is reviewed again.

## Evidence Available To The Reviewer

Collected by automation and retained in
[the VM-0 evidence bundle](docs/evidence/vm-0/README.md). It is input to a review, not
a substitute for one.

- **Build and tests:** `dotnet build Broiler.VM.slnx -c Release` completes with 0
  warnings and 0 errors; `dotnet test Broiler.VM.slnx -c Release` reports 35 passed, 0
  failed, 0 skipped.
- **Negative control:** injecting a forbidden edge - `Broiler.VM.Runtime` referencing
  the test-only `Broiler.VM.Fixtures` - fails rules A4 and A7; reverting returns the
  suite to green. Both runs are retained in `negative-control.log`.
- **Packaging:** `dotnet pack` produces exactly three `.nupkg` and three `.snupkg`.
  The fixture profile does not pack.
- **Runtime dependencies:** the three product projects have no package references and
  no project references outside the component. Test packages are confined to the test
  project.
- **Security-sensitive behaviour:** none exists to review. The product graph exports
  one public type carrying two integer constants, and contains no file, network,
  process, native-interop, unsafe, reflection, environment or code-execution path.
  Rule B5 asserts the absence of dynamic-loading APIs over compiled metadata.
- **License:** Apache-2.0, byte-identical to the other Broiler components. No
  third-party runtime dependency was introduced, so no notices file is carried.

## For The Reviewer's Attention

Offered because a reviewer's time is better spent where the risk is, not because these
are known defects.

- **The four invariant resolutions** in
  [ADR 0007](docs/adr/0007-resource-authority-and-budgets.md) and
  [ADR 0009](docs/adr/0009-external-suspension-and-async-instantiation.md). In
  particular, whether `TryTakeSuspension` gives the party entitled to resume a path to
  resume in every origin case, without reintroducing the second admission check it was
  designed to remove.
- **Nine of twenty-eight architecture rules assert nothing yet.** Six are Vacuous
  because their subject is a shell; three are Deferred to VM-6. `rules.register.json`
  records each with its activation milestone. The claim these records make is *"every
  forbidden edge in the VM-0 shell graph is expressed and witnessed"* - never the
  unqualified form.
- **Rules B3 and B6 have no witness at VM-0** and say so with the reason. Their
  project-file twins A1, A2 and A4 are witnessed.
- **The inbound half of the legacy-boundary rule is environment-conditional**
  (Exclusion EX-01). Rule D1 scans an aggregate checkout when one is present above the
  component, and records an explicit inconclusive result when it is not.
- **Seventeen roadmap amendments are proposed and unapplied** (Exclusion EX-11). The
  records and `docs/roadmap.md` therefore disagree in the places ADR 0003's register
  lists.
- **The records were drafted with AI assistance and reviewed the same way.** The
  adversarial review that produced the current revision confirmed 24 findings against
  the checkout, four of them blockers, two of which were rules that did not enforce
  what they claimed. That review is a check on the work, not an independent verdict on
  it.

## Residual Risk This Review Cannot Remove

Broiler.VM has one person in every role: architecture owner, core-contract owner,
security owner and reviewer are the same individual. The status ledger's update rule 7
asks for an owner and a reviewer to confirm every exit condition, and with a single
maintainer those are not independent. That is a property of the project's size rather
than a defect in this component, but it belongs on the record rather than glossed:
**no second pair of eyes has seen this work.**

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
