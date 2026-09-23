// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE REGEXP SYMBOL METHODS (JSeal follow-up VM-FIX-G, JSP-6).
//
// `RegExp.prototype[Symbol.match]`, `[Symbol.replace]`, `[Symbol.search]` and `[Symbol.split]`
// take any Object receiver and go through RegExpExec, so a custom `exec` answers for them
// (cases 1-12); they read the `flags` PROPERTY rather than the internal flags, and the `flags`
// getter itself reads the individual flag properties (cases 13-18); `lastIndex` is read with
// ToLength and written with a throwing Set in the order the algorithms give (cases 19-24);
// `Symbol.split` constructs its splitter through SpeciesConstructor with a "y" flag (cases 25-29);
// `Symbol.replace` builds its replacement from the result's `groups` object, with GetSubstitution's
// two-digit and named-reference rules (cases 30-37); the built-in `exec` reads `lastIndex` through
// ToLength even for a pattern that is neither global nor sticky, and `test` goes through
// RegExpExec (cases 38-41); and `super[Symbol.replace]` inside a class extending RegExp reaches
// the prototype's method (cases 42-43). Cases 44-45 check the built-in walk against a
// replaced `RegExp.prototype.exec`, which every per-position and per-match step must observe.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function fake(flags, answers) {
    var at = 0;
    return {
        flags: flags,
        lastIndex: 0,
        exec: function () { var a = answers[at++]; return a === undefined ? null : a; }
    };
}
function res(text, index, extra) {
    var r = [text];
    if (extra) { for (var i = 0; i < extra.length; i++) { r.push(extra[i]); } }
    r.index = index;
    return r;
}

// ---- any Object receiver, through RegExpExec (1-12) ----
p(function () { return RegExp.prototype[Symbol.match].call(1, "a"); });
p(function () { return RegExp.prototype[Symbol.replace].call("x", "a", "b"); });
p(function () { return RegExp.prototype[Symbol.search].call(null, "a"); });
p(function () { return RegExp.prototype[Symbol.split].call(undefined, "a"); });
p(function () { return JSON.stringify(RegExp.prototype[Symbol.match].call(fake("", [res("zz", 3)]), "abc")); });
p(function () { return RegExp.prototype[Symbol.match].call(fake("g", [res("q", 0), res("r", 1)]), "abc").join(","); });
p(function () { return RegExp.prototype[Symbol.replace].call(fake("", [res("b", 1)]), "abc", "[$&]"); });
p(function () { return RegExp.prototype[Symbol.replace].call(fake("g", [res("a", 0), res("c", 2)]), "abc", "-"); });
p(function () { return RegExp.prototype[Symbol.search].call(fake("", [res("c", 2)]), "abc"); });
p(function () {
    var r = /b/;
    r.exec = function (s) { return res("x", 7); };
    return "abc".search(r) + ";" + "abc".replace(r, "Q") + ";" + JSON.stringify("abc".match(r));
});
p(function () {
    var r = /./g;
    var calls = 0;
    r.exec = function (s) { calls++; return RegExp.prototype.exec.call(this, s); };
    return "abc".replace(r, "-") + ";" + calls;
});
p(function () { var r = /a/; r.exec = function () { return 1; }; return "a".match(r); });

// ---- flags read as a property (13-18) ----
p(function () {
    var log = [];
    var o = {};
    ["hasIndices", "global", "ignoreCase", "multiline", "dotAll", "unicode", "unicodeSets", "sticky"].forEach(function (k) {
        Object.defineProperty(o, k, { get: function () { log.push(k); return k === "global" || k === "sticky"; } });
    });
    var f = Object.getOwnPropertyDescriptor(RegExp.prototype, "flags").get.call(o);
    return f + ":" + log.join(",");
});
p(function () { var r = /a/g; Object.defineProperty(r, "global", { value: false }); return r.flags + ";" + "aaa".replace(r, "b"); });
p(function () { var r = /a/y; Object.defineProperty(r, "flags", { value: "g" }); return "aaba".replace(r, "b") + ";" + "aaba".match(r).length; });
p(function () { var r = /a/g; Object.defineProperty(r, "flags", { value: "" }); return "aaa".replace(r, "b") + ";" + r.lastIndex; });
p(function () { return Object.getOwnPropertyDescriptor(RegExp.prototype, "flags").get.call(1); });
p(function () { var r = /a/; Object.defineProperty(r, "flags", { get: function () { throw new RangeError(); } }); return "a".match(r); });

// ---- lastIndex through ToLength and Set (19-24) ----
p(function () { var r = /a/g; r.lastIndex = 3; var m = "aaaa".match(r); return m.length + ";" + r.lastIndex; });
p(function () { var r = /a/g; Object.defineProperty(r, "lastIndex", { writable: false, value: 0 }); return "aaa".replace(r, "b"); });
p(function () { var r = /b/y; r.lastIndex = 2; var n = "abcb".search(r); return n + ";" + r.lastIndex; });
p(function () {
    var r = /./;
    var log = [];
    r.lastIndex = { valueOf: function () { log.push("valueOf"); return 0; } };
    var n = "ab".search(r);
    return n + ";" + log.join(",") + ";" + typeof r.lastIndex;
});
p(function () {
    var r = /(?:)/g;
    var seen = [];
    r.exec = function (s) {
        seen.push(this.lastIndex);
        if (seen.length > 3) { return null; }
        return RegExp.prototype.exec.call(this, s);
    };
    return JSON.stringify("ab".match(r)) + ";" + seen.join(",");
});
p(function () { return JSON.stringify(String.fromCharCode(0xD83D, 0xDE00).match(/(?:)/gu)); });

