// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE LIBRARY METHODS ADDED AFTER THE FIRST PASS.
//
// `Array.from` over the iteration protocol, `Array.prototype.at`, `flat`, `flatMap`,
// `findLast`, `findLastIndex` and `copyWithin`, the four change-by-copy methods, `Math.cbrt`
// over a perfect cube, `String.prototype.normalize`, `String.fromCodePoint` and `String.raw`.
// It exists beside the general probe rather than inside it so that the cases which found a
// gap stay legible as the cases that found it.
//
// Each case prints its own number, so a divergence names a case rather than a line, and the
// numbering survives a case being rewritten in place. A case that throws prints the error's name,
// because a refusal is an answer and a probe that stopped at the first one would compare nothing
// after it.

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }
p(function () { return Array.from("\u{1F600}").length; });
p(function () { return Array.from(new Set([1,2,2])).join(","); });
p(function () { return Array.from(new Map([[1,2]])).length; });
p(function () { return Array.from("abc", function (c, i) { return c + i; }).join(","); });
p(function () { return Array.from({length:2, 0:"a", 1:"b"}).join(","); });
p(function () { return Array.from({length:2}).length; });
p(function () { return [1,2,3].at(-1); });
p(function () { return String([1,2,3].at(5)); });
p(function () { return [1,2,3].at("1"); });
p(function () { return [1,[2,[3,[4]]]].flat(2).join(","); });
p(function () { return [1,[2]].flat(Infinity).join(","); });
p(function () { return [1,,2].flat().length; });
p(function () { return [1,2].flatMap(function (x) { return [x, x*2]; }).join(","); });
p(function () { return [1,2].flatMap(function (x) { return x; }).join(","); });
p(function () { return [1,2,3].findLast(function (x) { return x < 3; }); });
p(function () { return [1,2,3].findLastIndex(function (x) { return x < 0; }); });
p(function () { return [,1].findLast(function (x) { return x === undefined; }); });
p(function () { return [1,2,3,4,5].copyWithin(0,3).join(","); });
p(function () { return [1,2,3,4,5].copyWithin(1,3,4).join(","); });
p(function () { return [1,2,3,4,5].copyWithin(-2,-3,-1).join(","); });
p(function () { return [1,2,3].copyWithin(0,-2,-1).join(","); });
p(function () { return [3,1,2].toSorted().join(","); });
p(function () { var a = [3,1,2]; a.toSorted(); return a.join(","); });
p(function () { return [3,1,2].toSorted(function (a,b) { return a - b; }).join(","); });
p(function () { return [,1].toReversed().length + ":" + String([,1].toReversed()[1]); });
p(function () { return [1,2,3].toReversed().join(","); });
p(function () { return [1,2,3].with(1, 9).join(","); });
p(function () { return [1,2,3].with(-1, 9).join(","); });
p(function () { return t(function () { return [1,2,3].with(5, 9); }); });
p(function () { return [1,2,3].toSpliced(1,1).join(","); });
p(function () { return [1,2,3].toSpliced(1,0,"a","b").join(","); });
p(function () { return [1,2,3].toSpliced(-1).join(","); });
p(function () { return [undefined,1].toSorted().join(","); });
p(function () { return Math.cbrt(27); });
p(function () { return Math.cbrt(-8); });
p(function () { return Math.cbrt(2); });
p(function () { return Math.cbrt(0); });
p(function () { return Math.cbrt(1e30); });
p(function () { return "abc".normalize(); });
p(function () { return "abc".normalize("NFD"); });
p(function () { return t(function () { return "abc".normalize("NFX"); }); });
p(function () { return String.fromCodePoint(0x1F600).length; });
p(function () { return t(function () { return String.fromCodePoint(-1); }); });
p(function () { return String.raw({raw: ["a","b"]}, 1); });
p(function () { return String.raw({raw: "xy"}, 1); });
p(function () { return typeof String.raw; });
p(function () { return String.raw.length; });
p(function () { return [1,2,3].at.length + "," + [].flat.length + "," + [].flatMap.length; });
p(function () { return [].with.length + "," + [].toSpliced.length + "," + [].toSorted.length; });
p(function () { return Object.getOwnPropertyDescriptor(Array.prototype, "at").enumerable; });

