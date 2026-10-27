// SJL-13.1's correspondence, sub-area T, rows T24b and T24f (added 2026-10-27): value probes, run whole as
// prelude.js + t24-values.js + epilogue.js (the prelude and epilogue beside t-values.js). Each source holds the text
// the specification reads as two divisions, `/b/g`, after `}` (T24b) or after `await` used as an identifier (T24f),
// followed by `;` or by the end of input. T24b7, T24b8 and T24f3 are the sources of the syntax vectors
// regexp-delimiting/0047, 0046 and 0052 of Broiler.JS's corpus revision 30.
t('T24b3 object literal, then /b/g and ;', function () { return ev('var b = 2, g = 3, x; x = {} /b/g;'); });
t('T24b4 object literal, then /b/g and the end of input', function () { return ev('var b = 2, g = 3, x; x = {} /b/g'); });
t('T24b5 function expression, then /b/g and ;', function () { return ev('var b = 2, g = 3, x; x = function () {} /b/g;'); });
t('T24b6 function expression, then /b/g and the end of input', function () { return ev('var b = 2, g = 3, x; x = function () {} /b/g'); });
t('T24b7 object literal, then /b/g and ; then x', function () { return ev('var b = 2, g = 3; var x = {} /b/g; x'); });
t('T24b8 function expression, then /b/g and ; then x', function () { return ev('var b = 2, g = 3; var x = function () {} /b/g; x'); });
t('T24f2 await / b / g and ; in a script', function () { return ev('var await = 6, b = 2, g = 3; await /b/g;'); });
t('T24f3 await / b / g and the end of input in a script', function () { return ev('var await = 6, b = 2, g = 3; await /b/g'); });
