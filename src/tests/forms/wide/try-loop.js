// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TRY-LOOP - an exception region entered and left every iteration, with nothing thrown.
//
// A region's handler offset is a place emitted code must be able to enter at, whatever it does
// between instructions. This shape pays for that on every iteration WITHOUT throwing, so it prices
// the region itself rather than the unwind; `throw-loop` beside it prices the unwind.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

var t = 0;

for (var i = 0; i < 4000000; i++) {
  try {
    t = t + i;
  } finally {
    t = t % 1000003;
  }
}

t;
