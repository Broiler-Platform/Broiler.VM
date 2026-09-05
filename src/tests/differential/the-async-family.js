// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER `async` FUNCTIONS AND `await`.
//
// Retained from the bring-up of the family it covers: every case was compared against the
// comparison engine before it was written down, and each prints its own number so a divergence
// names a case rather than a line.

var log = [];
function say(x) { print(x); }

// ---- 1. the call starts the body synchronously ------------------------------------------------
(function () {
  var order = [];
  async function f() { order.push("body"); await 0; order.push("after"); }
  order.push("before");
  f();
  order.push("call-returned");
  say("1 " + order.join(","));
})();

// ---- 2. the call answers a promise ------------------------------------------------------------
(function () {
  async function f() { return 1; }
  var p = f();
  say("2 " + (p instanceof Promise) + " " + Object.prototype.toString.call(p) + " " + typeof p.then);
})();

// ---- 3. an async function is not a constructor ------------------------------------------------
(function () {
  async function f() {}
  var caught = "none";
  try { new f(); } catch (e) { caught = e.constructor.name; }
  say("3 " + caught + " " + (f.prototype === undefined) + " " + f.length + " " + f.name);
})();

// ---- 4. the function's own prototype chain -----------------------------------------------------
(function () {
  async function f() {}
  var proto = Object.getPrototypeOf(f);
  say("4 " + (proto === Function.prototype) + " " +
      Object.prototype.toString.call(f) + " " +
      (Object.getPrototypeOf(proto) === Function.prototype) + " " +
      proto.constructor.name);
})();

// ---- 5. typeof an async function ---------------------------------------------------------------
(function () {
  async function f() {}
  var g = async () => 1;
  say("5 " + typeof f + " " + typeof g + " " + Object.prototype.toString.call(g));
})();

// ---- 6. await 0 costs exactly one turn ---------------------------------------------------------
(function () {
  var order = [];
  async function f() { await 0; order.push("a1"); await 0; order.push("a2"); }
  f();
  Promise.resolve().then(function () { order.push("t1"); })
    .then(function () { order.push("t2"); })
    .then(function () { say("6 " + order.join(",")); });
})();

// ---- 7. awaiting a native promise ---------------------------------------------------------------
(function () {
  var order = [];
  async function f() { await Promise.resolve(); order.push("a"); }
  f();
  Promise.resolve().then(function () { order.push("1"); })
    .then(function () { order.push("2"); })
    .then(function () { order.push("3"); })
    .then(function () { say("7 " + order.join(",")); });
})();

// ---- 8. awaiting a thenable ----------------------------------------------------------------------
(function () {
  var order = [];
  var thenable = { then: function (resolve) { order.push("then-called"); resolve(42); } };
  async function f() { var v = await thenable; order.push("got:" + v); }
  f();
  Promise.resolve().then(function () { order.push("1"); })
    .then(function () { order.push("2"); })
    .then(function () { order.push("3"); })
    .then(function () { order.push("4"); })
    .then(function () { say("8 " + order.join(",")); });
})();

// ---- 9. a thenable whose then itself awaits ------------------------------------------------------
(function () {
  var order = [];
  var thenable = {
    then: async function (resolve) { order.push("then-entered"); await 0; order.push("then-resumed"); resolve(7); },
  };
  async function f() { order.push("got:" + (await thenable)); }
  f();
  Promise.resolve().then(function () { order.push("1"); })
    .then(function () { order.push("2"); })
    .then(function () { order.push("3"); })
    .then(function () { order.push("4"); })
    .then(function () { order.push("5"); })
    .then(function () { say("9 " + order.join(",")); });
})();

// ---- 10. return resolves ---------------------------------------------------------------------------
(function () {
  async function f() { return 5; }
  f().then(function (v) { say("10 " + v); });
})();

// ---- 11. throwing before the first await rejects ----------------------------------------------------
(function () {
  async function f() { throw new TypeError("boom"); }
  var reached = false;
  try { f().then(null, function (e) { say("11 " + reached + " " + e.name + " " + e.message); }); reached = true; }
  catch (e) { say("11 threw synchronously " + e); }
})();

// ---- 12. throwing after an await rejects ------------------------------------------------------------
(function () {
  async function f() { await 0; throw new RangeError("later"); }
  f().catch(function (e) { say("12 " + e.name + " " + e.message); });
})();

// ---- 13. awaiting a rejected promise raises at the await ---------------------------------------------
(function () {
  async function f() {
    try { await Promise.reject(new Error("rejected")); return "no"; }
    catch (e) { return "caught:" + e.message; }
  }
  f().then(function (v) { say("13 " + v); });
})();

