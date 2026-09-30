// A PARAMETER LIST IS STRICT CODE WHEN THE BODY AFTER IT CARRIES THE DIRECTIVE, so `eval` is refused
// as a parameter name here although the parameters were read before the directive was.
function strictAfterwards(eval) {
  'use strict';
  return eval;
}
