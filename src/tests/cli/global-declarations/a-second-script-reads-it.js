// THE SECOND SCRIPT. It reads the first one's `let`, which no property of the global object shows.
// Not a program by itself: run alone it can only throw a ReferenceError.
print(shared + "," + typeof globalThis.shared);
