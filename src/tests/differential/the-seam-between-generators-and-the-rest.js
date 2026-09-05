// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// THE SEAM BETWEEN GENERATORS AND EVERYTHING THIS BRANCH ALREADY HAD.
//
// Every case prints one numbered line. Nothing here is about generators alone and nothing here is
// about the rest alone: each case is a composition of the two bundles, which is the region neither
// bundle's own probe could look at.

var n = 0;

function say(what) {
  n = n + 1;
  print(n + " " + what);
}

function fails(thunk) {
  try {
    var value = thunk();
    return "no-throw:" + value;
  } catch (failure) {
    return failure.name;
  }
}

// ---- 1: for … of over a generator ------------------------------------------------------------

function* small() { yield 1; yield 2; yield 3; }

var collected = [];
for (var v of small()) { collected.push(v); }
say(collected.join(","));

var sum = 0;
for (const w of small()) { sum += w; }
say(sum);

// The loop variable is a fresh binding per turn, which a closure over it observes.
var closures = [];
for (const each of small()) { closures.push(function () { return each; }); }
say(closures.map(function (f) { return f(); }).join(""));

// An empty generator runs the body no times.
function* nothing() { return 7; }
var ran = 0;
for (var _ of nothing()) { ran++; }
say(ran);

// A generator's RETURN VALUE is not a for … of element.
var seen = [];
function* withReturn() { yield "a"; return "b"; }
for (var r of withReturn()) { seen.push(r); }
say(seen.join(","));

// ---- 2: spread and Array.from ------------------------------------------------------------------

say([...small()].join(","));
say(Array.from(small()).join(","));
say(Array.from(small(), function (x) { return x * 10; }).join(","));
say([0, ...small(), 4].join(","));
say([...small(), ...small()].length);
say(Math.max(...small()));

function counted() { return arguments.length + ":" + [].slice.call(arguments).join(""); }
say(counted(...small()));

// A generator spread into an object literal spreads its OWN properties, not its elements.
say(Object.keys({ ...small() }).length);

// ---- 3: destructuring from a generator ---------------------------------------------------------

var [first, second] = small();
say(first + "," + second);

var [only, ...rest] = small();
say(only + "|" + rest.join(","));

var [, middle] = small();
say(middle);

var [p, q, r2, s = "default"] = small();
say([p, q, r2, s].join(","));

// A nested pattern over a generator of arrays.
function* pairs() { yield [1, 2]; yield [3, 4]; }
var [[pa, pb], [pc, pd]] = pairs();
say([pa, pb, pc, pd].join(""));

// A parameter list destructures a generator too.
function takes([a, b, ...more]) { return a + "/" + b + "/" + more.length; }
say(takes(small()));

// A rest element consumes the iterator, so nothing is closed after it.
function* noticing(log) {
  try { yield 1; yield 2; yield 3; } finally { log.push("finally"); }
}
var log1 = [];
var [head, ...tail] = noticing(log1);
say(head + ":" + tail.join("") + ":" + log1.join(""));

// A partial destructuring DOES close the generator, which its finaliser observes.
var log2 = [];
var [justOne] = noticing(log2);
say(justOne + ":" + log2.join(""));

// ---- 4: the generator method of an OBJECT LITERAL, which this bundle admits ---------------------
//
// A generator member of a CLASS body is not admitted and is refused by name; that refusal cannot be
// checked here, because the comparison engine admits it and the case would diverge. It is checked
// against the front end's own diagnostic instead, outside this probe.

var literalGenerator = { *m() { yield "lit"; yield "eral"; } };
say([...literalGenerator.m()].join(""));
say(typeof literalGenerator.m);
say(literalGenerator.m.name);

var computedKey = "dyn";
var computedGenerator = { *[computedKey]() { yield 1; yield 2; } };
say([...computedGenerator.dyn()].join(","));

