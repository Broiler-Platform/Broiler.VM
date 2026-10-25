// B6f: /(?:ab)*/ on (ab)^1048000: one frame and one cell write per iteration, under the frames'
// 2^20 (1,048,576). One case per file.
var s = 'ab'.repeat(1048000);
t('/(?:ab)*/ exec on (ab)^1048000', function () { var r = /(?:ab)*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
