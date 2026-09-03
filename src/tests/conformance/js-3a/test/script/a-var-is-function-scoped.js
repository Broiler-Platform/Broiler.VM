/*---
description: a var declared inside a block is one binding with the outer one
expected: completion 2
---*/
var a = 1;
{
  var a = 2;
}
a;