// ---- 14. an await in a finally ------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() {
    try { order.push("try"); return "returned"; }
    finally { order.push("finally-in"); await 0; order.push("finally-out"); }
  }
  f().then(function (v) { say("14 " + order.join(",") + " -> " + v); });
})();

// ---- 15. a finally that overrides the return ----------------------------------------------------------
(function () {
  async function f() {
    try { return "a"; }
    finally { await 0; return "b"; }
  }
  f().then(function (v) { say("15 " + v); });
})();

// ---- 16. an await in a loop -----------------------------------------------------------------------------
(function () {
  async function f() {
    var total = 0;
    for (var i = 0; i < 5; i++) { total += await i; }
    return total;
  }
  f().then(function (v) { say("16 " + v); });
})();

// ---- 17. an await in a while with a break ----------------------------------------------------------------
(function () {
  async function f() {
    var seen = [];
    var i = 0;
    while (true) { if (i === 3) break; seen.push(await i); i++; }
    return seen.join("-");
  }
  f().then(function (v) { say("17 " + v); });
})();

// ---- 18. an await in a for-of --------------------------------------------------------------------------
(function () {
  async function f() {
    var out = [];
    for (var x of [1, 2, 3]) { out.push(await (x * 2)); }
    return out.join("-");
  }
  f().then(function (v) { say("18 " + v); });
})();

// ---- 19. an await inside a try inside a loop -------------------------------------------------------------
(function () {
  async function f() {
    var out = [];
    for (var i = 0; i < 3; i++) {
      try { if (i === 1) { throw new Error("skip" + i); } out.push(await i); }
      catch (e) { out.push("c:" + e.message); }
      finally { out.push("f" + i); }
    }
    return out.join(",");
  }
  f().then(function (v) { say("19 " + v); });
})();

// ---- 20. await of an await ---------------------------------------------------------------------------------
(function () {
  async function inner() { await 0; return 3; }
  async function outer() { return (await inner()) + (await inner()); }
  outer().then(function (v) { say("20 " + v); });
})();

// ---- 21. an async arrow closing over this --------------------------------------------------------------------
(function () {
  var owner = {
    tag: "owner",
    run: function () { var f = async () => this.tag; return f(); },
  };
  owner.run().then(function (v) { say("21 " + v); });
})();

// ---- 22. an async method reading this -------------------------------------------------------------------------
(function () {
  var o = { tag: "o", async read() { await 0; return this.tag; } };
  o.read().then(function (v) { say("22 " + v); });
})();

// ---- 23. an async method in a class ---------------------------------------------------------------------------
(function () {
  class C {
    constructor(v) { this.v = v; }
    async twice() { await 0; return this.v * 2; }
    static async make(v) { await 0; return new C(v); }
  }
  C.make(21).then(function (c) { return c.twice(); }).then(function (v) { say("23 " + v); });
})();

// ---- 24. an async method reaching super -------------------------------------------------------------------------
(function () {
  class B { async name() { await 0; return "B"; } tag() { return "btag"; } }
  class D extends B {
    async name() { var base = await super.name(); return base + "-D-" + super.tag(); }
  }
  new D().name().then(function (v) { say("24 " + v); });
})();

// ---- 25. an async arrow inside a method sees the method's this and super -------------------------------------------
(function () {
  class B { who() { return "B"; } }
  class D extends B {
    run() { var f = async () => super.who() + ":" + this.constructor.name; return f(); }
  }
  new D().run().then(function (v) { say("25 " + v); });
})();

// ---- 26. an async function expression, named --------------------------------------------------------------------
(function () {
  var f = async function self(n) { return n === 0 ? "done" : self(n - 1); };
  f(3).then(function (v) { say("26 " + v); });
})();

// ---- 27. an async IIFE -------------------------------------------------------------------------------------------
(function () {
  (async function () { await 0; say("27 iife"); })();
})();

// ---- 28. Promise.all over async functions --------------------------------------------------------------------------
(function () {
  async function a() { await 0; return 1; }
  async function b() { await 0; await 0; return 2; }
  async function c() { return 3; }
  Promise.all([a(), b(), c()]).then(function (v) { say("28 " + v.join(",")); });
})();

// ---- 29. Promise.race over async functions -----------------------------------------------------------------------------
(function () {
  async function slow() { await 0; await 0; await 0; return "slow"; }
  async function fast() { return "fast"; }
  Promise.race([slow(), fast()]).then(function (v) { say("29 " + v); });
})();

// ---- 30. Promise.allSettled over async functions -------------------------------------------------------------------------
(function () {
  async function ok() { await 0; return "y"; }
  async function bad() { await 0; throw new Error("n"); }
  Promise.allSettled([ok(), bad()]).then(function (rs) {
    say("30 " + rs.map(function (r) { return r.status + ":" + (r.value || r.reason.message); }).join(","));
  });
})();

