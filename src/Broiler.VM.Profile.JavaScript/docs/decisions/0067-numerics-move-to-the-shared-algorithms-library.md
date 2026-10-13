<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0067 - Numerics move to the shared algorithms library: SJL-4's area verdict, sub-area by sub-area

**Status:** Decided 2026-10-13: **all four sub-areas accepted.** The programme's extraction gate (its
roadmap section 5, conditions X1 to X7) holds for each of the numerics area's sub-areas on its own:
integer conversions and Float16, the Math kernels, exact rounding, and the BigInt kernel. Each moves to
`Broiler.ECMAScript.Algorithms` at milestone SJL-4. Nothing in this profile changes with this record;
the cut-over is SJL-4.4's own change, and this record is what it applies.

**Date:** 2026-10-13.

**Owner:** the JavaScript profile owner. **Co-signers:** the Broiler.JS maintainer, as the home's
owner, and the core architecture owner, for rule I1's allow-list and the composition register. **All
three roles are held by one person** (EX-30), and this record does not claim that anyone independent
has read it.

**Milestone:** the programme's SJL-4, work package SJL-4.1. The same verdict is recorded in Broiler.JS's
shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area verdicts", which also records
decision `SJL-D-15`. **SJL-4 started before SJL-3 was met**, by the owner's dated choice the programme
ledger records; this record moves no gate of either milestone.

---

## Context

