/*---
esid: sec-let-and-const-declarations
description: One scope declaring a lexical name twice, read under the module goal.
negative:
  phase: parse
  type: SyntaxError
flags: [module]
---*/
let a;
let a;
