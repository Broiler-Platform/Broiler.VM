// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE MAPPED ARGUMENTS OBJECT (JSeal slices V05 and V06).
//
// A sloppy function with a simple parameter list gets an arguments object whose indices below the
// actual argument count alias the formal parameters, in both directions, until a deletion or a
// redefinition disconnects them. Strict functions and non-simple parameter lists get the unmapped
// object. Every case was compared against the comparison engine and the specification
// (CreateMappedArgumentsObject and the arguments exotic object's internal methods) before it was
// retained, and each prints its own number so a divergence names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- V05: the two directions of the alias
p(function () { function f(x) { arguments[0] = 9; return x; } return f(1); });
p(function () { function f(x) { x = 9; return arguments[0]; } return f(1); });
p(function () { function f(x, y) { arguments[1] = "b"; y += "!"; return x + ":" + y + ":" + arguments[1]; } return f("a", "z"); });
p(function () { function f(x) { x++; x++; return arguments[0]; } return f(1); });
p(function () { function f(a, b, c) { arguments[2] = 30; b = 20; return [a, b, c, arguments[0], arguments[1], arguments[2]].join(); } return f(1, 2, 3); });

// --- missing actuals are not mapped, extra actuals have no parameter
p(function () { function f(x, y) { y = 5; return String(arguments[1]) + ":" + arguments.length; } return f(1); });
p(function () { function f(x, y) { arguments[1] = 5; return String(y) + ":" + arguments.length; } return f(1); });
p(function () { function f(x) { arguments[1] = 5; return arguments[1] + ":" + x + ":" + arguments.length; } return f(1, 2); });
p(function () { function f(x) { arguments.length = 0; arguments[0] = 4; return x; } return f(1); });
p(function () { function f(x, y) { return Object.keys(arguments).join(); } return f(1); });

// --- duplicate formals: the last occurrence of a name is the one that maps
p(function () { function f(a, a) { return a; } return f(1, 2); });
p(function () { function f(a, a) { a = 9; return arguments[0] + ":" + arguments[1]; } return f(1, 2); });
p(function () { function f(a, a) { arguments[0] = 9; return a; } return f(1, 2); });
p(function () { function f(a, a) { arguments[1] = 9; return a; } return f(1, 2); });
p(function () { function f(a, a) { return String(a); } return f(1); });
p(function () { function f(a, b, a) { a = 7; return [arguments[0], arguments[1], arguments[2]].join(); } return f(1, 2, 3); });
p(function () { function f(a, a) { return f.length; } return f(); });

// --- strict functions, arrows and non-simple lists stay unmapped
p(function () { function f(x) { "use strict"; arguments[0] = 9; return x; } return f(1); });
p(function () { function f(x) { "use strict"; x = 9; return arguments[0]; } return f(1); });
p(function () { function f(x = 0) { arguments[0] = 9; return x; } return f(1); });
p(function () { function f(x = 0) { x = 9; return arguments[0]; } return f(1); });
p(function () { function f(x, ...r) { arguments[0] = 9; return x; } return f(1, 2); });
p(function () { function f(x, ...r) { x = 9; return arguments[0]; } return f(1, 2); });
p(function () { function f(x, {y}) { arguments[0] = 9; return x; } return f(1, {y: 2}); });
p(function () { function f(x, [y]) { x = 9; return arguments[0]; } return f(1, [2]); });
p(function () { function f(x) { var g = () => { arguments[0] = 9; return x; }; return g(); } return f(1); });
p(function () { class C { m(x) { arguments[0] = 9; return x; } } return new C().m(1); });

// --- closure visibility and callback reentry
p(function () { function f(x) { var get = function () { return x; }; arguments[0] = 9; return get(); } return f(1); });
p(function () { function f(x) { var set = function (v) { x = v; }; set(9); return arguments[0]; } return f(1); });
p(function () { function f(x) { [5].forEach(function (v) { arguments[0] = v; }); return x; } return f(1); });
p(function () { function f(x) { var a = arguments; [5].forEach(function (v) { a[0] = v; }); return x; } return f(1); });
p(function () { function f(x) { var a = arguments; [5].forEach(function (v) { x = v; }); return a[0]; } return f(1); });
p(function () { function f(x, n) { if (n > 0) { arguments[0] = x + 1; return f(x, n - 1) + ":" + x; } return x; } return f(1, 2); });
p(function () { function f(x) { return arguments; } var a = f(1); a[0] = 2; return a[0]; });
p(function () { function f(x) { return [arguments, function () { return x; }]; } var r = f(1); r[0][0] = 7; return r[1](); });
p(function () { function* g(x) { arguments[0] = 9; yield x; } return g(1).next().value; });

