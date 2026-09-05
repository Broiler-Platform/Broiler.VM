// A FUNCTION DECLARATION WRITTEN IN A BLOCK IS A BINDING OF THAT BLOCK, and Annex B additionally
// gives sloppy code a `var`-scoped alias assigned where the declaration stands. Both halves are
// needed at once: the first alone makes `if (x) { function f() { } } f();` a `ReferenceError` that
// every engine runs, and the second alone leaves the closure resolving the block's own `let` to a
// global.
//
// This file is the shape the whole rule turns on, and every value in it was taken from the
// comparison engine rather than reasoned out.
var out = [];

// The block's own bindings are on the chain the closure captures.
{
  let counter = 0;
  function increment() { counter += 1; }
  increment();
  increment();
  out.push(counter);
}

// Two sibling blocks declare two different functions of one name, and each block sees its own.
{ function which() { return "first"; } out.push(which()); }
{ function which() { return "second"; } out.push(which()); }

// A switch's clauses are one block, and a `try`'s halves are blocks of their own.
switch (1) {
  case 1: {
    let inSwitch = "switch";
    function readSwitch() { return inSwitch; }
    out.push(readSwitch());
  }
}

try {
  let inTry = "try";
  function readTry() { return inTry; }
  out.push(readTry());
} catch (e) {
  out.push(e.name);
}

// The Annex B alias holds `undefined` until the declaration is REACHED, and nothing at all when
// the block does not run.
out.push(typeof aliased);
{ function aliased() { return "aliased"; } }
out.push(aliased());

if (false) { function neverReached() { return 1; } }
out.push(typeof neverReached);

// An `if` clause that is a bare declaration is the block the annex says it is.
if (true) function inClause() { return "clause"; }
out.push(inClause());

// And the alias is declined where a `var` of the name could not have been added: a lexical
// declaration of the same name in an enclosing block is what makes it an early error.
{
  let declined = 1;
  { function declined() { return 2; } }
}

out.push(typeof declined);

// A generator, an async function and an async generator are separate productions the annex does
// not name, so none of the three gets an alias at all.
{ function* aGenerator() { yield 1; } }
{ async function anAsyncFunction() { return 1; } }
{ async function* anAsyncGenerator() { yield 1; } }
out.push(typeof aGenerator, typeof anAsyncFunction, typeof anAsyncGenerator);

out.join(" ");