// ---- 31. Promise.any over async functions ---------------------------------------------------------------------------------
(function () {
  async function bad1() { await 0; throw new Error("1"); }
  async function ok() { await 0; await 0; return "ok"; }
  Promise.any([bad1(), ok()]).then(function (v) { say("31 " + v); });
})();

// ---- 32. await of a generator's result -------------------------------------------------------------------------------------
(function () {
  function* g() { yield 1; yield 2; return 3; }
  async function f() {
    var it = g();
    var sum = 0;
    var step = it.next();
    while (!step.done) { sum += await step.value; step = it.next(); }
    return sum + "/" + (await step.value);
  }
  f().then(function (v) { say("32 " + v); });
})();

// ---- 33. await of a generator object itself -------------------------------------------------------------------------------
(function () {
  function* g() { yield 1; }
  async function f() { var v = await g(); return typeof v.next; }
  f().then(function (v) { say("33 " + v); });
})();

// ---- 34. an async function that awaits nothing settles on a later turn ------------------------------------------------------
(function () {
  var order = [];
  async function f() { return "v"; }
  f().then(function (v) { order.push("resolved:" + v); });
  order.push("sync");
  Promise.resolve().then(function () { order.push("t"); }).then(function () { say("34 " + order.join(",")); });
})();

// ---- 35. returning a promise from an async function costs extra turns -------------------------------------------------------
(function () {
  var order = [];
  async function f() { return Promise.resolve("inner"); }
  f().then(function (v) { order.push("f:" + v); });
  Promise.resolve().then(function () { order.push("1"); })
    .then(function () { order.push("2"); })
    .then(function () { order.push("3"); })
    .then(function () { order.push("4"); })
    .then(function () { say("35 " + order.join(",")); });
})();

// ---- 36. awaiting a promise resolved later ------------------------------------------------------------------------------------
(function () {
  var settle;
  var p = new Promise(function (r) { settle = r; });
  async function f() { return "got:" + (await p); }
  f().then(function (v) { say("36 " + v); });
  settle("late");
})();

// ---- 37. an await inside a nested ordinary function is not allowed -- but a nested async one is -----------------------------------
(function () {
  async function outer() {
    async function inner() { await 0; return "i"; }
    var v = await inner();
    return "o+" + v;
  }
  outer().then(function (v) { say("37 " + v); });
})();

// ---- 38. `await` as an identifier in a script ------------------------------------------------------------------------------------
(function () {
  var await = 5;
  say("38 " + await);
})();

// ---- 39. an async function whose parameter list is not simple ---------------------------------------------------------------------
(function () {
  async function f(a, b = a * 2, ...rest) { await 0; return a + "/" + b + "/" + rest.join("-"); }
  f(1).then(function (v) { say("39a " + v); });
  f(1, 2, 3, 4).then(function (v) { say("39b " + v); });
})();

// ---- 40. an async function destructuring its parameters ------------------------------------------------------------------------------
(function () {
  async function f({ a, b: [c] = [9] }) { await 0; return a + ":" + c; }
  f({ a: 1 }).then(function (v) { say("40 " + v); });
})();

// ---- 41. arguments inside an async function --------------------------------------------------------------------------------------------
(function () {
  async function f() { var n = arguments.length; await 0; return n + ":" + arguments[0] + ":" + arguments[1]; }
  f("x", "y").then(function (v) { say("41 " + v); });
})();

// ---- 42. arguments read through an await operand ------------------------------------------------------------------------------------------
(function () {
  async function f() { return await arguments[0]; }
  f("through").then(function (v) { say("42 " + v); });
})();

// ---- 43. await in an argument list ---------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return [await 1, await 2].concat([await 3]).join("-"); }
  f().then(function (v) { say("43 " + v); });
})();

// ---- 44. await in an object literal and an array literal --------------------------------------------------------------------------------------
(function () {
  async function f() { return JSON.stringify({ a: await 1, b: [await 2, await 3] }); }
  f().then(function (v) { say("44 " + v); });
})();

// ---- 45. await in a conditional and a logical operator ---------------------------------------------------------------------------------------
(function () {
  async function f() { return ((await 1) ? "t" : "f") + ((await 0) || "fallback") + ((await 1) && "and"); }
  f().then(function (v) { say("45 " + v); });
})();

// ---- 46. await binds tighter than + ------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return await 1 + 2; }
  f().then(function (v) { say("46 " + v); });
})();

// ---- 47. await of a unary expression ------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return (await -1) + (await typeof 1) + (await !0); }
  f().then(function (v) { say("47 " + v); });
})();

// ---- 48. await in a template substitution ----------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return `x${await 1}y${await 2}`; }
  f().then(function (v) { say("48 " + v); });
})();