// A literal generator method sees the literal's own `this` through the receiver it is called on.
var withThis = { base: 5, *scaled() { yield this.base; yield this.base * 2; } };
say([...withThis.scaled()].join(","));
say(Object.getPrototypeOf(withThis.scaled()) === withThis.scaled.prototype);

// ---- 5: yield* over everything with a Symbol.iterator ------------------------------------------

function* overArray() { yield* [1, 2, 3]; }
say([...overArray()].join(","));

function* overString() { yield* "abc"; }
say([...overString()].join(","));

function* overMap() { yield* new Map([["k", 1], ["j", 2]]); }
say(JSON.stringify([...overMap()]));

function* overMapKeys() { yield* new Map([["k", 1], ["j", 2]]).keys(); }
say([...overMapKeys()].join(","));

function* overSet() { yield* new Set([3, 1, 3, 2]); }
say([...overSet()].join(","));

function* inner() { yield "i"; yield "j"; return "returned"; }
function* overGenerator() { var got = yield* inner(); yield got; }
say([...overGenerator()].join(","));

var userIterable = {};
userIterable[Symbol.iterator] = function () {
  var at = 0;
  return { next: function () { at++; return at <= 3 ? { value: at * 11, done: false } : { value: "end", done: true }; } };
};
function* overUser() { var tail2 = yield* userIterable; yield "tail=" + tail2; }
say([...overUser()].join(","));

function* overArguments() { yield* (function () { return arguments; })(5, 6); }
say([...overArguments()].join(","));

// A `yield*` over something with no Symbol.iterator is a TypeError, and over a plain object too.
function* overPlain() { yield* { length: 2, 0: "a", 1: "b" }; }
say(fails(function () { return [...overPlain()]; }));

function* overNumber() { yield* 5; }
say(fails(function () { return [...overNumber()]; }));

// The value `yield*` evaluates to is the inner iterator's own return value, undefined for an Array.
function* arrayResult() { var got = yield* [1]; yield "got=" + got; }
say([...arrayResult()].join(","));

// A `yield*` chain three deep, which is the shape the delegation state has to survive.
function* level0() { yield 0; }
function* level1() { yield* level0(); yield 1; }
function* level2() { yield* level1(); yield 2; }
say([...level2()].join(","));

// A `next(v)` reaches THROUGH a delegation into the inner generator's own `yield`.
function* echo() { var got = yield "ask"; yield "heard " + got; }
function* delegating() { yield* echo(); }
var d = delegating();
say(d.next().value + "/" + d.next("answer").value);

// ---- 6: a generator body using the rest of this branch ------------------------------------------

function* everything(base = 10, ...others) {
  var { tag = "T", extra } = { extra: "E" };
  yield base;
  yield tag + extra;

  for (const each of others) {
    yield each * 2;
  }

  var [x, ...ys] = others;
  yield x + "&" + ys.join("");
  yield `template ${base} ${others.length}`;
  yield others.map((one) => one + base).join("+");
}
say([...everything()].join("|"));
say([...everything(1, 2, 3)].join("|"));

// `arguments` inside a generator body, which the compiler's own walk had to learn to see.
function* usesArguments() { yield arguments.length; yield arguments[0]; }
say([...usesArguments("x", "y")].join(","));

// A generator over a keyed collection, spread back into one. The Map is built from an ARRAY of the
// generator's output rather than from the generator: this realm's collection constructors read an
// array-like and do not run the iteration protocol, which is a gap of its own and not this seam's.
function* entriesOf(map) { for (const [k, val] of map) { yield [k, val * 2]; } }
say(JSON.stringify([...new Map([...entriesOf(new Map([["a", 1], ["b", 2]]))])]));

// A generator whose body optional-chains and uses `??`-shaped defaults.
function* optional(o) { yield o?.a?.b; yield o?.missing ?? "fallback"; }
say([...optional({ a: { b: "deep" } })].join(","));

