// A String key deleted and defined again is a new property, created last, so `Object.keys`,
// `for … in` and `JSON.stringify` list it after the keys that stayed. Until 2026-10-03 it took its
// old place back (JSC-247).
var r = [];
var o = { p1: 1, p2: 2, p3: 3 };
o.p4 = 4; o[2] = 0; o[0] = 0;
delete o.p1; delete o.p3; o.p1 = 1;
var keys = []; for (var key in o) keys.push(key);
r.push(keys.join());
var big = {};
for (var i = 0; i < 40; i++) big["k" + i] = i;
for (var j = 0; j < 30; j++) delete big["k" + j];
big.k5 = 5; big.k35 = 99;
r.push(Object.keys(big).join(), big.k35, "k10" in big);
var churn = { a: 1, b: 2 };
for (var n = 0; n < 1000; n++) { delete churn.a; churn.a = n; }
r.push(Object.keys(churn).join(), churn.a);
var t = { y: 0, x: 0 }; delete t.y; t.y = 3;
r.push(JSON.stringify(t));
var d = { a: 1 };
Object.defineProperty(d, "b", { value: 2, enumerable: true, configurable: true });
delete d.a;
Object.defineProperty(d, "a", { value: 3, enumerable: true });
r.push(Object.keys(d).join());
r.join(" / ")
