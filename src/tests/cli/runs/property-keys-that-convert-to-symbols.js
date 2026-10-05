// A property key is converted once by ToPropertyKey, which keeps a Symbol an object converts to,
// and `hasOwnProperty` and `propertyIsEnumerable` convert it before the receiver. Until
// 2026-10-03 these members converted an object key to a String, so a key whose `toString` answered
// a Symbol threw a TypeError, and `Object.defineProperty` read the descriptor before the key
// (JSC-248).
var r = [], sym = Symbol("s"), calls = 0;
var w = { toString() { calls++; return sym; }, valueOf() { throw new Error("valueOf"); } };
var o = { [sym]: 1 };
r.push(o.hasOwnProperty(w), o.propertyIsEnumerable(w), Object.hasOwn(o, w));
function first(f) { try { f(); return "none"; } catch (e) { return e.name; } }
var throwing = { toString() { throw new RangeError("key"); } };
r.push(first(() => Object.prototype.hasOwnProperty.call(undefined, throwing)));
r.push(first(() => Object.prototype.propertyIsEnumerable.call(null, throwing)));
r.push(Object.getOwnPropertyDescriptor(o, w).value, Reflect.get(o, w), Reflect.has(o, w));
r.push(Reflect.set(o, w, 5), o[sym], Reflect.defineProperty(o, w, { value: 7 }), o[sym]);
r.push(Reflect.getOwnPropertyDescriptor(o, w).value, Reflect.deleteProperty(o, w), sym in o);
r.push(Object.fromEntries([[w, 3]])[sym]);
var groups = Object.groupBy([1, 2, 3], x => x % 2 ? sym : "even");
r.push(groups[sym].join("+"), groups.even.join("+"));
var order = [];
Object.defineProperty({}, { toString() { order.push("key"); return "k"; } }, { get value() { order.push("descriptor"); return 1; } });
r.push(order.join(">"));
var h = {};
h.__defineGetter__(w, function () { return "got"; });
r.push(h[sym], h.__lookupGetter__(w)(), calls);
r.join(" ")
