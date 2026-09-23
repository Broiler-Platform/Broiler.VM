// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER WHAT A SCRIPT DECLARES AT THE GLOBAL SCOPE (JSeal slice V15-host,
// JSD-0024 section 15).
//
// A script's var and function declarations are non-configurable properties of the global object;
// its let, const and class declarations are bindings of the global lexical environment, which no
// property shows and which are in their dead zone from the top of the script to their initialiser.
// GlobalDeclarationInstantiation checks every declaration before any is created, and an eval that
// declares a var over a global lexical declaration is a SyntaxError. The cases across two scripts are
// the host-surface lane's and the pinned suite's global-code directories'; this file holds the ones a
// single script can show. Each case prints its own number so a divergence names a case.
var __n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function t(f) { try { return show(f()); } catch (e) { return e && e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function desc(name) {
  var d = Object.getOwnPropertyDescriptor(globalThis, name);
  return d === undefined ? "absent" : [typeof d.value, d.writable, d.enumerable, d.configurable].join("/");
}

// --- a lexical declaration is in its dead zone until its initialiser runs
p(function () { return sdLate; });

function sdFn() { return 1; }
var sdVar = 2;
let sdLet = 3;
const sdConst = 4;
class sdClass {}

// --- var and function declarations are non-configurable global properties
p(function () { return desc("sdFn"); });
p(function () { return desc("sdVar"); });

// --- lexical declarations are bindings, not properties
p(function () { return [typeof globalThis.sdLet, typeof globalThis.sdConst, typeof globalThis.sdClass].join("/"); });
p(function () { return [sdLet, sdConst, typeof sdClass].join("/"); });

// --- an eval's var over a script's lexical declaration is a SyntaxError, and creates nothing
__n++;
try { eval("var sdLet;"); print(__n + " admitted"); } catch (e) { print(__n + " " + e.name); }
p(function () { return (0, eval)("var sdFresh, sdConst;"); });
p(function () { return typeof sdFresh; });

// --- an eval's own lexical declaration shadows the script's, and does not replace it
p(function () { return (0, eval)("let sdLet = 30; sdLet"); });
p(function () { return sdLet; });

// --- an eval's function over the script's function: writable and enumerable, so admitted
p(function () { return (0, eval)("function sdFn() { return 10; } sdFn()"); });
p(function () { return desc("sdFn"); });

let sdLate = 5;
