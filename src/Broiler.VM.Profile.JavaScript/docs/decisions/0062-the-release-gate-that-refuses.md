<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0062 - The release gate that refuses

**Status:** Proposed, 2026-10-05. **Owner decision pending.** It records phase F9's slice R4 under
[JSD-0059](0059-the-release-under-the-mvp-programme.md): the JavaScript profile's release gate, which
reads roadmap [section 22](../roadmap.gates.md#22-release-gates)'s gates 1 to 13 from the checkout and
**refuses**, naming every blocker by the declaration that states it; its register,
[`docs/release-gate.md`](../release-gate.md); and rule **N38**, which holds the two to each other.

**What the owner decided.** On 2026-10-05 the repository owner asked for phase F9 to be done. Nothing
here is signed beyond that request.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are held
by one person**, and it does not claim the co-signature is independent.

**Milestone:** JS-10. Phase F9, slice R4.

---

## 1. What JS-10 asks, and what this builds

JS-10's next action ends: *run the release gate that refuses the tree while any relevant unit lacks a
human decision*. Release gate 11 is the hinge - no package published, no RID claimed, no table issued,
no milestone accepted until a named human has recorded a decision on every relevant unit - and the
other twelve gates are what such a release must also satisfy.

The gate is `JsReleaseGate` in the architecture test assembly. It reads the checkout and answers a
verdict, `REFUSED` or `PASSED`, and per gate one of three things:

- **held** by named rules, each of which it checks is `Active` in the rule register;
- **blocked** by an act only the owner or a named human can take, named with the declaration that
  states it is missing - the support table's banner, the composition register's empty advertised set,
  a vacant holder row, a notices row reading *not yet confirmed*, a unit's `// Broiler-Human:` line;
- **blocked as unread**, where a clause is shown by a retained bundle and this check reads no
  mechanism for it. An unread clause blocks as surely as a missing act; a gate that passed what it
  did not read would be the false record the release discipline exists to prevent.

## 2. It cannot invent a reviewer

A human decision is counted from one place: the `// Broiler-Human:` line on each relevant unit of the
JavaScript family, where the assurance policy puts it and rules J3 and J4 bind it to the declaration's
fingerprint. **No headline, document, table or commit message is counted.** A review headline that
claims more than the units record is itself a blocker, `G11-headline-contradicts-units`; a support
table that reads as issued while units are undecided is `G1-issued-without-review`. Rule N38's two
witnesses hand the gate exactly those - `HUMAN_REVIEW.md` claiming every unit verified, and the table
claiming to be issued and reviewed by name - and the gate refuses both times, naming the
contradiction.

## 3. What it says today

The verdict is **`REFUSED`**. [`docs/release-gate.md`](../release-gate.md) records every blocker, and
rule N38 holds that register to the gate's own output in both directions, so a blocker cleared on paper
that the checkout still shows fails, and so does one the checkout no longer shows that the paper keeps.
In outline:

- **Gates 2, 8 and 10 are held** by their rules.
- **Gates 3 to 6 are blocked as unread**: their clauses are shown, where they are shown at all, by
  bundles of the JS-3a, JS-4 and JS-9 series, which this check does not read.
- **Gate 1** waits on issuing the table, **gate 7** on an advertised composition and a claimed RID,
  **gate 11** on every relevant unit's human decision and on the owner's answer to the two package
  versions published before review, **gate 12** on the notices' confirmations and an Intl row, and
  **gate 13** on six vacant holders and a security intake.
- **Gate 9** is read from release-candidate bundle
  [JS-10-003](../evidence/js-10-003/README.md), file by file. Every run is retained with its totals,
  failure manifest and limit vector, and the bytecode floor holds; the native run could not be
  compared with its floor, which was set in the Windows calling convention, so
  `G9-wide-native-ratchet` blocks.

## 4. Choices

- **The gate lives in the architecture test assembly**, beside the rules it reads, rather than in a
  composition root: it reads the register, the assurance manifest and the documents, which a
  composition root has no business reading, and it runs where every other rule does. Its report is
  written on request by setting `BROILER_RELEASE_GATE_REPORT` to a path.
- **The rule holds the gate's honesty, not its verdict.** The day the owner and a named human take
  the acts the register names, the blockers shrink and the register shrinks with them, in the same
  change; the rule forbids only the two disagreeing.
- **Blocker identifiers are stable and carry no count**: the count of undecided units is in the
  gate's text, read live, and not in the register, as rule J12 asks of every figure a human would
  otherwise type.

## 5. Validation

- **Rule N38** passes: the gate refuses and names exactly what the register records; its two
  witnesses refuse and name the contradiction.

## 6. What is not done

- **Every act the register names**, which are the owner's and a named human's.
- **Reading gates 3 to 6**: a mechanism that reads their bundles would turn four unread blockers into
  held gates or named ones. It is not built here.

## 7. What would falsify this

- The gate answering `PASSED` while any relevant unit's human line reads `PENDING`.
- Any input other than the units' own human lines changing the count of decisions.
- A blocker the gate names that the register omits, or the reverse, with rule N38 passing.
