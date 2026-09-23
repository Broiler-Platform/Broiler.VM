// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER SYMBOL-KEYED PROPERTIES ON NON-EXTENSIBLE, SEALED AND FROZEN OBJECTS.
//
// Retained from JSeal slices V02 and V03: a Symbol key obeys the same extensibility and
// descriptor-integrity rules as a String key. Every case was compared against the comparison
// engine and the specification before it was written down.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function has(o, k) { return Object.getOwnPropertySymbols(o).indexOf(k) >= 0; }
function d(o, k) { var x = Object.getOwnPropertyDescriptor(o, k); return x === undefined ? "none" : ("get" in x) ? "acc:" + typeof x.get + typeof x.set + x.enumerable + x.configurable : String(x.value) + x.writable + x.enumerable + x.configurable; }

// --- V02: a non-extensible object never gains a Symbol key
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); o[s] = 1; return has(o, s); });
p(function () { "use strict"; var s = Symbol("s"); var o = Object.preventExtensions({}); o[s] = 1; return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.seal({}); o[s] = 1; return has(o, s); });
p(function () { "use strict"; var s = Symbol("s"); var o = Object.seal({}); o[s] = 1; return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.freeze({}); o[s] = 1; return has(o, s); });
p(function () { "use strict"; var s = Symbol("s"); var o = Object.freeze({}); o[s] = 1; return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); Object.defineProperty(o, s, {value: 1}); return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); try { Object.defineProperty(o, s, {value: 1}); } catch (e) {} return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); return Reflect.defineProperty(o, s, {value: 1}) + "/" + has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); return Reflect.set(o, s, 1) + "/" + has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); Object.defineProperties(o, {[s]: {value: 1}}); return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); try { Object.defineProperties(o, {[s]: {value: 1}}); } catch (e) {} return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); var q = new Proxy(o, {}); return Reflect.defineProperty(q, s, {value: 1}) + "/" + has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); var q = new Proxy(o, {}); q[s] = 1; return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); o.__defineGetter__(s, function () { return 1; }); return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); try { o.__defineGetter__(s, function () { return 1; }); } catch (e) {} return has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); Object.assign(o, {[s]: 1}); return has(o, s); });
p(function () { var s = Symbol("s"); class B { constructor() { return Object.preventExtensions({}); } } class C extends B { [s] = 1; } new C(); return "made"; });

// --- V02: existing writable Symbol-keyed properties stay writable
p(function () { var s = Symbol("s"); var o = {[s]: 1}; Object.preventExtensions(o); o[s] = 2; return o[s]; });
p(function () { "use strict"; var s = Symbol("s"); var o = {[s]: 1}; Object.seal(o); o[s] = 2; return o[s] + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {[s]: 1}; Object.seal(o); return Reflect.defineProperty(o, s, {value: 3}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {[s]: 1}; Object.preventExtensions(o); return Reflect.set(o, s, 5) + "/" + o[s]; });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, writable: true}); o[s] = 2; return d(o, s); });

// --- V03: frozen Symbol values cannot change
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); o[s] = 2; return o[s]; });
p(function () { "use strict"; var s = Symbol("s"); var o = Object.freeze({[s]: 1}); o[s] = 2; return o[s]; });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); Object.defineProperty(o, s, {value: 2}); return o[s]; });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); return Reflect.defineProperty(o, s, {value: 2}) + "/" + o[s]; });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); return Reflect.defineProperty(o, s, {writable: true}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); return d(o, s) + "/" + Object.isFrozen(o); });

// --- V03: a non-configurable Symbol-keyed property is not deleted or converted
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); return delete o[s]; });
p(function () { "use strict"; var s = Symbol("s"); var o = Object.seal({[s]: 1}); return delete o[s]; });
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); return Reflect.deleteProperty(o, s) + "/" + has(o, s); });
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); Object.defineProperty(o, s, {get: function () { return 2; }}); return d(o, s); });
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); return Reflect.defineProperty(o, s, {get: function () { return 2; }}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {get: function () { return 1; }}); return Reflect.defineProperty(o, s, {value: 2}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {get: function () { return 1; }}); return Reflect.defineProperty(o, s, {get: function () { return 2; }}) + "/" + o[s]; });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1}); return Reflect.defineProperty(o, s, {enumerable: true}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1}); return Reflect.defineProperty(o, s, {configurable: true}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, configurable: true}); Object.defineProperty(o, s, {get: function () { return 7; }}); return d(o, s) + "/" + o[s]; });

// --- V03: valid SameValue redefinitions still succeed, and partial redefinitions merge
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); return Reflect.defineProperty(o, s, {value: 1}) + "/" + d(o, s); });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: NaN}); Object.defineProperty(o, s, {value: NaN, writable: false, enumerable: true, configurable: false}); return d(o, s); });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 0}); return Reflect.defineProperty(o, s, {value: -0}); });
p(function () { var s = Symbol("s"); var g = function () {}; var o = {}; Object.defineProperty(o, s, {get: g}); return Reflect.defineProperty(o, s, {get: g}); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, writable: true, enumerable: true, configurable: true}); Object.defineProperty(o, s, {value: 2}); return d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, writable: true}); Object.defineProperty(o, s, {writable: false}); return d(o, s) + "/" + Reflect.defineProperty(o, s, {writable: true}); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, writable: true, configurable: true}); Object.defineProperty(o, s, {value: 2}); return d(o, s); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1, enumerable: false, writable: true}); o[s] = 2; return d(o, s); });

