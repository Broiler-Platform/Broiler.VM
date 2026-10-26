/*---
description: sjl13 D reader probe RD01 (row D30). continue names an outer label of a loop label set; the loop continues and the program completes.
esid: sec-static-semantics-containsundefinedcontinuetarget
flags: [raw]
---*/
var n = 0;
L1: L2: for (var i = 0; i < 3; i++) { n++; continue L1; }
if (n !== 3) { throw new Error("RD01 " + n); }
