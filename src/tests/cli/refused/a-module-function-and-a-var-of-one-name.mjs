// A module's top-level function is lexical, so a `var` of the same name is an early error.
// Until 2026-10-03 the module compiled (JSC-253).
var f;
function f() {}
