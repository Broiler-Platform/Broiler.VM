<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0023 — Which pauses route through a core suspension, and the count that bounds them

**Status:** Taken. 2026-09-06.

**Owner:** MaiRat. **Co-signer:** none. **Both roles are held by one person**, and this record
does not claim the co-signature is independent — there is no second signature to claim it of.

**Context.** Roadmap [section 12](../roadmap.md#12-suspension-generators-async-functions-and-top-level-await)
names three pause kinds and asks JS-7 to decide, per kind, which are represented as a **core
suspension** and which as this profile's own job records — **with the live-suspension count a
representative workload produces**, because routing every microtask through a core suspension would
make the suspended-operation limit the thing that governs a page. The decision had not been taken
and nothing produced the fifth step kind at all.

---

## 1. What each of the three kinds costs if it routes

**A generator `yield` or an `await`.** These already suspend, on a frame the executor captures on
the heap and reconstitutes, inside one operation. Routing them would mean a core suspension per
`await` — and an ordinary promise chain performs one per link. A page that awaits in a loop would
mint suspensions at the rate it iterates, and each one is an object the runtime holds until it is
resumed or the operation is abandoned.

**Instantiation parked on top-level await.** A module graph is evaluated inside an **invocation**
here — the composition asks for one entry point and the graph runs there — so nothing parks an
instantiation today. Routing this kind means declaring asynchronous instantiation and moving the
graph's evaluation to `Instantiate`, which is a change to when a host gets an instance rather than
to how a pause is represented.

**A host or diagnostic client pausing execution.** A double gate: the descriptor declares it and
the runtime enables it. At core contract version 1 **an executor cannot see that it has been
asked** — nothing on the execution environment reports the request — so a profile that declared it
could keep the promise only where the guest happened to reach a suspension point of its own.

---

## 2. What was considered

| Option | What it does | Why it was not taken |
|---|---|---|
| **Route every guest pause** | Each `yield` and each `await` answers `Suspended` | It is the shape section 12 names as the failure: the live-suspension count becomes a function of how often a program awaits, so the runtime's `maxLiveSuspendedOperations` starts governing ordinary programs. It also makes every promise chain a round trip through the host |
| **Route nothing, and keep the drain as the only surface** | A host asks for a drain and gets the queue run to exhaustion | It is what existed, and it leaves the fifth step kind produced by nothing — JS-1 declared it produced at JS-7 and JS-7 would have closed with it still unreachable. It also gives an embedder no way to interleave its own work between two turns, which is the thing an event loop is |
| **Declare external suspension and park at the next safepoint** | A host calls `RequestSuspend` and the profile parks | The profile cannot see the request. Declaring it would be a promise kept by luck, and a declaration true by luck is worse than an absent one, because a host reads it as a capability |
| **Route the turn, and only where a host asked for it** | A second reserved entry point runs one due job and parks if any remain | Taken |

---

## 3. The decision

**A pause routes through a core suspension exactly when a host asked for the turn as its unit, and
in no other case.**

- **The guest kinds stay guest kinds.** A `yield` and an `await` suspend on a heap frame inside one
  operation and mint no core suspension. Section 12's first row is unchanged.
- **A second reserved entry point,** published as `JavaScriptProfile.StepEntryPoint`, runs one due
  job and answers `Suspended` with a continuation and a projection while anything is still queued.
  A host that invokes it drives the event loop turn by turn; a host that invokes the drain instead
  gets the queue run to exhaustion in one operation, exactly as before; a host that invokes neither
  creates no suspension at all.
- **External suspension stays undeclared**, and a host that calls `RequestSuspend` is told
  `ExternalSuspensionNotDeclared` — which is the truthful answer and one half of the pair section 12
  asks to be told apart. The other half needs a descriptor that declares the row.
- **Asynchronous instantiation stays undeclared** and no instantiation parks. Section 12's second
  row is still the design for a host that wants the graph settled before it holds an instance, and
  it is still not what a caller gets.

---

## 4. The count, which is what section 12 asks this record to carry

**The live-suspension count this routing produces is at most one per instance being stepped, and
zero for every instance nobody steps.** It is a property of the embedding rather than of the
program: a pause exists only between the invocation that asked for a turn and the resume that takes
the next one, and there is one such pause outstanding at a time per instance because a step runs one
job and answers.

**What a representative workload produces is therefore the number of instances a host chooses to
step concurrently.** The conformance harness and the end-user host both drain rather than step, so
both produce zero; a host driving *n* instances through an event loop of its own produces *n*. That
is a number an embedder sets against `maxLiveSuspendedOperations` deliberately, which is the
property the rejected routing would have taken away.

---

## 5. What would falsify this

- **A pause that outlives the turn it was taken in.** If a step ever parks with work captured on a
  stack rather than in the queue, the claim that a pause holds no thread stops being structural.
- **A guest pause that mints a core suspension.** If a `yield` or an `await` ever answers
  `Suspended`, the count above stops being a property of the embedding.
- **The environment gaining a way to see an external suspend request.** A later core contract
  version that reports the request makes the third kind routable, and this record's refusal to
  declare it would then be the thing to revisit rather than the state of the tree.
