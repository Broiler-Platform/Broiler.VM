// VM-FIX-D: a completion value that is a Symbol is printed the way String(symbol) prints it.
// ToString of a Symbol throws, and the host used to report an uncaught TypeError the program
// never threw. The value was taken from the comparison engine (`node -p`).
var described = Symbol("x");
described;
