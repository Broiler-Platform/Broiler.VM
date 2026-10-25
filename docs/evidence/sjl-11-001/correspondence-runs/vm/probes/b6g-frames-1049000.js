// B6g: /(?:ab)*/ on (ab)^1049000: past the frames' 2^20, with about half the trail's 2^21 writes.
// One case per file.
var s = 'ab'.repeat(1049000);
t('/(?:ab)*/ exec on (ab)^1049000', function () { var r = /(?:ab)*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
