// A dynamic import of a module this artifact ALREADY CARRIES, which is the fast path: the
// specifier is one the static import above resolved, so nothing is asked of the host and the
// namespace that arrives is the same instance the static import got - counter included.
import answer, { counter, bump } from "./lib.mjs";

bump();
bump();
bump();

import("./lib.mjs").then(function (namespace) {
  print("same " + (namespace.default === answer) + " " + namespace.counter);
});

answer + counter;
