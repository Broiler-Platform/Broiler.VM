// Only an unparenthesised object or array literal is reinterpreted as a pattern; `({}) = 1` is an
// early error. Until 2026-10-03 it destructured (JSC-253).
({}) = 1;
