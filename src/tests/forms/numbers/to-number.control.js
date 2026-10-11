// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TO-NUMBER (CONTROL) - the workload in to-number.js minus the conversions.
//
// The same strings and loops, with each of the four conversions replaced by a read of the string's
// length. The difference between the two is the conversions' cost.

var strings = ["0", "-0", "1", "  42  ", "3.14159", "-2.5e-3", "1e21", "0x1F", "0b1011", "0o777",
    "Infinity", "-Infinity", "NaN", "", "   ", "12abc", "1_000", ".5", "5.", "123456789012345678901234567890",
    "4.9406564584124654e-324", "1.7976931348623157e308", "0.1", "9007199254740993", " ﻿7 "];
var total = 0;
for (var i = 0; i < 1000; i++) {
    for (var j = 0; j < strings.length; j++) {
        var s = strings[j];
        var n = s.length;
        if (n === n) total += 1;
        var p = s.length;
        if (p === p) total += 1;
        var q = s.length;
        if (q === q) total += 1;
        var r = s.length;
        if (r === r) total += 1;
    }
}
total;
