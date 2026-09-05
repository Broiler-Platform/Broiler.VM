// Renaming, a default for an absent property, a rest property, an elision, a default for an
// element the iterator never reached, and a rest element that finds nothing left.
var { a, b: renamed, missing = 5, ...rest } = { a: 1, b: 2, c: 3, d: 4 };
var [first, , third = 9, ...tail] = [10, 20];

a + " " + renamed + " " + missing + " " + JSON.stringify(rest) + " " + first + " " + third +
  " " + tail.length;
