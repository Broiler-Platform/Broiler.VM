// THE REFUSAL THE TWO BUNDLES LEFT BETWEEN THEM. The class family and the generator family were
// admitted by two different bundles, and the member that is both was written by neither: a `*`
// before a key in a CLASS BODY is still refused by name, where the same `*` in an object literal
// is now a generator method that runs. It needs a row of its own for the reason every other
// class-body construct does - until the class body was admitted, one refusal naming the class
// covered it, and admitting the body means each construct inside it answers for itself.
class C {
  *m() {
    yield 1;
  }
}
