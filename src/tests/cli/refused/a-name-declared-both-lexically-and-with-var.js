// A LEXICAL NAME AND A `var` OF THE SAME SCOPE ARE A COLLISION AND TWO `var`S ARE NOT, which is
// why this carries `2202` rather than the duplicate code. A `var` binding is created once and a
// second declaration finds it; a lexical binding is created BY its declaration, so the two rules
// are about different things.
//
// The `var` is one block deeper than the `let` deliberately. `VarDeclaredNames` descends through
// every statement of the scope and stops only at a function, so the nesting does not save it -
// which is the half of the rule a walk that stopped at the brace would have missed.
{
  {
    var g;
  }

  let g;
}
