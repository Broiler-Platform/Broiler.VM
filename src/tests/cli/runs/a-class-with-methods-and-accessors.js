// A class body of every member form this manifest admits: a constructor, a prototype
// method, a static method, an accessor pair and a computed name. The completion value is
// the one number that only comes out right if all five landed where they belong.
var key = "scaled";

class Cell {
  constructor(v) { this.v = v; }
  double() { return this.v * 2; }
  get half() { return this.v / 2; }
  set half(n) { this.v = n * 2; }
  [key]() { return this.v * 10; }
  static of(v) { return new Cell(v); }
}

var cell = Cell.of(3);
cell.half = 10;
cell.double() + cell.half + cell.scaled();
