// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// NORMALIZE - String.prototype.normalize in its four forms, over strings already in each form and
// strings that are not: Latin with combining marks, Hangul, compatibility characters, Greek with
// diacritics, supplementary-plane mappings and plain ASCII.
//
// The answer counts the normalizations that completed, not their results: SJL-7 corrects results in
// Broiler.JS, and a measurement whose answer changed with a correction would refuse the very change it
// measures, as SJL-3's did. The control beside this file is the same loops with each normalization
// replaced by a type test of the input. README.md beside this file says which rule judges it.

var samples = ["été café", "été café", "가나다라",
    "가나다", "ﬁﬂ Ω Å ㌀", "ᾀᾳΐ άέ",
    "Ǻ q̣̇ ẛ̣", "𝅗𝅥𝅘𝅥", "plain ascii text 0123",
    "ｶﾞﾊﾟ ½ ①"];
var forms = ["NFC", "NFD", "NFKC", "NFKD"];
var total = 0;
for (var i = 0; i < 600; i++) {
    for (var j = 0; j < samples.length; j++) {
        for (var f = 0; f < forms.length; f++) {
            total += typeof samples[j].normalize(forms[f]) === "string" ? 1 : 0;
        }
    }
}
total;
