// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// LOOP-INTEGER - the smallest loop this manifest can express, a million times.
//
// One comparison, one addition, one increment, one back edge. Every value stays an integer that a
// double holds exactly, so nothing here is about rounding: it is the per-iteration floor, and it
// is the kernel a difference between the forms should be largest on, because the loop body is
// almost entirely the machinery the forms differ in.
//
// The answer is 499999500000, which is 999999 * 1000000 / 2 and fits a double exactly.

function sum(n) {
  let total = 0;
  let i = 0;

  while (i < n) {
    total = total + i;
    i = i + 1;
  }

  return total;
}

sum(1000000);
