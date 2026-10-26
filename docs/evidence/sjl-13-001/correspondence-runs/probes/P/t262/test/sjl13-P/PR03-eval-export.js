/*---
description: sjl13 P reader probe PR03 (row P03). Eval code is a Script, so an export declaration is a SyntaxError.
esid: sec-performeval
flags: [raw]
negative:
  phase: runtime
  type: SyntaxError
---*/
eval("export {};");
