// THE FOUR OBJECTS BETWEEN AN ASYNC GENERATOR AND `Object.prototype`, and every hop is
// load-bearing. What a call answers inherits from the function's own `prototype`, which inherits
// from `%AsyncGeneratorPrototype%`, which inherits from `%AsyncIteratorPrototype%` - and it is that
// LAST one that carries `[Symbol.asyncIterator]` answering `this`, which is what makes an async
// generator an async ITERABLE without anything near it defining the Symbol.
//
// `%AsyncIteratorPrototype%` INHERITS FROM `Object.prototype` AND NOT FROM `%IteratorPrototype%`,
// which is the hop a reader is most likely to assume wrong. The two protocols are disjoint: nothing
// that iterates asynchronously has a `Symbol.iterator`, and an async generator that had one would
// answer itself to `for … of` and then fail on a `next` that returns a promise.
async function* pump() {
  yield 1;
}

var made = pump();
var generatorPrototype = Object.getPrototypeOf(pump.prototype);
var asyncIteratorPrototype = Object.getPrototypeOf(generatorPrototype);
var functionPrototype = Object.getPrototypeOf(pump);

var facts = [
  Object.prototype.toString.call(pump),
  Object.prototype.toString.call(made),
  String(Object.getPrototypeOf(made) === pump.prototype),
  Object.prototype.toString.call(generatorPrototype),
  String(Object.getPrototypeOf(asyncIteratorPrototype) === Object.prototype),
  String(typeof asyncIteratorPrototype[Symbol.asyncIterator]),
  String(made[Symbol.asyncIterator]() === made),
  Object.getOwnPropertyNames(generatorPrototype).join("+"),
  String(generatorPrototype.constructor === functionPrototype),
  String(functionPrototype.constructor.name),
  String(Object.getPrototypeOf(functionPrototype) === Function.prototype),
  String(pump.prototype.hasOwnProperty("constructor")),
  String(made.next() instanceof Promise),
];

print(facts.join(";"));
