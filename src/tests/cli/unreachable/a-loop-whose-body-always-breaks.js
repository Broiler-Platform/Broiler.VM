// PINNED AS A DEFECT AND NOW REPAIRED, which is what pinning it was for. This row was
// declared with exit 4 - the code that accuses this component - because the lowering
// emitted the update and the back-edge even though a body that always breaks reaches
// neither, and the verifier refused the artifact as 1411:UnreachableCode. Thirteen files
// of test262 were exactly this shape. The suite went red on the repair and the row moved
// to exit 0.
//
// It stays here rather than moving to runs/ so that the shape keeps a case of its own: a
// regression in the reachability analysis would put this back to 4, and a reader looking
// for why would find this comment rather than a bare arithmetic fixture.
for (var i = 0; i < 3; i = i + 1) {
  break;
}
