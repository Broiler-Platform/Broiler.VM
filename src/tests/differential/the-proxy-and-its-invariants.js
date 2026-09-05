// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER `Proxy`, AND OVER WHAT A PROXY MAKES THE REST OF THE REALM SAY.
//
// The thirteen traps, each forwarded and each trapped; the essential invariants, which are the
// part a proxy that merely relayed its handler would get wrong; revocation; and the places
// elsewhere in the language that have to notice a proxy - `typeof`, `instanceof`, `new`,
// `Array.isArray`, `JSON.stringify`, `for…in`, spread, the `Object` statics and `Reflect`.
//
// THE INVARIANT CASES ARE THE POINT AND THEY ARE WRITTEN AS REFUSALS. A handler that lies about a
// non-configurable, non-writable property is a `TypeError` and not a wrong value, so most of these
// cases print `TypeError` when the implementation is right - which is exactly the shape of case
// that a probe written from the implementation rather than from the language would never contain.
//
// Each case prints its own number, so a divergence names a case rather than a line, and the
// numbering survives a case being rewritten in place. A case that throws prints the error's name,
// because a refusal is an answer and a probe that stopped at the first one would compare nothing
// after it.

function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
var __n = 0;
function p(f) { __n++; print(__n + " " + t(f)); }

// --- the constructor itself
p(function () { return typeof Proxy; });
p(function () { return Proxy.length; });
p(function () { return Proxy.name; });
p(function () { return Object.getOwnPropertyDescriptor(Proxy, "prototype"); });
p(function () { return "prototype" in Proxy; });
p(function () { return Proxy({}, {}); });
p(function () { return new Proxy(1, {}); });
p(function () { return new Proxy({}, 1); });
p(function () { return new Proxy({}); });
p(function () { return Object.getPrototypeOf(Proxy) === Function.prototype; });
p(function () { return typeof Proxy.revocable; });
p(function () { return Proxy.revocable.length; });
p(function () { return Object.keys(Proxy.revocable({}, {})).join(","); });

// --- a handler with no traps forwards everything
p(function () { var o = { a: 1 }; return new Proxy(o, {}).a; });
p(function () { var o = { a: 1 }; var q = new Proxy(o, {}); q.b = 2; return o.b; });
p(function () { return "a" in new Proxy({ a: 1 }, {}); });
p(function () { return "toString" in new Proxy({}, {}); });
p(function () { var o = { a: 1 }; var q = new Proxy(o, {}); delete q.a; return "a" in o; });
p(function () { return Object.keys(new Proxy({ a: 1, b: 2 }, {})).join(","); });
p(function () { return Object.getPrototypeOf(new Proxy({}, {})) === Object.prototype; });
p(function () { return Object.isExtensible(new Proxy({}, {})); });
p(function () { return JSON.stringify(new Proxy({ a: 1 }, {})); });
p(function () { var s = ""; for (var k in new Proxy({ a: 1, b: 2 }, {})) { s += k; } return s; });

// --- each trap, once
p(function () { return new Proxy({}, { get: function (t, k) { return k + "!"; } }).x; });
p(function () { var seen; new Proxy({}, { set: function (t, k, v) { seen = k + "=" + v; return true; } }).x = 1; return seen; });
p(function () { return "anything" in new Proxy({}, { has: function () { return true; } }); });
p(function () { return delete new Proxy({}, { deleteProperty: function () { return false; } }).x; });
p(function () { return Object.getOwnPropertyNames(new Proxy({}, { ownKeys: function () { return ["a", "b"]; }, getOwnPropertyDescriptor: function () { return { value: 1, configurable: true }; } })).join(","); });
p(function () { return Object.getPrototypeOf(new Proxy({}, { getPrototypeOf: function () { return Array.prototype; } })) === Array.prototype; });
p(function () { return Reflect.setPrototypeOf(new Proxy({}, { setPrototypeOf: function () { return false; } }), null); });
p(function () { return Object.isExtensible(new Proxy({}, { isExtensible: function (t) { return Reflect.isExtensible(t); } })); });
p(function () { return Reflect.preventExtensions(new Proxy({}, { preventExtensions: function () { return false; } })); });
p(function () { return JSON.stringify(Object.getOwnPropertyDescriptor(new Proxy({}, { getOwnPropertyDescriptor: function () { return { value: 7, configurable: true }; } }), "x")); });
p(function () { return Reflect.defineProperty(new Proxy({}, { defineProperty: function () { return false; } }), "x", { value: 1 }); });
p(function () { return new Proxy(function () { return 1; }, { apply: function () { return 2; } })(); });
p(function () { return new (new Proxy(function () { }, { construct: function () { return { k: 3 }; } }))().k; });

