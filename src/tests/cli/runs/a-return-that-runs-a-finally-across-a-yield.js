// `gen.return` ON A GENERATOR SUSPENDED INSIDE A `try` MUST RUN THE `finally`, and a `catch` in the
// same statement must NOT see it. Both halves are here, because an implementation that routed a
// forced return through the ordinary throw path would pass the first half and fail the second: the
// `catch` would run, `caught` would become 1, and the generator would carry on instead of ending.
var caught = 0;
var finalised = 0;

function* guarded() {
  try {
    yield "suspended";
  } catch (e) {
    caught = caught + 1;
    yield "should not be reached";
  } finally {
    finalised = finalised + 1;
  }

  yield "also not reached";
}

var it = guarded();
it.next();
var ended = it.return("forced");
"caught=" + caught + " finalised=" + finalised + " value=" + ended.value + " done=" + ended.done;
