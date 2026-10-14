// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TRIM-UTF16 - trim, trimStart and trimEnd over the white space both engines already agree on;
// isWellFormed and toWellFormed over lone and paired surrogates; codePointAt, at and the string
// iterator over astral text.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var spaces = [" ", "\t", "\n", "\r", "\v", "\f", " ", " ", " ", " ", " ", " ", "　", "﻿"];
var n = 0;

for (var i = 0; i < 60; i++) {
  var pad = spaces[i % spaces.length] + spaces[(i * 5) % spaces.length];
  var s = pad + "word" + i + pad;
  n += s.trim().length + s.trimStart().length * 3 + s.trimEnd().length * 7;
}

var units = ["a", "😀", "\uD800", "\uDFFF", "b", "􏿿", "\uDC00\uD800"];

for (var j = 0; j < 80; j++) {
  var t = units[j % 7] + units[(j * 3) % 7] + units[(j * 5) % 7] + "xyz";
  n += (t.isWellFormed() ? 1 : 2) + t.toWellFormed().charCodeAt(0) % 97;
  n += t.codePointAt(0) % 89 + t.codePointAt(1) % 83 + (t.at(-1) === "z" ? 1 : 0);

  for (var c of t) {
    n += c.length;
  }
}

n;
