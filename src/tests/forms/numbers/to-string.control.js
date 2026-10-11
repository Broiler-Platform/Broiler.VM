// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TO-STRING (CONTROL) - the workload in to-string.js minus the conversions.
//
// The same values, the same loops and the same scaling, with each of the eight conversions replaced by
// a comparison of the value with itself. The difference between the two is the conversions' cost.

var values = [0, -0, 1, -1, 0.1, 0.5, 1 / 3, 2 / 3, 123.456, -987.654321, 1e21, 1.5e-7, 5e-324,
    1.7976931348623157e308, 9007199254740992, 9007199254740994, 123456789012345680000, 0.000001,
    1e-7, 255, 4294967295, 3.141592653589793, 2.718281828459045, 1e100, -1e-100];
var total = 0;
for (var i = 0; i < 400; i++) {
    for (var j = 0; j < values.length; j++) {
        var x = values[j] * (1 + i / 1024);
        total += (x === x ? 1 : 0);
        total += (x === x ? 1 : 0) + (x === x ? 1 : 0) + (x === x ? 1 : 0) + (x === x ? 1 : 0);
        total += (x === x ? 1 : 0) + (x === x ? 1 : 0) + (x === x ? 1 : 0);
    }
}
total;
