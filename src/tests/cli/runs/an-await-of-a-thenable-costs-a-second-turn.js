// A THENABLE IS ADOPTED THROUGH A JOB, so awaiting one costs a turn more than awaiting a promise
// and two more than awaiting nothing. The `then` here is itself an async function, which is the
// case a hand-written adoption gets wrong: it suspends, so the value does not arrive until its own
// continuation has run as well.
var trace = [];

var thenable = {
  then: async function (resolve) {
    trace.push("then-entered");
    await 0;
    trace.push("then-resumed");
    resolve("value");
  },
};

async function consume() {
  trace.push("got:" + (await thenable));
}

consume();

Promise.resolve()
  .then(function () { trace.push("1"); })
  .then(function () { trace.push("2"); })
  .then(function () { trace.push("3"); })
  .then(function () { trace.push("4"); })
  .then(function () { trace.push("5"); })
  .then(function () { print(trace.join(",")); });
