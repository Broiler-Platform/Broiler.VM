// EVERY CLAUSE OF ONE `switch` SHARES ONE BLOCK SCOPE, which is the whole reason this file exists
// beside the block one. A reader looking at the two clauses sees two scopes and the language sees
// one: the `CaseBlock` is the scope and a clause is not, so the second `let` is a second
// declaration of a name that is already bound and the source is refused.
switch (0) {
  case 1:
    let shared = 1;
    break;

  case 2:
    let shared = 2;
    break;
}
