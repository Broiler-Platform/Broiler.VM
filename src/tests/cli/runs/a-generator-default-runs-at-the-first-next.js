// A DEVIATION FROM THE LANGUAGE, PINNED HERE SO IT IS A CHECKED FACT RATHER THAN A SENTENCE IN A
// DOCUMENT. The specification binds a generator's parameters when the generator function is CALLED
// - `FunctionDeclarationInstantiation` runs before the generator object exists - so a default with
// a side effect runs at the call and a default that throws throws from the call.
//
// This bundle binds them at the FIRST RESUMPTION instead, and the cause is the join between two
// families rather than either of them. A unit with a non-simple parameter list carries
// `BindsParameters`, which puts the whole of parameter binding in the unit's own PROLOGUE - and a
// generator's prologue is body code, which does not run until `next`. A unit with a simple
// parameter list is unaffected, because its arguments are copied into slots by the frame.
//
// This row goes red the day that is repaired, which is the point of pinning it: the repair has to
// move it deliberately.
var order = [];

function side() {
  order.push("default");
  return 1;
}

function* g(a = side()) {
  yield a;
}

order.push("before");
var it = g();
order.push("called");
it.next();
order.push("resumed");
print(order.join(","));
