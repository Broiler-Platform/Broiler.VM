/*---
esid: sec-additive-operators
description: >
  A parse failure that is declared and does not happen, which must be a failure.
negative:
  phase: parse
  type: SyntaxError
flags: [onlyStrict]
---*/
1 + 2;
