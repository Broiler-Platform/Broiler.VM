// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TO-NUMBER - StringToNumber through Number(), parseFloat, and parseInt at radix 10 and 36.
//
// The pilot area's runtime half, string to number. The strings cover the grammar's branches:
// whitespace of every kind the trim reads (including U+00A0, U+FEFF and U+2028), signs, exponents,
// the three prefixed radix forms, Infinity and NaN, empty and blank strings, a trailing junk tail,
// a separator the grammar refuses, digit strings longer than a double holds, and values on rounding
// boundaries. The control beside this file is the same loop with each conversion replaced by a
// length read.

var strings = ["0", "-0", "1", "  42  ", "3.14159", "-2.5e-3", "1e21", "0x1F", "0b1011", "0o777",
    "Infinity", "-Infinity", "NaN", "", "   ", "12abc", "1_000", ".5", "5.", "123456789012345678901234567890",
    "4.9406564584124654e-324", "1.7976931348623157e308", "0.1", "9007199254740993", " ﻿7 "];
var total = 0;
for (var i = 0; i < 1000; i++) {
    for (var j = 0; j < strings.length; j++) {
        var s = strings[j];
        var n = Number(s);
        if (n === n) total += 1;
        var p = parseFloat(s);
        if (p === p) total += 1;
        var q = parseInt(s, 10);
        if (q === q) total += 1;
        var r = parseInt(s, 36);
        if (r === r) total += 1;
    }
}
total;
