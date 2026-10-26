function f() {
  return new Error("x").stack;
}
var o = { m: function () { return f(); } };
var s = o.
  m();
s;
