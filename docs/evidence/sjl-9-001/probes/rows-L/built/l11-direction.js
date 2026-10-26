// Shared prelude for SJL-9.1's probes (SJL-11.1's form, sjl-11-001/correspondence-runs/*/probes/prelude.js,
// with the regular-expression helpers left out and two Intl helpers added). Broiler.JS's shell has print;
// Broiler.VM's test262 realm has none, so there the lines are collected and handed back in one uncaught
// Error the conformance root prints (epilogue.js). The command-line root has print.
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX,
// so U+202F, U+00A0, a lone surrogate or a line terminator is visible and the output stays ASCII.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
// An engine's error message, unquoted, with every code unit outside printable ASCII as \uXXXX.
function plain(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return r;
}
// Objects are shown with their own enumerable keys in the engine's order (resolvedOptions' order
// is specified, so it is shown as the engine gives it).
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (v && v.__raw !== undefined) return v.__raw;
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  if (typeof v === 'boolean' || typeof v === 'bigint') return String(v);
  if (typeof v === 'function') return 'function ' + (v.name || '(anonymous)');
  if (Array.isArray(v)) { var a = []; for (var i = 0; i < v.length; i++) a.push(show(v[i])); return '[' + a.join(', ') + ']'; }
  if (typeof v === 'object') {
    var k = Object.keys(v), p = [];
    for (var j = 0; j < k.length; j++) p.push(k[j] + ': ' + show(v[k[j]]));
    return '{' + p.join(', ') + '}';
  }
  return String(v);
}
function R(s) { return { __raw: s }; }
function errName(e) {
  return (e && e.constructor && e.constructor.name) || typeof e;
}
// One line per behaviour: the label, then the value or the error's constructor and message.
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// formatToParts / formatRangeToParts as type:"value" pairs (source too, when present).
function parts(p) {
  var a = [];
  for (var i = 0; i < p.length; i++) a.push(p[i].type + (p[i].source !== undefined ? '/' + p[i].source : '') + ':' + q(String(p[i].value)));
  return R('[' + a.join(', ') + ']');
}
// An options bag that records every property read, in order, for observable-order rows.
function spy(bag, log) {
  var keys = Object.keys(bag);
  var o = {};
  keys.forEach(function (k) {
    Object.defineProperty(o, k, { enumerable: true, get: function () { log.push(k); return bag[k]; } });
  });
  return new Proxy(o, { get: function (target, k, r) { if (typeof k === 'string' && keys.indexOf(k) < 0) log.push(k); return Reflect.get(target, k, r); } });
}
// SJL-9.1 sub-area (a), row L17: getTextInfo's direction (ECMA-402 15.5.15 TextDirectionOfLocale: the script, or
// the likely script, and CLDR's script metadata) for every script CLDR 48.2.0's script metadata names (the list
// below is that table's keys at Broiler.VM cd1928d), for scripts it does not name, and for languages written
// without a script. One line per group: the tags whose answer is each value.
var scripts = ['Adlm', 'Aghb', 'Ahom', 'Arab', 'Armi', 'Armn', 'Avst', 'Bali', 'Bamu', 'Bass', 'Batk', 'Beng', 'Berf', 'Bhks', 'Bopo', 'Brah', 'Bugi', 'Buhd', 'Cakm', 'Cans', 'Cari', 'Cham', 'Cher', 'Chrs', 'Copt', 'Cpmn', 'Cprt', 'Cyrl', 'Deva', 'Diak', 'Dogr', 'Dsrt', 'Dupl', 'Egyp', 'Elba', 'Elym', 'Ethi', 'Gara', 'Geor', 'Glag', 'Gong', 'Gonm', 'Goth', 'Gran', 'Grek', 'Gujr', 'Gukh', 'Guru', 'Hanb', 'Hang', 'Hani', 'Hano', 'Hans', 'Hant', 'Hatr', 'Hebr', 'Hira', 'Hluw', 'Hmng', 'Hmnp', 'Hung', 'Ital', 'Jamo', 'Java', 'Jpan', 'Kali', 'Kana', 'Kawi', 'Khar', 'Khmr', 'Khoj', 'Kits', 'Knda', 'Kore', 'Krai', 'Kthi', 'Lana', 'Laoo', 'Latn', 'Lepc', 'Limb', 'Lina', 'Linb', 'Lisu', 'Lyci', 'Lydi', 'Mahj', 'Maka', 'Mand', 'Mani', 'Marc', 'Medf', 'Mend', 'Merc', 'Mero', 'Mlym', 'Modi', 'Mong', 'Mroo', 'Mtei', 'Mult', 'Mymr', 'Nagm', 'Nand', 'Narb', 'Nbat', 'Newa', 'Nkoo', 'Nshu', 'Ogam', 'Olck', 'Onao', 'Orkh', 'Orya', 'Osge', 'Osma', 'Ougr', 'Palm', 'Pauc', 'Perm', 'Phag', 'Phli', 'Phlp', 'Phnx', 'Plrd', 'Prti', 'Rjng', 'Rohg', 'Runr', 'Samr', 'Sarb', 'Saur', 'Sgnw', 'Shaw', 'Shrd', 'Sidd', 'Sidt', 'Sind', 'Sinh', 'Sogd', 'Sogo', 'Sora', 'Soyo', 'Sund', 'Sunu', 'Sylo', 'Syrc', 'Tagb', 'Takr', 'Tale', 'Talu', 'Taml', 'Tang', 'Tavt', 'Tayo', 'Telu', 'Tfng', 'Tglg', 'Thaa', 'Thai', 'Tibt', 'Tirh', 'Tnsa', 'Todr', 'Tols', 'Toto', 'Tutg', 'Ugar', 'Vaii', 'Vith', 'Wara', 'Wcho', 'Xpeo', 'Xsux', 'Yezi', 'Yiii', 'Zanb'];
function dir(tag) { try { return String(new Intl.Locale(tag).getTextInfo().direction); } catch (e) { return 'THROWS ' + errName(e); } }
function groups(tags) {
  var by = {}, order = [];
  tags.forEach(function (tag) { var d = dir(tag); if (!by[d]) { by[d] = []; order.push(d); } by[d].push(tag); });
  return R(order.map(function (d) { return d + ': ' + by[d].join(' '); }).join(' | '));
}
t('und-<script>, every script CLDR names', function () { return groups(scripts.map(function (s) { return 'und-' + s; })); });
t('und-<script>, scripts CLDR does not name', function () { return groups(['und-Qaaa', 'und-Qabx', 'und-Aran', 'und-Zzzz', 'und-Zyyy', 'und-Zinh', 'und-Zxxx', 'und-Zmth', 'und-Zsye', 'und-Latf', 'und-Syre', 'und-Hanb', 'und-Jamo']); });
t('languages without a script', function () { return groups(['ar', 'he', 'fa', 'ur', 'ps', 'sd', 'ug', 'dv', 'yi', 'syr', 'ks', 'ckb', 'mzn', 'lrc', 'glk', 'bal', 'bqi', 'pa-PK', 'uz-AF', 'az-IR', 'ku-IQ', 'ms-BN', 'arc', 'nqo', 'prs', 'ku', 'sd-IN', 'ff-GN', 'en', 'ja', 'xyz', 'und', 'und-IL', 'und-SA', 'zh-MO']); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
