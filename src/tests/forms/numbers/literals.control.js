// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// LITERALS (CONTROL) - the workload in literals.js with every literal written as 0.
//
// The same body shape and the same number of compilations; only the literal forms differ, so the
// difference between the two is the cost of computing the values of the forms literals.js writes.

var parts = ["0", "0", "0", "0", "0", "0", "0", "0", "0",
    "0", "0", "0", "0", "0", "0", "0",
    "0"];
var body = "var t = 0;";
for (var k = 0; k < 40; k++) {
    body += "t += [" + parts.join(", ") + "].length;";
}
body += "return t;";
var total = 0;
for (var i = 0; i < 100; i++) {
    total += new Function(body)();
}
total;