// --- the trap's own arguments
p(function () { var seen; new Proxy({ z: 0 }, { get: function (tt, k, r) { seen = [typeof tt, k, r === q].join(","); return 0; } }); var q = new Proxy({ z: 0 }, { get: function (tt, k, r) { seen = [typeof tt, k, r === q].join(","); return 0; } }); q.z; return seen; });
p(function () { var seen; var h = { defineProperty: function (tt, k, d) { seen = Object.keys(d).join(","); return true; } }; Object.defineProperty(new Proxy({}, h), "x", { value: 1 }); return seen; });
p(function () { var seen; var h = { defineProperty: function (tt, k, d) { seen = Object.keys(d).length; return true; } }; Object.defineProperty(new Proxy({}, h), "x", { value: 1, writable: true, enumerable: true, configurable: true }); return seen; });
p(function () { var seen; var h = { apply: function (tt, self, args) { seen = args.join(",") + "|" + String(self); return 0; } }; new Proxy(function () { }, h).call("me", 1, 2); return seen; });
p(function () { var seen; var f = function () { }; var h = { construct: function (tt, args, nt) { seen = args.length + "|" + (nt === q); return {}; } }; var q = new Proxy(f, h); new q(1, 2, 3); return seen; });

// --- a trap that is present and not callable, and a trap that is null
p(function () { return new Proxy({}, { get: 1 }).x; });
p(function () { return new Proxy({ a: 5 }, { get: null }).a; });
p(function () { return new Proxy({ a: 5 }, { get: undefined }).a; });

// --- the essential invariants
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, writable: false, configurable: false }); return new Proxy(o, { get: function () { return 2; } }).x; });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, writable: false, configurable: false }); return new Proxy(o, { get: function () { return 1; } }).x; });
p(function () { var o = {}; Object.defineProperty(o, "x", { get: undefined, configurable: false }); return new Proxy(o, { get: function () { return 2; } }).x; });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, writable: false, configurable: false }); var q = new Proxy(o, { set: function () { return true; } }); q.x = 9; return "no throw"; });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, configurable: false }); return "x" in new Proxy(o, { has: function () { return false; } }); });
p(function () { var o = Object.preventExtensions({ y: 1 }); return "y" in new Proxy(o, { has: function () { return false; } }); });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, configurable: false }); return delete new Proxy(o, { deleteProperty: function () { return true; } }).x; });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, configurable: false }); return Object.getOwnPropertyDescriptor(new Proxy(o, { getOwnPropertyDescriptor: function () { } }), "x"); });
p(function () { return Object.getOwnPropertyDescriptor(new Proxy({}, { getOwnPropertyDescriptor: function () { return { value: 1, configurable: false }; } }), "x"); });
p(function () { return Object.getOwnPropertyDescriptor(new Proxy({}, { getOwnPropertyDescriptor: function () { return 1; } }), "x"); });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, writable: true, configurable: false }); return Object.getOwnPropertyDescriptor(new Proxy(o, { getOwnPropertyDescriptor: function () { return { value: 1, writable: false, configurable: false }; } }), "x"); });
p(function () { return Object.getOwnPropertyNames(new Proxy({}, { ownKeys: function () { return ["a", "a"]; } })); });
p(function () { return Object.getOwnPropertyNames(new Proxy({}, { ownKeys: function () { return [1]; } })); });
p(function () { return Object.getOwnPropertyNames(new Proxy({}, { ownKeys: function () { return "no"; } })); });
p(function () { var o = {}; Object.defineProperty(o, "x", { value: 1, configurable: false }); return Object.getOwnPropertyNames(new Proxy(o, { ownKeys: function () { return []; } })); });
p(function () { var o = Object.preventExtensions({ a: 1 }); return Object.getOwnPropertyNames(new Proxy(o, { ownKeys: function () { return ["a", "b"]; } })); });
p(function () { var o = Object.preventExtensions({ a: 1 }); return Object.getOwnPropertyNames(new Proxy(o, { ownKeys: function () { return ["a"]; } })).join(","); });
p(function () { return Object.isExtensible(new Proxy({}, { isExtensible: function () { return false; } })); });
p(function () { return Reflect.preventExtensions(new Proxy({}, { preventExtensions: function () { return true; } })); });
p(function () { var o = Object.preventExtensions({}); return Object.getPrototypeOf(new Proxy(o, { getPrototypeOf: function () { return Array.prototype; } })); });
p(function () { var o = Object.preventExtensions({}); return Object.getPrototypeOf(new Proxy(o, { getPrototypeOf: function () { return Object.prototype; } })) === Object.prototype; });
p(function () { return Object.getPrototypeOf(new Proxy({}, { getPrototypeOf: function () { return 1; } })); });
p(function () { var o = Object.preventExtensions({}); return Reflect.setPrototypeOf(new Proxy(o, { setPrototypeOf: function () { return true; } }), Array.prototype); });
p(function () { var o = Object.preventExtensions({}); return Reflect.defineProperty(new Proxy(o, { defineProperty: function () { return true; } }), "n", { value: 1 }); });
p(function () { return Reflect.defineProperty(new Proxy({}, { defineProperty: function () { return true; } }), "n", { value: 1, configurable: false }); });
p(function () { return new (new Proxy(function () { }, { construct: function () { return 1; } }))(); });

