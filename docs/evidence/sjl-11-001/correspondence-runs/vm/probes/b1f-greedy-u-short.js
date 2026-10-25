// B1f: /a*/u on a^9999999, inside the 10,000,000-step fast path; the answer is length 9,999,999.
var s = 'a'.repeat(9999999);
t('/a*/u exec on a^9999999: length', function () { var r = /a*/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
