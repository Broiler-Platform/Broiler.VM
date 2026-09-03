/*---
description: a loop whose condition never becomes false, which must be reported as a timeout rather than as either verdict
expected: completion 0
---*/
let i = 0;
while (i < 1) { }
i;