// --- revocation
p(function () { var r = Proxy.revocable({ a: 1 }, {}); return r.proxy.a; });
p(function () { var r = Proxy.revocable({ a: 1 }, {}); r.revoke(); return r.proxy.a; });
p(function () { var r = Proxy.revocable({}, {}); r.revoke(); r.revoke(); return "twice is fine"; });
p(function () { var r = Proxy.revocable({}, {}); r.revoke(); return typeof r.proxy; });
p(function () { var r = Proxy.revocable(function () { }, {}); r.revoke(); return typeof r.proxy; });
p(function () { var r = Proxy.revocable(function () { }, {}); r.revoke(); return r.proxy(); });
p(function () { var r = Proxy.revocable({}, {}); r.revoke(); return Object.keys(r.proxy); });
p(function () { var r = Proxy.revocable({}, {}); r.revoke(); return "x" in r.proxy; });
p(function () { var r = Proxy.revocable({}, {}); r.revoke(); return Object.isExtensible(r.proxy); });
p(function () { var r = Proxy.revocable([], {}); r.revoke(); return Array.isArray(r.proxy); });
p(function () { var r = Proxy.revocable({}, {}); return r.revoke.name; });
p(function () { var r = Proxy.revocable({}, {}); return r.revoke.length; });
p(function () { var r = Proxy.revocable({}, {}); return String(r.revoke()); });

// --- callable and constructible follow the target
p(function () { return typeof new Proxy(function () { }, {}); });
p(function () { return typeof new Proxy({}, {}); });
p(function () { return typeof new Proxy(class { }, {}); });
p(function () { return new Proxy(function (a, b) { return a + b; }, {})(1, 2); });
p(function () { return new Proxy(function (a, b) { return a + b; }, {}).call(null, 1, 2); });
p(function () { return new Proxy(function (a, b) { return a + b; }, {}).apply(null, [1, 2]); });
p(function () { function F() { this.k = 1; } return new (new Proxy(F, {}))().k; });
p(function () { function F() { } var q = new Proxy(F, {}); return new q() instanceof F; });
p(function () { function F() { } var q = new Proxy(F, {}); return new F() instanceof q; });
p(function () { return new (new Proxy(Array, {}))(3).length; });
p(function () { return new (new Proxy({}, {}))(); });
p(function () { return new Proxy({}, {})(); });
p(function () { return Reflect.construct(new Proxy(function () { }, {}), []) instanceof Object; });
p(function () { class B { } class D extends B { } return new (new Proxy(D, {}))() instanceof B; });

