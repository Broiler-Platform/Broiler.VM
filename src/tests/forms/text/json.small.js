// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// JSON - JSON.parse over nested documents, escapes, numbers and white space, no deeper than either
// engine's limit; JSON.stringify's quoting of control characters and of text outside ASCII, and its
// gap from a number and from a string, with and without a replacer array. No text here is refused,
// and no reviver, toJSON or getter runs.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var n = 0;
var docs = [
  '{"a":[1,2,{"b":"c\\n\\u00e9\\"\\\\"}],"d":-0.5e-3,"e":true,"f":null}',
  ' [ 1 , 2.5 , -3e2 , "x" , [ [ [ ] ] ] , { } ] ',
  '{"k":"\\ud83d\\ude00","t":"\\t\\b\\f\\r\\/","n":12345678901234567890}',
  '[' + '['.repeat(40) + '7' + ']'.repeat(40) + ']'
];

for (var i = 0; i < 60; i++) {
  var v = JSON.parse(docs[i % 4]);
  var s = JSON.stringify(v);
  n += s.length;
  n += JSON.stringify(v, null, i % 12).length * 3;
  n += JSON.stringify(v, ["a", "b", "k"], "--" + i).length * 5;
  n += JSON.stringify("\u0001\u001f  é \"" + i).length * 7;
}

n;
