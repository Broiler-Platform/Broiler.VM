// A UNICODE ESCAPE SPELLS A NAME AND NEVER REACHES A RESERVED WORD THROUGH ONE. The tokenizer
// resolves the escape, so by the time the parser sees this binding it is the four characters
// `enum` - and the language refuses the escaped spelling exactly where the plain one is reserved,
// so that a program cannot smuggle a keyword into an identifier position.
//
// An IdentifierName may still be spelled with escapes freely: a property key and a member name
// both admit one, and `runs/the-names-an-escape-may-still-spell.js` beside this file runs a source
// that uses both.
var \u0065num = 1;
