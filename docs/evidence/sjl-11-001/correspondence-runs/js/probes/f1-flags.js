// F1: flag strings. Each of dgimsuvy at most once, never both u and v; flags reports them in the
// order "dgimsuvy" (22.2.6.4).
var flags = ['', 'g', 'gg', 'x', 'G', 'uv', 'vu', 'yvsmigd', 'dgimsuy', 'ygmisd', ' g', 'g ', 'gi\u0000'];
for (var i = 0; i < flags.length; i++) {
  (function (f) {
    t('new RegExp("a", ' + q(f) + ').flags', function () { return new RegExp('a', f).flags; });
  })(flags[i]);
}
var lits = ['gg', 'uv', 'x', 'dgimsvy', 'yvsmigd'];
for (var j = 0; j < lits.length; j++) {
  (function (f) {
    t('literal /a/' + f + ' .flags', function () { return L('a', f).flags; });
    t('literal /a/' + f + ', not evaluated', function () { return early('a', f); });
  })(lits[j]);
}
t('flags getter over a plain object', function () {
  var get = Object.getOwnPropertyDescriptor(RegExp.prototype, 'flags').get;
  return get.call({ hasIndices: 1, global: 1, ignoreCase: 0, multiline: '', dotAll: 1, unicode: 0, unicodeSets: 1, sticky: 1 });
});
t('flags getter read order', function () {
  var get = Object.getOwnPropertyDescriptor(RegExp.prototype, 'flags').get, seen = [];
  var o = {};
  ['hasIndices', 'global', 'ignoreCase', 'multiline', 'dotAll', 'unicode', 'unicodeSets', 'sticky'].forEach(function (k) {
    Object.defineProperty(o, k, { get: function () { seen.push(k); return false; } });
  });
  get.call(o);
  return seen.join(',');
});
t('/a/dgimsy accessors', function () { var r = /a/dgimsy; return [r.hasIndices, r.global, r.ignoreCase, r.multiline, r.dotAll, r.unicode, r.unicodeSets, r.sticky]; });
t('/a/v accessors unicode, unicodeSets', function () { var r = /a/v; return [r.unicode, r.unicodeSets]; });
t('RegExp.prototype.global', function () { return RegExp.prototype.global; });
t('RegExp.prototype.flags', function () { return RegExp.prototype.flags; });
t('String(new RegExp("a", "yg"))', function () { return String(new RegExp('a', 'yg')); });
