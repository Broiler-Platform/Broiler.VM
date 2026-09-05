// THE DECLARATIONS A CLAUSE AND A LABEL STILL ADMIT, which is the other direction of the rule that
// refuses `if (x) let y;`. The web-compatibility annex admits a PLAIN function declaration in an
// `if` clause, in an `else` clause and under a label, in sloppy code - and admits it nowhere else
// and in no other form, which is what makes these three rows worth pinning separately from the
// refusals beside them.
var out = [];

if (true) function inConsequent() { return "consequent"; }
if (false) ; else function inAlternate() { return "alternate"; }
labelled: function underLabel() { return "label"; }

out.push(inConsequent(), inAlternate(), underLabel());

// AND THE `let` THAT IS AN IDENTIFIER RATHER THAN A DECLARATION. An `ExpressionStatement` may not
// begin with `let [`, and may begin with `let` followed by anything else - so a `let` at the end of
// a line is a reference, the semicolon is inserted, and what follows is a statement of its own.
// Refusing this shape as a declaration is the mistake this row exists to catch.
var let = "an ordinary name";
if (false) let
out.push(let);

out.join("/");
