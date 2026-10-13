<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0069 - The profile on the shared numerics: SJL-4.4's cut-over

**Status:** Decided 2026-10-13, in the change that lands the cut-over. It applies
[JSD-0067](0067-numerics-move-to-the-shared-algorithms-library.md), the area's verdict, and
[JSD-0068](0068-the-numerics-vectors-find-what-x2-did-not-classify.md), the vectors' classification.

**Date:** 2026-10-13.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's
allow-list and rule I5's register and its revision, and the release owner, for the notices row. **All
three roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-4, work package SJL-4.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-4-numerics.md)). The
owner chose on 2026-10-13 to start SJL-4 before SJL-3 is met, and the ledger records that choice.

---

## Context

JSD-0067 accepted the numerics area for `Broiler.ECMAScript.Algorithms` and took the algorithm
choices; it changed nothing in this profile. This record is the change itself: the family references
the package version that carries the area, its integer conversions, Float16, Math kernels, exact
rounding and BigInt operations call the library, and the members that computed them are deleted. The
evidence that judges it - test262 in both forms, the fuel-parity twins and exhaustion verdicts, the
closures, the packages and the measurement - is bundle
[`sjl-4-001`](../../../../docs/evidence/sjl-4-001/README.md), under the rule SJL-4.1 committed before
any of this code existed.

## Decision

### 1. References

`Broiler.VM.Profile.JavaScript` and `Broiler.VM.Profile.JavaScript.Compiler` reference
`Broiler.ECMAScript.Algorithms` `0.2.0-preview.1`, packed from Broiler.JS
`01209c43a3a244b9e502908e68fcded49e9bc198` and restored from the committed feed `eng/packages/` with
lock files. Rule I1 admits one version of a package, so the allow-list's product entry moves to the new
version and its content hash, and `0.1.0-preview.1` and its pin leave the feed; the feed's README keeps
the old row, quoted, beside the new one.

### 2. Adapters

Each calls the library and keeps the coercion order, the errors, the charging and the value wrapping,
and computes no conversion, kernel or rounding:

| Adapter | What it keeps | What it calls |
|---|---|---|
| `JsEngine.ToInt32`, `ToUint32`, `ToInteger`, and every caller of the deleted `JsValue` statics; the public `JavaScriptValue.ToInt32()` and `ToUint32()`, as forwarders | `ToNumber`, in the specification's order | `IntegerConversions.ToInt32`, `ToUint32`, `ToIntegerOrInfinity` |
| `JsElements.Write`, `ReadValue`, `Narrow` and `BigIntElement`; `JsRealm.Shared`'s Atomics operands and results; `String.fromCharCode` | Storage and byte order; the one reading of a 64-bit element's bits, which `ReadValue` and the Atomics now share | `ToUint8`, `ToUint16`, `ToUint8Clamp`, `ToBigInt64`, `ToBigUint64`; `Float16.Encode` under `Float16NanPolicy.Canonical`, `Float16.Decode` |
| Every array-like length and `lastIndex`: `Array.prototype`'s methods, `concat`, `Array.fromAsync`, `Function.prototype.apply`, the RegExp methods and a typed array constructed from an array-like | `Get` and `ToNumber` | `IntegerConversions.ToLength` |
| `JsRealm.BinaryToIndex` | `undefined` as zero, `ToNumber`, and the `RangeError` naming the argument | `IntegerConversions.TryToIndex` |
| `JsRealm.Math`: `round`, `pow` and the `Exponent` opcode, `cbrt`, `expm1`, `log1p`, `f16round` | `ToNumber` of each argument, in order | `MathKernels.Round`, `Power`, `Cbrt`, `Expm1`, `Log1p`; `Float16.Round` |
| `Math.hypot`: `JsRealm.MathHypotOperands` | Every argument's `ToNumber` before any is examined, and the charges | `MathKernels.Hypot` |
| `Math.sumPrecise`: `JsRealm.MathSumPrecise` | `GetIterator(items, sync)`, the `TypeError` for an element that is not a Number and the `RangeError` at 2^53 elements, each closing the iterator, and the charges | `PreciseSum` |
| Temporal: `RoundTime`, `RoundDateTime`, `FormatOffsetRounded`, `RoundTimeDurationToIncrement`, `RoundInstant`, the duration totals, `NudgeToCalendarUnit`'s choice of a window's end, and the option reader `JsTemporal.RoundingModeOption` | The options and their errors, and the window's computation | `ExactRounding.RoundNumberToIncrement`, `RoundNumberToIncrementAsIfPositive`, `ApplyUnsignedRoundingMode`, `GetUnsignedRoundingMode`, `Negate`, `ToDouble`; the enumeration `RoundingMode` |
| `JsNumberFormatter.FormatNumericToString`, `JsDecimal.RoundToUnits` and `JsRawDecimal` | The Intl mathematical value's decimal rounding, SJL-9's | `ExactRounding.TryParseRoundingMode`, `GetUnsignedRoundingMode`; `UnsignedRoundingMode` |
| `JsBigInt` | Identity, `Value`, `Bits`, `Words`, `FromConstant`, `Policy`, equality; its operations' names, each one line | `BigIntMath`, `BigIntText`, under `Policy`, which is `new BigIntPolicy(MaximumBits)` |
| `JsHostRealm.AdmitBigInt`, `JsRealm.Clone`, `JsEngine.ToElementValue` | The surface refusal, the `RangeError` and the charges | `BigIntPolicy.Admits`, `BigIntMath.LinearCost`, `Words` |

