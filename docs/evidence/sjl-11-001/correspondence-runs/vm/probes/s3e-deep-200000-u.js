// S3e: 200,000 nested non-capturing groups under u (the shell's script thread has a 16 MiB stack,
// Program.cs ScriptHostStackBytes, so S3c's 20,000 fit). One case per file.
var p = '(?:'.repeat(200000) + 'a' + ')'.repeat(200000);
t('(?: x200000 /u new RegExp', function () { return m(new RegExp(p, 'u'), 'a'); });
