// THE TEMPORAL DEAD ZONE EXISTS IN A FUNCTION BODY TOO, and `typeof` does not step around it: the
// name is declared, so `typeof` reads it, and the read is the ReferenceError.
function early() {
  return typeof later;
  let later = 1;
}

early();
