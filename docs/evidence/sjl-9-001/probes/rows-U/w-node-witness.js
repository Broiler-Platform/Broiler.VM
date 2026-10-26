// Node 22.22.0 (ICU 77.1, CLDR 47.0) as a witness only, with --harmony-intl-duration-format.
function q(s) { var r = ''; for (var i = 0; i < s.length; i++) { var c = s.charCodeAt(i); r += (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i); } return '"' + r + '"'; }
function f(loc, o, d) { try { var df = new Intl.DurationFormat(loc, o); console.log(loc + ' ' + JSON.stringify(o) + ' ' + JSON.stringify(d) + ' => ' + df.resolvedOptions().locale + ' ' + df.resolvedOptions().numberingSystem + ' ' + q(df.format(d))); } catch (e) { console.log(loc + ' ' + JSON.stringify(o) + ' => THROWS ' + e.name); } }
f('en', { style: 'short', numberingSystem: 'arab' }, { hours: -1, minutes: -2, seconds: -3 });
f('en', { style: 'short', numberingSystem: 'arab' }, { days: 1000 });
f('en', { style: 'digital', numberingSystem: 'arab' }, { hours: 1, minutes: 2, seconds: 3, milliseconds: 500 });
f('ar', { style: 'long' }, { hours: 1, minutes: 2 });
f('ar-EG', { style: 'short' }, { hours: 1, minutes: 2 });
['da', 'id', 'si', 'en-DK', 'fi', 'af', 'lt', 'es-CL', 'sr', 'be'].forEach(function (l) { f(l, { style: 'digital' }, { hours: 1, minutes: 2, seconds: 3 }); });
f('en', { style: 'long', minutes: 'numeric' }, { minutes: 5 });
f('en', { style: 'long', hours: 'numeric' }, { hours: 1 });
f('en', { milliseconds: 'numeric', millisecondsDisplay: 'always' }, { hours: 1 });
console.log('nf en arab -1234.5 => ' + q(new Intl.NumberFormat('en', { numberingSystem: 'arab' }).format(-1234.5)));
console.log('nf ar unit hour long 1 => ' + q(new Intl.NumberFormat('ar', { style: 'unit', unit: 'hour', unitDisplay: 'long' }).format(1)));
['zh-TW', 'ja', 'fr', 'es'].forEach(function (l) { f(l, { style: 'long' }, { hours: 1, minutes: 2 }); });
console.log('nf ar unit hour long 2 => ' + q(new Intl.NumberFormat('ar', { style: 'unit', unit: 'hour', unitDisplay: 'long' }).format(2)));
console.log(process.version + ' icu ' + process.versions.icu + ' cldr ' + process.versions.cldr);
