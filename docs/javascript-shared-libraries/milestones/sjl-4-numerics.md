# Milestone SJL-4 — Numerics: integer conversions, Float16, Math kernels, exact rounding and BigInt

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/numerics.md)

- **Objective.** These operations come from `Broiler.ECMAScript.Algorithms` in both engines, and
  each engine's own copies are gone:
  - the specification's integer conversions;
  - Float16 encoding and rounding;
  - the Math kernels each engine implements itself;
  - the unsigned rounding modes with exact rational rounding, which SJL-9 and SJL-10 consume;
  - the BigInt kernel: arithmetic under a width policy and a meter, text conversion in both
    directions, `asIntN` and `asUintN`, and exact conversion and comparison against Number.
- **Owner roles.** The Broiler.JS maintainer (home, Broiler.JS cut-over) and the JavaScript profile
  owner (Broiler.VM cut-over). One person.
- **Repositories.** Broiler.JS, Broiler.VM.
- **Waits on.** SJL-3. It applies decision [SJL-D-15](../decisions.md#sjl-d-15), taken on
  2026-10-11: Broiler.JS passes an unbounded BigInt width policy.

---

## Work packages

### SJL-4.1 — Area verdict and comparison rules

The extraction gate is applied to each sub-area. The four sub-areas are integer conversions and
Float16; Math kernels; exact rounding; and the BigInt kernel. **A sub-area may be refused on its
own**, with its record, while the others proceed. The area's decision-rule files are committed
first.

*Done when* the verdicts and rules exist.

### SJL-4.2 — Vectors

The corpus gains:

- `ToInt32`, `ToUint32`, `ToInt16`, `ToUint16`, `ToInt8`, `ToUint8`, `ToUint8Clamp`, the
  `ToBigInt64` and `ToBigUint64` bit patterns, `ToIntegerOrInfinity`, `ToLength`, and `ToIndex`'s
  range rule;
- Float16 encode, decode and round, every NaN payload included. The canonical-NaN choice is a
  parameter, because Broiler.VM writes one canonical NaN and Broiler.JS does not normalise;
- Math, for each kernel an engine implements itself: `round`, `**` and `pow` with their special
  cases (`1 ** NaN`, `(-1) ** Infinity`), `hypot`, `cbrt`, `expm1`, `log1p`, `sumPrecise` (overflow
  partials and tiny terms included), `fround`, `f16round`, `clz32` and `imul`;
- unsigned rounding: every mode, increment and tie;
- BigInt:
  - arithmetic, shifts and bitwise operations at the policy ceiling and past it;
  - text at every radix;
  - `StringToBigInt` with the specification's whitespace;
  - `asIntN` and `asUintN` with very large bit counts;
  - Number conversion and comparison at 2^53 and beyond.

**Math functions that both engines take directly from `System.Math`** (the trigonometric and
logarithmic families) **are not shared kernels**: there is no duplicated algorithm to remove. They
get vectors only where an engine wraps them with a correction.

*Done when* the vectors are added with provenance, and both engines' current results are classified.

### SJL-4.3 — The implementation

The algorithms library gains:

- **Integer conversions and Float16.** Pure static functions.
- **Math kernels.** The rule is to share where an engine has custom logic:
  - `Power` holds the specification's special cases. **Both** of Broiler.JS's paths use it: its
    generic `JSValue.Power` and its numeric fast path, which emits `Math.Pow` directly;
  - `Round`, `Hypot` and `Cbrt`;
  - `Expm1` and `Log1p` with documented precision;
  - **`PreciseSum` as a streaming accumulator**, so each engine keeps iterating and closing the
    iterator itself, and correct where Broiler.VM's scaling flushes a tiny term today.
- **Exact rounding.** The rounding-mode enumeration, the negation of a mode for `since`, the
  unsigned modes, `RoundToIncrement` over exact rationals, and correctly rounded rational-to-double.
- **The BigInt kernel.**
  - Every operation takes a policy, the maximum bit width, and the meter. It charges before computing
    and returns an over-width status rather than a value past the ceiling. This is Broiler.VM's
    existing `JsBigInt` design, which its remark describes as "Every operation charges before it
    computes and answers null rather than a value past the ceiling".
  - Divide-and-conquer text conversion.
  - Culture-invariant formatting.
  - `StringToBigInt` with the specification's whitespace.
  - Exact Number conversion (ties to even) and comparison.

*Done when* vectors and reference arms pass under the area rule.

### SJL-4.4 — Broiler.VM's cut-over

**Deleted:**

- from `JsValue`: `ToInt32`, `ToUint32`, `ToInteger(double)`;
- from `JsElements`: `Clamp`, `Low64`;
- the class `JsFloat16`;
- the custom Math helpers in `JsRealm.Math`: `MathRoundHalfUp`, `MathHypotenuse`, `MathPower`, the
  `cbrt` snap, the `sumPrecise` core;
- `JsTemporalCore`'s rounding members: `Negate`, `UnsignedMode`, `ApplyUnsigned`,
  `RoundToIncrement`, `RoundToIncrementAsIfPositive`, `ToDouble(num, den)`;
- `JsDecimal`'s `JsUnsignedRounding`, together with its mapping in `JsNumberFormatter`;
- the arithmetic, text and parse bodies of `JsBigInt`.

**`JsBigInt` stays as a thin value wrapper** holding identity, `Words`, `FromConstant` (which
decodes the verifier's constant payload) and the policy constant. Engine-side mixing errors
(`TypeError`), zero-divisor and negative-exponent `RangeError`s, and the mapping of the over-width
status to `RangeError` stay in `JsEngine.Operators` and `JsEngine.Conversions`.

*Done when* the cut-over lands and rule `I5` holds the list.

### SJL-4.5 — Broiler.JS's cut-over

**Deleted:**

- `JSValue.ToUint32(double)`'s body;
- the inline clamp in `JSUint8ClampedArray`;
- the inline `(Half)` casts: `JSMath.F16round`, `DataView` get and set, `JSFloat16Array`;
- `JSMath`'s `round`, `cbrt`, `expm1`, `log1p`, `hypot`, `pow` and `sumPrecise` kernels;
- the special-case-less `JSValue.Power`, and the direct `Math.Pow` emission for `**`;
- the rounding copies: `JSTemporalRoundingOptions`, `JSTemporalZonedDateTime.Rounding`,
  `JSTemporalDuration.Arithmetic.RoundToIncrement`, and `JSIntl`'s unsigned rounding;
- `JSBigInt`'s `ToNumber`, `TryStringToBigInt`, `TryParseBigIntString`, `TryParsePrefixedDigits`,
  `BigIntegerToRadixString`, `Shift`, and the bodies of `AsIntN` and `AsUintN`;
- `JSBigIntExtensions.CompareToNumber`.

**Corrections**, each with its vector:

- `1 ** NaN`;
- `asUintN` with very large bit counts;
- BigInt property keys beyond `uint`, which raise a .NET overflow today;
- `EqualsLiteral` truncation;
- the culture-dependent BigInt text;
- the BigInt whitespace set.

**Public members:** `JSNumericOperators.ToInt32` and `ToUint32` stay as forwarders
([SJL-D-25](../decisions.md#sjl-d-25)). **BigInt width:** Broiler.JS passes the unbounded policy of
[SJL-D-15](../decisions.md#sjl-d-15) (a). The kernel's over-width status maps to `RangeError` at the
representation's limit, never to a .NET exception.

*Done when* the cut-over lands and the deletion search holds.

### SJL-4.6 — Evidence

As SJL-3.6, for this area. Broiler.VM's fuel-parity twins and exhaustion verdicts are a **required**
part: the BigInt kernel's charges must keep Broiler.VM's verdicts at every ceiling, or the difference
is admitted under a decision listed in the bundle.

*Done when* both bundles are retained.

---

## Exit gate

**Home clauses.**

1. Every sub-area that was not refused is implemented under the conventions.
2. Its vectors and reference arms pass under the area rule.
3. The `SL` rules, the API baseline and package validation pass, and the Native AOT probe runs.

**Broiler.JS clauses.**

4. The deletion list is absent and the public dispositions are recorded.
5. test262 per path meets the area rule.
6. Engine-level vectors pass.
7. Every correction is listed.

**Broiler.VM clauses.**

8. Rules `I1` to `I7` hold, and rule `I5` holds the deletion list.
9. Whole-suite test262 in both forms meets the area rule.
10. The fuel-parity twins and the exhaustion verdicts meet the area rule.
11. Every root is published and run in three modes, and its closure is declared.
12. Engine-level vectors pass.

**Records.** The area-verdict JSD and Broiler.JS record, any refused sub-area's record with source
pointers, and decision `SJL-D-15`.

---

## Evidence bundles

`docs/evidence/sjl-4-001/` in Broiler.JS and in Broiler.VM, in the shape of SJL-3's.

---

## Does not do

- **It does not change how either engine stores BigInts or Numbers**, or how operators dispatch.
- **It does not share the Intl mathematical value** (`JsDecimal`). That is SJL-9's, over the
  rounding this milestone shares.
- **It does not share `Math.random`.** Its state and seeding are engine policy.
