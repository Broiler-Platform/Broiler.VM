// AN `if` CLAUSE IS A `Statement` AND A LEXICAL DECLARATION IS NOT ONE. The reason is the same one
// the `with` body already gives: a binding whose only enclosing scope is the clause itself would
// have nowhere to put its slot and nothing could ever read it.
//
// The refusal is `2101` and not `2104`, because the manifest admits `let` and the LANGUAGE is what
// has no production here. A plain function declaration in this position is the one thing the
// web-compatibility annex admits, and `runs/the-declarations-a-clause-still-admits.js` runs it.
if (true) let x = 1;
