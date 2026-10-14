<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The numerics area's fuel-parity probes

These are the population that part B of bundle
[`sjl-4-001`](../../../../docs/evidence/sjl-4-001/decision-rule.md)'s rule adds to the wide shapes'
twins. Milestone SJL-4 of the shared JavaScript libraries programme moves this family's integer
conversions, Float16, Math kernels and BigInt arithmetic into `Broiler.ECMAScript.Algorithms`, and the
BigInt kernel there charges fuel. **The probes exist to show that it charges what this family charges
today**: each probe's smallest completing fuel allowance is bisected at the milestone's base commit,
and after the cut-over every probe must complete at that figure and refuse at one less, in both
output forms. **Nothing here is timed.** No figure from them is retained anywhere in this repository
except in bundle `sjl-4-001`.

They were written, and committed with the rule, before any code of SJL-4 existed.

## What each probe exercises

| Probe | What it charges |
|---|---|
| `bigint-arithmetic.small.js` | add, subtract, multiply, divide, remainder, negate, increment and decrement, and the bitwise operators on values of several thousand bits |
| `bigint-power.small.js` | exponentiation by squaring and its trivial bases, and two refusals: a result past the width ceiling and a negative exponent |
| `bigint-shift.small.js` | left and right shifts of multi-word values of both signs with the bitwise operators, and a left shift the ceiling refuses |
| `bigint-text.small.js` | a BigInt's text at radix 10 on both sides of the divide-and-conquer threshold and at other radixes, and StringToBigInt back, decimal beyond a thousand digits included |
| `bigint-asintn.small.js` | `BigInt.asIntN` and `BigInt.asUintN` across a value's size, with their early answers |
| `bigint-number.small.js` | a BigInt's conversion to a Number and back, and the comparisons between the two |
| `math-kernels.small.js` | `Math.round`, `**` and `Math.pow` with their special cases, `hypot`, `cbrt`, `expm1`, `log1p`, `fround`, `f16round`, `clz32` and `imul` |
| `sum-precise.small.js` | `Math.sumPrecise` over arrays of mixed magnitudes |
| `integer-conversions.small.js` | every typed array's element conversion, the clamp, the 64-bit bit patterns, Float16, `DataView`, Atomics' operands, and ToIntegerOrInfinity, ToLength and ToIndex through the built-ins that apply them |

## The rule they are written to

**Every probe's answer is one the cut-over does not change.** SJL-4 corrects some of this family's
results ([JSD-0067](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0067-numerics-move-to-the-shared-algorithms-library.md),
X2): `Math.expm1` and `Math.log1p` near zero, and `Math.sumPrecise` when a partial overflows or a tiny
term sits beside a cancelling pair. A probe that met one of those would change its answer as well as
its charge, and part B judges charges. So no probe uses such an input: the Math probe takes only
inputs whose result the specification fixes exactly, or compares within a tolerance, and the
`sumPrecise` probe keeps every partial finite. A probe whose answer changed after the cut-over would
be a defect of the probe or of the kernel, and part B reads it as a moved verdict.

Every probe is deterministic, takes its counts as literals, and ends in an expression statement whose
value is the answer. Each compiles under `broiler.javascript.wide`, the command-line root's manifest:
Temporal and Intl are outside it, so the area's exact rounding is judged through test262 (part A),
not here.
