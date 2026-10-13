// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BIGINT-TEXT - a BigInt's text at radix 10 on both sides of the divide-and-conquer threshold, at a
// power-of-two radix and at others, and StringToBigInt back: decimal beyond a thousand digits,
// prefixed forms, a sign and the specification's white space.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var big = 7n ** 3000n, small = 3n ** 500n;

var s1 = big.toString(), s2 = small.toString(), s3 = big.toString(16), s4 = small.toString(36);
var s5 = (-big).toString(7);

var p1 = BigInt(s1), p2 = BigInt("  0x" + s3 + " "), p3 = BigInt("0o" + small.toString(8));
var p4 = BigInt("-" + s2), p5 = BigInt("1".repeat(1500));

[p1 === big, p2 === big, p3 === small, p4 === -small, s1.length, s4.length, s5.length,
  String(p5 % 1000003n)].join(",");
