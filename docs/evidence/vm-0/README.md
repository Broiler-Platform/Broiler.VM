# Evidence bundle VM-0-001

The retained evidence for milestone VM-0's working claim. It is filed against the eight fields
[the status ledger](../../roadmap.status.md) requires of any status beyond `Not started`, and it
is deliberately explicit about what it does **not** show: ledger update rule 4 forbids promoting
a shell result beyond what it proves, and roadmap section 16 makes an untruthful support claim a
stop condition.

This bundle supports `In progress`. It does not support `Accepted`, and no dependent milestone
may treat VM-0 as accepted on the strength of it.

## Identity

| Field | Value |
|---|---|
| Milestone | VM-0 - freeze ownership, terminology, and the build-proven graph |
| Roadmap revision | `docs/roadmap.md` as of component commit `6235603` |
| Core contract version | 1, read from the build output as `VmCoreContract.Version` |
| Evidence-bundle ID | VM-0-001 |
| Collection timestamp | 2026-08-27, local time, single session |
| Owner | **VACANT** - no person is assigned to any Broiler.VM role |
| Reviewer | **VACANT** |

The vacant owner and reviewer are not an oversight in this bundle; they are the milestone's
unmet dependency. Roadmap section 13 lists "Named ownership for the core contract and its
amendments" as VM-0's only dependency, and it is not satisfied.
[ADR 0012](../adr/0012-security-ownership-and-support-matrix.md) records the six roles and what
each blocks.

## Source

| Field | Value |
|---|---|
| Component commit | `6235603ede34bc30089b366c16db10831e72e807` |
| Dirty-tree state | **Dirty.** Every artefact this bundle describes is uncommitted at collection time. |
| Paths under test | `src/Broiler.VM.Abstractions`, `src/Broiler.VM.Binary`, `src/Broiler.VM.Runtime`, `src/tests/Broiler.VM.Fixtures`, `src/tests/Broiler.VM.Architecture.Tests` |
| Records under test | `docs/adr/0001` through `docs/adr/0012` and `docs/adr/README.md` |

A dirty tree is recorded rather than hidden. The results below were produced from the working
tree, so the bundle is reproducible only against the commit that lands these files, and it must
be re-collected against that commit before anyone relies on it.

## Dependencies and corpus

| Field | Value |
|---|---|
| SDK | 10.0.400 |
| Runtime | Microsoft.NETCore.App 10.0.11 |
| Target framework | net10.0, all five projects |
| Test packages | Microsoft.NET.Test.Sdk 17.8.0, xunit 2.5.3, xunit.runner.visualstudio 2.5.3 |
| Vendored file | `eng/Broiler.Packaging.props`, byte-identical to `Broiler.DOM/eng/Broiler.Packaging.props`, SHA-256 in `hashes.txt` |
| Corpus | None. VM-0 has no artefact corpus; the malformed-input corpus is VM-2's. |

No SDK pin exists (Exclusion EX-03), so the toolchain above is what this machine happened to
resolve, not what the repository enforces.

## Environment

See `environment.txt`. Windows 11 Enterprise 10.0.26200 on x64, one machine, JIT only.

**Not covered:** Linux, macOS, arm64, trimming, Native AOT publish, and any second machine. VM-0
claims no RID and no AOT result; roadmap invariant 7 requires publish-and-run evidence for those
and none was collected.

## Procedure

Working directory `D:/Broiler.Browser/Broiler.VM`, with `DOTNET_CLI_UI_LANGUAGE=en` so the logs
are readable by reviewers who do not share this machine's locale.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4 | Negative control: inject `Broiler.VM.Runtime -> Broiler.VM.Fixtures`, re-run step 2, revert, re-run step 2 | `negative-control.log` |

Step 4 is the reason the green suite in step 2 means anything. The injected edge is a product
project referencing a test-only project: forbidden, but acyclic, so the rules have to catch it
rather than MSBuild refusing to restore. Two earlier attempts at this control were discarded and
are recorded here because a discarded control is evidence too:

- injecting the edge with `sed` mangled the Windows relative path into one that resolved
  nowhere, so only the catch-all edge rule fired and the containment rule never saw a real
  violation; and
- injecting `Broiler.VM.Abstractions -> Broiler.VM.Fixtures` created a genuine cycle, which
  MSBuild rejects at restore, so no test ran at all. That is a stronger result for that
  particular edge and a useless one for the general rule.

## Outputs

