// A SHORTHAND PROPERTY'S KEY IS ALSO THE NAME IT READS, so it is an `IdentifierReference` and not
// the `IdentifierName` a written-out key may be. `({ default: 42 })` is an object with a property
// called `default`; `({ default })` is a reference to a binding no source can declare.
//
// The refusal is `2209`, the code every other reserved word in a name position carries, and it is
// stated where the literal is parsed rather than where a pattern is recognised - because the same
// braces cover both and the rule is the same on either reading.
0, ({ default } = {});
