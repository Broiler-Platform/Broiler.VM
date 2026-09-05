// A derived constructor that returns without calling `super()` has no `this` to return,
// and the language says so with a ReferenceError rather than with an object nobody
// initialised. It is a RUN-TIME error and not a refusal: the source is admitted, and only
// the path taken decides.
class Base {
  constructor() { this.built = true; }
}

class Forgot extends Base {
  constructor() { this.tooEarly = 1; }
}

new Forgot();
