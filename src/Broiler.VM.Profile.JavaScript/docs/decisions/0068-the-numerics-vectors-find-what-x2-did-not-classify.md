<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0068 - The numerics vectors find four differences JSD-0067's X2 did not classify

**Status:** Decided 2026-10-13. **JSD-0067's first falsifier fired**, on the first runs of SJL-4.2's
vectors, and this record classifies what it found. JSD-0067's decisions stand: no sub-area is refused,
and the area moves to `Broiler.ECMAScript.Algorithms` as that record decided.

**Date:** 2026-10-13.

**Owner:** the JavaScript profile owner. **Co-signer:** the Broiler.JS maintainer, as the home's owner.
**Both roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the programme's SJL-4, work package SJL-4.2. Broiler.JS's shared-libraries plan,
`docs/roadmap/SharedLibraries.md`, records the same findings for that engine.

---

## Context

[JSD-0067](0067-numerics-move-to-the-shared-algorithms-library.md) was derived from code, before the
area had vectors, and named its own falsifier: "a difference between the engines on SJL-4.2's vectors
is found that X2 does not classify".

**The vectors.** Corpus revision 6 (Broiler.JS `e3892d6`) adds the numerics area: 12,493 vectors over
the integer conversions, Float16, the Math kernels, the rounding modes through Temporal, and BigInt.
Each engine's first engine-level run on it is retained, Broiler.JS's in its bundle `sjl-4-001`
(`engine/run-revision-6.*`) and this family's in [bundle `sjl-4-001`](../../../../docs/evidence/sjl-4-001/README.md)
(`vectors/run-revision-6.*`). Revision 7 (Broiler.JS `b725bb0`) classifies every difference and fixes
one vector, and both engines then classify every vector.

**What X2 already classified.** In this family: `Math.expm1` and `Math.log1p` near zero and beyond,
`Math.cbrt`, `Math.hypot`, and `Math.sumPrecise`'s flush of a tiny term. In Broiler.JS: `1 ** NaN`,
`Power`'s 32-bit exponent, `asUintN` at very large widths, BigInt property keys beyond 2^32 - 1,
`Math.cbrt` and `Math.hypot`. Where X2 named no difference, the engines agree with the vectors and with
each other: every integer conversion, Float16 (every NaN payload decoded, and each engine's NaN policy
on encoding), every rounding mode, BigInt text at every radix, StringToBigInt with the specification's
white space, `asIntN`, and BigInt's conversion to and comparison with Number.

## Decision

1. **The four differences, and their classes.**

   | Difference | Class |
   |---|---|
   | Broiler.JS's StringToBigInt does not complete a hexadecimal text of 262,144 digits within its runner's five seconds (`numerics/policy/0018` and `0019`); this family completes it. `TryParsePrefixedDigits` multiplies the whole value by the radix for every digit, so its time grows with the square of the text | **The engine is wrong, corrected at SJL-4.5**: the member is on JSD-0067's X6 list, and the kernel's parse replaces it |
   | Broiler.JS's `Math.expm1` and `Math.log1p` are more than a unit in the last place from the exact value on some inputs: `Math.expm1(-745)` is -1.00075, `Math.expm1` just below its overflow threshold is an infinity, and near -2^-52, and for some tiny negative arguments of `log1p`, the result lies outside the two doubles around the exact value | **The engine is wrong, corrected at SJL-4.5** by JSD-0067 decision 5's `Expm1` and `Log1p`, within a unit in the last place over the whole domain. **JSD-0067's X1 reading of `expm1`, "an adoption: only Broiler.JS has a kernel", does not hold**: the library implements both functions, as decision 5 says, and adopts neither engine's kernel |
   | `1 ** Infinity` and `1 ** -Infinity` written as literals are 1 in Broiler.JS (`numerics/exponentiate-literal/0099` and `0100`), where `(-1) ** Infinity` written so is NaN | **The class X2 gives the ±1-to-an-infinity cases holds**: the engine is wrong, corrected at SJL-4.5. **X2's reading that "the literal spelling reaches the generic path, which has the case" holds for -1 and not for 1** |
   | This family refuses a BigInt literal wider than 65,536 bits with an early error (the wide manifest's construct limit, diagnostic 2104 `ConstructOutsideManifest`), where Broiler.JS evaluates it | **Engine policy of this family's compiler, outside the area**: the kernel's width policy, 2^20 bits, is X2's first row; the literal limit is the wide manifest's, and SJL-4 does not move it. Corpus revision 7 builds the one vector that met it from narrower literals, so that the vector pins the conversion it names |

2. **The corrections at SJL-4.5 include the first three**, listed in Broiler.JS's changelog under
   decision `SJL-D-16` (a) with the corrections JSD-0067 lists. This family's own corrections, the JSC
   entries SJL-4.4 files, are unchanged by this record.
3. **This family's pin of the corpus moves to revision 7** (`src/tests/vectors/pins/`), and its
   engine-level run admits every surface the profile builds, Intl and Temporal included, because the
   rounding vectors run through Temporal. That run classifies every vector.

## What this rejects, and why

- **Reading the parse's time-out as the runner's.** The same text completes in this family, and the
  time grows with the square of the digits: it is the algorithm's.
- **Reading the falsifier as not fired because X2's rows come close.** JSD-0067 named its own
  falsifier, and a near row is not a classification. Recording the miss is what the falsifier is for.
- **Refusing the Math kernels sub-area.** Each difference is in an engine's copy, which the library
  replaces; none is in what the library is decided to compute.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## Correction entries

None by this record. JSD-0067's text is not edited; this record is where its readings are corrected.

## Falsified if

- after SJL-4.5, any of the first three differences above persists in Broiler.JS;
- a run on corpus revision 7, or a later revision, finds a difference between the engines that neither
  JSD-0067's X2 nor this record classifies.
