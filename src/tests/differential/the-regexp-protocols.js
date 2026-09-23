// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE REGEXP PROTOCOLS (JSeal follow-up VM-FIX-E, JSP-6).
//
// Four protocol steps the String and RegExp methods take and this profile used to skip:
// `String.prototype.matchAll` builds a fresh "g" RegExp from ToString of an argument that did not
// dispatch and Invokes its `Symbol.matchAll` (cases 1-5); `RegExp.prototype[Symbol.matchAll]`
// takes any Object receiver, builds its matcher through SpeciesConstructor from the `flags`
// PROPERTY, copies `lastIndex` through ToLength, and its iterator steps through RegExpExec so a
// custom `exec` is observed (cases 6-15); the `RegExp` constructor uses IsRegExp - a RegExp-like
// object is read for `source` and `flags`, and the call form returns its argument only when that
// argument's `constructor` is `RegExp` (cases 16-23); and `match`, `replace`, `search` and `split`
// ask only an Object pattern for its Symbol (cases 24-29), and a pattern or separator that did not
// dispatch is text - converted before `split`'s zero limit answers and, in `replace`, after a text
// replacement has been converted (cases 30-31). The RegExp String Iterator keeps a [[Done]] slot
// rather than generator states: a step that throws leaves it to run `exec` again, and a custom
// `exec` that re-enters `next` is answered, not refused (cases 32-35).
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function all(it) { return Array.from(it, function (m) { return m[0] + "@" + m.index; }).join(","); }
p(function () { var r = /a/g; r[Symbol.matchAll] = undefined; return all("/a/g x /a/g".matchAll(r)); });
p(function () { return all("a.b.c".matchAll(".")); });
p(function () { return all("aXbX".matchAll(undefined)).length; });
p(function () {
    var saved = RegExp.prototype[Symbol.matchAll];
    RegExp.prototype[Symbol.matchAll] = function (s) { return "invoked:" + this.source + ":" + this.flags + ":" + s; };
    try { return "abc".matchAll("b+"); } finally { RegExp.prototype[Symbol.matchAll] = saved; }
});
p(function () { return String.prototype.matchAll.call(null, "a"); });
p(function () { return RegExp.prototype[Symbol.matchAll].call(1, "a"); });
p(function () {
    var o = { constructor: undefined, flags: "g", lastIndex: 0, source: "b", exec: null };
    o[Symbol.match] = true;
    return all(RegExp.prototype[Symbol.matchAll].call(o, "abcb"));
});
p(function () {
    var log = [];
    var r = /./g;
    Object.defineProperty(r, "flags", { get: function () { log.push("flags"); return "g"; } });
    r.constructor = { get [Symbol.species]() { log.push("species"); return RegExp; } };
    r.lastIndex = { valueOf: function () { log.push("lastIndex"); return 1; } };
    var found = all(RegExp.prototype[Symbol.matchAll].call(r, "abc"));
    return log.join(",") + "=" + found;
});
p(function () { var r = /a/g; Object.defineProperty(r, "flags", { value: "" }); return all(r[Symbol.matchAll]("aaa")); });
p(function () {
    var r = /a/g;
    r.lastIndex = 2;
    var found = all(r[Symbol.matchAll]("aaaa"));
    return found + ";" + r.lastIndex;
});
p(function () {
    var calls = 0;
    var r = /a/g;
    r.constructor = { [Symbol.species]: function (source, flags) {
        var inner = new RegExp(source, flags);
        inner.exec = function (s) { calls++; return calls > 2 ? null : { 0: "z", index: calls, length: 1 }; };
        return inner;
    } };
    var found = all(r[Symbol.matchAll]("aaaa"));
    return found + ";" + calls;
});
p(function () {
    class R extends RegExp { exec(s) { var m = super.exec(s); if (m) { m[0] = m[0].toUpperCase(); } return m; } }
    return all("abab".matchAll(new R("a", "g")));
});
p(function () {
    var r = /(?:)/g;
    r.constructor = { [Symbol.species]: function () { var o = /(?:)/g; o.exec = function () { return 1; }; return o; } };
    return r[Symbol.matchAll]("x").next();
});
p(function () { return all(String.fromCharCode(0xD834, 0xDF06).matchAll(/(?:)/gu)); });
p(function () { return Object.prototype.toString.call("a".matchAll(/a/g)) + "," + typeof Object.getPrototypeOf("a".matchAll(/a/g)).next; });
p(function () { var o = { source: "b+", flags: "g", constructor: RegExp }; o[Symbol.match] = true; return RegExp(o) === o; });
p(function () { var o = { source: "b+", flags: "gi", constructor: Object }; o[Symbol.match] = true; return String(RegExp(o)); });
p(function () { var o = { source: "b+", flags: "gi" }; o[Symbol.match] = true; return String(new RegExp(o)); });
p(function () { var o = { source: "b+", flags: "gi" }; o[Symbol.match] = true; return String(new RegExp(o, "m")); });
p(function () { var r = /a/g; r.constructor = Object; return RegExp(r) === r; });
p(function () { var r = /a/g; return RegExp(r) === r; });
p(function () { var r = /a/g; r[Symbol.match] = false; return RegExp(r) === r; });
p(function () {
    var log = [];
    var o = {};
    Object.defineProperty(o, Symbol.match, { get: function () { log.push("match"); return true; } });
    Object.defineProperty(o, "constructor", { get: function () { log.push("constructor"); return RegExp; } });
    Object.defineProperty(o, "source", { get: function () { log.push("source"); return "q"; } });
    Object.defineProperty(o, "flags", { get: function () { log.push("flags"); return "y"; } });
    var made = new RegExp(o);
    return log.join(",") + "=" + String(made);
});
p(function () { Number.prototype[Symbol.match] = function () { return "called"; }; try { return "a1".match(1)[0]; } finally { delete Number.prototype[Symbol.match]; } });
p(function () { String.prototype[Symbol.search] = function () { return "called"; }; try { return "ab".search("b"); } finally { delete String.prototype[Symbol.search]; } });
p(function () { Boolean.prototype[Symbol.replace] = function () { return "called"; }; try { return "true!".replace(true, "T"); } finally { delete Boolean.prototype[Symbol.replace]; } });
p(function () { Number.prototype[Symbol.split] = function () { return "called"; }; try { return "a1b".split(1).join("|"); } finally { delete Number.prototype[Symbol.split]; } });
p(function () { var r = /a/g; r[Symbol.replace] = undefined; return "a/a/gb".replace(r, "X"); });
p(function () { var r = /,/; r[Symbol.split] = null; return "a,b/,/c".split(r).join("|"); });
p(function () { return "ab".split({ toString: function () { throw new RangeError("separator"); } }, 0); });
p(function () { var calls = 0; var r = "".replace("a", { toString: function () { calls++; return "b"; } }); return JSON.stringify(r) + ":" + calls; });
p(function () {
    var n = 0;
    class R extends RegExp { exec() { n++; if (n === 1) { throw new RangeError("once"); } return null; } }
    var it = "a".matchAll(new R("a", "g"));
    try { it.next(); } catch (e) { }
    return JSON.stringify(it.next()) + ":" + n;
});
p(function () {
    var n = 0;
    var r = /q/;
    r.constructor = { [Symbol.species]: function () { var o = /q/; o.exec = function () { n++; if (n === 1) { throw new RangeError("once"); } return ["q"]; }; return o; } };
    var it = r[Symbol.matchAll]("q");
    try { it.next(); } catch (e) { }
    var second = it.next();
    return second.value[0] + "," + second.done + "," + JSON.stringify(it.next()) + ":" + n;
});
p(function () {
    var it;
    var depth = 0;
    class R extends RegExp { exec() { depth++; if (depth === 1) { return { 0: "x", inner: JSON.stringify(it.next()) }; } return null; } }
    it = "a".matchAll(new R("a", "g"));
    var first = it.next();
    return first.value.inner + "," + first.done + "," + JSON.stringify(it.next()) + ":" + depth;
});
p(function () {
    var n = 0;
    class R extends RegExp { exec() { n++; var m = {}; if (n === 1) { Object.defineProperty(m, "0", { get: function () { throw new RangeError("zero"); } }); } else { m[0] = "a"; } return n > 2 ? null : m; } }
    var it = "a".matchAll(new R("a", "g"));
    var first;
    try { it.next(); first = "no throw"; } catch (e) { first = e.name; }
    return first + "," + it.next().value[0] + ":" + n;
});
