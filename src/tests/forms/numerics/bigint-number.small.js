// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-NUMBER - a BigInt's conversion to a Number, a Number's to a BigInt, and the comparisons
// between the two, beyond 2^53 and against the infinities.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var n = 0, c = 0, big = 2n ** 1100n;

for (var i = 0n; i < 200n; i++) {
  var v = (i * 12345678901234567891n) ** 3n;
  n += Number(v % (2n ** 60n)) / 2 ** 60;
  if (v > Number(i) * 1e57) c++;
  if (i * 3n == Number(i) * 3) c++;
  if (BigInt(Number(i)) === i) c++;
}

c += (big > Infinity ? 0 : 1) + (big < 1e308 ? 0 : 1) + (Number(big) === Infinity ? 1 : 0);
c += (2n ** 53n + 1n == 2 ** 53 + 1) ? 0 : 1;

Math.round(n * 1e6) + ":" + c;
