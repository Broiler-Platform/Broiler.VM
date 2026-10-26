// The survey stubs: every expression of the JS survey's probes p1-p4 (survey-sjl9-js/probe/) and the critic's
// p5-p6 (critic-sjl9-js/probe/), unchanged, one line each, labelled <probe>:<line>, so each stub has a run in
// both engines. Rows of other sub-areas are named in rows.md's survey table; this file only runs them.
// p1
t('p1:1 typeof Intl.DisplayNames.supportedLocalesOf', function () { return typeof Intl.DisplayNames.supportedLocalesOf; });
t('p1:3 DisplayNames de region US; en language de-AT; en currency EUR', function () { var n = new Intl.DisplayNames('de', { type: 'region' }); return [n.of('US'), new Intl.DisplayNames('en', { type: 'language' }).of('de-AT'), new Intl.DisplayNames('en', { type: 'currency' }).of('EUR')]; });
t('p1:4 NF en maxFD 1 1.15, 1.25; maxFD 2 1.005', function () { return [new Intl.NumberFormat('en', { maximumFractionDigits: 1 }).format(1.15), new Intl.NumberFormat('en', { maximumFractionDigits: 1 }).format(1.25), new Intl.NumberFormat('en', { maximumFractionDigits: 2 }).format(1.005)]; });
t('p1:5 PR en selectRange(1,2); de selectRange(0,1); en minFD 1 select(1)', function () { return [new Intl.PluralRules('en').selectRange(1, 2), new Intl.PluralRules('de').selectRange(0, 1), new Intl.PluralRules('en', { minimumFractionDigits: 1 }).select(1)]; });
t('p1:6 NF en minFD 2 stripIfInteger 5', function () { return new Intl.NumberFormat('en', { minimumFractionDigits: 2, trailingZeroDisplay: 'stripIfInteger' }).format(5); });
t('p1:7 NF en currency EUR name 2', function () { return new Intl.NumberFormat('en', { style: 'currency', currency: 'EUR', currencyDisplay: 'name' }).format(2); });
t('p1:8 DTF de month long 0; weekday long 0', function () { return [new Intl.DateTimeFormat('de', { month: 'long' }).format(0), new Intl.DateTimeFormat('de', { weekday: 'long' }).format(0)]; });
t('p1:9 Collator en numeric a2/a10; caseFirst upper a/A', function () { return [new Intl.Collator('en', { numeric: true }).compare('a2', 'a10'), new Intl.Collator('en', { caseFirst: 'upper' }).compare('a', 'A')]; });
t('p1:10 default DTF locale; NF xx; NF fr-FR', function () { return [Intl.DateTimeFormat().resolvedOptions().locale, new Intl.NumberFormat('xx').resolvedOptions().locale, new Intl.NumberFormat('fr-FR').resolvedOptions().locale]; });
t('p1:11 NF supportedLocalesOf ja, zz, sw-KE', function () { return Intl.NumberFormat.supportedLocalesOf(['ja', 'zz', 'sw-KE']).join(); });
t('p1:12 nf.format === nf.format', function () { var nf = new Intl.NumberFormat('en'); return nf.format === nf.format; });
t('p1:13 df.format === df.format', function () { var df = new Intl.DateTimeFormat('en'); return df.format === df.format; });
// p2
t('p2:2 typeof supportedLocalesOf of each service', function () { var names = ['Collator', 'DateTimeFormat', 'DisplayNames', 'DurationFormat', 'ListFormat', 'NumberFormat', 'PluralRules', 'RelativeTimeFormat', 'Segmenter']; return names.map(function (n) { return n + ':' + typeof Intl[n].supportedLocalesOf; }).join(' '); });
t('p2:3 NF de-DE 1234.5; ja-JP compact 123456; fr-FR 1234.5', function () { return [new Intl.NumberFormat('de-DE').format(1234.5), new Intl.NumberFormat('ja-JP', { notation: 'compact' }).format(123456), new Intl.NumberFormat('fr-FR').format(1234.5)]; });
t('p2:4 DTF ja-JP dateStyle long 0; fr month long 0', function () { return [new Intl.DateTimeFormat('ja-JP', { dateStyle: 'long' }).format(0), new Intl.DateTimeFormat('fr', { month: 'long' }).format(0)]; });
t('p2:5 LF es a,b,c; RTF pl -3 day', function () { return [new Intl.ListFormat('es').format(['a', 'b', 'c']), new Intl.RelativeTimeFormat('pl').format(-3, 'day')]; });
t('p2:6 RTF en numberingSystem arab 12 day', function () { var rt = new Intl.RelativeTimeFormat('en', { numberingSystem: 'arab' }); return rt.format(12, 'day'); });
t('p2:7 typeof lf.format', function () { var lf = new Intl.ListFormat('en'); return typeof lf.format; });
t('p2:8 c.compare === c.compare', function () { var c = new Intl.Collator('en'); return c.compare === c.compare; });
t('p2:9 DTF en Asia/Tokyo hour numeric 0; formatToParts', function () { var d = new Intl.DateTimeFormat('en', { timeZone: 'Asia/Tokyo', hour: 'numeric' }); return [d.format(0), JSON.stringify(d.formatToParts(0))]; });
// p3 (the survey ran it under invariant globalization as well)
t('p3:2 NF supportedLocalesOf de, en, ja', function () { return Intl.NumberFormat.supportedLocalesOf(['de', 'en', 'ja']).join(); });
t('p3:3 NF de 1234.5; resolved locale', function () { return [new Intl.NumberFormat('de').format(1234.5), new Intl.NumberFormat('de').resolvedOptions().locale]; });
t('p3:4 DTF de dateStyle short 0', function () { return new Intl.DateTimeFormat('de', { dateStyle: 'short' }).format(0); });
t('p3:5 sort b,a,B,A,a-umlaut by Collator de; a.localeCompare(B)', function () { return [['b', 'a', 'B', 'A', 'ä'].sort(new Intl.Collator('de').compare).join(), 'a'.localeCompare('B')]; });
t('p3:6 PR ru select(3); LF de a,b', function () { return [new Intl.PluralRules('ru').select(3), new Intl.ListFormat('de').format(['a', 'b'])]; });
// p4
t('p4:2 DTF en minute/second 2-digit Asia/Kolkata', function () { var f = new Intl.DateTimeFormat('en', { minute: '2-digit', second: '2-digit', timeZone: 'Asia/Kolkata' }); return [JSON.stringify(f.formatToParts(Date.UTC(2026, 9, 16, 12, 0, 5))), f.format(Date.UTC(2026, 9, 16, 12, 0, 5))]; });
t('p4:3 DTF formatToParts(8.64e15)', function () { return JSON.stringify(new Intl.DateTimeFormat('en', { minute: 'numeric', second: 'numeric' }).formatToParts(8.64e15)); });
t('p4:4-5 option reads of toLocaleString', function () { var o = {}; var log = []; ['weekday', 'year', 'month', 'day', 'hour', 'minute', 'second'].forEach(function (k) { Object.defineProperty(o, k, { get: function () { log.push(k); return undefined; } }); }); new Date(0).toLocaleString('en', o); return log.join(','); });
// p5 (critic)
t('p5:2 PR cs 3, uk 3, hi 0, he 2', function () { return [new Intl.PluralRules('cs').select(3), new Intl.PluralRules('uk').select(3), new Intl.PluralRules('hi').select(0), new Intl.PluralRules('he').select(2)]; });
t('p5:3 PR cs resolved locale', function () { return new Intl.PluralRules('cs').resolvedOptions().locale; });
t('p5:4 DTF en-u-ca-chinese year/month long', function () { return new Intl.DateTimeFormat('en-u-ca-chinese', { year: 'numeric', month: 'long' }).format(Date.UTC(2024, 1, 15)); });
t('p5:5 DTF zh-u-ca-chinese year formatToParts', function () { return JSON.stringify(new Intl.DateTimeFormat('zh-u-ca-chinese', { year: 'numeric' }).formatToParts(Date.UTC(2024, 1, 15))); });
t('p5:6 DTF th-TH-u-ca-buddhist year 0', function () { return new Intl.DateTimeFormat('th-TH-u-ca-buddhist', { year: 'numeric' }).format(0); });
t('p5:7 NF ar 123; resolved numberingSystem', function () { return [new Intl.NumberFormat('ar').format(123), new Intl.NumberFormat('ar').resolvedOptions().numberingSystem]; });
t('p5:8 Collator th ignorePunctuation', function () { return new Intl.Collator('th').resolvedOptions().ignorePunctuation; });
// p6 (critic)
t('p6:2 PR en select(1n)', function () { return new Intl.PluralRules('en').select(1n); });
t('p6:3 NF en percent 0.125', function () { return new Intl.NumberFormat('en', { style: 'percent' }).format(0.125); });
t('p6:4 NF en currency EUR code 0', function () { return new Intl.NumberFormat('en', { style: 'currency', currency: 'EUR', currencyDisplay: 'code' }).format(0); });
t('p6:5 Collator compare identity', function () { var c = new Intl.Collator('en'); return c.compare === c.compare; });
t('p6:6 Locale en getCollations; de getCollations', function () { return [new Intl.Locale('en').getCollations().join(), new Intl.Locale('de').getCollations().join()]; });
