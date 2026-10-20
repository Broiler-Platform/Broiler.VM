// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TO-STRING TRANSCRIPT - the loop of src/tests/forms/numbers/to-string.js, with each conversion's text kept.
//
// Bundle sjl-3-001's revised rule (decision-rule-revised-2026-10-20.md) runs this once per arm and form, before any
// lane of part C. It is to-string.js's loop, value for value and conversion for conversion, in the same order. Where
// to-string.js adds each text's length to a total, this keeps the text, one per line, and the program's answer is
// the lines joined. The rule's transcript check holds each text of arm A to the specification's answer, and ties
// the transcript to to-string.js's answer and to its control's. Nothing here is timed.

var values = [0, -0, 1, -1, 0.1, 0.5, 1 / 3, 2 / 3, 123.456, -987.654321, 1e21, 1.5e-7, 5e-324,
    1.7976931348623157e308, 9007199254740992, 9007199254740994, 123456789012345680000, 0.000001,
    1e-7, 255, 4294967295, 3.141592653589793, 2.718281828459045, 1e100, -1e-100];
var lines = [];
for (var i = 0; i < 400; i++) {
    for (var j = 0; j < values.length; j++) {
        var x = values[j] * (1 + i / 1024);
        lines.push(String(x));
        lines.push(x.toString(2));
        lines.push(x.toString(7));
        lines.push(x.toString(16));
        lines.push(x.toString(36));
        lines.push(x.toFixed(4));
        lines.push(x.toExponential(6));
        lines.push(x.toPrecision(10));
    }
}
lines.join("\n");
