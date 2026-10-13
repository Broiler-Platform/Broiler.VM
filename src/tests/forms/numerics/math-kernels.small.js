// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// MATH-KERNELS - Math.round, the ** operator and Math.pow with their special cases, Math.hypot,
// Math.cbrt, Math.expm1, Math.log1p, Math.fround, Math.f16round, Math.clz32 and Math.imul. Every
// input is one whose answer the specification fixes exactly, or is compared within a tolerance, so
// the answer does not depend on how a kernel approximates.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var t = 0, k = 0;

for (var i = 0; i < 400; i++) {
  var x = i - 200;
  t += Math.round(x / 8) + Math.round(-x / 4 + 0.5);
  t += Math.hypot(3 * x, 4 * x) + Math.hypot(x, 2 * x, 2 * x);
  t += Math.cbrt(x * x * x);
  t += x ** 2 + Math.pow(x, 3) / (x || 1);
  t += Math.fround(x / 3) === x / 3 ? 1 : 0;
  t += Math.clz32(x) + Math.imul(x, 0x7fffffff) % 1000;
  t += Math.f16round(x / 64) * 64;
  if (Math.abs(Math.expm1(x / 64) - (Math.exp(x / 64) - 1)) < 1e-9) k++;
  if (Math.abs(Math.log1p(i / 64) - Math.log(1 + i / 64)) < 1e-9) k++;
}

t += (1 ** Infinity !== 1) + ((-1) ** Infinity !== 1) + NaN ** 0;
t += (0 ** -1 === Infinity) + ((-0) ** -3 === -Infinity);

t + ":" + k;
