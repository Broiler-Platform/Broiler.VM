/*---
description: a falsy left operand means the right side is never evaluated
expected: completion 0
---*/
let a = 0;
let b = 0;
a && (b = 1);
b;
