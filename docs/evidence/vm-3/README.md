# Evidence bundle VM-3-001

**Milestone:** VM-3 — the public profile contract and exact composition closures
**Collected:** 2026-08-29
**Core contract version:** 1
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition. Under update
rule 8 the work could be built and landed without that decision; it could not be released, and
nothing here is a release.

What VM-3 was asked for is a claim about what the core does **not** contain. Two profiles nobody on
the core side wrote, in two assemblies the core has never heard of, composed into two images whose
contents are listed off the published output — and three product assemblies that did not change to
accommodate any of it. The single most load-bearing figure in this bundle is therefore a
non-difference: the Native AOT image of the fixtures host is 1557288 bytes, the same as VM-2's, and
the trimmed image is 78256 bytes, the same as VM-2's. Adding a profile changed nothing in the core.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry. Each is below; a field this
milestone cannot supply says so rather than being omitted.

| Field | Where |
|---|---|
| Identity | Section 1 |
| Source | Section 2 |
| Dependencies and corpus | Section 3 |
| Environment | Section 4 |
| Procedure | Section 5 |
| Outputs | Section 6 |
| Decision | Section 7 |
| Validity | Section 8 |

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-3-001 |
| Milestone | VM-3 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-3 gate |
| Core contract version | 1, unchanged. VM-3 mints no amendment and adds no public member. |
| Reason-registry revision | 2, unchanged. No reason was added: two consumer profiles were written against the existing set and needed none. |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

---

## 2. Source

Collected against the component with a **dirty working tree**: this bundle's own record set is
uncommitted at collection time — the ledger row, the review record's VM-3 paragraphs, the rule
register's four new rows and its milestone, the composition register, and the collection script's
composition step.

What VM-3 added to the checkout, in full:

| Added | Path | Kind |
|---|---|---|
| `Com.Example.Calculator` | `src/tests/Com.Example.Calculator/` | consumer profile, test-only path, non-packable |
| `Com.Example.Ledger` | `src/tests/Com.Example.Ledger/` | consumer profile, test-only path, non-packable |
| `Broiler.VM.Composition.Calculator` | `src/compositions/Broiler.VM.Composition.Calculator/` | composition root, non-packable |
| `Broiler.VM.Composition.Workbench` | `src/compositions/Broiler.VM.Composition.Workbench/` | composition root, non-packable |
| The composition register | `docs/compositions.md` | record; closes Exclusion EX-08 |
| The catalog baselines | `src/tests/Broiler.VM.Architecture.Tests/catalogs/` | drift baselines rule K3 reads |

**No file under `src/Broiler.VM.Abstractions/`, `src/Broiler.VM.Binary/` or
`src/Broiler.VM.Runtime/` was changed by this milestone.** That is the gate's central clause, and it
is checkable from `hashes.txt` against VM-2's: every product source hash is identical.

---

## 3. Dependencies and corpus

The VM-2 corpus is unchanged: eighty-seven artifacts under `src/tests/corpus/vm-2`, each with its
SHA-256 and expected answer, replayed here in all three publish modes. VM-3 adds no corpus of its
own, because its claims are about composition rather than about parsing.

Two profile formats were added instead, and they are deliberately unlike each other and unlike the
fixture's:

| Profile | Format | Entry point | Imports | Framing |
|---|---|---|---|---|
| `com.example.calculator` | magic, varint version, zigzag operand pool, flat one-byte token stream | one fixed name, `evaluate` | none at all | none: a flat read to the end, with trailing bytes refused |
| `com.example.ledger` | magic, varint version, two length-framed sections, a contiguous name blob and fixed-size records | the account name, as the caller's own UTF-8 bytes | one optional capability | two sections, each consumed exactly or refused at the frame |

A second profile that quietly reused the first one's shape would demonstrate that the core supports
one format twice. Between them the two exercise both halves of the bounded-reading surface —
declared counts and flat windows on one side, section frames and structural depth on the other —
from outside the repository's own fixtures.

---

## 4. Environment

| Field | Value |
|---|---|
| OS | Linux 6.18.44, glibc 2.39 |
| Architecture | x86_64 |
| RID | `linux-x64` |
| Processors | 4 |
| Configuration | Release |
| Publish modes | JIT, trimmed self-contained, Native AOT |
| Lane | None. One machine, one RID, no CI. |

`environment.txt` records the exact toolchain. Exclusion EX-45 stands: one machine and one RID are
not a matrix, and nothing here claims a RID.

---

## 5. Procedure

