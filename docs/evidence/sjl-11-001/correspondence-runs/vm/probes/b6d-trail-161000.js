// B6d: /(?:(a)(b)(c))*/ on (abc)^161000. Broiler.VM writes 13 cells per iteration (MarkPos, a Clear
// of six capture cells, six Saves: F/JsRegExpMatcher.cs:3934-3957, :3800-3808) and pushes one frame,
// so 161,000 iterations stay under the trail's 2^21 writes and far under the frames' 2^20. One case per file.
var s = 'abc'.repeat(161000);
t('/(?:(a)(b)(c))*/ exec on (abc)^161000', function () { var r = /(?:(a)(b)(c))*/.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length + ' ' + show([r[1], r[2], r[3]])); });
