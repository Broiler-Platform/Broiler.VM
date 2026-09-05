// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE ASYNC GENERATOR AND `for await`.
//
// Retained from the bring-up of the family it covers: every case was compared against the
// comparison engine before it was written down, and each prints its own number so a divergence
// names a case rather than a line.
//
// AN ASYNC GENERATOR IS NOT THE TWO FAMILIES IT IS SPELLED FROM, and the cases are grouped by the
// parts neither of them carries: the request queue, the promise every method answers, the four
// intrinsics between a call and `Object.prototype`, the wrapping of a synchronous iterator, and the
// delegation that awaits each inner step before it asks whether the inner one is done.

function say(x) { print(x); }
function shape(step) { return step.value + ":" + step.done; }

// ---- 1. the call runs nothing and answers an object -------------------------------------------
(function () {
  var order = [];
  async function* g() { order.push("body"); yield 1; }
  order.push("before");
  var it = g();
  order.push("called");
  it.next();
  order.push("stepped");
  say("1 " + order.join(","));
})();

// ---- 2. what the call answered ----------------------------------------------------------------
(function () {
  async function* g() { yield 1; }
  var it = g();
  say("2 " + typeof it + " " + Object.prototype.toString.call(it) + " " + typeof it.next);
})();

// ---- 3. what a step answers -------------------------------------------------------------------
(function () {
  async function* g() { yield 1; }
  var p = g().next();
  say("3 " + (p instanceof Promise) + " " + Object.prototype.toString.call(p) + " " + typeof p.then);
})();

// ---- 4. an async generator function is not a constructor --------------------------------------
(function () {
  async function* g() {}
  var caught = "none";
  try { new g(); } catch (e) { caught = e.constructor.name; }
  say("4 " + caught + " " + (typeof g.prototype) + " " + g.length + " " + g.name);
})();

// ---- 5. the tag of the function and of what it answers ----------------------------------------
(function () {
  async function* g() {}
  say("5 " + Object.prototype.toString.call(g) + " " + Object.prototype.toString.call(g()));
})();

// ---- 6. the prototype chain, four objects deep ------------------------------------------------
(function () {
  async function* g() {}
  var made = g();
  var proto = Object.getPrototypeOf(g.prototype);
  var iter = Object.getPrototypeOf(proto);
  say("6 " + (Object.getPrototypeOf(made) === g.prototype) +
    " " + Object.prototype.toString.call(proto) +
    " " + (Object.getPrototypeOf(iter) === Object.prototype));
})();

// ---- 7. `%AsyncIteratorPrototype%` carries the Symbol -----------------------------------------
(function () {
  async function* g() {}
  var iter = Object.getPrototypeOf(Object.getPrototypeOf(g.prototype));
  say("7 " + typeof iter[Symbol.asyncIterator] + " " + iter.hasOwnProperty(Symbol.asyncIterator) +
    " " + Object.getPrototypeOf(g.prototype).hasOwnProperty(Symbol.asyncIterator));
})();

// ---- 8. an async generator is its own async iterable ------------------------------------------
(function () {
  async function* g() {}
  var made = g();
  say("8 " + (made[Symbol.asyncIterator]() === made) + " " + (typeof made[Symbol.iterator]));
})();

// ---- 9. the members of `%AsyncGeneratorPrototype%`, in creation order -------------------------
(function () {
  async function* g() {}
  var proto = Object.getPrototypeOf(g.prototype);
  say("9 " + Object.getOwnPropertyNames(proto).join(",") + " " + proto[Symbol.toStringTag]);
})();

// ---- 10. the back-links --------------------------------------------------------------------
(function () {
  async function* g() {}
  var proto = Object.getPrototypeOf(g.prototype);
  var functionProto = Object.getPrototypeOf(g);
  say("10 " + (proto.constructor === functionProto) +
    " " + functionProto[Symbol.toStringTag] +
    " " + (functionProto.prototype === proto) +
    " " + (Object.getPrototypeOf(functionProto) === Function.prototype));
})();

