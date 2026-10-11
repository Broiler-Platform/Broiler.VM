// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TO-STRING - Number::toString at radix 10, radix 2 to 36, toFixed, toExponential and toPrecision.
//
// The pilot area's runtime half, number to string. Every value class the conversion branches on is
// in the list: zeros of both signs, subnormals, the extremes, integers around 2^53, values whose
// shortest form needs an exponent, and ordinary fractions. Each is scaled per iteration so no answer
// is a constant a host could fold. The control beside this file is the same loop with each conversion
// replaced by a comparison of the value with itself.
//
// The literals are fixed here and are not chosen by a pilot run. README.md beside this file says what
// the workload prices and which rule judges it.

var values = [0, -0, 1, -1, 0.1, 0.5, 1 / 3, 2 / 3, 123.456, -987.654321, 1e21, 1.5e-7, 5e-324,
    1.7976931348623157e308, 9007199254740992, 9007199254740994, 123456789012345680000, 0.000001,
    1e-7, 255, 4294967295, 3.141592653589793, 2.718281828459045, 1e100, -1e-100];
var total = 0;
for (var i = 0; i < 400; i++) {
    for (var j = 0; j < values.length; j++) {
        var x = values[j] * (1 + i / 1024);
        total += String(x).length;
        total += x.toString(2).length + x.toString(7).length + x.toString(16).length + x.toString(36).length;
        total += x.toFixed(4).length + x.toExponential(6).length + x.toPrecision(10).length;
    }
}
total;
