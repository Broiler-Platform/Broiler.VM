// Both forms go through the iteration protocol, which is why a String spreads into characters
// and why a hole in the source array arrives as `undefined` rather than staying a hole.
function three(a, b, c) {
  return a + "/" + b + "/" + c;
}

var pair = [1, 2];

[...pair, 3].join(",") + " " + three(...pair, 3) + " " + [..."ab"].join(",") + " " +
  [...[1, , 3]].length;
