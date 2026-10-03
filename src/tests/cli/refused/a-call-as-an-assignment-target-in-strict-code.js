// IN STRICT CODE A CALL IS NOT AN ASSIGNMENT TARGET AT ALL, and the program is refused before it
// runs; Annex B's run-time error is for non-strict code only (JSP-7, JSC-239).
'use strict';

function f() {
  return {};
}

f() = 1;
