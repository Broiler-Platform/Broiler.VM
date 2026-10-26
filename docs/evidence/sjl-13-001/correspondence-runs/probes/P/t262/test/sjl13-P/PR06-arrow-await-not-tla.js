/*---
description: >
  sjl13 P GAP probe PR06 (row P32, item 9). An await inside an async arrow at a module's top level is not a
  top-level await (Contains does not look into an ArrowFunction for AwaitExpression), so the module that holds it
  evaluates synchronously and the module that imports it runs before a job the first one queued.
esid: sec-static-semantics-contains
flags: [module, async]
---*/
import "./PR06-b_FIXTURE.js";

Promise.resolve().then(function () {
  var seen = globalThis.sjl13log.join(",");
  if (seen === "a,b,tick") {
    $DONE();
  } else {
    $DONE(new Test262Error("PR06 order " + seen));
  }
});