**Errors stay the engine's.** Mixing a BigInt with a Number, a zero divisor and a negative exponent are
refused in `JsEngine.Operators` before the call, and an over-width status becomes the engine's
`RangeError` there and in `JsEngine.Conversions`: `JsBigInt`'s adapters answer `null` for it, as its
operations did.

**Charging.** The engine passes the meter it has exposed as a `WorkCharge` since SJL-3.4
(`JsEngine.WorkCharge`); `JsEngine.ChargeFuel`, the same meter as an `Action<ulong>`, which only the
BigInt operations took, is removed. The BigInt kernel charges this family's units, as JSD-0067 decision 3
has it, and Broiler.JS's reference-arm comparison found arm V's charges equal the library's on every
input it ran. The Math kernels and exact rounding take no meter; the charges this family took around
them stay in the adapters, unit for unit: `hypot`'s unit per argument and, when the sum is taken, one
more per argument, and `sumPrecise`'s two units per element beside the iteration's own.

### 3. Deleted

The 44 members the milestone names and JSD-0067's X6 extends are deleted and registered in rule I5's
`retired-members.json`, each with what replaced it: `JsValue`'s `ToInt32`, `ToUint32` and
`ToInteger`; `JsElements.Clamp` and `Low64`; the class `JsFloat16`; `JsRealm.ArrayToLength`;
`JsRealm`'s `MathRoundHalfUp`, `MathHypotenuse`, `MathPower`, `MathExpMinusOne`, `MathLogOnePlus` and
`MathDistil`, with the `cbrt` snap, which was not a member; `JsTemporalCore`'s `Negate`, `Unsigned`,
`UnsignedMode`, `ApplyUnsigned`, `RoundToIncrement`, `RoundToIncrementAsIfPositive`, `RoundsToSecond`
and `ToDouble`, and the enumeration `TemporalRounding`; the enumeration `JsUnsignedRounding` and
`JsNumberFormatter.Unsigned`; and twenty members of `JsBigInt`, the bodies of its text, its parse, its
shifts and its width check, and its cost formulas.

**Rule I5 is revised to see a type.** It read the members of types, and three of the retired names are
types declared directly in a namespace (`JsFloat16`, `TemporalRounding`, `JsUnsignedRounding`). A type
so declared is now a declaration of its own name; the witness declares `JsFloat16` again beside the
member it already declared, and each is reported once.

**What stays, and holds no algorithm:** `JsBigInt`, `JsElements.Narrow`, `JsRealm.MathHypotOperands`,
`JsRealm.MathSumPrecise`, `JsRealm.BinaryToIndex` and `JavaScriptValue.ToInt32()` and `ToUint32()`,
each an adapter of section 2; and `JsShared`'s Atomics width masks, which mask bits already converted,
at the element's width in bytes, and convert no Number.

