// A BASE CONSTRUCTOR MAKES ITS OBJECT FROM `new.target` AND NOT FROM ITSELF. Nothing here
// mentions `Derived` except the `new`, and the object the BASE constructor made still has
// `Derived.prototype` - which is the only reason `instanceof` and the derived method work
// at all. A constructor that read its own `prototype` would answer `Base` three times.
class Base {
  constructor() { this.made = new.target.name; }
}

class Derived extends Base {
  tag() { return "derived"; }
}

var made = new Derived();

made.made + " " +
  (made instanceof Derived) + " " +
  (Object.getPrototypeOf(made) === Derived.prototype) + " " +
  made.tag();
