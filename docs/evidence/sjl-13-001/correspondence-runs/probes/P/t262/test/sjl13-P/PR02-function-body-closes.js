/*---
description: sjl13 P reader probe PR02 (row P26). The body text is parsed alone as FunctionBody, so it cannot close the function and open another.
esid: sec-createdynamicfunction
flags: [raw]
negative:
  phase: runtime
  type: SyntaxError
---*/
new Function("", "}); (function(){");
