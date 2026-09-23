// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE ORDER IN WHICH BINARY OPERATORS COERCE THEIR OPERANDS.
//
// Retained for JSeal slice V01: every binary numeric operator converts its left operand before its
// right, a throw from the left operand's conversion means the right one is never converted, and the
// arithmetic itself still uses the operands in source order. Each case prints its own number so a
// divergence names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function pair(log, l, r) {
  return [{ valueOf: function () { log.push("a"); return l; } },
          { valueOf: function () { log.push("b"); return r; } }];
}
function order(op, l, r) {
  var log = []; var o = pair(log, l, r); var v = op(o[0], o[1]); return log.join("") + ":" + v;
}
function thrower(log) { return { valueOf: function () { log.push("a"); throw new RangeError("left"); } }; }
function leftThrows(op) {
  var log = []; var r = { valueOf: function () { log.push("b"); return 1; } };
  try { op(thrower(log), r); return "no throw"; } catch (e) { return e.name + ":" + log.join(""); }
}
function compound(op, l, r) {
  var log = []; var o = pair(log, l, r); var x = o[0]; x = op(x, o[1]); return log.join("") + ":" + x;
}

// --- the arithmetic operators convert left before right, and keep the operands' order
p(function () { return order(function (a, b) { return a - b; }, 7, 2); });
p(function () { return order(function (a, b) { return a * b; }, 7, 2); });
p(function () { return order(function (a, b) { return a / b; }, 7, 2); });
p(function () { return order(function (a, b) { return a % b; }, 7, 2); });
p(function () { return order(function (a, b) { return a ** b; }, 7, 2); });
p(function () { return order(function (a, b) { return a + b; }, 7, 2); });

// --- the bitwise and shift operators do the same
p(function () { return order(function (a, b) { return a & b; }, 6, 3); });
p(function () { return order(function (a, b) { return a | b; }, 6, 3); });
p(function () { return order(function (a, b) { return a ^ b; }, 6, 3); });
p(function () { return order(function (a, b) { return a << b; }, -6, 3); });
p(function () { return order(function (a, b) { return a >> b; }, -64, 3); });
p(function () { return order(function (a, b) { return a >>> b; }, -64, 3); });

// --- the relational operators all convert left before right
p(function () { return order(function (a, b) { return a < b; }, 1, 2); });
p(function () { return order(function (a, b) { return a <= b; }, 1, 2); });
p(function () { return order(function (a, b) { return a > b; }, 1, 2); });
p(function () { return order(function (a, b) { return a >= b; }, 1, 2); });

// --- a throw from the left operand's conversion leaves the right one unconverted
p(function () { return leftThrows(function (a, b) { return a - b; }); });
p(function () { return leftThrows(function (a, b) { return a * b; }); });
p(function () { return leftThrows(function (a, b) { return a / b; }); });
p(function () { return leftThrows(function (a, b) { return a % b; }); });
p(function () { return leftThrows(function (a, b) { return a ** b; }); });
p(function () { return leftThrows(function (a, b) { return a & b; }); });
p(function () { return leftThrows(function (a, b) { return a | b; }); });
p(function () { return leftThrows(function (a, b) { return a ^ b; }); });
p(function () { return leftThrows(function (a, b) { return a << b; }); });
p(function () { return leftThrows(function (a, b) { return a >> b; }); });
p(function () { return leftThrows(function (a, b) { return a >>> b; }); });

// --- compound assignment reads the target, then converts it before the right operand
p(function () { return compound(function (x, b) { x -= b; return x; }, 7, 2); });
p(function () { return compound(function (x, b) { x *= b; return x; }, 7, 2); });
p(function () { return compound(function (x, b) { x /= b; return x; }, 7, 2); });
p(function () { return compound(function (x, b) { x %= b; return x; }, 7, 2); });
p(function () { return compound(function (x, b) { x **= b; return x; }, 7, 2); });
p(function () { return compound(function (x, b) { x &= b; return x; }, 6, 3); });
p(function () { return compound(function (x, b) { x |= b; return x; }, 6, 3); });
p(function () { return compound(function (x, b) { x ^= b; return x; }, 6, 3); });
p(function () { return compound(function (x, b) { x <<= b; return x; }, -6, 3); });
p(function () { return compound(function (x, b) { x >>= b; return x; }, -64, 3); });
p(function () { return compound(function (x, b) { x >>>= b; return x; }, -64, 3); });
p(function () { var log = []; var o = pair(log, 7, 2); var box = { v: o[0] }; box.v -= o[1]; return log.join("") + ":" + box.v; });
p(function () { var log = []; var o = pair(log, 7, 2); var box = [o[0]]; box[0] **= o[1]; return log.join("") + ":" + box[0]; });

// --- Symbol.toPrimitive and toString take part in the same order, and so does the operand itself
p(function () {
  var log = [];
  var a = { [Symbol.toPrimitive]: function (h) { log.push("a" + h); return 9; } };
  var b = { toString: function () { log.push("b"); return "4"; }, valueOf: null };
  return log.join(",") + ":" + (a - b) + ":" + log.join(",");
});
p(function () { var log = []; var o = pair(log, 2, 10); return (o[0] ** o[1]) + ":" + log.join(""); });
p(function () { var log = []; var o = pair(log, 1, 0); return (o[0] / o[1]) + ":" + (o[1] - o[0]) + ":" + log.join(""); });

// --- the right operand's expression is still evaluated before either conversion
p(function () {
  var log = [];
  var a = { valueOf: function () { log.push("convA"); return 5; } };
  var v = a - (log.push("evalB"), { valueOf: function () { log.push("convB"); return 1; } });
  return log.join(",") + ":" + v;
});
