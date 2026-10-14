<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-4.1's correspondence from code: the numerics area in both engines

**Written:** 2026-10-13, at SJL-4.1, as the input of
[JSD-0067](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0067-numerics-move-to-the-shared-algorithms-library.md)'s
X1, X2 and X6, before any code of SJL-4 exists. **Read at** Broiler.VM `90afea8` and Broiler.JS `4e29e5b`,
whose product sources are those of the default branches after SJL-3's merge, Broiler.VM `a04b2e5` and
Broiler.JS `4899f9c`. **By reading only:** no engine was run. Where a value is given, it was worked from
the code (for the Math kernels, by emulating each kernel's arithmetic with the C library .NET uses on
Linux); SJL-4.2's vectors confirm or refute each one. **It holds no figure of a run.**

**Paths.** `VM/` is `src/Broiler.VM.Profile.JavaScript/` in Broiler.VM, `VMC/` its `.Compiler`, `VMF/` its
`.Format`. `RT/`, `BI/`, `EX/` and `CO/` are `Broiler.JS/Broiler.JavaScript.Runtime/`, `.BuiltIns/`,
`.Expressions/` and `.Compiler/` in Broiler.JS, and `SL/` is
`Broiler.ECMAScript/Broiler.ECMAScript.Algorithms/Numbers/` there. A line number is the line at the
revision above.

---

## 1. Integer conversions and Float16

