/*---
description: continue skips the rest of the body and still runs the update
expected: completion 8
---*/
let sum = 0;
for (let i = 0; i < 5; i = i + 1) {
  if (i === 2) {
    continue;
  }
  sum = sum + i;
}
sum;
