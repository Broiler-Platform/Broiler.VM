// AND THE POSITION ONE TOKEN AWAY FROM AN ADMITTED ONE. An object literal's `async m()` and its
// `*m()` are both admitted; `async *m()` is neither of them, and the arm that decides has to test
// the `*` before it parses anything on the `async`. Reading `async` alone would have taken every
// ordinary async method with it, and reading the key first makes the `*` a surprise token.
var pump = {
  async *from(start) {
    yield start;
  },
};
