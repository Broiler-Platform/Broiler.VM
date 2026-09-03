// The ordinary case, pinned so a dead-zone check that fired too eagerly is caught: once the
// initialiser has run, both reading and writing are unremarkable.
let x = 1;
x = x + 4;
x;