// A generator driving a Promise job queue, which must interleave with the microtask drain.
var order = [];
function* steps() { order.push("one"); yield 1; order.push("two"); yield 2; }
var driver = steps();
Promise.resolve().then(function () { order.push("job"); });
driver.next();
driver.next();
say(order.join(","));

// ---- 7: a generator made inside a class method, closing over `this` and `super` -----------------

class Base {
  constructor() { this.tag = "base"; }
  greet() { return "Base"; }
}

class Derived extends Base {
  constructor() { super(); this.tag = "derived"; }

  make() {
    const self = this;
    const fromSuper = () => super.greet();

    return function* () {
      yield self.tag;
      yield fromSuper();
      yield new.target === undefined;
    };
  }

  spreadIntoSuper() { return [...small()]; }
}

var made = new Derived().make();
say([...made()].join(","));
say(new Derived().spreadIntoSuper().join(","));

// A spread of a generator INTO a `super(...)` call.
class Sums {
  constructor(a, b, c) { this.total = a + b + c; }
}

class SumsFromGenerator extends Sums {
  constructor() { super(...small()); }
}
say(new SumsFromGenerator().total);

class SumsForwarding extends Sums {
  constructor(...args) { super(...args); }
}
say(new SumsForwarding(...small()).total);

// A generator function is not a constructor, and its `prototype` has no `constructor` back-link.
function* plain() { yield 1; }
say(fails(function () { return new plain(); }));
say(Object.prototype.hasOwnProperty.call(plain.prototype, "constructor"));

// ---- 8: return() inside a try/finally that itself yields ----------------------------------------

function* finallyYields() {
  try {
    yield "body";
  } finally {
    yield "from the finaliser";
  }
}
var fy = finallyYields();
say(fy.next().value);
var forced = fy.return("forced");
say(forced.value + "/" + forced.done);
var after = fy.next();
say(after.value + "/" + after.done);

function* finallyRuns() {
  var order2 = [];
  try {
    try { yield "deep"; } finally { order2.push("inner"); }
  } finally { order2.push("outer"); yield order2.join(">"); }
}
var fr = finallyRuns();
fr.next();
say(fr.return("x").value);

// A `catch` must NOT see a forced return.
function* catchDoesNotSee() {
  try { yield 1; } catch (e) { yield "caught " + e; } finally { yield "finalised"; }
}
var cd = catchDoesNotSee();
cd.next();
say(cd.return("gone").value);
say(JSON.stringify(cd.next()));

// return() on a generator that has not started skips the body entirely.
var log3 = [];
var early = noticing(log3);
say(JSON.stringify(early.return("early")) + ":" + log3.length);

// ---- 9: throw() caught by the generator's own catch ---------------------------------------------

function* catches() {
  try { yield "waiting"; } catch (failure) { yield "caught " + failure.message; }
  yield "after";
}
var c1 = catches();
c1.next();
say(c1.throw(new Error("BANG")).value);
say(c1.next().value);

// An uncaught throw() completes the generator and reaches the caller.
var c2 = catches();
say(fails(function () { return c2.throw(new Error("early")); }));
say(JSON.stringify(c2.next()));

// A throw() forwarded through a delegation into the inner generator's own catch.
function* innerCatches() {
  try { yield "inner"; } catch (failure) { yield "inner caught " + failure.message; }
  return "inner done";
}
function* outerDelegates() { var got = yield* innerCatches(); yield "outer got " + got; }
var od = outerDelegates();
od.next();
say(od.throw(new Error("through")).value);
say(od.next().value);

// An inner iterator with no `throw` is closed and the delegation raises a TypeError.
function* overArrayThrown() { yield* [1, 2, 3]; }
var oat = overArrayThrown();
oat.next();
say(fails(function () { return oat.throw(new Error("nope")); }));

// An inner iterator with no `return` does not swallow the outer one.
var oar = overArrayThrown();
oar.next();
say(JSON.stringify(oar.return("outer wins")));

