<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0065 - Number conversion moves to the shared algorithms library: the pilot's area verdict

**Status:** Decided 2026-10-12: **accepted.** The programme's extraction gate (its roadmap section 5,
conditions X1 to X7) holds for the number-conversion area, and the area moves to
`Broiler.ECMAScript.Algorithms` at milestone SJL-3. Nothing in this profile changes with this record;
the cut-over is SJL-3.4's own change, and this record is what it applies.

**Date:** 2026-10-12.

**Owner:** the JavaScript profile owner. **Co-signers:** the Broiler.JS maintainer, as the home's
owner, and the core architecture owner, for rule I1's allow-list and the composition register. **All
three roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the programme's SJL-3, work package SJL-3.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts".

---

## Context

Core ADR 0014 admits the shared JavaScript libraries to this family, and
[JSD-0063](0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md) took this profile's half of
that admission. Neither moves an algorithm: each area moves only when the programme's own extraction
gate holds for it, and the area's first work package files the verdict. Number conversion is the
pilot. Its design is the programme's
[`areas/numbers.md`](../../../../docs/javascript-shared-libraries/areas/numbers.md); its vectors are the
shared corpus's number area, which both engines ran at SJL-1 (this repository's bundle
[`sjl-1-001`](../../../../docs/evidence/sjl-1-001/README.md), Broiler.JS's `docs/evidence/sjl-1-001/`).

## The gate, condition by condition

The correspondence was re-derived at **Broiler.VM `8519370`** and **Broiler.JS `b7a2a3f`**. Every file
the area design names is byte-identical there to the revisions it names, Broiler.VM `88d8b51d` and
Broiler.JS `44a214dc`: no commit between them touches any of them. The re-derivation found members the
design did not name, and they are added below.

### X1 - two implementations

**Met.** Both engines implement every responsibility of the area in merged code:

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| `Number::toString` at radix 10 | `JsNumberFormat.ToJsString`, `Decompose`, `Place`; the compiler's copy `JsParser.NumberText` | `JSNumber.ToECMAString`, `RoundTrippable`, reached from `Runtime` through the delegate `JSValue.NumberToECMAString` |
| `Number::toString` at radix 2 to 36 | `JsNumberFormat.ToRadixString`, `RoundUpRadix` | `JSNumberPrototype.DecimalToBase`, `DecimalToArbitrarySystem` |
| `toFixed`, `toExponential`, `toPrecision` digits | `JsRealm.Number`: `NumberFixedText` to `NumberIncrementFixed` | `JSNumberPrototype`: `RoundScaled`, `ToExactFraction`, `ExactSignificantDigits`, `Compare10Pow`, `ShortestSignificantDigits`, `FormatExponential` |
| `StringToNumber` | `JsNumberFormat.ToNumber`, `Trim`, `IsDecimalLiteral`, `ParseRadix` | `NumberParser.CoerceToNumber`, `ParseCore` and its members |
| `parseInt`, `parseFloat` | `JsNumberFormat.ParseInt`, `ParseFloat` | `NumberParser.ParseInt`, `ParseFloat` |
| Numeric literal values, BigInt literal values | `SliceTokenizer.ReadNumericLiteral`'s value loops; `JsParser.TryBigIntValue` | `Parser/NumberCoercion`; `JSBigInt.TryParseBigIntLiteral`; `FastCompiler.CreateClass.BigIntLiteralToValue`, `ParseBigIntRadix` |
| Array-index and canonical numeric keys | **`JsObject.IsArrayIndex`, `JsRealm.Object.ObjectIsCanonicalNumeric`** (not named by the design) | `NumberParser.TryGetArrayIndex`, `TryCoerceToUInt32`; **`Storage/KeyStrings.TryGetArrayIndex`, `TryGetCanonicalNumericIndex`** (not named by the design) |
| The string white-space set | `JsNumberFormat.IsWhiteSpace` | `NumberParser.IsWhiteSpaceOrLineTerminator`; `JSString.trimCharacters` |

### X2 - correspondence from code

