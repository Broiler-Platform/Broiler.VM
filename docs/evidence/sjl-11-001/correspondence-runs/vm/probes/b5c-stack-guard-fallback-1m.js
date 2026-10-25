// B5c: B5a at 1,000,000 iterations (the shell's script thread has 16 MiB of stack, so B5a's 100,000
// fit). The specification's answer is the whole input. One case per file.
var s = 'ab'.repeat(1000000);
t('/(?:ab)*/u exec on (ab)^1000000', function () { var r = /(?:ab)*/u.exec(s); return r === null ? R('null') : R('@' + r.index + ' length ' + r[0].length); });
