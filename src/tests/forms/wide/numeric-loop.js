// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// NUMERIC-LOOP - arithmetic, a comparison and a back edge, which is the least a loop can be.
//
// THE QUANTITY BEING MEASURED IS HOW OFTEN EMITTED CODE CALLS BACK INTO THE INTERPRETER'S DISPATCH,
// and almost every instruction of this body lies BETWEEN transfers: an add, a multiply, a remainder,
// a compare and an increment, with one back edge. A form that calls the dispatch less often has the
// most to gain here, and a form that gains nothing here has nothing to gain anywhere. That makes
// this shape the set's floor rather than its most interesting member.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

var t = 0;

for (var i = 0; i < 5000000; i++) {
  t = (t + i * 3) % 1000003;
}

t;
