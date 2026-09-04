// The catch parameter is a binding position like any other, so it takes a pattern - and a thrown
// value that is not an Error destructures the same way.
var seen;

try {
  null.x;
} catch ({ name, message }) {
  seen = name + "/" + (message.length > 0);
}

try {
  throw [1, 2];
} catch ([a, b]) {
  seen = seen + " " + (a + b);
}

seen;
