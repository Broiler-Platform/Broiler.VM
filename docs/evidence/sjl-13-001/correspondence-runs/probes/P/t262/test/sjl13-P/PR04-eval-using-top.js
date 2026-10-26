/*---
description: sjl13 P reader probe PR04 (row P25). A using declaration is not admitted at the top level of eval code.
esid: sec-performeval
features: [explicit-resource-management]
flags: [raw]
negative:
  phase: runtime
  type: SyntaxError
---*/
eval("using x = null;");
