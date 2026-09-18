// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// GENERATOR-LOOP - `for`-`of` over a generator: the iterator protocol, and a suspension per element.
//
// Every element crosses the protocol - a `next` call into guest code, a yielded value, a resumption
// at the instruction after the yield - so this shape exercises the two things no other shape here
// does: entering guest code on an instruction's common path, and re-entering a function at a point
// that is not its start. A form that treats either differently shows here.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

function* g(n) {
  for (var i = 0; i < n; i++) {
    yield i;
  }
}

var t = 0;

for (var v of g(3000000)) {
  t = (t + v) % 1000003;
}

t;
