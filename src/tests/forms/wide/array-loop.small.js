// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// ARRAY-LOOP (SMALL TWIN) - an indexed read per inner iteration, over an array built once.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

var a = [];

for (var i = 0; i < 1000; i++) {
  a.push(i);
}

var t = 0;

for (var k = 0; k < 2; k++) {
  for (var j = 0; j < a.length; j++) {
    t = (t + a[j]) % 1000003;
  }
}

t;
