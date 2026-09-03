/*---
description: a shift converts to a signed 32-bit integer first, so one shifted 31 places is negative
expected: completion -2147483648
---*/
1 << 31;
