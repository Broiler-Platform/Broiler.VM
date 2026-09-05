// Three levels of `extends`, with a `super` call in each derived constructor and a
// `super.who()` that reaches PAST a class which does not define `who` at all. The
// completion value carries both: the trail every constructor appended to, and the name
// the grandparent answered with.
class Base {
  constructor() { this.trail = "base"; }
  who() { return "Base"; }
}

class Middle extends Base {
  constructor() { super(); this.trail += "-middle"; }
}

class Leaf extends Middle {
  constructor() { super(); this.trail += "-leaf"; }
  who() { return "Leaf>" + super.who(); }
}

var leaf = new Leaf();
leaf.trail + " " + leaf.who() + " " + (Object.getPrototypeOf(Leaf) === Middle);
