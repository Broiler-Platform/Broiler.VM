// A PRIVATE NAME BELONGS TO A CLASS BODY AND IS NOT A PROPERTY KEY. The grammar admits `#m` in a
// class body and after a `.`, and nowhere else - so an object literal that spells one has written a
// production the language does not have, in any of the five forms an entry may take.
//
// It holds INSIDE a class as much as outside one: the private names a class body binds belong to
// the class, and an object literal written in a field's initialiser is still an object literal.
// `runs/the-keys-an-object-literal-still-admits.js` holds the keys that remain, the class members
// that are the real home of a private name among them.
var o = {
  #m() { return 1; },
};
