<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0066 - The profile on the shared number conversions: SJL-3.4's cut-over

**Status:** Decided 2026-10-12, in the change that lands the cut-over. It applies
[JSD-0065](0065-number-conversion-moves-to-the-shared-algorithms-library.md), the area's verdict.

**Date:** 2026-10-12.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list, rule I5's register and the composition register, and the release owner, for the notices
row. **All three roles are held by one person** (EX-30), and this record does not claim that anyone
independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-3, work package SJL-3.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-3-number-conversion-pilot.md)).

---

## Context

JSD-0065 accepted the number-conversion area for `Broiler.ECMAScript.Algorithms` and took the
algorithm choices; it changed nothing in this profile. This record is the change itself: the family
references the admitted package, its number conversions call the library, and the members that
computed them are deleted. The evidence that judges it - test262 in both forms, the fuel-parity
twins, the closures, the packages and the measurement - is bundle
[`sjl-3-001`](../../../../docs/evidence/sjl-3-001/README.md), under the rule SJL-0.9 committed before
any of this code existed.

## Decision

### 1. References

`Broiler.VM.Profile.JavaScript` and `Broiler.VM.Profile.JavaScript.Compiler` reference
`Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`, restored from the committed feed
`eng/packages/` with lock files. Rule I1's allow-list has one product entry for the package, naming
both projects and the admission probe, and pins its content hash; the probe pre-release of SJL-2.13
and its pin are removed from the feed.

### 2. Adapters

Each calls the library and keeps the coercion order, the `RangeError`s, the charging policy and the
string creation, and computes no conversion:

| Adapter | What it keeps | What it calls |
|---|---|---|
| `JsRealm.Number`: `toString`, `toFixed`, `toExponential`, `toPrecision` | `thisNumberValue`, `ToIntegerOrInfinity` of the argument in the specification's order, the range checks and their `RangeError`s, the string value | `NumberText.ToRadixString`, `ToFixed`, `ToExponential`, `ToPrecision` |
| `JsRealm.Global`: `parseInt`, `parseFloat` | `ToString`, `ToInt32` of the radix, the charge | `NumberParsing.ParseInt`, `ParseFloat` |
| `JsEngine.Conversions`: `ToNumber` of a string | The charge | `NumberParsing.StringToNumber` |
| `JsRealm.Json`: a JSON number's value | The JSON grammar, which finds the number's text | `NumberParsing.StringToNumber` over the span |
| `JsRealm.String`: `trim`, `trimStart`, `trimEnd` | The `this` coercion and the string | `NumberParsing.IsStrWhiteSpaceChar` |
| `JsBinary.NumberOf`, `JsBigInt`, `JsDecimal` | Their own grammars | `StringToNumber`; the white-space trim through `JsNumberFormat.TrimWhiteSpace` |
| `JsObject.IsArrayIndex`, `JsRealm.Object`'s canonical numeric test | Nothing: each is the library's call | `NumericKeys.TryParseArrayIndex`, `IsCanonicalNumericString` |
| The compiler: `SliceTokenizer`'s literal values, `JsParser.BigIntValueOf`, `JsParser.NumberKey` | The scan, the diagnostics, strictness, and the BigInt ceiling of the format (`CeilingBigIntConstantBytes`), passed as the width limit | `NumericLiterals.Evaluate`, `NumberText.ToString` |

**Charging.** The engine exposes its own meter to the library as a `WorkCharge`
(`JsEngine.WorkCharge`). An adapter charges one unit, as before, and passes the meter; the library
charges the text's length when it is not empty. The total is the one the deleted code charged - one
unit for an empty text, its length plus one otherwise - so no charge moves, and bundle `sjl-3-001`'s
part B admits no C2 row for this change.

### 3. Deleted

The 34 members the milestone's item 3 names are deleted and registered in rule I5's
`retired-members.json`, each with what replaced it: fifteen of `JsNumberFormat` (`ToJsString`,
`Decompose`, `Place`, `ToRadixString`, `RoundUpRadix`, `ToNumber`, `ParseInt`, `ParseFloat`,
`IsWhiteSpace`, `Trim`, `IsDecimalLiteral`, `ParseRadix` and their helpers), sixteen of `JsRealm`
(the statics from `NumberFixedText` to `NumberIncrementFixed`, their constants, and the overwritten
placeholder `parseInt` and `parseFloat`), `JsParser.NumberText` and `TryBigIntValue`, and
`SliceTokenizer`'s value accumulation.

