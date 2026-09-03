/*---
description: the module goal is strict code, and a legacy octal literal is a syntax error in strict code
expected: refused-by-source LegacyOctalInStrictCode
flags: [module]
---*/
010;
