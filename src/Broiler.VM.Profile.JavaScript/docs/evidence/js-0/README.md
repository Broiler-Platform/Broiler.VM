# Evidence bundle JS-0-001

**Milestone:** JS-0 - boundary, placement, identity, and the assurance floor.

**Verdict this bundle supports:** JS-0 is **In progress**, not accepted. Two of its exit-gate
clauses are open and named in the exclusions below, and **no human has reviewed anything**, which
the roadmap makes a precondition for `Accepted` on any milestone.

This bundle was produced by `eng/collect-js-evidence.py`, which is in the repository. Every file
beside this one is its output. **A command written in a plan is not evidence that the command
ran**; the logs are what ran.

**No result from any other component is evidence here.** No figure, total, conformance result,
benchmark or Native AOT sample from the Broiler.VM core, from the legacy JavaScript engine, or
from any other component appears in this bundle or is cited by it.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-0-001` |
| Milestone | JS-0 |
| Core contract version | 1 (implemented; **not accepted** - see exclusions) |
| Format version | none. No format exists |
| Feature manifest set | empty. No manifest is minted |
| Owner | profile architecture owner |
| Reviewer | **none** |
| Collected | see `identity.txt` |

Owner and reviewer are the same person for every role this milestone names, and the roadmap
requires the non-independence to be recorded rather than resolved by assertion. This is that
record: **no decision in this bundle was reviewed by anyone who did not make it.**

## Source

`identity.txt` carries the component commit, the branch, and whether the working tree was clean,
with every dirty entry listed rather than counted. `snapshot-identity.txt` re-derives the
candidate seed identity of [JSD-0005](../../decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md)
from the aggregate checkout - the seed commit and its three nested submodule revisions - and
reports match or mismatch per row. **All four matched.**

**Read that clause precisely.** The revisions are read from the aggregate repository's own
gitlinks, which is a source independent of the record they are compared against, so the record is
shown to be reproducible rather than merely self-consistent. It is **one checkout on one
machine**: nobody has cloned this repository a second time and re-derived the identity there, and
no snapshot has been taken.

## Dependencies and corpus

None. There is no corpus, no fixture, no pinned conformance suite and no satellite dependency
resolved. `hashes.txt` records a SHA-256 for every file this milestone's claims rest on: the
solution, the three project files, the three assembly markers, the rule register, the graph
manifest, the four roadmap documents and the seven decision records.

## Environment

`environment.txt`: OS, machine, and the full `dotnet --info`. **One machine, one RID.** No RID is
claimed by this milestone and none may be inferred from this bundle.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite, warnings as errors | `suite.log` | 207 contract tests and 127 architecture tests passed, 0 failed |
| Assurance gate mode | `assurance-gate.log` | Passed: every generated artefact is byte-identical to what the generator would write |
| Assurance release mode | `assurance-release.log` | **Refused, as it must.** Exit 1, rule J11, naming each blocking declaration individually with its file, line and unit |
| Negative controls | `negative-controls.log` | 8 injected, 8 failed the suite while injected and passed after revert |

The architecture suite grew from 122 tests to 127: the five that assert group N and A11's
sibling exemption.

## Negative controls

Eight, each an injection into the real checkout followed by a revert, each judged by the whole
suite rather than by the rule in isolation:

| Control | Proves |
|---|---|
| `N1-profile-references-the-runtime` | ADR 0011 P1 is enforced over the product profile, and the graph manifest catches the edge too |
| `N1-profile-references-the-lowering` | The execution-only property is a rule, not a convention |
| `N3-format-is-not-a-sink` | The format pivot cannot acquire an edge silently |
| `N4-family-project-declares-a-package-id` | The "nothing here is packable" claim cannot decay into prose |
| `N4-family-project-omits-ispackable` | An omission is caught, not defaulted to packable |
| `N2-a-non-family-project-references-the-profile` | The **inbound** half of the no-edge-to-another-profile rule, which would otherwise be satisfied from the side that never changes |
| `J3-a-profile-fingerprint-is-stale` | **The assurance system reaches the three new assemblies** rather than merely listing them |
| `J4-a-profile-unit-claims-a-reviewer` | The generator refuses to write a reviewer no source line carries, over a profile unit |

**The running count of negative controls in this component's JS series is 8**, and it grows at
every milestone.

## What this milestone inherits rather than re-implements

Four of JS-0's gate clauses are discharged by rules this component already had, which now sweep
the profile's three projects along with everything else. **Inheriting a rule is not the same as
having no rule, and it is not the same as writing one either**, so each is named with what
actually covers it:

| Clause | Covered by | Its witness |
|---|---|---|
| Both halves of the legacy-boundary rule | **A1**, **A2**, **A3** and **B3** outbound; **D1** inbound | Each has its own witness input; D1's is materialised outside the component root and run through the whole pipeline. `docs/evidence/vm-6/d1-outcome.txt` records that D1 SCANNED rather than reporting inconclusive on this machine |
| No build item resolves outside the component root, and an unresolvable one is reported rather than skipped | **A1** and **A3**, which treat an unresolvable path as a violation of its own kind rather than clearing it | `A1-outbound-project-reference`, `A3-shared-source-link`, `A3-property-shared-source-link` |
| The API baseline compares in both directions, an injected member failing and a deleted member failing | **M1** | Four inputs under `witnesses/api/` |
| The assurance generator is a fixed point, and refuses to invent a reviewer | **J4**, **J5**, **J8**, **J9** | Inputs under `witnesses/assurance/`, plus this bundle's own two controls over a **profile** unit |

**The legacy-boundary rules are the load-bearing inheritance**, because the fork from the legacy
JavaScript engine is the boundary this profile exists behind. They are rules about the whole
component and they were already sweeping every project in it; the three new projects entered
their subject on the commit that created them, and control
`N1-profile-references-the-runtime` shows the group A sweep failing over a profile project rather
than only over a witness.

## Closure

**None, and none is claimed.** No composition root exists, nothing was published, and no Native
AOT or trimmed output was produced. JS-0 claims no RID, advertises no composition and packs
nothing.

## Exclusions - what this bundle does not show

1. **The two-profile catalog test is not run.** JS-0's exit gate asks for this profile's
   descriptor composed beside a deliberately hostile neighbour, proving a neighbour's maxima do
   not reach this profile's artifacts and its adopted defaults do. **There is no descriptor**:
   the delivery order says JS-0 lands no product code, and JS-1 builds the first descriptor. The
   clause is carried to JS-1, and its `eval`-refusal half needs guest-initiated loads and is
   carried to JS-8. **This is an open gate condition, not a passed one.**
2. **The public API baseline does not cover the profile's assemblies.** Rule M1's subject is the
   packable set, and these three are deliberately unpackable. At JS-0 they export no public type,
   so nothing is unfrozen today; **JS-1 lands a public surface and owns extending the baseline**.
   Recorded in [JSD-0006](../../decisions/0006-assurance-evidence-and-rules-adoption.md).
3. **Rule N2's cross-family half has a witness but no negative control.** A second profile family
   does not exist in this graph, so an injected edge would name a project that is not there and
   the build would fail before any rule ran - the suite would go red for the wrong reason.
   Constructible when the WebAssembly profile's own equivalent milestone lands. **The inbound half
   does have a control**, so the rule is not uncontrolled, only half-controlled.
4. **The release-mode log's blocker list is truncated by the test runner** before it reaches the
   profile's own units. The gate refuses and names declarations individually, which is what the
   clause asks; that it reaches the profile's three assemblies is proved by controls
   `J3-a-profile-fingerprint-is-stale` and `J4-a-profile-unit-claims-a-reviewer` rather than by
   this log.
5. **The core contract is implemented but not accepted.** Every core milestone is in progress and
   the core's review record is unsigned. JS-0 and JS-1 build against the contract as implemented,
   which is legitimate; **JS-2 onward is blocked** on the acceptance, and this component does not
   hold that blocker.
6. **The seed's own gate state was not re-run.** JSD-0005 records the roadmap's claim that a
   repository gate in the seed is red at the candidate commit. Nobody has re-run the seed's suite
   from this component, so that defect is inherited from a reading rather than independently
   confirmed. JS-2 owns confirming it.
7. **One machine, one OS, one architecture.** Nothing here is evidence about any other.
8. **The licence-and-notice clause is met in the only way it can be at JS-0.** The component's
   `LICENSE` carries the Apache-2.0 text, and `THIRD_PARTY_NOTICES.md` was amended in this change
   to restate the scope of its no-vendored-code claim now that the profile lives in this
   repository, and to record the obligation that lands with the copy. **There is no upstream
   derivation to carry and no modified file to mark, because JS-0 copies nothing.** Both become
   real at JS-2, and the notice names them there rather than asserting them here.
9. **The `N2` cross-family control could not be constructed**, only its inbound half. See
   `negative-controls.log`'s stated limit.
10. **Nothing is reviewed.** Every relevant unit in this component - the profile's three among
    them - is `HUMAN_PENDING`. The assurance system records the absence of review precisely; that
    is its value and it is not a claim of safety.
