<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0060 - The support table, drafted and not issued

**Status:** Proposed, 2026-10-05. **Owner decision pending.** It records phase F9's slice R2 under
[JSD-0059](0059-the-release-under-the-mvp-programme.md): the JavaScript profile's support table,
[`docs/support.md`](../support.md), written in full and **not issued**, and rules **N34**, **N35** and
**N36**, which hold it to the checkout.

**What the owner decided.** On 2026-10-05 the repository owner asked for phase F9 to be done. Nothing
here is signed beyond that request, and nothing here issues the table.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are held
by one person**, and it does not claim the co-signature is independent.

**Milestone:** JS-10. Phase F9, slice R2.

---

## 1. Why draft a table nobody may issue

Release gate 1 ([section 22](../roadmap.gates.md#22-release-gates)) issues a support table, and release
gate 11 issues nothing a named human has not read. No human has: `HUMAN_REVIEW.md` is unsigned. So the
table cannot be issued, and [the MVP programme record](../../../../docs/mvp.md) section 3.5 forbids
treating anything as issued.

What the gate needs besides the signature is a fact, and a fact can be kept true now: the versions the
checkout implements and accepts, the manifests it declares, the editions it pins, the defaults it
sets, what a program meets where a capability is absent, the amendment register's state. **A table
written at release time is a rewrite under pressure; a table held to the checkout from now on is a
signature.** So the table is drafted in full, every row carries an evidence cell, and three rules keep
the parts a mechanism can read from drifting.

## 2. What the draft carries

Every cell gate 1 names, in twelve sections:

1. **Identity** - the profile, the core contract version it implements and the minimum the core
   accepts, the format-version range, the manifest set, the conformance manifest, the pinned edition
   and suite, the packages.
2. **What varies and what is fixed** - roadmap section 6's five kinds of surface and the four varying
   ones it does not list by kind: `Math.random`, the host's clock, the collector's timing, and anything
   a host capability supplies.
3. **The language, by manifest and surface** - what the contract admits, what this profile implements,
   and what a composition provides, kept apart. The wide manifest's row quotes run r32's totals and
   **no percentage**, as roadmap section 14 requires.
4. **What is not provided**, each with what a program or host meets instead - the `WebAssembly`
   host-object surface first.
5. **The amendment register's state**, one row per roadmap section 18 row, with the procedure stated
   as unexecutable.
6. **The extraction gate's state** for this profile's own mechanisms, with no verdict.
7. **The declared-default vector**, with its reconciliation recorded as unowned.
8. **Runtime identifiers**, none claimed, each with what has been shown and why it is not claimed.
9. **Packages and compositions**, none advertised.
10. **Operational holders**, the six roles no record names recorded **vacant**.
11. **Suppressions** - none.
12. **What the table does not say.**

## 3. The rules

| Rule | What it holds | Witnesses |
|---|---|---|
| **N34** | The banner; each identity cell to the source that declares it; the manifest set to every identifier the checkout declares, in both directions, with a section 3 row for each; the `WebAssembly` row to the realm's globals; section 5 to roadmap section 18's rows in both directions; each default to `Defaults()`; the RID sentence; and every row of an evidenced table to a registered or asserted rule or a file that exists, with no bare yes | a table claiming core contract 2, one omitting an amendment row, one misquoting `CallDepth`, one with a bare yes and an evidence cell that names nothing |
| **N35** | Section 6: the first condition's state and what would satisfy it, no verdict word, no identifier of another profile component | a section carrying a verdict and `broiler.webassembly` |
| **N36** | Section 11 against a scan of every source and project of the JavaScript family for a pragma, a suppression attribute, `NoWarn` or `WarningsNotAsErrors` | a source carrying a pragma |

**What none of them does**: decide that a row's prose is true. They hold the cells a mechanism can read
to where those cells come from, and every other row to having evidence at all.

## 4. Choices

- **The amendment section follows roadmap section 18's rows by their exact first cells**, so a row
  added to the roadmap fails N34 until the table states it, and a row the table invents fails too.
- **Two readings disagree, and the table takes the narrower one.** The roadmap and the delivery plan
  call amendment rows "filed and held"; [JSD-0007](0007-cross-profile-position-and-amendment-grading.md)
  records that no amendment is filed with the core and none is admissible. "Filed" there means written
  into this profile's register, not filed with the core, so the table says **held** and states that
  nothing has been filed with the core. ADR 0013 records the universal
  bytecode's G1 outcome for its own mechanisms; the table records the extraction gate's state for this
  profile's mechanisms only, which N35 holds it to.
- **Six operational roles are vacant** rather than assigned to the owner by default: release gate 13
  asks for each to be named, and appointing is the owner's act, not a document's. The two roles ADR
  0012 does name, vulnerability response and recertification, are recorded with what ADR 0012 itself
  says is missing.
- **Packages already published are stated as published.** `0.1.0-preview.4` and `preview.5` reached
  nuget.org by the owner's workflow on 2026-09-23, before any review; the table records that rather
  than implying nothing was published.

## 5. What drafting found stale, and what it amended

Reading every cell against the checkout found three statements that had stopped being true; each is
amended in place and recorded by [JSC-286](../roadmap.corrections.md#jsc-286):

- Roadmap section 9's table still said `typeof Intl` and `typeof Temporal` answer `"undefined"`.
- The ledger counted two runtime identifiers published and run; bundle JS-7-001 makes it three.
- The ledger's "no measurement exists" predates bundle JS-10-001.

## 6. Validation

- **Rules N34, N35 and N36** pass over the table and fail over each of their six witnesses with the
  violation the witness names. The architecture suite has 356 tests.

## 7. What is not done

- **Issuing the table**: a named human's review of every relevant unit, and the owner's act.
- **Slices R3 and R4**, which follow.
- **Rules N29 to N32** still have no register row (JSC-285).

## 8. What would falsify this

- A cell N34 reads that differs from the source it reads, and the rule passing.
- An amendment row in roadmap section 18 the table does not state, or the reverse.
- A suppression in a shipped JavaScript source that section 11 does not inventory.
- Any wording in this record or the table that reads as issuing, claiming or accepting.
