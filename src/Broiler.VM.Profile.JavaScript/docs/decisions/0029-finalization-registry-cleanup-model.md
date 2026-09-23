<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0029 - FinalizationRegistry cleanup: a sweep at the host's drain point, and nothing from a finalizer

**Status:** Proposed. 2026-09-21. **The current inert registry stays the default and this record
does not change it.** What this record proposes is an optional cleanup model and one bounded
implementation slice for it (section 7). Nothing described in sections 4 to 6 exists in the checkout.

**Owner:** MaiRat. **Co-signer:** none. **Both roles are held by one person**, and the record has
not been signed by either yet. It is written for the owner to take or refuse, and it does not claim
an approval it has not been given.

**Origin:** card D03 of the Broiler.JSeal plan `docs/roadmap.vm-features.md` ("Decide a
host-drained FinalizationRegistry cleanup model"). The one member this record also touches,
`cleanupSome`, is already owned by JSP-7 in the [parity roadmap](../roadmap.parity.md), and
section 4.9 below defers to that stage rather than taking its decision.

**Context.** The realm exposes `FinalizationRegistry` and never runs a cleanup callback. That is a
declared divergence written into the type's own remarks, and its high-security falsifier reads
*"a cleanup callback registered here is ever invoked, or any guest code runs from a CLR
finalizer"* (`JsCollections.cs`, `JsFinalizationRegistryObject`; the same line is in
`HUMAN_REVIEW.md` against fingerprint `66E399`). The remarks name the shape a later revision could
build - a sweep at a drain point that enqueues the callback as an ordinary job - and say it was not
built because *when a target counts as collected* had not been decided. This record decides that
question, and the questions that come with it, without building anything.

---

## 1. What the registry does today

Every row below was read in `JsRealm.Collections.cs` (`SetupWeakReferences`) and
`JsCollections.cs` (`JsFinalizationRecord`, `JsFinalizationRegistryObject`), and the observable
rows were probed on 2026-09-21 with the wide host built from this checkout
(`Broiler.VM.Composition.JavaScript.Cli.exe --quiet`) and compared with Node v24.17.

| Behaviour | This profile | Node v24.17 |
|---|---|---|
| `new FinalizationRegistry(nonCallable)`, and a call without `new` | `TypeError`, both | `TypeError`, both |
| `register(target, held, token)` | Validates, charges fuel 1 and `LiveBytes` 96 (`CollectionEntryBytes`), appends a record, answers `undefined` | Same answer |
| `register(o, o)` | `TypeError`: target and held value must differ | `TypeError` |
| Primitive target, primitive token | `TypeError` | `TypeError` |
| **Symbol target or token** | **`TypeError`** | A non-registered symbol is accepted; `Symbol.for(...)` is a `TypeError` |
| `unregister(token)` | Removes every record naming the token and answers whether it removed one; charges `Count + 1` fuel. Two calls in a row answered `true` then `false` | Same |
| `cleanupSome` | Present; validates an optional callback and does nothing. `typeof r.cleanupSome` is `"function"`. It checks the callback before the receiver, the reverse of the TC39 `cleanupSome` proposal (`cleanupSome.call({}, 1)` reports the callback); both orders give a `TypeError` | **Absent** - `r.cleanupSome is not a function` |
| Cleanup callback | **Never called.** Twenty thousand registrations of unreachable targets, then a drained job: the callback count was still 0 | Called after a collection, as a host task (with `--expose-gc` and `gc()` the four held values arrived as `h2`, `h1`, `h0`, `x`) |

**Four properties of the record are load-bearing for everything after this section.**

- **A record holds its target weakly, its held value strongly, and its token weakly.**
  `JsFinalizationRecord` wraps target and token in `System.WeakReference<JsObject>` and keeps
  `Held` as a plain `JsValue`. A held value therefore lives as long as the registry does.
- **A record whose target has died is never removed.** `Register` only appends; `Unregister`
  drops a record only when its token matches or its *token* has been collected. A token-less
  registration of an unreachable target stays in the list, with its held value, until the registry
  itself becomes unreachable. That is a real cost, and section 4.4 explains why it is kept rather
  than fixed on its own.
- **`unregister`'s answer does not depend on the collector.** Because nothing is pruned by target
  liveness, `unregister(token)` answers `true` for a registration whose target is long gone, which
  is also what the specification answers for a cell nobody has cleaned up yet. The answer is a
  function of the program alone.
- **The callback is validated at construction and held, and that is all.**
  `JsFinalizationRegistryObject.Cleanup` keeps it for identity.

