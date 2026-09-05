// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER JSON, DATE, REGULAR EXPRESSIONS AND CLOSURES.
//
// `JSON.stringify` with a replacer array, a replacer function, an indent and a circular graph;
// `JSON.parse` with a reviver and with the inputs it must refuse; Date construction, arithmetic,
// rendering and round-tripping through its own output; regular expressions used through the String
// methods that take them; and closures, capture, recursion and the depth at which recursion stops.
//
// It is the probe that found the guest `throw` terminating the process from any depth past about
// five hundred, which nothing else here would have reached: the case that found it is a runaway
// recursion inside a `try`, which is how a program probes its own depth.
//
// Each case prints its own number, so a divergence names a case rather than a line, and the
// numbering survives a case being rewritten in place. A case that throws prints the error's name,
// because a refusal is an answer and a probe that stopped at the first one would compare nothing
// after it.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- JSON
p(function () { return JSON.stringify({a:[1,{b:2}]}); });
p(function () { return JSON.stringify({a:1}, ["a"]); });
p(function () { return JSON.stringify({a:1,b:2}, ["b"]); });
p(function () { return JSON.stringify([1,2], null, "\t"); });
p(function () { return JSON.stringify({a:{b:1}}, null, 1); });
p(function () { return JSON.stringify("a b"); });
p(function () { return JSON.stringify(""); });
p(function () { return JSON.stringify(Infinity); });
p(function () { return String(JSON.stringify(undefined)); });
p(function () { return JSON.stringify([undefined, function(){}]); });
p(function () { var a = {}; a.self = a; try { return JSON.stringify(a); } catch (e) { return e.name; } });
p(function () { return JSON.parse(" 1 "); });
p(function () { return JSON.parse('"\\u0041"'); });
p(function () { return JSON.parse("[1,2]", function (k, v) { return typeof v === "number" ? v * 2 : v; }).join(","); });
p(function () { return JSON.stringify(JSON.parse('{"a":null}')); });
p(function () { return t(function () { return JSON.parse("01"); }); });
p(function () { return t(function () { return JSON.parse('{"a":1,}'); }); });
p(function () { return JSON.stringify({a:1}, null, 12).length; });
p(function () { return JSON.stringify(new Number(5)); });
p(function () { return JSON.stringify(new String("s")); });
p(function () { return JSON.stringify({x: new Boolean(false)}); });

// --- Date
p(function () { return new Date(2020, 0, 32).getUTCDate(); });
p(function () { return new Date("2020-02-30T00:00:00Z").getTime(); });
p(function () { return new Date(Date.UTC(2020,1,29)).toISOString(); });
p(function () { return new Date(0).getTimezoneOffset(); });
p(function () { var d = new Date(0); d.setUTCFullYear(2000); return d.toISOString(); });
p(function () { var d = new Date(0); d.setUTCMonth(13); return d.toISOString(); });
p(function () { return new Date(0).toUTCString(); });
p(function () { return new Date(0).toDateString(); });
p(function () { return String(new Date(0)); });
p(function () { return new Date(0).valueOf(); });
p(function () { return +new Date(0); });
p(function () { return new Date(0) - 0; });
p(function () { return new Date(0) + ""; });
p(function () { return Date.parse("2020-01-01"); });
p(function () { return Date.parse("Thu, 01 Jan 1970 00:00:00 GMT"); });
p(function () { return new Date(1e13).toISOString(); });
p(function () { return new Date(-1).toISOString(); });
p(function () { return new Date(0).getUTCMilliseconds(); });
p(function () { return typeof new Date().getTime(); });

// --- RegExp and String together
p(function () { return "aaa".split(/a/).length; });
p(function () { return "a1b2c".replace(/\d/g, "").length; });
p(function () { return "abc".replace(/(b)/, function (m, g1, off, s) { return off + g1 + s.length; }); });
p(function () { var r = /a/y; r.lastIndex = 1; return r.test("ba"); });
p(function () { var r = /a/y; r.lastIndex = 0; return r.test("ba"); });
p(function () { return "aBc".replace(/b/i, "X"); });
p(function () { return "a\nb".match(/^b/m)[0]; });
p(function () { return String("abc".match(/d/)); });
p(function () { return "abc".match(/./g).length; });
p(function () { return /(\d+)/.exec("x123")[1]; });
p(function () { return /(\d+)/.exec("x123").index; });
p(function () { return /(\d+)/.exec("x123").input; });
p(function () { return "$1".replace(/(\$)(1)/, "$2$1"); });
p(function () { return "abc".replace("b", "$&$&"); });
p(function () { return "aaa".replace(/a/g, function () { return "$&"; }); });
p(function () { return new RegExp("\\d").test("5"); });
p(function () { return RegExp.prototype.toString.call(/a/g); });
p(function () { return /[\d-a]/.test("-"); });
p(function () { return /a{2,}/.test("aaa"); });
p(function () { return /(?:a|b)+/.exec("abab")[0]; });
p(function () { return /(a)|(b)/.exec("b").slice(1).map(String).join(","); });
p(function () { return "x".replace(/(?<y>x)/, "$<y>$<y>"); });
p(function () { return /\u{1F600}/u.test("\u{1F600}"); });
p(function () { return "aaa".lastIndexOf("a", 1); });
p(function () { return "abcabc".split("b").length; });

