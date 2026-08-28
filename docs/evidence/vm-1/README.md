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
| `rules.register.json` | 52 rules: **47 Active, 1 Vacuous, 4 Deferred** |

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

Six entries have been corrected after collection. Two were figures that disagreed with the logs
this bundle retains. The other two are counts of a checkout file, not of a log, and each changed
after the logs were taken rather than having been wrong when written. All four are corrected above
and recorded here, because a retained bundle that is edited without saying so is worth less than
one that was wrong.

| Date | Corrected | Was | Is | Authority |
|---|---|---|---|---|
| 2026-08-28 | Section 6.2, negative control 4 | `1 failed, 105 passed; 106 passed after revert` | `1 failed, 130 passed of 131; 131 passed after revert` | `negative-control.log`, the `CONTROL 4` block. The old figures were the behavioural suite's size before the review-regression tests were added, and they contradicted this bundle's own 131. |
| 2026-08-28 | Section 7, the narrow claim | `fifteen of the exit gate's sixteen clauses are demonstrated` | `fourteen ... a fifteenth is demonstrated in part, the sixteenth is not met` | The clause table in the same section, which marks G-08 `[PART]`. Counting a partial clause as demonstrated is what ledger update rule 4 forbids. |
| 2026-08-28 | Section 6, the `rules.register.json` row | 38 rules, 33 of them Active | 43 rules, 38 of them Active | The register itself, which now carries the five group H rules that hold the review documents to the marks, identifiers and figures they quote. The old count was true of the tree these logs were collected against; section 8 names a new architecture rule as a recertification trigger, and this is one. |
| 2026-08-28 | Section 6, the `rules.register.json` row, a second time | 43 rules, 38 of them Active | 48 rules, 43 of them Active | The register itself, which now also carries the five group J rules that hold the Broiler Code Assurance record - the per-unit review annotations, the generated file headers and `CODE-ASSURANCE.md` - to the code it describes. Same reason as the row above, and the same recertification trigger. |
| 2026-08-28 | Section 6, the `rules.register.json` row, a third time | 48 rules, 43 of them Active | 49 rules, 44 of them Active | The register itself. Two adversarial passes over the group J implementation returned eighteen defeats, seven of them blockers; the repair added rule J6, which forbids a preprocessor directive in any covered source file, because a directive is trivia and no fingerprint records one. The same pass moved the unit counts in EX-61 and EX-62 - the exemption predicate was narrowed in three places and initialized constants became code units, so 496 units are now relevant where 463 were. Same recertification trigger as the two rows above. |
| 2026-08-28 | Section 6, the `rules.register.json` row, a fourth time | 49 rules, 44 of them Active | 50 rules, 45 of them Active | The register itself. A third adversarial pass brought the running total to 33 defeats, and nearly every blocker lived in one place - the exemption predicate, where a unit answered EXEMPT carries no annotation, therefore no fingerprint, therefore no record of any kind. Patching the predicate case by case had failed three times because each fix moved the same defeat one case over. The repair is structural rather than another patch: rule J7 holds a new generated artefact, `assurance.manifest.json`, to EVERY code unit in the product tree, exempt and relevant alike. Whether a unit needs a human annotation is still the predicate's question; whether it is watched for change is no longer a question. The same pass widened the unit set to every field declaration and narrowed three more predicate cases, so 543 units are now relevant across 1,169 where 496 across 1,034 were, which moves the counts in EX-61 and EX-62 again; and it added EX-67 and EX-68. Same recertification trigger as the three rows above. |
| 2026-08-28 | Section 6, the `rules.register.json` row, a fifth time | 50 rules, 45 of them Active | 52 rules, 47 of them Active | The register itself. A fourth adversarial pass found seven blockers in two groups. The first group attacked the unit ENUMERATION rather than the exemption predicate: a unit exists only for a declaration kind the scanner names, so an enum member, a type declaration header carrying a primary constructor, an event field declaration and an `[assembly: InternalsVisibleTo]` were each in no unit, no fingerprint and no manifest entry, with the suite green and `assurance.manifest.json` byte-unchanged. The first three are units now; the fourth is a member of nothing and can never be one, so the manifest gained a `files` array - one fingerprint per covered file over the complete token stream of its compilation unit - and rule J7 holds it to the tree beside the units. The second group attacked the GENERATED ARTEFACTS: two lines appended to the manifest's own header made it assert that every unit was verified, human-reviewed and eligible for release with both modes green, and nothing in the suite read that sentence. Rules J8 and J9 are the answer - J8 holds every generated artefact to a hand-maintained shape and re-derives every value, J9 forbids review vocabulary the annotations do not support - and the same pass widened the unit set again, so 689 units are now relevant across 1,592 where 543 across 1,169 were. That moves the counts in EX-61, EX-62, EX-63 and EX-67, and adds EX-69, EX-70 and EX-71. Same recertification trigger as the four rows above. |

