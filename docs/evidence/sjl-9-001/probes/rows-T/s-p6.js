// The JS critic's probe critic-sjl9-js/probe/p6.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the probe's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
function tt(f){ try { return f(); } catch (e) { return e.name + ': ' + e.message; } }
__p(tt(function(){ return new Intl.PluralRules('en').select(1n); }));
__p(tt(function(){ return new Intl.NumberFormat('en',{style:'percent'}).format(0.125); }));
__p(tt(function(){ return new Intl.NumberFormat('en',{style:'currency',currency:'EUR',currencyDisplay:'code'}).format(0); }));
__p(tt(function(){ var c = new Intl.Collator('en'); return c.compare === c.compare; }));
__p(tt(function(){ return new Intl.Locale('en').getCollations().join(); }), tt(function(){ return new Intl.Locale('de').getCollations().join(); }));
