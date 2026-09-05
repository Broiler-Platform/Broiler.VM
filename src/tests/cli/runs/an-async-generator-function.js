// THE CONSTRUCT THIS ROW USED TO REFUSE, AND THE ONE PROPERTY THAT MAKES IT NEITHER OF THE TWO
// FAMILIES IT IS SPELLED FROM. A generator's call runs no instruction of its body and answers an
// object the caller pulls; an async function's call runs its body to the first `await` and answers
// a promise. An async generator does BOTH HALVES OF NEITHER: the call runs nothing and answers an
// object, and each pull of that object answers a promise. The trace below is where a program sees
// it - `made` is pushed before any of `body` is, and `1` arrives only after the queue has turned.
var trace = [];

async function* pages() {
  trace.push("body");
  yield 1;
  yield 2;
}

var it = pages();
trace.push("made");

it.next().then(function (step) {
  trace.push("first:" + step.value + ":" + step.done);
  return it.next();
}).then(function (step) {
  trace.push("second:" + step.value + ":" + step.done);
  return it.next();
}).then(function (step) {
  trace.push("third:" + step.value + ":" + step.done);
  print(trace.join(","));
});
