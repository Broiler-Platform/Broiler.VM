/*---
description: two lexical declarations of one name in one scope
expected: refused-by-source DuplicateLexicalDeclaration
---*/
let a = 1;
let a = 2;
