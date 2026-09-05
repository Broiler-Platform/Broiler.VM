// THE THREE THINGS `for await` CAN BE POINTED AT, and only one of them has a
// `Symbol.asyncIterator`. An async generator has one and is driven directly. An ARRAY OF PROMISES
// has none - it has `Symbol.iterator` - so the head wraps the synchronous iterator in one that
// awaits each VALUE, which is the whole difference between iterating promises and iterating what
// they resolve to. A hand-written synchronous iterable of thenables goes down the same wrapper and
// proves the wrapper is not an Array special case.
//
// A THENABLE THAT IS NOT A PROMISE IS IN HERE ON PURPOSE. The wrapper resolves each value through
// the promise machinery rather than testing its type, so an object with a `then` is waited on
// exactly as a promise is.
async function* generated() {
  yield "g1";
  yield "g2";
}

function thenable(value) {
  return {
    then: function (resolve) {
      resolve(value);
    },
  };
}

function syncSource() {
  var at = 0;

  return {
    [Symbol.iterator]: function () {
      return {
        next: function () {
          at = at + 1;
          return at <= 2
            ? { value: thenable("t" + at), done: false }
            : { value: undefined, done: true };
        },
      };
    },
  };
}

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(generated()).then(function (fromGenerator) {
  return drain([Promise.resolve("p1"), "p2", Promise.resolve("p3")]).then(function (fromArray) {
    return drain(syncSource()).then(function (fromSync) {
      print([fromGenerator, fromArray, fromSync].join(";"));
    });
  });
});
