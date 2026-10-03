// A bound function's `length` reads only the target's own `length`, keeps an infinite one and
// one past 2^31, and is 0 for anything that is not a Number. `Object.prototype` keeps its null
// prototype: `Object.setPrototypeOf` throws, `Reflect.setPrototypeOf` answers false, and the
// `__proto__` setter throws for it and for an undefined or null receiver. The native rendering of a
// private method or a bound function has no name, as a NativeFunction must. Until 2026-10-03 each
// answered otherwise (JSC-249). The comparison engine keeps source text and prints the private
// method's, which this profile declines to keep (roadmap section 6).
var r = [];
function t(f) { try { return String(f()); } catch (e) { return e.name; } }
function foo(a, b, c) {}
r.push(foo.bind(null, 1).length);
for (var v of [undefined, "1", Symbol(), new Number(1), Infinity, -Infinity, 2147483648, 2.9]) {
  Object.defineProperty(foo, "length", { value: v });
  r.push(String(foo.bind(null).length));
}
function bar() {}
Object.setPrototypeOf(bar, { length: 42 });
delete bar.length;
r.push(Function.prototype.bind.call(bar, null).length);
r.push(t(() => Object.setPrototypeOf(Object.prototype, {})), t(() => Object.setPrototypeOf(Object.prototype, null) === Object.prototype));
r.push(t(() => Reflect.setPrototypeOf(Object.prototype, {})), t(() => { Object.prototype.__proto__ = {}; }));
var setter = Object.getOwnPropertyDescriptor(Object.prototype, "__proto__").set;
r.push(t(() => setter.call(undefined, {})), t(() => setter.call(null, 1)), t(() => setter.call(1, {})));
class C { #m() {} get() { return this.#m; } }
r.push(String(new C().get()), String(function f() {}.bind()), String(Math.max));
r.join(" / ")
