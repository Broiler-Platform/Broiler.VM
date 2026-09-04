// `super` is admitted and this is not a manifest refusal: the manifest has `super`, and
// the program wrote it where the language gives it nothing to start from. The diagnostic
// CODE is what tells a conformance runner which of the two happened, so this one must be a
// parse error and never 2104.
function reach() {
  return super.missing;
}
