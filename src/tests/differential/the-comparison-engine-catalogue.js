// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER WHAT MUST NOT BE TAKEN FROM THE COMPARISON ENGINE.
//
// The parity roadmap's section 4.9 catalogues the places the legacy engine, Broiler.JS, answers in a
// way the language does not permit. JSP-9 asks that each be a declared divergence in the retained
// answers, so that a comparison run reports it as known rather than as a finding. Most entries are
// already asked by the probes beside this one, and their answer files carry the declarations. This
// probe asks the entries nothing else asked: the `console` object, what an error hands to guest
// code, one enumeration of own names, `for … in` through a `Proxy`, a name on the global object, and
// the poisoned global variable store.
//
// THE POISONING IS DELIBERATE AND IT IS LAST. Under Broiler.JS a `with` whose head completes abruptly
// makes every later indirect `eval` restore the global `var` bindings to what they were when the
// head threw. Every answer after it in the same process is untrustworthy, so nothing may follow the
// two cases that show it. The case counter is a closure's variable and not a global `var` for the
// same reason: a global counter is restored too, and the cases after it are numbered again.
//
// Case 1 is written by `console.log` itself and not by `print`, because what it asks is whether
// `console.log` writes. An engine whose `console.log` writes nothing and returns its argument
// answers the case through the fallback `print`, with a different line.

var t = function (f) {
  try {
    var v = f();
    return typeof v === "string" ? JSON.stringify(v) : String(v);
  } catch (e) {
    return e.name;
  }
};

var p = (function () {
  var n = 1;
  return function (f) {
    n++;
    print(n + " " + t(f));
  };
})();

// 1. `console.log` writes its arguments and returns `undefined`.
var logged = console.log("1 written");
if (logged !== undefined) {
  print("1 returned " + String(logged));
}

// 2. The namespace object's own names can be listed. 3. And the message of an error raised for it
// names no platform type.
p(function () { return Object.getOwnPropertyNames(console).length > 0; });
p(function () {
  try {
    Object.getOwnPropertyNames(console);
    return "no throw";
  } catch (e) {
    return /\b(System|Broiler)\.[A-Z][A-Za-z]*\./.test(e.message);
  }
});

// 4. An error's `stack`, where there is one, names no source file of the engine's own build.
p(function () {
  try {
    null.x;
  } catch (e) {
    return /\.cs\b/.test(String(e.stack));
  }
});

// 5. `[[OwnPropertyKeys]]` lists each key once.
p(function () {
  var names = Object.getOwnPropertyNames(String.prototype);
  return names.length - new Set(names).size;
});

// 6. `for … in` reads the keys of every object on the prototype chain, a proxy's through its traps.
p(function () {
  var calls = 0;
  var proxy = new Proxy({ a: 1 }, {
    ownKeys: function (target) { calls++; return Reflect.ownKeys(target); },
    getOwnPropertyDescriptor: function (target, key) { calls++; return Reflect.getOwnPropertyDescriptor(target, key); },
  });
  var keys = [];
  for (var key in Object.create(proxy)) {
    keys.push(key);
  }
  return keys.join() + " " + (calls > 0);
});

// 7. The global object has no property named for a reserved word.
p(function () { return Object.prototype.hasOwnProperty.call(globalThis, "import"); });

// 8 and 9. The poisoned global variable store. NOTHING MAY FOLLOW THESE.
var poisoned = 1;
try {
  with ((function () { throw "head"; })()) {
  }
} catch (e) {
}
poisoned = 2;
p(function () { (0, eval)("0"); return poisoned; });
p(function () { poisoned = 3; (0, eval)("0"); return poisoned; });
