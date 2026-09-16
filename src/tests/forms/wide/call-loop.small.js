// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CALL-LOOP (SMALL TWIN) - one call per iteration, into a callee that does nothing.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

function f(x) {
  return x + 1;
}

var t = 0;

for (var i = 0; i < 1000; i++) {
  t = f(t);
}

t;
