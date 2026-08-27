# Broiler.VM roadmap status

**Last updated:** 2026-08-27

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[Broiler.VM roadmap](roadmap.md). The roadmap defines planned work and objective exit gates; this
ledger records whether those gates have accepted evidence.

No Broiler.VM milestone is complete merely because its design appears in the roadmap. At this
snapshot, **VM-0 is in progress and VM-1 through VM-6 are not started.** The repository contains
the Broiler.VM [component overview](../README.md), the roadmap documents, the twelve boundary
records in [docs/adr](adr/README.md), and a five-project shell graph with its architecture tests.
It contains no Broiler.VM runtime, catalog, verifier, budget, profile, sample, or Native AOT
result, and VM-0 itself is **not accepted**: its dependency on named ownership is unmet, so no
milestone that waits on VM-0 acceptance may begin.

---

## 1. Reading this ledger

The following categories must remain distinct:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation or validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout, such as the
  absence of Broiler.VM projects. It can explain a status but cannot by itself satisfy a future
  implementation, contract, performance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable evidence bundle that identifies the exact
  sources and gate, records the executed commands and environment, retains their outputs, and
  demonstrates every part of the objective exit gate. Only accepted evidence may advance a
  milestone to `Accepted`.

Work in other components is not Broiler.VM evidence. In particular, the legacy `Broiler.JS`
component's implementation, conformance results, measurements, and Native AOT samples establish
nothing here, and no core gate may cite them. Broiler.VM has no dependency on that component and
none of its milestones wait for it.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, owner, and unblock condition must be recorded; lack of scheduling is not a blocker. |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and owner/reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and decision record must be linked; evidence history is retained. |

---

## 2. Current milestone status

| Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|
| **VM-0 — ownership, terminology, core contract version, and graph** | **In progress** | [Evidence bundle VM-0-001](evidence/vm-0/README.md), collected 2026-08-27 against component commit `6235603` with a dirty tree. Twelve boundary records in [docs/adr](adr/README.md) freeze the graph, ID policy, lifecycle, result envelope, verified-artifact ownership, resource authority and precedence, the guest-initiated-load, asynchronous-instantiation, external-suspension and aggregate-budget decisions, the three embedding decisions, the profile-facing checklist and sharing rule, and core contract version 1 with its amendment procedure. A five-project acyclic shell graph builds Release with 0 warnings; 33 architecture tests pass; a negative control shows the containment and edge rules rejecting an injected forbidden edge; pack produces exactly three packages and does not pack the fixture profile. Every forbidden edge in the VM-0 shell graph is expressed and witnessed; nine rules await their subject and are registered in [rules.register.json](../src/tests/Broiler.VM.Architecture.Tests/rules.register.json). **Not shown:** any runtime behaviour, any RID, trimming or Native AOT result, any platform other than Windows x64, and any transition, member or category named in ADRs 0002-0011 — those are paper decisions (EX-21). | **Open gate conditions.** (1) VM-0's dependency is unmet: all six ownership roles in ADR 0012 are vacant, so no owner or reviewer can accept the records, and the ledger's update rule 7 therefore blocks `Accepted` (EX-30). (2) The twelve records are `Proposed`, not approved. (3) The inbound half of the legacy-boundary rule is environment-conditional (EX-01). (4) Six rules are Vacuous and three Deferred (EX-05). (5) The evidence was collected from a dirty tree and must be re-collected against the commit that lands it. (6) No SDK pin exists (EX-03). (7) Seventeen roadmap amendments are proposed and unapplied (EX-11). Next: name the six roles, then review and approve the twelve records. |
| **VM-1 — semantics-neutral runtime, catalog, and fixture profile** | **Not started** | No Broiler.VM contracts, binary primitives, runtime, catalog, composition root, fixture profile, or Native AOT construction host exists. | After VM-0 acceptance, implement and test the neutral contracts, bounded binary primitives, and catalog with a fixture profile, including whichever of guest-initiated-load mediation, artifact-provider registration, external suspension, and aggregate budget metering VM-0 assigned to the core, and their refusal paths. Shape the fixture adapter after a non-trivial existing runtime so the contract is not fitted to a toy. |
| **VM-2 — bounded artifacts, verification, and resources** | **Not started** | No common descriptor, opaque verified-artifact handle, bounded loader, trusted-limit intersection, verifier result contract, malformed corpus, or fuzz target exists. | After VM-1 acceptance, prove the common boundary with immutable copied or decoded fixture artifacts, caller-mutation tests, bounded failures, explicit default and omission cases, host/profile/artifact intersection, invocation-only tightening, and bounded guest-initiated loads charged to the requesting operation. |
| **VM-3 — public profile contract and exact closures** | **Not started** | The static catalog is documented only. No application-local consumer profile, ID-governance test, catalog drift check, or exact closure report exists. | After VM-2, implement an application-local profile through the public source contract alone and compose it by direct typed registration. Prove that a second profile requires no core change, and report the exact closure of each named composition under trimming and Native AOT. |
| **VM-4 — lifecycle, concurrency, diagnostics, and hosts** | **Not started** | No Broiler.VM lifecycle, reentrancy, cancellation, isolation, host-failure, diagnostics, disposal, or memory-plateau result exists. | After VM-3, stress the VM-0/VM-1 lifecycle with multiple fixture profiles and independent runtimes. Retain host-boundary, reclamation, diagnostics, isolation, external-suspension, in-flight guest-load cancellation, and aggregate budget evidence. |
| **VM-5 — core overhead baselines** | **Not started** | No accepted uninstrumented baseline of core overhead exists. | After VM-2 and VM-4, take decision-grade baselines of verification throughput, catalog and runtime lifecycle cost, budget metering overhead, guest-load mediation, envelope handling, startup, image size, and resident-set plateau on JIT and Native AOT with the fixture profile. |
| **VM-6 — package, release, and recertification** | **Not started** | No Broiler.VM package, API baseline, pristine feed consumer, support table, release bundle, rollback result, or recertification record exists. | After VM-0 through VM-4, finalize package boundaries, create pristine feed consumers and public-API samples, freeze the public API, the source-level profile contract and the core contract version, and wire graph, catalog, AOT, and drift checks into required CI and this ledger. |

