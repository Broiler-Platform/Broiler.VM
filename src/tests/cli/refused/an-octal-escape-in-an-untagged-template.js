// A TEMPLATE ADMITS NO OCTAL ESCAPE, sloppy code or not: `\1` to `\9`, and `\0` before a digit, are
// escapes only a string literal outside strict code has. An untagged template must be cooked, so the
// escape is refused; a tagged one cooks the chunk to `undefined` instead, and the fixture beside
// this one runs that. Until 2026-09-30 this template's value was "a", NUL, "1".
var value = `a\01`;
