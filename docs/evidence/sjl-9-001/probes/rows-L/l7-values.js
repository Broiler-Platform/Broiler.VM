// SJL-9.1 sub-area (a), rows L29-L34: Intl.supportedValuesOf (ECMA-402 8.3.2 and AvailableCanonical* 6.x),
// each key's list shown as its count, its first and last values, and its order and duplicates.
function summary(a) {
  var sorted = true, dup = false;
  for (var i = 1; i < a.length; i++) { if (a[i - 1] > a[i]) sorted = false; if (a[i - 1] === a[i]) dup = true; }
  return R(a.length + ' values, sorted ' + sorted + ', duplicates ' + dup + ': ' + show(a.slice(0, 6)) + ' ... ' + show(a.slice(-3)));
}
['calendar', 'collation', 'currency', 'numberingSystem', 'timeZone', 'unit'].forEach(function (k) {
  t('supportedValuesOf ' + q(k), function () { return summary(Intl.supportedValuesOf(k)); });
});
t('calendar, whole', function () { return Intl.supportedValuesOf('calendar'); });
t('collation, whole', function () { return Intl.supportedValuesOf('collation'); });
t('numberingSystem, whole', function () { return Intl.supportedValuesOf('numberingSystem'); });
t('unit, whole', function () { return Intl.supportedValuesOf('unit'); });
t('currency: EUR, USD, XXX, XTS, ZWL, VES, VEB, SLE, XCG, ANG', function () {
  var c = Intl.supportedValuesOf('currency');
  return ['EUR', 'USD', 'XXX', 'XTS', 'ZWL', 'VES', 'VEB', 'SLE', 'XCG', 'ANG'].map(function (x) { return x + ':' + (c.indexOf(x) >= 0); });
});
t('timeZone: links and primaries', function () {
  var z = Intl.supportedValuesOf('timeZone');
  return ['UTC', 'Etc/UTC', 'Etc/GMT', 'Asia/Kolkata', 'Asia/Calcutta', 'Europe/Kyiv', 'Europe/Kiev', 'America/Buenos_Aires', 'America/Argentina/Buenos_Aires', 'Antarctica/South_Pole', 'EST5EDT', 'Etc/GMT+5'].map(function (x) { return x + ':' + (z.indexOf(x) >= 0); });
});
// Each calendar and numbering system listed is accepted by the services that use it.
t('every calendar resolves in DateTimeFormat', function () {
  return Intl.supportedValuesOf('calendar').filter(function (c) { return new Intl.DateTimeFormat('en', { calendar: c }).resolvedOptions().calendar !== c; });
});
t('every numbering system resolves in NumberFormat', function () {
  return Intl.supportedValuesOf('numberingSystem').filter(function (n) { return new Intl.NumberFormat('en', { numberingSystem: n }).resolvedOptions().numberingSystem !== n; });
});
t('every collation resolves in some Collator', function () {
  return Intl.supportedValuesOf('collation').filter(function (co) {
    return ['en', 'de', 'zh', 'ja', 'ko', 'es', 'sv'].every(function (l) { return new Intl.Collator(l, { collation: co }).resolvedOptions().collation !== co; });
  });
});
t('every unit is accepted by NumberFormat', function () {
  return Intl.supportedValuesOf('unit').filter(function (u) { try { new Intl.NumberFormat('en', { style: 'unit', unit: u }); return false; } catch (e) { return true; } });
});
t('every currency is well-formed', function () { return Intl.supportedValuesOf('currency').filter(function (c) { return !/^[A-Z]{3}$/.test(c); }); });
// Keys that are refused, and the key's coercion.
['', 'calendars', 'Calendar', 'region', 'language', 'hourCycle'].forEach(function (k) {
  t('supportedValuesOf ' + q(k), function () { return Intl.supportedValuesOf(k); });
});
t('supportedValuesOf undefined', function () { return Intl.supportedValuesOf(); });
t('supportedValuesOf a Symbol', function () { return Intl.supportedValuesOf(Symbol('calendar')); });
t('supportedValuesOf {toString}', function () { return summary(Intl.supportedValuesOf({ toString: function () { return 'calendar'; } })); });
t('fresh array each call', function () { return Intl.supportedValuesOf('calendar') !== Intl.supportedValuesOf('calendar'); });
t('length, name', function () { return [Intl.supportedValuesOf.length, Intl.supportedValuesOf.name]; });
