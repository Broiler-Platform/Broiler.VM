# Broiler.VM support table

> **The core ships no language profile.** Broiler.VM is a semantics-neutral bytecode-VM core. It
> executes nothing on its own: every opcode, value, frame and verifier rule belongs to a profile,
> and no profile ships in any package listed here. A host that installs these three packages has
> installed a mechanism and no language.

**Core contract version: 1.** Versioned separately from any profile format, feature manifest,
package version and persisted-envelope schema version. Its amendment procedure is published in
[ADR 0003](adr/0003-core-contract-v1-and-amendments.md).

**Status: preview, unaccepted.** No milestone in this component has been accepted, because
`HUMAN_REVIEW.md` is unsigned and `PENDING`. Nothing here is a release, and the binding rule in
ADR 0001 stands: **no Broiler.VM package is published without a completed review naming the
reviewed commit.**

Roadmap section 16 makes an **untruthful support claim a stop condition**, not a defect to fix
later. Every row below therefore states what has been *demonstrated on retained evidence*, and
says so where nothing has been.

---

## 1. Packages

| Package | Version | What it is | Depends on |
|---|---|---|---|
| `Broiler.VM.Abstractions` | 0.1.0-preview.1 | The profile-neutral contracts: descriptors, results, diagnostics, budgets, the source-level profile contract | Nothing |
| `Broiler.VM.Binary` | 0.1.0-preview.1 | Bounded binary reading: checked readers, canonical-only LEB128, framing, allocation guards. No format, no schema, no semantics. **A format whose specification requires padded variable-length encodings to be accepted cannot use the variable-length readers or `TryReadDeclaredCount`**, and decodes its own integers over the byte-level members instead | Nothing |
| `Broiler.VM.Runtime` | 0.1.0-preview.1 | The catalog, the runtime and its lifecycle, resource authority, guest-load mediation, external suspension | `Broiler.VM.Abstractions`, `Broiler.VM.Binary` (assembly references; see below) |

**The three packages depend on nothing outside themselves.** `Broiler.VM.Runtime` declares the
other two and that is the whole of it: no third-party package, no other Broiler component, nothing
from nuget.org. Rule C2 asserts it against the retained `.nuspec` of every produced package, and
the pristine feed consumer is the same claim from the other side - that sample's only restore
source is a directory holding these three, with nuget.org unreachable, so a dependency on anything
else would fail its restore outright.

No fourth package exists. ADR 0001's budget section fixes the set at three and requires a dated
revision of that record before a fourth `PackageId` may appear; rule A6 asserts it.

---

## 2. What the core supports

| Capability | Support | Evidence |
|---|---|---|
| **Static profile registration** | Supported | A direct-factory catalog with no reflection and no name-based discovery. Rules B5 and A9; the catalog-drift baselines in `docs/compositions.md` |
| **The source-level profile contract** | Supported | Two application-local profiles and one package-only sample profile are written against it. Rule A11 forbids any project outside a composition root to reference a profile assembly, so the contract is the only route |
| **Verification separable from execution** | Supported, and required to stay so | One verification entry point in the whole core, asserted by rule V9. An artifact becomes a handle before anything instantiates it |
| **Immutable verified artifacts** | Supported | Caller bytes are decoded into a profile-bound handle; mutation, disposal and concurrent overwrite after verification change nothing |
| **Resource authority (15 dimensions)** | Supported | Host ceiling authoritative, profile may tighten, artifact may only request less; the intersection is computed before allocation and recorded on the handle |
| **Aggregate budgets across runtimes** | Supported | Shared rather than multiplied, under concurrency |
| **Guest-initiated loads** | Supported, bounded | Depth, fan-out, cumulative bytes and nested verifier work, charged to the requesting operation. A composition registering no provider refuses every request deterministically |
| **External suspension** | Supported, opt-in | Declared by the profile and enabled by the host; both gates required |
| **Cancellation and bounded disposal** | Supported | Cancellation reaches a step inside a profile; disposal drains in-flight steps under the host's own wall-clock bound |
| **Thread affinity** | **Partial** | `OperationThreadPinned` is enforced where the core can see a thread - on resume, the only place a second thread enters an existing operation. A profile that starts its own threads is invisible to the core. EX-89 |
| **Asynchronous instantiation** | **Not supported** at contract version 1 | Recorded as a decision, not an omission. No member returns a task; rule V8 asserts it |
| **Persisted envelope** | **Not supported** at contract version 1 | Admitted as contract by ADR 0010 decision 4 and implemented by no milestone. Release 1 exposes no envelope member; rule V10 asserts it. EX-25 |
| **Streaming or incremental verification** | **Not supported** at contract version 1 | The contract is whole-bytes-to-handle. Adding one is a numbered amendment, not a signature widening; rule V10 asserts it |
| **A binary plug-in ABI** | **Not supported, and not planned** | Extensibility is compile-time. Nothing here implies a loadable plug-in format |

