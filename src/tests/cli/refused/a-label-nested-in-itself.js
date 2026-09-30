// A LABEL ALREADY IN FORCE MAY NOT BE DECLARED AGAIN INSIDE IT. Until 2026-09-30 this program ran,
// and `break outer` left the inner statement rather than the outer.
outer: for (var row = 0; row < 2; row++) {
  outer: for (var column = 0; column < 2; column++) {
    break outer;
  }
}
