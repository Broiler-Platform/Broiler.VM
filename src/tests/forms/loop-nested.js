// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// LOOP-NESTED - an inner loop entered a thousand times, so the loop's own setup is measured too.
//
// `loop-integer.js` enters one loop once and runs it a million times; this one runs a million
// inner iterations spread over a thousand entries, which is the same amount of arithmetic with a
// thousand times as much loop entry and exit. Two kernels that differ in one thing is the only
// way to attribute anything, and this is that pair's second half.
//
// The answer is 249000750000.

function nested(n) {
  let acc = 0;
  let i = 0;

  while (i < n) {
    let j = 0;

    while (j < n) {
      acc = acc + i * j - j;
      j = j + 1;
    }

    i = i + 1;
  }

  return acc;
}

nested(1000);
