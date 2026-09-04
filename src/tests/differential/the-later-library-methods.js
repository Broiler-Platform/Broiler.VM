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
