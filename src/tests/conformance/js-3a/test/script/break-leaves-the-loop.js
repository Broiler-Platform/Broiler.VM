/*---
description: break leaves the loop rather than the iteration
expected: completion 3
---*/
let i = 0;
while (i < 10) {
  if (i === 3) {
    break;
  }
  i = i + 1;
}
i;