// --- what the rest of the realm says about a proxy
p(function () { return Array.isArray(new Proxy([], {})); });
p(function () { return Array.isArray(new Proxy(new Proxy([], {}), {})); });
p(function () { return Array.isArray(new Proxy({}, {})); });
p(function () { return Object.prototype.toString.call(new Proxy([], {})); });
p(function () { return JSON.stringify(new Proxy([1, 2], {})); });
p(function () { return JSON.stringify({ a: new Proxy({ b: 1 }, {}) }); });
p(function () { var o = { a: 1 }; return JSON.stringify(Object.assign({}, new Proxy(o, {}))); });
p(function () { return [].concat(new Proxy([1, 2], {})).length; });
p(function () { return Array.from(new Proxy([1, 2], {})).join(","); });
p(function () { return Object.entries(new Proxy({ a: 1 }, {})).map(function (e) { return e.join(":"); }).join(","); });
p(function () { return Object.getOwnPropertySymbols(new Proxy({}, { ownKeys: function () { return [Symbol.iterator]; }, getOwnPropertyDescriptor: function () { return { value: 1, configurable: true }; } })).length; });
p(function () { var s = Symbol("k"); var o = {}; o[s] = 1; return new Proxy(o, {})[s]; });
p(function () { var s = Symbol("k"); var seen; new Proxy({}, { get: function (t, k) { seen = typeof k; return 0; } })[s]; return seen; });
p(function () { return Reflect.ownKeys(new Proxy({ a: 1 }, {})).join(","); });
p(function () { var c = 0; var q = new Proxy({ a: 1 }, { ownKeys: function (t) { c++; return Reflect.ownKeys(t); }, getOwnPropertyDescriptor: function (t, k) { return Reflect.getOwnPropertyDescriptor(t, k); } }); Reflect.ownKeys(q); return c; });
p(function () { return Object.freeze(new Proxy({ a: 1 }, {})) && Object.isFrozen(new Proxy(Object.freeze({ a: 1 }), {})); });

// --- a proxy in the middle of a prototype chain
p(function () { var q = new Proxy({}, { get: function (t, k) { return k === "up" ? 9 : undefined; } }); var o = Object.create(q); return o.up; });
p(function () { var q = new Proxy({}, { has: function (t, k) { return k === "up"; } }); var o = Object.create(q); return "up" in o; });
p(function () { var q = new Proxy({}, { has: function () { return false; } }); var a = []; Object.setPrototypeOf(a, q); return 1 in a; });
p(function () { var seen; var q = new Proxy({}, { set: function (t, k, v, r) { seen = k + "=" + v; return true; } }); var o = Object.create(q); o.w = 4; return seen + "|" + Object.prototype.hasOwnProperty.call(o, "w"); });
p(function () { var q = new Proxy({}, { getPrototypeOf: function () { return Array.prototype; } }); var o = Object.create(q); return Object.getPrototypeOf(Object.getPrototypeOf(o)) === Array.prototype; });
p(function () { function F() { } var q = new Proxy({}, {}); F.prototype = q; return Object.create(q) instanceof F; });

// --- nesting and forwarding through a proxy target
p(function () { var inner = new Proxy({ a: 1 }, {}); return new Proxy(inner, {}).a; });
p(function () { var c = 0; var inner = new Proxy({}, { deleteProperty: function (t, k) { c++; return k === "yes"; } }); var outer = new Proxy(inner, { deleteProperty: undefined }); return String(delete outer.yes) + String(delete outer.no) + c; });
p(function () { var inner = new Proxy({}, { defineProperty: function (t, k) { return k === "ok"; } }); var outer = new Proxy(inner, { defineProperty: null }); return String(Reflect.defineProperty(outer, "ok", {})) + String(Reflect.defineProperty(outer, "no", {})); });
p(function () { var o = Object.preventExtensions({}); var outer = new Proxy(new Proxy(o, {}), { setPrototypeOf: undefined }); return Reflect.setPrototypeOf(outer, Array.prototype); });
p(function () { var o = Object.preventExtensions({}); var outer = new Proxy(new Proxy(o, {}), {}); return Reflect.defineProperty(outer, "n", { value: 1 }); });