Everything below was produced by `python eng/collect-evidence.py --bundle VM-3-001 --out
docs/evidence/vm-3`, run from the component root.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4 | `dotnet run --project <fixtures host> -c Release -- --verbose` | `publish-jit-and-trimmed.log` |
| 5 | `dotnet publish <fixtures host> -c Release -r linux-x64 --self-contained true -p:PublishTrimmed=true`, then run the binary | `publish-jit-and-trimmed.log` |
| 6 | `dotnet publish <fixtures host> -c Release -r linux-x64 -p:PublishAot=true`, then run the native binary | `publish-aot.log` |
| 7 | Replay the corpus from each of the three modes and compare the tables | `corpus-replay.log` |
| 8 | Eight seeded fuzz sessions of 250,000 iterations each | `fuzz.log` |
| 8b | For each composition: run under JIT, publish and run trimmed, publish and run Native AOT, compare the catalog table across the three, and list the closure of each published directory | `composition-calculator.log`, `composition-workbench.log`, `catalog-*.txt`, `closure-*.txt` |
| 9 | Eleven negative controls, each injected, run, reverted, and run again | `negative-control.log` |
| 10 | Environment and hashes | `environment.txt`, `hashes.txt` |

The script judges nothing. It runs the procedure and retains what happened, failures included.

---

## 6. Outputs

**Build.** Twelve projects build Release with 0 warnings and 0 errors. The four new ones are the
budget ADR 0001 revision 1 authorises, and the packable set is unchanged at three.

**Tests.** 262 tests pass, 0 failed, 0 skipped — 97 architecture and 165 behavioural. VM-2 retained
255; VM-3 adds seven architecture tests and no behavioural test, which is the honest shape of this
milestone: its claims are about the graph, the register and the published images, and the profiles
it added are exercised by composition roots that publish and run rather than by a suite that
references them.

**That last clause is a decision, not an oversight.** The behavioural suite does not reference
`Com.Example.Calculator` or `Com.Example.Ledger`, because rule A11 forbids any project outside a
composition root to reference a profile assembly — and that rule is one of the things this milestone
exists to demonstrate. Adding the two profiles to the test project's reference set would have made
the suite bigger and the claim smaller. What exercises them is twelve checks across two composition
roots, each of which is published and run in three modes.

**Pack.** Exactly three `.nupkg` and three `.snupkg`, one pair per product package. None of the
seven test-only projects and neither composition root packs.

**Publish and run.** The fixtures host publishes and runs in all three modes as before: the trimmed
self-contained image is 78256 bytes and the Native AOT native image is 1557288 bytes — **both
byte-identical to VM-2's**, which is the numeric form of "no core change". Each composition root
also publishes and runs in all three modes and passes every check in each; their native images are
1445920 and 1487152 bytes.

**The compositions.** Twelve checks in total, all passing in all three modes:

| Composition | Checks |
|---|---|
| `Broiler.VM.Composition.Calculator` | arithmetic through the full lifecycle; a language fault that stays a language fault; an unknown entry point answered by the profile rather than by the core; an over-deep program refused at verification rather than at execution; a registered capability the composed profile cannot reach |
| `Broiler.VM.Composition.Workbench` | two profiles in one catalog; each profile refusing the other's artifact; entry-point bytes read as a fixed name by one profile and as a lookup key by the other; an optional import bound; the same import unbound in a runtime whose host registered nothing; a profile the composition does not contain answered as unsupported rather than as invalid; two profiles' faults kept apart by payload identity |

**Catalog tables, across modes.** Each composition prints the profiles it composed. The three tables
per composition — JIT, trimmed, Native AOT — are identical, and each matches the checked-in baseline
rule K3 compares against.

**Closures.** Read off the published directories rather than derived from project files:

| Composition | Trimmed self-contained | Native AOT |
|---|---|---|
| `Broiler.VM.Composition.Calculator` | 5 non-framework assemblies: the three core packages, the composition, `Com.Example.Calculator` | none: a native image carries no managed assembly |
| `Broiler.VM.Composition.Workbench` | 6: the same five with `Com.Example.Ledger` added | none |

**The two closures differ by exactly one assembly**, and that assembly is the second profile. No
fixture assembly, no testing framework, no reflection-emit assembly, and nothing either composition
does not declare. A test asserts the difference rather than a sentence claiming it.

**Cross-mode failure classes.** The VM-2 corpus replay is repeated and the three tables remain
byte-identical, which is what makes it fair to say the product assemblies are unchanged in behaviour
as well as in bytes.

**Fuzz.** Eight sessions, 250,000 iterations each, 2,000,000 in total, no counterexample.

### 6.1 What building the second profile found

