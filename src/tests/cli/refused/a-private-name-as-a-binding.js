// A private name belongs to a class body and after a `.`; it binds nothing. `var #a` declared a name
// no reference could reach until 2026-10-04 (JSC-256).
var #a = 1;
