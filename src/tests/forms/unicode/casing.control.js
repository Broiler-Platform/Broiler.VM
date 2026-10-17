// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CASING (CONTROL) - the workload in casing.js with each mapping replaced by a type test of its input. The
// same strings, loops and accumulation; the difference between the two is the cost of the mappings.

var samples = ["The Quick Brown Fox", "Straße ÄÖÜ äöü", "ΑΣΤΡΟΣ λόγος",
    "Привет мир", "և ﬀﬁ ŉ", "𐐀𐐁 𐵐𐵑",
    "İstanbul ışık", "İ J̀ ÌÍ", "plain ascii text 0123", "ǅǈǋ ᾀᾈ"];
var total = 0;
for (var i = 0; i < 600; i++) {
    for (var j = 0; j < samples.length; j++) {
        var s = samples[j];
        total += typeof s === "string" ? 1 : 0;
        total += typeof s === "string" ? 1 : 0;
        total += typeof s === "string" ? 1 : 0;
        total += typeof s === "string" ? 1 : 0;
    }
}
total;
