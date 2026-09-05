// A STATIC BLOCK AND A STATIC FIELD ARE ONE ORDERED LIST, which is the whole reason the block is
// not sugar for a statement after the class. The three static elements below run in the order the
// body wrote them, with `this` bound to the constructor, and they run AFTER every key in the body
// has been evaluated and after the class binding exists - so the block may name `Registered` and
// the computed key beside it may not.
var log = [];

function key(name) {
  log.push("key:" + name);
  return name;
}

class Registered {
  static [key("first")] = log.push("field:first");

  static {
    log.push("block:this-is-the-class:" + (this === Registered));
    this.fromBlock = this.first + 1;
  }

  static [key("second")] = log.push("field:second");

  static {
    log.push("block:sees:" + Registered.fromBlock);
  }
}

log.join(" ") + " / " +
  Registered.first + " " + Registered.fromBlock + " " + Registered.second + " / " +
  Object.getOwnPropertyNames(Registered).join(",");
