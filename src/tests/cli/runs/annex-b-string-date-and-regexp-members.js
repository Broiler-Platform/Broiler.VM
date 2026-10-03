// ANNEX B.2'S MEMBERS ARE ALL PRESENT (JSP-7, JSC-239), which is the rule section 6 of the roadmap now
// states: the thirteen HTML methods of `String.prototype`, `trimLeft` and `trimRight` as the very
// function objects `trimStart` and `trimEnd` are, `Date`'s `getYear` and `setYear` and `toGMTString`
// as `toUTCString` itself, and `RegExp.prototype.compile`. Each was missing.
var html = [
  'x'.anchor('a"b'), 'x'.big(), 'x'.blink(), 'x'.bold(), 'x'.fixed(), 'x'.fontcolor('red'),
  'x'.fontsize(7), 'x'.italics(), 'x'.link('u'), 'x'.small(), 'x'.strike(), 'x'.sub(), 'x'.sup(),
].join('');

var date = new Date(Date.UTC(2020, 1, 2, 3));
var year = date.getYear();
date.setYear(99);

var pattern = /a/g;
pattern.lastIndex = 3;
var compiled = pattern.compile('b+', 'i');

[
  html,
  String.prototype.trimLeft === String.prototype.trimStart,
  String.prototype.trimRight.name,
  year + ' ' + date.getUTCFullYear(),
  Date.prototype.toGMTString === Date.prototype.toUTCString,
  (compiled === pattern) + ' ' + pattern.source + ' ' + pattern.flags + ' ' + pattern.lastIndex,
].join(' / ');
