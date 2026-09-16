// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// STRING-LOOP - text, whose charge grows with its input, and a branch per iteration.
//
// Concatenation and a length read are charged per character rather than per instruction, so this is
// the one shape whose per-iteration cost is not dominated by the dispatch. It is here as A CONTROL
// ON THE SET: a form that moved every shape by the same proportion would be moving something other
// than the dispatch, and this is where that would show. The string is reset before it can grow
// without bound, so the charge per iteration is bounded and the loop is flat.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

var s = '';

for (var i = 0; i < 4000000; i++) {
  s = s + 'x';

  if (s.length > 64) {
    s = '';
  }
}

s.length;
