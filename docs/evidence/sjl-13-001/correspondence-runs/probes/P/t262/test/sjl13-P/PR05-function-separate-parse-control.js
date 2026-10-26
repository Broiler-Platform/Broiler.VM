/*---
description: sjl13 P reader probe PR05 (row P26), the control. Separate parameter and body text that make one function.
esid: sec-createdynamicfunction
flags: [raw]
---*/
var f = new Function("a, b", "return a + b;");
if (f(1, 2) !== 3) { throw new Error("PR05 " + f(1, 2)); }
