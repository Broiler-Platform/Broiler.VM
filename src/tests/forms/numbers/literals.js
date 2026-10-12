// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// LITERALS - the compiler's numeric literal values, reached through the Function constructor.
//
// The pilot area's compile-time half: the value of every numeric literal form the grammar admits -
// decimal, exponent, hexadecimal, binary, octal, separated, a decimal that does not round trip, the
// extremes, BigInt in decimal and hexadecimal - computed by the compiler while it compiles a body that
// repeats them. The body is compiled again on every iteration, because a literal's value is computed
// once per compilation and a single compilation would be lost in the process's start-up. The control
// beside this file compiles the same body with every literal written as 0.

var parts = ["1", "0.5", "1e21", "0x1F", "0b1011", "0o777", "1_000_000", "123.456e-7", "9007199254740993",
    "5e-324", "1.7976931348623157e308", "0.1", "3.141592653589793", ".5e10", "1n", "0x1Fn",
    "123456789012345678901234567890n"];
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
