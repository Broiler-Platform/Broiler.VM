// The narrow manifests do not admit BigInt, and a literal is refused at compile time by name rather
// than read as a Number: the cheap half of the parity roadmap's JSP-2 gate, decided by `--check`
// and retained in the source corpus as refuse-a-bigint-literal (JSC-241).
9007199254740993n;
