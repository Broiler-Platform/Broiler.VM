// A `finally` MAY SUSPEND, AND THE PENDING RETURN WAITS FOR IT. The first function returns from its
// `try` and then awaits inside the finaliser, so the promise settles only after the finaliser has
// resumed; the second overrides that return from the finaliser after awaiting, which is the case
// where a lowering that parked the completion in the wrong place answers `a`.
var trace = [];

async function deferred() {
  try {
    trace.push("try");
    return "from-try";
  } finally {
    trace.push("finally-in");
    await 0;
    trace.push("finally-out");
  }
}

async function overridden() {
  try {
    return "a";
  } finally {
    await 0;
    return "b";
  }
}

deferred()
  .then(function (v) { trace.push("settled:" + v); return overridden(); })
  .then(function (v) { print(trace.join(",") + " " + v); });
