// THE BODY OF A `with` IS A `Statement`, AND A LEXICAL DECLARATION IS NOT ONE. The language refuses
// this, and so does this front end - with `2101` rather than `2104`, because the manifest admits
// `with` and admits `let`, and what has nothing here is the grammar.
//
// It is refused rather than lowered for a reason beyond conformance: the only scope enclosing this
// declaration would be the object environment record, which holds an object and has no slot to put
// a binding in. A lowering that accepted it would have to invent one.
with ({ anything: 1 }) let held = anything;
