// STRICT CODE HAS NO `with` STATEMENT, AND THIS REFUSAL IS THE LANGUAGE'S RATHER THAN THE
// MANIFEST'S. The wide manifest admits `with` - the fixtures beside this one run it - so answering
// `2104:ConstructOutsideManifest` here would say this profile declines a construct it implements,
// and a conformance runner reading the code would take every strict-mode case out of both the pass
// and the fail column instead of scoring the `SyntaxError` they expect. `2101:UnexpectedToken` is
// what a program wrong about the LANGUAGE gets, exactly as a `super` property outside a method does.
//
// The directive is inside the function rather than at the top of the file, so this fixture also
// asserts that the refusal follows the strictness a body imposes on itself.
function strictHere() {
  "use strict";

  with ({ anything: 1 }) {
    return anything;
  }
}
