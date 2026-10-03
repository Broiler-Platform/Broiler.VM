// `Function.prototype[Symbol.hasInstance]` EXISTS AND CANNOT BE CHANGED (JSP-6, JSC-238). It is
// `OrdinaryHasInstance`, neither writable nor configurable, so assigning to it changes nothing,
// where the assignment created the property and changed `instanceof` for every function in the
// realm. A constructor's own `Symbol.hasInstance` still decides for that constructor.
function Point() {}

var descriptor = Object.getOwnPropertyDescriptor(Function.prototype, Symbol.hasInstance);
var method = Function.prototype[Symbol.hasInstance];

Function.prototype[Symbol.hasInstance] = function () {
  return true;
};

var everything = {
  [Symbol.hasInstance]: function () {
    return true;
  },
};

[
  typeof descriptor.value,
  descriptor.writable + ',' + descriptor.enumerable + ',' + descriptor.configurable,
  method.name,
  method.length,
  method.call(Point, new Point()),
  ({}) instanceof Point,
  1 instanceof everything,
].join(' / ');
