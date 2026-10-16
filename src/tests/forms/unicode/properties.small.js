// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// PROPERTIES - the Unicode data the regular-expression matcher and the compiler read: property escapes
// by General_Category, Script, Script_Extensions and binary property, a set operation and a property of
// strings under v, simple case folding under iu and the canonicalisation under i, and identifiers outside
// ASCII compiled through the Function constructor. Every name is written exactly as the specification's
// tables give it. The matcher itself is milestone SJL-11's.
// A fuel-parity probe of milestone SJL-7 (README beside it). Nothing here is timed.

var n = 0;
var text = "Hello ΑΣσς World 日本語のテキスト " +
  "👍🏽 🇩🇪 ſtraße K k ẞ ℘x 𐐀𐐨 " +
  "a b　c\u0085d 123 ١٢";
var patterns = [/\p{Lu}/gu, /\p{General_Category=Lowercase_Letter}/gu, /\p{Script=Greek}/gu,
  /\p{scx=Hira}/gu, /\p{ID_Start}\p{ID_Continue}*/gu, /\p{White_Space}/gu, /[\p{L}--\p{Ll}]/gv,
  /\p{RGI_Emoji}/gv, /[a-z]+/giu, /[a-z]+/gi, /σ/giu, /\w+/giu, /\p{sc=Hrkt}/gu, /\p{Nd}+/gu,
  /\P{Any}/gu, /\p{Emoji_Presentation}/gu];

for (var i = 0; i < 40; i++) {
  for (var j = 0; j < patterns.length; j++) {
    var m = text.match(patterns[j]);
    n += m === null ? j : m.length * (j + 1);
  }
  var names = ["αβ", "ж", "日", "℘", "a‌", "𝑥", "゛"];
  var id = names[i % names.length] + i;
  n += new Function("var " + id + " = " + i + "; return " + id + ";")();
  n += new RegExp("\\p{Script_Extensions=" + ["Latin", "Greek", "Cyrillic", "Han"][i % 4] + "}", "u").test(text) ? 1 : 0;
}

n;
