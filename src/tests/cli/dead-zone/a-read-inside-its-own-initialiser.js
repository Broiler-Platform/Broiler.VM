// The binding is not initialised until its own initialiser has produced a value, so a
// reference to it inside that initialiser is in the dead zone.
let x = x + 1;
