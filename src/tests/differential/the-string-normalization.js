// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER String.prototype.normalize (JSeal slice F08, JSD-0031 U3).
//
// The pinned ES2026 normalize coerces its receiver with RequireObjectCoercible and ToString,
// defaults the form to "NFC", converts any other form with ToString, and throws a RangeError for
// anything but NFC, NFD, NFKC or NFKD before it looks at the text. The answer is the Unicode
// normalization of the string's code points; a lone surrogate is a code point with no mapping and
// passes through. The generated probes the-unicode-normalization-*.js carry the Unicode
// conformance vectors; these cases are the language's clauses and the edges around them.
//
// Answers are printed as code units in hex, so no non-ASCII character reaches the console.
//
// Each case prints its own number, so a divergence names a case rather than a line. A case that
// throws prints the error's name, because a refusal is an answer.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function units(s) { var out = []; for (var i = 0; i < s.length; i++) { out.push(s.charCodeAt(i).toString(16)); } return out.join(" "); }
function all(s) { return ["NFC", "NFD", "NFKC", "NFKD"].map(function (f) { return units(s.normalize(f)); }).join(" | "); }
p(function () { return typeof String.prototype.normalize; });
p(function () { return String.prototype.normalize.length + ":" + String.prototype.normalize.name; });
p(function () { return "e\u0301".normalize("NFC") === "\u00e9"; });
p(function () { return "e\u0301".normalize() === "\u00e9"; });
p(function () { return "e\u0301".normalize(undefined) === "\u00e9"; });
p(function () { return units("\u00e9".normalize("NFD")); });
p(function () { return "abc".normalize("NFX"); });
p(function () { return "\u00e9".normalize("NFX"); });
p(function () { return "abc".normalize("nfc"); });
p(function () { return "abc".normalize(null); });
p(function () { return "abc".normalize(""); });
p(function () { return String.prototype.normalize.call(null); });
p(function () { return String.prototype.normalize.call(undefined, "NFC"); });
p(function () { return String.prototype.normalize.call(1.5, "NFD"); });
p(function () { return units(String.prototype.normalize.call({ toString: function () { return "\u1e9b\u0323"; } }, "NFKC")); });
p(function () { return units("\u1e9b\u0323".normalize({ toString: function () { return "NFKD"; } })); });
p(function () { var log = []; try { String.prototype.normalize.call({ toString: function () { log.push("this"); return "x"; } }, { toString: function () { log.push("form"); return "NFQ"; } }); } catch (e) { log.push(e.name); } return log.join(","); });
p(function () { return "abc".normalize(Symbol("NFC")); });
p(function () { return String.prototype.normalize.call(Symbol("x")); });
p(function () { return all("\u1e9b\u0323"); });
p(function () { return all("\u1e0a\u0323"); });
p(function () { return all("\u1e0c\u0307"); });
p(function () { return all("q\u0307\u0323"); });
p(function () { return all("a\u0301\u0316\u0300"); });
p(function () { return all("\u212b"); });
p(function () { return all("\u2126"); });
p(function () { return all("\ufb01"); });
p(function () { return all("\u2460\u00bd"); });
p(function () { return all("\uac00\uac01\u1100\u1161\u11a8"); });
p(function () { return all("\u1100\u1161\u0301\u11a8"); });
p(function () { return all("\ud800"); });
p(function () { return all("\udc00"); });
p(function () { return all("a\ud800\u0301b"); });
p(function () { return all("\udc00\ud800"); });
p(function () { return all("e\ud834\u0301"); });
p(function () { return all("\ud834\udd5e"); });
p(function () { return all("\ud834\udd57\ud834\udd65"); });
p(function () { return all("\ud835\udc00"); });
p(function () { return all("\ud804\udd31\ud804\udd27"); });
p(function () { return all("\u0f73"); });
p(function () { return all("\u0344"); });
p(function () { return all("\u0958"); });
p(function () { return all("\u2adc"); });
p(function () { return all("a\u05b0\u05b1\u0301\u0300\u0316"); });
p(function () { return all("\u00c5\u0328"); });
p(function () { return units("\ufdfa".normalize("NFKD")).split(" ").length; });
p(function () { return "\ufdfa".normalize("NFC") === "\ufdfa"; });
p(function () { return "\ufdfa".repeat(10000).normalize("NFKC").length; });
p(function () { return "\ufdfa".repeat(10000).normalize("NFKD") === "\ufdfa".normalize("NFKD").repeat(10000); });
p(function () { var s = "a" + "\u0316\u0301".repeat(20000); var n = s.normalize("NFD"); return n.length + " " + units(n.slice(0, 3)) + " " + units(n.slice(-2)); });
p(function () { var s = "\u00e9".repeat(50000); return s.normalize("NFD").length + " " + (s.normalize("NFD").normalize("NFC") === s); });
p(function () { var s = "plain ascii text"; return s.normalize("NFKD") === s; });
p(function () { var s = "\u00e9t\u00e9"; return s.normalize("NFC") === s; });
p(function () { var a = "A\u030a", b = "\u00c5", c = "\u212b"; return (a.normalize() === b.normalize()) + "," + (b.normalize() === c.normalize()) + "," + (a === b); });
p(function () { return "".normalize("NFKD") === ""; });
p(function () { var s = new String("e\u0301"); return String.prototype.normalize.call(s) === "\u00e9"; });
