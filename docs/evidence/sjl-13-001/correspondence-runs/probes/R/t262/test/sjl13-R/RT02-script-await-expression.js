/*---
description: sjl13 R reader probe RT02 (row R05). An await expression at a Script's top level is a SyntaxError.
esid: sec-scripts
flags: [raw]
negative:
  phase: parse
  type: SyntaxError
---*/
var v = await Promise.resolve(7);
(function () {})("RT02 accepted " + v);
