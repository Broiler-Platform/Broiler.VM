// localeCompare answers 0 for canonically equivalent strings, the one ordering requirement ECMA-262
// states without ECMA-402: the order is ordinal over the canonical decompositions. Until 2026-10-03
// the raw code units were compared (JSD-0027 N3, JSC-242). Every value is the comparison engine's.
[
  '\u00e4'.localeCompare('a\u0308'), 'a\u0308'.localeCompare('\u00e4'),
  '\u1E9B\u0323'.localeCompare('\u017F\u0323\u0307'), '\u212B'.localeCompare('\u00C5'),
  'a'.localeCompare('b'), 'b'.localeCompare('a'), ''.localeCompare(''), 'abc'.localeCompare('abc'),
].join(' / ');