// ---- 49. await in a spread argument ------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return Math.max(...(await [1, 5, 3])); }
  f().then(function (v) { say("49 " + v); });
})();

// ---- 50. await in a computed member access -----------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var o = { k: "v" }; return o[await "k"]; }
  f().then(function (v) { say("50 " + v); });
})();

// ---- 51. await of an optional chain -------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var o = null; return String(await o?.x); }
  f().then(function (v) { say("51 " + v); });
})();

// ---- 52. a rejected await unwound through two finallys ------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() {
    try {
      try { await Promise.reject(new Error("deep")); }
      finally { order.push("inner-finally"); }
    } finally { order.push("outer-finally"); }
  }
  f().catch(function (e) { say("52 " + order.join(",") + " -> " + e.message); });
})();

// ---- 53. an await that resumes into a catch, then continues -----------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var out = [];
    for (var i = 0; i < 3; i++) {
      try { await (i === 1 ? Promise.reject(new Error("e" + i)) : i); out.push("ok" + i); }
      catch (e) { out.push("bad:" + e.message); }
    }
    return out.join(",");
  }
  f().then(function (v) { say("53 " + v); });
})();

// ---- 54. an async function used as a callback --------------------------------------------------------------------------------------------------------------
(function () {
  var promises = [1, 2, 3].map(async function (n) { await 0; return n * n; });
  Promise.all(promises).then(function (v) { say("54 " + v.join(",")); });
})();

// ---- 55. an async arrow with a concise body -----------------------------------------------------------------------------------------------------------------
(function () {
  var f = async (a, b) => a + b;
  f(2, 3).then(function (v) { say("55 " + v); });
})();

// ---- 56. an async arrow with one unparenthesised parameter -------------------------------------------------------------------------------------------------
(function () {
  var f = async n => (await n) * 2;
  f(4).then(function (v) { say("56 " + v); });
})();

// ---- 57. an async arrow with no parameters -------------------------------------------------------------------------------------------------------------------
(function () {
  var f = async () => { await 0; return "none"; };
  f().then(function (v) { say("57 " + v); });
})();

// ---- 58. `async` is still an ordinary identifier -------------------------------------------------------------------------------------------------------------
(function () {
  var async = 1;
  var o = { async: 2 };
  function g(x) { return "g" + x; }
  say("58 " + async + " " + o.async + " " + g(async));
})();

// ---- 59. an object property called async, and a method called async ---------------------------------------------------------------------------------------------
(function () {
  var o = { async: 1, async async() { return "m"; } };
  var p = { async(x) { return "plain" + x; } };
  o.async().then(function (v) { say("59 " + v + " " + p.async(2)); });
})();

// ---- 60. a class method called async ----------------------------------------------------------------------------------------------------------------------------
(function () {
  class C { async() { return "plain-async"; } static async static() { return "static-async"; } }
  C.static().then(function (v) { say("60 " + new C().async() + " " + v); });
})();

// ---- 61. an async getter is not a thing, but a getter returning a promise is ------------------------------------------------------------------------------------
(function () {
  var o = { get later() { return (async function () { await 0; return "lazy"; })(); } };
  o.later.then(function (v) { say("61 " + v); });
})();

// ---- 62. interleaving three async functions -------------------------------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function a() { order.push("a1"); await 0; order.push("a2"); await 0; order.push("a3"); }
  async function b() { order.push("b1"); await 0; order.push("b2"); }
  async function c() { order.push("c1"); }
  a(); b(); c();
  Promise.resolve().then(function () {}).then(function () {}).then(function () {})
    .then(function () { say("62 " + order.join(",")); });
})();

// ---- 63. an async function awaiting another that awaits ------------------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function inner() { order.push("i1"); await 0; order.push("i2"); return "iv"; }
  async function outer() { order.push("o1"); var v = await inner(); order.push("o2:" + v); }
  outer();
  order.push("sync");
  Promise.resolve().then(function () {}).then(function () {}).then(function () {}).then(function () {})
    .then(function () { say("63 " + order.join(",")); });
})();

// ---- 64. a rejection with a non-error value -------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { throw "a string"; }
  f().catch(function (e) { say("64 " + typeof e + " " + e); });
})();

// ---- 65. await of undefined and null ----------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return String(await undefined) + "/" + String(await null); }
  f().then(function (v) { say("65 " + v); });
})();

// ---- 66. an async function's promise is a distinct object each call ---------------------------------------------------------------------------------------------------
(function () {
  async function f() { return 1; }
  var p = f(), q = f();
  say("66 " + (p === q) + " " + (p instanceof Promise) + " " + (q instanceof Promise));
})();

// ---- 67. then on the same promise twice ---------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { await 0; return "shared"; }
  var p = f();
  var out = [];
  p.then(function (v) { out.push("one:" + v); });
  p.then(function (v) { out.push("two:" + v); });
  p.then(function () {}).then(function () { say("67 " + out.join(",")); });
})();

