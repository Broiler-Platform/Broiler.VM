// A CLASS BODY HAS ONE CONSTRUCTOR, and a static method or a computed key may still be called
// `constructor`: neither is the class's constructor, so neither is the second one the early error
// refuses.
class Counted {
  constructor(value) {
    this.value = value;
  }

  static constructor() {
    return 'static';
  }

  ['constructor']() {
    return 'computed';
  }
}

var made = new Counted(7);
[made.value, Counted.constructor(), Object.getOwnPropertyNames(Counted.prototype).length].join(' / ');
