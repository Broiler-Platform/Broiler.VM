// O9 to O13: the objects - option reading order, refusals, resolvedOptions, the bound compare, localeCompare's routing, collations.
function sgn(n) { return n < 0 ? '<' : n > 0 ? '>' : '='; }
function ro(c) { var r = c.resolvedOptions(); return show(r); }
t('o9 read order', function () { var log = []; new Intl.Collator('en', spy({ usage: 'sort', localeMatcher: 'lookup', collation: 'phonebk', numeric: true, caseFirst: 'upper', sensitivity: 'base', ignorePunctuation: true }, log)); return log.join(','); });
t('o9 read order localeCompare', function () { var log = []; 'a'.localeCompare('b', 'en', spy({ usage: 'sort', sensitivity: 'base' }, log)); return log.join(','); });
t('o9 collation not a type', function () { return ro(new Intl.Collator('en', { collation: 'a' })); });
t('o9 collation not a type 2', function () { return ro(new Intl.Collator('en', { collation: 'abc-' })); });
t('o9 sensitivity bad', function () { return ro(new Intl.Collator('en', { sensitivity: 'x' })); });
t('o9 usage bad', function () { return ro(new Intl.Collator('en', { usage: 'x' })); });
t('o9 caseFirst bad', function () { return ro(new Intl.Collator('en', { caseFirst: 'x' })); });
t('o9 localeMatcher bad', function () { return ro(new Intl.Collator('en', { localeMatcher: 'x' })); });
t('o9 numeric string', function () { return ro(new Intl.Collator('en', { numeric: 'false' })); });
t('o10 resolved de-u-co-phonebk-kn-kf-upper', function () { return ro(new Intl.Collator('de-u-co-phonebk-kn-kf-upper')); });
t('o10 resolved en-u-co-phonebk', function () { return ro(new Intl.Collator('en-u-co-phonebk')); });
t('o10 resolved en emoji', function () { return ro(new Intl.Collator('en', { collation: 'emoji' })); });
t('o10 resolved en eor', function () { return ro(new Intl.Collator('en-u-co-eor')); });
t('o10 resolved de search co', function () { return ro(new Intl.Collator('de-u-co-search')); });
t('o10 resolved de-u-co-standard', function () { return ro(new Intl.Collator('de-u-co-standard')); });
t('o10 resolved zh pinyin', function () { return ro(new Intl.Collator('zh-u-co-pinyin')); });
t('o10 resolved de phonebk usage search', function () { return ro(new Intl.Collator('de-u-co-phonebk', { usage: 'search' })); });
t('o10 resolved xx', function () { return ro(new Intl.Collator('xx')); });
t('o10 resolved default', function () { return ro(new Intl.Collator()); });
t('o10 supportedValuesOf collation', function () { return typeof Intl.supportedValuesOf === 'function' ? Intl.supportedValuesOf('collation') : 'absent'; });
t('o10 getCollations en de und zh', function () { return typeof Intl.Locale === 'function' ? ['en', 'de', 'und', 'zh', 'sv'].map(function (l) { return l + ':' + new Intl.Locale(l).getCollations().join('|'); }) : 'absent'; });
t('o11 bound compare identity', function () { var c = new Intl.Collator('en'); return c.compare === c.compare; });
t('o11 bound compare name length', function () { var f = new Intl.Collator('en').compare; return q(f.name) + ' ' + f.length + ' ' + Object.getPrototypeOf(f).constructor.name + ' ' + ('prototype' in f); });
t('o11 compare detached', function () { var f = new Intl.Collator('en').compare; return R(sgn(f('a', 'b')) + ' ' + ['c', 'b', 'a'].sort(f).join('')); });
t('o11 compare coerces', function () { return R(sgn(new Intl.Collator('en').compare(10, 9)) + ' ' + sgn(new Intl.Collator('en').compare()) + ' ' + sgn(new Intl.Collator('en').compare(undefined, 'undefined'))); });
t('o11 compare getter on non-collator', function () { return Object.getOwnPropertyDescriptor(Intl.Collator.prototype, 'compare').get.call({}); });
t('o12 localeCompare after Intl.Collator replaced', function () { var saved = Intl.Collator; Intl.Collator = function () { throw new Error('replaced'); }; try { return R(sgn('a'.localeCompare('b')) + ' ' + sgn('ä'.localeCompare('af', 'de-u-co-phonebk'))); } finally { Intl.Collator = saved; } });
t('o12 localeCompare refusals', function () { return 'a'.localeCompare('b', 'en', { sensitivity: 'x' }); });
t('o12 localeCompare bad tag', function () { return 'a'.localeCompare('b', 'en_US'); });
t('o12 localeCompare a B', function () { return R(sgn('a'.localeCompare('B')) + ' ' + sgn('B'.localeCompare('a')) + ' ' + sgn('ä'.localeCompare('b'))); });
t('o12 Collator call without new', function () { var c = Intl.Collator('de'); return R((c instanceof Intl.Collator) + ' ' + c.resolvedOptions().locale); });
t('o13 default locale and its collation', function () { var c = new Intl.Collator(); return R(c.resolvedOptions().locale + ' ' + sgn(c.compare('a', 'B')) + ' ' + sgn(c.compare('i', 'ı')) + ' ' + sgn(c.compare('ä', 'b'))); });
