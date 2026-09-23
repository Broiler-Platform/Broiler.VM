// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE REGEXP GUARDS IN THE STRING SEARCH METHODS (JSeal slice V04).
//
// `startsWith`, `endsWith` and `includes` refuse an argument the language's IsRegExp calls a
// regular expression - a `Symbol.match` that answers truthy, or a RegExp whose `Symbol.match` is
// absent - before converting it to a String, so `'abc'.startsWith(/a/)` is a TypeError rather than
// a search for the characters "/a/". `replaceAll` and `matchAll` refuse such an argument whose
// `flags` do not contain "g", reading `flags` through the property rather than the internal flag.
// The order cases log each observable step, because the order is part of what the language says.
// Cases 37-41 hold the RegExp String Iterator of a non-global pattern to one match and then done,
// which the matchAll change above made reachable from `String.prototype.matchAll`.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function falseMatch(r) { r[Symbol.match] = false; return r; }
function order(name) {
    var log = [];
    var receiver = { toString: function () { log.push("this"); return "abc"; } };
    var search = {
        get [Symbol.match]() { log.push("match"); return undefined; },
        toString: function () { log.push("search"); return "b"; }
    };
    var position = { valueOf: function () { log.push("position"); return 1; } };
    var answer = String.prototype[name].call(receiver, search, position);
    return log.join(",") + "=" + answer;
}
p(function () { return "abc".startsWith(/a/); });
p(function () { return "abc".endsWith(/c/); });
p(function () { return "abc".includes(/b/); });
p(function () { return "/./".startsWith(falseMatch(/./)); });
p(function () { return "x/./".endsWith(falseMatch(/./)); });
p(function () { return "x/./x".includes(falseMatch(/./)); });
p(function () { return "abc".startsWith({ [Symbol.match]: true, toString: function () { return "a"; } }); });
p(function () { return "abc".includes({ [Symbol.match]: 1, toString: function () { return "b"; } }); });
p(function () { return "abc".includes({ [Symbol.match]: 0, toString: function () { return "b"; } }); });
p(function () { var r = /a/; r[Symbol.match] = undefined; return "/a/".startsWith(r); });
p(function () { var r = /a/; r[Symbol.match] = null; return "/a/".startsWith(r); });
p(function () { var r = /a/; Object.defineProperty(r, Symbol.match, { get: function () { throw new RangeError("hook"); } }); return "abc".endsWith(r); });
p(function () { return String.prototype.includes.call({ toString: function () { throw new RangeError("this"); } }, /a/); });
p(function () { return String.prototype.startsWith.call(null, /a/); });
p(function () { return order("startsWith"); });
p(function () { return order("endsWith"); });
p(function () { return order("includes"); });
p(function () { return "abc".startsWith("a") + "," + "abc".endsWith("bc", 3) + "," + "abc".includes("c", 3); });
p(function () { return "aaa".replaceAll(/a/g, "b"); });
p(function () { return "aaa".replaceAll(/a/, "b"); });
p(function () { return "x".replaceAll({ [Symbol.match]: true, flags: "g", [Symbol.replace]: function (s, r) { return "custom:" + s + r; } }, "y"); });
p(function () { return "x".replaceAll({ [Symbol.match]: true, flags: "i", [Symbol.replace]: function () { return "called"; } }, "y"); });
p(function () { var r = /a/g; Object.defineProperty(r, "flags", { value: undefined }); return "a".replaceAll(r, "b"); });
p(function () { var r = /a/g; Object.defineProperty(r, "flags", { value: "" }); return "a".replaceAll(r, "b"); });
p(function () { return "aa".replaceAll(falseMatch(/a/), "b"); });
p(function () { return "a1b2".replaceAll(/\d/g, function (d) { return d * 2; }); });
p(function () {
    var log = [];
    var search = {
        get [Symbol.match]() { log.push("match"); return true; },
        get flags() { log.push("flags"); return "g"; },
        get [Symbol.replace]() { log.push("replace"); return function () { return "done"; }; }
    };
    return log.join(",") + "=" + "x".replaceAll(search, "y") + ":" + log.join(",");
});
p(function () { return "a.a".replaceAll(".", "-") + "," + "abab".replaceAll("b", "$&$&"); });
p(function () { return "a".matchAll(/a/); });
p(function () { var r = /a/g; Object.defineProperty(r, "flags", { value: "" }); return "a".matchAll(r); });
p(function () { return "x".matchAll({ [Symbol.match]: true, flags: "y", [Symbol.matchAll]: function () { return "called"; } }); });
p(function () { return "x".matchAll({ [Symbol.match]: true, flags: "g", [Symbol.matchAll]: function (s) { return "custom:" + s; } }); });
p(function () { return Array.from("a1a2".matchAll(/a(\d)/g), function (m) { return m[1]; }).join(","); });
p(function () { return String.prototype.replaceAll.call(undefined, /a/g, "b"); });
p(function () { Number.prototype[Symbol.replace] = function () { return "called"; }; try { return "a1".replaceAll(1, "x"); } finally { delete Number.prototype[Symbol.replace]; } });
p(function () { Number.prototype[Symbol.matchAll] = function () { return "called"; }; try { return Array.from("a1".matchAll(1), function (m) { return m[0]; }).join(","); } finally { delete Number.prototype[Symbol.matchAll]; } });
p(function () { var r = /a/; r[Symbol.match] = false; return Array.from("aa".matchAll(r)).length; });
p(function () { return Array.from(RegExp.prototype[Symbol.matchAll].call(/a/, "aa"), function (m) { return m.index; }).join(","); });
p(function () { var it = RegExp.prototype[Symbol.matchAll].call(/b/, "abc"); return it.next().value[0] + "," + it.next().done + "," + it.next().done; });
p(function () { return Array.from(RegExp.prototype[Symbol.matchAll].call(/z/, "abc")).length; });
p(function () { return Array.from(RegExp.prototype[Symbol.matchAll].call(/a/y, "aab"), function (m) { return m.index; }).join(","); });
