// A REST PARAMETER IS THE LAST PARAMETER, and a comma after it - even the trailing one an
// ordinary parameter list may end with - is a syntax error. Until 2026-09-29 this was answered as
// "`)` was expected".
function collect(...rest, last) {
  return rest;
}
