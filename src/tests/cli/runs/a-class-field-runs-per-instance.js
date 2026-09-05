// A FIELD HAS TWO TIMES IN IT AND THIS PINS BOTH. The KEY is evaluated once, when the class is
// defined and in body order; the INITIALISER runs once per instance, later, with `this` bound to
// the object being constructed and BEFORE the constructor body. A lowering that collapsed the two
// would get one of the orders right and this answer wrong, which is why the computed keys push
// into the same log the initialisers do.
var log = [];

function key(name) {
  log.push("key:" + name);
  return name;
}

class Counted {
  [key("a")] = log.push("init:a");
  bare;
  [key("b")] = this.constructor.name;

  constructor() {
    log.push("ctor");
  }
}

var first = new Counted();
var second = new Counted();
var shape = Object.getOwnPropertyDescriptor(first, "a");

log.join(" ") + " / " +
  first.a + " " + String(first.bare) + " " + first.b + " / " +
  Object.keys(first).join(",") + " / " +
  (first.a === second.a) + " / " +
  shape.writable + shape.enumerable + shape.configurable;
