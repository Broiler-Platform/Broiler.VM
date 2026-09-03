/*---
description: the else arm of a taken if does not run
expected: completion 2
---*/
let a = 0;
if (true) {
  a = 2;
} else {
  a = 3;
}
a;
