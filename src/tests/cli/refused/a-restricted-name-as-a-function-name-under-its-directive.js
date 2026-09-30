// THE RESTRICTED-NAME RULE REACHES A FUNCTION's OWN NAME, and a function whose body carries the
// directive is strict code for its name as well as its body: `arguments` is not a function name
// there, though the file around it is sloppy.
function arguments() {
  'use strict';
}
