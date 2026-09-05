// A dependency of the module-goal probe, reached only by a dynamic import. It has a top-level
// `await`, so its body finishes a turn after it starts - which is what makes it the witness for
// WHEN a dynamic import's promise settles. An importer that was handed the namespace at the moment
// the graph STARTED would read `ready` before the body reached it and get the dead zone's
// `ReferenceError`; one handed it when the graph FINISHED reads what the body wrote.
export let ready = "not-yet";

await null;

ready = "ready";