// --- V03: freeze, seal and the predicates over Symbol-keyed properties
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); return d(o, s) + "/" + Object.isSealed(o) + "/" + Object.isFrozen(o); });
p(function () { var s = Symbol("s"); var o = {}; Object.defineProperty(o, s, {value: 1}); Object.preventExtensions(o); return Object.isFrozen(o); });
p(function () { var s = Symbol("s"); var o = {[s]: 1}; Object.preventExtensions(o); return Object.isSealed(o) + "/" + Object.isFrozen(o); });
p(function () { var s = Symbol("s"); var o = Object.freeze({get [s]() { return 1; }}); return d(o, s) + "/" + Object.isFrozen(o); });

// --- V03: a Proxy-mediated define keeps the invariant checks
p(function () { var s = Symbol("s"); var o = Object.preventExtensions({}); var q = new Proxy(o, {defineProperty: function () { return true; }}); return Reflect.defineProperty(q, s, {value: 1}) + "/" + has(o, s); });
p(function () { var s = Symbol("s"); var o = {}; var q = new Proxy(o, {defineProperty: function () { return true; }}); return Reflect.defineProperty(q, s, {value: 1, configurable: false}); });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); var q = new Proxy(o, {}); return Reflect.defineProperty(q, s, {value: 2}) + "/" + o[s]; });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); var q = new Proxy(o, {}); return Reflect.defineProperty(q, s, {value: 1}); });
p(function () { var s = Symbol("s"); var o = Object.seal({[s]: 1}); var q = new Proxy(o, {deleteProperty: function () { return true; }}); return Reflect.deleteProperty(q, s); });
p(function () { var s = Symbol("s"); var o = Object.freeze({[s]: 1}); var q = new Proxy(o, {}); q[s] = 2; return o[s]; });
p(function () { var s = Symbol("s"); var seen = []; var q = new Proxy({[s]: 1, get g() { return 1; }}, {defineProperty: function (t, k, x) { seen.push(JSON.stringify(x)); return Reflect.defineProperty(t, k, x); }}); Object.freeze(q); return seen.join("|") + "/" + Object.isFrozen(q); });
p(function () { var s = Symbol("s"); var seen = []; var q = new Proxy({[s]: 1}, {defineProperty: function (t, k, x) { seen.push(JSON.stringify(x)); return Reflect.defineProperty(t, k, x); }}); Object.seal(q); return seen.join("|") + "/" + Object.isSealed(q); });
p(function () { var s = Symbol("s"); var q = new Proxy({[s]: 1}, {defineProperty: function () { return false; }}); Object.freeze(q); return "frozen"; });

// --- Review follow-up: a class field defines through one [[DefineOwnProperty]], and a primitive
// base walks its wrapper's prototype for an inherited Symbol-keyed setter
p(function () { var s = Symbol("s"); var log = []; class B { constructor() { return new Proxy({}, {getOwnPropertyDescriptor: function (t, k) { log.push("gopd"); return Reflect.getOwnPropertyDescriptor(t, k); }, isExtensible: function (t) { log.push("isExt"); return Reflect.isExtensible(t); }, defineProperty: function (t, k, x) { log.push("dp"); return Reflect.defineProperty(t, k, x); }}); } } class C extends B { [s] = 1; } new C(); return log.join(","); });
p(function () { var log = []; class B { constructor() { return new Proxy({}, {getOwnPropertyDescriptor: function (t, k) { log.push("gopd"); return Reflect.getOwnPropertyDescriptor(t, k); }, isExtensible: function (t) { log.push("isExt"); return Reflect.isExtensible(t); }, defineProperty: function (t, k, x) { log.push("dp"); return Reflect.defineProperty(t, k, x); }}); } } class C extends B { x = 1; } new C(); return log.join(","); });
p(function () { var s = Symbol("s"); class B { constructor() { return new Proxy({}, {defineProperty: function () { return false; }}); } } class C extends B { [s] = 1; } new C(); return "made"; });
p(function () { "use strict"; var s = Symbol("s"); var log = []; Object.defineProperty(Number.prototype, s, {set: function (v) { log.push(typeof this, v); }, configurable: true}); try { (5)[s] = 1; } finally { delete Number.prototype[s]; } return log.join(","); });
p(function () { var s = Symbol("s"); var log = []; Object.defineProperty(String.prototype, s, {set: function (v) { log.push(typeof this, v); }, configurable: true}); try { "a"[s] = 2; } finally { delete String.prototype[s]; } return log.join(","); });
p(function () { "use strict"; var s = Symbol("s"); (5)[s] = 1; return "made"; });
p(function () { var s = Symbol("s"); (5)[s] = 1; return "silent"; });
