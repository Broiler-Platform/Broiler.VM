/*---
esid: sec-delete-operator
description: >
  Deleting an unqualified name is a syntax error in strict code. This profile has no delete at
  all, so it refuses the source for a reason the test did not ask about.
negative:
  phase: parse
  type: SyntaxError
flags: [onlyStrict]
---*/
delete x;
