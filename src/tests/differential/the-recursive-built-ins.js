// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE BUILT-INS THAT RECURSE INTO WHAT THEY ARE GIVEN.
//
// `JSON.stringify` and `Array.prototype.join` descend into nested values, so a structure nested
// deeply enough exhausts whatever stack they descend on. What a program is owed is an exception it
// can catch, the same `RangeError` a guest-level recursion gets. The parity roadmap's section 4.9
// records that Broiler.JS has no such guard in its built-ins and terminates the process instead,
// which is why this probe stands alone: nothing after the first case would run there, and the
// declaration beside it is for the run and not for a case.

function t(f) {
  try {
    var v = f();
    return typeof v === "string" ? JSON.stringify(v) : String(v);
  } catch (e) {
    return e.name;
  }
}

var n = 0;
function p(f) { n++; print(n + " " + t(f)); }

function nestedArrays(depth) {
  var value = [];
  for (var i = 0; i < depth; i++) {
    value = [value];
  }
  return value;
}

function nestedObjects(depth) {
  var value = {};
  for (var i = 0; i < depth; i++) {
    value = { d: value };
  }
  return value;
}

p(function () { return JSON.stringify(nestedArrays(100000)).length; });
p(function () { return JSON.stringify(nestedObjects(100000)).length; });
p(function () { return nestedArrays(100000).join().length; });
