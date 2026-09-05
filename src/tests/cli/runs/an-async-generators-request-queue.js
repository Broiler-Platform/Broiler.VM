// THE QUEUE, WHICH IS THE PART AN ASYNC GENERATOR DOES NOT INHERIT FROM EITHER FAMILY IT IS
// SPELLED FROM. Four calls of `next` are made before any of them has settled, and all four are
// entitled to an answer in the order they were made - from a body only one of them may be inside.
// A synchronous generator never faces this: its `next` finishes before it returns.
//
// THE SENT VALUE IS THE SECOND CALL'S AND NOT THE FIRST'S, and that is the queue being visible
// rather than an accident. The first `next` starts the body, which runs to the first `yield`; what
// that `yield` evaluates to is what the NEXT resumption sends, and the next resumption is the
// second call. `first` is therefore recorded and `a` is never sent to anything.
var trace = [];

async function* counted() {
  var sent = yield 1;
  trace.push("sent:" + sent);
  var waited = await Promise.resolve("w");
  trace.push("awaited:" + waited);
  yield 2;
  return "done";
}

var it = counted();
var first = it.next("a");
var second = it.next("b");
var third = it.next("c");
var fourth = it.next("d");
trace.push("all-four-called");

Promise.all([first, second, third, fourth]).then(function (steps) {
  for (var index = 0; index < steps.length; index++) {
    trace.push(steps[index].value + ":" + steps[index].done);
  }

  print(trace.join(","));
});