// --- V06: deletion disconnects
p(function () { function f(x) { delete arguments[0]; arguments[0] = 9; return x + ":" + arguments[0]; } return f(1); });
p(function () { function f(x) { delete arguments[0]; x = 9; return String(arguments[0]); } return f(1); });
p(function () { function f(x) { return delete arguments[0]; } return f(1); });
p(function () { function f(x) { delete arguments[0]; return (0 in arguments) + ":" + arguments.length; } return f(1); });

// --- V06: an accessor replacement disconnects, and a failed one does not
p(function () { function f(x) { Object.defineProperty(arguments, "0", { get: function () { return "g"; } }); x = 9; return arguments[0] + ":" + x; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { get: function () { return "g"; }, configurable: true }); arguments[0] = 5; return x; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { configurable: false }); try { Object.defineProperty(arguments, "0", { get: function () {} }); } catch (e) { x = 9; return e.name + ":" + arguments[0]; } } return f(1); });

// --- V06: redefinition with a value keeps the alias, writable:false sets and then disconnects
p(function () { function f(x) { Object.defineProperty(arguments, "0", { value: 2 }); return x + ":" + arguments[0]; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { value: 2 }); x = 3; return arguments[0]; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { value: 2, writable: false }); x = 3; return arguments[0] + ":" + x; } return f(1); });
p(function () { function f(x) { x = 5; Object.defineProperty(arguments, "0", { writable: false }); x = 3; return arguments[0] + ":" + x; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { writable: false }); arguments[0] = 7; return arguments[0] + ":" + x; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { enumerable: false }); x = 4; return arguments[0] + ":" + Object.keys(arguments).length; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { configurable: false }); x = 4; var d = Object.getOwnPropertyDescriptor(arguments, "0"); return d.value + ":" + d.writable + ":" + d.configurable; } return f(1); });
p(function () { function f(x) { Object.defineProperty(arguments, "0", { configurable: false }); arguments[0] = 6; return x + ":" + delete arguments[0]; } return f(1); });
p(function () { function f(x) { Object.freeze(arguments); x = 8; return arguments[0] + ":" + x; } return f(1); });
p(function () { function f(x) { x = 2; Object.freeze(arguments); return arguments[0] + ":" + Object.isFrozen(arguments); } return f(1); });

// --- V06: an invalid descriptor does not partially mutate the map
p(function () { function f(x) { Object.defineProperty(arguments, "0", { value: 2, writable: false, configurable: false }); try { Object.defineProperty(arguments, "0", { value: 3 }); } catch (e) { return e.name + ":" + arguments[0] + ":" + x; } } return f(1); });
p(function () { function f(x) { try { Object.defineProperty(arguments, "0", { value: 2, get: function () {} }); } catch (e) { x = 5; return e.name + ":" + arguments[0]; } } return f(1); });
p(function () { function f(x) { return Reflect.defineProperty(arguments, "0", { value: 2 }) + ":" + x; } return f(1); });
p(function () { function f(x) { Object.preventExtensions(arguments); x = 3; return arguments[0] + ":" + Reflect.defineProperty(arguments, "5", { value: 1 }); } return f(1); });

// --- the descriptor reports the live value, and other receivers are not aliased
p(function () { function f(x) { x = 9; var d = Object.getOwnPropertyDescriptor(arguments, "0"); return d.value + ":" + d.writable + ":" + d.enumerable + ":" + d.configurable; } return f(1); });
p(function () { function f(x) { x = 9; return JSON.stringify(Array.prototype.slice.call(arguments)); } return f(1, 2); });
p(function () { function f(x) { var o = Object.create(arguments); o[0] = 9; return x + ":" + o[0]; } return f(1); });
p(function () { function f(x) { Reflect.set(arguments, "0", 9, {}); return x; } return f(1); });
p(function () { function f(x) { Reflect.set(arguments, "0", 9); return x; } return f(1); });
p(function () { function f(x) { x = 4; return [...arguments].join(); } return f(1, 2); });
p(function () { function f(x) { return Object.prototype.toString.call(arguments); } return f(1); });
