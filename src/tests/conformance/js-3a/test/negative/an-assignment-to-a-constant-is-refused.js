/*---
description: a const binding is not an assignment target
expected: refused-by-source AssignmentToConstant
---*/
const a = 1;
a = 2;
