// AWAITING A NUMBER IS NOT FREE, and the interleaving below is the whole reason. `await 0` resolves
// its operand the way `Promise.resolve` does, which makes an already-fulfilled promise and puts the
// continuation on the queue - so one `await` costs exactly one turn and lands between the first and
// second links of a `then` chain registered after it. An implementation that noticed `0` was not a
// thenable and carried straight on would print the two async lines first, and every invariant a
// caller established on the line below would sometimes hold too late.
var trace = [];

async function twice() {
  await 0;
  trace.push("a1");
  await 0;
  trace.push("a2");
}

twice();

Promise.resolve()
  .then(function () { trace.push("t1"); })
  .then(function () { trace.push("t2"); })
  .then(function () { print(trace.join(",")); });
