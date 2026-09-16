// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// PROPERTY-LOOP - reading and writing ordinary data properties, twice per iteration.
//
// Property access is what most programs are mostly made of, and it is also the instruction that
// re-enters guest code WHEN AND ONLY WHEN an operand is exotic or carries an accessor. This object
// has neither: two data properties on a plain object, no accessor and no Proxy. So the shape prices
// the ordinary case, which is the case a granularity decision turns on.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

var o = { a: 1, b: 2 };

for (var i = 0; i < 4000000; i++) {
  o.a = o.a + o.b;
  o.b = o.a - o.b;
}

o.a % 1000003;
