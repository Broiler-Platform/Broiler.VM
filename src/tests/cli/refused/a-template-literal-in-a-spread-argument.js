// The argument of a spread is a new expression position. A template literal stays refused by
// name inside it, and the refusal lands on the template rather than on the `...`.
takes(...`t`);
