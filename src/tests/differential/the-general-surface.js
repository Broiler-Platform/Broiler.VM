// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE SURFACE THIS PROFILE ADMITS.
//
// Coercions and operators, strings, arrays, objects, functions, errors, `Math` and
// `Number`, dates, regular expressions, `Symbol`, the keyed collections, promises and the
// binary surface. It is written to COVER the surface rather than to confirm it: the cases
// were chosen from what the language says, not from what this realm was known to have, which
// is why it found seven absent methods inside globals a name-level rule reported present.
//
// Each case prints its own number, so a divergence names a case rather than a line, and the
// numbering survives a case being rewritten in place. A case that throws prints the error's name,
// because a refusal is an answer and a probe that stopped at the first one would compare nothing
// after it.

function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
var __n = 0;
function p(f) { __n++; print(__n + " " + t(f)); }

// --- coercions and operators
p(function () { return [] + {}; });
p(function () { return {} + []; });
p(function () { return [] == false; });
p(function () { return null >= 0; });
p(function () { return null > 0; });
p(function () { return NaN !== NaN; });
p(function () { return 0.1 + 0.2; });
p(function () { return (-0).toString(); });
p(function () { return 1 / -0; });
p(function () { return String(-0); });
p(function () { return Object.is(-0, 0); });
p(function () { return 2 ** 53 === 2 ** 53 + 1; });
p(function () { return 1e21.toString(); });
p(function () { return (255).toString(16); });
p(function () { return (0.5).toString(2); });
p(function () { return parseInt("0x10"); });
p(function () { return parseInt("10", 36); });
p(function () { return parseFloat(".5e1"); });
p(function () { return Number(""); });
p(function () { return Number("  12  "); });
p(function () { return Number("0b101"); });
p(function () { return Number("Infinity"); });
p(function () { return +"1_0"; });
p(function () { return 5 % -3; });
p(function () { return -5 % 3; });
p(function () { return 1 << 31; });
p(function () { return -1 >>> 0; });
p(function () { return 2147483648 | 0; });
p(function () { return "b" + "a" + +"a" + "a"; });
p(function () { return typeof null; });
p(function () { return typeof undeclaredThing; });
p(function () { return void 0; });

// --- strings
p(function () { return "abc".at(-1); });
p(function () { return "abc".padStart(5, "xy"); });
p(function () { return "abc".padEnd(5); });
p(function () { return "a-b-c".split("-", 2).join("|"); });
p(function () { return "abc".split("").length; });
p(function () { return "".split("").length; });
p(function () { return "abc".slice(-2); });
p(function () { return "abc".substring(2, 0); });
p(function () { return "abc".substr(-2, 1); });
p(function () { return "aaa".replace("a", "$&$&"); });
p(function () { return "aaa".replaceAll("a", "b"); });
p(function () { return "abc".indexOf("", 10); });
p(function () { return "abc".lastIndexOf(""); });
p(function () { return "abc".codePointAt(0); });
p(function () { return String.fromCharCode(0x41, 0x42); });
p(function () { return "é".normalize("NFD").length; });
p(function () { return "ABC".toLowerCase(); });
p(function () { return "  x \t".trim(); });
p(function () { return " x ".trimStart() + "|"; });
p(function () { return "abc".localeCompare("abd"); });
p(function () { return "a".repeat(3); });
p(function () { return "abc".includes("b"); });
p(function () { return "abc".startsWith("b", 1); });
p(function () { return "abc"["1"]; });
p(function () { return "abc".charAt(9); });
p(function () { return "abc".charCodeAt(9); });
p(function () { return "😀".length; });
p(function () { return Array.from("😀").length; });
p(function () { return "x".concat(1, null); });
p(function () { return String.prototype.trim.call(5); });
p(function () { return "abc".search("b"); });
p(function () { return "a1b2".match(/\d/g).join(""); });
p(function () { return "a1b2".replace(/\d/g, function (m) { return "[" + m + "]"; }); });

