<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0028 - Shared memory and `Atomics`: the exclusion is kept, and what would have to exist to reopen it

**Status:** Proposed, 2026-09-21. **Owner decision pending.** The retention restates an exclusion
the tree already holds in every place listed in section 1, so signing it changes nothing in the tree. What
signing does add is the reopening conditions in section 5 and the ordering in section 6, and those
are why a signature is needed rather than assumed.

**Owner:** MaiRat, as this profile's owner. Not yet signed. **Co-signer:** none. When it is signed,
both roles will be held by one person, and this record does not claim the co-signature is
independent. There is no second signature to claim it of.

**Milestone:** none. The question comes from card D02 of the JSeal VM-feature roadmap
(`docs/roadmap.vm-features.md` in the Broiler.JSeal repository). That card asks for a decision
either way and for no implementation. The part of this profile's own plan that owns the subject is
the agent model in [section 13](../roadmap.md#13-realms-agents-and-the-host-boundary), which no
milestone has scheduled.

**Context.** `SharedArrayBuffer` and `Atomics` have been absent since the binary surface was
opened, and every place that says so gives the same one-line reason: *they are the multi-agent
surface and need the agent model*. Nothing has written down what that agent model would have to
decide, so the exclusion could not be told apart from a feature nobody had got to yet. Two changes
make the question more pressing. The JSeal integration roadmap (I14-I18) plans structured clone and
a second realm on a second thread for this profile. And the workloads and parity documents list the
pair as absent, while the ledger (roadmap.status.md) counts it among the declared-feature rows of
the remaining conformance failures. This record asks the question properly and
keeps the answer honest.

---

## 1. Where the exclusion is recorded today, and what a probe shows

**The exclusion appears in code, in rules, in the harness and in the documents, and all of them
agree.** The table covers the places that decide or enforce something. Places that only repeat
the reason are left out.

| Where | What it says or does |
|---|---|
| [`JsRealm.Binary.cs`](../../JsRealm.Binary.cs), the binary surface's remarks | "THERE IS NO `SharedArrayBuffer` AND NO `Atomics`, AND THAT IS THE POINT OF THIS PARAGRAPH." They need "an agent model this profile does not have" |
| [`JavaScriptProfile.BinaryManifest`](../../JavaScriptProfile.cs) and [`JsSurfaces.Binary`](../../../Broiler.VM.Profile.JavaScript.Format/JsSurfaces.cs) | "deliberately not in it", because folding them in "would let a composition that wanted an ordinary byte buffer admit cross-agent shared memory by accident" |
| [`JsArrayBuffer`](../../JsBinary.cs) | "The bytes are a plain `byte[]` and are never shared between agents." Detachment is the `Data` field becoming null |
| [The ledger's `absent-globals` block](../roadmap.status.md) | Lists `Atomics` and `SharedArrayBuffer`. Rule N17 (`src/tests/Broiler.VM.Architecture.Tests/N17RealmGlobalsRuleTests.cs`) fails if either name ever appears in [`realm/globals.txt`](../realm/globals.txt) while the block still calls it absent |
| `src/tests/cli/runs/a-typed-array-over-a-buffer.js`, pinned in `src/tests/cli/expected.txt` | Prints `typeof SharedArrayBuffer`, `typeof Atomics` and `typeof BigInt64Array`, and expects `undefined:undefined:undefined`. The fixture's comment says the row "goes red the day somebody folds them in". *(Amended 2026-09-22, JSeal B07: `BigInt64Array` now exists wherever BigInt is admitted, so the pinned line is `undefined:undefined:function`; the two shared-memory fields are unchanged.)* |
| `$262.agent` in [`JsRealm.Global.cs`](../../JsRealm.Global.cs) | Every member (`start`, `broadcast`, `getReport`, `sleep`, `monotonicNow`) throws "this profile runs no second agent" rather than answering `undefined` |
| [`Test262Adapter.cs`](../../../compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Adapter.cs) | Declines files flagged `CanBlockIsFalse`, `CanBlockIsTrue` or `non-deterministic` by name: "needs an agent and a shared memory this profile has none of". Other files that only *use* the pair are run and fail, and the ledger lists `SharedArrayBuffer` and `Atomics` among the declared-feature rows of the remaining failures |
| [Roadmap section 6](../roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted), [JSW-2](../roadmap.workloads.md#jsw-2--the-binary-surface-and-a-manifest-identity-for-it), [the parity roadmap](../roadmap.parity.md), [JSH-7](../roadmap.hosting.md) | The same exclusion, by name, with section 13 named as its owner |
| `WasmOpcode` in the WebAssembly profile | The `0xFE` atomics prefix is "deliberately absent" as well, so the other profile in this repository has no shared memory to offer either |

**On the JSeal side the exclusion is echoed and not decided.** The `BinaryData` capability reads
"SharedArrayBuffer is excluded". `VmRealm.Values.cs` in `Broiler.JSeal.Vm` says there is "no
`SharedArrayBuffer` to exclude here". The VM provider does not declare `WorkerRealms`, so
`VmRealm.Clone`, `Detach` and `Adopt` all throw `Lacking(WorkerRealms)`. Card I01 excludes
SharedArrayBuffer, and card I16 excludes "Shared-memory transfer".

**Probe, 2026-09-21.** The same script was run through the release CLI built from this worktree
and through Node v24.17.0:

| Observation | Broiler.VM CLI | Node |
|---|---|---|
| `typeof SharedArrayBuffer`, `typeof Atomics` | `undefined`, `undefined` | `function`, `object` |
| `"SharedArrayBuffer" in globalThis`, same for `Atomics` | `false`, `false` | `true`, `true` |
| `new SharedArrayBuffer(8)` | `ReferenceError` | constructs |
| Globals matching `/Shared\|Atomic\|Agent\|Worker/` | none | `Atomics`, `SharedArrayBuffer` |
| `typeof structuredClone`, `postMessage`, `Worker` | all `undefined` | `function`, `undefined`, `undefined` |
| `Object.getOwnPropertyNames($262)` | `agent,createRealm,detachArrayBuffer,evalScript,gc,global` | n/a |

Node was also used to confirm three facts about the specification that shape the rest of this
record. (1) `Atomics.add` and `Atomics.load` work on an `Int32Array` over an ordinary
`ArrayBuffer`, while `Atomics.wait` on one throws `TypeError` and `Atomics.notify` returns `0`.
(2) `structuredClone` of a `SharedArrayBuffer` **shares the bytes**: a write through the clone is
visible through the original. (3) Putting a `SharedArrayBuffer` in a transfer list is a
`DataCloneError`.

---

## 2. What admitting shared memory would actually ask of this profile

Each subsection below states what the tree does now (verified) and then what shared memory would
need on top of that. None of these needs is met today.

### 2.1 Agent and thread ownership

**Today there is one agent per instance, and one operation at a time inside it.** `JsEngine`
builds exactly one `JsRealm` in its constructor (`JsEngine.cs`), and nothing else creates a second.
The core refuses a second invocation while one is running: `VmInstanceImplementation.TryAdmit`
answers `ReentrancyRefused` in the `Executing` state. `JsExecution.RunOnGuestStack` starts a fresh
thread for each invocation. The host seam (`JsHostRealm.BeginStep`) records that thread's id and
refuses a touch from any other thread with `RealmNotCurrent`. Section 13 says "an agent is a
runtime" and that worker-style agents are separate runtimes under one aggregate budget. No code
creates such a sibling.

**Shared memory breaks one invariant the whole heap relies on: that bytes change only when this
agent's own code runs.** The binary built-ins lean on it directly. `JsRealm.Binary.cs` says "every
method that touches a buffer re-checks detachment ... after every step that could have run guest
code". With a shared block, bytes change between *any* two instructions, not only after guest code.
So every built-in that reads the same element twice or assumes a stable snapshot across a loop
(`sort`, `set` with overlapping ranges, `copyWithin`; `indexOf` and `join` for snapshot
consistency) would need to be checked against the specification's
shared-block semantics. Detachment checks do not help, because a `SharedArrayBuffer` cannot be
detached.

### 2.2 Shared accounting

**Today a buffer's cost belongs to one operation and one instance.** `BinaryNewBuffer` charges fuel
for the size before allocating, then reports retention with `engine.Retain`, which feeds
`LiveBytes`. [ADR-0007](../../../../docs/adr/0007-resource-authority-and-budgets.md) makes
`LiveBytes` a ceiling at runtime, instance and aggregate scope.

**A shared block outlives the operation that created it and is held by several instances.** If
retention stays with the creator, the block survives the creator's disposal with nobody paying for
it. If every holder is charged, one block is counted *n* times. The only scope in the core that
covers several runtimes is the aggregate budget (`VmAggregateBudget`,
`VmRuntimeCreationOptions.AggregateBudget`). That points at one rule: **a shared block may be
shared only between runtimes under one aggregate budget, and its retention is charged to that
aggregate exactly once.** Section 13 already says "a shared parent is a **channel**, not
isolation". Shared memory would make that channel explicit and addressable by index, so a host
that wants isolation must never be able to reach it by accident.

### 2.3 Synchronization and data races

**In the CLR, a racy read of a `byte[]` is not undefined behaviour; it can be a torn one.** The
runtime keeps type and memory safety, so a race cannot corrupt the heap. But the ECMAScript memory
model requires more than safety. Aligned integer accesses through an integer typed array must be
tear-free, and `Atomics` operations must be sequentially consistent. The element readers in
`JsBinary.cs` go through `BinaryPrimitives` over a span. That is correct for one agent, but it
promises nothing about atomicity. A shared build would need:

- element accessors for shared blocks built on `Volatile` and `Interlocked` at the element width,
- a stated treatment of the 8-byte widths, which only arrive with BigInt (cards B01-B08), and
- an `Atomics.isLockFree` answer derived from those accessors rather than copied from another
  engine.

**Races between agents are the program's business. Races inside the engine are not.** The
guarantee this profile would have to state is narrower than "no data races". Guest data races on a
shared block may yield any value the memory model allows, and they must never reach engine state:
lengths, the fact that a view is shared, or the waiter lists in 2.4. That is also why a shared
block must never be reachable from a non-shared `ArrayBuffer` object. Two different types is the
current design, and it has to stay that way.

### 2.4 Wait, wake, and how long a wait lives

**`Atomics.wait` blocks the agent's thread, and today nothing in this profile may hold a thread
while paused.** [JSD-0023](0023-which-pauses-route-through-a-core-suspension.md) routes a pause
through a core suspension only at a host-asked step. Its own falsifier is "a pause that outlives
the turn it was taken in". A blocking wait is exactly that: an operation that is neither running
nor suspended. So before `wait` can exist, the following have to be decided:

- **Whether an agent may block at all** (the specification's `[[CanBlock]]`). An instance driven
  by the host through `StepEntryPoint` is the event loop, and it should answer `false`, as a
  browser main thread does. Only an agent the host created as a worker could answer `true`.
- **What a blocked wait is charged to.** It spends no fuel, so the fuel poll in `JsEngine` never
  runs during it. The wait therefore has to sleep on a handle that the operation's cancellation
  token and its `WallClock` deadline also signal. A wait with no timeout is still bounded by the
  operation's allowance, never by the program.
- **Who owns a waiter record, and when it is removed.** A waiter is entered in a list keyed by the
  shared block and index. It must be removed when its operation is cancelled, exhausted, faulted or
  disposed, so that a later `notify` never wakes something that no longer exists. `notify`
  returning a count means the count must be exact under those removals.
- **`Atomics.waitAsync`** is a promise settled by another agent. Its settlement has to arrive as a
  job the host drains, which is the same shape card D03 asks of `FinalizationRegistry` cleanup. It
  can never run on the notifying agent's thread.

### 2.5 Cancellation and disposal

**Every JavaScript abort today is local to one operation.** A `JsAbort` (cancelled or exhausted)
unwinds one guest stack. Once blocks are shared, an abort in one agent must leave every other agent
consistent. Concretely: a shared block stays valid while any holder is alive; an agent that dies
while blocked in `wait` releases its waiter record (2.4); and disposal of the *last* holder is what
releases the aggregate's retention (2.2). No test can assert *which* sibling observes an aggregate
exhaustion, because section 13 already rules that out.

### 2.6 Embedding policy

**Admission has to be its own question, as the binary surface's remarks demand.** That means
another manifest identity beside `broiler.javascript.binary`. This record calls it
`broiler.javascript.shared` for discussion only and does **not** allocate it. Allocation belongs to
the decision that reopens the question (section 5). Composition options would have to say which
runtimes form one agent cluster and whether each agent may block. On the JSeal side it would be a
capability of its own. Neither `BinaryData`, whose contract already excludes shared buffers, nor
`WorkerRealms`, which card I18 requires to be earned by clone and transport alone, may silently
come to mean shared memory.

### 2.7 How this meets JSeal worker realms and structured clone (I14-I18)

**The clone carrier planned in I14 is the wrong shape for shared memory, and that is deliberate.**
I14 requires "a detached carrier containing clone data, not live source-realm handles". I17
requires "guest objects out of cross-thread carrier state". A cloned `SharedArrayBuffer` is, by
the specification (probe, section 1), a *live* reference to a block both sides then write. Carrying
one would need a third kind of carrier entry: not data, and not a transferred owner, but a counted
share of a block held under an aggregate budget. So:

- **I14-I18 go ahead without shared memory, and should.** Worker realms need a second realm, a
  second thread and a copying transport. None of that depends on sharing, and the Broiler.JS
  provider shows the split is coherent: it declares `WorkerRealms` while its `BinaryData` excludes
  `SharedArrayBuffer` by hand (`BroilerJsRealm.Values.cs`).
- **Worker realms come first, shared memory second, never the reverse.** A shared block with no
  second agent to share it with is the "globals for a `typeof` comparison" that card D02 excludes.
- **I15's supported-type matrix should list `SharedArrayBuffer` as a row that says "absent from
  this realm; not a clone brand"**, rather than leaving it off. A matrix that omits it invites the
  reading that it was forgotten.

---

## 3. What was considered

| Option | What it does | Why it was not taken |
|---|---|---|
| **Add both globals as stubs** | `typeof` answers like Node's, and every operation throws | It is the thing card D02 excludes by name. It would also turn N17's absent list and the CLI fixture into lies, and make the declined `CanBlock` files look runnable |
| **`Atomics` alone, over ordinary buffers** | The specification allows every `Atomics` operation except `wait` on a non-shared integer array. With one agent, each one is the plain read-modify-write | The pair (`wait`, `notify`) that makes the namespace mean anything would either throw (`wait`) or return `0` (`notify`) for ever. The gain would be a present global and some passing test262 rows about argument validation. The cost is splitting an exclusion recorded as a pair in every place listed in section 1, each of which would have to be re-decided. Rejected for now, and named in section 5 as the cheapest step once an agent model exists |
| **A single-agent `SharedArrayBuffer`** | Constructible, with `growable` semantics, and nobody to share it with | Nothing could observe the sharing, so every shared-semantics claim would be untested. It would also force the section 2.2 accounting rule to be decided with no second holder to test it against |
| **Design and build the agent model now** | Sections 2.1-2.6, implemented | Its prerequisite, a second realm on a second thread (JSH-7, I17), does not exist, and none of 2.1-2.6 could be tested before it does. It also serves no workload this profile has measured. The workload roadmap declines to promise the pair |
| **Keep the exclusion, write down why and what would reopen it** | This record | Taken, as a proposal |

---

## 4. The decision (proposed)

**`SharedArrayBuffer` and `Atomics` stay absent from every manifest this profile admits, as a
pair, and the reason on record is the list in section 2 rather than the one-line phrase "they need
the agent model".**

- **No global, no stub, no partial namespace.** `typeof` answers `undefined` for both, and
  `$262.agent` keeps refusing.
- **The inventories stay as they are, because they are already honest.** N17's absent block keeps
  both names. The CLI fixture keeps its `undefined:undefined:undefined` row. The conformance
  harness keeps declining `CanBlock*` files by name, and keeps counting files that merely *use*
  the pair as failures rather than moving them into an exclusion. Excluding them would raise the
  pass rate by hiding rows, which is what [JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md)
  refuses to do for anything that is not a proposal.
- **The binary identity never widens to include them.** If they ever arrive, they arrive under an
  identity of their own (2.6).
- **JSeal: nothing changes.** `BinaryData` keeps excluding shared buffers. `WorkerRealms`, once
  I14-I18 earn it, means copy-and-transfer only. Card I16's shared-memory exclusion stands.
- **The WebAssembly profile's `0xFE` refusal is consistent with this, and nothing here decides it.**
  Shared memory between a JavaScript agent and a WebAssembly instance would additionally need the
  cross-profile value channel that [JSD-0007](0007-cross-profile-position-and-amendment-grading.md)
  refuses.

---

## 5. What would reopen it

The exclusion is reopened by a new dated record that supersedes this one. **That record may be
written only when all three of the following are true:**

1. **A second agent exists.** JSH-7 and I17 have shipped: a second realm on a second host thread,
   with a clone transport whose tests pass. Without that, nothing in section 2 can be tested.
2. **A workload or an embedder needs it**, and the need is recorded. For example, a measured
   workload that fails only for want of the pair, or an embedder asking for worker-shared buffers
   under an aggregate budget. Test262 rows alone are not such a need.
3. **The reopening record answers sections 2.1-2.6 in its own text.** That covers: the agent
   cluster and its aggregate budget; the once-only retention rule; the element-width atomicity of
   the shared accessors; `[[CanBlock]]` per agent; the waiter record's owner and when it is
   removed; the route for `waitAsync`; the manifest identity it allocates; and the JSeal capability
   it proposes.

**Data races (2.3) and wait lifetime (2.4) are gates on exposure, not follow-ups to it.** No
global may appear until the slices in section 6 that cover them are accepted.

---

## 6. Staged implementation if it is reopened (not scheduled)

These are the slices a reopening record would schedule. They are listed here so that the cost is
visible, not because anything is planned. Each one is bounded and has its own gate. The first three
expose no global.

| Slice | Delivers | Accepted when |
|---|---|---|
| **S1 - shared block and accounting** | An internal `JsSharedBlock` type, never reachable from `JsArrayBuffer`, retained once against an aggregate budget and released with its last holder | Two runtimes under one aggregate hold one block and `LiveBytes` rises once. Disposing either holder keeps it; disposing both releases it. A runtime with no aggregate is refused the share by name. A negative control that charges per holder is watched failing |
| **S2 - atomic element access** | Shared-block accessors at widths 1, 2 and 4 built on `Volatile` and `Interlocked`, and an audit of every binary built-in that reads an element twice or assumes a stable snapshot across a loop | Every shared-block access at widths 1, 2 and 4 goes through `Volatile`/`Interlocked`, enforced by an architecture rule with a watched-failing negative control over the plain `BinaryPrimitives` path; a two-thread stress case over aligned `Int32` elements is a supporting check only, since a fixed iteration count can fail to find tearing but cannot prove its absence. Every built-in in the audit has a named disposition. A race cannot change a length or a view's shared/non-shared flag |
| **S3 - waiter lists and blocking** | A waiter record keyed by block and index; `wait` sleeping on a handle that cancellation and the `WallClock` deadline also signal; `[[CanBlock]]` false for any step-driven instance | `wait` on a step-driven instance throws. Cancelling a blocked operation returns the right abort within the poll bound and removes the waiter. `notify` counts are exact after a waiter's operation is exhausted. No blocked wait survives disposal |
| **S4 - exposure behind its own identity** | The two globals under a newly allocated manifest identity; N17's absent block, the CLI fixture and the parity rows updated in the same change | A composition that declines the identity refuses an artifact naming it, at verification. `$262.agent` implemented over real second agents. `CanBlock*` files moved from declined to scored, with the numbers recorded |
| **S5 - clone and JSeal** | A counted-share carrier entry in the I14 carrier, and a JSeal capability separate from `WorkerRealms` | Cloning a shared buffer across two VM realms shares bytes, as the Node probe in section 1 shows. Transferring one is a `DataCloneError`. A realm without the new capability refuses to carry one |

`waitAsync` is a sixth slice after S3. It waits on the host-drained job route that card D03 is
deciding, and it should not be designed before D03 is decided.

---

## 7. What would falsify this record

- **Either name appears in `realm/globals.txt`** while this record is still in force. N17 and the
  CLI fixture would both fail, and that is intended.
- **A composition reaches shared bytes through the binary identity**, for example by a
  `JsArrayBuffer` whose `Data` array is aliased into a second instance. The one-agent invariant in
  2.1 would then be false without anyone having decided it.
- **`WorkerRealms` is declared for the VM provider with a transport that aliases bytes** rather
  than copying or transferring them. That would be shared memory arriving without the gates in
  section 5.
