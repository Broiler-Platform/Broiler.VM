// PRIVATE ELEMENTS ARE NOT PROPERTIES, and this is the whole list of surfaces that would say
// otherwise if they were stored in the property table. Every one of them is asked here rather than
// one standing for the rest, because the storage decision is what makes them all true at once and a
// regression that moved private state into the property table would show up in whichever of these
// somebody happened not to ask.
class Holder {
  #secret = 41;
  #method() { return this.#secret + 1; }
  visible = 1;

  read() { return this.#method(); }
}

var held = new Holder();

var seen = [];

for (var name in held) {
  seen.push(name);
}

held.read() + " / " +
  Object.keys(held).join(",") + " " +
  Object.getOwnPropertyNames(held).join(",") + " " +
  Object.getOwnPropertySymbols(held).length + " " +
  Reflect.ownKeys(held).join(",") + " " +
  JSON.stringify(held) + " " +
  seen.join(",") + " " +
  Object.keys(Object.assign({}, held)).join(",") + " " +
  Object.keys({ ...held }).join(",");
