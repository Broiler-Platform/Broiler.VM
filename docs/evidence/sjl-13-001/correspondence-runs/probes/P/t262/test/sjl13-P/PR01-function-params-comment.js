/*---
description: sjl13 P reader probe PR01 (row P26). The parameter text is parsed alone as FormalParameters, so a comment opened there cannot close in the body.
esid: sec-createdynamicfunction
flags: [raw]
negative:
  phase: runtime
  type: SyntaxError
---*/
new Function("/*", "*/){");