// --- closures, recursion, miscellany
p(function () { function counter() { var n = 0; return function () { return ++n; }; } var c = counter(); c(); return c(); });
p(function () { var a = []; for (var i = 0; i < 3; i++) { (function (j) { a.push(function () { return j; }); })(i); } return a[1](); });
p(function () { function fib(n) { return n < 2 ? n : fib(n-1) + fib(n-2); } return fib(20); });
p(function () { var o = {a: 1, get b() { return this.a + 1; }}; return o.b; });
p(function () { var base = {get v() { return 7; }}; var d = Object.create(base); return d.v; });
p(function () { var o = {}; var n = 0; Object.defineProperty(o, "x", {get: function () { n++; return n; }, configurable:true}); o.x; return o.x; });
p(function () { var s = 0; [1,2,3].forEach(function (x) { s += x; }); return s; });
p(function () { return [1,2,3].filter(function (x) { return x !== 2; }).join(","); });
p(function () { return typeof (function () { return arguments; })().length; });
p(function () { return (function () { return Array.prototype.join.call(arguments, "-"); })(1,2); });
p(function () { return (function () {}).toString().indexOf("function"); });
p(function () { return String(Math.max.apply(null, [1,5,3])); });
p(function () { var a = [1,2,3]; return Math.min.apply(Math, a); });
p(function () { var n = 0; try { (function r() { n++; r(); })(); } catch (e) { return e instanceof RangeError; } return "no-throw"; });

