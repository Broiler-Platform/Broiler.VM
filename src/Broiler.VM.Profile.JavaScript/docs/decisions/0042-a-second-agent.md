<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0042 - A second agent: a runtime its host starts, holding a block another agent made

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the second slice of phase F6 that [JSD-0041](0041-shared-memory-in-one-agent.md)
section 5 names: it answers [JSD-0028](0028-shared-memory-and-atomics.md)'s sections 2.1 to 2.6 for
**several agents**, builds the host's half of handing a shared block from one agent to another, and
gives the conformance runner `$262.agent` over real second agents.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the second record of phase F6 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

---

## 1. What an agent is

- **An agent is a runtime, and its host starts it**, as roadmap section 13 says. The profile starts
  none: the edition defines no way for a program to create an agent, and the profile's own
  `$262.agent` still refuses every member. A host that starts agents builds each one as a runtime
  of its own - its own catalog, its own instance, its own guest thread per invocation - and drives
  it as it drives any other.
- **Two agents hold nothing in common but a shared block.** No object, realm, job queue or Symbol
  registry crosses; each agent has its own `[[CanBlock]]`, answered by its own host surface through
  `IJsHostAgentPolicy` (JSD-0041 section 2.4).

## 2. How a block crosses

- **`JsHostRealm.ShareBlock(buffer)`** answers the block a `SharedArrayBuffer` of that realm is over,
  as an opaque `JsHostSharedBlock` that holds nothing of the realm and may be carried to another
  thread. **`JsHostRealm.AdoptBlock(block)`** answers a new `SharedArrayBuffer` of the adopting
  realm over the same block. Nothing is copied: a write through either agent's views is a write to
  the one block, and an `Atomics.notify` in either wakes a waiter in the other, because waiters are
  keyed by the block and the index (JSD-0041 section 2.4).
- **Only a fixed-length shared buffer's block is handed out.** An `ArrayBuffer` has no block, and a
  growable block replaces its storage when it grows (JSD-0041 section 3), so another agent still
  writing to the storage it replaced would lose the write - an `Atomics` one included, which the
  memory model forbids. Each is a guest `TypeError`.
- **A realm whose composition declined `broiler.javascript.shared` adopts nothing**: `AdoptBlock`
  is a guest `TypeError` there, so a block never reaches a realm that has no
  `SharedArrayBuffer` to see it through.

## 3. JSD-0028's questions, for several agents

| Section | Question | Answer here |
|---|---|---|
| 2.1 | Agent and thread ownership | **Each agent is single-threaded, as before**: one operation at a time on its own guest thread. The block is the only state two agents reach, and everything about it that is not guest bytes - its length, its sharedness, its waiter list - is read and written under its lock or never written after it is made. |
| 2.2 | Shared accounting | **The block is charged once, to the operation that made it**; adopting it charges one crossing and no bytes. Worker agents share **one aggregate budget** (roadmap section 13), so a host ceiling is shared rather than multiplied; the conformance runner sizes it at one test's allowance in every aggregate dimension, with at most eight live agents. Retention follows the creating agent, not the last holder - section 5. |
| 2.3 | Synchronization and data races | **As JSD-0041 decided, now with a second writer**: `Atomics` accesses are sequentially consistent across agents, because the four- and eight-byte ones are `Interlocked` over the one array and the one- and two-byte ones take the one lock. Plain element reads and writes are unchanged, so a race between them is the program's, and never reaches engine state. Growth would be the engine's race, and is refused at the crossing. |
| 2.4 | Wait, wake, how long a wait lives | **As JSD-0041 decided, now across threads, with three corrections**: a blocking wait in one agent sleeps on the block's lock and a notification from another pulses it; a `waitAsync` waiter is settled at its own agent's host drain - **between two jobs as well as once the queue is empty**, because settling it is a job the host enqueues and the edition leaves its order against promise jobs to the host; a drain whose queue is empty **waits for a waiter with no deadline when its block has crossed to another agent**, woken by the notification's pulse, and returns at once when nothing could ever notify it; and **deadlines are read on the high-resolution clock**, so a wait never ends before its timeout by the clock a program measures it with. |
| 2.5 | Cancellation and disposal | **An agent's abort is its own**: it removes its own waiters as it unwinds (JSD-0041 section 2.5) and leaves the block to the others. The conformance runner cancels every agent when its test's verdict is reached, which ends any wait within one poll slice, then joins and disposes them before the test's own runtime. |
| 2.6 | Embedding policy | **Two host doors and one answer per agent**: `ShareBlock` and `AdoptBlock`, and `IJsHostAgentPolicy.CanBlock`. Which runtimes are agents of one cluster is the host's to say, by building them under one aggregate budget and handing the block between them. |

