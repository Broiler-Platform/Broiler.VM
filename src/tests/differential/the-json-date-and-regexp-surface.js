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