// --- the pattern, the text, the instant and the number, one corner at a time. APPENDED for the
// reason the numbering asks. It is the pass that found `matchAll` missing, the two well-formed
// methods missing, and a string literal holding an unpaired surrogate reaching the artifact as a
// replacement character.
p(function(){ return "abc".replace(/b/, "X"); });
p(function(){ return "aaa".replace(/a/g, "b"); });
p(function(){ return "abc".replaceAll("b", "X"); });
p(function(){ return "a1b2".replace(/\d/g, function(m){ return "<"+m+">"; }); });
p(function(){ return "2020-01-02".replace(/(\d+)-(\d+)-(\d+)/, "$3/$2/$1"); });
p(function(){ return "abc".replace(/(b)/, "[$&][$`][$'][$1]"); });
p(function(){ var m = "2020-01".match(/(?<y>\d{4})-(?<m>\d{2})/); return m.groups.y + "/" + m.groups.m; });
p(function(){ return "2020-01".replace(/(?<y>\d{4})/, "$<y>!"); });
p(function(){ return String(/a/y.sticky) + /a/g.flags + /a/gimsu.flags; });
p(function(){ var r = /a/g; r.lastIndex = 0; r.exec("aa"); return r.lastIndex; });
p(function(){ var r = /a/y; return String(r.test("ba")); });
p(function(){ return "aXbXc".split(/X/).join("|"); });
p(function(){ return "a1b2c".split(/(\d)/).join("|"); });
p(function(){ return "abc".split("").join("-"); });
p(function(){ return [..."a1b2".matchAll(/(\w)(\d)/g)].map(function(m){return m[1]+m[2];}).join(); });
p(function(){ return String("abc".search(/b/)) + "abc".search(/z/); });
p(function(){ return /(?<=a)b/.exec("ab") ? "lookbehind" : "none"; });
p(function(){ return /(?<!a)b/.test("cb") ? "neg-lookbehind" : "none"; });
p(function(){ return new RegExp("\\p{L}", "u").test("é") ? "prop" : "no-prop"; });
p(function(){ return /a{2,3}/.exec("aaaa")[0]; });
p(function(){ return /(a)?b/.exec("b")[1] === undefined ? "undef" : "set"; });
p(function(){ return /a|ab/.exec("ab")[0]; });
p(function(){ return String(/^$/.test("")); });
p(function(){ return "AAA".replace(/a/gi, "x"); });
p(function(){ return "a\nb".match(/^b$/m)[0]; });
p(function(){ return String(/./s.test("\n")); });
p(function(){ return "x".replace(/(x)/, function(m, p1, off, s){ return p1+off+s; }); });
p(function(){ return String(RegExp.prototype.source) + "," + String(new RegExp("").source); });
p(function(){ return new RegExp("a+", "g").exec("caaat")[0]; });
p(function(){ return String(/[A-Z]/.test("Q")); });
p(function(){ return "café".normalize ? "has-normalize" : "no"; });
p(function(){ return "abc".at(-1) + "abc".at(0); });
p(function(){ return "5".padStart(3, "0") + "|" + "5".padEnd(3, "*"); });
p(function(){ return "  x  ".trimStart() + "|" + "  x  ".trimEnd() + "|"; });
p(function(){ return "abc".codePointAt(0) + "," + String.fromCharCode(97, 98); });
p(function(){ return "ab".repeat(3) + "," + "abc".includes("bc") + "," + "abc".startsWith("a"); });
p(function(){ return "a-b-c".split("-", 2).join("|"); });
p(function(){ return "abc".localeCompare("abd") + "," + "b".localeCompare("a"); });
p(function(){ return "😀".length + "," + [..."😀"].length; });
p(function(){ return "abc".substring(2, 0) + "abc".slice(-2) + "abc".substr(1, 1); });
p(function(){ return String("abc".isWellFormed ? "abc".isWellFormed() : "absent"); });
p(function(){ return "a".concat("b", 1, null); });
p(function(){ return String("Abc".toLowerCase()) + "Abc".toUpperCase(); });
p(function(){ return "x".anchor ? "has-anchor" : "absent"; });
p(function(){ var d = new Date(Date.UTC(2020, 0, 2, 3, 4, 5)); return d.toISOString(); });
p(function(){ return String(new Date("2020-01-02T03:04:05Z").getTime()); });
p(function(){ return String(new Date(NaN)); });
p(function(){ var d = new Date(Date.UTC(2020,0,2)); return d.getUTCFullYear()+","+d.getUTCMonth()+","+d.getUTCDay(); });
p(function(){ return String(Date.parse("2020-01-02")); });
p(function(){ return new Date(0).toJSON(); });
p(function(){ var d = new Date(Date.UTC(2020,0,2)); d.setUTCDate(31); return d.toISOString(); });
p(function(){ return String(new Date(8.64e15 + 1)); });
p(function(){ return typeof Date.now() + "," + (Date.now() > 0); });
p(function(){ return String(new Date(Date.UTC(2020,0,2)).valueOf()); });
p(function(){ return (12345.6789).toFixed(2) + "," + (0.000001).toFixed(7) + "," + (1e21).toFixed(2); });
p(function(){ return (255).toString(16) + "," + (255).toString(2) + "," + (-255).toString(36); });
p(function(){ return (123.456).toPrecision(4) + "," + (0).toPrecision(1); });
p(function(){ return Number("0x10") + "," + Number("0b11") + "," + Number("0o17") + "," + Number(""); });
p(function(){ return parseInt("0x10") + "," + parseInt("12px") + "," + parseFloat("1.5e2x"); });
p(function(){ return Number.MAX_SAFE_INTEGER + "," + Number.EPSILON; });
p(function(){ return String(Number.isInteger(5.0)) + Number.isSafeInteger(2**53) + Number.parseFloat("1.5"); });
p(function(){ return Math.round(-0.5) + "," + Math.round(0.5) + "," + Math.round(2.5); });
p(function(){ return Math.max() + "," + Math.min() + "," + Math.hypot(3,4) + "," + Math.sign(-3); });
p(function(){ return Math.trunc(-4.7) + "," + Math.fround(5.5) + "," + Math.clz32(1) + "," + Math.imul(3,4); });
p(function(){ return (1000000).toLocaleString ? typeof (1000).toLocaleString() : "absent"; });
p(function(){ return [..."a1b2".matchAll(/(\w)(\d)/g)].map(function(m){return m[1]+m[2];}).join(); });
p(function(){ return [..."a1b2".matchAll(/(\w)(\d)/g)].map(function(m){return m.index;}).join(); });
p(function(){ try { "ab".matchAll(/a/); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ return [..."aaa".matchAll(/a/g)].length; });
p(function(){ return [..."aaa".matchAll("a")].length; });
p(function(){ return [..."abc".matchAll(/x/g)].length; });
p(function(){ return [..."aaa".matchAll(/(?:)/g)].length; });
p(function(){ var r = /a/g; r.lastIndex = 5; var n = [..."aaa".matchAll(r)].length; return n + "," + r.lastIndex; });
p(function(){ var m = [..."2020-01".matchAll(/(?<y>\d+)/g)]; return m[0].groups.y; });
p(function(){ var it = "ab".matchAll(/./g); return typeof it.next + "," + Object.prototype.toString.call(it); });
p(function(){ var it = "a".matchAll(/a/g); it.next(); return JSON.stringify(it.next()); });
p(function(){ return "abc".matchAll.length; });
p(function(){ return String("ab".isWellFormed()) + String("a\uD800b".isWellFormed()); });
p(function(){ return JSON.stringify("a\uD800b".toWellFormed()) + "," + "a\uD800b".toWellFormed().length; });
p(function(){ return "😀".isWellFormed() + "," + JSON.stringify("😀".toWellFormed()); });
p(function(){ return "\uDC00".isWellFormed() + "," + "\uD800\uD800".isWellFormed(); });
p(function(){ return String.prototype.isWellFormed.length + "," + String.prototype.toWellFormed.length; });
p(function(){ var s = "a\uD800b"; return s.length + "," + s.charCodeAt(1).toString(16); });
p(function(){ return JSON.stringify("\uD800"); });
p(function(){ return String.fromCharCode(0xD800).charCodeAt(0).toString(16); });
p(function(){ var o = {}; o["\uD800"] = 1; return Object.keys(o)[0].charCodeAt(0).toString(16); });

// --- the pattern protocol: the five Symbols a String method dispatches through, and the five
// methods `RegExp.prototype` answers them with. APPENDED for the reason the numbering asks.
p(function(){ return "abc".match(/b/)[0]; });
p(function(){ return "aaa".match(/a/g).join(); });
p(function(){ return "abc".search(/c/); });
p(function(){ return "abc".replace(/b/, "X"); });
p(function(){ return "a1b".split(/\d/).join("|"); });
p(function(){ return [..."a1b2".matchAll(/\d/g)].map(function(m){return m[0];}).join(); });
p(function(){ var o = {}; o[Symbol.match] = function(s){ return "custom:" + s; }; return "abc".match(o); });
p(function(){ var o = {}; o[Symbol.search] = function(s){ return 42; }; return "abc".search(o); });
p(function(){ var o = {}; o[Symbol.replace] = function(s, r){ return s + "|" + r; }; return "abc".replace(o, "R"); });
p(function(){ var o = {}; o[Symbol.split] = function(s, l){ return [s, l]; }; return "abc".split(o, 3).join(","); });
p(function(){ var o = {}; o[Symbol.matchAll] = function(s){ return "iter:" + s; }; return "abc".matchAll(o); });
p(function(){ var o = {}; o[Symbol.match] = 5; try { "abc".match(o); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ var o = {}; o[Symbol.match] = null; return String("abc".match(o)); });
p(function(){ return String("abc".match(null)); });
p(function(){ return "null".replace(null, "X"); });
p(function(){ return typeof RegExp.prototype[Symbol.match] + typeof RegExp.prototype[Symbol.replace] + typeof RegExp.prototype[Symbol.split] + typeof RegExp.prototype[Symbol.search] + typeof RegExp.prototype[Symbol.matchAll]; });
p(function(){ return RegExp.prototype[Symbol.match].name + "," + RegExp.prototype[Symbol.replace].length; });
p(function(){ try { RegExp.prototype[Symbol.match].call({}, "a"); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ return /b/[Symbol.match]("abc")[0]; });
p(function(){ return /b/[Symbol.replace]("abc", "Z"); });
p(function(){ return /\d/[Symbol.split]("a1b").join("|"); });
p(function(){ return /c/[Symbol.search]("abc"); });
p(function(){ return typeof Symbol.matchAll; });
p(function(){ var calls = []; var o = { get [Symbol.replace](){ calls.push("get"); return function(){ return "R"; }; } }; var r = "x".replace(o, "y"); return r + calls.join(); });
p(function(){ return "aXbXc".replace(/X/g, function(m, off){ return off; }); });
p(function(){ return "abc".replace("b", function(m){ return m.toUpperCase(); }); });
p(function(){ return String.fromCodePoint(0xD800).length + "," + String.fromCodePoint(0xD800).charCodeAt(0).toString(16); });
p(function(){ var s = String.fromCodePoint(0xDC00) + String.fromCodePoint(0xDC01); return s.length + "," + /^\D+$/u.test(s); });
p(function(){ return String.fromCodePoint(0x10000).length + "," + String.fromCodePoint(0x10000).codePointAt(0).toString(16); });
p(function(){ try { String.fromCodePoint(0x110000); return "no-throw"; } catch(e){ return e.name; } });
