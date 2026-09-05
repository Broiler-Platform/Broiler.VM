// A `yield*` CHAIN DEEPER THAN THE CALL-DEPTH ALLOWANCE. It ends by naming the dimension it spent,
// in a bounded number of resumptions rather than in a number of seconds, so this file decides the
// same way on a busy machine - and it must NEVER end by terminating the process, which is what a
// suspension that did not count its own frame did before the resumption was charged for one.
//
// Each level costs two: one for the `next` call that reaches the inner generator, and one for the
// resumption that re-enters its body. Both are live at once all the way down the chain, which is
// why counting only the call would say the chain is half as deep as it is.
function* chain(n) {
  if (n === 0) {
    yield "bottom";
    return;
  }

  yield* chain(n - 1);
}

chain(100000).next();