// ---- 68. an async function inside a generator ----------------------------------------------------------------------------------------------------------------------------
(function () {
  function* g() { yield (async function () { await 0; return "from-gen"; })(); }
  g().next().value.then(function (v) { say("68 " + v); });
})();

// ---- 69. a generator inside an async function -----------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    function* g() { yield 1; yield 2; }
    var total = 0;
    for (var v of g()) { total += await v; }
    return total;
  }
  f().then(function (v) { say("69 " + v); });
})();

// ---- 70. an await of a value with a throwing then getter ----------------------------------------------------------------------------------------------------------------
(function () {
  var trap = {};
  Object.defineProperty(trap, "then", { get: function () { throw new Error("getter"); } });
  async function f() { await trap; return "no"; }
  f().catch(function (e) { say("70 " + e.message); });
})();

// ---- 71. an await of a value whose then is not callable ------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var v = await { then: 5 }; return typeof v.then; }
  f().then(function (v) { say("71 " + v); });
})();

// ---- 72. a thenable that rejects ------------------------------------------------------------------------------------------------------------------------------------------
(function () {
  var t = { then: function (_, reject) { reject(new Error("thenable-rejected")); } };
  async function f() { try { await t; return "no"; } catch (e) { return "caught:" + e.message; } }
  f().then(function (v) { say("72 " + v); });
})();

// ---- 73. a thenable that settles twice -----------------------------------------------------------------------------------------------------------------------------------
(function () {
  var t = { then: function (resolve) { resolve("first"); resolve("second"); } };
  async function f() { return await t; }
  f().then(function (v) { say("73 " + v); });
})();

// ---- 74. await in a switch discriminant and a case -------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    switch (await 2) {
      case await 1: return "one";
      case await 2: return "two";
      default: return "other";
    }
  }
  f().then(function (v) { say("74 " + v); });
})();

// ---- 75. await in a do-while condition ----------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var n = 0; do { n++; } while (await (n < 3)); return n; }
  f().then(function (v) { say("75 " + v); });
})();

// ---- 76. await in a for head ---------------------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var out = [];
    for (var i = await 0; i < (await 3); i += await 1) { out.push(i); }
    return out.join("");
  }
  f().then(function (v) { say("76 " + v); });
})();

// ---- 77. await with a labelled continue ---------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var out = [];
    outer: for (var i = 0; i < 3; i++) {
      for (var j = 0; j < 3; j++) { if (j === 1) { continue outer; } out.push(await (i + "" + j)); }
    }
    return out.join(",");
  }
  f().then(function (v) { say("77 " + v); });
})();

// ---- 78. async functions and closures over a loop variable -----------------------------------------------------------------------------------------------------------------------
(function () {
  var fs = [];
  for (let i = 0; i < 3; i++) { fs.push(async function () { await 0; return i; }); }
  Promise.all(fs.map(function (f) { return f(); })).then(function (v) { say("78 " + v.join(",")); });
})();

// ---- 79. an async function stored on a prototype ---------------------------------------------------------------------------------------------------------------------------------
(function () {
  function C(v) { this.v = v; }
  C.prototype.get = async function () { await 0; return this.v; };
  new C("proto").get().then(function (v) { say("79 " + v); });
})();

// ---- 80. call, apply and bind on an async function --------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f(a, b) { await 0; return this.t + a + b; }
  var ctx = { t: "T" };
  Promise.all([f.call(ctx, 1, 2), f.apply(ctx, [3, 4]), f.bind(ctx)(5, 6)])
    .then(function (v) { say("80 " + v.join(",")); });
})();

// ---- 81. an async function's length and name -------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f(a, b, c = 1) {}
  var g = async (x) => x;
  var o = { async m(a, b) {} };
  say("81 " + f.length + " " + f.name + " " + g.length + " " + o.m.length + " " + o.m.name);
})();

// ---- 82. an async method is not a constructor -------------------------------------------------------------------------------------------------------------------------------------
(function () {
  var o = { async m() {} };
  var caught = "none";
  try { new o.m(); } catch (e) { caught = e.constructor.name; }
  say("82 " + caught);
})();

// ---- 83. await of an async function that returns a thenable ---------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return { then: function (r) { r("via-thenable"); } }; }
  async function g() { return await f(); }
  g().then(function (v) { say("83 " + v); });
})();

// ---- 84. errors thrown by an await operand's evaluation ------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { try { await (null).x; } catch (e) { return "caught:" + e.constructor.name; } }
  f().then(function (v) { say("84 " + v); });
})();

