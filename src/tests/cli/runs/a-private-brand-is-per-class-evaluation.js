// A PRIVATE NAME IS MINTED PER CLASS EVALUATION AND NOT PER CLASS BODY, which is the claim a
// constant-pool entry would have got wrong and the reason the name is an instruction. The factory
// below evaluates one class expression twice; the two `#x` that result are unrelated names, so each
// brand check answers for its own instances and refuses the other's. `#x in o` is the question and
// `o.#x` is the demand, and this pins that the first answers where the second throws - except for a
// non-object, where the brand check throws too, because the form is the `in` operator with a name
// the grammar spells differently and `"x" in 5` throws as well.
function makeHolder() {
  return class {
    #x = 1;

    static holds(candidate) { return #x in candidate; }
    static read(candidate) { return candidate.#x; }
  };
}

function refusal(attempt) {
  try {
    attempt();
    return "none";
  } catch (error) {
    return error.constructor.name;
  }
}

var First = makeHolder();
var Second = makeHolder();
var one = new First();
var two = new Second();

First.holds(one) + " " + First.holds(two) + " " + First.holds({}) + " / " +
  Second.holds(two) + " " + Second.holds(one) + " / " +
  First.read(one) + " " + Second.read(two) + " / " +
  refusal(function () { return First.read(two); }) + " " +
  refusal(function () { return First.read({}); }) + " " +
  refusal(function () { return First.holds(5); });
