// A KNOWN DEFECT, PINNED SO THE FIX IS DETECTABLE. Ordinary JavaScript: the loop runs its
// body once and leaves. The lowering emits the update and the back-edge anyway, and nothing
// reaches them, so this profile own verifier refuses the artifact as 1411:UnreachableCode.
// Thirteen files of test262 fail exactly this way - every one of them for(...) { break; }.
//
// WHEN THIS IS FIXED THIS FILE MUST START RUNNING and its row in expected.txt has to move
// from exit 4 to exit 0. That is the point of pinning it: the suite goes red on the repair,
// which is how a characterisation case tells you the defect is gone.
for (var i = 0; i < 3; i = i + 1) {
  break;
}
