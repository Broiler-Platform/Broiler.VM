// A RUNAWAY RECURSION THE PROGRAM ITSELF CATCHES, which is the shape the language promises and
// which this host could not give until 2026-09-04.
//
// `Maximum call stack size exceeded` is a catchable exception in every engine, and real programs
// catch it: a recursive descent probing its own depth, a benchmark sizing a workload, a conformance
// case asserting the error's type. While the call-depth BUDGET answered first, exhaustion was an
// abort no guest could see and the guard this program writes never ran.
//
// The companion fixtures are the other two answers: `an-unbounded-recursion.js` reaches the same
// bound with nobody catching it, and the same file under a tighter `--call-depth` reaches the
// host's ceiling instead, which is an abort and is meant to be.

var reached = 0;

function down() {
  reached++;
  down();
}

try {
  down();
  print("no refusal");
} catch (failure) {
  print(failure.name + " after more than a thousand frames: " + (reached > 1000));
}
