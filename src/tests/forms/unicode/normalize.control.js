// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// NORMALIZE (CONTROL) - the workload in normalize.js with each normalization replaced by a type test of
// its input. The same strings, loops and accumulation; the difference between the two is the cost of the
// normalizations.

var samples = ["été café", "été café", "가나다라",
    "가나다", "ﬁﬂ Ω Å ㌀", "ᾀᾳΐ άέ",
    "Ǻ q̣̇ ẛ̣", "𝅗𝅥𝅘𝅥", "plain ascii text 0123",
    "ｶﾞﾊﾟ ½ ①"];
var forms = ["NFC", "NFD", "NFKC", "NFKD"];
var total = 0;
for (var i = 0; i < 600; i++) {
    for (var j = 0; j < samples.length; j++) {
        for (var f = 0; f < forms.length; f++) {
            total += typeof samples[j] === "string" ? 1 : 0;
        }
    }
}
total;
