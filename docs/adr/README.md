# Broiler.VM ADR Index

These records are milestone VM-0 of [the component roadmap](../roadmap.md): they
freeze the component's ownership, terminology, dependency graph, and the
profile-neutral contract that milestone VM-1 implements. They are decisions, not
implementation. What has actually been built and proven is recorded in
[the status ledger](../roadmap.status.md), in
[the VM-0 evidence bundle](../evidence/vm-0/README.md) and in
[the VM-1 evidence bundle](../evidence/vm-1/README.md); nothing here should be
read as a capability claim.

**Milestone VM-1 has now implemented what these records describe, and three of
them could not be honoured verbatim.** The VM-1 bundle names all three as errata
rather than amendments: `VmControlResult` is a struct rather than the enum ADR
0003's name table records, because ADR 0004 and ADR 0009 both require it to carry
a reason; stage results are constructed through hidden public factories rather
than internal constructors, because rule A10 forbids `InternalsVisibleTo` and a
profile package must be able to name them; and `VmOperation` is a frozen public
name the implementation does not export. A reader comparing a record against the
code should read that bundle's Deviations section first.

**One record has since been amended rather than deviated from.** On 2026-08-29
the owner ruled that a reviewer fills in the `// Broiler-Human:` line on a
declaration and nothing else, so `HUMAN_REVIEW.md` became a generated record and
ADR 0001's artefact register - which described a hand-signed attestation naming a
reviewed commit - was revised to match. ADR 0001 is not contract-bearing, so no
version was minted; it carries a dated `Revisions` section stating what each
amended row said before. That is the distinction to keep in view when reading
the three deviations above: there, the implementation could not honour a record
and the record stands; here, the decision itself changed, so the record moved.

Every record carries a `**Core contract:**` header field. The ten
contract-bearing records own part of core contract version 1 and cannot change
without the amendment procedure in
[ADR 0003](0003-core-contract-v1-and-amendments.md). ADR 0001 and ADR 0012
govern component shape and ownership instead, and are not contract-bearing.
`CoreContractVersionTests` in the architecture test project asserts that this
table lists exactly the files present, that every record declares the field, and
that the contract-bearing ten declare the version the `VmCoreContract` constants
carry.

All twelve are `Proposed`, not `Approved`. Every one of the six ownership roles
in [ADR 0012](0012-security-ownership-and-support-matrix.md) is now held by one
person, so someone is in a position to accept them - but nobody has, and with a
single maintainer that confirmation would not be independent.

| ADR | Topic | Core contract |
|---|---|---|
| [0001](0001-component-topology-and-dependency-graph.md) | Component topology, package boundaries, and the dependency graph | not contract-bearing |
| [0002](0002-profile-identity-and-static-catalog.md) | Profile identity, version semantics, and the static catalog | version 1 |
| [0003](0003-core-contract-v1-and-amendments.md) | Core contract version 1 and the amendment procedure | version 1 |
| [0004](0004-lifecycle-and-state-machine.md) | The common execution lifecycle and state machine | version 1 |
| [0005](0005-operation-result-envelope.md) | The operation-result envelope and payload ownership | version 1 |
| [0006](0006-verified-artifact-ownership.md) | Verified-artifact ownership and immutability | version 1 |
| [0007](0007-resource-authority-and-budgets.md) | Resource authority, precedence, and aggregate budgets | version 1 |
| [0008](0008-guest-initiated-loads.md) | Guest-initiated loads and the artifact-provider capability | version 1 |
| [0009](0009-external-suspension-and-async-instantiation.md) | External suspension and asynchronous instantiation | version 1 |
| [0010](0010-embedding-decisions.md) | Embedding: byte round-trip, lazy sections, and incremental verification | version 1 |
| [0011](0011-source-level-profile-contract.md) | The source-level profile contract, profile checklist, and sharing rule | version 1 |
| [0012](0012-security-ownership-and-support-matrix.md) | Security ownership, support matrix, and pinned platform references | not contract-bearing |

## Reading order

[0001](0001-component-topology-and-dependency-graph.md) and
[0003](0003-core-contract-v1-and-amendments.md) first: one fixes what the
component is made of, the other fixes what "core contract version 1" means and
how it is amended. Everything else depends on both.

[0004](0004-lifecycle-and-state-machine.md) and
[0005](0005-operation-result-envelope.md) are a pair - the state machine and the
outcomes its stages return - and are best read together.
[0006](0006-verified-artifact-ownership.md) and
[0007](0007-resource-authority-and-budgets.md) then say what the lifecycle
operates on and what bounds it.

[0008](0008-guest-initiated-loads.md),
[0009](0009-external-suspension-and-async-instantiation.md) and
[0010](0010-embedding-decisions.md) settle the questions the VM-0 gate names one
by one, including the ones whose answer is that core contract version 1 admits a
transition the first release will not implement.
[0011](0011-source-level-profile-contract.md) is the record a profile author
reads; [0012](0012-security-ownership-and-support-matrix.md) is the record a
release or security owner reads.
