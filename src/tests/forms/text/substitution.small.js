// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// SUBSTITUTION - replacement templates with $$, $&, $`, $', $n, $nn and $<name>, through replace
// and replaceAll with string and regular-expression patterns. Every replacement is a string, and
// every named group comes from the built-in exec, so no part of a template is evaluated with a
// getter.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var n = 0;
var templates = ["[$&]", "$$-$`-$'", "<$1|$2>", "$2$1", "$10$11", "$<y>/$<m>", "$0 $00 $<", "$$$&$$"];

for (var i = 0; i < 70; i++) {
  var text = "a" + i + "-1999-12-31-b" + i + "-2024-02-29";
  var t = templates[i % templates.length];
  n += text.replace("-", t).length;
  n += text.replaceAll("-", t).length;
  n += text.replace(/(\d{4})-(\d{2})/, t).length;
  n += text.replace(/(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})/g, t).length;
  n += "abcdefghijkl".replace(/(a)(b)(c)(d)(e)(f)(g)(h)(i)(j)(k)/, t).length;
}

n;
