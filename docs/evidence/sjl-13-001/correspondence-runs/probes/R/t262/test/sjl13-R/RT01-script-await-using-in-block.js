/*---
description: sjl13 R reader probe RT01 (row R05). In a Script, await is an identifier outside async functions, so `await using` in a block is no declaration and the program is a SyntaxError.
esid: sec-scripts
flags: [raw]
negative:
  phase: parse
  type: SyntaxError
---*/
{ await using r = null; }
(function () {})("RT01 accepted");