| Artefact | Result |
|---|---|
| `build.log` | 5 projects, Release, **0 warnings, 0 errors** |
| `test.log` | **33 passed, 0 failed, 0 skipped** |
| `pack.log` | exactly **3 `.nupkg` and 3 `.snupkg`** - Abstractions, Binary, Runtime. `Broiler.VM.Fixtures` did not pack. |
| `negative-control.log` | injected forbidden edge -> **A4 and A7 fail**; after revert, 33 passed |
| `hashes.txt` | SHA-256 of the vendored, generated and contract-bearing files |
| `../../src/tests/Broiler.VM.Architecture.Tests/rules.register.json` | 28 rules: **19 Active, 6 Vacuous, 3 Deferred** |

Observed, not asserted: the produced `Broiler.VM.Runtime.nuspec` declares dependencies only on
`Broiler.VM.Abstractions` and `Broiler.VM.Binary`, and no produced package's metadata names a
language. Those are the conditions rules C1 to C3 will check when a pack step exists; at VM-0
they are an observation about a manual run, not a gate (Exclusion EX-04).

## Decision

**Expected gate:** roadmap section 13's VM-0 objective exit gate.

**Actual result:** the acyclic shell graph builds; the architecture tests run and pass; the
twelve boundary ADRs are written and bound to the code by tests E1 to E4; core contract version 1
is assigned and carried by `VmCoreContract`; the amendment procedure is published; the four
explicit decisions and the three embedding decisions each carry a recorded ruling with its
reasoning; and verification is recorded as separable from execution and required to stay so.

**The claim this justifies, in the words the ledger and the ADRs use:** *every forbidden edge in
the VM-0 shell graph is expressed and witnessed; nine rules await their subject and are
registered in rules.register.json.* The unqualified sentence "architecture tests express every
forbidden edge" is not claimed anywhere, because six rules are Vacuous and three are Deferred.

**Exclusions.** Thirty-four are declared across the ADRs and carried here by identifier; each is
stated in full in its owning record. The ones that most limit this bundle:

- **EX-01** - the inbound half of the legacy-boundary rule is decidable only when an aggregate
  checkout is present above the component. Rule D1 ran and passed here because one is; a
  standalone checkout reports inconclusive, not pass. Closed by: a check owned by the aggregate
  repository, recommended and not taken at VM-0.
- **EX-05** - 9 of 28 rules await their subject and assert nothing about behaviour that does not
  exist yet. Closed by: VM-1 for the six Vacuous rules, VM-6 for the three Deferred ones.
- **EX-03** - no SDK pin exists; this bundle is reproducible only against the recorded SDK
  version, not enforced by the repository.
- **EX-11** - VM-0 proposes but does not apply 17 roadmap amendments. The register in ADR 0003
  is a list of proposals; `roadmap.md` is unchanged except for one stale status sentence.
- **EX-21** - no architecture rule asserts any transition, member, state or category named in
  ADRs 0002 through 0011. Those are paper decisions; nothing in the shell graph implements or
  checks them. Closed by: VM-1.
- **EX-30** - the six ownership roles are vacant, so VM-0's dependency is unmet and no one is in
  a position to accept it.
- **EX-31** - no public support table exists; section 15 gate 1 is unmet. Closed by: VM-6.
- **EX-02** - section 15 gate 2 is not claimed. It is a release gate.

**Unexplained failures:** none. **Deviations:** none beyond the discarded controls recorded under
Procedure.

**Reviewer verdict:** none. No reviewer is assigned.

**Follow-up owner:** unassigned. The next action is not technical: it is naming the six roles in
[ADR 0012](../adr/0012-security-ownership-and-support-matrix.md).

## Validity

**Reproduction.** Check out the commit that lands these files, then run steps 1 to 4 above from
the component root. Step 4 needs the injection script; it is not retained in the repository, and
the edge it adds is described precisely enough above to recreate by hand in one line of XML.

**Expiry.** This bundle ages against its own environment. Re-collect on any change to: the
component source, the SDK or runtime, the core contract version, the package graph, the rule
register, the ADR set, or the vendored packaging props.

**Recertification triggers.** A core contract amendment recertifies nothing automatically: under
ledger update rule 5 it requires recording, per affected record, what recertifies unchanged, what
must be re-collected, and what is superseded. Because every result here is JIT-only and
single-platform, any claim about another RID, about trimming, or about Native AOT requires a new
bundle rather than an extension of this one.
