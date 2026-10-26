// The JS critic's probe critic-sjl9-js/probe/p5.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the probe's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
function tt(f){ try { return f(); } catch (e) { return e.name + ': ' + e.message; } }
__p(tt(function(){ return new Intl.PluralRules('cs').select(3); }), tt(function(){ return new Intl.PluralRules('uk').select(3); }), tt(function(){ return new Intl.PluralRules('hi').select(0); }), tt(function(){ return new Intl.PluralRules('he').select(2); }));
__p(tt(function(){ return new Intl.PluralRules('cs').resolvedOptions().locale; }));
__p(tt(function(){ return new Intl.DateTimeFormat('en-u-ca-chinese',{year:'numeric',month:'long'}).format(Date.UTC(2024,1,15)); }));
__p(tt(function(){ return JSON.stringify(new Intl.DateTimeFormat('zh-u-ca-chinese',{year:'numeric'}).formatToParts(Date.UTC(2024,1,15))); }));
__p(tt(function(){ return new Intl.DateTimeFormat('th-TH-u-ca-buddhist',{year:'numeric'}).format(0); }));
__p(tt(function(){ return new Intl.NumberFormat('ar').format(123); }), tt(function(){ return new Intl.NumberFormat('ar').resolvedOptions().numberingSystem; }));
__p(tt(function(){ return new Intl.Collator('th').resolvedOptions().ignorePunctuation; }));
