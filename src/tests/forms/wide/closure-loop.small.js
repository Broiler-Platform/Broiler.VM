// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CLOSURE-LOOP (SMALL TWIN) - a call through a variable, into a function that reads a scope above it.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

function counter() {
  var n = 0;

  return function () {
    n = n + 1;
    return n;
  };
}

var c = counter();
var t = 0;

for (var i = 0; i < 1000; i++) {
  t = c();
}

t;
