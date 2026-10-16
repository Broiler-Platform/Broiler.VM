// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CASING - toUpperCase, toLowerCase and toLocaleUpperCase and toLocaleLowerCase in a realm without Intl,
// where the locale is not read: one-to-one and one-to-many mappings, title-case letters, final sigma in
// and out of its context, supplementary-plane letters, mappings added in Unicode 16 and 17, and lone
// surrogates, which pass through. Every result is far below this family's string ceiling.
// A fuel-parity probe of milestone SJL-7 (README beside it). Nothing here is timed.

var n = 0;
var samples = ["abc", "ABC", "ß", "İ", "ı", "ǅ", "ŉ", "ᾀ", "ΐ",
  "Σ", "ΑΣ", "ΑΣ b", "ªΣ", "ΑΣ́", "𐐀",
  "𐵐", "Ɤ", "꟎", "Ᲊ", "\uD800a", "\uDC00Σ", "Straße", "և",
  "ﬀ", "İ", "ẞ"];

for (var i = 0; i < 80; i++) {
  for (var j = 0; j < samples.length; j++) {
    var s = samples[j] + (i % 2 ? " x" : "Y") + samples[(j + i) % samples.length];
    var u = s.toUpperCase();
    var l = s.toLowerCase();
    n += u.length * 3 + l.length * 5 + u.charCodeAt(0) % 31 + l.charCodeAt(l.length - 1) % 37;
    n += s.toLocaleUpperCase("tr").length + s.toLocaleLowerCase("lt").length;
  }
}

n;