### 4. Corrections

Every Broiler.VM difference JSD-0067's X2 classes as corrected at SJL-4.4 is corrected. At this
family's pin of the shared corpus (revision 7), every one of the 100 vectors that carried a
`divergence.broiler-vm` field now matches, and every other vector still does (22,634 of 22,634):

| What the field said | Vectors | For example |
|---|---|---|
| `Math.expm1` and `Math.log1p` compute `exp(x) - 1` and `log(x + 1)`, and lose their significant digits near zero | 70 (43 and 27) | `numerics/expm1/0008`, `numerics/log1p/0026` |
| `Math.cbrt` snaps the platform's root only when it cubes back exactly, so it is not correctly rounded (`Math.cbrt(0.125)` was `0.49999999999999994`) | 23 | `numerics/cbrt/0010` |
| `Math.hypot` scales by the largest argument in one pass and is more than a unit in the last place from the exact value on some inputs (`Math.hypot(2, 10, 11)` was not 15) | 5 | `numerics/hypot/0051` |
| `Math.sumPrecise` scales its terms when the largest is huge, which flushes a tiny term to zero | 2 | `numerics/sumprecise/0010` |

**Two more, without a vector at the kernel's level.** `Math.sumPrecise` iterates as the specification
says (X2's last row for this family): an array-like such as `{length: 1, 0: 5}` is a `TypeError`, as is
a string whose prototype's `Symbol.iterator` was deleted; a primitive whose prototype is iterable is
iterated; and `Symbol.iterator` is read once. And `Intl.NumberFormat`'s reading of a `0x`, `0o` or `0b`
string past the BigInt ceiling, found by this cut-over: `JsDecimal.FromString` took the parse's report
of a value too wide for its refusal and went on to format the absent value, a .NET
`NullReferenceException`; it now answers +Infinity, which is the value's Number, since a prefixed text
has no sign.

### 5. Records

- **Notices.** `THIRD_PARTY_NOTICES.md`'s row for the package moves to `0.2.0-preview.1` and names the
  second third-party component it carries, fdlibm's `expm1` and `log1p` (SunPro). The row is still not
  confirmed, so release gate 12's blocker for it stands, under the same name.
- **Packages.** `packages.md` names the new version ([JSC-301](../roadmap.corrections.md#jsc-301)).
- **Assurance.** The code that left this family's assurance coverage is exclusion EX-127 in bundle
  `sjl-4-001`: the home's own assurance component covers it in the Broiler.JS repository, as EX-125 says
  of the number-conversion area.

## Correction entries

[JSC-301](../roadmap.corrections.md#jsc-301) (`packages.md`).

## The milestone's readings

- "`JsBigInt` stays as a thin value wrapper holding identity, `Words`, `FromConstant` (...) and the
  policy constant": it holds those, and keeps its operations' names as one-line adapters, as JSD-0066
  kept `JsObject.IsArrayIndex`. Every caller is unchanged, and the policy and the meter are passed in
  one place.
- "`JsTemporalCore`'s rounding members: (...) `ToDouble(num, den)`": `ToDouble` of an integer, the two
  enumerations and `RoundsToSecond` go too, as JSD-0067's X6 extends the list.
- "the custom Math helpers in `JsRealm.Math`: (...) `MathHypotenuse`": deleted; what remains of it,
  the coercion and the charges, is `MathHypotOperands`, which computes nothing.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-4 is met.** Its evidence is bundle `sjl-4-001`, read under its rule, and the ledger
  records the result.
- **The Intl mathematical value**, which stays in `JsDecimal` until SJL-9, and **Temporal's
  consumers**, until SJL-10.
- **The wide manifest's BigInt literal limit**, which JSD-0068 leaves where it is.

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter in section 2 computes a conversion, a kernel, a rounding or a BigInt operation itself;
- a vector that matched before this change does not after it, or a fuel-parity twin completes at a
  different allowance or an exhaustion verdict moves, without a dated decision admitting it.