**No test covers any of it.** A search of `src/tests` finds neither `FinalizationRegistry` nor
`WeakRef`. The divergence is recorded in remarks and in the review register, and no test enforces it.

## 2. The weak reference it sits beside

`JsWeakRefObject` holds a `System.WeakReference<JsObject>`. The first successful `deref` also keeps
the target in a strong `kept` field **for ever**. The type's remarks explain why: the specification's
`AddToKeptObjects` needs the target alive until the end of the current job. No built-in here can see
the current job, so this profile keeps the target longer than required, which the specification
allows. Two consequences matter below. A target that has ever been dereferenced can never be
reported collected. And before its first read, a `WeakRef` observes the collector at a moment the
program cannot choose. The remarks call this the one case "no implementation of this type can
avoid". The probe confirmed that `deref()` answers an object and answers the same one twice.

Two more facts bound the design:

- **Nothing in this profile has a finalizer.** A search of the profile sources for a destructor
  finds none. [JSD-0011](0011-the-value-frame-and-call-abi.md) Row 2 refuses one by name: "No
  manual rooting, no handle table, no finalizer, no `GCHandle`." The only finalization that
  exists belongs to the BCL: `System.WeakReference<T>` declares its own `Finalize` (checked by
  reflection on .NET 10.0.12), which frees a weak handle and runs no guest code.
- **An instance has no disposal hook.** `IVmInstanceState` (`VmProfileContracts.cs`) is an empty
  marker interface. A realm "ends" by becoming unreachable, and the only teardown the profile runs is
  `JsExecution.Unwind`, which calls `JsEngine.DropPendingJobs` and deliberately runs no guest code.

## 3. What was considered

| Option | What it does | Why it was not taken |
|---|---|---|
| **Run the callback from a CLR finalizer** (a finalizable sentinel per target, or a `~` on the record) | Cleanup arrives when the collector notices | The collector's thread is outside every invocation: there is no meter to charge, no depth to count, no cancellation to poll and nobody to report a throw to. It also puts a finalizer into the profile, which [JSD-0011](0011-the-value-frame-and-call-abi.md) Row 2 refuses. **Excluded outright, not merely not chosen** |
| **Check liveness whenever a job runs, or inside any invocation** | Cleanup arrives as soon as a job boundary sees a dead target | The collector's timing becomes observable in the middle of a script, not only at a boundary the host chose, and every microtask pays for a sweep |
| **Force a collection at the drain point** (`GC.Collect` before the sweep) | Cleanup becomes prompt and nearly repeatable | A collection is process-wide: one guest's drain would stall every other runtime and the embedder with it. It is still not deterministic, because reachability also depends on JIT liveness and on embedder references (section 5) |
| **Keep the registry inert, and prune dead records silently** | Fixes the leak in section 1 without running anything | It makes `unregister`'s answer depend on the collector at any moment. The leak is a smaller defect than that nondeterminism (section 4.4) |
| **Keep the registry inert and defer** | No change | A reasonable answer, and it stays the default. It leaves no path for a program that relies on cleanup, and it leaves no written answer to the question the type's own remarks raise |
| **An opt-in sweep at the host's drain point, whose cleanup runs as an ordinary job** | Liveness is sampled only where the host asked the queue to run, and the callback runs on the guest stack under the allowance | **Proposed** |

## 4. The proposed model

**The rule: the collector's state is read only at a drain entry point the host invoked from outside
guest code (`#drain-jobs` or `#step-jobs`, section 4.2), and a cleanup callback runs only as an
ordinary job on the guest stack.** Everything else in this section follows from that rule.

### 4.1 Opt-in, and the default

The model is off unless a composition turns it on. Where the switch lives (a descriptor door beside
`DescriptorHostingRealms` or an engine option) is left to the implementation slice, but it must be a
composition's decision, fixed before the realm is built, and not a guest's. With the switch off, every
row in section 1 stays as it is, the falsifier stays true, and a registry used only for bookkeeping
behaves exactly as it does today.

### 4.2 Where the sweep happens

**Exactly two points sweep. Both are profile entry points that a host invokes from outside any
guest code, so no running script, turn or job can reach either one:**

- `#drain-jobs` (`JavaScriptProfile.DrainEntryPoint`, handled by `JsExecution.DrainJobs`, which
  calls `JsEngine.DrainJobs` through `RunOnGuestStack(instance, unit: null)`): **one sweep at
  entry**, before the first job;
- `#step-jobs` (`JavaScriptProfile.StepEntryPoint`, `JsExecution.StepJobs`, fresh and resumed,
  through `RunOneJobOnGuestStack`): **one sweep before each turn's job**.

