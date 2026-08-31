# Broiler.VM.Profile.JavaScript roadmap status

**Last updated:** 2026-08-31

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[JavaScript profile roadmap](roadmap.md). The roadmap defines planned work and objective exit
gates; this ledger records whether those gates have accepted evidence.

**At this snapshot, JS-0 and JS-1 are `In progress` and JS-2 through JS-10 are `Not started`.**
What exists is one feature manifest, one format version, a verifier, an executor, a descriptor
admitted by a catalog, a hand-written lowering, two composition roots that publish and run on one
RID under JIT, trimming and Native AOT, a 51-entry retained corpus, eight decision records, four
registered architecture rules, two evidence bundles and twelve negative controls. There is **no
tokenizer, no static-semantic stage, no object model, no standard library, no suspension, no
guest-initiated load, no snapshot and no conformance harness**. No milestone is complete because
its design appears in the roadmap, **nothing here has been reviewed by a human**, and nothing in
this component may be described as validated, accepted or supported.

**The placement decision is taken.** This component is not a repository of its own and is not a
component of its own: it is a family of product projects inside `Broiler.VM`, at
`src/Broiler.VM.Profile.JavaScript*`, with its roadmap and decisions in the profile assembly's own
project directory. The profile's half is [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md);
the core's half is ADR 0001 revision 5, which authorises the three projects and revises rule A11
so that a profile may reference its own format sibling. **Four things the roadmap assumed would be
this component's own are now the host component's** - the assurance system, the rule register, the
API baseline and the licence and notice files - and each is recorded as a dated deviation in
[JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md) rather than dropped. **What is
not shared is evidence:** a JS bundle is cited only by this ledger, a core bundle only by the
core's, and update rule 6 below is unchanged.

---

## 1. Reading this ledger

Four categories must remain distinct, and conflating any two of them is how an unfounded claim
gets recorded:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation evidence and not validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout — for instance
  that this component contains no project file. It can explain a status; it cannot satisfy a
  future implementation, contract, conformance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable bundle that identifies the exact sources and
  gate, records the executed commands and environment, retains their outputs, and demonstrates
  every part of the objective exit gate. Only accepted evidence may advance a milestone to
  `Accepted`.
- **Inherited material** is anything copied from the seed. It carries **no status of its own**.
  A copied file is unvalidated and unreviewed in this component on the day it lands, however long
  it has existed elsewhere.

**Work in other components is not this component's evidence.** In particular, no conformance
result, benchmark, measurement, review decision, or Native AOT sample produced by the legacy
JavaScript engine component or by the Broiler.VM core establishes anything here, and no gate in
this ledger may cite one. That rule is not a courtesy to the fork; it is what makes a number in
this file mean something.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, its holder, and its unblock condition must be recorded. **Lack of scheduling is not a blocker; an unaccepted upstream contract is.** |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and an owner and reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and the decision record must be linked; evidence history is retained. |

---

## 2. Current milestone status

The leading column is an **evidence verdict** — the author's mark about what a row's retained
evidence shows. It is not a reviewer's finding and not a change of state. The vocabulary is
closed and has three members:

- `[NONE]` — the row has retained evidence of no kind.
- `[PARTIAL]` — the row has a retained bundle that demonstrates some of its exit gate, with
  every unmet clause named in the bundle's own exclusions. **A `[PARTIAL]` row is not a
  qualified pass.** It is a row whose gate is open, and the named clauses are what is open.
- `[FULL]` — the row's bundle demonstrates every exit-gate clause. It is still not `Accepted`:
  acceptance additionally needs an owner and a reviewer decision, which nothing here has.

Two rows are `[PARTIAL]` and the remaining ten are `[NONE]`.

