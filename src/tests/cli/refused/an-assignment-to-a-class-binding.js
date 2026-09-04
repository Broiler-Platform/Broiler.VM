// The binding a named class expression gives its own body is CONSTANT, and this profile
// answers an assignment to a constant binding where it answers every other one: at compile
// time, under 2204. The language makes it a run-time TypeError instead, which is the
// divergence diagnostic 2204 already carries for `const`; the class binding inherits it
// rather than acquiring a second answer of its own.
var Held = class Inner {
  rename() {
    Inner = 1;
  }
};
