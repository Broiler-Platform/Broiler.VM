// STRICT CODE HAS NO LEGACY OCTAL ESCAPE, and neither `\8` nor `\9`: each is an early SyntaxError
// there. The directive is inside the function, so the refusal follows the strictness a body imposes
// on itself; the same escape outside it is Annex B's, and the fixture beside this one runs it.
function strictHere() {
  'use strict';
  return '\101';
}
