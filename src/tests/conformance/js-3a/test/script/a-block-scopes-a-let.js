/*---
description: a let in a block does not reach the binding outside it
expected: completion 1
---*/
let a = 1;
{
  let a = 2;
}
a;
