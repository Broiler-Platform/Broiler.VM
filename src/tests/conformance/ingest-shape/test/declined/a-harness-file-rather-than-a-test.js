// A source file a suite ships beside its tests: an assertion library, or a module fixture another
// test imports. It carries no metadata block, so it is not a test, and this harness counts it and
// declines it rather than refusing the suite it was handed.
var assertionLibraryWouldGoHere = 1;
