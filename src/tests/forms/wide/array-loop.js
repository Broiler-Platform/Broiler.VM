// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// ARRAY-LOOP - an indexed read per inner iteration, over an array built once.
//
// The array is a thousand elements built by `push` and then only read, so the outer literal is a
// number of PASSES and the inner loop is the work. Indexed access on a dense array is the other half
// of what `property-loop` measures - the same re-entry question asked of an index rather than of a
// name - and the nested loop gives the set a body whose back edge is taken a thousand times per
// outer iteration.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

var a = [];

for (var i = 0; i < 1000; i++) {
  a.push(i);
}

var t = 0;

for (var k = 0; k < 4000; k++) {
  for (var j = 0; j < a.length; j++) {
    t = (t + a[j]) % 1000003;
  }
}

t;