The area's design is the programme's
[`areas/numerics.md`](../../../../docs/javascript-shared-libraries/areas/numerics.md). Unlike the
pilot's, it has no vectors yet: SJL-4.2 adds them after this record, and the design marks its readings
*(reading)* until they do. So the correspondence below is **from code**: it was re-derived by reading
both engines at **Broiler.VM `a04b2e5`** and **Broiler.JS `4899f9c`** (the default branches after
SJL-3's merge; the later commits on this branch change records only). Every member is named by file
and line in [`correspondence.md`](../../../../docs/evidence/sjl-4-001/correspondence.md), which this
record's change retains in bundle `sjl-4-001` as the gate's input. Where a difference is stated with a
value, the value was worked from the code, not observed in an engine; SJL-4.2's vectors confirm or
refute each one, and a difference they find that this record does not classify falsifies it.

The re-derivation found far more than the design named, chiefly in Broiler.JS: copies of the same
conversions and roundings in places the design did not look, and readings of the design that do not
hold. Section "The design's readings" lists those; X6 extends the deletion lists.

## The gate, sub-area by sub-area

### Integer conversions and Float16

**X1 - met.** Both engines implement every responsibility:

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| `ToInt32`, `ToUint32` | `JsValue.ToInt32`, `ToUint32`, with the engine wrappers; **the public `JavaScriptValue.ToInt32()` and `ToUint32()`**, a second copy of the body | `JSValue.IntValue`, `ToUint32(double)`, `UIntValue`; the public `JSNumericOperators.ToInt32`, `ToUint32`; **`JSDate.CoercedIntValue`** |
| `ToInt16`, `ToUint16`, `ToInt8`, `ToUint8` | inline in `JsElements.Write`, `fromCharCode` and the Atomics width masks | inline in each typed array's `SetValue`, `DataView`, `fromCharCode` and `JSAtomics.TruncateToElement` |
| `ToUint8Clamp` | `JsElements.Clamp` | inline in `JSUint8ClampedArray.SetValue` |
| `ToBigInt64`, `ToBigUint64` bit patterns | `JsElements.Low64`, `Narrow` (Broiler.VM only: an adoption), **the Atomics masks of `JsRealm.Shared`** | the masks of `JSBigInt64Array`, **`JSBigUint64Array`**, `DataView.RawBitsFor`, **`JSAtomics.TruncateToBigElement`** |
| `ToIntegerOrInfinity` | `JsValue.ToInteger` | no single member: **six helpers** (in `JSNumberPrototype`, `JSTypedArray`, `JSArrayBuffer`, `JSArrayPrototype.Iteration`, the typed-array prototype and `JSAtomics.ToIntegerOperand`) and three inline copies; the public, saturating `IntegerValue` is not the specification's operation |
| `ToLength` | **`JsRealm.ArrayToLength`** and an inline copy in `JsRealm.Binary` | `JSJSON.ToLength` and **six more copies** (array iteration, `JSProxy`, the initializer twice, `JSTypedArray` twice, `JSIntl`) |
| `ToIndex`'s range rule | `JsRealm.BinaryToIndex`, the only copy | `DataView.ToIndex`, `JSBigInt.ToBitsIndex`, and **`DataView.ToByteOffset`, `JSAtomics.ToIndex`, `TypedArrayParameters.ToTypedArrayLength`, `JSArrayBuffer.ToBufferLength` and `ToBufferLengthLong`** |
| Float16 encode, decode, round | `JsFloat16.Encode` (one canonical NaN), `Decode`, `Round` | the `(Half)` casts in `JSMath.F16round`, `DataView` and `JSFloat16Array`, and the decodes `BitConverter.ToHalf` and `BitConverter.UInt16BitsToHalf` |

Members in **bold** are not named by the design.

**X2 - met.** The named kernels agree on every input read: −0, NaN, the infinities, 2^31, 2^32, 2^53
and beyond for the 32-bit conversions; the clamp's ties at 0.5, 1.5, 2.5, 254.5 and 255.5; `ToIndex`'s
bounds; Float16 rounding, which both take from the platform's single correctly rounded conversion. The
differences:

| Difference | Class |
|---|---|
| The NaN bytes a Float16 write produces: Broiler.VM writes one canonical quiet NaN; Broiler.JS keeps the sign and the high payload bits, so its bytes depend on the NaN and the host | **Implementation-defined** (NumericToRawBytes leaves a NaN's encoding to the implementation): the library's `Float16.Encode` takes the policy, canonical or preserve-sign-and-payload, and each engine passes its own |
| `Math.f16round(NaN)`'s bits, seen through a `Float64Array` | **Implementation-defined**, the same parameter |
| Typed arrays' byte order on a big-endian host: little-endian in Broiler.VM, the host's in Broiler.JS | **Engine policy, outside the kernel**: byte order is storage, which the milestone does not change |
| Broiler.JS's Atomics saturate an operand to a 64-bit integer and compute through a double, so `Atomics.add(i32, 0, 2**53)` leaves 0 where ToInt32 leaves 1, and `Atomics.compareExchange(i32, 0, 2**63, 5)` stores nothing where it should store 5 | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's `%TypedArray%.prototype.fill` converts its range with ToInt32, so `fill(1, 0, Infinity)` fills nothing | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's typed-array constructor converts the length before it checks the offset | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's `%TypedArray%.from` saturates a length to 2^31 − 1 | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's `JSDate.CoercedIntValue` wraps modulo 2^32 where MakeTime takes the integer value | **The engine is wrong, corrected at SJL-4.5** (a Date setter's argument is `ToIntegerOrInfinity`'s) |
| Broiler.JS's `String.prototype.endsWith` takes an undefined position as 0 through `IntegerValue` | **The engine is wrong, corrected at SJL-4.5** in the adapter |

**X6 - extended.** The deletion lists gain every member in bold above. Broiler.VM's public
`JavaScriptValue.ToInt32()` and `ToUint32()` become forwarders, as decision `SJL-D-25`'s reading for
Broiler.JS keeps public members; the native back end's in-range `ToInt32` template is machine code and
stays. Broiler.JS's public `IntegerValue` stays as a public member that forwards to the library's
`ToIntegerOrInfinity` and saturates as it does today; every specification path that used it calls the
library's operation instead.

### Math kernels

**X1 - met, with one adoption and one function neither engine implements well.**

| Function | Broiler.VM | Broiler.JS | Reading |
|---|---|---|---|
| `round` | `MathRoundHalfUp` | `JSMath.Round` | both custom: shared |
| `**` and `pow` | `MathPower`, used by `Math.pow` and the `Exponent` opcode | `JSMath.Pow`; the generic `JSValue.Power`; **the native emission `BExpression.Power`**, reached from every compiled route at one site, and **`LinqConverter.VisitPower`** | both custom: shared |
| `hypot` | `MathHypotenuse` | `JSMath.Hypot`, both overloads | both custom: shared |
| `cbrt` | an inline snap in the registration, not a member | `Cbrt`, `CorrectlyRoundedCbrt` | both custom: shared |
| `expm1` | **`MathExpMinusOne`**: a −0 guard over the platform's `ExpM1` | `Expm1`, Kahan's form | **an adoption**: only Broiler.JS has a kernel |
| `log1p` | **`MathLogOnePlus`**: a −0 guard over the platform's `LogP1` | `Log1p`: a Taylor series near zero, the platform elsewhere | neither is accurate across the range: the library implements it |
| `sumPrecise` | `MathSumPrecise`, which mixes iteration with the kernel, and `MathDistil` | `SumPrecise`, `TwoSum`, `SameNonFinite` and their constants | both custom: shared |
| `fround`, `clz32`, `imul`, `max`, `min`, `sign` | the platform, or a few lines over it | the same | **not shared**: the platform already gives the specified result, so there is no algorithm to move |
| `f16round` | `JsFloat16.Round` | a `(Half)` cast | the Float16 sub-area's `Round` |
| `random` | per realm | one per process | **engine policy**, not shared (the milestone's "Does not do") |

On .NET 10 the platform's `double.ExpM1` and `double.LogP1` are computed as `Exp(x) - 1` and
`Log(x + 1)`: Broiler.VM's remark that they are precise near zero does not hold.

**X2 - met.**

| Difference | Class |
|---|---|
| `1 ** NaN` is `1` in Broiler.JS on every path; `(-1) ** Infinity` and the other ±1-to-an-infinity cases are `1` on its compiled fast paths (the literal spelling reaches the generic path, which has the case) | **The engine is wrong, corrected at SJL-4.5**: both of Broiler.JS's paths call the library's `Power` |
| `Math.expm1` and `Math.log1p` near 0 in Broiler.VM: `Math.expm1(1e-300)` is `0` | **The engine is wrong, corrected at SJL-4.4**: the clause asks for a result "accurate even when the value of x is close to 0" |
| `Math.log1p` from 0.01 up, in both engines, tens of units in the last place from the exact value | **Implementation-defined** (implementation-approximated); the library's documented precision is the area's choice |
| `Math.hypot` with three or more arguments and `Math.cbrt` differ in the last place between the engines, and Broiler.VM's `cbrt` depends on the host's maths library | **Implementation-defined** (implementation-approximated); the library is correctly rounded for `cbrt` and documents its bound for `hypot` |
| `Math.sumPrecise` in Broiler.VM: its scaling flushes a tiny term to 0 (`[1e308, -1e308, 5e-324]` sums to 0), and a partial that overflows makes the result NaN where it should be an infinity or a cancelled finite sum | **The engine is wrong, corrected at SJL-4.4**: the clause specifies the exact sum |
| `Math.sumPrecise` in Broiler.VM falls back to an array-like walk, so it accepts `{length: 1, 0: 5}`, accepts `""` with `String.prototype[@@iterator]` deleted, refuses a primitive whose prototype is iterable, and reads `@@iterator` twice | **The engine is wrong, corrected at SJL-4.4** in the adapter, which keeps the iteration |
| `Math.sumPrecise` is absent from Broiler.JS unless a feature flag is set | **Engine policy**, unchanged |
| `Math.random`: per realm in Broiler.VM, one shared generator in Broiler.JS | **Engine policy**, not shared |

**X6 - extended.** Broiler.VM: `MathExpMinusOne`, `MathLogOnePlus`; the `Exponent` opcode's call to
`MathPower` is re-pointed; `MathSumPrecise` is split, its kernel deleted and its iteration kept as the
adapter. Broiler.JS: `JSMath`'s `Round`, `Pow`, both `Hypot` overloads, `Cbrt` with
`CorrectlyRoundedCbrt`, `Expm1`, `Log1p`, `SumPrecise` with `TwoSum`, `SameNonFinite` and its
constants; `JSValue.Power`'s arithmetic; `BExpression.Power`'s `Math.Pow` emission, which emits a call
to the library instead; and `LinqConverter.VisitPower`.

### Exact rounding

**X1 - met.** Broiler.VM holds two copies, `JsTemporalCore`'s (with `RoundsToSecond`, a third
application of the unsigned modes over a window, and `ToDouble` of a rational and of an integer) and
`JsDecimal`'s unsigned modes with `JsNumberFormatter`'s mapping. Broiler.JS holds about eleven:
`TemporalRoundingOptions` (the design's "`JSTemporalRoundingOptions`" is its file), a second
`NormalizeRoundingMode` and `RoundToIncrement` in `JSTemporalDuration.Arithmetic`, private copies of the
unsigned modes in `JSTemporalZonedDateTime.Rounding`, `RoundOffsetToMinute`, three inline nudge
decisions, two rational-to-double conversions (`JSTemporalDuration.NearestDouble`, `RatioToDouble`), and
`JSIntlNumberFormat`'s unsigned modes, `RoundRatio`, `RoundFixed`, `RawPrecisionDigits` and
`ToRawPrecision` (the design's "`JSIntl`" is the static class beside it).

**X2 - met.** Broiler.VM's copies follow the specification, but for a rational whose double is
subnormal, which `ToDouble` rounds twice and no Temporal value reaches. The differences:

| Difference | Class |
|---|---|
| Broiler.JS's scientific and compact notations decide the exponent and the mantissa with the platform's away-from-zero rounding, ignoring `roundingMode` and the increment (`{notation: "scientific", maximumFractionDigits: 1, roundingMode: "floor"}` formats 9.99 as `0.9E1`) | **The engine is wrong, corrected at SJL-4.5**: both call the library's modes |
| Broiler.JS's `NudgeToCalendarUnit` rounds linearly inside one unit, not over the whole increment's window (a two-month increment of `P1M` gives `PT0S`) | **The engine is wrong, corrected at SJL-4.5**: the library applies the unsigned mode over the window |
| Broiler.JS's `Duration.prototype.total` with a ZonedDateTime `relativeTo` rounds twice through a double | **The engine is wrong, corrected at SJL-4.5**: the library converts the exact rational once |
| A rational whose double is subnormal or beyond 2^768: Broiler.VM rounds twice, Broiler.JS answers 0 or Infinity | **Both wrong, latent**: no value either engine computes reaches it; the library's conversion is correctly rounded over the whole range |
| Broiler.JS rounds an Intl Number's exact binary value where ECMA-402 rounds its shortest decimal (`maximumFractionDigits: 1` formats 1.15 as `1.1`); formats a BigInt in scientific notation through a double; and `PluralRules` ignores the digit options | **The engine is wrong, corrected by SJL-9**: each is the Intl mathematical value or its consumers, which the milestone's "Does not do" leaves to SJL-9 |
| Broiler.JS's `PlainTime`, `PlainDateTime` and `ZonedDateTime` `round` round the nanoseconds since midnight rather than the quantity the specification names (a `halfEven` round of 01:10 to 20 minutes gives 01:20); calendar-unit rounding of a ZonedDateTime uses 24-hour days across a change of offset; non-ISO calendars skip difference rounding; `NudgeToCalendarUnit` returns the end where the window shifted without rounding up | **The engine is wrong, corrected by SJL-10**: the Temporal consumers, not the rounding |
| `Instant.prototype.since` negates the difference instead of the mode in Broiler.JS | **No observable difference** |

**X6 - extended.** Broiler.VM: `JsTemporalCore`'s `TemporalRounding` and `Unsigned` enumerations,
`RoundsToSecond` and `ToDouble(BigInteger)` beside the members the design names; `JsDecimal`'s
top-level `JsUnsignedRounding` and `JsNumberFormatter.Unsigned`. **`JsDecimal.RoundToUnits` and
`JsRawDecimal` stay**: they round the Intl mathematical value's decimal digits, in linear and metered
work, and that value is SJL-9's; they take the unsigned mode's decision from the library. Broiler.JS:
every copy X1 names, and the two `Math.Round` sites in `JSIntlNumberFormat`.

### The BigInt kernel

**X1 - met.**

| Responsibility | Broiler.VM (`JsBigInt` unless stated) | Broiler.JS (`JSBigInt` unless stated) |
|---|---|---|
| Width policy | `MaximumBits`, `Bounded`; **`JsHostRealm.AdmitBigInt`**'s copy of the check | none |
| Meter | `LinearCost`, `ProductCost`, `LeafCost`, through the engine's fuel charge | none |
| Arithmetic, bitwise operations, shifts | every operation, each answering null past the ceiling | the one-line operator bodies, `Power`, `Shift`; **`JSAtomics.ApplyBig`** |
| `asIntN`, `asUintN` | `AsIntN`, `AsUintN` | `AsIntN`, `AsUintN`; **`JSAtomics.TruncateToBigElement`** (the 64-bit case) |
| Text out | `ToDecimalString` and `ToRadixString`, divide and conquer, culture-invariant | `BigIntegerToRadixString`; **radix 10 through `BigInteger.ToString()`** in five places |
| `StringToBigInt` | `TryParseStringInteger`, `TryParseRadix`, `ParseDecimal`; **`JsDecimal.FromString`**'s use of it | `TryStringToBigInt`, `TryParseBigIntString`, `TryParsePrefixedDigits`, **`IsStrWhiteSpaceOnly`** |
| Number to BigInt, BigInt to Number | `FromIntegralNumber`, `ToNumber` | `NumberToBigInt`, `ToNumber`; **`EqualsLiteral(double)`**, a truncating second conversion; **the dead `BigIntegerExtensions.FromDouble`** |
| Comparison with a Number | `CompareToNumber`, the engine's `OrderAgainstNumber` for NaN and the infinities | `JSBigIntExtensions.CompareToNumber`, `TryCompare`, **the `Equals` Number branch** |
| Property key | the engine's `ToPropertyKeyValue` | **`ToKey`**, a `(uint)` conversion |

The shared library already holds part of the area since SJL-3: a BigInt literal's value
(`NumericLiterals`), the string white-space set, and the ties-to-even core of an integer's conversion
to a double (`ExactRounding`, internal).

**X2 - met.**

| Difference | Class |
|---|---|
| The ceiling: Broiler.VM refuses past a fixed width with a `RangeError`; Broiler.JS has none, so `2n ** 2000000000n`, `BigInt` of a very long string and `toString` of a huge value run without limit and without interruption | **Engine policy, a parameter**: the kernel takes a width policy and a meter (decisions `SJL-D-15` and `SJL-D-10`). No clause names a size limit, so the ceiling is a policy, not an implementation-defined result |
| Broiler.JS's radix-10 text follows the current culture's negative sign (`String(-1n)` under a culture whose minus sign is U+2212) | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's `Power` casts the exponent to a 32-bit integer, so `1n ** 2n ** 31n` raises a .NET overflow; a left shift by 2^31 − 1 raises a .NET exception | **The engine is wrong, corrected at SJL-4.5**: past the representation's limit the kernel answers over-width, which maps to `RangeError` (decision `SJL-D-15` (a)) |
| Broiler.JS's `asUintN` computes `1 << (int)bits`, so `asUintN(2 ** 32, -1n)` is `0n` and `asUintN(2 ** 31, -1n)` divides by zero | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's compiled `==` against a Number literal compares through a truncating conversion, so `2n ** 53n + 1n == 2 ** 53 + 1` and `2n ** 1024n == 1e400` are `true` | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's BigInt property key raises a .NET overflow beyond 2^32 − 1 and for a negative value, and 4294967295n keys differently from its string | **The engine is wrong, corrected at SJL-4.5** |
| Broiler.JS's typed-array `ToBigInt` takes a Number that an object's `valueOf` returns, and `ToBigInt` converts with the default hint where the specification names the number hint | **The engine is wrong, corrected at SJL-4.5** in the adapters |
| `BigInt("  12 ﻿")`, `" 0x10 "`, `""`, `"-0x10"`; comparisons with fractional and non-finite Numbers; `Number(2n ** 1024n)`; `asIntN(0, x)` | **No difference**: both engines already agree, and the white space is the shared set in both since SJL-3 |

**X6 - extended.** Broiler.VM: the arithmetic, text and parse bodies of `JsBigInt` as the design says,
and `JsHostRealm.AdmitBigInt`'s width check and `JsRealm.Shared`'s duplicate of `JsElements.Read`'s
BigInt arms; `JsBigInt` stays a value wrapper, and the engine-side charges (a comparison between two
BigInts, a typed-array element, a host crossing, a clone) stay in the adapters. Broiler.JS: every member
in bold above and the operator bodies, so every BigInt operation reaches the kernel and its over-width
status.

### X3 - neutral contract, all four sub-areas

**Met by the area's API.** Every public member takes and returns base class library values (`double`,
`int`, `long`, `ulong`, `ushort`, `byte`, `sbyte`, `short`, `bool`, `string`, `ReadOnlySpan<char>`,
`ReadOnlySpan<double>`, `BigInteger`) and the library's own types: `WorkCharge`, a width policy, the
status of an operation, the rounding modes and the Float16 NaN policy. There is no engine type (rule
SL1), no coercion, no property access and no iteration: `PreciseSum` is an accumulator the engine feeds
and reads, so each engine keeps iterating and closing the iterator itself. Errors are the engines':
the library answers a status, and the adapters raise the `TypeError`s and `RangeError`s.

### X4 - no edge between the engines, none into Broiler.VM

**Met**, as for the pilot: the library references neither engine and nothing in Broiler.VM (rule SL1 in
the home), and rules A2 and B3 refuse a Broiler.JS package or assembly in this family.

### X5 - boundaries hold

**Met.** The area holds no data, no frontend and no platform globalization (rule SL4): its text is
culture-invariant by construction, which is the correction Broiler.JS's radix-10 text needs.

### X7 - owners accept

**Met, by one person.** The JavaScript profile owner, the Broiler.JS maintainer and the core
architecture owner accept the verdict for each sub-area, all the same person (EX-30). No component
maintainer is involved.

## The design's readings

Each reading below is the design's, and the code at the revisions above does not bear it out. The
design is revised in the same change, quoting what it read.

- "`TryParseBigIntString` uses .NET `Trim`": it uses the shared `StrWhiteSpace` set, since SJL-3.5.
- "`(-1) ** Infinity` ... `1` *(reading)*" in Broiler.JS: the literal spelling gives `NaN`; only the
  compiled fast paths give `1`.
- Broiler.VM's `expm1` and `log1p` remark "claims precision near zero *(the claim is to be checked...)*":
  the platform computes `Exp(x) - 1` and `Log(x + 1)`, so it does not hold.
- "No observable difference in the conversions was found by reading": true of the kernels the design
  names, not of the copies it does not (X2, integer conversions).
- The names "`JsDecimal.JsUnsignedRounding`", "`JSTemporalRoundingOptions`" and "`JSIntl`'s unsigned
  rounding" are a top-level enum, a file, and `JSIntlNumberFormat`'s members respectively.

## Decision

1. **The numerics area moves**, all four sub-areas, to `Broiler.ECMAScript.Algorithms`, in namespace
   `Broiler.ECMAScript.Numbers`, and neither engine keeps an implementation of its own beyond what X6
   says stays.
2. **Decision `SJL-D-15` (a) applies**, as Broiler.JS's area record files it: Broiler.JS passes the
   unbounded width policy and a no-op meter, and maps the kernel's over-width status to `RangeError` at
   the representation's limit, never to a .NET exception.
3. **Decision `SJL-D-10` (a) applies as follows.** The BigInt kernel charges each operation **in this
   family's current units**, the formulas of `JsBigInt`'s `LinearCost`, `ProductCost` and `LeafCost`
   with its one-word-more convention for a value's size, before it computes; `ToNumber` and the
   comparison with a Number charge as they do here, and parsing reports an over-width result with its
   sign. So the fuel-parity twins and exhaustion verdicts of bundle `sjl-4-001`'s rule hold without a
   charge-model decision; if they do not, the difference is a defect of the kernel, not a C2 class. The
   Math kernels and exact rounding do bounded work for their inputs and take no meter; where this family
   charges around them today, the charge stays in the adapter.
4. **Decision `SJL-D-16` (a) applies.** Every difference X2 classes as corrected at SJL-4.4 or SJL-4.5
   is a correction, listed in this family by a JSC entry per changed reading and in bundle `sjl-4-001`,
   and in Broiler.JS by its changelog. The ones X2 leaves to SJL-9 and SJL-10 are recorded there, and
   their vectors carry a classified divergence until then.
5. **The algorithm choices are taken** as follows:
   - `Power` holds the specification's special cases, and is the one entry for `**` and `Math.pow` in
     both engines, Broiler.JS's emitted fast path included;
   - `Round` is the specification's; `Cbrt` is correctly rounded; `Hypot` is computed with scaling and
     a compensated sum, and the library states its bound; `Expm1` and `Log1p` are accurate to within one
     unit in the last place over their whole domain, near zero included;
   - `PreciseSum` is the exact algorithm of the `Math.sumPrecise` proposal's reference, as a streaming
     accumulator, with its overflow partial;
   - `Float16.Encode` rounds ties to even and takes the NaN policy; `Decode` is exact;
   - exact rounding is over exact rationals (`BigInteger` numerator and denominator), with the
     specification's unsigned modes, their application over a window for `NudgeToCalendarUnit`, the
     increment roundings, a mode's negation and the option names' parse, and a rational's conversion to
     the nearest double, ties to even, over the whole range, subnormals included. It extends the
     library's internal `ExactRounding`, whose name and namespace the design's sketch would otherwise
     duplicate;
   - the BigInt kernel's text is divide and conquer and culture-invariant, its parse checks the radix
     and reports over-width, and its conversion to a Number is exact, ties to even, sharing the
     existing core.

## What this rejects, and why

- **Keeping any engine's copy beside the library.** A second copy is the duplication the programme
  exists to remove.
- **Sharing `fround`, `clz32`, `imul`, `max`, `min` or `sign`.** The platform already computes the
  specified result; a library member would wrap a platform call and remove no algorithm.
- **A charge model of the library's own for BigInt.** It would move this family's fuel verdicts and
  need a decision for every moved row; keeping this family's units costs nothing in Broiler.JS, which
  passes no meter.
- **Moving the Intl mathematical value now.** It is SJL-9's (the milestone's "Does not do"); this
  milestone gives it the modes it rounds with.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.** A
sub-area that cannot be implemented as decided here is refused by such a record, with its reason and
its source pointers, while the others proceed, as SJL-4.1 allows.

## Correction entries

None by this record. SJL-4.4 files the JSC entries for every reading of this family that its cut-over
changes.

## What this record does not decide

- **Any other area.** Text algorithms are SJL-5, Date arithmetic SJL-6, the Intl mathematical value
  SJL-9 and the Temporal consumers SJL-10.
- **The cut-over itself, its evidence or its acceptance.** They are SJL-4.4, SJL-4.6 and the ledger's;
  bundle `sjl-4-001`'s rule, committed with this record, decides the evidence.
- **Anything about the core.**

## Falsified if

- a difference between the engines on SJL-4.2's vectors is found that X2 does not classify;
- after SJL-4.4 lands, a member X1 lists for Broiler.VM still computes a conversion, a kernel, a rounding
  or a BigInt operation of the area rather than calling the library, except what X6 says stays;
- the BigInt kernel charges any operation differently from this family's units at the base, so that a
  fuel-parity twin or an exhaustion verdict moves;
- the library's public API names an engine type, coerces a value, reads a property or iterates.
