// `this` in a derived constructor is in a temporal dead zone until `super()` returns, and
// reading it early is a ReferenceError rather than an object with nothing in it. This is
// the property that makes a derived class more than sugar over a function.
class Base {
  constructor() { this.built = true; }
}

class TooEager extends Base {
  constructor() {
    this.field = 1;
    super();
  }
}

new TooEager();
