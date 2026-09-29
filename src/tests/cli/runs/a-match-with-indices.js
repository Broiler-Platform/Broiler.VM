// THE `d` FLAG BUILDS `indices`: a [start, end] pair per group in code units, `undefined` for a
// group that did not take part, and `groups` beside them. Until 2026-09-29 `hasIndices` answered
// `true` and a result carried no `indices` at all, which the parity roadmap's JSP-8 records as
// this component disagreeing with itself. The answers were taken from the comparison engine.

var match = /a(?<letter>b)?(c)/d.exec("xxac");
var named = /(?<first>.)(?<second>.)/du.exec("\u{1F600}yz");
var plain = /b/.exec("abc");

[
  JSON.stringify(match.indices),
  Object.keys(match.indices.groups).join(",") + ":" + match.indices.groups.letter,
  Object.getPrototypeOf(match.indices.groups) === null,
  Object.keys(match).join(","),
  JSON.stringify(named.indices),
  JSON.stringify(named.indices.groups),
  String(/x/d.exec("x").indices.groups),
  "indices" in plain,
].join(" / ");
