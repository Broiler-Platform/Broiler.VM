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
//   broiler-js <octane>/base.js <octane>/richards.js src/tests/octane/run-one.js --quiet
//
// base.js runs the whole suite synchronously when there is no `window.setTimeout` to yield to, so
// no event loop is needed and the score is printed before RunSuites returns.
//
// IT RE-THROWS AT THE END RATHER THAN ONLY PRINTING. A benchmark that reports an error still lets
// the harness finish and still prints a line, so a driver that only printed would leave the host
// exiting 0 on a run that did not produce a score - and a caller reading the exit code would be
// told the wrong thing. The throw happens after RunSuites returns so that the printed lines are
// the whole run rather than the part before the first failure.
//
// IT ALSO PRINTS THE SUITE'S OWN UNFORMATTED SCORE COMPONENTS, added 2026-09-07, and that is the
// line an aggregate is computed from rather than the `result` line above it. Octane's final score
// is `100 * GeometricMean(BenchmarkSuite.scores)` over the RATIOS the suites pushed, and two
// details make the printed `result` and `score` lines the wrong input for it:
//
//   * `BenchmarkSuite.FormatScore` rounds to three significant digits, or to an integer above 100,
//     so a mean taken over printed values is a mean over rounded values; and
//   * a suite with a latency reference - `splay` and `mandreel` are the two - pushes TWO ratios,
//     so a geometric mean over the fifteen per-process `score` lines weights those suites once
//     where the suite's own aggregate weights them twice, and answers a different number.
//
// `BenchmarkSuite.scores` is the harness's own array, in push order, reset by `RunSuites` at the
// start of the run; each entry is `reference[k] / mean`, unscaled and unrounded. Printing it is
// reading the harness's own API, which is all this driver was ever allowed to do. THE COMPONENTS
// ARE RATIOS AND NOT SCORES: a score is a hundred times one of them, and the aggregator, not this
// file, applies that factor.
//
// NO FIGURE PRINTED HERE IS A BASELINE. It is a number about one configuration on one machine, and
// roadmap section 17 governs any figure a document retains; this file retains none.

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

// PRINTED AFTER `RunSuites` RETURNS AND BEFORE THE THROW, for the same reason the throw is late:
// the components belong to the whole run, and a driver that emitted them from inside a callback
// would emit them from a run that had not finished.
var components = BenchmarkSuite.scores;

for (var componentIndex = 0; componentIndex < components.length; componentIndex++) {
  print("component " + components[componentIndex]);
}

if (octaneFailure !== null) {
  throw new Error("a benchmark reported an error: " + octaneFailure);
}