The two-profile composition could not verify a ledger artifact until the profiles' hard maxima were
corrected, and the reason is a property of the contract rather than of these two profiles.

A runtime ceiling is clamped to the tightest profile hard maximum **in the catalog**, across every
profile in it, and adopting a profile default resolves to the tightest default in the catalog. Both
are catalog-wide facts. The calculator was written first and declared its own usage as its maxima —
zero host calls because it makes none, one section because it frames one — which capped every
profile composed beside it. The ledger frames two sections, so its own artifact was refused with
`ResourceExhaustion` naming `SectionCount`, in a verifier that had done nothing wrong, because of
what a different profile in the same catalog had declared.

Nothing in the core changed. What changed is the two profiles' declarations and the advice a profile
author now finds in `docs/compositions.md` section 5: a hard maximum is a statement about your
neighbours as much as about you, and the thing that keeps a dimension unreachable is your import
list and your budget matrix, not a zero ceiling. The two-profile root also states three ceilings
explicitly, which is what a host composing unlike profiles has to do; the effective ceiling for an
operation is still the intersection with that profile's own maxima, so the calculator is still held
to one section and no host call.

### 6.2 The negative controls

Eleven controls, each injected, run, reverted and run again. Three are new at VM-3 and are the ones
that matter for this milestone's claims: a composition root that links the fixture profile, a
composition deleted from the register, and a catalog baseline that gains a profile the composition
does not compose. `negative-control.log` retains both runs of every control.

---

## 7. Decision

