// A RECURSION WITH NO BASE CASE, WHICH IS THE INPUT THE DEPTH MEASUREMENT BISECTS AGAINST.
//
// What it must never do is terminate the process. A stack overflow is the one failure the CLR
// cannot turn into an exception, so nothing downstream could report it; roadmap section 8 asks for
// a counted bound compared against a limit instead, and JSC-79 records what happened the day this
// interpreter did not have one. `eng/measure-frame-cost.py` raises the ceiling with `--call-depth`
// until the answer stops being a named exhaustion, and the depth at which that happens is what the
// per-frame cost is derived from - a measurement, rather than the estimate section 8 forbids.

function down(n) { return down(n + 1); }
down(0);
