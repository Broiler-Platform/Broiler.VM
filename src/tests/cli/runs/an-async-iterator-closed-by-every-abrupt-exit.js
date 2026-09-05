// THE FOUR WAYS OUT OF A `for await` THAT OWE THE ITERATOR ITS `return`, and the one that does
// not. `break`, a labelled `break`, `return` and `throw` each close it; running out does not,
// because the iterator said it was finished. `continue` must NOT close it, which is the case a
// lowering that emitted the close at the loop's own bottom would get wrong.
//
// AND THE CLOSE IS AWAITED, which is what makes it `AsyncIteratorClose` rather than the synchronous
// one. The `return` below answers a promise, and the loop does not carry on until it has settled -
// so `closed` is in the trace before whatever the exit was doing next.
var trace = [];

function source(name) {
  var at = 0;

  return {
    [Symbol.asyncIterator]: function () {
      return {
        next: function () {
          at = at + 1;
          return Promise.resolve({ value: at, done: at > 9 });
        },
        return: function () {
          trace.push(name + ":closed");
          return Promise.resolve({ done: true });
        },
      };
    },
  };
}

async function withBreak() {
  for await (const value of source("break")) {
    if (value === 2) {
      break;
    }
  }

  trace.push("break:after");
}

async function withLabelledBreak() {
  outer: for await (const value of source("labelled")) {
    if (value === 2) {
      break outer;
    }
  }

  trace.push("labelled:after");
}

async function withReturn() {
  for await (const value of source("return")) {
    if (value === 2) {
      return "returned";
    }
  }

  return "ran-out";
}

async function withThrow() {
  try {
    for await (const value of source("throw")) {
      if (value === 2) {
        throw new Error("boom");
      }
    }
  } catch (error) {
    trace.push("throw:caught:" + error.message);
  }
}

async function withContinue() {
  var seen = 0;

  for await (const value of source("continue")) {
    if (value < 3) {
      continue;
    }

    seen = value;
    break;
  }

  trace.push("continue:seen:" + seen);
}

withBreak()
  .then(withLabelledBreak)
  .then(withReturn)
  .then(function (answer) {
    trace.push("return:" + answer);
  })
  .then(withThrow)
  .then(withContinue)
  .then(function () {
    print(trace.join(","));
  });
