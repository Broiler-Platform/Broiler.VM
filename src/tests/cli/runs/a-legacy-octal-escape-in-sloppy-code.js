// OUTSIDE STRICT CODE A STRING LITERAL KEEPS ANNEX B's LEGACY ESCAPES: `\101` is "A", an escape
// starting 0 to 3 reads up to three octal digits and one starting 4 to 7 up to two, `\0` before a
// digit is one of them, and `\8` and `\9` are the digits themselves. Until 2026-09-30 only `\0`
// was read, and every other digit escape was its digit: `'\101'` was "101".
function codes(text) {
  var out = [];
  for (var index = 0; index < text.length; index++) {
    out.push(text.charCodeAt(index));
  }
  return out.join(',');
}

[codes('\101'), codes('\0'), codes('\08'), codes('\012'), codes('\377'), codes('\400'),
  codes('\1234'), codes('\7a'), codes('\8'), codes('\9')].join(' / ');