**`JsNumberFormat` stays**, as the milestone allows when something remains, and its remarks say
what and why: the shared strings for the integers 0 to 255, `NaN` and the infinities in front of
`NumberText.ToString`, the array index's string, and the white-space trim's choice to return the
string it was given when nothing is trimmed. That is string creation, which the area's adapter
definition leaves to the engine; none of it computes a digit or reads a character's class.

### 4. Corrections

Every Broiler.VM difference SJL-1's run classified is corrected, as JSD-0065's decision 3 has it.
At this family's pin of the shared corpus (revision 3), every one of the 391 vectors that carried a
`divergence.broiler-vm` field now matches, and every other vector still does:

| Reason (`scripts/conformance/number-divergences.txt`) | Vectors | For example |
|---|---|---|
| `vm-shortest-digits`: 16 digits that do not read back where 17 are the shortest that do | 2 | `numbers/to-string/0127` |
| `vm-radix-longer`: more digits than the shortest at a radix other than 10 | 59 | `numbers/to-string-radix/0051` |
| `vm-radix-fraction`: fraction digits that do not denote the value | 134 | `numbers/to-string-radix/0056` |
| `vm-radix-integer`: integer digits that do not denote the value at 2^53 or more | 150 | `numbers/to-string-radix/0066` |
| `vm-parse-int-rounding`: `parseInt` not rounded to nearest beyond 2^53 | 12 | `numbers/parse-int/0225` |
| `vm-parse-int-approximated`: `parseInt` at an approximating radix, not the exact value this family's plan commits it to | 16 | `numbers/parse-int/0245` |
| `vm-non-decimal-rounding`: hexadecimal, octal and binary integers beyond 2^53 not rounded to nearest | 16 | `numbers/literal/0281` |
| `vm-legacy-octal-fraction`: `07.5` read as one literal | 2 | `numbers/literal/0091` |

The last was planned for SJL-13: the tokenizer still scans the literal, and now refuses it because
the library refuses its text. The white-space set is the specification's, from the pinned Unicode
17.0.0 data, where `JsNumberFormat.IsWhiteSpace` read the runtime's.

**One more, found by SJL-3.7's search for the area's surviving duplicates.** The wide compiler interned
a Number constant under the platform's `"R"` text of its value, which does not read back as the same
double for every value: `2**-25` and the double below it both render `2.980232238769531E-08`, so a
program holding both literals got one constant and `2.9802322387695312e-8 === 2.980232238769531e-8`
was `true`. `JsCompiler.NumberConstant` keys the pool by the value's bits now, as
`SliceProgramBuilder.Number` always did, and the CLI acceptance table holds the case
(`src/tests/cli/runs/two-number-literals-one-unit-apart.js`, its answer taken from node).

### 5. Records

- **The composition register** gains its shared-assemblies column (governance item V17), and every
  JavaScript root's row declares `Broiler.ECMAScript.Algorithms`; rules K4 and I3 read it.
- **Notices.** `THIRD_PARTY_NOTICES.md`'s ingesting-components table gains the package's row, not
  yet confirmed, which release gate 12 counts as a blocker.
- **Assurance.** The code that left this family's assurance coverage is exclusion EX-125 in bundle
  `sjl-3-001`'s section 9: it is now covered by the home's own assurance component.

## Correction entries

[JSC-297](../roadmap.corrections.md#jsc-297) (`support.md` section 2),
[JSC-298](../roadmap.corrections.md#jsc-298) (`release-gate.md`, gate 8),
[JSC-299](../roadmap.corrections.md#jsc-299) (`release-gate.md`, gate 12) and
[JSC-300](../roadmap.corrections.md#jsc-300) (`packages.md`).

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-3 is met.** Its evidence is bundle `sjl-3-001`, read under its rule, and the ledger
  records the result.
- **Literal scanning, diagnostics and strictness**, which stay in the front end until SJL-13.
- **Any other area.**

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 2 computes a digit, a rounding or a character class itself;
- a vector that matched before this change does not after it, or a fuel-parity twin completes at a
  different allowance, without a dated decision admitting it.
