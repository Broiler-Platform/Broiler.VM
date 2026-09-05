// A dependency of the module-goal probe, reached only by a dynamic import and never by a static
// one. It does not parse, which is the other half of the pair beside `no-such-export.mjs`: both
// reject the promise with a `SyntaxError`, and a host that told them apart there would be telling
// the guest which of its own passes refused.
var q = ;