The shared library has none of these yet; both engines already reference `Broiler.ECMAScript.Numbers`.

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| ToInt32 (7.1.6) | `JsValue.ToInt32` VM/JsValue.cs:358; `JsEngine.ToInt32` VM/JsEngine.Conversions.cs:331; public `JavaScriptValue.ToInt32()` VM/JavaScriptValue.cs:158; the native x64 in-range template VMF/JsValueLayout.cs:1542-1583 | `JSValue.IntValue` RT/JSValue.cs:383; public `JSNumericOperators.ToInt32` RT/JSNumericOperators.cs:35; `JSDate.CoercedIntValue` BI/Date/JSDatePrototype.Setters.cs:15 |
| ToUint32 (7.1.7) | `JsValue.ToUint32` VM/JsValue.cs:363; `JsEngine.ToUint32` Conversions.cs:336; public `JavaScriptValue.ToUint32()` VM/JavaScriptValue.cs:164 | `JSValue.ToUint32(double)` RT/JSValue.cs:391; `UIntValue` :428; public `JSNumericOperators.ToUint32` :38 |
| ToInt16, ToUint16 (7.1.8, 7.1.9) | inline: VM/JsBinary.cs:313; VM/JsRealm.String.cs:115; VM/JsShared.cs:172, :236; VM/JsRealm.Shared.cs:436-437 | inline: JSInt16Array.cs:49, :52; JSUInt16Array.cs:31, :34; DataView.cs:629, :665; JSStringStatic.cs:23; `JSAtomics.TruncateToElement` BI/Objects/JSAtomics.cs:125-134 |
| ToInt8, ToUint8 (7.1.10, 7.1.11) | inline: VM/JsBinary.cs:303; VM/JsRealm.Shared.cs:434-435 | inline: JSInt8Array.cs:48, :51; JSUInt8Array.cs:38, :41; DataView.cs:652, :687; JSAtomics.cs:127-128 |
| ToUint8Clamp (7.1.12) | `JsElements.Clamp` VM/JsBinary.cs:605 | inline in `JSUint8ClampedArray.SetValue` BI/Array/Typed/JSUint8ClampedArray.cs:53-68 |
| ToBigInt64, ToBigUint64 bits (7.1.15, 7.1.16) | `JsElements.Low64` VM/JsBinary.cs:490, `Narrow` :518 (Broiler.VM only), `ReadValue` :407; `JsEngine.ToElementValue` Conversions.cs:283-298; masks VM/JsRealm.Shared.cs:392, :455 | masks JSBigInt64Array.cs:38, JSBigUint64Array.cs:37; `DataView.RawBitsFor` DataView.cs:555-559; `JSAtomics.TruncateToBigElement` JSAtomics.cs:136-142 |
| ToIntegerOrInfinity (7.1.5) | `JsValue.ToInteger` VM/JsValue.cs:383 (−0 to +0 at :393); `JsEngine.ToInteger` :341 | no single member: `IntegerValue` RT/JSValue.cs:411 (saturating, public virtual; overrides RT/JSUndefined.cs:23, BI/Null/JSNull.cs:24); helpers JSNumberPrototype.cs:254, JSTypedArray.cs:46, JSArrayBuffer.cs:60, JSArrayPrototype.Iteration.cs:145, JSTypedArray.prototype.cs:28, `JSAtomics.ToIntegerOperand` :113; inline JSBigInt.cs:570, JSIntl.cs:3191, BuiltInsAssemblyInitializer.cs:1852 |
| ToLength (7.1.20) | `JsRealm.ArrayToLength` VM/JsRealm.Array.cs:1751; inline VM/JsRealm.Binary.cs:2280-2281 | `JSJSON.ToLength` BI/Json/JSJSON.cs:83; JSArrayPrototype.Iteration.cs:127; JSProxy.cs:914; BuiltInsAssemblyInitializer.cs:1479, :1494; JSTypedArray.cs:376, :469; JSIntl.cs:1321-1327 |
| ToIndex's range rule (7.1.22) | `JsRealm.BinaryToIndex` VM/JsRealm.Binary.cs:1903 (also serves asIntN, asUintN and Atomics) | `DataView.ToIndex` :173; `JSBigInt.ToBitsIndex` JSBigInt.cs:625; `DataView.ToByteOffset` :267; `JSAtomics.ToIndex` :94; `TypedArrayParameters.ToTypedArrayLength` :156; `JSArrayBuffer.ToBufferLengthLong` :94, `ToBufferLength` :78 |
| Float16 encode | `JsFloat16.Encode` VM/JsBinary.cs:660 (`QuietNaN` 0x7E00 :655) | `(Half)` casts JSFloat16Array.cs:32, DataView.cs:605, JSMath.cs:334 |
| Float16 decode | `JsFloat16.Decode` VM/JsBinary.cs:668 | `BitConverter.ToHalf` JSFloat16Array.cs:23; `BitConverter.UInt16BitsToHalf` DataView.cs:374 |
| Float16 round | `JsFloat16.Round` VM/JsBinary.cs:676 | `JSMath.F16round` BI/Objects/JSMath.cs:330-337 |
| Byte order | little-endian through `BinaryPrimitives` (VM/JsBinary.cs:134-142, :1298, :1317) | typed arrays through `BitConverter` (host order); `DataView` through `BinaryPrimitives` (DataView.cs:207-240) |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| D1 | the bytes of `new Float16Array([NaN])` | NumericToRawBytes: a NaN's encoding is the implementation's | `00 7E` | the sign and high payload kept (`00 FE` on x64 for `double.NaN`) | implementation-defined: a parameter |
| D2 | `Math.f16round(NaN)` seen through a `Float64Array` | the same | `7FF8...` | the sign kept | implementation-defined |
| D3 | `new Uint8Array(new Uint16Array([1]).buffer)[0]` on a big-endian host | `[[LittleEndian]]` is implementation-defined | 1 | 0 | engine policy, outside the kernel |
| J1 | `Atomics.add(i32, 0, 2**53)` on an element 1; `Atomics.or` the same | AtomicReadModifyWrite through ToInt32 | element stays 1 | element 0 (`ToIntegerOperand` saturates, `ApplyInt` :175 sums through a double) | Broiler.JS wrong |
| J2 | `Atomics.compareExchange(i32, 0, 2**63, 5)`; `Atomics.wait(..., 2**63, 0)` | ToInt32(2^63) is 0 | stores 5; `"timed-out"` | stores nothing; `"not-equal"` (JSAtomics.cs:237-243, :321) | Broiler.JS wrong |
| J3 | `new Uint8Array(3).fill(1, 0, Infinity)` | ToIntegerOrInfinity | `[1,1,1]` | `[0,0,0]` (`AsInt32OrDefault`, JSTypedArray.prototype.cs:163-164) | Broiler.JS wrong |
| J4 | `new Float64Array(new ArrayBuffer(16), 1, {valueOf(){ throw ... }})` | the offset's RangeError before ToIndex(length) | RangeError | the length's error (TypedArrayParameters.cs:115-116, JSTypedArray.cs:259) | Broiler.JS wrong |
| J5 | `Uint8Array.from.call(C, {length: 2**32})` | ToLength gives 2^32 | 2^32 | 2147483647 (JSTypedArray.cs:469, :392-394) | Broiler.JS wrong |
| J6 | `new Date(2020, 0, 1).setMinutes(2**32 + 1)` | MakeTime takes the integer | no wrap | wraps mod 2^32 (`CoercedIntValue`) | Broiler.JS wrong |
| J7 | `"abc".endsWith("c", undefined)` | an undefined position is the length | `true` | `false` (`IntegerValue` of undefined, JSStringPrototype.Search.cs:50) | Broiler.JS wrong |
| — | ToInt32, ToUint32 on −0, NaN, ±∞, 2^31, 2^32, 4294967295.5, 2^53, −2^63, 1e21; the clamp at 255.5, 254.5, 2.5, 1.5, 0.5, −0.5, NaN, ±∞; ToIndex at −0.5, −1, 2^53 − 1, 2^53, ±∞, NaN; Float16 rounding at 65520, 65519.99, 2^-25, −2^-26 and subnormals | | | | no difference |

