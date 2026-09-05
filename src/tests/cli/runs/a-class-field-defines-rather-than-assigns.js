// A FIELD IS `CreateDataPropertyOrThrow` AND NOT AN ASSIGNMENT, and an inherited setter is what
// tells the two apart. `this.x = 1` in a constructor calls a setter the prototype chain provides;
// `x = 1` as a field defines an own property and shadows it, so the setter never runs. The two
// halves of this file are the same class written both ways, and a lowering that emitted a store
// for the field would make them agree - which they must not.
var calls = [];

class Base {
  set x(value) {
    calls.push("setter:" + value);
    this.recorded = value;
  }

  get x() {
    return "through the getter";
  }
}

class ByField extends Base {
  x = 1;
}

class ByAssignment extends Base {
  constructor() {
    super();
    this.x = 2;
  }
}

var field = new ByField();
var assigned = new ByAssignment();

calls.join(" ") + "/" + calls.length + " / " +
  field.x + " " + String(field.recorded) + " / " +
  assigned.x + " " + String(assigned.recorded) + " / " +
  Object.prototype.hasOwnProperty.call(field, "x") + " " +
  Object.prototype.hasOwnProperty.call(assigned, "x");
