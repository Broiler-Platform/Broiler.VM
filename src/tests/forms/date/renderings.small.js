// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// RENDERINGS - the Date string renderings: toString, toDateString, toTimeString, toUTCString,
// toISOString and toJSON, and String of a Date, at instants across the representable range - negative
// years, years of one to six digits and the range's ends - each rendering read back by its length and
// a few of its code units. No instant is invalid, so nothing raises.
// A fuel-parity probe of milestone SJL-6 (README beside it). Nothing here is timed.

var n = 0;
var ends = [-8.64e15, 8.64e15, -62198755200000, -62135596800000, 0, 253402300799999, 253402300800000];

for (var i = 0; i < 90; i++) {
  var t = i < ends.length ? ends[i] : (i - 45) * 191889331234567 + i * 7919;
  var d = new Date(t);
  var a = d.toString();
  var b = d.toDateString();
  var c = d.toTimeString();
  var u = d.toUTCString();
  var s = d.toISOString();
  var j = d.toJSON();
  n += a.length * 3 + b.length * 5 + c.length * 7 + u.length * 11 + s.length * 13 + j.length;
  n += a.charCodeAt(a.length - 2) + b.charCodeAt(4) + c.charCodeAt(0) + u.charCodeAt(u.length - 5);
  n += s.charCodeAt(s.length - 9) + String(d).length;
}

n;
