// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-SHIFT - left and right shifts of multi-word values of both signs, mixed with and, or, xor and
// not, and a left shift the width ceiling refuses.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var x = (1n << 3000n) - 12345n, y = -(1n << 1500n) + 678n, s = 0n, refused = 0;

for (var k = 0n; k < 300n; k += 7n) {
  s ^= (x >> k) & (y << (k % 64n));
  s |= ~(x ^ (y >> 3n)) & 0xffffffffn;
  s += (x << k) >> (k + 1n);
}

try { 1n << (1n << 22n); } catch (e) { if (e instanceof RangeError) refused++; }

String(s % 1000000007n) + ":" + refused;