// --- an Array's `length` when it has been closed, and the integrity levels around it.
// APPENDED rather than inserted, because a case number is how a divergence is named and
// renumbering the cases above would silently move every declaration that points at one.
p(function(){ var a=[1]; Object.defineProperty(a,"length",{value:1,writable:false}); return t(function(){a.push(2);}); });
p(function(){ var a=[1]; Object.defineProperty(a,"length",{value:1,writable:false}); return a.length; });
p(function(){ var a=[1]; Object.defineProperty(a,"length",{value:1,writable:false}); a[5]=1; return a.length+":"+String(a[5]); });
p(function(){ var a=[1]; Object.defineProperty(a,"length",{value:1,writable:false}); a[0]=9; return a[0]; });
p(function(){ var a=[1]; Object.defineProperty(a,"length",{value:1,writable:false}); return JSON.stringify(Object.getOwnPropertyDescriptor(a,"length")); });
p(function(){ var a=[1,2,3]; return JSON.stringify(Object.getOwnPropertyDescriptor(a,"length")); });
p(function(){ var a=[1]; return t(function(){ Object.defineProperty(a,"length",{value:0,writable:false}); return a.length; }); });
p(function(){ var a=Object.freeze([1]); return t(function(){ a.push(2); }); });
p(function(){ var a=Object.freeze([1]); return t(function(){ a[0]=2; return a[0]; }); });
p(function(){ var a=[1,2]; a.length=1; return a.join(","); });
p(function(){ var a=[1,2]; return a.push(3); });
p(function(){ var a=[1,2,3]; a.length=0; return a.length; });
p(function(){ var a=Object.seal([1,2]); return t(function(){ a.push(3); }); });
p(function(){ var a=Object.seal([1,2]); a[0]=9; return a[0]; });
p(function(){ var a=[1,2,3]; return a.splice(1,1).join(","); });
p(function(){ var a=[1,2,3]; a.reverse(); return a.join(","); });
p(function(){ var a=[3,1,2]; a.sort(); return a.join(","); });
p(function(){ var a=[1,2,3]; a.fill(0,1); return a.join(","); });
p(function(){ var a=[1,2,3]; a.copyWithin(0,1); return a.join(","); });
p(function(){ var a=[1,2,3]; return a.pop()+":"+a.length; });
p(function(){ var a=[1,2,3]; return a.shift()+":"+a.join(","); });
p(function(){ var a=[1,2]; return a.unshift(0)+":"+a.join(","); });

// --- what is iterable, once the realm has an iteration protocol at all. APPENDED rather
// than inserted, for the reason the block above gives: a case number is how a divergence is
// named.
function* pairs(){ yield [1,"a"]; yield [2,"b"]; }
p(function(){ return new Map(pairs()).get(2); });
p(function(){ return new Set(function*(){ yield 1; yield 1; yield 2; }()).size; });
p(function(){ return new Map([[1,2]]).get(1); });
p(function(){ return new Set([1,2,2]).size; });
p(function(){ return new Set("abc").size; });
p(function(){ var o = { [Symbol.iterator]: function(){ var i=0; return { next: function(){ return i<2 ? {value:i++,done:false} : {done:true}; } }; } }; return new Set(o).size; });
p(function(){ return new Map(new Map([[1,2]])).get(1); });
p(function(){ return t(function(){ return new Map(5); }); });
p(function(){ return t(function(){ return new Set({}); }); });
p(function(){ return new WeakSet([{}]) instanceof WeakSet; });
p(function(){ return [...new Uint8Array([1,2,3])].join(","); });
p(function(){ var s=0; for (const b of new Uint8Array([1,2])) s+=b; return s; });
p(function(){ return Array.from(new Int16Array([4,5])).join(","); });
p(function(){ return [...new Uint8Array([7]).entries()].map(function(e){return e.join(":");}).join(","); });
p(function(){ return [...new Uint8Array([7]).keys()].join(","); });
p(function(){ return new Uint8Array(1).values === new Uint8Array(1)[Symbol.iterator]; });
p(function(){ return new Set(new Uint8Array([1,2,2])).size; });
p(function(){ return [...new Uint8Array(0)].length; });
p(function(){ return Object.prototype.toString.call(new Uint8Array(1)[Symbol.iterator]()); });
p(function(){ function* d(){ yield* new Uint8Array([8,9]); } return [...d()].join(","); });
p(function(){ return [...new Map([[1,2]])].map(function(e){return e.join("=");}).join(","); });

