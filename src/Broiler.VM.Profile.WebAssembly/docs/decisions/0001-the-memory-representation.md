<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# WAD-0001 - A linear memory is a pinned managed array, reallocated on growth with its base republished

**Status:** Taken, 2026-09-25, by the WebAssembly profile owner, before any line of the store this
record governs was written under the universal bytecode. Nobody has signed the record, so it claims no
approval. Approvals are deferred under the MVP terms.

**Owner:** the WebAssembly profile owner. **Co-signer:** the security owner, because a region an
emitted form addresses is a memory-safety boundary. **Both roles are held by one person**, and this
record does not claim the co-signature is independent.

**Milestone:** none in the `WA-` series. This is decision UBC-D-2 of
[the universal bytecode programme roadmap](../../../../docs/universal-bytecode.roadmap.md), taken at its
milestone UBC-4, work package UBC-4.4, and it is the memory-representation row this profile's
[section 13](../roadmap.md) reserves for WA-5. It moves no row of [this profile's ledger](../roadmap.status.md).

## What was open

The universal bytecode executes this profile's loads and stores as region primitives over a region the
family declares by name, `memory0`. The concept's Appendix D asks of such a region "a base that does not
move for the instance's life and a length the emitter reads at the access", and its section 8.2 names
the two ways a region can have one: "a pinned managed array, or native memory owned by a `SafeHandle`,
each with its growth story - a successful `memory.grow` invalidates every view, which that profile's
section 13 already rules". The programme's decision table offers the same two:

- (a) a pinned managed array reallocated on growth with the base republished;
- (b) native memory owned by a `SafeHandle`;

either with growth invalidating every view. Before this record the memory was an ordinary managed array,
reallocated and copied on growth (`WasmMemoryInstance`), and the choice was recorded only in that
type's remarks.

## Decision

1. **A linear memory is a pinned managed byte array.** It is allocated on the pinned object heap, so the
   collector never moves it while it lives, and it is owned by the instance's store and reclaimed by the
   collector when nothing reaches it.
2. **A successful `memory.grow` allocates a new pinned array, copies the old contents, and republishes
   the base**: the store's reference to the memory changes, and every reader of the memory reads it
   through the store at the access. The old array is dropped.
3. **A growth invalidates every view**, as section 13 already rules: a span, a reference or an address
   taken over the memory before a successful growth is not a view of the memory after it. Nothing in
   this profile holds such a view across an instruction; a form that addresses the memory directly must
   reload the base after every instruction that can grow it.
4. **The base is stable between growths, not for the instance's whole life.** Appendix D's wording - "a
   base that does not move for the instance's life" - is read as "a base that does not move while no
   growth succeeds", and the concept's Appendix G records the reading as a departure from the text, with
   its consequence for a native form: the base is reloaded after `memory.grow` and after any call, since
   a callee can grow.
5. **What the route keeps, unchanged:**
   - route MVP-1's guest-observable refusal: a growth past this profile's own declared page ceiling
     (`WasmMemoryInstance.ProfileMaximumPages`) answers minus one before any charge, and a refusal by a
     core budget stays the published deviation;
   - the charges: fuel per page grown and `AllocatedBytes` for the bytes a growth adds, both before the
     allocation;
   - the `LiveBytes` retention report for every byte the store holds, and its release when an
     instantiation that allocated it fails;
   - the specification's four-gibibyte address space is not reached: the profile's own page ceiling is
     far below what a single managed array can hold, and that ceiling, not the array, is the limit a
     guest meets.

## What it rejects, and why

- **Native memory owned by a `SafeHandle`** (option b). It is never moved by the collector and can
  exceed what one managed array holds, and it suits a frame that holds no managed reference. It is
  rejected for this milestone because it brings unsafe code into a profile that has none, turns a missed
  bounds check into native-heap corruption rather than a managed exception, and has no deterministic
  release: the core has no instance-disposal hook, so a live instance's memory would be returned only by
  a finalizer, which reports nothing to any meter.
- **Keeping an unpinned managed array and deciding when a native form first addresses the memory.** The
  concept's own phrase is that a native form "makes urgent" this row, and none exists yet. It is
  rejected because the programme's risk row asks for the decision before the store is written, and a
  store written unpinned would be rewritten when the native form arrives.
- **Allocating the declared maximum up front**, so that the base never moves at all. It would charge
  every instance the whole ceiling at instantiation and change `AllocatedBytes` and `LiveBytes` answers
  the base run records; a reserved virtual range with guard pages instead would bring memory-mapping
  entry points into a profile whose register row declares native execution `none`.

## What would settle it differently

A native form for this profile (the programme's UBC-6b) that measures the cost of reloading the base
after every call and growth, and finds it unacceptable, reopens decision 4; a guest need for memory
beyond what one managed array holds reopens decision 1. Either is this profile owner's to take, with the
security owner, in a later record of this series.

## Added 2026-09-26: its limit on each runtime identifier, and section 17's boundary

WA-5's exit gate asks the memory-representation decision to name its own limits per runtime identifier
and not to foreclose [section 17](../roadmap.md)'s boundary, and the record above did neither in words.

- **Per runtime identifier.** A pinned managed array is the same representation on every runtime
  identifier the runtime supports, with no platform call, no reserved range and no page protection, so
  its limit does not vary by identifier: a memory is bounded by this profile's own page ceiling
  (`WasmMemoryInstance.ProfileMaximumPages`) on every one, and that ceiling sits below what a single
  managed array can hold on any of them. No runtime identifier is claimed by this repository, and this
  record claims none.
- **Section 17.** The JavaScript API's `WebAssembly.Memory` exposes a buffer a script holds across
  calls. This representation does not foreclose it: a host view of the memory is a view of the array the
  store holds at the moment it is taken, and a successful growth invalidates it, which is what section 13
  already rules and what that specification requires of a detached buffer. Nothing here decides that
  boundary; it leaves it where section 17 put it.
- **Decision 5's third bullet.** The store that was built charges its `LiveBytes` retention before it
  allocates, with a charge that can be refused, rather than reporting it after the allocation;
  [WAD-0003](0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)'s correction of this
  date says what that answers. The release on a failed instantiation is unchanged. *(Corrected later
  the same day, and the sentence before is kept as written: the release is made on every failure the
  bytecode emitter answers, as the base executor made it on every failure it answered; an
  instantiation the emitter answered as complete and the core then dropped, because a refusal or a
  cancellation latched during it, is released by nothing - a defect of the core, recorded in
  WAD-0003's second correction and the plan's [WAC-43](../roadmap.corrections.md#wac-43).)*
