// Text that is not ASCII leaves this host as UTF-8 on both streams, whatever code page the
// console or pipe it writes to was opened with. The source is ASCII on purpose: the escapes are
// decoded by the language, so the only encoding under test is the one the host writes with.
print("caf\u00e9 \u221b8=2 \ud83d\ude00");
"na\u00efve \u00fc";
