// A CLASS BODY DECLARES ONE CONSTRUCTOR. Until 2026-09-30 the second was admitted and one of the two
// silently lost.
class Twice {
  constructor() {
    this.first = true;
  }

  'constructor'() {
    this.second = true;
  }
}
