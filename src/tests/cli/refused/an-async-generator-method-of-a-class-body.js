// THE SAME REFUSAL AT MEMBER POSITION, where an async method now parses. A class body reaches its
// own branch, an object literal reaches another, and each has to test the `*` before it commits to
// parsing - so this needs a row of its own beside the declaration's, for the reason every other
// class-body construct does.
class Feed {
  async *pages() {
    yield 1;
  }
}
