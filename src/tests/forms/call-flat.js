// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CALL-FLAT - a call per iteration, two arguments, one frame deep.
//
// `call-recursive.js` measures a call that nests a quarter of a million frames; this one measures
// a call that returns before the next one is made. They are separated because a form could plausibly
// differ on one and not the other - a deep stack is an allowance question as well as a speed one -
// and a single "calls" figure would hide which of the two it came from.
//
// The answer is 862500 - the accumulator wraps at a million, so the last wrap decides the digits.

function step(a, b) {
  return a + b * 0.5;
}

function drive(n) {
  let acc = 0.0;
  let i = 0;

  while (i < n) {
    acc = step(acc, i);

    if (acc > 1000000.0) {
      acc = acc - 1000000.0;
    }

    i = i + 1;
  }

  return acc;
}

drive(550000);