// ---- 11. the function's own `prototype` has no `constructor` ---------------------------------
(function () {
  async function* g() {}
  var d = Object.getOwnPropertyDescriptor(g, "prototype");
  say("11 " + g.prototype.hasOwnProperty("constructor") +
    " " + d.writable + " " + d.enumerable + " " + d.configurable);
})();

// ---- 12. `%AsyncGeneratorFunction%` is reachable and refuses ---------------------------------
(function () {
  async function* g() {}
  var ctor = Object.getPrototypeOf(g).constructor;
  say("12 " + ctor.name + " " + ctor.length + " " + (Object.getPrototypeOf(ctor) === Function));
})();

// ---- 13. an ordinary drain -------------------------------------------------------------------
(function () {
  async function* g() { yield 1; yield 2; return 3; }
  var it = g();
  it.next()
    .then(function (a) { return it.next().then(function (b) { return [a, b]; }); })
    .then(function (both) {
      return it.next().then(function (c) {
        return it.next().then(function (d) {
          say("13 " + [shape(both[0]), shape(both[1]), shape(c), shape(d)].join(" "));
        });
      });
    });
})();

// ---- 14. a yielded value is AWAITED before it is delivered ------------------------------------
(function () {
  async function* g() { yield Promise.resolve("resolved"); }
  g().next().then(function (step) { say("14 " + shape(step)); });
})();

// ---- 15. a RETURNED value is awaited too, and only in this body kind ---------------------------
(function () {
  async function* g() { return Promise.resolve("r"); }
  g().next().then(function (step) {
    say("15 " + (step.value instanceof Promise) + " " + step.done);
  });
})();

// ---- 16. the value a `yield` evaluates to is the NEXT resumption's -----------------------------
(function () {
  async function* g() { var a = yield 1; var b = yield 2; return a + "/" + b; }
  var it = g();
  it.next("first")
    .then(function () { return it.next("second"); })
    .then(function () { return it.next("third"); })
    .then(function (step) { say("16 " + shape(step)); });
})();

// ---- 17. four requests before the first settles, answered in order ----------------------------
(function () {
  async function* g() { yield 1; yield 2; return 3; }
  var it = g();
  var all = [it.next(), it.next(), it.next(), it.next()];
  Promise.all(all).then(function (steps) {
    say("17 " + steps.map(shape).join(" "));
  });
})();

// ---- 18. a request made mid-await is queued rather than dropped -------------------------------
(function () {
  var order = [];
  async function* g() { yield 1; await 0; await 0; yield 2; }
  var it = g();
  it.next().then(function (a) { order.push(shape(a)); });
  it.next().then(function (b) { order.push(shape(b)); });
  it.next().then(function (c) {
    order.push(shape(c));
    say("18 " + order.join(" "));
  });
})();

// ---- 19. `await` and `yield` in one body do not confuse each other -----------------------------
(function () {
  async function* g() {
    var a = await Promise.resolve("a");
    yield a;
    var b = await Promise.resolve("b");
    yield b;
  }
  var it = g();
  it.next()
    .then(function (first) { return it.next().then(function (second) { return [first, second]; }); })
    .then(function (both) { say("19 " + both.map(shape).join(" ")); });
})();

// ---- 20. a body that throws rejects the request it was serving --------------------------------
(function () {
  async function* g() { yield 1; throw new Error("boom"); }
  var it = g();
  it.next()
    .then(function () { return it.next(); })
    .then(
      function () { say("20 resolved"); },
      function (e) {
        return it.next().then(function (after) {
          say("20 " + e.constructor.name + ":" + e.message + " " + shape(after));
        });
      });
})();

// ---- 21. an await that rejects is a throw at the suspension point -----------------------------
(function () {
  async function* g() {
    try { await Promise.reject(new Error("r")); } catch (e) { yield "caught:" + e.message; }
  }
  g().next().then(function (step) { say("21 " + shape(step)); });
})();

// ---- 22. `next` on a completed generator answers a done step ----------------------------------
(function () {
  async function* g() { }
  var it = g();
  it.next()
    .then(function () { return it.next(); })
    .then(function (step) { say("22 " + shape(step)); });
})();

