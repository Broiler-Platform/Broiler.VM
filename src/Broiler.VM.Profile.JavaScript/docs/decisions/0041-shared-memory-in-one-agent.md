<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0041 - Shared memory and `Atomics`, in one agent first, behind `broiler.javascript.shared`

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the successor [JSD-0028](0028-shared-memory-and-atomics.md)'s reopened section asks
for: it answers that record's sections 2.1 to 2.6 for **one agent**, builds what one agent needs of
the slices in its section 6 (section 4 below says what each still owes), and mints the identity
section 2.6 named for discussion. A second agent, and with it a block several agents hold, is phase
F6's next slice and a section of this record or its successor.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the first record of phase F6 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

---

## 1. The identity

- **`broiler.javascript.shared`** owns the globals `SharedArrayBuffer` and `Atomics`. A program
  naming either declares it; a composition that declines it refuses that program at verification
  and builds neither global. **It is admitted only together with `broiler.javascript.binary`**,
  never folded into it (JSD-0028's reason): a descriptor naming it without the binary surface is
  refused when it is built, and the conformance runner's `--decline` of the binary surface declines
  it too. Every door admitting every surface admits it.
- `SharedArrayBuffer` and `Atomics` are in the pinned edition, so no proposal is admitted; the suite's
  `SharedArrayBuffer`, `Atomics`, `Atomics.waitAsync` and `Atomics.pause` features are standard ones.

## 2. JSD-0028's questions, for one agent

| Section | Question | Answer here |
|---|---|---|
| 2.1 | Agent and thread ownership | **One agent per instance, as before.** A shared buffer's bytes live in a *shared block* the buffer object points at, so the block and not the object is what a second agent will hold; until then only one agent ever reaches it. |
| 2.2 | Shared accounting | **The creating operation pays**, as for an `ArrayBuffer`: fuel for the bytes, then the live-bytes ceiling asked before the block exists; a growth is asked again past the block's high-water mark. Charging a block several agents hold to their aggregate once is the agent slice's. |
| 2.3 | Synchronization and data races | **Every `Atomics` access is sequentially consistent.** Four- and eight-byte elements use a compare-and-swap loop over the element, which is lock-free; one- and two-byte elements take the block's lock, so `Atomics.isLockFree` answers `true` for 4 and 8 and `false` for 1 and 2 - the edition makes only the answer for 4 normative. Plain typed-array reads and writes are unchanged. Engine state - lengths, sharedness, waiter lists - is never in guest bytes. |
| 2.4 | Wait, wake, how long a wait lives | **`[[CanBlock]]` is the host's**, through `IJsHostAgentPolicy`: an agent whose host surface does not say `true` is an event loop, and its `Atomics.wait` is a `TypeError` after the arguments are converted. A blocking wait sleeps on its block's lock in slices of 20 ms and, between them, asks cancellation and the meter's poll - which owns the wall clock - so it ends when the operation's allowance does and spends no fuel while it sleeps. A waiter is removed however its wait ends. **`waitAsync` is settled only at a host drain**, as a `FinalizationRegistry` cleanup is (JSD-0029): a notification marks the waiter, a timeout expires it, and `#drain-jobs` or `#step-jobs` settles its promise, whose reactions are ordinary jobs; a `#drain-jobs` whose queue is empty while a waiter of this agent has a deadline waits for it, bounded the same way. |
| 2.5 | Cancellation and disposal | **An abort during a wait removes its waiter** before it unwinds. A block lives while any buffer or view reaches it; nothing else holds it. |
| 2.6 | Embedding policy | **The identity in section 1, and `IJsHostAgentPolicy`**: a composition says whether it admits shared memory and, separately, whether its agent may block. The conformance runner answers `true`, as a shell does, and the CLI does not answer. |

## 3. What is built

- **`SharedArrayBuffer`**: the constructor with the options bag (`maxByteLength` makes it growable),
  `byteLength`, `growable`, `maxByteLength`, `grow` (which only grows), `slice` through the species,
  `Symbol.species` and `Symbol.toStringTag`. Every `ArrayBuffer.prototype` member refuses a shared
  buffer and every member here refuses an unshared one; `$262.detachArrayBuffer`, a transfer list and
  the clone carrier refuse a shared buffer. The typed arrays and `DataView` read and write one as any
  buffer, and a length-tracking view follows its growth.
- **`Atomics`**: `add`, `and`, `compareExchange`, `exchange`, `isLockFree`, `load`, `notify`, `or`,
  `pause`, `store`, `sub`, `wait`, `waitAsync` and `xor`, with the edition's validation order -
  `ValidateIntegerTypedArray`, `ValidateAtomicAccess`, the operand conversions, then
  `RevalidateAtomicAccess` - over every integer view, shared or not, and `wait`, `waitAsync` and a
  `notify` that wakes anything over a shared `Int32Array` or `BigInt64Array`.
- **A growable block replaces its array when it grows**, under its lock, as a resizable
  `ArrayBuffer` does. With one agent nothing can race that; a block two agents hold will need its
  storage reserved instead, and the agent slice decides it.

## 4. JSD-0028's slices, and what each still owes

| Slice | Built for one agent | Still owed |
|---|---|---|
| S1, the shared block and its accounting | The block, held by the buffer object and charged to the operation that made it | Retention against an aggregate budget, once, released with its last holder - which needs a second holder |
| S2, atomic element access | Every `Atomics` access through `Interlocked` at four and eight bytes and under the block's lock at one and two | The architecture rule over the plain element path, and the audit of the binary built-ins that read an element twice, which only matter once a second agent can write |
| S3, waiter lists and blocking | Waiters keyed by block and index; a blocking wait bounded by cancellation and the meter's poll; `[[CanBlock]]` false unless the host surface says otherwise | Nothing for one agent |
| S4, exposure behind its own identity | The two globals under `broiler.javascript.shared`, the `absent-globals` block, the CLI fixture | `$262.agent` over real second agents, and the `CanBlockIsFalse` cases |
| S5, clone and JSeal | Nothing: the carrier refuses a shared buffer | The counted-share carrier entry and a JSeal capability |
| The sixth, `waitAsync` | Settled at a host drain, as JSD-0029 decided for cleanup callbacks | Nothing for one agent |

## 5. What is not yet built

- **A second agent**: `$262.agent`, worker agents, a block held by several, and the `CanBlockIsFalse`
  cases that need an agent which may not block. The suite's cases that start an agent still fail at
  `$262.agent.start`'s refusal.
- **Sharing a block through the clone carrier**, which JSD-0028 section 2.7 says needs a third kind of
  carrier entry.

## 6. What would falsify this

- An `Atomics` access that is not sequentially consistent with another `Atomics` access to the same
  element, or a four-byte one that takes a lock.
- A blocking wait in an agent whose host did not say it may block, a wait that outlives its
  operation's allowance, or a waiter left in a list after its wait ended.
- A `waitAsync` promise settled anywhere but at a host drain or step.
- A shared buffer detached, transferred or copied into a clone carrier, or reachable as an
  `ArrayBuffer`.

## The second agent, 2026-10-04 (unsigned)

*Recorded with phase F6's second slice; it signs nothing and this record keeps its status line.
Corrections entry [JSC-267](../roadmap.corrections.md#jsc-267).*

- **Section 5's first item is built** under proposed [JSD-0042](0042-a-second-agent.md): a second
  agent is a runtime its host starts, a fixed-length block crosses to it through
  `JsHostRealm.ShareBlock` and `AdoptBlock`, and the conformance runner's `$262.agent` starts real
  agents and runs the suite's `CanBlockIsFalse` cases where the main agent may not block.
- **Section 3's growable block is decided for now by refusal**: a growable block is not handed to a
  second agent, because its growth replaces its storage. Section 4's S1 retention against an
  aggregate, S2's rule and audit and S5's carrier entry are still owed, as JSD-0042 section 5 says.
- **Section 2.4 is corrected in three places** (JSD-0042 section 3): a drain also settles a waiter
  that is due between two jobs; it waits for a waiter with no deadline when that waiter's block has
  crossed to another agent; and deadlines are read on the high-resolution clock.
- **Section 1 was wrong about `Atomics.pause`**: the pinned suite lists that feature among proposals,
  and the runner skips its six files. The member is built; admitting the proposal is not decided.