// --- arrays
p(function () { return [1,2,3].at(-1); });
p(function () { return [3,1,2].sort().join(","); });
p(function () { return [10,9].sort().join(","); });
p(function () { return [1,2,3].sort(function (a,b) { return b - a; }).join(","); });
p(function () { return [1,[2,[3]]].flat(2).join(","); });
p(function () { return [1,2].flatMap(function (x) { return [x, x]; }).join(","); });
p(function () { return [1,2,3].findLast(function (x) { return x < 3; }); });
p(function () { return [1,2,3].findLastIndex(function (x) { return x < 0; }); });
p(function () { return [,1].length; });
p(function () { return JSON.stringify([,1]); });
p(function () { return 1 in [,1]; });
p(function () { return [1,2,3].copyWithin(0,1).join(","); });
p(function () { return new Array(3).fill(0).join(","); });
p(function () { return [1,2,3].splice(1,1).join(",") ; });
p(function () { var a = [1,2,3]; a.splice(1,0,9); return a.join(","); });
p(function () { return [1,2,3].indexOf("2"); });
p(function () { return [NaN].indexOf(NaN); });
p(function () { return [NaN].includes(NaN); });
p(function () { return Array.of(7).length; });
p(function () { return Array.from({length:2, 0:"a", 1:"b"}).join(","); });
p(function () { return Array.from([1,2], function (x) { return x * 2; }).join(","); });
p(function () { return Array.isArray(Array.prototype); });
p(function () { return [1,2,3].reduce(function (a,b) { return a + b; }); });
p(function () { return [].reduce(function (a,b) { return a + b; }); });
p(function () { return [].reduce(function (a,b) { return a + b; }, 5); });
p(function () { return [1,2,3].reduceRight(function (a,b) { return a + "" + b; }); });
p(function () { var a = [1,2,3]; a.length = 1; return a.join(","); });
p(function () { var a = []; a[4294967294] = 1; return a.length; });
p(function () { var a = []; a[-1] = 1; return a.length; });
p(function () { return [1,2].concat(3, [4,[5]]).length; });
p(function () { return [1,2,3].slice(-2).join(","); });
p(function () { return [3,2,1].toSorted ? [3,2,1].toSorted().join(",") : "no-toSorted"; });
p(function () { return [1,2,3].join(); });
p(function () { return [null, undefined, 1].join("-"); });
p(function () { return String([1,[2,3]]); });
p(function () { return [1,2,3].toString(); });
p(function () { return Array.prototype.slice.call("abc").join(","); });

// --- objects
p(function () { return JSON.stringify(Object.keys({b:1, a:2, 2:3, 1:4})); });
p(function () { return JSON.stringify(Object.assign({}, {a:1}, {b:2})); });
p(function () { return JSON.stringify(Object.entries({a:1})); });
p(function () { return Object.getPrototypeOf([]) === Array.prototype; });
p(function () { return Object.create(null).toString; });
p(function () { var o = Object.freeze({a:1}); try { o.a = 2; } catch (e) {} return o.a; });
p(function () { "use strict"; var o = Object.freeze({a:1}); o.a = 2; return o.a; });
p(function () { return Object.isSealed(Object.preventExtensions({})); });
p(function () { var o = {}; Object.defineProperty(o, "x", {get: function () { return 7; }}); return o.x; });
p(function () { var o = {get x() { return 1; }, set x(v) {}}; return JSON.stringify(Object.getOwnPropertyDescriptor(o, "x").enumerable); });
p(function () { return JSON.stringify(Object.getOwnPropertyNames("ab")); });
p(function () { var o = {a:1}; delete o.a; return "a" in o; });
p(function () { return delete Object.prototype; });
p(function () { var o = {toString: function () { return "T"; }}; return "" + o; });
p(function () { var o = {valueOf: function () { return 5; }}; return o * 2; });
p(function () { var o = {a:1, b:2}; var s = ""; for (var k in o) s += k; return s; });
p(function () { var o = Object.create({p:1}); o.q = 2; var s = ""; for (var k in o) s += k; return s; });
p(function () { return ({}).hasOwnProperty.call([1], 0); });
p(function () { return ({}).propertyIsEnumerable.call([1], "length"); });
p(function () { return JSON.stringify({a: undefined, b: function () {}, c: 1}); });
p(function () { return JSON.stringify({a:1}, null, 2); });
p(function () { return JSON.stringify([1], function (k, v) { return v; }); });
p(function () { return JSON.parse('{"a":[1,2]}').a[1]; });
p(function () { return JSON.parse('1e3'); });
p(function () { return t(function () { return JSON.parse("{'a':1}"); }); });
p(function () { return JSON.stringify(new Date(0)); });
p(function () { return JSON.stringify({toJSON: function () { return 42; }}); });

// --- functions
p(function () { function f(a, b) {} return f.length; });
p(function () { return (function () { return arguments.length; })(1,2,3); });
p(function () { return (function (a) { a = 9; return arguments[0]; })(1); });
p(function () { "use strict"; return (function (a) { a = 9; return arguments[0]; })(1); });
p(function () { function f() { return this; } return f.call(null) === (function(){return this;})(); });
p(function () { "use strict"; function f() { return this; } return f.call(null); });
p(function () { function f(a) { return a; } return f.apply(null, [7]); });
p(function () { function f() { return this.x; } return f.bind({x:3})(); });
p(function () { function f(a, b) { return a + b; } return f.bind(null, 1)(2); });
p(function () { function f() {} return f.bind(null).name; });
p(function () { return (function () {}).constructor === Function; });
p(function () { return typeof Function.prototype; });
p(function () { return (function f() { return typeof f; })(); });
p(function () { var g = function f() {}; return typeof f; });
p(function () { return [1,2].map(function (x, i, a) { return a.length; }).join(","); });