// ---- 85. a rejected await inside a nested try ----------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    try { try { await Promise.reject(new Error("x")); } catch (e) { throw new Error("re:" + e.message); } }
    catch (e) { return e.message; }
  }
  f().then(function (v) { say("85 " + v); });
})();

// ---- 86. many awaits in sequence ----------------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var n = 0; for (var i = 0; i < 50; i++) { n += await 1; } return n; }
  f().then(function (v) { say("86 " + v); });
})();

// ---- 87. a chain of async functions ---------------------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function step(n) { if (n === 0) { return 0; } return n + (await step(n - 1)); }
  step(20).then(function (v) { say("87 " + v); });
})();

// ---- 88. an async function returning itself's promise chain --------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { return 1; }
  f().then(function (v) { return v + 1; }).then(function (v) { return f().then(function (w) { return v + w; }); })
    .then(function (v) { say("88 " + v); });
})();

// ---- 89. Promise.resolve of an async function's promise is the same object ---------------------------------------------------------------------------------------------------
(function () {
  async function f() { return 1; }
  var p = f();
  say("89 " + (Promise.resolve(p) === p));
})();

// ---- 90. await of a promise that never settles is simply never resumed -------------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() { await new Promise(function () {}); order.push("never"); }
  f();
  Promise.resolve().then(function () { order.push("still-running"); })
    .then(function () { say("90 " + order.join(",") + "|" + order.length); });
})();

// ---- 91. an async function called before its declaration (hoisting) ---------------------------------------------------------------------------------------------------------
(function () {
  hoisted().then(function (v) { say("91 " + v); });
  async function hoisted() { await 0; return "hoisted"; }
})();

// ---- 92. an async function declared inside a block ---------------------------------------------------------------------------------------------------------------------------
(function () {
  {
    async function blocked() { return "blocked"; }
    blocked().then(function (v) { say("92 " + v); });
  }
})();

// ---- 93. await of a string and of an object -------------------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var o = await { a: 1 }; return (await "s") + ":" + o.a; }
  f().then(function (v) { say("93 " + v); });
})();

// ---- 94. an async function that returns undefined ---------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() { await 0; }
  f().then(function (v) { say("94 " + String(v) + " " + (v === undefined)); });
})();

// ---- 95. an await of the async function's own promise deadlocks quietly ---------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() { var self = f.last; if (self) { await self; } order.push("done"); return "x"; }
  f.last = null;
  f().then(function () { order.push("settled"); });
  Promise.resolve().then(function () {}).then(function () { say("95 " + order.join(",")); });
})();

// ---- 96. a rejection produced by an await inside Promise.all ------------------------------------------------------------------------------------------------------------
(function () {
  async function ok() { await 0; return 1; }
  async function bad() { await 0; throw new Error("all-bad"); }
  Promise.all([ok(), bad()]).then(function () { say("96 no"); }, function (e) { say("96 " + e.message); });
})();

// ---- 97. an async function inside a class static method reaching the class ------------------------------------------------------------------------------------------
(function () {
  class D { static async make() { await 0; return D.name; } }
  D.make().then(function (v) { say("97 " + v); });
})();

// ---- 98. an async method with a computed key -----------------------------------------------------------------------------------------------------------------------
(function () {
  var key = "dyn";
  var o = { async [key]() { await 0; return "computed"; } };
  class C { async [key]() { await 0; return "class-computed"; } }
  Promise.all([o.dyn(), new C().dyn()]).then(function (v) { say("98 " + v.join(",")); });
})();

// ---- 99. an async function used with instanceof and typeof checks -------------------------------------------------------------------------------------------------
(function () {
  async function f() {}
  say("99 " + (f instanceof Function) + " " + (f instanceof Object) + " " + Object.getPrototypeOf(f).constructor.name);
})();

// ---- 100. an async function's toString-ish shape -------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {}
  say("100 " + (typeof f.toString === "function") + " " + Object.keys(f).length);
})();

// ---- 101. await inside a nested arrow inside an async function ------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var g = async () => (await 1) + 1;
    return await g();
  }
  f().then(function (v) { say("101 " + v); });
})();

// ---- 102. two async functions sharing a closure variable ------------------------------------------------------------------------------------------------------------
(function () {
  var shared = 0;
  async function inc() { await 0; shared++; return shared; }
  Promise.all([inc(), inc(), inc()]).then(function (v) { say("102 " + v.join(",") + " " + shared); });
})();

// ---- 103. await inside a getter's body reached through an async function -----------------------------------------------------------------------------------------
(function () {
  var o = { get v() { return 7; } };
  async function f() { return (await o).v; }
  f().then(function (v) { say("103 " + v); });
})();

// ---- 104. Promise.reject awaited without a catch inside try/finally ---------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() {
    try { await Promise.reject(new Error("r")); }
    finally { order.push("cleanup"); }
  }
  f().catch(function (e) { say("104 " + order.join(",") + "|" + e.message); });
})();

