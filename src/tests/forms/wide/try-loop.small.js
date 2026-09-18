// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TRY-LOOP (SMALL TWIN) - an exception region entered and left every iteration, with nothing thrown.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

var t = 0;

for (var i = 0; i < 1000; i++) {
  try {
    t = t + i;
  } finally {
    t = t % 1000003;
  }
}

t;
