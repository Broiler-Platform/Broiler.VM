// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// URI - encodeURI and encodeURIComponent over ASCII, two-, three- and four-octet text, and their
// decodes back, with reserved characters kept escaped by decodeURI; escape and unescape over Latin-1
// and wider units. Only well-formed input: no probe raises URIError.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var n = 0;
var pieces = ["a b", "/?#[]@!$&'()*+,;=", "éß", "€中", "😀𝄞", "~-_.", "%25"];

for (var i = 0; i < 70; i++) {
  var s = pieces[i % 7] + i + pieces[(i * 3) % 7];
  var c = encodeURIComponent(s);
  var u = encodeURI(s);
  n += c.length + u.length * 3;
  n += decodeURIComponent(c).length + decodeURI(u).length * 5 + decodeURI(c).length * 7;
  var e = escape(s);
  n += e.length + unescape(e).length * 11;
}

n += unescape("%u00e9%41%zz%u12").length;

n;
