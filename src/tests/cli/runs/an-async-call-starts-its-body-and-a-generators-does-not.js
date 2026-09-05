// THE ONE PROPERTY THAT MAKES `async` MORE THAN SUGAR OVER A GENERATOR, in one file. A generator's
// call runs NO instruction of its body; an async function's call runs the body straight through to
// its first `await` and only then returns. Both frames live on the heap and both suspend the same
// way, so this is the only place a program can see which driver it got.
var trace = [];

function* generated() {
  trace.push("generator-body");
}

async function awaited() {
  trace.push("async-body");
  await 0;
  trace.push("async-resumed");
}

trace.push("before");
generated();
awaited();
trace.push("after");
trace.join(",");
