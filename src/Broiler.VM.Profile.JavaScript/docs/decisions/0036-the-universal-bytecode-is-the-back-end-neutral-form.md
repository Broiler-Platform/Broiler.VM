<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0036 - The universal bytecode is the back-end-neutral intermediate form section 9 promises

**Status:** Taken, 2026-09-25, following
[ADR 0013](../../../../docs/adr/0013-the-universal-bytecode-extraction-record.md), the extraction record
of the universal bytecode, whose verdict on its candidate A is an acceptance. The owner gave that verdict
in advance of the record's correspondence table, on the condition that the table held, and this decision
follows from it rather than being weighed on its own. Nobody has signed the record, so it claims no
approval. Approvals are deferred under the MVP terms.

**Owner:** JavaScript profile owner. **Co-signer:** the core architecture owner, because the form this
record names is the core's shared assembly and not this profile's. **Both roles are held by one
person**, and this record does not claim the co-signature is independent.

**Milestone:** none in the `JS-` series, and that is stated rather than filled in. This record is work
package UBC-0.4 of [the universal bytecode programme roadmap](../../../../docs/universal-bytecode.roadmap.md),
and it is the dated decision the open clause of stage
[JSB-3](../roadmap.backends.md#jsb-3--the-backend-abstraction-and-where-the-form-is-chosen) of
[the backend roadmap](../roadmap.backends.md) asked for. It moves no row of
[this profile's ledger](../roadmap.status.md).

## What was open

[Section 9](../roadmap.md#9-the-semantic-front-end-and-lowering) of this profile's plan says, under
"What the front end is not", that the front-end contract "returns a validated tree or a
back-end-neutral intermediate form, and the lowering consumes that". No such intermediate form exists:
the syntax tree is the only tree, and the format-version-2 bytecode is the only intermediate form.

Stage JSB-3's exit gate asks that this sentence be "either corrected or discharged by a dated decision
recording that the bytecode **is** that form, with the corrections file carrying whichever it was", and
JSB-3's State bullet of 2026-09-07 names the clause as open. The backend roadmap's section 4 took the
route "that the bytecode is the back-end-neutral form" without a decision, and says in its own words
that no decision record has chosen between it and building the intermediate form section 9 promises.

On 2026-09-25 [the universal bytecode concept](../../../../docs/universal-bytecode.md) proposed that
every language profile lower only to one shared bytecode and that every output form be produced from it
by an emitter profile, and ADR 0013 admitted that bytecode through the core's extraction gate.

## Decision

1. **The back-end-neutral intermediate form section 9 promises is the universal bytecode.** The front
   end returns a validated tree; the lowering consumes it and emits universal bytecode; and every output
   form this profile has - the bytecode form, the numeric form and the baseline form - is produced from
   that one form by an emitter profile rather than by an exit of the lowering. That is the backend
   roadmap's section 4 route taken one step further: not "this profile's bytecode is the form" but "the
   shared bytecode is the form", which is the reading under which a back end is neutral as to language
   as well as to target.
2. **The form is named, and it is not yet in the tree.** Until the lowering emits universal bytecode -
   the programme's milestone UBC-3, whose work package UBC-3.4 gives the lowering that one exit - the
   form section 9 names does not exist in this checkout, and format version 2 remains the finished
   bytecode every native form of this profile attaches at, under
   [JSD-0025](0025-the-baseline-native-form-over-the-wide-manifest.md). This record discharges the
   sentence by saying what it points at; it does not claim the thing pointed at exists.
3. **Section 9's sentence is not rewritten.** It was the right promise and it now has a named subject.
   The corrections file records the changed reading as [JSC-227](../roadmap.corrections.md#jsc-227), and
   section 9 carries the bare pointer to it, which is this plan's discipline for a changed reading.
   **JSB-3's clause is read, not simply met.** Its words ask for "a dated decision recording that the
   bytecode **is** that form", written when the only bytecode was this profile's own. This record names
   a bytecode that is not yet in the tree, as the universal bytecode programme's work package UBC-0.4
   plans, and reads the clause as discharged by that naming; a reader who takes the clause to mean this
   profile's bytecode will find it still open until the lowering emits the universal bytecode at
   UBC-3, and both readings are stated here so that neither is mistaken for the other.
4. **JSB-3's other open clause is untouched.** The check that a backend refusing every unit leaves the
   artifacts byte-identical is still not written, and nothing here says otherwise.

## What this rejects

| Option | Why it is not taken |
|---|---|
| **Discharging the sentence onto format version 2**, the reading the backend roadmap's section 4 took | It was the route in force, and it would close JSB-3's clause today. But the programme retires format versions 1 and 2 at its work package UBC-3.10, subject to decision UBC-D-1, so the decision would name as the neutral form a format the same day's records schedule for replacement - a discharge onto something known to be leaving. |
| **Building a typed intermediate form with a control-flow graph** between the syntax tree and the bytecode | The concept's section 10 row: re-emission equality and the template-closure scan rest on a closed template table over a linear form, and an optimiser's output is closed under no such table. This profile's verification of a native payload would have nothing to check the new form against. |
| **Correcting the sentence away** | The promise is sound and has a subject now; deleting it would erase the one sentence in the plan that says the front end does not know which form an artifact will take, which is exactly what the programme makes true. |
| **Leaving the clause open** | JSB-3 would carry an unmet clause no stage owns, while a programme changes what the lowering emits. A form named after the lowering moved would be a record written to fit the code. |

## What this does not decide

- **Nothing about the value form or the retired formats.** Whether format versions 1 and 2 and the value
  form's substrate leave the tree is decision UBC-D-1, the JavaScript profile owner's to take at the
  programme's milestone UBC-3.
- **Nothing about any native form's granularity, rooting or speed.** JSD-0025 and route MVP-8 stand.
- **No milestone, no ledger row and no support claim.** It moves no `JS-` row and no row of the core's or
  the programme's ledgers.
- **Nothing about the native half of the concept.** ADR 0013 notes that G1 is unsatisfied for the
  native-form mechanism, so the gate cannot yet be invoked for it; how this profile's native forms are
  recovered on the universal bytecode is the programme's to settle once that changes.

**This record is amended at the programme's work package UBC-3.11 with what landed.**

## Falsified if

- At milestone UBC-3 the lowering acquires any exit other than universal bytecode, or a back end attaches
  to the syntax tree rather than to the bytecode.
- An output form of this profile is produced from anything but the universal bytecode after UBC-3.
