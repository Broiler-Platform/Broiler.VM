// toUpperCase and toLowerCase are the Unicode Default Case Conversion over the pinned 17.0.0 tables:
// SpecialCasing.txt's unconditional mappings over UnicodeData.txt's simple ones, and Final_Sigma.
// Until 2026-10-03 the platform's invariant TextInfo mapped one code unit to one: 'ß' stayed 'ß',
// no final sigma was written, and the non-u RegExp Canonicalize matched the 27 Greek letters with
// a ypogegrammeni to their title-case partners (JSD-0027 N2, JSC-242). Every value is the comparison
// engine's.
[
  'ß'.toUpperCase(), '\uFB00'.toUpperCase(), 'ŉ'.toUpperCase(), '\u1F80'.toUpperCase(),
  'ΑΣ'.toLowerCase(), 'ΣΣ'.toLowerCase(), 'Σ'.toLowerCase(), 'ΑΣ.'.toLowerCase(),
  'ΑΣ\u0301Β'.toLowerCase(), 'ΑΣ\u0301'.toLowerCase(),
  'İ'.toLowerCase().length, 'ı'.toUpperCase(), 'ǅ'.toUpperCase() + 'ǅ'.toLowerCase(),
  '\uD801\uDC00'.toLowerCase() === '\uD801\uDC28', '\uD800x'.toUpperCase().length,
  'ß'.toLocaleUpperCase(), 'ΑΣ'.toLocaleLowerCase(),
  /\u1F80/i.test('\u1F88'), /\u1F80/iu.test('\u1F88'), /ß/i.test('SS'),
].join(' / ');