---

## 3. Runtime identifiers

| RID | Publish and run | Status |
|---|---|---|
| `linux-x64` | JIT, trimmed self-contained, Native AOT | **Demonstrated.** Every collection in `docs/evidence/` was taken on it, publishing and running the fixtures host, both composition roots and the package-only sample with trim and AOT warnings as errors |
| `win-x64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed.** No retained collection exists on it. The CI lane publishes and runs every composition root here as Native AOT on a stock runner, which is what a claim would need to be *about* and is not a claim. EX-45. **The reason this row used to give is withdrawn**: the publish does not need a `vcvars64` environment, it needs `vswhere.exe` on `PATH`, which ADR 0001 records and the lane confirms — EX-42 |
| `linux-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, and declared for the *architecture* rather than for a consumer: nothing publishes it, and it is the only place this component compiles arm64 at all. **A green job on it is not Android coverage** — it shares an instruction set with `android-arm64` and nothing else, not the runtime, not the trimming configuration, not the head. EX-45 |
| `win-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, for the pair neither `win-x64` nor `linux-arm64` reaches: the Windows Native AOT toolchain targeting arm64. The limit on the row above applies here unchanged — this is not Android coverage either. EX-45 |
| `osx-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01** for the clang-into-Mach-O toolchain, which no other declared RID reaches: a different object format, a different linker, `dyld`, and an ad-hoc code signature Apple silicon requires before a binary will run at all. EX-45 |
| `osx-x64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed. Declared 2026-09-01**, and with the weakest ground of the six: the toolchain is reached by the row above and the architecture three times over, so what it adds is the pair. Collectible only on the successor Intel image — `macos-13` is retired. EX-45 |
| `android-x64` | **Mono, no trimming, no AOT** — in the CI lane and one retained collection | **Not claimed. Declared 2026-09-02** on the rule's consumer limb rather than the grid limb: the consuming repository's Android head ships it, and it is a fourth runtime family rather than a fourth toolchain cell. Bundle `js-android-001` retains a collection taken on an **emulator**, which is not a device. **Nothing about it is evidence about Native AOT** |
| `android-arm64` | published, **not run** | **Not declared.** The head builds it and nothing executes it — an arm64 emulator on an x64 host is not usable and this component has no device. A RID whose evidence stops at a build is what publish-and-run refuses |
| `ios-arm64` | — | **Excluded, and not for want of effort.** It is a *device* RID: publishing needs an Apple signing identity, running needs a tethered device, and no hosted runner has either. Only a self-hosted Mac with a device attached could ever collect it |
| `iossimulator-arm64`, `iossimulator-x64` | — | **Excluded pending a head.** Reachable in principle on a macOS runner's simulator; this component has not written the iOS head that would publish them |
| *(none)* | — | **No RID is reserved as of 2026-09-01.** The category is kept because a reserved RID is one this table may never name, and a rule nobody can find is a rule nobody applies `osx-arm64` was attempted and passed in the CI lane until 2026-09-01, when the lane was brought back to the declared matrix — the ADR's revisions record the withdrawal, and that a reserved RID with a lane behind it is one edit away from being read as supported |
| `android-arm64`, `android-x64` | — | **Excluded.** The consuming repository's Android head is `net10.0-android36.0` on Mono with trimming off, so the exclusion's original reason — no evidence that ILCompiler Native AOT targets an Android RID — answers a question that head does not ask. The gap that would have to close first is narrower: this component has no Android-targeted project and no device or emulator harness, and *publish and run* on an Android RID means an application package and a device. EX-32 |

**One machine, one RID for every retained collection.** That is EX-45, and it is still the widest
limit on everything in this table — but its old wording said "no CI lane has ever run", and that is
no longer true. The lane in `.github/workflows/` runs on hosted runners and passes, publishing and
running every composition root as Native AOT on **the declared RIDs**, which since 2026-09-01 is
also every RID it attempts — two of them at first, then all six as ADR 0012's revisions of that day
widened the matrix to the full grid. Every one published and ran at the first attempt it was given a
runner for; one `win-arm64` attempt of three failed inside the SDK installer on the hosted image,
before any of this component was built, which is recorded in the workflow rather than smoothed over
here.

**Since 2026-09-03 that sentence is about the FULL lane and not about every run.** The lane was
split: `broiler-vm.yml` runs on every push and pull request over two of the six cells —
`linux-x64` and `linux-arm64`, one per architecture — and `broiler-vm-full.yml` runs all six with
the emulator on a release tag, a weekly schedule and a button. So "every declared RID published and
ran" is a statement about a release tag and a Monday, not about the last push. It changes nothing
in the table above, because that table is fed by retained collections and by no lane at all; it is
recorded here so that a reader does not take a green pull request as the six-cell answer.

**That moves no row above, and the reason is the one distinction this table rests on.** A support
claim is made on a *retained collection*: a bundle in `docs/evidence/`, collected deliberately by a
person, naming its machine, its SDK, its effective configuration and its raw outputs. The lane
collects none of that — its own header says so — so a green job is evidence that the component
builds and runs somewhere, and evidence of nothing a reader could check twice. **A workflow that
has not run is a plan; a workflow that has run is still not a bundle.**

What did change is the honesty of the gap. A reader was previously told that nothing had ever
published on `win-x64`; something has, repeatedly. That row stays unclaimed because no collection
exists on it, which is a narrower and truer reason than the one it carried.

**And one row left this table rather than moving up it.** `osx-arm64` had a row of its own here
while the lane published on it. ADR 0012 marks that RID reserved, and a reserved RID may never
appear in a support table — so the row is gone and the lane entry with it. Nothing was claimed and
nothing is now unclaimed that was not: what a reader loses is a row that implied this component was
going somewhere it has not decided to go.

**Four rows arrived by the opposite route, and the difference is the point.** `linux-arm64`,
`win-arm64`, `osx-arm64` and `osx-x64` are not in this table because a lane runs on them; they are
here because the matrix was widened by dated decisions that state what each row does and does not
stand for, and the lane follows the matrix rather than leading it. That is the order the withdrawal
above exists to restore — and `osx-arm64` is the same RID that left this table that morning,
readmitted in the afternoon by an argument rather than by a job.

**The six are a grid rather than a list**: three Native AOT toolchains — MSVC into PE-COFF, clang
into ELF, clang into Mach-O — by two architectures, every cell filled. A seventh RID cannot come
from that reasoning, only from a consumer, which is where the Android and iOS RIDs sit and why they
remain excluded: no targeted project and no harness that can *run* what is published.

---

## 4. Deterministic exclusions

Behaviour that is bounded, deliberate and will not change without an amendment.

| | Exclusion |
|---|---|
| **A profile is never discovered** | Registration is a direct factory call in a composition root. There is no probing path, no assembly scan and no configuration file that can add one |
| **An unknown format version is refused, never guessed** | Interpreting old bytes under new semantics is prohibited. The refusal is deterministic and identical on repetition |
| **A composition with no artifact provider refuses every guest load** | That is the content policy expressed as a contract outcome, not an error condition |
| **An artifact cannot loosen a host ceiling** | It may request less. The effective policy is the intersection and is computed before any allocation proportional to a declared count |
| **Diagnostics cannot carry free text** | Every member of the record is an enum, a number, or one of four identity types. A host capability that throws an exception carrying a secret produces a failure that carries none of it - rule V11 |
| **A profile cannot reach undeclared CLR surface** | No profile-facing contract takes or returns `object`, a `Type`, a delegate, a reflection type, an assembly load context or a raw pointer - rule V12 |
| **The core reaches no legacy Broiler component** | An architecture-tested rule, not a convention. Rules A1, A2 and D1 |

---

## 5. Measured overhead

Published with its method in [the baseline register](baselines.md), and bounded by what it is:
figures from **one four-processor `linux-x64` machine**, of the **core's own overhead** around a
fixture profile whose executor is a toy.

**No language performance claim follows from any of it.** The core publishes only what it costs a
profile; what a language costs is that language's own.

---

## 6. Operations

| Question | Answer |
|---|---|
| **Rollback** | Exercised, not described. A consumer restores one package set from a feed, then is rolled back to the previous one and still restores, builds and runs - and prints the informational version it actually loaded, so the transcript shows which set answered. `feed-consumer.log` in the current bundle |
| **Format-version rejection** | Deterministic and repeatable; asserted from a package consumer's position by the sample |
| **Envelope recovery** | Not applicable at contract version 1: there is no persisted envelope to recover |
| **Vulnerability response** | The owner named in [ADR 0012](adr/0012-security-ownership-and-support-matrix.md) holds all six roles, security included. There is no separate security contact and no published disclosure timeline, which is a gap this table names rather than hides |
| **Recertification** | Required when the SDK or runtime, core contract version, package graph, host capability surface, Native AOT settings, RID matrix, cache identity, resource defaults, or representative workload changes. Each evidence bundle carries its own triggers in section 8 |
| **Support lifetime** | None stated. A preview with no accepted milestone has no support commitment, and inventing one here would be the untruthful claim section 16 forbids |

---

## 7. What this table does not say

- It does not say the component is production-ready. No milestone is accepted.
- It does not claim any platform beyond `linux-x64`.
- It does not claim a security review by anyone other than the author, who holds every role.
- It does not claim performance for any language, and the core has no language to claim it for.
