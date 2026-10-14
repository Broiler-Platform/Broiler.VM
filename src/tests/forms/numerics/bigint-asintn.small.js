// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-ASINTN - BigInt.asIntN and BigInt.asUintN at widths across a value's size, with the early
// answers for a value that already fits.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var v = (1n << 200n) - 3n, w = -(7n ** 90n), s = 0n;

for (var b = 0; b <= 260; b += 13) {
  s += BigInt.asIntN(b, v) ^ BigInt.asUintN(b, w);
}

s += BigInt.asUintN(64, -1n) + BigInt.asIntN(64, 2n ** 63n) + BigInt.asUintN(1000, 5n);
s += BigInt.asIntN(2 ** 40, -5n);

String(s % 1000000007n);
