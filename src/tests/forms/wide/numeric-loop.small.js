// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// NUMERIC-LOOP (SMALL TWIN) - arithmetic, a comparison and a back edge, which is the least a loop can be.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

var t = 0;

for (var i = 0; i < 1000; i++) {
  t = (t + i * 3) % 1000003;
}

t;
