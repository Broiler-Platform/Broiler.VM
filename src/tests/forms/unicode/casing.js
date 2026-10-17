// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CASING - toUpperCase and toLowerCase, and toLocaleUpperCase and toLocaleLowerCase with a Turkic and a
// Lithuanian locale, over ASCII, Latin with diacritics, German sharp s, Greek with final sigma, Cyrillic,
// Armenian ligatures and supplementary-plane letters.
//
// The answer counts the mappings that completed, not their results: SJL-7 corrects results in Broiler.JS,
// and an answer built from the results would change for a correction. The control beside this file is the
// same loops with each mapping replaced by a type test of the input. README.md beside this file says which
// rule judges it.

var samples = ["The Quick Brown Fox", "Straße ÄÖÜ äöü", "ΑΣΤΡΟΣ λόγος",
    "Привет мир", "և ﬀﬁ ŉ", "𐐀𐐁 𐵐𐵑",
    "İstanbul ışık", "İ J̀ ÌÍ", "plain ascii text 0123", "ǅǈǋ ᾀᾈ"];
var total = 0;
for (var i = 0; i < 600; i++) {
    for (var j = 0; j < samples.length; j++) {
        var s = samples[j];
        total += typeof s.toUpperCase() === "string" ? 1 : 0;
        total += typeof s.toLowerCase() === "string" ? 1 : 0;
        total += typeof s.toLocaleUpperCase("tr") === "string" ? 1 : 0;
        total += typeof s.toLocaleLowerCase("lt") === "string" ? 1 : 0;
    }
}
total;
