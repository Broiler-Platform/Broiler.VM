// STRICT CODE REPLACES NEITHER RESTRICTED NAME, AND A BRACKET DOES NOT CHANGE THAT. `arguments = 1`
// was already refused where the `=` was read; `[arguments] = []` reached no such place, because the
// bracket settled the left-hand side as a pattern before the rule was asked. The same replacement
// was accepted one bracket away from being refused.
//
// The rule belongs on every LEAF of a pattern, which is why `[...eval]` and `({ a: arguments })`
// are refused by the same clause. `runs/the-patterns-a-rest-and-a-shorthand-still-admit.js` holds
// the sloppy-mode side, where both names are ordinary targets.
"use strict";

0, [arguments] = [];
