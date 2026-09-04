// Object spread copies OWN ENUMERABLE properties and does NOT use the iteration protocol, so a
// later entry overwrites an earlier one and a nullish source contributes nothing at all rather
// than throwing.
var base = { a: 1, b: 2 };

JSON.stringify({ ...base, b: 3, c: 4 }) + " " + JSON.stringify({ ...null });
