// The JS survey's probe survey-sjl9-js/probe/p2.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the survey's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
var names = ['Collator','DateTimeFormat','DisplayNames','DurationFormat','ListFormat','NumberFormat','PluralRules','RelativeTimeFormat','Segmenter'];
__p(names.map(function(n){ return n + ':' + typeof Intl[n].supportedLocalesOf; }).join(' '));
__p(new Intl.NumberFormat('de-DE').format(1234.5), new Intl.NumberFormat('ja-JP',{notation:'compact'}).format(123456), new Intl.NumberFormat('fr-FR').format(1234.5));
__p(new Intl.DateTimeFormat('ja-JP',{dateStyle:'long'}).format(0), new Intl.DateTimeFormat('fr',{month:'long'}).format(0));
__p(new Intl.ListFormat('es').format(['a','b','c']), new Intl.RelativeTimeFormat('pl').format(-3,'day'));
var rt = new Intl.RelativeTimeFormat('en', {numberingSystem:'arab'}); __p(rt.format(12, 'day'));
var lf = new Intl.ListFormat('en'); __p(typeof lf.format);
var c = new Intl.Collator('en'); __p(c.compare === c.compare);
var d = new Intl.DateTimeFormat('en',{timeZone:'Asia/Tokyo', hour:'numeric'}); __p(d.format(0), JSON.stringify(d.formatToParts(0)));
