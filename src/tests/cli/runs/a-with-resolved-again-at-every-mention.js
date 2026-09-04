// NOTHING ABOUT A NAME INSIDE A `with` BODY IS CACHED, AND THAT IS THE BEHAVIOUR RATHER THAN A
// MISSING OPTIMISATION. The object may gain the property between two reads in the same body and
// lose it again, and the language says each read sees the state at the moment it runs. An
// implementation that resolved once and remembered where would answer the first reading three
// times - which is the defect this file exists to catch, and it is a WRONG ANSWER rather than a
// refusal, so nothing but a comparison finds it.
var answers = [];

var subject = {};
var moving = "the enclosing binding";

with (subject) {
  answers.push(moving);
  subject.moving = "the property";
  answers.push(moving);
  delete subject.moving;
  answers.push(moving);
}

// THE SAME FOR A WRITE. The first assignment reaches the enclosing binding because the object has
// nothing; the second reaches the property, because by then it does.
var destination = {};
var routed = "start";

with (destination) {
  routed = "first";
  destination.routed = "planted";
  routed = "second";
}

answers.push(routed, destination.routed);

// AND FOR A CALL, whose callee and whose receiver are both decided at the call.
var dispatch = {};
function outerCall() { return "outer:" + (this === undefined); }

with (dispatch) {
  answers.push(outerCall());
  dispatch.outerCall = function () { return "inner:" + (this === dispatch); };
  answers.push(outerCall());
}

// A CLOSURE MADE INSIDE THE BODY KEEPS THE RECORD, so it asks the object when it is CALLED - long
// after the statement finished. That is what makes `with` unoptimisable and is why the record is
// on the ordinary scope chain rather than being a compile-time fiction.
var captured = { later: "at capture" };
var reader;
var later = "the enclosing binding";

with (captured) {
  reader = function () { return later; };
}

answers.push(reader());
captured.later = "changed after the with ended";
answers.push(reader());
delete captured.later;
answers.push(reader());

// A GETTER RUNS ONCE PER MENTION, which is the visible cost of resolving again each time.
var counted = 0;
var counter = {};
Object.defineProperty(counter, "reads", {
  get: function () { counted++; return counted; },
  configurable: true,
});

with (counter) {
  reads;
  reads;
}

answers.push(counted);

print(answers.join(" "));
