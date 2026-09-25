<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# WAD-0003 - The nine value, store and frame routes under the universal bytecode

**Status:** Taken, 2026-09-25, by the WebAssembly profile owner, in the same act as the programme's
milestone UBC-4 retires `WasmInterpreter` and `WasmValue`. Nobody has signed the record, so it claims no
approval. Approvals are deferred under the MVP terms.

**Owner:** the WebAssembly profile owner. **Co-signer:** the core architecture owner, because four of the
rows are now answered by the core's shared assembly rather than by this profile. **Both roles are held by
one person**, and this record does not claim the co-signature is independent.

**Milestone:** none in the `WA-` series; the programme's milestone UBC-4. It moves no row of
[this profile's ledger](../roadmap.status.md).

## What was open

This profile's [section 9](../roadmap.md) asks for a nine-row value, store and frame decision before any
interpreter source is written. The rows were answered as routes and written down in the remarks of
`WasmValue`, which said of them that none was a review decision and each might be reversed. The
programme's milestone UBC-4 deletes `WasmValue` and `WasmInterpreter`: this profile's modules are
translated to the universal bytecode and executed by the bytecode emitter. The routes need a home that
does not disappear with the file, and each needs a statement of where it now stands.

## Decision

Each row, as `WasmValue`'s remarks stated it, and where it stands:

| Row | The route as it was | Where it stands under the universal bytecode |
|---|---|---|
| 1, numeric representation | A single untyped 64-bit payload in a sixteen-byte slot; validation proved every type, so the slot carries no tag | **Carries over, narrowed**: a value is an untyped 64-bit word of the universal bytecode's word plane; the walk proves every slot's type, so no word carries a tag |
| 2, vector representation | A sixteen-byte slot reserving the vector width (route MVP-2) | **Reversed**: the word is eight bytes and no slot reserves a vector's width. A `v128` slot is a question for a universal bytecode format version 2, and route MVP-2's reservation is annotated as such in `docs/mvp.md` rather than answered |
| 3, reference representation | None, because no reference type is admitted; a table holds function indices as integers | **Carries over**: the family declares no value plane, and a table's entries are unit indices held in the store |
| 4, rooting and lifetime | Nothing needs rooting; the store's arrays are reachable from the instance and no host holds a view | **Carries over, with [WAD-0001](0001-the-memory-representation.md)**: the memory is a pinned array reachable from the instance, and a growth invalidates every view |
| 5, call convention | Arguments popped into the callee's own locals array; results pushed back | **Now the universal bytecode's**: a call copies its parameters into the callee's first locals, as Appendix A's `call` row says, and results replace the arguments |
| 6, frames and labels | Heap frames and per-frame label arrays; guest depth never grows the CLR stack | **Now the universal bytecode's**: heap frames owned by the operation, one CLR frame whatever the guest's depth; labels are lowered away by the translator, which leaves only jumps, jump tables and `squash` |
| 7, trap propagation | A return code through the dispatch loop, never a CLR exception | **Now the universal bytecode's**: a trap is a status answered by a primitive or a handler and turned into the family's payload at the loop's boundary |
| 8, metering | A fuel unit per instruction, proportional charges beside it, CallDepth per activation, allocation and retention for the store, a poll at the declared bound | **Now the universal bytecode's, with this profile's store charges kept**: fuel per row and per frame's declared locals, charged before effects, polls at the declared bound; the store's charges and retention reports are this family's, unchanged. Structural instructions are no longer charged, because the translator emits no row for them |
| 9, what a `LiveBytes` breach does | Nothing this profile decides; observed at the next charge or poll | **Carries over, unchanged**: retention is still reported and the report still returns nothing |

## What it rejects, and why

**Keeping the sixteen-byte slot** (row 2) as a word-plane convention of this family alone. It is rejected
because the word plane is the universal bytecode's, shared by every family, and a width one family
reserves would be a width every family pays for; the question belongs to the format's next version.

## What would settle it differently

A universal bytecode format version 2 that admits a `v128` slot type reopens row 2; the admission of a
reference type into this profile's manifest reopens rows 3 and 4. Either is taken in a later record of
this series.