The VM-3 gate, clause by clause. The marks are evidence verdicts set by the author about what the
retained evidence shows; no reviewer has set anything.

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[MET]` | A consumer profile is added without changing the core runtime, the execution loop, or any Broiler-owned package | Two were added. No file under the three product project directories changed: `hashes.txt` carries the same product source hashes as VM-2's bundle, and both published image sizes are byte-identical to VM-2's. |
| `[MET]` | Without reflection, name-based loading, or an extension directory | Each profile arrives through a static accessor on its own type, named directly in a composition root. Rule B5b forbids a module initializer, V9 holds the construction site, and the closure reports contain no reflection-emit assembly. Neither profile assembly references anything but Abstractions and Binary, which rule A13 asserts. |
| `[MET]` | Single-profile and two-profile compositions each publish and run under trimming and Native AOT | Both roots published and ran in both modes plus JIT, passing every check in each. `composition-calculator.log` and `composition-workbench.log` retain the transcripts, image sizes and exit codes. |
| `[MET]` | Each closure report contains exactly the declared profiles and no fixture or test assembly | The trimmed closures are exactly five and six non-framework assemblies. ADR 0001 revision 1 fixes the reading of the clause — the consumer profile lives at a test-only path and belongs in the closure; what the clause excludes is the fixture profile and a test harness — and rule K4 asserts the equality in both directions rather than a subset. |
| `[MET]` | CI detects duplicate or reserved IDs, undocumented entries, missing factories, forbidden edges, and catalog drift | Six rules, minted here and each with its own violating input: A12 and A13 for the forbidden edges, K1 for undocumented entries in both directions, K2 for duplicate and reserved IDs and for a register row that disagrees with the reference set or the catalog, K3 for catalog drift against a published binary, K4 for the closure. "Missing factories" is the one word that needs care: the catalog builder refuses a descriptor without a verifier or an executor factory, and it is the core's own validation rather than a rule minted here — every composed descriptor passes through it at construction. |
| `[MET]` | Prove that adding a second profile requires no change to the core runtime or execution loop | The two composition roots differ by one project reference and one `.Add` call. The three core assemblies are the same build in both, and the two closures differ by exactly one assembly, which a test asserts. |
| `[N/A]` | The source-compatibility promise this exposes is frozen in VM-6 | VM-6's, not this milestone's. Nothing here freezes a promise, and no binary plug-in ABI is implied or implemented: composition is source-level, by direct typed registration. |

### 7.1 What is exercised by a published binary rather than by the suite

Worth stating plainly, because it changes how this bundle should be read. The consumer profiles are
not referenced by any test project, by design and by rule. Everything that is true of them is
demonstrated by two binaries that were published and run, and retained as transcripts here. A reader
checking this milestone reads `composition-*.log` for behaviour and `closure-*.txt` for containment;
the suite's 262 tests cover the graph, the register, the baselines and the closure reports, which is
the part a test project is allowed to see.

---

## 8. Validity

**Reproduction.** `python eng/collect-evidence.py --bundle VM-3-001 --out docs/evidence/vm-3`. The
script derives the RID, the binary's extension and the Native AOT invocation from the platform.

**Expiry.** The figures here are true of the logs as retained. Rule H5 holds every quoted headline
figure to those logs and cannot hold the logs to the checkout. Exclusion EX-54 records that, and this
section is what covers it.

**Recertification triggers.** Any of these invalidates this bundle and requires a fresh collection:

- a change to any file in `hashes.txt`, which now includes the composition register and the catalog
  baselines;
- a change to the core contract version, or to the reason-registry revision;
- a composition root added, removed, or given a different profile set;
- a change to either consumer profile's descriptor, since the catalog baselines are derived from it;
- an SDK change, since none is pinned;
- a claim about any RID other than the one section 4 records.

---

## 9. Exclusions

Each is a named limit on what this bundle shows. Carried-forward exclusions keep their earlier
identifiers and are listed only where VM-3 changes what they cover. **Status** is `Open` where the
limit still stands and `Closed` where a dated decision or a later milestone has discharged it; a
closed exclusion is retained, not deleted.

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. The toolchain is what this machine resolved, and `environment.txt` records what that was. |
| EX-08 | Closed | **Closed 2026-08-29 by this bundle.** The composition-root allow-list rule A11 reads was an empty constant inside the architecture-test project. It is now the path `src/compositions/`, and what each project there may contain is `docs/compositions.md`, held to the checkout by rules K1 to K4 in both directions. ADR 0001 revision 1 records the closure. |
| EX-25 | Open | **The persisted envelope is admitted by the contract and implemented by no milestone.** Unchanged by VM-3, which ships no envelope reader and no envelope writer. |
| EX-42 | Open | The Native AOT publish on `win-x64` requires a `vcvars64` environment. **It did not apply to this collection**: on `linux-x64` the ordinary publish produced and ran native images, for the fixtures host and for both composition roots. |
| EX-45 | Open | **One RID, one machine, one lane.** Nothing here claims a RID; claiming one is a release act, and `docs/compositions.md` section 4 says so of its own RID table. |
| EX-52 | Open | **Twenty-nine review findings of major and minor severity remain unaddressed.** VM-3 did not set out to work that list and did not. |
| EX-54 | Open | Rule H5 checks document against log, not log against checkout. Section 8's expiry clause is what covers the drift. |
| EX-78 | Open | **The guest-load nesting-depth bound is unreachable at contract version 1.** Unchanged: neither consumer profile declares guest-initiated loads, and neither composition registers a provider. |
| EX-79 | Open | **The corpus retains no minimized regression, because no session has found one.** Unchanged after another two million iterations. |
| EX-80 | Open | **The fuzz session varies the payload and never the descriptor.** Unchanged, and now understated: the two consumer profiles are not fuzzed at all. Their formats are exercised by the composition roots' fixed artifacts and by nothing adversarial. Closed by: a descriptor mutator, and a fuzz target that can reach a profile it may not reference — which needs a composition root to host it. |
| EX-81 | Open | **Rule H5 admits a figure from any bundle a document links.** Unchanged. |
| EX-82 | Open | **A verification that fails records no clamp.** Unchanged; VM-2's erratum stands. |
| EX-83 | Open | **A refused budget override populates a diagnostics group ADR 0005 annotates for a different category.** Unchanged; VM-2's erratum stands. |
| EX-84 | Open | **The cross-mode table is produced under one descriptor.** Unchanged. |
| EX-85 | Open | **`DescriptorMismatch` is unreachable through the public path** for the fixture profile. Both consumer profiles check the descriptor's format version against the payload's and reserve a diagnostic code for the disagreement, and both are equally unable to produce it for the same reason: each supports exactly one format version, so the core refuses the mismatch first. The exclusion now covers three profiles rather than one. |
| EX-86 | Open | **Rules K3 and K4 compare against the last collection, not against the working tree.** The catalog tables and closure reports they read are retained by a script a person runs, so a checkout whose compositions have changed since the last collection is compared against stale evidence and passes. This is the same shape as EX-54 and has the same answer: section 8's recertification triggers name a composition change explicitly. Closed by: a CI lane that collects on every change, which is EX-60's closure as well. |
| EX-87 | Open | **The closure report excludes framework assemblies by name prefix.** `System.*` and `Microsoft.*` are dropped from the listing, so a hypothetical profile assembly named that way would not appear in a closure report at all. Nothing in the checkout is named that way, and K4's exact-set clause catches everything else — an assembly missing from the closure fails the equality just as an extra one does. Closed by: a closure derived from the linker's own dependency output rather than from a directory listing. |