// --- the keyed collections, the promise as a value, the views and JSON, in one pass. APPENDED
// rather than inserted, for the reason the file's own numbering asks: a case number is how a
// divergence is named. It is the pass that found a Map iterator yielding its last entry twice
// after a delete, and six members the views did not have.
p(function(){ var m = new Map([[1,"a"],[2,"b"]]); return m.size + "," + m.get(1); });
p(function(){ var m = new Map(); m.set(NaN, 1); return m.get(NaN); });
p(function(){ var m = new Map(); m.set(-0, "z"); return m.get(0); });
p(function(){ var m = new Map([[1,1]]); var out=[]; m.forEach(function(v,k,mm){ out.push(k+":"+v+":"+(mm===m)); }); return out.join(); });
p(function(){ var m = new Map([[1,1],[2,2]]); m.delete(1); return [...m.keys()].join(); });
p(function(){ var s = new Set([1,2,2,3]); return s.size + "," + [...s].join(); });
p(function(){ var s = new Set([1,2]); return [...s.entries()].map(function(e){return e.join("-");}).join(); });
p(function(){ var m = new Map(); return String(m.get("missing")) + "," + m.has("missing"); });
p(function(){ var m = new Map([[1,1]]); m.clear(); return m.size; });
p(function(){ return Object.prototype.toString.call(new Map()) + new Map()[Symbol.toStringTag]; });
p(function(){ var m = new Map(); var r = m.set(1,1); return String(r === m); });
p(function(){ var s = new Set(); return String(s.add(1) === s); });
p(function(){ var wm = new WeakMap(); var k = {}; wm.set(k, 1); return wm.get(k) + "," + wm.has({}); });
p(function(){ try { new WeakMap().set(1, 1); return "no-throw"; } catch(e) { return e.name; } });
p(function(){ var ws = new WeakSet(); var o = {}; ws.add(o); return String(ws.has(o)); });
p(function(){ var m = new Map([[1,1],[2,2],[3,3]]); var seen=[]; for (var e of m) seen.push(e[0]); return seen.join(); });
p(function(){ var m = new Map(); m.set("a",1); m.set("b",2); m.delete("a"); m.set("c",3); return [...m.keys()].join(); });
p(function(){ return Map.length + "," + Set.length + "," + Map.prototype.set.length; });
p(function(){ try { Map(); return "no-throw"; } catch(e) { return e.name; } });
p(function(){ var m = new Map([[1,1]]); var it = m[Symbol.iterator](); return String(it.next().value); });
p(function(){ return typeof Promise.resolve().then; });
p(function(){ return String(Promise.resolve(1) instanceof Promise); });
p(function(){ return Promise.length + "," + Promise.prototype.then.length; });
p(function(){ var p1 = Promise.resolve(1); return String(Promise.resolve(p1) === p1); });
p(function(){ return typeof Promise.allSettled + typeof Promise.any + typeof Promise.race; });
p(function(){ return Object.prototype.toString.call(Promise.resolve()); });
p(function(){ var a = new Int8Array([1,-2,3]); return a.length + "," + a[1]; });
p(function(){ var a = new Uint8Array(3); a[0] = 300; return a[0]; });
p(function(){ var a = new Uint8ClampedArray(2); a[0] = 300; a[1] = -5; return a[0] + "," + a[1]; });
p(function(){ var a = new Float64Array([1.5]); return a[0]; });
p(function(){ var a = new Int32Array([3,1,2]); return a.sort().join(); });
p(function(){ var a = new Int32Array([1,2,3]); return a.subarray(1).join(); });
p(function(){ var b = new ArrayBuffer(8); var v = new DataView(b); v.setInt32(0, 123456); return v.getInt32(0); });
p(function(){ var b = new ArrayBuffer(4); var v = new DataView(b); v.setInt16(0, 1, true); return v.getInt16(0, false); });
p(function(){ var a = new Int16Array(4); return a.byteLength + "," + a.BYTES_PER_ELEMENT; });
p(function(){ return Object.prototype.toString.call(new Int8Array(1)); });
p(function(){ var a = new Int8Array([1,2,3]); return [...a].join(); });
p(function(){ var a = Int8Array.from([1,2,3], function(x){ return x*2; }); return a.join(); });
p(function(){ return Int8Array.of(1,2,3).join(); });
p(function(){ var a = new Int8Array(3); a.fill(7); return a.join(); });
p(function(){ var a = new Int8Array([1,2,3,4]); return a.slice(1,3).join() + "|" + a.join(); });
p(function(){ var a = new Uint8Array([1,2,3]); return a.reduce(function(x,y){return x+y;}, 0); });
p(function(){ var a = new Uint8Array([1,2,3]); return a.indexOf(2) + "," + a.includes(9); });
p(function(){ var a = new Int8Array([3,1,2]); return a.toSorted ? a.toSorted().join() : "absent"; });
p(function(){ var b = new ArrayBuffer(4); return b.byteLength + "," + (b.slice ? b.slice(1).byteLength : "no-slice"); });
p(function(){ return JSON.stringify({a:[1,{b:2}],c:"x"}); });
p(function(){ return JSON.stringify({a:1}, null, 2); });
p(function(){ return JSON.stringify([undefined, function(){}, Symbol()]); });
p(function(){ return JSON.stringify({u: undefined, f: function(){}}); });
p(function(){ return JSON.stringify({d: new Date(0)}); });
p(function(){ return JSON.stringify({a:1,b:2}, ["a"]); });
p(function(){ return JSON.stringify({a:1}, function(k,v){ return typeof v === "number" ? v*2 : v; }); });
p(function(){ return JSON.parse('{"a":[1,2]}').a[1]; });
p(function(){ return JSON.parse('"\\u00e9"'); });
p(function(){ try { JSON.parse("{"); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ var seen = []; JSON.parse('{"a":1}', function(k,v){ seen.push(k); return v; }); return seen.join("|"); });
p(function(){ var o = {}; o.self = o; try { JSON.stringify(o); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ return JSON.stringify("a b"); });
p(function(){ return JSON.stringify({toJSON: function(){ return 42; }}); });
p(function(){ return JSON.stringify(Infinity) + "," + JSON.stringify(NaN); });
p(function(){ return String(JSON.stringify(undefined)); });
p(function(){ return JSON.stringify([1,2], null, "\t"); });
p(function(){ return JSON.stringify({a:new Map()}); });
p(function(){ return Object.getOwnPropertyNames(Object.getPrototypeOf(Int8Array.prototype)).sort().join(","); });
p(function(){ return Object.getOwnPropertyNames(Int8Array).sort().join(","); });
p(function(){ return String(Int8Array.from === Uint8Array.from); });
p(function(){ return Int8Array.from([1,2,3]).join(); });
p(function(){ return Uint8Array.from(new Set([1,2,3])).join(); });
p(function(){ return Int8Array.from([1,2], function(x){ return x*3; }).join(); });
p(function(){ return Int16Array.of(1,2,3).join() + "," + Int16Array.of(1,2,3).BYTES_PER_ELEMENT; });
p(function(){ return Int8Array.from({length:2, 0:5, 1:6}).join(); });
p(function(){ var a = new Int8Array([3,1,2]); return a.toSorted().join() + "|" + a.join(); });
p(function(){ var a = new Int8Array([1,2,3]); return a.toReversed().join() + "|" + a.join(); });
p(function(){ var a = new Int8Array([1,2,3]); return a.with(1, 9).join() + "|" + a.join(); });
p(function(){ var a = new Int8Array([1,2,3]); return a.with(-1, 9).join(); });
p(function(){ var a = new Int8Array([1,2,3]); try { a.with(5, 1); return "no-throw"; } catch(e){ return e.name; } });
p(function(){ var a = new Int8Array([1,2,3]); return String(a.findLast(function(x){ return x < 3; })); });
p(function(){ var a = new Int8Array([1,2,3]); return String(a.findLastIndex(function(x){ return x < 0; })); });
p(function(){ var a = new Int8Array([1,2]); return a.toLocaleString(); });
p(function(){ var a = new Int8Array([3,1,2]); return String(a.toSorted() instanceof Int8Array); });
p(function(){ return typeof Int8Array.prototype.toSpliced; });
p(function(){ return Int8Array.from.length + "," + Int8Array.of.length; });
p(function(){ try { var f = Int8Array.from; return f.call({}, [1]).join(); } catch(e){ return e.name; } });
