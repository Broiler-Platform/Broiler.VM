// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// IDENTIFIERS (CONTROL) - the workload in identifiers.js with every identifier written in ASCII of the
// same length in code points. The same body shape and the same number of compilations; the difference
// between the two is the cost of classifying identifiers outside ASCII.

var names = ["abc", "zhk", "nih", "hira", "kata", "pe", "xmm", "azb", "xy", "ksa"];
var body = "var t = 0;";
for (var k = 0; k < 20; k++) {
    for (var j = 0; j < names.length; j++) {
        body += "var " + names[j] + k + " = " + j + "; t += " + names[j] + k + ";";
    }
}
body += "return 1;";
var total = 0;
for (var i = 0; i < 60; i++) {
    total += new Function(body)();
}
total;
