// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// PROPERTY-LOOP (SMALL TWIN) - reading and writing ordinary data properties, twice per iteration.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

var o = { a: 1, b: 2 };

for (var i = 0; i < 1000; i++) {
  o.a = o.a + o.b;
  o.b = o.a - o.b;
}

o.a % 1000003;