// ---- 23. `throw` into a generator that has not started rejects, and runs nothing ---------------
(function () {
  var ran = false;
  async function* g() { ran = true; yield 1; }
  var it = g();
  it.throw(new Error("early")).then(
    function () { say("23 resolved"); },
    function (e) {
      return it.next().then(function (after) {
        say("23 " + e.message + " " + ran + " " + shape(after));
      });
    });
})();

// ---- 24. `return` into a generator that has not started runs no `finally` ---------------------
(function () {
  var ran = [];
  async function* g() { try { ran.push("body"); yield 1; } finally { ran.push("fin"); } }
  var it = g();
  it.return("R").then(function (step) { say("24 " + shape(step) + " " + ran.join(",")); });
})();

// ---- 25. `return` into a suspended yield runs every `finally` ---------------------------------
(function () {
  var ran = [];
  async function* g() { try { yield 1; yield 2; } finally { ran.push("fin"); } }
  var it = g();
  it.next().then(function () { return it.return("R"); }).then(function (step) {
    say("25 " + shape(step) + " " + ran.join(","));
  });
})();

// ---- 26. a `finally` may override a forced return ---------------------------------------------
(function () {
  async function* g() { try { yield 1; } finally { yield "from-finally"; } }
  var it = g();
  it.next().then(function () { return it.return("R"); }).then(function (step) {
    return it.next().then(function (after) { say("26 " + shape(step) + " " + shape(after)); });
  });
})();

// ---- 27. `return`'s value is AWAITED before the finalisers run --------------------------------
(function () {
  var order = [];
  async function* g() { try { yield 1; } finally { order.push("fin"); } }
  var waited = { then: function (resolve) { order.push("awaited"); resolve("W"); } };
  var it = g();
  it.next().then(function () { return it.return(waited); }).then(function (step) {
    say("27 " + shape(step) + " " + order.join(","));
  });
})();

// ---- 28. `return` on a completed generator still awaits its value ------------------------------
(function () {
  async function* g() { }
  var it = g();
  it.next().then(function () { return it.return(Promise.resolve("late")); }).then(function (step) {
    say("28 " + shape(step));
  });
})();

// ---- 29. a rejected `return` value rejects the request ----------------------------------------
(function () {
  async function* g() { }
  var it = g();
  it.next().then(function () { return it.return(Promise.reject(new Error("no"))); }).then(
    function () { say("29 resolved"); },
    function (e) { say("29 " + e.constructor.name + ":" + e.message); });
})();

// ---- 30. a queued `return` is answered after the requests before it ----------------------------
(function () {
  async function* g() { yield 1; yield 2; yield 3; }
  var it = g();
  var all = [it.next(), it.next(), it.return("R"), it.next()];
  Promise.all(all).then(function (steps) { say("30 " + steps.map(shape).join(" ")); });
})();

// ---- 31. the three methods on a receiver that is not one --------------------------------------
(function () {
  async function* g() {}
  var proto = Object.getPrototypeOf(g.prototype);
  proto.next.call(1).then(
    function () { say("31 resolved"); },
    function (e) { say("31 " + e.constructor.name); });
})();

// ---- 32. `for await` over an async generator --------------------------------------------------
(function () {
  async function* g() { yield 1; yield 2; }
  (async function () {
    var out = [];
    for await (const v of g()) { out.push(v); }
    say("32 " + out.join(","));
  })();
})();

// ---- 33. `for await` over an Array of promises ------------------------------------------------
(function () {
  (async function () {
    var out = [];
    for await (const v of [Promise.resolve(1), 2, Promise.resolve(3)]) { out.push(v); }
    say("33 " + out.join(","));
  })();
})();

// ---- 34. `for await` over a synchronous iterable of thenables ---------------------------------
(function () {
  function thenable(v) { return { then: function (r) { r(v); } }; }
  var source = {};
  source[Symbol.iterator] = function () {
    var at = 0;
    return { next: function () {
      at++;
      return at <= 2 ? { value: thenable("t" + at), done: false } : { value: 0, done: true };
    } };
  };
  (async function () {
    var out = [];
    for await (const v of source) { out.push(v); }
    say("34 " + out.join(","));
  })();
})();

