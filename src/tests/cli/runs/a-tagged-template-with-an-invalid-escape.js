// A TAGGED TEMPLATE MAY HOLD AN ESCAPE A STRING COULD NOT (ES2018's NotEscapeSequence): the chunk's
// cooked value is `undefined` and its raw text is kept, so a tag that reads `raw` - String.raw is
// one - still sees what was written. Until 2026-09-30 the whole program was refused for it.
function tag(strings) {
  return String(strings[0]) + ':' + strings.raw[0];
}

[tag`\unicode`, tag`\xg`, tag`\u{110000}`, tag`\01`, tag`\1`, tag`\0`.length, String.raw`\8 and \u{`].join(' / ');