**Met.** The design's sections 2 to 4 compare the two implementations, and SJL-1's engine-level runs
confirmed or refuted each reading: every difference either run found is classified in the corpus's
`scripts/conformance/number-divergences.txt` at revision 3. Every behavioural difference falls in one
of the gate's admissible classes, and none is unknown:

| Class | Differences |
|---|---|
| **The engine is wrong, corrected at SJL-3** (decision `SJL-D-16`) | Broiler.VM: `vm-shortest-digits`, `vm-radix-longer`, `vm-radix-fraction`, `vm-radix-integer`, `vm-parse-int-rounding`, `vm-non-decimal-rounding`, and the white-space set read from the runtime's Unicode data. Broiler.JS: `radix-longer`, `radix-inexact`, `radix-64-bit`, `radix-argument`, `precision-argument`, `next-line-white-space`, `non-decimal-rounding`, `long-significand-hang`, `long-significand-nan`, `bigint-leading-zero`, and U+FEFF missing from its trim set. Each is listed in its engine's changelog or JSC entry by SJL-3.4 and SJL-3.5. |
| **The engine is wrong, corrected by a later milestone** | Literal *scanning* stays in each front end until SJL-13 and SJL-14 (the milestone's "Does not do"): Broiler.JS's `strict-non-octal-decimal`, `legacy-octal-fraction`, `empty-exponent` and `bigint-fraction`, and Broiler.VM's `vm-legacy-octal-fraction`. The shared operation refuses each of them at library level, so the vectors pass there; an engine that scans past the refusal keeps its classified divergence until its front-end milestone. |
| **Implementation-defined** | `parseInt` at a radix other than 2, 4, 8, 10, 16 or 32, and at radix 10 beyond twenty significant digits, may be approximated; `vm-parse-int-approximated` is Broiler.VM's choice. Both engines' plans commit them to the exact value, which the library computes. The least significant digit of `Number::toString` is not always determined; the library gives the recommended, closest value. |
| **An engine policy that becomes a parameter** | The BigInt literal's maximum width (Broiler.VM's format ceiling; unbounded in Broiler.JS) is the caller's `LengthLimit`. Charging (Broiler.VM's callers charge; Broiler.JS charges nothing) is the caller's nullable `WorkCharge` (decision `SJL-D-10`). Broiler.VM's small-integer string cache stays in its adapter. |
| **Not in the area** | Broiler.JS's non-standard extensions stay in Broiler.JS (decision `SJL-D-17`): the `m` literal suffix, `Number.prototype.clz`, and `toLocaleString`'s .NET format strings. |

### X3 - neutral contract

**Met by the area's API**, which the library's baseline (rule SL6) states in full once SJL-3.3 lands.
Every public member takes and returns base class library values: `double`, `int`, `uint`, `bool`,
`string`, `ReadOnlySpan<char>`, `Span<char>` and `BigInteger`, and the library's own `LengthLimit`,
`WorkCharge`, `ResultStatus` and `Failure`. There is no engine type (rule SL1), no coercion and no
property access: each operation takes already-converted inputs, and the adapters keep coercion order,
`RangeError`s and object creation. A malformed literal is a status with a position, not an exception.
Parsing, whose work grows with its input, takes the caller's meter; formatting a double does bounded
work and takes none.

### X4 - no edge between the engines, none into Broiler.VM

**Met.** The library references neither engine and nothing in Broiler.VM (rule SL1 in the home).
Neither engine references the other: this family's package edges are rule I1's allow-list, and rules A2
and B3 refuse a Broiler.JS package or assembly here.

### X5 - boundaries hold

**Met.** The algorithms assembly holds no frontend, no optional data and no platform globalization or
regular-expression API (rule SL4). It enters every JavaScript composition root's closure, the
compiler-free ones included, which rule I3 allows for an assembly that is not a frontend; the
composition register declares it in every JavaScript row (SJL-3.4 item 4). The white-space set's
`Zs` members come from a table generated from the pinned Unicode 17.0.0 data, never from the runtime's.

### X6 - deletion list

**Met, extended.** The design's section 7 names every member the cut-overs delete, and the public
dispositions under decision `SJL-D-25`. The re-derivation adds the members X1 marks:

