<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# WAD-0002 - The family's instruction table stays under the slice identity, and numeric1 is not minted

**Status:** Taken, 2026-09-25, by the WebAssembly profile owner. It records an absence: the manifest the
programme's decision UBC-D-3 offered to mint is not minted. Nobody has signed the record, so it claims no
approval. Approvals are deferred under the MVP terms.

**Owner:** the WebAssembly profile owner. **Co-signer:** none; the decision is this profile's alone.

**Milestone:** none in the `WA-` series. This is decision UBC-D-3 of
[the universal bytecode programme roadmap](../../../../docs/universal-bytecode.roadmap.md), taken at its
milestone UBC-4, work package UBC-4.3. Minting `broiler.webassembly.numeric1` stays WA-5's act, as this
profile's [section 6](../roadmap.md) allocates it. It moves no row of [this profile's ledger](../roadmap.status.md).

## What was open

Under the universal bytecode a family's instruction tables are selected by the manifest an artifact
names, so a manifest does real work: it decides which rows a verification admits. This profile has one
minted identity, `broiler.webassembly.slice`, and its plan defines that identity as an integer-only
surface; the code has admitted the whole numeric surface, memory, a table, globals, the start function
and segments under it since WA-3, and [the ledger](../roadmap.status.md) records that as a defect the
milestone that mints a second manifest owns closing. The programme offered two routes:

- (a) mint `broiler.webassembly.numeric1` at UBC-4 with its own retained run, so that each table is
  selected by the surface it admits;
- (b) keep the slice identity and record that the table admits more than the identity says.

## Decision

1. **The WebAssembly family has one instruction table, selected by `broiler.webassembly.slice`.** It
   holds a row, or a common mapping, for every instruction the code admitted under that identity before
   UBC-4, and no other.
2. **`broiler.webassembly.numeric1` is not minted.** Its allocation in section 6 stands as written, and
   minting it stays WA-5's act.
3. **The defect is kept and stated, not closed.** The table admits more than the slice identity's
   definition in section 6 - floats, memory, a table, globals, the start function and segments - exactly
   as the code did before, and invariant 10 of the plan ("unsupported surface is truthful") stays
   breached in the way the ledger already records. A native form produced from this table (the
   programme's UBC-6b) would be emitted under the same identity and carry the same statement.

## What it rejects, and why

**Minting `numeric1` now** (route a). It would make the concept's selection by manifest do the work it
was designed for and close the ledger's defect. It is rejected at this milestone because section 6's
`numeric1` row excludes the start function and element and data segments, which the code accepts today
and the harness exercises; minting the identity as written would refuse modules the base run of
[bundle ubc-4-001](../../../../docs/evidence/ubc-4-001/README.md) accepts, and the predeclared rule
counts every such difference as a regression. Doing it without that cost needs a re-scoping of section
6's allocation, which the plan's own rule forbids widening silently, and a population the suite pin
this profile does not yet have.

## What would settle it differently

WA-5, with a suite pin and a script reader in the harness (WA-4), mints `numeric1` as section 6 allocates
it, gives the slice identity its own narrow table, and closes the defect; the programme's native forms
then emit under the identity that describes them.
