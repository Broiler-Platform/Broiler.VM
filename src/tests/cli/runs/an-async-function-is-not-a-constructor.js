// WHAT AN ASYNC FUNCTION IS, AS AN OBJECT. It is not constructible and has no `prototype` property,
// which is the pair a reader is most likely to assume the other way round; it inherits from
// `%AsyncFunction.prototype%` rather than from `Function.prototype`, which is what makes the tag
// below `AsyncFunction`; and an async ARROW answers the same, because the async bit decides the
// prototype and the arrow bit does not.
async function declared(a, b) {}

var arrow = async (x) => x;
var method = { async member(a) {} }.member;
var refused = "none";

try {
  new declared();
} catch (e) {
  refused = e.constructor.name;
}

var proto = Object.getPrototypeOf(declared);

print(
  refused + " " +
  (declared.prototype === undefined) + " " +
  declared.length + " " +
  declared.name + " " +
  (proto === Function.prototype) + " " +
  (Object.getPrototypeOf(proto) === Function.prototype) + " " +
  proto.constructor.name + " " +
  Object.prototype.toString.call(declared) + " " +
  Object.prototype.toString.call(arrow) + " " +
  method.name);