// ---- Symbol.split through the species (25-29) ----
p(function () {
    var log = [];
    var r = /,/;
    r.constructor = {};
    r.constructor[Symbol.species] = function (source, flags) { log.push(flags); return new RegExp(source, flags); };
    return "a,b,c".split(r).join("|") + ";" + log.join(",");
});
p(function () {
    var r = /,/y;
    var made;
    r.constructor = {};
    r.constructor[Symbol.species] = function (source, flags) { made = new RegExp(source, flags); return made; };
    var out = "a,b".split(r).join("|");
    return out + ";" + made.flags + ";" + made.lastIndex;
});
p(function () {
    var r = /x/;
    r.constructor = {};
    r.constructor[Symbol.species] = function () { return fake("y", []); };
    return JSON.stringify("abc".split(r));
});
p(function () { return JSON.stringify("A<B>bold</B>and".split(/<(\/)?([^<>]+)>/)); });
p(function () { return JSON.stringify("ab".split(/a*?/)) + JSON.stringify("ab".split(/a*/)) + JSON.stringify("".split(/(?:)/)) + JSON.stringify("a1b2c".split(/\d/, 2)); });

// ---- Symbol.replace results and GetSubstitution (30-37) ----
p(function () { return "2026-09".replace(/(?<y>\d+)-(?<m>\d+)/, "$<m>/$<y>|$<zz>|$<"); });
p(function () {
    var o = fake("", [(function () { var r = res("b", 1); r.groups = { k: "K" }; return r; })()]);
    return RegExp.prototype[Symbol.replace].call(o, "abc", "[$<k>]");
});
p(function () {
    var o = fake("", [(function () { var r = res("b", 1); r.groups = "str"; return r; })()]);
    return RegExp.prototype[Symbol.replace].call(o, "abc", "[$<length>]");
});
p(function () {
    var o = fake("", [(function () { var r = res("b", 1, ["c1"]); r.groups = { g: 1 }; return r; })()]);
    return RegExp.prototype[Symbol.replace].call(o, "abc", function () { return Array.prototype.slice.call(arguments).map(function (a) { return typeof a === "object" ? "{" + a.g + "}" : a; }).join(","); });
});
p(function () { return "abc".replace(/(b)/, "$01$10$00$0$2"); });
p(function () { return RegExp.prototype[Symbol.replace].call(fake("", [res("b", 1, ["1", "2", "3", "4", "5", "6", "7", "8", "9", "X"])]), "abc", "$10$11"); });
p(function () { return RegExp.prototype[Symbol.replace].call(fake("", [res("zzzz", 2)]), "abc", "<$'>"); });
p(function () { return RegExp.prototype[Symbol.replace].call(fake("g", [res("c", 2), res("a", 0)]), "abc", "_"); });

// ---- the built-in exec and test (38-41) ----
p(function () {
    var r = /a/;
    var log = [];
    r.lastIndex = { valueOf: function () { log.push("valueOf"); return 5; } };
    var m = r.exec("aa");
    return m.index + ";" + log.join(",");
});
p(function () { var r = /a/; r.lastIndex = { valueOf: function () { throw new RangeError(); } }; return r.exec("a"); });
p(function () { var o = fake("", [res("x", 0)]); return RegExp.prototype.test.call(o, "q"); });
p(function () { return RegExp.prototype.test.call({ exec: 1 }, "q"); });

// ---- super[Symbol.replace] in a RegExp subclass (42-43) ----
p(function () {
    class R extends RegExp {
        [Symbol.replace](s, r) { return "<" + super[Symbol.replace](s, r) + ">"; }
    }
    return "aba".replace(new R("a", "g"), "x");
});
p(function () {
    class R extends RegExp {
        [Symbol.replace](s, r) { return super[Symbol.replace](s, r); }
    }
    return "aaa".replaceAll(new R("a", "g"), function (m, i) { return i; });
});

// ---- the built-in walk and a replaced RegExp.prototype.exec (44-45) ----
p(function () {
    var e = String.fromCharCode(0xD83D, 0xDE00);
    return ("x" + e + "y").replace(/(?<c>)/gu, function () {
        var a = Array.prototype.slice.call(arguments);
        return "[" + a[2] + (typeof a[4]) + "]";
    }).split(e).join("E") + "|" + "a1b22".replace(/(?<d>\d)|(z)/g, "<$<d>$2>");
});
p(function () {
    var saved = RegExp.prototype.exec;
    var calls = 0;
    RegExp.prototype.exec = function (s) { calls++; return saved.call(this, s); };
    try { return "a,b".split(/,/).join("|") + ";" + "aXa".replace(/a/g, "b") + ";" + calls; }
    finally { RegExp.prototype.exec = saved; }
});
