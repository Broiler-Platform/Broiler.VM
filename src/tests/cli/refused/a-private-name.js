// A private name is refused where it is WRITTEN, and a property access is the position
// that would otherwise slip through: `#secret` scans as an identifier, so without a
// refusal here the program would run and read a property nobody declared.
class Holder {
  read() { return this.#secret; }
}