// --- errors
p(function () { return new TypeError("x").message; });
p(function () { return String(new Error("x")); });
p(function () { return new Error("x") instanceof Error; });
p(function () { return TypeError.prototype.name; });
p(function () { try { null.x; } catch (e) { return e.constructor.name; } });
p(function () { try { undefinedFn(); } catch (e) { return e.name; } });
p(function () { try { (void 0)(); } catch (e) { return e.name; } });
p(function () { try { new (function () {})().x.y; } catch (e) { return e.name; } });
p(function () { try { ({}).x(); } catch (e) { return e.name; } });
p(function () { try { JSON.parse("["); } catch (e) { return e.name; } });
p(function () { try { decodeURIComponent("%"); } catch (e) { return e.name; } });
p(function () { try { (1).toFixed(101); } catch (e) { return e.name; } });
p(function () { try { new Array(-1); } catch (e) { return e.name; } });

// --- Math / Number
p(function () { return Math.max(); });
p(function () { return Math.min(); });
p(function () { return Math.round(-0.5); });
p(function () { return Math.round(0.5); });
p(function () { return Math.round(2.5); });
p(function () { return Math.sign(-0); });
p(function () { return Math.trunc(-1.5); });
p(function () { return Math.hypot(3,4); });
p(function () { return Math.cbrt(27); });
p(function () { return Math.clz32(1); });
p(function () { return Math.imul(3, 4); });
p(function () { return Math.fround(5.5); });
p(function () { return Math.log2(8); });
p(function () { return Math.expm1(0); });
p(function () { return Math.atan2(1, 1); });
p(function () { return Number.MAX_SAFE_INTEGER; });
p(function () { return Number.EPSILON > 0; });
p(function () { return Number.isInteger(5.0); });
p(function () { return Number.isSafeInteger(2 ** 53); });
p(function () { return (1234.5678).toFixed(2); });
p(function () { return (0.000001).toFixed(7); });
p(function () { return (123.456).toPrecision(4); });
p(function () { return (1e21).toFixed(2); });
p(function () { return (1.005).toFixed(2); });
p(function () { return (-1.5).toFixed(0); });
p(function () { return (123456789).toExponential(3); });

// --- dates (UTC only)
p(function () { return new Date(0).toISOString(); });
p(function () { return new Date("2020-01-01T00:00:00Z").getTime(); });
p(function () { return new Date(Date.UTC(2020, 0, 1)).getUTCFullYear(); });
p(function () { return new Date(NaN).toString(); });
p(function () { return new Date(8.64e15 + 1).getTime(); });
p(function () { return new Date(2020, 0, 1).getFullYear(); });
p(function () { return typeof Date.now(); });
p(function () { return new Date(0).getUTCDay(); });

// --- regexp
p(function () { return /a/g.flags; });
p(function () { return /(a)(b)?/.exec("a").length; });
p(function () { return String(/(a)(b)?/.exec("a")[2]); });
p(function () { return "aaa".replace(/a/g, "$'"); });
p(function () { return "xay".replace(/a/, "$`"); });
p(function () { return /a/.test("A"); });
p(function () { return /a/i.test("A"); });
p(function () { var r = /a/g; r.exec("aa"); return r.lastIndex; });
p(function () { return "a,b".split(/(,)/).join("|"); });
p(function () { return /(?<n>a)/.exec("a").groups.n; });
p(function () { return "abc".replace(/(?<x>b)/, "[$<x>]"); });
p(function () { return /\b/.test(""); });
p(function () { return /^$/m.test("a\n"); });
p(function () { return /./s.test("\n"); });
p(function () { return new RegExp("a+", "g").source; });
p(function () { return String(/a/); });

// --- Symbol / collections / promise
p(function () { return typeof Symbol(); });
p(function () { return Symbol("d").description; });
p(function () { return Symbol.for("k") === Symbol.for("k"); });
p(function () { return Symbol.keyFor(Symbol.for("k")); });
p(function () { return String(Symbol("s")); });
p(function () { var m = new Map(); m.set(NaN, 1); return m.get(NaN); });
p(function () { var s = new Set([1,1,2]); return s.size; });
p(function () { var m = new Map([[1,2]]); return m.get(1); });
p(function () { var w = new WeakMap(); var k = {}; w.set(k, 1); return w.get(k); });
p(function () { return Object.prototype.toString.call(new WeakSet()); });
p(function () { return typeof Promise.resolve().then; });
p(function () { return Object.prototype.toString.call(Promise.resolve()); });

// --- typed arrays
p(function () { var a = new Uint8Array([1,2,3]); return a.join(","); });
p(function () { var a = new Uint8Array(2); a[5] = 1; return a.length + "," + a[5]; });
p(function () { var a = new Int8Array([200]); return a[0]; });
p(function () { var a = new Uint8ClampedArray([300]); return a[0]; });
p(function () { var a = new Float64Array([1.5]); return a[0]; });
p(function () { var b = new ArrayBuffer(8); return new DataView(b).getUint8(0); });
p(function () { var b = new ArrayBuffer(4); var v = new DataView(b); v.setInt32(0, 1); return v.getInt32(0, true); });
p(function () { return new Uint8Array(new ArrayBuffer(8), 4).length; });
p(function () { return Object.prototype.toString.call(new Uint8Array(1)); });
p(function () { return new Uint8Array([1,2,3]).subarray(1).join(","); });
p(function () { return Array.prototype.slice.call(new Uint8Array([1,2])).join(","); });
