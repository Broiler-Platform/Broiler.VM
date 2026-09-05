// A PARAMETER LIST MAY NOT SUSPEND. A default is evaluated in the enclosing function's own
// prologue, so a `yield` written in a generator's parameter list - or an `await` in an async
// function's - is a suspension in a unit that is not yet suspendable, and the language states it as
// an early error rather than leaving it to mean something.
//
// It is the rule this component had the strongest reason to want: without it the lowering emitted a
// `Yield` into a unit carrying no generator flag and THIS COMPONENT'S OWN VERIFIER refused bytes
// this component had just produced, answering `SemanticValidationFailed` where the language has a
// syntax error. `runs/a-suspension-a-parameter-list-still-admits.js` holds the other direction: a
// method's list is `[~Yield, ~Await]` however deeply a generator encloses it, so the same two words
// there are ordinary parameter names.
function* g(x = yield) {
  return x;
}
