// The shape of the whole family in eight lines: a generator declaration, a `yield` whose value is
// what `next` was passed, a `return` that ends the sequence, and a resumption after completion that
// runs no body. The completion value is what the fourth step reports, which is the one step a
// reader is most likely to get wrong.
function* counter() {
  var seen = yield 1;
  yield seen + 1;
  return "done";
}

var it = counter();
var first = it.next().value;
var second = it.next(10).value;
var third = it.next().value;
var afterwards = it.next();
first + "," + second + "," + third + "," + afterwards.value + "," + afterwards.done;
