# Broiler.VM roadmap status

**Last updated:** 2026-08-28

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[Broiler.VM roadmap](roadmap.md). The roadmap defines planned work and objective exit gates; this
ledger records whether those gates have accepted evidence.

No Broiler.VM milestone is complete merely because its design appears in the roadmap. At this
snapshot, **VM-0 and VM-1 are both in progress and unaccepted, and VM-2 through VM-6 are not
started.**

The repository now contains the Broiler.VM [component overview](../README.md), the roadmap
documents, the twelve boundary records in [docs/adr](adr/README.md), and a seven-project graph
that implements core contract version 1: the profile-neutral contracts, the bounded binary
primitives, the immutable catalog, the runtime and its lifecycle, resource authority including
aggregate budgets, guest-initiated-load mediation, external suspension, two fixture profiles, a
composition-root host that publishes and runs under trimming and Native AOT, and 221 passing
tests: 90 architecture and 131 behavioural.

**Neither milestone is accepted, and neither can be until a human reads the work.** VM-0 and VM-1
are both unaccepted because no relevant code unit carries a decision on its `// Broiler-Human:`
line, so the generated [`HUMAN_REVIEW.md`](../HUMAN_REVIEW.md) states `PENDING`.

VM-1 was nonetheless built against VM-0's frozen-but-unapproved records, and update rule 8 now
records why that was legitimate: **human review gates a release, not a development step.** The
owner ruled on 2026-08-28 that implementing, landing and evidencing a milestone may proceed without
a review decision, and that publishing a package, claiming a RID, issuing a support table or moving
any milestone to `Accepted` may not. That resolves what Exclusion EX-43 previously recorded as an
open reading.

Nothing about the component's actual state changes with it. No release capability is claimed, and
the work that has accumulated unreviewed is not paper: it is a parser over untrusted bytes and a
budget enforcer.

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

