// Row U12: formatToParts: each unit's number parts carry the singular unit, the list's literals and the clock's
// separators carry none (ListFormatParts, FormatNumericMinutes/Seconds).
function p(opts, d) {
  t('U12 en ' + JSON.stringify(opts) + ' ' + JSON.stringify(d), function () {
    var r = new Intl.DurationFormat('en', opts).formatToParts(d), a = [];
    for (var i = 0; i < r.length; i++) a.push(r[i].type + ':' + q(r[i].value) + (r[i].unit !== undefined ? ':' + r[i].unit : '') + (Object.keys(r[i]).length !== (r[i].unit !== undefined ? 3 : 2) ? ':keys=' + Object.keys(r[i]).join('/') : ''));
    return R('[' + a.join(', ') + ']');
  });
}
p({ style: 'long' }, { hours: 1, minutes: 2 });
p({ style: 'long' }, { days: 1000, hours: 1, minutes: 2 });
p({ style: 'digital' }, { hours: 1, minutes: 2, seconds: 3, milliseconds: 4 });
p({ style: 'digital' }, { hours: -1, minutes: -2 });
p({ style: 'long', minutes: 'numeric' }, { hours: 1, minutes: 2 });
p({ style: 'narrow' }, { years: 1, months: 2, weeks: 3, days: 4, hours: 5, minutes: 6, seconds: 7, milliseconds: 8, microseconds: 9, nanoseconds: 10 });
p({}, { seconds: 0 });
t('U12 Object.getPrototypeOf(parts[0]) === Object.prototype, parts is an Array', function () { var r = new Intl.DurationFormat('en').formatToParts({ hours: 1 }); return [Object.getPrototypeOf(r[0]) === Object.prototype, Array.isArray(r)]; });