// ---- 10: break in a for … of calls the generator's return ----------------------------------------

var log4 = [];
for (var b of noticing(log4)) { if (b === 2) { break; } }
say(log4.join(",") + ":" + log4.length);

var log5 = [];
try {
  for (var t of noticing(log5)) { if (t === 2) { throw new Error("out"); } }
} catch (ignored) { /* the finaliser still ran */ }
say(log5.join(","));

var log6 = [];
function returningFrom() { for (var u of noticing(log6)) { return u; } }
say(returningFrom() + ":" + log6.join(","));

var log7 = [];
for (var f2 of noticing(log7)) { /* to exhaustion */ }
say(log7.join(","));

// A labelled break out of a nested for … of closes the generator it left.
var log8 = [];
outer: for (var g1 of noticing(log8)) { for (var g2 of small()) { break outer; } }
say(log8.join(","));

// ---- 11: the prototype chain and the tags -------------------------------------------------------

say(Object.prototype.toString.call(small()));
say(Object.prototype.toString.call(small));
say(Object.getPrototypeOf(small()) === small.prototype);
say(Object.getPrototypeOf(small.prototype) === Object.getPrototypeOf(Object.getPrototypeOf(small())));
say(Object.getOwnPropertyNames(Object.getPrototypeOf(small.prototype)).join(","));
say(typeof Object.getPrototypeOf(small.prototype).constructor);
say(Object.getPrototypeOf(small).constructor.name);
say(typeof Object.getPrototypeOf(small).constructor);
say(Object.getPrototypeOf(small).constructor.prototype === Object.getPrototypeOf(small));
say(Object.getPrototypeOf(Object.getPrototypeOf(small)) === Function.prototype);

// A generator IS an iterable iterator through the real Symbol.
var live = small();
say(live[Symbol.iterator]() === live);
say(typeof small.prototype[Symbol.iterator]);
say(small()[Symbol.toStringTag]);
say(typeof Object.getPrototypeOf(Object.getPrototypeOf(small()))[Symbol.iterator]);

// The generator's own %IteratorPrototype% is the realm's ONE, shared with every other iterator.
// It is asked as a prototype-chain question rather than as an identity against a fixed number of
// hops, because how many links a built-in iterator's chain has is a separate question from whether
// they end in the same place - and it is the same place that this seam is about.
var generatorRoot = Object.getPrototypeOf(Object.getPrototypeOf(Object.getPrototypeOf(small())));
say(generatorRoot.isPrototypeOf([][Symbol.iterator]()));
say(generatorRoot.isPrototypeOf(new Map()[Symbol.iterator]()));
say(generatorRoot.isPrototypeOf(new Set()[Symbol.iterator]()));
say(generatorRoot.isPrototypeOf(""[Symbol.iterator]()));
say(generatorRoot.isPrototypeOf(small()));
say(typeof generatorRoot[Symbol.iterator]);
say(generatorRoot[Symbol.iterator].call(generatorRoot) === generatorRoot);
say(Object.getPrototypeOf(generatorRoot) === Object.prototype);

// Replacing g.prototype changes what a fresh generator object inherits from.
function* replaceable() { yield 1; }
var mine = { marker: "mine" };
replaceable.prototype = mine;
say(Object.getPrototypeOf(replaceable()) === mine);
say(fails(function () { return replaceable().next(); }));

// ---- 12: the state machine, re-entry and the rest ------------------------------------------------

function* reenters() { yield me.next(); }
var me = reenters();
say(fails(function () { return me.next(); }));
say(JSON.stringify(me.next()));

var done1 = small();
say([...done1].join(",") + ":" + JSON.stringify(done1.next()));

say(JSON.stringify(small().next()));
say(fails(function () { return Object.getPrototypeOf(small()).next.call({}); }));
say(fails(function () { return small().next.call(undefined); }));

