// EVERY ABRUPT EXIT OWES THE ITERATOR ITS `return`, and running out does not: an iterator that
// reported completion is not asked to clean up after itself. `break` and a `throw` from the body
// are the two shapes here; `return` and a labelled `break` take the same path.
//
// The key is the real well-known Symbol. This surface has a Symbol primitive, so the protocol is
// keyed on `Symbol.iterator` and nothing else answers to it.
var ITERATOR = Symbol.iterator;
var log = [];

function watched(values) {
  var source = {};

  source[ITERATOR] = function () {
    var at = 0;

    return {
      next: function () {
        return at < values.length
          ? { value: values[at++], done: false }
          : { value: undefined, done: true };
      },
      return: function () {
        log.push("closed");
        return {};
      },
    };
  };

  return source;
}

for (const value of watched([1, 2, 3])) {
}

log.push("next");

for (const value of watched([1, 2, 3])) {
  break;
}

log.push("next");

try {
  for (const value of watched([1, 2, 3])) {
    throw new RangeError("out");
  }
} catch (error) {
  log.push(error.name);
}

log.join(",");
