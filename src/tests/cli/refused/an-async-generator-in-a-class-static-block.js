// THE SECOND NEW POSITION, and it is a statement position rather than an expression one. A static
// block's body is a statement list this front end had nowhere to put until the block was admitted,
// so a declaration inside one goes down a path no earlier bundle exercised. What must not happen is
// the refusal turning into a surprise token about the block.
class Registered {
  static {
    async function* pump() {
      yield 1;
    }
  }
}