The marks that appear in section 2 - evidence verdicts set by the author about evidence,
and review verdicts set by a reviewer about their own reading - are defined once, in
[HUMAN_REVIEW.md section 1](../HUMAN_REVIEW.md#1-how-to-use-this-file), which is their
canonical legend.

---

## 2. Current milestone status

The leading column is an evidence verdict - the author's mark about what the row's
retained evidence shows, not a reviewer's finding and not a change of state. An
`In progress` milestone with a retained bundle is partly demonstrated; a `Not started`
milestone claims nothing at this snapshot.

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [PART] | **VM-0 — ownership, terminology, core contract version, and graph** | **In progress** | [Evidence bundle VM-0-001](evidence/vm-0/README.md), collected 2026-08-27 against component commit `1bba027` with a clean source tree. Twelve boundary records in [docs/adr](adr/README.md) freeze the graph, ID policy, lifecycle, result envelope, verified-artifact ownership, resource authority and precedence, the guest-initiated-load, asynchronous-instantiation, external-suspension and aggregate-budget decisions, the three embedding decisions, the profile-facing checklist and sharing rule, and core contract version 1 with its amendment procedure. A five-project acyclic shell graph built Release with 0 warnings; 35 architecture tests passed; a negative control showed the containment and edge rules rejecting an injected forbidden edge; pack produced exactly three packages and did not pack the fixture profile. **Superseded in part by VM-1:** the shell graph and its counts are historical, and VM-1's implementation now exercises what those records describe. The *decisions* VM-0 froze are unchanged and still unapproved. | **Open gate conditions, unchanged.** (1) No review decision is recorded. All six ownership roles in ADR 0012 are held by MaiRat, so section 13's dependency line is satisfied, but `HUMAN_REVIEW.md` is unsigned and `PENDING`, and owner and reviewer are the same person, so update rule 7's confirmation would not be independent (EX-30). (2) The twelve records are `Proposed`, not approved. (3) The inbound half of the legacy-boundary rule is environment-conditional (EX-01). (4) No SDK pin exists (EX-03). (5) Seventeen roadmap amendments are proposed and unapplied (EX-11). Next: review and sign, or reject, the twelve records — now with an implementation to read them against, which is a better position to review from than paper alone. |
| [PART] | **VM-1 — semantics-neutral runtime, catalog, and fixture profile** | **In progress** | [Evidence bundle VM-1-003](evidence/vm-1/README.md), collected 2026-08-28, superseding VM-1-002 and VM-1-001. Re-collected because the component gained the group H review-record rules and the Broiler Code Assurance system, which changed the hash of every product source file and moved the architecture suite from 44 tests to 89. Core contract version 1 is implemented: the profile-neutral contracts and thirty-row descriptor, the bounded readers and allocation guard, the immutable catalog with its canonical encoding, the runtime with its lifecycle, execution slot, latches and idempotent disposal, the fifteen-dimension meter chain with aggregate budgets, guest-initiated-load mediation with its bounds and its deterministic no-provider refusal, external suspension behind the double gate, typed payload projection, and two fixture profiles built as a bytecode stack machine with a real framed format. Seven projects build Release with **0 warnings**; **221 tests pass** (90 architecture, 131 behavioural); pack still produces exactly three packages; **the composition-root host publishes and runs under JIT, trimming and Native AOT on `win-x64`**, composing two profiles through the generic contract. Four negative controls each fail when injected and pass after revert. Fourteen of the gate's sixteen clauses are demonstrated, a fifteenth in part (G-08: reentrancy is enforced on the execution path, thread affinity is never exercised across threads), and the sixteenth is not met. **The implementation was adversarially reviewed against the frozen records: 45 findings survived independent refutation, sixteen of them blockers, several confirmed by execution. Every blocker is corrected and regression-tested; the first bundle, VM-1-001, is superseded because it reported a green suite over a tree that contained all sixteen.** | **Open gate conditions.** (1) No review decision is recorded, and no reviewer has read this work — the gate's own last clause, "the accepted contract is recorded with its version", is therefore **not met**: the contract is implemented and versioned, not accepted. (2) VM-1's dependency on VM-0 acceptance was not satisfied when the work was done (EX-43). (3) Declared thread affinity is carried but never exercised across threads; concurrency is VM-4's (EX-44). (4) One RID, one machine, no CI (EX-45). (5) The Native AOT publish needs a `vcvars64` shell because the ILCompiler package's own toolchain discovery fails here, so no automation reproduces it (EX-42). (6) Three deviations from the frozen records are filed as errata rather than amendments — the control-result shape, the result-construction gate, and the unexported `VmOperation` (EX-41). (7) Twenty-nine review findings of major and minor severity are recorded and unaddressed (EX-52): the runtime is never poisoned by a broken metering contract, the non-reentrancy gate is absent on the control operations, the disposable artifact lifetime releases nothing, and several diagnostics groups are thinner than the records require. Next: review the contract surface and the three errata, then sign or reject — and read the review section of the bundle first, because one pass found sixteen blockers behind a suite that was passing. |
| [N/A] | **VM-2 — bounded artifacts, verification, and resources** | **Not started** | The descriptor, the opaque verified-artifact handle, the bounded loader, the limit intersection and the verifier result contract now exist and are exercised by VM-1 against a seven-corruption fixture corpus. That is not VM-2 evidence: no malformed-input corpus is retained, no fuzz target exists, and update rule 4 forbids promoting a subset generated to prove a contract into evidence for a corpus gate (EX-48). | After VM-1 acceptance, prove the common boundary with immutable copied or decoded fixture artifacts, caller-mutation tests, bounded failures, explicit default and omission cases, host/profile/artifact intersection, invocation-only tightening, and bounded guest-initiated loads charged to the requesting operation. |
| [N/A] | **VM-3 — public profile contract and exact closures** | **Not started** | The static catalog is documented only. No application-local consumer profile, ID-governance test, catalog drift check, or exact closure report exists. | After VM-2, implement an application-local profile through the public source contract alone and compose it by direct typed registration. Prove that a second profile requires no core change, and report the exact closure of each named composition under trimming and Native AOT. |
| [N/A] | **VM-4 — lifecycle, concurrency, diagnostics, and hosts** | **Not started** | No Broiler.VM lifecycle, reentrancy, cancellation, isolation, host-failure, diagnostics, disposal, or memory-plateau result exists. | After VM-3, stress the VM-0/VM-1 lifecycle with multiple fixture profiles and independent runtimes. Retain host-boundary, reclamation, diagnostics, isolation, external-suspension, in-flight guest-load cancellation, and aggregate budget evidence. |
| [N/A] | **VM-5 — core overhead baselines** | **Not started** | No accepted uninstrumented baseline of core overhead exists. | After VM-2 and VM-4, take decision-grade baselines of verification throughput, catalog and runtime lifecycle cost, budget metering overhead, guest-load mediation, envelope handling, startup, image size, and resident-set plateau on JIT and Native AOT with the fixture profile. |
| [N/A] | **VM-6 — package, release, and recertification** | **Not started** | No Broiler.VM package, API baseline, pristine feed consumer, support table, release bundle, rollback result, or recertification record exists. | After VM-0 through VM-4, finalize package boundaries, create pristine feed consumers and public-API samples, freeze the public API, the source-level profile contract and the core contract version, and wire graph, catalog, AOT, and drift checks into required CI and this ledger. |

### Where the review stands

No area has been reviewed, so no verdict below is set. **This table is a reader's summary and is
not the record.** The record is per code unit, in the `// Broiler-Human:` line on each declaration,
and [HUMAN_REVIEW.md](../HUMAN_REVIEW.md) is generated from those lines; the areas below are a
risk ordering a reader may find useful and are not bound to anything mechanical.

| Verdict | ID | Area |
|---|---|---|
| [ ] | RA-1 | Bounded reading of untrusted bytes |
| [ ] | RA-2 | Resource authority and budgets |
| [ ] | RA-3 | Lifecycle and state machine |
| [ ] | RA-4 | Verified-artifact ownership |
| [ ] | RA-5 | Guest-initiated loads and external suspension |
| [ ] | RA-6 | The public contract surface |
| [ ] | RA-7 | The records themselves |
| [ ] | RA-8 | The evidence and the rule register |

The immediate programme action is still **the review decision itself**, and it now covers two
milestones rather than one. VM-0's records and VM-1's implementation are both written, both have
retained evidence, and all six roles recorded in
[ADR 0012](adr/0012-security-ownership-and-support-matrix.md) are held. What remains is a human
reading the twelve records *and* the contract surface that implements them, and signing or
rejecting both on the declarations themselves, which regenerates
[HUMAN_REVIEW.md](../HUMAN_REVIEW.md). With a single maintainer that confirmation
is not independent, which EX-30 records rather than resolves.

Reviewing them together is easier than reviewing VM-0 alone was: a decision that was paper in
August can now be read against code that either honours it or does not, and the three places where
the implementation could not honour a record verbatim are named as errata rather than left for a
reader to discover.

It is also more necessary than it was. The first implementation passed 150 tests, built with no
warnings, and published and ran a Native AOT binary while containing sixteen blocking contract
violations - among them an aggregate budget that could be driven to zero while memory was live, and
a capability translation mode that translated nothing. An adversarial pass found them; a second pass
would be a reasonable thing to fund before anyone signs.

### Profiles

The JavaScript and WebAssembly profiles are separate components with their own roadmaps and their
own ledgers. They are **not planned or tracked here**, no row above depends on either, and no
profile result closes a core gate. Section 9 of the roadmap records only what they are expected to
require of the core contract.

A profile roadmap may open once VM-1's contract is accepted. It is implemented but not accepted, so
no profile roadmap may open yet. The JavaScript profile additionally
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
8. **Human review gates a release, not a development step.** Ruled 2026-08-28 by the architecture
   and release owner. Development work - implementing a milestone, landing it, and collecting its
   evidence - may proceed and merge without a review decision. A **release** may not: no package is
   published, no RID is claimed, no support table is issued, and no milestone moves to `Accepted`
   until a named human has read the work and recorded a decision on every relevant code unit, which
   the publish lane checks and [HUMAN_REVIEW.md](../HUMAN_REVIEW.md) reports. The decision is bound
   to the fingerprint of each declaration rather than to a revision, so a unit that changes
   afterwards is reported as `STALE` rather than silently carried.

   This settles the question Exclusion EX-43 recorded rather than answering. Roadmap section 13
   sequences each milestone "after VM-*n* acceptance"; under this ruling that sequencing binds the
   release train, and a milestone may be *built* against records that are frozen but unapproved. It
   does not weaken anything else in this file: rule 7 is unchanged, an unreviewed milestone stays
   `In progress` however complete its evidence is, and the roadmap is not re-authored - the
   divergence is recorded here, as ADR 0003's amendment register records the others.

   Two consequences worth stating plainly, because the alternative is discovering them at release
   time. Unreviewed work accumulates: the longer a release is deferred, the more there is to read
   in one sitting, and the VM-1 review found sixteen blockers in one milestone's worth. And a
   development step that lands unreviewed carries its risk forward rather than dissolving it - the
   component currently holds an unreviewed parser and an unreviewed budget enforcer, and that is
   true no matter how many green suites are collected over them.

Until such updates are recorded, the table in section 2 remains the complete Broiler.VM status:
VM-0 and VM-1 are in progress and unaccepted, VM-2 through VM-6 are not started, and no release
capability is claimed. Core contract version 1 is implemented and runs; it is not accepted, no RID
other than `win-x64` is claimed, no concurrency, corpus or performance result exists, and no
language profile ships.
