/*---
esid: sec-error-objects
description: A parse-phase failure declared as something other than a SyntaxError.
negative:
  phase: parse
  type: ReferenceError
---*/
1 + 2;
