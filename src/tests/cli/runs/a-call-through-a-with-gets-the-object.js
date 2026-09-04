// A CALL WHOSE CALLEE IS A BARE NAME RESOLVED THROUGH AN OBJECT ENVIRONMENT RECORD IS MADE AGAINST
// THAT OBJECT. `with (o) { f() }` runs `f` with `o` as its `this`, and `with ({}) { f() }` runs the
// enclosing `f` with `undefined` - which is the one thing about `with` that changes what a CALL
// means rather than what a NAME means, and the thing an implementation that only handled reads
// would get wrong while every other row still passed.
var answers = [];

// SLOPPY CODE NEVER SEES `undefined` AS ITS RECEIVER: a call with none gets the global object
// substituted, so "no receiver" below means the global rather than nothing.
var receiverless = function () { return this === globalThis ? "no receiver" : "a receiver"; };

var dispatcher = {
  label: "the dispatcher",
  reports: function () { return this.label; },
  identity: function () { return this === dispatcher; },
};

with (dispatcher) {
  answers.push(reports(), identity());
}

with ({}) {
  answers.push(receiverless());
}

// PARENTHESISING THE CALLEE DOES NOT CHANGE IT AND A COMMA DOES, exactly as for `o.f()`: the
// receiver survives a parenthesis and is lost the moment the callee stops being a reference.
with (dispatcher) {
  answers.push((identity)(), (0, identity)());
}

// AN INHERITED METHOD IS CALLED AGAINST THE OBJECT THE `with` NAMED and not against the prototype
// it was found on.
var protoHolder = { whichOne: function () { return this === subject ? "the object" : "the prototype"; } };
var subject = Object.create(protoHolder);

with (subject) {
  answers.push(whichOne());
}

// AND A NAME THE OBJECT DOES NOT HAVE REACHES THE ENCLOSING FUNCTION WITH NO RECEIVER, even while
// an object record is on the chain.
function callsOutward() {
  var mine = function () { return this === globalThis ? "no receiver" : "a receiver"; };

  with ({ somethingElse: 1 }) {
    return mine();
  }
}

answers.push(callsOutward());

print(answers.join(" "));
