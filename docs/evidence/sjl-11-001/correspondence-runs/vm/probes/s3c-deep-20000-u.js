// S3c: 20,000 nested non-capturing groups under u (Broiler.JS routes every u pattern to
// Broiler.Regex 0.1.0-preview.4; one case per file: a process may not survive it).
var p = '(?:'.repeat(20000) + 'a' + ')'.repeat(20000);
t('(?: x20000 /u new RegExp', function () { return m(new RegExp(p, 'u'), 'a'); });
