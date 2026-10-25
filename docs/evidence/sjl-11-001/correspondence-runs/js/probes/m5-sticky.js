// M5: the sticky flag. The match is attempted at lastIndex only (RegExpBuiltinExec step 13.d.i).
// Answers first; then a timing that shows whether a failure at lastIndex costs one attempt or a
// forward search (the elapsed milliseconds are data of this run, not a claim).
function at(re, s, li) {
  re.lastIndex = li;
  var r = re.exec(s);
  return R((r === null ? 'null' : '@' + r.index + ' ' + show(Array.prototype.slice.call(r))) + ' lastIndex ' + re.lastIndex);
}
t('/a/y lastIndex 0 on "ba"', function () { return at(/a/y, 'ba', 0); });
t('/a/y lastIndex 1 on "ba"', function () { return at(/a/y, 'ba', 1); });
t('/a/uy lastIndex 0 on "ba"', function () { return at(/a/uy, 'ba', 0); });
t('/b|ab/y lastIndex 0 on "ab"', function () { return at(/b|ab/y, 'ab', 0); });
t('/^b/y lastIndex 1 on "ab"', function () { return at(/^b/y, 'ab', 1); });
t('/^b/my lastIndex 2 on "a\\nb"', function () { return at(/^b/my, 'a\nb', 2); });
t('/(?<=a)b/y lastIndex 1 on "ab"', function () { return at(/(?<=a)b/y, 'ab', 1); });
t('/\\bb/y lastIndex 1 on "ab"', function () { return at(/\bb/y, 'ab', 1); });
t('"aab".search(/b/y)', function () { return 'aab'.search(/b/y); });
t('"aab".match(/a/gy)', function () { return 'aab'.match(/a/gy); });
t('"aaba".replace(/a/gy, "-")', function () { return 'aaba'.replace(/a/gy, '-'); });
t('"a,b".split(/,/y)', function () { return 'a,b'.split(/,/y); });
var s = 'y' + 'a'.repeat(22);
t('/x|(a+)+b/y lastIndex 0 on "y"+a^22 (answer)', function () { var e = elapsed(function () { return at(/x|(a+)+b/y, s, 0); }); return R(e.r.__raw + ' elapsedMs ' + e.ms); });
t('/x|(a+)+b/uy lastIndex 0 on "y"+a^22 (answer)', function () { var e = elapsed(function () { return at(/x|(a+)+b/uy, s, 0); }); return R(e.r.__raw + ' elapsedMs ' + e.ms); });