- **Broiler.VM:** `JsObject.IsArrayIndex` and `JsRealm.Object.ObjectIsCanonicalNumeric` become adapters
  of the library's key operations: their algorithm goes, and the hot path calls the library.
- **Broiler.JS:** `Storage/KeyStrings.TryGetArrayIndex` and `TryGetCanonicalNumericIndex` become adapters
  of the same operations, so `Broiler.JavaScript.Storage` references the library beside `Parser`,
  `Runtime` and `BuiltIns`.

Without these, each engine would keep an intra-engine duplicate of the area, which exit clause 14
refuses.

### X7 - owners accept

**Met, by one person.** The JavaScript profile owner, the Broiler.JS maintainer and the core
architecture owner accept the verdict, all the same person (EX-30). No component maintainer is
involved: the area has no data home and no component repository.

## Decision

1. **The number-conversion area moves** to `Broiler.ECMAScript.Algorithms`, in namespace
   `Broiler.ECMAScript.Numbers`, and neither engine keeps an implementation of its own.
2. **Decision `SJL-D-10` (a) applies as follows.** Every parsing operation of the area takes a nullable
   `WorkCharge` and charges the input's length in UTF-16 code units, in blocks, before it reads them;
   a BigInt literal's conversion charges its digits again. Formatting a double takes no meter. This
   family's adapters pass the engine's charge, so Broiler.VM's fuel accounting continues; where a
   charge's units move, the exhaustion verdicts that move are listed under bundle `sjl-3-001`'s rule,
   part B, class C2.
3. **Decision `SJL-D-16` (a) applies as follows.** Every difference X2 classifies as an engine being
   wrong is a correction, made by the cut-over and listed: in this family by a JSC entry per changed
   reading and in bundle `sjl-3-001`, and in Broiler.JS by its changelog. No difference is kept for
   compatibility.
4. **The algorithm choices of the design's section 5.2 are taken** as follows. Shortest digits are
   computed by a proven shortest-digits algorithm in the library, not from the platform's `"R"` format.
   Radix conversion computes the specification's shortest digits exactly. `toFixed`, `toExponential`
   and `toPrecision` keep this family's route - the platform's exact digits, with the tie detected
   exactly and broken upward - because the vectors pass it and it is the faster of the two. Parsing is
   correctly rounded for every literal form.

## What this rejects, and why

- **Keeping either engine's implementation beside the library.** A second copy is the duplication the
  programme exists to remove, and exit clause 14 refuses it.
- **Verifying the platform's `"R"` digits instead of computing shortest digits.** It is correct, but
  costs a parse per conversion on this family's hottest number path, which bundle `sjl-3-001`'s rule,
  part C, measures against the base.
- **The exact-rational route for `toFixed`, `toExponential` and `toPrecision`.** It is correct too, and
  slower; the reference-arm comparison of SJL-3.3 keeps it as Broiler.JS's arm, so a defect of the route
  kept is found against it.
- **Moving literal scanning now.** The milestone moves literal values only; scanning, diagnostics and
  strict-mode checks stay in each front end until SJL-13 and SJL-14.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.** If the
cut-over of SJL-3.4 cannot land as the milestone states it, that change files the record that says
what stays and why, as the milestone's item 3 requires for `JsNumberFormat`.

## Correction entries

None by this record. SJL-3.4 files the JSD and JSC entries the milestone's item 6 names, for
`support.md` section 2, `release-gate.md` gates 8 and 12, and `packages.md`.

## What this record does not decide

- **Any other area.** Integer conversions, Float16, Math and BigInt arithmetic are SJL-4.
- **The cut-over itself, its evidence or its acceptance.** They are SJL-3.4, SJL-3.6 and the ledger's.
- **Anything about the core.** Rule I1's allow-list, the composition register and ADR 0014 are the
  core's; ADR 0014's consequences section carries a pointer to this record.

## Falsified if

- after SJL-3.4 lands, a member X1 lists for Broiler.VM still computes a conversion of the area rather
  than calling the library;
- a difference between the engines on the area's vectors is found that X2's table does not classify;
- the library's public API names an engine type, coerces a value or reads a property.
