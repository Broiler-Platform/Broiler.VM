// S3f: 200,000 nested non-capturing groups without u (Broiler.JS's .NET translation). One case per file.
var p = '(?:'.repeat(200000) + 'a' + ')'.repeat(200000);
t('(?: x200000 new RegExp', function () { return m(new RegExp(p, ''), 'a'); });
