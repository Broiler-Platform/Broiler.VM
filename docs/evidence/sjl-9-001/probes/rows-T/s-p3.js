// The JS survey's probe survey-sjl9-js/probe/p3.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the survey's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
function t(f){ try { return f(); } catch (e) { return e.name + ': ' + e.message; } }
__p(t(function(){ return Intl.NumberFormat.supportedLocalesOf(['de','en','ja']).join(); }));
__p(t(function(){ return new Intl.NumberFormat('de').format(1234.5); }), t(function(){ return new Intl.NumberFormat('de').resolvedOptions().locale; }));
__p(t(function(){ return new Intl.DateTimeFormat('de',{dateStyle:'short'}).format(0); }));
__p(t(function(){ return ['b','a','B','A','ä'].sort(new Intl.Collator('de').compare).join(); }), t(function(){ return 'a'.localeCompare('B'); }));
__p(t(function(){ return new Intl.PluralRules('ru').select(3); }), t(function(){ return new Intl.ListFormat('de').format(['a','b']); }));
