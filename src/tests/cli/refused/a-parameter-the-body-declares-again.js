// A PARAMETER AND THE BODY'S OWN TOP LEVEL ARE TWO ENVIRONMENTS AND THE INNER ONE MAY NOT SHADOW
// THE OUTER AT ITS OWN TOP LEVEL. One block deeper is a third environment and shadowing is what it
// is for, so `function f(a) { { let a; } }` is a program and this is not.
function f(a) {
  let a = 1;
  return a;
}
