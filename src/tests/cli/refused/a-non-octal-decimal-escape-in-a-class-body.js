// A CLASS BODY IS STRICT CODE WHATEVER SURROUNDS IT, so `\8` in a method's string is refused here
// though the file around the class is sloppy.
class Escapes {
  eight() {
    return '\8';
  }
}