---

## 2. Math kernels

`SL/` has no Math kernel yet. On .NET 10.0.12, `double.ExpM1` is `Math.Exp(x) - 1` and `double.LogP1`
is `Math.Log(x + 1)` (read from the runtime's IL).

| Function | Broiler.VM (VM/JsRealm.Math.cs unless stated) | Broiler.JS (BI/Objects/JSMath.cs unless stated) |
|---|---|---|
| `round` | `MathRoundHalfUp` :239-262 | `Round` :48-79 |
| `**`, `pow` | `MathPower` :454-477; `Math.pow` :166; the `Exponent` opcode VM/JsEngine.Dispatch.cs:1363-1366 | `Pow` :491-511; `JSValue.Power` RT/JSValue.cs:1740-1762 (via BinaryOperation.cs:106, :170, JSValueBuilder.cs:112); `BExpression.Power` EX/Expressions/BExpression.cs:159-168, emitting `Math.Pow`, reached at CO/Expressions/FastCompiler.VisitBinaryExpression.cs:201 from every compiled route; `LinqConverter.VisitPower` EX/Converters/LinqConverter.VisitParameter.cs:99-109 |
| `hypot` | `MathHypotenuse` :360-417 | `Hypot(in Arguments)` :339-365; `Hypot(double, double)` :367-388 (no caller) |
| `cbrt` | inline snap :128-135 | `Cbrt` :200-206; `CorrectlyRoundedCbrt` :213-231 |
| `expm1` | `MathExpMinusOne` :202-203 | `Expm1` :291-313 |
| `log1p` | `MathLogOnePlus` :215-216 | `Log1p` :422-440 |
| `sumPrecise` | `MathSumPrecise` :532-627 (scaling at 2^1021, :623-626); `MathDistil` :632-708; iteration VM/JsRealm.Collections.cs:1405-1450 | `SumPrecise` :625-791; `TwoSum` :608-613; `SameNonFinite` :616; constants :603-605 |
| `fround`, `clz32`, `imul` | inline :144; `MathLeadingZeros` :482-483; `MathIntegerProduct` :429-434 | `Fround` :315-323; `Clz32` :252-260; `Imul` :390-399 |
| `max`, `min`, `sign` | :267-345 | :452-533 |
| `random` | xorshift per realm :69-84, :497-505 | one static `System.Random` :15, :45-46; `RandomNumber` :17 has no caller |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| a | `1 ** NaN` | NaN (Number::exponentiate step 1) | NaN | 1, every path | Broiler.JS wrong |
| b | `(-1) ** Infinity` written literally | NaN | NaN | NaN (the generic path, RT/JSValue.cs:1755-1759) | no difference |
| b′ | `(-1) ** (1/0)`; `var a = -1; a ** Infinity` in a function | NaN | NaN | 1 (the emitted `Math.Pow`) | Broiler.JS wrong |
| c | `Math.hypot(1, 3, 3)`; `Math.hypot(1, 4, 1)` | implementation-approximated | 4.358898943540673; 4.242640687119285 | 4.358898943540674; 4.242640687119286 | implementation-defined |
| d | `Math.cbrt(2)` | implementation-approximated | 1.2599210498948734 (the host's C library) | 1.2599210498948732 | implementation-defined |
| e | `Math.expm1(1e-300)`; `Math.expm1(1e-10)` | "accurate even when the value of x is close to 0" | 0; 1.000000082740371e-10 | 1e-300; 1.00000000005e-10 | Broiler.VM wrong |
| f | `Math.log1p(1e-300)`; `Math.log1p(0.012566334936294443)` | the same | 0; a result 64 units in the last place out | 1e-300; the same 64-unit result | Broiler.VM wrong near 0; both implementation-defined from 0.01 |
| g1 | `Math.sumPrecise([1e308, -1e308, 5e-324])` | the exact sum, 5e-324 | 0 | 5e-324 | Broiler.VM wrong |
| g2 | `Math.sumPrecise(Array(9).fill(2e307))`, and the same followed by nine `-2e307` | +∞; +0 | NaN; NaN | +∞; 0 | Broiler.VM wrong |
| g3 | `Math.sumPrecise({length: 1, 0: 5})` | TypeError (GetIterator) | 5 | TypeError | Broiler.VM wrong |
| g4 | `Math.sumPrecise("")`; `Math.sumPrecise("12")` | −0; TypeError | −0; TypeError | −0; TypeError | no difference |
| g5 | `Math.sumPrecise("")` with `String.prototype[@@iterator]` deleted | TypeError | −0 | TypeError | Broiler.VM wrong |
| g6 | `Math.sumPrecise(5)` with `Number.prototype[@@iterator]` defined | iterates | TypeError | iterates | Broiler.VM wrong |
| g7 | a getter on `@@iterator` | read once | read twice (:1410, VM/JsEngine.Iteration.cs:68) | once | Broiler.VM wrong |
| g8 | `Math.sumPrecise` in a default context | present (ES2026) | present | absent unless `JavaScriptFeatureFlags.MathSumPrecise` | Broiler.JS policy |
| h | `Math.random` | | per realm | one per process | engine policy |
| — | `round` over the edges and about 610,000 emulated values; `fround`, `f16round`, `clz32`, `imul` | | | | no difference |

**Provenance to record:** `JSMath.Clz32`'s comment cites Jurassic's `MathObject.cs`, and `SumPrecise`
ports the `Math.sumPrecise` proposal's reference algorithm; Broiler.JS's third-party notices name
neither. The library's `PreciseSum`, which takes the same algorithm, carries the proposal's notice.

---

## 3. Exact rounding

| Responsibility | Broiler.VM | Broiler.JS |
|---|---|---|
| The modes | `TemporalRounding` VM/JsTemporalCore.cs:48; Intl keeps a string, VM/JsNumberFormatter.cs:153 | strings; a private `UnsignedRoundingMode` in `JSIntlNumberFormat` BI/Intl/JSIntl.cs:5410 |
| Option text to mode | VM/JsTemporal.Options.cs:40, :136; VM/JsRealm.NumberFormat.cs:461-462 | `TemporalRoundingOptions.GetRoundingMode`, `NormalizeRoundingMode` JSTemporalRoundingOptions.cs:63, :70; `JSTemporalDuration.Arithmetic.NormalizeRoundingMode` :885; `JSIntl.ReadRoundingOptions` JSIntl.cs:2285-2293 |
| NegateRoundingMode | `JsTemporalCore.Negate` :502 | `TemporalRoundingOptions.NegateRoundingMode` :79 |
| GetUnsignedRoundingMode | `JsTemporalCore.UnsignedMode` :525 (enum `Unsigned` :513); `JsNumberFormatter.Unsigned` :860 with the top-level enum `JsUnsignedRounding` VM/JsDecimal.cs:44 | `TemporalRoundingOptions.UnsignedRoundingMode` :90; `JSTemporalZonedDateTime.Rounding` :408; `JSIntlNumberFormat.GetUnsignedRoundingMode` JSIntl.cs:5412 |
| ApplyUnsignedRoundingMode, one step | `JsTemporalCore.ApplyUnsigned` :545 | `TemporalRoundingOptions.ApplyRoundingPicksEnd` :106; `JSTemporalZonedDateTime.Rounding` :423; `JSIntlNumberFormat.RoundRatio` :5447 |
| ApplyUnsignedRoundingMode over a window | `JsTemporalCore.RoundsToSecond` :630 | inline: JSTemporalPlainDate.cs:1071-1074; JSTemporalZonedDateTime.Rounding.cs:212-214; JSTemporalDuration.Arithmetic.cs:729-731 (linear) |
| RoundNumberToIncrement | `JsTemporalCore.RoundToIncrement(x, inc, mode)` :603; `(num, den, inc, mode)` :590 | `TemporalRoundingOptions.RoundToIncrement` :188; `JSTemporalDuration.Arithmetic.RoundToIncrement` :893; `JSTemporalZonedDateTime.RoundOffsetToMinute` JSTemporalZonedDateTime.cs:1465 |
| RoundNumberToIncrementAsIfPositive | `JsTemporalCore.RoundToIncrementAsIfPositive` :609 | `TemporalRoundingOptions.RoundToIncrementAsIfPositive` :221 |
| Intl ToRawFixed, ToRawPrecision | `JsDecimal.RoundToUnits` VM/JsDecimal.cs:406 with :474-542; `JsRawDecimal.ToRawFixed` :559, `ToRawPrecision` :593 | `JSIntlNumberFormat.RoundFixed` :5474, `RawPrecisionDigits` :5499, `ToRawPrecision` :5637; `Math.Round` away from zero at :5190 and :5360 |
| A rational to the nearest double | `JsTemporalCore.ToDouble(num, den)` :677; `ToDouble(BigInteger)` :716 | `JSTemporalDuration.RatioToDouble` JSTemporalDuration.Arithmetic.cs:875; `NearestDouble` :843; a double sum at JSTemporalZonedDateTime.Rounding.cs:219 |
| In the library | n/a | internal `ExactRounding` SL/ExactRounding.cs:34: an integer's nearest double (`ToDouble(ulong, long, bool)` :46, `ToDouble(UInt128)` :83) |

| # | Input | Specification | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|---|---|
| 1 | `new Intl.NumberFormat("en", {maximumFractionDigits: 1}).format(1.15)` | ToIntlMathematicalValue takes Number::toString's value: `"1.2"` | `"1.2"` | `"1.1"` (the exact binary value, JSIntl.cs:5431) | Broiler.JS wrong, SJL-9 |
| 2 | `{notation: "scientific", maximumFractionDigits: 1, roundingMode: "floor"}.format(9.99)` | `"9.9E0"` | `"9.9E0"` | `"0.9E1"` (:5190) | Broiler.JS wrong |
| 3 | `{notation: "compact", roundingMode: "floor"}.format(1990)` | `"1.9K"` | `"1.9K"` | `"2K"` (:5360) | Broiler.JS wrong |
| 4 | `{notation: "scientific", maximumSignificantDigits: 21}.format(123456789012345678901n)` | the exact digits | exact | through a double (:4817, :4866) | Broiler.JS wrong, SJL-9 |
| 5 | `new Intl.PluralRules("en", {maximumFractionDigits: 0}).select(1.4)`; `{minimumFractionDigits: 1}.select(1)` | `"one"`; `"other"` | `"one"`; `"other"` | `"other"`; `"one"` (`SelectCategory` :4658) | Broiler.JS wrong, SJL-9 |
| 6 | `Temporal.PlainTime.from("01:10").round({smallestUnit: "minute", roundingIncrement: 20, roundingMode: "halfEven"})` | `01:00:00` | `01:00:00` | `01:20:00` (JSTemporalPlainTime.cs:294; also JSTemporalPlainDateTime.cs:563, JSTemporalZonedDateTime.cs:659) | Broiler.JS wrong, SJL-10 |
| 7 | `Temporal.Duration.from("P1M").round({smallestUnit: "months", largestUnit: "months", roundingIncrement: 2, roundingMode: "halfTrunc", relativeTo: "2023-01-01"})` | `P2M` | `P2M` | `PT0S` (Arithmetic.cs:724-731) | Broiler.JS wrong |
| 8 | `Duration.from({years: 5, seconds: 26368, nanoseconds: 123456789}).total({unit: "years", relativeTo: ZonedDateTime.from("2020-01-01T00:00[UTC]")})` | `5.000836127709817` | the same | `5.0008361277098174` (Rounding.cs:219) | Broiler.JS wrong |
| 9 | a rational to a double: `(5·2^125 + 1) / 2^1200` | `1.5e-323` | `1e-323` (rounds twice) | `0` | both wrong, latent |
| 10 | a rational to a double: `2^800 / 1` | `6.67e240` | correct | `Infinity` (`RatioToDouble` shifts by 256) | Broiler.JS wrong, latent |
| 11 | `Duration.from("-PT1M30S").round({smallestUnit: "minute", roundingMode: ...})`, each half mode | per the table | agrees | agrees | no difference |
| 12 | `Instant.prototype.since` with `roundingMode: "floor"` | NegateRoundingMode | negates the mode | rounds the negated difference with the mode | no observable difference |
| 13 | increments that do not divide; values beyond 2^53 | RangeError; exact | agrees | agrees | no difference |

**Consumers of the Temporal copies that an adapter alone does not correct (Broiler.JS, SJL-10):**
ZonedDateTime calendar-unit rounding on 24-hour days (JSTemporalZonedDateTime.cs:1092-1120); non-ISO
calendars skipping difference rounding (JSTemporalPlainDate.cs:339-342, JSTemporalZonedDateTime.cs:1082-1087);
`NudgeToCalendarUnit` returning the end where the window shifted without rounding up
(JSTemporalZonedDateTime.Rounding.cs:221-224).

---

## 4. The BigInt kernel

| Responsibility | Broiler.VM (VM/JsBigInt.cs unless stated) | Broiler.JS (BI/BigInt/JSBigInt.cs unless stated) |
|---|---|---|
| Width policy | ctor :81-89; `MaximumBits = 1L << 20` :75; `Bounded` :353; `JsHostRealm.AdmitBigInt` VM/JsHostRealm.cs:2289-2295 | none; a guard in `Shift` :470-471 |
| Meter | `LinearCost` :319; `ProductCost` :339-341; `LeafCost` :347; the engine's charge VM/JsEngine.cs:401, VM/JsEngine.Metering.cs:155-164 | none |
| Add, subtract, increment, decrement | `Add` :358; `Subtract` :367; dispatch VM/JsEngine.Operators.cs:119-150; `NumericStep` :165-178 | `AddValue` :518-542; `Subtract` :504; `Increment`, `Decrement` :433, :435 |
| Multiply, divide, remainder | `Multiply` :395-412; `Divide` :421; `Remainder` :433; the zero check Operators.cs:131 | `Multiply` :494; `Divide` :496-502; `Modulo` :506-512 |
| Exponentiate | `Power` :452-533; the negative-exponent check Operators.cs:132 | `Power` :439-446 (`(int)exponent`) |
| Negate, bitwise not, and, or, xor | `Negate` :376; `BitwiseNot` :385; :538-560 | `Negate` :431; `BitwiseNot` :437; :448-452 |
| Shifts | `ShiftLeft` :570; `ShiftRight` :579; `Shift` :586-619; `>>>`'s TypeError Operators.cs:133 | `LeftShift` :458; `RightShift` :460; `Shift` :462-481; `UnsignedRightShift` :488-492 |
| asIntN, asUintN | `AsIntN` :633-658; `AsUintN` :672-695; VM/JsRealm.BigInt.cs:151-161 | `AsIntN` :675-735; `AsUintN` :738-760; `ToBitsIndex` :625-636; `JSAtomics.TruncateToBigElement` :136-142; `JSAtomics.ApplyBig` :186-195 |
| Text out | `ToDecimalString` :147-182; `AppendDecimal` :198-234; `ToRadixString` :1041-1089 and its helpers :1094-1203; VM/JsRealm.BigInt.cs:99-116 | `ToString` :290; `ToDetailString` :292; concatenation :516, :532, :535; `JSToString` :560-579; `BigIntegerToRadixString` :581-605 |
| StringToBigInt | `TryParseStringInteger` :829-892; `TryParseRadix` :905-959; `ParseDecimal` :974-995; `ParseDecimalPart` :1001-1023; Conversions.cs:233-241; `JsDecimal.FromString` VM/JsDecimal.cs:234, :251-259 | `TryStringToBigInt` :177-186; `IsStrWhiteSpaceOnly` :189; `TryParseBigIntString` :191-224; `TryParsePrefixedDigits` :226-259 |
| Number and BigInt | `FromIntegralNumber` :734; `ToNumber` :748-772; Conversions.cs:250-258, :308 | `NumberToBigInt` :144-154; `ToNumber` :69-110; `EqualsLiteral(double)` :393; `BigIntegerExtensions.FromDouble` RT/BigIntegerExtensions.cs:14-39 (no caller) |
| Comparison | `CompareToNumber` :786-798; `OrderAgainstNumber` Operators.cs:323-327; `BigIntOrder` :274-305; `CompareBigInts` :310-314 | `JSBigIntExtensions.CompareToNumber` :30-47; `TryCompare` :320-344; `Less`...`GreaterOrEqual` :348-386; `Equals` :270-287; JSNumberPrototype.cs:39-109 |
| Property key | `ToPropertyKeyValue` Conversions.cs:362-371; `JsParser.BigIntKey` VMC/JsParser.cs:6342-6356 | `ToKey` :400; the compiler's keys CO/Declarations/FastCompiler.CreateClass.cs:23-24, :62-70 |
| Literal value | `JsParser.BigIntValueOf` :6310-6320 (the shared `NumericLiterals`); `JsCompiler.Emission.BigIntConstant` :153-166; the format's cap `CeilingBigIntConstantBytes` VMF/JsFormat.cs:766-775; `FromConstant` :120-124 | the constructor :160-171; the scanner's form check; literal text re-parsed at run time |

**Broiler.VM's charges, which the kernel keeps.** Let `B` be a value's bit length and
`W = ceil(B/64) + 1` its size in words. `L(w) = w + 1`; `P(a, b) = floor(max(a,1)·max(b,1)/16) + 32(a+b)`;
`Leaf(w) = floor(w²/2) + 16w + 1`.

- add, subtract, and, or, xor: `L(max(Wa, Wb))`; negate, not: `L(W)`; increment and decrement through
  add and subtract with `1n`.
- multiply: 1 when an operand is zero; `L(max(Wa, Wb))` then over-width when `Ba + Bb − 2` exceeds the
  ceiling; otherwise `P(Wa, Wb)`. Divide, remainder: `P(Wa, Wb)`.
- power: `L(max(Wbase, Wexp))` first; nothing more for a trivial base, a zero exponent or a refusal;
  then `P` of the result's and the square's words per multiplication and `P(w, w)` per squaring.
- shifts: 1 for zero; a left shift proven too wide `L(W)` then over-width; a left shift
  `L(words(B + count))`; a right shift `L(W)`.
- asIntN: `L(W)`; asUintN: `L(W)`, then `L(words(bits))` when there is no early answer.
- to a Number: `L(W)`; compared with a Number: `L(max(W, 17))`.
- decimal text: `L(floor(B·30103/100000) + 2)`; then `Leaf(W)` up to 4,096 bits, otherwise `P(w, w)` per
  squaring of the power, `P` of the part's and the power's words per split and `Leaf` per leaf. Other
  radixes: `L(floor(B/log2 r) + 2)`, nothing more for a power of two, otherwise as decimal.
- parse: `L(floor(len/8))` first; a prefixed text `L(words(digits·bitsPerDigit))`; a decimal text beyond
  1,024 digits `P(65, 65)` for the power, then `P(w, w)` per squaring, `Leaf(words(4·digits))` per leaf,
  `P` of the high part's and the power's words per join.
- charged by the engine, outside `JsBigInt`, and kept there: a comparison between two BigInts
  `min(Wa, Wb) + 1`; a typed-array element wider than 64 bits `L(W)`; a host crossing `L(W)` out and
  `L(words(B))` in; a clone `L(W)`; `JsDecimal.FromString` `floor(len/8) + 1`.

| Input | Broiler.VM | Broiler.JS | Class |
|---|---|---|---|
| `String(-1n)` under a culture whose minus sign is U+2212 | `"-1"` | `"−1"` (no culture passed, :290, :584) | Broiler.JS wrong |
| `2n ** 2000000000n`; `BigInt("9".repeat(400000))`; `(2n ** 1000000n).toString(3)`; a 10^6-digit hexadecimal string | RangeError at once; RangeError; metered | unlimited and uninterruptible | engine policy: a parameter |
| `1n ** 2n ** 31n`; `0n ** 2n ** 31n` | `1n`; `0n` | a .NET overflow | Broiler.JS wrong |
| `1n << 2147483647n` | RangeError | a .NET exception | Broiler.JS wrong (SJL-D-15 asks for a RangeError) |
| `BigInt.asUintN(2 ** 32, -1n)`; `asUintN(2 ** 32 + 3, -1n)`; `asUintN(2 ** 31, -1n)` | RangeError (ceiling) | `0n`; `7n`; divide by zero (:754) | Broiler.JS wrong |
| `2n ** 53n + 1n == 2 ** 53 + 1`; `2n ** 1024n == 1e400`, compiled against a literal | `false`; `false` | `true`; `true` (`EqualsLiteral(double)` :393 from CO/Expressions/FastCompiler.VisitBinaryExpression.cs:42-43, :108) | Broiler.JS wrong |
| `({})[2n ** 40n]`; `({})[-1n]`; `o[4294967295n]` | keys `"1099511627776"`, `"-1"`, `"4294967295"` | a .NET overflow; the same; a numeric key apart from the string's | Broiler.JS wrong |
| a Number from an object's `valueOf` written to a `BigInt64Array`; `ToBigInt`'s hint | not read (the specification: TypeError; the number hint) | accepted (JSBigInt64Array.cs:60); the default hint (:649) | Broiler.JS wrong |
| `-0n`; `2n ** 64n`; `asUintN(2 ** 53, -1n)`; `asIntN(0, x)`; `BigInt("  12 ﻿")`, `" 0x10 "`, `""`, `"-0x10"`; `1n < 1.5`, `-1n < -0.5`, `1n < NaN`, `2n ** 1100n < Infinity`; `Number(2n ** 1024n)`, `Number(2n ** 53n + 1n)` | | | no difference |

**The library already holds:** `NumericLiterals` (a BigInt literal's value, with a `LengthLimit`, and
Broiler.VM's power-of-two parse), `NumberParsing.TrimStrWhiteSpace` (public), `PrefixRadix` and
`DigitValue` (internal), and `ExactRounding.ToDouble(ulong, long, bool)` (internal), the ties-to-even
core both engines' `ToNumber` repeat.
