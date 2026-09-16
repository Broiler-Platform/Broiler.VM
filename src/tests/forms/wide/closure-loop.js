// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CLOSURE-LOOP - a call through a variable, into a function that reads a scope above it.
//
// `call-loop` calls a function by name; this one calls whatever a variable holds, into a body whose
// only state lives in an enclosing scope. The pair separates two things one call shape would
// confound: naming the callee, and reaching a binding that is not the frame's own.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

function counter() {
  var n = 0;

  return function () {
    n = n + 1;
    return n;
  };
}

var c = counter();
var t = 0;

for (var i = 0; i < 4000000; i++) {
  t = c();
}

t;