// --- the exotic own properties a proxy target may have
p(function () { return Reflect.deleteProperty([], "length"); });
p(function () { return Reflect.deleteProperty(new String("str"), "length"); });
p(function () { return Reflect.deleteProperty(new String("str"), "0"); });
p(function () { return Reflect.deleteProperty(/x/g, "lastIndex"); });
p(function () { var r = /x/g; Object.defineProperty(r, "lastIndex", { writable: false }); return JSON.stringify(Object.getOwnPropertyDescriptor(r, "lastIndex")); });
p(function () { var r = /x/g; Object.defineProperty(r, "lastIndex", { writable: false }); return r.exec("x"); });
p(function () { var r = /x/; Object.defineProperty(r, "lastIndex", { writable: false }); return r.exec("x")[0]; });
p(function () { "use strict"; var a = Object.freeze([1]); return delete a[0]; });
p(function () { var a = Object.freeze([1]); return delete a[0]; });
p(function () { "use strict"; return delete [].length; });

// --- the own keys of BOTH kinds, which is what `[[OwnPropertyKeys]]` means
p(function () { var s = Symbol("k"); var o = {}; o[s] = 1; Object.freeze(o); return Object.isFrozen(o); });
p(function () { var s = Symbol("k"); var o = {}; o[s] = 1; Object.freeze(o); return JSON.stringify(Object.getOwnPropertyDescriptor(o, s)); });
p(function () { var s = Symbol("k"); var o = {}; o[s] = 1; Object.seal(o); o[s] = 2; return o[s]; });
p(function () { var s = Symbol("k"); var o = { a: 1 }; o[s] = 2; return Object.assign({}, o)[s]; });
p(function () { var s = Symbol("k"); var o = {}; o[s] = 2; return Object.getOwnPropertySymbols(Object.getOwnPropertyDescriptors(o)).length; });
p(function () { var s = Symbol("k"); var d = {}; d[s] = { value: 7, enumerable: true }; return Object.create(null, d)[s]; });
p(function () { var s = Symbol("k"); var o = {}; Object.defineProperties(o, (function () { var d = {}; d[s] = { value: 8 }; return d; })()); return o[s]; });
p(function () { var t = { a: 1 }; var q = new Proxy(t, {}); Object.freeze(q); return [Object.isFrozen(q), Object.isFrozen(t), Object.isExtensible(q)].join(","); });
p(function () { var q = new Proxy({}, { ownKeys: function () { return ["a", "b"]; }, getOwnPropertyDescriptor: function (t, k) { return { value: 1, enumerable: k === "a", configurable: true }; } }); var acc = ""; for (var k in q) { acc += k; } return acc; });
p(function () { var q = new Proxy({}, { ownKeys: function () { return ["a", "b"]; }, getOwnPropertyDescriptor: function (t, k) { return { value: 1, enumerable: k === "a", configurable: true }; } }); return JSON.stringify(Object.assign({}, q)); });
p(function () { var proto = new Proxy({ m: function () { return this.v; } }, {}); var o = Object.create(proto); o.v = 5; return o.m(); });
p(function () { var r = Proxy.revocable({}, {}); var child = Object.create(r.proxy); r.revoke(); return child.anything; });

// --- the tag, which a proxy derives from two questions rather than from its target
p(function () { return Object.prototype.toString.call(new Proxy(new Date(), {})); });
p(function () { return Object.prototype.toString.call(new Proxy(new Error("x"), {})); });
p(function () { return Object.prototype.toString.call(new Proxy(/x/, {})); });
p(function () { return Object.prototype.toString.call(new Proxy(new String("s"), {})); });
p(function () { return Object.prototype.toString.call(new Proxy(function () { }, {})); });
p(function () { return Object.prototype.toString.call(new Proxy(new Proxy([], {}), {})); });
p(function () { var r = Proxy.revocable([], {}); r.revoke(); return Object.prototype.toString.call(r.proxy); });
p(function () { var q = new Proxy({}, {}); q[Symbol.toStringTag] = "Mine"; return Object.prototype.toString.call(q); });
