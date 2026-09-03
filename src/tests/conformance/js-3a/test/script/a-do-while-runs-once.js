/*---
description: a do-while body runs before its condition is read
expected: completion 1
---*/
let i = 0;
do {
  i = i + 1;
} while (false);
i;
