// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// THROW-LOOP - a throw caught every iteration, across a call boundary.
//
// This is the one path where control leaves an instruction for a HANDLER rather than for its
// successor, and it leaves a callee's frame to reach a caller's handler. Every form has to land
// there and carry on, so a form that is faster everywhere else and slower here has moved a cost
// rather than removed one.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

function thrower(i) {
  throw i;
}

var c = 0;

for (var i = 0; i < 3000000; i++) {
  try {
    thrower(i);
  } catch (e) {
    c = c + 1;
  }
}

c;
