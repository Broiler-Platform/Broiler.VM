// S3b: 20,000 nested non-capturing groups without u (one case per file: a process may not survive it).
var p = '(?:'.repeat(20000) + 'a' + ')'.repeat(20000);
t('(?: x20000 new RegExp', function () { return m(new RegExp(p, ''), 'a'); });