**The milestone set changed on 2026-08-31 and this table now carries the new shape.** What was one
`JS-3` is now `JS-3a` and `JS-3b`, split by dependency rather than by size: the conformance harness
needs a scoring target and not a copied front end, so leaving it fused put this component's only
external correctness signal behind both of the blockers in section 3 when it needed to be behind
neither. Twelve rows, not eleven. Nothing is accepted under either shape, so the split changes no
evidence claim — it changes what a reader is told is schedulable today.

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [PARTIAL] | **JS-0 — boundary, placement, identity, assurance floor** | **In progress** | [Bundle JS-0-001](evidence/js-0/README.md): Release build of the whole solution with 0 warnings; the whole suite green; the assurance gate green and the assurance **release** mode refusing while naming each blocking declaration individually; **8 negative controls, each failing the suite when injected and passing after revert**; the candidate seed identity re-derived and matching on all four revisions. Seven decision records, [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) through [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md). Rules N1–N4 registered Active with nine witness inputs. | **Two exit-gate clauses are open and neither may be read as passed.** (1) The two-profile catalog test needs this profile's descriptor and there is none until JS-1; its `eval`-refusal half needs guest loads and is carried to JS-8. (2) The public API baseline's subject is the packable set, so it does not cover the profile's assemblies; they export nothing public today, and **JS-1 lands a public surface and owns extending it**. Both are named in the bundle's exclusions. |
| [PARTIAL] | **JS-1 — the whole contract loop on a narrow slice** | **In progress** | [Bundle JS-1-001](evidence/js-1/README.md). `broiler.javascript.slice` is minted and format version 1 defined, carrying framed sections, a tagged constant pool, fixed instruction boundaries, **exception regions and suspension targets reserved and refused**, a canonical position table and declared maxima checked before use. All seven core-facing types are implemented. The descriptor is filled in one full-arity construction, admitted by a catalog, and **four named negative cases each provoke a refusal**. **All five verifier outcomes** are produced by named entries of a 51-entry retained corpus which replays twice with no residue, contains 16 passing controls, and on which the verifier throws nothing. **Four of the five execution-step kinds** are produced by named checks. **Two composition roots publish AND run on `win-x64` under JIT, trimmed self-contained and Native AOT**, warnings as errors, closures read off the published output: six managed assemblies for the execution-only image and seven for the compiler-bearing one, **differing by exactly the lowering**. JS-0's carried two-profile catalog clause is discharged in both directions. **Twelve negative controls**, four of them judged by the corpus rather than by the suite. Decision [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) records the entry-point answer and four corrections to earlier records. | **One exit-gate clause is open.** The public API baseline does not cover the profile's assemblies, and the reason is now known rather than pending: `ApiSurface` describes a surface by loading an assembly, which needs a project reference, which **rule A11 forbids a test project to have on a profile**. Two routes are named in the bundle — describe from metadata, or have a composition root print its own surface — and the clause is carried to JS-3b. `Suspended` is declared unreachable and produced at JS-7; five descriptor rows are provisional pending JS-5's measurements; one RID, one machine. |
| [NONE] | **JS-2 — seeding snapshot and front-end ingest** | **Blocked** (recorded as `Not started` above the blocker, because no work has begun either) | None. No snapshot has been taken. The candidate identity in roadmap [section 4.1](roadmap.md#41-the-snapshot-identity) is a recorded candidate, not a taken snapshot. | **Blocked on two named external dependencies.** See section 3. |
| [NONE] | **JS-3a — diagnostic registry, position encoding, pinned suite, the oracle** | **Not started** | None. No published registry, no harness, no pinned suite revision, no self-check fixture. The codes JS-1 emits exist and are grouped by emitting stage, and the position encoding JS-1 uses populates two of the record's four fields; neither is published or versioned. | **Openable now, and JS-1 has produced the scoring target the split was designed around**: five verifier outcomes each by a named corpus entry, and a corpus that replays twice with no residue. Publish the registry bound in both directions — including which half each code belongs to — then stand the harness up and prove the self-check before scoring anything. |
| [NONE] | **JS-3b — static semantics as one verification stage, and the lowering** | **Not started** | None. No consolidated early-error stage, no strict-mode ruling, no lowering, no recorded answer for where the verification boundary falls. | After JS-2, and after JS-3a supplies the registry its diagnostics land in. Record the boundary decision of roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) before writing the stage that depends on it. |
| [NONE] | **JS-4 — value representation and object model** | **Not started** | None. The value-representation decision is **open**, and roadmap [section 23](roadmap.gates.md#23-risks-and-stop-conditions) makes copying standard-library source while it is open a stop condition. | Open the decision now — it needs no copied code and can be prepared against JS-1 rather than waiting on the acceptance gate. |
| [NONE] | **JS-5 — executor, abrupt completion, budgets** | **Not started** | None. No interpreter, no charging model, no measured `CallDepth`. | After JS-4. |
| [NONE] | **JS-6 — the standard library** | **Not started** | None. The satellite-acquisition dependency is unopened and has no named owner. | Open the satellite dependency at JS-0 so JS-6 is not the milestone that discovers it. |
| [NONE] | **JS-7 — suspension** | **Not started** | None. The continuation design needs no copied code and may be opened early. | After JS-5 and JS-6. |
| [NONE] | **JS-8 — guest-initiated loads and the three compositions** | **Not started** | None. No guest-load declaration, no mediator adapter, no composition registers a provider or declines to. | After JS-7. |
| [NONE] | **JS-9 — adversarial input, agents, soak** | **Not started** | None. No corpus, no fuzz target, no soak host, no aggregate-budget exercise. | After JS-8, though the corpus grows from JS-1 onward rather than starting here. |
| [NONE] | **JS-10 — baselines, packaging, support table, release gate** | **Not started** | None. No measurement lane, no baseline register, no package, no support table, no human review decision on anything. Neither the language-specification edition nor the conformance-suite revision is pinned, and roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) requires a provisional pin to carry a named exclusion in this ledger until a human has retrieved, hashed, and archived it — see section 3. | After JS-9, and after a named human has read every relevant unit — which is the largest single-owner task in the programme and must be scheduled, not assumed. |