Each sweep runs on the guest thread, inside the host step window that those two methods open
(`JsEngine.BeginHostStep`) and before any guest code in that step. It therefore runs with a live
meter and with no guest frame beneath it. **Script entry points, the embedder turn
(`TurnEntryPoint`), `JsHostRealm.EnqueueJob`, `JsHostRealm.DrainJobs` and the jobs themselves never
sweep.** A drain that empties its queue does not sweep again. If a turn starts with an empty queue
and the sweep finds nothing, the step completes as it does now. If the sweep enqueues a cleanup,
that turn runs it.

**Why `JsHostRealm.DrainJobs(limit)` does not sweep.** It is a drain that an embedder delegates to,
and it can only be reached from inside a step. It begins with `Enter(2)`, and `Enter` refuses unless
`stepDepth > 0` on the guest thread (`JsHostRealm.cs`). It does not go through `RunOnGuestStack` or
`BeginHostStep`. An embedder may expose it to guest code, and the CLI's own host-surface check does
exactly that: in `HostSurfaceChecks.cs`, `broilerHost.drain` calls `r.DrainJobs()`, and a check
script calls `print('ran=' + broilerHost.drain())` partway through a script. A sweep there would let
a guest sample the collector in the middle of its own script, or of a job, which the rule at the top
of this section forbids. So this drain runs queued jobs and never queues a cleanup job itself. The
jobs it runs may include a cleanup job that an earlier sweep queued, for example one still waiting
between two `#step-jobs` turns. That callback then runs nested inside the script or job that called
the drain, but what it reports was fixed at the earlier host sweep. An embedder that exposes the
drain has delegated *when queued jobs run*, not *when the collector is read*.

### 4.3 When a target counts as collected

**A target is eligible when its record's weak reference no longer resolves at the moment the sweep
reads it. The sweep snapshots that answer; it is not re-read later.** The engine keeps a list of
its registries in creation order. Each entry is held weakly, so a registry that became unreachable
drops out and none of its callbacks ever run, which the specification allows. For each live
registry, the sweep visits records in registration order. It marks every record whose target has
gone as **collected**, drops the dead `WeakReference`, and keeps the held value. A record that is
marked stays marked. A record that is not marked is not looked at again until the next sweep.

This is the answer the type's remarks said was missing. It does **not** make collection
deterministic, and section 5 lists what still depends on the implementation.

### 4.4 Held values, unregister and the leak

- **At most one cleanup job per registry is queued at a time.** When a sweep marks at least one
  record and the registry has no cleanup job outstanding, it appends one job through
  `JsEngine.EnqueueJob`, which charges it like any other job. That job is
  `CleanupFinalizationRegistry`: it takes the **marked** records in registration order, removes each
  one *before* calling the callback with its held value, and ends when none is left. This follows
  the specification, where the job removes the cell and then calls the callback.
- **`unregister` removes marked and unmarked records alike and answers `true` for either**, so a
  token removed between the sweep and the job prevents that call. Between two sweeps, `unregister`
  still gives the same answer on every run of the program. It can change only across a sweep, and
  a sweep happens only at a drain the host requested.
- **Held values are released when their record is removed** (callback called, token unregistered,
  job dropped, or registry unreachable) and at no other time. The target itself is never handed to
  guest code.
- **The leak in section 1 is fixed only as a side effect of the sweep.** In inert mode, silently
  pruning dead records would make `unregister` answer depending on the collector at any moment,
  and that is worse than the retention. In inert mode, the retention is bounded by the registry's
  lifetime and charged to `LiveBytes` at registration.

### 4.5 Exceptions, cancellation, exhaustion and unwind

- **A throwing callback is a throwing job.** `#drain-jobs` folds it into the first fault and keeps
  running the queue. `#step-jobs` reports it in that turn's result: in the `JsPause` when jobs are
  still queued, or in the `JsCompletion` when the queue is now empty (`JsExecution.StepJobs`).
  `JsHostRealm.DrainJobs`, if it runs a queued cleanup job that throws, stops and leaves the rest of
  the queue. The cleanup job itself stops at the throw. **The records it had not
  reached stay marked and are run by a fresh cleanup job that the next sweep queues**, so one throw
  does not lose the other held values and does not start an unbounded retry loop within one drain.
- **The sweep is metered.** It charges one unit of fuel per registry and one per record inspected,
  the same scale as `unregister`'s `Count + 1`. Charging polls cancellation and the budget, so a
  sweep over a very large registry ends as `Cancelled` or `AllowanceExhausted`, like any other
  work. The records it already marked stay marked. A partial sweep loses nothing and repeats
  nothing.
