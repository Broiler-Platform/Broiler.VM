// `yield*` FORWARDS, and what it forwards is the whole of what makes it more than a loop. `throw`
// arriving while the delegation is suspended reaches the INNER generator's `catch`, the inner
// generator's own `return` value is what the `yield*` expression evaluates to, and an inner
// iterator with no `throw` method - which is every Array iterator - is a TypeError rather than a
// silent pass-through. All three are here.
var report = [];

function* inner() {
  try {
    yield "inner-1";
  } catch (e) {
    report.push("inner caught " + e);
    yield "inner-recovered";
  }

  return "inner-return";
}

function* outer() {
  report.push("outer got " + (yield* inner()));
}

var it = outer();
it.next();
it.throw("BANG");
it.next();

function* overArray() {
  yield* [1, 2, 3];
}

var over = overArray();
over.next();

try {
  over.throw("nope");
  report.push("the array delegation swallowed a throw");
} catch (e) {
  report.push(e.constructor.name);
}

// The separator is a slash rather than a bar, because the expectation table's own row format is
// bar-separated and a fixture whose output carries one cannot be declared in it.
report.join(" / ");
