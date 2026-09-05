// A named class expression can see its own name from inside its body and nowhere else,
// and an anonymous one assigned to a binding takes that binding's name. The completion
// value says both, plus that the inner name survives the outer one being reassigned.
var Held = class Inner {
  self() { return Inner; }
  label() { return Inner.name; }
};

var Kept = Held;
Held = null;

var Anonymous = class {};

(new Kept().self() === Kept) + " " +
  new Kept().label() + " " +
  Anonymous.name + " " +
  typeof Inner;
