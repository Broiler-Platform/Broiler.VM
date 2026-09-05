// A `catch` PARAMETER IS A BINDING OF ITS OWN SCOPE AND THE HANDLER MAY NOT DECLARE IT AGAIN.
// The `var` form of the same file is a PROGRAM - the specification's web-compatibility annex keeps
// `try {} catch (e) { var e; }` working - so the case pinned here is the lexical one, which no
// annex reaches.
try {
  throw new Error("caught");
} catch (e) {
  let e = 1;
}