// ---- 35. an object with BOTH Symbols takes the async one --------------------------------------
(function () {
  var source = {};
  source[Symbol.iterator] = function () {
    return { next: function () { return { value: "sync", done: false }; } };
  };
  source[Symbol.asyncIterator] = function () {
    var at = 0;
    return { next: function () {
      at++;
      return Promise.resolve(at <= 1 ? { value: "async", done: false } : { value: 0, done: true });
    } };
  };
  (async function () {
    var out = [];
    for await (const v of source) { out.push(v); }
    say("35 " + out.join(","));
  })();
})();

// ---- 36. `for await` over a String --------------------------------------------------------------
(function () {
  (async function () {
    var out = [];
    for await (const c of "ab") { out.push(c); }
    say("36 " + out.join("-"));
  })();
})();

// ---- 37. a value with neither Symbol -----------------------------------------------------------
(function () {
  (async function () {
    try { for await (const v of 5) { say("37 " + v); } }
    catch (e) { say("37 " + e.constructor.name); }
  })();
})();

// ---- 38. a rejected element rejects the loop ---------------------------------------------------
(function () {
  (async function () {
    try { for await (const v of [Promise.reject(new Error("bad"))]) { say("38 " + v); } }
    catch (e) { say("38 " + e.constructor.name + ":" + e.message); }
  })();
})();

// ---- 39. the head's own bindings, per iteration -------------------------------------------------
(function () {
  (async function () {
    var made = [];
    for await (const v of [1, 2, 3]) { made.push(function () { return v; }); }
    say("39 " + made.map(function (f) { return f(); }).join(","));
  })();
})();

// ---- 40. a `var` head, an assignment head and a destructuring head -----------------------------
(function () {
  (async function () {
    var out = [];
    for await (var a of [1, 2]) { out.push(a); }
    var b;
    for await (b of [3]) { out.push(b); }
    for await (const [c, d] of [[4, 5]]) { out.push(c + "/" + d); }
    for await (const { e } of [{ e: 6 }]) { out.push(e); }
    say("40 " + out.join(","));
  })();
})();

// ---- 41. `break` closes the iterator, and the close is awaited ---------------------------------
(function () {
  var order = [];
  function source() {
    var at = 0;
    var o = {};
    o[Symbol.asyncIterator] = function () {
      return {
        next: function () { at++; return Promise.resolve({ value: at, done: at > 9 }); },
        return: function () { order.push("closed"); return Promise.resolve({ done: true }); },
      };
    };
    return o;
  }
  (async function () {
    for await (const v of source()) { if (v === 2) { break; } }
    order.push("after");
    say("41 " + order.join(","));
  })();
})();

// ---- 42. `throw` from the body closes it and keeps the body's exception -------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    return {
      next: function () { return Promise.resolve({ value: 1, done: false }); },
      return: function () { order.push("closed"); return Promise.reject(new Error("ignored")); },
    };
  };
  (async function () {
    try { for await (const v of o) { throw new Error("mine"); } }
    catch (e) { order.push("caught:" + e.message); }
    say("42 " + order.join(","));
  })();
})();

// ---- 43. an iterator with no `return` is not closed and does not fail --------------------------
(function () {
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return { next: function () { at++; return Promise.resolve({ value: at, done: false }); } };
  };
  (async function () {
    var out = [];
    for await (const v of o) { out.push(v); if (v === 2) { break; } }
    say("43 " + out.join(","));
  })();
})();

// ---- 44. running out closes nothing --------------------------------------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return {
      next: function () { at++; return Promise.resolve({ value: at, done: at > 2 }); },
      return: function () { order.push("closed"); return Promise.resolve({ done: true }); },
    };
  };
  (async function () {
    for await (const v of o) { order.push(v); }
    say("44 " + order.join(","));
  })();
})();

// ---- 45. `continue` does not close ---------------------------------------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return {
      next: function () { at++; return Promise.resolve({ value: at, done: at > 3 }); },
      return: function () { order.push("closed"); return Promise.resolve({ done: true }); },
    };
  };
  (async function () {
    for await (const v of o) { if (v < 3) { continue; } order.push(v); }
    say("45 " + order.join(","));
  })();
})();

