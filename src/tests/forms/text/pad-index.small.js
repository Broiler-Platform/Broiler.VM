// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// PAD-INDEX - padStart, padEnd and repeat well below the string ceiling, with the default and
// explicit fillers; relative indices through String's slice, substring, substr and at, and
// Array's and a typed array's slice, at, fill, copyWithin and splice.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var n = 0;

for (var i = 0; i < 90; i++) {
  var s = "k" + i;
  n += s.padStart(i % 23 + 2).length + s.padEnd(i % 17 + 3, "ab").length;
  n += s.padStart(i % 11 + 4, "xyz").charCodeAt(0) + "-".repeat(i % 13).length;
}

var word = "abcdefghijklmnopqrstuvwxyz";

for (var j = -30; j < 30; j++) {
  n += word.slice(j).length + word.slice(j, -j).length + word.substring(j, 7 - j).length;
  n += word.substr(j, 5).length + (word.at(j) === undefined ? 0 : word.at(j).charCodeAt(0));

  var a = [1, 2, 3, 4, 5, 6, 7, 8];
  n += a.slice(j % 9).length + (a.at(j % 9) === undefined ? 0 : a.at(j % 9));
  a.fill(0, j % 5, -2);
  a.copyWithin(j % 4, 1, -1);
  n += a.splice(j % 6, 2).length + a.length;

  var b = new Int16Array([9, 8, 7, 6, 5, 4]);
  n += b.slice(j % 7).length + b.subarray(-3, j % 6).length;
  b.fill(1, j % 3);
  n += b[5] + (b.at(j % 7) === undefined ? 0 : b.at(j % 7));
}

n;
