// A `with` body is a Statement, and an async function declaration is not one. Until 2026-10-03
// the `with` body had a check of its own that admitted it (JSC-250).
with ({}) async function f() {}
