/*---
esid: sec-moduledeclarationlinking
description: A module whose import resolves to nothing.
negative:
  phase: resolution
  type: SyntaxError
flags: [module]
---*/
1 + 2;
