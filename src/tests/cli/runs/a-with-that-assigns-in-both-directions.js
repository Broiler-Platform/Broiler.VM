// AN ASSIGNMENT ASKS THE SAME OBJECTS A READ ASKS, AND THE TWO ANSWERS GO TO DIFFERENT PLACES.
// A name the object has is a write to the object's property - through the ordinary setter path, so
// an inherited accessor runs and an inherited data property is shadowed rather than replaced - and
// a name it does not have reaches the enclosing binding. Getting this half right and the read half
// wrong is the shape a `with` implementation usually fails in.
var answers = [];

var target = { held: "before" };
var held = "outer-before";

with (target) {
  held = "written";
}

answers.push(target.held, held);

with ({}) {
  held = "written-outside";
}

answers.push(held);

// A `var` DECLARATION INSIDE THE BODY IS STILL THE ENCLOSING FUNCTION'S BINDING, and its
// INITIALISER is still an assignment - so `var x = 1` inside a `with` whose object has `x` writes
// the object's property and leaves the variable alone.
function declaresInside() {
  var shared = "the variable";
  var object = { shared: "the property" };

  with (object) {
    var shared = "assigned";
  }

  return object.shared + "/" + shared;
}

answers.push(declaresInside());

// AN INHERITED DATA PROPERTY IS SHADOWED. The write lands on the object the `with` named, which is
// what makes the prototype's value survive.
var base = { inheritedSlot: "on the prototype" };
var derived = Object.create(base);

with (derived) {
  inheritedSlot = "on the instance";
}

answers.push(base.inheritedSlot, derived.inheritedSlot, derived.hasOwnProperty("inheritedSlot"));

// AN INHERITED SETTER RUNS, with the object as its receiver.
var accessorBase = {};
Object.defineProperty(accessorBase, "watched", {
  get: function () { return this.recorded; },
  set: function (value) { this.recorded = "set:" + value; },
  configurable: true,
});

var watcher = Object.create(accessorBase);

with (watcher) {
  watched = 7;
}

answers.push(watcher.recorded, watcher.hasOwnProperty("watched"));

// A COMPOUND ASSIGNMENT AND AN UPDATE ARE A READ AND A WRITE, and both halves resolve the same way.
var counted = { n: 1 };
var n = 100;

with (counted) {
  n += 1;
  n++;
}

answers.push(counted.n, n);

print(answers.join(" "));
