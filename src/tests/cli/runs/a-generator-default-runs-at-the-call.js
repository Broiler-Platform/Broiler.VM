// A GENERATOR'S PARAMETERS ARE BOUND WHEN IT IS CALLED, PINNED HERE SO IT IS A CHECKED FACT RATHER
// THAN A SENTENCE IN A DOCUMENT. The specification runs `FunctionDeclarationInstantiation` before
// the generator object exists, so a default with a side effect runs at the call and a default that
// throws throws from the call - `next` is never reached and there is no generator to reach it with.
//
// THIS ROW USED TO RECORD THE OPPOSITE, and it was pinned for exactly that reason. A unit with a
// non-simple parameter list carries `BindsParameters`, which puts the whole of parameter binding in
// the unit's own PROLOGUE - and a generator's prologue used to be body code, which does not run
// until `next`. The prologue and the body are now two runs of one instruction stream, parted by the
// `EnterBody` seam: the call runs everything above it and the first resumption starts below it
// *(corrected: JSC-220)*.
//
// A SIMPLE PARAMETER LIST WAS NEVER AFFECTED and still is not, because its arguments are copied
// into slots by the frame with no code to run at all.
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
