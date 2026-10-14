// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// SUM-PRECISE - Math.sumPrecise over arrays of mixed magnitudes and signs, none large enough for a
// partial to overflow and none tiny beside a cancelling pair.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var s = 0;

for (var i = 1; i <= 120; i++) {
  var a = [];
  for (var j = 0; j < 40; j++) {
    a.push(((i * 37 + j * 11) % 101 - 50) * 2 ** ((j % 9) * 3 - 12) + (j % 2 ? 0.1 : -0.1));
  }
  s += Math.sumPrecise(a);
}

s += Math.sumPrecise([1e20, 1, -1e20]) + Math.sumPrecise([]) + Math.sumPrecise([0.1, 0.2, 0.3]);

s;
