// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE RESIZABLE ArrayBuffer AND THE VIEWS OVER ONE: the constructor's
// options bag, resize, resizable, maxByteLength, detached, transfer and transferToFixedLength
// (ES2026 ArrayBuffer objects, ArrayBufferCopyAndDetach); length-tracking and fixed-length typed
// arrays and DataViews that go out of bounds and recover (IsTypedArrayOutOfBounds,
// IsViewOutOfBounds, IsValidIntegerIndex); and the methods that measure a receiver once and then
// run guest code - callbacks, coercions, species constructors, iteration - with the buffer resized
// in the middle (ValidateTypedArray's len, %ArrayIteratorPrototype%.next).
//
// Retained from JSeal F04-F06: every case was checked against those steps and compared against the
// comparison engine before it was written down.
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
function rab(n, max) { return new ArrayBuffer(n, { maxByteLength: max }); }
function fill(ta) { for (var i = 0; i < ta.length; i++) ta[i] = i + 1; return ta; }

// --- the constructor and the getters
p(function () { var b = rab(4, 16); return [b.byteLength, b.maxByteLength, b.resizable, b.detached].join(); });
p(function () { var b = new ArrayBuffer(4); return [b.byteLength, b.maxByteLength, b.resizable, b.detached].join(); });
p(function () { var b = new ArrayBuffer(4, {}); return b.resizable + " " + b.maxByteLength; });
p(function () { var b = new ArrayBuffer(4, 16); return b.resizable; });
p(function () { return rab(8, 4); });
p(function () { return new ArrayBuffer(0, { maxByteLength: -1 }); });
p(function () { var log = []; new ArrayBuffer({ valueOf: function () { log.push("len"); return 1; } }, { get maxByteLength() { log.push("max"); return 2; } }); return log.join(); });
// --- resize: grow zero-fills, shrink keeps the prefix, regrowth does not bring bytes back
p(function () { var b = rab(4, 8), u = new Uint8Array(b); fill(u); b.resize(6); return Array.from(new Uint8Array(b)).join(); });
p(function () { var b = rab(4, 8), u = new Uint8Array(b); fill(u); b.resize(2); b.resize(4); return Array.from(new Uint8Array(b)).join(); });
p(function () { var b = rab(4, 8); b.resize(4); b.resize(0); return b.byteLength + " " + b.maxByteLength; });
p(function () { var b = rab(4, 8); try { b.resize(9); } catch (e) { return e.name + " " + b.byteLength; } });
p(function () { return new ArrayBuffer(4).resize(2); });
p(function () { var b = rab(4, 8); b.transfer(); try { b.resize(2); } catch (e) { return e.name + " " + b.resizable + " " + b.maxByteLength; } });
p(function () { var b = rab(4, 8); return b.resize({ valueOf: function () { b.transfer(); return 1; } }); });
// --- a length-tracking typed array follows the buffer
p(function () { var b = rab(4, 16), a = new Int16Array(b); var r = [a.length]; b.resize(9); r.push(a.length, a.byteLength); b.resize(1); r.push(a.length, a.byteLength); return r.join(); });
p(function () { var b = rab(8, 16), a = new Int16Array(b, 4); b.resize(3); return [a.length, a.byteOffset, a.byteLength, a[0]].join(); });
p(function () { var b = rab(8, 16), a = new Int16Array(b, 4); b.resize(3); b.resize(10); return [a.length, a.byteOffset, a[0], a[2]].join(); });
p(function () { var b = rab(8, 16), a = new Int16Array(b, 8); return a.length + " " + a.byteOffset; });
// --- a fixed-length typed array goes out of bounds and recovers over the current bytes
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); fill(a); b.resize(5); return [a.length, a.byteOffset, a.byteLength, a[0], 0 in a, Object.keys(a).length].join(); });
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); fill(a); b.resize(5); b.resize(8); return Array.from(a).join(); });
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); b.resize(5); return a.fill(1); });
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); b.resize(5); a[0] = 9; b.resize(8); return a[0]; });
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); b.resize(5); return Object.defineProperty(a, "0", { value: 1 }); });
p(function () { var b = rab(8, 16), a = new Uint8Array(b, 2, 4); b.resize(5); return [...a]; });
// --- the constructor's buffer form
p(function () { var b = rab(7, 16); return new Int16Array(b).length; });
p(function () { return new Int16Array(new ArrayBuffer(7)); });
p(function () { var b = rab(8, 16); return new Int16Array(b, 10); });
p(function () { var b = rab(8, 16); return new Int16Array(b, 2, 4); });
p(function () { var b = rab(8, 16), src = new Uint8Array(b, 2, 4); b.resize(4); return new Uint8Array(src); });
// --- DataView: tracking, out of bounds, recovery
p(function () { var b = rab(4, 16), d = new DataView(b, 1); var r = [d.byteLength]; b.resize(10); r.push(d.byteLength); return r.join(); });
p(function () { var b = rab(8, 16), d = new DataView(b, 2, 4); b.resize(5); try { d.byteLength; } catch (e) { return e.name; } });
p(function () { var b = rab(8, 16), d = new DataView(b, 2, 4); b.resize(5); return d.byteOffset; });
p(function () { var b = rab(8, 16), d = new DataView(b, 2, 4); b.resize(5); return d.getUint8(0); });
p(function () { var b = rab(8, 16), d = new DataView(b, 2, 4); b.resize(5); b.resize(6); return d.getUint8(0) + " " + d.byteLength; });
p(function () { var b = rab(4, 16), d = new DataView(b); return d.getUint8(4); });
p(function () { var b = rab(4, 16), d = new DataView(b); b.resize(5); d.setUint8(4, 7); return new Uint8Array(b)[4]; });
p(function () { var b = rab(4, 16), d = new DataView(b, 2); return d.setUint8(0, { valueOf: function () { b.resize(1); return 1; } }); });
p(function () { var b = rab(4, 16); return new DataView(b, 4).byteLength; });
p(function () { var b = rab(4, 16); var nt = function () {}.bind(); Object.defineProperty(nt, "prototype", { get: function () { b.resize(1); return DataView.prototype; } }); return Reflect.construct(DataView, [b, 2, 2], nt); });
// --- methods that measure once: a callback shrinks or grows the buffer
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)), seen = []; a.forEach(function (v, i) { if (i === 0) b.resize(2); seen.push(v); }); return seen.join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)), n = 0; a.forEach(function (v, i) { if (i === 0) b.resize(8); n++; }); return n; });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.map(function (v, i) { if (i === 1) b.resize(2); return v === undefined ? 99 : v; }).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.filter(function (v, i) { if (i === 0) b.resize(2); return true; }).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.reduce(function (acc, v, i) { if (i === 1) b.resize(2); return acc + "," + v; }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.findLast(function (v, i) { if (i === 3) b.resize(2); return v === undefined; }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.every(function (v, i) { if (i === 0) b.resize(2); return i < 2 || v === undefined; }); });
// --- methods that measure once: a coercion shrinks or grows the buffer
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.includes(undefined, { valueOf: function () { b.resize(2); return 0; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.indexOf(undefined, { valueOf: function () { b.resize(2); return 0; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.lastIndexOf(0, { valueOf: function () { b.resize(8); return -1; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.join({ toString: function () { b.resize(2); return "-"; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.at({ valueOf: function () { b.resize(2); return 3; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); a.fill(7, { valueOf: function () { b.resize(2); return 0; } }); return Array.from(new Uint8Array(b)).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b, 0, 4)); return a.fill(7, { valueOf: function () { b.resize(2); return 0; } }); });
p(function () { var b = rab(6, 8), a = fill(new Uint8Array(b)); a.copyWithin(0, { valueOf: function () { b.resize(4); return 2; } }); return Array.from(a).join(); });
p(function () { var b = rab(6, 8), a = fill(new Uint8Array(b)); a.copyWithin(2, { valueOf: function () { b.resize(4); return 0; } }); return Array.from(a).join(); });
p(function () { var b = rab(4, 8), a = fill(new Float64Array(b = rab(32, 64))); return Array.from(a.with(0, { valueOf: function () { b.resize(16); return 9; } })).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); return a.with(3, { valueOf: function () { b.resize(2); return 9; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)), s = { 0: 8, 1: 9, get length() { b.resize(2); return 2; } }; a.set(s, 1); return Array.from(new Uint8Array(b)).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); a.set([5], { valueOf: function () { b.resize(1); return 1; } }); return Array.from(a).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b, 0, 4)); return a.set([5], { valueOf: function () { b.resize(1); return 1; } }); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); a.sort(function (x, y) { b.resize(2); return y - x; }); return Array.from(new Uint8Array(b)).join(); });
// --- species construction resizes the receiver
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); a.constructor = {}; a.constructor[Symbol.species] = function (n) { b.resize(2); return new Uint8Array(n); }; return Array.from(a.slice(0, 4)).join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b, 0, 4)); a.constructor = {}; a.constructor[Symbol.species] = function (n) { b.resize(2); return new Uint8Array(n); }; return a.slice(0, 4); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); var s = a.subarray(1); b.resize(6); return s.length; });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)); var s = a.subarray(1, 3); b.resize(6); return s.length; });
p(function () { var b = rab(8, 16), c = new ArrayBuffer(8); var x = new Uint8Array(b); x.set([1, 2, 3, 4, 5, 6, 7, 8]); b.constructor = {}; b.constructor[Symbol.species] = function (n) { b.resize(3); return c; }; return Array.from(new Uint8Array(b.slice(1, 6))).join() + "|" + Array.from(new Uint8Array(c)).join(); });
// --- iteration: a shrink under an iterator is a TypeError, a growth is followed
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b, 0, 4)), r = []; try { for (var v of a) { r.push(v); if (v === 2) b.resize(3); } } catch (e) { r.push(e.name); } return r.join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)), r = []; for (var v of a) { r.push(v); if (v === 2) b.resize(6); } return r.join(); });
p(function () { var b = rab(4, 8), a = fill(new Uint8Array(b)), r = []; for (var v of a) { r.push(v); if (v === 2) b.resize(3); } return r.join(); });
// --- transfer and transferToFixedLength
p(function () { var b = rab(4, 16); new Uint8Array(b)[1] = 5; var c = b.transfer(); return [b.detached, c.resizable, c.maxByteLength, c.byteLength, new Uint8Array(c)[1]].join(); });
p(function () { var b = rab(4, 16); var c = b.transfer(20); return c.byteLength; });
p(function () { var b = rab(4, 16); var c = b.transferToFixedLength(6); new Uint8Array(b); return [b.detached, c.resizable, c.maxByteLength, c.byteLength].join(); });
p(function () { var b = new ArrayBuffer(4); var c = b.transfer(); return c.resizable + " " + b.detached; });
p(function () { var b = rab(4, 16); b.transfer(); return b.transferToFixedLength(); });
p(function () { var b = rab(4, 16); return b.transfer({ valueOf: function () { b.transfer(); return 1; } }); });
// --- the view getters do not throw over a detached resizable buffer
p(function () { var b = rab(4, 16), a = new Uint8Array(b); b.transfer(); return [a.length, a.byteOffset, a.byteLength].join(); });
// --- the typed array object model: keys and descriptors follow the buffer
p(function () { var b = rab(2, 8), a = new Uint8Array(b); b.resize(3); return Reflect.ownKeys(a).join(); });
p(function () { var b = rab(2, 8), a = new Uint8Array(b); b.resize(1); return delete a[1]; });
p(function () { var b = rab(2, 8), a = new Uint8Array(b); b.resize(3); a[2] = 5; return a[2] + " " + (2 in a); });
// --- the DataView constructor measures the buffer before converting byteLength (steps 5, 9.b, 11)
p(function () { var b = rab(4, 16); return new DataView(b, 0, { valueOf: function () { b.resize(8); return 6; } }).byteLength; });
p(function () { var b = new ArrayBuffer(4); return new DataView(b, 0, { valueOf: function () { b.transfer(); return 1; } }); });
p(function () { var b = rab(4, 16); return new DataView(b, 0, { valueOf: function () { b.resize(2); return 3; } }); });
// --- [[PreventExtensions]] refuses for every typed array over a resizable buffer (IsTypedArrayFixedLength)
p(function () { var b = rab(4, 8); return Reflect.preventExtensions(new Uint8Array(b)) + " " + Reflect.preventExtensions(new Uint8Array(b, 0, 2)) + " " + Reflect.preventExtensions(new Uint8Array(new ArrayBuffer(4))); });
p(function () { var b = rab(0, 8), a = new Uint8Array(b, 0, 0); try { Object.preventExtensions(a); } catch (e) { return e.name + " " + Object.isExtensible(a); } });
p(function () { var a = new Uint8Array(rab(0, 8)); try { Object.freeze(a); } catch (e) { a.buffer.resize(2); a[1] = 5; return e.name + " " + Object.isFrozen(a) + " " + Object.isExtensible(a) + " " + a[1]; } });
p(function () { var a = new Uint8Array(rab(0, 8)); try { Object.seal(a); } catch (e) { return e.name + " " + Object.isSealed(a); } });
p(function () { var b = rab(4, 8), a = new Uint8Array(b, 0, 2); b.transfer(); return Reflect.preventExtensions(a); });
p(function () { return Reflect.preventExtensions(new Proxy(new Uint8Array(rab(0, 8)), {})); });
p(function () { var d = new DataView(rab(4, 8)); Object.preventExtensions(d); return Object.isExtensible(d); });
p(function () { var a = Object.freeze(new Uint8Array(0)); return Object.isFrozen(a) + " " + Object.isExtensible(a); });
