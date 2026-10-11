# Area: numerics: integer conversions, Float16, Math kernels, exact rounding and BigInt

**Delivered by:** [SJL-4](../milestones/sjl-4-numerics.md). **Library:**
`Broiler.ECMAScript.Algorithms`. **Status:** design. Observations marked *(reading)* are to be
confirmed by vectors.

---

## 1. Scope and sub-areas

| Sub-area | Responsibilities |
|---|---|
| **Integer conversions and Float16** | `ToInt32`, `ToUint32`, `ToInt16`, `ToUint16`, `ToInt8`, `ToUint8`, `ToUint8Clamp`, `ToBigInt64`/`ToBigUint64` bit patterns, `ToIntegerOrInfinity`, `ToLength`, `ToIndex`'s range rule; Float16 encode, decode and round |
| **Math kernels** | Only where an engine has custom logic: `round`, `**`/`pow` special cases, `hypot`, `cbrt`, `expm1`, `log1p`, `sumPrecise`, `fround`, `f16round`, `clz32`, `imul` |
| **Exact rounding** | The rounding-mode enumeration, mode negation, unsigned rounding modes, `RoundToIncrement` over exact rationals, correctly rounded rational-to-double. Consumed by Intl number formatting (SJL-9) and Temporal (SJL-10). |
| **BigInt kernel** | Arithmetic, shifts and bitwise operations under a width policy with a meter; text in both directions at every radix; `StringToBigInt`; `asIntN` and `asUintN`; exact BigInt-to-Number conversion; comparison against Number |

**Not shared:** the trigonometric, hyperbolic, exponential and logarithmic functions that both
engines take straight from `System.Math`, because there is no duplicated algorithm to remove.
`Math.random`'s state and seeding are engine policy. Operator dispatch and value storage stay with
each engine.

---

## 2. Current state

### Integer conversions and Float16

| | Broiler.VM | Broiler.JS |
|---|---|---|
| `ToInt32`, `ToUint32`, `ToIntegerOrInfinity` | `JsValue.ToInt32`, `ToUint32`, `ToInteger` (−0 becomes +0) | `JSValue.ToUint32`, `IntValue`; public `JSNumericOperators.ToInt32`/`ToUint32`; a non-specification saturating `IntegerValue` used by several string methods |
| Element narrowing, clamping | `JsBinary` `JsElements.Write`, `Clamp` (round half to even after bounds), `Low64`, `Narrow` | inline in `JSUint8ClampedArray.SetValue`; masks in `JSBigInt64Array` and `DataView` |
| `ToIndex`, `ToLength` | `JsRealm.Binary.BinaryToIndex` | duplicated in `DataView.ToIndex`, `JSBigInt.ToBitsIndex`; `JSJSON.ToLength` |
| Float16 | `JsBinary` `JsFloat16`: `Encode` (writes one canonical NaN), `Decode`, `Round` | inline `(Half)` casts in `JSMath.F16round`, `DataView` get and set, `JSFloat16Array`; NaN payloads not normalised |
| Typed-array byte order | explicit little-endian | typed arrays follow the host's byte order through `BitConverter`; `DataView` uses `BinaryPrimitives` |

No observable difference in the conversions was found by reading. The NaN bytes Float16 writes may
differ, and **the canonical-NaN choice becomes a parameter**.

### Math kernels

| Function | Broiler.VM (`JsRealm.Math`) | Broiler.JS (`BuiltIns/Objects/JSMath`) |
|---|---|---|
| `round` | `MathRoundHalfUp` | floor plus fraction |
| `**` and `pow` | `MathPower`, special cases for NaN exponent, zero exponent, and ±1 with infinite exponent; used by `**` too | `JSMath.pow` has the cases, **but `**` does not use it**: the generic `JSValue.Power` lacks the NaN-exponent case, and the numeric fast path emits `Math.Pow` directly |
| `hypot` | one pass scaled by the largest value | pairwise |
| `cbrt` | snaps when the rounded root cubes back exactly | picks the best of three neighbours |
| `expm1`, `log1p` | `double.ExpM1`/`LogP1` with a −0 fix; its remark claims precision near zero *(the claim is to be checked: if the runtime computes `Exp(x) - 1`, it does not hold)* | a Kahan-style correction; a Taylor series near zero |
| `sumPrecise` | Shewchuk's algorithm with a scaling step when the largest term is huge | a port of the TC39 reference algorithm with an overflow partial |
| `random` | per-realm xorshift seeded from a cryptographic source | one static `System.Random` |

