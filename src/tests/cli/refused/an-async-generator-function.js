// ADMITTING `async` OPENED THE POSITION AN ASYNC GENERATOR STANDS IN, and it is still refused BY
// NAME rather than parsed. The two constructs differ by one token - the `*` after `function` - so
// the branch that recognises this one has to stay ahead of the branch that now parses an async
// function, and a regression makes it a surprise token rather than a refusal the conformance runner
// scores as unsupported.
async function* pages() {
  yield 1;
}