// A generator is not callable and has no `length` of its own beyond its arity.
say(small.length + "," + everything.length + "," + small.name);

// A generator in a chain of ordinary iteration helpers.
say([...small()].filter(function (x) { return x > 1; }).join(","));
say(new Set([...small()]).size);
say(new Map([...pairs()]).get(3));
say(Array.from(new Set([...small(), ...small()])).join(","));

// A generator over a regular expression's matches, which composes the two newest families.
function* matches(text, pattern) {
  var found;
  while ((found = pattern.exec(text)) !== null) { yield found[0]; }
}
say([...matches("a1b22c333", /[0-9]+/g)].join(","));

// A generator inside a template literal substitution.
say(`spread ${[...small()].join("")} done`);

// A generator yielding into a tagged template's arguments.
function tag(strings, ...values) { return strings.raw.join("|") + "#" + values.join(","); }
say(tag`x${[...small()][0]}y`);

// `typeof` a generator function and a generator object.
say(typeof small + "," + typeof small());
say(small instanceof Function);
say(small() instanceof small);

// ---- the parameter list is bound at the CALL, and the body is not ------------------------------
//
// The seam this file is named for runs through a generator's own entry as well: a parameter list
// that has to run code - a default, a rest parameter, a pattern - is `FunctionDeclarationInstantiation`
// and belongs to the CALL, while the body belongs to the first `next`. Recorded with JSC-220.

function* patterned([x]) { yield x; }
say(fails(function () { return patterned(undefined); }));
say(fails(function () { return patterned([7]).next().value; }));

var order = "";
function* defaulted(a = (order += "d", 1)) { order += "b"; yield a; }
var pending = defaulted();
order += "m";
pending.next();
say(order);

// A generator whose parameter binding threw yields no object to resume.
var threw = "none";
function* boomed(a = (function () { throw new RangeError("param"); })()) { yield 1; }
try { boomed(); } catch (e) { threw = e.name; }
say(threw);

// A pattern steps its iterator at the call, not at the first resumption.
var steps = 0;
var counted = {};
counted[Symbol.iterator] = function () {
  return { next: function () { steps += 1; return { value: steps, done: steps > 2 }; } };
};
function* pair([p, q]) { yield p + q; }
var held = pair(counted);
say(steps + "," + held.next().value);

// A generator that has not started swallows `return` and rethrows `throw`, and its binding already ran.
var ranBinding = 0;
function* watched([w] = (ranBinding += 1, [5])) { ranBinding += 10; yield w; }
var early = watched();
var returned = early.return(3);
say(ranBinding + "," + returned.value + "," + returned.done);
say(fails(function () { return watched().throw(new RangeError("early")); }));

// An ASYNC generator binds at the call too, and an async FUNCTION does not: its promise exists
// first, so a failing default settles that promise instead of throwing where it was written.
say(fails(function () { return (async function* ([y]) { yield y; })(undefined); }));
say(fails(function () { return typeof (async function ([z]) { return z; })(undefined).catch(function () {}); }));

// A generator over a SIMPLE parameter list still runs none of its body at the call.
var plain = "";
function* simple(a) { plain += "b"; yield a; }
var idle = simple(1);
plain += "m";
say(plain + "," + idle.next().value);

// The whole of a generator's ordinary behaviour survives the split.
function* whole({ a } = { a: 10 }) { var s = yield a; yield s * 2; return "end"; }
var w = whole();
say(w.next().value + "," + w.next(3).value + "," + w.next().value + "," + w.next().done);

// A method's parameter list is bound at the call in each of the three places one can be written.
var methods = { *m([k]) { yield k; } };
say(fails(function () { return methods.m(undefined); }));
class Holder { *m([k]) { yield k; } static *s({ k }) { yield k; } }
say(fails(function () { return new Holder().m(undefined); }));
say(fails(function () { return Holder.s(undefined); }));
