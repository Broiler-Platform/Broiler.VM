// A CLASS BODY IS STRICT CODE WHATEVER ENCLOSES IT, so the same refusal that answers a `"use
// strict"` directive answers here - and nothing in this file says `strict` anywhere. It is the
// second of the three ways code becomes strict, and it is the one a reader is least likely to
// remember: the file's prologue is empty, the enclosing script is sloppy, and the method is not.
class Holder {
  read(source) {
    with (source) {
      return anything;
    }
  }
}
