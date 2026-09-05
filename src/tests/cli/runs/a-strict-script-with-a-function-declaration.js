"use strict";

// The declaration and the write are two steps in the specification for a reason: the binding
// exists before anything assigns to it. A host that only wrote it worked until strict code was
// forbidden from creating a global by assigning to one.
function declared() {
  return 3;
}

print("declared " + declared());
