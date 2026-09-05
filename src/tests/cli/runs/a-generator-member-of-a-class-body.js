// THE POSITION THE TWO BUNDLES ONCE LEFT BETWEEN THEM, now admitted. A `*` before a key in a CLASS
// body is a generator member, and it is the object literal's generator method plus the two things
// only a class gives it: a `super` that resolves through the home object, and a static form that
// lands on the constructor. Neither is reachable from the object-literal row beside it, which is
// why the class member needs a row of its own rather than being covered by the modifier. A
// generator is not a constructor either, which is the last clause and the one a flag decides.
class Source {
  base() { return 10; }
}

class Counted extends Source {
  step = 2;

  *from(start) {
    for (var at = 0; at < 3; at++) {
      yield super.base() + start + (at * this.step);
    }
  }

  static *labels() {
    yield "a";
    yield "b";
  }
}

var counted = new Counted();
var running = counted.from(1);
var refusal;

try {
  new counted.from(1);
  refusal = "none";
} catch (error) {
  refusal = error.constructor.name;
}

[...counted.from(1)].join(",") + " / " +
  [...Counted.labels()].join(",") + " / " +
  typeof counted.from + " " + typeof running.next + " " +
  (running[Symbol.iterator]() === running) + " / " + refusal;
