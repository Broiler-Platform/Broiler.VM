// THE SECOND SCRIPT DECLARES `taken` AGAIN, which GlobalDeclarationInstantiation refuses with a
// SyntaxError before the script's first statement runs: nothing is printed and `before` is never
// created. A host that replaced the binding instead would print `ran`.
var before = 1;
print("ran");
let taken = 2;
