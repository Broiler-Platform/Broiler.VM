// THE OBJECT IS ASKED THROUGH `HasProperty`, SO THE PROTOTYPE CHAIN COUNTS - and that is the half
// of `with` an implementation loses first, because an own-property test is the obvious one and is
// wrong. A method reached through the chain is also CALLED against the object the `with` named,
// which is the receiver rule an ordinary `o.m()` gets from its base and which a bare name inside a
// `with` body gets from the record that answered for it.
var answers = [];

function Holder() {}
Holder.prototype.inherited = "from-the-prototype";
Holder.prototype.whoAmI = function () {
  return this === subject ? "the object" : "somebody else";
};

var subject = new Holder();
var inherited = "outer";

with (subject) {
  answers.push(inherited, whoAmI());
}

// AN OBJECT WITH NO PROTOTYPE BINDS ONLY WHAT IT HAS. `toString` is on `Object.prototype`, so an
// ordinary object binds it and a null-prototype one does not - and the two answers below differ
// only because there is a local of that name for the second to fall through to. That local is what
// makes this a test of the CHAIN rather than of the realm: without it both would answer the global
// object's inherited `toString` and the difference would be invisible.
function shadowedByTheChain() {
  var toString = "my own";

  with ({}) {
    return typeof toString;
  }
}

function notShadowedByANullPrototype() {
  var toString = "my own";

  with (Object.create(null)) {
    return typeof toString;
  }
}

answers.push(shadowedByTheChain(), notShadowedByANullPrototype());

// `Symbol.unscopables` HIDES A NAME THE OBJECT HAS, and the language added it for one reason: new
// members of `Array.prototype` broke pages that wrote `with (someArray) { values }` against a
// variable of their own. So the blocklist is read off the object, prototype chain included, and a
// name listed truthily there is NOT taken from the object.
var values = "my own values";
with ([1, 2, 3]) {
  answers.push(values, typeof join, length);
}

var hidden = "outer";
var blocking = { hidden: "on the object" };
blocking[Symbol.unscopables] = { hidden: true };

with (blocking) {
  answers.push(hidden);
}

// A FALSY ENTRY HIDES NOTHING. The blocklist is consulted with `ToBoolean`, so `{ hidden: false }`
// leaves the name bound - which is the case an implementation that tested for presence gets wrong.
blocking[Symbol.unscopables] = { hidden: false };

with (blocking) {
  answers.push(hidden);
}

print(answers.join(" "));
