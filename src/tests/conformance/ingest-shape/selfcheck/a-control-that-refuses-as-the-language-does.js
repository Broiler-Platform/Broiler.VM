/*---
esid: sec-let-and-const-declarations
description: >
  A const declaration with no initialiser, which every conforming engine refuses.
negative:
  phase: parse
  type: SyntaxError
flags: [onlyStrict]
---*/
const x;
