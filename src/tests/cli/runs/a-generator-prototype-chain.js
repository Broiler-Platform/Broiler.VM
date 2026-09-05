// THE PROTOTYPES, WIRED THE WAY THE LANGUAGE WIRES THEM. A generator object's prototype is the
// generator function's own `prototype` property; that object has no `constructor` of its own and
// inherits one that is an OBJECT rather than a function; and a generator function is not a
// constructor. Each of the four is a thing an implementation gets wrong by copying the ordinary
// function's wiring, and each would be invisible to a program that only called `next`.
function* g() {
  yield 1;
}

var made = g();
var generatorPrototype = Object.getPrototypeOf(g.prototype);
var notAConstructor;

try {
  new g();
  notAConstructor = false;
} catch (e) {
  notAConstructor = e.constructor.name === "TypeError";
}

[
  Object.getPrototypeOf(made) === g.prototype,
  Object.getOwnPropertyNames(g.prototype).length,
  g.prototype.constructor === Object.getPrototypeOf(g),
  typeof g.prototype.constructor,
  Object.getOwnPropertyNames(generatorPrototype).join(","),
  Object.prototype.toString.call(made),
  notAConstructor,
].join(" ");
