// A PARAMETER DEFAULT IS A NEW EXPRESSION POSITION, AND A CLASS IS NOW ADMITTED IN IT. This row
// spent the bundle that added parameter defaults asserting the opposite - that a class in a
// default came back refused BY NAME - because the class family was still outside the manifest
// then. It is inside it now, so what the position has to be checked for is the composition
// working rather than the refusal landing: the default is evaluated only when the argument is
// absent, and what it evaluates to is a constructor with a prototype of its own.
function decorated(Kind = class { tag() { return "default"; } }) {
  return new Kind().tag();
}

class Named {
  tag() {
    return "named";
  }
}

decorated() + "/" + decorated(Named);
