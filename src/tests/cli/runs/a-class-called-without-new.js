// A class is not callable. The refusal happens where the call is made rather than inside
// the constructor, so it holds for a call through any route - and it is a TypeError at run
// time, because whether a value is called or constructed is not something the source of
// the class can decide.
class Cell {
  constructor(v) { this.v = v; }
}

Cell(1);
