// Twelve built-in behaviours that answered otherwise until 2026-10-03 (JSC-252): `Number.parseInt`
// and `Number.parseFloat` are the global functions; `String.prototype.toString` and `valueOf` do
// not coerce; `Date.prototype[Symbol.toPrimitive]` does not convert its hint; `Date.parse` reads
// an expanded year; an Error's `cause` is asked through HasProperty; `Object` called as a super
// constructor ignores its argument; a RegExp lists `lastIndex` first; `Object.fromEntries` closes
// its iterator on a bad entry; `Promise.any` hands each element the capability's own resolve; a
// script whose completion value cannot be converted completed all the same.
var r = [];
function t(f) { try { return String(f()); } catch (e) { return e.name; } }
r.push(Number.parseInt === parseInt, Number.parseFloat === parseFloat);
r.push(t(() => String.prototype.toString.call({ toString() { return "s"; } })), t(() => String.prototype.valueOf.call(1)), new String("x").toString());
r.push(t(() => new Date(0)[Symbol.toPrimitive](new String("number"))), new Date(0)[Symbol.toPrimitive]("number"));
r.push(Date.parse("-000001-07-01T00:00Z"), Date.parse("+275760-09-13T00:00:00.000Z"), Date.parse("-000000-01-01T00:00Z"));
r.push(new Date("-000012-07-01T00:00Z").toString().split(" ")[3]);
r.push(t(() => new Error("m", new Proxy({}, { has(target, key) { if (key === "cause") throw new RangeError("has"); return key in target; } }))), new Error("m", Object.create({ cause: 5 })).cause);
class O extends Object {}
r.push(String(new O({ a: 1 }).a), Object.getPrototypeOf(new O()) === O.prototype, new Object({ c: 3 }).c);
var re = /(?:)/g; re.a = 1; Object.defineProperty(re, "lastIndex", { value: 2 });
r.push(Reflect.ownKeys(re).join("+"));
var log = [];
var entries = { [Symbol.iterator]() { var i = 0; return { next() { log.push("next" + i); return { done: i > 1, value: i++ === 0 ? ["a", 1] : null }; }, return() { log.push("return"); return {}; } }; } };
r.push(t(() => Object.fromEntries(entries)), log.join("+"));
var calls = 0, settle;
function Capability(executor) { executor(function () { calls++; }, function () {}); }
Capability.resolve = function (v) { return v; };
Promise.any.call(Capability, [{ then(resolve) { settle = resolve; } }]);
settle(1); settle(2); r.push(calls);
r.push(t(() => /[\c0]/.test("\x10")), t(() => new RegExp("[\\c0]", "u")));
r.push(new RegExp("(".repeat(200) + "a" + ")".repeat(200)).exec("a").length);
r.join(" / ")
