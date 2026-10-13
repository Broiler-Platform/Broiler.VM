// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-ARITHMETIC - add, subtract, multiply, divide, remainder, negate, increment and the bitwise
// operators on values of several thousand bits, each charged by the kernel before it computes.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var a = 1n, b = 3n, acc = 0n;
var m = 2n ** 4096n + 1n;

for (var i = 0; i < 200; i++) {
  a = a * 1000003n + BigInt(i);
  b = (b * b + a) % m;
  acc = (acc + a / (b + 1n) + a % 97n) ^ (b & 0xffffn);
  acc = (acc - (a >> 64n)) | 1n;
  acc = -(~acc);
  acc++;
  acc--;
}

String(acc % 1000000007n) + ":" + a.toString(16).length;