- **Unwind runs no cleanup.** `JsEngine.DropPendingJobs` drops a queued cleanup job along with
  every other job. The records that job would have processed stay marked in their registry, and the
  next sweep queues a job for them if the instance is ever drained again.

### 4.6 Realm disposal

**Nothing runs when a realm ends, and nothing can run.** The profile has no disposal hook (section 2).
Registries, records, held values and queued jobs are ordinary managed objects that become
unreachable with the instance. Any guest code must run inside a drain, and nobody will request one
again. **No finalizer is added by this model**, and the implementation slice has to prove that with a
test (section 7).

### 4.7 Order

Registries are handled in creation order and records in registration order. Cleanup jobs go on the
same FIFO queue as promise reactions and host jobs (the reason is in `JsHostRealm.EnqueueJob`'s
remarks). A cleanup therefore runs after the jobs queued before its drain started, and before the
jobs its own callback queues. The specification does not fix this order, and Node visibly uses
another one (section 1). Fixing an order here means two runs that saw the same collections give the
same transcript.

### 4.8 What a guest cannot do

A guest cannot force a sweep, cannot tell whether one ran except by watching a callback arrive or an
`unregister` answer change across a drain, and cannot make the collector run. If an embedder exposes
its delegated drain (`JsHostRealm.DrainJobs`, section 4.2), a guest can choose *when an
already-queued cleanup job runs*, and nothing more. That drain samples nothing, so calling it tells
a guest nothing about the collector beyond what the last host sweep already fixed. `gc` is not a global,
and `$262.gc` is present but refuses with a `TypeError` ("this host exposes no collection hook",
`JsRealm.Global.cs`; probed). The model keeps it that way.

### 4.9 `cleanupSome`

`cleanupSome` is the one member that would let a guest ask for cleanup synchronously from its own
stack. Under this model it would have to be one of two things: a sweep outside a host drain point,
which breaks the rule at the top of section 4, or a no-op that the sweep has made misleading. **This
record recommends that it be removed**, which answers JSP-7's exit-gate item "removed or recorded as
a deliberate extension". The member is a stage-2 proposal that Node does not expose. That removal
belongs to JSP-7 and is listed here as an owner decision, not taken.

## 5. What remains implementation-dependent

- **When a target stops resolving.** The CLR decides. JIT liveness, tiered compilation, a
  `JsHostRef` an embedder still holds (its `Target` is a strong `object` field in `JsHostValue.cs`),
  a baseline native page, or any cache can keep an object reachable after the guest dropped it. The
  specification permits every one of these, and the model does not try to predict them.
- **Whether a sweep finds anything.** It depends on whether a collection ran before the drain.
  The host can call `GC.Collect` itself before draining if it accepts the process-wide cost. The
  profile never does this.
- **A dereferenced `WeakRef` pins its target** (section 2), so that target is never reported. This
  follows from an existing decision, and the model does not change it.

## 6. Deterministic test seams

**GC timing is not a test oracle. A test that expects a callback to arrive after `GC.Collect` is a
flaky test and is refused.** The model is tested through a seam and one safety property:

- **An internal eligibility seam.** The sweep asks an internal `IJsFinalizationEligibility` (name
  to be settled by the slice) whether a record's target is collected. The production answer is "the
  weak reference no longer resolves". A test supplies a scripted answer that marks the targets it
  names, by identity, as collected at a chosen sweep. That target may still be alive. The callback
  only ever receives the held value, so marking a live target changes nothing the model promises.
  Only internal tests can reach the seam. It is not public surface and does not enter the API
  baseline ([JSD-0012](0012-the-profile-api-baseline-and-where-its-clause-lives.md)).
- **One safety test on the production path.** A target the test keeps strongly reachable
  (`GC.KeepAlive` after the drain) is never reported, however many collections the test forces.
  Collections cannot make that assertion fail. Only a bug can.

## 7. Follow-up slices

**D03-a - The opt-in host-drained sweep.** Implements sections 4.1 to 4.7 behind the composition
switch, with the section 6 seam. The default is unchanged.

