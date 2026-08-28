# Evidence bundle VM-1-002

> **VM-1-001 is superseded and its result is retained here rather than deleted.** It was collected
> against component commit `350a7ba`, and an adversarial review of that commit returned 45 findings
> that survived independent refutation, sixteen of them blockers. Several were confirmed by running
> the code: the aggregate live measure could be driven to zero while memory was still live; an
> operation resumed normally under a spent parent; a declared asynchronous instantiation was
> reported as a profile fault; abandonment leaked a live-suspended slot for the life of the runtime;
> a capability declaring `TerminateOperation` never terminated anything; and a guest could swallow a
> terminal nested exhaustion and report success. VM-1-001 therefore demonstrated that the suite
> passed, not that the contract was implemented. This bundle is collected against the corrected
> tree, and section 6.1, *What the review found*, lists what changed and what is still open.


The retained evidence for milestone VM-1's working claim. It is filed against the eight fields
[the status ledger](../../roadmap.status.md) requires of any status beyond `Not started`, and it is
deliberately explicit about what it does **not** show: ledger update rule 4 forbids promoting a
result beyond what it proves, and roadmap section 16 makes an untruthful support claim a stop
condition.

**This bundle supports `In progress`. It does not support `Accepted`.** `HUMAN_REVIEW.md` is
unsigned and `PENDING`, so no milestone here may be accepted, no package published and no RID
claimed.

VM-1 was built against VM-0's frozen-but-unapproved records, and that is now settled rather than
assumed: the owner ruled on 2026-08-28 that **human review gates a release, not a development
step**, recorded as update rule 8 in the status ledger. Exclusion EX-43 in section 9 is closed by
that ruling. What the ruling does not do is make this bundle acceptance evidence - it is not, and
the last clause of VM-1's own exit gate remains unmet for exactly that reason.

---

## Field coverage

One row per field the status ledger requires, with the section that carries it. The mark is an
evidence verdict about what this bundle records; where a field is only partly recorded, the row
names what is missing.

