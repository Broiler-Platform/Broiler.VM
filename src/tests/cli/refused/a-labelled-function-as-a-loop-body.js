// IsLabelledFunction: a function declaration under labels may be a label's item and nothing
// else's, so a loop, an `if` or a `with` cannot take one as its body, in sloppy code too. Until
// 2026-10-03 it was admitted (JSC-250).
while (false) outer: inner: function f() {}