- *Accept:* with the switch off, every row in section 1 is unchanged, and the section 1 probe gives
  the same output byte for byte. With the switch on, and using the scripted seam: (1) a marked
  record's callback runs exactly once, with its held value, in a `#drain-jobs` and in a `#step-jobs`
  turn, and not in a script invocation that runs no drain; (1a) an embedder drain invoked by a guest
  (`JsHostRealm.DrainJobs`, exposed as in `HostSurfaceChecks.cs`) performs no sweep, both when a
  script calls it and when it is called from inside a job that `#drain-jobs` is running. With the
  scripted seam marking a target just before that call, the call queues no cleanup job; it runs
  only a cleanup job that an earlier `#step-jobs` sweep had already queued; (2) `unregister` between the sweep and
  the job prevents the call and answers `true`; (3) a throwing callback faults the drain as a job
  fault does, and the next sweep delivers the records it had not reached; (4) cancellation or
  exhaustion during a sweep leaves the records it already marked still marked; (5) `Unwind` drops a
  queued cleanup job and runs no guest code; (6) an unreachable registry's callback never runs;
  (7) an Architecture test fails if any type in the `Broiler.VM.Profile.JavaScript*` assemblies
  declares a finalizer (`Finalize` override), which puts JSD-0011 Row 2 and the "never from a
  finalizer" rule under test rather than in prose; (8) the production-path safety test in section 6
  passes.
- *Owes:* the type's remarks and its falsifier line in `JsCollections.cs` and `HUMAN_REVIEW.md`
  are rewritten to "a cleanup callback runs outside a host-requested drain, or any guest code runs
  from a CLR finalizer". Because this is a `Security=High` line, it goes back to human review. The
  Contract and Architecture suites stay green.
- *Excludes:* `GC.Collect` anywhere in the profile, any cleanup from a finalizer, a sweep in any
  place other than the two points in section 4.2 (in particular, none in `JsHostRealm.DrainJobs`), and switching the model on in the CLI host or
  the conformance composition. That last item needs its own decision, because a test262
  `host-gc-required` case would then depend on collector timing.

**D03-b - `cleanupSome`.** JSP-7's item. Remove the member (recommended) or record it as a deliberate
extension. *Accept:* `typeof new FinalizationRegistry(() => {}).cleanupSome` answers the same here as
in the comparison engine, or a record names it as an extension. It is independent of D03-a and can
be done before it.

**D03-c - Symbols as weak targets and tokens.** The parity roadmap already records that symbols are
rejected as weak keys and `WeakRef` targets. The same rejection applies to `register` and
`unregister`, and the probe in section 1 shows it. *Accept:* a non-registered symbol is accepted as a
target and a token, and `Symbol.for(...)` is still a `TypeError`, matching Node. This depends on a
weak symbol key existing and does not depend on D03-a.

## 8. What is deferred

- Turning the model on in any shipped composition (see D03-a's *Excludes*).
- Sweep cost proportional to live registrations rather than to all registrations. The model charges
  for every record it inspects, which is honest but O(n) per drain. A generation scheme is an
  optimisation for a later slice with a measured workload.
- Reconsidering the permanent `deref` promotion in section 2. It belongs to `WeakRef`'s own
  decision, not to this one.

## 9. What would falsify this

- **Guest code runs from a CLR finalizer, or on any thread other than the guest stack.** Nothing in
  the model allows either, and D03-a's test (7) makes the first one mechanical.
- **The collector's state is read anywhere except at a `#drain-jobs` or `#step-jobs` sweep, or a
  cleanup callback runs other than as a job such a sweep queued.** That would mean the collector's
  state is visible at a moment the guest chose. A queued cleanup job that an embedder-delegated drain
  runs partway through a script does not falsify this (section 4.2), because it reports what the
  earlier host sweep saw. A sweep inside `JsHostRealm.DrainJobs` would falsify it.
- **A test uses collector timing as its oracle.** That would make section 6 the fiction the D03
  card warns against.

## 10. Owner decisions pending

- **Whether to take this model at all, or keep deferring.** If the owner defers, sections 1 and 2
  still stand as the record of current behaviour, and D03-a is not scheduled. What the decision
  needs: whether any embedding the owner cares about depends on cleanup arriving. No workload in
  `src/tests` does today.
- **Removing `cleanupSome`** (D03-b), which JSP-7 owns. Recommended: remove it.
- **Where the opt-in switch lives** (section 4.1): a descriptor door beside
  `DescriptorHostingRealms`, or an engine option. The record requires only that it be the
  composition's choice, fixed before the realm is built. D03-a proposes the concrete place, and the
  owner confirms it.
- **Human re-review of the `Security=High` falsifier line** for `JsFinalizationRegistryObject`
  (`JsCollections.cs`, `HUMAN_REVIEW.md` fingerprint `66E399`). D03-a rewrites that line (see its
  *Owes*), and D03-a does not count as done until the owner has reviewed the new line.