No correction changes a log, and no log was re-collected. The first two make the prose agree with
evidence that was already retained here; the last four record changes to the checkout the bundle
describes, which is why they are written down rather than silently overwritten.

**What these logs do not cover at all.** The Broiler Code Assurance system landed after this
collection: every product source file now carries a generated header, every relevant unit carries a
review annotation, and `CODE-ASSURANCE.md` and `assurance.manifest.json` are generated in the
component root. None of that is in `test.log`, `build.log` or `hashes.txt`, and the hash of every
product source file has changed because every one of them gained comment lines. Nothing in the
change alters a single executable statement - the diff over the three product assemblies is comment
lines and nothing else - but a reader must not take this bundle's hashes as current. That is
section 8's expiry clause doing its job, and the recertification is a later step.

`test.log` is also older than the architecture suite it records. It retains **175 passed, 0 failed,
0 skipped - 44 architecture and 131 behavioural**, which is what the suite was when the collection
ran; the architecture assembly has grown since, first with the group H and group J rules and again
with rule J7 and the tests around it. The figures quoted throughout this bundle are the figures in
the retained logs, deliberately and per EX-54, and they are not a claim about the current
checkout's suite size.

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
| EX-53 | Open | **The group H and group J rules have a nominal owning record, not a real one.** All fourteen rows in `rules.register.json` name ADR 0012 as `owningAdr`: group H because it owns the support-claim and ownership surface those rules protect, group J because ADR 0012 owns who is allowed to say yes, which is exactly what the Broiler Code Assurance system records. ADR 0012 is frozen and names neither group. Every other row in the register names a record that states its rule, so these fourteen are the places where `owningAdr` asserts less than it looks like it asserts, and a reader should not read it as a binding the record itself carries. Group J is further out on this limb than group H: its authority is `BROILER-CODE-ASSURANCE.md`, an owner's policy document that is not an ADR of this component at all, so no frozen record in `docs/adr/` states the rules J1 to J9 enforce. Closed by: the first amendment that lets a record state them, or a record of their own. |
| EX-54 | Open | **Rule H5 checks document against log, not log against checkout.** It holds every quoted headline figure to the value in the log this bundle retains for it, which catches a document that has drifted from the evidence. It cannot catch a log that has drifted from the tree: a stale log and a stale document agree with each other, and nothing in the suite re-runs the collection. The figures in section 6 are therefore true of `test.log`, `publish-aot.log` and `publish-jit-and-trimmed.log` as retained, and are current only for the checkout those logs were collected against - which, since the group H rules and their tests were added after this collection, they are not. Section 8's expiry clause and recertification triggers are what cover that drift; H5 is not a substitute for re-collecting the bundle. |
| EX-55 | Open | **Rule H3 does not fix how many items the review worksheet must carry.** The eight review areas are a fixed enumeration in the test, and every area is held to having at least one item; the item COUNT is not an enumeration and cannot be, because the worksheet grows as the component does. It is held instead by agreement between two documents - the count HUMAN_REVIEW.md section 4 states and the items and progress table in `docs/review/vm-0-vm-1.md` - so no single-document edit can shed a checklist item and stay green. A coordinated edit to both still can, and nothing outside those two documents says how many items the review ought to have. Closed by: an external authority for the checklist's extent, which would have to be a record rather than a rule. |
| EX-56 | Open | **Rule H5 recognises a finite list of phrasings, and a figure worded outside it is not checked at all.** Four lists carry the rule - a suite total, an architecture/behavioural split in either word order, a Native AOT size and a trimmed size - and each entry is exercised by a witness sentence no other entry matches, so no entry is decoration and none can be deleted with the suite green. What the lists cannot be is complete: an adversarial pass defeated all four of the rule's document-facing clauses by rewording perfectly readable sentences, and the four wordings it used are now recognised, which is not the same as recognising every wording. The anti-deletion guards are what keep this from becoming a licence to reword the corpus into silence: the current suite total, its split, the native image size and the trimmed image size must each still be quoted somewhere in a recognised phrasing, compared by value. Closed by: figures carried as data the document renders rather than as prose a regular expression reads. |
| EX-57 | Open | **Rule H5 does not scan fenced code, so a figure quoted inside a fence is never compared.** Rules H1 and H2 disclose the same limit for marks and citations, where it is deliberate: an example in a fence is an illustration and not a claim. For a figure it is weaker than that, because pasting a log excerpt into a fence is an ordinary way this document set quotes a log, and such an excerpt would go unchecked. Found by attacking the rule after it was written, and recorded rather than fixed: scanning fences would make every illustrative figure in this file a violation, and deciding which fenced figures are claims is a change to the rule's subject, not a repair. |
| EX-58 | Open | **A register row's prose is held by literal substrings, not by the rule it describes.** `AssertTheRegisterRowStatesItsLimits` asserts that each group H row contains a handful of required phrases, and `Every_Rule_Row_States_Itself` asserts only that the fields are non-empty. Any limit sentence not covered by a required phrase can be deleted, and an over-claim can be added to `statement`, with the suite green - which is the standing defect this register exists to prevent, reachable by editing the row instead of the rule. No mechanism proposed so far binds prose to behaviour; the register is held to the rules by review. |
| EX-59 | Open | **About fifteen clauses of group H have no witness of their own.** The five rules were attacked in two rounds; the second round found, among other things, that the closing-hash strip on an ATX heading, the doubly-backticked-cell loop, the heading-predicate whitespace guard, the level-4-heading guard in the worksheet reader, two guards in the exclusion-table reader, and the single-size refusal in the log reader can each be deleted with the suite green. Every clause the FIRST round named is now witnessed and verified by mutation; these are the ones the second round found, and they are recorded rather than fixed. Each is a clause a later patch could remove without the suite noticing. |
| EX-60 | Open | **No CI lane runs either assurance mode, so the policy's CI-enforcement clause is unmet.** `BROILER-CODE-ASSURANCE.md` assigns fingerprint maintenance and generated-summary maintenance to CI, and makes an unresolved assurance state a release blocker. This component has no CI lane at all - EX-45 records one RID, one machine and no lane - so the generator and the gate are the same code run as a test in the architecture suite: `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release` writes every fingerprint, header and `CODE-ASSURANCE.md`, and a bare `dotnet test Broiler.VM.slnx -c Release` asserts that what is on disk is byte-identical to what that run would produce. Rule J5 is that gate and it is real, but nothing external compels it: no protected branch, required review or workflow refuses a commit whose annotations have gone stale, and a developer who does not run the suite is stopped by nothing. Two further things the policy asks for cannot exist without a lane and do not exist here: comparing a change against its parent commit to decide whether a review transition is legitimate, and binding a reviewer declaration to a GitHub identity and pull request. Closed by: a CI lane that runs both modes and blocks the merge on the gate. |
| EX-61 | Open | **A six-hex fingerprint answers one question and no other.** Every fingerprint in this component is the first 24 bits of SHA-256 over the declaration's token texts, joined by single spaces - trivia is excluded because a token's text is its own characters and never the comments or whitespace around it, which is the mechanism, and not the `WithoutTrivia()` call that used to sit in the chain doing nothing while three documents named it as the mechanism. That is enough for the question the policy asks of it - did this unit change since it was reviewed - and it is enough for nothing else. It is **not collision-free across units**: with 1,592 code units in the checkout as it stands - 689 of them carrying a fingerprint in an annotation, all 1,592 carrying one in `assurance.manifest.json`, and 45 more there for the covered files themselves - same-value pairs are expected by the birthday bound, so two units sharing a fingerprint says nothing about the two units. And it is **not a cryptographic commitment**: a party who can choose the code can brute-force a preimage for a 24-bit prefix in seconds, so a hostile rewrite that preserves the value is not detected by this number at all. Detecting that is the job of the git history and the review event, not of the fingerprint. Rule J3 binds each recorded value in an annotation to the declaration it describes, and rule J7 binds each recorded value in the manifest to the same; both can do no more than the value permits. Closed by: a wider fingerprint, which costs only line width, or a commitment scheme, which needs somewhere trustworthy to keep it. |
| EX-62 | Open | **The exemption predicate decides which units need a human annotation, and a unit it wrongly exempts is annotated by nobody.** Rule J1 asserts that every relevant unit carries a review annotation, and relevance is decided by one predicate, `AssuranceScanner.ExemptionFor`, over eight declared cases plus a per-unit escape hatch. As the checkout stands it exempts 903 of the 1,592 code units in the three product assemblies, and 277 of those are enum members, which case 8 exempts because the enum DECLARATION is the unit that carries the vocabulary and its fingerprint covers every member and every value. Six of its cases were too broad and were narrowed after three adversarial passes, each with a witness of its own: case 2 accepted any permutation of a constructor's assignments; case 3 accepted a delegation that supplied a literal or an enum member, and separately readmitted any member access at all; case 1 accepted a property that published a field other than its own, which is the case-2 defeat one screen further down the same file; case 5 whitelisted logical negation, so every `operator !=` in the component was a delegation to `Equals` when it is the opposite decision; and fields were not code units at all. If a case is still too broad, the unit is simply not reported by J1, because J1 was told not to look at it. **What this exclusion no longer covers is change detection.** Rule J7 holds `assurance.manifest.json` to every one of the 1,592 units, exempt and relevant alike, and to every covered file whole, so a unit the predicate wrongly exempts still carries a fingerprint the gate compares - the failure this exclusion used to describe, a semantic change that no record anywhere would notice, is closed. What remains is the narrower thing: nothing ASSESSES such a unit, no human line covers it, and the manifest is a change-detection record and not a review. That residue is EX-67. The predicate is one page in one place rather than several hundred hand-written `EXEMPT` lines because it is the reviewable artefact of the whole scheme, it is deliberately biased so that every borderline shape answers RELEVANT, and each of its eight cases carries a test of its own. What none of that supplies is evidence that the predicate is right. That is a review, and no human has performed one. Closed by: a human review of the predicate itself. |
| EX-63 | Open | **The covered source set is a directory enumeration, not the compiler's.** `AssuranceSources.Load` takes every `*.cs` file under each product project directory except the ones in that project's own `bin` and `obj`, which is what the SDK's `DefaultItemExcludes` removes. An earlier revision dropped any path with a segment named `bin` or `obj` ANYWHERE beneath the project, which does not agree with the compiler: a real product file at `src/Broiler.VM.Runtime/Internal/obj/VmHiddenGate.cs` compiled into the shipped assembly and was scanned by nothing, so J1 saw no unannotated unit, J4 saw no reviewer identifier and J5 regenerated no header - a file that shipped, displayed a reviewer identifier and a `GENERATED` summary claiming full human review, and passed the gate. That is fixed. What is not fixed is the general form: `<Compile Include>` may name any file, including one outside the project directory, and rule A3 permits a path that resolves inside the component root, so a file added that way still ships and is examined by no rule in group J. Closing it means deriving the covered set from each project's evaluated `Compile` items and failing when a compiled file is not in the scanned set, which needs an MSBuild evaluation the architecture tests do not currently perform. Rules J1, J6 and J7 all read this set and all carry the limit: a file outside it is in no manifest either. Closed by: evaluating the projects rather than globbing their directories. |
| EX-64 | Open | **No rule pins the invocation of J5's currency comparison.** The comparison itself - what is on disk against what the generator would write - is one function, `AssuranceGenerator.StaleArtefacts`, and it is witnessed: a deliberately stale artefact is written to a real file, read back, and the message the rule produces is asserted by content. Both places that assert the property over the real tree call that one function, so neither can drift from the other, and deleting the function fails the witness. A patch that deleted BOTH call sites and left the function alone would still be green, because no rule in this suite asserts that another rule was invoked. That limit is general - it applies to every rule here - and it is recorded on J5 because J5 is the gate the release depends on. Closed by: a lane that runs the gate and blocks on it, which is EX-60's closure as well. |
| EX-65 | Open | **J2 checks that a value is in its vocabulary, never that it is the right member of it.** `Origin`, `IP`, `Security` and `Resources` are held to their tables and a `Spec=ADR-nnnn` citation is held to a record that exists under `docs/adr/`. Nothing holds an assessment to being true. An annotation downgraded from `Security=High` to `Security=None` is well formed, and the file header and `CODE-ASSURANCE.md` are DERIVED from the annotations, so the downgrade is not detected as staleness - it is faithfully republished, and the report's `High-security review areas` section silently loses the entry. The component specification's rubric says all of `Broiler.VM.Binary` is at least High and that nothing in the component should be Medium or above for IP; both are prose with no rule behind them. A per-assembly floor would catch the crudest case and would still be a rule about vocabulary rather than about judgement. Closed by: a human review of the assessments, which is the thing this whole system records the absence of. Sharpened after the fourth attack round: because an assessment is a **comment**, and every fingerprint is built from token texts so that reformatting cannot invalidate a review, rewriting `Security=High` to `Security=Low` moves no recorded value anywhere - not the unit fingerprint, not the file fingerprint, not the manifest. The downgrade is then republished faithfully into the file header and into `CODE-ASSURANCE.md`. Nothing mechanical can judge an assessment; only a reviewer can. |
| EX-66 | Open | **The platform-root aggregate table the policy asks for does not exist, and J5 structurally cannot cover it.** `BROILER-CODE-ASSURANCE.md` asks for a table in the platform root aggregating the components - Broiler.VM implemented and 0% human-reviewed, every other component *not yet adopted* rather than 0%, because those are different facts. `D:\Broiler.Browser` carries no such table. J5's plan is exactly one artefact per covered product source file plus the component `CODE-ASSURANCE.md`, and that count is asserted, so the platform table is outside the plan entirely: it can be absent, or present and arbitrarily stale, and neither mode notices. The generator writes by full path and could write it, but the platform root is another component's territory and this component's rules stop at its own root. Closed by: adding the platform table to the plan, which needs a decision about who owns the platform root. |
| EX-67 | Open | **The manifest detects a change to an exempt unit; it does not review one.** `assurance.manifest.json` carries a fingerprint for all 1,592 code units, and 903 of them are exempt: they carry no annotation, no assessment, no human line and no review state, and this file does not give them one. It is a change-detection record, which is said in those words in the manifest header, in `CODE-ASSURANCE.md` and in rule J7's statement, so that a covered fingerprint is not read as a reviewed unit. Three things follow and none is fixed here. A moved fingerprint is a red suite and a line in a diff until somebody regenerates, and `BROILER_ASSURANCE_WRITE=1` accepts any change to an exempt unit silently - the developer who made the change is the only reader in the loop, and nothing records that they looked. The manifest inherits the width of the value it stores, so EX-61 applies to it entry for entry. And it inherits the covered set, so EX-63 applies to it file for file. What this exclusion replaces is the part of EX-62 that said a semantic change to a wrongly exempted unit was invisible; that part is closed, and what is left is that being watched is not being reviewed. Closed by: a human review of the exempt population, or a rule that makes a moved exempt fingerprint require an assessment rather than a regeneration. |
| EX-68 | Open | **The per-unit escape hatch is refused where it matters most, counted everywhere else, and judged nowhere.** `// Broiler-AI: EXEMPT=<reason>` exempts one unit by a sentence a human wrote. Nothing mechanical checks that the sentence is true, that it describes the unit it sits above, or that it refers to anything at all - the attack that found this replaced all twenty AI lines in `VmBoundedReader.cs`, the component's reader over untrusted input and every unit of it assessed `Security=High`, with one plausible sentence, and both modes of the gate stayed green: an exempt unit needs no annotation, and a unit with no annotation records no fingerprint. Two things now answer that and neither is a judgement. `Broiler.VM.Binary` is closed to the hatch outright, so a unit there is assessed or it is not shipped, and rule J1 asserts it in both directions. Every use anywhere else is counted and named in `CODE-ASSURANCE.md`, so a use is visible in the component's own report rather than silent in one source file, and rule J5 asserts the count and the naming separately. The component uses the hatch nowhere as it stands, so both clauses are currently guarding an empty set. What is not answered is whether a given reason is a good one, which is a review. Closed by: a human review of each use, or a rule that can read a sentence. |
| EX-69 | Open | **A file fingerprint says that a file changed, and not what changed in it.** `assurance.manifest.json` now records one fingerprint per covered file over the complete token stream of its compilation unit, beside the per-unit entries. That is what makes the record COMPLETE rather than merely wide: the unit enumeration is a whitelist, and an `[assembly: InternalsVisibleTo("anything")]` - which opens every internal type in `Broiler.VM.Runtime` to a caller of the attacker's choosing - is a member of nothing and can never be a unit however many declaration kinds are added to the list. What the file value cannot do is localise or explain. A moved unit fingerprint names the unit; a moved file fingerprint names only the file, and finding the change is a diff a person reads. It also cannot be annotated: no human line covers a file as a whole, so nothing here is reviewed by it, which is EX-67 applied one level up. And it inherits the width of the value it stores, so EX-61 applies to it entry for entry. Closed by: nothing that is a rule. A reader who sees a moved file fingerprint reads the diff. |
| EX-70 | Open | **The declared shape of every generated artefact is a hand-maintained second copy, so a change made in both places passes.** Rule J8 holds the manifest header, all 45 generated file headers and `CODE-ASSURANCE.md` line for line to `AssuranceArtefactShape`, which is written by hand and deliberately not generated from the generator - a copy the generator produced would agree with the generator whatever the generator said. That raises the cost of putting a sentence in front of a reader from one edit to two edits by somebody who has read both, which is exactly the property the group J register rows have and no more than that. Where content is derived the derivation is checked rather than copied, and by a different expression, so a defect in one is a disagreement rather than a shared answer; but a person who edits both sides consistently is not stopped. Rule J9 is the independent hold that cannot be lifted by editing a shape, and the two together are the answer rather than either alone. Closed by: a review of the shape, which is a review. |
| EX-71 | Open | **Rule J9 recognises a finite list of review terms, and a claim worded outside it is not seen.** The rule asserts that no generated text says verified, approved, reviewed by, a reviewer identifier or eligible for release unless the annotations support it - by stating the count the annotations give for that term, or by standing behind a negation. A generated sentence saying a unit was cleared, or signed, or passed, or that the component is ready, is not in the list and is not reported. This is the same shape as EX-56 for rule H5 and it is recorded for the same reason: the list is what an adversarial round used, which is not the same as every wording. Two things limit the damage. Rule J8 holds every generated line to a declared shape, so a sentence of ANY wording is a line that shape does not carry; and the terms are matched case-insensitively, as whole words, anywhere in a line, so the list cannot be evaded by shouting or by punctuation. A second, narrower limit: a negation standing before the term is accepted as a statement of absence, so a line that negates one thing and asserts another passes. Closed by: figures and states carried as data a reader renders rather than as prose a rule reads. |
| EX-72 | Open | **The covered corpus can be separated from what ships, and that is not fixed.** EX-63 records that `AssuranceSources.Load` enumerates directories rather than reading each project's evaluated `Compile` items. The fourth attack round confirmed the consequence: a source file can be removed from compilation, or a `<Compile Include>` can name a file outside the project directory, with no covered file changing and the whole record - annotations, per-unit fingerprints, file fingerprints, manifest - agreeing that nothing happened. Reading MSBuild's evaluated item set would close it and would put an MSBuild evaluation inside a test; the gap is recorded instead. Rule A3 constrains item paths from the other side, which narrows but does not close it. |
| EX-73 | Open | **Four attack rounds produced 46 defeats, and the pattern did not change.** Every blocker was in COVERAGE - what the system was looking at - and none was in the fingerprint, the state machine, or the refusal to manufacture a review. Two structural answers were adopted rather than a fifth round of patching: fingerprinting every unit so that exempt means unannotated rather than unwatched, and fingerprinting every file so that a declaration kind the unit enumeration does not list is still covered. What a fifth round would find is unknown, and the honest reading of this history is that it would find something. The system records the absence of human review precisely; it does not certify the code, and no count in `CODE-ASSURANCE.md` should be read as assurance. |
