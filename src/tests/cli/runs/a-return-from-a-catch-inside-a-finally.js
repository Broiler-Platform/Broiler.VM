// A REGRESSION FIXTURE, and the one whose failure a stack-height check could never have seen.
//
// Every unwinding path emitted `PopScope` and decremented the compiler's depth counter while
// leaving the compile-time SCOPE pointing at the block it had just discarded. So a name the
// finaliser read on the way out was resolved one hop too far, and the finaliser read whatever
// slot of that index the grandparent environment happened to hold. The artifact compiled, it
// verified, and it answered the wrong value.
//
// It needs the enclosing function: at script level the outer name is a property of the global
// object, which is reached by name rather than by hops, so the mis-resolution cannot show.
// Reverting the repair puts this file back to a TypeError on `seen`.
function outer() {
  var seen = [];

  function inner() {
    try {
      throw new RangeError("thrown");
    } catch (error) {
      return "caught";
    } finally {
      seen.push("finalised");
    }
  }

  return inner() + "/" + seen.join(",");
}

outer();