// ---- 46. a step that is not an object ------------------------------------------------------------
(function () {
  var o = {};
  o[Symbol.asyncIterator] = function () {
    return { next: function () { return Promise.resolve(5); } };
  };
  (async function () {
    try { for await (const v of o) { say("46 " + v); } }
    catch (e) { say("46 " + e.constructor.name); }
  })();
})();

// ---- 47. a `return` out of a `for await` closes it before it returns ------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return {
      next: function () { at++; return Promise.resolve({ value: at, done: false }); },
      return: function () { order.push("closed"); return Promise.resolve({ done: true }); },
    };
  };
  (async function () {
    async function inner() { for await (const v of o) { return "early"; } return "late"; }
    order.push(await inner());
    say("47 " + order.join(","));
  })();
})();

// ---- 48. `for await` in an async generator, an async method and an async arrow --------------------
(function () {
  async function* doubled(source) { for await (const v of source) { yield v * 2; } }
  var holder = { async run(source) { var o = []; for await (const v of source) { o.push(v); } return o.join("+"); } };
  var arrow = async function (source) { var o = []; for await (const v of source) { o.push(v); } return o.join("-"); };
  (async function () {
    var out = [];
    for await (const v of doubled([1, 2])) { out.push(v); }
    say("48 " + out.join(",") + " " + await holder.run([3, 4]) + " " + await arrow([5, 6]));
  })();
})();

// ---- 49. a nested `for await` -----------------------------------------------------------------------
(function () {
  (async function () {
    var out = [];
    for await (const row of [[1, 2], [3]]) {
      for await (const v of row) { out.push(v); }
    }
    say("49 " + out.join(","));
  })();
})();

// ---- 50. a labelled break out of the outer loop closes both ------------------------------------------
(function () {
  var order = [];
  function source(name) {
    var at = 0;
    var o = {};
    o[Symbol.asyncIterator] = function () {
      return {
        next: function () { at++; return Promise.resolve({ value: name + at, done: at > 9 }); },
        return: function () { order.push(name + ":closed"); return Promise.resolve({ done: true }); },
      };
    };
    return o;
  }
  (async function () {
    outer: for await (const a of source("a")) {
      for await (const b of source("b")) { break outer; }
    }
    say("50 " + order.join(","));
  })();
})();

// ---- 51. `yield*` over an async generator -------------------------------------------------------------
(function () {
  async function* inner() { yield 1; yield 2; return "inner"; }
  async function* outer() { var r = yield* inner(); yield "after:" + r; }
  (async function () {
    var out = [];
    for await (const v of outer()) { out.push(v); }
    say("51 " + out.join(","));
  })();
})();

// ---- 52. `yield*` over a SYNCHRONOUS generator ---------------------------------------------------------
(function () {
  function* inner() { yield 1; yield 2; return "sync"; }
  async function* outer() { var r = yield* inner(); yield "after:" + r; }
  (async function () {
    var out = [];
    for await (const v of outer()) { out.push(v); }
    say("52 " + out.join(","));
  })();
})();

// ---- 53. `yield*` over an Array of promises ------------------------------------------------------------
(function () {
  async function* outer() { yield* [Promise.resolve("p"), "q"]; }
  (async function () {
    var out = [];
    for await (const v of outer()) { out.push(v); }
    say("53 " + out.join(","));
  })();
})();

// ---- 54. a `throw` arriving mid-delegation reaches the inner iterator ------------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return {
      next: function () { at++; return Promise.resolve({ value: "n" + at, done: false }); },
      throw: function (e) { order.push("inner:" + e.message); return Promise.resolve({ value: "ok", done: false }); },
    };
  };
  async function* outer() { yield* o; }
  var it = outer();
  it.next().then(function () { return it.throw(new Error("E")); }).then(function (step) {
    say("54 " + order.join(",") + " " + shape(step));
  });
})();

