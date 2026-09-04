// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The Octane driver this repository holds, for running ONE benchmark through the end-user host.
//
// The Octane checkout's own run.js loads every benchmark and reports a geometric mean. That is not
// what a profile being brought up wants: it wants one benchmark, named on the command line, with
// its score printed. So the driver is here rather than there, it is the harness's own API and
// nothing else, and it holds no copy of any benchmark.
//
//   broiler-js <octane>/base.js <octane>/richards.js src/tests/octane/run-one.js
//
// base.js runs the whole suite synchronously when there is no `window.setTimeout` to yield to, so
// no event loop is needed and the score is printed before RunSuites returns.
//
// IT RE-THROWS AT THE END RATHER THAN ONLY PRINTING. A benchmark that reports an error still lets
// the harness finish and still prints a line, so a driver that only printed would leave the host
// exiting 0 on a run that did not produce a score - and a caller reading the exit code would be
// told the wrong thing. The throw happens after RunSuites returns so that the printed lines are
// the whole run rather than the part before the first failure.

var octaneFailure = null;

BenchmarkSuite.RunSuites({
  NotifyStart: function (name) { print("start " + name); },
  NotifyResult: function (name, result) { print("result " + name + " " + result); },
  NotifyError: function (name, error) {
    octaneFailure = name + ": " + error;
    print("error " + name + " " + error);
  },
  NotifyScore: function (score) { print("score " + score); }
});

if (octaneFailure !== null) {
  throw new Error("a benchmark reported an error: " + octaneFailure);
}
