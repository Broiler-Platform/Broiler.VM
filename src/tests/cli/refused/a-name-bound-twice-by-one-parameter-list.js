// A NAME MAY APPEAR TWICE IN A PARAMETER LIST ONLY IN THE ONE CASE THE WEB WAS WRITTEN WITH: a
// plain sloppy function whose parameters are plain names. This list has a DEFAULT in it, so it is
// not simple, and the duplicate is refused in sloppy code as it is in strict - because the
// arguments object can no longer be the mapped one that gave the legacy behaviour its meaning.
function f(a, a = 1) {
  return a;
}
