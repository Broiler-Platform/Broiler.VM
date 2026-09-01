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
| `osx-arm64` | JIT, trimmed self-contained, Native AOT — **in the CI lane only** | **Not claimed.** As above: attempted and passing in the lane, retained in no collection. EX-45 |
| `osx-x64`, `linux-arm64` | — | **Not claimed.** Never attempted, by the lane or by a collection |

**One machine, one RID for every retained collection.** That is EX-45, and it is still the widest
limit on everything in this table — but its old wording said "no CI lane has ever run", and that is
no longer true. The lane in `.github/workflows/` runs on hosted runners and passes, publishing and
running every composition root as Native AOT on three RIDs.

**That moves no row above, and the reason is the one distinction this table rests on.** A support
claim is made on a *retained collection*: a bundle in `docs/evidence/`, collected deliberately by a
person, naming its machine, its SDK, its effective configuration and its raw outputs. The lane
collects none of that — its own header says so — so a green job is evidence that the component
builds and runs somewhere, and evidence of nothing a reader could check twice. **A workflow that
has not run is a plan; a workflow that has run is still not a bundle.**

What did change is the honesty of the gap. A reader was previously told that nothing had ever
published on `win-x64` or `osx-arm64`; something has, repeatedly. The rows stay unclaimed because
no collection exists on them, which is a narrower and truer reason than the one they carried.

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
