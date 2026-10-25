// M6: String.prototype.search when the RegExp's @@search is absent. 22.1.3.21: GetMethod finds
// nothing, so rx = RegExpCreate(regexp) from ToString(regexp) ("/b/"), and Invoke(rx, @@search),
// which is undefined on RegExp.prototype now, so a TypeError.
var saved = RegExp.prototype[Symbol.search];
t('control: "abc".search(/b/)', function () { return 'abc'.search(/b/); });
t('delete RegExp.prototype[Symbol.search]', function () { return delete RegExp.prototype[Symbol.search]; });
t('"abc".search(/b/)', function () { return 'abc'.search(/b/); });
t('"a/b/c".search(/b/)', function () { return 'a/b/c'.search(/b/); });
t('"abc".search(/b/y) (sticky)', function () { return 'abc'.search(/b/y); });
t('"a b".search(/[\\s--\\d]/v)', function () { return 'a b'.search(C('[\\s--\\d]', 'v')); });
t('"abc".search("b")', function () { return 'abc'.search('b'); });
Object.defineProperty(RegExp.prototype, Symbol.search, { value: saved, writable: true, configurable: true });
t('restored: "abc".search(/b/)', function () { return 'abc'.search(/b/); });
t('own @@search = undefined on the instance: "abc".search(re)', function () { var re = /b/; re[Symbol.search] = undefined; return 'abc'.search(re); });
t('own @@search = null on the instance: "abc".search(re)', function () { var re = /b/; re[Symbol.search] = null; return 'abc'.search(re); });
