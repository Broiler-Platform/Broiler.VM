// A module with a top-level `await` finishes a turn after it starts, and a dynamic import of one
// settles when it has FINISHED. An importer handed the namespace at the moment the graph started
// would read a binding the body has not reached and get the dead zone's `ReferenceError`.
import("./late.mjs").then(
  function (namespace) {
    print("waited " + namespace.ready);
  },
  function (reason) {
    print("rejected " + reason.name);
  });

"asked";
