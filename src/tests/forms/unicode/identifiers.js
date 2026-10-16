// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// IDENTIFIERS - the compiler's identifier scanning outside ASCII, reached through the Function
// constructor: a body declaring and reading identifiers in Greek, Cyrillic, Han, Hiragana and Katakana,
// with Other_ID_Start and Other_ID_Continue characters, a joiner, and supplementary-plane letters. The
// body is compiled again on every iteration, because an identifier is classified once per compilation.
//
// The answer counts the compilations that completed. The control beside this file compiles the same body
// with every identifier written in ASCII of the same length in code points. README.md beside this file
// says which rule judges it.

var names = ["αβγ", "жук", "日本語", "ひらがな", "カタカナ",
    "℘℮", "x··", "a‌b", "𝑥𝑦", "क्ष"];
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
