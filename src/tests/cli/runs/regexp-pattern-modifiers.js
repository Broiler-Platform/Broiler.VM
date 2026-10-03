// A group `(?ims-ims:...)` turns the i, m and s flags on or off for its body alone: case folding,
// `^` and `$` at line terminators, `.` over line terminators, and `\b` under `iu`, with each
// nested group restoring its outer flags and a backreference compared as its own position says.
// A repeated flag, a flag both added and removed, an unknown letter and `(?-:` are early errors,
// and the regular expression's own flags do not change. Until 2026-10-03 every such group was a
// SyntaxError. The values are the specification's and test262's (built-ins/RegExp/regexp-modifiers);
// the comparison engine reads them only under --js-regexp-modifiers (JSC-244).
function t(f) { try { return String(f()); } catch (e) { return e.name; } }
var r = [];
r.push(t(() => /(?i:a)b/.test("Ab")) + ":" + t(() => /(?i:a)b/.test("AB")));
r.push(t(() => /(?-i:a)b/i.test("aB")) + ":" + t(() => /(?-i:a)b/i.test("AB")));
r.push(t(() => /(?m:^b)/.test("a\nb")) + ":" + t(() => /(?-m:^b)/m.test("a\nb")) + ":" + t(() => /(?-m:b$)/m.test("b\na")));
r.push(t(() => /(?s:.)/.test("\n")) + ":" + t(() => /(?-s:.)/s.test("\n")));
r.push(t(() => /(?i:\bk)/u.test("\u212A")) + ":" + t(() => /\bk/u.test("\u212A")));
r.push(t(() => /(?i:[a-c])+/.exec("xAbC")[0]));
r.push(t(() => /(a)(?i:\1)/.test("aA")) + ":" + t(() => /(a)\1/.test("aA")));
r.push(t(() => /(?i:a(?-i:b))/.test("Ab")) + ":" + t(() => /(?i:a(?-i:b))/.test("AB")));
r.push(t(() => /(?<=(?i:a))b/.test("Ab")));
r.push(t(() => /(?ims:a)/.flags === "" && !/(?i:a)/.ignoreCase));
r.push(["(?ii:a)", "(?i-i:a)", "(?-:a)", "(?x:a)", "(?i)a"].map(s => t(() => new RegExp(s))).join(","));
r.join(" / ")
