<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# WAD-0004 - Decoding and validation are fused within one function body, and ordered at module granularity

**Status:** Proposed, 2026-09-28. It is not yet a decision. It records what the code does, the other route,
and what each route costs, so that the WebAssembly profile owner can take or reject it. Nobody has signed
it, and approvals are deferred under the MVP terms.

**Owner:** the WebAssembly profile owner. **Co-signer:** none; the decision is this profile's alone.

**Milestone:** WA-3. Its exit gate asks that the milestone "record the decision on whether decode and
validate are fused within one function body, with the module-granularity phase-order property stated
either way". Roadmap [section 8](../roadmap.md#8-validation) allows the fusion "as an implementation
choice, provided the phase-order property above survives at module granularity", and says WA-3 states
which route it took. This record is that statement, in draft. It moves no row of
[this profile's ledger](../roadmap.status.md).

## What was open

The specification defines decoding and validation as two phases, and a module that is malformed and
invalid is malformed: the scripts assert it. Roadmap section 8 records that the single-pass validation
algorithm "is designed to run inside a decoder", and permits fusing the two inside one function body
if the ordering still holds between sections and between bodies. The two readings differ in what a
module reports when one function body is both malformed and invalid.

- **(a) Fused within a body.** The decoder frames every section and reads every grammar except a
  function body's instructions. Decoding completes, and then the validator reads each body once,
  decoding each instruction and its immediates and typing it in the same step.
- **(b) Separate everywhere.** The decoder also reads every body's instructions, either in a pass of
  its own or into an intermediate form, and the validator types what the decoder read.

## What the code does

Route (a):

- **Decoding.** `WasmDecoder` never reads a byte of a function body. It copies each body out, holding
  it to its declared length. Since 2026-09-28 it reads constant expressions as the format's instruction
  sequence, because a constant expression lives outside any body and something must find its end.
- **Validation.** `WasmValidator` begins after `WasmDecoder` returns a module, and walks each body once.
- **Module granularity.** A malformed section, a malformed type, name, limit, segment or constant
  expression, or a body whose framing disagrees with its declared length, is refused before any body is
  validated.
- **Within a body.** The first defect the walk meets is the answer. A malformation met first, such as a
  byte naming no instruction, an over-long integer or a reserved byte that is not zero, is refused with
  a malformation reason. An invalid instruction met first is refused as invalid, even when a
  malformation follows it in the same body.

**The code band does not say which phase refused, and the reason does.** A malformation the validator
meets carries a code of the validation band (`UnknownOpcode`, `ReservedImmediateNotZero`) with a
malformation reason (`MalformedEncoding`). The diagnostics enumeration and the corpus README both state
that the band tells the phase, which is true of the pass and not of the category. The harness root's
`--spec` lane scores by reason for that reason, as its `ScriptJudgement` states.

## What each route costs

- **(a)** One walk per body and no intermediate form, which is what section 8 prefers. It answers one
  shape of module differently from the specification: a body invalid before it is malformed.
  `binary.wast` in the pinned scripts carries one. A `br_table` declares one label more than it gives,
  so its labels read on into the body's closing bytes. The walk meets a branch depth no block provides
  before it meets the body's end, and the lane answers the module invalid where the script asserts
  malformed (record
  [WA-SPEC-001](../../../../docs/evidence/wa-spec-001/README.md), section 3). A reader of a corpus row's
  code cannot tell the category from the band alone.
- **(b)** Every body is read twice, or held twice. Both charge the verifier's work and allocation
  bounds again for what the validator will read anyway. It would answer the `binary.wast` shape as the
  script asserts, and it would make the band tell the category.

## Proposed decision

1. **Route (a) stands: decoding and validation are fused within one function body, and decoding
   completes before validation begins at module granularity.**
2. **The consequence is named, not hidden.** A body invalid before it is malformed is answered invalid.
   The `binary.wast` assertion that shows it stays failing in the lane, and this record is cited where
   it is counted.
3. **The category travels on the reason, and the band tells the pass.** The corpus README and the
   diagnostics enumeration each gain a sentence saying so when this record is taken.

## What would change it

A consumer that must hold the specification's answer for every module, rather than for every module
but this shape, would need (b). So would a milestone that makes the code band carry the category.
Either reopens this record, and WA-3's owner decides it.