The immediate programme action is therefore **naming the six Broiler.VM ownership roles recorded
in [ADR 0012](adr/0012-security-ownership-and-support-matrix.md)**, and then reviewing the twelve
boundary records. VM-0's technical work is done and its evidence is retained; what remains is the
one thing the roadmap listed as its dependency and the one thing this ledger cannot manufacture,
which is a named owner and a named reviewer.

### Profiles

The JavaScript and WebAssembly profiles are separate components with their own roadmaps and their
own ledgers. They are **not planned or tracked here**, no row above depends on either, and no
profile result closes a core gate. Section 9 of the roadmap records only what they are expected to
require of the core contract.

A profile roadmap may open once VM-1's contract is accepted. The JavaScript profile additionally
carries a seeding precondition recorded in its own roadmap: it starts from a named snapshot copy
of the legacy component taken after that component's in-flight fix programme lands, as a fork with
no dependency edge in either direction.

---

## 3. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle with all applicable fields
below. A command written in a plan is not evidence that the command ran.

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap/gate revision, core contract version, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Repository commit, dirty-tree state and patch identity, and exact paths and projects under test. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, fixture hashes, and applicable provenance or license decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets must be redacted without hiding semantically relevant configuration. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Outputs** | Durable logs, machine-readable results, binaries or packages or hashes, analyzer and trim/AOT warnings, crash dumps or minimized fuzz cases where applicable, and storage locations with retention policy. |
| **Decision** | Expected gate, actual result, unexplained failures, exclusions, deviations, the claim justified by the result, reviewer verdict, and follow-up owner. |
| **Validity** | Reproduction instructions, expiry or review date where evidence can age, and recertification triggers such as source, dependency, SDK, RID, contract-version, API, or composition changes. |

Performance evidence must additionally follow the repository's decision-grade measurement rules,
retain every repetition and resource metric, identify candidate and control exactly, and report
negative or inconclusive results. Security and fuzz evidence must retain corpus identity, budgets,
duration or iteration count, sanitizer and runtime settings, failures, and minimized regressions.
Native AOT evidence must come from publishing and running the declared composition on every
claimed RID; analyzer success alone is not a publish-and-run result.

---

## 4. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, including failures and exclusions.
3. Do not infer completion transitively. VM-0 acceptance does not accept VM-1; a fixture result
   does not accept an application-local one; and JIT, trimmed, or one-RID success does not accept
   an untested Native AOT or RID claim.
4. Do not promote seed, shell, smoke, analyzer-only, or shape-only results beyond what they prove.
   A failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence
   gathered for an older or different population is not silently carried forward. A core contract
   amendment is such a change: record the new version and state, per affected record, what
   recertifies unchanged, what must be re-collected, and what is superseded.
6. Do not record profile work here. A profile's status belongs to that profile's own ledger, and a
   profile result never advances a row in this file.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and evidence-bundle ID in
   the affected row.

Until such updates are recorded, the table in section 2 remains the complete Broiler.VM status:
VM-0 is in progress and unaccepted, VM-1 through VM-6 are not started, and no implementation or
release capability is claimed.