// ---- 105. an async function whose finally awaits after a throw --------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() {
    try { throw new Error("t"); }
    finally { order.push("f-in"); await 0; order.push("f-out"); }
  }
  f().catch(function (e) { say("105 " + order.join(",") + "|" + e.message); });
})();

// ---- 106. return inside try with an awaiting finally that also returns -----------------------------------------------------------------------------------------
(function () {
  async function f() {
    try { return "try"; }
    finally { await 0; }
  }
  f().then(function (v) { say("106 " + v); });
})();

// ---- 107. async functions in an array, resolved in reverse order -----------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function make(n, waits) { for (var i = 0; i < waits; i++) { await 0; } order.push(n); return n; }
  Promise.all([make("a", 3), make("b", 1), make("c", 2)]).then(function (v) {
    say("107 " + order.join(",") + " -> " + v.join(","));
  });
})();

// ---- 108. an async function reading a const in a temporal dead zone ---------------------------------------------------------------------------------------------
(function () {
  async function f() { try { return early; } catch (e) { return e.constructor.name; } finally { } }
  var r = f();
  const early = 1;
  r.then(function (v) { say("108 " + v); });
})();

// ---- 109. `this` in a sloppy async function called bare ---------------------------------------------------------------------------------------------------------
(function () {
  async function f() { await 0; return this === undefined ? "undefined" : typeof this; }
  f().then(function (v) { say("109 " + v); });
})();

// ---- 110. `this` in a strict async function called bare ---------------------------------------------------------------------------------------------------------
(function () {
  "use strict";
  async function f() { await 0; return this === undefined ? "undefined" : typeof this; }
  f().then(function (v) { say("110 " + v); });
})();

// ---- 111. an async function on a frozen object ------------------------------------------------------------------------------------------------------------------
(function () {
  var o = Object.freeze({ async m() { await 0; return "frozen"; } });
  o.m().then(function (v) { say("111 " + v); });
})();

// ---- 112. an async function used as an object's Symbol-keyed method ---------------------------------------------------------------------------------------------
(function () {
  var s = Symbol("k");
  var o = { async [s]() { await 0; return "symbol-keyed"; } };
  o[s]().then(function (v) { say("112 " + v); });
})();

// ---- 113. an async function inside a Map value -----------------------------------------------------------------------------------------------------------------
(function () {
  var m = new Map([["k", async function () { await 0; return "mapped"; }]]);
  m.get("k")().then(function (v) { say("113 " + v); });
})();

// ---- 114. await of NaN, Infinity and -0 -----------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var a = await NaN, b = await Infinity, c = await -0;
    return [a, b, Object.is(c, -0)].join("/");
  }
  f().then(function (v) { say("114 " + v); });
})();

// ---- 115. an async function whose body only throws a rejection through a helper ---------------------------------------------------------------------------------
(function () {
  function helper() { return Promise.reject(new Error("helper")); }
  async function f() { return helper(); }
  f().catch(function (e) { say("115 " + e.message); });
})();

// ---- 116. many independent async calls resolve in enqueue order ------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  for (var i = 0; i < 5; i++) { (async function (n) { await 0; order.push(n); })(i); }
  Promise.resolve().then(function () {}).then(function () { say("116 " + order.join(",")); });
})();

// ---- 117. an await after a for-in loop -----------------------------------------------------------------------------------------------------------------------
(function () {
  async function f() {
    var keys = [];
    for (var k in { a: 1, b: 2 }) { keys.push(k); }
    return keys.join(",") + ":" + (await keys.length);
  }
  f().then(function (v) { say("117 " + v); });
})();

// ---- 118. an async function that awaits a regexp match ---------------------------------------------------------------------------------------------------------
(function () {
  async function f() { var m = await "abc123".match(/(\d+)/); return m[1]; }
  f().then(function (v) { say("118 " + v); });
})();

// ---- 119. an unhandled rejection leaves the program running -----------------------------------------------------------------------------------------------------
(function () {
  var order = [];
  async function f() { throw new Error("nobody-catches"); }
  var p = f();
  p.catch(function () {});
  order.push("continued");
  Promise.resolve().then(function () { order.push("later"); })
    .then(function () { say("119 " + order.join(",")); });
})();

// ---- 121. an async function with `await` as a property name -------------------------------------
(function () {
  async function f() { var o = { await: 1, async: 2 }; return o.await + o.async + (await 3); }
  f().then(function (v) { say("121 " + v); });
})();

// ---- 122. an async function inside a class expression --------------------------------------------
(function () {
  var C = class { async run() { await 0; return "class-expression"; } };
  new C().run().then(function (v) { say("122 " + v); });
})();

