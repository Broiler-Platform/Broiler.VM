// `yield*` INSIDE AN ASYNC GENERATOR, WHICH IS NOT THE SYNCHRONOUS DELEGATION WITH A WAIT ADDED.
// Every inner step is awaited BEFORE it is asked whether it is done, so the loop has to survive a
// suspension in the middle of itself and re-enter at the same instruction - five times over, once
// for each thing it can be waiting on. What the source below shows is the two ends of that: an
// inner async generator, and an inner SYNCHRONOUS one, delegated to from the same operator.
//
// THE VALUE A DELEGATION EVALUATES TO IS THE INNER ITERATOR'S RETURN VALUE, which every hand-written
// forwarding loop forgets, and it is the half that is the same in both kinds.
async function* inner() {
  yield "i1";
  yield "i2";
  return "inner-done";
}

function* synchronous() {
  yield "s1";
  yield "s2";
  return "sync-done";
}

async function* outer() {
  var first = yield* inner();
  yield "after:" + first;
  var second = yield* synchronous();
  yield "after:" + second;
  yield* [Promise.resolve("a1"), "a2"];
}

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(outer()).then(function (values) {
  print(values);
});
