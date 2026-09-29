// UNDER --slice A CLASS STATIC BLOCK IS REFUSED FOR THE CLASS, which is the construct the slice
// manifest does not admit. Until 2026-09-29 the slice front end took `static` as a modifier and `{`
// as the key after it, read the block's statements as further members, and refused the program as
// an unexpected brace - a token, not a construct. The wide surface runs this file.
class Registry {
  static {
    this.ready = true;
  }
}
