// The JS survey's probe survey-sjl9-js/probe/p4.js, unchanged but for print, which writes through the prelude's out
// (Broiler.VM's test262 realm has no print); each line is the survey's own print call's arguments joined by spaces.
function __p() { out(Array.prototype.slice.call(arguments).join(" ")); }
var f = new Intl.DateTimeFormat('en', {minute:'2-digit', second:'2-digit', timeZone:'Asia/Kolkata'});
__p(JSON.stringify(f.formatToParts(Date.UTC(2026,9,16,12,0,5))), f.format(Date.UTC(2026,9,16,12,0,5)));
try { __p(JSON.stringify(new Intl.DateTimeFormat('en',{minute:'numeric',second:'numeric'}).formatToParts(8.64e15))); } catch (e) { __p(e.name + ': ' + e.message); }
var o = {}; var log = []; ['weekday','year','month','day','hour','minute','second'].forEach(function(k){ Object.defineProperty(o, k, {get: function(){ log.push(k); return undefined; }}); });
new Date(0).toLocaleString('en', o); __p(log.join(','));
