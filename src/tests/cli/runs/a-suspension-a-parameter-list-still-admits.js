// A METHOD'S PARAMETERS ARE `[~Yield, ~Await]` HOWEVER DEEPLY A GENERATOR OR AN ASYNC FUNCTION
// ENCLOSES THEM, so the two words the rule beside this one refuses as expressions are ordinary
// names here. `MethodDefinition : PropertyName ( UniqueFormalParameters )` passes neither parameter
// down, and an ordinary function nested in a generator is not a generator either.
//
// This is the direction the refusal must not reach. It is also the shape that found the defect: a
// `yield` in a method's default inside a generator was parsed as the operator, and the instruction
// went into a unit with no generator flag - so this component's own verifier refused bytes this
// component had just produced.
var out = [];

function* outer() {
  var literal = { m(yield) { return yield; }, n(x = "d") { return x; } };
  out.push(literal.m("name") + ":" + literal.n());
  yield 1;
}

out.push([...outer()].join(""));

// A generator's and an async function's own parameters are fine as long as they do not suspend.
function* counted(start = 20, step = 2) {
  yield start;
  yield start + step;
}

out.push([...counted()].join("/"));

// An ordinary arrow inside an async body takes ordinary parameters; only an `await` there is
// refused, and only because the arrow is not the async function.
async function arrows(z = 23) {
  var plain = (x = 21) => x;
  return plain() + ":" + z;
}

out.push(typeof arrows(24).then);

// And the same words as parameter names of an ordinary nested function.
function* holder() {
  function inner(yield_, await_) { return yield_ + await_; }
  yield inner(1, 2);
}

out.push([...holder()].join(""));

out.join(" ");