| Input | Specification | Broiler.VM | Broiler.JS |
|---|---|---|---|
| `1 ** NaN`, `(-1) ** Infinity` | `NaN` | `NaN` | `1` *(reading)* |
| `Math.sumPrecise([1e308, -1e308, 5e-324])` | `5e-324` | `0`: the scaling flushes the tiny term *(reading)* | `5e-324` |
| `Math.sumPrecise("")` | to be settled by a vector citing the clause | accepted as an iterable | accepted |

### Exact rounding

Broiler.VM holds two copies:

- `JsTemporalCore`: `Negate`, `UnsignedMode`, `ApplyUnsigned`, `RoundToIncrement`,
  `RoundToIncrementAsIfPositive`, `ToDouble(num, den)`;
- `JsDecimal.JsUnsignedRounding`, with its mapping in `JsNumberFormatter`.

Broiler.JS holds four:

- `JSTemporalRoundingOptions`;
- `JSTemporalZonedDateTime.Rounding`;
- `JSTemporalDuration.Arithmetic.RoundToIncrement`;
- `JSIntl`'s unsigned rounding and `RoundRatio`.

### BigInt

| | Broiler.VM (`JsBigInt`) | Broiler.JS (`BuiltIns/BigInt/JSBigInt`) |
|---|---|---|
| Representation | wraps `BigInteger`; a fixed maximum width | wraps `BigInteger`; no width limit |
| Metering | **every operation takes `Action<ulong> charge` and returns `null` past the ceiling**; the engine maps `null` to `RangeError` | none; `2n ** 2000000000n` cannot be interrupted *(reading)* |
| Text out | divide-and-conquer, culture-invariant | radix 10 through `BigInteger.ToString()` (current culture); other radixes by repeated division and insertion |
| Text in | `TryParseStringInteger`: the JavaScript whitespace set, radix-checked, over-width reported | `TryParseBigIntString` uses .NET `Trim` |
| `asIntN`, `asUintN` | exact, ceiling-checked | `AsUintN` shifts `1 << (int)bits`, so very large bit counts wrap *(reading)* |
| Number conversion, comparison | top bits plus sticky bit, ties to even; floor-based comparison | correctly rounded `ToNumber`; comparison by truncation; `EqualsLiteral(double)` truncates |
| Property keys | correct | `(uint)value` raises a .NET overflow beyond `uint` *(reading)* |
| Compile-time literal width | a separate cap in the format | none |

Broiler.VM's remark on `JsBigInt` states the design this area adopts: "The charge goes to a
delegate so that the arithmetic holds no engine and can be judged on its own; the engine passes its
fuel charge, which is also where cancellation is polled, and turns a null into the RangeError the
language gives a result it cannot represent."

---

## 3. The target

### API sketch

