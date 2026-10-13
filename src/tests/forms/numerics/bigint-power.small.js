// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-POWER - exponentiation by squaring, its trivial bases, and the refusals: a result past the
// width ceiling and a negative exponent, each a RangeError this probe catches and counts.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var r = 0n, refused = 0;

for (var e = 0n; e < 60n; e++) {
  r += (3n ** (e * 17n)) % 1000000007n;
  r += (-2n) ** e;
}

r += 1n ** (2n ** 40n) + 0n ** 5n + (-1n) ** (2n ** 40n + 1n);
r += ((2n ** 64n + 1n) ** 20n) % 1000000009n;

try { 2n ** (2n ** 21n); } catch (x) { if (x instanceof RangeError) refused++; }
try { 2n ** -1n; } catch (x) { if (x instanceof RangeError) refused++; }

String(r) + ":" + refused;
