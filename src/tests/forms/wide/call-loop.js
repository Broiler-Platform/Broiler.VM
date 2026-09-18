// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CALL-LOOP - one call per iteration, into a callee that does nothing.
//
// A call is where a nested activation is built, and it is the instruction a form is most likely to
// treat as a unit of its own. A body built around one is therefore a body of SHORT runs between
// dispatches, and this shape is what says what short runs cost. The callee is one addition, so
// nearly all of what either form spends here is entering it and leaving it.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

function f(x) {
  return x + 1;
}

var t = 0;

for (var i = 0; i < 4000000; i++) {
  t = f(t);
}

t;