## 4. The conformance runner's `$262.agent`

- **`start(source)`** compiles the source with the test's own compile request, builds a runtime from
  the test's manifest whose host surface is the agent's and whose `[[CanBlock]]` is `true`, runs the
  source on a thread of the agent's own, and returns. The agent's `$262.agent` has
  `receiveBroadcast`, `report`, `leaving`, `sleep` and `monotonicNow`.
- **`broadcast(buffer, id)`** shares the buffer's block with every agent started so far and returns
  once each has taken it, as INTERPRETING.md requires, bounded by the test's wall clock. Each agent
  then runs its callback in a host turn - adopting the block as its own `SharedArrayBuffer` - and
  drains its jobs. The id crosses only as a primitive.
- **`getReport()`** answers the oldest report not yet read, or `null` - after waiting up to 100 ms for
  one while an agent is still running and has not said it is leaving. The wait is the host's to choose:
  without it, a test that asks a moment after the notification that produces the report answers
  `null`, and the suite's stand-in for `setTimeout` then spins promise jobs for a second, which this
  profile charges to the operation's live bytes until they are spent. `report` converts with
  `ToString`, as other hosts do. `sleep` and `monotonicNow` are the same in every agent, and every
  agent's clock is one clock.
- **An agent's source runs, then its jobs drain**, before it takes any broadcast; each broadcast is a
  host turn that runs its callback, then a drain.
- **The test's agents share one aggregate budget** made at the first `start`: every aggregate
  dimension is bounded at the test runtime's own ceiling, and `LiveRuntimes` at eight, and each agent
  adopts what is left of it rather than declaring an allowance of its own. The test's own
  runtime is not under it, because it is created before anything knows whether the test will start
  an agent; a test and its agents together may spend two allowances, never more.
- **The suite's `CanBlockIsFalse` cases run** where the test's main agent may not block: the same
  composition with a host surface that answers `[[CanBlock]]` `false`, as a browser's main thread
  does. Every other test's main agent may block, as a shell's does.
- **Observed**: `test/built-ins/Atomics` passes 752 of its 752 scored variants, in three consecutive
  runs; its 6 skipped files claim `Atomics.pause`, which the pinned suite lists among proposals.

## 5. What is not yet built

- **Growable blocks across agents.** Holding a growable block's storage reserved at its maximum
  length, so growth never replaces it, would let one cross; until then the crossing refuses it.
- **Retention that follows the last holder.** The block's bytes are retained by the agent that made
  it and released with that agent's heap, as an `ArrayBuffer`'s are; an embedder that disposes the
  creator while another agent still holds the block keeps bytes nobody is charged for. The
  conformance runner disposes every agent first. JSD-0028's S1 asks for retention against the
  aggregate, once, released with the last holder.
- **JSD-0028's S2 rule and audit**: the architecture rule over the plain element path, and the
  audit of every binary built-in that reads an element twice or assumes a stable snapshot across a
  loop, which a second writer now makes observable.
- **The clone carrier's counted-share entry and a JSeal capability** (S5): `structuredClone` and a
  transfer list still refuse a shared buffer.

## 6. What would falsify this

- A block crossing to another agent by any door but `ShareBlock` and `AdoptBlock`, or one of them
  copying the bytes.
- A growable block, or anything but a `SharedArrayBuffer`'s block, handed out; or a realm that
  declined `broiler.javascript.shared` adopting one.
- An `Atomics` access in one agent that is not sequentially consistent with one in another, or a
  notification in one agent that does not wake a waiter of another on the same block and index.
- A test's agent outliving the test, or a test's agents spending more than one test's allowance in
  any aggregate dimension.
