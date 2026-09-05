// The dependency of the waiting-dynamic-import row: a module whose body finishes a turn after it
// starts, so that WHEN the importer is handed its namespace is observable through a `let` the body
// has not yet reached.
export let ready = "not-yet";

await null;

ready = "ready";
