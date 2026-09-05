// A `return` AND A `throw` ARRIVING WHILE A DELEGATION IS SUSPENDED ARE OFFERED TO THE INNER
// ITERATOR FIRST, and that is the property the delegation exists for rather than an optimisation of
// it. The consumer asked the OUTER generator to stop; the inner one is the thing actually producing,
// so it is the thing told - and only what it answers decides whether the outer one ends.
//
// AN INNER ITERATOR WITH NO `throw` IS CLOSED FIRST AND TOLD SECOND. Closing gives it its chance to
// clean up before it is informed that it violated the protocol, which is the order the language is
// explicit about; the `TypeError` that follows is about the missing method and not about the value
// that was thrown.
var trace = [];

function inner() {
  var at = 0;

  return {
    [Symbol.asyncIterator]: function () {
      return {
        next: function () {
          at = at + 1;
          return Promise.resolve({ value: "n" + at, done: false });
        },
        throw: function (error) {
          trace.push("inner-throw:" + error.message);
          return Promise.resolve({ value: "recovered", done: false });
        },
        return: function (value) {
          trace.push("inner-return:" + value);
          return Promise.resolve({ value: value, done: true });
        },
      };
    },
  };
}

function withoutThrow() {
  return {
    [Symbol.asyncIterator]: function () {
      return {
        next: function () {
          return Promise.resolve({ value: 1, done: false });
        },
        return: function () {
          trace.push("closed-before-told");
          return Promise.resolve({ done: true });
        },
      };
    },
  };
}

async function* delegating(source) {
  yield* source;
}

var forwarded = delegating(inner());

forwarded.next().then(function (first) {
  trace.push("first:" + first.value);
  return forwarded.throw(new Error("E"));
}).then(function (recovered) {
  trace.push("recovered:" + recovered.value + ":" + recovered.done);
  return forwarded.return("R");
}).then(function (ended) {
  trace.push("ended:" + ended.value + ":" + ended.done);

  var missing = delegating(withoutThrow());
  return missing.next().then(function () {
    return missing.throw(new Error("X"));
  }).then(
    function () {
      trace.push("missing:resolved");
    },
    function (error) {
      trace.push("missing:" + error.constructor.name);
    });
}).then(function () {
  print(trace.join(","));
});
