// THE TWO ABRUPT RESUMPTIONS, AND THE AWAIT NEITHER OF THEM LOOKS LIKE IT PERFORMS. `return(v)`
// into a body suspended at a `yield` waits for `v` BEFORE the body's own `finally` runs, so a
// finaliser that observes the world sees it after `v` has settled - which is why `awaited` is in
// the trace before `finalised`. A synchronous generator's `return` performs no such wait.
//
// AND EVERY ANSWER IS A PROMISE, including the ones that are errors: `throw` into a body that has
// not started REJECTS rather than throwing at the call site, because the method's contract is that
// it answers something with a `then`.
var trace = [];

async function* held() {
  try {
    yield 1;
    yield 2;
  } finally {
    trace.push("finalised");
  }
}

function waited(value) {
  return {
    then: function (resolve) {
      trace.push("awaited");
      resolve(value);
    },
  };
}

var returning = held();

returning.next().then(function (first) {
  trace.push("first:" + first.value);
  return returning.return(waited("R"));
}).then(function (ended) {
  trace.push("returned:" + ended.value + ":" + ended.done);
  return returning.next();
}).then(function (after) {
  trace.push("after:" + after.value + ":" + after.done);

  var thrown = held();
  return thrown.throw(new Error("early")).then(
    function () {
      trace.push("throw:resolved");
    },
    function (error) {
      trace.push("throw:rejected:" + error.message);
    });
}).then(function () {
  var running = held();
  return running.next().then(function () {
    return running.throw(new Error("inside"));
  }).then(
    function () {
      trace.push("inside:resolved");
    },
    function (error) {
      trace.push("inside:rejected:" + error.message);
    });
}).then(function () {
  print(trace.join(","));
});
