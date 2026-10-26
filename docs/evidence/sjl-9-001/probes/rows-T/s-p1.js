// The JS survey's probe survey-sjl9-js/probe/p1.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the survey's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
__p(typeof Intl.DisplayNames.supportedLocalesOf);
var n = new Intl.DisplayNames('de', {type:'region'});
__p(n.of('US'), new Intl.DisplayNames('en',{type:'language'}).of('de-AT'), new Intl.DisplayNames('en',{type:'currency'}).of('EUR'));
__p(new Intl.NumberFormat('en',{maximumFractionDigits:1}).format(1.15), new Intl.NumberFormat('en',{maximumFractionDigits:1}).format(1.25), new Intl.NumberFormat('en',{maximumFractionDigits:2}).format(1.005));
__p(new Intl.PluralRules('en').selectRange(1,2), new Intl.PluralRules('de').selectRange(0,1), new Intl.PluralRules('en',{minimumFractionDigits:1}).select(1));
__p(new Intl.NumberFormat('en',{minimumFractionDigits:2, trailingZeroDisplay:'stripIfInteger'}).format(5));
__p(new Intl.NumberFormat('en',{style:'currency',currency:'EUR',currencyDisplay:'name'}).format(2));
__p(new Intl.DateTimeFormat('de',{month:'long'}).format(0), new Intl.DateTimeFormat('de',{weekday:'long'}).format(0));
__p(new Intl.Collator('en',{numeric:true}).compare('a2','a10'), new Intl.Collator('en',{caseFirst:'upper'}).compare('a','A'));
__p(Intl.DateTimeFormat().resolvedOptions().locale, new Intl.NumberFormat('xx').resolvedOptions().locale, new Intl.NumberFormat('fr-FR').resolvedOptions().locale);
__p(Intl.NumberFormat.supportedLocalesOf(['ja','zz','sw-KE']).join());
var nf = new Intl.NumberFormat('en'); __p(nf.format === nf.format);
var df = new Intl.DateTimeFormat('en'); __p(df.format === df.format);
