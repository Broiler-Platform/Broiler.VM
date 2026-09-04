// A `finally` MAY ITSELF YIELD, and when it does the forced return waits for it. The sequence is
// the one every engine produces and the one a state machine that discarded the pending completion
// while the finaliser ran would get wrong: the `return` is answered with the finaliser's own yield
// first, and only the step after it carries the value the caller asked to return.
function* deferring() {
  try {
    yield "one";
  } finally {
    yield "from the finaliser";
  }
}

var it = deferring();
var a = it.next();
var b = it.return("asked");
var c = it.next();
var d = it.next();
[a.value, b.value + "/" + b.done, c.value + "/" + c.done, String(d.value) + "/" + d.done].join(" ");