// ---- 55. a `return` arriving mid-delegation reaches the inner iterator -----------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return {
      next: function () { at++; return Promise.resolve({ value: "n" + at, done: false }); },
      return: function (v) { order.push("inner:" + v); return Promise.resolve({ value: v, done: true }); },
    };
  };
  async function* outer() { yield* o; }
  var it = outer();
  it.next().then(function () { return it.return("R"); }).then(function (step) {
    say("55 " + order.join(",") + " " + shape(step));
  });
})();

// ---- 56. an inner iterator with no `throw` is closed and then refused -------------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    return {
      next: function () { return Promise.resolve({ value: 1, done: false }); },
      return: function () { order.push("closed"); return Promise.resolve({ done: true }); },
    };
  };
  async function* outer() { yield* o; }
  var it = outer();
  it.next().then(function () { return it.throw(new Error("X")); }).then(
    function () { say("56 resolved"); },
    function (e) { say("56 " + order.join(",") + " " + e.constructor.name); });
})();

// ---- 57. an inner iterator with no `return` does not swallow the outer one ---------------------------------
(function () {
  var o = {};
  o[Symbol.asyncIterator] = function () {
    return { next: function () { return Promise.resolve({ value: 1, done: false }); } };
  };
  async function* outer() { try { yield* o; } finally { } }
  var it = outer();
  it.next().then(function () { return it.return("R"); }).then(function (step) {
    say("57 " + shape(step));
  });
})();

// ---- 58. the delegation's own step is awaited ---------------------------------------------------------------
(function () {
  var order = [];
  var o = {};
  o[Symbol.asyncIterator] = function () {
    var at = 0;
    return { next: function () {
      at++;
      order.push("next" + at);
      return at <= 2
        ? { then: function (r) { order.push("awaited" + at); r({ value: at, done: false }); } }
        : Promise.resolve({ value: 0, done: true });
    } };
  };
  async function* outer() { yield* o; }
  (async function () {
    var out = [];
    for await (const v of outer()) { out.push(v); }
    say("58 " + order.join(",") + " " + out.join(","));
  })();
})();

// ---- 59. an async generator method of a class body, static and private ----------------------------------------
(function () {
  class C {
    async *m() { yield "m"; }
    static async *s() { yield "s"; }
    async *#p() { yield "p"; }
    run() { return this.#p(); }
  }
  async function drain(source) { var o = []; for await (const v of source) { o.push(v); } return o.join(","); }
  (async function () {
    say("59 " + await drain(new C().m()) + " " + await drain(C.s()) + " " + await drain(new C().run()));
  })();
})();

// ---- 60. an async generator method of an object literal, and a Symbol key ---------------------------------------
(function () {
  var key = Symbol("k");
  var o = { async *m() { yield "om"; } };
  o[key] = async function* () { yield "sym"; };
  async function drain(source) { var out = []; for await (const v of source) { out.push(v); } return out.join(","); }
  (async function () {
    say("60 " + await drain(o.m()) + " " + await drain(o[key]()));
  })();
})();

// ---- 61. a parameter default of an async generator ----------------------------------------------------------------
(function () {
  async function* g(a = 5, ...rest) { yield a; yield rest.length; }
  (async function () {
    var out = [];
    for await (const v of g(undefined, 1, 2)) { out.push(v); }
    say("61 " + out.join(",") + " " + g.length);
  })();
})();

// ---- 62. `arguments` and `this` inside an async generator ------------------------------------------------------------
(function () {
  var holder = { tag: "held", async *m() { yield this.tag; yield arguments.length; } };
  (async function () {
    var out = [];
    for await (const v of holder.m(1, 2, 3)) { out.push(v); }
    say("62 " + out.join(","));
  })();
})();

// ---- 63. a `for await` inside a `try` whose `finally` also awaits -----------------------------------------------------
(function () {
  var order = [];
  (async function () {
    try {
      for await (const v of [1, 2]) { order.push(v); if (v === 2) { throw new Error("t"); } }
    } catch (e) {
      order.push("caught");
    } finally {
      await 0;
      order.push("fin");
    }
    say("63 " + order.join(","));
  })();
})();

// ---- 64. every case has run --------------------------------------------------------------------------------------------
Promise.resolve()
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () {}).then(function () {}).then(function () {}).then(function () {})
  .then(function () { say("64 done"); });
