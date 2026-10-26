// Row N11: the format getter's bound function ([[BoundFormat]], made once), and row N12: the normative
// optional legacy constructor path (ChainNumberFormat, UnwrapNumberFormat, %Intl%.[[FallbackSymbol]]).
var nf = new Intl.NumberFormat('en');
t('nf.format === nf.format', function () { return nf.format === nf.format; });
t('bound format name, length, own prototype', function () { var f = nf.format; return [f.name, f.length, Object.getPrototypeOf(f) === Function.prototype, 'prototype' in f]; });
t('detached bound format(1234.5)', function () { var f = nf.format; return f(1234.5); });
t('[1000, 2.5].map(nf.format)', function () { return [1000, 2.5].map(nf.format); });
t('format getter on a non-NumberFormat', function () { return Object.getOwnPropertyDescriptor(Intl.NumberFormat.prototype, 'format').get.call({}); });
t('format getter descriptor', function () { var d = Object.getOwnPropertyDescriptor(Intl.NumberFormat.prototype, 'format'); return [typeof d.get, d.set, d.enumerable, d.configurable, d.get.name]; });
t('Intl.NumberFormat() without new is a NumberFormat', function () { var f = Intl.NumberFormat('de'); return [f instanceof Intl.NumberFormat, f.format(1234.5)]; });
t('legacy: Intl.NumberFormat.call(Object.create(Intl.NumberFormat.prototype)) returns the receiver', function () {
  var o = Object.create(Intl.NumberFormat.prototype);
  var r = Intl.NumberFormat.call(o, 'de');
  return [r === o, Object.getOwnPropertySymbols(o).map(String)];
});
t('legacy: resolvedOptions and format through the receiver', function () {
  var o = Object.create(Intl.NumberFormat.prototype);
  Intl.NumberFormat.call(o, 'de');
  return [Intl.NumberFormat.prototype.resolvedOptions.call(o).locale, o.format(1234.5)];
});
t('legacy: formatToParts through the receiver', function () {
  var o = Object.create(Intl.NumberFormat.prototype);
  Intl.NumberFormat.call(o, 'de');
  return Intl.NumberFormat.prototype.formatToParts.call(o, 1).length;
});
t('legacy: called on a plain object returns a new NumberFormat', function () { var o = {}; var r = Intl.NumberFormat.call(o, 'de'); return [r === o, r instanceof Intl.NumberFormat]; });
t('legacy: new.target subclass', function () { class N extends Intl.NumberFormat {} var n = new N('de'); return [n instanceof N, n.format(1.5)]; });
