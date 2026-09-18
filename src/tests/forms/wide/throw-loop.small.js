// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// THROW-LOOP (SMALL TWIN) - a throw caught every iteration, across a call boundary.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

function thrower(i) {
  throw i;
}

var c = 0;

for (var i = 0; i < 1000; i++) {
  try {
    thrower(i);
  } catch (e) {
    c = c + 1;
  }
}

c;
