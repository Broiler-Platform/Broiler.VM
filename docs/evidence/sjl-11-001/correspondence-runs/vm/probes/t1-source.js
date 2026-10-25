// T1: source and toString (EscapeRegExpPattern, 22.2.6.13.1). The text admits any S whose
// evaluation as a literal behaves as the pattern P does, with "/" and the line terminators escaped
// so that "/" + S + "/" + F is a literal; the same P and F must give the same S on every call; an
// empty pattern is "(?:)". Each literal below has a constructor twin with the same P and F.
t('/\\u{61}/u.source (literal)', function () { return L('\\u{61}', 'u').source; });
t('new RegExp("\\\\u{61}", "u").source', function () { return new RegExp('\\u{61}', 'u').source; });
t('/\\u{1F600}/u.source (literal)', function () { return L('\\u{1F600}', 'u').source; });
t('new RegExp("\\\\u{1F600}", "u").source', function () { return new RegExp('\\u{1F600}', 'u').source; });
t('/\\u{D83D}/u.source (literal)', function () { return L('\\u{D83D}', 'u').source; });
t('new RegExp("\\\\u{D83D}", "u").source', function () { return new RegExp('\\u{D83D}', 'u').source; });
t('/[\\u{61}-\\u{7A}]/v.source (literal)', function () { return L('[\\u{61}-\\u{7A}]', 'v').source; });
t('new RegExp("[\\\\u{61}-\\\\u{7A}]", "v").source', function () { return new RegExp('[\\u{61}-\\u{7A}]', 'v').source; });
t('/\\u{3}/.source (literal, no u)', function () { return L('\\u{3}', '').source; });
t('/\\\\u{61}/u.source (literal, escaped backslash)', function () { return L('\\\\u{61}', 'u').source; });
t('/a(b)c/.source', function () { return L('a(b)c', '').source; });
t('/[/]/.source', function () { return L('[/]', '').source; });
t('new RegExp("[/]").source', function () { return new RegExp('[/]').source; });
t('new RegExp("/").source', function () { return new RegExp('/').source; });
t('new RegExp("\\\\/").source', function () { return new RegExp('\\/').source; });
t('new RegExp("a/b").toString()', function () { return new RegExp('a/b').toString(); });
t('new RegExp("\\n").source', function () { return new RegExp('\n').source; });
t('new RegExp("\\r").source', function () { return new RegExp('\r').source; });
t('new RegExp("\\u2028").source', function () { return new RegExp('\u2028').source; });
t('new RegExp("\\u2029").source', function () { return new RegExp('\u2029').source; });
t('new RegExp("\\\\\\n").source (backslash, LF)', function () { return new RegExp('\\\n').source; });
t('new RegExp("\\\\\\u2028").source (backslash, LS)', function () { return new RegExp('\\\u2028').source; });
t('new RegExp("\\\\\\n").test("\\n")', function () { return new RegExp('\\\n').test('\n'); });
t('new RegExp("").source', function () { return new RegExp('').source; });
t('RegExp.prototype.source', function () { return RegExp.prototype.source; });
t('RegExp.prototype.toString()', function () { return RegExp.prototype.toString(); });
t('new RegExp("\\ud83d").source', function () { return new RegExp('\ud83d').source; });
t('new RegExp("\\u0085").source', function () { return new RegExp('\u0085').source; });
t('round trip of new RegExp("[/]\\n/", "g")', function () { var r = new RegExp('[/]\n/', 'g'); return String((0, eval)(String(r))) === String(r); });
