/*---
esid: sec-break-statement
description: A break with no enclosing breakable statement, which is an early error either way.
negative:
  phase: parse
  type: SyntaxError
---*/
break;
