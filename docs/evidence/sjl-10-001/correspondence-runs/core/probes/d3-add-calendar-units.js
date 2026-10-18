// D3: Duration.prototype.add and subtract with calendar units, and their second argument.
var log = [];
t('Duration({years: 1}).add({months: 1})', function () { return D({ years: 1 }).add({ months: 1 }); });
t('Duration({years: 1}).add({months: 1}, {relativeTo: "2020-01-01"})', function () { return D({ years: 1 }).add({ months: 1 }, { relativeTo: '2020-01-01' }); });
t('Duration({years: 1}).subtract({months: 1}, {relativeTo: "2020-01-01"})', function () { return D({ years: 1 }).subtract({ months: 1 }, { relativeTo: '2020-01-01' }); });
t('Duration({years: 1}).add({months: 1}, {relativeTo: ZonedDateTime})', function () { return D({ years: 1 }).add({ months: 1 }, { relativeTo: Z('2020-01-01T00:00[UTC]') }); });
t('Duration({weeks: 1}).add({days: 1})', function () { return D({ weeks: 1 }).add({ days: 1 }); });
t('Duration({hours: 1}).add({months: 1})', function () { return D({ hours: 1 }).add({ months: 1 }); });
t('Duration({years: 1}).add({years: -1})', function () { return D({ years: 1 }).add({ years: -1 }); });
t('Duration({hours: 1}).add({hours: 1}, 5)', function () { return D({ hours: 1 }).add({ hours: 1 }, 5); });
t('Duration({hours: 1}).add({hours: 1}, null)', function () { return D({ hours: 1 }).add({ hours: 1 }, null); });
t('Duration({hours: 1}).add({hours: 1}, {get relativeTo() {...}}) and the reads', function () {
  var r = D({ hours: 1 }).add({ hours: 1 }, { get relativeTo() { log.push('relativeTo'); return undefined; } });
  return r.toString() + ' reads=[' + log.join(',') + ']';
});
t('Duration.prototype.add.length', function () { return T.Duration.prototype.add.length; });
