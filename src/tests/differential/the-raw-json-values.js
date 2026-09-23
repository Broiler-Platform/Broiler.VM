// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER JSON.rawJSON AND JSON.isRawJSON.
//
// Retained for slice F17: a raw JSON value is a frozen, null-prototype object carrying one own
// "rawJSON" string and a brand no lookalike can forge; stringify emits its text verbatim wherever
// SerializeJSONProperty reaches it, after toJSON and the replacer function have had their turn.
// Each case prints its own number so a divergence names a case rather than a line.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function desc(o, k) {
  var d = Object.getOwnPropertyDescriptor(o, k);
  return d ? [d.writable, d.enumerable, d.configurable].join(",") : "none";
}

// --- the two functions themselves
p(function () { return typeof JSON.rawJSON + " " + JSON.rawJSON.length + " " + JSON.rawJSON.name; });
p(function () { return typeof JSON.isRawJSON + " " + JSON.isRawJSON.length + " " + JSON.isRawJSON.name; });
p(function () { return desc(JSON, "rawJSON") + " " + desc(JSON, "isRawJSON"); });
p(function () { return new JSON.rawJSON("1"); });
p(function () { return new JSON.isRawJSON({}); });

// --- the object rawJSON returns
p(function () { return Object.getPrototypeOf(JSON.rawJSON("1")) === null; });
p(function () { return Object.isFrozen(JSON.rawJSON("1")) + " " + Object.isExtensible(JSON.rawJSON("1")); });
p(function () { return Reflect.ownKeys(JSON.rawJSON('"x"')).join(","); });
p(function () { return desc(JSON.rawJSON("true"), "rawJSON"); });
p(function () { return typeof JSON.rawJSON(12).rawJSON + " " + JSON.rawJSON(12).rawJSON; });
p(function () { "use strict"; var r = JSON.rawJSON("1"); r.rawJSON = "2"; return r.rawJSON; });
p(function () { "use strict"; var r = JSON.rawJSON("1"); r.extra = 1; return "added"; });
p(function () { var r = JSON.rawJSON("1"); r.rawJSON = "2"; return JSON.stringify(r); });
p(function () { return Object.prototype.toString.call(JSON.rawJSON("1")); });

// --- the input it accepts, after ToString
p(function () { return JSON.rawJSON(-1.5e3).rawJSON; });
p(function () { return JSON.rawJSON(null).rawJSON + " " + JSON.rawJSON(false).rawJSON; });
p(function () { return JSON.rawJSON('"a\\u0062"').rawJSON; });
p(function () { return JSON.rawJSON({ toString: function () { return "7"; } }).rawJSON; });
p(function () { return JSON.rawJSON("1e400").rawJSON; });

// --- the input it refuses
p(function () { return JSON.rawJSON(""); });
p(function () { return JSON.rawJSON(" 1"); });
p(function () { return JSON.rawJSON("1 "); });
p(function () { return JSON.rawJSON("\t1"); });
p(function () { return JSON.rawJSON("1\n"); });
p(function () { return JSON.rawJSON("\r1"); });
p(function () { return JSON.rawJSON("{}"); });
p(function () { return JSON.rawJSON("[]"); });
p(function () { return JSON.rawJSON({}); });
p(function () { return JSON.rawJSON([1]); });
p(function () { return JSON.rawJSON(undefined); });
p(function () { return JSON.rawJSON(); });
p(function () { return JSON.rawJSON(Symbol("1")); });
p(function () { return JSON.rawJSON("01"); });
p(function () { return JSON.rawJSON("1."); });
p(function () { return JSON.rawJSON("'a'"); });
p(function () { return JSON.rawJSON("tru"); });
p(function () { return JSON.rawJSON("1 2"); });
p(function () { return JSON.rawJSON("NaN"); });
p(function () { return JSON.rawJSON("+1"); });
p(function () { return JSON.rawJSON('"a"b"'); });
p(function () { return JSON.rawJSON("\u00a01"); });
p(function () { return JSON.rawJSON({ toString: function () { throw new RangeError("t"); } }); });

// --- the brand, and what cannot forge it
p(function () { return JSON.isRawJSON(JSON.rawJSON("1")); });
p(function () { return JSON.isRawJSON({ rawJSON: "1" }); });
p(function () { var o = Object.freeze(Object.create(null, { rawJSON: { value: "1", enumerable: true } })); return JSON.isRawJSON(o); });
p(function () { return JSON.isRawJSON(Object.create(JSON.rawJSON("1"))); });
p(function () { return JSON.isRawJSON(new Proxy(JSON.rawJSON("1"), {})); });
p(function () { return [JSON.isRawJSON(), JSON.isRawJSON(1), JSON.isRawJSON("1"), JSON.isRawJSON(null)].join(","); });
p(function () { return JSON.stringify({ rawJSON: "1" }); });
p(function () { return JSON.stringify(Object.create(JSON.rawJSON("1"))); });
p(function () { return JSON.stringify(new Proxy(JSON.rawJSON("1"), {})); });

// --- stringify emits the text verbatim
p(function () { return JSON.stringify(JSON.rawJSON("1e1000")); });
p(function () { return JSON.stringify(JSON.rawJSON("-0")); });
p(function () { return JSON.stringify(JSON.rawJSON('"\\u0041"')); });
p(function () { return JSON.stringify(JSON.rawJSON("12345678901234567890")); });
p(function () { return JSON.stringify({ a: JSON.rawJSON("1"), b: [JSON.rawJSON("null"), { c: JSON.rawJSON("true") }] }); });
p(function () { return JSON.stringify([JSON.rawJSON("1"), JSON.rawJSON('"x"')], null, 2); });
p(function () { return JSON.stringify({ a: JSON.rawJSON("1"), b: 2 }, null, "--"); });

// --- replacers, and the order toJSON and the replacer see the value in
p(function () { return JSON.stringify({ a: 1, b: 2 }, function (k, v) { return k === "a" ? JSON.rawJSON("99") : v; }); });
p(function () { return JSON.stringify({ a: JSON.rawJSON("1"), b: JSON.rawJSON("2") }, ["b"]); });
p(function () { return JSON.stringify({ a: JSON.rawJSON("1") }, function (k, v) { return JSON.isRawJSON(v) ? "seen " + v.rawJSON : v; }); });
p(function () { return JSON.stringify({ a: { toJSON: function () { return JSON.rawJSON("42"); } } }); });
p(function () {
  var log = [];
  var s = JSON.stringify({ a: { toJSON: function (k) { log.push("toJSON:" + k); return JSON.rawJSON("5"); } } },
    function (k, v) { log.push("replacer:" + k + ":" + JSON.isRawJSON(v)); return v; });
  return s + " " + log.join(" ");
});
p(function () { var r = JSON.rawJSON("1"); return JSON.stringify([r, r, { r: r }]); });
p(function () { return JSON.stringify(JSON.rawJSON("1"), function (k, v) { return v; }); });
p(function () { return JSON.stringify({ a: JSON.rawJSON("1") }, function (k, v) { return k === "" ? v : undefined; }); });

// --- ordinary parse and stringify are untouched
p(function () { return JSON.stringify(JSON.parse('{"a":[1,"b",null,true]}')); });
p(function () { return JSON.stringify({ a: new Number(3), b: new String("s"), c: new Boolean(false) }); });
p(function () { return typeof JSON.parse("1") + " " + JSON.isRawJSON(JSON.parse('{"rawJSON":"1"}')); });
