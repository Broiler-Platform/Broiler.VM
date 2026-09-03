/*---
esid: sec-literals-numeric-literals
description: >
  A legacy octal literal, which is an early error in strict code and an ordinary number in sloppy
  code. The file declares neither strictness, so it is read both ways and the two readings answer
  differently.
negative:
  phase: parse
  type: SyntaxError
---*/
var n = 010;
