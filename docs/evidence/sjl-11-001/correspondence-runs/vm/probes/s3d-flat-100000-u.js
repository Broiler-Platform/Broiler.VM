// S3d: a flat sequence of 100,000 terms under u, no nesting at all (the critic's case: the
// continuation chain recurses once per term). One case per file.
var p = '(?:a)'.repeat(100000);
t('(?:a) x100000 /u new RegExp on 100000 a', function () { var r = new RegExp(p, 'u').exec('a'.repeat(100000)); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
