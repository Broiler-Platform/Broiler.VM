// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// STRING-LOOP (SMALL TWIN) - text, whose charge grows with its input, and a branch per iteration.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

var s = '';

for (var i = 0; i < 1000; i++) {
  s = s + 'x';

  if (s.length > 64) {
    s = '';
  }
}

s.length;
