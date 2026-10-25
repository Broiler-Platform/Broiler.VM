// B6e: /(?:(a)(b)(c))*/ on (abc)^162000: 162,000 iterations of 13 cell writes pass the trail's 2^21
// writes, while the frame count, one per iteration, stays far under 2^20. One case per file.
var s = 'abc'.repeat(162000);
t('/(?:(a)(b)(c))*/ exec on (abc)^162000', function () { var r = /(?:(a)(b)(c))*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length + ' ' + show([r[1], r[2], r[3]])); });