```csharp
namespace Broiler.ECMAScript.Numbers;

public static class IntegerConversions
{
    public static int ToInt32(double x);        public static uint ToUint32(double x);
    public static short ToInt16(double x);      public static ushort ToUint16(double x);
    public static sbyte ToInt8(double x);       public static byte ToUint8(double x);
    public static byte ToUint8Clamp(double x);  public static double ToIntegerOrInfinity(double x);
    public static double ToLength(double integer);
    public static bool IsValidIndex(double integer, out ulong index);   // ToIndex's range rule
    public static ulong ToBigUint64Bits(BigInteger value);  public static long ToBigInt64Bits(BigInteger value);
}

public static class Float16
{
    public static ushort Encode(double value, Float16Nan nanPolicy); // Canonical | PreservePayload
    public static double Decode(ushort bits);
    public static double Round(double value);
}

public static class MathKernels
{
    public static double Round(double x);   public static double Power(double b, double e);
    public static double Hypot(ReadOnlySpan<double> values);
    public static double Cbrt(double x);    public static double Expm1(double x);   public static double Log1p(double x);
    public struct PreciseSum { public void Add(double v); public double Result(); }  // engine iterates and closes
}

public enum RoundingMode { Ceil, Floor, Expand, Trunc, HalfCeil, HalfFloor, HalfExpand, HalfTrunc, HalfEven }
public static class ExactRounding
{
    public static RoundingMode Negate(RoundingMode mode);
    public static BigInteger RoundToIncrement(BigInteger numerator, BigInteger denominator, BigInteger increment, RoundingMode mode);
    public static double ToDouble(BigInteger numerator, BigInteger denominator);   // correctly rounded
}

public readonly record struct BigIntPolicy(long MaximumBits);
public static class BigIntMath
{
    public static BigIntStatus Add(BigInteger a, BigInteger b, in BigIntPolicy p, WorkCharge? charge, out BigInteger r);
    // …Subtract, Multiply, Divide, Remainder, Power, shifts, bitwise, Negate, BitwiseNot
    public static BigIntStatus AsIntN(long bits, BigInteger v, in BigIntPolicy p, WorkCharge? charge, out BigInteger r);
    public static BigIntStatus AsUintN(long bits, BigInteger v, in BigIntPolicy p, WorkCharge? charge, out BigInteger r);
    public static string ToString(BigInteger v, int radix, WorkCharge? charge);
    public static BigIntParseStatus TryParse(ReadOnlySpan<char> text, in BigIntPolicy p, WorkCharge? charge, out BigInteger v);
    public static double ToNumber(BigInteger v);                    // exact, ties to even
    public static int CompareToNumber(BigInteger v, double d);     // exact; NaN handled by caller
}
```

**Divide-by-zero and negative-exponent errors are the engine's**, raised before the call. The
specification names them, and the error types are engine types. An over-width status is mapped by
each engine. Broiler.VM maps it to its `RangeError`. Broiler.JS passes the unbounded policy of
decision [SJL-D-15](../decisions.md#sjl-d-15) (a), and maps the status to `RangeError` at the
representation's limit, never to a .NET exception.

### What adapters may contain

- operator dispatch and type mixing (`TypeError` when mixing BigInt and Number);
- argument conversion order;
- the engine's errors;
- value wrapping;
- iterator handling for `sumPrecise`;
- `Math.random`'s state;
- Broiler.VM's verifier-constant decoding for BigInt literals (`FromConstant`).

---

## 4. Deletion lists

**Broiler.VM:**

- `JsValue.ToInt32(double)`, `ToUint32(double)`, `ToInteger(double)`;
- `JsElements.Clamp`, `Low64` (and `Narrow`'s arithmetic);
- `JsFloat16`;
- `JsRealm.Math`'s `MathRoundHalfUp`, `MathHypotenuse` and `MathPower`, the `cbrt` snap and the
  `sumPrecise` core;
- `JsTemporalCore`'s rounding members;
- `JsDecimal.JsUnsignedRounding` and `JsNumberFormatter`'s mapping;
- the arithmetic, text and parse bodies of `JsBigInt`. The class remains as a value wrapper.

**Broiler.JS:**

- the body of `JSValue.ToUint32(double)`;
- the inline clamp, masks and `(Half)` casts;
- `JSMath`'s kernels;
- `JSValue.Power`'s arithmetic, and the direct `Math.Pow` emission for `**`;
- the rounding copies;
- `JSBigInt.ToNumber`, `TryStringToBigInt`, `TryParseBigIntString`, `TryParsePrefixedDigits`,
  `BigIntegerToRadixString`, `Shift`, and the `AsIntN`/`AsUintN` bodies;
- `JSBigIntExtensions.CompareToNumber`;
- the duplicated `ToIndex` and `ToLength` helpers.

**Public members kept as forwarders:** `JSNumericOperators.ToInt32` and `ToUint32`.

---

## 5. Risks

| Risk | Mitigation |
|---|---|
| Broiler.VM's fuel verdicts move because the shared BigInt kernel charges in different units | Charging units are specified per operation from Broiler.VM's current charges. The fuel-parity twins and exhaustion verdicts are a gate clause. Any change is admitted only by a decision, with its rows listed. |
| Broiler.JS hot paths slow down because the kernel is metered | The no-op meter, and Broiler.JS's own measurement under its rule. If the delegate costs too much for one operation, a later decision may move that operation to `SJL-D-10`'s (b), as the decision taken on 2026-10-11 allows. |
| `**` behaviour change in Broiler.JS's compiled fast path | Listed correction, with a test262 path and a vector |
