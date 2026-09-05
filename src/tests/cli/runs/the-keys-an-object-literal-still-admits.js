// EVERY KEY AN OBJECT LITERAL STILL TAKES, which is the direction the private-name refusal beside
// this one must not reach. The rule is stated in the one place an object literal's keys are read,
// so it covers five entry forms at once - and a rule that broad is exactly the one worth pinning
// from the accepting side.
//
// The class members at the end are the real home of a private name, and they never pass through
// that place at all: the member parser recognises its own private name before it asks for a key.
var out = [];

var literal = {
  plain: 1,
  "a string": 2,
  3: 4,
  ["comp" + "uted"]: 5,
  method() { return 6; },
  get accessor() { return 7; },
  set accessor(value) { out.push("set:" + value); },
  *generator() { return 8; },
  async asynchronous() { return 9; },
  async *both() { return 10; },
  shorthandKey,
  default: 11,
};

literal.accessor = 12;

out.push(
  literal.plain,
  literal["a string"],
  literal[3],
  literal.computed,
  literal.method(),
  literal.accessor,
  literal.generator().next().value,
  typeof literal.asynchronous,
  typeof literal.both,
  typeof literal.shorthandKey,
  literal.default);

// A GENERATOR METHOD IS A METHOD, which this component's parser alone did not record - so its body
// had no home object and `super` in it was refused while the same word in the four method forms
// beside it was not.
var base = { inherited: "from-the-prototype" };
var derived = {
  *viaGenerator() { return super.inherited; },
  viaMethod() { return super.inherited; },
  async *viaAsyncGenerator() { return super.inherited; },
};

Object.setPrototypeOf(derived, base);
out.push(derived.viaGenerator().next().value, derived.viaMethod());

// And the class members a private name belongs to.
class Holder {
  #field = "private-field";
  #method() { return "private-method"; }
  static #shared = "private-static";

  read() { return this.#field + "/" + this.#method() + "/" + Holder.#shared; }
}

out.push(new Holder().read());

function shorthandKey() { return "shorthand"; }

out.join(" ");