// ---- 123. an async method on a class the program extends -----------------------------------------
(function () {
  class A { async go() { await 0; return "A"; } }
  class B extends A {}
  new B().go().then(function (v) { say("123 " + v + " " + (new B() instanceof A)); });
})();

// ---- 124. awaiting inside a nested try/catch/finally that rethrows -------------------------------
(function () {
  var order = [];
  async function f() {
    try {
      try { await 0; throw new Error("inner"); }
      catch (e) { order.push("c1"); throw new Error("second"); }
      finally { order.push("f1"); await 0; order.push("f1b"); }
    } catch (e) { order.push("c2:" + e.message); }
    finally { order.push("f2"); }
    return order.join(",");
  }
  f().then(function (v) { say("124 " + v); });
})();

// ---- 125. an async function whose await operand is a call chain ----------------------------------
(function () {
  var o = { list: [1, 2, 3], get: function (i) { return this.list[i]; } };
  async function f() { return (await o.get(1)) + (await o["get"](2)); }
  f().then(function (v) { say("125 " + v); });
})();

// ---- 126. an async function that returns an already-rejected promise ------------------------------
(function () {
  async function f() { return Promise.reject(new Error("returned-rejection")); }
  f().catch(function (e) { say("126 " + e.message); });
})();

// ---- 127. Promise.prototype.finally after an async call ------------------------------------------
(function () {
  var order = [];
  async function ok() { await 0; return "v"; }
  ok().finally(function () { order.push("finally"); }).then(function (v) { say("127 " + order.join(",") + ":" + v); });
})();

// ---- 128. an async function whose await is the only statement -------------------------------------
(function () {
  async function f() { await 0; }
  var p = f();
  var before = p instanceof Promise;
  p.then(function (v) { say("128 " + before + " " + String(v)); });
})();

// ---- 129. an async function over a Set and a Map ---------------------------------------------------
(function () {
  async function f() {
    var s = new Set([1, 2, 3]);
    var total = 0;
    for (var v of s) { total += await v; }
    var m = new Map([["a", 1]]);
    return total + ":" + (await m.get("a"));
  }
  f().then(function (v) { say("129 " + v); });
})();

// ---- 130. an async function over a typed array -----------------------------------------------------
(function () {
  async function f() {
    var a = new Uint8Array([1, 2, 3]);
    var total = 0;
    for (var i = 0; i < a.length; i++) { total += await a[i]; }
    return total;
  }
  f().then(function (v) { say("130 " + v); });
})();

// ---- 131. an async function that awaits inside a JSON round trip ------------------------------------
(function () {
  async function f() { return JSON.parse(await JSON.stringify({ a: [1, 2] })).a[1]; }
  f().then(function (v) { say("131 " + v); });
})();

// ---- 132. an async arrow returning an object literal -------------------------------------------------
(function () {
  var f = async () => ({ tag: "literal" });
  f().then(function (v) { say("132 " + v.tag); });
})();

// ---- 133. an async function with a default that calls another async function --------------------------
(function () {
  async function inner() { await 0; return "inner"; }
  async function outer(v = inner()) { return "outer/" + (await v); }
  outer().then(function (v) { say("133 " + v); });
})();

// ---- 134. an async function's rejection reaching a later catch in a chain ------------------------------
(function () {
  async function f() { await 0; throw new Error("chained"); }
  f().then(function () { return "no"; }).then(function () { return "still-no"; })
    .catch(function (e) { say("134 " + e.message); });
})();

// ---- 135. await in a getter-backed property read after suspension ---------------------------------------
(function () {
  var count = 0;
  var o = { get n() { count++; return count; } };
  async function f() { var a = await o.n; await 0; var b = await o.n; return a + ":" + b + ":" + count; }
  f().then(function (v) { say("135 " + v); });
})();

// ---- 136. a rejection an await recovers from, twice in one body -------------------------------------------
(function () {
  async function f() {
    var out = [];
    try { await Promise.reject(new Error("one")); } catch (e) { out.push(e.message); }
    try { await Promise.reject(new Error("two")); } catch (e) { out.push(e.message); }
    return out.join("+");
  }
  f().then(function (v) { say("136 " + v); });
})();

// ---- 137. an async function called from a promise reaction ---------------------------------------------
(function () {
  async function inner() { await 0; return "from-a-reaction"; }
  Promise.resolve().then(function () { return inner(); }).then(function (v) { say("137 " + v); });
})();

// ---- 138. an async function called from a generator's body ----------------------------------------------
(function () {
  async function inner() { await 0; return "async-in-generator"; }
  function* g() { yield inner(); }
  g().next().value.then(function (v) { say("138 " + v); });
})();

// ---- 139. every case has run ------------------------------------------------------------------------------
Promise.resolve()
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () { say("139 done"); });
