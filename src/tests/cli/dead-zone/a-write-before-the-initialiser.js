// The write half. Assigning to a lexical binding before its initialiser has run is a
// ReferenceError too, and the language throws it where PutValue happens - AFTER the
// right-hand side has been evaluated - which is where the guard is emitted.
x = 1; let x;
