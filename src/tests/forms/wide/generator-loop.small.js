// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// GENERATOR-LOOP (SMALL TWIN) - `for`-`of` over a generator: the iterator protocol, and a suspension per element.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

function* g(n) {
  for (var i = 0; i < n; i++) {
    yield i;
  }
}

var t = 0;

for (var v of g(1000)) {
  t = (t + v) % 1000003;
}

t;