### What this component is not claiming

Stated positively, because a table of empty rows invites a reader to fill them in:

- **No language is supported.** One feature manifest exists and **none is accepted**: acceptance
  needs a retained oracle run, there is no oracle, and a manifest name would not be a conformance
  claim even if there were. `broiler.javascript.slice` admits numbers, arithmetic, comparison,
  local variables and structured control flow, and admits no object, no string, no function and no
  property access - which is deliberately not JavaScript anyone would ship.
- **No composition is advertised** and none is packable. Two composition roots exist, both
  registered as demonstrations. **One runtime identifier is recorded as published and run** -
  `win-x64` - which is a record of what happened on one machine and not a supported-RID claim;
  claiming a RID is a release act and JS-10 owns it.
- **No conformance result exists.** The suite is not pinned and the harness is not built.
- **No measurement exists**, and no figure from any other component stands in for one.
- **Nothing is reviewed.** No human has read anything here, and nothing that will be copied
  arrives reviewed.
- **The seed has not been taken.** Section 4.1 of the roadmap records a candidate identity so the
  record has a shape; JS-2 records what was actually taken, and may differ. Bundle JS-0-001
  re-derives that candidate from the checkout and matches on all four revisions, which says the
  record is reproducible and says nothing about a snapshot having happened.
- **The product code that exists is a slice and says so.** There is a verifier, an executor and a
  hand-written lowering over about two thousand readable lines. There is no tokenizer, no static
  semantics, no object model, no standard library, no suspension and no guest-initiated load, and
  the value representation is provisional until JS-4.
- **JS-1's hand-written encoder and lowering are scheduled for deletion at JS-4**, with a named
  owner and a gate clause, because a second handle-producing path and a second lowering are
  non-goals.
- **Neither JS-0 nor JS-1 is accepted.** Each has an open exit-gate clause, and acceptance would
  in any case need a reviewer decision that nobody has made.

---

## 3. Open external dependencies

A milestone blocked by a named external dependency records the blocker, its holder, and its
unblock condition. **One is open today**, and it belongs to JS-2. The second, the seed's
un-itemised waited-on set, was closed by JS-0 and is recorded below as closed rather than
deleted.

| Blocker | Holder | Unblock condition | Note |
|---|---|---|---|
| **The core contract is not accepted.** Every core milestone is in progress and unaccepted, and the core's review record is unsigned. The core roadmap's own seeding conditions require the copy to be adapted to an accepted contract rather than a moving one. | The Broiler.VM core's architecture and release owners | A recorded human review decision on the core's contract surface, at a named contract version | This blocks JS-2 onward. It does **not** block JS-0 or JS-1, which build against the contract as implemented — a distinction the roadmap's delivery order states and this ledger holds it to. |
| ~~**The seed's waited-on set has not been itemised.**~~ **Closed 2026-08-31** by [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md): a dated ruling on each of the five items — one `Wait`, four `Do not wait` — plus a stop condition, **2026-11-30 or 400 further commits on the seed's default branch, whichever comes first**, after which the snapshot is taken as-is and the remaining waited-on item is re-derived on this side of the fork. | This component's architecture owner | Met | The closure removes the open-ended postponement roadmap [section 23](roadmap.gates.md#23-risks-and-stop-conditions) names as a risk. **It does not unblock JS-2**, which still waits on the row above. |

Four further dependencies were **unopened rather than blocked** — an unopened dependency has no
holder and no unblock condition, which is a weaker position than a blocked one, not a stronger
one. **JS-0 opened two of them and left two unopened**, and the table says which is which:

| Unopened dependency | Opened at | If it has not landed |
|---|---|---|
| **OPENED 2026-08-31.** Acquisition of the regular-expression matcher and the Unicode and locale data as this checkout's own dependencies. **Owner: the profile built-ins owner**, named in [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md). Nothing is acquired yet; what changed is that the dependency now has a holder. | Opened at JS-0, consumed at JS-6 | JS-6 excludes every surface needing it and publishes the exclusions, rather than waiting. `broiler.javascript.regexp` is already a separate manifest identity, so the exclusion is a manifest not yet minted rather than a hole in one that is. |
| **The language-specification edition is not pinned, and JS-0 did not pin it.** Retrieving, hashing and archiving a third-party document is a human action; until someone performs it the pin is provisional, and roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) requires a provisional pin to carry a named exclusion here. This row is that exclusion, and it is **still open**: JS-0 was asked to record the intended edition and no decision record does, because recording an edition nobody has retrieved would be a pin in name only. | JS-3a records the pin actually taken | No manifest may be accepted against an unpinned edition, because a conformance total against a moving document is not a total. |
| **The conformance-suite revision is not pinned**, and its licence and attribution obligations are unexamined. The suite is third-party material this component ingests; roadmap [section 22](roadmap.gates.md#22-release-gates) gate 12 makes an attribution obligation discovered during a publish a stop. | JS-3a | The harness cannot start: a branch name is not a pin, and the method requires the revision resolved once before any shard. |
| **This profile's declared defaults are catalog-wide and unreconciled.** Roadmap section 3 records that a host adopting profile defaults gets the tightest in the catalog, so a neighbour's stingy default reaches this profile wherever ceilings are adopted rather than stated, and that reconciling two profiles' declarations belongs to whichever component composes them. That component does not exist and has no owner. **Narrowed 2026-08-31**: the maxima half of this row was retired when the core removed a catalog-wide maximum clamp its own record never authorised. A maximum now binds only the artifacts of the profile that declared it. **PARTLY OPENED 2026-08-31**: [JSD-0004](decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md) records the fifteen defaults and fifteen maxima with the split stated inside the decision, and chooses `NestedLoadDepth`'s default at 4 rather than the 1 this profile would need, precisely so a neighbour adopting defaults is not strangled. **The reconciliation itself is still unowned.** | JS-0 recorded the vectors; the composing component owns the reconciliation | A browser composition that adopts defaults discovers it as a resource exhaustion naming a dimension this profile did not breach, in a verifier that did nothing wrong. |

---

## 4. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle carrying all applicable
fields below. **A command written in a plan is not evidence that the command ran.**

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap and gate revision, core contract version, format version, feature manifest set, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Component commit, recursive submodule revisions, dirty-tree state and patch identity, and the exact paths and projects under test. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, corpus and fixture hashes, the pinned conformance suite revision, and applicable provenance or licence decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets redacted without hiding semantically relevant configuration. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Results** | Raw outputs retained, including failures. A bundle that retains only the passing half is not a bundle. |
| **Negative controls** | Each control, the injection that must make it fail, and the revert that must make it pass. The count is stated and grows across milestones. |
| **Closure** | For any Native AOT claim: the published output's dependency closure, read off the published image rather than asserted. |
| **Exclusions** | What the bundle does **not** show. Every open gate clause, every unexercised path, every single-machine or single-RID limitation, named. |

---

## 5. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, **including its failures and its exclusions**.
3. Do not infer completion transitively. JS-1 acceptance does not accept JS-2; a slice-manifest
   result does not accept a later manifest; and JIT, trimmed, or one-RID success does not accept
   an untested Native AOT or RID claim.
4. Do not promote seed, shell, smoke, analyzer-only, or shape-only results beyond what they prove.
   A failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence
   gathered against a different population is not silently carried forward. A core contract
   amendment is such a change: record the new version and state, per affected record, what
   recertifies unchanged, what must be re-collected, and what is superseded.
6. **Do not record core work here, and never record profile work in the core's ledger.** A core
   result never advances a row in this file, and no row here advances a row there.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and the evidence-bundle ID
   in the affected row. Where owner and reviewer are the same person, record the
   non-independence in the row rather than resolving it by assertion.
8. **Human review gates a release, not a development step.** Development work — implementing a
   milestone, landing it, collecting its evidence — may proceed and merge without a review
   decision. A **release** may not: no package is published, no RID is claimed, no support table
   is issued, and no milestone moves to `Accepted` until a named human has read the work and
   recorded a decision on every relevant code unit, bound to that declaration's fingerprint so a
   unit that changes afterwards reports stale rather than being silently carried.

   Two consequences are worth stating plainly, because this component will feel them harder than
   a greenfield one would. **Unreviewed work accumulates**, and this component starts with a large
   copied body of it: everything the snapshot brings in is unreviewed here on the day it lands,
   and the review debt is real from the first commit rather than from the first release. And **a
   development step that lands unreviewed carries its risk forward rather than dissolving it** —
   a passing conformance run over an unreviewed parser is a statement about the parser's outputs,
   not about the parser.
9. **A copied unit records its origin.** Every unit taken from the seed is annotated as ported,
   and the origin distribution is published in the generated assurance report. A component whose
   report cannot say how much of it was written here is a component whose review status cannot be
   read.
10. **No count, total, graph, commit, or score is copied into prose.** This ledger names the
    command or the retained record that reads it. A number transcribed into a sentence goes stale
    silently, and a ledger that goes stale silently is worse than one with a gap in it.

---

Until such updates are recorded, section 2 remains the complete status of this component: **JS-0
through JS-10 are not started, no source exists, no snapshot has been taken, no language surface
is supported, no composition is advertised, no runtime identifier is claimed, no measurement or
conformance result exists, and nothing has been reviewed.**
