// A ShadowRealm owns a realm of its own, and only primitives and callables cross its boundary,
// a callable as a new wrapper each time (JSD-0040, JSC-265). An exception crosses as a fresh
// TypeError of the caller's realm, and the shadow realm's global holds nothing a host adds.
var sr = new ShadowRealm();
var rows = [];

function ask(name, f) {
  try { rows.push(name + " " + f()); }
  catch (e) { rows.push(name + " threw " + e.name + (e instanceof TypeError ? " of ours" : "")); }
}

globalThis.leak = 1;
ask("evaluate", function () { return sr.evaluate("6 * 7"); });
ask("its own global", function () { return sr.evaluate("typeof leak"); });
ask("an object", function () { return sr.evaluate("({})"); });

var add = sr.evaluate("(a, b) => a + b");
ask("a wrapper", function () {
  return [add(2, 3), typeof add, add.length, Object.getPrototypeOf(add) === Function.prototype].join(" ");
});
ask("an object argument", function () { return add({}, 1); });
ask("a callable argument", function () { return sr.evaluate("(f) => f(20) + 1")(function (x) { return x * 2; }); });
ask("an exception", function () { return sr.evaluate("() => { throw new RangeError('inside'); }")(); });
ask("a new wrapper each time", function () { return sr.evaluate("globalThis.f = () => 1; f") === sr.evaluate("f"); });
ask("shared Symbols", function () {
  return (sr.evaluate("Symbol.for('k')") === Symbol.for("k")) + " " + (sr.evaluate("Symbol.iterator") === Symbol.iterator);
});
ask("state kept", function () { sr.evaluate("globalThis.n = 1"); return sr.evaluate("++n"); });
ask("host members", function () { return sr.evaluate("typeof print + ' ' + typeof $262"); });
ask("nested", function () { return sr.evaluate("new ShadowRealm().evaluate('1 + 1')"); });
ask("bad source", function () { return sr.evaluate("("); });
rows.join("\n");
