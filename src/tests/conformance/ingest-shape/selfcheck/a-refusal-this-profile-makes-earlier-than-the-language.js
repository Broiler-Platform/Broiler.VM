/*---
esid: sec-identifiers
description: >
  A free name is a run-time ReferenceError in the language. This profile has no global object, so
  it answers at compile time, which is a recorded divergence and not a syntax error.
negative:
  phase: parse
  type: SyntaxError
flags: [noStrict]
---*/
undeclared;
