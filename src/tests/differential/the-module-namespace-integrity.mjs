// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER A MODULE NAMESPACE'S [[DefineOwnProperty]] AND THE INTEGRITY LEVELS.
//
// Retained from JSeal slice VM-FIX-F: a namespace answers a definition by comparing it with the
// export as it stands and never writes; `Object.seal` succeeds and `Object.freeze` throws over one
// with exports; a namespace reached as the receiver of `Reflect.set` answers through the same rule.
// The module imports its own namespace, so `late` is still in its temporal dead zone while the
// cases run. Every case was compared against the comparison engine and the specification before it
// was written down.
import * as ns from "./the-module-namespace-integrity.mjs";

export var a = 1;
export function f() { return 0; }

var __n = 0;
function t(g) { try { var v = g(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(g) { __n++; print(__n + " " + t(g)); }

p(function () { return Reflect.defineProperty(ns, "a", {value: 1}); });
p(function () { return Reflect.defineProperty(ns, "a", {value: 2}) + "/" + ns.a; });
p(function () { return Reflect.defineProperty(ns, "a", {}); });
p(function () { return Reflect.defineProperty(ns, "a", {writable: true, enumerable: true, configurable: false}); });
p(function () { return Reflect.defineProperty(ns, "a", {configurable: true}); });
p(function () { return Reflect.defineProperty(ns, "a", {enumerable: false}); });
p(function () { return Reflect.defineProperty(ns, "a", {writable: false}); });
p(function () { return Reflect.defineProperty(ns, "a", {get: function () { return 1; }}); });
p(function () { return Reflect.defineProperty(ns, "zz", {value: 1}); });
p(function () { return Reflect.defineProperty(ns, "late", {value: 1}); });
p(function () { return Reflect.defineProperty(ns, Symbol.toStringTag, {value: "Module"}); });
p(function () { return Reflect.defineProperty(ns, Symbol.toStringTag, {value: "x"}); });
p(function () { Object.defineProperty(ns, "a", {value: 3}); return "defined"; });
p(function () { return Reflect.set({}, "a", 1, ns); });
p(function () { return Reflect.set({}, "a", 5, ns) + "/" + ns.a; });
p(function () { return Reflect.set({}, "zz", 1, ns); });
p(function () { Object.seal({}); Object.preventExtensions(ns); return Object.isExtensible(ns); });
p(function () { a = 1; return Object.isSealed(ns); });
p(function () { return Object.isFrozen(ns); });
p(function () { Object.freeze(ns); return "frozen"; });
p(function () { return Object.getOwnPropertyDescriptor(ns, "a").writable; });

export let late = 1;
