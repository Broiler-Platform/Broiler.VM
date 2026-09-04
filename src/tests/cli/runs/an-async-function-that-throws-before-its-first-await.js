// A THROW BEFORE THE FIRST `await` REJECTS RATHER THAN THROWING AT THE CALL SITE, and that is the
// half of the semantics an implementation that started the body synchronously is most likely to get
// wrong: the body IS running on the caller's stack when it throws, so letting the exception out
// would have been the natural thing to do. The `try` around the call is what would catch it if it
// did, and the line it would then print is not the one below.
var trace = [];

async function early() {
  throw new TypeError("before");
}

async function late() {
  await 0;
  throw new RangeError("after");
}

try {
  early().catch(function (e) { trace.push("early:" + e.name + ":" + e.message); });
  trace.push("call-returned");
} catch (e) {
  trace.push("threw-at-the-call:" + e.name);
}

late().catch(function (e) {
  trace.push("late:" + e.name + ":" + e.message);
  print(trace.join(" "));
});
