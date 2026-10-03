// AN ANONYMOUS FUNCTION TAKES ITS NAME FROM A COMPUTED KEY AND FROM A CLASS FIELD (JSP-6, JSC-238),
// as it does from a literal key: `{ [k]: function () {} }`, `class { f = () => {} }`, a private
// field, a static field and a computed field each name the function, where each left it nameless.
// A Symbol key names it in brackets, so the object literal and the class body now agree. The
// computed key is converted before the value is evaluated, and a computed field's key once.
var symbol = Symbol('sym');
var computed = 'com' + 'puted';
var order = '';

var literal = {
  [computed]: function () {},
  [symbol]: function () {},
  ['arrow']: () => 0,
  [{ toString: function () { order += 'key '; return 'ordered'; } }]: (order += 'value', function () {}),
};

var conversions = 0;
var counted = { toString: function () { conversions++; return 'counted'; } };

class Fields {
  field = function () {};
  arrow = () => 0;
  #hidden = function () {};
  static shared = function () {};
  [computed] = function () {};
  [symbol] = () => 0;
  [counted] = function () {};

  hiddenName() {
    return this.#hidden.name;
  }
}

var first = new Fields();
new Fields();

[
  literal.computed.name,
  literal[symbol].name,
  literal.arrow.name,
  order,
  first.field.name,
  first.arrow.name,
  first.hiddenName(),
  Fields.shared.name,
  first.computed.name,
  first[symbol].name,
  first.counted.name + ' ' + conversions,
].join(' / ');
