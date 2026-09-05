// The binding a named class expression gives its own body is CONSTANT, and an assignment to it
// is a run-time TypeError rather than an early error. This host answered it at compile time under
// 2204 until JSW-8, which is the divergence that row recorded; the repair is `ThrowImmutable`, and
// what it buys is exactly this program: the assignment is reachable code that throws when it runs
// and is not a refusal of a program that may never reach it.
//
// The binding is still there afterwards, and that is the second half of the answer: a refusal that
// merely discarded the store would have left `Inner` readable and the program none the wiser.
var Held = class Inner {
  rename() {
    Inner = 1;
  }
  static kind() {
    return typeof Inner;
  }
};

var held = new Held();
var caught = "none";

try {
  held.rename();
} catch (error) {
  caught = error.constructor.name;
}

caught + " " + Held.kind() + " " + (Held === Held);
