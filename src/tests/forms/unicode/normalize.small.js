// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// NORMALIZE - String.prototype.normalize in its four forms and its default: canonical and compatibility
// decompositions, composition and its blocking, canonical reordering, Hangul both ways, composition
// exclusions, singletons, supplementary-plane mappings, decompositions added in Unicode 16, ASCII, and
// lone surrogates, which pass through. Every result is far below this family's string ceiling.
// A fuel-parity probe of milestone SJL-7 (README beside it). Nothing here is timed.

var n = 0;
var samples = ["é", "é", "ﬁ", "가", "각", "̈́", "ẛ̣",
  "q̣̇", "ﷺ", "𝅗𝅥", "ཱི", "Ω", "Å", "Ǻ", "क़",
  "𐗉", "𖄡", "a\uD800b", "\uDC00́", "abc", "ABC xyz 123",
  "Ặ", "㌀", "ｶﾞ"];
var forms = ["NFC", "NFD", "NFKC", "NFKD"];

for (var i = 0; i < 60; i++) {
  for (var j = 0; j < samples.length; j++) {
    var s = samples[j] + String.fromCharCode(0x41 + i % 26) + samples[(j + i) % samples.length];
    for (var f = 0; f < forms.length; f++) {
      var r = s.normalize(forms[f]);
      n += r.length * (f + 1) + r.charCodeAt(r.length - 1) % 97;
    }
    n += s.normalize().length;
  }
}

n;