Mark legend: [`HUMAN_REVIEW.md` section 1](../../../HUMAN_REVIEW.md#1-how-to-use-this-file) is
canonical. Evidence
verdicts are the author's; review verdicts are the reviewer's, and every review verdict in this
file starts `[ ]` because no reviewer has read it.

| Field | Section | Evidence | What is not recorded |
|---|---|---|---|
| Identity | 1 | [MET] | - |
| Source | 2 | [PART] | The component commit is not written as a hash - it is deferred to the change that lands this bundle - and no patch identity is recorded. |
| Dependencies and corpus | 3 | [PART] | No lockfile and no SDK pin, so the toolchain is recorded but not enforced (EX-03). The fixture corpus is generated rather than stored, so it carries no fixture hashes (EX-48). |
| Environment | 4 | [PART] | One RID on one developer workstation, and no CI lane: no arm64, Linux, macOS or second machine (EX-45). |
| Procedure | 5 | [MET] | - |
| Outputs | 6 | [PART] | The packages and the published binaries are not retained, only the logs that record them, and no retention policy is stated. |
| Decision | 7 | [PART] | No reviewer verdict. None exists to record: no reviewer has read this bundle. |
| Validity | 8 | [MET] | - |

## 1. Identity

| Field | Value |
|---|---|
| Milestone | VM-1 - build the semantics-neutral runtime, catalog, and fixture profile |
| Roadmap revision | `docs/roadmap.md` as of component commit `a46d6c8`, unchanged by this work |
| Core contract version | 1, read from the build output as `VmCoreContract.Version` |
| Reason-registry revision | 1, read as `VmReasonRegistry.Revision` |
| Evidence-bundle ID | VM-1-002, superseding VM-1-001 |
| Collection timestamp | 2026-08-28, local time; VM-1-001 was collected 2026-08-27 |
| Owner | MaiRat / Maik Ratzmer, holding all six roles in ADR 0012 |
| Reviewer | **None.** No reviewer has read this work. |

The Reviewer row is empty rather than repeating the owner's name. VM-0's bundle recorded the same
person in both rows and noted that the confirmation was therefore not independent; this bundle does
not record a reviewer at all, because none has looked at it. Ledger update rule 7 requires an owner
*and* a reviewer to confirm every exit condition before `Accepted`, and that has not happened.

## 2. Source

| Field | Value |
|---|---|
| Component commit | recorded by the change that lands this bundle; the tree is otherwise clean |
| Dirty-tree state | The only files modified after the implementation are the logs in this directory and this file, which are this collection's own output |
| Paths under test | `src/Broiler.VM.Abstractions`, `src/Broiler.VM.Binary`, `src/Broiler.VM.Runtime`, `src/tests/Broiler.VM.Fixtures`, `src/tests/Broiler.VM.Fixtures.Host`, `src/tests/Broiler.VM.Contract.Tests`, `src/tests/Broiler.VM.Architecture.Tests` |
| Records under test | `docs/adr/0001` through `docs/adr/0012`; VM-1 amends none of them and files errata against three (see section 7, Deviations) |

## 3. Dependencies and corpus

| Field | Value |
|---|---|
| SDK | 10.0.400 |
| Runtime | Microsoft.NETCore.App 10.0.11 |
| Target framework | net10.0, all seven projects |
| Test packages | Microsoft.NET.Test.Sdk 17.8.0, xunit 2.5.3, xunit.runner.visualstudio 2.5.3 |
| Product package references | none. The three product projects reference no package and no project outside the component |
| Vendored file | `eng/Broiler.Packaging.props`, SHA-256 in `hashes.txt` |
| Corpus | The fixture artifact corpus is generated by `FixtureArtifactWriter`, not stored. It covers a well-formed artifact and seven deliberate corruptions. The malformed-input corpus proper is VM-2's; this is the subset VM-1 needs to show the taxonomy is stable |

No SDK pin exists (Exclusion EX-03, carried forward from VM-0), so the toolchain above is what this
machine happened to resolve, not what the repository enforces.

## 4. Environment

See `environment.txt`. Windows 11 Enterprise on x64, one developer workstation, not a CI lane.

**Not covered:** Linux, macOS, arm64, any second machine, and any RID other than `win-x64`.

## 5. Procedure

Working directory `D:/Broiler.Browser/Broiler.VM`, with `DOTNET_CLI_UI_LANGUAGE=en` so the logs are
readable by a reviewer who does not share this machine's locale.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4 | `dotnet run --project <host> -c Release -- --verbose` | `publish-jit-and-trimmed.log` |
| 5 | `dotnet publish <host> -c Release -r win-x64 --self-contained true -p:PublishAot=false`, then run the produced binary | `publish-jit-and-trimmed.log` |
| 6 | `dotnet publish <host> -c Release -r win-x64 -p:PublishAot=true -p:IlcUseEnvironmentalTools=true`, inside a `vcvars64` environment, then run the produced binary | `publish-aot.log` |
| 7 | Four negative controls, each injected, run, reverted and re-run | `negative-control.log` |

Step 6 needs the `vcvars64` shell and `IlcUseEnvironmentalTools=true` because the ILCompiler
package's own `findvcvarsall.bat` fails on this machine: it cannot locate `vswhere.exe` and emits
its error text into the property that becomes the linker path, so a plain
`-p:PublishAot=true` fails with MSB3073. Exclusion EX-42 records that, because it means the Native
AOT result depends on a step no CI job currently performs.

Step 7 is why the green suite in step 2 means anything. A rule that has never rejected anything
expresses nothing, so each control injects one violation and both runs are retained.

## 6. Outputs

| Artefact | Result |
|---|---|
| `build.log` | 7 projects, Release, **0 warnings, 0 errors** |
| `test.log` | **175 passed, 0 failed, 0 skipped** - 44 architecture and 131 behavioural |
| `pack.log` | exactly **3 `.nupkg` and 3 `.snupkg`** - Abstractions, Binary, Runtime. Neither new test-only project packs |
| `publish-jit-and-trimmed.log` | the fixtures host runs under JIT and as a trimmed self-contained binary of 162,816 bytes; **5 checks passed**, exit code 0, both times |
| `publish-aot.log` | the fixtures host publishes and runs as a Native AOT binary of 1,279,488 bytes; **5 checks passed**, exit code 0. No trim or AOT warnings; the host builds with `TreatWarningsAsErrors` |
| `negative-control.log` | four controls, each failing when injected and green after revert |
| `d1-outcome.txt` | `SCANNED` - an aggregate checkout is present, and 401 project files outside the component were examined |
| `hashes.txt` | SHA-256 of the vendored file, the manifests, every product source file and all thirteen records under `docs/adr/` |
| `rules.register.json` | 38 rules: **33 Active, 1 Vacuous, 4 Deferred** |

### 6.1 What the review found

The implementation was reviewed adversarially against the frozen records before this bundle was
collected: six reviewers, one per contract dimension, and every finding then put to two independent
refuters with instructions to default to refuting. Forty-eight findings were raised, three were
refuted, and forty-five survived - sixteen of them blockers.

They are not listed exhaustively here; the corrections are in the commit and each has a regression
test in `ReviewRegressionTests`. What the reader needs is the shape of them, because it says
something about what a passing suite was worth:

| Class | Examples |
|---|---|
| **Asymmetric accounting** | A retention the parent refused was still released from the parent, so the aggregate live sum could be driven below the true sum across children and then to zero - after which the parent admitted a retention it should have refused. A refused wall-clock charge was silently dropped, permanently under-summing attributed time. |
| **Missing admission checks** | No resume admission check existed at all: an operation resumed normally under a parent with no remaining allowance. A parent whose allowance was fully spent still admitted new runtimes. |
| **Mandatory mappings not implemented** | The outcome-to-instance-state mapping collapsed cancellation, exhaustion and host failure to `Live`, leaving an instance re-invocable after its stack had been abandoned mid-step. |
| **Inverted precedence** | A poll-bound breach was reported ahead of cancellation and exhaustion, blaming the profile for a condition it did not cause and dropping the exhaustion dimension from the diagnostics. |
| **Declarations enforced nowhere** | A capability declaring `TerminateOperation` never terminated: both translation modes reached the profile identically, so a host defect was billed to the guest. The artifact provider ran outside the capability boundary, so its mandatory non-reentrancy was unenforced and its calls were uncharged. A `Value`-kind descriptor could carry a provider, because the registration and the duplicate guard keyed on different fields. |
| **Refusals that left no trace** | The mediator's own nested-load bounds refused without touching the meter, so a profile that ignored the result completed `Normal` - a guest could swallow a terminal exhaustion. |
| **Leaks** | Abandonment never unparked, so a dead operation consumed a live-suspended slot for the life of the runtime. Instantiation took no lease, so a handle backing a live instance went straight to `Disposed` instead of draining. |
| **Rules weaker than their statements** | V4 checked a property count rather than the frozen rows it claimed to check; V9 asserted return types rather than the construction site. Both are corrected, and the register rows now say what the rules do. |
| **Translation that hid a bug** | An escaping verifier exception was translated into `InvalidArtifact`, so a verifier that dereferenced null was indistinguishable from a malicious artifact. It now propagates, and a core-detected verifier breach throws and poisons the runtime. |

**What this says about the first bundle.** VM-1-001 reported a green suite, a clean build, and a
Native AOT binary that ran - and every one of those statements was true while the sixteen blockers
above were present. A passing suite is evidence that the tests pass. It is evidence about the
contract only to the extent that the tests were written to catch the contract being broken, and on
the first pass a great many of them were not.

**Still open after the review.** Twenty-nine findings were majors and minors that this pass did not
address: among them the runtime is still never poisoned by a broken metering contract, the
non-reentrancy gate is absent on the control operations, `VmArtifactLifetimeKind.Disposable`
releases nothing, `Snapshot` never causes the core to copy, and several diagnostics groups are not
populated where the records require. They are recorded as Exclusion EX-52 rather than fixed, and a
reviewer should treat the list as the next piece of work rather than as noise.

### 6.2 The negative controls, in detail

| # | Injected | Expected | Observed |
|---|---|---|---|
| 1 | `Broiler.VM.Runtime` references the test-only `Broiler.VM.Fixtures` | A4 and A7 fail | 2 failed, 42 passed; 44 passed after revert |
| 2 | An edge the checkout **has** is deleted from `graph.manifest.json` | A7 fails on a missing edge as on an extra one | 1 failed, 43 passed; 44 passed after revert |
| 3 | A struck name, `VmHandle`, is exported from a product assembly | V3 fails | 1 failed, 43 passed; 44 passed after revert |
| 4 | The mediator's no-provider refusal is removed | the behavioural suite fails | 1 failed, 130 passed of 131; 131 passed after revert |

**Control 4 did not fail on its first run, and that is retained rather than quietly fixed.** With
the refusal removed, a null provider threw, the exception was translated to a host failure, and the
fixture profile still reported a profile fault - so a test asserting only the outcome category
passed. The tests were asserting the profile's *reaction* rather than the core's *reason*. Four
guest-load tests were strengthened to assert the reason the core produced, carried in the fixture's
own fault payload, and the control then failed as it should. A negative control that passes is a
finding about the suite, and the finding was that four assertions were one level too shallow.

## 7. Decision

**Expected gate:** roadmap section 13's VM-1 objective exit gate.

**Actual result, clause by clause.** The gate names sixteen things; each is listed with what was
observed, and the two that are not fully discharged say so.

Each clause carries a stable ID, `G-01` to `G-16`, in the order the gate names them. The
**Evidence** column is the author's verdict and says the same thing as the prose in **Result**. The
**Review** column is the reviewer's, one cell per clause, so a clause can be disagreed with
singly; every cell is `[ ]`.

| ID | Evidence | Review | Gate clause | Result |
|---|---|---|---|---|
| G-01 | [MET] | [ ] | deterministic registration | Shown. A descriptor registers, is retrievable by exact ordinal identity, and is not retrievable by a folded one |
| G-02 | [MET] | [ ] | duplicate and alias rejection | Shown. A duplicate ID and a case-confusable ID are both thrown at `Add`, naming the offending entry |
| G-03 | [MET] | [ ] | unknown-profile and unsupported-version failures | Shown. An absent profile is `UnsupportedProfile`/`ProfileNotInCatalog` and never `InvalidArtifact`; an unsupported format version and an unaccepted manifest are `InvalidArtifact` with distinct reasons; a descriptor built against a future contract version is refused at composition |
| G-04 | [MET] | [ ] | catalog-order independence | Shown at the byte level. Two catalogs built from the same descriptors in either order produce byte-identical canonical encodings |
| G-05 | [MET] | [ ] | per-runtime state isolation | Shown. Two runtimes over one catalog produce independent instances and identities; a handle whose ceilings differ from the receiving runtime is refused with the clause-8 reason |
| G-06 | [MET] | [ ] | legal and illegal lifecycle transitions | Shown. Use-after-dispose on the runtime, the instance and the handle each answer `InvalidState` with the right reason; a second invocation while one is suspended is refused; disposal is idempotent |
| G-07 | [MET] | [ ] | cancellation and disposal behaviour | Shown. Cancellation is observed at a polling point and reported as `Cancellation` rather than as a profile fault; the request latch is monotonic; a handle with a live lease drains rather than being seized |
| G-08 | [PART] | [ ] | declared thread affinity and reentrancy | **Partly.** Reentrancy is enforced on the execution path and witnessed, and the artifact provider now runs inside the same boundary. It is still absent on the control operations (EX-52), and thread affinity is declared and carried but never exercised across threads (EX-44) |
| G-09 | [MET] | [ ] | typed profile-payload preservation | Shown. A fixture value and a fixture fault both round-trip through the neutral envelope; a payload outside its profile's declared kind range is dropped rather than handed on |
| G-10 | [MET] | [ ] | the explicit absence of reflection or name-based discovery | Shown. Rule B5 over compiled metadata, plus a surface check that no public member takes a `Type` or an `Assembly` |
| G-11 | [MET] | [ ] | a guest-initiated load through a fixture provider | Shown. A declaring profile loads through a registered provider, and the provider is asked exactly once |
| G-12 | [MET] | [ ] | deterministic refusal where no provider is registered | Shown, and hardened by control 4. Two invocations produce the same outcome and the same reason, and the reason is `ProviderNotRegistered` |
| G-13 | [MET] | [ ] | external suspension and resume | Shown. The double gate refuses with `ExternalSuspensionNotDeclared` and `ExternalSuspensionNotEnabled` respectively; guest suspension resumes through the single resume entry point; a suspension object is single-use and runtime-bound; disposing a handle holding an untaken external suspension latches cancellation |
| G-14 | [MET] | [ ] | aggregate budget exhaustion across several runtimes | Shown. Two runtimes under one parent cannot together spend more than the parent's ceiling, and the refusal names aggregate scope; the live-runtime ceiling, sealing, and refusal to dispose a parent with live children are all exercised |
| G-15 | [MET] | [ ] | trimmed and Native AOT test hosts | Shown. One composition-root host published and **run** in three modes - JIT, trimmed self-contained, Native AOT - composing two fixture profiles through the generic contract, 5 checks passing in each |
| G-16 | [UNMET] | [ ] | the accepted contract recorded with its version | **Not met.** The contract is *implemented* and its version is recorded in code, in every diagnostics record and in every verified handle. It is not *accepted*: no reviewer has read it |

**Unexplained failures:** none.

**Deviations from the frozen records.** Three, each recorded here rather than silently absorbed:

1. **`VmControlResult` is a struct, not an enum.** ADR 0003's name table records it as
   `enum {Accepted, NoOp, InvalidState, Unsupported}`; ADR 0004 requires it to carry exactly one
   reason code and ADR 0009 requires it to distinguish an undeclared external suspension from an
   unenabled one. A bare enum cannot do both. The four frozen members are preserved as
   `VmControlOutcome` and the carrier is a struct. Filed as an erratum against the name table.
2. **Stage results are constructed through hidden public factories, not internal constructors.**
   ADR 0005 says construction is `internal`; rule A10 forbids `InternalsVisibleTo`; and a profile
   package must be able to name the result types, so they live in the contracts assembly while the
   runtime constructs them. The compromise is a `public static` factory per legal cell, hidden from
   IntelliSense, with **no factory at all for an illegal cell** - so the matrix stays a
   compile-time fact - and rule V9 asserting the single construction site for the verified handle.
3. **`VmOperation` is not exported.** ADR 0003's table lists it among ADR 0004's lifecycle objects.
   VM-1 realises the operation as an internal runtime object addressed publicly through
   `VmOperationControlHandle` and `VmOperationStateSnapshot`, so the frozen name is used but not
   public. Exclusion EX-41.

**The claim this bundle justifies, stated narrowly:** *the profile-neutral contract of core
contract version 1 is implemented; fourteen of the exit gate's sixteen clauses are demonstrated
against two fixture profiles in JIT, trimmed and Native AOT hosts on `win-x64`, a fifteenth is
demonstrated in part, the sixteenth is not met; and the
implementation has been adversarially reviewed once against the frozen records, with every
surviving blocker corrected and regression-tested.* It justifies no claim about any other RID,
about concurrency, about performance, about any language, or about acceptance - and, given that a
single review pass found sixteen blockers behind a green suite, no claim that a second pass would
find none.

**Reviewer verdict:** none recorded. **Follow-up owner:** MaiRat.

## 8. Validity

**Reproduction.** Clone the component, run steps 1 to 3 above. For step 6, open a `vcvars64`
shell first. The negative controls are reproduced by the script in the change that landed this
bundle; each mutates one file, runs the suite, and reverts.

**Expiry.** This bundle expires when any of the following changes: the source, the SDK or runtime,
the core contract version, the reason-registry revision, the package graph, the public API surface,
the Native AOT settings, the RID matrix, or the fixture profiles. It is already expired for any
claim about VM-2 through VM-6, which it does not address.

**Recertification triggers.** A core contract amendment; a change to the frozen public-name table;
a new architecture rule or a status change to an existing one; a second RID; a CI lane.

### 7.1 Corrections to this bundle after collection

Two figures in this bundle disagreed with the logs it retains. They are corrected above and
recorded here, because a retained bundle that is edited without saying so is worth less than one
that was wrong.

| Date | Corrected | Was | Is | Authority |
|---|---|---|---|---|
| 2026-08-28 | Section 6.2, negative control 4 | `1 failed, 105 passed; 106 passed after revert` | `1 failed, 130 passed of 131; 131 passed after revert` | `negative-control.log`, the `CONTROL 4` block. The old figures were the behavioural suite's size before the review-regression tests were added, and they contradicted this bundle's own 131. |
| 2026-08-28 | Section 7, the narrow claim | `fifteen of the exit gate's sixteen clauses are demonstrated` | `fourteen ... a fifteenth is demonstrated in part, the sixteenth is not met` | The clause table in the same section, which marks G-08 `[PART]`. Counting a partial clause as demonstrated is what ledger update rule 4 forbids. |

Neither correction changes a log, and no log was re-collected. Both make the prose agree with
evidence that was already retained here.

## 9. Exclusions

Each is a named limit on what this bundle shows. Carried-forward exclusions keep their VM-0
identifiers. **Status** is `Open` where the limit still stands and `Closed` where a dated decision
has discharged it; a closed exclusion is retained, not deleted.

| ID | Status | Exclusion |
|---|---|---|
| EX-01 | Open | The inbound half of the legacy-boundary rule is environment-conditional. It reports `SCANNED` here because an aggregate checkout is present; in a standalone clone it reports `INCONCLUSIVE` and proves nothing. |
| EX-03 | Open | No SDK pin exists. The toolchain is what this machine resolved. |
| EX-11 | Open | Seventeen roadmap amendments remain proposed and unapplied, so `docs/roadmap.md` and the records still disagree where ADR 0003's register lists. VM-1 implements the records. |
| EX-40 | Open | Rule B3 stays `Vacuous`, with its activation milestone moved to VM-3. Its subject now exists, but a violation remains unreachable by construction rather than merely absent: A1 forbids the outbound project reference, A2 the package-shaped one, and the single-source `NuGet.config` makes a foreign `Broiler.*` package unresolvable. Marking it `Active` would claim the suite had rejected something it cannot construct. |
| EX-41 | Open | `VmOperation` is a frozen public name that VM-1 does not export. See Deviations. |
| EX-42 | Open | The Native AOT publish requires a `vcvars64` environment and `IlcUseEnvironmentalTools=true`, because the ILCompiler package's own toolchain discovery fails on this machine. No CI job performs that step, so this result is not currently reproducible by automation. |
| EX-43 | Closed | **Closed 2026-08-28.** VM-0 is unaccepted and `HUMAN_REVIEW.md` is unsigned, and VM-1 was built anyway. The owner has since ruled that human review gates a release rather than a development step (ledger update rule 8), which makes that legitimate rather than merely recorded. The exclusion is retained as dated history: what it flagged was real until the ruling was made, and a reader comparing this bundle against the VM-1 gate should still see that no review has happened. |
| EX-44 | Open | **Concurrency is not tested.** Declared thread affinity is carried in the descriptor and in runtime identity but no test runs two threads. Reentrancy is enforced and witnessed; affinity is not. Stress, soak and concurrency evidence is VM-4's, and nothing here anticipates it. |
| EX-45 | Open | **One RID, one machine, one lane.** `win-x64` only, on a developer workstation. No arm64, no Linux, no macOS, no second machine, and no CI. |
| EX-46 | Open | **No performance claim.** Nothing here is a measurement. Image sizes are recorded as facts about two binaries, not as baselines; VM-5 owns decision-grade measurement and none of its rules were followed. |
| EX-47 | Open | **The persisted envelope is absent, not implemented.** Stage S2 is admitted by the contract and no public member can enter it. Its invariant 8 discharge is that absence, which rule V10 asserts. |
| EX-48 | Open | **The malformed-input corpus is a subset.** Seven deliberate corruptions, generated rather than stored, and no fuzzing. The corpus, the fuzz targets and the minimized regressions are VM-2's. |
| EX-49 | Open | **The catalog identity is not a hash.** It is the canonical encoding itself, compared by bytes. That is enough for order-independence and drift detection and is not a content-addressing scheme. |
| EX-50 | Open | **The application-local consumer profile does not exist.** Both fixture profiles live in the same test-only assembly and use the reserved `Broiler.*` namespace. Proving the public contract from a separate consumer package under a reverse-domain namespace is VM-3's, and this bundle establishes nothing about it. |
| EX-51 | Open | **Rule V9 does not count call sites.** It asserts that one public member mints a verified artifact and that one product assembly reaches it, both from metadata. Counting individual call sites would need an IL decoder, and a hand-written one that got an operand length wrong would make the rule worse than the proxy it replaced. The register row says what the rule does. |
| EX-52 | Open | **Twenty-nine review findings are unaddressed.** The sixteen blockers and a handful of majors were fixed; the rest were not. They include: the runtime is never poisoned by a broken metering contract, so `VmRuntimeState.Poisoned` is reachable only through a verifier contract breach; the non-reentrancy gate is absent on `Dispose`, `RequestCancel` and `PollDeadlines`; `VmArtifactLifetimeKind.Disposable` releases nothing; `VmArtifactRepresentationKind.Snapshot` never causes the core to retain bytes; `TryTakeSuspension` never answers `Unsupported` or `NoOp`; binding-failure diagnostics do not name the full triple; the four core-contract admission rules report reasons that do not distinguish all four cases; and a diagnostics token under another profile's namespace is accepted. None is a silent-wrong-answer defect of the kind fixed above, but each is a place where the implementation is thinner than the record it implements. |
