/*---
description: a counting for loop runs its body once per step
expected: completion 55
---*/
let sum = 0;
for (let i = 1; i <= 10; i = i + 1) {
  sum = sum + i;
}
sum;
