// A PARAMETER DEFAULT IS A NEW EXPRESSION POSITION, and admitting one is a chance to lose the
// refusal a still-excluded family relies on. A class expression here has to come back named,
// under 2104, and not as an unexpected token - the conformance runner grades the manifest
// boundary on the diagnostic code, and a negative test expecting a syntax error at parse would
// otherwise be scored a PASS for the wrong reason.
function decorated(a = class {}) {
  return a;
}
