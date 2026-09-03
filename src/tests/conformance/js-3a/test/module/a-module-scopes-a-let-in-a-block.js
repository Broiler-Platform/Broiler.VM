/*---
description: block scoping is the same under the module goal
expected: completion 1
flags: [module]
---*/
let a = 1;
{
  let a = 2;
}
a;
